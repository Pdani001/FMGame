using System;
using System.Collections.Generic;
using System.Text;
using Alzaitu.Lacewing.Client.Packet;
using System.Threading;
using System.IO;
using Alzaitu.Lacewing.Client.Packet.EventData;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Alzaitu.Lacewing.Client.StateMachine
{
	class ClientThread : BaseThread
	{
		private readonly LacewingClient client;
		public const int BufferSize = 256;
		private byte[] buffer = new byte[BufferSize];
		private readonly List<byte> message = new List<byte>();
		private static ManualResetEvent receiveDone = new ManualResetEvent(false);
		public ClientThread(LacewingClient client)
		{
			this.client = client;
		}
		protected override void RunThread()
		{
			/*Packet.Packet packet;
			while (!client.Disposed && !client.IsDisconnect)
			{
				if (!client.IsConnected)
				{
					packet = client.ReadPacket();
					PacketHandler.Handle(packet,client);
				} else
				{
					try
					{
						packet = client.ReadPacket();
					}
					catch (Exception e)
					{
						if(client.debug)
							client.logger.Write("Exception: {0}",e);
						if (e.GetType() == typeof(IOException))
						{
							client.Event.OnDisconnect(new EventDisconnect
							{
								Client = client,
								Reason = e.Message
							});
							break;
						}
						else
							continue;
					}
					PacketHandler.Handle(packet,client);
				}
			}
			if(!client.Disposed && !client.IsDisconnect)
				client.Dispose();*/
			Debug.WriteLine("TCP Thread started, begin receive");
			client._client.Client.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(ReceivePacket), client._client.Client);
			//client._client.Client.ReceiveAsync();
			//Task.Run(() => Read());
		}

		/*public async Task Read()
		{
			var buffer = new byte[uint.MaxValue];
			var ns = client.GetStream();
			while (true)
			{
				Console.WriteLine("Waiting for data read");
				var bytesRead = await ns.ReadAsync(buffer, 0, buffer.Length);
				if (bytesRead == 0)
				{
					Console.WriteLine("Stream was closed");
					return; // Stream was closed
				}
				Packet.Packet packet;
				if (!client.IsConnected)
				{
					packet = client.ReadNewPacket(new List<byte>(buffer));
					new Thread(() => {
						PacketHandler.Handle(packet, client);
					})
					{
						Name = "Pre-Packet Handler"
					}.Start();
				}
				else
				{
					try
					{
						packet = client.ReadNewPacket(new List<byte>(buffer));
						new Thread(() =>
						{
							PacketHandler.Handle(packet, client);
						})
						{
							Name = "Post-Packet Handler"
						}.Start();
					}
					catch (Exception e)
					{
						if (client.debug)
							client.logger.Write("Exception: {0}", e);
						if (e.GetType() == typeof(IOException))
						{
							client.Event.OnDisconnect(new EventDisconnect
							{
								Client = client,
								Reason = e.Message
							});
							return;
						}
					}
				}
			}
		}*/

		private void ReceivePacket(IAsyncResult ar)
		{
			if (client.Disposed || client.IsDisconnect)
				return;
			int read = ((Socket)ar.AsyncState).EndReceive(ar);
			if (read > 0)
			{
				Debug.WriteLine("[TCP] Packet receive in progress, length: "+read);
				List<byte> range = new List<byte>();
				if (read < BufferSize)
				{
					for (int i = 0; i < read; i++)
					{
						range.Add(buffer[i]);
					}
				} else
				{
					range.AddRange(buffer);
				}
				message.AddRange(range);
				Debug.WriteLine("[TCP] Current message: {"+string.Join(",",message)+"}");
				if (read != BufferSize)
				{
					ProcessPacket();
				}
				try
				{
					client._client.Client.BeginReceive(buffer, 0, BufferSize, 0, new AsyncCallback(ReceivePacket), ar.AsyncState);
				}
				catch (Exception)
				{
					if (!client.Disposed && !client.IsDisconnect)
						client.Dispose();
				}
			}
			else
			{
				client.Event.OnDisconnect(new EventDisconnect
				{
					Client = client,
					Reason = "Remote server closed connection"
				});
				if (!client.Disposed && !client.IsDisconnect)
					client.Dispose();
			}
		}
		
		private void ProcessPacket()
		{
			if (message.Count > 0)
			{
				Packet.Packet packet;
				if (!client.IsConnected)
				{
					packet = client.ReadNewPacket(message);
					PacketHandler.Handle(packet, client);
				}
				else
				{
					try
					{
						packet = client.ReadNewPacket(message);
						PacketHandler.Handle(packet, client);
					}
					catch (Exception e)
					{
						Debug.WriteLine("[TCP] Exception: {0}", e);
						if (e.GetType() == typeof(IOException))
						{
							client.Event.OnDisconnect(new EventDisconnect
							{
								Client = client,
								Reason = e.Message
							});
						}
					}
				}
				Debug.WriteLine("[TCP] Packet received: {" + string.Join(",", message) + "}");
				message.Clear();
			} else
			{
				Debug.WriteLine("[TCP] Empty packet proccess!");
			}
		}
	}
}
