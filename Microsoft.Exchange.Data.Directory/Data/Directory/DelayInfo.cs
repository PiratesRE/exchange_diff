using System;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	internal class DelayInfo
	{
		public static void TraceMicroDelays(IBudget budget, TimeSpan workAccomplished, TimeSpan microDelay)
		{
			StandardBudgetWrapper standardBudgetWrapper = budget as StandardBudgetWrapper;
			if (standardBudgetWrapper != null)
			{
				int num = (int)standardBudgetWrapper.GetInnerBudget().CasTokenBucket.GetBalance();
				ExTraceGlobals.BudgetDelayTracer.TraceDebug(0L, "Budget: '{0}', Balance: {1}, Work Done: {2}, MicroDelay: {3}", new object[]
				{
					budget.Owner,
					num,
					workAccomplished,
					microDelay
				});
			}
		}

		public DelayInfo(TimeSpan delay, bool required)
		{
			this.Delay = delay;
			this.Required = required;
		}

		public TimeSpan Delay { get; private set; }

		public bool Required { get; private set; }

		public static readonly DelayInfo NoDelay = new DelayInfo(TimeSpan.Zero, false);
	}
}
