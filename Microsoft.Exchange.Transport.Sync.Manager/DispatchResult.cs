using System;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	internal enum DispatchResult
	{
		Success,
		TransientFailure = 4096,
		PermanentFailure = 8192,
		InvalidSubscription = 16384,
		SubscriptionLoseItsTurnAtTransientFailure = 36864,
		DatabaseLosesItsTurnAtTransientFailure = 4352,
		WorkerSlotsFull = 4097,
		UnableToContactWorker,
		NoHubsToDispatchTo,
		SubscriptionAlreadyDispatched = 36868,
		SubscriptionCacheMessageDoesNotExist = 16389,
		TransientFailureReadingCache = 4358,
		SubscriptionDisabled = 16391,
		MaxSyncsPerDatabase = 4360,
		DatabaseHealthUnknown,
		DatabaseRpcLatencyUnhealthy,
		MailboxServerHAUnhealthy,
		MailboxServerCpuUnknown = 4108,
		MailboxServerCpuOverloaded,
		TransportQueueHealthUnknown,
		ServerTransportQueueUnhealthy,
		UserTransportQueueUnhealthy = 37136,
		PolicyInducedDeletion = 17,
		DatabaseOverloaded = 4370,
		ServerNotAvailable = 4115,
		EdgeTransportStopped,
		SubscriptionTypeDisabled,
		TransportSyncDisabled,
		MaxConcurrentMailboxSubmissions
	}
}
