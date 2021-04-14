using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel.EnumTypes
{
	internal static class EnumStrings
	{
		static EnumStrings()
		{
			EnumStrings.stringIDs.Add(2831161206U, "CopyStatusDisconnected");
			EnumStrings.stringIDs.Add(2979702410U, "Inbox");
			EnumStrings.stringIDs.Add(3692015522U, "G711");
			EnumStrings.stringIDs.Add(3707194053U, "ServerRoleExtendedRole3");
			EnumStrings.stringIDs.Add(2125756541U, "ServerRoleMailbox");
			EnumStrings.stringIDs.Add(3194934827U, "ServerRoleUnifiedMessaging");
			EnumStrings.stringIDs.Add(1208301054U, "CopyStatusMounting");
			EnumStrings.stringIDs.Add(62599113U, "GroupNamingPolicyOffice");
			EnumStrings.stringIDs.Add(3725493575U, "WellKnownRecipientTypeMailGroups");
			EnumStrings.stringIDs.Add(4134731995U, "EnterpriseTrialEdition");
			EnumStrings.stringIDs.Add(1377545167U, "ADAttributeCustomAttribute1");
			EnumStrings.stringIDs.Add(756854696U, "ServerRoleEdge");
			EnumStrings.stringIDs.Add(2191591450U, "CoexistenceTrialEdition");
			EnumStrings.stringIDs.Add(172810921U, "ServerRoleHubTransport");
			EnumStrings.stringIDs.Add(1016721882U, "ADAttributeLastName");
			EnumStrings.stringIDs.Add(107906018U, "TeamMailboxRecipientTypeDetails");
			EnumStrings.stringIDs.Add(2385442291U, "ContentIndexStatusDisabled");
			EnumStrings.stringIDs.Add(2476473719U, "CcRecipientType");
			EnumStrings.stringIDs.Add(2328530086U, "FallbackIgnore");
			EnumStrings.stringIDs.Add(1970247521U, "MailEnabledUniversalSecurityGroupRecipientTypeDetails");
			EnumStrings.stringIDs.Add(623586765U, "CopyStatusMounted");
			EnumStrings.stringIDs.Add(3115737533U, "Gsm");
			EnumStrings.stringIDs.Add(1924734198U, "GroupNamingPolicyCustomAttribute6");
			EnumStrings.stringIDs.Add(2129610537U, "MessageTypeOof");
			EnumStrings.stringIDs.Add(862838650U, "GroupNamingPolicyCompany");
			EnumStrings.stringIDs.Add(2654331961U, "SpamFilteringOptionOn");
			EnumStrings.stringIDs.Add(1414246128U, "None");
			EnumStrings.stringIDs.Add(2630084427U, "ConversationHistory");
			EnumStrings.stringIDs.Add(1479682494U, "NoNewCalls");
			EnumStrings.stringIDs.Add(1716515484U, "Adfs");
			EnumStrings.stringIDs.Add(1277957481U, "Certificate");
			EnumStrings.stringIDs.Add(2003039265U, "ConnectorTypePartner");
			EnumStrings.stringIDs.Add(2468414724U, "ADAttributeInitials");
			EnumStrings.stringIDs.Add(135933047U, "AsyncOperationTypeUnknown");
			EnumStrings.stringIDs.Add(1059422816U, "TelExtn");
			EnumStrings.stringIDs.Add(2913015079U, "Authoritative");
			EnumStrings.stringIDs.Add(2227674028U, "MicrosoftExchangeRecipientTypeDetails");
			EnumStrings.stringIDs.Add(1091035505U, "LiveIdNegotiate");
			EnumStrings.stringIDs.Add(1677492010U, "SpecificUsers");
			EnumStrings.stringIDs.Add(1377545174U, "ADAttributeCustomAttribute8");
			EnumStrings.stringIDs.Add(1690995826U, "WSSecurity");
			EnumStrings.stringIDs.Add(2041595767U, "MessageTypeAutoForward");
			EnumStrings.stringIDs.Add(3211494971U, "Misconfigured");
			EnumStrings.stringIDs.Add(2773964607U, "DatabaseMasterTypeDag");
			EnumStrings.stringIDs.Add(680782013U, "MessageTypeReadReceipt");
			EnumStrings.stringIDs.Add(3341243277U, "MoveToDeletedItems");
			EnumStrings.stringIDs.Add(2611996929U, "RedirectRecipientType");
			EnumStrings.stringIDs.Add(4145265495U, "RemoteUserMailboxTypeDetails");
			EnumStrings.stringIDs.Add(393808047U, "ContentIndexStatusAutoSuspended");
			EnumStrings.stringIDs.Add(3799817423U, "ExternalManagedMailContactTypeDetails");
			EnumStrings.stringIDs.Add(635036566U, "Negotiate");
			EnumStrings.stringIDs.Add(3786203794U, "ServerRoleFrontendTransport");
			EnumStrings.stringIDs.Add(3655046083U, "CopyStatusDisconnectedAndResynchronizing");
			EnumStrings.stringIDs.Add(3811183882U, "Allowed");
			EnumStrings.stringIDs.Add(3197896354U, "GroupNamingPolicyExtensionCustomAttribute3");
			EnumStrings.stringIDs.Add(3050431750U, "ADAttributeName");
			EnumStrings.stringIDs.Add(1484405454U, "Disabled");
			EnumStrings.stringIDs.Add(4060482376U, "ConnectorTypeOnPremises");
			EnumStrings.stringIDs.Add(4072748617U, "MessageTypeApprovalRequest");
			EnumStrings.stringIDs.Add(3115100581U, "RejectUnlessExplicitOverrideActionType");
			EnumStrings.stringIDs.Add(2167560764U, "WellKnownRecipientTypeMailboxUsers");
			EnumStrings.stringIDs.Add(3144351139U, "FallbackReject");
			EnumStrings.stringIDs.Add(444885611U, "RemoteSharedMailboxTypeDetails");
			EnumStrings.stringIDs.Add(1241597555U, "Secured");
			EnumStrings.stringIDs.Add(1182470434U, "MoveToFolder");
			EnumStrings.stringIDs.Add(1377545163U, "ADAttributeCustomAttribute5");
			EnumStrings.stringIDs.Add(4199979286U, "ToRecipientType");
			EnumStrings.stringIDs.Add(3221974997U, "RoleGroupTypeDetails");
			EnumStrings.stringIDs.Add(2966158940U, "Tasks");
			EnumStrings.stringIDs.Add(2173147846U, "DatabaseMasterTypeServer");
			EnumStrings.stringIDs.Add(2338964630U, "UserRecipientTypeDetails");
			EnumStrings.stringIDs.Add(3990056197U, "CopyStatusNonExchangeReplication");
			EnumStrings.stringIDs.Add(1310067130U, "CopyStatusNotConfigured");
			EnumStrings.stringIDs.Add(1010456570U, "DeviceDiscovery");
			EnumStrings.stringIDs.Add(137387861U, "ContactRecipientTypeDetails");
			EnumStrings.stringIDs.Add(1361373610U, "ContentIndexStatusSeeding");
			EnumStrings.stringIDs.Add(2423361114U, "GroupNamingPolicyCustomAttribute11");
			EnumStrings.stringIDs.Add(3168546739U, "Ntlm");
			EnumStrings.stringIDs.Add(590977256U, "SentItems");
			EnumStrings.stringIDs.Add(1377545169U, "ADAttributeCustomAttribute3");
			EnumStrings.stringIDs.Add(3850073087U, "ADAttributePagerNumber");
			EnumStrings.stringIDs.Add(2002903510U, "ADAttributeStreet");
			EnumStrings.stringIDs.Add(2665399355U, "Wma");
			EnumStrings.stringIDs.Add(2703120928U, "GroupNamingPolicyCity");
			EnumStrings.stringIDs.Add(600983985U, "NonIpmRoot");
			EnumStrings.stringIDs.Add(1937417240U, "AsyncOperationTypeExportPST");
			EnumStrings.stringIDs.Add(2889762178U, "UnknownEdition");
			EnumStrings.stringIDs.Add(41715449U, "ModeEnforce");
			EnumStrings.stringIDs.Add(3918497079U, "EvaluationNotEqual");
			EnumStrings.stringIDs.Add(2099880135U, "WellKnownRecipientTypeAllRecipients");
			EnumStrings.stringIDs.Add(1048761747U, "ADAttributeCustomAttribute14");
			EnumStrings.stringIDs.Add(661425765U, "DatabaseMasterTypeUnknown");
			EnumStrings.stringIDs.Add(4137481806U, "ADAttributePhoneNumber");
			EnumStrings.stringIDs.Add(1924734196U, "GroupNamingPolicyCustomAttribute4");
			EnumStrings.stringIDs.Add(553174585U, "StandardTrialEdition");
			EnumStrings.stringIDs.Add(2283186478U, "PersonalFolder");
			EnumStrings.stringIDs.Add(754287197U, "LiveIdBasic");
			EnumStrings.stringIDs.Add(933193541U, "WellKnownRecipientTypeMailUsers");
			EnumStrings.stringIDs.Add(1818643265U, "SystemAttendantMailboxRecipientTypeDetails");
			EnumStrings.stringIDs.Add(1373187244U, "CopyStatusInitializing");
			EnumStrings.stringIDs.Add(1052758952U, "ServerRoleClientAccess");
			EnumStrings.stringIDs.Add(1903193717U, "MessageTypeCalendaring");
			EnumStrings.stringIDs.Add(3694564633U, "SyncIssues");
			EnumStrings.stringIDs.Add(798637440U, "AlwaysEnabled");
			EnumStrings.stringIDs.Add(1631091055U, "ContentIndexStatusUnknown");
			EnumStrings.stringIDs.Add(4263249978U, "SharedMailboxRecipientTypeDetails");
			EnumStrings.stringIDs.Add(3288506612U, "InternalRelay");
			EnumStrings.stringIDs.Add(1474747046U, "CoexistenceEdition");
			EnumStrings.stringIDs.Add(1924734197U, "GroupNamingPolicyCustomAttribute7");
			EnumStrings.stringIDs.Add(629464291U, "Outbox");
			EnumStrings.stringIDs.Add(2614845688U, "ADAttributeCustomAttribute15");
			EnumStrings.stringIDs.Add(1849540794U, "WellKnownRecipientTypeNone");
			EnumStrings.stringIDs.Add(25634710U, "ManagementRelationshipManager");
			EnumStrings.stringIDs.Add(986970413U, "ServerRoleCafeArray");
			EnumStrings.stringIDs.Add(1097129869U, "ExternalManagedGroupTypeDetails");
			EnumStrings.stringIDs.Add(3086386447U, "ArchiveStateNone");
			EnumStrings.stringIDs.Add(2509095413U, "EvaluatedUserSender");
			EnumStrings.stringIDs.Add(1389339898U, "IncidentReportIncludeOriginalMail");
			EnumStrings.stringIDs.Add(65728472U, "GroupNamingPolicyExtensionCustomAttribute1");
			EnumStrings.stringIDs.Add(4289093673U, "ADAttributeEmail");
			EnumStrings.stringIDs.Add(438888054U, "E164");
			EnumStrings.stringIDs.Add(4231482709U, "All");
			EnumStrings.stringIDs.Add(428619956U, "ManagementRelationshipDirectReport");
			EnumStrings.stringIDs.Add(637440764U, "InheritFromDialPlan");
			EnumStrings.stringIDs.Add(3459736224U, "EvaluationEqual");
			EnumStrings.stringIDs.Add(1094750789U, "MailboxPlanTypeDetails");
			EnumStrings.stringIDs.Add(3262572344U, "FallbackWrap");
			EnumStrings.stringIDs.Add(1377545162U, "ADAttributeCustomAttribute4");
			EnumStrings.stringIDs.Add(3367615085U, "ADAttributeDepartment");
			EnumStrings.stringIDs.Add(2944126402U, "SpamFilteringOptionTest");
			EnumStrings.stringIDs.Add(3026477473U, "Private");
			EnumStrings.stringIDs.Add(4226527350U, "ADAttributeCity");
			EnumStrings.stringIDs.Add(104454802U, "DiscoveryMailboxTypeDetails");
			EnumStrings.stringIDs.Add(4260106383U, "ADAttributePOBox");
			EnumStrings.stringIDs.Add(3707194057U, "ServerRoleExtendedRole7");
			EnumStrings.stringIDs.Add(3708929833U, "Everyone");
			EnumStrings.stringIDs.Add(221683052U, "LegacyMailboxRecipientTypeDetails");
			EnumStrings.stringIDs.Add(2182511137U, "ADAttributeFaxNumber");
			EnumStrings.stringIDs.Add(29398792U, "IncidentReportDoNotIncludeOriginalMail");
			EnumStrings.stringIDs.Add(3631693406U, "ExternalUser");
			EnumStrings.stringIDs.Add(1406382714U, "RemoteEquipmentMailboxTypeDetails");
			EnumStrings.stringIDs.Add(696030922U, "Tag");
			EnumStrings.stringIDs.Add(1494101274U, "GroupTypeFlagsBuiltinLocal");
			EnumStrings.stringIDs.Add(3802186670U, "ServerRoleManagementFrontEnd");
			EnumStrings.stringIDs.Add(4088287609U, "GroupNamingPolicyStateOrProvince");
			EnumStrings.stringIDs.Add(920444171U, "ArchiveStateHostedPending");
			EnumStrings.stringIDs.Add(322963092U, "RemoteTeamMailboxRecipientTypeDetails");
			EnumStrings.stringIDs.Add(381216251U, "ADAttributeZipCode");
			EnumStrings.stringIDs.Add(3675904764U, "PermanentlyDelete");
			EnumStrings.stringIDs.Add(2325276717U, "Location");
			EnumStrings.stringIDs.Add(3938481035U, "EquipmentMailboxRecipientTypeDetails");
			EnumStrings.stringIDs.Add(3673730471U, "CopyStatusDismounted");
			EnumStrings.stringIDs.Add(3423767853U, "SipName");
			EnumStrings.stringIDs.Add(3869829980U, "ModeAudit");
			EnumStrings.stringIDs.Add(3641768400U, "DumpsterFolder");
			EnumStrings.stringIDs.Add(1067650092U, "Organizational");
			EnumStrings.stringIDs.Add(2986926906U, "ADAttributeFirstName");
			EnumStrings.stringIDs.Add(407788899U, "ServerRoleSCOM");
			EnumStrings.stringIDs.Add(3613623199U, "DeletedItems");
			EnumStrings.stringIDs.Add(1924734194U, "GroupNamingPolicyCustomAttribute2");
			EnumStrings.stringIDs.Add(1377545168U, "ADAttributeCustomAttribute2");
			EnumStrings.stringIDs.Add(1638178773U, "GroupTypeFlagsDomainLocal");
			EnumStrings.stringIDs.Add(3980237751U, "ServerRoleCentralAdminFrontEnd");
			EnumStrings.stringIDs.Add(2795331228U, "InternalUser");
			EnumStrings.stringIDs.Add(1923042104U, "ContentIndexStatusFailedAndSuspended");
			EnumStrings.stringIDs.Add(3600528589U, "ADAttributeCountry");
			EnumStrings.stringIDs.Add(2030161115U, "SpamFilteringOptionOff");
			EnumStrings.stringIDs.Add(4137211921U, "GroupNamingPolicyTitle");
			EnumStrings.stringIDs.Add(1798370525U, "BccRecipientType");
			EnumStrings.stringIDs.Add(4181674605U, "AsyncOperationTypeImportPST");
			EnumStrings.stringIDs.Add(2736707353U, "RejectUnlessFalsePositiveOverrideActionType");
			EnumStrings.stringIDs.Add(3918345138U, "SpamFilteringActionDelete");
			EnumStrings.stringIDs.Add(1625030180U, "PublicFolderRecipientTypeDetails");
			EnumStrings.stringIDs.Add(685401583U, "SpamFilteringActionAddXHeader");
			EnumStrings.stringIDs.Add(2349327181U, "SpamFilteringActionModifySubject");
			EnumStrings.stringIDs.Add(3268869348U, "ContentIndexStatusHealthyAndUpgrading");
			EnumStrings.stringIDs.Add(4128944152U, "Basic");
			EnumStrings.stringIDs.Add(1855823700U, "Department");
			EnumStrings.stringIDs.Add(3606274629U, "MessageTypeSigned");
			EnumStrings.stringIDs.Add(2638599330U, "WellKnownRecipientTypeMailContacts");
			EnumStrings.stringIDs.Add(2411750738U, "ADAttributeMobileNumber");
			EnumStrings.stringIDs.Add(117825870U, "MessageTypeVoicemail");
			EnumStrings.stringIDs.Add(3689869554U, "MailEnabledUserRecipientTypeDetails");
			EnumStrings.stringIDs.Add(1549653732U, "Mp3");
			EnumStrings.stringIDs.Add(2447598924U, "RejectUnlessSilentOverrideActionType");
			EnumStrings.stringIDs.Add(3674978674U, "GroupNamingPolicyCountryOrRegion");
			EnumStrings.stringIDs.Add(2698858797U, "ServerRoleLanguagePacks");
			EnumStrings.stringIDs.Add(1359519288U, "CopyStatusSinglePageRestore");
			EnumStrings.stringIDs.Add(3376217818U, "MailEnabledNonUniversalGroupRecipientTypeDetails");
			EnumStrings.stringIDs.Add(1123996746U, "SpamFilteringActionJmf");
			EnumStrings.stringIDs.Add(115734878U, "Drafts");
			EnumStrings.stringIDs.Add(3374360575U, "ADAttributeCustomAttribute10");
			EnumStrings.stringIDs.Add(110833865U, "ArchiveStateOnPremise");
			EnumStrings.stringIDs.Add(1966081841U, "UniversalDistributionGroupRecipientTypeDetails");
			EnumStrings.stringIDs.Add(2852597951U, "SpamFilteringActionQuarantine");
			EnumStrings.stringIDs.Add(1377545164U, "ADAttributeCustomAttribute6");
			EnumStrings.stringIDs.Add(252422050U, "GroupTypeFlagsNone");
			EnumStrings.stringIDs.Add(2562345274U, "ContentIndexStatusFailed");
			EnumStrings.stringIDs.Add(2775202161U, "ServerRoleOSP");
			EnumStrings.stringIDs.Add(3689464497U, "ADAttributeOtherFaxNumber");
			EnumStrings.stringIDs.Add(97762286U, "GroupNamingPolicyCustomAttribute15");
			EnumStrings.stringIDs.Add(634395589U, "Enabled");
			EnumStrings.stringIDs.Add(1377545165U, "ADAttributeCustomAttribute7");
			EnumStrings.stringIDs.Add(1924734191U, "GroupNamingPolicyCustomAttribute1");
			EnumStrings.stringIDs.Add(1191186633U, "GroupTypeFlagsUniversal");
			EnumStrings.stringIDs.Add(645477220U, "ADAttributeCustomAttribute11");
			EnumStrings.stringIDs.Add(2391327300U, "GroupNamingPolicyExtensionCustomAttribute5");
			EnumStrings.stringIDs.Add(3707194059U, "ServerRoleExtendedRole5");
			EnumStrings.stringIDs.Add(3309342631U, "OAuth");
			EnumStrings.stringIDs.Add(856583401U, "ADAttributeOtherHomePhoneNumber");
			EnumStrings.stringIDs.Add(665936024U, "ArchiveStateLocal");
			EnumStrings.stringIDs.Add(3586160528U, "GroupNamingPolicyCustomAttribute13");
			EnumStrings.stringIDs.Add(3489169852U, "ComputerRecipientTypeDetails");
			EnumStrings.stringIDs.Add(1582423804U, "LiveIdFba");
			EnumStrings.stringIDs.Add(494686544U, "ADAttributeManager");
			EnumStrings.stringIDs.Add(3162495226U, "ADAttributeOtherPhoneNumber");
			EnumStrings.stringIDs.Add(3464146580U, "ServerRoleFfoWebServices");
			EnumStrings.stringIDs.Add(1575862374U, "ContentIndexStatusCrawling");
			EnumStrings.stringIDs.Add(2835967712U, "MoveToArchive");
			EnumStrings.stringIDs.Add(729925097U, "MonitoringMailboxRecipientTypeDetails");
			EnumStrings.stringIDs.Add(570563164U, "ServerRoleAll");
			EnumStrings.stringIDs.Add(825243359U, "GroupNamingPolicyExtensionCustomAttribute4");
			EnumStrings.stringIDs.Add(3773054995U, "WellKnownRecipientTypeResources");
			EnumStrings.stringIDs.Add(1924734195U, "GroupNamingPolicyCustomAttribute5");
			EnumStrings.stringIDs.Add(872998734U, "WindowsIntegrated");
			EnumStrings.stringIDs.Add(980672066U, "SMTPAddress");
			EnumStrings.stringIDs.Add(1452889642U, "ADAttributeUserLogonName");
			EnumStrings.stringIDs.Add(863112602U, "ADAttributeNotes");
			EnumStrings.stringIDs.Add(1738880682U, "LinkedUserTypeDetails");
			EnumStrings.stringIDs.Add(2303788021U, "PromptForAlias");
			EnumStrings.stringIDs.Add(2227190334U, "NonUniversalGroupRecipientTypeDetails");
			EnumStrings.stringIDs.Add(2634964433U, "ADAttributeTitle");
			EnumStrings.stringIDs.Add(2422734853U, "SIPSecured");
			EnumStrings.stringIDs.Add(4189810048U, "CopyStatusDismounting");
			EnumStrings.stringIDs.Add(729299916U, "CopyStatusServiceDown");
			EnumStrings.stringIDs.Add(1487832074U, "PublicFolderMailboxRecipientTypeDetails");
			EnumStrings.stringIDs.Add(996355914U, "Quarantined");
			EnumStrings.stringIDs.Add(3200416695U, "GroupTypeFlagsSecurityEnabled");
			EnumStrings.stringIDs.Add(2094315795U, "ServerRoleNone");
			EnumStrings.stringIDs.Add(26915469U, "EnterpriseEdition");
			EnumStrings.stringIDs.Add(2045069482U, "AsyncOperationTypeCertExpiry");
			EnumStrings.stringIDs.Add(715964235U, "ExternalPartner");
			EnumStrings.stringIDs.Add(1924734193U, "GroupNamingPolicyCustomAttribute3");
			EnumStrings.stringIDs.Add(1765158362U, "CopyStatusFailedAndSuspended");
			EnumStrings.stringIDs.Add(3949283739U, "AllUsers");
			EnumStrings.stringIDs.Add(2605454650U, "CopyStatusSuspended");
			EnumStrings.stringIDs.Add(4137480277U, "Journal");
			EnumStrings.stringIDs.Add(2321790947U, "StandardEdition");
			EnumStrings.stringIDs.Add(3453679227U, "UndefinedRecipientTypeDetails");
			EnumStrings.stringIDs.Add(2160282563U, "CopyStatusSeedingSource");
			EnumStrings.stringIDs.Add(230388220U, "ModeAuditAndNotify");
			EnumStrings.stringIDs.Add(2891753468U, "ADAttributeCompany");
			EnumStrings.stringIDs.Add(2030715989U, "EvaluatedUserRecipient");
			EnumStrings.stringIDs.Add(4019774802U, "Blocked");
			EnumStrings.stringIDs.Add(2155604814U, "ExternalNonPartner");
			EnumStrings.stringIDs.Add(3815678973U, "MailEnabledContactRecipientTypeDetails");
			EnumStrings.stringIDs.Add(1573777228U, "Unsecured");
			EnumStrings.stringIDs.Add(2472951404U, "ArchiveStateHostedProvisioned");
			EnumStrings.stringIDs.Add(1924734199U, "GroupNamingPolicyCustomAttribute9");
			EnumStrings.stringIDs.Add(1292798904U, "Calendar");
			EnumStrings.stringIDs.Add(3647297993U, "ArbitrationMailboxTypeDetails");
			EnumStrings.stringIDs.Add(3569405894U, "DisabledUserRecipientTypeDetails");
			EnumStrings.stringIDs.Add(1960737953U, "CopyStatusUnknown");
			EnumStrings.stringIDs.Add(142823596U, "LastFirst");
			EnumStrings.stringIDs.Add(2872629304U, "MessageTypePermissionControlled");
			EnumStrings.stringIDs.Add(3598244064U, "RssSubscriptions");
			EnumStrings.stringIDs.Add(4010596708U, "ContentIndexStatusHealthy");
			EnumStrings.stringIDs.Add(1808276634U, "ADAttributeCustomAttribute13");
			EnumStrings.stringIDs.Add(645017541U, "Kerberos");
			EnumStrings.stringIDs.Add(1484668346U, "CopyStatusHealthy");
			EnumStrings.stringIDs.Add(1391517930U, "RoomListGroupTypeDetails");
			EnumStrings.stringIDs.Add(1377545175U, "ADAttributeCustomAttribute9");
			EnumStrings.stringIDs.Add(1536572748U, "ServerRoleCafe");
			EnumStrings.stringIDs.Add(1924734200U, "GroupNamingPolicyCustomAttribute8");
			EnumStrings.stringIDs.Add(3133553171U, "SoftDelete");
			EnumStrings.stringIDs.Add(2171581398U, "ExternalRelay");
			EnumStrings.stringIDs.Add(2300412432U, "FirstLast");
			EnumStrings.stringIDs.Add(1663846227U, "GroupNamingPolicyCustomAttribute14");
			EnumStrings.stringIDs.Add(4189167987U, "GroupTypeFlagsGlobal");
			EnumStrings.stringIDs.Add(1919306754U, "ConferenceRoomMailboxRecipientTypeDetails");
			EnumStrings.stringIDs.Add(1588035907U, "AsyncOperationTypeMailboxRestore");
			EnumStrings.stringIDs.Add(2846264340U, "Unknown");
			EnumStrings.stringIDs.Add(3882899654U, "ADAttributeState");
			EnumStrings.stringIDs.Add(2223810040U, "GroupMailboxRecipientTypeDetails");
			EnumStrings.stringIDs.Add(1927573801U, "ADAttributeOffice");
			EnumStrings.stringIDs.Add(1545501201U, "CopyStatusResynchronizing");
			EnumStrings.stringIDs.Add(1432667858U, "LinkedMailboxRecipientTypeDetails");
			EnumStrings.stringIDs.Add(968858937U, "UniversalSecurityGroupRecipientTypeDetails");
			EnumStrings.stringIDs.Add(242192693U, "ADAttributeCustomAttribute12");
			EnumStrings.stringIDs.Add(1631812413U, "GroupNamingPolicyExtensionCustomAttribute2");
			EnumStrings.stringIDs.Add(1594549261U, "RemoteRoomMailboxTypeDetails");
			EnumStrings.stringIDs.Add(2435266816U, "Title");
			EnumStrings.stringIDs.Add(1605633982U, "MailboxUserRecipientTypeDetails");
			EnumStrings.stringIDs.Add(3707194060U, "ServerRoleExtendedRole4");
			EnumStrings.stringIDs.Add(1922689150U, "ServerRoleProvisionedServer");
			EnumStrings.stringIDs.Add(4255105347U, "SpamFilteringActionRedirect");
			EnumStrings.stringIDs.Add(857277173U, "GroupNamingPolicyCustomAttribute12");
			EnumStrings.stringIDs.Add(562488721U, "MailEnabledUniversalDistributionGroupRecipientTypeDetails");
			EnumStrings.stringIDs.Add(604363629U, "NotifyOnlyActionType");
			EnumStrings.stringIDs.Add(498572210U, "RejectMessageActionType");
			EnumStrings.stringIDs.Add(1587179080U, "CopyStatusMisconfigured");
			EnumStrings.stringIDs.Add(1457839961U, "ADAttributeHomePhoneNumber");
			EnumStrings.stringIDs.Add(1716044995U, "Contacts");
			EnumStrings.stringIDs.Add(3635271833U, "LegacyArchiveJournals");
			EnumStrings.stringIDs.Add(154085973U, "GroupNamingPolicyDepartment");
			EnumStrings.stringIDs.Add(696678862U, "ServerRoleManagementBackEnd");
			EnumStrings.stringIDs.Add(815864422U, "Digest");
			EnumStrings.stringIDs.Add(1702371863U, "AsyncOperationTypeMigration");
			EnumStrings.stringIDs.Add(4015121608U, "CopyStatusFailed");
			EnumStrings.stringIDs.Add(1601836855U, "Notes");
			EnumStrings.stringIDs.Add(3707194054U, "ServerRoleExtendedRole2");
			EnumStrings.stringIDs.Add(273163868U, "NegoEx");
			EnumStrings.stringIDs.Add(3544120613U, "MessageTypeEncrypted");
			EnumStrings.stringIDs.Add(3989445055U, "GroupNamingPolicyCustomAttribute10");
			EnumStrings.stringIDs.Add(2999125469U, "MailEnabledDynamicDistributionGroupRecipientTypeDetails");
			EnumStrings.stringIDs.Add(1056819816U, "ContentIndexStatusSuspended");
			EnumStrings.stringIDs.Add(1099314853U, "Fba");
			EnumStrings.stringIDs.Add(3985647980U, "CopyStatusDisconnectedAndHealthy");
			EnumStrings.stringIDs.Add(3586618070U, "MailEnabledForestContactRecipientTypeDetails");
			EnumStrings.stringIDs.Add(2241039844U, "JunkEmail");
			EnumStrings.stringIDs.Add(1024471425U, "ServerRoleMonitoring");
			EnumStrings.stringIDs.Add(1850977098U, "SystemMailboxRecipientTypeDetails");
			EnumStrings.stringIDs.Add(4022404286U, "GroupNamingPolicyCountryCode");
			EnumStrings.stringIDs.Add(448862132U, "CopyStatusSeeding");
		}

		public static LocalizedString CopyStatusDisconnected
		{
			get
			{
				return new LocalizedString("CopyStatusDisconnected", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Inbox
		{
			get
			{
				return new LocalizedString("Inbox", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString G711
		{
			get
			{
				return new LocalizedString("G711", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleExtendedRole3
		{
			get
			{
				return new LocalizedString("ServerRoleExtendedRole3", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleMailbox
		{
			get
			{
				return new LocalizedString("ServerRoleMailbox", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleUnifiedMessaging
		{
			get
			{
				return new LocalizedString("ServerRoleUnifiedMessaging", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CopyStatusMounting
		{
			get
			{
				return new LocalizedString("CopyStatusMounting", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyOffice
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyOffice", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WellKnownRecipientTypeMailGroups
		{
			get
			{
				return new LocalizedString("WellKnownRecipientTypeMailGroups", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EnterpriseTrialEdition
		{
			get
			{
				return new LocalizedString("EnterpriseTrialEdition", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCustomAttribute1
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute1", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleEdge
		{
			get
			{
				return new LocalizedString("ServerRoleEdge", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CoexistenceTrialEdition
		{
			get
			{
				return new LocalizedString("CoexistenceTrialEdition", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleHubTransport
		{
			get
			{
				return new LocalizedString("ServerRoleHubTransport", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeLastName
		{
			get
			{
				return new LocalizedString("ADAttributeLastName", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("TeamMailboxRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContentIndexStatusDisabled
		{
			get
			{
				return new LocalizedString("ContentIndexStatusDisabled", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CcRecipientType
		{
			get
			{
				return new LocalizedString("CcRecipientType", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FallbackIgnore
		{
			get
			{
				return new LocalizedString("FallbackIgnore", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailEnabledUniversalSecurityGroupRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("MailEnabledUniversalSecurityGroupRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CopyStatusMounted
		{
			get
			{
				return new LocalizedString("CopyStatusMounted", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Gsm
		{
			get
			{
				return new LocalizedString("Gsm", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute6
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute6", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTypeOof
		{
			get
			{
				return new LocalizedString("MessageTypeOof", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCompany
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCompany", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpamFilteringOptionOn
		{
			get
			{
				return new LocalizedString("SpamFilteringOptionOn", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString None
		{
			get
			{
				return new LocalizedString("None", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversationHistory
		{
			get
			{
				return new LocalizedString("ConversationHistory", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoNewCalls
		{
			get
			{
				return new LocalizedString("NoNewCalls", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Adfs
		{
			get
			{
				return new LocalizedString("Adfs", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Certificate
		{
			get
			{
				return new LocalizedString("Certificate", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectorTypePartner
		{
			get
			{
				return new LocalizedString("ConnectorTypePartner", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeInitials
		{
			get
			{
				return new LocalizedString("ADAttributeInitials", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AsyncOperationTypeUnknown
		{
			get
			{
				return new LocalizedString("AsyncOperationTypeUnknown", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TelExtn
		{
			get
			{
				return new LocalizedString("TelExtn", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Authoritative
		{
			get
			{
				return new LocalizedString("Authoritative", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MicrosoftExchangeRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("MicrosoftExchangeRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LiveIdNegotiate
		{
			get
			{
				return new LocalizedString("LiveIdNegotiate", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpecificUsers
		{
			get
			{
				return new LocalizedString("SpecificUsers", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCustomAttribute8
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute8", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WSSecurity
		{
			get
			{
				return new LocalizedString("WSSecurity", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTypeAutoForward
		{
			get
			{
				return new LocalizedString("MessageTypeAutoForward", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Misconfigured
		{
			get
			{
				return new LocalizedString("Misconfigured", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseMasterTypeDag
		{
			get
			{
				return new LocalizedString("DatabaseMasterTypeDag", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTypeReadReceipt
		{
			get
			{
				return new LocalizedString("MessageTypeReadReceipt", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MoveToDeletedItems
		{
			get
			{
				return new LocalizedString("MoveToDeletedItems", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RedirectRecipientType
		{
			get
			{
				return new LocalizedString("RedirectRecipientType", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteUserMailboxTypeDetails
		{
			get
			{
				return new LocalizedString("RemoteUserMailboxTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContentIndexStatusAutoSuspended
		{
			get
			{
				return new LocalizedString("ContentIndexStatusAutoSuspended", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalManagedMailContactTypeDetails
		{
			get
			{
				return new LocalizedString("ExternalManagedMailContactTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Negotiate
		{
			get
			{
				return new LocalizedString("Negotiate", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleFrontendTransport
		{
			get
			{
				return new LocalizedString("ServerRoleFrontendTransport", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CopyStatusDisconnectedAndResynchronizing
		{
			get
			{
				return new LocalizedString("CopyStatusDisconnectedAndResynchronizing", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Allowed
		{
			get
			{
				return new LocalizedString("Allowed", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyExtensionCustomAttribute3
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyExtensionCustomAttribute3", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeName
		{
			get
			{
				return new LocalizedString("ADAttributeName", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Disabled
		{
			get
			{
				return new LocalizedString("Disabled", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectorTypeOnPremises
		{
			get
			{
				return new LocalizedString("ConnectorTypeOnPremises", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTypeApprovalRequest
		{
			get
			{
				return new LocalizedString("MessageTypeApprovalRequest", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RejectUnlessExplicitOverrideActionType
		{
			get
			{
				return new LocalizedString("RejectUnlessExplicitOverrideActionType", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WellKnownRecipientTypeMailboxUsers
		{
			get
			{
				return new LocalizedString("WellKnownRecipientTypeMailboxUsers", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FallbackReject
		{
			get
			{
				return new LocalizedString("FallbackReject", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteSharedMailboxTypeDetails
		{
			get
			{
				return new LocalizedString("RemoteSharedMailboxTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Secured
		{
			get
			{
				return new LocalizedString("Secured", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MoveToFolder
		{
			get
			{
				return new LocalizedString("MoveToFolder", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCustomAttribute5
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute5", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ToRecipientType
		{
			get
			{
				return new LocalizedString("ToRecipientType", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleGroupTypeDetails
		{
			get
			{
				return new LocalizedString("RoleGroupTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Tasks
		{
			get
			{
				return new LocalizedString("Tasks", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseMasterTypeServer
		{
			get
			{
				return new LocalizedString("DatabaseMasterTypeServer", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("UserRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CopyStatusNonExchangeReplication
		{
			get
			{
				return new LocalizedString("CopyStatusNonExchangeReplication", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CopyStatusNotConfigured
		{
			get
			{
				return new LocalizedString("CopyStatusNotConfigured", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceDiscovery
		{
			get
			{
				return new LocalizedString("DeviceDiscovery", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContactRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("ContactRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContentIndexStatusSeeding
		{
			get
			{
				return new LocalizedString("ContentIndexStatusSeeding", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute11
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute11", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Ntlm
		{
			get
			{
				return new LocalizedString("Ntlm", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SentItems
		{
			get
			{
				return new LocalizedString("SentItems", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCustomAttribute3
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute3", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributePagerNumber
		{
			get
			{
				return new LocalizedString("ADAttributePagerNumber", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeStreet
		{
			get
			{
				return new LocalizedString("ADAttributeStreet", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Wma
		{
			get
			{
				return new LocalizedString("Wma", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCity
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCity", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NonIpmRoot
		{
			get
			{
				return new LocalizedString("NonIpmRoot", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AsyncOperationTypeExportPST
		{
			get
			{
				return new LocalizedString("AsyncOperationTypeExportPST", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnknownEdition
		{
			get
			{
				return new LocalizedString("UnknownEdition", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ModeEnforce
		{
			get
			{
				return new LocalizedString("ModeEnforce", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EvaluationNotEqual
		{
			get
			{
				return new LocalizedString("EvaluationNotEqual", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WellKnownRecipientTypeAllRecipients
		{
			get
			{
				return new LocalizedString("WellKnownRecipientTypeAllRecipients", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCustomAttribute14
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute14", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseMasterTypeUnknown
		{
			get
			{
				return new LocalizedString("DatabaseMasterTypeUnknown", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributePhoneNumber
		{
			get
			{
				return new LocalizedString("ADAttributePhoneNumber", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute4
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute4", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StandardTrialEdition
		{
			get
			{
				return new LocalizedString("StandardTrialEdition", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PersonalFolder
		{
			get
			{
				return new LocalizedString("PersonalFolder", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LiveIdBasic
		{
			get
			{
				return new LocalizedString("LiveIdBasic", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WellKnownRecipientTypeMailUsers
		{
			get
			{
				return new LocalizedString("WellKnownRecipientTypeMailUsers", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SystemAttendantMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("SystemAttendantMailboxRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CopyStatusInitializing
		{
			get
			{
				return new LocalizedString("CopyStatusInitializing", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleClientAccess
		{
			get
			{
				return new LocalizedString("ServerRoleClientAccess", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTypeCalendaring
		{
			get
			{
				return new LocalizedString("MessageTypeCalendaring", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SyncIssues
		{
			get
			{
				return new LocalizedString("SyncIssues", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlwaysEnabled
		{
			get
			{
				return new LocalizedString("AlwaysEnabled", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContentIndexStatusUnknown
		{
			get
			{
				return new LocalizedString("ContentIndexStatusUnknown", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SharedMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("SharedMailboxRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InternalRelay
		{
			get
			{
				return new LocalizedString("InternalRelay", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CoexistenceEdition
		{
			get
			{
				return new LocalizedString("CoexistenceEdition", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute7
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute7", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Outbox
		{
			get
			{
				return new LocalizedString("Outbox", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCustomAttribute15
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute15", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WellKnownRecipientTypeNone
		{
			get
			{
				return new LocalizedString("WellKnownRecipientTypeNone", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ManagementRelationshipManager
		{
			get
			{
				return new LocalizedString("ManagementRelationshipManager", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleCafeArray
		{
			get
			{
				return new LocalizedString("ServerRoleCafeArray", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalManagedGroupTypeDetails
		{
			get
			{
				return new LocalizedString("ExternalManagedGroupTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ArchiveStateNone
		{
			get
			{
				return new LocalizedString("ArchiveStateNone", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EvaluatedUserSender
		{
			get
			{
				return new LocalizedString("EvaluatedUserSender", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportIncludeOriginalMail
		{
			get
			{
				return new LocalizedString("IncidentReportIncludeOriginalMail", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyExtensionCustomAttribute1
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyExtensionCustomAttribute1", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeEmail
		{
			get
			{
				return new LocalizedString("ADAttributeEmail", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString E164
		{
			get
			{
				return new LocalizedString("E164", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString All
		{
			get
			{
				return new LocalizedString("All", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ManagementRelationshipDirectReport
		{
			get
			{
				return new LocalizedString("ManagementRelationshipDirectReport", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InheritFromDialPlan
		{
			get
			{
				return new LocalizedString("InheritFromDialPlan", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EvaluationEqual
		{
			get
			{
				return new LocalizedString("EvaluationEqual", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxPlanTypeDetails
		{
			get
			{
				return new LocalizedString("MailboxPlanTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FallbackWrap
		{
			get
			{
				return new LocalizedString("FallbackWrap", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCustomAttribute4
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute4", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeDepartment
		{
			get
			{
				return new LocalizedString("ADAttributeDepartment", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpamFilteringOptionTest
		{
			get
			{
				return new LocalizedString("SpamFilteringOptionTest", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Private
		{
			get
			{
				return new LocalizedString("Private", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCity
		{
			get
			{
				return new LocalizedString("ADAttributeCity", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DiscoveryMailboxTypeDetails
		{
			get
			{
				return new LocalizedString("DiscoveryMailboxTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributePOBox
		{
			get
			{
				return new LocalizedString("ADAttributePOBox", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleExtendedRole7
		{
			get
			{
				return new LocalizedString("ServerRoleExtendedRole7", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Everyone
		{
			get
			{
				return new LocalizedString("Everyone", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LegacyMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("LegacyMailboxRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeFaxNumber
		{
			get
			{
				return new LocalizedString("ADAttributeFaxNumber", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportDoNotIncludeOriginalMail
		{
			get
			{
				return new LocalizedString("IncidentReportDoNotIncludeOriginalMail", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalUser
		{
			get
			{
				return new LocalizedString("ExternalUser", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteEquipmentMailboxTypeDetails
		{
			get
			{
				return new LocalizedString("RemoteEquipmentMailboxTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Tag
		{
			get
			{
				return new LocalizedString("Tag", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupTypeFlagsBuiltinLocal
		{
			get
			{
				return new LocalizedString("GroupTypeFlagsBuiltinLocal", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleManagementFrontEnd
		{
			get
			{
				return new LocalizedString("ServerRoleManagementFrontEnd", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyStateOrProvince
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyStateOrProvince", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ArchiveStateHostedPending
		{
			get
			{
				return new LocalizedString("ArchiveStateHostedPending", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteTeamMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("RemoteTeamMailboxRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeZipCode
		{
			get
			{
				return new LocalizedString("ADAttributeZipCode", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PermanentlyDelete
		{
			get
			{
				return new LocalizedString("PermanentlyDelete", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Location
		{
			get
			{
				return new LocalizedString("Location", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EquipmentMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("EquipmentMailboxRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CopyStatusDismounted
		{
			get
			{
				return new LocalizedString("CopyStatusDismounted", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SipName
		{
			get
			{
				return new LocalizedString("SipName", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ModeAudit
		{
			get
			{
				return new LocalizedString("ModeAudit", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DumpsterFolder
		{
			get
			{
				return new LocalizedString("DumpsterFolder", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Organizational
		{
			get
			{
				return new LocalizedString("Organizational", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeFirstName
		{
			get
			{
				return new LocalizedString("ADAttributeFirstName", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleSCOM
		{
			get
			{
				return new LocalizedString("ServerRoleSCOM", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeletedItems
		{
			get
			{
				return new LocalizedString("DeletedItems", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute2
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute2", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCustomAttribute2
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute2", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupTypeFlagsDomainLocal
		{
			get
			{
				return new LocalizedString("GroupTypeFlagsDomainLocal", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleCentralAdminFrontEnd
		{
			get
			{
				return new LocalizedString("ServerRoleCentralAdminFrontEnd", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InternalUser
		{
			get
			{
				return new LocalizedString("InternalUser", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContentIndexStatusFailedAndSuspended
		{
			get
			{
				return new LocalizedString("ContentIndexStatusFailedAndSuspended", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCountry
		{
			get
			{
				return new LocalizedString("ADAttributeCountry", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpamFilteringOptionOff
		{
			get
			{
				return new LocalizedString("SpamFilteringOptionOff", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyTitle
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyTitle", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BccRecipientType
		{
			get
			{
				return new LocalizedString("BccRecipientType", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AsyncOperationTypeImportPST
		{
			get
			{
				return new LocalizedString("AsyncOperationTypeImportPST", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RejectUnlessFalsePositiveOverrideActionType
		{
			get
			{
				return new LocalizedString("RejectUnlessFalsePositiveOverrideActionType", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpamFilteringActionDelete
		{
			get
			{
				return new LocalizedString("SpamFilteringActionDelete", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFolderRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("PublicFolderRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpamFilteringActionAddXHeader
		{
			get
			{
				return new LocalizedString("SpamFilteringActionAddXHeader", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpamFilteringActionModifySubject
		{
			get
			{
				return new LocalizedString("SpamFilteringActionModifySubject", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContentIndexStatusHealthyAndUpgrading
		{
			get
			{
				return new LocalizedString("ContentIndexStatusHealthyAndUpgrading", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Basic
		{
			get
			{
				return new LocalizedString("Basic", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Department
		{
			get
			{
				return new LocalizedString("Department", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTypeSigned
		{
			get
			{
				return new LocalizedString("MessageTypeSigned", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WellKnownRecipientTypeMailContacts
		{
			get
			{
				return new LocalizedString("WellKnownRecipientTypeMailContacts", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeMobileNumber
		{
			get
			{
				return new LocalizedString("ADAttributeMobileNumber", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTypeVoicemail
		{
			get
			{
				return new LocalizedString("MessageTypeVoicemail", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailEnabledUserRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("MailEnabledUserRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Mp3
		{
			get
			{
				return new LocalizedString("Mp3", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RejectUnlessSilentOverrideActionType
		{
			get
			{
				return new LocalizedString("RejectUnlessSilentOverrideActionType", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCountryOrRegion
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCountryOrRegion", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleLanguagePacks
		{
			get
			{
				return new LocalizedString("ServerRoleLanguagePacks", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CopyStatusSinglePageRestore
		{
			get
			{
				return new LocalizedString("CopyStatusSinglePageRestore", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailEnabledNonUniversalGroupRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("MailEnabledNonUniversalGroupRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpamFilteringActionJmf
		{
			get
			{
				return new LocalizedString("SpamFilteringActionJmf", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Drafts
		{
			get
			{
				return new LocalizedString("Drafts", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCustomAttribute10
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute10", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ArchiveStateOnPremise
		{
			get
			{
				return new LocalizedString("ArchiveStateOnPremise", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnsupportServerEdition(string edition)
		{
			return new LocalizedString("UnsupportServerEdition", EnumStrings.ResourceManager, new object[]
			{
				edition
			});
		}

		public static LocalizedString UniversalDistributionGroupRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("UniversalDistributionGroupRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpamFilteringActionQuarantine
		{
			get
			{
				return new LocalizedString("SpamFilteringActionQuarantine", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCustomAttribute6
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute6", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupTypeFlagsNone
		{
			get
			{
				return new LocalizedString("GroupTypeFlagsNone", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContentIndexStatusFailed
		{
			get
			{
				return new LocalizedString("ContentIndexStatusFailed", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleOSP
		{
			get
			{
				return new LocalizedString("ServerRoleOSP", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeOtherFaxNumber
		{
			get
			{
				return new LocalizedString("ADAttributeOtherFaxNumber", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute15
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute15", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Enabled
		{
			get
			{
				return new LocalizedString("Enabled", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCustomAttribute7
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute7", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute1
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute1", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupTypeFlagsUniversal
		{
			get
			{
				return new LocalizedString("GroupTypeFlagsUniversal", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCustomAttribute11
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute11", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyExtensionCustomAttribute5
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyExtensionCustomAttribute5", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleExtendedRole5
		{
			get
			{
				return new LocalizedString("ServerRoleExtendedRole5", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OAuth
		{
			get
			{
				return new LocalizedString("OAuth", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeOtherHomePhoneNumber
		{
			get
			{
				return new LocalizedString("ADAttributeOtherHomePhoneNumber", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ArchiveStateLocal
		{
			get
			{
				return new LocalizedString("ArchiveStateLocal", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute13
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute13", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ComputerRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("ComputerRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LiveIdFba
		{
			get
			{
				return new LocalizedString("LiveIdFba", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeManager
		{
			get
			{
				return new LocalizedString("ADAttributeManager", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeOtherPhoneNumber
		{
			get
			{
				return new LocalizedString("ADAttributeOtherPhoneNumber", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleFfoWebServices
		{
			get
			{
				return new LocalizedString("ServerRoleFfoWebServices", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContentIndexStatusCrawling
		{
			get
			{
				return new LocalizedString("ContentIndexStatusCrawling", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MoveToArchive
		{
			get
			{
				return new LocalizedString("MoveToArchive", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MonitoringMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("MonitoringMailboxRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleAll
		{
			get
			{
				return new LocalizedString("ServerRoleAll", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyExtensionCustomAttribute4
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyExtensionCustomAttribute4", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WellKnownRecipientTypeResources
		{
			get
			{
				return new LocalizedString("WellKnownRecipientTypeResources", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute5
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute5", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WindowsIntegrated
		{
			get
			{
				return new LocalizedString("WindowsIntegrated", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SMTPAddress
		{
			get
			{
				return new LocalizedString("SMTPAddress", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeUserLogonName
		{
			get
			{
				return new LocalizedString("ADAttributeUserLogonName", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeNotes
		{
			get
			{
				return new LocalizedString("ADAttributeNotes", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedUserTypeDetails
		{
			get
			{
				return new LocalizedString("LinkedUserTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PromptForAlias
		{
			get
			{
				return new LocalizedString("PromptForAlias", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NonUniversalGroupRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("NonUniversalGroupRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeTitle
		{
			get
			{
				return new LocalizedString("ADAttributeTitle", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SIPSecured
		{
			get
			{
				return new LocalizedString("SIPSecured", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CopyStatusDismounting
		{
			get
			{
				return new LocalizedString("CopyStatusDismounting", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CopyStatusServiceDown
		{
			get
			{
				return new LocalizedString("CopyStatusServiceDown", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFolderMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("PublicFolderMailboxRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Quarantined
		{
			get
			{
				return new LocalizedString("Quarantined", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupTypeFlagsSecurityEnabled
		{
			get
			{
				return new LocalizedString("GroupTypeFlagsSecurityEnabled", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleNone
		{
			get
			{
				return new LocalizedString("ServerRoleNone", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EnterpriseEdition
		{
			get
			{
				return new LocalizedString("EnterpriseEdition", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AsyncOperationTypeCertExpiry
		{
			get
			{
				return new LocalizedString("AsyncOperationTypeCertExpiry", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalPartner
		{
			get
			{
				return new LocalizedString("ExternalPartner", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute3
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute3", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CopyStatusFailedAndSuspended
		{
			get
			{
				return new LocalizedString("CopyStatusFailedAndSuspended", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AllUsers
		{
			get
			{
				return new LocalizedString("AllUsers", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CopyStatusSuspended
		{
			get
			{
				return new LocalizedString("CopyStatusSuspended", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Journal
		{
			get
			{
				return new LocalizedString("Journal", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StandardEdition
		{
			get
			{
				return new LocalizedString("StandardEdition", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UndefinedRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("UndefinedRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CopyStatusSeedingSource
		{
			get
			{
				return new LocalizedString("CopyStatusSeedingSource", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ModeAuditAndNotify
		{
			get
			{
				return new LocalizedString("ModeAuditAndNotify", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCompany
		{
			get
			{
				return new LocalizedString("ADAttributeCompany", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EvaluatedUserRecipient
		{
			get
			{
				return new LocalizedString("EvaluatedUserRecipient", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Blocked
		{
			get
			{
				return new LocalizedString("Blocked", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalNonPartner
		{
			get
			{
				return new LocalizedString("ExternalNonPartner", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailEnabledContactRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("MailEnabledContactRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Unsecured
		{
			get
			{
				return new LocalizedString("Unsecured", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ArchiveStateHostedProvisioned
		{
			get
			{
				return new LocalizedString("ArchiveStateHostedProvisioned", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute9
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute9", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Calendar
		{
			get
			{
				return new LocalizedString("Calendar", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ArbitrationMailboxTypeDetails
		{
			get
			{
				return new LocalizedString("ArbitrationMailboxTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DisabledUserRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("DisabledUserRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CopyStatusUnknown
		{
			get
			{
				return new LocalizedString("CopyStatusUnknown", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LastFirst
		{
			get
			{
				return new LocalizedString("LastFirst", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTypePermissionControlled
		{
			get
			{
				return new LocalizedString("MessageTypePermissionControlled", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RssSubscriptions
		{
			get
			{
				return new LocalizedString("RssSubscriptions", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContentIndexStatusHealthy
		{
			get
			{
				return new LocalizedString("ContentIndexStatusHealthy", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCustomAttribute13
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute13", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Kerberos
		{
			get
			{
				return new LocalizedString("Kerberos", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CopyStatusHealthy
		{
			get
			{
				return new LocalizedString("CopyStatusHealthy", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoomListGroupTypeDetails
		{
			get
			{
				return new LocalizedString("RoomListGroupTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCustomAttribute9
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute9", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleCafe
		{
			get
			{
				return new LocalizedString("ServerRoleCafe", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute8
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute8", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SoftDelete
		{
			get
			{
				return new LocalizedString("SoftDelete", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalRelay
		{
			get
			{
				return new LocalizedString("ExternalRelay", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FirstLast
		{
			get
			{
				return new LocalizedString("FirstLast", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute14
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute14", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupTypeFlagsGlobal
		{
			get
			{
				return new LocalizedString("GroupTypeFlagsGlobal", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConferenceRoomMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("ConferenceRoomMailboxRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AsyncOperationTypeMailboxRestore
		{
			get
			{
				return new LocalizedString("AsyncOperationTypeMailboxRestore", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Unknown
		{
			get
			{
				return new LocalizedString("Unknown", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeState
		{
			get
			{
				return new LocalizedString("ADAttributeState", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("GroupMailboxRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeOffice
		{
			get
			{
				return new LocalizedString("ADAttributeOffice", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CopyStatusResynchronizing
		{
			get
			{
				return new LocalizedString("CopyStatusResynchronizing", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("LinkedMailboxRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UniversalSecurityGroupRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("UniversalSecurityGroupRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCustomAttribute12
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute12", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyExtensionCustomAttribute2
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyExtensionCustomAttribute2", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteRoomMailboxTypeDetails
		{
			get
			{
				return new LocalizedString("RemoteRoomMailboxTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title
		{
			get
			{
				return new LocalizedString("Title", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxUserRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("MailboxUserRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleExtendedRole4
		{
			get
			{
				return new LocalizedString("ServerRoleExtendedRole4", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleProvisionedServer
		{
			get
			{
				return new LocalizedString("ServerRoleProvisionedServer", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpamFilteringActionRedirect
		{
			get
			{
				return new LocalizedString("SpamFilteringActionRedirect", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute12
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute12", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailEnabledUniversalDistributionGroupRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("MailEnabledUniversalDistributionGroupRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotifyOnlyActionType
		{
			get
			{
				return new LocalizedString("NotifyOnlyActionType", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RejectMessageActionType
		{
			get
			{
				return new LocalizedString("RejectMessageActionType", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CopyStatusMisconfigured
		{
			get
			{
				return new LocalizedString("CopyStatusMisconfigured", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeHomePhoneNumber
		{
			get
			{
				return new LocalizedString("ADAttributeHomePhoneNumber", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Contacts
		{
			get
			{
				return new LocalizedString("Contacts", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LegacyArchiveJournals
		{
			get
			{
				return new LocalizedString("LegacyArchiveJournals", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyDepartment
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyDepartment", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleManagementBackEnd
		{
			get
			{
				return new LocalizedString("ServerRoleManagementBackEnd", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Digest
		{
			get
			{
				return new LocalizedString("Digest", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AsyncOperationTypeMigration
		{
			get
			{
				return new LocalizedString("AsyncOperationTypeMigration", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CopyStatusFailed
		{
			get
			{
				return new LocalizedString("CopyStatusFailed", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Notes
		{
			get
			{
				return new LocalizedString("Notes", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleExtendedRole2
		{
			get
			{
				return new LocalizedString("ServerRoleExtendedRole2", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NegoEx
		{
			get
			{
				return new LocalizedString("NegoEx", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTypeEncrypted
		{
			get
			{
				return new LocalizedString("MessageTypeEncrypted", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute10
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute10", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailEnabledDynamicDistributionGroupRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("MailEnabledDynamicDistributionGroupRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContentIndexStatusSuspended
		{
			get
			{
				return new LocalizedString("ContentIndexStatusSuspended", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Fba
		{
			get
			{
				return new LocalizedString("Fba", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CopyStatusDisconnectedAndHealthy
		{
			get
			{
				return new LocalizedString("CopyStatusDisconnectedAndHealthy", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailEnabledForestContactRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("MailEnabledForestContactRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmail
		{
			get
			{
				return new LocalizedString("JunkEmail", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleMonitoring
		{
			get
			{
				return new LocalizedString("ServerRoleMonitoring", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SystemMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("SystemMailboxRecipientTypeDetails", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCountryCode
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCountryCode", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CopyStatusSeeding
		{
			get
			{
				return new LocalizedString("CopyStatusSeeding", EnumStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(EnumStrings.IDs key)
		{
			return new LocalizedString(EnumStrings.stringIDs[(uint)key], EnumStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(324);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Management.ControlPanel.EnumStrings", typeof(EnumStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			CopyStatusDisconnected = 2831161206U,
			Inbox = 2979702410U,
			G711 = 3692015522U,
			ServerRoleExtendedRole3 = 3707194053U,
			ServerRoleMailbox = 2125756541U,
			ServerRoleUnifiedMessaging = 3194934827U,
			CopyStatusMounting = 1208301054U,
			GroupNamingPolicyOffice = 62599113U,
			WellKnownRecipientTypeMailGroups = 3725493575U,
			EnterpriseTrialEdition = 4134731995U,
			ADAttributeCustomAttribute1 = 1377545167U,
			ServerRoleEdge = 756854696U,
			CoexistenceTrialEdition = 2191591450U,
			ServerRoleHubTransport = 172810921U,
			ADAttributeLastName = 1016721882U,
			TeamMailboxRecipientTypeDetails = 107906018U,
			ContentIndexStatusDisabled = 2385442291U,
			CcRecipientType = 2476473719U,
			FallbackIgnore = 2328530086U,
			MailEnabledUniversalSecurityGroupRecipientTypeDetails = 1970247521U,
			CopyStatusMounted = 623586765U,
			Gsm = 3115737533U,
			GroupNamingPolicyCustomAttribute6 = 1924734198U,
			MessageTypeOof = 2129610537U,
			GroupNamingPolicyCompany = 862838650U,
			SpamFilteringOptionOn = 2654331961U,
			None = 1414246128U,
			ConversationHistory = 2630084427U,
			NoNewCalls = 1479682494U,
			Adfs = 1716515484U,
			Certificate = 1277957481U,
			ConnectorTypePartner = 2003039265U,
			ADAttributeInitials = 2468414724U,
			AsyncOperationTypeUnknown = 135933047U,
			TelExtn = 1059422816U,
			Authoritative = 2913015079U,
			MicrosoftExchangeRecipientTypeDetails = 2227674028U,
			LiveIdNegotiate = 1091035505U,
			SpecificUsers = 1677492010U,
			ADAttributeCustomAttribute8 = 1377545174U,
			WSSecurity = 1690995826U,
			MessageTypeAutoForward = 2041595767U,
			Misconfigured = 3211494971U,
			DatabaseMasterTypeDag = 2773964607U,
			MessageTypeReadReceipt = 680782013U,
			MoveToDeletedItems = 3341243277U,
			RedirectRecipientType = 2611996929U,
			RemoteUserMailboxTypeDetails = 4145265495U,
			ContentIndexStatusAutoSuspended = 393808047U,
			ExternalManagedMailContactTypeDetails = 3799817423U,
			Negotiate = 635036566U,
			ServerRoleFrontendTransport = 3786203794U,
			CopyStatusDisconnectedAndResynchronizing = 3655046083U,
			Allowed = 3811183882U,
			GroupNamingPolicyExtensionCustomAttribute3 = 3197896354U,
			ADAttributeName = 3050431750U,
			Disabled = 1484405454U,
			ConnectorTypeOnPremises = 4060482376U,
			MessageTypeApprovalRequest = 4072748617U,
			RejectUnlessExplicitOverrideActionType = 3115100581U,
			WellKnownRecipientTypeMailboxUsers = 2167560764U,
			FallbackReject = 3144351139U,
			RemoteSharedMailboxTypeDetails = 444885611U,
			Secured = 1241597555U,
			MoveToFolder = 1182470434U,
			ADAttributeCustomAttribute5 = 1377545163U,
			ToRecipientType = 4199979286U,
			RoleGroupTypeDetails = 3221974997U,
			Tasks = 2966158940U,
			DatabaseMasterTypeServer = 2173147846U,
			UserRecipientTypeDetails = 2338964630U,
			CopyStatusNonExchangeReplication = 3990056197U,
			CopyStatusNotConfigured = 1310067130U,
			DeviceDiscovery = 1010456570U,
			ContactRecipientTypeDetails = 137387861U,
			ContentIndexStatusSeeding = 1361373610U,
			GroupNamingPolicyCustomAttribute11 = 2423361114U,
			Ntlm = 3168546739U,
			SentItems = 590977256U,
			ADAttributeCustomAttribute3 = 1377545169U,
			ADAttributePagerNumber = 3850073087U,
			ADAttributeStreet = 2002903510U,
			Wma = 2665399355U,
			GroupNamingPolicyCity = 2703120928U,
			NonIpmRoot = 600983985U,
			AsyncOperationTypeExportPST = 1937417240U,
			UnknownEdition = 2889762178U,
			ModeEnforce = 41715449U,
			EvaluationNotEqual = 3918497079U,
			WellKnownRecipientTypeAllRecipients = 2099880135U,
			ADAttributeCustomAttribute14 = 1048761747U,
			DatabaseMasterTypeUnknown = 661425765U,
			ADAttributePhoneNumber = 4137481806U,
			GroupNamingPolicyCustomAttribute4 = 1924734196U,
			StandardTrialEdition = 553174585U,
			PersonalFolder = 2283186478U,
			LiveIdBasic = 754287197U,
			WellKnownRecipientTypeMailUsers = 933193541U,
			SystemAttendantMailboxRecipientTypeDetails = 1818643265U,
			CopyStatusInitializing = 1373187244U,
			ServerRoleClientAccess = 1052758952U,
			MessageTypeCalendaring = 1903193717U,
			SyncIssues = 3694564633U,
			AlwaysEnabled = 798637440U,
			ContentIndexStatusUnknown = 1631091055U,
			SharedMailboxRecipientTypeDetails = 4263249978U,
			InternalRelay = 3288506612U,
			CoexistenceEdition = 1474747046U,
			GroupNamingPolicyCustomAttribute7 = 1924734197U,
			Outbox = 629464291U,
			ADAttributeCustomAttribute15 = 2614845688U,
			WellKnownRecipientTypeNone = 1849540794U,
			ManagementRelationshipManager = 25634710U,
			ServerRoleCafeArray = 986970413U,
			ExternalManagedGroupTypeDetails = 1097129869U,
			ArchiveStateNone = 3086386447U,
			EvaluatedUserSender = 2509095413U,
			IncidentReportIncludeOriginalMail = 1389339898U,
			GroupNamingPolicyExtensionCustomAttribute1 = 65728472U,
			ADAttributeEmail = 4289093673U,
			E164 = 438888054U,
			All = 4231482709U,
			ManagementRelationshipDirectReport = 428619956U,
			InheritFromDialPlan = 637440764U,
			EvaluationEqual = 3459736224U,
			MailboxPlanTypeDetails = 1094750789U,
			FallbackWrap = 3262572344U,
			ADAttributeCustomAttribute4 = 1377545162U,
			ADAttributeDepartment = 3367615085U,
			SpamFilteringOptionTest = 2944126402U,
			Private = 3026477473U,
			ADAttributeCity = 4226527350U,
			DiscoveryMailboxTypeDetails = 104454802U,
			ADAttributePOBox = 4260106383U,
			ServerRoleExtendedRole7 = 3707194057U,
			Everyone = 3708929833U,
			LegacyMailboxRecipientTypeDetails = 221683052U,
			ADAttributeFaxNumber = 2182511137U,
			IncidentReportDoNotIncludeOriginalMail = 29398792U,
			ExternalUser = 3631693406U,
			RemoteEquipmentMailboxTypeDetails = 1406382714U,
			Tag = 696030922U,
			GroupTypeFlagsBuiltinLocal = 1494101274U,
			ServerRoleManagementFrontEnd = 3802186670U,
			GroupNamingPolicyStateOrProvince = 4088287609U,
			ArchiveStateHostedPending = 920444171U,
			RemoteTeamMailboxRecipientTypeDetails = 322963092U,
			ADAttributeZipCode = 381216251U,
			PermanentlyDelete = 3675904764U,
			Location = 2325276717U,
			EquipmentMailboxRecipientTypeDetails = 3938481035U,
			CopyStatusDismounted = 3673730471U,
			SipName = 3423767853U,
			ModeAudit = 3869829980U,
			DumpsterFolder = 3641768400U,
			Organizational = 1067650092U,
			ADAttributeFirstName = 2986926906U,
			ServerRoleSCOM = 407788899U,
			DeletedItems = 3613623199U,
			GroupNamingPolicyCustomAttribute2 = 1924734194U,
			ADAttributeCustomAttribute2 = 1377545168U,
			GroupTypeFlagsDomainLocal = 1638178773U,
			ServerRoleCentralAdminFrontEnd = 3980237751U,
			InternalUser = 2795331228U,
			ContentIndexStatusFailedAndSuspended = 1923042104U,
			ADAttributeCountry = 3600528589U,
			SpamFilteringOptionOff = 2030161115U,
			GroupNamingPolicyTitle = 4137211921U,
			BccRecipientType = 1798370525U,
			AsyncOperationTypeImportPST = 4181674605U,
			RejectUnlessFalsePositiveOverrideActionType = 2736707353U,
			SpamFilteringActionDelete = 3918345138U,
			PublicFolderRecipientTypeDetails = 1625030180U,
			SpamFilteringActionAddXHeader = 685401583U,
			SpamFilteringActionModifySubject = 2349327181U,
			ContentIndexStatusHealthyAndUpgrading = 3268869348U,
			Basic = 4128944152U,
			Department = 1855823700U,
			MessageTypeSigned = 3606274629U,
			WellKnownRecipientTypeMailContacts = 2638599330U,
			ADAttributeMobileNumber = 2411750738U,
			MessageTypeVoicemail = 117825870U,
			MailEnabledUserRecipientTypeDetails = 3689869554U,
			Mp3 = 1549653732U,
			RejectUnlessSilentOverrideActionType = 2447598924U,
			GroupNamingPolicyCountryOrRegion = 3674978674U,
			ServerRoleLanguagePacks = 2698858797U,
			CopyStatusSinglePageRestore = 1359519288U,
			MailEnabledNonUniversalGroupRecipientTypeDetails = 3376217818U,
			SpamFilteringActionJmf = 1123996746U,
			Drafts = 115734878U,
			ADAttributeCustomAttribute10 = 3374360575U,
			ArchiveStateOnPremise = 110833865U,
			UniversalDistributionGroupRecipientTypeDetails = 1966081841U,
			SpamFilteringActionQuarantine = 2852597951U,
			ADAttributeCustomAttribute6 = 1377545164U,
			GroupTypeFlagsNone = 252422050U,
			ContentIndexStatusFailed = 2562345274U,
			ServerRoleOSP = 2775202161U,
			ADAttributeOtherFaxNumber = 3689464497U,
			GroupNamingPolicyCustomAttribute15 = 97762286U,
			Enabled = 634395589U,
			ADAttributeCustomAttribute7 = 1377545165U,
			GroupNamingPolicyCustomAttribute1 = 1924734191U,
			GroupTypeFlagsUniversal = 1191186633U,
			ADAttributeCustomAttribute11 = 645477220U,
			GroupNamingPolicyExtensionCustomAttribute5 = 2391327300U,
			ServerRoleExtendedRole5 = 3707194059U,
			OAuth = 3309342631U,
			ADAttributeOtherHomePhoneNumber = 856583401U,
			ArchiveStateLocal = 665936024U,
			GroupNamingPolicyCustomAttribute13 = 3586160528U,
			ComputerRecipientTypeDetails = 3489169852U,
			LiveIdFba = 1582423804U,
			ADAttributeManager = 494686544U,
			ADAttributeOtherPhoneNumber = 3162495226U,
			ServerRoleFfoWebServices = 3464146580U,
			ContentIndexStatusCrawling = 1575862374U,
			MoveToArchive = 2835967712U,
			MonitoringMailboxRecipientTypeDetails = 729925097U,
			ServerRoleAll = 570563164U,
			GroupNamingPolicyExtensionCustomAttribute4 = 825243359U,
			WellKnownRecipientTypeResources = 3773054995U,
			GroupNamingPolicyCustomAttribute5 = 1924734195U,
			WindowsIntegrated = 872998734U,
			SMTPAddress = 980672066U,
			ADAttributeUserLogonName = 1452889642U,
			ADAttributeNotes = 863112602U,
			LinkedUserTypeDetails = 1738880682U,
			PromptForAlias = 2303788021U,
			NonUniversalGroupRecipientTypeDetails = 2227190334U,
			ADAttributeTitle = 2634964433U,
			SIPSecured = 2422734853U,
			CopyStatusDismounting = 4189810048U,
			CopyStatusServiceDown = 729299916U,
			PublicFolderMailboxRecipientTypeDetails = 1487832074U,
			Quarantined = 996355914U,
			GroupTypeFlagsSecurityEnabled = 3200416695U,
			ServerRoleNone = 2094315795U,
			EnterpriseEdition = 26915469U,
			AsyncOperationTypeCertExpiry = 2045069482U,
			ExternalPartner = 715964235U,
			GroupNamingPolicyCustomAttribute3 = 1924734193U,
			CopyStatusFailedAndSuspended = 1765158362U,
			AllUsers = 3949283739U,
			CopyStatusSuspended = 2605454650U,
			Journal = 4137480277U,
			StandardEdition = 2321790947U,
			UndefinedRecipientTypeDetails = 3453679227U,
			CopyStatusSeedingSource = 2160282563U,
			ModeAuditAndNotify = 230388220U,
			ADAttributeCompany = 2891753468U,
			EvaluatedUserRecipient = 2030715989U,
			Blocked = 4019774802U,
			ExternalNonPartner = 2155604814U,
			MailEnabledContactRecipientTypeDetails = 3815678973U,
			Unsecured = 1573777228U,
			ArchiveStateHostedProvisioned = 2472951404U,
			GroupNamingPolicyCustomAttribute9 = 1924734199U,
			Calendar = 1292798904U,
			ArbitrationMailboxTypeDetails = 3647297993U,
			DisabledUserRecipientTypeDetails = 3569405894U,
			CopyStatusUnknown = 1960737953U,
			LastFirst = 142823596U,
			MessageTypePermissionControlled = 2872629304U,
			RssSubscriptions = 3598244064U,
			ContentIndexStatusHealthy = 4010596708U,
			ADAttributeCustomAttribute13 = 1808276634U,
			Kerberos = 645017541U,
			CopyStatusHealthy = 1484668346U,
			RoomListGroupTypeDetails = 1391517930U,
			ADAttributeCustomAttribute9 = 1377545175U,
			ServerRoleCafe = 1536572748U,
			GroupNamingPolicyCustomAttribute8 = 1924734200U,
			SoftDelete = 3133553171U,
			ExternalRelay = 2171581398U,
			FirstLast = 2300412432U,
			GroupNamingPolicyCustomAttribute14 = 1663846227U,
			GroupTypeFlagsGlobal = 4189167987U,
			ConferenceRoomMailboxRecipientTypeDetails = 1919306754U,
			AsyncOperationTypeMailboxRestore = 1588035907U,
			Unknown = 2846264340U,
			ADAttributeState = 3882899654U,
			GroupMailboxRecipientTypeDetails = 2223810040U,
			ADAttributeOffice = 1927573801U,
			CopyStatusResynchronizing = 1545501201U,
			LinkedMailboxRecipientTypeDetails = 1432667858U,
			UniversalSecurityGroupRecipientTypeDetails = 968858937U,
			ADAttributeCustomAttribute12 = 242192693U,
			GroupNamingPolicyExtensionCustomAttribute2 = 1631812413U,
			RemoteRoomMailboxTypeDetails = 1594549261U,
			Title = 2435266816U,
			MailboxUserRecipientTypeDetails = 1605633982U,
			ServerRoleExtendedRole4 = 3707194060U,
			ServerRoleProvisionedServer = 1922689150U,
			SpamFilteringActionRedirect = 4255105347U,
			GroupNamingPolicyCustomAttribute12 = 857277173U,
			MailEnabledUniversalDistributionGroupRecipientTypeDetails = 562488721U,
			NotifyOnlyActionType = 604363629U,
			RejectMessageActionType = 498572210U,
			CopyStatusMisconfigured = 1587179080U,
			ADAttributeHomePhoneNumber = 1457839961U,
			Contacts = 1716044995U,
			LegacyArchiveJournals = 3635271833U,
			GroupNamingPolicyDepartment = 154085973U,
			ServerRoleManagementBackEnd = 696678862U,
			Digest = 815864422U,
			AsyncOperationTypeMigration = 1702371863U,
			CopyStatusFailed = 4015121608U,
			Notes = 1601836855U,
			ServerRoleExtendedRole2 = 3707194054U,
			NegoEx = 273163868U,
			MessageTypeEncrypted = 3544120613U,
			GroupNamingPolicyCustomAttribute10 = 3989445055U,
			MailEnabledDynamicDistributionGroupRecipientTypeDetails = 2999125469U,
			ContentIndexStatusSuspended = 1056819816U,
			Fba = 1099314853U,
			CopyStatusDisconnectedAndHealthy = 3985647980U,
			MailEnabledForestContactRecipientTypeDetails = 3586618070U,
			JunkEmail = 2241039844U,
			ServerRoleMonitoring = 1024471425U,
			SystemMailboxRecipientTypeDetails = 1850977098U,
			GroupNamingPolicyCountryCode = 4022404286U,
			CopyStatusSeeding = 448862132U
		}

		private enum ParamIDs
		{
			UnsupportServerEdition
		}
	}
}
