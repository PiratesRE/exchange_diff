using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Notifications.Broker;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class UnseenCountBrokerHandler : BrokerHandler
	{
		public UnseenCountBrokerHandler(string subscriptionId, SubscriptionParameters parameters, IMailboxContext userContext, IRecipientSession adSession) : base(subscriptionId, parameters, userContext)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.adSession = adSession;
				this.unseenItemNotifier = new UnseenItemNotifier(subscriptionId, userContext, null, null);
				this.unseenItemNotifier.RegisterWithPendingRequestNotifier();
				disposeGuard.Success();
			}
		}

		protected override ExchangePrincipal SenderPrincipal
		{
			get
			{
				return ExchangePrincipal.FromProxyAddress(this.adSession, base.Parameters.MailboxId, RemotingOptions.AllowCrossSite);
			}
		}

		protected override BaseSubscription GetSubscriptionParmeters()
		{
			return new UnseenCountSubscription
			{
				ConsumerSubscriptionId = base.SubscriptionId,
				UserExternalDirectoryObjectId = base.UserContext.ExchangePrincipal.ExternalDirectoryObjectId,
				UserLegacyDN = base.UserContext.ExchangePrincipal.LegacyDn
			};
		}

		protected override void HandleNotificatonInternal(BrokerNotification notification)
		{
			UnseenCountNotification unseenCountNotification = notification.Payload as UnseenCountNotification;
			if (unseenCountNotification == null)
			{
				return;
			}
			this.unseenItemNotifier.AddGroupNotificationPayload(new UnseenItemNotificationPayload
			{
				SubscriptionId = unseenCountNotification.ConsumerSubscriptionId,
				UnseenData = unseenCountNotification.UnseenData
			});
			this.unseenItemNotifier.PickupData();
		}

		private readonly UnseenItemNotifier unseenItemNotifier;

		private readonly IRecipientSession adSession;
	}
}
