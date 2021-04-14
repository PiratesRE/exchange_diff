using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal static class BinaryOrdinalGenerator
	{
		public static byte[] GetInbetweenOrdinalValue(byte[] beforeOrdinal, byte[] afterOrdinal)
		{
			int num = 0;
			int num2 = Math.Max((beforeOrdinal != null) ? beforeOrdinal.Length : 0, (afterOrdinal != null) ? afterOrdinal.Length : 0) + 1;
			byte[] array = new byte[num2];
			if (beforeOrdinal != null && afterOrdinal != null && Utilities.CompareByteArrays(beforeOrdinal, afterOrdinal) >= 0)
			{
				throw new OwaInvalidOperationException("Previous ordinal value is greater then after ordinal value");
			}
			if (beforeOrdinal != null && BinaryOrdinalGenerator.CheckAllZero(beforeOrdinal))
			{
				beforeOrdinal = null;
			}
			if (afterOrdinal != null && BinaryOrdinalGenerator.CheckAllZero(afterOrdinal))
			{
				afterOrdinal = null;
			}
			byte beforeVal;
			byte afterVal;
			for (;;)
			{
				beforeVal = BinaryOrdinalGenerator.GetBeforeVal(num, beforeOrdinal);
				afterVal = BinaryOrdinalGenerator.GetAfterVal(num, afterOrdinal);
				if (afterVal > beforeVal + 1)
				{
					break;
				}
				array[num++] = beforeVal;
				if (beforeVal + 1 == afterVal)
				{
					afterOrdinal = null;
				}
			}
			array[num++] = (afterVal + beforeVal) / 2;
			byte[] array2 = new byte[num];
			Array.Copy(array, array2, num);
			return array2;
		}

		private static bool CheckAllZero(byte[] bytes)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			foreach (byte b in bytes)
			{
				if (b != 0)
				{
					return false;
				}
			}
			return true;
		}

		private static byte GetValEx(int index, byte newValue, byte[] ordinal)
		{
			if (index >= ordinal.Length)
			{
				return newValue;
			}
			return ordinal[index];
		}

		private static byte GetBeforeVal(int index, byte[] beforeOrdinal)
		{
			if (beforeOrdinal == null)
			{
				return 0;
			}
			return BinaryOrdinalGenerator.GetValEx(index, 0, beforeOrdinal);
		}

		private static byte GetAfterVal(int index, byte[] afterOrdinal)
		{
			if (afterOrdinal == null)
			{
				return byte.MaxValue;
			}
			return BinaryOrdinalGenerator.GetValEx(index, byte.MaxValue, afterOrdinal);
		}

		private const byte MinValue = 0;

		private const byte MaxValue = 255;
	}
}
