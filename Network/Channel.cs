using System;
using System.Collections.Generic;

namespace ReFMGame.Network
{
    public class Channel
    {
        public string Name { get; set; }
        public Guid Owner { get; set; }
        public List<Client> Clients { get; set; }
        public bool Password { get; set; } = false;
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
}
