using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IValidatable
	{
		void Validate(ValidationContext context, IList<StoreObjectValidationError> validationErrors);

		bool ValidateAllProperties { get; }

		Schema Schema { get; }
	}
}
