using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Hygiene.Cache.Data
{
	[Serializable]
	internal class CacheUnableToLockException : HygieneCacheException
	{
		public CacheUnableToLockException()
		{
		}

		public CacheUnableToLockException(string message) : base(message)
		{
		}

		public CacheUnableToLockException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected CacheUnableToLockException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
