using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.SharedCache.Client
{
	public static class SerializationHelper
	{
		public static T Deserialize<T>(byte[] serializedBytes) where T : ISharedCacheEntry, new()
		{
			ArgumentValidator.ThrowIfNull("serializedBytes", serializedBytes);
			T result = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
			result.FromByteArray(serializedBytes);
			return result;
		}

		public static byte[] Serialize(ISharedCacheEntry serializableObject)
		{
			ArgumentValidator.ThrowIfNull("serializableObject", serializableObject);
			return serializableObject.ToByteArray();
		}
	}
}
