namespace MonitorBot.Chat.Wrapper.Chat;

public record CardButton(
    CardTheme Theme,
    string Text,
    string Value,
    bool IsLink,
    Dictionary<object, object?>? Options = null
);