using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public enum MRSJobType
	{
		Unknown = -1,
		RequestJobE14R3,
		RequestJobE14R4_WithGuid,
		RequestJobE14R4_WithArchive,
		RequestJobE14R4_WithPush,
		RequestJobE14R4_WithSuspend,
		RequestJobE14R4_WithDurations,
		RequestJobE14R5_WithImportExportMerge,
		RequestJobE14R5_PrimaryOrArchiveExclusiveMoves,
		RequestJobE14R6_CompressedReports,
		RequestJobE15_TenantHint,
		RequestJobE15_AutoResume,
		RequestJobE15_SubType,
		RequestJobE15_AutoResumeMerges,
		RequestJobE15_CreatePublicFoldersUnderParentInSecondary
	}
}
