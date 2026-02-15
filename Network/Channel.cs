using System;
using System.Collections.Generic;

namespace ReFMGame.Network
{
    public class Channel
    {
        public string Name { get; set; }
        public Guid Owner { get; set; }
        public List<Client> Clients { get; set; }
        public int MaxPlayers { get; set; } = 5;
        public bool Password { get; set; } = false;
        public CustomGamemodes Gamemodes { get; set; } = new();
        public List<MoveTimes> MoveTimes { get; set; } = [];
        public List<AILevel> AILevels { get; set; } = [];
        public override string ToString()
        {
            return Name + '\n' + (Password ? "\u00CC " : "  ") + Clients.Count + "/" + MaxPlayers;
        }
    }
    public class Selected
    {
        public Character Character { get; set; }
        public Guid Id { get; set; }
        public bool Ready { get; set; }
    }
    public class CharacterPosition
    {
        public Character Character { get; set; }
        public short Position {  get; set; }
    }
    public class AILevel
    {
        public Character Character { get; set; }
        public byte Level { get; set; }
    }
    public class CustomGamemodes
    {
        public bool AnimatronicAI { get; set; } = false;
        public bool CustomNight { get; set; } = false;
    }
    public class MoveTimes
    {
        public Character Character { get; set; } = Character.None;
        public int Min { get; set; } = 0;
        public int Max { get; set; } = 0;
    }
}
