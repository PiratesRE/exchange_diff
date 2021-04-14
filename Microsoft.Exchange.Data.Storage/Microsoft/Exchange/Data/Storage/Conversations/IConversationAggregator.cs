using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IConversationAggregator
	{
		ConversationAggregationResult Aggregate(ICoreItem message);
	}
}
