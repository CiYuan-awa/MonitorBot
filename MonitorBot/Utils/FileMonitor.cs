using Kook.WebSocket;
using System.Text.RegularExpressions;
using System.Text;
using System.ComponentModel;

namespace MonitorBot.Utils
{
    partial class FileMonitor(string FilePath, SocketTextChannel Channel)
    {
        private readonly string _FilePath = FilePath;
        private readonly SocketTextChannel _Channel = Channel;
        private long _LastMaxOffset = new FileInfo(FilePath).Length;

        public async void ReadFile(CancellationToken Token)
        {
            while (!Token.IsCancellationRequested)
            {
                using (var FS = new FileStream(_FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    FS.Seek(_LastMaxOffset, SeekOrigin.Begin);
                    using var SR = new StreamReader(FS, Encoding.GetEncoding("GBK"));
                    string Line;
                    while ((Line = SR.ReadLine()!) != null)
                    {
                        if (Regex.IsMatch(Line, "^\\[\\d\\d\\:\\d\\d\\:\\d\\d\\] \\[Client thread/INFO\\]\\: \\[CHAT\\] .*$"))
                        {
                            Line = Line.Remove(0, 40);
                            Line = Regex.Replace(Line, "§.", "");
                            if (await IsValidMessage(Line)) await _Channel.SendTextAsync(Line);
                        }
                    }
                    _LastMaxOffset = FS.Position;
                }
                Thread.Sleep(1000); // Check for new content every second
            }
        }

        public static async Task<bool> IsValidMessage(string Text)
        {
            await Task.Yield();
            string Pattern = @"(^\* )|(died\.$)|(joined\.$)|(left\.$)|(was killed by)|(You cannot say the same message twice!)|(-----------------------------------------)";
            return !Regex.IsMatch(Text, Pattern);
        }
    }
}