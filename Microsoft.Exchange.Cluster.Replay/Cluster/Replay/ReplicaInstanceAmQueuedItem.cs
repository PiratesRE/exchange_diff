using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ReplicaInstanceAmQueuedItem : ReplicaInstanceQueuedItem
	{
		protected ReplicaInstanceAmQueuedItem(ReplicaInstance replicaInstance) : base(replicaInstance)
		{
		}

		protected override Exception GetOperationCancelledException()
		{
			return new AmDbActionCancelledException(base.DbName, this.Name);
		}

		protected override Exception GetOperationTimedoutException(TimeSpan timeout)
		{
			return new AmDbOperationTimedoutException(base.DbName, this.Name, timeout);
		}
	}
}
