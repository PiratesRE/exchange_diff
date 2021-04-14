using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal class DiscoveryTenantBudgetKey : TenantBudgetKey
	{
		public DiscoveryTenantBudgetKey(OrganizationId organizationId, BudgetType budgetType) : base(organizationId, budgetType)
		{
		}

		protected override bool InternalEquals(object obj)
		{
			DiscoveryTenantBudgetKey discoveryTenantBudgetKey = obj as DiscoveryTenantBudgetKey;
			return !(discoveryTenantBudgetKey == null) && discoveryTenantBudgetKey.BudgetType == base.BudgetType && discoveryTenantBudgetKey.OrganizationId == base.OrganizationId;
		}

		protected override IThrottlingPolicy LookupPolicyByOrganizationId()
		{
			string distinguishedName;
			if (OrganizationId.ForestWideOrgId == base.OrganizationId)
			{
				distinguishedName = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest().DistinguishedName;
			}
			else
			{
				distinguishedName = base.OrganizationId.ConfigurationUnit.DistinguishedName;
			}
			string distinguishedName2 = string.Format("CN=Global Settings,{0}", distinguishedName);
			ADObjectId adobjectId = new ADObjectId(distinguishedName2);
			ADObjectId childId = adobjectId.GetChildId("DiscoveryThrottlingPolicy");
			return ThrottlingPolicyCache.Singleton.Get(base.OrganizationId, childId);
		}
	}
}
