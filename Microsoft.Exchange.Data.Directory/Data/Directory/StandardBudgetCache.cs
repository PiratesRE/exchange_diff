using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal class StandardBudgetCache : BudgetCache<StandardBudget>
	{
		protected override StandardBudget CreateBudget(BudgetKey key, IThrottlingPolicy policy)
		{
			return new StandardBudget(key, policy);
		}

		public static readonly StandardBudgetCache Singleton = new StandardBudgetCache();
	}
}
