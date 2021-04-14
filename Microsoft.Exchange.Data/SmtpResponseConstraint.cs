using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class SmtpResponseConstraint : PropertyDefinitionConstraint
	{
		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			string text = value.ToString();
			if (string.IsNullOrEmpty(text))
			{
				return new PropertyConstraintViolationError(DataStrings.ConstraintViolationValueIsNullOrEmpty, propertyDefinition, value, this);
			}
			for (int i = 0; i < text.Length; i++)
			{
				if (SmtpResponseConstraint.IsIllegal(text[i]) || SmtpResponseConstraint.IsBareCR(text, i) || SmtpResponseConstraint.IsBareLF(text, i))
				{
					return new PropertyConstraintViolationError(DataStrings.SmtpResponseConstraintViolation(propertyDefinition.Name, text), propertyDefinition, value, this);
				}
			}
			return null;
		}

		private static bool IsIllegal(char ch)
		{
			return ch < '\0' || ch > 'Ā' || (char.IsControl(ch) && ch != '\r' && ch != '\n');
		}

		private static bool IsBareCR(string s, int index)
		{
			return s[index] == '\r' && index + 1 < s.Length && s[index + 1] != '\n';
		}

		private static bool IsBareLF(string s, int index)
		{
			return s[index] == '\n' && index > 0 && s[index - 1] != '\r';
		}
	}
}
