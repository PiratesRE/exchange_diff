using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum LockTypeFlag
	{
		None = 0,
		LockWrite = 1,
		LockExclusive = 2,
		LockOnlyOnce = 4
	}
}
