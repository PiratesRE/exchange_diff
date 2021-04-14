using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class NoTrailingSpecificCharacterConstraint : PropertyDefinitionConstraint
	{
		public NoTrailingSpecificCharacterConstraint(char c)
		{
			this.invalidChar = c;
		}

		public char InvalidChar
		{
			get
			{
				return this.invalidChar;
			}
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			string text = (string)value;
			if (!string.IsNullOrEmpty(text) && text[text.Length - 1] == this.invalidChar)
			{
				return new PropertyConstraintViolationError(DataStrings.ConstraintNoTrailingSpecificCharacter(text, this.invalidChar), propertyDefinition, value, this);
			}
			return null;
		}

		private char invalidChar;
	}
}
