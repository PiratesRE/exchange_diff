using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class BrokerNotificationManager : DisposeTrackableBase, INotificationManager, IDisposable
	{
		internal BrokerNotificationManager(IMailboxContext userContext)
		{
			this.userContext = userContext;
			this.mapiNotificationManager = new OwaMapiNotificationManager(userContext);
			this.activeHandlers = new Dictionary<string, BrokerHandlerReferenceCounter>();
			this.userContext.PendingRequestManager.KeepAlive += this.KeepHandlersAlive;
		}

		public event EventHandler<EventArgs> RemoteKeepAliveEvent
		{
			add
			{
				lock (this.syncRoot)
				{
					if (!this.isDisposed)
					{
						this.mapiNotificationManager.RemoteKeepAliveEvent += value;
					}
				}
			}
			remove
			{
				lock (this.syncRoot)
				{
					if (!this.isDisposed)
					{
						this.mapiNotificationManager.RemoteKeepAliveEvent -= value;
					}
				}
			}
		}

		public SearchNotificationHandler SearchNotificationHandler
		{
			get
			{
				return this.mapiNotificationManager.SearchNotificationHandler;
			}
		}

		public void SubscribeToHierarchyNotification(string subscriptionId)
		{
			lock (this.syncRoot)
			{
				if (!this.isDisposed)
				{
					this.mapiNotificationManager.SubscribeToHierarchyNotification(subscriptionId);
				}
			}
		}

		public void SubscribeToRowNotification(string subscriptionId, SubscriptionParameters parameters, ExTimeZone timeZone, CallContext callContext, bool remoteSubscription)
		{
			lock (this.syncRoot)
			{
				if (!this.isDisposed)
				{
					this.mapiNotificationManager.SubscribeToRowNotification(subscriptionId, parameters, timeZone, callContext, remoteSubscription);
				}
			}
		}

		public void SubscribeToReminderNotification(string subscriptionId)
		{
			lock (this.syncRoot)
			{
				if (!this.isDisposed)
				{
					this.mapiNotificationManager.SubscribeToReminderNotification(subscriptionId);
				}
			}
		}

		public void SubscribeToNewMailNotification(string subscriptionId, SubscriptionParameters parameters)
		{
			lock (this.syncRoot)
			{
				if (!this.isDisposed)
				{
					this.mapiNotificationManager.SubscribeToNewMailNotification(subscriptionId, parameters);
				}
			}
		}

		public string SubscribeToUnseenItemNotification(string subscriptionId, UserMailboxLocator mailboxLocator, IRecipientSession adSession)
		{
			string result;
			lock (this.syncRoot)
			{
				if (this.isDisposed)
				{
					result = null;
				}
				else
				{
					result = this.mapiNotificationManager.SubscribeToUnseenItemNotification(subscriptionId, mailboxLocator, adSession);
				}
			}
			return result;
		}

		public void SubscribeToUnseenCountNotification(string subscriptionId, SubscriptionParameters parameters, IRecipientSession adSession)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("subscriptionId", subscriptionId);
			ArgumentValidator.ThrowIfNull("parameters", parameters);
			ArgumentValidator.ThrowIfNull("MailboxId", parameters.MailboxId);
			ArgumentValidator.ThrowIfNull("adSession", adSession);
			this.Subscribe(subscriptionId, parameters.ChannelId, () => new UnseenCountBrokerHandler(subscriptionId, parameters, this.userContext, adSession));
		}

		public void UnsubscribeToUnseenCountNotification(string subscriptionId, SubscriptionParameters parameters)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("subscriptionId", subscriptionId);
			ArgumentValidator.ThrowIfNull("parameters", parameters);
			ArgumentValidator.ThrowIfNull("MailboxId", parameters.MailboxId);
			this.Unsubscribe(subscriptionId, parameters);
		}

		public void SubscribeToGroupAssociationNotification(string subscriptionId, IRecipientSession adSession)
		{
			lock (this.syncRoot)
			{
				if (!this.isDisposed)
				{
					this.mapiNotificationManager.SubscribeToGroupAssociationNotification(subscriptionId, adSession);
				}
			}
		}

		public void SubscribeToSearchNotification()
		{
			lock (this.syncRoot)
			{
				if (!this.isDisposed)
				{
					this.mapiNotificationManager.SubscribeToSearchNotification();
				}
			}
		}

		public void UnsubscribeForRowNotifications(string subscriptionId, SubscriptionParameters parameters)
		{
			lock (this.syncRoot)
			{
				if (!this.isDisposed)
				{
					this.mapiNotificationManager.UnsubscribeForRowNotifications(subscriptionId, parameters);
				}
			}
		}

		public void ReleaseSubscription(string subscriptionId)
		{
			lock (this.syncRoot)
			{
				if (!this.isDisposed)
				{
					this.mapiNotificationManager.ReleaseSubscription(subscriptionId);
				}
			}
		}

		public void ReleaseSubscriptionsForChannelId(string channelId)
		{
			List<KeyValuePair<string, BrokerHandlerReferenceCounter>> list = new List<KeyValuePair<string, BrokerHandlerReferenceCounter>>();
			lock (this.syncRoot)
			{
				if (this.isDisposed)
				{
					return;
				}
				this.mapiNotificationManager.ReleaseSubscriptionsForChannelId(channelId);
				foreach (KeyValuePair<string, BrokerHandlerReferenceCounter> item in this.activeHandlers)
				{
					item.Value.Remove(channelId);
					if (item.Value.Count == 0)
					{
						list.Add(item);
					}
				}
				foreach (KeyValuePair<string, BrokerHandlerReferenceCounter> keyValuePair in list)
				{
					this.activeHandlers.Remove(keyValuePair.Key);
				}
			}
			foreach (KeyValuePair<string, BrokerHandlerReferenceCounter> keyValuePair2 in list)
			{
				keyValuePair2.Value.Dispose();
			}
		}

		public void RefreshSubscriptions(ExTimeZone timeZone)
		{
			lock (this.syncRoot)
			{
				if (!this.isDisposed)
				{
					this.mapiNotificationManager.RefreshSubscriptions(timeZone);
				}
			}
		}

		public void CleanupSubscriptions()
		{
			lock (this.syncRoot)
			{
				if (!this.isDisposed)
				{
					this.mapiNotificationManager.CleanupSubscriptions();
				}
			}
		}

		public void HandleConnectionDroppedNotification()
		{
			lock (this.syncRoot)
			{
				if (!this.isDisposed)
				{
					this.mapiNotificationManager.HandleConnectionDroppedNotification();
				}
			}
		}

		public void StartRemoteKeepAliveTimer()
		{
			lock (this.syncRoot)
			{
				if (!this.isDisposed)
				{
					this.mapiNotificationManager.StartRemoteKeepAliveTimer();
				}
			}
		}

		protected override void InternalDispose(bool isDisposing)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<bool>((long)this.GetHashCode(), "BrokerNotificationManager.Dispose. IsDisposing: {0}", isDisposing);
			if (isDisposing)
			{
				try
				{
					OwaDiagnostics.SendWatsonReportsForGrayExceptions(delegate()
					{
						lock (this.syncRoot)
						{
							this.isDisposed = true;
							if (this.userContext != null && this.userContext.PendingRequestManager != null)
							{
								this.userContext.PendingRequestManager.KeepAlive -= this.KeepHandlersAlive;
							}
							foreach (BrokerHandlerReferenceCounter brokerHandlerReferenceCounter in this.activeHandlers.Values)
							{
								brokerHandlerReferenceCounter.Dispose();
							}
							this.activeHandlers.Clear();
							this.activeHandlers = null;
							if (this.mapiNotificationManager != null)
							{
								this.mapiNotificationManager.Dispose();
								this.mapiNotificationManager = null;
							}
						}
					});
				}
				catch (GrayException ex)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceError<string>(0L, "[BrokerNotificationManager.Dispose]. Unable to dispose object.  exception {0}", ex.Message);
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<BrokerNotificationManager>(this);
		}

		private void Subscribe(string subscriptionId, string channelId, Func<BrokerHandler> createHandlerDelegate)
		{
			lock (this.syncRoot)
			{
				if (!this.isDisposed)
				{
					BrokerHandlerReferenceCounter brokerHandlerReferenceCounter;
					if (!this.activeHandlers.TryGetValue(subscriptionId, out brokerHandlerReferenceCounter))
					{
						brokerHandlerReferenceCounter = new BrokerHandlerReferenceCounter(createHandlerDelegate);
						this.activeHandlers[subscriptionId] = brokerHandlerReferenceCounter;
					}
					brokerHandlerReferenceCounter.Add(channelId);
				}
			}
		}

		private void Unsubscribe(string subscriptionId, SubscriptionParameters parameters)
		{
			BrokerHandlerReferenceCounter brokerHandlerReferenceCounter = null;
			lock (this.syncRoot)
			{
				if (this.isDisposed)
				{
					return;
				}
				BrokerHandlerReferenceCounter brokerHandlerReferenceCounter2;
				if (this.activeHandlers.TryGetValue(subscriptionId, out brokerHandlerReferenceCounter2))
				{
					brokerHandlerReferenceCounter2.Remove(parameters.ChannelId);
					if (brokerHandlerReferenceCounter2.Count == 0)
					{
						this.activeHandlers.Remove(subscriptionId);
						brokerHandlerReferenceCounter = brokerHandlerReferenceCounter2;
					}
				}
			}
			if (brokerHandlerReferenceCounter != null)
			{
				brokerHandlerReferenceCounter.Dispose();
			}
		}

		private void KeepHandlersAlive(object sender, EventArgs e)
		{
			ExDateTime now = ExDateTime.Now;
			foreach (BrokerHandlerReferenceCounter brokerHandlerReferenceCounter in this.GetActiveHandlers())
			{
				brokerHandlerReferenceCounter.KeepAlive(now);
			}
		}

		private BrokerHandlerReferenceCounter[] GetActiveHandlers()
		{
			BrokerHandlerReferenceCounter[] result = Array<BrokerHandlerReferenceCounter>.Empty;
			lock (this.syncRoot)
			{
				if (!this.isDisposed)
				{
					result = this.activeHandlers.Values.ToArray<BrokerHandlerReferenceCounter>();
				}
			}
			return result;
		}

		private OwaMapiNotificationManager mapiNotificationManager;

		private IMailboxContext userContext;

		private Dictionary<string, BrokerHandlerReferenceCounter> activeHandlers;

		private object syncRoot = new object();

		private bool isDisposed;
	}
}
