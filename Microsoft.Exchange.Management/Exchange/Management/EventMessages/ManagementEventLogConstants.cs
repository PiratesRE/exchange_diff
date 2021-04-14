using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.EventMessages
{
	internal static class ManagementEventLogConstants
	{
		public const string EventSource = "MSExchange Management Application";

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RecipientsUpdateForAddressBookCancelled = new ExEventLog.EventTuple(263144U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RecipientsUpdateForEmailAddressPolicyCancelled = new ExEventLog.EventTuple(263145U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DoNotApplyAddressBook = new ExEventLog.EventTuple(264144U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ApplyAddressBookCancelled = new ExEventLog.EventTuple(264145U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DoNotApplyEmailAddressPolicy = new ExEventLog.EventTuple(264146U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ApplyEmailAddressPolicyCancelled = new ExEventLog.EventTuple(264147U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TransportServerCmdletsDeprecated = new ExEventLog.EventTuple(264148U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UpdateProxyGenerationDllFailed = new ExEventLog.EventTuple(265144U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GenerateProxyAddressFailed = new ExEventLog.EventTuple(265145U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CopyProxyGeneratinDllFailed = new ExEventLog.EventTuple(265146U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LoadBalancingFailedToFindDatabase = new ExEventLog.EventTuple(3221490619U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ScriptExecutionSuccessfully = new ExEventLog.EventTuple(4000U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ScriptExecutionFailed = new ExEventLog.EventTuple(3221229473U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ComponentTaskExecutedSuccessfully = new ExEventLog.EventTuple(4002U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ComponentTaskFailed = new ExEventLog.EventTuple(3221229475U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ExecuteTaskScriptOptic = new ExEventLog.EventTuple(4005U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToLog = new ExEventLog.EventTuple(3221230472U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToInitEwsMailer = new ExEventLog.EventTuple(3221230473U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NoAdminAuditLogConfig = new ExEventLog.EventTuple(2147488650U, 5, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AdminLogFull = new ExEventLog.EventTuple(3221230475U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_MultipleAdminAuditLogConfig = new ExEventLog.EventTuple(3221230476U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToConnectToWLCD = new ExEventLog.EventTuple(3221231472U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToResolveWLCDHost = new ExEventLog.EventTuple(3221231473U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WLCDRefusingConnection = new ExEventLog.EventTuple(3221231474U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnknownErrorCommunicatingWithWLCD = new ExEventLog.EventTuple(3221231475U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DomainInvalidStateWithWLCD = new ExEventLog.EventTuple(6004U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToAddToUSG = new ExEventLog.EventTuple(7001U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToRemoveFromUSG = new ExEventLog.EventTuple(7002U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToFindUSG = new ExEventLog.EventTuple(7003U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SupportedToolsInformationFileMissing = new ExEventLog.EventTuple(3221233473U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SupportedToolsInformationDataFileCorupted = new ExEventLog.EventTuple(3221233474U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SupportedToolsInformationDataFileInconsistent = new ExEventLog.EventTuple(3221233475U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MigrationServiceConnectionError = new ExEventLog.EventTuple(3221234473U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NewMailboxAttempts = new ExEventLog.EventTuple(3221235472U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NewMailboxIterationAttempts = new ExEventLog.EventTuple(3221235473U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NewOrganizationAttempts = new ExEventLog.EventTuple(3221235474U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NewOrganizationIterationAttempts = new ExEventLog.EventTuple(3221235475U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RemoveOrganizationAttempts = new ExEventLog.EventTuple(3221235476U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RemoveOrganizationIterationAttempts = new ExEventLog.EventTuple(3221235477U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AddSecondaryDomainAttempts = new ExEventLog.EventTuple(3221235478U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AddSecondaryDomainIterationAttempts = new ExEventLog.EventTuple(3221235479U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RemoveSecondaryDomainAttempts = new ExEventLog.EventTuple(3221235480U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RemoveSecondaryDomainIterationAttempts = new ExEventLog.EventTuple(3221235481U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GetManagementEndpointAttempts = new ExEventLog.EventTuple(3221235482U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GetManagementEndpointIterationAttempts = new ExEventLog.EventTuple(3221235483U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CmdletAttempts = new ExEventLog.EventTuple(3221235484U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CmdletIterationAttempts = new ExEventLog.EventTuple(3221235485U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TenantMonitoringSuccess = new ExEventLog.EventTuple(10014U, 10, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidStatusOrganizationFailure = new ExEventLog.EventTuple(2147493663U, 10, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidStatusOrganizationSuccess = new ExEventLog.EventTuple(10016U, 10, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InstanceAboveThreshold = new ExEventLog.EventTuple(10017U, 10, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IgnoringInstanceData = new ExEventLog.EventTuple(2147493666U, 10, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidMerveEntriesError = new ExEventLog.EventTuple(2147493668U, 10, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidMerveEntriesSuccess = new ExEventLog.EventTuple(10021U, 10, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailToRetrieveErrorDetails = new ExEventLog.EventTuple(2147493667U, 10, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ProvisioningReconciliationFailure = new ExEventLog.EventTuple(3221240473U, 15, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FswChangedToPrimary = new ExEventLog.EventTuple(273145U, 11, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FswChangedToAlternate = new ExEventLog.EventTuple(273146U, 11, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_BackSyncTooManyObjectReadRestarts = new ExEventLog.EventTuple(2147495649U, 12, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_BackSyncExcludeFromBackSync = new ExEventLog.EventTuple(3221237474U, 12, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_BackSyncFullSyncFailbackDetected = new ExEventLog.EventTuple(3221237475U, 12, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_BackSyncExceptionCaught = new ExEventLog.EventTuple(3221237476U, 12, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RequiredServiceNotRunning = new ExEventLog.EventTuple(3221238473U, 13, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AppPoolNotRunning = new ExEventLog.EventTuple(3221238474U, 13, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TransientErrorCacheInsertEntry = new ExEventLog.EventTuple(2147496651U, 13, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TransientErrorCacheRemoveEntry = new ExEventLog.EventTuple(2147496652U, 13, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TransientErrorCacheFindEntry = new ExEventLog.EventTuple(2147496653U, 13, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RollAsaPwdStarting = new ExEventLog.EventTuple(1073755825U, 14, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RollAsaPwdFinishedSuccess = new ExEventLog.EventTuple(14002U, 14, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RollAsaPwdFinishedWithWarning = new ExEventLog.EventTuple(14003U, 14, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RollAsaPwdFinishedFailure = new ExEventLog.EventTuple(3221239476U, 14, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UpgradeOrchestratorSucceeded = new ExEventLog.EventTuple(16001U, 16, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UpgradeOrchestratorFailed = new ExEventLog.EventTuple(3221241474U, 16, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DataMartConnectionFailed = new ExEventLog.EventTuple(3221242473U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DataMartConfigurationError = new ExEventLog.EventTuple(3221242474U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DkmProvisioningException = new ExEventLog.EventTuple(3221242475U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ImportTpdFailure = new ExEventLog.EventTuple(3221242476U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DkmProvisioningSuccessful = new ExEventLog.EventTuple(17005U, 17, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SkipHSMEncryptedTpd = new ExEventLog.EventTuple(2147500654U, 17, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TenantConfigurationIsNull = new ExEventLog.EventTuple(2147500655U, 17, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DataMartConnectionFailOverToBackupServer = new ExEventLog.EventTuple(2147500656U, 17, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CorruptClassificationRuleCollection = new ExEventLog.EventTuple(2147763793U, 18, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DuplicateDataClassificationIdAcrossRulePack = new ExEventLog.EventTuple(2147763794U, 18, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ClassificationEngineFailure = new ExEventLog.EventTuple(3221505619U, 18, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ClassificationEngineTimeout = new ExEventLog.EventTuple(3221505620U, 18, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ClassificationEngineConfigurationError = new ExEventLog.EventTuple(2147763797U, 18, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FfoReportingTaskFailure = new ExEventLog.EventTuple(3221506617U, 19, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FfoReportingRecipientTaskFailure = new ExEventLog.EventTuple(3221506618U, 19, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToUploadPhoto = new ExEventLog.EventTuple(3221507617U, 20, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToRetrievePhoto = new ExEventLog.EventTuple(3221507618U, 20, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToRemovePhoto = new ExEventLog.EventTuple(3221507619U, 20, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReducedDynamicRangeSuccess = new ExEventLog.EventTuple(1074014971U, 11, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReducedDynamicRangeFailure = new ExEventLog.EventTuple(3221498620U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToGetCurrentDynamicRange = new ExEventLog.EventTuple(3221498621U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CurrentDynamicRange = new ExEventLog.EventTuple(1074014974U, 11, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		private enum Category : short
		{
			Shell = 1,
			Console,
			ProvisioningAgent,
			ComponentInfoBasedTask,
			AdminAuditLog,
			DatacenterProvisioningAgent,
			MailboxTaskHelper,
			SupportedToolsInformation,
			Migration,
			TenantMonitoring,
			DatabaseAvailabilityGroupManagement,
			BackSync,
			MonitoringTask,
			Kerberos,
			ProvisioningReconciliation,
			UpgradeOrchestrator,
			Reporting,
			Dlp,
			FfoReporting,
			Photos
		}

		internal enum Message : uint
		{
			RecipientsUpdateForAddressBookCancelled = 263144U,
			RecipientsUpdateForEmailAddressPolicyCancelled,
			DoNotApplyAddressBook = 264144U,
			ApplyAddressBookCancelled,
			DoNotApplyEmailAddressPolicy,
			ApplyEmailAddressPolicyCancelled,
			TransportServerCmdletsDeprecated,
			UpdateProxyGenerationDllFailed = 265144U,
			GenerateProxyAddressFailed,
			CopyProxyGeneratinDllFailed,
			LoadBalancingFailedToFindDatabase = 3221490619U,
			ScriptExecutionSuccessfully = 4000U,
			ScriptExecutionFailed = 3221229473U,
			ComponentTaskExecutedSuccessfully = 4002U,
			ComponentTaskFailed = 3221229475U,
			ExecuteTaskScriptOptic = 4005U,
			FailedToLog = 3221230472U,
			FailedToInitEwsMailer,
			NoAdminAuditLogConfig = 2147488650U,
			AdminLogFull = 3221230475U,
			MultipleAdminAuditLogConfig,
			FailedToConnectToWLCD = 3221231472U,
			FailedToResolveWLCDHost,
			WLCDRefusingConnection,
			UnknownErrorCommunicatingWithWLCD,
			DomainInvalidStateWithWLCD = 6004U,
			FailedToAddToUSG = 7001U,
			FailedToRemoveFromUSG,
			FailedToFindUSG,
			SupportedToolsInformationFileMissing = 3221233473U,
			SupportedToolsInformationDataFileCorupted,
			SupportedToolsInformationDataFileInconsistent,
			MigrationServiceConnectionError = 3221234473U,
			NewMailboxAttempts = 3221235472U,
			NewMailboxIterationAttempts,
			NewOrganizationAttempts,
			NewOrganizationIterationAttempts,
			RemoveOrganizationAttempts,
			RemoveOrganizationIterationAttempts,
			AddSecondaryDomainAttempts,
			AddSecondaryDomainIterationAttempts,
			RemoveSecondaryDomainAttempts,
			RemoveSecondaryDomainIterationAttempts,
			GetManagementEndpointAttempts,
			GetManagementEndpointIterationAttempts,
			CmdletAttempts,
			CmdletIterationAttempts,
			TenantMonitoringSuccess = 10014U,
			InvalidStatusOrganizationFailure = 2147493663U,
			InvalidStatusOrganizationSuccess = 10016U,
			InstanceAboveThreshold,
			IgnoringInstanceData = 2147493666U,
			InvalidMerveEntriesError = 2147493668U,
			InvalidMerveEntriesSuccess = 10021U,
			FailToRetrieveErrorDetails = 2147493667U,
			ProvisioningReconciliationFailure = 3221240473U,
			FswChangedToPrimary = 273145U,
			FswChangedToAlternate,
			BackSyncTooManyObjectReadRestarts = 2147495649U,
			BackSyncExcludeFromBackSync = 3221237474U,
			BackSyncFullSyncFailbackDetected,
			BackSyncExceptionCaught,
			RequiredServiceNotRunning = 3221238473U,
			AppPoolNotRunning,
			TransientErrorCacheInsertEntry = 2147496651U,
			TransientErrorCacheRemoveEntry,
			TransientErrorCacheFindEntry,
			RollAsaPwdStarting = 1073755825U,
			RollAsaPwdFinishedSuccess = 14002U,
			RollAsaPwdFinishedWithWarning,
			RollAsaPwdFinishedFailure = 3221239476U,
			UpgradeOrchestratorSucceeded = 16001U,
			UpgradeOrchestratorFailed = 3221241474U,
			DataMartConnectionFailed = 3221242473U,
			DataMartConfigurationError,
			DkmProvisioningException,
			ImportTpdFailure,
			DkmProvisioningSuccessful = 17005U,
			SkipHSMEncryptedTpd = 2147500654U,
			TenantConfigurationIsNull,
			DataMartConnectionFailOverToBackupServer,
			CorruptClassificationRuleCollection = 2147763793U,
			DuplicateDataClassificationIdAcrossRulePack,
			ClassificationEngineFailure = 3221505619U,
			ClassificationEngineTimeout,
			ClassificationEngineConfigurationError = 2147763797U,
			FfoReportingTaskFailure = 3221506617U,
			FfoReportingRecipientTaskFailure,
			FailedToUploadPhoto = 3221507617U,
			FailedToRetrievePhoto,
			FailedToRemovePhoto,
			ReducedDynamicRangeSuccess = 1074014971U,
			ReducedDynamicRangeFailure = 3221498620U,
			FailedToGetCurrentDynamicRange,
			CurrentDynamicRange = 1074014974U
		}
	}
}
