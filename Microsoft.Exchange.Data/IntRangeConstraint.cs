using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal sealed class IntRangeConstraint : PropertyDefinitionConstraint
	{
		public IntRangeConstraint(int minValue, int maxValue)
		{
			this.minimumValue = minValue;
			this.maximumValue = maxValue;
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			IntRange intRange = value as IntRange;
			if (intRange != null && (intRange.LowerBound < this.minimumValue || intRange.UpperBound > this.maximumValue))
			{
				return new PropertyConstraintViolationError(DataStrings.ConstraintViolationInvalidIntRange(this.minimumValue, this.maximumValue, intRange.ToString()), propertyDefinition, value, this);
			}
			return null;
		}

		private readonly int minimumValue;

		private readonly int maximumValue;
	}
}
