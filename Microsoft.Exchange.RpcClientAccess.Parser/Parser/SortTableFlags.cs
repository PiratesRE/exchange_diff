using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum SortTableFlags : byte
	{
		None = 0,
		NoWait = 1
	}
}
