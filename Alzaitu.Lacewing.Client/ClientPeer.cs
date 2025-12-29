using Alzaitu.Lacewing.Client.StateMachine;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Alzaitu.Lacewing.Client.Packet.Response;
using System;
using Alzaitu.Lacewing.Client.Packet.Message;
using System.Net;
using System.Text;

namespace Alzaitu.Lacewing.Client
{
	public sealed class ClientPeer
	{

		public string Name { get; internal set; }

		public ushort Id { get; private set; }

		public bool ChannelMaster { get; internal set; }

		internal ClientPeer(ushort Id)
		{
			this.Id = Id;
		}

		public void SendBinaryMessage(LacewingClient client, int subChannel, ClientChannel channel, byte[] data, bool blasted = false)
		{
			if (channel.Contains(this))
			{
				Packet.Packet packet = new PacketBinaryPeerMessage
				{
					SubChannel = (byte)subChannel,
					Channel = channel.Id,
					Peer = Id,
					Message = data
				};
				packet.Variant = 2;
				client.WritePacket(packet, blasted);
			}
		}

		public void SendNumberMessage(LacewingClient client, int subChannel, ClientChannel channel, int data, bool blasted = false)
		{
			if (channel.Contains(this))
			{
				Packet.Packet packet = new PacketBinaryPeerMessage
				{
					SubChannel = (byte)subChannel,
					Channel = channel.Id,
					Peer = Id,
					Message = BitConverter.GetBytes(data)
				};
				packet.Variant = 1;
				client.WritePacket(packet, blasted);
			}
		}

		public void SendTextMessage(LacewingClient client, int subChannel, ClientChannel channel, string data, bool blasted = false)
		{
			if (channel.Contains(this))
			{
				Packet.Packet packet = new PacketBinaryPeerMessage
				{
					SubChannel = (byte)subChannel,
					Channel = channel.Id,
					Peer = Id,
					Message = Encoding.UTF8.GetBytes(data)
				};
				packet.Variant = 0;
				client.WritePacket(packet, blasted);
			}
		}

		public static ClientPeer GetPeer(LacewingClient lc, ushort id)
		{
			if (!lc.clientsByID.ContainsKey(id))
				lc.clientsByID.Add(id, new ClientPeer(id));

			return lc.clientsByID[id];
		}
	}
}
