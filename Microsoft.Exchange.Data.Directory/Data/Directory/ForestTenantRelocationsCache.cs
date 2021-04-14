using System;
using System.Collections.Concurrent;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class ForestTenantRelocationsCache
	{
		public static bool IsTenantRelocationAllowed(string partitionFqdn)
		{
			return ForestTenantRelocationsCache.RelocationsAllowedCache.IsTenantRelocationAllowed(partitionFqdn);
		}

		public static void UpdateTenantRelocationAllowedValue(Organization rootOrganization)
		{
			ForestTenantRelocationsCache.RelocationsAllowedCache.UpdateTenantRelocationAllowedValue(rootOrganization);
		}

		public static string GetRidMasterName(PartitionId partitionId)
		{
			return ForestTenantRelocationsCache.RidMastersCache.GetRidMasterName(partitionId, false);
		}

		public static string RefreshRidMasterName(PartitionId partitionId)
		{
			return ForestTenantRelocationsCache.RidMastersCache.GetRidMasterName(partitionId, true);
		}

		internal static void Reset()
		{
			ForestTenantRelocationsCache.RidMastersCache.Reset();
			ForestTenantRelocationsCache.RelocationsAllowedCache.Reset();
		}

		private class RelocationsAllowedCache
		{
			private static ForestTenantRelocationsCache.RelocationsAllowedCache GetInstance()
			{
				if (ForestTenantRelocationsCache.RelocationsAllowedCache.instance == null)
				{
					ForestTenantRelocationsCache.RelocationsAllowedCache.instance = new ForestTenantRelocationsCache.RelocationsAllowedCache();
				}
				return ForestTenantRelocationsCache.RelocationsAllowedCache.instance;
			}

			internal static void Reset()
			{
				ForestTenantRelocationsCache.RelocationsAllowedCache.instance = null;
			}

			public static void UpdateTenantRelocationAllowedValue(Organization rootOrganization)
			{
				if (!OrganizationId.ForestWideOrgId.Equals(rootOrganization.OrganizationId))
				{
					throw new ArgumentException("rootOrganization parameter value must be root Organization");
				}
				string forestFQDN = rootOrganization.Id.GetPartitionId().ForestFQDN;
				bool value = rootOrganization.TenantRelocationsAllowed;
				ForestTenantRelocationsCache.RelocationsAllowedCache.GetInstance().tenantRelocationsAllowed[forestFQDN] = new ExpiringTenantRelocationsAllowedValue(value);
			}

			public static bool IsTenantRelocationAllowed(string partitionFqdn)
			{
				if (string.IsNullOrEmpty(partitionFqdn))
				{
					throw new ArgumentNullException("partitionFqdn");
				}
				if (!Datacenter.IsMultiTenancyEnabled())
				{
					return false;
				}
				ForestTenantRelocationsCache.RelocationsAllowedCache relocationsAllowedCache = ForestTenantRelocationsCache.RelocationsAllowedCache.GetInstance();
				ExpiringTenantRelocationsAllowedValue expiringTenantRelocationsAllowedValue;
				if (!relocationsAllowedCache.tenantRelocationsAllowed.TryGetValue(partitionFqdn, out expiringTenantRelocationsAllowedValue) || expiringTenantRelocationsAllowedValue.Expired)
				{
					Organization rootOrgContainer = ADSystemConfigurationSession.GetRootOrgContainer(partitionFqdn, null, null);
					expiringTenantRelocationsAllowedValue = new ExpiringTenantRelocationsAllowedValue(rootOrgContainer.TenantRelocationsAllowed);
					relocationsAllowedCache.tenantRelocationsAllowed[partitionFqdn] = expiringTenantRelocationsAllowedValue;
				}
				return expiringTenantRelocationsAllowedValue.Value;
			}

			private static ForestTenantRelocationsCache.RelocationsAllowedCache instance;

			private ConcurrentDictionary<string, ExpiringTenantRelocationsAllowedValue> tenantRelocationsAllowed = new ConcurrentDictionary<string, ExpiringTenantRelocationsAllowedValue>(StringComparer.OrdinalIgnoreCase);
		}

		private class RidMastersCache
		{
			private static ForestTenantRelocationsCache.RidMastersCache GetInstance()
			{
				if (ForestTenantRelocationsCache.RidMastersCache.instance == null)
				{
					ForestTenantRelocationsCache.RidMastersCache.instance = new ForestTenantRelocationsCache.RidMastersCache();
				}
				return ForestTenantRelocationsCache.RidMastersCache.instance;
			}

			internal static void Reset()
			{
				ForestTenantRelocationsCache.RidMastersCache.instance = null;
			}

			public static string GetRidMasterName(PartitionId partitionId, bool forceRefresh)
			{
				if (partitionId == null)
				{
					throw new ArgumentNullException("partitionId");
				}
				ForestTenantRelocationsCache.RidMastersCache ridMastersCache = ForestTenantRelocationsCache.RidMastersCache.GetInstance();
				ExpiringRidMasterNameValue expiringRidMasterNameValue;
				if (forceRefresh || !ridMastersCache.ridMasters.TryGetValue(partitionId.ForestFQDN, out expiringRidMasterNameValue) || expiringRidMasterNameValue.Expired)
				{
					string text = ForestTenantRelocationsCache.RidMastersCache.FindRidMasterNameForPartition(partitionId);
					if (text == null)
					{
						throw new ADTransientException(DirectoryStrings.ErrorCannotFindRidMasterForPartition(partitionId.ForestFQDN));
					}
					expiringRidMasterNameValue = new ExpiringRidMasterNameValue(text);
					ridMastersCache.ridMasters[partitionId.ForestFQDN] = expiringRidMasterNameValue;
				}
				return expiringRidMasterNameValue.Value;
			}

			private static string FindRidMasterNameForPartition(PartitionId partitionId)
			{
				string result = null;
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(partitionId), 213, "FindRidMasterNameForPartition", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\RelocationCache\\ForestTenantRelocationsCache.cs");
				topologyConfigurationSession.UseConfigNC = false;
				RidManagerContainer[] array = topologyConfigurationSession.Find<RidManagerContainer>(null, QueryScope.SubTree, null, null, 1);
				if (array != null && array.Length > 0)
				{
					ADObjectId fsmoRoleOwner = array[0].FsmoRoleOwner;
					if (fsmoRoleOwner != null)
					{
						topologyConfigurationSession.UseConfigNC = true;
						ADServer adserver = topologyConfigurationSession.Read<ADServer>(fsmoRoleOwner.Parent);
						if (adserver != null)
						{
							result = adserver.DnsHostName;
						}
					}
				}
				return result;
			}

			private static ForestTenantRelocationsCache.RidMastersCache instance;

			private ConcurrentDictionary<string, ExpiringRidMasterNameValue> ridMasters = new ConcurrentDictionary<string, ExpiringRidMasterNameValue>(StringComparer.OrdinalIgnoreCase);
		}
	}
}
