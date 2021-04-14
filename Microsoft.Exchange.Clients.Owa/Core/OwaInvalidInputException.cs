using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaInvalidInputException : OwaPermanentException
	{
		public OwaInvalidInputException(string message, Exception innerException, object thisObject) : base(message, innerException, thisObject)
		{
		}

		public OwaInvalidInputException(string message) : base(message)
		{
		}
	}
}
