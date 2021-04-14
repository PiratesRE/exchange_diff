using System;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.Common
{
	internal interface IConversationProperties
	{
		IMessageRecipient ConversationStarter { get; }

		int MailboxOwnerContributions { get; }

		int ContributionsByOthers { get; }

		bool ConversationHasMeeting { get; }

		bool ConversationStartedByForward { get; }

		int NumberOfPreviousMessages { get; }

		int NumberOfPreviousUnread { get; }

		int NumberOfPreviousFlaggedByOwner { get; }

		ReplyToAMessageFromMeEnum IsReplyToAMessageFromMe { get; }

		bool IsSwitchedToToLineFirstTime { get; }

		bool IsResponsePropagated { get; }
	}
}
