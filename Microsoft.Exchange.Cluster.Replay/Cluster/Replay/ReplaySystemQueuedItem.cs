using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ReplaySystemQueuedItem : ReplayQueuedItemBase
	{
		protected ReplaySystemQueuedItem(ReplicaInstanceManager riManager)
		{
			this.ReplicaInstanceManager = riManager;
		}

		protected ReplicaInstanceManager ReplicaInstanceManager { get; set; }

		protected override Exception GetOperationCancelledException()
		{
			return new ReplaySystemOperationCancelledException(base.GetType().Name);
		}

		protected override Exception GetOperationTimedoutException(TimeSpan timeout)
		{
			return new ReplaySystemOperationTimedoutException(base.GetType().Name, timeout);
		}
	}
}
