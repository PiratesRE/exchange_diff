using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class AcllPerformanceTracker : FailoverPerformanceTrackerBase<AcllTimedOperation>
	{
		public AcllPerformanceTracker(ReplayConfiguration config, string uniqueOperationId, int subactionAttemptNumber) : base("AcllPerf")
		{
			this.m_uniqueOperationId = uniqueOperationId;
			this.m_subactionAttemptNumber = subactionAttemptNumber;
			this.m_config = config;
		}

		public bool IsSkipHealthChecks { private get; set; }

		public bool IsPrepareToStopCalled { private get; set; }

		public bool IsNewCopierInspectorCreated { private get; set; }

		public bool IsGranuleUsedAsE00 { private get; set; }

		public bool IsE00EndOfLogStreamAfterIncReseed { private get; set; }

		public bool IsAcllFoundDeadConnection { private get; set; }

		public bool IsAcllCouldNotControlLogCopier { private get; set; }

		public bool IsLogCopierInitializedForAcll { private get; set; }

		public ReplicaInstanceStage ReplicaInstanceStage { private get; set; }

		public CopyStatusEnum CopyStatus { private get; set; }

		public long CopyQueueLengthAcllStart { get; set; }

		public long ReplayQueueLengthAcllStart { private get; set; }

		public long ReplayQueueLengthAcllEnd { private get; set; }

		public long NumberOfLogsLost { private get; set; }

		public long NumberOfLogsCopied { private get; set; }

		public override void LogEvent()
		{
			ReplayCrimsonEvents.AcllPerformance.Log<string, string, Guid, int, bool, bool, bool, bool, bool, bool, ReplicaInstanceStage, CopyStatusEnum, long, long, long, long, long, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, bool, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, bool, TimeSpan>(this.m_uniqueOperationId, this.m_config.DatabaseName, this.m_config.IdentityGuid, this.m_subactionAttemptNumber, this.IsSkipHealthChecks, this.IsPrepareToStopCalled, this.IsNewCopierInspectorCreated, this.IsGranuleUsedAsE00, this.IsAcllFoundDeadConnection, this.IsAcllCouldNotControlLogCopier, this.ReplicaInstanceStage, this.CopyStatus, this.CopyQueueLengthAcllStart, this.ReplayQueueLengthAcllStart, this.ReplayQueueLengthAcllEnd, this.NumberOfLogsLost, this.NumberOfLogsCopied, base.GetDuration(AcllTimedOperation.AcllQueuedOpStart), base.GetDuration(AcllTimedOperation.AcllQueuedOpExecution), base.GetDuration(AcllTimedOperation.AcquireSuspendLock), base.GetDuration(AcllTimedOperation.EnsureTargetDismounted), base.GetDuration(AcllTimedOperation.FileCheckerAtStart), base.GetDuration(AcllTimedOperation.CleanUpTempIncReseedFiles), base.GetDuration(AcllTimedOperation.IsIncrementalReseedNecessary), base.GetDuration(AcllTimedOperation.AttemptFinalCopy), base.GetDuration(AcllTimedOperation.InspectFinalLogs), base.GetDuration(AcllTimedOperation.InspectE00Log), base.GetDuration(AcllTimedOperation.CheckRequiredLogFilesAtEnd), base.GetDuration(AcllTimedOperation.CopyLogsOverall), base.GetDuration(AcllTimedOperation.AcllEnterLogCopierWorkerLock), base.GetDuration(AcllTimedOperation.RunAcllInit), this.IsLogCopierInitializedForAcll, base.GetDuration(AcllTimedOperation.AcllLogCopierFirstInit), base.GetDuration(AcllTimedOperation.AcllInitWaitForReadCallback), base.GetDuration(AcllTimedOperation.ComputeLossAndMountAllowedOverall), base.GetDuration(AcllTimedOperation.SetLossVariables), base.GetDuration(AcllTimedOperation.ProtectUnboundedDataloss), base.GetDuration(AcllTimedOperation.MarkRedeliveryRequired), base.GetDuration(AcllTimedOperation.RollbackLastLogIfNecessary), this.IsE00EndOfLogStreamAfterIncReseed, base.GetDuration(AcllTimedOperation.SetE00LogGeneration));
		}

		private ReplayConfiguration m_config;

		private string m_uniqueOperationId;

		private int m_subactionAttemptNumber;
	}
}
