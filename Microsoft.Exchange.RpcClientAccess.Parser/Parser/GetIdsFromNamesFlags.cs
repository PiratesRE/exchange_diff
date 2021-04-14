using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum GetIdsFromNamesFlags : byte
	{
		None = 0,
		Create = 2
	}
}
