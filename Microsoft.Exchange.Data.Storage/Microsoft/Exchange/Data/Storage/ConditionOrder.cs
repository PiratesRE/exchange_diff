using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum ConditionOrder
	{
		SentToMeCondition = 1,
		SentOnlyToMeCondition,
		SentCcMeCondition,
		SentToOrCcMeCondition,
		NotSentToMeCondition,
		FromRecipientsCondition,
		SentToRecipientsCondition,
		ContainsSubjectStringCondition,
		ContainsBodyStringCondition,
		ContainsSubjectOrBodyStringCondition,
		ContainsHeaderStringCondition,
		MarkedAsImportanceCondition,
		MarkedAsSensitivityCondition,
		MarkedAsOofCondition,
		HasAttachmentCondition,
		WithinSizeRangeCondition,
		WithinDateRangeCondition,
		ContainsSenderStringCondition,
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
