using System;
using System.Security.Principal;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.InfoWorker.Common.Search;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class MailboxSessionCache : LazyLookupTimeoutCache<ExchangePrincipal, StoreSession>
	{
		internal event MailboxSessionCache.AddedToCacheDelegate OnAddedToCached;

		internal event MailboxSessionCache.RemovedFromCacheDelegate OnRemovedFromCache;

		public MailboxSessionCache(int cacheSize, GenericIdentity executingUserIdentity, Guid queryCorrelationId, TimeSpan cacheExpiryPeriod, MailboxSessionCache.CreateSessionHandler createSessionHandler = null) : base(1, cacheSize, true, cacheExpiryPeriod)
		{
			this.executingUserIdentity = executingUserIdentity;
			this.queryCorrelationId = queryCorrelationId;
			AppDomain.CurrentDomain.DomainUnload += this.HandleDomainUnload;
			AppDomain.CurrentDomain.ProcessExit += this.HandleProcessExit;
			if (createSessionHandler == null)
			{
				this.OnCreateSession = ((ExchangePrincipal mailboxIdentity) => SearchUtils.OpenSession(mailboxIdentity, this.ExecutingUserGenericIdentity, false));
				return;
			}
			this.OnCreateSession = createSessionHandler;
		}

		internal int ItemsInCache
		{
			get
			{
				return base.Count;
			}
		}

		internal GenericIdentity ExecutingUserGenericIdentity
		{
			get
			{
				return this.executingUserIdentity;
			}
		}

		protected override StoreSession CreateOnCacheMiss(ExchangePrincipal userPrincipal, ref bool shouldAdd)
		{
			shouldAdd = true;
			Factory.Current.LocalTaskTracer.TraceInformation<Guid, string>(this.GetHashCode(), 0L, "Correlation Id:{0}. MailboxSession for {1} not found in the SessionCache, creating one and adding it to the cache.", this.queryCorrelationId, userPrincipal.ToString());
			StoreSession result = this.OnCreateSession(userPrincipal);
			if (this.OnAddedToCached != null)
			{
				this.OnAddedToCached(userPrincipal);
			}
			return result;
		}

		protected override void HandleRemove(ExchangePrincipal key, StoreSession value, RemoveReason reason)
		{
			Factory.Current.LocalTaskTracer.TraceInformation<Guid, string, string>(this.GetHashCode(), 0L, "Correlation Id:{0}. Removing MailboxSession for mailbox:{1} from the cache, removal reason is {2}", this.queryCorrelationId, key.ToString(), reason.ToString());
			base.HandleRemove(key, value, reason);
			if (this.OnRemovedFromCache != null)
			{
				this.OnRemovedFromCache(key, reason);
			}
		}

		protected override void CleanupValue(ExchangePrincipal key, StoreSession value)
		{
			Factory.Current.LocalTaskTracer.TraceInformation<Guid, string>(this.GetHashCode(), 0L, "Correlation Id:{0}. Cleanup called for MailboxSession for mailbox:{1} from the cache.", this.queryCorrelationId, key.ToString());
			if (value != null)
			{
				value.Dispose();
				value = null;
			}
		}

		private void HandleDomainUnload(object sender, EventArgs e)
		{
			AppDomain.CurrentDomain.ProcessExit -= this.HandleProcessExit;
			this.Dispose();
		}

		private void HandleProcessExit(object sender, EventArgs e)
		{
			this.Dispose();
		}

		private readonly GenericIdentity executingUserIdentity;

		private readonly Guid queryCorrelationId;

		private MailboxSessionCache.CreateSessionHandler OnCreateSession;

		internal delegate void AddedToCacheDelegate(ExchangePrincipal mailboxIdentity);

		internal delegate void RemovedFromCacheDelegate(ExchangePrincipal mailboxIdentity, RemoveReason removeReason);

		internal delegate StoreSession CreateSessionHandler(ExchangePrincipal mailboxIdentity);
	}
}
