using System;

namespace Microsoft.Exchange.Assistants
{
	internal class DisconnectedMailboxException : AIMailboxUnavailableException
	{
		public DisconnectedMailboxException(Exception innerException) : base(Strings.descDisconnectedMailboxException, innerException)
		{
		}
	}
}
