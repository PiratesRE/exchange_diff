using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum ImportMessageChangeFlags : byte
	{
		None = 0,
		Associated = 16,
		FailOnConflict = 64
	}
}
