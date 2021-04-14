using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ProvisioningAgent;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal sealed class MailboxSessionCache : Disposable
	{
		internal MailboxSessionCache(string instanceName, int maxNumberOfMailboxSessions, int maxNumberOfMailboxSessionsPerMailbox, TimeSpan cacheTimeout)
		{
			this.maxNumberOfMailboxSessionsPerMailbox = maxNumberOfMailboxSessionsPerMailbox;
			this.cacheTimeout = cacheTimeout;
			this.mailboxSessionCache = new TimeoutCache<long, MailboxSession>(1, maxNumberOfMailboxSessions, true);
		}

		internal Dictionary<MailboxSessionCacheKey, List<long>> InnerCacheLookup
		{
			get
			{
				return this.cacheLookup;
			}
		}

		internal TimeoutCache<long, MailboxSession> InnerTimeoutCache
		{
			get
			{
				return this.mailboxSessionCache;
			}
		}

		internal MailboxSession GetMailboxSession(MailboxSessionCacheKey cacheKey, ExchangePrincipal principal)
		{
			base.CheckDisposed();
			MailboxSession mailboxSession = null;
			bool flag = false;
			MailboxSessionCache.Tracer.TraceDebug<MailboxSessionCache, MailboxSessionCacheKey>((long)this.GetHashCode(), "{0} Getting mailbox session for {1}", this, cacheKey);
			try
			{
				lock (this.locker)
				{
					List<long> list;
					if (this.cacheLookup.TryGetValue(cacheKey, out list))
					{
						MailboxSessionCache.Tracer.TraceDebug<MailboxSessionCacheKey, int>((long)this.GetHashCode(), "Found lookup cache for {0}: {1} mailbox session caches", cacheKey, list.Count);
						long num = list[0];
						list.RemoveAt(0);
						if (list.Count == 0)
						{
							MailboxSessionCache.Tracer.TraceDebug<MailboxSessionCache, MailboxSessionCacheKey>((long)this.GetHashCode(), "{0} Remove the lookup cache for {1}", this, cacheKey);
							this.cacheLookup.Remove(cacheKey);
						}
						MailboxSessionCache.Tracer.TraceDebug<long, MailboxSessionCacheKey>((long)this.GetHashCode(), "Checking out the mailbox session cache {0} for {1}", num, cacheKey);
						mailboxSession = this.mailboxSessionCache.Remove(num);
					}
				}
				if (mailboxSession == null)
				{
					MailboxSessionCache.Tracer.TraceDebug<MailboxSessionCacheKey>((long)this.GetHashCode(), "Create a new mailbox session since no mailbox session cache found for {0}", cacheKey);
					mailboxSession = MailboxSessionManager.CreateMailboxSession(principal);
				}
				else if (!mailboxSession.IsConnected)
				{
					mailboxSession.ConnectWithStatus();
				}
				flag = true;
			}
			finally
			{
				if (mailboxSession != null && !flag)
				{
					mailboxSession.Dispose();
					mailboxSession = null;
				}
			}
			return mailboxSession;
		}

		internal void ReturnMailboxSession(ref MailboxSession mailboxSession)
		{
			base.CheckDisposed();
			bool flag = false;
			MailboxSessionCacheKey mailboxSessionCacheKey = new MailboxSessionCacheKey(mailboxSession.MailboxOwner);
			MailboxSessionCache.Tracer.TraceDebug<MailboxSessionCacheKey>((long)this.GetHashCode(), "Returning mailbox session for {0}", mailboxSessionCacheKey);
			try
			{
				lock (this.locker)
				{
					List<long> list;
					if (!this.cacheLookup.TryGetValue(mailboxSessionCacheKey, out list))
					{
						MailboxSessionCache.Tracer.TraceDebug<MailboxSessionCacheKey>((long)this.GetHashCode(), "Adding lookup cache for {0}", mailboxSessionCacheKey);
						list = new List<long>(this.maxNumberOfMailboxSessionsPerMailbox);
						this.cacheLookup.Add(mailboxSessionCacheKey, list);
					}
					MailboxSessionCache.Tracer.TraceDebug<MailboxSessionCacheKey, int>((long)this.GetHashCode(), "The lookup cache for {0}: {1} mailbox session caches", mailboxSessionCacheKey, list.Count);
					if (list.Count < this.maxNumberOfMailboxSessionsPerMailbox)
					{
						long num = Interlocked.Increment(ref MailboxSessionCache.globalCacheId);
						MailboxSessionCache.Tracer.TraceDebug<long, MailboxSessionCacheKey>((long)this.GetHashCode(), "Checking in mailbox session cache {0} back for {1}", num, mailboxSessionCacheKey);
						this.mailboxSessionCache.AddSliding(num, mailboxSession, this.cacheTimeout, new RemoveItemDelegate<long, MailboxSession>(this.RemoveFromCacheCallback));
						flag = true;
						list.Add(num);
					}
				}
			}
			finally
			{
				if (!flag)
				{
					mailboxSession.Dispose();
				}
				mailboxSession = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxSessionCache>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				lock (this.locker)
				{
					this.mailboxSessionCache.Dispose();
					this.cacheLookup.Clear();
				}
			}
		}

		private void RemoveFromCacheCallback(long cacheId, MailboxSession mailboxSession, RemoveReason reason)
		{
			if (reason == RemoveReason.Removed)
			{
				return;
			}
			try
			{
				MailboxSessionCacheKey mailboxSessionCacheKey = new MailboxSessionCacheKey(mailboxSession.MdbGuid, mailboxSession.MailboxGuid);
				MailboxSessionCache.Tracer.TraceDebug<MailboxSessionCacheKey, long, RemoveReason>((long)this.GetHashCode(), "Mailbox session for {0} is removed from cache {1} due to reason: {2}", mailboxSessionCacheKey, cacheId, reason);
				lock (this.locker)
				{
					List<long> list;
					if (this.cacheLookup.TryGetValue(mailboxSessionCacheKey, out list))
					{
						MailboxSessionCache.Tracer.TraceDebug<MailboxSessionCacheKey, int>((long)this.GetHashCode(), "Removing from lookup cache for {0}: {1} mailbox session caches", mailboxSessionCacheKey, list.Count);
						list.Remove(cacheId);
						if (list.Count == 0)
						{
							MailboxSessionCache.Tracer.TraceDebug<MailboxSessionCacheKey>((long)this.GetHashCode(), "Remove the lookup cache for {0}", mailboxSessionCacheKey);
							this.cacheLookup.Remove(mailboxSessionCacheKey);
						}
					}
				}
			}
			finally
			{
				mailboxSession.Dispose();
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.AdminAuditLogTracer;

		private readonly int maxNumberOfMailboxSessionsPerMailbox;

		private readonly TimeSpan cacheTimeout;

		private static long globalCacheId = 0L;

		private TimeoutCache<long, MailboxSession> mailboxSessionCache;

		private Dictionary<MailboxSessionCacheKey, List<long>> cacheLookup = new Dictionary<MailboxSessionCacheKey, List<long>>();

		private object locker = new object();
	}
}
