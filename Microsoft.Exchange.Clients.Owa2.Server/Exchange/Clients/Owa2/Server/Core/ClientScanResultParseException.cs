using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Serializable]
	internal class ClientScanResultParseException : OwaPermanentException
	{
		public ClientScanResultParseException(string errorMessage) : this(errorMessage, null)
		{
		}

		public ClientScanResultParseException(string errorMessage, Exception innerException) : base(errorMessage, innerException)
		{
		}
	}
}
