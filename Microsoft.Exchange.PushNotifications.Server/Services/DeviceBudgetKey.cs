using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Server.Services
{
	internal class DeviceBudgetKey : LookupBudgetKey
	{
		public DeviceBudgetKey(string deviceId, OrganizationId tenantId) : this(deviceId, tenantId, ThrottlingPolicyCache.Singleton)
		{
		}

		internal DeviceBudgetKey(string deviceId, OrganizationId tenantId, ThrottlingPolicyCache throttlingPolicyCache) : base(BudgetType.PushNotificationTenant, false)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("deviceId", deviceId);
			ArgumentValidator.ThrowIfNull("tenantId", tenantId);
			this.tenantId = tenantId;
			this.deviceId = deviceId;
			this.toString = DeviceBudgetKey.ToString(deviceId, tenantId);
			this.getHashCode = this.toString.GetHashCode();
			this.throttlingPolicyCache = throttlingPolicyCache;
		}

		public override bool Equals(object obj)
		{
			DeviceBudgetKey deviceBudgetKey = obj as DeviceBudgetKey;
			return !(deviceBudgetKey == null) && (object.ReferenceEquals(deviceBudgetKey, this) || (deviceBudgetKey.tenantId.Equals(this.tenantId) && deviceBudgetKey.deviceId.Equals(this.deviceId, StringComparison.OrdinalIgnoreCase)));
		}

		public override string ToString()
		{
			return this.toString;
		}

		public override int GetHashCode()
		{
			return this.getHashCode;
		}

		internal override IThrottlingPolicy InternalLookup()
		{
			return base.ADRetryLookup(() => this.throttlingPolicyCache.Get(this.tenantId));
		}

		private static string ToString(string deviceId, OrganizationId tenantId)
		{
			return string.Format("deviceId~{0}~tenantId~{1}", deviceId, tenantId);
		}

		private readonly string deviceId;

		private readonly OrganizationId tenantId;

		private readonly ThrottlingPolicyCache throttlingPolicyCache;

		private readonly string toString;

		private readonly int getHashCode;
	}
}
