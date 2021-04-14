using System;

namespace Microsoft.Exchange.Transport
{
	internal class TenantBasedCondition : WaitCondition
	{
		public TenantBasedCondition(Guid tenantId)
		{
			this.tenant = tenantId;
		}

		public Guid TenantId
		{
			get
			{
				return this.tenant;
			}
		}

		public override int CompareTo(object obj)
		{
			TenantBasedCondition tenantBasedCondition = obj as TenantBasedCondition;
			if (tenantBasedCondition == null)
			{
				throw new ArgumentException();
			}
			return this.tenant.CompareTo(tenantBasedCondition.tenant);
		}

		public override bool Equals(object obj)
		{
			TenantBasedCondition tenantBasedCondition = obj as TenantBasedCondition;
			return tenantBasedCondition != null && this.Equals(tenantBasedCondition);
		}

		public bool Equals(TenantBasedCondition condition)
		{
			return condition != null && this.tenant.Equals(condition.tenant);
		}

		public override int GetHashCode()
		{
			return this.tenant.GetHashCode();
		}

		public override string ToString()
		{
			return "TenantBasedCondition-" + this.tenant.ToString();
		}

		private readonly Guid tenant;
	}
}
