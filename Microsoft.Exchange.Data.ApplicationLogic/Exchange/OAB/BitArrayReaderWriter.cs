using System;
using System.Collections;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.OAB
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class BitArrayReaderWriter
	{
		public static void WriteTo(BinaryWriter writer, BitArray bitArray)
		{
			byte[] array = new byte[(bitArray.Length + 7) / 8];
			for (int i = 0; i < bitArray.Length; i++)
			{
				if (bitArray.Get(i))
				{
					byte[] array2 = array;
					int num = i / 8;
					array2[num] |= BitArrayReaderWriter.BitArrayBitValue(i);
				}
			}
			writer.Write(array);
		}

		public static BitArray ReadFrom(BinaryReader reader, int count, string elementName)
		{
			BitArray bitArray = new BitArray(count);
			byte[] array = reader.ReadBytes((bitArray.Length + 7) / 8, elementName);
			for (int i = 0; i < count; i++)
			{
				bitArray.Set(i, (array[i / 8] & BitArrayReaderWriter.BitArrayBitValue(i)) != 0);
			}
			return bitArray;
		}

		private static byte BitArrayBitValue(int index)
		{
			return (byte)(128 >> index % 8);
		}
	}
}
