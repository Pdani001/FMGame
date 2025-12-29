using System;
using System.Collections.Generic;
using System.Text;

namespace Alzaitu.Lacewing.Client.Packet.EventData
{
	public class EventResponseConnect : EventArgs
	{
		public LacewingClient Client { get; internal set; }
		public int PeerID { get; set; }
	}
}
