using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alzaitu.Lacewing.Client.Packet
{
	public class BinaryData
	{
		public static byte[] GetData(short value)
		{
			return BitConverter.GetBytes(value);
		}
		public static byte[] GetData(ushort value)
		{
			return BitConverter.GetBytes(value);
		}
		public static byte[] GetData(int value)
		{
			return BitConverter.GetBytes(value);
		}
		public static byte[] GetData(uint value)
		{
			return BitConverter.GetBytes(value);
		}
		public static byte[] GetData(long value)
		{
			return BitConverter.GetBytes(value);
		}
		public static byte[] GetData(ulong value)
		{
			return BitConverter.GetBytes(value);
		}
		public static byte[] GetData(string value, bool nulled = false)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			List<byte> data = new List<byte>();
			if (!nulled)
				data.AddRange(GetData(Encoding.UTF8.GetByteCount(value)));
			data.AddRange(Encoding.UTF8.GetBytes(value));
			if(nulled)
				data.AddRange(Encoding.UTF8.GetBytes("\0"));
			return data.ToArray();
		}
	}
}
