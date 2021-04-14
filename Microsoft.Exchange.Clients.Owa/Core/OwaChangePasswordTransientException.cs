using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public class OwaChangePasswordTransientException : OwaTransientException
	{
		public OwaChangePasswordTransientException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
