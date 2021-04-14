using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReplicaInstanceDisableReplayLag : ReplicaInstanceQueuedItem
	{
		public override string Name
		{
			get
			{
				return ReplayStrings.DisableReplayLagOperationName;
			}
		}

		public ReplicaInstanceDisableReplayLag(ReplicaInstance instance) : base(instance)
		{
		}

		internal string Reason { get; set; }

		internal ActionInitiatorType ActionInitiator { get; set; }

		protected override void CheckOperationApplicable()
		{
		}

		protected override void ExecuteInternal()
		{
			base.ExecuteInternal();
			base.ReplicaInstance.DisableReplayLag(this.Reason, this.ActionInitiator);
		}
	}
}
