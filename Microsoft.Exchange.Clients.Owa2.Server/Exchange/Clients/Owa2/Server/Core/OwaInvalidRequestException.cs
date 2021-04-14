using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Serializable]
	public sealed class OwaInvalidRequestException : OwaPermanentException
	{
		public OwaInvalidRequestException(string message, Exception innerException, object thisObject) : base(message, innerException, thisObject)
		{
		}

		public OwaInvalidRequestException(string message, Exception innerException) : this(message, innerException, null)
		{
		}

		public OwaInvalidRequestException(string message) : base(message)
		{
		}

		public OwaInvalidRequestException() : base(null)
		{
		}
	}
}
