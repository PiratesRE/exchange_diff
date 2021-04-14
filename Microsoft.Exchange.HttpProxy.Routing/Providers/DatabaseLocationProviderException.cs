using System;

namespace Microsoft.Exchange.HttpProxy.Routing.Providers
{
	public class DatabaseLocationProviderException : Exception
	{
		public DatabaseLocationProviderException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
