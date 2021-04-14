using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal enum MRSCapabilities
	{
		E14_RTM,
		Merges,
		ArchiveSeparation,
		TickleWithSide,
		TenantHint,
		MrsProxyVerification,
		AutoResume,
		MrsProxyPing,
		SubType,
		AutoResumeMerges,
		SyncNow,
		GetMailboxInformationWithRequestJob,
		CreatePublicFoldersUnderParentInSecondary,
		MaxElement
	}
}
