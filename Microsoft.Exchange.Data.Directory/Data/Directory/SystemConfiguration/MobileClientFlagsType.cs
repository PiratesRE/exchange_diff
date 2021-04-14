using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	public enum MobileClientFlagsType
	{
		None = 0,
		ClientCertProvisionEnabled = 1,
		BadItemReportingEnabled = 2,
		RemoteDocumentsActionForUnknownServers = 4,
		SendWatsonReport = 8,
		MailboxLoggingEnabled = 16
	}
}
