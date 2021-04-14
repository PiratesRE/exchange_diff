using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal interface IBudget : IDisposable
	{
		void StartLocal(string callerInfo, TimeSpan preCharge = default(TimeSpan));

		void EndLocal();

		LocalTimeCostHandle LocalCostHandle { get; }

		DelayInfo GetDelay();

		DelayInfo GetDelay(ICollection<CostType> consideredCostTypes);

		void CheckOverBudget();

		void CheckOverBudget(ICollection<CostType> consideredCostTypes);

		bool TryCheckOverBudget(out OverBudgetException exception);

		bool TryCheckOverBudget(ICollection<CostType> consideredCostTypes, out OverBudgetException exception);

		BudgetKey Owner { get; }

		IThrottlingPolicy ThrottlingPolicy { get; }

		TimeSpan ResourceWorkAccomplished { get; }

		void ResetWorkAccomplished();

		bool TryGetBudgetBalance(out string budgetBalance);
	}
}
