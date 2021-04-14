using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	internal class FindDomainResult
	{
		internal FindDomainResult(string domain, Guid tenantId, IDictionary<TenantProperty, PropertyValue> tenantProperties, IDictionary<DomainProperty, PropertyValue> domainProperties)
		{
			this.Domain = domain;
			this.tenantId = tenantId;
			this.tenantProperties = tenantProperties;
			this.domainProperties = domainProperties;
		}

		internal Guid TenantId
		{
			get
			{
				return this.tenantId;
			}
		}

		internal PropertyValue GetTenantPropertyValue(TenantProperty property)
		{
			PropertyValue result;
			if (!this.tenantProperties.TryGetValue(property, out result))
			{
				return PropertyValue.Create(null, property);
			}
			return result;
		}

		internal PropertyValue GetDomainPropertyValue(DomainProperty property)
		{
			PropertyValue result;
			if (!this.domainProperties.TryGetValue(property, out result))
			{
				return PropertyValue.Create(null, property);
			}
			return result;
		}

		internal bool HasDomainProperties()
		{
			return this.domainProperties.Count > 0;
		}

		internal bool HasTenantProperties()
		{
			return this.tenantProperties.Count > 0;
		}

		private readonly Guid tenantId;

		private IDictionary<DomainProperty, PropertyValue> domainProperties;

		private IDictionary<TenantProperty, PropertyValue> tenantProperties;

		internal readonly string Domain;
	}
}
