using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	internal static class DataStrings
	{
		static DataStrings()
		{
			DataStrings.stringIDs.Add(401351504U, "NotesProxyAddressPrefixDisplayName");
			DataStrings.stringIDs.Add(2802501868U, "InvalidCallerIdItemTypePersonalContact");
			DataStrings.stringIDs.Add(1831343041U, "ExceptionParseNotSupported");
			DataStrings.stringIDs.Add(2207492931U, "InvalidKeySelectionA");
			DataStrings.stringIDs.Add(2003039265U, "ConnectorTypePartner");
			DataStrings.stringIDs.Add(214463947U, "SclValue");
			DataStrings.stringIDs.Add(343989129U, "MessageStatusLocked");
			DataStrings.stringIDs.Add(2044618123U, "DaysOfWeek_None");
			DataStrings.stringIDs.Add(3268914636U, "InvalidAddressSpaceTypeNullOrEmpty");
			DataStrings.stringIDs.Add(1025341760U, "ProxyAddressShouldNotBeAllSpace");
			DataStrings.stringIDs.Add(3216786356U, "CalendarSharingFreeBusyReviewer");
			DataStrings.stringIDs.Add(2296262216U, "ProtocolLocalRpc");
			DataStrings.stringIDs.Add(3662889629U, "InsufficientSpace");
			DataStrings.stringIDs.Add(1357933441U, "ExchangeUsers");
			DataStrings.stringIDs.Add(4099951545U, "RoutingIncompatibleDeliveryDomain");
			DataStrings.stringIDs.Add(4134731995U, "EnterpriseTrialEdition");
			DataStrings.stringIDs.Add(315555574U, "InvalidNumberFormatString");
			DataStrings.stringIDs.Add(1448686489U, "Partners");
			DataStrings.stringIDs.Add(3119708545U, "CustomScheduleDaily12PM");
			DataStrings.stringIDs.Add(2191591450U, "CoexistenceTrialEdition");
			DataStrings.stringIDs.Add(3866345678U, "CustomExtensionInvalidArgument");
			DataStrings.stringIDs.Add(955250317U, "NonWorkingHours");
			DataStrings.stringIDs.Add(801480496U, "ClientAccessProtocolIMAP4");
			DataStrings.stringIDs.Add(319956112U, "ShadowMessagePreferencePreferRemote");
			DataStrings.stringIDs.Add(842516494U, "CustomScheduleDailyFromMidnightTo4AM");
			DataStrings.stringIDs.Add(2432881730U, "CustomScheduleDailyFrom8AMTo5PMAtWeekDays");
			DataStrings.stringIDs.Add(3910386293U, "EventLogText");
			DataStrings.stringIDs.Add(2532083337U, "ClientAccessAdfsAuthentication");
			DataStrings.stringIDs.Add(3646660508U, "ErrorADFormatError");
			DataStrings.stringIDs.Add(4100370227U, "MeetingFullDetailsWithAttendees");
			DataStrings.stringIDs.Add(2139945176U, "CustomScheduleDaily3AM");
			DataStrings.stringIDs.Add(1571847379U, "ExceptionCalculatedDependsOnCalculated");
			DataStrings.stringIDs.Add(4153254550U, "RoutingNoRouteToMdb");
			DataStrings.stringIDs.Add(3267940029U, "SmtpReceiveCapabilitiesAcceptProxyProtocol");
			DataStrings.stringIDs.Add(554923823U, "DeferReasonADTransientFailureDuringContentConversion");
			DataStrings.stringIDs.Add(3857745828U, "HeaderPromotionModeMayCreate");
			DataStrings.stringIDs.Add(2392738789U, "ConnectorSourceHybridWizard");
			DataStrings.stringIDs.Add(2680336401U, "DeferReasonAgent");
			DataStrings.stringIDs.Add(86136247U, "InvalidCustomMenuKeyMappingA");
			DataStrings.stringIDs.Add(352888273U, "InvalidResourcePropertySyntax");
			DataStrings.stringIDs.Add(651446229U, "SmtpReceiveCapabilitiesAcceptCloudServicesMail");
			DataStrings.stringIDs.Add(830368596U, "CcMailProxyAddressPrefixDisplayName");
			DataStrings.stringIDs.Add(1420072811U, "CustomScheduleEveryHour");
			DataStrings.stringIDs.Add(255146954U, "DeferReasonConcurrencyLimitInStoreDriver");
			DataStrings.stringIDs.Add(1322244886U, "GroupByMonth");
			DataStrings.stringIDs.Add(906828259U, "ContactsSharing");
			DataStrings.stringIDs.Add(2020388072U, "ExceptionUnsupportedNetworkProtocol");
			DataStrings.stringIDs.Add(452471808U, "SmtpReceiveCapabilitiesAcceptXAttrProtocol");
			DataStrings.stringIDs.Add(2998256021U, "ExceptionInvlidCharInProtocolName");
			DataStrings.stringIDs.Add(1284528402U, "TransientFailure");
			DataStrings.stringIDs.Add(2446644924U, "InvalidInputErrorMsg");
			DataStrings.stringIDs.Add(242132428U, "ErrorFileShareWitnessServerNameMustNotBeEmpty");
			DataStrings.stringIDs.Add(2360034689U, "HostnameTooLong");
			DataStrings.stringIDs.Add(118693033U, "InvalidKeyMappingVoiceMail");
			DataStrings.stringIDs.Add(2600270045U, "KindKeywordEmail");
			DataStrings.stringIDs.Add(3423705850U, "ConstraintViolationStringLengthIsEmpty");
			DataStrings.stringIDs.Add(1737591621U, "AnonymousUsers");
			DataStrings.stringIDs.Add(2996964666U, "ErrorQuotionMarkNotSupportedInKql");
			DataStrings.stringIDs.Add(2637923947U, "LatAmSpanish");
			DataStrings.stringIDs.Add(1831009676U, "SearchRecipientsCc");
			DataStrings.stringIDs.Add(1181135370U, "InvalidOperationCurrentProperty");
			DataStrings.stringIDs.Add(665138238U, "ErrorPoliciesUpgradeCustomFailures");
			DataStrings.stringIDs.Add(643679508U, "CustomScheduleEveryHalfHour");
			DataStrings.stringIDs.Add(4198186357U, "EmptyExchangeObjectVersion");
			DataStrings.stringIDs.Add(2760883904U, "CustomGreetingFilePatternDescription");
			DataStrings.stringIDs.Add(1485002803U, "Partitioned");
			DataStrings.stringIDs.Add(220124761U, "ClientAccessProtocolOA");
			DataStrings.stringIDs.Add(3095705478U, "PublicFolderPermissionRolePublishingAuthor");
			DataStrings.stringIDs.Add(2578042802U, "InvalidIPAddressInSmartHost");
			DataStrings.stringIDs.Add(2736312437U, "DeliveryTypeSmtpRelayToMailboxDeliveryGroup");
			DataStrings.stringIDs.Add(2889762178U, "UnknownEdition");
			DataStrings.stringIDs.Add(3765811860U, "InvalidFormatExchangeObjectVersion");
			DataStrings.stringIDs.Add(3427924322U, "ToInternet");
			DataStrings.stringIDs.Add(1226277855U, "KindKeywordTasks");
			DataStrings.stringIDs.Add(736960779U, "MarkedAsRetryDeliveryIfRejected");
			DataStrings.stringIDs.Add(4060482376U, "ConnectorTypeOnPremises");
			DataStrings.stringIDs.Add(2163423328U, "HeaderValue");
			DataStrings.stringIDs.Add(2117971051U, "SmtpReceiveCapabilitiesAcceptProxyFromProtocol");
			DataStrings.stringIDs.Add(1153159321U, "PoisonQueueNextHopDomain");
			DataStrings.stringIDs.Add(936162170U, "GroupWiseProxyAddressPrefixDisplayName");
			DataStrings.stringIDs.Add(3861972062U, "MsMailProxyAddressPrefixDisplayName");
			DataStrings.stringIDs.Add(2924600836U, "Exchange2007");
			DataStrings.stringIDs.Add(2909775076U, "HeaderName");
			DataStrings.stringIDs.Add(1178729042U, "RejectText");
			DataStrings.stringIDs.Add(2493635772U, "NameValidationSpaceAllowedPatternDescription");
			DataStrings.stringIDs.Add(2255949938U, "ConstraintViolationStringLengthCauseOutOfMemory");
			DataStrings.stringIDs.Add(3639763634U, "InvalidFormat");
			DataStrings.stringIDs.Add(729961244U, "CustomPeriod");
			DataStrings.stringIDs.Add(95015764U, "PublicFolderPermissionRolePublishingEditor");
			DataStrings.stringIDs.Add(1893156877U, "InvalidKeyMappingKey");
			DataStrings.stringIDs.Add(224331957U, "StringConversionDelegateNotSet");
			DataStrings.stringIDs.Add(2603796430U, "NumberingPlanPatternDescription");
			DataStrings.stringIDs.Add(716522036U, "MeetingFullDetails");
			DataStrings.stringIDs.Add(3427843085U, "ProtocolLoggingLevelNone");
			DataStrings.stringIDs.Add(2353628225U, "ErrorInputFormatError");
			DataStrings.stringIDs.Add(3390434404U, "Ascending");
			DataStrings.stringIDs.Add(3345411900U, "SmtpReceiveCapabilitiesAcceptXOriginalFromProtocol");
			DataStrings.stringIDs.Add(599002008U, "Exchange2003");
			DataStrings.stringIDs.Add(1604545240U, "WorkingHours");
			DataStrings.stringIDs.Add(2838855213U, "DeferReasonTransientAttributionFailure");
			DataStrings.stringIDs.Add(2536591407U, "FromEnterprise");
			DataStrings.stringIDs.Add(1206366831U, "TlsAuthLevelCertificateValidation");
			DataStrings.stringIDs.Add(4094875965U, "Friday");
			DataStrings.stringIDs.Add(4009888891U, "MeetingLimitedDetails");
			DataStrings.stringIDs.Add(61359385U, "KeyMappingInvalidArgument");
			DataStrings.stringIDs.Add(1186448673U, "LegacyDNPatternDescription");
			DataStrings.stringIDs.Add(2245804386U, "SmtpReceiveCapabilitiesAcceptXSysProbeProtocol");
			DataStrings.stringIDs.Add(2677919833U, "SentTime");
			DataStrings.stringIDs.Add(2511844601U, "SearchSender");
			DataStrings.stringIDs.Add(2397470300U, "ErrorCannotAddNullValue");
			DataStrings.stringIDs.Add(3811769309U, "ScheduleModeScheduledTimes");
			DataStrings.stringIDs.Add(391606028U, "ProtocolAppleTalk");
			DataStrings.stringIDs.Add(3390939332U, "DigitStringOrEmptyPatternDescription");
			DataStrings.stringIDs.Add(26915469U, "EnterpriseEdition");
			DataStrings.stringIDs.Add(2726993793U, "ConnectorSourceDefault");
			DataStrings.stringIDs.Add(3372601165U, "ExceptionVersionlessObject");
			DataStrings.stringIDs.Add(2815103562U, "AliasPatternDescription");
			DataStrings.stringIDs.Add(1026314696U, "ReceivedTime");
			DataStrings.stringIDs.Add(3479640092U, "ParameterNameEmptyException");
			DataStrings.stringIDs.Add(2043548597U, "RecipientStatusLocked");
			DataStrings.stringIDs.Add(1248401958U, "SmtpReceiveCapabilitiesAcceptProxyToProtocol");
			DataStrings.stringIDs.Add(1109398861U, "PermissionGroupsNone");
			DataStrings.stringIDs.Add(2137442040U, "ArgumentMustBeAscii");
			DataStrings.stringIDs.Add(1284326164U, "ExceptionNetworkProtocolDuplicate");
			DataStrings.stringIDs.Add(3511836031U, "RecipientStatusRetry");
			DataStrings.stringIDs.Add(1002496550U, "GroupExpansionRecipients");
			DataStrings.stringIDs.Add(1061876008U, "LegacyDNProxyAddressPrefixDisplayName");
			DataStrings.stringIDs.Add(781217110U, "HeaderPromotionModeMustCreate");
			DataStrings.stringIDs.Add(2082847001U, "ProtocolLoggingLevelVerbose");
			DataStrings.stringIDs.Add(1176489840U, "HeaderPromotionModeNoCreate");
			DataStrings.stringIDs.Add(462794175U, "EncryptionTypeSSL");
			DataStrings.stringIDs.Add(3512036720U, "KindKeywordDocs");
			DataStrings.stringIDs.Add(2589626668U, "Unreachable");
			DataStrings.stringIDs.Add(1884450703U, "CustomScheduleSundayAtMidnight");
			DataStrings.stringIDs.Add(4294726685U, "ExceptionUnknownUnit");
			DataStrings.stringIDs.Add(3410257022U, "SendNone");
			DataStrings.stringIDs.Add(1100730082U, "SubjectPrefix");
			DataStrings.stringIDs.Add(3791230818U, "DeliveryTypeSmtpRelayToServers");
			DataStrings.stringIDs.Add(1667689070U, "InvalidKeyMappingFindMeFirstNumberDuration");
			DataStrings.stringIDs.Add(2482088490U, "AirSyncProxyAddressPrefixDisplayName");
			DataStrings.stringIDs.Add(1474747046U, "CoexistenceEdition");
			DataStrings.stringIDs.Add(2175102537U, "UnsearchableItemsAdded");
			DataStrings.stringIDs.Add(2846565657U, "InvalidKeyMappingFormat");
			DataStrings.stringIDs.Add(4125301959U, "ClientAccessProtocolPOP3");
			DataStrings.stringIDs.Add(4143129766U, "Word");
			DataStrings.stringIDs.Add(3614810764U, "AddressSpaceIsTooLong");
			DataStrings.stringIDs.Add(3699252422U, "AddressFamilyMismatch");
			DataStrings.stringIDs.Add(294615990U, "KindKeywordFaxes");
			DataStrings.stringIDs.Add(2440320060U, "ExceptionEmptyProxyAddress");
			DataStrings.stringIDs.Add(882442171U, "ExceptionObjectInvalid");
			DataStrings.stringIDs.Add(1177570172U, "ExceptionReadOnlyMultiValuedProperty");
			DataStrings.stringIDs.Add(2820941203U, "Tuesday");
			DataStrings.stringIDs.Add(2714058314U, "DeferReasonADTransientFailureDuringResolve");
			DataStrings.stringIDs.Add(3770991413U, "DeliveryTypeNonSmtpGatewayDelivery");
			DataStrings.stringIDs.Add(980447290U, "UseExchangeDSNs");
			DataStrings.stringIDs.Add(1316672322U, "KindKeywordContacts");
			DataStrings.stringIDs.Add(2112156755U, "FromLocal");
			DataStrings.stringIDs.Add(3991588639U, "ErrorPoliciesDowngradeDnsFailures");
			DataStrings.stringIDs.Add(466678310U, "TlsAuthLevelEncryptionOnly");
			DataStrings.stringIDs.Add(1073167130U, "Sunday");
			DataStrings.stringIDs.Add(1629702106U, "CustomScheduleDailyFrom9AMTo6PMAtWeekDays");
			DataStrings.stringIDs.Add(1777112844U, "Descending");
			DataStrings.stringIDs.Add(2350082752U, "DeliveryTypeUnreachable");
			DataStrings.stringIDs.Add(4067663994U, "KindKeywordPosts");
			DataStrings.stringIDs.Add(4234859176U, "ErrorServerGuidAndNameBothEmpty");
			DataStrings.stringIDs.Add(64170653U, "ExLengthOfVersionByteArrayError");
			DataStrings.stringIDs.Add(3444147793U, "SearchRecipientsTo");
			DataStrings.stringIDs.Add(918988081U, "DeliveryTypeMapiDelivery");
			DataStrings.stringIDs.Add(2906226218U, "SearchRecipientsBcc");
			DataStrings.stringIDs.Add(1947725858U, "EmptyNameInHostname");
			DataStrings.stringIDs.Add(1689084926U, "DisclaimerText");
			DataStrings.stringIDs.Add(1950405677U, "InvalidSmtpDomainWildcard");
			DataStrings.stringIDs.Add(3328112862U, "CustomScheduleDailyFrom2AMTo6AM");
			DataStrings.stringIDs.Add(3884278210U, "InvalidTimeOfDayFormat");
			DataStrings.stringIDs.Add(1054423051U, "Failed");
			DataStrings.stringIDs.Add(2176757305U, "CustomScheduleDailyFrom11PMTo6AM");
			DataStrings.stringIDs.Add(3124035205U, "ColonPrefix");
			DataStrings.stringIDs.Add(3544743625U, "ToGroupExpansionRecipients");
			DataStrings.stringIDs.Add(923847139U, "InvalidCallerIdItemFormat");
			DataStrings.stringIDs.Add(4060377141U, "PublicFolderPermissionRoleOwner");
			DataStrings.stringIDs.Add(2634610906U, "PermissionGroupsCustom");
			DataStrings.stringIDs.Add(3561826809U, "KindKeywordIm");
			DataStrings.stringIDs.Add(1977971622U, "GroupByTotal");
			DataStrings.stringIDs.Add(3781744254U, "BccGroupExpansionRecipients");
			DataStrings.stringIDs.Add(833225182U, "ExceptionInvlidNetworkAddressFormat");
			DataStrings.stringIDs.Add(2456836287U, "KindKeywordJournals");
			DataStrings.stringIDs.Add(1525449578U, "EmptyExchangeBuild");
			DataStrings.stringIDs.Add(2321790947U, "StandardEdition");
			DataStrings.stringIDs.Add(29693289U, "FormatExchangeBuildWrong");
			DataStrings.stringIDs.Add(79160252U, "ClientAccessProtocolPSWS");
			DataStrings.stringIDs.Add(3706983644U, "DeliveryTypeSmtpRelayToConnectorSourceServers");
			DataStrings.stringIDs.Add(3519073218U, "CustomScheduleSaturdayAtMidnight");
			DataStrings.stringIDs.Add(71385097U, "ExceptionNoValue");
			DataStrings.stringIDs.Add(2608795131U, "MailRecipientTypeDistributionGroup");
			DataStrings.stringIDs.Add(1561011830U, "ExceptionInvlidProtocolAddressFormat");
			DataStrings.stringIDs.Add(2364433662U, "CustomScheduleFridayAtMidnight");
			DataStrings.stringIDs.Add(255999871U, "DeliveryTypeShadowRedundancy");
			DataStrings.stringIDs.Add(3951803838U, "DeferReasonLoopDetected");
			DataStrings.stringIDs.Add(3183375374U, "SearchRecipients");
			DataStrings.stringIDs.Add(2862582797U, "CalendarSharingFreeBusySimple");
			DataStrings.stringIDs.Add(3422989433U, "PublicFolderPermissionRoleReviewer");
			DataStrings.stringIDs.Add(2267899661U, "QueueStatusActive");
			DataStrings.stringIDs.Add(2387319017U, "TlsAuthLevelCertificateExpiryCheck");
			DataStrings.stringIDs.Add(3030346869U, "InvalidAddressSpaceAddress");
			DataStrings.stringIDs.Add(1501056036U, "FromPartner");
			DataStrings.stringIDs.Add(3502523774U, "AllDays");
			DataStrings.stringIDs.Add(2099540954U, "DeferReasonTargetSiteInboundMailDisabled");
			DataStrings.stringIDs.Add(2760297639U, "ExceptionFormatNotSupported");
			DataStrings.stringIDs.Add(2279665409U, "DeliveryTypeDeliveryAgent");
			DataStrings.stringIDs.Add(1531043460U, "EstimatedItems");
			DataStrings.stringIDs.Add(1261683271U, "CmdletParameterEmptyValidationException");
			DataStrings.stringIDs.Add(66601190U, "MessageStatusPendingRemove");
			DataStrings.stringIDs.Add(3129286587U, "QueueStatusConnecting");
			DataStrings.stringIDs.Add(1108839058U, "DeliveryTypeSmtpRelayToRemoteAdSite");
			DataStrings.stringIDs.Add(3478111469U, "Saturday");
			DataStrings.stringIDs.Add(4135023588U, "ToEnterprise");
			DataStrings.stringIDs.Add(339456021U, "InvalidHolidayScheduleFormat");
			DataStrings.stringIDs.Add(1915155164U, "InvalidTimeOfDayFormatWorkingHours");
			DataStrings.stringIDs.Add(3119708320U, "CustomScheduleDaily11PM");
			DataStrings.stringIDs.Add(1761145356U, "QueueStatusSuspended");
			DataStrings.stringIDs.Add(999227985U, "SubmissionQueueNextHopDomain");
			DataStrings.stringIDs.Add(3153135486U, "ErrorNotSupportedForChangesOnlyCopy");
			DataStrings.stringIDs.Add(4098793892U, "CcGroupExpansionRecipients");
			DataStrings.stringIDs.Add(3039999898U, "ProxyAddressPrefixTooLong");
			DataStrings.stringIDs.Add(1660006804U, "Pattern");
			DataStrings.stringIDs.Add(1029030096U, "DeliveryTypeSmartHostConnectorDelivery");
			DataStrings.stringIDs.Add(2368086636U, "KindKeywordVoiceMail");
			DataStrings.stringIDs.Add(570747971U, "RoutingNoRouteToMta");
			DataStrings.stringIDs.Add(2517721976U, "ClientAccessProtocolEWS");
			DataStrings.stringIDs.Add(810977739U, "MailRecipientTypeExternal");
			DataStrings.stringIDs.Add(749395574U, "DeliveryTypeSmtpRelayWithinAdSite");
			DataStrings.stringIDs.Add(1800544803U, "InvalidDialGroupEntryCsvFormat");
			DataStrings.stringIDs.Add(607066203U, "KindKeywordMeetings");
			DataStrings.stringIDs.Add(2682624686U, "CustomScheduleEveryFourHours");
			DataStrings.stringIDs.Add(3134372274U, "MailRecipientTypeUnknown");
			DataStrings.stringIDs.Add(894196715U, "AmbiguousRecipient");
			DataStrings.stringIDs.Add(182065869U, "SmtpReceiveCapabilitiesAllowConsumerMail");
			DataStrings.stringIDs.Add(1601563988U, "InvalidFlagValue");
			DataStrings.stringIDs.Add(1435915854U, "TlsAuthLevelDomainValidation");
			DataStrings.stringIDs.Add(3942979931U, "ClientAccessBasicAuthentication");
			DataStrings.stringIDs.Add(3828585651U, "InvalidTimeOfDayFormatCustomWorkingHours");
			DataStrings.stringIDs.Add(1720677880U, "AttachmentContent");
			DataStrings.stringIDs.Add(1152451692U, "InvalidKeyMappingTransferToGalContact");
			DataStrings.stringIDs.Add(2941311901U, "ProtocolSpx");
			DataStrings.stringIDs.Add(4237384485U, "ErrorPoliciesDowngradeCustomFailures");
			DataStrings.stringIDs.Add(4240385111U, "ErrorCostOutOfRange");
			DataStrings.stringIDs.Add(1140124266U, "ExceptionNegativeUnit");
			DataStrings.stringIDs.Add(982561113U, "DeliveryTypeSmtpDeliveryToMailbox");
			DataStrings.stringIDs.Add(2755629282U, "PropertyIsMandatory");
			DataStrings.stringIDs.Add(2085180697U, "RoutingNonBHExpansionServer");
			DataStrings.stringIDs.Add(838517570U, "ExceptionReadOnlyPropertyBag");
			DataStrings.stringIDs.Add(1594003981U, "UnreachableQueueNextHopDomain");
			DataStrings.stringIDs.Add(682709795U, "InvalidSmtpDomain");
			DataStrings.stringIDs.Add(416319699U, "InvalidDialledNumberFormatC");
			DataStrings.stringIDs.Add(3338773880U, "DeliveryTypeDnsConnectorDelivery");
			DataStrings.stringIDs.Add(2544690258U, "DigitStringPatternDescription");
			DataStrings.stringIDs.Add(3001525213U, "CustomScheduleDailyFrom9AMTo5PMAtWeekDays");
			DataStrings.stringIDs.Add(4197662657U, "InvalidKeyMappingContext");
			DataStrings.stringIDs.Add(3826988642U, "CustomScheduleDaily1AM");
			DataStrings.stringIDs.Add(1512272061U, "FromInternet");
			DataStrings.stringIDs.Add(2854645212U, "TotalCopiedItems");
			DataStrings.stringIDs.Add(4083806514U, "ClientAccessProtocolRPS");
			DataStrings.stringIDs.Add(3406691936U, "ClientAccessNonBasicAuthentication");
			DataStrings.stringIDs.Add(3877102044U, "RoutingNoMdb");
			DataStrings.stringIDs.Add(443721013U, "PublicFolderPermissionRoleEditor");
			DataStrings.stringIDs.Add(3090942252U, "MailRecipientTypeMailbox");
			DataStrings.stringIDs.Add(460086660U, "CopyErrors");
			DataStrings.stringIDs.Add(2182454218U, "SnapinNameTooShort");
			DataStrings.stringIDs.Add(4028819673U, "TextBody");
			DataStrings.stringIDs.Add(1277617174U, "DeliveryTypeSmtpRelayToTiRg");
			DataStrings.stringIDs.Add(2058499689U, "ConstraintViolationNoLeadingOrTrailingWhitespace");
			DataStrings.stringIDs.Add(616309426U, "CalendarSharingFreeBusyDetail");
			DataStrings.stringIDs.Add(4177834631U, "ItemClass");
			DataStrings.stringIDs.Add(38623232U, "InvalidNotationFormat");
			DataStrings.stringIDs.Add(2067382811U, "ClientAccessProtocolOAB");
			DataStrings.stringIDs.Add(2956076415U, "InvalidCallerIdItemTypePhoneNumber");
			DataStrings.stringIDs.Add(3803801081U, "ExceptionDurationOverflow");
			DataStrings.stringIDs.Add(4064690660U, "ExchangeLegacyServers");
			DataStrings.stringIDs.Add(1367191786U, "Down");
			DataStrings.stringIDs.Add(416319697U, "InvalidDialledNumberFormatA");
			DataStrings.stringIDs.Add(3211494971U, "Misconfigured");
			DataStrings.stringIDs.Add(974174060U, "ProtocolTcpIP");
			DataStrings.stringIDs.Add(3287485561U, "RoleEntryStringMustBeCommaSeparated");
			DataStrings.stringIDs.Add(3779112048U, "SmtpReceiveCapabilitiesAcceptOorgProtocol");
			DataStrings.stringIDs.Add(4100738364U, "WeekendDays");
			DataStrings.stringIDs.Add(2509290958U, "ProtocolNetBios");
			DataStrings.stringIDs.Add(1022093144U, "CustomScheduleDailyFrom8AMTo12PMAnd1PMTo5PMAtWeekDays");
			DataStrings.stringIDs.Add(1817337337U, "ConfigurationSettingsAppSettingsError");
			DataStrings.stringIDs.Add(1261519073U, "InvalidKeyMappingTransferToNumber");
			DataStrings.stringIDs.Add(452901710U, "CustomScheduleDaily5AM");
			DataStrings.stringIDs.Add(871198498U, "KindKeywordRssFeeds");
			DataStrings.stringIDs.Add(1937548848U, "MessageStatusSuspended");
			DataStrings.stringIDs.Add(462978851U, "ShadowMessagePreferenceLocalOnly");
			DataStrings.stringIDs.Add(2047193656U, "Unavailable");
			DataStrings.stringIDs.Add(1434991878U, "RejectStatusCode");
			DataStrings.stringIDs.Add(1760294240U, "Thursday");
			DataStrings.stringIDs.Add(1039830289U, "StartingAddressAndMaskAddressFamilyMismatch");
			DataStrings.stringIDs.Add(2802337392U, "InvalidCallerIdItemTypeDefaultContactsFolder");
			DataStrings.stringIDs.Add(3137483865U, "ErrorPoliciesDefault");
			DataStrings.stringIDs.Add(268882683U, "PublicFolderPermissionRoleContributor");
			DataStrings.stringIDs.Add(2587222631U, "Weekdays");
			DataStrings.stringIDs.Add(3048094658U, "SmtpReceiveCapabilitiesAllowSubmit");
			DataStrings.stringIDs.Add(3673950617U, "PropertyNotEmptyOrNull");
			DataStrings.stringIDs.Add(2788726202U, "MAPIBlockOutlookVersionsPatternDescription");
			DataStrings.stringIDs.Add(4263235384U, "BucketVersionPatternDescription");
			DataStrings.stringIDs.Add(1602600165U, "MessageStatusNone");
			DataStrings.stringIDs.Add(2795839773U, "PublicFolderPermissionRoleAuthor");
			DataStrings.stringIDs.Add(2292506358U, "DeliveryTypeUndefined");
			DataStrings.stringIDs.Add(1405683073U, "SmtpReceiveCapabilitiesAcceptCrossForestMail");
			DataStrings.stringIDs.Add(2397369159U, "ConnectorSourceMigrated");
			DataStrings.stringIDs.Add(2934715437U, "RoutingNoMatchingConnector");
			DataStrings.stringIDs.Add(548875607U, "QueueStatusNone");
			DataStrings.stringIDs.Add(2067383266U, "ClientAccessProtocolEAC");
			DataStrings.stringIDs.Add(2929555192U, "EncryptionTypeTLS");
			DataStrings.stringIDs.Add(3496963749U, "RoleEntryNameTooShort");
			DataStrings.stringIDs.Add(3076383364U, "DuplicatesRemoved");
			DataStrings.stringIDs.Add(681220170U, "MessageStatusPendingSuspend");
			DataStrings.stringIDs.Add(3785867729U, "SmtpReceiveCapabilitiesAcceptOrgHeaders");
			DataStrings.stringIDs.Add(2114081924U, "CustomScheduleDailyAtMidnight");
			DataStrings.stringIDs.Add(412393748U, "RecipientStatusComplete");
			DataStrings.stringIDs.Add(2721491554U, "InvalidCallerIdItemTypeGALContactr");
			DataStrings.stringIDs.Add(1093243629U, "ExchangeServers");
			DataStrings.stringIDs.Add(1206036754U, "ScheduleModeNever");
			DataStrings.stringIDs.Add(1511350752U, "CustomScheduleDailyFrom11PMTo3AM");
			DataStrings.stringIDs.Add(416319702U, "InvalidDialledNumberFormatD");
			DataStrings.stringIDs.Add(1826938428U, "CustomScheduleDailyFrom1AMTo5AM");
			DataStrings.stringIDs.Add(344942096U, "ProxyAddressPrefixShouldNotBeAllSpace");
			DataStrings.stringIDs.Add(835983261U, "CustomScheduleDaily2AM");
			DataStrings.stringIDs.Add(1899327622U, "InvalidKeyMappingFindMeSecondNumber");
			DataStrings.stringIDs.Add(202819370U, "MessageStatusReady");
			DataStrings.stringIDs.Add(1069174646U, "GroupByDay");
			DataStrings.stringIDs.Add(3028676818U, "DeliveryTypeHeartbeat");
			DataStrings.stringIDs.Add(2470774060U, "FileExtensionOrSplatPatternDescription");
			DataStrings.stringIDs.Add(3713180243U, "MessageStatusActive");
			DataStrings.stringIDs.Add(3364213626U, "Monday");
			DataStrings.stringIDs.Add(2973542840U, "DeferReasonTransientAcceptedDomainsLoadFailure");
			DataStrings.stringIDs.Add(3065954297U, "ExceptionEventSourceNull");
			DataStrings.stringIDs.Add(1205942262U, "PublicFolderPermissionRoleNonEditingAuthor");
			DataStrings.stringIDs.Add(700746266U, "CustomScheduleEveryTwoHours");
			DataStrings.stringIDs.Add(2988328408U, "InvalidCallerIdItemTypePersonaContact");
			DataStrings.stringIDs.Add(1169266063U, "DeferReasonConfigUpdate");
			DataStrings.stringIDs.Add(654440350U, "ConstraintViolationValueIsNullOrEmpty");
			DataStrings.stringIDs.Add(1069535743U, "SubjectProperty");
			DataStrings.stringIDs.Add(1074863583U, "InvalidKeySelection_Zero");
			DataStrings.stringIDs.Add(2743519022U, "QueueStatusReady");
			DataStrings.stringIDs.Add(2677933048U, "ProtocolNamedPipes");
			DataStrings.stringIDs.Add(1839463316U, "ProtocolVnsSpp");
			DataStrings.stringIDs.Add(2247071192U, "KindKeywordNotes");
			DataStrings.stringIDs.Add(1707192398U, "ErrorCannotConvert");
			DataStrings.stringIDs.Add(416319700U, "InvalidDialledNumberFormatB");
			DataStrings.stringIDs.Add(1593650977U, "InvalidDialGroupEntryFormat");
			DataStrings.stringIDs.Add(3543498202U, "EapMustHaveOneEnabledPrimarySmtpAddressTemplate");
			DataStrings.stringIDs.Add(2682420737U, "FileExtensionPatternDescription");
			DataStrings.stringIDs.Add(2846264340U, "Unknown");
			DataStrings.stringIDs.Add(4060777377U, "QueueStatusRetry");
			DataStrings.stringIDs.Add(3452652986U, "Wednesday");
			DataStrings.stringIDs.Add(3417603459U, "InvalidKeyMappingFindMe");
			DataStrings.stringIDs.Add(417785204U, "DeferReasonReroutedByStoreDriver");
			DataStrings.stringIDs.Add(1326579539U, "ScheduleModeAlways");
			DataStrings.stringIDs.Add(2583398905U, "FileIsEmpty");
			DataStrings.stringIDs.Add(621231604U, "ExceptionSerializationDataAbsent");
			DataStrings.stringIDs.Add(3488668113U, "SmtpReceiveCapabilitiesAcceptOorgHeader");
			DataStrings.stringIDs.Add(881737792U, "DsnText");
			DataStrings.stringIDs.Add(269836784U, "CustomScheduleDailyFrom9AMTo12PMAnd1PMTo6PMAtWeekDays");
			DataStrings.stringIDs.Add(1301289463U, "StorageTransientFailureDuringContentConversion");
			DataStrings.stringIDs.Add(1500391415U, "MessageStatusRetry");
			DataStrings.stringIDs.Add(387961094U, "CustomProxyAddressPrefixDisplayName");
			DataStrings.stringIDs.Add(1088965676U, "CLIDPatternDescription");
			DataStrings.stringIDs.Add(2067383282U, "ClientAccessProtocolEAS");
			DataStrings.stringIDs.Add(304927915U, "ConstraintViolationInvalidWindowsLiveIDLocalPart");
			DataStrings.stringIDs.Add(1914858911U, "ExceptionParseInternalMessageId");
			DataStrings.stringIDs.Add(2476902678U, "RecipientStatusReady");
			DataStrings.stringIDs.Add(2221667633U, "ReceiveNone");
			DataStrings.stringIDs.Add(560731754U, "UserPrincipalNamePatternDescription");
			DataStrings.stringIDs.Add(2579867249U, "ToPartner");
			DataStrings.stringIDs.Add(824224038U, "DeliveryTypeSmtpRelayToDag");
			DataStrings.stringIDs.Add(1676404342U, "DeliveryTypeSmtpRelayWithinAdSiteToEdge");
			DataStrings.stringIDs.Add(1472458119U, "DeferReasonRecipientThreadLimitExceeded");
			DataStrings.stringIDs.Add(1543969273U, "Up");
			DataStrings.stringIDs.Add(4133244061U, "PreserveDSNBody");
			DataStrings.stringIDs.Add(2194722870U, "ElcScheduleInsufficientGap");
			DataStrings.stringIDs.Add(3501929169U, "ExceptionTypeNotEnhancedTimeSpanOrTimeSpan");
			DataStrings.stringIDs.Add(986397318U, "Recipients");
			DataStrings.stringIDs.Add(3443907091U, "CustomScheduleDaily4AM");
			DataStrings.stringIDs.Add(4277209464U, "ExceptionDefaultTypeMismatch");
			DataStrings.stringIDs.Add(2517721504U, "ClientAccessProtocolOWA");
			DataStrings.stringIDs.Add(2947517465U, "RecipientStatusNone");
			DataStrings.stringIDs.Add(553174585U, "StandardTrialEdition");
			DataStrings.stringIDs.Add(1980952396U, "ShadowMessagePreferenceRemoteOnly");
			DataStrings.stringIDs.Add(1065872813U, "DoNotConvert");
			DataStrings.stringIDs.Add(3119708351U, "CustomScheduleDaily10PM");
		}

		public static LocalizedString NotesProxyAddressPrefixDisplayName
		{
			get
			{
				return new LocalizedString("NotesProxyAddressPrefixDisplayName", "Ex8F6208", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidCallerIdItemTypePersonalContact
		{
			get
			{
				return new LocalizedString("InvalidCallerIdItemTypePersonalContact", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddressSpaceCostOutOfRange(int minCost, int maxCost)
		{
			return new LocalizedString("AddressSpaceCostOutOfRange", "ExFB6E8C", false, true, DataStrings.ResourceManager, new object[]
			{
				minCost,
				maxCost
			});
		}

		public static LocalizedString ExceptionWriteOnceProperty(string propertyName)
		{
			return new LocalizedString("ExceptionWriteOnceProperty", "Ex49D657", false, true, DataStrings.ResourceManager, new object[]
			{
				propertyName
			});
		}

		public static LocalizedString ExceptionParseNotSupported
		{
			get
			{
				return new LocalizedString("ExceptionParseNotSupported", "Ex2465C4", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidKeySelectionA
		{
			get
			{
				return new LocalizedString("InvalidKeySelectionA", "ExAD41C5", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectorTypePartner
		{
			get
			{
				return new LocalizedString("ConnectorTypePartner", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SharingPolicyDomainInvalidAction(string value)
		{
			return new LocalizedString("SharingPolicyDomainInvalidAction", "Ex375C51", false, true, DataStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString SclValue
		{
			get
			{
				return new LocalizedString("SclValue", "ExAB6A24", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidCIDRLengthIPv6(short cidrlength)
		{
			return new LocalizedString("InvalidCIDRLengthIPv6", "Ex119BC6", false, true, DataStrings.ResourceManager, new object[]
			{
				cidrlength
			});
		}

		public static LocalizedString MessageStatusLocked
		{
			get
			{
				return new LocalizedString("MessageStatusLocked", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorToStringNotImplemented(string sourceType)
		{
			return new LocalizedString("ErrorToStringNotImplemented", "Ex7FFFA1", false, true, DataStrings.ResourceManager, new object[]
			{
				sourceType
			});
		}

		public static LocalizedString DaysOfWeek_None
		{
			get
			{
				return new LocalizedString("DaysOfWeek_None", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAddressSpaceTypeNullOrEmpty
		{
			get
			{
				return new LocalizedString("InvalidAddressSpaceTypeNullOrEmpty", "Ex3CE1A7", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProxyAddressShouldNotBeAllSpace
		{
			get
			{
				return new LocalizedString("ProxyAddressShouldNotBeAllSpace", "ExF7F3BF", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarSharingFreeBusyReviewer
		{
			get
			{
				return new LocalizedString("CalendarSharingFreeBusyReviewer", "ExC08914", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProtocolLocalRpc
		{
			get
			{
				return new LocalizedString("ProtocolLocalRpc", "Ex4A438E", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InsufficientSpace
		{
			get
			{
				return new LocalizedString("InsufficientSpace", "ExB951DE", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeUsers
		{
			get
			{
				return new LocalizedString("ExchangeUsers", "ExA30F35", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoutingIncompatibleDeliveryDomain
		{
			get
			{
				return new LocalizedString("RoutingIncompatibleDeliveryDomain", "Ex14B04A", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CmdletFullNameFormatException(string name)
		{
			return new LocalizedString("CmdletFullNameFormatException", "ExE01E63", false, true, DataStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString EnterpriseTrialEdition
		{
			get
			{
				return new LocalizedString("EnterpriseTrialEdition", "Ex444ED8", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NumberFormatStringTooLong(string propertyName, int maxLength, int actualLength)
		{
			return new LocalizedString("NumberFormatStringTooLong", "", false, false, DataStrings.ResourceManager, new object[]
			{
				propertyName,
				maxLength,
				actualLength
			});
		}

		public static LocalizedString InvalidNumberFormatString
		{
			get
			{
				return new LocalizedString("InvalidNumberFormatString", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionInvalidFormat(string token, Type type, string invalidQuery, int position)
		{
			return new LocalizedString("ExceptionInvalidFormat", "Ex79A704", false, true, DataStrings.ResourceManager, new object[]
			{
				token,
				type,
				invalidQuery,
				position
			});
		}

		public static LocalizedString ErrorFileShareWitnessServerNameIsNotValidHostNameorFqdnWildcard(string computerName)
		{
			return new LocalizedString("ErrorFileShareWitnessServerNameIsNotValidHostNameorFqdnWildcard", "", false, false, DataStrings.ResourceManager, new object[]
			{
				computerName
			});
		}

		public static LocalizedString Partners
		{
			get
			{
				return new LocalizedString("Partners", "Ex19525C", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidTypeArgumentException(string paramName, Type type, Type expectedType)
		{
			return new LocalizedString("InvalidTypeArgumentException", "Ex65174A", false, true, DataStrings.ResourceManager, new object[]
			{
				paramName,
				type,
				expectedType
			});
		}

		public static LocalizedString CustomScheduleDaily12PM
		{
			get
			{
				return new LocalizedString("CustomScheduleDaily12PM", "ExAE8945", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PropertyTypeMismatch(string actualType, string requiredType)
		{
			return new LocalizedString("PropertyTypeMismatch", "ExF9F1C0", false, true, DataStrings.ResourceManager, new object[]
			{
				actualType,
				requiredType
			});
		}

		public static LocalizedString CoexistenceTrialEdition
		{
			get
			{
				return new LocalizedString("CoexistenceTrialEdition", "Ex39589B", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomExtensionInvalidArgument
		{
			get
			{
				return new LocalizedString("CustomExtensionInvalidArgument", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NonWorkingHours
		{
			get
			{
				return new LocalizedString("NonWorkingHours", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientAccessProtocolIMAP4
		{
			get
			{
				return new LocalizedString("ClientAccessProtocolIMAP4", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShadowMessagePreferencePreferRemote
		{
			get
			{
				return new LocalizedString("ShadowMessagePreferencePreferRemote", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomScheduleDailyFromMidnightTo4AM
		{
			get
			{
				return new LocalizedString("CustomScheduleDailyFromMidnightTo4AM", "Ex5C8BA3", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomScheduleDailyFrom8AMTo5PMAtWeekDays
		{
			get
			{
				return new LocalizedString("CustomScheduleDailyFrom8AMTo5PMAtWeekDays", "ExA362EC", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ScriptRoleEntryNameInvalidException(string name)
		{
			return new LocalizedString("ScriptRoleEntryNameInvalidException", "ExB2981E", false, true, DataStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ExceptionParseNonFilterablePropertyErrorWithList(string propertyName, string knownProperties, string invalidQuery, int position)
		{
			return new LocalizedString("ExceptionParseNonFilterablePropertyErrorWithList", "", false, false, DataStrings.ResourceManager, new object[]
			{
				propertyName,
				knownProperties,
				invalidQuery,
				position
			});
		}

		public static LocalizedString ExceptionProtocolConnectionSettingsInvalidEncryptionType(string settings)
		{
			return new LocalizedString("ExceptionProtocolConnectionSettingsInvalidEncryptionType", "ExB4D5E2", false, true, DataStrings.ResourceManager, new object[]
			{
				settings
			});
		}

		public static LocalizedString ErrorCannotCopyFromDifferentType(Type thisType, Type otherType)
		{
			return new LocalizedString("ErrorCannotCopyFromDifferentType", "ExEB6451", false, true, DataStrings.ResourceManager, new object[]
			{
				thisType,
				otherType
			});
		}

		public static LocalizedString ConfigurationSettingsPropertyNotFound2(string name, string knownProperties)
		{
			return new LocalizedString("ConfigurationSettingsPropertyNotFound2", "", false, false, DataStrings.ResourceManager, new object[]
			{
				name,
				knownProperties
			});
		}

		public static LocalizedString InvalidCIDRLengthIPv4(short cidrlength)
		{
			return new LocalizedString("InvalidCIDRLengthIPv4", "ExE65AB7", false, true, DataStrings.ResourceManager, new object[]
			{
				cidrlength
			});
		}

		public static LocalizedString EventLogText
		{
			get
			{
				return new LocalizedString("EventLogText", "Ex796CF5", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientAccessAdfsAuthentication
		{
			get
			{
				return new LocalizedString("ClientAccessAdfsAuthentication", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorADFormatError
		{
			get
			{
				return new LocalizedString("ErrorADFormatError", "Ex5DCFC1", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MeetingFullDetailsWithAttendees
		{
			get
			{
				return new LocalizedString("MeetingFullDetailsWithAttendees", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomScheduleDaily3AM
		{
			get
			{
				return new LocalizedString("CustomScheduleDaily3AM", "ExDABD94", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionCalculatedDependsOnCalculated
		{
			get
			{
				return new LocalizedString("ExceptionCalculatedDependsOnCalculated", "ExDBC5CA", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoutingNoRouteToMdb
		{
			get
			{
				return new LocalizedString("RoutingNoRouteToMdb", "Ex145AF6", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationValueIsNotInGivenStringArray(string arrayValues, string input)
		{
			return new LocalizedString("ConstraintViolationValueIsNotInGivenStringArray", "ExE53E39", false, true, DataStrings.ResourceManager, new object[]
			{
				arrayValues,
				input
			});
		}

		public static LocalizedString SmtpReceiveCapabilitiesAcceptProxyProtocol
		{
			get
			{
				return new LocalizedString("SmtpReceiveCapabilitiesAcceptProxyProtocol", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeferReasonADTransientFailureDuringContentConversion
		{
			get
			{
				return new LocalizedString("DeferReasonADTransientFailureDuringContentConversion", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionRemoveSmtpPrimary(string primary)
		{
			return new LocalizedString("ExceptionRemoveSmtpPrimary", "Ex277A23", false, true, DataStrings.ResourceManager, new object[]
			{
				primary
			});
		}

		public static LocalizedString HeaderPromotionModeMayCreate
		{
			get
			{
				return new LocalizedString("HeaderPromotionModeMayCreate", "Ex16B729", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotConvertFromBinary(string resultType, string error)
		{
			return new LocalizedString("ErrorCannotConvertFromBinary", "Ex4C9ABB", false, true, DataStrings.ResourceManager, new object[]
			{
				resultType,
				error
			});
		}

		public static LocalizedString ConnectorSourceHybridWizard
		{
			get
			{
				return new LocalizedString("ConnectorSourceHybridWizard", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeferReasonAgent
		{
			get
			{
				return new LocalizedString("DeferReasonAgent", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidCustomMenuKeyMappingA
		{
			get
			{
				return new LocalizedString("InvalidCustomMenuKeyMappingA", "Ex26F508", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidResourcePropertySyntax
		{
			get
			{
				return new LocalizedString("InvalidResourcePropertySyntax", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SmtpReceiveCapabilitiesAcceptCloudServicesMail
		{
			get
			{
				return new LocalizedString("SmtpReceiveCapabilitiesAcceptCloudServicesMail", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CcMailProxyAddressPrefixDisplayName
		{
			get
			{
				return new LocalizedString("CcMailProxyAddressPrefixDisplayName", "Ex0C72EB", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomScheduleEveryHour
		{
			get
			{
				return new LocalizedString("CustomScheduleEveryHour", "ExD1431D", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeferReasonConcurrencyLimitInStoreDriver
		{
			get
			{
				return new LocalizedString("DeferReasonConcurrencyLimitInStoreDriver", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupByMonth
		{
			get
			{
				return new LocalizedString("GroupByMonth", "ExCBBDED", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSmtpX509Identifier(string s)
		{
			return new LocalizedString("InvalidSmtpX509Identifier", "", false, false, DataStrings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString DNWithBinaryFormatError(string dnwb)
		{
			return new LocalizedString("DNWithBinaryFormatError", "ExF65E77", false, true, DataStrings.ResourceManager, new object[]
			{
				dnwb
			});
		}

		public static LocalizedString ContactsSharing
		{
			get
			{
				return new LocalizedString("ContactsSharing", "Ex874F2A", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionUnsupportedNetworkProtocol
		{
			get
			{
				return new LocalizedString("ExceptionUnsupportedNetworkProtocol", "ExD9EE37", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SmtpReceiveCapabilitiesAcceptXAttrProtocol
		{
			get
			{
				return new LocalizedString("SmtpReceiveCapabilitiesAcceptXAttrProtocol", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionInvlidCharInProtocolName
		{
			get
			{
				return new LocalizedString("ExceptionInvlidCharInProtocolName", "Ex87B6A7", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DialGroupNotSpecifiedOnDialPlan(string name)
		{
			return new LocalizedString("DialGroupNotSpecifiedOnDialPlan", "", false, false, DataStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString TransientFailure
		{
			get
			{
				return new LocalizedString("TransientFailure", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidInputErrorMsg
		{
			get
			{
				return new LocalizedString("InvalidInputErrorMsg", "ExDEB0CE", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorFileShareWitnessServerNameMustNotBeEmpty
		{
			get
			{
				return new LocalizedString("ErrorFileShareWitnessServerNameMustNotBeEmpty", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationPathLength(string path, string error)
		{
			return new LocalizedString("ConstraintViolationPathLength", "Ex00FF3E", false, true, DataStrings.ResourceManager, new object[]
			{
				path,
				error
			});
		}

		public static LocalizedString ConfigurationSettingsPropertyBadValue(string name, string value)
		{
			return new LocalizedString("ConfigurationSettingsPropertyBadValue", "", false, false, DataStrings.ResourceManager, new object[]
			{
				name,
				value
			});
		}

		public static LocalizedString HostnameTooLong
		{
			get
			{
				return new LocalizedString("HostnameTooLong", "Ex528C69", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidKeyMappingVoiceMail
		{
			get
			{
				return new LocalizedString("InvalidKeyMappingVoiceMail", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KindKeywordEmail
		{
			get
			{
				return new LocalizedString("KindKeywordEmail", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationStringLengthIsEmpty
		{
			get
			{
				return new LocalizedString("ConstraintViolationStringLengthIsEmpty", "Ex916A5D", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AnonymousUsers
		{
			get
			{
				return new LocalizedString("AnonymousUsers", "Ex82B65C", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ThrottlingPolicyStateCorrupted(string value)
		{
			return new LocalizedString("ThrottlingPolicyStateCorrupted", "ExC8845E", false, true, DataStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString ErrorQuotionMarkNotSupportedInKql
		{
			get
			{
				return new LocalizedString("ErrorQuotionMarkNotSupportedInKql", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LatAmSpanish
		{
			get
			{
				return new LocalizedString("LatAmSpanish", "ExEE5C93", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationObjectIsBelowRange(string lowObject)
		{
			return new LocalizedString("ConstraintViolationObjectIsBelowRange", "ExD08CD7", false, true, DataStrings.ResourceManager, new object[]
			{
				lowObject
			});
		}

		public static LocalizedString SearchRecipientsCc
		{
			get
			{
				return new LocalizedString("SearchRecipientsCc", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidOperationCurrentProperty
		{
			get
			{
				return new LocalizedString("InvalidOperationCurrentProperty", "Ex4E3059", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPoliciesUpgradeCustomFailures
		{
			get
			{
				return new LocalizedString("ErrorPoliciesUpgradeCustomFailures", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionVariablesNotSupported(string invalidQuery, int position)
		{
			return new LocalizedString("ExceptionVariablesNotSupported", "Ex2D4D13", false, true, DataStrings.ResourceManager, new object[]
			{
				invalidQuery,
				position
			});
		}

		public static LocalizedString CustomScheduleEveryHalfHour
		{
			get
			{
				return new LocalizedString("CustomScheduleEveryHalfHour", "Ex668840", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmptyExchangeObjectVersion
		{
			get
			{
				return new LocalizedString("EmptyExchangeObjectVersion", "ExFAC945", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomGreetingFilePatternDescription
		{
			get
			{
				return new LocalizedString("CustomGreetingFilePatternDescription", "ExB92914", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Partitioned
		{
			get
			{
				return new LocalizedString("Partitioned", "ExA7EC3C", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientAccessProtocolOA
		{
			get
			{
				return new LocalizedString("ClientAccessProtocolOA", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFolderPermissionRolePublishingAuthor
		{
			get
			{
				return new LocalizedString("PublicFolderPermissionRolePublishingAuthor", "Ex219C85", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidIPAddressInSmartHost
		{
			get
			{
				return new LocalizedString("InvalidIPAddressInSmartHost", "ExAF9E3B", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeliveryTypeSmtpRelayToMailboxDeliveryGroup
		{
			get
			{
				return new LocalizedString("DeliveryTypeSmtpRelayToMailboxDeliveryGroup", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnknownEdition
		{
			get
			{
				return new LocalizedString("UnknownEdition", "ExEDE19B", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfigurationSettingsScopePropertyBadValue(string name, string value)
		{
			return new LocalizedString("ConfigurationSettingsScopePropertyBadValue", "", false, false, DataStrings.ResourceManager, new object[]
			{
				name,
				value
			});
		}

		public static LocalizedString InvalidFormatExchangeObjectVersion
		{
			get
			{
				return new LocalizedString("InvalidFormatExchangeObjectVersion", "ExF5FF7E", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidEumAddress(string address)
		{
			return new LocalizedString("InvalidEumAddress", "Ex8348ED", false, true, DataStrings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString ElcScheduleInvalidType(string actualType)
		{
			return new LocalizedString("ElcScheduleInvalidType", "Ex52A42B", false, true, DataStrings.ResourceManager, new object[]
			{
				actualType
			});
		}

		public static LocalizedString ToInternet
		{
			get
			{
				return new LocalizedString("ToInternet", "Ex5E345F", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionUnsupportedTypeConversion(string desired)
		{
			return new LocalizedString("ExceptionUnsupportedTypeConversion", "ExC2AC8C", false, true, DataStrings.ResourceManager, new object[]
			{
				desired
			});
		}

		public static LocalizedString KindKeywordTasks
		{
			get
			{
				return new LocalizedString("KindKeywordTasks", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSmtpAddress(string address)
		{
			return new LocalizedString("InvalidSmtpAddress", "ExD11FBD", false, true, DataStrings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString MarkedAsRetryDeliveryIfRejected
		{
			get
			{
				return new LocalizedString("MarkedAsRetryDeliveryIfRejected", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectorTypeOnPremises
		{
			get
			{
				return new LocalizedString("ConnectorTypeOnPremises", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HeaderValue
		{
			get
			{
				return new LocalizedString("HeaderValue", "ExC88A06", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SmtpReceiveCapabilitiesAcceptProxyFromProtocol
		{
			get
			{
				return new LocalizedString("SmtpReceiveCapabilitiesAcceptProxyFromProtocol", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PoisonQueueNextHopDomain
		{
			get
			{
				return new LocalizedString("PoisonQueueNextHopDomain", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorObjectVersionReadOnly(string name)
		{
			return new LocalizedString("ErrorObjectVersionReadOnly", "Ex2F3F64", false, true, DataStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ErrorFileShareWitnessServerNameIsNotValidHostNameorFqdn(string computerName)
		{
			return new LocalizedString("ErrorFileShareWitnessServerNameIsNotValidHostNameorFqdn", "", false, false, DataStrings.ResourceManager, new object[]
			{
				computerName
			});
		}

		public static LocalizedString GroupWiseProxyAddressPrefixDisplayName
		{
			get
			{
				return new LocalizedString("GroupWiseProxyAddressPrefixDisplayName", "Ex9B0DBA", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MsMailProxyAddressPrefixDisplayName
		{
			get
			{
				return new LocalizedString("MsMailProxyAddressPrefixDisplayName", "Ex7936C6", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Exchange2007
		{
			get
			{
				return new LocalizedString("Exchange2007", "ExD9D03B", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionParseNonFilterablePropertyError(string propertyName, string invalidQuery, int position)
		{
			return new LocalizedString("ExceptionParseNonFilterablePropertyError", "ExDF50CE", false, true, DataStrings.ResourceManager, new object[]
			{
				propertyName,
				invalidQuery,
				position
			});
		}

		public static LocalizedString HeaderName
		{
			get
			{
				return new LocalizedString("HeaderName", "ExE2AE7E", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RejectText
		{
			get
			{
				return new LocalizedString("RejectText", "Ex8C2FA7", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NameValidationSpaceAllowedPatternDescription
		{
			get
			{
				return new LocalizedString("NameValidationSpaceAllowedPatternDescription", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationStringLengthCauseOutOfMemory
		{
			get
			{
				return new LocalizedString("ConstraintViolationStringLengthCauseOutOfMemory", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionUnsupportedDestinationType(string type)
		{
			return new LocalizedString("ExceptionUnsupportedDestinationType", "Ex8C29A6", false, true, DataStrings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString InvalidFormat
		{
			get
			{
				return new LocalizedString("InvalidFormat", "ExBD3CCB", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomPeriod
		{
			get
			{
				return new LocalizedString("CustomPeriod", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFolderPermissionRolePublishingEditor
		{
			get
			{
				return new LocalizedString("PublicFolderPermissionRolePublishingEditor", "Ex2F1F29", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationNotValidEmailAddress(string emailAddress)
		{
			return new LocalizedString("ConstraintViolationNotValidEmailAddress", "ExBB250A", false, true, DataStrings.ResourceManager, new object[]
			{
				emailAddress
			});
		}

		public static LocalizedString ErrorInputSchedulerBuilder(int actual, int expected)
		{
			return new LocalizedString("ErrorInputSchedulerBuilder", "Ex4A115B", false, true, DataStrings.ResourceManager, new object[]
			{
				actual,
				expected
			});
		}

		public static LocalizedString ConstraintViolationInvalidUriScheme(Uri uri, string uriSchemes)
		{
			return new LocalizedString("ConstraintViolationInvalidUriScheme", "", false, false, DataStrings.ResourceManager, new object[]
			{
				uri,
				uriSchemes
			});
		}

		public static LocalizedString ExceptionCurrentIndexOutOfRange(string current, string minimum, string maximum)
		{
			return new LocalizedString("ExceptionCurrentIndexOutOfRange", "ExA12D5C", false, true, DataStrings.ResourceManager, new object[]
			{
				current,
				minimum,
				maximum
			});
		}

		public static LocalizedString InvalidKeyMappingKey
		{
			get
			{
				return new LocalizedString("InvalidKeyMappingKey", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StringConversionDelegateNotSet
		{
			get
			{
				return new LocalizedString("StringConversionDelegateNotSet", "ExE84810", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NumberingPlanPatternDescription
		{
			get
			{
				return new LocalizedString("NumberingPlanPatternDescription", "Ex722E57", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MeetingFullDetails
		{
			get
			{
				return new LocalizedString("MeetingFullDetails", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProtocolLoggingLevelNone
		{
			get
			{
				return new LocalizedString("ProtocolLoggingLevelNone", "Ex896283", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncludeExcludeConflict(string value)
		{
			return new LocalizedString("IncludeExcludeConflict", "", false, false, DataStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString ErrorInputFormatError
		{
			get
			{
				return new LocalizedString("ErrorInputFormatError", "Ex758C3C", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Ascending
		{
			get
			{
				return new LocalizedString("Ascending", "Ex7640A8", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionReadOnlyBecauseTooNew(ExchangeObjectVersion objectVersion, ExchangeObjectVersion currentVersion)
		{
			return new LocalizedString("ExceptionReadOnlyBecauseTooNew", "Ex77EADC", false, true, DataStrings.ResourceManager, new object[]
			{
				objectVersion,
				currentVersion
			});
		}

		public static LocalizedString SmtpReceiveCapabilitiesAcceptXOriginalFromProtocol
		{
			get
			{
				return new LocalizedString("SmtpReceiveCapabilitiesAcceptXOriginalFromProtocol", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Exchange2003
		{
			get
			{
				return new LocalizedString("Exchange2003", "Ex14D09F", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WorkingHours
		{
			get
			{
				return new LocalizedString("WorkingHours", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeferReasonTransientAttributionFailure
		{
			get
			{
				return new LocalizedString("DeferReasonTransientAttributionFailure", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FromEnterprise
		{
			get
			{
				return new LocalizedString("FromEnterprise", "Ex3A11E3", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TlsAuthLevelCertificateValidation
		{
			get
			{
				return new LocalizedString("TlsAuthLevelCertificateValidation", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PropertyNotACollection(string actualType)
		{
			return new LocalizedString("PropertyNotACollection", "Ex0D1DEF", false, true, DataStrings.ResourceManager, new object[]
			{
				actualType
			});
		}

		public static LocalizedString Friday
		{
			get
			{
				return new LocalizedString("Friday", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MeetingLimitedDetails
		{
			get
			{
				return new LocalizedString("MeetingLimitedDetails", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KeyMappingInvalidArgument
		{
			get
			{
				return new LocalizedString("KeyMappingInvalidArgument", "Ex021240", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorFileShareWitnessServerNameIsLocalhost(string computerName)
		{
			return new LocalizedString("ErrorFileShareWitnessServerNameIsLocalhost", "", false, false, DataStrings.ResourceManager, new object[]
			{
				computerName
			});
		}

		public static LocalizedString LegacyDNPatternDescription
		{
			get
			{
				return new LocalizedString("LegacyDNPatternDescription", "ExA6B413", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SmtpReceiveCapabilitiesAcceptXSysProbeProtocol
		{
			get
			{
				return new LocalizedString("SmtpReceiveCapabilitiesAcceptXSysProbeProtocol", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SentTime
		{
			get
			{
				return new LocalizedString("SentTime", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CollectiionWithTooManyItemsFormat(string value)
		{
			return new LocalizedString("CollectiionWithTooManyItemsFormat", "Ex4166C5", false, true, DataStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString SearchSender
		{
			get
			{
				return new LocalizedString("SearchSender", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationEnumValueNotDefined(string actualValue, string enumType)
		{
			return new LocalizedString("ConstraintViolationEnumValueNotDefined", "Ex37CD44", false, true, DataStrings.ResourceManager, new object[]
			{
				actualValue,
				enumType
			});
		}

		public static LocalizedString ExArgumentNullException(string paramName)
		{
			return new LocalizedString("ExArgumentNullException", "ExB256EB", false, true, DataStrings.ResourceManager, new object[]
			{
				paramName
			});
		}

		public static LocalizedString ExceptionProtocolConnectionSettingsInvalidFormat(string settings)
		{
			return new LocalizedString("ExceptionProtocolConnectionSettingsInvalidFormat", "ExDA3189", false, true, DataStrings.ResourceManager, new object[]
			{
				settings
			});
		}

		public static LocalizedString ErrorInvalidEnumValue(string values)
		{
			return new LocalizedString("ErrorInvalidEnumValue", "ExEFE4B2", false, true, DataStrings.ResourceManager, new object[]
			{
				values
			});
		}

		public static LocalizedString ExceptionComparisonNotSupported(string name, Type type, ComparisonOperator comparison)
		{
			return new LocalizedString("ExceptionComparisonNotSupported", "Ex7B21BF", false, true, DataStrings.ResourceManager, new object[]
			{
				name,
				type,
				comparison
			});
		}

		public static LocalizedString ErrorCannotAddNullValue
		{
			get
			{
				return new LocalizedString("ErrorCannotAddNullValue", "Ex4521B2", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDialGroupEntryElementLength(string val, string name, int len)
		{
			return new LocalizedString("InvalidDialGroupEntryElementLength", "", false, false, DataStrings.ResourceManager, new object[]
			{
				val,
				name,
				len
			});
		}

		public static LocalizedString ExceptionGeoCoordinatesWithInvalidLatitude(string geoCoordinates)
		{
			return new LocalizedString("ExceptionGeoCoordinatesWithInvalidLatitude", "", false, false, DataStrings.ResourceManager, new object[]
			{
				geoCoordinates
			});
		}

		public static LocalizedString ScheduleModeScheduledTimes
		{
			get
			{
				return new LocalizedString("ScheduleModeScheduledTimes", "ExE1314D", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProtocolAppleTalk
		{
			get
			{
				return new LocalizedString("ProtocolAppleTalk", "Ex18A87A", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DigitStringOrEmptyPatternDescription
		{
			get
			{
				return new LocalizedString("DigitStringOrEmptyPatternDescription", "ExCDCC63", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EnterpriseEdition
		{
			get
			{
				return new LocalizedString("EnterpriseEdition", "ExA73FFA", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionInvalidLatitude(double lat)
		{
			return new LocalizedString("ExceptionInvalidLatitude", "", false, false, DataStrings.ResourceManager, new object[]
			{
				lat
			});
		}

		public static LocalizedString ConnectorSourceDefault
		{
			get
			{
				return new LocalizedString("ConnectorSourceDefault", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionVersionlessObject
		{
			get
			{
				return new LocalizedString("ExceptionVersionlessObject", "ExA9E0E4", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AliasPatternDescription
		{
			get
			{
				return new LocalizedString("AliasPatternDescription", "Ex57B176", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReceivedTime
		{
			get
			{
				return new LocalizedString("ReceivedTime", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ParameterNameEmptyException
		{
			get
			{
				return new LocalizedString("ParameterNameEmptyException", "ExC865C2", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecipientStatusLocked
		{
			get
			{
				return new LocalizedString("RecipientStatusLocked", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SmtpReceiveCapabilitiesAcceptProxyToProtocol
		{
			get
			{
				return new LocalizedString("SmtpReceiveCapabilitiesAcceptProxyToProtocol", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PermissionGroupsNone
		{
			get
			{
				return new LocalizedString("PermissionGroupsNone", "Ex25D8F9", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidOrganizationSummaryEntryFormat(string s)
		{
			return new LocalizedString("InvalidOrganizationSummaryEntryFormat", "Ex74AC48", false, true, DataStrings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString InvalidDateFormat(string date, string fmt1)
		{
			return new LocalizedString("InvalidDateFormat", "", false, false, DataStrings.ResourceManager, new object[]
			{
				date,
				fmt1
			});
		}

		public static LocalizedString ArgumentMustBeAscii
		{
			get
			{
				return new LocalizedString("ArgumentMustBeAscii", "Ex3E1102", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionNetworkProtocolDuplicate
		{
			get
			{
				return new LocalizedString("ExceptionNetworkProtocolDuplicate", "ExB269FF", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecipientStatusRetry
		{
			get
			{
				return new LocalizedString("RecipientStatusRetry", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidEumAddressTemplateFormat(string template)
		{
			return new LocalizedString("InvalidEumAddressTemplateFormat", "ExD4A618", false, true, DataStrings.ResourceManager, new object[]
			{
				template
			});
		}

		public static LocalizedString GroupExpansionRecipients
		{
			get
			{
				return new LocalizedString("GroupExpansionRecipients", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LegacyDNProxyAddressPrefixDisplayName
		{
			get
			{
				return new LocalizedString("LegacyDNProxyAddressPrefixDisplayName", "ExCF27DD", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HeaderPromotionModeMustCreate
		{
			get
			{
				return new LocalizedString("HeaderPromotionModeMustCreate", "ExE35B1A", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProtocolLoggingLevelVerbose
		{
			get
			{
				return new LocalizedString("ProtocolLoggingLevelVerbose", "ExD3B628", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HeaderPromotionModeNoCreate
		{
			get
			{
				return new LocalizedString("HeaderPromotionModeNoCreate", "ExBCF9A1", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EncryptionTypeSSL
		{
			get
			{
				return new LocalizedString("EncryptionTypeSSL", "Ex96F5EC", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionProtocolConnectionSettingsInvalidPort(string settings, int min, int max)
		{
			return new LocalizedString("ExceptionProtocolConnectionSettingsInvalidPort", "ExC94EB8", false, true, DataStrings.ResourceManager, new object[]
			{
				settings,
				min,
				max
			});
		}

		public static LocalizedString ConstraintViolationObjectIsBeyondRange(string highObject)
		{
			return new LocalizedString("ConstraintViolationObjectIsBeyondRange", "ExA7FF86", false, true, DataStrings.ResourceManager, new object[]
			{
				highObject
			});
		}

		public static LocalizedString KindKeywordDocs
		{
			get
			{
				return new LocalizedString("KindKeywordDocs", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Unreachable
		{
			get
			{
				return new LocalizedString("Unreachable", "Ex85C2BE", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomScheduleSundayAtMidnight
		{
			get
			{
				return new LocalizedString("CustomScheduleSundayAtMidnight", "Ex5DEC9D", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionGeoCoordinatesWithInvalidLongitude(string geoCoordinates)
		{
			return new LocalizedString("ExceptionGeoCoordinatesWithInvalidLongitude", "", false, false, DataStrings.ResourceManager, new object[]
			{
				geoCoordinates
			});
		}

		public static LocalizedString ExceptionInvalidOperation(string op, string typeName)
		{
			return new LocalizedString("ExceptionInvalidOperation", "ExA72668", false, true, DataStrings.ResourceManager, new object[]
			{
				op,
				typeName
			});
		}

		public static LocalizedString ExceptionUnknownUnit
		{
			get
			{
				return new LocalizedString("ExceptionUnknownUnit", "ExFDAF41", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorOperationNotSupported(string originalType, string resultType)
		{
			return new LocalizedString("ErrorOperationNotSupported", "Ex30D899", false, true, DataStrings.ResourceManager, new object[]
			{
				originalType,
				resultType
			});
		}

		public static LocalizedString ErrorIncorrectLiveIdFormat(string netID)
		{
			return new LocalizedString("ErrorIncorrectLiveIdFormat", "Ex7138C9", false, true, DataStrings.ResourceManager, new object[]
			{
				netID
			});
		}

		public static LocalizedString SendNone
		{
			get
			{
				return new LocalizedString("SendNone", "Ex7A6E9F", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnknownColumns(string columns, IEnumerable<string> unknownColumns)
		{
			return new LocalizedString("UnknownColumns", "Ex510233", false, true, DataStrings.ResourceManager, new object[]
			{
				columns,
				unknownColumns
			});
		}

		public static LocalizedString SubjectPrefix
		{
			get
			{
				return new LocalizedString("SubjectPrefix", "Ex1B5E7F", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeliveryTypeSmtpRelayToServers
		{
			get
			{
				return new LocalizedString("DeliveryTypeSmtpRelayToServers", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidKeyMappingFindMeFirstNumberDuration
		{
			get
			{
				return new LocalizedString("InvalidKeyMappingFindMeFirstNumberDuration", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidTypeArgumentExceptionMultipleExceptedTypes(string paramName, Type type, Type expectedType, Type expectedType2)
		{
			return new LocalizedString("InvalidTypeArgumentExceptionMultipleExceptedTypes", "Ex2BDBFD", false, true, DataStrings.ResourceManager, new object[]
			{
				paramName,
				type,
				expectedType,
				expectedType2
			});
		}

		public static LocalizedString AirSyncProxyAddressPrefixDisplayName
		{
			get
			{
				return new LocalizedString("AirSyncProxyAddressPrefixDisplayName", "ExAF06B5", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CoexistenceEdition
		{
			get
			{
				return new LocalizedString("CoexistenceEdition", "Ex6175A7", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnsearchableItemsAdded
		{
			get
			{
				return new LocalizedString("UnsearchableItemsAdded", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidKeyMappingFormat
		{
			get
			{
				return new LocalizedString("InvalidKeyMappingFormat", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientAccessProtocolPOP3
		{
			get
			{
				return new LocalizedString("ClientAccessProtocolPOP3", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationIpV6NotAllowed(string ipAddress)
		{
			return new LocalizedString("ConstraintViolationIpV6NotAllowed", "", false, false, DataStrings.ResourceManager, new object[]
			{
				ipAddress
			});
		}

		public static LocalizedString ExceptionEventCategoryNotFound(string eventCategory)
		{
			return new LocalizedString("ExceptionEventCategoryNotFound", "Ex0F2690", false, true, DataStrings.ResourceManager, new object[]
			{
				eventCategory
			});
		}

		public static LocalizedString Word
		{
			get
			{
				return new LocalizedString("Word", "ExFDE75C", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddressSpaceIsTooLong
		{
			get
			{
				return new LocalizedString("AddressSpaceIsTooLong", "Ex259905", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddressFamilyMismatch
		{
			get
			{
				return new LocalizedString("AddressFamilyMismatch", "Ex9025C4", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KindKeywordFaxes
		{
			get
			{
				return new LocalizedString("KindKeywordFaxes", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorObjectSerialization(ObjectId id, Version currentVersion, Version objectVersion)
		{
			return new LocalizedString("ErrorObjectSerialization", "ExBA6ECA", false, true, DataStrings.ResourceManager, new object[]
			{
				id,
				currentVersion,
				objectVersion
			});
		}

		public static LocalizedString ExceptionEmptyProxyAddress
		{
			get
			{
				return new LocalizedString("ExceptionEmptyProxyAddress", "ExE01039", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionObjectInvalid
		{
			get
			{
				return new LocalizedString("ExceptionObjectInvalid", "Ex39A256", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationInvalidUriKind(Uri uri, UriKind uriKind)
		{
			return new LocalizedString("ConstraintViolationInvalidUriKind", "Ex246112", false, true, DataStrings.ResourceManager, new object[]
			{
				uri,
				uriKind
			});
		}

		public static LocalizedString ExceptionReadOnlyMultiValuedProperty
		{
			get
			{
				return new LocalizedString("ExceptionReadOnlyMultiValuedProperty", "ExA3B4E0", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ParameterNameInvalidCharException(string name)
		{
			return new LocalizedString("ParameterNameInvalidCharException", "Ex31250B", false, true, DataStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString Tuesday
		{
			get
			{
				return new LocalizedString("Tuesday", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeferReasonADTransientFailureDuringResolve
		{
			get
			{
				return new LocalizedString("DeferReasonADTransientFailureDuringResolve", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeliveryTypeNonSmtpGatewayDelivery
		{
			get
			{
				return new LocalizedString("DeliveryTypeNonSmtpGatewayDelivery", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UseExchangeDSNs
		{
			get
			{
				return new LocalizedString("UseExchangeDSNs", "ExA4287D", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfigurationSettingsScopePropertyNotFound(string name)
		{
			return new LocalizedString("ConfigurationSettingsScopePropertyNotFound", "", false, false, DataStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ConstraintViolationSecurityDescriptorContainsInheritedACEs(string sddl)
		{
			return new LocalizedString("ConstraintViolationSecurityDescriptorContainsInheritedACEs", "", false, false, DataStrings.ResourceManager, new object[]
			{
				sddl
			});
		}

		public static LocalizedString KindKeywordContacts
		{
			get
			{
				return new LocalizedString("KindKeywordContacts", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FromLocal
		{
			get
			{
				return new LocalizedString("FromLocal", "Ex10A5E0", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPoliciesDowngradeDnsFailures
		{
			get
			{
				return new LocalizedString("ErrorPoliciesDowngradeDnsFailures", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationInvalidIntRange(int min, int max, string range)
		{
			return new LocalizedString("ConstraintViolationInvalidIntRange", "", false, false, DataStrings.ResourceManager, new object[]
			{
				min,
				max,
				range
			});
		}

		public static LocalizedString TlsAuthLevelEncryptionOnly
		{
			get
			{
				return new LocalizedString("TlsAuthLevelEncryptionOnly", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationValueIsNotAllowed(string validValues, string input)
		{
			return new LocalizedString("ConstraintViolationValueIsNotAllowed", "ExC6B952", false, true, DataStrings.ResourceManager, new object[]
			{
				validValues,
				input
			});
		}

		public static LocalizedString ConstraintViolationMalformedExtensionValuePair(string actualValue)
		{
			return new LocalizedString("ConstraintViolationMalformedExtensionValuePair", "Ex6FFF62", false, true, DataStrings.ResourceManager, new object[]
			{
				actualValue
			});
		}

		public static LocalizedString Sunday
		{
			get
			{
				return new LocalizedString("Sunday", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomScheduleDailyFrom9AMTo6PMAtWeekDays
		{
			get
			{
				return new LocalizedString("CustomScheduleDailyFrom9AMTo6PMAtWeekDays", "Ex4BC927", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Descending
		{
			get
			{
				return new LocalizedString("Descending", "ExE0EE79", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeliveryTypeUnreachable
		{
			get
			{
				return new LocalizedString("DeliveryTypeUnreachable", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KindKeywordPosts
		{
			get
			{
				return new LocalizedString("KindKeywordPosts", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorServerGuidAndNameBothEmpty
		{
			get
			{
				return new LocalizedString("ErrorServerGuidAndNameBothEmpty", "ExCE3A3B", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ElcScheduleInvalidIntervals(string actualInterval)
		{
			return new LocalizedString("ElcScheduleInvalidIntervals", "Ex8BF199", false, true, DataStrings.ResourceManager, new object[]
			{
				actualInterval
			});
		}

		public static LocalizedString ExLengthOfVersionByteArrayError
		{
			get
			{
				return new LocalizedString("ExLengthOfVersionByteArrayError", "Ex3FF47B", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchRecipientsTo
		{
			get
			{
				return new LocalizedString("SearchRecipientsTo", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionUsingInvalidAddress(string address, string error)
		{
			return new LocalizedString("ExceptionUsingInvalidAddress", "ExB883C4", false, true, DataStrings.ResourceManager, new object[]
			{
				address,
				error
			});
		}

		public static LocalizedString InvalidNumber(string value, string propertyname)
		{
			return new LocalizedString("InvalidNumber", "", false, false, DataStrings.ResourceManager, new object[]
			{
				value,
				propertyname
			});
		}

		public static LocalizedString ErrorCannotSaveBecauseTooNew(ExchangeObjectVersion objectVersion, ExchangeObjectVersion currentVersion)
		{
			return new LocalizedString("ErrorCannotSaveBecauseTooNew", "Ex755DF0", false, true, DataStrings.ResourceManager, new object[]
			{
				objectVersion,
				currentVersion
			});
		}

		public static LocalizedString ConfigurationSettingsScopePropertyFailedValidation(string name, string value)
		{
			return new LocalizedString("ConfigurationSettingsScopePropertyFailedValidation", "", false, false, DataStrings.ResourceManager, new object[]
			{
				name,
				value
			});
		}

		public static LocalizedString DeliveryTypeMapiDelivery
		{
			get
			{
				return new LocalizedString("DeliveryTypeMapiDelivery", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchRecipientsBcc
		{
			get
			{
				return new LocalizedString("SearchRecipientsBcc", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmptyNameInHostname
		{
			get
			{
				return new LocalizedString("EmptyNameInHostname", "ExF963B0", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DisclaimerText
		{
			get
			{
				return new LocalizedString("DisclaimerText", "Ex4D1DEA", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDomainInSmtpX509Identifier(string s)
		{
			return new LocalizedString("InvalidDomainInSmtpX509Identifier", "", false, false, DataStrings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString InvalidSmtpDomainWildcard
		{
			get
			{
				return new LocalizedString("InvalidSmtpDomainWildcard", "Ex5F07FC", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAddressSpaceCostFormat(string s)
		{
			return new LocalizedString("InvalidAddressSpaceCostFormat", "ExB81FA0", false, true, DataStrings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString CustomScheduleDailyFrom2AMTo6AM
		{
			get
			{
				return new LocalizedString("CustomScheduleDailyFrom2AMTo6AM", "Ex3B0A96", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidTimeOfDayFormat
		{
			get
			{
				return new LocalizedString("InvalidTimeOfDayFormat", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Failed
		{
			get
			{
				return new LocalizedString("Failed", "ExAD0308", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomScheduleDailyFrom11PMTo6AM
		{
			get
			{
				return new LocalizedString("CustomScheduleDailyFrom11PMTo6AM", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServicePlanFeatureCheckFailed(string feature, string sku)
		{
			return new LocalizedString("ServicePlanFeatureCheckFailed", "Ex3EC25C", false, true, DataStrings.ResourceManager, new object[]
			{
				feature,
				sku
			});
		}

		public static LocalizedString ColonPrefix
		{
			get
			{
				return new LocalizedString("ColonPrefix", "Ex1209EF", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ToGroupExpansionRecipients
		{
			get
			{
				return new LocalizedString("ToGroupExpansionRecipients", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidCallerIdItemFormat
		{
			get
			{
				return new LocalizedString("InvalidCallerIdItemFormat", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSMTPAddressTemplateFormat(string template)
		{
			return new LocalizedString("InvalidSMTPAddressTemplateFormat", "ExA86A7F", false, true, DataStrings.ResourceManager, new object[]
			{
				template
			});
		}

		public static LocalizedString PublicFolderPermissionRoleOwner
		{
			get
			{
				return new LocalizedString("PublicFolderPermissionRoleOwner", "ExE13D97", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PermissionGroupsCustom
		{
			get
			{
				return new LocalizedString("PermissionGroupsCustom", "ExCF4BB4", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KindKeywordIm
		{
			get
			{
				return new LocalizedString("KindKeywordIm", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupByTotal
		{
			get
			{
				return new LocalizedString("GroupByTotal", "Ex8E1724", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidOrganizationSummaryEntryValue(string value)
		{
			return new LocalizedString("InvalidOrganizationSummaryEntryValue", "Ex119611", false, true, DataStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString BccGroupExpansionRecipients
		{
			get
			{
				return new LocalizedString("BccGroupExpansionRecipients", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionInvlidNetworkAddressFormat
		{
			get
			{
				return new LocalizedString("ExceptionInvlidNetworkAddressFormat", "Ex6ABEF6", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KindKeywordJournals
		{
			get
			{
				return new LocalizedString("KindKeywordJournals", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmptyExchangeBuild
		{
			get
			{
				return new LocalizedString("EmptyExchangeBuild", "Ex68E6B0", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StandardEdition
		{
			get
			{
				return new LocalizedString("StandardEdition", "Ex248BE8", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FormatExchangeBuildWrong
		{
			get
			{
				return new LocalizedString("FormatExchangeBuildWrong", "ExD0EE61", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DataNotCloneable(string datatype)
		{
			return new LocalizedString("DataNotCloneable", "Ex66D056", false, true, DataStrings.ResourceManager, new object[]
			{
				datatype
			});
		}

		public static LocalizedString ClientAccessProtocolPSWS
		{
			get
			{
				return new LocalizedString("ClientAccessProtocolPSWS", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeliveryTypeSmtpRelayToConnectorSourceServers
		{
			get
			{
				return new LocalizedString("DeliveryTypeSmtpRelayToConnectorSourceServers", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomScheduleSaturdayAtMidnight
		{
			get
			{
				return new LocalizedString("CustomScheduleSaturdayAtMidnight", "ExCB17BE", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionNoValue
		{
			get
			{
				return new LocalizedString("ExceptionNoValue", "ExD91A99", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailRecipientTypeDistributionGroup
		{
			get
			{
				return new LocalizedString("MailRecipientTypeDistributionGroup", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionInvlidProtocolAddressFormat
		{
			get
			{
				return new LocalizedString("ExceptionInvlidProtocolAddressFormat", "ExBC5C32", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomScheduleFridayAtMidnight
		{
			get
			{
				return new LocalizedString("CustomScheduleFridayAtMidnight", "ExC5DF82", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionBitMaskNotSupported(string name, Type type)
		{
			return new LocalizedString("ExceptionBitMaskNotSupported", "Ex482DC2", false, true, DataStrings.ResourceManager, new object[]
			{
				name,
				type
			});
		}

		public static LocalizedString ConstraintViolationStringContainsInvalidCharacters2(string invalidCharacters, string input)
		{
			return new LocalizedString("ConstraintViolationStringContainsInvalidCharacters2", "Ex746C45", false, true, DataStrings.ResourceManager, new object[]
			{
				invalidCharacters,
				input
			});
		}

		public static LocalizedString DeliveryTypeShadowRedundancy
		{
			get
			{
				return new LocalizedString("DeliveryTypeShadowRedundancy", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeferReasonLoopDetected
		{
			get
			{
				return new LocalizedString("DeferReasonLoopDetected", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchRecipients
		{
			get
			{
				return new LocalizedString("SearchRecipients", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidIPAddressMask(string ipAddress)
		{
			return new LocalizedString("InvalidIPAddressMask", "Ex3854FD", false, true, DataStrings.ResourceManager, new object[]
			{
				ipAddress
			});
		}

		public static LocalizedString ConstraintViolationStringDoesNotMatchRegularExpression(string pattern, string input)
		{
			return new LocalizedString("ConstraintViolationStringDoesNotMatchRegularExpression", "ExD5AF6C", false, true, DataStrings.ResourceManager, new object[]
			{
				pattern,
				input
			});
		}

		public static LocalizedString CalendarSharingFreeBusySimple
		{
			get
			{
				return new LocalizedString("CalendarSharingFreeBusySimple", "ExC35F6E", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFolderPermissionRoleReviewer
		{
			get
			{
				return new LocalizedString("PublicFolderPermissionRoleReviewer", "Ex0A53E3", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ScheduleDateInvalid(DateTime start, DateTime end)
		{
			return new LocalizedString("ScheduleDateInvalid", "", false, false, DataStrings.ResourceManager, new object[]
			{
				start,
				end
			});
		}

		public static LocalizedString QueueStatusActive
		{
			get
			{
				return new LocalizedString("QueueStatusActive", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSmtpReceiveDomainCapabilities(string s)
		{
			return new LocalizedString("InvalidSmtpReceiveDomainCapabilities", "Ex020660", false, true, DataStrings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ExceptionCannotResolveOperation(string op, string type1, string type2)
		{
			return new LocalizedString("ExceptionCannotResolveOperation", "ExD39651", false, true, DataStrings.ResourceManager, new object[]
			{
				op,
				type1,
				type2
			});
		}

		public static LocalizedString InvalidConnectedDomainFormat(string s)
		{
			return new LocalizedString("InvalidConnectedDomainFormat", "ExAA0D6C", false, true, DataStrings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString TlsAuthLevelCertificateExpiryCheck
		{
			get
			{
				return new LocalizedString("TlsAuthLevelCertificateExpiryCheck", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAddressSpaceAddress
		{
			get
			{
				return new LocalizedString("InvalidAddressSpaceAddress", "ExAA6190", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidOrganizationSummaryEntryKey(string key)
		{
			return new LocalizedString("InvalidOrganizationSummaryEntryKey", "Ex65A1FA", false, true, DataStrings.ResourceManager, new object[]
			{
				key
			});
		}

		public static LocalizedString FromPartner
		{
			get
			{
				return new LocalizedString("FromPartner", "ExB2FA65", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AllDays
		{
			get
			{
				return new LocalizedString("AllDays", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionEndPointPortOutOfRange(int min, int max, int value)
		{
			return new LocalizedString("ExceptionEndPointPortOutOfRange", "Ex8DC954", false, true, DataStrings.ResourceManager, new object[]
			{
				min,
				max,
				value
			});
		}

		public static LocalizedString DeferReasonTargetSiteInboundMailDisabled
		{
			get
			{
				return new LocalizedString("DeferReasonTargetSiteInboundMailDisabled", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionGeoCoordinatesWithInvalidAltitude(string geoCoordinates)
		{
			return new LocalizedString("ExceptionGeoCoordinatesWithInvalidAltitude", "", false, false, DataStrings.ResourceManager, new object[]
			{
				geoCoordinates
			});
		}

		public static LocalizedString ExceptionFormatNotSupported
		{
			get
			{
				return new LocalizedString("ExceptionFormatNotSupported", "Ex96C3FF", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationEnumValueNotAllowed(string actualValue)
		{
			return new LocalizedString("ConstraintViolationEnumValueNotAllowed", "ExEE6068", false, true, DataStrings.ResourceManager, new object[]
			{
				actualValue
			});
		}

		public static LocalizedString SharingPolicyDomainInvalidDomain(string value)
		{
			return new LocalizedString("SharingPolicyDomainInvalidDomain", "Ex8F6F62", false, true, DataStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString DeliveryTypeDeliveryAgent
		{
			get
			{
				return new LocalizedString("DeliveryTypeDeliveryAgent", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EstimatedItems
		{
			get
			{
				return new LocalizedString("EstimatedItems", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CmdletParameterEmptyValidationException
		{
			get
			{
				return new LocalizedString("CmdletParameterEmptyValidationException", "Ex7F0EEC", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageStatusPendingRemove
		{
			get
			{
				return new LocalizedString("MessageStatusPendingRemove", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QueueStatusConnecting
		{
			get
			{
				return new LocalizedString("QueueStatusConnecting", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnsupportServerEdition(string edition)
		{
			return new LocalizedString("UnsupportServerEdition", "Ex58CEE2", false, true, DataStrings.ResourceManager, new object[]
			{
				edition
			});
		}

		public static LocalizedString DeliveryTypeSmtpRelayToRemoteAdSite
		{
			get
			{
				return new LocalizedString("DeliveryTypeSmtpRelayToRemoteAdSite", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Saturday
		{
			get
			{
				return new LocalizedString("Saturday", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ToEnterprise
		{
			get
			{
				return new LocalizedString("ToEnterprise", "Ex639CAE", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidHolidayScheduleFormat
		{
			get
			{
				return new LocalizedString("InvalidHolidayScheduleFormat", "Ex69C3D0", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationByteArrayLengthTooLong(int maxLength, int actualLength)
		{
			return new LocalizedString("ConstraintViolationByteArrayLengthTooLong", "Ex309B7D", false, true, DataStrings.ResourceManager, new object[]
			{
				maxLength,
				actualLength
			});
		}

		public static LocalizedString InvalidTimeOfDayFormatWorkingHours
		{
			get
			{
				return new LocalizedString("InvalidTimeOfDayFormatWorkingHours", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomScheduleDaily11PM
		{
			get
			{
				return new LocalizedString("CustomScheduleDaily11PM", "Ex37365F", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidCIDRLength(string cidrlength)
		{
			return new LocalizedString("InvalidCIDRLength", "Ex57742F", false, true, DataStrings.ResourceManager, new object[]
			{
				cidrlength
			});
		}

		public static LocalizedString ErrorFileShareWitnessServerNameCannotConvert(string computerName)
		{
			return new LocalizedString("ErrorFileShareWitnessServerNameCannotConvert", "", false, false, DataStrings.ResourceManager, new object[]
			{
				computerName
			});
		}

		public static LocalizedString ExceptionFormatNotCorrect(string value)
		{
			return new LocalizedString("ExceptionFormatNotCorrect", "Ex718F94", false, true, DataStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString QueueStatusSuspended
		{
			get
			{
				return new LocalizedString("QueueStatusSuspended", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationByteArrayLengthTooShort(int minLength, int actualLength)
		{
			return new LocalizedString("ConstraintViolationByteArrayLengthTooShort", "ExA88136", false, true, DataStrings.ResourceManager, new object[]
			{
				minLength,
				actualLength
			});
		}

		public static LocalizedString InvalidX400Domain(string domain)
		{
			return new LocalizedString("InvalidX400Domain", "Ex51AF8A", false, true, DataStrings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString InvalidDialGroupEntryElementFormat(string name)
		{
			return new LocalizedString("InvalidDialGroupEntryElementFormat", "", false, false, DataStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString SubmissionQueueNextHopDomain
		{
			get
			{
				return new LocalizedString("SubmissionQueueNextHopDomain", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNotSupportedForChangesOnlyCopy
		{
			get
			{
				return new LocalizedString("ErrorNotSupportedForChangesOnlyCopy", "Ex65E5CE", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CcGroupExpansionRecipients
		{
			get
			{
				return new LocalizedString("CcGroupExpansionRecipients", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProxyAddressPrefixTooLong
		{
			get
			{
				return new LocalizedString("ProxyAddressPrefixTooLong", "ExBCCEFE", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionPropertyTooNew(string name, ExchangeObjectVersion versionRequired, ExchangeObjectVersion objectVersion)
		{
			return new LocalizedString("ExceptionPropertyTooNew", "Ex51405F", false, true, DataStrings.ResourceManager, new object[]
			{
				name,
				versionRequired,
				objectVersion
			});
		}

		public static LocalizedString Pattern
		{
			get
			{
				return new LocalizedString("Pattern", "ExE1853E", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequiredColumnMissing(string missingColumn)
		{
			return new LocalizedString("RequiredColumnMissing", "Ex45050E", false, true, DataStrings.ResourceManager, new object[]
			{
				missingColumn
			});
		}

		public static LocalizedString ExceptionQueueIdentityCompare(string type)
		{
			return new LocalizedString("ExceptionQueueIdentityCompare", "", false, false, DataStrings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString ErrorIncorrectWindowsLiveIdFormat(string id)
		{
			return new LocalizedString("ErrorIncorrectWindowsLiveIdFormat", "Ex5A2092", false, true, DataStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString InvalidIPRange(string startAddress, string endAddress)
		{
			return new LocalizedString("InvalidIPRange", "ExE4B7F5", false, true, DataStrings.ResourceManager, new object[]
			{
				startAddress,
				endAddress
			});
		}

		public static LocalizedString DeliveryTypeSmartHostConnectorDelivery
		{
			get
			{
				return new LocalizedString("DeliveryTypeSmartHostConnectorDelivery", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KindKeywordVoiceMail
		{
			get
			{
				return new LocalizedString("KindKeywordVoiceMail", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoutingNoRouteToMta
		{
			get
			{
				return new LocalizedString("RoutingNoRouteToMta", "Ex6528F8", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientAccessProtocolEWS
		{
			get
			{
				return new LocalizedString("ClientAccessProtocolEWS", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionInvalidMeumAddress(string address)
		{
			return new LocalizedString("ExceptionInvalidMeumAddress", "Ex11F8EB", false, true, DataStrings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString MailRecipientTypeExternal
		{
			get
			{
				return new LocalizedString("MailRecipientTypeExternal", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeliveryTypeSmtpRelayWithinAdSite
		{
			get
			{
				return new LocalizedString("DeliveryTypeSmtpRelayWithinAdSite", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSerialNumberFormatError(string serialNumber)
		{
			return new LocalizedString("ErrorSerialNumberFormatError", "Ex9FC4FD", false, true, DataStrings.ResourceManager, new object[]
			{
				serialNumber
			});
		}

		public static LocalizedString InvalidDialGroupEntryCsvFormat
		{
			get
			{
				return new LocalizedString("InvalidDialGroupEntryCsvFormat", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionProtocolConnectionSettingsInvalidHostname(string settings)
		{
			return new LocalizedString("ExceptionProtocolConnectionSettingsInvalidHostname", "ExF95D91", false, true, DataStrings.ResourceManager, new object[]
			{
				settings
			});
		}

		public static LocalizedString ErrorCannotConvertToBinary(string error)
		{
			return new LocalizedString("ErrorCannotConvertToBinary", "ExBA027C", false, true, DataStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString KindKeywordMeetings
		{
			get
			{
				return new LocalizedString("KindKeywordMeetings", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomScheduleEveryFourHours
		{
			get
			{
				return new LocalizedString("CustomScheduleEveryFourHours", "ExCEB31E", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProxyAddressTemplateEmptyPrefixOrValue(string template)
		{
			return new LocalizedString("ProxyAddressTemplateEmptyPrefixOrValue", "ExAF1843", false, true, DataStrings.ResourceManager, new object[]
			{
				template
			});
		}

		public static LocalizedString InvalidAddressSpaceType(string addressType)
		{
			return new LocalizedString("InvalidAddressSpaceType", "Ex5D713F", false, true, DataStrings.ResourceManager, new object[]
			{
				addressType
			});
		}

		public static LocalizedString MailRecipientTypeUnknown
		{
			get
			{
				return new LocalizedString("MailRecipientTypeUnknown", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmbiguousRecipient
		{
			get
			{
				return new LocalizedString("AmbiguousRecipient", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SmtpReceiveCapabilitiesAllowConsumerMail
		{
			get
			{
				return new LocalizedString("SmtpReceiveCapabilitiesAllowConsumerMail", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorFileShareWitnessServerNameMustNotBeIP(string computerName)
		{
			return new LocalizedString("ErrorFileShareWitnessServerNameMustNotBeIP", "", false, false, DataStrings.ResourceManager, new object[]
			{
				computerName
			});
		}

		public static LocalizedString InvalidFlagValue
		{
			get
			{
				return new LocalizedString("InvalidFlagValue", "ExB5B5F5", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TlsAuthLevelDomainValidation
		{
			get
			{
				return new LocalizedString("TlsAuthLevelDomainValidation", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StarDomainNotAllowed(string property)
		{
			return new LocalizedString("StarDomainNotAllowed", "Ex2050B9", false, true, DataStrings.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString ClientAccessBasicAuthentication
		{
			get
			{
				return new LocalizedString("ClientAccessBasicAuthentication", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidTimeOfDayFormatCustomWorkingHours
		{
			get
			{
				return new LocalizedString("InvalidTimeOfDayFormatCustomWorkingHours", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AttachmentContent
		{
			get
			{
				return new LocalizedString("AttachmentContent", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorValueAlreadyPresent(string value)
		{
			return new LocalizedString("ErrorValueAlreadyPresent", "Ex0659B9", false, true, DataStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString InvalidKeyMappingTransferToGalContact
		{
			get
			{
				return new LocalizedString("InvalidKeyMappingTransferToGalContact", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProtocolSpx
		{
			get
			{
				return new LocalizedString("ProtocolSpx", "Ex6115B0", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNonGeneric(string typeName)
		{
			return new LocalizedString("ErrorNonGeneric", "ExC7B407", false, true, DataStrings.ResourceManager, new object[]
			{
				typeName
			});
		}

		public static LocalizedString ErrorPoliciesDowngradeCustomFailures
		{
			get
			{
				return new LocalizedString("ErrorPoliciesDowngradeCustomFailures", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Int32ParsableStringConstraintViolation(string value)
		{
			return new LocalizedString("Int32ParsableStringConstraintViolation", "Ex2BD81F", false, true, DataStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString ErrorCostOutOfRange
		{
			get
			{
				return new LocalizedString("ErrorCostOutOfRange", "Ex1B6704", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionRemoveEumPrimary(string primary)
		{
			return new LocalizedString("ExceptionRemoveEumPrimary", "Ex0EBE82", false, true, DataStrings.ResourceManager, new object[]
			{
				primary
			});
		}

		public static LocalizedString ExceptionNegativeUnit
		{
			get
			{
				return new LocalizedString("ExceptionNegativeUnit", "ExA4258B", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationIpRangeNotAllowed(string ipAddress, ulong maxAllowed)
		{
			return new LocalizedString("ConstraintViolationIpRangeNotAllowed", "", false, false, DataStrings.ResourceManager, new object[]
			{
				ipAddress,
				maxAllowed
			});
		}

		public static LocalizedString DeliveryTypeSmtpDeliveryToMailbox
		{
			get
			{
				return new LocalizedString("DeliveryTypeSmtpDeliveryToMailbox", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorOutOfRange(int min, int max)
		{
			return new LocalizedString("ErrorOutOfRange", "Ex3E498C", false, true, DataStrings.ResourceManager, new object[]
			{
				min,
				max
			});
		}

		public static LocalizedString PropertyIsMandatory
		{
			get
			{
				return new LocalizedString("PropertyIsMandatory", "ExF0DCB8", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoutingNonBHExpansionServer
		{
			get
			{
				return new LocalizedString("RoutingNonBHExpansionServer", "Ex2E6B0F", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionReadOnlyPropertyBag
		{
			get
			{
				return new LocalizedString("ExceptionReadOnlyPropertyBag", "Ex794ED6", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnreachableQueueNextHopDomain
		{
			get
			{
				return new LocalizedString("UnreachableQueueNextHopDomain", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSmtpDomain
		{
			get
			{
				return new LocalizedString("InvalidSmtpDomain", "ExA19A7C", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDialledNumberFormatC
		{
			get
			{
				return new LocalizedString("InvalidDialledNumberFormatC", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeliveryTypeDnsConnectorDelivery
		{
			get
			{
				return new LocalizedString("DeliveryTypeDnsConnectorDelivery", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DialGroupNotSpecifiedOnDialPlanB(string name, string group)
		{
			return new LocalizedString("DialGroupNotSpecifiedOnDialPlanB", "", false, false, DataStrings.ResourceManager, new object[]
			{
				name,
				group
			});
		}

		public static LocalizedString SharingPolicyDomainInvalidActionForDomain(string value)
		{
			return new LocalizedString("SharingPolicyDomainInvalidActionForDomain", "", false, false, DataStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString InvalidTlsCertificateName(string s)
		{
			return new LocalizedString("InvalidTlsCertificateName", "", false, false, DataStrings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString DigitStringPatternDescription
		{
			get
			{
				return new LocalizedString("DigitStringPatternDescription", "ExCF6A08", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomScheduleDailyFrom9AMTo5PMAtWeekDays
		{
			get
			{
				return new LocalizedString("CustomScheduleDailyFrom9AMTo5PMAtWeekDays", "Ex0697CA", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidKeyMappingContext
		{
			get
			{
				return new LocalizedString("InvalidKeyMappingContext", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomScheduleDaily1AM
		{
			get
			{
				return new LocalizedString("CustomScheduleDaily1AM", "Ex4A5C60", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FromInternet
		{
			get
			{
				return new LocalizedString("FromInternet", "Ex1D11EA", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInputOfScheduleMustExclusive(string exclusiveInput)
		{
			return new LocalizedString("ErrorInputOfScheduleMustExclusive", "ExE1CFCF", false, true, DataStrings.ResourceManager, new object[]
			{
				exclusiveInput
			});
		}

		public static LocalizedString ExceptionCannotSetDifferentType(Type propertyType, Type otherType)
		{
			return new LocalizedString("ExceptionCannotSetDifferentType", "Ex7AEA22", false, true, DataStrings.ResourceManager, new object[]
			{
				propertyType,
				otherType
			});
		}

		public static LocalizedString TotalCopiedItems
		{
			get
			{
				return new LocalizedString("TotalCopiedItems", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientAccessProtocolRPS
		{
			get
			{
				return new LocalizedString("ClientAccessProtocolRPS", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnknownOperation(string op, string adds, string removes)
		{
			return new LocalizedString("ErrorUnknownOperation", "ExE436F3", false, true, DataStrings.ResourceManager, new object[]
			{
				op,
				adds,
				removes
			});
		}

		public static LocalizedString ClientAccessNonBasicAuthentication
		{
			get
			{
				return new LocalizedString("ClientAccessNonBasicAuthentication", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionParseError(string invalidQuery, int position)
		{
			return new LocalizedString("ExceptionParseError", "Ex0D5420", false, true, DataStrings.ResourceManager, new object[]
			{
				invalidQuery,
				position
			});
		}

		public static LocalizedString LinkedPartnerGroupInformationInvalidParameter(string propertyValue)
		{
			return new LocalizedString("LinkedPartnerGroupInformationInvalidParameter", "Ex1CA2DB", false, true, DataStrings.ResourceManager, new object[]
			{
				propertyValue
			});
		}

		public static LocalizedString RoutingNoMdb
		{
			get
			{
				return new LocalizedString("RoutingNoMdb", "ExD1ECC6", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFolderPermissionRoleEditor
		{
			get
			{
				return new LocalizedString("PublicFolderPermissionRoleEditor", "Ex91F409", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SharingPolicyDomainInvalidActionForAnonymous(string value)
		{
			return new LocalizedString("SharingPolicyDomainInvalidActionForAnonymous", "ExF4E04C", false, true, DataStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString ConstraintViolationDontMatchUnit(string unit, string timeSpan)
		{
			return new LocalizedString("ConstraintViolationDontMatchUnit", "Ex3F6F4A", false, true, DataStrings.ResourceManager, new object[]
			{
				unit,
				timeSpan
			});
		}

		public static LocalizedString MailRecipientTypeMailbox
		{
			get
			{
				return new LocalizedString("MailRecipientTypeMailbox", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CopyErrors
		{
			get
			{
				return new LocalizedString("CopyErrors", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SnapinNameTooShort
		{
			get
			{
				return new LocalizedString("SnapinNameTooShort", "Ex86B3D3", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TextBody
		{
			get
			{
				return new LocalizedString("TextBody", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeliveryTypeSmtpRelayToTiRg
		{
			get
			{
				return new LocalizedString("DeliveryTypeSmtpRelayToTiRg", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientAccessRulesBlockedConnection(string ruleName)
		{
			return new LocalizedString("ClientAccessRulesBlockedConnection", "", false, false, DataStrings.ResourceManager, new object[]
			{
				ruleName
			});
		}

		public static LocalizedString ConstraintViolationNoLeadingOrTrailingWhitespace
		{
			get
			{
				return new LocalizedString("ConstraintViolationNoLeadingOrTrailingWhitespace", "Ex9E6D8B", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarSharingFreeBusyDetail
		{
			get
			{
				return new LocalizedString("CalendarSharingFreeBusyDetail", "ExAB93FA", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorLengthOfExDataTimeByteArray(int length)
		{
			return new LocalizedString("ErrorLengthOfExDataTimeByteArray", "Ex15DC70", false, true, DataStrings.ResourceManager, new object[]
			{
				length
			});
		}

		public static LocalizedString ConfigurationSettingsScopePropertyNotFound2(string name, string knownScopes)
		{
			return new LocalizedString("ConfigurationSettingsScopePropertyNotFound2", "", false, false, DataStrings.ResourceManager, new object[]
			{
				name,
				knownScopes
			});
		}

		public static LocalizedString ItemClass
		{
			get
			{
				return new LocalizedString("ItemClass", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PropertyName(string propertyName)
		{
			return new LocalizedString("PropertyName", "Ex9C3C13", false, true, DataStrings.ResourceManager, new object[]
			{
				propertyName
			});
		}

		public static LocalizedString InvalidNotationFormat
		{
			get
			{
				return new LocalizedString("InvalidNotationFormat", "ExEA7943", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientAccessProtocolOAB
		{
			get
			{
				return new LocalizedString("ClientAccessProtocolOAB", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidCharInString(string s, string c)
		{
			return new LocalizedString("InvalidCharInString", "", false, false, DataStrings.ResourceManager, new object[]
			{
				s,
				c
			});
		}

		public static LocalizedString InvalidCallerIdItemTypePhoneNumber
		{
			get
			{
				return new LocalizedString("InvalidCallerIdItemTypePhoneNumber", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionDurationOverflow
		{
			get
			{
				return new LocalizedString("ExceptionDurationOverflow", "ExCD0088", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeLegacyServers
		{
			get
			{
				return new LocalizedString("ExchangeLegacyServers", "ExECFE38", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Down
		{
			get
			{
				return new LocalizedString("Down", "Ex455BE6", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidX400AddressSpace(string s)
		{
			return new LocalizedString("InvalidX400AddressSpace", "Ex8015D9", false, true, DataStrings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString InvalidDialledNumberFormatA
		{
			get
			{
				return new LocalizedString("InvalidDialledNumberFormatA", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationStringLengthTooShort(int minLength, int actualLength)
		{
			return new LocalizedString("ConstraintViolationStringLengthTooShort", "ExF2B74F", false, true, DataStrings.ResourceManager, new object[]
			{
				minLength,
				actualLength
			});
		}

		public static LocalizedString Misconfigured
		{
			get
			{
				return new LocalizedString("Misconfigured", "ExBB5420", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProtocolTcpIP
		{
			get
			{
				return new LocalizedString("ProtocolTcpIP", "Ex69620F", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidIPAddressOrHostNameInSmartHost(string s)
		{
			return new LocalizedString("InvalidIPAddressOrHostNameInSmartHost", "Ex6F5D30", false, true, DataStrings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString SmtpResponseConstraintViolation(string property, string value)
		{
			return new LocalizedString("SmtpResponseConstraintViolation", "ExF50434", false, true, DataStrings.ResourceManager, new object[]
			{
				property,
				value
			});
		}

		public static LocalizedString ExceptionEndPointMissingSeparator(string ipBinding)
		{
			return new LocalizedString("ExceptionEndPointMissingSeparator", "Ex008998", false, true, DataStrings.ResourceManager, new object[]
			{
				ipBinding
			});
		}

		public static LocalizedString RoleEntryStringMustBeCommaSeparated
		{
			get
			{
				return new LocalizedString("RoleEntryStringMustBeCommaSeparated", "ExBE931F", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SmtpReceiveCapabilitiesAcceptOorgProtocol
		{
			get
			{
				return new LocalizedString("SmtpReceiveCapabilitiesAcceptOorgProtocol", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionFormatInvalid(string input)
		{
			return new LocalizedString("ExceptionFormatInvalid", "Ex5B47E7", false, true, DataStrings.ResourceManager, new object[]
			{
				input
			});
		}

		public static LocalizedString IncludeExcludeInvalid(string value)
		{
			return new LocalizedString("IncludeExcludeInvalid", "", false, false, DataStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString WeekendDays
		{
			get
			{
				return new LocalizedString("WeekendDays", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProtocolNetBios
		{
			get
			{
				return new LocalizedString("ProtocolNetBios", "Ex9D59FF", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomScheduleDailyFrom8AMTo12PMAnd1PMTo5PMAtWeekDays
		{
			get
			{
				return new LocalizedString("CustomScheduleDailyFrom8AMTo12PMAnd1PMTo5PMAtWeekDays", "Ex5F5396", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfigurationSettingsAppSettingsError
		{
			get
			{
				return new LocalizedString("ConfigurationSettingsAppSettingsError", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidKeyMappingTransferToNumber
		{
			get
			{
				return new LocalizedString("InvalidKeyMappingTransferToNumber", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomScheduleDaily5AM
		{
			get
			{
				return new LocalizedString("CustomScheduleDaily5AM", "Ex6D671C", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KindKeywordRssFeeds
		{
			get
			{
				return new LocalizedString("KindKeywordRssFeeds", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotConvertNull(string type)
		{
			return new LocalizedString("ErrorCannotConvertNull", "ExB15E10", false, true, DataStrings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString InvalidScopedAddressSpace(string s)
		{
			return new LocalizedString("InvalidScopedAddressSpace", "ExB0B976", false, true, DataStrings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString MessageStatusSuspended
		{
			get
			{
				return new LocalizedString("MessageStatusSuspended", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShadowMessagePreferenceLocalOnly
		{
			get
			{
				return new LocalizedString("ShadowMessagePreferenceLocalOnly", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Unavailable
		{
			get
			{
				return new LocalizedString("Unavailable", "ExADA259", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorConversionFailedWithException(string value, string originalType, string resultType, Exception inner)
		{
			return new LocalizedString("ErrorConversionFailedWithException", "ExFC3A2E", false, true, DataStrings.ResourceManager, new object[]
			{
				value,
				originalType,
				resultType,
				inner
			});
		}

		public static LocalizedString DuplicateParameterException(string paramName)
		{
			return new LocalizedString("DuplicateParameterException", "ExBA0CA8", false, true, DataStrings.ResourceManager, new object[]
			{
				paramName
			});
		}

		public static LocalizedString RejectStatusCode
		{
			get
			{
				return new LocalizedString("RejectStatusCode", "Ex336AFD", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionInvalidEumAddress(string address)
		{
			return new LocalizedString("ExceptionInvalidEumAddress", "Ex1255BC", false, true, DataStrings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString Thursday
		{
			get
			{
				return new LocalizedString("Thursday", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DuplicatedColumn(string duplicatedColumn)
		{
			return new LocalizedString("DuplicatedColumn", "ExE8A76C", false, true, DataStrings.ResourceManager, new object[]
			{
				duplicatedColumn
			});
		}

		public static LocalizedString StartingAddressAndMaskAddressFamilyMismatch
		{
			get
			{
				return new LocalizedString("StartingAddressAndMaskAddressFamilyMismatch", "Ex55EF2D", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidCallerIdItemTypeDefaultContactsFolder
		{
			get
			{
				return new LocalizedString("InvalidCallerIdItemTypeDefaultContactsFolder", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPoliciesDefault
		{
			get
			{
				return new LocalizedString("ErrorPoliciesDefault", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationValueIsDisallowed(string invalidValues, string input)
		{
			return new LocalizedString("ConstraintViolationValueIsDisallowed", "Ex75F989", false, true, DataStrings.ResourceManager, new object[]
			{
				invalidValues,
				input
			});
		}

		public static LocalizedString PublicFolderPermissionRoleContributor
		{
			get
			{
				return new LocalizedString("PublicFolderPermissionRoleContributor", "ExD5FF81", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Weekdays
		{
			get
			{
				return new LocalizedString("Weekdays", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SmtpReceiveCapabilitiesAllowSubmit
		{
			get
			{
				return new LocalizedString("SmtpReceiveCapabilitiesAllowSubmit", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionEndPointInvalidPort(string ipBinding)
		{
			return new LocalizedString("ExceptionEndPointInvalidPort", "Ex9F2388", false, true, DataStrings.ResourceManager, new object[]
			{
				ipBinding
			});
		}

		public static LocalizedString PropertyNotEmptyOrNull
		{
			get
			{
				return new LocalizedString("PropertyNotEmptyOrNull", "Ex9D869A", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReadOnlyCauseObject(string name)
		{
			return new LocalizedString("ErrorReadOnlyCauseObject", "ExF07C01", false, true, DataStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ErrorCannotConvertFromString(string value, string resultType, string error)
		{
			return new LocalizedString("ErrorCannotConvertFromString", "Ex57F8F7", false, true, DataStrings.ResourceManager, new object[]
			{
				value,
				resultType,
				error
			});
		}

		public static LocalizedString MAPIBlockOutlookVersionsPatternDescription
		{
			get
			{
				return new LocalizedString("MAPIBlockOutlookVersionsPatternDescription", "ExD18D66", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BucketVersionPatternDescription
		{
			get
			{
				return new LocalizedString("BucketVersionPatternDescription", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageStatusNone
		{
			get
			{
				return new LocalizedString("MessageStatusNone", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFolderPermissionRoleAuthor
		{
			get
			{
				return new LocalizedString("PublicFolderPermissionRoleAuthor", "Ex4E0FFA", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeliveryTypeUndefined
		{
			get
			{
				return new LocalizedString("DeliveryTypeUndefined", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SmtpReceiveCapabilitiesAcceptCrossForestMail
		{
			get
			{
				return new LocalizedString("SmtpReceiveCapabilitiesAcceptCrossForestMail", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectorSourceMigrated
		{
			get
			{
				return new LocalizedString("ConnectorSourceMigrated", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoutingNoMatchingConnector
		{
			get
			{
				return new LocalizedString("RoutingNoMatchingConnector", "Ex8457BA", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QueueStatusNone
		{
			get
			{
				return new LocalizedString("QueueStatusNone", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientAccessProtocolEAC
		{
			get
			{
				return new LocalizedString("ClientAccessProtocolEAC", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EncryptionTypeTLS
		{
			get
			{
				return new LocalizedString("EncryptionTypeTLS", "Ex25776E", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoleEntryNameTooShort
		{
			get
			{
				return new LocalizedString("RoleEntryNameTooShort", "ExC54CD3", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DuplicatesRemoved
		{
			get
			{
				return new LocalizedString("DuplicatesRemoved", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageStatusPendingSuspend
		{
			get
			{
				return new LocalizedString("MessageStatusPendingSuspend", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintNoTrailingSpecificCharacter(string input, char invalidChar)
		{
			return new LocalizedString("ConstraintNoTrailingSpecificCharacter", "Ex8A4B03", false, true, DataStrings.ResourceManager, new object[]
			{
				input,
				invalidChar
			});
		}

		public static LocalizedString DependencyCheckFailed(string feature, string featureValue, string dependencyFeatureName, string dependencyFeatureValue)
		{
			return new LocalizedString("DependencyCheckFailed", "Ex74B1D5", false, true, DataStrings.ResourceManager, new object[]
			{
				feature,
				featureValue,
				dependencyFeatureName,
				dependencyFeatureValue
			});
		}

		public static LocalizedString SmtpReceiveCapabilitiesAcceptOrgHeaders
		{
			get
			{
				return new LocalizedString("SmtpReceiveCapabilitiesAcceptOrgHeaders", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomScheduleDailyAtMidnight
		{
			get
			{
				return new LocalizedString("CustomScheduleDailyAtMidnight", "ExBF07EE", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecipientStatusComplete
		{
			get
			{
				return new LocalizedString("RecipientStatusComplete", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidCallerIdItemTypeGALContactr
		{
			get
			{
				return new LocalizedString("InvalidCallerIdItemTypeGALContactr", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeServers
		{
			get
			{
				return new LocalizedString("ExchangeServers", "ExBCB94C", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ScheduleModeNever
		{
			get
			{
				return new LocalizedString("ScheduleModeNever", "Ex78E645", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfigurationSettingsPropertyNotFound(string name)
		{
			return new LocalizedString("ConfigurationSettingsPropertyNotFound", "", false, false, DataStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString CustomScheduleDailyFrom11PMTo3AM
		{
			get
			{
				return new LocalizedString("CustomScheduleDailyFrom11PMTo3AM", "Ex44DB2D", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDialledNumberFormatD
		{
			get
			{
				return new LocalizedString("InvalidDialledNumberFormatD", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotConvertToString(string error)
		{
			return new LocalizedString("ErrorCannotConvertToString", "ExFD6919", false, true, DataStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString ServicePlanSchemaCheckFailed(string schemaError)
		{
			return new LocalizedString("ServicePlanSchemaCheckFailed", "Ex09C509", false, true, DataStrings.ResourceManager, new object[]
			{
				schemaError
			});
		}

		public static LocalizedString CustomScheduleDailyFrom1AMTo5AM
		{
			get
			{
				return new LocalizedString("CustomScheduleDailyFrom1AMTo5AM", "Ex864B55", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProxyAddressPrefixShouldNotBeAllSpace
		{
			get
			{
				return new LocalizedString("ProxyAddressPrefixShouldNotBeAllSpace", "Ex56DEBF", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ApplicationPermissionRoleEntryParameterNotEmptyException(string name)
		{
			return new LocalizedString("ApplicationPermissionRoleEntryParameterNotEmptyException", "ExC82900", false, true, DataStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString CustomScheduleDaily2AM
		{
			get
			{
				return new LocalizedString("CustomScheduleDaily2AM", "Ex69544A", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SharingPolicyDomainInvalid(string value)
		{
			return new LocalizedString("SharingPolicyDomainInvalid", "Ex693DF9", false, true, DataStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString InvalidKeyMappingFindMeSecondNumber
		{
			get
			{
				return new LocalizedString("InvalidKeyMappingFindMeSecondNumber", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionValueOverflow(string minValue, string maxValue, string value)
		{
			return new LocalizedString("ExceptionValueOverflow", "Ex59AFCB", false, true, DataStrings.ResourceManager, new object[]
			{
				minValue,
				maxValue,
				value
			});
		}

		public static LocalizedString MessageStatusReady
		{
			get
			{
				return new LocalizedString("MessageStatusReady", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupByDay
		{
			get
			{
				return new LocalizedString("GroupByDay", "ExD5BDA4", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeliveryTypeHeartbeat
		{
			get
			{
				return new LocalizedString("DeliveryTypeHeartbeat", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorToBinaryNotImplemented(string sourceType)
		{
			return new LocalizedString("ErrorToBinaryNotImplemented", "Ex5E8F74", false, true, DataStrings.ResourceManager, new object[]
			{
				sourceType
			});
		}

		public static LocalizedString BadEnumValue(Type enumType)
		{
			return new LocalizedString("BadEnumValue", "", false, false, DataStrings.ResourceManager, new object[]
			{
				enumType
			});
		}

		public static LocalizedString ExceptionInvalidServerName(string server)
		{
			return new LocalizedString("ExceptionInvalidServerName", "", false, false, DataStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString ExceptionInvalidSmtpAddress(string address)
		{
			return new LocalizedString("ExceptionInvalidSmtpAddress", "Ex7573F6", false, true, DataStrings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString FileExtensionOrSplatPatternDescription
		{
			get
			{
				return new LocalizedString("FileExtensionOrSplatPatternDescription", "ExADC20C", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageStatusActive
		{
			get
			{
				return new LocalizedString("MessageStatusActive", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Monday
		{
			get
			{
				return new LocalizedString("Monday", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeferReasonTransientAcceptedDomainsLoadFailure
		{
			get
			{
				return new LocalizedString("DeferReasonTransientAcceptedDomainsLoadFailure", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationStringContainsInvalidCharacters(string invalidCharacters, string input)
		{
			return new LocalizedString("ConstraintViolationStringContainsInvalidCharacters", "Ex15340E", false, true, DataStrings.ResourceManager, new object[]
			{
				invalidCharacters,
				input
			});
		}

		public static LocalizedString ExceptionEventSourceNull
		{
			get
			{
				return new LocalizedString("ExceptionEventSourceNull", "Ex6D5B28", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFolderPermissionRoleNonEditingAuthor
		{
			get
			{
				return new LocalizedString("PublicFolderPermissionRoleNonEditingAuthor", "Ex046A9A", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidIntRangeArgument(string argument)
		{
			return new LocalizedString("InvalidIntRangeArgument", "", false, false, DataStrings.ResourceManager, new object[]
			{
				argument
			});
		}

		public static LocalizedString CustomScheduleEveryTwoHours
		{
			get
			{
				return new LocalizedString("CustomScheduleEveryTwoHours", "Ex14ED89", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WrongNumberOfColumns(int rowNumber, int expectedColumnCount, int actualColumnCount)
		{
			return new LocalizedString("WrongNumberOfColumns", "Ex2C49BB", false, true, DataStrings.ResourceManager, new object[]
			{
				rowNumber,
				expectedColumnCount,
				actualColumnCount
			});
		}

		public static LocalizedString InvalidCallerIdItemTypePersonaContact
		{
			get
			{
				return new LocalizedString("InvalidCallerIdItemTypePersonaContact", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeferReasonConfigUpdate
		{
			get
			{
				return new LocalizedString("DeferReasonConfigUpdate", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationValueIsNullOrEmpty
		{
			get
			{
				return new LocalizedString("ConstraintViolationValueIsNullOrEmpty", "ExB7847B", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubjectProperty
		{
			get
			{
				return new LocalizedString("SubjectProperty", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FilterOnlyAttributes(string attributeName)
		{
			return new LocalizedString("FilterOnlyAttributes", "Ex125B9D", false, true, DataStrings.ResourceManager, new object[]
			{
				attributeName
			});
		}

		public static LocalizedString InvalidAlternateMailboxString(string blob, char separator)
		{
			return new LocalizedString("InvalidAlternateMailboxString", "Ex9ED82F", false, true, DataStrings.ResourceManager, new object[]
			{
				blob,
				separator
			});
		}

		public static LocalizedString InvalidKeySelection_Zero
		{
			get
			{
				return new LocalizedString("InvalidKeySelection_Zero", "ExFBBC38", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QueueStatusReady
		{
			get
			{
				return new LocalizedString("QueueStatusReady", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProtocolNamedPipes
		{
			get
			{
				return new LocalizedString("ProtocolNamedPipes", "Ex19D08D", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationValueOutOfRange(string minValue, string maxValue, string actualValue)
		{
			return new LocalizedString("ConstraintViolationValueOutOfRange", "Ex503186", false, true, DataStrings.ResourceManager, new object[]
			{
				minValue,
				maxValue,
				actualValue
			});
		}

		public static LocalizedString ExceptionInvalidLongitude(double lon)
		{
			return new LocalizedString("ExceptionInvalidLongitude", "", false, false, DataStrings.ResourceManager, new object[]
			{
				lon
			});
		}

		public static LocalizedString ExceptionGeoCoordinatesWithWrongFormat(string geoCoordinates)
		{
			return new LocalizedString("ExceptionGeoCoordinatesWithWrongFormat", "", false, false, DataStrings.ResourceManager, new object[]
			{
				geoCoordinates
			});
		}

		public static LocalizedString RoleEntryNameInvalidException(string name)
		{
			return new LocalizedString("RoleEntryNameInvalidException", "ExE1DB37", false, true, DataStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ConstraintViolationStringLengthTooLong(int maxLength, int actualLength)
		{
			return new LocalizedString("ConstraintViolationStringLengthTooLong", "ExA75D27", false, true, DataStrings.ResourceManager, new object[]
			{
				maxLength,
				actualLength
			});
		}

		public static LocalizedString ProtocolVnsSpp
		{
			get
			{
				return new LocalizedString("ProtocolVnsSpp", "Ex85E948", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionUnsupportedSourceType(object obj, Type type)
		{
			return new LocalizedString("ExceptionUnsupportedSourceType", "Ex4086F4", false, true, DataStrings.ResourceManager, new object[]
			{
				obj,
				type
			});
		}

		public static LocalizedString InvalidRoleEntryType(string entryType)
		{
			return new LocalizedString("InvalidRoleEntryType", "ExA26E16", false, true, DataStrings.ResourceManager, new object[]
			{
				entryType
			});
		}

		public static LocalizedString ConfigurationSettingsPropertyBadType(string name, string type)
		{
			return new LocalizedString("ConfigurationSettingsPropertyBadType", "", false, false, DataStrings.ResourceManager, new object[]
			{
				name,
				type
			});
		}

		public static LocalizedString KindKeywordNotes
		{
			get
			{
				return new LocalizedString("KindKeywordNotes", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSmtpDomainName(string address)
		{
			return new LocalizedString("InvalidSmtpDomainName", "Ex41D592", false, true, DataStrings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString ErrorCannotConvert
		{
			get
			{
				return new LocalizedString("ErrorCannotConvert", "Ex9D3C98", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDialledNumberFormatB
		{
			get
			{
				return new LocalizedString("InvalidDialledNumberFormatB", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDialGroupEntryFormat
		{
			get
			{
				return new LocalizedString("InvalidDialGroupEntryFormat", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EapMustHaveOneEnabledPrimarySmtpAddressTemplate
		{
			get
			{
				return new LocalizedString("EapMustHaveOneEnabledPrimarySmtpAddressTemplate", "Ex77737B", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FileExtensionPatternDescription
		{
			get
			{
				return new LocalizedString("FileExtensionPatternDescription", "Ex543745", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TooManyRows(int maximumRowCount)
		{
			return new LocalizedString("TooManyRows", "Ex6D5ECE", false, true, DataStrings.ResourceManager, new object[]
			{
				maximumRowCount
			});
		}

		public static LocalizedString Unknown
		{
			get
			{
				return new LocalizedString("Unknown", "Ex54E959", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QueueStatusRetry
		{
			get
			{
				return new LocalizedString("QueueStatusRetry", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidHostname(string s)
		{
			return new LocalizedString("InvalidHostname", "ExFD9758", false, true, DataStrings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString Wednesday
		{
			get
			{
				return new LocalizedString("Wednesday", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidKeyMappingFindMe
		{
			get
			{
				return new LocalizedString("InvalidKeyMappingFindMe", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeferReasonReroutedByStoreDriver
		{
			get
			{
				return new LocalizedString("DeferReasonReroutedByStoreDriver", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidX509IdentifierFormat(string value)
		{
			return new LocalizedString("InvalidX509IdentifierFormat", "Ex2F4975", false, true, DataStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString ScheduleModeAlways
		{
			get
			{
				return new LocalizedString("ScheduleModeAlways", "ExAB9272", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationStringIsNotValidCultureInfo(string value)
		{
			return new LocalizedString("ConstraintViolationStringIsNotValidCultureInfo", "Ex96BDA9", false, true, DataStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString ErrorContainsOutOfRange(string value)
		{
			return new LocalizedString("ErrorContainsOutOfRange", "Ex7CB832", false, true, DataStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString ErrorConversionFailed(ProviderPropertyDefinition property, object value)
		{
			return new LocalizedString("ErrorConversionFailed", "ExF99E12", false, true, DataStrings.ResourceManager, new object[]
			{
				property,
				value
			});
		}

		public static LocalizedString ExceptionInvalidFilterOperator(string op, string invalidQuery, int position)
		{
			return new LocalizedString("ExceptionInvalidFilterOperator", "ExB87763", false, true, DataStrings.ResourceManager, new object[]
			{
				op,
				invalidQuery,
				position
			});
		}

		public static LocalizedString FileIsEmpty
		{
			get
			{
				return new LocalizedString("FileIsEmpty", "Ex4E4545", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionSerializationDataAbsent
		{
			get
			{
				return new LocalizedString("ExceptionSerializationDataAbsent", "ExC24362", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SmtpReceiveCapabilitiesAcceptOorgHeader
		{
			get
			{
				return new LocalizedString("SmtpReceiveCapabilitiesAcceptOorgHeader", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DsnText
		{
			get
			{
				return new LocalizedString("DsnText", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomScheduleDailyFrom9AMTo12PMAnd1PMTo6PMAtWeekDays
		{
			get
			{
				return new LocalizedString("CustomScheduleDailyFrom9AMTo12PMAnd1PMTo6PMAtWeekDays", "Ex7CC99C", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StorageTransientFailureDuringContentConversion
		{
			get
			{
				return new LocalizedString("StorageTransientFailureDuringContentConversion", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidScheduleType(string typeName)
		{
			return new LocalizedString("ErrorInvalidScheduleType", "Ex60837A", false, true, DataStrings.ResourceManager, new object[]
			{
				typeName
			});
		}

		public static LocalizedString MessageStatusRetry
		{
			get
			{
				return new LocalizedString("MessageStatusRetry", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomProxyAddressPrefixDisplayName
		{
			get
			{
				return new LocalizedString("CustomProxyAddressPrefixDisplayName", "Ex7F31AD", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CLIDPatternDescription
		{
			get
			{
				return new LocalizedString("CLIDPatternDescription", "Ex667AA2", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientAccessProtocolEAS
		{
			get
			{
				return new LocalizedString("ClientAccessProtocolEAS", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDSNCultureInput(string cultureName)
		{
			return new LocalizedString("ErrorDSNCultureInput", "ExA6BD51", false, true, DataStrings.ResourceManager, new object[]
			{
				cultureName
			});
		}

		public static LocalizedString ExceptionEmptyPrefixEum(string address)
		{
			return new LocalizedString("ExceptionEmptyPrefixEum", "Ex4B2C15", false, true, DataStrings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString ConstraintViolationInvalidWindowsLiveIDLocalPart
		{
			get
			{
				return new LocalizedString("ConstraintViolationInvalidWindowsLiveIDLocalPart", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionParseInternalMessageId
		{
			get
			{
				return new LocalizedString("ExceptionParseInternalMessageId", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecipientStatusReady
		{
			get
			{
				return new LocalizedString("RecipientStatusReady", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReceiveNone
		{
			get
			{
				return new LocalizedString("ReceiveNone", "ExBE515B", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReadOnlyCauseProperty(string name)
		{
			return new LocalizedString("ErrorReadOnlyCauseProperty", "Ex5CB650", false, true, DataStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ErrorOrignalMultiValuedProperty(string name)
		{
			return new LocalizedString("ErrorOrignalMultiValuedProperty", "ExBAE406", false, true, DataStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString UserPrincipalNamePatternDescription
		{
			get
			{
				return new LocalizedString("UserPrincipalNamePatternDescription", "Ex74BF44", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ToPartner
		{
			get
			{
				return new LocalizedString("ToPartner", "ExAFACB1", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeliveryTypeSmtpRelayToDag
		{
			get
			{
				return new LocalizedString("DeliveryTypeSmtpRelayToDag", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidIPAddressFormat(string ipAddress)
		{
			return new LocalizedString("InvalidIPAddressFormat", "ExD5D51B", false, true, DataStrings.ResourceManager, new object[]
			{
				ipAddress
			});
		}

		public static LocalizedString ExceptionEventSourceNotFound(string eventSource)
		{
			return new LocalizedString("ExceptionEventSourceNotFound", "ExFFF932", false, true, DataStrings.ResourceManager, new object[]
			{
				eventSource
			});
		}

		public static LocalizedString DeliveryTypeSmtpRelayWithinAdSiteToEdge
		{
			get
			{
				return new LocalizedString("DeliveryTypeSmtpRelayWithinAdSiteToEdge", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidNumberFormat(string value)
		{
			return new LocalizedString("InvalidNumberFormat", "", false, false, DataStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString DeferReasonRecipientThreadLimitExceeded
		{
			get
			{
				return new LocalizedString("DeferReasonRecipientThreadLimitExceeded", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidIPRangeFormat(string s)
		{
			return new LocalizedString("InvalidIPRangeFormat", "Ex549848", false, true, DataStrings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString Up
		{
			get
			{
				return new LocalizedString("Up", "Ex9D354C", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PreserveDSNBody
		{
			get
			{
				return new LocalizedString("PreserveDSNBody", "Ex4BE49F", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ElcScheduleInsufficientGap
		{
			get
			{
				return new LocalizedString("ElcScheduleInsufficientGap", "Ex2F9187", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionTypeNotEnhancedTimeSpanOrTimeSpan
		{
			get
			{
				return new LocalizedString("ExceptionTypeNotEnhancedTimeSpanOrTimeSpan", "Ex902DFC", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationNotValidDomain(string domain)
		{
			return new LocalizedString("ConstraintViolationNotValidDomain", "Ex01B62F", false, true, DataStrings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString Recipients
		{
			get
			{
				return new LocalizedString("Recipients", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomScheduleDaily4AM
		{
			get
			{
				return new LocalizedString("CustomScheduleDaily4AM", "Ex2B23AD", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdminAuditLogInvalidParameterOrModifiedProperty(string propertyValue)
		{
			return new LocalizedString("AdminAuditLogInvalidParameterOrModifiedProperty", "Ex174688", false, true, DataStrings.ResourceManager, new object[]
			{
				propertyValue
			});
		}

		public static LocalizedString ConfigurationSettingsPropertyFailedValidation(string name, string value)
		{
			return new LocalizedString("ConfigurationSettingsPropertyFailedValidation", "", false, false, DataStrings.ResourceManager, new object[]
			{
				name,
				value
			});
		}

		public static LocalizedString ExceptionDefaultTypeMismatch
		{
			get
			{
				return new LocalizedString("ExceptionDefaultTypeMismatch", "ExFB6B13", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WebServiceRoleEntryParameterNotEmptyException(string name)
		{
			return new LocalizedString("WebServiceRoleEntryParameterNotEmptyException", "", false, false, DataStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ClientAccessProtocolOWA
		{
			get
			{
				return new LocalizedString("ClientAccessProtocolOWA", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationStringDoesNotContainNonWhitespaceCharacter(string input)
		{
			return new LocalizedString("ConstraintViolationStringDoesNotContainNonWhitespaceCharacter", "ExCC54B0", false, true, DataStrings.ResourceManager, new object[]
			{
				input
			});
		}

		public static LocalizedString ExceptionUnsupportedDataFormat(object data)
		{
			return new LocalizedString("ExceptionUnsupportedDataFormat", "Ex30771D", false, true, DataStrings.ResourceManager, new object[]
			{
				data
			});
		}

		public static LocalizedString RecipientStatusNone
		{
			get
			{
				return new LocalizedString("RecipientStatusNone", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProhibitedColumnPresent(string prohibitedColumn)
		{
			return new LocalizedString("ProhibitedColumnPresent", "Ex4D5119", false, true, DataStrings.ResourceManager, new object[]
			{
				prohibitedColumn
			});
		}

		public static LocalizedString StandardTrialEdition
		{
			get
			{
				return new LocalizedString("StandardTrialEdition", "Ex200B65", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShadowMessagePreferenceRemoteOnly
		{
			get
			{
				return new LocalizedString("ShadowMessagePreferenceRemoteOnly", "", false, false, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConstraintViolationStringDoesContainsNonASCIICharacter(string input)
		{
			return new LocalizedString("ConstraintViolationStringDoesContainsNonASCIICharacter", "Ex15BD8B", false, true, DataStrings.ResourceManager, new object[]
			{
				input
			});
		}

		public static LocalizedString ExceptionEmptyPrefix(string address)
		{
			return new LocalizedString("ExceptionEmptyPrefix", "Ex54A3BD", false, true, DataStrings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString DoNotConvert
		{
			get
			{
				return new LocalizedString("DoNotConvert", "ExC2DDEE", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMvpNotImplemented(string type, string propName)
		{
			return new LocalizedString("ErrorMvpNotImplemented", "ExAF1C63", false, true, DataStrings.ResourceManager, new object[]
			{
				type,
				propName
			});
		}

		public static LocalizedString CustomScheduleDaily10PM
		{
			get
			{
				return new LocalizedString("CustomScheduleDaily10PM", "ExCB21D8", false, true, DataStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SnapinNameInvalidCharException(string name)
		{
			return new LocalizedString("SnapinNameInvalidCharException", "Ex9CFED5", false, true, DataStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ExceptionEndPointInvalidIPAddress(string ipBinding)
		{
			return new LocalizedString("ExceptionEndPointInvalidIPAddress", "Ex07E0D9", false, true, DataStrings.ResourceManager, new object[]
			{
				ipBinding
			});
		}

		public static LocalizedString GetLocalizedString(DataStrings.IDs key)
		{
			return new LocalizedString(DataStrings.stringIDs[(uint)key], DataStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(391);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Data.DataStrings", typeof(DataStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			NotesProxyAddressPrefixDisplayName = 401351504U,
			InvalidCallerIdItemTypePersonalContact = 2802501868U,
			ExceptionParseNotSupported = 1831343041U,
			InvalidKeySelectionA = 2207492931U,
			ConnectorTypePartner = 2003039265U,
			SclValue = 214463947U,
			MessageStatusLocked = 343989129U,
			DaysOfWeek_None = 2044618123U,
			InvalidAddressSpaceTypeNullOrEmpty = 3268914636U,
			ProxyAddressShouldNotBeAllSpace = 1025341760U,
			CalendarSharingFreeBusyReviewer = 3216786356U,
			ProtocolLocalRpc = 2296262216U,
			InsufficientSpace = 3662889629U,
			ExchangeUsers = 1357933441U,
			RoutingIncompatibleDeliveryDomain = 4099951545U,
			EnterpriseTrialEdition = 4134731995U,
			InvalidNumberFormatString = 315555574U,
			Partners = 1448686489U,
			CustomScheduleDaily12PM = 3119708545U,
			CoexistenceTrialEdition = 2191591450U,
			CustomExtensionInvalidArgument = 3866345678U,
			NonWorkingHours = 955250317U,
			ClientAccessProtocolIMAP4 = 801480496U,
			ShadowMessagePreferencePreferRemote = 319956112U,
			CustomScheduleDailyFromMidnightTo4AM = 842516494U,
			CustomScheduleDailyFrom8AMTo5PMAtWeekDays = 2432881730U,
			EventLogText = 3910386293U,
			ClientAccessAdfsAuthentication = 2532083337U,
			ErrorADFormatError = 3646660508U,
			MeetingFullDetailsWithAttendees = 4100370227U,
			CustomScheduleDaily3AM = 2139945176U,
			ExceptionCalculatedDependsOnCalculated = 1571847379U,
			RoutingNoRouteToMdb = 4153254550U,
			SmtpReceiveCapabilitiesAcceptProxyProtocol = 3267940029U,
			DeferReasonADTransientFailureDuringContentConversion = 554923823U,
			HeaderPromotionModeMayCreate = 3857745828U,
			ConnectorSourceHybridWizard = 2392738789U,
			DeferReasonAgent = 2680336401U,
			InvalidCustomMenuKeyMappingA = 86136247U,
			InvalidResourcePropertySyntax = 352888273U,
			SmtpReceiveCapabilitiesAcceptCloudServicesMail = 651446229U,
			CcMailProxyAddressPrefixDisplayName = 830368596U,
			CustomScheduleEveryHour = 1420072811U,
			DeferReasonConcurrencyLimitInStoreDriver = 255146954U,
			GroupByMonth = 1322244886U,
			ContactsSharing = 906828259U,
			ExceptionUnsupportedNetworkProtocol = 2020388072U,
			SmtpReceiveCapabilitiesAcceptXAttrProtocol = 452471808U,
			ExceptionInvlidCharInProtocolName = 2998256021U,
			TransientFailure = 1284528402U,
			InvalidInputErrorMsg = 2446644924U,
			ErrorFileShareWitnessServerNameMustNotBeEmpty = 242132428U,
			HostnameTooLong = 2360034689U,
			InvalidKeyMappingVoiceMail = 118693033U,
			KindKeywordEmail = 2600270045U,
			ConstraintViolationStringLengthIsEmpty = 3423705850U,
			AnonymousUsers = 1737591621U,
			ErrorQuotionMarkNotSupportedInKql = 2996964666U,
			LatAmSpanish = 2637923947U,
			SearchRecipientsCc = 1831009676U,
			InvalidOperationCurrentProperty = 1181135370U,
			ErrorPoliciesUpgradeCustomFailures = 665138238U,
			CustomScheduleEveryHalfHour = 643679508U,
			EmptyExchangeObjectVersion = 4198186357U,
			CustomGreetingFilePatternDescription = 2760883904U,
			Partitioned = 1485002803U,
			ClientAccessProtocolOA = 220124761U,
			PublicFolderPermissionRolePublishingAuthor = 3095705478U,
			InvalidIPAddressInSmartHost = 2578042802U,
			DeliveryTypeSmtpRelayToMailboxDeliveryGroup = 2736312437U,
			UnknownEdition = 2889762178U,
			InvalidFormatExchangeObjectVersion = 3765811860U,
			ToInternet = 3427924322U,
			KindKeywordTasks = 1226277855U,
			MarkedAsRetryDeliveryIfRejected = 736960779U,
			ConnectorTypeOnPremises = 4060482376U,
			HeaderValue = 2163423328U,
			SmtpReceiveCapabilitiesAcceptProxyFromProtocol = 2117971051U,
			PoisonQueueNextHopDomain = 1153159321U,
			GroupWiseProxyAddressPrefixDisplayName = 936162170U,
			MsMailProxyAddressPrefixDisplayName = 3861972062U,
			Exchange2007 = 2924600836U,
			HeaderName = 2909775076U,
			RejectText = 1178729042U,
			NameValidationSpaceAllowedPatternDescription = 2493635772U,
			ConstraintViolationStringLengthCauseOutOfMemory = 2255949938U,
			InvalidFormat = 3639763634U,
			CustomPeriod = 729961244U,
			PublicFolderPermissionRolePublishingEditor = 95015764U,
			InvalidKeyMappingKey = 1893156877U,
			StringConversionDelegateNotSet = 224331957U,
			NumberingPlanPatternDescription = 2603796430U,
			MeetingFullDetails = 716522036U,
			ProtocolLoggingLevelNone = 3427843085U,
			ErrorInputFormatError = 2353628225U,
			Ascending = 3390434404U,
			SmtpReceiveCapabilitiesAcceptXOriginalFromProtocol = 3345411900U,
			Exchange2003 = 599002008U,
			WorkingHours = 1604545240U,
			DeferReasonTransientAttributionFailure = 2838855213U,
			FromEnterprise = 2536591407U,
			TlsAuthLevelCertificateValidation = 1206366831U,
			Friday = 4094875965U,
			MeetingLimitedDetails = 4009888891U,
			KeyMappingInvalidArgument = 61359385U,
			LegacyDNPatternDescription = 1186448673U,
			SmtpReceiveCapabilitiesAcceptXSysProbeProtocol = 2245804386U,
			SentTime = 2677919833U,
			SearchSender = 2511844601U,
			ErrorCannotAddNullValue = 2397470300U,
			ScheduleModeScheduledTimes = 3811769309U,
			ProtocolAppleTalk = 391606028U,
			DigitStringOrEmptyPatternDescription = 3390939332U,
			EnterpriseEdition = 26915469U,
			ConnectorSourceDefault = 2726993793U,
			ExceptionVersionlessObject = 3372601165U,
			AliasPatternDescription = 2815103562U,
			ReceivedTime = 1026314696U,
			ParameterNameEmptyException = 3479640092U,
			RecipientStatusLocked = 2043548597U,
			SmtpReceiveCapabilitiesAcceptProxyToProtocol = 1248401958U,
			PermissionGroupsNone = 1109398861U,
			ArgumentMustBeAscii = 2137442040U,
			ExceptionNetworkProtocolDuplicate = 1284326164U,
			RecipientStatusRetry = 3511836031U,
			GroupExpansionRecipients = 1002496550U,
			LegacyDNProxyAddressPrefixDisplayName = 1061876008U,
			HeaderPromotionModeMustCreate = 781217110U,
			ProtocolLoggingLevelVerbose = 2082847001U,
			HeaderPromotionModeNoCreate = 1176489840U,
			EncryptionTypeSSL = 462794175U,
			KindKeywordDocs = 3512036720U,
			Unreachable = 2589626668U,
			CustomScheduleSundayAtMidnight = 1884450703U,
			ExceptionUnknownUnit = 4294726685U,
			SendNone = 3410257022U,
			SubjectPrefix = 1100730082U,
			DeliveryTypeSmtpRelayToServers = 3791230818U,
			InvalidKeyMappingFindMeFirstNumberDuration = 1667689070U,
			AirSyncProxyAddressPrefixDisplayName = 2482088490U,
			CoexistenceEdition = 1474747046U,
			UnsearchableItemsAdded = 2175102537U,
			InvalidKeyMappingFormat = 2846565657U,
			ClientAccessProtocolPOP3 = 4125301959U,
			Word = 4143129766U,
			AddressSpaceIsTooLong = 3614810764U,
			AddressFamilyMismatch = 3699252422U,
			KindKeywordFaxes = 294615990U,
			ExceptionEmptyProxyAddress = 2440320060U,
			ExceptionObjectInvalid = 882442171U,
			ExceptionReadOnlyMultiValuedProperty = 1177570172U,
			Tuesday = 2820941203U,
			DeferReasonADTransientFailureDuringResolve = 2714058314U,
			DeliveryTypeNonSmtpGatewayDelivery = 3770991413U,
			UseExchangeDSNs = 980447290U,
			KindKeywordContacts = 1316672322U,
			FromLocal = 2112156755U,
			ErrorPoliciesDowngradeDnsFailures = 3991588639U,
			TlsAuthLevelEncryptionOnly = 466678310U,
			Sunday = 1073167130U,
			CustomScheduleDailyFrom9AMTo6PMAtWeekDays = 1629702106U,
			Descending = 1777112844U,
			DeliveryTypeUnreachable = 2350082752U,
			KindKeywordPosts = 4067663994U,
			ErrorServerGuidAndNameBothEmpty = 4234859176U,
			ExLengthOfVersionByteArrayError = 64170653U,
			SearchRecipientsTo = 3444147793U,
			DeliveryTypeMapiDelivery = 918988081U,
			SearchRecipientsBcc = 2906226218U,
			EmptyNameInHostname = 1947725858U,
			DisclaimerText = 1689084926U,
			InvalidSmtpDomainWildcard = 1950405677U,
			CustomScheduleDailyFrom2AMTo6AM = 3328112862U,
			InvalidTimeOfDayFormat = 3884278210U,
			Failed = 1054423051U,
			CustomScheduleDailyFrom11PMTo6AM = 2176757305U,
			ColonPrefix = 3124035205U,
			ToGroupExpansionRecipients = 3544743625U,
			InvalidCallerIdItemFormat = 923847139U,
			PublicFolderPermissionRoleOwner = 4060377141U,
			PermissionGroupsCustom = 2634610906U,
			KindKeywordIm = 3561826809U,
			GroupByTotal = 1977971622U,
			BccGroupExpansionRecipients = 3781744254U,
			ExceptionInvlidNetworkAddressFormat = 833225182U,
			KindKeywordJournals = 2456836287U,
			EmptyExchangeBuild = 1525449578U,
			StandardEdition = 2321790947U,
			FormatExchangeBuildWrong = 29693289U,
			ClientAccessProtocolPSWS = 79160252U,
			DeliveryTypeSmtpRelayToConnectorSourceServers = 3706983644U,
			CustomScheduleSaturdayAtMidnight = 3519073218U,
			ExceptionNoValue = 71385097U,
			MailRecipientTypeDistributionGroup = 2608795131U,
			ExceptionInvlidProtocolAddressFormat = 1561011830U,
			CustomScheduleFridayAtMidnight = 2364433662U,
			DeliveryTypeShadowRedundancy = 255999871U,
			DeferReasonLoopDetected = 3951803838U,
			SearchRecipients = 3183375374U,
			CalendarSharingFreeBusySimple = 2862582797U,
			PublicFolderPermissionRoleReviewer = 3422989433U,
			QueueStatusActive = 2267899661U,
			TlsAuthLevelCertificateExpiryCheck = 2387319017U,
			InvalidAddressSpaceAddress = 3030346869U,
			FromPartner = 1501056036U,
			AllDays = 3502523774U,
			DeferReasonTargetSiteInboundMailDisabled = 2099540954U,
			ExceptionFormatNotSupported = 2760297639U,
			DeliveryTypeDeliveryAgent = 2279665409U,
			EstimatedItems = 1531043460U,
			CmdletParameterEmptyValidationException = 1261683271U,
			MessageStatusPendingRemove = 66601190U,
			QueueStatusConnecting = 3129286587U,
			DeliveryTypeSmtpRelayToRemoteAdSite = 1108839058U,
			Saturday = 3478111469U,
			ToEnterprise = 4135023588U,
			InvalidHolidayScheduleFormat = 339456021U,
			InvalidTimeOfDayFormatWorkingHours = 1915155164U,
			CustomScheduleDaily11PM = 3119708320U,
			QueueStatusSuspended = 1761145356U,
			SubmissionQueueNextHopDomain = 999227985U,
			ErrorNotSupportedForChangesOnlyCopy = 3153135486U,
			CcGroupExpansionRecipients = 4098793892U,
			ProxyAddressPrefixTooLong = 3039999898U,
			Pattern = 1660006804U,
			DeliveryTypeSmartHostConnectorDelivery = 1029030096U,
			KindKeywordVoiceMail = 2368086636U,
			RoutingNoRouteToMta = 570747971U,
			ClientAccessProtocolEWS = 2517721976U,
			MailRecipientTypeExternal = 810977739U,
			DeliveryTypeSmtpRelayWithinAdSite = 749395574U,
			InvalidDialGroupEntryCsvFormat = 1800544803U,
			KindKeywordMeetings = 607066203U,
			CustomScheduleEveryFourHours = 2682624686U,
			MailRecipientTypeUnknown = 3134372274U,
			AmbiguousRecipient = 894196715U,
			SmtpReceiveCapabilitiesAllowConsumerMail = 182065869U,
			InvalidFlagValue = 1601563988U,
			TlsAuthLevelDomainValidation = 1435915854U,
			ClientAccessBasicAuthentication = 3942979931U,
			InvalidTimeOfDayFormatCustomWorkingHours = 3828585651U,
			AttachmentContent = 1720677880U,
			InvalidKeyMappingTransferToGalContact = 1152451692U,
			ProtocolSpx = 2941311901U,
			ErrorPoliciesDowngradeCustomFailures = 4237384485U,
			ErrorCostOutOfRange = 4240385111U,
			ExceptionNegativeUnit = 1140124266U,
			DeliveryTypeSmtpDeliveryToMailbox = 982561113U,
			PropertyIsMandatory = 2755629282U,
			RoutingNonBHExpansionServer = 2085180697U,
			ExceptionReadOnlyPropertyBag = 838517570U,
			UnreachableQueueNextHopDomain = 1594003981U,
			InvalidSmtpDomain = 682709795U,
			InvalidDialledNumberFormatC = 416319699U,
			DeliveryTypeDnsConnectorDelivery = 3338773880U,
			DigitStringPatternDescription = 2544690258U,
			CustomScheduleDailyFrom9AMTo5PMAtWeekDays = 3001525213U,
			InvalidKeyMappingContext = 4197662657U,
			CustomScheduleDaily1AM = 3826988642U,
			FromInternet = 1512272061U,
			TotalCopiedItems = 2854645212U,
			ClientAccessProtocolRPS = 4083806514U,
			ClientAccessNonBasicAuthentication = 3406691936U,
			RoutingNoMdb = 3877102044U,
			PublicFolderPermissionRoleEditor = 443721013U,
			MailRecipientTypeMailbox = 3090942252U,
			CopyErrors = 460086660U,
			SnapinNameTooShort = 2182454218U,
			TextBody = 4028819673U,
			DeliveryTypeSmtpRelayToTiRg = 1277617174U,
			ConstraintViolationNoLeadingOrTrailingWhitespace = 2058499689U,
			CalendarSharingFreeBusyDetail = 616309426U,
			ItemClass = 4177834631U,
			InvalidNotationFormat = 38623232U,
			ClientAccessProtocolOAB = 2067382811U,
			InvalidCallerIdItemTypePhoneNumber = 2956076415U,
			ExceptionDurationOverflow = 3803801081U,
			ExchangeLegacyServers = 4064690660U,
			Down = 1367191786U,
			InvalidDialledNumberFormatA = 416319697U,
			Misconfigured = 3211494971U,
			ProtocolTcpIP = 974174060U,
			RoleEntryStringMustBeCommaSeparated = 3287485561U,
			SmtpReceiveCapabilitiesAcceptOorgProtocol = 3779112048U,
			WeekendDays = 4100738364U,
			ProtocolNetBios = 2509290958U,
			CustomScheduleDailyFrom8AMTo12PMAnd1PMTo5PMAtWeekDays = 1022093144U,
			ConfigurationSettingsAppSettingsError = 1817337337U,
			InvalidKeyMappingTransferToNumber = 1261519073U,
			CustomScheduleDaily5AM = 452901710U,
			KindKeywordRssFeeds = 871198498U,
			MessageStatusSuspended = 1937548848U,
			ShadowMessagePreferenceLocalOnly = 462978851U,
			Unavailable = 2047193656U,
			RejectStatusCode = 1434991878U,
			Thursday = 1760294240U,
			StartingAddressAndMaskAddressFamilyMismatch = 1039830289U,
			InvalidCallerIdItemTypeDefaultContactsFolder = 2802337392U,
			ErrorPoliciesDefault = 3137483865U,
			PublicFolderPermissionRoleContributor = 268882683U,
			Weekdays = 2587222631U,
			SmtpReceiveCapabilitiesAllowSubmit = 3048094658U,
			PropertyNotEmptyOrNull = 3673950617U,
			MAPIBlockOutlookVersionsPatternDescription = 2788726202U,
			BucketVersionPatternDescription = 4263235384U,
			MessageStatusNone = 1602600165U,
			PublicFolderPermissionRoleAuthor = 2795839773U,
			DeliveryTypeUndefined = 2292506358U,
			SmtpReceiveCapabilitiesAcceptCrossForestMail = 1405683073U,
			ConnectorSourceMigrated = 2397369159U,
			RoutingNoMatchingConnector = 2934715437U,
			QueueStatusNone = 548875607U,
			ClientAccessProtocolEAC = 2067383266U,
			EncryptionTypeTLS = 2929555192U,
			RoleEntryNameTooShort = 3496963749U,
			DuplicatesRemoved = 3076383364U,
			MessageStatusPendingSuspend = 681220170U,
			SmtpReceiveCapabilitiesAcceptOrgHeaders = 3785867729U,
			CustomScheduleDailyAtMidnight = 2114081924U,
			RecipientStatusComplete = 412393748U,
			InvalidCallerIdItemTypeGALContactr = 2721491554U,
			ExchangeServers = 1093243629U,
			ScheduleModeNever = 1206036754U,
			CustomScheduleDailyFrom11PMTo3AM = 1511350752U,
			InvalidDialledNumberFormatD = 416319702U,
			CustomScheduleDailyFrom1AMTo5AM = 1826938428U,
			ProxyAddressPrefixShouldNotBeAllSpace = 344942096U,
			CustomScheduleDaily2AM = 835983261U,
			InvalidKeyMappingFindMeSecondNumber = 1899327622U,
			MessageStatusReady = 202819370U,
			GroupByDay = 1069174646U,
			DeliveryTypeHeartbeat = 3028676818U,
			FileExtensionOrSplatPatternDescription = 2470774060U,
			MessageStatusActive = 3713180243U,
			Monday = 3364213626U,
			DeferReasonTransientAcceptedDomainsLoadFailure = 2973542840U,
			ExceptionEventSourceNull = 3065954297U,
			PublicFolderPermissionRoleNonEditingAuthor = 1205942262U,
			CustomScheduleEveryTwoHours = 700746266U,
			InvalidCallerIdItemTypePersonaContact = 2988328408U,
			DeferReasonConfigUpdate = 1169266063U,
			ConstraintViolationValueIsNullOrEmpty = 654440350U,
			SubjectProperty = 1069535743U,
			InvalidKeySelection_Zero = 1074863583U,
			QueueStatusReady = 2743519022U,
			ProtocolNamedPipes = 2677933048U,
			ProtocolVnsSpp = 1839463316U,
			KindKeywordNotes = 2247071192U,
			ErrorCannotConvert = 1707192398U,
			InvalidDialledNumberFormatB = 416319700U,
			InvalidDialGroupEntryFormat = 1593650977U,
			EapMustHaveOneEnabledPrimarySmtpAddressTemplate = 3543498202U,
			FileExtensionPatternDescription = 2682420737U,
			Unknown = 2846264340U,
			QueueStatusRetry = 4060777377U,
			Wednesday = 3452652986U,
			InvalidKeyMappingFindMe = 3417603459U,
			DeferReasonReroutedByStoreDriver = 417785204U,
			ScheduleModeAlways = 1326579539U,
			FileIsEmpty = 2583398905U,
			ExceptionSerializationDataAbsent = 621231604U,
			SmtpReceiveCapabilitiesAcceptOorgHeader = 3488668113U,
			DsnText = 881737792U,
			CustomScheduleDailyFrom9AMTo12PMAnd1PMTo6PMAtWeekDays = 269836784U,
			StorageTransientFailureDuringContentConversion = 1301289463U,
			MessageStatusRetry = 1500391415U,
			CustomProxyAddressPrefixDisplayName = 387961094U,
			CLIDPatternDescription = 1088965676U,
			ClientAccessProtocolEAS = 2067383282U,
			ConstraintViolationInvalidWindowsLiveIDLocalPart = 304927915U,
			ExceptionParseInternalMessageId = 1914858911U,
			RecipientStatusReady = 2476902678U,
			ReceiveNone = 2221667633U,
			UserPrincipalNamePatternDescription = 560731754U,
			ToPartner = 2579867249U,
			DeliveryTypeSmtpRelayToDag = 824224038U,
			DeliveryTypeSmtpRelayWithinAdSiteToEdge = 1676404342U,
			DeferReasonRecipientThreadLimitExceeded = 1472458119U,
			Up = 1543969273U,
			PreserveDSNBody = 4133244061U,
			ElcScheduleInsufficientGap = 2194722870U,
			ExceptionTypeNotEnhancedTimeSpanOrTimeSpan = 3501929169U,
			Recipients = 986397318U,
			CustomScheduleDaily4AM = 3443907091U,
			ExceptionDefaultTypeMismatch = 4277209464U,
			ClientAccessProtocolOWA = 2517721504U,
			RecipientStatusNone = 2947517465U,
			StandardTrialEdition = 553174585U,
			ShadowMessagePreferenceRemoteOnly = 1980952396U,
			DoNotConvert = 1065872813U,
			CustomScheduleDaily10PM = 3119708351U
		}

		private enum ParamIDs
		{
			AddressSpaceCostOutOfRange,
			ExceptionWriteOnceProperty,
			SharingPolicyDomainInvalidAction,
			InvalidCIDRLengthIPv6,
			ErrorToStringNotImplemented,
			CmdletFullNameFormatException,
			NumberFormatStringTooLong,
			ExceptionInvalidFormat,
			ErrorFileShareWitnessServerNameIsNotValidHostNameorFqdnWildcard,
			InvalidTypeArgumentException,
			PropertyTypeMismatch,
			ScriptRoleEntryNameInvalidException,
			ExceptionParseNonFilterablePropertyErrorWithList,
			ExceptionProtocolConnectionSettingsInvalidEncryptionType,
			ErrorCannotCopyFromDifferentType,
			ConfigurationSettingsPropertyNotFound2,
			InvalidCIDRLengthIPv4,
			ConstraintViolationValueIsNotInGivenStringArray,
			ExceptionRemoveSmtpPrimary,
			ErrorCannotConvertFromBinary,
			InvalidSmtpX509Identifier,
			DNWithBinaryFormatError,
			DialGroupNotSpecifiedOnDialPlan,
			ConstraintViolationPathLength,
			ConfigurationSettingsPropertyBadValue,
			ThrottlingPolicyStateCorrupted,
			ConstraintViolationObjectIsBelowRange,
			ExceptionVariablesNotSupported,
			ConfigurationSettingsScopePropertyBadValue,
			InvalidEumAddress,
			ElcScheduleInvalidType,
			ExceptionUnsupportedTypeConversion,
			InvalidSmtpAddress,
			ErrorObjectVersionReadOnly,
			ErrorFileShareWitnessServerNameIsNotValidHostNameorFqdn,
			ExceptionParseNonFilterablePropertyError,
			ExceptionUnsupportedDestinationType,
			ConstraintViolationNotValidEmailAddress,
			ErrorInputSchedulerBuilder,
			ConstraintViolationInvalidUriScheme,
			ExceptionCurrentIndexOutOfRange,
			IncludeExcludeConflict,
			ExceptionReadOnlyBecauseTooNew,
			PropertyNotACollection,
			ErrorFileShareWitnessServerNameIsLocalhost,
			CollectiionWithTooManyItemsFormat,
			ConstraintViolationEnumValueNotDefined,
			ExArgumentNullException,
			ExceptionProtocolConnectionSettingsInvalidFormat,
			ErrorInvalidEnumValue,
			ExceptionComparisonNotSupported,
			InvalidDialGroupEntryElementLength,
			ExceptionGeoCoordinatesWithInvalidLatitude,
			ExceptionInvalidLatitude,
			InvalidOrganizationSummaryEntryFormat,
			InvalidDateFormat,
			InvalidEumAddressTemplateFormat,
			ExceptionProtocolConnectionSettingsInvalidPort,
			ConstraintViolationObjectIsBeyondRange,
			ExceptionGeoCoordinatesWithInvalidLongitude,
			ExceptionInvalidOperation,
			ErrorOperationNotSupported,
			ErrorIncorrectLiveIdFormat,
			UnknownColumns,
			InvalidTypeArgumentExceptionMultipleExceptedTypes,
			ConstraintViolationIpV6NotAllowed,
			ExceptionEventCategoryNotFound,
			ErrorObjectSerialization,
			ConstraintViolationInvalidUriKind,
			ParameterNameInvalidCharException,
			ConfigurationSettingsScopePropertyNotFound,
			ConstraintViolationSecurityDescriptorContainsInheritedACEs,
			ConstraintViolationInvalidIntRange,
			ConstraintViolationValueIsNotAllowed,
			ConstraintViolationMalformedExtensionValuePair,
			ElcScheduleInvalidIntervals,
			ExceptionUsingInvalidAddress,
			InvalidNumber,
			ErrorCannotSaveBecauseTooNew,
			ConfigurationSettingsScopePropertyFailedValidation,
			InvalidDomainInSmtpX509Identifier,
			InvalidAddressSpaceCostFormat,
			ServicePlanFeatureCheckFailed,
			InvalidSMTPAddressTemplateFormat,
			InvalidOrganizationSummaryEntryValue,
			DataNotCloneable,
			ExceptionBitMaskNotSupported,
			ConstraintViolationStringContainsInvalidCharacters2,
			InvalidIPAddressMask,
			ConstraintViolationStringDoesNotMatchRegularExpression,
			ScheduleDateInvalid,
			InvalidSmtpReceiveDomainCapabilities,
			ExceptionCannotResolveOperation,
			InvalidConnectedDomainFormat,
			InvalidOrganizationSummaryEntryKey,
			ExceptionEndPointPortOutOfRange,
			ExceptionGeoCoordinatesWithInvalidAltitude,
			ConstraintViolationEnumValueNotAllowed,
			SharingPolicyDomainInvalidDomain,
			UnsupportServerEdition,
			ConstraintViolationByteArrayLengthTooLong,
			InvalidCIDRLength,
			ErrorFileShareWitnessServerNameCannotConvert,
			ExceptionFormatNotCorrect,
			ConstraintViolationByteArrayLengthTooShort,
			InvalidX400Domain,
			InvalidDialGroupEntryElementFormat,
			ExceptionPropertyTooNew,
			RequiredColumnMissing,
			ExceptionQueueIdentityCompare,
			ErrorIncorrectWindowsLiveIdFormat,
			InvalidIPRange,
			ExceptionInvalidMeumAddress,
			ErrorSerialNumberFormatError,
			ExceptionProtocolConnectionSettingsInvalidHostname,
			ErrorCannotConvertToBinary,
			ProxyAddressTemplateEmptyPrefixOrValue,
			InvalidAddressSpaceType,
			ErrorFileShareWitnessServerNameMustNotBeIP,
			StarDomainNotAllowed,
			ErrorValueAlreadyPresent,
			ErrorNonGeneric,
			Int32ParsableStringConstraintViolation,
			ExceptionRemoveEumPrimary,
			ConstraintViolationIpRangeNotAllowed,
			ErrorOutOfRange,
			DialGroupNotSpecifiedOnDialPlanB,
			SharingPolicyDomainInvalidActionForDomain,
			InvalidTlsCertificateName,
			ErrorInputOfScheduleMustExclusive,
			ExceptionCannotSetDifferentType,
			ErrorUnknownOperation,
			ExceptionParseError,
			LinkedPartnerGroupInformationInvalidParameter,
			SharingPolicyDomainInvalidActionForAnonymous,
			ConstraintViolationDontMatchUnit,
			ClientAccessRulesBlockedConnection,
			ErrorLengthOfExDataTimeByteArray,
			ConfigurationSettingsScopePropertyNotFound2,
			PropertyName,
			InvalidCharInString,
			InvalidX400AddressSpace,
			ConstraintViolationStringLengthTooShort,
			InvalidIPAddressOrHostNameInSmartHost,
			SmtpResponseConstraintViolation,
			ExceptionEndPointMissingSeparator,
			ExceptionFormatInvalid,
			IncludeExcludeInvalid,
			ErrorCannotConvertNull,
			InvalidScopedAddressSpace,
			ErrorConversionFailedWithException,
			DuplicateParameterException,
			ExceptionInvalidEumAddress,
			DuplicatedColumn,
			ConstraintViolationValueIsDisallowed,
			ExceptionEndPointInvalidPort,
			ErrorReadOnlyCauseObject,
			ErrorCannotConvertFromString,
			ConstraintNoTrailingSpecificCharacter,
			DependencyCheckFailed,
			ConfigurationSettingsPropertyNotFound,
			ErrorCannotConvertToString,
			ServicePlanSchemaCheckFailed,
			ApplicationPermissionRoleEntryParameterNotEmptyException,
			SharingPolicyDomainInvalid,
			ExceptionValueOverflow,
			ErrorToBinaryNotImplemented,
			BadEnumValue,
			ExceptionInvalidServerName,
			ExceptionInvalidSmtpAddress,
			ConstraintViolationStringContainsInvalidCharacters,
			InvalidIntRangeArgument,
			WrongNumberOfColumns,
			FilterOnlyAttributes,
			InvalidAlternateMailboxString,
			ConstraintViolationValueOutOfRange,
			ExceptionInvalidLongitude,
			ExceptionGeoCoordinatesWithWrongFormat,
			RoleEntryNameInvalidException,
			ConstraintViolationStringLengthTooLong,
			ExceptionUnsupportedSourceType,
			InvalidRoleEntryType,
			ConfigurationSettingsPropertyBadType,
			InvalidSmtpDomainName,
			TooManyRows,
			InvalidHostname,
			InvalidX509IdentifierFormat,
			ConstraintViolationStringIsNotValidCultureInfo,
			ErrorContainsOutOfRange,
			ErrorConversionFailed,
			ExceptionInvalidFilterOperator,
			ErrorInvalidScheduleType,
			ErrorDSNCultureInput,
			ExceptionEmptyPrefixEum,
			ErrorReadOnlyCauseProperty,
			ErrorOrignalMultiValuedProperty,
			InvalidIPAddressFormat,
			ExceptionEventSourceNotFound,
			InvalidNumberFormat,
			InvalidIPRangeFormat,
			ConstraintViolationNotValidDomain,
			AdminAuditLogInvalidParameterOrModifiedProperty,
			ConfigurationSettingsPropertyFailedValidation,
			WebServiceRoleEntryParameterNotEmptyException,
			ConstraintViolationStringDoesNotContainNonWhitespaceCharacter,
			ExceptionUnsupportedDataFormat,
			ProhibitedColumnPresent,
			ConstraintViolationStringDoesContainsNonASCIICharacter,
			ExceptionEmptyPrefix,
			ErrorMvpNotImplemented,
			SnapinNameInvalidCharException,
			ExceptionEndPointInvalidIPAddress
		}
	}
}
