using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class StoreObjectAdditionalPropertyConstraint : StoreObjectConstraint
	{
		internal StoreObjectAdditionalPropertyConstraint(StorePropertyDefinition propertyDefinition, PropertyDefinitionConstraint constraint) : base(new PropertyDefinition[]
		{
			propertyDefinition
		})
		{
			this.propertyDefinition = propertyDefinition;
			this.constraint = constraint;
		}

		public PropertyDefinitionConstraint Constraint
		{
			get
			{
				return this.constraint;
			}
		}

		public PropertyDefinition PropertyDefinition
		{
			get
			{
				return this.propertyDefinition;
			}
		}

		internal override StoreObjectValidationError Validate(ValidationContext context, IValidatablePropertyBag validatablePropertyBag)
		{
			object obj = validatablePropertyBag.TryGetProperty(this.propertyDefinition);
			if (!PropertyError.IsPropertyNotFound(obj))
			{
				PropertyConstraintViolationError propertyConstraintViolationError = this.constraint.Validate(obj, this.propertyDefinition, null);
				if (propertyConstraintViolationError != null)
				{
					return new StoreObjectValidationError(context, this.propertyDefinition, obj, this);
				}
			}
			return null;
		}

		public override string ToString()
		{
			return string.Format("For objects of this type, property {0} has the additional constraint {1}.", this.propertyDefinition, this.constraint);
		}

		private readonly PropertyDefinitionConstraint constraint;

		private readonly StorePropertyDefinition propertyDefinition;
	}
}
