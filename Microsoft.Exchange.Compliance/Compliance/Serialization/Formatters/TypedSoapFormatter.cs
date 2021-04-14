using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;

namespace Microsoft.Exchange.Compliance.Serialization.Formatters
{
	internal sealed class TypedSoapFormatter : TypedSerializationFormatter
	{
		public static object DeserializeObject(Stream serializationStream, TypedSerializationFormatter.TypeBinder binder)
		{
			if (!binder.IsInitialized)
			{
				throw new ArgumentException("binder", "Binder must be initialized before use");
			}
			return TypedSoapFormatter.Deserialize(serializationStream, binder);
		}

		public static object DeserializeObject(Stream serializationStream, Type[] expectedTypes, TypedSerializationFormatter.TypeEncounteredDelegate typeEncountered, bool strict)
		{
			return TypedSoapFormatter.Deserialize(serializationStream, new TypedSerializationFormatter.TypeBinder(expectedTypes, typeEncountered, strict));
		}

		public static object DeserializeObject(Stream serializationStream, Type[] expectedTypes, Type[] baseClasses, TypedSerializationFormatter.TypeEncounteredDelegate typeEncountered, bool strict)
		{
			return TypedSoapFormatter.Deserialize(serializationStream, new TypedSerializationFormatter.TypeBinder(expectedTypes, baseClasses, typeEncountered, strict));
		}

		public static object DeserializeObject(Stream serializationStream, Dictionary<Type, string> expectedTypes, TypedSerializationFormatter.TypeEncounteredDelegate typeEncountered)
		{
			if (expectedTypes == null || expectedTypes.Count == 0)
			{
				throw new ArgumentException("expectedTypes", "ExpectedTypes must be initialized before use");
			}
			return TypedSoapFormatter.Deserialize(serializationStream, new TypedSerializationFormatter.TypeBinder(expectedTypes, typeEncountered));
		}

		private static object Deserialize(Stream serializationStream, SerializationBinder binder)
		{
			return new SoapFormatter
			{
				Binder = binder
			}.Deserialize(serializationStream);
		}
	}
}
