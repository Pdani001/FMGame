using System.IO;

namespace Alzaitu.Lacewing.Client.Packet.Response
{
	internal abstract class PacketResponse : Packet
	{
		public bool Success { get; set; }

		protected sealed override void ReadImpl(byte[] bytes, long size, int pos, LacewingClient client = null, bool blasted = false)
		{
			Success = bytes[pos++] == 1;
			ReadResponse(bytes, size--, pos, client);
		}

		protected override bool NeedsFirstZero()
		{
			return base.NeedsFirstZero();
		}

		protected abstract void ReadResponse(byte[] bytes, long size, int pos, LacewingClient client = null);
	}
}
