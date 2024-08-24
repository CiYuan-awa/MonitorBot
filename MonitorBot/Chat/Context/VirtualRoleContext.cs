namespace MonitorBot.Chat.Wrapper.Chat.Context;

/*
 * an abstraction for User and Role
 *
 * VirtualRole:
 *	 User: user#USERID
 *	 Role: ROLEID
 */
public record VirtualRoleContext(
	string Guild,
	ChatWrapper Chat,
	string? User = null,
	string? Role = null) : GuildContext(Guild, Chat)
{
	public VirtualRoleContext(string virtualRole, string guild, ChatWrapper chat) :
		this(guild, chat,
			virtualRole.StartsWith("user#") ? virtualRole[5..] : null,
			virtualRole.StartsWith("user#") ? null : virtualRole)
	{
	}

	public string VirtualRole =>
		Role != null ? Role :
		User != null ? $"user#{User}" :
		throw new Exception("Virtual role objects must hold either a role or a user.");

	public bool IsUser => User != null;

	public static implicit operator UserContext(VirtualRoleContext virtualRole)
	{
		return new UserContext(virtualRole.User!, virtualRole.Guild, virtualRole.Chat);
	}

	public static implicit operator RoleContext(VirtualRoleContext virtualRole)
	{
		return new RoleContext(virtualRole.VirtualRole, virtualRole.Guild, virtualRole.Chat);
	}
}