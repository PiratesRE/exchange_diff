using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReadOnlyPropertyConstraint : StoreObjectConstraint
	{
		internal ReadOnlyPropertyConstraint(StorePropertyDefinition propertyDefinition) : base(new PropertyDefinition[]
		{
			propertyDefinition
		})
		{
			this.propertyDefinition = propertyDefinition;
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
			if (validatablePropertyBag.IsPropertyDirty(this.propertyDefinition))
			{
				return new StoreObjectValidationError(context, this.propertyDefinition, validatablePropertyBag.TryGetProperty(this.propertyDefinition), this);
			}
			return null;
		}

		public override string ToString()
		{
			return string.Format("Property {0} is read-only.", this.propertyDefinition);
		}

		private readonly StorePropertyDefinition propertyDefinition;
	}
}
