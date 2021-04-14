using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum MRSRequestType
	{
		Move,
		Merge,
		MailboxImport,
		MailboxExport,
		MailboxRestore,
		PublicFolderMove = 6,
		PublicFolderMigration,
		Sync,
		MailboxRelocation,
		FolderMove,
		PublicFolderMailboxMigration
	}
}
