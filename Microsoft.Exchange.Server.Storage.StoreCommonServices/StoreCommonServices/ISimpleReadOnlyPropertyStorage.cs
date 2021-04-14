using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public interface ISimpleReadOnlyPropertyStorage
	{
		object GetBlobPropertyValue(Context context, StorePropTag propTag);

		object GetPhysicalColumnValue(Context context, PhysicalColumn column);

		bool TryGetBlobProperty(Context context, ushort propId, out StorePropTag propTag, out object value);

		void EnumerateBlobProperties(Context context, Func<StorePropTag, object, bool> action, bool showValue);
	}
}
