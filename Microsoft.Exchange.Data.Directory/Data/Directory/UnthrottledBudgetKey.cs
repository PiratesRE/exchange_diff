using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal class UnthrottledBudgetKey : LookupBudgetKey
	{
		public UnthrottledBudgetKey(string id, BudgetType budgetType) : this(id, budgetType, false)
		{
		}

		public UnthrottledBudgetKey(string id, BudgetType budgetType, bool isServiceAccount) : base(budgetType, isServiceAccount)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentException("Id cannot be null or empty");
			}
			this.Id = id;
			this.cachedHashCode = (base.BudgetType.GetHashCode() ^ this.Id.GetHashCode());
			this.cachedToString = string.Format("Unthrottled~{0}~{1}", id, budgetType);
		}

		public string Id { get; private set; }

		public override bool Equals(object obj)
		{
			UnthrottledBudgetKey unthrottledBudgetKey = obj as UnthrottledBudgetKey;
			return !(unthrottledBudgetKey == null) && unthrottledBudgetKey.BudgetType == base.BudgetType && unthrottledBudgetKey.Id == this.Id;
		}

		public override string ToString()
		{
			return this.cachedToString;
		}

		public override int GetHashCode()
		{
			return this.cachedHashCode;
		}

		internal override IThrottlingPolicy InternalLookup()
		{
			return UnthrottledThrottlingPolicy.GetSingleton();
		}

		private const string ToStringFormat = "Unthrottled~{0}~{1}";

		private readonly string cachedToString;

		private readonly int cachedHashCode;
	}
}
