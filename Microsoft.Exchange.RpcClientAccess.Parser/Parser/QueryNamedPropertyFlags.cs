using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum QueryNamedPropertyFlags : byte
	{
		None = 0,
		NoStrings = 1,
		NoIds = 2
	}
}
