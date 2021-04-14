using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Hygiene.Cache.Data
{
	[Serializable]
	internal class CacheDeserializationException : HygieneCacheException
	{
		public CacheDeserializationException()
		{
		}

		public CacheDeserializationException(string message) : base(message)
		{
		}

		public CacheDeserializationException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected CacheDeserializationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
