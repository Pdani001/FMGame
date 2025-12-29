using System;
using System.IO;
using System.Text;

namespace Alzaitu.Lacewing.Client.Packet.Response
{
	[PacketType(0, 0, false, true)]
	internal class PacketResponseConnect : PacketResponse
	{
		public const string DENY_REASON = "Connection was not allowed.";

		public ushort PeerId { get; set; }
		
		public string WelcomeMessage { get; set; }

		public string DenyReason { get; set; }

		protected override void ReadResponse(byte[] bytes, long size, int pos, LacewingClient client = null)
		{
			if (!Success)
			{
				DenyReason = Encoding.UTF8.GetString(bytes);
			}
			else
			{
				PeerId = bytes[pos++];
				byte[] msg = new byte[size-2];
				for(int i = 0; i < msg.Length; i++)
				{
					msg[i] = bytes[pos++];
				}
				WelcomeMessage = Encoding.UTF8.GetString(msg);
			}
		}

		protected override bool NeedsFirstZero() => true;

	}
}
