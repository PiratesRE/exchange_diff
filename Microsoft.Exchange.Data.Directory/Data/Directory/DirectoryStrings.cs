using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class DirectoryStrings
	{
		static DirectoryStrings()
		{
			DirectoryStrings.stringIDs.Add(2223810040U, "GroupMailboxRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(3059540560U, "InvalidTransportSyncHealthLogSizeConfiguration");
			DirectoryStrings.stringIDs.Add(3553131525U, "ReceiveExtendedProtectionPolicyNone");
			DirectoryStrings.stringIDs.Add(1352261648U, "OrganizationCapabilityManagement");
			DirectoryStrings.stringIDs.Add(3384994469U, "EsnLangTamil");
			DirectoryStrings.stringIDs.Add(1623026330U, "LdapFilterErrorInvalidWildCard");
			DirectoryStrings.stringIDs.Add(3266435989U, "Individual");
			DirectoryStrings.stringIDs.Add(2171581398U, "ExternalRelay");
			DirectoryStrings.stringIDs.Add(1719230762U, "InvalidTransportSyncDownloadSizeConfiguration");
			DirectoryStrings.stringIDs.Add(2928684304U, "MessageRateSourceFlagsAll");
			DirectoryStrings.stringIDs.Add(4209173728U, "SKUCapabilityBPOSSBasic");
			DirectoryStrings.stringIDs.Add(3376948578U, "IndustryMediaMarketingAdvertising");
			DirectoryStrings.stringIDs.Add(2570737323U, "SKUCapabilityUnmanaged");
			DirectoryStrings.stringIDs.Add(4073959654U, "BackSyncDataSourceTransientErrorMessage");
			DirectoryStrings.stringIDs.Add(3376217818U, "MailEnabledNonUniversalGroupRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(3806493464U, "ADDriverStoreAccessPermanentError");
			DirectoryStrings.stringIDs.Add(3323369056U, "DeviceType");
			DirectoryStrings.stringIDs.Add(3229570631U, "EsnLangFarsi");
			DirectoryStrings.stringIDs.Add(3503019411U, "InvalidTempErrorSetting");
			DirectoryStrings.stringIDs.Add(3874381006U, "ReplicationTypeNone");
			DirectoryStrings.stringIDs.Add(1593550494U, "IndustryBusinessServicesConsulting");
			DirectoryStrings.stringIDs.Add(210447381U, "ErrorAdfsConfigFormat");
			DirectoryStrings.stringIDs.Add(996355914U, "Quarantined");
			DirectoryStrings.stringIDs.Add(3114754472U, "OutboundConnectorSmartHostShouldBePresentIfUseMXRecordFalse");
			DirectoryStrings.stringIDs.Add(364260824U, "LongRunningCostHandle");
			DirectoryStrings.stringIDs.Add(3479185892U, "EsnLangChineseTraditional");
			DirectoryStrings.stringIDs.Add(1803878278U, "IndustryTransportation");
			DirectoryStrings.stringIDs.Add(815885189U, "Silent");
			DirectoryStrings.stringIDs.Add(3145530267U, "AlternateServiceAccountCredentialQualifiedUserNameWrongFormat");
			DirectoryStrings.stringIDs.Add(1840954879U, "InvalidBannerSetting");
			DirectoryStrings.stringIDs.Add(1924734196U, "GroupNamingPolicyCustomAttribute4");
			DirectoryStrings.stringIDs.Add(230574830U, "InboundConnectorIncorrectCloudServicesMailEnabled");
			DirectoryStrings.stringIDs.Add(3936814429U, "LdapFilterErrorAnrIsNotSupported");
			DirectoryStrings.stringIDs.Add(438888054U, "E164");
			DirectoryStrings.stringIDs.Add(1874297349U, "ErrorAuthMetaDataContentEmpty");
			DirectoryStrings.stringIDs.Add(2285243143U, "MailEnabledContactRecipientType");
			DirectoryStrings.stringIDs.Add(562488721U, "MailEnabledUniversalDistributionGroupRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(360676809U, "SendAuthMechanismExternalAuthoritative");
			DirectoryStrings.stringIDs.Add(528770240U, "InboundConnectorRequiredTlsSettingsInvalid");
			DirectoryStrings.stringIDs.Add(1924734191U, "GroupNamingPolicyCustomAttribute1");
			DirectoryStrings.stringIDs.Add(1743625100U, "Dual");
			DirectoryStrings.stringIDs.Add(1822619008U, "DatabaseCopyAutoActivationPolicyIntrasiteOnly");
			DirectoryStrings.stringIDs.Add(1594930120U, "Never");
			DirectoryStrings.stringIDs.Add(1644017996U, "ByteEncoderTypeUndefined");
			DirectoryStrings.stringIDs.Add(3817431401U, "InvalidRcvProtocolLogSizeConfiguration");
			DirectoryStrings.stringIDs.Add(2785880074U, "GetRootDseRequiresDomainController");
			DirectoryStrings.stringIDs.Add(637440764U, "InheritFromDialPlan");
			DirectoryStrings.stringIDs.Add(737697267U, "OrganizationCapabilityMessageTracking");
			DirectoryStrings.stringIDs.Add(1573064009U, "InboundConnectorInvalidTlsSenderCertificateName");
			DirectoryStrings.stringIDs.Add(3133553171U, "SoftDelete");
			DirectoryStrings.stringIDs.Add(2570855206U, "OrganizationCapabilityUMGrammar");
			DirectoryStrings.stringIDs.Add(1776488541U, "Allow");
			DirectoryStrings.stringIDs.Add(1261795780U, "DomainNameIsNull");
			DirectoryStrings.stringIDs.Add(2303788021U, "PromptForAlias");
			DirectoryStrings.stringIDs.Add(3614458512U, "ErrorSystemAddressListInWrongContainer");
			DirectoryStrings.stringIDs.Add(651742924U, "ExceptionUnableToDisableAdminTopologyMode");
			DirectoryStrings.stringIDs.Add(1241597555U, "Secured");
			DirectoryStrings.stringIDs.Add(2462615416U, "ExternalAndAuthSet");
			DirectoryStrings.stringIDs.Add(597372135U, "EsnLangJapanese");
			DirectoryStrings.stringIDs.Add(1391104995U, "EsnLangPortuguesePortugal");
			DirectoryStrings.stringIDs.Add(2487111597U, "EsnLangFinnish");
			DirectoryStrings.stringIDs.Add(2811972280U, "ExceptionOwaCannotSetPropertyOnVirtualDirectoryOtherThanExchweb");
			DirectoryStrings.stringIDs.Add(2180736490U, "WhenDelivered");
			DirectoryStrings.stringIDs.Add(2839188613U, "DomainStatePendingRelease");
			DirectoryStrings.stringIDs.Add(1631812413U, "GroupNamingPolicyExtensionCustomAttribute2");
			DirectoryStrings.stringIDs.Add(2127564032U, "AutoGroup");
			DirectoryStrings.stringIDs.Add(3880444299U, "ErrorStartDateExpiration");
			DirectoryStrings.stringIDs.Add(285960632U, "MailboxMoveStatusQueued");
			DirectoryStrings.stringIDs.Add(2220842206U, "Minute");
			DirectoryStrings.stringIDs.Add(590977256U, "SentItems");
			DirectoryStrings.stringIDs.Add(4029642168U, "ExchangeVoicemailMC");
			DirectoryStrings.stringIDs.Add(3010978409U, "AppliedInFull");
			DirectoryStrings.stringIDs.Add(2215231792U, "NoAddressSpaces");
			DirectoryStrings.stringIDs.Add(1387109368U, "SKUCapabilityEOPStandardAddOn");
			DirectoryStrings.stringIDs.Add(2260925979U, "IndustryNonProfit");
			DirectoryStrings.stringIDs.Add(3413117907U, "EsnLangDefault");
			DirectoryStrings.stringIDs.Add(707064308U, "SpecifyCustomGreetingFileName");
			DirectoryStrings.stringIDs.Add(671952847U, "EsnLangSlovenian");
			DirectoryStrings.stringIDs.Add(1059422816U, "TelExtn");
			DirectoryStrings.stringIDs.Add(4045631128U, "LdapFilterErrorInvalidGtLtOperand");
			DirectoryStrings.stringIDs.Add(434185288U, "SystemMailboxRecipientType");
			DirectoryStrings.stringIDs.Add(2824730050U, "ReplicationTypeRemote");
			DirectoryStrings.stringIDs.Add(1414596097U, "Enterprise");
			DirectoryStrings.stringIDs.Add(3115737533U, "Gsm");
			DirectoryStrings.stringIDs.Add(4137480277U, "Journal");
			DirectoryStrings.stringIDs.Add(924597469U, "SpamFilteringTestActionNone");
			DirectoryStrings.stringIDs.Add(1877544294U, "CustomRoleDescription_MyPersonalInformation");
			DirectoryStrings.stringIDs.Add(1608856269U, "MailboxMoveStatusAutoSuspended");
			DirectoryStrings.stringIDs.Add(3068683316U, "Any");
			DirectoryStrings.stringIDs.Add(2325276717U, "Location");
			DirectoryStrings.stringIDs.Add(3031587385U, "ExternalTrust");
			DirectoryStrings.stringIDs.Add(2830455760U, "IndustryPrintingPublishing");
			DirectoryStrings.stringIDs.Add(3577501733U, "AllComputers");
			DirectoryStrings.stringIDs.Add(3208958876U, "ExceptionRusNotFound");
			DirectoryStrings.stringIDs.Add(2703120928U, "GroupNamingPolicyCity");
			DirectoryStrings.stringIDs.Add(1213668011U, "NoPagesSpecified");
			DirectoryStrings.stringIDs.Add(2286842903U, "PublicDatabaseRecipientType");
			DirectoryStrings.stringIDs.Add(4050233751U, "CanEnableLocalCopyState_CanBeEnabled");
			DirectoryStrings.stringIDs.Add(1069069500U, "RedirectToRecipientsNotSet");
			DirectoryStrings.stringIDs.Add(2472102570U, "InfoAnnouncementEnabled");
			DirectoryStrings.stringIDs.Add(3867415726U, "ConfigurationSettingsADConfigDriverError");
			DirectoryStrings.stringIDs.Add(1919923094U, "LdapFilterErrorEscCharWithoutEscapable");
			DirectoryStrings.stringIDs.Add(2829212743U, "IndustryGovernment");
			DirectoryStrings.stringIDs.Add(1921301802U, "CustomRoleDescription_MyAddressInformation");
			DirectoryStrings.stringIDs.Add(861191034U, "EsnLangNorwegianNynorsk");
			DirectoryStrings.stringIDs.Add(1996416364U, "IndustryEngineeringArchitecture");
			DirectoryStrings.stringIDs.Add(2551449657U, "SendAuthMechanismBasicAuth");
			DirectoryStrings.stringIDs.Add(4060941198U, "SKUCapabilityEOPPremiumAddOn");
			DirectoryStrings.stringIDs.Add(1820951283U, "ErrorResourceTypeInvalid");
			DirectoryStrings.stringIDs.Add(2134869325U, "OrgContainerNotFoundException");
			DirectoryStrings.stringIDs.Add(4076766949U, "SKUCapabilityBPOSSStandardArchive");
			DirectoryStrings.stringIDs.Add(1359848478U, "InternalSenderAdminAddressRequired");
			DirectoryStrings.stringIDs.Add(237887777U, "CannotGetUsefulDomainInfo");
			DirectoryStrings.stringIDs.Add(332960507U, "ErrorElcSuspensionNotEnabled");
			DirectoryStrings.stringIDs.Add(2173147846U, "DatabaseMasterTypeServer");
			DirectoryStrings.stringIDs.Add(3421832948U, "ConnectionTimeoutLessThanInactivityTimeout");
			DirectoryStrings.stringIDs.Add(444871648U, "HygieneSuitePremium");
			DirectoryStrings.stringIDs.Add(2986397362U, "Exadmin");
			DirectoryStrings.stringIDs.Add(2704331370U, "ExceptionADTopologyCannotFindWellKnownExchangeGroup");
			DirectoryStrings.stringIDs.Add(2979126483U, "CommandFrequency");
			DirectoryStrings.stringIDs.Add(1861502555U, "IndustryConstruction");
			DirectoryStrings.stringIDs.Add(4263249978U, "SharedMailboxRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(2830208040U, "AccessDeniedToEventLog");
			DirectoryStrings.stringIDs.Add(1445823720U, "EsnLangSerbian");
			DirectoryStrings.stringIDs.Add(2242173198U, "ReplicationTypeUnknown");
			DirectoryStrings.stringIDs.Add(2058453416U, "ErrorDuplicateMapiIdsInConfiguredAttributes");
			DirectoryStrings.stringIDs.Add(56170716U, "DirectoryBasedEdgeBlockModeOn");
			DirectoryStrings.stringIDs.Add(2380978387U, "LiveCredentialWithoutBasic");
			DirectoryStrings.stringIDs.Add(1607133185U, "ExclusiveConfigScopes");
			DirectoryStrings.stringIDs.Add(1118921376U, "IndustryRealEstate");
			DirectoryStrings.stringIDs.Add(2956380840U, "EsnLangNorwegian");
			DirectoryStrings.stringIDs.Add(1024471425U, "ServerRoleMonitoring");
			DirectoryStrings.stringIDs.Add(1106844962U, "ASInvalidAccessMethod");
			DirectoryStrings.stringIDs.Add(403740404U, "NotApplied");
			DirectoryStrings.stringIDs.Add(2619186021U, "ConfigurationSettingsADNotificationError");
			DirectoryStrings.stringIDs.Add(729925097U, "MonitoringMailboxRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(56435549U, "EsnLangCroatian");
			DirectoryStrings.stringIDs.Add(2386455749U, "TlsAuthLevelWithDomainSecureEnabled");
			DirectoryStrings.stringIDs.Add(4104902926U, "EsnLangGerman");
			DirectoryStrings.stringIDs.Add(3816616305U, "RoleAssignmentPolicyDescription_Default");
			DirectoryStrings.stringIDs.Add(252422050U, "GroupTypeFlagsNone");
			DirectoryStrings.stringIDs.Add(2167560764U, "WellKnownRecipientTypeMailboxUsers");
			DirectoryStrings.stringIDs.Add(1324265457U, "LdapFilterErrorInvalidWildCardGtLt");
			DirectoryStrings.stringIDs.Add(3536945350U, "SmartHostNotSet");
			DirectoryStrings.stringIDs.Add(3531789014U, "DeviceRule");
			DirectoryStrings.stringIDs.Add(1299460569U, "NotTrust");
			DirectoryStrings.stringIDs.Add(2824779686U, "EmailAgeFilterAll");
			DirectoryStrings.stringIDs.Add(1524653102U, "LanguageBlockListNotSet");
			DirectoryStrings.stringIDs.Add(2547761307U, "EsnLangSerbianCyrillic");
			DirectoryStrings.stringIDs.Add(1551636376U, "CalendarAgeFilterSixMonths");
			DirectoryStrings.stringIDs.Add(1442063141U, "ErrorMetadataNotSearchProperty");
			DirectoryStrings.stringIDs.Add(52431374U, "InvalidDefaultMailbox");
			DirectoryStrings.stringIDs.Add(115734878U, "Drafts");
			DirectoryStrings.stringIDs.Add(2819908830U, "RemoteGroupMailboxRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(289102393U, "EsnLangSwahili");
			DirectoryStrings.stringIDs.Add(794922827U, "ExceptionPagedReaderReadAllAfterEnumerating");
			DirectoryStrings.stringIDs.Add(835151952U, "DsnDefaultLanguageMustBeSpecificCulture");
			DirectoryStrings.stringIDs.Add(719400811U, "BestBodyFormat");
			DirectoryStrings.stringIDs.Add(3064393132U, "CanEnableLocalCopyState_AlreadyEnabled");
			DirectoryStrings.stringIDs.Add(1010456570U, "DeviceDiscovery");
			DirectoryStrings.stringIDs.Add(3963885811U, "AccessDenied");
			DirectoryStrings.stringIDs.Add(29593584U, "InvalidActiveUserStatisticsLogSizeConfiguration");
			DirectoryStrings.stringIDs.Add(1014182364U, "ErrorActionOnExpirationSpecified");
			DirectoryStrings.stringIDs.Add(3776155750U, "TlsAuthLevelWithNoDomainOnSmartHost");
			DirectoryStrings.stringIDs.Add(1587883572U, "DeferredFailoverEntryString");
			DirectoryStrings.stringIDs.Add(4110637509U, "TaskItemsMC");
			DirectoryStrings.stringIDs.Add(1924734197U, "GroupNamingPolicyCustomAttribute7");
			DirectoryStrings.stringIDs.Add(2647513696U, "UnknownAttribute");
			DirectoryStrings.stringIDs.Add(703634004U, "MountDialOverrideBestAvailability");
			DirectoryStrings.stringIDs.Add(3562314173U, "ErrorArbitrationMailboxPropertyEmailAddressesEmpty");
			DirectoryStrings.stringIDs.Add(608628016U, "AlternateServiceAccountCredentialNotSet");
			DirectoryStrings.stringIDs.Add(2092029626U, "DataMoveReplicationConstraintAllCopies");
			DirectoryStrings.stringIDs.Add(3902771789U, "GlobalThrottlingPolicyAmbiguousException");
			DirectoryStrings.stringIDs.Add(2998146498U, "InvalidServerStatisticsLogSizeConfiguration");
			DirectoryStrings.stringIDs.Add(3361458474U, "SipResourceIdRequired");
			DirectoryStrings.stringIDs.Add(1616139245U, "EsnLangPortuguese");
			DirectoryStrings.stringIDs.Add(4241336410U, "AutoDetect");
			DirectoryStrings.stringIDs.Add(4255105347U, "SpamFilteringActionRedirect");
			DirectoryStrings.stringIDs.Add(3532819894U, "CanRunRestoreState_Invalid");
			DirectoryStrings.stringIDs.Add(2530566197U, "OutboundConnectorIncorrectCloudServicesMailEnabled");
			DirectoryStrings.stringIDs.Add(95325995U, "DatabaseCopyAutoActivationPolicyBlocked");
			DirectoryStrings.stringIDs.Add(3838277697U, "CustomRoleDescription_MyName");
			DirectoryStrings.stringIDs.Add(3720125794U, "EsnLangOriya");
			DirectoryStrings.stringIDs.Add(702028774U, "UserAgent");
			DirectoryStrings.stringIDs.Add(4211599483U, "DomainStateActive");
			DirectoryStrings.stringIDs.Add(1640391059U, "PartnersCannotHaveWildcards");
			DirectoryStrings.stringIDs.Add(352018919U, "IPv4Only");
			DirectoryStrings.stringIDs.Add(4150730333U, "InboundConnectorInvalidIPCertificateCombinations");
			DirectoryStrings.stringIDs.Add(2814018773U, "Exchange2003or2000");
			DirectoryStrings.stringIDs.Add(2730108605U, "ErrorOneProcessInitializedAsBothSingleAndMultiple");
			DirectoryStrings.stringIDs.Add(1391517930U, "RoomListGroupTypeDetails");
			DirectoryStrings.stringIDs.Add(3586618070U, "MailEnabledForestContactRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(2286188335U, "ErrorAuthMetadataNoIssuingEndpoint");
			DirectoryStrings.stringIDs.Add(2227190334U, "NonUniversalGroupRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(2587673404U, "ErrorMustBeSysConfigObject");
			DirectoryStrings.stringIDs.Add(2742534530U, "OutboundConnectorTlsSettingsInvalidTlsDomainWithoutDomainValidation");
			DirectoryStrings.stringIDs.Add(135248896U, "LdapFilterErrorInvalidBitwiseOperand");
			DirectoryStrings.stringIDs.Add(2106980546U, "ExceptionSetPreferredDCsOnlyForManagement");
			DirectoryStrings.stringIDs.Add(3635271833U, "LegacyArchiveJournals");
			DirectoryStrings.stringIDs.Add(2980205681U, "CustomInternalSubjectRequired");
			DirectoryStrings.stringIDs.Add(238304980U, "ErrorCannotAddArchiveMailbox");
			DirectoryStrings.stringIDs.Add(1479682494U, "NoNewCalls");
			DirectoryStrings.stringIDs.Add(1615193486U, "ErrorMessageClassEmpty");
			DirectoryStrings.stringIDs.Add(189075208U, "GloballyDistributedOABCacheReadTimeoutError");
			DirectoryStrings.stringIDs.Add(523533880U, "Manual");
			DirectoryStrings.stringIDs.Add(3179357576U, "ErrorAcceptedDomainCannotContainWildcardAndNegoConfig");
			DirectoryStrings.stringIDs.Add(968858937U, "UniversalSecurityGroupRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(3647297993U, "ArbitrationMailboxTypeDetails");
			DirectoryStrings.stringIDs.Add(575705180U, "CalendarAgeFilterAll");
			DirectoryStrings.stringIDs.Add(862838650U, "GroupNamingPolicyCompany");
			DirectoryStrings.stringIDs.Add(700328008U, "IndustryMining");
			DirectoryStrings.stringIDs.Add(2775202161U, "ServerRoleOSP");
			DirectoryStrings.stringIDs.Add(1190445522U, "InvalidDirectoryConfiguration");
			DirectoryStrings.stringIDs.Add(3381355689U, "ErrorDDLReferral");
			DirectoryStrings.stringIDs.Add(3541370315U, "LdapFilterErrorNoAttributeValue");
			DirectoryStrings.stringIDs.Add(606960029U, "ExternalEnrollment");
			DirectoryStrings.stringIDs.Add(161599950U, "ErrorTimeoutReadingSystemAddressListCache");
			DirectoryStrings.stringIDs.Add(2375038189U, "CanRunDefaultUpdateState_NotSuspended");
			DirectoryStrings.stringIDs.Add(1615928121U, "PreferredInternetCodePageSio2022Jp");
			DirectoryStrings.stringIDs.Add(1762945050U, "HtmlAndTextAlternative");
			DirectoryStrings.stringIDs.Add(1164140307U, "GlobalAddressList");
			DirectoryStrings.stringIDs.Add(1912797067U, "MailTipsAccessLevelNone");
			DirectoryStrings.stringIDs.Add(3599499864U, "EsnLangGalician");
			DirectoryStrings.stringIDs.Add(3786203794U, "ServerRoleFrontendTransport");
			DirectoryStrings.stringIDs.Add(4087400250U, "Exchange2009");
			DirectoryStrings.stringIDs.Add(1259815309U, "TransientMservErrorDescription");
			DirectoryStrings.stringIDs.Add(48855524U, "ReceiveAuthMechanismExchangeServer");
			DirectoryStrings.stringIDs.Add(3380639415U, "Watsons");
			DirectoryStrings.stringIDs.Add(2074397341U, "OrganizationCapabilityPstProvider");
			DirectoryStrings.stringIDs.Add(2277391560U, "ErrorCapabilityNone");
			DirectoryStrings.stringIDs.Add(224904099U, "ExceptionAllDomainControllersUnavailable");
			DirectoryStrings.stringIDs.Add(3495902273U, "ServersContainerNotFoundException");
			DirectoryStrings.stringIDs.Add(296014363U, "MailboxMoveStatusCompletionInProgress");
			DirectoryStrings.stringIDs.Add(2125756541U, "ServerRoleMailbox");
			DirectoryStrings.stringIDs.Add(3930280380U, "ErrorResourceTypeMissing");
			DirectoryStrings.stringIDs.Add(1716044995U, "Contacts");
			DirectoryStrings.stringIDs.Add(3268644368U, "SendAuthMechanismTls");
			DirectoryStrings.stringIDs.Add(3574113162U, "AggregatedSessionCannotMakeMbxChanges");
			DirectoryStrings.stringIDs.Add(1802707653U, "PAAEnabled");
			DirectoryStrings.stringIDs.Add(2191519417U, "NonPartner");
			DirectoryStrings.stringIDs.Add(4008691139U, "BasicAfterTLSWithoutBasic");
			DirectoryStrings.stringIDs.Add(2672001483U, "ErrorSharedConfigurationBothRoles");
			DirectoryStrings.stringIDs.Add(4077321274U, "EsnLangDutch");
			DirectoryStrings.stringIDs.Add(1406386932U, "DsnLanguageNotSupportedForCustomization");
			DirectoryStrings.stringIDs.Add(2440749065U, "IndustryNotSpecified");
			DirectoryStrings.stringIDs.Add(1045941944U, "ErrorDDLFilterError");
			DirectoryStrings.stringIDs.Add(3599602982U, "AddressList");
			DirectoryStrings.stringIDs.Add(795884132U, "MustDisplayComment");
			DirectoryStrings.stringIDs.Add(3464146580U, "ServerRoleFfoWebServices");
			DirectoryStrings.stringIDs.Add(1052758952U, "ServerRoleClientAccess");
			DirectoryStrings.stringIDs.Add(3595585153U, "SKUCapabilityBPOSSEnterprise");
			DirectoryStrings.stringIDs.Add(3839109662U, "InvalidReceiveAuthModeExternalOnly");
			DirectoryStrings.stringIDs.Add(2653001089U, "ErrorSettingOverrideNull");
			DirectoryStrings.stringIDs.Add(4230107429U, "LdapFilterErrorQueryTooLong");
			DirectoryStrings.stringIDs.Add(1655452658U, "ErrorMoveToDestinationFolderNotDefined");
			DirectoryStrings.stringIDs.Add(4190154187U, "MailboxMoveStatusInProgress");
			DirectoryStrings.stringIDs.Add(370461711U, "SecurityPrincipalTypeGroup");
			DirectoryStrings.stringIDs.Add(1470218539U, "X400Authoritative");
			DirectoryStrings.stringIDs.Add(3038327607U, "MailFlowPartnerInternalMailContentTypeMimeHtml");
			DirectoryStrings.stringIDs.Add(3689869554U, "MailEnabledUserRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(2902411778U, "ExtensionNull");
			DirectoryStrings.stringIDs.Add(1573777228U, "Unsecured");
			DirectoryStrings.stringIDs.Add(4079674260U, "ConnectorIdIsNotAnInteger");
			DirectoryStrings.stringIDs.Add(2617699176U, "ErrorMissingPrimaryUM");
			DirectoryStrings.stringIDs.Add(3783332642U, "CannotDetermineDataSessionType");
			DirectoryStrings.stringIDs.Add(3601314308U, "UserAgentsChanges");
			DirectoryStrings.stringIDs.Add(1601836855U, "Notes");
			DirectoryStrings.stringIDs.Add(2395522212U, "EsnLangTelugu");
			DirectoryStrings.stringIDs.Add(65728472U, "GroupNamingPolicyExtensionCustomAttribute1");
			DirectoryStrings.stringIDs.Add(1221325234U, "MailFlowPartnerInternalMailContentTypeNone");
			DirectoryStrings.stringIDs.Add(268472571U, "DefaultRapName");
			DirectoryStrings.stringIDs.Add(3648445463U, "DeleteUseDefaultAlert");
			DirectoryStrings.stringIDs.Add(4156100093U, "ErrorOrganizationResourceAddressListsCount");
			DirectoryStrings.stringIDs.Add(3706505019U, "EsnLangChineseSimplified");
			DirectoryStrings.stringIDs.Add(1919306754U, "ConferenceRoomMailboxRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(557877518U, "BlockedOutlookClientVersionPatternDescription");
			DirectoryStrings.stringIDs.Add(1448153692U, "UserHasNoSmtpProxyAddressWithFederatedDomain");
			DirectoryStrings.stringIDs.Add(2296213214U, "OrganizationCapabilityMailRouting");
			DirectoryStrings.stringIDs.Add(1325366747U, "SKUCapabilityBPOSSStandard");
			DirectoryStrings.stringIDs.Add(1850977098U, "SystemMailboxRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(3702456775U, "ExceptionADTopologyNoLocalDomain");
			DirectoryStrings.stringIDs.Add(3504940969U, "EsnLangDanish");
			DirectoryStrings.stringIDs.Add(779349581U, "IndustryRetail");
			DirectoryStrings.stringIDs.Add(2864464497U, "ErrorDDLNoSuchObject");
			DirectoryStrings.stringIDs.Add(2713172052U, "IndustryComputerRelatedProductsServices");
			DirectoryStrings.stringIDs.Add(3288506612U, "InternalRelay");
			DirectoryStrings.stringIDs.Add(602695546U, "ErrorEmptyArchiveName");
			DirectoryStrings.stringIDs.Add(1231743030U, "EmailAddressPolicyPriorityLowest");
			DirectoryStrings.stringIDs.Add(326106109U, "ExternalMdm");
			DirectoryStrings.stringIDs.Add(3799883362U, "TransportSettingsNotFoundException");
			DirectoryStrings.stringIDs.Add(3552372249U, "DomainSecureEnabledWithoutTls");
			DirectoryStrings.stringIDs.Add(1904959661U, "BccSuspiciousOutboundAdditionalRecipientsRequired");
			DirectoryStrings.stringIDs.Add(4291428005U, "NoRoleEntriesFound");
			DirectoryStrings.stringIDs.Add(1515341016U, "IndustryWholesale");
			DirectoryStrings.stringIDs.Add(3980237751U, "ServerRoleCentralAdminFrontEnd");
			DirectoryStrings.stringIDs.Add(4068243401U, "ErrorInvalidPushNotificationPlatform");
			DirectoryStrings.stringIDs.Add(2277405024U, "MailTipsAccessLevelAll");
			DirectoryStrings.stringIDs.Add(1625030180U, "PublicFolderRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(943620946U, "ValueNotAvailableForUnchangedProperty");
			DirectoryStrings.stringIDs.Add(3641768400U, "DumpsterFolder");
			DirectoryStrings.stringIDs.Add(1756055255U, "CannotParseMimeTypes");
			DirectoryStrings.stringIDs.Add(2518725308U, "ExclusiveRecipientScopes");
			DirectoryStrings.stringIDs.Add(1771494609U, "QuarantineMailboxIsInvalid");
			DirectoryStrings.stringIDs.Add(1094750789U, "MailboxPlanTypeDetails");
			DirectoryStrings.stringIDs.Add(986970413U, "ServerRoleCafeArray");
			DirectoryStrings.stringIDs.Add(2874697860U, "SendCredentialIsNull");
			DirectoryStrings.stringIDs.Add(3323264318U, "True");
			DirectoryStrings.stringIDs.Add(300685400U, "StarAcceptedDomainCannotBeAuthoritative");
			DirectoryStrings.stringIDs.Add(2302903917U, "AllRooms");
			DirectoryStrings.stringIDs.Add(2650345129U, "EsnLangRussian");
			DirectoryStrings.stringIDs.Add(3989445055U, "GroupNamingPolicyCustomAttribute10");
			DirectoryStrings.stringIDs.Add(79975170U, "SitesContainerNotFound");
			DirectoryStrings.stringIDs.Add(1581344072U, "ExceptionServerTimeoutNegative");
			DirectoryStrings.stringIDs.Add(665936024U, "ArchiveStateLocal");
			DirectoryStrings.stringIDs.Add(991629433U, "NotesMC");
			DirectoryStrings.stringIDs.Add(3403459873U, "InvalidDomain");
			DirectoryStrings.stringIDs.Add(2092969439U, "EmailAgeFilterOneMonth");
			DirectoryStrings.stringIDs.Add(4021009371U, "FullDomain");
			DirectoryStrings.stringIDs.Add(1430923151U, "DeviceModel");
			DirectoryStrings.stringIDs.Add(777275992U, "GroupRecipientType");
			DirectoryStrings.stringIDs.Add(444885611U, "RemoteSharedMailboxTypeDetails");
			DirectoryStrings.stringIDs.Add(261312351U, "LdapSearch");
			DirectoryStrings.stringIDs.Add(3378383244U, "EsnLangArabic");
			DirectoryStrings.stringIDs.Add(3490390166U, "SKUCapabilityBPOSSDeskless");
			DirectoryStrings.stringIDs.Add(3131560893U, "ModeratedRecipients");
			DirectoryStrings.stringIDs.Add(1935657977U, "ExceptionRusOperationFailed");
			DirectoryStrings.stringIDs.Add(4110564385U, "ExceptionDomainInfoRpcTooBusy");
			DirectoryStrings.stringIDs.Add(3057991035U, "ErrorArchiveDomainInvalidInDatacenter");
			DirectoryStrings.stringIDs.Add(2922780138U, "PublicFolderRecipientType");
			DirectoryStrings.stringIDs.Add(2843126128U, "ErrorMessageClassHasUnsupportedWildcard");
			DirectoryStrings.stringIDs.Add(2976002772U, "ErrorPipelineTracingRequirementsMissing");
			DirectoryStrings.stringIDs.Add(2423361114U, "GroupNamingPolicyCustomAttribute11");
			DirectoryStrings.stringIDs.Add(65716788U, "ErrorMailTipMustNotBeEmpty");
			DirectoryStrings.stringIDs.Add(1479699766U, "ComputerRecipientType");
			DirectoryStrings.stringIDs.Add(2493930652U, "ErrorArbitrationMailboxCannotBeModerated");
			DirectoryStrings.stringIDs.Add(3119627512U, "EsnLangKannada");
			DirectoryStrings.stringIDs.Add(2435266816U, "Title");
			DirectoryStrings.stringIDs.Add(389262922U, "MessageWaitingIndicatorEnabled");
			DirectoryStrings.stringIDs.Add(3178475968U, "PublicFolders");
			DirectoryStrings.stringIDs.Add(92440185U, "Millisecond");
			DirectoryStrings.stringIDs.Add(2661434578U, "StarAcceptedDomainCannotBeDefault");
			DirectoryStrings.stringIDs.Add(2033242172U, "ReceiveExtendedProtectionPolicyAllow");
			DirectoryStrings.stringIDs.Add(2995964430U, "ResourceMailbox");
			DirectoryStrings.stringIDs.Add(3176413521U, "ErrorThrottlingPolicyStateIsCorrupt");
			DirectoryStrings.stringIDs.Add(3005725472U, "MailEnabledNonUniversalGroupRecipientType");
			DirectoryStrings.stringIDs.Add(573230607U, "ExternalAuthoritativeWithoutExchangeServerPermission");
			DirectoryStrings.stringIDs.Add(2913015079U, "Authoritative");
			DirectoryStrings.stringIDs.Add(2813895538U, "ErrorPrimarySmtpAddressAndWindowsEmailAddressNotMatch");
			DirectoryStrings.stringIDs.Add(1421974844U, "PostMC");
			DirectoryStrings.stringIDs.Add(3819234583U, "UnknownConfigObject");
			DirectoryStrings.stringIDs.Add(4274370117U, "MalwareScanErrorActionAllow");
			DirectoryStrings.stringIDs.Add(1924734198U, "GroupNamingPolicyCustomAttribute6");
			DirectoryStrings.stringIDs.Add(2610662106U, "InvalidTransportSyncLogSizeConfiguration");
			DirectoryStrings.stringIDs.Add(3725493575U, "WellKnownRecipientTypeMailGroups");
			DirectoryStrings.stringIDs.Add(120558192U, "ADDriverStoreAccessTransientError");
			DirectoryStrings.stringIDs.Add(635049629U, "AACantChangeName");
			DirectoryStrings.stringIDs.Add(3247735886U, "ContactItemsMC");
			DirectoryStrings.stringIDs.Add(4170667976U, "EsnLangKorean");
			DirectoryStrings.stringIDs.Add(131007819U, "RssSubscriptionMC");
			DirectoryStrings.stringIDs.Add(1652093200U, "LdapFilterErrorSpaceMiddleType");
			DirectoryStrings.stringIDs.Add(1924734193U, "GroupNamingPolicyCustomAttribute3");
			DirectoryStrings.stringIDs.Add(3167491578U, "ExceptionNoFsmoRoleOwnerAttribute");
			DirectoryStrings.stringIDs.Add(600983985U, "NonIpmRoot");
			DirectoryStrings.stringIDs.Add(2829159765U, "ErrorTimeoutWritingSystemAddressListMemberCount");
			DirectoryStrings.stringIDs.Add(2900785706U, "ExceptionExternalError");
			DirectoryStrings.stringIDs.Add(1292798904U, "Calendar");
			DirectoryStrings.stringIDs.Add(2665399355U, "Wma");
			DirectoryStrings.stringIDs.Add(869401742U, "ErrorInvalidDNDepth");
			DirectoryStrings.stringIDs.Add(1435812789U, "CapabilityMasteredOnPremise");
			DirectoryStrings.stringIDs.Add(371000500U, "EdgeSyncEhfConnectorFailedToDecryptPassword");
			DirectoryStrings.stringIDs.Add(1758334214U, "ErrorArchiveDomainSetForNonArchive");
			DirectoryStrings.stringIDs.Add(3086681225U, "ExceptionObjectHasBeenDeleted");
			DirectoryStrings.stringIDs.Add(2211212701U, "EsnLangBengaliIndia");
			DirectoryStrings.stringIDs.Add(613950136U, "PublicFolderServer");
			DirectoryStrings.stringIDs.Add(4127869723U, "ErrorCannotSetPrimarySmtpAddress");
			DirectoryStrings.stringIDs.Add(2852597951U, "SpamFilteringActionQuarantine");
			DirectoryStrings.stringIDs.Add(1313260064U, "MailboxMoveStatusFailed");
			DirectoryStrings.stringIDs.Add(1169031248U, "SecurityPrincipalTypeUniversalSecurityGroup");
			DirectoryStrings.stringIDs.Add(1254627662U, "DynamicDLRecipientType");
			DirectoryStrings.stringIDs.Add(2863183086U, "ErrorNonTinyTenantShouldNotHaveSharedConfig");
			DirectoryStrings.stringIDs.Add(3057941193U, "CanRunRestoreState_Allowed");
			DirectoryStrings.stringIDs.Add(1199714779U, "DomainSecureWithIgnoreStartTLSEnabled");
			DirectoryStrings.stringIDs.Add(825243359U, "GroupNamingPolicyExtensionCustomAttribute4");
			DirectoryStrings.stringIDs.Add(3705158290U, "UseMsg");
			DirectoryStrings.stringIDs.Add(149761450U, "InvalidTenantFullSyncCookieException");
			DirectoryStrings.stringIDs.Add(3241000569U, "AutoDatabaseMountDialGoodAvailability");
			DirectoryStrings.stringIDs.Add(4056279737U, "ForestTrust");
			DirectoryStrings.stringIDs.Add(556546691U, "ErrorInvalidMailboxRelationType");
			DirectoryStrings.stringIDs.Add(2757465550U, "ErrorDDLInvalidDNSyntax");
			DirectoryStrings.stringIDs.Add(3615619130U, "ByteEncoderTypeUseQP");
			DirectoryStrings.stringIDs.Add(2204790954U, "NoLocatorInformationInMServException");
			DirectoryStrings.stringIDs.Add(2004555878U, "SecurityPrincipalTypeGlobalSecurityGroup");
			DirectoryStrings.stringIDs.Add(2999553646U, "CannotGetUsefulSiteInfo");
			DirectoryStrings.stringIDs.Add(1031444357U, "ErrorPipelineTracingPathNotExist");
			DirectoryStrings.stringIDs.Add(1832080745U, "MailboxServer");
			DirectoryStrings.stringIDs.Add(4019774802U, "Blocked");
			DirectoryStrings.stringIDs.Add(2649572709U, "InvalidMainStreamCookieException");
			DirectoryStrings.stringIDs.Add(1341999288U, "MoveNotAllowed");
			DirectoryStrings.stringIDs.Add(1594549261U, "RemoteRoomMailboxTypeDetails");
			DirectoryStrings.stringIDs.Add(1776235905U, "SecurityPrincipalTypeUser");
			DirectoryStrings.stringIDs.Add(2078807267U, "TextEnrichedOnly");
			DirectoryStrings.stringIDs.Add(3036506883U, "BluetoothAllow");
			DirectoryStrings.stringIDs.Add(154085973U, "GroupNamingPolicyDepartment");
			DirectoryStrings.stringIDs.Add(3388588817U, "UseDefaultSettings");
			DirectoryStrings.stringIDs.Add(3102724093U, "ByteEncoderTypeUseQPHtmlDetectTextPlain");
			DirectoryStrings.stringIDs.Add(2924600836U, "Exchange2007");
			DirectoryStrings.stringIDs.Add(788602100U, "DisabledPartner");
			DirectoryStrings.stringIDs.Add(1344968854U, "Consumer");
			DirectoryStrings.stringIDs.Add(1013979892U, "PrimaryMailboxRelationType");
			DirectoryStrings.stringIDs.Add(1484405454U, "Disabled");
			DirectoryStrings.stringIDs.Add(1091797613U, "SKUCapabilityBPOSSBasicCustomDomain");
			DirectoryStrings.stringIDs.Add(2308256473U, "ControlTextNull");
			DirectoryStrings.stringIDs.Add(629464291U, "Outbox");
			DirectoryStrings.stringIDs.Add(3086386447U, "ArchiveStateNone");
			DirectoryStrings.stringIDs.Add(1727459539U, "MailFlowPartnerInternalMailContentTypeMimeText");
			DirectoryStrings.stringIDs.Add(2238564813U, "CustomInternalBodyRequired");
			DirectoryStrings.stringIDs.Add(2086215909U, "TlsDomainWithIncorrectTlsAuthLevel");
			DirectoryStrings.stringIDs.Add(213405127U, "SystemTag");
			DirectoryStrings.stringIDs.Add(65301504U, "AllMailboxContentMC");
			DirectoryStrings.stringIDs.Add(4145265495U, "RemoteUserMailboxTypeDetails");
			DirectoryStrings.stringIDs.Add(3441693128U, "BluetoothDisable");
			DirectoryStrings.stringIDs.Add(2698858797U, "ServerRoleLanguagePacks");
			DirectoryStrings.stringIDs.Add(1415894913U, "PrincipalName");
			DirectoryStrings.stringIDs.Add(3541826428U, "IdIsNotSet");
			DirectoryStrings.stringIDs.Add(1254345332U, "ConstraintViolationSupervisionListEntryStringPartIsInvalid");
			DirectoryStrings.stringIDs.Add(2638599330U, "WellKnownRecipientTypeMailContacts");
			DirectoryStrings.stringIDs.Add(172810921U, "ServerRoleHubTransport");
			DirectoryStrings.stringIDs.Add(4251572755U, "IndustryHealthcare");
			DirectoryStrings.stringIDs.Add(3956811795U, "CapabilityPartnerManaged");
			DirectoryStrings.stringIDs.Add(4087147933U, "ErrorArchiveDatabaseArchiveDomainMissing");
			DirectoryStrings.stringIDs.Add(2967905667U, "MailEnabledUniversalSecurityGroupRecipientType");
			DirectoryStrings.stringIDs.Add(1486937545U, "ErrorRemovalNotSupported");
			DirectoryStrings.stringIDs.Add(1129549138U, "ExchangeFaxMC");
			DirectoryStrings.stringIDs.Add(2611743021U, "ByteEncoderTypeUse7Bit");
			DirectoryStrings.stringIDs.Add(2176173662U, "InvalidBindingAddressSetting");
			DirectoryStrings.stringIDs.Add(4172461161U, "ASAccessMethodNeedsAuthenticationAccount");
			DirectoryStrings.stringIDs.Add(491964191U, "CanRunDefaultUpdateState_Allowed");
			DirectoryStrings.stringIDs.Add(2884121764U, "EsnLangMalay");
			DirectoryStrings.stringIDs.Add(1167380226U, "FailedToParseAlternateServiceAccountCredential");
			DirectoryStrings.stringIDs.Add(3799817423U, "ExternalManagedMailContactTypeDetails");
			DirectoryStrings.stringIDs.Add(1403090333U, "IPv6Only");
			DirectoryStrings.stringIDs.Add(2827463711U, "MountDialOverrideLossless");
			DirectoryStrings.stringIDs.Add(3607366039U, "Percent");
			DirectoryStrings.stringIDs.Add(1922689150U, "ServerRoleProvisionedServer");
			DirectoryStrings.stringIDs.Add(2350890097U, "CalendarAgeFilterOneMonth");
			DirectoryStrings.stringIDs.Add(4169982073U, "TextOnly");
			DirectoryStrings.stringIDs.Add(1291987412U, "InvalidMsgTrackingLogSizeConfiguration");
			DirectoryStrings.stringIDs.Add(1881847987U, "ErrorArchiveDatabaseSetForNonArchive");
			DirectoryStrings.stringIDs.Add(1940813882U, "InvalidGenerationTime");
			DirectoryStrings.stringIDs.Add(820626981U, "CalendarItemMC");
			DirectoryStrings.stringIDs.Add(3745862197U, "Block");
			DirectoryStrings.stringIDs.Add(3519747066U, "ErrorNullExternalEmailAddress");
			DirectoryStrings.stringIDs.Add(1681980649U, "ExceptionRusNotRunning");
			DirectoryStrings.stringIDs.Add(862550630U, "PropertyCannotBeSetToTest");
			DirectoryStrings.stringIDs.Add(1641698628U, "LdapFilterErrorInvalidEscaping");
			DirectoryStrings.stringIDs.Add(867516332U, "ForceSave");
			DirectoryStrings.stringIDs.Add(1798526791U, "LinkedRoomMailboxRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(3794956711U, "DeleteUseCustomAlert");
			DirectoryStrings.stringIDs.Add(370123223U, "CannotDeserializePartitionHint");
			DirectoryStrings.stringIDs.Add(1284675050U, "InboundConnectorInvalidRestrictDomainsToIPAddresses");
			DirectoryStrings.stringIDs.Add(1663846227U, "GroupNamingPolicyCustomAttribute14");
			DirectoryStrings.stringIDs.Add(2839121159U, "ContactRecipientType");
			DirectoryStrings.stringIDs.Add(2600481667U, "DomainSecureWithoutDNSRoutingEnabled");
			DirectoryStrings.stringIDs.Add(3062547699U, "RunspaceServerSettingsChanged");
			DirectoryStrings.stringIDs.Add(3313482992U, "EsnLangGreek");
			DirectoryStrings.stringIDs.Add(309994549U, "TooManyEntriesError");
			DirectoryStrings.stringIDs.Add(1899391140U, "OrganizationRelationshipMissingTargetApplicationUri");
			DirectoryStrings.stringIDs.Add(3489169852U, "ComputerRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(96146822U, "Exchweb");
			DirectoryStrings.stringIDs.Add(2202908911U, "OutboundConnectorIncorrectRouteAllMessagesViaOnPremises");
			DirectoryStrings.stringIDs.Add(1499257862U, "CalendarSharingFreeBusyAvailabilityOnly");
			DirectoryStrings.stringIDs.Add(3707194059U, "ServerRoleExtendedRole5");
			DirectoryStrings.stringIDs.Add(390976058U, "AutoAttendantLink");
			DirectoryStrings.stringIDs.Add(4020865807U, "CustomRoleDescription_MyDisplayName");
			DirectoryStrings.stringIDs.Add(3949283739U, "AllUsers");
			DirectoryStrings.stringIDs.Add(4231482709U, "All");
			DirectoryStrings.stringIDs.Add(4256840071U, "OrganizationCapabilityMigration");
			DirectoryStrings.stringIDs.Add(142272059U, "DialPlan");
			DirectoryStrings.stringIDs.Add(2868284030U, "EsnLangUkrainian");
			DirectoryStrings.stringIDs.Add(2052645173U, "MessageRateSourceFlagsNone");
			DirectoryStrings.stringIDs.Add(2196808673U, "IndustryLegal");
			DirectoryStrings.stringIDs.Add(3379397641U, "CapabilityUMFeatureRestricted");
			DirectoryStrings.stringIDs.Add(1494101274U, "GroupTypeFlagsBuiltinLocal");
			DirectoryStrings.stringIDs.Add(2412300803U, "ReceiveAuthMechanismBasicAuthPlusTls");
			DirectoryStrings.stringIDs.Add(3811183882U, "Allowed");
			DirectoryStrings.stringIDs.Add(3579894660U, "ByteEncoderTypeUseQPHtml7BitTextPlain");
			DirectoryStrings.stringIDs.Add(4217035038U, "High");
			DirectoryStrings.stringIDs.Add(821502958U, "MicrosoftExchangeRecipientType");
			DirectoryStrings.stringIDs.Add(1429014682U, "BackSyncDataSourceUnavailableMessage");
			DirectoryStrings.stringIDs.Add(110833865U, "ArchiveStateOnPremise");
			DirectoryStrings.stringIDs.Add(936096413U, "OrganizationCapabilitySuiteServiceStorage");
			DirectoryStrings.stringIDs.Add(1592544157U, "MalwareScanErrorActionBlock");
			DirectoryStrings.stringIDs.Add(1924944146U, "SKUCapabilityBPOSSArchiveAddOn");
			DirectoryStrings.stringIDs.Add(4227895642U, "ExceptionRusAccessDenied");
			DirectoryStrings.stringIDs.Add(2094315795U, "ServerRoleNone");
			DirectoryStrings.stringIDs.Add(3352185505U, "AlternateServiceAccountConfigurationDisplayFormatMoreDataAvailable");
			DirectoryStrings.stringIDs.Add(2728392679U, "GloballyDistributedOABCacheWriteTimeoutError");
			DirectoryStrings.stringIDs.Add(3727360630U, "UserName");
			DirectoryStrings.stringIDs.Add(1173768533U, "Reserved1");
			DirectoryStrings.stringIDs.Add(3144162877U, "NoAddresses");
			DirectoryStrings.stringIDs.Add(2817538580U, "RegionBlockListNotSet");
			DirectoryStrings.stringIDs.Add(1398191848U, "CapabilityRichCoexistence");
			DirectoryStrings.stringIDs.Add(2032072470U, "ErrorUserAccountNameIncludeAt");
			DirectoryStrings.stringIDs.Add(634395589U, "Enabled");
			DirectoryStrings.stringIDs.Add(3840534502U, "AttachmentsWereRemovedMessage");
			DirectoryStrings.stringIDs.Add(4176423907U, "ErrorCannotFindUnusedLegacyDN");
			DirectoryStrings.stringIDs.Add(2318114319U, "EmailAgeFilterOneWeek");
			DirectoryStrings.stringIDs.Add(2193124873U, "GroupNameInNamingPolicy");
			DirectoryStrings.stringIDs.Add(2504573058U, "OrganizationCapabilityClientExtensions");
			DirectoryStrings.stringIDs.Add(3081766090U, "CalendarAgeFilterTwoWeeks");
			DirectoryStrings.stringIDs.Add(583050472U, "ErrorElcCommentNotAllowed");
			DirectoryStrings.stringIDs.Add(3275453767U, "ErrorOwnersUpdated");
			DirectoryStrings.stringIDs.Add(3776377092U, "EsnLangIndonesian");
			DirectoryStrings.stringIDs.Add(2631270417U, "Extension");
			DirectoryStrings.stringIDs.Add(1468404604U, "CanEnableLocalCopyState_Invalid");
			DirectoryStrings.stringIDs.Add(2146247679U, "MailEnabledUniversalDistributionGroupRecipientType");
			DirectoryStrings.stringIDs.Add(893817173U, "ReceiveCredentialIsNull");
			DirectoryStrings.stringIDs.Add(1885276797U, "EsnLangLithuanian");
			DirectoryStrings.stringIDs.Add(570563164U, "ServerRoleAll");
			DirectoryStrings.stringIDs.Add(756854696U, "ServerRoleEdge");
			DirectoryStrings.stringIDs.Add(982491582U, "ExceptionObjectStillExists");
			DirectoryStrings.stringIDs.Add(387112589U, "AllRecipients");
			DirectoryStrings.stringIDs.Add(3685369418U, "LdapFilterErrorNoAttributeType");
			DirectoryStrings.stringIDs.Add(3802186670U, "ServerRoleManagementFrontEnd");
			DirectoryStrings.stringIDs.Add(2609910045U, "False");
			DirectoryStrings.stringIDs.Add(519619317U, "CalendarSharingFreeBusyLimitedDetails");
			DirectoryStrings.stringIDs.Add(2662725163U, "SystemAttendantMailboxRecipientType");
			DirectoryStrings.stringIDs.Add(696678862U, "ServerRoleManagementBackEnd");
			DirectoryStrings.stringIDs.Add(4088287609U, "GroupNamingPolicyStateOrProvince");
			DirectoryStrings.stringIDs.Add(1955666018U, "IndustryFinance");
			DirectoryStrings.stringIDs.Add(3313162693U, "ErrorAgeLimitExpiration");
			DirectoryStrings.stringIDs.Add(2067616247U, "InboundConnectorMissingTlsCertificateOrSenderIP");
			DirectoryStrings.stringIDs.Add(1247338605U, "ErrorMailTipTranslationFormatIncorrect");
			DirectoryStrings.stringIDs.Add(3892578519U, "MountDialOverrideGoodAvailability");
			DirectoryStrings.stringIDs.Add(2615196936U, "ConfigReadScope");
			DirectoryStrings.stringIDs.Add(2338964630U, "UserRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(3291024868U, "MeetingRequestMC");
			DirectoryStrings.stringIDs.Add(696030922U, "Tag");
			DirectoryStrings.stringIDs.Add(3607016823U, "MailFlowPartnerInternalMailContentTypeTNEF");
			DirectoryStrings.stringIDs.Add(1793427927U, "SerialNumberMissing");
			DirectoryStrings.stringIDs.Add(2193656120U, "AttributeNameNull");
			DirectoryStrings.stringIDs.Add(1260468432U, "ErrorIsDehydratedSetOnNonTinyTenant");
			DirectoryStrings.stringIDs.Add(4069918469U, "TUIPromptEditingEnabled");
			DirectoryStrings.stringIDs.Add(885421749U, "StarAcceptedDomainCannotBeInitialDomain");
			DirectoryStrings.stringIDs.Add(1860494422U, "LdapFilterErrorNotSupportSingleComp");
			DirectoryStrings.stringIDs.Add(1191236736U, "UseTnef");
			DirectoryStrings.stringIDs.Add(654334258U, "AttachmentFilterEntryInvalid");
			DirectoryStrings.stringIDs.Add(599002007U, "Exchange2013");
			DirectoryStrings.stringIDs.Add(3145869218U, "SendAuthMechanismBasicAuthPlusTls");
			DirectoryStrings.stringIDs.Add(3341243277U, "MoveToDeletedItems");
			DirectoryStrings.stringIDs.Add(2403277311U, "TCP");
			DirectoryStrings.stringIDs.Add(2664643149U, "DocumentMC");
			DirectoryStrings.stringIDs.Add(1531250846U, "ErrorCannotSetWindowsEmailAddress");
			DirectoryStrings.stringIDs.Add(3115737588U, "Msn");
			DirectoryStrings.stringIDs.Add(619725344U, "MessageRateSourceFlagsIPAddress");
			DirectoryStrings.stringIDs.Add(4163650158U, "ErrorTextMessageIncludingAppleAttachment");
			DirectoryStrings.stringIDs.Add(3857647582U, "ForwardCallsToDefaultMailbox");
			DirectoryStrings.stringIDs.Add(3221974997U, "RoleGroupTypeDetails");
			DirectoryStrings.stringIDs.Add(3815678973U, "MailEnabledContactRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(647747116U, "EsnLangEnglish");
			DirectoryStrings.stringIDs.Add(2250496622U, "EsnLangMarathi");
			DirectoryStrings.stringIDs.Add(1092312607U, "SpecifyAnnouncementFileName");
			DirectoryStrings.stringIDs.Add(857277173U, "GroupNamingPolicyCustomAttribute12");
			DirectoryStrings.stringIDs.Add(3759553744U, "SystemAddressListDoesNotExist");
			DirectoryStrings.stringIDs.Add(3499307032U, "DefaultOabName");
			DirectoryStrings.stringIDs.Add(1855185352U, "EsnLangSpanish");
			DirectoryStrings.stringIDs.Add(3227102809U, "FederatedOrganizationIdNoNamespaceAccount");
			DirectoryStrings.stringIDs.Add(1406382714U, "RemoteEquipmentMailboxTypeDetails");
			DirectoryStrings.stringIDs.Add(2654331961U, "SpamFilteringOptionOn");
			DirectoryStrings.stringIDs.Add(1495966060U, "ErrorNoSharedConfigurationInfo");
			DirectoryStrings.stringIDs.Add(3938481035U, "EquipmentMailboxRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(375049999U, "ErrorCannotSetMoveToDestinationFolder");
			DirectoryStrings.stringIDs.Add(290262264U, "CapabilityTOUSigned");
			DirectoryStrings.stringIDs.Add(3707194054U, "ServerRoleExtendedRole2");
			DirectoryStrings.stringIDs.Add(3707194053U, "ServerRoleExtendedRole3");
			DirectoryStrings.stringIDs.Add(2283186478U, "PersonalFolder");
			DirectoryStrings.stringIDs.Add(584737882U, "CapabilityNone");
			DirectoryStrings.stringIDs.Add(31546440U, "ErrorEmptyResourceTypeofResourceMailbox");
			DirectoryStrings.stringIDs.Add(2469247251U, "InternalDNSServersNotSet");
			DirectoryStrings.stringIDs.Add(4205089983U, "ExceptionImpersonation");
			DirectoryStrings.stringIDs.Add(3072026616U, "ReceiveAuthMechanismNone");
			DirectoryStrings.stringIDs.Add(1924734199U, "GroupNamingPolicyCustomAttribute9");
			DirectoryStrings.stringIDs.Add(2999125469U, "MailEnabledDynamicDistributionGroupRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(685401583U, "SpamFilteringActionAddXHeader");
			DirectoryStrings.stringIDs.Add(141120823U, "RecentCommands");
			DirectoryStrings.stringIDs.Add(2690725740U, "SecurityPrincipalTypeNone");
			DirectoryStrings.stringIDs.Add(1589279983U, "MailboxMoveStatusNone");
			DirectoryStrings.stringIDs.Add(3976915092U, "LocalForest");
			DirectoryStrings.stringIDs.Add(221683052U, "LegacyMailboxRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(1924734194U, "GroupNamingPolicyCustomAttribute2");
			DirectoryStrings.stringIDs.Add(661425765U, "DatabaseMasterTypeUnknown");
			DirectoryStrings.stringIDs.Add(2630084427U, "ConversationHistory");
			DirectoryStrings.stringIDs.Add(2338066360U, "OutboundConnectorTlsSettingsInvalidDomainValidationWithoutTlsDomain");
			DirectoryStrings.stringIDs.Add(282367765U, "WhenMoved");
			DirectoryStrings.stringIDs.Add(1039356237U, "ErrorDuplicateLanguage");
			DirectoryStrings.stringIDs.Add(1268762784U, "ExceptionObjectAlreadyExists");
			DirectoryStrings.stringIDs.Add(908880307U, "EsnLangCzech");
			DirectoryStrings.stringIDs.Add(3859838333U, "ComponentNameInvalid");
			DirectoryStrings.stringIDs.Add(3587044099U, "ErrorAuthMetadataCannotResolveIssuer");
			DirectoryStrings.stringIDs.Add(4137211921U, "GroupNamingPolicyTitle");
			DirectoryStrings.stringIDs.Add(3738926850U, "MailboxMoveStatusSuspended");
			DirectoryStrings.stringIDs.Add(1795765194U, "DomainSecureEnabledWithExternalAuthoritative");
			DirectoryStrings.stringIDs.Add(4014391034U, "BasicAfterTLSWithoutTLS");
			DirectoryStrings.stringIDs.Add(3026477473U, "Private");
			DirectoryStrings.stringIDs.Add(1481245394U, "Mailboxes");
			DirectoryStrings.stringIDs.Add(1548074671U, "ErrorModeratorRequiredForModeration");
			DirectoryStrings.stringIDs.Add(51460946U, "CustomFromAddressRequired");
			DirectoryStrings.stringIDs.Add(1951705699U, "LdapModifyDN");
			DirectoryStrings.stringIDs.Add(2453785463U, "CustomExternalSubjectRequired");
			DirectoryStrings.stringIDs.Add(2119492277U, "ErrorInternalLocationsCountMissMatch");
			DirectoryStrings.stringIDs.Add(2620688997U, "ASOnlyOneAuthenticationMethodAllowed");
			DirectoryStrings.stringIDs.Add(4001967317U, "Tnef");
			DirectoryStrings.stringIDs.Add(2746045715U, "ByteEncoderTypeUseBase64HtmlDetectTextPlain");
			DirectoryStrings.stringIDs.Add(1347571030U, "EsnLangIcelandic");
			DirectoryStrings.stringIDs.Add(2324863376U, "ServerRoleNAT");
			DirectoryStrings.stringIDs.Add(1966081841U, "UniversalDistributionGroupRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(2222835032U, "ErrorReplicationLatency");
			DirectoryStrings.stringIDs.Add(461546579U, "EnabledPartner");
			DirectoryStrings.stringIDs.Add(1942443133U, "OutboundConnectorSmarthostTlsSettingsInvalid");
			DirectoryStrings.stringIDs.Add(1538151754U, "ExternalCompliance");
			DirectoryStrings.stringIDs.Add(1293877238U, "ErrorAuthMetadataNoSigningKey");
			DirectoryStrings.stringIDs.Add(2521212798U, "InboundConnectorIncorrectAllAcceptedDomains");
			DirectoryStrings.stringIDs.Add(1182470434U, "MoveToFolder");
			DirectoryStrings.stringIDs.Add(1421152560U, "Byte");
			DirectoryStrings.stringIDs.Add(2557668279U, "EsnLangCyrillic");
			DirectoryStrings.stringIDs.Add(1711185212U, "CanRunDefaultUpdateState_Invalid");
			DirectoryStrings.stringIDs.Add(3569405894U, "DisabledUserRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(725514504U, "InvalidRecipientType");
			DirectoryStrings.stringIDs.Add(532726102U, "EmailAgeFilterThreeDays");
			DirectoryStrings.stringIDs.Add(3099081087U, "DataMoveReplicationConstraintCISecondCopy");
			DirectoryStrings.stringIDs.Add(1499716658U, "ErrorMissingPrimarySmtp");
			DirectoryStrings.stringIDs.Add(398140363U, "ErrorELCFolderNotSpecified");
			DirectoryStrings.stringIDs.Add(4174419723U, "ErrorCannotHaveMoreThanOneDefaultThrottlingPolicy");
			DirectoryStrings.stringIDs.Add(4070703744U, "ReceiveModeCannotBeZero");
			DirectoryStrings.stringIDs.Add(3248373105U, "OwaDefaultDomainRequiredWhenLogonFormatIsUserName");
			DirectoryStrings.stringIDs.Add(2806561839U, "TLS");
			DirectoryStrings.stringIDs.Add(1432667858U, "LinkedMailboxRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(2966158940U, "Tasks");
			DirectoryStrings.stringIDs.Add(1982036425U, "RejectAndQuarantineThreshold");
			DirectoryStrings.stringIDs.Add(1213745183U, "LdapFilterErrorInvalidDecimal");
			DirectoryStrings.stringIDs.Add(2461262221U, "SpamFilteringTestActionAddXHeader");
			DirectoryStrings.stringIDs.Add(1880194541U, "OrganizationCapabilityScaleOut");
			DirectoryStrings.stringIDs.Add(3454173033U, "ConstraintViolationOneOffSupervisionListEntryStringPartIsInvalid");
			DirectoryStrings.stringIDs.Add(104454802U, "DiscoveryMailboxTypeDetails");
			DirectoryStrings.stringIDs.Add(826243425U, "ErrorAdfsTrustedIssuers");
			DirectoryStrings.stringIDs.Add(2167266391U, "DataMoveReplicationConstraintCIAllDatacenters");
			DirectoryStrings.stringIDs.Add(396559062U, "HygieneSuiteStandard");
			DirectoryStrings.stringIDs.Add(291819084U, "EsnLangHindi");
			DirectoryStrings.stringIDs.Add(708782482U, "ExceptionUnableToCreateConnections");
			DirectoryStrings.stringIDs.Add(796260281U, "SecurityPrincipalTypeWellknownSecurityPrincipal");
			DirectoryStrings.stringIDs.Add(22442200U, "Error");
			DirectoryStrings.stringIDs.Add(1999329050U, "ElcScheduleOnWrongServer");
			DirectoryStrings.stringIDs.Add(3694564633U, "SyncIssues");
			DirectoryStrings.stringIDs.Add(3184119847U, "PartiallyApplied");
			DirectoryStrings.stringIDs.Add(361358848U, "PreferredInternetCodePageUndefined");
			DirectoryStrings.stringIDs.Add(3399683424U, "NoRoleEntriesCmdletOrScriptFound");
			DirectoryStrings.stringIDs.Add(2126631961U, "CannotDeserializePartitionHintTooShort");
			DirectoryStrings.stringIDs.Add(2412912437U, "InvalidReceiveAuthModeTLSPassword");
			DirectoryStrings.stringIDs.Add(1924734200U, "GroupNamingPolicyCustomAttribute8");
			DirectoryStrings.stringIDs.Add(401605495U, "EsnLangSwedish");
			DirectoryStrings.stringIDs.Add(2253071944U, "IndustryUtilities");
			DirectoryStrings.stringIDs.Add(3692015522U, "G711");
			DirectoryStrings.stringIDs.Add(3044377029U, "ExternalDNSServersNotSet");
			DirectoryStrings.stringIDs.Add(3168546709U, "Item");
			DirectoryStrings.stringIDs.Add(3325431492U, "LdapFilterErrorUnsupportedAttributeType");
			DirectoryStrings.stringIDs.Add(2932678028U, "ExternalSenderAdminAddressRequired");
			DirectoryStrings.stringIDs.Add(582855761U, "ErrorBadLocalizedFolderName");
			DirectoryStrings.stringIDs.Add(3297182182U, "AutoDatabaseMountDialBestAvailability");
			DirectoryStrings.stringIDs.Add(391848840U, "OrganizationalFolder");
			DirectoryStrings.stringIDs.Add(2944126402U, "SpamFilteringOptionTest");
			DirectoryStrings.stringIDs.Add(56024811U, "LdapFilterErrorInvalidToken");
			DirectoryStrings.stringIDs.Add(2570241570U, "MessageRateSourceFlagsUser");
			DirectoryStrings.stringIDs.Add(1461717404U, "TextEnrichedAndTextAlternative");
			DirectoryStrings.stringIDs.Add(3920082026U, "FederatedOrganizationIdNoFederatedDomains");
			DirectoryStrings.stringIDs.Add(1191186633U, "GroupTypeFlagsUniversal");
			DirectoryStrings.stringIDs.Add(3774252481U, "CustomAlertTextRequired");
			DirectoryStrings.stringIDs.Add(1272682565U, "EsnLangEstonian");
			DirectoryStrings.stringIDs.Add(1502599728U, "Low");
			DirectoryStrings.stringIDs.Add(467677052U, "IndustryPersonalServices");
			DirectoryStrings.stringIDs.Add(3282711248U, "ErrorInvalidPipelineTracingSenderAddress");
			DirectoryStrings.stringIDs.Add(131691172U, "AccessQuarantined");
			DirectoryStrings.stringIDs.Add(3874249800U, "LdapFilterErrorTypeOnlySpaces");
			DirectoryStrings.stringIDs.Add(2878385120U, "UserFilterChoice");
			DirectoryStrings.stringIDs.Add(587065991U, "ErrorRemovePrimaryExternalSMTPAddress");
			DirectoryStrings.stringIDs.Add(62599113U, "GroupNamingPolicyOffice");
			DirectoryStrings.stringIDs.Add(3503686282U, "ErrorHostServerNotSet");
			DirectoryStrings.stringIDs.Add(1998007652U, "BitMaskOrIpAddressMatchMustBeSet");
			DirectoryStrings.stringIDs.Add(621310157U, "OrganizationCapabilityGMGen");
			DirectoryStrings.stringIDs.Add(3723962467U, "ErrorArchiveDatabaseArchiveDomainConflict");
			DirectoryStrings.stringIDs.Add(2472951404U, "ArchiveStateHostedProvisioned");
			DirectoryStrings.stringIDs.Add(614706510U, "InvalidHttpProtocolLogSizeConfiguration");
			DirectoryStrings.stringIDs.Add(3225083443U, "PermanentMservErrorDescription");
			DirectoryStrings.stringIDs.Add(1285289871U, "CustomExternalBodyRequired");
			DirectoryStrings.stringIDs.Add(1415475463U, "LdapFilterErrorUndefinedAttributeType");
			DirectoryStrings.stringIDs.Add(603975640U, "ErrorTextMessageIncludingHtmlBody");
			DirectoryStrings.stringIDs.Add(3773054995U, "WellKnownRecipientTypeResources");
			DirectoryStrings.stringIDs.Add(2097957443U, "PrimaryDefault");
			DirectoryStrings.stringIDs.Add(2943465798U, "MailFlowPartnerInternalMailContentTypeMimeHtmlText");
			DirectoryStrings.stringIDs.Add(2685207586U, "DataMoveReplicationConstraintNone");
			DirectoryStrings.stringIDs.Add(3040710945U, "ErrorAdfsAudienceUris");
			DirectoryStrings.stringIDs.Add(1190928622U, "InvalidAnrFilter");
			DirectoryStrings.stringIDs.Add(117943812U, "AuditLogMailboxRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(1849540794U, "WellKnownRecipientTypeNone");
			DirectoryStrings.stringIDs.Add(3296665743U, "EsnLangGujarati");
			DirectoryStrings.stringIDs.Add(672991527U, "DomainStateUnknown");
			DirectoryStrings.stringIDs.Add(687591962U, "IndustryManufacturing");
			DirectoryStrings.stringIDs.Add(345329738U, "IndustryHospitality");
			DirectoryStrings.stringIDs.Add(534671299U, "ErrorAdfsIssuer");
			DirectoryStrings.stringIDs.Add(1058308747U, "EmailAgeFilterOneDay");
			DirectoryStrings.stringIDs.Add(3471478219U, "AllEmailMC");
			DirectoryStrings.stringIDs.Add(1753080574U, "OrgContainerAmbiguousException");
			DirectoryStrings.stringIDs.Add(1439034128U, "GlobalThrottlingPolicyNotFoundException");
			DirectoryStrings.stringIDs.Add(3636659332U, "EsnLangTurkish");
			DirectoryStrings.stringIDs.Add(1642025802U, "SKUCapabilityBPOSSLite");
			DirectoryStrings.stringIDs.Add(2794974035U, "RecipientWriteScopes");
			DirectoryStrings.stringIDs.Add(104189932U, "CalendarAgeFilterThreeMonths");
			DirectoryStrings.stringIDs.Add(2669194754U, "MailboxMoveStatusCompletedWithWarning");
			DirectoryStrings.stringIDs.Add(3674978674U, "GroupNamingPolicyCountryOrRegion");
			DirectoryStrings.stringIDs.Add(2482287296U, "EsnLangFrench");
			DirectoryStrings.stringIDs.Add(2050489682U, "CapabilityExcludedFromBackSync");
			DirectoryStrings.stringIDs.Add(452620031U, "CapabilityBEVDirLockdown");
			DirectoryStrings.stringIDs.Add(4409738U, "ReceiveAuthMechanismBasicAuth");
			DirectoryStrings.stringIDs.Add(2046074250U, "IndustryEducation");
			DirectoryStrings.stringIDs.Add(2536752615U, "NotSpecified");
			DirectoryStrings.stringIDs.Add(3675904764U, "PermanentlyDelete");
			DirectoryStrings.stringIDs.Add(2346580185U, "FederatedIdentityMisconfigured");
			DirectoryStrings.stringIDs.Add(3845937663U, "MountDialOverrideNone");
			DirectoryStrings.stringIDs.Add(3563162252U, "AlwaysUTF8");
			DirectoryStrings.stringIDs.Add(2660137110U, "ExceptionPagedReaderIsSingleUse");
			DirectoryStrings.stringIDs.Add(3323087513U, "InvalidFilterLength");
			DirectoryStrings.stringIDs.Add(3664117547U, "MailboxMoveStatusSynced");
			DirectoryStrings.stringIDs.Add(2422734853U, "SIPSecured");
			DirectoryStrings.stringIDs.Add(630988704U, "ErrorRejectedCookie");
			DirectoryStrings.stringIDs.Add(3800196293U, "ASInvalidProxyASUrlOption");
			DirectoryStrings.stringIDs.Add(407788899U, "ServerRoleSCOM");
			DirectoryStrings.stringIDs.Add(3289792773U, "JournalItemsMC");
			DirectoryStrings.stringIDs.Add(2617145200U, "ErrorEmptySearchProperty");
			DirectoryStrings.stringIDs.Add(3900005531U, "OutboundConnectorIncorrectTransportRuleScopedParameters");
			DirectoryStrings.stringIDs.Add(107906018U, "TeamMailboxRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(4272675708U, "CustomRoleDescription_MyMobileInformation");
			DirectoryStrings.stringIDs.Add(920444171U, "ArchiveStateHostedPending");
			DirectoryStrings.stringIDs.Add(2153511661U, "DPCantChangeName");
			DirectoryStrings.stringIDs.Add(2175447826U, "OrganizationCapabilityUMDataStorage");
			DirectoryStrings.stringIDs.Add(2304217557U, "TlsAuthLevelWithRequireTlsDisabled");
			DirectoryStrings.stringIDs.Add(3453679227U, "UndefinedRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(3608358242U, "Upgrade");
			DirectoryStrings.stringIDs.Add(3905558735U, "Global");
			DirectoryStrings.stringIDs.Add(4264103832U, "DeleteMessage");
			DirectoryStrings.stringIDs.Add(4018720312U, "LdapDelete");
			DirectoryStrings.stringIDs.Add(1291341805U, "EsnLangHungarian");
			DirectoryStrings.stringIDs.Add(699909606U, "ErrorAddressAutoCopy");
			DirectoryStrings.stringIDs.Add(2968709845U, "EsnLangLatvian");
			DirectoryStrings.stringIDs.Add(674244647U, "CanRunDefaultUpdateState_NotLocal");
			DirectoryStrings.stringIDs.Add(1855823700U, "Department");
			DirectoryStrings.stringIDs.Add(1123996746U, "SpamFilteringActionJmf");
			DirectoryStrings.stringIDs.Add(1079159280U, "ErrorDDLOperationsError");
			DirectoryStrings.stringIDs.Add(64564864U, "ErrorSharedConfigurationCannotBeEnabled");
			DirectoryStrings.stringIDs.Add(1411554219U, "ErrorMailTipCultureNotSpecified");
			DirectoryStrings.stringIDs.Add(2068838733U, "LdapModify");
			DirectoryStrings.stringIDs.Add(2212942115U, "DataMoveReplicationConstraintSecondDatacenter");
			DirectoryStrings.stringIDs.Add(3501307892U, "CapabilityResourceMailbox");
			DirectoryStrings.stringIDs.Add(2955006930U, "Second");
			DirectoryStrings.stringIDs.Add(451948526U, "InboundConnectorInvalidRestrictDomainsToCertificate");
			DirectoryStrings.stringIDs.Add(97762286U, "GroupNamingPolicyCustomAttribute15");
			DirectoryStrings.stringIDs.Add(1669305113U, "SendAuthMechanismNone");
			DirectoryStrings.stringIDs.Add(2028679986U, "ServicesContainerNotFound");
			DirectoryStrings.stringIDs.Add(368981658U, "MissingDefaultOutboundCallingLineId");
			DirectoryStrings.stringIDs.Add(1638178773U, "GroupTypeFlagsDomainLocal");
			DirectoryStrings.stringIDs.Add(2292597411U, "ErrorCannotAggregateAndLinkMailbox");
			DirectoryStrings.stringIDs.Add(1975373491U, "SyncCommands");
			DirectoryStrings.stringIDs.Add(2584752109U, "PreferredInternetCodePageEsc2022Jp");
			DirectoryStrings.stringIDs.Add(2869997774U, "DirectoryBasedEdgeBlockModeOff");
			DirectoryStrings.stringIDs.Add(3205211544U, "InvalidSourceAddressSetting");
			DirectoryStrings.stringIDs.Add(3459813102U, "ElcContentSettingsDescription");
			DirectoryStrings.stringIDs.Add(3194934827U, "ServerRoleUnifiedMessaging");
			DirectoryStrings.stringIDs.Add(1193235970U, "DataMoveReplicationConstraintCIAllCopies");
			DirectoryStrings.stringIDs.Add(96477845U, "MailTipsAccessLevelLimited");
			DirectoryStrings.stringIDs.Add(4229158936U, "SecondaryMailboxRelationType");
			DirectoryStrings.stringIDs.Add(3828198519U, "Ocs");
			DirectoryStrings.stringIDs.Add(2122644134U, "IndustryOther");
			DirectoryStrings.stringIDs.Add(3032910929U, "ErrorMimeMessageIncludingUuEncodedAttachment");
			DirectoryStrings.stringIDs.Add(1886413222U, "ServerRoleDHCP");
			DirectoryStrings.stringIDs.Add(1924734195U, "GroupNamingPolicyCustomAttribute5");
			DirectoryStrings.stringIDs.Add(102260678U, "EnableNotificationEmail");
			DirectoryStrings.stringIDs.Add(4022404286U, "GroupNamingPolicyCountryCode");
			DirectoryStrings.stringIDs.Add(4204248234U, "MailboxMoveStatusCompleted");
			DirectoryStrings.stringIDs.Add(2831291713U, "IndustryCommunications");
			DirectoryStrings.stringIDs.Add(2559242555U, "LdapFilterErrorNoValidComparison");
			DirectoryStrings.stringIDs.Add(3598244064U, "RssSubscriptions");
			DirectoryStrings.stringIDs.Add(1071018894U, "EsnLangThai");
			DirectoryStrings.stringIDs.Add(2921549042U, "ErrorDDLFilterMissing");
			DirectoryStrings.stringIDs.Add(1447606358U, "ExtendedProtectionNonTlsTerminatingProxyScenarioRequireTls");
			DirectoryStrings.stringIDs.Add(267717298U, "NoResetOrAssignedMvp");
			DirectoryStrings.stringIDs.Add(663506969U, "MountDialOverrideBestEffort");
			DirectoryStrings.stringIDs.Add(2367428005U, "NoComputers");
			DirectoryStrings.stringIDs.Add(665042539U, "RegistryContentTypeException");
			DirectoryStrings.stringIDs.Add(366040629U, "DataMoveReplicationConstraintAllDatacenters");
			DirectoryStrings.stringIDs.Add(2564080149U, "ExceptionObjectNotFound");
			DirectoryStrings.stringIDs.Add(882963645U, "DomainStateCustomProvision");
			DirectoryStrings.stringIDs.Add(3517179940U, "SKUCapabilityBPOSMidSize");
			DirectoryStrings.stringIDs.Add(1117900463U, "LdapFilterErrorUnsupportedOperand");
			DirectoryStrings.stringIDs.Add(483196058U, "DirectoryBasedEdgeBlockModeDefault");
			DirectoryStrings.stringIDs.Add(113073592U, "ErrorWrongTypeParameter");
			DirectoryStrings.stringIDs.Add(2757326190U, "EsnLangCatalan");
			DirectoryStrings.stringIDs.Add(3980183679U, "InvalidSndProtocolLogSizeConfiguration");
			DirectoryStrings.stringIDs.Add(3586160528U, "GroupNamingPolicyCustomAttribute13");
			DirectoryStrings.stringIDs.Add(1960526324U, "ErrorThrottlingPolicyGlobalAndOrganizationScope");
			DirectoryStrings.stringIDs.Add(980672066U, "SMTPAddress");
			DirectoryStrings.stringIDs.Add(3976700013U, "EsnLangPolish");
			DirectoryStrings.stringIDs.Add(1017523965U, "CanEnableLocalCopyState_DatabaseEnabled");
			DirectoryStrings.stringIDs.Add(3315201717U, "EsnLangRomanian");
			DirectoryStrings.stringIDs.Add(1097129869U, "ExternalManagedGroupTypeDetails");
			DirectoryStrings.stringIDs.Add(2773964607U, "DatabaseMasterTypeDag");
			DirectoryStrings.stringIDs.Add(3197896354U, "GroupNamingPolicyExtensionCustomAttribute3");
			DirectoryStrings.stringIDs.Add(3071618850U, "ExchangeConfigurationContainerNotFoundException");
			DirectoryStrings.stringIDs.Add(170342216U, "EsnLangUrdu");
			DirectoryStrings.stringIDs.Add(579329341U, "MservAndMbxExclusive");
			DirectoryStrings.stringIDs.Add(2300412432U, "FirstLast");
			DirectoryStrings.stringIDs.Add(2228665429U, "EsnLangBulgarian");
			DirectoryStrings.stringIDs.Add(1970247521U, "MailEnabledUniversalSecurityGroupRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(3065017355U, "ErrorTimeoutReadingSystemAddressListMemberCount");
			DirectoryStrings.stringIDs.Add(3411768540U, "FaxServerURINoValue");
			DirectoryStrings.stringIDs.Add(1118762177U, "ErrorDefaultThrottlingPolicyNotFound");
			DirectoryStrings.stringIDs.Add(3372089172U, "ErrorRecipientContainerCanNotNull");
			DirectoryStrings.stringIDs.Add(2835967712U, "MoveToArchive");
			DirectoryStrings.stringIDs.Add(667068758U, "ModifySubjectValueNotSet");
			DirectoryStrings.stringIDs.Add(882536335U, "NotLocalMaiboxException");
			DirectoryStrings.stringIDs.Add(3811978523U, "RecipientReadScope");
			DirectoryStrings.stringIDs.Add(1067650092U, "Organizational");
			DirectoryStrings.stringIDs.Add(1818643265U, "SystemAttendantMailboxRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(114732651U, "OrganizationCapabilityOABGen");
			DirectoryStrings.stringIDs.Add(2360810543U, "StarOutToDialPlanEnabled");
			DirectoryStrings.stringIDs.Add(3673152204U, "AuthenticationCredentialNotSet");
			DirectoryStrings.stringIDs.Add(1776441609U, "NotifyOutboundSpamRecipientsRequired");
			DirectoryStrings.stringIDs.Add(2241039844U, "JunkEmail");
			DirectoryStrings.stringIDs.Add(2270844793U, "LdapFilterErrorValueOnlySpaces");
			DirectoryStrings.stringIDs.Add(3423767853U, "SipName");
			DirectoryStrings.stringIDs.Add(24965481U, "EsnLangMalayalam");
			DirectoryStrings.stringIDs.Add(2349327181U, "SpamFilteringActionModifySubject");
			DirectoryStrings.stringIDs.Add(1153697179U, "XHeaderValueNotSet");
			DirectoryStrings.stringIDs.Add(3613623199U, "DeletedItems");
			DirectoryStrings.stringIDs.Add(3387472355U, "OrganizationCapabilityUMGrammarReady");
			DirectoryStrings.stringIDs.Add(142823596U, "LastFirst");
			DirectoryStrings.stringIDs.Add(2055652669U, "SendAuthMechanismExchangeServer");
			DirectoryStrings.stringIDs.Add(322963092U, "RemoteTeamMailboxRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(1068346025U, "OutOfBudgets");
			DirectoryStrings.stringIDs.Add(3424913979U, "Off");
			DirectoryStrings.stringIDs.Add(3200416695U, "GroupTypeFlagsSecurityEnabled");
			DirectoryStrings.stringIDs.Add(2618688392U, "InvalidCookieException");
			DirectoryStrings.stringIDs.Add(122679092U, "UserLanguageChoice");
			DirectoryStrings.stringIDs.Add(1291237470U, "SpamFilteringTestActionBccMessage");
			DirectoryStrings.stringIDs.Add(1118847720U, "DelayCacheFull");
			DirectoryStrings.stringIDs.Add(1149691394U, "ErrorAutoCopyMessageFormat");
			DirectoryStrings.stringIDs.Add(1173768531U, "Reserved3");
			DirectoryStrings.stringIDs.Add(2523055253U, "HtmlOnly");
			DirectoryStrings.stringIDs.Add(285356425U, "DefaultFolder");
			DirectoryStrings.stringIDs.Add(1487832074U, "PublicFolderMailboxRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(1549653732U, "Mp3");
			DirectoryStrings.stringIDs.Add(2737500906U, "FederatedOrganizationIdNotEnabled");
			DirectoryStrings.stringIDs.Add(3082202919U, "EsnLangVietnamese");
			DirectoryStrings.stringIDs.Add(2532765903U, "AccessGranted");
			DirectoryStrings.stringIDs.Add(4281433724U, "MailboxUserRecipientType");
			DirectoryStrings.stringIDs.Add(1183009861U, "ExceptionNoSchemaContainerObject");
			DirectoryStrings.stringIDs.Add(2078410195U, "TargetDeliveryDomainCannotBeStar");
			DirectoryStrings.stringIDs.Add(2402032744U, "ErrorAuthMetadataCannotResolveServiceName");
			DirectoryStrings.stringIDs.Add(4046275528U, "ByteEncoderTypeUseBase64");
			DirectoryStrings.stringIDs.Add(2114338030U, "BackSyncDataSourceReplicationErrorMessage");
			DirectoryStrings.stringIDs.Add(2327110479U, "EsnLangHebrew");
			DirectoryStrings.stringIDs.Add(2099880135U, "WellKnownRecipientTypeAllRecipients");
			DirectoryStrings.stringIDs.Add(33168083U, "ExceptionCredentialsNotSupportedWithoutDC");
			DirectoryStrings.stringIDs.Add(247236896U, "NoneMailboxRelationType");
			DirectoryStrings.stringIDs.Add(1605633982U, "MailboxUserRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(3918345138U, "SpamFilteringActionDelete");
			DirectoryStrings.stringIDs.Add(1574700905U, "FederatedOrganizationIdNotFound");
			DirectoryStrings.stringIDs.Add(3777672192U, "SKUCapabilityBPOSSArchive");
			DirectoryStrings.stringIDs.Add(1038035039U, "ReceiveAuthMechanismIntegrated");
			DirectoryStrings.stringIDs.Add(1607061032U, "NameLookupEnabled");
			DirectoryStrings.stringIDs.Add(1972085753U, "ForceFilter");
			DirectoryStrings.stringIDs.Add(3515139435U, "OrganizationCapabilityOfficeMessageEncryption");
			DirectoryStrings.stringIDs.Add(906595693U, "PreferredInternetCodePageIso2022Jp");
			DirectoryStrings.stringIDs.Add(1412620754U, "AlternateServiceAccountCredentialIsInvalid");
			DirectoryStrings.stringIDs.Add(1859518684U, "EmailAgeFilterTwoWeeks");
			DirectoryStrings.stringIDs.Add(2080073494U, "DeviceOS");
			DirectoryStrings.stringIDs.Add(1451782196U, "ErrorTenantRelocationsAllowedOnlyForRootOrg");
			DirectoryStrings.stringIDs.Add(3809750167U, "OrganizationCapabilityTenantUpgrade");
			DirectoryStrings.stringIDs.Add(426414486U, "StarTlsDomainCapabilitiesNotAllowed");
			DirectoryStrings.stringIDs.Add(2391327300U, "GroupNamingPolicyExtensionCustomAttribute5");
			DirectoryStrings.stringIDs.Add(1103339046U, "ErrorTimeoutWritingSystemAddressListCache");
			DirectoryStrings.stringIDs.Add(2249628033U, "CannotGetLocalSite");
			DirectoryStrings.stringIDs.Add(3080481085U, "DatabaseCopyAutoActivationPolicyUnrestricted");
			DirectoryStrings.stringIDs.Add(3562221485U, "PrivateComputersOnly");
			DirectoryStrings.stringIDs.Add(887700241U, "Always");
			DirectoryStrings.stringIDs.Add(933193541U, "WellKnownRecipientTypeMailUsers");
			DirectoryStrings.stringIDs.Add(3512186809U, "CannotSetZeroAsEapPriority");
			DirectoryStrings.stringIDs.Add(2442344752U, "RootZone");
			DirectoryStrings.stringIDs.Add(1151884593U, "RenameNotAllowed");
			DirectoryStrings.stringIDs.Add(2846264340U, "Unknown");
			DirectoryStrings.stringIDs.Add(4013633336U, "EsnLangItalian");
			DirectoryStrings.stringIDs.Add(2446612004U, "ErrorDisplayNameInvalid");
			DirectoryStrings.stringIDs.Add(10930364U, "ConstraintViolationNotValidLegacyDN");
			DirectoryStrings.stringIDs.Add(3650953906U, "ReceiveExtendedProtectionPolicyRequire");
			DirectoryStrings.stringIDs.Add(2030161115U, "SpamFilteringOptionOff");
			DirectoryStrings.stringIDs.Add(1656602441U, "ExternallyManaged");
			DirectoryStrings.stringIDs.Add(3909129905U, "RequireTLSWithoutTLS");
			DirectoryStrings.stringIDs.Add(1437476905U, "ErrorCannotParseAuthMetadata");
			DirectoryStrings.stringIDs.Add(2411242862U, "ErrorInvalidActivationPreference");
			DirectoryStrings.stringIDs.Add(1499015349U, "CapabilityFederatedUser");
			DirectoryStrings.stringIDs.Add(992862894U, "EsnLangFilipino");
			DirectoryStrings.stringIDs.Add(2687926967U, "OutboundConnectorUseMXRecordShouldBeFalseIfSmartHostsIsPresent");
			DirectoryStrings.stringIDs.Add(1688256845U, "LdapFilterErrorBracketMismatch");
			DirectoryStrings.stringIDs.Add(843851219U, "SipResourceIdentifierRequiredNotAllowed");
			DirectoryStrings.stringIDs.Add(2178386640U, "XMSWLHeader");
			DirectoryStrings.stringIDs.Add(1536572748U, "ServerRoleCafe");
			DirectoryStrings.stringIDs.Add(3319415544U, "DeleteAndRejectThreshold");
			DirectoryStrings.stringIDs.Add(816661212U, "Policy");
			DirectoryStrings.stringIDs.Add(2870485117U, "CanRunRestoreState_NotLocal");
			DirectoryStrings.stringIDs.Add(2379521528U, "ElcAuditLogPathMissing");
			DirectoryStrings.stringIDs.Add(4221359213U, "ClientCertAuthIgnore");
			DirectoryStrings.stringIDs.Add(1173768532U, "Reserved2");
			DirectoryStrings.stringIDs.Add(3743229054U, "ConfigWriteScopes");
			DirectoryStrings.stringIDs.Add(3856328942U, "DetailsTemplateCorrupted");
			DirectoryStrings.stringIDs.Add(1942592476U, "ClientCertAuthAccepted");
			DirectoryStrings.stringIDs.Add(1778180980U, "ExceptionAdminLimitExceeded");
			DirectoryStrings.stringIDs.Add(970530017U, "DataMoveReplicationConstraintSecondCopy");
			DirectoryStrings.stringIDs.Add(3956092407U, "ReceiveAuthMechanismTls");
			DirectoryStrings.stringIDs.Add(990840756U, "CannotFindTemplateTenant");
			DirectoryStrings.stringIDs.Add(2189879122U, "FailedToReadStoreUserInformation");
			DirectoryStrings.stringIDs.Add(2227674028U, "MicrosoftExchangeRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(1188824751U, "DataMoveReplicationConstraintCINoReplication");
			DirectoryStrings.stringIDs.Add(575137052U, "ErrorTransitionCounterHasZeroCount");
			DirectoryStrings.stringIDs.Add(73047031U, "DeleteAndQuarantineThreshold");
			DirectoryStrings.stringIDs.Add(1642796619U, "IndustryAgriculture");
			DirectoryStrings.stringIDs.Add(3603173284U, "ClientCertAuthRequired");
			DirectoryStrings.stringIDs.Add(3707194057U, "ServerRoleExtendedRole7");
			DirectoryStrings.stringIDs.Add(2804536165U, "SubmissionOverrideListOnWrongServer");
			DirectoryStrings.stringIDs.Add(4170864973U, "EsnLangBasque");
			DirectoryStrings.stringIDs.Add(659240048U, "UserRecipientType");
			DirectoryStrings.stringIDs.Add(363625972U, "MailEnabledUserRecipientType");
			DirectoryStrings.stringIDs.Add(4189167987U, "GroupTypeFlagsGlobal");
			DirectoryStrings.stringIDs.Add(4091901749U, "DataMoveReplicationConstraintCISecondDatacenter");
			DirectoryStrings.stringIDs.Add(4024084584U, "LoadBalanceCannotUseBothInclusionLists");
			DirectoryStrings.stringIDs.Add(2771491650U, "ExchangeMissedcallMC");
			DirectoryStrings.stringIDs.Add(3414623930U, "RequesterNameInvalid");
			DirectoryStrings.stringIDs.Add(3393062226U, "ByteEncoderTypeUseBase64Html7BitTextPlain");
			DirectoryStrings.stringIDs.Add(2666751303U, "SecurityPrincipalTypeComputer");
			DirectoryStrings.stringIDs.Add(3235051081U, "EsnLangAmharic");
			DirectoryStrings.stringIDs.Add(3212819533U, "LimitedMoveOnlyAllowed");
			DirectoryStrings.stringIDs.Add(2828547743U, "ASInvalidAuthenticationOptionsForAccessMethod");
			DirectoryStrings.stringIDs.Add(1241097582U, "NullPasswordEncryptionKey");
			DirectoryStrings.stringIDs.Add(1738880682U, "LinkedUserTypeDetails");
			DirectoryStrings.stringIDs.Add(3409499789U, "AutoDatabaseMountDialLossless");
			DirectoryStrings.stringIDs.Add(3238398976U, "ReceiveAuthMechanismExternalAuthoritative");
			DirectoryStrings.stringIDs.Add(1472430496U, "ErrorTruncationLagTime");
			DirectoryStrings.stringIDs.Add(1996912758U, "ExceptionIdImmutable");
			DirectoryStrings.stringIDs.Add(1638589089U, "ExceptionDefaultScopeAndSearchRoot");
			DirectoryStrings.stringIDs.Add(3667281372U, "ErrorOfferProgramIdMandatoryOnSharedConfig");
			DirectoryStrings.stringIDs.Add(3707194060U, "ServerRoleExtendedRole4");
			DirectoryStrings.stringIDs.Add(3615458703U, "ErrorComment");
			DirectoryStrings.stringIDs.Add(4244443796U, "ErrorReplayLagTime");
			DirectoryStrings.stringIDs.Add(64170653U, "ExLengthOfVersionByteArrayError");
			DirectoryStrings.stringIDs.Add(3109296950U, "LdapAdd");
			DirectoryStrings.stringIDs.Add(1578973890U, "DomainStatePendingActivation");
			DirectoryStrings.stringIDs.Add(3790383196U, "Uninterruptible");
			DirectoryStrings.stringIDs.Add(3789585333U, "ErrorMustBeADRawEntry");
			DirectoryStrings.stringIDs.Add(1414246128U, "None");
			DirectoryStrings.stringIDs.Add(1049375487U, "ErrorBadLocalizedComment");
			DirectoryStrings.stringIDs.Add(1223035494U, "EsnLangSlovak");
			DirectoryStrings.stringIDs.Add(3378717027U, "LdapFilterErrorInvalidBooleanValue");
			DirectoryStrings.stringIDs.Add(1343826401U, "OabVersionsNullException");
			DirectoryStrings.stringIDs.Add(2979702410U, "Inbox");
			DirectoryStrings.stringIDs.Add(137387861U, "ContactRecipientTypeDetails");
			DirectoryStrings.stringIDs.Add(1522344710U, "EsnLangKazakh");
			DirectoryStrings.stringIDs.Add(1662145344U, "DisableFilter");
			DirectoryStrings.stringIDs.Add(3713089550U, "BluetoothHandsfreeOnly");
			DirectoryStrings.stringIDs.Add(3178378607U, "GatewayGuid");
			DirectoryStrings.stringIDs.Add(3927045149U, "CalendarSharingFreeBusyNone");
		}

		public static LocalizedString ExceptionADWriteDisabled(string process, string forest)
		{
			return new LocalizedString("ExceptionADWriteDisabled", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				process,
				forest
			});
		}

		public static LocalizedString GroupMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("GroupMailboxRecipientTypeDetails", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MobileAdOrphanFound(string id)
		{
			return new LocalizedString("MobileAdOrphanFound", "ExE7B760", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString InvalidTransportSyncHealthLogSizeConfiguration
		{
			get
			{
				return new LocalizedString("InvalidTransportSyncHealthLogSizeConfiguration", "ExDAB1CE", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReceiveExtendedProtectionPolicyNone
		{
			get
			{
				return new LocalizedString("ReceiveExtendedProtectionPolicyNone", "Ex5F3B5A", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OrganizationCapabilityManagement
		{
			get
			{
				return new LocalizedString("OrganizationCapabilityManagement", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangTamil
		{
			get
			{
				return new LocalizedString("EsnLangTamil", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionInvalidOperationOnReadOnlyObject(string operation)
		{
			return new LocalizedString("ExceptionInvalidOperationOnReadOnlyObject", "Ex0006B6", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				operation
			});
		}

		public static LocalizedString ErrorUnsafeIdentityFilterNotAllowed(string filter, string orgId)
		{
			return new LocalizedString("ErrorUnsafeIdentityFilterNotAllowed", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				filter,
				orgId
			});
		}

		public static LocalizedString LdapFilterErrorInvalidWildCard
		{
			get
			{
				return new LocalizedString("LdapFilterErrorInvalidWildCard", "Ex59E760", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Individual
		{
			get
			{
				return new LocalizedString("Individual", "Ex96E414", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalRelay
		{
			get
			{
				return new LocalizedString("ExternalRelay", "Ex966870", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorProductFileDirectoryIdenticalWithCopyFileDirectory(string directoryName)
		{
			return new LocalizedString("ErrorProductFileDirectoryIdenticalWithCopyFileDirectory", "Ex8DB0BF", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				directoryName
			});
		}

		public static LocalizedString InvalidTransportSyncDownloadSizeConfiguration
		{
			get
			{
				return new LocalizedString("InvalidTransportSyncDownloadSizeConfiguration", "ExF31F0D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageRateSourceFlagsAll
		{
			get
			{
				return new LocalizedString("MessageRateSourceFlagsAll", "Ex4D0C10", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorIsServerSuitableMissingDefaultNamingContext(string dcName)
		{
			return new LocalizedString("ErrorIsServerSuitableMissingDefaultNamingContext", "Ex6B1A1F", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				dcName
			});
		}

		public static LocalizedString SKUCapabilityBPOSSBasic
		{
			get
			{
				return new LocalizedString("SKUCapabilityBPOSSBasic", "Ex42AAA4", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorBothTargetAndSourceForestPopulated(string fqdn1, string fqdn2)
		{
			return new LocalizedString("ErrorBothTargetAndSourceForestPopulated", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				fqdn1,
				fqdn2
			});
		}

		public static LocalizedString IndustryMediaMarketingAdvertising
		{
			get
			{
				return new LocalizedString("IndustryMediaMarketingAdvertising", "Ex44C191", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SKUCapabilityUnmanaged
		{
			get
			{
				return new LocalizedString("SKUCapabilityUnmanaged", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BackSyncDataSourceTransientErrorMessage
		{
			get
			{
				return new LocalizedString("BackSyncDataSourceTransientErrorMessage", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailEnabledNonUniversalGroupRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("MailEnabledNonUniversalGroupRecipientTypeDetails", "Ex876BC8", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADDriverStoreAccessPermanentError
		{
			get
			{
				return new LocalizedString("ADDriverStoreAccessPermanentError", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceType
		{
			get
			{
				return new LocalizedString("DeviceType", "Ex1B6E63", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnsupportedObjectClass(string objectClass)
		{
			return new LocalizedString("UnsupportedObjectClass", "Ex1D7664", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				objectClass
			});
		}

		public static LocalizedString DefaultAdministrativeGroupNotFoundException(string agName)
		{
			return new LocalizedString("DefaultAdministrativeGroupNotFoundException", "Ex155173", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				agName
			});
		}

		public static LocalizedString PropertyDependencyRequired(string propertyName, string dependantPropertyName)
		{
			return new LocalizedString("PropertyDependencyRequired", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				propertyName,
				dependantPropertyName
			});
		}

		public static LocalizedString ProviderFactoryClassNotFoundLoadException(string providerName, string assemblyPath, string factoryTypeName)
		{
			return new LocalizedString("ProviderFactoryClassNotFoundLoadException", "Ex152E2A", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				providerName,
				assemblyPath,
				factoryTypeName
			});
		}

		public static LocalizedString EsnLangFarsi
		{
			get
			{
				return new LocalizedString("EsnLangFarsi", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidTempErrorSetting
		{
			get
			{
				return new LocalizedString("InvalidTempErrorSetting", "ExAB3AD8", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfigurationSettingsRestrictionSummary(string nameMatch, string minVersion, string maxVersion)
		{
			return new LocalizedString("ConfigurationSettingsRestrictionSummary", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				nameMatch,
				minVersion,
				maxVersion
			});
		}

		public static LocalizedString ReplicationTypeNone
		{
			get
			{
				return new LocalizedString("ReplicationTypeNone", "Ex3AD385", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IndustryBusinessServicesConsulting
		{
			get
			{
				return new LocalizedString("IndustryBusinessServicesConsulting", "Ex768107", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TenantNotFoundInGlsError(string tenant)
		{
			return new LocalizedString("TenantNotFoundInGlsError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				tenant
			});
		}

		public static LocalizedString ErrorAdfsConfigFormat
		{
			get
			{
				return new LocalizedString("ErrorAdfsConfigFormat", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Quarantined
		{
			get
			{
				return new LocalizedString("Quarantined", "Ex2E9B51", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidCallSomeoneScopeSettings(string prop1, string prop2)
		{
			return new LocalizedString("InvalidCallSomeoneScopeSettings", "Ex165C88", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				prop1,
				prop2
			});
		}

		public static LocalizedString ConfigurationSettingsNotUnique(string name)
		{
			return new LocalizedString("ConfigurationSettingsNotUnique", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString OutboundConnectorSmartHostShouldBePresentIfUseMXRecordFalse
		{
			get
			{
				return new LocalizedString("OutboundConnectorSmartHostShouldBePresentIfUseMXRecordFalse", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LongRunningCostHandle
		{
			get
			{
				return new LocalizedString("LongRunningCostHandle", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangChineseTraditional
		{
			get
			{
				return new LocalizedString("EsnLangChineseTraditional", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IndustryTransportation
		{
			get
			{
				return new LocalizedString("IndustryTransportation", "Ex0407D8", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Silent
		{
			get
			{
				return new LocalizedString("Silent", "ExBA24CB", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlternateServiceAccountCredentialQualifiedUserNameWrongFormat
		{
			get
			{
				return new LocalizedString("AlternateServiceAccountCredentialQualifiedUserNameWrongFormat", "Ex5F12C0", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KpkUseProblem(string propertyName)
		{
			return new LocalizedString("KpkUseProblem", "Ex81BA56", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				propertyName
			});
		}

		public static LocalizedString InvalidBannerSetting
		{
			get
			{
				return new LocalizedString("InvalidBannerSetting", "Ex287F11", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute4
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute4", "Ex28B52D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboundConnectorIncorrectCloudServicesMailEnabled
		{
			get
			{
				return new LocalizedString("InboundConnectorIncorrectCloudServicesMailEnabled", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorAnrIsNotSupported
		{
			get
			{
				return new LocalizedString("LdapFilterErrorAnrIsNotSupported", "Ex6BF14B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString E164
		{
			get
			{
				return new LocalizedString("E164", "ExA4442B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ForwardingSmtpAddressNotValidSmtpAddress(object address)
		{
			return new LocalizedString("ForwardingSmtpAddressNotValidSmtpAddress", "Ex2E510E", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString ConfigurationSettingsGroupNotFound(string name)
		{
			return new LocalizedString("ConfigurationSettingsGroupNotFound", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ErrorAuthMetaDataContentEmpty
		{
			get
			{
				return new LocalizedString("ErrorAuthMetaDataContentEmpty", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailEnabledContactRecipientType
		{
			get
			{
				return new LocalizedString("MailEnabledContactRecipientType", "Ex27C052", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MoreThanOneRecipientWithNetId(string netId)
		{
			return new LocalizedString("MoreThanOneRecipientWithNetId", "ExCB5C82", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				netId
			});
		}

		public static LocalizedString MailEnabledUniversalDistributionGroupRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("MailEnabledUniversalDistributionGroupRecipientTypeDetails", "Ex7C8E9D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SendAuthMechanismExternalAuthoritative
		{
			get
			{
				return new LocalizedString("SendAuthMechanismExternalAuthoritative", "Ex23FFEF", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboundConnectorRequiredTlsSettingsInvalid
		{
			get
			{
				return new LocalizedString("InboundConnectorRequiredTlsSettingsInvalid", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute1
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute1", "Ex71E24D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Dual
		{
			get
			{
				return new LocalizedString("Dual", "Ex21FA19", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseCopyAutoActivationPolicyIntrasiteOnly
		{
			get
			{
				return new LocalizedString("DatabaseCopyAutoActivationPolicyIntrasiteOnly", "Ex986029", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotResolvePartitionFqdnError(string fqdn)
		{
			return new LocalizedString("CannotResolvePartitionFqdnError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				fqdn
			});
		}

		public static LocalizedString Never
		{
			get
			{
				return new LocalizedString("Never", "Ex5316EA", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ByteEncoderTypeUndefined
		{
			get
			{
				return new LocalizedString("ByteEncoderTypeUndefined", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRcvProtocolLogSizeConfiguration
		{
			get
			{
				return new LocalizedString("InvalidRcvProtocolLogSizeConfiguration", "Ex24493B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetRootDseRequiresDomainController
		{
			get
			{
				return new LocalizedString("GetRootDseRequiresDomainController", "ExDC9BB2", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPublicFolderReferralConflict(string entry1, string entry2)
		{
			return new LocalizedString("ErrorPublicFolderReferralConflict", "Ex0A81BB", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				entry1,
				entry2
			});
		}

		public static LocalizedString ExceptionADTopologyCreationTimeout(int timeoutSeconds)
		{
			return new LocalizedString("ExceptionADTopologyCreationTimeout", "Ex2BC3C3", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				timeoutSeconds
			});
		}

		public static LocalizedString InheritFromDialPlan
		{
			get
			{
				return new LocalizedString("InheritFromDialPlan", "Ex03EEAC", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionADOperationFailedEntryAlreadyExist(string server, string dn)
		{
			return new LocalizedString("ExceptionADOperationFailedEntryAlreadyExist", "Ex905F80", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				server,
				dn
			});
		}

		public static LocalizedString OrganizationCapabilityMessageTracking
		{
			get
			{
				return new LocalizedString("OrganizationCapabilityMessageTracking", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboundConnectorInvalidTlsSenderCertificateName
		{
			get
			{
				return new LocalizedString("InboundConnectorInvalidTlsSenderCertificateName", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADTopologyEndpointNotFoundException(string url)
		{
			return new LocalizedString("ADTopologyEndpointNotFoundException", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString SoftDelete
		{
			get
			{
				return new LocalizedString("SoftDelete", "ExF5005A", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AsyncTimeout(int timeoutSeconds)
		{
			return new LocalizedString("AsyncTimeout", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				timeoutSeconds
			});
		}

		public static LocalizedString OrganizationCapabilityUMGrammar
		{
			get
			{
				return new LocalizedString("OrganizationCapabilityUMGrammar", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Allow
		{
			get
			{
				return new LocalizedString("Allow", "Ex47E7DF", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNonUniqueExchangeGuid(string guidString)
		{
			return new LocalizedString("ErrorNonUniqueExchangeGuid", "Ex1D084C", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				guidString
			});
		}

		public static LocalizedString DomainNameIsNull
		{
			get
			{
				return new LocalizedString("DomainNameIsNull", "Ex142269", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PromptForAlias
		{
			get
			{
				return new LocalizedString("PromptForAlias", "ExE2E4E4", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSyncObjectId(string idValue)
		{
			return new LocalizedString("InvalidSyncObjectId", "ExBBC3F0", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				idValue
			});
		}

		public static LocalizedString AggregatedSessionCannotMakeADChanges(string attribute)
		{
			return new LocalizedString("AggregatedSessionCannotMakeADChanges", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				attribute
			});
		}

		public static LocalizedString ErrorSystemAddressListInWrongContainer
		{
			get
			{
				return new LocalizedString("ErrorSystemAddressListInWrongContainer", "Ex997361", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionUnableToDisableAdminTopologyMode
		{
			get
			{
				return new LocalizedString("ExceptionUnableToDisableAdminTopologyMode", "Ex8DFF95", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Secured
		{
			get
			{
				return new LocalizedString("Secured", "ExEF4E06", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalAndAuthSet
		{
			get
			{
				return new LocalizedString("ExternalAndAuthSet", "ExC4D34A", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangJapanese
		{
			get
			{
				return new LocalizedString("EsnLangJapanese", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidRemoteRecipientType(string value)
		{
			return new LocalizedString("ErrorInvalidRemoteRecipientType", "Ex82B8CE", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString EsnLangPortuguesePortugal
		{
			get
			{
				return new LocalizedString("EsnLangPortuguesePortugal", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangFinnish
		{
			get
			{
				return new LocalizedString("EsnLangFinnish", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionOwaCannotSetPropertyOnVirtualDirectoryOtherThanExchweb
		{
			get
			{
				return new LocalizedString("ExceptionOwaCannotSetPropertyOnVirtualDirectoryOtherThanExchweb", "ExF78B27", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TenantIsRelocatedException(string dn)
		{
			return new LocalizedString("TenantIsRelocatedException", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				dn
			});
		}

		public static LocalizedString WhenDelivered
		{
			get
			{
				return new LocalizedString("WhenDelivered", "Ex981FFA", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DomainStatePendingRelease
		{
			get
			{
				return new LocalizedString("DomainStatePendingRelease", "Ex9BFAF1", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyExtensionCustomAttribute2
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyExtensionCustomAttribute2", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoGroup
		{
			get
			{
				return new LocalizedString("AutoGroup", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorStartDateExpiration
		{
			get
			{
				return new LocalizedString("ErrorStartDateExpiration", "Ex131185", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSingletonMailboxLocationType(string mailboxLocationType)
		{
			return new LocalizedString("ErrorSingletonMailboxLocationType", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				mailboxLocationType
			});
		}

		public static LocalizedString MailboxMoveStatusQueued
		{
			get
			{
				return new LocalizedString("MailboxMoveStatusQueued", "Ex15556A", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Minute
		{
			get
			{
				return new LocalizedString("Minute", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SentItems
		{
			get
			{
				return new LocalizedString("SentItems", "Ex508526", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeVoicemailMC
		{
			get
			{
				return new LocalizedString("ExchangeVoicemailMC", "ExAA11CF", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DuplicateHolidaysError(string s)
		{
			return new LocalizedString("DuplicateHolidaysError", "Ex3C7541", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString UnknownAccountForest(string forest)
		{
			return new LocalizedString("UnknownAccountForest", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				forest
			});
		}

		public static LocalizedString ExchangeUpgradeBucketInvalidVersionFormat(string version)
		{
			return new LocalizedString("ExchangeUpgradeBucketInvalidVersionFormat", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				version
			});
		}

		public static LocalizedString InvalidControlAttributeForTemplateType(string controlType, string pageName, int controlPosition, string attributeName, string template)
		{
			return new LocalizedString("InvalidControlAttributeForTemplateType", "Ex10710B", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				controlType,
				pageName,
				controlPosition,
				attributeName,
				template
			});
		}

		public static LocalizedString AppliedInFull
		{
			get
			{
				return new LocalizedString("AppliedInFull", "ExD314FB", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoAddressSpaces
		{
			get
			{
				return new LocalizedString("NoAddressSpaces", "Ex95B78A", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SKUCapabilityEOPStandardAddOn
		{
			get
			{
				return new LocalizedString("SKUCapabilityEOPStandardAddOn", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionUnsupportedFilter(string filterType)
		{
			return new LocalizedString("ExceptionUnsupportedFilter", "Ex13B3D4", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				filterType
			});
		}

		public static LocalizedString IndustryNonProfit
		{
			get
			{
				return new LocalizedString("IndustryNonProfit", "Ex0BDDA6", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangDefault
		{
			get
			{
				return new LocalizedString("EsnLangDefault", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpecifyCustomGreetingFileName
		{
			get
			{
				return new LocalizedString("SpecifyCustomGreetingFileName", "ExA4123E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangSlovenian
		{
			get
			{
				return new LocalizedString("EsnLangSlovenian", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TelExtn
		{
			get
			{
				return new LocalizedString("TelExtn", "Ex28FC3B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNonUniqueDomainAccount(string domainName, string accountName)
		{
			return new LocalizedString("ErrorNonUniqueDomainAccount", "ExF77354", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				domainName,
				accountName
			});
		}

		public static LocalizedString LdapFilterErrorInvalidGtLtOperand
		{
			get
			{
				return new LocalizedString("LdapFilterErrorInvalidGtLtOperand", "Ex634ACA", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SystemMailboxRecipientType
		{
			get
			{
				return new LocalizedString("SystemMailboxRecipientType", "ExE85A1A", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PilotingOrganization_Error(string objectName, string cmdLet, string parameters, string capabilities)
		{
			return new LocalizedString("PilotingOrganization_Error", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				objectName,
				cmdLet,
				parameters,
				capabilities
			});
		}

		public static LocalizedString ReplicationTypeRemote
		{
			get
			{
				return new LocalizedString("ReplicationTypeRemote", "Ex186CD3", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Enterprise
		{
			get
			{
				return new LocalizedString("Enterprise", "Ex9EE827", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Gsm
		{
			get
			{
				return new LocalizedString("Gsm", "Ex0A6BEC", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Journal
		{
			get
			{
				return new LocalizedString("Journal", "Ex205EC8", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpamFilteringTestActionNone
		{
			get
			{
				return new LocalizedString("SpamFilteringTestActionNone", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfigScopeMustBeEmpty(ConfigWriteScopeType scopeType)
		{
			return new LocalizedString("ConfigScopeMustBeEmpty", "Ex69EE6C", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				scopeType
			});
		}

		public static LocalizedString ErrorDuplicateManagedFolderAddition(string elcFolderName)
		{
			return new LocalizedString("ErrorDuplicateManagedFolderAddition", "Ex09A253", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				elcFolderName
			});
		}

		public static LocalizedString ErrorInvalidConfigScope(string configScope)
		{
			return new LocalizedString("ErrorInvalidConfigScope", "ExEB518A", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				configScope
			});
		}

		public static LocalizedString CustomRoleDescription_MyPersonalInformation
		{
			get
			{
				return new LocalizedString("CustomRoleDescription_MyPersonalInformation", "Ex5E8698", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmailAddressPolicyPriorityLowestFormatError(string value)
		{
			return new LocalizedString("EmailAddressPolicyPriorityLowestFormatError", "Ex8BDEA1", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString MailboxMoveStatusAutoSuspended
		{
			get
			{
				return new LocalizedString("MailboxMoveStatusAutoSuspended", "Ex5C6755", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Any
		{
			get
			{
				return new LocalizedString("Any", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Location
		{
			get
			{
				return new LocalizedString("Location", "Ex0DD32B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalTrust
		{
			get
			{
				return new LocalizedString("ExternalTrust", "Ex483E87", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IndustryPrintingPublishing
		{
			get
			{
				return new LocalizedString("IndustryPrintingPublishing", "ExA934A7", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AllComputers
		{
			get
			{
				return new LocalizedString("AllComputers", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionRusNotFound
		{
			get
			{
				return new LocalizedString("ExceptionRusNotFound", "Ex518DD0", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCity
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCity", "Ex78CA26", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoPagesSpecified
		{
			get
			{
				return new LocalizedString("NoPagesSpecified", "ExAE6AF7", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicDatabaseRecipientType
		{
			get
			{
				return new LocalizedString("PublicDatabaseRecipientType", "Ex493E24", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSubnetNameFormat(string value)
		{
			return new LocalizedString("InvalidSubnetNameFormat", "Ex130E42", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString CanEnableLocalCopyState_CanBeEnabled
		{
			get
			{
				return new LocalizedString("CanEnableLocalCopyState_CanBeEnabled", "Ex1D0587", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorServerRoleNotSupported(string id)
		{
			return new LocalizedString("ErrorServerRoleNotSupported", "ExB932D0", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ServiceInstanceContainerNotFoundException(string serviceInstance)
		{
			return new LocalizedString("ServiceInstanceContainerNotFoundException", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				serviceInstance
			});
		}

		public static LocalizedString InvalidAttachmentFilterRegex(string fileSpec)
		{
			return new LocalizedString("InvalidAttachmentFilterRegex", "Ex93E816", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				fileSpec
			});
		}

		public static LocalizedString RedirectToRecipientsNotSet
		{
			get
			{
				return new LocalizedString("RedirectToRecipientsNotSet", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InfoAnnouncementEnabled
		{
			get
			{
				return new LocalizedString("InfoAnnouncementEnabled", "ExA5E081", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAdfsAudienceUriFormat(string audienceUriString)
		{
			return new LocalizedString("ErrorAdfsAudienceUriFormat", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				audienceUriString
			});
		}

		public static LocalizedString ConfigurationSettingsADConfigDriverError
		{
			get
			{
				return new LocalizedString("ConfigurationSettingsADConfigDriverError", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorEscCharWithoutEscapable
		{
			get
			{
				return new LocalizedString("LdapFilterErrorEscCharWithoutEscapable", "Ex7B5018", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IndustryGovernment
		{
			get
			{
				return new LocalizedString("IndustryGovernment", "Ex83D3DC", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NspiRpcError(string error)
		{
			return new LocalizedString("NspiRpcError", "Ex34AFDD", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString CustomRoleDescription_MyAddressInformation
		{
			get
			{
				return new LocalizedString("CustomRoleDescription_MyAddressInformation", "Ex7F212F", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangNorwegianNynorsk
		{
			get
			{
				return new LocalizedString("EsnLangNorwegianNynorsk", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IndustryEngineeringArchitecture
		{
			get
			{
				return new LocalizedString("IndustryEngineeringArchitecture", "ExBCBB59", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SendAuthMechanismBasicAuth
		{
			get
			{
				return new LocalizedString("SendAuthMechanismBasicAuth", "ExF76D4C", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SKUCapabilityEOPPremiumAddOn
		{
			get
			{
				return new LocalizedString("SKUCapabilityEOPPremiumAddOn", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionConflictingArguments(string property1, object value1, string property2, object value2)
		{
			return new LocalizedString("ExceptionConflictingArguments", "ExD4DA45", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				property1,
				value1,
				property2,
				value2
			});
		}

		public static LocalizedString ExceptionNoSchemaMasterServerObject(string serverId)
		{
			return new LocalizedString("ExceptionNoSchemaMasterServerObject", "Ex42060E", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				serverId
			});
		}

		public static LocalizedString ErrorResourceTypeInvalid
		{
			get
			{
				return new LocalizedString("ErrorResourceTypeInvalid", "ExE8B339", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMailboxExistsInCollection(string guid)
		{
			return new LocalizedString("ErrorMailboxExistsInCollection", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				guid
			});
		}

		public static LocalizedString ConfigurationSettingsDriverNotInitialized(string id)
		{
			return new LocalizedString("ConfigurationSettingsDriverNotInitialized", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString OrgContainerNotFoundException
		{
			get
			{
				return new LocalizedString("OrgContainerNotFoundException", "Ex31B4F7", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidPartitionFqdn(string fqdn)
		{
			return new LocalizedString("InvalidPartitionFqdn", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				fqdn
			});
		}

		public static LocalizedString RoleIsMandatoryInRoleAssignment(string roleAssignment)
		{
			return new LocalizedString("RoleIsMandatoryInRoleAssignment", "ExC24890", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				roleAssignment
			});
		}

		public static LocalizedString MsaUserNotFoundInGlsError(string msaUserNetId)
		{
			return new LocalizedString("MsaUserNotFoundInGlsError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				msaUserNetId
			});
		}

		public static LocalizedString SKUCapabilityBPOSSStandardArchive
		{
			get
			{
				return new LocalizedString("SKUCapabilityBPOSSStandardArchive", "Ex736D27", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InternalSenderAdminAddressRequired
		{
			get
			{
				return new LocalizedString("InternalSenderAdminAddressRequired", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotGetUsefulDomainInfo
		{
			get
			{
				return new LocalizedString("CannotGetUsefulDomainInfo", "Ex49782D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BackSyncDataSourceInDifferentSiteMessage(string domainController)
		{
			return new LocalizedString("BackSyncDataSourceInDifferentSiteMessage", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				domainController
			});
		}

		public static LocalizedString ErrorElcSuspensionNotEnabled
		{
			get
			{
				return new LocalizedString("ErrorElcSuspensionNotEnabled", "Ex4181F3", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseMasterTypeServer
		{
			get
			{
				return new LocalizedString("DatabaseMasterTypeServer", "Ex11F659", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotResolveTenantRelocationRequestIdentity(string name)
		{
			return new LocalizedString("CannotResolveTenantRelocationRequestIdentity", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ConnectionTimeoutLessThanInactivityTimeout
		{
			get
			{
				return new LocalizedString("ConnectionTimeoutLessThanInactivityTimeout", "ExE567D7", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FFOMigration_Error(string objectName, string cmdLet, string parameters, string capabilities)
		{
			return new LocalizedString("FFOMigration_Error", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				objectName,
				cmdLet,
				parameters,
				capabilities
			});
		}

		public static LocalizedString ExceptionTokenGroupsNeedsDomainSession(string id)
		{
			return new LocalizedString("ExceptionTokenGroupsNeedsDomainSession", "ExD7F5EB", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString HygieneSuitePremium
		{
			get
			{
				return new LocalizedString("HygieneSuitePremium", "Ex6251E0", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Exadmin
		{
			get
			{
				return new LocalizedString("Exadmin", "ExDF2256", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAdfsAudienceUriDup(string audienceUriString)
		{
			return new LocalizedString("ErrorAdfsAudienceUriDup", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				audienceUriString
			});
		}

		public static LocalizedString ExceptionADTopologyCannotFindWellKnownExchangeGroup
		{
			get
			{
				return new LocalizedString("ExceptionADTopologyCannotFindWellKnownExchangeGroup", "Ex786BA2", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReplicationNotComplete(string forestName, string dcName)
		{
			return new LocalizedString("ReplicationNotComplete", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				forestName,
				dcName
			});
		}

		public static LocalizedString CommandFrequency
		{
			get
			{
				return new LocalizedString("CommandFrequency", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserIsMandatoryInRoleAssignment(string roleAssignment)
		{
			return new LocalizedString("UserIsMandatoryInRoleAssignment", "Ex835876", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				roleAssignment
			});
		}

		public static LocalizedString IndustryConstruction
		{
			get
			{
				return new LocalizedString("IndustryConstruction", "Ex712455", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SharedMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("SharedMailboxRecipientTypeDetails", "ExB1D42C", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfigurationSettingsInvalidMatch(string expression)
		{
			return new LocalizedString("ConfigurationSettingsInvalidMatch", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				expression
			});
		}

		public static LocalizedString AccessDeniedToEventLog
		{
			get
			{
				return new LocalizedString("AccessDeniedToEventLog", "Ex87B610", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangSerbian
		{
			get
			{
				return new LocalizedString("EsnLangSerbian", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReplicationTypeUnknown
		{
			get
			{
				return new LocalizedString("ReplicationTypeUnknown", "Ex89D12B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDuplicateMapiIdsInConfiguredAttributes
		{
			get
			{
				return new LocalizedString("ErrorDuplicateMapiIdsInConfiguredAttributes", "Ex1AAB23", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DirectoryBasedEdgeBlockModeOn
		{
			get
			{
				return new LocalizedString("DirectoryBasedEdgeBlockModeOn", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LiveCredentialWithoutBasic
		{
			get
			{
				return new LocalizedString("LiveCredentialWithoutBasic", "ExB9FFB5", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TenantIsLockedDownForRelocationException(string dn)
		{
			return new LocalizedString("TenantIsLockedDownForRelocationException", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				dn
			});
		}

		public static LocalizedString ExclusiveConfigScopes
		{
			get
			{
				return new LocalizedString("ExclusiveConfigScopes", "Ex1C038A", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IndustryRealEstate
		{
			get
			{
				return new LocalizedString("IndustryRealEstate", "Ex442E53", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangNorwegian
		{
			get
			{
				return new LocalizedString("EsnLangNorwegian", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleMonitoring
		{
			get
			{
				return new LocalizedString("ServerRoleMonitoring", "Ex734227", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ASInvalidAccessMethod
		{
			get
			{
				return new LocalizedString("ASInvalidAccessMethod", "ExB624F6", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionInvalidApprovedApplication(string cabName)
		{
			return new LocalizedString("ExceptionInvalidApprovedApplication", "Ex6B54A4", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				cabName
			});
		}

		public static LocalizedString NotApplied
		{
			get
			{
				return new LocalizedString("NotApplied", "Ex12D9B4", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfigurationSettingsADNotificationError
		{
			get
			{
				return new LocalizedString("ConfigurationSettingsADNotificationError", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNonUniqueProxy(string proxy)
		{
			return new LocalizedString("ErrorNonUniqueProxy", "Ex64B7A8", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				proxy
			});
		}

		public static LocalizedString ExceptionWKGuidNeedsGCSession(Guid wkguid)
		{
			return new LocalizedString("ExceptionWKGuidNeedsGCSession", "ExA532DB", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				wkguid
			});
		}

		public static LocalizedString MonitoringMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("MonitoringMailboxRecipientTypeDetails", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangCroatian
		{
			get
			{
				return new LocalizedString("EsnLangCroatian", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPolicyDontSupportedPresentationObject(Type poType, Type policyType)
		{
			return new LocalizedString("ErrorPolicyDontSupportedPresentationObject", "Ex0BDC0E", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				poType,
				policyType
			});
		}

		public static LocalizedString TlsAuthLevelWithDomainSecureEnabled
		{
			get
			{
				return new LocalizedString("TlsAuthLevelWithDomainSecureEnabled", "Ex22DD90", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangGerman
		{
			get
			{
				return new LocalizedString("EsnLangGerman", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleAssignmentPolicyDescription_Default
		{
			get
			{
				return new LocalizedString("RoleAssignmentPolicyDescription_Default", "ExEDB6B3", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorIsServerSuitableInvalidOSVersion(string dcName, string osVersion, string osServicePack, string minOSVerion, string minServicePack)
		{
			return new LocalizedString("ErrorIsServerSuitableInvalidOSVersion", "Ex8C2DEE", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				dcName,
				osVersion,
				osServicePack,
				minOSVerion,
				minServicePack
			});
		}

		public static LocalizedString GroupTypeFlagsNone
		{
			get
			{
				return new LocalizedString("GroupTypeFlagsNone", "Ex4DEE8D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SipUriAlreadyRegistered(string sipUri, string user)
		{
			return new LocalizedString("SipUriAlreadyRegistered", "ExDE9693", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				sipUri,
				user
			});
		}

		public static LocalizedString WellKnownRecipientTypeMailboxUsers
		{
			get
			{
				return new LocalizedString("WellKnownRecipientTypeMailboxUsers", "Ex5323F7", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorInvalidWildCardGtLt
		{
			get
			{
				return new LocalizedString("LdapFilterErrorInvalidWildCardGtLt", "ExC904F2", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SmartHostNotSet
		{
			get
			{
				return new LocalizedString("SmartHostNotSet", "Ex036929", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceRule
		{
			get
			{
				return new LocalizedString("DeviceRule", "Ex6D77B2", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotTrust
		{
			get
			{
				return new LocalizedString("NotTrust", "Ex12ECDA", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmailAgeFilterAll
		{
			get
			{
				return new LocalizedString("EmailAgeFilterAll", "ExD345D8", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LanguageBlockListNotSet
		{
			get
			{
				return new LocalizedString("LanguageBlockListNotSet", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangSerbianCyrillic
		{
			get
			{
				return new LocalizedString("EsnLangSerbianCyrillic", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarAgeFilterSixMonths
		{
			get
			{
				return new LocalizedString("CalendarAgeFilterSixMonths", "Ex5834F4", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WrongDCForCurrentPartition(string scName, string partitionFqdn)
		{
			return new LocalizedString("WrongDCForCurrentPartition", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				scName,
				partitionFqdn
			});
		}

		public static LocalizedString ErrorMetadataNotSearchProperty
		{
			get
			{
				return new LocalizedString("ErrorMetadataNotSearchProperty", "ExAFF7B8", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BPOS_S_Policy_License_Violation(string objectName, string cmdLet, string parameters, string capabilities)
		{
			return new LocalizedString("BPOS_S_Policy_License_Violation", "ExD4C0C9", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				objectName,
				cmdLet,
				parameters,
				capabilities
			});
		}

		public static LocalizedString ExceptionExtendedRightsNotUnique(string displayName, string exRight1, string exRight2)
		{
			return new LocalizedString("ExceptionExtendedRightsNotUnique", "Ex952707", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				displayName,
				exRight1,
				exRight2
			});
		}

		public static LocalizedString ExceptionGuidSearchRootWithDefaultScope(string guid)
		{
			return new LocalizedString("ExceptionGuidSearchRootWithDefaultScope", "Ex572273", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				guid
			});
		}

		public static LocalizedString InvalidDefaultMailbox
		{
			get
			{
				return new LocalizedString("InvalidDefaultMailbox", "Ex7FA38D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Drafts
		{
			get
			{
				return new LocalizedString("Drafts", "Ex301322", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteGroupMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("RemoteGroupMailboxRecipientTypeDetails", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangSwahili
		{
			get
			{
				return new LocalizedString("EsnLangSwahili", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionPagedReaderReadAllAfterEnumerating
		{
			get
			{
				return new LocalizedString("ExceptionPagedReaderReadAllAfterEnumerating", "ExBD3952", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DsnDefaultLanguageMustBeSpecificCulture
		{
			get
			{
				return new LocalizedString("DsnDefaultLanguageMustBeSpecificCulture", "ExF0474E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSettingOverrideUnknown(string errorType, string componentName, string objectName, string parameters)
		{
			return new LocalizedString("ErrorSettingOverrideUnknown", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				errorType,
				componentName,
				objectName,
				parameters
			});
		}

		public static LocalizedString BestBodyFormat
		{
			get
			{
				return new LocalizedString("BestBodyFormat", "Ex706614", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorIncorrectlyModifiedMailboxCollection(string property)
		{
			return new LocalizedString("ErrorIncorrectlyModifiedMailboxCollection", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString ConfigurationSettingsRestrictionExtraProperty(string name, string propertyName)
		{
			return new LocalizedString("ConfigurationSettingsRestrictionExtraProperty", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				name,
				propertyName
			});
		}

		public static LocalizedString ConstraintViolationInvalidRecipientType(string propertyName, string actualValue)
		{
			return new LocalizedString("ConstraintViolationInvalidRecipientType", "Ex6552BA", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				propertyName,
				actualValue
			});
		}

		public static LocalizedString CanEnableLocalCopyState_AlreadyEnabled
		{
			get
			{
				return new LocalizedString("CanEnableLocalCopyState_AlreadyEnabled", "Ex59D5C0", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceDiscovery
		{
			get
			{
				return new LocalizedString("DeviceDiscovery", "ExF4E0B7", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AccessDenied
		{
			get
			{
				return new LocalizedString("AccessDenied", "Ex8805BA", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidActiveUserStatisticsLogSizeConfiguration
		{
			get
			{
				return new LocalizedString("InvalidActiveUserStatisticsLogSizeConfiguration", "Ex2D8E8D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorActionOnExpirationSpecified
		{
			get
			{
				return new LocalizedString("ErrorActionOnExpirationSpecified", "Ex198A72", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToResolveMapiIdException(int mapiid)
		{
			return new LocalizedString("UnableToResolveMapiIdException", "Ex21AC53", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				mapiid
			});
		}

		public static LocalizedString TlsAuthLevelWithNoDomainOnSmartHost
		{
			get
			{
				return new LocalizedString("TlsAuthLevelWithNoDomainOnSmartHost", "ExD3505B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeferredFailoverEntryString
		{
			get
			{
				return new LocalizedString("DeferredFailoverEntryString", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSettingOverrideInvalidVariantName(string componentName, string sectionName, string variantName, string availableVariantNames)
		{
			return new LocalizedString("ErrorSettingOverrideInvalidVariantName", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				componentName,
				sectionName,
				variantName,
				availableVariantNames
			});
		}

		public static LocalizedString ErrorWebDistributionEnabledWithoutVersion4(string name)
		{
			return new LocalizedString("ErrorWebDistributionEnabledWithoutVersion4", "ExE62073", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString TaskItemsMC
		{
			get
			{
				return new LocalizedString("TaskItemsMC", "Ex1A139D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute7
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute7", "Ex16034B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnknownAttribute
		{
			get
			{
				return new LocalizedString("UnknownAttribute", "Ex6BB76E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MountDialOverrideBestAvailability
		{
			get
			{
				return new LocalizedString("MountDialOverrideBestAvailability", "Ex1CCE6E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotResolveAccountForestDnError(string fqdn)
		{
			return new LocalizedString("CannotResolveAccountForestDnError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				fqdn
			});
		}

		public static LocalizedString ErrorArbitrationMailboxPropertyEmailAddressesEmpty
		{
			get
			{
				return new LocalizedString("ErrorArbitrationMailboxPropertyEmailAddressesEmpty", "ExD43DC1", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlternateServiceAccountCredentialNotSet
		{
			get
			{
				return new LocalizedString("AlternateServiceAccountCredentialNotSet", "ExE2101D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MobileMetabasePathIsInvalid(string id, string path)
		{
			return new LocalizedString("MobileMetabasePathIsInvalid", "Ex95ABC7", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				id,
				path
			});
		}

		public static LocalizedString DataMoveReplicationConstraintAllCopies
		{
			get
			{
				return new LocalizedString("DataMoveReplicationConstraintAllCopies", "Ex98FF4C", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GlobalThrottlingPolicyAmbiguousException
		{
			get
			{
				return new LocalizedString("GlobalThrottlingPolicyAmbiguousException", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidServerStatisticsLogSizeConfiguration
		{
			get
			{
				return new LocalizedString("InvalidServerStatisticsLogSizeConfiguration", "ExDCECA4", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SipResourceIdRequired
		{
			get
			{
				return new LocalizedString("SipResourceIdRequired", "ExA03081", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAttachmentFilterExtension(string fileSpec)
		{
			return new LocalizedString("InvalidAttachmentFilterExtension", "ExF84F4A", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				fileSpec
			});
		}

		public static LocalizedString EsnLangPortuguese
		{
			get
			{
				return new LocalizedString("EsnLangPortuguese", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoDetect
		{
			get
			{
				return new LocalizedString("AutoDetect", "ExDBE7AF", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidAuthSettings(string orgId)
		{
			return new LocalizedString("ErrorInvalidAuthSettings", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				orgId
			});
		}

		public static LocalizedString SpamFilteringActionRedirect
		{
			get
			{
				return new LocalizedString("SpamFilteringActionRedirect", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CanRunRestoreState_Invalid
		{
			get
			{
				return new LocalizedString("CanRunRestoreState_Invalid", "ExCC2634", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OutboundConnectorIncorrectCloudServicesMailEnabled
		{
			get
			{
				return new LocalizedString("OutboundConnectorIncorrectCloudServicesMailEnabled", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseCopyAutoActivationPolicyBlocked
		{
			get
			{
				return new LocalizedString("DatabaseCopyAutoActivationPolicyBlocked", "ExA9434B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomRoleDescription_MyName
		{
			get
			{
				return new LocalizedString("CustomRoleDescription_MyName", "Ex806355", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorExchangeGroupNotFound(Guid idStringValue)
		{
			return new LocalizedString("ErrorExchangeGroupNotFound", "Ex519D3C", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				idStringValue
			});
		}

		public static LocalizedString EsnLangOriya
		{
			get
			{
				return new LocalizedString("EsnLangOriya", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSettingOverrideInvalidVariantValue(string componentName, string sectionName, string variantName, string variantType, string format)
		{
			return new LocalizedString("ErrorSettingOverrideInvalidVariantValue", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				componentName,
				sectionName,
				variantName,
				variantType,
				format
			});
		}

		public static LocalizedString UserAgent
		{
			get
			{
				return new LocalizedString("UserAgent", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnKnownScopeRestrictionType(string scopeType, string objectName)
		{
			return new LocalizedString("UnKnownScopeRestrictionType", "ExDFC368", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				scopeType,
				objectName
			});
		}

		public static LocalizedString DomainStateActive
		{
			get
			{
				return new LocalizedString("DomainStateActive", "ExE67962", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PartnersCannotHaveWildcards
		{
			get
			{
				return new LocalizedString("PartnersCannotHaveWildcards", "Ex2959D7", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NonexistentTimeZoneError(string etz)
		{
			return new LocalizedString("NonexistentTimeZoneError", "Ex39DB84", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				etz
			});
		}

		public static LocalizedString IPv4Only
		{
			get
			{
				return new LocalizedString("IPv4Only", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboundConnectorInvalidIPCertificateCombinations
		{
			get
			{
				return new LocalizedString("InboundConnectorInvalidIPCertificateCombinations", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Exchange2003or2000
		{
			get
			{
				return new LocalizedString("Exchange2003or2000", "Ex091970", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorOneProcessInitializedAsBothSingleAndMultiple
		{
			get
			{
				return new LocalizedString("ErrorOneProcessInitializedAsBothSingleAndMultiple", "Ex013067", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoomListGroupTypeDetails
		{
			get
			{
				return new LocalizedString("RoomListGroupTypeDetails", "Ex2A94AA", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailEnabledForestContactRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("MailEnabledForestContactRecipientTypeDetails", "ExF49729", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAuthMetadataNoIssuingEndpoint
		{
			get
			{
				return new LocalizedString("ErrorAuthMetadataNoIssuingEndpoint", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorExceededMultiTenantResourceCountQuota(string policyId, string poType, string org, int countQuota)
		{
			return new LocalizedString("ErrorExceededMultiTenantResourceCountQuota", "Ex39B7BD", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				policyId,
				poType,
				org,
				countQuota
			});
		}

		public static LocalizedString NonUniversalGroupRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("NonUniversalGroupRecipientTypeDetails", "Ex71129D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMustBeSysConfigObject
		{
			get
			{
				return new LocalizedString("ErrorMustBeSysConfigObject", "Ex6088D1", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddressBookNotFoundException(string id)
		{
			return new LocalizedString("AddressBookNotFoundException", "Ex858C48", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ExtensionIsInvalid(string s, int i)
		{
			return new LocalizedString("ExtensionIsInvalid", "Ex9E4318", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				s,
				i
			});
		}

		public static LocalizedString AppendLocalizedStrings(string str1, string str2)
		{
			return new LocalizedString("AppendLocalizedStrings", "Ex091BF9", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				str1,
				str2
			});
		}

		public static LocalizedString OutboundConnectorTlsSettingsInvalidTlsDomainWithoutDomainValidation
		{
			get
			{
				return new LocalizedString("OutboundConnectorTlsSettingsInvalidTlsDomainWithoutDomainValidation", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorInvalidBitwiseOperand
		{
			get
			{
				return new LocalizedString("LdapFilterErrorInvalidBitwiseOperand", "Ex76E0B9", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionSetPreferredDCsOnlyForManagement
		{
			get
			{
				return new LocalizedString("ExceptionSetPreferredDCsOnlyForManagement", "Ex0B689E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SuitabilityExceptionLdapSearch(string fqnd, string details)
		{
			return new LocalizedString("SuitabilityExceptionLdapSearch", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				fqnd,
				details
			});
		}

		public static LocalizedString LegacyArchiveJournals
		{
			get
			{
				return new LocalizedString("LegacyArchiveJournals", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomInternalSubjectRequired
		{
			get
			{
				return new LocalizedString("CustomInternalSubjectRequired", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotAddArchiveMailbox
		{
			get
			{
				return new LocalizedString("ErrorCannotAddArchiveMailbox", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoNewCalls
		{
			get
			{
				return new LocalizedString("NoNewCalls", "Ex6080C2", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorExchangeMailboxExists(string guid)
		{
			return new LocalizedString("ErrorExchangeMailboxExists", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				guid
			});
		}

		public static LocalizedString ErrorMessageClassEmpty
		{
			get
			{
				return new LocalizedString("ErrorMessageClassEmpty", "Ex502380", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionInvalidVlvFilterProperty(string propertyName)
		{
			return new LocalizedString("ExceptionInvalidVlvFilterProperty", "Ex8695E4", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				propertyName
			});
		}

		public static LocalizedString GloballyDistributedOABCacheReadTimeoutError
		{
			get
			{
				return new LocalizedString("GloballyDistributedOABCacheReadTimeoutError", "ExAC1F55", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Manual
		{
			get
			{
				return new LocalizedString("Manual", "ExF5288D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAcceptedDomainCannotContainWildcardAndNegoConfig
		{
			get
			{
				return new LocalizedString("ErrorAcceptedDomainCannotContainWildcardAndNegoConfig", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UniversalSecurityGroupRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("UniversalSecurityGroupRecipientTypeDetails", "Ex4721BC", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionADInvalidHandleCookie(string server, string message)
		{
			return new LocalizedString("ExceptionADInvalidHandleCookie", "Ex6086AE", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				server,
				message
			});
		}

		public static LocalizedString ArbitrationMailboxTypeDetails
		{
			get
			{
				return new LocalizedString("ArbitrationMailboxTypeDetails", "Ex65DB4F", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAdfsTrustedIssuerFormat(string thumbprintString)
		{
			return new LocalizedString("ErrorAdfsTrustedIssuerFormat", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				thumbprintString
			});
		}

		public static LocalizedString CalendarAgeFilterAll
		{
			get
			{
				return new LocalizedString("CalendarAgeFilterAll", "Ex6DFFF0", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KpkAccessProblem(string propertyName)
		{
			return new LocalizedString("KpkAccessProblem", "ExF59538", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				propertyName
			});
		}

		public static LocalizedString GroupNamingPolicyCompany
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCompany", "ExCA7DAE", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IndustryMining
		{
			get
			{
				return new LocalizedString("IndustryMining", "Ex7F4D0B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ApiDoesNotSupportInputFormatError(string cl, string member, string input)
		{
			return new LocalizedString("ApiDoesNotSupportInputFormatError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				cl,
				member,
				input
			});
		}

		public static LocalizedString ServerRoleOSP
		{
			get
			{
				return new LocalizedString("ServerRoleOSP", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DomainAlreadyExistsInMserv(string domainName, int existingPartnerId, int localSitePartnerId)
		{
			return new LocalizedString("DomainAlreadyExistsInMserv", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				domainName,
				existingPartnerId,
				localSitePartnerId
			});
		}

		public static LocalizedString InvalidDirectoryConfiguration
		{
			get
			{
				return new LocalizedString("InvalidDirectoryConfiguration", "Ex6FE768", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDDLReferral
		{
			get
			{
				return new LocalizedString("ErrorDDLReferral", "ExFBD693", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorNoAttributeValue
		{
			get
			{
				return new LocalizedString("LdapFilterErrorNoAttributeValue", "ExFEA394", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalEnrollment
		{
			get
			{
				return new LocalizedString("ExternalEnrollment", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTimeoutReadingSystemAddressListCache
		{
			get
			{
				return new LocalizedString("ErrorTimeoutReadingSystemAddressListCache", "Ex65877A", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CanRunDefaultUpdateState_NotSuspended
		{
			get
			{
				return new LocalizedString("CanRunDefaultUpdateState_NotSuspended", "ExDE60CF", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PreferredInternetCodePageSio2022Jp
		{
			get
			{
				return new LocalizedString("PreferredInternetCodePageSio2022Jp", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotResolveTenantName(string name)
		{
			return new LocalizedString("CannotResolveTenantName", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString HtmlAndTextAlternative
		{
			get
			{
				return new LocalizedString("HtmlAndTextAlternative", "Ex74CC7A", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GlobalAddressList
		{
			get
			{
				return new LocalizedString("GlobalAddressList", "Ex29C85E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidPrivateCertificate(string thumbprint)
		{
			return new LocalizedString("ErrorInvalidPrivateCertificate", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				thumbprint
			});
		}

		public static LocalizedString MailTipsAccessLevelNone
		{
			get
			{
				return new LocalizedString("MailTipsAccessLevelNone", "Ex41AD69", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionSearchRootNotChildOfDefaultScope(string child, string scope)
		{
			return new LocalizedString("ExceptionSearchRootNotChildOfDefaultScope", "Ex5C4FAD", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				child,
				scope
			});
		}

		public static LocalizedString EsnLangGalician
		{
			get
			{
				return new LocalizedString("EsnLangGalician", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorLegacyVersionOfflineAddressBookWithoutPublicFolderDatabase(string name)
		{
			return new LocalizedString("ErrorLegacyVersionOfflineAddressBookWithoutPublicFolderDatabase", "Ex7756D3", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString DefaultDatabaseAvailabilityGroupContainerNotFoundException(string agName)
		{
			return new LocalizedString("DefaultDatabaseAvailabilityGroupContainerNotFoundException", "Ex48CEEB", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				agName
			});
		}

		public static LocalizedString ErrorSettingOverrideInvalidSectionName(string componentName, string sectionName, string availableObjects)
		{
			return new LocalizedString("ErrorSettingOverrideInvalidSectionName", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				componentName,
				sectionName,
				availableObjects
			});
		}

		public static LocalizedString ServerRoleFrontendTransport
		{
			get
			{
				return new LocalizedString("ServerRoleFrontendTransport", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Exchange2009
		{
			get
			{
				return new LocalizedString("Exchange2009", "Ex1B4BBA", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TransientMservErrorDescription
		{
			get
			{
				return new LocalizedString("TransientMservErrorDescription", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSubnetMaskGreaterThanAddress(int maskBits, string address)
		{
			return new LocalizedString("ErrorSubnetMaskGreaterThanAddress", "ExB8D1BD", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				maskBits,
				address
			});
		}

		public static LocalizedString ReceiveAuthMechanismExchangeServer
		{
			get
			{
				return new LocalizedString("ReceiveAuthMechanismExchangeServer", "Ex27D615", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Watsons
		{
			get
			{
				return new LocalizedString("Watsons", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OrganizationCapabilityPstProvider
		{
			get
			{
				return new LocalizedString("OrganizationCapabilityPstProvider", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidServiceInstanceIdException(string serviceInstanceId)
		{
			return new LocalizedString("InvalidServiceInstanceIdException", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				serviceInstanceId
			});
		}

		public static LocalizedString ErrorCapabilityNone
		{
			get
			{
				return new LocalizedString("ErrorCapabilityNone", "Ex522BFE", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionAllDomainControllersUnavailable
		{
			get
			{
				return new LocalizedString("ExceptionAllDomainControllersUnavailable", "Ex6D4173", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EndpointContainerNotFoundException(string endpointName)
		{
			return new LocalizedString("EndpointContainerNotFoundException", "ExE3CF8F", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				endpointName
			});
		}

		public static LocalizedString ErrorIsServerSuitableRODC(string dcName)
		{
			return new LocalizedString("ErrorIsServerSuitableRODC", "ExDF5A44", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				dcName
			});
		}

		public static LocalizedString ServersContainerNotFoundException
		{
			get
			{
				return new LocalizedString("ServersContainerNotFoundException", "Ex5CC4E4", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxMoveStatusCompletionInProgress
		{
			get
			{
				return new LocalizedString("MailboxMoveStatusCompletionInProgress", "ExAEA738", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleMailbox
		{
			get
			{
				return new LocalizedString("ServerRoleMailbox", "Ex333797", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorResourceTypeMissing
		{
			get
			{
				return new LocalizedString("ErrorResourceTypeMissing", "Ex2E99DA", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TransientMservError(string message)
		{
			return new LocalizedString("TransientMservError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString Contacts
		{
			get
			{
				return new LocalizedString("Contacts", "ExE0B278", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SendAuthMechanismTls
		{
			get
			{
				return new LocalizedString("SendAuthMechanismTls", "Ex200B5B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionResourceUnhealthy(ResourceKey key)
		{
			return new LocalizedString("ExceptionResourceUnhealthy", "ExCE9BDB", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				key
			});
		}

		public static LocalizedString AggregatedSessionCannotMakeMbxChanges
		{
			get
			{
				return new LocalizedString("AggregatedSessionCannotMakeMbxChanges", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PAAEnabled
		{
			get
			{
				return new LocalizedString("PAAEnabled", "Ex778C25", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NonPartner
		{
			get
			{
				return new LocalizedString("NonPartner", "ExEA93E7", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BasicAfterTLSWithoutBasic
		{
			get
			{
				return new LocalizedString("BasicAfterTLSWithoutBasic", "Ex134A43", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSharedConfigurationBothRoles
		{
			get
			{
				return new LocalizedString("ErrorSharedConfigurationBothRoles", "Ex4E9CD0", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangDutch
		{
			get
			{
				return new LocalizedString("EsnLangDutch", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotGetComputerName(string error)
		{
			return new LocalizedString("CannotGetComputerName", "Ex036C6E", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString ProviderBadImpageFormatLoadException(string providerName, string assemblyPath)
		{
			return new LocalizedString("ProviderBadImpageFormatLoadException", "ExC1314A", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				providerName,
				assemblyPath
			});
		}

		public static LocalizedString ConfigurationSettingsOrganizationNotFound(string id)
		{
			return new LocalizedString("ConfigurationSettingsOrganizationNotFound", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString CannotFindTenantByMSAUserNetID(string msaUserNetID)
		{
			return new LocalizedString("CannotFindTenantByMSAUserNetID", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				msaUserNetID
			});
		}

		public static LocalizedString DsnLanguageNotSupportedForCustomization
		{
			get
			{
				return new LocalizedString("DsnLanguageNotSupportedForCustomization", "ExA50363", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecordValueFormatChange(string key, string oldValue)
		{
			return new LocalizedString("RecordValueFormatChange", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				key,
				oldValue
			});
		}

		public static LocalizedString IndustryNotSpecified
		{
			get
			{
				return new LocalizedString("IndustryNotSpecified", "Ex735023", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDDLFilterError
		{
			get
			{
				return new LocalizedString("ErrorDDLFilterError", "Ex1451DD", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddressList
		{
			get
			{
				return new LocalizedString("AddressList", "Ex2492E9", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LitigationHold_License_Violation(string objectName, string cmdLet, string parameters, string capabilities)
		{
			return new LocalizedString("LitigationHold_License_Violation", "Ex163984", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				objectName,
				cmdLet,
				parameters,
				capabilities
			});
		}

		public static LocalizedString ErrorInvalidISOCountryCode(int countrycode)
		{
			return new LocalizedString("ErrorInvalidISOCountryCode", "Ex0FBE7B", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				countrycode
			});
		}

		public static LocalizedString MustDisplayComment
		{
			get
			{
				return new LocalizedString("MustDisplayComment", "Ex4D13AB", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleFfoWebServices
		{
			get
			{
				return new LocalizedString("ServerRoleFfoWebServices", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleClientAccess
		{
			get
			{
				return new LocalizedString("ServerRoleClientAccess", "Ex8578F6", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorServiceEndpointNotFound(string serviceEndpointName)
		{
			return new LocalizedString("ErrorServiceEndpointNotFound", "ExC90781", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				serviceEndpointName
			});
		}

		public static LocalizedString SKUCapabilityBPOSSEnterprise
		{
			get
			{
				return new LocalizedString("SKUCapabilityBPOSSEnterprise", "Ex95491D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidReceiveAuthModeExternalOnly
		{
			get
			{
				return new LocalizedString("InvalidReceiveAuthModeExternalOnly", "ExC88E24", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSettingOverrideNull
		{
			get
			{
				return new LocalizedString("ErrorSettingOverrideNull", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorLinkedADObjectNotInSameOrganization(string propertyName, string propertyValue, string objectId, string orgId)
		{
			return new LocalizedString("ErrorLinkedADObjectNotInSameOrganization", "Ex4BCBE7", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				propertyName,
				propertyValue,
				objectId,
				orgId
			});
		}

		public static LocalizedString LdapFilterErrorQueryTooLong
		{
			get
			{
				return new LocalizedString("LdapFilterErrorQueryTooLong", "ExB5260B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionProxyGeneratorDLLFailed(string server)
		{
			return new LocalizedString("ExceptionProxyGeneratorDLLFailed", "ExCF6D75", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString BPOS_License_NumericLimitViolation(string objectName, string cmdLet, string parameters, string capabilities)
		{
			return new LocalizedString("BPOS_License_NumericLimitViolation", "Ex4B5FC3", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				objectName,
				cmdLet,
				parameters,
				capabilities
			});
		}

		public static LocalizedString SessionSubscriptionDisabled(Guid guid)
		{
			return new LocalizedString("SessionSubscriptionDisabled", "Ex2F34FE", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				guid
			});
		}

		public static LocalizedString ErrorMoveToDestinationFolderNotDefined
		{
			get
			{
				return new LocalizedString("ErrorMoveToDestinationFolderNotDefined", "Ex89E555", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidControlTextLength(int maxLength)
		{
			return new LocalizedString("InvalidControlTextLength", "ExDD8CAA", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				maxLength
			});
		}

		public static LocalizedString RelocationInProgress(string tenantName, string permError, string suspened, string autoCompletion, string currentState, string requestedState)
		{
			return new LocalizedString("RelocationInProgress", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				tenantName,
				permError,
				suspened,
				autoCompletion,
				currentState,
				requestedState
			});
		}

		public static LocalizedString MailboxMoveStatusInProgress
		{
			get
			{
				return new LocalizedString("MailboxMoveStatusInProgress", "Ex9285EC", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SecurityPrincipalTypeGroup
		{
			get
			{
				return new LocalizedString("SecurityPrincipalTypeGroup", "ExE9ADC8", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString X400Authoritative
		{
			get
			{
				return new LocalizedString("X400Authoritative", "Ex090621", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailFlowPartnerInternalMailContentTypeMimeHtml
		{
			get
			{
				return new LocalizedString("MailFlowPartnerInternalMailContentTypeMimeHtml", "Ex39B525", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailEnabledUserRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("MailEnabledUserRecipientTypeDetails", "ExC65CDF", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionFailedToRebuildConnection(string serverName)
		{
			return new LocalizedString("ExceptionFailedToRebuildConnection", "Ex6ED5CE", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString RootCannotBeEmpty(ScopeRestrictionType scopeType)
		{
			return new LocalizedString("RootCannotBeEmpty", "ExD5FE2F", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				scopeType
			});
		}

		public static LocalizedString ExtensionNull
		{
			get
			{
				return new LocalizedString("ExtensionNull", "Ex5D1856", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TenantIsArrivingException(string dn)
		{
			return new LocalizedString("TenantIsArrivingException", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				dn
			});
		}

		public static LocalizedString TenantPerimeterConfigSettingsNotFoundException(string ordId)
		{
			return new LocalizedString("TenantPerimeterConfigSettingsNotFoundException", "Ex25E7EF", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				ordId
			});
		}

		public static LocalizedString Unsecured
		{
			get
			{
				return new LocalizedString("Unsecured", "ExB84289", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectorIdIsNotAnInteger
		{
			get
			{
				return new LocalizedString("ConnectorIdIsNotAnInteger", "Ex2FB9D0", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMissingPrimaryUM
		{
			get
			{
				return new LocalizedString("ErrorMissingPrimaryUM", "Ex7681E6", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotDetermineDataSessionType
		{
			get
			{
				return new LocalizedString("CannotDetermineDataSessionType", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfigScopeCannotBeEmpty(ConfigWriteScopeType scopeType)
		{
			return new LocalizedString("ConfigScopeCannotBeEmpty", "ExE74C4B", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				scopeType
			});
		}

		public static LocalizedString UserAgentsChanges
		{
			get
			{
				return new LocalizedString("UserAgentsChanges", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDNFormat(string str)
		{
			return new LocalizedString("InvalidDNFormat", "Ex786089", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				str
			});
		}

		public static LocalizedString Notes
		{
			get
			{
				return new LocalizedString("Notes", "Ex96E8FC", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangTelugu
		{
			get
			{
				return new LocalizedString("EsnLangTelugu", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTransitionCounterHasDuplicateEntry(string transitiontype)
		{
			return new LocalizedString("ErrorTransitionCounterHasDuplicateEntry", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				transitiontype
			});
		}

		public static LocalizedString ExceptionADOperationFailedRemoveContainer(string server, string dn)
		{
			return new LocalizedString("ExceptionADOperationFailedRemoveContainer", "ExB26DF4", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				server,
				dn
			});
		}

		public static LocalizedString GroupNamingPolicyExtensionCustomAttribute1
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyExtensionCustomAttribute1", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailFlowPartnerInternalMailContentTypeNone
		{
			get
			{
				return new LocalizedString("MailFlowPartnerInternalMailContentTypeNone", "Ex63CA8D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecipientWriteScopeNotLessThan(string leftScopeType, string rightScopeType)
		{
			return new LocalizedString("RecipientWriteScopeNotLessThan", "Ex3755BA", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				leftScopeType,
				rightScopeType
			});
		}

		public static LocalizedString DefaultRapName
		{
			get
			{
				return new LocalizedString("DefaultRapName", "Ex3B46EB", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeleteUseDefaultAlert
		{
			get
			{
				return new LocalizedString("DeleteUseDefaultAlert", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotResolveTenantNameByAcceptedDomain(string domain)
		{
			return new LocalizedString("CannotResolveTenantNameByAcceptedDomain", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString ErrorOrganizationResourceAddressListsCount
		{
			get
			{
				return new LocalizedString("ErrorOrganizationResourceAddressListsCount", "ExB6E71B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionSearchRootChildDomain(string childDomain, string scopeDomain)
		{
			return new LocalizedString("ExceptionSearchRootChildDomain", "ExA95B78", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				childDomain,
				scopeDomain
			});
		}

		public static LocalizedString EsnLangChineseSimplified
		{
			get
			{
				return new LocalizedString("EsnLangChineseSimplified", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConferenceRoomMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("ConferenceRoomMailboxRecipientTypeDetails", "Ex6F3AB9", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRemovedMailboxDoesNotHaveDatabase(string id)
		{
			return new LocalizedString("ErrorRemovedMailboxDoesNotHaveDatabase", "Ex22BDC8", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString BlockedOutlookClientVersionPatternDescription
		{
			get
			{
				return new LocalizedString("BlockedOutlookClientVersionPatternDescription", "ExCCE8F7", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserHasNoSmtpProxyAddressWithFederatedDomain
		{
			get
			{
				return new LocalizedString("UserHasNoSmtpProxyAddressWithFederatedDomain", "Ex9D7C34", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OrganizationCapabilityMailRouting
		{
			get
			{
				return new LocalizedString("OrganizationCapabilityMailRouting", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SKUCapabilityBPOSSStandard
		{
			get
			{
				return new LocalizedString("SKUCapabilityBPOSSStandard", "ExAE0C69", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SystemMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("SystemMailboxRecipientTypeDetails", "ExBBEBE9", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionADTopologyNoLocalDomain
		{
			get
			{
				return new LocalizedString("ExceptionADTopologyNoLocalDomain", "Ex2D2F44", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlternateServiceAccountCredentialDisplayFormat(string isInvalid, DateTime timeStamp, string domain, string userName)
		{
			return new LocalizedString("AlternateServiceAccountCredentialDisplayFormat", "Ex494D05", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				isInvalid,
				timeStamp,
				domain,
				userName
			});
		}

		public static LocalizedString ErrorCannotSaveBecauseTooNew(ExchangeObjectVersion objectVersion, ExchangeObjectVersion currentVersion)
		{
			return new LocalizedString("ErrorCannotSaveBecauseTooNew", "Ex3E755D", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				objectVersion,
				currentVersion
			});
		}

		public static LocalizedString InvalidRootDse(string server)
		{
			return new LocalizedString("InvalidRootDse", "Ex8737DC", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString ExceptionCannotAddSidHistory(string srcObj, string srcDom, string dstObj, string dstDom, string errorCode)
		{
			return new LocalizedString("ExceptionCannotAddSidHistory", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				srcObj,
				srcDom,
				dstObj,
				dstDom,
				errorCode
			});
		}

		public static LocalizedString ErrorInvalidServerFqdn(string fqdn, string hostName)
		{
			return new LocalizedString("ErrorInvalidServerFqdn", "Ex0E7BD0", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				fqdn,
				hostName
			});
		}

		public static LocalizedString ExceptionSearchRootNotChildOfSessionSearchRoot(string child, string scope)
		{
			return new LocalizedString("ExceptionSearchRootNotChildOfSessionSearchRoot", "Ex6EC18C", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				child,
				scope
			});
		}

		public static LocalizedString EsnLangDanish
		{
			get
			{
				return new LocalizedString("EsnLangDanish", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRemovedMailboxDoesNotHaveMailboxGuid(string id)
		{
			return new LocalizedString("ErrorRemovedMailboxDoesNotHaveMailboxGuid", "ExBCF5E3", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString IndustryRetail
		{
			get
			{
				return new LocalizedString("IndustryRetail", "ExF465FC", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDDLNoSuchObject
		{
			get
			{
				return new LocalizedString("ErrorDDLNoSuchObject", "Ex232B18", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IndustryComputerRelatedProductsServices
		{
			get
			{
				return new LocalizedString("IndustryComputerRelatedProductsServices", "ExA4D963", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InternalRelay
		{
			get
			{
				return new LocalizedString("InternalRelay", "Ex7DAA91", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorEmptyArchiveName
		{
			get
			{
				return new LocalizedString("ErrorEmptyArchiveName", "ExDE90E9", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmailAddressPolicyPriorityLowest
		{
			get
			{
				return new LocalizedString("EmailAddressPolicyPriorityLowest", "Ex4D5247", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalMdm
		{
			get
			{
				return new LocalizedString("ExternalMdm", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TransportSettingsNotFoundException
		{
			get
			{
				return new LocalizedString("TransportSettingsNotFoundException", "ExD5A88D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DomainSecureEnabledWithoutTls
		{
			get
			{
				return new LocalizedString("DomainSecureEnabledWithoutTls", "ExFAC5F7", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BccSuspiciousOutboundAdditionalRecipientsRequired
		{
			get
			{
				return new LocalizedString("BccSuspiciousOutboundAdditionalRecipientsRequired", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoRoleEntriesFound
		{
			get
			{
				return new LocalizedString("NoRoleEntriesFound", "Ex200BA5", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IndustryWholesale
		{
			get
			{
				return new LocalizedString("IndustryWholesale", "ExEAA6B6", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionADTopologyServiceDown(string server, string serviceType, string error)
		{
			return new LocalizedString("ExceptionADTopologyServiceDown", "Ex33BDA3", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				server,
				serviceType,
				error
			});
		}

		public static LocalizedString CannotCalculatePropertyGeneric(string calculatedPropertyName)
		{
			return new LocalizedString("CannotCalculatePropertyGeneric", "Ex76E5F2", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				calculatedPropertyName
			});
		}

		public static LocalizedString ServerRoleCentralAdminFrontEnd
		{
			get
			{
				return new LocalizedString("ServerRoleCentralAdminFrontEnd", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionADConstraintViolation(string server, string errorMessage)
		{
			return new LocalizedString("ExceptionADConstraintViolation", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				server,
				errorMessage
			});
		}

		public static LocalizedString ADTreeDeleteNotFinishedException(string server)
		{
			return new LocalizedString("ADTreeDeleteNotFinishedException", "ExE3A815", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString ErrorInvalidPushNotificationPlatform
		{
			get
			{
				return new LocalizedString("ErrorInvalidPushNotificationPlatform", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAAConfiguration(string a, string b)
		{
			return new LocalizedString("InvalidAAConfiguration", "Ex7FDD1A", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				a,
				b
			});
		}

		public static LocalizedString MailTipsAccessLevelAll
		{
			get
			{
				return new LocalizedString("MailTipsAccessLevelAll", "ExF7A6C8", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PermanentGlsError(string message)
		{
			return new LocalizedString("PermanentGlsError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString BPOS_S_Property_License_Violation(string objectName, string cmdLet, string parameters, string capabilities)
		{
			return new LocalizedString("BPOS_S_Property_License_Violation", "Ex6F303E", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				objectName,
				cmdLet,
				parameters,
				capabilities
			});
		}

		public static LocalizedString PublicFolderRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("PublicFolderRecipientTypeDetails", "Ex7D796A", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValueNotAvailableForUnchangedProperty
		{
			get
			{
				return new LocalizedString("ValueNotAvailableForUnchangedProperty", "Ex50AAAB", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DumpsterFolder
		{
			get
			{
				return new LocalizedString("DumpsterFolder", "ExC4D9A5", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotParseMimeTypes
		{
			get
			{
				return new LocalizedString("CannotParseMimeTypes", "ExC27AD8", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotResolveTenantNameByExternalDirectoryId(string id)
		{
			return new LocalizedString("CannotResolveTenantNameByExternalDirectoryId", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString BEVDirLockdown_Violation(string objectName, string cmdLet, string parameters, string capabilities)
		{
			return new LocalizedString("BEVDirLockdown_Violation", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				objectName,
				cmdLet,
				parameters,
				capabilities
			});
		}

		public static LocalizedString ExclusiveRecipientScopes
		{
			get
			{
				return new LocalizedString("ExclusiveRecipientScopes", "Ex4E0C7D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorProperty1EqProperty2(string property1Name, string property2Name, string propertyValue)
		{
			return new LocalizedString("ErrorProperty1EqProperty2", "Ex1CF833", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				property1Name,
				property2Name,
				propertyValue
			});
		}

		public static LocalizedString QuarantineMailboxIsInvalid
		{
			get
			{
				return new LocalizedString("QuarantineMailboxIsInvalid", "ExE43D96", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNonUniqueSid(string sidString)
		{
			return new LocalizedString("ErrorNonUniqueSid", "Ex1BE998", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				sidString
			});
		}

		public static LocalizedString ExceptionADTopologyErrorWhenLookingForGlobalCatalogsInForest(int error, string forest, string message)
		{
			return new LocalizedString("ExceptionADTopologyErrorWhenLookingForGlobalCatalogsInForest", "ExD96E4B", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				error,
				forest,
				message
			});
		}

		public static LocalizedString ExceptionObjectPartitionDoesNotMatchSessionPartition(string dn, string fqdn)
		{
			return new LocalizedString("ExceptionObjectPartitionDoesNotMatchSessionPartition", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				dn,
				fqdn
			});
		}

		public static LocalizedString ExceptionADTopologyHasNoServersInForest(string forest)
		{
			return new LocalizedString("ExceptionADTopologyHasNoServersInForest", "Ex075281", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				forest
			});
		}

		public static LocalizedString MailboxPlanTypeDetails
		{
			get
			{
				return new LocalizedString("MailboxPlanTypeDetails", "ExFF6044", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WACDiscoveryEndpointShouldBeAbsoluteUri(string actualValue)
		{
			return new LocalizedString("WACDiscoveryEndpointShouldBeAbsoluteUri", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				actualValue
			});
		}

		public static LocalizedString ServerRoleCafeArray
		{
			get
			{
				return new LocalizedString("ServerRoleCafeArray", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SendCredentialIsNull
		{
			get
			{
				return new LocalizedString("SendCredentialIsNull", "Ex5F3D40", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MasteredOnPremiseCapabilityUndefinedTenantNotDirSyncing(string capability, string property)
		{
			return new LocalizedString("MasteredOnPremiseCapabilityUndefinedTenantNotDirSyncing", "ExC70267", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				capability,
				property
			});
		}

		public static LocalizedString True
		{
			get
			{
				return new LocalizedString("True", "ExBC8412", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotGetDnFromGuid(Guid guid)
		{
			return new LocalizedString("CannotGetDnFromGuid", "Ex2A9BDE", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				guid
			});
		}

		public static LocalizedString ExceptionObjectHasBeenDeletedDuringCurrentOperation(string id)
		{
			return new LocalizedString("ExceptionObjectHasBeenDeletedDuringCurrentOperation", "ExE73C2C", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString StarAcceptedDomainCannotBeAuthoritative
		{
			get
			{
				return new LocalizedString("StarAcceptedDomainCannotBeAuthoritative", "Ex2BF988", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalculatedPropertyFailed(string propertyName, string basePropertyName, string errorMessage)
		{
			return new LocalizedString("CalculatedPropertyFailed", "ExCC6DFF", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				propertyName,
				basePropertyName,
				errorMessage
			});
		}

		public static LocalizedString ApiNotSupportedInBusinessSessionError(string cl, string member)
		{
			return new LocalizedString("ApiNotSupportedInBusinessSessionError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				cl,
				member
			});
		}

		public static LocalizedString AllRooms
		{
			get
			{
				return new LocalizedString("AllRooms", "Ex70BA64", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RusInvalidFilter(string error)
		{
			return new LocalizedString("RusInvalidFilter", "Ex5B04B5", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString EsnLangRussian
		{
			get
			{
				return new LocalizedString("EsnLangRussian", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoSuitableDCInDomain(string domainName, string errorMessage)
		{
			return new LocalizedString("ErrorNoSuitableDCInDomain", "Ex50BF2A", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				domainName,
				errorMessage
			});
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute10
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute10", "Ex05D681", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SitesContainerNotFound
		{
			get
			{
				return new LocalizedString("SitesContainerNotFound", "ExE31758", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionServerTimeoutNegative
		{
			get
			{
				return new LocalizedString("ExceptionServerTimeoutNegative", "Ex088C34", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ArchiveStateLocal
		{
			get
			{
				return new LocalizedString("ArchiveStateLocal", "Ex148892", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotesMC
		{
			get
			{
				return new LocalizedString("NotesMC", "Ex667DA1", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SuitabilityErrorDNS(string fqdn, string details)
		{
			return new LocalizedString("SuitabilityErrorDNS", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				fqdn,
				details
			});
		}

		public static LocalizedString ExceptionGetLocalSiteArgumentException(string siteName)
		{
			return new LocalizedString("ExceptionGetLocalSiteArgumentException", "Ex95D783", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				siteName
			});
		}

		public static LocalizedString InvalidDomain
		{
			get
			{
				return new LocalizedString("InvalidDomain", "Ex9A29DE", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSyncLinkFormat(string link)
		{
			return new LocalizedString("InvalidSyncLinkFormat", "Ex51D6DD", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				link
			});
		}

		public static LocalizedString EmailAgeFilterOneMonth
		{
			get
			{
				return new LocalizedString("EmailAgeFilterOneMonth", "Ex288AD3", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDuplicatePartnerApplicationId(string applicationId)
		{
			return new LocalizedString("ErrorDuplicatePartnerApplicationId", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				applicationId
			});
		}

		public static LocalizedString FullDomain
		{
			get
			{
				return new LocalizedString("FullDomain", "Ex106EA7", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAccountPartitionCantBeLocalAndHaveTrustAtTheSameTime(string id)
		{
			return new LocalizedString("ErrorAccountPartitionCantBeLocalAndHaveTrustAtTheSameTime", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ErrorADResponse(string message)
		{
			return new LocalizedString("ErrorADResponse", "Ex84338A", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString DeviceModel
		{
			get
			{
				return new LocalizedString("DeviceModel", "Ex3359D0", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupRecipientType
		{
			get
			{
				return new LocalizedString("GroupRecipientType", "ExDB67AE", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteSharedMailboxTypeDetails
		{
			get
			{
				return new LocalizedString("RemoteSharedMailboxTypeDetails", "ExB74764", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapSearch
		{
			get
			{
				return new LocalizedString("LdapSearch", "Ex0C4B66", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory(string property)
		{
			return new LocalizedString("ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory", "Ex9307C4", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString EsnLangArabic
		{
			get
			{
				return new LocalizedString("EsnLangArabic", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SKUCapabilityBPOSSDeskless
		{
			get
			{
				return new LocalizedString("SKUCapabilityBPOSSDeskless", "Ex719ADB", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ModeratedRecipients
		{
			get
			{
				return new LocalizedString("ModeratedRecipients", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionRusOperationFailed
		{
			get
			{
				return new LocalizedString("ExceptionRusOperationFailed", "ExEB0DEE", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionDomainInfoRpcTooBusy
		{
			get
			{
				return new LocalizedString("ExceptionDomainInfoRpcTooBusy", "ExC400D9", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorArchiveDomainInvalidInDatacenter
		{
			get
			{
				return new LocalizedString("ErrorArchiveDomainInvalidInDatacenter", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnrecognizedRoleEntryType(string entry)
		{
			return new LocalizedString("UnrecognizedRoleEntryType", "Ex7B3ABD", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				entry
			});
		}

		public static LocalizedString PublicFolderRecipientType
		{
			get
			{
				return new LocalizedString("PublicFolderRecipientType", "ExC9284D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMessageClassHasUnsupportedWildcard
		{
			get
			{
				return new LocalizedString("ErrorMessageClassHasUnsupportedWildcard", "Ex6405EF", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPipelineTracingRequirementsMissing
		{
			get
			{
				return new LocalizedString("ErrorPipelineTracingRequirementsMissing", "Ex906171", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute11
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute11", "Ex7638F4", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMailTipMustNotBeEmpty
		{
			get
			{
				return new LocalizedString("ErrorMailTipMustNotBeEmpty", "ExC78163", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ComputerRecipientType
		{
			get
			{
				return new LocalizedString("ComputerRecipientType", "Ex060F59", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorArbitrationMailboxCannotBeModerated
		{
			get
			{
				return new LocalizedString("ErrorArbitrationMailboxCannotBeModerated", "Ex387876", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangKannada
		{
			get
			{
				return new LocalizedString("EsnLangKannada", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title
		{
			get
			{
				return new LocalizedString("Title", "Ex40E83A", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToUpdateEmailAddressesForExternal(string external, string propEmailAddresses, string reason)
		{
			return new LocalizedString("FailedToUpdateEmailAddressesForExternal", "ExCBEA5F", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				external,
				propEmailAddresses,
				reason
			});
		}

		public static LocalizedString MessageWaitingIndicatorEnabled
		{
			get
			{
				return new LocalizedString("MessageWaitingIndicatorEnabled", "Ex5E7DA9", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFolders
		{
			get
			{
				return new LocalizedString("PublicFolders", "Ex7CC436", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Millisecond
		{
			get
			{
				return new LocalizedString("Millisecond", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StarAcceptedDomainCannotBeDefault
		{
			get
			{
				return new LocalizedString("StarAcceptedDomainCannotBeDefault", "ExC3D5D5", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReceiveExtendedProtectionPolicyAllow
		{
			get
			{
				return new LocalizedString("ReceiveExtendedProtectionPolicyAllow", "Ex9A250A", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ResourceMailbox
		{
			get
			{
				return new LocalizedString("ResourceMailbox", "Ex81C4FC", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorThrottlingPolicyStateIsCorrupt
		{
			get
			{
				return new LocalizedString("ErrorThrottlingPolicyStateIsCorrupt", "Ex7894C5", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailEnabledNonUniversalGroupRecipientType
		{
			get
			{
				return new LocalizedString("MailEnabledNonUniversalGroupRecipientType", "ExB5904C", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionErrorFromRUS(string server, int error)
		{
			return new LocalizedString("ExceptionErrorFromRUS", "Ex5790AD", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				server,
				error
			});
		}

		public static LocalizedString ExternalAuthoritativeWithoutExchangeServerPermission
		{
			get
			{
				return new LocalizedString("ExternalAuthoritativeWithoutExchangeServerPermission", "Ex2F3871", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Authoritative
		{
			get
			{
				return new LocalizedString("Authoritative", "Ex364E82", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPrimarySmtpAddressAndWindowsEmailAddressNotMatch
		{
			get
			{
				return new LocalizedString("ErrorPrimarySmtpAddressAndWindowsEmailAddressNotMatch", "Ex28C9AC", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PostMC
		{
			get
			{
				return new LocalizedString("PostMC", "Ex2B153A", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnknownConfigObject
		{
			get
			{
				return new LocalizedString("UnknownConfigObject", "Ex69DC5E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSubnetMaskOutOfRange(int maskBits, string address, int min, int max)
		{
			return new LocalizedString("ErrorSubnetMaskOutOfRange", "Ex3F1C79", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				maskBits,
				address,
				min,
				max
			});
		}

		public static LocalizedString MalwareScanErrorActionAllow
		{
			get
			{
				return new LocalizedString("MalwareScanErrorActionAllow", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute6
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute6", "ExC01F0E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NonUniquePilotIdentifier(string pilotId, string dialPlan)
		{
			return new LocalizedString("NonUniquePilotIdentifier", "Ex60A5D2", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				pilotId,
				dialPlan
			});
		}

		public static LocalizedString ErrorThresholdMustBeSet(string name)
		{
			return new LocalizedString("ErrorThresholdMustBeSet", "ExC25A5B", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ErrorProperty1NeValue1WhileProperty2EqValue2(string property1Name, string value1, string property2Name, string value2)
		{
			return new LocalizedString("ErrorProperty1NeValue1WhileProperty2EqValue2", "ExA09669", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				property1Name,
				value1,
				property2Name,
				value2
			});
		}

		public static LocalizedString InvalidTransportSyncLogSizeConfiguration
		{
			get
			{
				return new LocalizedString("InvalidTransportSyncLogSizeConfiguration", "Ex66671C", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WellKnownRecipientTypeMailGroups
		{
			get
			{
				return new LocalizedString("WellKnownRecipientTypeMailGroups", "ExCCA960", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADDriverStoreAccessTransientError
		{
			get
			{
				return new LocalizedString("ADDriverStoreAccessTransientError", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAutoAttendantSetting(string value, string argument)
		{
			return new LocalizedString("InvalidAutoAttendantSetting", "Ex7718A0", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				value,
				argument
			});
		}

		public static LocalizedString AACantChangeName
		{
			get
			{
				return new LocalizedString("AACantChangeName", "Ex7EC7CE", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContactItemsMC
		{
			get
			{
				return new LocalizedString("ContactItemsMC", "ExB884C8", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangKorean
		{
			get
			{
				return new LocalizedString("EsnLangKorean", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidFilterSize(int size)
		{
			return new LocalizedString("InvalidFilterSize", "ExE186A6", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				size
			});
		}

		public static LocalizedString RssSubscriptionMC
		{
			get
			{
				return new LocalizedString("RssSubscriptionMC", "ExF86F02", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerComponentReadADError(string adErrorStr)
		{
			return new LocalizedString("ServerComponentReadADError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				adErrorStr
			});
		}

		public static LocalizedString ErrorLogFolderPathEqualsCopyLogFolderPath(string path)
		{
			return new LocalizedString("ErrorLogFolderPathEqualsCopyLogFolderPath", "ExF9578F", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString LdapFilterErrorSpaceMiddleType
		{
			get
			{
				return new LocalizedString("LdapFilterErrorSpaceMiddleType", "Ex8866E5", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute3
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute3", "Ex20271C", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorArchiveMailboxExists(string guid)
		{
			return new LocalizedString("ErrorArchiveMailboxExists", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				guid
			});
		}

		public static LocalizedString InvalidCrossTenantIdFormat(string str)
		{
			return new LocalizedString("InvalidCrossTenantIdFormat", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				str
			});
		}

		public static LocalizedString ExceptionNoFsmoRoleOwnerAttribute
		{
			get
			{
				return new LocalizedString("ExceptionNoFsmoRoleOwnerAttribute", "ExE7E68E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NonIpmRoot
		{
			get
			{
				return new LocalizedString("NonIpmRoot", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTimeoutWritingSystemAddressListMemberCount
		{
			get
			{
				return new LocalizedString("ErrorTimeoutWritingSystemAddressListMemberCount", "ExD84E2E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomRecipientWriteScopeCannotBeEmpty(RecipientWriteScopeType scopeType)
		{
			return new LocalizedString("CustomRecipientWriteScopeCannotBeEmpty", "ExD0F2C8", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				scopeType
			});
		}

		public static LocalizedString ExceptionExternalError
		{
			get
			{
				return new LocalizedString("ExceptionExternalError", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Calendar
		{
			get
			{
				return new LocalizedString("Calendar", "Ex113F91", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotBuildAuthenticationTypeFilterNoNamespacesOfType(string authType)
		{
			return new LocalizedString("CannotBuildAuthenticationTypeFilterNoNamespacesOfType", "ExD98642", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				authType
			});
		}

		public static LocalizedString Wma
		{
			get
			{
				return new LocalizedString("Wma", "ExACDD6A", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSystemFolderPathNotEqualLogFolderPath(NonRootLocalLongFullPath sysPath, NonRootLocalLongFullPath logPath)
		{
			return new LocalizedString("ErrorSystemFolderPathNotEqualLogFolderPath", "Ex9B0573", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				sysPath,
				logPath
			});
		}

		public static LocalizedString ErrorInvalidDNDepth
		{
			get
			{
				return new LocalizedString("ErrorInvalidDNDepth", "Ex9AF71B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CapabilityMasteredOnPremise
		{
			get
			{
				return new LocalizedString("CapabilityMasteredOnPremise", "Ex65AADE", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EdgeSyncEhfConnectorFailedToDecryptPassword
		{
			get
			{
				return new LocalizedString("EdgeSyncEhfConnectorFailedToDecryptPassword", "ExDEB7E0", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LegacyGwartNotFoundException(string gwartName, string adminGroupName)
		{
			return new LocalizedString("LegacyGwartNotFoundException", "ExCA93B2", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				gwartName,
				adminGroupName
			});
		}

		public static LocalizedString ErrorSystemFolderPathEqualsCopySystemFolderPath(string path)
		{
			return new LocalizedString("ErrorSystemFolderPathEqualsCopySystemFolderPath", "Ex71C029", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString ErrorArchiveDomainSetForNonArchive
		{
			get
			{
				return new LocalizedString("ErrorArchiveDomainSetForNonArchive", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionObjectHasBeenDeleted
		{
			get
			{
				return new LocalizedString("ExceptionObjectHasBeenDeleted", "Ex3124CB", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangBengaliIndia
		{
			get
			{
				return new LocalizedString("EsnLangBengaliIndia", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFolderServer
		{
			get
			{
				return new LocalizedString("PublicFolderServer", "Ex0B618C", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotSetPrimarySmtpAddress
		{
			get
			{
				return new LocalizedString("ErrorCannotSetPrimarySmtpAddress", "Ex9727CE", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpamFilteringActionQuarantine
		{
			get
			{
				return new LocalizedString("SpamFilteringActionQuarantine", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionWKGuidNeedsDomainSession(Guid wkguid)
		{
			return new LocalizedString("ExceptionWKGuidNeedsDomainSession", "ExAFE1DE", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				wkguid
			});
		}

		public static LocalizedString MailboxMoveStatusFailed
		{
			get
			{
				return new LocalizedString("MailboxMoveStatusFailed", "ExF26D08", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReportToManagedEnabledWithoutManager(string id, string propertyName)
		{
			return new LocalizedString("ErrorReportToManagedEnabledWithoutManager", "Ex80687E", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				id,
				propertyName
			});
		}

		public static LocalizedString SecurityPrincipalTypeUniversalSecurityGroup
		{
			get
			{
				return new LocalizedString("SecurityPrincipalTypeUniversalSecurityGroup", "Ex61EE5C", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ThrottlingPolicyCorrupted(string policyId)
		{
			return new LocalizedString("ThrottlingPolicyCorrupted", "Ex034692", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				policyId
			});
		}

		public static LocalizedString ExceptionOwaCannotSetPropertyOnE14MailboxPolicyToNull(string property)
		{
			return new LocalizedString("ExceptionOwaCannotSetPropertyOnE14MailboxPolicyToNull", "ExAC78E8", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString EXOStandardRestrictions_Error(string objectName, string cmdLet, string parameters, string capabilities)
		{
			return new LocalizedString("EXOStandardRestrictions_Error", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				objectName,
				cmdLet,
				parameters,
				capabilities
			});
		}

		public static LocalizedString DynamicDLRecipientType
		{
			get
			{
				return new LocalizedString("DynamicDLRecipientType", "Ex37D4DD", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDialPlan(string s)
		{
			return new LocalizedString("InvalidDialPlan", "Ex759A2F", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ErrorNonTinyTenantShouldNotHaveSharedConfig
		{
			get
			{
				return new LocalizedString("ErrorNonTinyTenantShouldNotHaveSharedConfig", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CanRunRestoreState_Allowed
		{
			get
			{
				return new LocalizedString("CanRunRestoreState_Allowed", "Ex6F69F3", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DomainSecureWithIgnoreStartTLSEnabled
		{
			get
			{
				return new LocalizedString("DomainSecureWithIgnoreStartTLSEnabled", "ExBE80FB", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSubnetAddressDoesNotMatchMask(string address, int maskBits, string realAddress)
		{
			return new LocalizedString("ErrorSubnetAddressDoesNotMatchMask", "Ex5B1E6A", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				address,
				maskBits,
				realAddress
			});
		}

		public static LocalizedString GroupNamingPolicyExtensionCustomAttribute4
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyExtensionCustomAttribute4", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UseMsg
		{
			get
			{
				return new LocalizedString("UseMsg", "ExA3C339", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidTenantFullSyncCookieException
		{
			get
			{
				return new LocalizedString("InvalidTenantFullSyncCookieException", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorProperty1GtProperty2(string property1Name, string property1Value, string property2Name, string property2Value)
		{
			return new LocalizedString("ErrorProperty1GtProperty2", "ExFAD17D", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				property1Name,
				property1Value,
				property2Name,
				property2Value
			});
		}

		public static LocalizedString AutoDatabaseMountDialGoodAvailability
		{
			get
			{
				return new LocalizedString("AutoDatabaseMountDialGoodAvailability", "Ex0CF926", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ForestTrust
		{
			get
			{
				return new LocalizedString("ForestTrust", "ExD9F5D9", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidMailboxRelationType
		{
			get
			{
				return new LocalizedString("ErrorInvalidMailboxRelationType", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDDLInvalidDNSyntax
		{
			get
			{
				return new LocalizedString("ErrorDDLInvalidDNSyntax", "Ex778F19", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidNonPositiveResourceThreshold(string classification, string thresholdName, int value)
		{
			return new LocalizedString("InvalidNonPositiveResourceThreshold", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				classification,
				thresholdName,
				value
			});
		}

		public static LocalizedString GlsEndpointNotFound(string endpoint, string message)
		{
			return new LocalizedString("GlsEndpointNotFound", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				endpoint,
				message
			});
		}

		public static LocalizedString InvalidWaveFilename(string s)
		{
			return new LocalizedString("InvalidWaveFilename", "ExE33FFA", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ByteEncoderTypeUseQP
		{
			get
			{
				return new LocalizedString("ByteEncoderTypeUseQP", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotFindTenantCUByExternalDirectoryId(string id)
		{
			return new LocalizedString("CannotFindTenantCUByExternalDirectoryId", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString NoLocatorInformationInMServException
		{
			get
			{
				return new LocalizedString("NoLocatorInformationInMServException", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SecurityPrincipalTypeGlobalSecurityGroup
		{
			get
			{
				return new LocalizedString("SecurityPrincipalTypeGlobalSecurityGroup", "Ex7C0C6F", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSyncCompanyId(string idValue)
		{
			return new LocalizedString("InvalidSyncCompanyId", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				idValue
			});
		}

		public static LocalizedString CustomRecipientWriteScopeMustBeEmpty(RecipientWriteScopeType scopeType)
		{
			return new LocalizedString("CustomRecipientWriteScopeMustBeEmpty", "Ex7486C6", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				scopeType
			});
		}

		public static LocalizedString ErrorReportToBothManagerAndOriginator(string id, string prop1, string prop2)
		{
			return new LocalizedString("ErrorReportToBothManagerAndOriginator", "Ex9A45AC", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				id,
				prop1,
				prop2
			});
		}

		public static LocalizedString PublicFolderReferralServerNotExisting(string serverGuid)
		{
			return new LocalizedString("PublicFolderReferralServerNotExisting", "ExC4DC0B", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				serverGuid
			});
		}

		public static LocalizedString CannotGetUsefulSiteInfo
		{
			get
			{
				return new LocalizedString("CannotGetUsefulSiteInfo", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPipelineTracingPathNotExist
		{
			get
			{
				return new LocalizedString("ErrorPipelineTracingPathNotExist", "ExFFB6AE", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxServer
		{
			get
			{
				return new LocalizedString("MailboxServer", "Ex10681A", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Blocked
		{
			get
			{
				return new LocalizedString("Blocked", "Ex81292C", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidMainStreamCookieException
		{
			get
			{
				return new LocalizedString("InvalidMainStreamCookieException", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MoveNotAllowed
		{
			get
			{
				return new LocalizedString("MoveNotAllowed", "ExA949A6", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteRoomMailboxTypeDetails
		{
			get
			{
				return new LocalizedString("RemoteRoomMailboxTypeDetails", "Ex20D754", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SecurityPrincipalTypeUser
		{
			get
			{
				return new LocalizedString("SecurityPrincipalTypeUser", "ExA27E57", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TextEnrichedOnly
		{
			get
			{
				return new LocalizedString("TextEnrichedOnly", "ExF33ED6", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BluetoothAllow
		{
			get
			{
				return new LocalizedString("BluetoothAllow", "Ex85B727", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyDepartment
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyDepartment", "Ex1E50B4", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UseDefaultSettings
		{
			get
			{
				return new LocalizedString("UseDefaultSettings", "ExDBFB4E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNullRecipientTypeInPrecannedFilter(string includedRecipients)
		{
			return new LocalizedString("ErrorNullRecipientTypeInPrecannedFilter", "ExB4EEEE", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				includedRecipients
			});
		}

		public static LocalizedString ByteEncoderTypeUseQPHtmlDetectTextPlain
		{
			get
			{
				return new LocalizedString("ByteEncoderTypeUseQPHtmlDetectTextPlain", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Exchange2007
		{
			get
			{
				return new LocalizedString("Exchange2007", "Ex65D9D0", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProviderFileLoadException(string providerName, string assemblyPath)
		{
			return new LocalizedString("ProviderFileLoadException", "Ex3F3CB6", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				providerName,
				assemblyPath
			});
		}

		public static LocalizedString SharedConfigurationNotFound(string tinyTenant)
		{
			return new LocalizedString("SharedConfigurationNotFound", "Ex947D5F", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				tinyTenant
			});
		}

		public static LocalizedString DisabledPartner
		{
			get
			{
				return new LocalizedString("DisabledPartner", "Ex831C1D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionFilterWithNullValue(string property)
		{
			return new LocalizedString("ExceptionFilterWithNullValue", "ExD9BA71", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString ErrorPrimarySmtpTemplateInvalid(string primary)
		{
			return new LocalizedString("ErrorPrimarySmtpTemplateInvalid", "Ex585166", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				primary
			});
		}

		public static LocalizedString ExceptionOwaCannotSetPropertyOnLegacyMailboxPolicy(string property)
		{
			return new LocalizedString("ExceptionOwaCannotSetPropertyOnLegacyMailboxPolicy", "ExAE3E2A", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString InvalidControlAttributeName(string controlType, string pageName, int controlPosition, string attributeName)
		{
			return new LocalizedString("InvalidControlAttributeName", "Ex42C0A0", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				controlType,
				pageName,
				controlPosition,
				attributeName
			});
		}

		public static LocalizedString Consumer
		{
			get
			{
				return new LocalizedString("Consumer", "Ex796023", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PrimaryMailboxRelationType
		{
			get
			{
				return new LocalizedString("PrimaryMailboxRelationType", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Disabled
		{
			get
			{
				return new LocalizedString("Disabled", "Ex8A45D0", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorProperty1LtProperty2(string property1Name, string property1Value, string property2Name, string property2Value)
		{
			return new LocalizedString("ErrorProperty1LtProperty2", "Ex62EFBE", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				property1Name,
				property1Value,
				property2Name,
				property2Value
			});
		}

		public static LocalizedString SKUCapabilityBPOSSBasicCustomDomain
		{
			get
			{
				return new LocalizedString("SKUCapabilityBPOSSBasicCustomDomain", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ControlTextNull
		{
			get
			{
				return new LocalizedString("ControlTextNull", "Ex9C1B4E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Outbox
		{
			get
			{
				return new LocalizedString("Outbox", "Ex473C6F", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ArchiveStateNone
		{
			get
			{
				return new LocalizedString("ArchiveStateNone", "ExF4F608", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailFlowPartnerInternalMailContentTypeMimeText
		{
			get
			{
				return new LocalizedString("MailFlowPartnerInternalMailContentTypeMimeText", "ExF2ED8D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomInternalBodyRequired
		{
			get
			{
				return new LocalizedString("CustomInternalBodyRequired", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PartnerManaged_Violation(string objectName, string cmdLet, string parameters, string capabilities)
		{
			return new LocalizedString("PartnerManaged_Violation", "ExD10332", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				objectName,
				cmdLet,
				parameters,
				capabilities
			});
		}

		public static LocalizedString TlsDomainWithIncorrectTlsAuthLevel
		{
			get
			{
				return new LocalizedString("TlsDomainWithIncorrectTlsAuthLevel", "ExD05846", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SystemTag
		{
			get
			{
				return new LocalizedString("SystemTag", "Ex238508", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfigurationSettingsInvalidPriority(int priority)
		{
			return new LocalizedString("ConfigurationSettingsInvalidPriority", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				priority
			});
		}

		public static LocalizedString AllMailboxContentMC
		{
			get
			{
				return new LocalizedString("AllMailboxContentMC", "Ex79A8C0", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteUserMailboxTypeDetails
		{
			get
			{
				return new LocalizedString("RemoteUserMailboxTypeDetails", "Ex5525D7", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BluetoothDisable
		{
			get
			{
				return new LocalizedString("BluetoothDisable", "Ex869DB6", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorParseCountryInfo(string name, int countrycode, string displayName)
		{
			return new LocalizedString("ErrorParseCountryInfo", "ExD0212C", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				name,
				countrycode,
				displayName
			});
		}

		public static LocalizedString ServerRoleLanguagePacks
		{
			get
			{
				return new LocalizedString("ServerRoleLanguagePacks", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMailTipTranslationCultureNotSupported(string culture)
		{
			return new LocalizedString("ErrorMailTipTranslationCultureNotSupported", "Ex22BD1F", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				culture
			});
		}

		public static LocalizedString CantSetDialPlanProperty(string name)
		{
			return new LocalizedString("CantSetDialPlanProperty", "ExE8E1C7", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString PrincipalName
		{
			get
			{
				return new LocalizedString("PrincipalName", "Ex0C344B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidCustomGreetingFilename(string s)
		{
			return new LocalizedString("InvalidCustomGreetingFilename", "Ex4CF087", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString IdIsNotSet
		{
			get
			{
				return new LocalizedString("IdIsNotSet", "Ex4120B4", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationSupervisionListEntryStringPartIsInvalid
		{
			get
			{
				return new LocalizedString("ConstraintViolationSupervisionListEntryStringPartIsInvalid", "ExA3BF0F", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WellKnownRecipientTypeMailContacts
		{
			get
			{
				return new LocalizedString("WellKnownRecipientTypeMailContacts", "ExDF27E6", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleHubTransport
		{
			get
			{
				return new LocalizedString("ServerRoleHubTransport", "ExDABB86", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IndustryHealthcare
		{
			get
			{
				return new LocalizedString("IndustryHealthcare", "ExF88F2F", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionCannotBindToDC(string server)
		{
			return new LocalizedString("ExceptionCannotBindToDC", "ExFAF7B3", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString CapabilityPartnerManaged
		{
			get
			{
				return new LocalizedString("CapabilityPartnerManaged", "Ex77CEE6", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionUnsupportedFilterForPropertyMultiple(string propertyName, Type filterType, Type supportedType1, Type supportedType2)
		{
			return new LocalizedString("ExceptionUnsupportedFilterForPropertyMultiple", "Ex70F37B", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				propertyName,
				filterType,
				supportedType1,
				supportedType2
			});
		}

		public static LocalizedString ExArgumentNullException(string paramName)
		{
			return new LocalizedString("ExArgumentNullException", "Ex6FB256", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				paramName
			});
		}

		public static LocalizedString ErrorArchiveDatabaseArchiveDomainMissing
		{
			get
			{
				return new LocalizedString("ErrorArchiveDatabaseArchiveDomainMissing", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailEnabledUniversalSecurityGroupRecipientType
		{
			get
			{
				return new LocalizedString("MailEnabledUniversalSecurityGroupRecipientType", "ExBC0A67", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRemovalNotSupported
		{
			get
			{
				return new LocalizedString("ErrorRemovalNotSupported", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeFaxMC
		{
			get
			{
				return new LocalizedString("ExchangeFaxMC", "Ex0664DC", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ByteEncoderTypeUse7Bit
		{
			get
			{
				return new LocalizedString("ByteEncoderTypeUse7Bit", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidBindingAddressSetting
		{
			get
			{
				return new LocalizedString("InvalidBindingAddressSetting", "Ex28BCF1", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ASAccessMethodNeedsAuthenticationAccount
		{
			get
			{
				return new LocalizedString("ASAccessMethodNeedsAuthenticationAccount", "ExB071A0", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionSearchRootNotWithinScope(string root, string scope)
		{
			return new LocalizedString("ExceptionSearchRootNotWithinScope", "Ex8B3BB7", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				root,
				scope
			});
		}

		public static LocalizedString CanRunDefaultUpdateState_Allowed
		{
			get
			{
				return new LocalizedString("CanRunDefaultUpdateState_Allowed", "Ex873F7F", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangMalay
		{
			get
			{
				return new LocalizedString("EsnLangMalay", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionTimelimitExceeded(string server, TimeSpan serverTimeout)
		{
			return new LocalizedString("ExceptionTimelimitExceeded", "Ex2C7201", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				server,
				serverTimeout
			});
		}

		public static LocalizedString FailedToParseAlternateServiceAccountCredential
		{
			get
			{
				return new LocalizedString("FailedToParseAlternateServiceAccountCredential", "Ex4C38F2", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionADRetryOnceOperationFailed(string server, string message)
		{
			return new LocalizedString("ExceptionADRetryOnceOperationFailed", "Ex872538", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				server,
				message
			});
		}

		public static LocalizedString ExternalManagedMailContactTypeDetails
		{
			get
			{
				return new LocalizedString("ExternalManagedMailContactTypeDetails", "Ex325B7F", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IPv6Only
		{
			get
			{
				return new LocalizedString("IPv6Only", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNeutralCulture(string culture)
		{
			return new LocalizedString("ErrorNeutralCulture", "ExBA5E48", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				culture
			});
		}

		public static LocalizedString ExceptionInvalidOperationOnObject(string operation)
		{
			return new LocalizedString("ExceptionInvalidOperationOnObject", "Ex9CEE4B", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				operation
			});
		}

		public static LocalizedString MountDialOverrideLossless
		{
			get
			{
				return new LocalizedString("MountDialOverrideLossless", "Ex9E41A7", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerSideADTopologyServiceCallError(string server, string error)
		{
			return new LocalizedString("ServerSideADTopologyServiceCallError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				server,
				error
			});
		}

		public static LocalizedString Percent
		{
			get
			{
				return new LocalizedString("Percent", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionWin32OperationFailed(int error, string message)
		{
			return new LocalizedString("ExceptionWin32OperationFailed", "Ex8537A8", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				error,
				message
			});
		}

		public static LocalizedString ErrorDCNotFound(string hostName)
		{
			return new LocalizedString("ErrorDCNotFound", "ExC55DB1", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				hostName
			});
		}

		public static LocalizedString ServerRoleProvisionedServer
		{
			get
			{
				return new LocalizedString("ServerRoleProvisionedServer", "ExA60544", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarAgeFilterOneMonth
		{
			get
			{
				return new LocalizedString("CalendarAgeFilterOneMonth", "Ex2F881A", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TextOnly
		{
			get
			{
				return new LocalizedString("TextOnly", "ExAAFCCE", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddressBookNoSecurityDescriptor(string id)
		{
			return new LocalizedString("AddressBookNoSecurityDescriptor", "ExCEC884", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString NotInWriteToMbxMode(string propName)
		{
			return new LocalizedString("NotInWriteToMbxMode", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				propName
			});
		}

		public static LocalizedString ErrorAuthServerNotFound(string issuerIdentifier)
		{
			return new LocalizedString("ErrorAuthServerNotFound", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				issuerIdentifier
			});
		}

		public static LocalizedString InvalidMsgTrackingLogSizeConfiguration
		{
			get
			{
				return new LocalizedString("InvalidMsgTrackingLogSizeConfiguration", "ExEE9F08", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorArchiveDatabaseSetForNonArchive
		{
			get
			{
				return new LocalizedString("ErrorArchiveDatabaseSetForNonArchive", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidGenerationTime
		{
			get
			{
				return new LocalizedString("InvalidGenerationTime", "ExEDFCB0", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarItemMC
		{
			get
			{
				return new LocalizedString("CalendarItemMC", "Ex925150", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorProductFileNameDifferentFromCopyFileName(string productFileName, string copyFileName)
		{
			return new LocalizedString("ErrorProductFileNameDifferentFromCopyFileName", "ExAC0BFA", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				productFileName,
				copyFileName
			});
		}

		public static LocalizedString Block
		{
			get
			{
				return new LocalizedString("Block", "Ex3B9A4D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValueNotInRange(int minValue, int maxValue)
		{
			return new LocalizedString("ValueNotInRange", "ExBFF981", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				minValue,
				maxValue
			});
		}

		public static LocalizedString ErrorNullExternalEmailAddress
		{
			get
			{
				return new LocalizedString("ErrorNullExternalEmailAddress", "Ex693C8A", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRemoteAccountPartitionMustHaveTrust(string id)
		{
			return new LocalizedString("ErrorRemoteAccountPartitionMustHaveTrust", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ExceptionRusNotRunning
		{
			get
			{
				return new LocalizedString("ExceptionRusNotRunning", "Ex5ACDCA", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PropertyCannotBeSetToTest
		{
			get
			{
				return new LocalizedString("PropertyCannotBeSetToTest", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorInvalidEscaping
		{
			get
			{
				return new LocalizedString("LdapFilterErrorInvalidEscaping", "Ex5C0803", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ForceSave
		{
			get
			{
				return new LocalizedString("ForceSave", "ExA22F93", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedRoomMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("LinkedRoomMailboxRecipientTypeDetails", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeleteUseCustomAlert
		{
			get
			{
				return new LocalizedString("DeleteUseCustomAlert", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotDeserializePartitionHint
		{
			get
			{
				return new LocalizedString("CannotDeserializePartitionHint", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMasterServerInvalid(string dbName)
		{
			return new LocalizedString("ErrorMasterServerInvalid", "Ex8D3ED6", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				dbName
			});
		}

		public static LocalizedString InboundConnectorInvalidRestrictDomainsToIPAddresses
		{
			get
			{
				return new LocalizedString("InboundConnectorInvalidRestrictDomainsToIPAddresses", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidExecutingOrg(string execOrg, string currentOrg)
		{
			return new LocalizedString("ErrorInvalidExecutingOrg", "Ex7D102A", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				execOrg,
				currentOrg
			});
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute14
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute14", "Ex355A75", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContactRecipientType
		{
			get
			{
				return new LocalizedString("ContactRecipientType", "ExD04F8D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DomainSecureWithoutDNSRoutingEnabled
		{
			get
			{
				return new LocalizedString("DomainSecureWithoutDNSRoutingEnabled", "Ex973FDE", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidMailboxProvisioningConstraint(string parserErrorString)
		{
			return new LocalizedString("ErrorInvalidMailboxProvisioningConstraint", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				parserErrorString
			});
		}

		public static LocalizedString RunspaceServerSettingsChanged
		{
			get
			{
				return new LocalizedString("RunspaceServerSettingsChanged", "ExAC157D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangGreek
		{
			get
			{
				return new LocalizedString("EsnLangGreek", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotMakePrimary(MservRecord record, string recordType)
		{
			return new LocalizedString("CannotMakePrimary", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				record,
				recordType
			});
		}

		public static LocalizedString MsaUserAlreadyExistsInGlsError(string msaUserNetId)
		{
			return new LocalizedString("MsaUserAlreadyExistsInGlsError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				msaUserNetId
			});
		}

		public static LocalizedString TooManyEntriesError
		{
			get
			{
				return new LocalizedString("TooManyEntriesError", "ExBA47A1", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerComponentReadTimeout(int timeoutInSec)
		{
			return new LocalizedString("ServerComponentReadTimeout", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				timeoutInSec
			});
		}

		public static LocalizedString InvalidOABMapiPropertyParseStringException(string str)
		{
			return new LocalizedString("InvalidOABMapiPropertyParseStringException", "Ex9F9257", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				str
			});
		}

		public static LocalizedString OrganizationRelationshipMissingTargetApplicationUri
		{
			get
			{
				return new LocalizedString("OrganizationRelationshipMissingTargetApplicationUri", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRecipientDoesNotExist(string id)
		{
			return new LocalizedString("ErrorRecipientDoesNotExist", "Ex07F5B3", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ComputerRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("ComputerRecipientTypeDetails", "Ex38B480", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Exchweb
		{
			get
			{
				return new LocalizedString("Exchweb", "Ex51FC39", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OutboundConnectorIncorrectRouteAllMessagesViaOnPremises
		{
			get
			{
				return new LocalizedString("OutboundConnectorIncorrectRouteAllMessagesViaOnPremises", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarSharingFreeBusyAvailabilityOnly
		{
			get
			{
				return new LocalizedString("CalendarSharingFreeBusyAvailabilityOnly", "Ex3C5AA4", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotCompareScopeObjects(string leftId, string rightId)
		{
			return new LocalizedString("CannotCompareScopeObjects", "Ex8E5470", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				leftId,
				rightId
			});
		}

		public static LocalizedString ServerRoleExtendedRole5
		{
			get
			{
				return new LocalizedString("ServerRoleExtendedRole5", "Ex71E13E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionADTopologyErrorWhenLookingForServersInDomain(int error, string domain, string message)
		{
			return new LocalizedString("ExceptionADTopologyErrorWhenLookingForServersInDomain", "Ex4D4CEC", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				error,
				domain,
				message
			});
		}

		public static LocalizedString AutoAttendantLink
		{
			get
			{
				return new LocalizedString("AutoAttendantLink", "Ex17EEE4", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomRoleDescription_MyDisplayName
		{
			get
			{
				return new LocalizedString("CustomRoleDescription_MyDisplayName", "Ex29C8C7", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AllUsers
		{
			get
			{
				return new LocalizedString("AllUsers", "Ex038FB6", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString All
		{
			get
			{
				return new LocalizedString("All", "ExCCDF22", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotFindTenantCUByAcceptedDomain(string domain)
		{
			return new LocalizedString("CannotFindTenantCUByAcceptedDomain", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString ComposedSuitabilityReachabilityError(string fqdn, string details)
		{
			return new LocalizedString("ComposedSuitabilityReachabilityError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				fqdn,
				details
			});
		}

		public static LocalizedString ErrorNoWriteScope(string identity)
		{
			return new LocalizedString("ErrorNoWriteScope", "ExE5C228", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ErrorNonUniqueDN(string DN)
		{
			return new LocalizedString("ErrorNonUniqueDN", "ExD784D5", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				DN
			});
		}

		public static LocalizedString OrganizationCapabilityMigration
		{
			get
			{
				return new LocalizedString("OrganizationCapabilityMigration", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DialPlan
		{
			get
			{
				return new LocalizedString("DialPlan", "ExB5CB6C", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangUkrainian
		{
			get
			{
				return new LocalizedString("EsnLangUkrainian", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotCompareAssignmentsMissingScope(string leftId, string rightId)
		{
			return new LocalizedString("CannotCompareAssignmentsMissingScope", "Ex6E6E9F", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				leftId,
				rightId
			});
		}

		public static LocalizedString InvalidInfluence(Influence influence)
		{
			return new LocalizedString("InvalidInfluence", "ExF307D3", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				influence
			});
		}

		public static LocalizedString MessageRateSourceFlagsNone
		{
			get
			{
				return new LocalizedString("MessageRateSourceFlagsNone", "ExB119D6", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EOPPremiumRestrictions_Error(string objectName, string cmdLet, string parameters, string capabilities)
		{
			return new LocalizedString("EOPPremiumRestrictions_Error", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				objectName,
				cmdLet,
				parameters,
				capabilities
			});
		}

		public static LocalizedString IndustryLegal
		{
			get
			{
				return new LocalizedString("IndustryLegal", "ExDF1C27", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CapabilityUMFeatureRestricted
		{
			get
			{
				return new LocalizedString("CapabilityUMFeatureRestricted", "Ex64A8C5", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupTypeFlagsBuiltinLocal
		{
			get
			{
				return new LocalizedString("GroupTypeFlagsBuiltinLocal", "Ex288CF4", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReceiveAuthMechanismBasicAuthPlusTls
		{
			get
			{
				return new LocalizedString("ReceiveAuthMechanismBasicAuthPlusTls", "ExC96519", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Allowed
		{
			get
			{
				return new LocalizedString("Allowed", "Ex054DEC", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ByteEncoderTypeUseQPHtml7BitTextPlain
		{
			get
			{
				return new LocalizedString("ByteEncoderTypeUseQPHtml7BitTextPlain", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmbiguousTimeZoneNameError(string etz)
		{
			return new LocalizedString("AmbiguousTimeZoneNameError", "Ex4EE412", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				etz
			});
		}

		public static LocalizedString High
		{
			get
			{
				return new LocalizedString("High", "Ex278F98", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MicrosoftExchangeRecipientType
		{
			get
			{
				return new LocalizedString("MicrosoftExchangeRecipientType", "Ex6EE737", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BackSyncDataSourceUnavailableMessage
		{
			get
			{
				return new LocalizedString("BackSyncDataSourceUnavailableMessage", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ArchiveStateOnPremise
		{
			get
			{
				return new LocalizedString("ArchiveStateOnPremise", "Ex7043B8", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRoleEntryType(string entry, RoleType roleType, ManagementRoleEntryType roleEntryType)
		{
			return new LocalizedString("InvalidRoleEntryType", "Ex77A26E", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				entry,
				roleType,
				roleEntryType
			});
		}

		public static LocalizedString OrganizationCapabilitySuiteServiceStorage
		{
			get
			{
				return new LocalizedString("OrganizationCapabilitySuiteServiceStorage", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MalwareScanErrorActionBlock
		{
			get
			{
				return new LocalizedString("MalwareScanErrorActionBlock", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SKUCapabilityBPOSSArchiveAddOn
		{
			get
			{
				return new LocalizedString("SKUCapabilityBPOSSArchiveAddOn", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxPropertiesMustBeClearedFirst(MservRecord record)
		{
			return new LocalizedString("MailboxPropertiesMustBeClearedFirst", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				record
			});
		}

		public static LocalizedString ExceptionRusAccessDenied
		{
			get
			{
				return new LocalizedString("ExceptionRusAccessDenied", "Ex4E1FB2", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnexpectedGlsError(string message)
		{
			return new LocalizedString("UnexpectedGlsError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString ServerRoleNone
		{
			get
			{
				return new LocalizedString("ServerRoleNone", "Ex4DBF20", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlternateServiceAccountConfigurationDisplayFormatMoreDataAvailable
		{
			get
			{
				return new LocalizedString("AlternateServiceAccountConfigurationDisplayFormatMoreDataAvailable", "Ex32595D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotInWriteToMServMode(string propName)
		{
			return new LocalizedString("NotInWriteToMServMode", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				propName
			});
		}

		public static LocalizedString GloballyDistributedOABCacheWriteTimeoutError
		{
			get
			{
				return new LocalizedString("GloballyDistributedOABCacheWriteTimeoutError", "Ex45DB87", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorEmptyString(string propertyName)
		{
			return new LocalizedString("ErrorEmptyString", "Ex9EA7D3", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				propertyName
			});
		}

		public static LocalizedString UserName
		{
			get
			{
				return new LocalizedString("UserName", "ExAC819B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Reserved1
		{
			get
			{
				return new LocalizedString("Reserved1", "ExE974A4", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionSizelimitExceeded(string server)
		{
			return new LocalizedString("ExceptionSizelimitExceeded", "ExAE3319", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString NoAddresses
		{
			get
			{
				return new LocalizedString("NoAddresses", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RegionBlockListNotSet
		{
			get
			{
				return new LocalizedString("RegionBlockListNotSet", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CapabilityRichCoexistence
		{
			get
			{
				return new LocalizedString("CapabilityRichCoexistence", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUserAccountNameIncludeAt
		{
			get
			{
				return new LocalizedString("ErrorUserAccountNameIncludeAt", "Ex055DD6", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Enabled
		{
			get
			{
				return new LocalizedString("Enabled", "ExA964BC", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AttachmentsWereRemovedMessage
		{
			get
			{
				return new LocalizedString("AttachmentsWereRemovedMessage", "ExD7F183", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotFindUnusedLegacyDN
		{
			get
			{
				return new LocalizedString("ErrorCannotFindUnusedLegacyDN", "ExA00F16", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmailAgeFilterOneWeek
		{
			get
			{
				return new LocalizedString("EmailAgeFilterOneWeek", "Ex91148D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNameInNamingPolicy
		{
			get
			{
				return new LocalizedString("GroupNameInNamingPolicy", "Ex616D28", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRealmFormatInvalid(string realm)
		{
			return new LocalizedString("ErrorRealmFormatInvalid", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				realm
			});
		}

		public static LocalizedString OrganizationCapabilityClientExtensions
		{
			get
			{
				return new LocalizedString("OrganizationCapabilityClientExtensions", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarAgeFilterTwoWeeks
		{
			get
			{
				return new LocalizedString("CalendarAgeFilterTwoWeeks", "Ex7874E3", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfigurationSettingsGroupExists(string name)
		{
			return new LocalizedString("ConfigurationSettingsGroupExists", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ExceptionADOperationFailedAlreadyExist(string server, string dn)
		{
			return new LocalizedString("ExceptionADOperationFailedAlreadyExist", "Ex17CDAE", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				server,
				dn
			});
		}

		public static LocalizedString ErrorElcCommentNotAllowed
		{
			get
			{
				return new LocalizedString("ErrorElcCommentNotAllowed", "Ex1FBDE8", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorOwnersUpdated
		{
			get
			{
				return new LocalizedString("ErrorOwnersUpdated", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangIndonesian
		{
			get
			{
				return new LocalizedString("EsnLangIndonesian", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidEndpointAddressErrorMessage(string exceptionType, string wcfEndpoint)
		{
			return new LocalizedString("InvalidEndpointAddressErrorMessage", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				exceptionType,
				wcfEndpoint
			});
		}

		public static LocalizedString Extension
		{
			get
			{
				return new LocalizedString("Extension", "ExFED42A", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAuthServerTypeNotFound(string type)
		{
			return new LocalizedString("ErrorAuthServerTypeNotFound", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString CanEnableLocalCopyState_Invalid
		{
			get
			{
				return new LocalizedString("CanEnableLocalCopyState_Invalid", "Ex268363", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IsMemberOfQueryFailed(string group)
		{
			return new LocalizedString("IsMemberOfQueryFailed", "Ex249393", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				group
			});
		}

		public static LocalizedString MailEnabledUniversalDistributionGroupRecipientType
		{
			get
			{
				return new LocalizedString("MailEnabledUniversalDistributionGroupRecipientType", "ExBC6DD1", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReceiveCredentialIsNull
		{
			get
			{
				return new LocalizedString("ReceiveCredentialIsNull", "Ex843842", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionADTopologyErrorWhenLookingForSite(int error, string siteName, string message)
		{
			return new LocalizedString("ExceptionADTopologyErrorWhenLookingForSite", "Ex30CCEF", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				error,
				siteName,
				message
			});
		}

		public static LocalizedString ErrorNotResettableProperty(string propertyName, string value)
		{
			return new LocalizedString("ErrorNotResettableProperty", "Ex989D1A", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				propertyName,
				value
			});
		}

		public static LocalizedString ErrorMailTipHtmlCorrupt(string message)
		{
			return new LocalizedString("ErrorMailTipHtmlCorrupt", "ExB99CE5", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString ErrorInvalidOrganizationId(string dn, string ou, string cu)
		{
			return new LocalizedString("ErrorInvalidOrganizationId", "Ex0510A3", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				dn,
				ou,
				cu
			});
		}

		public static LocalizedString ErrorConversionFailed(string name)
		{
			return new LocalizedString("ErrorConversionFailed", "Ex50F99E", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString EsnLangLithuanian
		{
			get
			{
				return new LocalizedString("EsnLangLithuanian", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ResourceMailbox_Violation(string objectName, string cmdLet, string parameters, string capabilities)
		{
			return new LocalizedString("ResourceMailbox_Violation", "ExD234A9", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				objectName,
				cmdLet,
				parameters,
				capabilities
			});
		}

		public static LocalizedString ServerRoleAll
		{
			get
			{
				return new LocalizedString("ServerRoleAll", "ExCAD036", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleEdge
		{
			get
			{
				return new LocalizedString("ServerRoleEdge", "ExCE4073", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DomainNotFoundInGlsError(string domain)
		{
			return new LocalizedString("DomainNotFoundInGlsError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString ExceptionObjectStillExists
		{
			get
			{
				return new LocalizedString("ExceptionObjectStillExists", "Ex8EA36A", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AllRecipients
		{
			get
			{
				return new LocalizedString("AllRecipients", "ExDA64AB", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorNoAttributeType
		{
			get
			{
				return new LocalizedString("LdapFilterErrorNoAttributeType", "Ex723877", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleManagementFrontEnd
		{
			get
			{
				return new LocalizedString("ServerRoleManagementFrontEnd", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString False
		{
			get
			{
				return new LocalizedString("False", "ExB6CDA4", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionSchemaMismatch(string attributeName, bool isBinaryInADRecipient, bool isMultiValuedInADRecipient, bool isBinaryInRUS, bool isMultiValuedInRus)
		{
			return new LocalizedString("ExceptionSchemaMismatch", "Ex20DF9F", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				attributeName,
				isBinaryInADRecipient,
				isMultiValuedInADRecipient,
				isBinaryInRUS,
				isMultiValuedInRus
			});
		}

		public static LocalizedString EapDuplicatedEmailAddressTemplate(string emailAddressTemplate)
		{
			return new LocalizedString("EapDuplicatedEmailAddressTemplate", "Ex876DD7", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				emailAddressTemplate
			});
		}

		public static LocalizedString NspiFailureException(int status)
		{
			return new LocalizedString("NspiFailureException", "Ex266E5C", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				status
			});
		}

		public static LocalizedString EapMustHaveOnePrimaryAddressTemplate(string prefix)
		{
			return new LocalizedString("EapMustHaveOnePrimaryAddressTemplate", "ExB9B6A1", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				prefix
			});
		}

		public static LocalizedString CalendarSharingFreeBusyLimitedDetails
		{
			get
			{
				return new LocalizedString("CalendarSharingFreeBusyLimitedDetails", "ExFC8395", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SystemAttendantMailboxRecipientType
		{
			get
			{
				return new LocalizedString("SystemAttendantMailboxRecipientType", "Ex926652", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleManagementBackEnd
		{
			get
			{
				return new LocalizedString("ServerRoleManagementBackEnd", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SuitabilityDirectoryException(string fqdn, int error, string errorMessage)
		{
			return new LocalizedString("SuitabilityDirectoryException", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				fqdn,
				error,
				errorMessage
			});
		}

		public static LocalizedString ExceptionADTopologyHasNoAvailableServersInForest(string forest)
		{
			return new LocalizedString("ExceptionADTopologyHasNoAvailableServersInForest", "Ex4C422F", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				forest
			});
		}

		public static LocalizedString InvalidRecipientScope(object scope)
		{
			return new LocalizedString("InvalidRecipientScope", "Ex578D9C", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				scope
			});
		}

		public static LocalizedString UnableToResolveMapiPropertyNameException(string name)
		{
			return new LocalizedString("UnableToResolveMapiPropertyNameException", "ExD716B7", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString GroupNamingPolicyStateOrProvince
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyStateOrProvince", "Ex7CC623", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IndustryFinance
		{
			get
			{
				return new LocalizedString("IndustryFinance", "Ex53C08F", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAgeLimitExpiration
		{
			get
			{
				return new LocalizedString("ErrorAgeLimitExpiration", "ExD679C7", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidOABMapiPropertyTypeException(string type)
		{
			return new LocalizedString("InvalidOABMapiPropertyTypeException", "ExA57B43", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString InboundConnectorMissingTlsCertificateOrSenderIP
		{
			get
			{
				return new LocalizedString("InboundConnectorMissingTlsCertificateOrSenderIP", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMailTipTranslationFormatIncorrect
		{
			get
			{
				return new LocalizedString("ErrorMailTipTranslationFormatIncorrect", "ExE140A5", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MountDialOverrideGoodAvailability
		{
			get
			{
				return new LocalizedString("MountDialOverrideGoodAvailability", "ExCDE6CC", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidMailboxProvisioningAttribute(string attribute)
		{
			return new LocalizedString("ErrorInvalidMailboxProvisioningAttribute", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				attribute
			});
		}

		public static LocalizedString ConfigReadScope
		{
			get
			{
				return new LocalizedString("ConfigReadScope", "Ex78877B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMailboxProvisioningAttributeDoesNotMatchSchema(string keyName, string validKeys)
		{
			return new LocalizedString("ErrorMailboxProvisioningAttributeDoesNotMatchSchema", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				keyName,
				validKeys
			});
		}

		public static LocalizedString ErrorMultiplePrimaries(string prefix)
		{
			return new LocalizedString("ErrorMultiplePrimaries", "Ex0D6BFD", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				prefix
			});
		}

		public static LocalizedString ErrorMinAdminVersionNull(ExchangeObjectVersion exchangeVersion)
		{
			return new LocalizedString("ErrorMinAdminVersionNull", "Ex7D5EA6", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				exchangeVersion
			});
		}

		public static LocalizedString UserRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("UserRecipientTypeDetails", "Ex97118C", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MismatchedMapiPropertyTypesException(int prop1, int prop2)
		{
			return new LocalizedString("MismatchedMapiPropertyTypesException", "Ex3D3F48", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				prop1,
				prop2
			});
		}

		public static LocalizedString ServerHasNotBeenFound(int versionNumber, string identifier, bool needsExactVersionMatch, string siteName)
		{
			return new LocalizedString("ServerHasNotBeenFound", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				versionNumber,
				identifier,
				needsExactVersionMatch,
				siteName
			});
		}

		public static LocalizedString ErrorDuplicateKeyInMailboxProvisioningAttributes(string duplicateKeyName)
		{
			return new LocalizedString("ErrorDuplicateKeyInMailboxProvisioningAttributes", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				duplicateKeyName
			});
		}

		public static LocalizedString MeetingRequestMC
		{
			get
			{
				return new LocalizedString("MeetingRequestMC", "ExFEABF4", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Tag
		{
			get
			{
				return new LocalizedString("Tag", "Ex9F8D72", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailFlowPartnerInternalMailContentTypeTNEF
		{
			get
			{
				return new LocalizedString("MailFlowPartnerInternalMailContentTypeTNEF", "ExE2BB59", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPrimarySmtpInvalid(string primary)
		{
			return new LocalizedString("ErrorPrimarySmtpInvalid", "Ex2AD0EF", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				primary
			});
		}

		public static LocalizedString SerialNumberMissing
		{
			get
			{
				return new LocalizedString("SerialNumberMissing", "Ex0FA073", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionReferral(string target, string referral, string dn, string filter)
		{
			return new LocalizedString("ExceptionReferral", "Ex9288F1", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				target,
				referral,
				dn,
				filter
			});
		}

		public static LocalizedString AttributeNameNull
		{
			get
			{
				return new LocalizedString("AttributeNameNull", "Ex6194D5", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorIsDehydratedSetOnNonTinyTenant
		{
			get
			{
				return new LocalizedString("ErrorIsDehydratedSetOnNonTinyTenant", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionRootDSE(string attr, string server)
		{
			return new LocalizedString("ExceptionRootDSE", "ExD724D2", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				attr,
				server
			});
		}

		public static LocalizedString TUIPromptEditingEnabled
		{
			get
			{
				return new LocalizedString("TUIPromptEditingEnabled", "ExE96435", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StarAcceptedDomainCannotBeInitialDomain
		{
			get
			{
				return new LocalizedString("StarAcceptedDomainCannotBeInitialDomain", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorNotSupportSingleComp
		{
			get
			{
				return new LocalizedString("LdapFilterErrorNotSupportSingleComp", "ExC6795F", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UseTnef
		{
			get
			{
				return new LocalizedString("UseTnef", "Ex6B67E7", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AttachmentFilterEntryInvalid
		{
			get
			{
				return new LocalizedString("AttachmentFilterEntryInvalid", "ExA1D0A4", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Exchange2013
		{
			get
			{
				return new LocalizedString("Exchange2013", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SendAuthMechanismBasicAuthPlusTls
		{
			get
			{
				return new LocalizedString("SendAuthMechanismBasicAuthPlusTls", "Ex8B8C82", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MoveToDeletedItems
		{
			get
			{
				return new LocalizedString("MoveToDeletedItems", "Ex8BFB3B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TCP
		{
			get
			{
				return new LocalizedString("TCP", "ExD6B94B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoSuitableGCInForest(string domainName, string errorMessage)
		{
			return new LocalizedString("ErrorNoSuitableGCInForest", "Ex5BDD81", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				domainName,
				errorMessage
			});
		}

		public static LocalizedString InvalidBiggerResourceThreshold(string classification, string thresholdName1, string thresholdName2, int value1, int value2)
		{
			return new LocalizedString("InvalidBiggerResourceThreshold", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				classification,
				thresholdName1,
				thresholdName2,
				value1,
				value2
			});
		}

		public static LocalizedString DocumentMC
		{
			get
			{
				return new LocalizedString("DocumentMC", "Ex6C2D12", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotSetWindowsEmailAddress
		{
			get
			{
				return new LocalizedString("ErrorCannotSetWindowsEmailAddress", "Ex0F45A4", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidTimeZoneId(string id)
		{
			return new LocalizedString("InvalidTimeZoneId", "ExFDA24E", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString CannotSerializePartitionHint(string hint)
		{
			return new LocalizedString("CannotSerializePartitionHint", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				hint
			});
		}

		public static LocalizedString ErrorThisThresholdMustBeGreaterThanThatThreshold(string name1, string name2)
		{
			return new LocalizedString("ErrorThisThresholdMustBeGreaterThanThatThreshold", "ExBD3235", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				name1,
				name2
			});
		}

		public static LocalizedString Msn
		{
			get
			{
				return new LocalizedString("Msn", "ExDF59E6", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageRateSourceFlagsIPAddress
		{
			get
			{
				return new LocalizedString("MessageRateSourceFlagsIPAddress", "ExA2B22B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTextMessageIncludingAppleAttachment
		{
			get
			{
				return new LocalizedString("ErrorTextMessageIncludingAppleAttachment", "Ex82DD36", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ForwardCallsToDefaultMailbox
		{
			get
			{
				return new LocalizedString("ForwardCallsToDefaultMailbox", "ExAD51C3", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionRemoveApprovedApplication(string id)
		{
			return new LocalizedString("ExceptionRemoveApprovedApplication", "ExD16E61", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString RoleGroupTypeDetails
		{
			get
			{
				return new LocalizedString("RoleGroupTypeDetails", "Ex291323", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailEnabledContactRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("MailEnabledContactRecipientTypeDetails", "Ex8B3132", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorJoinApprovalRequiredWithoutManager(string id)
		{
			return new LocalizedString("ErrorJoinApprovalRequiredWithoutManager", "Ex7A64B7", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString InvalidConsumerDialPlanSetting(string prop)
		{
			return new LocalizedString("InvalidConsumerDialPlanSetting", "ExD9B17B", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				prop
			});
		}

		public static LocalizedString EsnLangEnglish
		{
			get
			{
				return new LocalizedString("EsnLangEnglish", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangMarathi
		{
			get
			{
				return new LocalizedString("EsnLangMarathi", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RangeInformationFormatInvalid(string str)
		{
			return new LocalizedString("RangeInformationFormatInvalid", "Ex54FC08", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				str
			});
		}

		public static LocalizedString SpecifyAnnouncementFileName
		{
			get
			{
				return new LocalizedString("SpecifyAnnouncementFileName", "ExD1B2BF", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionADTopologyHasNoAvailableServersInDomain(string domain)
		{
			return new LocalizedString("ExceptionADTopologyHasNoAvailableServersInDomain", "Ex302882", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString RuleMigration_Error(string objectName, string cmdLet, string parameters, string capabilities)
		{
			return new LocalizedString("RuleMigration_Error", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				objectName,
				cmdLet,
				parameters,
				capabilities
			});
		}

		public static LocalizedString PropertiesMasteredOnPremise_Violation(string objectName, string cmdLet, string parameters, string capabilities)
		{
			return new LocalizedString("PropertiesMasteredOnPremise_Violation", "Ex945F88", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				objectName,
				cmdLet,
				parameters,
				capabilities
			});
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute12
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute12", "Ex14AA12", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BadSwapOperationCount(int changeCount)
		{
			return new LocalizedString("BadSwapOperationCount", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				changeCount
			});
		}

		public static LocalizedString CannotDetermineDataSessionTypeForObject(string type)
		{
			return new LocalizedString("CannotDetermineDataSessionTypeForObject", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString SystemAddressListDoesNotExist
		{
			get
			{
				return new LocalizedString("SystemAddressListDoesNotExist", "Ex659C48", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DefaultOabName
		{
			get
			{
				return new LocalizedString("DefaultOabName", "ExCBC06D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangSpanish
		{
			get
			{
				return new LocalizedString("EsnLangSpanish", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionADOperationFailedNoSuchObject(string server, string dn)
		{
			return new LocalizedString("ExceptionADOperationFailedNoSuchObject", "Ex2A592C", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				server,
				dn
			});
		}

		public static LocalizedString FederatedOrganizationIdNoNamespaceAccount
		{
			get
			{
				return new LocalizedString("FederatedOrganizationIdNoNamespaceAccount", "Ex0CF246", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionPropertyCannotBeSearchedOn(string property)
		{
			return new LocalizedString("ExceptionPropertyCannotBeSearchedOn", "ExF6FA9E", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString RemoteEquipmentMailboxTypeDetails
		{
			get
			{
				return new LocalizedString("RemoteEquipmentMailboxTypeDetails", "Ex3FE261", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpamFilteringOptionOn
		{
			get
			{
				return new LocalizedString("SpamFilteringOptionOn", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoSharedConfigurationInfo
		{
			get
			{
				return new LocalizedString("ErrorNoSharedConfigurationInfo", "ExBC8D65", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EquipmentMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("EquipmentMailboxRecipientTypeDetails", "ExBC6B80", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotSetMoveToDestinationFolder
		{
			get
			{
				return new LocalizedString("ErrorCannotSetMoveToDestinationFolder", "ExF94D65", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CapabilityTOUSigned
		{
			get
			{
				return new LocalizedString("CapabilityTOUSigned", "ExD7C91F", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidNtds(string server)
		{
			return new LocalizedString("InvalidNtds", "Ex747A5F", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString ServerRoleExtendedRole2
		{
			get
			{
				return new LocalizedString("ServerRoleExtendedRole2", "ExCEDEA5", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleExtendedRole3
		{
			get
			{
				return new LocalizedString("ServerRoleExtendedRole3", "Ex5AC339", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionADConfigurationObjectRequired(string objectType, string methodName)
		{
			return new LocalizedString("ExceptionADConfigurationObjectRequired", "Ex7FB667", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				objectType,
				methodName
			});
		}

		public static LocalizedString TenantOrgContainerNotFoundException(string orgId)
		{
			return new LocalizedString("TenantOrgContainerNotFoundException", "Ex071072", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				orgId
			});
		}

		public static LocalizedString PersonalFolder
		{
			get
			{
				return new LocalizedString("PersonalFolder", "ExADA167", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CapabilityNone
		{
			get
			{
				return new LocalizedString("CapabilityNone", "Ex0425E3", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorEmptyResourceTypeofResourceMailbox
		{
			get
			{
				return new LocalizedString("ErrorEmptyResourceTypeofResourceMailbox", "Ex1C6D31", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InternalDNSServersNotSet
		{
			get
			{
				return new LocalizedString("InternalDNSServersNotSet", "Ex246010", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionImpersonation
		{
			get
			{
				return new LocalizedString("ExceptionImpersonation", "ExDF6BB4", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionADTopologyUnexpectedError(string server, string error)
		{
			return new LocalizedString("ExceptionADTopologyUnexpectedError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				server,
				error
			});
		}

		public static LocalizedString ReceiveAuthMechanismNone
		{
			get
			{
				return new LocalizedString("ReceiveAuthMechanismNone", "Ex6818A8", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute9
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute9", "ExA79439", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailEnabledDynamicDistributionGroupRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("MailEnabledDynamicDistributionGroupRecipientTypeDetails", "ExCC2975", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpamFilteringActionAddXHeader
		{
			get
			{
				return new LocalizedString("SpamFilteringActionAddXHeader", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNotInServerWriteScope(string identity)
		{
			return new LocalizedString("ErrorNotInServerWriteScope", "Ex5CC9EA", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString DuplicatedAcceptedDomain(string domainName, string firstDup, string secondDup)
		{
			return new LocalizedString("DuplicatedAcceptedDomain", "ExAF3249", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				domainName,
				firstDup,
				secondDup
			});
		}

		public static LocalizedString RecentCommands
		{
			get
			{
				return new LocalizedString("RecentCommands", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SecurityPrincipalTypeNone
		{
			get
			{
				return new LocalizedString("SecurityPrincipalTypeNone", "Ex45443A", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotSetPermanentAttributes(string permanentAttributeNames)
		{
			return new LocalizedString("ErrorCannotSetPermanentAttributes", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				permanentAttributeNames
			});
		}

		public static LocalizedString MailboxMoveStatusNone
		{
			get
			{
				return new LocalizedString("MailboxMoveStatusNone", "Ex3A7378", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotBuildAuthenticationTypeFilterBadArgument(string authType)
		{
			return new LocalizedString("CannotBuildAuthenticationTypeFilterBadArgument", "Ex27E265", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				authType
			});
		}

		public static LocalizedString LocalForest
		{
			get
			{
				return new LocalizedString("LocalForest", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LegacyMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("LegacyMailboxRecipientTypeDetails", "Ex3AD144", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute2
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute2", "Ex624F4E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTargetPartitionHas2TenantsWithSameId(string oldTenant, string newPartition, string guid)
		{
			return new LocalizedString("ErrorTargetPartitionHas2TenantsWithSameId", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				oldTenant,
				newPartition,
				guid
			});
		}

		public static LocalizedString DatabaseMasterTypeUnknown
		{
			get
			{
				return new LocalizedString("DatabaseMasterTypeUnknown", "Ex5A641A", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversationHistory
		{
			get
			{
				return new LocalizedString("ConversationHistory", "Ex082CC3", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OutboundConnectorTlsSettingsInvalidDomainValidationWithoutTlsDomain
		{
			get
			{
				return new LocalizedString("OutboundConnectorTlsSettingsInvalidDomainValidationWithoutTlsDomain", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenMoved
		{
			get
			{
				return new LocalizedString("WhenMoved", "Ex478EE0", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceededMaximumCollectionCount(string propertyName, int maxLength, int actualLength)
		{
			return new LocalizedString("ExceededMaximumCollectionCount", "Ex2BA297", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				propertyName,
				maxLength,
				actualLength
			});
		}

		public static LocalizedString ErrorDuplicateLanguage
		{
			get
			{
				return new LocalizedString("ErrorDuplicateLanguage", "Ex055943", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionReferralWhenBoundToDomainController(string domain, string dc)
		{
			return new LocalizedString("ExceptionReferralWhenBoundToDomainController", "Ex3AA09E", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				domain,
				dc
			});
		}

		public static LocalizedString AssignmentsWithConflictingScope(string id, string anotherId, string details)
		{
			return new LocalizedString("AssignmentsWithConflictingScope", "Ex5157BB", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				id,
				anotherId,
				details
			});
		}

		public static LocalizedString ExceptionReadingRootDSE(string serverName, string message)
		{
			return new LocalizedString("ExceptionReadingRootDSE", "Ex218628", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				serverName,
				message
			});
		}

		public static LocalizedString ExceptionObjectAlreadyExists
		{
			get
			{
				return new LocalizedString("ExceptionObjectAlreadyExists", "ExA1C69E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangCzech
		{
			get
			{
				return new LocalizedString("EsnLangCzech", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ComponentNameInvalid
		{
			get
			{
				return new LocalizedString("ComponentNameInvalid", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotGetForestInfo(string forest)
		{
			return new LocalizedString("CannotGetForestInfo", "ExCC70A2", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				forest
			});
		}

		public static LocalizedString InvalidCertificateName(string certName)
		{
			return new LocalizedString("InvalidCertificateName", "Ex07E797", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				certName
			});
		}

		public static LocalizedString CannotParse(string data)
		{
			return new LocalizedString("CannotParse", "Ex1E4981", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				data
			});
		}

		public static LocalizedString TransportSettingsAmbiguousException(string orgId)
		{
			return new LocalizedString("TransportSettingsAmbiguousException", "ExA135C4", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				orgId
			});
		}

		public static LocalizedString ExceptionADTopologyNoSuchForest(string forest)
		{
			return new LocalizedString("ExceptionADTopologyNoSuchForest", "Ex7AF243", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				forest
			});
		}

		public static LocalizedString PropertyRequired(string propertyName, string recipientType)
		{
			return new LocalizedString("PropertyRequired", "Ex99AECC", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				propertyName,
				recipientType
			});
		}

		public static LocalizedString OUsNotSmallerOrEqual(string leftOU, string rightOU)
		{
			return new LocalizedString("OUsNotSmallerOrEqual", "ExAA571A", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				leftOU,
				rightOU
			});
		}

		public static LocalizedString ErrorAuthMetadataCannotResolveIssuer
		{
			get
			{
				return new LocalizedString("ErrorAuthMetadataCannotResolveIssuer", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyTitle
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyTitle", "ExBEFD80", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxMoveStatusSuspended
		{
			get
			{
				return new LocalizedString("MailboxMoveStatusSuspended", "ExC51ED2", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DomainSecureEnabledWithExternalAuthoritative
		{
			get
			{
				return new LocalizedString("DomainSecureEnabledWithExternalAuthoritative", "ExC43BF8", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BasicAfterTLSWithoutTLS
		{
			get
			{
				return new LocalizedString("BasicAfterTLSWithoutTLS", "ExB40D87", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeoutGlsError(string message)
		{
			return new LocalizedString("TimeoutGlsError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString ExtensionAlreadyUsedAsPilotNumber(string phoneNumber, string dialPlan)
		{
			return new LocalizedString("ExtensionAlreadyUsedAsPilotNumber", "Ex6E5038", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				phoneNumber,
				dialPlan
			});
		}

		public static LocalizedString Private
		{
			get
			{
				return new LocalizedString("Private", "ExECF0B9", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Mailboxes
		{
			get
			{
				return new LocalizedString("Mailboxes", "Ex875C09", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotFindRidMasterForPartition(string partition)
		{
			return new LocalizedString("ErrorCannotFindRidMasterForPartition", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				partition
			});
		}

		public static LocalizedString InvalidCharacterSet(string charset, string supported)
		{
			return new LocalizedString("InvalidCharacterSet", "Ex874AB3", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				charset,
				supported
			});
		}

		public static LocalizedString ErrorModeratorRequiredForModeration
		{
			get
			{
				return new LocalizedString("ErrorModeratorRequiredForModeration", "ExEC501D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomFromAddressRequired
		{
			get
			{
				return new LocalizedString("CustomFromAddressRequired", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapModifyDN
		{
			get
			{
				return new LocalizedString("LdapModifyDN", "ExF4328B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomExternalSubjectRequired
		{
			get
			{
				return new LocalizedString("CustomExternalSubjectRequired", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionADUnavailable(string serverName)
		{
			return new LocalizedString("ExceptionADUnavailable", "ExE542FC", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString ErrorSettingOverrideInvalidFlightName(string flightName, string availableFlights)
		{
			return new LocalizedString("ErrorSettingOverrideInvalidFlightName", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				flightName,
				availableFlights
			});
		}

		public static LocalizedString ErrorInternalLocationsCountMissMatch
		{
			get
			{
				return new LocalizedString("ErrorInternalLocationsCountMissMatch", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ASOnlyOneAuthenticationMethodAllowed
		{
			get
			{
				return new LocalizedString("ASOnlyOneAuthenticationMethodAllowed", "Ex84660E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Tnef
		{
			get
			{
				return new LocalizedString("Tnef", "Ex0DF85E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ByteEncoderTypeUseBase64HtmlDetectTextPlain
		{
			get
			{
				return new LocalizedString("ByteEncoderTypeUseBase64HtmlDetectTextPlain", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TooManyCustomExtensions(string a)
		{
			return new LocalizedString("TooManyCustomExtensions", "ExAD0209", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				a
			});
		}

		public static LocalizedString EsnLangIcelandic
		{
			get
			{
				return new LocalizedString("EsnLangIcelandic", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionOwaCannotSetPropertyOnE12VirtualDirectory(string property)
		{
			return new LocalizedString("ExceptionOwaCannotSetPropertyOnE12VirtualDirectory", "ExDABBEB", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString ServerRoleNAT
		{
			get
			{
				return new LocalizedString("ServerRoleNAT", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UniversalDistributionGroupRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("UniversalDistributionGroupRecipientTypeDetails", "ExD0510E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReplicationLatency
		{
			get
			{
				return new LocalizedString("ErrorReplicationLatency", "Ex15DE8E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EnabledPartner
		{
			get
			{
				return new LocalizedString("EnabledPartner", "ExA88908", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OutboundConnectorSmarthostTlsSettingsInvalid
		{
			get
			{
				return new LocalizedString("OutboundConnectorSmarthostTlsSettingsInvalid", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalCompliance
		{
			get
			{
				return new LocalizedString("ExternalCompliance", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAuthMetadataNoSigningKey
		{
			get
			{
				return new LocalizedString("ErrorAuthMetadataNoSigningKey", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExArgumentException(string paramName, object value)
		{
			return new LocalizedString("ExArgumentException", "ExB1B609", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				paramName,
				value
			});
		}

		public static LocalizedString InboundConnectorIncorrectAllAcceptedDomains
		{
			get
			{
				return new LocalizedString("InboundConnectorIncorrectAllAcceptedDomains", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MoveToFolder
		{
			get
			{
				return new LocalizedString("MoveToFolder", "ExA31786", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Byte
		{
			get
			{
				return new LocalizedString("Byte", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotResolvePartitionGuidError(string guid)
		{
			return new LocalizedString("CannotResolvePartitionGuidError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				guid
			});
		}

		public static LocalizedString EsnLangCyrillic
		{
			get
			{
				return new LocalizedString("EsnLangCyrillic", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CanRunDefaultUpdateState_Invalid
		{
			get
			{
				return new LocalizedString("CanRunDefaultUpdateState_Invalid", "ExEA5F0B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DisabledUserRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("DisabledUserRecipientTypeDetails", "ExE4F7D7", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRecipientType
		{
			get
			{
				return new LocalizedString("InvalidRecipientType", "Ex2E8EE6", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmailAgeFilterThreeDays
		{
			get
			{
				return new LocalizedString("EmailAgeFilterThreeDays", "ExDABD80", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DataMoveReplicationConstraintCISecondCopy
		{
			get
			{
				return new LocalizedString("DataMoveReplicationConstraintCISecondCopy", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotGetDnAtDepth(string dn, int depth)
		{
			return new LocalizedString("CannotGetDnAtDepth", "Ex09C258", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				dn,
				depth
			});
		}

		public static LocalizedString ErrorMissingPrimarySmtp
		{
			get
			{
				return new LocalizedString("ErrorMissingPrimarySmtp", "ExC71195", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorELCFolderNotSpecified
		{
			get
			{
				return new LocalizedString("ErrorELCFolderNotSpecified", "Ex13B523", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotHaveMoreThanOneDefaultThrottlingPolicy
		{
			get
			{
				return new LocalizedString("ErrorCannotHaveMoreThanOneDefaultThrottlingPolicy", "Ex88B409", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReceiveModeCannotBeZero
		{
			get
			{
				return new LocalizedString("ReceiveModeCannotBeZero", "Ex88AFCF", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OwaDefaultDomainRequiredWhenLogonFormatIsUserName
		{
			get
			{
				return new LocalizedString("OwaDefaultDomainRequiredWhenLogonFormatIsUserName", "Ex4B0461", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TLS
		{
			get
			{
				return new LocalizedString("TLS", "Ex167363", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidHostname(object value)
		{
			return new LocalizedString("InvalidHostname", "Ex35FD97", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString LinkedMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("LinkedMailboxRecipientTypeDetails", "Ex93A886", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Tasks
		{
			get
			{
				return new LocalizedString("Tasks", "Ex6B5E90", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RejectAndQuarantineThreshold
		{
			get
			{
				return new LocalizedString("RejectAndQuarantineThreshold", "ExEBA8BE", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorInvalidDecimal
		{
			get
			{
				return new LocalizedString("LdapFilterErrorInvalidDecimal", "ExEF43A4", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpamFilteringTestActionAddXHeader
		{
			get
			{
				return new LocalizedString("SpamFilteringTestActionAddXHeader", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTargetOrSourceForestPopulatedStatusNotStarted(string fqdn1, string fqdn2)
		{
			return new LocalizedString("ErrorTargetOrSourceForestPopulatedStatusNotStarted", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				fqdn1,
				fqdn2
			});
		}

		public static LocalizedString OrganizationCapabilityScaleOut
		{
			get
			{
				return new LocalizedString("OrganizationCapabilityScaleOut", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BadSwapResourceIds(byte rid00, byte rid01, byte rid10, byte rid11)
		{
			return new LocalizedString("BadSwapResourceIds", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				rid00,
				rid01,
				rid10,
				rid11
			});
		}

		public static LocalizedString ExceptionADInvalidPassword(string server)
		{
			return new LocalizedString("ExceptionADInvalidPassword", "ExA04FE5", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString ConfigurationSettingsDuplicateRestriction(string name, string groupName)
		{
			return new LocalizedString("ConfigurationSettingsDuplicateRestriction", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				name,
				groupName
			});
		}

		public static LocalizedString ConstraintViolationOneOffSupervisionListEntryStringPartIsInvalid
		{
			get
			{
				return new LocalizedString("ConstraintViolationOneOffSupervisionListEntryStringPartIsInvalid", "Ex21A177", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidForestFqdnInGls(string forestName, string tenantName, string message)
		{
			return new LocalizedString("InvalidForestFqdnInGls", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				forestName,
				tenantName,
				message
			});
		}

		public static LocalizedString DiscoveryMailboxTypeDetails
		{
			get
			{
				return new LocalizedString("DiscoveryMailboxTypeDetails", "ExC38CF6", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNonUniqueExchangeObjectId(string exchangeObjectIdString)
		{
			return new LocalizedString("ErrorNonUniqueExchangeObjectId", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				exchangeObjectIdString
			});
		}

		public static LocalizedString ErrorAdfsTrustedIssuers
		{
			get
			{
				return new LocalizedString("ErrorAdfsTrustedIssuers", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DataMoveReplicationConstraintCIAllDatacenters
		{
			get
			{
				return new LocalizedString("DataMoveReplicationConstraintCIAllDatacenters", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HygieneSuiteStandard
		{
			get
			{
				return new LocalizedString("HygieneSuiteStandard", "Ex6510CC", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNonUniqueMailboxGetMailboxLocation(string mailboxLocationType)
		{
			return new LocalizedString("ErrorNonUniqueMailboxGetMailboxLocation", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				mailboxLocationType
			});
		}

		public static LocalizedString ErrorMailTipDisplayableLengthExceeded(int max)
		{
			return new LocalizedString("ErrorMailTipDisplayableLengthExceeded", "Ex2C2349", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				max
			});
		}

		public static LocalizedString ErrorIsServerSuitableMissingComputerData(string fqdn, string dcName)
		{
			return new LocalizedString("ErrorIsServerSuitableMissingComputerData", "Ex62AC2D", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				fqdn,
				dcName
			});
		}

		public static LocalizedString ConversionFailed(string propertyName, string typeName, string errorMessage)
		{
			return new LocalizedString("ConversionFailed", "Ex235FAC", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				propertyName,
				typeName,
				errorMessage
			});
		}

		public static LocalizedString EsnLangHindi
		{
			get
			{
				return new LocalizedString("EsnLangHindi", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionOneTimeBindFailed(string serverName, string message)
		{
			return new LocalizedString("ExceptionOneTimeBindFailed", "Ex422932", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				serverName,
				message
			});
		}

		public static LocalizedString ExceptionUnableToCreateConnections
		{
			get
			{
				return new LocalizedString("ExceptionUnableToCreateConnections", "Ex1F8D9B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionDefaultScopeInvalidFormat(string scope)
		{
			return new LocalizedString("ExceptionDefaultScopeInvalidFormat", "Ex451A06", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				scope
			});
		}

		public static LocalizedString SecurityPrincipalTypeWellknownSecurityPrincipal
		{
			get
			{
				return new LocalizedString("SecurityPrincipalTypeWellknownSecurityPrincipal", "ExADF61F", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TenantNameTooLong(string name)
		{
			return new LocalizedString("TenantNameTooLong", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ExArgumentOutOfRangeException(string paramName, object actualValue)
		{
			return new LocalizedString("ExArgumentOutOfRangeException", "ExE05A51", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				paramName,
				actualValue
			});
		}

		public static LocalizedString Error
		{
			get
			{
				return new LocalizedString("Error", "ExDCC897", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidISOTwoLetterOrCountryCode(string name)
		{
			return new LocalizedString("ErrorInvalidISOTwoLetterOrCountryCode", "ExE0452E", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ElcScheduleOnWrongServer
		{
			get
			{
				return new LocalizedString("ElcScheduleOnWrongServer", "Ex968F4D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SyncIssues
		{
			get
			{
				return new LocalizedString("SyncIssues", "Ex35C079", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfigurationSettingsDatabaseNotFound(string id)
		{
			return new LocalizedString("ConfigurationSettingsDatabaseNotFound", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString PartiallyApplied
		{
			get
			{
				return new LocalizedString("PartiallyApplied", "Ex17281D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PreferredInternetCodePageUndefined
		{
			get
			{
				return new LocalizedString("PreferredInternetCodePageUndefined", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoRoleEntriesCmdletOrScriptFound
		{
			get
			{
				return new LocalizedString("NoRoleEntriesCmdletOrScriptFound", "Ex6F96DA", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ApiNotSupportedError(string cl, string member)
		{
			return new LocalizedString("ApiNotSupportedError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				cl,
				member
			});
		}

		public static LocalizedString CannotDeserializePartitionHintTooShort
		{
			get
			{
				return new LocalizedString("CannotDeserializePartitionHintTooShort", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDatabaseCopiesInvalid(string dbName)
		{
			return new LocalizedString("ErrorDatabaseCopiesInvalid", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				dbName
			});
		}

		public static LocalizedString InvalidReceiveAuthModeTLSPassword
		{
			get
			{
				return new LocalizedString("InvalidReceiveAuthModeTLSPassword", "ExA0E3C8", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute8
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute8", "Ex504F82", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangSwedish
		{
			get
			{
				return new LocalizedString("EsnLangSwedish", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidExtension(string property, int length)
		{
			return new LocalizedString("InvalidExtension", "Ex04010B", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				property,
				length
			});
		}

		public static LocalizedString IndustryUtilities
		{
			get
			{
				return new LocalizedString("IndustryUtilities", "Ex9A2A93", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString G711
		{
			get
			{
				return new LocalizedString("G711", "ExF843A4", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalDNSServersNotSet
		{
			get
			{
				return new LocalizedString("ExternalDNSServersNotSet", "Ex040011", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Item
		{
			get
			{
				return new LocalizedString("Item", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorUnsupportedAttributeType
		{
			get
			{
				return new LocalizedString("LdapFilterErrorUnsupportedAttributeType", "Ex4A2EBD", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionDnLimitExceeded(int actualCount, int limit)
		{
			return new LocalizedString("ExceptionDnLimitExceeded", "Ex4D1B00", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				actualCount,
				limit
			});
		}

		public static LocalizedString ExternalSenderAdminAddressRequired
		{
			get
			{
				return new LocalizedString("ExternalSenderAdminAddressRequired", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfigurationSettingsRestrictionNotExpected(string name)
		{
			return new LocalizedString("ConfigurationSettingsRestrictionNotExpected", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ErrorBadLocalizedFolderName
		{
			get
			{
				return new LocalizedString("ErrorBadLocalizedFolderName", "Ex096A6F", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoDatabaseMountDialBestAvailability
		{
			get
			{
				return new LocalizedString("AutoDatabaseMountDialBestAvailability", "Ex200EC7", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OrganizationalFolder
		{
			get
			{
				return new LocalizedString("OrganizationalFolder", "ExBAF4FF", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNotNullProperty(string propertyName)
		{
			return new LocalizedString("ErrorNotNullProperty", "Ex52EBD7", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				propertyName
			});
		}

		public static LocalizedString ErrorConversionFailedWithError(string name, uint err)
		{
			return new LocalizedString("ErrorConversionFailedWithError", "ExF4B43B", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				name,
				err
			});
		}

		public static LocalizedString ExceptionCannotBindToDomain(string domaincontroller, string domain, string errorCode)
		{
			return new LocalizedString("ExceptionCannotBindToDomain", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				domaincontroller,
				domain,
				errorCode
			});
		}

		public static LocalizedString SpamFilteringOptionTest
		{
			get
			{
				return new LocalizedString("SpamFilteringOptionTest", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionInvalidCredentialsFailedToGetIdentity(string server)
		{
			return new LocalizedString("ExceptionInvalidCredentialsFailedToGetIdentity", "ExF3DAEC", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString CannotCalculateProperty(string calculatedPropertyName, string errorMessage)
		{
			return new LocalizedString("CannotCalculateProperty", "Ex35C090", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				calculatedPropertyName,
				errorMessage
			});
		}

		public static LocalizedString ExceptionNotifyErrorGettingResults(string server)
		{
			return new LocalizedString("ExceptionNotifyErrorGettingResults", "ExB18FF0", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString ErrorSettingOverrideInvalidParameterSyntax(string componentName, string sectionName, string parameter)
		{
			return new LocalizedString("ErrorSettingOverrideInvalidParameterSyntax", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				componentName,
				sectionName,
				parameter
			});
		}

		public static LocalizedString LdapFilterErrorInvalidToken
		{
			get
			{
				return new LocalizedString("LdapFilterErrorInvalidToken", "ExB88801", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageRateSourceFlagsUser
		{
			get
			{
				return new LocalizedString("MessageRateSourceFlagsUser", "Ex5D2DFE", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WrongDelegationTypeForPolicy(string roleAssignment)
		{
			return new LocalizedString("WrongDelegationTypeForPolicy", "Ex44DDEC", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				roleAssignment
			});
		}

		public static LocalizedString TextEnrichedAndTextAlternative
		{
			get
			{
				return new LocalizedString("TextEnrichedAndTextAlternative", "Ex80E553", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionCreateLdapConnection(string server, string message, uint errorCode)
		{
			return new LocalizedString("ExceptionCreateLdapConnection", "Ex267C27", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				server,
				message,
				errorCode
			});
		}

		public static LocalizedString FederatedOrganizationIdNoFederatedDomains
		{
			get
			{
				return new LocalizedString("FederatedOrganizationIdNoFederatedDomains", "ExB63C8A", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionADTopologyErrorWhenLookingForTrustRelationships(int error, string fqdn, string message)
		{
			return new LocalizedString("ExceptionADTopologyErrorWhenLookingForTrustRelationships", "Ex289101", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				error,
				fqdn,
				message
			});
		}

		public static LocalizedString ErrorMailboxCollectionNotSupportType(string mailboxLocationType)
		{
			return new LocalizedString("ErrorMailboxCollectionNotSupportType", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				mailboxLocationType
			});
		}

		public static LocalizedString GroupTypeFlagsUniversal
		{
			get
			{
				return new LocalizedString("GroupTypeFlagsUniversal", "Ex4EB5B1", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomAlertTextRequired
		{
			get
			{
				return new LocalizedString("CustomAlertTextRequired", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionInvalidVlvFilterOption(string matchOption)
		{
			return new LocalizedString("ExceptionInvalidVlvFilterOption", "ExA0016D", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				matchOption
			});
		}

		public static LocalizedString EsnLangEstonian
		{
			get
			{
				return new LocalizedString("EsnLangEstonian", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Low
		{
			get
			{
				return new LocalizedString("Low", "ExFCE381", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IndustryPersonalServices
		{
			get
			{
				return new LocalizedString("IndustryPersonalServices", "Ex6209CE", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidPipelineTracingSenderAddress
		{
			get
			{
				return new LocalizedString("ErrorInvalidPipelineTracingSenderAddress", "Ex32A044", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AccessQuarantined
		{
			get
			{
				return new LocalizedString("AccessQuarantined", "Ex775044", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorTypeOnlySpaces
		{
			get
			{
				return new LocalizedString("LdapFilterErrorTypeOnlySpaces", "Ex210D48", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerSideADTopologyUnexpectedError(string server, string error)
		{
			return new LocalizedString("ServerSideADTopologyUnexpectedError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				server,
				error
			});
		}

		public static LocalizedString UserFilterChoice
		{
			get
			{
				return new LocalizedString("UserFilterChoice", "Ex5F282E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotBuildCapabilityFilterUnsupportedCapability(string capability)
		{
			return new LocalizedString("CannotBuildCapabilityFilterUnsupportedCapability", "Ex7B88DE", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				capability
			});
		}

		public static LocalizedString ExceptionInvalidVlvFilter(string filterType)
		{
			return new LocalizedString("ExceptionInvalidVlvFilter", "ExDA53B9", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				filterType
			});
		}

		public static LocalizedString ErrorRemovePrimaryExternalSMTPAddress
		{
			get
			{
				return new LocalizedString("ErrorRemovePrimaryExternalSMTPAddress", "Ex3EA3AC", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyOffice
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyOffice", "Ex3BB510", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorHostServerNotSet
		{
			get
			{
				return new LocalizedString("ErrorHostServerNotSet", "Ex3D0E53", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BitMaskOrIpAddressMatchMustBeSet
		{
			get
			{
				return new LocalizedString("BitMaskOrIpAddressMatchMustBeSet", "ExA1D701", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PerimeterSettingsAmbiguousException(string orgId)
		{
			return new LocalizedString("PerimeterSettingsAmbiguousException", "Ex92854D", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				orgId
			});
		}

		public static LocalizedString OrganizationCapabilityGMGen
		{
			get
			{
				return new LocalizedString("OrganizationCapabilityGMGen", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorArchiveDatabaseArchiveDomainConflict
		{
			get
			{
				return new LocalizedString("ErrorArchiveDatabaseArchiveDomainConflict", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ArchiveStateHostedProvisioned
		{
			get
			{
				return new LocalizedString("ArchiveStateHostedProvisioned", "Ex9DB0C4", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionCannotRemoveDsServer(string server)
		{
			return new LocalizedString("ExceptionCannotRemoveDsServer", "Ex4729E3", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString ExceptionCannotUseCredentials(TopologyMode topo)
		{
			return new LocalizedString("ExceptionCannotUseCredentials", "ExDEFCDE", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				topo
			});
		}

		public static LocalizedString InvalidHttpProtocolLogSizeConfiguration
		{
			get
			{
				return new LocalizedString("InvalidHttpProtocolLogSizeConfiguration", "ExFDC4E4", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionUnsupportedFilterForProperty(string propertyName, Type filterType, Type supportedType)
		{
			return new LocalizedString("ExceptionUnsupportedFilterForProperty", "Ex988CDB", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				propertyName,
				filterType,
				supportedType
			});
		}

		public static LocalizedString SuitabilityReachabilityError(string fqdn, string port, string details)
		{
			return new LocalizedString("SuitabilityReachabilityError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				fqdn,
				port,
				details
			});
		}

		public static LocalizedString PermanentMservErrorDescription
		{
			get
			{
				return new LocalizedString("PermanentMservErrorDescription", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomExternalBodyRequired
		{
			get
			{
				return new LocalizedString("CustomExternalBodyRequired", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorUndefinedAttributeType
		{
			get
			{
				return new LocalizedString("LdapFilterErrorUndefinedAttributeType", "Ex987FCC", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTextMessageIncludingHtmlBody
		{
			get
			{
				return new LocalizedString("ErrorTextMessageIncludingHtmlBody", "ExEB2B72", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WellKnownRecipientTypeResources
		{
			get
			{
				return new LocalizedString("WellKnownRecipientTypeResources", "Ex90AF45", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PrimaryDefault
		{
			get
			{
				return new LocalizedString("PrimaryDefault", "Ex9EFEF4", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailFlowPartnerInternalMailContentTypeMimeHtmlText
		{
			get
			{
				return new LocalizedString("MailFlowPartnerInternalMailContentTypeMimeHtmlText", "Ex456B24", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DataMoveReplicationConstraintNone
		{
			get
			{
				return new LocalizedString("DataMoveReplicationConstraintNone", "Ex5E0C04", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAdfsAudienceUris
		{
			get
			{
				return new LocalizedString("ErrorAdfsAudienceUris", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAnrFilter
		{
			get
			{
				return new LocalizedString("InvalidAnrFilter", "ExCA2BA5", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AuditLogMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("AuditLogMailboxRecipientTypeDetails", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WellKnownRecipientTypeNone
		{
			get
			{
				return new LocalizedString("WellKnownRecipientTypeNone", "ExC8B2E3", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangGujarati
		{
			get
			{
				return new LocalizedString("EsnLangGujarati", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DomainStateUnknown
		{
			get
			{
				return new LocalizedString("DomainStateUnknown", "Ex44041F", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IndustryManufacturing
		{
			get
			{
				return new LocalizedString("IndustryManufacturing", "ExB982E6", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IndustryHospitality
		{
			get
			{
				return new LocalizedString("IndustryHospitality", "Ex720546", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionInvalidOperationOnReadOnlySession(string operation)
		{
			return new LocalizedString("ExceptionInvalidOperationOnReadOnlySession", "Ex069AB3", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				operation
			});
		}

		public static LocalizedString ErrorAdfsIssuer
		{
			get
			{
				return new LocalizedString("ErrorAdfsIssuer", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmailAgeFilterOneDay
		{
			get
			{
				return new LocalizedString("EmailAgeFilterOneDay", "Ex13CBCE", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AllEmailMC
		{
			get
			{
				return new LocalizedString("AllEmailMC", "ExE78357", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfigurationSettingsRestrictionExpected(string name)
		{
			return new LocalizedString("ConfigurationSettingsRestrictionExpected", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString OrgContainerAmbiguousException
		{
			get
			{
				return new LocalizedString("OrgContainerAmbiguousException", "ExE63874", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionADTopologyServiceNotStarted(string server)
		{
			return new LocalizedString("ExceptionADTopologyServiceNotStarted", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString GlobalThrottlingPolicyNotFoundException
		{
			get
			{
				return new LocalizedString("GlobalThrottlingPolicyNotFoundException", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExEmptyStringArgumentException(string paramName)
		{
			return new LocalizedString("ExEmptyStringArgumentException", "ExC8F283", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				paramName
			});
		}

		public static LocalizedString ConstraintLocationValueReservedForSystemUse(string constraintNameValue)
		{
			return new LocalizedString("ConstraintLocationValueReservedForSystemUse", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				constraintNameValue
			});
		}

		public static LocalizedString EsnLangTurkish
		{
			get
			{
				return new LocalizedString("EsnLangTurkish", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToReadAlternateServiceAccountConfigFromRegistry(string keyPath)
		{
			return new LocalizedString("FailedToReadAlternateServiceAccountConfigFromRegistry", "ExC76F9D", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				keyPath
			});
		}

		public static LocalizedString ErrorGlobalWebDistributionAndVDirsSet(string name)
		{
			return new LocalizedString("ErrorGlobalWebDistributionAndVDirsSet", "Ex012158", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ServerComponentLocalRegistryError(string regErrorStr)
		{
			return new LocalizedString("ServerComponentLocalRegistryError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				regErrorStr
			});
		}

		public static LocalizedString SKUCapabilityBPOSSLite
		{
			get
			{
				return new LocalizedString("SKUCapabilityBPOSSLite", "Ex6A7867", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecipientWriteScopes
		{
			get
			{
				return new LocalizedString("RecipientWriteScopes", "Ex4D4E9F", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarAgeFilterThreeMonths
		{
			get
			{
				return new LocalizedString("CalendarAgeFilterThreeMonths", "Ex5D7DB9", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxMoveStatusCompletedWithWarning
		{
			get
			{
				return new LocalizedString("MailboxMoveStatusCompletedWithWarning", "ExFA97CF", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCountryOrRegion
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCountryOrRegion", "ExB259C2", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangFrench
		{
			get
			{
				return new LocalizedString("EsnLangFrench", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TooManyKeyMappings(string a)
		{
			return new LocalizedString("TooManyKeyMappings", "ExDA6DBD", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				a
			});
		}

		public static LocalizedString CapabilityExcludedFromBackSync
		{
			get
			{
				return new LocalizedString("CapabilityExcludedFromBackSync", "Ex7E8F7B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CapabilityBEVDirLockdown
		{
			get
			{
				return new LocalizedString("CapabilityBEVDirLockdown", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ScopeCannotBeExclusive(ScopeRestrictionType scopeType)
		{
			return new LocalizedString("ScopeCannotBeExclusive", "ExF616B9", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				scopeType
			});
		}

		public static LocalizedString UnsupportedADSyntaxException(string syntax)
		{
			return new LocalizedString("UnsupportedADSyntaxException", "ExCD0111", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				syntax
			});
		}

		public static LocalizedString CannotFindOabException(string oabId)
		{
			return new LocalizedString("CannotFindOabException", "Ex0A505A", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				oabId
			});
		}

		public static LocalizedString ExceptionInvalidOperationOnInvalidSession(string operation)
		{
			return new LocalizedString("ExceptionInvalidOperationOnInvalidSession", "Ex6ADB8F", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				operation
			});
		}

		public static LocalizedString ReceiveAuthMechanismBasicAuth
		{
			get
			{
				return new LocalizedString("ReceiveAuthMechanismBasicAuth", "Ex01BF69", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IndustryEducation
		{
			get
			{
				return new LocalizedString("IndustryEducation", "ExD5213F", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorProperty1EqValue1WhileProperty2EqValue2(string property1Name, string value1, string property2Name, string value2)
		{
			return new LocalizedString("ErrorProperty1EqValue1WhileProperty2EqValue2", "ExF0EF89", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				property1Name,
				value1,
				property2Name,
				value2
			});
		}

		public static LocalizedString NotSpecified
		{
			get
			{
				return new LocalizedString("NotSpecified", "ExA5AF6D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PermanentlyDelete
		{
			get
			{
				return new LocalizedString("PermanentlyDelete", "ExBDB7B1", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FederatedIdentityMisconfigured
		{
			get
			{
				return new LocalizedString("FederatedIdentityMisconfigured", "ExAD915C", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSettingOverrideInvalidComponentName(string componentName, string availableComponents)
		{
			return new LocalizedString("ErrorSettingOverrideInvalidComponentName", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				componentName,
				availableComponents
			});
		}

		public static LocalizedString MountDialOverrideNone
		{
			get
			{
				return new LocalizedString("MountDialOverrideNone", "Ex5F8A1B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlwaysUTF8
		{
			get
			{
				return new LocalizedString("AlwaysUTF8", "Ex3FF45F", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionPagedReaderIsSingleUse
		{
			get
			{
				return new LocalizedString("ExceptionPagedReaderIsSingleUse", "ExAE75BD", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidFilterLength
		{
			get
			{
				return new LocalizedString("InvalidFilterLength", "ExD5D91B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxMoveStatusSynced
		{
			get
			{
				return new LocalizedString("MailboxMoveStatusSynced", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecipientWriteScopeNotLessThanBecauseOfDelegationFlags(string leftScopeType, string rightScopeType)
		{
			return new LocalizedString("RecipientWriteScopeNotLessThanBecauseOfDelegationFlags", "ExCEDFCB", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				leftScopeType,
				rightScopeType
			});
		}

		public static LocalizedString SIPSecured
		{
			get
			{
				return new LocalizedString("SIPSecured", "Ex71C298", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRejectedCookie
		{
			get
			{
				return new LocalizedString("ErrorRejectedCookie", "Ex28C6DD", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TooManyDataInLdapProperty(string ldapPropertyName, int maxCount)
		{
			return new LocalizedString("TooManyDataInLdapProperty", "Ex96AA64", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				ldapPropertyName,
				maxCount
			});
		}

		public static LocalizedString ASInvalidProxyASUrlOption
		{
			get
			{
				return new LocalizedString("ASInvalidProxyASUrlOption", "Ex434DE7", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidMailboxProvisioningAttributes(int maximumAllowedAttributes)
		{
			return new LocalizedString("ErrorInvalidMailboxProvisioningAttributes", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				maximumAllowedAttributes
			});
		}

		public static LocalizedString ServerRoleSCOM
		{
			get
			{
				return new LocalizedString("ServerRoleSCOM", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionUnsupportedOperatorForProperty(string propertyName, string operatorName)
		{
			return new LocalizedString("ExceptionUnsupportedOperatorForProperty", "Ex1F6696", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				propertyName,
				operatorName
			});
		}

		public static LocalizedString JournalItemsMC
		{
			get
			{
				return new LocalizedString("JournalItemsMC", "Ex184082", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTargetPartitionSctMissing(string oldTenant, string newPartition, string sct)
		{
			return new LocalizedString("ErrorTargetPartitionSctMissing", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				oldTenant,
				newPartition,
				sct
			});
		}

		public static LocalizedString TenantTransportSettingsNotFoundException(string orgId)
		{
			return new LocalizedString("TenantTransportSettingsNotFoundException", "Ex9564DC", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				orgId
			});
		}

		public static LocalizedString ErrorNonUniqueLegacyDN(string legacyDN)
		{
			return new LocalizedString("ErrorNonUniqueLegacyDN", "ExEB64EC", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				legacyDN
			});
		}

		public static LocalizedString ErrorAccountPartitionCantBeLocalAndSecondaryAtTheSameTime(string id)
		{
			return new LocalizedString("ErrorAccountPartitionCantBeLocalAndSecondaryAtTheSameTime", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ErrorEmptySearchProperty
		{
			get
			{
				return new LocalizedString("ErrorEmptySearchProperty", "ExD5949E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionADTopologyTimeoutCall(string server, string error)
		{
			return new LocalizedString("ExceptionADTopologyTimeoutCall", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				server,
				error
			});
		}

		public static LocalizedString ExceptionUnsupportedOperator(string operatorName, string filterType)
		{
			return new LocalizedString("ExceptionUnsupportedOperator", "Ex00F5CF", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				operatorName,
				filterType
			});
		}

		public static LocalizedString NoMatchingTenantInTargetPartition(string oldTenant, string newPartition, string guid)
		{
			return new LocalizedString("NoMatchingTenantInTargetPartition", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				oldTenant,
				newPartition,
				guid
			});
		}

		public static LocalizedString RootMustBeEmpty(ScopeRestrictionType scopeType)
		{
			return new LocalizedString("RootMustBeEmpty", "ExFE601A", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				scopeType
			});
		}

		public static LocalizedString OutboundConnectorIncorrectTransportRuleScopedParameters
		{
			get
			{
				return new LocalizedString("OutboundConnectorIncorrectTransportRuleScopedParameters", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("TeamMailboxRecipientTypeDetails", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomRoleDescription_MyMobileInformation
		{
			get
			{
				return new LocalizedString("CustomRoleDescription_MyMobileInformation", "ExC0AC83", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ArchiveStateHostedPending
		{
			get
			{
				return new LocalizedString("ArchiveStateHostedPending", "Ex2CD23D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DPCantChangeName
		{
			get
			{
				return new LocalizedString("DPCantChangeName", "Ex0147D6", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OrganizationCapabilityUMDataStorage
		{
			get
			{
				return new LocalizedString("OrganizationCapabilityUMDataStorage", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TlsAuthLevelWithRequireTlsDisabled
		{
			get
			{
				return new LocalizedString("TlsAuthLevelWithRequireTlsDisabled", "Ex7A63EC", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UndefinedRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("UndefinedRecipientTypeDetails", "Ex8AA2FD", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Upgrade
		{
			get
			{
				return new LocalizedString("Upgrade", "ExF45929", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Global
		{
			get
			{
				return new LocalizedString("Global", "ExEA06B3", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeleteMessage
		{
			get
			{
				return new LocalizedString("DeleteMessage", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapDelete
		{
			get
			{
				return new LocalizedString("LdapDelete", "Ex11D92C", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangHungarian
		{
			get
			{
				return new LocalizedString("EsnLangHungarian", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionValueNotPresent(string propertyName, string objectName)
		{
			return new LocalizedString("ExceptionValueNotPresent", "ExBBAFE5", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				propertyName,
				objectName
			});
		}

		public static LocalizedString ErrorAddressAutoCopy
		{
			get
			{
				return new LocalizedString("ErrorAddressAutoCopy", "ExE5CBF5", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangLatvian
		{
			get
			{
				return new LocalizedString("EsnLangLatvian", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMinAdminVersionOutOfSync(int minAdminVersion, ExchangeObjectVersion currentVersion, int correctMinAdminVersion)
		{
			return new LocalizedString("ErrorMinAdminVersionOutOfSync", "Ex02F649", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				minAdminVersion,
				currentVersion,
				correctMinAdminVersion
			});
		}

		public static LocalizedString CanRunDefaultUpdateState_NotLocal
		{
			get
			{
				return new LocalizedString("CanRunDefaultUpdateState_NotLocal", "Ex16C492", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Department
		{
			get
			{
				return new LocalizedString("Department", "Ex584E88", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MasteredOnPremiseCapabilityUndefinedNotTenant(string capability)
		{
			return new LocalizedString("MasteredOnPremiseCapabilityUndefinedNotTenant", "ExB23FD3", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				capability
			});
		}

		public static LocalizedString ExceptionUnsupportedTextFilterOption(string option)
		{
			return new LocalizedString("ExceptionUnsupportedTextFilterOption", "Ex748AA9", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				option
			});
		}

		public static LocalizedString SpamFilteringActionJmf
		{
			get
			{
				return new LocalizedString("SpamFilteringActionJmf", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorServiceAccountThrottlingPolicy(string scope)
		{
			return new LocalizedString("ErrorServiceAccountThrottlingPolicy", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				scope
			});
		}

		public static LocalizedString ErrorDDLOperationsError
		{
			get
			{
				return new LocalizedString("ErrorDDLOperationsError", "ExC1558E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSharedConfigurationCannotBeEnabled
		{
			get
			{
				return new LocalizedString("ErrorSharedConfigurationCannotBeEnabled", "Ex37918C", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMailTipCultureNotSpecified
		{
			get
			{
				return new LocalizedString("ErrorMailTipCultureNotSpecified", "Ex918659", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDLAsBothAcceptedAndRejected(string dl)
		{
			return new LocalizedString("ErrorDLAsBothAcceptedAndRejected", "Ex3CB891", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				dl
			});
		}

		public static LocalizedString ErrorSettingOverrideSyntax(string componentName, string sectionName, string parameters, string error)
		{
			return new LocalizedString("ErrorSettingOverrideSyntax", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				componentName,
				sectionName,
				parameters,
				error
			});
		}

		public static LocalizedString PermanentMservError(string message)
		{
			return new LocalizedString("PermanentMservError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString LdapModify
		{
			get
			{
				return new LocalizedString("LdapModify", "Ex983C72", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DataMoveReplicationConstraintSecondDatacenter
		{
			get
			{
				return new LocalizedString("DataMoveReplicationConstraintSecondDatacenter", "Ex20CA37", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CapabilityResourceMailbox
		{
			get
			{
				return new LocalizedString("CapabilityResourceMailbox", "Ex75E71B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidCapabilityOnMailboxPlan(string currentCapability, string skuCapabilities)
		{
			return new LocalizedString("InvalidCapabilityOnMailboxPlan", "ExC0F98E", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				currentCapability,
				skuCapabilities
			});
		}

		public static LocalizedString Second
		{
			get
			{
				return new LocalizedString("Second", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboundConnectorInvalidRestrictDomainsToCertificate
		{
			get
			{
				return new LocalizedString("InboundConnectorInvalidRestrictDomainsToCertificate", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute15
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute15", "ExE36B6F", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SendAuthMechanismNone
		{
			get
			{
				return new LocalizedString("SendAuthMechanismNone", "ExD84EDF", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServicesContainerNotFound
		{
			get
			{
				return new LocalizedString("ServicesContainerNotFound", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingDefaultOutboundCallingLineId
		{
			get
			{
				return new LocalizedString("MissingDefaultOutboundCallingLineId", "ExCE5E1F", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupTypeFlagsDomainLocal
		{
			get
			{
				return new LocalizedString("GroupTypeFlagsDomainLocal", "Ex986159", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SharedConfigurationVersionNotSupported(string tinyTenant, string version)
		{
			return new LocalizedString("SharedConfigurationVersionNotSupported", "ExB4A8CC", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				tinyTenant,
				version
			});
		}

		public static LocalizedString ErrorCannotAggregateAndLinkMailbox
		{
			get
			{
				return new LocalizedString("ErrorCannotAggregateAndLinkMailbox", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SyncCommands
		{
			get
			{
				return new LocalizedString("SyncCommands", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OwaAdOrphanFound(string id)
		{
			return new LocalizedString("OwaAdOrphanFound", "ExCFCCE5", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ExceptionRUSServerDown(string server)
		{
			return new LocalizedString("ExceptionRUSServerDown", "ExF5CFA4", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString PreferredInternetCodePageEsc2022Jp
		{
			get
			{
				return new LocalizedString("PreferredInternetCodePageEsc2022Jp", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DefaultRoutingGroupNotFoundException(string rgName)
		{
			return new LocalizedString("DefaultRoutingGroupNotFoundException", "ExEC1E62", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				rgName
			});
		}

		public static LocalizedString DirectoryBasedEdgeBlockModeOff
		{
			get
			{
				return new LocalizedString("DirectoryBasedEdgeBlockModeOff", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSourceAddressSetting
		{
			get
			{
				return new LocalizedString("InvalidSourceAddressSetting", "Ex51BAE5", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ElcContentSettingsDescription
		{
			get
			{
				return new LocalizedString("ElcContentSettingsDescription", "Ex0E091C", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleUnifiedMessaging
		{
			get
			{
				return new LocalizedString("ServerRoleUnifiedMessaging", "Ex8F8134", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DataMoveReplicationConstraintCIAllCopies
		{
			get
			{
				return new LocalizedString("DataMoveReplicationConstraintCIAllCopies", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailTipsAccessLevelLimited
		{
			get
			{
				return new LocalizedString("MailTipsAccessLevelLimited", "Ex919781", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SecondaryMailboxRelationType
		{
			get
			{
				return new LocalizedString("SecondaryMailboxRelationType", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Ocs
		{
			get
			{
				return new LocalizedString("Ocs", "Ex7E09BF", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IndustryOther
		{
			get
			{
				return new LocalizedString("IndustryOther", "Ex357EFD", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMimeMessageIncludingUuEncodedAttachment
		{
			get
			{
				return new LocalizedString("ErrorMimeMessageIncludingUuEncodedAttachment", "Ex1ABC61", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionCopyChangesForIncompatibleTypes(Type type1, Type type2)
		{
			return new LocalizedString("ExceptionCopyChangesForIncompatibleTypes", "ExCF37A6", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				type1,
				type2
			});
		}

		public static LocalizedString ServerRoleDHCP
		{
			get
			{
				return new LocalizedString("ServerRoleDHCP", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute5
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute5", "ExB92AC6", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSettingOverrideInvalidParameterName(string componentName, string sectionName, string parameterName, string availableParameterNames)
		{
			return new LocalizedString("ErrorSettingOverrideInvalidParameterName", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				componentName,
				sectionName,
				parameterName,
				availableParameterNames
			});
		}

		public static LocalizedString EnableNotificationEmail
		{
			get
			{
				return new LocalizedString("EnableNotificationEmail", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCountryCode
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCountryCode", "ExE7BFF9", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxMoveStatusCompleted
		{
			get
			{
				return new LocalizedString("MailboxMoveStatusCompleted", "Ex3E6503", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RangePropertyResponseDoesNotContainRangeInformation(string str)
		{
			return new LocalizedString("RangePropertyResponseDoesNotContainRangeInformation", "Ex93BF72", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				str
			});
		}

		public static LocalizedString ExceptionSeverNotInPartition(string serverName, string partitionName)
		{
			return new LocalizedString("ExceptionSeverNotInPartition", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				serverName,
				partitionName
			});
		}

		public static LocalizedString IndustryCommunications
		{
			get
			{
				return new LocalizedString("IndustryCommunications", "Ex460042", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorNoValidComparison
		{
			get
			{
				return new LocalizedString("LdapFilterErrorNoValidComparison", "Ex6AEFD1", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotResolvePartitionFqdnFromAccountForestDnError(string dn)
		{
			return new LocalizedString("CannotResolvePartitionFqdnFromAccountForestDnError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				dn
			});
		}

		public static LocalizedString RssSubscriptions
		{
			get
			{
				return new LocalizedString("RssSubscriptions", "Ex870DD4", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSettingOverrideUnexpected(string errorType)
		{
			return new LocalizedString("ErrorSettingOverrideUnexpected", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				errorType
			});
		}

		public static LocalizedString OrgWideDelegatingWriteScopeMustBeTheSameAsRoleImplicitWriteScope(RecipientWriteScopeType scopeType)
		{
			return new LocalizedString("OrgWideDelegatingWriteScopeMustBeTheSameAsRoleImplicitWriteScope", "Ex3D27BC", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				scopeType
			});
		}

		public static LocalizedString InvalidPhrase(string listName, int length)
		{
			return new LocalizedString("InvalidPhrase", "Ex54C462", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				listName,
				length
			});
		}

		public static LocalizedString EsnLangThai
		{
			get
			{
				return new LocalizedString("EsnLangThai", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRealmNotFound(string realm)
		{
			return new LocalizedString("ErrorRealmNotFound", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				realm
			});
		}

		public static LocalizedString ErrorDDLFilterMissing
		{
			get
			{
				return new LocalizedString("ErrorDDLFilterMissing", "Ex4EFFD9", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExtendedProtectionNonTlsTerminatingProxyScenarioRequireTls
		{
			get
			{
				return new LocalizedString("ExtendedProtectionNonTlsTerminatingProxyScenarioRequireTls", "ExA5D67C", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorLogonFailuresBeforePINReset(int logonFailuresBeforePINReset, string maxLogonAttempts)
		{
			return new LocalizedString("ErrorLogonFailuresBeforePINReset", "ExDA4E3D", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				logonFailuresBeforePINReset,
				maxLogonAttempts
			});
		}

		public static LocalizedString TenantNotFoundInMservError(string tenant)
		{
			return new LocalizedString("TenantNotFoundInMservError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				tenant
			});
		}

		public static LocalizedString NoResetOrAssignedMvp
		{
			get
			{
				return new LocalizedString("NoResetOrAssignedMvp", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MountDialOverrideBestEffort
		{
			get
			{
				return new LocalizedString("MountDialOverrideBestEffort", "ExD53E80", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorIsServerInMaintenanceMode(string fqdn)
		{
			return new LocalizedString("ErrorIsServerInMaintenanceMode", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				fqdn
			});
		}

		public static LocalizedString ExceptionADOperationFailed(string server, string errorMessage)
		{
			return new LocalizedString("ExceptionADOperationFailed", "Ex6AE46B", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				server,
				errorMessage
			});
		}

		public static LocalizedString NoComputers
		{
			get
			{
				return new LocalizedString("NoComputers", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfigWriteScopeNotLessThanBecauseOfDelegationFlags(string leftScopeType, string rightScopeType)
		{
			return new LocalizedString("ConfigWriteScopeNotLessThanBecauseOfDelegationFlags", "ExAE2E51", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				leftScopeType,
				rightScopeType
			});
		}

		public static LocalizedString RegistryContentTypeException
		{
			get
			{
				return new LocalizedString("RegistryContentTypeException", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DataMoveReplicationConstraintAllDatacenters
		{
			get
			{
				return new LocalizedString("DataMoveReplicationConstraintAllDatacenters", "Ex3525F0", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionObjectNotFound
		{
			get
			{
				return new LocalizedString("ExceptionObjectNotFound", "Ex7571B9", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DomainStateCustomProvision
		{
			get
			{
				return new LocalizedString("DomainStateCustomProvision", "ExACD43A", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SKUCapabilityBPOSMidSize
		{
			get
			{
				return new LocalizedString("SKUCapabilityBPOSMidSize", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorUnsupportedOperand
		{
			get
			{
				return new LocalizedString("LdapFilterErrorUnsupportedOperand", "Ex3BC457", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DirectoryBasedEdgeBlockModeDefault
		{
			get
			{
				return new LocalizedString("DirectoryBasedEdgeBlockModeDefault", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SwapShouldNotChangeValues(string oldValue, byte oldRid, string newValue, byte newRid)
		{
			return new LocalizedString("SwapShouldNotChangeValues", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				oldValue,
				oldRid,
				newValue,
				newRid
			});
		}

		public static LocalizedString ConfigurationSettingsHistorySummary(string name, int count)
		{
			return new LocalizedString("ConfigurationSettingsHistorySummary", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				name,
				count
			});
		}

		public static LocalizedString ErrorWrongTypeParameter
		{
			get
			{
				return new LocalizedString("ErrorWrongTypeParameter", "ExA51C26", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangCatalan
		{
			get
			{
				return new LocalizedString("EsnLangCatalan", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSndProtocolLogSizeConfiguration
		{
			get
			{
				return new LocalizedString("InvalidSndProtocolLogSizeConfiguration", "Ex2E47CB", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyCustomAttribute13
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyCustomAttribute13", "Ex448D4C", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorThrottlingPolicyGlobalAndOrganizationScope
		{
			get
			{
				return new LocalizedString("ErrorThrottlingPolicyGlobalAndOrganizationScope", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SMTPAddress
		{
			get
			{
				return new LocalizedString("SMTPAddress", "Ex8A0496", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionUnsupportedPropertyValue(string propertyName, object value)
		{
			return new LocalizedString("ExceptionUnsupportedPropertyValue", "Ex22FD94", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				propertyName,
				value
			});
		}

		public static LocalizedString EsnLangPolish
		{
			get
			{
				return new LocalizedString("EsnLangPolish", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CanEnableLocalCopyState_DatabaseEnabled
		{
			get
			{
				return new LocalizedString("CanEnableLocalCopyState_DatabaseEnabled", "ExC31189", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangRomanian
		{
			get
			{
				return new LocalizedString("EsnLangRomanian", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalManagedGroupTypeDetails
		{
			get
			{
				return new LocalizedString("ExternalManagedGroupTypeDetails", "Ex5EA363", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseMasterTypeDag
		{
			get
			{
				return new LocalizedString("DatabaseMasterTypeDag", "Ex763399", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionNetLogonOperation(string netLogonOperation, string domain)
		{
			return new LocalizedString("ExceptionNetLogonOperation", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				netLogonOperation,
				domain
			});
		}

		public static LocalizedString GroupNamingPolicyExtensionCustomAttribute3
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyExtensionCustomAttribute3", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionADTopologyNoSuchDomain(string domain)
		{
			return new LocalizedString("ExceptionADTopologyNoSuchDomain", "ExCD40EE", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString InvalidNumberOfCapabilitiesOnMailboxPlan(string skuCapabilities)
		{
			return new LocalizedString("InvalidNumberOfCapabilitiesOnMailboxPlan", "Ex3A8F0D", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				skuCapabilities
			});
		}

		public static LocalizedString ExchangeConfigurationContainerNotFoundException
		{
			get
			{
				return new LocalizedString("ExchangeConfigurationContainerNotFoundException", "Ex63E670", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangUrdu
		{
			get
			{
				return new LocalizedString("EsnLangUrdu", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MservAndMbxExclusive
		{
			get
			{
				return new LocalizedString("MservAndMbxExclusive", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FirstLast
		{
			get
			{
				return new LocalizedString("FirstLast", "Ex28BC2F", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangBulgarian
		{
			get
			{
				return new LocalizedString("EsnLangBulgarian", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailEnabledUniversalSecurityGroupRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("MailEnabledUniversalSecurityGroupRecipientTypeDetails", "Ex060A04", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionReadOnlyBecauseTooNew(ExchangeObjectVersion objectVersion, ExchangeObjectVersion currentVersion)
		{
			return new LocalizedString("ExceptionReadOnlyBecauseTooNew", "ExF977EA", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				objectVersion,
				currentVersion
			});
		}

		public static LocalizedString ErrorTimeoutReadingSystemAddressListMemberCount
		{
			get
			{
				return new LocalizedString("ErrorTimeoutReadingSystemAddressListMemberCount", "Ex4388F6", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FaxServerURINoValue
		{
			get
			{
				return new LocalizedString("FaxServerURINoValue", "Ex8F5C5F", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDefaultThrottlingPolicyNotFound
		{
			get
			{
				return new LocalizedString("ErrorDefaultThrottlingPolicyNotFound", "ExF7CACF", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotGetSiteInfo(string error)
		{
			return new LocalizedString("CannotGetSiteInfo", "Ex25AC16", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString UnableToDeserializeXMLError(string errorStr)
		{
			return new LocalizedString("UnableToDeserializeXMLError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				errorStr
			});
		}

		public static LocalizedString ConfigurationSettingsRestrictionMissingProperty(string name, string propertyName)
		{
			return new LocalizedString("ConfigurationSettingsRestrictionMissingProperty", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				name,
				propertyName
			});
		}

		public static LocalizedString ErrorRecipientContainerCanNotNull
		{
			get
			{
				return new LocalizedString("ErrorRecipientContainerCanNotNull", "ExB42B18", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MoveToArchive
		{
			get
			{
				return new LocalizedString("MoveToArchive", "ExC6E83C", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfigurationSettingsInvalidScopeFilter(string error)
		{
			return new LocalizedString("ConfigurationSettingsInvalidScopeFilter", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString ModifySubjectValueNotSet
		{
			get
			{
				return new LocalizedString("ModifySubjectValueNotSet", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OwaMetabasePathIsInvalid(string id, string path)
		{
			return new LocalizedString("OwaMetabasePathIsInvalid", "Ex1E041E", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				id,
				path
			});
		}

		public static LocalizedString NotLocalMaiboxException
		{
			get
			{
				return new LocalizedString("NotLocalMaiboxException", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDNStringFormat(string str)
		{
			return new LocalizedString("InvalidDNStringFormat", "ExDDC2EA", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				str
			});
		}

		public static LocalizedString RecipientReadScope
		{
			get
			{
				return new LocalizedString("RecipientReadScope", "ExD041A7", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionADTopologyNoServersForPartition(string partition)
		{
			return new LocalizedString("ExceptionADTopologyNoServersForPartition", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				partition
			});
		}

		public static LocalizedString TenantAlreadyExistsInMserv(Guid tenantGuid, int existingPartnerId, int localSitePartnerId)
		{
			return new LocalizedString("TenantAlreadyExistsInMserv", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				tenantGuid,
				existingPartnerId,
				localSitePartnerId
			});
		}

		public static LocalizedString Organizational
		{
			get
			{
				return new LocalizedString("Organizational", "Ex752504", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionUnsupportedDefaultValueFilter(string propertyName, string operatorName, string value)
		{
			return new LocalizedString("ExceptionUnsupportedDefaultValueFilter", "Ex512393", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				propertyName,
				operatorName,
				value
			});
		}

		public static LocalizedString SystemAttendantMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("SystemAttendantMailboxRecipientTypeDetails", "Ex3BEE54", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidResourceThresholdBetweenClassifications(string thresholdName, string classification1, string classification2, int value1, int value2)
		{
			return new LocalizedString("InvalidResourceThresholdBetweenClassifications", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				thresholdName,
				classification1,
				classification2,
				value1,
				value2
			});
		}

		public static LocalizedString OrganizationCapabilityOABGen
		{
			get
			{
				return new LocalizedString("OrganizationCapabilityOABGen", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StarOutToDialPlanEnabled
		{
			get
			{
				return new LocalizedString("StarOutToDialPlanEnabled", "ExA00A9E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AuthenticationCredentialNotSet
		{
			get
			{
				return new LocalizedString("AuthenticationCredentialNotSet", "ExA19CDE", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotifyOutboundSpamRecipientsRequired
		{
			get
			{
				return new LocalizedString("NotifyOutboundSpamRecipientsRequired", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmail
		{
			get
			{
				return new LocalizedString("JunkEmail", "Ex426E42", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorValueOnlySpaces
		{
			get
			{
				return new LocalizedString("LdapFilterErrorValueOnlySpaces", "Ex8168A0", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SipName
		{
			get
			{
				return new LocalizedString("SipName", "Ex04214A", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangMalayalam
		{
			get
			{
				return new LocalizedString("EsnLangMalayalam", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpamFilteringActionModifySubject
		{
			get
			{
				return new LocalizedString("SpamFilteringActionModifySubject", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString XHeaderValueNotSet
		{
			get
			{
				return new LocalizedString("XHeaderValueNotSet", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeletedItems
		{
			get
			{
				return new LocalizedString("DeletedItems", "ExE37A57", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionInvalidAddressFormat(string address)
		{
			return new LocalizedString("ExceptionInvalidAddressFormat", "Ex903F52", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString ExceptionMostDerivedOnBase(string objectName)
		{
			return new LocalizedString("ExceptionMostDerivedOnBase", "Ex7C5148", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				objectName
			});
		}

		public static LocalizedString ExceptionOwaCannotSetPropertyOnE12VirtualDirectoryToNull(string property)
		{
			return new LocalizedString("ExceptionOwaCannotSetPropertyOnE12VirtualDirectoryToNull", "Ex934429", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString OrganizationCapabilityUMGrammarReady
		{
			get
			{
				return new LocalizedString("OrganizationCapabilityUMGrammarReady", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LastFirst
		{
			get
			{
				return new LocalizedString("LastFirst", "Ex9B48A8", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCopySystemFolderPathNotEqualCopyLogFolderPath(NonRootLocalLongFullPath cpySysPath, NonRootLocalLongFullPath cpyLogPath)
		{
			return new LocalizedString("ErrorCopySystemFolderPathNotEqualCopyLogFolderPath", "Ex608DB3", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				cpySysPath,
				cpyLogPath
			});
		}

		public static LocalizedString ConfigurationSettingsUnsupportedVersion(string version)
		{
			return new LocalizedString("ConfigurationSettingsUnsupportedVersion", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				version
			});
		}

		public static LocalizedString SendAuthMechanismExchangeServer
		{
			get
			{
				return new LocalizedString("SendAuthMechanismExchangeServer", "Ex8D4B00", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteTeamMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("RemoteTeamMailboxRecipientTypeDetails", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OutOfBudgets
		{
			get
			{
				return new LocalizedString("OutOfBudgets", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Off
		{
			get
			{
				return new LocalizedString("Off", "Ex7DB229", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidOpathFilter(string filter)
		{
			return new LocalizedString("ErrorInvalidOpathFilter", "Ex7CFE2C", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				filter
			});
		}

		public static LocalizedString GroupTypeFlagsSecurityEnabled
		{
			get
			{
				return new LocalizedString("GroupTypeFlagsSecurityEnabled", "ExAE6448", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidCookieException
		{
			get
			{
				return new LocalizedString("InvalidCookieException", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WrongAssigneeTypeForPolicyOrPartnerApplication(string roleAssignment)
		{
			return new LocalizedString("WrongAssigneeTypeForPolicyOrPartnerApplication", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				roleAssignment
			});
		}

		public static LocalizedString WrongScopeForCurrentPartition(string scopeDN, string partitionFqdn)
		{
			return new LocalizedString("WrongScopeForCurrentPartition", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				scopeDN,
				partitionFqdn
			});
		}

		public static LocalizedString UserLanguageChoice
		{
			get
			{
				return new LocalizedString("UserLanguageChoice", "Ex91975E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpamFilteringTestActionBccMessage
		{
			get
			{
				return new LocalizedString("SpamFilteringTestActionBccMessage", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidLegacyRdnPrefix(string prefix)
		{
			return new LocalizedString("ErrorInvalidLegacyRdnPrefix", "ExCF04C9", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				prefix
			});
		}

		public static LocalizedString DelayCacheFull
		{
			get
			{
				return new LocalizedString("DelayCacheFull", "Ex0EBFE8", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAutoCopyMessageFormat
		{
			get
			{
				return new LocalizedString("ErrorAutoCopyMessageFormat", "Ex55AEF3", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidIdFormat(string str)
		{
			return new LocalizedString("InvalidIdFormat", "Ex0BB379", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				str
			});
		}

		public static LocalizedString Reserved3
		{
			get
			{
				return new LocalizedString("Reserved3", "Ex79ABC9", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HtmlOnly
		{
			get
			{
				return new LocalizedString("HtmlOnly", "Ex43F6E0", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LocalServerNotFound(string fqdn)
		{
			return new LocalizedString("LocalServerNotFound", "Ex28CE91", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				fqdn
			});
		}

		public static LocalizedString DefaultFolder
		{
			get
			{
				return new LocalizedString("DefaultFolder", "Ex578F5D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorResultsAreNonUnique(string filter)
		{
			return new LocalizedString("ErrorResultsAreNonUnique", "ExD53968", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				filter
			});
		}

		public static LocalizedString PublicFolderMailboxRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("PublicFolderMailboxRecipientTypeDetails", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OrganizationMailboxNotFound(string id)
		{
			return new LocalizedString("OrganizationMailboxNotFound", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString Mp3
		{
			get
			{
				return new LocalizedString("Mp3", "Ex574040", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionUnsupportedPropertyValueType(string propertyName, Type valueType, Type supportedType)
		{
			return new LocalizedString("ExceptionUnsupportedPropertyValueType", "ExCF8596", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				propertyName,
				valueType,
				supportedType
			});
		}

		public static LocalizedString FederatedOrganizationIdNotEnabled
		{
			get
			{
				return new LocalizedString("FederatedOrganizationIdNotEnabled", "Ex325103", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionInvalidBitwiseComparison(string propertyName, string operatorName)
		{
			return new LocalizedString("ExceptionInvalidBitwiseComparison", "ExFE6432", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				propertyName,
				operatorName
			});
		}

		public static LocalizedString EsnLangVietnamese
		{
			get
			{
				return new LocalizedString("EsnLangVietnamese", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AccessGranted
		{
			get
			{
				return new LocalizedString("AccessGranted", "ExAFD833", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxUserRecipientType
		{
			get
			{
				return new LocalizedString("MailboxUserRecipientType", "ExC81C49", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotFindTemplateUser(string dn)
		{
			return new LocalizedString("CannotFindTemplateUser", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				dn
			});
		}

		public static LocalizedString ErrorNoSuitableGC(string server, string forest)
		{
			return new LocalizedString("ErrorNoSuitableGC", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				server,
				forest
			});
		}

		public static LocalizedString ExceptionNoSchemaContainerObject
		{
			get
			{
				return new LocalizedString("ExceptionNoSchemaContainerObject", "Ex125C4F", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TargetDeliveryDomainCannotBeStar
		{
			get
			{
				return new LocalizedString("TargetDeliveryDomainCannotBeStar", "ExD5728C", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAuthMetadataCannotResolveServiceName
		{
			get
			{
				return new LocalizedString("ErrorAuthMetadataCannotResolveServiceName", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ByteEncoderTypeUseBase64
		{
			get
			{
				return new LocalizedString("ByteEncoderTypeUseBase64", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionServerUnavailable(string serverName)
		{
			return new LocalizedString("ExceptionServerUnavailable", "Ex99F94E", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString CannotGetDomainFromDN(string dn)
		{
			return new LocalizedString("CannotGetDomainFromDN", "ExA3F6DB", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				dn
			});
		}

		public static LocalizedString FederatedUser_Violation(string objectName, string cmdLet, string parameters, string capabilities)
		{
			return new LocalizedString("FederatedUser_Violation", "ExD9DDB9", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				objectName,
				cmdLet,
				parameters,
				capabilities
			});
		}

		public static LocalizedString BPOS_Feature_UsageLocation_Violation(string objectName, string cmdLet, string parameters, string capabilities)
		{
			return new LocalizedString("BPOS_Feature_UsageLocation_Violation", "Ex721F28", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				objectName,
				cmdLet,
				parameters,
				capabilities
			});
		}

		public static LocalizedString BackSyncDataSourceReplicationErrorMessage
		{
			get
			{
				return new LocalizedString("BackSyncDataSourceReplicationErrorMessage", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidMaxOutboundConnectionConfiguration(string value1, string value2)
		{
			return new LocalizedString("InvalidMaxOutboundConnectionConfiguration", "Ex4AAF8A", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				value1,
				value2
			});
		}

		public static LocalizedString EsnLangHebrew
		{
			get
			{
				return new LocalizedString("EsnLangHebrew", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionADVlvSizeLimitExceeded(string server, string message)
		{
			return new LocalizedString("ExceptionADVlvSizeLimitExceeded", "Ex66F6D8", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				server,
				message
			});
		}

		public static LocalizedString ExceptionWKGuidNeedsConfigSession(Guid wkguid)
		{
			return new LocalizedString("ExceptionWKGuidNeedsConfigSession", "Ex86A38D", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				wkguid
			});
		}

		public static LocalizedString WellKnownRecipientTypeAllRecipients
		{
			get
			{
				return new LocalizedString("WellKnownRecipientTypeAllRecipients", "Ex26FB9E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidCookieServiceInstanceIdException(string serviceInstanceId)
		{
			return new LocalizedString("InvalidCookieServiceInstanceIdException", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				serviceInstanceId
			});
		}

		public static LocalizedString InternalDsnLanguageNotSupported(string language)
		{
			return new LocalizedString("InternalDsnLanguageNotSupported", "Ex6520A9", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				language
			});
		}

		public static LocalizedString ExceptionCredentialsNotSupportedWithoutDC
		{
			get
			{
				return new LocalizedString("ExceptionCredentialsNotSupportedWithoutDC", "ExF75F28", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAdditionalInfo(string info)
		{
			return new LocalizedString("ErrorAdditionalInfo", "Ex86C126", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				info
			});
		}

		public static LocalizedString ExceptionOverBudget(string policyPart, string policyValue, BudgetType budgetType, int backoffTime)
		{
			return new LocalizedString("ExceptionOverBudget", "ExFF035A", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				policyPart,
				policyValue,
				budgetType,
				backoffTime
			});
		}

		public static LocalizedString ExceptionInvalidAccountName(string name)
		{
			return new LocalizedString("ExceptionInvalidAccountName", "ExA44AB9", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString InvalidConfigScope(object scope)
		{
			return new LocalizedString("InvalidConfigScope", "ExC47354", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				scope
			});
		}

		public static LocalizedString NoneMailboxRelationType
		{
			get
			{
				return new LocalizedString("NoneMailboxRelationType", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfigScopeNotLessThan(string leftScopeType, string rightScopeType)
		{
			return new LocalizedString("ConfigScopeNotLessThan", "ExF1D274", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				leftScopeType,
				rightScopeType
			});
		}

		public static LocalizedString MailboxUserRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("MailboxUserRecipientTypeDetails", "Ex24B5AF", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpamFilteringActionDelete
		{
			get
			{
				return new LocalizedString("SpamFilteringActionDelete", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionInvalidScopeOperation(ConfigScopes configScope)
		{
			return new LocalizedString("ExceptionInvalidScopeOperation", "Ex27ECA1", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				configScope
			});
		}

		public static LocalizedString FederatedOrganizationIdNotFound
		{
			get
			{
				return new LocalizedString("FederatedOrganizationIdNotFound", "Ex7DF8A1", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNonUniqueNetId(string netIdString)
		{
			return new LocalizedString("ErrorNonUniqueNetId", "Ex53C1DB", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				netIdString
			});
		}

		public static LocalizedString InvalidMailboxMoveFlags(object scope)
		{
			return new LocalizedString("InvalidMailboxMoveFlags", "ExFEA005", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				scope
			});
		}

		public static LocalizedString ExInvalidTypeArgumentException(string paramName, Type type, Type expectedType)
		{
			return new LocalizedString("ExInvalidTypeArgumentException", "ExDF0EAE", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				paramName,
				type,
				expectedType
			});
		}

		public static LocalizedString HostNameMatchesMultipleComputers(string hostName, string dn1, string dn2)
		{
			return new LocalizedString("HostNameMatchesMultipleComputers", "ExB03808", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				hostName,
				dn1,
				dn2
			});
		}

		public static LocalizedString SKUCapabilityBPOSSArchive
		{
			get
			{
				return new LocalizedString("SKUCapabilityBPOSSArchive", "Ex11BC4E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDefaultElcFolderTypeExists(string elcFolderName, string defaultFolderType)
		{
			return new LocalizedString("ErrorDefaultElcFolderTypeExists", "ExD6B3EB", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				elcFolderName,
				defaultFolderType
			});
		}

		public static LocalizedString ExceptionADTopologyDomainNameIsNotFqdn(string name, string fqdn)
		{
			return new LocalizedString("ExceptionADTopologyDomainNameIsNotFqdn", "ExC147A9", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				name,
				fqdn
			});
		}

		public static LocalizedString ReceiveAuthMechanismIntegrated
		{
			get
			{
				return new LocalizedString("ReceiveAuthMechanismIntegrated", "Ex010602", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionDefaultScopeMustContainDomainDN(string scope)
		{
			return new LocalizedString("ExceptionDefaultScopeMustContainDomainDN", "Ex8F7242", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				scope
			});
		}

		public static LocalizedString NameLookupEnabled
		{
			get
			{
				return new LocalizedString("NameLookupEnabled", "Ex447BD1", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidFolderLinksAddition(string elcFolderName, string error)
		{
			return new LocalizedString("ErrorInvalidFolderLinksAddition", "Ex75D6EE", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				elcFolderName,
				error
			});
		}

		public static LocalizedString ExceptionADTopologyHasNoServersInDomain(string domain)
		{
			return new LocalizedString("ExceptionADTopologyHasNoServersInDomain", "Ex4CEB8D", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString ForceFilter
		{
			get
			{
				return new LocalizedString("ForceFilter", "ExB23077", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionWriteOnceProperty(string propertyName)
		{
			return new LocalizedString("ExceptionWriteOnceProperty", "Ex3949D6", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				propertyName
			});
		}

		public static LocalizedString OrganizationCapabilityOfficeMessageEncryption
		{
			get
			{
				return new LocalizedString("OrganizationCapabilityOfficeMessageEncryption", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionOrgScopeNotInUserScope(string orgScope, string userScope)
		{
			return new LocalizedString("ExceptionOrgScopeNotInUserScope", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				orgScope,
				userScope
			});
		}

		public static LocalizedString PreferredInternetCodePageIso2022Jp
		{
			get
			{
				return new LocalizedString("PreferredInternetCodePageIso2022Jp", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlternateServiceAccountCredentialIsInvalid
		{
			get
			{
				return new LocalizedString("AlternateServiceAccountCredentialIsInvalid", "Ex6A6106", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmailAgeFilterTwoWeeks
		{
			get
			{
				return new LocalizedString("EmailAgeFilterTwoWeeks", "Ex306743", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceOS
		{
			get
			{
				return new LocalizedString("DeviceOS", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionInvalidVlvSeekReference(string seekReference)
		{
			return new LocalizedString("ExceptionInvalidVlvSeekReference", "Ex0E58AB", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				seekReference
			});
		}

		public static LocalizedString ExceptionADOperationFailedNotAMember(string server)
		{
			return new LocalizedString("ExceptionADOperationFailedNotAMember", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString ErrorTenantRelocationsAllowedOnlyForRootOrg
		{
			get
			{
				return new LocalizedString("ErrorTenantRelocationsAllowedOnlyForRootOrg", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OrganizationCapabilityTenantUpgrade
		{
			get
			{
				return new LocalizedString("OrganizationCapabilityTenantUpgrade", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorIsServerSuitableMissingOperatingSystemResponse(string dcName)
		{
			return new LocalizedString("ErrorIsServerSuitableMissingOperatingSystemResponse", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				dcName
			});
		}

		public static LocalizedString StarTlsDomainCapabilitiesNotAllowed
		{
			get
			{
				return new LocalizedString("StarTlsDomainCapabilitiesNotAllowed", "ExE9B082", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNamingPolicyExtensionCustomAttribute5
		{
			get
			{
				return new LocalizedString("GroupNamingPolicyExtensionCustomAttribute5", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTimeoutWritingSystemAddressListCache
		{
			get
			{
				return new LocalizedString("ErrorTimeoutWritingSystemAddressListCache", "ExC0C4B0", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPartnerApplicationNotFound(string applicationId)
		{
			return new LocalizedString("ErrorPartnerApplicationNotFound", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				applicationId
			});
		}

		public static LocalizedString ExceptionInvalidCredentials(string server, string credential)
		{
			return new LocalizedString("ExceptionInvalidCredentials", "ExE5DB78", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				server,
				credential
			});
		}

		public static LocalizedString MicrosoftExchangeRecipientDisplayNameError(string displayName)
		{
			return new LocalizedString("MicrosoftExchangeRecipientDisplayNameError", "ExA2C1DB", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				displayName
			});
		}

		public static LocalizedString CannotGetChild(string message)
		{
			return new LocalizedString("CannotGetChild", "Ex9A591E", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString CannotGetLocalSite
		{
			get
			{
				return new LocalizedString("CannotGetLocalSite", "Ex890F37", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseCopyAutoActivationPolicyUnrestricted
		{
			get
			{
				return new LocalizedString("DatabaseCopyAutoActivationPolicyUnrestricted", "Ex729064", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PrivateComputersOnly
		{
			get
			{
				return new LocalizedString("PrivateComputersOnly", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Always
		{
			get
			{
				return new LocalizedString("Always", "ExA361BF", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FacebookEnabled_Error(string objectName, string cmdLet, string parameters, string capabilities)
		{
			return new LocalizedString("FacebookEnabled_Error", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				objectName,
				cmdLet,
				parameters,
				capabilities
			});
		}

		public static LocalizedString WellKnownRecipientTypeMailUsers
		{
			get
			{
				return new LocalizedString("WellKnownRecipientTypeMailUsers", "ExA8268E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionADTopologyErrorWhenLookingForForest(int error, string fqdn, string message)
		{
			return new LocalizedString("ExceptionADTopologyErrorWhenLookingForForest", "ExA62B6D", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				error,
				fqdn,
				message
			});
		}

		public static LocalizedString CannotSetZeroAsEapPriority
		{
			get
			{
				return new LocalizedString("CannotSetZeroAsEapPriority", "Ex6985D2", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMultiValuedPropertyTooLarge(string property, int count, int max)
		{
			return new LocalizedString("ErrorMultiValuedPropertyTooLarge", "ExC89A6F", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				property,
				count,
				max
			});
		}

		public static LocalizedString ErrorLinkedADObjectNotInFirstOrganization(string propertyName, string propertyValue, string objectId, string firstOrgId)
		{
			return new LocalizedString("ErrorLinkedADObjectNotInFirstOrganization", "Ex3F9F60", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				propertyName,
				propertyValue,
				objectId,
				firstOrgId
			});
		}

		public static LocalizedString RootZone
		{
			get
			{
				return new LocalizedString("RootZone", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RenameNotAllowed
		{
			get
			{
				return new LocalizedString("RenameNotAllowed", "Ex742D23", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Unknown
		{
			get
			{
				return new LocalizedString("Unknown", "Ex5D54E9", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangItalian
		{
			get
			{
				return new LocalizedString("EsnLangItalian", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDisplayNameInvalid
		{
			get
			{
				return new LocalizedString("ErrorDisplayNameInvalid", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationNotValidLegacyDN
		{
			get
			{
				return new LocalizedString("ConstraintViolationNotValidLegacyDN", "ExBE78E8", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReceiveExtendedProtectionPolicyRequire
		{
			get
			{
				return new LocalizedString("ReceiveExtendedProtectionPolicyRequire", "Ex4D9510", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorExceededHosterResourceCountQuota(string policyId, string poType, int countQuota)
		{
			return new LocalizedString("ErrorExceededHosterResourceCountQuota", "Ex77A0D5", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				policyId,
				poType,
				countQuota
			});
		}

		public static LocalizedString SpamFilteringOptionOff
		{
			get
			{
				return new LocalizedString("SpamFilteringOptionOff", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationValueNotSupportedLCID(string propertyName, int LCID)
		{
			return new LocalizedString("ConstraintViolationValueNotSupportedLCID", "Ex1F6DE9", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				propertyName,
				LCID
			});
		}

		public static LocalizedString RusUnableToPerformValidation(string s)
		{
			return new LocalizedString("RusUnableToPerformValidation", "Ex030A7B", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ExternallyManaged
		{
			get
			{
				return new LocalizedString("ExternallyManaged", "ExF9D9E6", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequireTLSWithoutTLS
		{
			get
			{
				return new LocalizedString("RequireTLSWithoutTLS", "Ex22F161", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSecondaryAccountPartitionCantBeUsedForProvisioning(string id)
		{
			return new LocalizedString("ErrorSecondaryAccountPartitionCantBeUsedForProvisioning", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ErrorCannotParseAuthMetadata
		{
			get
			{
				return new LocalizedString("ErrorCannotParseAuthMetadata", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidActivationPreference
		{
			get
			{
				return new LocalizedString("ErrorInvalidActivationPreference", "Ex7CD088", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CapabilityFederatedUser
		{
			get
			{
				return new LocalizedString("CapabilityFederatedUser", "Ex7DF90D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidLegacyCommonName(string cn)
		{
			return new LocalizedString("ErrorInvalidLegacyCommonName", "Ex33411D", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				cn
			});
		}

		public static LocalizedString EsnLangFilipino
		{
			get
			{
				return new LocalizedString("EsnLangFilipino", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionExtendedRightNotFound(string displayName)
		{
			return new LocalizedString("ExceptionExtendedRightNotFound", "Ex176FFE", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				displayName
			});
		}

		public static LocalizedString FailedToWriteAlternateServiceAccountConfigToRegistry(string keyPath)
		{
			return new LocalizedString("FailedToWriteAlternateServiceAccountConfigToRegistry", "Ex7C1F04", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				keyPath
			});
		}

		public static LocalizedString ExceptionUnsupportedMatchOptionsForProperty(string propertyName, string optionsType)
		{
			return new LocalizedString("ExceptionUnsupportedMatchOptionsForProperty", "Ex14A0D0", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				propertyName,
				optionsType
			});
		}

		public static LocalizedString ExceptionUnknownDirectoryAttribute(string srcObj, string dn)
		{
			return new LocalizedString("ExceptionUnknownDirectoryAttribute", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				srcObj,
				dn
			});
		}

		public static LocalizedString OutboundConnectorUseMXRecordShouldBeFalseIfSmartHostsIsPresent
		{
			get
			{
				return new LocalizedString("OutboundConnectorUseMXRecordShouldBeFalseIfSmartHostsIsPresent", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotAcquireAuthMetadata(string url, string error)
		{
			return new LocalizedString("ErrorCannotAcquireAuthMetadata", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				url,
				error
			});
		}

		public static LocalizedString LdapFilterErrorBracketMismatch
		{
			get
			{
				return new LocalizedString("LdapFilterErrorBracketMismatch", "Ex886AD3", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExtensionNotUnique(string s, string dialPlan)
		{
			return new LocalizedString("ExtensionNotUnique", "ExC9C216", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				s,
				dialPlan
			});
		}

		public static LocalizedString SipResourceIdentifierRequiredNotAllowed
		{
			get
			{
				return new LocalizedString("SipResourceIdentifierRequiredNotAllowed", "ExC29F3D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString XMSWLHeader
		{
			get
			{
				return new LocalizedString("XMSWLHeader", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionADTopologyNoSuchSite(string siteName, string errorMessage)
		{
			return new LocalizedString("ExceptionADTopologyNoSuchSite", "Ex863AD3", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				siteName,
				errorMessage
			});
		}

		public static LocalizedString ErrorRecipientTypeIsNotValidForDeliveryRestrictionOrModeration(string id, string type)
		{
			return new LocalizedString("ErrorRecipientTypeIsNotValidForDeliveryRestrictionOrModeration", "Ex159685", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				id,
				type
			});
		}

		public static LocalizedString ServerRoleCafe
		{
			get
			{
				return new LocalizedString("ServerRoleCafe", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeleteAndRejectThreshold
		{
			get
			{
				return new LocalizedString("DeleteAndRejectThreshold", "ExF3F819", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTwoOrMoreUniqueRecipientTypes(string value)
		{
			return new LocalizedString("ErrorTwoOrMoreUniqueRecipientTypes", "Ex13C1DF", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString Policy
		{
			get
			{
				return new LocalizedString("Policy", "Ex37A429", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CanRunRestoreState_NotLocal
		{
			get
			{
				return new LocalizedString("CanRunRestoreState_NotLocal", "Ex85C03B", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ElcAuditLogPathMissing
		{
			get
			{
				return new LocalizedString("ElcAuditLogPathMissing", "Ex9C7E49", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCertAuthIgnore
		{
			get
			{
				return new LocalizedString("ClientCertAuthIgnore", "ExB83ACA", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Reserved2
		{
			get
			{
				return new LocalizedString("Reserved2", "Ex6752DF", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionADTopologyErrorWhenLookingForLocalDomainTrustRelationships(int error, string message)
		{
			return new LocalizedString("ExceptionADTopologyErrorWhenLookingForLocalDomainTrustRelationships", "Ex788FD9", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				error,
				message
			});
		}

		public static LocalizedString ExternalEmailAddressInvalid(string message)
		{
			return new LocalizedString("ExternalEmailAddressInvalid", "Ex4300A1", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString ConfigWriteScopes
		{
			get
			{
				return new LocalizedString("ConfigWriteScopes", "Ex64C531", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DetailsTemplateCorrupted
		{
			get
			{
				return new LocalizedString("DetailsTemplateCorrupted", "ExEB07EF", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCertAuthAccepted
		{
			get
			{
				return new LocalizedString("ClientCertAuthAccepted", "ExC62663", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorGroupMemberDepartRestrictionApprovalRequired(string id)
		{
			return new LocalizedString("ErrorGroupMemberDepartRestrictionApprovalRequired", "Ex0C7650", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ExceptionAdminLimitExceeded
		{
			get
			{
				return new LocalizedString("ExceptionAdminLimitExceeded", "ExA990B2", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DataMoveReplicationConstraintSecondCopy
		{
			get
			{
				return new LocalizedString("DataMoveReplicationConstraintSecondCopy", "Ex3A5152", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TransientGlsError(string message)
		{
			return new LocalizedString("TransientGlsError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString ReceiveAuthMechanismTls
		{
			get
			{
				return new LocalizedString("ReceiveAuthMechanismTls", "Ex70CA91", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotFindTemplateTenant
		{
			get
			{
				return new LocalizedString("CannotFindTemplateTenant", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToReadStoreUserInformation
		{
			get
			{
				return new LocalizedString("FailedToReadStoreUserInformation", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FilterCannotBeEmpty(ScopeRestrictionType scopeType)
		{
			return new LocalizedString("FilterCannotBeEmpty", "ExC4D6A3", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				scopeType
			});
		}

		public static LocalizedString ExceptionADTopologyServiceCallError(string server, string error)
		{
			return new LocalizedString("ExceptionADTopologyServiceCallError", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				server,
				error
			});
		}

		public static LocalizedString MicrosoftExchangeRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("MicrosoftExchangeRecipientTypeDetails", "ExFC8F34", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DataMoveReplicationConstraintCINoReplication
		{
			get
			{
				return new LocalizedString("DataMoveReplicationConstraintCINoReplication", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDistributionGroupNamingPolicyFormat(string value, string placeHolders)
		{
			return new LocalizedString("InvalidDistributionGroupNamingPolicyFormat", "Ex6CBA35", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				value,
				placeHolders
			});
		}

		public static LocalizedString ErrorTransitionCounterHasZeroCount
		{
			get
			{
				return new LocalizedString("ErrorTransitionCounterHasZeroCount", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeleteAndQuarantineThreshold
		{
			get
			{
				return new LocalizedString("DeleteAndQuarantineThreshold", "Ex9B4DE4", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfigurationSettingsNotFoundForGroup(string groupName, string key)
		{
			return new LocalizedString("ConfigurationSettingsNotFoundForGroup", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				groupName,
				key
			});
		}

		public static LocalizedString IndustryAgriculture
		{
			get
			{
				return new LocalizedString("IndustryAgriculture", "Ex592140", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotGetDomainInfo(string error)
		{
			return new LocalizedString("CannotGetDomainInfo", "ExCAD228", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString ExceptionGuidSearchRootWithScope(string guid)
		{
			return new LocalizedString("ExceptionGuidSearchRootWithScope", "ExA302C0", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				guid
			});
		}

		public static LocalizedString ClientCertAuthRequired
		{
			get
			{
				return new LocalizedString("ClientCertAuthRequired", "ExAFB27E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerRoleExtendedRole7
		{
			get
			{
				return new LocalizedString("ServerRoleExtendedRole7", "ExD7581D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubmissionOverrideListOnWrongServer
		{
			get
			{
				return new LocalizedString("SubmissionOverrideListOnWrongServer", "Ex19E9E0", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangBasque
		{
			get
			{
				return new LocalizedString("EsnLangBasque", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserRecipientType
		{
			get
			{
				return new LocalizedString("UserRecipientType", "Ex8D44B2", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailEnabledUserRecipientType
		{
			get
			{
				return new LocalizedString("MailEnabledUserRecipientType", "ExAA595C", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupTypeFlagsGlobal
		{
			get
			{
				return new LocalizedString("GroupTypeFlagsGlobal", "Ex470809", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DataMoveReplicationConstraintCISecondDatacenter
		{
			get
			{
				return new LocalizedString("DataMoveReplicationConstraintCISecondDatacenter", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LoadBalanceCannotUseBothInclusionLists
		{
			get
			{
				return new LocalizedString("LoadBalanceCannotUseBothInclusionLists", "ExEEC7D2", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeMissedcallMC
		{
			get
			{
				return new LocalizedString("ExchangeMissedcallMC", "Ex7DE132", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotFindTenant(string tenant, string partition)
		{
			return new LocalizedString("ErrorCannotFindTenant", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				tenant,
				partition
			});
		}

		public static LocalizedString RequesterNameInvalid
		{
			get
			{
				return new LocalizedString("RequesterNameInvalid", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoSuitableDC(string server, string forest)
		{
			return new LocalizedString("ErrorNoSuitableDC", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				server,
				forest
			});
		}

		public static LocalizedString SharingPolicyDuplicateDomain(string domains)
		{
			return new LocalizedString("SharingPolicyDuplicateDomain", "Ex63CC41", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				domains
			});
		}

		public static LocalizedString ErrorNonUniqueCertificate(string certificate)
		{
			return new LocalizedString("ErrorNonUniqueCertificate", "ExC29AE0", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				certificate
			});
		}

		public static LocalizedString ByteEncoderTypeUseBase64Html7BitTextPlain
		{
			get
			{
				return new LocalizedString("ByteEncoderTypeUseBase64Html7BitTextPlain", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SecurityPrincipalTypeComputer
		{
			get
			{
				return new LocalizedString("SecurityPrincipalTypeComputer", "Ex62AE96", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangAmharic
		{
			get
			{
				return new LocalizedString("EsnLangAmharic", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OrgWideDelegatingConfigScopeMustBeTheSameAsRoleImplicitWriteScope(ConfigWriteScopeType scopeType)
		{
			return new LocalizedString("OrgWideDelegatingConfigScopeMustBeTheSameAsRoleImplicitWriteScope", "ExCB74DA", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				scopeType
			});
		}

		public static LocalizedString LimitedMoveOnlyAllowed
		{
			get
			{
				return new LocalizedString("LimitedMoveOnlyAllowed", "ExDE80AA", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ASInvalidAuthenticationOptionsForAccessMethod
		{
			get
			{
				return new LocalizedString("ASInvalidAuthenticationOptionsForAccessMethod", "Ex12F6AD", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NullPasswordEncryptionKey
		{
			get
			{
				return new LocalizedString("NullPasswordEncryptionKey", "Ex18E022", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedUserTypeDetails
		{
			get
			{
				return new LocalizedString("LinkedUserTypeDetails", "ExB27AF6", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionNativeErrorWhenLookingForServersInDomain(int error, string domain, string message)
		{
			return new LocalizedString("ExceptionNativeErrorWhenLookingForServersInDomain", "Ex9AD963", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				error,
				domain,
				message
			});
		}

		public static LocalizedString ErrorMoreThanOneSKUCapability(string values)
		{
			return new LocalizedString("ErrorMoreThanOneSKUCapability", "Ex71B824", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				values
			});
		}

		public static LocalizedString AutoDatabaseMountDialLossless
		{
			get
			{
				return new LocalizedString("AutoDatabaseMountDialLossless", "Ex4C0DF7", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReceiveAuthMechanismExternalAuthoritative
		{
			get
			{
				return new LocalizedString("ReceiveAuthMechanismExternalAuthoritative", "Ex402E55", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTruncationLagTime
		{
			get
			{
				return new LocalizedString("ErrorTruncationLagTime", "Ex3A1B51", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionIdImmutable
		{
			get
			{
				return new LocalizedString("ExceptionIdImmutable", "ExB9F6F0", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlternateServiceAccountConfigurationDisplayFormat(string latestCredential, string previousCredential, string elipsis)
		{
			return new LocalizedString("AlternateServiceAccountConfigurationDisplayFormat", "ExC646BD", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				latestCredential,
				previousCredential,
				elipsis
			});
		}

		public static LocalizedString ExceptionDefaultScopeAndSearchRoot
		{
			get
			{
				return new LocalizedString("ExceptionDefaultScopeAndSearchRoot", "Ex843EB0", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorOfferProgramIdMandatoryOnSharedConfig
		{
			get
			{
				return new LocalizedString("ErrorOfferProgramIdMandatoryOnSharedConfig", "Ex3C3F13", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DefaultDatabaseContainerNotFoundException(string agName)
		{
			return new LocalizedString("DefaultDatabaseContainerNotFoundException", "ExEBBDF2", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				agName
			});
		}

		public static LocalizedString ServerRoleExtendedRole4
		{
			get
			{
				return new LocalizedString("ServerRoleExtendedRole4", "Ex125DC2", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionAdamGetServerFromDomainDN(string DN)
		{
			return new LocalizedString("ExceptionAdamGetServerFromDomainDN", "Ex502EE4", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				DN
			});
		}

		public static LocalizedString ErrorComment
		{
			get
			{
				return new LocalizedString("ErrorComment", "Ex9EF6E6", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReplayLagTime
		{
			get
			{
				return new LocalizedString("ErrorReplayLagTime", "Ex0077A5", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorEdbFilePathInRoot(string edbFileFullPath)
		{
			return new LocalizedString("ErrorEdbFilePathInRoot", "Ex709FA0", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				edbFileFullPath
			});
		}

		public static LocalizedString ExLengthOfVersionByteArrayError
		{
			get
			{
				return new LocalizedString("ExLengthOfVersionByteArrayError", "Ex4E3FF4", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfigurationSettingsGroupSummary(string name, int priority, bool enabled, int count)
		{
			return new LocalizedString("ConfigurationSettingsGroupSummary", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				name,
				priority,
				enabled,
				count
			});
		}

		public static LocalizedString LdapAdd
		{
			get
			{
				return new LocalizedString("LdapAdd", "Ex33E59E", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DomainStatePendingActivation
		{
			get
			{
				return new LocalizedString("DomainStatePendingActivation", "ExFE17FF", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSmartHost(string smartHost)
		{
			return new LocalizedString("InvalidSmartHost", "Ex969628", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				smartHost
			});
		}

		public static LocalizedString ProviderFileNotFoundLoadException(string providerName, string assemblyPath)
		{
			return new LocalizedString("ProviderFileNotFoundLoadException", "Ex2C855E", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				providerName,
				assemblyPath
			});
		}

		public static LocalizedString Uninterruptible
		{
			get
			{
				return new LocalizedString("Uninterruptible", "Ex9AF75D", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMustBeADRawEntry
		{
			get
			{
				return new LocalizedString("ErrorMustBeADRawEntry", "ExD92094", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString None
		{
			get
			{
				return new LocalizedString("None", "Ex93DDB2", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRecipientAsBothAcceptedAndRejected(string recipient)
		{
			return new LocalizedString("ErrorRecipientAsBothAcceptedAndRejected", "ExF446CA", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				recipient
			});
		}

		public static LocalizedString ErrorBadLocalizedComment
		{
			get
			{
				return new LocalizedString("ErrorBadLocalizedComment", "Ex68C60A", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EsnLangSlovak
		{
			get
			{
				return new LocalizedString("EsnLangSlovak", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LdapFilterErrorInvalidBooleanValue
		{
			get
			{
				return new LocalizedString("LdapFilterErrorInvalidBooleanValue", "Ex9F209A", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OabVersionsNullException
		{
			get
			{
				return new LocalizedString("OabVersionsNullException", "Ex0E1513", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DuplicateTlsDomainCapabilitiesNotAllowed(SmtpDomainWithSubdomains domain)
		{
			return new LocalizedString("DuplicateTlsDomainCapabilitiesNotAllowed", "Ex999C8F", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString CannotConstructCrossTenantObjectId(string property)
		{
			return new LocalizedString("CannotConstructCrossTenantObjectId", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString CrossRecordMismatch(MservRecord record1, MservRecord record2)
		{
			return new LocalizedString("CrossRecordMismatch", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				record1,
				record2
			});
		}

		public static LocalizedString Inbox
		{
			get
			{
				return new LocalizedString("Inbox", "ExF47939", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorStartDateAfterEndDate(string start, string end)
		{
			return new LocalizedString("ErrorStartDateAfterEndDate", "Ex33BBE5", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				start,
				end
			});
		}

		public static LocalizedString ErrorNotInReadScope(string identity)
		{
			return new LocalizedString("ErrorNotInReadScope", "ExD700E7", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ErrorSubnetMaskLessThanMinRange(int maskBits, int minRange)
		{
			return new LocalizedString("ErrorSubnetMaskLessThanMinRange", "ExA97C7C", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				maskBits,
				minRange
			});
		}

		public static LocalizedString ContactRecipientTypeDetails
		{
			get
			{
				return new LocalizedString("ContactRecipientTypeDetails", "Ex6F6521", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidTenantRecordInGls(Guid tenantGuid, string resourceForestFqdn, string accountForestFqdn)
		{
			return new LocalizedString("InvalidTenantRecordInGls", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				tenantGuid,
				resourceForestFqdn,
				accountForestFqdn
			});
		}

		public static LocalizedString EsnLangKazakh
		{
			get
			{
				return new LocalizedString("EsnLangKazakh", "", false, false, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNonUniqueLiveIdMemberName(string liveIdMemberName)
		{
			return new LocalizedString("ErrorNonUniqueLiveIdMemberName", "", false, false, DirectoryStrings.ResourceManager, new object[]
			{
				liveIdMemberName
			});
		}

		public static LocalizedString DisableFilter
		{
			get
			{
				return new LocalizedString("DisableFilter", "Ex758142", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BluetoothHandsfreeOnly
		{
			get
			{
				return new LocalizedString("BluetoothHandsfreeOnly", "Ex5497C8", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GatewayGuid
		{
			get
			{
				return new LocalizedString("GatewayGuid", "Ex6F7DD5", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarSharingFreeBusyNone
		{
			get
			{
				return new LocalizedString("CalendarSharingFreeBusyNone", "Ex584674", false, true, DirectoryStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionAccessDeniedFromRUS(string server)
		{
			return new LocalizedString("ExceptionAccessDeniedFromRUS", "Ex08CB7F", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString CannotCompareScopeObjectWithOU(string leftId, string leftType, string ou)
		{
			return new LocalizedString("CannotCompareScopeObjectWithOU", "Ex81EF20", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				leftId,
				leftType,
				ou
			});
		}

		public static LocalizedString ErrorInvalidLegacyDN(string legacyDN)
		{
			return new LocalizedString("ErrorInvalidLegacyDN", "ExA5C9D3", false, true, DirectoryStrings.ResourceManager, new object[]
			{
				legacyDN
			});
		}

		public static LocalizedString GetLocalizedString(DirectoryStrings.IDs key)
		{
			return new LocalizedString(DirectoryStrings.stringIDs[(uint)key], DirectoryStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(976);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Data.Directory.Strings", typeof(DirectoryStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			GroupMailboxRecipientTypeDetails = 2223810040U,
			InvalidTransportSyncHealthLogSizeConfiguration = 3059540560U,
			ReceiveExtendedProtectionPolicyNone = 3553131525U,
			OrganizationCapabilityManagement = 1352261648U,
			EsnLangTamil = 3384994469U,
			LdapFilterErrorInvalidWildCard = 1623026330U,
			Individual = 3266435989U,
			ExternalRelay = 2171581398U,
			InvalidTransportSyncDownloadSizeConfiguration = 1719230762U,
			MessageRateSourceFlagsAll = 2928684304U,
			SKUCapabilityBPOSSBasic = 4209173728U,
			IndustryMediaMarketingAdvertising = 3376948578U,
			SKUCapabilityUnmanaged = 2570737323U,
			BackSyncDataSourceTransientErrorMessage = 4073959654U,
			MailEnabledNonUniversalGroupRecipientTypeDetails = 3376217818U,
			ADDriverStoreAccessPermanentError = 3806493464U,
			DeviceType = 3323369056U,
			EsnLangFarsi = 3229570631U,
			InvalidTempErrorSetting = 3503019411U,
			ReplicationTypeNone = 3874381006U,
			IndustryBusinessServicesConsulting = 1593550494U,
			ErrorAdfsConfigFormat = 210447381U,
			Quarantined = 996355914U,
			OutboundConnectorSmartHostShouldBePresentIfUseMXRecordFalse = 3114754472U,
			LongRunningCostHandle = 364260824U,
			EsnLangChineseTraditional = 3479185892U,
			IndustryTransportation = 1803878278U,
			Silent = 815885189U,
			AlternateServiceAccountCredentialQualifiedUserNameWrongFormat = 3145530267U,
			InvalidBannerSetting = 1840954879U,
			GroupNamingPolicyCustomAttribute4 = 1924734196U,
			InboundConnectorIncorrectCloudServicesMailEnabled = 230574830U,
			LdapFilterErrorAnrIsNotSupported = 3936814429U,
			E164 = 438888054U,
			ErrorAuthMetaDataContentEmpty = 1874297349U,
			MailEnabledContactRecipientType = 2285243143U,
			MailEnabledUniversalDistributionGroupRecipientTypeDetails = 562488721U,
			SendAuthMechanismExternalAuthoritative = 360676809U,
			InboundConnectorRequiredTlsSettingsInvalid = 528770240U,
			GroupNamingPolicyCustomAttribute1 = 1924734191U,
			Dual = 1743625100U,
			DatabaseCopyAutoActivationPolicyIntrasiteOnly = 1822619008U,
			Never = 1594930120U,
			ByteEncoderTypeUndefined = 1644017996U,
			InvalidRcvProtocolLogSizeConfiguration = 3817431401U,
			GetRootDseRequiresDomainController = 2785880074U,
			InheritFromDialPlan = 637440764U,
			OrganizationCapabilityMessageTracking = 737697267U,
			InboundConnectorInvalidTlsSenderCertificateName = 1573064009U,
			SoftDelete = 3133553171U,
			OrganizationCapabilityUMGrammar = 2570855206U,
			Allow = 1776488541U,
			DomainNameIsNull = 1261795780U,
			PromptForAlias = 2303788021U,
			ErrorSystemAddressListInWrongContainer = 3614458512U,
			ExceptionUnableToDisableAdminTopologyMode = 651742924U,
			Secured = 1241597555U,
			ExternalAndAuthSet = 2462615416U,
			EsnLangJapanese = 597372135U,
			EsnLangPortuguesePortugal = 1391104995U,
			EsnLangFinnish = 2487111597U,
			ExceptionOwaCannotSetPropertyOnVirtualDirectoryOtherThanExchweb = 2811972280U,
			WhenDelivered = 2180736490U,
			DomainStatePendingRelease = 2839188613U,
			GroupNamingPolicyExtensionCustomAttribute2 = 1631812413U,
			AutoGroup = 2127564032U,
			ErrorStartDateExpiration = 3880444299U,
			MailboxMoveStatusQueued = 285960632U,
			Minute = 2220842206U,
			SentItems = 590977256U,
			ExchangeVoicemailMC = 4029642168U,
			AppliedInFull = 3010978409U,
			NoAddressSpaces = 2215231792U,
			SKUCapabilityEOPStandardAddOn = 1387109368U,
			IndustryNonProfit = 2260925979U,
			EsnLangDefault = 3413117907U,
			SpecifyCustomGreetingFileName = 707064308U,
			EsnLangSlovenian = 671952847U,
			TelExtn = 1059422816U,
			LdapFilterErrorInvalidGtLtOperand = 4045631128U,
			SystemMailboxRecipientType = 434185288U,
			ReplicationTypeRemote = 2824730050U,
			Enterprise = 1414596097U,
			Gsm = 3115737533U,
			Journal = 4137480277U,
			SpamFilteringTestActionNone = 924597469U,
			CustomRoleDescription_MyPersonalInformation = 1877544294U,
			MailboxMoveStatusAutoSuspended = 1608856269U,
			Any = 3068683316U,
			Location = 2325276717U,
			ExternalTrust = 3031587385U,
			IndustryPrintingPublishing = 2830455760U,
			AllComputers = 3577501733U,
			ExceptionRusNotFound = 3208958876U,
			GroupNamingPolicyCity = 2703120928U,
			NoPagesSpecified = 1213668011U,
			PublicDatabaseRecipientType = 2286842903U,
			CanEnableLocalCopyState_CanBeEnabled = 4050233751U,
			RedirectToRecipientsNotSet = 1069069500U,
			InfoAnnouncementEnabled = 2472102570U,
			ConfigurationSettingsADConfigDriverError = 3867415726U,
			LdapFilterErrorEscCharWithoutEscapable = 1919923094U,
			IndustryGovernment = 2829212743U,
			CustomRoleDescription_MyAddressInformation = 1921301802U,
			EsnLangNorwegianNynorsk = 861191034U,
			IndustryEngineeringArchitecture = 1996416364U,
			SendAuthMechanismBasicAuth = 2551449657U,
			SKUCapabilityEOPPremiumAddOn = 4060941198U,
			ErrorResourceTypeInvalid = 1820951283U,
			OrgContainerNotFoundException = 2134869325U,
			SKUCapabilityBPOSSStandardArchive = 4076766949U,
			InternalSenderAdminAddressRequired = 1359848478U,
			CannotGetUsefulDomainInfo = 237887777U,
			ErrorElcSuspensionNotEnabled = 332960507U,
			DatabaseMasterTypeServer = 2173147846U,
			ConnectionTimeoutLessThanInactivityTimeout = 3421832948U,
			HygieneSuitePremium = 444871648U,
			Exadmin = 2986397362U,
			ExceptionADTopologyCannotFindWellKnownExchangeGroup = 2704331370U,
			CommandFrequency = 2979126483U,
			IndustryConstruction = 1861502555U,
			SharedMailboxRecipientTypeDetails = 4263249978U,
			AccessDeniedToEventLog = 2830208040U,
			EsnLangSerbian = 1445823720U,
			ReplicationTypeUnknown = 2242173198U,
			ErrorDuplicateMapiIdsInConfiguredAttributes = 2058453416U,
			DirectoryBasedEdgeBlockModeOn = 56170716U,
			LiveCredentialWithoutBasic = 2380978387U,
			ExclusiveConfigScopes = 1607133185U,
			IndustryRealEstate = 1118921376U,
			EsnLangNorwegian = 2956380840U,
			ServerRoleMonitoring = 1024471425U,
			ASInvalidAccessMethod = 1106844962U,
			NotApplied = 403740404U,
			ConfigurationSettingsADNotificationError = 2619186021U,
			MonitoringMailboxRecipientTypeDetails = 729925097U,
			EsnLangCroatian = 56435549U,
			TlsAuthLevelWithDomainSecureEnabled = 2386455749U,
			EsnLangGerman = 4104902926U,
			RoleAssignmentPolicyDescription_Default = 3816616305U,
			GroupTypeFlagsNone = 252422050U,
			WellKnownRecipientTypeMailboxUsers = 2167560764U,
			LdapFilterErrorInvalidWildCardGtLt = 1324265457U,
			SmartHostNotSet = 3536945350U,
			DeviceRule = 3531789014U,
			NotTrust = 1299460569U,
			EmailAgeFilterAll = 2824779686U,
			LanguageBlockListNotSet = 1524653102U,
			EsnLangSerbianCyrillic = 2547761307U,
			CalendarAgeFilterSixMonths = 1551636376U,
			ErrorMetadataNotSearchProperty = 1442063141U,
			InvalidDefaultMailbox = 52431374U,
			Drafts = 115734878U,
			RemoteGroupMailboxRecipientTypeDetails = 2819908830U,
			EsnLangSwahili = 289102393U,
			ExceptionPagedReaderReadAllAfterEnumerating = 794922827U,
			DsnDefaultLanguageMustBeSpecificCulture = 835151952U,
			BestBodyFormat = 719400811U,
			CanEnableLocalCopyState_AlreadyEnabled = 3064393132U,
			DeviceDiscovery = 1010456570U,
			AccessDenied = 3963885811U,
			InvalidActiveUserStatisticsLogSizeConfiguration = 29593584U,
			ErrorActionOnExpirationSpecified = 1014182364U,
			TlsAuthLevelWithNoDomainOnSmartHost = 3776155750U,
			DeferredFailoverEntryString = 1587883572U,
			TaskItemsMC = 4110637509U,
			GroupNamingPolicyCustomAttribute7 = 1924734197U,
			UnknownAttribute = 2647513696U,
			MountDialOverrideBestAvailability = 703634004U,
			ErrorArbitrationMailboxPropertyEmailAddressesEmpty = 3562314173U,
			AlternateServiceAccountCredentialNotSet = 608628016U,
			DataMoveReplicationConstraintAllCopies = 2092029626U,
			GlobalThrottlingPolicyAmbiguousException = 3902771789U,
			InvalidServerStatisticsLogSizeConfiguration = 2998146498U,
			SipResourceIdRequired = 3361458474U,
			EsnLangPortuguese = 1616139245U,
			AutoDetect = 4241336410U,
			SpamFilteringActionRedirect = 4255105347U,
			CanRunRestoreState_Invalid = 3532819894U,
			OutboundConnectorIncorrectCloudServicesMailEnabled = 2530566197U,
			DatabaseCopyAutoActivationPolicyBlocked = 95325995U,
			CustomRoleDescription_MyName = 3838277697U,
			EsnLangOriya = 3720125794U,
			UserAgent = 702028774U,
			DomainStateActive = 4211599483U,
			PartnersCannotHaveWildcards = 1640391059U,
			IPv4Only = 352018919U,
			InboundConnectorInvalidIPCertificateCombinations = 4150730333U,
			Exchange2003or2000 = 2814018773U,
			ErrorOneProcessInitializedAsBothSingleAndMultiple = 2730108605U,
			RoomListGroupTypeDetails = 1391517930U,
			MailEnabledForestContactRecipientTypeDetails = 3586618070U,
			ErrorAuthMetadataNoIssuingEndpoint = 2286188335U,
			NonUniversalGroupRecipientTypeDetails = 2227190334U,
			ErrorMustBeSysConfigObject = 2587673404U,
			OutboundConnectorTlsSettingsInvalidTlsDomainWithoutDomainValidation = 2742534530U,
			LdapFilterErrorInvalidBitwiseOperand = 135248896U,
			ExceptionSetPreferredDCsOnlyForManagement = 2106980546U,
			LegacyArchiveJournals = 3635271833U,
			CustomInternalSubjectRequired = 2980205681U,
			ErrorCannotAddArchiveMailbox = 238304980U,
			NoNewCalls = 1479682494U,
			ErrorMessageClassEmpty = 1615193486U,
			GloballyDistributedOABCacheReadTimeoutError = 189075208U,
			Manual = 523533880U,
			ErrorAcceptedDomainCannotContainWildcardAndNegoConfig = 3179357576U,
			UniversalSecurityGroupRecipientTypeDetails = 968858937U,
			ArbitrationMailboxTypeDetails = 3647297993U,
			CalendarAgeFilterAll = 575705180U,
			GroupNamingPolicyCompany = 862838650U,
			IndustryMining = 700328008U,
			ServerRoleOSP = 2775202161U,
			InvalidDirectoryConfiguration = 1190445522U,
			ErrorDDLReferral = 3381355689U,
			LdapFilterErrorNoAttributeValue = 3541370315U,
			ExternalEnrollment = 606960029U,
			ErrorTimeoutReadingSystemAddressListCache = 161599950U,
			CanRunDefaultUpdateState_NotSuspended = 2375038189U,
			PreferredInternetCodePageSio2022Jp = 1615928121U,
			HtmlAndTextAlternative = 1762945050U,
			GlobalAddressList = 1164140307U,
			MailTipsAccessLevelNone = 1912797067U,
			EsnLangGalician = 3599499864U,
			ServerRoleFrontendTransport = 3786203794U,
			Exchange2009 = 4087400250U,
			TransientMservErrorDescription = 1259815309U,
			ReceiveAuthMechanismExchangeServer = 48855524U,
			Watsons = 3380639415U,
			OrganizationCapabilityPstProvider = 2074397341U,
			ErrorCapabilityNone = 2277391560U,
			ExceptionAllDomainControllersUnavailable = 224904099U,
			ServersContainerNotFoundException = 3495902273U,
			MailboxMoveStatusCompletionInProgress = 296014363U,
			ServerRoleMailbox = 2125756541U,
			ErrorResourceTypeMissing = 3930280380U,
			Contacts = 1716044995U,
			SendAuthMechanismTls = 3268644368U,
			AggregatedSessionCannotMakeMbxChanges = 3574113162U,
			PAAEnabled = 1802707653U,
			NonPartner = 2191519417U,
			BasicAfterTLSWithoutBasic = 4008691139U,
			ErrorSharedConfigurationBothRoles = 2672001483U,
			EsnLangDutch = 4077321274U,
			DsnLanguageNotSupportedForCustomization = 1406386932U,
			IndustryNotSpecified = 2440749065U,
			ErrorDDLFilterError = 1045941944U,
			AddressList = 3599602982U,
			MustDisplayComment = 795884132U,
			ServerRoleFfoWebServices = 3464146580U,
			ServerRoleClientAccess = 1052758952U,
			SKUCapabilityBPOSSEnterprise = 3595585153U,
			InvalidReceiveAuthModeExternalOnly = 3839109662U,
			ErrorSettingOverrideNull = 2653001089U,
			LdapFilterErrorQueryTooLong = 4230107429U,
			ErrorMoveToDestinationFolderNotDefined = 1655452658U,
			MailboxMoveStatusInProgress = 4190154187U,
			SecurityPrincipalTypeGroup = 370461711U,
			X400Authoritative = 1470218539U,
			MailFlowPartnerInternalMailContentTypeMimeHtml = 3038327607U,
			MailEnabledUserRecipientTypeDetails = 3689869554U,
			ExtensionNull = 2902411778U,
			Unsecured = 1573777228U,
			ConnectorIdIsNotAnInteger = 4079674260U,
			ErrorMissingPrimaryUM = 2617699176U,
			CannotDetermineDataSessionType = 3783332642U,
			UserAgentsChanges = 3601314308U,
			Notes = 1601836855U,
			EsnLangTelugu = 2395522212U,
			GroupNamingPolicyExtensionCustomAttribute1 = 65728472U,
			MailFlowPartnerInternalMailContentTypeNone = 1221325234U,
			DefaultRapName = 268472571U,
			DeleteUseDefaultAlert = 3648445463U,
			ErrorOrganizationResourceAddressListsCount = 4156100093U,
			EsnLangChineseSimplified = 3706505019U,
			ConferenceRoomMailboxRecipientTypeDetails = 1919306754U,
			BlockedOutlookClientVersionPatternDescription = 557877518U,
			UserHasNoSmtpProxyAddressWithFederatedDomain = 1448153692U,
			OrganizationCapabilityMailRouting = 2296213214U,
			SKUCapabilityBPOSSStandard = 1325366747U,
			SystemMailboxRecipientTypeDetails = 1850977098U,
			ExceptionADTopologyNoLocalDomain = 3702456775U,
			EsnLangDanish = 3504940969U,
			IndustryRetail = 779349581U,
			ErrorDDLNoSuchObject = 2864464497U,
			IndustryComputerRelatedProductsServices = 2713172052U,
			InternalRelay = 3288506612U,
			ErrorEmptyArchiveName = 602695546U,
			EmailAddressPolicyPriorityLowest = 1231743030U,
			ExternalMdm = 326106109U,
			TransportSettingsNotFoundException = 3799883362U,
			DomainSecureEnabledWithoutTls = 3552372249U,
			BccSuspiciousOutboundAdditionalRecipientsRequired = 1904959661U,
			NoRoleEntriesFound = 4291428005U,
			IndustryWholesale = 1515341016U,
			ServerRoleCentralAdminFrontEnd = 3980237751U,
			ErrorInvalidPushNotificationPlatform = 4068243401U,
			MailTipsAccessLevelAll = 2277405024U,
			PublicFolderRecipientTypeDetails = 1625030180U,
			ValueNotAvailableForUnchangedProperty = 943620946U,
			DumpsterFolder = 3641768400U,
			CannotParseMimeTypes = 1756055255U,
			ExclusiveRecipientScopes = 2518725308U,
			QuarantineMailboxIsInvalid = 1771494609U,
			MailboxPlanTypeDetails = 1094750789U,
			ServerRoleCafeArray = 986970413U,
			SendCredentialIsNull = 2874697860U,
			True = 3323264318U,
			StarAcceptedDomainCannotBeAuthoritative = 300685400U,
			AllRooms = 2302903917U,
			EsnLangRussian = 2650345129U,
			GroupNamingPolicyCustomAttribute10 = 3989445055U,
			SitesContainerNotFound = 79975170U,
			ExceptionServerTimeoutNegative = 1581344072U,
			ArchiveStateLocal = 665936024U,
			NotesMC = 991629433U,
			InvalidDomain = 3403459873U,
			EmailAgeFilterOneMonth = 2092969439U,
			FullDomain = 4021009371U,
			DeviceModel = 1430923151U,
			GroupRecipientType = 777275992U,
			RemoteSharedMailboxTypeDetails = 444885611U,
			LdapSearch = 261312351U,
			EsnLangArabic = 3378383244U,
			SKUCapabilityBPOSSDeskless = 3490390166U,
			ModeratedRecipients = 3131560893U,
			ExceptionRusOperationFailed = 1935657977U,
			ExceptionDomainInfoRpcTooBusy = 4110564385U,
			ErrorArchiveDomainInvalidInDatacenter = 3057991035U,
			PublicFolderRecipientType = 2922780138U,
			ErrorMessageClassHasUnsupportedWildcard = 2843126128U,
			ErrorPipelineTracingRequirementsMissing = 2976002772U,
			GroupNamingPolicyCustomAttribute11 = 2423361114U,
			ErrorMailTipMustNotBeEmpty = 65716788U,
			ComputerRecipientType = 1479699766U,
			ErrorArbitrationMailboxCannotBeModerated = 2493930652U,
			EsnLangKannada = 3119627512U,
			Title = 2435266816U,
			MessageWaitingIndicatorEnabled = 389262922U,
			PublicFolders = 3178475968U,
			Millisecond = 92440185U,
			StarAcceptedDomainCannotBeDefault = 2661434578U,
			ReceiveExtendedProtectionPolicyAllow = 2033242172U,
			ResourceMailbox = 2995964430U,
			ErrorThrottlingPolicyStateIsCorrupt = 3176413521U,
			MailEnabledNonUniversalGroupRecipientType = 3005725472U,
			ExternalAuthoritativeWithoutExchangeServerPermission = 573230607U,
			Authoritative = 2913015079U,
			ErrorPrimarySmtpAddressAndWindowsEmailAddressNotMatch = 2813895538U,
			PostMC = 1421974844U,
			UnknownConfigObject = 3819234583U,
			MalwareScanErrorActionAllow = 4274370117U,
			GroupNamingPolicyCustomAttribute6 = 1924734198U,
			InvalidTransportSyncLogSizeConfiguration = 2610662106U,
			WellKnownRecipientTypeMailGroups = 3725493575U,
			ADDriverStoreAccessTransientError = 120558192U,
			AACantChangeName = 635049629U,
			ContactItemsMC = 3247735886U,
			EsnLangKorean = 4170667976U,
			RssSubscriptionMC = 131007819U,
			LdapFilterErrorSpaceMiddleType = 1652093200U,
			GroupNamingPolicyCustomAttribute3 = 1924734193U,
			ExceptionNoFsmoRoleOwnerAttribute = 3167491578U,
			NonIpmRoot = 600983985U,
			ErrorTimeoutWritingSystemAddressListMemberCount = 2829159765U,
			ExceptionExternalError = 2900785706U,
			Calendar = 1292798904U,
			Wma = 2665399355U,
			ErrorInvalidDNDepth = 869401742U,
			CapabilityMasteredOnPremise = 1435812789U,
			EdgeSyncEhfConnectorFailedToDecryptPassword = 371000500U,
			ErrorArchiveDomainSetForNonArchive = 1758334214U,
			ExceptionObjectHasBeenDeleted = 3086681225U,
			EsnLangBengaliIndia = 2211212701U,
			PublicFolderServer = 613950136U,
			ErrorCannotSetPrimarySmtpAddress = 4127869723U,
			SpamFilteringActionQuarantine = 2852597951U,
			MailboxMoveStatusFailed = 1313260064U,
			SecurityPrincipalTypeUniversalSecurityGroup = 1169031248U,
			DynamicDLRecipientType = 1254627662U,
			ErrorNonTinyTenantShouldNotHaveSharedConfig = 2863183086U,
			CanRunRestoreState_Allowed = 3057941193U,
			DomainSecureWithIgnoreStartTLSEnabled = 1199714779U,
			GroupNamingPolicyExtensionCustomAttribute4 = 825243359U,
			UseMsg = 3705158290U,
			InvalidTenantFullSyncCookieException = 149761450U,
			AutoDatabaseMountDialGoodAvailability = 3241000569U,
			ForestTrust = 4056279737U,
			ErrorInvalidMailboxRelationType = 556546691U,
			ErrorDDLInvalidDNSyntax = 2757465550U,
			ByteEncoderTypeUseQP = 3615619130U,
			NoLocatorInformationInMServException = 2204790954U,
			SecurityPrincipalTypeGlobalSecurityGroup = 2004555878U,
			CannotGetUsefulSiteInfo = 2999553646U,
			ErrorPipelineTracingPathNotExist = 1031444357U,
			MailboxServer = 1832080745U,
			Blocked = 4019774802U,
			InvalidMainStreamCookieException = 2649572709U,
			MoveNotAllowed = 1341999288U,
			RemoteRoomMailboxTypeDetails = 1594549261U,
			SecurityPrincipalTypeUser = 1776235905U,
			TextEnrichedOnly = 2078807267U,
			BluetoothAllow = 3036506883U,
			GroupNamingPolicyDepartment = 154085973U,
			UseDefaultSettings = 3388588817U,
			ByteEncoderTypeUseQPHtmlDetectTextPlain = 3102724093U,
			Exchange2007 = 2924600836U,
			DisabledPartner = 788602100U,
			Consumer = 1344968854U,
			PrimaryMailboxRelationType = 1013979892U,
			Disabled = 1484405454U,
			SKUCapabilityBPOSSBasicCustomDomain = 1091797613U,
			ControlTextNull = 2308256473U,
			Outbox = 629464291U,
			ArchiveStateNone = 3086386447U,
			MailFlowPartnerInternalMailContentTypeMimeText = 1727459539U,
			CustomInternalBodyRequired = 2238564813U,
			TlsDomainWithIncorrectTlsAuthLevel = 2086215909U,
			SystemTag = 213405127U,
			AllMailboxContentMC = 65301504U,
			RemoteUserMailboxTypeDetails = 4145265495U,
			BluetoothDisable = 3441693128U,
			ServerRoleLanguagePacks = 2698858797U,
			PrincipalName = 1415894913U,
			IdIsNotSet = 3541826428U,
			ConstraintViolationSupervisionListEntryStringPartIsInvalid = 1254345332U,
			WellKnownRecipientTypeMailContacts = 2638599330U,
			ServerRoleHubTransport = 172810921U,
			IndustryHealthcare = 4251572755U,
			CapabilityPartnerManaged = 3956811795U,
			ErrorArchiveDatabaseArchiveDomainMissing = 4087147933U,
			MailEnabledUniversalSecurityGroupRecipientType = 2967905667U,
			ErrorRemovalNotSupported = 1486937545U,
			ExchangeFaxMC = 1129549138U,
			ByteEncoderTypeUse7Bit = 2611743021U,
			InvalidBindingAddressSetting = 2176173662U,
			ASAccessMethodNeedsAuthenticationAccount = 4172461161U,
			CanRunDefaultUpdateState_Allowed = 491964191U,
			EsnLangMalay = 2884121764U,
			FailedToParseAlternateServiceAccountCredential = 1167380226U,
			ExternalManagedMailContactTypeDetails = 3799817423U,
			IPv6Only = 1403090333U,
			MountDialOverrideLossless = 2827463711U,
			Percent = 3607366039U,
			ServerRoleProvisionedServer = 1922689150U,
			CalendarAgeFilterOneMonth = 2350890097U,
			TextOnly = 4169982073U,
			InvalidMsgTrackingLogSizeConfiguration = 1291987412U,
			ErrorArchiveDatabaseSetForNonArchive = 1881847987U,
			InvalidGenerationTime = 1940813882U,
			CalendarItemMC = 820626981U,
			Block = 3745862197U,
			ErrorNullExternalEmailAddress = 3519747066U,
			ExceptionRusNotRunning = 1681980649U,
			PropertyCannotBeSetToTest = 862550630U,
			LdapFilterErrorInvalidEscaping = 1641698628U,
			ForceSave = 867516332U,
			LinkedRoomMailboxRecipientTypeDetails = 1798526791U,
			DeleteUseCustomAlert = 3794956711U,
			CannotDeserializePartitionHint = 370123223U,
			InboundConnectorInvalidRestrictDomainsToIPAddresses = 1284675050U,
			GroupNamingPolicyCustomAttribute14 = 1663846227U,
			ContactRecipientType = 2839121159U,
			DomainSecureWithoutDNSRoutingEnabled = 2600481667U,
			RunspaceServerSettingsChanged = 3062547699U,
			EsnLangGreek = 3313482992U,
			TooManyEntriesError = 309994549U,
			OrganizationRelationshipMissingTargetApplicationUri = 1899391140U,
			ComputerRecipientTypeDetails = 3489169852U,
			Exchweb = 96146822U,
			OutboundConnectorIncorrectRouteAllMessagesViaOnPremises = 2202908911U,
			CalendarSharingFreeBusyAvailabilityOnly = 1499257862U,
			ServerRoleExtendedRole5 = 3707194059U,
			AutoAttendantLink = 390976058U,
			CustomRoleDescription_MyDisplayName = 4020865807U,
			AllUsers = 3949283739U,
			All = 4231482709U,
			OrganizationCapabilityMigration = 4256840071U,
			DialPlan = 142272059U,
			EsnLangUkrainian = 2868284030U,
			MessageRateSourceFlagsNone = 2052645173U,
			IndustryLegal = 2196808673U,
			CapabilityUMFeatureRestricted = 3379397641U,
			GroupTypeFlagsBuiltinLocal = 1494101274U,
			ReceiveAuthMechanismBasicAuthPlusTls = 2412300803U,
			Allowed = 3811183882U,
			ByteEncoderTypeUseQPHtml7BitTextPlain = 3579894660U,
			High = 4217035038U,
			MicrosoftExchangeRecipientType = 821502958U,
			BackSyncDataSourceUnavailableMessage = 1429014682U,
			ArchiveStateOnPremise = 110833865U,
			OrganizationCapabilitySuiteServiceStorage = 936096413U,
			MalwareScanErrorActionBlock = 1592544157U,
			SKUCapabilityBPOSSArchiveAddOn = 1924944146U,
			ExceptionRusAccessDenied = 4227895642U,
			ServerRoleNone = 2094315795U,
			AlternateServiceAccountConfigurationDisplayFormatMoreDataAvailable = 3352185505U,
			GloballyDistributedOABCacheWriteTimeoutError = 2728392679U,
			UserName = 3727360630U,
			Reserved1 = 1173768533U,
			NoAddresses = 3144162877U,
			RegionBlockListNotSet = 2817538580U,
			CapabilityRichCoexistence = 1398191848U,
			ErrorUserAccountNameIncludeAt = 2032072470U,
			Enabled = 634395589U,
			AttachmentsWereRemovedMessage = 3840534502U,
			ErrorCannotFindUnusedLegacyDN = 4176423907U,
			EmailAgeFilterOneWeek = 2318114319U,
			GroupNameInNamingPolicy = 2193124873U,
			OrganizationCapabilityClientExtensions = 2504573058U,
			CalendarAgeFilterTwoWeeks = 3081766090U,
			ErrorElcCommentNotAllowed = 583050472U,
			ErrorOwnersUpdated = 3275453767U,
			EsnLangIndonesian = 3776377092U,
			Extension = 2631270417U,
			CanEnableLocalCopyState_Invalid = 1468404604U,
			MailEnabledUniversalDistributionGroupRecipientType = 2146247679U,
			ReceiveCredentialIsNull = 893817173U,
			EsnLangLithuanian = 1885276797U,
			ServerRoleAll = 570563164U,
			ServerRoleEdge = 756854696U,
			ExceptionObjectStillExists = 982491582U,
			AllRecipients = 387112589U,
			LdapFilterErrorNoAttributeType = 3685369418U,
			ServerRoleManagementFrontEnd = 3802186670U,
			False = 2609910045U,
			CalendarSharingFreeBusyLimitedDetails = 519619317U,
			SystemAttendantMailboxRecipientType = 2662725163U,
			ServerRoleManagementBackEnd = 696678862U,
			GroupNamingPolicyStateOrProvince = 4088287609U,
			IndustryFinance = 1955666018U,
			ErrorAgeLimitExpiration = 3313162693U,
			InboundConnectorMissingTlsCertificateOrSenderIP = 2067616247U,
			ErrorMailTipTranslationFormatIncorrect = 1247338605U,
			MountDialOverrideGoodAvailability = 3892578519U,
			ConfigReadScope = 2615196936U,
			UserRecipientTypeDetails = 2338964630U,
			MeetingRequestMC = 3291024868U,
			Tag = 696030922U,
			MailFlowPartnerInternalMailContentTypeTNEF = 3607016823U,
			SerialNumberMissing = 1793427927U,
			AttributeNameNull = 2193656120U,
			ErrorIsDehydratedSetOnNonTinyTenant = 1260468432U,
			TUIPromptEditingEnabled = 4069918469U,
			StarAcceptedDomainCannotBeInitialDomain = 885421749U,
			LdapFilterErrorNotSupportSingleComp = 1860494422U,
			UseTnef = 1191236736U,
			AttachmentFilterEntryInvalid = 654334258U,
			Exchange2013 = 599002007U,
			SendAuthMechanismBasicAuthPlusTls = 3145869218U,
			MoveToDeletedItems = 3341243277U,
			TCP = 2403277311U,
			DocumentMC = 2664643149U,
			ErrorCannotSetWindowsEmailAddress = 1531250846U,
			Msn = 3115737588U,
			MessageRateSourceFlagsIPAddress = 619725344U,
			ErrorTextMessageIncludingAppleAttachment = 4163650158U,
			ForwardCallsToDefaultMailbox = 3857647582U,
			RoleGroupTypeDetails = 3221974997U,
			MailEnabledContactRecipientTypeDetails = 3815678973U,
			EsnLangEnglish = 647747116U,
			EsnLangMarathi = 2250496622U,
			SpecifyAnnouncementFileName = 1092312607U,
			GroupNamingPolicyCustomAttribute12 = 857277173U,
			SystemAddressListDoesNotExist = 3759553744U,
			DefaultOabName = 3499307032U,
			EsnLangSpanish = 1855185352U,
			FederatedOrganizationIdNoNamespaceAccount = 3227102809U,
			RemoteEquipmentMailboxTypeDetails = 1406382714U,
			SpamFilteringOptionOn = 2654331961U,
			ErrorNoSharedConfigurationInfo = 1495966060U,
			EquipmentMailboxRecipientTypeDetails = 3938481035U,
			ErrorCannotSetMoveToDestinationFolder = 375049999U,
			CapabilityTOUSigned = 290262264U,
			ServerRoleExtendedRole2 = 3707194054U,
			ServerRoleExtendedRole3 = 3707194053U,
			PersonalFolder = 2283186478U,
			CapabilityNone = 584737882U,
			ErrorEmptyResourceTypeofResourceMailbox = 31546440U,
			InternalDNSServersNotSet = 2469247251U,
			ExceptionImpersonation = 4205089983U,
			ReceiveAuthMechanismNone = 3072026616U,
			GroupNamingPolicyCustomAttribute9 = 1924734199U,
			MailEnabledDynamicDistributionGroupRecipientTypeDetails = 2999125469U,
			SpamFilteringActionAddXHeader = 685401583U,
			RecentCommands = 141120823U,
			SecurityPrincipalTypeNone = 2690725740U,
			MailboxMoveStatusNone = 1589279983U,
			LocalForest = 3976915092U,
			LegacyMailboxRecipientTypeDetails = 221683052U,
			GroupNamingPolicyCustomAttribute2 = 1924734194U,
			DatabaseMasterTypeUnknown = 661425765U,
			ConversationHistory = 2630084427U,
			OutboundConnectorTlsSettingsInvalidDomainValidationWithoutTlsDomain = 2338066360U,
			WhenMoved = 282367765U,
			ErrorDuplicateLanguage = 1039356237U,
			ExceptionObjectAlreadyExists = 1268762784U,
			EsnLangCzech = 908880307U,
			ComponentNameInvalid = 3859838333U,
			ErrorAuthMetadataCannotResolveIssuer = 3587044099U,
			GroupNamingPolicyTitle = 4137211921U,
			MailboxMoveStatusSuspended = 3738926850U,
			DomainSecureEnabledWithExternalAuthoritative = 1795765194U,
			BasicAfterTLSWithoutTLS = 4014391034U,
			Private = 3026477473U,
			Mailboxes = 1481245394U,
			ErrorModeratorRequiredForModeration = 1548074671U,
			CustomFromAddressRequired = 51460946U,
			LdapModifyDN = 1951705699U,
			CustomExternalSubjectRequired = 2453785463U,
			ErrorInternalLocationsCountMissMatch = 2119492277U,
			ASOnlyOneAuthenticationMethodAllowed = 2620688997U,
			Tnef = 4001967317U,
			ByteEncoderTypeUseBase64HtmlDetectTextPlain = 2746045715U,
			EsnLangIcelandic = 1347571030U,
			ServerRoleNAT = 2324863376U,
			UniversalDistributionGroupRecipientTypeDetails = 1966081841U,
			ErrorReplicationLatency = 2222835032U,
			EnabledPartner = 461546579U,
			OutboundConnectorSmarthostTlsSettingsInvalid = 1942443133U,
			ExternalCompliance = 1538151754U,
			ErrorAuthMetadataNoSigningKey = 1293877238U,
			InboundConnectorIncorrectAllAcceptedDomains = 2521212798U,
			MoveToFolder = 1182470434U,
			Byte = 1421152560U,
			EsnLangCyrillic = 2557668279U,
			CanRunDefaultUpdateState_Invalid = 1711185212U,
			DisabledUserRecipientTypeDetails = 3569405894U,
			InvalidRecipientType = 725514504U,
			EmailAgeFilterThreeDays = 532726102U,
			DataMoveReplicationConstraintCISecondCopy = 3099081087U,
			ErrorMissingPrimarySmtp = 1499716658U,
			ErrorELCFolderNotSpecified = 398140363U,
			ErrorCannotHaveMoreThanOneDefaultThrottlingPolicy = 4174419723U,
			ReceiveModeCannotBeZero = 4070703744U,
			OwaDefaultDomainRequiredWhenLogonFormatIsUserName = 3248373105U,
			TLS = 2806561839U,
			LinkedMailboxRecipientTypeDetails = 1432667858U,
			Tasks = 2966158940U,
			RejectAndQuarantineThreshold = 1982036425U,
			LdapFilterErrorInvalidDecimal = 1213745183U,
			SpamFilteringTestActionAddXHeader = 2461262221U,
			OrganizationCapabilityScaleOut = 1880194541U,
			ConstraintViolationOneOffSupervisionListEntryStringPartIsInvalid = 3454173033U,
			DiscoveryMailboxTypeDetails = 104454802U,
			ErrorAdfsTrustedIssuers = 826243425U,
			DataMoveReplicationConstraintCIAllDatacenters = 2167266391U,
			HygieneSuiteStandard = 396559062U,
			EsnLangHindi = 291819084U,
			ExceptionUnableToCreateConnections = 708782482U,
			SecurityPrincipalTypeWellknownSecurityPrincipal = 796260281U,
			Error = 22442200U,
			ElcScheduleOnWrongServer = 1999329050U,
			SyncIssues = 3694564633U,
			PartiallyApplied = 3184119847U,
			PreferredInternetCodePageUndefined = 361358848U,
			NoRoleEntriesCmdletOrScriptFound = 3399683424U,
			CannotDeserializePartitionHintTooShort = 2126631961U,
			InvalidReceiveAuthModeTLSPassword = 2412912437U,
			GroupNamingPolicyCustomAttribute8 = 1924734200U,
			EsnLangSwedish = 401605495U,
			IndustryUtilities = 2253071944U,
			G711 = 3692015522U,
			ExternalDNSServersNotSet = 3044377029U,
			Item = 3168546709U,
			LdapFilterErrorUnsupportedAttributeType = 3325431492U,
			ExternalSenderAdminAddressRequired = 2932678028U,
			ErrorBadLocalizedFolderName = 582855761U,
			AutoDatabaseMountDialBestAvailability = 3297182182U,
			OrganizationalFolder = 391848840U,
			SpamFilteringOptionTest = 2944126402U,
			LdapFilterErrorInvalidToken = 56024811U,
			MessageRateSourceFlagsUser = 2570241570U,
			TextEnrichedAndTextAlternative = 1461717404U,
			FederatedOrganizationIdNoFederatedDomains = 3920082026U,
			GroupTypeFlagsUniversal = 1191186633U,
			CustomAlertTextRequired = 3774252481U,
			EsnLangEstonian = 1272682565U,
			Low = 1502599728U,
			IndustryPersonalServices = 467677052U,
			ErrorInvalidPipelineTracingSenderAddress = 3282711248U,
			AccessQuarantined = 131691172U,
			LdapFilterErrorTypeOnlySpaces = 3874249800U,
			UserFilterChoice = 2878385120U,
			ErrorRemovePrimaryExternalSMTPAddress = 587065991U,
			GroupNamingPolicyOffice = 62599113U,
			ErrorHostServerNotSet = 3503686282U,
			BitMaskOrIpAddressMatchMustBeSet = 1998007652U,
			OrganizationCapabilityGMGen = 621310157U,
			ErrorArchiveDatabaseArchiveDomainConflict = 3723962467U,
			ArchiveStateHostedProvisioned = 2472951404U,
			InvalidHttpProtocolLogSizeConfiguration = 614706510U,
			PermanentMservErrorDescription = 3225083443U,
			CustomExternalBodyRequired = 1285289871U,
			LdapFilterErrorUndefinedAttributeType = 1415475463U,
			ErrorTextMessageIncludingHtmlBody = 603975640U,
			WellKnownRecipientTypeResources = 3773054995U,
			PrimaryDefault = 2097957443U,
			MailFlowPartnerInternalMailContentTypeMimeHtmlText = 2943465798U,
			DataMoveReplicationConstraintNone = 2685207586U,
			ErrorAdfsAudienceUris = 3040710945U,
			InvalidAnrFilter = 1190928622U,
			AuditLogMailboxRecipientTypeDetails = 117943812U,
			WellKnownRecipientTypeNone = 1849540794U,
			EsnLangGujarati = 3296665743U,
			DomainStateUnknown = 672991527U,
			IndustryManufacturing = 687591962U,
			IndustryHospitality = 345329738U,
			ErrorAdfsIssuer = 534671299U,
			EmailAgeFilterOneDay = 1058308747U,
			AllEmailMC = 3471478219U,
			OrgContainerAmbiguousException = 1753080574U,
			GlobalThrottlingPolicyNotFoundException = 1439034128U,
			EsnLangTurkish = 3636659332U,
			SKUCapabilityBPOSSLite = 1642025802U,
			RecipientWriteScopes = 2794974035U,
			CalendarAgeFilterThreeMonths = 104189932U,
			MailboxMoveStatusCompletedWithWarning = 2669194754U,
			GroupNamingPolicyCountryOrRegion = 3674978674U,
			EsnLangFrench = 2482287296U,
			CapabilityExcludedFromBackSync = 2050489682U,
			CapabilityBEVDirLockdown = 452620031U,
			ReceiveAuthMechanismBasicAuth = 4409738U,
			IndustryEducation = 2046074250U,
			NotSpecified = 2536752615U,
			PermanentlyDelete = 3675904764U,
			FederatedIdentityMisconfigured = 2346580185U,
			MountDialOverrideNone = 3845937663U,
			AlwaysUTF8 = 3563162252U,
			ExceptionPagedReaderIsSingleUse = 2660137110U,
			InvalidFilterLength = 3323087513U,
			MailboxMoveStatusSynced = 3664117547U,
			SIPSecured = 2422734853U,
			ErrorRejectedCookie = 630988704U,
			ASInvalidProxyASUrlOption = 3800196293U,
			ServerRoleSCOM = 407788899U,
			JournalItemsMC = 3289792773U,
			ErrorEmptySearchProperty = 2617145200U,
			OutboundConnectorIncorrectTransportRuleScopedParameters = 3900005531U,
			TeamMailboxRecipientTypeDetails = 107906018U,
			CustomRoleDescription_MyMobileInformation = 4272675708U,
			ArchiveStateHostedPending = 920444171U,
			DPCantChangeName = 2153511661U,
			OrganizationCapabilityUMDataStorage = 2175447826U,
			TlsAuthLevelWithRequireTlsDisabled = 2304217557U,
			UndefinedRecipientTypeDetails = 3453679227U,
			Upgrade = 3608358242U,
			Global = 3905558735U,
			DeleteMessage = 4264103832U,
			LdapDelete = 4018720312U,
			EsnLangHungarian = 1291341805U,
			ErrorAddressAutoCopy = 699909606U,
			EsnLangLatvian = 2968709845U,
			CanRunDefaultUpdateState_NotLocal = 674244647U,
			Department = 1855823700U,
			SpamFilteringActionJmf = 1123996746U,
			ErrorDDLOperationsError = 1079159280U,
			ErrorSharedConfigurationCannotBeEnabled = 64564864U,
			ErrorMailTipCultureNotSpecified = 1411554219U,
			LdapModify = 2068838733U,
			DataMoveReplicationConstraintSecondDatacenter = 2212942115U,
			CapabilityResourceMailbox = 3501307892U,
			Second = 2955006930U,
			InboundConnectorInvalidRestrictDomainsToCertificate = 451948526U,
			GroupNamingPolicyCustomAttribute15 = 97762286U,
			SendAuthMechanismNone = 1669305113U,
			ServicesContainerNotFound = 2028679986U,
			MissingDefaultOutboundCallingLineId = 368981658U,
			GroupTypeFlagsDomainLocal = 1638178773U,
			ErrorCannotAggregateAndLinkMailbox = 2292597411U,
			SyncCommands = 1975373491U,
			PreferredInternetCodePageEsc2022Jp = 2584752109U,
			DirectoryBasedEdgeBlockModeOff = 2869997774U,
			InvalidSourceAddressSetting = 3205211544U,
			ElcContentSettingsDescription = 3459813102U,
			ServerRoleUnifiedMessaging = 3194934827U,
			DataMoveReplicationConstraintCIAllCopies = 1193235970U,
			MailTipsAccessLevelLimited = 96477845U,
			SecondaryMailboxRelationType = 4229158936U,
			Ocs = 3828198519U,
			IndustryOther = 2122644134U,
			ErrorMimeMessageIncludingUuEncodedAttachment = 3032910929U,
			ServerRoleDHCP = 1886413222U,
			GroupNamingPolicyCustomAttribute5 = 1924734195U,
			EnableNotificationEmail = 102260678U,
			GroupNamingPolicyCountryCode = 4022404286U,
			MailboxMoveStatusCompleted = 4204248234U,
			IndustryCommunications = 2831291713U,
			LdapFilterErrorNoValidComparison = 2559242555U,
			RssSubscriptions = 3598244064U,
			EsnLangThai = 1071018894U,
			ErrorDDLFilterMissing = 2921549042U,
			ExtendedProtectionNonTlsTerminatingProxyScenarioRequireTls = 1447606358U,
			NoResetOrAssignedMvp = 267717298U,
			MountDialOverrideBestEffort = 663506969U,
			NoComputers = 2367428005U,
			RegistryContentTypeException = 665042539U,
			DataMoveReplicationConstraintAllDatacenters = 366040629U,
			ExceptionObjectNotFound = 2564080149U,
			DomainStateCustomProvision = 882963645U,
			SKUCapabilityBPOSMidSize = 3517179940U,
			LdapFilterErrorUnsupportedOperand = 1117900463U,
			DirectoryBasedEdgeBlockModeDefault = 483196058U,
			ErrorWrongTypeParameter = 113073592U,
			EsnLangCatalan = 2757326190U,
			InvalidSndProtocolLogSizeConfiguration = 3980183679U,
			GroupNamingPolicyCustomAttribute13 = 3586160528U,
			ErrorThrottlingPolicyGlobalAndOrganizationScope = 1960526324U,
			SMTPAddress = 980672066U,
			EsnLangPolish = 3976700013U,
			CanEnableLocalCopyState_DatabaseEnabled = 1017523965U,
			EsnLangRomanian = 3315201717U,
			ExternalManagedGroupTypeDetails = 1097129869U,
			DatabaseMasterTypeDag = 2773964607U,
			GroupNamingPolicyExtensionCustomAttribute3 = 3197896354U,
			ExchangeConfigurationContainerNotFoundException = 3071618850U,
			EsnLangUrdu = 170342216U,
			MservAndMbxExclusive = 579329341U,
			FirstLast = 2300412432U,
			EsnLangBulgarian = 2228665429U,
			MailEnabledUniversalSecurityGroupRecipientTypeDetails = 1970247521U,
			ErrorTimeoutReadingSystemAddressListMemberCount = 3065017355U,
			FaxServerURINoValue = 3411768540U,
			ErrorDefaultThrottlingPolicyNotFound = 1118762177U,
			ErrorRecipientContainerCanNotNull = 3372089172U,
			MoveToArchive = 2835967712U,
			ModifySubjectValueNotSet = 667068758U,
			NotLocalMaiboxException = 882536335U,
			RecipientReadScope = 3811978523U,
			Organizational = 1067650092U,
			SystemAttendantMailboxRecipientTypeDetails = 1818643265U,
			OrganizationCapabilityOABGen = 114732651U,
			StarOutToDialPlanEnabled = 2360810543U,
			AuthenticationCredentialNotSet = 3673152204U,
			NotifyOutboundSpamRecipientsRequired = 1776441609U,
			JunkEmail = 2241039844U,
			LdapFilterErrorValueOnlySpaces = 2270844793U,
			SipName = 3423767853U,
			EsnLangMalayalam = 24965481U,
			SpamFilteringActionModifySubject = 2349327181U,
			XHeaderValueNotSet = 1153697179U,
			DeletedItems = 3613623199U,
			OrganizationCapabilityUMGrammarReady = 3387472355U,
			LastFirst = 142823596U,
			SendAuthMechanismExchangeServer = 2055652669U,
			RemoteTeamMailboxRecipientTypeDetails = 322963092U,
			OutOfBudgets = 1068346025U,
			Off = 3424913979U,
			GroupTypeFlagsSecurityEnabled = 3200416695U,
			InvalidCookieException = 2618688392U,
			UserLanguageChoice = 122679092U,
			SpamFilteringTestActionBccMessage = 1291237470U,
			DelayCacheFull = 1118847720U,
			ErrorAutoCopyMessageFormat = 1149691394U,
			Reserved3 = 1173768531U,
			HtmlOnly = 2523055253U,
			DefaultFolder = 285356425U,
			PublicFolderMailboxRecipientTypeDetails = 1487832074U,
			Mp3 = 1549653732U,
			FederatedOrganizationIdNotEnabled = 2737500906U,
			EsnLangVietnamese = 3082202919U,
			AccessGranted = 2532765903U,
			MailboxUserRecipientType = 4281433724U,
			ExceptionNoSchemaContainerObject = 1183009861U,
			TargetDeliveryDomainCannotBeStar = 2078410195U,
			ErrorAuthMetadataCannotResolveServiceName = 2402032744U,
			ByteEncoderTypeUseBase64 = 4046275528U,
			BackSyncDataSourceReplicationErrorMessage = 2114338030U,
			EsnLangHebrew = 2327110479U,
			WellKnownRecipientTypeAllRecipients = 2099880135U,
			ExceptionCredentialsNotSupportedWithoutDC = 33168083U,
			NoneMailboxRelationType = 247236896U,
			MailboxUserRecipientTypeDetails = 1605633982U,
			SpamFilteringActionDelete = 3918345138U,
			FederatedOrganizationIdNotFound = 1574700905U,
			SKUCapabilityBPOSSArchive = 3777672192U,
			ReceiveAuthMechanismIntegrated = 1038035039U,
			NameLookupEnabled = 1607061032U,
			ForceFilter = 1972085753U,
			OrganizationCapabilityOfficeMessageEncryption = 3515139435U,
			PreferredInternetCodePageIso2022Jp = 906595693U,
			AlternateServiceAccountCredentialIsInvalid = 1412620754U,
			EmailAgeFilterTwoWeeks = 1859518684U,
			DeviceOS = 2080073494U,
			ErrorTenantRelocationsAllowedOnlyForRootOrg = 1451782196U,
			OrganizationCapabilityTenantUpgrade = 3809750167U,
			StarTlsDomainCapabilitiesNotAllowed = 426414486U,
			GroupNamingPolicyExtensionCustomAttribute5 = 2391327300U,
			ErrorTimeoutWritingSystemAddressListCache = 1103339046U,
			CannotGetLocalSite = 2249628033U,
			DatabaseCopyAutoActivationPolicyUnrestricted = 3080481085U,
			PrivateComputersOnly = 3562221485U,
			Always = 887700241U,
			WellKnownRecipientTypeMailUsers = 933193541U,
			CannotSetZeroAsEapPriority = 3512186809U,
			RootZone = 2442344752U,
			RenameNotAllowed = 1151884593U,
			Unknown = 2846264340U,
			EsnLangItalian = 4013633336U,
			ErrorDisplayNameInvalid = 2446612004U,
			ConstraintViolationNotValidLegacyDN = 10930364U,
			ReceiveExtendedProtectionPolicyRequire = 3650953906U,
			SpamFilteringOptionOff = 2030161115U,
			ExternallyManaged = 1656602441U,
			RequireTLSWithoutTLS = 3909129905U,
			ErrorCannotParseAuthMetadata = 1437476905U,
			ErrorInvalidActivationPreference = 2411242862U,
			CapabilityFederatedUser = 1499015349U,
			EsnLangFilipino = 992862894U,
			OutboundConnectorUseMXRecordShouldBeFalseIfSmartHostsIsPresent = 2687926967U,
			LdapFilterErrorBracketMismatch = 1688256845U,
			SipResourceIdentifierRequiredNotAllowed = 843851219U,
			XMSWLHeader = 2178386640U,
			ServerRoleCafe = 1536572748U,
			DeleteAndRejectThreshold = 3319415544U,
			Policy = 816661212U,
			CanRunRestoreState_NotLocal = 2870485117U,
			ElcAuditLogPathMissing = 2379521528U,
			ClientCertAuthIgnore = 4221359213U,
			Reserved2 = 1173768532U,
			ConfigWriteScopes = 3743229054U,
			DetailsTemplateCorrupted = 3856328942U,
			ClientCertAuthAccepted = 1942592476U,
			ExceptionAdminLimitExceeded = 1778180980U,
			DataMoveReplicationConstraintSecondCopy = 970530017U,
			ReceiveAuthMechanismTls = 3956092407U,
			CannotFindTemplateTenant = 990840756U,
			FailedToReadStoreUserInformation = 2189879122U,
			MicrosoftExchangeRecipientTypeDetails = 2227674028U,
			DataMoveReplicationConstraintCINoReplication = 1188824751U,
			ErrorTransitionCounterHasZeroCount = 575137052U,
			DeleteAndQuarantineThreshold = 73047031U,
			IndustryAgriculture = 1642796619U,
			ClientCertAuthRequired = 3603173284U,
			ServerRoleExtendedRole7 = 3707194057U,
			SubmissionOverrideListOnWrongServer = 2804536165U,
			EsnLangBasque = 4170864973U,
			UserRecipientType = 659240048U,
			MailEnabledUserRecipientType = 363625972U,
			GroupTypeFlagsGlobal = 4189167987U,
			DataMoveReplicationConstraintCISecondDatacenter = 4091901749U,
			LoadBalanceCannotUseBothInclusionLists = 4024084584U,
			ExchangeMissedcallMC = 2771491650U,
			RequesterNameInvalid = 3414623930U,
			ByteEncoderTypeUseBase64Html7BitTextPlain = 3393062226U,
			SecurityPrincipalTypeComputer = 2666751303U,
			EsnLangAmharic = 3235051081U,
			LimitedMoveOnlyAllowed = 3212819533U,
			ASInvalidAuthenticationOptionsForAccessMethod = 2828547743U,
			NullPasswordEncryptionKey = 1241097582U,
			LinkedUserTypeDetails = 1738880682U,
			AutoDatabaseMountDialLossless = 3409499789U,
			ReceiveAuthMechanismExternalAuthoritative = 3238398976U,
			ErrorTruncationLagTime = 1472430496U,
			ExceptionIdImmutable = 1996912758U,
			ExceptionDefaultScopeAndSearchRoot = 1638589089U,
			ErrorOfferProgramIdMandatoryOnSharedConfig = 3667281372U,
			ServerRoleExtendedRole4 = 3707194060U,
			ErrorComment = 3615458703U,
			ErrorReplayLagTime = 4244443796U,
			ExLengthOfVersionByteArrayError = 64170653U,
			LdapAdd = 3109296950U,
			DomainStatePendingActivation = 1578973890U,
			Uninterruptible = 3790383196U,
			ErrorMustBeADRawEntry = 3789585333U,
			None = 1414246128U,
			ErrorBadLocalizedComment = 1049375487U,
			EsnLangSlovak = 1223035494U,
			LdapFilterErrorInvalidBooleanValue = 3378717027U,
			OabVersionsNullException = 1343826401U,
			Inbox = 2979702410U,
			ContactRecipientTypeDetails = 137387861U,
			EsnLangKazakh = 1522344710U,
			DisableFilter = 1662145344U,
			BluetoothHandsfreeOnly = 3713089550U,
			GatewayGuid = 3178378607U,
			CalendarSharingFreeBusyNone = 3927045149U
		}

		private enum ParamIDs
		{
			ExceptionADWriteDisabled,
			MobileAdOrphanFound,
			ExceptionInvalidOperationOnReadOnlyObject,
			ErrorUnsafeIdentityFilterNotAllowed,
			ErrorProductFileDirectoryIdenticalWithCopyFileDirectory,
			ErrorIsServerSuitableMissingDefaultNamingContext,
			ErrorBothTargetAndSourceForestPopulated,
			UnsupportedObjectClass,
			DefaultAdministrativeGroupNotFoundException,
			PropertyDependencyRequired,
			ProviderFactoryClassNotFoundLoadException,
			ConfigurationSettingsRestrictionSummary,
			TenantNotFoundInGlsError,
			InvalidCallSomeoneScopeSettings,
			ConfigurationSettingsNotUnique,
			KpkUseProblem,
			ForwardingSmtpAddressNotValidSmtpAddress,
			ConfigurationSettingsGroupNotFound,
			MoreThanOneRecipientWithNetId,
			CannotResolvePartitionFqdnError,
			ErrorPublicFolderReferralConflict,
			ExceptionADTopologyCreationTimeout,
			ExceptionADOperationFailedEntryAlreadyExist,
			ADTopologyEndpointNotFoundException,
			AsyncTimeout,
			ErrorNonUniqueExchangeGuid,
			InvalidSyncObjectId,
			AggregatedSessionCannotMakeADChanges,
			ErrorInvalidRemoteRecipientType,
			TenantIsRelocatedException,
			ErrorSingletonMailboxLocationType,
			DuplicateHolidaysError,
			UnknownAccountForest,
			ExchangeUpgradeBucketInvalidVersionFormat,
			InvalidControlAttributeForTemplateType,
			ExceptionUnsupportedFilter,
			ErrorNonUniqueDomainAccount,
			PilotingOrganization_Error,
			ConfigScopeMustBeEmpty,
			ErrorDuplicateManagedFolderAddition,
			ErrorInvalidConfigScope,
			EmailAddressPolicyPriorityLowestFormatError,
			InvalidSubnetNameFormat,
			ErrorServerRoleNotSupported,
			ServiceInstanceContainerNotFoundException,
			InvalidAttachmentFilterRegex,
			ErrorAdfsAudienceUriFormat,
			NspiRpcError,
			ExceptionConflictingArguments,
			ExceptionNoSchemaMasterServerObject,
			ErrorMailboxExistsInCollection,
			ConfigurationSettingsDriverNotInitialized,
			InvalidPartitionFqdn,
			RoleIsMandatoryInRoleAssignment,
			MsaUserNotFoundInGlsError,
			BackSyncDataSourceInDifferentSiteMessage,
			CannotResolveTenantRelocationRequestIdentity,
			FFOMigration_Error,
			ExceptionTokenGroupsNeedsDomainSession,
			ErrorAdfsAudienceUriDup,
			ReplicationNotComplete,
			UserIsMandatoryInRoleAssignment,
			ConfigurationSettingsInvalidMatch,
			TenantIsLockedDownForRelocationException,
			ExceptionInvalidApprovedApplication,
			ErrorNonUniqueProxy,
			ExceptionWKGuidNeedsGCSession,
			ErrorPolicyDontSupportedPresentationObject,
			ErrorIsServerSuitableInvalidOSVersion,
			SipUriAlreadyRegistered,
			WrongDCForCurrentPartition,
			BPOS_S_Policy_License_Violation,
			ExceptionExtendedRightsNotUnique,
			ExceptionGuidSearchRootWithDefaultScope,
			ErrorSettingOverrideUnknown,
			ErrorIncorrectlyModifiedMailboxCollection,
			ConfigurationSettingsRestrictionExtraProperty,
			ConstraintViolationInvalidRecipientType,
			UnableToResolveMapiIdException,
			ErrorSettingOverrideInvalidVariantName,
			ErrorWebDistributionEnabledWithoutVersion4,
			CannotResolveAccountForestDnError,
			MobileMetabasePathIsInvalid,
			InvalidAttachmentFilterExtension,
			ErrorInvalidAuthSettings,
			ErrorExchangeGroupNotFound,
			ErrorSettingOverrideInvalidVariantValue,
			UnKnownScopeRestrictionType,
			NonexistentTimeZoneError,
			ErrorExceededMultiTenantResourceCountQuota,
			AddressBookNotFoundException,
			ExtensionIsInvalid,
			AppendLocalizedStrings,
			SuitabilityExceptionLdapSearch,
			ErrorExchangeMailboxExists,
			ExceptionInvalidVlvFilterProperty,
			ExceptionADInvalidHandleCookie,
			ErrorAdfsTrustedIssuerFormat,
			KpkAccessProblem,
			ApiDoesNotSupportInputFormatError,
			DomainAlreadyExistsInMserv,
			CannotResolveTenantName,
			ErrorInvalidPrivateCertificate,
			ExceptionSearchRootNotChildOfDefaultScope,
			ErrorLegacyVersionOfflineAddressBookWithoutPublicFolderDatabase,
			DefaultDatabaseAvailabilityGroupContainerNotFoundException,
			ErrorSettingOverrideInvalidSectionName,
			ErrorSubnetMaskGreaterThanAddress,
			InvalidServiceInstanceIdException,
			EndpointContainerNotFoundException,
			ErrorIsServerSuitableRODC,
			TransientMservError,
			ExceptionResourceUnhealthy,
			CannotGetComputerName,
			ProviderBadImpageFormatLoadException,
			ConfigurationSettingsOrganizationNotFound,
			CannotFindTenantByMSAUserNetID,
			RecordValueFormatChange,
			LitigationHold_License_Violation,
			ErrorInvalidISOCountryCode,
			ErrorServiceEndpointNotFound,
			ErrorLinkedADObjectNotInSameOrganization,
			ExceptionProxyGeneratorDLLFailed,
			BPOS_License_NumericLimitViolation,
			SessionSubscriptionDisabled,
			InvalidControlTextLength,
			RelocationInProgress,
			ExceptionFailedToRebuildConnection,
			RootCannotBeEmpty,
			TenantIsArrivingException,
			TenantPerimeterConfigSettingsNotFoundException,
			ConfigScopeCannotBeEmpty,
			InvalidDNFormat,
			ErrorTransitionCounterHasDuplicateEntry,
			ExceptionADOperationFailedRemoveContainer,
			RecipientWriteScopeNotLessThan,
			CannotResolveTenantNameByAcceptedDomain,
			ExceptionSearchRootChildDomain,
			ErrorRemovedMailboxDoesNotHaveDatabase,
			AlternateServiceAccountCredentialDisplayFormat,
			ErrorCannotSaveBecauseTooNew,
			InvalidRootDse,
			ExceptionCannotAddSidHistory,
			ErrorInvalidServerFqdn,
			ExceptionSearchRootNotChildOfSessionSearchRoot,
			ErrorRemovedMailboxDoesNotHaveMailboxGuid,
			ExceptionADTopologyServiceDown,
			CannotCalculatePropertyGeneric,
			ExceptionADConstraintViolation,
			ADTreeDeleteNotFinishedException,
			InvalidAAConfiguration,
			PermanentGlsError,
			BPOS_S_Property_License_Violation,
			CannotResolveTenantNameByExternalDirectoryId,
			BEVDirLockdown_Violation,
			ErrorProperty1EqProperty2,
			ErrorNonUniqueSid,
			ExceptionADTopologyErrorWhenLookingForGlobalCatalogsInForest,
			ExceptionObjectPartitionDoesNotMatchSessionPartition,
			ExceptionADTopologyHasNoServersInForest,
			WACDiscoveryEndpointShouldBeAbsoluteUri,
			MasteredOnPremiseCapabilityUndefinedTenantNotDirSyncing,
			CannotGetDnFromGuid,
			ExceptionObjectHasBeenDeletedDuringCurrentOperation,
			CalculatedPropertyFailed,
			ApiNotSupportedInBusinessSessionError,
			RusInvalidFilter,
			ErrorNoSuitableDCInDomain,
			SuitabilityErrorDNS,
			ExceptionGetLocalSiteArgumentException,
			InvalidSyncLinkFormat,
			ErrorDuplicatePartnerApplicationId,
			ErrorAccountPartitionCantBeLocalAndHaveTrustAtTheSameTime,
			ErrorADResponse,
			ExceptionOwaCannotSetPropertyOnLegacyVirtualDirectory,
			UnrecognizedRoleEntryType,
			FailedToUpdateEmailAddressesForExternal,
			ExceptionErrorFromRUS,
			ErrorSubnetMaskOutOfRange,
			NonUniquePilotIdentifier,
			ErrorThresholdMustBeSet,
			ErrorProperty1NeValue1WhileProperty2EqValue2,
			InvalidAutoAttendantSetting,
			InvalidFilterSize,
			ServerComponentReadADError,
			ErrorLogFolderPathEqualsCopyLogFolderPath,
			ErrorArchiveMailboxExists,
			InvalidCrossTenantIdFormat,
			CustomRecipientWriteScopeCannotBeEmpty,
			CannotBuildAuthenticationTypeFilterNoNamespacesOfType,
			ErrorSystemFolderPathNotEqualLogFolderPath,
			LegacyGwartNotFoundException,
			ErrorSystemFolderPathEqualsCopySystemFolderPath,
			ExceptionWKGuidNeedsDomainSession,
			ErrorReportToManagedEnabledWithoutManager,
			ThrottlingPolicyCorrupted,
			ExceptionOwaCannotSetPropertyOnE14MailboxPolicyToNull,
			EXOStandardRestrictions_Error,
			InvalidDialPlan,
			ErrorSubnetAddressDoesNotMatchMask,
			ErrorProperty1GtProperty2,
			InvalidNonPositiveResourceThreshold,
			GlsEndpointNotFound,
			InvalidWaveFilename,
			CannotFindTenantCUByExternalDirectoryId,
			InvalidSyncCompanyId,
			CustomRecipientWriteScopeMustBeEmpty,
			ErrorReportToBothManagerAndOriginator,
			PublicFolderReferralServerNotExisting,
			ErrorNullRecipientTypeInPrecannedFilter,
			ProviderFileLoadException,
			SharedConfigurationNotFound,
			ExceptionFilterWithNullValue,
			ErrorPrimarySmtpTemplateInvalid,
			ExceptionOwaCannotSetPropertyOnLegacyMailboxPolicy,
			InvalidControlAttributeName,
			ErrorProperty1LtProperty2,
			PartnerManaged_Violation,
			ConfigurationSettingsInvalidPriority,
			ErrorParseCountryInfo,
			ErrorMailTipTranslationCultureNotSupported,
			CantSetDialPlanProperty,
			InvalidCustomGreetingFilename,
			ExceptionCannotBindToDC,
			ExceptionUnsupportedFilterForPropertyMultiple,
			ExArgumentNullException,
			ExceptionSearchRootNotWithinScope,
			ExceptionTimelimitExceeded,
			ExceptionADRetryOnceOperationFailed,
			ErrorNeutralCulture,
			ExceptionInvalidOperationOnObject,
			ServerSideADTopologyServiceCallError,
			ExceptionWin32OperationFailed,
			ErrorDCNotFound,
			AddressBookNoSecurityDescriptor,
			NotInWriteToMbxMode,
			ErrorAuthServerNotFound,
			ErrorProductFileNameDifferentFromCopyFileName,
			ValueNotInRange,
			ErrorRemoteAccountPartitionMustHaveTrust,
			ErrorMasterServerInvalid,
			ErrorInvalidExecutingOrg,
			ErrorInvalidMailboxProvisioningConstraint,
			CannotMakePrimary,
			MsaUserAlreadyExistsInGlsError,
			ServerComponentReadTimeout,
			InvalidOABMapiPropertyParseStringException,
			ErrorRecipientDoesNotExist,
			CannotCompareScopeObjects,
			ExceptionADTopologyErrorWhenLookingForServersInDomain,
			CannotFindTenantCUByAcceptedDomain,
			ComposedSuitabilityReachabilityError,
			ErrorNoWriteScope,
			ErrorNonUniqueDN,
			CannotCompareAssignmentsMissingScope,
			InvalidInfluence,
			EOPPremiumRestrictions_Error,
			AmbiguousTimeZoneNameError,
			InvalidRoleEntryType,
			MailboxPropertiesMustBeClearedFirst,
			UnexpectedGlsError,
			NotInWriteToMServMode,
			ErrorEmptyString,
			ExceptionSizelimitExceeded,
			ErrorRealmFormatInvalid,
			ConfigurationSettingsGroupExists,
			ExceptionADOperationFailedAlreadyExist,
			InvalidEndpointAddressErrorMessage,
			ErrorAuthServerTypeNotFound,
			IsMemberOfQueryFailed,
			ExceptionADTopologyErrorWhenLookingForSite,
			ErrorNotResettableProperty,
			ErrorMailTipHtmlCorrupt,
			ErrorInvalidOrganizationId,
			ErrorConversionFailed,
			ResourceMailbox_Violation,
			DomainNotFoundInGlsError,
			ExceptionSchemaMismatch,
			EapDuplicatedEmailAddressTemplate,
			NspiFailureException,
			EapMustHaveOnePrimaryAddressTemplate,
			SuitabilityDirectoryException,
			ExceptionADTopologyHasNoAvailableServersInForest,
			InvalidRecipientScope,
			UnableToResolveMapiPropertyNameException,
			InvalidOABMapiPropertyTypeException,
			ErrorInvalidMailboxProvisioningAttribute,
			ErrorMailboxProvisioningAttributeDoesNotMatchSchema,
			ErrorMultiplePrimaries,
			ErrorMinAdminVersionNull,
			MismatchedMapiPropertyTypesException,
			ServerHasNotBeenFound,
			ErrorDuplicateKeyInMailboxProvisioningAttributes,
			ErrorPrimarySmtpInvalid,
			ExceptionReferral,
			ExceptionRootDSE,
			ErrorNoSuitableGCInForest,
			InvalidBiggerResourceThreshold,
			InvalidTimeZoneId,
			CannotSerializePartitionHint,
			ErrorThisThresholdMustBeGreaterThanThatThreshold,
			ExceptionRemoveApprovedApplication,
			ErrorJoinApprovalRequiredWithoutManager,
			InvalidConsumerDialPlanSetting,
			RangeInformationFormatInvalid,
			ExceptionADTopologyHasNoAvailableServersInDomain,
			RuleMigration_Error,
			PropertiesMasteredOnPremise_Violation,
			BadSwapOperationCount,
			CannotDetermineDataSessionTypeForObject,
			ExceptionADOperationFailedNoSuchObject,
			ExceptionPropertyCannotBeSearchedOn,
			InvalidNtds,
			ExceptionADConfigurationObjectRequired,
			TenantOrgContainerNotFoundException,
			ExceptionADTopologyUnexpectedError,
			ErrorNotInServerWriteScope,
			DuplicatedAcceptedDomain,
			ErrorCannotSetPermanentAttributes,
			CannotBuildAuthenticationTypeFilterBadArgument,
			ErrorTargetPartitionHas2TenantsWithSameId,
			ExceededMaximumCollectionCount,
			ExceptionReferralWhenBoundToDomainController,
			AssignmentsWithConflictingScope,
			ExceptionReadingRootDSE,
			CannotGetForestInfo,
			InvalidCertificateName,
			CannotParse,
			TransportSettingsAmbiguousException,
			ExceptionADTopologyNoSuchForest,
			PropertyRequired,
			OUsNotSmallerOrEqual,
			TimeoutGlsError,
			ExtensionAlreadyUsedAsPilotNumber,
			ErrorCannotFindRidMasterForPartition,
			InvalidCharacterSet,
			ExceptionADUnavailable,
			ErrorSettingOverrideInvalidFlightName,
			TooManyCustomExtensions,
			ExceptionOwaCannotSetPropertyOnE12VirtualDirectory,
			ExArgumentException,
			CannotResolvePartitionGuidError,
			CannotGetDnAtDepth,
			InvalidHostname,
			ErrorTargetOrSourceForestPopulatedStatusNotStarted,
			BadSwapResourceIds,
			ExceptionADInvalidPassword,
			ConfigurationSettingsDuplicateRestriction,
			InvalidForestFqdnInGls,
			ErrorNonUniqueExchangeObjectId,
			ErrorNonUniqueMailboxGetMailboxLocation,
			ErrorMailTipDisplayableLengthExceeded,
			ErrorIsServerSuitableMissingComputerData,
			ConversionFailed,
			ExceptionOneTimeBindFailed,
			ExceptionDefaultScopeInvalidFormat,
			TenantNameTooLong,
			ExArgumentOutOfRangeException,
			ErrorInvalidISOTwoLetterOrCountryCode,
			ConfigurationSettingsDatabaseNotFound,
			ApiNotSupportedError,
			ErrorDatabaseCopiesInvalid,
			InvalidExtension,
			ExceptionDnLimitExceeded,
			ConfigurationSettingsRestrictionNotExpected,
			ErrorNotNullProperty,
			ErrorConversionFailedWithError,
			ExceptionCannotBindToDomain,
			ExceptionInvalidCredentialsFailedToGetIdentity,
			CannotCalculateProperty,
			ExceptionNotifyErrorGettingResults,
			ErrorSettingOverrideInvalidParameterSyntax,
			WrongDelegationTypeForPolicy,
			ExceptionCreateLdapConnection,
			ExceptionADTopologyErrorWhenLookingForTrustRelationships,
			ErrorMailboxCollectionNotSupportType,
			ExceptionInvalidVlvFilterOption,
			ServerSideADTopologyUnexpectedError,
			CannotBuildCapabilityFilterUnsupportedCapability,
			ExceptionInvalidVlvFilter,
			PerimeterSettingsAmbiguousException,
			ExceptionCannotRemoveDsServer,
			ExceptionCannotUseCredentials,
			ExceptionUnsupportedFilterForProperty,
			SuitabilityReachabilityError,
			ExceptionInvalidOperationOnReadOnlySession,
			ConfigurationSettingsRestrictionExpected,
			ExceptionADTopologyServiceNotStarted,
			ExEmptyStringArgumentException,
			ConstraintLocationValueReservedForSystemUse,
			FailedToReadAlternateServiceAccountConfigFromRegistry,
			ErrorGlobalWebDistributionAndVDirsSet,
			ServerComponentLocalRegistryError,
			TooManyKeyMappings,
			ScopeCannotBeExclusive,
			UnsupportedADSyntaxException,
			CannotFindOabException,
			ExceptionInvalidOperationOnInvalidSession,
			ErrorProperty1EqValue1WhileProperty2EqValue2,
			ErrorSettingOverrideInvalidComponentName,
			RecipientWriteScopeNotLessThanBecauseOfDelegationFlags,
			TooManyDataInLdapProperty,
			ErrorInvalidMailboxProvisioningAttributes,
			ExceptionUnsupportedOperatorForProperty,
			ErrorTargetPartitionSctMissing,
			TenantTransportSettingsNotFoundException,
			ErrorNonUniqueLegacyDN,
			ErrorAccountPartitionCantBeLocalAndSecondaryAtTheSameTime,
			ExceptionADTopologyTimeoutCall,
			ExceptionUnsupportedOperator,
			NoMatchingTenantInTargetPartition,
			RootMustBeEmpty,
			ExceptionValueNotPresent,
			ErrorMinAdminVersionOutOfSync,
			MasteredOnPremiseCapabilityUndefinedNotTenant,
			ExceptionUnsupportedTextFilterOption,
			ErrorServiceAccountThrottlingPolicy,
			ErrorDLAsBothAcceptedAndRejected,
			ErrorSettingOverrideSyntax,
			PermanentMservError,
			InvalidCapabilityOnMailboxPlan,
			SharedConfigurationVersionNotSupported,
			OwaAdOrphanFound,
			ExceptionRUSServerDown,
			DefaultRoutingGroupNotFoundException,
			ExceptionCopyChangesForIncompatibleTypes,
			ErrorSettingOverrideInvalidParameterName,
			RangePropertyResponseDoesNotContainRangeInformation,
			ExceptionSeverNotInPartition,
			CannotResolvePartitionFqdnFromAccountForestDnError,
			ErrorSettingOverrideUnexpected,
			OrgWideDelegatingWriteScopeMustBeTheSameAsRoleImplicitWriteScope,
			InvalidPhrase,
			ErrorRealmNotFound,
			ErrorLogonFailuresBeforePINReset,
			TenantNotFoundInMservError,
			ErrorIsServerInMaintenanceMode,
			ExceptionADOperationFailed,
			ConfigWriteScopeNotLessThanBecauseOfDelegationFlags,
			SwapShouldNotChangeValues,
			ConfigurationSettingsHistorySummary,
			ExceptionUnsupportedPropertyValue,
			ExceptionNetLogonOperation,
			ExceptionADTopologyNoSuchDomain,
			InvalidNumberOfCapabilitiesOnMailboxPlan,
			ExceptionReadOnlyBecauseTooNew,
			CannotGetSiteInfo,
			UnableToDeserializeXMLError,
			ConfigurationSettingsRestrictionMissingProperty,
			ConfigurationSettingsInvalidScopeFilter,
			OwaMetabasePathIsInvalid,
			InvalidDNStringFormat,
			ExceptionADTopologyNoServersForPartition,
			TenantAlreadyExistsInMserv,
			ExceptionUnsupportedDefaultValueFilter,
			InvalidResourceThresholdBetweenClassifications,
			ExceptionInvalidAddressFormat,
			ExceptionMostDerivedOnBase,
			ExceptionOwaCannotSetPropertyOnE12VirtualDirectoryToNull,
			ErrorCopySystemFolderPathNotEqualCopyLogFolderPath,
			ConfigurationSettingsUnsupportedVersion,
			ErrorInvalidOpathFilter,
			WrongAssigneeTypeForPolicyOrPartnerApplication,
			WrongScopeForCurrentPartition,
			ErrorInvalidLegacyRdnPrefix,
			InvalidIdFormat,
			LocalServerNotFound,
			ErrorResultsAreNonUnique,
			OrganizationMailboxNotFound,
			ExceptionUnsupportedPropertyValueType,
			ExceptionInvalidBitwiseComparison,
			CannotFindTemplateUser,
			ErrorNoSuitableGC,
			ExceptionServerUnavailable,
			CannotGetDomainFromDN,
			FederatedUser_Violation,
			BPOS_Feature_UsageLocation_Violation,
			InvalidMaxOutboundConnectionConfiguration,
			ExceptionADVlvSizeLimitExceeded,
			ExceptionWKGuidNeedsConfigSession,
			InvalidCookieServiceInstanceIdException,
			InternalDsnLanguageNotSupported,
			ErrorAdditionalInfo,
			ExceptionOverBudget,
			ExceptionInvalidAccountName,
			InvalidConfigScope,
			ConfigScopeNotLessThan,
			ExceptionInvalidScopeOperation,
			ErrorNonUniqueNetId,
			InvalidMailboxMoveFlags,
			ExInvalidTypeArgumentException,
			HostNameMatchesMultipleComputers,
			ErrorDefaultElcFolderTypeExists,
			ExceptionADTopologyDomainNameIsNotFqdn,
			ExceptionDefaultScopeMustContainDomainDN,
			ErrorInvalidFolderLinksAddition,
			ExceptionADTopologyHasNoServersInDomain,
			ExceptionWriteOnceProperty,
			ExceptionOrgScopeNotInUserScope,
			ExceptionInvalidVlvSeekReference,
			ExceptionADOperationFailedNotAMember,
			ErrorIsServerSuitableMissingOperatingSystemResponse,
			ErrorPartnerApplicationNotFound,
			ExceptionInvalidCredentials,
			MicrosoftExchangeRecipientDisplayNameError,
			CannotGetChild,
			FacebookEnabled_Error,
			ExceptionADTopologyErrorWhenLookingForForest,
			ErrorMultiValuedPropertyTooLarge,
			ErrorLinkedADObjectNotInFirstOrganization,
			ErrorExceededHosterResourceCountQuota,
			ConstraintViolationValueNotSupportedLCID,
			RusUnableToPerformValidation,
			ErrorSecondaryAccountPartitionCantBeUsedForProvisioning,
			ErrorInvalidLegacyCommonName,
			ExceptionExtendedRightNotFound,
			FailedToWriteAlternateServiceAccountConfigToRegistry,
			ExceptionUnsupportedMatchOptionsForProperty,
			ExceptionUnknownDirectoryAttribute,
			ErrorCannotAcquireAuthMetadata,
			ExtensionNotUnique,
			ExceptionADTopologyNoSuchSite,
			ErrorRecipientTypeIsNotValidForDeliveryRestrictionOrModeration,
			ErrorTwoOrMoreUniqueRecipientTypes,
			ExceptionADTopologyErrorWhenLookingForLocalDomainTrustRelationships,
			ExternalEmailAddressInvalid,
			ErrorGroupMemberDepartRestrictionApprovalRequired,
			TransientGlsError,
			FilterCannotBeEmpty,
			ExceptionADTopologyServiceCallError,
			InvalidDistributionGroupNamingPolicyFormat,
			ConfigurationSettingsNotFoundForGroup,
			CannotGetDomainInfo,
			ExceptionGuidSearchRootWithScope,
			ErrorCannotFindTenant,
			ErrorNoSuitableDC,
			SharingPolicyDuplicateDomain,
			ErrorNonUniqueCertificate,
			OrgWideDelegatingConfigScopeMustBeTheSameAsRoleImplicitWriteScope,
			ExceptionNativeErrorWhenLookingForServersInDomain,
			ErrorMoreThanOneSKUCapability,
			AlternateServiceAccountConfigurationDisplayFormat,
			DefaultDatabaseContainerNotFoundException,
			ExceptionAdamGetServerFromDomainDN,
			ErrorEdbFilePathInRoot,
			ConfigurationSettingsGroupSummary,
			InvalidSmartHost,
			ProviderFileNotFoundLoadException,
			ErrorRecipientAsBothAcceptedAndRejected,
			DuplicateTlsDomainCapabilitiesNotAllowed,
			CannotConstructCrossTenantObjectId,
			CrossRecordMismatch,
			ErrorStartDateAfterEndDate,
			ErrorNotInReadScope,
			ErrorSubnetMaskLessThanMinRange,
			InvalidTenantRecordInGls,
			ErrorNonUniqueLiveIdMemberName,
			ExceptionAccessDeniedFromRUS,
			CannotCompareScopeObjectWithOU,
			ErrorInvalidLegacyDN
		}
	}
}
