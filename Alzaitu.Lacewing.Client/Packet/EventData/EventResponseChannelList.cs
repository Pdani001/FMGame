using System;
using System.Collections.Generic;
using System.Text;

namespace Alzaitu.Lacewing.Client.Packet.EventData
{
	public class EventResponseChannelList : EventArgs
	{
		public LacewingClient Client { get; internal set; }
		public Dictionary<string, ClientChannel> ChannelList { get; set; }
	}
}
