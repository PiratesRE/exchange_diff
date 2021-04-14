using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Configuration;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class MobileDevicePolicyCache : LazyLookupExactTimeoutCache<OrgIdADObjectWrapper, MobileDevicePolicyData>
	{
		protected MobileDevicePolicyCache(int maxCount, TimeSpan slidingLiveTime, TimeSpan absoluteLiveTime) : base(maxCount, false, slidingLiveTime, absoluteLiveTime, CacheFullBehavior.ExpireExisting)
		{
		}

		protected MobileDevicePolicyCache() : this(5000, TimeSpan.FromMinutes(15.0), TimeSpan.FromMinutes(60.0))
		{
		}

		internal static MobileDevicePolicyCache Instance
		{
			get
			{
				return MobileDevicePolicyCache.instance;
			}
		}

		protected override MobileDevicePolicyData CreateOnCacheMiss(OrgIdADObjectWrapper key, ref bool shouldAdd)
		{
			MobileDevicePolicyData policyFromAD = this.GetPolicyFromAD(key);
			shouldAdd = (policyFromAD != null);
			return policyFromAD;
		}

		protected virtual IConfigurationSession GetConfigSession(ADSessionSettings settings)
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.FullyConsistent, settings, 101, "GetConfigSession", "f:\\15.00.1497\\sources\\dev\\clients\\src\\Owa2\\Server\\Core\\configuration\\MobileDevicePolicyCache.cs");
		}

		private MobileDevicePolicyData GetPolicyFromAD(OrgIdADObjectWrapper key)
		{
			ExTraceGlobals.MobileDevicePolicyTracer.Information<OrgIdADObjectWrapper>(0L, "MobileDevicePolicyCache.GetPolicyFromAD({0})", key);
			ADSessionSettings settings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(key.OrgId);
			IConfigurationSession session = this.GetConfigSession(settings);
			MobileDevicePolicyData policyData = null;
			try
			{
				ADNotificationAdapter.RunADOperation(delegate()
				{
					policyData = MobileDevicePolicyDataFactory.GetMobileDevicePolicyDataFromAD(session, key.AdObject);
				});
			}
			catch (LocalizedException arg)
			{
				ExTraceGlobals.MobileDevicePolicyTracer.TraceError<OrgIdADObjectWrapper, LocalizedException>((long)this.GetHashCode(), "MobileDevicePolicyCache.GetPolicyFromAD({0}) threw exception: {1}", key, arg);
				throw;
			}
			ExTraceGlobals.MobileDevicePolicyTracer.Information<OrgIdADObjectWrapper, MobileDevicePolicyData>((long)this.GetHashCode(), "MobileDevicePolicyCache.GetPolicyFromAD({0}) returned: {1}", key, policyData);
			return policyData;
		}

		private const int MobileDevicePolicyCacheCapacity = 5000;

		private static MobileDevicePolicyCache instance = new MobileDevicePolicyCache();
	}
}
