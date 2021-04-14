using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class ConstraintAttribute : Attribute
	{
		internal abstract StoreObjectConstraint GetConstraint(StorePropertyDefinition propertyDefinition);
	}
}
