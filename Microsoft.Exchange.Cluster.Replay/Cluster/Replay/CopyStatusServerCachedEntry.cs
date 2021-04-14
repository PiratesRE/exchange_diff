using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CopyStatusServerCachedEntry : ICopyStatusCachedEntry
	{
		public RpcDatabaseCopyStatus2 CopyStatus { get; private set; }

		internal DateTime CreateTimeUtc { get; private set; }

		internal bool ForceRefresh { get; set; }

		internal CopyStatusServerCachedEntry(RpcDatabaseCopyStatus2 status)
		{
			this.CreateTimeUtc = DateTime.UtcNow;
			this.CopyStatus = status;
		}
	}
}
