using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public interface IMRSRemoteException
	{
		string OriginalFailureType { get; set; }

		WellKnownException[] WKEClasses { get; set; }

		int MapiLowLevelError { get; set; }

		string RemoteStackTrace { get; set; }
	}
}
