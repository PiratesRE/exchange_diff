using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum ExceptionOrder
	{
		FromRecipientsCondition = 1,
		MarkedAsImportanceCondition,
		MarkedAsSensitivityCondition,
		MarkedAsOofCondition,
		HasAttachmentCondition,
		SentToMeCondition,
		SentOnlyToMeCondition,
		SentCcMeCondition,
		SentToOrCcMeCondition,
		NotSentToMeCondition,
		SentToRecipientsCondition,
		ContainsSubjectStringCondition,
		ContainsBodyStringCondition,
		ContainsSubjectOrBodyStringCondition,
		WithinSizeRangeCondition,
		WithinDateRangeCondition,
		ContainsSenderStringCondition,
		ContainsHeaderStringCondition,
		MeetingMessageCondition,
		MeetingResponseCondition,
		ContainsRecipientStringCondition,
		AssignedCategoriesCondition,
		FormsCondition,
		MessageClassificationCondition,
		NdrCondition,
		AutomaticForwardCondition,
		EncryptedCondition,
		SignedCondition,
		ReadReceiptCondition,
		PermissionControlledCondition,
		ApprovalRequestCondition,
		VoicemailCondition,
		FlaggedForActionCondition,
		FromSubscriptionCondition
	}
}
