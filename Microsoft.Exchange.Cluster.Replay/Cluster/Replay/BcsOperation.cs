using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal enum BcsOperation
	{
		HasDatabaseBeenMounted = 1,
		GetDatabaseCopies,
		DetermineServersToContact,
		GetCopyStatusRpc,
		BcsOverall
	}
}
