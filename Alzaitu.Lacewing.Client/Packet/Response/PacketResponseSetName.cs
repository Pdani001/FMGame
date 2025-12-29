using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Alzaitu.Lacewing.Client.Packet.Response
{
	[PacketType(0, 1, false, true)]
	internal class PacketResponseSetName : PacketResponse
	{
		public const string DENY_REASON = "Name was not accepted.";

		public string Name { get; set; }

		public string DenyReason { get; set; }

		protected override void ReadResponse(byte[] bytes, long size, int pos, LacewingClient client = null)
		{
			byte length = bytes[pos++];
			size--;
			List<byte> data = new List<byte>();
			for(int i = 0; i < length; i++)
			{
				data.Add(bytes[pos++]);
			}
			Name = Encoding.UTF8.GetString(data.ToArray());
			size -= length;
			if (!Success)
			{
				data = new List<byte>();
				while(bytes.Length > pos)
				{
					data.Add(bytes[pos++]);
				}
				DenyReason = Encoding.UTF8.GetString(data.ToArray());
			}
		}
	}
}
