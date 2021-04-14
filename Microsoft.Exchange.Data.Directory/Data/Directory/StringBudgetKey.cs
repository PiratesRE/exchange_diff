using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal class StringBudgetKey : LookupBudgetKey
	{
		public StringBudgetKey(string key, bool isServiceAccount, BudgetType budgetType) : base(budgetType, isServiceAccount)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			this.Key = key;
			this.cachedToString = string.Format("Str~{0}~{1}~{2}", key, budgetType, isServiceAccount);
			this.cachedhashCode = this.cachedToString.GetHashCode();
		}

		public string Key { get; private set; }

		public override bool Equals(object obj)
		{
			StringBudgetKey stringBudgetKey = obj as StringBudgetKey;
			return !(stringBudgetKey == null) && (stringBudgetKey.BudgetType == base.BudgetType && stringBudgetKey.Key == this.Key) && stringBudgetKey.IsServiceAccountBudget == base.IsServiceAccountBudget;
		}

		public override int GetHashCode()
		{
			return this.cachedhashCode;
		}

		public override string ToString()
		{
			return this.cachedToString;
		}

		internal override IThrottlingPolicy InternalLookup()
		{
			return ThrottlingPolicyCache.Singleton.GetGlobalThrottlingPolicy();
		}

		private const string ToStringFormat = "Str~{0}~{1}~{2}";

		private readonly int cachedhashCode;

		private readonly string cachedToString;
	}
}
