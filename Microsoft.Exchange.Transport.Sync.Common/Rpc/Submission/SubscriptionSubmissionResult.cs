using System;

namespace Microsoft.Exchange.Transport.Sync.Common.Rpc.Submission
{
	internal enum SubscriptionSubmissionResult : uint
	{
		Success,
		SubscriptionAlreadyOnHub,
		UnknownRetryableError,
		SchedulerQueueFull,
		MaxConcurrentMailboxSubmissions,
		RpcServerTooBusy,
		RetryableRpcError,
		DatabaseRpcLatencyUnhealthy,
		DatabaseHealthUnknown,
		DatabaseOverloaded,
		ServerNotAvailable,
		EdgeTransportStopped,
		SubscriptionTypeDisabled,
		TransportSyncDisabled,
		MailboxServerCpuUnhealthy,
		MailboxServerCpuUnknown,
		MailboxServerHAUnhealthy,
		ServerTransportQueueUnhealthy,
		UserTransportQueueUnhealthy,
		TransportQueueHealthUnknown
	}
}
