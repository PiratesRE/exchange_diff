using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum SetColumnsFlags : byte
	{
		None = 0,
		Asynchronous = 1
	}
}
