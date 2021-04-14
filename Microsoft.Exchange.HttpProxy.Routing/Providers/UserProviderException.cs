using System;

namespace Microsoft.Exchange.HttpProxy.Routing.Providers
{
	public class UserProviderException : Exception
	{
		public UserProviderException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
