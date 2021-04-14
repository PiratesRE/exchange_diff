using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory
{
	public class BudgetCacheHandlerMetadata
	{
		public int TotalCount { get; set; }

		public int MatchingCount { get; set; }

		public int Efficiency { get; set; }

		public int NotThrottled { get; set; }

		public int InMicroDelay { get; set; }

		public int InCutoff { get; set; }

		public int ServiceAccountBudgets { get; set; }

		public List<BudgetHandlerMetadata> Budgets { get; set; }
	}
}
