using System;
using System.Collections.Concurrent;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class ExchangeServiceNotificationHandler : DisposeTrackableBase
	{
		internal static ExchangeServiceNotificationHandler GetHandler(CallContext callContext)
		{
			return ExchangeServiceNotificationHandler.GetHandler(callContext, false);
		}

		internal static ExchangeServiceNotificationHandler GetHandler(CallContext callContext, bool isUnifiedSessionRequired)
		{
			string key = ExchangeServiceNotificationHandler.MakeKey(callContext, isUnifiedSessionRequired);
			return ExchangeServiceNotificationHandler.allHandlers.GetOrAdd(key, (string param0) => new ExchangeServiceNotificationHandler(isUnifiedSessionRequired));
		}

		internal static void RemoveHandler(CallContext callContext, bool isUnifiedSessionRequired)
		{
			string key = ExchangeServiceNotificationHandler.MakeKey(callContext, isUnifiedSessionRequired);
			ExchangeServiceNotificationHandler exchangeServiceNotificationHandler;
			if (ExchangeServiceNotificationHandler.allHandlers.TryRemove(key, out exchangeServiceNotificationHandler))
			{
				exchangeServiceNotificationHandler.Dispose();
			}
		}

		private static string MakeKey(CallContext callContext, bool isUnifiedSessionRequired)
		{
			return string.Format("{0}_{1}_{2}_{3}_{4}", new object[]
			{
				callContext.EffectiveCaller.ObjectGuid,
				callContext.OriginalCallerContext.IdentifierString,
				callContext.MailboxIdentityPrincipal.MailboxInfo.MailboxGuid,
				callContext.LogonType,
				isUnifiedSessionRequired
			});
		}

		private ExchangeServiceNotificationHandler(bool isUnifiedSessionRequired)
		{
			this.isUnifiedSessionRequired = isUnifiedSessionRequired;
		}

		internal MailboxSession Session { get; private set; }

		internal Subscription DisconnectSubscription { get; private set; }

		internal void AddSubscription(ExchangePrincipal exchangePrincipal, CallContext callContext, string subscriptionId, ExchangeServiceNotificationHandler.SubscriptionCreator creatorDelegate)
		{
			lock (this.mutex)
			{
				base.CheckDisposed();
				if (this.Session != null && this.Session.IsDead)
				{
					this.Session.Dispose();
					this.Session = null;
				}
				if (this.Session == null)
				{
					this.Session = MailboxSession.OpenWithBestAccess(exchangePrincipal, callContext.AccessingADUser, callContext.EffectiveCaller.ClientSecurityContext, callContext.ClientCulture, "Client=WebServices;Action=ExchangeServiceNotification", this.isUnifiedSessionRequired);
					this.DisconnectSubscription = Subscription.CreateMailboxSubscription(this.Session, new NotificationHandler(this.OnDisconnect), NotificationType.ConnectionDropped);
				}
				this.RemoveSubscriptionInternal(subscriptionId);
				ExchangeServiceSubscription value;
				try
				{
					value = creatorDelegate();
				}
				catch (ConnectionFailedTransientException)
				{
					ExchangeServiceNotificationHandler.RemoveHandler(callContext, this.isUnifiedSessionRequired);
					throw;
				}
				this.subscriptions.TryAdd(subscriptionId, value);
			}
		}

		internal void RemoveSubscription(string subscriptionId)
		{
			lock (this.mutex)
			{
				this.RemoveSubscriptionInternal(subscriptionId);
				if (this.subscriptions.Count == 0)
				{
					if (this.Session != null)
					{
						this.Session.Dispose();
						this.Session = null;
					}
					if (this.DisconnectSubscription != null)
					{
						this.DisconnectSubscription.Dispose();
						this.DisconnectSubscription = null;
					}
				}
			}
		}

		private void OnDisconnect(Notification notification)
		{
			foreach (ExchangeServiceSubscription exchangeServiceSubscription in this.subscriptions.Values)
			{
				exchangeServiceSubscription.HandleNotification(notification);
			}
		}

		private void RemoveSubscriptionInternal(string subscriptionId)
		{
			if (subscriptionId == null)
			{
				return;
			}
			ExchangeServiceSubscription exchangeServiceSubscription = null;
			if (this.subscriptions.TryRemove(subscriptionId, out exchangeServiceSubscription) && exchangeServiceSubscription != null)
			{
				exchangeServiceSubscription.Dispose();
				exchangeServiceSubscription = null;
			}
		}

		protected override void InternalDispose(bool isDisposing)
		{
			lock (this.mutex)
			{
				base.CheckDisposed();
				foreach (ExchangeServiceSubscription exchangeServiceSubscription in this.subscriptions.Values)
				{
					exchangeServiceSubscription.Dispose();
				}
				this.subscriptions.Clear();
				if (this.Session != null)
				{
					this.Session.Dispose();
					this.Session = null;
				}
				if (this.DisconnectSubscription != null)
				{
					this.DisconnectSubscription.Dispose();
					this.DisconnectSubscription = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ExchangeServiceNotificationHandler>(this);
		}

		internal const string ClientInfoString = "Client=WebServices;Action=ExchangeServiceNotification";

		private static readonly ConcurrentDictionary<string, ExchangeServiceNotificationHandler> allHandlers = new ConcurrentDictionary<string, ExchangeServiceNotificationHandler>();

		private readonly object mutex = new object();

		private readonly ConcurrentDictionary<string, ExchangeServiceSubscription> subscriptions = new ConcurrentDictionary<string, ExchangeServiceSubscription>();

		private bool isUnifiedSessionRequired;

		internal delegate ExchangeServiceSubscription SubscriptionCreator();
	}
}
