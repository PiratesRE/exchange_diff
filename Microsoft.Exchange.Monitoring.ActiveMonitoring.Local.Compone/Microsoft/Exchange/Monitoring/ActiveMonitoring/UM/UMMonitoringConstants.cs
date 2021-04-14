using System;
using System.Reflection;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	public abstract class UMMonitoringConstants
	{
		public const string AlertTypeIdFormat = "Exchange/UM/{0}";

		public const string MsDiagnostics = "ms-diagnostics";

		public const string MsDiagnosticsPublic = "ms-diagnostics-public";

		public const int E14RedirectProvisionalResponseCode = 15637;

		public const int E15RedirectProvisionalResponseCode = 15643;

		public const int E14CallReceivedProvisionalResponseCode = 15638;

		public const int E15CallReceivedProvisionalResponseCode = 15644;

		public const int ExUMLyncServerProvisionalResponseCodeMin = 15000;

		public const int ExUMLyncServerProvisionalResponseCodeMax = 15499;

		public const int ExUMExchangeServerProvisionalResponseCodeMin = 15500;

		public const int ExUMExchangeServerProvisionalResponseCodeMax = 15899;

		public const int DiagnosticsTraceCode = 15900;

		public const int CallStartCode = 15901;

		public const int CallEstablishingCode = 15902;

		public const int CallEstablishedCode = 15903;

		public const int CallEstablishFailedCode = 15904;

		public const int CallDisconnectedCode = 15905;

		public const int CallAudioReceivedCode = 15906;

		public const int StateAttributeLength = 1024;

		public const string MsFe = "ms-fe";

		public const string WildCard = "*";

		public const string UMServiceTypeParameterName = "UMServiceType";

		public const string UMSipTransportParameterName = "UMSipTransport";

		public const string UMMediaProtocolParameterName = "UMMediaProtocol";

		public const string UMCertificateThumbprintParameterName = "UMCertificateThumbprint";

		public const string UMCertificateSubjectNameParameterName = "UMCertificateSubjectName";

		public const string UMSipListeningPortParameterName = "UMSipListeningPort";

		public const string UMSrcAccountExtensionParameterName = "SrcAccountExtension";

		public const string UMDestAccountExtensionParameterName = "DestAccountExtension";

		public const string UMSrcAccountSipUriParameterName = "SrcAccountSipUri";

		public const string DestAccountSipUriParameterName = "DestAccountSipUri";

		public const string UMDestAccountTenantDomainParameterName = "DestAccountTenantDomain";

		public const string UMActiveMonitoringCertificateSubjectName = "um.o365.exchangemon.net";

		public const string UMServerAddressForOutsideInProbeParameterName = "DestAccountGatewayForwardingAddress";

		public const string UMServiceAddressForLocalMonitoringProbeParameterName = "UMServiceAddress";

		public const string MaxCallsReached = "15500";

		public const string NoWorkerProcess = "15501";

		public const string DiskspaceFull = "15503";

		public const string TransientError = "15604";

		public const string ConnectionFailed = "ConnectionFailed";

		public const string CertificateNotConfigured = "CertificateNotConfigured";

		public const string CertificateMissing = "CertificateMissing";

		public const string OneBoxDifferentCertificateConfiguredOnUMServices = "OneBoxDifferentCertificateConfiguredOnUMServices";

		public const string WildCardInCertificateSubjectName = "WildCardInCertificateSubjectName";

		public const string Localhost = "localhost";

		public const string UMCallRouterTestProbe = "UMCallRouterTestProbe";

		public const string UMCallRouterTestMonitor = "UMCallRouterTestMonitor";

		public const string UMCallRouterTestEscalate = "UMCallRouterTestEscalate";

		public const string UMCallRouterTestRestart = "UMCallRouterTestRestart";

		public const string UMCallRouterTestOffline = "UMCallRouterTestOffline";

		public const string UMSelfTestProbe = "UMSelfTestProbe";

		public const string UMSelfTestMonitor = "UMSelfTestMonitor";

		public const string UMSelfTestRestart = "UMSelfTestRestart";

		public const string UMSelfTestWithoutRecoveryEscalate = "UMSelfTestWithoutRecoveryEscalate";

		public const string UMSelfTestEscalate = "UMSelfTestEscalate";

		public const string UMCallRouterRecentMissedCallNotificationProxyFailedMonitor = "UMCallRouterRecentMissedCallNotificationProxyFailedMonitor";

		public const string UMCallRouterRecentMissedCallNotificationProxyFailedEscalate = "UMCallRouterRecentMissedCallNotificationProxyFailedEscalate";

		public const string UMRecentPartnerTranscriptionFailedMonitor = "UMServiceRecentPartnerTranscriptionFailedMonitor";

		public const string UMRecentPartnerTranscriptionFailedEscalate = "UMServiceRecentPartnerTranscriptionFailedEscalate";

		public const string UMPipelineSLAMonitor = "UMPipelineSLAMonitor";

		public const string UMPipelineSLAEscalate = "UMPipelineSLAEscalate";

		public const string UMPipelineFullMonitor = "UMPipelineFullMonitor";

		public const string UMPipelineFullEscalate = "UMPipelineFullEscalate";

		public const string MediaEstablishedFailedMonitor = "MediaEstablishedFailedMonitor";

		public const string MediaEstablishedFailedEscalate = "MediaEstablishedFailedEscalate";

		public const string MediaEdgeAuthenticationServiceCredentialsAcquisitionFailedMonitor = "MediaEdgeAuthenticationServiceCredentialsAcquisitionFailedMonitor";

		public const string MediaEdgeAuthenticationServiceCredentialsAcquisitionFailedEscalate = "MediaEdgeAuthenticationServiceCredentialsAcquisitionFailedEscalate";

		public const string MediaEdgeResourceAllocationFailedMonitor = "MediaEdgeResourceAllocationFailedMonitor";

		public const string MediaEdgeResourceAllocationFailedEscalate = "MediaEdgeResourceAllocationFailedEscalate";

		public const string UMCertificateNearExpiryMonitor = "UMCertificateNearExpiryMonitor";

		public const string UMCertificateNearExpiryEscalate = "UMCertificateNearExpiryEscalate";

		public const string UMCallRouterCertificateNearExpiryMonitor = "UMCallRouterCertificateNearExpiryMonitor";

		public const string UMCallRouterCertificateNearExpiryEscalate = "UMCallRouterCertificateNearExpiryEscalate";

		public const string UMProtectedVoiceMessageEncryptDecryptFailedMonitor = "UMProtectedVoiceMessageEncryptDecryptFailedMonitor";

		public const string UMProtectedVoiceMessageEncryptDecryptFailedEscalate = "UMProtectedVoiceMessageEncryptDecryptFailedEscalate";

		public const string UMTranscriptionThrottledMonitor = "UMTranscriptionThrottledMonitor";

		public const string UMTranscriptionThrottledEscalate = "UMTranscriptionThrottledEscalate";

		public const string UMGrammarUsageMonitor = "UMGrammarUsageMonitor";

		public const string UMGrammarUsageEscalate = "UMGrammarUsageEscalate";

		public const int TimeToWaitForMedia = 30;

		public static readonly string UMCallRouterHealthSet = ExchangeComponent.UMCallRouter.Name;

		public static readonly string UMProtocolHealthSet = ExchangeComponent.UMProtocol.Name;

		public static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		public static readonly string UmEscalationTeam = ExchangeComponent.UMCallRouter.EscalationTeam;
	}
}
