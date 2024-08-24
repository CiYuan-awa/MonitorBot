using MonitorBot.Chat.Wrapper.Chat.Context;

namespace MonitorBot.Chat.Wrapper.Chat;

public abstract class ChatWrapper
{
	public delegate Task CardButtonHandler(string button, UserContext user, MessageContext message);

	public delegate Task ImageMessageHandler(ImageMessageContext message);

	public delegate Task MessageHandler(string content, MessageContext message);


	public delegate Task TextMessageHandler(TextMessageContext message);

	public event MessageHandler OnMessage = (_, _) => Task.CompletedTask;

	public event TextMessageHandler OnTextMessage = _ => Task.CompletedTask;

	public event ImageMessageHandler OnImageMessage = _ => Task.CompletedTask;

	public event CardButtonHandler OnCardButton = (_, _, _) => Task.CompletedTask;

	protected async Task CallOnMessage(string content, MessageContext message)
	{
		await OnMessage(content, message);
	}

	protected async Task CallOnTextMessage(TextMessageContext message)
	{
		await OnTextMessage(message);
	}

	protected async Task CallOnImageMessage(ImageMessageContext message)
	{
		await OnImageMessage(message);
	}

	protected async Task CallOnCardButton(string button, UserContext user, MessageContext message)
	{
		await OnCardButton(button, user, message);
	}

	public abstract Task Login();

	public abstract string Escape(string text);

	public abstract string AtUser(string user);

	public abstract string AtRole(string role);

	public abstract string AtOnline();

	public abstract string AtAll();

	public abstract string AtChannel(string channel);

	public abstract string Bold(string text);

	public abstract string Italic(string text);

	public abstract string Underline(string text);

	public abstract string Strike(string text);

	public abstract string Quote(string text);

	public abstract string CodeLine(string text);

	public abstract string CodeBlock(string text, string language);

	public abstract string Link(string text, string link);

	public abstract string Separate();

	public abstract string Hide(string text);

	public abstract object MakeCard
		(CardTheme theme, IEnumerable<object> elements, Dictionary<object, object?>? options = null);

	public abstract object CardTitle
		(string text, Dictionary<object, object?>? options = null);

	public abstract object CardText
		(string text, Dictionary<object, object?>? options = null);

	public abstract object CardButtonLine
		(IEnumerable<CardButton> buttons, Dictionary<object, object?>? options = null);

	public abstract object CardTimer
		(bool barMode, DateTimeOffset? fromTime, DateTimeOffset toTime, Dictionary<object, object?>? options = null);

	public abstract object CardTable
		(IEnumerable<IEnumerable<string>> table, Dictionary<object, object?>? options = null);

	public abstract object CardFile
		(string url, string name, Dictionary<object, object?>? options = null);

	public abstract Task<string> UploadFile(byte[] content, string name);

	public abstract Task<string> GetUserIdentifier(string user);

	public abstract Task SendTextMessage
		(ChannelContext channel, string message, MessageContext? reply = null, MessageContext? modify = null);

	public abstract Task SendCardMessages
	(ChannelContext channel, IEnumerable<object> cards, MessageContext? reply = null,
		MessageContext? modify = null);

	public abstract Task AddReaction(MessageContext message, string reaction);

	public abstract Task<ChannelContext> CreateChannelCategory(GuildContext guild, string name);

	public abstract Task<ChannelContext> CreateTextChannel(GuildContext guild, string name,
		ChannelContext? category = null);

	public abstract Task<ChannelContext> CreateVoiceChannel(GuildContext guild, string name, int limit,
		ChannelContext? category = null);

	public abstract Task<ChannelContext?> GetChannelCategory(ChannelContext channel, string name);

	public abstract Task<string> GetChannelName(ChannelContext channel, string name);

	public abstract Task<int> GetVoiceChannelLimit(ChannelContext channel, string name);

	public abstract Task ModifyChannelName(ChannelContext channel, string name);

	public abstract Task ModifyVoiceChannelLimit(ChannelContext channel, int limit);

	public abstract Task ModifyChannelPermission(ChannelContext channel, VirtualRoleContext virtualRole,
		IEnumerable<string> allow, IEnumerable<string> deny, IEnumerable<string> inherit);

	public abstract Task DeleteChannelPermission(ChannelContext channel, VirtualRoleContext virtualRole);

	public abstract Task DeleteChannel(ChannelContext channel);

	public abstract Task<List<RoleContext>> ListUserRoles(UserContext user);

	public abstract Task AddUserRoles(UserContext user, IEnumerable<string> roles);

	public abstract Task DeleteUserRoles(UserContext user, IEnumerable<string> roles);
}