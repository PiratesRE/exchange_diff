using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	public enum MailboxCopierFlags
	{
		None = 0,
		CrossOrg = 1,
		Merge = 2,
		PublicFolderMigration = 4,
		SourceIsArchive = 8,
		TargetIsArchive = 16,
		SourceIsPST = 32,
		TargetIsPST = 64,
		Root = 128,
		Imap = 256,
		Pop = 512,
		Eas = 1024,
		ContainerAggregated = 2048,
		ContainerOrg = 4096,
		Olc = 8192
	}
}
