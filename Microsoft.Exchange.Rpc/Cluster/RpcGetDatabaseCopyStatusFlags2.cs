using System;

namespace Microsoft.Exchange.Rpc.Cluster
{
	[Flags]
	internal enum RpcGetDatabaseCopyStatusFlags2 : uint
	{
		None = 0U,
		ReadThrough = 1U,
		TestOOMHandling = 32768U
	}
}
