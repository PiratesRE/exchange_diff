using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Data.ContentTypes.Tnef
{
	internal static class TnefBitConverter
	{
		public static void GetBytes(byte[] buffer, int offset, short value)
		{
			buffer[offset] = (byte)(value & 255);
			buffer[offset + 1] = (byte)(((int)value & 65280) >> 8);
		}

		public unsafe static void GetBytes(byte[] buffer, int offset, int value)
		{
			fixed (byte* ptr = buffer)
			{
				if (BitConverter.IsLittleEndian)
				{
					int* ptr2 = (int*)ptr + offset / 4;
					*ptr2 = value;
				}
				else
				{
					byte* ptr3 = ptr + offset;
					byte* ptr4 = (byte*)(&value);
					*ptr3 = ptr4[3];
					ptr3[1] = ptr4[2];
					ptr3[2] = ptr4[1];
					ptr3[3] = *ptr4;
				}
			}
		}

		public unsafe static void GetBytes(byte[] buffer, int offset, long value)
		{
			fixed (byte* ptr = buffer)
			{
				if (BitConverter.IsLittleEndian)
				{
					long* ptr2 = (long*)ptr + offset / 8;
					*ptr2 = value;
				}
				else
				{
					byte* ptr3 = ptr + offset;
					byte* ptr4 = (byte*)(&value);
					*ptr3 = ptr4[7];
					ptr3[1] = ptr4[6];
					ptr3[2] = ptr4[5];
					ptr3[3] = ptr4[4];
					ptr3[4] = ptr4[3];
					ptr3[5] = ptr4[2];
					ptr3[6] = ptr4[1];
					ptr3[7] = *ptr4;
				}
			}
		}

		public unsafe static void GetBytes(byte[] buffer, int offset, float value)
		{
			fixed (byte* ptr = buffer)
			{
				if (BitConverter.IsLittleEndian)
				{
					float* ptr2 = (float*)ptr + offset / 4;
					*ptr2 = value;
				}
				else
				{
					byte* ptr3 = ptr + offset;
					byte* ptr4 = (byte*)(&value);
					*ptr3 = ptr4[3];
					ptr3[1] = ptr4[2];
					ptr3[2] = ptr4[1];
					ptr3[3] = *ptr4;
				}
			}
		}

		public unsafe static void GetBytes(byte[] buffer, int offset, double value)
		{
			fixed (byte* ptr = buffer)
			{
				if (BitConverter.IsLittleEndian)
				{
					double* ptr2 = (double*)ptr + offset / 8;
					*ptr2 = value;
				}
				else
				{
					byte* ptr3 = ptr + offset;
					byte* ptr4 = (byte*)(&value);
					*ptr3 = ptr4[7];
					ptr3[1] = ptr4[6];
					ptr3[2] = ptr4[5];
					ptr3[3] = ptr4[4];
					ptr3[4] = ptr4[3];
					ptr3[5] = ptr4[2];
					ptr3[6] = ptr4[1];
					ptr3[7] = *ptr4;
				}
			}
		}

		private unsafe static bool InitNiceLittleEndianGuidLayout()
		{
			Guid guid = new Guid(1732584193, -21623, -4147, 254, 220, 186, 152, 118, 84, 50, 16);
			byte[] array = new byte[]
			{
				1,
				35,
				69,
				103,
				137,
				171,
				205,
				239,
				254,
				220,
				186,
				152,
				118,
				84,
				50,
				16
			};
			if (Marshal.SizeOf(typeof(Guid)) == 16)
			{
				fixed (byte* ptr = array)
				{
					byte* ptr2 = ptr;
					byte* ptr3 = ptr2 + 16;
					byte* ptr4 = (byte*)(&guid);
					while (ptr2 != ptr3 && *ptr2 == *ptr4)
					{
						ptr2++;
						ptr4++;
					}
					if (ptr2 == ptr3)
					{
						return true;
					}
				}
			}
			byte[] array2 = guid.ToByteArray();
			int num = 0;
			if (array2.Length == 16)
			{
				while (num < 16 && array2[num] == array[num])
				{
					num++;
				}
			}
			if (num != 16)
			{
				throw new NotSupportedException("The application cannot run on this platform, Guid format is incompatible.");
			}
			return false;
		}

		public unsafe static void GetBytes(byte[] buffer, int offset, Guid value)
		{
			if (TnefBitConverter.niceLittleEndianGuidLayout)
			{
				fixed (byte* ptr = buffer)
				{
					Guid* ptr2 = (Guid*)ptr + offset / sizeof(Guid);
					*ptr2 = value;
				}
				return;
			}
			byte[] src = value.ToByteArray();
			Buffer.BlockCopy(src, 0, buffer, offset, 16);
		}

		private static bool niceLittleEndianGuidLayout = TnefBitConverter.InitNiceLittleEndianGuidLayout();
	}
}
