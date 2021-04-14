using System;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.FailureItemEventLog;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HA.FailureItem
{
	internal class UnexpectedDismountTagHandler : TagHandler
	{
		internal UnexpectedDismountTagHandler(FailureItemWatcher watcher, DatabaseFailureItem dbfi) : base("UnexpectedDismountTagHandler", watcher, dbfi)
		{
		}

		internal override ExEventLog.EventTuple? Event9aSrc
		{
			get
			{
				if (this.m_moveWasSkipped)
				{
					return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_UnexpectedDismountMoveWasSkipped9a);
				}
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_UnexpectedDismount9a);
			}
		}

		internal override ExEventLog.EventTuple? Event9aTgt
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_UnexpectedDismount9a);
			}
		}

		internal override ExEventLog.EventTuple? Event9bSrc
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_UnexpectedDismount9b);
			}
		}

		internal override bool IsTPRMoveTheActiveRecoveryAction
		{
			get
			{
				return !AmStoreHelper.IsMountedLocally(base.Database.Guid);
			}
		}

		internal override void ActiveRecoveryActionInternal()
		{
			if (!AmStoreHelper.IsMountedLocally(base.Database.Guid))
			{
				DatabaseTasks.Move(base.Database, Environment.MachineName);
				return;
			}
			this.m_moveWasSkipped = true;
			base.Trace("Skipping failover since the database is already mounted", new object[0]);
		}

		internal override void PassiveRecoveryActionInternal()
		{
		}

		private bool m_moveWasSkipped;
	}
}
