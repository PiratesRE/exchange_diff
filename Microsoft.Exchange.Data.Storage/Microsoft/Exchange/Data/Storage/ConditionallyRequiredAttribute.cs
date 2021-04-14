using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[AttributeUsage(AttributeTargets.Field)]
	internal sealed class ConditionallyRequiredAttribute : ConstraintAttribute
	{
		internal static StoreObjectConstraint GetConstraint(StorePropertyDefinition propertyDefinition, CustomConstraintDelegateEnum isPropertyRequired)
		{
			return new OrConstraint(new StoreObjectConstraint[]
			{
				new RequiredPropertyConstraint(propertyDefinition),
				new CustomConstraint(isPropertyRequired, false)
			});
		}

		internal ConditionallyRequiredAttribute(CustomConstraintDelegateEnum isPropertyRequired)
		{
			this.isPropertyRequired = isPropertyRequired;
		}

		internal override StoreObjectConstraint GetConstraint(StorePropertyDefinition propertyDefinition)
		{
			return ConditionallyRequiredAttribute.GetConstraint(propertyDefinition, this.isPropertyRequired);
		}

		private readonly CustomConstraintDelegateEnum isPropertyRequired;
	}
}
