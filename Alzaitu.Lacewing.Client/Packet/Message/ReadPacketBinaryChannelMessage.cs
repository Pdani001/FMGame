using System;
using System.Collections.Generic;

namespace Alzaitu.Lacewing.Client.Packet.Message
{
	[PacketType(2, false, true)]
	internal class ReadPacketBinaryChannelMessage : Packet
	{
		public byte SubChannel { get; set; }

		public int Channel { get; set; }

		public int Peer { get; set; }

		public byte[] Message { get; set; }

		protected override void ReadImpl(byte[] bytes, long size, int pos, LacewingClient client = null, bool blasted = false)
		{
			SubChannel = bytes[pos++];
			Channel = ReadInversedShort(new byte[2] { bytes[pos++], bytes[pos++] });
			Peer = ReadInversedShort(new byte[2] { bytes[pos++], bytes[pos++] });
			byte[] data;
			if (blasted)
			{
				size -= pos;
				data = new byte[size];
				for(int i= 0; i < size; i++)
				{
					data[i] = bytes[pos++];
				}
			} else
			{
				if (bytes.Length > pos)
				{
					List<byte> m = new List<byte>();
					for (int i = pos; i < bytes.Length; i++)
					{
						m.Add(bytes[i]);
					}
					if(m.Count < size - 5)
					{
						m.AddRange(ReadMessage(client.GetStream(), size - m.Count - 5));
					}
					data = m.ToArray();
				}
				else
				{
					data = ReadMessage(client.GetStream(), size - 5);
				}
			}
			Message = data;
		}

		//public override long GetSize() => sizeof(byte) + Message.LongLength;
	}
}
