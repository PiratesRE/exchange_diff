using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class DelegateConstraint : PropertyDefinitionConstraint
	{
		public DelegateConstraint(ValidationDelegate validator)
		{
			this.validator = validator;
		}

		public ValidationDelegate Delegate
		{
			get
			{
				return this.validator;
			}
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			return this.Delegate(value, propertyDefinition, propertyBag, this);
		}

		[NonSerialized]
		private ValidationDelegate validator;
	}
}
