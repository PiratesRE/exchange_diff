using System;
using System.IO;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal abstract class SerializableProperty<DataType> : ISerializableProperty
	{
		protected SerializableProperty()
		{
		}

		protected SerializableProperty(SerializablePropertyId id, DataType value)
		{
			this.propertyId = id;
			this.propertyValue = value;
		}

		public abstract SerializablePropertyType Type { get; }

		public SerializablePropertyId Id
		{
			get
			{
				return this.propertyId;
			}
			protected set
			{
				this.propertyId = value;
			}
		}

		public object Value
		{
			get
			{
				return this.propertyValue;
			}
		}

		protected DataType PropertyValue
		{
			get
			{
				return this.propertyValue;
			}
			set
			{
				this.propertyValue = value;
			}
		}

		public void Serialize(BinaryWriter writer)
		{
			Util.ThrowOnNullArgument(writer, "writer");
			writer.Write((byte)this.Id);
			this.SerializeValue(writer);
		}

		public void Deserialize(BinaryReader reader)
		{
			Util.ThrowOnNullArgument(reader, "reader");
			byte b = reader.ReadByte();
			if (b <= 0 || b >= 16)
			{
				throw new InvalidDataException(string.Format("Invalid Property Id {0}", b));
			}
			this.Id = (SerializablePropertyId)b;
			this.DeserializeValue(reader);
		}

		protected abstract void SerializeValue(BinaryWriter writer);

		protected abstract void DeserializeValue(BinaryReader reader);

		private SerializablePropertyId propertyId;

		private DataType propertyValue;
	}
}
