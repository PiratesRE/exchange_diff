using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaInvalidOperationException : OwaPermanentException
	{
		public OwaInvalidOperationException(string message, Exception innerException, object thisObject) : base(message, innerException, thisObject)
		{
		}

		public OwaInvalidOperationException(string message) : base(message)
		{
		}
	}
}
