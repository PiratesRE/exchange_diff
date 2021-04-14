using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class StoreObjectConstraint
	{
		protected StoreObjectConstraint(ICollection<PropertyDefinition> relevantProperties)
		{
			this.relevantProperties = relevantProperties;
		}

		internal abstract StoreObjectValidationError Validate(ValidationContext context, IValidatablePropertyBag validatablePropertyBag);

		internal ICollection<PropertyDefinition> RelevantProperties
		{
			get
			{
				return this.relevantProperties;
			}
		}

		private readonly ICollection<PropertyDefinition> relevantProperties;
	}
}
