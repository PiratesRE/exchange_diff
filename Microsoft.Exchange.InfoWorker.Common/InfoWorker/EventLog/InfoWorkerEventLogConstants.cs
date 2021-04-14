using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.EventLog
{
	public static class InfoWorkerEventLogConstants
	{
		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceStarted = new ExEventLog.EventTuple(1074004969U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceStopped = new ExEventLog.EventTuple(1074004970U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceFailedToStart = new ExEventLog.EventTuple(3221488619U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceStarting = new ExEventLog.EventTuple(1074004972U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceStopping = new ExEventLog.EventTuple(1074004973U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceOutOfMemory = new ExEventLog.EventTuple(3221488622U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CreatingAssistant = new ExEventLog.EventTuple(1074004975U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailCreateAssistant = new ExEventLog.EventTuple(3221488624U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DisabledAssistant = new ExEventLog.EventTuple(1074004977U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OOFConfigNotAccessible = new ExEventLog.EventTuple(3221490617U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OOFInvalidScheduleLine = new ExEventLog.EventTuple(3221490618U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OOFTooManyContacts = new ExEventLog.EventTuple(3221490619U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OOFRulesQuotaExceeded = new ExEventLog.EventTuple(3221490620U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OOFHistoryMapiPermanentException = new ExEventLog.EventTuple(3221490621U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OOFUnableToReadScheduleCache = new ExEventLog.EventTuple(3221490622U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AutoDiscoverFailed = new ExEventLog.EventTuple(3221491617U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyWebRequestFailed = new ExEventLog.EventTuple(3221491618U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PublicFolderRequestFailed = new ExEventLog.EventTuple(3221491619U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PublicFolderServerNotFoundForOU = new ExEventLog.EventTuple(3221491620U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LocalForestConfigurationNotFound = new ExEventLog.EventTuple(3221491621U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxLogonFailed = new ExEventLog.EventTuple(3221491625U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WebRequestFailedSecurityChecks = new ExEventLog.EventTuple(3221491626U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidCredentialsForCrossForestProxying = new ExEventLog.EventTuple(3221491628U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CalendarQueryFailed = new ExEventLog.EventTuple(3221491629U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogonAsNetworkServiceFailed = new ExEventLog.EventTuple(3221491632U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NoCASFoundForRequest = new ExEventLog.EventTuple(3221491633U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CASDiscoveryExceptionHandled = new ExEventLog.EventTuple(3221491634U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WorkingHoursFailed = new ExEventLog.EventTuple(3221491635U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidMaximumDatabasesInQuery = new ExEventLog.EventTuple(3221491637U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidMinimumDatabasesInQuery = new ExEventLog.EventTuple(3221491638U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DuplicateAvailabilityAddressSpace = new ExEventLog.EventTuple(3221491639U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CannotGetLocalSiteName = new ExEventLog.EventTuple(3221491640U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SCPCannotConnectToRemoteDirectory = new ExEventLog.EventTuple(3221491641U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SCPErrorSearchingLocalADForSCP = new ExEventLog.EventTuple(3221491642U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SCPErrorSearchingForRemoteSCP = new ExEventLog.EventTuple(3221491643U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SCPMisconfiguredLocalServiceBindings = new ExEventLog.EventTuple(3221491644U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SCPMisconfiguredRemoteServiceBindings = new ExEventLog.EventTuple(3221491645U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MSExchangeSystemAccountRetrieval = new ExEventLog.EventTuple(3221491646U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AAResourceBooked = new ExEventLog.EventTuple(1074011969U, 8, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AAResourceCanceled = new ExEventLog.EventTuple(1074011970U, 8, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CorruptCalendarConfiguration = new ExEventLog.EventTuple(3221495619U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RBAProcessedMeetingMessage = new ExEventLog.EventTuple(1074011972U, 8, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RBAProcessedMeetingCancelation = new ExEventLog.EventTuple(1074011973U, 8, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RBAValidationException = new ExEventLog.EventTuple(3221495622U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RBANeutralCultureEncountered = new ExEventLog.EventTuple(1074011975U, 8, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RBANonUniqueLegacyDN = new ExEventLog.EventTuple(1074011976U, 8, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ProcessingMeetingMessageFailure = new ExEventLog.EventTuple(2147754793U, 9, EventLogEntryType.Warning, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidElcDataInAD = new ExEventLog.EventTuple(3221497617U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidELCFolderChange = new ExEventLog.EventTuple(3221497618U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidServerADObject = new ExEventLog.EventTuple(3221497619U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NullDestinationFolder = new ExEventLog.EventTuple(3221497620U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DestinationFolderSameAsSource = new ExEventLog.EventTuple(3221497621U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToUpdateElcFolder = new ExEventLog.EventTuple(3221497622U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CorruptionInADElcFolders = new ExEventLog.EventTuple(3221497623U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToCreateFolderHierarchy = new ExEventLog.EventTuple(3221497624U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToReadAuditLogArgsFromAD = new ExEventLog.EventTuple(3221497625U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MissingAuditLogPath = new ExEventLog.EventTuple(3221497626U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigureAuditLogFailed = new ExEventLog.EventTuple(3221497627U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AppendAuditLogFailed = new ExEventLog.EventTuple(3221497628U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ELCRootNameClash = new ExEventLog.EventTuple(3221497630U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CycleInPolicies = new ExEventLog.EventTuple(3221497631U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidRetentionAgeLimit = new ExEventLog.EventTuple(3221497632U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CurrentFolderOnUnhandledException = new ExEventLog.EventTuple(3221497633U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidTagDataInAD = new ExEventLog.EventTuple(3221497634U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IncompleteRPTUpgrade = new ExEventLog.EventTuple(3221497635U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CorruptionInADElcTags = new ExEventLog.EventTuple(3221497636U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MRMSkippingMailbox = new ExEventLog.EventTuple(3221497637U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ArchiveOverWarningQuota = new ExEventLog.EventTuple(2147755814U, 10, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DumpsterOverQuotaDeletedMails = new ExEventLog.EventTuple(2147755815U, 10, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MRMSkippingFolder = new ExEventLog.EventTuple(3221497640U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ExpirationOfCurrentBatchFailed = new ExEventLog.EventTuple(2147755817U, 10, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FoldersWithOversizedItems = new ExEventLog.EventTuple(2147755818U, 10, EventLogEntryType.Warning, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MRMExpirationStatistics = new ExEventLog.EventTuple(1074013995U, 10, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AdminAuditsQuotaWarning = new ExEventLog.EventTuple(2147755820U, 10, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoveryHoldTransientErrorSkipMailbox = new ExEventLog.EventTuple(3221497645U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoveryHoldPermanentErrorSkipMailbox = new ExEventLog.EventTuple(3221497646U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoveryHoldSearchFailed = new ExEventLog.EventTuple(3221497647U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ExpirationOfMsgsInDiscoveryHoldsFolderFailed = new ExEventLog.EventTuple(2147755824U, 10, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoveryHoldsSkippedForTooManyQueries = new ExEventLog.EventTuple(1074014001U, 10, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CorruptDiscoverySearchObject = new ExEventLog.EventTuple(3221497650U, 16, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CorruptDiscoverySearchObjectId = new ExEventLog.EventTuple(3221497651U, 16, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoverySearchObjectNotFound = new ExEventLog.EventTuple(3221497652U, 16, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoverySearchObjectLoadError = new ExEventLog.EventTuple(3221497653U, 16, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoverySearchObjectNotFoundForOrg = new ExEventLog.EventTuple(3221497654U, 16, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToCopyDiscoverySearchToArchive = new ExEventLog.EventTuple(3221497655U, 16, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CorruptDiscoverySearchObjectProperty = new ExEventLog.EventTuple(3221497656U, 16, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoverySearchObjectLoadErrorForMailbox = new ExEventLog.EventTuple(3221497657U, 16, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DumpsterOverQuotaDeletedAuditLogs = new ExEventLog.EventTuple(2147755834U, 10, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EhaMailboxQuotaWarning = new ExEventLog.EventTuple(2147755835U, 10, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToCopyLitigationHoldDurationToArchive = new ExEventLog.EventTuple(3221497660U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToReadLitigationHoldDurationFromPrimaryMailbox = new ExEventLog.EventTuple(3221497661U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ELCFailedToLoadProcessEhaMigrationMessageSetting = new ExEventLog.EventTuple(1074014014U, 10, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToCopyEhaMigrationFlagToArchive = new ExEventLog.EventTuple(3221497663U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HoldCleanupStatistics = new ExEventLog.EventTuple(1074014016U, 10, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoveryFailedToFetchSizeInformation = new ExEventLog.EventTuple(3221497665U, 26, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoveryServerLocatorTimeout = new ExEventLog.EventTuple(3221497666U, 26, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoveryFanoutError = new ExEventLog.EventTuple(3221497667U, 26, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoveryAutodiscoverError = new ExEventLog.EventTuple(3221497668U, 26, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoveryFailedToGetOWAUrl = new ExEventLog.EventTuple(2147755858U, 26, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoveryFailedToGetOWAService = new ExEventLog.EventTuple(2147755852U, 16, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoveryFailedToGetOWAServiceWithException = new ExEventLog.EventTuple(2147755853U, 16, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoverySearchCIFailure = new ExEventLog.EventTuple(3221497678U, 26, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoverySearchFailure = new ExEventLog.EventTuple(3221497679U, 26, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoveryMailboxServerLocatorTime = new ExEventLog.EventTuple(1074014032U, 26, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ELCSkipProcessingMailboxTransientException = new ExEventLog.EventTuple(3221497681U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ELCSkipProcessingMailbox = new ExEventLog.EventTuple(1074014068U, 10, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToUpdateSafeLists = new ExEventLog.EventTuple(2147756792U, 11, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToEnsureJunkEmailRule = new ExEventLog.EventTuple(2147756798U, 11, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToUpdateMailbox = new ExEventLog.EventTuple(3221498624U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToUpdateConversationOnDelete = new ExEventLog.EventTuple(2147757793U, 12, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToUpdateConversationOnQuotaExceeded = new ExEventLog.EventTuple(2147757794U, 12, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToUpdateConversationOnFolderDelete = new ExEventLog.EventTuple(2147757795U, 12, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CorruptConversationActionItem = new ExEventLog.EventTuple(3221499620U, 12, EventLogEntryType.Error, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TotalNumberOfItemsForBodyTagProcessing = new ExEventLog.EventTuple(1074015973U, 12, EventLogEntryType.Information, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_BodyTagProcessingFailed = new ExEventLog.EventTuple(3221499622U, 12, EventLogEntryType.Error, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_BodyTagProcessingSucceeded = new ExEventLog.EventTuple(1074015975U, 12, EventLogEntryType.Information, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_BodyTagProcessingSkipped = new ExEventLog.EventTuple(1074015976U, 12, EventLogEntryType.Information, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_BodyTagProcessingRequestQueued = new ExEventLog.EventTuple(1074015977U, 12, EventLogEntryType.Information, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DeliveryToArbitrationMailboxExceededRateLimits = new ExEventLog.EventTuple(2147758793U, 13, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToReadConfigurationFromAD = new ExEventLog.EventTuple(3221501617U, 14, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToCreateGroupMetricsDirectory = new ExEventLog.EventTuple(3221501618U, 14, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToGetDomainList = new ExEventLog.EventTuple(3221501624U, 14, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToGetListOfChangedGroupsForDomain = new ExEventLog.EventTuple(3221501626U, 14, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToReadGroupMetricsCookie = new ExEventLog.EventTuple(2147759804U, 14, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToDeserializeGroupMetricsCookie = new ExEventLog.EventTuple(2147759805U, 14, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToRemoveCorruptGroupMetricsCookie = new ExEventLog.EventTuple(3221501630U, 14, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToSaveGroupMetricsCookie = new ExEventLog.EventTuple(3221501631U, 14, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToOpenFile = new ExEventLog.EventTuple(3221501632U, 14, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GroupMetricsCookieExpired = new ExEventLog.EventTuple(3221501633U, 14, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToReadChangedGroupList = new ExEventLog.EventTuple(3221501635U, 14, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GroupExpansionHaltedWarning = new ExEventLog.EventTuple(2147759812U, 14, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GroupExpansionHaltedError = new ExEventLog.EventTuple(3221501637U, 14, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToRegisterChangeNotification = new ExEventLog.EventTuple(2147759817U, 14, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToFindPublicFolderServer = new ExEventLog.EventTuple(3221501645U, 15, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToConnectToAnyPublicFolderServer = new ExEventLog.EventTuple(3221501646U, 15, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToFindFreeBusyPublicFolder = new ExEventLog.EventTuple(3221501647U, 15, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToSaveGroupMetricsToAD = new ExEventLog.EventTuple(1074018000U, 14, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GroupMetricsGenerationFailed = new ExEventLog.EventTuple(3221501649U, 14, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GroupMetricsGenerationSuccessful = new ExEventLog.EventTuple(1074018002U, 14, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MailTipsMailboxQueryFailed = new ExEventLog.EventTuple(3221501651U, 14, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GroupMetricsGenerationStarted = new ExEventLog.EventTuple(1074018004U, 14, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToWriteFile = new ExEventLog.EventTuple(3221501653U, 14, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToRemoveDirectory = new ExEventLog.EventTuple(3221501654U, 14, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GroupMetricsGenerationSkippedNoADFile = new ExEventLog.EventTuple(3221501655U, 14, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GroupMetricsGenerationCouldntFindSystemMailbox = new ExEventLog.EventTuple(3221501656U, 14, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToAccessOrganizationMailbox = new ExEventLog.EventTuple(3221501657U, 14, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UploadGroupMetricsCookieFailed = new ExEventLog.EventTuple(2147759834U, 14, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DownloadGroupMetricsCookieFailed = new ExEventLog.EventTuple(2147759835U, 14, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DeleteGroupMetricsCookieFailed = new ExEventLog.EventTuple(2147759836U, 14, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CorruptTagConfigItem = new ExEventLog.EventTuple(2147761793U, 16, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OABGenerationStartGeneration = new ExEventLog.EventTuple(279145U, 17, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OABGenerationCompletedGeneration = new ExEventLog.EventTuple(279146U, 17, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OABGenerationCouldntFindSystemMailbox = new ExEventLog.EventTuple(3221504619U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OABGenerationFailureException = new ExEventLog.EventTuple(3221504620U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CannotFindOAB = new ExEventLog.EventTuple(3221504621U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FatalOABFindException = new ExEventLog.EventTuple(3221504622U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OrganizationalMailboxGuidIsNotUnique = new ExEventLog.EventTuple(3221504623U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OABGenerationFailureExceptionNoSpecificOAB = new ExEventLog.EventTuple(3221504624U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OABNotProcessedBecauseAddressListIsInvalid = new ExEventLog.EventTuple(3221504625U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CorruptOutlookProtectionRule = new ExEventLog.EventTuple(3221506617U, 19, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CorruptPolicyNudgeRule = new ExEventLog.EventTuple(3221506618U, 21, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ProvisioningAssitantProvisionedMailbox = new ExEventLog.EventTuple(1074023970U, 20, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ProvisioningAssistantFailedToProvisionMailbox = new ExEventLog.EventTuple(3221507619U, 20, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CorruptClassificationDefinition = new ExEventLog.EventTuple(3221508617U, 22, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToUpdateSubscriptionOnMailboxTable = new ExEventLog.EventTuple(3221509617U, 23, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorProcessingHandleEvent = new ExEventLog.EventTuple(3221509618U, 23, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToResolveInboxFolderId = new ExEventLog.EventTuple(2147767795U, 23, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorAccessingRemoteMailbox = new ExEventLog.EventTuple(3221511617U, 24, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ProcessingStatisticsForPeopleRelevanceFeeder = new ExEventLog.EventTuple(287145U, 25, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		private enum Category : short
		{
			Service = 1,
			OOF_Assistant,
			OOF_Library,
			Availability_Service,
			Availability_Service_Configuration,
			Availability_Service_Authentication,
			Availability_Service_Authorization,
			Resource_Booking_Attendant,
			Calendar_Attendant,
			Managed_Folder_Assistant,
			Junk_Email_Options_Assistant,
			Conversations_Assistant,
			Approval_Assistant,
			MailTips,
			FreeBusy_Assistant,
			ELC_library,
			OAB_Generator_Assistant,
			TopN_Words_Assistant,
			Outlook_Protection_Rules,
			Provisioning_Assistant_General,
			Policy_Nudge_Rules,
			Classification_Definitions,
			Push_Notification_Assistant,
			Calendar_Repair_Assistant,
			People_Relevance_Assistant,
			Discovery_Search,
			Reminders_Assistant,
			Calendar_Interop_Assistant
		}

		internal enum Message : uint
		{
			ServiceStarted = 1074004969U,
			ServiceStopped,
			ServiceFailedToStart = 3221488619U,
			ServiceStarting = 1074004972U,
			ServiceStopping,
			ServiceOutOfMemory = 3221488622U,
			CreatingAssistant = 1074004975U,
			FailCreateAssistant = 3221488624U,
			DisabledAssistant = 1074004977U,
			OOFConfigNotAccessible = 3221490617U,
			OOFInvalidScheduleLine,
			OOFTooManyContacts,
			OOFRulesQuotaExceeded,
			OOFHistoryMapiPermanentException,
			OOFUnableToReadScheduleCache,
			AutoDiscoverFailed = 3221491617U,
			ProxyWebRequestFailed,
			PublicFolderRequestFailed,
			PublicFolderServerNotFoundForOU,
			LocalForestConfigurationNotFound,
			MailboxLogonFailed = 3221491625U,
			WebRequestFailedSecurityChecks,
			InvalidCredentialsForCrossForestProxying = 3221491628U,
			CalendarQueryFailed,
			LogonAsNetworkServiceFailed = 3221491632U,
			NoCASFoundForRequest,
			CASDiscoveryExceptionHandled,
			WorkingHoursFailed,
			InvalidMaximumDatabasesInQuery = 3221491637U,
			InvalidMinimumDatabasesInQuery,
			DuplicateAvailabilityAddressSpace,
			CannotGetLocalSiteName,
			SCPCannotConnectToRemoteDirectory,
			SCPErrorSearchingLocalADForSCP,
			SCPErrorSearchingForRemoteSCP,
			SCPMisconfiguredLocalServiceBindings,
			SCPMisconfiguredRemoteServiceBindings,
			MSExchangeSystemAccountRetrieval,
			AAResourceBooked = 1074011969U,
			AAResourceCanceled,
			CorruptCalendarConfiguration = 3221495619U,
			RBAProcessedMeetingMessage = 1074011972U,
			RBAProcessedMeetingCancelation,
			RBAValidationException = 3221495622U,
			RBANeutralCultureEncountered = 1074011975U,
			RBANonUniqueLegacyDN,
			ProcessingMeetingMessageFailure = 2147754793U,
			InvalidElcDataInAD = 3221497617U,
			InvalidELCFolderChange,
			InvalidServerADObject,
			NullDestinationFolder,
			DestinationFolderSameAsSource,
			UnableToUpdateElcFolder,
			CorruptionInADElcFolders,
			FailedToCreateFolderHierarchy,
			FailedToReadAuditLogArgsFromAD,
			MissingAuditLogPath,
			ConfigureAuditLogFailed,
			AppendAuditLogFailed,
			ELCRootNameClash = 3221497630U,
			CycleInPolicies,
			InvalidRetentionAgeLimit,
			CurrentFolderOnUnhandledException,
			InvalidTagDataInAD,
			IncompleteRPTUpgrade,
			CorruptionInADElcTags,
			MRMSkippingMailbox,
			ArchiveOverWarningQuota = 2147755814U,
			DumpsterOverQuotaDeletedMails,
			MRMSkippingFolder = 3221497640U,
			ExpirationOfCurrentBatchFailed = 2147755817U,
			FoldersWithOversizedItems,
			MRMExpirationStatistics = 1074013995U,
			AdminAuditsQuotaWarning = 2147755820U,
			DiscoveryHoldTransientErrorSkipMailbox = 3221497645U,
			DiscoveryHoldPermanentErrorSkipMailbox,
			DiscoveryHoldSearchFailed,
			ExpirationOfMsgsInDiscoveryHoldsFolderFailed = 2147755824U,
			DiscoveryHoldsSkippedForTooManyQueries = 1074014001U,
			CorruptDiscoverySearchObject = 3221497650U,
			CorruptDiscoverySearchObjectId,
			DiscoverySearchObjectNotFound,
			DiscoverySearchObjectLoadError,
			DiscoverySearchObjectNotFoundForOrg,
			FailedToCopyDiscoverySearchToArchive,
			CorruptDiscoverySearchObjectProperty,
			DiscoverySearchObjectLoadErrorForMailbox,
			DumpsterOverQuotaDeletedAuditLogs = 2147755834U,
			EhaMailboxQuotaWarning,
			FailedToCopyLitigationHoldDurationToArchive = 3221497660U,
			FailedToReadLitigationHoldDurationFromPrimaryMailbox,
			ELCFailedToLoadProcessEhaMigrationMessageSetting = 1074014014U,
			FailedToCopyEhaMigrationFlagToArchive = 3221497663U,
			HoldCleanupStatistics = 1074014016U,
			DiscoveryFailedToFetchSizeInformation = 3221497665U,
			DiscoveryServerLocatorTimeout,
			DiscoveryFanoutError,
			DiscoveryAutodiscoverError,
			DiscoveryFailedToGetOWAUrl = 2147755858U,
			DiscoveryFailedToGetOWAService = 2147755852U,
			DiscoveryFailedToGetOWAServiceWithException,
			DiscoverySearchCIFailure = 3221497678U,
			DiscoverySearchFailure,
			DiscoveryMailboxServerLocatorTime = 1074014032U,
			ELCSkipProcessingMailboxTransientException = 3221497681U,
			ELCSkipProcessingMailbox = 1074014068U,
			FailedToUpdateSafeLists = 2147756792U,
			FailedToEnsureJunkEmailRule = 2147756798U,
			FailedToUpdateMailbox = 3221498624U,
			FailedToUpdateConversationOnDelete = 2147757793U,
			FailedToUpdateConversationOnQuotaExceeded,
			FailedToUpdateConversationOnFolderDelete,
			CorruptConversationActionItem = 3221499620U,
			TotalNumberOfItemsForBodyTagProcessing = 1074015973U,
			BodyTagProcessingFailed = 3221499622U,
			BodyTagProcessingSucceeded = 1074015975U,
			BodyTagProcessingSkipped,
			BodyTagProcessingRequestQueued,
			DeliveryToArbitrationMailboxExceededRateLimits = 2147758793U,
			UnableToReadConfigurationFromAD = 3221501617U,
			UnableToCreateGroupMetricsDirectory,
			UnableToGetDomainList = 3221501624U,
			UnableToGetListOfChangedGroupsForDomain = 3221501626U,
			UnableToReadGroupMetricsCookie = 2147759804U,
			UnableToDeserializeGroupMetricsCookie,
			UnableToRemoveCorruptGroupMetricsCookie = 3221501630U,
			UnableToSaveGroupMetricsCookie,
			UnableToOpenFile,
			GroupMetricsCookieExpired,
			UnableToReadChangedGroupList = 3221501635U,
			GroupExpansionHaltedWarning = 2147759812U,
			GroupExpansionHaltedError = 3221501637U,
			UnableToRegisterChangeNotification = 2147759817U,
			UnableToFindPublicFolderServer = 3221501645U,
			UnableToConnectToAnyPublicFolderServer,
			UnableToFindFreeBusyPublicFolder,
			UnableToSaveGroupMetricsToAD = 1074018000U,
			GroupMetricsGenerationFailed = 3221501649U,
			GroupMetricsGenerationSuccessful = 1074018002U,
			MailTipsMailboxQueryFailed = 3221501651U,
			GroupMetricsGenerationStarted = 1074018004U,
			UnableToWriteFile = 3221501653U,
			UnableToRemoveDirectory,
			GroupMetricsGenerationSkippedNoADFile,
			GroupMetricsGenerationCouldntFindSystemMailbox,
			UnableToAccessOrganizationMailbox,
			UploadGroupMetricsCookieFailed = 2147759834U,
			DownloadGroupMetricsCookieFailed,
			DeleteGroupMetricsCookieFailed,
			CorruptTagConfigItem = 2147761793U,
			OABGenerationStartGeneration = 279145U,
			OABGenerationCompletedGeneration,
			OABGenerationCouldntFindSystemMailbox = 3221504619U,
			OABGenerationFailureException,
			CannotFindOAB,
			FatalOABFindException,
			OrganizationalMailboxGuidIsNotUnique,
			OABGenerationFailureExceptionNoSpecificOAB,
			OABNotProcessedBecauseAddressListIsInvalid,
			CorruptOutlookProtectionRule = 3221506617U,
			CorruptPolicyNudgeRule,
			ProvisioningAssitantProvisionedMailbox = 1074023970U,
			ProvisioningAssistantFailedToProvisionMailbox = 3221507619U,
			CorruptClassificationDefinition = 3221508617U,
			FailedToUpdateSubscriptionOnMailboxTable = 3221509617U,
			ErrorProcessingHandleEvent,
			FailedToResolveInboxFolderId = 2147767795U,
			ErrorAccessingRemoteMailbox = 3221511617U,
			ProcessingStatisticsForPeopleRelevanceFeeder = 287145U
		}
	}
}
