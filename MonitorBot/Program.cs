using MonitorBot.Utils;

namespace MonitorBot
{
    public class Program
    {
        public const string Version = "1.0.1";
        public static Task Main(string[] args)
        {
            Console.WriteLine($"Monitor Kook Bot Version {Version}");
            try { using StreamReader sr = new(Environment.CurrentDirectory + @"\Logo.txt"); Console.WriteLine(sr.ReadToEnd()); } catch { }
            if (args.Length > 0) { _ = ulong.TryParse(args[0], out MonitorBot.GuildId); Logger.Info($"GuildId is designated. GuildId: {MonitorBot.GuildId}"); }
            return MonitorBot.Instance.MainAsync();
        }
    }
}
