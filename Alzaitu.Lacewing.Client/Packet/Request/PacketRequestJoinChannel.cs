using System;
using System.IO;
using System.Text;

namespace Alzaitu.Lacewing.Client.Packet.Request
{
	[PacketType(0, 2, true, false)]
	internal class PacketRequestJoinChannel : PacketRequest
	{
		public byte Flags { get; internal set; }

		public string ChannelName { get; internal set; }

		protected override void WriteImpl(BinaryWriter wrt)
		{
			wrt.Write(Flags);
			wrt.Write(Encoding.UTF8.GetBytes(ChannelName));
		}

		public override long GetSize() => sizeof(byte) + Encoding.UTF8.GetByteCount(ChannelName);
	}
}
