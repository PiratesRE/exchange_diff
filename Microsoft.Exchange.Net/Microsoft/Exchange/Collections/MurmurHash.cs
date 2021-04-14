using System;

namespace Microsoft.Exchange.Collections
{
	public class MurmurHash
	{
		[CLSCompliant(false)]
		public static uint Hash(byte[] data)
		{
			uint num = 3314489979U;
			uint num2 = 1540483477U;
			int num3 = 24;
			int i = data.Length;
			if (i == 0)
			{
				return 0U;
			}
			uint num4 = num ^ (uint)i;
			int num5 = 0;
			while (i >= 4)
			{
				uint num6 = BitConverter.ToUInt32(data, num5);
				num6 *= num2;
				num6 ^= num6 >> num3;
				num6 *= num2;
				num4 *= num2;
				num4 ^= num6;
				num5 += 4;
				i -= 4;
			}
			switch (i)
			{
			case 1:
				num4 ^= (uint)data[num5];
				num4 *= num2;
				break;
			case 2:
				num4 ^= (uint)BitConverter.ToUInt16(data, num5);
				num4 *= num2;
				break;
			case 3:
				num4 ^= (uint)BitConverter.ToUInt16(data, num5);
				num4 ^= (uint)((uint)data[num5 + 2] << 16);
				num4 *= num2;
				break;
			}
			num4 ^= num4 >> 13;
			num4 *= num2;
			return num4 ^ num4 >> 15;
		}
	}
}
