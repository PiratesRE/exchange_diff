using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class UIImpactStringLengthConstraint : PropertyDefinitionConstraint
	{
		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			return null;
		}

		public UIImpactStringLengthConstraint(int minLength, int maxLength)
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

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			UIImpactStringLengthConstraint uiimpactStringLengthConstraint = obj as UIImpactStringLengthConstraint;
			return uiimpactStringLengthConstraint.MinLength == this.MinLength && uiimpactStringLengthConstraint.MaxLength == this.MaxLength;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ this.MinLength ^ this.MaxLength;
		}

		private int minLength;

		private int maxLength;
	}
}
