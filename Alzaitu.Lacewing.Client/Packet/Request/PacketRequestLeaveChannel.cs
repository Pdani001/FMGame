using System.IO;

namespace Alzaitu.Lacewing.Client.Packet.Request
{
	[PacketType(0, 3, true, false)]
	internal class PacketRequestLeaveChannel : PacketRequest
	{
		public short ChannelId { get; internal set; }

		protected override void WriteImpl(BinaryWriter wrt)
		{
			wrt.Write(ChannelId);
		}

		public override long GetSize() => sizeof(short);

	}
}
