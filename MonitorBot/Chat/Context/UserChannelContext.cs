namespace MonitorBot.Chat.Wrapper.Chat.Context;

public record UserChannelContext(string User, string Channel, string Guild, ChatWrapper Chat)
	: ChannelContext(Channel, Guild, Chat)
{
	public static implicit operator UserContext(UserChannelContext userChannel)
	{
		return new UserContext(userChannel.User, userChannel.Guild, userChannel.Chat);
	}
}