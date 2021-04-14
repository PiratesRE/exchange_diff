using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "PredictedActionReasonType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum PredictedActionReasonType
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
