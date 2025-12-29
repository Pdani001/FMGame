using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReFMGame.Network
{
    public class Channel
    {
        public string Name { get; }
        public string Owner { get; }
        public string[] Clients { get; }
    }
}
