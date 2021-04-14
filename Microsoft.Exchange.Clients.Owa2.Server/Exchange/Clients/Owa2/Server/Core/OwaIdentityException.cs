using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Serializable]
	public class OwaIdentityException : OwaTransientException
	{
		public OwaIdentityException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public OwaIdentityException(string message) : base(message)
		{
		}
	}
}
