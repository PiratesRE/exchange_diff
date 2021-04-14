using System;

namespace Microsoft.Exchange.Rpc.Cluster
{
	internal enum ReplayLagEnabledEnum : uint
	{
		Unknown,
		Enabled,
		Disabled,
		CmdletDisabled
	}
}
