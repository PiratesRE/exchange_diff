using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class ValidSmtpAddressConstraint : PropertyDefinitionConstraint
	{
		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			SmtpAddress value2 = SmtpAddress.Empty;
			if (value is SmtpAddress)
			{
				value2 = (SmtpAddress)value;
			}
			else if (value is string)
			{
				value2 = (SmtpAddress)((string)value);
			}
			if (value2 != SmtpAddress.Empty && !value2.IsValidAddress)
			{
				return new PropertyConstraintViolationError(DataStrings.ConstraintViolationNotValidEmailAddress(value2.ToString()), propertyDefinition, value, this);
			}
			return null;
		}
	}
}
