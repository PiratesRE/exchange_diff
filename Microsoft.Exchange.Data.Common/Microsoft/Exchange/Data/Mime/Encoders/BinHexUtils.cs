using System;

namespace Microsoft.Exchange.Data.Mime.Encoders
{
	internal class BinHexUtils
	{
		private BinHexUtils()
		{
		}

		internal static ushort CalculateHeaderCrc(byte[] bytes, int count)
		{
			ushort num = 0;
			for (int i = 0; i < count; i++)
			{
				byte b = bytes[i];
				for (int j = 0; j < 8; j++)
				{
					ushort num2 = num & 32768;
					num = (ushort)((int)num << 1 | b >> 7);
					if (num2 != 0)
					{
						num ^= 4129;
					}
					b = (byte)(b << 1);
				}
			}
			for (int k = 0; k < 2; k++)
			{
				for (int l = 0; l < 8; l++)
				{
					ushort num3 = num & 32768;
					num = (ushort)(num << 1);
					if (num3 != 0)
					{
						num ^= 4129;
					}
				}
			}
			return num;
		}

		internal static ushort CalculateCrc(byte[] bytes, int index, int size, ushort seed)
		{
			ushort num = seed;
			for (int i = index; i < index + size; i++)
			{
				byte b = bytes[i];
				for (int j = 0; j < 8; j++)
				{
					ushort num2 = num & 32768;
					num = (ushort)((int)num << 1 | b >> 7);
					if (num2 != 0)
					{
						num ^= 4129;
					}
					b = (byte)(b << 1);
				}
			}
			return num;
		}

		internal static ushort CalculateCrc(byte data, int count, ushort seed)
		{
			ushort num = seed;
			while (0 < count--)
			{
				byte b = data;
				for (int i = 0; i < 8; i++)
				{
					ushort num2 = num & 32768;
					num = (ushort)((int)num << 1 | b >> 7);
					if (num2 != 0)
					{
						num ^= 4129;
					}
					b = (byte)(b << 1);
				}
			}
			return num;
		}

		internal static ushort CalculateCrc(ushort seed)
		{
			byte[] array = new byte[2];
			byte[] array2 = array;
			return BinHexUtils.CalculateCrc(array2, 0, array2.Length, seed);
		}

		internal static int MarshalInt32(byte[] array, int offset, long value)
		{
			array[offset] = (byte)((value & (long)((ulong)-16777216)) >> 24);
			array[offset + 1] = (byte)((value & 16711680L) >> 16);
			array[offset + 2] = (byte)((value & 65280L) >> 8);
			array[offset + 3] = (byte)(value & 255L);
			return 4;
		}

		internal static int UnmarshalInt32(byte[] array, int index)
		{
			uint num = (uint)array[index];
			num = (num << 8 | (uint)array[index + 1]);
			num = (num << 8 | (uint)array[index + 2]);
			return (int)(num << 8 | (uint)array[index + 3]);
		}

		internal static int MarshalUInt16(byte[] array, int offset, ushort value)
		{
			array[offset] = (byte)((value & 65280) >> 8);
			array[offset + 1] = (byte)(value & 255);
			return 2;
		}

		internal static ushort UnmarshalUInt16(byte[] array, int index)
		{
			uint num = (uint)array[index];
			num = (num << 8 | (uint)array[index + 1]);
			return (ushort)num;
		}

		internal enum State
		{
			Starting,
			Started,
			Prefix,
			HdrFileSize,
			Header,
			HeaderCRC,
			Data,
			DataCRC,
			Resource,
			ResourceCRC,
			Ending,
			Ended
		}
	}
}
