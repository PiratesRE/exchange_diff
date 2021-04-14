using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public interface ISimplePropertyStorageWithChangeTracking : ISimplePropertyStorage, ISimpleReadOnlyPropertyStorage
	{
		ISimpleReadOnlyPropertyBag OriginalBag { get; }

		bool IsBlobPropertyChanged(Context context, StorePropTag propTag);

		bool IsPhysicalColumnChanged(Context context, PhysicalColumn column);

		object GetOriginalBlobPropertyValue(Context context, StorePropTag propTag);

		bool TryGetOriginalBlobProperty(Context context, ushort propId, out StorePropTag propTag, out object value);

		object GetOriginalPhysicalColumnValue(Context context, PhysicalColumn column);

		void EnumerateOriginalBlobProperties(Context context, Func<StorePropTag, object, bool> action, bool showValue);
	}
}
