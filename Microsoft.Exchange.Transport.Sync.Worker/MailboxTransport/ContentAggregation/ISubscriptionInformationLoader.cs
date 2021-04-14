using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface ISubscriptionInformationLoader
	{
		bool TryLoadStateStorage(AggregationWorkItem workItem, SyncMailboxSession syncMailboxSession, ISyncWorkerData subscription, out IStateStorage stateStorage, out ISyncException exception);

		bool TryReloadStateStorage(AggregationWorkItem workItem, IStateStorage stateStorage, out ISyncException exception);

		bool TryLoadSubscription(AggregationWorkItem workItem, SyncMailboxSession syncMailboxSession, out ISyncWorkerData subscription, out ISyncException exception, out bool invalidState);

		bool TryLoadMailboxSession(AggregationWorkItem workItem, SyncMailboxSession syncMailboxSession, out OrganizationId organizationId, out bool invalidState, out ISyncException exception);

		bool TrySaveSubscription(SyncMailboxSession syncMailboxSession, ISyncWorkerData subscription, EventHandler<RoundtripCompleteEventArgs> roundtripComplete, out Exception exception);

		bool TryDeleteSubscription(SyncMailboxSession syncMailboxSession, ISyncWorkerData subscription, EventHandler<RoundtripCompleteEventArgs> roundtripComplete);

		bool IsMailboxOverQuota(SyncMailboxSession syncMailboxSession, SyncLogSession syncLogSession, ulong requiredFreeBytes);

		bool TrySendSubscriptionNotificationEmail(SyncMailboxSession syncMailboxSession, ISyncWorkerData subscription, SyncLogSession syncLogSession, out bool retry);
	}
}
