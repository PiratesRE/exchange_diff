using System;
using System.IO;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public delegate ErrorCode StreamGetterDelegate(Context context, ISimplePropertyBag bag, out Stream stream);
}
