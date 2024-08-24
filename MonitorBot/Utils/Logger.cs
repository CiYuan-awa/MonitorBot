namespace MonitorBot.Utils
{
    public class Logger
    {
        private static string GetTime()
        {
            return DateTime.Now.ToString("[HH:mm:ss.ffff] ");
        }
        private static string GetType(string Type)
        {
            return Type switch
            {
                "M" => "[Message] ",
                "I" => "[Info] ",
                "W" => "[Warn] ",
                "E" => "[Error] ",
                _ => string.Empty
            };
        }
        public static void Message<Template>(Template message)
        {
            Log("M", message);
        }
        public static void Message<Template>(Template prefix, Template message)
        {
            Log("M", prefix, message);
        }
        public static void Message<Template>(ColorType color, Template message)
        {
            Log("M", color, message);
        }
        public static void Message<Template>(ColorType color, Template prefix, Template message)
        {
            Log(color, prefix, message);
        }
        public static void Info<Template>(Template message)
        {
            Log("I", message);
        }
        public static void Info<Template>(Template prefix, Template message)
        {
            Log("I", prefix, message);
        }
        public static void Info<Template>(ColorType color, Template message)
        {
            Log("I", color, message);
        }
        public static void Info<Template>(ColorType color, Template prefix, Template message)
        {
            Log(color, prefix, message);
        }
        public static void Warn<Template>(Template message)
        {
            Log("W", message);
        }
        public static void Warn<Template>(Template prefix, Template message)
        {
            Log("W", prefix, message);
        }
        public static void Warn<Template>(ColorType color, Template message)
        {
            Log("W", color, message);
        }
        public static void Warn<Template>(ColorType color, Template prefix, Template message)
        {
            Log(color, prefix, message);
        }
        public static void Error<Template>(Template message)
        {
            Log("E", message);
        }
        public static void Error<Template>(Template prefix, Template message)
        {
            Log("E", prefix, message);
        }
        public static void Error<Template>(ColorType color, Template message)
        {
            Log("E", color, message);
        }
        public static void Error<Template>(ColorType color, Template prefix, Template message)
        {
            Log(color, prefix, message);
        }

        private static void Log<Template>(string type, Template message)
        {
            Console.ForegroundColor = type switch
            {
                "M" => ConsoleColor.Blue,
                "I" => ConsoleColor.White,
                "W" => ConsoleColor.DarkYellow,
                "E" => ConsoleColor.Red,
                _ => Console.ForegroundColor
            };
            Console.WriteLine(GetTime() + GetType(type) + message);
        }
        private static void Log<Template>(string type, Template prefix, Template message)
        {
            Console.ForegroundColor = type switch
            {
                "M" => ConsoleColor.Blue,
                "I" => ConsoleColor.White,
                "W" => ConsoleColor.DarkYellow,
                "E" => ConsoleColor.Red,
                _ => ConsoleColor.White
            };
            Console.WriteLine(GetTime() + "[" + prefix + "] " + message);
        }
        private static void Log<Template>(string type, ColorType color, Template message)
        {
            SetColor(color);
            Console.WriteLine(GetTime() + GetType(type) + message);
        }
        private static void Log<Template>(ColorType color, Template prefix, Template message)
        {
            SetColor(color);
            Console.WriteLine(GetTime() + "[" + prefix + "] " + message);
        }


        public static void SetColor(ColorType Color)
        {
            Console.ForegroundColor = Color switch
            {
                ColorType.Black => ConsoleColor.Black,
                ColorType.DarkBlue => ConsoleColor.DarkBlue,
                ColorType.DarkGreen => ConsoleColor.DarkGreen,
                ColorType.DarkAqua => ConsoleColor.DarkCyan,
                ColorType.DarkRed => ConsoleColor.DarkRed,
                ColorType.DarkMagenta => ConsoleColor.DarkMagenta,
                ColorType.Yellow => ConsoleColor.DarkYellow,
                ColorType.Gray => ConsoleColor.Gray,
                ColorType.DarkGray => ConsoleColor.DarkGray,
                ColorType.Blue => ConsoleColor.Blue,
                ColorType.Green => ConsoleColor.Green,
                ColorType.Aqua => ConsoleColor.Cyan,
                ColorType.Red => ConsoleColor.Red,
                ColorType.Magenta => ConsoleColor.Magenta,
                ColorType.LightYellow => ConsoleColor.Yellow,
                ColorType.White => ConsoleColor.White,
                _ => ConsoleColor.White
            };
        }
    }

    public enum ColorType
    {
        Black = 0,
        DarkBlue = 1,
        DarkGreen = 2,
        DarkAqua = 3,
        DarkRed = 4,
        DarkMagenta = 5,
        Yellow = 6,
        Gray = 7,
        DarkGray = 8,
        Blue = 9,
        Green = 10,
        Aqua = 11,
        Red = 12,
        Magenta = 13,
        LightYellow = 14,
        White = 15
    }
}
