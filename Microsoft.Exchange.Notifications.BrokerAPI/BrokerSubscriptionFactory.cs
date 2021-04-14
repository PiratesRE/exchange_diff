using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Notifications.Broker
{
	internal static class BrokerSubscriptionFactory
	{
		public static BrokerSubscription Create(Guid subscriptionId, string channelId, DateTime expiration, ExchangePrincipal senderExchangePrincipal, ExchangePrincipal receiverExchangePrincipal, BaseSubscription parameters)
		{
			return BrokerSubscriptionFactory.Create(subscriptionId, channelId, expiration, NotificationParticipantFactory.FromExchangePrincipal(senderExchangePrincipal), NotificationParticipantFactory.FromExchangePrincipal(receiverExchangePrincipal), parameters);
		}

		public static BrokerSubscription Create(Guid subscriptionId, string channelId, DateTime expiration, ADUser senderUser, ADUser receiverUser, BaseSubscription parameters)
		{
			return BrokerSubscriptionFactory.Create(subscriptionId, channelId, expiration, NotificationParticipantFactory.FromADUser(senderUser), NotificationParticipantFactory.FromADUser(receiverUser), parameters);
		}

		public static BrokerSubscription Create(Guid subscriptionId, string channelId, DateTime expiration, NotificationParticipant sender, NotificationParticipant receiver, BaseSubscription parameters)
		{
			ArgumentValidator.ThrowIfInvalidValue<Guid>("subscriptionId must be set", subscriptionId, (Guid g) => g != Guid.Empty);
			ArgumentValidator.ThrowIfInvalidValue<DateTime>("expiration time must be in UTC and in the future", expiration, (DateTime d) => d.Kind == DateTimeKind.Utc && d > DateTime.UtcNow);
			ArgumentValidator.ThrowIfNull("sender", sender);
			ArgumentValidator.ThrowIfNull("receiver", receiver);
			ArgumentValidator.ThrowIfNull("parameters", parameters);
			if (sender.OrganizationId != receiver.OrganizationId)
			{
				throw new NotSupportedException("sender and receiver must belong to the same organization.");
			}
			return new BrokerSubscription
			{
				SubscriptionId = subscriptionId,
				ConsumerId = Consumer.Current.ConsumerId,
				ChannelId = channelId,
				Expiration = expiration,
				Sender = sender,
				Receiver = receiver,
				Parameters = parameters
			};
		}
	}
}
