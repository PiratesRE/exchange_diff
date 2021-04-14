using System;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	internal class PswsBudgetWrapper : PowerShellBudgetWrapper
	{
		internal PswsBudgetWrapper(PowerShellBudget innerBudget) : base(innerBudget)
		{
		}

		protected override void StartLocalImpl(string callerInfo, TimeSpan preCharge)
		{
			if (base.LocalCostHandle != null)
			{
				LocalTimeCostHandle localCostHandle = base.LocalCostHandle;
				ExTraceGlobals.ClientThrottlingTracer.TraceDebug<BudgetKey>((long)this.GetHashCode(), "[PswsBudgetWrapper.StartLocalImpl] BudgetWrapper of user \"{0}\" is accessed by multi-thread concurrently.", localCostHandle.Budget.Owner);
				base.LocalCostHandle = null;
				localCostHandle.Dispose();
			}
			base.StartLocalImpl(callerInfo, preCharge);
		}
	}
}
