using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class ValidDomainConstraint : PropertyDefinitionConstraint
	{
		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			if (!SmtpAddress.IsValidDomain((string)value))
			{
				return new PropertyConstraintViolationError(DataStrings.ConstraintViolationNotValidDomain((string)value), propertyDefinition, value, this);
			}
			return null;
		}
	}
}
