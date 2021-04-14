using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[AttributeUsage(AttributeTargets.Field)]
	internal sealed class FixedValueOnlyAttribute : ConstraintAttribute
	{
		internal FixedValueOnlyAttribute(object fixedValue)
		{
			this.fixedValue = fixedValue;
		}

		internal override StoreObjectConstraint GetConstraint(StorePropertyDefinition propertyDefinition)
		{
			return new FixedValueOnlyPropertyConstraint(propertyDefinition, this.fixedValue);
		}

		private readonly object fixedValue;
	}
}
