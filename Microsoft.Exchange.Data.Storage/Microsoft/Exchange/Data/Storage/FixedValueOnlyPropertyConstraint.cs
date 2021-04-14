using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FixedValueOnlyPropertyConstraint : ReadOnlyPropertyConstraint
	{
		internal FixedValueOnlyPropertyConstraint(StorePropertyDefinition propertyDefinition, object fixedValue) : base(propertyDefinition)
		{
			this.fixedValue = fixedValue;
		}

		internal override StoreObjectValidationError Validate(ValidationContext context, IValidatablePropertyBag validatablePropertyBag)
		{
			if (!validatablePropertyBag.IsPropertyDirty(base.PropertyDefinition))
			{
				return null;
			}
			object obj = validatablePropertyBag.TryGetProperty(base.PropertyDefinition);
			if (this.fixedValue.Equals(obj))
			{
				return null;
			}
			return base.Validate(context, validatablePropertyBag);
		}

		public override string ToString()
		{
			return string.Format("Property {0} can be set to value {1} only.", base.PropertyDefinition, this.fixedValue);
		}

		private readonly object fixedValue;
	}
}
