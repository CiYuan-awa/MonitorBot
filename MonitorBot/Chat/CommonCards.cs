using MonitorBot.Chat.Wrapper.Chat;
using MonitorBot.Chat.Wrapper.Chat.Context;
using MonitorBot.Utils;
using Kook.WebSocket;

namespace MonitorBot.Chat
{

    public static class CommonCards
    {
        private static readonly ChatWrapper Chat = MonitorBot.Instance.Chat!;

        public static async Task SendCustomCard(this SocketGuildChannel Channel, string Title, string ThingsToSay, CardTheme Theme)
        {
            ChannelContext channelcontext = new(Channel.Id.ToString(), Channel.Guild.Id.ToString(), Chat!);
            await Chat!.SendCardMessages(channelcontext,
            [
                Chat.MakeCard(
                Theme,
                [
                    Chat.CardTitle(Title),
                    Chat.CardText(ThingsToSay)
                ]
            )]).Catch();
        }

        public static async Task SendSucceededCard(this SocketGuildChannel Channel, string ThingsToSay)
        {
            ChannelContext channelcontext = new(Channel.Id.ToString(), Channel.Guild.Id.ToString(), Chat!);
            await Chat!.SendCardMessages(channelcontext,
            [
                Chat.MakeCard(
                CardTheme.Success,
                [
                    Chat.CardTitle("辞愿的小猫"),
                    Chat.CardText("\u2705 成功"),
                    Chat.CardText(ThingsToSay)
                ]
            )]).Catch();
        }

        public static async Task SendWaringCard(this SocketGuildChannel Channel, string ThingsToSay)
        {
            ChannelContext channelcontext = new(Channel.Id.ToString(), Channel.Guild.Id.ToString(), Chat!);
            await Chat!.SendCardMessages(channelcontext,
            [
                Chat.MakeCard(
                CardTheme.Warning,
                [
                    Chat.CardTitle("辞愿的小猫"),
                    Chat.CardText("\u26A0 警告"),
                    Chat.CardText(ThingsToSay)
                ]
            )]).Catch();
        }

        public static async Task SendFailedCard(this SocketGuildChannel Channel, string ThingsToSay)
        {
            ChannelContext channelcontext = new(Channel.Id.ToString(), Channel.Guild.Id.ToString(), Chat!);
            await Chat!.SendCardMessages(channelcontext,
            [
                Chat.MakeCard(
                CardTheme.Danger,
                [
                    Chat.CardTitle("辞愿的小猫"),
                    Chat.CardText(":x: 错误"),
                    Chat.CardText(ThingsToSay)
                ]
            )]).Catch();
        }

        public static async Task SendNormalCard(this SocketGuildChannel Channel, string ThingsToSay)
        {
            ChannelContext channelcontext = new(Channel.Id.ToString(), Channel.Guild.Id.ToString(), Chat!);
            await Chat!.SendCardMessages(channelcontext,
            [
                Chat.MakeCard(
                CardTheme.Info,
                [
                    Chat.CardTitle("辞愿的小猫"),
                    Chat.CardText(ThingsToSay)
                ]
            )]).Catch();
        }

        public static async Task SendListCard(this SocketGuildChannel Channel, string v1, string v2)
        {
            ChannelContext channelcontext = new(Channel.Id.ToString(), Channel.Guild.Id.ToString(), Chat!);
            await Chat!.SendCardMessages(channelcontext,
            [
                Chat.MakeCard(
                CardTheme.Info,
                [
                    Chat.CardTitle("辞愿的小猫"),
                    Chat.CardText(v1),
                    Chat.CardText(Chat.Quote(v2))
                ]
            )]).Catch();
        }

        public static async Task SendListCard(this SocketGuildChannel Channel, string v1, string v2, string v3, string v4)
        {
            ChannelContext channelcontext = new(Channel.Id.ToString(), Channel.Guild.Id.ToString(), Chat!);
            await Chat!.SendCardMessages(channelcontext,
            [
                Chat.MakeCard(
                CardTheme.Info,
                [
                    Chat.CardTitle("辞愿的小猫"),
                    Chat.CardText(v1),
                    Chat.CardText(Chat.Quote(v2)),
                    Chat.CardText(v3),
                    Chat.CardText(Chat.Quote(v4))
                ]
            )]).Catch();
        }
    }
}
