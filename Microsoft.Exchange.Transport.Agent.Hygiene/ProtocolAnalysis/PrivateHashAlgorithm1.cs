using System;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis
{
	internal sealed class PrivateHashAlgorithm1
	{
		private static void Mix(ref uint a, ref uint b, ref uint c)
		{
			a -= b;
			a -= c;
			a ^= c >> 13;
			b -= c;
			b -= a;
			b ^= a << 8;
			c -= a;
			c -= b;
			c ^= b >> 13;
			a -= b;
			a -= c;
			a ^= c >> 12;
			b -= c;
			b -= a;
			b ^= a << 16;
			c -= a;
			c -= b;
			c ^= b >> 5;
			a -= b;
			a -= c;
			a ^= c >> 3;
			b -= c;
			b -= a;
			b ^= a << 10;
			c -= a;
			c -= b;
			c ^= b >> 15;
		}

		public static uint GetUInt32HashCode(byte[] data)
		{
			uint num = 0U;
			uint num2 = 2654435769U;
			uint num3 = 2654435769U;
			uint num4 = 3141592653U;
			while ((ulong)(num + 12U) <= (ulong)((long)data.Length))
			{
				num2 += (uint)((int)data[(int)((UIntPtr)num)] + ((int)data[(int)((UIntPtr)(num + 1U))] << 8) + ((int)data[(int)((UIntPtr)(num + 2U))] << 16) + ((int)data[(int)((UIntPtr)(num + 3U))] << 24));
				num3 += (uint)((int)data[(int)((UIntPtr)(num + 4U))] + ((int)data[(int)((UIntPtr)(num + 5U))] << 8) + ((int)data[(int)((UIntPtr)(num + 6U))] << 16) + ((int)data[(int)((UIntPtr)(num + 7U))] << 24));
				num4 += (uint)((int)data[(int)((UIntPtr)(num + 8U))] + ((int)data[(int)((UIntPtr)(num + 9U))] << 8) + ((int)data[(int)((UIntPtr)(num + 10U))] << 16) + ((int)data[(int)((UIntPtr)(num + 11U))] << 24));
				PrivateHashAlgorithm1.Mix(ref num2, ref num3, ref num4);
				num += 12U;
			}
			num4 += (uint)((long)data.Length - (long)((ulong)num));
			long num5 = (long)data.Length - (long)((ulong)num);
			if (num5 <= 11L && num5 >= 1L)
			{
				switch ((int)(num5 - 1L))
				{
				case 0:
					goto IL_17F;
				case 1:
					goto IL_173;
				case 2:
					goto IL_167;
				case 3:
					goto IL_15B;
				case 4:
					goto IL_152;
				case 5:
					goto IL_147;
				case 6:
					goto IL_13B;
				case 7:
					goto IL_12F;
				case 8:
					goto IL_124;
				case 9:
					break;
				case 10:
					num4 += (uint)((uint)data[(int)((UIntPtr)(num + 10U))] << 24);
					break;
				default:
					goto IL_186;
				}
				num4 += (uint)((uint)data[(int)((UIntPtr)(num + 9U))] << 16);
				IL_124:
				num4 += (uint)((uint)data[(int)((UIntPtr)(num + 8U))] << 8);
				IL_12F:
				num3 += (uint)((uint)data[(int)((UIntPtr)(num + 7U))] << 24);
				IL_13B:
				num3 += (uint)((uint)data[(int)((UIntPtr)(num + 6U))] << 16);
				IL_147:
				num3 += (uint)((uint)data[(int)((UIntPtr)(num + 5U))] << 8);
				IL_152:
				num3 += (uint)data[(int)((UIntPtr)(num + 4U))];
				IL_15B:
				num2 += (uint)((uint)data[(int)((UIntPtr)(num + 3U))] << 24);
				IL_167:
				num2 += (uint)((uint)data[(int)((UIntPtr)(num + 2U))] << 16);
				IL_173:
				num2 += (uint)((uint)data[(int)((UIntPtr)(num + 1U))] << 16);
				IL_17F:
				num2 += (uint)data[(int)((UIntPtr)num)];
			}
			IL_186:
			PrivateHashAlgorithm1.Mix(ref num2, ref num3, ref num4);
			return num4;
		}

		public static ushort GetUShort16HashCode(byte[] data)
		{
			uint uint32HashCode = PrivateHashAlgorithm1.GetUInt32HashCode(data);
			return (ushort)(uint32HashCode & 65535U);
		}
	}
}
