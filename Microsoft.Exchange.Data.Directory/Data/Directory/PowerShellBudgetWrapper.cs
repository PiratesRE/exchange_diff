using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal class PowerShellBudgetWrapper : BudgetWrapper<PowerShellBudget>, IPowerShellBudget, IBudget, IDisposable
	{
		internal PowerShellBudgetWrapper(PowerShellBudget innerBudget) : base(innerBudget)
		{
		}

		public CostHandle StartCmdlet(string cmdletName)
		{
			CostHandle costHandle = base.GetInnerBudget().StartCmdlet(cmdletName, new Action<CostHandle>(base.HandleCostHandleRelease));
			base.AddAction(costHandle);
			return costHandle;
		}

		public CostHandle StartActiveRunspace()
		{
			CostHandle costHandle = base.GetInnerBudget().StartActiveRunspace(new Action<CostHandle>(base.HandleCostHandleRelease));
			base.AddAction(costHandle);
			return costHandle;
		}

		public bool TryCheckOverBudget(CostType costType, out OverBudgetException exception)
		{
			return base.GetInnerBudget().TryCheckOverBudget(costType, out exception);
		}

		public void CheckOverBudget(CostType costType)
		{
			OverBudgetException ex;
			if (this.TryCheckOverBudget(costType, out ex))
			{
				throw ex;
			}
		}

		public int TotalActiveRunspacesCount
		{
			get
			{
				return base.GetInnerBudget().TotalActiveRunspacesCount;
			}
		}

		public void CorrectRunspacesLeak(int leakedValue)
		{
			base.GetInnerBudget().CorrectRunspacesLeak(leakedValue);
		}

		public string GetWSManBudgetUsage()
		{
			return base.GetInnerBudget().GetWSManBudgetUsage();
		}

		public string GetCmdletBudgetUsage()
		{
			return base.GetInnerBudget().GetCmdletBudgetUsage();
		}

		protected override PowerShellBudget ReacquireBudget()
		{
			return PowerShellBudgetCache.Singleton.Get(base.Owner);
		}
	}
}
