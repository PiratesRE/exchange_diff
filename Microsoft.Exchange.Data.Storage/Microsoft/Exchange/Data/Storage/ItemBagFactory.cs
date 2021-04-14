using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ItemBagFactory : IPropertyBagFactory
	{
		internal ItemBagFactory(StoreSession storeSession, StoreObjectId id)
		{
			this.storeSession = storeSession;
			this.exTimeZone = storeSession.ExTimeZone;
			this.id = id;
		}

		public PersistablePropertyBag CreateStorePropertyBag(PropertyBag propertyBag, ICollection<PropertyDefinition> prefetchPropertyArray)
		{
			PersistablePropertyBag persistablePropertyBag = ItemBagFactory.CreatePropertyBag(this.storeSession, this.id, prefetchPropertyArray);
			persistablePropertyBag.ExTimeZone = this.exTimeZone;
			return persistablePropertyBag;
		}

		internal static StoreObjectPropertyBag CreatePropertyBag(StoreSession storeSession, StoreObjectId id, ICollection<PropertyDefinition> prefetchPropertyArray)
		{
			MapiProp mapiProp = null;
			StoreObjectPropertyBag storeObjectPropertyBag = null;
			bool flag = false;
			StoreObjectPropertyBag result;
			try
			{
				mapiProp = storeSession.GetMapiProp(id);
				storeObjectPropertyBag = new StoreObjectPropertyBag(storeSession, mapiProp, prefetchPropertyArray);
				flag = true;
				result = storeObjectPropertyBag;
			}
			finally
			{
				if (!flag)
				{
					if (storeObjectPropertyBag != null)
					{
						storeObjectPropertyBag.Dispose();
						storeObjectPropertyBag = null;
					}
					if (mapiProp != null)
					{
						mapiProp.Dispose();
						mapiProp = null;
					}
				}
			}
			return result;
		}

		private readonly StoreSession storeSession;

		private readonly ExTimeZone exTimeZone;

		private StoreObjectId id;
	}
}
