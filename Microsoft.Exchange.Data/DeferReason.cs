using System;

namespace Microsoft.Exchange.Data
{
	internal enum DeferReason
	{
		None,
		ADTransientFailureDuringResolve,
		ADTransientFailureDuringContentConversion,
		Agent,
		LoopDetected,
		ReroutedByStoreDriver,
		StorageTransientFailureDuringContentConversion,
		MarkedAsRetryDeliveryIfRejected,
		TransientFailure,
		AmbiguousRecipient,
		ConcurrencyLimitInStoreDriver,
		TargetSiteInboundMailDisabled,
		RecipientThreadLimitExceeded,
		TransientAttributionFailure,
		TransientAcceptedDomainsLoadFailure,
		RecipientHasNoMdb,
		ConfigUpdate
	}
}
