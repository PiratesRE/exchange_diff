using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationCache
{
	internal static class InternalDirectoryRootOrganizationCache
	{
		internal static ADObjectId GetRootOrgContainerId(string partitionFqdn)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("partitionFqdn", partitionFqdn);
			ADObjectIdCachableItem adobjectIdCachableItem;
			if (InternalDirectoryRootOrganizationCache.rootOrgContainerIds.TryGetValue(partitionFqdn, out adobjectIdCachableItem))
			{
				return adobjectIdCachableItem.Value;
			}
			return null;
		}

		internal static TenantCULocation GetTenantCULocation(string partitionFqdn)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("partitionFqdn", partitionFqdn);
			return InternalDirectoryRootOrganizationCache.InternalGetTenantCULocation(partitionFqdn);
		}

		internal static void PopulateCache(string partitionFqdn, Organization rootOrganization)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("partitionFqdn", partitionFqdn);
			ArgumentValidator.ThrowIfNull("organization", rootOrganization);
			if (InternalDirectoryRootOrganizationCache.InternalGetTenantCULocation(partitionFqdn) == TenantCULocation.Undefined)
			{
				TenantCULocation value;
				if (Globals.IsDatacenter)
				{
					value = ((rootOrganization.ForestMode == ForestModeFlags.Legacy) ? TenantCULocation.ConfigNC : TenantCULocation.DomainNC);
				}
				else
				{
					value = TenantCULocation.ConfigNC;
				}
				InternalDirectoryRootOrganizationCache.tenantCULocations.TryAdd(partitionFqdn, value);
			}
			InternalDirectoryRootOrganizationCache.rootOrgContainerIds.TryAdd(partitionFqdn, new ADObjectIdCachableItem(rootOrganization.Id));
			ForestTenantRelocationsCache.UpdateTenantRelocationAllowedValue(rootOrganization);
		}

		internal static void InitializeForestModeFlagForSetup(string partitionFqdn, TenantCULocation cuLocation)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("partitionFqdn", partitionFqdn);
			if (cuLocation == TenantCULocation.Undefined)
			{
				throw new ArgumentOutOfRangeException("cuLocation has invalid value");
			}
			if (InternalDirectoryRootOrganizationCache.InternalGetTenantCULocation(partitionFqdn) == TenantCULocation.Undefined)
			{
				InternalDirectoryRootOrganizationCache.tenantCULocations.TryAdd(partitionFqdn, cuLocation);
			}
		}

		private static TenantCULocation InternalGetTenantCULocation(string partitionFqdn)
		{
			TenantCULocation result;
			if (InternalDirectoryRootOrganizationCache.tenantCULocations.TryGetValue(partitionFqdn, out result))
			{
				return result;
			}
			return TenantCULocation.Undefined;
		}

		[Conditional("DEBUG")]
		private static void Dbg_CheckCaller()
		{
		}

		[Conditional("DEBUG")]
		private static void Dbg_CheckCaller(Func<string, string, bool> frameMatch)
		{
			StackTrace stackTrace = new StackTrace();
			foreach (StackFrame stackFrame in stackTrace.GetFrames())
			{
				MethodBase method = stackFrame.GetMethod();
				if (!(method.DeclaringType == null))
				{
					string fullName = method.DeclaringType.FullName;
					string name = method.Name;
					if (frameMatch(fullName, name))
					{
						break;
					}
				}
			}
		}

		private const long RootOrgContainerIdSizeInBytes = 1048576L;

		private const int RootOrgContainerIdCacheExpirationHours = 24;

		private static ConcurrentDictionary<string, TenantCULocation> tenantCULocations = new ConcurrentDictionary<string, TenantCULocation>(StringComparer.OrdinalIgnoreCase);

		private static Cache<string, ADObjectIdCachableItem> rootOrgContainerIds = new Cache<string, ADObjectIdCachableItem>(1048576L, TimeSpan.FromHours(24.0), TimeSpan.Zero);
	}
}
