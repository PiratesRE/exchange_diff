using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ConversationAggregatorBase : IConversationAggregator
	{
		public ConversationAggregatorBase(IAggregationByItemClassReferencesSubjectProcessor referencesProcessor)
		{
			this.referencesProcessor = referencesProcessor;
		}

		protected IAggregationByItemClassReferencesSubjectProcessor ReferencesProcessor
		{
			get
			{
				return this.referencesProcessor;
			}
		}

		public abstract ConversationAggregationResult Aggregate(ICoreItem message);

		protected virtual ConversationAggregationResult ConstructResult(ConversationIndex.FixupStage bySubjectResultingStage, ConversationIndex bySubjectResultingIndex, IStorePropertyBag parentItem)
		{
			return new ConversationAggregationResult
			{
				Stage = bySubjectResultingStage,
				ConversationIndex = bySubjectResultingIndex,
				SupportsSideConversation = false,
				ConversationFamilyId = ConversationId.Create(bySubjectResultingIndex)
			};
		}

		private readonly IAggregationByItemClassReferencesSubjectProcessor referencesProcessor;
	}
}
