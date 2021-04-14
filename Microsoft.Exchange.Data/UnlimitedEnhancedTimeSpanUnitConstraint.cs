using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class UnlimitedEnhancedTimeSpanUnitConstraint : EnhancedTimeSpanUnitConstraint
	{
		public UnlimitedEnhancedTimeSpanUnitConstraint(EnhancedTimeSpan unit) : base(unit)
		{
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			object value2 = null;
			bool isUnlimited;
			if (value is Unlimited<TimeSpan>)
			{
				isUnlimited = ((Unlimited<TimeSpan>)value).IsUnlimited;
				if (!isUnlimited)
				{
					value2 = ((Unlimited<TimeSpan>)value).Value;
				}
			}
			else
			{
				isUnlimited = ((Unlimited<EnhancedTimeSpan>)value).IsUnlimited;
				if (!isUnlimited)
				{
					value2 = ((Unlimited<EnhancedTimeSpan>)value).Value;
				}
			}
			if (!isUnlimited)
			{
				return base.Validate(value2, propertyDefinition, propertyBag);
			}
			return null;
		}
	}
}
