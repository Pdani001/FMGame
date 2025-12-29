using System;
using System.Collections.Generic;
using System.Text;

namespace Alzaitu.Lacewing.Client.Packet.EventData
{
	public class EventPeer : EventArgs
	{
		public LacewingClient Client { get; internal set; }
		public string Name { get; set; }
		public int PeerID { get; set; }
		public int Channel { get; set; }
		public PeerAction Action { get; set; }

		public enum PeerAction : byte
		{
			None = 0,
			Join = 1,
			Change = 2,
			Left = 3
		}
	}
}
