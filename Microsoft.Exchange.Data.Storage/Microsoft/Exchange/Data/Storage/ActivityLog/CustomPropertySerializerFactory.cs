using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage.Clutter;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ActivityLog
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class CustomPropertySerializerFactory
	{
		static CustomPropertySerializerFactory()
		{
			CustomPropertySerializerFactory.serializerVersions.Add(1, () => new CustomPropertySerializerV1());
			CustomPropertySerializerFactory.maxVersion = CustomPropertySerializerFactory.serializerVersions.Keys.Max();
		}

		public static AbstractCustomPropertySerializer GetSerializer()
		{
			return CustomPropertySerializerFactory.GetSerializer(CustomPropertySerializerFactory.maxVersion);
		}

		public static AbstractCustomPropertySerializer GetDeserializer(byte[] bytes)
		{
			ArgumentValidator.ThrowIfNull("bytes", bytes);
			if (bytes.Length < 3)
			{
				InferenceDiagnosticsLog.Log("CustomPropertySerializerFactory.GetDeserializer", string.Format("Cannot deserialize a byte array that does not have a header. Length of bytes: '{0}'", bytes.Length));
				return null;
			}
			int num = (int)bytes[0];
			int num2 = (int)bytes[1];
			if (num <= CustomPropertySerializerFactory.maxVersion)
			{
				return CustomPropertySerializerFactory.GetSerializer(num);
			}
			if (num2 <= CustomPropertySerializerFactory.maxVersion)
			{
				return CustomPropertySerializerFactory.GetSerializer(CustomPropertySerializerFactory.maxVersion);
			}
			InferenceDiagnosticsLog.Log("Activity.DeserializeCustomPropertiesDictionary", string.Format("Unable to find a serializer for deserializing. Version used for serializing '{0}', MinVersion supported '{1}', MaxVersion understood by factory '{2}'", num, num2, CustomPropertySerializerFactory.maxVersion));
			return null;
		}

		private static AbstractCustomPropertySerializer GetSerializer(int version)
		{
			Func<AbstractCustomPropertySerializer> func;
			bool flag = CustomPropertySerializerFactory.serializerVersions.TryGetValue(version, out func);
			if (flag)
			{
				return func();
			}
			return null;
		}

		private static readonly Dictionary<int, Func<AbstractCustomPropertySerializer>> serializerVersions = new Dictionary<int, Func<AbstractCustomPropertySerializer>>();

		private static readonly int maxVersion;
	}
}
