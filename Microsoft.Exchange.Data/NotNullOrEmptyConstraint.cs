using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class NotNullOrEmptyConstraint : PropertyDefinitionConstraint
	{
		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			string value2 = value as string;
			if (string.IsNullOrEmpty(value2))
			{
				return new PropertyConstraintViolationError(DataStrings.ConstraintViolationValueIsNullOrEmpty, propertyDefinition, value, this);
			}
			return null;
		}
	}
}
