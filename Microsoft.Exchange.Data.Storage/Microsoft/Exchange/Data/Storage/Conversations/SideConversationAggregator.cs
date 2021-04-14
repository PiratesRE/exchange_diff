using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SideConversationAggregator : AudienceBasedConversationAggregator
	{
		public SideConversationAggregator(IMailboxOwner mailboxOwner, bool scenarioSupportsSearchForDuplicatedMessages, IAggregationByItemClassReferencesSubjectProcessor referencesProcessor, IAggregationByParticipantProcessor participantProcessor, IConversationAggregationDiagnosticsFrame diagnosticsFrame) : base(mailboxOwner, scenarioSupportsSearchForDuplicatedMessages, referencesProcessor, participantProcessor, diagnosticsFrame)
		{
		}

		protected override ConversationAggregationResult ConstructResult(ConversationIndex.FixupStage bySubjectResultingStage, ConversationIndex bySubjectResultingIndex, IStorePropertyBag parentItem, bool participantRemoved)
		{
			ConversationAggregationResult conversationAggregationResult = this.ConstructResult(bySubjectResultingStage, bySubjectResultingIndex, parentItem);
			if (participantRemoved)
			{
				conversationAggregationResult.Stage |= ConversationIndex.FixupStage.SC;
				conversationAggregationResult.ConversationIndex = bySubjectResultingIndex.UpdateGuid(Guid.NewGuid());
			}
			return conversationAggregationResult;
		}
	}
}
