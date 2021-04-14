using System;
using System.Text;

namespace Microsoft.Exchange.Conversion
{
	internal static class ExBitConverter
	{
		public static int Write(short value, byte[] buffer, int offset)
		{
			buffer[offset] = (byte)value;
			buffer[offset + 1] = (byte)(value >> 8);
			return 2;
		}

		public static int Write(ushort value, byte[] buffer, int offset)
		{
			buffer[offset] = (byte)value;
			buffer[offset + 1] = (byte)(value >> 8);
			return 2;
		}

		public static int Write(int value, byte[] buffer, int offset)
		{
			buffer[offset] = (byte)value;
			buffer[offset + 1] = (byte)(value >> 8);
			buffer[offset + 2] = (byte)(value >> 16);
			buffer[offset + 3] = (byte)(value >> 24);
			return 4;
		}

		public static int Write(uint value, byte[] buffer, int offset)
		{
			buffer[offset] = (byte)value;
			buffer[offset + 1] = (byte)(value >> 8);
			buffer[offset + 2] = (byte)(value >> 16);
			buffer[offset + 3] = (byte)(value >> 24);
			return 4;
		}

		public static int Write(long value, byte[] buffer, int offset)
		{
			ExBitConverter.Write((int)value, buffer, offset);
			ExBitConverter.Write((int)(value >> 32), buffer, offset + 4);
			return 8;
		}

		public static int Write(ulong value, byte[] buffer, int offset)
		{
			return ExBitConverter.Write((long)value, buffer, offset);
		}

		public unsafe static int Write(float value, byte[] buffer, int offset)
		{
			return ExBitConverter.Write(*(int*)(&value), buffer, offset);
		}

		public unsafe static int Write(double value, byte[] buffer, int offset)
		{
			return ExBitConverter.Write(*(long*)(&value), buffer, offset);
		}

		public static int Write(char value, byte[] buffer, int offset)
		{
			return ExBitConverter.Write((short)value, buffer, offset);
		}

		public unsafe static int Write(Guid value, byte[] buffer, int offset)
		{
			byte* ptr = (byte*)(&value);
			for (int i = 0; i < 16; i += 4)
			{
				buffer[offset] = ptr[i];
				buffer[offset + 1] = ptr[i + 1];
				buffer[offset + 2] = ptr[i + 2];
				buffer[offset + 3] = ptr[i + 3];
				offset += 4;
			}
			return 16;
		}

		public static int Write(string value, int maxCharCount, bool unicode, byte[] buffer, int offset)
		{
			int num = Math.Min(value.Length, maxCharCount);
			if (!unicode)
			{
				for (int i = 0; i < num; i++)
				{
					buffer[offset++] = ((value[i] < '\u0080') ? ((byte)value[i]) : 63);
				}
				buffer[offset++] = 0;
				return num + 1;
			}
			int bytes = CTSGlobals.UnicodeEncoding.GetBytes(value, 0, num, buffer, offset);
			return bytes + ExBitConverter.Write(0, buffer, offset + bytes);
		}

		public static int Write(string value, bool unicode, byte[] buffer, int offset)
		{
			return ExBitConverter.Write(value, value.Length, unicode, buffer, offset);
		}

		public unsafe static Guid ReadGuid(byte[] buffer, int offset)
		{
			if (offset < 0 || offset > buffer.Length - sizeof(Guid))
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			Guid result;
			byte* ptr = (byte*)(&result);
			for (int i = 0; i < sizeof(Guid); i += 4)
			{
				ptr[i] = buffer[offset];
				ptr[i + 1] = buffer[offset + 1];
				ptr[i + 2] = buffer[offset + 2];
				ptr[i + 3] = buffer[offset + 3];
				offset += 4;
			}
			return result;
		}

		public static byte[] ReadByteArray(byte[] buffer, int offset, int length)
		{
			byte[] array = new byte[length];
			Array.Copy(buffer, offset, array, 0, length);
			return array;
		}

		public static string ReadAsciiString(byte[] buffer, int offset)
		{
			int num = offset;
			while (num < buffer.Length && buffer[num] != 0)
			{
				num++;
			}
			StringBuilder stringBuilder = new StringBuilder(num - offset);
			for (int i = offset; i < num; i++)
			{
				stringBuilder.Append((char)((buffer[i] < 128) ? buffer[i] : 63));
			}
			return stringBuilder.ToString();
		}
	}
}
