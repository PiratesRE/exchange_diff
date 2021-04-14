using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UserSyncStateMetadataCache
	{
		public static UserSyncStateMetadataCache Singleton
		{
			get
			{
				return UserSyncStateMetadataCache.singleton.Value;
			}
		}

		private UserSyncStateMetadataCache()
		{
			this.cache = new ExactTimeoutCache<Guid, UserSyncStateMetadata>(new RemoveItemDelegate<Guid, UserSyncStateMetadata>(this.HandleRemoveItem), null, null, UserSyncStateMetadataCache.CacheSizeLimitEntry.Value, false, CacheFullBehavior.ExpireExisting);
		}

		public UserSyncStateMetadata Get(MailboxSession mailboxSession, ISyncLogger syncLogger = null)
		{
			if (syncLogger == null)
			{
				syncLogger = TracingLogger.Singleton;
			}
			UserSyncStateMetadata userSyncStateMetadata = null;
			if (!this.cache.TryGetValue(mailboxSession.MailboxGuid, out userSyncStateMetadata))
			{
				syncLogger.TraceDebug<Guid>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[UserSyncStateMetadataCache.Get] Cache miss for mailbox {0}", mailboxSession.MailboxGuid);
				userSyncStateMetadata = new UserSyncStateMetadata(mailboxSession);
				lock (this.instanceLock)
				{
					UserSyncStateMetadata result = null;
					if (this.cache.TryGetValue(mailboxSession.MailboxGuid, out result))
					{
						syncLogger.TraceDebug(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[UserSyncStateMetadataCache.Get] Second TryGet returned cached value.  Discarding new one.");
						return result;
					}
					this.cache.TryAddSliding(mailboxSession.MailboxGuid, userSyncStateMetadata, UserSyncStateMetadataCache.CacheEntrySlidingLiveTimeEntry.Value);
				}
				return userSyncStateMetadata;
			}
			syncLogger.TraceDebug<SmtpAddress>(ExTraceGlobals.SyncProcessTracer, (long)this.GetHashCode(), "[UserSyncStateMetadataCache.Get] Cache hit for user: {0}", mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress);
			if (userSyncStateMetadata.MailboxGuid != mailboxSession.MailboxGuid)
			{
				throw new InvalidOperationException(string.Format("[UserSyncStateMetadataCache.Get] cached userState for mailbox {0} was keyed off incorrect mailbox {1}", userSyncStateMetadata.MailboxGuid, mailboxSession.MailboxGuid));
			}
			return userSyncStateMetadata;
		}

		public void Clear()
		{
			lock (this.instanceLock)
			{
				this.cache.Clear();
			}
		}

		private void HandleRemoveItem(Guid key, UserSyncStateMetadata value, RemoveReason removeReason)
		{
			ExTraceGlobals.SyncProcessTracer.TraceDebug<Guid, RemoveReason>((long)this.GetHashCode(), "[UserSyncStateMetadataCache.HandleRemoveItem] Mailbox {0} removed from cache due to reason: {1}", key, removeReason);
		}

		private object instanceLock = new object();

		private static readonly IntAppSettingsEntry CacheSizeLimitEntry = new IntAppSettingsEntry("UserSyncStateMetadataCache.CacheSizeLimit", 100000, ExTraceGlobals.SyncProcessTracer);

		private static readonly TimeSpanAppSettingsEntry CacheEntrySlidingLiveTimeEntry = new TimeSpanAppSettingsEntry("UserSyncStateMetadataCache.CacheEntrySlidingLiveTime", TimeSpanUnit.Hours, TimeSpan.FromHours(3.0), ExTraceGlobals.SyncProcessTracer);

		private static Lazy<UserSyncStateMetadataCache> singleton = new Lazy<UserSyncStateMetadataCache>(() => new UserSyncStateMetadataCache(), true);

		private static object staticLock = new object();

		private ExactTimeoutCache<Guid, UserSyncStateMetadata> cache;
	}
}
