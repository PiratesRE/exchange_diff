using System;
using System.IO;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface ISerializableProperty
	{
		SerializablePropertyType Type { get; }

		SerializablePropertyId Id { get; }

		object Value { get; }

		void Serialize(BinaryWriter writer);

		void Deserialize(BinaryReader reader);
	}
}
