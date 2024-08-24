namespace MonitorBot.Chat.Wrapper.Chat.Context;

public record RoleContext(string Role, string Guild, ChatWrapper Chat) : GuildContext(Guild, Chat)
{
	public static implicit operator VirtualRoleContext(RoleContext role)
	{
		return new VirtualRoleContext(role.Guild, role.Chat, null, role.Role);
	}
}