using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public class OwaInvalidWebPartRequestException : OwaPermanentException
	{
		public OwaInvalidWebPartRequestException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public OwaInvalidWebPartRequestException(string message) : base(message)
		{
		}
	}
}
