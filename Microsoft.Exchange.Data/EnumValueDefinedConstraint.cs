using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class EnumValueDefinedConstraint : PropertyDefinitionConstraint
	{
		public EnumValueDefinedConstraint(Type enumType)
		{
			this.enumType = enumType;
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			if (!Enum.IsDefined(this.enumType, value))
			{
				return new PropertyConstraintViolationError(DataStrings.ConstraintViolationEnumValueNotDefined(value.ToString(), propertyDefinition.Type.Name), propertyDefinition, value, this);
			}
			return null;
		}

		private Type enumType;
	}
}
