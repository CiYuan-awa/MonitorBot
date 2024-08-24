namespace MonitorBot.Chat.Wrapper.Chat.Context;

public record MessageContext(string Message, string User, string Channel, string Guild, ChatWrapper Chat)
	: UserChannelContext(User, Channel, Guild, Chat);