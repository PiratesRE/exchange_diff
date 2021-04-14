using System;
using System.Collections;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal abstract class CollectionPropertyDefinitionConstraint : PropertyDefinitionConstraint
	{
		public sealed override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			IEnumerable enumerable = value as IEnumerable;
			if (enumerable == null)
			{
				throw new ArgumentException(DataStrings.PropertyNotACollection((value == null) ? "<null>" : value.GetType().Name));
			}
			return this.Validate(enumerable, propertyDefinition, propertyBag);
		}

		public abstract PropertyConstraintViolationError Validate(IEnumerable collection, PropertyDefinition propertyDefinition, IPropertyBag propertyBag);
	}
}
