using System.IO;
using System.Text;
using static Alzaitu.Lacewing.Client.Packet.EventData.EventPeer;

namespace Alzaitu.Lacewing.Client.Packet.Message
{
	[PacketType(9, false, true)]
	internal class ReadPacketPeer : Packet
	{
		public ClientPeer Peer { get; set; }
		public int Channel { get; set; }
		public PeerAction Action { get; private set; }
		private short _peerId = -1;
		public short PeerID
		{
			get { return _peerId; }
			set { _peerId = value; }
		}
		public string OldName { get; private set; }
		private bool isMaster = false;
		private string name = null;

		protected override void ReadImpl(byte[] bytes, long size, int pos, LacewingClient client = null, bool blasted = false)
		{
			if (size > 0)
			{
				Channel = ReadInversedShort(new byte[2] { bytes[pos++], bytes[pos++] });
				size -= 2;
				PeerID = (short)ReadInversedShort(new byte[2] { bytes[pos++], bytes[pos++] });
				size -= 2;
				if (size > 0)
				{
					isMaster = bytes[pos++] == 1;
					size--;
					byte[] msg = new byte[size];
					for (int i = 0; i < size; i++)
					{
						msg[i] = bytes[pos++];
					}
					name = Encoding.UTF8.GetString(msg);
				}
			}
		}

		public void UpdatePeer(LacewingClient lc)
		{
			ClientPeer peer = ClientPeer.GetPeer(lc, (ushort)PeerID);
			ClientChannel channel = ClientChannel.GetChannelByID(lc, Channel);
			if(channel != null && lc.joinedChannels.Contains(channel))
			{
				if (name != null)
				{
					if (!channel.Contains(peer))
					{
						Action = PeerAction.Join;
					}
					else
					{
						Action = PeerAction.Change;
					}
				} else
				{
					Action = PeerAction.Left;
				}
			}
			switch (Action)
			{
				case PeerAction.Join:
					peer.Name = name;
					peer.ChannelMaster = isMaster;
					channel.Add(peer);
					break;
				case PeerAction.Left:
					OldName = peer.Name;
					lc.clientsByID.Remove(PeerID);
					channel.Remove(peer);
					break;
				case PeerAction.Change:
					peer.Name = name;
					peer.ChannelMaster = isMaster;
					break;
				default:
					break;
			}
		}

		//public override long GetSize() => (PeerID == -1) ? 5 + Peer.Name.Length : 4;
	}
}
