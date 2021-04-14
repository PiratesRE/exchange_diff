using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Hygiene.Cache.Data
{
	[Serializable]
	internal class CacheWatchdogException : HygieneCacheException
	{
		public CacheWatchdogException()
		{
		}

		public CacheWatchdogException(string message) : base(message)
		{
		}

		public CacheWatchdogException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected CacheWatchdogException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
