using System;
using System.IO;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public interface ISimplePropertyBag : ISimpleReadOnlyPropertyBag, ISimplePropertyStorage, ISimpleReadOnlyPropertyStorage
	{
		IReplidGuidMap ReplidGuidMap { get; }

		ErrorCode SetProperty(Context context, StorePropTag propTag, object value);

		ErrorCode OpenPropertyReadStream(Context context, StorePropTag propTag, out Stream stream);

		ErrorCode OpenPropertyWriteStream(Context context, StorePropTag propTag, out Stream stream);
	}
}
