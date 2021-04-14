using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AggregationByParticipantProcessor : IAggregationByParticipantProcessor
	{
		public AggregationByParticipantProcessor(IReplyAllExtractor replyAllExtractor)
		{
			this.replyAllExtractor = replyAllExtractor;
		}

		protected IReplyAllExtractor ReplyAllExtractor
		{
			get
			{
				return this.replyAllExtractor;
			}
		}

		public static AggregationByParticipantProcessor CreateInstance(IMailboxSession session, IXSOFactory xsoFactory)
		{
			return new AggregationByParticipantProcessor(new ReplyAllExtractor(session, xsoFactory));
		}

		public bool ShouldAggregate(ICorePropertyBag message, IStorePropertyBag parentMessage, ConversationIndex.FixupStage previousStage)
		{
			if (AudienceBasedConversationAggregator.IsMessageCreatingNewConversation(parentMessage, previousStage))
			{
				return false;
			}
			if (!AudienceBasedConversationAggregator.SupportsSideConversation(parentMessage))
			{
				return false;
			}
			if (!this.IsSupportedItem(message) || !this.IsSupportedItem(parentMessage))
			{
				return false;
			}
			if (previousStage <= ConversationIndex.FixupStage.H13)
			{
				if (previousStage != ConversationIndex.FixupStage.Unknown && previousStage != ConversationIndex.FixupStage.H13)
				{
					return true;
				}
			}
			else if (previousStage != ConversationIndex.FixupStage.Error && previousStage != ConversationIndex.FixupStage.S1 && previousStage != ConversationIndex.FixupStage.S2)
			{
				return true;
			}
			return false;
		}

		public bool Aggregate(IConversationAggregationLogger logger, ICorePropertyBag message, IStorePropertyBag parentMessage)
		{
			return this.CheckParticipantsGotRemoved(logger, message, parentMessage);
		}

		protected virtual bool IsSupportedItem(IStorePropertyBag item)
		{
			return ComplexParticipantExtractorBase<IStorePropertyBag>.CanExtractRecipients(item);
		}

		protected virtual bool IsSupportedItem(ICorePropertyBag item)
		{
			return ComplexParticipantExtractorBase<ICorePropertyBag>.CanExtractRecipients(item);
		}

		private bool CheckParticipantsGotRemoved(IConversationAggregationLogger logger, ICorePropertyBag deliveredItemPropertyBag, IStorePropertyBag parentItemPropertyBag)
		{
			bool result;
			AggregationByParticipantProcessor.DisplayNameCheckResult displayNameCheckResult = this.TryCheckParticipantsRemovedUsingDisplayNames(logger, deliveredItemPropertyBag, parentItemPropertyBag, out result);
			if (displayNameCheckResult == AggregationByParticipantProcessor.DisplayNameCheckResult.Success)
			{
				logger.LogSideConversationProcessingData(displayNameCheckResult.ToString(), false);
				return result;
			}
			logger.LogSideConversationProcessingData(displayNameCheckResult.ToString(), true);
			return this.CheckParticipantsRemovedUsingParticipants(logger, deliveredItemPropertyBag, parentItemPropertyBag);
		}

		private AggregationByParticipantProcessor.DisplayNameCheckResult TryCheckParticipantsRemovedUsingDisplayNames(IConversationAggregationLogger logger, ICorePropertyBag deliveredItemPropertyBag, IStorePropertyBag parentItemPropertyBag, out bool participantsRemoved)
		{
			HashSet<string> hashSet = null;
			HashSet<string> hashSet2 = null;
			AggregationByParticipantProcessor.DisplayNameCheckResult displayNameCheckResult = AggregationByParticipantProcessor.DisplayNameCheckResult.Success;
			participantsRemoved = false;
			if (this.ReplyAllExtractor.TryRetrieveReplyAllDisplayNames(parentItemPropertyBag, out hashSet2))
			{
				hashSet = this.ReplyAllExtractor.RetrieveReplyAllDisplayNames(deliveredItemPropertyBag);
				if (hashSet.Count < hashSet2.Count)
				{
					participantsRemoved = true;
					displayNameCheckResult = AggregationByParticipantProcessor.DisplayNameCheckResult.Success;
				}
				else
				{
					hashSet2.ExceptWith(hashSet);
					if (hashSet2.Any<string>())
					{
						displayNameCheckResult = AggregationByParticipantProcessor.DisplayNameCheckResult.DisplayNamesNotMatching;
					}
				}
			}
			else
			{
				displayNameCheckResult = AggregationByParticipantProcessor.DisplayNameCheckResult.DisplayNamesTooLong;
			}
			if (displayNameCheckResult == AggregationByParticipantProcessor.DisplayNameCheckResult.Success && participantsRemoved)
			{
				logger.LogSideConversationProcessingData(hashSet2, hashSet);
			}
			return displayNameCheckResult;
		}

		private bool CheckParticipantsRemovedUsingParticipants(IConversationAggregationLogger logger, ICorePropertyBag deliveredItemPropertyBag, IStorePropertyBag parentItemPropertyBag)
		{
			ParticipantSet participantSet = this.ReplyAllExtractor.RetrieveReplyAllParticipants(deliveredItemPropertyBag);
			ParticipantSet participantSet2 = this.ReplyAllExtractor.RetrieveReplyAllParticipants(parentItemPropertyBag);
			bool flag = !participantSet2.IsSubsetOf(participantSet);
			if (flag)
			{
				logger.LogSideConversationProcessingData(participantSet2, participantSet);
			}
			return flag;
		}

		private readonly IReplyAllExtractor replyAllExtractor;

		private enum DisplayNameCheckResult
		{
			Success,
			DisplayNamesTooLong,
			DisplayNamesNotMatching
		}
	}
}
