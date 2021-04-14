using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Worker.Agents
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class SubscriptionAgent : DisposeTrackableBase
	{
		public SubscriptionAgent(string name)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("name", name);
			this.name = name;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public virtual void OnWorkItemCompleted(SubscriptionEventSource source, SubscriptionWorkItemCompletedEventArgs e)
		{
		}

		public virtual void OnWorkItemFailedLoadSubscription(SubscriptionEventSource source, SubscriptionWorkItemFailedLoadSubscriptionEventArgs e)
		{
		}

		public abstract bool IsEventInteresting(AggregationType aggregationType, SubscriptionEvents events);

		public void Invoke(SubscriptionEvents subscriptionEvent, object source, object e)
		{
			SubscriptionEventSource source2 = (SubscriptionEventSource)source;
			switch (subscriptionEvent)
			{
			case SubscriptionEvents.WorkItemCompleted:
			{
				SubscriptionWorkItemCompletedEventArgs e2 = (SubscriptionWorkItemCompletedEventArgs)e;
				this.OnWorkItemCompleted(source2, e2);
				return;
			}
			case SubscriptionEvents.WorkItemFailedLoadSubscription:
			{
				SubscriptionWorkItemFailedLoadSubscriptionEventArgs e3 = (SubscriptionWorkItemFailedLoadSubscriptionEventArgs)e;
				this.OnWorkItemFailedLoadSubscription(source2, e3);
				return;
			}
			default:
				throw new NotSupportedException("Unsupported Subscription Event: " + subscriptionEvent);
			}
		}

		private string name;
	}
}
