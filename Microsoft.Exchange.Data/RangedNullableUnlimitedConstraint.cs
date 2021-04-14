using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class RangedNullableUnlimitedConstraint<T> : RangedUnlimitedConstraint<T> where T : struct, IComparable
	{
		public RangedNullableUnlimitedConstraint(T minValue, T maxValue) : base(minValue, maxValue)
		{
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			if (value == null)
			{
				return null;
			}
			return base.Validate((Unlimited<T>)value, propertyDefinition, propertyBag);
		}
	}
}
