using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public struct ResourceMonitorDigestSnapshot
	{
		public ResourceDigestStats[][] TimeInServerDigest;

		public ResourceDigestStats[][] LogRecordBytesDigest;
	}
}
