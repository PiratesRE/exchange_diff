using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[AttributeUsage(AttributeTargets.Field)]
	internal sealed class ConditionalStringLengthConstraintAttribute : StringLengthConstraintAttribute
	{
		internal ConditionalStringLengthConstraintAttribute(int minLength, int maxLength, CustomConstraintDelegateEnum applyConstraint) : base(minLength, maxLength)
		{
			this.applyConstraint = applyConstraint;
		}

		internal override StoreObjectConstraint GetConstraint(StorePropertyDefinition propertyDefinition)
		{
			return new OrConstraint(new StoreObjectConstraint[]
			{
				base.GetConstraint(propertyDefinition),
				new CustomConstraint(this.applyConstraint, false)
			});
		}

		private readonly CustomConstraintDelegateEnum applyConstraint;
	}
}
