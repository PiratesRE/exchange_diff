using System;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal abstract class CharacterRegexConstraint : PropertyDefinitionConstraint
	{
		protected CharacterRegexConstraint(string pattern)
		{
			if (string.IsNullOrEmpty(pattern))
			{
				throw new ArgumentNullException("pattern");
			}
			this.pattern = pattern;
		}

		public string Pattern
		{
			get
			{
				return this.pattern;
			}
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			string text = value.ToString();
			if (!string.IsNullOrEmpty(text) && !Regex.IsMatch(text, "^" + this.Pattern + "+$"))
			{
				LocalizedString description = this.CustomErrorMessage(text, propertyDefinition);
				return new PropertyConstraintViolationError(description, propertyDefinition, value, this);
			}
			return null;
		}

		protected virtual LocalizedString CustomErrorMessage(string value, PropertyDefinition propertyDefinition)
		{
			return DataStrings.ConstraintViolationStringDoesNotMatchRegularExpression(this.Pattern, value);
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			CharacterRegexConstraint characterRegexConstraint = obj as CharacterRegexConstraint;
			return characterRegexConstraint.pattern == this.pattern;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ this.pattern.GetHashCode();
		}

		private string pattern;
	}
}
