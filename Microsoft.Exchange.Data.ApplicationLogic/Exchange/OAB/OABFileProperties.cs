using System;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.OAB
{
	internal sealed class OABFileProperties : IWriteToBinaryWriter
	{
		public OABPropertyDescriptor[] HeaderProperties { get; set; }

		public OABPropertyDescriptor[] DetailProperties { get; set; }

		public int Size
		{
			get
			{
				int num = (this.HeaderProperties != null) ? this.HeaderProperties.Length : 0;
				int num2 = (this.DetailProperties != null) ? this.DetailProperties.Length : 0;
				return 8 + num * OABPropertyDescriptor.Size + 4 + num2 * OABPropertyDescriptor.Size;
			}
		}

		public static OABFileProperties ReadFrom(BinaryReader reader, string elementName)
		{
			long position = reader.BaseStream.Position;
			int num = (int)reader.ReadUInt32(elementName + ".size");
			OABPropertyDescriptor[] headerProperties = OABFileProperties.ReadProperties(reader, elementName + ".headerProperties");
			OABPropertyDescriptor[] detailProperties = OABFileProperties.ReadProperties(reader, elementName + ".detailProperties");
			long position2 = reader.BaseStream.Position;
			int num2 = (int)(position2 - position);
			if (num2 != num)
			{
				throw new InvalidDataException(string.Format("Unable to read element '{0}' at position {1} because number of bytes read from stream doesn't match in size in header. Size in header: {2}, bytes read from stream: {3}", new object[]
				{
					elementName,
					position,
					num,
					num2
				}));
			}
			return new OABFileProperties
			{
				HeaderProperties = headerProperties,
				DetailProperties = detailProperties
			};
		}

		public void WriteTo(BinaryWriter writer)
		{
			writer.Write((uint)this.Size);
			OABFileProperties.WriteProperties(writer, this.HeaderProperties);
			OABFileProperties.WriteProperties(writer, this.DetailProperties);
		}

		private static OABPropertyDescriptor[] ReadProperties(BinaryReader reader, string elementName)
		{
			int num = (int)reader.ReadUInt32(elementName + ".count");
			OABPropertyDescriptor[] array = new OABPropertyDescriptor[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = OABPropertyDescriptor.ReadFrom(reader, string.Concat(new object[]
				{
					elementName,
					"[",
					i,
					"]"
				}));
			}
			return array;
		}

		private static void WriteProperties(BinaryWriter writer, OABPropertyDescriptor[] properties)
		{
			if (properties != null)
			{
				writer.Write((uint)properties.Length);
				foreach (OABPropertyDescriptor oabpropertyDescriptor in properties)
				{
					oabpropertyDescriptor.WriteTo(writer);
				}
				return;
			}
			writer.Write(0U);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(200);
			if (this.HeaderProperties != null)
			{
				stringBuilder.AppendLine("HeaderProperties:");
				foreach (OABPropertyDescriptor oabpropertyDescriptor in this.HeaderProperties)
				{
					stringBuilder.Append("  ");
					stringBuilder.AppendLine(oabpropertyDescriptor.ToString());
				}
			}
			if (this.DetailProperties != null)
			{
				stringBuilder.AppendLine("DetailProperties:");
				foreach (OABPropertyDescriptor oabpropertyDescriptor2 in this.DetailProperties)
				{
					stringBuilder.Append("  ");
					stringBuilder.AppendLine(oabpropertyDescriptor2.ToString());
				}
			}
			return stringBuilder.ToString();
		}
	}
}
