using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReFMGame.Network
{
    public class Message
    {
        public string type { get; set; }
        public byte subchannel { get; set; }
        public string client { get; set; }
        public string channelname { get; set; }
        public Channel channel { get; set; }
        public string text { get; set; }
        public int? value { get; set; }
        public Channel[] channels { get; set; }
        public string[] clients { get; set; }
        public string error { get; set; }
        public bool? success { get; set; }
    }
}
