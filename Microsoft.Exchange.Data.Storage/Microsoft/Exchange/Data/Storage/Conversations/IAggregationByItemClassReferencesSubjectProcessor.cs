using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAggregationByItemClassReferencesSubjectProcessor
	{
		void Aggregate(ICorePropertyBag item, bool shouldSearchForDuplicatedMessage, out IStorePropertyBag parentItem, out ConversationIndex newIndex, out ConversationIndex.FixupStage stage);
	}
}
