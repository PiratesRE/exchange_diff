using System;
using Microsoft.Exchange.Cluster.FailureItemEventLog;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HA.FailureItem
{
	internal class HungIoExceededThresholdDoubleDiskTagHandler : TagHandler
	{
		internal HungIoExceededThresholdDoubleDiskTagHandler(FailureItemWatcher watcher, DatabaseFailureItem dbfi) : base("HungIoExceededThresholdDoubleDiskTagHandler", watcher, dbfi)
		{
		}

		internal override ExEventLog.EventTuple? Event9aSrc
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_SrcHungIoExceededThresholdDoubleDisk9a);
			}
		}

		internal override ExEventLog.EventTuple? Event9bSrc
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_SrcHungIoExceededThresholdDoubleDisk9b);
			}
		}

		internal override ExEventLog.EventTuple? Event9aTgt
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_TgtHungIoExceededThresholdDoubleDisk9a);
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
			base.PostProcessingAction = delegate()
			{
				BugcheckHelper.TriggerBugcheckIfRequired(base.FailureItem.CreationTime.ToUniversalTime(), string.Format("HungIoExceededThresholdDoubleDiskTagHandler FailureItem at {0}", base.FailureItem.CreationTime));
			};
		}

		internal override void PassiveRecoveryActionInternal()
		{
			base.PostProcessingAction = delegate()
			{
				BugcheckHelper.TriggerBugcheckIfRequired(base.FailureItem.CreationTime.ToUniversalTime(), string.Format("HungIoExceededThresholdDoubleDiskTagHandler FailureItem at {0}", base.FailureItem.CreationTime));
			};
		}
	}
}
