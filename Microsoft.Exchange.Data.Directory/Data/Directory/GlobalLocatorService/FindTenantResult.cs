using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	internal class FindTenantResult
	{
		internal FindTenantResult(IDictionary<TenantProperty, PropertyValue> properties)
		{
			this.properties = properties;
		}

		internal PropertyValue GetPropertyValue(TenantProperty property)
		{
			PropertyValue result;
			if (!this.properties.TryGetValue(property, out result))
			{
				return PropertyValue.Create(null, property);
			}
			return result;
		}

		internal bool HasProperties()
		{
			return this.properties.Count > 0;
		}

		private IDictionary<TenantProperty, PropertyValue> properties;
	}
}
