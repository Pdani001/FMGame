using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Alzaitu.Lacewing.Client.Packet
{
	public class BinaryReader
	{
		private byte[] Message { get; set; }
		private int cursor = 0;
		public BinaryReader(byte[] message)
		{
			Message = message;
		}

		public byte ReadByte(int position = int.MaxValue)
		{
			if(position == int.MaxValue)
			{
				position = cursor;
				cursor++;
			}
			if(Message.Length > position)
			{
				return Message[position];
			}
			return 0;
		}

		public string ReadText(int length = 0, int position = int.MaxValue)
		{
			bool inc = false;
			if (position == int.MaxValue)
			{
				inc = true;
				position = cursor;
				cursor++;
			}
			if (length > 0)
			{
				if (Message.Length > position + length - 1)
				{
					List<byte> list = new List<byte>();
					for (int i = 0; i < length; i++)
					{
						list.Add(Message[position]);
						position++;
						if(inc)
							cursor++;
					}
					return Encoding.UTF8.GetString(list.ToArray());
				}
			}
			else
			{
				byte[] terminator = Encoding.UTF8.GetBytes("\0"); // Problem: The encoding may not have a NULL character
				int charSize = terminator.Length; // Problem: The character size may be variable
				List<byte> strBytes = new List<byte>();
				byte[] chr = new byte[0];
				while (!chr.Equals(terminator))
				{
					List<byte> t = new List<byte>();
					for (int i = 0; i < charSize; i++)
						t.Add(Message[position++]);
					chr = t.ToArray();

					if (chr.Length != charSize)
						throw new EndOfStreamException();

					if(inc)
						cursor += charSize;

					strBytes.AddRange(chr);
				}

				return Encoding.UTF8.GetString(strBytes.ToArray());
			}
			return null;
		}

		public int ReadInt(int position = int.MaxValue)
		{
			if (position == int.MaxValue)
			{
				position = cursor;
				cursor += sizeof(int);
			}
			if (Message.Length > position+(sizeof(int)-1))
			{
				return BitConverter.ToInt32(Message,position);
			}
			return 0;
		}

		public uint ReadUInt(int position = int.MaxValue)
		{
			if (position == int.MaxValue)
			{
				position = cursor;
				cursor += sizeof(uint);
			}
			if (Message.Length > position + (sizeof(uint) - 1))
			{
				return BitConverter.ToUInt32(Message, position);
			}
			return 0;
		}

		public short ReadShort(int position = int.MaxValue)
		{
			if (position == int.MaxValue)
			{
				position = cursor;
				cursor += sizeof(short);
			}
			if (Message.Length > position + (sizeof(short) - 1))
			{
				return BitConverter.ToInt16(Message, position);
			}
			return 0;
		}

		public ushort ReadUShort(int position = int.MaxValue)
		{
			if (position == int.MaxValue)
			{
				position = cursor;
				cursor += sizeof(ushort);
			}
			if (Message.Length > position + (sizeof(ushort) - 1))
			{
				return BitConverter.ToUInt16(Message, position);
			}
			return 0;
		}

		public float ReadFloat(int position = int.MaxValue)
		{
			if (position == int.MaxValue)
			{
				position = cursor;
				cursor += sizeof(float);
			}
			if (Message.Length > position + (sizeof(float) - 1))
			{
				return BitConverter.ToSingle(Message, position);
			}
			return 0;
		}
	}
}
