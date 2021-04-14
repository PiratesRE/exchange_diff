using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAggregationByParticipantProcessor
	{
		bool ShouldAggregate(ICorePropertyBag message, IStorePropertyBag parentMessage, ConversationIndex.FixupStage previousStage);

		bool Aggregate(IConversationAggregationLogger logger, ICorePropertyBag message, IStorePropertyBag parentMessage);
	}
}
