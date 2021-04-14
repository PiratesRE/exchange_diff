using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaAsyncRequestTimeoutException : OwaTransientException
	{
		public OwaAsyncRequestTimeoutException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
