using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public class OwaParsingErrorException : OwaPermanentException
	{
		public OwaParsingErrorException() : base(null)
		{
		}

		public OwaParsingErrorException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public OwaParsingErrorException(string message) : base(message)
		{
		}
	}
}
