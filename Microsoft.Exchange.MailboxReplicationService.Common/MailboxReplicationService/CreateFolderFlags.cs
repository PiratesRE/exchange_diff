using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum CreateFolderFlags
	{
		None = 0,
		FailIfExists = 1,
		CreatePublicFolderDumpster = 2,
		InternalAccess = 4
	}
}
