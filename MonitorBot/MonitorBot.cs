using MonitorBot.Utils;
using Kook;
using Kook.Commands;
using Kook.WebSocket;
using MonitorBot.Chat.Wrapper.Chat;
using MonitorBot.Chat.Wrapper.Chat.Context;
using System.Reflection;
using MonitorBot.DataStorage;
using MonitorBot.Chat;
using System.Text;

namespace MonitorBot
{
    public class MonitorBot
    {
        public TaskExecutor TaskExecutor { get; } = new();
        public static readonly MonitorBot Instance = new();
        public readonly KookSocketClient client;
        public static ulong GuildId;
        public ChatWrapper? Chat { get; }
        public CommandService commandService = new
        (new()
        {
            DefaultRunMode = RunMode.Async,
            IgnoreExtraArgs = true
        });

        public MonitorBot()
        {
            AppDomain.CurrentDomain.UnhandledException += this.OnUnhandledException;
            this.client = new KookSocketClient
            (
                new()
                {
                    AlwaysDownloadVoiceStates = true,
                    AlwaysDownloadUsers = true
                }
            );
            Chat = new KookChatWrapper(client);
        }

        public async Task Init()
        {
            await commandService.AddModulesAsync(Assembly.GetEntryAssembly()!, null!);
            await Data.Init();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _ = ulong.TryParse((string)Data.GetTomlValue(Data.MainConfig, "MainGuildId"), out GuildId);
        }

        public async Task MainAsync()
        {
            this.client.Log += LogAsync;
            this.client.MessageReceived += MessageReceived;
            this.client.DirectMessageReceived += DirectMessageReceived;
            this.client.Ready += () => { Logger.Info("Bot is Successful Started!"); return Task.CompletedTask; };
            await this.Init();

            await this.client.LoginAsync(TokenType.Bot, (string)await Data.GetTomlValueAsync(Data.MainConfig, "Kook_Token"));
            await this.client.StartAsync();
            await Task.Delay(Timeout.Infinite);
        }

        private static Task LogAsync(LogMessage Log)
        {
            if (Log.Exception is CommandException CmdException)
            {
                Logger.Error($"[Command/{Log.Severity}] {CmdException.Command.Aliases[0]} failed to execute in {CmdException.Context.Channel}.");
                Logger.Error(CmdException);
            }
            else Logger.Message(Utils.ColorType.White, "Kook API", Log.ToString());

            return Task.CompletedTask;
        }

        private async Task MessageReceived(SocketMessage message, SocketGuildUser User, SocketTextChannel Channel)
        {
            await Task.Yield();
            ChannelContext channelcontext = new(Channel.Id.ToString(), Channel.Guild.Id.ToString(), Chat!);
            string TryParseCard = JsonManager.TryGetKookCardContent(message.Content);
            if (message.Author.Id == this.client.CurrentUser!.Id && TryParseCard != null)
            {
                Logger.Message(Utils.ColorType.Green, "Chat Thread", $"[{Channel.Guild.Name}] [{message.Channel.Name}] [Bot发出卡片] {TryParseCard}");
                return;
            }
            if (message.Author.Id == this.client.CurrentUser!.Id && TryParseCard == null)
            {
                Logger.Message(Utils.ColorType.Green, "Chat Thread", $"[{Channel.Guild.Name}] [{message.Channel.Name}] [Bot发出消息] {message.Content}");
                return;
            }
            if (TryParseCard != null)
            {
                Logger.Message(Utils.ColorType.DarkAqua, "Chat Thread", $"(JsonText) [{Channel.Guild.Name}] [{message.Channel.Name}] {message.Author.Username} ({message.Author.Id}): {TryParseCard}");
                goto CONTINUE;
            }
            Logger.Message(Utils.ColorType.Aqua, "Chat Thread", $"[{Channel.Guild.Name}] [{message.Channel.Name}] {message.Author.Username} ({message.Author.Id}): {message.Content}");
        CONTINUE:
            if (message is not SocketUserMessage userMessage) return;
            if (message.Author.IsBot ?? false) return;
            if (message.Type is MessageType.KMarkdown or MessageType.Text)
            {
                TaskExecutor.Execute(async () =>
                {
                    try
                    {
                        SocketCommandContext context = new(this.client, userMessage);
                        int argPos = 0;
                        if (userMessage.HasCharPrefix('=', ref argPos))
                        {
                            IResult result = await this.commandService.ExecuteAsync(context, argPos, null!, MultiMatchHandling.Best);
                            Logger.Info(Utils.ColorType.Gray, "Command Execution", $"Result: {result.ErrorReason}");
                            switch (result.Error)
                            {
                                case CommandError.UnknownCommand: break;
                                case CommandError.ParseFailed:
                                    await Channel.SendFailedCard("指令解析失败！").Catch();
                                    break;
                                case CommandError.BadArgCount:
                                    await Channel.SendFailedCard("参数数量错误！").Catch();
                                    break;
                                case CommandError.ObjectNotFound:
                                    await Channel.SendFailedCard("参数错误！").Catch();
                                    break;
                                case CommandError.UnmetPrecondition:
                                    await Channel.SendFailedCard("权限不足！").Catch();
                                    break;
                                case CommandError.MultipleMatches:
                                    await Channel.SendTextAsync("内部错误：多个指令被匹配").Catch();
                                    break;
                                case CommandError.Exception:
                                    await Channel.SendTextAsync("内部错误：运行过程中出现异常").Catch();
                                    break;
                                case CommandError.Unsuccessful:
                                    await Channel.SendTextAsync("指令运行失败").Catch();
                                    break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error("处理消息过程中发生未知错误\n" + e + "\n此错误不会导致程序退出，但请及时排除故障");
                    }
                });
            }
        }
        private async Task DirectMessageReceived(SocketMessage message, SocketUser socketUser, SocketDMChannel channel)
        {
            await Task.Yield();
            if (message.Author.Id == this.client.CurrentUser!.Id)
            {
                Logger.Message(Utils.ColorType.Blue, "Direct Chat Thread", $"{message.Author.Username} ({message.Author.Id}) {message.Content}");
                return;
            }
            Logger.Message(Utils.ColorType.Blue, "Direct Chat Thread", $"{message.Author.Username} ({message.Author.Id}) {message.Content}");
            if (message is not SocketUserMessage userMessage) return;
            if (message.Author.IsBot ?? false) return;
            if (message.Type is MessageType.KMarkdown or MessageType.Text)
            {
                TaskExecutor.Execute
                (
                    async () => await Task.Run(async () =>
                    {
                        await Task.Yield();
                        try { }
                        catch (Exception e)
                        {
                            Logger.Error("处理消息过程中发生未知错误\n" + e + "\n此错误不会导致程序退出，但请及时排除故障");
                        }
                    })
                );
            }
        }
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            string error = "运行过程中发生未知错误\n" + ex.Message + "\n" + ex.StackTrace + "\nCaused by : " + ex.Source;
            if (e.IsTerminating) error += "\n此异常无法被内部处理，程序正在退出";
            else error += "\n程序仍旧可以运行，但是可能出现未知问题，建议手动关闭查找故障";
            Logger.Error(error);
        }
    }
}
