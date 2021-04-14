using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct PropertyRow
	{
		public PropertyRow(PropertyTag[] columns, PropertyValue[] propertyValues)
		{
			Util.ThrowOnNullArgument(columns, "columns");
			Util.ThrowOnNullArgument(propertyValues, "propertyValues");
			this.columns = columns;
			this.propertyValues = propertyValues;
		}

		internal PropertyValue[] PropertyValues
		{
			get
			{
				return this.propertyValues;
			}
		}

		internal static PropertyRow Parse(Reader reader, PropertyTag[] columns, WireFormatStyle wireFormatStyle)
		{
			Util.ThrowOnNullArgument(reader, "reader");
			Util.ThrowOnNullArgument(columns, "columns");
			bool flag = reader.ReadBool();
			PropertyValue[] array = new PropertyValue[columns.Length];
			for (int i = 0; i < columns.Length; i++)
			{
				PropertyTag propertyTag = PropertyTag.RemoveMviWithMvIfNeeded(columns[i]);
				if (propertyTag.PropertyType == PropertyType.Unspecified)
				{
					PropertyType propertyType = (PropertyType)reader.ReadUInt16();
					propertyTag = new PropertyTag(propertyTag.PropertyId, propertyType);
				}
				if (flag)
				{
					byte b = reader.ReadByte();
					if (b != 0)
					{
						PropertyType propertyType2 = (PropertyType)b;
						propertyTag = new PropertyTag(propertyTag.PropertyId, propertyType2);
					}
				}
				array[i] = reader.ReadPropertyValueForTag(propertyTag, wireFormatStyle);
			}
			return new PropertyRow(columns, array);
		}

		internal static bool RemoveLargestValue(PropertyValue[] propertyValues)
		{
			int num = 0;
			int num2 = -1;
			int num3 = -1;
			int num4 = -1;
			int num5 = -1;
			for (int i = 0; i < propertyValues.Length; i++)
			{
				PropertyValue propertyValue = propertyValues[i];
				int num6 = 0;
				PropertyId propertyId = propertyValue.PropertyTag.PropertyId;
				if (propertyId != PropertyId.Body)
				{
					if (propertyId != PropertyId.RtfCompressed)
					{
						if (propertyId == PropertyId.Html)
						{
							num3 = i;
						}
					}
					else
					{
						num4 = i;
					}
				}
				else
				{
					num5 = i;
				}
				if (propertyValue.PropertyTag.PropertyType == PropertyType.Binary)
				{
					num6 = propertyValue.GetValueAssert<byte[]>().Length;
				}
				else if (propertyValue.PropertyTag.PropertyType == PropertyType.String8)
				{
					num6 = propertyValue.GetValueAssert<string>().Length;
				}
				else if (propertyValue.PropertyTag.PropertyType == PropertyType.Unicode)
				{
					num6 = propertyValue.GetValueAssert<string>().Length * 2;
				}
				if (num6 > num)
				{
					num = num6;
					num2 = i;
				}
			}
			if (num < 255)
			{
				return false;
			}
			PropertyId propertyId2 = propertyValues[num2].PropertyTag.PropertyId;
			propertyValues[num2] = PropertyValue.CreateNotEnoughMemory(propertyId2);
			if (PropertyRow.IsBodyPropertyId(propertyId2) && num3 >= 0 && num4 >= 0 && num5 >= 0)
			{
				if (!propertyValues[num3].IsError)
				{
					propertyValues[num3] = PropertyValue.CreateNotEnoughMemory(PropertyId.Html);
				}
				if (!propertyValues[num4].IsError)
				{
					propertyValues[num4] = PropertyValue.CreateNotEnoughMemory(PropertyId.RtfCompressed);
				}
				if (!propertyValues[num5].IsError)
				{
					propertyValues[num5] = PropertyValue.CreateNotEnoughMemory(PropertyId.Body);
				}
			}
			return true;
		}

		internal void Serialize(Writer writer, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (string8Encoding == null)
			{
				throw new ArgumentNullException("string8Encoding");
			}
			bool flag = false;
			for (int i = 0; i < this.columns.Length; i++)
			{
				if (this.propertyValues[i].PropertyTag.PropertyId != this.columns[i].PropertyId)
				{
					throw new InvalidOperationException(string.Format("Attempted to serialize {0} instead of {1}", this.propertyValues[i], this.columns[i]));
				}
				if (this.propertyValues[i].PropertyTag.PropertyType == PropertyType.Error || this.propertyValues[i].PropertyTag.PropertyType == PropertyType.Null)
				{
					flag = true;
					break;
				}
			}
			writer.WriteBool(flag, 1);
			for (int j = 0; j < this.columns.Length; j++)
			{
				if (this.columns[j].PropertyType == PropertyType.Unspecified)
				{
					writer.WriteUInt16((ushort)this.propertyValues[j].PropertyTag.PropertyType);
				}
				if (flag)
				{
					if (this.propertyValues[j].PropertyTag.PropertyType == PropertyType.Null || this.propertyValues[j].PropertyTag.PropertyType == PropertyType.Error)
					{
						writer.WriteByte((byte)this.propertyValues[j].PropertyTag.PropertyType);
					}
					else
					{
						writer.WriteByte(0);
					}
				}
				writer.WritePropertyValueWithoutTag(this.propertyValues[j], string8Encoding, wireFormatStyle);
			}
		}

		internal void ResolveString8Values(Encoding string8Encoding)
		{
			foreach (PropertyValue propertyValue in this.propertyValues)
			{
				propertyValue.ResolveString8Values(string8Encoding);
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.AppendToString(stringBuilder);
			return stringBuilder.ToString();
		}

		internal void AppendToString(StringBuilder stringBuilder)
		{
			if (this.propertyValues != null)
			{
				for (int i = 0; i < this.propertyValues.Length; i++)
				{
					if (i != 0)
					{
						stringBuilder.Append(" ");
					}
					stringBuilder.Append("[");
					this.propertyValues[i].AppendToString(stringBuilder);
					stringBuilder.Append("]");
				}
			}
		}

		private static bool IsBodyPropertyId(PropertyId largestPropertyId)
		{
			return largestPropertyId == PropertyId.Html || largestPropertyId == PropertyId.RtfCompressed || largestPropertyId == PropertyId.Body;
		}

		private readonly PropertyTag[] columns;

		private readonly PropertyValue[] propertyValues;

		internal static readonly PropertyRow Empty = new PropertyRow(Array<PropertyTag>.Empty, Array<PropertyValue>.Empty);
	}
}
