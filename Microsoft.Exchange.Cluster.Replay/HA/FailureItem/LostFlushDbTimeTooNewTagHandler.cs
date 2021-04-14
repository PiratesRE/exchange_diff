using System;
using Microsoft.Exchange.Cluster.FailureItemEventLog;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HA.FailureItem
{
	internal class LostFlushDbTimeTooNewTagHandler : TagHandler
	{
		internal LostFlushDbTimeTooNewTagHandler(FailureItemWatcher watcher, DatabaseFailureItem dbfi) : base("LostFlushDbTimeTooNewTagHandler", watcher, dbfi)
		{
		}

		internal override ExEventLog.EventTuple? Event9aSrc
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_SrcLostFlushDbTimeTooNew9a);
			}
		}

		internal override ExEventLog.EventTuple? Event9aTgt
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_TgtLostFlushDbTimeTooNew9a);
			}
		}

		internal override ExEventLog.EventTuple? Event9bTgt
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_TgtLostFlushDbTimeTooNew9b);
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
			base.SuspendAndFailLocalCopy(false, true, false);
		}
	}
}
