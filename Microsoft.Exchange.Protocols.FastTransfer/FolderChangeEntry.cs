using System;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	public struct FolderChangeEntry
	{
		public ExchangeShortId FolderId;

		public long Cn;
	}
}
