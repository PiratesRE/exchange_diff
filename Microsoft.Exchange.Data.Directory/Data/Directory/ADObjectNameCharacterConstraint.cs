using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal sealed class ADObjectNameCharacterConstraint : CharacterConstraint
	{
		public ADObjectNameCharacterConstraint(char[] invalidCharacters) : base(invalidCharacters, false)
		{
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			string text = (string)value;
			int num = -1;
			if (!string.IsNullOrEmpty(text) && ADObjectNameHelper.CheckIsUnicodeStringWellFormed(text, out num))
			{
				if (ADObjectNameHelper.ReservedADNameStringRegex.IsMatch(text))
				{
					return null;
				}
				return base.Validate(value, propertyDefinition, propertyBag);
			}
			else
			{
				if (num == -1)
				{
					return new PropertyConstraintViolationError(DataStrings.ConstraintViolationStringLengthIsEmpty, propertyDefinition, value, this);
				}
				return new PropertyConstraintViolationError(DataStrings.ConstraintViolationStringContainsInvalidCharacters(text.Substring(num, 1), text), propertyDefinition, value, this);
			}
		}
	}
}
