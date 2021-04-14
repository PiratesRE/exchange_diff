using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class ExtensionMethods
	{
		public static int GetHashCodeCaseInsensitive(this string value)
		{
			int length = value.Length;
			int num = 5381;
			for (int i = 0; i < length; i++)
			{
				int num2 = ExtensionMethods.ToLower((int)value[i]);
				num = (num << 5) + num + (num >> 27);
				num ^= num2 << 16 * (i & 1);
			}
			return num;
		}

		private static byte[] ComputeToLowerTable()
		{
			byte[] array = new byte[128];
			for (int i = 0; i < 128; i++)
			{
				array[i] = (byte)char.ToLowerInvariant((char)i);
			}
			return array;
		}

		private static int ToLower(int ch)
		{
			if (ch >= 128)
			{
				return (int)char.ToLowerInvariant((char)ch);
			}
			return (int)ExtensionMethods.toLower[ch];
		}

		private static readonly byte[] toLower = ExtensionMethods.ComputeToLowerTable();
	}
}
