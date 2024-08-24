namespace MonitorBot.Chat.Wrapper.Chat.Context;

public record ImageMessageContext(
	string Url,
	string Message,
	string User,
	string Channel,
	string Guild,
	ChatWrapper Chat)
	: MessageContext(Message, User, Channel, Guild, Chat);