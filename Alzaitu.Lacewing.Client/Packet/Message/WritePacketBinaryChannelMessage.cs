using System;
using System.Collections.Generic;
using System.IO;

namespace Alzaitu.Lacewing.Client.Packet.Message
{
	[PacketType(2, true, false)]
	internal class WritePacketBinaryChannelMessage : Packet
	{
		public byte SubChannel { get; set; }

		public ushort Channel { get; set; }

		public byte[] Message { get; set; }

		protected override void WriteImpl(BinaryWriter wrt)
		{
			wrt.Write(SubChannel);
			wrt.Write(BitConverter.GetBytes(Channel));
			wrt.Write(Message);
		}

		protected override void WriteImpl(int pos, out byte[] bytes)
		{
			List<byte> list = new List<byte>();
			list.Add(SubChannel);
			list.AddRange(BitConverter.GetBytes(Channel));
			list.AddRange(Message);
			bytes = list.ToArray();
		}

		public override long GetSize() => 3 + Message.LongLength;
	}
}
