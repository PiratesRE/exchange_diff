using System;
using System.Diagnostics;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Mdb
{
	internal sealed class StoreSessionManager
	{
		private StoreSessionManager()
		{
			SearchConfig searchConfig = SearchConfig.Instance;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				this.storeSessionCache = new StoreSessionCache(currentProcess.ProcessName, searchConfig.MaxNumberOfMailboxSessions, searchConfig.MaxNumberOfMailboxSessionsPerMailbox, TimeSpan.FromMinutes((double)searchConfig.CacheTimeoutMinutes));
			}
		}

		internal static StoreSessionCache InnerStoreSessionCache
		{
			get
			{
				return StoreSessionManager.instance.storeSessionCache;
			}
		}

		internal static StoreSession GetStoreSessionFromCache(MdbItemIdentity identity, bool localOnly)
		{
			return StoreSessionManager.GetStoreSessionFromCache(identity, false, false, localOnly);
		}

		internal static StoreSession GetWritableStoreSessionFromCache(MdbItemIdentity identity, bool isMoveDestination, bool wantSessionForRMS)
		{
			StoreSessionCacheKey cacheKey = new StoreSessionCacheKey(identity.GetMdbGuid(MdbItemIdentity.Location.FastCatalog), identity.MailboxGuid, isMoveDestination);
			return StoreSessionManager.instance.storeSessionCache.GetWritableStoreSession(cacheKey, identity, wantSessionForRMS);
		}

		internal static StoreSession GetStoreSessionFromCache(MdbItemIdentity identity, bool isMoveDestination, bool wantSessionForRMS, bool localOnly)
		{
			StoreSessionCacheKey cacheKey = new StoreSessionCacheKey(identity.GetMdbGuid(MdbItemIdentity.Location.FastCatalog), identity.MailboxGuid, isMoveDestination);
			return StoreSessionManager.instance.storeSessionCache.GetStoreSession(cacheKey, identity, wantSessionForRMS, localOnly);
		}

		internal static void ReturnStoreSessionToCache(ref StoreSession storeSession, bool discard)
		{
			if (storeSession == null)
			{
				return;
			}
			if (!discard)
			{
				StoreSessionManager.instance.storeSessionCache.ReturnStoreSession(ref storeSession);
				return;
			}
			storeSession.Dispose();
			storeSession = null;
		}

		internal static bool IsSessionUsableForRMS(StoreSession storeSession)
		{
			return !string.IsNullOrEmpty(storeSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString());
		}

		private static readonly StoreSessionManager instance = new StoreSessionManager();

		private readonly StoreSessionCache storeSessionCache;
	}
}
