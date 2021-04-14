using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class EnhancedTimeSpanUnitConstraint : PropertyDefinitionConstraint
	{
		public EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan unit)
		{
			if (EnhancedTimeSpan.Zero > unit)
			{
				throw new ArgumentException(DataStrings.ExceptionNegativeUnit, "unit");
			}
			this.unit = unit;
		}

		public EnhancedTimeSpan Unit
		{
			get
			{
				return this.unit;
			}
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			EnhancedTimeSpan t = (value is TimeSpan) ? ((TimeSpan)value) : ((EnhancedTimeSpan)value);
			if (EnhancedTimeSpan.Zero != t % this.unit)
			{
				return new PropertyConstraintViolationError(DataStrings.ConstraintViolationDontMatchUnit(this.unit.ToString(), t.ToString()), propertyDefinition, value, this);
			}
			return null;
		}

		private EnhancedTimeSpan unit;
	}
}
