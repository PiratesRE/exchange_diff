using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[AttributeUsage(AttributeTargets.Field)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class StringLengthConstraintAttribute : ConstraintAttribute
	{
		internal StringLengthConstraintAttribute(int minLength, int maxLength)
		{
			this.minLength = minLength;
			this.maxLength = maxLength;
		}

		internal override StoreObjectConstraint GetConstraint(StorePropertyDefinition propertyDefinition)
		{
			return new StoreObjectAdditionalPropertyConstraint(propertyDefinition, new StringLengthConstraint(this.minLength, this.maxLength));
		}

		private readonly int minLength;

		private readonly int maxLength;
	}
}
