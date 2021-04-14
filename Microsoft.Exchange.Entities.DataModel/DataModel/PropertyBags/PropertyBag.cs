using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Entities.DataModel.PropertyBags
{
	[DataContract]
	internal struct PropertyBag : IPropertyChangeTracker<PropertyDefinition>
	{
		public static PropertyBag CreateInstance()
		{
			return new PropertyBag
			{
				propertyValues = new Dictionary<string, object>()
			};
		}

		public bool IsPropertySet(PropertyDefinition property)
		{
			return this.propertyValues.ContainsKey(property.Name);
		}

		public TValue GetValueOrDefault<TValue>(TypedPropertyDefinition<TValue> typedProperty)
		{
			object obj;
			if (!this.propertyValues.TryGetValue(typedProperty.Name, out obj))
			{
				return typedProperty.DefaultValue;
			}
			return (TValue)((object)obj);
		}

		public void SetValue<TValue>(TypedPropertyDefinition<TValue> typedProperty, TValue value)
		{
			this.propertyValues[typedProperty.Name] = value;
		}

		[DataMember]
		private Dictionary<string, object> propertyValues;
	}
}
