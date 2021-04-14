using System;

namespace Microsoft.Exchange.Inference.Common
{
	internal enum InferenceFeatureReasons : ushort
	{
		None,
		ConversationStarterIsYou,
		OnlyRecipient,
		ConversationContributions,
		MarkedImportantBySender,
		SenderIsManager,
		SenderIsInManagementChain,
		SenderIsDirectReport,
		ActionBasedOnSender,
		NameOnToLine,
		NameOnCcLine,
		ManagerPosition,
		ReplyToAMessageFromMe,
		PreviouslyFlagged,
		ActionBasedOnRecipients,
		ActionBasedOnSubjectWords,
		ActionBasedOnBasedOnBodyWords
	}
}
