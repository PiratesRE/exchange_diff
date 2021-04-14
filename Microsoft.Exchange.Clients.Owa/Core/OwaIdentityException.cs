using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public class OwaIdentityException : OwaTransientException
	{
		public OwaIdentityException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
