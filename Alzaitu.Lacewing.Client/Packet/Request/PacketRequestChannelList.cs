using System.IO;

namespace Alzaitu.Lacewing.Client.Packet.Request
{
	[PacketType(0, 4, true, false)]
	internal class PacketRequestChannelList : PacketRequest
	{
		protected override void WriteImpl(BinaryWriter wrt)
		{
			
		}

		public override long GetSize() => 0;
	}
}
