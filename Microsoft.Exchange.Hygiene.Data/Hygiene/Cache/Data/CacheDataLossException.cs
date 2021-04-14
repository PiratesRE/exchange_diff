using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Hygiene.Cache.Data
{
	[Serializable]
	internal class CacheDataLossException : HygieneCacheException
	{
		public CacheDataLossException()
		{
		}

		public CacheDataLossException(string message) : base(message)
		{
		}

		public CacheDataLossException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected CacheDataLossException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
