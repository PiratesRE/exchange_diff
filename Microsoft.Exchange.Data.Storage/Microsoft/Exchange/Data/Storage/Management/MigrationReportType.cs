using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public enum MigrationReportType
	{
		[LocDescription(ServerStrings.IDs.MigrationReportUnknown)]
		Unknown,
		[LocDescription(ServerStrings.IDs.MigrationReportBatchSuccess)]
		BatchSuccessReport,
		[LocDescription(ServerStrings.IDs.MigrationReportBatchFailure)]
		BatchFailureReport,
		[LocDescription(ServerStrings.IDs.MigrationReportFinalizationSuccess)]
		FinalizationSuccessReport,
		[LocDescription(ServerStrings.IDs.MigrationReportFinalizationFailure)]
		FinalizationFailureReport,
		[LocDescription(ServerStrings.IDs.MigrationReportBatch)]
		BatchReport
	}
}
