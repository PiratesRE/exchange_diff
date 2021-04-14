using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SubscriptionSink
	{
		internal SubscriptionSink(SubscriptionsManager manager, bool needsNotifyManager)
		{
			this.manager = manager;
			this.subscription = null;
			this.needsNotifyManager = needsNotifyManager;
			this.writer = new SubscriptionSink.NotificationNode(null);
			this.reader = this.writer;
		}

		internal SubscriptionSink(Subscription subscription)
		{
			this.subscription = subscription;
		}

		internal int Count
		{
			get
			{
				return this.pendingNotificationCount;
			}
		}

		internal bool HasDroppedNotification
		{
			get
			{
				return this.hasDroppedNotification;
			}
		}

		private bool Enqueue(MapiNotification mapiNotification)
		{
			if (this.pendingNotificationCount > 256)
			{
				return false;
			}
			SubscriptionSink.NotificationNode next = new SubscriptionSink.NotificationNode(mapiNotification);
			this.writer.Next = next;
			this.writer = this.writer.Next;
			Interlocked.Increment(ref this.pendingNotificationCount);
			return true;
		}

		internal MapiNotification Dequeue()
		{
			if (this.pendingNotificationCount <= 0)
			{
				return null;
			}
			Interlocked.Decrement(ref this.pendingNotificationCount);
			this.reader = this.reader.Next;
			return this.reader.Data;
		}

		internal void OnNotify(MapiNotification notification)
		{
			if (ExTraceGlobals.StorageTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.StorageTracer.TraceDebug((long)this.GetHashCode(), "SubscriptionSink::OnNotify. Capacity = {0}, Count = {1}, Notification = {2}, needsNotifyManager = {3}.", new object[]
				{
					256,
					this.pendingNotificationCount,
					(notification == null) ? "<Null>" : notification.NotificationType.ToString(),
					this.needsNotifyManager
				});
			}
			Notification notification2;
			if (this.subscription == null)
			{
				if (!this.Enqueue(notification))
				{
					ExTraceGlobals.StorageTracer.TraceError<int, int>((long)this.GetHashCode(), "SubscriptionSink::OnNotify. The sink is full. Capacity = {0}, Count = {1}", 256, this.pendingNotificationCount);
					this.hasDroppedNotification = true;
				}
				if (this.needsNotifyManager)
				{
					this.manager.SendNotificationAlert();
					return;
				}
			}
			else if (this.subscription.TryCreateXsoNotification(notification, out notification2))
			{
				this.subscription.InvokeHandler(notification2);
			}
		}

		private const int Capacity = 256;

		private readonly SubscriptionsManager manager;

		private readonly Subscription subscription;

		private readonly bool needsNotifyManager;

		private int pendingNotificationCount;

		private bool hasDroppedNotification;

		private SubscriptionSink.NotificationNode writer;

		private SubscriptionSink.NotificationNode reader;

		private class NotificationNode
		{
			internal NotificationNode(MapiNotification data)
			{
				this.Data = data;
				this.Next = null;
			}

			internal readonly MapiNotification Data;

			internal SubscriptionSink.NotificationNode Next;
		}
	}
}
