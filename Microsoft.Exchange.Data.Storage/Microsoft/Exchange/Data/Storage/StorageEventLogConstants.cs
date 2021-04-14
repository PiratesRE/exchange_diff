using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class StorageEventLogConstants
	{
		public const string EventSource = "MSExchange Mid-Tier Storage";

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ExternalAuthDisabledAutoDiscover = new ExEventLog.EventTuple(3221226473U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AutoDiscoverFailedToAquireSecurityToken = new ExEventLog.EventTuple(3221226474U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AutoDiscoverFailed = new ExEventLog.EventTuple(3221226475U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ExternalAuthDisabledExchangePrincipal = new ExEventLog.EventTuple(3221226476U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_XtcInvalidSmtpAddress = new ExEventLog.EventTuple(3221226477U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_XtcOrgRelationshipMissing = new ExEventLog.EventTuple(3221226478U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_XtcArchiveGuidMissing = new ExEventLog.EventTuple(3221226479U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_XtcAutoDiscoverRequestFailed = new ExEventLog.EventTuple(3221226480U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_XtcMapiError = new ExEventLog.EventTuple(3221226481U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ExternalAuthDisabledMailboxSession = new ExEventLog.EventTuple(3221226482U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ExternalArchiveDisabled = new ExEventLog.EventTuple(3221226483U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FederatedMailboxMisconfigured = new ExEventLog.EventTuple(3221226484U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AutoDiscoverFailedForSetting = new ExEventLog.EventTuple(3221226485U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_XtcOrgRelationshipArchiveDisabled = new ExEventLog.EventTuple(3221226486U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_XtcInvalidOrgRelationshipTargetAutodiscoverEpr = new ExEventLog.EventTuple(3221226487U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_XtcInvalidOrgRelationshipTargetApplicationUri = new ExEventLog.EventTuple(3221226488U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorSavingMailboxAudit = new ExEventLog.EventTuple(3221227473U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorSavingLastAccessTime = new ExEventLog.EventTuple(3221227474U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorReadingBypassAudit = new ExEventLog.EventTuple(3221227475U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorResolvingLogonUser = new ExEventLog.EventTuple(3221227476U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorOpeningParticipantSession = new ExEventLog.EventTuple(2147485653U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorBindingMessageItem = new ExEventLog.EventTuple(2147485654U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorBindingFolderForFolderBindHistory = new ExEventLog.EventTuple(2147485655U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorSavingFolderBindHistory = new ExEventLog.EventTuple(2147485656U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorResolvingFromAddress = new ExEventLog.EventTuple(2147485657U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorLoadAuditPolicyConfiguration = new ExEventLog.EventTuple(3221227482U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SearchObjectSaved = new ExEventLog.EventTuple(1074006969U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SearchStatusSaved = new ExEventLog.EventTuple(1074006970U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SearchStatusError = new ExEventLog.EventTuple(3221490619U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoveryAndHoldSaved = new ExEventLog.EventTuple(1074006972U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoverySearchStartRequested = new ExEventLog.EventTuple(1074006973U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoverySearchStopRequested = new ExEventLog.EventTuple(1074006974U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoverySearchRemoveRequested = new ExEventLog.EventTuple(1074006975U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InPlaceHoldSettingsSynchronized = new ExEventLog.EventTuple(1074006976U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SynchronizeInPlaceHoldError = new ExEventLog.EventTuple(3221490625U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoverySearchRequestPickedUp = new ExEventLog.EventTuple(1074006978U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoverySearchStartRequestProcessed = new ExEventLog.EventTuple(1074006979U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoverySearchStopRequestProcessed = new ExEventLog.EventTuple(1074006980U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoverySearchRemoveRequestProcessed = new ExEventLog.EventTuple(1074006981U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoverySearchTaskError = new ExEventLog.EventTuple(3221490630U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoverySearchTaskStarted = new ExEventLog.EventTuple(1074006983U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoverySearchTaskCompleted = new ExEventLog.EventTuple(1074006984U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoverySearchServerError = new ExEventLog.EventTuple(3221490633U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoverySearchStatusChanged = new ExEventLog.EventTuple(1074006986U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToSyncDiscoveryHoldToExchangeOnline = new ExEventLog.EventTuple(3221490635U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SyncDiscoveryHoldToExchangeOnlineStart = new ExEventLog.EventTuple(1074006988U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SyncDiscoveryHoldToExchangeOnlineDetails = new ExEventLog.EventTuple(1074006989U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SingleFailureSyncDiscoveryHoldToExchangeOnline = new ExEventLog.EventTuple(3221490638U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoverySearchWorkItemQueueChanged = new ExEventLog.EventTuple(1074006991U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoverySearchWorkItemQueueNotProcessed = new ExEventLog.EventTuple(1074006992U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoverySearchRpcServerRestarted = new ExEventLog.EventTuple(1074006993U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RecipientDLExpansionIsProcessing = new ExEventLog.EventTuple(1074007069U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RecipientDLExpansionFailed = new ExEventLog.EventTuple(3221490718U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RecipientDLExpansionMessageAlreadyProcessed = new ExEventLog.EventTuple(1074007071U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RecipientDLExpansionMessageNoDLRecipients = new ExEventLog.EventTuple(1074007072U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RecipientDLExpansionMessageNoLongerExist = new ExEventLog.EventTuple(1074007073U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RecipientDLExpansionMismatchResults = new ExEventLog.EventTuple(3221490722U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RecipientDLExpansionMaxNestedDLsLimit = new ExEventLog.EventTuple(3221490723U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RecipientDLExpansionMaxRecipientsLimit = new ExEventLog.EventTuple(3221490724U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RecipientDLExpansionSkipped = new ExEventLog.EventTuple(1074007077U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RecipientDLExpansionUpdateItemInDumpster = new ExEventLog.EventTuple(1074007078U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UnknownTemplateInPublishingLicense = new ExEventLog.EventTuple(3221229472U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorMultipleSaveOperationFailed = new ExEventLog.EventTuple(2147488649U, 5, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorCOWCacheWaitTimeout = new ExEventLog.EventTuple(2147488650U, 5, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_COWCalendarLoggingDisabled = new ExEventLog.EventTuple(2147488651U, 5, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_COWCalendarLoggingStopped = new ExEventLog.EventTuple(2147488652U, 5, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorDatabasePingTimedOut = new ExEventLog.EventTuple(2147489650U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PFRuleConfigGetLocalIPFailure = new ExEventLog.EventTuple(3221494617U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PFRuleSettingFromAddressFailure = new ExEventLog.EventTuple(2147752794U, 7, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PopulatedServiceTopology = new ExEventLog.EventTuple(1073750824U, 9, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorPopulatingServiceTopology = new ExEventLog.EventTuple(3221234473U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RegisteredForTopologyChangedNotification = new ExEventLog.EventTuple(1073750826U, 9, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorRegisteringForTopologyChangedNotification = new ExEventLog.EventTuple(3221234475U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorCreateNotification = new ExEventLog.EventTuple(3221235473U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorUpdateNotification = new ExEventLog.EventTuple(3221235474U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorRemoveNotification = new ExEventLog.EventTuple(3221235475U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorDiscoverEwsUrlForMailbox = new ExEventLog.EventTuple(3221235476U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorSendNotificationEmail = new ExEventLog.EventTuple(3221235477U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorActiveManagerClientADTimeout = new ExEventLog.EventTuple(3221235478U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorActiveManagerClientADError = new ExEventLog.EventTuple(3221235479U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DkmDecryptionFailure = new ExEventLog.EventTuple(3221235480U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RsaCapiKeyImportFailure = new ExEventLog.EventTuple(3221235481U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorCheckReplicationFlushedDatabaseNotFound = new ExEventLog.EventTuple(3221235482U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorCheckReplicationFlushed = new ExEventLog.EventTuple(3221235483U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorCheckReplicationThrottlingDatabaseNotFound = new ExEventLog.EventTuple(3221235484U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorCheckReplicationThrottling = new ExEventLog.EventTuple(3221235485U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ActiveManagerClientAnotherThreadInADCall = new ExEventLog.EventTuple(1073751838U, 10, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ActiveManagerClientAnotherThreadInADCallTimeout = new ExEventLog.EventTuple(2147493663U, 10, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ActiveManagerClientAnotherThreadCompleted = new ExEventLog.EventTuple(1073751840U, 10, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ActiveManagerWCFCleanup = new ExEventLog.EventTuple(1073751841U, 10, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		private enum Category : short
		{
			Xtc = 1,
			Audit,
			Discovery,
			Information_Rights_Management,
			CopyOnWrite,
			ResourceHealth,
			PFRule,
			ServiceDiscovery = 9,
			Provider
		}

		internal enum Message : uint
		{
			ExternalAuthDisabledAutoDiscover = 3221226473U,
			AutoDiscoverFailedToAquireSecurityToken,
			AutoDiscoverFailed,
			ExternalAuthDisabledExchangePrincipal,
			XtcInvalidSmtpAddress,
			XtcOrgRelationshipMissing,
			XtcArchiveGuidMissing,
			XtcAutoDiscoverRequestFailed,
			XtcMapiError,
			ExternalAuthDisabledMailboxSession,
			ExternalArchiveDisabled,
			FederatedMailboxMisconfigured,
			AutoDiscoverFailedForSetting,
			XtcOrgRelationshipArchiveDisabled,
			XtcInvalidOrgRelationshipTargetAutodiscoverEpr,
			XtcInvalidOrgRelationshipTargetApplicationUri,
			ErrorSavingMailboxAudit = 3221227473U,
			ErrorSavingLastAccessTime,
			ErrorReadingBypassAudit,
			ErrorResolvingLogonUser,
			ErrorOpeningParticipantSession = 2147485653U,
			ErrorBindingMessageItem,
			ErrorBindingFolderForFolderBindHistory,
			ErrorSavingFolderBindHistory,
			ErrorResolvingFromAddress,
			ErrorLoadAuditPolicyConfiguration = 3221227482U,
			SearchObjectSaved = 1074006969U,
			SearchStatusSaved,
			SearchStatusError = 3221490619U,
			DiscoveryAndHoldSaved = 1074006972U,
			DiscoverySearchStartRequested,
			DiscoverySearchStopRequested,
			DiscoverySearchRemoveRequested,
			InPlaceHoldSettingsSynchronized,
			SynchronizeInPlaceHoldError = 3221490625U,
			DiscoverySearchRequestPickedUp = 1074006978U,
			DiscoverySearchStartRequestProcessed,
			DiscoverySearchStopRequestProcessed,
			DiscoverySearchRemoveRequestProcessed,
			DiscoverySearchTaskError = 3221490630U,
			DiscoverySearchTaskStarted = 1074006983U,
			DiscoverySearchTaskCompleted,
			DiscoverySearchServerError = 3221490633U,
			DiscoverySearchStatusChanged = 1074006986U,
			FailedToSyncDiscoveryHoldToExchangeOnline = 3221490635U,
			SyncDiscoveryHoldToExchangeOnlineStart = 1074006988U,
			SyncDiscoveryHoldToExchangeOnlineDetails,
			SingleFailureSyncDiscoveryHoldToExchangeOnline = 3221490638U,
			DiscoverySearchWorkItemQueueChanged = 1074006991U,
			DiscoverySearchWorkItemQueueNotProcessed,
			DiscoverySearchRpcServerRestarted,
			RecipientDLExpansionIsProcessing = 1074007069U,
			RecipientDLExpansionFailed = 3221490718U,
			RecipientDLExpansionMessageAlreadyProcessed = 1074007071U,
			RecipientDLExpansionMessageNoDLRecipients,
			RecipientDLExpansionMessageNoLongerExist,
			RecipientDLExpansionMismatchResults = 3221490722U,
			RecipientDLExpansionMaxNestedDLsLimit,
			RecipientDLExpansionMaxRecipientsLimit,
			RecipientDLExpansionSkipped = 1074007077U,
			RecipientDLExpansionUpdateItemInDumpster,
			UnknownTemplateInPublishingLicense = 3221229472U,
			ErrorMultipleSaveOperationFailed = 2147488649U,
			ErrorCOWCacheWaitTimeout,
			COWCalendarLoggingDisabled,
			COWCalendarLoggingStopped,
			ErrorDatabasePingTimedOut = 2147489650U,
			PFRuleConfigGetLocalIPFailure = 3221494617U,
			PFRuleSettingFromAddressFailure = 2147752794U,
			PopulatedServiceTopology = 1073750824U,
			ErrorPopulatingServiceTopology = 3221234473U,
			RegisteredForTopologyChangedNotification = 1073750826U,
			ErrorRegisteringForTopologyChangedNotification = 3221234475U,
			ErrorCreateNotification = 3221235473U,
			ErrorUpdateNotification,
			ErrorRemoveNotification,
			ErrorDiscoverEwsUrlForMailbox,
			ErrorSendNotificationEmail,
			ErrorActiveManagerClientADTimeout,
			ErrorActiveManagerClientADError,
			DkmDecryptionFailure,
			RsaCapiKeyImportFailure,
			ErrorCheckReplicationFlushedDatabaseNotFound,
			ErrorCheckReplicationFlushed,
			ErrorCheckReplicationThrottlingDatabaseNotFound,
			ErrorCheckReplicationThrottling,
			ActiveManagerClientAnotherThreadInADCall = 1073751838U,
			ActiveManagerClientAnotherThreadInADCallTimeout = 2147493663U,
			ActiveManagerClientAnotherThreadCompleted = 1073751840U,
			ActiveManagerWCFCleanup
		}
	}
}
