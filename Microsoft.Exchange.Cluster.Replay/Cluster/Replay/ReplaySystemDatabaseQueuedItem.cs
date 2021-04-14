using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ReplaySystemDatabaseQueuedItem : ReplaySystemQueuedItem
	{
		protected ReplaySystemDatabaseQueuedItem(ReplicaInstanceManager riManager, Guid dbGuid) : base(riManager)
		{
			this.DbGuid = dbGuid;
		}

		protected Guid DbGuid { get; set; }

		protected override Exception GetOperationCancelledException()
		{
			return new ReplayDatabaseOperationCancelledException(base.GetType().Name, this.DbGuid.ToString());
		}

		protected override Exception GetOperationTimedoutException(TimeSpan timeout)
		{
			return new ReplayDatabaseOperationTimedoutException(base.GetType().Name, this.DbGuid.ToString(), timeout);
		}
	}
}
