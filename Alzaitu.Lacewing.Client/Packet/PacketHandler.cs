using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Alzaitu.Lacewing.Client.Packet.EventData;
using Alzaitu.Lacewing.Client.Packet.Message;
using Alzaitu.Lacewing.Client.Packet.Request;
using Alzaitu.Lacewing.Client.Packet.Response;

namespace Alzaitu.Lacewing.Client.Packet
{
	class PacketHandler
	{
		public static void Handle(Packet packet, LacewingClient client, bool blasted = false)
		{
			if (!client.IsConnected)
			{
				switch (packet)
				{
					case PacketResponseConnect responseConnect:
						if (blasted)
							return;
						Debug.WriteLine("Connect response packet received");
						if (responseConnect.Success)
						{
							client.Event.OnResponseConnect(new EventResponseConnect
							{
								PeerID = responseConnect.PeerId,
								Client = client
							});
							client.ID = responseConnect.PeerId;
							client.IsConnected = true;
							client.FinishConnect();
						}
						else
						{
							//Console.WriteLine("Connection declined: " + responseConnect.DenyReason);
							client.Event.OnError(new EventError {
								Client = client,
								Error = responseConnect.DenyReason
							});
							client.Dispose();
						}
						break;
					default:
						break;
				}
			}
			else
			{
				switch (packet)
				{
					case PacketResponseSetName responseSetName:
						if (blasted)
							break;
						Debug.WriteLine("Setname response packet received");
						if (responseSetName.Success)
						{
							client.Event.OnResponseSetName(new EventResponseSetName
							{
								PeerID = client.ID,
								NewName = responseSetName.Name,
								Client = client
							});
							client.UserName = responseSetName.Name;
						} else
						{
							client.Event.OnResponseSetName(new EventResponseSetName
							{
								PeerID = client.ID,
								DenyReason = responseSetName.DenyReason,
								Client = client
							});
							client.Event.OnError(new EventError
							{
								Client = client,
								PeerID = client.ID,
								Error = responseSetName.DenyReason
							});
						}
						break;
					case PacketResponseJoinChannel responseJoinChannel:
						if (blasted)
							break;
						Debug.WriteLine("Channel join response packet received");
						if (responseJoinChannel.Success)
						{
							client.IsMaster = responseJoinChannel.IsMaster;
							ClientChannel channel = ClientChannel.GetChannel(client, responseJoinChannel.Channel.Name);
							client.joinedChannels.Add(channel);
							client.Event.OnResponseJoinChannel(new EventResponseJoinChannel
							{
								PeerID = client.ID,
								Channel = responseJoinChannel.Channel.Name,
								Client = client
							});
							foreach(ClientPeer peer in channel)
							{
								client.clientsByID[peer.Id] = peer;
							}
						}
						else
						{
							client.Event.OnError(new EventError
							{
								Client = client,
								PeerID = client.ID,
								Error = "Failed to join channel"
							});
						}
						break;
					case PacketResponseLeaveChannel responseLeaveChannel:
						if (blasted)
							break;
						Debug.WriteLine("Channel leave response packet received");
						if (responseLeaveChannel.Success)
						{
							ClientChannel channel = ClientChannel.GetChannelByID(client, responseLeaveChannel.ChannelID);
							client.joinedChannels.Remove(channel);
							client.Event.OnResponseLeaveChannel(new EventResponseLeaveChannel
							{
								PeerID = client.ID,
								Channel = responseLeaveChannel.ChannelID,
								Client = client
							});
						}
						else
						{
							client.Event.OnError(new EventError
							{
								Client = client,
								PeerID = client.ID,
								Error = "Failed to leave channel"
							});
						}
						break;
					case PacketResponseChannelList responseChannelList:
						if (blasted)
							break;
						Debug.WriteLine("Channel list response packet received");
						client.Event.OnResponseChannelList(new EventResponseChannelList
						{
							ChannelList = responseChannelList.ChannelList,
							Client = client
						});
						break;
					case PacketBinaryPeerMessage packetBinaryPeer:
						Debug.WriteLine("Binary peer packet received");
						bool hasClient = client.clientsByID.TryGetValue(packetBinaryPeer.Peer, out _);
						ClientChannel ch = ClientChannel.GetChannelByID(client, packetBinaryPeer.Channel);
						if (ch != null && hasClient)
						{
							switch (packet.Variant)
							{
								case 2:
									client.Event.OnBinaryMessage(new EventBinaryMessage
									{
										PeerID = packetBinaryPeer.Peer,
										Channel = packetBinaryPeer.Channel,
										SubChannel = packetBinaryPeer.SubChannel,
										Message = packetBinaryPeer.Message,
										Blasted = blasted,
										Client = client,
										Type = MessageEventType.Peer
									});
									break;
								case 1:
									client.Event.OnNumberMessage(new EventNumberMessage
									{
										PeerID = packetBinaryPeer.Peer,
										Channel = packetBinaryPeer.Channel,
										SubChannel = packetBinaryPeer.SubChannel,
										Message = BitConverter.ToInt32(packetBinaryPeer.Message, 0),
										Blasted = blasted,
										Client = client,
										Type = MessageEventType.Peer
									});
									break;
								case 0:
									client.Event.OnTextMessage(new EventTextMessage
									{
										PeerID = packetBinaryPeer.Peer,
										Channel = packetBinaryPeer.Channel,
										SubChannel = packetBinaryPeer.SubChannel,
										Message = Encoding.UTF8.GetString(packetBinaryPeer.Message),
										Blasted = blasted,
										Client = client,
										Type = MessageEventType.Peer
									});
									break;
								default:
									break;
							}
						}
						break;
					case ReadPacketBinaryChannelMessage packetBinaryChannel:
						ch = ClientChannel.GetChannelByID(client, packetBinaryChannel.Channel);
						if (ch != null)
						{
							switch (packet.Variant)
							{
								case 2:
									client.Event.OnBinaryMessage(new EventBinaryMessage
									{
										PeerID = packetBinaryChannel.Peer,
										Channel = packetBinaryChannel.Channel,
										SubChannel = packetBinaryChannel.SubChannel,
										Message = packetBinaryChannel.Message,
										Blasted = blasted,
										Client = client,
										Type = MessageEventType.Channel
									});
									break;
								case 1:
									client.Event.OnNumberMessage(new EventNumberMessage
									{
										PeerID = packetBinaryChannel.Peer,
										Channel = packetBinaryChannel.Channel,
										SubChannel = packetBinaryChannel.SubChannel,
										Message = BitConverter.ToInt32(packetBinaryChannel.Message, 0),
										Blasted = blasted,
										Client = client,
										Type = MessageEventType.Channel
									});
									break;
								case 0:
									client.Event.OnTextMessage(new EventTextMessage
									{
										PeerID = packetBinaryChannel.Peer,
										Channel = packetBinaryChannel.Channel,
										SubChannel = packetBinaryChannel.SubChannel,
										Message = Encoding.UTF8.GetString(packetBinaryChannel.Message),
										Blasted = blasted,
										Client = client,
										Type = MessageEventType.Channel
									});
									break;
								default:
									break;
							}
						}
						break;
					case ReadPacketPeer packetPeer:
						if (blasted)
							break;
						packetPeer.UpdatePeer(client);
						string name = packetPeer.Action == EventPeer.PeerAction.Left ? packetPeer.OldName : ClientPeer.GetPeer(client, (ushort)packetPeer.PeerID).Name;
						client.Event.OnPeer(new EventPeer
						{
							PeerID = packetPeer.PeerID,
							Action = packetPeer.Action,
							Channel = packetPeer.Channel,
							Name = name,
							Client = client
						});
						break;
					case ReadPacketPing _:
						Debug.WriteLine("Handling ping packet");
						client.WritePacket(new WritePacketPong(), blasted);
						break;
					default:
						break;
				}
			}
		}
	}
}
