using System;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public interface ISimplePropertyBagWithChangeTracking : ISimplePropertyBag, ISimpleReadOnlyPropertyBag, ISimplePropertyStorageWithChangeTracking, ISimplePropertyStorage, ISimpleReadOnlyPropertyStorage
	{
		bool IsPropertyChanged(Context context, StorePropTag propTag);

		object GetOriginalPropertyValue(Context context, StorePropTag propTag);
	}
}
