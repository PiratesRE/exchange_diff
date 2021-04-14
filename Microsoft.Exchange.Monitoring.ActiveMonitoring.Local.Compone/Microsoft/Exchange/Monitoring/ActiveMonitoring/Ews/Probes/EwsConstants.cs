using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Ews.Probes
{
	public class EwsConstants
	{
		public static string LocalProbeEndpoint
		{
			get
			{
				return Uri.UriSchemeHttps + "://localhost/";
			}
		}

		public static string AutodiscoverSvcEndpoint
		{
			get
			{
				return EwsConstants.LocalProbeEndpoint.TrimEnd(new char[]
				{
					'/'
				}) + "/autodiscover/autodiscover.svc";
			}
		}

		public static string AutodiscoverXmlEndpoint
		{
			get
			{
				return EwsConstants.LocalProbeEndpoint.TrimEnd(new char[]
				{
					'/'
				}) + "/autodiscover/autodiscover.xml";
			}
		}

		public static string EwsEndpoint
		{
			get
			{
				return EwsConstants.LocalProbeEndpoint.TrimEnd(new char[]
				{
					'/'
				}) + "/ews/exchange.asmx";
			}
		}

		public const int DefaultProbeRecurrenceIntervalSeconds = 300;

		public const int DefaultMonitoringIntervalSeconds = 1800;

		public const int DefaultMonitoringRecurrenceIntervalSeconds = 0;

		public const int DefaultApiRetryCount = 1;

		public const int DefaultApiRetrySleep = 5000;

		public const int MaxSyncItemsCount = 512;

		public const int MinimumTimeLimit = 5000;

		public const int DefaultUnhealthyTransitionSpanInMinutes = 20;

		public const int DefaultUnrecoverableTransitionSpanInMinutes = 20;

		public const int DefaultDegradedTransitionSpanInMinutes = 0;

		public const int DefaultIISRecycleRetryCount = 1;

		public const int DefaultIISRecycleRetrySpanInSeconds = 30;

		public const int DefaultFailedProbeThreshold = 4;

		public const int DefaultProbeTimeoutSpanInSeconds = 20;

		public const string DefaultOperationName = "ConvertId";

		public const string DefaultServerRole = "Mailbox";

		public const string DefaultProbeName = "EwsProtocolSelfTest";

		public const string DefaultUserAgentPart = "AMProbe";

		public const string ClientStatisticsHeader = "X-ClientStatistics";

		public const string GetFolderOperationName = "GetFolder";

		public const string SendEmailOperationName = "SendEmail";

		public const string ConvertIdOperationName = "ConvertId";

		public const string RequestIdHeader = "RequestId";

		public const string TrustAnySslCertificateAttributeName = "TrustAnySslCertificate";

		public const string PrimaryAuthNAttributeName = "PrimaryAuthN";

		public const string IsOutsideInMonitoringAttributeName = "IsOutsideInMonitoring";

		public const string ExchangeSkuAttributeName = "ExchangeSku";

		public const string TargetPortAttributeName = "TargetPort";

		public const string VerboseAttributeName = "Verbose";

		public const string ApiRetryCountAttributeName = "ApiRetryCount";

		public const string ApiRetrySleepInMillisecondsAttributeName = "ApiRetrySleepInMilliseconds";

		public const string UseXropEndPointAttributeName = "UseXropEndPoint";

		public const string DomainAttributeName = "Domain";

		public const string UserAgentPartAttributeName = "UserAgentPart";

		public const string OperationNameAttributeName = "OperationName";

		public const string IncludeExchangeRpcUrlAttributeName = "IncludeExchangeRpcUrl";

		public const string EWSDeepTestProbeName = "EWSDeepTest";

		public const string EWSSelfTestProbeName = "EWSSelfTest";

		public const string EWSCtpTestProbeName = "EWSCtpTest";

		public const string AutodiscoverSelfTestProbeName = "AutodiscoverSelfTest";

		public const string AutodiscoverCtpTestProbeName = "AutodiscoverCtpTest";

		public const string MSExchangeAutoDiscoverAppPoolName = "MSExchangeAutoDiscoverAppPool";

		public const string MSExchangeServicesAppPoolName = "MSExchangeServicesAppPool";

		public const string ServerRoleAttributeName = "ServerRole";

		public const string ProbeNameAttributeName = "ProbeType";

		public const string ProbeRecurrenceSpanAttributeName = "ProbeRecurrenceSpan";

		public const string ProbeTimeoutSpanAttributeName = "ProbeTimeoutSpan";

		public const string MonitoringIntervalSpanAttributeName = "MonitoringIntervalSpan";

		public const string MonitoringRecurrenceIntervalSpanAttributeName = "MonitoringRecurrenceIntervalSpan";

		public const string DegradedTransitionSpanAttributeName = "DegradedTransitionSpan";

		public const string UnhealthyTransitionSpanAttributeName = "UnhealthyTransitionSpan";

		public const string UnrecoverableTransitionSpanAttributeName = "UnrecoverableTransitionSpan";

		public const string IISRecycleRetryCountAttributeName = "IISRecycleRetryCount";

		public const string IISRecycleRetrySpanAttributeName = "IISRecycleRetrySpan";

		public const string FailedProbeThresholdAttributeName = "FailedProbeThreshold";

		public const string IISRecycleResponderEnabledAttributeName = "IISRecycleResponderEnabled";

		public const string FailoverResponderEnabledAttributeName = "FailoverResponderEnabled";

		public const string AlertResponderEnabledAttributeName = "AlertResponderEnabled";

		public const string EnablePagedAlertsAttributeName = "EnablePagedAlerts";

		public const string CreateRespondersForTestAttributeName = "CreateRespondersForTest";

		public const string BaseNameAttributeName = "BaseName";

		public const string Name = "Name";

		public const string ServiceName = "ServiceName";

		public const string TargetResource = "TargetResource";

		public const string Account = "Account";

		public const string Password = "Password";

		public const string Endpoint = "Endpoint";

		public const string TimeoutSeconds = "TimeoutSeconds";
	}
}
