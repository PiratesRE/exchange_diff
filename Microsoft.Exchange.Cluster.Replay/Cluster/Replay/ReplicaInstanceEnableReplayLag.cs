using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReplicaInstanceEnableReplayLag : ReplicaInstanceQueuedItem
	{
		public override string Name
		{
			get
			{
				return ReplayStrings.EnableReplayLagOperationName;
			}
		}

		public ReplicaInstanceEnableReplayLag(ReplicaInstance instance) : base(instance)
		{
		}

		internal ActionInitiatorType ActionInitiator { get; set; }

		protected override void CheckOperationApplicable()
		{
		}

		protected override void ExecuteInternal()
		{
			base.ExecuteInternal();
			base.ReplicaInstance.EnableReplayLag(this.ActionInitiator);
		}
	}
}
