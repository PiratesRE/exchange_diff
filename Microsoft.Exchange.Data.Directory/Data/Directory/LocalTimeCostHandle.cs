using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory
{
	internal class LocalTimeCostHandle : CostHandle
	{
		public LocalTimeCostHandle(Budget budget, Action<CostHandle> onRelease, string description, TimeSpan preCharge = default(TimeSpan)) : base(budget, CostType.CAS, onRelease, description, preCharge)
		{
			this.UnaccountedStartTime = base.StartTime;
		}

		public DateTime UnaccountedStartTime { get; set; }

		public TimeSpan UnaccountedForTime
		{
			get
			{
				return TimeProvider.UtcNow - this.UnaccountedStartTime;
			}
		}

		internal bool ReverseBudgetCharge { get; set; }
	}
}
