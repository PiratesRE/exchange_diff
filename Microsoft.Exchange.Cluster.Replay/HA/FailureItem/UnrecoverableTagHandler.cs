using System;
using Microsoft.Exchange.Cluster.FailureItemEventLog;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HA.FailureItem
{
	internal class UnrecoverableTagHandler : TagHandler
	{
		internal UnrecoverableTagHandler(FailureItemWatcher watcher, DatabaseFailureItem dbfi) : base("Unrecoverable", watcher, dbfi)
		{
		}

		internal override ExEventLog.EventTuple? Event9aSrc
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_SrcUnrecoverable9a);
			}
		}

		internal override ExEventLog.EventTuple? Event9aTgt
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_TgtUnrecoverable9a);
			}
		}

		internal override ExEventLog.EventTuple? Event9bSrc
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_SrcUnrecoverable9b);
			}
		}

		internal override ExEventLog.EventTuple? Event9bTgt
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_TgtUnrecoverable9b);
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
			Exception ex = null;
			try
			{
				base.SuspendAndFailLocalCopyNoThrow(false, false, false);
			}
			finally
			{
				ex = DatabaseTasks.TryToDismountClean(base.Database);
			}
			if (ex != null)
			{
				ReplayCrimsonEvents.UnrecoverableDismountCleanFailure.Log<IADDatabase, string>(base.Database, ex.Message);
				throw new FailureItemRecoveryException(base.Database.Name, ex.ToString(), ex);
			}
		}

		internal override void PassiveRecoveryActionInternal()
		{
			base.SuspendAndFailLocalCopy(false, false, false);
		}
	}
}
