using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public static class MessagePrefetchConfiguration
	{
		public static int NumberOfMessagesToPrefetch
		{
			get
			{
				return MessagePrefetchConfiguration.numberOfMessagesToPrefetch;
			}
		}

		public static bool PrefetchOnlyUnreadMessages
		{
			get
			{
				return MessagePrefetchConfiguration.prefetchOnlyUnreadMessages;
			}
		}

		public static int MaxMessagesInCache
		{
			get
			{
				return MessagePrefetchConfiguration.maxMessagesInCache;
			}
		}

		public static int MaxCacheSizeInMegaBytes
		{
			get
			{
				return MessagePrefetchConfiguration.maxCacheSizeInMegaBytes;
			}
		}

		public static void InitializeSettings()
		{
			MessagePrefetchConfiguration.isMessagePrefetchEnabled = AppSettings.GetConfiguredValue<bool>("IsMessagePrefetchEnabled", true);
			MessagePrefetchConfiguration.numberOfMessagesToPrefetch = AppSettings.GetConfiguredValue<int>("NumberOfMessagesToPrefetch", 0);
			MessagePrefetchConfiguration.prefetchOnlyUnreadMessages = AppSettings.GetConfiguredValue<bool>("PrefetchOnlyUnreadMessages", false);
			MessagePrefetchConfiguration.maxMessagesInCache = AppSettings.GetConfiguredValue<int>("MaxMessagesInCache", 500);
			MessagePrefetchConfiguration.maxCacheSizeInMegaBytes = AppSettings.GetConfiguredValue<int>("MaxCacheSizeInMegaBytes", 30);
		}

		public static bool IsMessagePrefetchEnabled(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			return !userContext.IsWebPartRequest && MessagePrefetchConfiguration.isMessagePrefetchEnabled;
		}

		internal static bool IsMessagePrefetchEnabledForSession(UserContext userContext, StoreSession session)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			return MessagePrefetchConfiguration.IsMessagePrefetchEnabled(userContext) && ServerVersion.IsE14SP1OrGreater(userContext.MailboxSession.MailboxOwner.MailboxInfo.Location.ServerVersion) && !(session is PublicFolderSession);
		}

		private static bool isMessagePrefetchEnabled;

		private static int numberOfMessagesToPrefetch;

		private static bool prefetchOnlyUnreadMessages;

		private static int maxMessagesInCache;

		private static int maxCacheSizeInMegaBytes;
	}
}
