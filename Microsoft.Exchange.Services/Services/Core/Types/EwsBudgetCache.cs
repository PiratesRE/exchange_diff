using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class EwsBudgetCache : BudgetCache<EwsBudget>
	{
		protected override EwsBudget CreateBudget(BudgetKey key, IThrottlingPolicy policy)
		{
			EwsBudget ewsBudget = new EwsBudget(key, policy);
			ewsBudget.CheckOverBudget();
			return ewsBudget;
		}

		protected override void AfterCacheHit(BudgetKey key, EwsBudget value)
		{
			base.AfterCacheHit(key, value);
			value.CheckOverBudget();
		}

		public static readonly EwsBudgetCache Singleton = new EwsBudgetCache();
	}
}
