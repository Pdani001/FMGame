using System.IO;

namespace Alzaitu.Lacewing.Client.Packet.Message
{
	[PacketType(7, true, false)]
	internal class WritePacketUdpHello : Packet
	{

		protected override void WriteImpl(int pos, out byte[] bytes)
		{
			bytes = new byte[] { };
		}

		public override long GetSize() => 0;
	}
}
