using Microsoft.Xna.Framework.Input;
using ReFMGame.GameHelper;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ReFMGame
{
    public class Settings
    {
        public Dictionary<BindKey, KeyBind> KeyBinds { get; set; } = new() {
            {BindKey.Fullscreen, new(Key: Keys.F11)},
            {BindKey.Chat, new(Key: Keys.T, Char: 't')},
            {BindKey.Screenshot, new(Key: Keys.F2)},
            {BindKey.Debug, new(Key: Keys.F1, Ctrl: true) },
        };
        public int ServerIndex { get; set; } = 0;
        public string CustomAddress { get; set; } = "";
    }

    [JsonSourceGenerationOptions(
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        WriteIndented = true)]
    [JsonSerializable(typeof(Settings))]
    internal partial class SettingsContext : JsonSerializerContext { }
}
