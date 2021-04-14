using System;
using System.IO;

namespace Microsoft.Exchange.Data.ContentTypes.Tnef
{
	public struct TnefPropertyReader
	{
		internal TnefPropertyReader(TnefReader reader)
		{
			this.Reader = reader;
		}

		public int RowCount
		{
			get
			{
				return this.Reader.RowCount;
			}
		}

		public int PropertyCount
		{
			get
			{
				return this.Reader.PropertyCount;
			}
		}

		public TnefPropertyTag PropertyTag
		{
			get
			{
				return this.Reader.PropertyTag;
			}
		}

		public bool IsMultiValuedProperty
		{
			get
			{
				return this.Reader.PropertyTag.IsMultiValued;
			}
		}

		public int ValueCount
		{
			get
			{
				return this.Reader.PropertyValueCount;
			}
		}

		public bool IsNamedProperty
		{
			get
			{
				return this.Reader.PropertyTag.IsNamed;
			}
		}

		public TnefNameId PropertyNameId
		{
			get
			{
				return this.Reader.PropertyNameId;
			}
		}

		public Type ValueType
		{
			get
			{
				return this.Reader.PropertyValueClrType;
			}
		}

		public bool IsObjectProperty
		{
			get
			{
				return this.Reader.PropertyTag.ValueTnefType == TnefPropertyType.Object;
			}
		}

		public Guid ObjectIid
		{
			get
			{
				return this.Reader.PropertyValueOleIID;
			}
		}

		public bool IsEmbeddedMessage
		{
			get
			{
				return this.Reader.IsPropertyEmbeddedMessage;
			}
		}

		public bool IsLargeValue
		{
			get
			{
				return this.Reader.IsLargePropertyValue;
			}
		}

		public bool IsComputedProperty
		{
			get
			{
				return this.Reader.IsComputedProperty;
			}
		}

		public int RawValueStreamOffset
		{
			get
			{
				return this.Reader.PropertyRawValueStreamOffset;
			}
		}

		public int RawValueLength
		{
			get
			{
				return this.Reader.PropertyRawValueLength;
			}
		}

		public bool ReadValueAsBoolean()
		{
			return this.Reader.ReadPropertyValueAsBool();
		}

		public short ReadValueAsInt16()
		{
			return this.Reader.ReadPropertyValueAsShort();
		}

		public int ReadValueAsInt32()
		{
			return this.Reader.ReadPropertyValueAsInt();
		}

		public long ReadValueAsInt64()
		{
			return this.Reader.ReadPropertyValueAsLong();
		}

		public Guid ReadValueAsGuid()
		{
			return this.Reader.ReadPropertyValueAsGuid();
		}

		public float ReadValueAsFloat()
		{
			return this.Reader.ReadPropertyValueAsFloat();
		}

		public double ReadValueAsDouble()
		{
			return this.Reader.ReadPropertyValueAsDouble();
		}

		public DateTime ReadValueAsDateTime()
		{
			return this.Reader.ReadPropertyValueAsDateTime();
		}

		public string ReadValueAsString()
		{
			return this.Reader.ReadPropertyValueAsString();
		}

		public byte[] ReadValueAsBytes()
		{
			return this.Reader.ReadPropertyValueAsByteArray();
		}

		public object ReadValue()
		{
			return this.Reader.ReadPropertyValue();
		}

		public int ReadTextValue(char[] buffer, int offset, int count)
		{
			return this.Reader.ReadPropertyTextValue(buffer, offset, count);
		}

		public int ReadRawValue(byte[] buffer, int offset, int count)
		{
			return this.Reader.ReadPropertyRawValue(buffer, offset, count, false);
		}

		public TnefReader GetEmbeddedMessageReader()
		{
			return this.Reader.GetEmbeddedMessageReader();
		}

		public Stream GetRawValueReadStream()
		{
			return this.Reader.GetRawPropertyValueReadStream();
		}

		public bool ReadNextValue()
		{
			return this.Reader.ReadNextPropertyValue();
		}

		public bool ReadNextProperty()
		{
			return this.Reader.ReadNextProperty();
		}

		public bool ReadNextRow()
		{
			return this.Reader.ReadNextRow();
		}

		internal TnefReader Reader;
	}
}
