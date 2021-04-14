using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MessagingPolicies
{
	public static class MessagingPoliciesEventLogConstants
	{
		public const string EventSource = "MSExchange Messaging Policies";

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_JournalingRulesLoaded = new ExEventLog.EventTuple(1074004968U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_JournalingRulesConfigurationLoadError = new ExEventLog.EventTuple(3221488617U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_JournalingDroppingJournalReportToDCMailbox = new ExEventLog.EventTuple(2147746794U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UnJournalingPermanentError = new ExEventLog.EventTuple(3221488619U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UnJournalingTransientError = new ExEventLog.EventTuple(2147746796U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_JournalFilterTransportSettingLoadError = new ExEventLog.EventTuple(3221488621U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_JournalingTargetDGEmptyError = new ExEventLog.EventTuple(3221488622U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_JournalingTargetDGNotFoundError = new ExEventLog.EventTuple(3221488623U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_JournalingLITenantNotFoundInResourceForestError = new ExEventLog.EventTuple(3221488625U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_JournalingLogConfigureError = new ExEventLog.EventTuple(3221488626U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_JournalingLogException = new ExEventLog.EventTuple(3221488627U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AttachFilterConfigLoaded = new ExEventLog.EventTuple(1074005968U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AttachFilterConfigCorrupt = new ExEventLog.EventTuple(3221489617U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AddressRewriteConfigLoaded = new ExEventLog.EventTuple(1074006968U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AddressRewriteConfigCorrupt = new ExEventLog.EventTuple(3221490617U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RuleActionLogEvent = new ExEventLog.EventTuple(1074007968U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RuleCollectionLoaded = new ExEventLog.EventTuple(1074007970U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RuleCollectionLoadingTransientError = new ExEventLog.EventTuple(2147749795U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RuleCollectionLoadingError = new ExEventLog.EventTuple(3221491620U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RuleLoadTimeExceededThreshold = new ExEventLog.EventTuple(2147749797U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RuleExecutionTimeExceededThreshold = new ExEventLog.EventTuple(2147749798U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RuleEvaluationFailure = new ExEventLog.EventTuple(3221491623U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RuleEvaluationFilteringServiceFailure = new ExEventLog.EventTuple(3221491624U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RuleEvaluationIgnoredFailure = new ExEventLog.EventTuple(3221491625U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RuleEvaluationIgnoredFilteringServiceFailure = new ExEventLog.EventTuple(3221491626U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RuleDetectedExcessiveBifurcation = new ExEventLog.EventTuple(3221491627U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ApplyPolicyOperationNDRedDueToEncryptionOff = new ExEventLog.EventTuple(3221231473U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ApplyPolicyOperationFailNDR = new ExEventLog.EventTuple(3221231475U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ApplyPolicyOperationFailDefer = new ExEventLog.EventTuple(2147489653U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SkippedDecryptionForMaliciousTargetAddress = new ExEventLog.EventTuple(2147752793U, 7, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToLoadDrmMessage = new ExEventLog.EventTuple(3221232480U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RmsGeneralFailure = new ExEventLog.EventTuple(3221233472U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RmsSpecialFailure = new ExEventLog.EventTuple(3221233473U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RmsConnectFailure = new ExEventLog.EventTuple(3221233474U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RmsTrustFailure = new ExEventLog.EventTuple(3221233475U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_Rms401Failure = new ExEventLog.EventTuple(3221233476U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_Rms403Failure = new ExEventLog.EventTuple(3221233477U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_Rms404Failure = new ExEventLog.EventTuple(3221233478U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RmsNoRightFailure = new ExEventLog.EventTuple(3221233479U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToLoadIRMConfiguration = new ExEventLog.EventTuple(3221233672U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TransportDecryptionSucceeded = new ExEventLog.EventTuple(1074012169U, 9, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TransportDecryptionTransientException = new ExEventLog.EventTuple(3221495818U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TransportDecryptionPermanentException = new ExEventLog.EventTuple(3221495819U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TransportReEncryptionFailed = new ExEventLog.EventTuple(3221495821U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TransportReEncryptionFailedInvalidPLOrUL = new ExEventLog.EventTuple(3221495822U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RedirectionAgentCreated = new ExEventLog.EventTuple(1073750031U, 10, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidForwardingSmtpAddressError = new ExEventLog.EventTuple(3221233680U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TemplateDoesNotExist = new ExEventLog.EventTuple(3221236472U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		private enum Category : short
		{
			Journaling = 1,
			AttachFilter,
			AddressRewrite,
			Rules,
			Prelicensing,
			PolicyApplication,
			JournalReportDecryption,
			RightsManagement,
			TransportDecryption,
			RedirectionAgent,
			Information_Rights_Management
		}

		internal enum Message : uint
		{
			JournalingRulesLoaded = 1074004968U,
			JournalingRulesConfigurationLoadError = 3221488617U,
			JournalingDroppingJournalReportToDCMailbox = 2147746794U,
			UnJournalingPermanentError = 3221488619U,
			UnJournalingTransientError = 2147746796U,
			JournalFilterTransportSettingLoadError = 3221488621U,
			JournalingTargetDGEmptyError,
			JournalingTargetDGNotFoundError,
			JournalingLITenantNotFoundInResourceForestError = 3221488625U,
			JournalingLogConfigureError,
			JournalingLogException,
			AttachFilterConfigLoaded = 1074005968U,
			AttachFilterConfigCorrupt = 3221489617U,
			AddressRewriteConfigLoaded = 1074006968U,
			AddressRewriteConfigCorrupt = 3221490617U,
			RuleActionLogEvent = 1074007968U,
			RuleCollectionLoaded = 1074007970U,
			RuleCollectionLoadingTransientError = 2147749795U,
			RuleCollectionLoadingError = 3221491620U,
			RuleLoadTimeExceededThreshold = 2147749797U,
			RuleExecutionTimeExceededThreshold,
			RuleEvaluationFailure = 3221491623U,
			RuleEvaluationFilteringServiceFailure,
			RuleEvaluationIgnoredFailure,
			RuleEvaluationIgnoredFilteringServiceFailure,
			RuleDetectedExcessiveBifurcation,
			ApplyPolicyOperationNDRedDueToEncryptionOff = 3221231473U,
			ApplyPolicyOperationFailNDR = 3221231475U,
			ApplyPolicyOperationFailDefer = 2147489653U,
			SkippedDecryptionForMaliciousTargetAddress = 2147752793U,
			FailedToLoadDrmMessage = 3221232480U,
			RmsGeneralFailure = 3221233472U,
			RmsSpecialFailure,
			RmsConnectFailure,
			RmsTrustFailure,
			Rms401Failure,
			Rms403Failure,
			Rms404Failure,
			RmsNoRightFailure,
			FailedToLoadIRMConfiguration = 3221233672U,
			TransportDecryptionSucceeded = 1074012169U,
			TransportDecryptionTransientException = 3221495818U,
			TransportDecryptionPermanentException,
			TransportReEncryptionFailed = 3221495821U,
			TransportReEncryptionFailedInvalidPLOrUL,
			RedirectionAgentCreated = 1073750031U,
			InvalidForwardingSmtpAddressError = 3221233680U,
			TemplateDoesNotExist = 3221236472U
		}
	}
}
