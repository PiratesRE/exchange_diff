using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Diagnostics.Service.Common
{
	public static class MSExchangeDiagnosticsEventLogConstants
	{
		public const string EventSource = "MSExchangeDiagnostics";

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigurationExceptionOnStartup = new ExEventLog.EventTuple(3221488617U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DirectoryCreationException = new ExEventLog.EventTuple(3221488618U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PerformanceCounterWarningTriggerEvent = new ExEventLog.EventTuple(2147746797U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PerformanceCounterErrorTriggerEvent = new ExEventLog.EventTuple(3221488622U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PerformanceCounterInformationTriggerEvent = new ExEventLog.EventTuple(1074005021U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PerformanceLogError = new ExEventLog.EventTuple(3221488623U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceStarting = new ExEventLog.EventTuple(1074004976U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceStarted = new ExEventLog.EventTuple(1074004992U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceStopping = new ExEventLog.EventTuple(1074004977U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceStopped = new ExEventLog.EventTuple(1074004993U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RetentionAgentUnhandledException = new ExEventLog.EventTuple(3221488627U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RetentionAgentDataLossOccurred = new ExEventLog.EventTuple(3221488628U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RetentionAgentPotentialDataLossWarning = new ExEventLog.EventTuple(2147746805U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SqlOutputStreamUnhandledException = new ExEventLog.EventTuple(3221488630U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ActiveDirectoryUnavailable = new ExEventLog.EventTuple(3221488631U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SqlOutputStreamConnectionStringFromAdNotFound = new ExEventLog.EventTuple(3221488632U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SqlOutputStreamConsecutiveRetriesForASpecifiedTime = new ExEventLog.EventTuple(3221488633U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SqlOutputStreamDecryptFailed = new ExEventLog.EventTuple(3221488634U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_BadIisLogHeader = new ExEventLog.EventTuple(3221488636U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_IisLogLineNotProcessed = new ExEventLog.EventTuple(3221488637U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OwaServiceUnavailable = new ExEventLog.EventTuple(3221488638U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RelogManagerUnhandledException = new ExEventLog.EventTuple(3221488639U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OwaHttpStatus400 = new ExEventLog.EventTuple(3221488642U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OwaHttpStatus440 = new ExEventLog.EventTuple(3221488643U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OwaHttpStatus500 = new ExEventLog.EventTuple(3221488644U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OwaHttpStatusOther = new ExEventLog.EventTuple(3221488645U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_JobManagerStarted = new ExEventLog.EventTuple(1074004998U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_JobManagerNotStarted = new ExEventLog.EventTuple(1074004999U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_JobManagerStartupFailures = new ExEventLog.EventTuple(3221488648U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_JobCrashed = new ExEventLog.EventTuple(3221488649U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_JobPoisoned = new ExEventLog.EventTuple(3221488650U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_BingServicesLatencyAboveThreshold = new ExEventLog.EventTuple(3221488651U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_BingServicesLatencyBelowThreshold = new ExEventLog.EventTuple(1074005004U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FindPlacesFailureAboveThreshold = new ExEventLog.EventTuple(3221488653U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FindPlacesFailureBelowThreshold = new ExEventLog.EventTuple(1074005006U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_BitlockerStateDetectionError = new ExEventLog.EventTuple(3221488655U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_BitlockerState = new ExEventLog.EventTuple(1074005008U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ConnectionStringManagerPartitionInvalid = new ExEventLog.EventTuple(3221488657U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ConnectionStringManagerUnableToConnect = new ExEventLog.EventTuple(3221488658U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AnalyzerNotAdded = new ExEventLog.EventTuple(1074005011U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WatsonCrashAlert = new ExEventLog.EventTuple(3221488660U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WatsonExceptionCrashAlert = new ExEventLog.EventTuple(3221488670U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OwaLyncFailureAboveThreshold = new ExEventLog.EventTuple(3221488664U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OwaLyncFailureBelowThreshold = new ExEventLog.EventTuple(1074005017U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OwaTooManyExceptionsEncountered = new ExEventLog.EventTuple(3221488717U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TooManyObjectsOpenedException = new ExEventLog.EventTuple(3221488718U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OwaLogonFailures = new ExEventLog.EventTuple(3221488719U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OwaHighLatencyLoadStartPage = new ExEventLog.EventTuple(3221488720U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OwaTooManyHttpErrorResponsesEncountered = new ExEventLog.EventTuple(3221488721U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OwaTooManyHttpErrorResponsesResolved = new ExEventLog.EventTuple(1074005074U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OabTooManyHttpErrorResponsesEncountered = new ExEventLog.EventTuple(3221489167U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OabTooManyHttpErrorResponsesResolved = new ExEventLog.EventTuple(1074005520U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OabTooManyExceptionsEncountered = new ExEventLog.EventTuple(3221489169U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OabFileLoadException = new ExEventLog.EventTuple(3221489170U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_IisAppPoolHttpErrorsDefaultEvent = new ExEventLog.EventTuple(3221488723U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IisAppPoolTooManyHttpErrorResponsesEncounteredDefaultEvent = new ExEventLog.EventTuple(3221488724U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IisAppPoolTooManyHttpErrorResponsesResolvedDefaultEvent = new ExEventLog.EventTuple(1074005077U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OwaStartPageFailures = new ExEventLog.EventTuple(3221488726U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OwaStartPageFailuresResolved = new ExEventLog.EventTuple(1074005079U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OwaActiveDatabaseAvailabilityBelowThreshold = new ExEventLog.EventTuple(3221488728U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OwaActiveDatabaseAvailabilityBelowThresholdResolved = new ExEventLog.EventTuple(1074005081U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OwaLogoffFailures = new ExEventLog.EventTuple(3221488730U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OwaLogoffFailuresResolved = new ExEventLog.EventTuple(1074005083U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TestEvent = new ExEventLog.EventTuple(1074003968U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RbaRequestsWithExceptionsReachedThreshold = new ExEventLog.EventTuple(3221488661U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RbaAtLeastOneExceptionReachedThreshold = new ExEventLog.EventTuple(3221488662U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RbaAllIsWell = new ExEventLog.EventTuple(1074005015U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CalSyncRequestsWithExceptionsReachedThreshold = new ExEventLog.EventTuple(3221488666U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CalSyncRequestsWithSyncFailuresReachedThreshold = new ExEventLog.EventTuple(3221488667U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CalSyncAllIsWell = new ExEventLog.EventTuple(1074005020U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AvailabilityServiceExceptionAboveThreshold = new ExEventLog.EventTuple(3221489272U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TxSyncPopRequestsWithExceptionsReachedThreshold = new ExEventLog.EventTuple(3221488676U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TxSyncIMAPRequestsWithExceptionsReachedThreshold = new ExEventLog.EventTuple(3221488677U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TxSyncDeltaSyncMailRequestsWithExceptionsReachedThreshold = new ExEventLog.EventTuple(3221488678U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TxSyncFacebookRequestsWithExceptionsReachedThreshold = new ExEventLog.EventTuple(3221488679U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TxSyncLinkedInRequestsWithExceptionsReachedThreshold = new ExEventLog.EventTuple(3221488680U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TxSyncPopIsWell = new ExEventLog.EventTuple(1074005033U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TxSyncIMAPIsWell = new ExEventLog.EventTuple(1074005034U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TxSyncDeltaSyncMailIsWell = new ExEventLog.EventTuple(1074005035U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TxSyncFacebookIsWell = new ExEventLog.EventTuple(1074005036U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TxSyncLinkedInIsWell = new ExEventLog.EventTuple(1074005037U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PowerShellTimeoutAuthzRequestAboveThreshold = new ExEventLog.EventTuple(3221489717U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PowerShellLiveIDTimeoutAuthzRequestAboveThreshold = new ExEventLog.EventTuple(3221489718U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PowerShellLegacyTimeoutAuthzRequestAboveThreshold = new ExEventLog.EventTuple(3221489719U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PowerShellUnhandledCmdletExceptionAboveThreshold = new ExEventLog.EventTuple(3221489720U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PowerShellLiveIDUnhandledCmdletExceptionAboveThreshold = new ExEventLog.EventTuple(3221489721U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PowerShellLegacyUnhandledCmdletExceptionAboveThreshold = new ExEventLog.EventTuple(3221489722U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PowerShellLongLatencyCmdletAboveThreshold = new ExEventLog.EventTuple(3221489723U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PowerShellLiveIDLongLatencyCmdletAboveThreshold = new ExEventLog.EventTuple(3221489724U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PowerShellLegacyLongLatencyCmdletAboveThreshold = new ExEventLog.EventTuple(3221489725U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PowerShellAuthzErrorAboveThreshold = new ExEventLog.EventTuple(3221489726U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PowerShellLiveIDAuthzErrorAboveThreshold = new ExEventLog.EventTuple(3221489727U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PowerShellLegacyAuthzErrorAboveThreshold = new ExEventLog.EventTuple(3221489728U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PSWSLongLatencyCmdletAboveThreshold = new ExEventLog.EventTuple(3221489729U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PSWSUnhandledCmdletExceptionAboveThreshold = new ExEventLog.EventTuple(3221489730U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PowerShellHttpGenericErrorAboveThreshold = new ExEventLog.EventTuple(3221489731U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PowerShellLiveIDHttpGenericErrorAboveThreshold = new ExEventLog.EventTuple(3221489732U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PowerShellLegacyHttpGenericErrorAboveThreshold = new ExEventLog.EventTuple(3221489733U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PowerShellHttpErrorResponseAboveThreshold = new ExEventLog.EventTuple(3221489734U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PowerShellLiveIDHttpErrorResponseAboveThreshold = new ExEventLog.EventTuple(3221489735U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PowerShellLegacyHttpErrorResponseAboveThreshold = new ExEventLog.EventTuple(3221489736U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TestApi1_TestCan1_EX = new ExEventLog.EventTuple(3221488686U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TestApi1_TestCan1_OK = new ExEventLog.EventTuple(1074005039U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TestApi2_TestCan2_EX = new ExEventLog.EventTuple(3221488688U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TestApi2_TestCan2_OK = new ExEventLog.EventTuple(1074005041U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FindPeople_BrowseInMailbox_EX = new ExEventLog.EventTuple(3221488690U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FindPeople_BrowseInMailbox_OK = new ExEventLog.EventTuple(1074005043U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FindPeople_HoverCard_EX = new ExEventLog.EventTuple(3221488692U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FindPeople_HoverCard_OK = new ExEventLog.EventTuple(1074005045U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FindPeople_RecipientCache_EX = new ExEventLog.EventTuple(3221488694U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FindPeople_RecipientCache_OK = new ExEventLog.EventTuple(1074005047U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TestApi4__EX = new ExEventLog.EventTuple(3221488696U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TestApi4__OK = new ExEventLog.EventTuple(1074005049U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CreateItem_CreateMessageForComposeSend_EX = new ExEventLog.EventTuple(3221489316U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CreateItem_CreateMessageForComposeSend_OK = new ExEventLog.EventTuple(1074005669U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CreateItem_MailComposeUpgrade_EX = new ExEventLog.EventTuple(3221489318U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CreateItem_MailComposeUpgrade_OK = new ExEventLog.EventTuple(1074005671U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CreateItem_CreateResponseSend_EX = new ExEventLog.EventTuple(3221489320U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CreateItem_CreateResponseSend_OK = new ExEventLog.EventTuple(1074005673U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UpdateItem_UpdateMessageForComposeSend_EX = new ExEventLog.EventTuple(3221489322U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UpdateItem_UpdateMessageForComposeSend_OK = new ExEventLog.EventTuple(1074005675U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CreateItem_CreateMessageForCompose_EX = new ExEventLog.EventTuple(3221489624U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CreateItem_CreateMessageForCompose_OK = new ExEventLog.EventTuple(1074005977U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CreateItem_CreateResponse_EX = new ExEventLog.EventTuple(3221489626U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CreateItem_CreateResponse_OK = new ExEventLog.EventTuple(1074005979U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UpdateItem_UpdateMessageForCompose_EX = new ExEventLog.EventTuple(3221489628U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UpdateItem_UpdateMessageForCompose_OK = new ExEventLog.EventTuple(1074005981U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GetWacAttachmentInfo__EX = new ExEventLog.EventTuple(3221488817U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GetWacAttachmentInfo__OK = new ExEventLog.EventTuple(1074005170U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WACMdbCacheUpdate__EX = new ExEventLog.EventTuple(3221488819U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WACMdbCacheUpdate__OK = new ExEventLog.EventTuple(1074005172U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WACCheckFile__EX = new ExEventLog.EventTuple(3221488821U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WACCheckFile__OK = new ExEventLog.EventTuple(1074005174U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WACGetFile__EX = new ExEventLog.EventTuple(3221488823U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WACGetFile__OK = new ExEventLog.EventTuple(1074005176U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WACCleanCobaltStore__EX = new ExEventLog.EventTuple(3221488825U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WACCleanCobaltStore__OK = new ExEventLog.EventTuple(1074005178U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WACPutFile__EX = new ExEventLog.EventTuple(3221488827U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WACPutFile__OK = new ExEventLog.EventTuple(1074005180U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WACCobalt__EX = new ExEventLog.EventTuple(3221488829U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WACCobalt__OK = new ExEventLog.EventTuple(1074005182U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WACAutoSave__EX = new ExEventLog.EventTuple(3221488831U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WACAutoSave__OK = new ExEventLog.EventTuple(1074005184U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GetWacIFrameUrl__EX = new ExEventLog.EventTuple(3221488833U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GetWacIFrameUrl__OK = new ExEventLog.EventTuple(1074005186U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WACBadRequest__EX = new ExEventLog.EventTuple(3221488835U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WACBadRequest__OK = new ExEventLog.EventTuple(1074005188U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WACLock__EX = new ExEventLog.EventTuple(3221488837U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WACLock__OK = new ExEventLog.EventTuple(1074005190U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WACUnlock__EX = new ExEventLog.EventTuple(3221488839U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WACUnlock__OK = new ExEventLog.EventTuple(1074005192U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WACCacheEntryExpired__EX = new ExEventLog.EventTuple(3221488841U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WACCacheEntryExpired__OK = new ExEventLog.EventTuple(1074005194U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WACRefreshLock__EX = new ExEventLog.EventTuple(3221488843U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WACRefreshLock__OK = new ExEventLog.EventTuple(1074005196U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CreateAttachment__EX = new ExEventLog.EventTuple(3221488845U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CreateAttachment__OK = new ExEventLog.EventTuple(1074005198U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CreateAttachmentFromAttachmentDataProvider__EX = new ExEventLog.EventTuple(3221488847U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CreateAttachmentFromAttachmentDataProvider__OK = new ExEventLog.EventTuple(1074005200U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CreateReferenceAttachmentFromLocalFile__EX = new ExEventLog.EventTuple(3221488849U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CreateReferenceAttachmentFromLocalFile__OK = new ExEventLog.EventTuple(1074005202U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CreateReferenceAttachmentFromAttachmentDataProvider__EX = new ExEventLog.EventTuple(3221488851U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CreateReferenceAttachmentFromAttachmentDataProvider__OK = new ExEventLog.EventTuple(1074005204U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GetAttachmentDataProviders__EX = new ExEventLog.EventTuple(3221488853U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GetAttachmentDataProviders__OK = new ExEventLog.EventTuple(1074005206U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GetAttachmentDataProviderItems__EX = new ExEventLog.EventTuple(3221488855U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GetAttachmentDataProviderItems__OK = new ExEventLog.EventTuple(1074005208U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GetAttachmentDataProviderRecentItems__EX = new ExEventLog.EventTuple(3221488857U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GetAttachmentDataProviderRecentItems__OK = new ExEventLog.EventTuple(1074005210U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SynchronizeWacAttachment__EX = new ExEventLog.EventTuple(3221488859U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SynchronizeWacAttachment__OK = new ExEventLog.EventTuple(1074005212U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GetFileAttachment__EX = new ExEventLog.EventTuple(3221488861U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GetFileAttachment__OK = new ExEventLog.EventTuple(1074005214U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FindConversation_BrowseAll_EX = new ExEventLog.EventTuple(3221489116U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FindConversation_BrowseAll_OK = new ExEventLog.EventTuple(1074005469U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FindConversation_BrowseUnread_EX = new ExEventLog.EventTuple(3221489118U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FindConversation_BrowseUnread_OK = new ExEventLog.EventTuple(1074005471U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FindConversation_BrowseNoClutterAll_EX = new ExEventLog.EventTuple(3221489126U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FindConversation_BrowseNoClutterAll_OK = new ExEventLog.EventTuple(1074005479U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FindConversation_BrowseNoClutterUnread_EX = new ExEventLog.EventTuple(3221489128U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FindConversation_BrowseNoClutterUnread_OK = new ExEventLog.EventTuple(1074005481U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GetConversationItems__EX = new ExEventLog.EventTuple(3221488700U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GetConversationItems__OK = new ExEventLog.EventTuple(1074005053U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FindItem_BrowseAll_EX = new ExEventLog.EventTuple(3221489146U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FindItem_BrowseAll_OK = new ExEventLog.EventTuple(1074005499U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FindItem_BrowseUnread_EX = new ExEventLog.EventTuple(3221489148U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FindItem_BrowseUnread_OK = new ExEventLog.EventTuple(1074005501U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FindItem_BrowseNoClutterAll_EX = new ExEventLog.EventTuple(3221489156U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FindItem_BrowseNoClutterAll_OK = new ExEventLog.EventTuple(1074005509U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FindItem_BrowseNoClutterUnread_EX = new ExEventLog.EventTuple(3221489158U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FindItem_BrowseNoClutterUnread_OK = new ExEventLog.EventTuple(1074005511U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GetItem_GetMailItem_EX = new ExEventLog.EventTuple(3221488704U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GetItem_GetMailItem_OK = new ExEventLog.EventTuple(1074005057U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GetExtensibilityContext_ExtLoadApps_EX = new ExEventLog.EventTuple(3221489017U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GetExtensibilityContext_ExtLoadApps_OK = new ExEventLog.EventTuple(1074005370U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CreateModernGroup_CreateModernGroupAction_EX = new ExEventLog.EventTuple(3221489019U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CreateModernGroup_CreateModernGroupAction_OK = new ExEventLog.EventTuple(1074005372U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CreateUnifiedGroup_CreateUnifiedGroupAction_EX = new ExEventLog.EventTuple(3221489041U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CreateUnifiedGroup_CreateUnifiedGroupAction_OK = new ExEventLog.EventTuple(1074005394U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AddMembersToUnifiedGroup_AddMembersToUnifiedGroupAction_EX = new ExEventLog.EventTuple(3221489043U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AddMembersToUnifiedGroup_AddMembersToUnifiedGroupAction_OK = new ExEventLog.EventTuple(1074005396U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GetModernGroup_GetModernGroupAction_EX = new ExEventLog.EventTuple(3221489021U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GetModernGroup_GetModernGroupAction_OK = new ExEventLog.EventTuple(1074005374U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GetModernGroups_GetModernGroupsAction_EX = new ExEventLog.EventTuple(3221489023U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GetModernGroups_GetModernGroupsAction_OK = new ExEventLog.EventTuple(1074005376U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GetModernGroupUnseenItems_GetModernGroupUnseenItemsAction_EX = new ExEventLog.EventTuple(3221489025U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GetModernGroupUnseenItems_GetModernGroupUnseenItemsAction_OK = new ExEventLog.EventTuple(1074005378U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PostModernGroupItem_QuickCompose_EX = new ExEventLog.EventTuple(3221489027U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PostModernGroupItem_QuickCompose_OK = new ExEventLog.EventTuple(1074005380U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PostModernGroupItem_QuickReply_EX = new ExEventLog.EventTuple(3221489045U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PostModernGroupItem_QuickReply_OK = new ExEventLog.EventTuple(1074005398U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UpdateModernGroup_UpdateModernGroupAction_EX = new ExEventLog.EventTuple(3221489029U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UpdateModernGroup_UpdateModernGroupAction_OK = new ExEventLog.EventTuple(1074005382U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GetConversationItems_ModernGroup_EX = new ExEventLog.EventTuple(3221489031U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GetConversationItems_ModernGroup_OK = new ExEventLog.EventTuple(1074005384U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FindConversation_ModernGroupAll_EX = new ExEventLog.EventTuple(3221489033U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FindConversation_ModernGroupAll_OK = new ExEventLog.EventTuple(1074005386U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FindConversation_SearchModernGroupAll_EX = new ExEventLog.EventTuple(3221489047U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FindConversation_SearchModernGroupAll_OK = new ExEventLog.EventTuple(1074005400U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SetModernGroupMembership_JoinModernGroup_EX = new ExEventLog.EventTuple(3221489035U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SetModernGroupMembership_JoinModernGroup_OK = new ExEventLog.EventTuple(1074005388U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SetModernGroupMembership_LeaveModernGroup_EX = new ExEventLog.EventTuple(3221489037U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SetModernGroupMembership_LeaveModernGroup_OK = new ExEventLog.EventTuple(1074005390U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RemoveModernGroup_RemoveModernGroupAction_EX = new ExEventLog.EventTuple(3221489039U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RemoveModernGroup_RemoveModernGroupAction_OK = new ExEventLog.EventTuple(1074005392U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GetPersonaPhoto__EX = new ExEventLog.EventTuple(3221489287U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GetPersonaPhoto__OK = new ExEventLog.EventTuple(1074005640U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UploadPhoto__EX = new ExEventLog.EventTuple(3221489312U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UploadPhoto__OK = new ExEventLog.EventTuple(1074005665U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UploadPhotoFromForm__EX = new ExEventLog.EventTuple(3221489314U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UploadPhotoFromForm__OK = new ExEventLog.EventTuple(1074005667U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ProcessSuiteStorage__EX = new ExEventLog.EventTuple(3221489417U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ProcessSuiteStorage__OK = new ExEventLog.EventTuple(1074005770U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SetNotificationSettings__EX = new ExEventLog.EventTuple(3221489419U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SetNotificationSettings__OK = new ExEventLog.EventTuple(1074005772U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TestApi1__EX = new ExEventLog.EventTuple(3221489324U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TestApi1__OK = new ExEventLog.EventTuple(1074005677U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TestApi2__EX = new ExEventLog.EventTuple(3221489326U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TestApi2__OK = new ExEventLog.EventTuple(1074005679U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GetUserPhoto__EX = new ExEventLog.EventTuple(3221489328U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GetUserPhoto__OK = new ExEventLog.EventTuple(1074005681U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FindPeople_PeopleIKnow_EX = new ExEventLog.EventTuple(3221489310U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FindPeople_PeopleIKnow_OK = new ExEventLog.EventTuple(1074005663U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FindPeople_BrowseInDirectory_EX = new ExEventLog.EventTuple(3221489517U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FindPeople_BrowseInDirectory_OK = new ExEventLog.EventTuple(1074005870U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FindPeople_PeopleModule_EX = new ExEventLog.EventTuple(3221489519U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FindPeople_PeopleModule_OK = new ExEventLog.EventTuple(1074005872U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FindPeople_ComposeForms_EX = new ExEventLog.EventTuple(3221489521U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FindPeople_ComposeForms_OK = new ExEventLog.EventTuple(1074005874U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GetPersona_PersonaCardreadOnlyRecipientWell_EX = new ExEventLog.EventTuple(3221489367U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GetPersona_PersonaCardreadOnlyRecipientWell_OK = new ExEventLog.EventTuple(1074005720U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GetPersona_PersonaCardreadWriteRecipientWell_EX = new ExEventLog.EventTuple(3221489369U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GetPersona_PersonaCardreadWriteRecipientWell_OK = new ExEventLog.EventTuple(1074005722U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GetPersona_PersonaCardsharePoint_EX = new ExEventLog.EventTuple(3221489371U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GetPersona_PersonaCardsharePoint_OK = new ExEventLog.EventTuple(1074005724U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GetPersona_PersonaCardpeopleHub_EX = new ExEventLog.EventTuple(3221489373U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GetPersona_PersonaCardpeopleHub_OK = new ExEventLog.EventTuple(1074005726U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GetPersona_HoverCardreadOnlyRecipientWell_EX = new ExEventLog.EventTuple(3221489377U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GetPersona_HoverCardreadOnlyRecipientWell_OK = new ExEventLog.EventTuple(1074005730U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GetPersona_HoverCardreadWriteRecipientWell_EX = new ExEventLog.EventTuple(3221489379U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GetPersona_HoverCardreadWriteRecipientWell_OK = new ExEventLog.EventTuple(1074005732U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PerfTraceCTQ_TestCtq1_EX = new ExEventLog.EventTuple(3221488706U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PerfTraceCTQ_TestCtq1_OK = new ExEventLog.EventTuple(1074005059U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PerfTraceCTQ_TestCtq2_EX = new ExEventLog.EventTuple(3221488708U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PerfTraceCTQ_TestCtq2_OK = new ExEventLog.EventTuple(1074005061U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PerfTraceCTQ_NavigateToPeople_EX = new ExEventLog.EventTuple(3221488710U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PerfTraceCTQ_NavigateToPeople_OK = new ExEventLog.EventTuple(1074005063U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PerfTraceCTQ_ShowPersonaCardCollapsed_EX = new ExEventLog.EventTuple(3221489437U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PerfTraceCTQ_ShowPersonaCardCollapsed_OK = new ExEventLog.EventTuple(1074005790U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PerfTraceCTQ_ShowPersonaCardExpanded_EX = new ExEventLog.EventTuple(3221489439U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PerfTraceCTQ_ShowPersonaCardExpanded_OK = new ExEventLog.EventTuple(1074005792U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PerfTraceCTQ_GetWacAttachmentInfo_EX = new ExEventLog.EventTuple(3221488917U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PerfTraceCTQ_GetWacAttachmentInfo_OK = new ExEventLog.EventTuple(1074005270U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PerfTraceCTQ_EditACopy_EX = new ExEventLog.EventTuple(3221488919U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PerfTraceCTQ_EditACopy_OK = new ExEventLog.EventTuple(1074005272U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PerfTraceCTQ_DocumentAttachmentPopOut_EX = new ExEventLog.EventTuple(3221488921U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PerfTraceCTQ_DocumentAttachmentPopOut_OK = new ExEventLog.EventTuple(1074005274U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PerfTraceCTQ_GetWacIFrameUrl_EX = new ExEventLog.EventTuple(3221488925U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PerfTraceCTQ_GetWacIFrameUrl_OK = new ExEventLog.EventTuple(1074005278U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_Ecp_EventLog_HttpUnhandledExceptionReachedThreshold = new ExEventLog.EventTuple(3221488927U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_Ecp_EventLog_LandingDefaultPageErrorReachedThreshold = new ExEventLog.EventTuple(3221488928U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SSLCertificateWarningEvent = new ExEventLog.EventTuple(2147746912U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SSLCertificateErrorEvent = new ExEventLog.EventTuple(3221488737U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ExtensibilityOmexWsRequestErrorReachedThreshold = new ExEventLog.EventTuple(3221489236U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OAuthPassiveMonitoringExceptionAboveThreshold = new ExEventLog.EventTuple(3221489286U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OABGenTenantOutOfSLA = new ExEventLog.EventTuple(3221489166U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxAuditingFailureAboveThreshold = new ExEventLog.EventTuple(3221490617U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AdminAuditingFailureAboveThreshold = new ExEventLog.EventTuple(3221490618U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SynchronousAuditSearchFailureAboveThreshold = new ExEventLog.EventTuple(3221490619U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AsynchronousAuditSearchFailureAboveThreshold = new ExEventLog.EventTuple(3221490620U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PFAssistantItemProcessor_EX = new ExEventLog.EventTuple(3221489516U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EscalateItem_MSExchangeDelivery_EX = new ExEventLog.EventTuple(3221489616U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_EscalateItem_MSExchangeDelivery_OK = new ExEventLog.EventTuple(1074005969U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EscalationGetter_MSExchangeDelivery_EX = new ExEventLog.EventTuple(3221489618U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_EscalationGetter_MSExchangeDelivery_OK = new ExEventLog.EventTuple(1074005971U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EscalateItem_MSExchangeOWAAppPool_EX = new ExEventLog.EventTuple(3221489620U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_EscalateItem_MSExchangeOWAAppPool_OK = new ExEventLog.EventTuple(1074005973U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EscalationGetter_MSExchangeOWAAppPool_EX = new ExEventLog.EventTuple(3221489622U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_EscalationGetter_MSExchangeOWAAppPool_OK = new ExEventLog.EventTuple(1074005975U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HxService_WindowsEventLog_EX = new ExEventLog.EventTuple(3221489404U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_HxService_WindowsEventLog_OK = new ExEventLog.EventTuple(1074005757U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HxService_AllEventLog_EX = new ExEventLog.EventTuple(3221489376U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_HxService_AllEventLog_OK = new ExEventLog.EventTuple(1074005729U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HxService_NoUserEvents_EX = new ExEventLog.EventTuple(3221489378U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_HxService_NoUserEvents_OK = new ExEventLog.EventTuple(1074005731U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HxMail_GetConversationPartsCommand_EX = new ExEventLog.EventTuple(3221489330U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_HxMail_GetConversationPartsCommand_OK = new ExEventLog.EventTuple(1074005683U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HxMail_BeginSyncCommand_EX = new ExEventLog.EventTuple(3221489356U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_HxMail_BeginSyncCommand_OK = new ExEventLog.EventTuple(1074005709U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HxCalendar_GetBasedSyncCommand_EX = new ExEventLog.EventTuple(3221489332U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_HxCalendar_GetBasedSyncCommand_OK = new ExEventLog.EventTuple(1074005685U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HxCalendar_CreateItemCommand_EX = new ExEventLog.EventTuple(3221489340U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_HxCalendar_CreateItemCommand_OK = new ExEventLog.EventTuple(1074005693U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HxCalendar_CalendarDataCommand_EX = new ExEventLog.EventTuple(3221489342U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_HxCalendar_CalendarDataCommand_OK = new ExEventLog.EventTuple(1074005695U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HxCalendar_CancelEventCommand_EX = new ExEventLog.EventTuple(3221489344U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_HxCalendar_CancelEventCommand_OK = new ExEventLog.EventTuple(1074005697U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HxCalendar_UpdateItemCommand_EX = new ExEventLog.EventTuple(3221489346U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_HxCalendar_UpdateItemCommand_OK = new ExEventLog.EventTuple(1074005699U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HxCalendar_GetFullItemCommand_EX = new ExEventLog.EventTuple(3221489348U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_HxCalendar_GetFullItemCommand_OK = new ExEventLog.EventTuple(1074005701U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HxCalendar_SyncCommand_EX = new ExEventLog.EventTuple(3221489350U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_HxCalendar_SyncCommand_OK = new ExEventLog.EventTuple(1074005703U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HxService_HttpStatusErrorCode_EX = new ExEventLog.EventTuple(3221489406U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_HxService_HttpStatusErrorCode_OK = new ExEventLog.EventTuple(1074005759U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PFAssistantSplitFailed = new ExEventLog.EventTuple(3221489526U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HxMail_SearchResolveNameCommand_EX = new ExEventLog.EventTuple(3221489352U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_HxCalendar_SearchResolveNameCommand_EX = new ExEventLog.EventTuple(1074005705U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HxMail_SyncRecipientsCommand_EX = new ExEventLog.EventTuple(3221489354U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_HxCalendar_SyncRecipientsCommand_EX = new ExEventLog.EventTuple(1074005707U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HxMail_SetupMailboxCommand_EX = new ExEventLog.EventTuple(3221489358U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_HxMail_SetupMailboxCommand_OK = new ExEventLog.EventTuple(1074005711U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HxMail_AddAccountCommand_EX = new ExEventLog.EventTuple(3221489360U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_HxMail_AddAccountCommand_OK = new ExEventLog.EventTuple(1074005713U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GetAggregatedAccount_GetAggregatedAccountAction_EX = new ExEventLog.EventTuple(3221489362U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GetAggregatedAccount_GetAggregatedAccountAction_OK = new ExEventLog.EventTuple(1074005715U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AddAggregatedAccount_AddAggregatedAccountAction_EX = new ExEventLog.EventTuple(3221489364U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AddAggregatedAccount_AddAggregatedAccountAction_OK = new ExEventLog.EventTuple(1074005717U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SendLinkClickedSignalToSP_SendLinkClickedSignalToSPAction_EX = new ExEventLog.EventTuple(3221489366U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SendLinkClickedSignalToSP_SendLinkClickedSignalToSPAction_OK = new ExEventLog.EventTuple(1074005719U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		private enum Category : short
		{
			General = 1,
			Triggers
		}

		internal enum Message : uint
		{
			ConfigurationExceptionOnStartup = 3221488617U,
			DirectoryCreationException,
			PerformanceCounterWarningTriggerEvent = 2147746797U,
			PerformanceCounterErrorTriggerEvent = 3221488622U,
			PerformanceCounterInformationTriggerEvent = 1074005021U,
			PerformanceLogError = 3221488623U,
			ServiceStarting = 1074004976U,
			ServiceStarted = 1074004992U,
			ServiceStopping = 1074004977U,
			ServiceStopped = 1074004993U,
			RetentionAgentUnhandledException = 3221488627U,
			RetentionAgentDataLossOccurred,
			RetentionAgentPotentialDataLossWarning = 2147746805U,
			SqlOutputStreamUnhandledException = 3221488630U,
			ActiveDirectoryUnavailable,
			SqlOutputStreamConnectionStringFromAdNotFound,
			SqlOutputStreamConsecutiveRetriesForASpecifiedTime,
			SqlOutputStreamDecryptFailed,
			BadIisLogHeader = 3221488636U,
			IisLogLineNotProcessed,
			OwaServiceUnavailable,
			RelogManagerUnhandledException,
			OwaHttpStatus400 = 3221488642U,
			OwaHttpStatus440,
			OwaHttpStatus500,
			OwaHttpStatusOther,
			JobManagerStarted = 1074004998U,
			JobManagerNotStarted,
			JobManagerStartupFailures = 3221488648U,
			JobCrashed,
			JobPoisoned,
			BingServicesLatencyAboveThreshold,
			BingServicesLatencyBelowThreshold = 1074005004U,
			FindPlacesFailureAboveThreshold = 3221488653U,
			FindPlacesFailureBelowThreshold = 1074005006U,
			BitlockerStateDetectionError = 3221488655U,
			BitlockerState = 1074005008U,
			ConnectionStringManagerPartitionInvalid = 3221488657U,
			ConnectionStringManagerUnableToConnect,
			AnalyzerNotAdded = 1074005011U,
			WatsonCrashAlert = 3221488660U,
			WatsonExceptionCrashAlert = 3221488670U,
			OwaLyncFailureAboveThreshold = 3221488664U,
			OwaLyncFailureBelowThreshold = 1074005017U,
			OwaTooManyExceptionsEncountered = 3221488717U,
			TooManyObjectsOpenedException,
			OwaLogonFailures,
			OwaHighLatencyLoadStartPage,
			OwaTooManyHttpErrorResponsesEncountered,
			OwaTooManyHttpErrorResponsesResolved = 1074005074U,
			OabTooManyHttpErrorResponsesEncountered = 3221489167U,
			OabTooManyHttpErrorResponsesResolved = 1074005520U,
			OabTooManyExceptionsEncountered = 3221489169U,
			OabFileLoadException,
			IisAppPoolHttpErrorsDefaultEvent = 3221488723U,
			IisAppPoolTooManyHttpErrorResponsesEncounteredDefaultEvent,
			IisAppPoolTooManyHttpErrorResponsesResolvedDefaultEvent = 1074005077U,
			OwaStartPageFailures = 3221488726U,
			OwaStartPageFailuresResolved = 1074005079U,
			OwaActiveDatabaseAvailabilityBelowThreshold = 3221488728U,
			OwaActiveDatabaseAvailabilityBelowThresholdResolved = 1074005081U,
			OwaLogoffFailures = 3221488730U,
			OwaLogoffFailuresResolved = 1074005083U,
			TestEvent = 1074003968U,
			RbaRequestsWithExceptionsReachedThreshold = 3221488661U,
			RbaAtLeastOneExceptionReachedThreshold,
			RbaAllIsWell = 1074005015U,
			CalSyncRequestsWithExceptionsReachedThreshold = 3221488666U,
			CalSyncRequestsWithSyncFailuresReachedThreshold,
			CalSyncAllIsWell = 1074005020U,
			AvailabilityServiceExceptionAboveThreshold = 3221489272U,
			TxSyncPopRequestsWithExceptionsReachedThreshold = 3221488676U,
			TxSyncIMAPRequestsWithExceptionsReachedThreshold,
			TxSyncDeltaSyncMailRequestsWithExceptionsReachedThreshold,
			TxSyncFacebookRequestsWithExceptionsReachedThreshold,
			TxSyncLinkedInRequestsWithExceptionsReachedThreshold,
			TxSyncPopIsWell = 1074005033U,
			TxSyncIMAPIsWell,
			TxSyncDeltaSyncMailIsWell,
			TxSyncFacebookIsWell,
			TxSyncLinkedInIsWell,
			PowerShellTimeoutAuthzRequestAboveThreshold = 3221489717U,
			PowerShellLiveIDTimeoutAuthzRequestAboveThreshold,
			PowerShellLegacyTimeoutAuthzRequestAboveThreshold,
			PowerShellUnhandledCmdletExceptionAboveThreshold,
			PowerShellLiveIDUnhandledCmdletExceptionAboveThreshold,
			PowerShellLegacyUnhandledCmdletExceptionAboveThreshold,
			PowerShellLongLatencyCmdletAboveThreshold,
			PowerShellLiveIDLongLatencyCmdletAboveThreshold,
			PowerShellLegacyLongLatencyCmdletAboveThreshold,
			PowerShellAuthzErrorAboveThreshold,
			PowerShellLiveIDAuthzErrorAboveThreshold,
			PowerShellLegacyAuthzErrorAboveThreshold,
			PSWSLongLatencyCmdletAboveThreshold,
			PSWSUnhandledCmdletExceptionAboveThreshold,
			PowerShellHttpGenericErrorAboveThreshold,
			PowerShellLiveIDHttpGenericErrorAboveThreshold,
			PowerShellLegacyHttpGenericErrorAboveThreshold,
			PowerShellHttpErrorResponseAboveThreshold,
			PowerShellLiveIDHttpErrorResponseAboveThreshold,
			PowerShellLegacyHttpErrorResponseAboveThreshold,
			TestApi1_TestCan1_EX = 3221488686U,
			TestApi1_TestCan1_OK = 1074005039U,
			TestApi2_TestCan2_EX = 3221488688U,
			TestApi2_TestCan2_OK = 1074005041U,
			FindPeople_BrowseInMailbox_EX = 3221488690U,
			FindPeople_BrowseInMailbox_OK = 1074005043U,
			FindPeople_HoverCard_EX = 3221488692U,
			FindPeople_HoverCard_OK = 1074005045U,
			FindPeople_RecipientCache_EX = 3221488694U,
			FindPeople_RecipientCache_OK = 1074005047U,
			TestApi4__EX = 3221488696U,
			TestApi4__OK = 1074005049U,
			CreateItem_CreateMessageForComposeSend_EX = 3221489316U,
			CreateItem_CreateMessageForComposeSend_OK = 1074005669U,
			CreateItem_MailComposeUpgrade_EX = 3221489318U,
			CreateItem_MailComposeUpgrade_OK = 1074005671U,
			CreateItem_CreateResponseSend_EX = 3221489320U,
			CreateItem_CreateResponseSend_OK = 1074005673U,
			UpdateItem_UpdateMessageForComposeSend_EX = 3221489322U,
			UpdateItem_UpdateMessageForComposeSend_OK = 1074005675U,
			CreateItem_CreateMessageForCompose_EX = 3221489624U,
			CreateItem_CreateMessageForCompose_OK = 1074005977U,
			CreateItem_CreateResponse_EX = 3221489626U,
			CreateItem_CreateResponse_OK = 1074005979U,
			UpdateItem_UpdateMessageForCompose_EX = 3221489628U,
			UpdateItem_UpdateMessageForCompose_OK = 1074005981U,
			GetWacAttachmentInfo__EX = 3221488817U,
			GetWacAttachmentInfo__OK = 1074005170U,
			WACMdbCacheUpdate__EX = 3221488819U,
			WACMdbCacheUpdate__OK = 1074005172U,
			WACCheckFile__EX = 3221488821U,
			WACCheckFile__OK = 1074005174U,
			WACGetFile__EX = 3221488823U,
			WACGetFile__OK = 1074005176U,
			WACCleanCobaltStore__EX = 3221488825U,
			WACCleanCobaltStore__OK = 1074005178U,
			WACPutFile__EX = 3221488827U,
			WACPutFile__OK = 1074005180U,
			WACCobalt__EX = 3221488829U,
			WACCobalt__OK = 1074005182U,
			WACAutoSave__EX = 3221488831U,
			WACAutoSave__OK = 1074005184U,
			GetWacIFrameUrl__EX = 3221488833U,
			GetWacIFrameUrl__OK = 1074005186U,
			WACBadRequest__EX = 3221488835U,
			WACBadRequest__OK = 1074005188U,
			WACLock__EX = 3221488837U,
			WACLock__OK = 1074005190U,
			WACUnlock__EX = 3221488839U,
			WACUnlock__OK = 1074005192U,
			WACCacheEntryExpired__EX = 3221488841U,
			WACCacheEntryExpired__OK = 1074005194U,
			WACRefreshLock__EX = 3221488843U,
			WACRefreshLock__OK = 1074005196U,
			CreateAttachment__EX = 3221488845U,
			CreateAttachment__OK = 1074005198U,
			CreateAttachmentFromAttachmentDataProvider__EX = 3221488847U,
			CreateAttachmentFromAttachmentDataProvider__OK = 1074005200U,
			CreateReferenceAttachmentFromLocalFile__EX = 3221488849U,
			CreateReferenceAttachmentFromLocalFile__OK = 1074005202U,
			CreateReferenceAttachmentFromAttachmentDataProvider__EX = 3221488851U,
			CreateReferenceAttachmentFromAttachmentDataProvider__OK = 1074005204U,
			GetAttachmentDataProviders__EX = 3221488853U,
			GetAttachmentDataProviders__OK = 1074005206U,
			GetAttachmentDataProviderItems__EX = 3221488855U,
			GetAttachmentDataProviderItems__OK = 1074005208U,
			GetAttachmentDataProviderRecentItems__EX = 3221488857U,
			GetAttachmentDataProviderRecentItems__OK = 1074005210U,
			SynchronizeWacAttachment__EX = 3221488859U,
			SynchronizeWacAttachment__OK = 1074005212U,
			GetFileAttachment__EX = 3221488861U,
			GetFileAttachment__OK = 1074005214U,
			FindConversation_BrowseAll_EX = 3221489116U,
			FindConversation_BrowseAll_OK = 1074005469U,
			FindConversation_BrowseUnread_EX = 3221489118U,
			FindConversation_BrowseUnread_OK = 1074005471U,
			FindConversation_BrowseNoClutterAll_EX = 3221489126U,
			FindConversation_BrowseNoClutterAll_OK = 1074005479U,
			FindConversation_BrowseNoClutterUnread_EX = 3221489128U,
			FindConversation_BrowseNoClutterUnread_OK = 1074005481U,
			GetConversationItems__EX = 3221488700U,
			GetConversationItems__OK = 1074005053U,
			FindItem_BrowseAll_EX = 3221489146U,
			FindItem_BrowseAll_OK = 1074005499U,
			FindItem_BrowseUnread_EX = 3221489148U,
			FindItem_BrowseUnread_OK = 1074005501U,
			FindItem_BrowseNoClutterAll_EX = 3221489156U,
			FindItem_BrowseNoClutterAll_OK = 1074005509U,
			FindItem_BrowseNoClutterUnread_EX = 3221489158U,
			FindItem_BrowseNoClutterUnread_OK = 1074005511U,
			GetItem_GetMailItem_EX = 3221488704U,
			GetItem_GetMailItem_OK = 1074005057U,
			GetExtensibilityContext_ExtLoadApps_EX = 3221489017U,
			GetExtensibilityContext_ExtLoadApps_OK = 1074005370U,
			CreateModernGroup_CreateModernGroupAction_EX = 3221489019U,
			CreateModernGroup_CreateModernGroupAction_OK = 1074005372U,
			CreateUnifiedGroup_CreateUnifiedGroupAction_EX = 3221489041U,
			CreateUnifiedGroup_CreateUnifiedGroupAction_OK = 1074005394U,
			AddMembersToUnifiedGroup_AddMembersToUnifiedGroupAction_EX = 3221489043U,
			AddMembersToUnifiedGroup_AddMembersToUnifiedGroupAction_OK = 1074005396U,
			GetModernGroup_GetModernGroupAction_EX = 3221489021U,
			GetModernGroup_GetModernGroupAction_OK = 1074005374U,
			GetModernGroups_GetModernGroupsAction_EX = 3221489023U,
			GetModernGroups_GetModernGroupsAction_OK = 1074005376U,
			GetModernGroupUnseenItems_GetModernGroupUnseenItemsAction_EX = 3221489025U,
			GetModernGroupUnseenItems_GetModernGroupUnseenItemsAction_OK = 1074005378U,
			PostModernGroupItem_QuickCompose_EX = 3221489027U,
			PostModernGroupItem_QuickCompose_OK = 1074005380U,
			PostModernGroupItem_QuickReply_EX = 3221489045U,
			PostModernGroupItem_QuickReply_OK = 1074005398U,
			UpdateModernGroup_UpdateModernGroupAction_EX = 3221489029U,
			UpdateModernGroup_UpdateModernGroupAction_OK = 1074005382U,
			GetConversationItems_ModernGroup_EX = 3221489031U,
			GetConversationItems_ModernGroup_OK = 1074005384U,
			FindConversation_ModernGroupAll_EX = 3221489033U,
			FindConversation_ModernGroupAll_OK = 1074005386U,
			FindConversation_SearchModernGroupAll_EX = 3221489047U,
			FindConversation_SearchModernGroupAll_OK = 1074005400U,
			SetModernGroupMembership_JoinModernGroup_EX = 3221489035U,
			SetModernGroupMembership_JoinModernGroup_OK = 1074005388U,
			SetModernGroupMembership_LeaveModernGroup_EX = 3221489037U,
			SetModernGroupMembership_LeaveModernGroup_OK = 1074005390U,
			RemoveModernGroup_RemoveModernGroupAction_EX = 3221489039U,
			RemoveModernGroup_RemoveModernGroupAction_OK = 1074005392U,
			GetPersonaPhoto__EX = 3221489287U,
			GetPersonaPhoto__OK = 1074005640U,
			UploadPhoto__EX = 3221489312U,
			UploadPhoto__OK = 1074005665U,
			UploadPhotoFromForm__EX = 3221489314U,
			UploadPhotoFromForm__OK = 1074005667U,
			ProcessSuiteStorage__EX = 3221489417U,
			ProcessSuiteStorage__OK = 1074005770U,
			SetNotificationSettings__EX = 3221489419U,
			SetNotificationSettings__OK = 1074005772U,
			TestApi1__EX = 3221489324U,
			TestApi1__OK = 1074005677U,
			TestApi2__EX = 3221489326U,
			TestApi2__OK = 1074005679U,
			GetUserPhoto__EX = 3221489328U,
			GetUserPhoto__OK = 1074005681U,
			FindPeople_PeopleIKnow_EX = 3221489310U,
			FindPeople_PeopleIKnow_OK = 1074005663U,
			FindPeople_BrowseInDirectory_EX = 3221489517U,
			FindPeople_BrowseInDirectory_OK = 1074005870U,
			FindPeople_PeopleModule_EX = 3221489519U,
			FindPeople_PeopleModule_OK = 1074005872U,
			FindPeople_ComposeForms_EX = 3221489521U,
			FindPeople_ComposeForms_OK = 1074005874U,
			GetPersona_PersonaCardreadOnlyRecipientWell_EX = 3221489367U,
			GetPersona_PersonaCardreadOnlyRecipientWell_OK = 1074005720U,
			GetPersona_PersonaCardreadWriteRecipientWell_EX = 3221489369U,
			GetPersona_PersonaCardreadWriteRecipientWell_OK = 1074005722U,
			GetPersona_PersonaCardsharePoint_EX = 3221489371U,
			GetPersona_PersonaCardsharePoint_OK = 1074005724U,
			GetPersona_PersonaCardpeopleHub_EX = 3221489373U,
			GetPersona_PersonaCardpeopleHub_OK = 1074005726U,
			GetPersona_HoverCardreadOnlyRecipientWell_EX = 3221489377U,
			GetPersona_HoverCardreadOnlyRecipientWell_OK = 1074005730U,
			GetPersona_HoverCardreadWriteRecipientWell_EX = 3221489379U,
			GetPersona_HoverCardreadWriteRecipientWell_OK = 1074005732U,
			PerfTraceCTQ_TestCtq1_EX = 3221488706U,
			PerfTraceCTQ_TestCtq1_OK = 1074005059U,
			PerfTraceCTQ_TestCtq2_EX = 3221488708U,
			PerfTraceCTQ_TestCtq2_OK = 1074005061U,
			PerfTraceCTQ_NavigateToPeople_EX = 3221488710U,
			PerfTraceCTQ_NavigateToPeople_OK = 1074005063U,
			PerfTraceCTQ_ShowPersonaCardCollapsed_EX = 3221489437U,
			PerfTraceCTQ_ShowPersonaCardCollapsed_OK = 1074005790U,
			PerfTraceCTQ_ShowPersonaCardExpanded_EX = 3221489439U,
			PerfTraceCTQ_ShowPersonaCardExpanded_OK = 1074005792U,
			PerfTraceCTQ_GetWacAttachmentInfo_EX = 3221488917U,
			PerfTraceCTQ_GetWacAttachmentInfo_OK = 1074005270U,
			PerfTraceCTQ_EditACopy_EX = 3221488919U,
			PerfTraceCTQ_EditACopy_OK = 1074005272U,
			PerfTraceCTQ_DocumentAttachmentPopOut_EX = 3221488921U,
			PerfTraceCTQ_DocumentAttachmentPopOut_OK = 1074005274U,
			PerfTraceCTQ_GetWacIFrameUrl_EX = 3221488925U,
			PerfTraceCTQ_GetWacIFrameUrl_OK = 1074005278U,
			Ecp_EventLog_HttpUnhandledExceptionReachedThreshold = 3221488927U,
			Ecp_EventLog_LandingDefaultPageErrorReachedThreshold,
			SSLCertificateWarningEvent = 2147746912U,
			SSLCertificateErrorEvent = 3221488737U,
			ExtensibilityOmexWsRequestErrorReachedThreshold = 3221489236U,
			OAuthPassiveMonitoringExceptionAboveThreshold = 3221489286U,
			OABGenTenantOutOfSLA = 3221489166U,
			MailboxAuditingFailureAboveThreshold = 3221490617U,
			AdminAuditingFailureAboveThreshold,
			SynchronousAuditSearchFailureAboveThreshold,
			AsynchronousAuditSearchFailureAboveThreshold,
			PFAssistantItemProcessor_EX = 3221489516U,
			EscalateItem_MSExchangeDelivery_EX = 3221489616U,
			EscalateItem_MSExchangeDelivery_OK = 1074005969U,
			EscalationGetter_MSExchangeDelivery_EX = 3221489618U,
			EscalationGetter_MSExchangeDelivery_OK = 1074005971U,
			EscalateItem_MSExchangeOWAAppPool_EX = 3221489620U,
			EscalateItem_MSExchangeOWAAppPool_OK = 1074005973U,
			EscalationGetter_MSExchangeOWAAppPool_EX = 3221489622U,
			EscalationGetter_MSExchangeOWAAppPool_OK = 1074005975U,
			HxService_WindowsEventLog_EX = 3221489404U,
			HxService_WindowsEventLog_OK = 1074005757U,
			HxService_AllEventLog_EX = 3221489376U,
			HxService_AllEventLog_OK = 1074005729U,
			HxService_NoUserEvents_EX = 3221489378U,
			HxService_NoUserEvents_OK = 1074005731U,
			HxMail_GetConversationPartsCommand_EX = 3221489330U,
			HxMail_GetConversationPartsCommand_OK = 1074005683U,
			HxMail_BeginSyncCommand_EX = 3221489356U,
			HxMail_BeginSyncCommand_OK = 1074005709U,
			HxCalendar_GetBasedSyncCommand_EX = 3221489332U,
			HxCalendar_GetBasedSyncCommand_OK = 1074005685U,
			HxCalendar_CreateItemCommand_EX = 3221489340U,
			HxCalendar_CreateItemCommand_OK = 1074005693U,
			HxCalendar_CalendarDataCommand_EX = 3221489342U,
			HxCalendar_CalendarDataCommand_OK = 1074005695U,
			HxCalendar_CancelEventCommand_EX = 3221489344U,
			HxCalendar_CancelEventCommand_OK = 1074005697U,
			HxCalendar_UpdateItemCommand_EX = 3221489346U,
			HxCalendar_UpdateItemCommand_OK = 1074005699U,
			HxCalendar_GetFullItemCommand_EX = 3221489348U,
			HxCalendar_GetFullItemCommand_OK = 1074005701U,
			HxCalendar_SyncCommand_EX = 3221489350U,
			HxCalendar_SyncCommand_OK = 1074005703U,
			HxService_HttpStatusErrorCode_EX = 3221489406U,
			HxService_HttpStatusErrorCode_OK = 1074005759U,
			PFAssistantSplitFailed = 3221489526U,
			HxMail_SearchResolveNameCommand_EX = 3221489352U,
			HxCalendar_SearchResolveNameCommand_EX = 1074005705U,
			HxMail_SyncRecipientsCommand_EX = 3221489354U,
			HxCalendar_SyncRecipientsCommand_EX = 1074005707U,
			HxMail_SetupMailboxCommand_EX = 3221489358U,
			HxMail_SetupMailboxCommand_OK = 1074005711U,
			HxMail_AddAccountCommand_EX = 3221489360U,
			HxMail_AddAccountCommand_OK = 1074005713U,
			GetAggregatedAccount_GetAggregatedAccountAction_EX = 3221489362U,
			GetAggregatedAccount_GetAggregatedAccountAction_OK = 1074005715U,
			AddAggregatedAccount_AddAggregatedAccountAction_EX = 3221489364U,
			AddAggregatedAccount_AddAggregatedAccountAction_OK = 1074005717U,
			SendLinkClickedSignalToSP_SendLinkClickedSignalToSPAction_EX = 3221489366U,
			SendLinkClickedSignalToSP_SendLinkClickedSignalToSPAction_OK = 1074005719U
		}
	}
}
