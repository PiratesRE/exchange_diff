using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class PropertyBag
	{
		public bool Contains(PropertyDefinition propertyDefinition)
		{
			ArgumentValidator.ThrowIfNull("propertyDefinition", propertyDefinition);
			return this.properties.ContainsKey(propertyDefinition);
		}

		public object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				ArgumentValidator.ThrowIfNull("propertyDefinition", propertyDefinition);
				return this.properties[propertyDefinition];
			}
			set
			{
				ArgumentValidator.ThrowIfNull("propertyDefinition", propertyDefinition);
				this.properties[propertyDefinition] = value;
			}
		}

		public bool TryGetValue(PropertyDefinition propertyDefinition, out object value)
		{
			value = null;
			return this.properties.TryGetValue(propertyDefinition, out value);
		}

		public PropertyDefinition[] GetProperties()
		{
			return this.properties.Keys.ToArray<PropertyDefinition>();
		}

		private Dictionary<PropertyDefinition, object> properties = new Dictionary<PropertyDefinition, object>();
	}
}
