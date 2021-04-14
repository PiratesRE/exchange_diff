using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal abstract class AtomicStorePropertyDefinition : StorePropertyDefinition
	{
		protected AtomicStorePropertyDefinition(PropertyTypeSpecifier propertyTypeSpecifier, string displayName, Type type, PropertyFlags childFlags, PropertyDefinitionConstraint[] constraints) : base(propertyTypeSpecifier, displayName, type, childFlags, constraints)
		{
		}

		protected override bool InternalIsDirty(PropertyBag.BasicPropertyStore propertyBag)
		{
			return propertyBag.IsDirty(this);
		}
	}
}
