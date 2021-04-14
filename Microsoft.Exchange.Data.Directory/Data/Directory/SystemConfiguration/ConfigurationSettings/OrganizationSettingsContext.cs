using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OrganizationSettingsContext : SettingsContextBase
	{
		public OrganizationSettingsContext(OrganizationId orgId, SettingsContextBase nextContext = null) : base(nextContext)
		{
			if (orgId != OrganizationId.ForestWideOrgId)
			{
				this.Initialize(OrganizationSettingsContext.OrgCache.Get(orgId));
			}
		}

		public OrganizationSettingsContext(ExchangeConfigurationUnit org, SettingsContextBase nextContext = null) : base(nextContext)
		{
			this.Initialize(org);
		}

		public override string OrganizationName
		{
			get
			{
				return this.orgName;
			}
		}

		public override ExchangeObjectVersion OrganizationVersion
		{
			get
			{
				return this.orgVersion;
			}
		}

		private void Initialize(ExchangeConfigurationUnit org)
		{
			if (org != null)
			{
				this.orgName = org.Name;
				this.orgVersion = org.AdminDisplayVersion;
			}
		}

		private static readonly OrganizationSettingsContext.OrganizationCache OrgCache = new OrganizationSettingsContext.OrganizationCache();

		private string orgName;

		private ExchangeObjectVersion orgVersion;

		private class OrganizationCache : LazyLookupTimeoutCache<OrganizationId, ExchangeConfigurationUnit>
		{
			public OrganizationCache() : base(8, 16, false, TimeSpan.FromHours(1.0), TimeSpan.FromHours(1.0))
			{
			}

			protected override ExchangeConfigurationUnit CreateOnCacheMiss(OrganizationId orgId, ref bool shouldAdd)
			{
				shouldAdd = false;
				if (orgId == OrganizationId.ForestWideOrgId)
				{
					return null;
				}
				ExchangeConfigurationUnit org = null;
				ADNotificationAdapter.TryRunADOperation(delegate()
				{
					ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(orgId), 135, "CreateOnCacheMiss", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ConfigurationSettings\\OrganizationSettingsContext.cs");
					org = tenantConfigurationSession.Read<ExchangeConfigurationUnit>(orgId.ConfigurationUnit);
				});
				shouldAdd = true;
				return org;
			}
		}
	}
}
