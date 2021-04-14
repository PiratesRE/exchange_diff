using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal class WSManTenantBudget : PowerShellBudget
	{
		internal WSManTenantBudget(BudgetKey owner, IThrottlingPolicy policy) : base(owner, policy)
		{
		}

		protected override string MaxConcurrencyOverBudgetReason
		{
			get
			{
				return "MaxTenantConcurrency";
			}
		}

		protected override string MaxRunspacesTimePeriodOverBudgetReason
		{
			get
			{
				return "MaxTenantRunspaces";
			}
		}

		protected override bool TrackActiveRunspacePerfCounter
		{
			get
			{
				return false;
			}
		}

		protected override void UpdatePolicyValueTakingEffectInThisBudget(SingleComponentThrottlingPolicy policy)
		{
			this.activeRunspacesPolicyValue = policy.PowerShellMaxTenantConcurrency;
			this.powerShellMaxRunspacesPolicyValue = policy.PowerShellMaxTenantRunspaces;
			this.powerShellMaxRunspacesTimePeriodPolicyValue = policy.PowerShellMaxRunspacesTimePeriod;
		}

		public const string MaxTenantRunspacesPart = "MaxTenantRunspaces";
	}
}
