using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Notifications.Broker;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class BrokerGateway : DisposeTrackableBase, IBrokerGateway
	{
		protected BrokerGateway()
		{
			this.subscriptionHandlers = new ConcurrentDictionary<Guid, BrokerHandler>();
		}

		public static BrokerGateway Instance
		{
			get
			{
				if (BrokerGateway.instance == null)
				{
					lock (BrokerGateway.syncRoot)
					{
						if (BrokerGateway.instance == null)
						{
							using (DisposeGuard disposeGuard = default(DisposeGuard))
							{
								BrokerGateway brokerGateway = new BrokerGateway();
								disposeGuard.Add<BrokerGateway>(brokerGateway);
								brokerGateway.Initialize();
								disposeGuard.Success();
								BrokerGateway.instance = brokerGateway;
							}
						}
					}
				}
				return BrokerGateway.instance;
			}
		}

		public static void Shutdown()
		{
			BrokerGateway comparand = BrokerGateway.instance;
			BrokerGateway brokerGateway = Interlocked.CompareExchange<BrokerGateway>(ref BrokerGateway.instance, null, comparand);
			if (brokerGateway != null)
			{
				brokerGateway.Dispose();
			}
		}

		public void Subscribe(BrokerSubscription brokerSubscription, BrokerHandler handler)
		{
			ArgumentValidator.ThrowIfNull("brokerSubscription", brokerSubscription);
			ArgumentValidator.ThrowIfNull("handler", handler);
			this.broker.Subscribe(brokerSubscription);
			this.subscriptionHandlers[brokerSubscription.SubscriptionId] = handler;
		}

		public void Unsubscribe(BrokerSubscription brokerSubscription)
		{
			ArgumentValidator.ThrowIfNull("brokerSubscription", brokerSubscription);
			BrokerHandler brokerHandler;
			if (this.subscriptionHandlers.TryRemove(brokerSubscription.SubscriptionId, out brokerHandler))
			{
				this.broker.Unsubscribe(brokerSubscription);
			}
		}

		protected override void InternalDispose(bool isDisposing)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<bool>((long)this.GetHashCode(), "BrokerNotificationManager.Dispose. IsDisposing: {0}", isDisposing);
			if (isDisposing && this.broker != null)
			{
				this.broker.Dispose();
				this.broker = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<BrokerGateway>(this);
		}

		protected virtual INotificationBrokerClient GetBroker()
		{
			return new NotificationBrokerClient();
		}

		protected void Initialize()
		{
			this.broker = this.GetBroker();
			this.broker.StartNotificationCallbacks(new Action<BrokerNotification>(this.ProcessNotifications));
		}

		private void ProcessNotifications(BrokerNotification notification)
		{
			BrokerHandler brokerHandler = null;
			if (this.subscriptionHandlers.TryGetValue(notification.SubscriptionId, out brokerHandler))
			{
				brokerHandler.HandleNotification(notification);
			}
		}

		private static BrokerGateway instance;

		private static readonly object syncRoot = new object();

		private INotificationBrokerClient broker;

		private ConcurrentDictionary<Guid, BrokerHandler> subscriptionHandlers;
	}
}
