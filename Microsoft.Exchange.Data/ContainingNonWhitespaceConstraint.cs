using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class ContainingNonWhitespaceConstraint : PropertyDefinitionConstraint
	{
		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			string text = (string)value;
			if (!string.IsNullOrEmpty(text))
			{
				bool flag = true;
				for (int i = 0; i < text.Length; i++)
				{
					if (!char.IsWhiteSpace(text[i]))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					return new PropertyConstraintViolationError(DataStrings.ConstraintViolationStringDoesNotContainNonWhitespaceCharacter(text), propertyDefinition, value, this);
				}
			}
			return null;
		}
	}
}
