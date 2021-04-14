using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class OccurrenceBagFactory : IPropertyBagFactory
	{
		internal OccurrenceBagFactory(StoreSession storeSession, OccurrenceStoreObjectId occurrenceUniqueItemId)
		{
			this.storeSession = storeSession;
			this.exTimeZone = storeSession.ExTimeZone;
			this.occurrenceUniqueItemId = occurrenceUniqueItemId;
		}

		public PersistablePropertyBag CreateStorePropertyBag(PropertyBag propertyBag, ICollection<PropertyDefinition> propsToReturn)
		{
			OccurrencePropertyBag occurrencePropertyBag = Item.CreateOccurrencePropertyBag(this.storeSession, this.occurrenceUniqueItemId, propsToReturn);
			occurrencePropertyBag.ExTimeZone = this.exTimeZone;
			return occurrencePropertyBag;
		}

		private readonly StoreSession storeSession;

		private readonly OccurrenceStoreObjectId occurrenceUniqueItemId;

		private readonly ExTimeZone exTimeZone;
	}
}
