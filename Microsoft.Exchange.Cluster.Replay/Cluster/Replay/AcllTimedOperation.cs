using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal enum AcllTimedOperation
	{
		AcllQueuedOpStart = 1,
		AcllQueuedOpExecution,
		AcquireSuspendLock,
		EnsureTargetDismounted,
		FileCheckerAtStart,
		CleanUpTempIncReseedFiles,
		IsIncrementalReseedNecessary,
		AttemptFinalCopy,
		InspectFinalLogs,
		InspectE00Log,
		CheckRequiredLogFilesAtEnd,
		CopyLogsOverall,
		AcllEnterLogCopierWorkerLock,
		RunAcllInit,
		AcllLogCopierFirstInit,
		AcllInitWaitForReadCallback,
		ComputeLossAndMountAllowedOverall,
		SetLossVariables,
		ProtectUnboundedDataloss,
		MarkRedeliveryRequired,
		RollbackLastLogIfNecessary,
		SetE00LogGeneration
	}
}
