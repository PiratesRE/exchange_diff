using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal interface IPowerShellBudget : IBudget, IDisposable
	{
		CostHandle StartCmdlet(string cmdletName);

		CostHandle StartActiveRunspace();

		bool TryCheckOverBudget(CostType costType, out OverBudgetException exception);

		void CheckOverBudget(CostType costType);

		int TotalActiveRunspacesCount { get; }

		void CorrectRunspacesLeak(int leakedValue);

		string GetWSManBudgetUsage();

		string GetCmdletBudgetUsage();
	}
}
