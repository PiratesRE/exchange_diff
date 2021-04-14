using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum RestrictFlags : byte
	{
		None = 0,
		NoWait = 1,
		Static = 4
	}
}
