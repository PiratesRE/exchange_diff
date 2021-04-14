using System;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public interface ISimpleReadOnlyPropertyBag : ISimpleReadOnlyPropertyStorage
	{
		object GetPropertyValue(Context context, StorePropTag propertyTag);

		bool TryGetProperty(Context context, ushort propId, out StorePropTag propTag, out object value);

		void EnumerateProperties(Context context, Func<StorePropTag, object, bool> action, bool showValue);
	}
}
