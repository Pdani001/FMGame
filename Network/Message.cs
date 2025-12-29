using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReFMGame.Network
{
    public class Message
    {
        public string Type { get; set; }
        public byte SubChannel { get; set; }
        public string Client { get; set; }
        public string ChannelName { get; set; }
        public Channel Channel { get; set; }
        public string Text { get; set; }
        public int? Value { get; set; }
        public Channel[] Channels { get; set; }
        public string[] Clients { get; set; }
        public string Error { get; set; }
        public bool? Success { get; set; }
    }
}
