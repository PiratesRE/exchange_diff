using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SubscriptionsManager : DisposableObject
	{
		internal SubscriptionsManager()
		{
		}

		internal void RegisterSubscription(Subscription subscription)
		{
			lock (this.lockSubscriptionQueue)
			{
				this.subscriptionQueue.Add(subscription);
			}
		}

		internal void UnRegisterSubscription(Subscription subscription)
		{
			lock (this.lockSubscriptionQueue)
			{
				this.subscriptionQueue.Remove(subscription);
			}
		}

		internal int SubscriptionCount
		{
			get
			{
				int count;
				lock (this.lockSubscriptionQueue)
				{
					count = this.subscriptionQueue.Count;
				}
				return count;
			}
		}

		internal void SendNotificationAlert()
		{
			ExTraceGlobals.StorageTracer.TraceDebug<bool>((long)this.GetHashCode(), "SubscriptionManager::SendNotificationAlert. isDeliveryThreadAlive = {0}.", this.isDeliveryThreadAlive);
			lock (this.lockSubscriptionQueue)
			{
				if (!this.isDeliveryThreadAlive)
				{
					if (ThreadPool.QueueUserWorkItem(delegate(object state)
					{
						this.DeliverNotifications(state);
					}))
					{
						this.isDeliveryThreadAlive = true;
					}
				}
			}
		}

		private void DeliverNotifications(object state)
		{
			if (base.IsDisposed)
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			while (!flag2)
			{
				try
				{
					bool flag3 = true;
					while (flag3)
					{
						flag3 = false;
						Subscription subscription = null;
						while (this.RoundRobinSubscriptions(ref subscription))
						{
							if (base.IsDisposed)
							{
								return;
							}
							MapiNotification mapiNotification = subscription.Sink.Dequeue();
							if (ExTraceGlobals.StorageTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								ExTraceGlobals.StorageTracer.TraceDebug<string>((long)this.GetHashCode(), "SubscriptionManager::DeliverNotifications. notification = {0}.", (mapiNotification == null) ? "<Null>" : mapiNotification.NotificationType.ToString());
							}
							if (mapiNotification != null)
							{
								flag3 = true;
								Notification notification;
								if (subscription.TryCreateXsoNotification(mapiNotification, out notification))
								{
									if (ExTraceGlobals.StorageTracer.IsTraceEnabled(TraceType.DebugTrace))
									{
										ExTraceGlobals.StorageTracer.TraceDebug<string>((long)this.GetHashCode(), "SubscriptionManager::DeliverNotifications. notification = {0}.", notification.Type.ToString());
									}
									subscription.InvokeHandler(notification);
								}
								else if (ExTraceGlobals.StorageTracer.IsTraceEnabled(TraceType.DebugTrace))
								{
									ExTraceGlobals.StorageTracer.TraceDebug((long)this.GetHashCode(), "SubscriptionManager::DeliverNotifications. notification = <null>.");
								}
							}
						}
					}
					flag = true;
				}
				finally
				{
					bool flag4 = false;
					lock (this.lockSubscriptionQueue)
					{
						flag4 = this.HasAnyNotifications();
						if (!flag || !flag4)
						{
							this.isDeliveryThreadAlive = false;
							flag2 = true;
						}
					}
					if (ExTraceGlobals.StorageTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.StorageTracer.TraceDebug<bool, bool, bool>((long)this.GetHashCode(), "SubscriptionManager::DeliverNotifications. Delivery thread is leaving. isOperationSuccess = {0}, hasAnyNotifications = {1}, workDone = {2}.", flag, flag4, flag2);
					}
				}
			}
		}

		private bool RoundRobinSubscriptions(ref Subscription subscription)
		{
			bool result;
			lock (this.lockSubscriptionQueue)
			{
				if (this.subscriptionQueue.Count == 0)
				{
					result = false;
				}
				else
				{
					int num;
					if (subscription != null)
					{
						num = this.subscriptionQueue.IndexOf(subscription);
					}
					else
					{
						num = -1;
					}
					num++;
					if (num >= this.subscriptionQueue.Count)
					{
						result = false;
					}
					else
					{
						subscription = this.subscriptionQueue[num];
						result = true;
					}
				}
			}
			return result;
		}

		private bool HasAnyNotifications()
		{
			foreach (Subscription subscription in this.subscriptionQueue)
			{
				if (subscription.Sink.Count > 0)
				{
					return true;
				}
			}
			return false;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SubscriptionsManager>(this);
		}

		private readonly List<Subscription> subscriptionQueue = new List<Subscription>();

		private readonly object lockSubscriptionQueue = new object();

		private bool isDeliveryThreadAlive;
	}
}
