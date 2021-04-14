using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaAsyncOperationException : OwaPermanentException
	{
		public OwaAsyncOperationException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
