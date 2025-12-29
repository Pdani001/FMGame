using System;
using System.Collections.Generic;
using System.Text;

namespace Alzaitu.Lacewing.Client.Packet.EventData
{
	public class EventTextMessage : EventArgs
	{
		public LacewingClient Client { get; internal set; }
		public int PeerID { get; set; }
		public int Channel { get; set; }
		public byte SubChannel { get; set; }
		public string Message { get; set; }
		public bool Blasted { get; set; }
		public MessageEventType Type { get; internal set; }
	}
}
