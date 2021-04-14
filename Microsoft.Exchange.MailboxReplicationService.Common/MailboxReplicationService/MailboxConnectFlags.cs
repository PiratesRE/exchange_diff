using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum MailboxConnectFlags
	{
		None = 0,
		DoNotOpenMapiSession = 1,
		ValidateOnly = 2,
		NonMrsLogon = 4,
		PublicFolderHierarchyReplication = 8,
		HighPriority = 16,
		AllowRestoreFromConnectedMailbox = 32
	}
}
