using System;

namespace Microsoft.Exchange.Rpc.Cluster
{
	internal enum RpcCopyStatusSummary
	{
		Replicating,
		Mounted,
		Dismounted,
		Mounting,
		Dismounting,
		Initializing,
		Resynchronizing
	}
}
