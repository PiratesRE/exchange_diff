using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AudienceBasedConversationAggregator : ConversationAggregatorBase
	{
		public AudienceBasedConversationAggregator(IMailboxOwner mailboxOwner, bool scenarioSupportsSearchForDuplicatedMessages, IAggregationByItemClassReferencesSubjectProcessor referencesProcessor, IAggregationByParticipantProcessor participantProcessor, IConversationAggregationDiagnosticsFrame diagnosticsFrame) : base(referencesProcessor)
		{
			this.mailboxOwner = mailboxOwner;
			this.participantProcessor = participantProcessor;
			this.diagnosticsFrame = diagnosticsFrame;
			this.scenarioSupportsSearchForDuplicatedMessages = scenarioSupportsSearchForDuplicatedMessages;
		}

		public static bool IsMessageCreatingNewConversation(IStorePropertyBag parentItemPropertyBag, ConversationIndex.FixupStage stage)
		{
			return parentItemPropertyBag == null || ConversationIndex.IsFixUpCreatingNewConversation(stage);
		}

		public static bool SupportsSideConversation(IStorePropertyBag message)
		{
			if (!message.GetValueOrDefault<bool>(ItemSchema.SupportsSideConversation, false))
			{
				string itemClass = message.TryGetProperty(InternalSchema.ItemClass) as string;
				return ObjectClass.IsCalendarItem(itemClass);
			}
			return true;
		}

		public override ConversationAggregationResult Aggregate(ICoreItem message)
		{
			return this.diagnosticsFrame.TrackAggregation(base.GetType().Name, delegate(IConversationAggregationLogger logger)
			{
				bool shouldSearchForDuplicatedMessage = this.ShouldSearchForDuplicatedMessage(message.PropertyBag);
				logger.LogMailboxOwnerData(this.mailboxOwner, shouldSearchForDuplicatedMessage);
				logger.LogDeliveredMessageData(message.PropertyBag);
				IStorePropertyBag storePropertyBag;
				ConversationIndex bySubjectResultingIndex;
				ConversationIndex.FixupStage fixupStage;
				this.ReferencesProcessor.Aggregate(message.PropertyBag, shouldSearchForDuplicatedMessage, out storePropertyBag, out bySubjectResultingIndex, out fixupStage);
				logger.LogParentMessageData(storePropertyBag);
				bool participantRemoved = false;
				if (this.participantProcessor.ShouldAggregate(message.PropertyBag, storePropertyBag, fixupStage))
				{
					participantRemoved = this.participantProcessor.Aggregate(logger, message.PropertyBag, storePropertyBag);
				}
				ConversationAggregationResult conversationAggregationResult = this.ConstructResult(fixupStage, bySubjectResultingIndex, storePropertyBag, participantRemoved);
				logger.LogAggregationResultData(conversationAggregationResult);
				return conversationAggregationResult;
			});
		}

		protected override ConversationAggregationResult ConstructResult(ConversationIndex.FixupStage bySubjectResultingStage, ConversationIndex bySubjectResultingIndex, IStorePropertyBag parentItem)
		{
			ConversationAggregationResult conversationAggregationResult = base.ConstructResult(bySubjectResultingStage, bySubjectResultingIndex, parentItem);
			if (AudienceBasedConversationAggregator.IsMessageCreatingNewConversation(parentItem, bySubjectResultingStage))
			{
				conversationAggregationResult.SupportsSideConversation = true;
				conversationAggregationResult.ConversationFamilyId = ConversationId.Create(bySubjectResultingIndex);
			}
			else
			{
				conversationAggregationResult.SupportsSideConversation = AudienceBasedConversationAggregator.SupportsSideConversation(parentItem);
				conversationAggregationResult.ConversationFamilyId = parentItem.GetValueOrDefault<ConversationId>(ItemSchema.ConversationFamilyId, null);
			}
			return conversationAggregationResult;
		}

		protected abstract ConversationAggregationResult ConstructResult(ConversationIndex.FixupStage bySubjectResultingStage, ConversationIndex bySubjectResultingIndex, IStorePropertyBag parentItem, bool participantRemoved);

		private bool ShouldSearchForDuplicatedMessage(ICorePropertyBag messageToAggregate)
		{
			return this.scenarioSupportsSearchForDuplicatedMessages && this.mailboxOwner.SearchDuplicatedMessagesEnabled && this.mailboxOwner.SentToMySelf(messageToAggregate);
		}

		private readonly bool scenarioSupportsSearchForDuplicatedMessages;

		private readonly IConversationAggregationDiagnosticsFrame diagnosticsFrame;

		private readonly IAggregationByParticipantProcessor participantProcessor;

		private readonly IMailboxOwner mailboxOwner;
	}
}
