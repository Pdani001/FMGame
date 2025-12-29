using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Alzaitu.Lacewing.Client.Packet.Response
{
	[PacketType(0, 4, false, true)]
	internal class PacketResponseChannelList : PacketResponse
	{
		public Dictionary<string, ClientChannel> ChannelList { get; private set; }

		protected override void ReadResponse(byte[] bytes, long size, int pos, LacewingClient client = null)
		{
			ChannelList = new Dictionary<string, ClientChannel>();
			if (Success && client != null)
			{
				while (bytes.Length > pos)
				{
					int count = bytes[pos++];
					pos++;
					int namelength = bytes[pos++];
					byte[] data = new byte[namelength];
					for(int i = 0; i < data.Length; i++)
					{
						data[i] = bytes[pos++];
					}
					string name = Encoding.UTF8.GetString(data);
					size -= 3 + namelength;
					ClientChannel channel = ClientChannel.GetChannel(client, name);
					channel.InitPeerList(count);
					ChannelList.Add(name, channel);
				}
			}
		}
	}
}
