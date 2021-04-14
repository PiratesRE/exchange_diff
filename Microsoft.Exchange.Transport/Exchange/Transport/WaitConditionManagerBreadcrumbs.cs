using System;

namespace Microsoft.Exchange.Transport
{
	internal enum WaitConditionManagerBreadcrumbs
	{
		EMPTY,
		IncrementInUse,
		DecrementInUse,
		WaitlistNewItem,
		ReactivateOlderItem,
		ReactivateOlderCondition,
		NewItemTokenUsed,
		ReturnTokenUnused,
		ConditionExceedsQuota,
		ActivateWaitingItemFound,
		ActivateBlockedCondition,
		UpdatePriority,
		RemoveEmptyInUse,
		RemoveEmptyWaitlist,
		ActivateAll,
		ItemNotFoundToActivate,
		RemoveBlockedCondition,
		AddBlockedCondition,
		ExceededMaxThreads,
		CleanupItem,
		CleanupQueue,
		CleanupCondition,
		BlockedExceedsQuota,
		OlderItemFound,
		AddDisabled
	}
}
