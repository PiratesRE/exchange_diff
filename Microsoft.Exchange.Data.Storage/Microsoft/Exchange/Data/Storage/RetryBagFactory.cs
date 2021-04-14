using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RetryBagFactory : IPropertyBagFactory
	{
		internal RetryBagFactory(StoreSession storeSession)
		{
			this.storeSession = storeSession;
			this.exTimeZone = storeSession.ExTimeZone;
		}

		public PersistablePropertyBag CreateStorePropertyBag(PropertyBag propertyBag, ICollection<PropertyDefinition> prefetchProperties)
		{
			byte[] entryId = propertyBag.TryGetProperty(InternalSchema.EntryId) as byte[];
			StoreObjectId id = StoreObjectId.FromProviderSpecificId(entryId, StoreObjectType.Unknown);
			StoreObjectPropertyBag storeObjectPropertyBag = propertyBag as StoreObjectPropertyBag;
			ICollection<PropertyDefinition> prefetchPropertyArray;
			if (storeObjectPropertyBag != null)
			{
				prefetchPropertyArray = ((prefetchProperties != null) ? prefetchProperties.Union(storeObjectPropertyBag.PrefetchPropertyArray) : storeObjectPropertyBag.PrefetchPropertyArray);
			}
			else
			{
				prefetchPropertyArray = prefetchProperties;
			}
			PersistablePropertyBag persistablePropertyBag = ItemBagFactory.CreatePropertyBag(this.storeSession, id, prefetchPropertyArray);
			persistablePropertyBag.ExTimeZone = this.exTimeZone;
			return persistablePropertyBag;
		}

		private readonly StoreSession storeSession;

		private readonly ExTimeZone exTimeZone;
	}
}
