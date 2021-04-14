using System;
using System.IO;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public interface ISimplePropertyStorage : ISimpleReadOnlyPropertyStorage
	{
		void SetBlobProperty(Context context, StorePropTag propTag, object value);

		void SetPhysicalColumn(Context context, PhysicalColumn column, object value);

		ErrorCode OpenPhysicalColumnReadStream(Context context, PhysicalColumn column, out Stream stream);

		ErrorCode OpenPhysicalColumnWriteStream(Context context, PhysicalColumn column, out Stream stream);
	}
}
