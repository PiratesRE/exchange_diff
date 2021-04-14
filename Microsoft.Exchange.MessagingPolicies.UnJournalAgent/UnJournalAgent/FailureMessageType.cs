using System;

namespace Microsoft.Exchange.MessagingPolicies.UnJournalAgent
{
	internal enum FailureMessageType
	{
		Unknown,
		DefectiveJournalNoRecipientsMsg,
		DefectiveJournalWithRecipientsMsg,
		UnProvisionedRecipientsMsg,
		NoRecipientsResolvedMsg,
		UnexpectedJournalMessageFormatMsg,
		PermanentErrorOther
	}
}
