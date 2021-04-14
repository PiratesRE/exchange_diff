using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmSystemFailoverOnReplayDownTimer : TimerComponent
	{
		internal AmSystemFailoverOnReplayDownTimer(AmServerName serverName, TimeSpan startDelay) : base(startDelay, TimeSpan.FromMilliseconds(-1.0), "SystemFailoverOnReplayDownTimer")
		{
			this.serverName = serverName;
		}

		protected override void TimerCallbackInternal()
		{
			bool flag = true;
			Exception ex = null;
			AmSystemFailoverOnReplayDownTracker failoverTracker = AmSystemManager.Instance.SystemFailoverOnReplayDownTracker;
			if (failoverTracker != null)
			{
				try
				{
					ex = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
					{
						this.Run(failoverTracker);
					});
					flag = false;
				}
				finally
				{
					failoverTracker.RemoveTimer(this.serverName);
					if (flag || ex != null)
					{
						ReplayCrimsonEvents.FailoverOnReplDownFailedToInitiate.Log<AmServerName, string>(this.serverName, (ex != null) ? ex.Message : "<Unhandled exception>");
					}
				}
			}
		}

		private void Run(AmSystemFailoverOnReplayDownTracker failoverTracker)
		{
			AmConfig config = AmSystemManager.Instance.Config;
			if (!config.IsPAM)
			{
				ReplayCrimsonEvents.FailoverOnReplDownSkipped.Log<AmServerName, string, string>(this.serverName, "RoleNotPAM", "TimerCallback");
				return;
			}
			if (AmHelper.IsReplayRunning(this.serverName))
			{
				ReplayCrimsonEvents.FailoverOnReplDownSkipped.Log<AmServerName, string, string>(this.serverName, "ReplRunning", "TimerCallback");
				return;
			}
			AmThrottledActionTracker<FailoverData>.ThrottlingShapshot throttlingSnapshot = failoverTracker.GetThrottlingSnapshot(this.serverName);
			if (throttlingSnapshot.IsActionCalledTooSoonPerNode || throttlingSnapshot.IsActionCalledTooSoonAcrossDag || throttlingSnapshot.IsMaxActionsPerNodeExceeded || throttlingSnapshot.IsMaxActionsAcrossDagExceeded)
			{
				throttlingSnapshot.LogResults(ReplayCrimsonEvents.FailoverOnReplDownThrottlingFailed, TimeSpan.FromMinutes(15.0));
				return;
			}
			failoverTracker.AddFailoverEntry(this.serverName);
			throttlingSnapshot.LogResults(ReplayCrimsonEvents.FailoverOnReplDownThrottlingSucceeded, TimeSpan.Zero);
			AmEvtSystemFailoverOnReplayDown amEvtSystemFailoverOnReplayDown = new AmEvtSystemFailoverOnReplayDown(this.serverName);
			amEvtSystemFailoverOnReplayDown.Notify();
		}

		private readonly AmServerName serverName;
	}
}
