using System;
using System.Threading;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AcllAutoLockRelease : TimerComponent
	{
		public AcllAutoLockRelease(ReplicaInstance instance) : base(TimeSpan.FromMilliseconds((double)RegistryParameters.AcllLockAutoReleaseAfterDurationMs), TimeSpan.FromMilliseconds((double)RegistryParameters.AcllLockAutoReleaseAfterDurationMs), "AcllAutoLockRelease")
		{
			this.ReplicaInstance = instance;
		}

		private ReplicaInstance ReplicaInstance { get; set; }

		protected override void StopInternal()
		{
			base.StopInternal();
			this.ReplicaInstance = null;
		}

		protected override void TimerCallbackInternal()
		{
			bool flag = true;
			if (this.ReplicaInstance.CurrentContext.InAttemptCopyLastLogs)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, string>((long)this.ReplicaInstance.GetHashCode(), "AcllAutoLockRelease: Skipping ACLL lock auto-release for {0}({1}) since ACLL is still in progress.", this.ReplicaInstance.Configuration.DisplayName, this.ReplicaInstance.Configuration.Identity);
				flag = false;
			}
			else if (this.ReplicaInstance.CurrentContext.IsFailoverPending())
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, string, TimeSpan>((long)this.ReplicaInstance.GetHashCode(), "AcllAutoLockRelease: Auto-releasing ACLL lock for {0}({1}) after {2}ms", this.ReplicaInstance.Configuration.DisplayName, this.ReplicaInstance.Configuration.Identity, base.Period);
				this.ReplicaInstance.CurrentContext.BestEffortDismountReplayDatabase();
				this.ReplicaInstance.Configuration.ReplayState.SuspendLock.LeaveAttemptCopyLastLogs();
				this.ReplicaInstance.CurrentContext.ClearAttemptCopyLastLogsEndTime();
			}
			else
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, string>((long)this.ReplicaInstance.GetHashCode(), "AcllAutoLockRelease: Skipping ACLL lock auto-release for {0}({1}) since a move/failover is no longer pending.", this.ReplicaInstance.Configuration.DisplayName, this.ReplicaInstance.Configuration.Identity);
			}
			if (flag)
			{
				ThreadPool.QueueUserWorkItem(delegate(object param0)
				{
					base.Stop();
				});
			}
		}
	}
}
