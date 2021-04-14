using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core.RuleTasks
{
	public static class RulesTasksStrings
	{
		static RulesTasksStrings()
		{
			RulesTasksStrings.stringIDs.Add(3817895306U, "LinkedPredicateHeaderMatches");
			RulesTasksStrings.stringIDs.Add(3690383901U, "SetAuditSeverityDisplayName");
			RulesTasksStrings.stringIDs.Add(856583401U, "ADAttributeOtherHomePhoneNumber");
			RulesTasksStrings.stringIDs.Add(607657692U, "JournalingParameterErrorFullReportWithoutGcc");
			RulesTasksStrings.stringIDs.Add(3705591125U, "RecipientTypeDescription");
			RulesTasksStrings.stringIDs.Add(1840928351U, "EvaluatedUserDisplayName");
			RulesTasksStrings.stringIDs.Add(1484331716U, "LinkedPredicateHeaderContains");
			RulesTasksStrings.stringIDs.Add(2129610537U, "MessageTypeOof");
			RulesTasksStrings.stringIDs.Add(862345881U, "ImportanceDisplayName");
			RulesTasksStrings.stringIDs.Add(887494032U, "DlpPolicyModeIsOverridenByModeParameter");
			RulesTasksStrings.stringIDs.Add(3832906569U, "LinkedActionApplyHtmlDisclaimer");
			RulesTasksStrings.stringIDs.Add(2799440992U, "RejectMessageParameterWillBeIgnored");
			RulesTasksStrings.stringIDs.Add(4206835639U, "LinkedPredicateRecipientAddressContainsWords");
			RulesTasksStrings.stringIDs.Add(3386546304U, "AttachmentSizeDescription");
			RulesTasksStrings.stringIDs.Add(1966725662U, "LinkedPredicateRecipientAddressMatchesPatternsException");
			RulesTasksStrings.stringIDs.Add(2655586213U, "AttributeValueDescription");
			RulesTasksStrings.stringIDs.Add(3729435602U, "LinkedPredicateAttachmentMatchesPatternsException");
			RulesTasksStrings.stringIDs.Add(694309943U, "LinkedActionLogEvent");
			RulesTasksStrings.stringIDs.Add(3534737571U, "LinkedPredicateAnyOfToHeader");
			RulesTasksStrings.stringIDs.Add(3304155086U, "EvaluatedUserDescription");
			RulesTasksStrings.stringIDs.Add(1377545167U, "ADAttributeCustomAttribute1");
			RulesTasksStrings.stringIDs.Add(996206651U, "LinkedPredicateAttachmentPropertyContainsWordsException");
			RulesTasksStrings.stringIDs.Add(2978999437U, "LinkedActionApplyClassification");
			RulesTasksStrings.stringIDs.Add(1938376749U, "LinkedActionSetScl");
			RulesTasksStrings.stringIDs.Add(2156000375U, "LinkedPredicateAnyOfCcHeaderException");
			RulesTasksStrings.stringIDs.Add(207684870U, "ArgumentNotSet");
			RulesTasksStrings.stringIDs.Add(2267758972U, "InvalidRuleName");
			RulesTasksStrings.stringIDs.Add(5978988U, "RuleDescriptionDisclaimerRejectFallback");
			RulesTasksStrings.stringIDs.Add(2498488782U, "LinkedPredicateFromAddressContainsException");
			RulesTasksStrings.stringIDs.Add(3718292884U, "LinkedPredicateAttachmentNameMatchesException");
			RulesTasksStrings.stringIDs.Add(1016721882U, "ADAttributeLastName");
			RulesTasksStrings.stringIDs.Add(151284935U, "GenerateNotificationDisplayName");
			RulesTasksStrings.stringIDs.Add(281219183U, "RuleDescriptionNotifySenderNotifyOnly");
			RulesTasksStrings.stringIDs.Add(3600528589U, "ADAttributeCountry");
			RulesTasksStrings.stringIDs.Add(3542666061U, "MessageHeaderDisplayName");
			RulesTasksStrings.stringIDs.Add(4246027971U, "LinkedPredicateAttachmentMatchesPatterns");
			RulesTasksStrings.stringIDs.Add(3208048360U, "ListsDisplayName");
			RulesTasksStrings.stringIDs.Add(2328530086U, "FallbackIgnore");
			RulesTasksStrings.stringIDs.Add(77980785U, "LinkedPredicateSenderIpRangesException");
			RulesTasksStrings.stringIDs.Add(763738384U, "AttributeValueDisplayName");
			RulesTasksStrings.stringIDs.Add(2380701036U, "LinkedPredicateRecipientDomainIsException");
			RulesTasksStrings.stringIDs.Add(1655133361U, "InboxRuleDescriptionHasAttachment");
			RulesTasksStrings.stringIDs.Add(2340468721U, "RuleDescriptionHasSenderOverride");
			RulesTasksStrings.stringIDs.Add(2538565266U, "LinkedPredicateRecipientInSenderListException");
			RulesTasksStrings.stringIDs.Add(705865031U, "LinkedPredicateAttachmentExtensionMatchesWordsException");
			RulesTasksStrings.stringIDs.Add(3850060255U, "ImportanceNormal");
			RulesTasksStrings.stringIDs.Add(3099194133U, "LinkedActionCopyTo");
			RulesTasksStrings.stringIDs.Add(2306965519U, "ListsDescription");
			RulesTasksStrings.stringIDs.Add(1130765528U, "TextPatternsDescription");
			RulesTasksStrings.stringIDs.Add(1191033945U, "NewRuleSyncAcrossDifferentVersionsNeeded");
			RulesTasksStrings.stringIDs.Add(3108375176U, "RmsTemplateDescription");
			RulesTasksStrings.stringIDs.Add(4223753465U, "LinkedPredicateRecipientInSenderList");
			RulesTasksStrings.stringIDs.Add(2468414724U, "ADAttributeInitials");
			RulesTasksStrings.stringIDs.Add(1331793023U, "LinkedPredicateHeaderContainsException");
			RulesTasksStrings.stringIDs.Add(3721269126U, "LinkedPredicateContentCharacterSetContainsWords");
			RulesTasksStrings.stringIDs.Add(1697423233U, "MessageTypeDescription");
			RulesTasksStrings.stringIDs.Add(3882899654U, "ADAttributeState");
			RulesTasksStrings.stringIDs.Add(3643356345U, "FromScopeDisplayName");
			RulesTasksStrings.stringIDs.Add(2812686069U, "InboxRuleDescriptionAnd");
			RulesTasksStrings.stringIDs.Add(1377545174U, "ADAttributeCustomAttribute8");
			RulesTasksStrings.stringIDs.Add(1720532765U, "LinkedPredicateSubjectMatches");
			RulesTasksStrings.stringIDs.Add(2041595767U, "MessageTypeAutoForward");
			RulesTasksStrings.stringIDs.Add(680782013U, "MessageTypeReadReceipt");
			RulesTasksStrings.stringIDs.Add(431368248U, "LinkedActionBlindCopyTo");
			RulesTasksStrings.stringIDs.Add(2112278218U, "LinkedPredicateSentToScopeException");
			RulesTasksStrings.stringIDs.Add(3773694392U, "LinkedActionStopRuleProcessing");
			RulesTasksStrings.stringIDs.Add(3956509743U, "LinkedPredicateAnyOfToCcHeader");
			RulesTasksStrings.stringIDs.Add(3369692937U, "LinkedPredicateHasSenderOverride");
			RulesTasksStrings.stringIDs.Add(2611996929U, "RedirectRecipientType");
			RulesTasksStrings.stringIDs.Add(1231044387U, "FallbackActionDisplayName");
			RulesTasksStrings.stringIDs.Add(161711511U, "NoAction");
			RulesTasksStrings.stringIDs.Add(3921523715U, "InboxRuleDescriptionFlaggedForAnyAction");
			RulesTasksStrings.stringIDs.Add(1645493202U, "LinkedActionSetHeader");
			RulesTasksStrings.stringIDs.Add(3850705779U, "SenderIpRangesDisplayName");
			RulesTasksStrings.stringIDs.Add(2375893868U, "LinkedPredicateAttachmentIsUnsupported");
			RulesTasksStrings.stringIDs.Add(3050431750U, "ADAttributeName");
			RulesTasksStrings.stringIDs.Add(3095013874U, "EventMessageDisplayName");
			RulesTasksStrings.stringIDs.Add(1080850367U, "IncidentReportOriginalMailnDisplayName");
			RulesTasksStrings.stringIDs.Add(3689464497U, "ADAttributeOtherFaxNumber");
			RulesTasksStrings.stringIDs.Add(3115100581U, "RejectUnlessExplicitOverrideActionType");
			RulesTasksStrings.stringIDs.Add(4231516709U, "RuleDescriptionHasNoClassification");
			RulesTasksStrings.stringIDs.Add(130230884U, "LinkedPredicateAnyOfRecipientAddressMatchesException");
			RulesTasksStrings.stringIDs.Add(3856180838U, "MessageTypeDisplayName");
			RulesTasksStrings.stringIDs.Add(3144351139U, "FallbackReject");
			RulesTasksStrings.stringIDs.Add(3374360575U, "ADAttributeCustomAttribute10");
			RulesTasksStrings.stringIDs.Add(4289093673U, "ADAttributeEmail");
			RulesTasksStrings.stringIDs.Add(1377545163U, "ADAttributeCustomAttribute5");
			RulesTasksStrings.stringIDs.Add(1301491835U, "LinkedPredicateSubjectOrBodyMatchesException");
			RulesTasksStrings.stringIDs.Add(2591548322U, "InboxRuleDescriptionMyNameInToBox");
			RulesTasksStrings.stringIDs.Add(4199979286U, "ToRecipientType");
			RulesTasksStrings.stringIDs.Add(1321315595U, "LinkedPredicateAttachmentContainsWords");
			RulesTasksStrings.stringIDs.Add(2934581752U, "LinkedActionRightsProtectMessage");
			RulesTasksStrings.stringIDs.Add(3527153583U, "LinkedActionSetAuditSeverity");
			RulesTasksStrings.stringIDs.Add(1125919747U, "LinkedPredicateBetweenMemberOf");
			RulesTasksStrings.stringIDs.Add(1050054139U, "RemoveRuleSyncAcrossDifferentVersionsNeeded");
			RulesTasksStrings.stringIDs.Add(2569693958U, "ADAttributeDisplayName");
			RulesTasksStrings.stringIDs.Add(354503089U, "LinkedPredicateSentToScope");
			RulesTasksStrings.stringIDs.Add(3455130960U, "DisclaimerLocationDescription");
			RulesTasksStrings.stringIDs.Add(1316941123U, "FromAddressesDisplayName");
			RulesTasksStrings.stringIDs.Add(1648356515U, "LinkedActionRejectMessage");
			RulesTasksStrings.stringIDs.Add(328892189U, "LinkedPredicateSenderAttributeMatchesException");
			RulesTasksStrings.stringIDs.Add(1857695339U, "LinkedActionRouteMessageOutboundRequireTls");
			RulesTasksStrings.stringIDs.Add(3266871523U, "LinkedPredicateAttachmentNameMatches");
			RulesTasksStrings.stringIDs.Add(3211626450U, "HeaderValueDescription");
			RulesTasksStrings.stringIDs.Add(4103233806U, "RecipientTypeDisplayName");
			RulesTasksStrings.stringIDs.Add(2869706848U, "LinkedPredicateFromMemberOfException");
			RulesTasksStrings.stringIDs.Add(1377545169U, "ADAttributeCustomAttribute3");
			RulesTasksStrings.stringIDs.Add(3850073087U, "ADAttributePagerNumber");
			RulesTasksStrings.stringIDs.Add(117825870U, "MessageTypeVoicemail");
			RulesTasksStrings.stringIDs.Add(903770574U, "LinkedPredicateWithImportance");
			RulesTasksStrings.stringIDs.Add(2749090792U, "GenerateNotificationDescription");
			RulesTasksStrings.stringIDs.Add(2634964433U, "ADAttributeTitle");
			RulesTasksStrings.stringIDs.Add(517035719U, "TextPatternsDisplayName");
			RulesTasksStrings.stringIDs.Add(1086713105U, "LinkedPredicateHeaderMatchesException");
			RulesTasksStrings.stringIDs.Add(240566931U, "EvaluationDisplayName");
			RulesTasksStrings.stringIDs.Add(37980935U, "ToDLAddressDescription");
			RulesTasksStrings.stringIDs.Add(1419022259U, "DuplicateDataClassificationSpecified");
			RulesTasksStrings.stringIDs.Add(4270036386U, "RuleDescriptionRouteMessageOutboundRequireTls");
			RulesTasksStrings.stringIDs.Add(3918497079U, "EvaluationNotEqual");
			RulesTasksStrings.stringIDs.Add(1318242726U, "StatusCodeDisplayName");
			RulesTasksStrings.stringIDs.Add(4178557944U, "ConnectorNameDescription");
			RulesTasksStrings.stringIDs.Add(2828094232U, "RuleDescriptionAndDelimiter");
			RulesTasksStrings.stringIDs.Add(1048761747U, "ADAttributeCustomAttribute14");
			RulesTasksStrings.stringIDs.Add(1514251090U, "LinkedActionApplyOME");
			RulesTasksStrings.stringIDs.Add(4137481806U, "ADAttributePhoneNumber");
			RulesTasksStrings.stringIDs.Add(1927573801U, "ADAttributeOffice");
			RulesTasksStrings.stringIDs.Add(1784539898U, "LinkedPredicateSenderInRecipientListException");
			RulesTasksStrings.stringIDs.Add(947201504U, "RuleDescriptionDisconnect");
			RulesTasksStrings.stringIDs.Add(250901884U, "RuleDescriptionOrDelimiter");
			RulesTasksStrings.stringIDs.Add(162518937U, "LinkedActionSmtpRejectMessage");
			RulesTasksStrings.stringIDs.Add(1218766792U, "LinkedPredicateAnyOfToHeaderMemberOf");
			RulesTasksStrings.stringIDs.Add(3733737272U, "ErrorInboxRuleMustHaveActions");
			RulesTasksStrings.stringIDs.Add(4170515100U, "QuarantineActionNotAvailable");
			RulesTasksStrings.stringIDs.Add(1484034422U, "LinkedPredicateAttachmentPropertyContainsWords");
			RulesTasksStrings.stringIDs.Add(2351889904U, "LinkedPredicateAnyOfToHeaderException");
			RulesTasksStrings.stringIDs.Add(3617076017U, "EventMessageDescription");
			RulesTasksStrings.stringIDs.Add(2990680076U, "LinkedPredicateAnyOfRecipientAddressContainsException");
			RulesTasksStrings.stringIDs.Add(1857974425U, "RmsTemplateDisplayName");
			RulesTasksStrings.stringIDs.Add(999677583U, "LinkedPredicateRecipientAttributeContainsException");
			RulesTasksStrings.stringIDs.Add(2463949652U, "LinkedPredicateHasClassification");
			RulesTasksStrings.stringIDs.Add(1025350601U, "InboxRuleDescriptionSentOnlyToMe");
			RulesTasksStrings.stringIDs.Add(3905367638U, "LinkedPredicateAttachmentIsPasswordProtected");
			RulesTasksStrings.stringIDs.Add(4055862009U, "PromptToUpgradeRulesFormat");
			RulesTasksStrings.stringIDs.Add(2147095286U, "NotifySenderActionRequiresMcdcPredicate");
			RulesTasksStrings.stringIDs.Add(1853592207U, "LinkedActionGenerateNotificationAction");
			RulesTasksStrings.stringIDs.Add(3677381679U, "ClassificationDisplayName");
			RulesTasksStrings.stringIDs.Add(104177927U, "LinkedActionPrependSubject");
			RulesTasksStrings.stringIDs.Add(2002903510U, "ADAttributeStreet");
			RulesTasksStrings.stringIDs.Add(3208826121U, "FromDLAddressDisplayName");
			RulesTasksStrings.stringIDs.Add(1121933765U, "LinkedPredicateMessageTypeMatchesException");
			RulesTasksStrings.stringIDs.Add(2614845688U, "ADAttributeCustomAttribute15");
			RulesTasksStrings.stringIDs.Add(3575782782U, "RuleDescriptionQuarantine");
			RulesTasksStrings.stringIDs.Add(1485874038U, "MissingDataClassificationsParameter");
			RulesTasksStrings.stringIDs.Add(2891590424U, "LinkedPredicateHasNoClassificationException");
			RulesTasksStrings.stringIDs.Add(3816854736U, "LinkedPredicateFromScope");
			RulesTasksStrings.stringIDs.Add(25634710U, "ManagementRelationshipManager");
			RulesTasksStrings.stringIDs.Add(2463135270U, "LinkedPredicateSubjectMatchesException");
			RulesTasksStrings.stringIDs.Add(3105847291U, "LinkedPredicateSentTo");
			RulesTasksStrings.stringIDs.Add(1943465529U, "LinkedPredicateAnyOfCcHeaderMemberOf");
			RulesTasksStrings.stringIDs.Add(2075495475U, "InboxRuleDescriptionFolderNotFound");
			RulesTasksStrings.stringIDs.Add(3967326684U, "PrefixDescription");
			RulesTasksStrings.stringIDs.Add(2509095413U, "EvaluatedUserSender");
			RulesTasksStrings.stringIDs.Add(1886943840U, "RuleDescriptionDeleteMessage");
			RulesTasksStrings.stringIDs.Add(2588281890U, "LinkedPredicateHasSenderOverrideException");
			RulesTasksStrings.stringIDs.Add(4011098093U, "SubTypeDisplayName");
			RulesTasksStrings.stringIDs.Add(929006655U, "ParameterVersionMismatch");
			RulesTasksStrings.stringIDs.Add(2191228215U, "LinkedPredicateManagementRelationship");
			RulesTasksStrings.stringIDs.Add(2135540736U, "LinkedActionRouteMessageOutboundConnector");
			RulesTasksStrings.stringIDs.Add(3722738631U, "LinkedActionModerateMessageByUser");
			RulesTasksStrings.stringIDs.Add(3172147964U, "LinkedActionAddToRecipient");
			RulesTasksStrings.stringIDs.Add(1516175560U, "IncidentReportContentDisplayName");
			RulesTasksStrings.stringIDs.Add(2942456627U, "LinkedPredicateSubjectContains");
			RulesTasksStrings.stringIDs.Add(790749074U, "LinkedPredicateRecipientAddressContainsWordsException");
			RulesTasksStrings.stringIDs.Add(3262572344U, "FallbackWrap");
			RulesTasksStrings.stringIDs.Add(1377545162U, "ADAttributeCustomAttribute4");
			RulesTasksStrings.stringIDs.Add(699378618U, "ADAttributeEvaluationTypeContains");
			RulesTasksStrings.stringIDs.Add(2876513949U, "ADAttributeEvaluationTypeDescription");
			RulesTasksStrings.stringIDs.Add(3627783386U, "InvalidPredicate");
			RulesTasksStrings.stringIDs.Add(3367615085U, "ADAttributeDepartment");
			RulesTasksStrings.stringIDs.Add(4233384325U, "RuleStateDisabled");
			RulesTasksStrings.stringIDs.Add(1474730269U, "LinkedPredicateSclOverException");
			RulesTasksStrings.stringIDs.Add(4156341045U, "ConnectorNameDisplayName");
			RulesTasksStrings.stringIDs.Add(1640312644U, "LinkedPredicateSentToException");
			RulesTasksStrings.stringIDs.Add(975362769U, "EnhancedStatusCodeDescription");
			RulesTasksStrings.stringIDs.Add(3680228519U, "LinkedActionDisconnect");
			RulesTasksStrings.stringIDs.Add(4260106383U, "ADAttributePOBox");
			RulesTasksStrings.stringIDs.Add(2557875829U, "RuleDescriptionRemoveOME");
			RulesTasksStrings.stringIDs.Add(4072748617U, "MessageTypeApprovalRequest");
			RulesTasksStrings.stringIDs.Add(2182511137U, "ADAttributeFaxNumber");
			RulesTasksStrings.stringIDs.Add(1453329754U, "ErrorAccessingTransportSettings");
			RulesTasksStrings.stringIDs.Add(3459736224U, "EvaluationEqual");
			RulesTasksStrings.stringIDs.Add(1008100316U, "LinkedPredicateRecipientAttributeContains");
			RulesTasksStrings.stringIDs.Add(1994464450U, "LinkedPredicateAttachmentSizeOver");
			RulesTasksStrings.stringIDs.Add(3312695260U, "LinkedPredicateFrom");
			RulesTasksStrings.stringIDs.Add(583131402U, "LinkedPredicateAnyOfToCcHeaderException");
			RulesTasksStrings.stringIDs.Add(2016017520U, "LinkedPredicateSubjectOrBodyContains");
			RulesTasksStrings.stringIDs.Add(2966559302U, "LinkedPredicateAttachmentContainsWordsException");
			RulesTasksStrings.stringIDs.Add(2052587897U, "InboxRuleDescriptionFlaggedForNoResponse");
			RulesTasksStrings.stringIDs.Add(3353805215U, "StatusCodeDescription");
			RulesTasksStrings.stringIDs.Add(1663659467U, "LinkedPredicateFromScopeException");
			RulesTasksStrings.stringIDs.Add(1157968148U, "LinkedPredicateAttachmentProcessingLimitExceeded");
			RulesTasksStrings.stringIDs.Add(428619956U, "ManagementRelationshipDirectReport");
			RulesTasksStrings.stringIDs.Add(381216251U, "ADAttributeZipCode");
			RulesTasksStrings.stringIDs.Add(4146681835U, "SetRuleSyncAcrossDifferentVersionsNeeded");
			RulesTasksStrings.stringIDs.Add(2075192559U, "IncidentReportContentDescription");
			RulesTasksStrings.stringIDs.Add(1604244669U, "InvalidIncidentReportOriginalMail");
			RulesTasksStrings.stringIDs.Add(2457055209U, "LinkedPredicateRecipientAttributeMatchesException");
			RulesTasksStrings.stringIDs.Add(2083948085U, "ClientAccessRulesAuthenticationTypeInvalid");
			RulesTasksStrings.stringIDs.Add(1419241760U, "ClassificationDescription");
			RulesTasksStrings.stringIDs.Add(6731194U, "ReportDestinationDescription");
			RulesTasksStrings.stringIDs.Add(2492349756U, "LinkedPredicateSentToMemberOf");
			RulesTasksStrings.stringIDs.Add(1882052979U, "ToScopeDescription");
			RulesTasksStrings.stringIDs.Add(3163284624U, "WordsDisplayName");
			RulesTasksStrings.stringIDs.Add(3682029584U, "ModerateActionMustBeTheOnlyAction");
			RulesTasksStrings.stringIDs.Add(2063369946U, "InvalidRejectEnhancedStatus");
			RulesTasksStrings.stringIDs.Add(2986926906U, "ADAttributeFirstName");
			RulesTasksStrings.stringIDs.Add(3927787984U, "LinkedPredicateAnyOfCcHeader");
			RulesTasksStrings.stringIDs.Add(3247824756U, "ErrorInvalidCharException");
			RulesTasksStrings.stringIDs.Add(1871050104U, "LinkedPredicateMessageSizeOver");
			RulesTasksStrings.stringIDs.Add(312772195U, "LinkedPredicateAttachmentProcessingLimitExceededException");
			RulesTasksStrings.stringIDs.Add(278098341U, "ManagementRelationshipDescription");
			RulesTasksStrings.stringIDs.Add(3355891993U, "LinkedPredicateSenderDomainIs");
			RulesTasksStrings.stringIDs.Add(2795331228U, "InternalUser");
			RulesTasksStrings.stringIDs.Add(2609648925U, "InvalidRmsTemplate");
			RulesTasksStrings.stringIDs.Add(3500719009U, "HeaderValueDisplayName");
			RulesTasksStrings.stringIDs.Add(2141139127U, "EdgeParameterNotValidOnHubRole");
			RulesTasksStrings.stringIDs.Add(1798370525U, "BccRecipientType");
			RulesTasksStrings.stringIDs.Add(2736707353U, "RejectUnlessFalsePositiveOverrideActionType");
			RulesTasksStrings.stringIDs.Add(4141158958U, "LinkedPredicateSenderAttributeContains");
			RulesTasksStrings.stringIDs.Add(2139768671U, "LinkedPredicateAttachmentHasExecutableContentPredicate");
			RulesTasksStrings.stringIDs.Add(1691025395U, "PromptToRemoveLogEventAction");
			RulesTasksStrings.stringIDs.Add(2412654301U, "ToAddressesDescription");
			RulesTasksStrings.stringIDs.Add(3606274629U, "MessageTypeSigned");
			RulesTasksStrings.stringIDs.Add(2370124961U, "RejectReasonDescription");
			RulesTasksStrings.stringIDs.Add(2333734413U, "InboxRuleDescriptionFlaggedForFYI");
			RulesTasksStrings.stringIDs.Add(2632339802U, "LinkedPredicateRecipientAttributeMatches");
			RulesTasksStrings.stringIDs.Add(1776244123U, "LinkedPredicateAnyOfRecipientAddressMatches");
			RulesTasksStrings.stringIDs.Add(3487178095U, "LinkedPredicateRecipientAddressMatchesPatterns");
			RulesTasksStrings.stringIDs.Add(1279708626U, "SclValueDisplayName");
			RulesTasksStrings.stringIDs.Add(3219452859U, "PrependDisclaimer");
			RulesTasksStrings.stringIDs.Add(1416106290U, "ClientAccessRuleSetDatacenterAdminsOnlyError");
			RulesTasksStrings.stringIDs.Add(2826669353U, "LinkedPredicateAnyOfToCcHeaderMemberOfException");
			RulesTasksStrings.stringIDs.Add(1377545165U, "ADAttributeCustomAttribute7");
			RulesTasksStrings.stringIDs.Add(1713926708U, "FromDLAddressDescription");
			RulesTasksStrings.stringIDs.Add(756934846U, "RejectReasonDisplayName");
			RulesTasksStrings.stringIDs.Add(2589988606U, "LinkedPredicateSenderIpRanges");
			RulesTasksStrings.stringIDs.Add(1489020517U, "LinkedPredicateMessageContainsDataClassifications");
			RulesTasksStrings.stringIDs.Add(976180545U, "LinkedPredicateAnyOfToHeaderMemberOfException");
			RulesTasksStrings.stringIDs.Add(900910488U, "SetAuditSeverityDescription");
			RulesTasksStrings.stringIDs.Add(3952930894U, "RuleDescriptionAttachmentIsUnsupported");
			RulesTasksStrings.stringIDs.Add(1377545164U, "ADAttributeCustomAttribute6");
			RulesTasksStrings.stringIDs.Add(2479616296U, "LinkedPredicateBetweenMemberOfException");
			RulesTasksStrings.stringIDs.Add(4226527350U, "ADAttributeCity");
			RulesTasksStrings.stringIDs.Add(3204500204U, "DlpRuleMustContainMessageContainsDataClassificationPredicate");
			RulesTasksStrings.stringIDs.Add(3673771834U, "ImportanceDescription");
			RulesTasksStrings.stringIDs.Add(436298925U, "AppendDisclaimer");
			RulesTasksStrings.stringIDs.Add(2937791153U, "NegativePriority");
			RulesTasksStrings.stringIDs.Add(503362863U, "MessageSizeDisplayName");
			RulesTasksStrings.stringIDs.Add(523804848U, "LinkedPredicateSenderAttributeMatches");
			RulesTasksStrings.stringIDs.Add(1160183589U, "LinkedPredicateFromException");
			RulesTasksStrings.stringIDs.Add(296148748U, "LinkedPredicateAnyOfCcHeaderMemberOfException");
			RulesTasksStrings.stringIDs.Add(4241674U, "LinkedPredicateManagementRelationshipException");
			RulesTasksStrings.stringIDs.Add(1296350534U, "LinkedPredicateSenderDomainIsException");
			RulesTasksStrings.stringIDs.Add(1273212758U, "ToDLAddressDisplayName");
			RulesTasksStrings.stringIDs.Add(1316087888U, "ADAttributeEvaluationTypeDisplayName");
			RulesTasksStrings.stringIDs.Add(1461448431U, "DisclaimerLocationDisplayName");
			RulesTasksStrings.stringIDs.Add(1535769152U, "ImportanceHigh");
			RulesTasksStrings.stringIDs.Add(2537686292U, "InboxRuleDescriptionStopProcessingRules");
			RulesTasksStrings.stringIDs.Add(4149032993U, "InboxRuleDescriptionIf");
			RulesTasksStrings.stringIDs.Add(1251033108U, "LinkedActionRemoveOME");
			RulesTasksStrings.stringIDs.Add(3037379396U, "RuleDescriptionAttachmentIsPasswordProtected");
			RulesTasksStrings.stringIDs.Add(89686081U, "LinkedPredicateSubjectOrBodyContainsException");
			RulesTasksStrings.stringIDs.Add(494686544U, "ADAttributeManager");
			RulesTasksStrings.stringIDs.Add(3162495226U, "ADAttributeOtherPhoneNumber");
			RulesTasksStrings.stringIDs.Add(3800656714U, "LinkedPredicateFromAddressMatchesException");
			RulesTasksStrings.stringIDs.Add(3942685886U, "EvaluationDescription");
			RulesTasksStrings.stringIDs.Add(575057689U, "PrefixDisplayName");
			RulesTasksStrings.stringIDs.Add(2936457540U, "LinkedActionRemoveHeader");
			RulesTasksStrings.stringIDs.Add(2128154505U, "LinkedPredicateSenderInRecipientList");
			RulesTasksStrings.stringIDs.Add(2438278052U, "JournalingParameterErrorGccWithOrganization");
			RulesTasksStrings.stringIDs.Add(3631693406U, "ExternalUser");
			RulesTasksStrings.stringIDs.Add(573203429U, "RuleDescriptionStopRuleProcessing");
			RulesTasksStrings.stringIDs.Add(248554005U, "ErrorInboxRuleHasNoAction");
			RulesTasksStrings.stringIDs.Add(2822159338U, "RuleDescriptionModerateMessageByManager");
			RulesTasksStrings.stringIDs.Add(3080369366U, "RuleDescriptionAttachmentProcessingLimitExceeded");
			RulesTasksStrings.stringIDs.Add(2547374239U, "DisclaimerTextDisplayName");
			RulesTasksStrings.stringIDs.Add(1704483791U, "HubParameterNotValidOnEdgeRole");
			RulesTasksStrings.stringIDs.Add(2511974477U, "LinkedActionModerateMessageByManager");
			RulesTasksStrings.stringIDs.Add(2319428331U, "ADAttributeDescription");
			RulesTasksStrings.stringIDs.Add(2845043929U, "InboxRuleDescriptionMyNameNotInToBox");
			RulesTasksStrings.stringIDs.Add(1377545168U, "ADAttributeCustomAttribute2");
			RulesTasksStrings.stringIDs.Add(777113388U, "JournalingParameterErrorGccWithoutRecipient");
			RulesTasksStrings.stringIDs.Add(4027942746U, "ADAttributeEvaluationTypeEquals");
			RulesTasksStrings.stringIDs.Add(2409541533U, "LinkedPredicateFromAddressMatches");
			RulesTasksStrings.stringIDs.Add(2388837118U, "MessageDataClassificationDisplayName");
			RulesTasksStrings.stringIDs.Add(620435488U, "FromAddressesDescription");
			RulesTasksStrings.stringIDs.Add(1680325849U, "LinkedPredicateManagerIs");
			RulesTasksStrings.stringIDs.Add(787621858U, "LinkedActionAddManagerAsRecipientType");
			RulesTasksStrings.stringIDs.Add(1452889642U, "ADAttributeUserLogonName");
			RulesTasksStrings.stringIDs.Add(863112602U, "ADAttributeNotes");
			RulesTasksStrings.stringIDs.Add(3943169064U, "InboxRuleDescriptionSubscriptionNotFound");
			RulesTasksStrings.stringIDs.Add(914353404U, "IncidentReportOriginalMailDescription");
			RulesTasksStrings.stringIDs.Add(2376119956U, "JournalingParameterErrorExpiryDateWithoutGcc");
			RulesTasksStrings.stringIDs.Add(587899315U, "WordsDescription");
			RulesTasksStrings.stringIDs.Add(1041810761U, "PromptToOverwriteRulesOnImport");
			RulesTasksStrings.stringIDs.Add(638620002U, "SenderIpRangesDescription");
			RulesTasksStrings.stringIDs.Add(3576176750U, "LinkedPredicateAttachmentHasExecutableContentPredicateException");
			RulesTasksStrings.stringIDs.Add(4116851739U, "DeleteMessageActionMustBeTheOnlyAction");
			RulesTasksStrings.stringIDs.Add(3557196794U, "LinkedPredicateAnyOfToCcHeaderMemberOf");
			RulesTasksStrings.stringIDs.Add(1505963751U, "InvalidClassification");
			RulesTasksStrings.stringIDs.Add(2003153304U, "LinkedPredicateMessageContainsDataClassificationsException");
			RulesTasksStrings.stringIDs.Add(3380805386U, "InboxRuleDescriptionMessageClassificationNotFound");
			RulesTasksStrings.stringIDs.Add(2049063793U, "LinkedActionNotifySenderAction");
			RulesTasksStrings.stringIDs.Add(1070980084U, "LinkedPredicateAttachmentExtensionMatchesWords");
			RulesTasksStrings.stringIDs.Add(2365361139U, "RuleDescriptionApplyOME");
			RulesTasksStrings.stringIDs.Add(715964235U, "ExternalPartner");
			RulesTasksStrings.stringIDs.Add(1596558929U, "LinkedPredicateAttachmentSizeOverException");
			RulesTasksStrings.stringIDs.Add(562852627U, "LinkedPredicateADAttributeComparisonException");
			RulesTasksStrings.stringIDs.Add(3544120613U, "MessageTypeEncrypted");
			RulesTasksStrings.stringIDs.Add(1324690146U, "SenderNotificationTypeDescription");
			RulesTasksStrings.stringIDs.Add(2303954899U, "SenderNotificationTypeDisplayName");
			RulesTasksStrings.stringIDs.Add(3806797692U, "LinkedPredicateMessageTypeMatches");
			RulesTasksStrings.stringIDs.Add(1457839961U, "ADAttributeHomePhoneNumber");
			RulesTasksStrings.stringIDs.Add(2334278303U, "LinkedPredicateFromAddressContains");
			RulesTasksStrings.stringIDs.Add(4162282226U, "LinkedPredicateSubjectOrBodyMatches");
			RulesTasksStrings.stringIDs.Add(2891753468U, "ADAttributeCompany");
			RulesTasksStrings.stringIDs.Add(2030715989U, "EvaluatedUserRecipient");
			RulesTasksStrings.stringIDs.Add(816385242U, "RuleDescriptionAttachmentHasExecutableContent");
			RulesTasksStrings.stringIDs.Add(890082467U, "LinkedPredicateHasClassificationException");
			RulesTasksStrings.stringIDs.Add(398851013U, "LinkedPredicateAnyOfRecipientAddressContains");
			RulesTasksStrings.stringIDs.Add(952041456U, "LinkedPredicateSclOver");
			RulesTasksStrings.stringIDs.Add(1485258685U, "LinkedActionDeleteMessage");
			RulesTasksStrings.stringIDs.Add(3943754367U, "RuleDescriptionDisclaimerIgnoreFallback");
			RulesTasksStrings.stringIDs.Add(3708149033U, "RejectMessageActionMustBeTheOnlyAction");
			RulesTasksStrings.stringIDs.Add(1708203343U, "RuleDescriptionDisclaimerWrapFallback");
			RulesTasksStrings.stringIDs.Add(1663446825U, "InboxRuleDescriptionMyNameInToOrCcBox");
			RulesTasksStrings.stringIDs.Add(1780014951U, "LinkedPredicateMessageSizeOverException");
			RulesTasksStrings.stringIDs.Add(2476473719U, "CcRecipientType");
			RulesTasksStrings.stringIDs.Add(2808719285U, "ClientAccessRulesNameAlreadyInUse");
			RulesTasksStrings.stringIDs.Add(2767244755U, "LinkedPredicateContentCharacterSetContainsWordsException");
			RulesTasksStrings.stringIDs.Add(403146382U, "LinkedPredicateSubjectContainsException");
			RulesTasksStrings.stringIDs.Add(1808276634U, "ADAttributeCustomAttribute13");
			RulesTasksStrings.stringIDs.Add(2828094182U, "ToAddressesDisplayName");
			RulesTasksStrings.stringIDs.Add(3891477573U, "LinkedPredicateHasNoClassification");
			RulesTasksStrings.stringIDs.Add(2269102077U, "LinkedActionQuarantine");
			RulesTasksStrings.stringIDs.Add(2872629304U, "MessageTypePermissionControlled");
			RulesTasksStrings.stringIDs.Add(1377545175U, "ADAttributeCustomAttribute9");
			RulesTasksStrings.stringIDs.Add(4006689082U, "SubTypeDescription");
			RulesTasksStrings.stringIDs.Add(2743288863U, "LinkedPredicateSenderAttributeContainsException");
			RulesTasksStrings.stringIDs.Add(972971379U, "ReportDestinationDisplayName");
			RulesTasksStrings.stringIDs.Add(2666086208U, "InboxRuleDescriptionDeleteMessage");
			RulesTasksStrings.stringIDs.Add(893471950U, "ToScopeDisplayName");
			RulesTasksStrings.stringIDs.Add(4135645967U, "NotifySenderActionIsBeingOverridded");
			RulesTasksStrings.stringIDs.Add(1386608555U, "JournalingParameterErrorGccWithScope");
			RulesTasksStrings.stringIDs.Add(2720144322U, "RuleStateEnabled");
			RulesTasksStrings.stringIDs.Add(3836787260U, "LinkedActionRedirectMessage");
			RulesTasksStrings.stringIDs.Add(242192693U, "ADAttributeCustomAttribute12");
			RulesTasksStrings.stringIDs.Add(1453324876U, "RuleNameAlreadyExist");
			RulesTasksStrings.stringIDs.Add(1873569937U, "LinkedPredicateAttachmentIsPasswordProtectedException");
			RulesTasksStrings.stringIDs.Add(2411750738U, "ADAttributeMobileNumber");
			RulesTasksStrings.stringIDs.Add(3894352642U, "LinkedPredicateADAttributeComparison");
			RulesTasksStrings.stringIDs.Add(1954471536U, "EnhancedStatusCodeDisplayName");
			RulesTasksStrings.stringIDs.Add(889104748U, "InboxRuleDescriptionTakeActions");
			RulesTasksStrings.stringIDs.Add(3134498491U, "RejectMessageActionIsBeingOverridded");
			RulesTasksStrings.stringIDs.Add(553196496U, "ManagementRelationshipDisplayName");
			RulesTasksStrings.stringIDs.Add(403803765U, "AttachmentSizeDisplayName");
			RulesTasksStrings.stringIDs.Add(2021688564U, "ClientAccessRuleRemoveDatacenterAdminsOnlyError");
			RulesTasksStrings.stringIDs.Add(1957537238U, "InvalidMessageDataClassificationEmptyName");
			RulesTasksStrings.stringIDs.Add(498572210U, "RejectMessageActionType");
			RulesTasksStrings.stringIDs.Add(3512455487U, "InboxRuleDescriptionMyNameInCcBox");
			RulesTasksStrings.stringIDs.Add(1408423837U, "LinkedPredicateRecipientDomainIs");
			RulesTasksStrings.stringIDs.Add(3557021932U, "MissingDataClassificationsName");
			RulesTasksStrings.stringIDs.Add(3340948424U, "LinkedActionGenerateIncidentReportAction");
			RulesTasksStrings.stringIDs.Add(1466544691U, "InvalidAction");
			RulesTasksStrings.stringIDs.Add(115064938U, "MessageHeaderDescription");
			RulesTasksStrings.stringIDs.Add(742388560U, "DisclaimerTextDescription");
			RulesTasksStrings.stringIDs.Add(3609362930U, "MessageSizeDescription");
			RulesTasksStrings.stringIDs.Add(4164216509U, "SclValueDescription");
			RulesTasksStrings.stringIDs.Add(2447598924U, "RejectUnlessSilentOverrideActionType");
			RulesTasksStrings.stringIDs.Add(4271012524U, "LinkedPredicateManagerIsException");
			RulesTasksStrings.stringIDs.Add(2391892399U, "MessageDataClassificationDescription");
			RulesTasksStrings.stringIDs.Add(2953542218U, "ImportanceLow");
			RulesTasksStrings.stringIDs.Add(785480795U, "LinkedPredicateFromMemberOf");
			RulesTasksStrings.stringIDs.Add(1467203807U, "InboxRuleDescriptionOr");
			RulesTasksStrings.stringIDs.Add(2881863453U, "LinkedPredicateAttachmentIsUnsupportedException");
			RulesTasksStrings.stringIDs.Add(598779112U, "FromScopeDescription");
			RulesTasksStrings.stringIDs.Add(2155604814U, "ExternalNonPartner");
			RulesTasksStrings.stringIDs.Add(645477220U, "ADAttributeCustomAttribute11");
			RulesTasksStrings.stringIDs.Add(3309135616U, "FallbackActionDescription");
			RulesTasksStrings.stringIDs.Add(382355823U, "InboxRuleDescriptionMarkAsRead");
			RulesTasksStrings.stringIDs.Add(1118581569U, "LinkedPredicateSentToMemberOfException");
			RulesTasksStrings.stringIDs.Add(4189101821U, "LinkedPredicateWithImportanceException");
			RulesTasksStrings.stringIDs.Add(3339775592U, "ImportFileDataIsNull");
			RulesTasksStrings.stringIDs.Add(1903193717U, "MessageTypeCalendaring");
			RulesTasksStrings.stringIDs.Add(604363629U, "NotifyOnlyActionType");
		}

		public static LocalizedString LinkedPredicateHeaderMatches
		{
			get
			{
				return new LocalizedString("LinkedPredicateHeaderMatches", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetAuditSeverityDisplayName
		{
			get
			{
				return new LocalizedString("SetAuditSeverityDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeOtherHomePhoneNumber
		{
			get
			{
				return new LocalizedString("ADAttributeOtherHomePhoneNumber", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JournalingParameterErrorFullReportWithoutGcc
		{
			get
			{
				return new LocalizedString("JournalingParameterErrorFullReportWithoutGcc", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecipientTypeDescription
		{
			get
			{
				return new LocalizedString("RecipientTypeDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportConflictingParameters(string parameter1, string parameter2)
		{
			return new LocalizedString("IncidentReportConflictingParameters", RulesTasksStrings.ResourceManager, new object[]
			{
				parameter1,
				parameter2
			});
		}

		public static LocalizedString EvaluatedUserDisplayName
		{
			get
			{
				return new LocalizedString("EvaluatedUserDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateHeaderContains
		{
			get
			{
				return new LocalizedString("LinkedPredicateHeaderContains", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTypeOof
		{
			get
			{
				return new LocalizedString("MessageTypeOof", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImportanceDisplayName
		{
			get
			{
				return new LocalizedString("ImportanceDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DlpPolicyModeIsOverridenByModeParameter
		{
			get
			{
				return new LocalizedString("DlpPolicyModeIsOverridenByModeParameter", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedActionApplyHtmlDisclaimer
		{
			get
			{
				return new LocalizedString("LinkedActionApplyHtmlDisclaimer", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RejectMessageParameterWillBeIgnored
		{
			get
			{
				return new LocalizedString("RejectMessageParameterWillBeIgnored", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateRecipientAddressContainsWords
		{
			get
			{
				return new LocalizedString("LinkedPredicateRecipientAddressContainsWords", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AttachmentSizeDescription
		{
			get
			{
				return new LocalizedString("AttachmentSizeDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateRecipientAddressMatchesPatternsException
		{
			get
			{
				return new LocalizedString("LinkedPredicateRecipientAddressMatchesPatternsException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AttributeValueDescription
		{
			get
			{
				return new LocalizedString("AttributeValueDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionMoveToFolder(string folder)
		{
			return new LocalizedString("InboxRuleDescriptionMoveToFolder", RulesTasksStrings.ResourceManager, new object[]
			{
				folder
			});
		}

		public static LocalizedString RuleDescriptionRecipientInSenderList(string lists)
		{
			return new LocalizedString("RuleDescriptionRecipientInSenderList", RulesTasksStrings.ResourceManager, new object[]
			{
				lists
			});
		}

		public static LocalizedString RuleDescriptionSentToMemberOf(string addresses)
		{
			return new LocalizedString("RuleDescriptionSentToMemberOf", RulesTasksStrings.ResourceManager, new object[]
			{
				addresses
			});
		}

		public static LocalizedString LinkedPredicateAttachmentMatchesPatternsException
		{
			get
			{
				return new LocalizedString("LinkedPredicateAttachmentMatchesPatternsException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedActionLogEvent
		{
			get
			{
				return new LocalizedString("LinkedActionLogEvent", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAnyOfToHeader
		{
			get
			{
				return new LocalizedString("LinkedPredicateAnyOfToHeader", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionManagementRelationship(string relationship)
		{
			return new LocalizedString("RuleDescriptionManagementRelationship", RulesTasksStrings.ResourceManager, new object[]
			{
				relationship
			});
		}

		public static LocalizedString EvaluatedUserDescription
		{
			get
			{
				return new LocalizedString("EvaluatedUserDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionHasClassification(string classification)
		{
			return new LocalizedString("RuleDescriptionHasClassification", RulesTasksStrings.ResourceManager, new object[]
			{
				classification
			});
		}

		public static LocalizedString ADAttributeCustomAttribute1
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute1", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAttachmentPropertyContainsWordsException
		{
			get
			{
				return new LocalizedString("LinkedPredicateAttachmentPropertyContainsWordsException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedActionApplyClassification
		{
			get
			{
				return new LocalizedString("LinkedActionApplyClassification", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedActionSetScl
		{
			get
			{
				return new LocalizedString("LinkedActionSetScl", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAnyOfCcHeaderException
		{
			get
			{
				return new LocalizedString("LinkedPredicateAnyOfCcHeaderException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ArgumentNotSet
		{
			get
			{
				return new LocalizedString("ArgumentNotSet", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRuleName
		{
			get
			{
				return new LocalizedString("InvalidRuleName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionDisclaimerRejectFallback
		{
			get
			{
				return new LocalizedString("RuleDescriptionDisclaimerRejectFallback", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateFromAddressContainsException
		{
			get
			{
				return new LocalizedString("LinkedPredicateFromAddressContainsException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAttachmentNameMatchesException
		{
			get
			{
				return new LocalizedString("LinkedPredicateAttachmentNameMatchesException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeLastName
		{
			get
			{
				return new LocalizedString("ADAttributeLastName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GenerateNotificationDisplayName
		{
			get
			{
				return new LocalizedString("GenerateNotificationDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionNotifySenderNotifyOnly
		{
			get
			{
				return new LocalizedString("RuleDescriptionNotifySenderNotifyOnly", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCountry
		{
			get
			{
				return new LocalizedString("ADAttributeCountry", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageHeaderDisplayName
		{
			get
			{
				return new LocalizedString("MessageHeaderDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAttachmentMatchesPatterns
		{
			get
			{
				return new LocalizedString("LinkedPredicateAttachmentMatchesPatterns", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionFlaggedForUnsupportedAction(string action)
		{
			return new LocalizedString("InboxRuleDescriptionFlaggedForUnsupportedAction", RulesTasksStrings.ResourceManager, new object[]
			{
				action
			});
		}

		public static LocalizedString ListsDisplayName
		{
			get
			{
				return new LocalizedString("ListsDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FallbackIgnore
		{
			get
			{
				return new LocalizedString("FallbackIgnore", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAuditSeverityLevel(string severityLevel)
		{
			return new LocalizedString("InvalidAuditSeverityLevel", RulesTasksStrings.ResourceManager, new object[]
			{
				severityLevel
			});
		}

		public static LocalizedString RuleDescriptionAnyOfToCcHeaderMemberOf(string addresses)
		{
			return new LocalizedString("RuleDescriptionAnyOfToCcHeaderMemberOf", RulesTasksStrings.ResourceManager, new object[]
			{
				addresses
			});
		}

		public static LocalizedString CannotParseRuleDueToVersion(string name)
		{
			return new LocalizedString("CannotParseRuleDueToVersion", RulesTasksStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString RuleDescriptionHeaderContains(string header, string words)
		{
			return new LocalizedString("RuleDescriptionHeaderContains", RulesTasksStrings.ResourceManager, new object[]
			{
				header,
				words
			});
		}

		public static LocalizedString LinkedPredicateSenderIpRangesException
		{
			get
			{
				return new LocalizedString("LinkedPredicateSenderIpRangesException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AttributeValueDisplayName
		{
			get
			{
				return new LocalizedString("AttributeValueDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateRecipientDomainIsException
		{
			get
			{
				return new LocalizedString("LinkedPredicateRecipientDomainIsException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionHasAttachment
		{
			get
			{
				return new LocalizedString("InboxRuleDescriptionHasAttachment", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionHasSenderOverride
		{
			get
			{
				return new LocalizedString("RuleDescriptionHasSenderOverride", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomizedDsnNotConfigured(string status)
		{
			return new LocalizedString("CustomizedDsnNotConfigured", RulesTasksStrings.ResourceManager, new object[]
			{
				status
			});
		}

		public static LocalizedString LinkedPredicateRecipientInSenderListException
		{
			get
			{
				return new LocalizedString("LinkedPredicateRecipientInSenderListException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAttachmentExtensionMatchesWordsException
		{
			get
			{
				return new LocalizedString("LinkedPredicateAttachmentExtensionMatchesWordsException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImportanceNormal
		{
			get
			{
				return new LocalizedString("ImportanceNormal", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedActionCopyTo
		{
			get
			{
				return new LocalizedString("LinkedActionCopyTo", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ListsDescription
		{
			get
			{
				return new LocalizedString("ListsDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientAccessRulesFilterPropertyRequired(string name)
		{
			return new LocalizedString("ClientAccessRulesFilterPropertyRequired", RulesTasksStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString RuleDescriptionRecipientAttributeContains(string words)
		{
			return new LocalizedString("RuleDescriptionRecipientAttributeContains", RulesTasksStrings.ResourceManager, new object[]
			{
				words
			});
		}

		public static LocalizedString TextPatternsDescription
		{
			get
			{
				return new LocalizedString("TextPatternsDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionApplyRetentionPolicyTag(string policyTag)
		{
			return new LocalizedString("InboxRuleDescriptionApplyRetentionPolicyTag", RulesTasksStrings.ResourceManager, new object[]
			{
				policyTag
			});
		}

		public static LocalizedString RuleDescriptionNotifySenderRejectUnlessExplicitOverride(string rejectText, string rejectCode)
		{
			return new LocalizedString("RuleDescriptionNotifySenderRejectUnlessExplicitOverride", RulesTasksStrings.ResourceManager, new object[]
			{
				rejectText,
				rejectCode
			});
		}

		public static LocalizedString NewRuleSyncAcrossDifferentVersionsNeeded
		{
			get
			{
				return new LocalizedString("NewRuleSyncAcrossDifferentVersionsNeeded", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OutlookProtectionRuleRmsTemplateNotUnique(string name)
		{
			return new LocalizedString("OutlookProtectionRuleRmsTemplateNotUnique", RulesTasksStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString RmsTemplateDescription
		{
			get
			{
				return new LocalizedString("RmsTemplateDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionNotifySenderRejectUnlessSilentOverride(string rejectText, string rejectCode)
		{
			return new LocalizedString("RuleDescriptionNotifySenderRejectUnlessSilentOverride", RulesTasksStrings.ResourceManager, new object[]
			{
				rejectText,
				rejectCode
			});
		}

		public static LocalizedString RuleDescriptionAttachmentExtensionMatchesWords(string words)
		{
			return new LocalizedString("RuleDescriptionAttachmentExtensionMatchesWords", RulesTasksStrings.ResourceManager, new object[]
			{
				words
			});
		}

		public static LocalizedString InboxRuleDescriptionReceivedBeforeDate(string date)
		{
			return new LocalizedString("InboxRuleDescriptionReceivedBeforeDate", RulesTasksStrings.ResourceManager, new object[]
			{
				date
			});
		}

		public static LocalizedString InboxRuleDescriptionCopyToFolder(string folder)
		{
			return new LocalizedString("InboxRuleDescriptionCopyToFolder", RulesTasksStrings.ResourceManager, new object[]
			{
				folder
			});
		}

		public static LocalizedString RuleDescriptionMessageSizeOver(string size)
		{
			return new LocalizedString("RuleDescriptionMessageSizeOver", RulesTasksStrings.ResourceManager, new object[]
			{
				size
			});
		}

		public static LocalizedString LinkedPredicateRecipientInSenderList
		{
			get
			{
				return new LocalizedString("LinkedPredicateRecipientInSenderList", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeInitials
		{
			get
			{
				return new LocalizedString("ADAttributeInitials", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateHeaderContainsException
		{
			get
			{
				return new LocalizedString("LinkedPredicateHeaderContainsException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateContentCharacterSetContainsWords
		{
			get
			{
				return new LocalizedString("LinkedPredicateContentCharacterSetContainsWords", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTypeDescription
		{
			get
			{
				return new LocalizedString("MessageTypeDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionFromSubscription(string subscriptionEmailAddresses)
		{
			return new LocalizedString("InboxRuleDescriptionFromSubscription", RulesTasksStrings.ResourceManager, new object[]
			{
				subscriptionEmailAddresses
			});
		}

		public static LocalizedString ADAttributeState
		{
			get
			{
				return new LocalizedString("ADAttributeState", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FromScopeDisplayName
		{
			get
			{
				return new LocalizedString("FromScopeDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionAnd
		{
			get
			{
				return new LocalizedString("InboxRuleDescriptionAnd", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionRedirectTo(string recipients)
		{
			return new LocalizedString("InboxRuleDescriptionRedirectTo", RulesTasksStrings.ResourceManager, new object[]
			{
				recipients
			});
		}

		public static LocalizedString ADAttributeCustomAttribute8
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute8", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateSubjectMatches
		{
			get
			{
				return new LocalizedString("LinkedPredicateSubjectMatches", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTypeAutoForward
		{
			get
			{
				return new LocalizedString("MessageTypeAutoForward", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTypeReadReceipt
		{
			get
			{
				return new LocalizedString("MessageTypeReadReceipt", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedActionBlindCopyTo
		{
			get
			{
				return new LocalizedString("LinkedActionBlindCopyTo", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateSentToScopeException
		{
			get
			{
				return new LocalizedString("LinkedPredicateSentToScopeException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedActionStopRuleProcessing
		{
			get
			{
				return new LocalizedString("LinkedActionStopRuleProcessing", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAnyOfToCcHeader
		{
			get
			{
				return new LocalizedString("LinkedPredicateAnyOfToCcHeader", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionSentTo(string address)
		{
			return new LocalizedString("InboxRuleDescriptionSentTo", RulesTasksStrings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString LinkedPredicateHasSenderOverride
		{
			get
			{
				return new LocalizedString("LinkedPredicateHasSenderOverride", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RedirectRecipientType
		{
			get
			{
				return new LocalizedString("RedirectRecipientType", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionReceivedAfterDate(string date)
		{
			return new LocalizedString("InboxRuleDescriptionReceivedAfterDate", RulesTasksStrings.ResourceManager, new object[]
			{
				date
			});
		}

		public static LocalizedString ConditionIncompatibleWithNotifySenderAction(string conditionName, string subtypeName)
		{
			return new LocalizedString("ConditionIncompatibleWithNotifySenderAction", RulesTasksStrings.ResourceManager, new object[]
			{
				conditionName,
				subtypeName
			});
		}

		public static LocalizedString FallbackActionDisplayName
		{
			get
			{
				return new LocalizedString("FallbackActionDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoAction
		{
			get
			{
				return new LocalizedString("NoAction", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionFlaggedForAnyAction
		{
			get
			{
				return new LocalizedString("InboxRuleDescriptionFlaggedForAnyAction", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientAccessRulesIpPropertyRequired(string name)
		{
			return new LocalizedString("ClientAccessRulesIpPropertyRequired", RulesTasksStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString LinkedActionSetHeader
		{
			get
			{
				return new LocalizedString("LinkedActionSetHeader", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionADAttributeMatchesText(string evaluatedUser, string attribute, string evaluation, string value)
		{
			return new LocalizedString("RuleDescriptionADAttributeMatchesText", RulesTasksStrings.ResourceManager, new object[]
			{
				evaluatedUser,
				attribute,
				evaluation,
				value
			});
		}

		public static LocalizedString RuleDescriptionSenderAttributeContains(string words)
		{
			return new LocalizedString("RuleDescriptionSenderAttributeContains", RulesTasksStrings.ResourceManager, new object[]
			{
				words
			});
		}

		public static LocalizedString SenderIpRangesDisplayName
		{
			get
			{
				return new LocalizedString("SenderIpRangesDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionFromAddressMatches(string patterns)
		{
			return new LocalizedString("RuleDescriptionFromAddressMatches", RulesTasksStrings.ResourceManager, new object[]
			{
				patterns
			});
		}

		public static LocalizedString InboxRuleDescriptionFlaggedForAction(string action)
		{
			return new LocalizedString("InboxRuleDescriptionFlaggedForAction", RulesTasksStrings.ResourceManager, new object[]
			{
				action
			});
		}

		public static LocalizedString LinkedPredicateAttachmentIsUnsupported
		{
			get
			{
				return new LocalizedString("LinkedPredicateAttachmentIsUnsupported", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeName
		{
			get
			{
				return new LocalizedString("ADAttributeName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionSentToScope(string scope)
		{
			return new LocalizedString("RuleDescriptionSentToScope", RulesTasksStrings.ResourceManager, new object[]
			{
				scope
			});
		}

		public static LocalizedString EventMessageDisplayName
		{
			get
			{
				return new LocalizedString("EventMessageDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportOriginalMailnDisplayName
		{
			get
			{
				return new LocalizedString("IncidentReportOriginalMailnDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionForwardTo(string address)
		{
			return new LocalizedString("InboxRuleDescriptionForwardTo", RulesTasksStrings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString ADAttributeOtherFaxNumber
		{
			get
			{
				return new LocalizedString("ADAttributeOtherFaxNumber", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RejectUnlessExplicitOverrideActionType
		{
			get
			{
				return new LocalizedString("RejectUnlessExplicitOverrideActionType", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionHasNoClassification
		{
			get
			{
				return new LocalizedString("RuleDescriptionHasNoClassification", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionForwardAsAttachmentTo(string recipients)
		{
			return new LocalizedString("InboxRuleDescriptionForwardAsAttachmentTo", RulesTasksStrings.ResourceManager, new object[]
			{
				recipients
			});
		}

		public static LocalizedString LinkedPredicateAnyOfRecipientAddressMatchesException
		{
			get
			{
				return new LocalizedString("LinkedPredicateAnyOfRecipientAddressMatchesException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTypeDisplayName
		{
			get
			{
				return new LocalizedString("MessageTypeDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FallbackReject
		{
			get
			{
				return new LocalizedString("FallbackReject", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidMacroName(string invalidMacroName)
		{
			return new LocalizedString("InvalidMacroName", RulesTasksStrings.ResourceManager, new object[]
			{
				invalidMacroName
			});
		}

		public static LocalizedString ErrorRuleXmlTooBig(int currentSize, long maxSize)
		{
			return new LocalizedString("ErrorRuleXmlTooBig", RulesTasksStrings.ResourceManager, new object[]
			{
				currentSize,
				maxSize
			});
		}

		public static LocalizedString ADAttributeCustomAttribute10
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute10", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeEmail
		{
			get
			{
				return new LocalizedString("ADAttributeEmail", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCustomAttribute5
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute5", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateSubjectOrBodyMatchesException
		{
			get
			{
				return new LocalizedString("LinkedPredicateSubjectOrBodyMatchesException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionMyNameInToBox
		{
			get
			{
				return new LocalizedString("InboxRuleDescriptionMyNameInToBox", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionModerateMessageByUser(string addresses)
		{
			return new LocalizedString("RuleDescriptionModerateMessageByUser", RulesTasksStrings.ResourceManager, new object[]
			{
				addresses
			});
		}

		public static LocalizedString ToRecipientType
		{
			get
			{
				return new LocalizedString("ToRecipientType", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAttachmentContainsWords
		{
			get
			{
				return new LocalizedString("LinkedPredicateAttachmentContainsWords", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedActionRightsProtectMessage
		{
			get
			{
				return new LocalizedString("LinkedActionRightsProtectMessage", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedActionSetAuditSeverity
		{
			get
			{
				return new LocalizedString("LinkedActionSetAuditSeverity", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateBetweenMemberOf
		{
			get
			{
				return new LocalizedString("LinkedPredicateBetweenMemberOf", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleStateParameterValueIsInconsistentWithDlpPolicyState(string enabledParameterName)
		{
			return new LocalizedString("RuleStateParameterValueIsInconsistentWithDlpPolicyState", RulesTasksStrings.ResourceManager, new object[]
			{
				enabledParameterName
			});
		}

		public static LocalizedString RuleDescriptionAttachmentMatchesPatterns(string patterns)
		{
			return new LocalizedString("RuleDescriptionAttachmentMatchesPatterns", RulesTasksStrings.ResourceManager, new object[]
			{
				patterns
			});
		}

		public static LocalizedString RemoveRuleSyncAcrossDifferentVersionsNeeded
		{
			get
			{
				return new LocalizedString("RemoveRuleSyncAcrossDifferentVersionsNeeded", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeDisplayName
		{
			get
			{
				return new LocalizedString("ADAttributeDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateSentToScope
		{
			get
			{
				return new LocalizedString("LinkedPredicateSentToScope", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DisclaimerLocationDescription
		{
			get
			{
				return new LocalizedString("DisclaimerLocationDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FromAddressesDisplayName
		{
			get
			{
				return new LocalizedString("FromAddressesDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionReceivedBetweenDates(string before, string after)
		{
			return new LocalizedString("InboxRuleDescriptionReceivedBetweenDates", RulesTasksStrings.ResourceManager, new object[]
			{
				before,
				after
			});
		}

		public static LocalizedString LinkedActionRejectMessage
		{
			get
			{
				return new LocalizedString("LinkedActionRejectMessage", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateSenderAttributeMatchesException
		{
			get
			{
				return new LocalizedString("LinkedPredicateSenderAttributeMatchesException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedActionRouteMessageOutboundRequireTls
		{
			get
			{
				return new LocalizedString("LinkedActionRouteMessageOutboundRequireTls", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAttachmentNameMatches
		{
			get
			{
				return new LocalizedString("LinkedPredicateAttachmentNameMatches", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HeaderValueDescription
		{
			get
			{
				return new LocalizedString("HeaderValueDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecipientTypeDisplayName
		{
			get
			{
				return new LocalizedString("RecipientTypeDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateFromMemberOfException
		{
			get
			{
				return new LocalizedString("LinkedPredicateFromMemberOfException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionBodyContainsWords(string words)
		{
			return new LocalizedString("InboxRuleDescriptionBodyContainsWords", RulesTasksStrings.ResourceManager, new object[]
			{
				words
			});
		}

		public static LocalizedString ADAttributeCustomAttribute3
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute3", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionSubjectOrBodyContainsWords(string words)
		{
			return new LocalizedString("InboxRuleDescriptionSubjectOrBodyContainsWords", RulesTasksStrings.ResourceManager, new object[]
			{
				words
			});
		}

		public static LocalizedString ADAttributePagerNumber
		{
			get
			{
				return new LocalizedString("ADAttributePagerNumber", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTypeVoicemail
		{
			get
			{
				return new LocalizedString("MessageTypeVoicemail", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateWithImportance
		{
			get
			{
				return new LocalizedString("LinkedPredicateWithImportance", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GenerateNotificationDescription
		{
			get
			{
				return new LocalizedString("GenerateNotificationDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MacroNameNotSpecified(string attribute)
		{
			return new LocalizedString("MacroNameNotSpecified", RulesTasksStrings.ResourceManager, new object[]
			{
				attribute
			});
		}

		public static LocalizedString RuleDescriptionBlindCopyTo(string recipients)
		{
			return new LocalizedString("RuleDescriptionBlindCopyTo", RulesTasksStrings.ResourceManager, new object[]
			{
				recipients
			});
		}

		public static LocalizedString ADAttributeTitle
		{
			get
			{
				return new LocalizedString("ADAttributeTitle", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TextPatternsDisplayName
		{
			get
			{
				return new LocalizedString("TextPatternsDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OutlookProtectionRuleRmsTemplateNotFound(string name)
		{
			return new LocalizedString("OutlookProtectionRuleRmsTemplateNotFound", RulesTasksStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString RuleDescriptionPrependSubject(string prefix)
		{
			return new LocalizedString("RuleDescriptionPrependSubject", RulesTasksStrings.ResourceManager, new object[]
			{
				prefix
			});
		}

		public static LocalizedString LinkedPredicateHeaderMatchesException
		{
			get
			{
				return new LocalizedString("LinkedPredicateHeaderMatchesException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EvaluationDisplayName
		{
			get
			{
				return new LocalizedString("EvaluationDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ToDLAddressDescription
		{
			get
			{
				return new LocalizedString("ToDLAddressDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DuplicateDataClassificationSpecified
		{
			get
			{
				return new LocalizedString("DuplicateDataClassificationSpecified", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionRouteMessageOutboundRequireTls
		{
			get
			{
				return new LocalizedString("RuleDescriptionRouteMessageOutboundRequireTls", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EvaluationNotEqual
		{
			get
			{
				return new LocalizedString("EvaluationNotEqual", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StatusCodeDisplayName
		{
			get
			{
				return new LocalizedString("StatusCodeDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectorNameDescription
		{
			get
			{
				return new LocalizedString("ConnectorNameDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionSentTo(string addresses)
		{
			return new LocalizedString("RuleDescriptionSentTo", RulesTasksStrings.ResourceManager, new object[]
			{
				addresses
			});
		}

		public static LocalizedString InboxRuleDescriptionWithSensitivity(string sensitivity)
		{
			return new LocalizedString("InboxRuleDescriptionWithSensitivity", RulesTasksStrings.ResourceManager, new object[]
			{
				sensitivity
			});
		}

		public static LocalizedString RuleDescriptionAndDelimiter
		{
			get
			{
				return new LocalizedString("RuleDescriptionAndDelimiter", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCustomAttribute14
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute14", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedActionApplyOME
		{
			get
			{
				return new LocalizedString("LinkedActionApplyOME", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientAccessRulesUsernamePatternRequired(string name)
		{
			return new LocalizedString("ClientAccessRulesUsernamePatternRequired", RulesTasksStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ADAttributePhoneNumber
		{
			get
			{
				return new LocalizedString("ADAttributePhoneNumber", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeOffice
		{
			get
			{
				return new LocalizedString("ADAttributeOffice", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateSenderInRecipientListException
		{
			get
			{
				return new LocalizedString("LinkedPredicateSenderInRecipientListException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTooManyRegexCharsInRuleCollection(int currentChars, long maxChars)
		{
			return new LocalizedString("ErrorTooManyRegexCharsInRuleCollection", RulesTasksStrings.ResourceManager, new object[]
			{
				currentChars,
				maxChars
			});
		}

		public static LocalizedString NoSmtpAddressForRecipientId(string recipId)
		{
			return new LocalizedString("NoSmtpAddressForRecipientId", RulesTasksStrings.ResourceManager, new object[]
			{
				recipId
			});
		}

		public static LocalizedString RuleDescriptionDisconnect
		{
			get
			{
				return new LocalizedString("RuleDescriptionDisconnect", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionOrDelimiter
		{
			get
			{
				return new LocalizedString("RuleDescriptionOrDelimiter", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionCopyTo(string recipients)
		{
			return new LocalizedString("RuleDescriptionCopyTo", RulesTasksStrings.ResourceManager, new object[]
			{
				recipients
			});
		}

		public static LocalizedString RuleDescriptionGenerateIncidentReport(string reportDestination, string includeOriginalMail)
		{
			return new LocalizedString("RuleDescriptionGenerateIncidentReport", RulesTasksStrings.ResourceManager, new object[]
			{
				reportDestination,
				includeOriginalMail
			});
		}

		public static LocalizedString LinkedActionSmtpRejectMessage
		{
			get
			{
				return new LocalizedString("LinkedActionSmtpRejectMessage", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAnyOfToHeaderMemberOf
		{
			get
			{
				return new LocalizedString("LinkedPredicateAnyOfToHeaderMemberOf", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInboxRuleMustHaveActions
		{
			get
			{
				return new LocalizedString("ErrorInboxRuleMustHaveActions", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuarantineActionNotAvailable
		{
			get
			{
				return new LocalizedString("QuarantineActionNotAvailable", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionFrom(string address)
		{
			return new LocalizedString("InboxRuleDescriptionFrom", RulesTasksStrings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString LinkedPredicateAttachmentPropertyContainsWords
		{
			get
			{
				return new LocalizedString("LinkedPredicateAttachmentPropertyContainsWords", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAnyOfToHeaderException
		{
			get
			{
				return new LocalizedString("LinkedPredicateAnyOfToHeaderException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EventMessageDescription
		{
			get
			{
				return new LocalizedString("EventMessageDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAnyOfRecipientAddressContainsException
		{
			get
			{
				return new LocalizedString("LinkedPredicateAnyOfRecipientAddressContainsException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RmsTemplateDisplayName
		{
			get
			{
				return new LocalizedString("RmsTemplateDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateRecipientAttributeContainsException
		{
			get
			{
				return new LocalizedString("LinkedPredicateRecipientAttributeContainsException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionSetAuditSeverity(string severityLevel)
		{
			return new LocalizedString("RuleDescriptionSetAuditSeverity", RulesTasksStrings.ResourceManager, new object[]
			{
				severityLevel
			});
		}

		public static LocalizedString AttachmentMetadataParameterContainsEmptyWords(string input)
		{
			return new LocalizedString("AttachmentMetadataParameterContainsEmptyWords", RulesTasksStrings.ResourceManager, new object[]
			{
				input
			});
		}

		public static LocalizedString LinkedPredicateHasClassification
		{
			get
			{
				return new LocalizedString("LinkedPredicateHasClassification", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionSentOnlyToMe
		{
			get
			{
				return new LocalizedString("InboxRuleDescriptionSentOnlyToMe", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAttachmentIsPasswordProtected
		{
			get
			{
				return new LocalizedString("LinkedPredicateAttachmentIsPasswordProtected", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PromptToUpgradeRulesFormat
		{
			get
			{
				return new LocalizedString("PromptToUpgradeRulesFormat", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotifySenderActionRequiresMcdcPredicate
		{
			get
			{
				return new LocalizedString("NotifySenderActionRequiresMcdcPredicate", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedActionGenerateNotificationAction
		{
			get
			{
				return new LocalizedString("LinkedActionGenerateNotificationAction", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClassificationDisplayName
		{
			get
			{
				return new LocalizedString("ClassificationDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedActionPrependSubject
		{
			get
			{
				return new LocalizedString("LinkedActionPrependSubject", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSmtpAddress(string address)
		{
			return new LocalizedString("InvalidSmtpAddress", RulesTasksStrings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString ADAttributeStreet
		{
			get
			{
				return new LocalizedString("ADAttributeStreet", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FromDLAddressDisplayName
		{
			get
			{
				return new LocalizedString("FromDLAddressDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateMessageTypeMatchesException
		{
			get
			{
				return new LocalizedString("LinkedPredicateMessageTypeMatchesException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCustomAttribute15
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute15", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionQuarantine
		{
			get
			{
				return new LocalizedString("RuleDescriptionQuarantine", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingDataClassificationsParameter
		{
			get
			{
				return new LocalizedString("MissingDataClassificationsParameter", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionWithImportance(string importance)
		{
			return new LocalizedString("InboxRuleDescriptionWithImportance", RulesTasksStrings.ResourceManager, new object[]
			{
				importance
			});
		}

		public static LocalizedString RuleDescriptionApplyClassification(string classification)
		{
			return new LocalizedString("RuleDescriptionApplyClassification", RulesTasksStrings.ResourceManager, new object[]
			{
				classification
			});
		}

		public static LocalizedString LinkedPredicateHasNoClassificationException
		{
			get
			{
				return new LocalizedString("LinkedPredicateHasNoClassificationException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateFromScope
		{
			get
			{
				return new LocalizedString("LinkedPredicateFromScope", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionAnyOfToHeader(string addresses)
		{
			return new LocalizedString("RuleDescriptionAnyOfToHeader", RulesTasksStrings.ResourceManager, new object[]
			{
				addresses
			});
		}

		public static LocalizedString ManagementRelationshipManager
		{
			get
			{
				return new LocalizedString("ManagementRelationshipManager", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateSubjectMatchesException
		{
			get
			{
				return new LocalizedString("LinkedPredicateSubjectMatchesException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateSentTo
		{
			get
			{
				return new LocalizedString("LinkedPredicateSentTo", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAnyOfCcHeaderMemberOf
		{
			get
			{
				return new LocalizedString("LinkedPredicateAnyOfCcHeaderMemberOf", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionFolderNotFound
		{
			get
			{
				return new LocalizedString("InboxRuleDescriptionFolderNotFound", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PrefixDescription
		{
			get
			{
				return new LocalizedString("PrefixDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionAttachmentPropertyContainsWords(string words)
		{
			return new LocalizedString("RuleDescriptionAttachmentPropertyContainsWords", RulesTasksStrings.ResourceManager, new object[]
			{
				words
			});
		}

		public static LocalizedString TestClientAccessRuleUserNotFoundOrMoreThanOne(string name)
		{
			return new LocalizedString("TestClientAccessRuleUserNotFoundOrMoreThanOne", RulesTasksStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString InboxRuleDescriptionWithSizeBetween(string min, string max)
		{
			return new LocalizedString("InboxRuleDescriptionWithSizeBetween", RulesTasksStrings.ResourceManager, new object[]
			{
				min,
				max
			});
		}

		public static LocalizedString EvaluatedUserSender
		{
			get
			{
				return new LocalizedString("EvaluatedUserSender", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionDeleteMessage
		{
			get
			{
				return new LocalizedString("RuleDescriptionDeleteMessage", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRecipient(string recipient)
		{
			return new LocalizedString("InvalidRecipient", RulesTasksStrings.ResourceManager, new object[]
			{
				recipient
			});
		}

		public static LocalizedString LinkedPredicateHasSenderOverrideException
		{
			get
			{
				return new LocalizedString("LinkedPredicateHasSenderOverrideException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubTypeDisplayName
		{
			get
			{
				return new LocalizedString("SubTypeDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ParameterVersionMismatch
		{
			get
			{
				return new LocalizedString("ParameterVersionMismatch", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateManagementRelationship
		{
			get
			{
				return new LocalizedString("LinkedPredicateManagementRelationship", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedActionRouteMessageOutboundConnector
		{
			get
			{
				return new LocalizedString("LinkedActionRouteMessageOutboundConnector", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HeaderNameNotAllowed(string headerName)
		{
			return new LocalizedString("HeaderNameNotAllowed", RulesTasksStrings.ResourceManager, new object[]
			{
				headerName
			});
		}

		public static LocalizedString LinkedActionModerateMessageByUser
		{
			get
			{
				return new LocalizedString("LinkedActionModerateMessageByUser", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionSendTextMessageNotificationTo(string address)
		{
			return new LocalizedString("InboxRuleDescriptionSendTextMessageNotificationTo", RulesTasksStrings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString LinkedActionAddToRecipient
		{
			get
			{
				return new LocalizedString("LinkedActionAddToRecipient", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportContentDisplayName
		{
			get
			{
				return new LocalizedString("IncidentReportContentDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionAnyOfToCcHeader(string addresses)
		{
			return new LocalizedString("RuleDescriptionAnyOfToCcHeader", RulesTasksStrings.ResourceManager, new object[]
			{
				addresses
			});
		}

		public static LocalizedString LinkedPredicateSubjectContains
		{
			get
			{
				return new LocalizedString("LinkedPredicateSubjectContains", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionRouteMessageOutboundConnector(string connectorName)
		{
			return new LocalizedString("RuleDescriptionRouteMessageOutboundConnector", RulesTasksStrings.ResourceManager, new object[]
			{
				connectorName
			});
		}

		public static LocalizedString LinkedPredicateRecipientAddressContainsWordsException
		{
			get
			{
				return new LocalizedString("LinkedPredicateRecipientAddressContainsWordsException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FallbackWrap
		{
			get
			{
				return new LocalizedString("FallbackWrap", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCustomAttribute4
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute4", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionHeaderMatches(string header, string patterns)
		{
			return new LocalizedString("RuleDescriptionHeaderMatches", RulesTasksStrings.ResourceManager, new object[]
			{
				header,
				patterns
			});
		}

		public static LocalizedString ADAttributeEvaluationTypeContains
		{
			get
			{
				return new LocalizedString("ADAttributeEvaluationTypeContains", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeEvaluationTypeDescription
		{
			get
			{
				return new LocalizedString("ADAttributeEvaluationTypeDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidPredicate
		{
			get
			{
				return new LocalizedString("InvalidPredicate", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeDepartment
		{
			get
			{
				return new LocalizedString("ADAttributeDepartment", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalScopeInvalidInDc(string scope, string stringInOrganization, string notInOrganization)
		{
			return new LocalizedString("ExternalScopeInvalidInDc", RulesTasksStrings.ResourceManager, new object[]
			{
				scope,
				stringInOrganization,
				notInOrganization
			});
		}

		public static LocalizedString RuleStateDisabled
		{
			get
			{
				return new LocalizedString("RuleStateDisabled", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateSclOverException
		{
			get
			{
				return new LocalizedString("LinkedPredicateSclOverException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectorNameDisplayName
		{
			get
			{
				return new LocalizedString("ConnectorNameDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateSentToException
		{
			get
			{
				return new LocalizedString("LinkedPredicateSentToException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EnhancedStatusCodeDescription
		{
			get
			{
				return new LocalizedString("EnhancedStatusCodeDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedActionDisconnect
		{
			get
			{
				return new LocalizedString("LinkedActionDisconnect", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributePOBox
		{
			get
			{
				return new LocalizedString("ADAttributePOBox", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionRemoveOME
		{
			get
			{
				return new LocalizedString("RuleDescriptionRemoveOME", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTypeApprovalRequest
		{
			get
			{
				return new LocalizedString("MessageTypeApprovalRequest", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionAddManagerAsRecipientType(string recipientType)
		{
			return new LocalizedString("RuleDescriptionAddManagerAsRecipientType", RulesTasksStrings.ResourceManager, new object[]
			{
				recipientType
			});
		}

		public static LocalizedString ADAttributeFaxNumber
		{
			get
			{
				return new LocalizedString("ADAttributeFaxNumber", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTooManyAddedRecipientsInRuleCollection(int currentRecipients, int maxRecipients)
		{
			return new LocalizedString("ErrorTooManyAddedRecipientsInRuleCollection", RulesTasksStrings.ResourceManager, new object[]
			{
				currentRecipients,
				maxRecipients
			});
		}

		public static LocalizedString ErrorAccessingTransportSettings
		{
			get
			{
				return new LocalizedString("ErrorAccessingTransportSettings", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EvaluationEqual
		{
			get
			{
				return new LocalizedString("EvaluationEqual", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateRecipientAttributeContains
		{
			get
			{
				return new LocalizedString("LinkedPredicateRecipientAttributeContains", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAttachmentSizeOver
		{
			get
			{
				return new LocalizedString("LinkedPredicateAttachmentSizeOver", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateFrom
		{
			get
			{
				return new LocalizedString("LinkedPredicateFrom", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAnyOfToCcHeaderException
		{
			get
			{
				return new LocalizedString("LinkedPredicateAnyOfToCcHeaderException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CommentsHaveInvalidChars(int ch)
		{
			return new LocalizedString("CommentsHaveInvalidChars", RulesTasksStrings.ResourceManager, new object[]
			{
				ch
			});
		}

		public static LocalizedString InvalidMessageDataClassificationParameterMinGreaterThanMax(string paramName1, string paramName2)
		{
			return new LocalizedString("InvalidMessageDataClassificationParameterMinGreaterThanMax", RulesTasksStrings.ResourceManager, new object[]
			{
				paramName1,
				paramName2
			});
		}

		public static LocalizedString LinkedPredicateSubjectOrBodyContains
		{
			get
			{
				return new LocalizedString("LinkedPredicateSubjectOrBodyContains", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAttachmentContainsWordsException
		{
			get
			{
				return new LocalizedString("LinkedPredicateAttachmentContainsWordsException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionAttachmentContainsWords(string words)
		{
			return new LocalizedString("RuleDescriptionAttachmentContainsWords", RulesTasksStrings.ResourceManager, new object[]
			{
				words
			});
		}

		public static LocalizedString RuleDescriptionPrependHtmlDisclaimer(string text)
		{
			return new LocalizedString("RuleDescriptionPrependHtmlDisclaimer", RulesTasksStrings.ResourceManager, new object[]
			{
				text
			});
		}

		public static LocalizedString RuleDescriptionFromScope(string scope)
		{
			return new LocalizedString("RuleDescriptionFromScope", RulesTasksStrings.ResourceManager, new object[]
			{
				scope
			});
		}

		public static LocalizedString InboxRuleDescriptionFlaggedForNoResponse
		{
			get
			{
				return new LocalizedString("InboxRuleDescriptionFlaggedForNoResponse", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StatusCodeDescription
		{
			get
			{
				return new LocalizedString("StatusCodeDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionContentCharacterSetContainsWords(string words)
		{
			return new LocalizedString("RuleDescriptionContentCharacterSetContainsWords", RulesTasksStrings.ResourceManager, new object[]
			{
				words
			});
		}

		public static LocalizedString LinkedPredicateFromScopeException
		{
			get
			{
				return new LocalizedString("LinkedPredicateFromScopeException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAttachmentProcessingLimitExceeded
		{
			get
			{
				return new LocalizedString("LinkedPredicateAttachmentProcessingLimitExceeded", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ManagementRelationshipDirectReport
		{
			get
			{
				return new LocalizedString("ManagementRelationshipDirectReport", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeZipCode
		{
			get
			{
				return new LocalizedString("ADAttributeZipCode", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetRuleSyncAcrossDifferentVersionsNeeded
		{
			get
			{
				return new LocalizedString("SetRuleSyncAcrossDifferentVersionsNeeded", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportContentDescription
		{
			get
			{
				return new LocalizedString("IncidentReportContentDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidIncidentReportOriginalMail
		{
			get
			{
				return new LocalizedString("InvalidIncidentReportOriginalMail", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateRecipientAttributeMatchesException
		{
			get
			{
				return new LocalizedString("LinkedPredicateRecipientAttributeMatchesException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientAccessRulesAuthenticationTypeInvalid
		{
			get
			{
				return new LocalizedString("ClientAccessRulesAuthenticationTypeInvalid", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClassificationDescription
		{
			get
			{
				return new LocalizedString("ClassificationDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportDestinationDescription
		{
			get
			{
				return new LocalizedString("ReportDestinationDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateSentToMemberOf
		{
			get
			{
				return new LocalizedString("LinkedPredicateSentToMemberOf", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoRecipientsForRecipientId(string recipId)
		{
			return new LocalizedString("NoRecipientsForRecipientId", RulesTasksStrings.ResourceManager, new object[]
			{
				recipId
			});
		}

		public static LocalizedString ToScopeDescription
		{
			get
			{
				return new LocalizedString("ToScopeDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionMessageContainsDataClassifications(string lists)
		{
			return new LocalizedString("RuleDescriptionMessageContainsDataClassifications", RulesTasksStrings.ResourceManager, new object[]
			{
				lists
			});
		}

		public static LocalizedString AttachmentMetadataPropertyNotSpecified(string input)
		{
			return new LocalizedString("AttachmentMetadataPropertyNotSpecified", RulesTasksStrings.ResourceManager, new object[]
			{
				input
			});
		}

		public static LocalizedString WordsDisplayName
		{
			get
			{
				return new LocalizedString("WordsDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ModerateActionMustBeTheOnlyAction
		{
			get
			{
				return new LocalizedString("ModerateActionMustBeTheOnlyAction", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRejectEnhancedStatus
		{
			get
			{
				return new LocalizedString("InvalidRejectEnhancedStatus", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeFirstName
		{
			get
			{
				return new LocalizedString("ADAttributeFirstName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAnyOfCcHeader
		{
			get
			{
				return new LocalizedString("LinkedPredicateAnyOfCcHeader", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidCharException
		{
			get
			{
				return new LocalizedString("ErrorInvalidCharException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateMessageSizeOver
		{
			get
			{
				return new LocalizedString("LinkedPredicateMessageSizeOver", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAttachmentProcessingLimitExceededException
		{
			get
			{
				return new LocalizedString("LinkedPredicateAttachmentProcessingLimitExceededException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ManagementRelationshipDescription
		{
			get
			{
				return new LocalizedString("ManagementRelationshipDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateSenderDomainIs
		{
			get
			{
				return new LocalizedString("LinkedPredicateSenderDomainIs", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InternalUser
		{
			get
			{
				return new LocalizedString("InternalUser", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRmsTemplate
		{
			get
			{
				return new LocalizedString("InvalidRmsTemplate", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HeaderValueDisplayName
		{
			get
			{
				return new LocalizedString("HeaderValueDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionFrom(string addresses)
		{
			return new LocalizedString("RuleDescriptionFrom", RulesTasksStrings.ResourceManager, new object[]
			{
				addresses
			});
		}

		public static LocalizedString EdgeParameterNotValidOnHubRole
		{
			get
			{
				return new LocalizedString("EdgeParameterNotValidOnHubRole", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BccRecipientType
		{
			get
			{
				return new LocalizedString("BccRecipientType", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionGenerateNotification(string content)
		{
			return new LocalizedString("RuleDescriptionGenerateNotification", RulesTasksStrings.ResourceManager, new object[]
			{
				content
			});
		}

		public static LocalizedString InboxRuleDescriptionRecipientAddressContainsWords(string words)
		{
			return new LocalizedString("InboxRuleDescriptionRecipientAddressContainsWords", RulesTasksStrings.ResourceManager, new object[]
			{
				words
			});
		}

		public static LocalizedString RejectUnlessFalsePositiveOverrideActionType
		{
			get
			{
				return new LocalizedString("RejectUnlessFalsePositiveOverrideActionType", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionAnyOfRecipientAddressContains(string words)
		{
			return new LocalizedString("RuleDescriptionAnyOfRecipientAddressContains", RulesTasksStrings.ResourceManager, new object[]
			{
				words
			});
		}

		public static LocalizedString LinkedPredicateSenderAttributeContains
		{
			get
			{
				return new LocalizedString("LinkedPredicateSenderAttributeContains", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAttachmentHasExecutableContentPredicate
		{
			get
			{
				return new LocalizedString("LinkedPredicateAttachmentHasExecutableContentPredicate", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionSetScl(string sclValue)
		{
			return new LocalizedString("RuleDescriptionSetScl", RulesTasksStrings.ResourceManager, new object[]
			{
				sclValue
			});
		}

		public static LocalizedString RuleDescriptionSubjectOrBodyMatches(string patterns)
		{
			return new LocalizedString("RuleDescriptionSubjectOrBodyMatches", RulesTasksStrings.ResourceManager, new object[]
			{
				patterns
			});
		}

		public static LocalizedString PromptToRemoveLogEventAction
		{
			get
			{
				return new LocalizedString("PromptToRemoveLogEventAction", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ToAddressesDescription
		{
			get
			{
				return new LocalizedString("ToAddressesDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTypeSigned
		{
			get
			{
				return new LocalizedString("MessageTypeSigned", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionSubjectMatches(string patterns)
		{
			return new LocalizedString("RuleDescriptionSubjectMatches", RulesTasksStrings.ResourceManager, new object[]
			{
				patterns
			});
		}

		public static LocalizedString RejectReasonDescription
		{
			get
			{
				return new LocalizedString("RejectReasonDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionFlaggedForFYI
		{
			get
			{
				return new LocalizedString("InboxRuleDescriptionFlaggedForFYI", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateRecipientAttributeMatches
		{
			get
			{
				return new LocalizedString("LinkedPredicateRecipientAttributeMatches", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAnyOfRecipientAddressMatches
		{
			get
			{
				return new LocalizedString("LinkedPredicateAnyOfRecipientAddressMatches", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateRecipientAddressMatchesPatterns
		{
			get
			{
				return new LocalizedString("LinkedPredicateRecipientAddressMatchesPatterns", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SclValueDisplayName
		{
			get
			{
				return new LocalizedString("SclValueDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionRejectMessage(string rejectText, string rejectCode)
		{
			return new LocalizedString("RuleDescriptionRejectMessage", RulesTasksStrings.ResourceManager, new object[]
			{
				rejectText,
				rejectCode
			});
		}

		public static LocalizedString RuleDescriptionNotifySenderRejectMessage(string rejectText, string rejectCode)
		{
			return new LocalizedString("RuleDescriptionNotifySenderRejectMessage", RulesTasksStrings.ResourceManager, new object[]
			{
				rejectText,
				rejectCode
			});
		}

		public static LocalizedString PrependDisclaimer
		{
			get
			{
				return new LocalizedString("PrependDisclaimer", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientAccessRuleSetDatacenterAdminsOnlyError
		{
			get
			{
				return new LocalizedString("ClientAccessRuleSetDatacenterAdminsOnlyError", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAnyOfToCcHeaderMemberOfException
		{
			get
			{
				return new LocalizedString("LinkedPredicateAnyOfToCcHeaderMemberOfException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCustomAttribute7
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute7", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AtatchmentExtensionMatchesWordsParameterContainsWordsThatStartWithDot(string predicateName, string Words)
		{
			return new LocalizedString("AtatchmentExtensionMatchesWordsParameterContainsWordsThatStartWithDot", RulesTasksStrings.ResourceManager, new object[]
			{
				predicateName,
				Words
			});
		}

		public static LocalizedString InvalidMessageDataClassificationParameterLessThanOne(string paramName)
		{
			return new LocalizedString("InvalidMessageDataClassificationParameterLessThanOne", RulesTasksStrings.ResourceManager, new object[]
			{
				paramName
			});
		}

		public static LocalizedString FromDLAddressDescription
		{
			get
			{
				return new LocalizedString("FromDLAddressDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RejectReasonDisplayName
		{
			get
			{
				return new LocalizedString("RejectReasonDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateSenderIpRanges
		{
			get
			{
				return new LocalizedString("LinkedPredicateSenderIpRanges", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateMessageContainsDataClassifications
		{
			get
			{
				return new LocalizedString("LinkedPredicateMessageContainsDataClassifications", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMaxParameterLengthExceeded(string parameterName, int maxValueLength)
		{
			return new LocalizedString("ErrorMaxParameterLengthExceeded", RulesTasksStrings.ResourceManager, new object[]
			{
				parameterName,
				maxValueLength
			});
		}

		public static LocalizedString RuleDescriptionManagerIs(string evaluatesUser, string addresses)
		{
			return new LocalizedString("RuleDescriptionManagerIs", RulesTasksStrings.ResourceManager, new object[]
			{
				evaluatesUser,
				addresses
			});
		}

		public static LocalizedString InvalidDisclaimerMacroName(string invalidMacroName)
		{
			return new LocalizedString("InvalidDisclaimerMacroName", RulesTasksStrings.ResourceManager, new object[]
			{
				invalidMacroName
			});
		}

		public static LocalizedString LinkedPredicateAnyOfToHeaderMemberOfException
		{
			get
			{
				return new LocalizedString("LinkedPredicateAnyOfToHeaderMemberOfException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetAuditSeverityDescription
		{
			get
			{
				return new LocalizedString("SetAuditSeverityDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionRecipientAddressContains(string words)
		{
			return new LocalizedString("RuleDescriptionRecipientAddressContains", RulesTasksStrings.ResourceManager, new object[]
			{
				words
			});
		}

		public static LocalizedString HubServerVersionNotFound(string server)
		{
			return new LocalizedString("HubServerVersionNotFound", RulesTasksStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString RuleDescriptionAttachmentIsUnsupported
		{
			get
			{
				return new LocalizedString("RuleDescriptionAttachmentIsUnsupported", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCustomAttribute6
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute6", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionSenderDomainIs(string domains)
		{
			return new LocalizedString("RuleDescriptionSenderDomainIs", RulesTasksStrings.ResourceManager, new object[]
			{
				domains
			});
		}

		public static LocalizedString LinkedPredicateBetweenMemberOfException
		{
			get
			{
				return new LocalizedString("LinkedPredicateBetweenMemberOfException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCity
		{
			get
			{
				return new LocalizedString("ADAttributeCity", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DlpRuleMustContainMessageContainsDataClassificationPredicate
		{
			get
			{
				return new LocalizedString("DlpRuleMustContainMessageContainsDataClassificationPredicate", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImportanceDescription
		{
			get
			{
				return new LocalizedString("ImportanceDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AppendDisclaimer
		{
			get
			{
				return new LocalizedString("AppendDisclaimer", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CorruptRuleCollection(string reason)
		{
			return new LocalizedString("CorruptRuleCollection", RulesTasksStrings.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString NegativePriority
		{
			get
			{
				return new LocalizedString("NegativePriority", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageSizeDisplayName
		{
			get
			{
				return new LocalizedString("MessageSizeDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionAnyOfCcHeaderMemberOf(string addresses)
		{
			return new LocalizedString("RuleDescriptionAnyOfCcHeaderMemberOf", RulesTasksStrings.ResourceManager, new object[]
			{
				addresses
			});
		}

		public static LocalizedString LinkedPredicateSenderAttributeMatches
		{
			get
			{
				return new LocalizedString("LinkedPredicateSenderAttributeMatches", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateFromException
		{
			get
			{
				return new LocalizedString("LinkedPredicateFromException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionBetweenMemberOf(string addresses, string addresses2)
		{
			return new LocalizedString("RuleDescriptionBetweenMemberOf", RulesTasksStrings.ResourceManager, new object[]
			{
				addresses,
				addresses2
			});
		}

		public static LocalizedString LinkedPredicateAnyOfCcHeaderMemberOfException
		{
			get
			{
				return new LocalizedString("LinkedPredicateAnyOfCcHeaderMemberOfException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateManagementRelationshipException
		{
			get
			{
				return new LocalizedString("LinkedPredicateManagementRelationshipException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateSenderDomainIsException
		{
			get
			{
				return new LocalizedString("LinkedPredicateSenderDomainIsException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ToDLAddressDisplayName
		{
			get
			{
				return new LocalizedString("ToDLAddressDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeEvaluationTypeDisplayName
		{
			get
			{
				return new LocalizedString("ADAttributeEvaluationTypeDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DisclaimerLocationDisplayName
		{
			get
			{
				return new LocalizedString("DisclaimerLocationDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImportanceHigh
		{
			get
			{
				return new LocalizedString("ImportanceHigh", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionStopProcessingRules
		{
			get
			{
				return new LocalizedString("InboxRuleDescriptionStopProcessingRules", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotCreateRuleDueToVersion(string name)
		{
			return new LocalizedString("CannotCreateRuleDueToVersion", RulesTasksStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString InboxRuleDescriptionIf
		{
			get
			{
				return new LocalizedString("InboxRuleDescriptionIf", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionAddToRecipient(string recipients)
		{
			return new LocalizedString("RuleDescriptionAddToRecipient", RulesTasksStrings.ResourceManager, new object[]
			{
				recipients
			});
		}

		public static LocalizedString LinkedActionRemoveOME
		{
			get
			{
				return new LocalizedString("LinkedActionRemoveOME", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionAttachmentIsPasswordProtected
		{
			get
			{
				return new LocalizedString("RuleDescriptionAttachmentIsPasswordProtected", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateSubjectOrBodyContainsException
		{
			get
			{
				return new LocalizedString("LinkedPredicateSubjectOrBodyContainsException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidMessageClassification(string classification)
		{
			return new LocalizedString("InvalidMessageClassification", RulesTasksStrings.ResourceManager, new object[]
			{
				classification
			});
		}

		public static LocalizedString RuleDescriptionRecipientAttributeMatches(string patterns)
		{
			return new LocalizedString("RuleDescriptionRecipientAttributeMatches", RulesTasksStrings.ResourceManager, new object[]
			{
				patterns
			});
		}

		public static LocalizedString ADAttributeManager
		{
			get
			{
				return new LocalizedString("ADAttributeManager", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeOtherPhoneNumber
		{
			get
			{
				return new LocalizedString("ADAttributeOtherPhoneNumber", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionWithSizeLessThan(string size)
		{
			return new LocalizedString("InboxRuleDescriptionWithSizeLessThan", RulesTasksStrings.ResourceManager, new object[]
			{
				size
			});
		}

		public static LocalizedString LinkedPredicateFromAddressMatchesException
		{
			get
			{
				return new LocalizedString("LinkedPredicateFromAddressMatchesException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleNotFound(string rule)
		{
			return new LocalizedString("RuleNotFound", RulesTasksStrings.ResourceManager, new object[]
			{
				rule
			});
		}

		public static LocalizedString EvaluationDescription
		{
			get
			{
				return new LocalizedString("EvaluationDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PrefixDisplayName
		{
			get
			{
				return new LocalizedString("PrefixDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedActionRemoveHeader
		{
			get
			{
				return new LocalizedString("LinkedActionRemoveHeader", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateSenderInRecipientList
		{
			get
			{
				return new LocalizedString("LinkedPredicateSenderInRecipientList", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientAccessRuleActionNotSupported(string action)
		{
			return new LocalizedString("ClientAccessRuleActionNotSupported", RulesTasksStrings.ResourceManager, new object[]
			{
				action
			});
		}

		public static LocalizedString JournalingParameterErrorGccWithOrganization
		{
			get
			{
				return new LocalizedString("JournalingParameterErrorGccWithOrganization", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalUser
		{
			get
			{
				return new LocalizedString("ExternalUser", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OutboundConnectorIdNotFound(string connectorIdentity)
		{
			return new LocalizedString("OutboundConnectorIdNotFound", RulesTasksStrings.ResourceManager, new object[]
			{
				connectorIdentity
			});
		}

		public static LocalizedString RuleDescriptionStopRuleProcessing
		{
			get
			{
				return new LocalizedString("RuleDescriptionStopRuleProcessing", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInboxRuleHasNoAction
		{
			get
			{
				return new LocalizedString("ErrorInboxRuleHasNoAction", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionModerateMessageByManager
		{
			get
			{
				return new LocalizedString("RuleDescriptionModerateMessageByManager", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionAttachmentProcessingLimitExceeded
		{
			get
			{
				return new LocalizedString("RuleDescriptionAttachmentProcessingLimitExceeded", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DisclaimerTextDisplayName
		{
			get
			{
				return new LocalizedString("DisclaimerTextDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HubParameterNotValidOnEdgeRole
		{
			get
			{
				return new LocalizedString("HubParameterNotValidOnEdgeRole", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedActionModerateMessageByManager
		{
			get
			{
				return new LocalizedString("LinkedActionModerateMessageByManager", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionMessageType(string type)
		{
			return new LocalizedString("InboxRuleDescriptionMessageType", RulesTasksStrings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString RuleDescriptionRemoveHeader(string headerName)
		{
			return new LocalizedString("RuleDescriptionRemoveHeader", RulesTasksStrings.ResourceManager, new object[]
			{
				headerName
			});
		}

		public static LocalizedString RuleDescriptionSubjectOrBodyContains(string words)
		{
			return new LocalizedString("RuleDescriptionSubjectOrBodyContains", RulesTasksStrings.ResourceManager, new object[]
			{
				words
			});
		}

		public static LocalizedString ServerVersionNull(string server, string rulename)
		{
			return new LocalizedString("ServerVersionNull", RulesTasksStrings.ResourceManager, new object[]
			{
				server,
				rulename
			});
		}

		public static LocalizedString ADAttributeDescription
		{
			get
			{
				return new LocalizedString("ADAttributeDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OutlookProtectionRuleClassificationNotUnique(string name)
		{
			return new LocalizedString("OutlookProtectionRuleClassificationNotUnique", RulesTasksStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString InboxRuleDescriptionMyNameNotInToBox
		{
			get
			{
				return new LocalizedString("InboxRuleDescriptionMyNameNotInToBox", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCustomAttribute2
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute2", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JournalingParameterErrorGccWithoutRecipient
		{
			get
			{
				return new LocalizedString("JournalingParameterErrorGccWithoutRecipient", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CommentsTooLong(int limit, int actual)
		{
			return new LocalizedString("CommentsTooLong", RulesTasksStrings.ResourceManager, new object[]
			{
				limit,
				actual
			});
		}

		public static LocalizedString ADAttributeEvaluationTypeEquals
		{
			get
			{
				return new LocalizedString("ADAttributeEvaluationTypeEquals", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionRedirectMessage(string addresses)
		{
			return new LocalizedString("RuleDescriptionRedirectMessage", RulesTasksStrings.ResourceManager, new object[]
			{
				addresses
			});
		}

		public static LocalizedString OutlookProtectionRuleClassificationNotFound(string name)
		{
			return new LocalizedString("OutlookProtectionRuleClassificationNotFound", RulesTasksStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString RuleDescriptionLogEvent(string message)
		{
			return new LocalizedString("RuleDescriptionLogEvent", RulesTasksStrings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString LinkedPredicateFromAddressMatches
		{
			get
			{
				return new LocalizedString("LinkedPredicateFromAddressMatches", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageDataClassificationDisplayName
		{
			get
			{
				return new LocalizedString("MessageDataClassificationDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FromAddressesDescription
		{
			get
			{
				return new LocalizedString("FromAddressesDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateManagerIs
		{
			get
			{
				return new LocalizedString("LinkedPredicateManagerIs", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionSetHeader(string headerName, string headerValue)
		{
			return new LocalizedString("RuleDescriptionSetHeader", RulesTasksStrings.ResourceManager, new object[]
			{
				headerName,
				headerValue
			});
		}

		public static LocalizedString LinkedActionAddManagerAsRecipientType
		{
			get
			{
				return new LocalizedString("LinkedActionAddManagerAsRecipientType", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionHeaderContainsWords(string words)
		{
			return new LocalizedString("InboxRuleDescriptionHeaderContainsWords", RulesTasksStrings.ResourceManager, new object[]
			{
				words
			});
		}

		public static LocalizedString ADAttributeUserLogonName
		{
			get
			{
				return new LocalizedString("ADAttributeUserLogonName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionWithImportance(string importance)
		{
			return new LocalizedString("RuleDescriptionWithImportance", RulesTasksStrings.ResourceManager, new object[]
			{
				importance
			});
		}

		public static LocalizedString ADAttributeNotes
		{
			get
			{
				return new LocalizedString("ADAttributeNotes", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionSubscriptionNotFound
		{
			get
			{
				return new LocalizedString("InboxRuleDescriptionSubscriptionNotFound", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncidentReportOriginalMailDescription
		{
			get
			{
				return new LocalizedString("IncidentReportOriginalMailDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JournalingParameterErrorExpiryDateWithoutGcc
		{
			get
			{
				return new LocalizedString("JournalingParameterErrorExpiryDateWithoutGcc", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionFromAddressContainsWords(string words)
		{
			return new LocalizedString("InboxRuleDescriptionFromAddressContainsWords", RulesTasksStrings.ResourceManager, new object[]
			{
				words
			});
		}

		public static LocalizedString WordsDescription
		{
			get
			{
				return new LocalizedString("WordsDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PromptToOverwriteRulesOnImport
		{
			get
			{
				return new LocalizedString("PromptToOverwriteRulesOnImport", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SenderIpRangesDescription
		{
			get
			{
				return new LocalizedString("SenderIpRangesDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAttachmentHasExecutableContentPredicateException
		{
			get
			{
				return new LocalizedString("LinkedPredicateAttachmentHasExecutableContentPredicateException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidMessageDataClassificationParameterConfidenceExceedsMaxAllowed(string paramName, int maxAllowedValue)
		{
			return new LocalizedString("InvalidMessageDataClassificationParameterConfidenceExceedsMaxAllowed", RulesTasksStrings.ResourceManager, new object[]
			{
				paramName,
				maxAllowedValue
			});
		}

		public static LocalizedString DeleteMessageActionMustBeTheOnlyAction
		{
			get
			{
				return new LocalizedString("DeleteMessageActionMustBeTheOnlyAction", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAnyOfToCcHeaderMemberOf
		{
			get
			{
				return new LocalizedString("LinkedPredicateAnyOfToCcHeaderMemberOf", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidClassification
		{
			get
			{
				return new LocalizedString("InvalidClassification", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateMessageContainsDataClassificationsException
		{
			get
			{
				return new LocalizedString("LinkedPredicateMessageContainsDataClassificationsException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientAccessRuleWillBeAddedToCollection(string name)
		{
			return new LocalizedString("ClientAccessRuleWillBeAddedToCollection", RulesTasksStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString InboxRuleDescriptionApplyCategory(string category)
		{
			return new LocalizedString("InboxRuleDescriptionApplyCategory", RulesTasksStrings.ResourceManager, new object[]
			{
				category
			});
		}

		public static LocalizedString InboxRuleDescriptionMessageClassificationNotFound
		{
			get
			{
				return new LocalizedString("InboxRuleDescriptionMessageClassificationNotFound", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedActionNotifySenderAction
		{
			get
			{
				return new LocalizedString("LinkedActionNotifySenderAction", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAttachmentExtensionMatchesWords
		{
			get
			{
				return new LocalizedString("LinkedPredicateAttachmentExtensionMatchesWords", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionApplyOME
		{
			get
			{
				return new LocalizedString("RuleDescriptionApplyOME", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalPartner
		{
			get
			{
				return new LocalizedString("ExternalPartner", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAttachmentSizeOverException
		{
			get
			{
				return new LocalizedString("LinkedPredicateAttachmentSizeOverException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionAttachmentSizeOver(string size)
		{
			return new LocalizedString("RuleDescriptionAttachmentSizeOver", RulesTasksStrings.ResourceManager, new object[]
			{
				size
			});
		}

		public static LocalizedString LinkedPredicateADAttributeComparisonException
		{
			get
			{
				return new LocalizedString("LinkedPredicateADAttributeComparisonException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTypeEncrypted
		{
			get
			{
				return new LocalizedString("MessageTypeEncrypted", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SenderNotificationTypeDescription
		{
			get
			{
				return new LocalizedString("SenderNotificationTypeDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientAccessRulesAuthenticationTypeRequired(string name)
		{
			return new LocalizedString("ClientAccessRulesAuthenticationTypeRequired", RulesTasksStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString RuleDescriptionSenderAttributeMatches(string patterns)
		{
			return new LocalizedString("RuleDescriptionSenderAttributeMatches", RulesTasksStrings.ResourceManager, new object[]
			{
				patterns
			});
		}

		public static LocalizedString InboxRuleDescriptionSubjectContainsWords(string words)
		{
			return new LocalizedString("InboxRuleDescriptionSubjectContainsWords", RulesTasksStrings.ResourceManager, new object[]
			{
				words
			});
		}

		public static LocalizedString SenderNotificationTypeDisplayName
		{
			get
			{
				return new LocalizedString("SenderNotificationTypeDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateMessageTypeMatches
		{
			get
			{
				return new LocalizedString("LinkedPredicateMessageTypeMatches", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeHomePhoneNumber
		{
			get
			{
				return new LocalizedString("ADAttributeHomePhoneNumber", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateFromAddressContains
		{
			get
			{
				return new LocalizedString("LinkedPredicateFromAddressContains", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionWithSizeGreaterThan(string size)
		{
			return new LocalizedString("InboxRuleDescriptionWithSizeGreaterThan", RulesTasksStrings.ResourceManager, new object[]
			{
				size
			});
		}

		public static LocalizedString LinkedPredicateSubjectOrBodyMatches
		{
			get
			{
				return new LocalizedString("LinkedPredicateSubjectOrBodyMatches", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCompany
		{
			get
			{
				return new LocalizedString("ADAttributeCompany", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EvaluatedUserRecipient
		{
			get
			{
				return new LocalizedString("EvaluatedUserRecipient", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientAccessRulesPortRangePropertyRequired(string name)
		{
			return new LocalizedString("ClientAccessRulesPortRangePropertyRequired", RulesTasksStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString RuleDescriptionAnyOfToHeaderMemberOf(string addresses)
		{
			return new LocalizedString("RuleDescriptionAnyOfToHeaderMemberOf", RulesTasksStrings.ResourceManager, new object[]
			{
				addresses
			});
		}

		public static LocalizedString RuleDescriptionRightsProtectMessage(string template)
		{
			return new LocalizedString("RuleDescriptionRightsProtectMessage", RulesTasksStrings.ResourceManager, new object[]
			{
				template
			});
		}

		public static LocalizedString RuleDescriptionAttachmentHasExecutableContent
		{
			get
			{
				return new LocalizedString("RuleDescriptionAttachmentHasExecutableContent", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateHasClassificationException
		{
			get
			{
				return new LocalizedString("LinkedPredicateHasClassificationException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionAnyOfCcHeader(string addresses)
		{
			return new LocalizedString("RuleDescriptionAnyOfCcHeader", RulesTasksStrings.ResourceManager, new object[]
			{
				addresses
			});
		}

		public static LocalizedString RuleDescriptionRecipientDomainIs(string domains)
		{
			return new LocalizedString("RuleDescriptionRecipientDomainIs", RulesTasksStrings.ResourceManager, new object[]
			{
				domains
			});
		}

		public static LocalizedString LinkedPredicateAnyOfRecipientAddressContains
		{
			get
			{
				return new LocalizedString("LinkedPredicateAnyOfRecipientAddressContains", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionAnyOfRecipientAddressMatches(string addresses)
		{
			return new LocalizedString("RuleDescriptionAnyOfRecipientAddressMatches", RulesTasksStrings.ResourceManager, new object[]
			{
				addresses
			});
		}

		public static LocalizedString RuleDescriptionApplyHtmlDisclaimer(string text)
		{
			return new LocalizedString("RuleDescriptionApplyHtmlDisclaimer", RulesTasksStrings.ResourceManager, new object[]
			{
				text
			});
		}

		public static LocalizedString ClientAccessRuleWillBeConsideredEnabled(string name)
		{
			return new LocalizedString("ClientAccessRuleWillBeConsideredEnabled", RulesTasksStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString LinkedPredicateSclOver
		{
			get
			{
				return new LocalizedString("LinkedPredicateSclOver", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedActionDeleteMessage
		{
			get
			{
				return new LocalizedString("LinkedActionDeleteMessage", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionDisclaimerIgnoreFallback
		{
			get
			{
				return new LocalizedString("RuleDescriptionDisclaimerIgnoreFallback", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionFromMemberOf(string addresses)
		{
			return new LocalizedString("RuleDescriptionFromMemberOf", RulesTasksStrings.ResourceManager, new object[]
			{
				addresses
			});
		}

		public static LocalizedString CannotModifyRuleDueToVersion(string name)
		{
			return new LocalizedString("CannotModifyRuleDueToVersion", RulesTasksStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString RejectMessageActionMustBeTheOnlyAction
		{
			get
			{
				return new LocalizedString("RejectMessageActionMustBeTheOnlyAction", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionDisclaimerWrapFallback
		{
			get
			{
				return new LocalizedString("RuleDescriptionDisclaimerWrapFallback", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionMyNameInToOrCcBox
		{
			get
			{
				return new LocalizedString("InboxRuleDescriptionMyNameInToOrCcBox", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionHasClassification(string classifications)
		{
			return new LocalizedString("InboxRuleDescriptionHasClassification", RulesTasksStrings.ResourceManager, new object[]
			{
				classifications
			});
		}

		public static LocalizedString CorruptRule(string name, string reason)
		{
			return new LocalizedString("CorruptRule", RulesTasksStrings.ResourceManager, new object[]
			{
				name,
				reason
			});
		}

		public static LocalizedString LinkedPredicateMessageSizeOverException
		{
			get
			{
				return new LocalizedString("LinkedPredicateMessageSizeOverException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CcRecipientType
		{
			get
			{
				return new LocalizedString("CcRecipientType", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TestClientAccessRuleFoundUsername(string username)
		{
			return new LocalizedString("TestClientAccessRuleFoundUsername", RulesTasksStrings.ResourceManager, new object[]
			{
				username
			});
		}

		public static LocalizedString ClientAccessRulesNameAlreadyInUse
		{
			get
			{
				return new LocalizedString("ClientAccessRulesNameAlreadyInUse", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionADAttributeComparison(string attribute, string evaluation)
		{
			return new LocalizedString("RuleDescriptionADAttributeComparison", RulesTasksStrings.ResourceManager, new object[]
			{
				attribute,
				evaluation
			});
		}

		public static LocalizedString LinkedPredicateContentCharacterSetContainsWordsException
		{
			get
			{
				return new LocalizedString("LinkedPredicateContentCharacterSetContainsWordsException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateSubjectContainsException
		{
			get
			{
				return new LocalizedString("LinkedPredicateSubjectContainsException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRecipientForModerationAction(string recipient)
		{
			return new LocalizedString("InvalidRecipientForModerationAction", RulesTasksStrings.ResourceManager, new object[]
			{
				recipient
			});
		}

		public static LocalizedString ADAttributeCustomAttribute13
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute13", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ToAddressesDisplayName
		{
			get
			{
				return new LocalizedString("ToAddressesDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateHasNoClassification
		{
			get
			{
				return new LocalizedString("LinkedPredicateHasNoClassification", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionMessageTypeMatches(string messageType)
		{
			return new LocalizedString("RuleDescriptionMessageTypeMatches", RulesTasksStrings.ResourceManager, new object[]
			{
				messageType
			});
		}

		public static LocalizedString LinkedActionQuarantine
		{
			get
			{
				return new LocalizedString("LinkedActionQuarantine", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTypePermissionControlled
		{
			get
			{
				return new LocalizedString("MessageTypePermissionControlled", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionIpRanges(string lists)
		{
			return new LocalizedString("RuleDescriptionIpRanges", RulesTasksStrings.ResourceManager, new object[]
			{
				lists
			});
		}

		public static LocalizedString ADAttributeCustomAttribute9
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute9", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubTypeDescription
		{
			get
			{
				return new LocalizedString("SubTypeDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateSenderAttributeContainsException
		{
			get
			{
				return new LocalizedString("LinkedPredicateSenderAttributeContainsException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageSetClientAccessRule(string name)
		{
			return new LocalizedString("ConfirmationMessageSetClientAccessRule", RulesTasksStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ReportDestinationDisplayName
		{
			get
			{
				return new LocalizedString("ReportDestinationDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncompleteParameterGroup(string parameter, string otherParameters)
		{
			return new LocalizedString("IncompleteParameterGroup", RulesTasksStrings.ResourceManager, new object[]
			{
				parameter,
				otherParameters
			});
		}

		public static LocalizedString InboxRuleDescriptionDeleteMessage
		{
			get
			{
				return new LocalizedString("InboxRuleDescriptionDeleteMessage", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MoreThanOneRecipientForRecipientId(string recipId)
		{
			return new LocalizedString("MoreThanOneRecipientForRecipientId", RulesTasksStrings.ResourceManager, new object[]
			{
				recipId
			});
		}

		public static LocalizedString ToScopeDisplayName
		{
			get
			{
				return new LocalizedString("ToScopeDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionNotifySenderRejectUnlessFalsePositiveOverride(string rejectText, string rejectCode)
		{
			return new LocalizedString("RuleDescriptionNotifySenderRejectUnlessFalsePositiveOverride", RulesTasksStrings.ResourceManager, new object[]
			{
				rejectText,
				rejectCode
			});
		}

		public static LocalizedString ConditionIncompatibleWithTheSubtype(string conditionName, string subtypeName)
		{
			return new LocalizedString("ConditionIncompatibleWithTheSubtype", RulesTasksStrings.ResourceManager, new object[]
			{
				conditionName,
				subtypeName
			});
		}

		public static LocalizedString NotifySenderActionIsBeingOverridded
		{
			get
			{
				return new LocalizedString("NotifySenderActionIsBeingOverridded", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientAccessRulesLimitError(int limit)
		{
			return new LocalizedString("ClientAccessRulesLimitError", RulesTasksStrings.ResourceManager, new object[]
			{
				limit
			});
		}

		public static LocalizedString JournalingParameterErrorGccWithScope
		{
			get
			{
				return new LocalizedString("JournalingParameterErrorGccWithScope", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleStateEnabled
		{
			get
			{
				return new LocalizedString("RuleStateEnabled", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionSubjectContains(string words)
		{
			return new LocalizedString("RuleDescriptionSubjectContains", RulesTasksStrings.ResourceManager, new object[]
			{
				words
			});
		}

		public static LocalizedString LinkedActionRedirectMessage
		{
			get
			{
				return new LocalizedString("LinkedActionRedirectMessage", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCustomAttribute12
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute12", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleNameAlreadyExist
		{
			get
			{
				return new LocalizedString("RuleNameAlreadyExist", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAttachmentIsPasswordProtectedException
		{
			get
			{
				return new LocalizedString("LinkedPredicateAttachmentIsPasswordProtectedException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeMobileNumber
		{
			get
			{
				return new LocalizedString("ADAttributeMobileNumber", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateADAttributeComparison
		{
			get
			{
				return new LocalizedString("LinkedPredicateADAttributeComparison", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EnhancedStatusCodeDisplayName
		{
			get
			{
				return new LocalizedString("EnhancedStatusCodeDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionMarkImportance(string importance)
		{
			return new LocalizedString("InboxRuleDescriptionMarkImportance", RulesTasksStrings.ResourceManager, new object[]
			{
				importance
			});
		}

		public static LocalizedString InboxRuleDescriptionTakeActions
		{
			get
			{
				return new LocalizedString("InboxRuleDescriptionTakeActions", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RejectMessageActionIsBeingOverridded
		{
			get
			{
				return new LocalizedString("RejectMessageActionIsBeingOverridded", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ManagementRelationshipDisplayName
		{
			get
			{
				return new LocalizedString("ManagementRelationshipDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAddedRecipientCannotBeDistributionGroup(string recipient)
		{
			return new LocalizedString("ErrorAddedRecipientCannotBeDistributionGroup", RulesTasksStrings.ResourceManager, new object[]
			{
				recipient
			});
		}

		public static LocalizedString ClientAccessRulesProtocolPropertyRequired(string name)
		{
			return new LocalizedString("ClientAccessRulesProtocolPropertyRequired", RulesTasksStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString RuleDescriptionSenderInRecipientList(string lists)
		{
			return new LocalizedString("RuleDescriptionSenderInRecipientList", RulesTasksStrings.ResourceManager, new object[]
			{
				lists
			});
		}

		public static LocalizedString ExportSkippedE15Rules(int skippedRuleCount)
		{
			return new LocalizedString("ExportSkippedE15Rules", RulesTasksStrings.ResourceManager, new object[]
			{
				skippedRuleCount
			});
		}

		public static LocalizedString AttachmentSizeDisplayName
		{
			get
			{
				return new LocalizedString("AttachmentSizeDisplayName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientAccessRuleRemoveDatacenterAdminsOnlyError
		{
			get
			{
				return new LocalizedString("ClientAccessRuleRemoveDatacenterAdminsOnlyError", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidMessageDataClassificationEmptyName
		{
			get
			{
				return new LocalizedString("InvalidMessageDataClassificationEmptyName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RejectMessageActionType
		{
			get
			{
				return new LocalizedString("RejectMessageActionType", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionMyNameInCcBox
		{
			get
			{
				return new LocalizedString("InboxRuleDescriptionMyNameInCcBox", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionRecipientAddressMatches(string addresses)
		{
			return new LocalizedString("RuleDescriptionRecipientAddressMatches", RulesTasksStrings.ResourceManager, new object[]
			{
				addresses
			});
		}

		public static LocalizedString InvalidMessageDataClassification(string dataClassification)
		{
			return new LocalizedString("InvalidMessageDataClassification", RulesTasksStrings.ResourceManager, new object[]
			{
				dataClassification
			});
		}

		public static LocalizedString LinkedPredicateRecipientDomainIs
		{
			get
			{
				return new LocalizedString("LinkedPredicateRecipientDomainIs", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTooManyTransportRules(int max)
		{
			return new LocalizedString("ErrorTooManyTransportRules", RulesTasksStrings.ResourceManager, new object[]
			{
				max
			});
		}

		public static LocalizedString MissingDataClassificationsName
		{
			get
			{
				return new LocalizedString("MissingDataClassificationsName", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedActionGenerateIncidentReportAction
		{
			get
			{
				return new LocalizedString("LinkedActionGenerateIncidentReportAction", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAction
		{
			get
			{
				return new LocalizedString("InvalidAction", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionFromAddressContains(string words)
		{
			return new LocalizedString("RuleDescriptionFromAddressContains", RulesTasksStrings.ResourceManager, new object[]
			{
				words
			});
		}

		public static LocalizedString MessageHeaderDescription
		{
			get
			{
				return new LocalizedString("MessageHeaderDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageNewClientAccessRule(string name)
		{
			return new LocalizedString("ConfirmationMessageNewClientAccessRule", RulesTasksStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString DisclaimerTextDescription
		{
			get
			{
				return new LocalizedString("DisclaimerTextDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageSizeDescription
		{
			get
			{
				return new LocalizedString("MessageSizeDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SclValueDescription
		{
			get
			{
				return new LocalizedString("SclValueDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionSclOver(string sclValue)
		{
			return new LocalizedString("RuleDescriptionSclOver", RulesTasksStrings.ResourceManager, new object[]
			{
				sclValue
			});
		}

		public static LocalizedString RejectUnlessSilentOverrideActionType
		{
			get
			{
				return new LocalizedString("RejectUnlessSilentOverrideActionType", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateManagerIsException
		{
			get
			{
				return new LocalizedString("LinkedPredicateManagerIsException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageDataClassificationDescription
		{
			get
			{
				return new LocalizedString("MessageDataClassificationDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRegex(string regex)
		{
			return new LocalizedString("InvalidRegex", RulesTasksStrings.ResourceManager, new object[]
			{
				regex
			});
		}

		public static LocalizedString ImportanceLow
		{
			get
			{
				return new LocalizedString("ImportanceLow", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateFromMemberOf
		{
			get
			{
				return new LocalizedString("LinkedPredicateFromMemberOf", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleDescriptionAttachmentNameMatches(string patterns)
		{
			return new LocalizedString("RuleDescriptionAttachmentNameMatches", RulesTasksStrings.ResourceManager, new object[]
			{
				patterns
			});
		}

		public static LocalizedString InboxRuleDescriptionOr
		{
			get
			{
				return new LocalizedString("InboxRuleDescriptionOr", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateAttachmentIsUnsupportedException
		{
			get
			{
				return new LocalizedString("LinkedPredicateAttachmentIsUnsupportedException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDlpPolicy(string dlpPolicy)
		{
			return new LocalizedString("InvalidDlpPolicy", RulesTasksStrings.ResourceManager, new object[]
			{
				dlpPolicy
			});
		}

		public static LocalizedString FromScopeDescription
		{
			get
			{
				return new LocalizedString("FromScopeDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalNonPartner
		{
			get
			{
				return new LocalizedString("ExternalNonPartner", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADAttributeCustomAttribute11
		{
			get
			{
				return new LocalizedString("ADAttributeCustomAttribute11", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FallbackActionDescription
		{
			get
			{
				return new LocalizedString("FallbackActionDescription", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionMarkAsRead
		{
			get
			{
				return new LocalizedString("InboxRuleDescriptionMarkAsRead", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfirmationMessageRemoveClientAccessRule(string name)
		{
			return new LocalizedString("ConfirmationMessageRemoveClientAccessRule", RulesTasksStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString LinkedPredicateSentToMemberOfException
		{
			get
			{
				return new LocalizedString("LinkedPredicateSentToMemberOfException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedPredicateWithImportanceException
		{
			get
			{
				return new LocalizedString("LinkedPredicateWithImportanceException", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImportFileDataIsNull
		{
			get
			{
				return new LocalizedString("ImportFileDataIsNull", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTypeCalendaring
		{
			get
			{
				return new LocalizedString("MessageTypeCalendaring", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotifyOnlyActionType
		{
			get
			{
				return new LocalizedString("NotifyOnlyActionType", RulesTasksStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(RulesTasksStrings.IDs key)
		{
			return new LocalizedString(RulesTasksStrings.stringIDs[(uint)key], RulesTasksStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(395);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Core.RulesTasksStrings", typeof(RulesTasksStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			LinkedPredicateHeaderMatches = 3817895306U,
			SetAuditSeverityDisplayName = 3690383901U,
			ADAttributeOtherHomePhoneNumber = 856583401U,
			JournalingParameterErrorFullReportWithoutGcc = 607657692U,
			RecipientTypeDescription = 3705591125U,
			EvaluatedUserDisplayName = 1840928351U,
			LinkedPredicateHeaderContains = 1484331716U,
			MessageTypeOof = 2129610537U,
			ImportanceDisplayName = 862345881U,
			DlpPolicyModeIsOverridenByModeParameter = 887494032U,
			LinkedActionApplyHtmlDisclaimer = 3832906569U,
			RejectMessageParameterWillBeIgnored = 2799440992U,
			LinkedPredicateRecipientAddressContainsWords = 4206835639U,
			AttachmentSizeDescription = 3386546304U,
			LinkedPredicateRecipientAddressMatchesPatternsException = 1966725662U,
			AttributeValueDescription = 2655586213U,
			LinkedPredicateAttachmentMatchesPatternsException = 3729435602U,
			LinkedActionLogEvent = 694309943U,
			LinkedPredicateAnyOfToHeader = 3534737571U,
			EvaluatedUserDescription = 3304155086U,
			ADAttributeCustomAttribute1 = 1377545167U,
			LinkedPredicateAttachmentPropertyContainsWordsException = 996206651U,
			LinkedActionApplyClassification = 2978999437U,
			LinkedActionSetScl = 1938376749U,
			LinkedPredicateAnyOfCcHeaderException = 2156000375U,
			ArgumentNotSet = 207684870U,
			InvalidRuleName = 2267758972U,
			RuleDescriptionDisclaimerRejectFallback = 5978988U,
			LinkedPredicateFromAddressContainsException = 2498488782U,
			LinkedPredicateAttachmentNameMatchesException = 3718292884U,
			ADAttributeLastName = 1016721882U,
			GenerateNotificationDisplayName = 151284935U,
			RuleDescriptionNotifySenderNotifyOnly = 281219183U,
			ADAttributeCountry = 3600528589U,
			MessageHeaderDisplayName = 3542666061U,
			LinkedPredicateAttachmentMatchesPatterns = 4246027971U,
			ListsDisplayName = 3208048360U,
			FallbackIgnore = 2328530086U,
			LinkedPredicateSenderIpRangesException = 77980785U,
			AttributeValueDisplayName = 763738384U,
			LinkedPredicateRecipientDomainIsException = 2380701036U,
			InboxRuleDescriptionHasAttachment = 1655133361U,
			RuleDescriptionHasSenderOverride = 2340468721U,
			LinkedPredicateRecipientInSenderListException = 2538565266U,
			LinkedPredicateAttachmentExtensionMatchesWordsException = 705865031U,
			ImportanceNormal = 3850060255U,
			LinkedActionCopyTo = 3099194133U,
			ListsDescription = 2306965519U,
			TextPatternsDescription = 1130765528U,
			NewRuleSyncAcrossDifferentVersionsNeeded = 1191033945U,
			RmsTemplateDescription = 3108375176U,
			LinkedPredicateRecipientInSenderList = 4223753465U,
			ADAttributeInitials = 2468414724U,
			LinkedPredicateHeaderContainsException = 1331793023U,
			LinkedPredicateContentCharacterSetContainsWords = 3721269126U,
			MessageTypeDescription = 1697423233U,
			ADAttributeState = 3882899654U,
			FromScopeDisplayName = 3643356345U,
			InboxRuleDescriptionAnd = 2812686069U,
			ADAttributeCustomAttribute8 = 1377545174U,
			LinkedPredicateSubjectMatches = 1720532765U,
			MessageTypeAutoForward = 2041595767U,
			MessageTypeReadReceipt = 680782013U,
			LinkedActionBlindCopyTo = 431368248U,
			LinkedPredicateSentToScopeException = 2112278218U,
			LinkedActionStopRuleProcessing = 3773694392U,
			LinkedPredicateAnyOfToCcHeader = 3956509743U,
			LinkedPredicateHasSenderOverride = 3369692937U,
			RedirectRecipientType = 2611996929U,
			FallbackActionDisplayName = 1231044387U,
			NoAction = 161711511U,
			InboxRuleDescriptionFlaggedForAnyAction = 3921523715U,
			LinkedActionSetHeader = 1645493202U,
			SenderIpRangesDisplayName = 3850705779U,
			LinkedPredicateAttachmentIsUnsupported = 2375893868U,
			ADAttributeName = 3050431750U,
			EventMessageDisplayName = 3095013874U,
			IncidentReportOriginalMailnDisplayName = 1080850367U,
			ADAttributeOtherFaxNumber = 3689464497U,
			RejectUnlessExplicitOverrideActionType = 3115100581U,
			RuleDescriptionHasNoClassification = 4231516709U,
			LinkedPredicateAnyOfRecipientAddressMatchesException = 130230884U,
			MessageTypeDisplayName = 3856180838U,
			FallbackReject = 3144351139U,
			ADAttributeCustomAttribute10 = 3374360575U,
			ADAttributeEmail = 4289093673U,
			ADAttributeCustomAttribute5 = 1377545163U,
			LinkedPredicateSubjectOrBodyMatchesException = 1301491835U,
			InboxRuleDescriptionMyNameInToBox = 2591548322U,
			ToRecipientType = 4199979286U,
			LinkedPredicateAttachmentContainsWords = 1321315595U,
			LinkedActionRightsProtectMessage = 2934581752U,
			LinkedActionSetAuditSeverity = 3527153583U,
			LinkedPredicateBetweenMemberOf = 1125919747U,
			RemoveRuleSyncAcrossDifferentVersionsNeeded = 1050054139U,
			ADAttributeDisplayName = 2569693958U,
			LinkedPredicateSentToScope = 354503089U,
			DisclaimerLocationDescription = 3455130960U,
			FromAddressesDisplayName = 1316941123U,
			LinkedActionRejectMessage = 1648356515U,
			LinkedPredicateSenderAttributeMatchesException = 328892189U,
			LinkedActionRouteMessageOutboundRequireTls = 1857695339U,
			LinkedPredicateAttachmentNameMatches = 3266871523U,
			HeaderValueDescription = 3211626450U,
			RecipientTypeDisplayName = 4103233806U,
			LinkedPredicateFromMemberOfException = 2869706848U,
			ADAttributeCustomAttribute3 = 1377545169U,
			ADAttributePagerNumber = 3850073087U,
			MessageTypeVoicemail = 117825870U,
			LinkedPredicateWithImportance = 903770574U,
			GenerateNotificationDescription = 2749090792U,
			ADAttributeTitle = 2634964433U,
			TextPatternsDisplayName = 517035719U,
			LinkedPredicateHeaderMatchesException = 1086713105U,
			EvaluationDisplayName = 240566931U,
			ToDLAddressDescription = 37980935U,
			DuplicateDataClassificationSpecified = 1419022259U,
			RuleDescriptionRouteMessageOutboundRequireTls = 4270036386U,
			EvaluationNotEqual = 3918497079U,
			StatusCodeDisplayName = 1318242726U,
			ConnectorNameDescription = 4178557944U,
			RuleDescriptionAndDelimiter = 2828094232U,
			ADAttributeCustomAttribute14 = 1048761747U,
			LinkedActionApplyOME = 1514251090U,
			ADAttributePhoneNumber = 4137481806U,
			ADAttributeOffice = 1927573801U,
			LinkedPredicateSenderInRecipientListException = 1784539898U,
			RuleDescriptionDisconnect = 947201504U,
			RuleDescriptionOrDelimiter = 250901884U,
			LinkedActionSmtpRejectMessage = 162518937U,
			LinkedPredicateAnyOfToHeaderMemberOf = 1218766792U,
			ErrorInboxRuleMustHaveActions = 3733737272U,
			QuarantineActionNotAvailable = 4170515100U,
			LinkedPredicateAttachmentPropertyContainsWords = 1484034422U,
			LinkedPredicateAnyOfToHeaderException = 2351889904U,
			EventMessageDescription = 3617076017U,
			LinkedPredicateAnyOfRecipientAddressContainsException = 2990680076U,
			RmsTemplateDisplayName = 1857974425U,
			LinkedPredicateRecipientAttributeContainsException = 999677583U,
			LinkedPredicateHasClassification = 2463949652U,
			InboxRuleDescriptionSentOnlyToMe = 1025350601U,
			LinkedPredicateAttachmentIsPasswordProtected = 3905367638U,
			PromptToUpgradeRulesFormat = 4055862009U,
			NotifySenderActionRequiresMcdcPredicate = 2147095286U,
			LinkedActionGenerateNotificationAction = 1853592207U,
			ClassificationDisplayName = 3677381679U,
			LinkedActionPrependSubject = 104177927U,
			ADAttributeStreet = 2002903510U,
			FromDLAddressDisplayName = 3208826121U,
			LinkedPredicateMessageTypeMatchesException = 1121933765U,
			ADAttributeCustomAttribute15 = 2614845688U,
			RuleDescriptionQuarantine = 3575782782U,
			MissingDataClassificationsParameter = 1485874038U,
			LinkedPredicateHasNoClassificationException = 2891590424U,
			LinkedPredicateFromScope = 3816854736U,
			ManagementRelationshipManager = 25634710U,
			LinkedPredicateSubjectMatchesException = 2463135270U,
			LinkedPredicateSentTo = 3105847291U,
			LinkedPredicateAnyOfCcHeaderMemberOf = 1943465529U,
			InboxRuleDescriptionFolderNotFound = 2075495475U,
			PrefixDescription = 3967326684U,
			EvaluatedUserSender = 2509095413U,
			RuleDescriptionDeleteMessage = 1886943840U,
			LinkedPredicateHasSenderOverrideException = 2588281890U,
			SubTypeDisplayName = 4011098093U,
			ParameterVersionMismatch = 929006655U,
			LinkedPredicateManagementRelationship = 2191228215U,
			LinkedActionRouteMessageOutboundConnector = 2135540736U,
			LinkedActionModerateMessageByUser = 3722738631U,
			LinkedActionAddToRecipient = 3172147964U,
			IncidentReportContentDisplayName = 1516175560U,
			LinkedPredicateSubjectContains = 2942456627U,
			LinkedPredicateRecipientAddressContainsWordsException = 790749074U,
			FallbackWrap = 3262572344U,
			ADAttributeCustomAttribute4 = 1377545162U,
			ADAttributeEvaluationTypeContains = 699378618U,
			ADAttributeEvaluationTypeDescription = 2876513949U,
			InvalidPredicate = 3627783386U,
			ADAttributeDepartment = 3367615085U,
			RuleStateDisabled = 4233384325U,
			LinkedPredicateSclOverException = 1474730269U,
			ConnectorNameDisplayName = 4156341045U,
			LinkedPredicateSentToException = 1640312644U,
			EnhancedStatusCodeDescription = 975362769U,
			LinkedActionDisconnect = 3680228519U,
			ADAttributePOBox = 4260106383U,
			RuleDescriptionRemoveOME = 2557875829U,
			MessageTypeApprovalRequest = 4072748617U,
			ADAttributeFaxNumber = 2182511137U,
			ErrorAccessingTransportSettings = 1453329754U,
			EvaluationEqual = 3459736224U,
			LinkedPredicateRecipientAttributeContains = 1008100316U,
			LinkedPredicateAttachmentSizeOver = 1994464450U,
			LinkedPredicateFrom = 3312695260U,
			LinkedPredicateAnyOfToCcHeaderException = 583131402U,
			LinkedPredicateSubjectOrBodyContains = 2016017520U,
			LinkedPredicateAttachmentContainsWordsException = 2966559302U,
			InboxRuleDescriptionFlaggedForNoResponse = 2052587897U,
			StatusCodeDescription = 3353805215U,
			LinkedPredicateFromScopeException = 1663659467U,
			LinkedPredicateAttachmentProcessingLimitExceeded = 1157968148U,
			ManagementRelationshipDirectReport = 428619956U,
			ADAttributeZipCode = 381216251U,
			SetRuleSyncAcrossDifferentVersionsNeeded = 4146681835U,
			IncidentReportContentDescription = 2075192559U,
			InvalidIncidentReportOriginalMail = 1604244669U,
			LinkedPredicateRecipientAttributeMatchesException = 2457055209U,
			ClientAccessRulesAuthenticationTypeInvalid = 2083948085U,
			ClassificationDescription = 1419241760U,
			ReportDestinationDescription = 6731194U,
			LinkedPredicateSentToMemberOf = 2492349756U,
			ToScopeDescription = 1882052979U,
			WordsDisplayName = 3163284624U,
			ModerateActionMustBeTheOnlyAction = 3682029584U,
			InvalidRejectEnhancedStatus = 2063369946U,
			ADAttributeFirstName = 2986926906U,
			LinkedPredicateAnyOfCcHeader = 3927787984U,
			ErrorInvalidCharException = 3247824756U,
			LinkedPredicateMessageSizeOver = 1871050104U,
			LinkedPredicateAttachmentProcessingLimitExceededException = 312772195U,
			ManagementRelationshipDescription = 278098341U,
			LinkedPredicateSenderDomainIs = 3355891993U,
			InternalUser = 2795331228U,
			InvalidRmsTemplate = 2609648925U,
			HeaderValueDisplayName = 3500719009U,
			EdgeParameterNotValidOnHubRole = 2141139127U,
			BccRecipientType = 1798370525U,
			RejectUnlessFalsePositiveOverrideActionType = 2736707353U,
			LinkedPredicateSenderAttributeContains = 4141158958U,
			LinkedPredicateAttachmentHasExecutableContentPredicate = 2139768671U,
			PromptToRemoveLogEventAction = 1691025395U,
			ToAddressesDescription = 2412654301U,
			MessageTypeSigned = 3606274629U,
			RejectReasonDescription = 2370124961U,
			InboxRuleDescriptionFlaggedForFYI = 2333734413U,
			LinkedPredicateRecipientAttributeMatches = 2632339802U,
			LinkedPredicateAnyOfRecipientAddressMatches = 1776244123U,
			LinkedPredicateRecipientAddressMatchesPatterns = 3487178095U,
			SclValueDisplayName = 1279708626U,
			PrependDisclaimer = 3219452859U,
			ClientAccessRuleSetDatacenterAdminsOnlyError = 1416106290U,
			LinkedPredicateAnyOfToCcHeaderMemberOfException = 2826669353U,
			ADAttributeCustomAttribute7 = 1377545165U,
			FromDLAddressDescription = 1713926708U,
			RejectReasonDisplayName = 756934846U,
			LinkedPredicateSenderIpRanges = 2589988606U,
			LinkedPredicateMessageContainsDataClassifications = 1489020517U,
			LinkedPredicateAnyOfToHeaderMemberOfException = 976180545U,
			SetAuditSeverityDescription = 900910488U,
			RuleDescriptionAttachmentIsUnsupported = 3952930894U,
			ADAttributeCustomAttribute6 = 1377545164U,
			LinkedPredicateBetweenMemberOfException = 2479616296U,
			ADAttributeCity = 4226527350U,
			DlpRuleMustContainMessageContainsDataClassificationPredicate = 3204500204U,
			ImportanceDescription = 3673771834U,
			AppendDisclaimer = 436298925U,
			NegativePriority = 2937791153U,
			MessageSizeDisplayName = 503362863U,
			LinkedPredicateSenderAttributeMatches = 523804848U,
			LinkedPredicateFromException = 1160183589U,
			LinkedPredicateAnyOfCcHeaderMemberOfException = 296148748U,
			LinkedPredicateManagementRelationshipException = 4241674U,
			LinkedPredicateSenderDomainIsException = 1296350534U,
			ToDLAddressDisplayName = 1273212758U,
			ADAttributeEvaluationTypeDisplayName = 1316087888U,
			DisclaimerLocationDisplayName = 1461448431U,
			ImportanceHigh = 1535769152U,
			InboxRuleDescriptionStopProcessingRules = 2537686292U,
			InboxRuleDescriptionIf = 4149032993U,
			LinkedActionRemoveOME = 1251033108U,
			RuleDescriptionAttachmentIsPasswordProtected = 3037379396U,
			LinkedPredicateSubjectOrBodyContainsException = 89686081U,
			ADAttributeManager = 494686544U,
			ADAttributeOtherPhoneNumber = 3162495226U,
			LinkedPredicateFromAddressMatchesException = 3800656714U,
			EvaluationDescription = 3942685886U,
			PrefixDisplayName = 575057689U,
			LinkedActionRemoveHeader = 2936457540U,
			LinkedPredicateSenderInRecipientList = 2128154505U,
			JournalingParameterErrorGccWithOrganization = 2438278052U,
			ExternalUser = 3631693406U,
			RuleDescriptionStopRuleProcessing = 573203429U,
			ErrorInboxRuleHasNoAction = 248554005U,
			RuleDescriptionModerateMessageByManager = 2822159338U,
			RuleDescriptionAttachmentProcessingLimitExceeded = 3080369366U,
			DisclaimerTextDisplayName = 2547374239U,
			HubParameterNotValidOnEdgeRole = 1704483791U,
			LinkedActionModerateMessageByManager = 2511974477U,
			ADAttributeDescription = 2319428331U,
			InboxRuleDescriptionMyNameNotInToBox = 2845043929U,
			ADAttributeCustomAttribute2 = 1377545168U,
			JournalingParameterErrorGccWithoutRecipient = 777113388U,
			ADAttributeEvaluationTypeEquals = 4027942746U,
			LinkedPredicateFromAddressMatches = 2409541533U,
			MessageDataClassificationDisplayName = 2388837118U,
			FromAddressesDescription = 620435488U,
			LinkedPredicateManagerIs = 1680325849U,
			LinkedActionAddManagerAsRecipientType = 787621858U,
			ADAttributeUserLogonName = 1452889642U,
			ADAttributeNotes = 863112602U,
			InboxRuleDescriptionSubscriptionNotFound = 3943169064U,
			IncidentReportOriginalMailDescription = 914353404U,
			JournalingParameterErrorExpiryDateWithoutGcc = 2376119956U,
			WordsDescription = 587899315U,
			PromptToOverwriteRulesOnImport = 1041810761U,
			SenderIpRangesDescription = 638620002U,
			LinkedPredicateAttachmentHasExecutableContentPredicateException = 3576176750U,
			DeleteMessageActionMustBeTheOnlyAction = 4116851739U,
			LinkedPredicateAnyOfToCcHeaderMemberOf = 3557196794U,
			InvalidClassification = 1505963751U,
			LinkedPredicateMessageContainsDataClassificationsException = 2003153304U,
			InboxRuleDescriptionMessageClassificationNotFound = 3380805386U,
			LinkedActionNotifySenderAction = 2049063793U,
			LinkedPredicateAttachmentExtensionMatchesWords = 1070980084U,
			RuleDescriptionApplyOME = 2365361139U,
			ExternalPartner = 715964235U,
			LinkedPredicateAttachmentSizeOverException = 1596558929U,
			LinkedPredicateADAttributeComparisonException = 562852627U,
			MessageTypeEncrypted = 3544120613U,
			SenderNotificationTypeDescription = 1324690146U,
			SenderNotificationTypeDisplayName = 2303954899U,
			LinkedPredicateMessageTypeMatches = 3806797692U,
			ADAttributeHomePhoneNumber = 1457839961U,
			LinkedPredicateFromAddressContains = 2334278303U,
			LinkedPredicateSubjectOrBodyMatches = 4162282226U,
			ADAttributeCompany = 2891753468U,
			EvaluatedUserRecipient = 2030715989U,
			RuleDescriptionAttachmentHasExecutableContent = 816385242U,
			LinkedPredicateHasClassificationException = 890082467U,
			LinkedPredicateAnyOfRecipientAddressContains = 398851013U,
			LinkedPredicateSclOver = 952041456U,
			LinkedActionDeleteMessage = 1485258685U,
			RuleDescriptionDisclaimerIgnoreFallback = 3943754367U,
			RejectMessageActionMustBeTheOnlyAction = 3708149033U,
			RuleDescriptionDisclaimerWrapFallback = 1708203343U,
			InboxRuleDescriptionMyNameInToOrCcBox = 1663446825U,
			LinkedPredicateMessageSizeOverException = 1780014951U,
			CcRecipientType = 2476473719U,
			ClientAccessRulesNameAlreadyInUse = 2808719285U,
			LinkedPredicateContentCharacterSetContainsWordsException = 2767244755U,
			LinkedPredicateSubjectContainsException = 403146382U,
			ADAttributeCustomAttribute13 = 1808276634U,
			ToAddressesDisplayName = 2828094182U,
			LinkedPredicateHasNoClassification = 3891477573U,
			LinkedActionQuarantine = 2269102077U,
			MessageTypePermissionControlled = 2872629304U,
			ADAttributeCustomAttribute9 = 1377545175U,
			SubTypeDescription = 4006689082U,
			LinkedPredicateSenderAttributeContainsException = 2743288863U,
			ReportDestinationDisplayName = 972971379U,
			InboxRuleDescriptionDeleteMessage = 2666086208U,
			ToScopeDisplayName = 893471950U,
			NotifySenderActionIsBeingOverridded = 4135645967U,
			JournalingParameterErrorGccWithScope = 1386608555U,
			RuleStateEnabled = 2720144322U,
			LinkedActionRedirectMessage = 3836787260U,
			ADAttributeCustomAttribute12 = 242192693U,
			RuleNameAlreadyExist = 1453324876U,
			LinkedPredicateAttachmentIsPasswordProtectedException = 1873569937U,
			ADAttributeMobileNumber = 2411750738U,
			LinkedPredicateADAttributeComparison = 3894352642U,
			EnhancedStatusCodeDisplayName = 1954471536U,
			InboxRuleDescriptionTakeActions = 889104748U,
			RejectMessageActionIsBeingOverridded = 3134498491U,
			ManagementRelationshipDisplayName = 553196496U,
			AttachmentSizeDisplayName = 403803765U,
			ClientAccessRuleRemoveDatacenterAdminsOnlyError = 2021688564U,
			InvalidMessageDataClassificationEmptyName = 1957537238U,
			RejectMessageActionType = 498572210U,
			InboxRuleDescriptionMyNameInCcBox = 3512455487U,
			LinkedPredicateRecipientDomainIs = 1408423837U,
			MissingDataClassificationsName = 3557021932U,
			LinkedActionGenerateIncidentReportAction = 3340948424U,
			InvalidAction = 1466544691U,
			MessageHeaderDescription = 115064938U,
			DisclaimerTextDescription = 742388560U,
			MessageSizeDescription = 3609362930U,
			SclValueDescription = 4164216509U,
			RejectUnlessSilentOverrideActionType = 2447598924U,
			LinkedPredicateManagerIsException = 4271012524U,
			MessageDataClassificationDescription = 2391892399U,
			ImportanceLow = 2953542218U,
			LinkedPredicateFromMemberOf = 785480795U,
			InboxRuleDescriptionOr = 1467203807U,
			LinkedPredicateAttachmentIsUnsupportedException = 2881863453U,
			FromScopeDescription = 598779112U,
			ExternalNonPartner = 2155604814U,
			ADAttributeCustomAttribute11 = 645477220U,
			FallbackActionDescription = 3309135616U,
			InboxRuleDescriptionMarkAsRead = 382355823U,
			LinkedPredicateSentToMemberOfException = 1118581569U,
			LinkedPredicateWithImportanceException = 4189101821U,
			ImportFileDataIsNull = 3339775592U,
			MessageTypeCalendaring = 1903193717U,
			NotifyOnlyActionType = 604363629U
		}

		private enum ParamIDs
		{
			IncidentReportConflictingParameters,
			InboxRuleDescriptionMoveToFolder,
			RuleDescriptionRecipientInSenderList,
			RuleDescriptionSentToMemberOf,
			RuleDescriptionManagementRelationship,
			RuleDescriptionHasClassification,
			InboxRuleDescriptionFlaggedForUnsupportedAction,
			InvalidAuditSeverityLevel,
			RuleDescriptionAnyOfToCcHeaderMemberOf,
			CannotParseRuleDueToVersion,
			RuleDescriptionHeaderContains,
			CustomizedDsnNotConfigured,
			ClientAccessRulesFilterPropertyRequired,
			RuleDescriptionRecipientAttributeContains,
			InboxRuleDescriptionApplyRetentionPolicyTag,
			RuleDescriptionNotifySenderRejectUnlessExplicitOverride,
			OutlookProtectionRuleRmsTemplateNotUnique,
			RuleDescriptionNotifySenderRejectUnlessSilentOverride,
			RuleDescriptionAttachmentExtensionMatchesWords,
			InboxRuleDescriptionReceivedBeforeDate,
			InboxRuleDescriptionCopyToFolder,
			RuleDescriptionMessageSizeOver,
			InboxRuleDescriptionFromSubscription,
			InboxRuleDescriptionRedirectTo,
			InboxRuleDescriptionSentTo,
			InboxRuleDescriptionReceivedAfterDate,
			ConditionIncompatibleWithNotifySenderAction,
			ClientAccessRulesIpPropertyRequired,
			RuleDescriptionADAttributeMatchesText,
			RuleDescriptionSenderAttributeContains,
			RuleDescriptionFromAddressMatches,
			InboxRuleDescriptionFlaggedForAction,
			RuleDescriptionSentToScope,
			InboxRuleDescriptionForwardTo,
			InboxRuleDescriptionForwardAsAttachmentTo,
			InvalidMacroName,
			ErrorRuleXmlTooBig,
			RuleDescriptionModerateMessageByUser,
			RuleStateParameterValueIsInconsistentWithDlpPolicyState,
			RuleDescriptionAttachmentMatchesPatterns,
			InboxRuleDescriptionReceivedBetweenDates,
			InboxRuleDescriptionBodyContainsWords,
			InboxRuleDescriptionSubjectOrBodyContainsWords,
			MacroNameNotSpecified,
			RuleDescriptionBlindCopyTo,
			OutlookProtectionRuleRmsTemplateNotFound,
			RuleDescriptionPrependSubject,
			RuleDescriptionSentTo,
			InboxRuleDescriptionWithSensitivity,
			ClientAccessRulesUsernamePatternRequired,
			ErrorTooManyRegexCharsInRuleCollection,
			NoSmtpAddressForRecipientId,
			RuleDescriptionCopyTo,
			RuleDescriptionGenerateIncidentReport,
			InboxRuleDescriptionFrom,
			RuleDescriptionSetAuditSeverity,
			AttachmentMetadataParameterContainsEmptyWords,
			InvalidSmtpAddress,
			InboxRuleDescriptionWithImportance,
			RuleDescriptionApplyClassification,
			RuleDescriptionAnyOfToHeader,
			RuleDescriptionAttachmentPropertyContainsWords,
			TestClientAccessRuleUserNotFoundOrMoreThanOne,
			InboxRuleDescriptionWithSizeBetween,
			InvalidRecipient,
			HeaderNameNotAllowed,
			InboxRuleDescriptionSendTextMessageNotificationTo,
			RuleDescriptionAnyOfToCcHeader,
			RuleDescriptionRouteMessageOutboundConnector,
			RuleDescriptionHeaderMatches,
			ExternalScopeInvalidInDc,
			RuleDescriptionAddManagerAsRecipientType,
			ErrorTooManyAddedRecipientsInRuleCollection,
			CommentsHaveInvalidChars,
			InvalidMessageDataClassificationParameterMinGreaterThanMax,
			RuleDescriptionAttachmentContainsWords,
			RuleDescriptionPrependHtmlDisclaimer,
			RuleDescriptionFromScope,
			RuleDescriptionContentCharacterSetContainsWords,
			NoRecipientsForRecipientId,
			RuleDescriptionMessageContainsDataClassifications,
			AttachmentMetadataPropertyNotSpecified,
			RuleDescriptionFrom,
			RuleDescriptionGenerateNotification,
			InboxRuleDescriptionRecipientAddressContainsWords,
			RuleDescriptionAnyOfRecipientAddressContains,
			RuleDescriptionSetScl,
			RuleDescriptionSubjectOrBodyMatches,
			RuleDescriptionSubjectMatches,
			RuleDescriptionRejectMessage,
			RuleDescriptionNotifySenderRejectMessage,
			AtatchmentExtensionMatchesWordsParameterContainsWordsThatStartWithDot,
			InvalidMessageDataClassificationParameterLessThanOne,
			ErrorMaxParameterLengthExceeded,
			RuleDescriptionManagerIs,
			InvalidDisclaimerMacroName,
			RuleDescriptionRecipientAddressContains,
			HubServerVersionNotFound,
			RuleDescriptionSenderDomainIs,
			CorruptRuleCollection,
			RuleDescriptionAnyOfCcHeaderMemberOf,
			RuleDescriptionBetweenMemberOf,
			CannotCreateRuleDueToVersion,
			RuleDescriptionAddToRecipient,
			InvalidMessageClassification,
			RuleDescriptionRecipientAttributeMatches,
			InboxRuleDescriptionWithSizeLessThan,
			RuleNotFound,
			ClientAccessRuleActionNotSupported,
			OutboundConnectorIdNotFound,
			InboxRuleDescriptionMessageType,
			RuleDescriptionRemoveHeader,
			RuleDescriptionSubjectOrBodyContains,
			ServerVersionNull,
			OutlookProtectionRuleClassificationNotUnique,
			CommentsTooLong,
			RuleDescriptionRedirectMessage,
			OutlookProtectionRuleClassificationNotFound,
			RuleDescriptionLogEvent,
			RuleDescriptionSetHeader,
			InboxRuleDescriptionHeaderContainsWords,
			RuleDescriptionWithImportance,
			InboxRuleDescriptionFromAddressContainsWords,
			InvalidMessageDataClassificationParameterConfidenceExceedsMaxAllowed,
			ClientAccessRuleWillBeAddedToCollection,
			InboxRuleDescriptionApplyCategory,
			RuleDescriptionAttachmentSizeOver,
			ClientAccessRulesAuthenticationTypeRequired,
			RuleDescriptionSenderAttributeMatches,
			InboxRuleDescriptionSubjectContainsWords,
			InboxRuleDescriptionWithSizeGreaterThan,
			ClientAccessRulesPortRangePropertyRequired,
			RuleDescriptionAnyOfToHeaderMemberOf,
			RuleDescriptionRightsProtectMessage,
			RuleDescriptionAnyOfCcHeader,
			RuleDescriptionRecipientDomainIs,
			RuleDescriptionAnyOfRecipientAddressMatches,
			RuleDescriptionApplyHtmlDisclaimer,
			ClientAccessRuleWillBeConsideredEnabled,
			RuleDescriptionFromMemberOf,
			CannotModifyRuleDueToVersion,
			InboxRuleDescriptionHasClassification,
			CorruptRule,
			TestClientAccessRuleFoundUsername,
			RuleDescriptionADAttributeComparison,
			InvalidRecipientForModerationAction,
			RuleDescriptionMessageTypeMatches,
			RuleDescriptionIpRanges,
			ConfirmationMessageSetClientAccessRule,
			IncompleteParameterGroup,
			MoreThanOneRecipientForRecipientId,
			RuleDescriptionNotifySenderRejectUnlessFalsePositiveOverride,
			ConditionIncompatibleWithTheSubtype,
			ClientAccessRulesLimitError,
			RuleDescriptionSubjectContains,
			InboxRuleDescriptionMarkImportance,
			ErrorAddedRecipientCannotBeDistributionGroup,
			ClientAccessRulesProtocolPropertyRequired,
			RuleDescriptionSenderInRecipientList,
			ExportSkippedE15Rules,
			RuleDescriptionRecipientAddressMatches,
			InvalidMessageDataClassification,
			ErrorTooManyTransportRules,
			RuleDescriptionFromAddressContains,
			ConfirmationMessageNewClientAccessRule,
			RuleDescriptionSclOver,
			InvalidRegex,
			RuleDescriptionAttachmentNameMatches,
			InvalidDlpPolicy,
			ConfirmationMessageRemoveClientAccessRule
		}
	}
}
