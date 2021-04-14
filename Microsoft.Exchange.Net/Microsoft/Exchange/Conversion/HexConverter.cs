using System;
using System.Text;

namespace Microsoft.Exchange.Conversion
{
	public static class HexConverter
	{
		public static byte[] HexStringToByteArray(string value)
		{
			return HexConverter.HexStringToByteArray(value, 0, value.Length);
		}

		public static byte[] HexStringToByteArray(string value, int offset, int length)
		{
			if (length % 2 != 0)
			{
				throw new FormatException("Invalid Hex Data");
			}
			int num = length / 2;
			byte[] array = new byte[num];
			int num2 = offset;
			for (int i = 0; i < num; i++)
			{
				array[i] = HexConverter.ByteFromHexPair(value[num2++], value[num2++]);
			}
			return array;
		}

		public static byte ByteFromHexPair(char firstChar, char secondChar)
		{
			byte b = HexConverter.NumFromHex(firstChar);
			byte b2 = HexConverter.NumFromHex(secondChar);
			return (byte)((int)b << 4 | (int)b2);
		}

		public static byte NumFromHex(char ch)
		{
			byte b = (ch < '\u0080') ? HexConverter.HexCharToNum[(int)ch] : byte.MaxValue;
			if (b != 255)
			{
				return b;
			}
			throw new FormatException("Invalid Hex Data");
		}

		public static byte NumFromHex(byte ch)
		{
			return HexConverter.HexCharToNum[(int)ch];
		}

		public static string ByteArrayToHexString(byte[] array)
		{
			if (array == null)
			{
				return null;
			}
			return HexConverter.ByteArrayToHexString(array, 0, array.Length);
		}

		public static string ByteArrayToHexString(byte[] array, int start, int count)
		{
			if (array == null)
			{
				return null;
			}
			byte[] array2 = new byte[count * 2];
			int num = 0;
			for (int i = start; i < start + count; i++)
			{
				array2[num++] = HexConverter.NibbleToHex[array[i] >> 4];
				array2[num++] = HexConverter.NibbleToHex[(int)(array[i] & 15)];
			}
			return HexConverter.asciiEncoding.GetString(array2, 0, array2.Length);
		}

		internal static string ByteArrayToEscapedHexString(byte[] array, byte escapeByte, int start, int count)
		{
			if (array == null)
			{
				return null;
			}
			byte[] array2 = new byte[count * 3];
			int num = 0;
			for (int i = start; i < start + count; i++)
			{
				array2[num++] = escapeByte;
				array2[num++] = HexConverter.NibbleToHex[array[i] >> 4];
				array2[num++] = HexConverter.NibbleToHex[(int)(array[i] & 15)];
			}
			return HexConverter.asciiEncoding.GetString(array2, 0, array2.Length);
		}

		private static readonly Encoding asciiEncoding = Encoding.GetEncoding("us-ascii");

		private static readonly byte[] NibbleToHex = HexConverter.asciiEncoding.GetBytes("0123456789ABCDEF");

		private static readonly byte[] HexCharToNum = new byte[]
		{
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			8,
			9,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			10,
			11,
			12,
			13,
			14,
			15,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			10,
			11,
			12,
			13,
			14,
			15,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue
		};
	}
}
