using System;
using System.Collections.Generic;
using System.Text;

namespace Alzaitu.Lacewing.Client.Packet.EventData
{
	public class EventError : EventArgs
	{
		public LacewingClient Client { get; internal set; }
		public int PeerID { get; set; }
		public string Error { get; set; }
	}
}
