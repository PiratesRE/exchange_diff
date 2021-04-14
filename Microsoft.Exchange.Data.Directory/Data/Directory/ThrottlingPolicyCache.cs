using System;
using System.Threading;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.IsMemberOfProvider;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.Data.Directory
{
	internal class ThrottlingPolicyCache
	{
		protected ThrottlingPolicyCache()
		{
			this.globalThrottlingPolicyCache = new ThrottlingPolicyCache.GlobalThrottlingPolicyCache(ThrottlingPolicyCache.cacheExpirationInterval);
			this.organizationThrottlingPolicies = new AutoRefreshCache<OrganizationId, CachableThrottlingPolicyItem, object>(10000L, ThrottlingPolicyCache.cacheExpirationInterval, ThrottlingPolicyCache.cacheCleanupInterval, ThrottlingPolicyCache.cachePurgeInterval, ThrottlingPolicyCache.cacheRefreshInterval, new DefaultCacheTracer<OrganizationId>(ThrottlingPolicyCache.Tracer, "OrganizationThrottlingPolicies"), ThrottlingPerfCounterWrapper.GetOrganizationThrottlingPolicyCacheCounters(10000L), new AutoRefreshCache<OrganizationId, CachableThrottlingPolicyItem, object>.CreateEntryDelegate(ThrottlingPolicyCache.ResolveOrganizationThrottlingPolicy));
			this.throttlingPolicies = new AutoRefreshCache<OrgAndObjectId, CachableThrottlingPolicyItem, object>(10000L, ThrottlingPolicyCache.cacheExpirationInterval, ThrottlingPolicyCache.cacheCleanupInterval, ThrottlingPolicyCache.cachePurgeInterval, ThrottlingPolicyCache.cacheRefreshInterval, new DefaultCacheTracer<OrgAndObjectId>(ThrottlingPolicyCache.Tracer, "ThrottlingPolicies"), ThrottlingPerfCounterWrapper.GetThrottlingPolicyCacheCounters(10000L), new AutoRefreshCache<OrgAndObjectId, CachableThrottlingPolicyItem, object>.CreateEntryDelegate(ThrottlingPolicyCache.ResolveThrottlingPolicy));
		}

		internal static ThrottlingPolicy ReadThrottlingPolicyFromAD(IConfigurationSession session, object id, Func<IConfigurationSession, object, ThrottlingPolicy> getPolicy)
		{
			ThrottlingPolicy policy = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				session.SessionSettings.IsSharedConfigChecked = true;
				policy = getPolicy(session, id);
			});
			if (!adoperationResult.Succeeded && adoperationResult.Exception != null)
			{
				ThrottlingPolicyCache.Tracer.TraceError<string, string, object>(0L, "Encountered exception reading throttling policy.  Exception Class: {0}, Message: {1}, Key: {2}", adoperationResult.Exception.GetType().FullName, adoperationResult.Exception.Message, id);
			}
			if (policy != null)
			{
				ValidationError[] array = policy.Validate();
				if (array != null && array.Length > 0)
				{
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_FailedToReadThrottlingPolicy, id.ToString(), new object[]
					{
						id,
						array[0].Description
					});
					policy = null;
				}
			}
			return policy;
		}

		internal static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ClientThrottlingTracer;
			}
		}

		public int OrganizationThrottlingPolicyCount
		{
			get
			{
				return this.organizationThrottlingPolicies.Count;
			}
		}

		public int ThrottlingPolicyCount
		{
			get
			{
				return this.throttlingPolicies.Count;
			}
		}

		public IThrottlingPolicy GetGlobalThrottlingPolicy()
		{
			this.BeforeGet();
			return this.globalThrottlingPolicyCache.Get();
		}

		public virtual IThrottlingPolicy Get(OrganizationId organizationId)
		{
			this.BeforeGet();
			return this.organizationThrottlingPolicies.GetValue(null, organizationId).ThrottlingPolicy;
		}

		public virtual IThrottlingPolicy Get(OrganizationId orgId, ADObjectId throttlingPolicyId)
		{
			if (throttlingPolicyId == null)
			{
				return this.Get(orgId);
			}
			return this.Get(new OrgAndObjectId(orgId, throttlingPolicyId));
		}

		public virtual IThrottlingPolicy Get(OrgAndObjectId orgAndObjectId)
		{
			this.BeforeGet();
			return this.throttlingPolicies.GetValue(null, orgAndObjectId).ThrottlingPolicy;
		}

		public void Remove(OrganizationId organizationId)
		{
			this.organizationThrottlingPolicies.Remove(organizationId);
		}

		public void Clear()
		{
			this.globalThrottlingPolicyCache.Clear();
			this.organizationThrottlingPolicies.Clear();
			this.throttlingPolicies.Clear();
		}

		private static IConfigurationSession GetSession(OrganizationId organizationId)
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(organizationId), 371, "GetSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\throttling\\throttlingpolicycache.cs");
		}

		private static CachableThrottlingPolicyItem ResolveOrganizationThrottlingPolicy(object obj, OrganizationId organizationId)
		{
			SharedConfiguration sharedConfiguration = SharedConfiguration.GetSharedConfiguration(organizationId);
			if (sharedConfiguration != null)
			{
				organizationId = sharedConfiguration.SharedConfigId;
			}
			ThrottlingPolicy throttlingPolicy = ThrottlingPolicyCache.ReadThrottlingPolicyFromAD(ThrottlingPolicyCache.GetSession(organizationId), organizationId, (IConfigurationSession session1, object id) => session1.GetOrganizationThrottlingPolicy((OrganizationId)id));
			return new CachableThrottlingPolicyItem((throttlingPolicy == null) ? ThrottlingPolicyCache.Singleton.GetGlobalThrottlingPolicy() : throttlingPolicy.GetEffectiveThrottlingPolicy(true));
		}

		private static CachableThrottlingPolicyItem ResolveThrottlingPolicy(object obj, OrgAndObjectId orgAndObjectId)
		{
			ThrottlingPolicy throttlingPolicy = null;
			if (orgAndObjectId.Id.IsDeleted)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_DeletedThrottlingPolicyReferenced, orgAndObjectId.ToString(), new object[]
				{
					orgAndObjectId
				});
			}
			else
			{
				throttlingPolicy = ThrottlingPolicyCache.ReadThrottlingPolicyFromAD(ThrottlingPolicyCache.GetSession(orgAndObjectId.OrganizationId), orgAndObjectId.Id, (IConfigurationSession session1, object id) => session1.Read<ThrottlingPolicy>((ADObjectId)id));
			}
			return new CachableThrottlingPolicyItem((throttlingPolicy == null) ? ThrottlingPolicyCache.Singleton.Get(orgAndObjectId.OrganizationId) : throttlingPolicy.GetEffectiveThrottlingPolicy(true));
		}

		private void BeforeGet()
		{
			int num = 0;
			ExTraceGlobals.FaultInjectionTracer.TraceTest<int>(2663787837U, ref num);
			if (num != 0 && num != this.lastClearCacheStamp)
			{
				this.lastClearCacheStamp = num;
				this.Clear();
			}
		}

		private const int MaxCacheSize = 10000;

		private const uint LidClearThrottlingCaches = 2663787837U;

		private static readonly TimeSpan cacheExpirationInterval = TimeSpan.FromMinutes(5.0);

		private static readonly TimeSpan cacheCleanupInterval = TimeSpan.FromMinutes(10.0);

		private static readonly TimeSpan cachePurgeInterval = TimeSpan.FromMinutes(1.0);

		private static readonly TimeSpan cacheRefreshInterval = TimeSpan.FromMinutes(1.0);

		public static readonly ThrottlingPolicyCache Singleton = new ThrottlingPolicyCache();

		private int lastClearCacheStamp;

		private ThrottlingPolicyCache.GlobalThrottlingPolicyCache globalThrottlingPolicyCache;

		private AutoRefreshCache<OrganizationId, CachableThrottlingPolicyItem, object> organizationThrottlingPolicies;

		private AutoRefreshCache<OrgAndObjectId, CachableThrottlingPolicyItem, object> throttlingPolicies;

		private class GlobalThrottlingPolicyCache : IDisposable
		{
			internal GlobalThrottlingPolicyCache(TimeSpan refreshInterval)
			{
				this.session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 481, ".ctor", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\throttling\\throttlingpolicycache.cs");
				this.refreshTimer = new GuardedTimer(new TimerCallback(this.Refresh), null, refreshInterval);
			}

			internal IThrottlingPolicy Get()
			{
				if (!this.disposed && this.policy == null)
				{
					this.Refresh(null);
				}
				return this.policy;
			}

			internal void Clear()
			{
				lock (this.objLock)
				{
					this.policy = null;
				}
			}

			public void Dispose()
			{
				this.refreshTimer.Dispose(true);
				this.disposed = true;
			}

			private IThrottlingPolicy ResolveGlobalThrottlingPolicy()
			{
				ThrottlingPolicy throttlingPolicy = null;
				if (!this.disposed)
				{
					throttlingPolicy = ThrottlingPolicyCache.ReadThrottlingPolicyFromAD(this.session, null, (IConfigurationSession session1, object id) => ((ITopologyConfigurationSession)session1).GetGlobalThrottlingPolicy());
				}
				if (throttlingPolicy != null)
				{
					return throttlingPolicy.GetEffectiveThrottlingPolicy(false);
				}
				return FallbackThrottlingPolicy.GetSingleton();
			}

			private void Refresh(object unused)
			{
				IThrottlingPolicy throttlingPolicy = this.ResolveGlobalThrottlingPolicy();
				lock (this.objLock)
				{
					this.policy = throttlingPolicy;
				}
			}

			private GuardedTimer refreshTimer;

			private IThrottlingPolicy policy;

			private object objLock = new object();

			private ITopologyConfigurationSession session;

			private bool disposed;
		}
	}
}
