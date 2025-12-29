using System.IO;

namespace Alzaitu.Lacewing.Client.Packet.Request
{
	internal abstract class PacketRequest : Packet
	{
		protected abstract override void WriteImpl(BinaryWriter wrt);
	}
}
