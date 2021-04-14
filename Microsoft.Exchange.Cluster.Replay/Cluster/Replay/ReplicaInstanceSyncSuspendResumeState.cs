using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReplicaInstanceSyncSuspendResumeState : ReplicaInstanceQueuedItem
	{
		public override string Name
		{
			get
			{
				return ReplayStrings.SyncSuspendResumeOperationName;
			}
		}

		public ReplicaInstanceSyncSuspendResumeState(ReplicaInstance instance) : base(instance)
		{
		}

		protected override void CheckOperationApplicable()
		{
			if (base.ReplicaInstance.CurrentContext.IsFailoverPending())
			{
				throw new ReplayServiceSyncStateInvalidDuringMoveException(base.DbCopyName);
			}
		}

		protected override void ExecuteInternal()
		{
			base.ExecuteInternal();
			base.ReplicaInstance.SyncSuspendResumeState();
		}
	}
}
