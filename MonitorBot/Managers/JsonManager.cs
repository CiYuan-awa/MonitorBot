using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace MonitorBot.Utils
{
    public partial class JsonManager
    {

        public static string TryGetKookCardContent(string CardContent)
        {
            try
            {
                List<Card> cards = JsonSerializer.Deserialize<List<Card>>(CardContent)!;
                string contents = string.Empty;

                foreach (var card in cards)
                {
                    foreach (var module in card.Modules!)
                    {
                        if (module.Type == "section" && module.Text != null && module.Text.Content != null)
                        {
                            contents += module.Text.Content + " ";
                        }
                    }
                }
                return CardLog().Replace(contents, string.Empty).Replace("\n", " ");
            }
            catch { return null!; }
        }

        public class Text
        {
            [JsonPropertyName("content")]
            public string? Content { get; set; }
        }

        public class Module
        {
            [JsonPropertyName("type")]
            public string? Type { get; set; }

            [JsonPropertyName("text")]
            public Text? Text { get; set; }
        }

        public class Card
        {
            [JsonPropertyName("modules")]
            public List<Module>? Modules { get; set; }
        }

        [GeneratedRegex("[!-\\-/:-@\\[-`{-~]+")]
        private static partial Regex CardLog();
    }
}
