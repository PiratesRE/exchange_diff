using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public class OwaArchiveInTransitException : OwaPermanentException
	{
		public OwaArchiveInTransitException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
