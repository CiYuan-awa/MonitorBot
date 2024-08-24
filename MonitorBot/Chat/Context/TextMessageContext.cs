namespace MonitorBot.Chat.Wrapper.Chat.Context;

public record TextMessageContext(
	string Content,
	string Message,
	string User,
	string Channel,
	string Guild,
	ChatWrapper Chat)
	: MessageContext(Message, User, Channel, Guild, Chat);