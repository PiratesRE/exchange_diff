using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal static class OrganizationPropertyCache
	{
		public static bool TryGetOrganizationProperties(OrganizationId organizationId, out OrganizationProperties organizationProperties)
		{
			if (OrganizationPropertyCache.orgPropertiesCache.TryGetValue(organizationId, out organizationProperties))
			{
				OrganizationPropertyCache.Tracer.TraceDebug<OrganizationId>(0L, "OrganizationProperties for '{0}' found in the cache", organizationId);
				return true;
			}
			if (organizationId == OrganizationId.ForestWideOrgId)
			{
				organizationProperties = new OrganizationProperties(true, null);
				OrganizationPropertyCache.Tracer.TraceDebug(0L, "OrganizationProperties for ForestWideOrgId has been manually constructed.");
			}
			else if (!OrganizationPropertyCache.TryGetOrganizationPropertiesFromAD(organizationId, ref organizationProperties))
			{
				OrganizationPropertyCache.Tracer.TraceError<OrganizationId>(0L, "TryGetOrganizationProperties('{0}') returns false because org is not found in AD!", organizationId);
				return false;
			}
			OrganizationPropertyCache.orgPropertiesCache.InsertAbsolute(organizationId, organizationProperties, CacheTimeToLive.OrgPropertyCacheTimeToLive, null);
			OrganizationPropertyCache.Tracer.TraceDebug<OrganizationId>(0L, "OrganizationProperties for '{0}' located in AD and added to the cache", organizationId);
			return true;
		}

		internal static void RemoveCacheEntry(OrganizationId organizationId)
		{
			OrganizationPropertyCache.orgPropertiesCache.Remove(organizationId);
		}

		private static bool TryGetOrganizationPropertiesFromAD(OrganizationId organizationId, ref OrganizationProperties organizationProperties)
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 271, "TryGetOrganizationPropertiesFromAD", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ConfigurationCache\\OrganizationPropertyCache.cs");
			ADRawEntry adrawEntry = tenantOrTopologyConfigurationSession.ReadADRawEntry(organizationId.ConfigurationUnit, OrganizationPropertyCache.orgProperties);
			if (adrawEntry == null)
			{
				OrganizationPropertyCache.Tracer.TraceError<OrganizationId>(0L, "CU for '{0}' is not found in AD", organizationId);
				return false;
			}
			organizationProperties = new OrganizationProperties((bool)adrawEntry[OrganizationSchema.SkipToUAndParentalControlCheck], (string)adrawEntry[ExchangeConfigurationUnitSchema.ServicePlan]);
			organizationProperties.ShowAdminAccessWarning = (bool)adrawEntry[OrganizationSchema.ShowAdminAccessWarning];
			organizationProperties.HideAdminAccessWarning = (bool)adrawEntry[OrganizationSchema.HideAdminAccessWarning];
			organizationProperties.ActivityBasedAuthenticationTimeoutEnabled = !(bool)adrawEntry[OrganizationSchema.ActivityBasedAuthenticationTimeoutDisabled];
			organizationProperties.ActivityBasedAuthenticationTimeoutWithSingleSignOnEnabled = !(bool)adrawEntry[OrganizationSchema.ActivityBasedAuthenticationTimeoutWithSingleSignOnDisabled];
			organizationProperties.ActivityBasedAuthenticationTimeoutInterval = (EnhancedTimeSpan)adrawEntry[OrganizationSchema.ActivityBasedAuthenticationTimeoutInterval];
			organizationProperties.IsLicensingEnforced = (bool)adrawEntry[OrganizationSchema.IsLicensingEnforced];
			organizationProperties.IsTenantAccessBlocked = (bool)adrawEntry[OrganizationSchema.IsTenantAccessBlocked];
			return true;
		}

		private static readonly Trace Tracer = ExTraceGlobals.SystemConfigurationCacheTracer;

		private static TimeoutCache<OrganizationId, OrganizationProperties> orgPropertiesCache = new TimeoutCache<OrganizationId, OrganizationProperties>(16, 1024, false);

		private static ADPropertyDefinition[] orgProperties = new ADPropertyDefinition[]
		{
			OrganizationSchema.SkipToUAndParentalControlCheck,
			ExchangeConfigurationUnitSchema.ServicePlan,
			OrganizationSchema.HideAdminAccessWarning,
			OrganizationSchema.ShowAdminAccessWarning,
			OrganizationSchema.ActivityBasedAuthenticationTimeoutDisabled,
			OrganizationSchema.ActivityBasedAuthenticationTimeoutWithSingleSignOnDisabled,
			OrganizationSchema.ActivityBasedAuthenticationTimeoutInterval,
			OrganizationSchema.IsLicensingEnforced,
			OrganizationSchema.IsTenantAccessBlocked
		};
	}
}
