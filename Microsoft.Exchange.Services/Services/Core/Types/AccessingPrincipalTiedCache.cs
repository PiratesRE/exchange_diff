using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Caching;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class AccessingPrincipalTiedCache : IDisposable
	{
		public AccessingPrincipalTiedCache(Guid principalObjectGuid)
		{
			this.principalObjectGuid = principalObjectGuid;
		}

		public int Count
		{
			get
			{
				int count;
				lock (this.keyCache)
				{
					count = this.keyCache.Count;
				}
				return count;
			}
		}

		public static string BuildKeyCacheKey(Guid mailboxGuid, CultureInfo culture, LogonType logonType)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}_{1}{2}", new object[]
			{
				mailboxGuid,
				culture.Name,
				AccessingPrincipalTiedCache.GetLogonTypeSuffix(logonType)
			});
		}

		private static string BuildWebCacheKey(StoreSession storeSession, CultureInfo cultureInfo)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}_{1}_{2}{3}", new object[]
			{
				storeSession.MailboxOwner.MailboxInfo.MailboxGuid,
				storeSession.GetHashCode(),
				cultureInfo.Name,
				AccessingPrincipalTiedCache.GetLogonTypeSuffix(storeSession.LogonType)
			});
		}

		private static string GetLogonTypeSuffix(LogonType logonType)
		{
			string result = string.Empty;
			if (logonType == LogonType.Admin)
			{
				result = "_a";
			}
			else if (logonType == LogonType.SystemService)
			{
				result = "_s";
			}
			return result;
		}

		public void AddToCheckedOutSessions(Guid mailboxGuid, LogonType logonType, SessionAndAuthZ session)
		{
			if (session == null)
			{
				return;
			}
			string key = AccessingPrincipalTiedCache.BuildKeyCacheKey(mailboxGuid, session.CultureInfo, logonType);
			lock (this.checkoutSessions)
			{
				LinkedList<WeakReference<SessionAndAuthZ>> linkedList;
				if (!this.checkoutSessions.TryGetValue(key, out linkedList))
				{
					linkedList = new LinkedList<WeakReference<SessionAndAuthZ>>();
					this.checkoutSessions.Add(key, linkedList);
				}
				linkedList.AddLast(new WeakReference<SessionAndAuthZ>(session));
			}
		}

		public void RemoveFromCheckedOutSessions(Guid mailboxGuid, LogonType logonType, SessionAndAuthZ session)
		{
			string key = AccessingPrincipalTiedCache.BuildKeyCacheKey(mailboxGuid, session.CultureInfo, logonType);
			lock (this.checkoutSessions)
			{
				LinkedList<WeakReference<SessionAndAuthZ>> linkedList;
				if (this.checkoutSessions.TryGetValue(key, out linkedList))
				{
					LinkedListNode<WeakReference<SessionAndAuthZ>> next;
					for (LinkedListNode<WeakReference<SessionAndAuthZ>> linkedListNode = linkedList.First; linkedListNode != null; linkedListNode = next)
					{
						next = linkedListNode.Next;
						SessionAndAuthZ sessionAndAuthZ;
						if (!linkedListNode.Value.TryGetTarget(out sessionAndAuthZ) || sessionAndAuthZ == session)
						{
							linkedList.Remove(linkedListNode);
						}
						if (sessionAndAuthZ == session)
						{
							break;
						}
					}
				}
			}
		}

		public SessionAndAuthZ GetFromCheckedOutSessionsForCloning(Guid mailboxGuid, CultureInfo cultureInfo, LogonType logonType)
		{
			string key = AccessingPrincipalTiedCache.BuildKeyCacheKey(mailboxGuid, cultureInfo, logonType);
			lock (this.checkoutSessions)
			{
				LinkedList<WeakReference<SessionAndAuthZ>> linkedList;
				if (this.checkoutSessions.TryGetValue(key, out linkedList))
				{
					LinkedListNode<WeakReference<SessionAndAuthZ>> next;
					for (LinkedListNode<WeakReference<SessionAndAuthZ>> linkedListNode = linkedList.First; linkedListNode != null; linkedListNode = next)
					{
						next = linkedListNode.Next;
						SessionAndAuthZ result;
						if (linkedListNode.Value.TryGetTarget(out result))
						{
							return result;
						}
						linkedList.Remove(linkedListNode);
					}
				}
			}
			return null;
		}

		public SessionAndAuthZ GetFromCache(Guid mailboxGuid, CultureInfo culture, LogonType logonType)
		{
			this.CheckDisposed();
			SessionAndAuthZ sessionAndAuthZ = null;
			string key = AccessingPrincipalTiedCache.BuildKeyCacheKey(mailboxGuid, culture, logonType);
			lock (this.keyCache)
			{
				Cache cache = HttpRuntime.Cache;
				List<string> list = null;
				if (this.keyCache.TryGetValue(key, out list))
				{
					while (list.Count > 0)
					{
						string key2 = list[0];
						list.RemoveAt(0);
						this.cachedMailboxSessionCount--;
						sessionAndAuthZ = (SessionAndAuthZ)cache.Remove(key2);
						if (sessionAndAuthZ != null)
						{
							break;
						}
					}
				}
			}
			if (sessionAndAuthZ != null)
			{
				try
				{
					bool isConnected = sessionAndAuthZ.Session.IsConnected;
				}
				catch (ObjectDisposedException)
				{
					ExTraceGlobals.SessionCacheTracer.TraceDebug<int>((long)this.GetHashCode(), "AccessingPrincipalTiedCache.GetFromCache(). Hashcode: {0}. A mailbox session from the cache was already disposed.", this.GetHashCode());
					sessionAndAuthZ.Dispose();
					sessionAndAuthZ = null;
				}
			}
			return sessionAndAuthZ;
		}

		public void RemoveAllFromCache(Guid mailboxGuid, CultureInfo culture, LogonType logonType)
		{
			this.CheckDisposed();
			string key = AccessingPrincipalTiedCache.BuildKeyCacheKey(mailboxGuid, culture, logonType);
			List<string> list = null;
			lock (this.keyCache)
			{
				if (this.keyCache.TryGetValue(key, out list))
				{
					this.cachedMailboxSessionCount -= list.Count;
					this.keyCache.Remove(key);
				}
			}
			if (list != null)
			{
				Cache cache = HttpRuntime.Cache;
				foreach (string key2 in list)
				{
					SessionAndAuthZ sessionAndAuthZ = (SessionAndAuthZ)cache.Remove(key2);
					if (sessionAndAuthZ != null)
					{
						sessionAndAuthZ.Dispose();
					}
				}
			}
		}

		public void AddToCache(SessionAndAuthZ sessionAndAuthZ, CultureInfo cultureInfo)
		{
			if (sessionAndAuthZ == null)
			{
				throw new ArgumentException("[AccessingPrincipalTiedCache::AddToCache] sessionAndAuthZ is null");
			}
			if (sessionAndAuthZ.ClientInfo == null)
			{
				throw new ArgumentException("[AccessingPrincipalTiedCache::AddToCache] Session being added to cache has null ClientInfo.");
			}
			this.CheckDisposed();
			Cache cache = HttpRuntime.Cache;
			string text = AccessingPrincipalTiedCache.BuildWebCacheKey(sessionAndAuthZ.Session, cultureInfo);
			lock (this.keyCache)
			{
				if (this.cachedMailboxSessionCount >= Global.AccessingPrincipalCacheSize)
				{
					ExTraceGlobals.SessionCacheTracer.TraceDebug<Guid>((long)this.GetHashCode(), "Web cache for accessing principal is full. Flushing cache. Principal Guid: {0}", this.principalObjectGuid);
					this.ClearCache();
				}
				if ((SessionAndAuthZ)cache.Get(text) == null)
				{
					cache.Add(text, sessionAndAuthZ, null, ExDateTime.Now.AddMinutes(5.0).UniversalTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, new CacheItemRemovedCallback(this.RemovedCallback));
					List<string> list = null;
					StoreSession session = sessionAndAuthZ.Session;
					string key = AccessingPrincipalTiedCache.BuildKeyCacheKey(session.MailboxOwner.MailboxInfo.MailboxGuid, cultureInfo, session.LogonType);
					if (!this.keyCache.TryGetValue(key, out list))
					{
						list = new List<string>();
						this.keyCache.Add(key, list);
					}
					this.cachedMailboxSessionCount++;
					list.Add(text);
					ExTraceGlobals.SessionCacheTracer.TraceDebug((long)this.GetHashCode(), "Added StoreSession instance to the web cache.  HashCode: {0}, Principal Guid: {1}, SmtpAddress: {2}, CacheKey: {3}, Count: {4}", new object[]
					{
						session.GetHashCode(),
						this.principalObjectGuid,
						session.MailboxOwner.MailboxInfo.PrimarySmtpAddress,
						text,
						this.keyCache.Count
					});
				}
				else
				{
					StoreSession session2 = sessionAndAuthZ.Session;
					ExTraceGlobals.SessionCacheTracer.TraceDebug<Guid, SmtpAddress>((long)this.GetHashCode(), "An attempt was made to add a duplicate mailbox instance to the cache.  Ignoring. Principal Guid: {0}, SmtpAddress: {1}", this.principalObjectGuid, session2.MailboxOwner.MailboxInfo.PrimarySmtpAddress);
				}
				if (sessionAndAuthZ.Session.IsConnected)
				{
					sessionAndAuthZ.Session.Disconnect();
				}
			}
		}

		private void ClearCache()
		{
			ExTraceGlobals.SessionCacheTracer.TraceDebug((long)this.GetHashCode(), "ClearCache method called.");
			lock (this.keyCache)
			{
				foreach (KeyValuePair<string, List<string>> keyValuePair in this.keyCache)
				{
					for (int i = 0; i < keyValuePair.Value.Count; i++)
					{
						string key = keyValuePair.Value[i];
						SessionAndAuthZ sessionAndAuthZ = (SessionAndAuthZ)HttpRuntime.Cache.Remove(key);
						if (sessionAndAuthZ != null)
						{
							sessionAndAuthZ.Dispose();
						}
					}
				}
				this.keyCache.Clear();
				this.cachedMailboxSessionCount = 0;
			}
		}

		private void RemovedCallback(string key, object mailboxObject, CacheItemRemovedReason reason)
		{
			if (reason == CacheItemRemovedReason.Removed)
			{
				return;
			}
			if (this.isDisposed)
			{
				return;
			}
			SessionAndAuthZ sessionAndAuthZ = (SessionAndAuthZ)mailboxObject;
			StoreSession session = sessionAndAuthZ.Session;
			lock (this.keyCache)
			{
				List<string> list = null;
				string key2 = AccessingPrincipalTiedCache.BuildKeyCacheKey(session.MailboxOwner.MailboxInfo.MailboxGuid, sessionAndAuthZ.CultureInfo, session.LogonType);
				if (this.keyCache.TryGetValue(key2, out list))
				{
					int num = list.IndexOf(key);
					if (num <= -1)
					{
						return;
					}
					list.RemoveAt(num);
					this.cachedMailboxSessionCount--;
				}
			}
			ExTraceGlobals.SessionCacheTracer.TraceDebug<string, Guid, string>((long)this.GetHashCode(), "RemoveCallback delegate called by web cache.  Key: {0}, Principal Guid: {1}, MailboxHashCode: {2}", key, this.principalObjectGuid, (session == null) ? "<NULL>" : session.GetHashCode().ToString(CultureInfo.InvariantCulture));
			sessionAndAuthZ.Dispose();
		}

		public void Dispose()
		{
			ExTraceGlobals.SessionCacheTracer.TraceDebug<int>((long)this.GetHashCode(), "MailboxSessionTiedCache.Dispose(). Hashcode: {0}", this.GetHashCode());
			if (!this.isDisposed)
			{
				this.ClearCache();
				this.isDisposed = true;
			}
		}

		private void CheckDisposed()
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private const int CacheHoldTimeMinutes = 5;

		private Guid principalObjectGuid;

		private Dictionary<string, List<string>> keyCache = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

		private Dictionary<string, LinkedList<WeakReference<SessionAndAuthZ>>> checkoutSessions = new Dictionary<string, LinkedList<WeakReference<SessionAndAuthZ>>>();

		private bool isDisposed;

		private int cachedMailboxSessionCount;
	}
}
