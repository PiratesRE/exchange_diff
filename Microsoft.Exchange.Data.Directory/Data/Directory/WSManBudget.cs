using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal class WSManBudget : PowerShellBudget
	{
		internal WSManBudget(BudgetKey owner, IThrottlingPolicy policy) : base(owner, policy)
		{
		}

		protected override bool TrackActiveRunspacePerfCounter
		{
			get
			{
				return true;
			}
		}

		protected override void UpdatePolicyValueTakingEffectInThisBudget(SingleComponentThrottlingPolicy policy)
		{
			this.activeRunspacesPolicyValue = policy.MaxConcurrency;
			this.powerShellMaxCmdletsPolicyValue = policy.PowerShellMaxCmdlets;
			this.powerShellMaxCmdletsTimePeriodPolicyValue = policy.PowerShellMaxCmdletsTimePeriod;
			this.powerShellMaxRunspacesPolicyValue = policy.PowerShellMaxRunspaces;
			this.powerShellMaxRunspacesTimePeriodPolicyValue = policy.PowerShellMaxRunspacesTimePeriod;
		}
	}
}
