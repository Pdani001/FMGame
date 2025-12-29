using System.IO;
using System.Text;

namespace Alzaitu.Lacewing.Client.Packet.Response
{
	[PacketType(0, 3, false, true)]
	internal class PacketResponseLeaveChannel : PacketResponse
	{
		public int ChannelID { get; set; }

		protected override void ReadResponse(byte[] bytes, long size, int pos, LacewingClient client = null)
		{
			ChannelID = bytes[pos++];
		}
	}
}
