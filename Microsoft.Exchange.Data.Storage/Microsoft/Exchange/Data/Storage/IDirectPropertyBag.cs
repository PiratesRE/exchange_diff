using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IDirectPropertyBag
	{
		PropertyBagContext Context { get; }

		bool IsNew { get; }

		void SetValue(StorePropertyDefinition propertyDefinition, object propertyValue);

		object GetValue(StorePropertyDefinition propertyDefinition);

		void Delete(StorePropertyDefinition propertyDefinition);

		bool IsLoaded(NativeStorePropertyDefinition propertyDefinition);

		bool IsDirty(AtomicStorePropertyDefinition propertyDefinition);
	}
}
