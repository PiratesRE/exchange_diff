using System;

namespace Microsoft.Exchange.Inference.Common
{
	internal enum FeatureNumericId : short
	{
		UnAssigned,
		FeatureBias,
		IsFlaggedBySender,
		IsMarkedHighImportanceBySender,
		IsMarkedUnimportantBySender,
		IsResponseRequested,
		SenderIsAutomated,
		SenderIsYou,
		ConversationStarterIsYou,
		OnlyRecipient,
		ConversationContributions,
		SenderIsManager,
		SenderIsDirectReport,
		RecipientPosition,
		ManagerPosition,
		RecipientCountOnToLine,
		RecipientCountOnCcLine,
		ReplyToAMessageFromMe,
		PreviousUnread,
		PreviousFlagged,
		NumberOfFileAttachments,
		NumberOfMessageAttachments,
		NumberOfInlineAttachments,
		NewConversationStarter,
		NewSender,
		SwitchedToToLineFirstTime,
		SubjectContent,
		ItemClass,
		SubjectPrefix,
		Sender,
		SenderCc,
		SenderDList,
		ConversationStarter,
		ConversationStarterCc,
		ConversationStarterDList,
		RecipientOnToLine,
		RecipientOnCcLine
	}
}
