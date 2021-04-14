using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OrConstraint : CompositeConstraint
	{
		internal OrConstraint(params StoreObjectConstraint[] constraints) : base(constraints)
		{
		}

		internal override StoreObjectValidationError Validate(ValidationContext context, IValidatablePropertyBag validatablePropertyBag)
		{
			StoreObjectValidationError storeObjectValidationError = null;
			foreach (StoreObjectConstraint storeObjectConstraint in base.Constraints)
			{
				StoreObjectValidationError storeObjectValidationError2 = storeObjectConstraint.Validate(context, validatablePropertyBag);
				if (storeObjectValidationError2 == null)
				{
					return null;
				}
				if (storeObjectValidationError == null)
				{
					storeObjectValidationError = storeObjectValidationError2;
				}
			}
			return new StoreObjectValidationError(context, storeObjectValidationError.PropertyDefinition, storeObjectValidationError.InvalidData, this);
		}

		protected override string CompositionTypeDescription
		{
			get
			{
				return "OR";
			}
		}
	}
}
