using System;
using Microsoft.Exchange.Cluster.FailureItemEventLog;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HA.FailureItem
{
	internal class LogGapFatalTagHandler : TagHandler
	{
		internal LogGapFatalTagHandler(FailureItemWatcher watcher, DatabaseFailureItem dbfi) : base("LogGapFatalTagHandler", watcher, dbfi)
		{
		}

		internal override ExEventLog.EventTuple? Event9aTgt
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_LogGapFatal9a);
			}
		}

		internal override ExEventLog.EventTuple? Event9bTgt
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_LogGapFatal9b);
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
		}
	}
}
