using System;

namespace Microsoft.Exchange.MessagingPolicies.UnJournalAgent
{
	internal enum ProcessingStatus
	{
		NotDone,
		PermanentError,
		TransientError,
		UnwrapProcessSuccess,
		NdrProcessSuccess,
		LegacyArchiveJournallingDisabled,
		NonJournalMsgFromLegacyArchiveCustomer,
		AlreadyProcessed,
		DropJournalReportWithoutNdr,
		NoUsersResolved,
		NdrJournalReport
	}
}
