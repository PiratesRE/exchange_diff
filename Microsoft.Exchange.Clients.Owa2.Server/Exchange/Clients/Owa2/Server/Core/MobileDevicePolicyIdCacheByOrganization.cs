using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Configuration;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class MobileDevicePolicyIdCacheByOrganization : LazyLookupExactTimeoutCache<OrganizationId, ADObjectId>
	{
		protected MobileDevicePolicyIdCacheByOrganization(int maxCount, TimeSpan slidingLiveTime, TimeSpan absoluteLiveTime) : base(maxCount, false, slidingLiveTime, absoluteLiveTime, CacheFullBehavior.ExpireExisting)
		{
		}

		protected MobileDevicePolicyIdCacheByOrganization() : this(5000, TimeSpan.FromMinutes(15.0), TimeSpan.FromMinutes(60.0))
		{
		}

		internal static MobileDevicePolicyIdCacheByOrganization Instance
		{
			get
			{
				return MobileDevicePolicyIdCacheByOrganization.instance;
			}
		}

		protected override ADObjectId CreateOnCacheMiss(OrganizationId key, ref bool shouldAdd)
		{
			ADObjectId policyIdFromAD = this.GetPolicyIdFromAD(key);
			shouldAdd = (policyIdFromAD != null);
			return policyIdFromAD;
		}

		protected virtual IConfigurationSession GetConfigSession(ADSessionSettings settings)
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.FullyConsistent, settings, 103, "GetConfigSession", "f:\\15.00.1497\\sources\\dev\\clients\\src\\Owa2\\Server\\Core\\configuration\\MobileDevicePolicyIdCacheByOrganization.cs");
		}

		private ADObjectId GetPolicyIdFromAD(OrganizationId key)
		{
			ExTraceGlobals.MobileDevicePolicyTracer.Information<OrganizationId>((long)this.GetHashCode(), "MobileDevicePolicyIdCacheByOrganization.GetPolicyFromAD({0})", key);
			ADSessionSettings settings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(key);
			IConfigurationSession session = this.GetConfigSession(settings);
			ADObjectId rootId = session.GetOrgContainerId();
			QueryFilter filter = new BitMaskAndFilter(MobileMailboxPolicySchema.MobileFlags, 4096UL);
			SortBy sortBy = new SortBy(ADObjectSchema.WhenChanged, SortOrder.Descending);
			ADObjectId policyId = null;
			try
			{
				ADNotificationAdapter.RunADOperation(delegate()
				{
					MobileMailboxPolicy[] array = session.Find<MobileMailboxPolicy>(rootId, QueryScope.SubTree, filter, sortBy, 1);
					if (array != null && array.Length > 0)
					{
						policyId = array[0].Id;
						OrgIdADObjectWrapper key2 = new OrgIdADObjectWrapper(policyId, key);
						if (!MobileDevicePolicyCache.Instance.Contains(key2))
						{
							MobileDevicePolicyData mobileDevicePolicyDataFromMobileMailboxPolicy = MobileDevicePolicyDataFactory.GetMobileDevicePolicyDataFromMobileMailboxPolicy(array[0]);
							MobileDevicePolicyCache.Instance.TryAdd(key2, ref mobileDevicePolicyDataFromMobileMailboxPolicy);
						}
					}
				});
			}
			catch (LocalizedException arg)
			{
				ExTraceGlobals.MobileDevicePolicyTracer.TraceError<OrganizationId, LocalizedException>((long)this.GetHashCode(), "MobileDevicePolicyIdCacheByOrganization.GetPolicyIdFromAD({0}) threw exception: {1}", key, arg);
				throw;
			}
			ExTraceGlobals.MobileDevicePolicyTracer.Information<OrganizationId, ADObjectId>((long)this.GetHashCode(), "MobileDevicePolicyIdCacheByOrganization.GetPolicyFromAD({0}) returned: {1}", key, policyId);
			return policyId;
		}

		private const int MobileDevicePolicyIdCacheCapacity = 5000;

		private static MobileDevicePolicyIdCacheByOrganization instance = new MobileDevicePolicyIdCacheByOrganization();
	}
}
