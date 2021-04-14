using System;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class RegexConstraint : PropertyDefinitionConstraint
	{
		public RegexConstraint(string pattern, LocalizedString patternDescription) : this(pattern, RegexOptions.None, patternDescription)
		{
		}

		public RegexConstraint(string pattern, RegexOptions options, LocalizedString patternDescription)
		{
			this.pattern = pattern;
			this.options = options;
			this.patternDescription = patternDescription;
		}

		public string Pattern
		{
			get
			{
				return this.pattern;
			}
		}

		public RegexOptions Options
		{
			get
			{
				return this.options;
			}
		}

		public LocalizedString PatternDescription
		{
			get
			{
				return this.patternDescription;
			}
		}

		private Regex Constraint
		{
			get
			{
				if (this.constraint == null)
				{
					this.constraint = new Regex(this.pattern, this.options);
				}
				return this.constraint;
			}
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			try
			{
				string text = (string)value;
				if (text != null && !this.Constraint.IsMatch(text))
				{
					return new PropertyConstraintViolationError(DataStrings.ConstraintViolationStringDoesNotMatchRegularExpression(this.patternDescription.ToString(), text), propertyDefinition, value, this);
				}
			}
			catch (OutOfMemoryException)
			{
				return new PropertyConstraintViolationError(DataStrings.ConstraintViolationStringLengthCauseOutOfMemory, propertyDefinition, null, this);
			}
			return null;
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			RegexConstraint regexConstraint = obj as RegexConstraint;
			return StringComparer.OrdinalIgnoreCase.Equals(regexConstraint.pattern, this.pattern) && regexConstraint.options == this.options;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private string pattern;

		private RegexOptions options;

		[NonSerialized]
		private Regex constraint;

		private LocalizedString patternDescription;
	}
}
