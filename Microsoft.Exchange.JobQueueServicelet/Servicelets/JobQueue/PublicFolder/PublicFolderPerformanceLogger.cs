using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.Storage.PublicFolder;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.LatencyDetection;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Servicelets.JobQueue.PublicFolder
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PublicFolderPerformanceLogger
	{
		public PublicFolderPerformanceLogger(PublicFolderSynchronizerContext syncContext)
		{
			ArgumentValidator.ThrowIfNull("syncContext", syncContext);
			this.syncContext = syncContext;
			this.transientRetryDelayTracker = new PerformanceDataProvider("TransientRetryDelay");
			this.performanceTrackers = new Dictionary<SyncActivity, PublicFolderActivityPerformanceTracker>(30);
		}

		public PerformanceDataProvider TransientRetryDelayTracker
		{
			get
			{
				return this.transientRetryDelayTracker;
			}
		}

		private FolderOperationCounter FolderOperationCounter
		{
			get
			{
				return this.syncContext.FolderOperationCount;
			}
		}

		private SyncStateCounter SyncStateCounter
		{
			get
			{
				return this.syncContext.SyncStateCounter;
			}
		}

		private LatencyInfo MRSProxyLatencyInfo
		{
			get
			{
				return this.syncContext.MRSProxyLatencyInfo;
			}
		}

		private Guid? CorrelationId
		{
			get
			{
				return new Guid?(this.syncContext.CorrelationId);
			}
		}

		public IDisposable GetTaskFrame(SyncActivity activity)
		{
			PublicFolderActivityPerformanceTracker publicFolderActivityPerformanceTracker;
			if (!this.performanceTrackers.TryGetValue(activity, out publicFolderActivityPerformanceTracker))
			{
				publicFolderActivityPerformanceTracker = new PublicFolderActivityPerformanceTracker(activity, this.FolderOperationCounter, this.SyncStateCounter, this.MRSProxyLatencyInfo, this.transientRetryDelayTracker);
				this.performanceTrackers[activity] = publicFolderActivityPerformanceTracker;
			}
			return new PublicFolderPerformanceLogger.TaskFrame(activity, publicFolderActivityPerformanceTracker);
		}

		public void InitializeCounters(int batchNumber)
		{
			this.batchNumber = batchNumber;
			this.performanceTrackers.Clear();
		}

		public void WriteActivitiesCountersToLog()
		{
			StringBuilder stringBuilder = new StringBuilder(1000);
			foreach (PublicFolderActivityPerformanceTracker publicFolderActivityPerformanceTracker in this.performanceTrackers.Values)
			{
				stringBuilder.AppendFormat("Batch={0};", this.batchNumber);
				publicFolderActivityPerformanceTracker.AppendLogData(stringBuilder);
				PublicFolderSynchronizerLogger.LogOnServer(stringBuilder.ToString(), LogEventType.PerfCounters, this.CorrelationId);
				stringBuilder.Clear();
			}
		}

		private readonly Dictionary<SyncActivity, PublicFolderActivityPerformanceTracker> performanceTrackers;

		private readonly PublicFolderSynchronizerContext syncContext;

		private readonly PerformanceDataProvider transientRetryDelayTracker;

		private int batchNumber;

		private class TaskFrame : IDisposable
		{
			public TaskFrame(SyncActivity activity, PublicFolderActivityPerformanceTracker performanceTracker)
			{
				IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
				if (currentActivityScope != null)
				{
					this.previousActionDescription = currentActivityScope.Action;
					currentActivityScope.Action = activity.ToString();
				}
				this.performanceTracker = performanceTracker;
				this.performanceTracker.Start();
			}

			public void Dispose()
			{
				this.performanceTracker.Stop();
				IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
				if (currentActivityScope != null)
				{
					currentActivityScope.Action = this.previousActionDescription;
				}
			}

			private readonly PublicFolderActivityPerformanceTracker performanceTracker;

			private readonly string previousActionDescription;
		}
	}
}
