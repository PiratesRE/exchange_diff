using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[AttributeUsage(AttributeTargets.Field)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ConditionallyReadOnlyAttribute : ConstraintAttribute
	{
		internal ConditionallyReadOnlyAttribute(CustomConstraintDelegateEnum isPropertyReadOnly)
		{
			this.isPropertyReadOnly = isPropertyReadOnly;
		}

		internal override StoreObjectConstraint GetConstraint(StorePropertyDefinition propertyDefinition)
		{
			return new OrConstraint(new StoreObjectConstraint[]
			{
				new ReadOnlyPropertyConstraint(propertyDefinition),
				new CustomConstraint(this.isPropertyReadOnly, false)
			});
		}

		private readonly CustomConstraintDelegateEnum isPropertyReadOnly;
	}
}
