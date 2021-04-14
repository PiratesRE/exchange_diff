using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Compliance.Serialization.Formatters
{
	internal sealed class TypedBinaryFormatter : TypedSerializationFormatter
	{
		public static object DeserializeObject(Stream serializationStream, TypedSerializationFormatter.TypeBinder binder)
		{
			if (!binder.IsInitialized)
			{
				throw new ArgumentException("binder", "Binder must be initialized before use");
			}
			return TypedBinaryFormatter.Deserialize(serializationStream, binder);
		}

		public static object DeserializeObject(Stream serializationStream, Type[] expectedTypes, TypedSerializationFormatter.TypeEncounteredDelegate typeEncountered, bool strict)
		{
			return TypedBinaryFormatter.Deserialize(serializationStream, new TypedSerializationFormatter.TypeBinder(expectedTypes, typeEncountered, strict));
		}

		public static object DeserializeObject(Stream serializationStream, Type[] expectedTypes, Type[] baseClasses, TypedSerializationFormatter.TypeEncounteredDelegate typeEncountered, bool strict)
		{
			return TypedBinaryFormatter.Deserialize(serializationStream, new TypedSerializationFormatter.TypeBinder(expectedTypes, baseClasses, typeEncountered, strict));
		}

		public static object DeserializeObject(Stream serializationStream, Dictionary<Type, string> expectedTypes, TypedSerializationFormatter.TypeEncounteredDelegate typeEncountered)
		{
			if (expectedTypes == null || expectedTypes.Count == 0)
			{
				throw new ArgumentException("expectedTypes", "ExpectedTypes must be initialized before use");
			}
			return TypedBinaryFormatter.Deserialize(serializationStream, new TypedSerializationFormatter.TypeBinder(expectedTypes, typeEncountered));
		}

		private static object Deserialize(Stream serializationStream, SerializationBinder binder)
		{
			BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(binder, new string[]
			{
				"System.DelegateSerializationHolder"
			});
			return binaryFormatter.Deserialize(serializationStream);
		}
	}
}
