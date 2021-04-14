using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class ValidLegacyDNConstraint : PropertyDefinitionConstraint
	{
		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			if (!LegacyDN.IsValidLegacyDN((string)value))
			{
				return new PropertyConstraintViolationError(DirectoryStrings.ConstraintViolationNotValidLegacyDN, propertyDefinition, value, this);
			}
			return null;
		}
	}
}
