using System;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	[Flags]
	internal enum CacheMode
	{
		Disabled = 0,
		Read = 2,
		SyncWrite = 4,
		AsyncWrite = 8
	}
}
