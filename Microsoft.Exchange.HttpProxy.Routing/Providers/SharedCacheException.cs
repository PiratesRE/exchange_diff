using System;

namespace Microsoft.Exchange.HttpProxy.Routing.Providers
{
	public class SharedCacheException : Exception
	{
		public SharedCacheException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
