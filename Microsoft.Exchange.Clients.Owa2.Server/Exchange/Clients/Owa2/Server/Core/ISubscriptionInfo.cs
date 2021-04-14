using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal interface ISubscriptionInfo
	{
		event EventHandler onNoActiveSubscriptionEvent;

		event EventHandler<RemoteSubscriptionEventArgs> noListenersForSubscriptionEvent;

		int SubscriptionCount { get; }

		string Mailbox { get; }

		string ContextKey { get; }

		void Add(string subscriptionId, string channelId, string user, NotificationType notificationType, out bool subscriptionExists);

		void Add(string subscriptionId, string channelId, string user, NotificationType notificationType, string remoteEndpointOverride, out bool subscriptionExists);

		void Remove(string subscriptionId, string channelId, string user);

		void NotifyAllChannelsRemoved();

		bool CleanUpChannel(string channelId);

		RemoteChannelInfo[] GetChannels(string subscriptionId);
	}
}
