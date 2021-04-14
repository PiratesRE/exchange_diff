using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal class TenantHydrationBudgetKey : LookupBudgetKey
	{
		private TenantHydrationBudgetKey() : base(BudgetType.PowerShell, false)
		{
			this.cachedHashCode = "TenantHydrationBudgetKey under FirstOrg".GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is TenantHydrationBudgetKey;
		}

		internal override IThrottlingPolicy InternalLookup()
		{
			ADObjectId rootOrgContainerIdForLocalForest = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
			string distinguishedName = string.Format("CN={0},CN=Global Settings,{1}", "TenantHydrationThrottlingPolicy", rootOrgContainerIdForLocalForest.DistinguishedName);
			ADObjectId throttlingPolicyId = new ADObjectId(distinguishedName);
			return ThrottlingPolicyCache.Singleton.Get(OrganizationId.ForestWideOrgId, throttlingPolicyId);
		}

		public override string ToString()
		{
			return "TenantHydrationBudgetKey under FirstOrg";
		}

		public override int GetHashCode()
		{
			return this.cachedHashCode;
		}

		private const string CachedToString = "TenantHydrationBudgetKey under FirstOrg";

		private readonly int cachedHashCode;

		public static readonly TenantHydrationBudgetKey Singleton = new TenantHydrationBudgetKey();
	}
}
