using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReplicaInstanceRequestSuspendAndFail : ReplicaInstanceQueuedItem
	{
		public ReplicaInstanceRequestSuspendAndFail(ReplicaInstance instance) : base(instance)
		{
			this.SuspendCopy = true;
		}

		internal uint ErrorEventId { get; set; }

		internal string ErrorMessage { get; set; }

		internal string SuspendComment { get; set; }

		internal bool PreserveExistingError { get; set; }

		internal bool SuspendCopy { get; set; }

		internal bool BlockResume { get; set; }

		internal bool BlockReseed { get; set; }

		internal bool BlockInPlaceReseed { get; set; }

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
			base.ReplicaInstance.RequestSuspendAndFail(this);
		}
	}
}
