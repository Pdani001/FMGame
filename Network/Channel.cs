using System;
using System.Collections.Generic;

namespace ReFMGame.Network
{
    public class Channel
    {
        public string Name { get; set; }
        public Guid Owner { get; set; }
        public List<Client> Clients { get; set; }
    }
}
