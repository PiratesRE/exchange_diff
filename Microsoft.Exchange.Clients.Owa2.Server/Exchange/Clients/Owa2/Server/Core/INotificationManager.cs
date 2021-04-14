using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal interface INotificationManager : IDisposable
	{
		event EventHandler<EventArgs> RemoteKeepAliveEvent;

		SearchNotificationHandler SearchNotificationHandler { get; }

		void SubscribeToHierarchyNotification(string subscriptionId);

		void SubscribeToRowNotification(string subscriptionId, SubscriptionParameters parameters, ExTimeZone timeZone, CallContext callContext, bool remoteSubscription);

		void SubscribeToReminderNotification(string subscriptionId);

		void SubscribeToNewMailNotification(string subscriptionId, SubscriptionParameters parameters);

		string SubscribeToUnseenItemNotification(string subscriptionId, UserMailboxLocator mailboxLocator, IRecipientSession adSession);

		void SubscribeToGroupAssociationNotification(string subscriptionId, IRecipientSession adSession);

		void SubscribeToSearchNotification();

		void UnsubscribeForRowNotifications(string subscriptionId, SubscriptionParameters parameters);

		void CleanupSubscriptions();

		void HandleConnectionDroppedNotification();

		void RefreshSubscriptions(ExTimeZone timeZone);

		void StartRemoteKeepAliveTimer();

		void ReleaseSubscriptionsForChannelId(string channelId);

		void ReleaseSubscription(string subscriptionId);

		void SubscribeToUnseenCountNotification(string subscriptionId, SubscriptionParameters parameters, IRecipientSession adSession);

		void UnsubscribeToUnseenCountNotification(string subscriptionId, SubscriptionParameters parameters);
	}
}
