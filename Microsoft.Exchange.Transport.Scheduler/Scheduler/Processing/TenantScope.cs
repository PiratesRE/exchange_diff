using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Scheduler.Contracts;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal sealed class TenantScope : IMessageScope, IEquatable<IMessageScope>, IEquatable<TenantScope>
	{
		public TenantScope(Guid tenantId)
		{
			ArgumentValidator.ThrowIfNull("tenantId", tenantId);
			this.tenantId = tenantId;
		}

		public MessageScopeType Type
		{
			get
			{
				return MessageScopeType.Tenant;
			}
		}

		public string Display
		{
			get
			{
				return "Tenant:" + this.Value;
			}
		}

		public object Value
		{
			get
			{
				return this.tenantId;
			}
		}

		public static bool operator ==(TenantScope left, TenantScope right)
		{
			return object.Equals(left, right);
		}

		public static bool operator !=(TenantScope left, TenantScope right)
		{
			return !object.Equals(left, right);
		}

		public bool Equals(TenantScope other)
		{
			return !object.ReferenceEquals(null, other) && (object.ReferenceEquals(this, other) || object.Equals(this.tenantId, other.tenantId));
		}

		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (!(obj.GetType() != base.GetType()) && this.Equals((TenantScope)obj)));
		}

		public override int GetHashCode()
		{
			return this.tenantId.GetHashCode();
		}

		public bool Equals(IMessageScope other)
		{
			return this.Equals(other as TenantScope);
		}

		public override string ToString()
		{
			return this.Display;
		}

		private readonly Guid tenantId;
	}
}
