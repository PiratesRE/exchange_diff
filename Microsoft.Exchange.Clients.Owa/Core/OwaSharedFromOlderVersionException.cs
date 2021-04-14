using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public class OwaSharedFromOlderVersionException : OwaPermanentException
	{
		public OwaSharedFromOlderVersionException() : base(null)
		{
		}

		public OwaSharedFromOlderVersionException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public OwaSharedFromOlderVersionException(string message) : base(message)
		{
		}
	}
}
