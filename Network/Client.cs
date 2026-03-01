using System;

namespace ReFMGame.Network
{
    public class Client
    {
        public Guid Id { get; set; }
        public string Nick { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}
