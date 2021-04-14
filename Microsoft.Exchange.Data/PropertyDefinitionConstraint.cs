using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal abstract class PropertyDefinitionConstraint
	{
		public abstract PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag);

		public virtual PropertyConstraintViolationError Validate(ExchangeOperationContext operationContext, object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			return this.Validate(value, propertyDefinition, propertyBag);
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj.GetType() == base.GetType();
		}

		public override int GetHashCode()
		{
			return base.GetType().GetHashCode();
		}

		public static PropertyDefinitionConstraint[] None = new PropertyDefinitionConstraint[0];
	}
}
