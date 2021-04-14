using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public static class MSExchangeISEventLogConstants
	{
		public const string EventSource = "MSExchangeIS";

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InternalLogicError = new ExEventLog.EventTuple(3221488617U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnhandledException = new ExEventLog.EventTuple(3221488618U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LeakedException = new ExEventLog.EventTuple(3221488619U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_DuplicateRpcEndpoint = new ExEventLog.EventTuple(3221488620U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_DuplicateRpcMTEndpoint = new ExEventLog.EventTuple(3221488621U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_DuplicatePoolRpcEndpoint = new ExEventLog.EventTuple(3221488622U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_DuplicateAdminRpcEndpoint = new ExEventLog.EventTuple(3221488623U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxRetention = new ExEventLog.EventTuple(1074004976U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxPurged = new ExEventLog.EventTuple(1074004977U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxReconnected = new ExEventLog.EventTuple(1074004978U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseBadVersion = new ExEventLog.EventTuple(3221488627U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FullTextIndexException = new ExEventLog.EventTuple(3221488628U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxPrequarantined = new ExEventLog.EventTuple(3221488629U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxQuarantined = new ExEventLog.EventTuple(2147746806U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxUnquarantined = new ExEventLog.EventTuple(1074004983U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NamedPropertyMappingChange = new ExEventLog.EventTuple(3221488632U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RepidGuidMappingChange = new ExEventLog.EventTuple(3221488633U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceProcessTerminated = new ExEventLog.EventTuple(3221488634U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_StartupFailureDueToADError = new ExEventLog.EventTuple(3221488635U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WorkerProcessStarted = new ExEventLog.EventTuple(1074004988U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WorkerProcessStopped = new ExEventLog.EventTuple(1074004989U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WorkerProcessFailedToStart = new ExEventLog.EventTuple(3221488638U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceInitializationFailure = new ExEventLog.EventTuple(3221488639U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxLocaleIdChanged = new ExEventLog.EventTuple(1074004992U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxGlobcntRolledOver = new ExEventLog.EventTuple(1074004993U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxReplidExhaustionApproaching = new ExEventLog.EventTuple(2147746818U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxReplidExhaustion = new ExEventLog.EventTuple(3221488643U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TraceLoggerStarted = new ExEventLog.EventTuple(1074004996U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TraceLoggerStopped = new ExEventLog.EventTuple(1074004997U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TraceLoggerFailed = new ExEventLog.EventTuple(2147746822U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WorkerProcessStopTimeout = new ExEventLog.EventTuple(3221488647U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_StartTraceSessionFailed = new ExEventLog.EventTuple(3221488648U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TaskRequestFailed = new ExEventLog.EventTuple(1074005001U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TaskNonFatalDBException = new ExEventLog.EventTuple(3221488650U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TaskFatalDBException = new ExEventLog.EventTuple(3221488651U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseVersionTooOld = new ExEventLog.EventTuple(3221488652U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseVersionTooNew = new ExEventLog.EventTuple(3221488653U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseBadRequestedUpdgradeVersion = new ExEventLog.EventTuple(3221488654U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SchemaUpgradeFailure = new ExEventLog.EventTuple(3221488655U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SchemaUpgradeCommencing = new ExEventLog.EventTuple(1074005008U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SchemaUpgradeComplete = new ExEventLog.EventTuple(1074005009U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SchemaNoUpgradeFromCurrentVersionFailure = new ExEventLog.EventTuple(3221488658U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SchemaVersionPublicationComplete = new ExEventLog.EventTuple(1074005011U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SchemaVersionPublicationFailure = new ExEventLog.EventTuple(2147746836U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidAppConfig = new ExEventLog.EventTuple(3221488661U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CriticalBlockFailure = new ExEventLog.EventTuple(3221488662U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseGuidPatchRequired = new ExEventLog.EventTuple(1074005015U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseFileRestoreNotAllowed = new ExEventLog.EventTuple(3221488664U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TraceMaintenanceFailed = new ExEventLog.EventTuple(2147746841U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidMailboxGlobcntAllocation = new ExEventLog.EventTuple(3221488666U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MountedStoreNotInActiveDirectory = new ExEventLog.EventTuple(3221488667U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_TraceMessageFailed = new ExEventLog.EventTuple(3221488668U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_CannotDetectEnvironment = new ExEventLog.EventTuple(3221488669U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SchemaUpgradeQuarantined = new ExEventLog.EventTuple(1074005022U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SQLExceptionDetected = new ExEventLog.EventTuple(3221489617U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IOExceptionDetected = new ExEventLog.EventTuple(3221489618U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FileDeleted = new ExEventLog.EventTuple(2147747795U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DirectoryDeleted = new ExEventLog.EventTuple(2147747796U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SQLExtremeLatency = new ExEventLog.EventTuple(3221489621U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_JetExceptionDetected = new ExEventLog.EventTuple(3221489622U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TableOperatorCommitTransaction = new ExEventLog.EventTuple(2147485655U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_JetFatalDatabaseException = new ExEventLog.EventTuple(3221489624U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidSubmitTime = new ExEventLog.EventTuple(2147749793U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PropertyPromotion = new ExEventLog.EventTuple(1074007970U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IntegrityCheckStart = new ExEventLog.EventTuple(1074007971U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IntegrityCheckEnd = new ExEventLog.EventTuple(1074007972U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IntegrityCheckFailed = new ExEventLog.EventTuple(3221491621U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IntegrityCheckMailboxStart = new ExEventLog.EventTuple(1074007974U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IntegrityCheckCorruptionDetected = new ExEventLog.EventTuple(2147749799U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IntegrityCheckMailboxEnd = new ExEventLog.EventTuple(1074007976U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IntegrityCheckMailboxFailed = new ExEventLog.EventTuple(3221491625U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PossibleDuplicateMID = new ExEventLog.EventTuple(3221229482U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CorruptSearchBacklink = new ExEventLog.EventTuple(3221229483U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseMaintenancePreemptedByDbEngineBusy = new ExEventLog.EventTuple(2147487660U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxMaintenancePreemptedByDbEngineBusy = new ExEventLog.EventTuple(2147487661U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_IntegrityCheckConfigurationSkippedEntry = new ExEventLog.EventTuple(3221491630U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IntegrityCheckScheduledNewCorruption = new ExEventLog.EventTuple(2147749807U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CorruptDirectoryObjectDetected = new ExEventLog.EventTuple(3221492617U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnsupportedRecipientTypeDetected = new ExEventLog.EventTuple(3221492618U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PermanentADError = new ExEventLog.EventTuple(3221492619U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TransientADError = new ExEventLog.EventTuple(3221492620U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxStorageOverWarningLimit = new ExEventLog.EventTuple(2147746869U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxStorageOverQuotaLimit = new ExEventLog.EventTuple(2147746870U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxStorageShutoff = new ExEventLog.EventTuple(2147754320U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxOverDumpsterQuota = new ExEventLog.EventTuple(2147755815U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxOverDumpsterWarningQuota = new ExEventLog.EventTuple(2147755816U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ArchiveStorageOverWarningLimit = new ExEventLog.EventTuple(2147755817U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ArchiveStorageShutoff = new ExEventLog.EventTuple(2147754330U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ArchiveOverDumpsterQuota = new ExEventLog.EventTuple(3221497642U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ArchiveOverDumpsterWarningQuota = new ExEventLog.EventTuple(3221497643U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToSendQuotaWarning = new ExEventLog.EventTuple(3221497644U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MaxObjectsExceeded = new ExEventLog.EventTuple(3221497262U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MaintenanceProcessorIsIdle = new ExEventLog.EventTuple(2147755821U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FolderStorageOverWarningLimit = new ExEventLog.EventTuple(2147755823U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FolderStorageShutoff = new ExEventLog.EventTuple(2147755824U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToSendPublicFolderQuotaWarning = new ExEventLog.EventTuple(3221497649U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToSendPerFolderQuotaWarning = new ExEventLog.EventTuple(3221497650U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NamedPropsQuotaError = new ExEventLog.EventTuple(3221497652U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxMessagesPerFolderCountWarningQuota = new ExEventLog.EventTuple(2147755829U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxMessagesPerFolderCountReceiveQuota = new ExEventLog.EventTuple(2147755830U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DumpsterMessagesPerFolderCountWarningQuota = new ExEventLog.EventTuple(2147755831U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DumpsterMessagesPerFolderCountReceiveQuota = new ExEventLog.EventTuple(2147755832U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FolderHierarchyChildrenCountWarningQuota = new ExEventLog.EventTuple(2147755833U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FolderHierarchyChildrenCountReceiveQuota = new ExEventLog.EventTuple(2147755834U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FolderHierarchyDepthWarningQuota = new ExEventLog.EventTuple(2147755835U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FolderHierarchyDepthReceiveQuota = new ExEventLog.EventTuple(2147755836U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FoldersCountWarningQuota = new ExEventLog.EventTuple(2147755837U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FoldersCountReceiveQuota = new ExEventLog.EventTuple(2147755838U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LastLogUpdateFailed = new ExEventLog.EventTuple(3221527616U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LastLogWriterHung = new ExEventLog.EventTuple(3221527617U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TooManyRecoveryMDBsMounted = new ExEventLog.EventTuple(3221265474U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TooManyMDBsMounted = new ExEventLog.EventTuple(3221265475U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TooManyActiveMDBsMounted = new ExEventLog.EventTuple(3221265476U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TooManyDBsConfigured = new ExEventLog.EventTuple(3221265477U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NoDatabase = new ExEventLog.EventTuple(3221265478U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SetColumnsHasNonPromotedProperties = new ExEventLog.EventTuple(2147523655U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MountCompleted = new ExEventLog.EventTuple(1073781832U, 7, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DismountCompleted = new ExEventLog.EventTuple(1073781833U, 7, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ApproachingMaxDbSize = new ExEventLog.EventTuple(2147785802U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MaxDbSizeExceededDismountForced = new ExEventLog.EventTuple(3221527627U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseLogicalCorruption = new ExEventLog.EventTuple(3221527628U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UrgentTombstoneCleanupDispatched = new ExEventLog.EventTuple(2147785805U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LastLogUpdateTooAdvanced = new ExEventLog.EventTuple(2147785806U, 7, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LastLogUpdateLaggingBehind = new ExEventLog.EventTuple(2147785807U, 7, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NoActiveDatabase = new ExEventLog.EventTuple(3221265488U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UrgentTombstoneCleanupSummary = new ExEventLog.EventTuple(2147785809U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WorkerMountCompleted = new ExEventLog.EventTuple(1073781842U, 7, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PassiveDatabaseAttachedReadOnly = new ExEventLog.EventTuple(1073781843U, 7, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PassiveDatabaseDetached = new ExEventLog.EventTuple(1073781844U, 7, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PassiveDatabaseAttachDetachException = new ExEventLog.EventTuple(3221265493U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedFinishLogReplayForTransitionToActive = new ExEventLog.EventTuple(3221265494U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RunningWithRepairedDatabase = new ExEventLog.EventTuple(3221265497U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		private enum Category : short
		{
			General = 1,
			Physical_Access,
			Lazy_Indexing,
			Logical_Data_Model,
			Directory_Services,
			MAPI,
			High_Availability
		}

		internal enum Message : uint
		{
			InternalLogicError = 3221488617U,
			UnhandledException,
			LeakedException,
			DuplicateRpcEndpoint,
			DuplicateRpcMTEndpoint,
			DuplicatePoolRpcEndpoint,
			DuplicateAdminRpcEndpoint,
			MailboxRetention = 1074004976U,
			MailboxPurged,
			MailboxReconnected,
			DatabaseBadVersion = 3221488627U,
			FullTextIndexException,
			MailboxPrequarantined,
			MailboxQuarantined = 2147746806U,
			MailboxUnquarantined = 1074004983U,
			NamedPropertyMappingChange = 3221488632U,
			RepidGuidMappingChange,
			ServiceProcessTerminated,
			StartupFailureDueToADError,
			WorkerProcessStarted = 1074004988U,
			WorkerProcessStopped,
			WorkerProcessFailedToStart = 3221488638U,
			ServiceInitializationFailure,
			MailboxLocaleIdChanged = 1074004992U,
			MailboxGlobcntRolledOver,
			MailboxReplidExhaustionApproaching = 2147746818U,
			MailboxReplidExhaustion = 3221488643U,
			TraceLoggerStarted = 1074004996U,
			TraceLoggerStopped,
			TraceLoggerFailed = 2147746822U,
			WorkerProcessStopTimeout = 3221488647U,
			StartTraceSessionFailed,
			TaskRequestFailed = 1074005001U,
			TaskNonFatalDBException = 3221488650U,
			TaskFatalDBException,
			DatabaseVersionTooOld,
			DatabaseVersionTooNew,
			DatabaseBadRequestedUpdgradeVersion,
			SchemaUpgradeFailure,
			SchemaUpgradeCommencing = 1074005008U,
			SchemaUpgradeComplete,
			SchemaNoUpgradeFromCurrentVersionFailure = 3221488658U,
			SchemaVersionPublicationComplete = 1074005011U,
			SchemaVersionPublicationFailure = 2147746836U,
			InvalidAppConfig = 3221488661U,
			CriticalBlockFailure,
			DatabaseGuidPatchRequired = 1074005015U,
			DatabaseFileRestoreNotAllowed = 3221488664U,
			TraceMaintenanceFailed = 2147746841U,
			InvalidMailboxGlobcntAllocation = 3221488666U,
			MountedStoreNotInActiveDirectory,
			TraceMessageFailed,
			CannotDetectEnvironment,
			SchemaUpgradeQuarantined = 1074005022U,
			SQLExceptionDetected = 3221489617U,
			IOExceptionDetected,
			FileDeleted = 2147747795U,
			DirectoryDeleted,
			SQLExtremeLatency = 3221489621U,
			JetExceptionDetected,
			TableOperatorCommitTransaction = 2147485655U,
			JetFatalDatabaseException = 3221489624U,
			InvalidSubmitTime = 2147749793U,
			PropertyPromotion = 1074007970U,
			IntegrityCheckStart,
			IntegrityCheckEnd,
			IntegrityCheckFailed = 3221491621U,
			IntegrityCheckMailboxStart = 1074007974U,
			IntegrityCheckCorruptionDetected = 2147749799U,
			IntegrityCheckMailboxEnd = 1074007976U,
			IntegrityCheckMailboxFailed = 3221491625U,
			PossibleDuplicateMID = 3221229482U,
			CorruptSearchBacklink,
			DatabaseMaintenancePreemptedByDbEngineBusy = 2147487660U,
			MailboxMaintenancePreemptedByDbEngineBusy,
			IntegrityCheckConfigurationSkippedEntry = 3221491630U,
			IntegrityCheckScheduledNewCorruption = 2147749807U,
			CorruptDirectoryObjectDetected = 3221492617U,
			UnsupportedRecipientTypeDetected,
			PermanentADError,
			TransientADError,
			MailboxStorageOverWarningLimit = 2147746869U,
			MailboxStorageOverQuotaLimit,
			MailboxStorageShutoff = 2147754320U,
			MailboxOverDumpsterQuota = 2147755815U,
			MailboxOverDumpsterWarningQuota,
			ArchiveStorageOverWarningLimit,
			ArchiveStorageShutoff = 2147754330U,
			ArchiveOverDumpsterQuota = 3221497642U,
			ArchiveOverDumpsterWarningQuota,
			FailedToSendQuotaWarning,
			MaxObjectsExceeded = 3221497262U,
			MaintenanceProcessorIsIdle = 2147755821U,
			FolderStorageOverWarningLimit = 2147755823U,
			FolderStorageShutoff,
			FailedToSendPublicFolderQuotaWarning = 3221497649U,
			FailedToSendPerFolderQuotaWarning,
			NamedPropsQuotaError = 3221497652U,
			MailboxMessagesPerFolderCountWarningQuota = 2147755829U,
			MailboxMessagesPerFolderCountReceiveQuota,
			DumpsterMessagesPerFolderCountWarningQuota,
			DumpsterMessagesPerFolderCountReceiveQuota,
			FolderHierarchyChildrenCountWarningQuota,
			FolderHierarchyChildrenCountReceiveQuota,
			FolderHierarchyDepthWarningQuota,
			FolderHierarchyDepthReceiveQuota,
			FoldersCountWarningQuota,
			FoldersCountReceiveQuota,
			LastLogUpdateFailed = 3221527616U,
			LastLogWriterHung,
			TooManyRecoveryMDBsMounted = 3221265474U,
			TooManyMDBsMounted,
			TooManyActiveMDBsMounted,
			TooManyDBsConfigured,
			NoDatabase,
			SetColumnsHasNonPromotedProperties = 2147523655U,
			MountCompleted = 1073781832U,
			DismountCompleted,
			ApproachingMaxDbSize = 2147785802U,
			MaxDbSizeExceededDismountForced = 3221527627U,
			DatabaseLogicalCorruption,
			UrgentTombstoneCleanupDispatched = 2147785805U,
			LastLogUpdateTooAdvanced,
			LastLogUpdateLaggingBehind,
			NoActiveDatabase = 3221265488U,
			UrgentTombstoneCleanupSummary = 2147785809U,
			WorkerMountCompleted = 1073781842U,
			PassiveDatabaseAttachedReadOnly,
			PassiveDatabaseDetached,
			PassiveDatabaseAttachDetachException = 3221265493U,
			FailedFinishLogReplayForTransitionToActive,
			RunningWithRepairedDatabase = 3221265497U
		}
	}
}
