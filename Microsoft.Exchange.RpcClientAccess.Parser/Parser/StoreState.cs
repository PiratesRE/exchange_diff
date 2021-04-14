using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal enum StoreState : uint
	{
		None,
		HasSearches = 16777216U
	}
}
