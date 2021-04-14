using System;

namespace Microsoft.Exchange.Server.Storage.RpcProxy
{
	[Flags]
	internal enum UnmountFlags : uint
	{
		ForceDatabaseDeletion = 8U,
		SkipCacheFlush = 16U
	}
}
