using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum ModifyRulesFlags : byte
	{
		None = 0,
		ReplaceRows = 1
	}
}
