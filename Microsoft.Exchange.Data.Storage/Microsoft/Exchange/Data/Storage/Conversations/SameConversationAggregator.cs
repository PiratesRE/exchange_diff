using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SameConversationAggregator : ConversationAggregatorBase
	{
		public SameConversationAggregator(IAggregationByItemClassReferencesSubjectProcessor referenceProcessor) : base(referenceProcessor)
		{
		}

		public override ConversationAggregationResult Aggregate(ICoreItem message)
		{
			IStorePropertyBag parentItem;
			ConversationIndex bySubjectResultingIndex;
			ConversationIndex.FixupStage bySubjectResultingStage;
			base.ReferencesProcessor.Aggregate(message.PropertyBag, false, out parentItem, out bySubjectResultingIndex, out bySubjectResultingStage);
			return this.ConstructResult(bySubjectResultingStage, bySubjectResultingIndex, parentItem);
		}
	}
}
