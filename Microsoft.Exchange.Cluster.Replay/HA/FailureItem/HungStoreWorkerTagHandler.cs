using System;
using Microsoft.Exchange.Cluster.FailureItemEventLog;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HA.FailureItem
{
	internal class HungStoreWorkerTagHandler : TagHandler
	{
		internal HungStoreWorkerTagHandler(FailureItemWatcher watcher, DatabaseFailureItem dbfi) : base("HungStoreWorkerTagHandler", watcher, dbfi)
		{
		}

		internal override ExEventLog.EventTuple? Event9aSrc
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_SrcHungStoreWorker9a);
			}
		}

		internal override ExEventLog.EventTuple? Event9bSrc
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_SrcHungStoreWorker9b);
			}
		}

		internal override ExEventLog.EventTuple? Event9aTgt
		{
			get
			{
				return new ExEventLog.EventTuple?(FailureItemEventLogConstants.Tuple_TgtHungStoreWorker9a);
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
			DatabaseTasks.Move(base.Database, Environment.MachineName);
		}

		internal override void PassiveRecoveryActionInternal()
		{
		}
	}
}
