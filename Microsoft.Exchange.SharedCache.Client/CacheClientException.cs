using System;

namespace Microsoft.Exchange.SharedCache.Client
{
	public class CacheClientException : Exception
	{
		public CacheClientException(string message) : base(message)
		{
		}

		public CacheClientException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
