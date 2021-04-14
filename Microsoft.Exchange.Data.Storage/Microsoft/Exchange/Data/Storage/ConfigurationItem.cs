using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ConfigurationItem : Item, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal ConfigurationItem(ICoreItem coreItem) : base(coreItem, false)
		{
		}

		internal new static ConfigurationItem Bind(StoreSession session, StoreId id)
		{
			return ItemBuilder.ItemBind<ConfigurationItem>(session, id, ConfigurationItemSchema.Instance, null);
		}
	}
}
