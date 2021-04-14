using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.ReplayEventLog
{
	public static class ReplayEventLogConstants
	{
		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceStarted = new ExEventLog.EventTuple(1074005969U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceStopped = new ExEventLog.EventTuple(1074005970U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AllFacilitiesAreOnline = new ExEventLog.EventTuple(1074005971U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceStarting = new ExEventLog.EventTuple(1074005972U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceStopping = new ExEventLog.EventTuple(1074005973U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SourceInstanceStart = new ExEventLog.EventTuple(1074005976U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SourceInstanceStop = new ExEventLog.EventTuple(1074005977U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TargetInstanceStart = new ExEventLog.EventTuple(1074005978U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TargetInstanceStop = new ExEventLog.EventTuple(1074005979U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InstanceBroken = new ExEventLog.EventTuple(3221489628U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_E00LogMoved = new ExEventLog.EventTuple(1074005981U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CheckpointDeleted = new ExEventLog.EventTuple(1074005982U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VssInitFailed = new ExEventLog.EventTuple(3221489633U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterInitialize = new ExEventLog.EventTuple(1074005986U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterTerminate = new ExEventLog.EventTuple(1074005987U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterMetadata = new ExEventLog.EventTuple(1074005989U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterMetadataError = new ExEventLog.EventTuple(3221489638U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterBackup = new ExEventLog.EventTuple(1074005991U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterBackupError = new ExEventLog.EventTuple(3221489640U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterSnapshot = new ExEventLog.EventTuple(1074005993U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterSnapshotError = new ExEventLog.EventTuple(3221489642U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterFreeze = new ExEventLog.EventTuple(1074005995U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterFreezeError = new ExEventLog.EventTuple(3221489644U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterThaw = new ExEventLog.EventTuple(1074005997U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterThawError = new ExEventLog.EventTuple(3221489646U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterAbort = new ExEventLog.EventTuple(1074005999U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterAbortError = new ExEventLog.EventTuple(3221489648U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterBackupComplete = new ExEventLog.EventTuple(1074006001U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterBackupCompleteError = new ExEventLog.EventTuple(3221489650U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterPostSnapshot = new ExEventLog.EventTuple(1074006003U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterOnBackupShutdownError = new ExEventLog.EventTuple(3221489652U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterOnBackupShutdown = new ExEventLog.EventTuple(1074006005U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterBackupCompleteFailureWarning = new ExEventLog.EventTuple(2147747830U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterPostSnapshotError = new ExEventLog.EventTuple(3221489655U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterAddComponentsError = new ExEventLog.EventTuple(3221489656U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterAddDatabaseComponentError = new ExEventLog.EventTuple(3221489657U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterDbFileInfoError = new ExEventLog.EventTuple(3221489658U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterSetPrivateMetadataError = new ExEventLog.EventTuple(3221489659U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterCheckInstanceVolumeDependenciesError = new ExEventLog.EventTuple(3221489660U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterCheckDatabaseVolumeDependenciesError = new ExEventLog.EventTuple(3221489661U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterBackupCompleteLogsTruncated = new ExEventLog.EventTuple(1074006014U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterBackupCompleteNoTruncateRequested = new ExEventLog.EventTuple(1074006015U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterBackupCompleteWithFailure = new ExEventLog.EventTuple(3221489664U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterBackupCompleteUnknownGuid = new ExEventLog.EventTuple(3221489665U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterBackupCompleteWithFailureAndUnknownGuid = new ExEventLog.EventTuple(3221489666U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterRestoreFailed = new ExEventLog.EventTuple(3221489667U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSSnapshotWriter = new ExEventLog.EventTuple(1074006020U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OnlineDatabaseFailed = new ExEventLog.EventTuple(3221489669U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToCreateTempLogFile = new ExEventLog.EventTuple(3221489671U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ReseedRequired = new ExEventLog.EventTuple(3221489672U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_IncrementalReseedBlocked = new ExEventLog.EventTuple(3221489673U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_IncrementalReseedError = new ExEventLog.EventTuple(3221489674U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LogFileGapFound = new ExEventLog.EventTuple(3221489675U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigurationCheckerFailedTransient = new ExEventLog.EventTuple(3221489676U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AttemptCopyLastLogsFailed = new ExEventLog.EventTuple(3221489677U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_NoDatabasesInReplica = new ExEventLog.EventTuple(3221489678U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidFilePath = new ExEventLog.EventTuple(3221489682U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LogFileCorruptError = new ExEventLog.EventTuple(3221489683U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LogInspectorFailed = new ExEventLog.EventTuple(3221489684U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CouldNotDeleteLogFile = new ExEventLog.EventTuple(3221489685U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FileCheckError = new ExEventLog.EventTuple(3221489686U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayCheckError = new ExEventLog.EventTuple(3221489687U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToCreateDirectory = new ExEventLog.EventTuple(3221489689U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_NoDirectory = new ExEventLog.EventTuple(3221489690U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FilesystemCorrupt = new ExEventLog.EventTuple(3221489691U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SavedStateError = new ExEventLog.EventTuple(3221489693U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CouldNotCompareLogFiles = new ExEventLog.EventTuple(3221489697U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CouldNotCreateNetworkShare = new ExEventLog.EventTuple(3221489698U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RestoreDatabaseCopySuccessful = new ExEventLog.EventTuple(1074006053U, 5, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RestoreDatabaseCopySuccessfulPathsChanged = new ExEventLog.EventTuple(1074006054U, 5, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RestoreDatabaseCopyIncomplete = new ExEventLog.EventTuple(1074006055U, 5, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RestoreDatabaseCopyIncompletePathsChanged = new ExEventLog.EventTuple(1074006056U, 5, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RestoreDatabaseCopyFailed = new ExEventLog.EventTuple(1074006057U, 5, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AutoMountReportNoLoss = new ExEventLog.EventTuple(1074006058U, 5, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AutoMountReportMountWithDataLoss = new ExEventLog.EventTuple(2147747883U, 5, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AutoMountReportMountNotAllowed = new ExEventLog.EventTuple(3221489708U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AutoMountReportMountAfter = new ExEventLog.EventTuple(2147747885U, 5, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AutoMountReportPublicFolderMountNotAllowed = new ExEventLog.EventTuple(2147747886U, 5, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CouldNotReplayLogFile = new ExEventLog.EventTuple(3221489711U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ReseedCheckMissingLogfile = new ExEventLog.EventTuple(3221489712U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_IsamException = new ExEventLog.EventTuple(3221489713U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorCouldNotRecopyLogFile = new ExEventLog.EventTuple(3221489714U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseSubmitDumpsterMessages = new ExEventLog.EventTuple(1074006067U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReseederDeletedCheckpointFile = new ExEventLog.EventTuple(1074006068U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigurationCheckerFailedADError = new ExEventLog.EventTuple(3221489717U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReseederDeletedTargetDatabaseFile = new ExEventLog.EventTuple(1074006070U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ShipLogFailed = new ExEventLog.EventTuple(3221489720U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReseederDeletedExistingLogs = new ExEventLog.EventTuple(1074006073U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseNotPresentAfterReplay = new ExEventLog.EventTuple(3221489722U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SeederPerfCountersLoadFailure = new ExEventLog.EventTuple(3221489725U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterBackupDatabaseFullCopy = new ExEventLog.EventTuple(1074006078U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterBackupDatabaseIncrementalDifferential = new ExEventLog.EventTuple(1074006079U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterBackupDatabaseError = new ExEventLog.EventTuple(3221489728U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseDumpsterRedeliveryRequired = new ExEventLog.EventTuple(1074006081U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplicaInstanceLogCopied = new ExEventLog.EventTuple(1074006082U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplicaInstanceLogsReplayed = new ExEventLog.EventTuple(1074006083U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplicaInstanceStartIncrementalReseed = new ExEventLog.EventTuple(1074006084U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplicaInstanceFinishIncrementalReseed = new ExEventLog.EventTuple(1074006085U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterOrphanedBackupInstance = new ExEventLog.EventTuple(2147747910U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterMissingFile = new ExEventLog.EventTuple(3221489735U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HTTPListenerFailedToStart = new ExEventLog.EventTuple(3221489736U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TcpListenerFailedToStart = new ExEventLog.EventTuple(3221489737U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ScrConfigPathConflict = new ExEventLog.EventTuple(3221489741U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MultiplePathNext = new ExEventLog.EventTuple(1074006094U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NetworkPathNext = new ExEventLog.EventTuple(1074006095U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogCopyFailedDueToDuplicateName = new ExEventLog.EventTuple(3221489744U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ScrConfigExceedLimit = new ExEventLog.EventTuple(2147747921U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ScrConfigConflictWithDb = new ExEventLog.EventTuple(2147747922U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSReplicaBroken = new ExEventLog.EventTuple(3221489747U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSReplicaSuspend = new ExEventLog.EventTuple(3221489748U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RpcServerStarted = new ExEventLog.EventTuple(1074006101U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RpcServerStopped = new ExEventLog.EventTuple(1074006102U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RpcServerFailedToStart = new ExEventLog.EventTuple(3221489751U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LogTruncationOpenFailed = new ExEventLog.EventTuple(2147747928U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogTruncationSourceFailure = new ExEventLog.EventTuple(2147747929U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogTruncationLocalFailure = new ExEventLog.EventTuple(2147747930U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterException = new ExEventLog.EventTuple(3221489756U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RpcServerFailedToFindExchangeServersUsg = new ExEventLog.EventTuple(3221489757U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InstanceFailedToStart = new ExEventLog.EventTuple(3221489758U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InstanceFailedToDeleteRegistryStateWarning = new ExEventLog.EventTuple(2147747935U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_NoDirectoryHostInaccessible = new ExEventLog.EventTuple(3221489761U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ExceptionDuringCallback = new ExEventLog.EventTuple(3221489762U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AlternateNetworkHadProblem = new ExEventLog.EventTuple(3221489763U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LogCopierFoundNoLogsOnSource = new ExEventLog.EventTuple(3221489766U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LogCopierFailedDueToSource = new ExEventLog.EventTuple(3221489767U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LogCopierFailedDueToTarget = new ExEventLog.EventTuple(3221489768U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LogCopierFailedToCommunicate = new ExEventLog.EventTuple(3221489769U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TPRExchangeListenerStarted = new ExEventLog.EventTuple(1074006122U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TPRManagerInitFailure = new ExEventLog.EventTuple(3221489771U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LogCopierErrorOnSource = new ExEventLog.EventTuple(3221489772U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplicaInstanceMadeProgress = new ExEventLog.EventTuple(1074006125U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LogCopierErrorOnSourceTriggerFailover = new ExEventLog.EventTuple(3221489774U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LogCopierIsStalledDueToSource = new ExEventLog.EventTuple(3221489775U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CorruptLogRecoveryIsAttempted = new ExEventLog.EventTuple(3221489776U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CorruptLogRecoveryFailedToSuspend = new ExEventLog.EventTuple(3221489777U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CorruptLogRecoveryFailedToDismount = new ExEventLog.EventTuple(3221489778U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LogCopierReceivedSourceSideError = new ExEventLog.EventTuple(3221489779U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LogCopierBlockedByFullDisk = new ExEventLog.EventTuple(3221489780U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LogCorruptionTriggersFailover = new ExEventLog.EventTuple(3221489781U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LogCopierDisconnectedTooLong = new ExEventLog.EventTuple(3221489782U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CorruptLogDetectedOnActive = new ExEventLog.EventTuple(3221489783U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorReadingLogOnActive = new ExEventLog.EventTuple(3221489784U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CorruptLogRepaired = new ExEventLog.EventTuple(2147747961U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SlowIoDetected = new ExEventLog.EventTuple(2147747962U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CorruptLogRecoveryIsImmediatelyAttempted = new ExEventLog.EventTuple(3221489787U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ResumeFailedDuringFailureItemProcessing = new ExEventLog.EventTuple(3221489788U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InspectorFixedCorruptLog = new ExEventLog.EventTuple(2147747965U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LogCopierDetectsPossibleLogStreamReset = new ExEventLog.EventTuple(2147747966U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LogCopierFailedToTransitOutOfBlockMode = new ExEventLog.EventTuple(2147747967U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InspectorDetectedCorruptLog = new ExEventLog.EventTuple(3221489792U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FatalIOErrorEncountered = new ExEventLog.EventTuple(3221489793U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PassiveMonitoredDBFailedToStart = new ExEventLog.EventTuple(3221489794U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ActiveMonitoredDBFailedToStart = new ExEventLog.EventTuple(3221489795U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSWriterMissingLogFileSignature = new ExEventLog.EventTuple(3221489796U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NetworkRoleChanged = new ExEventLog.EventTuple(2147747992U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RegistryReplicatorException = new ExEventLog.EventTuple(3221489817U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ClusterApiHungAlert = new ExEventLog.EventTuple(2147748174U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IncrementalReseedInitException = new ExEventLog.EventTuple(3221490760U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IncrementalReseedFailedError = new ExEventLog.EventTuple(3221490761U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IncrementalReseedRetryableError = new ExEventLog.EventTuple(3221490762U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IncrementalReseedPrereqError = new ExEventLog.EventTuple(3221490763U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IncSeedingStarted = new ExEventLog.EventTuple(1074007116U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IncSeedingComplete = new ExEventLog.EventTuple(1074007117U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IncSeedingSourceReleased = new ExEventLog.EventTuple(1074007118U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmChangingRole = new ExEventLog.EventTuple(1074007119U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmStoreServiceStarted = new ExEventLog.EventTuple(1074007120U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmInitiatingNodeFailover = new ExEventLog.EventTuple(1074007121U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmDatabaseMountFailed = new ExEventLog.EventTuple(3221490770U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmDatabaseMounted = new ExEventLog.EventTuple(1074007124U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmDetectedNodeStateChange = new ExEventLog.EventTuple(1074007125U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmStartingAutoMount = new ExEventLog.EventTuple(1074007126U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmIgnoringDatabaseMount = new ExEventLog.EventTuple(1074007127U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmDatabaseDismounted = new ExEventLog.EventTuple(1074007129U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmKnownError = new ExEventLog.EventTuple(3221490778U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmUnknownCrticalError = new ExEventLog.EventTuple(3221490779U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmErrorReadingConfiguration = new ExEventLog.EventTuple(3221490780U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmCriticalErrorReadingConfiguration = new ExEventLog.EventTuple(3221490781U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmMoveNotApplicableForDatabase = new ExEventLog.EventTuple(2147748958U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmFailedToAutomountDatabase = new ExEventLog.EventTuple(3221490756U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SuspendMarkedForDatabaseCopy = new ExEventLog.EventTuple(1074007110U, 5, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ResumeMarkedForDatabaseCopy = new ExEventLog.EventTuple(1074007111U, 5, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MountAllowedWithMountDialOverride = new ExEventLog.EventTuple(1074007135U, 5, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MountNotAllowedWithMountDialOverride = new ExEventLog.EventTuple(3221490784U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmDatabaseMoved = new ExEventLog.EventTuple(1074007137U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmDatabaseMoveFailed = new ExEventLog.EventTuple(3221490786U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DbSeedingRequired = new ExEventLog.EventTuple(2147748963U, 5, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseExistsInADButRegistryDeleted = new ExEventLog.EventTuple(3221490788U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmRpcServerStarted = new ExEventLog.EventTuple(1074007141U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmRpcServerStopped = new ExEventLog.EventTuple(1074007142U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmRpcServerFailedToStart = new ExEventLog.EventTuple(3221490791U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmRpcServerFailedToFindExchangeServersUsg = new ExEventLog.EventTuple(3221490792U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceFailedToStartAMFailure = new ExEventLog.EventTuple(3221490793U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmDatabaseNotMountedServersDown = new ExEventLog.EventTuple(3221490794U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmForceDismountingDatabases = new ExEventLog.EventTuple(2147748971U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmDatabaseAcllComplete = new ExEventLog.EventTuple(1074007150U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmDatabaseAcllFailed = new ExEventLog.EventTuple(3221490799U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedMovePAM = new ExEventLog.EventTuple(3221490800U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SuccMovePAM = new ExEventLog.EventTuple(1074007153U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AmIgnoredMapiNetFailureBecauseNodeNotUp = new ExEventLog.EventTuple(2147748978U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmKilledStoreToForceDismount = new ExEventLog.EventTuple(3221490803U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmFailedToStopService = new ExEventLog.EventTuple(3221490804U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmFailedToStartService = new ExEventLog.EventTuple(3221490805U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AMDetectedMapiNetworkFailure = new ExEventLog.EventTuple(3221490806U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AmIgnoredMapiNetFailureBecauseMapiLooksUp = new ExEventLog.EventTuple(2147748983U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AmIgnoredMapiNetFailureBecauseADIsWorking = new ExEventLog.EventTuple(2147748984U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AmIgnoredMapiNetFailureBecauseNotThePam = new ExEventLog.EventTuple(2147748985U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PauseSuccessful = new ExEventLog.EventTuple(1074007970U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_StopFailed = new ExEventLog.EventTuple(3221491619U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_StartFailed = new ExEventLog.EventTuple(3221491620U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PauseFailed = new ExEventLog.EventTuple(3221491622U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CommandOK = new ExEventLog.EventTuple(3221491621U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CommandFailed = new ExEventLog.EventTuple(3221491623U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PowerEventOK = new ExEventLog.EventTuple(3221491624U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PowerEventFailed = new ExEventLog.EventTuple(3221491625U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SessionChangeFailed = new ExEventLog.EventTuple(3221491626U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ShutdownOK = new ExEventLog.EventTuple(3221491627U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ShutdownFailed = new ExEventLog.EventTuple(3221491628U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CommandSuccessful = new ExEventLog.EventTuple(3221491629U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ContinueSuccessful = new ExEventLog.EventTuple(3221491630U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ContinueFailed = new ExEventLog.EventTuple(3221491631U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToUnloadAppDomain = new ExEventLog.EventTuple(3221491632U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PreShutdownOK = new ExEventLog.EventTuple(1074007985U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PreShutdownFailed = new ExEventLog.EventTuple(3221491634U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PreShutdownStart = new ExEventLog.EventTuple(1074007987U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SeedManagerStarted = new ExEventLog.EventTuple(1074007988U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SeedManagerStopped = new ExEventLog.EventTuple(1074007989U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SeedInstancePrepareAdded = new ExEventLog.EventTuple(1074007990U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SeedInstancePrepareSucceeded = new ExEventLog.EventTuple(1074007991U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SeedInstancePrepareUnknownError = new ExEventLog.EventTuple(3221491640U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SeedInstancePrepareFailed = new ExEventLog.EventTuple(3221491641U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SeedInstanceInProgressFailed = new ExEventLog.EventTuple(3221491642U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SeedInstanceCancelled = new ExEventLog.EventTuple(2147749819U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SeedInstanceBeginSucceeded = new ExEventLog.EventTuple(1074007996U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SeedInstanceBeginUnknownError = new ExEventLog.EventTuple(3221491645U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SeedInstanceCancelRequestedByAdmin = new ExEventLog.EventTuple(1074007998U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SeedInstanceCleanupRequestedByAdmin = new ExEventLog.EventTuple(1074007999U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SeedInstanceCleanupConfigChanged = new ExEventLog.EventTuple(1074008000U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SeedInstanceCleanupStale = new ExEventLog.EventTuple(1074008001U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SeedInstancesStoppedServiceShutdown = new ExEventLog.EventTuple(2147749826U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SeedInstanceDeletedExistingLogs = new ExEventLog.EventTuple(1074008003U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SeedInstanceDeletedCheckpointFile = new ExEventLog.EventTuple(1074008004U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SeedInstanceSuccess = new ExEventLog.EventTuple(1074008005U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringClusterServiceCheckFailed = new ExEventLog.EventTuple(3221491654U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringClusterServiceCheckPassed = new ExEventLog.EventTuple(1074008007U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringActiveManagerCheckFailed = new ExEventLog.EventTuple(3221491656U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringActiveManagerCheckPassed = new ExEventLog.EventTuple(1074008009U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringReplayServiceCheckFailed = new ExEventLog.EventTuple(3221491658U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringReplayServiceCheckPassed = new ExEventLog.EventTuple(1074008011U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringDagMembersUpCheckFailed = new ExEventLog.EventTuple(3221491660U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringDagMembersUpCheckPassed = new ExEventLog.EventTuple(1074008013U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringClusterNetworkCheckFailed = new ExEventLog.EventTuple(3221491662U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringClusterNetworkCheckWarning = new ExEventLog.EventTuple(2147749839U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringClusterNetworkCheckPassed = new ExEventLog.EventTuple(1074008016U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringFileShareQuorumCheckFailed = new ExEventLog.EventTuple(3221491665U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringFileShareQuorumCheckPassed = new ExEventLog.EventTuple(1074008018U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringQuorumGroupCheckFailed = new ExEventLog.EventTuple(3221491667U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringQuorumGroupCheckPassed = new ExEventLog.EventTuple(1074008020U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringTasksRpcListenerCheckFailed = new ExEventLog.EventTuple(3221491669U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringTasksRpcListenerCheckPassed = new ExEventLog.EventTuple(1074008022U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringHttpListenerCheckFailed = new ExEventLog.EventTuple(3221491671U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringHttpListenerCheckPassed = new ExEventLog.EventTuple(1074008024U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LogReplayMapiException = new ExEventLog.EventTuple(3221491673U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseOperationLockIsTakingLongTime = new ExEventLog.EventTuple(2147749850U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CiSeedInstanceSuccess = new ExEventLog.EventTuple(1074008027U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SourceReplicaInstanceNotStarted = new ExEventLog.EventTuple(3221491676U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TargetReplicaInstanceNotStarted = new ExEventLog.EventTuple(3221491677U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SubmitDumpsterMessagesFailed = new ExEventLog.EventTuple(3221491681U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ClusterDatabaseWriteFailed = new ExEventLog.EventTuple(3221491682U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IncSeedingTerminated = new ExEventLog.EventTuple(2147749859U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AmRoleMonitoringError = new ExEventLog.EventTuple(3221491684U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AutoMountReportDbInUseOnSource = new ExEventLog.EventTuple(2147749861U, 5, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AutoMountReportDbInUseAcllInProgress = new ExEventLog.EventTuple(2147749862U, 5, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmDatabaseDismountFailed = new ExEventLog.EventTuple(3221491687U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmForceDismountMasterMismatch = new ExEventLog.EventTuple(2147749864U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmDatabaseMountFailedGeneric = new ExEventLog.EventTuple(3221491689U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmDatabaseDismountFailedGeneric = new ExEventLog.EventTuple(3221491690U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmDatabaseMoveFailedGeneric = new ExEventLog.EventTuple(3221491691U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_NetworkReplicationDisabled = new ExEventLog.EventTuple(3221491692U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringTcpListenerCheckFailed = new ExEventLog.EventTuple(3221491696U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringTcpListenerCheckPassed = new ExEventLog.EventTuple(1074008049U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_NetworkMonitoringError = new ExEventLog.EventTuple(3221491698U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SeedInstanceAnotherError = new ExEventLog.EventTuple(3221491699U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ForceNewLogError = new ExEventLog.EventTuple(3221491700U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ReadOnePageError = new ExEventLog.EventTuple(3221491701U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ReadPageSizeError = new ExEventLog.EventTuple(3221491702U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmDatabaseMoveUnspecifiedServerFailed = new ExEventLog.EventTuple(3221491703U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VSSReplicaCopyUnhealthy = new ExEventLog.EventTuple(3221491704U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_IncrementalReseedSourceDatabaseDismounted = new ExEventLog.EventTuple(3221491705U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_IncrementalReseedSourceDatabaseMountRpcError = new ExEventLog.EventTuple(3221491706U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DumpsterRedeliveryFailed = new ExEventLog.EventTuple(3221491707U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_NodeNotInCluster = new ExEventLog.EventTuple(3221491708U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayRpcServerFailedToRegister = new ExEventLog.EventTuple(3221491709U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AmRpcServerFailedToRegister = new ExEventLog.EventTuple(3221491710U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceFailedToStartComponentFailure = new ExEventLog.EventTuple(3221491711U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToStartRetriableComponent = new ExEventLog.EventTuple(3221491712U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigUpdaterScanFailed = new ExEventLog.EventTuple(3221491713U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigUpdaterFailedToFindConfig = new ExEventLog.EventTuple(3221491714U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringTPRListenerCheckFailed = new ExEventLog.EventTuple(3221491715U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringTPRListenerCheckPassed = new ExEventLog.EventTuple(1074008068U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EsebcliTooManyApplications = new ExEventLog.EventTuple(3221491717U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SeedingSourceBegin = new ExEventLog.EventTuple(1074008070U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SeedingSourceEnd = new ExEventLog.EventTuple(1074008071U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SeedingSourceCancel = new ExEventLog.EventTuple(2147749896U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CheckConnectionToStoreFailed = new ExEventLog.EventTuple(3221491721U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CheckDatabaseHeaderFailed = new ExEventLog.EventTuple(3221491722U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PossibleSplitBrainDetected = new ExEventLog.EventTuple(3221491723U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToCleanUpSingleIncReseedFile = new ExEventLog.EventTuple(3221491724U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToCleanUpFile = new ExEventLog.EventTuple(3221491725U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LogReplaySuspendedDueToCopyQ = new ExEventLog.EventTuple(2147749902U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LogReplayResumedDueToCopyQ = new ExEventLog.EventTuple(1074008079U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SyncSuspendResumeOperationFailed = new ExEventLog.EventTuple(3221491728U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringDatabaseRedundancyCheckFailed = new ExEventLog.EventTuple(3221491729U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringDatabaseRedundancyCheckPassed = new ExEventLog.EventTuple(1074008082U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseRedistributionReport = new ExEventLog.EventTuple(1074008083U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FqdnResolutionFailure = new ExEventLog.EventTuple(3221491732U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LogReplayPatchFailedIsamException = new ExEventLog.EventTuple(3221491733U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RedirtyDatabaseCreateTempLog = new ExEventLog.EventTuple(3221491734U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LogReplayPatchFailedPrepareException = new ExEventLog.EventTuple(3221491735U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToPublishFailureItem = new ExEventLog.EventTuple(3221491736U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PeriodicOperationFailedRetrievingStatuses = new ExEventLog.EventTuple(2147749913U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ProcessDiagnosticsTerminatingService = new ExEventLog.EventTuple(3221491738U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GetBootTimeWithWmiFailure = new ExEventLog.EventTuple(3221491739U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogRepairFailedDueToRetryLimit = new ExEventLog.EventTuple(3221491740U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogRepairSuccess = new ExEventLog.EventTuple(2147749917U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigurationCheckerFailedGeneric = new ExEventLog.EventTuple(3221491742U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogFileCorruptOrGapFoundOutsideRequiredRange = new ExEventLog.EventTuple(3221491743U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MovingFilesToRestartLogStream = new ExEventLog.EventTuple(2147749920U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DeletedSkippedLogsDirectory = new ExEventLog.EventTuple(2147749921U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MissingFailureItemDetected = new ExEventLog.EventTuple(3221491746U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LogReplayPatchFailedOnLaggedCopy = new ExEventLog.EventTuple(3221491747U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringDatabaseOneDatacenterCheckFailed = new ExEventLog.EventTuple(3221491749U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ActiveSeedingSourceBegin = new ExEventLog.EventTuple(1074008102U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ActiveSeedingSourceEnd = new ExEventLog.EventTuple(1074008103U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ActiveSeedingSourceCancel = new ExEventLog.EventTuple(2147749928U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogRepairNotPossible = new ExEventLog.EventTuple(3221491753U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringDatabaseRedundancyServerCheckFailed = new ExEventLog.EventTuple(3221491754U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringDatabaseRedundancyServerCheckPassed = new ExEventLog.EventTuple(1074008107U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DetermineWorkerProcessIdFailed = new ExEventLog.EventTuple(3221491756U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DagNetworkConfigOld = new ExEventLog.EventTuple(1074008268U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DagNetworkConfigNew = new ExEventLog.EventTuple(1074008269U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NotifyActiveSendFailed = new ExEventLog.EventTuple(3221491927U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AmCheckRemoteSiteLocalServerSiteNull = new ExEventLog.EventTuple(2147750104U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AmCheckRemoteSiteNotFound = new ExEventLog.EventTuple(1074008281U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AmCheckRemoteSiteAlert = new ExEventLog.EventTuple(3221491930U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AmCheckRemoteSiteDismount = new ExEventLog.EventTuple(3221491931U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AmCheckRemoteSiteSucceeded = new ExEventLog.EventTuple(1074008284U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AmCheckRemoteSiteDisabled = new ExEventLog.EventTuple(2147750109U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiskHbFailedToStartWatchdogTimer = new ExEventLog.EventTuple(3221491934U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiskHbFailedToPrepareHeartbeatFile = new ExEventLog.EventTuple(3221491935U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiskHbEncounteredUnhandledException = new ExEventLog.EventTuple(3221491936U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiskHbFailedToDetermineBytesPerSector = new ExEventLog.EventTuple(3221491937U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiskHbIoWriteTestFailed = new ExEventLog.EventTuple(3221491938U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiskHbIoReadTestFailed = new ExEventLog.EventTuple(3221491939U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiskHbIoLatencyExceeded = new ExEventLog.EventTuple(2147750116U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiskHbFailedToStart = new ExEventLog.EventTuple(3221491941U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiskHbFailedToStop = new ExEventLog.EventTuple(3221491942U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AmTimeStampEntryMissingInOneOrMoreServers = new ExEventLog.EventTuple(1074008295U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AutoReseedInstanceStarted = new ExEventLog.EventTuple(1074008296U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AutoReseedPrereqFailed = new ExEventLog.EventTuple(3221491945U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AutoReseedNoSpareDisk = new ExEventLog.EventTuple(3221491946U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AutoReseedFailed = new ExEventLog.EventTuple(3221491947U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AutoReseedSuccessful = new ExEventLog.EventTuple(1074008300U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AutoReseedSpareDisksReleased = new ExEventLog.EventTuple(1074008301U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringDatabaseOneDatacenterCheckSuccess = new ExEventLog.EventTuple(1074008302U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SeedInstanceStartedSetBroken = new ExEventLog.EventTuple(3221491951U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LogReplayGenericError = new ExEventLog.EventTuple(3221491952U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterMissingVersionStampError = new ExEventLog.EventTuple(3221491953U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterBadVersionStampError = new ExEventLog.EventTuple(3221491954U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterRestoreOptionsString = new ExEventLog.EventTuple(1074008307U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterDbGuidMappingMismatchError = new ExEventLog.EventTuple(3221491956U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterMultipleRetargettingError = new ExEventLog.EventTuple(3221491957U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterOriginalLogfilePathMismatchError = new ExEventLog.EventTuple(3221491958U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterOriginalSystemPathMismatchError = new ExEventLog.EventTuple(3221491959U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterDbRetargetMismatchError = new ExEventLog.EventTuple(3221491960U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterTargetSGLookupError = new ExEventLog.EventTuple(3221491961U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterTargetSGOnline = new ExEventLog.EventTuple(3221491962U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterRestoreToOriginalSG = new ExEventLog.EventTuple(1074008315U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterRestoreToAlternateSG = new ExEventLog.EventTuple(1074008316U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterTargetLogfilePathMismatchError = new ExEventLog.EventTuple(3221491965U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterTargetSystemPathMismatchError = new ExEventLog.EventTuple(3221491966U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterTargetLogfileBaseNameMismatchError = new ExEventLog.EventTuple(3221491967U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterSGRestoreInProgressError = new ExEventLog.EventTuple(3221491968U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterCircularLogDBRestore = new ExEventLog.EventTuple(3221491969U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterTargetDbMismatchError = new ExEventLog.EventTuple(3221491970U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterCannotOverwriteError = new ExEventLog.EventTuple(3221491971U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterFindLogFilesError = new ExEventLog.EventTuple(3221491972U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterLocationRestoreInProgressError = new ExEventLog.EventTuple(3221491973U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterTargetLogfilePathInUseError = new ExEventLog.EventTuple(3221491974U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterRestoreEnvSGMismatchError = new ExEventLog.EventTuple(3221491975U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterRestoreEnvLogfilePathMismatchError = new ExEventLog.EventTuple(3221491976U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterRestoreEnvLogfileBaseNameMismatchError = new ExEventLog.EventTuple(3221491977U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterRestoreEnvLogfileSignatureMismatchError = new ExEventLog.EventTuple(3221491978U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterRestoreEnvSystemPathMismatchError = new ExEventLog.EventTuple(3221491979U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterRestoreEnvCircularLogEnabledError = new ExEventLog.EventTuple(3221491980U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterRestoreEnvAlreadyRecoveredError = new ExEventLog.EventTuple(3221491981U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterRenameDbError = new ExEventLog.EventTuple(3221491982U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterRecoveryAfterRestore = new ExEventLog.EventTuple(1074008335U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterAdditionalRestoresPending = new ExEventLog.EventTuple(1074008336U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterNoDatabasesToRecover = new ExEventLog.EventTuple(1074008337U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterDbToRecover = new ExEventLog.EventTuple(1074008338U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterChkptNotDeleted = new ExEventLog.EventTuple(3221491987U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterLogsNotDeleted = new ExEventLog.EventTuple(3221491988U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReplayServiceVSSWriterRestoreEnvNotDeleted = new ExEventLog.EventTuple(3221491989U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringDatabaseAvailabilityCheckFailed = new ExEventLog.EventTuple(3221491990U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringDatabaseAvailabilityCheckPassed = new ExEventLog.EventTuple(1074008343U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringDatabaseOneDatacenterAvailableCopyCheckFailed = new ExEventLog.EventTuple(3221491992U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringDatabaseOneDatacenterAvailableCopyCheckSuccess = new ExEventLog.EventTuple(1074008345U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ServerLocatorServiceStarted = new ExEventLog.EventTuple(1074008346U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ServerLocatorServiceStopped = new ExEventLog.EventTuple(1074008347U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ServerLocatorServiceFailedToStart = new ExEventLog.EventTuple(3221491996U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringServerLocatorServiceCheckFailed = new ExEventLog.EventTuple(3221491997U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringServerLocatorServiceCheckPassed = new ExEventLog.EventTuple(1074008350U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ServerLocatorServiceCommunicationChannelFaulted = new ExEventLog.EventTuple(3221491999U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ServerLocatorServiceRestartScheduled = new ExEventLog.EventTuple(1074008352U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseDirectoryNotUnderMountPoint = new ExEventLog.EventTuple(3221492001U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiskHbStartingWatchdogTimer = new ExEventLog.EventTuple(1074008358U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiskHbStoppingWatchdogTimer = new ExEventLog.EventTuple(1074008359U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiskHbFailedToStopWatchdogTimer = new ExEventLog.EventTuple(3221492008U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiskHbMinimumTimeNotElaspedFromLastReboot = new ExEventLog.EventTuple(2147750185U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiskHbConfigChanged = new ExEventLog.EventTuple(2147750186U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiskHbTerminatingCurrentProcess = new ExEventLog.EventTuple(3221492011U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiskHbFailedToTerminateCurrentProcess = new ExEventLog.EventTuple(3221492012U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiskHbTriggeringImmediateBugcheck = new ExEventLog.EventTuple(3221492013U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ServerLocatorServiceStartTimeout = new ExEventLog.EventTuple(3221492014U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ProcessDiagnosticsTerminatingServiceNoDump = new ExEventLog.EventTuple(3221492015U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ServerLocatorServiceAnotherProcessUsingPort = new ExEventLog.EventTuple(3221492016U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ServerLocatorServiceServerForDatabaseNotFoundError = new ExEventLog.EventTuple(3221492017U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ServerLocatorServiceGetAllError = new ExEventLog.EventTuple(3221492018U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseSuspendedDueToLowSpace = new ExEventLog.EventTuple(3221492019U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ServerLocatorServiceGetServerObjectError = new ExEventLog.EventTuple(3221492020U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringMonitoringServiceCheckFailed = new ExEventLog.EventTuple(3221492021U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitoringMonitoringServiceCheckPassed = new ExEventLog.EventTuple(1074008374U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		private enum Category : short
		{
			Service = 1,
			Exchange_VSS_Writer,
			Move,
			Upgrade,
			Action,
			ExRes
		}

		internal enum Message : uint
		{
			ServiceStarted = 1074005969U,
			ServiceStopped,
			AllFacilitiesAreOnline,
			ServiceStarting,
			ServiceStopping,
			SourceInstanceStart = 1074005976U,
			SourceInstanceStop,
			TargetInstanceStart,
			TargetInstanceStop,
			InstanceBroken = 3221489628U,
			E00LogMoved = 1074005981U,
			CheckpointDeleted,
			VssInitFailed = 3221489633U,
			VSSWriterInitialize = 1074005986U,
			VSSWriterTerminate,
			VSSWriterMetadata = 1074005989U,
			VSSWriterMetadataError = 3221489638U,
			VSSWriterBackup = 1074005991U,
			VSSWriterBackupError = 3221489640U,
			VSSWriterSnapshot = 1074005993U,
			VSSWriterSnapshotError = 3221489642U,
			VSSWriterFreeze = 1074005995U,
			VSSWriterFreezeError = 3221489644U,
			VSSWriterThaw = 1074005997U,
			VSSWriterThawError = 3221489646U,
			VSSWriterAbort = 1074005999U,
			VSSWriterAbortError = 3221489648U,
			VSSWriterBackupComplete = 1074006001U,
			VSSWriterBackupCompleteError = 3221489650U,
			VSSWriterPostSnapshot = 1074006003U,
			VSSWriterOnBackupShutdownError = 3221489652U,
			VSSWriterOnBackupShutdown = 1074006005U,
			VSSWriterBackupCompleteFailureWarning = 2147747830U,
			VSSWriterPostSnapshotError = 3221489655U,
			VSSWriterAddComponentsError,
			VSSWriterAddDatabaseComponentError,
			VSSWriterDbFileInfoError,
			VSSWriterSetPrivateMetadataError,
			VSSWriterCheckInstanceVolumeDependenciesError,
			VSSWriterCheckDatabaseVolumeDependenciesError,
			VSSWriterBackupCompleteLogsTruncated = 1074006014U,
			VSSWriterBackupCompleteNoTruncateRequested,
			VSSWriterBackupCompleteWithFailure = 3221489664U,
			VSSWriterBackupCompleteUnknownGuid,
			VSSWriterBackupCompleteWithFailureAndUnknownGuid,
			ReplayServiceVSSWriterRestoreFailed,
			VSSSnapshotWriter = 1074006020U,
			OnlineDatabaseFailed = 3221489669U,
			FailedToCreateTempLogFile = 3221489671U,
			ReseedRequired,
			IncrementalReseedBlocked,
			IncrementalReseedError,
			LogFileGapFound,
			ConfigurationCheckerFailedTransient,
			AttemptCopyLastLogsFailed,
			NoDatabasesInReplica,
			InvalidFilePath = 3221489682U,
			LogFileCorruptError,
			LogInspectorFailed,
			CouldNotDeleteLogFile,
			FileCheckError,
			ReplayCheckError,
			FailedToCreateDirectory = 3221489689U,
			NoDirectory,
			FilesystemCorrupt,
			SavedStateError = 3221489693U,
			CouldNotCompareLogFiles = 3221489697U,
			CouldNotCreateNetworkShare,
			RestoreDatabaseCopySuccessful = 1074006053U,
			RestoreDatabaseCopySuccessfulPathsChanged,
			RestoreDatabaseCopyIncomplete,
			RestoreDatabaseCopyIncompletePathsChanged,
			RestoreDatabaseCopyFailed,
			AutoMountReportNoLoss,
			AutoMountReportMountWithDataLoss = 2147747883U,
			AutoMountReportMountNotAllowed = 3221489708U,
			AutoMountReportMountAfter = 2147747885U,
			AutoMountReportPublicFolderMountNotAllowed,
			CouldNotReplayLogFile = 3221489711U,
			ReseedCheckMissingLogfile,
			IsamException,
			ErrorCouldNotRecopyLogFile,
			DatabaseSubmitDumpsterMessages = 1074006067U,
			ReseederDeletedCheckpointFile,
			ConfigurationCheckerFailedADError = 3221489717U,
			ReseederDeletedTargetDatabaseFile = 1074006070U,
			ShipLogFailed = 3221489720U,
			ReseederDeletedExistingLogs = 1074006073U,
			DatabaseNotPresentAfterReplay = 3221489722U,
			SeederPerfCountersLoadFailure = 3221489725U,
			VSSWriterBackupDatabaseFullCopy = 1074006078U,
			VSSWriterBackupDatabaseIncrementalDifferential,
			VSSWriterBackupDatabaseError = 3221489728U,
			DatabaseDumpsterRedeliveryRequired = 1074006081U,
			ReplicaInstanceLogCopied,
			ReplicaInstanceLogsReplayed,
			ReplicaInstanceStartIncrementalReseed,
			ReplicaInstanceFinishIncrementalReseed,
			VSSWriterOrphanedBackupInstance = 2147747910U,
			VSSWriterMissingFile = 3221489735U,
			HTTPListenerFailedToStart,
			TcpListenerFailedToStart,
			ScrConfigPathConflict = 3221489741U,
			MultiplePathNext = 1074006094U,
			NetworkPathNext,
			LogCopyFailedDueToDuplicateName = 3221489744U,
			ScrConfigExceedLimit = 2147747921U,
			ScrConfigConflictWithDb,
			VSSReplicaBroken = 3221489747U,
			VSSReplicaSuspend,
			RpcServerStarted = 1074006101U,
			RpcServerStopped,
			RpcServerFailedToStart = 3221489751U,
			LogTruncationOpenFailed = 2147747928U,
			LogTruncationSourceFailure,
			LogTruncationLocalFailure,
			VSSWriterException = 3221489756U,
			RpcServerFailedToFindExchangeServersUsg,
			InstanceFailedToStart,
			InstanceFailedToDeleteRegistryStateWarning = 2147747935U,
			NoDirectoryHostInaccessible = 3221489761U,
			ExceptionDuringCallback,
			AlternateNetworkHadProblem,
			LogCopierFoundNoLogsOnSource = 3221489766U,
			LogCopierFailedDueToSource,
			LogCopierFailedDueToTarget,
			LogCopierFailedToCommunicate,
			TPRExchangeListenerStarted = 1074006122U,
			TPRManagerInitFailure = 3221489771U,
			LogCopierErrorOnSource,
			ReplicaInstanceMadeProgress = 1074006125U,
			LogCopierErrorOnSourceTriggerFailover = 3221489774U,
			LogCopierIsStalledDueToSource,
			CorruptLogRecoveryIsAttempted,
			CorruptLogRecoveryFailedToSuspend,
			CorruptLogRecoveryFailedToDismount,
			LogCopierReceivedSourceSideError,
			LogCopierBlockedByFullDisk,
			LogCorruptionTriggersFailover,
			LogCopierDisconnectedTooLong,
			CorruptLogDetectedOnActive,
			ErrorReadingLogOnActive,
			CorruptLogRepaired = 2147747961U,
			SlowIoDetected,
			CorruptLogRecoveryIsImmediatelyAttempted = 3221489787U,
			ResumeFailedDuringFailureItemProcessing,
			InspectorFixedCorruptLog = 2147747965U,
			LogCopierDetectsPossibleLogStreamReset,
			LogCopierFailedToTransitOutOfBlockMode,
			InspectorDetectedCorruptLog = 3221489792U,
			FatalIOErrorEncountered,
			PassiveMonitoredDBFailedToStart,
			ActiveMonitoredDBFailedToStart,
			VSSWriterMissingLogFileSignature,
			NetworkRoleChanged = 2147747992U,
			RegistryReplicatorException = 3221489817U,
			ClusterApiHungAlert = 2147748174U,
			IncrementalReseedInitException = 3221490760U,
			IncrementalReseedFailedError,
			IncrementalReseedRetryableError,
			IncrementalReseedPrereqError,
			IncSeedingStarted = 1074007116U,
			IncSeedingComplete,
			IncSeedingSourceReleased,
			AmChangingRole,
			AmStoreServiceStarted,
			AmInitiatingNodeFailover,
			AmDatabaseMountFailed = 3221490770U,
			AmDatabaseMounted = 1074007124U,
			AmDetectedNodeStateChange,
			AmStartingAutoMount,
			AmIgnoringDatabaseMount,
			AmDatabaseDismounted = 1074007129U,
			AmKnownError = 3221490778U,
			AmUnknownCrticalError,
			AmErrorReadingConfiguration,
			AmCriticalErrorReadingConfiguration,
			AmMoveNotApplicableForDatabase = 2147748958U,
			AmFailedToAutomountDatabase = 3221490756U,
			SuspendMarkedForDatabaseCopy = 1074007110U,
			ResumeMarkedForDatabaseCopy,
			MountAllowedWithMountDialOverride = 1074007135U,
			MountNotAllowedWithMountDialOverride = 3221490784U,
			AmDatabaseMoved = 1074007137U,
			AmDatabaseMoveFailed = 3221490786U,
			DbSeedingRequired = 2147748963U,
			DatabaseExistsInADButRegistryDeleted = 3221490788U,
			AmRpcServerStarted = 1074007141U,
			AmRpcServerStopped,
			AmRpcServerFailedToStart = 3221490791U,
			AmRpcServerFailedToFindExchangeServersUsg,
			ServiceFailedToStartAMFailure,
			AmDatabaseNotMountedServersDown,
			AmForceDismountingDatabases = 2147748971U,
			AmDatabaseAcllComplete = 1074007150U,
			AmDatabaseAcllFailed = 3221490799U,
			FailedMovePAM,
			SuccMovePAM = 1074007153U,
			AmIgnoredMapiNetFailureBecauseNodeNotUp = 2147748978U,
			AmKilledStoreToForceDismount = 3221490803U,
			AmFailedToStopService,
			AmFailedToStartService,
			AMDetectedMapiNetworkFailure,
			AmIgnoredMapiNetFailureBecauseMapiLooksUp = 2147748983U,
			AmIgnoredMapiNetFailureBecauseADIsWorking,
			AmIgnoredMapiNetFailureBecauseNotThePam,
			PauseSuccessful = 1074007970U,
			StopFailed = 3221491619U,
			StartFailed,
			PauseFailed = 3221491622U,
			CommandOK = 3221491621U,
			CommandFailed = 3221491623U,
			PowerEventOK,
			PowerEventFailed,
			SessionChangeFailed,
			ShutdownOK,
			ShutdownFailed,
			CommandSuccessful,
			ContinueSuccessful,
			ContinueFailed,
			FailedToUnloadAppDomain,
			PreShutdownOK = 1074007985U,
			PreShutdownFailed = 3221491634U,
			PreShutdownStart = 1074007987U,
			SeedManagerStarted,
			SeedManagerStopped,
			SeedInstancePrepareAdded,
			SeedInstancePrepareSucceeded,
			SeedInstancePrepareUnknownError = 3221491640U,
			SeedInstancePrepareFailed,
			SeedInstanceInProgressFailed,
			SeedInstanceCancelled = 2147749819U,
			SeedInstanceBeginSucceeded = 1074007996U,
			SeedInstanceBeginUnknownError = 3221491645U,
			SeedInstanceCancelRequestedByAdmin = 1074007998U,
			SeedInstanceCleanupRequestedByAdmin,
			SeedInstanceCleanupConfigChanged,
			SeedInstanceCleanupStale,
			SeedInstancesStoppedServiceShutdown = 2147749826U,
			SeedInstanceDeletedExistingLogs = 1074008003U,
			SeedInstanceDeletedCheckpointFile,
			SeedInstanceSuccess,
			MonitoringClusterServiceCheckFailed = 3221491654U,
			MonitoringClusterServiceCheckPassed = 1074008007U,
			MonitoringActiveManagerCheckFailed = 3221491656U,
			MonitoringActiveManagerCheckPassed = 1074008009U,
			MonitoringReplayServiceCheckFailed = 3221491658U,
			MonitoringReplayServiceCheckPassed = 1074008011U,
			MonitoringDagMembersUpCheckFailed = 3221491660U,
			MonitoringDagMembersUpCheckPassed = 1074008013U,
			MonitoringClusterNetworkCheckFailed = 3221491662U,
			MonitoringClusterNetworkCheckWarning = 2147749839U,
			MonitoringClusterNetworkCheckPassed = 1074008016U,
			MonitoringFileShareQuorumCheckFailed = 3221491665U,
			MonitoringFileShareQuorumCheckPassed = 1074008018U,
			MonitoringQuorumGroupCheckFailed = 3221491667U,
			MonitoringQuorumGroupCheckPassed = 1074008020U,
			MonitoringTasksRpcListenerCheckFailed = 3221491669U,
			MonitoringTasksRpcListenerCheckPassed = 1074008022U,
			MonitoringHttpListenerCheckFailed = 3221491671U,
			MonitoringHttpListenerCheckPassed = 1074008024U,
			LogReplayMapiException = 3221491673U,
			DatabaseOperationLockIsTakingLongTime = 2147749850U,
			CiSeedInstanceSuccess = 1074008027U,
			SourceReplicaInstanceNotStarted = 3221491676U,
			TargetReplicaInstanceNotStarted,
			SubmitDumpsterMessagesFailed = 3221491681U,
			ClusterDatabaseWriteFailed,
			IncSeedingTerminated = 2147749859U,
			AmRoleMonitoringError = 3221491684U,
			AutoMountReportDbInUseOnSource = 2147749861U,
			AutoMountReportDbInUseAcllInProgress,
			AmDatabaseDismountFailed = 3221491687U,
			AmForceDismountMasterMismatch = 2147749864U,
			AmDatabaseMountFailedGeneric = 3221491689U,
			AmDatabaseDismountFailedGeneric,
			AmDatabaseMoveFailedGeneric,
			NetworkReplicationDisabled,
			MonitoringTcpListenerCheckFailed = 3221491696U,
			MonitoringTcpListenerCheckPassed = 1074008049U,
			NetworkMonitoringError = 3221491698U,
			SeedInstanceAnotherError,
			ForceNewLogError,
			ReadOnePageError,
			ReadPageSizeError,
			AmDatabaseMoveUnspecifiedServerFailed,
			VSSReplicaCopyUnhealthy,
			IncrementalReseedSourceDatabaseDismounted,
			IncrementalReseedSourceDatabaseMountRpcError,
			DumpsterRedeliveryFailed,
			NodeNotInCluster,
			ReplayRpcServerFailedToRegister,
			AmRpcServerFailedToRegister,
			ServiceFailedToStartComponentFailure,
			FailedToStartRetriableComponent,
			ConfigUpdaterScanFailed,
			ConfigUpdaterFailedToFindConfig,
			MonitoringTPRListenerCheckFailed,
			MonitoringTPRListenerCheckPassed = 1074008068U,
			EsebcliTooManyApplications = 3221491717U,
			SeedingSourceBegin = 1074008070U,
			SeedingSourceEnd,
			SeedingSourceCancel = 2147749896U,
			CheckConnectionToStoreFailed = 3221491721U,
			CheckDatabaseHeaderFailed,
			PossibleSplitBrainDetected,
			FailedToCleanUpSingleIncReseedFile,
			FailedToCleanUpFile,
			LogReplaySuspendedDueToCopyQ = 2147749902U,
			LogReplayResumedDueToCopyQ = 1074008079U,
			SyncSuspendResumeOperationFailed = 3221491728U,
			MonitoringDatabaseRedundancyCheckFailed,
			MonitoringDatabaseRedundancyCheckPassed = 1074008082U,
			DatabaseRedistributionReport,
			FqdnResolutionFailure = 3221491732U,
			LogReplayPatchFailedIsamException,
			RedirtyDatabaseCreateTempLog,
			LogReplayPatchFailedPrepareException,
			FailedToPublishFailureItem,
			PeriodicOperationFailedRetrievingStatuses = 2147749913U,
			ProcessDiagnosticsTerminatingService = 3221491738U,
			GetBootTimeWithWmiFailure,
			LogRepairFailedDueToRetryLimit,
			LogRepairSuccess = 2147749917U,
			ConfigurationCheckerFailedGeneric = 3221491742U,
			LogFileCorruptOrGapFoundOutsideRequiredRange,
			MovingFilesToRestartLogStream = 2147749920U,
			DeletedSkippedLogsDirectory,
			MissingFailureItemDetected = 3221491746U,
			LogReplayPatchFailedOnLaggedCopy,
			MonitoringDatabaseOneDatacenterCheckFailed = 3221491749U,
			ActiveSeedingSourceBegin = 1074008102U,
			ActiveSeedingSourceEnd,
			ActiveSeedingSourceCancel = 2147749928U,
			LogRepairNotPossible = 3221491753U,
			MonitoringDatabaseRedundancyServerCheckFailed,
			MonitoringDatabaseRedundancyServerCheckPassed = 1074008107U,
			DetermineWorkerProcessIdFailed = 3221491756U,
			DagNetworkConfigOld = 1074008268U,
			DagNetworkConfigNew,
			NotifyActiveSendFailed = 3221491927U,
			AmCheckRemoteSiteLocalServerSiteNull = 2147750104U,
			AmCheckRemoteSiteNotFound = 1074008281U,
			AmCheckRemoteSiteAlert = 3221491930U,
			AmCheckRemoteSiteDismount,
			AmCheckRemoteSiteSucceeded = 1074008284U,
			AmCheckRemoteSiteDisabled = 2147750109U,
			DiskHbFailedToStartWatchdogTimer = 3221491934U,
			DiskHbFailedToPrepareHeartbeatFile,
			DiskHbEncounteredUnhandledException,
			DiskHbFailedToDetermineBytesPerSector,
			DiskHbIoWriteTestFailed,
			DiskHbIoReadTestFailed,
			DiskHbIoLatencyExceeded = 2147750116U,
			DiskHbFailedToStart = 3221491941U,
			DiskHbFailedToStop,
			AmTimeStampEntryMissingInOneOrMoreServers = 1074008295U,
			AutoReseedInstanceStarted,
			AutoReseedPrereqFailed = 3221491945U,
			AutoReseedNoSpareDisk,
			AutoReseedFailed,
			AutoReseedSuccessful = 1074008300U,
			AutoReseedSpareDisksReleased,
			MonitoringDatabaseOneDatacenterCheckSuccess,
			SeedInstanceStartedSetBroken = 3221491951U,
			LogReplayGenericError,
			ReplayServiceVSSWriterMissingVersionStampError,
			ReplayServiceVSSWriterBadVersionStampError,
			ReplayServiceVSSWriterRestoreOptionsString = 1074008307U,
			ReplayServiceVSSWriterDbGuidMappingMismatchError = 3221491956U,
			ReplayServiceVSSWriterMultipleRetargettingError,
			ReplayServiceVSSWriterOriginalLogfilePathMismatchError,
			ReplayServiceVSSWriterOriginalSystemPathMismatchError,
			ReplayServiceVSSWriterDbRetargetMismatchError,
			ReplayServiceVSSWriterTargetSGLookupError,
			ReplayServiceVSSWriterTargetSGOnline,
			ReplayServiceVSSWriterRestoreToOriginalSG = 1074008315U,
			ReplayServiceVSSWriterRestoreToAlternateSG,
			ReplayServiceVSSWriterTargetLogfilePathMismatchError = 3221491965U,
			ReplayServiceVSSWriterTargetSystemPathMismatchError,
			ReplayServiceVSSWriterTargetLogfileBaseNameMismatchError,
			ReplayServiceVSSWriterSGRestoreInProgressError,
			ReplayServiceVSSWriterCircularLogDBRestore,
			ReplayServiceVSSWriterTargetDbMismatchError,
			ReplayServiceVSSWriterCannotOverwriteError,
			ReplayServiceVSSWriterFindLogFilesError,
			ReplayServiceVSSWriterLocationRestoreInProgressError,
			ReplayServiceVSSWriterTargetLogfilePathInUseError,
			ReplayServiceVSSWriterRestoreEnvSGMismatchError,
			ReplayServiceVSSWriterRestoreEnvLogfilePathMismatchError,
			ReplayServiceVSSWriterRestoreEnvLogfileBaseNameMismatchError,
			ReplayServiceVSSWriterRestoreEnvLogfileSignatureMismatchError,
			ReplayServiceVSSWriterRestoreEnvSystemPathMismatchError,
			ReplayServiceVSSWriterRestoreEnvCircularLogEnabledError,
			ReplayServiceVSSWriterRestoreEnvAlreadyRecoveredError,
			ReplayServiceVSSWriterRenameDbError,
			ReplayServiceVSSWriterRecoveryAfterRestore = 1074008335U,
			ReplayServiceVSSWriterAdditionalRestoresPending,
			ReplayServiceVSSWriterNoDatabasesToRecover,
			ReplayServiceVSSWriterDbToRecover,
			ReplayServiceVSSWriterChkptNotDeleted = 3221491987U,
			ReplayServiceVSSWriterLogsNotDeleted,
			ReplayServiceVSSWriterRestoreEnvNotDeleted,
			MonitoringDatabaseAvailabilityCheckFailed,
			MonitoringDatabaseAvailabilityCheckPassed = 1074008343U,
			MonitoringDatabaseOneDatacenterAvailableCopyCheckFailed = 3221491992U,
			MonitoringDatabaseOneDatacenterAvailableCopyCheckSuccess = 1074008345U,
			ServerLocatorServiceStarted,
			ServerLocatorServiceStopped,
			ServerLocatorServiceFailedToStart = 3221491996U,
			MonitoringServerLocatorServiceCheckFailed,
			MonitoringServerLocatorServiceCheckPassed = 1074008350U,
			ServerLocatorServiceCommunicationChannelFaulted = 3221491999U,
			ServerLocatorServiceRestartScheduled = 1074008352U,
			DatabaseDirectoryNotUnderMountPoint = 3221492001U,
			DiskHbStartingWatchdogTimer = 1074008358U,
			DiskHbStoppingWatchdogTimer,
			DiskHbFailedToStopWatchdogTimer = 3221492008U,
			DiskHbMinimumTimeNotElaspedFromLastReboot = 2147750185U,
			DiskHbConfigChanged,
			DiskHbTerminatingCurrentProcess = 3221492011U,
			DiskHbFailedToTerminateCurrentProcess,
			DiskHbTriggeringImmediateBugcheck,
			ServerLocatorServiceStartTimeout,
			ProcessDiagnosticsTerminatingServiceNoDump,
			ServerLocatorServiceAnotherProcessUsingPort,
			ServerLocatorServiceServerForDatabaseNotFoundError,
			ServerLocatorServiceGetAllError,
			DatabaseSuspendedDueToLowSpace,
			ServerLocatorServiceGetServerObjectError,
			MonitoringMonitoringServiceCheckFailed,
			MonitoringMonitoringServiceCheckPassed = 1074008374U
		}
	}
}
