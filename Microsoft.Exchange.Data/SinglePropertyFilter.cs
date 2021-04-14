using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal abstract class SinglePropertyFilter : QueryFilter
	{
		public SinglePropertyFilter(PropertyDefinition property)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			this.property = property;
		}

		public PropertyDefinition Property
		{
			get
			{
				return this.property;
			}
		}

		internal override IEnumerable<PropertyDefinition> FilterProperties()
		{
			return new List<PropertyDefinition>(1)
			{
				this.Property
			};
		}

		public override bool Equals(object obj)
		{
			SinglePropertyFilter singlePropertyFilter = obj as SinglePropertyFilter;
			return singlePropertyFilter != null && singlePropertyFilter.GetType() == base.GetType() && this.property.Equals(singlePropertyFilter.property);
		}

		public abstract SinglePropertyFilter CloneWithAnotherProperty(PropertyDefinition property);

		public override int GetHashCode()
		{
			return base.GetType().GetHashCode() ^ this.property.GetHashCode();
		}

		public override QueryFilter CloneWithPropertyReplacement(IDictionary<PropertyDefinition, PropertyDefinition> propertyMap)
		{
			return this.CloneWithAnotherProperty(propertyMap[this.Property]);
		}

		protected void CheckClonable(PropertyDefinition targetProperty)
		{
			if (targetProperty.Type == typeof(SmtpAddress))
			{
				return;
			}
			if (this.Property.Type == typeof(string))
			{
				targetProperty.Type == typeof(MultiValuedProperty<string>);
			}
		}

		private readonly PropertyDefinition property;
	}
}
