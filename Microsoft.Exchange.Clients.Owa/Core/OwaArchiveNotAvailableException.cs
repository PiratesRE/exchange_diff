using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public class OwaArchiveNotAvailableException : OwaPermanentException
	{
		public OwaArchiveNotAvailableException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
