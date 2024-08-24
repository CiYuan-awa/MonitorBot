namespace MonitorBot.Chat.Wrapper.Chat.Context;

public record UserContext(string User, string Guild, ChatWrapper Chat) : GuildContext(Guild, Chat)
{
	public static implicit operator VirtualRoleContext(UserContext user)
	{
		return new VirtualRoleContext(user.Guild, user.Chat, user.User);
	}
}