using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum ConditionType
	{
		FromRecipientsCondition = 1,
		ContainsSubjectStringCondition,
		SentOnlyToMeCondition = 4,
		SentToMeCondition,
		MarkedAsImportanceCondition,
		MarkedAsSensitivityCondition,
		SentCcMeCondition = 9,
		SentToOrCcMeCondition,
		NotSentToMeCondition,
		SentToRecipientsCondition,
		ContainsBodyStringCondition,
		ContainsSubjectOrBodyStringCondition,
		ContainsHeaderStringCondition,
		ContainsSenderStringCondition = 17,
		MarkedAsOofCondition = 19,
		HasAttachmentCondition,
		WithinSizeRangeCondition,
		WithinDateRangeCondition,
		MeetingMessageCondition = 26,
		ContainsRecipientStringCondition,
		AssignedCategoriesCondition,
		FormsCondition,
		MessageClassificationCondition,
		NdrCondition,
		AutomaticForwardCondition,
		EncryptedCondition,
		SignedCondition,
		ReadReceiptCondition,
		MeetingResponseCondition,
		PermissionControlledCondition,
		ApprovalRequestCondition,
		VoicemailCondition,
		FlaggedForActionCondition,
		FromSubscriptionCondition
	}
}
