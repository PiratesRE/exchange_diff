using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RequiredPropertyConstraint : StoreObjectConstraint
	{
		internal RequiredPropertyConstraint(StorePropertyDefinition propertyDefinition) : base(new PropertyDefinition[]
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
			object obj = validatablePropertyBag.TryGetProperty(this.propertyDefinition);
			if (PropertyError.IsPropertyError(obj) && !PropertyError.IsPropertyValueTooBig(obj))
			{
				return new StoreObjectValidationError(context, this.propertyDefinition, obj, this);
			}
			return null;
		}

		public override string ToString()
		{
			return string.Format("Property {0} is required.", this.propertyDefinition);
		}

		private readonly StorePropertyDefinition propertyDefinition;
	}
}
