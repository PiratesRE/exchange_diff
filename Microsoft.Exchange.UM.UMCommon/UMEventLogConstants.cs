using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM
{
	public static class UMEventLogConstants
	{
		public const string EventSource = "MSExchange Unified Messaging";

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMWorkerProcessStartSuccess = new ExEventLog.EventTuple(263144U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMWorkerProcessStartFailure = new ExEventLog.EventTuple(3221488617U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMWorkerProcessStopSuccess = new ExEventLog.EventTuple(263146U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMWorkerProcessStopFailure = new ExEventLog.EventTuple(3221488619U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InboundCallParams = new ExEventLog.EventTuple(263148U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OutboundCallParams = new ExEventLog.EventTuple(263149U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CallReceived = new ExEventLog.EventTuple(263150U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CallEndedByUser = new ExEventLog.EventTuple(263151U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FsmConfigurationError = new ExEventLog.EventTuple(3221488624U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FsmActivityStart = new ExEventLog.EventTuple(263153U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FsmConfigurationInitialized = new ExEventLog.EventTuple(263154U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PromptsPlayed = new ExEventLog.EventTuple(263155U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxLocked = new ExEventLog.EventTuple(263156U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogonDisconnect = new ExEventLog.EventTuple(263157U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ExceptionDuringCall = new ExEventLog.EventTuple(3221488630U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMUserEnabled = new ExEventLog.EventTuple(263159U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMUserDisabled = new ExEventLog.EventTuple(263160U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMUserUnlocked = new ExEventLog.EventTuple(263161U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMUserPasswordChanged = new ExEventLog.EventTuple(263162U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidChecksum = new ExEventLog.EventTuple(3221488635U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DirectorySearchResults = new ExEventLog.EventTuple(263164U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CallRejected = new ExEventLog.EventTuple(3221488637U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UMUserNotConfiguredForFax = new ExEventLog.EventTuple(2147746825U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SuccessfulLogon = new ExEventLog.EventTuple(263180U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMServiceStartSuccess = new ExEventLog.EventTuple(263181U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMServiceStartFailure = new ExEventLog.EventTuple(3221488654U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMServiceStopSuccess = new ExEventLog.EventTuple(263183U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMWorkerProcessExited = new ExEventLog.EventTuple(1074005014U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMWorkerProcessRequestedRecycle = new ExEventLog.EventTuple(1074005015U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMWorkerProcessNoProcessData = new ExEventLog.EventTuple(1074005016U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RecycledMaxCallsExceeded = new ExEventLog.EventTuple(1074005017U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WatsoningDueToRecycling = new ExEventLog.EventTuple(1074005018U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RecycledMaxMemoryPressureExceeded = new ExEventLog.EventTuple(1074005019U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RecycledMaxUptimeExceeded = new ExEventLog.EventTuple(1074005020U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RecycledMaxHeartBeatsMissedExceeded = new ExEventLog.EventTuple(1074005021U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_KilledMaxStartuptimeExceeded = new ExEventLog.EventTuple(1074005022U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_KilledMaxRetireTimeExceeded = new ExEventLog.EventTuple(1074005023U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMServiceStateChange = new ExEventLog.EventTuple(1074005024U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMWorkerProcessUnhandledException = new ExEventLog.EventTuple(3221488673U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_KilledCouldntEstablishControlChannel = new ExEventLog.EventTuple(1074005027U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NewDialPlanCreated = new ExEventLog.EventTuple(263204U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NewIPGatewayCreated = new ExEventLog.EventTuple(263205U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NewHuntGroupCreated = new ExEventLog.EventTuple(263206U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DialPlanRemoved = new ExEventLog.EventTuple(263207U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IPGatewayRemoved = new ExEventLog.EventTuple(263208U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HuntGroupRemoved = new ExEventLog.EventTuple(263209U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMServerEnabled = new ExEventLog.EventTuple(263210U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IPGatewayEnabled = new ExEventLog.EventTuple(263211U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMServerDisabled = new ExEventLog.EventTuple(263212U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IPGatewayDisabled = new ExEventLog.EventTuple(263213U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AutoAttendantCreated = new ExEventLog.EventTuple(263214U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AutoAttendantEnabled = new ExEventLog.EventTuple(263215U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AutoAttendantDisabled = new ExEventLog.EventTuple(263216U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CallTransfer = new ExEventLog.EventTuple(263217U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_KillWorkItemAndMoveToBadVMFolder = new ExEventLog.EventTuple(3221488690U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PlayOnPhoneRequest = new ExEventLog.EventTuple(263219U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OutDialingRulesFailure = new ExEventLog.EventTuple(263220U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DisconnectRequest = new ExEventLog.EventTuple(263221U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ExExceptionDuringCall = new ExEventLog.EventTuple(2147746870U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PlatformException = new ExEventLog.EventTuple(2147746871U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_QuotaExceededFailedSubmit = new ExEventLog.EventTuple(2147746872U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FailedSubmitSincePipelineIsFull = new ExEventLog.EventTuple(2147746873U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMClientAccessError = new ExEventLog.EventTuple(2147746875U, 5, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CallEndedByApplication = new ExEventLog.EventTuple(263228U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OutdialingConfigurationWarning = new ExEventLog.EventTuple(2147746877U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AutoAttendantNoGrammarFileWarning = new ExEventLog.EventTuple(2147746878U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OutboundCallFailed = new ExEventLog.EventTuple(2147746879U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RecycledMaxTempDirSizeExceeded = new ExEventLog.EventTuple(1074005060U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DialPlanCustomPromptUploadSucceeded = new ExEventLog.EventTuple(1074005062U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DialPlanCustomPromptUploadFailed = new ExEventLog.EventTuple(2147746887U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DialPlanCustomPromptCacheUpdated = new ExEventLog.EventTuple(1074005064U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DialPlanDeleteContentFailed = new ExEventLog.EventTuple(2147746889U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AutoAttendantCustomPromptUploadSucceeded = new ExEventLog.EventTuple(1074005066U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AutoAttendantCustomPromptUploadFailed = new ExEventLog.EventTuple(2147746891U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AutoAttendantCustomPromptCacheUpdate = new ExEventLog.EventTuple(1074005068U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AutoAttendantDeleteContentFailed = new ExEventLog.EventTuple(2147746893U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ContactsNoGrammarFileWarning = new ExEventLog.EventTuple(2147746894U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMInvalidSchema = new ExEventLog.EventTuple(3221488719U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ADTransientError = new ExEventLog.EventTuple(2147746898U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ADPermanentError = new ExEventLog.EventTuple(3221488723U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADDataError = new ExEventLog.EventTuple(2147746900U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidExtensionInCall = new ExEventLog.EventTuple(2147746901U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnabletoRegisterForDialPlanADNotifications = new ExEventLog.EventTuple(2147746902U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnabletoRegisterForIPGatewayADNotifications = new ExEventLog.EventTuple(2147746903U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceCertificateDetails = new ExEventLog.EventTuple(1074005080U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_IncomingTLSCallFailure = new ExEventLog.EventTuple(3221488729U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_StartingMode = new ExEventLog.EventTuple(1074005082U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IncorrectPeers = new ExEventLog.EventTuple(2147746907U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_StoppingListeningforCertificateChange = new ExEventLog.EventTuple(1074005084U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_StartedListeningWithNewCertificate = new ExEventLog.EventTuple(1074005085U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMWorkerProcessRecycledToChangeCerts = new ExEventLog.EventTuple(1074005087U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CertificateNearExpiry = new ExEventLog.EventTuple(2147746912U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnabletoRegisterForServerADNotifications = new ExEventLog.EventTuple(2147746913U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnabletoRegisterForAutoAttendantADNotifications = new ExEventLog.EventTuple(2147746914U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CertificateExpiryIsGood = new ExEventLog.EventTuple(1074005091U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NoPeersFound = new ExEventLog.EventTuple(2147746916U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMClientAccessCertDetails = new ExEventLog.EventTuple(1074005093U, 5, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MSSIncomingTLSCallFailure = new ExEventLog.EventTuple(2147746918U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AcmConversionFailed = new ExEventLog.EventTuple(2147746919U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AAMissingOperatorExtension = new ExEventLog.EventTuple(3221488744U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CorruptedConfiguration = new ExEventLog.EventTuple(2147746926U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CorruptedPIN = new ExEventLog.EventTuple(3221488751U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CallTransferFailed = new ExEventLog.EventTuple(2147746928U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpSpnRegistrationFailure = new ExEventLog.EventTuple(3221488754U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DisconnectOnUMIPGatewayDisabledImmediate = new ExEventLog.EventTuple(263286U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DisconnectOnUMServerDisabledImmediate = new ExEventLog.EventTuple(263287U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADNotificationProcessingError = new ExEventLog.EventTuple(2147746936U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnabletoRegisterForHuntGroupADNotifications = new ExEventLog.EventTuple(2147746937U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToResolveVoicemailCaller = new ExEventLog.EventTuple(2147746938U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PingResponseFailure = new ExEventLog.EventTuple(2147746943U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidSipHeader = new ExEventLog.EventTuple(2147746944U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SpeechAAGrammarEntryFormatErrors = new ExEventLog.EventTuple(2147746946U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AALanguageNotFound = new ExEventLog.EventTuple(2147746948U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SpeechGrammarFilterListSchemaFailureWarning = new ExEventLog.EventTuple(2147746949U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SpeechGrammarFilterListInvalidWarning = new ExEventLog.EventTuple(2147746950U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SystemError = new ExEventLog.EventTuple(2147746951U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AACustomPromptFileMissing = new ExEventLog.EventTuple(2147746952U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DialPlanCustomPromptFileMissing = new ExEventLog.EventTuple(2147746953U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CallToUnusableAA = new ExEventLog.EventTuple(2147746956U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WNoPeersFound = new ExEventLog.EventTuple(2147746957U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WADAccessError = new ExEventLog.EventTuple(2147746958U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CallData = new ExEventLog.EventTuple(263312U, 6, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DivertedExtensionNotProvisioned = new ExEventLog.EventTuple(2147746961U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CallDataCallAnswer = new ExEventLog.EventTuple(263314U, 6, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CallDataSubscriberAccess = new ExEventLog.EventTuple(263315U, 6, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CallDataAutoAttendant = new ExEventLog.EventTuple(263316U, 6, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CallDataOutbound = new ExEventLog.EventTuple(263317U, 6, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RPCRequestError = new ExEventLog.EventTuple(2147746966U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DuplicatePeersFound = new ExEventLog.EventTuple(2147746967U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MissingDialGroupEntry = new ExEventLog.EventTuple(2147746968U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EventNotifSessionInvalidFormat = new ExEventLog.EventTuple(2147746969U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EventNotifSessionSignalingError = new ExEventLog.EventTuple(2147746970U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PersonalContactsSearchPlatformFailure = new ExEventLog.EventTuple(2147746971U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GALSearchPlatformFailure = new ExEventLog.EventTuple(2147746972U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMNumberNotConfiguredForFax = new ExEventLog.EventTuple(2147746974U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMServiceUnhandledException = new ExEventLog.EventTuple(3221488799U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SamePeerInTwoModes = new ExEventLog.EventTuple(2147746976U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToGetSocket = new ExEventLog.EventTuple(3221488804U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiagnosticResponseSequence = new ExEventLog.EventTuple(263333U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ContactResolutionTemporarilyDisabled = new ExEventLog.EventTuple(2147746983U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CallDataFax = new ExEventLog.EventTuple(263336U, 6, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LangPackDirectoryNotFound = new ExEventLog.EventTuple(2147746985U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidPeerDNSHostName = new ExEventLog.EventTuple(2147746986U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ProcessingSipHeaderForCalleeInfo = new ExEventLog.EventTuple(263444U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OutDialingRequest = new ExEventLog.EventTuple(263445U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FindMeFailedSinceMaximumCallsLimitReached = new ExEventLog.EventTuple(2147747094U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FindMeOutDialingRulesFailure = new ExEventLog.EventTuple(263447U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FindMeInvalidPhoneNumber = new ExEventLog.EventTuple(263448U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CallAnsweredByPAA = new ExEventLog.EventTuple(263449U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TranscriptionNotAttemptedDueToThrottling = new ExEventLog.EventTuple(2147747098U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TranscriptionAttemptedButCancelled = new ExEventLog.EventTuple(2147747099U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CorruptedPAAStore = new ExEventLog.EventTuple(2147747100U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_NoOutboundGatewaysForDialPlanWithId = new ExEventLog.EventTuple(3221488926U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToConnectToMailbox = new ExEventLog.EventTuple(2147747103U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToRetrieveMailboxData = new ExEventLog.EventTuple(2147747104U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TimedOutRetrievingMailboxData = new ExEventLog.EventTuple(2147747105U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ExternalFqdnDetected = new ExEventLog.EventTuple(263458U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CallRedirectedToServer = new ExEventLog.EventTuple(263461U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PlatformTlsException = new ExEventLog.EventTuple(2147747111U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TimedOutEvaluatingPAA = new ExEventLog.EventTuple(2147747113U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UserNotEnabledForPlayOnPhone = new ExEventLog.EventTuple(263466U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VirtualNumberCall = new ExEventLog.EventTuple(263468U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VirtualNumberCallBlocked = new ExEventLog.EventTuple(263469U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AACustomPromptInvalid = new ExEventLog.EventTuple(3221488942U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DialPlanCustomPromptInvalid = new ExEventLog.EventTuple(3221488943U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MserveLookup = new ExEventLog.EventTuple(263472U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MserveLookupError = new ExEventLog.EventTuple(3221488945U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MserveLookupTargetForest = new ExEventLog.EventTuple(263475U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TranscriptionWordCounts = new ExEventLog.EventTuple(263477U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_KillWorkItemAndDelete = new ExEventLog.EventTuple(3221488951U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AAPlayOnPhoneRequest = new ExEventLog.EventTuple(263483U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AAOutDialingRulesFailure = new ExEventLog.EventTuple(2147747132U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AAOutDialingFailure = new ExEventLog.EventTuple(2147747133U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MwiMessageDeliverySucceeded = new ExEventLog.EventTuple(1074005311U, 7, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MwiMessageDeliveryFailed = new ExEventLog.EventTuple(2147747136U, 7, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MwiSyncMailboxFailed = new ExEventLog.EventTuple(2147747137U, 7, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MwiQueryDatabaseFailed = new ExEventLog.EventTuple(2147747138U, 7, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MwiTextMessageSent = new ExEventLog.EventTuple(1074005315U, 7, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PoPRequestError = new ExEventLog.EventTuple(2147747140U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CallRouterUnableToMapGatewayToForest = new ExEventLog.EventTuple(3221488967U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CallRouterRedirectedTenantGatewayCall = new ExEventLog.EventTuple(1074005320U, 8, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMPartnerMessageSucceeded = new ExEventLog.EventTuple(1074005323U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UMPartnerMessageServerFailed = new ExEventLog.EventTuple(2147747148U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UMPartnerMessageNoServersAvailable = new ExEventLog.EventTuple(2147747149U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMPartnerMessageEventSkippedError = new ExEventLog.EventTuple(3221488974U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMPartnerMessageRpcRequestError = new ExEventLog.EventTuple(2147747151U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MwiMessageDeliveryFailedToUM = new ExEventLog.EventTuple(2147747152U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SipPeersUnhealthy = new ExEventLog.EventTuple(2147747192U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SipPeersHealthy = new ExEventLog.EventTuple(263545U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LegacyServerNotFoundInDialPlan = new ExEventLog.EventTuple(3221489018U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LegacyServerNotRunningInDialPlan = new ExEventLog.EventTuple(3221489019U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMServerNotFoundInSite = new ExEventLog.EventTuple(3221489020U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToResolveCallerToSubscriber = new ExEventLog.EventTuple(2147747198U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxAccessFailure = new ExEventLog.EventTuple(3221489023U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CasToUmRpcFailure = new ExEventLog.EventTuple(3221489024U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple____CasToUmRpcSuccess___ = new ExEventLog.EventTuple(3221489025U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMMwiAssistantStarted = new ExEventLog.EventTuple(1074005379U, 7, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMServiceCallRejected = new ExEventLog.EventTuple(3221489028U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UMServiceLowOnResources = new ExEventLog.EventTuple(3221489029U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OutboundCallFailedForOnPremiseGateway = new ExEventLog.EventTuple(2147747207U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FaxTransferFailure = new ExEventLog.EventTuple(3221489032U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UserFaxServerSetupFailure = new ExEventLog.EventTuple(3221489033U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FaxPartnerHasServerError = new ExEventLog.EventTuple(3221489034U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMStartupModeChanged = new ExEventLog.EventTuple(2147747211U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RMSIntepersonalSendFailure = new ExEventLog.EventTuple(3221489036U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RMSCallAnsweringSendFailure = new ExEventLog.EventTuple(3221489037U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RMSReadFailure = new ExEventLog.EventTuple(3221489038U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PipeLineError = new ExEventLog.EventTuple(3221489039U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SipPeerCacheRefreshed = new ExEventLog.EventTuple(1074005392U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FindUMIPGatewayInAD = new ExEventLog.EventTuple(1074005393U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidClientCertificate = new ExEventLog.EventTuple(2147747218U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMServiceFatalException = new ExEventLog.EventTuple(3221489043U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FAXRequestIsNotAcceptable = new ExEventLog.EventTuple(2147747220U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TranscriptionPartnerFailure = new ExEventLog.EventTuple(2147747221U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMKillCurrentProcess = new ExEventLog.EventTuple(3221489046U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToSaveCDR = new ExEventLog.EventTuple(3221489047U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToFindUMReportData = new ExEventLog.EventTuple(3221489048U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FatalErrorDuringAggregation = new ExEventLog.EventTuple(3221489049U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PermanentErrorDuringAggregation = new ExEventLog.EventTuple(3221489050U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PipelineStalled = new ExEventLog.EventTuple(3221489051U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CannotSetExtendedProp = new ExEventLog.EventTuple(2147747228U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HeuristicallyChosenSIPProxy = new ExEventLog.EventTuple(1074005405U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MRASMediaEstablishedStatusFailed = new ExEventLog.EventTuple(2147747230U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MRASCredentialsAcquisitionFailed = new ExEventLog.EventTuple(2147747231U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MRASResourceAllocationFailed = new ExEventLog.EventTuple(2147747232U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SIPProxyDetails = new ExEventLog.EventTuple(1074005409U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CallRejectedSinceGatewayDisabled = new ExEventLog.EventTuple(3221489058U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToFindEDiscoveryMailbox = new ExEventLog.EventTuple(3221489059U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OCSUserNotProvisioned = new ExEventLog.EventTuple(2147747236U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DialPlanOrAutoAttendantNotProvisioned = new ExEventLog.EventTuple(2147747237U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PipelineWorkItemSLAFailure = new ExEventLog.EventTuple(3221489062U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMServiceSocketShutdown = new ExEventLog.EventTuple(2147747239U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMServiceSocketOpen = new ExEventLog.EventTuple(1074005416U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MsOrganizationNotAuthoritativeDomain = new ExEventLog.EventTuple(3221489065U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MobileSpeechRecoRPCGeneralUnexpectedFailure = new ExEventLog.EventTuple(3221489216U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MobileSpeechRecoRPCUnexpectedFailure = new ExEventLog.EventTuple(3221489217U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MobileSpeechRecoRPCFailure = new ExEventLog.EventTuple(3221489218U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MobileSpeechRecoRPCSuccess = new ExEventLog.EventTuple(263747U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MobileSpeechRecoAddRecoRequestRPCParams = new ExEventLog.EventTuple(263748U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MobileSpeechRecoRecognizeRPCParams = new ExEventLog.EventTuple(263749U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MobileSpeechRecoClientRPCFailure = new ExEventLog.EventTuple(3221489222U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MobileSpeechRecoClientRPCSuccess = new ExEventLog.EventTuple(263751U, 5, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MobileSpeechRecoClientAddRecoRequestRPCParams = new ExEventLog.EventTuple(263752U, 5, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MobileSpeechRecoClientRecognizeRPCParams = new ExEventLog.EventTuple(263753U, 5, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMGrammarFetcherError = new ExEventLog.EventTuple(3221489226U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMGrammarFetcherSuccess = new ExEventLog.EventTuple(1074005579U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GrammarGenerationStarted = new ExEventLog.EventTuple(1074005581U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GrammarGenerationSuccessful = new ExEventLog.EventTuple(1074005582U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GrammarGenerationFailed = new ExEventLog.EventTuple(3221489231U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GrammarGenerationMissingCulture = new ExEventLog.EventTuple(3221489232U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GrammarGeneratorCouldntFindUser = new ExEventLog.EventTuple(3221489233U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GrammarGenerationCouldntFindSystemMailbox = new ExEventLog.EventTuple(3221489234U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MobileSpeechRecoLoadGrammarFailure = new ExEventLog.EventTuple(3221489236U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMCallRouterSocketShutdown = new ExEventLog.EventTuple(2147747413U, 8, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CallRoutedSuccessfully = new ExEventLog.EventTuple(263766U, 8, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GrammarGenerationCleanupFailed = new ExEventLog.EventTuple(3221489239U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GrammarFetcherCleanupFailed = new ExEventLog.EventTuple(3221489240U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToCreateDirectoryProcessorDirectory = new ExEventLog.EventTuple(3221489241U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CopyADToFileStarted = new ExEventLog.EventTuple(1074005597U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CopyADToFileCompleted = new ExEventLog.EventTuple(1074005598U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GrammarGenerationWritingGrammarEntriesStarted = new ExEventLog.EventTuple(1074005599U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GrammarGenerationWritingGrammarEntriesCompleted = new ExEventLog.EventTuple(1074005600U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnabletoRegisterForCallRouterSettingsADNotifications = new ExEventLog.EventTuple(2147747425U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DynamicDirectoryGrammarGenerationFailure = new ExEventLog.EventTuple(3221489250U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DialPlanDefaultLanguageNotFound = new ExEventLog.EventTuple(2147747427U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceRequestRejected = new ExEventLog.EventTuple(2147747428U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UserNotificationProxied = new ExEventLog.EventTuple(263781U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UserNotificationFailed = new ExEventLog.EventTuple(3221489254U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LoadNormalizationCacheFailed = new ExEventLog.EventTuple(2147747431U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SaveNormalizationCacheFailed = new ExEventLog.EventTuple(2147747432U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DirectoryProcessorStarted = new ExEventLog.EventTuple(1074005609U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DirectoryProcessorCompleted = new ExEventLog.EventTuple(1074005610U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CallRouterCertificateNearExpiry = new ExEventLog.EventTuple(2147747435U, 8, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CallRouterCertificateExpiryIsGood = new ExEventLog.EventTuple(1074005612U, 8, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CallRouterCertificateDetails = new ExEventLog.EventTuple(1074005613U, 8, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CallRouterStartingMode = new ExEventLog.EventTuple(1074005614U, 8, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CallRouterCallRejected = new ExEventLog.EventTuple(3221489263U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CallRouterSocketOpen = new ExEventLog.EventTuple(1074005616U, 8, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CallRouterIncomingTLSCallFailure = new ExEventLog.EventTuple(2147747441U, 8, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CallRouterInboundCallParams = new ExEventLog.EventTuple(263794U, 8, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OptionsMessageRejected = new ExEventLog.EventTuple(2147747443U, 8, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CopyADToFileFailed = new ExEventLog.EventTuple(3221489268U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GrammarGenerationSkippedNoADFile = new ExEventLog.EventTuple(3221489269U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DirectoryProcessorTaskThrewException = new ExEventLog.EventTuple(2147747446U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DtmfMapGenerationStarted = new ExEventLog.EventTuple(1074005623U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DtmfMapGenerationSuccessful = new ExEventLog.EventTuple(1074005624U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DtmfMapUpdateFailed = new ExEventLog.EventTuple(2147747449U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DtmfMapGenerationSkippedNoADFile = new ExEventLog.EventTuple(3221489274U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SpeechRecoRequestParams = new ExEventLog.EventTuple(1074005627U, 5, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidSpeechRecoRequest = new ExEventLog.EventTuple(2147747452U, 5, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SpeechRecoRequestCompleted = new ExEventLog.EventTuple(1074005629U, 5, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SpeechRecoRequestFailed = new ExEventLog.EventTuple(3221489278U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_StartFindInGALSpeechRecoRequestParams = new ExEventLog.EventTuple(1074005631U, 5, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_StartFindInGALSpeechRecoRequestSuccess = new ExEventLog.EventTuple(1074005632U, 5, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_StartFindInGALSpeechRecoRequestFailed = new ExEventLog.EventTuple(3221489281U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CompleteFindInGALSpeechRecoRequestParams = new ExEventLog.EventTuple(1074005634U, 5, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CompleteFindInGALSpeechRecoRequestSuccess = new ExEventLog.EventTuple(1074005635U, 5, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CompleteFindInGALSpeechRecoRequestFailed = new ExEventLog.EventTuple(3221489284U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MobileSpeechRecoClientStartFindInGALRequestParams = new ExEventLog.EventTuple(1074005637U, 5, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MobileSpeechRecoClientCompleteFindInGALRequestParams = new ExEventLog.EventTuple(1074005638U, 5, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MobileSpeechRecoClientFindInGALResult = new ExEventLog.EventTuple(1074005639U, 5, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReadLastSuccessRunIDFailed = new ExEventLog.EventTuple(1074005641U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DirectoryProcessorInitialStepEncounteredException = new ExEventLog.EventTuple(3221489290U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LoadDtmfMapGenerationMetadataFailed = new ExEventLog.EventTuple(2147747467U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SaveDtmfMapGenerationMetadataFailed = new ExEventLog.EventTuple(2147747468U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMMailboxCmdletError = new ExEventLog.EventTuple(3221489293U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GrammarFileUploadToSystemMailboxFailed = new ExEventLog.EventTuple(3221489294U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GrammarMailboxNotFound = new ExEventLog.EventTuple(2147747471U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SetUMGrammarReadyFlagFailed = new ExEventLog.EventTuple(3221489296U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MobileSpeechRecoTimeout = new ExEventLog.EventTuple(2147747473U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UploadNormalizationCacheFailed = new ExEventLog.EventTuple(2147747474U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DownloadNormalizationCacheFailed = new ExEventLog.EventTuple(2147747475U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SipPeerCertificateSubjectName = new ExEventLog.EventTuple(3221489300U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UploadDtmfMapMetadataFailed = new ExEventLog.EventTuple(2147747477U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DownloadDtmfMapMetadataFailed = new ExEventLog.EventTuple(2147747478U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MobileSpeechRecoClientAsyncCallTimedOut = new ExEventLog.EventTuple(3221489303U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToAccessOrganizationMailbox = new ExEventLog.EventTuple(3221489304U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SetScaleOutCapabilityFailed = new ExEventLog.EventTuple(3221489305U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GrammarFileMaxEntriesExceeded = new ExEventLog.EventTuple(2147747482U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GrammarFileMaxCountExceeded = new ExEventLog.EventTuple(2147747483U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MRASMediaChannelEstablishFailed = new ExEventLog.EventTuple(2147747484U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WatsoningDueToTimeout = new ExEventLog.EventTuple(2147747485U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WatsoningDueToWorkerProcessNotTerminating = new ExEventLog.EventTuple(2147747486U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		private enum Category : short
		{
			UMWorkerProcess = 1,
			UMCore,
			UMManagement,
			UMService,
			UMClientAccess,
			UMCallData,
			MWI_General,
			UMCallRouter
		}

		internal enum Message : uint
		{
			UMWorkerProcessStartSuccess = 263144U,
			UMWorkerProcessStartFailure = 3221488617U,
			UMWorkerProcessStopSuccess = 263146U,
			UMWorkerProcessStopFailure = 3221488619U,
			InboundCallParams = 263148U,
			OutboundCallParams,
			CallReceived,
			CallEndedByUser,
			FsmConfigurationError = 3221488624U,
			FsmActivityStart = 263153U,
			FsmConfigurationInitialized,
			PromptsPlayed,
			MailboxLocked,
			LogonDisconnect,
			ExceptionDuringCall = 3221488630U,
			UMUserEnabled = 263159U,
			UMUserDisabled,
			UMUserUnlocked,
			UMUserPasswordChanged,
			InvalidChecksum = 3221488635U,
			DirectorySearchResults = 263164U,
			CallRejected = 3221488637U,
			UMUserNotConfiguredForFax = 2147746825U,
			SuccessfulLogon = 263180U,
			UMServiceStartSuccess,
			UMServiceStartFailure = 3221488654U,
			UMServiceStopSuccess = 263183U,
			UMWorkerProcessExited = 1074005014U,
			UMWorkerProcessRequestedRecycle,
			UMWorkerProcessNoProcessData,
			RecycledMaxCallsExceeded,
			WatsoningDueToRecycling,
			RecycledMaxMemoryPressureExceeded,
			RecycledMaxUptimeExceeded,
			RecycledMaxHeartBeatsMissedExceeded,
			KilledMaxStartuptimeExceeded,
			KilledMaxRetireTimeExceeded,
			UMServiceStateChange,
			UMWorkerProcessUnhandledException = 3221488673U,
			KilledCouldntEstablishControlChannel = 1074005027U,
			NewDialPlanCreated = 263204U,
			NewIPGatewayCreated,
			NewHuntGroupCreated,
			DialPlanRemoved,
			IPGatewayRemoved,
			HuntGroupRemoved,
			UMServerEnabled,
			IPGatewayEnabled,
			UMServerDisabled,
			IPGatewayDisabled,
			AutoAttendantCreated,
			AutoAttendantEnabled,
			AutoAttendantDisabled,
			CallTransfer,
			KillWorkItemAndMoveToBadVMFolder = 3221488690U,
			PlayOnPhoneRequest = 263219U,
			OutDialingRulesFailure,
			DisconnectRequest,
			ExExceptionDuringCall = 2147746870U,
			PlatformException,
			QuotaExceededFailedSubmit,
			FailedSubmitSincePipelineIsFull,
			UMClientAccessError = 2147746875U,
			CallEndedByApplication = 263228U,
			OutdialingConfigurationWarning = 2147746877U,
			AutoAttendantNoGrammarFileWarning,
			OutboundCallFailed,
			RecycledMaxTempDirSizeExceeded = 1074005060U,
			DialPlanCustomPromptUploadSucceeded = 1074005062U,
			DialPlanCustomPromptUploadFailed = 2147746887U,
			DialPlanCustomPromptCacheUpdated = 1074005064U,
			DialPlanDeleteContentFailed = 2147746889U,
			AutoAttendantCustomPromptUploadSucceeded = 1074005066U,
			AutoAttendantCustomPromptUploadFailed = 2147746891U,
			AutoAttendantCustomPromptCacheUpdate = 1074005068U,
			AutoAttendantDeleteContentFailed = 2147746893U,
			ContactsNoGrammarFileWarning,
			UMInvalidSchema = 3221488719U,
			ADTransientError = 2147746898U,
			ADPermanentError = 3221488723U,
			ADDataError = 2147746900U,
			InvalidExtensionInCall,
			UnabletoRegisterForDialPlanADNotifications,
			UnabletoRegisterForIPGatewayADNotifications,
			ServiceCertificateDetails = 1074005080U,
			IncomingTLSCallFailure = 3221488729U,
			StartingMode = 1074005082U,
			IncorrectPeers = 2147746907U,
			StoppingListeningforCertificateChange = 1074005084U,
			StartedListeningWithNewCertificate,
			UMWorkerProcessRecycledToChangeCerts = 1074005087U,
			CertificateNearExpiry = 2147746912U,
			UnabletoRegisterForServerADNotifications,
			UnabletoRegisterForAutoAttendantADNotifications,
			CertificateExpiryIsGood = 1074005091U,
			NoPeersFound = 2147746916U,
			UMClientAccessCertDetails = 1074005093U,
			MSSIncomingTLSCallFailure = 2147746918U,
			AcmConversionFailed,
			AAMissingOperatorExtension = 3221488744U,
			CorruptedConfiguration = 2147746926U,
			CorruptedPIN = 3221488751U,
			CallTransferFailed = 2147746928U,
			SmtpSpnRegistrationFailure = 3221488754U,
			DisconnectOnUMIPGatewayDisabledImmediate = 263286U,
			DisconnectOnUMServerDisabledImmediate,
			ADNotificationProcessingError = 2147746936U,
			UnabletoRegisterForHuntGroupADNotifications,
			UnableToResolveVoicemailCaller,
			PingResponseFailure = 2147746943U,
			InvalidSipHeader,
			SpeechAAGrammarEntryFormatErrors = 2147746946U,
			AALanguageNotFound = 2147746948U,
			SpeechGrammarFilterListSchemaFailureWarning,
			SpeechGrammarFilterListInvalidWarning,
			SystemError,
			AACustomPromptFileMissing,
			DialPlanCustomPromptFileMissing,
			CallToUnusableAA = 2147746956U,
			WNoPeersFound,
			WADAccessError,
			CallData = 263312U,
			DivertedExtensionNotProvisioned = 2147746961U,
			CallDataCallAnswer = 263314U,
			CallDataSubscriberAccess,
			CallDataAutoAttendant,
			CallDataOutbound,
			RPCRequestError = 2147746966U,
			DuplicatePeersFound,
			MissingDialGroupEntry,
			EventNotifSessionInvalidFormat,
			EventNotifSessionSignalingError,
			PersonalContactsSearchPlatformFailure,
			GALSearchPlatformFailure,
			UMNumberNotConfiguredForFax = 2147746974U,
			UMServiceUnhandledException = 3221488799U,
			SamePeerInTwoModes = 2147746976U,
			UnableToGetSocket = 3221488804U,
			DiagnosticResponseSequence = 263333U,
			ContactResolutionTemporarilyDisabled = 2147746983U,
			CallDataFax = 263336U,
			LangPackDirectoryNotFound = 2147746985U,
			InvalidPeerDNSHostName,
			ProcessingSipHeaderForCalleeInfo = 263444U,
			OutDialingRequest,
			FindMeFailedSinceMaximumCallsLimitReached = 2147747094U,
			FindMeOutDialingRulesFailure = 263447U,
			FindMeInvalidPhoneNumber,
			CallAnsweredByPAA,
			TranscriptionNotAttemptedDueToThrottling = 2147747098U,
			TranscriptionAttemptedButCancelled,
			CorruptedPAAStore,
			NoOutboundGatewaysForDialPlanWithId = 3221488926U,
			FailedToConnectToMailbox = 2147747103U,
			FailedToRetrieveMailboxData,
			TimedOutRetrievingMailboxData,
			ExternalFqdnDetected = 263458U,
			CallRedirectedToServer = 263461U,
			PlatformTlsException = 2147747111U,
			TimedOutEvaluatingPAA = 2147747113U,
			UserNotEnabledForPlayOnPhone = 263466U,
			VirtualNumberCall = 263468U,
			VirtualNumberCallBlocked,
			AACustomPromptInvalid = 3221488942U,
			DialPlanCustomPromptInvalid,
			MserveLookup = 263472U,
			MserveLookupError = 3221488945U,
			MserveLookupTargetForest = 263475U,
			TranscriptionWordCounts = 263477U,
			KillWorkItemAndDelete = 3221488951U,
			AAPlayOnPhoneRequest = 263483U,
			AAOutDialingRulesFailure = 2147747132U,
			AAOutDialingFailure,
			MwiMessageDeliverySucceeded = 1074005311U,
			MwiMessageDeliveryFailed = 2147747136U,
			MwiSyncMailboxFailed,
			MwiQueryDatabaseFailed,
			MwiTextMessageSent = 1074005315U,
			PoPRequestError = 2147747140U,
			CallRouterUnableToMapGatewayToForest = 3221488967U,
			CallRouterRedirectedTenantGatewayCall = 1074005320U,
			UMPartnerMessageSucceeded = 1074005323U,
			UMPartnerMessageServerFailed = 2147747148U,
			UMPartnerMessageNoServersAvailable,
			UMPartnerMessageEventSkippedError = 3221488974U,
			UMPartnerMessageRpcRequestError = 2147747151U,
			MwiMessageDeliveryFailedToUM,
			SipPeersUnhealthy = 2147747192U,
			SipPeersHealthy = 263545U,
			LegacyServerNotFoundInDialPlan = 3221489018U,
			LegacyServerNotRunningInDialPlan,
			UMServerNotFoundInSite,
			UnableToResolveCallerToSubscriber = 2147747198U,
			MailboxAccessFailure = 3221489023U,
			CasToUmRpcFailure,
			___CasToUmRpcSuccess___,
			UMMwiAssistantStarted = 1074005379U,
			UMServiceCallRejected = 3221489028U,
			UMServiceLowOnResources,
			OutboundCallFailedForOnPremiseGateway = 2147747207U,
			FaxTransferFailure = 3221489032U,
			UserFaxServerSetupFailure,
			FaxPartnerHasServerError,
			UMStartupModeChanged = 2147747211U,
			RMSIntepersonalSendFailure = 3221489036U,
			RMSCallAnsweringSendFailure,
			RMSReadFailure,
			PipeLineError,
			SipPeerCacheRefreshed = 1074005392U,
			FindUMIPGatewayInAD,
			InvalidClientCertificate = 2147747218U,
			UMServiceFatalException = 3221489043U,
			FAXRequestIsNotAcceptable = 2147747220U,
			TranscriptionPartnerFailure,
			UMKillCurrentProcess = 3221489046U,
			UnableToSaveCDR,
			UnableToFindUMReportData,
			FatalErrorDuringAggregation,
			PermanentErrorDuringAggregation,
			PipelineStalled,
			CannotSetExtendedProp = 2147747228U,
			HeuristicallyChosenSIPProxy = 1074005405U,
			MRASMediaEstablishedStatusFailed = 2147747230U,
			MRASCredentialsAcquisitionFailed,
			MRASResourceAllocationFailed,
			SIPProxyDetails = 1074005409U,
			CallRejectedSinceGatewayDisabled = 3221489058U,
			UnableToFindEDiscoveryMailbox,
			OCSUserNotProvisioned = 2147747236U,
			DialPlanOrAutoAttendantNotProvisioned,
			PipelineWorkItemSLAFailure = 3221489062U,
			UMServiceSocketShutdown = 2147747239U,
			UMServiceSocketOpen = 1074005416U,
			MsOrganizationNotAuthoritativeDomain = 3221489065U,
			MobileSpeechRecoRPCGeneralUnexpectedFailure = 3221489216U,
			MobileSpeechRecoRPCUnexpectedFailure,
			MobileSpeechRecoRPCFailure,
			MobileSpeechRecoRPCSuccess = 263747U,
			MobileSpeechRecoAddRecoRequestRPCParams,
			MobileSpeechRecoRecognizeRPCParams,
			MobileSpeechRecoClientRPCFailure = 3221489222U,
			MobileSpeechRecoClientRPCSuccess = 263751U,
			MobileSpeechRecoClientAddRecoRequestRPCParams,
			MobileSpeechRecoClientRecognizeRPCParams,
			UMGrammarFetcherError = 3221489226U,
			UMGrammarFetcherSuccess = 1074005579U,
			GrammarGenerationStarted = 1074005581U,
			GrammarGenerationSuccessful,
			GrammarGenerationFailed = 3221489231U,
			GrammarGenerationMissingCulture,
			GrammarGeneratorCouldntFindUser,
			GrammarGenerationCouldntFindSystemMailbox,
			MobileSpeechRecoLoadGrammarFailure = 3221489236U,
			UMCallRouterSocketShutdown = 2147747413U,
			CallRoutedSuccessfully = 263766U,
			GrammarGenerationCleanupFailed = 3221489239U,
			GrammarFetcherCleanupFailed,
			UnableToCreateDirectoryProcessorDirectory,
			CopyADToFileStarted = 1074005597U,
			CopyADToFileCompleted,
			GrammarGenerationWritingGrammarEntriesStarted,
			GrammarGenerationWritingGrammarEntriesCompleted,
			UnabletoRegisterForCallRouterSettingsADNotifications = 2147747425U,
			DynamicDirectoryGrammarGenerationFailure = 3221489250U,
			DialPlanDefaultLanguageNotFound = 2147747427U,
			ServiceRequestRejected,
			UserNotificationProxied = 263781U,
			UserNotificationFailed = 3221489254U,
			LoadNormalizationCacheFailed = 2147747431U,
			SaveNormalizationCacheFailed,
			DirectoryProcessorStarted = 1074005609U,
			DirectoryProcessorCompleted,
			CallRouterCertificateNearExpiry = 2147747435U,
			CallRouterCertificateExpiryIsGood = 1074005612U,
			CallRouterCertificateDetails,
			CallRouterStartingMode,
			CallRouterCallRejected = 3221489263U,
			CallRouterSocketOpen = 1074005616U,
			CallRouterIncomingTLSCallFailure = 2147747441U,
			CallRouterInboundCallParams = 263794U,
			OptionsMessageRejected = 2147747443U,
			CopyADToFileFailed = 3221489268U,
			GrammarGenerationSkippedNoADFile,
			DirectoryProcessorTaskThrewException = 2147747446U,
			DtmfMapGenerationStarted = 1074005623U,
			DtmfMapGenerationSuccessful,
			DtmfMapUpdateFailed = 2147747449U,
			DtmfMapGenerationSkippedNoADFile = 3221489274U,
			SpeechRecoRequestParams = 1074005627U,
			InvalidSpeechRecoRequest = 2147747452U,
			SpeechRecoRequestCompleted = 1074005629U,
			SpeechRecoRequestFailed = 3221489278U,
			StartFindInGALSpeechRecoRequestParams = 1074005631U,
			StartFindInGALSpeechRecoRequestSuccess,
			StartFindInGALSpeechRecoRequestFailed = 3221489281U,
			CompleteFindInGALSpeechRecoRequestParams = 1074005634U,
			CompleteFindInGALSpeechRecoRequestSuccess,
			CompleteFindInGALSpeechRecoRequestFailed = 3221489284U,
			MobileSpeechRecoClientStartFindInGALRequestParams = 1074005637U,
			MobileSpeechRecoClientCompleteFindInGALRequestParams,
			MobileSpeechRecoClientFindInGALResult,
			ReadLastSuccessRunIDFailed = 1074005641U,
			DirectoryProcessorInitialStepEncounteredException = 3221489290U,
			LoadDtmfMapGenerationMetadataFailed = 2147747467U,
			SaveDtmfMapGenerationMetadataFailed,
			UMMailboxCmdletError = 3221489293U,
			GrammarFileUploadToSystemMailboxFailed,
			GrammarMailboxNotFound = 2147747471U,
			SetUMGrammarReadyFlagFailed = 3221489296U,
			MobileSpeechRecoTimeout = 2147747473U,
			UploadNormalizationCacheFailed,
			DownloadNormalizationCacheFailed,
			SipPeerCertificateSubjectName = 3221489300U,
			UploadDtmfMapMetadataFailed = 2147747477U,
			DownloadDtmfMapMetadataFailed,
			MobileSpeechRecoClientAsyncCallTimedOut = 3221489303U,
			UnableToAccessOrganizationMailbox,
			SetScaleOutCapabilityFailed,
			GrammarFileMaxEntriesExceeded = 2147747482U,
			GrammarFileMaxCountExceeded,
			MRASMediaChannelEstablishFailed,
			WatsoningDueToTimeout,
			WatsoningDueToWorkerProcessNotTerminating
		}
	}
}
