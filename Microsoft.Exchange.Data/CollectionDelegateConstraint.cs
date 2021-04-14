using System;
using System.Collections;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class CollectionDelegateConstraint : CollectionPropertyDefinitionConstraint
	{
		public CollectionDelegateConstraint(CollectionValidationDelegate validator)
		{
			this.validator = validator;
		}

		public CollectionValidationDelegate Delegate
		{
			get
			{
				return this.validator;
			}
		}

		public override PropertyConstraintViolationError Validate(IEnumerable collection, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			return this.Delegate(collection, propertyDefinition, propertyBag, this);
		}

		[NonSerialized]
		private CollectionValidationDelegate validator;
	}
}
