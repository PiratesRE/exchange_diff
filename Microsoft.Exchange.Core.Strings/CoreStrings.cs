using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core
{
	public static class CoreStrings
	{
		static CoreStrings()
		{
			CoreStrings.stringIDs.Add(2092707270U, "RoleTypeDescription_RemoteAndAcceptedDomains");
			CoreStrings.stringIDs.Add(1886101956U, "TimeZoneSouthAfricaStandardTime");
			CoreStrings.stringIDs.Add(3906704818U, "PolicyTipNotifyOnly");
			CoreStrings.stringIDs.Add(97673562U, "TimeZoneLibyaStandardTime");
			CoreStrings.stringIDs.Add(1829747073U, "ReportSeverityLow");
			CoreStrings.stringIDs.Add(2344870457U, "RoleTypeDescription_ArchiveApplication");
			CoreStrings.stringIDs.Add(3472506566U, "EventDelivered");
			CoreStrings.stringIDs.Add(3720562738U, "AntispamIPBlockListSettingMigrated");
			CoreStrings.stringIDs.Add(327230581U, "RoleTypeDescription_ActiveMonitoring");
			CoreStrings.stringIDs.Add(2962389050U, "RoleTypeDescription_UMPrompts");
			CoreStrings.stringIDs.Add(146865784U, "TimeZoneCentralAsiaStandardTime");
			CoreStrings.stringIDs.Add(146778149U, "TimeZoneESouthAmericaStandardTime");
			CoreStrings.stringIDs.Add(80585286U, "TimeZoneMountainStandardTime");
			CoreStrings.stringIDs.Add(1446960263U, "TimeZoneKaliningradStandardTime");
			CoreStrings.stringIDs.Add(137362532U, "ExchangeViewOnlyManagementForestOperatorDescription");
			CoreStrings.stringIDs.Add(3151979146U, "AntispamMigrationSucceeded");
			CoreStrings.stringIDs.Add(1683344478U, "TimeZoneNorthAsiaStandardTime");
			CoreStrings.stringIDs.Add(89789448U, "InvalidSender");
			CoreStrings.stringIDs.Add(1221420638U, "RoleTypeDescription_MyTeamMailboxes");
			CoreStrings.stringIDs.Add(2027768067U, "RoleTypeDescription_MyDistributionGroups");
			CoreStrings.stringIDs.Add(1738495689U, "TimeZoneFijiStandardTime");
			CoreStrings.stringIDs.Add(1010967094U, "TimeZoneEAfricaStandardTime");
			CoreStrings.stringIDs.Add(498311706U, "NotAuthorizedToViewRecipientPath");
			CoreStrings.stringIDs.Add(2847306432U, "RoleTypeDescription_UserOptions");
			CoreStrings.stringIDs.Add(3322063993U, "RoleTypeDescription_UmRecipientManagement");
			CoreStrings.stringIDs.Add(1029644363U, "ReportSeverityHigh");
			CoreStrings.stringIDs.Add(2763281369U, "RoleTypeDescription_CmdletExtensionAgents");
			CoreStrings.stringIDs.Add(2874223383U, "AntimalwareUserOptOut");
			CoreStrings.stringIDs.Add(2736802282U, "RoleTypeDescription_MyLogon");
			CoreStrings.stringIDs.Add(2385442291U, "ContentIndexStatusDisabled");
			CoreStrings.stringIDs.Add(466036006U, "PolicyTipRejectOverride");
			CoreStrings.stringIDs.Add(1875295180U, "StatusDelivered");
			CoreStrings.stringIDs.Add(1255891408U, "TimeZoneWMongoliaStandardTime");
			CoreStrings.stringIDs.Add(872336955U, "RoleTypeDescription_TransportHygiene");
			CoreStrings.stringIDs.Add(112955817U, "TraceLevelHigh");
			CoreStrings.stringIDs.Add(4100231581U, "RoleTypeDescription_InformationRightsManagement");
			CoreStrings.stringIDs.Add(1461929024U, "TestModifySubject");
			CoreStrings.stringIDs.Add(2652353769U, "TimeZoneLordHoweStandardTime");
			CoreStrings.stringIDs.Add(1319959286U, "InvalidSenderForAdmin");
			CoreStrings.stringIDs.Add(1633693746U, "RoleTypeDescription_DiscoveryManagement");
			CoreStrings.stringIDs.Add(3964643789U, "TimeZoneTurksAndCaicosStandardTime");
			CoreStrings.stringIDs.Add(2230053377U, "RoleTypeDescription_MailboxExport");
			CoreStrings.stringIDs.Add(3106057138U, "AntispamIPAllowListSettingMigrated");
			CoreStrings.stringIDs.Add(4036159751U, "ExchangeManagementForestOperatorDescription");
			CoreStrings.stringIDs.Add(2575879303U, "TimeZoneArabianStandardTime");
			CoreStrings.stringIDs.Add(2009673553U, "TimeZoneIsraelStandardTime");
			CoreStrings.stringIDs.Add(4146612654U, "RoleTypeDescription_ActiveDirectoryPermissions");
			CoreStrings.stringIDs.Add(3378761982U, "RunOnce");
			CoreStrings.stringIDs.Add(4217676503U, "MessageDirectionAll");
			CoreStrings.stringIDs.Add(2281308004U, "RoleTypeDescription_UmManagement");
			CoreStrings.stringIDs.Add(2385887948U, "AuditSeverityLevelHigh");
			CoreStrings.stringIDs.Add(1680752826U, "TimeZoneArgentinaStandardTime");
			CoreStrings.stringIDs.Add(2184195641U, "RoleTypeDescription_Migration");
			CoreStrings.stringIDs.Add(1406395995U, "TimeZoneUlaanbaatarStandardTime");
			CoreStrings.stringIDs.Add(1799519309U, "TimeZoneSaratovStandardTime");
			CoreStrings.stringIDs.Add(1545851488U, "TimeZoneVenezuelaStandardTime");
			CoreStrings.stringIDs.Add(1103744372U, "ExchangeAllMailboxesDescription");
			CoreStrings.stringIDs.Add(926819582U, "RoleTypeDescription_GALSynchronizationManagement");
			CoreStrings.stringIDs.Add(3690262447U, "RoleTypeDescription_ViewOnlyRoleManagement");
			CoreStrings.stringIDs.Add(683655822U, "RoleTypeDescription_LiveID");
			CoreStrings.stringIDs.Add(3406878936U, "RoleTypeDescription_TransportAgents");
			CoreStrings.stringIDs.Add(2562309877U, "RoleTypeDescription_MyDiagnostics");
			CoreStrings.stringIDs.Add(411395545U, "TrackingWarningNoResultsDueToUntrackableMessagePath");
			CoreStrings.stringIDs.Add(4221322756U, "EventTransferredToLegacyExchangeServer");
			CoreStrings.stringIDs.Add(1998960776U, "RoleTypeDescription_TeamMailboxes");
			CoreStrings.stringIDs.Add(1130640660U, "TimeZoneSAPacificStandardTime");
			CoreStrings.stringIDs.Add(1318716879U, "ExchangeServerManagementDescription");
			CoreStrings.stringIDs.Add(74443346U, "UnsupportedSenderForTracking");
			CoreStrings.stringIDs.Add(2164669709U, "TimeZoneMontevideoStandardTime");
			CoreStrings.stringIDs.Add(2912446405U, "TimeZoneUSEasternStandardTime");
			CoreStrings.stringIDs.Add(517696069U, "EventApprovedModerationIW");
			CoreStrings.stringIDs.Add(103080662U, "TimeZoneCentralAmericaStandardTime");
			CoreStrings.stringIDs.Add(393808047U, "ContentIndexStatusAutoSuspended");
			CoreStrings.stringIDs.Add(2501174206U, "TimeZoneBahiaStandardTime");
			CoreStrings.stringIDs.Add(2005680638U, "RoleTypeDescription_EdgeSubscriptions");
			CoreStrings.stringIDs.Add(3905123624U, "TimeZoneCubaStandardTime");
			CoreStrings.stringIDs.Add(3492975378U, "TimeZoneEasterIslandStandardTime");
			CoreStrings.stringIDs.Add(3708464597U, "JobStatusInProgress");
			CoreStrings.stringIDs.Add(2294597162U, "TimeZoneUTC09");
			CoreStrings.stringIDs.Add(2993499200U, "TimeZoneEgyptStandardTime");
			CoreStrings.stringIDs.Add(3231667300U, "StatusRead");
			CoreStrings.stringIDs.Add(2740570033U, "TimeZoneCaucasusStandardTime");
			CoreStrings.stringIDs.Add(4120033037U, "RoleTypeDescription_MailEnabledPublicFolders");
			CoreStrings.stringIDs.Add(4113945646U, "RoleTypeDescription_ViewOnlyAuditLogs");
			CoreStrings.stringIDs.Add(576007077U, "TrackingWarningReadStatusUnavailable");
			CoreStrings.stringIDs.Add(176786758U, "ViewOnlyPIIGroupDescription");
			CoreStrings.stringIDs.Add(2073899161U, "RoleTypeDescription_FederatedSharing");
			CoreStrings.stringIDs.Add(3390444882U, "RoleTypeDescription_RoleManagement");
			CoreStrings.stringIDs.Add(2903872252U, "TrackingExplanationNormalTimeSpan");
			CoreStrings.stringIDs.Add(2937865572U, "RoleTypeDescription_ExchangeServers");
			CoreStrings.stringIDs.Add(641655538U, "TimeZoneAstrakhanStandardTime");
			CoreStrings.stringIDs.Add(840197340U, "ReportSeverityMedium");
			CoreStrings.stringIDs.Add(424676612U, "RoleTypeDescription_SupportDiagnostics");
			CoreStrings.stringIDs.Add(2220779553U, "TimeZoneCentralEuropeanStandardTime");
			CoreStrings.stringIDs.Add(1476061287U, "TimeZoneKoreaStandardTime");
			CoreStrings.stringIDs.Add(1361373610U, "ContentIndexStatusSeeding");
			CoreStrings.stringIDs.Add(3998935900U, "TimeZoneWAustraliaStandardTime");
			CoreStrings.stringIDs.Add(3209229526U, "RoleTypeDescription_Custom");
			CoreStrings.stringIDs.Add(390519148U, "AntispamActionTypeSettingMigrated");
			CoreStrings.stringIDs.Add(559475236U, "TrackingExplanationExcessiveTimeSpan");
			CoreStrings.stringIDs.Add(2416995053U, "QuarantineSpam");
			CoreStrings.stringIDs.Add(728513229U, "TimeZoneUTC11");
			CoreStrings.stringIDs.Add(1698995718U, "RoleTypeDescription_PublicFolderReplication");
			CoreStrings.stringIDs.Add(3202170171U, "Encrypt");
			CoreStrings.stringIDs.Add(1562661243U, "TimeZoneRussiaTimeZone11");
			CoreStrings.stringIDs.Add(875871474U, "TimeZoneEkaterinburgStandardTime");
			CoreStrings.stringIDs.Add(3860979609U, "RoleTypeDescription_LegalHold");
			CoreStrings.stringIDs.Add(50740234U, "TimeZoneTocantinsStandardTime");
			CoreStrings.stringIDs.Add(2428028299U, "TimeZoneArabicStandardTime");
			CoreStrings.stringIDs.Add(1011790132U, "RoleTypeDescription_MailboxImportExport");
			CoreStrings.stringIDs.Add(4065087132U, "RoleTypeDescription_Supervision");
			CoreStrings.stringIDs.Add(937823105U, "RoleTypeDescription_LawEnforcementRequests");
			CoreStrings.stringIDs.Add(851475441U, "RoleTypeDescription_MailboxSearchApplication");
			CoreStrings.stringIDs.Add(20620266U, "RoleTypeDescription_RetentionManagement");
			CoreStrings.stringIDs.Add(937651576U, "TimeZoneWestAsiaStandardTime");
			CoreStrings.stringIDs.Add(1727765283U, "QuarantineTransportRule");
			CoreStrings.stringIDs.Add(3421391191U, "RoleTypeDescription_ViewOnlyCentralAdminManagement");
			CoreStrings.stringIDs.Add(2151396155U, "EventFailedTransportRulesIW");
			CoreStrings.stringIDs.Add(199727676U, "EventDelayedAfterTransferToPartnerOrgIW");
			CoreStrings.stringIDs.Add(1348529911U, "RoleTypeDescription_DatacenterOperationsDCOnly");
			CoreStrings.stringIDs.Add(4089915379U, "TimeZoneTomskStandardTime");
			CoreStrings.stringIDs.Add(3099813970U, "TrackingBusy");
			CoreStrings.stringIDs.Add(1728005806U, "TimeZoneTongaStandardTime");
			CoreStrings.stringIDs.Add(3917786073U, "TimeZoneTasmaniaStandardTime");
			CoreStrings.stringIDs.Add(2471410929U, "ExchangePublicFolderAdminDescription");
			CoreStrings.stringIDs.Add(1018209439U, "TrafficScopeOutbound");
			CoreStrings.stringIDs.Add(2309256410U, "EventForwardedToDelegateAndDeleted");
			CoreStrings.stringIDs.Add(3851963997U, "RoleTypeDescription_GALSynchronization");
			CoreStrings.stringIDs.Add(1747500934U, "CompressionOutOfMemory");
			CoreStrings.stringIDs.Add(1069292955U, "TrafficScopeDisabled");
			CoreStrings.stringIDs.Add(1969466653U, "RoleTypeDescription_OrganizationManagement");
			CoreStrings.stringIDs.Add(1654105079U, "TimeZoneOmskStandardTime");
			CoreStrings.stringIDs.Add(1984558815U, "TimeZoneBelarusStandardTime");
			CoreStrings.stringIDs.Add(3386037373U, "TimeZoneParaguayStandardTime");
			CoreStrings.stringIDs.Add(1198640795U, "RoleTypeDescription_Reporting");
			CoreStrings.stringIDs.Add(3103556173U, "TimeZoneChathamIslandsStandardTime");
			CoreStrings.stringIDs.Add(1660783915U, "RoleTypeDescription_MyMailboxDelegation");
			CoreStrings.stringIDs.Add(397099290U, "RoleTypeDescription_ExchangeVirtualDirectories");
			CoreStrings.stringIDs.Add(3213863256U, "TimeZoneAUSEasternStandardTime");
			CoreStrings.stringIDs.Add(3256513739U, "EventNotRead");
			CoreStrings.stringIDs.Add(463534759U, "TimeZoneMiddleEastStandardTime");
			CoreStrings.stringIDs.Add(907728851U, "RoleTypeDescription_ApplicationImpersonation");
			CoreStrings.stringIDs.Add(2979872069U, "TrackingWarningTooManyEvents");
			CoreStrings.stringIDs.Add(1631091055U, "ContentIndexStatusUnknown");
			CoreStrings.stringIDs.Add(1826684897U, "TimeZoneSyriaStandardTime");
			CoreStrings.stringIDs.Add(3804745356U, "TimeZoneMauritiusStandardTime");
			CoreStrings.stringIDs.Add(667589187U, "TrackingMessageTypeNotSupported");
			CoreStrings.stringIDs.Add(3096621845U, "TimeZoneCentralPacificStandardTime");
			CoreStrings.stringIDs.Add(1156915939U, "RoleTypeDescription_MailboxSearch");
			CoreStrings.stringIDs.Add(1027560048U, "StdUnknownTimeZone");
			CoreStrings.stringIDs.Add(3485911895U, "StatusUnsuccessFul");
			CoreStrings.stringIDs.Add(1187302658U, "RoleTypeDescription_MyLinkedInEnabled");
			CoreStrings.stringIDs.Add(4008183169U, "TestXHeader");
			CoreStrings.stringIDs.Add(1479842702U, "RoleTypeDescription_ReceiveConnectors");
			CoreStrings.stringIDs.Add(1338433320U, "TimeZoneRussiaTimeZone3");
			CoreStrings.stringIDs.Add(2262407331U, "TimeZoneTransbaikalStandardTime");
			CoreStrings.stringIDs.Add(2629692075U, "RoleTypeDescription_Databases");
			CoreStrings.stringIDs.Add(14056478U, "StatusTransferred");
			CoreStrings.stringIDs.Add(2237907931U, "TimeZoneGeorgianStandardTime");
			CoreStrings.stringIDs.Add(3620221593U, "Decrypt");
			CoreStrings.stringIDs.Add(3879259618U, "RoleTypeDescription_MyReadWriteMailboxApps");
			CoreStrings.stringIDs.Add(3742415118U, "TimeZoneBougainvilleStandardTime");
			CoreStrings.stringIDs.Add(99561377U, "RoleTypeDescription_EdgeSync");
			CoreStrings.stringIDs.Add(2333794793U, "TimeZoneTurkeyStandardTime");
			CoreStrings.stringIDs.Add(3305616866U, "RoleTypeDescription_MyMailSubscriptions");
			CoreStrings.stringIDs.Add(1393844023U, "PartialMessages");
			CoreStrings.stringIDs.Add(3351363629U, "InboundIpMigrationCompleted");
			CoreStrings.stringIDs.Add(626810642U, "RoleTypeDescription_MyMarketplaceApps");
			CoreStrings.stringIDs.Add(448910837U, "TimeZoneJordanStandardTime");
			CoreStrings.stringIDs.Add(1664785551U, "EventPendingModerationHelpDesk");
			CoreStrings.stringIDs.Add(1334000728U, "DeliveryStatusDelivered");
			CoreStrings.stringIDs.Add(135488574U, "TimeZoneEEuropeStandardTime");
			CoreStrings.stringIDs.Add(3847090244U, "RoleTypeDescription_MyVoiceMail");
			CoreStrings.stringIDs.Add(3097579504U, "TimeZoneMyanmarStandardTime");
			CoreStrings.stringIDs.Add(1321416518U, "TrackingExplanationLogsDeleted");
			CoreStrings.stringIDs.Add(2198659572U, "ExchangeOrgAdminDescription");
			CoreStrings.stringIDs.Add(2229119179U, "TimeZoneNepalStandardTime");
			CoreStrings.stringIDs.Add(203352201U, "TimeZoneCenAustraliaStandardTime");
			CoreStrings.stringIDs.Add(2350709050U, "JobStatusFailed");
			CoreStrings.stringIDs.Add(2909789462U, "RoleTypeDescription_TransportQueues");
			CoreStrings.stringIDs.Add(2239001839U, "TimeZoneWestPacificStandardTime");
			CoreStrings.stringIDs.Add(21646529U, "TrackingWarningNoResultsDueToTrackingTooEarly");
			CoreStrings.stringIDs.Add(4161689178U, "RoleTypeDescription_ViewOnlyOrganizationManagement");
			CoreStrings.stringIDs.Add(2206122650U, "RoleTypeDescription_ViewOnlyRecipients");
			CoreStrings.stringIDs.Add(1776488541U, "Allow");
			CoreStrings.stringIDs.Add(430852938U, "DomainScopeAlLDomains");
			CoreStrings.stringIDs.Add(1082730632U, "NoValidDomainNameExistsInDomainSettings");
			CoreStrings.stringIDs.Add(377106190U, "ExchangeRecordsManagementDescription");
			CoreStrings.stringIDs.Add(2294597161U, "TimeZoneUTC08");
			CoreStrings.stringIDs.Add(1319136228U, "RoleTypeDescription_NetworkingManagement");
			CoreStrings.stringIDs.Add(3173642551U, "EventTransferredToForeignOrgHelpDesk");
			CoreStrings.stringIDs.Add(3245137676U, "RoleTypeDescription_MyFacebookEnabled");
			CoreStrings.stringIDs.Add(3487147524U, "TimeZoneCentralEuropeStandardTime");
			CoreStrings.stringIDs.Add(254529528U, "RoleTypeDescription_SecurityGroupCreationAndMembership");
			CoreStrings.stringIDs.Add(1316318251U, "DeliveryStatusExpanded");
			CoreStrings.stringIDs.Add(1981651471U, "StatusPending");
			CoreStrings.stringIDs.Add(3359056161U, "TimeZoneMidAtlanticStandardTime");
			CoreStrings.stringIDs.Add(970642999U, "RoleTypeDescription_ResetPassword");
			CoreStrings.stringIDs.Add(1907760708U, "ExchangeUMManagementDescription");
			CoreStrings.stringIDs.Add(1392628209U, "TimeZoneUTC");
			CoreStrings.stringIDs.Add(547528310U, "RoleTypeDescription_DataLossPrevention");
			CoreStrings.stringIDs.Add(429272165U, "EventPendingModerationIW");
			CoreStrings.stringIDs.Add(1939880180U, "RoleTypeDescription_MyCustomApps");
			CoreStrings.stringIDs.Add(2722115341U, "RoleTypeDescription_DatabaseAvailabilityGroups");
			CoreStrings.stringIDs.Add(2031056185U, "ExchangeHelpDeskDescription");
			CoreStrings.stringIDs.Add(2388819289U, "MessageDirectionReceived");
			CoreStrings.stringIDs.Add(1562661838U, "SpamQuarantineMigrationSucceeded");
			CoreStrings.stringIDs.Add(2708860089U, "EventFailedModerationIW");
			CoreStrings.stringIDs.Add(2294597167U, "TimeZoneUTC02");
			CoreStrings.stringIDs.Add(4202622721U, "AntimalwareScopingConstraint");
			CoreStrings.stringIDs.Add(2928288207U, "DeliveryStatusFailed");
			CoreStrings.stringIDs.Add(3278150655U, "QuarantineInbound");
			CoreStrings.stringIDs.Add(872985302U, "RoleTypeDescription_Throttling");
			CoreStrings.stringIDs.Add(2019318218U, "RoleTypeDescription_DataCenterDestructiveOperations");
			CoreStrings.stringIDs.Add(2945933912U, "RoleTypeDescription_AddressLists");
			CoreStrings.stringIDs.Add(1959014922U, "RoleTypeDescription_CentralAdminManagement");
			CoreStrings.stringIDs.Add(694118059U, "TimeZoneAfghanistanStandardTime");
			CoreStrings.stringIDs.Add(2433016809U, "EventTransferredToForeignOrgIW");
			CoreStrings.stringIDs.Add(1457011382U, "JobStatusCancelled");
			CoreStrings.stringIDs.Add(2033924825U, "TimeZoneAtlanticStandardTime");
			CoreStrings.stringIDs.Add(1901284917U, "TimeZoneArabStandardTime");
			CoreStrings.stringIDs.Add(238886746U, "RoleTypeDescription_MailRecipients");
			CoreStrings.stringIDs.Add(2886777393U, "RoleTypeDescription_WorkloadManagement");
			CoreStrings.stringIDs.Add(1286295614U, "TimeZoneAlaskanStandardTime");
			CoreStrings.stringIDs.Add(1235839409U, "MsoManagedTenantHelpdeskGroupDescription");
			CoreStrings.stringIDs.Add(1838296451U, "AntimalwareInboundRecipientNotifications");
			CoreStrings.stringIDs.Add(270033310U, "TestXHeaderAndModifySubject");
			CoreStrings.stringIDs.Add(1923042104U, "ContentIndexStatusFailedAndSuspended");
			CoreStrings.stringIDs.Add(2104897796U, "MsoManagedTenantAdminGroupDescription");
			CoreStrings.stringIDs.Add(1558282382U, "RoleTypeDescription_DataCenterOperations");
			CoreStrings.stringIDs.Add(3700079135U, "EventModerationExpired");
			CoreStrings.stringIDs.Add(29170872U, "TraceLevelMedium");
			CoreStrings.stringIDs.Add(432229092U, "RoleTypeDescription_MoveMailboxes");
			CoreStrings.stringIDs.Add(1038994972U, "RoleTypeDescription_MailRecipientCreation");
			CoreStrings.stringIDs.Add(3021954325U, "TimeZoneLineIslandsStandardTime");
			CoreStrings.stringIDs.Add(2008936115U, "MissingIdentityParameter");
			CoreStrings.stringIDs.Add(3268869348U, "ContentIndexStatusHealthyAndUpgrading");
			CoreStrings.stringIDs.Add(1450163010U, "TrafficScopeInbound");
			CoreStrings.stringIDs.Add(880515475U, "RoleTypeDescription_UnScopedRoleManagement");
			CoreStrings.stringIDs.Add(267867511U, "AuditSeverityLevelDoNotAudit");
			CoreStrings.stringIDs.Add(1778108211U, "ExchangeHygieneManagementDescription");
			CoreStrings.stringIDs.Add(1246550425U, "ClassIdExtensions");
			CoreStrings.stringIDs.Add(4236830390U, "TimeZoneNamibiaStandardTime");
			CoreStrings.stringIDs.Add(2735513322U, "RejectedExplanationContentFiltering");
			CoreStrings.stringIDs.Add(1179870130U, "QuarantineOutbound");
			CoreStrings.stringIDs.Add(2454182516U, "RunWeekly");
			CoreStrings.stringIDs.Add(1623584678U, "TimeZoneAltaiStandardTime");
			CoreStrings.stringIDs.Add(889690570U, "EventSubmittedCrossSite");
			CoreStrings.stringIDs.Add(565087651U, "TimeZoneEasternStandardTime");
			CoreStrings.stringIDs.Add(1000900012U, "TimeZoneAzerbaijanStandardTime");
			CoreStrings.stringIDs.Add(2216352038U, "TimeZonePakistanStandardTime");
			CoreStrings.stringIDs.Add(790166856U, "RoleTypeDescription_OrgMarketplaceApps");
			CoreStrings.stringIDs.Add(3066204968U, "TimeZonePacificSAStandardTime");
			CoreStrings.stringIDs.Add(2981542010U, "TimeZoneRussianStandardTime");
			CoreStrings.stringIDs.Add(2623630612U, "RoleTypeDescription_POP3AndIMAP4Protocols");
			CoreStrings.stringIDs.Add(860548277U, "TimeZoneTaipeiStandardTime");
			CoreStrings.stringIDs.Add(1145350245U, "RoleTypeDescription_HelpdeskRecipientManagement");
			CoreStrings.stringIDs.Add(756612021U, "TimeZoneHawaiianStandardTime");
			CoreStrings.stringIDs.Add(2907857020U, "TimeZoneMagallanesStandardTime");
			CoreStrings.stringIDs.Add(101906589U, "EventFailedTransportRulesHelpDesk");
			CoreStrings.stringIDs.Add(2104951827U, "TimeZoneTokyoStandardTime");
			CoreStrings.stringIDs.Add(2425183541U, "DeliveryStatusAll");
			CoreStrings.stringIDs.Add(4216139125U, "RoleTypeDescription_MyOptions");
			CoreStrings.stringIDs.Add(996841945U, "RoleTypeDescription_DisasterRecovery");
			CoreStrings.stringIDs.Add(3202528089U, "RoleTypeDescription_EmailAddressPolicies");
			CoreStrings.stringIDs.Add(3655383969U, "RoleTypeDescription_SendConnectors");
			CoreStrings.stringIDs.Add(3185790196U, "TimeZoneSudanStandardTime");
			CoreStrings.stringIDs.Add(1544756182U, "EventRead");
			CoreStrings.stringIDs.Add(2467780809U, "TimeZoneGreenlandStandardTime");
			CoreStrings.stringIDs.Add(2169942641U, "RoleTypeDescription_OutlookProvider");
			CoreStrings.stringIDs.Add(1460332657U, "EventTransferredIntermediate");
			CoreStrings.stringIDs.Add(3444675422U, "RoleTypeDescription_OrgCustomApps");
			CoreStrings.stringIDs.Add(2174613220U, "TimeZoneBangladeshStandardTime");
			CoreStrings.stringIDs.Add(373243623U, "TimeZoneGMTStandardTime");
			CoreStrings.stringIDs.Add(1223375664U, "TimeZoneGTBStandardTime");
			CoreStrings.stringIDs.Add(4078458935U, "JobStatusNotStarted");
			CoreStrings.stringIDs.Add(1663352993U, "TrackingWarningNoResultsDueToLogsNotFound");
			CoreStrings.stringIDs.Add(77017683U, "TimeZoneSAWesternStandardTime");
			CoreStrings.stringIDs.Add(3490088586U, "TimeZoneHaitiStandardTime");
			CoreStrings.stringIDs.Add(2700445791U, "TrackingErrorFailedToInitialize");
			CoreStrings.stringIDs.Add(728892142U, "TimeZoneKamchatkaStandardTime");
			CoreStrings.stringIDs.Add(4111676399U, "TrackingWarningResultsMissingTransient");
			CoreStrings.stringIDs.Add(2247997721U, "RulesMerged");
			CoreStrings.stringIDs.Add(3804873683U, "RoleTypeDescription_MyRetentionPolicies");
			CoreStrings.stringIDs.Add(647678831U, "DomainScopedRulesMerged");
			CoreStrings.stringIDs.Add(1612370800U, "EventResolvedHelpDesk");
			CoreStrings.stringIDs.Add(1531217347U, "RoleTypeDescription_LegalHoldApplication");
			CoreStrings.stringIDs.Add(3498536455U, "TimeZoneSingaporeStandardTime");
			CoreStrings.stringIDs.Add(1949143649U, "AntimalwareAdminAddressNull");
			CoreStrings.stringIDs.Add(2725696980U, "RoleTypeDescription_UserApplication");
			CoreStrings.stringIDs.Add(2096766553U, "RoleTypeDescription_MyContactInformation");
			CoreStrings.stringIDs.Add(967467375U, "TimeZoneNorthAsiaEastStandardTime");
			CoreStrings.stringIDs.Add(4108314201U, "ExchangeRecipientAdminDescription");
			CoreStrings.stringIDs.Add(226934128U, "TimeZoneCapeVerdeStandardTime");
			CoreStrings.stringIDs.Add(3317872187U, "TrackingSearchNotAuthorized");
			CoreStrings.stringIDs.Add(1575862374U, "ContentIndexStatusCrawling");
			CoreStrings.stringIDs.Add(2874655766U, "TimeZoneMagadanStandardTime");
			CoreStrings.stringIDs.Add(2055163269U, "TimeZoneAUSCentralStandardTime");
			CoreStrings.stringIDs.Add(2585558426U, "ExchangeDiscoveryManagementDescription");
			CoreStrings.stringIDs.Add(3586764147U, "EventMessageDefer");
			CoreStrings.stringIDs.Add(2033601224U, "TimeZoneVolgogradStandardTime");
			CoreStrings.stringIDs.Add(2373652411U, "TimeZonePacificStandardTimeMexico");
			CoreStrings.stringIDs.Add(2679378804U, "TimeZoneSamoaStandardTime");
			CoreStrings.stringIDs.Add(2872669904U, "RoleTypeDescription_PersonallyIdentifiableInformation");
			CoreStrings.stringIDs.Add(728513226U, "TimeZoneUTC12");
			CoreStrings.stringIDs.Add(3152944101U, "RoleTypeDescription_MessageTracking");
			CoreStrings.stringIDs.Add(1084831951U, "TraceLevelLow");
			CoreStrings.stringIDs.Add(661809288U, "RoleTypeDescription_MyTextMessaging");
			CoreStrings.stringIDs.Add(2577770798U, "RoleTypeDescription_RecipientPolicies");
			CoreStrings.stringIDs.Add(953426317U, "TimeZoneSAEasternStandardTime");
			CoreStrings.stringIDs.Add(2376615838U, "RoleTypeDescription_TeamMailboxLifecycleApplication");
			CoreStrings.stringIDs.Add(1205552534U, "InvalidMessageTrackingReportId");
			CoreStrings.stringIDs.Add(430105556U, "TimeZoneIndiaStandardTime");
			CoreStrings.stringIDs.Add(2562345274U, "ContentIndexStatusFailed");
			CoreStrings.stringIDs.Add(1494389266U, "TimeZoneNewfoundlandStandardTime");
			CoreStrings.stringIDs.Add(2246440755U, "TimeZoneYakutskStandardTime");
			CoreStrings.stringIDs.Add(1274798456U, "RoleTypeDescription_OrganizationHelpSettings");
			CoreStrings.stringIDs.Add(1835211555U, "EventFailedModerationHelpDesk");
			CoreStrings.stringIDs.Add(903792657U, "RoleTypeDescription_PublicFolders");
			CoreStrings.stringIDs.Add(3005390568U, "TimeZonePacificStandardTime");
			CoreStrings.stringIDs.Add(61789322U, "TimeZoneAleutianStandardTime");
			CoreStrings.stringIDs.Add(3049152015U, "ExchangeManagementForestTier1SupportDescription");
			CoreStrings.stringIDs.Add(2034512234U, "TimeZoneNorthKoreaStandardTime");
			CoreStrings.stringIDs.Add(920357151U, "DltUnknownTimeZone");
			CoreStrings.stringIDs.Add(2373064468U, "RoleTypeDescription_ExchangeConnectors");
			CoreStrings.stringIDs.Add(4036330655U, "RoleTypeDescription_ViewOnlyCentralAdminSupport");
			CoreStrings.stringIDs.Add(4097835338U, "TimeZoneNorfolkStandardTime");
			CoreStrings.stringIDs.Add(273095822U, "RoleTypeDescription_MyProfileInformation");
			CoreStrings.stringIDs.Add(1814215051U, "TimeZoneSaoTomeStandardTime");
			CoreStrings.stringIDs.Add(2648516368U, "RoleTypeDescription_Journaling");
			CoreStrings.stringIDs.Add(1736152949U, "TimeZoneAzoresStandardTime");
			CoreStrings.stringIDs.Add(290876870U, "TimeZoneAusCentralWStandardTime");
			CoreStrings.stringIDs.Add(4043635750U, "TimeZoneUSMountainStandardTime");
			CoreStrings.stringIDs.Add(606097983U, "RoleTypeDescription_ExchangeCrossServiceIntegration");
			CoreStrings.stringIDs.Add(3335916139U, "PolicyMigrationSucceeded");
			CoreStrings.stringIDs.Add(140823076U, "TimeZoneCentralBrazilianStandardTime");
			CoreStrings.stringIDs.Add(2103082753U, "EventRulesCc");
			CoreStrings.stringIDs.Add(2088363403U, "ComplianceManagementGroupDescription");
			CoreStrings.stringIDs.Add(1556269475U, "TimeZoneWCentralAfricaStandardTime");
			CoreStrings.stringIDs.Add(3371502447U, "RoleTypeDescription_UMMailboxes");
			CoreStrings.stringIDs.Add(1332960560U, "ExchangeAllHostedOrgsDescription");
			CoreStrings.stringIDs.Add(1124818555U, "RoleTypeDescription_DistributionGroups");
			CoreStrings.stringIDs.Add(92857077U, "ExchangeDelegatedSetupDescription");
			CoreStrings.stringIDs.Add(2984557217U, "TimeZoneVladivostokStandardTime");
			CoreStrings.stringIDs.Add(1755122990U, "TimeZoneCentralStandardTime");
			CoreStrings.stringIDs.Add(278680123U, "RoleTypeDescription_OrganizationClientAccess");
			CoreStrings.stringIDs.Add(137603179U, "ExchangeViewOnlyAdminDescription");
			CoreStrings.stringIDs.Add(2189811946U, "TimeZoneEAustraliaStandardTime");
			CoreStrings.stringIDs.Add(763976563U, "TrackingTransientError");
			CoreStrings.stringIDs.Add(3970531209U, "TrafficScopeInboundAndOutbound");
			CoreStrings.stringIDs.Add(675468529U, "RoleTypeDescription_CentralAdminCredentialManagement");
			CoreStrings.stringIDs.Add(2365507750U, "RoleTypeDescription_UnScoped");
			CoreStrings.stringIDs.Add(2259701124U, "RoleTypeDescription_MyDistributionGroupMembership");
			CoreStrings.stringIDs.Add(3079546428U, "RoleTypeDescription_OfficeExtensionApplication");
			CoreStrings.stringIDs.Add(3118752499U, "RoleTypeDescription_DistributionGroupManagement");
			CoreStrings.stringIDs.Add(2462125228U, "AntimalwareMigrationSucceeded");
			CoreStrings.stringIDs.Add(529456528U, "TimeZoneWEuropeStandardTime");
			CoreStrings.stringIDs.Add(172038955U, "RoleTypeDescription_AuditLogs");
			CoreStrings.stringIDs.Add(3669915041U, "EventSubmitted");
			CoreStrings.stringIDs.Add(2836851600U, "TimeZoneEasternStandardTimeMexico");
			CoreStrings.stringIDs.Add(2471825646U, "TimeZoneRomanceStandardTime");
			CoreStrings.stringIDs.Add(2847973369U, "TimeZoneIranStandardTime");
			CoreStrings.stringIDs.Add(2304159373U, "RoleTypeDescription_AccessToCustomerDataDCOnly");
			CoreStrings.stringIDs.Add(102930739U, "InvalidIdentityForAdmin");
			CoreStrings.stringIDs.Add(3118313859U, "TimeZoneMarquesasStandardTime");
			CoreStrings.stringIDs.Add(4010596708U, "ContentIndexStatusHealthy");
			CoreStrings.stringIDs.Add(2141699896U, "RunMonthly");
			CoreStrings.stringIDs.Add(1224765338U, "RoleTypeDescription_OrganizationTransportSettings");
			CoreStrings.stringIDs.Add(56162392U, "RoleTypeDescription_ViewOnlyConfiguration");
			CoreStrings.stringIDs.Add(3450693719U, "TimeZoneMountainStandardTimeMexico");
			CoreStrings.stringIDs.Add(3338361539U, "RoleTypeDescription_PartnerDelegatedTenantManagement");
			CoreStrings.stringIDs.Add(1621594316U, "RoleTypeDescription_OrganizationConfiguration");
			CoreStrings.stringIDs.Add(3266046896U, "MsoMailTenantAdminGroupDescription");
			CoreStrings.stringIDs.Add(4268682097U, "TimeZoneSEAsiaStandardTime");
			CoreStrings.stringIDs.Add(3867579719U, "RoleTypeDescription_DatabaseCopies");
			CoreStrings.stringIDs.Add(1556691491U, "TimeZoneCentralStandardTimeMexico");
			CoreStrings.stringIDs.Add(2695329098U, "OutboundIpMigrationCompleted");
			CoreStrings.stringIDs.Add(2154772013U, "RecipientPathFilterNeeded");
			CoreStrings.stringIDs.Add(3188452985U, "TrackingWarningResultsMissingConnection");
			CoreStrings.stringIDs.Add(4058915032U, "TimeZoneNCentralAsiaStandardTime");
			CoreStrings.stringIDs.Add(1753131922U, "PolicyTipReject");
			CoreStrings.stringIDs.Add(2037460782U, "AuditSeverityLevelLow");
			CoreStrings.stringIDs.Add(2054972301U, "RoleTypeDescription_RecipientManagement");
			CoreStrings.stringIDs.Add(435759238U, "EventForwarded");
			CoreStrings.stringIDs.Add(1260062527U, "EventApprovedModerationHelpDesk");
			CoreStrings.stringIDs.Add(2235638931U, "Reject");
			CoreStrings.stringIDs.Add(3526220825U, "TimeZoneSaintPierreStandardTime");
			CoreStrings.stringIDs.Add(1391837657U, "TimeZoneMoroccoStandardTime");
			CoreStrings.stringIDs.Add(3250495282U, "TimeZoneFLEStandardTime");
			CoreStrings.stringIDs.Add(566029912U, "RoleTypeDescription_MailTips");
			CoreStrings.stringIDs.Add(3881958977U, "RoleTypeDescription_Monitoring");
			CoreStrings.stringIDs.Add(2892044242U, "AntispamEdgeBlockModeSettingNotMigrated");
			CoreStrings.stringIDs.Add(779085836U, "TimeZoneCanadaCentralStandardTime");
			CoreStrings.stringIDs.Add(2024391679U, "ExchangeManagementForestMonitoringDescription");
			CoreStrings.stringIDs.Add(1004126924U, "TimeZoneSakhalinStandardTime");
			CoreStrings.stringIDs.Add(59677322U, "TimeZoneSriLankaStandardTime");
			CoreStrings.stringIDs.Add(766524993U, "RoleTypeDescription_AutoDiscover");
			CoreStrings.stringIDs.Add(4291544598U, "TimeZoneRussiaTimeZone10");
			CoreStrings.stringIDs.Add(3673094505U, "EventTransferredToSMSMessage");
			CoreStrings.stringIDs.Add(3152178141U, "TrackingWarningNoResultsDueToLogsExpired");
			CoreStrings.stringIDs.Add(3797334152U, "MessageDirectionSent");
			CoreStrings.stringIDs.Add(4116693438U, "RoleTypeDescription_RecordsManagement");
			CoreStrings.stringIDs.Add(825624151U, "RoleTypeDescription_TransportRules");
			CoreStrings.stringIDs.Add(1566193246U, "ForceTls");
			CoreStrings.stringIDs.Add(3297737305U, "EventFailedGeneral");
			CoreStrings.stringIDs.Add(2580146609U, "RoleTypeDescription_ExchangeServerCertificates");
			CoreStrings.stringIDs.Add(1092647167U, "EventPending");
			CoreStrings.stringIDs.Add(845119240U, "TimeZoneChinaStandardTime");
			CoreStrings.stringIDs.Add(4218393972U, "TimeZoneWestBankStandardTime");
			CoreStrings.stringIDs.Add(1316750960U, "RoleTypeDescription_MyBaseOptions");
			CoreStrings.stringIDs.Add(3120202503U, "TimeZoneGreenwichStandardTime");
			CoreStrings.stringIDs.Add(3840952578U, "RoleTypeDescription_UMPromptManagement");
			CoreStrings.stringIDs.Add(3678129241U, "AuditSeverityLevelMedium");
			CoreStrings.stringIDs.Add(4008278162U, "StatusFilterCannotBeSpecified");
			CoreStrings.stringIDs.Add(1056819816U, "ContentIndexStatusSuspended");
			CoreStrings.stringIDs.Add(728513227U, "TimeZoneUTC13");
			CoreStrings.stringIDs.Add(1737084126U, "PolicyTipUrl");
			CoreStrings.stringIDs.Add(2937930519U, "JobStatusDone");
			CoreStrings.stringIDs.Add(2403632430U, "Quarantine");
			CoreStrings.stringIDs.Add(3403825909U, "TrackingWarningResultsMissingFatal");
			CoreStrings.stringIDs.Add(1215124770U, "TimeZoneNewZealandStandardTime");
			CoreStrings.stringIDs.Add(3584252448U, "ExecutableAttachments");
			CoreStrings.stringIDs.Add(2732724415U, "TimeZoneDatelineStandardTime");
			CoreStrings.stringIDs.Add(1156214899U, "EventQueueRetryIW");
		}

		public static LocalizedString RoleTypeDescription_RemoteAndAcceptedDomains
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_RemoteAndAcceptedDomains", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneSouthAfricaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneSouthAfricaStandardTime", "Ex180677", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PolicyTipNotifyOnly
		{
			get
			{
				return new LocalizedString("PolicyTipNotifyOnly", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneLibyaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneLibyaStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportSeverityLow
		{
			get
			{
				return new LocalizedString("ReportSeverityLow", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_ArchiveApplication
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_ArchiveApplication", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidIpRange(int ruleId, string invalidIpRange)
		{
			return new LocalizedString("InvalidIpRange", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId,
				invalidIpRange
			});
		}

		public static LocalizedString EventDelivered
		{
			get
			{
				return new LocalizedString("EventDelivered", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AntispamIPBlockListSettingMigrated
		{
			get
			{
				return new LocalizedString("AntispamIPBlockListSettingMigrated", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_ActiveMonitoring
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_ActiveMonitoring", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_UMPrompts
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_UMPrompts", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneCentralAsiaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneCentralAsiaStandardTime", "Ex7D6A1F", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneESouthAmericaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneESouthAmericaStandardTime", "ExAA4B78", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneMountainStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneMountainStandardTime", "Ex5F9A41", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneKaliningradStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneKaliningradStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeViewOnlyManagementForestOperatorDescription
		{
			get
			{
				return new LocalizedString("ExchangeViewOnlyManagementForestOperatorDescription", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AntispamMigrationSucceeded
		{
			get
			{
				return new LocalizedString("AntispamMigrationSucceeded", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneNorthAsiaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneNorthAsiaStandardTime", "ExA93F77", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSender
		{
			get
			{
				return new LocalizedString("InvalidSender", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_MyTeamMailboxes
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MyTeamMailboxes", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_MyDistributionGroups
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MyDistributionGroups", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneFijiStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneFijiStandardTime", "Ex762231", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneEAfricaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneEAfricaStandardTime", "ExCF702A", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotAuthorizedToViewRecipientPath
		{
			get
			{
				return new LocalizedString("NotAuthorizedToViewRecipientPath", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_UserOptions
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_UserOptions", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_UmRecipientManagement
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_UmRecipientManagement", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDomainNameInDomainSettings(string domainName)
		{
			return new LocalizedString("InvalidDomainNameInDomainSettings", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				domainName
			});
		}

		public static LocalizedString ReportSeverityHigh
		{
			get
			{
				return new LocalizedString("ReportSeverityHigh", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FopePolicyRuleIsTooLargeToMigrate(string ruleName, ulong ruleSize, ulong maxRuleSize)
		{
			return new LocalizedString("FopePolicyRuleIsTooLargeToMigrate", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleName,
				ruleSize,
				maxRuleSize
			});
		}

		public static LocalizedString RoleTypeDescription_CmdletExtensionAgents
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_CmdletExtensionAgents", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OpportunisticTls(int ruleId)
		{
			return new LocalizedString("OpportunisticTls", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId
			});
		}

		public static LocalizedString AntimalwareUserOptOut
		{
			get
			{
				return new LocalizedString("AntimalwareUserOptOut", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventMovedToFolderByInboxRuleHelpDesk(string folderName)
		{
			return new LocalizedString("EventMovedToFolderByInboxRuleHelpDesk", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				folderName
			});
		}

		public static LocalizedString RoleTypeDescription_MyLogon
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MyLogon", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContentIndexStatusDisabled
		{
			get
			{
				return new LocalizedString("ContentIndexStatusDisabled", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PolicyTipRejectOverride
		{
			get
			{
				return new LocalizedString("PolicyTipRejectOverride", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StatusDelivered
		{
			get
			{
				return new LocalizedString("StatusDelivered", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneWMongoliaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneWMongoliaStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_TransportHygiene
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_TransportHygiene", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TraceLevelHigh
		{
			get
			{
				return new LocalizedString("TraceLevelHigh", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_InformationRightsManagement
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_InformationRightsManagement", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TestModifySubject
		{
			get
			{
				return new LocalizedString("TestModifySubject", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneLordHoweStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneLordHoweStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSenderForAdmin
		{
			get
			{
				return new LocalizedString("InvalidSenderForAdmin", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_DiscoveryManagement
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_DiscoveryManagement", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FopePolicyRuleContainsInvalidPattern(string ruleName)
		{
			return new LocalizedString("FopePolicyRuleContainsInvalidPattern", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleName
			});
		}

		public static LocalizedString TimeZoneTurksAndCaicosStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneTurksAndCaicosStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_MailboxExport
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MailboxExport", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AntispamIPAllowListSettingMigrated
		{
			get
			{
				return new LocalizedString("AntispamIPAllowListSettingMigrated", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeManagementForestOperatorDescription
		{
			get
			{
				return new LocalizedString("ExchangeManagementForestOperatorDescription", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneArabianStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneArabianStandardTime", "Ex36DED6", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneIsraelStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneIsraelStandardTime", "Ex2004E1", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_ActiveDirectoryPermissions
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_ActiveDirectoryPermissions", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventSubmittedHelpDesk(string hubServerFqdn)
		{
			return new LocalizedString("EventSubmittedHelpDesk", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				hubServerFqdn
			});
		}

		public static LocalizedString RunOnce
		{
			get
			{
				return new LocalizedString("RunOnce", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FopePolicyRuleExpired(int ruleId, DateTime expiredOn)
		{
			return new LocalizedString("FopePolicyRuleExpired", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId,
				expiredOn
			});
		}

		public static LocalizedString MessageDirectionAll
		{
			get
			{
				return new LocalizedString("MessageDirectionAll", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_UmManagement
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_UmManagement", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AuditSeverityLevelHigh
		{
			get
			{
				return new LocalizedString("AuditSeverityLevelHigh", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneArgentinaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneArgentinaStandardTime", "ExBE5AC7", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecipientDomainConditionContainsInvalidDomainNames(int ruleId, string domainNames)
		{
			return new LocalizedString("RecipientDomainConditionContainsInvalidDomainNames", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId,
				domainNames
			});
		}

		public static LocalizedString RoleTypeDescription_Migration
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_Migration", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneUlaanbaatarStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneUlaanbaatarStandardTime", "Ex5AA96E", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneSaratovStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneSaratovStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DistributionGroupForVirtualDomainsCreated(string dgName, string dgOwner)
		{
			return new LocalizedString("DistributionGroupForVirtualDomainsCreated", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				dgName,
				dgOwner
			});
		}

		public static LocalizedString EventSmtpReceiveHelpDesk(string local, string remote)
		{
			return new LocalizedString("EventSmtpReceiveHelpDesk", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				local,
				remote
			});
		}

		public static LocalizedString TimeZoneVenezuelaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneVenezuelaStandardTime", "Ex5A6891", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecipientDomainNames(string recipientDomain)
		{
			return new LocalizedString("RecipientDomainNames", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				recipientDomain
			});
		}

		public static LocalizedString ExchangeAllMailboxesDescription
		{
			get
			{
				return new LocalizedString("ExchangeAllMailboxesDescription", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_GALSynchronizationManagement
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_GALSynchronizationManagement", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_ViewOnlyRoleManagement
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_ViewOnlyRoleManagement", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_LiveID
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_LiveID", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BodyCaseSensitive(string body)
		{
			return new LocalizedString("BodyCaseSensitive", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				body
			});
		}

		public static LocalizedString TenantSkuNotSupportedByAntispam(string skuName)
		{
			return new LocalizedString("TenantSkuNotSupportedByAntispam", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				skuName
			});
		}

		public static LocalizedString DisabledInboundConnector(string name)
		{
			return new LocalizedString("DisabledInboundConnector", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString InvalidSmtpAddressInFopeRule(int ruleId, string smtpAddress)
		{
			return new LocalizedString("InvalidSmtpAddressInFopeRule", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId,
				smtpAddress
			});
		}

		public static LocalizedString RoleTypeDescription_TransportAgents
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_TransportAgents", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_MyDiagnostics
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MyDiagnostics", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingWarningNoResultsDueToUntrackableMessagePath
		{
			get
			{
				return new LocalizedString("TrackingWarningNoResultsDueToUntrackableMessagePath", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventTransferredToLegacyExchangeServer
		{
			get
			{
				return new LocalizedString("EventTransferredToLegacyExchangeServer", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_TeamMailboxes
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_TeamMailboxes", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneSAPacificStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneSAPacificStandardTime", "ExA231DF", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeServerManagementDescription
		{
			get
			{
				return new LocalizedString("ExchangeServerManagementDescription", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnsupportedSenderForTracking
		{
			get
			{
				return new LocalizedString("UnsupportedSenderForTracking", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneMontevideoStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneMontevideoStandardTime", "ExFB22C1", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneUSEasternStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneUSEasternStandardTime", "ExB9A839", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventApprovedModerationIW
		{
			get
			{
				return new LocalizedString("EventApprovedModerationIW", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneCentralAmericaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneCentralAmericaStandardTime", "Ex441256", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContentIndexStatusAutoSuspended
		{
			get
			{
				return new LocalizedString("ContentIndexStatusAutoSuspended", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneBahiaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneBahiaStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_EdgeSubscriptions
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_EdgeSubscriptions", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecipientAddresses(string recipientAddress)
		{
			return new LocalizedString("RecipientAddresses", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				recipientAddress
			});
		}

		public static LocalizedString TimeZoneCubaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneCubaStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneEasterIslandStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneEasterIslandStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubjectExactMatchCondition(int ruleId)
		{
			return new LocalizedString("SubjectExactMatchCondition", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId
			});
		}

		public static LocalizedString JobStatusInProgress
		{
			get
			{
				return new LocalizedString("JobStatusInProgress", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubjectExactMatchCaseSensitive(string subject)
		{
			return new LocalizedString("SubjectExactMatchCaseSensitive", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				subject
			});
		}

		public static LocalizedString TimeZoneUTC09
		{
			get
			{
				return new LocalizedString("TimeZoneUTC09", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneEgyptStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneEgyptStandardTime", "ExB364F1", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StatusRead
		{
			get
			{
				return new LocalizedString("StatusRead", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneCaucasusStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneCaucasusStandardTime", "ExAE218F", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_MailEnabledPublicFolders
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MailEnabledPublicFolders", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_ViewOnlyAuditLogs
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_ViewOnlyAuditLogs", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingWarningReadStatusUnavailable
		{
			get
			{
				return new LocalizedString("TrackingWarningReadStatusUnavailable", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ViewOnlyPIIGroupDescription
		{
			get
			{
				return new LocalizedString("ViewOnlyPIIGroupDescription", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_FederatedSharing
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_FederatedSharing", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_RoleManagement
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_RoleManagement", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AntimalwareInboundRecipientNotificationsScoped(string policyName)
		{
			return new LocalizedString("AntimalwareInboundRecipientNotificationsScoped", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				policyName
			});
		}

		public static LocalizedString TrackingExplanationNormalTimeSpan
		{
			get
			{
				return new LocalizedString("TrackingExplanationNormalTimeSpan", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_ExchangeServers
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_ExchangeServers", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneAstrakhanStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneAstrakhanStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportSeverityMedium
		{
			get
			{
				return new LocalizedString("ReportSeverityMedium", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Body(string body)
		{
			return new LocalizedString("Body", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				body
			});
		}

		public static LocalizedString RoleTypeDescription_SupportDiagnostics
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_SupportDiagnostics", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneCentralEuropeanStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneCentralEuropeanStandardTime", "Ex980388", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneKoreaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneKoreaStandardTime", "Ex088D32", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContentIndexStatusSeeding
		{
			get
			{
				return new LocalizedString("ContentIndexStatusSeeding", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneWAustraliaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneWAustraliaStandardTime", "ExEFC547", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_Custom
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_Custom", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AntispamActionTypeSettingMigrated
		{
			get
			{
				return new LocalizedString("AntispamActionTypeSettingMigrated", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingExplanationExcessiveTimeSpan
		{
			get
			{
				return new LocalizedString("TrackingExplanationExcessiveTimeSpan", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuarantineSpam
		{
			get
			{
				return new LocalizedString("QuarantineSpam", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneUTC11
		{
			get
			{
				return new LocalizedString("TimeZoneUTC11", "Ex598DA7", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_PublicFolderReplication
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_PublicFolderReplication", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventQueueRetryNoRetryTimeHelpDesk(string queueName, string errorMessage)
		{
			return new LocalizedString("EventQueueRetryNoRetryTimeHelpDesk", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				queueName,
				errorMessage
			});
		}

		public static LocalizedString Encrypt
		{
			get
			{
				return new LocalizedString("Encrypt", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneRussiaTimeZone11
		{
			get
			{
				return new LocalizedString("TimeZoneRussiaTimeZone11", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneEkaterinburgStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneEkaterinburgStandardTime", "Ex7F643C", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_LegalHold
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_LegalHold", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneTocantinsStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneTocantinsStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneArabicStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneArabicStandardTime", "Ex249593", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubjectCaseSensitiveCondition(int ruleId)
		{
			return new LocalizedString("SubjectCaseSensitiveCondition", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId
			});
		}

		public static LocalizedString RoleTypeDescription_MailboxImportExport
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MailboxImportExport", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_Supervision
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_Supervision", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_LawEnforcementRequests
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_LawEnforcementRequests", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_MailboxSearchApplication
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MailboxSearchApplication", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_RetentionManagement
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_RetentionManagement", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneWestAsiaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneWestAsiaStandardTime", "Ex12FC9B", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuarantineTransportRule
		{
			get
			{
				return new LocalizedString("QuarantineTransportRule", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_ViewOnlyCentralAdminManagement
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_ViewOnlyCentralAdminManagement", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventFailedTransportRulesIW
		{
			get
			{
				return new LocalizedString("EventFailedTransportRulesIW", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventDelayedAfterTransferToPartnerOrgIW
		{
			get
			{
				return new LocalizedString("EventDelayedAfterTransferToPartnerOrgIW", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_DatacenterOperationsDCOnly
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_DatacenterOperationsDCOnly", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneTomskStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneTomskStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MaximumMessageSize(int maxSize)
		{
			return new LocalizedString("MaximumMessageSize", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				maxSize
			});
		}

		public static LocalizedString TrackingBusy
		{
			get
			{
				return new LocalizedString("TrackingBusy", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneTongaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneTongaStandardTime", "Ex2872C8", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneTasmaniaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneTasmaniaStandardTime", "ExC5BEEF", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangePublicFolderAdminDescription
		{
			get
			{
				return new LocalizedString("ExchangePublicFolderAdminDescription", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrafficScopeOutbound
		{
			get
			{
				return new LocalizedString("TrafficScopeOutbound", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventQueueRetryNoErrorHelpDesk(string server, string inRetrySinceTime, string lastAttemptTime, string timeZone)
		{
			return new LocalizedString("EventQueueRetryNoErrorHelpDesk", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				server,
				inRetrySinceTime,
				lastAttemptTime,
				timeZone
			});
		}

		public static LocalizedString EventForwardedToDelegateAndDeleted
		{
			get
			{
				return new LocalizedString("EventForwardedToDelegateAndDeleted", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_GALSynchronization
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_GALSynchronization", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CompressionOutOfMemory
		{
			get
			{
				return new LocalizedString("CompressionOutOfMemory", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidUserName(string userName)
		{
			return new LocalizedString("InvalidUserName", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString TrafficScopeDisabled
		{
			get
			{
				return new LocalizedString("TrafficScopeDisabled", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_OrganizationManagement
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_OrganizationManagement", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneOmskStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneOmskStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AntimalwareAdminAddressValidations(string invalidSmtpAddress)
		{
			return new LocalizedString("AntimalwareAdminAddressValidations", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				invalidSmtpAddress
			});
		}

		public static LocalizedString SenderDomainNames(string senderDomain)
		{
			return new LocalizedString("SenderDomainNames", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				senderDomain
			});
		}

		public static LocalizedString TimeZoneBelarusStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneBelarusStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneParaguayStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneParaguayStandardTime", "ExCDD9CA", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_Reporting
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_Reporting", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneChathamIslandsStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneChathamIslandsStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_MyMailboxDelegation
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MyMailboxDelegation", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_ExchangeVirtualDirectories
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_ExchangeVirtualDirectories", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Subject(string subject)
		{
			return new LocalizedString("Subject", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				subject
			});
		}

		public static LocalizedString CharacterSets(string charsets)
		{
			return new LocalizedString("CharacterSets", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				charsets
			});
		}

		public static LocalizedString TimeZoneAUSEasternStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneAUSEasternStandardTime", "Ex7DA421", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventNotRead
		{
			get
			{
				return new LocalizedString("EventNotRead", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneMiddleEastStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneMiddleEastStandardTime", "Ex4F5342", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdminNotificationContainsMultipleAddresses(int ruleId, int numAdminAddress, string firstAddress, string skippedAddresses)
		{
			return new LocalizedString("AdminNotificationContainsMultipleAddresses", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId,
				numAdminAddress,
				firstAddress,
				skippedAddresses
			});
		}

		public static LocalizedString RoleTypeDescription_ApplicationImpersonation
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_ApplicationImpersonation", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingWarningTooManyEvents
		{
			get
			{
				return new LocalizedString("TrackingWarningTooManyEvents", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContentIndexStatusUnknown
		{
			get
			{
				return new LocalizedString("ContentIndexStatusUnknown", "Ex891A11", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneSyriaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneSyriaStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneMauritiusStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneMauritiusStandardTime", "Ex15E3D2", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingMessageTypeNotSupported
		{
			get
			{
				return new LocalizedString("TrackingMessageTypeNotSupported", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneCentralPacificStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneCentralPacificStandardTime", "Ex12E5BB", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FopePolicyRuleDisabled(int ruleId)
		{
			return new LocalizedString("FopePolicyRuleDisabled", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId
			});
		}

		public static LocalizedString RoleTypeDescription_MailboxSearch
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MailboxSearch", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StdUnknownTimeZone
		{
			get
			{
				return new LocalizedString("StdUnknownTimeZone", "Ex1B20DC", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StatusUnsuccessFul
		{
			get
			{
				return new LocalizedString("StatusUnsuccessFul", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_MyLinkedInEnabled
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MyLinkedInEnabled", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TestXHeader
		{
			get
			{
				return new LocalizedString("TestXHeader", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_ReceiveConnectors
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_ReceiveConnectors", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneRussiaTimeZone3
		{
			get
			{
				return new LocalizedString("TimeZoneRussiaTimeZone3", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneTransbaikalStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneTransbaikalStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_Databases
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_Databases", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DistributionGroupForExcludedUsersCreated(string dgName, string dgOwner)
		{
			return new LocalizedString("DistributionGroupForExcludedUsersCreated", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				dgName,
				dgOwner
			});
		}

		public static LocalizedString BodyCaseSensitiveCondition(int ruleId)
		{
			return new LocalizedString("BodyCaseSensitiveCondition", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId
			});
		}

		public static LocalizedString StatusTransferred
		{
			get
			{
				return new LocalizedString("StatusTransferred", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoValidSmtpAddress(int ruleId)
		{
			return new LocalizedString("NoValidSmtpAddress", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId
			});
		}

		public static LocalizedString TimeZoneGeorgianStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneGeorgianStandardTime", "Ex1225C4", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Decrypt
		{
			get
			{
				return new LocalizedString("Decrypt", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_MyReadWriteMailboxApps
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MyReadWriteMailboxApps", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneBougainvilleStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneBougainvilleStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_EdgeSync
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_EdgeSync", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneTurkeyStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneTurkeyStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_MyMailSubscriptions
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MyMailSubscriptions", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PartialMessages
		{
			get
			{
				return new LocalizedString("PartialMessages", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboundIpMigrationCompleted
		{
			get
			{
				return new LocalizedString("InboundIpMigrationCompleted", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_MyMarketplaceApps
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MyMarketplaceApps", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneJordanStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneJordanStandardTime", "Ex72D0C7", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventPendingModerationHelpDesk
		{
			get
			{
				return new LocalizedString("EventPendingModerationHelpDesk", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeliveryStatusDelivered
		{
			get
			{
				return new LocalizedString("DeliveryStatusDelivered", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneEEuropeStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneEEuropeStandardTime", "Ex906D9D", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_MyVoiceMail
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MyVoiceMail", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneMyanmarStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneMyanmarStandardTime", "ExAA522C", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingExplanationLogsDeleted
		{
			get
			{
				return new LocalizedString("TrackingExplanationLogsDeleted", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeOrgAdminDescription
		{
			get
			{
				return new LocalizedString("ExchangeOrgAdminDescription", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FopePolicyRuleContainsRecipientAddressAndRecipientDomainConditions(int ruleId)
		{
			return new LocalizedString("FopePolicyRuleContainsRecipientAddressAndRecipientDomainConditions", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId
			});
		}

		public static LocalizedString TimeZoneNepalStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneNepalStandardTime", "Ex3BEAAA", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FopePolicyRuleHasMaxRecipientsCondition(int ruleId, int maxRecipients)
		{
			return new LocalizedString("FopePolicyRuleHasMaxRecipientsCondition", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId,
				maxRecipients
			});
		}

		public static LocalizedString ClassIdProperty(int ruleId)
		{
			return new LocalizedString("ClassIdProperty", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId
			});
		}

		public static LocalizedString TimeZoneCenAustraliaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneCenAustraliaStandardTime", "Ex0396B1", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JobStatusFailed
		{
			get
			{
				return new LocalizedString("JobStatusFailed", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_TransportQueues
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_TransportQueues", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigratedFooterSizeExceedsDisclaimerMaxSize(string domain, string disclaimer, int actualSize, int maxSize)
		{
			return new LocalizedString("MigratedFooterSizeExceedsDisclaimerMaxSize", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				domain,
				disclaimer,
				actualSize,
				maxSize
			});
		}

		public static LocalizedString BodyExactMatchCaseSensitive(string body)
		{
			return new LocalizedString("BodyExactMatchCaseSensitive", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				body
			});
		}

		public static LocalizedString TimeZoneWestPacificStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneWestPacificStandardTime", "Ex631179", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingWarningNoResultsDueToTrackingTooEarly
		{
			get
			{
				return new LocalizedString("TrackingWarningNoResultsDueToTrackingTooEarly", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_ViewOnlyOrganizationManagement
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_ViewOnlyOrganizationManagement", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_ViewOnlyRecipients
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_ViewOnlyRecipients", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Allow
		{
			get
			{
				return new LocalizedString("Allow", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DomainScopeAlLDomains
		{
			get
			{
				return new LocalizedString("DomainScopeAlLDomains", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoValidDomainNameExistsInDomainSettings
		{
			get
			{
				return new LocalizedString("NoValidDomainNameExistsInDomainSettings", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventMovedToFolderByInboxRuleIW(string folderName)
		{
			return new LocalizedString("EventMovedToFolderByInboxRuleIW", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				folderName
			});
		}

		public static LocalizedString DistributionListEmpty(int ruleId, string distributionList)
		{
			return new LocalizedString("DistributionListEmpty", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId,
				distributionList
			});
		}

		public static LocalizedString ExchangeRecordsManagementDescription
		{
			get
			{
				return new LocalizedString("ExchangeRecordsManagementDescription", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneUTC08
		{
			get
			{
				return new LocalizedString("TimeZoneUTC08", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_NetworkingManagement
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_NetworkingManagement", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventTransferredToForeignOrgHelpDesk
		{
			get
			{
				return new LocalizedString("EventTransferredToForeignOrgHelpDesk", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_MyFacebookEnabled
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MyFacebookEnabled", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneCentralEuropeStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneCentralEuropeStandardTime", "ExC535AE", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_SecurityGroupCreationAndMembership
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_SecurityGroupCreationAndMembership", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeliveryStatusExpanded
		{
			get
			{
				return new LocalizedString("DeliveryStatusExpanded", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StatusPending
		{
			get
			{
				return new LocalizedString("StatusPending", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneMidAtlanticStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneMidAtlanticStandardTime", "Ex399DFA", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_ResetPassword
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_ResetPassword", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoValidDomainNameExistsInDomainScopedRule(int ruleId)
		{
			return new LocalizedString("NoValidDomainNameExistsInDomainScopedRule", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId
			});
		}

		public static LocalizedString InvalidRecipientForAdmin(string recipient)
		{
			return new LocalizedString("InvalidRecipientForAdmin", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				recipient
			});
		}

		public static LocalizedString ExchangeUMManagementDescription
		{
			get
			{
				return new LocalizedString("ExchangeUMManagementDescription", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneUTC
		{
			get
			{
				return new LocalizedString("TimeZoneUTC", "ExD2D4A1", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_DataLossPrevention
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_DataLossPrevention", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventPendingModerationIW
		{
			get
			{
				return new LocalizedString("EventPendingModerationIW", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_MyCustomApps
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MyCustomApps", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_DatabaseAvailabilityGroups
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_DatabaseAvailabilityGroups", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeHelpDeskDescription
		{
			get
			{
				return new LocalizedString("ExchangeHelpDeskDescription", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageDirectionReceived
		{
			get
			{
				return new LocalizedString("MessageDirectionReceived", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpamQuarantineMigrationSucceeded
		{
			get
			{
				return new LocalizedString("SpamQuarantineMigrationSucceeded", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventFailedModerationIW
		{
			get
			{
				return new LocalizedString("EventFailedModerationIW", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneUTC02
		{
			get
			{
				return new LocalizedString("TimeZoneUTC02", "ExAD5C35", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AntimalwareScopingConstraint
		{
			get
			{
				return new LocalizedString("AntimalwareScopingConstraint", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeliveryStatusFailed
		{
			get
			{
				return new LocalizedString("DeliveryStatusFailed", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuarantineInbound
		{
			get
			{
				return new LocalizedString("QuarantineInbound", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoValidIpRangesInFopeRule(int ruleId)
		{
			return new LocalizedString("NoValidIpRangesInFopeRule", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId
			});
		}

		public static LocalizedString CompressionError(int errCode)
		{
			return new LocalizedString("CompressionError", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				errCode
			});
		}

		public static LocalizedString RoleTypeDescription_Throttling
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_Throttling", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_DataCenterDestructiveOperations
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_DataCenterDestructiveOperations", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_AddressLists
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_AddressLists", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_CentralAdminManagement
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_CentralAdminManagement", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneAfghanistanStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneAfghanistanStandardTime", "Ex611A94", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventTransferredToForeignOrgIW
		{
			get
			{
				return new LocalizedString("EventTransferredToForeignOrgIW", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AttachmentExtensionContainsInvalidCharacters(int FopePolicyRuleId, string extension)
		{
			return new LocalizedString("AttachmentExtensionContainsInvalidCharacters", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				FopePolicyRuleId,
				extension
			});
		}

		public static LocalizedString JobStatusCancelled
		{
			get
			{
				return new LocalizedString("JobStatusCancelled", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubjectCaseSensitive(string subject)
		{
			return new LocalizedString("SubjectCaseSensitive", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				subject
			});
		}

		public static LocalizedString TimeZoneAtlanticStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneAtlanticStandardTime", "Ex611C88", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneArabStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneArabStandardTime", "ExBDF984", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_MailRecipients
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MailRecipients", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_WorkloadManagement
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_WorkloadManagement", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneAlaskanStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneAlaskanStandardTime", "ExE1F137", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingSearchException(LocalizedString reason)
		{
			return new LocalizedString("TrackingSearchException", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString MsoManagedTenantHelpdeskGroupDescription
		{
			get
			{
				return new LocalizedString("MsoManagedTenantHelpdeskGroupDescription", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DecompressionError(int errCode)
		{
			return new LocalizedString("DecompressionError", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				errCode
			});
		}

		public static LocalizedString AntimalwareInboundRecipientNotifications
		{
			get
			{
				return new LocalizedString("AntimalwareInboundRecipientNotifications", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TestXHeaderAndModifySubject
		{
			get
			{
				return new LocalizedString("TestXHeaderAndModifySubject", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContentIndexStatusFailedAndSuspended
		{
			get
			{
				return new LocalizedString("ContentIndexStatusFailedAndSuspended", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MsoManagedTenantAdminGroupDescription
		{
			get
			{
				return new LocalizedString("MsoManagedTenantAdminGroupDescription", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_DataCenterOperations
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_DataCenterOperations", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventModerationExpired
		{
			get
			{
				return new LocalizedString("EventModerationExpired", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DomainLevelAdminNotSupportedByEOP(string userName, string roleNames)
		{
			return new LocalizedString("DomainLevelAdminNotSupportedByEOP", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				userName,
				roleNames
			});
		}

		public static LocalizedString TraceLevelMedium
		{
			get
			{
				return new LocalizedString("TraceLevelMedium", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_MoveMailboxes
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MoveMailboxes", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_MailRecipientCreation
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MailRecipientCreation", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneLineIslandsStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneLineIslandsStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingIdentityParameter
		{
			get
			{
				return new LocalizedString("MissingIdentityParameter", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContentIndexStatusHealthyAndUpgrading
		{
			get
			{
				return new LocalizedString("ContentIndexStatusHealthyAndUpgrading", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventTransferredToLegacyExchangeServerHelpDesk(string local, string remote)
		{
			return new LocalizedString("EventTransferredToLegacyExchangeServerHelpDesk", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				local,
				remote
			});
		}

		public static LocalizedString TrafficScopeInbound
		{
			get
			{
				return new LocalizedString("TrafficScopeInbound", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_UnScopedRoleManagement
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_UnScopedRoleManagement", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AntimalwareAdminAddressValidationsScoped(string invalidSmtpAddress, string policyName)
		{
			return new LocalizedString("AntimalwareAdminAddressValidationsScoped", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				invalidSmtpAddress,
				policyName
			});
		}

		public static LocalizedString AuditSeverityLevelDoNotAudit
		{
			get
			{
				return new LocalizedString("AuditSeverityLevelDoNotAudit", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeHygieneManagementDescription
		{
			get
			{
				return new LocalizedString("ExchangeHygieneManagementDescription", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClassIdExtensions
		{
			get
			{
				return new LocalizedString("ClassIdExtensions", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneNamibiaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneNamibiaStandardTime", "ExEBD3D6", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RejectedExplanationContentFiltering
		{
			get
			{
				return new LocalizedString("RejectedExplanationContentFiltering", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuarantineOutbound
		{
			get
			{
				return new LocalizedString("QuarantineOutbound", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RunWeekly
		{
			get
			{
				return new LocalizedString("RunWeekly", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneAltaiStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneAltaiStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventSubmittedCrossSite
		{
			get
			{
				return new LocalizedString("EventSubmittedCrossSite", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneEasternStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneEasternStandardTime", "Ex469AF3", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DisabledOutboundConnector(string name)
		{
			return new LocalizedString("DisabledOutboundConnector", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString BodyCondition(int ruleId)
		{
			return new LocalizedString("BodyCondition", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId
			});
		}

		public static LocalizedString TimeZoneAzerbaijanStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneAzerbaijanStandardTime", "ExDED1C0", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FopePolicyRuleContainsIncompatibleConditions(int ruleId)
		{
			return new LocalizedString("FopePolicyRuleContainsIncompatibleConditions", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId
			});
		}

		public static LocalizedString PlusUnknownTimeZone(int hour, int minute)
		{
			return new LocalizedString("PlusUnknownTimeZone", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				hour,
				minute
			});
		}

		public static LocalizedString TimeZonePakistanStandardTime
		{
			get
			{
				return new LocalizedString("TimeZonePakistanStandardTime", "ExC176EA", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_OrgMarketplaceApps
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_OrgMarketplaceApps", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZonePacificSAStandardTime
		{
			get
			{
				return new LocalizedString("TimeZonePacificSAStandardTime", "Ex16C4A1", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneRussianStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneRussianStandardTime", "Ex5D4EC1", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_POP3AndIMAP4Protocols
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_POP3AndIMAP4Protocols", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAttachmentExtensionCondition(int ruleId)
		{
			return new LocalizedString("InvalidAttachmentExtensionCondition", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId
			});
		}

		public static LocalizedString TimeZoneTaipeiStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneTaipeiStandardTime", "ExF4738A", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_HelpdeskRecipientManagement
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_HelpdeskRecipientManagement", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneHawaiianStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneHawaiianStandardTime", "ExD9A3CB", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneMagallanesStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneMagallanesStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventFailedTransportRulesHelpDesk
		{
			get
			{
				return new LocalizedString("EventFailedTransportRulesHelpDesk", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneTokyoStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneTokyoStandardTime", "ExB8F00C", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeliveryStatusAll
		{
			get
			{
				return new LocalizedString("DeliveryStatusAll", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_MyOptions
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MyOptions", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FopePolicyRuleHasProhibitedRegularExpressions(int ruleId, string reason)
		{
			return new LocalizedString("FopePolicyRuleHasProhibitedRegularExpressions", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId,
				reason
			});
		}

		public static LocalizedString RoleTypeDescription_DisasterRecovery
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_DisasterRecovery", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_EmailAddressPolicies
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_EmailAddressPolicies", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HeaderValueMatch(string headerValue)
		{
			return new LocalizedString("HeaderValueMatch", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				headerValue
			});
		}

		public static LocalizedString RoleTypeDescription_SendConnectors
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_SendConnectors", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FopePolicyRuleSummary(int RuleId, string trafficScope, string domainScope, string action, string details)
		{
			return new LocalizedString("FopePolicyRuleSummary", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				RuleId,
				trafficScope,
				domainScope,
				action,
				details
			});
		}

		public static LocalizedString BCC(string to)
		{
			return new LocalizedString("BCC", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				to
			});
		}

		public static LocalizedString TimeZoneSudanStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneSudanStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventRead
		{
			get
			{
				return new LocalizedString("EventRead", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneGreenlandStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneGreenlandStandardTime", "ExDE17E6", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_OutlookProvider
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_OutlookProvider", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventTransferredIntermediate
		{
			get
			{
				return new LocalizedString("EventTransferredIntermediate", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_OrgCustomApps
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_OrgCustomApps", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneBangladeshStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneBangladeshStandardTime", "ExF23691", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BodyExactMatch(string body)
		{
			return new LocalizedString("BodyExactMatch", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				body
			});
		}

		public static LocalizedString TimeZoneGMTStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneGMTStandardTime", "Ex35C612", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneGTBStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneGTBStandardTime", "Ex96BA20", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JobStatusNotStarted
		{
			get
			{
				return new LocalizedString("JobStatusNotStarted", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TenantSkuNotSupported(string skuName)
		{
			return new LocalizedString("TenantSkuNotSupported", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				skuName
			});
		}

		public static LocalizedString TrackingWarningNoResultsDueToLogsNotFound
		{
			get
			{
				return new LocalizedString("TrackingWarningNoResultsDueToLogsNotFound", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubjectExactMatch(string subject)
		{
			return new LocalizedString("SubjectExactMatch", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				subject
			});
		}

		public static LocalizedString TimeZoneSAWesternStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneSAWesternStandardTime", "ExCD3627", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FopePolicyRuleIsPartialMessage(int ruleId)
		{
			return new LocalizedString("FopePolicyRuleIsPartialMessage", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId
			});
		}

		public static LocalizedString TimeZoneHaitiStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneHaitiStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingErrorFailedToInitialize
		{
			get
			{
				return new LocalizedString("TrackingErrorFailedToInitialize", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneKamchatkaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneKamchatkaStandardTime", "ExD1FCF5", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingWarningResultsMissingTransient
		{
			get
			{
				return new LocalizedString("TrackingWarningResultsMissingTransient", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RulesMerged
		{
			get
			{
				return new LocalizedString("RulesMerged", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AntimalwareVirtualDomainFailure(string invalidVirtualDomain)
		{
			return new LocalizedString("AntimalwareVirtualDomainFailure", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				invalidVirtualDomain
			});
		}

		public static LocalizedString RoleTypeDescription_MyRetentionPolicies
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MyRetentionPolicies", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DomainScopedRulesMerged
		{
			get
			{
				return new LocalizedString("DomainScopedRulesMerged", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventResolvedHelpDesk
		{
			get
			{
				return new LocalizedString("EventResolvedHelpDesk", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_LegalHoldApplication
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_LegalHoldApplication", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneSingaporeStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneSingaporeStandardTime", "Ex85CE3C", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AntimalwareAdminAddressNull
		{
			get
			{
				return new LocalizedString("AntimalwareAdminAddressNull", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_UserApplication
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_UserApplication", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboundConnectorWithoutSenderIPsAndCert(string name)
		{
			return new LocalizedString("InboundConnectorWithoutSenderIPsAndCert", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString RoleTypeDescription_MyContactInformation
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MyContactInformation", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneNorthAsiaEastStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneNorthAsiaEastStandardTime", "ExA847F9", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeRecipientAdminDescription
		{
			get
			{
				return new LocalizedString("ExchangeRecipientAdminDescription", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneCapeVerdeStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneCapeVerdeStandardTime", "ExEBA757", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingSearchNotAuthorized
		{
			get
			{
				return new LocalizedString("TrackingSearchNotAuthorized", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContentIndexStatusCrawling
		{
			get
			{
				return new LocalizedString("ContentIndexStatusCrawling", "ExBFB72B", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneMagadanStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneMagadanStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneAUSCentralStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneAUSCentralStandardTime", "Ex3FEDDD", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeDiscoveryManagementDescription
		{
			get
			{
				return new LocalizedString("ExchangeDiscoveryManagementDescription", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventMessageDefer
		{
			get
			{
				return new LocalizedString("EventMessageDefer", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneVolgogradStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneVolgogradStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FilenameWordMatchCondition(int ruleId)
		{
			return new LocalizedString("FilenameWordMatchCondition", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId
			});
		}

		public static LocalizedString TimeZonePacificStandardTimeMexico
		{
			get
			{
				return new LocalizedString("TimeZonePacificStandardTimeMexico", "Ex7BA959", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AttachmentFilenames(string attachmentFilenames)
		{
			return new LocalizedString("AttachmentFilenames", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				attachmentFilenames
			});
		}

		public static LocalizedString TimeZoneSamoaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneSamoaStandardTime", "Ex02633F", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_PersonallyIdentifiableInformation
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_PersonallyIdentifiableInformation", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneUTC12
		{
			get
			{
				return new LocalizedString("TimeZoneUTC12", "ExEEDEA2", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_MessageTracking
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MessageTracking", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TraceLevelLow
		{
			get
			{
				return new LocalizedString("TraceLevelLow", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_MyTextMessaging
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MyTextMessaging", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_RecipientPolicies
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_RecipientPolicies", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneSAEasternStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneSAEasternStandardTime", "Ex2B0B5B", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventSubmittedCrossSiteHelpDesk(string hubServerFqdn)
		{
			return new LocalizedString("EventSubmittedCrossSiteHelpDesk", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				hubServerFqdn
			});
		}

		public static LocalizedString RoleTypeDescription_TeamMailboxLifecycleApplication
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_TeamMailboxLifecycleApplication", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboundFopePolicyRuleWithDuplicateDomainName(int ruleId)
		{
			return new LocalizedString("InboundFopePolicyRuleWithDuplicateDomainName", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId
			});
		}

		public static LocalizedString InvalidSecondaryEmailAddresses(string userName)
		{
			return new LocalizedString("InvalidSecondaryEmailAddresses", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString InvalidMessageTrackingReportId
		{
			get
			{
				return new LocalizedString("InvalidMessageTrackingReportId", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneIndiaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneIndiaStandardTime", "ExEB2050", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DistributionListExpanded(int ruleId, string distributionList)
		{
			return new LocalizedString("DistributionListExpanded", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId,
				distributionList
			});
		}

		public static LocalizedString ContentIndexStatusFailed
		{
			get
			{
				return new LocalizedString("ContentIndexStatusFailed", "Ex0C6286", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneNewfoundlandStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneNewfoundlandStandardTime", "ExCAC12E", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneYakutskStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneYakutskStandardTime", "ExD95910", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoValidRecipientDomainNameExistsInRecipientDomainCondition(int ruleId)
		{
			return new LocalizedString("NoValidRecipientDomainNameExistsInRecipientDomainCondition", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId
			});
		}

		public static LocalizedString RoleTypeDescription_OrganizationHelpSettings
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_OrganizationHelpSettings", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FopePolicyConsolidationList(string ruleName, string fopeIds)
		{
			return new LocalizedString("FopePolicyConsolidationList", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleName,
				fopeIds
			});
		}

		public static LocalizedString EventFailedModerationHelpDesk
		{
			get
			{
				return new LocalizedString("EventFailedModerationHelpDesk", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_PublicFolders
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_PublicFolders", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZonePacificStandardTime
		{
			get
			{
				return new LocalizedString("TimeZonePacificStandardTime", "ExC6C6F0", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneAleutianStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneAleutianStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeManagementForestTier1SupportDescription
		{
			get
			{
				return new LocalizedString("ExchangeManagementForestTier1SupportDescription", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneNorthKoreaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneNorthKoreaStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DltUnknownTimeZone
		{
			get
			{
				return new LocalizedString("DltUnknownTimeZone", "Ex6DAA38", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_ExchangeConnectors
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_ExchangeConnectors", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_ViewOnlyCentralAdminSupport
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_ViewOnlyCentralAdminSupport", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneNorfolkStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneNorfolkStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_MyProfileInformation
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MyProfileInformation", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneSaoTomeStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneSaoTomeStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AttachmentExtensions(string attachmentExtensions)
		{
			return new LocalizedString("AttachmentExtensions", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				attachmentExtensions
			});
		}

		public static LocalizedString RoleTypeDescription_Journaling
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_Journaling", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneAzoresStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneAzoresStandardTime", "Ex15D215", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneAusCentralWStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneAusCentralWStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneUSMountainStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneUSMountainStandardTime", "Ex48C151", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_ExchangeCrossServiceIntegration
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_ExchangeCrossServiceIntegration", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PolicyMigrationSucceeded
		{
			get
			{
				return new LocalizedString("PolicyMigrationSucceeded", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneCentralBrazilianStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneCentralBrazilianStandardTime", "Ex10A588", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventRulesCc
		{
			get
			{
				return new LocalizedString("EventRulesCc", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MaximumRecipientNumber(int maxRecipients)
		{
			return new LocalizedString("MaximumRecipientNumber", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				maxRecipients
			});
		}

		public static LocalizedString EventDeliveredInboxRule(string folderName)
		{
			return new LocalizedString("EventDeliveredInboxRule", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				folderName
			});
		}

		public static LocalizedString ComplianceManagementGroupDescription
		{
			get
			{
				return new LocalizedString("ComplianceManagementGroupDescription", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneWCentralAfricaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneWCentralAfricaStandardTime", "Ex87B651", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_UMMailboxes
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_UMMailboxes", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeAllHostedOrgsDescription
		{
			get
			{
				return new LocalizedString("ExchangeAllHostedOrgsDescription", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_DistributionGroups
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_DistributionGroups", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeDelegatedSetupDescription
		{
			get
			{
				return new LocalizedString("ExchangeDelegatedSetupDescription", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneVladivostokStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneVladivostokStandardTime", "Ex15317C", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneCentralStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneCentralStandardTime", "ExFA79C8", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_OrganizationClientAccess
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_OrganizationClientAccess", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeViewOnlyAdminDescription
		{
			get
			{
				return new LocalizedString("ExchangeViewOnlyAdminDescription", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneEAustraliaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneEAustraliaStandardTime", "ExF6E116", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DomainScopedRuleContainsInvalidDomainNames(int ruleId, string domainNames)
		{
			return new LocalizedString("DomainScopedRuleContainsInvalidDomainNames", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId,
				domainNames
			});
		}

		public static LocalizedString InvalidDomainNameInConnectorSetting(string invalidDomainName)
		{
			return new LocalizedString("InvalidDomainNameInConnectorSetting", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				invalidDomainName
			});
		}

		public static LocalizedString OutboundDomainScopedConnectorsMigrated(string connectors)
		{
			return new LocalizedString("OutboundDomainScopedConnectorsMigrated", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				connectors
			});
		}

		public static LocalizedString TenantSkuFilteringNotSupported(string skuName)
		{
			return new LocalizedString("TenantSkuFilteringNotSupported", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				skuName
			});
		}

		public static LocalizedString FopePolicyRuleHasWordsThatExceedMaximumLength(int ruleId, int maxWordLength)
		{
			return new LocalizedString("FopePolicyRuleHasWordsThatExceedMaximumLength", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId,
				maxWordLength
			});
		}

		public static LocalizedString MinusUnknownTimeZone(int hour, int minute)
		{
			return new LocalizedString("MinusUnknownTimeZone", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				hour,
				minute
			});
		}

		public static LocalizedString AntimalwareTruncation(string longMessage, string domainName)
		{
			return new LocalizedString("AntimalwareTruncation", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				longMessage,
				domainName
			});
		}

		public static LocalizedString TrackingTransientError
		{
			get
			{
				return new LocalizedString("TrackingTransientError", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrafficScopeInboundAndOutbound
		{
			get
			{
				return new LocalizedString("TrafficScopeInboundAndOutbound", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExamineArchivesProperty(int ruleId)
		{
			return new LocalizedString("ExamineArchivesProperty", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId
			});
		}

		public static LocalizedString RoleTypeDescription_CentralAdminCredentialManagement
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_CentralAdminCredentialManagement", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_UnScoped
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_UnScoped", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_MyDistributionGroupMembership
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MyDistributionGroupMembership", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_OfficeExtensionApplication
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_OfficeExtensionApplication", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_DistributionGroupManagement
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_DistributionGroupManagement", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AntimalwareMigrationSucceeded
		{
			get
			{
				return new LocalizedString("AntimalwareMigrationSucceeded", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneWEuropeStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneWEuropeStandardTime", "ExB33E7A", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_AuditLogs
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_AuditLogs", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventSubmitted
		{
			get
			{
				return new LocalizedString("EventSubmitted", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneEasternStandardTimeMexico
		{
			get
			{
				return new LocalizedString("TimeZoneEasternStandardTimeMexico", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneRomanceStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneRomanceStandardTime", "Ex5E5458", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDomainNameInHipaaDomainSetting(string invalidDomainName)
		{
			return new LocalizedString("InvalidDomainNameInHipaaDomainSetting", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				invalidDomainName
			});
		}

		public static LocalizedString UnableToAddUserToDistributionGroup(string user, string dg, string error)
		{
			return new LocalizedString("UnableToAddUserToDistributionGroup", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				user,
				dg,
				error
			});
		}

		public static LocalizedString TimeZoneIranStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneIranStandardTime", "Ex5D6DCC", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_AccessToCustomerDataDCOnly
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_AccessToCustomerDataDCOnly", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Redirect(string to)
		{
			return new LocalizedString("Redirect", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				to
			});
		}

		public static LocalizedString InvalidIdentityForAdmin
		{
			get
			{
				return new LocalizedString("InvalidIdentityForAdmin", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneMarquesasStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneMarquesasStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContentIndexStatusHealthy
		{
			get
			{
				return new LocalizedString("ContentIndexStatusHealthy", "ExAE6F15", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RunMonthly
		{
			get
			{
				return new LocalizedString("RunMonthly", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_OrganizationTransportSettings
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_OrganizationTransportSettings", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_ViewOnlyConfiguration
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_ViewOnlyConfiguration", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HipaaPolicyMigrated(string domains)
		{
			return new LocalizedString("HipaaPolicyMigrated", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				domains
			});
		}

		public static LocalizedString TimeZoneMountainStandardTimeMexico
		{
			get
			{
				return new LocalizedString("TimeZoneMountainStandardTimeMexico", "Ex7226F6", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_PartnerDelegatedTenantManagement
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_PartnerDelegatedTenantManagement", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventResolvedWithDetailsHelpDesk(string originalAddress, string resolvedAddress)
		{
			return new LocalizedString("EventResolvedWithDetailsHelpDesk", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				originalAddress,
				resolvedAddress
			});
		}

		public static LocalizedString RoleTypeDescription_OrganizationConfiguration
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_OrganizationConfiguration", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MsoMailTenantAdminGroupDescription
		{
			get
			{
				return new LocalizedString("MsoMailTenantAdminGroupDescription", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneSEAsiaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneSEAsiaStandardTime", "ExBCFCCD", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_DatabaseCopies
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_DatabaseCopies", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneCentralStandardTimeMexico
		{
			get
			{
				return new LocalizedString("TimeZoneCentralStandardTimeMexico", "Ex0243BD", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OutboundIpMigrationCompleted
		{
			get
			{
				return new LocalizedString("OutboundIpMigrationCompleted", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecipientPathFilterNeeded
		{
			get
			{
				return new LocalizedString("RecipientPathFilterNeeded", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingWarningResultsMissingConnection
		{
			get
			{
				return new LocalizedString("TrackingWarningResultsMissingConnection", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboundConnectorsWithSpamOrConnectionFilteringDisabledMigrated(string connectors)
		{
			return new LocalizedString("InboundConnectorsWithSpamOrConnectionFilteringDisabledMigrated", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				connectors
			});
		}

		public static LocalizedString SenderDomainConditionContainsInvalidDomainNames(int ruleId, string domainNames)
		{
			return new LocalizedString("SenderDomainConditionContainsInvalidDomainNames", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId,
				domainNames
			});
		}

		public static LocalizedString TimeZoneNCentralAsiaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneNCentralAsiaStandardTime", "Ex18B2E7", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PolicyTipReject
		{
			get
			{
				return new LocalizedString("PolicyTipReject", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AuditSeverityLevelLow
		{
			get
			{
				return new LocalizedString("AuditSeverityLevelLow", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_RecipientManagement
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_RecipientManagement", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventForwarded
		{
			get
			{
				return new LocalizedString("EventForwarded", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventApprovedModerationHelpDesk
		{
			get
			{
				return new LocalizedString("EventApprovedModerationHelpDesk", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Reject
		{
			get
			{
				return new LocalizedString("Reject", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneSaintPierreStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneSaintPierreStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneMoroccoStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneMoroccoStandardTime", "Ex106F0F", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FopePolicyRuleIsSkippableAntiSpamRule(int ruleId)
		{
			return new LocalizedString("FopePolicyRuleIsSkippableAntiSpamRule", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId
			});
		}

		public static LocalizedString TimeZoneFLEStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneFLEStandardTime", "Ex68ED89", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_MailTips
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MailTips", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_Monitoring
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_Monitoring", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoValidSenderDomainNameExistsInSenderDomainCondition(int ruleId)
		{
			return new LocalizedString("NoValidSenderDomainNameExistsInSenderDomainCondition", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId
			});
		}

		public static LocalizedString AntispamEdgeBlockModeSettingNotMigrated
		{
			get
			{
				return new LocalizedString("AntispamEdgeBlockModeSettingNotMigrated", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneCanadaCentralStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneCanadaCentralStandardTime", "Ex4FC02A", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeManagementForestMonitoringDescription
		{
			get
			{
				return new LocalizedString("ExchangeManagementForestMonitoringDescription", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneSakhalinStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneSakhalinStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventExpanded(string groupName)
		{
			return new LocalizedString("EventExpanded", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				groupName
			});
		}

		public static LocalizedString FopePolicyRuleHasUnrecognizedAction(int ruleId, int actionId)
		{
			return new LocalizedString("FopePolicyRuleHasUnrecognizedAction", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId,
				actionId
			});
		}

		public static LocalizedString TimeZoneSriLankaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneSriLankaStandardTime", "ExD87835", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_AutoDiscover
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_AutoDiscover", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneRussiaTimeZone10
		{
			get
			{
				return new LocalizedString("TimeZoneRussiaTimeZone10", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventSmtpSendHelpDesk(string local, string remote)
		{
			return new LocalizedString("EventSmtpSendHelpDesk", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				local,
				remote
			});
		}

		public static LocalizedString SenderIpAddresses(string senderIpAddresses)
		{
			return new LocalizedString("SenderIpAddresses", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				senderIpAddresses
			});
		}

		public static LocalizedString EventQueueRetryHelpDesk(string server, string inRetrySinceTime, string lastAttemptTime, string timeZone, string errorMessage)
		{
			return new LocalizedString("EventQueueRetryHelpDesk", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				server,
				inRetrySinceTime,
				lastAttemptTime,
				timeZone,
				errorMessage
			});
		}

		public static LocalizedString EventTransferredToSMSMessage
		{
			get
			{
				return new LocalizedString("EventTransferredToSMSMessage", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingWarningNoResultsDueToLogsExpired
		{
			get
			{
				return new LocalizedString("TrackingWarningNoResultsDueToLogsExpired", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageDirectionSent
		{
			get
			{
				return new LocalizedString("MessageDirectionSent", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HeaderNameMatch(string headerName)
		{
			return new LocalizedString("HeaderNameMatch", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				headerName
			});
		}

		public static LocalizedString EventDelayedAfterTransferToPartnerOrgHelpDesk(string partnerOrgDomain)
		{
			return new LocalizedString("EventDelayedAfterTransferToPartnerOrgHelpDesk", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				partnerOrgDomain
			});
		}

		public static LocalizedString RoleTypeDescription_RecordsManagement
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_RecordsManagement", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_TransportRules
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_TransportRules", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ForceTls
		{
			get
			{
				return new LocalizedString("ForceTls", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventFailedGeneral
		{
			get
			{
				return new LocalizedString("EventFailedGeneral", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleTypeDescription_ExchangeServerCertificates
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_ExchangeServerCertificates", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventPending
		{
			get
			{
				return new LocalizedString("EventPending", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneChinaStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneChinaStandardTime", "ExE7D7D4", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneWestBankStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneWestBankStandardTime", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Description(string description)
		{
			return new LocalizedString("Description", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				description
			});
		}

		public static LocalizedString InboundConnectorsWithPolicyFilteringDisabledMigrated(string connectors)
		{
			return new LocalizedString("InboundConnectorsWithPolicyFilteringDisabledMigrated", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				connectors
			});
		}

		public static LocalizedString RoleTypeDescription_MyBaseOptions
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_MyBaseOptions", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneGreenwichStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneGreenwichStandardTime", "ExB5297B", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventPendingAfterTransferToPartnerOrgHelpDesk(string partnerOrgDomain)
		{
			return new LocalizedString("EventPendingAfterTransferToPartnerOrgHelpDesk", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				partnerOrgDomain
			});
		}

		public static LocalizedString RoleTypeDescription_UMPromptManagement
		{
			get
			{
				return new LocalizedString("RoleTypeDescription_UMPromptManagement", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AuditSeverityLevelMedium
		{
			get
			{
				return new LocalizedString("AuditSeverityLevelMedium", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StatusFilterCannotBeSpecified
		{
			get
			{
				return new LocalizedString("StatusFilterCannotBeSpecified", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContentIndexStatusSuspended
		{
			get
			{
				return new LocalizedString("ContentIndexStatusSuspended", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneUTC13
		{
			get
			{
				return new LocalizedString("TimeZoneUTC13", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PolicyTipUrl
		{
			get
			{
				return new LocalizedString("PolicyTipUrl", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DomainNameTruncated(string domainName, string truncatedDomainName)
		{
			return new LocalizedString("DomainNameTruncated", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				domainName,
				truncatedDomainName
			});
		}

		public static LocalizedString JobStatusDone
		{
			get
			{
				return new LocalizedString("JobStatusDone", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SenderAddress(string senderAddress)
		{
			return new LocalizedString("SenderAddress", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				senderAddress
			});
		}

		public static LocalizedString Quarantine
		{
			get
			{
				return new LocalizedString("Quarantine", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingWarningResultsMissingFatal
		{
			get
			{
				return new LocalizedString("TrackingWarningResultsMissingFatal", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BodyExactMatchCondition(int ruleId)
		{
			return new LocalizedString("BodyExactMatchCondition", "", false, false, CoreStrings.ResourceManager, new object[]
			{
				ruleId
			});
		}

		public static LocalizedString TimeZoneNewZealandStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneNewZealandStandardTime", "Ex2E0FE2", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExecutableAttachments
		{
			get
			{
				return new LocalizedString("ExecutableAttachments", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneDatelineStandardTime
		{
			get
			{
				return new LocalizedString("TimeZoneDatelineStandardTime", "ExCC54E8", false, true, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventQueueRetryIW
		{
			get
			{
				return new LocalizedString("EventQueueRetryIW", "", false, false, CoreStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(CoreStrings.IDs key)
		{
			return new LocalizedString(CoreStrings.stringIDs[(uint)key], CoreStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(424);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Core.CoreStrings", typeof(CoreStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			RoleTypeDescription_RemoteAndAcceptedDomains = 2092707270U,
			TimeZoneSouthAfricaStandardTime = 1886101956U,
			PolicyTipNotifyOnly = 3906704818U,
			TimeZoneLibyaStandardTime = 97673562U,
			ReportSeverityLow = 1829747073U,
			RoleTypeDescription_ArchiveApplication = 2344870457U,
			EventDelivered = 3472506566U,
			AntispamIPBlockListSettingMigrated = 3720562738U,
			RoleTypeDescription_ActiveMonitoring = 327230581U,
			RoleTypeDescription_UMPrompts = 2962389050U,
			TimeZoneCentralAsiaStandardTime = 146865784U,
			TimeZoneESouthAmericaStandardTime = 146778149U,
			TimeZoneMountainStandardTime = 80585286U,
			TimeZoneKaliningradStandardTime = 1446960263U,
			ExchangeViewOnlyManagementForestOperatorDescription = 137362532U,
			AntispamMigrationSucceeded = 3151979146U,
			TimeZoneNorthAsiaStandardTime = 1683344478U,
			InvalidSender = 89789448U,
			RoleTypeDescription_MyTeamMailboxes = 1221420638U,
			RoleTypeDescription_MyDistributionGroups = 2027768067U,
			TimeZoneFijiStandardTime = 1738495689U,
			TimeZoneEAfricaStandardTime = 1010967094U,
			NotAuthorizedToViewRecipientPath = 498311706U,
			RoleTypeDescription_UserOptions = 2847306432U,
			RoleTypeDescription_UmRecipientManagement = 3322063993U,
			ReportSeverityHigh = 1029644363U,
			RoleTypeDescription_CmdletExtensionAgents = 2763281369U,
			AntimalwareUserOptOut = 2874223383U,
			RoleTypeDescription_MyLogon = 2736802282U,
			ContentIndexStatusDisabled = 2385442291U,
			PolicyTipRejectOverride = 466036006U,
			StatusDelivered = 1875295180U,
			TimeZoneWMongoliaStandardTime = 1255891408U,
			RoleTypeDescription_TransportHygiene = 872336955U,
			TraceLevelHigh = 112955817U,
			RoleTypeDescription_InformationRightsManagement = 4100231581U,
			TestModifySubject = 1461929024U,
			TimeZoneLordHoweStandardTime = 2652353769U,
			InvalidSenderForAdmin = 1319959286U,
			RoleTypeDescription_DiscoveryManagement = 1633693746U,
			TimeZoneTurksAndCaicosStandardTime = 3964643789U,
			RoleTypeDescription_MailboxExport = 2230053377U,
			AntispamIPAllowListSettingMigrated = 3106057138U,
			ExchangeManagementForestOperatorDescription = 4036159751U,
			TimeZoneArabianStandardTime = 2575879303U,
			TimeZoneIsraelStandardTime = 2009673553U,
			RoleTypeDescription_ActiveDirectoryPermissions = 4146612654U,
			RunOnce = 3378761982U,
			MessageDirectionAll = 4217676503U,
			RoleTypeDescription_UmManagement = 2281308004U,
			AuditSeverityLevelHigh = 2385887948U,
			TimeZoneArgentinaStandardTime = 1680752826U,
			RoleTypeDescription_Migration = 2184195641U,
			TimeZoneUlaanbaatarStandardTime = 1406395995U,
			TimeZoneSaratovStandardTime = 1799519309U,
			TimeZoneVenezuelaStandardTime = 1545851488U,
			ExchangeAllMailboxesDescription = 1103744372U,
			RoleTypeDescription_GALSynchronizationManagement = 926819582U,
			RoleTypeDescription_ViewOnlyRoleManagement = 3690262447U,
			RoleTypeDescription_LiveID = 683655822U,
			RoleTypeDescription_TransportAgents = 3406878936U,
			RoleTypeDescription_MyDiagnostics = 2562309877U,
			TrackingWarningNoResultsDueToUntrackableMessagePath = 411395545U,
			EventTransferredToLegacyExchangeServer = 4221322756U,
			RoleTypeDescription_TeamMailboxes = 1998960776U,
			TimeZoneSAPacificStandardTime = 1130640660U,
			ExchangeServerManagementDescription = 1318716879U,
			UnsupportedSenderForTracking = 74443346U,
			TimeZoneMontevideoStandardTime = 2164669709U,
			TimeZoneUSEasternStandardTime = 2912446405U,
			EventApprovedModerationIW = 517696069U,
			TimeZoneCentralAmericaStandardTime = 103080662U,
			ContentIndexStatusAutoSuspended = 393808047U,
			TimeZoneBahiaStandardTime = 2501174206U,
			RoleTypeDescription_EdgeSubscriptions = 2005680638U,
			TimeZoneCubaStandardTime = 3905123624U,
			TimeZoneEasterIslandStandardTime = 3492975378U,
			JobStatusInProgress = 3708464597U,
			TimeZoneUTC09 = 2294597162U,
			TimeZoneEgyptStandardTime = 2993499200U,
			StatusRead = 3231667300U,
			TimeZoneCaucasusStandardTime = 2740570033U,
			RoleTypeDescription_MailEnabledPublicFolders = 4120033037U,
			RoleTypeDescription_ViewOnlyAuditLogs = 4113945646U,
			TrackingWarningReadStatusUnavailable = 576007077U,
			ViewOnlyPIIGroupDescription = 176786758U,
			RoleTypeDescription_FederatedSharing = 2073899161U,
			RoleTypeDescription_RoleManagement = 3390444882U,
			TrackingExplanationNormalTimeSpan = 2903872252U,
			RoleTypeDescription_ExchangeServers = 2937865572U,
			TimeZoneAstrakhanStandardTime = 641655538U,
			ReportSeverityMedium = 840197340U,
			RoleTypeDescription_SupportDiagnostics = 424676612U,
			TimeZoneCentralEuropeanStandardTime = 2220779553U,
			TimeZoneKoreaStandardTime = 1476061287U,
			ContentIndexStatusSeeding = 1361373610U,
			TimeZoneWAustraliaStandardTime = 3998935900U,
			RoleTypeDescription_Custom = 3209229526U,
			AntispamActionTypeSettingMigrated = 390519148U,
			TrackingExplanationExcessiveTimeSpan = 559475236U,
			QuarantineSpam = 2416995053U,
			TimeZoneUTC11 = 728513229U,
			RoleTypeDescription_PublicFolderReplication = 1698995718U,
			Encrypt = 3202170171U,
			TimeZoneRussiaTimeZone11 = 1562661243U,
			TimeZoneEkaterinburgStandardTime = 875871474U,
			RoleTypeDescription_LegalHold = 3860979609U,
			TimeZoneTocantinsStandardTime = 50740234U,
			TimeZoneArabicStandardTime = 2428028299U,
			RoleTypeDescription_MailboxImportExport = 1011790132U,
			RoleTypeDescription_Supervision = 4065087132U,
			RoleTypeDescription_LawEnforcementRequests = 937823105U,
			RoleTypeDescription_MailboxSearchApplication = 851475441U,
			RoleTypeDescription_RetentionManagement = 20620266U,
			TimeZoneWestAsiaStandardTime = 937651576U,
			QuarantineTransportRule = 1727765283U,
			RoleTypeDescription_ViewOnlyCentralAdminManagement = 3421391191U,
			EventFailedTransportRulesIW = 2151396155U,
			EventDelayedAfterTransferToPartnerOrgIW = 199727676U,
			RoleTypeDescription_DatacenterOperationsDCOnly = 1348529911U,
			TimeZoneTomskStandardTime = 4089915379U,
			TrackingBusy = 3099813970U,
			TimeZoneTongaStandardTime = 1728005806U,
			TimeZoneTasmaniaStandardTime = 3917786073U,
			ExchangePublicFolderAdminDescription = 2471410929U,
			TrafficScopeOutbound = 1018209439U,
			EventForwardedToDelegateAndDeleted = 2309256410U,
			RoleTypeDescription_GALSynchronization = 3851963997U,
			CompressionOutOfMemory = 1747500934U,
			TrafficScopeDisabled = 1069292955U,
			RoleTypeDescription_OrganizationManagement = 1969466653U,
			TimeZoneOmskStandardTime = 1654105079U,
			TimeZoneBelarusStandardTime = 1984558815U,
			TimeZoneParaguayStandardTime = 3386037373U,
			RoleTypeDescription_Reporting = 1198640795U,
			TimeZoneChathamIslandsStandardTime = 3103556173U,
			RoleTypeDescription_MyMailboxDelegation = 1660783915U,
			RoleTypeDescription_ExchangeVirtualDirectories = 397099290U,
			TimeZoneAUSEasternStandardTime = 3213863256U,
			EventNotRead = 3256513739U,
			TimeZoneMiddleEastStandardTime = 463534759U,
			RoleTypeDescription_ApplicationImpersonation = 907728851U,
			TrackingWarningTooManyEvents = 2979872069U,
			ContentIndexStatusUnknown = 1631091055U,
			TimeZoneSyriaStandardTime = 1826684897U,
			TimeZoneMauritiusStandardTime = 3804745356U,
			TrackingMessageTypeNotSupported = 667589187U,
			TimeZoneCentralPacificStandardTime = 3096621845U,
			RoleTypeDescription_MailboxSearch = 1156915939U,
			StdUnknownTimeZone = 1027560048U,
			StatusUnsuccessFul = 3485911895U,
			RoleTypeDescription_MyLinkedInEnabled = 1187302658U,
			TestXHeader = 4008183169U,
			RoleTypeDescription_ReceiveConnectors = 1479842702U,
			TimeZoneRussiaTimeZone3 = 1338433320U,
			TimeZoneTransbaikalStandardTime = 2262407331U,
			RoleTypeDescription_Databases = 2629692075U,
			StatusTransferred = 14056478U,
			TimeZoneGeorgianStandardTime = 2237907931U,
			Decrypt = 3620221593U,
			RoleTypeDescription_MyReadWriteMailboxApps = 3879259618U,
			TimeZoneBougainvilleStandardTime = 3742415118U,
			RoleTypeDescription_EdgeSync = 99561377U,
			TimeZoneTurkeyStandardTime = 2333794793U,
			RoleTypeDescription_MyMailSubscriptions = 3305616866U,
			PartialMessages = 1393844023U,
			InboundIpMigrationCompleted = 3351363629U,
			RoleTypeDescription_MyMarketplaceApps = 626810642U,
			TimeZoneJordanStandardTime = 448910837U,
			EventPendingModerationHelpDesk = 1664785551U,
			DeliveryStatusDelivered = 1334000728U,
			TimeZoneEEuropeStandardTime = 135488574U,
			RoleTypeDescription_MyVoiceMail = 3847090244U,
			TimeZoneMyanmarStandardTime = 3097579504U,
			TrackingExplanationLogsDeleted = 1321416518U,
			ExchangeOrgAdminDescription = 2198659572U,
			TimeZoneNepalStandardTime = 2229119179U,
			TimeZoneCenAustraliaStandardTime = 203352201U,
			JobStatusFailed = 2350709050U,
			RoleTypeDescription_TransportQueues = 2909789462U,
			TimeZoneWestPacificStandardTime = 2239001839U,
			TrackingWarningNoResultsDueToTrackingTooEarly = 21646529U,
			RoleTypeDescription_ViewOnlyOrganizationManagement = 4161689178U,
			RoleTypeDescription_ViewOnlyRecipients = 2206122650U,
			Allow = 1776488541U,
			DomainScopeAlLDomains = 430852938U,
			NoValidDomainNameExistsInDomainSettings = 1082730632U,
			ExchangeRecordsManagementDescription = 377106190U,
			TimeZoneUTC08 = 2294597161U,
			RoleTypeDescription_NetworkingManagement = 1319136228U,
			EventTransferredToForeignOrgHelpDesk = 3173642551U,
			RoleTypeDescription_MyFacebookEnabled = 3245137676U,
			TimeZoneCentralEuropeStandardTime = 3487147524U,
			RoleTypeDescription_SecurityGroupCreationAndMembership = 254529528U,
			DeliveryStatusExpanded = 1316318251U,
			StatusPending = 1981651471U,
			TimeZoneMidAtlanticStandardTime = 3359056161U,
			RoleTypeDescription_ResetPassword = 970642999U,
			ExchangeUMManagementDescription = 1907760708U,
			TimeZoneUTC = 1392628209U,
			RoleTypeDescription_DataLossPrevention = 547528310U,
			EventPendingModerationIW = 429272165U,
			RoleTypeDescription_MyCustomApps = 1939880180U,
			RoleTypeDescription_DatabaseAvailabilityGroups = 2722115341U,
			ExchangeHelpDeskDescription = 2031056185U,
			MessageDirectionReceived = 2388819289U,
			SpamQuarantineMigrationSucceeded = 1562661838U,
			EventFailedModerationIW = 2708860089U,
			TimeZoneUTC02 = 2294597167U,
			AntimalwareScopingConstraint = 4202622721U,
			DeliveryStatusFailed = 2928288207U,
			QuarantineInbound = 3278150655U,
			RoleTypeDescription_Throttling = 872985302U,
			RoleTypeDescription_DataCenterDestructiveOperations = 2019318218U,
			RoleTypeDescription_AddressLists = 2945933912U,
			RoleTypeDescription_CentralAdminManagement = 1959014922U,
			TimeZoneAfghanistanStandardTime = 694118059U,
			EventTransferredToForeignOrgIW = 2433016809U,
			JobStatusCancelled = 1457011382U,
			TimeZoneAtlanticStandardTime = 2033924825U,
			TimeZoneArabStandardTime = 1901284917U,
			RoleTypeDescription_MailRecipients = 238886746U,
			RoleTypeDescription_WorkloadManagement = 2886777393U,
			TimeZoneAlaskanStandardTime = 1286295614U,
			MsoManagedTenantHelpdeskGroupDescription = 1235839409U,
			AntimalwareInboundRecipientNotifications = 1838296451U,
			TestXHeaderAndModifySubject = 270033310U,
			ContentIndexStatusFailedAndSuspended = 1923042104U,
			MsoManagedTenantAdminGroupDescription = 2104897796U,
			RoleTypeDescription_DataCenterOperations = 1558282382U,
			EventModerationExpired = 3700079135U,
			TraceLevelMedium = 29170872U,
			RoleTypeDescription_MoveMailboxes = 432229092U,
			RoleTypeDescription_MailRecipientCreation = 1038994972U,
			TimeZoneLineIslandsStandardTime = 3021954325U,
			MissingIdentityParameter = 2008936115U,
			ContentIndexStatusHealthyAndUpgrading = 3268869348U,
			TrafficScopeInbound = 1450163010U,
			RoleTypeDescription_UnScopedRoleManagement = 880515475U,
			AuditSeverityLevelDoNotAudit = 267867511U,
			ExchangeHygieneManagementDescription = 1778108211U,
			ClassIdExtensions = 1246550425U,
			TimeZoneNamibiaStandardTime = 4236830390U,
			RejectedExplanationContentFiltering = 2735513322U,
			QuarantineOutbound = 1179870130U,
			RunWeekly = 2454182516U,
			TimeZoneAltaiStandardTime = 1623584678U,
			EventSubmittedCrossSite = 889690570U,
			TimeZoneEasternStandardTime = 565087651U,
			TimeZoneAzerbaijanStandardTime = 1000900012U,
			TimeZonePakistanStandardTime = 2216352038U,
			RoleTypeDescription_OrgMarketplaceApps = 790166856U,
			TimeZonePacificSAStandardTime = 3066204968U,
			TimeZoneRussianStandardTime = 2981542010U,
			RoleTypeDescription_POP3AndIMAP4Protocols = 2623630612U,
			TimeZoneTaipeiStandardTime = 860548277U,
			RoleTypeDescription_HelpdeskRecipientManagement = 1145350245U,
			TimeZoneHawaiianStandardTime = 756612021U,
			TimeZoneMagallanesStandardTime = 2907857020U,
			EventFailedTransportRulesHelpDesk = 101906589U,
			TimeZoneTokyoStandardTime = 2104951827U,
			DeliveryStatusAll = 2425183541U,
			RoleTypeDescription_MyOptions = 4216139125U,
			RoleTypeDescription_DisasterRecovery = 996841945U,
			RoleTypeDescription_EmailAddressPolicies = 3202528089U,
			RoleTypeDescription_SendConnectors = 3655383969U,
			TimeZoneSudanStandardTime = 3185790196U,
			EventRead = 1544756182U,
			TimeZoneGreenlandStandardTime = 2467780809U,
			RoleTypeDescription_OutlookProvider = 2169942641U,
			EventTransferredIntermediate = 1460332657U,
			RoleTypeDescription_OrgCustomApps = 3444675422U,
			TimeZoneBangladeshStandardTime = 2174613220U,
			TimeZoneGMTStandardTime = 373243623U,
			TimeZoneGTBStandardTime = 1223375664U,
			JobStatusNotStarted = 4078458935U,
			TrackingWarningNoResultsDueToLogsNotFound = 1663352993U,
			TimeZoneSAWesternStandardTime = 77017683U,
			TimeZoneHaitiStandardTime = 3490088586U,
			TrackingErrorFailedToInitialize = 2700445791U,
			TimeZoneKamchatkaStandardTime = 728892142U,
			TrackingWarningResultsMissingTransient = 4111676399U,
			RulesMerged = 2247997721U,
			RoleTypeDescription_MyRetentionPolicies = 3804873683U,
			DomainScopedRulesMerged = 647678831U,
			EventResolvedHelpDesk = 1612370800U,
			RoleTypeDescription_LegalHoldApplication = 1531217347U,
			TimeZoneSingaporeStandardTime = 3498536455U,
			AntimalwareAdminAddressNull = 1949143649U,
			RoleTypeDescription_UserApplication = 2725696980U,
			RoleTypeDescription_MyContactInformation = 2096766553U,
			TimeZoneNorthAsiaEastStandardTime = 967467375U,
			ExchangeRecipientAdminDescription = 4108314201U,
			TimeZoneCapeVerdeStandardTime = 226934128U,
			TrackingSearchNotAuthorized = 3317872187U,
			ContentIndexStatusCrawling = 1575862374U,
			TimeZoneMagadanStandardTime = 2874655766U,
			TimeZoneAUSCentralStandardTime = 2055163269U,
			ExchangeDiscoveryManagementDescription = 2585558426U,
			EventMessageDefer = 3586764147U,
			TimeZoneVolgogradStandardTime = 2033601224U,
			TimeZonePacificStandardTimeMexico = 2373652411U,
			TimeZoneSamoaStandardTime = 2679378804U,
			RoleTypeDescription_PersonallyIdentifiableInformation = 2872669904U,
			TimeZoneUTC12 = 728513226U,
			RoleTypeDescription_MessageTracking = 3152944101U,
			TraceLevelLow = 1084831951U,
			RoleTypeDescription_MyTextMessaging = 661809288U,
			RoleTypeDescription_RecipientPolicies = 2577770798U,
			TimeZoneSAEasternStandardTime = 953426317U,
			RoleTypeDescription_TeamMailboxLifecycleApplication = 2376615838U,
			InvalidMessageTrackingReportId = 1205552534U,
			TimeZoneIndiaStandardTime = 430105556U,
			ContentIndexStatusFailed = 2562345274U,
			TimeZoneNewfoundlandStandardTime = 1494389266U,
			TimeZoneYakutskStandardTime = 2246440755U,
			RoleTypeDescription_OrganizationHelpSettings = 1274798456U,
			EventFailedModerationHelpDesk = 1835211555U,
			RoleTypeDescription_PublicFolders = 903792657U,
			TimeZonePacificStandardTime = 3005390568U,
			TimeZoneAleutianStandardTime = 61789322U,
			ExchangeManagementForestTier1SupportDescription = 3049152015U,
			TimeZoneNorthKoreaStandardTime = 2034512234U,
			DltUnknownTimeZone = 920357151U,
			RoleTypeDescription_ExchangeConnectors = 2373064468U,
			RoleTypeDescription_ViewOnlyCentralAdminSupport = 4036330655U,
			TimeZoneNorfolkStandardTime = 4097835338U,
			RoleTypeDescription_MyProfileInformation = 273095822U,
			TimeZoneSaoTomeStandardTime = 1814215051U,
			RoleTypeDescription_Journaling = 2648516368U,
			TimeZoneAzoresStandardTime = 1736152949U,
			TimeZoneAusCentralWStandardTime = 290876870U,
			TimeZoneUSMountainStandardTime = 4043635750U,
			RoleTypeDescription_ExchangeCrossServiceIntegration = 606097983U,
			PolicyMigrationSucceeded = 3335916139U,
			TimeZoneCentralBrazilianStandardTime = 140823076U,
			EventRulesCc = 2103082753U,
			ComplianceManagementGroupDescription = 2088363403U,
			TimeZoneWCentralAfricaStandardTime = 1556269475U,
			RoleTypeDescription_UMMailboxes = 3371502447U,
			ExchangeAllHostedOrgsDescription = 1332960560U,
			RoleTypeDescription_DistributionGroups = 1124818555U,
			ExchangeDelegatedSetupDescription = 92857077U,
			TimeZoneVladivostokStandardTime = 2984557217U,
			TimeZoneCentralStandardTime = 1755122990U,
			RoleTypeDescription_OrganizationClientAccess = 278680123U,
			ExchangeViewOnlyAdminDescription = 137603179U,
			TimeZoneEAustraliaStandardTime = 2189811946U,
			TrackingTransientError = 763976563U,
			TrafficScopeInboundAndOutbound = 3970531209U,
			RoleTypeDescription_CentralAdminCredentialManagement = 675468529U,
			RoleTypeDescription_UnScoped = 2365507750U,
			RoleTypeDescription_MyDistributionGroupMembership = 2259701124U,
			RoleTypeDescription_OfficeExtensionApplication = 3079546428U,
			RoleTypeDescription_DistributionGroupManagement = 3118752499U,
			AntimalwareMigrationSucceeded = 2462125228U,
			TimeZoneWEuropeStandardTime = 529456528U,
			RoleTypeDescription_AuditLogs = 172038955U,
			EventSubmitted = 3669915041U,
			TimeZoneEasternStandardTimeMexico = 2836851600U,
			TimeZoneRomanceStandardTime = 2471825646U,
			TimeZoneIranStandardTime = 2847973369U,
			RoleTypeDescription_AccessToCustomerDataDCOnly = 2304159373U,
			InvalidIdentityForAdmin = 102930739U,
			TimeZoneMarquesasStandardTime = 3118313859U,
			ContentIndexStatusHealthy = 4010596708U,
			RunMonthly = 2141699896U,
			RoleTypeDescription_OrganizationTransportSettings = 1224765338U,
			RoleTypeDescription_ViewOnlyConfiguration = 56162392U,
			TimeZoneMountainStandardTimeMexico = 3450693719U,
			RoleTypeDescription_PartnerDelegatedTenantManagement = 3338361539U,
			RoleTypeDescription_OrganizationConfiguration = 1621594316U,
			MsoMailTenantAdminGroupDescription = 3266046896U,
			TimeZoneSEAsiaStandardTime = 4268682097U,
			RoleTypeDescription_DatabaseCopies = 3867579719U,
			TimeZoneCentralStandardTimeMexico = 1556691491U,
			OutboundIpMigrationCompleted = 2695329098U,
			RecipientPathFilterNeeded = 2154772013U,
			TrackingWarningResultsMissingConnection = 3188452985U,
			TimeZoneNCentralAsiaStandardTime = 4058915032U,
			PolicyTipReject = 1753131922U,
			AuditSeverityLevelLow = 2037460782U,
			RoleTypeDescription_RecipientManagement = 2054972301U,
			EventForwarded = 435759238U,
			EventApprovedModerationHelpDesk = 1260062527U,
			Reject = 2235638931U,
			TimeZoneSaintPierreStandardTime = 3526220825U,
			TimeZoneMoroccoStandardTime = 1391837657U,
			TimeZoneFLEStandardTime = 3250495282U,
			RoleTypeDescription_MailTips = 566029912U,
			RoleTypeDescription_Monitoring = 3881958977U,
			AntispamEdgeBlockModeSettingNotMigrated = 2892044242U,
			TimeZoneCanadaCentralStandardTime = 779085836U,
			ExchangeManagementForestMonitoringDescription = 2024391679U,
			TimeZoneSakhalinStandardTime = 1004126924U,
			TimeZoneSriLankaStandardTime = 59677322U,
			RoleTypeDescription_AutoDiscover = 766524993U,
			TimeZoneRussiaTimeZone10 = 4291544598U,
			EventTransferredToSMSMessage = 3673094505U,
			TrackingWarningNoResultsDueToLogsExpired = 3152178141U,
			MessageDirectionSent = 3797334152U,
			RoleTypeDescription_RecordsManagement = 4116693438U,
			RoleTypeDescription_TransportRules = 825624151U,
			ForceTls = 1566193246U,
			EventFailedGeneral = 3297737305U,
			RoleTypeDescription_ExchangeServerCertificates = 2580146609U,
			EventPending = 1092647167U,
			TimeZoneChinaStandardTime = 845119240U,
			TimeZoneWestBankStandardTime = 4218393972U,
			RoleTypeDescription_MyBaseOptions = 1316750960U,
			TimeZoneGreenwichStandardTime = 3120202503U,
			RoleTypeDescription_UMPromptManagement = 3840952578U,
			AuditSeverityLevelMedium = 3678129241U,
			StatusFilterCannotBeSpecified = 4008278162U,
			ContentIndexStatusSuspended = 1056819816U,
			TimeZoneUTC13 = 728513227U,
			PolicyTipUrl = 1737084126U,
			JobStatusDone = 2937930519U,
			Quarantine = 2403632430U,
			TrackingWarningResultsMissingFatal = 3403825909U,
			TimeZoneNewZealandStandardTime = 1215124770U,
			ExecutableAttachments = 3584252448U,
			TimeZoneDatelineStandardTime = 2732724415U,
			EventQueueRetryIW = 1156214899U
		}

		private enum ParamIDs
		{
			InvalidIpRange,
			InvalidDomainNameInDomainSettings,
			FopePolicyRuleIsTooLargeToMigrate,
			OpportunisticTls,
			EventMovedToFolderByInboxRuleHelpDesk,
			FopePolicyRuleContainsInvalidPattern,
			EventSubmittedHelpDesk,
			FopePolicyRuleExpired,
			RecipientDomainConditionContainsInvalidDomainNames,
			DistributionGroupForVirtualDomainsCreated,
			EventSmtpReceiveHelpDesk,
			RecipientDomainNames,
			BodyCaseSensitive,
			TenantSkuNotSupportedByAntispam,
			DisabledInboundConnector,
			InvalidSmtpAddressInFopeRule,
			RecipientAddresses,
			SubjectExactMatchCondition,
			SubjectExactMatchCaseSensitive,
			AntimalwareInboundRecipientNotificationsScoped,
			Body,
			EventQueueRetryNoRetryTimeHelpDesk,
			SubjectCaseSensitiveCondition,
			MaximumMessageSize,
			EventQueueRetryNoErrorHelpDesk,
			InvalidUserName,
			AntimalwareAdminAddressValidations,
			SenderDomainNames,
			Subject,
			CharacterSets,
			AdminNotificationContainsMultipleAddresses,
			FopePolicyRuleDisabled,
			DistributionGroupForExcludedUsersCreated,
			BodyCaseSensitiveCondition,
			NoValidSmtpAddress,
			FopePolicyRuleContainsRecipientAddressAndRecipientDomainConditions,
			FopePolicyRuleHasMaxRecipientsCondition,
			ClassIdProperty,
			MigratedFooterSizeExceedsDisclaimerMaxSize,
			BodyExactMatchCaseSensitive,
			EventMovedToFolderByInboxRuleIW,
			DistributionListEmpty,
			NoValidDomainNameExistsInDomainScopedRule,
			InvalidRecipientForAdmin,
			NoValidIpRangesInFopeRule,
			CompressionError,
			AttachmentExtensionContainsInvalidCharacters,
			SubjectCaseSensitive,
			TrackingSearchException,
			DecompressionError,
			DomainLevelAdminNotSupportedByEOP,
			EventTransferredToLegacyExchangeServerHelpDesk,
			AntimalwareAdminAddressValidationsScoped,
			DisabledOutboundConnector,
			BodyCondition,
			FopePolicyRuleContainsIncompatibleConditions,
			PlusUnknownTimeZone,
			InvalidAttachmentExtensionCondition,
			FopePolicyRuleHasProhibitedRegularExpressions,
			HeaderValueMatch,
			FopePolicyRuleSummary,
			BCC,
			BodyExactMatch,
			TenantSkuNotSupported,
			SubjectExactMatch,
			FopePolicyRuleIsPartialMessage,
			AntimalwareVirtualDomainFailure,
			InboundConnectorWithoutSenderIPsAndCert,
			FilenameWordMatchCondition,
			AttachmentFilenames,
			EventSubmittedCrossSiteHelpDesk,
			InboundFopePolicyRuleWithDuplicateDomainName,
			InvalidSecondaryEmailAddresses,
			DistributionListExpanded,
			NoValidRecipientDomainNameExistsInRecipientDomainCondition,
			FopePolicyConsolidationList,
			AttachmentExtensions,
			MaximumRecipientNumber,
			EventDeliveredInboxRule,
			DomainScopedRuleContainsInvalidDomainNames,
			InvalidDomainNameInConnectorSetting,
			OutboundDomainScopedConnectorsMigrated,
			TenantSkuFilteringNotSupported,
			FopePolicyRuleHasWordsThatExceedMaximumLength,
			MinusUnknownTimeZone,
			AntimalwareTruncation,
			ExamineArchivesProperty,
			InvalidDomainNameInHipaaDomainSetting,
			UnableToAddUserToDistributionGroup,
			Redirect,
			HipaaPolicyMigrated,
			EventResolvedWithDetailsHelpDesk,
			InboundConnectorsWithSpamOrConnectionFilteringDisabledMigrated,
			SenderDomainConditionContainsInvalidDomainNames,
			FopePolicyRuleIsSkippableAntiSpamRule,
			NoValidSenderDomainNameExistsInSenderDomainCondition,
			EventExpanded,
			FopePolicyRuleHasUnrecognizedAction,
			EventSmtpSendHelpDesk,
			SenderIpAddresses,
			EventQueueRetryHelpDesk,
			HeaderNameMatch,
			EventDelayedAfterTransferToPartnerOrgHelpDesk,
			Description,
			InboundConnectorsWithPolicyFilteringDisabledMigrated,
			EventPendingAfterTransferToPartnerOrgHelpDesk,
			DomainNameTruncated,
			SenderAddress,
			BodyExactMatchCondition
		}
	}
}
