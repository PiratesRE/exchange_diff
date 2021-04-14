using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.AirSync
{
	internal class EasDeviceBudgetCache : BudgetCache<EasDeviceBudget>
	{
		protected override EasDeviceBudget CreateBudget(BudgetKey key, IThrottlingPolicy policy)
		{
			EasDeviceBudgetKey easDeviceBudgetKey = key as EasDeviceBudgetKey;
			if (easDeviceBudgetKey == null)
			{
				throw new ArgumentException("key must be an EasDeviceBudgetKey", "key");
			}
			return new EasDeviceBudget(easDeviceBudgetKey, policy);
		}

		public static readonly EasDeviceBudgetCache Singleton = new EasDeviceBudgetCache();
	}
}
