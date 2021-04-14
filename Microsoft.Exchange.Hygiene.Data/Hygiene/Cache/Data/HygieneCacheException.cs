using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Hygiene.Cache.Data
{
	[Serializable]
	internal class HygieneCacheException : Exception
	{
		public HygieneCacheException()
		{
		}

		public HygieneCacheException(string message) : base(message)
		{
		}

		public HygieneCacheException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected HygieneCacheException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
