using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PropertyBag : IPropertyBag
	{
		public IEnumerable<ContextProperty> AllProperties
		{
			get
			{
				return this.propertyBag.Keys;
			}
		}

		public bool TryGet(ContextProperty property, out object value)
		{
			if (this.propertyBag.TryGetValue(property, out value))
			{
				return true;
			}
			if (property.TryGetDefaultValue(out value))
			{
				this.propertyBag.Add(property, value);
				return true;
			}
			return false;
		}

		public void Set(ContextProperty property, object value)
		{
			if (value == null && property.Type.IsValueType && property.Type.IsGenericType && property.Type.GetGenericTypeDefinition() != typeof(Nullable<>))
			{
				throw new InvalidCastException(string.Format("Cannot assign null to a property of a non-nullable value type {0}", property.Type));
			}
			if (value != null && !property.Type.IsAssignableFrom(value.GetType()))
			{
				value = Convert.ChangeType(value, property.Type);
			}
			this.propertyBag[property] = value;
		}

		private readonly IDictionary<ContextProperty, object> propertyBag = new Dictionary<ContextProperty, object>();
	}
}
