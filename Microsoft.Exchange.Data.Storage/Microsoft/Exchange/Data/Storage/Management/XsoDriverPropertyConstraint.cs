using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class XsoDriverPropertyConstraint : PropertyDefinitionConstraint
	{
		public StorePropertyDefinition StorePropertyDefinition { get; private set; }

		public XsoDriverPropertyConstraint(StorePropertyDefinition storePropertyDefinition)
		{
			if (storePropertyDefinition == null)
			{
				throw new ArgumentNullException("storePropertyDefinition");
			}
			this.StorePropertyDefinition = storePropertyDefinition;
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			if (value == null)
			{
				return null;
			}
			PropertyValidationError[] array = this.StorePropertyDefinition.Validate(null, value);
			if (array == null || array.Length == 0)
			{
				return null;
			}
			return new PropertyConstraintViolationError(array[0].Description, propertyDefinition, value, this);
		}
	}
}
