using System;
using System.IO;
using System.Text;

namespace Alzaitu.Lacewing.Client.Packet.Request
{
	[PacketType(0, 0, true, false)]
	internal class PacketRequestConnect : PacketRequest
	{
		public const string CURRENT_VERSION = "revision 3";

		public string Version { get; internal set; }
		protected override void WriteImpl(BinaryWriter wrt)
		{
			if(Version != null)
				wrt.Write(Encoding.UTF8.GetBytes(Version));
		}

		public override long GetSize() => Version == null ? 0 : Encoding.UTF8.GetByteCount(Version);

		protected override bool NeedsFirstZero() => true;
	}
}
