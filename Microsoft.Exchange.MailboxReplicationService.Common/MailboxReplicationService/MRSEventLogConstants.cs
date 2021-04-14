using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal static class MRSEventLogConstants
	{
		public const string EventSource = "MSExchange Mailbox Replication";

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceStarted = new ExEventLog.EventTuple(263144U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceStopped = new ExEventLog.EventTuple(263145U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceIsDisabled = new ExEventLog.EventTuple(2147746794U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceFailedToRegisterEndpoint = new ExEventLog.EventTuple(2147746795U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceConfigCorrupt = new ExEventLog.EventTuple(3221488620U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToFindMbxServer = new ExEventLog.EventTuple(3221488621U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToProcessJobsInDatabase = new ExEventLog.EventTuple(2147746798U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToDetermineHostedMdbsOnServer = new ExEventLog.EventTuple(2147746799U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RemovedOrphanedMoveRequest = new ExEventLog.EventTuple(2147746800U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedInvalidRequest = new ExEventLog.EventTuple(2147746801U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceFailedToStart = new ExEventLog.EventTuple(3221488626U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RemovedCompletedRequest = new ExEventLog.EventTuple(263155U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CrashEvent = new ExEventLog.EventTuple(3221488628U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ScanADInconsistencyRequestFailEvent = new ExEventLog.EventTuple(3221488629U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RequestFatalFailure = new ExEventLog.EventTuple(3221488716U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RequestTransientFailure = new ExEventLog.EventTuple(2147746893U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MoveStarted = new ExEventLog.EventTuple(263246U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RequestContinued = new ExEventLog.EventTuple(263247U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MoveSeedingStarted = new ExEventLog.EventTuple(263248U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MoveSeedingCompleted = new ExEventLog.EventTuple(263249U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MoveFinalizationStarted = new ExEventLog.EventTuple(263250U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RequestCompleted = new ExEventLog.EventTuple(263251U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RequestCanceled = new ExEventLog.EventTuple(263252U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MoveIncrementalSyncCompleted = new ExEventLog.EventTuple(263253U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MoveUnableToApplySearchCriteria = new ExEventLog.EventTuple(2147746902U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MoveUnableToUpdateSourceMailbox = new ExEventLog.EventTuple(3221488727U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToCleanupCanceledRequest = new ExEventLog.EventTuple(2147746904U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToUpdateCompletedRequest = new ExEventLog.EventTuple(2147746905U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RequestSaveFailed = new ExEventLog.EventTuple(2147746906U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DestinationMailboxCleanupFailed = new ExEventLog.EventTuple(2147746907U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SourceMailboxResetFailed = new ExEventLog.EventTuple(2147746908U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SourceMailboxCleanupFailed = new ExEventLog.EventTuple(2147746909U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LocalDestinationMailboxResetFailed = new ExEventLog.EventTuple(2147746910U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReportFlushFailed = new ExEventLog.EventTuple(2147746911U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SyncStateSaveFailed = new ExEventLog.EventTuple(2147746912U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToProcessRequest = new ExEventLog.EventTuple(3221488737U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToPreserveMailboxSignature = new ExEventLog.EventTuple(2147746914U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MoveRestartedDueToSignatureChange = new ExEventLog.EventTuple(2147746915U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DestinationMailboxSeedMBICacheFailed = new ExEventLog.EventTuple(2147746916U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DestinationMailboxSyncStateDeletionFailed = new ExEventLog.EventTuple(2147746917U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DestinationMailboxMoveHistoryEntryFailed = new ExEventLog.EventTuple(2147746918U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RequestCompletedWithWarnings = new ExEventLog.EventTuple(2147746919U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToReadGlobalDatabaseState = new ExEventLog.EventTuple(2147746920U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToWriteGlobalDatabaseState = new ExEventLog.EventTuple(2147746921U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PeriodicTaskStoppingExecution = new ExEventLog.EventTuple(3221488746U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplicationConstraintCheckNotSatisfied = new ExEventLog.EventTuple(2147746923U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplicationConstraintCheckSatisfied = new ExEventLog.EventTuple(2147746924U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DestinationMailboxResetNotGuaranteed = new ExEventLog.EventTuple(2147746926U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADWriteFailed = new ExEventLog.EventTuple(2147746927U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RemoteDestinationMailboxResetFailed = new ExEventLog.EventTuple(2147746928U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RequestIsPoisoned = new ExEventLog.EventTuple(3221488753U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IncrementalMoveRestartDueToGlobalCounterRangeDepletion = new ExEventLog.EventTuple(2147746930U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SourceMailboxMoveHistoryEntryFailed = new ExEventLog.EventTuple(2147746931U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MoveRestartedDueToMailboxCorruption = new ExEventLog.EventTuple(2147746932U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		private enum Category : short
		{
			Service = 1,
			Request
		}

		internal enum Message : uint
		{
			ServiceStarted = 263144U,
			ServiceStopped,
			ServiceIsDisabled = 2147746794U,
			ServiceFailedToRegisterEndpoint,
			ServiceConfigCorrupt = 3221488620U,
			UnableToFindMbxServer,
			UnableToProcessJobsInDatabase = 2147746798U,
			UnableToDetermineHostedMdbsOnServer,
			RemovedOrphanedMoveRequest,
			FailedInvalidRequest,
			ServiceFailedToStart = 3221488626U,
			RemovedCompletedRequest = 263155U,
			CrashEvent = 3221488628U,
			ScanADInconsistencyRequestFailEvent,
			RequestFatalFailure = 3221488716U,
			RequestTransientFailure = 2147746893U,
			MoveStarted = 263246U,
			RequestContinued,
			MoveSeedingStarted,
			MoveSeedingCompleted,
			MoveFinalizationStarted,
			RequestCompleted,
			RequestCanceled,
			MoveIncrementalSyncCompleted,
			MoveUnableToApplySearchCriteria = 2147746902U,
			MoveUnableToUpdateSourceMailbox = 3221488727U,
			FailedToCleanupCanceledRequest = 2147746904U,
			FailedToUpdateCompletedRequest,
			RequestSaveFailed,
			DestinationMailboxCleanupFailed,
			SourceMailboxResetFailed,
			SourceMailboxCleanupFailed,
			LocalDestinationMailboxResetFailed,
			ReportFlushFailed,
			SyncStateSaveFailed,
			UnableToProcessRequest = 3221488737U,
			UnableToPreserveMailboxSignature = 2147746914U,
			MoveRestartedDueToSignatureChange,
			DestinationMailboxSeedMBICacheFailed,
			DestinationMailboxSyncStateDeletionFailed,
			DestinationMailboxMoveHistoryEntryFailed,
			RequestCompletedWithWarnings,
			UnableToReadGlobalDatabaseState,
			UnableToWriteGlobalDatabaseState,
			PeriodicTaskStoppingExecution = 3221488746U,
			ReplicationConstraintCheckNotSatisfied = 2147746923U,
			ReplicationConstraintCheckSatisfied,
			DestinationMailboxResetNotGuaranteed = 2147746926U,
			ADWriteFailed,
			RemoteDestinationMailboxResetFailed,
			RequestIsPoisoned = 3221488753U,
			IncrementalMoveRestartDueToGlobalCounterRangeDepletion = 2147746930U,
			SourceMailboxMoveHistoryEntryFailed,
			MoveRestartedDueToMailboxCorruption
		}
	}
}
