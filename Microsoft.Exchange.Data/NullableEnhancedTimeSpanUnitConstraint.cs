using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class NullableEnhancedTimeSpanUnitConstraint : EnhancedTimeSpanUnitConstraint
	{
		public NullableEnhancedTimeSpanUnitConstraint(EnhancedTimeSpan unit) : base(unit)
		{
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			if (value != null)
			{
				return base.Validate(value, propertyDefinition, propertyBag);
			}
			return null;
		}
	}
}
