using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class StringLengthConstraint : PropertyDefinitionConstraint
	{
		public StringLengthConstraint(int minLength, int maxLength)
		{
			if (minLength > maxLength && maxLength != 0)
			{
				throw new ArgumentException("minLength > maxLength");
			}
			if (minLength < 0)
			{
				throw new ArgumentException("minLength < 0");
			}
			this.minLength = minLength;
			this.maxLength = maxLength;
		}

		public int MinLength
		{
			get
			{
				return this.minLength;
			}
		}

		public int MaxLength
		{
			get
			{
				return this.maxLength;
			}
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			if (value != null)
			{
				string text = value.ToString();
				if (text.Length < this.minLength)
				{
					return new PropertyConstraintViolationError(DataStrings.ConstraintViolationStringLengthTooShort(this.minLength, text.Length), propertyDefinition, value, this);
				}
				if (this.maxLength != 0 && text.Length > this.maxLength)
				{
					return new PropertyConstraintViolationError(DataStrings.ConstraintViolationStringLengthTooLong(this.maxLength, text.Length), propertyDefinition, value, this);
				}
			}
			return null;
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			StringLengthConstraint stringLengthConstraint = obj as StringLengthConstraint;
			return stringLengthConstraint.MinLength == this.MinLength && stringLengthConstraint.MaxLength == this.MaxLength;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ this.MinLength ^ this.MaxLength;
		}

		private int minLength;

		private int maxLength;
	}
}
