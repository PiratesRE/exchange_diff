using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum FindRowFlags : byte
	{
		None = 0,
		Backward = 1
	}
}
