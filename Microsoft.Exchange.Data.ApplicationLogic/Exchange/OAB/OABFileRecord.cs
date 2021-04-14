using System;
using System.Collections;
using System.IO;
using Microsoft.Mapi;

namespace Microsoft.Exchange.OAB
{
	internal sealed class OABFileRecord : IWriteToBinaryWriter
	{
		public OABPropertyValue[] PropertyValues { get; set; }

		public static OABFileRecord ReadFrom(BinaryReader reader, PropTag[] properties, string elementName)
		{
			OABFileRecord oabfileRecord = new OABFileRecord();
			long position = reader.BaseStream.Position;
			int num = (int)reader.ReadUInt32(elementName + ".recordSize");
			BitArray bitArray = BitArrayReaderWriter.ReadFrom(reader, properties.Length + 1, elementName + ".presentArray");
			oabfileRecord.PropertyValues = new OABPropertyValue[properties.Length];
			for (int i = 0; i < properties.Length; i++)
			{
				if (bitArray.Get(i))
				{
					PropTag propTag = properties[i];
					oabfileRecord.PropertyValues[i] = OABPropertyValue.ReadFrom(reader, propTag, string.Concat(new object[]
					{
						elementName,
						".property[",
						propTag,
						"]"
					}));
				}
			}
			long position2 = reader.BaseStream.Position;
			int num2 = (int)(position2 - position);
			if (num2 != num)
			{
				throw new InvalidDataException(string.Format("Unable to read element '{0}' at position {1} because record size is different than actual data read from stream. Record size: {2}, actual data read: {3}", new object[]
				{
					elementName,
					position,
					num,
					num2
				}));
			}
			return oabfileRecord;
		}

		public void WriteTo(BinaryWriter writer)
		{
			long position = writer.BaseStream.Position;
			writer.Write(0);
			BitArray bitArray = new BitArray(this.PropertyValues.Length);
			for (int i = 0; i < this.PropertyValues.Length; i++)
			{
				if (this.PropertyValues[i] != null)
				{
					bitArray[i] = this.PropertyValues[i].IsWritable;
				}
			}
			BitArrayReaderWriter.WriteTo(writer, bitArray);
			for (int j = 0; j < this.PropertyValues.Length; j++)
			{
				OABPropertyValue oabpropertyValue = this.PropertyValues[j];
				if (oabpropertyValue != null && oabpropertyValue.IsWritable)
				{
					oabpropertyValue.WriteTo(writer);
				}
			}
			long position2 = writer.BaseStream.Position;
			int value = (int)(position2 - position);
			writer.BaseStream.Seek(position, SeekOrigin.Begin);
			writer.Write(value);
			writer.BaseStream.Seek(position2, SeekOrigin.Begin);
		}
	}
}
