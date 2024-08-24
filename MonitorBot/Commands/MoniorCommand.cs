using Kook.Commands;
using Kook.WebSocket;
using MonitorBot.DataStorage;
using MonitorBot.Utils;

namespace MonitorBot.Commands
{
    public class MoniorCommand : ModuleBase<SocketCommandContext>
    {
        private static string MinecraftLogFilePath = string.Empty;
        private static Thread MonitorThread = null!;
        private static CancellationTokenSource CTS = null!;

        [Command("monitor")]
        [Alias("mon", "m")]
        public async Task Monitor(string ToggleCommand)
        {
            if (Context.Channel is not SocketTextChannel Channel) return;
            MinecraftLogFilePath = (string)await Data.GetTomlValueAsync(Data.MainConfig, "Minecraft_LogFile");
            switch (ToggleCommand)
            {
                case "on":
                    CTS = new CancellationTokenSource();
                    FileMonitor Monitor = new(MinecraftLogFilePath, Channel);
                    MonitorThread = new Thread(() => Monitor.ReadFile(CTS.Token));
                    MonitorThread.Start();
                    break;
                case "off":
                    CTS.Cancel();
                    CTS.Dispose();
                    break;
            }
        }
    }
}
