using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal delegate bool IsObjectValidDelegate(ValidationContext context, IValidatablePropertyBag validatablePropertyBag);
}
