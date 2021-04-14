using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.LogUploaderProxy
{
	public static class LogUploaderEventLogConstants
	{
		public const string EventSource = "LogUploader";

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_Startup_Impl = new ExEventLog.EventTuple(1073742824U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_Startup = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_Startup_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_Shutdown_Impl = new ExEventLog.EventTuple(1073742825U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_Shutdown = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_Shutdown_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_ServiceDisabled_Impl = new ExEventLog.EventTuple(1073742826U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_ServiceDisabled = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_ServiceDisabled_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_ServiceEnabled_Impl = new ExEventLog.EventTuple(1073742827U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_ServiceEnabled = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_ServiceEnabled_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_FailToAccessADTemporarily_Impl = new ExEventLog.EventTuple(3221226476U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_FailToAccessADTemporarily = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_FailToAccessADTemporarily_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_FailedToInstantiateLogFileInfoFileNotExist_Impl = new ExEventLog.EventTuple(3221226477U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_FailedToInstantiateLogFileInfoFileNotExist = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_FailedToInstantiateLogFileInfoFileNotExist_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_ReadConfigFromADSucceeded_Impl = new ExEventLog.EventTuple(1073742830U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_ReadConfigFromADSucceeded = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_ReadConfigFromADSucceeded_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_ServerNotFoundInAD_Impl = new ExEventLog.EventTuple(3221226479U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_ServerNotFoundInAD = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_ServerNotFoundInAD_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_LogTypeNotFoundInConfigFile_Impl = new ExEventLog.EventTuple(3221226481U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_LogTypeNotFoundInConfigFile = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_LogTypeNotFoundInConfigFile_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_ParamNotFoundInConfigFile_Impl = new ExEventLog.EventTuple(3221226482U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_ParamNotFoundInConfigFile = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_ParamNotFoundInConfigFile_Impl);

		[EventLogPeriod(Period = "LogAlways")]
		private static readonly ExEventLog.EventTuple Tuple_FailedToReadConfigFile_Impl = new ExEventLog.EventTuple(3221226484U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		public static readonly ExEventLog.EventTuple Tuple_FailedToReadConfigFile = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_FailedToReadConfigFile_Impl);

		[EventLogPeriod(Period = "LogAlways")]
		private static readonly ExEventLog.EventTuple Tuple_ServiceStartUnknownException_Impl = new ExEventLog.EventTuple(3221226485U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		public static readonly ExEventLog.EventTuple Tuple_ServiceStartUnknownException = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_ServiceStartUnknownException_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_FailedToGetConfigValueFromAD_Impl = new ExEventLog.EventTuple(3221226486U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_FailedToGetConfigValueFromAD = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_FailedToGetConfigValueFromAD_Impl);

		[EventLogPeriod(Period = "LogAlways")]
		private static readonly ExEventLog.EventTuple Tuple_ServiceStartedForProcessingLogs_Impl = new ExEventLog.EventTuple(1073742839U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		public static readonly ExEventLog.EventTuple Tuple_ServiceStartedForProcessingLogs = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_ServiceStartedForProcessingLogs_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_NoLogsToProcess_Impl = new ExEventLog.EventTuple(3221226488U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_NoLogsToProcess = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_NoLogsToProcess_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_LogDirIsDisabled_Impl = new ExEventLog.EventTuple(1073742842U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_LogDirIsDisabled = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_LogDirIsDisabled_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_WorkerStartup_Impl = new ExEventLog.EventTuple(1073742843U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_WorkerStartup = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_WorkerStartup_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_UnsupportedLogSchemaType_Impl = new ExEventLog.EventTuple(3221226492U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_UnsupportedLogSchemaType = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_UnsupportedLogSchemaType_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_FailedToRemoveMessagesForDomain_Impl = new ExEventLog.EventTuple(3221226494U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_FailedToRemoveMessagesForDomain = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_FailedToRemoveMessagesForDomain_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_ConfigSettingNotFound_Impl = new ExEventLog.EventTuple(2147484671U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_ConfigSettingNotFound = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_ConfigSettingNotFound_Impl);

		[EventLogPeriod(Period = "LogAlways")]
		private static readonly ExEventLog.EventTuple Tuple_InconsistentPersistentStoreCopies_Impl = new ExEventLog.EventTuple(1074004992U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		public static readonly ExEventLog.EventTuple Tuple_InconsistentPersistentStoreCopies = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_InconsistentPersistentStoreCopies_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_FailedToGetLogPath_Impl = new ExEventLog.EventTuple(3221488641U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_FailedToGetLogPath = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_FailedToGetLogPath_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_MissingWatermark_Impl = new ExEventLog.EventTuple(3221227472U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_MissingWatermark = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_MissingWatermark_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_DatabaseWriterUnknownException_Impl = new ExEventLog.EventTuple(3221489617U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_DatabaseWriterUnknownException = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_DatabaseWriterUnknownException_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_DatabaseWriterPermanentException_Impl = new ExEventLog.EventTuple(3221489618U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_DatabaseWriterPermanentException = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_DatabaseWriterPermanentException_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_DatabaseWriterTransientException_Impl = new ExEventLog.EventTuple(3221489619U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_DatabaseWriterTransientException = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_DatabaseWriterTransientException_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_WritePerItemPermanentException_Impl = new ExEventLog.EventTuple(3221489620U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_WritePerItemPermanentException = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_WritePerItemPermanentException_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_WebServiceWriteException_Impl = new ExEventLog.EventTuple(3221489621U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_WebServiceWriteException = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_WebServiceWriteException_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_WritePerItemPermanentException2_Impl = new ExEventLog.EventTuple(3221489622U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_WritePerItemPermanentException2 = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_WritePerItemPermanentException2_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_DatabaseWriterTransientException2_Impl = new ExEventLog.EventTuple(3221489623U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_DatabaseWriterTransientException2 = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_DatabaseWriterTransientException2_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_FailedToGetTenantDomain_Impl = new ExEventLog.EventTuple(3221489625U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_FailedToGetTenantDomain = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_FailedToGetTenantDomain_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_TransportQueueDBWriteException_Impl = new ExEventLog.EventTuple(3221489626U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_TransportQueueDBWriteException = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_TransportQueueDBWriteException_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_TransportQueueDBWriteTreatingTransientAsPermanent_Impl = new ExEventLog.EventTuple(3221489627U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_TransportQueueDBWriteTreatingTransientAsPermanent = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_TransportQueueDBWriteTreatingTransientAsPermanent_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_AdTransientExceptionWhenWriteViaWebServiceDAL_Impl = new ExEventLog.EventTuple(3221489628U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_AdTransientExceptionWhenWriteViaWebServiceDAL = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_AdTransientExceptionWhenWriteViaWebServiceDAL_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_ADTopologyEndpointNotFound_Impl = new ExEventLog.EventTuple(3221489629U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_ADTopologyEndpointNotFound = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_ADTopologyEndpointNotFound_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_DatabaseServerBusyException_Impl = new ExEventLog.EventTuple(3221489630U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_DatabaseServerBusyException = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_DatabaseServerBusyException_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_ServerBusyExceptionWhenWriteViaWebServiceDAL_Impl = new ExEventLog.EventTuple(3221489631U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_ServerBusyExceptionWhenWriteViaWebServiceDAL = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_ServerBusyExceptionWhenWriteViaWebServiceDAL_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_TimeoutExceptionWhenWriteViaWebServiceDAL_Impl = new ExEventLog.EventTuple(3221489632U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_TimeoutExceptionWhenWriteViaWebServiceDAL = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_TimeoutExceptionWhenWriteViaWebServiceDAL_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_CommunicationExceptionWhenWriteViaWebServiceDAL_Impl = new ExEventLog.EventTuple(3221489633U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_CommunicationExceptionWhenWriteViaWebServiceDAL = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_CommunicationExceptionWhenWriteViaWebServiceDAL_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_FaultExceptionWhenWriteViaWebServiceDAL_Impl = new ExEventLog.EventTuple(3221489634U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_FaultExceptionWhenWriteViaWebServiceDAL = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_FaultExceptionWhenWriteViaWebServiceDAL_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_CertificateException_Impl = new ExEventLog.EventTuple(3221489635U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_CertificateException = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_CertificateException_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_LogMonitorRequestedStop_Impl = new ExEventLog.EventTuple(1073744825U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_LogMonitorRequestedStop = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_LogMonitorRequestedStop_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_LogMonitorAllStopped_Impl = new ExEventLog.EventTuple(1073744826U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_LogMonitorAllStopped = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_LogMonitorAllStopped_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_LogMonitorStopTimedOut_Impl = new ExEventLog.EventTuple(1073744827U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_LogMonitorStopTimedOut = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_LogMonitorStopTimedOut_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_LogMonitorWatermarkCleanupFailed_Impl = new ExEventLog.EventTuple(3221228476U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_LogMonitorWatermarkCleanupFailed = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_LogMonitorWatermarkCleanupFailed_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_LogMonitorLogCompleted_Impl = new ExEventLog.EventTuple(1073744829U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_LogMonitorLogCompleted = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_LogMonitorLogCompleted_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_LogMonitorDetectLogProcessingFallsBehind_Impl = new ExEventLog.EventTuple(3221228478U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_LogMonitorDetectLogProcessingFallsBehind = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_LogMonitorDetectLogProcessingFallsBehind_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_LogMonitorDetectNoStaleLog_Impl = new ExEventLog.EventTuple(1073744831U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_LogMonitorDetectNoStaleLog = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_LogMonitorDetectNoStaleLog_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_NonexistentLogDirectory_Impl = new ExEventLog.EventTuple(1073744832U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_NonexistentLogDirectory = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_NonexistentLogDirectory_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_LogDirectoryChanged_Impl = new ExEventLog.EventTuple(1073744833U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_LogDirectoryChanged = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_LogDirectoryChanged_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_LogFileIsDeleted_Impl = new ExEventLog.EventTuple(2147486658U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_LogFileIsDeleted = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_LogFileIsDeleted_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_CheckDirectoryCaughtException_Impl = new ExEventLog.EventTuple(3221228483U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_CheckDirectoryCaughtException = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_CheckDirectoryCaughtException_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_FailedToInstantiateLogFileInfo_Impl = new ExEventLog.EventTuple(2147486660U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_FailedToInstantiateLogFileInfo = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_FailedToInstantiateLogFileInfo_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_FileDeletedWhenCheckingItsCompletion_Impl = new ExEventLog.EventTuple(2147486661U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_FileDeletedWhenCheckingItsCompletion = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_FileDeletedWhenCheckingItsCompletion_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_TransportQueueMachineNotPartOfStampGroup_Impl = new ExEventLog.EventTuple(3221490630U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_TransportQueueMachineNotPartOfStampGroup = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_TransportQueueMachineNotPartOfStampGroup_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_TransportQueueServerInvalidState_Impl = new ExEventLog.EventTuple(3221490631U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_TransportQueueServerInvalidState = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_TransportQueueServerInvalidState_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_LogDisappearedFromKnownLogNameToLogFileMap_Impl = new ExEventLog.EventTuple(2147748808U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_LogDisappearedFromKnownLogNameToLogFileMap = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_LogDisappearedFromKnownLogNameToLogFileMap_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_PendingProcessLogFilesInfo_Impl = new ExEventLog.EventTuple(1074006985U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_PendingProcessLogFilesInfo = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_PendingProcessLogFilesInfo_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_SuppressedBacklogAlertBecauseDBOffline_Impl = new ExEventLog.EventTuple(1074006986U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_SuppressedBacklogAlertBecauseDBOffline = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_SuppressedBacklogAlertBecauseDBOffline_Impl);

		[EventLogPeriod(Period = "LogAlways")]
		private static readonly ExEventLog.EventTuple Tuple_LogFileDeletedFromKnownLogNameToLogFileMap_Impl = new ExEventLog.EventTuple(1074006987U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		public static readonly ExEventLog.EventTuple Tuple_LogFileDeletedFromKnownLogNameToLogFileMap = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_LogFileDeletedFromKnownLogNameToLogFileMap_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_FailToDeleteOldAndProcessedLogFile_Impl = new ExEventLog.EventTuple(2147748812U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_FailToDeleteOldAndProcessedLogFile = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_FailToDeleteOldAndProcessedLogFile_Impl);

		[EventLogPeriod(Period = "LogAlways")]
		private static readonly ExEventLog.EventTuple Tuple_LogReaderUnknownError_Impl = new ExEventLog.EventTuple(3221229474U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		public static readonly ExEventLog.EventTuple Tuple_LogReaderUnknownError = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_LogReaderUnknownError_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_LogReaderFileOpenFailed_Impl = new ExEventLog.EventTuple(3221229475U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_LogReaderFileOpenFailed = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_LogReaderFileOpenFailed_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_LogReaderReadFailed_Impl = new ExEventLog.EventTuple(3221229476U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_LogReaderReadFailed = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_LogReaderReadFailed_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_LogReaderQueueFullException_Impl = new ExEventLog.EventTuple(1073745829U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_LogReaderQueueFullException = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_LogReaderQueueFullException_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_LogReaderQueueFull_Impl = new ExEventLog.EventTuple(2147487654U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_LogReaderQueueFull = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_LogReaderQueueFull_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_LogReaderStartedParsingLog_Impl = new ExEventLog.EventTuple(1073745831U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_LogReaderStartedParsingLog = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_LogReaderStartedParsingLog_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_LogReaderFinishedParsingLog_Impl = new ExEventLog.EventTuple(1073745832U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_LogReaderFinishedParsingLog = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_LogReaderFinishedParsingLog_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_LogReaderParseEmptyLog_Impl = new ExEventLog.EventTuple(2147487657U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_LogReaderParseEmptyLog = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_LogReaderParseEmptyLog_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_LogReaderLogTooBig_Impl = new ExEventLog.EventTuple(3221229482U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_LogReaderLogTooBig = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_LogReaderLogTooBig_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_LogReaderLogMissing_Impl = new ExEventLog.EventTuple(2147487659U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_LogReaderLogMissing = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_LogReaderLogMissing_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_FailedToGetVersionFromLogHeader_Impl = new ExEventLog.EventTuple(2147487660U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_FailedToGetVersionFromLogHeader = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_FailedToGetVersionFromLogHeader_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_ReadLogCaughtIOException_Impl = new ExEventLog.EventTuple(3221229485U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_ReadLogCaughtIOException = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_ReadLogCaughtIOException_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_CsvParserFailedToParseLogLine_Impl = new ExEventLog.EventTuple(3221229487U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_CsvParserFailedToParseLogLine = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_CsvParserFailedToParseLogLine_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_UnsupportedLogVersion_Impl = new ExEventLog.EventTuple(3221229488U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_UnsupportedLogVersion = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_UnsupportedLogVersion_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_LogLineParseError_Impl = new ExEventLog.EventTuple(3221230473U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_LogLineParseError = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_LogLineParseError_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_FailedToParseLatencyData_Impl = new ExEventLog.EventTuple(3221230474U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_FailedToParseLatencyData = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_FailedToParseLatencyData_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_InvalidProperty_Impl = new ExEventLog.EventTuple(2147488652U, 5, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_InvalidProperty = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_InvalidProperty_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_OneDomainHasDifferentTenantIds_Impl = new ExEventLog.EventTuple(2147488653U, 5, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_OneDomainHasDifferentTenantIds = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_OneDomainHasDifferentTenantIds_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_InvalidAgentInfoGroupName_Impl = new ExEventLog.EventTuple(3221230478U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_InvalidAgentInfoGroupName = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_InvalidAgentInfoGroupName_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_InvalidSysprobeLogLine_Impl = new ExEventLog.EventTuple(3221230479U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_InvalidSysprobeLogLine = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_InvalidSysprobeLogLine_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_MissingPropertyInParse_Impl = new ExEventLog.EventTuple(3221230480U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_MissingPropertyInParse = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_MissingPropertyInParse_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_InvalidPropertyValueInParse_Impl = new ExEventLog.EventTuple(3221230481U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_InvalidPropertyValueInParse = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_InvalidPropertyValueInParse_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_InvalidCastInParse_Impl = new ExEventLog.EventTuple(3221230482U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_InvalidCastInParse = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_InvalidCastInParse_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_InvalidPropertyValue_Impl = new ExEventLog.EventTuple(3221230483U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_InvalidPropertyValue = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_InvalidPropertyValue_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_WatermarkFileMapRemoveFailed_Impl = new ExEventLog.EventTuple(3221231473U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_WatermarkFileMapRemoveFailed = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_WatermarkFileMapRemoveFailed_Impl);

		[EventLogPeriod(Period = "LogAlways")]
		private static readonly ExEventLog.EventTuple Tuple_WatermarkFileDuplicateBlock_Impl = new ExEventLog.EventTuple(3221231474U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		public static readonly ExEventLog.EventTuple Tuple_WatermarkFileDuplicateBlock = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_WatermarkFileDuplicateBlock_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_WatermarkFileParseException_Impl = new ExEventLog.EventTuple(3221231475U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_WatermarkFileParseException = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_WatermarkFileParseException_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_OverlappingLogRangeInWatermarkFile_Impl = new ExEventLog.EventTuple(3221231476U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_OverlappingLogRangeInWatermarkFile = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_OverlappingLogRangeInWatermarkFile_Impl);

		[EventLogPeriod(Period = "LogAlways")]
		private static readonly ExEventLog.EventTuple Tuple_DualWriteErrorEvent_Impl = new ExEventLog.EventTuple(3221231477U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		public static readonly ExEventLog.EventTuple Tuple_DualWriteErrorEvent = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_DualWriteErrorEvent_Impl);

		[EventLogPeriod(Period = "LogAlways")]
		private static readonly ExEventLog.EventTuple Tuple_DualWriteWarningEvent_Impl = new ExEventLog.EventTuple(2147489654U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		public static readonly ExEventLog.EventTuple Tuple_DualWriteWarningEvent = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_DualWriteWarningEvent_Impl);

		[EventLogPeriod(Period = "LogAlways")]
		private static readonly ExEventLog.EventTuple Tuple_WatermarkFileOverlappingBlock_Impl = new ExEventLog.EventTuple(3221231479U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		public static readonly ExEventLog.EventTuple Tuple_WatermarkFileOverlappingBlock = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_WatermarkFileOverlappingBlock_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_OpticsLogMonitorTopologyStart_Impl = new ExEventLog.EventTuple(1073747832U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_OpticsLogMonitorTopologyStart = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_OpticsLogMonitorTopologyStart_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_OpticsLogMonitorTopologyFailedToStart_Impl = new ExEventLog.EventTuple(3221231481U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_OpticsLogMonitorTopologyFailedToStart = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_OpticsLogMonitorTopologyFailedToStart_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_OpticsLogMonitorTopologyStop_Impl = new ExEventLog.EventTuple(1073747834U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_OpticsLogMonitorTopologyStop = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_OpticsLogMonitorTopologyStop_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_OpticsLogMonitorTopologyFailedToStop_Impl = new ExEventLog.EventTuple(2147489659U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_OpticsLogMonitorTopologyFailedToStop = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_OpticsLogMonitorTopologyFailedToStop_Impl);

		[EventLogPeriod(Period = "LogAlways")]
		private static readonly ExEventLog.EventTuple Tuple_OpticsWriteExtractionWarningEvent_Impl = new ExEventLog.EventTuple(2147489660U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		public static readonly ExEventLog.EventTuple Tuple_OpticsWriteExtractionWarningEvent = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_OpticsWriteExtractionWarningEvent_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_OpticsDisabled_Impl = new ExEventLog.EventTuple(1073747837U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_OpticsDisabled = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_OpticsDisabled_Impl);

		[EventLogPeriod(Period = "LogOneTime")]
		private static readonly ExEventLog.EventTuple Tuple_OpticsEnabled_Impl = new ExEventLog.EventTuple(1073747838U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		public static readonly ExEventLog.EventTuple Tuple_OpticsEnabled = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_OpticsEnabled_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_WatermarkFileIOException_Impl = new ExEventLog.EventTuple(3221231487U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_WatermarkFileIOException = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_WatermarkFileIOException_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_InactiveFileTurnsToActiveException_Impl = new ExEventLog.EventTuple(3221231488U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_InactiveFileTurnsToActiveException = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_InactiveFileTurnsToActiveException_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_FileDeleted_Impl = new ExEventLog.EventTuple(3221231489U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_FileDeleted = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_FileDeleted_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_WatermarkFileObjectNotFound_Impl = new ExEventLog.EventTuple(1073747842U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_WatermarkFileObjectNotFound = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_WatermarkFileObjectNotFound_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_DeletingFile_Impl = new ExEventLog.EventTuple(1073747843U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_DeletingFile = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_DeletingFile_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_LogBatchEnqueue_Impl = new ExEventLog.EventTuple(1073747844U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_LogBatchEnqueue = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_LogBatchEnqueue_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_ForcePickUnprocessedHoles_Impl = new ExEventLog.EventTuple(2147489669U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_ForcePickUnprocessedHoles = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_ForcePickUnprocessedHoles_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_PartitionIsNotHealthy_Impl = new ExEventLog.EventTuple(2147489670U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_PartitionIsNotHealthy = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_PartitionIsNotHealthy_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_PartitionHealthyVerboseMessage_Impl = new ExEventLog.EventTuple(1073747847U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_PartitionHealthyVerboseMessage = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_PartitionHealthyVerboseMessage_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_WatermarkFileObjectDisposed_Impl = new ExEventLog.EventTuple(3221231496U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_WatermarkFileObjectDisposed = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_WatermarkFileObjectDisposed_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_ConfigFileNotFound_Impl = new ExEventLog.EventTuple(3221231497U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_ConfigFileNotFound = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_ConfigFileNotFound_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_InvalidConfigFile_Impl = new ExEventLog.EventTuple(3221231498U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_InvalidConfigFile = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_InvalidConfigFile_Impl);

		[EventLogPeriod(Period = "LogPeriodic")]
		private static readonly ExEventLog.EventTuple Tuple_InvalidRuleCollection_Impl = new ExEventLog.EventTuple(3221231499U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		public static readonly ExEventLog.EventTuple Tuple_InvalidRuleCollection = new ExEventLog.EventTuple(LogUploaderEventLogConstants.Tuple_InvalidRuleCollection_Impl);

		private enum Category : short
		{
			General = 1,
			DatabaseWriter,
			LogMonitor,
			LogReader,
			Parser,
			WatermarkFile,
			Optics,
			UploaderConfig
		}

		internal enum Message : uint
		{
			Startup = 1073742824U,
			Shutdown,
			ServiceDisabled,
			ServiceEnabled,
			FailToAccessADTemporarily = 3221226476U,
			FailedToInstantiateLogFileInfoFileNotExist,
			ReadConfigFromADSucceeded = 1073742830U,
			ServerNotFoundInAD = 3221226479U,
			LogTypeNotFoundInConfigFile = 3221226481U,
			ParamNotFoundInConfigFile,
			FailedToReadConfigFile = 3221226484U,
			ServiceStartUnknownException,
			FailedToGetConfigValueFromAD,
			ServiceStartedForProcessingLogs = 1073742839U,
			NoLogsToProcess = 3221226488U,
			LogDirIsDisabled = 1073742842U,
			WorkerStartup,
			UnsupportedLogSchemaType = 3221226492U,
			FailedToRemoveMessagesForDomain = 3221226494U,
			ConfigSettingNotFound = 2147484671U,
			InconsistentPersistentStoreCopies = 1074004992U,
			FailedToGetLogPath = 3221488641U,
			MissingWatermark = 3221227472U,
			DatabaseWriterUnknownException = 3221489617U,
			DatabaseWriterPermanentException,
			DatabaseWriterTransientException,
			WritePerItemPermanentException,
			WebServiceWriteException,
			WritePerItemPermanentException2,
			DatabaseWriterTransientException2,
			FailedToGetTenantDomain = 3221489625U,
			TransportQueueDBWriteException,
			TransportQueueDBWriteTreatingTransientAsPermanent,
			AdTransientExceptionWhenWriteViaWebServiceDAL,
			ADTopologyEndpointNotFound,
			DatabaseServerBusyException,
			ServerBusyExceptionWhenWriteViaWebServiceDAL,
			TimeoutExceptionWhenWriteViaWebServiceDAL,
			CommunicationExceptionWhenWriteViaWebServiceDAL,
			FaultExceptionWhenWriteViaWebServiceDAL,
			CertificateException,
			LogMonitorRequestedStop = 1073744825U,
			LogMonitorAllStopped,
			LogMonitorStopTimedOut,
			LogMonitorWatermarkCleanupFailed = 3221228476U,
			LogMonitorLogCompleted = 1073744829U,
			LogMonitorDetectLogProcessingFallsBehind = 3221228478U,
			LogMonitorDetectNoStaleLog = 1073744831U,
			NonexistentLogDirectory,
			LogDirectoryChanged,
			LogFileIsDeleted = 2147486658U,
			CheckDirectoryCaughtException = 3221228483U,
			FailedToInstantiateLogFileInfo = 2147486660U,
			FileDeletedWhenCheckingItsCompletion,
			TransportQueueMachineNotPartOfStampGroup = 3221490630U,
			TransportQueueServerInvalidState,
			LogDisappearedFromKnownLogNameToLogFileMap = 2147748808U,
			PendingProcessLogFilesInfo = 1074006985U,
			SuppressedBacklogAlertBecauseDBOffline,
			LogFileDeletedFromKnownLogNameToLogFileMap,
			FailToDeleteOldAndProcessedLogFile = 2147748812U,
			LogReaderUnknownError = 3221229474U,
			LogReaderFileOpenFailed,
			LogReaderReadFailed,
			LogReaderQueueFullException = 1073745829U,
			LogReaderQueueFull = 2147487654U,
			LogReaderStartedParsingLog = 1073745831U,
			LogReaderFinishedParsingLog,
			LogReaderParseEmptyLog = 2147487657U,
			LogReaderLogTooBig = 3221229482U,
			LogReaderLogMissing = 2147487659U,
			FailedToGetVersionFromLogHeader,
			ReadLogCaughtIOException = 3221229485U,
			CsvParserFailedToParseLogLine = 3221229487U,
			UnsupportedLogVersion,
			LogLineParseError = 3221230473U,
			FailedToParseLatencyData,
			InvalidProperty = 2147488652U,
			OneDomainHasDifferentTenantIds,
			InvalidAgentInfoGroupName = 3221230478U,
			InvalidSysprobeLogLine,
			MissingPropertyInParse,
			InvalidPropertyValueInParse,
			InvalidCastInParse,
			InvalidPropertyValue,
			WatermarkFileMapRemoveFailed = 3221231473U,
			WatermarkFileDuplicateBlock,
			WatermarkFileParseException,
			OverlappingLogRangeInWatermarkFile,
			DualWriteErrorEvent,
			DualWriteWarningEvent = 2147489654U,
			WatermarkFileOverlappingBlock = 3221231479U,
			OpticsLogMonitorTopologyStart = 1073747832U,
			OpticsLogMonitorTopologyFailedToStart = 3221231481U,
			OpticsLogMonitorTopologyStop = 1073747834U,
			OpticsLogMonitorTopologyFailedToStop = 2147489659U,
			OpticsWriteExtractionWarningEvent,
			OpticsDisabled = 1073747837U,
			OpticsEnabled,
			WatermarkFileIOException = 3221231487U,
			InactiveFileTurnsToActiveException,
			FileDeleted,
			WatermarkFileObjectNotFound = 1073747842U,
			DeletingFile,
			LogBatchEnqueue,
			ForcePickUnprocessedHoles = 2147489669U,
			PartitionIsNotHealthy,
			PartitionHealthyVerboseMessage = 1073747847U,
			WatermarkFileObjectDisposed = 3221231496U,
			ConfigFileNotFound,
			InvalidConfigFile,
			InvalidRuleCollection
		}
	}
}
