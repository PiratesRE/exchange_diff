using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum CreateMessageExtendedFlags : uint
	{
		None = 0U,
		ContentAggregation = 1U,
		ClientAssociated = 64U
	}
}
