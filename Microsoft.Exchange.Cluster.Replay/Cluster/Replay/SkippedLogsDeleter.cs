using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class SkippedLogsDeleter : TimerComponent
	{
		public SkippedLogsDeleter() : base(TimeSpan.Zero, TimeSpan.FromSeconds((double)RegistryParameters.SkippedLogsDeletionIntervalSecs), "SkippedLogsDeleter")
		{
		}

		public void UpdateDiscoveredConfigurations(List<ReplayConfiguration> allConfigurations)
		{
			List<ReplayConfiguration> allConfigurationsCached = new List<ReplayConfiguration>(allConfigurations);
			lock (this.m_cacheLock)
			{
				this.m_allConfigurationsCached = allConfigurationsCached;
			}
		}

		protected override void TimerCallbackInternal()
		{
			List<ReplayConfiguration> list = null;
			lock (this.m_cacheLock)
			{
				if (this.m_allConfigurationsCached == null || this.m_allConfigurationsCached.Count == 0)
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug((long)this.GetHashCode(), "SkippedLogsDeleter: No configurations have been discovered, so nothing to do! Exiting.");
					return;
				}
				list = new List<ReplayConfiguration>(this.m_allConfigurationsCached);
			}
			foreach (ReplayConfiguration replayConfiguration in list)
			{
				if (base.PrepareToStopCalled)
				{
					break;
				}
				if (replayConfiguration != null && (replayConfiguration.Type == ReplayConfigType.RemoteCopySource || replayConfiguration.Type == ReplayConfigType.RemoteCopyTarget))
				{
					AgedOutDirectoryHelper.DeleteSkippedLogs(replayConfiguration.E00LogBackupPath, replayConfiguration.DatabaseName, false);
				}
			}
		}

		private List<ReplayConfiguration> m_allConfigurationsCached;

		private object m_cacheLock = new object();
	}
}
