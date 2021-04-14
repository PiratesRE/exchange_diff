using System;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Notifications.Broker;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal abstract class BrokerHandler : DisposeTrackableBase
	{
		protected BrokerHandler(string subscriptionId, SubscriptionParameters parameters, IMailboxContext userContext)
		{
			this.SubscriptionId = subscriptionId;
			this.Parameters = parameters;
			this.UserContext = userContext;
			this.BrokerSubscriptionId = Guid.NewGuid();
			this.nextResubscribeTime = ExDateTime.MinValue;
		}

		public string SubscriptionId { get; private set; }

		public Guid BrokerSubscriptionId { get; private set; }

		private protected SubscriptionParameters Parameters { protected get; private set; }

		private protected IMailboxContext UserContext { protected get; private set; }

		protected virtual int ExpirationDurationInMins
		{
			get
			{
				return BrokerHandler.DefaultExpirationDurationInMins;
			}
		}

		protected virtual int ResubscribeTimeInMins
		{
			get
			{
				return BrokerHandler.DefaultResubscribeTimeInMins;
			}
		}

		public virtual IBrokerGateway Gateway
		{
			get
			{
				return BrokerGateway.Instance;
			}
		}

		protected virtual ExchangePrincipal ReceiverPrincipal
		{
			get
			{
				return this.UserContext.ExchangePrincipal;
			}
		}

		protected virtual ExchangePrincipal SenderPrincipal
		{
			get
			{
				return this.UserContext.ExchangePrincipal;
			}
		}

		public void Subscribe()
		{
			try
			{
				this.SubscribeInternal();
			}
			catch (NotificationsBrokerException handledException)
			{
				OwaServerTraceLogger.AppendToLog(new BrokerLogEvent
				{
					Principal = this.UserContext.ExchangePrincipal,
					UserContextKey = this.UserContext.Key.ToString(),
					SubscriptionId = this.SubscriptionId,
					BrokerSubscriptionId = this.BrokerSubscriptionId,
					EventName = "Subscribe",
					HandledException = handledException
				});
				throw;
			}
		}

		public void KeepAlive(ExDateTime eventTime)
		{
			if (base.IsDisposed || eventTime <= this.nextResubscribeTime)
			{
				return;
			}
			try
			{
				this.SubscribeInternal();
			}
			catch (NotificationsBrokerException handledException)
			{
				OwaServerTraceLogger.AppendToLog(new BrokerLogEvent
				{
					Principal = this.UserContext.ExchangePrincipal,
					UserContextKey = this.UserContext.Key.ToString(),
					SubscriptionId = this.SubscriptionId,
					BrokerSubscriptionId = this.BrokerSubscriptionId,
					EventName = "KeepAlive",
					HandledException = handledException
				});
			}
		}

		public void HandleNotification(BrokerNotification notification)
		{
			try
			{
				if (!base.IsDisposed)
				{
					OwaDiagnostics.SendWatsonReportsForGrayExceptions(delegate()
					{
						this.HandleNotificatonInternal(notification);
					});
				}
			}
			catch (GrayException handledException)
			{
				OwaServerTraceLogger.AppendToLog(new BrokerLogEvent
				{
					Principal = this.UserContext.ExchangePrincipal,
					UserContextKey = this.UserContext.Key.ToString(),
					SubscriptionId = this.SubscriptionId,
					BrokerSubscriptionId = this.BrokerSubscriptionId,
					EventName = "HandleNotification",
					HandledException = handledException
				});
			}
		}

		protected abstract BaseSubscription GetSubscriptionParmeters();

		protected abstract void HandleNotificatonInternal(BrokerNotification notification);

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<BrokerHandler>(this);
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing)
			{
				this.Unsubscribe();
			}
		}

		private void SubscribeInternal()
		{
			this.Gateway.Subscribe(this.GetBrokerSubscription(), this);
			this.nextResubscribeTime = ExDateTime.Now.AddMinutes((double)this.ResubscribeTimeInMins);
		}

		private void Unsubscribe()
		{
			try
			{
				this.Gateway.Unsubscribe(this.GetBrokerSubscription());
			}
			catch (NotificationsBrokerException handledException)
			{
				OwaServerTraceLogger.AppendToLog(new BrokerLogEvent
				{
					Principal = this.UserContext.ExchangePrincipal,
					UserContextKey = this.UserContext.Key.ToString(),
					SubscriptionId = this.SubscriptionId,
					BrokerSubscriptionId = this.BrokerSubscriptionId,
					EventName = "Unsubscribe",
					HandledException = handledException
				});
			}
		}

		private BrokerSubscription GetBrokerSubscription()
		{
			return BrokerSubscriptionFactory.Create(this.BrokerSubscriptionId, this.Parameters.ChannelId, DateTime.UtcNow.AddMinutes((double)this.ExpirationDurationInMins), this.SenderPrincipal, this.ReceiverPrincipal, this.GetSubscriptionParmeters());
		}

		private const string DefaultExpirationDurationKey = "BrokerHandlerDefaultExpirationDurationInMins";

		private const string DefaultResubscribeTimeKey = "BrokerHandlerDefaultResubscribeTimeInMins";

		private static readonly int DefaultExpirationDurationInMins = BaseApplication.GetAppSetting<int>("BrokerHandlerDefaultExpirationDurationInMins", 60);

		private static readonly int DefaultResubscribeTimeInMins = BaseApplication.GetAppSetting<int>("BrokerHandlerDefaultResubscribeTimeInMins", 45);

		private ExDateTime nextResubscribeTime;
	}
}
