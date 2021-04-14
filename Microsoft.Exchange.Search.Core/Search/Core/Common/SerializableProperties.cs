using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal sealed class SerializableProperties
	{
		internal SerializableProperties(params ISerializableProperty[] properties)
		{
			Util.ThrowOnNullOrEmptyArgument<ISerializableProperty>(properties, "properties");
			HashSet<SerializablePropertyId> hashSet = new HashSet<SerializablePropertyId>();
			foreach (ISerializableProperty serializableProperty in properties)
			{
				if (hashSet.Contains(serializableProperty.Id))
				{
					throw new ArgumentException(string.Format("Duplicated property Id {0}", serializableProperty.Id));
				}
				hashSet.Add(serializableProperty.Id);
			}
			this.properties = new List<ISerializableProperty>(properties);
		}

		public static IEnumerable<ISerializableProperty> DeserializeFrom(Stream inputStream)
		{
			BinaryReader reader = new BinaryReader(inputStream);
			byte[] propertyTypeByte = new byte[1];
			while (reader.Read(propertyTypeByte, 0, 1) != 0)
			{
				ISerializableProperty property;
				switch (propertyTypeByte[0])
				{
				case 0:
					property = new SerializableStreamProperty(reader);
					break;
				case 1:
					property = new SerializableStringProperty();
					break;
				default:
					throw new InvalidDataException(string.Format("Invalid Property Type {0}", propertyTypeByte));
				}
				property.Deserialize(reader);
				yield return property;
			}
			yield break;
		}

		public void SerializeTo(Stream outputStream)
		{
			foreach (ISerializableProperty serializableProperty in this.properties)
			{
				BinaryWriter binaryWriter = new BinaryWriter(outputStream);
				binaryWriter.Write((byte)serializableProperty.Type);
				serializableProperty.Serialize(binaryWriter);
			}
		}

		private readonly List<ISerializableProperty> properties;
	}
}
