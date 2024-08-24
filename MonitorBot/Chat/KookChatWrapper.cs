using Kook;
using Kook.WebSocket;
using MonitorBot.Chat.Wrapper.Chat.Context;

namespace MonitorBot.Chat.Wrapper.Chat;

public class KookChatWrapper : ChatWrapper
{
    private readonly IKookClient _kook;
    private readonly KookSocketClient? _kookSocket;

    public KookChatWrapper(KookSocketClient client)
    {
        _kookSocket = client;
        _kook = client;
    }

    public override async Task Login() { await Task.Yield(); }

    public override string Escape(string text)
    {
        return text
            .Replace("\\", @"\\")
            .Replace("*", "\\*")
            .Replace("~", "\\~")
            .Replace("[", "\\[")
            .Replace("]", "\\]")
            .Replace("-", "\\-")
            .Replace(">", "\\>")
            .Replace("(", "\\(")
            .Replace(")", "\\)")
            .Replace(":", "\\:")
            .Replace("`", "\\`");
    }

    public override string AtUser(string user)
    {
        return $"(met){user}(met)";
    }

    public override string AtRole(string role)
    {
        if (role.StartsWith("user#")) return AtUser(role[5..]);
        return $"(rol){role}(rol)";
    }

    public override string AtOnline()
    {
        return "(rol)here(rol)";
    }

    public override string AtAll()
    {
        return "(rol)all(rol)";
    }

    public override string AtChannel(string channel)
    {
        return $"(chn){channel}(chn)";
    }

    public override string Bold(string text)
    {
        return $"**{Escape(text)}**";
    }

    public override string Italic(string text)
    {
        return $"*{Escape(text)}*";
    }

    public override string Underline(string text)
    {
        return $"(ins){Escape(text)}(ins)";
    }

    public override string Strike(string text)
    {
        return $"~~{Escape(text)}~~";
    }

    public override string Quote(string text)
    {
        return $"> {Escape(text)}\n\n";
    }

    public override string CodeLine(string text)
    {
        return $"`{Escape(text)}`";
    }

    public override string CodeBlock(string text, string language)
    {
        return $"```{language}\n{Escape(text)}```\n";
    }

    public override string Link(string text, string link)
    {
        return $"[{Escape(text)}]({link})";
    }

    public override string Separate()
    {
        return "---\n";
    }

    public override string Hide(string text)
    {
        return $"(spl){Escape(text)}(spl)";
    }

    private Kook.CardTheme ToKookCardTheme(CardTheme theme)
    {
        return theme switch
        {
            CardTheme.Primary => Kook.CardTheme.Primary,
            CardTheme.Secondary => Kook.CardTheme.Secondary,
            CardTheme.Info => Kook.CardTheme.Info,
            CardTheme.Success => Kook.CardTheme.Success,
            CardTheme.Warning => Kook.CardTheme.Warning,
            CardTheme.Danger => Kook.CardTheme.Danger,
            _ => Kook.CardTheme.Primary
        };
    }

    private ButtonTheme ToKookButtonTheme(CardTheme theme)
    {
        return theme switch
        {
            CardTheme.Primary => ButtonTheme.Primary,
            CardTheme.Secondary => ButtonTheme.Secondary,
            CardTheme.Info => ButtonTheme.Info,
            CardTheme.Success => ButtonTheme.Success,
            CardTheme.Warning => ButtonTheme.Warning,
            CardTheme.Danger => ButtonTheme.Danger,
            _ => ButtonTheme.Primary
        };
    }

    public override object MakeCard(CardTheme theme, IEnumerable<object> elements,
        Dictionary<object, object?>? options = null)
    {
        var cardBuilder = new CardBuilder()
            .WithSize(CardSize.Large)
            .WithTheme(ToKookCardTheme(theme));
        elements.ToList().ForEach(element => cardBuilder.AddModule((IModuleBuilder)element));
        return cardBuilder.Build();
    }

    public override object CardTitle(string text, Dictionary<object, object?>? options = null)
    {
        return new HeaderModuleBuilder(text);
    }

    public override object CardText(string text, Dictionary<object, object?>? options = null)
    {
        return new SectionModuleBuilder(text, true);
    }

    public override object CardButtonLine(IEnumerable<CardButton> buttons, Dictionary<object, object?>? options = null)
    {
        var group = new ActionGroupModuleBuilder();
        buttons.ToList().ForEach(button =>
        {
            group.AddElement(new ButtonElementBuilder(
                button.Text,
                ToKookButtonTheme(button.Theme),
                button.Value,
                button.IsLink ? ButtonClickEventType.Link : ButtonClickEventType.ReturnValue
            ));
        });
        return group;
    }

    public override object CardTimer(bool barMode, DateTimeOffset? fromTime, DateTimeOffset toTime,
        Dictionary<object, object?>? options = null)
    {
        return new CountdownModuleBuilder(
            barMode ? CountdownMode.Second : CountdownMode.Day,
            toTime, fromTime
        );
    }

    public override object CardTable(IEnumerable<IEnumerable<string>> table,
        Dictionary<object, object?>? options = null)
    {
        var paragraph = new ParagraphStructBuilder();
        var columns = 0;
        foreach (var line in table)
        {
            var size = 0;
            foreach (var element in line)
            {
                paragraph.AddField(new KMarkdownElementBuilder(element));
                ++size;
            }

            if (columns == 0)
                columns = size;
            else if (columns != size) throw new Exception("Table column sizes do not match!");
        }

        return paragraph.WithColumnCount(columns);
    }

    public override object CardFile(string url, string name, Dictionary<object, object?>? options = null)
    {
        return new FileModuleBuilder(url, name);
    }

    public override async Task<string> UploadFile(byte[] content, string name)
    {
        using var stream = new MemoryStream(content);
        return await _kookSocket!.Rest.CreateAssetAsync(stream, name);
    }

    public override async Task<string> GetUserIdentifier(string user)
    {
        var kookUser = await _kook.GetUserAsync(ulong.Parse(user));
        return kookUser!.UsernameAndIdentifyNumber(false);
    }

    public override async Task SendTextMessage(ChannelContext channel, string message, MessageContext? reply = null,
        MessageContext? modify = null)
    {
        var textChannel = await GetTextChannel(channel);
        if (modify == null)
            await textChannel.SendTextAsync(
                message,
                reply == null ? null : new MessageReference(Guid.Parse(reply.Message))
            );
        else
            await textChannel.ModifyMessageAsync(
                Guid.Parse(modify.Message),
                msg => { msg.Content = message; }
            );
    }

    public override async Task SendCardMessages(ChannelContext channel, IEnumerable<object> cards,
        MessageContext? reply = null, MessageContext? modify = null)
    {
        var textChannel = await GetTextChannel(channel);
        if (modify == null)
            await textChannel.SendCardsAsync(
                cards.Select(card => (ICard)card),
                reply == null ? null : new MessageReference(Guid.Parse(reply.Message))
            );
        else
            await textChannel.ModifyMessageAsync(
                Guid.Parse(modify.Message),
                msg => { msg.Cards = cards.Select(card => (ICard)card); }
            );
    }

    public override async Task AddReaction(MessageContext message, string reaction)
    {
        var textChannel = await GetTextChannel(message);
        var kookMessage = await textChannel.GetMessageAsync(Guid.Parse(message.Message));
        await kookMessage!.AddReactionAsync(new Emoji(reaction));
    }

    public override async Task<ChannelContext> CreateChannelCategory(GuildContext guild, string name)
    {
        var kookGuild = (SocketGuild)await GetGuild(guild);
        var category = await kookGuild.CreateCategoryChannelAsync(name);
        await kookGuild.UpdateAsync();
        return new ChannelContext(category.Id.ToString(), guild.Guild, this);
    }

    public override async Task<ChannelContext> CreateTextChannel(GuildContext guild, string name,
        ChannelContext? category = null)
    {
        var kookGuild = (SocketGuild)await GetGuild(guild);
        var channel = await kookGuild.CreateTextChannelAsync(
            name,
            category == null ? null : prop => prop.CategoryId = ulong.Parse(category.Channel)
        );
        await kookGuild.UpdateAsync();
        return new ChannelContext(channel.Id.ToString(), guild.Guild, this);
    }

    public override async Task<ChannelContext> CreateVoiceChannel(GuildContext guild, string name, int limit,
        ChannelContext? category = null)
    {
        var kookGuild = (SocketGuild)await GetGuild(guild);
        var channel = await kookGuild.CreateVoiceChannelAsync(
            name,
            category == null
                ? null
                : prop =>
                {
                    prop.CategoryId = ulong.Parse(category.Channel);
                    prop.UserLimit = limit == int.MaxValue ? null : limit;
                }
        );
        await kookGuild.UpdateAsync();
        return new ChannelContext(channel.Id.ToString(), guild.Guild, this);
    }

    public override async Task<ChannelContext?> GetChannelCategory(ChannelContext channel, string name)
    {
        var kookChannel = await GetChannel(channel);
        if (kookChannel is not INestedChannel nestedChannel) return null;
        if (nestedChannel.CategoryId == null) return null;
        return new ChannelContext(
            nestedChannel.CategoryId.Value.ToString(),
            channel.Guild,
            this
        );
    }

    public override async Task<string> GetChannelName(ChannelContext channel, string name)
    {
        var kookChannel = await GetChannel(channel);
        return kookChannel.Name;
    }

    public override async Task<int> GetVoiceChannelLimit(ChannelContext channel, string name)
    {
        var kookChannel = await GetVoiceChannel(channel);
        return kookChannel.UserLimit == 0 ? int.MaxValue : kookChannel.UserLimit;
    }

    public override async Task ModifyChannelName(ChannelContext channel, string name)
    {
        var kookChannel = await GetGuildChannel(channel);
        await kookChannel.ModifyAsync(prop => prop.Name = name);
    }

    public override async Task ModifyVoiceChannelLimit(ChannelContext channel, int limit)
    {
        var kookChannel = await GetVoiceChannel(channel);
        await kookChannel.ModifyAsync(prop => prop.UserLimit = limit == int.MaxValue ? 0 : limit);
    }

    public override async Task ModifyChannelPermission(ChannelContext channel, VirtualRoleContext virtualRole,
        IEnumerable<string> allow, IEnumerable<string> deny, IEnumerable<string> inherit)
    {
        var kookChannel = (SocketGuildChannel)await GetGuildChannel(channel);
        var guild = await GetGuild(channel);
        var allowedPerms = allow.ToList();
        var deniedPerms = deny.ToList();
        var inheritedPerms = inherit.ToList();

        void Modify(string perm, Action<PermValue> setter)
        {
            var inAllow = allowedPerms.Contains(perm);
            var inDeny = deniedPerms.Contains(perm);
            var inInherit = inheritedPerms.Contains(perm);
            var value = inAllow ? PermValue.Allow : inDeny ? PermValue.Deny : PermValue.Inherit;
            if (inAllow || inDeny || inInherit) setter(value);
        }

        var func = (Func<OverwritePermissions, OverwritePermissions>)(perms =>
        {
            Modify("view", x => perms = perms.Modify(viewChannel: x));
            Modify("send_message", x => perms = perms.Modify(sendMessages: x));
            Modify("connect", x => perms = perms.Modify(connect: x));
            Modify("passive_connect", x => perms = perms.Modify(passiveConnect: x));
            Modify("speak", x => perms = perms.Modify(speak: x));
            Modify("create_invite", x => perms = perms.Modify(x));
            Modify("manage_message", x => perms = perms.Modify(manageMessages: x));
            Modify("manage_voice", x => perms = perms.Modify(manageVoice: x));
            Modify("manage_channel", x => perms = perms.Modify(manageChannels: x));
            Modify("manage_role", x => perms = perms.Modify(manageRoles: x));
            Modify("attach_file", x => perms = perms.Modify(attachFiles: x));
            Modify("at_everyone", x => perms = perms.Modify(mentionEveryone: x));
            Modify("add_reaction", x => perms = perms.Modify(addReactions: x));
            Modify("voice_activity", x => perms = perms.Modify(useVoiceActivity: x));
            Modify("deafen_members", x => perms = perms.Modify(deafenMembers: x));
            Modify("mute_members", x => perms = perms.Modify(muteMembers: x));
            Modify("play_music", x => perms = perms.Modify(playSoundtrack: x));
            Modify("share_screen", x => perms = perms.Modify(shareScreen: x));
            return perms;
        });
        if (virtualRole.IsUser)
        {
            var user = await guild.GetUserAsync(ulong.Parse(virtualRole.User!));
            if (user != null)
            {
                if (kookChannel.GetPermissionOverwrite(user) == null)
                {
                    await kookChannel.AddPermissionOverwriteAsync(user);
                    await kookChannel.UpdateAsync();
                }

                await kookChannel.ModifyPermissionOverwriteAsync(user, func);
            }
        }
        else
        {
            var role = guild.GetRole(uint.Parse(virtualRole.Role!));
            if (role != null)
            {
                if (kookChannel.GetPermissionOverwrite(role) == null)
                {
                    await kookChannel.AddPermissionOverwriteAsync(role);
                    await kookChannel.UpdateAsync();
                }

                await kookChannel.ModifyPermissionOverwriteAsync(role, func);
            }
        }
    }

    public override async Task DeleteChannelPermission(ChannelContext channel, VirtualRoleContext virtualRole)
    {
        var kookChannel = await GetGuildChannel(channel);
        var guild = await GetGuild(channel);
        if (virtualRole.IsUser)
        {
            var user = await guild.GetUserAsync(ulong.Parse(virtualRole.User!));
            if (user != null) await kookChannel.RemovePermissionOverwriteAsync(user);
        }
        else
        {
            var role = guild.GetRole(uint.Parse(virtualRole.Role!));
            if (role != null) await kookChannel.RemovePermissionOverwriteAsync(role);
        }
    }

    public override async Task DeleteChannel(ChannelContext channel)
    {
        var kookChannel = await GetGuildChannel(channel);
        await kookChannel.DeleteAsync();
    }

    public override async Task<List<RoleContext>> ListUserRoles(UserContext user)
    {
        var kookUser = (SocketGuildUser)await GetGuildUser(user);
        await kookUser.UpdateAsync();
        return kookUser.Roles
            .Select(role => new RoleContext(role.Id.ToString(), user.Guild, this))
            .ToList();
    }

    public override async Task AddUserRoles(UserContext user, IEnumerable<string> roles)
    {
        var kookUser = (SocketGuildUser)await GetGuildUser(user);
        await kookUser.UpdateAsync();
        await kookUser.AddRolesAsync(roles.Select(uint.Parse));
        await kookUser.UpdateAsync();
    }

    public override async Task DeleteUserRoles(UserContext user, IEnumerable<string> roles)
    {
        var kookUser = (SocketGuildUser)await GetGuildUser(user);
        await kookUser.UpdateAsync();
        await kookUser.RemoveRolesAsync(roles.Select(uint.Parse));
        await kookUser.UpdateAsync();
    }

    private async Task<IGuild> GetGuild(GuildContext guild)
    {
        return (await _kook.GetGuildAsync(ulong.Parse(guild.Guild)))!;
    }

    private async Task<IChannel> GetChannel(ChannelContext channel)
    {
        return (await _kook.GetChannelAsync(ulong.Parse(channel.Channel)))!;
    }

    private async Task<IGuildChannel> GetGuildChannel(ChannelContext channel)
    {
        return (await (await GetGuild(channel)).GetChannelAsync(ulong.Parse(channel.Channel)))!;
    }

    private async Task<ITextChannel> GetTextChannel(ChannelContext channel)
    {
        return (ITextChannel)await GetChannel(channel);
    }

    private async Task<IVoiceChannel> GetVoiceChannel(ChannelContext channel)
    {
        return (IVoiceChannel)await GetGuildChannel(channel);
    }

    private async Task<IUser> GetUser(UserContext user)
    {
        return (await _kook.GetUserAsync(ulong.Parse(user.User)))!;
    }

    private async Task<IGuildUser> GetGuildUser(UserContext user)
    {
        return (await (await GetGuild(user)).GetUserAsync(ulong.Parse(user.User)))!;
    }

    private async Task<IRole> GetRole(RoleContext role)
    {
        return (await GetGuild(role)).GetRole(uint.Parse(role.Role))!;
    }

    private async Task<ICategoryChannel> GetChannelCategory(ChannelContext channel)
    {
        return (ICategoryChannel)await GetChannel(channel);
    }
}