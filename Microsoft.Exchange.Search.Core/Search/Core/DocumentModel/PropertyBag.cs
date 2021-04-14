using System;
using System.Collections.Generic;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Search.Core.DocumentModel
{
	internal class PropertyBag : IPropertyBag, IReadOnlyPropertyBag
	{
		internal PropertyBag()
		{
			this.values = new Dictionary<PropertyDefinition, object>(128);
		}

		public IEnumerable<KeyValuePair<PropertyDefinition, object>> Values
		{
			get
			{
				return this.values;
			}
		}

		public TPropertyValue GetProperty<TPropertyValue>(PropertyDefinition property)
		{
			object obj;
			if (!this.TryGetProperty(property, out obj))
			{
				throw new PropertyErrorException(property.Name);
			}
			if (!typeof(TPropertyValue).IsAssignableFrom(obj.GetType()))
			{
				throw new PropertyTypeErrorException(property.Name);
			}
			return (TPropertyValue)((object)obj);
		}

		public bool TryGetProperty(PropertyDefinition property, out object value)
		{
			Util.ThrowOnNullArgument(property, "property");
			return this.values.TryGetValue(property, out value);
		}

		public void SetProperty<TPropertyValue>(PropertyDefinition property, TPropertyValue value)
		{
			this.InternalValidateSetProperty(property, value);
			this.InternalValidatePropertyValueType(property, typeof(TPropertyValue));
			this.values[property] = value;
		}

		public void SetProperty(PropertyDefinition property, object value)
		{
			this.InternalValidateSetProperty(property, value);
			this.InternalValidatePropertyValueType(property, value.GetType());
			this.values[property] = value;
		}

		private void InternalValidateSetProperty(PropertyDefinition property, object value)
		{
			Util.ThrowOnNullArgument(property, "property");
			Util.ThrowOnNullArgument(value, "value");
		}

		private void InternalValidatePropertyValueType(PropertyDefinition property, Type valueType)
		{
			Type type = property.Type;
			if (!type.Equals(valueType) && !type.IsAssignableFrom(valueType))
			{
				throw new ArgumentException();
			}
		}

		private const int IntialCapacity = 128;

		private Dictionary<PropertyDefinition, object> values;
	}
}
