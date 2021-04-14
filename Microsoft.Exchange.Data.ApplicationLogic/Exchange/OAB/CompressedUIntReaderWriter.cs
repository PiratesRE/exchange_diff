using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.OAB
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class CompressedUIntReaderWriter
	{
		public static void WriteTo(BinaryWriter writer, uint value)
		{
			if (value <= 127U)
			{
				writer.Write((byte)value);
				return;
			}
			int i;
			if (value <= 255U)
			{
				i = 1;
			}
			else if (value <= 65535U)
			{
				i = 2;
			}
			else if (value <= 16777215U)
			{
				i = 3;
			}
			else
			{
				i = 4;
			}
			writer.Write((byte)(128 + i));
			while (i > 0)
			{
				writer.Write((byte)(value & 255U));
				value >>= 8;
				i--;
			}
		}

		public static uint ReadFrom(BinaryReader reader, string elementName)
		{
			long position = reader.BaseStream.Position;
			byte b = reader.ReadByte(elementName);
			if (b <= 127)
			{
				return (uint)b;
			}
			int num;
			switch (b)
			{
			case 129:
				num = 1;
				break;
			case 130:
				num = 2;
				break;
			case 131:
				num = 3;
				break;
			case 132:
				num = 4;
				break;
			default:
				throw new InvalidDataException(string.Format("Unable to read compressed uint element '{0}' from stream at position {1} due unexpected head value: {2}", elementName, position, b));
			}
			uint num2 = 0U;
			int num3 = 0;
			for (int i = 0; i < num; i++)
			{
				byte b2 = reader.ReadByte(string.Concat(new object[]
				{
					elementName,
					".byte[",
					i,
					"]"
				}));
				num2 |= (uint)((uint)b2 << num3);
				num3 += 8;
			}
			return num2;
		}
	}
}
