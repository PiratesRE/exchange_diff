using System;

namespace Microsoft.Exchange.Rpc.Cluster
{
	internal enum SeederState
	{
		Unknown,
		SeedPrepared,
		SeedInProgress,
		SeedSuccessful,
		SeedCancelled,
		SeedFailed
	}
}
