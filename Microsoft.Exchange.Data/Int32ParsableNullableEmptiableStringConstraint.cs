using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class Int32ParsableNullableEmptiableStringConstraint : Int32ParsableStringConstraint
	{
		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			if (value == null || value.ToString().Length == 0)
			{
				return null;
			}
			return base.Validate(value, propertyDefinition, propertyBag);
		}
	}
}
