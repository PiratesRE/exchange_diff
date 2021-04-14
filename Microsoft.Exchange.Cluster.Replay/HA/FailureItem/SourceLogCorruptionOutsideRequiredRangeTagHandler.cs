using System;
using Microsoft.Exchange.Cluster.FailureItemEventLog;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HA.FailureItem
{
	internal class SourceLogCorruptionOutsideRequiredRangeTagHandler : TagHandler
	{
		internal override ExEventLog.EventTuple? Event9aSrc
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_SrcSourceLogCorruptOutsideRequiredRange9a);
			}
		}

		internal override ExEventLog.EventTuple? Event9bSrc
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_SrcSourceLogCorruptOutsideRequiredRange9b);
			}
		}

		internal SourceLogCorruptionOutsideRequiredRangeTagHandler(FailureItemWatcher watcher, DatabaseFailureItem dbfi) : base("SourceLogCorruptionOutsideRequiredRange", watcher, dbfi)
		{
		}

		internal override bool IsTPRMoveTheActiveRecoveryAction
		{
			get
			{
				return true;
			}
		}

		internal override void ActiveRecoveryActionInternal()
		{
			DatabaseTasks.Move(base.Database, Environment.MachineName);
		}

		internal override void PassiveRecoveryActionInternal()
		{
		}
	}
}
