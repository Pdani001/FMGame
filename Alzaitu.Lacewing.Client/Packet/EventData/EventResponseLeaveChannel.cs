using System;
using System.Collections.Generic;
using System.Text;

namespace Alzaitu.Lacewing.Client.Packet.EventData
{
	public class EventResponseLeaveChannel : EventArgs
	{
		public LacewingClient Client { get; internal set; }
		public int PeerID { get; set; }
		public int Channel { get; set; }
	}
}
