using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Diagnostics
{
	public static class CommonEventLogConstants
	{
		public const string EventSource = "MSExchange Common";

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ExchangeCrash = new ExEventLog.EventTuple(1074008967U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WatsonReportError = new ExEventLog.EventTuple(2147750790U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PrivilegeRemovalFailure = new ExEventLog.EventTuple(3221488195U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NoAdapterDnsServerList = new ExEventLog.EventTuple(3221487821U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DnsQueryReturnedPartialResults = new ExEventLog.EventTuple(2147745998U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_NoMachineDnsServerList = new ExEventLog.EventTuple(3221487823U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DnsServerConfigurationFetchFailed = new ExEventLog.EventTuple(3221487824U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PerfCounterProblem = new ExEventLog.EventTuple(3221487722U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TraceConfigFileAccessDeniedProblem = new ExEventLog.EventTuple(3221488022U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiskProblem = new ExEventLog.EventTuple(3221488122U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiskHealthy = new ExEventLog.EventTuple(1074004473U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LatencyDetection = new ExEventLog.EventTuple(2147746299U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DeleteOldLog = new ExEventLog.EventTuple(1074009969U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DeleteLogDueToQuota = new ExEventLog.EventTuple(1074009970U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToCreateDirectory = new ExEventLog.EventTuple(3221493619U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToAppendLog = new ExEventLog.EventTuple(3221493620U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PendingLoggingRequestsReachedMaximum = new ExEventLog.EventTuple(2147751797U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SslCertificateTrustError = new ExEventLog.EventTuple(3221230475U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RightsAccountCertificateTrustError = new ExEventLog.EventTuple(3221230476U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RightsManagementServerVersionError = new ExEventLog.EventTuple(3221230477U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RightsManagementServerAuthenticationError = new ExEventLog.EventTuple(3221230478U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RightsManagementServerAuthorizationError = new ExEventLog.EventTuple(3221230479U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RightsManagementServerResourceNotFound = new ExEventLog.EventTuple(3221230480U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RightsManagementServerNameResolutionError = new ExEventLog.EventTuple(3221230481U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RightsManagementServerDecommissioned = new ExEventLog.EventTuple(3221230482U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RightsManagementServerRedirected = new ExEventLog.EventTuple(3221230483U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UnknownTemplate = new ExEventLog.EventTuple(3221230485U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_IssuanceLicenseTrustError = new ExEventLog.EventTuple(3221230486U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorOpeningLanguagePackRegistryKey = new ExEventLog.EventTuple(3221226472U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorScanningLanguagePackFolders = new ExEventLog.EventTuple(3221226572U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidCultureIdentifier = new ExEventLog.EventTuple(2147484848U, 5, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FederationTrustOrganizationCertificateNotFound = new ExEventLog.EventTuple(3221225873U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FederationTrustOrganizationCertificateNoPrivateKey = new ExEventLog.EventTuple(3221225874U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FederationTrustCertificateExpired = new ExEventLog.EventTuple(3221225875U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TenantMonitoringWorkflowError = new ExEventLog.EventTuple(3221231472U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TenantMonitoringTestEvent = new ExEventLog.EventTuple(3221231473U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EscalationLoopTestRedEvent = new ExEventLog.EventTuple(1073748825U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EscalationLoopTestYellowEvent = new ExEventLog.EventTuple(1073748826U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CannotContactMserveCacheService = new ExEventLog.EventTuple(3221495617U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MserveCacheServiceModeChanged = new ExEventLog.EventTuple(1074011970U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WcfClientConfigError = new ExEventLog.EventTuple(3221233475U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ActivityRollupReportWithUsageEvent = new ExEventLog.EventTuple(1073748827U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ActivityRollupReportWithNoUsageEvent = new ExEventLog.EventTuple(1073748828U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorOpeningUMLanguagePackRegistryKey = new ExEventLog.EventTuple(3221232477U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RulesBasedHttpModule_InvalidRuleConfigured = new ExEventLog.EventTuple(3221233472U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RulesBasedHttpModule_UserAccessDenied = new ExEventLog.EventTuple(3221233473U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DeploymentF5ConnectToFloaterErrorEvent = new ExEventLog.EventTuple(3221234472U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DeploymentF5ConnectToPartnerErrorEvent = new ExEventLog.EventTuple(3221234473U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DeploymentF5PartnerErrorEvent = new ExEventLog.EventTuple(3221234474U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_StreamInsightsDataUploaderGetValueFailed = new ExEventLog.EventTuple(2147492651U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OutOfMemory = new ExEventLog.EventTuple(3221245473U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnhandledException = new ExEventLog.EventTuple(3221225473U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NonCrashingException = new ExEventLog.EventTuple(3221225474U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AppSettingLoadException = new ExEventLog.EventTuple(3221225475U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		private enum Category : short
		{
			General = 1,
			Configuration,
			Logging,
			RightsManagement,
			LanguagePackInfo,
			TenantMonitoring,
			Deployment
		}

		internal enum Message : uint
		{
			ExchangeCrash = 1074008967U,
			WatsonReportError = 2147750790U,
			PrivilegeRemovalFailure = 3221488195U,
			NoAdapterDnsServerList = 3221487821U,
			DnsQueryReturnedPartialResults = 2147745998U,
			NoMachineDnsServerList = 3221487823U,
			DnsServerConfigurationFetchFailed,
			PerfCounterProblem = 3221487722U,
			TraceConfigFileAccessDeniedProblem = 3221488022U,
			DiskProblem = 3221488122U,
			DiskHealthy = 1074004473U,
			LatencyDetection = 2147746299U,
			DeleteOldLog = 1074009969U,
			DeleteLogDueToQuota,
			FailedToCreateDirectory = 3221493619U,
			FailedToAppendLog,
			PendingLoggingRequestsReachedMaximum = 2147751797U,
			SslCertificateTrustError = 3221230475U,
			RightsAccountCertificateTrustError,
			RightsManagementServerVersionError,
			RightsManagementServerAuthenticationError,
			RightsManagementServerAuthorizationError,
			RightsManagementServerResourceNotFound,
			RightsManagementServerNameResolutionError,
			RightsManagementServerDecommissioned,
			RightsManagementServerRedirected,
			UnknownTemplate = 3221230485U,
			IssuanceLicenseTrustError,
			ErrorOpeningLanguagePackRegistryKey = 3221226472U,
			ErrorScanningLanguagePackFolders = 3221226572U,
			InvalidCultureIdentifier = 2147484848U,
			FederationTrustOrganizationCertificateNotFound = 3221225873U,
			FederationTrustOrganizationCertificateNoPrivateKey,
			FederationTrustCertificateExpired,
			TenantMonitoringWorkflowError = 3221231472U,
			TenantMonitoringTestEvent,
			EscalationLoopTestRedEvent = 1073748825U,
			EscalationLoopTestYellowEvent,
			CannotContactMserveCacheService = 3221495617U,
			MserveCacheServiceModeChanged = 1074011970U,
			WcfClientConfigError = 3221233475U,
			ActivityRollupReportWithUsageEvent = 1073748827U,
			ActivityRollupReportWithNoUsageEvent,
			ErrorOpeningUMLanguagePackRegistryKey = 3221232477U,
			RulesBasedHttpModule_InvalidRuleConfigured = 3221233472U,
			RulesBasedHttpModule_UserAccessDenied,
			DeploymentF5ConnectToFloaterErrorEvent = 3221234472U,
			DeploymentF5ConnectToPartnerErrorEvent,
			DeploymentF5PartnerErrorEvent,
			StreamInsightsDataUploaderGetValueFailed = 2147492651U,
			OutOfMemory = 3221245473U,
			UnhandledException = 3221225473U,
			NonCrashingException,
			AppSettingLoadException
		}
	}
}
