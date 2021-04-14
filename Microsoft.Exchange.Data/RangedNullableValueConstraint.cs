using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class RangedNullableValueConstraint<T> : RangedValueConstraint<T> where T : struct, IComparable
	{
		public RangedNullableValueConstraint(T minValue, T maxValue) : base(minValue, maxValue)
		{
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			if (value == null)
			{
				return null;
			}
			return base.Validate((T)((object)value), propertyDefinition, propertyBag);
		}
	}
}
