using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class NotNullOrEmptyStrictConstraint : PropertyDefinitionConstraint
	{
		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			if (value == null)
			{
				return new PropertyConstraintViolationError(DataStrings.PropertyNotEmptyOrNull, propertyDefinition, value, this);
			}
			return null;
		}
	}
}
