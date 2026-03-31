using Microsoft.Xna.Framework.Input;
using System.Text.Json.Serialization;

namespace ReFMGame.GameHelper
{
    public class Settings
    {
        public Dictionary<BindKey, KeyBind> KeyBinds { get; set; } = new() {
            {BindKey.Fullscreen, new(Key: Keys.F11)},
            {BindKey.Chat, new(Key: Keys.T, Char: 't')},
            {BindKey.Screenshot, new(Key: Keys.F2)},
            {BindKey.Debug, new(Key: Keys.F1, Ctrl: true) },
        };
        public float Volume { get; set; } = 1f;
        public int ServerIndex { get; set; } = 0;
        public string CustomAddress { get; set; } = "";
        public string Nickname { get; set; } = "";
        public bool WarningDismissed { get; set; } = false;
    }

    [JsonSourceGenerationOptions(
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        WriteIndented = true)]
    [JsonSerializable(typeof(Settings))]
    public partial class SettingsContext : JsonSerializerContext { }
}
