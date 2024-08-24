using Timer = System.Threading.Timer;

namespace MonitorBot.Utils
{
    public class TaskExecutor
    {
        private readonly List<ThreadHolder> holders = new();
        private readonly Dictionary<Priority, List<Action>> taskMap = new();
        private int TaskTimeout { get; set; } //second
        private int MaxThread { get; set; }
        private bool DoLog { get; set; }
        private Action<string> LogFunc { get; set; }
        private Timer Timer;
        private bool closed;

        public TaskExecutor(int timeoutSecond = 5, int maxThread = -1, bool doLog = false, Action<string>? logFunc = null)
        {
            TaskTimeout = timeoutSecond;
            MaxThread = maxThread;
            DoLog = doLog;
            LogFunc = logFunc ?? Console.WriteLine;
            this.holders.Add(new(this));
            this.taskMap.Add(Priority.Highest, new());
            this.taskMap.Add(Priority.High, new());
            this.taskMap.Add(Priority.Mid, new());
            this.taskMap.Add(Priority.Low, new());
            this.taskMap.Add(Priority.Lowest, new());
            this.Timer = new(_ =>
            {
                DateTime now = DateTime.Now;
                if (this.holders.All(x => x.Executing && (now - x.StartTime).TotalSeconds >= TaskTimeout))
                {
                    if (this.holders.Count >= MaxThread)
                        this.Log("All thread are running, but max thread count exceeded");
                    else
                    {
                        this.Log("All thread are running, create new thread");
                        this.holders.Add(new(this));
                    }
                }
                else
                {
                    List<ThreadHolder> idled = this.holders.FindAll(x => !x.Executing);
                    if (idled.Count <= 1) return;
                    this.Log("Found more than 1 idle thread, try to release");
                    idled.RemoveAt(0);
                    foreach (ThreadHolder holder in idled)
                    {
                        holder.ShouldStop = true;
                        this.holders.Remove(holder);
                    }
                }
            }, null, 0, 1 * 1000);
        }

        private void Log(string message)
        {
            if (DoLog) LogFunc(message);
        }

        public void Execute(Action task, Priority priority = Priority.Mid)
        {
            if (this.closed) throw new NotSupportedException("This executor is closed!");
            lock (this.taskMap) this.taskMap[priority].Add(task);
        }

        private Action? FindAndRemoveTask()
        {
            lock (this.taskMap)
            {
                foreach (Priority p in new[] {
                             Priority.Highest, Priority.High, Priority.Mid, Priority.Low, Priority.Lowest
                         })
                {
                    if (!this.taskMap.ContainsKey(p)) continue;
                    List<Action> l = this.taskMap[p];
                    if (l.Count <= 0) continue;
                    Action action = l[0];
                    l.RemoveAt(0);
                    return action;
                }
            }
            return null;
        }

        public void Close()
        {
            this.closed = true;
            this.holders.ForEach(x => x.ShouldStop = true);
            Action? action = this.FindAndRemoveTask();
            while (action != null)
            {
                action();
                action = this.FindAndRemoveTask();
            }
        }

        private class ThreadHolder
        {
            public DateTime StartTime = new(0);
            public bool Executing;
            public bool ShouldStop;

            public ThreadHolder(TaskExecutor executor)
            {
                new Thread(() =>
                {
                    while (!this.ShouldStop)
                    {
                        Action? action = executor.FindAndRemoveTask();
                        if (action == null)
                        {
                            this.Executing = false;
                            Thread.Sleep(0);
                        }
                        else
                        {
                            this.Executing = true;
                            this.StartTime = DateTime.Now;
                            action();
                        }
                    }
                }).Start();
            }
        }

        public enum Priority
        {
            Lowest, Low, Mid, High, Highest
        }
    }
}
