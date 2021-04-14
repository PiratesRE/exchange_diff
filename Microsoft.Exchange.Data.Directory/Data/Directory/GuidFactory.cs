using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class GuidFactory
	{
		internal static Guid Parse(string guidString, int offset)
		{
			if (guidString == null)
			{
				throw new ArgumentNullException("guidString");
			}
			if (guidString.Length < offset + 36)
			{
				throw new ArgumentException("Too close to the end to point to a valid Guid string.", "offset");
			}
			foreach (int num in GuidFactory.dashPositions)
			{
				int num2 = offset + num;
				if (guidString[num2] != '-')
				{
					string message = string.Format("Expected dash at index {0}.", num2);
					throw new FormatException(message);
				}
			}
			int a = (int)GuidFactory.Read32(guidString, offset);
			short b = (short)GuidFactory.Read16(guidString, offset + 9);
			short c = (short)GuidFactory.Read16(guidString, offset + 14);
			byte d = GuidFactory.Read8(guidString, offset + 19);
			byte e = GuidFactory.Read8(guidString, offset + 21);
			byte f = GuidFactory.Read8(guidString, offset + 24);
			byte g = GuidFactory.Read8(guidString, offset + 26);
			byte h = GuidFactory.Read8(guidString, offset + 28);
			byte i2 = GuidFactory.Read8(guidString, offset + 30);
			byte j = GuidFactory.Read8(guidString, offset + 32);
			byte k = GuidFactory.Read8(guidString, offset + 34);
			return new Guid(a, b, c, d, e, f, g, h, i2, j, k);
		}

		private static byte Read4(string data, int offset)
		{
			uint num = (uint)data[offset];
			byte b = byte.MaxValue;
			if (num < 104U)
			{
				b = GuidFactory.vals[(int)((UIntPtr)num)];
			}
			if (b == 255)
			{
				string message = string.Format("Expected a hexadecimal digit at index {0}.", offset);
				throw new FormatException(message);
			}
			return b;
		}

		private static byte Read8(string data, int offset)
		{
			byte b = GuidFactory.Read4(data, offset);
			b = (byte)(b << 4);
			return b | GuidFactory.Read4(data, offset + 1);
		}

		private static ushort Read16(string data, int offset)
		{
			ushort num = (ushort)GuidFactory.Read8(data, offset);
			num = (ushort)(num << 8);
			return num | (ushort)GuidFactory.Read8(data, offset + 2);
		}

		private static uint Read32(string data, int offset)
		{
			uint num = (uint)GuidFactory.Read16(data, offset);
			num <<= 16;
			return num | (uint)GuidFactory.Read16(data, offset + 4);
		}

		private static readonly byte[] vals = new byte[]
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
			byte.MaxValue
		};

		private static readonly int[] dashPositions = new int[]
		{
			8,
			13,
			18,
			23
		};
	}
}
