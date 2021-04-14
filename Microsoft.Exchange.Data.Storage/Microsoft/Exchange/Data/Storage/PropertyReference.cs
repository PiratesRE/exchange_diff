using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class PropertyReference
	{
		internal PropertyReference(NativeStorePropertyDefinition usedProperty, PropertyAccess type)
		{
			this.AccessType = type;
			this.Property = usedProperty;
		}

		internal readonly NativeStorePropertyDefinition Property;

		internal readonly PropertyAccess AccessType;
	}
}
