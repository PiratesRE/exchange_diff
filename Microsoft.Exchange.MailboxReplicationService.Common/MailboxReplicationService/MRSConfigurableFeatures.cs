using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum MRSConfigurableFeatures
	{
		None = 0,
		SkipCopyFolderPropertyCheck = 1
	}
}
