using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class NoLeadingOrTrailingWhitespaceConstraint : PropertyDefinitionConstraint
	{
		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			string text = (value != null) ? value.ToString() : null;
			if (!string.IsNullOrEmpty(text) && (char.IsWhiteSpace(text[0]) || char.IsWhiteSpace(text[text.Length - 1])))
			{
				return new PropertyConstraintViolationError(DataStrings.ConstraintViolationNoLeadingOrTrailingWhitespace, propertyDefinition, value, this);
			}
			return null;
		}
	}
}
