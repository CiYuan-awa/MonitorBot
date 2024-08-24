namespace MonitorBot.Chat.Wrapper.Chat.Context;

public record GuildContext(string Guild, ChatWrapper Chat) : ChatContext(Chat);