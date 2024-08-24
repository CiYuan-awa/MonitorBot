using Tomlyn;
using Tomlyn.Model;

namespace MonitorBot.DataStorage
{
    public class Data
    {
        public static DirectoryInfo ConfigDir = new(Environment.CurrentDirectory + "\\Config");
        public static FileInfo MainConfig = new(Environment.CurrentDirectory + "\\Config\\Main.toml");
        public static async Task Init()
        {
            if (!ConfigDir.Exists) ConfigDir.Create();
            if (!MainConfig.Exists)
            {
                using FileStream FS = MainConfig.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                using StreamWriter Writer = new(FS);
                await Writer.WriteAsync(Toml.FromModel(MainToml()));
            }
        }

        public static async Task<string> GetTomlContentAsync(FileInfo TomlFile)
        {
            string TomlContent;
            using FileStream FS = TomlFile.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            using StreamReader Reader = new(FS);
            TomlContent = await Reader.ReadToEndAsync();
            return TomlContent;
        }

        public static async Task<object> GetTomlValueAsync(FileInfo TomlFile, string Key)
        {
            TomlTable Model = Toml.ToModel(await GetTomlContentAsync(TomlFile));
            return Model[Key];
        }

        public static string GetTomlContent(FileInfo TomlFile)
        {
            string TomlContent;
            using FileStream FS = TomlFile.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            using StreamReader Reader = new(FS);
            TomlContent = Reader.ReadToEnd();
            return TomlContent;
        }

        public static object GetTomlValue(FileInfo TomlFile, string Key)
        {
            TomlTable Model = Toml.ToModel(GetTomlContent(TomlFile));
            return Model[Key];
        }

        private static TomlTable MainToml()
        {
            return new TomlTable
            {
                ["Kook_Token"] = "ENTER YOUR KOOK TOKEN HERE",
                ["MainGuildId"] = "ENTER YOUR MAIN GUILD ID HERE",
                ["Minecraft_LogFile"] = "ENTER YOUR MINECRAFT LOG FILE HERE"
            };
        }
    }
}
