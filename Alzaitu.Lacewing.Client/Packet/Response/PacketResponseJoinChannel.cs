using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Alzaitu.Lacewing.Client.Packet.Response
{
	[PacketType(0, 2, false, true)]
	internal class PacketResponseJoinChannel : PacketResponse
	{
		public bool IsMaster { get; set; }

		public string ChannelName { get; set; }

		public ClientChannel Channel { get; set; }

		protected override void ReadResponse(byte[] bytes, long size, int pos, LacewingClient client = null)
		{
			if (client != null)
			{
				if (Success) {
					IsMaster = bytes[pos++] == 1;
					size--;
					int channelnamelength = bytes[pos++];
					size--;
					byte[] data = new byte[channelnamelength];
					for (int i = 0; i < data.Length; i++)
					{
						data[i] = bytes[pos++];
					}
					string name = Encoding.UTF8.GetString(data);
					size -= channelnamelength;
					int ChannelID = BitConverter.ToUInt16(new byte[2] { bytes[pos++], bytes[pos++] },0);
					size -= 2;
					Channel = ClientChannel.GetChannel(client, name);
					List<ClientPeer> peers = new List<ClientPeer>();
					while (bytes.Length > pos)
					{
						ushort ClientID = BitConverter.ToUInt16(new byte[2] { bytes[pos++], bytes[pos++] },0);
						size -= 2;
						bool isMaster = bytes[pos++] == 1;
						size--;
						int ClientNameLength = bytes[pos++];
						size--;
						data = new byte[ClientNameLength];
						for (int i = 0; i < data.Length; i++)
						{
							data[i] = bytes[pos++];
						}
						string ClientName = Encoding.UTF8.GetString(data);
						size -= ClientNameLength;
						
						ClientPeer peer = ClientPeer.GetPeer(client, ClientID);
						peer.Name = ClientName;
						peer.ChannelMaster = isMaster;
						if (isMaster)
							Channel.ChannelMaster = peer;
						peers.Add(peer);
					}
					Channel.ResetPeerList(peers);
					Channel.Id = ChannelID;
					client.globalChannelsByID[ChannelID] = Channel;
				}
				else
				{
					int channelnamelength = bytes[pos++];
					byte[] data = new byte[channelnamelength];
					for (int i = 0; i < data.Length; i++)
					{
						data[i] = bytes[pos++];
					}
					ChannelName = Encoding.UTF8.GetString(data);
				}
			}
		}
	}
}
