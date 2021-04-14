using System;
using System.IO;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal sealed class SerializableStringProperty : SerializableProperty<string>
	{
		internal SerializableStringProperty(SerializablePropertyId id, string value) : base(id, value)
		{
		}

		internal SerializableStringProperty()
		{
		}

		public override SerializablePropertyType Type
		{
			get
			{
				return SerializablePropertyType.String;
			}
		}

		protected override void SerializeValue(BinaryWriter writer)
		{
			writer.Write(base.PropertyValue);
		}

		protected override void DeserializeValue(BinaryReader reader)
		{
			base.PropertyValue = reader.ReadString();
		}
	}
}
