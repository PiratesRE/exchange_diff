using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.ProvisioningCache
{
	internal static class CannedProvisioningExpirationTime
	{
		static CannedProvisioningExpirationTime()
		{
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.FirstOrgContainerId, new TimeSpan(24, 0, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.DatabaseContainerId, new TimeSpan(24, 0, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.ServerContainerId, new TimeSpan(24, 0, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.AdministrativeGroupLegDN, new TimeSpan(24, 0, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.ServerAdminDisplayVersionCacheKey, new TimeSpan(0, 30, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.ProvisioningEnabledDatabasesOnLocalSite, new TimeSpan(0, 30, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.ProvisioningEnabledDatabasesOnAllSites, new TimeSpan(6, 0, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.MtaDictionary, new TimeSpan(24, 0, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.AddressBookPolicies, new TimeSpan(0, 60, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.CacheKeyMailboxPlanId, new TimeSpan(0, 60, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.CacheKeyMailboxPlanIdParameterId, new TimeSpan(0, 60, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.DefaultMailboxPlan, new TimeSpan(0, 60, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.DefaultRoleAssignmentPolicyId, new TimeSpan(0, 60, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.EnforcementProvisioningPolicies, new TimeSpan(0, 60, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.OrganizationAcceptedDomains, new TimeSpan(0, 60, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.OrganizationSiteMailboxAddressesTemplate, new TimeSpan(0, 60, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.OrganizationUnifiedPolicyNotificationClients, new TimeSpan(0, 60, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.GlobalUnifiedPolicyNotificationClientsInfo, new TimeSpan(24, 0, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.OrganizationCUContainer, new TimeSpan(0, 60, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.OrganizationIdDictionary, new TimeSpan(0, 60, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.OrganizationalUnitDictionary, new TimeSpan(0, 60, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.EomsGroupCacheKey, new TimeSpan(0, 60, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.EopsGroupCacheKey, new TimeSpan(0, 60, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.MailboxSharedConfigCacheKey, new TimeSpan(0, 60, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.MailboxRoleAssignmentPolicyCacheKey, new TimeSpan(0, 60, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.MailboxDatabaseForDefaultRetentionValuesCacheKey, new TimeSpan(0, 60, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.SharedConfigurationStateCacheKey, new TimeSpan(0, 60, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.IsDehydratedConfigurationCacheKey, new TimeSpan(0, 60, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.NamespaceAuthenticationTypeCacheKey, new TimeSpan(0, 60, 0));
			CannedProvisioningExpirationTime.defaultExpirationTime.Add(CannedProvisioningCacheKeys.TransportRuleAttachmentTextScanLimitCacheKey, new TimeSpan(0, 60, 0));
		}

		public static TimeSpan GetDefaultExpirationTime(Guid key)
		{
			if (CannedProvisioningExpirationTime.defaultExpirationTime.ContainsKey(key))
			{
				return CannedProvisioningExpirationTime.defaultExpirationTime[key];
			}
			throw new ArgumentException("Key is not a valid ProvisioningCache Key.");
		}

		public static ICollection<Guid> AllCannedProvisioningCacheKeys
		{
			get
			{
				return CannedProvisioningExpirationTime.defaultExpirationTime.Keys;
			}
		}

		private static Dictionary<Guid, TimeSpan> defaultExpirationTime = new Dictionary<Guid, TimeSpan>();
	}
}
