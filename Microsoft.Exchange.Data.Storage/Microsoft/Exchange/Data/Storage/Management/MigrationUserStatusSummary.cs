using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public enum MigrationUserStatusSummary
	{
		[LocDescription(ServerStrings.IDs.MigrationUserStatusSummaryActive)]
		Active = 1,
		[LocDescription(ServerStrings.IDs.MigrationUserStatusSummaryFailed)]
		Failed,
		[LocDescription(ServerStrings.IDs.MigrationUserStatusSummarySynced)]
		Synced,
		[LocDescription(ServerStrings.IDs.MigrationUserStatusSummaryCompleted)]
		Completed,
		[LocDescription(ServerStrings.IDs.MigrationUserStatusSummaryStopped)]
		Stopped
	}
}
