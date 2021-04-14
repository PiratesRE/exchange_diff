using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Serializable]
	public sealed class OwaInvalidIdFormatException : OwaPermanentException
	{
		public OwaInvalidIdFormatException(string message, Exception innerException, object thisObject) : base(message, innerException, thisObject)
		{
		}

		public OwaInvalidIdFormatException(string message, Exception innerException) : this(message, innerException, null)
		{
		}

		public OwaInvalidIdFormatException(string message) : base(message)
		{
		}

		public OwaInvalidIdFormatException() : base(null)
		{
		}
	}
}
