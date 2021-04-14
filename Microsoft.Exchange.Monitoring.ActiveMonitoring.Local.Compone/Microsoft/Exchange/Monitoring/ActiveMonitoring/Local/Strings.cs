using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(2286371297U, "DatabaseGuidNotSupplied");
			Strings.stringIDs.Add(1189391965U, "OABGenTenantOutOfSLABody");
			Strings.stringIDs.Add(52276049U, "SearchFailToSaveMessage");
			Strings.stringIDs.Add(3960338262U, "ForwardSyncHaltEscalationSubject");
			Strings.stringIDs.Add(312538007U, "MaintenanceFailureEscalationMessage");
			Strings.stringIDs.Add(299573369U, "DatabaseSpaceHelpString");
			Strings.stringIDs.Add(683259685U, "PumServiceNotRunningEscalationMessage");
			Strings.stringIDs.Add(1378981546U, "HealthSetMaintenanceEscalationSubjectPrefix");
			Strings.stringIDs.Add(2358296594U, "RegisterDnsHostRecordResponderName");
			Strings.stringIDs.Add(2482686375U, "RcaDiscoveryOutlookAnywhereNotFound");
			Strings.stringIDs.Add(3458357958U, "UnableToCompleteTopologyEscalationMessage");
			Strings.stringIDs.Add(945891153U, "LargestDeliveryQueueLengthEscalationMessage");
			Strings.stringIDs.Add(1718676120U, "DSNotifyQueueHigh15MinutesEscalationMessage");
			Strings.stringIDs.Add(2503653255U, "OutStandingATQRequests15MinutesEscalationMessage");
			Strings.stringIDs.Add(2592211666U, "ADDatabaseCorruption1017EscalationMessage");
			Strings.stringIDs.Add(2321911502U, "DeviceDegradedEscalationMessage");
			Strings.stringIDs.Add(1918560769U, "RemoteDomainControllerStateEscalationMessage");
			Strings.stringIDs.Add(164779627U, "OWACalendarAppPoolEscalationBody");
			Strings.stringIDs.Add(1218627587U, "MediaEstablishedFailedEscalationMessage");
			Strings.stringIDs.Add(1980518279U, "RequestForFfoApprovalToOfflineFailed");
			Strings.stringIDs.Add(3706262484U, "InsufficientInformationKCCEscalationMessage");
			Strings.stringIDs.Add(2150007296U, "ClusterNodeEvictedEscalationMessage");
			Strings.stringIDs.Add(635314528U, "OwaOutsideInDatabaseAvailabilityFailuresSubject");
			Strings.stringIDs.Add(112230113U, "RouteTableRecoveryResponderName");
			Strings.stringIDs.Add(2257888472U, "AssistantsActiveDatabaseSubject");
			Strings.stringIDs.Add(3750243459U, "ForwardSyncMonopolizedEscalationSubject");
			Strings.stringIDs.Add(805912300U, "UMProtectedVoiceMessageEncryptDecryptFailedEscalationMessage");
			Strings.stringIDs.Add(1299295308U, "SearchIndexFailureEscalationMessage");
			Strings.stringIDs.Add(4294224569U, "ForwardSyncCookieNotUpToDateEscalationMessage");
			Strings.stringIDs.Add(2092011713U, "CannotBootEscalationMessage");
			Strings.stringIDs.Add(2288602703U, "PassiveReplicationPerformanceCounterProbeEscalationMessage");
			Strings.stringIDs.Add(3399715728U, "OwaTooManyStartPageFailuresSubject");
			Strings.stringIDs.Add(4261321224U, "OwaOutsideInDatabaseAvailabilityFailuresBody");
			Strings.stringIDs.Add(409381696U, "SearchWordBreakerLoadingFailureEscalationMessage");
			Strings.stringIDs.Add(741999051U, "Pop3CommandProcessingTimeEscalationMessage");
			Strings.stringIDs.Add(3741312692U, "DeltaSyncEndpointUnreachableEscalationMessage");
			Strings.stringIDs.Add(1119726172U, "EventLogProbeRedEvents");
			Strings.stringIDs.Add(109013642U, "ProvisioningBigVolumeErrorProbeName");
			Strings.stringIDs.Add(2897164042U, "PassiveADReplicationMonitorEscalationMessage");
			Strings.stringIDs.Add(373161856U, "UMCertificateThumbprint");
			Strings.stringIDs.Add(893346260U, "ForwardSyncMonopolizedEscalationMessage");
			Strings.stringIDs.Add(2704062871U, "NoResponseHeadersAvailable");
			Strings.stringIDs.Add(380917770U, "AdminAuditingAvailabilityFailureEscalationSubject");
			Strings.stringIDs.Add(1943611390U, "DeltaSyncPartnerAuthenticationFailedEscalationMessage");
			Strings.stringIDs.Add(1893530126U, "SharedCacheEscalationSubject");
			Strings.stringIDs.Add(3957270050U, "JournalingEscalationSubject");
			Strings.stringIDs.Add(2370864531U, "HighProcessor15MinutesEscalationMessage");
			Strings.stringIDs.Add(3308508053U, "NetworkAdapterRssEscalationMessage");
			Strings.stringIDs.Add(1482699764U, "CPUOverThresholdErrorEscalationSubject");
			Strings.stringIDs.Add(1218523202U, "Transport80thPercentileMissingSLAEscalationMessage");
			Strings.stringIDs.Add(2302596019U, "InferenceTrainingSLAEscalationMessage");
			Strings.stringIDs.Add(2517955710U, "EDiscoveryEscalationBodyEntText");
			Strings.stringIDs.Add(1451525307U, "AsynchronousAuditSearchAvailabilityFailureEscalationSubject");
			Strings.stringIDs.Add(3795580654U, "SearchRopNotSupportedEscalationMessage");
			Strings.stringIDs.Add(909096326U, "PushNotificationEnterpriseUnknownError");
			Strings.stringIDs.Add(610566341U, "OwaClientAccessRoleNotInstalled");
			Strings.stringIDs.Add(3645413289U, "BridgeHeadReplicationEscalationMessage");
			Strings.stringIDs.Add(2836474835U, "PushNotificationEnterpriseNotConfigured");
			Strings.stringIDs.Add(892806076U, "IncompatibleVectorEscalationMessage");
			Strings.stringIDs.Add(4138453910U, "DatabaseCorruptionEscalationMessage");
			Strings.stringIDs.Add(1752264565U, "ReplicationOutdatedObjectsFailedEscalationMessage");
			Strings.stringIDs.Add(443970804U, "DatabaseCorruptEscalationMessage");
			Strings.stringIDs.Add(48294403U, "HealthSetAlertSuppressionWarning");
			Strings.stringIDs.Add(1065939221U, "OwaIMInitializationFailedMessage");
			Strings.stringIDs.Add(2172476143U, "ForwardSyncHaltEscalationMessage");
			Strings.stringIDs.Add(1679303927U, "OfflineGLSEscalationMessage");
			Strings.stringIDs.Add(2914700647U, "UnableToRunEscalateByDatabaseHealthResponder");
			Strings.stringIDs.Add(2514146180U, "AggregateDeliveryQueueLengthEscalationMessage");
			Strings.stringIDs.Add(4070157535U, "NoCafeMonitoringAccountsAvailable");
			Strings.stringIDs.Add(2950938112U, "MediaEdgeResourceAllocationFailedEscalationMessage");
			Strings.stringIDs.Add(1420198520U, "DRAPendingReplication5MinutesEscalationMessage");
			Strings.stringIDs.Add(357692756U, "SchemaPartitionFailedEscalationMessage");
			Strings.stringIDs.Add(2064178893U, "DatabaseSchemaVersionCheckEscalationSubject");
			Strings.stringIDs.Add(878340914U, "UMSipListeningPort");
			Strings.stringIDs.Add(3803135725U, "ELCMailboxSLAEscalationSubject");
			Strings.stringIDs.Add(1586522085U, "DHCPNacksEscalationMessage");
			Strings.stringIDs.Add(1507797344U, "ELCArchiveDumpsterEscalationMessage");
			Strings.stringIDs.Add(3628981090U, "KDCServiceStatusTestMessage");
			Strings.stringIDs.Add(401186895U, "LowMemoryUnderThresholdErrorEscalationSubject");
			Strings.stringIDs.Add(1108409616U, "OwaIMInitializationFailedSubject");
			Strings.stringIDs.Add(3717637996U, "PingConnectivityEscalationSubject");
			Strings.stringIDs.Add(529762034U, "PublicFolderConnectionCountEscalationMessage");
			Strings.stringIDs.Add(663711086U, "FastNodeNotHealthyEscalationMessage");
			Strings.stringIDs.Add(1307224852U, "CheckDCMMDivergenceScriptExceptionMessage");
			Strings.stringIDs.Add(1220266742U, "CrossPremiseMailflowEscalationMessage");
			Strings.stringIDs.Add(2945963449U, "ForwardSyncStandardCompanyEscalationSubject");
			Strings.stringIDs.Add(1462332256U, "JournalArchiveEscalationSubject");
			Strings.stringIDs.Add(3746115424U, "DoMTConnectivityEscalateMessage");
			Strings.stringIDs.Add(680617032U, "InferenceComponentDisabledEscalationMessage");
			Strings.stringIDs.Add(2776049552U, "NoBackendMonitoringAccountsAvailable");
			Strings.stringIDs.Add(349561476U, "ActiveDirectoryConnectivityEscalationMessage");
			Strings.stringIDs.Add(4196713157U, "SyntheticReplicationTransactionEscalationMessage");
			Strings.stringIDs.Add(814146069U, "OabFileLoadExceptionEncounteredSubject");
			Strings.stringIDs.Add(4268033680U, "RegistryAccessDeniedEscalationMessage");
			Strings.stringIDs.Add(2465131700U, "AuditLogSearchServiceletEscalationSubject");
			Strings.stringIDs.Add(649999061U, "EventLogProbeLogName");
			Strings.stringIDs.Add(536159405U, "Imap4ProtocolUnhealthy");
			Strings.stringIDs.Add(1179543515U, "DLExpansionEscalationMessage");
			Strings.stringIDs.Add(2046186369U, "ReplicationFailuresEscalationMessage");
			Strings.stringIDs.Add(1915337724U, "SCTStateMonitoringScriptExceptionMessage");
			Strings.stringIDs.Add(1417399775U, "ELCExceptionEscalationMessage");
			Strings.stringIDs.Add(1844700225U, "OabTooManyHttpErrorResponsesEncounteredBody");
			Strings.stringIDs.Add(275645054U, "QuarantineEscalationMessage");
			Strings.stringIDs.Add(3642796412U, "TransportRejectingMessageSubmissions");
			Strings.stringIDs.Add(3898201799U, "PublicFolderConnectionCountEscalationSubject");
			Strings.stringIDs.Add(2285670747U, "PowerShellProfileEscalationSubject");
			Strings.stringIDs.Add(93821081U, "DivergenceBetweenCAAndAD1006EscalationMessage");
			Strings.stringIDs.Add(726991111U, "UnreachableQueueLengthEscalationMessage");
			Strings.stringIDs.Add(3639475733U, "OabFileLoadExceptionEncounteredBody");
			Strings.stringIDs.Add(252047361U, "PublicFolderSyncEscalationSubject");
			Strings.stringIDs.Add(4139459438U, "Imap4CommandProcessingTimeEscalationMessage");
			Strings.stringIDs.Add(3378952345U, "InvalidSearchResultsExceptionMessage");
			Strings.stringIDs.Add(4272242116U, "SearchInformationNotAvailable");
			Strings.stringIDs.Add(2844472229U, "ActiveDatabaseAvailabilityEscalationSubject");
			Strings.stringIDs.Add(952182041U, "ELCPermanentEscalationSubject");
			Strings.stringIDs.Add(1117652268U, "EventLogProbeGreenEvents");
			Strings.stringIDs.Add(4155705872U, "ClusterHangEscalationMessage");
			Strings.stringIDs.Add(891001936U, "FEPServiceNotRunningEscalationMessage");
			Strings.stringIDs.Add(286684653U, "RidMonitorEscalationMessage");
			Strings.stringIDs.Add(2576630513U, "SystemMailboxGuidNotFound");
			Strings.stringIDs.Add(2727074534U, "MediaEdgeAuthenticationServiceCredentialsAcquisitionFailedEscalationMessage");
			Strings.stringIDs.Add(3448175994U, "SearchTransportAgentFailureEscalationMessage");
			Strings.stringIDs.Add(2551706401U, "TransportMessageCategorizationEscalationMessage");
			Strings.stringIDs.Add(2595326046U, "InocrrectSCTStateExceptionMessage");
			Strings.stringIDs.Add(1789741911U, "DataIssueEscalationMessage");
			Strings.stringIDs.Add(3993818915U, "KerbAuthFailureEscalationMessagPAC");
			Strings.stringIDs.Add(423349970U, "DivergenceInDefinitionEscalationMessage");
			Strings.stringIDs.Add(2198917190U, "MobilityAccount");
			Strings.stringIDs.Add(682587060U, "OwaTooManyLogoffFailuresBody");
			Strings.stringIDs.Add(770708540U, "ForwardSyncProcessRepeatedlyCrashingEscalationSubject");
			Strings.stringIDs.Add(1169357891U, "ADDatabaseCorruptionEscalationMessage");
			Strings.stringIDs.Add(580917981U, "MailboxAuditingAvailabilityFailureEscalationSubject");
			Strings.stringIDs.Add(1969709639U, "TopologyServiceConnectivityEscalationMessage");
			Strings.stringIDs.Add(4160621849U, "UMSipTransport");
			Strings.stringIDs.Add(2022121183U, "OabProtocolEscalationBody");
			Strings.stringIDs.Add(3048659136U, "PushNotificationEnterpriseEmptyServiceUri");
			Strings.stringIDs.Add(3214050158U, "PushNotificationEnterpriseAuthError");
			Strings.stringIDs.Add(2021347314U, "UnableToRunAlertNotificationTypeByDatabaseCopyStateResponder");
			Strings.stringIDs.Add(1920206617U, "ELCDumpsterWarningEscalationSubject");
			Strings.stringIDs.Add(2542492469U, "OabMailboxEscalationBody");
			Strings.stringIDs.Add(3640747412U, "CheckZombieDCEscalateMessage");
			Strings.stringIDs.Add(2206456195U, "RidSetMonitorEscalationMessage");
			Strings.stringIDs.Add(584392819U, "PushNotificationCafeEndpointUnhealthy");
			Strings.stringIDs.Add(632484951U, "ProvisioningBigVolumeErrorEscalationMessage");
			Strings.stringIDs.Add(58433467U, "PublicFolderMailboxQuotaEscalationMessage");
			Strings.stringIDs.Add(1077120006U, "CASRoutingFailureEscalationSubject");
			Strings.stringIDs.Add(3549972347U, "OAuthRequestFailureEscalationBody");
			Strings.stringIDs.Add(3456628766U, "GLSEscalationMessage");
			Strings.stringIDs.Add(2809892364U, "SCTNotFoundForAllVersionsExceptionMessage");
			Strings.stringIDs.Add(1613949426U, "SqlOutputStreamInRetryEscalationMessage");
			Strings.stringIDs.Add(3213813466U, "DefaultEscalationSubject");
			Strings.stringIDs.Add(471095525U, "MailboxAuditingAvailabilityFailureEscalationBody");
			Strings.stringIDs.Add(1276311330U, "BulkProvisioningNoProgressEscalationSubject");
			Strings.stringIDs.Add(1377854022U, "InfrastructureValidationSubject");
			Strings.stringIDs.Add(1194398797U, "SearchMemoryUsageOverThresholdEscalationMessage");
			Strings.stringIDs.Add(105667215U, "SharedCacheEscalationMessage");
			Strings.stringIDs.Add(799661977U, "CannotRecoverEscalationMessage");
			Strings.stringIDs.Add(4214982447U, "AsynchronousAuditSearchAvailabilityFailureEscalationBody");
			Strings.stringIDs.Add(483050246U, "OwaIMLogAnalyzerMessage");
			Strings.stringIDs.Add(1005998838U, "UMCertificateSubjectName");
			Strings.stringIDs.Add(1108329015U, "OwaNoMailboxesAvailable");
			Strings.stringIDs.Add(2516924014U, "TransportCategorizerJobsUnavailableEscalationMessage");
			Strings.stringIDs.Add(910032993U, "SingleAvailableDatabaseCopyEscalationMessage");
			Strings.stringIDs.Add(1345149923U, "DivergenceInSiteNameEscalationMessage");
			Strings.stringIDs.Add(3893081264U, "NullSearchResponseExceptionMessage");
			Strings.stringIDs.Add(144342951U, "UncategorizedProcess");
			Strings.stringIDs.Add(3984209328U, "MSExchangeProtectedServiceHostCrashingMessage");
			Strings.stringIDs.Add(3400380005U, "UMDatacenterLoadBalancerSipOptionsPingEscalationMessage");
			Strings.stringIDs.Add(564297537U, "PassiveReplicationMonitorEscalationMessage");
			Strings.stringIDs.Add(4039996833U, "ReinstallServerEscalationMessage");
			Strings.stringIDs.Add(2217573206U, "ForwardSyncCookieNotUpToDateEscalationSubject");
			Strings.stringIDs.Add(2260546867U, "ActiveDirectoryConnectivityLocalEscalationMessage");
			Strings.stringIDs.Add(70227012U, "PassiveDatabaseAvailabilityEscalationSubject");
			Strings.stringIDs.Add(3278262633U, "OabTooManyHttpErrorResponsesEncounteredSubject");
			Strings.stringIDs.Add(2089742391U, "ELCTransientEscalationSubject");
			Strings.stringIDs.Add(1923047497U, "SiteFailureEscalationMessage");
			Strings.stringIDs.Add(1012424075U, "OnlineMeetingCreateEscalationBody");
			Strings.stringIDs.Add(462239732U, "SiteMailboxDocumentSyncEscalationSubject");
			Strings.stringIDs.Add(4075662140U, "NTDSCorruptionEscalationMessage");
			Strings.stringIDs.Add(3665176336U, "TopoDiscoveryFailedAllServersEscalationMessage");
			Strings.stringIDs.Add(400522478U, "VersionStore1479EscalationMessage");
			Strings.stringIDs.Add(868604876U, "AssistantsNotRunningToCompletionSubject");
			Strings.stringIDs.Add(1817218534U, "AdminAuditingAvailabilityFailureEscalationBody");
			Strings.stringIDs.Add(3226571974U, "ForwardSyncLiteCompanyEscalationSubject");
			Strings.stringIDs.Add(4083150976U, "MSExchangeInformationStoreCannotContactADEscalationMessage");
			Strings.stringIDs.Add(465857804U, "DHCPServerRequestsEscalationMessage");
			Strings.stringIDs.Add(4092609939U, "NoNTDSObjectEscalationMessage");
			Strings.stringIDs.Add(88349250U, "PublicFolderMoveJobStuckEscalationSubject");
			Strings.stringIDs.Add(4169066611U, "SCTMonitoringScriptExceptionMessage");
			Strings.stringIDs.Add(380251366U, "ProvisionedDCBelowMinimumEscalationMessage");
			Strings.stringIDs.Add(3378026954U, "KerbAuthFailureEscalationMessage");
			Strings.stringIDs.Add(1277128402U, "RequestsQueuedOver500EscalationMessage");
			Strings.stringIDs.Add(2497401637U, "PopImapGuid");
			Strings.stringIDs.Add(77538240U, "MaintenanceFailureEscalationSubject");
			Strings.stringIDs.Add(1577351776U, "TransportServerDownEscalationMessage");
			Strings.stringIDs.Add(3174826533U, "PopImapEndpoint");
			Strings.stringIDs.Add(3229931132U, "NtlmConnectivityEscalationMessage");
			Strings.stringIDs.Add(499742697U, "OabTooManyWebAppStartsSubject");
			Strings.stringIDs.Add(3922382486U, "CafeEscalationSubjectUnhealthy");
			Strings.stringIDs.Add(2676970837U, "DnsHostRecordProbeName");
			Strings.stringIDs.Add(1037389911U, "ELCArchiveDumpsterWarningEscalationSubject");
			Strings.stringIDs.Add(1536257732U, "JournalFilterAgentEscalationMessage");
			Strings.stringIDs.Add(1098709463U, "WacDiscoveryFailureSubject");
			Strings.stringIDs.Add(887962606U, "CASRoutingLatencyEscalationSubject");
			Strings.stringIDs.Add(1060163078U, "EDSJobPoisonedEscalationMessage");
			Strings.stringIDs.Add(938822773U, "JournalFilterAgentEscalationSubject");
			Strings.stringIDs.Add(3660582759U, "DnsHostRecordMonitorName");
			Strings.stringIDs.Add(3238760785U, "VersionStore2008EscalationMessage");
			Strings.stringIDs.Add(2695821343U, "DnsServiceMonitorName");
			Strings.stringIDs.Add(3687026854U, "DatabaseAvailabilityHelpString");
			Strings.stringIDs.Add(3465747486U, "PublicFolderMailboxQuotaEscalationSubject");
			Strings.stringIDs.Add(1074585207U, "HealthSetEscalationSubjectPrefix");
			Strings.stringIDs.Add(4054742343U, "DHCPServerDeclinesEscalationMessage");
			Strings.stringIDs.Add(235731656U, "TrustMonitorProbeEscalationMessage");
			Strings.stringIDs.Add(959637577U, "InvalidIncludedAssistantType");
			Strings.stringIDs.Add(861211960U, "ReplicationDisabledEscalationMessage");
			Strings.stringIDs.Add(2731022862U, "ProvisioningBigVolumeErrorEscalateResponderName");
			Strings.stringIDs.Add(3881757534U, "CASRoutingLatencyEscalationBody");
			Strings.stringIDs.Add(1221520849U, "SecurityAlertMalwareDetectedEscalationMessage");
			Strings.stringIDs.Add(1306675352U, "SynchronousAuditSearchAvailabilityFailureEscalationBody");
			Strings.stringIDs.Add(4173100116U, "MaintenanceTimeoutEscalationMessage");
			Strings.stringIDs.Add(217815433U, "DeltaSyncServiceEndpointsLoadFailedEscalationMessage");
			Strings.stringIDs.Add(269594253U, "SchedulingLatencyEscalateResponderMessage");
			Strings.stringIDs.Add(4073533312U, "PowerShellProfileEscalationMessage");
			Strings.stringIDs.Add(3913093087U, "OAuthRequestFailureEscalationSubject");
			Strings.stringIDs.Add(1166689995U, "OabMailboxNoOrgMailbox");
			Strings.stringIDs.Add(3054327783U, "PushNotificationEnterpriseEmptyDomain");
			Strings.stringIDs.Add(3814043621U, "AssistantsOutOfSlaMessage");
			Strings.stringIDs.Add(3546783417U, "EwsAutodEscalationSubjectUnhealthy");
			Strings.stringIDs.Add(1293802800U, "HttpConnectivityEscalationSubject");
			Strings.stringIDs.Add(4157781316U, "DatabaseObjectNotFoundException");
			Strings.stringIDs.Add(2661017855U, "FSMODCNotProvisionedEscalationMessage");
			Strings.stringIDs.Add(3444300625U, "AssistantsNotRunningToCompletionMessage");
			Strings.stringIDs.Add(1281304358U, "DLExpansionEscalationSubject");
			Strings.stringIDs.Add(2407301436U, "RcaTaskOutlineFailed");
			Strings.stringIDs.Add(3706272566U, "CASRoutingFailureEscalationBody");
			Strings.stringIDs.Add(2189832741U, "DnsServiceProbeName");
			Strings.stringIDs.Add(2148760356U, "SynchronousAuditSearchAvailabilityFailureEscalationSubject");
			Strings.stringIDs.Add(4134231892U, "CannotRebuildIndexEscalationMessage");
			Strings.stringIDs.Add(3667272656U, "UMPipelineFullEscalationMessageString");
			Strings.stringIDs.Add(1880452434U, "DatabaseNotAttachedReadOnly");
			Strings.stringIDs.Add(2815102519U, "InfrastructureValidationMessage");
			Strings.stringIDs.Add(977800858U, "OwaTooManyHttpErrorResponsesEncounteredSubject");
			Strings.stringIDs.Add(3387090034U, "CPUOverThresholdWarningEscalationSubject");
			Strings.stringIDs.Add(35045070U, "SubscriptionSlaMissedEscalationMessage");
			Strings.stringIDs.Add(1680117983U, "ActiveDirectoryConnectivityConfigDCEscalationMessage");
			Strings.stringIDs.Add(3785224032U, "OwaMailboxRoleNotInstalled");
			Strings.stringIDs.Add(421810782U, "PushNotificationEnterpriseNetworkingError");
			Strings.stringIDs.Add(1548802155U, "DatabaseAvailabilityTimeout");
			Strings.stringIDs.Add(843177689U, "DatabaseGuidNotFound");
			Strings.stringIDs.Add(678296020U, "SearchIndexBacklogAggregatedEscalationMessage");
			Strings.stringIDs.Add(2793575470U, "PublicFolderLocalEWSLogonEscalationMessage");
			Strings.stringIDs.Add(3926584359U, "TenantRelocationErrorsFoundExceptionMessage");
			Strings.stringIDs.Add(3506328701U, "UMCallRouterCertificateNearExpiryEscalationMessage");
			Strings.stringIDs.Add(136972698U, "SlowADWritesEscalationMessage");
			Strings.stringIDs.Add(2850892442U, "RcaEscalationBodyEnt");
			Strings.stringIDs.Add(2395533582U, "MailboxDatabasesUnavailable");
			Strings.stringIDs.Add(515794671U, "RetryRemoteDeliveryQueueLengthEscalationMessage");
			Strings.stringIDs.Add(3544714230U, "FailedToUpgradeIndexEscalationMessage");
			Strings.stringIDs.Add(18446274U, "EventAssistantsWatermarksHelpString");
			Strings.stringIDs.Add(1165995200U, "InferenceClassifcationSLAEscalationMessage");
			Strings.stringIDs.Add(4059291937U, "MRSUnhealthyMessage");
			Strings.stringIDs.Add(3867394701U, "UMServiceType");
			Strings.stringIDs.Add(365492232U, "DivergenceBetweenCAAndAD1003EscalationMessage");
			Strings.stringIDs.Add(182386565U, "AssistantsActiveDatabaseMessage");
			Strings.stringIDs.Add(1990015256U, "SchedulingLatencyEscalateResponderSubject");
			Strings.stringIDs.Add(3218355606U, "OfficeGraphTransportDeliveryAgentFailureEscalationMessage");
			Strings.stringIDs.Add(1104137584U, "OfficeGraphMessageTracingPluginFailureEscalationMessage");
			Strings.stringIDs.Add(424604115U, "LogicalDiskFreeMegabytesEscalationMessage");
			Strings.stringIDs.Add(1920697843U, "OwaIMLogAnalyzerSubject");
			Strings.stringIDs.Add(3515405004U, "RaidDegradedEscalationMessage");
			Strings.stringIDs.Add(1236475651U, "BulkProvisioningNoProgressEscalationMessage");
			Strings.stringIDs.Add(1313509642U, "AssistantsOutOfSlaSubject");
			Strings.stringIDs.Add(1818757842U, "OwaTooManyHttpErrorResponsesEncounteredBody");
			Strings.stringIDs.Add(2102184447U, "CheckSumEscalationMessage");
			Strings.stringIDs.Add(3498323379U, "PublicFolderLocalEWSLogonEscalationSubject");
			Strings.stringIDs.Add(776931395U, "UMServerAddress");
			Strings.stringIDs.Add(3415559268U, "CafeArrayNameCouldNotBeRetrieved");
			Strings.stringIDs.Add(3465381739U, "EDSServiceNotRunningEscalationMessage");
			Strings.stringIDs.Add(2618809902U, "SearchFailToCheckNodeState");
			Strings.stringIDs.Add(2406788764U, "OwaTooManyStartPageFailuresBody");
			Strings.stringIDs.Add(1518847271U, "QuarantineEscalationSubject");
			Strings.stringIDs.Add(1941315341U, "HostControllerServiceRunningMessage");
			Strings.stringIDs.Add(3578257008U, "SearchGracefulDegradationManagerFailureEscalationMessage");
			Strings.stringIDs.Add(46842886U, "ProvisioningBigVolumeErrorMonitorName");
			Strings.stringIDs.Add(3672282944U, "OwaTooManyWebAppStartsSubject");
			Strings.stringIDs.Add(1495785996U, "SearchQueryStxSimpleQueryMode");
			Strings.stringIDs.Add(3956529557U, "OABGenTenantOutOfSLASubject");
			Strings.stringIDs.Add(951276022U, "ELCDumpsterEscalationMessage");
			Strings.stringIDs.Add(1559006762U, "DirectoryConfigDiscrepancyEscalationMessage");
			Strings.stringIDs.Add(1496058059U, "NetworkAdapterRecoveryResponderName");
			Strings.stringIDs.Add(898751369U, "SearchGracefulDegradationStatusEscalationMessage");
			Strings.stringIDs.Add(3135153224U, "DnsServiceRestartResponderName");
			Strings.stringIDs.Add(3845606054U, "ELCMailboxSLAEscalationMessage");
			Strings.stringIDs.Add(1456195959U, "JournalingEscalationMessage");
			Strings.stringIDs.Add(1598120667U, "MaintenanceTimeoutEscalationSubject");
			Strings.stringIDs.Add(3433261884U, "ContentsUnpredictableEscalationMessage");
			Strings.stringIDs.Add(3788119207U, "EscalationSubjectUnhealthy");
			Strings.stringIDs.Add(859758172U, "AsyncAuditLogSearchEscalationSubject");
			Strings.stringIDs.Add(706708545U, "DefaultEscalationMessage");
			Strings.stringIDs.Add(2480657645U, "SyntheticReplicationMonitorEscalationMessage");
			Strings.stringIDs.Add(31173904U, "EDiscoveryEscalationBodyDCHTML");
			Strings.stringIDs.Add(1499273528U, "OwaTooManyLogoffFailuresSubject");
			Strings.stringIDs.Add(4220476005U, "CheckProvisionedDCExceptionMessage");
			Strings.stringIDs.Add(3913310860U, "ProvisioningBigVolumeErrorEscalationSubject");
			Strings.stringIDs.Add(3906074550U, "UMCertificateNearExpiryEscalationMessage");
			Strings.stringIDs.Add(182285359U, "FailureItemMessageForNTFSCorruption");
			Strings.stringIDs.Add(821565004U, "DatabaseRPCLatencyMonitorGreenMessage");
			Strings.stringIDs.Add(1705255289U, "RelocationServicePermanentExceptionMessage");
			Strings.stringIDs.Add(3730111670U, "LiveIdAuthenticationEscalationMesage");
			Strings.stringIDs.Add(3256369209U, "JournalArchiveEscalationMessage");
			Strings.stringIDs.Add(1746979174U, "Pop3ProtocolUnhealthy");
			Strings.stringIDs.Add(408002753U, "HxServiceEscalationMessageUnhealthy");
			Strings.stringIDs.Add(2258779484U, "RequestForNewRidPoolFailedEscalationMessage");
			Strings.stringIDs.Add(2903867061U, "DatabaseSizeEscalationSubject");
			Strings.stringIDs.Add(4196623762U, "OabMailboxManifestEmpty");
			Strings.stringIDs.Add(1260554753U, "CheckFsmoRolesScriptExceptionMessage");
			Strings.stringIDs.Add(1084277863U, "PopImapSecondaryEndpoint");
			Strings.stringIDs.Add(3639485535U, "CannotFunctionNormallyEscalationMessage");
			Strings.stringIDs.Add(116423715U, "EscalationMessageHealthy");
			Strings.stringIDs.Add(2944547573U, "PublicFolderMoveJobStuckEscalationMessage");
			Strings.stringIDs.Add(667399815U, "SearchServiceNotRunningEscalationMessage");
			Strings.stringIDs.Add(1455075163U, "MobilityAccountPassword");
			Strings.stringIDs.Add(3746052928U, "EventLogProbeProviderName");
			Strings.stringIDs.Add(1122579724U, "VersionStore623EscalationMessage");
		}

		public static LocalizedString DatabaseGuidNotSupplied
		{
			get
			{
				return new LocalizedString("DatabaseGuidNotSupplied", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuarantinedMailboxEscalationMessageEnt(string databaseName)
		{
			return new LocalizedString("QuarantinedMailboxEscalationMessageEnt", Strings.ResourceManager, new object[]
			{
				databaseName
			});
		}

		public static LocalizedString OABGenTenantOutOfSLABody
		{
			get
			{
				return new LocalizedString("OABGenTenantOutOfSLABody", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PopSelfTestEscalationBodyDC(string serverName, string probeName)
		{
			return new LocalizedString("PopSelfTestEscalationBodyDC", Strings.ResourceManager, new object[]
			{
				serverName,
				probeName
			});
		}

		public static LocalizedString SearchFailToSaveMessage
		{
			get
			{
				return new LocalizedString("SearchFailToSaveMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OneCopyMonitorFailureEscalationSubject(string component, string machine, int threshold)
		{
			return new LocalizedString("OneCopyMonitorFailureEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine,
				threshold
			});
		}

		public static LocalizedString CircularLoggingDisabledEscalationMessage(string database)
		{
			return new LocalizedString("CircularLoggingDisabledEscalationMessage", Strings.ResourceManager, new object[]
			{
				database
			});
		}

		public static LocalizedString MailSubmissionBehindWatermarksEscalationMessageEnt(TimeSpan ageThreshold, TimeSpan duration, string databaseName, string invokeNowCommand, string unhealthyMonitorsCommand)
		{
			return new LocalizedString("MailSubmissionBehindWatermarksEscalationMessageEnt", Strings.ResourceManager, new object[]
			{
				ageThreshold,
				duration,
				databaseName,
				invokeNowCommand,
				unhealthyMonitorsCommand
			});
		}

		public static LocalizedString ForwardSyncHaltEscalationSubject
		{
			get
			{
				return new LocalizedString("ForwardSyncHaltEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MaintenanceFailureEscalationMessage
		{
			get
			{
				return new LocalizedString("MaintenanceFailureEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseSpaceHelpString
		{
			get
			{
				return new LocalizedString("DatabaseSpaceHelpString", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PumServiceNotRunningEscalationMessage
		{
			get
			{
				return new LocalizedString("PumServiceNotRunningEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HealthSetMaintenanceEscalationSubjectPrefix
		{
			get
			{
				return new LocalizedString("HealthSetMaintenanceEscalationSubjectPrefix", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InsufficientRedundancyEscalationSubject(string component, string machine)
		{
			return new LocalizedString("InsufficientRedundancyEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine
			});
		}

		public static LocalizedString RegisterDnsHostRecordResponderName
		{
			get
			{
				return new LocalizedString("RegisterDnsHostRecordResponderName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LagCopyHealthProblemEscalationSubject(string component, string machine, string database)
		{
			return new LocalizedString("LagCopyHealthProblemEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine,
				database
			});
		}

		public static LocalizedString RcaDiscoveryOutlookAnywhereNotFound
		{
			get
			{
				return new LocalizedString("RcaDiscoveryOutlookAnywhereNotFound", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ActiveDatabaseAvailabilityEscalationMessageEnt(string invokeNowCommand, string unhealthyMonitorsCommand)
		{
			return new LocalizedString("ActiveDatabaseAvailabilityEscalationMessageEnt", Strings.ResourceManager, new object[]
			{
				invokeNowCommand,
				unhealthyMonitorsCommand
			});
		}

		public static LocalizedString StoreAdminRPCInterfaceEscalationEscalationMessageEnt(TimeSpan duration)
		{
			return new LocalizedString("StoreAdminRPCInterfaceEscalationEscalationMessageEnt", Strings.ResourceManager, new object[]
			{
				duration
			});
		}

		public static LocalizedString UnableToCompleteTopologyEscalationMessage
		{
			get
			{
				return new LocalizedString("UnableToCompleteTopologyEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LargestDeliveryQueueLengthEscalationMessage
		{
			get
			{
				return new LocalizedString("LargestDeliveryQueueLengthEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DSNotifyQueueHigh15MinutesEscalationMessage
		{
			get
			{
				return new LocalizedString("DSNotifyQueueHigh15MinutesEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InfrastructureValidationError(string error)
		{
			return new LocalizedString("InfrastructureValidationError", Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString OutStandingATQRequests15MinutesEscalationMessage
		{
			get
			{
				return new LocalizedString("OutStandingATQRequests15MinutesEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADDatabaseCorruption1017EscalationMessage
		{
			get
			{
				return new LocalizedString("ADDatabaseCorruption1017EscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceDegradedEscalationMessage
		{
			get
			{
				return new LocalizedString("DeviceDegradedEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AvailabilityServiceEscalationHtmlBody(string monitorName)
		{
			return new LocalizedString("AvailabilityServiceEscalationHtmlBody", Strings.ResourceManager, new object[]
			{
				monitorName
			});
		}

		public static LocalizedString RwsDatamartConnectionEscalationBody(string serverName)
		{
			return new LocalizedString("RwsDatamartConnectionEscalationBody", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString ServiceNotRunningEscalationMessage(string serviceName)
		{
			return new LocalizedString("ServiceNotRunningEscalationMessage", Strings.ResourceManager, new object[]
			{
				serviceName
			});
		}

		public static LocalizedString DatabaseLocationNotFoundException(string databaseGuid)
		{
			return new LocalizedString("DatabaseLocationNotFoundException", Strings.ResourceManager, new object[]
			{
				databaseGuid
			});
		}

		public static LocalizedString OwaCustomerTouchPointEscalationHtmlBody(string serverName, string logPath)
		{
			return new LocalizedString("OwaCustomerTouchPointEscalationHtmlBody", Strings.ResourceManager, new object[]
			{
				serverName,
				logPath
			});
		}

		public static LocalizedString RemoteDomainControllerStateEscalationMessage
		{
			get
			{
				return new LocalizedString("RemoteDomainControllerStateEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EwsAutodSelfTestEscalationRecoveryDetails(string appPool)
		{
			return new LocalizedString("EwsAutodSelfTestEscalationRecoveryDetails", Strings.ResourceManager, new object[]
			{
				appPool
			});
		}

		public static LocalizedString MRSServiceNotRunningSubject(string service)
		{
			return new LocalizedString("MRSServiceNotRunningSubject", Strings.ResourceManager, new object[]
			{
				service
			});
		}

		public static LocalizedString OWACalendarAppPoolEscalationBody
		{
			get
			{
				return new LocalizedString("OWACalendarAppPoolEscalationBody", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchQueryStxSuccess(int hits, string query, string mailboxSmtpAddress)
		{
			return new LocalizedString("SearchQueryStxSuccess", Strings.ResourceManager, new object[]
			{
				hits,
				query,
				mailboxSmtpAddress
			});
		}

		public static LocalizedString QuarantinedMailboxEscalationMessageDc(string databaseName)
		{
			return new LocalizedString("QuarantinedMailboxEscalationMessageDc", Strings.ResourceManager, new object[]
			{
				databaseName
			});
		}

		public static LocalizedString MediaEstablishedFailedEscalationMessage
		{
			get
			{
				return new LocalizedString("MediaEstablishedFailedEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequestForFfoApprovalToOfflineFailed
		{
			get
			{
				return new LocalizedString("RequestForFfoApprovalToOfflineFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InsufficientInformationKCCEscalationMessage
		{
			get
			{
				return new LocalizedString("InsufficientInformationKCCEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClusterNodeEvictedEscalationMessage
		{
			get
			{
				return new LocalizedString("ClusterNodeEvictedEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OwaOutsideInDatabaseAvailabilityFailuresSubject
		{
			get
			{
				return new LocalizedString("OwaOutsideInDatabaseAvailabilityFailuresSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RouteTableRecoveryResponderName
		{
			get
			{
				return new LocalizedString("RouteTableRecoveryResponderName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeCrashExceededErrorThresholdMessage(string processName)
		{
			return new LocalizedString("ExchangeCrashExceededErrorThresholdMessage", Strings.ResourceManager, new object[]
			{
				processName
			});
		}

		public static LocalizedString AssistantsActiveDatabaseSubject
		{
			get
			{
				return new LocalizedString("AssistantsActiveDatabaseSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseRepeatedMountsEscalationMessage(string databaseName, TimeSpan duration)
		{
			return new LocalizedString("DatabaseRepeatedMountsEscalationMessage", Strings.ResourceManager, new object[]
			{
				databaseName,
				duration
			});
		}

		public static LocalizedString OnlineMeetingCreateEscalationSubject(string serverName)
		{
			return new LocalizedString("OnlineMeetingCreateEscalationSubject", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString ForwardSyncMonopolizedEscalationSubject
		{
			get
			{
				return new LocalizedString("ForwardSyncMonopolizedEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LocalMachineDriveEncryptionLockEscalationMessage(string volumes, string serverName)
		{
			return new LocalizedString("LocalMachineDriveEncryptionLockEscalationMessage", Strings.ResourceManager, new object[]
			{
				volumes,
				serverName
			});
		}

		public static LocalizedString UMProtectedVoiceMessageEncryptDecryptFailedEscalationMessage
		{
			get
			{
				return new LocalizedString("UMProtectedVoiceMessageEncryptDecryptFailedEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchIndexFailureEscalationMessage
		{
			get
			{
				return new LocalizedString("SearchIndexFailureEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ForwardSyncCookieNotUpToDateEscalationMessage
		{
			get
			{
				return new LocalizedString("ForwardSyncCookieNotUpToDateEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DbFailureItemIoHardEscalationSubject(string component, string machine, string dbCopy)
		{
			return new LocalizedString("DbFailureItemIoHardEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine,
				dbCopy
			});
		}

		public static LocalizedString CannotBootEscalationMessage
		{
			get
			{
				return new LocalizedString("CannotBootEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogVolumeSpaceEscalationMessage(string database)
		{
			return new LocalizedString("LogVolumeSpaceEscalationMessage", Strings.ResourceManager, new object[]
			{
				database
			});
		}

		public static LocalizedString DatabaseSchemaVersionCheckEscalationMessageDc(string invokeNowCommand, string unhealthyMonitorsCommand)
		{
			return new LocalizedString("DatabaseSchemaVersionCheckEscalationMessageDc", Strings.ResourceManager, new object[]
			{
				invokeNowCommand,
				unhealthyMonitorsCommand
			});
		}

		public static LocalizedString ProcessCrashing(string serviceName, string server)
		{
			return new LocalizedString("ProcessCrashing", Strings.ResourceManager, new object[]
			{
				serviceName,
				server
			});
		}

		public static LocalizedString CafeEscalationRecoveryDetails(string appPool)
		{
			return new LocalizedString("CafeEscalationRecoveryDetails", Strings.ResourceManager, new object[]
			{
				appPool
			});
		}

		public static LocalizedString StoreProcessRepeatedlyCrashingEscalationMessageEnt(string processName, int count, TimeSpan duration)
		{
			return new LocalizedString("StoreProcessRepeatedlyCrashingEscalationMessageEnt", Strings.ResourceManager, new object[]
			{
				processName,
				count,
				duration
			});
		}

		public static LocalizedString PassiveReplicationPerformanceCounterProbeEscalationMessage
		{
			get
			{
				return new LocalizedString("PassiveReplicationPerformanceCounterProbeEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PrivateWorkingSetExceededErrorThresholdMessage(string processName)
		{
			return new LocalizedString("PrivateWorkingSetExceededErrorThresholdMessage", Strings.ResourceManager, new object[]
			{
				processName
			});
		}

		public static LocalizedString ProcessorTimeExceededWarningThresholdMessage(string processName)
		{
			return new LocalizedString("ProcessorTimeExceededWarningThresholdMessage", Strings.ResourceManager, new object[]
			{
				processName
			});
		}

		public static LocalizedString OwaTooManyStartPageFailuresSubject
		{
			get
			{
				return new LocalizedString("OwaTooManyStartPageFailuresSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OwaOutsideInDatabaseAvailabilityFailuresBody
		{
			get
			{
				return new LocalizedString("OwaOutsideInDatabaseAvailabilityFailuresBody", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseAvailabilityFailure(string database)
		{
			return new LocalizedString("DatabaseAvailabilityFailure", Strings.ResourceManager, new object[]
			{
				database
			});
		}

		public static LocalizedString EDiscoveryscalationSubject(string monitorIdentity, string target)
		{
			return new LocalizedString("EDiscoveryscalationSubject", Strings.ResourceManager, new object[]
			{
				monitorIdentity,
				target
			});
		}

		public static LocalizedString SearchWordBreakerLoadingFailureEscalationMessage
		{
			get
			{
				return new LocalizedString("SearchWordBreakerLoadingFailureEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Pop3CommandProcessingTimeEscalationMessage
		{
			get
			{
				return new LocalizedString("Pop3CommandProcessingTimeEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ParseDiagnosticsStringError(string error)
		{
			return new LocalizedString("ParseDiagnosticsStringError", Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString DeltaSyncEndpointUnreachableEscalationMessage
		{
			get
			{
				return new LocalizedString("DeltaSyncEndpointUnreachableEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventLogProbeRedEvents
		{
			get
			{
				return new LocalizedString("EventLogProbeRedEvents", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProvisioningBigVolumeErrorProbeName
		{
			get
			{
				return new LocalizedString("ProvisioningBigVolumeErrorProbeName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SystemDriveSpaceEscalationSubject(string component, string machine, string drive, string threshold)
		{
			return new LocalizedString("SystemDriveSpaceEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine,
				drive,
				threshold
			});
		}

		public static LocalizedString DatabaseConsistencyEscalationMessage(string databaseName)
		{
			return new LocalizedString("DatabaseConsistencyEscalationMessage", Strings.ResourceManager, new object[]
			{
				databaseName
			});
		}

		public static LocalizedString PassiveADReplicationMonitorEscalationMessage
		{
			get
			{
				return new LocalizedString("PassiveADReplicationMonitorEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GenericOverallXFailureEscalationMessage(string target)
		{
			return new LocalizedString("GenericOverallXFailureEscalationMessage", Strings.ResourceManager, new object[]
			{
				target
			});
		}

		public static LocalizedString UMCertificateThumbprint
		{
			get
			{
				return new LocalizedString("UMCertificateThumbprint", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ForwardSyncMonopolizedEscalationMessage
		{
			get
			{
				return new LocalizedString("ForwardSyncMonopolizedEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapProxyTestEscalationBodyDC(string serverName, string probeName)
		{
			return new LocalizedString("ImapProxyTestEscalationBodyDC", Strings.ResourceManager, new object[]
			{
				serverName,
				probeName
			});
		}

		public static LocalizedString PopSelfTestEscalationBodyENT(string serverName, string probeName)
		{
			return new LocalizedString("PopSelfTestEscalationBodyENT", Strings.ResourceManager, new object[]
			{
				serverName,
				probeName
			});
		}

		public static LocalizedString NoResponseHeadersAvailable
		{
			get
			{
				return new LocalizedString("NoResponseHeadersAvailable", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdminAuditingAvailabilityFailureEscalationSubject
		{
			get
			{
				return new LocalizedString("AdminAuditingAvailabilityFailureEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TransportSyncManagerServiceNotRunningEscalationMessage(string service)
		{
			return new LocalizedString("TransportSyncManagerServiceNotRunningEscalationMessage", Strings.ResourceManager, new object[]
			{
				service
			});
		}

		public static LocalizedString EscalationMessageUnhealthy(string customMessage)
		{
			return new LocalizedString("EscalationMessageUnhealthy", Strings.ResourceManager, new object[]
			{
				customMessage
			});
		}

		public static LocalizedString EacDeepTestEscalationSubject(string serverName)
		{
			return new LocalizedString("EacDeepTestEscalationSubject", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString InvokeNowDefinitionFailure(string requestId)
		{
			return new LocalizedString("InvokeNowDefinitionFailure", Strings.ResourceManager, new object[]
			{
				requestId
			});
		}

		public static LocalizedString ImapCustomerTouchPointEscalationBodyENT(string serverName, string probeName)
		{
			return new LocalizedString("ImapCustomerTouchPointEscalationBodyENT", Strings.ResourceManager, new object[]
			{
				serverName,
				probeName
			});
		}

		public static LocalizedString PushNotificationChannelError(string channelType, string channelName, string serverName)
		{
			return new LocalizedString("PushNotificationChannelError", Strings.ResourceManager, new object[]
			{
				channelType,
				channelName,
				serverName
			});
		}

		public static LocalizedString DeltaSyncPartnerAuthenticationFailedEscalationMessage
		{
			get
			{
				return new LocalizedString("DeltaSyncPartnerAuthenticationFailedEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DBAvailableButUnloadedByTransportSyncManagerMessage(string databaseName, string guid)
		{
			return new LocalizedString("DBAvailableButUnloadedByTransportSyncManagerMessage", Strings.ResourceManager, new object[]
			{
				databaseName,
				guid
			});
		}

		public static LocalizedString MonitoringAccountUnavailable(string mailboxDatabaseName)
		{
			return new LocalizedString("MonitoringAccountUnavailable", Strings.ResourceManager, new object[]
			{
				mailboxDatabaseName
			});
		}

		public static LocalizedString LocalDriveLogSpaceEscalationMessageDc(string drive, TimeSpan duration, string threshold)
		{
			return new LocalizedString("LocalDriveLogSpaceEscalationMessageDc", Strings.ResourceManager, new object[]
			{
				drive,
				duration,
				threshold
			});
		}

		public static LocalizedString SharedCacheEscalationSubject
		{
			get
			{
				return new LocalizedString("SharedCacheEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseRPCLatencyEscalationSubject(string databaseName, int latency, TimeSpan duration)
		{
			return new LocalizedString("DatabaseRPCLatencyEscalationSubject", Strings.ResourceManager, new object[]
			{
				databaseName,
				latency,
				duration
			});
		}

		public static LocalizedString LocalDriveLogSpaceEscalationMessageEnt(string drive, TimeSpan duration, string threshold)
		{
			return new LocalizedString("LocalDriveLogSpaceEscalationMessageEnt", Strings.ResourceManager, new object[]
			{
				drive,
				duration,
				threshold
			});
		}

		public static LocalizedString SearchQueryFailure(string databaseName, string failureRate, string threshold, string total, string successful, string errorEvents)
		{
			return new LocalizedString("SearchQueryFailure", Strings.ResourceManager, new object[]
			{
				databaseName,
				failureRate,
				threshold,
				total,
				successful,
				errorEvents
			});
		}

		public static LocalizedString EndpointManagerEndpointUninitialized(string name)
		{
			return new LocalizedString("EndpointManagerEndpointUninitialized", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString JournalingEscalationSubject
		{
			get
			{
				return new LocalizedString("JournalingEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HighProcessor15MinutesEscalationMessage
		{
			get
			{
				return new LocalizedString("HighProcessor15MinutesEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NetworkAdapterRssEscalationMessage
		{
			get
			{
				return new LocalizedString("NetworkAdapterRssEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationNotificationMessage(string notification)
		{
			return new LocalizedString("MigrationNotificationMessage", Strings.ResourceManager, new object[]
			{
				notification
			});
		}

		public static LocalizedString HostControllerServiceNodeExcessivePrivateBytes(string details)
		{
			return new LocalizedString("HostControllerServiceNodeExcessivePrivateBytes", Strings.ResourceManager, new object[]
			{
				details
			});
		}

		public static LocalizedString HostControllerServiceNodeExcessivePrivateBytesDetails(string nodeName, double thresholdGb, long actual)
		{
			return new LocalizedString("HostControllerServiceNodeExcessivePrivateBytesDetails", Strings.ResourceManager, new object[]
			{
				nodeName,
				thresholdGb,
				actual
			});
		}

		public static LocalizedString CPUOverThresholdErrorEscalationSubject
		{
			get
			{
				return new LocalizedString("CPUOverThresholdErrorEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchIndexServerCopyStatus(string message, string serverCopyStatus)
		{
			return new LocalizedString("SearchIndexServerCopyStatus", Strings.ResourceManager, new object[]
			{
				message,
				serverCopyStatus
			});
		}

		public static LocalizedString Transport80thPercentileMissingSLAEscalationMessage
		{
			get
			{
				return new LocalizedString("Transport80thPercentileMissingSLAEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InferenceTrainingSLAEscalationMessage
		{
			get
			{
				return new LocalizedString("InferenceTrainingSLAEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EDiscoveryEscalationBodyEntText
		{
			get
			{
				return new LocalizedString("EDiscoveryEscalationBodyEntText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AsynchronousAuditSearchAvailabilityFailureEscalationSubject
		{
			get
			{
				return new LocalizedString("AsynchronousAuditSearchAvailabilityFailureEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchActiveCopyUnhealthyEscalationMessage(string databaseName)
		{
			return new LocalizedString("SearchActiveCopyUnhealthyEscalationMessage", Strings.ResourceManager, new object[]
			{
				databaseName
			});
		}

		public static LocalizedString SearchRopNotSupportedEscalationMessage
		{
			get
			{
				return new LocalizedString("SearchRopNotSupportedEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PushNotificationEnterpriseUnknownError
		{
			get
			{
				return new LocalizedString("PushNotificationEnterpriseUnknownError", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OwaClientAccessRoleNotInstalled
		{
			get
			{
				return new LocalizedString("OwaClientAccessRoleNotInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BridgeHeadReplicationEscalationMessage
		{
			get
			{
				return new LocalizedString("BridgeHeadReplicationEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PushNotificationEnterpriseNotConfigured
		{
			get
			{
				return new LocalizedString("PushNotificationEnterpriseNotConfigured", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProcessRepeatedlyCrashingEscalationSubject(string processName)
		{
			return new LocalizedString("ProcessRepeatedlyCrashingEscalationSubject", Strings.ResourceManager, new object[]
			{
				processName
			});
		}

		public static LocalizedString UMCallRouterRecentCallRejectedMessageString(int percentageValue)
		{
			return new LocalizedString("UMCallRouterRecentCallRejectedMessageString", Strings.ResourceManager, new object[]
			{
				percentageValue
			});
		}

		public static LocalizedString OwaCustomerTouchPointEscalationBody(string serverName, string logPath)
		{
			return new LocalizedString("OwaCustomerTouchPointEscalationBody", Strings.ResourceManager, new object[]
			{
				serverName,
				logPath
			});
		}

		public static LocalizedString SearchIndexCrawlingNoProgress(string databaseName, string status, long numberOfDocumentsIndexedCrawler, DateTime startTime, DateTime endTime)
		{
			return new LocalizedString("SearchIndexCrawlingNoProgress", Strings.ResourceManager, new object[]
			{
				databaseName,
				status,
				numberOfDocumentsIndexedCrawler,
				startTime,
				endTime
			});
		}

		public static LocalizedString ActiveSyncDeepTestEscalationBodyDC(string serverName, string probeName)
		{
			return new LocalizedString("ActiveSyncDeepTestEscalationBodyDC", Strings.ResourceManager, new object[]
			{
				serverName,
				probeName
			});
		}

		public static LocalizedString IncompatibleVectorEscalationMessage
		{
			get
			{
				return new LocalizedString("IncompatibleVectorEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseCorruptionEscalationMessage
		{
			get
			{
				return new LocalizedString("DatabaseCorruptionEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReplicationOutdatedObjectsFailedEscalationMessage
		{
			get
			{
				return new LocalizedString("ReplicationOutdatedObjectsFailedEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ComponentHealthPercentFailureEscalationMessageUnhealthy(int percentFailureThreshold, int monitoringIntervalMinutes)
		{
			return new LocalizedString("ComponentHealthPercentFailureEscalationMessageUnhealthy", Strings.ResourceManager, new object[]
			{
				percentFailureThreshold,
				monitoringIntervalMinutes
			});
		}

		public static LocalizedString RwsDatamartAvailabilityEscalationSubject(string serverName, string cName)
		{
			return new LocalizedString("RwsDatamartAvailabilityEscalationSubject", Strings.ResourceManager, new object[]
			{
				serverName,
				cName
			});
		}

		public static LocalizedString CafeServerNotOwner(string mailboxDatabaseName, string upn)
		{
			return new LocalizedString("CafeServerNotOwner", Strings.ResourceManager, new object[]
			{
				mailboxDatabaseName,
				upn
			});
		}

		public static LocalizedString CafeOfflineFailedEscalationRecoveryDetails(string appPool)
		{
			return new LocalizedString("CafeOfflineFailedEscalationRecoveryDetails", Strings.ResourceManager, new object[]
			{
				appPool
			});
		}

		public static LocalizedString SearchIndexCopyUnhealthy(string databaseName, string status, string errorMessage, string diagnosticInfoError, string nodesInfo)
		{
			return new LocalizedString("SearchIndexCopyUnhealthy", Strings.ResourceManager, new object[]
			{
				databaseName,
				status,
				errorMessage,
				diagnosticInfoError,
				nodesInfo
			});
		}

		public static LocalizedString DatabaseCorruptEscalationMessage
		{
			get
			{
				return new LocalizedString("DatabaseCorruptEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HealthSetAlertSuppressionWarning
		{
			get
			{
				return new LocalizedString("HealthSetAlertSuppressionWarning", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchInstantSearchStxZeroHitMonitoringMailbox(string query, string mailboxSmtpAddress, string timestamp)
		{
			return new LocalizedString("SearchInstantSearchStxZeroHitMonitoringMailbox", Strings.ResourceManager, new object[]
			{
				query,
				mailboxSmtpAddress,
				timestamp
			});
		}

		public static LocalizedString ImapSelfTestEscalationBodyENT(string serverName, string probeName)
		{
			return new LocalizedString("ImapSelfTestEscalationBodyENT", Strings.ResourceManager, new object[]
			{
				serverName,
				probeName
			});
		}

		public static LocalizedString StalledCopyEscalationSubject(string component, string machine, string dbCopy, string threshold)
		{
			return new LocalizedString("StalledCopyEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine,
				dbCopy,
				threshold
			});
		}

		public static LocalizedString OwaIMInitializationFailedMessage
		{
			get
			{
				return new LocalizedString("OwaIMInitializationFailedMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InsufficientRedundancyEscalationMessage(string database)
		{
			return new LocalizedString("InsufficientRedundancyEscalationMessage", Strings.ResourceManager, new object[]
			{
				database
			});
		}

		public static LocalizedString DatabaseLogicalPhysicalSizeRatioEscalationMessageDc(double threshold, TimeSpan duration, string unhealthyMonitorsCommand)
		{
			return new LocalizedString("DatabaseLogicalPhysicalSizeRatioEscalationMessageDc", Strings.ResourceManager, new object[]
			{
				threshold,
				duration,
				unhealthyMonitorsCommand
			});
		}

		public static LocalizedString ForwardSyncHaltEscalationMessage
		{
			get
			{
				return new LocalizedString("ForwardSyncHaltEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LocalMachineDriveBootVolumeEncryptionStateEscalationMessage(string serverName)
		{
			return new LocalizedString("LocalMachineDriveBootVolumeEncryptionStateEscalationMessage", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString LocalMachineDriveNotProtectedWithDraEscalationMessage(string volumes, string serverName)
		{
			return new LocalizedString("LocalMachineDriveNotProtectedWithDraEscalationMessage", Strings.ResourceManager, new object[]
			{
				volumes,
				serverName
			});
		}

		public static LocalizedString ReplServiceCrashEscalationMessage(int times, int hour)
		{
			return new LocalizedString("ReplServiceCrashEscalationMessage", Strings.ResourceManager, new object[]
			{
				times,
				hour
			});
		}

		public static LocalizedString PassiveDatabaseAvailabilityEscalationMessageEnt(string invokeNowCommand, string unhealthyMonitorsCommand)
		{
			return new LocalizedString("PassiveDatabaseAvailabilityEscalationMessageEnt", Strings.ResourceManager, new object[]
			{
				invokeNowCommand,
				unhealthyMonitorsCommand
			});
		}

		public static LocalizedString OfflineGLSEscalationMessage
		{
			get
			{
				return new LocalizedString("OfflineGLSEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CircularLoggingDisabledEscalationSubject(string component, string machine, string database)
		{
			return new LocalizedString("CircularLoggingDisabledEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine,
				database
			});
		}

		public static LocalizedString UnableToRunEscalateByDatabaseHealthResponder
		{
			get
			{
				return new LocalizedString("UnableToRunEscalateByDatabaseHealthResponder", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AttributeMissingFromProbeDefinition(string attributeName)
		{
			return new LocalizedString("AttributeMissingFromProbeDefinition", Strings.ResourceManager, new object[]
			{
				attributeName
			});
		}

		public static LocalizedString UnableToGetDatabaseState(string database)
		{
			return new LocalizedString("UnableToGetDatabaseState", Strings.ResourceManager, new object[]
			{
				database
			});
		}

		public static LocalizedString ProcessorTimeExceededErrorThresholdWithAffinitizationMessage(string processName)
		{
			return new LocalizedString("ProcessorTimeExceededErrorThresholdWithAffinitizationMessage", Strings.ResourceManager, new object[]
			{
				processName
			});
		}

		public static LocalizedString AggregateDeliveryQueueLengthEscalationMessage
		{
			get
			{
				return new LocalizedString("AggregateDeliveryQueueLengthEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseCopyBehindEscalationSubject(string component, string machine, string dbCopy, int threshold)
		{
			return new LocalizedString("DatabaseCopyBehindEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine,
				dbCopy,
				threshold
			});
		}

		public static LocalizedString NoCafeMonitoringAccountsAvailable
		{
			get
			{
				return new LocalizedString("NoCafeMonitoringAccountsAvailable", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MediaEdgeResourceAllocationFailedEscalationMessage
		{
			get
			{
				return new LocalizedString("MediaEdgeResourceAllocationFailedEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DRAPendingReplication5MinutesEscalationMessage
		{
			get
			{
				return new LocalizedString("DRAPendingReplication5MinutesEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SchemaPartitionFailedEscalationMessage
		{
			get
			{
				return new LocalizedString("SchemaPartitionFailedEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchIndexCopyStatusError(string copyName, string result, string errorMessage)
		{
			return new LocalizedString("SearchIndexCopyStatusError", Strings.ResourceManager, new object[]
			{
				copyName,
				result,
				errorMessage
			});
		}

		public static LocalizedString DatabaseSchemaVersionCheckEscalationSubject
		{
			get
			{
				return new LocalizedString("DatabaseSchemaVersionCheckEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseDiskReadLatencyEscalationMessageEnt(TimeSpan duration, string unhealthyMonitorsCommand)
		{
			return new LocalizedString("DatabaseDiskReadLatencyEscalationMessageEnt", Strings.ResourceManager, new object[]
			{
				duration,
				unhealthyMonitorsCommand
			});
		}

		public static LocalizedString UMSipListeningPort
		{
			get
			{
				return new LocalizedString("UMSipListeningPort", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToGetDatabaseSize(string database)
		{
			return new LocalizedString("UnableToGetDatabaseSize", Strings.ResourceManager, new object[]
			{
				database
			});
		}

		public static LocalizedString ELCMailboxSLAEscalationSubject
		{
			get
			{
				return new LocalizedString("ELCMailboxSLAEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LocalMachineDriveNotProtectedWithDraEscalationSubject(string serverName)
		{
			return new LocalizedString("LocalMachineDriveNotProtectedWithDraEscalationSubject", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString StoreProcessRepeatedlyCrashingEscalationMessageDc(string processName, int count, TimeSpan duration)
		{
			return new LocalizedString("StoreProcessRepeatedlyCrashingEscalationMessageDc", Strings.ResourceManager, new object[]
			{
				processName,
				count,
				duration
			});
		}

		public static LocalizedString SearchCatalogNotLoaded(string databaseName, string serverName, string diagnosticInfoXml)
		{
			return new LocalizedString("SearchCatalogNotLoaded", Strings.ResourceManager, new object[]
			{
				databaseName,
				serverName,
				diagnosticInfoXml
			});
		}

		public static LocalizedString ActiveSyncDeepTestEscalationBodyENT(string serverName, string probeName)
		{
			return new LocalizedString("ActiveSyncDeepTestEscalationBodyENT", Strings.ResourceManager, new object[]
			{
				serverName,
				probeName
			});
		}

		public static LocalizedString DatabasePercentRPCRequestsEscalationMessageEnt(string databaseName, int percentRequests, TimeSpan duration)
		{
			return new LocalizedString("DatabasePercentRPCRequestsEscalationMessageEnt", Strings.ResourceManager, new object[]
			{
				databaseName,
				percentRequests,
				duration
			});
		}

		public static LocalizedString DHCPNacksEscalationMessage
		{
			get
			{
				return new LocalizedString("DHCPNacksEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ELCArchiveDumpsterEscalationMessage
		{
			get
			{
				return new LocalizedString("ELCArchiveDumpsterEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KDCServiceStatusTestMessage
		{
			get
			{
				return new LocalizedString("KDCServiceStatusTestMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseRPCLatencyEscalationMessageEnt(string databaseName, int latency, TimeSpan duration)
		{
			return new LocalizedString("DatabaseRPCLatencyEscalationMessageEnt", Strings.ResourceManager, new object[]
			{
				databaseName,
				latency,
				duration
			});
		}

		public static LocalizedString LowMemoryUnderThresholdErrorEscalationSubject
		{
			get
			{
				return new LocalizedString("LowMemoryUnderThresholdErrorEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OwaIMInitializationFailedSubject
		{
			get
			{
				return new LocalizedString("OwaIMInitializationFailedSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ForwardSyncStandardCompanyEscalationMessage(int count, int duration)
		{
			return new LocalizedString("ForwardSyncStandardCompanyEscalationMessage", Strings.ResourceManager, new object[]
			{
				count,
				duration
			});
		}

		public static LocalizedString PutDCIntoMMFailureEscalateMessage(string originalError, string dcFQDN)
		{
			return new LocalizedString("PutDCIntoMMFailureEscalateMessage", Strings.ResourceManager, new object[]
			{
				originalError,
				dcFQDN
			});
		}

		public static LocalizedString HostControllerServiceNodeOperationFailed(string nodeName, string operation)
		{
			return new LocalizedString("HostControllerServiceNodeOperationFailed", Strings.ResourceManager, new object[]
			{
				nodeName,
				operation
			});
		}

		public static LocalizedString PingConnectivityEscalationSubject
		{
			get
			{
				return new LocalizedString("PingConnectivityEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFolderConnectionCountEscalationMessage
		{
			get
			{
				return new LocalizedString("PublicFolderConnectionCountEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FastNodeNotHealthyEscalationMessage
		{
			get
			{
				return new LocalizedString("FastNodeNotHealthyEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ObserverHeartbeatEscalateResponderSubject(string subject)
		{
			return new LocalizedString("ObserverHeartbeatEscalateResponderSubject", Strings.ResourceManager, new object[]
			{
				subject
			});
		}

		public static LocalizedString AntimalwareEngineErrorsEscalationMessage(string engineName, double threshold, int duration)
		{
			return new LocalizedString("AntimalwareEngineErrorsEscalationMessage", Strings.ResourceManager, new object[]
			{
				engineName,
				threshold,
				duration
			});
		}

		public static LocalizedString AuditLogSearchServiceletEscalationMessage(string serverName)
		{
			return new LocalizedString("AuditLogSearchServiceletEscalationMessage", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString OwaMailboxDatabaseDoesntExist(string targetResource)
		{
			return new LocalizedString("OwaMailboxDatabaseDoesntExist", Strings.ResourceManager, new object[]
			{
				targetResource
			});
		}

		public static LocalizedString CheckDCMMDivergenceScriptExceptionMessage
		{
			get
			{
				return new LocalizedString("CheckDCMMDivergenceScriptExceptionMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OwaTooManyWebAppStartsBody(string serverName)
		{
			return new LocalizedString("OwaTooManyWebAppStartsBody", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString EscalationMessageFailuresUnhealthy(string customMessage)
		{
			return new LocalizedString("EscalationMessageFailuresUnhealthy", Strings.ResourceManager, new object[]
			{
				customMessage
			});
		}

		public static LocalizedString SearchQueryStxZeroHitMonitoringMailbox(string query, string mailboxSmtpAddress, string timestamp, string errorEvents)
		{
			return new LocalizedString("SearchQueryStxZeroHitMonitoringMailbox", Strings.ResourceManager, new object[]
			{
				query,
				mailboxSmtpAddress,
				timestamp,
				errorEvents
			});
		}

		public static LocalizedString ClusterServiceCrashEscalationMessage(int times, int hour, int duration)
		{
			return new LocalizedString("ClusterServiceCrashEscalationMessage", Strings.ResourceManager, new object[]
			{
				times,
				hour,
				duration
			});
		}

		public static LocalizedString OWACalendarAppPoolEscalationSubject(string serverName)
		{
			return new LocalizedString("OWACalendarAppPoolEscalationSubject", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString CrossPremiseMailflowEscalationMessage
		{
			get
			{
				return new LocalizedString("CrossPremiseMailflowEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UMSipOptionsToUMServiceFailedEscalationSubject(string serverName)
		{
			return new LocalizedString("UMSipOptionsToUMServiceFailedEscalationSubject", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString ForwardSyncStandardCompanyEscalationSubject
		{
			get
			{
				return new LocalizedString("ForwardSyncStandardCompanyEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PutDCIntoMMSuccessNotificationMessage(string originalError, string dcFQDN)
		{
			return new LocalizedString("PutDCIntoMMSuccessNotificationMessage", Strings.ResourceManager, new object[]
			{
				originalError,
				dcFQDN
			});
		}

		public static LocalizedString EseDbDivergenceDetectedEscalationMessage(string machine, string database)
		{
			return new LocalizedString("EseDbDivergenceDetectedEscalationMessage", Strings.ResourceManager, new object[]
			{
				machine,
				database
			});
		}

		public static LocalizedString JournalArchiveEscalationSubject
		{
			get
			{
				return new LocalizedString("JournalArchiveEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClusterServiceCrashEscalationSubject(string component, string target, int times, int hour)
		{
			return new LocalizedString("ClusterServiceCrashEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				target,
				times,
				hour
			});
		}

		public static LocalizedString DoMTConnectivityEscalateMessage
		{
			get
			{
				return new LocalizedString("DoMTConnectivityEscalateMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InferenceComponentDisabledEscalationMessage
		{
			get
			{
				return new LocalizedString("InferenceComponentDisabledEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LocalDriveLogSpaceEscalationSubject(string drive, TimeSpan duration)
		{
			return new LocalizedString("LocalDriveLogSpaceEscalationSubject", Strings.ResourceManager, new object[]
			{
				drive,
				duration
			});
		}

		public static LocalizedString ImapDeepTestEscalationBodyDC(string serverName, string probeName)
		{
			return new LocalizedString("ImapDeepTestEscalationBodyDC", Strings.ResourceManager, new object[]
			{
				serverName,
				probeName
			});
		}

		public static LocalizedString NoBackendMonitoringAccountsAvailable
		{
			get
			{
				return new LocalizedString("NoBackendMonitoringAccountsAvailable", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ActiveDirectoryConnectivityEscalationMessage
		{
			get
			{
				return new LocalizedString("ActiveDirectoryConnectivityEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SyntheticReplicationTransactionEscalationMessage
		{
			get
			{
				return new LocalizedString("SyntheticReplicationTransactionEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OabFileLoadExceptionEncounteredSubject
		{
			get
			{
				return new LocalizedString("OabFileLoadExceptionEncounteredSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ObserverHeartbeatEscalateResponderMessage(string subject, string observer)
		{
			return new LocalizedString("ObserverHeartbeatEscalateResponderMessage", Strings.ResourceManager, new object[]
			{
				subject,
				observer
			});
		}

		public static LocalizedString RegistryAccessDeniedEscalationMessage
		{
			get
			{
				return new LocalizedString("RegistryAccessDeniedEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReplServiceDownEscalationMessage(string machine, int restartService, int failoverTime, int bugcheckTime)
		{
			return new LocalizedString("ReplServiceDownEscalationMessage", Strings.ResourceManager, new object[]
			{
				machine,
				restartService,
				failoverTime,
				bugcheckTime
			});
		}

		public static LocalizedString AuditLogSearchServiceletEscalationSubject
		{
			get
			{
				return new LocalizedString("AuditLogSearchServiceletEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteStoreAdminRPCInterfaceEscalationEscalationMessageEnt(TimeSpan duration)
		{
			return new LocalizedString("RemoteStoreAdminRPCInterfaceEscalationEscalationMessageEnt", Strings.ResourceManager, new object[]
			{
				duration
			});
		}

		public static LocalizedString EventLogProbeLogName
		{
			get
			{
				return new LocalizedString("EventLogProbeLogName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Imap4ProtocolUnhealthy
		{
			get
			{
				return new LocalizedString("Imap4ProtocolUnhealthy", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchNumberOfParserServersDegradation(int registryKey, int defaultRegistryKey, string memoryUsage)
		{
			return new LocalizedString("SearchNumberOfParserServersDegradation", Strings.ResourceManager, new object[]
			{
				registryKey,
				defaultRegistryKey,
				memoryUsage
			});
		}

		public static LocalizedString UnMonitoredDatabaseEscalationSubject(string component, string machine, string database, TimeSpan duration)
		{
			return new LocalizedString("UnMonitoredDatabaseEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine,
				database,
				duration
			});
		}

		public static LocalizedString NumberOfActiveBackgroundTasksEscalationMessageEnt(string databaseName, int threshold, TimeSpan duration)
		{
			return new LocalizedString("NumberOfActiveBackgroundTasksEscalationMessageEnt", Strings.ResourceManager, new object[]
			{
				databaseName,
				threshold,
				duration
			});
		}

		public static LocalizedString OwaIMSigninFailedMessage(string serverName)
		{
			return new LocalizedString("OwaIMSigninFailedMessage", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString DLExpansionEscalationMessage
		{
			get
			{
				return new LocalizedString("DLExpansionEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HostControllerNodeRestartDetails(string startTime, string endTime, string restarts)
		{
			return new LocalizedString("HostControllerNodeRestartDetails", Strings.ResourceManager, new object[]
			{
				startTime,
				endTime,
				restarts
			});
		}

		public static LocalizedString ReplicationFailuresEscalationMessage
		{
			get
			{
				return new LocalizedString("ReplicationFailuresEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SCTStateMonitoringScriptExceptionMessage
		{
			get
			{
				return new LocalizedString("SCTStateMonitoringScriptExceptionMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AssistantsOutOfSlaError(string error)
		{
			return new LocalizedString("AssistantsOutOfSlaError", Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString SearchIndexStall(string databaseName, string lastPollTimestamp, string serverName)
		{
			return new LocalizedString("SearchIndexStall", Strings.ResourceManager, new object[]
			{
				databaseName,
				lastPollTimestamp,
				serverName
			});
		}

		public static LocalizedString UMTranscriptionThrottledEscalationMessage(int percentageValue)
		{
			return new LocalizedString("UMTranscriptionThrottledEscalationMessage", Strings.ResourceManager, new object[]
			{
				percentageValue
			});
		}

		public static LocalizedString SiteFailureEscalationSubject(string component, string machine)
		{
			return new LocalizedString("SiteFailureEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine
			});
		}

		public static LocalizedString ELCExceptionEscalationMessage
		{
			get
			{
				return new LocalizedString("ELCExceptionEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PopProxyTestEscalationBodyENT(string serverName, string probeName)
		{
			return new LocalizedString("PopProxyTestEscalationBodyENT", Strings.ResourceManager, new object[]
			{
				serverName,
				probeName
			});
		}

		public static LocalizedString PushNotificationDatacenterBackendEndpointUnhealthy(string serverName)
		{
			return new LocalizedString("PushNotificationDatacenterBackendEndpointUnhealthy", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString ComponentHealthErrorHeader(int failedResults)
		{
			return new LocalizedString("ComponentHealthErrorHeader", Strings.ResourceManager, new object[]
			{
				failedResults
			});
		}

		public static LocalizedString ImapEscalationSubject(string probeName, string serverName)
		{
			return new LocalizedString("ImapEscalationSubject", Strings.ResourceManager, new object[]
			{
				probeName,
				serverName
			});
		}

		public static LocalizedString PushNotificationPublisherUnhealthy(string channelName, string serverName)
		{
			return new LocalizedString("PushNotificationPublisherUnhealthy", Strings.ResourceManager, new object[]
			{
				channelName,
				serverName
			});
		}

		public static LocalizedString OabTooManyHttpErrorResponsesEncounteredBody
		{
			get
			{
				return new LocalizedString("OabTooManyHttpErrorResponsesEncounteredBody", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EacSelfTestEscalationBody(string serverName, string logPath)
		{
			return new LocalizedString("EacSelfTestEscalationBody", Strings.ResourceManager, new object[]
			{
				serverName,
				logPath
			});
		}

		public static LocalizedString QuarantineEscalationMessage
		{
			get
			{
				return new LocalizedString("QuarantineEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TransportRejectingMessageSubmissions
		{
			get
			{
				return new LocalizedString("TransportRejectingMessageSubmissions", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFolderConnectionCountEscalationSubject
		{
			get
			{
				return new LocalizedString("PublicFolderConnectionCountEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MonitoringAccountImproper(string mailboxDatabaseName, string upn)
		{
			return new LocalizedString("MonitoringAccountImproper", Strings.ResourceManager, new object[]
			{
				mailboxDatabaseName,
				upn
			});
		}

		public static LocalizedString PowerShellProfileEscalationSubject
		{
			get
			{
				return new LocalizedString("PowerShellProfileEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DivergenceBetweenCAAndAD1006EscalationMessage
		{
			get
			{
				return new LocalizedString("DivergenceBetweenCAAndAD1006EscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnreachableQueueLengthEscalationMessage
		{
			get
			{
				return new LocalizedString("UnreachableQueueLengthEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchQueryFailureEscalationMessage(string databaseName)
		{
			return new LocalizedString("SearchQueryFailureEscalationMessage", Strings.ResourceManager, new object[]
			{
				databaseName
			});
		}

		public static LocalizedString PushNotificationCafeUnexpectedResponse(string response, string requestHeaders, string responseHeaders, string body)
		{
			return new LocalizedString("PushNotificationCafeUnexpectedResponse", Strings.ResourceManager, new object[]
			{
				response,
				requestHeaders,
				responseHeaders,
				body
			});
		}

		public static LocalizedString ForwardSyncLiteCompanyEscalationMessage(int count, int duration)
		{
			return new LocalizedString("ForwardSyncLiteCompanyEscalationMessage", Strings.ResourceManager, new object[]
			{
				count,
				duration
			});
		}

		public static LocalizedString OabFileLoadExceptionEncounteredBody
		{
			get
			{
				return new LocalizedString("OabFileLoadExceptionEncounteredBody", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LastDBDiscoveryTimeFailedMessage(string interval)
		{
			return new LocalizedString("LastDBDiscoveryTimeFailedMessage", Strings.ResourceManager, new object[]
			{
				interval
			});
		}

		public static LocalizedString PublicFolderSyncEscalationSubject
		{
			get
			{
				return new LocalizedString("PublicFolderSyncEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StoreAdminRPCInterfaceEscalationSubject(TimeSpan duration)
		{
			return new LocalizedString("StoreAdminRPCInterfaceEscalationSubject", Strings.ResourceManager, new object[]
			{
				duration
			});
		}

		public static LocalizedString Imap4CommandProcessingTimeEscalationMessage
		{
			get
			{
				return new LocalizedString("Imap4CommandProcessingTimeEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProcessRepeatedlyCrashingEscalationMessage(string processName, int minCount, int durationMinutes)
		{
			return new LocalizedString("ProcessRepeatedlyCrashingEscalationMessage", Strings.ResourceManager, new object[]
			{
				processName,
				minCount,
				durationMinutes
			});
		}

		public static LocalizedString InvalidSearchResultsExceptionMessage
		{
			get
			{
				return new LocalizedString("InvalidSearchResultsExceptionMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UMServiceRecentCallRejectedEscalationMessageString(int percentageValue)
		{
			return new LocalizedString("UMServiceRecentCallRejectedEscalationMessageString", Strings.ResourceManager, new object[]
			{
				percentageValue
			});
		}

		public static LocalizedString SearchInformationNotAvailable
		{
			get
			{
				return new LocalizedString("SearchInformationNotAvailable", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ActiveDatabaseAvailabilityEscalationSubject
		{
			get
			{
				return new LocalizedString("ActiveDatabaseAvailabilityEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvokeNowAssemblyInfoFailure(string monitorIdentity)
		{
			return new LocalizedString("InvokeNowAssemblyInfoFailure", Strings.ResourceManager, new object[]
			{
				monitorIdentity
			});
		}

		public static LocalizedString UnableToGetDatabaseSchemaVersion(string database)
		{
			return new LocalizedString("UnableToGetDatabaseSchemaVersion", Strings.ResourceManager, new object[]
			{
				database
			});
		}

		public static LocalizedString ELCPermanentEscalationSubject
		{
			get
			{
				return new LocalizedString("ELCPermanentEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventLogProbeGreenEvents
		{
			get
			{
				return new LocalizedString("EventLogProbeGreenEvents", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InferenceDisabledComponentDetails(string componentName, string location)
		{
			return new LocalizedString("InferenceDisabledComponentDetails", Strings.ResourceManager, new object[]
			{
				componentName,
				location
			});
		}

		public static LocalizedString ClusterHangEscalationMessage
		{
			get
			{
				return new LocalizedString("ClusterHangEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FEPServiceNotRunningEscalationMessage
		{
			get
			{
				return new LocalizedString("FEPServiceNotRunningEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LocalMachineDriveEncryptionSuspendEscalationSubject(string serverName)
		{
			return new LocalizedString("LocalMachineDriveEncryptionSuspendEscalationSubject", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString QuarantinedMailboxEscalationSubject(string databaseName)
		{
			return new LocalizedString("QuarantinedMailboxEscalationSubject", Strings.ResourceManager, new object[]
			{
				databaseName
			});
		}

		public static LocalizedString SearchLocalCopyStatusEscalationMessage(string databaseName)
		{
			return new LocalizedString("SearchLocalCopyStatusEscalationMessage", Strings.ResourceManager, new object[]
			{
				databaseName
			});
		}

		public static LocalizedString ProcessCrashDetectionEscalationMessage(string process)
		{
			return new LocalizedString("ProcessCrashDetectionEscalationMessage", Strings.ResourceManager, new object[]
			{
				process
			});
		}

		public static LocalizedString LogVolumeSpaceEscalationSubject(string component, string machine, string database)
		{
			return new LocalizedString("LogVolumeSpaceEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine,
				database
			});
		}

		public static LocalizedString ClusterGroupDownEscalationSubject(string component, string target, int threshold)
		{
			return new LocalizedString("ClusterGroupDownEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				target,
				threshold
			});
		}

		public static LocalizedString LocalMachineDriveBootVolumeEncryptionStateEscalationSubject(string serverName)
		{
			return new LocalizedString("LocalMachineDriveBootVolumeEncryptionStateEscalationSubject", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString DatabaseCopySlowReplayEscalationSubject(string component, string machine, string dbCopy, int threshold)
		{
			return new LocalizedString("DatabaseCopySlowReplayEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine,
				dbCopy,
				threshold
			});
		}

		public static LocalizedString RidMonitorEscalationMessage
		{
			get
			{
				return new LocalizedString("RidMonitorEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SystemMailboxGuidNotFound
		{
			get
			{
				return new LocalizedString("SystemMailboxGuidNotFound", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MediaEdgeAuthenticationServiceCredentialsAcquisitionFailedEscalationMessage
		{
			get
			{
				return new LocalizedString("MediaEdgeAuthenticationServiceCredentialsAcquisitionFailedEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchIndexBacklogWithHistory(string databaseName, string backlog, string retryQueue, string lastTime, string lastBacklog, string lastRetryQueue, string upTime, string serverStatus)
		{
			return new LocalizedString("SearchIndexBacklogWithHistory", Strings.ResourceManager, new object[]
			{
				databaseName,
				backlog,
				retryQueue,
				lastTime,
				lastBacklog,
				lastRetryQueue,
				upTime,
				serverStatus
			});
		}

		public static LocalizedString EacCtpTestEscalationBody(string serverName, string logPath)
		{
			return new LocalizedString("EacCtpTestEscalationBody", Strings.ResourceManager, new object[]
			{
				serverName,
				logPath
			});
		}

		public static LocalizedString OabMailboxEscalationSubject(string oabGuid, string serverName)
		{
			return new LocalizedString("OabMailboxEscalationSubject", Strings.ResourceManager, new object[]
			{
				oabGuid,
				serverName
			});
		}

		public static LocalizedString SearchTransportAgentFailureEscalationMessage
		{
			get
			{
				return new LocalizedString("SearchTransportAgentFailureEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TransportMessageCategorizationEscalationMessage
		{
			get
			{
				return new LocalizedString("TransportMessageCategorizationEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerInMaintenanceModeForTooLongEscalationSubject(string component, string machine, string threshold)
		{
			return new LocalizedString("ServerInMaintenanceModeForTooLongEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine,
				threshold
			});
		}

		public static LocalizedString ServiceNotRunningEscalationMessageEnt(string serviceName)
		{
			return new LocalizedString("ServiceNotRunningEscalationMessageEnt", Strings.ResourceManager, new object[]
			{
				serviceName
			});
		}

		public static LocalizedString DatabaseCopySlowReplayEscalationMessage(string database, int threshold)
		{
			return new LocalizedString("DatabaseCopySlowReplayEscalationMessage", Strings.ResourceManager, new object[]
			{
				database,
				threshold
			});
		}

		public static LocalizedString UMGrammarUsageEscalationMessage(int percentageValue)
		{
			return new LocalizedString("UMGrammarUsageEscalationMessage", Strings.ResourceManager, new object[]
			{
				percentageValue
			});
		}

		public static LocalizedString FireWallEscalationMessage(int count, int duration)
		{
			return new LocalizedString("FireWallEscalationMessage", Strings.ResourceManager, new object[]
			{
				count,
				duration
			});
		}

		public static LocalizedString InocrrectSCTStateExceptionMessage
		{
			get
			{
				return new LocalizedString("InocrrectSCTStateExceptionMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HAPassiveCopyUnhealthy(string copyState)
		{
			return new LocalizedString("HAPassiveCopyUnhealthy", Strings.ResourceManager, new object[]
			{
				copyState
			});
		}

		public static LocalizedString UMSipOptionsToUMServiceFailedEscalationBody(string serverName)
		{
			return new LocalizedString("UMSipOptionsToUMServiceFailedEscalationBody", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString DataIssueEscalationMessage
		{
			get
			{
				return new LocalizedString("DataIssueEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailSubmissionBehindWatermarksEscalationSubject(TimeSpan ageThreshold, TimeSpan duration, string databaseName)
		{
			return new LocalizedString("MailSubmissionBehindWatermarksEscalationSubject", Strings.ResourceManager, new object[]
			{
				ageThreshold,
				duration,
				databaseName
			});
		}

		public static LocalizedString ComponentHealthErrorContent(string componentName, string resultName, DateTime executionEndTime)
		{
			return new LocalizedString("ComponentHealthErrorContent", Strings.ResourceManager, new object[]
			{
				componentName,
				resultName,
				executionEndTime
			});
		}

		public static LocalizedString KerbAuthFailureEscalationMessagPAC
		{
			get
			{
				return new LocalizedString("KerbAuthFailureEscalationMessagPAC", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PrivateWorkingSetExceededWarningThresholdMessage(string processName)
		{
			return new LocalizedString("PrivateWorkingSetExceededWarningThresholdMessage", Strings.ResourceManager, new object[]
			{
				processName
			});
		}

		public static LocalizedString DivergenceInDefinitionEscalationMessage
		{
			get
			{
				return new LocalizedString("DivergenceInDefinitionEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MobilityAccount
		{
			get
			{
				return new LocalizedString("MobilityAccount", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ActiveSyncSelfTestEscalationBodyDC(string serverName, string probeName)
		{
			return new LocalizedString("ActiveSyncSelfTestEscalationBodyDC", Strings.ResourceManager, new object[]
			{
				serverName,
				probeName
			});
		}

		public static LocalizedString NTFSCorruptionEscalationMessage(string database, string threshold)
		{
			return new LocalizedString("NTFSCorruptionEscalationMessage", Strings.ResourceManager, new object[]
			{
				database,
				threshold
			});
		}

		public static LocalizedString FailedAndSuspendedCopyEscalationMessage(string database, string threshold)
		{
			return new LocalizedString("FailedAndSuspendedCopyEscalationMessage", Strings.ResourceManager, new object[]
			{
				database,
				threshold
			});
		}

		public static LocalizedString OneCopyMonitorFailureMessage(int duration, int threshold)
		{
			return new LocalizedString("OneCopyMonitorFailureMessage", Strings.ResourceManager, new object[]
			{
				duration,
				threshold
			});
		}

		public static LocalizedString EventAssistantsProcessRepeatedlyCrashingEscalationMessageEnt(string processName, int count, TimeSpan duration)
		{
			return new LocalizedString("EventAssistantsProcessRepeatedlyCrashingEscalationMessageEnt", Strings.ResourceManager, new object[]
			{
				processName,
				count,
				duration
			});
		}

		public static LocalizedString OwaTooManyLogoffFailuresBody
		{
			get
			{
				return new LocalizedString("OwaTooManyLogoffFailuresBody", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ForwardSyncProcessRepeatedlyCrashingEscalationSubject
		{
			get
			{
				return new LocalizedString("ForwardSyncProcessRepeatedlyCrashingEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PutMultipleDCIntoMMFailureEscalateMessage(string dcFQDN)
		{
			return new LocalizedString("PutMultipleDCIntoMMFailureEscalateMessage", Strings.ResourceManager, new object[]
			{
				dcFQDN
			});
		}

		public static LocalizedString SystemDriveSpaceEscalationMessage(string machine, string drive, string threshold)
		{
			return new LocalizedString("SystemDriveSpaceEscalationMessage", Strings.ResourceManager, new object[]
			{
				machine,
				drive,
				threshold
			});
		}

		public static LocalizedString DatabasePercentRPCRequestsEscalationSubject(string databaseName, int percentRequests)
		{
			return new LocalizedString("DatabasePercentRPCRequestsEscalationSubject", Strings.ResourceManager, new object[]
			{
				databaseName,
				percentRequests
			});
		}

		public static LocalizedString SearchQuerySlow(string databaseName, string slowRate, string threshold)
		{
			return new LocalizedString("SearchQuerySlow", Strings.ResourceManager, new object[]
			{
				databaseName,
				slowRate,
				threshold
			});
		}

		public static LocalizedString ADDatabaseCorruptionEscalationMessage
		{
			get
			{
				return new LocalizedString("ADDatabaseCorruptionEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchIndexSuspendedEscalationMessage(string databaseName)
		{
			return new LocalizedString("SearchIndexSuspendedEscalationMessage", Strings.ResourceManager, new object[]
			{
				databaseName
			});
		}

		public static LocalizedString InvokeNowInvalidWorkDefinition(string requestId)
		{
			return new LocalizedString("InvokeNowInvalidWorkDefinition", Strings.ResourceManager, new object[]
			{
				requestId
			});
		}

		public static LocalizedString OabTooManyWebAppStartsBody(string serverName)
		{
			return new LocalizedString("OabTooManyWebAppStartsBody", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString HostControllerServiceNodeUnhealthy(string details)
		{
			return new LocalizedString("HostControllerServiceNodeUnhealthy", Strings.ResourceManager, new object[]
			{
				details
			});
		}

		public static LocalizedString HostControllerExcessiveNodeRestarts(string nodeName, string count, string minutes, string details)
		{
			return new LocalizedString("HostControllerExcessiveNodeRestarts", Strings.ResourceManager, new object[]
			{
				nodeName,
				count,
				minutes,
				details
			});
		}

		public static LocalizedString MailboxAuditingAvailabilityFailureEscalationSubject
		{
			get
			{
				return new LocalizedString("MailboxAuditingAvailabilityFailureEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TopologyServiceConnectivityEscalationMessage
		{
			get
			{
				return new LocalizedString("TopologyServiceConnectivityEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OwaDeepTestEscalationHtmlBody(string serverName, string logPath)
		{
			return new LocalizedString("OwaDeepTestEscalationHtmlBody", Strings.ResourceManager, new object[]
			{
				serverName,
				logPath
			});
		}

		public static LocalizedString SuspendedCopyEscalationMessage(string database, int threshold)
		{
			return new LocalizedString("SuspendedCopyEscalationMessage", Strings.ResourceManager, new object[]
			{
				database,
				threshold
			});
		}

		public static LocalizedString UMSipTransport
		{
			get
			{
				return new LocalizedString("UMSipTransport", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OabProtocolEscalationBody
		{
			get
			{
				return new LocalizedString("OabProtocolEscalationBody", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PushNotificationEnterpriseEmptyServiceUri
		{
			get
			{
				return new LocalizedString("PushNotificationEnterpriseEmptyServiceUri", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PushNotificationEnterpriseAuthError
		{
			get
			{
				return new LocalizedString("PushNotificationEnterpriseAuthError", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapProxyTestEscalationBodyENT(string serverName, string probeName)
		{
			return new LocalizedString("ImapProxyTestEscalationBodyENT", Strings.ResourceManager, new object[]
			{
				serverName,
				probeName
			});
		}

		public static LocalizedString SearchResourceLoadEscalationMessage(string databaseName, int minutes)
		{
			return new LocalizedString("SearchResourceLoadEscalationMessage", Strings.ResourceManager, new object[]
			{
				databaseName,
				minutes
			});
		}

		public static LocalizedString InvalidAccessToken(string userSid)
		{
			return new LocalizedString("InvalidAccessToken", Strings.ResourceManager, new object[]
			{
				userSid
			});
		}

		public static LocalizedString LocalMachineDriveProtectedWithDraWithoutDecryptorEscalationMessage(string volumes, string serverName)
		{
			return new LocalizedString("LocalMachineDriveProtectedWithDraWithoutDecryptorEscalationMessage", Strings.ResourceManager, new object[]
			{
				volumes,
				serverName
			});
		}

		public static LocalizedString UnableToRunAlertNotificationTypeByDatabaseCopyStateResponder
		{
			get
			{
				return new LocalizedString("UnableToRunAlertNotificationTypeByDatabaseCopyStateResponder", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ELCDumpsterWarningEscalationSubject
		{
			get
			{
				return new LocalizedString("ELCDumpsterWarningEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationNotificationSubject(string component, string notification)
		{
			return new LocalizedString("MigrationNotificationSubject", Strings.ResourceManager, new object[]
			{
				component,
				notification
			});
		}

		public static LocalizedString PublicFolderSyncEscalationMessage(int minCount, int durationMinutes)
		{
			return new LocalizedString("PublicFolderSyncEscalationMessage", Strings.ResourceManager, new object[]
			{
				minCount,
				durationMinutes
			});
		}

		public static LocalizedString RcaWorkItemCreationSummaryEntry(int successful, int total)
		{
			return new LocalizedString("RcaWorkItemCreationSummaryEntry", Strings.ResourceManager, new object[]
			{
				successful,
				total
			});
		}

		public static LocalizedString OabMailboxEscalationBody
		{
			get
			{
				return new LocalizedString("OabMailboxEscalationBody", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WacDiscoveryFailureBody(string serverName)
		{
			return new LocalizedString("WacDiscoveryFailureBody", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString CheckZombieDCEscalateMessage
		{
			get
			{
				return new LocalizedString("CheckZombieDCEscalateMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HighLogGenerationRateEscalationSubject(string component, string machine, string dbCopy, int threshold)
		{
			return new LocalizedString("HighLogGenerationRateEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine,
				dbCopy,
				threshold
			});
		}

		public static LocalizedString RidSetMonitorEscalationMessage
		{
			get
			{
				return new LocalizedString("RidSetMonitorEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PushNotificationCafeEndpointUnhealthy
		{
			get
			{
				return new LocalizedString("PushNotificationCafeEndpointUnhealthy", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseLogicalPhysicalSizeRatioEscalationMessageEnt(double threshold, TimeSpan duration, string unhealthyMonitorsCommand)
		{
			return new LocalizedString("DatabaseLogicalPhysicalSizeRatioEscalationMessageEnt", Strings.ResourceManager, new object[]
			{
				threshold,
				duration,
				unhealthyMonitorsCommand
			});
		}

		public static LocalizedString ProvisioningBigVolumeErrorEscalationMessage
		{
			get
			{
				return new LocalizedString("ProvisioningBigVolumeErrorEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OwaSelfTestEscalationBody(string serverName, string logPath)
		{
			return new LocalizedString("OwaSelfTestEscalationBody", Strings.ResourceManager, new object[]
			{
				serverName,
				logPath
			});
		}

		public static LocalizedString MailboxAssistantsBehindWatermarksEscalationSubject(TimeSpan ageThreshold, TimeSpan duration)
		{
			return new LocalizedString("MailboxAssistantsBehindWatermarksEscalationSubject", Strings.ResourceManager, new object[]
			{
				ageThreshold,
				duration
			});
		}

		public static LocalizedString PublicFolderMailboxQuotaEscalationMessage
		{
			get
			{
				return new LocalizedString("PublicFolderMailboxQuotaEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CASRoutingFailureEscalationSubject
		{
			get
			{
				return new LocalizedString("CASRoutingFailureEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OAuthRequestFailureEscalationBody
		{
			get
			{
				return new LocalizedString("OAuthRequestFailureEscalationBody", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GLSEscalationMessage
		{
			get
			{
				return new LocalizedString("GLSEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SCTNotFoundForAllVersionsExceptionMessage
		{
			get
			{
				return new LocalizedString("SCTNotFoundForAllVersionsExceptionMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ControllerFailureMessage(string machine, int duration)
		{
			return new LocalizedString("ControllerFailureMessage", Strings.ResourceManager, new object[]
			{
				machine,
				duration
			});
		}

		public static LocalizedString HighLogGenerationRateEscalationMessage(string database, int threshold, string logGenThreshold)
		{
			return new LocalizedString("HighLogGenerationRateEscalationMessage", Strings.ResourceManager, new object[]
			{
				database,
				threshold,
				logGenThreshold
			});
		}

		public static LocalizedString SearchFeedingControllerFailureEscalationMessage(string databaseName, int threshold, int minutes)
		{
			return new LocalizedString("SearchFeedingControllerFailureEscalationMessage", Strings.ResourceManager, new object[]
			{
				databaseName,
				threshold,
				minutes
			});
		}

		public static LocalizedString ClusterNetworkDownEscalationSubject(string component, string target, int threshold)
		{
			return new LocalizedString("ClusterNetworkDownEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				target,
				threshold
			});
		}

		public static LocalizedString SqlOutputStreamInRetryEscalationMessage
		{
			get
			{
				return new LocalizedString("SqlOutputStreamInRetryEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FireWallEscalationSubject(string machine)
		{
			return new LocalizedString("FireWallEscalationSubject", Strings.ResourceManager, new object[]
			{
				machine
			});
		}

		public static LocalizedString DefaultEscalationSubject
		{
			get
			{
				return new LocalizedString("DefaultEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxAuditingAvailabilityFailureEscalationBody
		{
			get
			{
				return new LocalizedString("MailboxAuditingAvailabilityFailureEscalationBody", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BulkProvisioningNoProgressEscalationSubject
		{
			get
			{
				return new LocalizedString("BulkProvisioningNoProgressEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerVersionNotFound(string serverName)
		{
			return new LocalizedString("ServerVersionNotFound", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString EacDeepTestEscalationBody(string serverName, string logPath)
		{
			return new LocalizedString("EacDeepTestEscalationBody", Strings.ResourceManager, new object[]
			{
				serverName,
				logPath
			});
		}

		public static LocalizedString InfrastructureValidationSubject
		{
			get
			{
				return new LocalizedString("InfrastructureValidationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchMemoryUsageOverThresholdEscalationMessage
		{
			get
			{
				return new LocalizedString("SearchMemoryUsageOverThresholdEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VersionBucketsAllocatedEscalationEscalationMessageDc(TimeSpan duration, string unhealthyMonitorsCommand)
		{
			return new LocalizedString("VersionBucketsAllocatedEscalationEscalationMessageDc", Strings.ResourceManager, new object[]
			{
				duration,
				unhealthyMonitorsCommand
			});
		}

		public static LocalizedString SharedCacheEscalationMessage
		{
			get
			{
				return new LocalizedString("SharedCacheEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchIndexCrawlingWithHealthyCopy(string databaseName, string healthyCopyServerName)
		{
			return new LocalizedString("SearchIndexCrawlingWithHealthyCopy", Strings.ResourceManager, new object[]
			{
				databaseName,
				healthyCopyServerName
			});
		}

		public static LocalizedString ProcessorTimeExceededErrorThresholdSubject(string processName)
		{
			return new LocalizedString("ProcessorTimeExceededErrorThresholdSubject", Strings.ResourceManager, new object[]
			{
				processName
			});
		}

		public static LocalizedString MailboxAssistantsBehindWatermarksEscalationMessageEnt(TimeSpan ageThreshold, TimeSpan duration, string invokeNowCommand, string unhealthyMonitorsCommand)
		{
			return new LocalizedString("MailboxAssistantsBehindWatermarksEscalationMessageEnt", Strings.ResourceManager, new object[]
			{
				ageThreshold,
				duration,
				invokeNowCommand,
				unhealthyMonitorsCommand
			});
		}

		public static LocalizedString CannotRecoverEscalationMessage
		{
			get
			{
				return new LocalizedString("CannotRecoverEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AsynchronousAuditSearchAvailabilityFailureEscalationBody
		{
			get
			{
				return new LocalizedString("AsynchronousAuditSearchAvailabilityFailureEscalationBody", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OwaIMLogAnalyzerMessage
		{
			get
			{
				return new LocalizedString("OwaIMLogAnalyzerMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UMCertificateSubjectName
		{
			get
			{
				return new LocalizedString("UMCertificateSubjectName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PassiveDatabaseAvailabilityEscalationMessageDc(string invokeNowCommand, string unhealthyMonitorsCommand)
		{
			return new LocalizedString("PassiveDatabaseAvailabilityEscalationMessageDc", Strings.ResourceManager, new object[]
			{
				invokeNowCommand,
				unhealthyMonitorsCommand
			});
		}

		public static LocalizedString OwaNoMailboxesAvailable
		{
			get
			{
				return new LocalizedString("OwaNoMailboxesAvailable", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SuspendedCopyEscalationSubject(string component, string machine, string dbCopy, int threshold)
		{
			return new LocalizedString("SuspendedCopyEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine,
				dbCopy,
				threshold
			});
		}

		public static LocalizedString SearchCatalogSuspended(string databaseName, string databaseState)
		{
			return new LocalizedString("SearchCatalogSuspended", Strings.ResourceManager, new object[]
			{
				databaseName,
				databaseState
			});
		}

		public static LocalizedString UMRecentPartnerTranscriptionFailedEscalationMessageString(int percentageValue)
		{
			return new LocalizedString("UMRecentPartnerTranscriptionFailedEscalationMessageString", Strings.ResourceManager, new object[]
			{
				percentageValue
			});
		}

		public static LocalizedString EseDbTimeAdvanceEscalationMessage(string machine, string database)
		{
			return new LocalizedString("EseDbTimeAdvanceEscalationMessage", Strings.ResourceManager, new object[]
			{
				machine,
				database
			});
		}

		public static LocalizedString TransportCategorizerJobsUnavailableEscalationMessage
		{
			get
			{
				return new LocalizedString("TransportCategorizerJobsUnavailableEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SingleAvailableDatabaseCopyEscalationMessage
		{
			get
			{
				return new LocalizedString("SingleAvailableDatabaseCopyEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DivergenceInSiteNameEscalationMessage
		{
			get
			{
				return new LocalizedString("DivergenceInSiteNameEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NullSearchResponseExceptionMessage
		{
			get
			{
				return new LocalizedString("NullSearchResponseExceptionMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ControllerFailureEscalationSubject(string component, string machine)
		{
			return new LocalizedString("ControllerFailureEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine
			});
		}

		public static LocalizedString SearchWordBreakerLoadingFailure(string nodeName, string timeStamp, string redEventId, string channelName, string greenEventId)
		{
			return new LocalizedString("SearchWordBreakerLoadingFailure", Strings.ResourceManager, new object[]
			{
				nodeName,
				timeStamp,
				redEventId,
				channelName,
				greenEventId
			});
		}

		public static LocalizedString SearchIndexMultiCrawling(string databaseName, int count, string details)
		{
			return new LocalizedString("SearchIndexMultiCrawling", Strings.ResourceManager, new object[]
			{
				databaseName,
				count,
				details
			});
		}

		public static LocalizedString AvailabilityServiceEscalationBody(string probeType)
		{
			return new LocalizedString("AvailabilityServiceEscalationBody", Strings.ResourceManager, new object[]
			{
				probeType
			});
		}

		public static LocalizedString OwaSelfTestEscalationHtmlBody(string serverName, string logPath)
		{
			return new LocalizedString("OwaSelfTestEscalationHtmlBody", Strings.ResourceManager, new object[]
			{
				serverName,
				logPath
			});
		}

		public static LocalizedString ProcessorTimeExceededWarningThresholdSubject(string processName)
		{
			return new LocalizedString("ProcessorTimeExceededWarningThresholdSubject", Strings.ResourceManager, new object[]
			{
				processName
			});
		}

		public static LocalizedString SearchIndexSingleHealthyCopyWithSeeding(string databaseName, string details, string seedingStartTime)
		{
			return new LocalizedString("SearchIndexSingleHealthyCopyWithSeeding", Strings.ResourceManager, new object[]
			{
				databaseName,
				details,
				seedingStartTime
			});
		}

		public static LocalizedString DatabasePercentRPCRequestsEscalationMessageDc(string databaseName, int percentRequests, TimeSpan duration)
		{
			return new LocalizedString("DatabasePercentRPCRequestsEscalationMessageDc", Strings.ResourceManager, new object[]
			{
				databaseName,
				percentRequests,
				duration
			});
		}

		public static LocalizedString DatabaseCopyBehindEscalationMessage(string database, int threshold)
		{
			return new LocalizedString("DatabaseCopyBehindEscalationMessage", Strings.ResourceManager, new object[]
			{
				database,
				threshold
			});
		}

		public static LocalizedString EseInconsistentDataDetectedEscalationSubject(string component, string machine, string database)
		{
			return new LocalizedString("EseInconsistentDataDetectedEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine,
				database
			});
		}

		public static LocalizedString UncategorizedProcess
		{
			get
			{
				return new LocalizedString("UncategorizedProcess", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseValidationNullRef(string database)
		{
			return new LocalizedString("DatabaseValidationNullRef", Strings.ResourceManager, new object[]
			{
				database
			});
		}

		public static LocalizedString CafeThreadCountMessageUnhealthy(string appPool, string percentThreshold, string maxThreads)
		{
			return new LocalizedString("CafeThreadCountMessageUnhealthy", Strings.ResourceManager, new object[]
			{
				appPool,
				percentThreshold,
				maxThreads
			});
		}

		public static LocalizedString MSExchangeProtectedServiceHostCrashingMessage
		{
			get
			{
				return new LocalizedString("MSExchangeProtectedServiceHostCrashingMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UMDatacenterLoadBalancerSipOptionsPingEscalationMessage
		{
			get
			{
				return new LocalizedString("UMDatacenterLoadBalancerSipOptionsPingEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PassiveReplicationMonitorEscalationMessage
		{
			get
			{
				return new LocalizedString("PassiveReplicationMonitorEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PotentialInsufficientRedundancyEscalationSubject(string component, string machine)
		{
			return new LocalizedString("PotentialInsufficientRedundancyEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine
			});
		}

		public static LocalizedString ReinstallServerEscalationMessage
		{
			get
			{
				return new LocalizedString("ReinstallServerEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClusterNetworkReportErrorEscalationMessage(int suppression)
		{
			return new LocalizedString("ClusterNetworkReportErrorEscalationMessage", Strings.ResourceManager, new object[]
			{
				suppression
			});
		}

		public static LocalizedString ForwardSyncCookieNotUpToDateEscalationSubject
		{
			get
			{
				return new LocalizedString("ForwardSyncCookieNotUpToDateEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DbFailureItemIoHardEscalationMessage(string database)
		{
			return new LocalizedString("DbFailureItemIoHardEscalationMessage", Strings.ResourceManager, new object[]
			{
				database
			});
		}

		public static LocalizedString MonitoringAccountDomainUnavailable(string mailboxDatabaseName)
		{
			return new LocalizedString("MonitoringAccountDomainUnavailable", Strings.ResourceManager, new object[]
			{
				mailboxDatabaseName
			});
		}

		public static LocalizedString ClusterNodeEvictedEscalationSubject(string component, string target)
		{
			return new LocalizedString("ClusterNodeEvictedEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				target
			});
		}

		public static LocalizedString ActiveDirectoryConnectivityLocalEscalationMessage
		{
			get
			{
				return new LocalizedString("ActiveDirectoryConnectivityLocalEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LocalMachineDriveEncryptionSuspendEscalationMessage(string volumes, string serverName)
		{
			return new LocalizedString("LocalMachineDriveEncryptionSuspendEscalationMessage", Strings.ResourceManager, new object[]
			{
				volumes,
				serverName
			});
		}

		public static LocalizedString ActiveManagerUnhealthyEscalationSubject(string component, string machine)
		{
			return new LocalizedString("ActiveManagerUnhealthyEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine
			});
		}

		public static LocalizedString SearchIndexSeedingNoProgres(string databaseName, string percent, string seedingSource)
		{
			return new LocalizedString("SearchIndexSeedingNoProgres", Strings.ResourceManager, new object[]
			{
				databaseName,
				percent,
				seedingSource
			});
		}

		public static LocalizedString PassiveDatabaseAvailabilityEscalationSubject
		{
			get
			{
				return new LocalizedString("PassiveDatabaseAvailabilityEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LagCopyHealthProblemEscalationMessage(string database)
		{
			return new LocalizedString("LagCopyHealthProblemEscalationMessage", Strings.ResourceManager, new object[]
			{
				database
			});
		}

		public static LocalizedString OabTooManyHttpErrorResponsesEncounteredSubject
		{
			get
			{
				return new LocalizedString("OabTooManyHttpErrorResponsesEncounteredSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ELCTransientEscalationSubject
		{
			get
			{
				return new LocalizedString("ELCTransientEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedAndSuspendedCopyEscalationSubject(string component, string machine, string dbCopy, string threshold)
		{
			return new LocalizedString("FailedAndSuspendedCopyEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine,
				dbCopy,
				threshold
			});
		}

		public static LocalizedString RemoteStoreAdminRPCInterfaceEscalationSubject(TimeSpan duration)
		{
			return new LocalizedString("RemoteStoreAdminRPCInterfaceEscalationSubject", Strings.ResourceManager, new object[]
			{
				duration
			});
		}

		public static LocalizedString SiteFailureEscalationMessage
		{
			get
			{
				return new LocalizedString("SiteFailureEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvokeNowPickupEventNotFound(string requestId)
		{
			return new LocalizedString("InvokeNowPickupEventNotFound", Strings.ResourceManager, new object[]
			{
				requestId
			});
		}

		public static LocalizedString OnlineMeetingCreateEscalationBody
		{
			get
			{
				return new LocalizedString("OnlineMeetingCreateEscalationBody", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SiteMailboxDocumentSyncEscalationSubject
		{
			get
			{
				return new LocalizedString("SiteMailboxDocumentSyncEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NTDSCorruptionEscalationMessage
		{
			get
			{
				return new LocalizedString("NTDSCorruptionEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TopoDiscoveryFailedAllServersEscalationMessage
		{
			get
			{
				return new LocalizedString("TopoDiscoveryFailedAllServersEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ActiveSyncSelfTestEscalationBodyENT(string serverName, string probeName)
		{
			return new LocalizedString("ActiveSyncSelfTestEscalationBodyENT", Strings.ResourceManager, new object[]
			{
				serverName,
				probeName
			});
		}

		public static LocalizedString PrivateWorkingSetExceededErrorThresholdSubject(string processName)
		{
			return new LocalizedString("PrivateWorkingSetExceededErrorThresholdSubject", Strings.ResourceManager, new object[]
			{
				processName
			});
		}

		public static LocalizedString AvailabilityServiceEscalationSubjectUnhealthy(string probeType)
		{
			return new LocalizedString("AvailabilityServiceEscalationSubjectUnhealthy", Strings.ResourceManager, new object[]
			{
				probeType
			});
		}

		public static LocalizedString ActiveSyncEscalationSubject(string probeName, string serverName)
		{
			return new LocalizedString("ActiveSyncEscalationSubject", Strings.ResourceManager, new object[]
			{
				probeName,
				serverName
			});
		}

		public static LocalizedString StoreMaintenanceAssistantEscalationMessageEnt(TimeSpan duration, string unhealthyMonitorsCommand)
		{
			return new LocalizedString("StoreMaintenanceAssistantEscalationMessageEnt", Strings.ResourceManager, new object[]
			{
				duration,
				unhealthyMonitorsCommand
			});
		}

		public static LocalizedString SearchTransportAgentFailure(string failureRate, string threshold, string window, string total, string failed)
		{
			return new LocalizedString("SearchTransportAgentFailure", Strings.ResourceManager, new object[]
			{
				failureRate,
				threshold,
				window,
				total,
				failed
			});
		}

		public static LocalizedString VersionStore1479EscalationMessage
		{
			get
			{
				return new LocalizedString("VersionStore1479EscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AssistantsNotRunningError(string error)
		{
			return new LocalizedString("AssistantsNotRunningError", Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString AssistantsNotRunningToCompletionSubject
		{
			get
			{
				return new LocalizedString("AssistantsNotRunningToCompletionSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapCustomerTouchPointEscalationBodyDC(string serverName, string probeName)
		{
			return new LocalizedString("ImapCustomerTouchPointEscalationBodyDC", Strings.ResourceManager, new object[]
			{
				serverName,
				probeName
			});
		}

		public static LocalizedString AdminAuditingAvailabilityFailureEscalationBody
		{
			get
			{
				return new LocalizedString("AdminAuditingAvailabilityFailureEscalationBody", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ForwardSyncLiteCompanyEscalationSubject
		{
			get
			{
				return new LocalizedString("ForwardSyncLiteCompanyEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MSExchangeInformationStoreCannotContactADEscalationMessage
		{
			get
			{
				return new LocalizedString("MSExchangeInformationStoreCannotContactADEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DHCPServerRequestsEscalationMessage
		{
			get
			{
				return new LocalizedString("DHCPServerRequestsEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RpsFailedEscalationMessage(string virtualDirectory)
		{
			return new LocalizedString("RpsFailedEscalationMessage", Strings.ResourceManager, new object[]
			{
				virtualDirectory
			});
		}

		public static LocalizedString ForwardSyncProcessRepeatedlyCrashingEscalationMessage(int count, int duration)
		{
			return new LocalizedString("ForwardSyncProcessRepeatedlyCrashingEscalationMessage", Strings.ResourceManager, new object[]
			{
				count,
				duration
			});
		}

		public static LocalizedString StoreAdminRPCInterfaceNotResponding(string server)
		{
			return new LocalizedString("StoreAdminRPCInterfaceNotResponding", Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString NoNTDSObjectEscalationMessage
		{
			get
			{
				return new LocalizedString("NoNTDSObjectEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OwaCustomerTouchPointEscalationSubject(string serverName)
		{
			return new LocalizedString("OwaCustomerTouchPointEscalationSubject", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString PublicFolderMoveJobStuckEscalationSubject
		{
			get
			{
				return new LocalizedString("PublicFolderMoveJobStuckEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SCTMonitoringScriptExceptionMessage
		{
			get
			{
				return new LocalizedString("SCTMonitoringScriptExceptionMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OwaIMSigninFailedSubject(string serverName)
		{
			return new LocalizedString("OwaIMSigninFailedSubject", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString UMPipelineSLAEscalationMessageString(int percentageValue)
		{
			return new LocalizedString("UMPipelineSLAEscalationMessageString", Strings.ResourceManager, new object[]
			{
				percentageValue
			});
		}

		public static LocalizedString ComponentHealthHeartbeatEscalationMessageUnhealthy(int heartbeatThreshold, int monitoringIntervalMinutes)
		{
			return new LocalizedString("ComponentHealthHeartbeatEscalationMessageUnhealthy", Strings.ResourceManager, new object[]
			{
				heartbeatThreshold,
				monitoringIntervalMinutes
			});
		}

		public static LocalizedString ProvisionedDCBelowMinimumEscalationMessage
		{
			get
			{
				return new LocalizedString("ProvisionedDCBelowMinimumEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KerbAuthFailureEscalationMessage
		{
			get
			{
				return new LocalizedString("KerbAuthFailureEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequestsQueuedOver500EscalationMessage
		{
			get
			{
				return new LocalizedString("RequestsQueuedOver500EscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PopImapGuid
		{
			get
			{
				return new LocalizedString("PopImapGuid", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchResourceLoadUnhealthy(string databaseName, string copyStatus, string moveJob, string resource, string resourceHistory)
		{
			return new LocalizedString("SearchResourceLoadUnhealthy", Strings.ResourceManager, new object[]
			{
				databaseName,
				copyStatus,
				moveJob,
				resource,
				resourceHistory
			});
		}

		public static LocalizedString MaintenanceFailureEscalationSubject
		{
			get
			{
				return new LocalizedString("MaintenanceFailureEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TransportServerDownEscalationMessage
		{
			get
			{
				return new LocalizedString("TransportServerDownEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnmonitoredDatabaseEscalationMessage(string database)
		{
			return new LocalizedString("UnmonitoredDatabaseEscalationMessage", Strings.ResourceManager, new object[]
			{
				database
			});
		}

		public static LocalizedString PopImapEndpoint
		{
			get
			{
				return new LocalizedString("PopImapEndpoint", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LongRunningWerMgrTriggerWarningThresholdSubject(string processName)
		{
			return new LocalizedString("LongRunningWerMgrTriggerWarningThresholdSubject", Strings.ResourceManager, new object[]
			{
				processName
			});
		}

		public static LocalizedString EseDbTimeSmallerEscalationMessage(string machine, string database)
		{
			return new LocalizedString("EseDbTimeSmallerEscalationMessage", Strings.ResourceManager, new object[]
			{
				machine,
				database
			});
		}

		public static LocalizedString NtlmConnectivityEscalationMessage
		{
			get
			{
				return new LocalizedString("NtlmConnectivityEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchProcessCrashingTooManyTimesEscalationMessage(string processName, int count, int seconds)
		{
			return new LocalizedString("SearchProcessCrashingTooManyTimesEscalationMessage", Strings.ResourceManager, new object[]
			{
				processName,
				count,
				seconds
			});
		}

		public static LocalizedString GetDiagnosticInfoTimeoutMessage(int timeoutSeconds)
		{
			return new LocalizedString("GetDiagnosticInfoTimeoutMessage", Strings.ResourceManager, new object[]
			{
				timeoutSeconds
			});
		}

		public static LocalizedString WatermarksBehind(string database)
		{
			return new LocalizedString("WatermarksBehind", Strings.ResourceManager, new object[]
			{
				database
			});
		}

		public static LocalizedString ClusterGroupDownEscalationMessage(int suppression)
		{
			return new LocalizedString("ClusterGroupDownEscalationMessage", Strings.ResourceManager, new object[]
			{
				suppression
			});
		}

		public static LocalizedString PswsEscalationBody(string serverName)
		{
			return new LocalizedString("PswsEscalationBody", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString EseDbTimeSmallerEscalationSubject(string component, string machine, string database)
		{
			return new LocalizedString("EseDbTimeSmallerEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine,
				database
			});
		}

		public static LocalizedString OabMailboxFileNotFound(string fileName)
		{
			return new LocalizedString("OabMailboxFileNotFound", Strings.ResourceManager, new object[]
			{
				fileName
			});
		}

		public static LocalizedString SearchCatalogInFailedAndSuspendedState(string databaseName, string stateString, string healthyCopyServer, string timestamp, string localServer)
		{
			return new LocalizedString("SearchCatalogInFailedAndSuspendedState", Strings.ResourceManager, new object[]
			{
				databaseName,
				stateString,
				healthyCopyServer,
				timestamp,
				localServer
			});
		}

		public static LocalizedString EseSinglePageLogicalCorruptionDetectedSubject(string component, string machine, string database)
		{
			return new LocalizedString("EseSinglePageLogicalCorruptionDetectedSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine,
				database
			});
		}

		public static LocalizedString OabTooManyWebAppStartsSubject
		{
			get
			{
				return new LocalizedString("OabTooManyWebAppStartsSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StoreAdminRPCInterfaceEscalationEscalationMessageDc(TimeSpan duration)
		{
			return new LocalizedString("StoreAdminRPCInterfaceEscalationEscalationMessageDc", Strings.ResourceManager, new object[]
			{
				duration
			});
		}

		public static LocalizedString CafeEscalationSubjectUnhealthy
		{
			get
			{
				return new LocalizedString("CafeEscalationSubjectUnhealthy", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DnsHostRecordProbeName
		{
			get
			{
				return new LocalizedString("DnsHostRecordProbeName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ELCArchiveDumpsterWarningEscalationSubject
		{
			get
			{
				return new LocalizedString("ELCArchiveDumpsterWarningEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VersionBucketsAllocatedEscalationSubject(TimeSpan duration)
		{
			return new LocalizedString("VersionBucketsAllocatedEscalationSubject", Strings.ResourceManager, new object[]
			{
				duration
			});
		}

		public static LocalizedString OwaDeepTestEscalationBody(string serverName, string logPath)
		{
			return new LocalizedString("OwaDeepTestEscalationBody", Strings.ResourceManager, new object[]
			{
				serverName,
				logPath
			});
		}

		public static LocalizedString JournalFilterAgentEscalationMessage
		{
			get
			{
				return new LocalizedString("JournalFilterAgentEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WacDiscoveryFailureSubject
		{
			get
			{
				return new LocalizedString("WacDiscoveryFailureSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CASRoutingLatencyEscalationSubject
		{
			get
			{
				return new LocalizedString("CASRoutingLatencyEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MRSLongQueueScanMessage(string dumpDirectory)
		{
			return new LocalizedString("MRSLongQueueScanMessage", Strings.ResourceManager, new object[]
			{
				dumpDirectory
			});
		}

		public static LocalizedString SearchIndexFailure(string failureRate, string threshold, string completedCallbacks, string failedCallbacks, string minutes)
		{
			return new LocalizedString("SearchIndexFailure", Strings.ResourceManager, new object[]
			{
				failureRate,
				threshold,
				completedCallbacks,
				failedCallbacks,
				minutes
			});
		}

		public static LocalizedString EDSJobPoisonedEscalationMessage
		{
			get
			{
				return new LocalizedString("EDSJobPoisonedEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MRSRPCPingSubject(string service)
		{
			return new LocalizedString("MRSRPCPingSubject", Strings.ResourceManager, new object[]
			{
				service
			});
		}

		public static LocalizedString UserThrottlingLockedOutUsersSubject(string protocol)
		{
			return new LocalizedString("UserThrottlingLockedOutUsersSubject", Strings.ResourceManager, new object[]
			{
				protocol
			});
		}

		public static LocalizedString JournalFilterAgentEscalationSubject
		{
			get
			{
				return new LocalizedString("JournalFilterAgentEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DnsHostRecordMonitorName
		{
			get
			{
				return new LocalizedString("DnsHostRecordMonitorName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClusterHangEscalationSubject(string component, string target)
		{
			return new LocalizedString("ClusterHangEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				target
			});
		}

		public static LocalizedString RcaEscalationSubject(string monitorIdentity, string target)
		{
			return new LocalizedString("RcaEscalationSubject", Strings.ResourceManager, new object[]
			{
				monitorIdentity,
				target
			});
		}

		public static LocalizedString VersionStore2008EscalationMessage
		{
			get
			{
				return new LocalizedString("VersionStore2008EscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OwaDeepTestEscalationSubject(string serverName)
		{
			return new LocalizedString("OwaDeepTestEscalationSubject", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString DnsServiceMonitorName
		{
			get
			{
				return new LocalizedString("DnsServiceMonitorName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClusterServiceDownEscalationMessage(int suppression)
		{
			return new LocalizedString("ClusterServiceDownEscalationMessage", Strings.ResourceManager, new object[]
			{
				suppression
			});
		}

		public static LocalizedString EscalationMessagePercentUnhealthy(string customMessage)
		{
			return new LocalizedString("EscalationMessagePercentUnhealthy", Strings.ResourceManager, new object[]
			{
				customMessage
			});
		}

		public static LocalizedString DatabaseAvailabilityHelpString
		{
			get
			{
				return new LocalizedString("DatabaseAvailabilityHelpString", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchIndexCopyStatus(string copyName, string databaseStatus, string catalogStatus, string errorMessage)
		{
			return new LocalizedString("SearchIndexCopyStatus", Strings.ResourceManager, new object[]
			{
				copyName,
				databaseStatus,
				catalogStatus,
				errorMessage
			});
		}

		public static LocalizedString PublicFolderMailboxQuotaEscalationSubject
		{
			get
			{
				return new LocalizedString("PublicFolderMailboxQuotaEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HealthSetEscalationSubjectPrefix
		{
			get
			{
				return new LocalizedString("HealthSetEscalationSubjectPrefix", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DHCPServerDeclinesEscalationMessage
		{
			get
			{
				return new LocalizedString("DHCPServerDeclinesEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JobobjectCpuExceededThresholdSubject(string jobObjectName)
		{
			return new LocalizedString("JobobjectCpuExceededThresholdSubject", Strings.ResourceManager, new object[]
			{
				jobObjectName
			});
		}

		public static LocalizedString FindPlacesRequestsError(string serverName)
		{
			return new LocalizedString("FindPlacesRequestsError", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString TrustMonitorProbeEscalationMessage
		{
			get
			{
				return new LocalizedString("TrustMonitorProbeEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidIncludedAssistantType
		{
			get
			{
				return new LocalizedString("InvalidIncludedAssistantType", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteStoreAdminRPCInterfaceEscalationEscalationMessageDc(TimeSpan duration)
		{
			return new LocalizedString("RemoteStoreAdminRPCInterfaceEscalationEscalationMessageDc", Strings.ResourceManager, new object[]
			{
				duration
			});
		}

		public static LocalizedString ReplicationDisabledEscalationMessage
		{
			get
			{
				return new LocalizedString("ReplicationDisabledEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EseInconsistentDataDetectedEscalationMessage(string machine, string database)
		{
			return new LocalizedString("EseInconsistentDataDetectedEscalationMessage", Strings.ResourceManager, new object[]
			{
				machine,
				database
			});
		}

		public static LocalizedString DatabaseNotFoundInADException(string databaseGuid)
		{
			return new LocalizedString("DatabaseNotFoundInADException", Strings.ResourceManager, new object[]
			{
				databaseGuid
			});
		}

		public static LocalizedString PrivateWorkingSetExceededWarningThresholdSubject(string processName)
		{
			return new LocalizedString("PrivateWorkingSetExceededWarningThresholdSubject", Strings.ResourceManager, new object[]
			{
				processName
			});
		}

		public static LocalizedString MRSRepeatedlyCrashingEscalationSubject(string service)
		{
			return new LocalizedString("MRSRepeatedlyCrashingEscalationSubject", Strings.ResourceManager, new object[]
			{
				service
			});
		}

		public static LocalizedString ProvisioningBigVolumeErrorEscalateResponderName
		{
			get
			{
				return new LocalizedString("ProvisioningBigVolumeErrorEscalateResponderName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CASRoutingLatencyEscalationBody
		{
			get
			{
				return new LocalizedString("CASRoutingLatencyEscalationBody", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProcessorTimeExceededErrorThresholdMessage(string processName)
		{
			return new LocalizedString("ProcessorTimeExceededErrorThresholdMessage", Strings.ResourceManager, new object[]
			{
				processName
			});
		}

		public static LocalizedString MRSLongQueueScanSubject(string service)
		{
			return new LocalizedString("MRSLongQueueScanSubject", Strings.ResourceManager, new object[]
			{
				service
			});
		}

		public static LocalizedString StalledCopyEscalationMessage(string database, string threshold)
		{
			return new LocalizedString("StalledCopyEscalationMessage", Strings.ResourceManager, new object[]
			{
				database,
				threshold
			});
		}

		public static LocalizedString OabProtocolEscalationSubject(string serverName)
		{
			return new LocalizedString("OabProtocolEscalationSubject", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString SecurityAlertMalwareDetectedEscalationMessage
		{
			get
			{
				return new LocalizedString("SecurityAlertMalwareDetectedEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProcessorTimeExceededWarningThresholdWithAffinitizationMessage(string processName)
		{
			return new LocalizedString("ProcessorTimeExceededWarningThresholdWithAffinitizationMessage", Strings.ResourceManager, new object[]
			{
				processName
			});
		}

		public static LocalizedString InvalidSystemMailbox(string mailboxDatabaseName)
		{
			return new LocalizedString("InvalidSystemMailbox", Strings.ResourceManager, new object[]
			{
				mailboxDatabaseName
			});
		}

		public static LocalizedString SynchronousAuditSearchAvailabilityFailureEscalationBody
		{
			get
			{
				return new LocalizedString("SynchronousAuditSearchAvailabilityFailureEscalationBody", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CafeEscalationMessageUnhealthyForDC(string cafeArrayName)
		{
			return new LocalizedString("CafeEscalationMessageUnhealthyForDC", Strings.ResourceManager, new object[]
			{
				cafeArrayName
			});
		}

		public static LocalizedString UMSipOptionsToUMCallRouterServiceFailedEscalationSubject(string serverName)
		{
			return new LocalizedString("UMSipOptionsToUMCallRouterServiceFailedEscalationSubject", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString MaintenanceTimeoutEscalationMessage
		{
			get
			{
				return new LocalizedString("MaintenanceTimeoutEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeltaSyncServiceEndpointsLoadFailedEscalationMessage
		{
			get
			{
				return new LocalizedString("DeltaSyncServiceEndpointsLoadFailedEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PutMultipleDCIntoMMSuccessNotificationMessage(string dcFQDN)
		{
			return new LocalizedString("PutMultipleDCIntoMMSuccessNotificationMessage", Strings.ResourceManager, new object[]
			{
				dcFQDN
			});
		}

		public static LocalizedString InferenceTrainingDataCollectionRepeatedCrashEscalationMessage(string processName, int minCount, int durationMinutes)
		{
			return new LocalizedString("InferenceTrainingDataCollectionRepeatedCrashEscalationMessage", Strings.ResourceManager, new object[]
			{
				processName,
				minCount,
				durationMinutes
			});
		}

		public static LocalizedString SearchInstantSearchStxException(string query, string mailboxSmtpAddress, string exception, string queryStats)
		{
			return new LocalizedString("SearchInstantSearchStxException", Strings.ResourceManager, new object[]
			{
				query,
				mailboxSmtpAddress,
				exception,
				queryStats
			});
		}

		public static LocalizedString SchedulingLatencyEscalateResponderMessage
		{
			get
			{
				return new LocalizedString("SchedulingLatencyEscalateResponderMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PowerShellProfileEscalationMessage
		{
			get
			{
				return new LocalizedString("PowerShellProfileEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OAuthRequestFailureEscalationSubject
		{
			get
			{
				return new LocalizedString("OAuthRequestFailureEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ActiveSyncCustomerTouchPointEscalationBodyDC(string serverName, string probeName)
		{
			return new LocalizedString("ActiveSyncCustomerTouchPointEscalationBodyDC", Strings.ResourceManager, new object[]
			{
				serverName,
				probeName
			});
		}

		public static LocalizedString HealthSetsStates(string command)
		{
			return new LocalizedString("HealthSetsStates", Strings.ResourceManager, new object[]
			{
				command
			});
		}

		public static LocalizedString LongRunningWerMgrTriggerWarningThresholdMessage(string processName)
		{
			return new LocalizedString("LongRunningWerMgrTriggerWarningThresholdMessage", Strings.ResourceManager, new object[]
			{
				processName
			});
		}

		public static LocalizedString ReplServiceDownEscalationSubject(string component, string machine)
		{
			return new LocalizedString("ReplServiceDownEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine
			});
		}

		public static LocalizedString SearchIndexBacklogWithProcessingRateAndHistory(string databaseName, string backlog, string retryQueue, string lastTime, string lastBacklog, string lastRetryQueue, string completedCount, string processingRate, string minutes, string upTime, string serverStatus)
		{
			return new LocalizedString("SearchIndexBacklogWithProcessingRateAndHistory", Strings.ResourceManager, new object[]
			{
				databaseName,
				backlog,
				retryQueue,
				lastTime,
				lastBacklog,
				lastRetryQueue,
				completedCount,
				processingRate,
				minutes,
				upTime,
				serverStatus
			});
		}

		public static LocalizedString MultipleRecipientsFound(string queryFilter)
		{
			return new LocalizedString("MultipleRecipientsFound", Strings.ResourceManager, new object[]
			{
				queryFilter
			});
		}

		public static LocalizedString CafeThreadCountSubjectUnhealthy(string appPool)
		{
			return new LocalizedString("CafeThreadCountSubjectUnhealthy", Strings.ResourceManager, new object[]
			{
				appPool
			});
		}

		public static LocalizedString PotentialInsufficientRedundancyEscalationMessage(string machine)
		{
			return new LocalizedString("PotentialInsufficientRedundancyEscalationMessage", Strings.ResourceManager, new object[]
			{
				machine
			});
		}

		public static LocalizedString OabMailboxNoOrgMailbox
		{
			get
			{
				return new LocalizedString("OabMailboxNoOrgMailbox", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidUserName(string userName)
		{
			return new LocalizedString("InvalidUserName", Strings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString SearchInstantSearchStxEscalationMessage(string databaseName, int threshold, int interval)
		{
			return new LocalizedString("SearchInstantSearchStxEscalationMessage", Strings.ResourceManager, new object[]
			{
				databaseName,
				threshold,
				interval
			});
		}

		public static LocalizedString MailboxAssistantsBehindWatermarksEscalationMessageDc(TimeSpan ageThreshold, TimeSpan duration, string invokeNowCommand, string unhealthyMonitorsCommand)
		{
			return new LocalizedString("MailboxAssistantsBehindWatermarksEscalationMessageDc", Strings.ResourceManager, new object[]
			{
				ageThreshold,
				duration,
				invokeNowCommand,
				unhealthyMonitorsCommand
			});
		}

		public static LocalizedString ExchangeCrashExceededErrorThresholdSubject(string processName)
		{
			return new LocalizedString("ExchangeCrashExceededErrorThresholdSubject", Strings.ResourceManager, new object[]
			{
				processName
			});
		}

		public static LocalizedString PushNotificationEnterpriseEmptyDomain
		{
			get
			{
				return new LocalizedString("PushNotificationEnterpriseEmptyDomain", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RcaWorkItemDescriptionEntry(string alertMask, string exceptionMessage)
		{
			return new LocalizedString("RcaWorkItemDescriptionEntry", Strings.ResourceManager, new object[]
			{
				alertMask,
				exceptionMessage
			});
		}

		public static LocalizedString SearchIndexActiveCopyUnhealthy(string databaseName, string status, string errorMessage, string diagnosticInfoError, string nodesInfo)
		{
			return new LocalizedString("SearchIndexActiveCopyUnhealthy", Strings.ResourceManager, new object[]
			{
				databaseName,
				status,
				errorMessage,
				diagnosticInfoError,
				nodesInfo
			});
		}

		public static LocalizedString AssistantsOutOfSlaMessage
		{
			get
			{
				return new LocalizedString("AssistantsOutOfSlaMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EwsAutodEscalationSubjectUnhealthy
		{
			get
			{
				return new LocalizedString("EwsAutodEscalationSubjectUnhealthy", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UMCallRouterRecentMissedCallNotificationProxyFailedEscalationMessageString(int percentageValue)
		{
			return new LocalizedString("UMCallRouterRecentMissedCallNotificationProxyFailedEscalationMessageString", Strings.ResourceManager, new object[]
			{
				percentageValue
			});
		}

		public static LocalizedString HttpConnectivityEscalationSubject
		{
			get
			{
				return new LocalizedString("HttpConnectivityEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchNumDiskPartsEscalationMessage(string databaseName)
		{
			return new LocalizedString("SearchNumDiskPartsEscalationMessage", Strings.ResourceManager, new object[]
			{
				databaseName
			});
		}

		public static LocalizedString DatabaseObjectNotFoundException
		{
			get
			{
				return new LocalizedString("DatabaseObjectNotFoundException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InferenceClassificationRepeatedCrashEscalationMessage(string processName, int minCount, int durationMinutes)
		{
			return new LocalizedString("InferenceClassificationRepeatedCrashEscalationMessage", Strings.ResourceManager, new object[]
			{
				processName,
				minCount,
				durationMinutes
			});
		}

		public static LocalizedString AssistantsActiveDatabaseError(string error)
		{
			return new LocalizedString("AssistantsActiveDatabaseError", Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString DatabaseDiskReadLatencyEscalationSubject(TimeSpan duration)
		{
			return new LocalizedString("DatabaseDiskReadLatencyEscalationSubject", Strings.ResourceManager, new object[]
			{
				duration
			});
		}

		public static LocalizedString FSMODCNotProvisionedEscalationMessage
		{
			get
			{
				return new LocalizedString("FSMODCNotProvisionedEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NumberOfActiveBackgroundTasksEscalationMessageDc(string databaseName, int threshold, TimeSpan duration)
		{
			return new LocalizedString("NumberOfActiveBackgroundTasksEscalationMessageDc", Strings.ResourceManager, new object[]
			{
				databaseName,
				threshold,
				duration
			});
		}

		public static LocalizedString AssistantsNotRunningToCompletionMessage
		{
			get
			{
				return new LocalizedString("AssistantsNotRunningToCompletionMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserthtottlingLockedOutUsersMessage(string protocol, int threshold, int sample)
		{
			return new LocalizedString("UserthtottlingLockedOutUsersMessage", Strings.ResourceManager, new object[]
			{
				protocol,
				threshold,
				sample
			});
		}

		public static LocalizedString SearchCatalogHasError(string databaseName, string error, string serverName)
		{
			return new LocalizedString("SearchCatalogHasError", Strings.ResourceManager, new object[]
			{
				databaseName,
				error,
				serverName
			});
		}

		public static LocalizedString DLExpansionEscalationSubject
		{
			get
			{
				return new LocalizedString("DLExpansionEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NTFSCorruptionEscalationSubject(string component, string machine, string database)
		{
			return new LocalizedString("NTFSCorruptionEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine,
				database
			});
		}

		public static LocalizedString ActiveManagerUnhealthyEscalationMessage(string machine, int restartService, int bugcheckTime)
		{
			return new LocalizedString("ActiveManagerUnhealthyEscalationMessage", Strings.ResourceManager, new object[]
			{
				machine,
				restartService,
				bugcheckTime
			});
		}

		public static LocalizedString RcaTaskOutlineFailed
		{
			get
			{
				return new LocalizedString("RcaTaskOutlineFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CASRoutingFailureEscalationBody
		{
			get
			{
				return new LocalizedString("CASRoutingFailureEscalationBody", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DnsServiceProbeName
		{
			get
			{
				return new LocalizedString("DnsServiceProbeName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HighDiskLatencyEscalationSubject(string component, string machine, string threshold, string suppresion)
		{
			return new LocalizedString("HighDiskLatencyEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine,
				threshold,
				suppresion
			});
		}

		public static LocalizedString TooManyDatabaseMountedEscalationSubject(string component, string machine)
		{
			return new LocalizedString("TooManyDatabaseMountedEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine
			});
		}

		public static LocalizedString SynchronousAuditSearchAvailabilityFailureEscalationSubject
		{
			get
			{
				return new LocalizedString("SynchronousAuditSearchAvailabilityFailureEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotRebuildIndexEscalationMessage
		{
			get
			{
				return new LocalizedString("CannotRebuildIndexEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UMPipelineFullEscalationMessageString
		{
			get
			{
				return new LocalizedString("UMPipelineFullEscalationMessageString", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PasswordVerificationFailed(string mailboxDatabaseName, string upn, string error)
		{
			return new LocalizedString("PasswordVerificationFailed", Strings.ResourceManager, new object[]
			{
				mailboxDatabaseName,
				upn,
				error
			});
		}

		public static LocalizedString LocalMachineDriveEncryptionStateEscalationMessage(string volumes, string serverName)
		{
			return new LocalizedString("LocalMachineDriveEncryptionStateEscalationMessage", Strings.ResourceManager, new object[]
			{
				volumes,
				serverName
			});
		}

		public static LocalizedString DatabaseNotAttachedReadOnly
		{
			get
			{
				return new LocalizedString("DatabaseNotAttachedReadOnly", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UMWorkerProcessRecentCallRejectedEscalationMessageString(int percentageValue)
		{
			return new LocalizedString("UMWorkerProcessRecentCallRejectedEscalationMessageString", Strings.ResourceManager, new object[]
			{
				percentageValue
			});
		}

		public static LocalizedString SingleAvailableDatabaseCopyEscalationSubject(string component, string machine)
		{
			return new LocalizedString("SingleAvailableDatabaseCopyEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine
			});
		}

		public static LocalizedString InfrastructureValidationMessage
		{
			get
			{
				return new LocalizedString("InfrastructureValidationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PopProxyTestEscalationBodyDC(string serverName, string probeName)
		{
			return new LocalizedString("PopProxyTestEscalationBodyDC", Strings.ResourceManager, new object[]
			{
				serverName,
				probeName
			});
		}

		public static LocalizedString OwaTooManyHttpErrorResponsesEncounteredSubject
		{
			get
			{
				return new LocalizedString("OwaTooManyHttpErrorResponsesEncounteredSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CPUOverThresholdWarningEscalationSubject
		{
			get
			{
				return new LocalizedString("CPUOverThresholdWarningEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StoreNotificationEscalationMessage(string dbName)
		{
			return new LocalizedString("StoreNotificationEscalationMessage", Strings.ResourceManager, new object[]
			{
				dbName
			});
		}

		public static LocalizedString SubscriptionSlaMissedEscalationMessage
		{
			get
			{
				return new LocalizedString("SubscriptionSlaMissedEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ActiveDirectoryConnectivityConfigDCEscalationMessage
		{
			get
			{
				return new LocalizedString("ActiveDirectoryConnectivityConfigDCEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OwaMailboxRoleNotInstalled
		{
			get
			{
				return new LocalizedString("OwaMailboxRoleNotInstalled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CafeEscalationMessageUnhealthy(string recoveryDetails, string cafeArrayName)
		{
			return new LocalizedString("CafeEscalationMessageUnhealthy", Strings.ResourceManager, new object[]
			{
				recoveryDetails,
				cafeArrayName
			});
		}

		public static LocalizedString PushNotificationEnterpriseNetworkingError
		{
			get
			{
				return new LocalizedString("PushNotificationEnterpriseNetworkingError", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PopCustomerTouchPointEscalationBodyDC(string serverName, string probeName)
		{
			return new LocalizedString("PopCustomerTouchPointEscalationBodyDC", Strings.ResourceManager, new object[]
			{
				serverName,
				probeName
			});
		}

		public static LocalizedString PopEscalationSubject(string probeName, string serverName)
		{
			return new LocalizedString("PopEscalationSubject", Strings.ResourceManager, new object[]
			{
				probeName,
				serverName
			});
		}

		public static LocalizedString DatabaseAvailabilityTimeout
		{
			get
			{
				return new LocalizedString("DatabaseAvailabilityTimeout", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseGuidNotFound
		{
			get
			{
				return new LocalizedString("DatabaseGuidNotFound", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchIndexBacklogAggregatedEscalationMessage
		{
			get
			{
				return new LocalizedString("SearchIndexBacklogAggregatedEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFolderLocalEWSLogonEscalationMessage
		{
			get
			{
				return new LocalizedString("PublicFolderLocalEWSLogonEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TenantRelocationErrorsFoundExceptionMessage
		{
			get
			{
				return new LocalizedString("TenantRelocationErrorsFoundExceptionMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UMCallRouterCertificateNearExpiryEscalationMessage
		{
			get
			{
				return new LocalizedString("UMCallRouterCertificateNearExpiryEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SlowADWritesEscalationMessage
		{
			get
			{
				return new LocalizedString("SlowADWritesEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InferenceComponentDisabled(string details)
		{
			return new LocalizedString("InferenceComponentDisabled", Strings.ResourceManager, new object[]
			{
				details
			});
		}

		public static LocalizedString RcaEscalationBodyEnt
		{
			get
			{
				return new LocalizedString("RcaEscalationBodyEnt", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchQueryStxEscalationMessage(string databaseName, int threshold, int interval)
		{
			return new LocalizedString("SearchQueryStxEscalationMessage", Strings.ResourceManager, new object[]
			{
				databaseName,
				threshold,
				interval
			});
		}

		public static LocalizedString DatabaseSizeEscalationMessageEnt(string invokeNowCommand, string unhealthyMonitorsCommand)
		{
			return new LocalizedString("DatabaseSizeEscalationMessageEnt", Strings.ResourceManager, new object[]
			{
				invokeNowCommand,
				unhealthyMonitorsCommand
			});
		}

		public static LocalizedString ActiveSyncCustomerTouchPointEscalationBodyENT(string serverName, string probeName)
		{
			return new LocalizedString("ActiveSyncCustomerTouchPointEscalationBodyENT", Strings.ResourceManager, new object[]
			{
				serverName,
				probeName
			});
		}

		public static LocalizedString MailboxDatabasesUnavailable
		{
			get
			{
				return new LocalizedString("MailboxDatabasesUnavailable", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RetryRemoteDeliveryQueueLengthEscalationMessage
		{
			get
			{
				return new LocalizedString("RetryRemoteDeliveryQueueLengthEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HealthSetMonitorsStates(string command)
		{
			return new LocalizedString("HealthSetMonitorsStates", Strings.ResourceManager, new object[]
			{
				command
			});
		}

		public static LocalizedString FailedToUpgradeIndexEscalationMessage
		{
			get
			{
				return new LocalizedString("FailedToUpgradeIndexEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchGetDiagnosticInfoTimeout(int timeoutSeconds)
		{
			return new LocalizedString("SearchGetDiagnosticInfoTimeout", Strings.ResourceManager, new object[]
			{
				timeoutSeconds
			});
		}

		public static LocalizedString EventAssistantsWatermarksHelpString
		{
			get
			{
				return new LocalizedString("EventAssistantsWatermarksHelpString", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidMailboxDatabaseEndpoint(string message)
		{
			return new LocalizedString("InvalidMailboxDatabaseEndpoint", Strings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString InferenceClassifcationSLAEscalationMessage
		{
			get
			{
				return new LocalizedString("InferenceClassifcationSLAEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MRSUnhealthyMessage
		{
			get
			{
				return new LocalizedString("MRSUnhealthyMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UMServiceType
		{
			get
			{
				return new LocalizedString("UMServiceType", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DivergenceBetweenCAAndAD1003EscalationMessage
		{
			get
			{
				return new LocalizedString("DivergenceBetweenCAAndAD1003EscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PopDeepTestEscalationBodyDC(string serverName, string probeName)
		{
			return new LocalizedString("PopDeepTestEscalationBodyDC", Strings.ResourceManager, new object[]
			{
				serverName,
				probeName
			});
		}

		public static LocalizedString LocalMachineDriveEncryptionLockEscalationSubject(string serverName)
		{
			return new LocalizedString("LocalMachineDriveEncryptionLockEscalationSubject", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString SearchGracefulDegradationStatus(string timestamp)
		{
			return new LocalizedString("SearchGracefulDegradationStatus", Strings.ResourceManager, new object[]
			{
				timestamp
			});
		}

		public static LocalizedString EseDbDivergenceDetectedSubject(string component, string machine, string database)
		{
			return new LocalizedString("EseDbDivergenceDetectedSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine,
				database
			});
		}

		public static LocalizedString SearchIndexActiveCopySeedingWithHealthyCopy(string databaseName, string healthyCopyServerName)
		{
			return new LocalizedString("SearchIndexActiveCopySeedingWithHealthyCopy", Strings.ResourceManager, new object[]
			{
				databaseName,
				healthyCopyServerName
			});
		}

		public static LocalizedString NumberOfActiveBackgroundTasksEscalationSubject(string databaseName, int threshold, TimeSpan duration)
		{
			return new LocalizedString("NumberOfActiveBackgroundTasksEscalationSubject", Strings.ResourceManager, new object[]
			{
				databaseName,
				threshold,
				duration
			});
		}

		public static LocalizedString StoreMaintenanceAssistantEscalationSubject(TimeSpan duration)
		{
			return new LocalizedString("StoreMaintenanceAssistantEscalationSubject", Strings.ResourceManager, new object[]
			{
				duration
			});
		}

		public static LocalizedString AssistantsActiveDatabaseMessage
		{
			get
			{
				return new LocalizedString("AssistantsActiveDatabaseMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StoreNotificationEscalationSubject(string component, string machine, string database)
		{
			return new LocalizedString("StoreNotificationEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine,
				database
			});
		}

		public static LocalizedString RwsDatamartAvailabilityEscalationBody(string serverName, string cName)
		{
			return new LocalizedString("RwsDatamartAvailabilityEscalationBody", Strings.ResourceManager, new object[]
			{
				serverName,
				cName
			});
		}

		public static LocalizedString ComponentHealthPercentFailureEscalationMessageHealthy(int percentFailureThreshold, int monitoringIntervalMinutes)
		{
			return new LocalizedString("ComponentHealthPercentFailureEscalationMessageHealthy", Strings.ResourceManager, new object[]
			{
				percentFailureThreshold,
				monitoringIntervalMinutes
			});
		}

		public static LocalizedString ActiveDatabaseAvailabilityEscalationMessageDc(string invokeNowCommand, string unhealthyMonitorsCommand)
		{
			return new LocalizedString("ActiveDatabaseAvailabilityEscalationMessageDc", Strings.ResourceManager, new object[]
			{
				invokeNowCommand,
				unhealthyMonitorsCommand
			});
		}

		public static LocalizedString SchedulingLatencyEscalateResponderSubject
		{
			get
			{
				return new LocalizedString("SchedulingLatencyEscalateResponderSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OfficeGraphTransportDeliveryAgentFailureEscalationMessage
		{
			get
			{
				return new LocalizedString("OfficeGraphTransportDeliveryAgentFailureEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchIndexDatabaseCopyStatus(string message, string databaseCopyStatus)
		{
			return new LocalizedString("SearchIndexDatabaseCopyStatus", Strings.ResourceManager, new object[]
			{
				message,
				databaseCopyStatus
			});
		}

		public static LocalizedString OfficeGraphMessageTracingPluginFailureEscalationMessage
		{
			get
			{
				return new LocalizedString("OfficeGraphMessageTracingPluginFailureEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogicalDiskFreeMegabytesEscalationMessage
		{
			get
			{
				return new LocalizedString("LogicalDiskFreeMegabytesEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OwaIMLogAnalyzerSubject
		{
			get
			{
				return new LocalizedString("OwaIMLogAnalyzerSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OabMailboxManifestAddressListEmpty(string addrList)
		{
			return new LocalizedString("OabMailboxManifestAddressListEmpty", Strings.ResourceManager, new object[]
			{
				addrList
			});
		}

		public static LocalizedString PopCustomerTouchPointEscalationBodyENT(string serverName, string probeName)
		{
			return new LocalizedString("PopCustomerTouchPointEscalationBodyENT", Strings.ResourceManager, new object[]
			{
				serverName,
				probeName
			});
		}

		public static LocalizedString ServerInMaintenanceModeForTooLongEscalationMessage(string machine, string threshold)
		{
			return new LocalizedString("ServerInMaintenanceModeForTooLongEscalationMessage", Strings.ResourceManager, new object[]
			{
				machine,
				threshold
			});
		}

		public static LocalizedString RaidDegradedEscalationMessage
		{
			get
			{
				return new LocalizedString("RaidDegradedEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BulkProvisioningNoProgressEscalationMessage
		{
			get
			{
				return new LocalizedString("BulkProvisioningNoProgressEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LocalMachineDriveEncryptionStateEscalationSubject(string serverName)
		{
			return new LocalizedString("LocalMachineDriveEncryptionStateEscalationSubject", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString AssistantsOutOfSlaSubject
		{
			get
			{
				return new LocalizedString("AssistantsOutOfSlaSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OwaTooManyHttpErrorResponsesEncounteredBody
		{
			get
			{
				return new LocalizedString("OwaTooManyHttpErrorResponsesEncounteredBody", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EseLostFlushDetectedEscalationSubject(string component, string machine, string database)
		{
			return new LocalizedString("EseLostFlushDetectedEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine,
				database
			});
		}

		public static LocalizedString CheckSumEscalationMessage
		{
			get
			{
				return new LocalizedString("CheckSumEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFolderLocalEWSLogonEscalationSubject
		{
			get
			{
				return new LocalizedString("PublicFolderLocalEWSLogonEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapSelfTestEscalationBodyDC(string serverName, string probeName)
		{
			return new LocalizedString("ImapSelfTestEscalationBodyDC", Strings.ResourceManager, new object[]
			{
				serverName,
				probeName
			});
		}

		public static LocalizedString ArchiveNamePrefix(string primaryMailboxName)
		{
			return new LocalizedString("ArchiveNamePrefix", Strings.ResourceManager, new object[]
			{
				primaryMailboxName
			});
		}

		public static LocalizedString ReplServiceCrashEscalationSubject(string component, string machine, int times, int hour)
		{
			return new LocalizedString("ReplServiceCrashEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine,
				times,
				hour
			});
		}

		public static LocalizedString EventAssistantsProcessRepeatedlyCrashingEscalationMessageDc(string processName, int count, TimeSpan duration)
		{
			return new LocalizedString("EventAssistantsProcessRepeatedlyCrashingEscalationMessageDc", Strings.ResourceManager, new object[]
			{
				processName,
				count,
				duration
			});
		}

		public static LocalizedString UMServerAddress
		{
			get
			{
				return new LocalizedString("UMServerAddress", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CafeArrayNameCouldNotBeRetrieved
		{
			get
			{
				return new LocalizedString("CafeArrayNameCouldNotBeRetrieved", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ComponentHealthHeartbeatEscalationMessageHealthy(int heartbeatThreshold, int monitoringIntervalMinutes)
		{
			return new LocalizedString("ComponentHealthHeartbeatEscalationMessageHealthy", Strings.ResourceManager, new object[]
			{
				heartbeatThreshold,
				monitoringIntervalMinutes
			});
		}

		public static LocalizedString OwaSelfTestEscalationSubject(string serverName)
		{
			return new LocalizedString("OwaSelfTestEscalationSubject", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString EseSinglePageLogicalCorruptionDetectedEscalationMessage(string machine, string database)
		{
			return new LocalizedString("EseSinglePageLogicalCorruptionDetectedEscalationMessage", Strings.ResourceManager, new object[]
			{
				machine,
				database
			});
		}

		public static LocalizedString JobobjectCpuExceededThresholdMessage(string jobObjectName)
		{
			return new LocalizedString("JobobjectCpuExceededThresholdMessage", Strings.ResourceManager, new object[]
			{
				jobObjectName
			});
		}

		public static LocalizedString EDSServiceNotRunningEscalationMessage
		{
			get
			{
				return new LocalizedString("EDSServiceNotRunningEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HighDiskLatencyEscalationMessage(string machine, string threshold, string suppresion)
		{
			return new LocalizedString("HighDiskLatencyEscalationMessage", Strings.ResourceManager, new object[]
			{
				machine,
				threshold,
				suppresion
			});
		}

		public static LocalizedString SearchFailToCheckNodeState
		{
			get
			{
				return new LocalizedString("SearchFailToCheckNodeState", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapDeepTestEscalationBodyENT(string serverName, string probeName)
		{
			return new LocalizedString("ImapDeepTestEscalationBodyENT", Strings.ResourceManager, new object[]
			{
				serverName,
				probeName
			});
		}

		public static LocalizedString OfficeGraphMessageTracingPluginLogDirectoryExceedsSizeLimit(string machineName, string logDirectorySizeInMB, string sizeLimitInMB)
		{
			return new LocalizedString("OfficeGraphMessageTracingPluginLogDirectoryExceedsSizeLimit", Strings.ResourceManager, new object[]
			{
				machineName,
				logDirectorySizeInMB,
				sizeLimitInMB
			});
		}

		public static LocalizedString OwaTooManyStartPageFailuresBody
		{
			get
			{
				return new LocalizedString("OwaTooManyStartPageFailuresBody", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailSubmissionBehindWatermarksEscalationMessageDc(TimeSpan ageThreshold, TimeSpan duration, string databaseName, string invokeNowCommand, string unhealthyMonitorsCommand)
		{
			return new LocalizedString("MailSubmissionBehindWatermarksEscalationMessageDc", Strings.ResourceManager, new object[]
			{
				ageThreshold,
				duration,
				databaseName,
				invokeNowCommand,
				unhealthyMonitorsCommand
			});
		}

		public static LocalizedString QuarantineEscalationSubject
		{
			get
			{
				return new LocalizedString("QuarantineEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AsyncAuditLogSearchEscalationMessage(string serverName, string notification)
		{
			return new LocalizedString("AsyncAuditLogSearchEscalationMessage", Strings.ResourceManager, new object[]
			{
				serverName,
				notification
			});
		}

		public static LocalizedString DatabaseLogicalPhysicalSizeRatioEscalationSubject(TimeSpan duration)
		{
			return new LocalizedString("DatabaseLogicalPhysicalSizeRatioEscalationSubject", Strings.ResourceManager, new object[]
			{
				duration
			});
		}

		public static LocalizedString HostControllerServiceRunningMessage
		{
			get
			{
				return new LocalizedString("HostControllerServiceRunningMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServiceNotRunningEscalationMessageDc(string serviceName)
		{
			return new LocalizedString("ServiceNotRunningEscalationMessageDc", Strings.ResourceManager, new object[]
			{
				serviceName
			});
		}

		public static LocalizedString SearchGracefulDegradationManagerFailureEscalationMessage
		{
			get
			{
				return new LocalizedString("SearchGracefulDegradationManagerFailureEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseRPCLatencyEscalationMessageDc(string databaseName, int latency, TimeSpan duration)
		{
			return new LocalizedString("DatabaseRPCLatencyEscalationMessageDc", Strings.ResourceManager, new object[]
			{
				databaseName,
				latency,
				duration
			});
		}

		public static LocalizedString ProvisioningBigVolumeErrorMonitorName
		{
			get
			{
				return new LocalizedString("ProvisioningBigVolumeErrorMonitorName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClusterServiceDownEscalationSubject(string component, string target, int threshold)
		{
			return new LocalizedString("ClusterServiceDownEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				target,
				threshold
			});
		}

		public static LocalizedString OwaTooManyWebAppStartsSubject
		{
			get
			{
				return new LocalizedString("OwaTooManyWebAppStartsSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchQueryStxSimpleQueryMode
		{
			get
			{
				return new LocalizedString("SearchQueryStxSimpleQueryMode", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToGetStoreUsageStatisticsData(string database)
		{
			return new LocalizedString("UnableToGetStoreUsageStatisticsData", Strings.ResourceManager, new object[]
			{
				database
			});
		}

		public static LocalizedString BingServicesLatencyError(string serverName)
		{
			return new LocalizedString("BingServicesLatencyError", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString OABGenTenantOutOfSLASubject
		{
			get
			{
				return new LocalizedString("OABGenTenantOutOfSLASubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UMSipOptionsToUMCallRouterServiceFailedEscalationBody(string serverName)
		{
			return new LocalizedString("UMSipOptionsToUMCallRouterServiceFailedEscalationBody", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString CouldNotAddExchangeSnapInExceptionMessage(string snapInName)
		{
			return new LocalizedString("CouldNotAddExchangeSnapInExceptionMessage", Strings.ResourceManager, new object[]
			{
				snapInName
			});
		}

		public static LocalizedString ELCDumpsterEscalationMessage
		{
			get
			{
				return new LocalizedString("ELCDumpsterEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClassficationEngineErrorsEscalationMessage(string engineName, double threshold, int duration)
		{
			return new LocalizedString("ClassficationEngineErrorsEscalationMessage", Strings.ResourceManager, new object[]
			{
				engineName,
				threshold,
				duration
			});
		}

		public static LocalizedString PopDeepTestEscalationBodyENT(string serverName, string probeName)
		{
			return new LocalizedString("PopDeepTestEscalationBodyENT", Strings.ResourceManager, new object[]
			{
				serverName,
				probeName
			});
		}

		public static LocalizedString DatabaseRepeatedMountsEscalationSubject(string databaseName, TimeSpan duration)
		{
			return new LocalizedString("DatabaseRepeatedMountsEscalationSubject", Strings.ResourceManager, new object[]
			{
				databaseName,
				duration
			});
		}

		public static LocalizedString DirectoryConfigDiscrepancyEscalationMessage
		{
			get
			{
				return new LocalizedString("DirectoryConfigDiscrepancyEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RwsDatamartConnectionEscalationSubject(string serverName)
		{
			return new LocalizedString("RwsDatamartConnectionEscalationSubject", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString NetworkAdapterRecoveryResponderName
		{
			get
			{
				return new LocalizedString("NetworkAdapterRecoveryResponderName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchGracefulDegradationStatusEscalationMessage
		{
			get
			{
				return new LocalizedString("SearchGracefulDegradationStatusEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchIndexBacklog(string databaseName, string backlog, string retryQueue, string upTime, string serverStatus)
		{
			return new LocalizedString("SearchIndexBacklog", Strings.ResourceManager, new object[]
			{
				databaseName,
				backlog,
				retryQueue,
				upTime,
				serverStatus
			});
		}

		public static LocalizedString DnsServiceRestartResponderName
		{
			get
			{
				return new LocalizedString("DnsServiceRestartResponderName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ELCMailboxSLAEscalationMessage
		{
			get
			{
				return new LocalizedString("ELCMailboxSLAEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JournalingEscalationMessage
		{
			get
			{
				return new LocalizedString("JournalingEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MaintenanceTimeoutEscalationSubject
		{
			get
			{
				return new LocalizedString("MaintenanceTimeoutEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContentsUnpredictableEscalationMessage
		{
			get
			{
				return new LocalizedString("ContentsUnpredictableEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EscalationSubjectUnhealthy
		{
			get
			{
				return new LocalizedString("EscalationSubjectUnhealthy", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TransportSyncOutOfSLA(string databaseName, string guid)
		{
			return new LocalizedString("TransportSyncOutOfSLA", Strings.ResourceManager, new object[]
			{
				databaseName,
				guid
			});
		}

		public static LocalizedString EwsAutodEscalationMessageUnhealthy(string recoveryDetails)
		{
			return new LocalizedString("EwsAutodEscalationMessageUnhealthy", Strings.ResourceManager, new object[]
			{
				recoveryDetails
			});
		}

		public static LocalizedString AsyncAuditLogSearchEscalationSubject
		{
			get
			{
				return new LocalizedString("AsyncAuditLogSearchEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SecurityAlertEscalationMessage(string alertName)
		{
			return new LocalizedString("SecurityAlertEscalationMessage", Strings.ResourceManager, new object[]
			{
				alertName
			});
		}

		public static LocalizedString VersionBucketsAllocatedEscalationEscalationMessageEnt(TimeSpan duration, string unhealthyMonitorsCommand)
		{
			return new LocalizedString("VersionBucketsAllocatedEscalationEscalationMessageEnt", Strings.ResourceManager, new object[]
			{
				duration,
				unhealthyMonitorsCommand
			});
		}

		public static LocalizedString DefaultEscalationMessage
		{
			get
			{
				return new LocalizedString("DefaultEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PushNotificationSendPublishNotificationError(string serverName)
		{
			return new LocalizedString("PushNotificationSendPublishNotificationError", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString SearchCatalogTooBig(string databaseName, string databaseSizeGb, string catalogSizeGb, string threshold)
		{
			return new LocalizedString("SearchCatalogTooBig", Strings.ResourceManager, new object[]
			{
				databaseName,
				databaseSizeGb,
				catalogSizeGb,
				threshold
			});
		}

		public static LocalizedString SyntheticReplicationMonitorEscalationMessage
		{
			get
			{
				return new LocalizedString("SyntheticReplicationMonitorEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EDiscoveryEscalationBodyDCHTML
		{
			get
			{
				return new LocalizedString("EDiscoveryEscalationBodyDCHTML", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchMemoryUsageOverThreshold(string memoryUsage)
		{
			return new LocalizedString("SearchMemoryUsageOverThreshold", Strings.ResourceManager, new object[]
			{
				memoryUsage
			});
		}

		public static LocalizedString OwaTooManyLogoffFailuresSubject
		{
			get
			{
				return new LocalizedString("OwaTooManyLogoffFailuresSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CheckProvisionedDCExceptionMessage
		{
			get
			{
				return new LocalizedString("CheckProvisionedDCExceptionMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProvisioningBigVolumeErrorEscalationSubject
		{
			get
			{
				return new LocalizedString("ProvisioningBigVolumeErrorEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LocalMachineDriveProtectedWithDraWithoutDecryptorEscalationSubject(string serverName)
		{
			return new LocalizedString("LocalMachineDriveProtectedWithDraWithoutDecryptorEscalationSubject", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString StoreMaintenanceAssistantEscalationMessageDc(TimeSpan duration, string unhealthyMonitorsCommand)
		{
			return new LocalizedString("StoreMaintenanceAssistantEscalationMessageDc", Strings.ResourceManager, new object[]
			{
				duration,
				unhealthyMonitorsCommand
			});
		}

		public static LocalizedString UMCertificateNearExpiryEscalationMessage
		{
			get
			{
				return new LocalizedString("UMCertificateNearExpiryEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailureItemMessageForNTFSCorruption
		{
			get
			{
				return new LocalizedString("FailureItemMessageForNTFSCorruption", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseRPCLatencyMonitorGreenMessage
		{
			get
			{
				return new LocalizedString("DatabaseRPCLatencyMonitorGreenMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RelocationServicePermanentExceptionMessage
		{
			get
			{
				return new LocalizedString("RelocationServicePermanentExceptionMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EacSelfTestEscalationSubject(string serverName)
		{
			return new LocalizedString("EacSelfTestEscalationSubject", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString EacCtpTestEscalationSubject(string serverName)
		{
			return new LocalizedString("EacCtpTestEscalationSubject", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString LiveIdAuthenticationEscalationMesage
		{
			get
			{
				return new LocalizedString("LiveIdAuthenticationEscalationMesage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvokeNowProbeResultNotFound(string requestId, int workDefinitionId)
		{
			return new LocalizedString("InvokeNowProbeResultNotFound", Strings.ResourceManager, new object[]
			{
				requestId,
				workDefinitionId
			});
		}

		public static LocalizedString JournalArchiveEscalationMessage
		{
			get
			{
				return new LocalizedString("JournalArchiveEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BugCheckActionFailed(string errMsg)
		{
			return new LocalizedString("BugCheckActionFailed", Strings.ResourceManager, new object[]
			{
				errMsg
			});
		}

		public static LocalizedString Pop3ProtocolUnhealthy
		{
			get
			{
				return new LocalizedString("Pop3ProtocolUnhealthy", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SiteMailboxDocumentSyncEscalationMessage(int minFailPercent)
		{
			return new LocalizedString("SiteMailboxDocumentSyncEscalationMessage", Strings.ResourceManager, new object[]
			{
				minFailPercent
			});
		}

		public static LocalizedString SearchIndexBacklogWithProcessingRate(string databaseName, string backlog, string retryQueue, string completedCount, string processingRate, string minutes, string upTime, string serverStatus)
		{
			return new LocalizedString("SearchIndexBacklogWithProcessingRate", Strings.ResourceManager, new object[]
			{
				databaseName,
				backlog,
				retryQueue,
				completedCount,
				processingRate,
				minutes,
				upTime,
				serverStatus
			});
		}

		public static LocalizedString HxServiceEscalationMessageUnhealthy
		{
			get
			{
				return new LocalizedString("HxServiceEscalationMessageUnhealthy", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchSingleCopyEscalationMessage(string databaseName)
		{
			return new LocalizedString("SearchSingleCopyEscalationMessage", Strings.ResourceManager, new object[]
			{
				databaseName
			});
		}

		public static LocalizedString RequestForNewRidPoolFailedEscalationMessage
		{
			get
			{
				return new LocalizedString("RequestForNewRidPoolFailedEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchIndexSingleHealthyCopy(string databaseName, string details)
		{
			return new LocalizedString("SearchIndexSingleHealthyCopy", Strings.ResourceManager, new object[]
			{
				databaseName,
				details
			});
		}

		public static LocalizedString MRSRepeatedlyCrashingEscalationMessage(int count, TimeSpan duration)
		{
			return new LocalizedString("MRSRepeatedlyCrashingEscalationMessage", Strings.ResourceManager, new object[]
			{
				count,
				duration
			});
		}

		public static LocalizedString DatabaseSizeEscalationSubject
		{
			get
			{
				return new LocalizedString("DatabaseSizeEscalationSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseSizeEscalationMessageDc(string invokeNowCommand, string unhealthyMonitorsCommand)
		{
			return new LocalizedString("DatabaseSizeEscalationMessageDc", Strings.ResourceManager, new object[]
			{
				invokeNowCommand,
				unhealthyMonitorsCommand
			});
		}

		public static LocalizedString TooManyDatabaseMountedEscalationMessage(int threshold)
		{
			return new LocalizedString("TooManyDatabaseMountedEscalationMessage", Strings.ResourceManager, new object[]
			{
				threshold
			});
		}

		public static LocalizedString OabMailboxManifestEmpty
		{
			get
			{
				return new LocalizedString("OabMailboxManifestEmpty", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchCatalogNotHealthyEscalationMessage(string databaseName)
		{
			return new LocalizedString("SearchCatalogNotHealthyEscalationMessage", Strings.ResourceManager, new object[]
			{
				databaseName
			});
		}

		public static LocalizedString CheckFsmoRolesScriptExceptionMessage
		{
			get
			{
				return new LocalizedString("CheckFsmoRolesScriptExceptionMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PopImapSecondaryEndpoint
		{
			get
			{
				return new LocalizedString("PopImapSecondaryEndpoint", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotFunctionNormallyEscalationMessage
		{
			get
			{
				return new LocalizedString("CannotFunctionNormallyEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RcaTaskOutlineEntry(int milliseconds, LocalizedString resultType, LocalizedString taskName)
		{
			return new LocalizedString("RcaTaskOutlineEntry", Strings.ResourceManager, new object[]
			{
				milliseconds,
				resultType,
				taskName
			});
		}

		public static LocalizedString AssistantsNotRunningToCompletionError(string error)
		{
			return new LocalizedString("AssistantsNotRunningToCompletionError", Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString NetworkAdapterRssEscalationSubject(string serverName)
		{
			return new LocalizedString("NetworkAdapterRssEscalationSubject", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString EseDbTimeAdvanceEscalationSubject(string component, string machine, string database)
		{
			return new LocalizedString("EseDbTimeAdvanceEscalationSubject", Strings.ResourceManager, new object[]
			{
				component,
				machine,
				database
			});
		}

		public static LocalizedString SearchIndexActiveCopyNotIndxed(string databaseName, string state)
		{
			return new LocalizedString("SearchIndexActiveCopyNotIndxed", Strings.ResourceManager, new object[]
			{
				databaseName,
				state
			});
		}

		public static LocalizedString EscalationMessageHealthy
		{
			get
			{
				return new LocalizedString("EscalationMessageHealthy", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServiceNotRunningEscalationSubject(string serviceName)
		{
			return new LocalizedString("ServiceNotRunningEscalationSubject", Strings.ResourceManager, new object[]
			{
				serviceName
			});
		}

		public static LocalizedString SearchCatalogNotificationFeederLastEventZero(string databaseName, string serverName)
		{
			return new LocalizedString("SearchCatalogNotificationFeederLastEventZero", Strings.ResourceManager, new object[]
			{
				databaseName,
				serverName
			});
		}

		public static LocalizedString PublicFolderMoveJobStuckEscalationMessage
		{
			get
			{
				return new LocalizedString("PublicFolderMoveJobStuckEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchServiceNotRunningEscalationMessage
		{
			get
			{
				return new LocalizedString("SearchServiceNotRunningEscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchQueryStxTimeout(string query, string mailboxSmtpAddress, int seconds)
		{
			return new LocalizedString("SearchQueryStxTimeout", Strings.ResourceManager, new object[]
			{
				query,
				mailboxSmtpAddress,
				seconds
			});
		}

		public static LocalizedString DatabaseDiskReadLatencyEscalationMessageDc(TimeSpan duration, string unhealthyMonitorsCommand)
		{
			return new LocalizedString("DatabaseDiskReadLatencyEscalationMessageDc", Strings.ResourceManager, new object[]
			{
				duration,
				unhealthyMonitorsCommand
			});
		}

		public static LocalizedString SearchIndexCopyBacklogStatus(string copyName, string databaseStatus, string catalogStatus, string backlog, string retryQueueSize)
		{
			return new LocalizedString("SearchIndexCopyBacklogStatus", Strings.ResourceManager, new object[]
			{
				copyName,
				databaseStatus,
				catalogStatus,
				backlog,
				retryQueueSize
			});
		}

		public static LocalizedString EseLostFlushDetectedEscalationMessage(string machine, string database)
		{
			return new LocalizedString("EseLostFlushDetectedEscalationMessage", Strings.ResourceManager, new object[]
			{
				machine,
				database
			});
		}

		public static LocalizedString MobilityAccountPassword
		{
			get
			{
				return new LocalizedString("MobilityAccountPassword", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventLogProbeProviderName
		{
			get
			{
				return new LocalizedString("EventLogProbeProviderName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PswsEscalationSubject(string serverName)
		{
			return new LocalizedString("PswsEscalationSubject", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString VersionStore623EscalationMessage
		{
			get
			{
				return new LocalizedString("VersionStore623EscalationMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(325);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			DatabaseGuidNotSupplied = 2286371297U,
			OABGenTenantOutOfSLABody = 1189391965U,
			SearchFailToSaveMessage = 52276049U,
			ForwardSyncHaltEscalationSubject = 3960338262U,
			MaintenanceFailureEscalationMessage = 312538007U,
			DatabaseSpaceHelpString = 299573369U,
			PumServiceNotRunningEscalationMessage = 683259685U,
			HealthSetMaintenanceEscalationSubjectPrefix = 1378981546U,
			RegisterDnsHostRecordResponderName = 2358296594U,
			RcaDiscoveryOutlookAnywhereNotFound = 2482686375U,
			UnableToCompleteTopologyEscalationMessage = 3458357958U,
			LargestDeliveryQueueLengthEscalationMessage = 945891153U,
			DSNotifyQueueHigh15MinutesEscalationMessage = 1718676120U,
			OutStandingATQRequests15MinutesEscalationMessage = 2503653255U,
			ADDatabaseCorruption1017EscalationMessage = 2592211666U,
			DeviceDegradedEscalationMessage = 2321911502U,
			RemoteDomainControllerStateEscalationMessage = 1918560769U,
			OWACalendarAppPoolEscalationBody = 164779627U,
			MediaEstablishedFailedEscalationMessage = 1218627587U,
			RequestForFfoApprovalToOfflineFailed = 1980518279U,
			InsufficientInformationKCCEscalationMessage = 3706262484U,
			ClusterNodeEvictedEscalationMessage = 2150007296U,
			OwaOutsideInDatabaseAvailabilityFailuresSubject = 635314528U,
			RouteTableRecoveryResponderName = 112230113U,
			AssistantsActiveDatabaseSubject = 2257888472U,
			ForwardSyncMonopolizedEscalationSubject = 3750243459U,
			UMProtectedVoiceMessageEncryptDecryptFailedEscalationMessage = 805912300U,
			SearchIndexFailureEscalationMessage = 1299295308U,
			ForwardSyncCookieNotUpToDateEscalationMessage = 4294224569U,
			CannotBootEscalationMessage = 2092011713U,
			PassiveReplicationPerformanceCounterProbeEscalationMessage = 2288602703U,
			OwaTooManyStartPageFailuresSubject = 3399715728U,
			OwaOutsideInDatabaseAvailabilityFailuresBody = 4261321224U,
			SearchWordBreakerLoadingFailureEscalationMessage = 409381696U,
			Pop3CommandProcessingTimeEscalationMessage = 741999051U,
			DeltaSyncEndpointUnreachableEscalationMessage = 3741312692U,
			EventLogProbeRedEvents = 1119726172U,
			ProvisioningBigVolumeErrorProbeName = 109013642U,
			PassiveADReplicationMonitorEscalationMessage = 2897164042U,
			UMCertificateThumbprint = 373161856U,
			ForwardSyncMonopolizedEscalationMessage = 893346260U,
			NoResponseHeadersAvailable = 2704062871U,
			AdminAuditingAvailabilityFailureEscalationSubject = 380917770U,
			DeltaSyncPartnerAuthenticationFailedEscalationMessage = 1943611390U,
			SharedCacheEscalationSubject = 1893530126U,
			JournalingEscalationSubject = 3957270050U,
			HighProcessor15MinutesEscalationMessage = 2370864531U,
			NetworkAdapterRssEscalationMessage = 3308508053U,
			CPUOverThresholdErrorEscalationSubject = 1482699764U,
			Transport80thPercentileMissingSLAEscalationMessage = 1218523202U,
			InferenceTrainingSLAEscalationMessage = 2302596019U,
			EDiscoveryEscalationBodyEntText = 2517955710U,
			AsynchronousAuditSearchAvailabilityFailureEscalationSubject = 1451525307U,
			SearchRopNotSupportedEscalationMessage = 3795580654U,
			PushNotificationEnterpriseUnknownError = 909096326U,
			OwaClientAccessRoleNotInstalled = 610566341U,
			BridgeHeadReplicationEscalationMessage = 3645413289U,
			PushNotificationEnterpriseNotConfigured = 2836474835U,
			IncompatibleVectorEscalationMessage = 892806076U,
			DatabaseCorruptionEscalationMessage = 4138453910U,
			ReplicationOutdatedObjectsFailedEscalationMessage = 1752264565U,
			DatabaseCorruptEscalationMessage = 443970804U,
			HealthSetAlertSuppressionWarning = 48294403U,
			OwaIMInitializationFailedMessage = 1065939221U,
			ForwardSyncHaltEscalationMessage = 2172476143U,
			OfflineGLSEscalationMessage = 1679303927U,
			UnableToRunEscalateByDatabaseHealthResponder = 2914700647U,
			AggregateDeliveryQueueLengthEscalationMessage = 2514146180U,
			NoCafeMonitoringAccountsAvailable = 4070157535U,
			MediaEdgeResourceAllocationFailedEscalationMessage = 2950938112U,
			DRAPendingReplication5MinutesEscalationMessage = 1420198520U,
			SchemaPartitionFailedEscalationMessage = 357692756U,
			DatabaseSchemaVersionCheckEscalationSubject = 2064178893U,
			UMSipListeningPort = 878340914U,
			ELCMailboxSLAEscalationSubject = 3803135725U,
			DHCPNacksEscalationMessage = 1586522085U,
			ELCArchiveDumpsterEscalationMessage = 1507797344U,
			KDCServiceStatusTestMessage = 3628981090U,
			LowMemoryUnderThresholdErrorEscalationSubject = 401186895U,
			OwaIMInitializationFailedSubject = 1108409616U,
			PingConnectivityEscalationSubject = 3717637996U,
			PublicFolderConnectionCountEscalationMessage = 529762034U,
			FastNodeNotHealthyEscalationMessage = 663711086U,
			CheckDCMMDivergenceScriptExceptionMessage = 1307224852U,
			CrossPremiseMailflowEscalationMessage = 1220266742U,
			ForwardSyncStandardCompanyEscalationSubject = 2945963449U,
			JournalArchiveEscalationSubject = 1462332256U,
			DoMTConnectivityEscalateMessage = 3746115424U,
			InferenceComponentDisabledEscalationMessage = 680617032U,
			NoBackendMonitoringAccountsAvailable = 2776049552U,
			ActiveDirectoryConnectivityEscalationMessage = 349561476U,
			SyntheticReplicationTransactionEscalationMessage = 4196713157U,
			OabFileLoadExceptionEncounteredSubject = 814146069U,
			RegistryAccessDeniedEscalationMessage = 4268033680U,
			AuditLogSearchServiceletEscalationSubject = 2465131700U,
			EventLogProbeLogName = 649999061U,
			Imap4ProtocolUnhealthy = 536159405U,
			DLExpansionEscalationMessage = 1179543515U,
			ReplicationFailuresEscalationMessage = 2046186369U,
			SCTStateMonitoringScriptExceptionMessage = 1915337724U,
			ELCExceptionEscalationMessage = 1417399775U,
			OabTooManyHttpErrorResponsesEncounteredBody = 1844700225U,
			QuarantineEscalationMessage = 275645054U,
			TransportRejectingMessageSubmissions = 3642796412U,
			PublicFolderConnectionCountEscalationSubject = 3898201799U,
			PowerShellProfileEscalationSubject = 2285670747U,
			DivergenceBetweenCAAndAD1006EscalationMessage = 93821081U,
			UnreachableQueueLengthEscalationMessage = 726991111U,
			OabFileLoadExceptionEncounteredBody = 3639475733U,
			PublicFolderSyncEscalationSubject = 252047361U,
			Imap4CommandProcessingTimeEscalationMessage = 4139459438U,
			InvalidSearchResultsExceptionMessage = 3378952345U,
			SearchInformationNotAvailable = 4272242116U,
			ActiveDatabaseAvailabilityEscalationSubject = 2844472229U,
			ELCPermanentEscalationSubject = 952182041U,
			EventLogProbeGreenEvents = 1117652268U,
			ClusterHangEscalationMessage = 4155705872U,
			FEPServiceNotRunningEscalationMessage = 891001936U,
			RidMonitorEscalationMessage = 286684653U,
			SystemMailboxGuidNotFound = 2576630513U,
			MediaEdgeAuthenticationServiceCredentialsAcquisitionFailedEscalationMessage = 2727074534U,
			SearchTransportAgentFailureEscalationMessage = 3448175994U,
			TransportMessageCategorizationEscalationMessage = 2551706401U,
			InocrrectSCTStateExceptionMessage = 2595326046U,
			DataIssueEscalationMessage = 1789741911U,
			KerbAuthFailureEscalationMessagPAC = 3993818915U,
			DivergenceInDefinitionEscalationMessage = 423349970U,
			MobilityAccount = 2198917190U,
			OwaTooManyLogoffFailuresBody = 682587060U,
			ForwardSyncProcessRepeatedlyCrashingEscalationSubject = 770708540U,
			ADDatabaseCorruptionEscalationMessage = 1169357891U,
			MailboxAuditingAvailabilityFailureEscalationSubject = 580917981U,
			TopologyServiceConnectivityEscalationMessage = 1969709639U,
			UMSipTransport = 4160621849U,
			OabProtocolEscalationBody = 2022121183U,
			PushNotificationEnterpriseEmptyServiceUri = 3048659136U,
			PushNotificationEnterpriseAuthError = 3214050158U,
			UnableToRunAlertNotificationTypeByDatabaseCopyStateResponder = 2021347314U,
			ELCDumpsterWarningEscalationSubject = 1920206617U,
			OabMailboxEscalationBody = 2542492469U,
			CheckZombieDCEscalateMessage = 3640747412U,
			RidSetMonitorEscalationMessage = 2206456195U,
			PushNotificationCafeEndpointUnhealthy = 584392819U,
			ProvisioningBigVolumeErrorEscalationMessage = 632484951U,
			PublicFolderMailboxQuotaEscalationMessage = 58433467U,
			CASRoutingFailureEscalationSubject = 1077120006U,
			OAuthRequestFailureEscalationBody = 3549972347U,
			GLSEscalationMessage = 3456628766U,
			SCTNotFoundForAllVersionsExceptionMessage = 2809892364U,
			SqlOutputStreamInRetryEscalationMessage = 1613949426U,
			DefaultEscalationSubject = 3213813466U,
			MailboxAuditingAvailabilityFailureEscalationBody = 471095525U,
			BulkProvisioningNoProgressEscalationSubject = 1276311330U,
			InfrastructureValidationSubject = 1377854022U,
			SearchMemoryUsageOverThresholdEscalationMessage = 1194398797U,
			SharedCacheEscalationMessage = 105667215U,
			CannotRecoverEscalationMessage = 799661977U,
			AsynchronousAuditSearchAvailabilityFailureEscalationBody = 4214982447U,
			OwaIMLogAnalyzerMessage = 483050246U,
			UMCertificateSubjectName = 1005998838U,
			OwaNoMailboxesAvailable = 1108329015U,
			TransportCategorizerJobsUnavailableEscalationMessage = 2516924014U,
			SingleAvailableDatabaseCopyEscalationMessage = 910032993U,
			DivergenceInSiteNameEscalationMessage = 1345149923U,
			NullSearchResponseExceptionMessage = 3893081264U,
			UncategorizedProcess = 144342951U,
			MSExchangeProtectedServiceHostCrashingMessage = 3984209328U,
			UMDatacenterLoadBalancerSipOptionsPingEscalationMessage = 3400380005U,
			PassiveReplicationMonitorEscalationMessage = 564297537U,
			ReinstallServerEscalationMessage = 4039996833U,
			ForwardSyncCookieNotUpToDateEscalationSubject = 2217573206U,
			ActiveDirectoryConnectivityLocalEscalationMessage = 2260546867U,
			PassiveDatabaseAvailabilityEscalationSubject = 70227012U,
			OabTooManyHttpErrorResponsesEncounteredSubject = 3278262633U,
			ELCTransientEscalationSubject = 2089742391U,
			SiteFailureEscalationMessage = 1923047497U,
			OnlineMeetingCreateEscalationBody = 1012424075U,
			SiteMailboxDocumentSyncEscalationSubject = 462239732U,
			NTDSCorruptionEscalationMessage = 4075662140U,
			TopoDiscoveryFailedAllServersEscalationMessage = 3665176336U,
			VersionStore1479EscalationMessage = 400522478U,
			AssistantsNotRunningToCompletionSubject = 868604876U,
			AdminAuditingAvailabilityFailureEscalationBody = 1817218534U,
			ForwardSyncLiteCompanyEscalationSubject = 3226571974U,
			MSExchangeInformationStoreCannotContactADEscalationMessage = 4083150976U,
			DHCPServerRequestsEscalationMessage = 465857804U,
			NoNTDSObjectEscalationMessage = 4092609939U,
			PublicFolderMoveJobStuckEscalationSubject = 88349250U,
			SCTMonitoringScriptExceptionMessage = 4169066611U,
			ProvisionedDCBelowMinimumEscalationMessage = 380251366U,
			KerbAuthFailureEscalationMessage = 3378026954U,
			RequestsQueuedOver500EscalationMessage = 1277128402U,
			PopImapGuid = 2497401637U,
			MaintenanceFailureEscalationSubject = 77538240U,
			TransportServerDownEscalationMessage = 1577351776U,
			PopImapEndpoint = 3174826533U,
			NtlmConnectivityEscalationMessage = 3229931132U,
			OabTooManyWebAppStartsSubject = 499742697U,
			CafeEscalationSubjectUnhealthy = 3922382486U,
			DnsHostRecordProbeName = 2676970837U,
			ELCArchiveDumpsterWarningEscalationSubject = 1037389911U,
			JournalFilterAgentEscalationMessage = 1536257732U,
			WacDiscoveryFailureSubject = 1098709463U,
			CASRoutingLatencyEscalationSubject = 887962606U,
			EDSJobPoisonedEscalationMessage = 1060163078U,
			JournalFilterAgentEscalationSubject = 938822773U,
			DnsHostRecordMonitorName = 3660582759U,
			VersionStore2008EscalationMessage = 3238760785U,
			DnsServiceMonitorName = 2695821343U,
			DatabaseAvailabilityHelpString = 3687026854U,
			PublicFolderMailboxQuotaEscalationSubject = 3465747486U,
			HealthSetEscalationSubjectPrefix = 1074585207U,
			DHCPServerDeclinesEscalationMessage = 4054742343U,
			TrustMonitorProbeEscalationMessage = 235731656U,
			InvalidIncludedAssistantType = 959637577U,
			ReplicationDisabledEscalationMessage = 861211960U,
			ProvisioningBigVolumeErrorEscalateResponderName = 2731022862U,
			CASRoutingLatencyEscalationBody = 3881757534U,
			SecurityAlertMalwareDetectedEscalationMessage = 1221520849U,
			SynchronousAuditSearchAvailabilityFailureEscalationBody = 1306675352U,
			MaintenanceTimeoutEscalationMessage = 4173100116U,
			DeltaSyncServiceEndpointsLoadFailedEscalationMessage = 217815433U,
			SchedulingLatencyEscalateResponderMessage = 269594253U,
			PowerShellProfileEscalationMessage = 4073533312U,
			OAuthRequestFailureEscalationSubject = 3913093087U,
			OabMailboxNoOrgMailbox = 1166689995U,
			PushNotificationEnterpriseEmptyDomain = 3054327783U,
			AssistantsOutOfSlaMessage = 3814043621U,
			EwsAutodEscalationSubjectUnhealthy = 3546783417U,
			HttpConnectivityEscalationSubject = 1293802800U,
			DatabaseObjectNotFoundException = 4157781316U,
			FSMODCNotProvisionedEscalationMessage = 2661017855U,
			AssistantsNotRunningToCompletionMessage = 3444300625U,
			DLExpansionEscalationSubject = 1281304358U,
			RcaTaskOutlineFailed = 2407301436U,
			CASRoutingFailureEscalationBody = 3706272566U,
			DnsServiceProbeName = 2189832741U,
			SynchronousAuditSearchAvailabilityFailureEscalationSubject = 2148760356U,
			CannotRebuildIndexEscalationMessage = 4134231892U,
			UMPipelineFullEscalationMessageString = 3667272656U,
			DatabaseNotAttachedReadOnly = 1880452434U,
			InfrastructureValidationMessage = 2815102519U,
			OwaTooManyHttpErrorResponsesEncounteredSubject = 977800858U,
			CPUOverThresholdWarningEscalationSubject = 3387090034U,
			SubscriptionSlaMissedEscalationMessage = 35045070U,
			ActiveDirectoryConnectivityConfigDCEscalationMessage = 1680117983U,
			OwaMailboxRoleNotInstalled = 3785224032U,
			PushNotificationEnterpriseNetworkingError = 421810782U,
			DatabaseAvailabilityTimeout = 1548802155U,
			DatabaseGuidNotFound = 843177689U,
			SearchIndexBacklogAggregatedEscalationMessage = 678296020U,
			PublicFolderLocalEWSLogonEscalationMessage = 2793575470U,
			TenantRelocationErrorsFoundExceptionMessage = 3926584359U,
			UMCallRouterCertificateNearExpiryEscalationMessage = 3506328701U,
			SlowADWritesEscalationMessage = 136972698U,
			RcaEscalationBodyEnt = 2850892442U,
			MailboxDatabasesUnavailable = 2395533582U,
			RetryRemoteDeliveryQueueLengthEscalationMessage = 515794671U,
			FailedToUpgradeIndexEscalationMessage = 3544714230U,
			EventAssistantsWatermarksHelpString = 18446274U,
			InferenceClassifcationSLAEscalationMessage = 1165995200U,
			MRSUnhealthyMessage = 4059291937U,
			UMServiceType = 3867394701U,
			DivergenceBetweenCAAndAD1003EscalationMessage = 365492232U,
			AssistantsActiveDatabaseMessage = 182386565U,
			SchedulingLatencyEscalateResponderSubject = 1990015256U,
			OfficeGraphTransportDeliveryAgentFailureEscalationMessage = 3218355606U,
			OfficeGraphMessageTracingPluginFailureEscalationMessage = 1104137584U,
			LogicalDiskFreeMegabytesEscalationMessage = 424604115U,
			OwaIMLogAnalyzerSubject = 1920697843U,
			RaidDegradedEscalationMessage = 3515405004U,
			BulkProvisioningNoProgressEscalationMessage = 1236475651U,
			AssistantsOutOfSlaSubject = 1313509642U,
			OwaTooManyHttpErrorResponsesEncounteredBody = 1818757842U,
			CheckSumEscalationMessage = 2102184447U,
			PublicFolderLocalEWSLogonEscalationSubject = 3498323379U,
			UMServerAddress = 776931395U,
			CafeArrayNameCouldNotBeRetrieved = 3415559268U,
			EDSServiceNotRunningEscalationMessage = 3465381739U,
			SearchFailToCheckNodeState = 2618809902U,
			OwaTooManyStartPageFailuresBody = 2406788764U,
			QuarantineEscalationSubject = 1518847271U,
			HostControllerServiceRunningMessage = 1941315341U,
			SearchGracefulDegradationManagerFailureEscalationMessage = 3578257008U,
			ProvisioningBigVolumeErrorMonitorName = 46842886U,
			OwaTooManyWebAppStartsSubject = 3672282944U,
			SearchQueryStxSimpleQueryMode = 1495785996U,
			OABGenTenantOutOfSLASubject = 3956529557U,
			ELCDumpsterEscalationMessage = 951276022U,
			DirectoryConfigDiscrepancyEscalationMessage = 1559006762U,
			NetworkAdapterRecoveryResponderName = 1496058059U,
			SearchGracefulDegradationStatusEscalationMessage = 898751369U,
			DnsServiceRestartResponderName = 3135153224U,
			ELCMailboxSLAEscalationMessage = 3845606054U,
			JournalingEscalationMessage = 1456195959U,
			MaintenanceTimeoutEscalationSubject = 1598120667U,
			ContentsUnpredictableEscalationMessage = 3433261884U,
			EscalationSubjectUnhealthy = 3788119207U,
			AsyncAuditLogSearchEscalationSubject = 859758172U,
			DefaultEscalationMessage = 706708545U,
			SyntheticReplicationMonitorEscalationMessage = 2480657645U,
			EDiscoveryEscalationBodyDCHTML = 31173904U,
			OwaTooManyLogoffFailuresSubject = 1499273528U,
			CheckProvisionedDCExceptionMessage = 4220476005U,
			ProvisioningBigVolumeErrorEscalationSubject = 3913310860U,
			UMCertificateNearExpiryEscalationMessage = 3906074550U,
			FailureItemMessageForNTFSCorruption = 182285359U,
			DatabaseRPCLatencyMonitorGreenMessage = 821565004U,
			RelocationServicePermanentExceptionMessage = 1705255289U,
			LiveIdAuthenticationEscalationMesage = 3730111670U,
			JournalArchiveEscalationMessage = 3256369209U,
			Pop3ProtocolUnhealthy = 1746979174U,
			HxServiceEscalationMessageUnhealthy = 408002753U,
			RequestForNewRidPoolFailedEscalationMessage = 2258779484U,
			DatabaseSizeEscalationSubject = 2903867061U,
			OabMailboxManifestEmpty = 4196623762U,
			CheckFsmoRolesScriptExceptionMessage = 1260554753U,
			PopImapSecondaryEndpoint = 1084277863U,
			CannotFunctionNormallyEscalationMessage = 3639485535U,
			EscalationMessageHealthy = 116423715U,
			PublicFolderMoveJobStuckEscalationMessage = 2944547573U,
			SearchServiceNotRunningEscalationMessage = 667399815U,
			MobilityAccountPassword = 1455075163U,
			EventLogProbeProviderName = 3746052928U,
			VersionStore623EscalationMessage = 1122579724U
		}

		private enum ParamIDs
		{
			QuarantinedMailboxEscalationMessageEnt,
			PopSelfTestEscalationBodyDC,
			OneCopyMonitorFailureEscalationSubject,
			CircularLoggingDisabledEscalationMessage,
			MailSubmissionBehindWatermarksEscalationMessageEnt,
			InsufficientRedundancyEscalationSubject,
			LagCopyHealthProblemEscalationSubject,
			ActiveDatabaseAvailabilityEscalationMessageEnt,
			StoreAdminRPCInterfaceEscalationEscalationMessageEnt,
			InfrastructureValidationError,
			AvailabilityServiceEscalationHtmlBody,
			RwsDatamartConnectionEscalationBody,
			ServiceNotRunningEscalationMessage,
			DatabaseLocationNotFoundException,
			OwaCustomerTouchPointEscalationHtmlBody,
			EwsAutodSelfTestEscalationRecoveryDetails,
			MRSServiceNotRunningSubject,
			SearchQueryStxSuccess,
			QuarantinedMailboxEscalationMessageDc,
			ExchangeCrashExceededErrorThresholdMessage,
			DatabaseRepeatedMountsEscalationMessage,
			OnlineMeetingCreateEscalationSubject,
			LocalMachineDriveEncryptionLockEscalationMessage,
			DbFailureItemIoHardEscalationSubject,
			LogVolumeSpaceEscalationMessage,
			DatabaseSchemaVersionCheckEscalationMessageDc,
			ProcessCrashing,
			CafeEscalationRecoveryDetails,
			StoreProcessRepeatedlyCrashingEscalationMessageEnt,
			PrivateWorkingSetExceededErrorThresholdMessage,
			ProcessorTimeExceededWarningThresholdMessage,
			DatabaseAvailabilityFailure,
			EDiscoveryscalationSubject,
			ParseDiagnosticsStringError,
			SystemDriveSpaceEscalationSubject,
			DatabaseConsistencyEscalationMessage,
			GenericOverallXFailureEscalationMessage,
			ImapProxyTestEscalationBodyDC,
			PopSelfTestEscalationBodyENT,
			TransportSyncManagerServiceNotRunningEscalationMessage,
			EscalationMessageUnhealthy,
			EacDeepTestEscalationSubject,
			InvokeNowDefinitionFailure,
			ImapCustomerTouchPointEscalationBodyENT,
			PushNotificationChannelError,
			DBAvailableButUnloadedByTransportSyncManagerMessage,
			MonitoringAccountUnavailable,
			LocalDriveLogSpaceEscalationMessageDc,
			DatabaseRPCLatencyEscalationSubject,
			LocalDriveLogSpaceEscalationMessageEnt,
			SearchQueryFailure,
			EndpointManagerEndpointUninitialized,
			MigrationNotificationMessage,
			HostControllerServiceNodeExcessivePrivateBytes,
			HostControllerServiceNodeExcessivePrivateBytesDetails,
			SearchIndexServerCopyStatus,
			SearchActiveCopyUnhealthyEscalationMessage,
			ProcessRepeatedlyCrashingEscalationSubject,
			UMCallRouterRecentCallRejectedMessageString,
			OwaCustomerTouchPointEscalationBody,
			SearchIndexCrawlingNoProgress,
			ActiveSyncDeepTestEscalationBodyDC,
			ComponentHealthPercentFailureEscalationMessageUnhealthy,
			RwsDatamartAvailabilityEscalationSubject,
			CafeServerNotOwner,
			CafeOfflineFailedEscalationRecoveryDetails,
			SearchIndexCopyUnhealthy,
			SearchInstantSearchStxZeroHitMonitoringMailbox,
			ImapSelfTestEscalationBodyENT,
			StalledCopyEscalationSubject,
			InsufficientRedundancyEscalationMessage,
			DatabaseLogicalPhysicalSizeRatioEscalationMessageDc,
			LocalMachineDriveBootVolumeEncryptionStateEscalationMessage,
			LocalMachineDriveNotProtectedWithDraEscalationMessage,
			ReplServiceCrashEscalationMessage,
			PassiveDatabaseAvailabilityEscalationMessageEnt,
			CircularLoggingDisabledEscalationSubject,
			AttributeMissingFromProbeDefinition,
			UnableToGetDatabaseState,
			ProcessorTimeExceededErrorThresholdWithAffinitizationMessage,
			DatabaseCopyBehindEscalationSubject,
			SearchIndexCopyStatusError,
			DatabaseDiskReadLatencyEscalationMessageEnt,
			UnableToGetDatabaseSize,
			LocalMachineDriveNotProtectedWithDraEscalationSubject,
			StoreProcessRepeatedlyCrashingEscalationMessageDc,
			SearchCatalogNotLoaded,
			ActiveSyncDeepTestEscalationBodyENT,
			DatabasePercentRPCRequestsEscalationMessageEnt,
			DatabaseRPCLatencyEscalationMessageEnt,
			ForwardSyncStandardCompanyEscalationMessage,
			PutDCIntoMMFailureEscalateMessage,
			HostControllerServiceNodeOperationFailed,
			ObserverHeartbeatEscalateResponderSubject,
			AntimalwareEngineErrorsEscalationMessage,
			AuditLogSearchServiceletEscalationMessage,
			OwaMailboxDatabaseDoesntExist,
			OwaTooManyWebAppStartsBody,
			EscalationMessageFailuresUnhealthy,
			SearchQueryStxZeroHitMonitoringMailbox,
			ClusterServiceCrashEscalationMessage,
			OWACalendarAppPoolEscalationSubject,
			UMSipOptionsToUMServiceFailedEscalationSubject,
			PutDCIntoMMSuccessNotificationMessage,
			EseDbDivergenceDetectedEscalationMessage,
			ClusterServiceCrashEscalationSubject,
			LocalDriveLogSpaceEscalationSubject,
			ImapDeepTestEscalationBodyDC,
			ObserverHeartbeatEscalateResponderMessage,
			ReplServiceDownEscalationMessage,
			RemoteStoreAdminRPCInterfaceEscalationEscalationMessageEnt,
			SearchNumberOfParserServersDegradation,
			UnMonitoredDatabaseEscalationSubject,
			NumberOfActiveBackgroundTasksEscalationMessageEnt,
			OwaIMSigninFailedMessage,
			HostControllerNodeRestartDetails,
			AssistantsOutOfSlaError,
			SearchIndexStall,
			UMTranscriptionThrottledEscalationMessage,
			SiteFailureEscalationSubject,
			PopProxyTestEscalationBodyENT,
			PushNotificationDatacenterBackendEndpointUnhealthy,
			ComponentHealthErrorHeader,
			ImapEscalationSubject,
			PushNotificationPublisherUnhealthy,
			EacSelfTestEscalationBody,
			MonitoringAccountImproper,
			SearchQueryFailureEscalationMessage,
			PushNotificationCafeUnexpectedResponse,
			ForwardSyncLiteCompanyEscalationMessage,
			LastDBDiscoveryTimeFailedMessage,
			StoreAdminRPCInterfaceEscalationSubject,
			ProcessRepeatedlyCrashingEscalationMessage,
			UMServiceRecentCallRejectedEscalationMessageString,
			InvokeNowAssemblyInfoFailure,
			UnableToGetDatabaseSchemaVersion,
			InferenceDisabledComponentDetails,
			LocalMachineDriveEncryptionSuspendEscalationSubject,
			QuarantinedMailboxEscalationSubject,
			SearchLocalCopyStatusEscalationMessage,
			ProcessCrashDetectionEscalationMessage,
			LogVolumeSpaceEscalationSubject,
			ClusterGroupDownEscalationSubject,
			LocalMachineDriveBootVolumeEncryptionStateEscalationSubject,
			DatabaseCopySlowReplayEscalationSubject,
			SearchIndexBacklogWithHistory,
			EacCtpTestEscalationBody,
			OabMailboxEscalationSubject,
			ServerInMaintenanceModeForTooLongEscalationSubject,
			ServiceNotRunningEscalationMessageEnt,
			DatabaseCopySlowReplayEscalationMessage,
			UMGrammarUsageEscalationMessage,
			FireWallEscalationMessage,
			HAPassiveCopyUnhealthy,
			UMSipOptionsToUMServiceFailedEscalationBody,
			MailSubmissionBehindWatermarksEscalationSubject,
			ComponentHealthErrorContent,
			PrivateWorkingSetExceededWarningThresholdMessage,
			ActiveSyncSelfTestEscalationBodyDC,
			NTFSCorruptionEscalationMessage,
			FailedAndSuspendedCopyEscalationMessage,
			OneCopyMonitorFailureMessage,
			EventAssistantsProcessRepeatedlyCrashingEscalationMessageEnt,
			PutMultipleDCIntoMMFailureEscalateMessage,
			SystemDriveSpaceEscalationMessage,
			DatabasePercentRPCRequestsEscalationSubject,
			SearchQuerySlow,
			SearchIndexSuspendedEscalationMessage,
			InvokeNowInvalidWorkDefinition,
			OabTooManyWebAppStartsBody,
			HostControllerServiceNodeUnhealthy,
			HostControllerExcessiveNodeRestarts,
			OwaDeepTestEscalationHtmlBody,
			SuspendedCopyEscalationMessage,
			ImapProxyTestEscalationBodyENT,
			SearchResourceLoadEscalationMessage,
			InvalidAccessToken,
			LocalMachineDriveProtectedWithDraWithoutDecryptorEscalationMessage,
			MigrationNotificationSubject,
			PublicFolderSyncEscalationMessage,
			RcaWorkItemCreationSummaryEntry,
			WacDiscoveryFailureBody,
			HighLogGenerationRateEscalationSubject,
			DatabaseLogicalPhysicalSizeRatioEscalationMessageEnt,
			OwaSelfTestEscalationBody,
			MailboxAssistantsBehindWatermarksEscalationSubject,
			ControllerFailureMessage,
			HighLogGenerationRateEscalationMessage,
			SearchFeedingControllerFailureEscalationMessage,
			ClusterNetworkDownEscalationSubject,
			FireWallEscalationSubject,
			ServerVersionNotFound,
			EacDeepTestEscalationBody,
			VersionBucketsAllocatedEscalationEscalationMessageDc,
			SearchIndexCrawlingWithHealthyCopy,
			ProcessorTimeExceededErrorThresholdSubject,
			MailboxAssistantsBehindWatermarksEscalationMessageEnt,
			PassiveDatabaseAvailabilityEscalationMessageDc,
			SuspendedCopyEscalationSubject,
			SearchCatalogSuspended,
			UMRecentPartnerTranscriptionFailedEscalationMessageString,
			EseDbTimeAdvanceEscalationMessage,
			ControllerFailureEscalationSubject,
			SearchWordBreakerLoadingFailure,
			SearchIndexMultiCrawling,
			AvailabilityServiceEscalationBody,
			OwaSelfTestEscalationHtmlBody,
			ProcessorTimeExceededWarningThresholdSubject,
			SearchIndexSingleHealthyCopyWithSeeding,
			DatabasePercentRPCRequestsEscalationMessageDc,
			DatabaseCopyBehindEscalationMessage,
			EseInconsistentDataDetectedEscalationSubject,
			DatabaseValidationNullRef,
			CafeThreadCountMessageUnhealthy,
			PotentialInsufficientRedundancyEscalationSubject,
			ClusterNetworkReportErrorEscalationMessage,
			DbFailureItemIoHardEscalationMessage,
			MonitoringAccountDomainUnavailable,
			ClusterNodeEvictedEscalationSubject,
			LocalMachineDriveEncryptionSuspendEscalationMessage,
			ActiveManagerUnhealthyEscalationSubject,
			SearchIndexSeedingNoProgres,
			LagCopyHealthProblemEscalationMessage,
			FailedAndSuspendedCopyEscalationSubject,
			RemoteStoreAdminRPCInterfaceEscalationSubject,
			InvokeNowPickupEventNotFound,
			ActiveSyncSelfTestEscalationBodyENT,
			PrivateWorkingSetExceededErrorThresholdSubject,
			AvailabilityServiceEscalationSubjectUnhealthy,
			ActiveSyncEscalationSubject,
			StoreMaintenanceAssistantEscalationMessageEnt,
			SearchTransportAgentFailure,
			AssistantsNotRunningError,
			ImapCustomerTouchPointEscalationBodyDC,
			RpsFailedEscalationMessage,
			ForwardSyncProcessRepeatedlyCrashingEscalationMessage,
			StoreAdminRPCInterfaceNotResponding,
			OwaCustomerTouchPointEscalationSubject,
			OwaIMSigninFailedSubject,
			UMPipelineSLAEscalationMessageString,
			ComponentHealthHeartbeatEscalationMessageUnhealthy,
			SearchResourceLoadUnhealthy,
			UnmonitoredDatabaseEscalationMessage,
			LongRunningWerMgrTriggerWarningThresholdSubject,
			EseDbTimeSmallerEscalationMessage,
			SearchProcessCrashingTooManyTimesEscalationMessage,
			GetDiagnosticInfoTimeoutMessage,
			WatermarksBehind,
			ClusterGroupDownEscalationMessage,
			PswsEscalationBody,
			EseDbTimeSmallerEscalationSubject,
			OabMailboxFileNotFound,
			SearchCatalogInFailedAndSuspendedState,
			EseSinglePageLogicalCorruptionDetectedSubject,
			StoreAdminRPCInterfaceEscalationEscalationMessageDc,
			VersionBucketsAllocatedEscalationSubject,
			OwaDeepTestEscalationBody,
			MRSLongQueueScanMessage,
			SearchIndexFailure,
			MRSRPCPingSubject,
			UserThrottlingLockedOutUsersSubject,
			ClusterHangEscalationSubject,
			RcaEscalationSubject,
			OwaDeepTestEscalationSubject,
			ClusterServiceDownEscalationMessage,
			EscalationMessagePercentUnhealthy,
			SearchIndexCopyStatus,
			JobobjectCpuExceededThresholdSubject,
			FindPlacesRequestsError,
			RemoteStoreAdminRPCInterfaceEscalationEscalationMessageDc,
			EseInconsistentDataDetectedEscalationMessage,
			DatabaseNotFoundInADException,
			PrivateWorkingSetExceededWarningThresholdSubject,
			MRSRepeatedlyCrashingEscalationSubject,
			ProcessorTimeExceededErrorThresholdMessage,
			MRSLongQueueScanSubject,
			StalledCopyEscalationMessage,
			OabProtocolEscalationSubject,
			ProcessorTimeExceededWarningThresholdWithAffinitizationMessage,
			InvalidSystemMailbox,
			CafeEscalationMessageUnhealthyForDC,
			UMSipOptionsToUMCallRouterServiceFailedEscalationSubject,
			PutMultipleDCIntoMMSuccessNotificationMessage,
			InferenceTrainingDataCollectionRepeatedCrashEscalationMessage,
			SearchInstantSearchStxException,
			ActiveSyncCustomerTouchPointEscalationBodyDC,
			HealthSetsStates,
			LongRunningWerMgrTriggerWarningThresholdMessage,
			ReplServiceDownEscalationSubject,
			SearchIndexBacklogWithProcessingRateAndHistory,
			MultipleRecipientsFound,
			CafeThreadCountSubjectUnhealthy,
			PotentialInsufficientRedundancyEscalationMessage,
			InvalidUserName,
			SearchInstantSearchStxEscalationMessage,
			MailboxAssistantsBehindWatermarksEscalationMessageDc,
			ExchangeCrashExceededErrorThresholdSubject,
			RcaWorkItemDescriptionEntry,
			SearchIndexActiveCopyUnhealthy,
			UMCallRouterRecentMissedCallNotificationProxyFailedEscalationMessageString,
			SearchNumDiskPartsEscalationMessage,
			InferenceClassificationRepeatedCrashEscalationMessage,
			AssistantsActiveDatabaseError,
			DatabaseDiskReadLatencyEscalationSubject,
			NumberOfActiveBackgroundTasksEscalationMessageDc,
			UserthtottlingLockedOutUsersMessage,
			SearchCatalogHasError,
			NTFSCorruptionEscalationSubject,
			ActiveManagerUnhealthyEscalationMessage,
			HighDiskLatencyEscalationSubject,
			TooManyDatabaseMountedEscalationSubject,
			PasswordVerificationFailed,
			LocalMachineDriveEncryptionStateEscalationMessage,
			UMWorkerProcessRecentCallRejectedEscalationMessageString,
			SingleAvailableDatabaseCopyEscalationSubject,
			PopProxyTestEscalationBodyDC,
			StoreNotificationEscalationMessage,
			CafeEscalationMessageUnhealthy,
			PopCustomerTouchPointEscalationBodyDC,
			PopEscalationSubject,
			InferenceComponentDisabled,
			SearchQueryStxEscalationMessage,
			DatabaseSizeEscalationMessageEnt,
			ActiveSyncCustomerTouchPointEscalationBodyENT,
			HealthSetMonitorsStates,
			SearchGetDiagnosticInfoTimeout,
			InvalidMailboxDatabaseEndpoint,
			PopDeepTestEscalationBodyDC,
			LocalMachineDriveEncryptionLockEscalationSubject,
			SearchGracefulDegradationStatus,
			EseDbDivergenceDetectedSubject,
			SearchIndexActiveCopySeedingWithHealthyCopy,
			NumberOfActiveBackgroundTasksEscalationSubject,
			StoreMaintenanceAssistantEscalationSubject,
			StoreNotificationEscalationSubject,
			RwsDatamartAvailabilityEscalationBody,
			ComponentHealthPercentFailureEscalationMessageHealthy,
			ActiveDatabaseAvailabilityEscalationMessageDc,
			SearchIndexDatabaseCopyStatus,
			OabMailboxManifestAddressListEmpty,
			PopCustomerTouchPointEscalationBodyENT,
			ServerInMaintenanceModeForTooLongEscalationMessage,
			LocalMachineDriveEncryptionStateEscalationSubject,
			EseLostFlushDetectedEscalationSubject,
			ImapSelfTestEscalationBodyDC,
			ArchiveNamePrefix,
			ReplServiceCrashEscalationSubject,
			EventAssistantsProcessRepeatedlyCrashingEscalationMessageDc,
			ComponentHealthHeartbeatEscalationMessageHealthy,
			OwaSelfTestEscalationSubject,
			EseSinglePageLogicalCorruptionDetectedEscalationMessage,
			JobobjectCpuExceededThresholdMessage,
			HighDiskLatencyEscalationMessage,
			ImapDeepTestEscalationBodyENT,
			OfficeGraphMessageTracingPluginLogDirectoryExceedsSizeLimit,
			MailSubmissionBehindWatermarksEscalationMessageDc,
			AsyncAuditLogSearchEscalationMessage,
			DatabaseLogicalPhysicalSizeRatioEscalationSubject,
			ServiceNotRunningEscalationMessageDc,
			DatabaseRPCLatencyEscalationMessageDc,
			ClusterServiceDownEscalationSubject,
			UnableToGetStoreUsageStatisticsData,
			BingServicesLatencyError,
			UMSipOptionsToUMCallRouterServiceFailedEscalationBody,
			CouldNotAddExchangeSnapInExceptionMessage,
			ClassficationEngineErrorsEscalationMessage,
			PopDeepTestEscalationBodyENT,
			DatabaseRepeatedMountsEscalationSubject,
			RwsDatamartConnectionEscalationSubject,
			SearchIndexBacklog,
			TransportSyncOutOfSLA,
			EwsAutodEscalationMessageUnhealthy,
			SecurityAlertEscalationMessage,
			VersionBucketsAllocatedEscalationEscalationMessageEnt,
			PushNotificationSendPublishNotificationError,
			SearchCatalogTooBig,
			SearchMemoryUsageOverThreshold,
			LocalMachineDriveProtectedWithDraWithoutDecryptorEscalationSubject,
			StoreMaintenanceAssistantEscalationMessageDc,
			EacSelfTestEscalationSubject,
			EacCtpTestEscalationSubject,
			InvokeNowProbeResultNotFound,
			BugCheckActionFailed,
			SiteMailboxDocumentSyncEscalationMessage,
			SearchIndexBacklogWithProcessingRate,
			SearchSingleCopyEscalationMessage,
			SearchIndexSingleHealthyCopy,
			MRSRepeatedlyCrashingEscalationMessage,
			DatabaseSizeEscalationMessageDc,
			TooManyDatabaseMountedEscalationMessage,
			SearchCatalogNotHealthyEscalationMessage,
			RcaTaskOutlineEntry,
			AssistantsNotRunningToCompletionError,
			NetworkAdapterRssEscalationSubject,
			EseDbTimeAdvanceEscalationSubject,
			SearchIndexActiveCopyNotIndxed,
			ServiceNotRunningEscalationSubject,
			SearchCatalogNotificationFeederLastEventZero,
			SearchQueryStxTimeout,
			DatabaseDiskReadLatencyEscalationMessageDc,
			SearchIndexCopyBacklogStatus,
			EseLostFlushDetectedEscalationMessage,
			PswsEscalationSubject
		}
	}
}
