using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	internal class FindUserResult
	{
		internal FindUserResult(string msaUserMemberName, Guid tenantId, IDictionary<TenantProperty, PropertyValue> tenantProperties)
		{
			this.MSAUserMemberName = msaUserMemberName;
			this.tenantId = tenantId;
			this.tenantProperties = tenantProperties;
		}

		internal Guid TenantId
		{
			get
			{
				return this.tenantId;
			}
		}

		internal PropertyValue GetTenantPropertyValue(TenantProperty tenantProperty)
		{
			PropertyValue result;
			if (!this.tenantProperties.TryGetValue(tenantProperty, out result))
			{
				return PropertyValue.Create(null, tenantProperty);
			}
			return result;
		}

		internal bool HasTenantProperties()
		{
			return this.tenantProperties.Count > 0;
		}

		private readonly Guid tenantId;

		private IDictionary<TenantProperty, PropertyValue> tenantProperties;

		internal readonly string MSAUserMemberName;
	}
}
