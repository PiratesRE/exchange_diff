using System;

namespace Microsoft.Exchange.Rpc.Cluster
{
	internal enum CopyStatusEnum
	{
		Unknown,
		Initializing,
		Resynchronizing,
		DisconnectedAndResynchronizing,
		DisconnectedAndHealthy,
		Healthy,
		Failed,
		FailedAndSuspended,
		Suspended,
		Seeding,
		Mounting,
		Mounted,
		Dismounting,
		Dismounted,
		NonExchangeReplication,
		SeedingSource,
		Misconfigured
	}
}
