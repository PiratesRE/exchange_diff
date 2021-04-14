using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ByteArrayUtilities
	{
		private ByteArrayUtilities()
		{
		}

		public static void Clear(byte[] array)
		{
			if (array != null)
			{
				Array.Clear(array, 0, array.Length);
			}
		}

		public static byte[] CreateReversedArray(byte[] array)
		{
			if (array == null)
			{
				return null;
			}
			byte[] array2 = (byte[])array.Clone();
			Array.Reverse(array2);
			return array2;
		}

		public static uint ToUInt32(byte[] array)
		{
			return ByteArrayUtilities.ToUInt32(array, 0, array.Length);
		}

		public static uint ToUInt32(byte[] array, int offset, int length)
		{
			if (length > 4)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			uint num = 0U;
			for (int i = offset + length - 1; i >= offset; i--)
			{
				num = (num << 8) + (uint)array[i];
			}
			return num;
		}
	}
}
