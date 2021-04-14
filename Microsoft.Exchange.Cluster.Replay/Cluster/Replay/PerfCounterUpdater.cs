using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PerfCounterUpdater : TimerComponent
	{
		public PerfCounterUpdater(IPerfmonCounters counters, ReplayConfiguration config) : base(TimeSpan.FromMilliseconds((double)RegistryParameters.PerfCounterUpdateIntervalInMSec), TimeSpan.FromMilliseconds((double)RegistryParameters.PerfCounterUpdateIntervalInMSec), "PerfCounterUpdater")
		{
			this.m_counters = counters;
			this.m_config = config;
			this.m_amCounters = ActiveManagerPerfmon.GetInstance(config.Name);
		}

		protected override void TimerCallbackInternal()
		{
			this.UpdateQueueAlertPerfCounters();
			this.ClearDatabaseMountedCounter();
		}

		private void UpdateQueueAlertPerfCounters()
		{
			if (this.m_counters.Suspended != 0L || this.m_counters.Initializing != 0L || this.m_counters.Failed != 0L || this.m_counters.FailedSuspended != 0L || this.m_counters.Resynchronizing != 0L)
			{
				this.m_counters.CopyQueueNotKeepingUp = 0L;
				this.m_counters.ReplayQueueNotKeepingUp = 0L;
				this.m_firstTime = true;
				return;
			}
			if (this.m_counters.Disconnected != 0L)
			{
				Exception ex = null;
				try
				{
					ReplayState replayState = this.m_config.ReplayState;
					long lastLogCommittedGenerationNumberFromCluster = replayState.GetLastLogCommittedGenerationNumberFromCluster();
					if (this.m_counters.CopyNotificationGenerationNumber < lastLogCommittedGenerationNumberFromCluster)
					{
						this.m_counters.CopyNotificationGenerationNumber = lastLogCommittedGenerationNumberFromCluster;
					}
				}
				catch (ClusterException ex2)
				{
					ex = ex2;
				}
				catch (TransientException ex3)
				{
					ex = ex3;
				}
				catch (AmServerException ex4)
				{
					ex = ex4;
				}
				if (ex != null)
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, Exception>((long)this.GetHashCode(), "PerfCounterUpdater.UpdateQueueAlertPerfCounters({0}): Exception trying to update disconnected generation: {1}", this.m_config.DisplayName, ex);
				}
			}
			long copyQueueLength = this.m_counters.CopyQueueLength;
			long replayQueueLength = this.m_counters.ReplayQueueLength;
			long inspectorGenerationNumber = this.m_counters.InspectorGenerationNumber;
			long replayGenerationNumber = this.m_counters.ReplayGenerationNumber;
			DateTime utcNow = DateTime.UtcNow;
			if (this.m_firstTime)
			{
				this.m_baselineCopyQueueLength = copyQueueLength;
				this.m_baselineReplayQueueLength = replayQueueLength;
				this.m_firstTime = false;
			}
			else
			{
				if (copyQueueLength > this.m_baselineCopyQueueLength + this.CopyQueueAlertThreshold)
				{
					this.m_counters.CopyQueueNotKeepingUp = 1L;
				}
				else if (this.m_lastCopyQueueLength > 0L && inspectorGenerationNumber == this.m_lastCopiedGeneration)
				{
					if (++this.m_copyNotMakingProgressIntervals >= 4)
					{
						this.m_counters.CopyQueueNotKeepingUp = 1L;
					}
				}
				else
				{
					this.m_copyNotMakingProgressIntervals = 0;
					if ((double)copyQueueLength < (double)this.m_baselineCopyQueueLength + (double)this.CopyQueueAlertThreshold * 0.8)
					{
						this.m_counters.CopyQueueNotKeepingUp = 0L;
						if (copyQueueLength < this.m_baselineCopyQueueLength)
						{
							this.m_baselineReplayQueueLength += this.m_baselineCopyQueueLength - copyQueueLength;
							this.m_baselineCopyQueueLength = copyQueueLength;
						}
					}
				}
				if (utcNow - this.m_config.ReplayState.CurrentReplayTime > this.m_config.ReplayLagTime + this.ExtraReplayLagAllowed)
				{
					if (replayQueueLength > this.m_baselineReplayQueueLength + this.ReplayQueueAlertThreshold)
					{
						if (this.m_counters.PassiveSeedingSource == 0L && !this.m_config.ReplayState.ReplaySuspended)
						{
							this.m_counters.ReplayQueueNotKeepingUp = 1L;
						}
					}
					else if (this.m_lastReplayQueueLength > 0L && replayGenerationNumber == this.m_lastReplayedGeneration)
					{
						if (++this.m_replayNotMakingProgressIntervals >= 4 && this.m_counters.PassiveSeedingSource == 0L && !this.m_config.ReplayState.ReplaySuspended)
						{
							this.m_counters.ReplayQueueNotKeepingUp = 1L;
						}
					}
					else
					{
						this.m_replayNotMakingProgressIntervals = 0;
						if ((double)replayQueueLength < (double)this.m_baselineReplayQueueLength + (double)this.ReplayQueueAlertThreshold * 0.8)
						{
							this.m_counters.ReplayQueueNotKeepingUp = 0L;
							if (replayQueueLength < this.m_baselineReplayQueueLength)
							{
								this.m_baselineReplayQueueLength = replayQueueLength;
							}
						}
					}
				}
				else
				{
					this.m_counters.ReplayQueueNotKeepingUp = 0L;
					if (replayQueueLength < this.m_baselineReplayQueueLength)
					{
						this.m_baselineReplayQueueLength = replayQueueLength;
					}
				}
			}
			this.m_lastCopyQueueLength = copyQueueLength;
			this.m_lastReplayQueueLength = replayQueueLength;
			this.m_lastCopiedGeneration = inspectorGenerationNumber;
			this.m_lastReplayedGeneration = replayGenerationNumber;
			ExTraceGlobals.ReplicaInstanceTracer.TraceDebug((long)this.GetHashCode(), "PerfCounterUpdater.UpdateQueueAlertPerfCounters(): Config '{0}': CopyQueueLength={1}, CopyQueueLengthBaseline={2}, CopyQueueNotKeepingUp={3}", new object[]
			{
				this.m_config.DisplayName,
				copyQueueLength,
				this.m_baselineCopyQueueLength,
				this.m_counters.CopyQueueNotKeepingUp
			});
			ExTraceGlobals.ReplicaInstanceTracer.TraceDebug((long)this.GetHashCode(), "PerfCounterUpdater.UpdateQueueAlertPerfCounters(): Config '{0}': ReplayQueueLength={1}, ReplayQueueLengthBaseline={2}, ReplayQueueNotKeepingUp={3}", new object[]
			{
				this.m_config.DisplayName,
				replayQueueLength,
				this.m_baselineReplayQueueLength,
				this.m_counters.ReplayQueueNotKeepingUp
			});
		}

		private void ClearDatabaseMountedCounter()
		{
			this.m_amCounters.IsMounted.RawValue = 0L;
		}

		private readonly long CopyQueueAlertThreshold = (long)RegistryParameters.CopyQueueAlertThreshold;

		private readonly long ReplayQueueAlertThreshold = (long)RegistryParameters.ReplayQueueAlertThreshold;

		private readonly TimeSpan ExtraReplayLagAllowed = TimeSpan.FromMinutes((double)RegistryParameters.ExtraReplayLagAllowedMinutes);

		private IPerfmonCounters m_counters;

		private ActiveManagerPerfmonInstance m_amCounters;

		private ReplayConfiguration m_config;

		private bool m_firstTime = true;

		private long m_baselineCopyQueueLength;

		private long m_baselineReplayQueueLength;

		private long m_lastCopyQueueLength;

		private long m_lastReplayQueueLength;

		private long m_lastCopiedGeneration;

		private long m_lastReplayedGeneration;

		private int m_copyNotMakingProgressIntervals;

		private int m_replayNotMakingProgressIntervals;
	}
}
