using System.Diagnostics;
using System.IO;

namespace Alzaitu.Lacewing.Client.Packet.Message
{
	[PacketType(9, true, false)]
	internal class WritePacketPong : Packet
	{
		protected override void WriteImpl(BinaryWriter wrt)
		{
			
		}

		protected override void WriteImpl(int pos, out byte[] bytes)
		{
			bytes = new byte[0];
		}

		public override long GetSize() => 0;
	}
}
