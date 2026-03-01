using System;
using System.Collections.Generic;

namespace ReFMGame.Network
{
    public class Message
    {
        public string Type { get; set; }
        public Client Client { get; set; }
        public bool IsAdmin { get; set; }
        public string ChannelName { get; set; }
        public Channel Channel { get; set; }
        public string Text { get; set; }
        public int? Value { get; set; }
        public Character? Character { get; set; }
        public Channel[] Channels { get; set; }
        public string Nick { get; set; }
        public string Error { get; set; }
        public bool Ready { get; set; }
        public bool? Success { get; set; }
        public long? Tick { get; set; }
        public MoveTimes MoveTime { get; set; }
        public AILevel AILevel { get; set; }
        public CharacterPosition[] Positions { get; set; }
        public List<Selected> Selected { get; set; }
        public GameState State { get; set; }
    }
}
