using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Flags]
	public enum StreamCopyMode
	{
		SyncReadSyncWrite = 3,
		SyncReadAsyncWrite = 9,
		AsyncReadSyncWrite = 6,
		AsyncReadAsyncWrite = 12
	}
}
