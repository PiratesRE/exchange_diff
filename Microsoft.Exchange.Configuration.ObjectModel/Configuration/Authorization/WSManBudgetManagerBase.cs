using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.Authorization;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal abstract class WSManBudgetManagerBase<T> : BudgetManager where T : PowerShellBudget
	{
		protected override TimeSpan BudgetTimeout
		{
			get
			{
				return WSManBudgetManagerBase<T>.budgetTimeout;
			}
		}

		internal CostHandle StartRunspace(AuthZPluginUserToken userToken)
		{
			if (!this.ShouldThrottling(userToken))
			{
				return null;
			}
			CostHandle result;
			lock (base.InstanceLock)
			{
				result = this.StartRunspaceImpl(userToken);
			}
			return result;
		}

		internal CostHandle StartCmdlet(AuthZPluginUserToken userToken)
		{
			if (!this.ShouldThrottling(userToken))
			{
				return null;
			}
			lock (base.InstanceLock)
			{
				IPowerShellBudget budget = base.GetBudget(userToken, true, true);
				if (budget != null)
				{
					ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string>(0L, "Start budget tracking for Cmdlet, key {0}", this.CreateKey(userToken));
					return budget.StartCmdlet(null);
				}
				ExTraceGlobals.PublicPluginAPITracer.TraceError<string>(0L, "Try to start budget tracking for Cmdlet, key {0} But the budget doesn't exist.", this.CreateKey(userToken));
			}
			return null;
		}

		private static readonly TimeSpan budgetTimeout = TimeSpan.FromMinutes(10.0);
	}
}
