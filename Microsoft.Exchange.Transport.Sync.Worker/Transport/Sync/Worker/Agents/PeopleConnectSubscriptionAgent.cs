using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Worker.Agents
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PeopleConnectSubscriptionAgent : SubscriptionAgent
	{
		private IConnectSubscriptionCleanup SubscriptionCleanup { get; set; }

		public PeopleConnectSubscriptionAgent() : this(null)
		{
		}

		internal PeopleConnectSubscriptionAgent(IConnectSubscriptionCleanup cleanup) : base("People Connect Agent")
		{
			this.SubscriptionCleanup = cleanup;
		}

		public override bool IsEventInteresting(AggregationType aggregationType, SubscriptionEvents events)
		{
			return aggregationType == AggregationType.PeopleConnection && events == SubscriptionEvents.WorkItemCompleted;
		}

		public override void OnWorkItemCompleted(SubscriptionEventSource source, SubscriptionWorkItemCompletedEventArgs e)
		{
			e.SyncLogSession.LogDebugging((TSLID)1493UL, PeopleConnectSubscriptionAgent.Tracer, "{0}: OnWorkItemCompleted event received for subscription {1}.", new object[]
			{
				base.Name,
				e.SubscriptionMessageId
			});
			AggregationStatus status = e.Subscription.Status;
			if (status != AggregationStatus.Succeeded)
			{
				if (status != AggregationStatus.Disabled)
				{
					return;
				}
				if (!PeopleConnectSubscriptionAgent.VerifyAndLogMailboxSession(e.MailboxSession, e.SyncLogSession, e.SubscriptionMessageId))
				{
					return;
				}
				if (WellKnownNetworkNames.LinkedIn.Equals(e.Subscription.Name, StringComparison.OrdinalIgnoreCase) && e.Subscription.DetailedAggregationStatus == DetailedAggregationStatus.AuthenticationError)
				{
					if (this.SubscriptionCleanup == null)
					{
						this.SubscriptionCleanup = new ConnectSubscriptionCleanup(SubscriptionManager.Instance);
					}
					try
					{
						this.SubscriptionCleanup.Cleanup(e.MailboxSession, (IConnectSubscription)e.Subscription, true);
						return;
					}
					catch (LocalizedException ex)
					{
						e.SyncLogSession.LogError((TSLID)178UL, PeopleConnectSubscriptionAgent.Tracer, "Encountered exception during cleanup: {0}", new object[]
						{
							ex
						});
						return;
					}
				}
				new PeopleConnectNotifier(e.MailboxSession).NotifyNewTokenNeeded(e.Subscription.Name);
			}
			else
			{
				if (!PeopleConnectSubscriptionAgent.VerifyAndLogMailboxSession(e.MailboxSession, e.SyncLogSession, e.SubscriptionMessageId))
				{
					return;
				}
				if (e.Subscription.IsInitialSyncDone && !e.Subscription.WasInitialSyncDone)
				{
					new PeopleConnectNotifier(e.MailboxSession).NotifyInitialSyncCompleted(e.Subscription.Name);
					return;
				}
			}
		}

		private static bool VerifyAndLogMailboxSession(MailboxSession mailboxSession, SyncLogSession syncLogSession, StoreObjectId subscriptionMessageId)
		{
			if (mailboxSession == null)
			{
				syncLogSession.LogError((TSLID)1371UL, PeopleConnectSubscriptionAgent.Tracer, "Mailbox is null {0}", new object[]
				{
					subscriptionMessageId
				});
				return false;
			}
			if (!mailboxSession.IsConnected)
			{
				syncLogSession.LogError((TSLID)1494UL, PeopleConnectSubscriptionAgent.Tracer, "Mailbox is not connected {0}", new object[]
				{
					subscriptionMessageId
				});
				return false;
			}
			return true;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PeopleConnectSubscriptionAgent>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		private static readonly Trace Tracer = ExTraceGlobals.SubscriptionAgentManagerTracer;
	}
}
