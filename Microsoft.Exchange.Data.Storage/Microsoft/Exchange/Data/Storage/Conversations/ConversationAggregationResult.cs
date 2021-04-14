using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationAggregationResult
	{
		public bool SupportsSideConversation { get; set; }

		public ConversationIndex.FixupStage Stage { get; set; }

		public ConversationId ConversationFamilyId { get; set; }

		public ConversationIndex ConversationIndex { get; set; }
	}
}
