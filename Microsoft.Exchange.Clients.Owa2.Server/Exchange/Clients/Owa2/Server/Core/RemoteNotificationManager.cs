using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class RemoteNotificationManager
	{
		public static RemoteNotificationManager Instance
		{
			get
			{
				return RemoteNotificationManager.instance;
			}
		}

		protected RemoteNotificationManager()
		{
		}

		public void Subscribe(string contextKey, string subscriptionMailbox, string subscriptionId, string channelId, string user, NotificationType notificationType, out bool subscriptionExists)
		{
			this.Subscribe(contextKey, subscriptionMailbox, subscriptionId, channelId, user, notificationType, null, out subscriptionExists);
		}

		public void Subscribe(string contextKey, string subscriptionMailbox, string subscriptionId, string channelId, string user, NotificationType notificationType, string remoteEndpointOverride, out bool subscriptionExists)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("contextKey", contextKey);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("subscriptionMailbox", subscriptionMailbox);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("subscriptionId", subscriptionId);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("channelId", channelId);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("user", user);
			if (this.contextKeySubscriptionsLock.LockWriterElastic(3000))
			{
				try
				{
					ISubscriptionInfo subscriptionInfo;
					if (!this.contextKeySubscriptions.TryGetValue(contextKey, out subscriptionInfo))
					{
						subscriptionInfo = (this.contextKeySubscriptions[contextKey] = this.CreateSubscriptionInfo(subscriptionMailbox, contextKey));
						subscriptionInfo.onNoActiveSubscriptionEvent += this.NoActiveSubscription;
						subscriptionInfo.noListenersForSubscriptionEvent += this.NoActiveListeners;
					}
					subscriptionInfo.Add(subscriptionId, channelId, user, notificationType, remoteEndpointOverride, out subscriptionExists);
					return;
				}
				finally
				{
					this.contextKeySubscriptionsLock.ReleaseWriterLock();
				}
			}
			ExTraceGlobals.NotificationsCallTracer.TraceError((long)this.GetHashCode(), "[RemoteNotificationManager::Subscribe] mailboxSubscriptionsLock timed");
			throw new OwaLockTimeoutException(string.Format("Could not acquire WriterLock on mailboxSubscriptions", new object[0]));
		}

		public virtual ISubscriptionInfo CreateSubscriptionInfo(string mailbox, string contextKey)
		{
			return new RemoteSubscriptionsInfo(mailbox, contextKey);
		}

		public void UnSubscribe(string contextKey, string subscriptionId, string channelId, string user)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("contextKey", contextKey);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("subscriptionId", subscriptionId);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("channelId", channelId);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("user", user);
			if (this.contextKeySubscriptionsLock.LockWriterElastic(3000))
			{
				try
				{
					ISubscriptionInfo subscriptionInfo;
					if (this.contextKeySubscriptions.TryGetValue(contextKey, out subscriptionInfo))
					{
						subscriptionInfo.Remove(subscriptionId, channelId, user);
					}
					else
					{
						ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "RemoteNotificationManager.UnSubscribe - No Subscriptions exists for this user context. UserContextKey: {0}", contextKey);
					}
					return;
				}
				finally
				{
					this.contextKeySubscriptionsLock.ReleaseWriterLock();
				}
			}
			ExTraceGlobals.NotificationsCallTracer.TraceError((long)this.GetHashCode(), "[RemoteNotificationManager::UnSubscribe] mailboxSubscriptionsLock timed");
			throw new OwaLockTimeoutException(string.Format("Could not acquire WriterLock on mailboxSubscriptions", new object[0]));
		}

		public void CleanUpSubscriptions(string contextKey)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("contextKey", contextKey);
			if (this.contextKeySubscriptionsLock.LockWriterElastic(3000))
			{
				try
				{
					ISubscriptionInfo subscriptions;
					if (this.contextKeySubscriptions.TryGetValue(contextKey, out subscriptions))
					{
						this.RemoveSubscriptions(subscriptions);
						ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "RemoteNotificationManager.CleanUpSubscriptions - All subscriptions removed for this user context. UserContextKey: {0}", contextKey);
					}
					else
					{
						ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "RemoteNotificationManager.CleanUpSubscriptions - No Subscriptions exists for this user context. UserContextKey: {0}", contextKey);
					}
					return;
				}
				finally
				{
					this.contextKeySubscriptionsLock.ReleaseWriterLock();
				}
			}
			ExTraceGlobals.NotificationsCallTracer.TraceError((long)this.GetHashCode(), "[RemoteNotificationManager::CleanUpSubscriptions] mailboxSubscriptionsLock timed");
			throw new OwaLockTimeoutException(string.Format("Could not acquire WriterLock on mailboxSubscriptions", new object[0]));
		}

		public virtual void CleanUpChannel(string channelId)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("channelId", channelId);
			List<string> list = new List<string>();
			if (this.contextKeySubscriptionsLock.LockWriterElastic(3000))
			{
				try
				{
					foreach (ISubscriptionInfo subscriptionInfo in this.contextKeySubscriptions.Values.ToArray<ISubscriptionInfo>())
					{
						if (subscriptionInfo.CleanUpChannel(channelId))
						{
							list.Add(subscriptionInfo.ContextKey);
						}
					}
					goto IL_9D;
				}
				finally
				{
					this.contextKeySubscriptionsLock.ReleaseWriterLock();
				}
				goto IL_71;
				IL_9D:
				foreach (string contextKey in list)
				{
					UserContextKey userContextKey;
					if (this.TryParseUserContextKey(contextKey, out userContextKey))
					{
						IMailboxContext mailboxContextFromCache = UserContextManager.GetMailboxContextFromCache(userContextKey);
						if (mailboxContextFromCache != null)
						{
							mailboxContextFromCache.NotificationManager.ReleaseSubscriptionsForChannelId(channelId);
						}
					}
				}
				return;
			}
			IL_71:
			ExTraceGlobals.NotificationsCallTracer.TraceError((long)this.GetHashCode(), "[RemoteNotificationManager::CleanUpChannel] mailboxSubscriptionsLock timed");
			throw new OwaLockTimeoutException(string.Format("Could not acquire WriterLock on mailboxSubscriptions", new object[0]));
		}

		public virtual IEnumerable<IDestinationInfo> GetDestinations(string contextKey, string subscriptionId)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("contextKey", contextKey);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("subscriptionId", subscriptionId);
			Dictionary<Uri, IDestinationInfo> dictionary = new Dictionary<Uri, IDestinationInfo>();
			foreach (RemoteChannelInfo remoteChannelInfo in this.GetChannels(contextKey, subscriptionId))
			{
				Uri uri;
				if (string.IsNullOrEmpty(remoteChannelInfo.EndpointTestOverride))
				{
					uri = this.GetDestinationUri(remoteChannelInfo.User);
				}
				else
				{
					uri = new Uri(remoteChannelInfo.EndpointTestOverride);
				}
				IDestinationInfo destinationInfo;
				if (uri == null)
				{
					string message = string.Format("Could not resolve url. User - {0}, Subscription Id - {1}, UserContextKey - {2}, Channel - {3}.", new object[]
					{
						remoteChannelInfo.User,
						subscriptionId,
						contextKey,
						remoteChannelInfo.ChannelId
					});
					OwaServerTraceLogger.AppendToLog(new TraceLogEvent("RemoteNotificationManager", null, "GetDestinations", message));
					this.UnSubscribe(contextKey, subscriptionId, remoteChannelInfo.ChannelId, remoteChannelInfo.User);
				}
				else if (!dictionary.TryGetValue(uri, out destinationInfo))
				{
					dictionary[uri] = new RemoteDestinationInfo(uri, remoteChannelInfo.ChannelId);
				}
				else
				{
					destinationInfo.AddChannel(remoteChannelInfo.ChannelId);
				}
			}
			return dictionary.Values;
		}

		public virtual Uri GetDestinationUri(string user)
		{
			Uri result;
			if (HttpProxyBackEndHelper.TryGetBackEndWebServicesUrlFromSmtp(user, (SmtpAddress address) => UserContextUtilities.CreateScopedRecipientSession(true, ConsistencyMode.FullyConsistent, address.Domain, null), out result))
			{
				return result;
			}
			return null;
		}

		private RemoteChannelInfo[] GetChannels(string contextKey, string subscriptionId)
		{
			RemoteChannelInfo[] result = Array<RemoteChannelInfo>.Empty;
			if (this.contextKeySubscriptionsLock.LockReaderElastic(3000))
			{
				try
				{
					ISubscriptionInfo subscriptionInfo;
					this.contextKeySubscriptions.TryGetValue(contextKey, out subscriptionInfo);
					if (subscriptionInfo != null)
					{
						result = subscriptionInfo.GetChannels(subscriptionId);
					}
					else
					{
						ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "RemoteNotificationManager.GetChannels - No Subscriptions exists for this user context. UserContextKey: {0}", contextKey);
					}
					return result;
				}
				finally
				{
					this.contextKeySubscriptionsLock.ReleaseReaderLock();
				}
			}
			ExTraceGlobals.NotificationsCallTracer.TraceError((long)this.GetHashCode(), "[RemoteNotificationManager::GetChannels] mailboxSubscriptionsLock timed");
			throw new OwaLockTimeoutException(string.Format("Could not acquire ReaderLock on mailboxSubscriptions", new object[0]));
		}

		private void NoActiveListeners(object sender, RemoteSubscriptionEventArgs eventArgs)
		{
			UserContextKey userContextKey;
			if (this.TryParseUserContextKey(eventArgs.ContextKey, out userContextKey))
			{
				IMailboxContext mailboxContextFromCache = UserContextManager.GetMailboxContextFromCache(userContextKey);
				if (mailboxContextFromCache != null)
				{
					mailboxContextFromCache.NotificationManager.ReleaseSubscription(eventArgs.SubscriptionId);
				}
			}
		}

		private void NoActiveSubscription(object sender, EventArgs e)
		{
			ISubscriptionInfo subscriptionInfo = sender as ISubscriptionInfo;
			if (subscriptionInfo != null)
			{
				this.RemoveSubscriptions(subscriptionInfo);
				UserContextKey userContextKey;
				if (this.TryParseUserContextKey(subscriptionInfo.ContextKey, out userContextKey))
				{
					UserContext userContext = UserContextManager.GetMailboxContextFromCache(userContextKey) as UserContext;
					if (userContext != null && userContext.IsGroupUserContext)
					{
						userContext.RetireMailboxSessionForGroupMailbox();
					}
				}
			}
		}

		private bool TryParseUserContextKey(string contextKey, out UserContextKey contextKeyObj)
		{
			bool flag = UserContextKey.TryParse(contextKey, out contextKeyObj);
			if (!flag)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceError<string>((long)this.GetHashCode(), "RemoteNotificationManager.TryParseUserContextKey - couldn't parse UserContextKey {0}, skipping cleanup", contextKey);
				ExWatson.SendReport(new ArgumentException(string.Format("RemoteNotificationManager::TryParseUserContextKey - TryParse failed for UserContextKey string - '{0}'", contextKey)), ReportOptions.None, null);
			}
			return flag;
		}

		private void RemoveSubscriptions(ISubscriptionInfo subscriptions)
		{
			if (subscriptions == null)
			{
				return;
			}
			if (this.contextKeySubscriptions.Remove(subscriptions.ContextKey))
			{
				subscriptions.NotifyAllChannelsRemoved();
			}
			subscriptions.onNoActiveSubscriptionEvent -= this.NoActiveSubscription;
			subscriptions.noListenersForSubscriptionEvent -= this.NoActiveListeners;
		}

		internal const int ContextKeySubscriptionsLockTimeout = 3000;

		private static readonly RemoteNotificationManager instance = new RemoteNotificationManager();

		private readonly OwaRWLockWrapper contextKeySubscriptionsLock = new OwaRWLockWrapper();

		protected Dictionary<string, ISubscriptionInfo> contextKeySubscriptions = new Dictionary<string, ISubscriptionInfo>();
	}
}
