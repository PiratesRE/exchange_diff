using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReplicaInstanceRequestSuspend : ReplicaInstanceQueuedItem
	{
		public override string Name
		{
			get
			{
				return ReplayStrings.SuspendOperationName;
			}
		}

		public ReplicaInstanceRequestSuspend(ReplicaInstance instance) : base(instance)
		{
		}

		internal string SuspendComment { get; set; }

		internal DatabaseCopyActionFlags Flags { get; set; }

		internal ActionInitiatorType ActionInitiator { get; set; }

		protected override void CheckOperationApplicable()
		{
			if (base.ReplicaInstance.CurrentContext.IsFailoverPending())
			{
				throw new ReplayServiceSuspendInvalidDuringMoveException(base.DbCopyName);
			}
		}

		protected override void ExecuteInternal()
		{
			base.ExecuteInternal();
			base.ReplicaInstance.RequestSuspend(this.SuspendComment, this.Flags, this.ActionInitiator);
		}
	}
}
