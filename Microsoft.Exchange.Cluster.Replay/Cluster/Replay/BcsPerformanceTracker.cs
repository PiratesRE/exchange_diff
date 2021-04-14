using System;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.ReplayEventLog;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class BcsPerformanceTracker : FailoverPerformanceTrackerBase<BcsOperation>
	{
		public BcsPerformanceTracker(AmDbAction.PrepareSubactionArgsDelegate prepareSubAction) : base("BcsPerf")
		{
			this.m_prepareSubAction = prepareSubAction;
		}

		public override void LogEvent()
		{
			if (this.m_prepareSubAction != null)
			{
				ReplayCrimsonEvents.BcsCompleted.LogGeneric(this.m_prepareSubAction(new object[]
				{
					base.GetDuration(BcsOperation.BcsOverall),
					base.GetDuration(BcsOperation.HasDatabaseBeenMounted),
					base.GetDuration(BcsOperation.GetDatabaseCopies),
					base.GetDuration(BcsOperation.DetermineServersToContact),
					base.GetDuration(BcsOperation.GetCopyStatusRpc)
				}));
			}
		}

		private AmDbAction.PrepareSubactionArgsDelegate m_prepareSubAction;
	}
}
