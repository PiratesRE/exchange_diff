using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class RangedUnlimitedConstraint<T> : RangedValueConstraint<T> where T : struct, IComparable
	{
		public RangedUnlimitedConstraint(T minValue, T maxValue) : base(minValue, maxValue)
		{
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			Unlimited<T> unlimited = (Unlimited<T>)value;
			if (unlimited.IsUnlimited)
			{
				return null;
			}
			return base.Validate(unlimited.Value, propertyDefinition, propertyBag);
		}
	}
}
