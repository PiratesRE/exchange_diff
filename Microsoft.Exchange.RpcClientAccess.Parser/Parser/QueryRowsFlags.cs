using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum QueryRowsFlags : byte
	{
		None = 0,
		DoNotAdvance = 1,
		SendMax = 2,
		Backwards = 4,
		ChainAlways = 8
	}
}
