using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class AppWideStoreSessionCache : StoreSessionCacheBase, IDisposable
	{
		private static string BuildCacheKey(Guid accessingGuid, string originalCallerString, bool unifiedLogon)
		{
			string arg = unifiedLogon ? "_unified" : string.Empty;
			return string.Format("{0}_{1}{2}", accessingGuid, originalCallerString, arg);
		}

		public SessionAndAuthZ GetCachedMailboxSessionByGuid(Guid mailboxGuid, CallContext callContext, bool unifiedLogon)
		{
			if (callContext == null)
			{
				throw new ArgumentNullException("callContext");
			}
			this.CheckDisposed();
			SessionAndAuthZ sessionAndAuthZ = null;
			AccessingPrincipalTiedCache accessingPrincipalTiedCache = null;
			bool flag = false;
			string key = AppWideStoreSessionCache.BuildCacheKey(callContext.EffectiveCaller.ObjectGuid, callContext.OriginalCallerContext.IdentifierString, unifiedLogon);
			lock (this.cache)
			{
				if (!(flag = this.cache.TryGetValue(key, out accessingPrincipalTiedCache)))
				{
					accessingPrincipalTiedCache = new AccessingPrincipalTiedCache(callContext.EffectiveCaller.ObjectGuid);
					this.cache.Add(key, accessingPrincipalTiedCache);
				}
			}
			if (flag)
			{
				sessionAndAuthZ = accessingPrincipalTiedCache.GetFromCache(mailboxGuid, callContext.ClientCulture, callContext.LogonType);
				if (sessionAndAuthZ != null)
				{
					if (sessionAndAuthZ.ClientInfo != null && sessionAndAuthZ.ClientInfo.ClientSecurityContext != null)
					{
						ExTraceGlobals.SessionCacheTracer.TraceDebug<Guid, int, string>((long)this.GetHashCode(), "AppWideMailboxSessionCache.GetCachedMailboxSessionByGuid() Cache Hit! Cache Hit! MailboxGuid: {0} CacheSize: {1} Culture: {2}", mailboxGuid, accessingPrincipalTiedCache.Count, callContext.ClientCulture.Name);
						sessionAndAuthZ.IsFromBackingCache = true;
					}
					else
					{
						ExTraceGlobals.SessionCacheTracer.TraceDebug((long)this.GetHashCode(), "[AppWideMailboxSessionCache::GetCachedMailboxSessionByGuid] Found a SessionAndAuthZ in our cache, but the clientInfo has been disposed of.");
						sessionAndAuthZ.Dispose();
						sessionAndAuthZ = null;
					}
				}
			}
			if (sessionAndAuthZ == null)
			{
				ExchangePrincipal fromCacheByGuid = ExchangePrincipalCache.GetFromCacheByGuid(mailboxGuid, callContext.ADRecipientSessionContext);
				if (fromCacheByGuid == null)
				{
					throw new NonExistentMailboxException((CoreResources.IDs)3279543955U, mailboxGuid.ToString());
				}
				sessionAndAuthZ = base.CreateMailboxSessionBasedOnAccessType(fromCacheByGuid, callContext, unifiedLogon);
				ExTraceGlobals.SessionCacheTracer.TraceDebug<Guid>((long)this.GetHashCode(), "AppWideMailboxSessionCache.GetCachedMailboxSessionByGuid() Cache MISS.  Mailbox Guid {0}", mailboxGuid);
			}
			bool flag3 = false;
			try
			{
				base.SafeConnect(sessionAndAuthZ.Session);
				flag3 = true;
			}
			catch (WrongServerException ex)
			{
				ExTraceGlobals.SessionCacheTracer.TraceDebug<WrongServerException>((long)this.GetHashCode(), "AppWideMailboxSessionCache.GetCachedMailboxSessionByGuid() The EP might be stale given WrongServerException is hit: {0}", ex);
				base.HandleStaleExchangePrincipal(sessionAndAuthZ.Session.MailboxOwner, callContext);
				if (sessionAndAuthZ.IsFromBackingCache)
				{
					accessingPrincipalTiedCache.RemoveAllFromCache(mailboxGuid, callContext.ClientCulture, callContext.LogonType);
				}
				if (ex.MdbGuid != Guid.Empty && !string.IsNullOrEmpty(ex.RightServerFqdn))
				{
					callContext.HttpContext.Response.Headers["X-DBMountedOnServer"] = ex.RightServerToString();
					callContext.HttpContext.Response.Headers["X-BEServerException"] = typeof(IllegalCrossServerConnectionException).FullName;
				}
				else
				{
					callContext.HttpContext.Response.Headers["X-BEServerRoutingError"] = typeof(IllegalCrossServerConnectionException).FullName;
				}
				throw;
			}
			catch (Exception ex2)
			{
				if (ex2 is MailboxInSiteFailoverException || ex2 is MailboxCrossSiteFailoverException)
				{
					ExTraceGlobals.SessionCacheTracer.TraceDebug<Exception>((long)this.GetHashCode(), "AppWideMailboxSessionCache.GetCachedMailboxSessionByGuid() The EP might be stale given MailboxInSiteFailoverException is hit: {0}", ex2);
					if (Global.SendXBEServerExceptionHeaderToCafeOnFailover)
					{
						ExchangePrincipalCache.Remove(sessionAndAuthZ.Session.MailboxOwner);
						if (sessionAndAuthZ.IsFromBackingCache)
						{
							accessingPrincipalTiedCache.RemoveAllFromCache(mailboxGuid, callContext.ClientCulture, callContext.LogonType);
						}
						base.SendExceptionHeaderIfNeeded(callContext, mailboxGuid);
					}
				}
				throw;
			}
			finally
			{
				if (!flag3)
				{
					sessionAndAuthZ.Dispose();
					sessionAndAuthZ = null;
				}
			}
			if (sessionAndAuthZ.Session.AccountingObject != null)
			{
				sessionAndAuthZ.Dispose();
				sessionAndAuthZ = null;
				throw new InvalidOperationException("[AppWideMailboxSessionCache::GetCachedMailboxSessionByGuid] Encountered a session in the cache that has a Budget object already set on it.  Budgets MUST be cleared off of sessions before they are released back into the cache.");
			}
			accessingPrincipalTiedCache.AddToCheckedOutSessions(mailboxGuid, callContext.LogonType, sessionAndAuthZ);
			return sessionAndAuthZ;
		}

		private void CheckDisposed()
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		public void ReleaseMailboxSession(SessionAndAuthZ sessionAndAuthZ, CallContext callContext)
		{
			if (sessionAndAuthZ == null)
			{
				throw new ArgumentNullException("sessionAndAuthZ");
			}
			this.CheckDisposed();
			if (sessionAndAuthZ.ClientInfo == null)
			{
				throw new ArgumentException("[AppWideMailboxSessionCache::ReleaseMailboxSession] ClientInfo is null.  This suggests a ref count issue.");
			}
			if (sessionAndAuthZ.Session.AccountingObject != null)
			{
				throw new ArgumentException("[AppWideMailboxSessionCache::ReleaseMailboxSession] You cannot add sessions back to the app wide cache that have Budget objects still set on them.  You MUST clear the budget object first.", "sessionAndAuthZ");
			}
			if (sessionAndAuthZ.ClientInfo.ObjectGuid == Guid.Empty)
			{
				ExTraceGlobals.SessionCacheTracer.TraceDebug<string>((long)this.GetHashCode(), "[AppWideMailboxSessionCache::ReleaseMailboxSession] The object Guid for the caller was Guid.Empty which implies that this is a cross forest caller. Therefore we cannot add their mailbox sessions to the cache.  Caller Sid: {0}", sessionAndAuthZ.ClientInfo.ClientSecurityContext.UserSid.ToString());
				sessionAndAuthZ.Dispose();
				return;
			}
			AccessingPrincipalTiedCache accessingPrincipalTiedCache = null;
			MailboxSession mailboxSession = sessionAndAuthZ.Session as MailboxSession;
			string key = AppWideStoreSessionCache.BuildCacheKey(sessionAndAuthZ.ClientInfo.ObjectGuid, callContext.OriginalCallerContext.IdentifierString, mailboxSession != null && mailboxSession.IsUnified);
			lock (this.cache)
			{
				if (!this.cache.TryGetValue(key, out accessingPrincipalTiedCache))
				{
					accessingPrincipalTiedCache = new AccessingPrincipalTiedCache(sessionAndAuthZ.ClientInfo.ObjectGuid);
					this.cache.Add(key, accessingPrincipalTiedCache);
				}
			}
			sessionAndAuthZ.Session.ExTimeZone = ExTimeZone.UtcTimeZone;
			try
			{
				ExTraceGlobals.SessionCacheTracer.TraceDebug((long)this.GetHashCode(), "[AppWideMailboxSessionCache::ReleaseMailboxSession] Disconnecting session.");
			}
			finally
			{
				if (sessionAndAuthZ.Session.IsConnected)
				{
					sessionAndAuthZ.Session.Disconnect();
				}
			}
			accessingPrincipalTiedCache.RemoveFromCheckedOutSessions(sessionAndAuthZ.Session.MailboxGuid, callContext.LogonType, sessionAndAuthZ);
			accessingPrincipalTiedCache.AddToCache(sessionAndAuthZ, callContext.GetSessionCulture(sessionAndAuthZ.Session));
		}

		protected override MailboxSession GetMailboxSessionToClone(CallContext callContext, Guid mailboxOwner, CultureInfo cultureInfo, LogonType logonType, bool unifiedLogon)
		{
			string key = AppWideStoreSessionCache.BuildCacheKey(callContext.EffectiveCaller.ObjectGuid, callContext.OriginalCallerContext.IdentifierString, unifiedLogon);
			AccessingPrincipalTiedCache accessingPrincipalTiedCache;
			lock (this.cache)
			{
				this.cache.TryGetValue(key, out accessingPrincipalTiedCache);
			}
			if (accessingPrincipalTiedCache == null)
			{
				return null;
			}
			SessionAndAuthZ fromCheckedOutSessionsForCloning = accessingPrincipalTiedCache.GetFromCheckedOutSessionsForCloning(mailboxOwner, cultureInfo, logonType);
			if (fromCheckedOutSessionsForCloning != null)
			{
				return fromCheckedOutSessionsForCloning.Session as MailboxSession;
			}
			return null;
		}

		protected sealed override void Dispose(bool isDisposing)
		{
			ExTraceGlobals.SessionCacheTracer.TraceDebug<int, bool>((long)this.GetHashCode(), "AppWideMailboxSessionCache.Dispose(). Hashcode: {0}. IsDisposing: {1}", this.GetHashCode(), isDisposing);
			if (!this.isDisposed)
			{
				foreach (KeyValuePair<string, AccessingPrincipalTiedCache> keyValuePair in this.cache)
				{
					keyValuePair.Value.Dispose();
				}
				this.isDisposed = true;
			}
		}

		private Dictionary<string, AccessingPrincipalTiedCache> cache = new Dictionary<string, AccessingPrincipalTiedCache>();
	}
}
