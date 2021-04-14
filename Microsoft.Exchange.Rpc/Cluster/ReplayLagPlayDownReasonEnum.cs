using System;

namespace Microsoft.Exchange.Rpc.Cluster
{
	internal enum ReplayLagPlayDownReasonEnum : uint
	{
		None,
		LagDisabled,
		NotEnoughFreeSpace,
		InRequiredRange
	}
}
