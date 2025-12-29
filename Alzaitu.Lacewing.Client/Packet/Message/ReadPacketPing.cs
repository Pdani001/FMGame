using System.IO;

namespace Alzaitu.Lacewing.Client.Packet.Message
{
	[PacketType(11, false, true)]
	internal class ReadPacketPing : Packet
	{

		protected override void ReadImpl(byte[] bytes, long size, int pos, LacewingClient client = null, bool blasted = false)
		{
			
		}

		//public override long GetSize() => 0;
	}
}
