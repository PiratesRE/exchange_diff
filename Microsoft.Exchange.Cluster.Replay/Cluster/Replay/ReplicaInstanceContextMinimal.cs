using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReplicaInstanceContextMinimal
	{
		public bool Suspended { get; private set; }

		public CopyStatusEnum LastCopyStatus { get; private set; }

		public FailureInfo FailureInfo { get; private set; }

		public ReplicaInstanceContextMinimal(ReplicaInstanceContext previousContext)
		{
			this.Suspended = previousContext.Suspended;
			this.LastCopyStatus = previousContext.GetStatus();
			this.FailureInfo = new FailureInfo(previousContext.FailureInfo);
		}
	}
}
