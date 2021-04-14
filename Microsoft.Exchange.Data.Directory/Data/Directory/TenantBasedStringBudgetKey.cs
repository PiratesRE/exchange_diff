using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal class TenantBasedStringBudgetKey : LookupBudgetKey
	{
		public TenantBasedStringBudgetKey(string key, OrganizationId organizationId, bool isServiceAccount, BudgetType budgetType) : base(budgetType, isServiceAccount)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (organizationId == null)
			{
				throw new ArgumentNullException("organizationId");
			}
			this.Key = key;
			this.organizationId = organizationId;
			this.cachedToString = string.Format("TenantStr~{0}~{1}~{2}~{3}", new object[]
			{
				key,
				organizationId,
				budgetType,
				isServiceAccount
			});
			this.cachedhashCode = this.cachedToString.GetHashCode();
		}

		public string Key { get; private set; }

		public OrganizationId OrganizationId
		{
			get
			{
				return this.organizationId;
			}
		}

		public override bool Equals(object obj)
		{
			TenantBasedStringBudgetKey tenantBasedStringBudgetKey = obj as TenantBasedStringBudgetKey;
			return !(tenantBasedStringBudgetKey == null) && (tenantBasedStringBudgetKey.BudgetType == base.BudgetType && tenantBasedStringBudgetKey.Key == this.Key && tenantBasedStringBudgetKey.OrganizationId == this.OrganizationId) && tenantBasedStringBudgetKey.IsServiceAccountBudget == base.IsServiceAccountBudget;
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
			return ThrottlingPolicyCache.Singleton.Get(this.OrganizationId);
		}

		private const string ToStringFormat = "TenantStr~{0}~{1}~{2}~{3}";

		private readonly OrganizationId organizationId;

		private readonly int cachedhashCode;

		private readonly string cachedToString;
	}
}
