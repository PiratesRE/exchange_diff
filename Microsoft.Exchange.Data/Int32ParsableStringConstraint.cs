using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class Int32ParsableStringConstraint : PropertyDefinitionConstraint
	{
		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			int num;
			if (value == null || !int.TryParse(value.ToString(), out num))
			{
				return new PropertyConstraintViolationError(DataStrings.Int32ParsableStringConstraintViolation((value == null) ? "null" : value.ToString()), propertyDefinition, value, this);
			}
			return null;
		}
	}
}
