using System;
using System.Text;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal class TenantBudgetKey : LookupBudgetKey
	{
		public TenantBudgetKey(OrganizationId organizationId, BudgetType budgetType) : base(budgetType, false)
		{
			this.organizationId = organizationId;
			this.cachedToString = TenantBudgetKey.GetCachedToString(organizationId, budgetType);
			this.cachedHashCode = this.cachedToString.GetHashCode();
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return this.organizationId;
			}
		}

		public override bool Equals(object obj)
		{
			return this.InternalEquals(obj);
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
			return this.LookupPolicyByOrganizationId();
		}

		protected virtual IThrottlingPolicy LookupPolicyByOrganizationId()
		{
			return ThrottlingPolicyCache.Singleton.Get(this.OrganizationId);
		}

		protected virtual bool InternalEquals(object obj)
		{
			TenantBudgetKey tenantBudgetKey = obj as TenantBudgetKey;
			return !(tenantBudgetKey == null) && tenantBudgetKey.BudgetType == base.BudgetType && tenantBudgetKey.OrganizationId == this.OrganizationId;
		}

		protected static string GetCachedToString(OrganizationId organizationId, BudgetType budgetType)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("oid~");
			stringBuilder.Append(organizationId.ToString());
			stringBuilder.Append("~");
			stringBuilder.Append(budgetType.ToString());
			return stringBuilder.ToString();
		}

		private readonly OrganizationId organizationId;

		private readonly int cachedHashCode;

		private readonly string cachedToString;
	}
}
