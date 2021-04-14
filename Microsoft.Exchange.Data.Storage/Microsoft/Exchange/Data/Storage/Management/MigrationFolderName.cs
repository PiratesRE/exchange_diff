using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public enum MigrationFolderName
	{
		[LocDescription(ServerStrings.IDs.MigrationFolderSyncMigration)]
		SyncMigration,
		[LocDescription(ServerStrings.IDs.MigrationFolderSyncMigrationReports)]
		SyncMigrationReports,
		[LocDescription(ServerStrings.IDs.MigrationFolderCorruptedItems)]
		CorruptedItems,
		[LocDescription(ServerStrings.IDs.MigrationFolderSettings)]
		Settings,
		[LocDescription(ServerStrings.IDs.MigrationFolderDrumTesting)]
		DrumTesting
	}
}
