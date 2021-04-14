using System;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.LatencyDetection;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Servicelets.JobQueue.PublicFolder
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PublicFolderActivityPerformanceTracker : PerformanceTrackerBase
	{
		public PublicFolderActivityPerformanceTracker(SyncActivity trackedActivity, FolderOperationCounter folderOperationCounter, SyncStateCounter syncStateCounter, LatencyInfo mrsProxyLatencyInfo, PerformanceDataProvider transientRetryDelayTracker)
		{
			ArgumentValidator.ThrowIfNull("folderOperationCounter", folderOperationCounter);
			ArgumentValidator.ThrowIfNull("syncStateCounter", syncStateCounter);
			ArgumentValidator.ThrowIfNull("mrsProxyLatencyInfo", mrsProxyLatencyInfo);
			ArgumentValidator.ThrowIfNull("transientRetryDelayTracker", transientRetryDelayTracker);
			this.folderOperationCounter = folderOperationCounter;
			this.syncStateCounter = syncStateCounter;
			this.transientRetryDelayTracker = transientRetryDelayTracker;
			this.trackedActivity = trackedActivity;
			this.mrsProxyLatencyInfo = mrsProxyLatencyInfo;
		}

		public override void Start()
		{
			base.Start();
			this.invokeCount += 1U;
			this.startFoldersAdded = this.folderOperationCounter.Added;
			this.startFoldersUpdated = this.folderOperationCounter.Updated;
			this.startFoldersDeleted = this.folderOperationCounter.Deleted;
			this.startOrphanFoldersDetected = this.folderOperationCounter.OrphanDetected;
			this.startOrphanFoldersFixed = this.folderOperationCounter.OrphanFixed;
			this.startParentChainMissing = this.folderOperationCounter.ParentChainMissing;
			this.startSyncStateBytesReceived = this.syncStateCounter.BytesReceived;
			this.startSyncStateBytesSent = this.syncStateCounter.BytesSent;
			this.startWebServiceCount = this.mrsProxyLatencyInfo.TotalNumberOfRemoteCalls;
			this.startWebServiceDuration = this.mrsProxyLatencyInfo.TotalRemoteCallDuration;
			this.startTransientRetryDelayCount = this.transientRetryDelayTracker.RequestCount;
			this.startTransientRetryDelayLatency = this.transientRetryDelayTracker.Latency;
		}

		public override void Stop()
		{
			base.Stop();
			this.foldersAdded += this.folderOperationCounter.Added - this.startFoldersAdded;
			this.foldersUpdated += this.folderOperationCounter.Updated - this.startFoldersUpdated;
			this.foldersDeleted += this.folderOperationCounter.Deleted - this.startFoldersDeleted;
			this.orphanFoldersDetected += this.folderOperationCounter.OrphanDetected - this.startOrphanFoldersDetected;
			this.orphanFoldersFixed += this.folderOperationCounter.OrphanFixed - this.startOrphanFoldersFixed;
			this.parentChainMissing += this.folderOperationCounter.ParentChainMissing - this.startParentChainMissing;
			this.syncStateBytesReceived += this.syncStateCounter.BytesReceived - this.startSyncStateBytesReceived;
			this.syncStateBytesSent += this.syncStateCounter.BytesSent - this.startSyncStateBytesSent;
			this.webServiceCount += this.mrsProxyLatencyInfo.TotalNumberOfRemoteCalls - this.startWebServiceCount;
			this.webServiceLatency += this.mrsProxyLatencyInfo.TotalRemoteCallDuration - this.startWebServiceDuration;
			this.transientRetryDelayCount += this.transientRetryDelayTracker.RequestCount - this.startTransientRetryDelayCount;
			this.transientRetryDelayLatency += this.transientRetryDelayTracker.Latency - this.startTransientRetryDelayLatency;
		}

		public void AppendLogData(StringBuilder logData)
		{
			logData.Append("Activity=");
			logData.Append(this.trackedActivity);
			logData.Append(";CPU=");
			logData.Append(base.CpuTime.TotalMilliseconds);
			logData.Append(";DC=");
			logData.Append(base.DirectoryCount);
			logData.Append(";DL=");
			logData.Append(base.DirectoryLatency.TotalMilliseconds);
			logData.Append(";E=");
			logData.Append(base.ElapsedTime.TotalMilliseconds);
			logData.Append(";RPCC=");
			logData.Append(base.StoreRpcCount);
			logData.Append(";RPCL=");
			logData.Append(base.StoreRpcLatency.TotalMilliseconds);
			logData.Append(";C=");
			logData.Append(this.invokeCount);
			logData.Append(";FA=");
			logData.Append(this.foldersAdded);
			logData.Append(";FU=");
			logData.Append(this.foldersUpdated);
			logData.Append(";FD=");
			logData.Append(this.foldersDeleted);
			logData.Append(";SSR=");
			logData.Append(this.syncStateBytesReceived);
			logData.Append(";SSS=");
			logData.Append(this.syncStateBytesSent);
			logData.Append(";WC=");
			logData.Append(this.webServiceCount);
			logData.Append(";WL=");
			logData.Append(this.webServiceLatency.TotalMilliseconds);
			logData.Append(";TRDC=");
			logData.Append(this.transientRetryDelayCount);
			logData.Append(";TRDL=");
			logData.Append(this.transientRetryDelayLatency.TotalMilliseconds);
			logData.Append(";OD=");
			logData.Append(this.orphanFoldersDetected);
			logData.Append(";OF=");
			logData.Append(this.orphanFoldersFixed);
			logData.Append(";MPC=");
			logData.Append(this.parentChainMissing);
			logData.Append(";");
		}

		private readonly FolderOperationCounter folderOperationCounter;

		private readonly SyncStateCounter syncStateCounter;

		private readonly PerformanceDataProvider transientRetryDelayTracker;

		private readonly SyncActivity trackedActivity;

		private readonly LatencyInfo mrsProxyLatencyInfo;

		private int startFoldersAdded;

		private int startFoldersUpdated;

		private int startFoldersDeleted;

		private int startOrphanFoldersDetected;

		private int startOrphanFoldersFixed;

		private int startParentChainMissing;

		private long startSyncStateBytesReceived;

		private long startSyncStateBytesSent;

		private int startWebServiceCount;

		private TimeSpan startWebServiceDuration;

		private uint invokeCount;

		private int foldersAdded;

		private int foldersUpdated;

		private int foldersDeleted;

		private int orphanFoldersDetected;

		private int orphanFoldersFixed;

		private int parentChainMissing;

		private uint startTransientRetryDelayCount;

		private TimeSpan startTransientRetryDelayLatency;

		private uint transientRetryDelayCount;

		private TimeSpan transientRetryDelayLatency;

		private TimeSpan webServiceLatency;

		private int webServiceCount;

		private long syncStateBytesReceived;

		private long syncStateBytesSent;
	}
}
