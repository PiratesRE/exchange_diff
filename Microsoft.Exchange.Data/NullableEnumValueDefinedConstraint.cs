using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal sealed class NullableEnumValueDefinedConstraint : EnumValueDefinedConstraint
	{
		public NullableEnumValueDefinedConstraint(Type enumType) : base(enumType)
		{
		}

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
