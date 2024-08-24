namespace MonitorBot.Chat.Wrapper.Chat.Context;

public record ChannelContext(string Channel, string Guild, ChatWrapper Chat) : GuildContext(Guild, Chat);