using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	public enum HeuristicsFlags
	{
		None = 0,
		SuspendFolderReplication = 1024,
		PublicFoldersLockedForMigration = 2048,
		PublicFolderMigrationComplete = 4096,
		PublicFoldersDisabled = 8192,
		RemotePublicFolders = 16384,
		PublicFolderMailboxesLockedForNewConnections = 32768,
		PublicFolderMailboxesMigrationComplete = 65536
	}
}
