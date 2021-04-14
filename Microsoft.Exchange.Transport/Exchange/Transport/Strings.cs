using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Transport
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(3699713858U, "RoutingNoAdSites");
			Strings.stringIDs.Add(101432279U, "LatencyComponentHeartbeat");
			Strings.stringIDs.Add(677442004U, "ComponentsDisabledNone");
			Strings.stringIDs.Add(680722199U, "LatencyComponentRmsRequestDelegationToken");
			Strings.stringIDs.Add(2008205339U, "LatencyComponentRmsAcquireServerBoxRac");
			Strings.stringIDs.Add(393272335U, "LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionStoreStats");
			Strings.stringIDs.Add(642728361U, "DatabaseOpen");
			Strings.stringIDs.Add(2221898189U, "ColumnName");
			Strings.stringIDs.Add(28104421U, "ShadowSendConnector");
			Strings.stringIDs.Add(781896450U, "LatencyComponentDeliveryQueueMailboxInsufficientResources");
			Strings.stringIDs.Add(897286993U, "Restricted");
			Strings.stringIDs.Add(3327987659U, "LatencyComponentStoreDriverDeliveryContentConversion");
			Strings.stringIDs.Add(3070454738U, "IntraorgSendConnectorName");
			Strings.stringIDs.Add(2741991560U, "LatencyComponentSmtpReceiveCommitLocal");
			Strings.stringIDs.Add(2269890262U, "LatencyComponentDeliveryQueueMailbox");
			Strings.stringIDs.Add(366323840U, "LatencyComponentDeliveryAgent");
			Strings.stringIDs.Add(2455354172U, "LatencyComponentRmsFindServiceLocation");
			Strings.stringIDs.Add(229786213U, "NormalPriority");
			Strings.stringIDs.Add(2359831369U, "LatencyComponentSmtpReceiveOnEndOfHeaders");
			Strings.stringIDs.Add(3475865632U, "Confidential");
			Strings.stringIDs.Add(3022513318U, "NormalRisk");
			Strings.stringIDs.Add(2222703033U, "ShadowRedundancyNoActiveServerInNexthopSolution");
			Strings.stringIDs.Add(1699301985U, "LatencyComponentStoreDriverOnCompletedMessage");
			Strings.stringIDs.Add(130958697U, "HighRisk");
			Strings.stringIDs.Add(1048481989U, "DatabaseRecoveryActionDelete");
			Strings.stringIDs.Add(1785499833U, "LatencyComponentSmtpReceiveOnProxyInboundMessage");
			Strings.stringIDs.Add(3547958413U, "LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionSmtpOut");
			Strings.stringIDs.Add(4212340469U, "LatencyComponentSmtpSend");
			Strings.stringIDs.Add(3759339643U, "MediumResourceUses");
			Strings.stringIDs.Add(3152927583U, "LatencyComponentSmtpReceiveCommitRemote");
			Strings.stringIDs.Add(4145912840U, "LatencyComponentCategorizer");
			Strings.stringIDs.Add(1414246128U, "None");
			Strings.stringIDs.Add(2123701631U, "LatencyComponentDumpster");
			Strings.stringIDs.Add(2393050832U, "EnumeratorBadPosition");
			Strings.stringIDs.Add(1226316175U, "LatencyComponentRmsAcquireCertificationMexData");
			Strings.stringIDs.Add(2892365298U, "ContentAggregationComponent");
			Strings.stringIDs.Add(1244889643U, "DiscardingDataFalse");
			Strings.stringIDs.Add(1050903407U, "DatabaseClosed");
			Strings.stringIDs.Add(2048777343U, "FailedToReadServerRole");
			Strings.stringIDs.Add(821664177U, "LatencyComponentDeliveryQueueMailboxDynamicMailboxDatabaseThrottlingLimitExceeded");
			Strings.stringIDs.Add(2000842882U, "LatencyComponentMailSubmissionServiceNotify");
			Strings.stringIDs.Add(217112309U, "LatencyComponentStoreDriverSubmissionStore");
			Strings.stringIDs.Add(3442182595U, "SeekFailed");
			Strings.stringIDs.Add(1243242609U, "LatencyComponentMailSubmissionServiceNotifyRetrySchedule");
			Strings.stringIDs.Add(1618072168U, "LatencyComponentStoreDriverOnDemotedMessage");
			Strings.stringIDs.Add(683438393U, "Public");
			Strings.stringIDs.Add(1815474557U, "AcceptedDomainTableNotLoaded");
			Strings.stringIDs.Add(1747692937U, "LatencyComponentServiceRestart");
			Strings.stringIDs.Add(525138956U, "NormalRiskNonePriority");
			Strings.stringIDs.Add(4093059930U, "LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionContentConversion");
			Strings.stringIDs.Add(526223919U, "LatencyComponentDeliveryQueueMailboxMapiExceptionTimeout");
			Strings.stringIDs.Add(177467553U, "LatencyComponentSmtpSendMailboxDelivery");
			Strings.stringIDs.Add(680901367U, "LatencyComponentDeliveryQueueExternal");
			Strings.stringIDs.Add(464945964U, "HighAndBulkRisk");
			Strings.stringIDs.Add(850634908U, "JetOperationFailure");
			Strings.stringIDs.Add(4222452556U, "LatencyComponentSmtpReceive");
			Strings.stringIDs.Add(2713736967U, "LatencyComponentSmtpReceiveDataExternal");
			Strings.stringIDs.Add(2517616835U, "LatencyComponentStoreDriverDeliveryAD");
			Strings.stringIDs.Add(3066388364U, "LatencyComponentDeliveryQueueMailboxDeliverAgentTransientFailure");
			Strings.stringIDs.Add(2895510700U, "LatencyComponentCategorizerFinal");
			Strings.stringIDs.Add(412845223U, "BulkRisk");
			Strings.stringIDs.Add(2314636918U, "LatencyComponentSubmissionQueue");
			Strings.stringIDs.Add(871184586U, "LatencyComponentStoreDriverSubmit");
			Strings.stringIDs.Add(1421458560U, "DumpsterJobStatusQueued");
			Strings.stringIDs.Add(320885804U, "LatencyComponentStoreDriverDeliveryMailboxDatabaseThrottling");
			Strings.stringIDs.Add(939699173U, "LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionOnDemotedMessage");
			Strings.stringIDs.Add(3432225769U, "SecureMailInvalidNumberOfLayers");
			Strings.stringIDs.Add(3242396317U, "LowRiskLowPriority");
			Strings.stringIDs.Add(1951232521U, "LatencyComponentMailSubmissionServiceFailedAttempt");
			Strings.stringIDs.Add(4208962580U, "RemoteDomainTableNotLoaded");
			Strings.stringIDs.Add(1681384042U, "OutboundMailDeliveryToRemoteDomainsComponent");
			Strings.stringIDs.Add(4095319950U, "AttachmentReadFailed");
			Strings.stringIDs.Add(2378292844U, "LatencyComponentMailboxRules");
			Strings.stringIDs.Add(3395591492U, "LowResourceUses");
			Strings.stringIDs.Add(1151336747U, "LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionStoreDisposeSession");
			Strings.stringIDs.Add(4217035038U, "High");
			Strings.stringIDs.Add(198598092U, "IdentityParameterNotFound");
			Strings.stringIDs.Add(1049847567U, "NotOpenForWrite");
			Strings.stringIDs.Add(4063125577U, "TcpListenerError");
			Strings.stringIDs.Add(429833158U, "LatencyComponentMexRuntimeThreadpoolQueue");
			Strings.stringIDs.Add(1306604228U, "LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionPerfContextLdap");
			Strings.stringIDs.Add(1836041741U, "NormalResourceUses");
			Strings.stringIDs.Add(364271663U, "DumpsterJobStatusCreated");
			Strings.stringIDs.Add(3603045308U, "LowPriority");
			Strings.stringIDs.Add(2295242475U, "ExternalDestinationInboundProxySendConnector");
			Strings.stringIDs.Add(1092559024U, "IPFilterDatabaseInstanceName");
			Strings.stringIDs.Add(3889172535U, "ActivationFailed");
			Strings.stringIDs.Add(2198647971U, "LatencyComponentDelivery");
			Strings.stringIDs.Add(3545397251U, "AggregateResource");
			Strings.stringIDs.Add(1807713273U, "LatencyComponentProcessingSchedulerScoped");
			Strings.stringIDs.Add(1437992463U, "LatencyComponentSmtpReceiveOnRcpt2Command");
			Strings.stringIDs.Add(68652566U, "LatencyComponentStoreDriverOnPromotedMessage");
			Strings.stringIDs.Add(1004476481U, "PoisonMessageRegistryAccessFailed");
			Strings.stringIDs.Add(4020652394U, "HighResourceUses");
			Strings.stringIDs.Add(3706269143U, "EnvelopRecipientDisposed");
			Strings.stringIDs.Add(2556558528U, "SystemMemory");
			Strings.stringIDs.Add(184239278U, "ReadOrgContainerFailed");
			Strings.stringIDs.Add(488909308U, "LatencyComponentStoreDriverOnCreatedMessage");
			Strings.stringIDs.Add(3548219859U, "CategorizerMaxConfigLoadRetriesReached");
			Strings.stringIDs.Add(1542680578U, "LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionStoreOpenSession");
			Strings.stringIDs.Add(1797311582U, "RoutingLocalServerIsNotBridgehead");
			Strings.stringIDs.Add(1030204127U, "LatencyComponentCategorizerOnCategorizedMessage");
			Strings.stringIDs.Add(727067537U, "LatencyComponentRmsAcquireB2BRac");
			Strings.stringIDs.Add(2163339405U, "InternalDestinationInboundProxySendConnector");
			Strings.stringIDs.Add(1080294189U, "LatencyComponentDeliveryQueueMailboxMaxConcurrentMessageSizeLimitExceeded");
			Strings.stringIDs.Add(170486207U, "LatencyComponentStoreDriverDeliveryRpc");
			Strings.stringIDs.Add(1602038336U, "ConnectionInUse");
			Strings.stringIDs.Add(4260841639U, "LatencyComponentMailSubmissionService");
			Strings.stringIDs.Add(4278195556U, "NonePriority");
			Strings.stringIDs.Add(3768774525U, "LatencyComponentRmsAcquireTemplateInfo");
			Strings.stringIDs.Add(3451284522U, "LatencyComponentContentAggregation");
			Strings.stringIDs.Add(2443567820U, "LatencyComponentMailSubmissionServiceShadowResubmitDecision");
			Strings.stringIDs.Add(3636715311U, "LatencyComponentContentAggregationMailItemCommit");
			Strings.stringIDs.Add(1674978893U, "DatabaseRecoveryActionMove");
			Strings.stringIDs.Add(3640623549U, "ReadTransportServerConfigFailed");
			Strings.stringIDs.Add(4082435799U, "LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionHubSelector");
			Strings.stringIDs.Add(1054523613U, "MessageTrackingConfigNotFound");
			Strings.stringIDs.Add(16119773U, "LatencyComponentDeliveryQueueInternal");
			Strings.stringIDs.Add(2383564204U, "LatencyComponentDeliveryQueueMailboxMailboxServerOffline");
			Strings.stringIDs.Add(3307097748U, "PrivateBytesResource");
			Strings.stringIDs.Add(4156740374U, "LatencyComponentDeliveryQueueMailboxMailboxDatabaseOffline");
			Strings.stringIDs.Add(1447270652U, "RoutingMaxConfigLoadRetriesReached");
			Strings.stringIDs.Add(1851026156U, "LatencyComponentRmsAcquireServerLicensingMexData");
			Strings.stringIDs.Add(2614121165U, "LatencyComponentDeliveryQueueMailboxRecipientThreadLimitExceeded");
			Strings.stringIDs.Add(1010194958U, "SmtpReceiveParserNegativeBytes");
			Strings.stringIDs.Add(702550931U, "InvalidRoleChange");
			Strings.stringIDs.Add(3077939474U, "LatencyComponentPoisonQueue");
			Strings.stringIDs.Add(3953400951U, "BootScannerComponent");
			Strings.stringIDs.Add(1434651324U, "NotInTransaction");
			Strings.stringIDs.Add(3231014581U, "LatencyComponentSmtpReceiveOnRcptCommand");
			Strings.stringIDs.Add(2675789057U, "BodyReadFailed");
			Strings.stringIDs.Add(1355928173U, "TextConvertersFailed");
			Strings.stringIDs.Add(604013917U, "RoutingNoRoutingGroups");
			Strings.stringIDs.Add(1040763200U, "LatencyComponentStoreDriverDeliveryMessageConcurrency");
			Strings.stringIDs.Add(2064442254U, "Submission");
			Strings.stringIDs.Add(1817505332U, "ReadingADConfigFailed");
			Strings.stringIDs.Add(4179070144U, "LatencyComponentTooManyComponents");
			Strings.stringIDs.Add(531266226U, "ReadTransportConfigConfigFailed");
			Strings.stringIDs.Add(3659171826U, "InvalidMessageResubmissionState");
			Strings.stringIDs.Add(2339552741U, "LatencyComponentSmtpReceiveOnEndOfData");
			Strings.stringIDs.Add(551877430U, "InvalidTransportRole");
			Strings.stringIDs.Add(2370750848U, "LatencyComponentReplay");
			Strings.stringIDs.Add(3447320822U, "CloneMoveDestination");
			Strings.stringIDs.Add(3132079455U, "LowRiskNonePriority");
			Strings.stringIDs.Add(3933849364U, "LatencyComponentCategorizerBifurcation");
			Strings.stringIDs.Add(3849329560U, "SchemaInvalid");
			Strings.stringIDs.Add(2223967066U, "QuoteNestLevel");
			Strings.stringIDs.Add(1966949091U, "LatencyComponentUnknown");
			Strings.stringIDs.Add(1884563717U, "LatencyComponentTotal");
			Strings.stringIDs.Add(2229981216U, "LatencyComponentRmsAcquireLicense");
			Strings.stringIDs.Add(1359462263U, "InboundMailSubmissionFromReplayDirectoryComponent");
			Strings.stringIDs.Add(3952286128U, "NoColumns");
			Strings.stringIDs.Add(3021848955U, "LatencyComponentRmsAcquireClc");
			Strings.stringIDs.Add(1362050859U, "CloneMoveComplete");
			Strings.stringIDs.Add(1615937519U, "LatencyComponentQuarantineReleaseOrReport");
			Strings.stringIDs.Add(662654939U, "LatencyComponentSubmissionAssistant");
			Strings.stringIDs.Add(529499203U, "DumpsterJobResponseSuccess");
			Strings.stringIDs.Add(1767865225U, "AgentComponentFailed");
			Strings.stringIDs.Add(2584904976U, "ClientProxySendConnector");
			Strings.stringIDs.Add(4128944152U, "Basic");
			Strings.stringIDs.Add(2705384945U, "LatencyComponentCategorizerOnSubmittedMessage");
			Strings.stringIDs.Add(3565174541U, "NormalRiskNormalPriority");
			Strings.stringIDs.Add(1411027439U, "LatencyComponentSmtpReceiveCommit");
			Strings.stringIDs.Add(3882493397U, "RoutingNoLocalServer");
			Strings.stringIDs.Add(3622146271U, "SecureMailSecondLayerMustBeEnveloped");
			Strings.stringIDs.Add(3840724219U, "MailItemDeferred");
			Strings.stringIDs.Add(3706643741U, "InboundMailSubmissionFromMailboxComponent");
			Strings.stringIDs.Add(56765413U, "AttachmentProtectionFailed");
			Strings.stringIDs.Add(4115715614U, "LatencyComponentSubmissionAssistantThrottling");
			Strings.stringIDs.Add(1586175673U, "LatencyComponentCategorizerLocking");
			Strings.stringIDs.Add(955728834U, "ValueNull");
			Strings.stringIDs.Add(2197880345U, "TooManyAgents");
			Strings.stringIDs.Add(3324883850U, "DumpsterJobStatusCompleted");
			Strings.stringIDs.Add(2127171860U, "LatencyComponentStoreDriverOnDeliveredMessage");
			Strings.stringIDs.Add(1858725880U, "LatencyComponentRmsAcquireB2BLicense");
			Strings.stringIDs.Add(2602495526U, "LatencyComponentDeliveryQueueMailboxMapiExceptionLockViolation");
			Strings.stringIDs.Add(1946647341U, "Medium");
			Strings.stringIDs.Add(1261670220U, "NotBufferedStream");
			Strings.stringIDs.Add(2888586632U, "IncorrectBaseStream");
			Strings.stringIDs.Add(26231820U, "LatencyComponentProcess");
			Strings.stringIDs.Add(3133088287U, "CommitMailFailed");
			Strings.stringIDs.Add(1974402452U, "NonAsciiData");
			Strings.stringIDs.Add(2736623442U, "SeekBarred");
			Strings.stringIDs.Add(3819242299U, "RoutingLocalRgNotSet");
			Strings.stringIDs.Add(262955557U, "MessagingDatabaseInstanceName");
			Strings.stringIDs.Add(2000471546U, "LatencyComponentUnreachableQueue");
			Strings.stringIDs.Add(1113830170U, "LatencyComponentStoreDriverDelivery");
			Strings.stringIDs.Add(1062938671U, "DatabaseStillInUse");
			Strings.stringIDs.Add(680830688U, "LatencyComponentDeferral");
			Strings.stringIDs.Add(793374748U, "DumpsterJobResponseRetryLater");
			Strings.stringIDs.Add(1287594420U, "RoutingNoLocalAdSite");
			Strings.stringIDs.Add(3350471676U, "LatencyComponentRmsAcquireTemplates");
			Strings.stringIDs.Add(3717583665U, "InvalidRoutingOverrideEvent");
			Strings.stringIDs.Add(1384341089U, "GetSclThresholdDefaultValueOutOfRange");
			Strings.stringIDs.Add(3449384489U, "LatencyComponentSmtpReceiveDataInternal");
			Strings.stringIDs.Add(2279316324U, "InvalidTenantLicensePair");
			Strings.stringIDs.Add(3889416851U, "ColumnIndexesMustBeSequential");
			Strings.stringIDs.Add(3854009776U, "LatencyComponentNonSmtpGateway");
			Strings.stringIDs.Add(2176966386U, "LatencyComponentDeliveryAgentOnDeliverMailItem");
			Strings.stringIDs.Add(2711829446U, "ShadowRedundancyComponentBanner");
			Strings.stringIDs.Add(350383284U, "CloneMoveSourceModified");
			Strings.stringIDs.Add(1674978920U, "DatabaseRecoveryActionNone");
			Strings.stringIDs.Add(1142385700U, "LatencyComponentMailSubmissionServiceThrottling");
			Strings.stringIDs.Add(1673427229U, "LatencyComponentDsnGenerator");
			Strings.stringIDs.Add(2385069791U, "RowDeleted");
			Strings.stringIDs.Add(4154552087U, "LatencyComponentCategorizerContentConversion");
			Strings.stringIDs.Add(3393444732U, "LatencyComponentExternalServers");
			Strings.stringIDs.Add(3672571637U, "ReadMicrosoftExchangeRecipientFailed");
			Strings.stringIDs.Add(3993229380U, "SeekGeneralFailure");
			Strings.stringIDs.Add(3321191670U, "LatencyComponentCategorizerRouting");
			Strings.stringIDs.Add(1692037724U, "LatencyComponentOriginalMailDsn");
			Strings.stringIDs.Add(1471624947U, "LatencyComponentPickup");
			Strings.stringIDs.Add(615004755U, "LowRisk");
			Strings.stringIDs.Add(2919439981U, "LatencyComponentDeliveryAgentOnOpenConnection");
			Strings.stringIDs.Add(2349744297U, "LatencyComponentCategorizerOnRoutedMessage");
			Strings.stringIDs.Add(1601350579U, "CategorizerConfigValidationFailed");
			Strings.stringIDs.Add(2863890221U, "LatencyComponentNone");
			Strings.stringIDs.Add(2071866321U, "LatencyComponentDeliveryQueueLocking");
			Strings.stringIDs.Add(2768133224U, "LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionAD");
			Strings.stringIDs.Add(3862731819U, "MessageResubmissionComponentBanner");
			Strings.stringIDs.Add(3955727133U, "TotalExcludingPriorityNone");
			Strings.stringIDs.Add(3906053774U, "CloneMoveSourceNull");
			Strings.stringIDs.Add(92299947U, "LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionSmtp");
			Strings.stringIDs.Add(3127930220U, "LatencyComponentAgent");
			Strings.stringIDs.Add(650877237U, "InboundMailSubmissionFromHubsComponent");
			Strings.stringIDs.Add(3875073169U, "AlreadyJoined");
			Strings.stringIDs.Add(3994617651U, "LatencyComponentStoreDriverSubmissionRpc");
			Strings.stringIDs.Add(2896583167U, "NormalAndLowRisk");
			Strings.stringIDs.Add(175131561U, "LatencyComponentMailboxTransportSubmissionStoreDriverSubmission");
			Strings.stringIDs.Add(335920517U, "InvalidRoutingOverride");
			Strings.stringIDs.Add(3689512814U, "LatencyComponentSmtpReceiveOnDataCommand");
			Strings.stringIDs.Add(4114179756U, "LowRiskNormalPriority");
			Strings.stringIDs.Add(3171622157U, "AalCalClassificationDisplayName");
			Strings.stringIDs.Add(1744966745U, "HighlyConfidential");
			Strings.stringIDs.Add(2327669046U, "LatencyComponentStoreDriverOnInitializedMessage");
			Strings.stringIDs.Add(4016006905U, "LatencyComponentStoreDriverDeliveryStore");
			Strings.stringIDs.Add(3611628552U, "InvalidRowState");
			Strings.stringIDs.Add(3334507377U, "LatencyComponentMailboxTransportSubmissionService");
			Strings.stringIDs.Add(3514558095U, "LatencyComponentStoreDriverSubmissionAD");
			Strings.stringIDs.Add(2595056666U, "DumpsterJobStatusProcessing");
			Strings.stringIDs.Add(2912766470U, "RoutingNoLocalRgObject");
			Strings.stringIDs.Add(2117972488U, "PendingTransactions");
			Strings.stringIDs.Add(3750964199U, "TrailingEscape");
			Strings.stringIDs.Add(1679638210U, "CloneMoveTargetNotNew");
			Strings.stringIDs.Add(2607217314U, "LatencyComponentCategorizerResolver");
			Strings.stringIDs.Add(99902445U, "TransportComponentLoadFailed");
			Strings.stringIDs.Add(2986853275U, "LatencyComponentProcessingScheduler");
			Strings.stringIDs.Add(2104269935U, "LatencyComponentSmtpSendConnect");
			Strings.stringIDs.Add(936688982U, "Minimum");
			Strings.stringIDs.Add(1677063078U, "InboundMailSubmissionFromPickupDirectoryComponent");
			Strings.stringIDs.Add(1209655894U, "BreadCrumbSize");
			Strings.stringIDs.Add(885455923U, "CloneMoveSourceNotSaved");
			Strings.stringIDs.Add(3063068326U, "LatencyComponentShadowQueue");
			Strings.stringIDs.Add(3504101496U, "InvalidCursorState");
			Strings.stringIDs.Add(3589283362U, "LatencyComponentUnderThreshold");
			Strings.stringIDs.Add(3124262306U, "HighPriority");
			Strings.stringIDs.Add(1180878696U, "CircularClone");
			Strings.stringIDs.Add(3959631871U, "InvalidDeleteState");
			Strings.stringIDs.Add(642210074U, "LatencyComponentExternalPartnerServers");
			Strings.stringIDs.Add(1582326851U, "MailboxProxySendConnector");
			Strings.stringIDs.Add(3238954214U, "ExternalDestinationOutboundProxySendConnector");
			Strings.stringIDs.Add(2588281919U, "InvalidTransportServerRole");
			Strings.stringIDs.Add(3502104427U, "IncorrectColumn");
			Strings.stringIDs.Add(2368056258U, "CountWrong");
			Strings.stringIDs.Add(1954095043U, "LatencyComponentRmsAcquirePreLicense");
			Strings.stringIDs.Add(3452310564U, "SecureMailOuterLayerMustBeSigned");
			Strings.stringIDs.Add(3835436216U, "LatencyComponentMailboxMove");
			Strings.stringIDs.Add(1450242468U, "InternalDestinationOutboundProxySendConnector");
			Strings.stringIDs.Add(3494923132U, "IncorrectBrace");
			Strings.stringIDs.Add(309843435U, "KeyLength");
			Strings.stringIDs.Add(1227018995U, "InvalidRank");
			Strings.stringIDs.Add(3974043093U, "MimeWriteStreamOpen");
			Strings.stringIDs.Add(1578443098U, "StreamStateInvalid");
			Strings.stringIDs.Add(2070671180U, "NormalRiskLowPriority");
			Strings.stringIDs.Add(2721873196U, "InboundMailSubmissionFromInternetComponent");
			Strings.stringIDs.Add(3160113472U, "LatencyComponentCategorizerOnResolvedMessage");
			Strings.stringIDs.Add(4049537308U, "BodyFormatUnsupported");
		}

		public static LocalizedString RoutingNoAdSites
		{
			get
			{
				return new LocalizedString("RoutingNoAdSites", "ExB4B5E0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentHeartbeat
		{
			get
			{
				return new LocalizedString("LatencyComponentHeartbeat", "Ex3347E3", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ComponentsDisabledNone
		{
			get
			{
				return new LocalizedString("ComponentsDisabledNone", "ExBC74F8", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentRmsRequestDelegationToken
		{
			get
			{
				return new LocalizedString("LatencyComponentRmsRequestDelegationToken", "Ex1029ED", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentRmsAcquireServerBoxRac
		{
			get
			{
				return new LocalizedString("LatencyComponentRmsAcquireServerBoxRac", "Ex9BBD15", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionStoreStats
		{
			get
			{
				return new LocalizedString("LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionStoreStats", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseOpen
		{
			get
			{
				return new LocalizedString("DatabaseOpen", "ExC6ED6D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ColumnName
		{
			get
			{
				return new LocalizedString("ColumnName", "ExF5AE98", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShadowSendConnector
		{
			get
			{
				return new LocalizedString("ShadowSendConnector", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentDeliveryQueueMailboxInsufficientResources
		{
			get
			{
				return new LocalizedString("LatencyComponentDeliveryQueueMailboxInsufficientResources", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Restricted
		{
			get
			{
				return new LocalizedString("Restricted", "ExFF6B5E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentStoreDriverDeliveryContentConversion
		{
			get
			{
				return new LocalizedString("LatencyComponentStoreDriverDeliveryContentConversion", "ExEB2D89", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IntraorgSendConnectorName
		{
			get
			{
				return new LocalizedString("IntraorgSendConnectorName", "Ex25CD79", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PhysicalMemoryUses(int pressure, int limit)
		{
			return new LocalizedString("PhysicalMemoryUses", "Ex6A7FEE", false, true, Strings.ResourceManager, new object[]
			{
				pressure,
				limit
			});
		}

		public static LocalizedString LatencyComponentSmtpReceiveCommitLocal
		{
			get
			{
				return new LocalizedString("LatencyComponentSmtpReceiveCommitLocal", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentDeliveryQueueMailbox
		{
			get
			{
				return new LocalizedString("LatencyComponentDeliveryQueueMailbox", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentDeliveryAgent
		{
			get
			{
				return new LocalizedString("LatencyComponentDeliveryAgent", "ExCC0B74", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentRmsFindServiceLocation
		{
			get
			{
				return new LocalizedString("LatencyComponentRmsFindServiceLocation", "Ex19390E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NormalPriority
		{
			get
			{
				return new LocalizedString("NormalPriority", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentSmtpReceiveOnEndOfHeaders
		{
			get
			{
				return new LocalizedString("LatencyComponentSmtpReceiveOnEndOfHeaders", "ExF1ACF6", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Confidential
		{
			get
			{
				return new LocalizedString("Confidential", "Ex3F6E10", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NormalRisk
		{
			get
			{
				return new LocalizedString("NormalRisk", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShadowRedundancyNoActiveServerInNexthopSolution
		{
			get
			{
				return new LocalizedString("ShadowRedundancyNoActiveServerInNexthopSolution", "Ex69F8D5", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentStoreDriverOnCompletedMessage
		{
			get
			{
				return new LocalizedString("LatencyComponentStoreDriverOnCompletedMessage", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseAttachFailed(string databaseName)
		{
			return new LocalizedString("DatabaseAttachFailed", "", false, false, Strings.ResourceManager, new object[]
			{
				databaseName
			});
		}

		public static LocalizedString HighRisk
		{
			get
			{
				return new LocalizedString("HighRisk", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseRecoveryActionDelete
		{
			get
			{
				return new LocalizedString("DatabaseRecoveryActionDelete", "Ex62B1D2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentSmtpReceiveOnProxyInboundMessage
		{
			get
			{
				return new LocalizedString("LatencyComponentSmtpReceiveOnProxyInboundMessage", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionSmtpOut
		{
			get
			{
				return new LocalizedString("LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionSmtpOut", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentSmtpSend
		{
			get
			{
				return new LocalizedString("LatencyComponentSmtpSend", "ExDD5B34", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MediumResourceUses
		{
			get
			{
				return new LocalizedString("MediumResourceUses", "Ex6A4267", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentSmtpReceiveCommitRemote
		{
			get
			{
				return new LocalizedString("LatencyComponentSmtpReceiveCommitRemote", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidCharset(string name)
		{
			return new LocalizedString("InvalidCharset", "Ex38A3AD", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString LatencyComponentCategorizer
		{
			get
			{
				return new LocalizedString("LatencyComponentCategorizer", "ExFACFA2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ResourcesInNormalPressure(string resources)
		{
			return new LocalizedString("ResourcesInNormalPressure", "ExE6E8C7", false, true, Strings.ResourceManager, new object[]
			{
				resources
			});
		}

		public static LocalizedString ResourceUses(string name, int pressure, string uses, int normal, int medium, int high)
		{
			return new LocalizedString("ResourceUses", "Ex374B24", false, true, Strings.ResourceManager, new object[]
			{
				name,
				pressure,
				uses,
				normal,
				medium,
				high
			});
		}

		public static LocalizedString None
		{
			get
			{
				return new LocalizedString("None", "Ex8D93DD", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentDumpster
		{
			get
			{
				return new LocalizedString("LatencyComponentDumpster", "Ex36EE06", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EnumeratorBadPosition
		{
			get
			{
				return new LocalizedString("EnumeratorBadPosition", "Ex8642C3", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentRmsAcquireCertificationMexData
		{
			get
			{
				return new LocalizedString("LatencyComponentRmsAcquireCertificationMexData", "Ex50E004", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContentAggregationComponent
		{
			get
			{
				return new LocalizedString("ContentAggregationComponent", "Ex16EB9F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DiscardingDataFalse
		{
			get
			{
				return new LocalizedString("DiscardingDataFalse", "ExE5C5D8", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseClosed
		{
			get
			{
				return new LocalizedString("DatabaseClosed", "Ex92FBE4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToReadServerRole
		{
			get
			{
				return new LocalizedString("FailedToReadServerRole", "ExC69380", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentDeliveryQueueMailboxDynamicMailboxDatabaseThrottlingLimitExceeded
		{
			get
			{
				return new LocalizedString("LatencyComponentDeliveryQueueMailboxDynamicMailboxDatabaseThrottlingLimitExceeded", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentMailSubmissionServiceNotify
		{
			get
			{
				return new LocalizedString("LatencyComponentMailSubmissionServiceNotify", "Ex5B1A7D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentStoreDriverSubmissionStore
		{
			get
			{
				return new LocalizedString("LatencyComponentStoreDriverSubmissionStore", "Ex93C7FE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SeekFailed
		{
			get
			{
				return new LocalizedString("SeekFailed", "ExDCE0E2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentMailSubmissionServiceNotifyRetrySchedule
		{
			get
			{
				return new LocalizedString("LatencyComponentMailSubmissionServiceNotifyRetrySchedule", "ExA34296", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentStoreDriverOnDemotedMessage
		{
			get
			{
				return new LocalizedString("LatencyComponentStoreDriverOnDemotedMessage", "Ex339F42", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Public
		{
			get
			{
				return new LocalizedString("Public", "ExCEA3DE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseAndDatabaseLogDeleted(string databasePath, string databaseLogPath)
		{
			return new LocalizedString("DatabaseAndDatabaseLogDeleted", "", false, false, Strings.ResourceManager, new object[]
			{
				databasePath,
				databaseLogPath
			});
		}

		public static LocalizedString AcceptedDomainTableNotLoaded
		{
			get
			{
				return new LocalizedString("AcceptedDomainTableNotLoaded", "Ex353A51", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentServiceRestart
		{
			get
			{
				return new LocalizedString("LatencyComponentServiceRestart", "Ex87CE3A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NormalRiskNonePriority
		{
			get
			{
				return new LocalizedString("NormalRiskNonePriority", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoutingDecryptConnectorPasswordFailure(string connectorName, string errorCode)
		{
			return new LocalizedString("RoutingDecryptConnectorPasswordFailure", "ExE7DD67", false, true, Strings.ResourceManager, new object[]
			{
				connectorName,
				errorCode
			});
		}

		public static LocalizedString DatabaseLoggingResource(string loggingPath)
		{
			return new LocalizedString("DatabaseLoggingResource", "Ex2F7357", false, true, Strings.ResourceManager, new object[]
			{
				loggingPath
			});
		}

		public static LocalizedString LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionContentConversion
		{
			get
			{
				return new LocalizedString("LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionContentConversion", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentDeliveryQueueMailboxMapiExceptionTimeout
		{
			get
			{
				return new LocalizedString("LatencyComponentDeliveryQueueMailboxMapiExceptionTimeout", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentSmtpSendMailboxDelivery
		{
			get
			{
				return new LocalizedString("LatencyComponentSmtpSendMailboxDelivery", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentDeliveryQueueExternal
		{
			get
			{
				return new LocalizedString("LatencyComponentDeliveryQueueExternal", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HighAndBulkRisk
		{
			get
			{
				return new LocalizedString("HighAndBulkRisk", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JetOperationFailure
		{
			get
			{
				return new LocalizedString("JetOperationFailure", "Ex053A49", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentSmtpReceive
		{
			get
			{
				return new LocalizedString("LatencyComponentSmtpReceive", "Ex86CE7C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentSmtpReceiveDataExternal
		{
			get
			{
				return new LocalizedString("LatencyComponentSmtpReceiveDataExternal", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentStoreDriverDeliveryAD
		{
			get
			{
				return new LocalizedString("LatencyComponentStoreDriverDeliveryAD", "Ex561E96", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentDeliveryQueueMailboxDeliverAgentTransientFailure
		{
			get
			{
				return new LocalizedString("LatencyComponentDeliveryQueueMailboxDeliverAgentTransientFailure", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentCategorizerFinal
		{
			get
			{
				return new LocalizedString("LatencyComponentCategorizerFinal", "Ex66AF88", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BulkRisk
		{
			get
			{
				return new LocalizedString("BulkRisk", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentSubmissionQueue
		{
			get
			{
				return new LocalizedString("LatencyComponentSubmissionQueue", "Ex3B52C2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentStoreDriverSubmit
		{
			get
			{
				return new LocalizedString("LatencyComponentStoreDriverSubmit", "Ex757015", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DumpsterJobStatusQueued
		{
			get
			{
				return new LocalizedString("DumpsterJobStatusQueued", "Ex89538D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TlsCertificateNameNotFound(string tlsCertificateName, string connectorName)
		{
			return new LocalizedString("TlsCertificateNameNotFound", "", false, false, Strings.ResourceManager, new object[]
			{
				tlsCertificateName,
				connectorName
			});
		}

		public static LocalizedString LatencyComponentStoreDriverDeliveryMailboxDatabaseThrottling
		{
			get
			{
				return new LocalizedString("LatencyComponentStoreDriverDeliveryMailboxDatabaseThrottling", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionOnDemotedMessage
		{
			get
			{
				return new LocalizedString("LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionOnDemotedMessage", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SecureMailInvalidNumberOfLayers
		{
			get
			{
				return new LocalizedString("SecureMailInvalidNumberOfLayers", "Ex8EF442", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LowRiskLowPriority
		{
			get
			{
				return new LocalizedString("LowRiskLowPriority", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentMailSubmissionServiceFailedAttempt
		{
			get
			{
				return new LocalizedString("LatencyComponentMailSubmissionServiceFailedAttempt", "ExA27F43", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteDomainTableNotLoaded
		{
			get
			{
				return new LocalizedString("RemoteDomainTableNotLoaded", "ExFF7502", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ColumnsMustBeOrdered(string columnName)
		{
			return new LocalizedString("ColumnsMustBeOrdered", "Ex09A2C2", false, true, Strings.ResourceManager, new object[]
			{
				columnName
			});
		}

		public static LocalizedString OutboundMailDeliveryToRemoteDomainsComponent
		{
			get
			{
				return new LocalizedString("OutboundMailDeliveryToRemoteDomainsComponent", "Ex31F696", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AttachmentReadFailed
		{
			get
			{
				return new LocalizedString("AttachmentReadFailed", "Ex0885BC", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentMailboxRules
		{
			get
			{
				return new LocalizedString("LatencyComponentMailboxRules", "ExB7F29A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoutingIdenticalExchangeLegacyDns(string server1, string server2)
		{
			return new LocalizedString("RoutingIdenticalExchangeLegacyDns", "ExF4FA16", false, true, Strings.ResourceManager, new object[]
			{
				server1,
				server2
			});
		}

		public static LocalizedString LowResourceUses
		{
			get
			{
				return new LocalizedString("LowResourceUses", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionStoreDisposeSession
		{
			get
			{
				return new LocalizedString("LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionStoreDisposeSession", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString High
		{
			get
			{
				return new LocalizedString("High", "ExA8278F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IdentityParameterNotFound
		{
			get
			{
				return new LocalizedString("IdentityParameterNotFound", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotOpenForWrite
		{
			get
			{
				return new LocalizedString("NotOpenForWrite", "ExADA24D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TcpListenerError
		{
			get
			{
				return new LocalizedString("TcpListenerError", "Ex521241", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentMexRuntimeThreadpoolQueue
		{
			get
			{
				return new LocalizedString("LatencyComponentMexRuntimeThreadpoolQueue", "Ex289D68", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionPerfContextLdap
		{
			get
			{
				return new LocalizedString("LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionPerfContextLdap", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NormalResourceUses
		{
			get
			{
				return new LocalizedString("NormalResourceUses", "ExA91D90", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DumpsterJobStatusCreated
		{
			get
			{
				return new LocalizedString("DumpsterJobStatusCreated", "Ex450EBD", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LowPriority
		{
			get
			{
				return new LocalizedString("LowPriority", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalDestinationInboundProxySendConnector
		{
			get
			{
				return new LocalizedString("ExternalDestinationInboundProxySendConnector", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IPFilterDatabaseInstanceName
		{
			get
			{
				return new LocalizedString("IPFilterDatabaseInstanceName", "Ex3E0C50", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ActivationFailed
		{
			get
			{
				return new LocalizedString("ActivationFailed", "Ex3BCE23", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentDelivery
		{
			get
			{
				return new LocalizedString("LatencyComponentDelivery", "Ex24CE50", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AggregateResource
		{
			get
			{
				return new LocalizedString("AggregateResource", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DataBaseError(string databaseName)
		{
			return new LocalizedString("DataBaseError", "", false, false, Strings.ResourceManager, new object[]
			{
				databaseName
			});
		}

		public static LocalizedString DuplicateColumnIndexes(string columnNameA, string columnNameB)
		{
			return new LocalizedString("DuplicateColumnIndexes", "Ex9939EC", false, true, Strings.ResourceManager, new object[]
			{
				columnNameA,
				columnNameB
			});
		}

		public static LocalizedString LatencyComponentProcessingSchedulerScoped
		{
			get
			{
				return new LocalizedString("LatencyComponentProcessingSchedulerScoped", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentSmtpReceiveOnRcpt2Command
		{
			get
			{
				return new LocalizedString("LatencyComponentSmtpReceiveOnRcpt2Command", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentStoreDriverOnPromotedMessage
		{
			get
			{
				return new LocalizedString("LatencyComponentStoreDriverOnPromotedMessage", "Ex0F574F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PoisonMessageRegistryAccessFailed
		{
			get
			{
				return new LocalizedString("PoisonMessageRegistryAccessFailed", "ExD03EAE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HighResourceUses
		{
			get
			{
				return new LocalizedString("HighResourceUses", "Ex53218A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EnvelopRecipientDisposed
		{
			get
			{
				return new LocalizedString("EnvelopRecipientDisposed", "Ex65E8AE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SystemMemory
		{
			get
			{
				return new LocalizedString("SystemMemory", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReadOrgContainerFailed
		{
			get
			{
				return new LocalizedString("ReadOrgContainerFailed", "ExDB3FBD", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentStoreDriverOnCreatedMessage
		{
			get
			{
				return new LocalizedString("LatencyComponentStoreDriverOnCreatedMessage", "ExAFC8A5", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CategorizerMaxConfigLoadRetriesReached
		{
			get
			{
				return new LocalizedString("CategorizerMaxConfigLoadRetriesReached", "Ex22D247", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionStoreOpenSession
		{
			get
			{
				return new LocalizedString("LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionStoreOpenSession", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoutingLocalServerIsNotBridgehead
		{
			get
			{
				return new LocalizedString("RoutingLocalServerIsNotBridgehead", "Ex225794", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FilePathOnLockedVolume(string filePath, int retrySeconds, int retryCount, int maxRetryCount)
		{
			return new LocalizedString("FilePathOnLockedVolume", "", false, false, Strings.ResourceManager, new object[]
			{
				filePath,
				retrySeconds,
				retryCount,
				maxRetryCount
			});
		}

		public static LocalizedString BitlockerQueryFailed(string filePath, string exception, int retrySeconds, int retryCount, int maxRetryCount)
		{
			return new LocalizedString("BitlockerQueryFailed", "", false, false, Strings.ResourceManager, new object[]
			{
				filePath,
				exception,
				retrySeconds,
				retryCount,
				maxRetryCount
			});
		}

		public static LocalizedString LatencyComponentCategorizerOnCategorizedMessage
		{
			get
			{
				return new LocalizedString("LatencyComponentCategorizerOnCategorizedMessage", "ExF69D17", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentRmsAcquireB2BRac
		{
			get
			{
				return new LocalizedString("LatencyComponentRmsAcquireB2BRac", "Ex0ED294", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InternalDestinationInboundProxySendConnector
		{
			get
			{
				return new LocalizedString("InternalDestinationInboundProxySendConnector", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentDeliveryQueueMailboxMaxConcurrentMessageSizeLimitExceeded
		{
			get
			{
				return new LocalizedString("LatencyComponentDeliveryQueueMailboxMaxConcurrentMessageSizeLimitExceeded", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentStoreDriverDeliveryRpc
		{
			get
			{
				return new LocalizedString("LatencyComponentStoreDriverDeliveryRpc", "Ex2DB2CA", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectionInUse
		{
			get
			{
				return new LocalizedString("ConnectionInUse", "ExC13129", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentMailSubmissionService
		{
			get
			{
				return new LocalizedString("LatencyComponentMailSubmissionService", "ExF3DDF1", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NonePriority
		{
			get
			{
				return new LocalizedString("NonePriority", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSmtpAddress(string address)
		{
			return new LocalizedString("InvalidSmtpAddress", "Ex8C830F", false, true, Strings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString LatencyComponentRmsAcquireTemplateInfo
		{
			get
			{
				return new LocalizedString("LatencyComponentRmsAcquireTemplateInfo", "ExD68E65", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentContentAggregation
		{
			get
			{
				return new LocalizedString("LatencyComponentContentAggregation", "Ex30AF3D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentMailSubmissionServiceShadowResubmitDecision
		{
			get
			{
				return new LocalizedString("LatencyComponentMailSubmissionServiceShadowResubmitDecision", "Ex79D02D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentContentAggregationMailItemCommit
		{
			get
			{
				return new LocalizedString("LatencyComponentContentAggregationMailItemCommit", "Ex1030A7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseRecoveryActionMove
		{
			get
			{
				return new LocalizedString("DatabaseRecoveryActionMove", "Ex3E9869", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReadTransportServerConfigFailed
		{
			get
			{
				return new LocalizedString("ReadTransportServerConfigFailed", "Ex9C96A5", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionHubSelector
		{
			get
			{
				return new LocalizedString("LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionHubSelector", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTrackingConfigNotFound
		{
			get
			{
				return new LocalizedString("MessageTrackingConfigNotFound", "ExEB8A78", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RMSTemplateNotFound(Guid templateId)
		{
			return new LocalizedString("RMSTemplateNotFound", "Ex4060AE", false, true, Strings.ResourceManager, new object[]
			{
				templateId
			});
		}

		public static LocalizedString LatencyComponentDeliveryQueueInternal
		{
			get
			{
				return new LocalizedString("LatencyComponentDeliveryQueueInternal", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentDeliveryQueueMailboxMailboxServerOffline
		{
			get
			{
				return new LocalizedString("LatencyComponentDeliveryQueueMailboxMailboxServerOffline", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PrivateBytesResource
		{
			get
			{
				return new LocalizedString("PrivateBytesResource", "ExF17246", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VersionBuckets(string databasePath)
		{
			return new LocalizedString("VersionBuckets", "", false, false, Strings.ResourceManager, new object[]
			{
				databasePath
			});
		}

		public static LocalizedString LatencyComponentDeliveryQueueMailboxMailboxDatabaseOffline
		{
			get
			{
				return new LocalizedString("LatencyComponentDeliveryQueueMailboxMailboxDatabaseOffline", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotModifyCompletedRequest(long requestId)
		{
			return new LocalizedString("CannotModifyCompletedRequest", "", false, false, Strings.ResourceManager, new object[]
			{
				requestId
			});
		}

		public static LocalizedString RoutingMaxConfigLoadRetriesReached
		{
			get
			{
				return new LocalizedString("RoutingMaxConfigLoadRetriesReached", "Ex7D039C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentRmsAcquireServerLicensingMexData
		{
			get
			{
				return new LocalizedString("LatencyComponentRmsAcquireServerLicensingMexData", "ExD58728", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentDeliveryQueueMailboxRecipientThreadLimitExceeded
		{
			get
			{
				return new LocalizedString("LatencyComponentDeliveryQueueMailboxRecipientThreadLimitExceeded", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SmtpReceiveParserNegativeBytes
		{
			get
			{
				return new LocalizedString("SmtpReceiveParserNegativeBytes", "Ex3B35E5", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRoleChange
		{
			get
			{
				return new LocalizedString("InvalidRoleChange", "Ex19D8AA", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DefaultAuthoritativeDomainNotFound(OrganizationId orgId)
		{
			return new LocalizedString("DefaultAuthoritativeDomainNotFound", "ExF5500C", false, true, Strings.ResourceManager, new object[]
			{
				orgId
			});
		}

		public static LocalizedString LatencyComponentPoisonQueue
		{
			get
			{
				return new LocalizedString("LatencyComponentPoisonQueue", "ExC760D5", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BootScannerComponent
		{
			get
			{
				return new LocalizedString("BootScannerComponent", "Ex7FD341", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotInTransaction
		{
			get
			{
				return new LocalizedString("NotInTransaction", "Ex28A52B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentSmtpReceiveOnRcptCommand
		{
			get
			{
				return new LocalizedString("LatencyComponentSmtpReceiveOnRcptCommand", "ExB873C3", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BodyReadFailed
		{
			get
			{
				return new LocalizedString("BodyReadFailed", "Ex6EC7B9", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TextConvertersFailed
		{
			get
			{
				return new LocalizedString("TextConvertersFailed", "Ex38BA0F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoutingNoRoutingGroups
		{
			get
			{
				return new LocalizedString("RoutingNoRoutingGroups", "Ex733C6D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentStoreDriverDeliveryMessageConcurrency
		{
			get
			{
				return new LocalizedString("LatencyComponentStoreDriverDeliveryMessageConcurrency", "Ex5E0FC5", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Submission
		{
			get
			{
				return new LocalizedString("Submission", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReadingADConfigFailed
		{
			get
			{
				return new LocalizedString("ReadingADConfigFailed", "Ex4986D4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TemporaryStorageResource(string tempPath)
		{
			return new LocalizedString("TemporaryStorageResource", "", false, false, Strings.ResourceManager, new object[]
			{
				tempPath
			});
		}

		public static LocalizedString LatencyComponentTooManyComponents
		{
			get
			{
				return new LocalizedString("LatencyComponentTooManyComponents", "Ex261973", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReadTransportConfigConfigFailed
		{
			get
			{
				return new LocalizedString("ReadTransportConfigConfigFailed", "Ex96B07F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SchemaRequiredColumnNotFound(string table, string columnName)
		{
			return new LocalizedString("SchemaRequiredColumnNotFound", "ExB91D51", false, true, Strings.ResourceManager, new object[]
			{
				table,
				columnName
			});
		}

		public static LocalizedString InvalidMessageResubmissionState
		{
			get
			{
				return new LocalizedString("InvalidMessageResubmissionState", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentSmtpReceiveOnEndOfData
		{
			get
			{
				return new LocalizedString("LatencyComponentSmtpReceiveOnEndOfData", "Ex8D038F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QueueLength(string queueName)
		{
			return new LocalizedString("QueueLength", "", false, false, Strings.ResourceManager, new object[]
			{
				queueName
			});
		}

		public static LocalizedString InvalidTransportRole
		{
			get
			{
				return new LocalizedString("InvalidTransportRole", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentReplay
		{
			get
			{
				return new LocalizedString("LatencyComponentReplay", "ExCD83D5", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CloneMoveDestination
		{
			get
			{
				return new LocalizedString("CloneMoveDestination", "Ex2A3AFF", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LowRiskNonePriority
		{
			get
			{
				return new LocalizedString("LowRiskNonePriority", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentCategorizerBifurcation
		{
			get
			{
				return new LocalizedString("LatencyComponentCategorizerBifurcation", "Ex4E745B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SchemaInvalid
		{
			get
			{
				return new LocalizedString("SchemaInvalid", "Ex18CF4C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuoteNestLevel
		{
			get
			{
				return new LocalizedString("QuoteNestLevel", "Ex8DA874", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentUnknown
		{
			get
			{
				return new LocalizedString("LatencyComponentUnknown", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentTotal
		{
			get
			{
				return new LocalizedString("LatencyComponentTotal", "Ex2FE97B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentRmsAcquireLicense
		{
			get
			{
				return new LocalizedString("LatencyComponentRmsAcquireLicense", "ExF834A0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboundMailSubmissionFromReplayDirectoryComponent
		{
			get
			{
				return new LocalizedString("InboundMailSubmissionFromReplayDirectoryComponent", "ExA0D92A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoColumns
		{
			get
			{
				return new LocalizedString("NoColumns", "Ex432F52", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentRmsAcquireClc
		{
			get
			{
				return new LocalizedString("LatencyComponentRmsAcquireClc", "Ex983A86", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CloneMoveComplete
		{
			get
			{
				return new LocalizedString("CloneMoveComplete", "ExC21EF2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseIsNotMovable(string sourcePath, string destPath)
		{
			return new LocalizedString("DatabaseIsNotMovable", "ExC75FF5", false, true, Strings.ResourceManager, new object[]
			{
				sourcePath,
				destPath
			});
		}

		public static LocalizedString LatencyComponentQuarantineReleaseOrReport
		{
			get
			{
				return new LocalizedString("LatencyComponentQuarantineReleaseOrReport", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentSubmissionAssistant
		{
			get
			{
				return new LocalizedString("LatencyComponentSubmissionAssistant", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DumpsterJobResponseSuccess
		{
			get
			{
				return new LocalizedString("DumpsterJobResponseSuccess", "Ex051B82", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExtractNotAllowed(Uri url, string orgId)
		{
			return new LocalizedString("ExtractNotAllowed", "ExFA32C2", false, true, Strings.ResourceManager, new object[]
			{
				url,
				orgId
			});
		}

		public static LocalizedString AgentComponentFailed
		{
			get
			{
				return new LocalizedString("AgentComponentFailed", "Ex76E2B6", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotRemoveRequestInRunningState(long requestId)
		{
			return new LocalizedString("CannotRemoveRequestInRunningState", "", false, false, Strings.ResourceManager, new object[]
			{
				requestId
			});
		}

		public static LocalizedString ClientProxySendConnector
		{
			get
			{
				return new LocalizedString("ClientProxySendConnector", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Basic
		{
			get
			{
				return new LocalizedString("Basic", "ExA01C4F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentCategorizerOnSubmittedMessage
		{
			get
			{
				return new LocalizedString("LatencyComponentCategorizerOnSubmittedMessage", "Ex952BE3", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NormalRiskNormalPriority
		{
			get
			{
				return new LocalizedString("NormalRiskNormalPriority", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentSmtpReceiveCommit
		{
			get
			{
				return new LocalizedString("LatencyComponentSmtpReceiveCommit", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoutingNoLocalServer
		{
			get
			{
				return new LocalizedString("RoutingNoLocalServer", "Ex42844E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SecureMailSecondLayerMustBeEnveloped
		{
			get
			{
				return new LocalizedString("SecureMailSecondLayerMustBeEnveloped", "Ex200777", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseAndDatabaseLogMoved(string databasePath, string databaseMovePath, string databaseLogPath, string databaseLogMovePath)
		{
			return new LocalizedString("DatabaseAndDatabaseLogMoved", "", false, false, Strings.ResourceManager, new object[]
			{
				databasePath,
				databaseMovePath,
				databaseLogPath,
				databaseLogMovePath
			});
		}

		public static LocalizedString MailItemDeferred
		{
			get
			{
				return new LocalizedString("MailItemDeferred", "Ex371895", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboundMailSubmissionFromMailboxComponent
		{
			get
			{
				return new LocalizedString("InboundMailSubmissionFromMailboxComponent", "ExA8CDDA", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AttachmentProtectionFailed
		{
			get
			{
				return new LocalizedString("AttachmentProtectionFailed", "ExEA40C1", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentSubmissionAssistantThrottling
		{
			get
			{
				return new LocalizedString("LatencyComponentSubmissionAssistantThrottling", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DiskFull(string path)
		{
			return new LocalizedString("DiskFull", "ExFECC85", false, true, Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString DatabaseResource(string databasePath)
		{
			return new LocalizedString("DatabaseResource", "Ex6ADCE8", false, true, Strings.ResourceManager, new object[]
			{
				databasePath
			});
		}

		public static LocalizedString LatencyComponentCategorizerLocking
		{
			get
			{
				return new LocalizedString("LatencyComponentCategorizerLocking", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValueNull
		{
			get
			{
				return new LocalizedString("ValueNull", "ExC68788", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseSchemaNotSupported(string databaseName)
		{
			return new LocalizedString("DatabaseSchemaNotSupported", "", false, false, Strings.ResourceManager, new object[]
			{
				databaseName
			});
		}

		public static LocalizedString TooManyAgents
		{
			get
			{
				return new LocalizedString("TooManyAgents", "ExB98825", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ColumnAccessInvalid(string columnName)
		{
			return new LocalizedString("ColumnAccessInvalid", "ExF3238F", false, true, Strings.ResourceManager, new object[]
			{
				columnName
			});
		}

		public static LocalizedString DumpsterJobStatusCompleted
		{
			get
			{
				return new LocalizedString("DumpsterJobStatusCompleted", "Ex2E880E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentStoreDriverOnDeliveredMessage
		{
			get
			{
				return new LocalizedString("LatencyComponentStoreDriverOnDeliveredMessage", "ExB82838", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentRmsAcquireB2BLicense
		{
			get
			{
				return new LocalizedString("LatencyComponentRmsAcquireB2BLicense", "Ex281C09", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentDeliveryQueueMailboxMapiExceptionLockViolation
		{
			get
			{
				return new LocalizedString("LatencyComponentDeliveryQueueMailboxMapiExceptionLockViolation", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Medium
		{
			get
			{
				return new LocalizedString("Medium", "ExFE566F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotBufferedStream
		{
			get
			{
				return new LocalizedString("NotBufferedStream", "Ex623A85", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseMoved(string databasePath, string movePath)
		{
			return new LocalizedString("DatabaseMoved", "", false, false, Strings.ResourceManager, new object[]
			{
				databasePath,
				movePath
			});
		}

		public static LocalizedString IndexOutOfBounds(int index, int count)
		{
			return new LocalizedString("IndexOutOfBounds", "Ex3A8B25", false, true, Strings.ResourceManager, new object[]
			{
				index,
				count
			});
		}

		public static LocalizedString IncorrectBaseStream
		{
			get
			{
				return new LocalizedString("IncorrectBaseStream", "Ex697148", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentProcess
		{
			get
			{
				return new LocalizedString("LatencyComponentProcess", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CommitMailFailed
		{
			get
			{
				return new LocalizedString("CommitMailFailed", "ExD1D56C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NonAsciiData
		{
			get
			{
				return new LocalizedString("NonAsciiData", "ExE35D2D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OutboundConnectorNotFound(string name, OrganizationId orgId)
		{
			return new LocalizedString("OutboundConnectorNotFound", "", false, false, Strings.ResourceManager, new object[]
			{
				name,
				orgId
			});
		}

		public static LocalizedString SchemaVersion(long expected, long found)
		{
			return new LocalizedString("SchemaVersion", "Ex8ED756", false, true, Strings.ResourceManager, new object[]
			{
				expected,
				found
			});
		}

		public static LocalizedString SeekBarred
		{
			get
			{
				return new LocalizedString("SeekBarred", "Ex39B2E6", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoutingLocalRgNotSet
		{
			get
			{
				return new LocalizedString("RoutingLocalRgNotSet", "Ex72888D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TransportComponentLoadFailedWithName(string componentName)
		{
			return new LocalizedString("TransportComponentLoadFailedWithName", "", false, false, Strings.ResourceManager, new object[]
			{
				componentName
			});
		}

		public static LocalizedString MessagingDatabaseInstanceName
		{
			get
			{
				return new LocalizedString("MessagingDatabaseInstanceName", "Ex235DD2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentUnreachableQueue
		{
			get
			{
				return new LocalizedString("LatencyComponentUnreachableQueue", "ExE09761", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentStoreDriverDelivery
		{
			get
			{
				return new LocalizedString("LatencyComponentStoreDriverDelivery", "ExDE552B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseStillInUse
		{
			get
			{
				return new LocalizedString("DatabaseStillInUse", "ExAEEC70", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentDeferral
		{
			get
			{
				return new LocalizedString("LatencyComponentDeferral", "ExAA114E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentMExReserved(int index)
		{
			return new LocalizedString("LatencyComponentMExReserved", "Ex899D35", false, true, Strings.ResourceManager, new object[]
			{
				index
			});
		}

		public static LocalizedString ValueIsTooLarge(int length, int maxLength)
		{
			return new LocalizedString("ValueIsTooLarge", "ExBB4978", false, true, Strings.ResourceManager, new object[]
			{
				length,
				maxLength
			});
		}

		public static LocalizedString DumpsterJobResponseRetryLater
		{
			get
			{
				return new LocalizedString("DumpsterJobResponseRetryLater", "Ex9C089F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoutingNoLocalAdSite
		{
			get
			{
				return new LocalizedString("RoutingNoLocalAdSite", "Ex2DC77D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentRmsAcquireTemplates
		{
			get
			{
				return new LocalizedString("LatencyComponentRmsAcquireTemplates", "Ex7EB976", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRoutingOverrideEvent
		{
			get
			{
				return new LocalizedString("InvalidRoutingOverrideEvent", "ExDD58BB", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetSclThresholdDefaultValueOutOfRange
		{
			get
			{
				return new LocalizedString("GetSclThresholdDefaultValueOutOfRange", "Ex7F4CB6", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentSmtpReceiveDataInternal
		{
			get
			{
				return new LocalizedString("LatencyComponentSmtpReceiveDataInternal", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidTenantLicensePair
		{
			get
			{
				return new LocalizedString("InvalidTenantLicensePair", "Ex698AF4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ColumnIndexesMustBeSequential
		{
			get
			{
				return new LocalizedString("ColumnIndexesMustBeSequential", "Ex61D691", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentNonSmtpGateway
		{
			get
			{
				return new LocalizedString("LatencyComponentNonSmtpGateway", "Ex6C9727", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentDeliveryAgentOnDeliverMailItem
		{
			get
			{
				return new LocalizedString("LatencyComponentDeliveryAgentOnDeliverMailItem", "ExA2B488", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShadowRedundancyComponentBanner
		{
			get
			{
				return new LocalizedString("ShadowRedundancyComponentBanner", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CloneMoveSourceModified
		{
			get
			{
				return new LocalizedString("CloneMoveSourceModified", "Ex67EFA9", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseRecoveryActionNone
		{
			get
			{
				return new LocalizedString("DatabaseRecoveryActionNone", "Ex074D46", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentMailSubmissionServiceThrottling
		{
			get
			{
				return new LocalizedString("LatencyComponentMailSubmissionServiceThrottling", "ExAA6A02", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DataBaseInUse(string databaseName)
		{
			return new LocalizedString("DataBaseInUse", "", false, false, Strings.ResourceManager, new object[]
			{
				databaseName
			});
		}

		public static LocalizedString SchemaTypeMismatch(JET_coltyp expected, JET_coltyp got)
		{
			return new LocalizedString("SchemaTypeMismatch", "Ex6DC38A", false, true, Strings.ResourceManager, new object[]
			{
				expected,
				got
			});
		}

		public static LocalizedString LatencyComponentDsnGenerator
		{
			get
			{
				return new LocalizedString("LatencyComponentDsnGenerator", "ExCD3EC7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RowDeleted
		{
			get
			{
				return new LocalizedString("RowDeleted", "Ex699AF2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentCategorizerContentConversion
		{
			get
			{
				return new LocalizedString("LatencyComponentCategorizerContentConversion", "ExC970B7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentExternalServers
		{
			get
			{
				return new LocalizedString("LatencyComponentExternalServers", "ExDEB5BD", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UsedDiskSpaceResource(string tempPath)
		{
			return new LocalizedString("UsedDiskSpaceResource", "", false, false, Strings.ResourceManager, new object[]
			{
				tempPath
			});
		}

		public static LocalizedString ReadMicrosoftExchangeRecipientFailed
		{
			get
			{
				return new LocalizedString("ReadMicrosoftExchangeRecipientFailed", "ExE36E8E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SeekGeneralFailure
		{
			get
			{
				return new LocalizedString("SeekGeneralFailure", "Ex7CBFF4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentCategorizerRouting
		{
			get
			{
				return new LocalizedString("LatencyComponentCategorizerRouting", "Ex2ACB31", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentOriginalMailDsn
		{
			get
			{
				return new LocalizedString("LatencyComponentOriginalMailDsn", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentPickup
		{
			get
			{
				return new LocalizedString("LatencyComponentPickup", "Ex666A76", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LowRisk
		{
			get
			{
				return new LocalizedString("LowRisk", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentDeliveryAgentOnOpenConnection
		{
			get
			{
				return new LocalizedString("LatencyComponentDeliveryAgentOnOpenConnection", "Ex2B96CC", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfigurationLoaderFailed(string componentName)
		{
			return new LocalizedString("ConfigurationLoaderFailed", "", false, false, Strings.ResourceManager, new object[]
			{
				componentName
			});
		}

		public static LocalizedString LatencyComponentCategorizerOnRoutedMessage
		{
			get
			{
				return new LocalizedString("LatencyComponentCategorizerOnRoutedMessage", "Ex0FC763", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CategorizerConfigValidationFailed
		{
			get
			{
				return new LocalizedString("CategorizerConfigValidationFailed", "ExD59195", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentNone
		{
			get
			{
				return new LocalizedString("LatencyComponentNone", "Ex3E6FB9", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentDeliveryQueueLocking
		{
			get
			{
				return new LocalizedString("LatencyComponentDeliveryQueueLocking", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionAD
		{
			get
			{
				return new LocalizedString("LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionAD", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageResubmissionComponentBanner
		{
			get
			{
				return new LocalizedString("MessageResubmissionComponentBanner", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TotalExcludingPriorityNone
		{
			get
			{
				return new LocalizedString("TotalExcludingPriorityNone", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CloneMoveSourceNull
		{
			get
			{
				return new LocalizedString("CloneMoveSourceNull", "Ex1A2B8A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionSmtp
		{
			get
			{
				return new LocalizedString("LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionSmtp", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentAgent
		{
			get
			{
				return new LocalizedString("LatencyComponentAgent", "Ex985392", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboundMailSubmissionFromHubsComponent
		{
			get
			{
				return new LocalizedString("InboundMailSubmissionFromHubsComponent", "ExFBCBCC", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlreadyJoined
		{
			get
			{
				return new LocalizedString("AlreadyJoined", "ExA45BBD", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentStoreDriverSubmissionRpc
		{
			get
			{
				return new LocalizedString("LatencyComponentStoreDriverSubmissionRpc", "ExA6DEA7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NormalAndLowRisk
		{
			get
			{
				return new LocalizedString("NormalAndLowRisk", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentMailboxTransportSubmissionStoreDriverSubmission
		{
			get
			{
				return new LocalizedString("LatencyComponentMailboxTransportSubmissionStoreDriverSubmission", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRoutingOverride
		{
			get
			{
				return new LocalizedString("InvalidRoutingOverride", "Ex451377", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentSmtpReceiveOnDataCommand
		{
			get
			{
				return new LocalizedString("LatencyComponentSmtpReceiveOnDataCommand", "Ex9FD61C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LowRiskNormalPriority
		{
			get
			{
				return new LocalizedString("LowRiskNormalPriority", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AalCalClassificationDisplayName
		{
			get
			{
				return new LocalizedString("AalCalClassificationDisplayName", "Ex9AD315", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HighlyConfidential
		{
			get
			{
				return new LocalizedString("HighlyConfidential", "Ex670E75", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentStoreDriverOnInitializedMessage
		{
			get
			{
				return new LocalizedString("LatencyComponentStoreDriverOnInitializedMessage", "Ex5CD5C2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentStoreDriverDeliveryStore
		{
			get
			{
				return new LocalizedString("LatencyComponentStoreDriverDeliveryStore", "Ex2EDF67", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRowState
		{
			get
			{
				return new LocalizedString("InvalidRowState", "Ex83E4B9", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentMailboxTransportSubmissionService
		{
			get
			{
				return new LocalizedString("LatencyComponentMailboxTransportSubmissionService", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentStoreDriverSubmissionAD
		{
			get
			{
				return new LocalizedString("LatencyComponentStoreDriverSubmissionAD", "Ex737FC0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DumpsterJobStatusProcessing
		{
			get
			{
				return new LocalizedString("DumpsterJobStatusProcessing", "Ex1D5F76", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ResourcesInAboveNormalPressure(string resources)
		{
			return new LocalizedString("ResourcesInAboveNormalPressure", "ExD8F560", false, true, Strings.ResourceManager, new object[]
			{
				resources
			});
		}

		public static LocalizedString ComponentsDisabledByBackPressure(string componentNames)
		{
			return new LocalizedString("ComponentsDisabledByBackPressure", "Ex099F3B", false, true, Strings.ResourceManager, new object[]
			{
				componentNames
			});
		}

		public static LocalizedString RoutingNoLocalRgObject
		{
			get
			{
				return new LocalizedString("RoutingNoLocalRgObject", "Ex7F036B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PendingTransactions
		{
			get
			{
				return new LocalizedString("PendingTransactions", "Ex8507C2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrailingEscape
		{
			get
			{
				return new LocalizedString("TrailingEscape", "ExDFC953", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CloneMoveTargetNotNew
		{
			get
			{
				return new LocalizedString("CloneMoveTargetNotNew", "Ex79B83C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentCategorizerResolver
		{
			get
			{
				return new LocalizedString("LatencyComponentCategorizerResolver", "Ex0AA974", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidColumn(string table, int index)
		{
			return new LocalizedString("InvalidColumn", "ExF9F0C7", false, true, Strings.ResourceManager, new object[]
			{
				table,
				index
			});
		}

		public static LocalizedString SubmissionQueueUses(int pressure, string uses, int normal, int medium, int high)
		{
			return new LocalizedString("SubmissionQueueUses", "Ex757B0C", false, true, Strings.ResourceManager, new object[]
			{
				pressure,
				uses,
				normal,
				medium,
				high
			});
		}

		public static LocalizedString TransportComponentLoadFailed
		{
			get
			{
				return new LocalizedString("TransportComponentLoadFailed", "Ex9C6F9A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentProcessingScheduler
		{
			get
			{
				return new LocalizedString("LatencyComponentProcessingScheduler", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentSmtpSendConnect
		{
			get
			{
				return new LocalizedString("LatencyComponentSmtpSendConnect", "ExB63E7B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Minimum
		{
			get
			{
				return new LocalizedString("Minimum", "Ex6C6620", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboundMailSubmissionFromPickupDirectoryComponent
		{
			get
			{
				return new LocalizedString("InboundMailSubmissionFromPickupDirectoryComponent", "Ex52059E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BreadCrumbSize
		{
			get
			{
				return new LocalizedString("BreadCrumbSize", "ExE83A07", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CloneMoveSourceNotSaved
		{
			get
			{
				return new LocalizedString("CloneMoveSourceNotSaved", "Ex0910C2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentShadowQueue
		{
			get
			{
				return new LocalizedString("LatencyComponentShadowQueue", "ExDA7A0F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoutingIdenticalFqdns(string server1, string server2)
		{
			return new LocalizedString("RoutingIdenticalFqdns", "ExF53547", false, true, Strings.ResourceManager, new object[]
			{
				server1,
				server2
			});
		}

		public static LocalizedString InvalidCursorState
		{
			get
			{
				return new LocalizedString("InvalidCursorState", "ExECFD0B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentUnderThreshold
		{
			get
			{
				return new LocalizedString("LatencyComponentUnderThreshold", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HighPriority
		{
			get
			{
				return new LocalizedString("HighPriority", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CircularClone
		{
			get
			{
				return new LocalizedString("CircularClone", "Ex3C545C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDeleteState
		{
			get
			{
				return new LocalizedString("InvalidDeleteState", "Ex1D9051", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentExternalPartnerServers
		{
			get
			{
				return new LocalizedString("LatencyComponentExternalPartnerServers", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxProxySendConnector
		{
			get
			{
				return new LocalizedString("MailboxProxySendConnector", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalDestinationOutboundProxySendConnector
		{
			get
			{
				return new LocalizedString("ExternalDestinationOutboundProxySendConnector", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotEnoughRightsToReEncrypt(int rights)
		{
			return new LocalizedString("NotEnoughRightsToReEncrypt", "Ex12BA80", false, true, Strings.ResourceManager, new object[]
			{
				rights
			});
		}

		public static LocalizedString InvalidTransportServerRole
		{
			get
			{
				return new LocalizedString("InvalidTransportServerRole", "Ex32892A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncorrectColumn
		{
			get
			{
				return new LocalizedString("IncorrectColumn", "ExCA9176", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CountWrong
		{
			get
			{
				return new LocalizedString("CountWrong", "Ex463102", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentRmsAcquirePreLicense
		{
			get
			{
				return new LocalizedString("LatencyComponentRmsAcquirePreLicense", "Ex99F176", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SecureMailOuterLayerMustBeSigned
		{
			get
			{
				return new LocalizedString("SecureMailOuterLayerMustBeSigned", "Ex40FCCE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentMailboxMove
		{
			get
			{
				return new LocalizedString("LatencyComponentMailboxMove", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VersionBucketUses(int pressure, string uses, int normal, int medium, int high)
		{
			return new LocalizedString("VersionBucketUses", "Ex12A0D5", false, true, Strings.ResourceManager, new object[]
			{
				pressure,
				uses,
				normal,
				medium,
				high
			});
		}

		public static LocalizedString InternalDestinationOutboundProxySendConnector
		{
			get
			{
				return new LocalizedString("InternalDestinationOutboundProxySendConnector", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncorrectBrace
		{
			get
			{
				return new LocalizedString("IncorrectBrace", "Ex51FA89", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KeyLength
		{
			get
			{
				return new LocalizedString("KeyLength", "ExAF3CC0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRank
		{
			get
			{
				return new LocalizedString("InvalidRank", "Ex6A2384", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventAgentComponent(string eventName, string agentName)
		{
			return new LocalizedString("EventAgentComponent", "ExCF38A9", false, true, Strings.ResourceManager, new object[]
			{
				eventName,
				agentName
			});
		}

		public static LocalizedString MimeWriteStreamOpen
		{
			get
			{
				return new LocalizedString("MimeWriteStreamOpen", "ExCA0E3B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseDeleted(string databasePath)
		{
			return new LocalizedString("DatabaseDeleted", "", false, false, Strings.ResourceManager, new object[]
			{
				databasePath
			});
		}

		public static LocalizedString AalCalBanner(string aal, string cal, string mechanism)
		{
			return new LocalizedString("AalCalBanner", "Ex8BB6D1", false, true, Strings.ResourceManager, new object[]
			{
				aal,
				cal,
				mechanism
			});
		}

		public static LocalizedString StreamStateInvalid
		{
			get
			{
				return new LocalizedString("StreamStateInvalid", "Ex3391A3", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NormalRiskLowPriority
		{
			get
			{
				return new LocalizedString("NormalRiskLowPriority", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboundMailSubmissionFromInternetComponent
		{
			get
			{
				return new LocalizedString("InboundMailSubmissionFromInternetComponent", "ExB43ECA", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatencyComponentCategorizerOnResolvedMessage
		{
			get
			{
				return new LocalizedString("LatencyComponentCategorizerOnResolvedMessage", "Ex1AC891", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BodyFormatUnsupported
		{
			get
			{
				return new LocalizedString("BodyFormatUnsupported", "Ex8D1105", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(277);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Transport.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			RoutingNoAdSites = 3699713858U,
			LatencyComponentHeartbeat = 101432279U,
			ComponentsDisabledNone = 677442004U,
			LatencyComponentRmsRequestDelegationToken = 680722199U,
			LatencyComponentRmsAcquireServerBoxRac = 2008205339U,
			LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionStoreStats = 393272335U,
			DatabaseOpen = 642728361U,
			ColumnName = 2221898189U,
			ShadowSendConnector = 28104421U,
			LatencyComponentDeliveryQueueMailboxInsufficientResources = 781896450U,
			Restricted = 897286993U,
			LatencyComponentStoreDriverDeliveryContentConversion = 3327987659U,
			IntraorgSendConnectorName = 3070454738U,
			LatencyComponentSmtpReceiveCommitLocal = 2741991560U,
			LatencyComponentDeliveryQueueMailbox = 2269890262U,
			LatencyComponentDeliveryAgent = 366323840U,
			LatencyComponentRmsFindServiceLocation = 2455354172U,
			NormalPriority = 229786213U,
			LatencyComponentSmtpReceiveOnEndOfHeaders = 2359831369U,
			Confidential = 3475865632U,
			NormalRisk = 3022513318U,
			ShadowRedundancyNoActiveServerInNexthopSolution = 2222703033U,
			LatencyComponentStoreDriverOnCompletedMessage = 1699301985U,
			HighRisk = 130958697U,
			DatabaseRecoveryActionDelete = 1048481989U,
			LatencyComponentSmtpReceiveOnProxyInboundMessage = 1785499833U,
			LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionSmtpOut = 3547958413U,
			LatencyComponentSmtpSend = 4212340469U,
			MediumResourceUses = 3759339643U,
			LatencyComponentSmtpReceiveCommitRemote = 3152927583U,
			LatencyComponentCategorizer = 4145912840U,
			None = 1414246128U,
			LatencyComponentDumpster = 2123701631U,
			EnumeratorBadPosition = 2393050832U,
			LatencyComponentRmsAcquireCertificationMexData = 1226316175U,
			ContentAggregationComponent = 2892365298U,
			DiscardingDataFalse = 1244889643U,
			DatabaseClosed = 1050903407U,
			FailedToReadServerRole = 2048777343U,
			LatencyComponentDeliveryQueueMailboxDynamicMailboxDatabaseThrottlingLimitExceeded = 821664177U,
			LatencyComponentMailSubmissionServiceNotify = 2000842882U,
			LatencyComponentStoreDriverSubmissionStore = 217112309U,
			SeekFailed = 3442182595U,
			LatencyComponentMailSubmissionServiceNotifyRetrySchedule = 1243242609U,
			LatencyComponentStoreDriverOnDemotedMessage = 1618072168U,
			Public = 683438393U,
			AcceptedDomainTableNotLoaded = 1815474557U,
			LatencyComponentServiceRestart = 1747692937U,
			NormalRiskNonePriority = 525138956U,
			LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionContentConversion = 4093059930U,
			LatencyComponentDeliveryQueueMailboxMapiExceptionTimeout = 526223919U,
			LatencyComponentSmtpSendMailboxDelivery = 177467553U,
			LatencyComponentDeliveryQueueExternal = 680901367U,
			HighAndBulkRisk = 464945964U,
			JetOperationFailure = 850634908U,
			LatencyComponentSmtpReceive = 4222452556U,
			LatencyComponentSmtpReceiveDataExternal = 2713736967U,
			LatencyComponentStoreDriverDeliveryAD = 2517616835U,
			LatencyComponentDeliveryQueueMailboxDeliverAgentTransientFailure = 3066388364U,
			LatencyComponentCategorizerFinal = 2895510700U,
			BulkRisk = 412845223U,
			LatencyComponentSubmissionQueue = 2314636918U,
			LatencyComponentStoreDriverSubmit = 871184586U,
			DumpsterJobStatusQueued = 1421458560U,
			LatencyComponentStoreDriverDeliveryMailboxDatabaseThrottling = 320885804U,
			LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionOnDemotedMessage = 939699173U,
			SecureMailInvalidNumberOfLayers = 3432225769U,
			LowRiskLowPriority = 3242396317U,
			LatencyComponentMailSubmissionServiceFailedAttempt = 1951232521U,
			RemoteDomainTableNotLoaded = 4208962580U,
			OutboundMailDeliveryToRemoteDomainsComponent = 1681384042U,
			AttachmentReadFailed = 4095319950U,
			LatencyComponentMailboxRules = 2378292844U,
			LowResourceUses = 3395591492U,
			LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionStoreDisposeSession = 1151336747U,
			High = 4217035038U,
			IdentityParameterNotFound = 198598092U,
			NotOpenForWrite = 1049847567U,
			TcpListenerError = 4063125577U,
			LatencyComponentMexRuntimeThreadpoolQueue = 429833158U,
			LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionPerfContextLdap = 1306604228U,
			NormalResourceUses = 1836041741U,
			DumpsterJobStatusCreated = 364271663U,
			LowPriority = 3603045308U,
			ExternalDestinationInboundProxySendConnector = 2295242475U,
			IPFilterDatabaseInstanceName = 1092559024U,
			ActivationFailed = 3889172535U,
			LatencyComponentDelivery = 2198647971U,
			AggregateResource = 3545397251U,
			LatencyComponentProcessingSchedulerScoped = 1807713273U,
			LatencyComponentSmtpReceiveOnRcpt2Command = 1437992463U,
			LatencyComponentStoreDriverOnPromotedMessage = 68652566U,
			PoisonMessageRegistryAccessFailed = 1004476481U,
			HighResourceUses = 4020652394U,
			EnvelopRecipientDisposed = 3706269143U,
			SystemMemory = 2556558528U,
			ReadOrgContainerFailed = 184239278U,
			LatencyComponentStoreDriverOnCreatedMessage = 488909308U,
			CategorizerMaxConfigLoadRetriesReached = 3548219859U,
			LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionStoreOpenSession = 1542680578U,
			RoutingLocalServerIsNotBridgehead = 1797311582U,
			LatencyComponentCategorizerOnCategorizedMessage = 1030204127U,
			LatencyComponentRmsAcquireB2BRac = 727067537U,
			InternalDestinationInboundProxySendConnector = 2163339405U,
			LatencyComponentDeliveryQueueMailboxMaxConcurrentMessageSizeLimitExceeded = 1080294189U,
			LatencyComponentStoreDriverDeliveryRpc = 170486207U,
			ConnectionInUse = 1602038336U,
			LatencyComponentMailSubmissionService = 4260841639U,
			NonePriority = 4278195556U,
			LatencyComponentRmsAcquireTemplateInfo = 3768774525U,
			LatencyComponentContentAggregation = 3451284522U,
			LatencyComponentMailSubmissionServiceShadowResubmitDecision = 2443567820U,
			LatencyComponentContentAggregationMailItemCommit = 3636715311U,
			DatabaseRecoveryActionMove = 1674978893U,
			ReadTransportServerConfigFailed = 3640623549U,
			LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionHubSelector = 4082435799U,
			MessageTrackingConfigNotFound = 1054523613U,
			LatencyComponentDeliveryQueueInternal = 16119773U,
			LatencyComponentDeliveryQueueMailboxMailboxServerOffline = 2383564204U,
			PrivateBytesResource = 3307097748U,
			LatencyComponentDeliveryQueueMailboxMailboxDatabaseOffline = 4156740374U,
			RoutingMaxConfigLoadRetriesReached = 1447270652U,
			LatencyComponentRmsAcquireServerLicensingMexData = 1851026156U,
			LatencyComponentDeliveryQueueMailboxRecipientThreadLimitExceeded = 2614121165U,
			SmtpReceiveParserNegativeBytes = 1010194958U,
			InvalidRoleChange = 702550931U,
			LatencyComponentPoisonQueue = 3077939474U,
			BootScannerComponent = 3953400951U,
			NotInTransaction = 1434651324U,
			LatencyComponentSmtpReceiveOnRcptCommand = 3231014581U,
			BodyReadFailed = 2675789057U,
			TextConvertersFailed = 1355928173U,
			RoutingNoRoutingGroups = 604013917U,
			LatencyComponentStoreDriverDeliveryMessageConcurrency = 1040763200U,
			Submission = 2064442254U,
			ReadingADConfigFailed = 1817505332U,
			LatencyComponentTooManyComponents = 4179070144U,
			ReadTransportConfigConfigFailed = 531266226U,
			InvalidMessageResubmissionState = 3659171826U,
			LatencyComponentSmtpReceiveOnEndOfData = 2339552741U,
			InvalidTransportRole = 551877430U,
			LatencyComponentReplay = 2370750848U,
			CloneMoveDestination = 3447320822U,
			LowRiskNonePriority = 3132079455U,
			LatencyComponentCategorizerBifurcation = 3933849364U,
			SchemaInvalid = 3849329560U,
			QuoteNestLevel = 2223967066U,
			LatencyComponentUnknown = 1966949091U,
			LatencyComponentTotal = 1884563717U,
			LatencyComponentRmsAcquireLicense = 2229981216U,
			InboundMailSubmissionFromReplayDirectoryComponent = 1359462263U,
			NoColumns = 3952286128U,
			LatencyComponentRmsAcquireClc = 3021848955U,
			CloneMoveComplete = 1362050859U,
			LatencyComponentQuarantineReleaseOrReport = 1615937519U,
			LatencyComponentSubmissionAssistant = 662654939U,
			DumpsterJobResponseSuccess = 529499203U,
			AgentComponentFailed = 1767865225U,
			ClientProxySendConnector = 2584904976U,
			Basic = 4128944152U,
			LatencyComponentCategorizerOnSubmittedMessage = 2705384945U,
			NormalRiskNormalPriority = 3565174541U,
			LatencyComponentSmtpReceiveCommit = 1411027439U,
			RoutingNoLocalServer = 3882493397U,
			SecureMailSecondLayerMustBeEnveloped = 3622146271U,
			MailItemDeferred = 3840724219U,
			InboundMailSubmissionFromMailboxComponent = 3706643741U,
			AttachmentProtectionFailed = 56765413U,
			LatencyComponentSubmissionAssistantThrottling = 4115715614U,
			LatencyComponentCategorizerLocking = 1586175673U,
			ValueNull = 955728834U,
			TooManyAgents = 2197880345U,
			DumpsterJobStatusCompleted = 3324883850U,
			LatencyComponentStoreDriverOnDeliveredMessage = 2127171860U,
			LatencyComponentRmsAcquireB2BLicense = 1858725880U,
			LatencyComponentDeliveryQueueMailboxMapiExceptionLockViolation = 2602495526U,
			Medium = 1946647341U,
			NotBufferedStream = 1261670220U,
			IncorrectBaseStream = 2888586632U,
			LatencyComponentProcess = 26231820U,
			CommitMailFailed = 3133088287U,
			NonAsciiData = 1974402452U,
			SeekBarred = 2736623442U,
			RoutingLocalRgNotSet = 3819242299U,
			MessagingDatabaseInstanceName = 262955557U,
			LatencyComponentUnreachableQueue = 2000471546U,
			LatencyComponentStoreDriverDelivery = 1113830170U,
			DatabaseStillInUse = 1062938671U,
			LatencyComponentDeferral = 680830688U,
			DumpsterJobResponseRetryLater = 793374748U,
			RoutingNoLocalAdSite = 1287594420U,
			LatencyComponentRmsAcquireTemplates = 3350471676U,
			InvalidRoutingOverrideEvent = 3717583665U,
			GetSclThresholdDefaultValueOutOfRange = 1384341089U,
			LatencyComponentSmtpReceiveDataInternal = 3449384489U,
			InvalidTenantLicensePair = 2279316324U,
			ColumnIndexesMustBeSequential = 3889416851U,
			LatencyComponentNonSmtpGateway = 3854009776U,
			LatencyComponentDeliveryAgentOnDeliverMailItem = 2176966386U,
			ShadowRedundancyComponentBanner = 2711829446U,
			CloneMoveSourceModified = 350383284U,
			DatabaseRecoveryActionNone = 1674978920U,
			LatencyComponentMailSubmissionServiceThrottling = 1142385700U,
			LatencyComponentDsnGenerator = 1673427229U,
			RowDeleted = 2385069791U,
			LatencyComponentCategorizerContentConversion = 4154552087U,
			LatencyComponentExternalServers = 3393444732U,
			ReadMicrosoftExchangeRecipientFailed = 3672571637U,
			SeekGeneralFailure = 3993229380U,
			LatencyComponentCategorizerRouting = 3321191670U,
			LatencyComponentOriginalMailDsn = 1692037724U,
			LatencyComponentPickup = 1471624947U,
			LowRisk = 615004755U,
			LatencyComponentDeliveryAgentOnOpenConnection = 2919439981U,
			LatencyComponentCategorizerOnRoutedMessage = 2349744297U,
			CategorizerConfigValidationFailed = 1601350579U,
			LatencyComponentNone = 2863890221U,
			LatencyComponentDeliveryQueueLocking = 2071866321U,
			LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionAD = 2768133224U,
			MessageResubmissionComponentBanner = 3862731819U,
			TotalExcludingPriorityNone = 3955727133U,
			CloneMoveSourceNull = 3906053774U,
			LatencyComponentMailboxTransportSubmissionStoreDriverSubmissionSmtp = 92299947U,
			LatencyComponentAgent = 3127930220U,
			InboundMailSubmissionFromHubsComponent = 650877237U,
			AlreadyJoined = 3875073169U,
			LatencyComponentStoreDriverSubmissionRpc = 3994617651U,
			NormalAndLowRisk = 2896583167U,
			LatencyComponentMailboxTransportSubmissionStoreDriverSubmission = 175131561U,
			InvalidRoutingOverride = 335920517U,
			LatencyComponentSmtpReceiveOnDataCommand = 3689512814U,
			LowRiskNormalPriority = 4114179756U,
			AalCalClassificationDisplayName = 3171622157U,
			HighlyConfidential = 1744966745U,
			LatencyComponentStoreDriverOnInitializedMessage = 2327669046U,
			LatencyComponentStoreDriverDeliveryStore = 4016006905U,
			InvalidRowState = 3611628552U,
			LatencyComponentMailboxTransportSubmissionService = 3334507377U,
			LatencyComponentStoreDriverSubmissionAD = 3514558095U,
			DumpsterJobStatusProcessing = 2595056666U,
			RoutingNoLocalRgObject = 2912766470U,
			PendingTransactions = 2117972488U,
			TrailingEscape = 3750964199U,
			CloneMoveTargetNotNew = 1679638210U,
			LatencyComponentCategorizerResolver = 2607217314U,
			TransportComponentLoadFailed = 99902445U,
			LatencyComponentProcessingScheduler = 2986853275U,
			LatencyComponentSmtpSendConnect = 2104269935U,
			Minimum = 936688982U,
			InboundMailSubmissionFromPickupDirectoryComponent = 1677063078U,
			BreadCrumbSize = 1209655894U,
			CloneMoveSourceNotSaved = 885455923U,
			LatencyComponentShadowQueue = 3063068326U,
			InvalidCursorState = 3504101496U,
			LatencyComponentUnderThreshold = 3589283362U,
			HighPriority = 3124262306U,
			CircularClone = 1180878696U,
			InvalidDeleteState = 3959631871U,
			LatencyComponentExternalPartnerServers = 642210074U,
			MailboxProxySendConnector = 1582326851U,
			ExternalDestinationOutboundProxySendConnector = 3238954214U,
			InvalidTransportServerRole = 2588281919U,
			IncorrectColumn = 3502104427U,
			CountWrong = 2368056258U,
			LatencyComponentRmsAcquirePreLicense = 1954095043U,
			SecureMailOuterLayerMustBeSigned = 3452310564U,
			LatencyComponentMailboxMove = 3835436216U,
			InternalDestinationOutboundProxySendConnector = 1450242468U,
			IncorrectBrace = 3494923132U,
			KeyLength = 309843435U,
			InvalidRank = 1227018995U,
			MimeWriteStreamOpen = 3974043093U,
			StreamStateInvalid = 1578443098U,
			NormalRiskLowPriority = 2070671180U,
			InboundMailSubmissionFromInternetComponent = 2721873196U,
			LatencyComponentCategorizerOnResolvedMessage = 3160113472U,
			BodyFormatUnsupported = 4049537308U
		}

		private enum ParamIDs
		{
			PhysicalMemoryUses,
			DatabaseAttachFailed,
			InvalidCharset,
			ResourcesInNormalPressure,
			ResourceUses,
			DatabaseAndDatabaseLogDeleted,
			RoutingDecryptConnectorPasswordFailure,
			DatabaseLoggingResource,
			TlsCertificateNameNotFound,
			ColumnsMustBeOrdered,
			RoutingIdenticalExchangeLegacyDns,
			DataBaseError,
			DuplicateColumnIndexes,
			FilePathOnLockedVolume,
			BitlockerQueryFailed,
			InvalidSmtpAddress,
			RMSTemplateNotFound,
			VersionBuckets,
			CannotModifyCompletedRequest,
			DefaultAuthoritativeDomainNotFound,
			TemporaryStorageResource,
			SchemaRequiredColumnNotFound,
			QueueLength,
			DatabaseIsNotMovable,
			ExtractNotAllowed,
			CannotRemoveRequestInRunningState,
			DatabaseAndDatabaseLogMoved,
			DiskFull,
			DatabaseResource,
			DatabaseSchemaNotSupported,
			ColumnAccessInvalid,
			DatabaseMoved,
			IndexOutOfBounds,
			OutboundConnectorNotFound,
			SchemaVersion,
			TransportComponentLoadFailedWithName,
			LatencyComponentMExReserved,
			ValueIsTooLarge,
			DataBaseInUse,
			SchemaTypeMismatch,
			UsedDiskSpaceResource,
			ConfigurationLoaderFailed,
			ResourcesInAboveNormalPressure,
			ComponentsDisabledByBackPressure,
			InvalidColumn,
			SubmissionQueueUses,
			RoutingIdenticalFqdns,
			NotEnoughRightsToReEncrypt,
			VersionBucketUses,
			EventAgentComponent,
			DatabaseDeleted,
			AalCalBanner
		}
	}
}
