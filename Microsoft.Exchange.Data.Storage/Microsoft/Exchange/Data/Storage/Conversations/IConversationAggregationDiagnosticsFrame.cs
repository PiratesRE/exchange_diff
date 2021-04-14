using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IConversationAggregationDiagnosticsFrame
	{
		ConversationAggregationResult TrackAggregation(string operationName, AggregationDelegate aggregation);
	}
}
