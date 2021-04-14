using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Flags]
	[Serializable]
	public enum MigrationBatchFlags
	{
		[LocDescription(ServerStrings.IDs.MigrationBatchFlagNone)]
		None = 0,
		[LocDescription(ServerStrings.IDs.MigrationBatchFlagDisallowExistingUsers)]
		DisallowExistingUsers = 1,
		[LocDescription(ServerStrings.IDs.MigrationBatchFlagForceNewMigration)]
		ForceNewMigration = 2,
		[LocDescription(ServerStrings.IDs.MigrationBatchFlagUseAdvancedValidation)]
		UseAdvancedValidation = 4,
		[LocDescription(ServerStrings.IDs.MigrationBatchAutoComplete)]
		AutoComplete = 8,
		[LocDescription(ServerStrings.IDs.MigrationBatchFlagAutoStop)]
		AutoStop = 16,
		[LocDescription(ServerStrings.IDs.MigrationBatchFlagDisableOnCopy)]
		DisableOnCopy = 32,
		[LocDescription(ServerStrings.IDs.MigrationBatchFlagReportInitial)]
		ReportInitial = 64
	}
}
