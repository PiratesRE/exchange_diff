using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IConversationStatistics
	{
		ConversationId ConversationId { get; }

		int TotalNodeCount { get; }

		int LeafNodeCount { get; }

		int ItemsExtracted { get; }

		int ItemsOpened { get; }

		int SummariesConstructed { get; }

		int BodyTagMatchingAttemptsCount { get; }

		int BodyTagMatchingIssuesCount { get; }

		int BodyTagNotPresentCount { get; }

		int BodyTagMismatchedCount { get; }

		int BodyFormatMismatchedCount { get; }

		int NonMSHeaderCount { get; }

		int ExtraPropertiesNeededCount { get; }

		int ParticipantNotFoundCount { get; }

		int AttachmentPresentCount { get; }

		int MapiAttachmentPresentCount { get; }

		int PossibleInlinesCount { get; }

		int IrmProtectedCount { get; }
	}
}
