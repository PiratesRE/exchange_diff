using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class MandatoryStringLengthConstraint : StringLengthConstraint
	{
		public MandatoryStringLengthConstraint(int minLength, int maxLength) : base(minLength, maxLength)
		{
			if (minLength < 1)
			{
				throw new ArgumentException("minLength < 1");
			}
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			int num;
			if (value == null)
			{
				num = 0;
			}
			else
			{
				num = value.ToString().Length;
			}
			if (num == 0)
			{
				return new PropertyConstraintViolationError(DataStrings.ConstraintViolationStringLengthIsEmpty, propertyDefinition, value, this);
			}
			return base.Validate(value, propertyDefinition, propertyBag);
		}
	}
}
