using System;
using Microsoft.Exchange.Cluster.FailureItemEventLog;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HA.FailureItem
{
	internal class RecoveryRedoLogCorruptionTagHandler : TagHandler
	{
		internal RecoveryRedoLogCorruptionTagHandler(FailureItemWatcher watcher, DatabaseFailureItem dbfi) : base("RecoveryRedoLogCorruptionTagHandler", watcher, dbfi)
		{
		}

		internal override ExEventLog.EventTuple? Event9aSrc
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_SrcLogCorruptionDetectedByESE9a);
			}
		}

		internal override ExEventLog.EventTuple? Event9bSrc
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_SrcLogCorruptionDetectedByESE9b);
			}
		}

		internal override ExEventLog.EventTuple? Event9aTgt
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_TgtLogCorruptionDetectedByESE9a);
			}
		}

		internal override ExEventLog.EventTuple? Event9bTgt
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_TgtLogCorruptionDetectedByESE9b);
			}
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
			LogRepair.EnableLogRepair(base.Database.Guid);
			DatabaseTasks.Move(base.Database, Environment.MachineName);
		}

		internal override void PassiveRecoveryActionInternal()
		{
			LogRepair.EnableLogRepair(base.Database.Guid);
		}
	}
}
