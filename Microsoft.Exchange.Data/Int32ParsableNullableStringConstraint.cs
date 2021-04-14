using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class Int32ParsableNullableStringConstraint : Int32ParsableStringConstraint
	{
		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			if (value == null)
			{
				return null;
			}
			return base.Validate(value, propertyDefinition, propertyBag);
		}
	}
}
