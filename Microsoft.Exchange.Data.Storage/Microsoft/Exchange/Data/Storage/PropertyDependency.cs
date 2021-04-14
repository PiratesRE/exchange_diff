using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class PropertyDependency
	{
		internal PropertyDependency(NativeStorePropertyDefinition property, PropertyDependencyType type)
		{
			this.Type = type;
			this.Property = property;
		}

		internal readonly NativeStorePropertyDefinition Property;

		internal readonly PropertyDependencyType Type;
	}
}
