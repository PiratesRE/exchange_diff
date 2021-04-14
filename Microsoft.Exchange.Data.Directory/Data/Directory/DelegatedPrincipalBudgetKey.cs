using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	internal class DelegatedPrincipalBudgetKey : LookupBudgetKey
	{
		public DelegatedPrincipalBudgetKey(DelegatedPrincipal principal, BudgetType budgetType) : base(budgetType, false)
		{
			this.cachedToString = string.Format("Delegated~{0}~{1}~{2}", principal.DelegatedOrganization, principal.UserId, budgetType);
			this.principal = principal;
			this.cachedHashCode = (base.BudgetType.GetHashCode() ^ this.principal.DelegatedOrganization.GetHashCode() ^ this.principal.UserId.GetHashCode());
		}

		public override bool Equals(object obj)
		{
			DelegatedPrincipalBudgetKey delegatedPrincipalBudgetKey = obj as DelegatedPrincipalBudgetKey;
			return !(delegatedPrincipalBudgetKey == null) && (delegatedPrincipalBudgetKey.BudgetType == base.BudgetType && delegatedPrincipalBudgetKey.principal.DelegatedOrganization == this.principal.DelegatedOrganization) && delegatedPrincipalBudgetKey.principal.UserId == this.principal.UserId;
		}

		internal override IThrottlingPolicy InternalLookup()
		{
			ExchangeConfigurationUnit cu = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				PartitionId partitionIdByAcceptedDomainName = ADAccountPartitionLocator.GetPartitionIdByAcceptedDomainName(this.principal.DelegatedOrganization);
				if (partitionIdByAcceptedDomainName != null)
				{
					ADSessionSettings sessionSettings = ADSessionSettings.FromAllTenantsPartitionId(partitionIdByAcceptedDomainName);
					ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 86, "InternalLookup", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\throttling\\DelegatedPrincipalBudgetKey.cs");
					tenantConfigurationSession.SessionSettings.IsSharedConfigChecked = true;
					cu = tenantConfigurationSession.GetExchangeConfigurationUnitByNameOrAcceptedDomain(this.principal.DelegatedOrganization);
				}
				if (cu == null)
				{
					throw new CannotResolveTenantNameException(DirectoryStrings.CannotResolveTenantNameByAcceptedDomain(this.principal.DelegatedOrganization));
				}
			});
			if (!adoperationResult.Succeeded)
			{
				ExTraceGlobals.ClientThrottlingTracer.TraceError<string, string, Exception>((long)this.GetHashCode(), "[DelegatedPrincipalBudgetKey.Lookup] Failed to find Delegated Organization: '{0}' for user '{1}', using global throttling policy.  Exception: '{2}'", this.principal.DelegatedOrganization, this.principal.UserId, adoperationResult.Exception);
				return ThrottlingPolicyCache.Singleton.GetGlobalThrottlingPolicy();
			}
			OrganizationId orgId = cu.OrganizationId;
			if (cu.SupportedSharedConfigurations.Count > 0)
			{
				SharedConfiguration sharedConfig = null;
				adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					sharedConfig = SharedConfiguration.GetSharedConfiguration(cu.OrganizationId);
				});
				if (!adoperationResult.Succeeded)
				{
					ExTraceGlobals.ClientThrottlingTracer.TraceError<OrganizationId, Exception>((long)this.GetHashCode(), "[DelegatedPrincipalBudgetKey.Lookup] Failed to get Shared Configuration Organization: '{0}', using global throttling policy.  Exception: '{1}'", cu.OrganizationId, adoperationResult.Exception);
					return ThrottlingPolicyCache.Singleton.GetGlobalThrottlingPolicy();
				}
				if (sharedConfig != null)
				{
					orgId = sharedConfig.SharedConfigId;
				}
			}
			return base.ADRetryLookup(delegate
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(orgId);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 149, "InternalLookup", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\throttling\\DelegatedPrincipalBudgetKey.cs");
				tenantOrTopologyConfigurationSession.SessionSettings.IsSharedConfigChecked = true;
				ThrottlingPolicy organizationThrottlingPolicy = tenantOrTopologyConfigurationSession.GetOrganizationThrottlingPolicy(orgId, false);
				if (organizationThrottlingPolicy == null)
				{
					ExTraceGlobals.ClientThrottlingTracer.TraceError<string, string>((long)this.GetHashCode(), "[DelegatedPrincipalBudgetKey.Lookup] Failed to find Default Throttling Policy for '{0}\\{1}', using global throttling policy", this.principal.DelegatedOrganization, this.principal.UserId);
					return ThrottlingPolicyCache.Singleton.GetGlobalThrottlingPolicy();
				}
				return organizationThrottlingPolicy.GetEffectiveThrottlingPolicy(true);
			});
		}

		public override string ToString()
		{
			return this.cachedToString;
		}

		public override int GetHashCode()
		{
			return this.cachedHashCode;
		}

		private const string ToStringFormat = "Delegated~{0}~{1}~{2}";

		private readonly string cachedToString;

		private readonly int cachedHashCode;

		private DelegatedPrincipal principal;
	}
}
