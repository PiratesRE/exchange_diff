using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class BudgetInformation
	{
		public IPowerShellBudget Budget { get; set; }

		public CostHandle Handle { get; set; }

		public ExEventLog.EventTuple ThrottledEventInfo { get; set; }
	}
}
