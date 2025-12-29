using System;
using System.Collections.Generic;
using System.Text;

namespace Alzaitu.Lacewing.Client.Packet.EventData
{
	public class EventDisconnect : EventArgs
	{
		public LacewingClient Client { get; internal set; }
		public string Reason { get; set; }
	}
}
