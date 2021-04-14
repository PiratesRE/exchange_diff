using System;
using Microsoft.Exchange.Cluster.FailureItemEventLog;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.HA.FailureItem
{
	internal class LowDiskSpaceStragglerTagHandler : TagHandler
	{
		internal LowDiskSpaceStragglerTagHandler(FailureItemWatcher watcher, DatabaseFailureItem dbfi) : base("LowDiskSpaceStragglerTagHandler", watcher, dbfi)
		{
		}

		internal override ExEventLog.EventTuple? Event9aTgt
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_TgtLowDiskSpaceStraggler9a);
			}
		}

		internal override ExEventLog.EventTuple? Event9bTgt
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_TgtLowDiskSpaceStraggler9b);
			}
		}

		internal override bool IsTPRMoveTheActiveRecoveryAction
		{
			get
			{
				return false;
			}
		}

		internal override void ActiveRecoveryActionInternal()
		{
		}

		internal override void PassiveRecoveryActionInternal()
		{
			base.SuspendAndFailLocalCopy(false, false, false);
			Exception ex = AutoReseedWorkflowState.TriggerInPlaceReseed(base.Database.Guid, base.Database.Name);
			if (ex != null)
			{
				ExTraceGlobals.FailureItemTracer.TraceError<Guid, Exception>((long)this.GetHashCode(), "LowDiskSpaceStragglerTagHandler({0}) failed to write autoReseedWorkflowState: {1}", base.Database.Guid, ex);
			}
		}
	}
}
