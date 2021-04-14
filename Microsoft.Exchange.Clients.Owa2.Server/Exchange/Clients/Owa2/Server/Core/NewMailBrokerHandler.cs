using System;
using Microsoft.Exchange.Notifications.Broker;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class NewMailBrokerHandler : BrokerHandler
	{
		public NewMailBrokerHandler(string subscriptionId, SubscriptionParameters parameters, IMailboxContext userContext) : base(subscriptionId, parameters, userContext)
		{
			this.newMailNotifier = new NewMailNotifier(subscriptionId, userContext);
			this.newMailNotifier.RegisterWithPendingRequestNotifier();
		}

		protected override BaseSubscription GetSubscriptionParmeters()
		{
			return new NewMailSubscription
			{
				ConsumerSubscriptionId = base.SubscriptionId
			};
		}

		protected override void HandleNotificatonInternal(BrokerNotification notification)
		{
			NewMailNotification newMailNotification = notification.Payload as NewMailNotification;
			if (newMailNotification == null)
			{
				return;
			}
			this.newMailNotifier.Payload = new NewMailNotificationPayload
			{
				SubscriptionId = newMailNotification.ConsumerSubscriptionId,
				ConversationId = newMailNotification.ConversationId,
				ItemId = newMailNotification.ItemId,
				Sender = newMailNotification.Sender,
				Subject = newMailNotification.Subject,
				PreviewText = newMailNotification.PreviewText,
				EventType = newMailNotification.EventType
			};
			this.newMailNotifier.PickupData();
		}

		private readonly NewMailNotifier newMailNotifier;
	}
}
