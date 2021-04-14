using System;
using System.Collections.Concurrent;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class SerializerCache
	{
		public static XmlSerializer GetSerializer(Type type)
		{
			return SerializerCache.cache.GetOrAdd(type, (Type t) => new XmlSerializer(t));
		}

		private static ConcurrentDictionary<Type, XmlSerializer> cache = new ConcurrentDictionary<Type, XmlSerializer>();
	}
}
