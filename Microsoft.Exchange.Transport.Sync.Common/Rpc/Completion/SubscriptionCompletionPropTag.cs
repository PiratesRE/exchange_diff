using System;

namespace Microsoft.Exchange.Transport.Sync.Common.Rpc.Completion
{
	internal enum SubscriptionCompletionPropTag : uint
	{
		InArgDatabaseGuid = 2684354632U,
		InArgSubscriptionMessageID = 2684420354U,
		InArgMoreSyncRequested = 2684551171U,
		InArgErrorClass = 2684616707U,
		InArgUserMailboxGuid = 2684813384U,
		InArgSyncPhase = 2684878851U,
		InArgSubscription = 2684944642U,
		InArgSyncWatermark = 2685009951U,
		InArgSubscriptionGuid = 2685075528U,
		InArgAggregationType = 2685140995U,
		OutArgErrorCode = 2835349507U
	}
}
