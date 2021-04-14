using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public class OwaDefaultFolderIdUnavailableException : OwaPermanentException
	{
		public OwaDefaultFolderIdUnavailableException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public OwaDefaultFolderIdUnavailableException(string message) : base(message)
		{
		}
	}
}
