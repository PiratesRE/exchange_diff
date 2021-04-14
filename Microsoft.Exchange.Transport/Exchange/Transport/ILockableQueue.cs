using System;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport
{
	internal interface ILockableQueue
	{
		NextHopSolutionKey Key { get; }

		int LockedCount { get; }

		void Lock(ILockableItem item, WaitCondition condition, string lockReason, int dehydrateThreshold);

		bool ActivateOne(WaitCondition condition, DeliveryPriority suggestedPriority, AccessToken token);

		ILockableItem DequeueInternal();

		ILockableItem DequeueInternal(DeliveryPriority priority);
	}
}
