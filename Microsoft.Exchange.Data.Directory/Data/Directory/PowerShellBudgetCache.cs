using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal class PowerShellBudgetCache : BudgetCache<PowerShellBudget>
	{
		protected override PowerShellBudget CreateBudget(BudgetKey key, IThrottlingPolicy policy)
		{
			BudgetType budgetType = key.BudgetType;
			if (budgetType == BudgetType.PowerShell)
			{
				return new PowerShellBudget(key, policy);
			}
			switch (budgetType)
			{
			case BudgetType.WSMan:
				return new WSManBudget(key, policy);
			case BudgetType.WSManTenant:
				return new WSManTenantBudget(key, policy);
			default:
				throw new ArgumentException("PowerShellBudgetCache can only be used to create Power-ish budgets.  Passed budget key: " + key);
			}
		}

		public static readonly PowerShellBudgetCache Singleton = new PowerShellBudgetCache();
	}
}
