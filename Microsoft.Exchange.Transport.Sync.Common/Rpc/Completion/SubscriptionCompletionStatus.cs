using System;

namespace Microsoft.Exchange.Transport.Sync.Common.Rpc.Completion
{
	internal enum SubscriptionCompletionStatus
	{
		NoError,
		SyncError,
		DisableSubscription,
		HubShutdown = 4,
		InvalidState,
		DeleteSubscription
	}
}
