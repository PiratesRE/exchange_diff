using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal abstract class BudgetKey
	{
		public BudgetKey(BudgetType budgetType, bool isServiceAccountBudget)
		{
			this.BudgetType = budgetType;
			this.IsServiceAccountBudget = isServiceAccountBudget;
		}

		public BudgetType BudgetType { get; private set; }

		public bool IsServiceAccountBudget { get; private set; }

		public static bool operator ==(BudgetKey key1, BudgetKey key2)
		{
			return object.Equals(key1, key2);
		}

		public static bool operator !=(BudgetKey key1, BudgetKey key2)
		{
			return !object.Equals(key1, key2);
		}

		public override bool Equals(object obj)
		{
			throw new NotImplementedException("BudgetKey.Equals must be overridden in derived classes");
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException("BudgetKey.GetHashCode must be overridden in derived classes.");
		}

		public static Func<BudgetKey, IThrottlingPolicy> LookupPolicyForTest;
	}
}
