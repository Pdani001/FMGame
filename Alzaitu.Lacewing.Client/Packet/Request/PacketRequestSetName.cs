using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Alzaitu.Lacewing.Client.Packet.Request
{
	[PacketType(0, 1, true, false)]
	internal class PacketRequestSetName : PacketRequest
	{
		public string Name { get; internal set; }

		protected override void WriteImpl(BinaryWriter wrt)
		{
			wrt.Write(Encoding.UTF8.GetBytes(Name));
		}

		public override long GetSize() => Encoding.UTF8.GetByteCount(Name);
	}
}
