using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class RemoteSubscriptionsInfo : ISubscriptionInfo
	{
		public event EventHandler onNoActiveSubscriptionEvent;

		public event EventHandler<RemoteSubscriptionEventArgs> noListenersForSubscriptionEvent;

		public int SubscriptionCount
		{
			get
			{
				return this.remoteSubscriptions.Count;
			}
		}

		public string Mailbox { get; private set; }

		public string ContextKey { get; private set; }

		public RemoteSubscriptionsInfo(string mailbox, string contextKey)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("mailbox", mailbox);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("contextKey", contextKey);
			this.Mailbox = mailbox;
			this.ContextKey = contextKey;
		}

		public void Add(string subscriptionId, string channelId, string user, NotificationType notificationType, out bool subscriptionExists)
		{
			this.Add(subscriptionId, channelId, user, notificationType, null, out subscriptionExists);
		}

		public void Add(string subscriptionId, string channelId, string user, NotificationType notificationType, string remoteEndpointOverride, out bool subscriptionExists)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("subscriptionId", subscriptionId);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("channelId", channelId);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("user", user);
			RemoteSubscriptionsInfo.NotificationTypeAndChannelInfo notificationTypeAndChannelInfo;
			if (!this.remoteSubscriptions.TryGetValue(subscriptionId, out notificationTypeAndChannelInfo))
			{
				notificationTypeAndChannelInfo = (this.remoteSubscriptions[subscriptionId] = new RemoteSubscriptionsInfo.NotificationTypeAndChannelInfo(notificationType));
			}
			HashSet<RemoteChannelInfo> channels = notificationTypeAndChannelInfo.Channels;
			RemoteChannelInfo remoteChannelInfo = new RemoteChannelInfo(channelId, user);
			if (!string.IsNullOrEmpty(remoteEndpointOverride) && AppConfigLoader.GetConfigBoolValue("Test_OwaAllowHeaderOverride", false))
			{
				remoteChannelInfo.EndpointTestOverride = remoteEndpointOverride;
			}
			subscriptionExists = !channels.Add(remoteChannelInfo);
			if (subscriptionExists)
			{
				SubscriberConcurrencyTracker.Instance.OnResubscribe(this.Mailbox, notificationType);
			}
			else
			{
				SubscriberConcurrencyTracker.Instance.OnSubscribe(this.Mailbox, notificationType);
			}
			this.AddSubscriptionToPerChannelList(channelId, subscriptionId);
		}

		public void Remove(string subscriptionId, string channelId, string user)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("subscriptionId", subscriptionId);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("channelId", channelId);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("user", user);
			this.Remove(subscriptionId, new RemoteChannelInfo(channelId, user));
		}

		public void NotifyAllChannelsRemoved()
		{
			Dictionary<NotificationType, int> dictionary = new Dictionary<NotificationType, int>();
			foreach (RemoteSubscriptionsInfo.NotificationTypeAndChannelInfo notificationTypeAndChannelInfo in this.remoteSubscriptions.Values)
			{
				int num;
				if (!dictionary.TryGetValue(notificationTypeAndChannelInfo.Type, out num))
				{
					dictionary.Add(notificationTypeAndChannelInfo.Type, notificationTypeAndChannelInfo.Channels.Count);
				}
				else
				{
					Dictionary<NotificationType, int> dictionary2;
					NotificationType type;
					(dictionary2 = dictionary)[type = notificationTypeAndChannelInfo.Type] = dictionary2[type] + notificationTypeAndChannelInfo.Channels.Count;
				}
			}
			foreach (KeyValuePair<NotificationType, int> keyValuePair in dictionary)
			{
				SubscriberConcurrencyTracker.Instance.OnUnsubscribe(this.Mailbox, keyValuePair.Key, keyValuePair.Value);
			}
		}

		public bool CleanUpChannel(string channelId)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("channelId", channelId);
			List<string> list;
			if (this.perChannelSubscriptionLists.TryGetValue(channelId, out list))
			{
				foreach (string subscriptionId in list.ToArray())
				{
					this.Remove(subscriptionId, new RemoteChannelInfo(channelId, null));
				}
				return true;
			}
			return false;
		}

		public RemoteChannelInfo[] GetChannels(string subscriptionId)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("subscriptionId", subscriptionId);
			RemoteSubscriptionsInfo.NotificationTypeAndChannelInfo notificationTypeAndChannelInfo;
			if (this.remoteSubscriptions.TryGetValue(subscriptionId, out notificationTypeAndChannelInfo))
			{
				return notificationTypeAndChannelInfo.Channels.ToArray<RemoteChannelInfo>();
			}
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "RemoteSubscriptionInfo.GetChannels - Subscription not active. SubscriptionId: {0}", subscriptionId);
			return Array<RemoteChannelInfo>.Empty;
		}

		private void Remove(string subscriptionId, RemoteChannelInfo channelInfo)
		{
			RemoteSubscriptionsInfo.NotificationTypeAndChannelInfo notificationTypeAndChannelInfo;
			if (this.remoteSubscriptions.TryGetValue(subscriptionId, out notificationTypeAndChannelInfo))
			{
				if (notificationTypeAndChannelInfo.Channels.Remove(channelInfo))
				{
					SubscriberConcurrencyTracker.Instance.OnUnsubscribe(this.Mailbox, notificationTypeAndChannelInfo.Type);
				}
				this.RemoveSubscriptionFromPerChannelList(channelInfo.ChannelId, subscriptionId);
				if (notificationTypeAndChannelInfo.Channels.Count == 0)
				{
					this.remoteSubscriptions.Remove(subscriptionId);
					this.RaiseEventsIfNeeded(subscriptionId);
					return;
				}
			}
			else
			{
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "RemoteSubscriptionInfo.Remove - Subscription not active. SubscriptionId: {0}", subscriptionId);
			}
		}

		private void RaiseEventsIfNeeded(string subscriptionId)
		{
			if (this.noListenersForSubscriptionEvent != null)
			{
				this.noListenersForSubscriptionEvent(this, new RemoteSubscriptionEventArgs(this.ContextKey, subscriptionId));
			}
			if (this.SubscriptionCount == 0 && this.onNoActiveSubscriptionEvent != null)
			{
				this.onNoActiveSubscriptionEvent(this, EventArgs.Empty);
			}
		}

		private void AddSubscriptionToPerChannelList(string channelId, string subscriptionId)
		{
			List<string> list;
			if (this.perChannelSubscriptionLists.TryGetValue(channelId, out list))
			{
				if (!list.Contains(subscriptionId))
				{
					list.Add(subscriptionId);
					return;
				}
			}
			else
			{
				this.perChannelSubscriptionLists[channelId] = new List<string>(5);
				this.perChannelSubscriptionLists[channelId].Add(subscriptionId);
			}
		}

		private void RemoveSubscriptionFromPerChannelList(string channelId, string subscriptionId)
		{
			List<string> list;
			if (this.perChannelSubscriptionLists.TryGetValue(channelId, out list))
			{
				list.Remove(subscriptionId);
				if (list.Count == 0)
				{
					this.perChannelSubscriptionLists.Remove(channelId);
				}
			}
		}

		private const int InitialSizeForChannelList = 5;

		private readonly Dictionary<string, RemoteSubscriptionsInfo.NotificationTypeAndChannelInfo> remoteSubscriptions = new Dictionary<string, RemoteSubscriptionsInfo.NotificationTypeAndChannelInfo>();

		private readonly Dictionary<string, List<string>> perChannelSubscriptionLists = new Dictionary<string, List<string>>();

		private class NotificationTypeAndChannelInfo
		{
			public NotificationType Type { get; private set; }

			public HashSet<RemoteChannelInfo> Channels { get; private set; }

			public NotificationTypeAndChannelInfo(NotificationType type)
			{
				this.Type = type;
				this.Channels = new HashSet<RemoteChannelInfo>();
			}
		}
	}
}
