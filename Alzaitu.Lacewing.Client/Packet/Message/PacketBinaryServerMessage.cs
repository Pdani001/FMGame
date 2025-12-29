using System.Collections.Generic;
using System.IO;

namespace Alzaitu.Lacewing.Client.Packet.Message
{
	[PacketType(1, true, true)]
	internal class PacketBinaryServerMessage : Packet
	{
		public byte SubChannel { get; set; }

		public byte[] Message { get; set; }

		protected override void WriteImpl(BinaryWriter wrt)
		{
			wrt.Write(SubChannel);
			wrt.Write(Message);
		}

		protected override void WriteImpl(int pos, out byte[] bytes)
		{
			List<byte> list = new List<byte>();
			list.Add(SubChannel);
			list.AddRange(Message);
			bytes = list.ToArray();
		}

		protected override void ReadImpl(byte[] bytes, long size, int pos, LacewingClient client = null, bool blasted = false)
		{
			SubChannel = bytes[pos++];
			byte[] data = new byte[size - 1];
			for (int i = 0; i < data.Length; i++)
			{
				data[i] = bytes[pos++];
			}
			Message = data;
		}

		public override long GetSize() => sizeof(byte) + Message.LongLength;
	}
}
