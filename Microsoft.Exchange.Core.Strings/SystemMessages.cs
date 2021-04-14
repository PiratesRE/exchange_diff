using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core
{
	internal static class SystemMessages
	{
		static SystemMessages()
		{
			SystemMessages.stringIDs.Add(3060389430U, "HumanText5_2_1");
			SystemMessages.stringIDs.Add(224226320U, "ArchiveQuotaWarningSubject");
			SystemMessages.stringIDs.Add(1616176476U, "ShortText5_1_7");
			SystemMessages.stringIDs.Add(955676612U, "DSNEnhanced_5_4_4_SMTPSEND_DNS_NonExistentDomain");
			SystemMessages.stringIDs.Add(4216909332U, "DataCenterHumanText5_4_6");
			SystemMessages.stringIDs.Add(2547234435U, "ExAttachmentRemovedSenderDescription");
			SystemMessages.stringIDs.Add(3483876933U, "HumanText5_3_5");
			SystemMessages.stringIDs.Add(3038358855U, "DSNEnhanced_5_2_3_QUEUE_Priority");
			SystemMessages.stringIDs.Add(171052060U, "ShortText5_3_5");
			SystemMessages.stringIDs.Add(2132860851U, "HumanText5_4_8");
			SystemMessages.stringIDs.Add(264042572U, "QuotaWarningSubjectPF");
			SystemMessages.stringIDs.Add(1070379938U, "E4EViewMessage");
			SystemMessages.stringIDs.Add(1498176974U, "ReadSubject");
			SystemMessages.stringIDs.Add(3726703438U, "HumanReadableFinalText");
			SystemMessages.stringIDs.Add(1459475246U, "DSNEnhanced_5_2_3_RESOLVER_RST_SendSizeLimit_Sender");
			SystemMessages.stringIDs.Add(2954596652U, "ArchiveQuotaWarningTopText");
			SystemMessages.stringIDs.Add(3283016966U, "ShortText5_6_2");
			SystemMessages.stringIDs.Add(1494114401U, "DSNEnhanced_5_7_1_RESOLVER_RST_NotAuthorizedToGroup");
			SystemMessages.stringIDs.Add(286121318U, "QuotaWarningFolderHierarchyDepthSubject");
			SystemMessages.stringIDs.Add(1411446847U, "DecisionProcessedNotificationSubjectPrefix");
			SystemMessages.stringIDs.Add(4198661606U, "QuotaWarningFolderHierarchyDepthNoLimitDetails");
			SystemMessages.stringIDs.Add(3925426878U, "QuotaProhibitReceiveFolderHierarchyDepthSubject");
			SystemMessages.stringIDs.Add(4044098184U, "QuotaWarningFolderHierarchyDepthDetails");
			SystemMessages.stringIDs.Add(80797861U, "DSNEnhanced_5_6_0_RESOLVER_MT_ModerationReencrptionFailed");
			SystemMessages.stringIDs.Add(715499088U, "ShortText5_2_0");
			SystemMessages.stringIDs.Add(1234410400U, "QuotaProhibitReceiveFoldersCountDetails");
			SystemMessages.stringIDs.Add(1595973500U, "ShortText5_4_2");
			SystemMessages.stringIDs.Add(1979055051U, "ShortText5_7_2");
			SystemMessages.stringIDs.Add(3244754815U, "NotReadSubject");
			SystemMessages.stringIDs.Add(3645242410U, "HumanText5_7_0");
			SystemMessages.stringIDs.Add(2798911709U, "DSNEnhanced_5_2_3_RESOLVER_RST_SendSizeLimit_Org");
			SystemMessages.stringIDs.Add(937708661U, "HumanText5_7_10");
			SystemMessages.stringIDs.Add(634034053U, "HumanText5_5_3");
			SystemMessages.stringIDs.Add(816255637U, "ShortText5_7_4");
			SystemMessages.stringIDs.Add(2677307879U, "HumanText5_3_3");
			SystemMessages.stringIDs.Add(2038752517U, "HumanText5_1_7");
			SystemMessages.stringIDs.Add(2058955493U, "HumanText5_6_0");
			SystemMessages.stringIDs.Add(292011585U, "ShortText5_5_2");
			SystemMessages.stringIDs.Add(3633934039U, "QuotaProhibitReceiveFolderHierarchyChildrenCountSubject");
			SystemMessages.stringIDs.Add(4064869466U, "QuotaWarningSubject");
			SystemMessages.stringIDs.Add(1655670966U, "HumanText5_6_5");
			SystemMessages.stringIDs.Add(3182260417U, "ShortText5_1_6");
			SystemMessages.stringIDs.Add(2766572931U, "HumanText5_2_2_StoreNDR");
			SystemMessages.stringIDs.Add(3706504469U, "ShortText5_3_0");
			SystemMessages.stringIDs.Add(1156273137U, "ArchiveQuotaFullTopText");
			SystemMessages.stringIDs.Add(3666692194U, "DoNotForwardName");
			SystemMessages.stringIDs.Add(3686301493U, "ShortText5_6_5");
			SystemMessages.stringIDs.Add(3685029433U, "QuotaWarningMailboxMessagesPerFolderNoLimitSubject");
			SystemMessages.stringIDs.Add(1737136001U, "ShortText5_3_4");
			SystemMessages.stringIDs.Add(3625039434U, "HumanText5_6_1");
			SystemMessages.stringIDs.Add(3888161794U, "QuotaProhibitReceiveMailboxMessagesPerFolderCountDetails");
			SystemMessages.stringIDs.Add(3162057441U, "ShortText5_4_3");
			SystemMessages.stringIDs.Add(2844775350U, "RejectButtonText");
			SystemMessages.stringIDs.Add(1467823001U, "DataCenterHumanText5_1_0");
			SystemMessages.stringIDs.Add(448693229U, "ArchiveQuotaFullDetails");
			SystemMessages.stringIDs.Add(3424179467U, "ShortText5_5_0");
			SystemMessages.stringIDs.Add(513074528U, "HumanText5_7_2");
			SystemMessages.stringIDs.Add(997824090U, "ShortText5_0_0");
			SystemMessages.stringIDs.Add(3457394445U, "DataCenterHumanText5_4_1");
			SystemMessages.stringIDs.Add(2762698027U, "RelayedHumanReadableTopText");
			SystemMessages.stringIDs.Add(2043697631U, "FailedSubject");
			SystemMessages.stringIDs.Add(2357302625U, "DelayedHumanReadableTopText");
			SystemMessages.stringIDs.Add(34865187U, "QuotaMaxSize");
			SystemMessages.stringIDs.Add(2785624105U, "ShortText5_7_8");
			SystemMessages.stringIDs.Add(4194575540U, "QuotaSendTopText");
			SystemMessages.stringIDs.Add(2295532082U, "QuotaSendSubjectPF");
			SystemMessages.stringIDs.Add(2798267404U, "HumanText5_1_0");
			SystemMessages.stringIDs.Add(2435388829U, "HumanText5_7_9");
			SystemMessages.stringIDs.Add(1858095526U, "ShortText5_5_1");
			SystemMessages.stringIDs.Add(2240869372U, "QuotaProhibitReceiveMailboxMessagesPerFolderCountSubject");
			SystemMessages.stringIDs.Add(913497348U, "ExpandedHumanReadableTopText");
			SystemMessages.stringIDs.Add(2179915018U, "HumanText5_4_3");
			SystemMessages.stringIDs.Add(894078784U, "FailedHumanReadableTopText");
			SystemMessages.stringIDs.Add(3183792054U, "BodyHeaderFontTag");
			SystemMessages.stringIDs.Add(4029344418U, "BodyBlockFontTag");
			SystemMessages.stringIDs.Add(1086182386U, "DSNEnhanced_5_4_4_ROUTING_NoNextHop");
			SystemMessages.stringIDs.Add(1775266991U, "E4EDisclaimer");
			SystemMessages.stringIDs.Add(4171950547U, "ModerationNdrNotificationForSenderTopText");
			SystemMessages.stringIDs.Add(4135226003U, "ExOrarMailSenderDescription");
			SystemMessages.stringIDs.Add(777497770U, "QuotaWarningFolderHierarchyChildrenNoLimitDetails");
			SystemMessages.stringIDs.Add(1075924926U, "DSNEnhanced_5_7_1_RESOLVER_RST_AuthRequired");
			SystemMessages.stringIDs.Add(3187564900U, "E4EHeaderCustom");
			SystemMessages.stringIDs.Add(3017333248U, "DelayedSubject");
			SystemMessages.stringIDs.Add(3276713205U, "ModerationExpiryNoticationForModeratorTopText");
			SystemMessages.stringIDs.Add(3020894940U, "ShortText5_5_3");
			SystemMessages.stringIDs.Add(89587025U, "HumanText5_6_4");
			SystemMessages.stringIDs.Add(2397807267U, "E4EViewMessageOTPButton");
			SystemMessages.stringIDs.Add(3154497764U, "HumanText5_1_8");
			SystemMessages.stringIDs.Add(1062071583U, "RejectedNotificationSubjectPrefix");
			SystemMessages.stringIDs.Add(3905817172U, "QuotaWarningMailboxMessagesPerFolderCountSubject");
			SystemMessages.stringIDs.Add(1575770524U, "ShortText5_7_1");
			SystemMessages.stringIDs.Add(1494305489U, "HumanText5_2_0");
			SystemMessages.stringIDs.Add(1211980487U, "HumanText5_0_0");
			SystemMessages.stringIDs.Add(3597730735U, "EnhancedDsnTextFontTag");
			SystemMessages.stringIDs.Add(3847666970U, "ShortText5_2_2");
			SystemMessages.stringIDs.Add(273945714U, "ApprovalRequestPreview");
			SystemMessages.stringIDs.Add(613831077U, "HumanText5_4_2");
			SystemMessages.stringIDs.Add(1416892261U, "OriginalHeadersTitle");
			SystemMessages.stringIDs.Add(3886882982U, "ModeratorsOofSubjectPrefix");
			SystemMessages.stringIDs.Add(3328018042U, "ModeratorRejectTopText");
			SystemMessages.stringIDs.Add(3837274988U, "FinalTextFontTag");
			SystemMessages.stringIDs.Add(4028322787U, "ExOrarMailDisplayName");
			SystemMessages.stringIDs.Add(228586384U, "DSNEnhanced_5_7_1_RESOLVER_RST_DLNeedsSenderRestrictions");
			SystemMessages.stringIDs.Add(3547364398U, "ModeratorCommentsHeader");
			SystemMessages.stringIDs.Add(2894800102U, "ModeratorsNdrSubjectPrefix");
			SystemMessages.stringIDs.Add(2504161731U, "ReceivingServerTitle");
			SystemMessages.stringIDs.Add(3335723890U, "QuotaSendDetails");
			SystemMessages.stringIDs.Add(69384049U, "HumanText5_1_3");
			SystemMessages.stringIDs.Add(1797209243U, "QuarantinedHumanReadableTopText");
			SystemMessages.stringIDs.Add(2296356702U, "DeliveredSubject");
			SystemMessages.stringIDs.Add(4273299450U, "DSNEnhanced_5_2_3_RESOLVER_RST_RecipSizeLimit_DL");
			SystemMessages.stringIDs.Add(1203275484U, "DSNEnhanced_5_7_1_APPROVAL_NotAuthorized");
			SystemMessages.stringIDs.Add(109790001U, "HumanText5_7_5");
			SystemMessages.stringIDs.Add(4001472770U, "HumanText5_7_8");
			SystemMessages.stringIDs.Add(238766803U, "DecisionConflictNotificationSubjectPrefix");
			SystemMessages.stringIDs.Add(1830882122U, "QuotaWarningNoLimitSubjectPF");
			SystemMessages.stringIDs.Add(439284250U, "BodyDownload");
			SystemMessages.stringIDs.Add(1979593922U, "QuotaProhibitReceiveFoldersCountSubject");
			SystemMessages.stringIDs.Add(2618404892U, "ShortText5_7_100");
			SystemMessages.stringIDs.Add(2028676810U, "QuotaWarningTopText");
			SystemMessages.stringIDs.Add(2900729894U, "ShortText5_7_300");
			SystemMessages.stringIDs.Add(3949783936U, "FailedHumanReadableTopTextForTextMessageNotification");
			SystemMessages.stringIDs.Add(2592547661U, "DelayHumanReadableExplanation");
			SystemMessages.stringIDs.Add(1118783615U, "ShortText5_2_3");
			SystemMessages.stringIDs.Add(2820071495U, "InternetConfidentialDescription");
			SystemMessages.stringIDs.Add(2657104903U, "HumanText5_2_2");
			SystemMessages.stringIDs.Add(836458613U, "ShortText5_4_7");
			SystemMessages.stringIDs.Add(742696162U, "ApprovalRequestTopText");
			SystemMessages.stringIDs.Add(4102229319U, "HumanText5_4_4");
			SystemMessages.stringIDs.Add(1091303688U, "QuotaWarningFolderHierarchyDepthNoLimitSubject");
			SystemMessages.stringIDs.Add(325931173U, "ModerationOofNotificationForSenderTopText");
			SystemMessages.stringIDs.Add(76943726U, "ShortText5_4_8");
			SystemMessages.stringIDs.Add(3301930267U, "ExpandedSubject");
			SystemMessages.stringIDs.Add(4122432295U, "HumanText5_5_5");
			SystemMessages.stringIDs.Add(3033906942U, "DataCenterHumanText5_1_1");
			SystemMessages.stringIDs.Add(3806593754U, "ArchiveQuotaWarningDetails");
			SystemMessages.stringIDs.Add(1586140930U, "E4EViewMessageInfo");
			SystemMessages.stringIDs.Add(3351672821U, "DeliveredHumanReadableTopText");
			SystemMessages.stringIDs.Add(304371086U, "RelayedSubject");
			SystemMessages.stringIDs.Add(754993578U, "HumanText5_3_4");
			SystemMessages.stringIDs.Add(2281583029U, "ShortText5_2_1");
			SystemMessages.stringIDs.Add(4035994692U, "QuotaSendSubject");
			SystemMessages.stringIDs.Add(2422745530U, "ShortText5_1_1");
			SystemMessages.stringIDs.Add(3158471333U, "DiagnosticsFontTag");
			SystemMessages.stringIDs.Add(833904234U, "QuotaWarningFoldersCountNoLimitDetails");
			SystemMessages.stringIDs.Add(1675873942U, "HumanText5_7_4");
			SystemMessages.stringIDs.Add(375773780U, "QuotaProhibitReceiveFolderHierarchyDepthDetails");
			SystemMessages.stringIDs.Add(2271073400U, "QuotaWarningFoldersCountDetails");
			SystemMessages.stringIDs.Add(799906996U, "E4EReceivedMessage");
			SystemMessages.stringIDs.Add(3093270387U, "ExPartnerMailDisplayName");
			SystemMessages.stringIDs.Add(4198466683U, "ExAttachmentRemovedDisplayName");
			SystemMessages.stringIDs.Add(977621114U, "ShortText5_3_3");
			SystemMessages.stringIDs.Add(2308350314U, "QuotaWarningDetailsPF");
			SystemMessages.stringIDs.Add(2844551000U, "ArchiveQuotaWarningNoLimitDetails");
			SystemMessages.stringIDs.Add(4076586603U, "GeneratingServerTitle");
			SystemMessages.stringIDs.Add(1431455589U, "QuotaSendReceiveSubject");
			SystemMessages.stringIDs.Add(3632599111U, "ShortText5_1_8");
			SystemMessages.stringIDs.Add(3585544944U, "ShortText5_1_3");
			SystemMessages.stringIDs.Add(159863259U, "ExOrarMailRecipientDescription");
			SystemMessages.stringIDs.Add(1343379807U, "ArchiveQuotaFullSubject");
			SystemMessages.stringIDs.Add(2333692916U, "QuotaWarningNoLimitDetailsPF");
			SystemMessages.stringIDs.Add(2402542554U, "ShortText5_4_4");
			SystemMessages.stringIDs.Add(2088818508U, "HumanText_InitMsg");
			SystemMessages.stringIDs.Add(322888411U, "HumanText5_7_300");
			SystemMessages.stringIDs.Add(4249345458U, "BodyReceiveRMEmail");
			SystemMessages.stringIDs.Add(1733655566U, "QuotaWarningFoldersCountSubject");
			SystemMessages.stringIDs.Add(3171360660U, "QuotaWarningDetails");
			SystemMessages.stringIDs.Add(3080592406U, "HumanText5_3_0");
			SystemMessages.stringIDs.Add(605213413U, "HumanText5_7_100");
			SystemMessages.stringIDs.Add(3604836458U, "HumanText5_1_6");
			SystemMessages.stringIDs.Add(2382339578U, "ShortText5_7_7");
			SystemMessages.stringIDs.Add(347238620U, "QuotaWarningFolderHierarchyChildrenNoLimitSubject");
			SystemMessages.stringIDs.Add(3141854465U, "ShortText5_7_0");
			SystemMessages.stringIDs.Add(3362917408U, "HumanText5_5_0");
			SystemMessages.stringIDs.Add(1321639685U, "QuotaProhibitReceiveFolderHierarchyChildrenCountDetails");
			SystemMessages.stringIDs.Add(3988829471U, "ShortText5_1_0");
			SystemMessages.stringIDs.Add(2455731400U, "QuotaWarningFoldersCountNoLimitSubject");
			SystemMessages.stringIDs.Add(1776630491U, "HumanText5_4_0");
			SystemMessages.stringIDs.Add(750807939U, "ExAttachmentRemovedRecipientDescription");
			SystemMessages.stringIDs.Add(3452592194U, "QuotaCurrentSize");
			SystemMessages.stringIDs.Add(2915608171U, "DecisionUpdateTopText");
			SystemMessages.stringIDs.Add(3342714432U, "HumanText5_4_1");
			SystemMessages.stringIDs.Add(3565341968U, "ShortText5_4_6");
			SystemMessages.stringIDs.Add(3948423519U, "ShortText5_7_6");
			SystemMessages.stringIDs.Add(1242161506U, "ApprovalRequestSubjectPrefix");
			SystemMessages.stringIDs.Add(4133660744U, "E4ESignIn");
			SystemMessages.stringIDs.Add(831975206U, "InternetConfidentialName");
			SystemMessages.stringIDs.Add(1272589415U, "HumanText5_7_7");
			SystemMessages.stringIDs.Add(412971110U, "ShortText5_7_3");
			SystemMessages.stringIDs.Add(452614234U, "QuotaWarningNoLimitDetails");
			SystemMessages.stringIDs.Add(1230109185U, "E4EEncryptedMessage");
			SystemMessages.stringIDs.Add(1796833467U, "HumanText5_5_1");
			SystemMessages.stringIDs.Add(3316231944U, "DataCenterHumanText5_7_1");
			SystemMessages.stringIDs.Add(2261380053U, "ShortText5_5_6");
			SystemMessages.stringIDs.Add(1525463967U, "QuotaWarningFolderHierarchyChildrenCountDetails");
			SystemMessages.stringIDs.Add(1219540164U, "ShortText5_7_9");
			SystemMessages.stringIDs.Add(856661589U, "ShortText5_1_2");
			SystemMessages.stringIDs.Add(859070570U, "QuotaWarningMailboxMessagesPerFolderCountDetails");
			SystemMessages.stringIDs.Add(3241957883U, "HumanText5_7_3");
			SystemMessages.stringIDs.Add(1793216387U, "DoNotForwardDescription");
			SystemMessages.stringIDs.Add(4019621683U, "ModerationExpiryNoticationForSenderTopText");
			SystemMessages.stringIDs.Add(210546550U, "HumanText5_4_7");
			SystemMessages.stringIDs.Add(1569096212U, "DataCenterHumanText4_4_7");
			SystemMessages.stringIDs.Add(492871552U, "HumanText5_6_3");
			SystemMessages.stringIDs.Add(4243391820U, "HumanText5_3_2");
			SystemMessages.stringIDs.Add(3124761826U, "ApprovalCommentAttachmentFilename");
			SystemMessages.stringIDs.Add(2375073438U, "ArchiveQuotaWarningNoLimitSubject");
			SystemMessages.stringIDs.Add(472668576U, "HumanText5_1_4");
			SystemMessages.stringIDs.Add(2988353893U, "E4EHosted");
			SystemMessages.stringIDs.Add(3224920035U, "QuotaSendReceiveDetails");
			SystemMessages.stringIDs.Add(3041097916U, "ShortText5_2_4");
			SystemMessages.stringIDs.Add(843859989U, "DSNEnhanced_5_7_1_RESOLVER_RST_NotAuthorized");
			SystemMessages.stringIDs.Add(1098580639U, "ShortText5_5_4");
			SystemMessages.stringIDs.Add(86736107U, "QuotaWarningMailboxMessagesPerFolderNoLimitDetails");
			SystemMessages.stringIDs.Add(2120217552U, "ShortText5_6_4");
			SystemMessages.stringIDs.Add(2019461003U, "ShortText5_1_4");
			SystemMessages.stringIDs.Add(1635467990U, "HumanText5_1_2");
			SystemMessages.stringIDs.Add(3715345541U, "QuotaWarningFolderHierarchyChildrenCountSubject");
			SystemMessages.stringIDs.Add(2200117994U, "HumanText5_5_2");
			SystemMessages.stringIDs.Add(433174086U, "ShortText5_4_0");
			SystemMessages.stringIDs.Add(2079158469U, "HumanText5_7_1");
			SystemMessages.stringIDs.Add(3463673957U, "HumanText5_2_4");
			SystemMessages.stringIDs.Add(357259713U, "DSNEnhanced_5_2_3_RESOLVER_RST_RecipSizeLimit");
			SystemMessages.stringIDs.Add(20682666U, "E4EWaitMessage");
			SystemMessages.stringIDs.Add(1418666303U, "ApprovalRequestExpiryTopText");
			SystemMessages.stringIDs.Add(1835138854U, "E4EViewMessageButton");
			SystemMessages.stringIDs.Add(554133611U, "ShortText5_6_3");
			SystemMessages.stringIDs.Add(1232183463U, "HumanText5_1_1");
			SystemMessages.stringIDs.Add(2489798467U, "Moderation_Reencryption_Exception");
			SystemMessages.stringIDs.Add(150849084U, "ShortText5_6_0");
			SystemMessages.stringIDs.Add(1716933025U, "ShortText5_6_1");
			SystemMessages.stringIDs.Add(2028225464U, "ApprovalRequestExpiredNotificationSubjectPrefix");
			SystemMessages.stringIDs.Add(1393548940U, "HumanText5_5_4");
			SystemMessages.stringIDs.Add(1999258027U, "ShortText5_4_1");
			SystemMessages.stringIDs.Add(2807992444U, "QuotaWarningNoLimitSubject");
			SystemMessages.stringIDs.Add(724420710U, "DecisionConflictTopText");
			SystemMessages.stringIDs.Add(2707856071U, "FailedHumanReadableTopTextForTextMessage");
			SystemMessages.stringIDs.Add(3221754907U, "HumanText5_6_2");
			SystemMessages.stringIDs.Add(96782060U, "QuotaSendDetailsPF");
			SystemMessages.stringIDs.Add(2838673356U, "HumanText5_7_6");
			SystemMessages.stringIDs.Add(2912990139U, "QuotaSendReceiveTopText");
			SystemMessages.stringIDs.Add(4223188844U, "HumanText5_2_3");
			SystemMessages.stringIDs.Add(3163998196U, "ApproveButtonText");
			SystemMessages.stringIDs.Add(3827208783U, "DiagnosticInformationTitle");
			SystemMessages.stringIDs.Add(2939429905U, "HumanText5_4_6");
			SystemMessages.stringIDs.Add(230749526U, "HumanText5_5_6");
			SystemMessages.stringIDs.Add(3827463994U, "ShortText5_5_5");
			SystemMessages.stringIDs.Add(3545138992U, "ShortText5_7_5");
			SystemMessages.stringIDs.Add(2279047500U, "DSNEnhanced_5_7_1_TRANSPORT_RULES_RejectMessage");
			SystemMessages.stringIDs.Add(1514508465U, "HumanText5_3_1");
			SystemMessages.stringIDs.Add(2140420528U, "ShortText5_3_1");
			SystemMessages.stringIDs.Add(2543705055U, "ShortText5_3_2");
			SystemMessages.stringIDs.Add(2107776300U, "ShortText5_7_10");
		}

		public static LocalizedString HumanText5_2_1
		{
			get
			{
				return new LocalizedString("HumanText5_2_1", "ExB408C8", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ArchiveQuotaWarningSubject
		{
			get
			{
				return new LocalizedString("ArchiveQuotaWarningSubject", "Ex9E567B", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_1_7
		{
			get
			{
				return new LocalizedString("ShortText5_1_7", "ExFA341E", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DSNEnhanced_5_4_4_SMTPSEND_DNS_NonExistentDomain
		{
			get
			{
				return new LocalizedString("DSNEnhanced_5_4_4_SMTPSEND_DNS_NonExistentDomain", "ExFCCCD7", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanReadableBoldedSubjectLine(string subject)
		{
			return new LocalizedString("HumanReadableBoldedSubjectLine", "", false, false, SystemMessages.ResourceManager, new object[]
			{
				subject
			});
		}

		public static LocalizedString DataCenterHumanText5_4_6
		{
			get
			{
				return new LocalizedString("DataCenterHumanText5_4_6", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExAttachmentRemovedSenderDescription
		{
			get
			{
				return new LocalizedString("ExAttachmentRemovedSenderDescription", "ExEE203E", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_3_5
		{
			get
			{
				return new LocalizedString("HumanText5_3_5", "Ex93851F", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DsnParamTextMessageSizePerRecipientInMB(string currentSize, string maxSize)
		{
			return new LocalizedString("DsnParamTextMessageSizePerRecipientInMB", "Ex869418", false, true, SystemMessages.ResourceManager, new object[]
			{
				currentSize,
				maxSize
			});
		}

		public static LocalizedString DSNEnhanced_5_2_3_QUEUE_Priority
		{
			get
			{
				return new LocalizedString("DSNEnhanced_5_2_3_QUEUE_Priority", "Ex91FDA3", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_3_5
		{
			get
			{
				return new LocalizedString("ShortText5_3_5", "Ex87FD47", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_4_8
		{
			get
			{
				return new LocalizedString("HumanText5_4_8", "Ex5183B0", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningSubjectPF
		{
			get
			{
				return new LocalizedString("QuotaWarningSubjectPF", "Ex935943", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString E4EViewMessage
		{
			get
			{
				return new LocalizedString("E4EViewMessage", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReadSubject
		{
			get
			{
				return new LocalizedString("ReadSubject", "Ex60F14F", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanReadableFinalText
		{
			get
			{
				return new LocalizedString("HumanReadableFinalText", "Ex409525", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DSNEnhanced_5_2_3_RESOLVER_RST_SendSizeLimit_Sender
		{
			get
			{
				return new LocalizedString("DSNEnhanced_5_2_3_RESOLVER_RST_SendSizeLimit_Sender", "ExB3F3FB", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ArchiveQuotaWarningTopText
		{
			get
			{
				return new LocalizedString("ArchiveQuotaWarningTopText", "Ex9117F0", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_6_2
		{
			get
			{
				return new LocalizedString("ShortText5_6_2", "Ex19F714", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DSNEnhanced_5_7_1_RESOLVER_RST_NotAuthorizedToGroup
		{
			get
			{
				return new LocalizedString("DSNEnhanced_5_7_1_RESOLVER_RST_NotAuthorizedToGroup", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningFolderHierarchyDepthSubject
		{
			get
			{
				return new LocalizedString("QuotaWarningFolderHierarchyDepthSubject", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DecisionProcessedNotificationSubjectPrefix
		{
			get
			{
				return new LocalizedString("DecisionProcessedNotificationSubjectPrefix", "Ex2B2687", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningFolderHierarchyDepthNoLimitDetails
		{
			get
			{
				return new LocalizedString("QuotaWarningFolderHierarchyDepthNoLimitDetails", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaProhibitReceiveFolderHierarchyDepthSubject
		{
			get
			{
				return new LocalizedString("QuotaProhibitReceiveFolderHierarchyDepthSubject", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningFolderHierarchyDepthDetails
		{
			get
			{
				return new LocalizedString("QuotaWarningFolderHierarchyDepthDetails", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DSNEnhanced_5_6_0_RESOLVER_MT_ModerationReencrptionFailed
		{
			get
			{
				return new LocalizedString("DSNEnhanced_5_6_0_RESOLVER_MT_ModerationReencrptionFailed", "Ex83139A", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_2_0
		{
			get
			{
				return new LocalizedString("ShortText5_2_0", "Ex73A191", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaProhibitReceiveFoldersCountDetails
		{
			get
			{
				return new LocalizedString("QuotaProhibitReceiveFoldersCountDetails", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_4_2
		{
			get
			{
				return new LocalizedString("ShortText5_4_2", "Ex397784", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_7_2
		{
			get
			{
				return new LocalizedString("ShortText5_7_2", "Ex505A9B", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotReadSubject
		{
			get
			{
				return new LocalizedString("NotReadSubject", "ExE3B31A", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_7_0
		{
			get
			{
				return new LocalizedString("HumanText5_7_0", "Ex35FDD9", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DSNEnhanced_5_2_3_RESOLVER_RST_SendSizeLimit_Org
		{
			get
			{
				return new LocalizedString("DSNEnhanced_5_2_3_RESOLVER_RST_SendSizeLimit_Org", "Ex274547", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_7_10
		{
			get
			{
				return new LocalizedString("HumanText5_7_10", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_5_3
		{
			get
			{
				return new LocalizedString("HumanText5_5_3", "ExDB4E85", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_7_4
		{
			get
			{
				return new LocalizedString("ShortText5_7_4", "Ex0D8773", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_3_3
		{
			get
			{
				return new LocalizedString("HumanText5_3_3", "ExC668E0", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_1_7
		{
			get
			{
				return new LocalizedString("HumanText5_1_7", "ExCF82B7", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_6_0
		{
			get
			{
				return new LocalizedString("HumanText5_6_0", "ExEFB6F0", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaProhibitReceiveFoldersCountTopText(string count)
		{
			return new LocalizedString("QuotaProhibitReceiveFoldersCountTopText", "", false, false, SystemMessages.ResourceManager, new object[]
			{
				count
			});
		}

		public static LocalizedString ShortText5_5_2
		{
			get
			{
				return new LocalizedString("ShortText5_5_2", "Ex786EEE", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaProhibitReceiveFolderHierarchyChildrenCountSubject
		{
			get
			{
				return new LocalizedString("QuotaProhibitReceiveFolderHierarchyChildrenCountSubject", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningFolderHierarchyDepthTopText(string folder, string count)
		{
			return new LocalizedString("QuotaWarningFolderHierarchyDepthTopText", "", false, false, SystemMessages.ResourceManager, new object[]
			{
				folder,
				count
			});
		}

		public static LocalizedString QuotaWarningSubject
		{
			get
			{
				return new LocalizedString("QuotaWarningSubject", "Ex81CBC1", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_6_5
		{
			get
			{
				return new LocalizedString("HumanText5_6_5", "ExC4BEFD", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_1_6
		{
			get
			{
				return new LocalizedString("ShortText5_1_6", "ExD6357D", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_2_2_StoreNDR
		{
			get
			{
				return new LocalizedString("HumanText5_2_2_StoreNDR", "Ex94675B", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_3_0
		{
			get
			{
				return new LocalizedString("ShortText5_3_0", "Ex0455D6", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ArchiveQuotaFullTopText
		{
			get
			{
				return new LocalizedString("ArchiveQuotaFullTopText", "ExB1BA4D", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DoNotForwardName
		{
			get
			{
				return new LocalizedString("DoNotForwardName", "Ex58E3CC", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaProhibitReceiveFolderHierarchyDepthTopText(string folder, string count)
		{
			return new LocalizedString("QuotaProhibitReceiveFolderHierarchyDepthTopText", "", false, false, SystemMessages.ResourceManager, new object[]
			{
				folder,
				count
			});
		}

		public static LocalizedString ModeratedDLApprovalRequestRecipientList(int count)
		{
			return new LocalizedString("ModeratedDLApprovalRequestRecipientList", "Ex917C07", false, true, SystemMessages.ResourceManager, new object[]
			{
				count
			});
		}

		public static LocalizedString ShortText5_6_5
		{
			get
			{
				return new LocalizedString("ShortText5_6_5", "Ex153777", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningMailboxMessagesPerFolderNoLimitSubject
		{
			get
			{
				return new LocalizedString("QuotaWarningMailboxMessagesPerFolderNoLimitSubject", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_3_4
		{
			get
			{
				return new LocalizedString("ShortText5_3_4", "Ex71E89C", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_6_1
		{
			get
			{
				return new LocalizedString("HumanText5_6_1", "Ex655E57", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaProhibitReceiveMailboxMessagesPerFolderCountDetails
		{
			get
			{
				return new LocalizedString("QuotaProhibitReceiveMailboxMessagesPerFolderCountDetails", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_4_3
		{
			get
			{
				return new LocalizedString("ShortText5_4_3", "Ex62D181", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RejectButtonText
		{
			get
			{
				return new LocalizedString("RejectButtonText", "Ex4E7583", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DataCenterHumanText5_1_0
		{
			get
			{
				return new LocalizedString("DataCenterHumanText5_1_0", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningFoldersCountNoLimitTopText(string count)
		{
			return new LocalizedString("QuotaWarningFoldersCountNoLimitTopText", "", false, false, SystemMessages.ResourceManager, new object[]
			{
				count
			});
		}

		public static LocalizedString ArchiveQuotaFullDetails
		{
			get
			{
				return new LocalizedString("ArchiveQuotaFullDetails", "Ex0DD730", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_5_0
		{
			get
			{
				return new LocalizedString("ShortText5_5_0", "Ex97BC73", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaProhibitReceiveMailboxMessagesPerFolderCountTopText(string folder, string count)
		{
			return new LocalizedString("QuotaProhibitReceiveMailboxMessagesPerFolderCountTopText", "", false, false, SystemMessages.ResourceManager, new object[]
			{
				folder,
				count
			});
		}

		public static LocalizedString HumanText5_7_2
		{
			get
			{
				return new LocalizedString("HumanText5_7_2", "ExC68461", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DsnParamTextRecipientCount(string currentRecipientCount, string maxRecipientCount)
		{
			return new LocalizedString("DsnParamTextRecipientCount", "Ex3F3468", false, true, SystemMessages.ResourceManager, new object[]
			{
				currentRecipientCount,
				maxRecipientCount
			});
		}

		public static LocalizedString HumanTextFailedSmtpToSmsGatewayNotification(string number, string carrier, string bodyText)
		{
			return new LocalizedString("HumanTextFailedSmtpToSmsGatewayNotification", "Ex068E4A", false, true, SystemMessages.ResourceManager, new object[]
			{
				number,
				carrier,
				bodyText
			});
		}

		public static LocalizedString ShortText5_0_0
		{
			get
			{
				return new LocalizedString("ShortText5_0_0", "Ex9431D4", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DataCenterHumanText5_4_1
		{
			get
			{
				return new LocalizedString("DataCenterHumanText5_4_1", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RelayedHumanReadableTopText
		{
			get
			{
				return new LocalizedString("RelayedHumanReadableTopText", "Ex0935BC", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedSubject
		{
			get
			{
				return new LocalizedString("FailedSubject", "Ex1B3E1B", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DelayedHumanReadableTopText
		{
			get
			{
				return new LocalizedString("DelayedHumanReadableTopText", "Ex7B6028", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaMaxSize
		{
			get
			{
				return new LocalizedString("QuotaMaxSize", "Ex9915CA", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_7_8
		{
			get
			{
				return new LocalizedString("ShortText5_7_8", "Ex517439", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaSendTopText
		{
			get
			{
				return new LocalizedString("QuotaSendTopText", "Ex7E7395", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaSendSubjectPF
		{
			get
			{
				return new LocalizedString("QuotaSendSubjectPF", "Ex0B015A", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_1_0
		{
			get
			{
				return new LocalizedString("HumanText5_1_0", "ExCA6A4C", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_7_9
		{
			get
			{
				return new LocalizedString("HumanText5_7_9", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_5_1
		{
			get
			{
				return new LocalizedString("ShortText5_5_1", "ExBE46D2", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaProhibitReceiveMailboxMessagesPerFolderCountSubject
		{
			get
			{
				return new LocalizedString("QuotaProhibitReceiveMailboxMessagesPerFolderCountSubject", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningFolderHierarchyDepthNoLimitTopText(string folder, string count)
		{
			return new LocalizedString("QuotaWarningFolderHierarchyDepthNoLimitTopText", "", false, false, SystemMessages.ResourceManager, new object[]
			{
				folder,
				count
			});
		}

		public static LocalizedString ExpandedHumanReadableTopText
		{
			get
			{
				return new LocalizedString("ExpandedHumanReadableTopText", "Ex8A1995", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_4_3
		{
			get
			{
				return new LocalizedString("HumanText5_4_3", "Ex0FF9D9", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedHumanReadableTopText
		{
			get
			{
				return new LocalizedString("FailedHumanReadableTopText", "ExD02008", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BodyHeaderFontTag
		{
			get
			{
				return new LocalizedString("BodyHeaderFontTag", "ExB30ACE", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BodyBlockFontTag
		{
			get
			{
				return new LocalizedString("BodyBlockFontTag", "ExD050DF", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DSNEnhanced_5_4_4_ROUTING_NoNextHop
		{
			get
			{
				return new LocalizedString("DSNEnhanced_5_4_4_ROUTING_NoNextHop", "ExADF848", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString E4EDisclaimer
		{
			get
			{
				return new LocalizedString("E4EDisclaimer", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ModerationNdrNotificationForSenderTopText
		{
			get
			{
				return new LocalizedString("ModerationNdrNotificationForSenderTopText", "Ex170399", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExOrarMailSenderDescription
		{
			get
			{
				return new LocalizedString("ExOrarMailSenderDescription", "Ex6CA2C7", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningFolderHierarchyChildrenNoLimitDetails
		{
			get
			{
				return new LocalizedString("QuotaWarningFolderHierarchyChildrenNoLimitDetails", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DSNEnhanced_5_7_1_RESOLVER_RST_AuthRequired
		{
			get
			{
				return new LocalizedString("DSNEnhanced_5_7_1_RESOLVER_RST_AuthRequired", "ExB24F25", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString E4EHeaderCustom
		{
			get
			{
				return new LocalizedString("E4EHeaderCustom", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DelayedSubject
		{
			get
			{
				return new LocalizedString("DelayedSubject", "Ex1B9E9C", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ArchiveQuotaWarningNoLimitTopText(string size)
		{
			return new LocalizedString("ArchiveQuotaWarningNoLimitTopText", "Ex367C56", false, true, SystemMessages.ResourceManager, new object[]
			{
				size
			});
		}

		public static LocalizedString ModerationExpiryNoticationForModeratorTopText
		{
			get
			{
				return new LocalizedString("ModerationExpiryNoticationForModeratorTopText", "Ex28DF95", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_5_3
		{
			get
			{
				return new LocalizedString("ShortText5_5_3", "Ex4F4960", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_6_4
		{
			get
			{
				return new LocalizedString("HumanText5_6_4", "ExBE73FF", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString E4EViewMessageOTPButton
		{
			get
			{
				return new LocalizedString("E4EViewMessageOTPButton", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_1_8
		{
			get
			{
				return new LocalizedString("HumanText5_1_8", "Ex18A34D", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RejectedNotificationSubjectPrefix
		{
			get
			{
				return new LocalizedString("RejectedNotificationSubjectPrefix", "Ex65F68A", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningMailboxMessagesPerFolderCountSubject
		{
			get
			{
				return new LocalizedString("QuotaWarningMailboxMessagesPerFolderCountSubject", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_7_1
		{
			get
			{
				return new LocalizedString("ShortText5_7_1", "Ex6A075C", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_2_0
		{
			get
			{
				return new LocalizedString("HumanText5_2_0", "Ex355D15", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_0_0
		{
			get
			{
				return new LocalizedString("HumanText5_0_0", "Ex04D4D6", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EnhancedDsnTextFontTag
		{
			get
			{
				return new LocalizedString("EnhancedDsnTextFontTag", "Ex7A1961", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_2_2
		{
			get
			{
				return new LocalizedString("ShortText5_2_2", "Ex4EEE80", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanReadableBoldedToLine(string toAddresses)
		{
			return new LocalizedString("HumanReadableBoldedToLine", "", false, false, SystemMessages.ResourceManager, new object[]
			{
				toAddresses
			});
		}

		public static LocalizedString ApprovalRequestPreview
		{
			get
			{
				return new LocalizedString("ApprovalRequestPreview", "Ex0C11DE", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_4_2
		{
			get
			{
				return new LocalizedString("HumanText5_4_2", "Ex397263", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OriginalHeadersTitle
		{
			get
			{
				return new LocalizedString("OriginalHeadersTitle", "Ex9968C9", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ModeratorsOofSubjectPrefix
		{
			get
			{
				return new LocalizedString("ModeratorsOofSubjectPrefix", "Ex7AA69E", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ModeratorRejectTopText
		{
			get
			{
				return new LocalizedString("ModeratorRejectTopText", "Ex6D6465", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FinalTextFontTag
		{
			get
			{
				return new LocalizedString("FinalTextFontTag", "Ex32AFD3", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExOrarMailDisplayName
		{
			get
			{
				return new LocalizedString("ExOrarMailDisplayName", "ExC77430", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DSNEnhanced_5_7_1_RESOLVER_RST_DLNeedsSenderRestrictions
		{
			get
			{
				return new LocalizedString("DSNEnhanced_5_7_1_RESOLVER_RST_DLNeedsSenderRestrictions", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DelayedHumanReadableBottomTextHours(int expiryTimeHours, int expiryTimeMinutes)
		{
			return new LocalizedString("DelayedHumanReadableBottomTextHours", "", false, false, SystemMessages.ResourceManager, new object[]
			{
				expiryTimeHours,
				expiryTimeMinutes
			});
		}

		public static LocalizedString ModeratorCommentsHeader
		{
			get
			{
				return new LocalizedString("ModeratorCommentsHeader", "ExEA0405", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ModeratorsNdrSubjectPrefix
		{
			get
			{
				return new LocalizedString("ModeratorsNdrSubjectPrefix", "Ex4045FF", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReceivingServerTitle
		{
			get
			{
				return new LocalizedString("ReceivingServerTitle", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaSendDetails
		{
			get
			{
				return new LocalizedString("QuotaSendDetails", "Ex70521D", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_1_3
		{
			get
			{
				return new LocalizedString("HumanText5_1_3", "Ex42EDF0", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString E4EOpenAttachment(string name)
		{
			return new LocalizedString("E4EOpenAttachment", "", false, false, SystemMessages.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString QuotaWarningFolderHierarchyChildrenNoLimitTopText(string folder, string count)
		{
			return new LocalizedString("QuotaWarningFolderHierarchyChildrenNoLimitTopText", "", false, false, SystemMessages.ResourceManager, new object[]
			{
				folder,
				count
			});
		}

		public static LocalizedString QuarantinedHumanReadableTopText
		{
			get
			{
				return new LocalizedString("QuarantinedHumanReadableTopText", "Ex03E04A", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeliveredSubject
		{
			get
			{
				return new LocalizedString("DeliveredSubject", "Ex7D49AA", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DSNEnhanced_5_2_3_RESOLVER_RST_RecipSizeLimit_DL
		{
			get
			{
				return new LocalizedString("DSNEnhanced_5_2_3_RESOLVER_RST_RecipSizeLimit_DL", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DSNEnhanced_5_7_1_APPROVAL_NotAuthorized
		{
			get
			{
				return new LocalizedString("DSNEnhanced_5_7_1_APPROVAL_NotAuthorized", "ExB013C8", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_7_5
		{
			get
			{
				return new LocalizedString("HumanText5_7_5", "Ex65B0FA", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_7_8
		{
			get
			{
				return new LocalizedString("HumanText5_7_8", "Ex852059", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DecisionConflictNotificationSubjectPrefix
		{
			get
			{
				return new LocalizedString("DecisionConflictNotificationSubjectPrefix", "Ex163DA9", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningNoLimitSubjectPF
		{
			get
			{
				return new LocalizedString("QuotaWarningNoLimitSubjectPF", "Ex814216", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BodyDownload
		{
			get
			{
				return new LocalizedString("BodyDownload", "ExF04F81", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaProhibitReceiveFoldersCountSubject
		{
			get
			{
				return new LocalizedString("QuotaProhibitReceiveFoldersCountSubject", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_7_100
		{
			get
			{
				return new LocalizedString("ShortText5_7_100", "Ex04234F", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ModeratedDLApprovalRequest(string senderName)
		{
			return new LocalizedString("ModeratedDLApprovalRequest", "Ex1893BF", false, true, SystemMessages.ResourceManager, new object[]
			{
				senderName
			});
		}

		public static LocalizedString QuotaWarningTopText
		{
			get
			{
				return new LocalizedString("QuotaWarningTopText", "ExC96165", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_7_300
		{
			get
			{
				return new LocalizedString("ShortText5_7_300", "Ex666A2C", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedHumanReadableTopTextForTextMessageNotification
		{
			get
			{
				return new LocalizedString("FailedHumanReadableTopTextForTextMessageNotification", "Ex5B9C89", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DelayHumanReadableExplanation
		{
			get
			{
				return new LocalizedString("DelayHumanReadableExplanation", "Ex764691", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_2_3
		{
			get
			{
				return new LocalizedString("ShortText5_2_3", "Ex38D1AF", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaProhibitReceiveFolderHierarchyChildrenCountTopText(string folder, string count)
		{
			return new LocalizedString("QuotaProhibitReceiveFolderHierarchyChildrenCountTopText", "", false, false, SystemMessages.ResourceManager, new object[]
			{
				folder,
				count
			});
		}

		public static LocalizedString InternetConfidentialDescription
		{
			get
			{
				return new LocalizedString("InternetConfidentialDescription", "ExE274BE", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_2_2
		{
			get
			{
				return new LocalizedString("HumanText5_2_2", "ExFA7D2D", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalFailedHumanReadableErrorText(string externalserver)
		{
			return new LocalizedString("ExternalFailedHumanReadableErrorText", "Ex50A06C", false, true, SystemMessages.ResourceManager, new object[]
			{
				externalserver
			});
		}

		public static LocalizedString ShortText5_4_7
		{
			get
			{
				return new LocalizedString("ShortText5_4_7", "Ex10388D", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ApprovalRequestTopText
		{
			get
			{
				return new LocalizedString("ApprovalRequestTopText", "ExBB0B32", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_4_4
		{
			get
			{
				return new LocalizedString("HumanText5_4_4", "ExFEC724", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningFolderHierarchyDepthNoLimitSubject
		{
			get
			{
				return new LocalizedString("QuotaWarningFolderHierarchyDepthNoLimitSubject", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ModerationOofNotificationForSenderTopText
		{
			get
			{
				return new LocalizedString("ModerationOofNotificationForSenderTopText", "ExD3B813", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_4_8
		{
			get
			{
				return new LocalizedString("ShortText5_4_8", "Ex818E63", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DecisionConflictWithDetailsNotification(string decisionMaker, string decision, string time, string timeZone)
		{
			return new LocalizedString("DecisionConflictWithDetailsNotification", "Ex813718", false, true, SystemMessages.ResourceManager, new object[]
			{
				decisionMaker,
				decision,
				time,
				timeZone
			});
		}

		public static LocalizedString ExpandedSubject
		{
			get
			{
				return new LocalizedString("ExpandedSubject", "Ex191809", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_5_5
		{
			get
			{
				return new LocalizedString("HumanText5_5_5", "Ex34C9AA", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DataCenterHumanText5_1_1
		{
			get
			{
				return new LocalizedString("DataCenterHumanText5_1_1", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ArchiveQuotaWarningDetails
		{
			get
			{
				return new LocalizedString("ArchiveQuotaWarningDetails", "ExA3ED4F", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DsnParamTextMessageSizePerMessageInMB(string currentSize, string maxSize)
		{
			return new LocalizedString("DsnParamTextMessageSizePerMessageInMB", "Ex3CAC48", false, true, SystemMessages.ResourceManager, new object[]
			{
				currentSize,
				maxSize
			});
		}

		public static LocalizedString E4EViewMessageInfo
		{
			get
			{
				return new LocalizedString("E4EViewMessageInfo", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DecisionConflictNotification(string subject)
		{
			return new LocalizedString("DecisionConflictNotification", "Ex4DDABA", false, true, SystemMessages.ResourceManager, new object[]
			{
				subject
			});
		}

		public static LocalizedString DeliveredHumanReadableTopText
		{
			get
			{
				return new LocalizedString("DeliveredHumanReadableTopText", "Ex21EDE0", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RelayedSubject
		{
			get
			{
				return new LocalizedString("RelayedSubject", "Ex34EFE0", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_3_4
		{
			get
			{
				return new LocalizedString("HumanText5_3_4", "Ex9EC0EA", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_2_1
		{
			get
			{
				return new LocalizedString("ShortText5_2_1", "ExE38CD1", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaSendSubject
		{
			get
			{
				return new LocalizedString("QuotaSendSubject", "Ex591232", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_1_1
		{
			get
			{
				return new LocalizedString("ShortText5_1_1", "Ex99BDE6", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DiagnosticsFontTag
		{
			get
			{
				return new LocalizedString("DiagnosticsFontTag", "Ex6CC99E", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanReadableBoldedCcLine(string ccAddresses)
		{
			return new LocalizedString("HumanReadableBoldedCcLine", "", false, false, SystemMessages.ResourceManager, new object[]
			{
				ccAddresses
			});
		}

		public static LocalizedString QuotaWarningFoldersCountNoLimitDetails
		{
			get
			{
				return new LocalizedString("QuotaWarningFoldersCountNoLimitDetails", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_7_4
		{
			get
			{
				return new LocalizedString("HumanText5_7_4", "ExEE4688", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaProhibitReceiveFolderHierarchyDepthDetails
		{
			get
			{
				return new LocalizedString("QuotaProhibitReceiveFolderHierarchyDepthDetails", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ModeratorExpiryNotification(string sender, int expiryTime)
		{
			return new LocalizedString("ModeratorExpiryNotification", "Ex515F05", false, true, SystemMessages.ResourceManager, new object[]
			{
				sender,
				expiryTime
			});
		}

		public static LocalizedString ApprovalRequestExpiryNotification(string subject)
		{
			return new LocalizedString("ApprovalRequestExpiryNotification", "Ex8588C0", false, true, SystemMessages.ResourceManager, new object[]
			{
				subject
			});
		}

		public static LocalizedString QuotaWarningFoldersCountDetails
		{
			get
			{
				return new LocalizedString("QuotaWarningFoldersCountDetails", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString E4EReceivedMessage
		{
			get
			{
				return new LocalizedString("E4EReceivedMessage", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExPartnerMailDisplayName
		{
			get
			{
				return new LocalizedString("ExPartnerMailDisplayName", "ExCB1EDF", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExAttachmentRemovedDisplayName
		{
			get
			{
				return new LocalizedString("ExAttachmentRemovedDisplayName", "Ex0BF4FE", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_3_3
		{
			get
			{
				return new LocalizedString("ShortText5_3_3", "Ex765DB3", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningDetailsPF
		{
			get
			{
				return new LocalizedString("QuotaWarningDetailsPF", "ExAD6D3E", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ArchiveQuotaWarningNoLimitDetails
		{
			get
			{
				return new LocalizedString("ArchiveQuotaWarningNoLimitDetails", "ExC33E78", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GeneratingServerTitle
		{
			get
			{
				return new LocalizedString("GeneratingServerTitle", "Ex3CEBB8", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaSendReceiveSubject
		{
			get
			{
				return new LocalizedString("QuotaSendReceiveSubject", "Ex0F3D81", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_1_8
		{
			get
			{
				return new LocalizedString("ShortText5_1_8", "Ex017A6D", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_1_3
		{
			get
			{
				return new LocalizedString("ShortText5_1_3", "ExA7F278", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExOrarMailRecipientDescription
		{
			get
			{
				return new LocalizedString("ExOrarMailRecipientDescription", "Ex466A0C", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ArchiveQuotaFullSubject
		{
			get
			{
				return new LocalizedString("ArchiveQuotaFullSubject", "Ex4AFF7C", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningNoLimitDetailsPF
		{
			get
			{
				return new LocalizedString("QuotaWarningNoLimitDetailsPF", "Ex0FD9BD", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_4_4
		{
			get
			{
				return new LocalizedString("ShortText5_4_4", "Ex3C1A13", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText_InitMsg
		{
			get
			{
				return new LocalizedString("HumanText_InitMsg", "Ex28619D", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_7_300
		{
			get
			{
				return new LocalizedString("HumanText5_7_300", "ExD068FC", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalFailedHumanReadableTopText(string externalserver)
		{
			return new LocalizedString("ExternalFailedHumanReadableTopText", "ExF565D8", false, true, SystemMessages.ResourceManager, new object[]
			{
				externalserver
			});
		}

		public static LocalizedString BodyReceiveRMEmail
		{
			get
			{
				return new LocalizedString("BodyReceiveRMEmail", "Ex7140EA", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningFoldersCountSubject
		{
			get
			{
				return new LocalizedString("QuotaWarningFoldersCountSubject", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningDetails
		{
			get
			{
				return new LocalizedString("QuotaWarningDetails", "Ex69BEE7", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_3_0
		{
			get
			{
				return new LocalizedString("HumanText5_3_0", "ExC6D5CB", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_7_100
		{
			get
			{
				return new LocalizedString("HumanText5_7_100", "ExB0DB4D", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_1_6
		{
			get
			{
				return new LocalizedString("HumanText5_1_6", "Ex15812E", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_7_7
		{
			get
			{
				return new LocalizedString("ShortText5_7_7", "Ex51E620", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningFolderHierarchyChildrenNoLimitSubject
		{
			get
			{
				return new LocalizedString("QuotaWarningFolderHierarchyChildrenNoLimitSubject", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_7_0
		{
			get
			{
				return new LocalizedString("ShortText5_7_0", "ExC2157B", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_5_0
		{
			get
			{
				return new LocalizedString("HumanText5_5_0", "ExE647CD", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaProhibitReceiveFolderHierarchyChildrenCountDetails
		{
			get
			{
				return new LocalizedString("QuotaProhibitReceiveFolderHierarchyChildrenCountDetails", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_1_0
		{
			get
			{
				return new LocalizedString("ShortText5_1_0", "Ex2FC69C", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningFoldersCountNoLimitSubject
		{
			get
			{
				return new LocalizedString("QuotaWarningFoldersCountNoLimitSubject", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_4_0
		{
			get
			{
				return new LocalizedString("HumanText5_4_0", "Ex25AC0A", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExAttachmentRemovedRecipientDescription
		{
			get
			{
				return new LocalizedString("ExAttachmentRemovedRecipientDescription", "Ex8DF76F", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaCurrentSize
		{
			get
			{
				return new LocalizedString("QuotaCurrentSize", "ExAC7949", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DecisionUpdateTopText
		{
			get
			{
				return new LocalizedString("DecisionUpdateTopText", "Ex088DD0", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_4_1
		{
			get
			{
				return new LocalizedString("HumanText5_4_1", "Ex2A4CBE", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningMailboxMessagesPerFolderCountTopText(string folder, string count)
		{
			return new LocalizedString("QuotaWarningMailboxMessagesPerFolderCountTopText", "", false, false, SystemMessages.ResourceManager, new object[]
			{
				folder,
				count
			});
		}

		public static LocalizedString ExternalFailedHumanReadableErrorNoDetailText(string externalserver)
		{
			return new LocalizedString("ExternalFailedHumanReadableErrorNoDetailText", "ExDC79C1", false, true, SystemMessages.ResourceManager, new object[]
			{
				externalserver
			});
		}

		public static LocalizedString ShortText5_4_6
		{
			get
			{
				return new LocalizedString("ShortText5_4_6", "Ex1A0BC6", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_7_6
		{
			get
			{
				return new LocalizedString("ShortText5_7_6", "ExE82864", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ApprovalRequestSubjectPrefix
		{
			get
			{
				return new LocalizedString("ApprovalRequestSubjectPrefix", "ExE89649", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString E4ESignIn
		{
			get
			{
				return new LocalizedString("E4ESignIn", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InternetConfidentialName
		{
			get
			{
				return new LocalizedString("InternetConfidentialName", "ExC319D4", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_7_7
		{
			get
			{
				return new LocalizedString("HumanText5_7_7", "Ex750074", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_7_3
		{
			get
			{
				return new LocalizedString("ShortText5_7_3", "ExA8A389", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningNoLimitDetails
		{
			get
			{
				return new LocalizedString("QuotaWarningNoLimitDetails", "Ex1A7280", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString E4EEncryptedMessage
		{
			get
			{
				return new LocalizedString("E4EEncryptedMessage", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_5_1
		{
			get
			{
				return new LocalizedString("HumanText5_5_1", "Ex4A8919", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningFoldersCountTopText(string count)
		{
			return new LocalizedString("QuotaWarningFoldersCountTopText", "", false, false, SystemMessages.ResourceManager, new object[]
			{
				count
			});
		}

		public static LocalizedString DataCenterHumanText5_7_1
		{
			get
			{
				return new LocalizedString("DataCenterHumanText5_7_1", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_5_6
		{
			get
			{
				return new LocalizedString("ShortText5_5_6", "ExCFD2EE", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningFolderHierarchyChildrenCountDetails
		{
			get
			{
				return new LocalizedString("QuotaWarningFolderHierarchyChildrenCountDetails", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SizeInFolders(string count)
		{
			return new LocalizedString("SizeInFolders", "", false, false, SystemMessages.ResourceManager, new object[]
			{
				count
			});
		}

		public static LocalizedString HumanReadableSubjectLine(string subject)
		{
			return new LocalizedString("HumanReadableSubjectLine", "", false, false, SystemMessages.ResourceManager, new object[]
			{
				subject
			});
		}

		public static LocalizedString ShortText5_7_9
		{
			get
			{
				return new LocalizedString("ShortText5_7_9", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_1_2
		{
			get
			{
				return new LocalizedString("ShortText5_1_2", "Ex45C40E", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningMailboxMessagesPerFolderCountDetails
		{
			get
			{
				return new LocalizedString("QuotaWarningMailboxMessagesPerFolderCountDetails", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_7_3
		{
			get
			{
				return new LocalizedString("HumanText5_7_3", "ExEB0312", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DoNotForwardDescription
		{
			get
			{
				return new LocalizedString("DoNotForwardDescription", "ExE9ED1D", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ModerationExpiryNoticationForSenderTopText
		{
			get
			{
				return new LocalizedString("ModerationExpiryNoticationForSenderTopText", "Ex3EB3AA", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_4_7
		{
			get
			{
				return new LocalizedString("HumanText5_4_7", "Ex57C427", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DelayedHumanReadableBottomText(int expiryTimeDays, int expiryTimeHours, int expiryTimeMinutes)
		{
			return new LocalizedString("DelayedHumanReadableBottomText", "", false, false, SystemMessages.ResourceManager, new object[]
			{
				expiryTimeDays,
				expiryTimeHours,
				expiryTimeMinutes
			});
		}

		public static LocalizedString DataCenterHumanText4_4_7
		{
			get
			{
				return new LocalizedString("DataCenterHumanText4_4_7", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_6_3
		{
			get
			{
				return new LocalizedString("HumanText5_6_3", "Ex9A4CB3", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_3_2
		{
			get
			{
				return new LocalizedString("HumanText5_3_2", "Ex823C2B", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ApprovalCommentAttachmentFilename
		{
			get
			{
				return new LocalizedString("ApprovalCommentAttachmentFilename", "Ex948363", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ArchiveQuotaWarningNoLimitSubject
		{
			get
			{
				return new LocalizedString("ArchiveQuotaWarningNoLimitSubject", "ExE3A691", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_1_4
		{
			get
			{
				return new LocalizedString("HumanText5_1_4", "Ex8CE1B3", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString E4EHosted
		{
			get
			{
				return new LocalizedString("E4EHosted", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaSendReceiveDetails
		{
			get
			{
				return new LocalizedString("QuotaSendReceiveDetails", "Ex9C6C59", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_2_4
		{
			get
			{
				return new LocalizedString("ShortText5_2_4", "Ex3BD4EB", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DSNEnhanced_5_7_1_RESOLVER_RST_NotAuthorized
		{
			get
			{
				return new LocalizedString("DSNEnhanced_5_7_1_RESOLVER_RST_NotAuthorized", "Ex62D442", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_5_4
		{
			get
			{
				return new LocalizedString("ShortText5_5_4", "ExD5A915", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningMailboxMessagesPerFolderNoLimitDetails
		{
			get
			{
				return new LocalizedString("QuotaWarningMailboxMessagesPerFolderNoLimitDetails", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_6_4
		{
			get
			{
				return new LocalizedString("ShortText5_6_4", "Ex54C089", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_1_4
		{
			get
			{
				return new LocalizedString("ShortText5_1_4", "Ex25B38E", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_1_2
		{
			get
			{
				return new LocalizedString("HumanText5_1_2", "Ex0CE300", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningFolderHierarchyChildrenCountSubject
		{
			get
			{
				return new LocalizedString("QuotaWarningFolderHierarchyChildrenCountSubject", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_5_2
		{
			get
			{
				return new LocalizedString("HumanText5_5_2", "ExE2A31B", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_4_0
		{
			get
			{
				return new LocalizedString("ShortText5_4_0", "ExFCD628", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_7_1
		{
			get
			{
				return new LocalizedString("HumanText5_7_1", "Ex41CD9B", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DsnParamTextMessageSizePerMessageInKB(string currentSize, string maxSize)
		{
			return new LocalizedString("DsnParamTextMessageSizePerMessageInKB", "ExB8F513", false, true, SystemMessages.ResourceManager, new object[]
			{
				currentSize,
				maxSize
			});
		}

		public static LocalizedString HumanText5_2_4
		{
			get
			{
				return new LocalizedString("HumanText5_2_4", "Ex5D1D57", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DsnParamTextMessageSizePerRecipientInKB(string currentSize, string maxSize)
		{
			return new LocalizedString("DsnParamTextMessageSizePerRecipientInKB", "Ex588BA3", false, true, SystemMessages.ResourceManager, new object[]
			{
				currentSize,
				maxSize
			});
		}

		public static LocalizedString DSNEnhanced_5_2_3_RESOLVER_RST_RecipSizeLimit
		{
			get
			{
				return new LocalizedString("DSNEnhanced_5_2_3_RESOLVER_RST_RecipSizeLimit", "ExE616ED", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningTopTextPF(string publicFolderName)
		{
			return new LocalizedString("QuotaWarningTopTextPF", "Ex37F059", false, true, SystemMessages.ResourceManager, new object[]
			{
				publicFolderName
			});
		}

		public static LocalizedString E4EWaitMessage
		{
			get
			{
				return new LocalizedString("E4EWaitMessage", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ApprovalRequestExpiryTopText
		{
			get
			{
				return new LocalizedString("ApprovalRequestExpiryTopText", "Ex39753B", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString E4EViewMessageButton
		{
			get
			{
				return new LocalizedString("E4EViewMessageButton", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_6_3
		{
			get
			{
				return new LocalizedString("ShortText5_6_3", "Ex73807B", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaSendTopTextPF(string publicFolderName)
		{
			return new LocalizedString("QuotaSendTopTextPF", "Ex839B3B", false, true, SystemMessages.ResourceManager, new object[]
			{
				publicFolderName
			});
		}

		public static LocalizedString QuotaWarningNoLimitTopTextPF(string publicFolderName, string size)
		{
			return new LocalizedString("QuotaWarningNoLimitTopTextPF", "ExF36392", false, true, SystemMessages.ResourceManager, new object[]
			{
				publicFolderName,
				size
			});
		}

		public static LocalizedString HumanText5_1_1
		{
			get
			{
				return new LocalizedString("HumanText5_1_1", "Ex53AE07", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Moderation_Reencryption_Exception
		{
			get
			{
				return new LocalizedString("Moderation_Reencryption_Exception", "Ex0DDCB9", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningNoLimitTopText(string size)
		{
			return new LocalizedString("QuotaWarningNoLimitTopText", "ExFFD076", false, true, SystemMessages.ResourceManager, new object[]
			{
				size
			});
		}

		public static LocalizedString ShortText5_6_0
		{
			get
			{
				return new LocalizedString("ShortText5_6_0", "Ex9D8F56", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_6_1
		{
			get
			{
				return new LocalizedString("ShortText5_6_1", "Ex994293", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningMailboxMessagesPerFolderNoLimitTopText(string folder, string count)
		{
			return new LocalizedString("QuotaWarningMailboxMessagesPerFolderNoLimitTopText", "", false, false, SystemMessages.ResourceManager, new object[]
			{
				folder,
				count
			});
		}

		public static LocalizedString ApprovalRequestExpiredNotificationSubjectPrefix
		{
			get
			{
				return new LocalizedString("ApprovalRequestExpiredNotificationSubjectPrefix", "Ex26246C", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_5_4
		{
			get
			{
				return new LocalizedString("HumanText5_5_4", "ExD72E6F", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_4_1
		{
			get
			{
				return new LocalizedString("ShortText5_4_1", "Ex844FC6", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningNoLimitSubject
		{
			get
			{
				return new LocalizedString("QuotaWarningNoLimitSubject", "ExD6C43B", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanTextFailedPasscodeWithReason(string number, string error)
		{
			return new LocalizedString("HumanTextFailedPasscodeWithReason", "Ex694130", false, true, SystemMessages.ResourceManager, new object[]
			{
				number,
				error
			});
		}

		public static LocalizedString SizeInMessages(string count)
		{
			return new LocalizedString("SizeInMessages", "", false, false, SystemMessages.ResourceManager, new object[]
			{
				count
			});
		}

		public static LocalizedString DecisionConflictTopText
		{
			get
			{
				return new LocalizedString("DecisionConflictTopText", "Ex5159BD", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedHumanReadableTopTextForTextMessage
		{
			get
			{
				return new LocalizedString("FailedHumanReadableTopTextForTextMessage", "ExCCCB72", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_6_2
		{
			get
			{
				return new LocalizedString("HumanText5_6_2", "Ex2DC288", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaSendDetailsPF
		{
			get
			{
				return new LocalizedString("QuotaSendDetailsPF", "ExDFB39B", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_7_6
		{
			get
			{
				return new LocalizedString("HumanText5_7_6", "ExAED519", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaSendReceiveTopText
		{
			get
			{
				return new LocalizedString("QuotaSendReceiveTopText", "Ex8C5663", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_2_3
		{
			get
			{
				return new LocalizedString("HumanText5_2_3", "Ex4E71B6", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ApproveButtonText
		{
			get
			{
				return new LocalizedString("ApproveButtonText", "Ex32115A", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DiagnosticInformationTitle
		{
			get
			{
				return new LocalizedString("DiagnosticInformationTitle", "Ex0BFB98", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanReadableBoldedFromLine(string from)
		{
			return new LocalizedString("HumanReadableBoldedFromLine", "", false, false, SystemMessages.ResourceManager, new object[]
			{
				from
			});
		}

		public static LocalizedString HumanText5_4_6
		{
			get
			{
				return new LocalizedString("HumanText5_4_6", "Ex1FED2A", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanTextFailedPasscodeWithoutReason(string number)
		{
			return new LocalizedString("HumanTextFailedPasscodeWithoutReason", "Ex39BE84", false, true, SystemMessages.ResourceManager, new object[]
			{
				number
			});
		}

		public static LocalizedString HumanText5_5_6
		{
			get
			{
				return new LocalizedString("HumanText5_5_6", "Ex34BE17", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaWarningFolderHierarchyChildrenCountTopText(string folder, string count)
		{
			return new LocalizedString("QuotaWarningFolderHierarchyChildrenCountTopText", "", false, false, SystemMessages.ResourceManager, new object[]
			{
				folder,
				count
			});
		}

		public static LocalizedString ShortText5_5_5
		{
			get
			{
				return new LocalizedString("ShortText5_5_5", "Ex2633AC", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_7_5
		{
			get
			{
				return new LocalizedString("ShortText5_7_5", "Ex90F8FD", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanTextFailedOmsNotification(string bodyText, string error)
		{
			return new LocalizedString("HumanTextFailedOmsNotification", "Ex89D5CB", false, true, SystemMessages.ResourceManager, new object[]
			{
				bodyText,
				error
			});
		}

		public static LocalizedString DSNEnhanced_5_7_1_TRANSPORT_RULES_RejectMessage
		{
			get
			{
				return new LocalizedString("DSNEnhanced_5_7_1_TRANSPORT_RULES_RejectMessage", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanText5_3_1
		{
			get
			{
				return new LocalizedString("HumanText5_3_1", "ExE2A518", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HumanReadableRemoteServerText(string remoteServer)
		{
			return new LocalizedString("HumanReadableRemoteServerText", "", false, false, SystemMessages.ResourceManager, new object[]
			{
				remoteServer
			});
		}

		public static LocalizedString ShortText5_3_1
		{
			get
			{
				return new LocalizedString("ShortText5_3_1", "Ex11B600", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SizeInMB(string count)
		{
			return new LocalizedString("SizeInMB", "", false, false, SystemMessages.ResourceManager, new object[]
			{
				count
			});
		}

		public static LocalizedString ShortText5_3_2
		{
			get
			{
				return new LocalizedString("ShortText5_3_2", "ExED88AE", false, true, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortText5_7_10
		{
			get
			{
				return new LocalizedString("ShortText5_7_10", "", false, false, SystemMessages.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(SystemMessages.IDs key)
		{
			return new LocalizedString(SystemMessages.stringIDs[(uint)key], SystemMessages.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(253);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Core.SystemMessages", typeof(SystemMessages).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			HumanText5_2_1 = 3060389430U,
			ArchiveQuotaWarningSubject = 224226320U,
			ShortText5_1_7 = 1616176476U,
			DSNEnhanced_5_4_4_SMTPSEND_DNS_NonExistentDomain = 955676612U,
			DataCenterHumanText5_4_6 = 4216909332U,
			ExAttachmentRemovedSenderDescription = 2547234435U,
			HumanText5_3_5 = 3483876933U,
			DSNEnhanced_5_2_3_QUEUE_Priority = 3038358855U,
			ShortText5_3_5 = 171052060U,
			HumanText5_4_8 = 2132860851U,
			QuotaWarningSubjectPF = 264042572U,
			E4EViewMessage = 1070379938U,
			ReadSubject = 1498176974U,
			HumanReadableFinalText = 3726703438U,
			DSNEnhanced_5_2_3_RESOLVER_RST_SendSizeLimit_Sender = 1459475246U,
			ArchiveQuotaWarningTopText = 2954596652U,
			ShortText5_6_2 = 3283016966U,
			DSNEnhanced_5_7_1_RESOLVER_RST_NotAuthorizedToGroup = 1494114401U,
			QuotaWarningFolderHierarchyDepthSubject = 286121318U,
			DecisionProcessedNotificationSubjectPrefix = 1411446847U,
			QuotaWarningFolderHierarchyDepthNoLimitDetails = 4198661606U,
			QuotaProhibitReceiveFolderHierarchyDepthSubject = 3925426878U,
			QuotaWarningFolderHierarchyDepthDetails = 4044098184U,
			DSNEnhanced_5_6_0_RESOLVER_MT_ModerationReencrptionFailed = 80797861U,
			ShortText5_2_0 = 715499088U,
			QuotaProhibitReceiveFoldersCountDetails = 1234410400U,
			ShortText5_4_2 = 1595973500U,
			ShortText5_7_2 = 1979055051U,
			NotReadSubject = 3244754815U,
			HumanText5_7_0 = 3645242410U,
			DSNEnhanced_5_2_3_RESOLVER_RST_SendSizeLimit_Org = 2798911709U,
			HumanText5_7_10 = 937708661U,
			HumanText5_5_3 = 634034053U,
			ShortText5_7_4 = 816255637U,
			HumanText5_3_3 = 2677307879U,
			HumanText5_1_7 = 2038752517U,
			HumanText5_6_0 = 2058955493U,
			ShortText5_5_2 = 292011585U,
			QuotaProhibitReceiveFolderHierarchyChildrenCountSubject = 3633934039U,
			QuotaWarningSubject = 4064869466U,
			HumanText5_6_5 = 1655670966U,
			ShortText5_1_6 = 3182260417U,
			HumanText5_2_2_StoreNDR = 2766572931U,
			ShortText5_3_0 = 3706504469U,
			ArchiveQuotaFullTopText = 1156273137U,
			DoNotForwardName = 3666692194U,
			ShortText5_6_5 = 3686301493U,
			QuotaWarningMailboxMessagesPerFolderNoLimitSubject = 3685029433U,
			ShortText5_3_4 = 1737136001U,
			HumanText5_6_1 = 3625039434U,
			QuotaProhibitReceiveMailboxMessagesPerFolderCountDetails = 3888161794U,
			ShortText5_4_3 = 3162057441U,
			RejectButtonText = 2844775350U,
			DataCenterHumanText5_1_0 = 1467823001U,
			ArchiveQuotaFullDetails = 448693229U,
			ShortText5_5_0 = 3424179467U,
			HumanText5_7_2 = 513074528U,
			ShortText5_0_0 = 997824090U,
			DataCenterHumanText5_4_1 = 3457394445U,
			RelayedHumanReadableTopText = 2762698027U,
			FailedSubject = 2043697631U,
			DelayedHumanReadableTopText = 2357302625U,
			QuotaMaxSize = 34865187U,
			ShortText5_7_8 = 2785624105U,
			QuotaSendTopText = 4194575540U,
			QuotaSendSubjectPF = 2295532082U,
			HumanText5_1_0 = 2798267404U,
			HumanText5_7_9 = 2435388829U,
			ShortText5_5_1 = 1858095526U,
			QuotaProhibitReceiveMailboxMessagesPerFolderCountSubject = 2240869372U,
			ExpandedHumanReadableTopText = 913497348U,
			HumanText5_4_3 = 2179915018U,
			FailedHumanReadableTopText = 894078784U,
			BodyHeaderFontTag = 3183792054U,
			BodyBlockFontTag = 4029344418U,
			DSNEnhanced_5_4_4_ROUTING_NoNextHop = 1086182386U,
			E4EDisclaimer = 1775266991U,
			ModerationNdrNotificationForSenderTopText = 4171950547U,
			ExOrarMailSenderDescription = 4135226003U,
			QuotaWarningFolderHierarchyChildrenNoLimitDetails = 777497770U,
			DSNEnhanced_5_7_1_RESOLVER_RST_AuthRequired = 1075924926U,
			E4EHeaderCustom = 3187564900U,
			DelayedSubject = 3017333248U,
			ModerationExpiryNoticationForModeratorTopText = 3276713205U,
			ShortText5_5_3 = 3020894940U,
			HumanText5_6_4 = 89587025U,
			E4EViewMessageOTPButton = 2397807267U,
			HumanText5_1_8 = 3154497764U,
			RejectedNotificationSubjectPrefix = 1062071583U,
			QuotaWarningMailboxMessagesPerFolderCountSubject = 3905817172U,
			ShortText5_7_1 = 1575770524U,
			HumanText5_2_0 = 1494305489U,
			HumanText5_0_0 = 1211980487U,
			EnhancedDsnTextFontTag = 3597730735U,
			ShortText5_2_2 = 3847666970U,
			ApprovalRequestPreview = 273945714U,
			HumanText5_4_2 = 613831077U,
			OriginalHeadersTitle = 1416892261U,
			ModeratorsOofSubjectPrefix = 3886882982U,
			ModeratorRejectTopText = 3328018042U,
			FinalTextFontTag = 3837274988U,
			ExOrarMailDisplayName = 4028322787U,
			DSNEnhanced_5_7_1_RESOLVER_RST_DLNeedsSenderRestrictions = 228586384U,
			ModeratorCommentsHeader = 3547364398U,
			ModeratorsNdrSubjectPrefix = 2894800102U,
			ReceivingServerTitle = 2504161731U,
			QuotaSendDetails = 3335723890U,
			HumanText5_1_3 = 69384049U,
			QuarantinedHumanReadableTopText = 1797209243U,
			DeliveredSubject = 2296356702U,
			DSNEnhanced_5_2_3_RESOLVER_RST_RecipSizeLimit_DL = 4273299450U,
			DSNEnhanced_5_7_1_APPROVAL_NotAuthorized = 1203275484U,
			HumanText5_7_5 = 109790001U,
			HumanText5_7_8 = 4001472770U,
			DecisionConflictNotificationSubjectPrefix = 238766803U,
			QuotaWarningNoLimitSubjectPF = 1830882122U,
			BodyDownload = 439284250U,
			QuotaProhibitReceiveFoldersCountSubject = 1979593922U,
			ShortText5_7_100 = 2618404892U,
			QuotaWarningTopText = 2028676810U,
			ShortText5_7_300 = 2900729894U,
			FailedHumanReadableTopTextForTextMessageNotification = 3949783936U,
			DelayHumanReadableExplanation = 2592547661U,
			ShortText5_2_3 = 1118783615U,
			InternetConfidentialDescription = 2820071495U,
			HumanText5_2_2 = 2657104903U,
			ShortText5_4_7 = 836458613U,
			ApprovalRequestTopText = 742696162U,
			HumanText5_4_4 = 4102229319U,
			QuotaWarningFolderHierarchyDepthNoLimitSubject = 1091303688U,
			ModerationOofNotificationForSenderTopText = 325931173U,
			ShortText5_4_8 = 76943726U,
			ExpandedSubject = 3301930267U,
			HumanText5_5_5 = 4122432295U,
			DataCenterHumanText5_1_1 = 3033906942U,
			ArchiveQuotaWarningDetails = 3806593754U,
			E4EViewMessageInfo = 1586140930U,
			DeliveredHumanReadableTopText = 3351672821U,
			RelayedSubject = 304371086U,
			HumanText5_3_4 = 754993578U,
			ShortText5_2_1 = 2281583029U,
			QuotaSendSubject = 4035994692U,
			ShortText5_1_1 = 2422745530U,
			DiagnosticsFontTag = 3158471333U,
			QuotaWarningFoldersCountNoLimitDetails = 833904234U,
			HumanText5_7_4 = 1675873942U,
			QuotaProhibitReceiveFolderHierarchyDepthDetails = 375773780U,
			QuotaWarningFoldersCountDetails = 2271073400U,
			E4EReceivedMessage = 799906996U,
			ExPartnerMailDisplayName = 3093270387U,
			ExAttachmentRemovedDisplayName = 4198466683U,
			ShortText5_3_3 = 977621114U,
			QuotaWarningDetailsPF = 2308350314U,
			ArchiveQuotaWarningNoLimitDetails = 2844551000U,
			GeneratingServerTitle = 4076586603U,
			QuotaSendReceiveSubject = 1431455589U,
			ShortText5_1_8 = 3632599111U,
			ShortText5_1_3 = 3585544944U,
			ExOrarMailRecipientDescription = 159863259U,
			ArchiveQuotaFullSubject = 1343379807U,
			QuotaWarningNoLimitDetailsPF = 2333692916U,
			ShortText5_4_4 = 2402542554U,
			HumanText_InitMsg = 2088818508U,
			HumanText5_7_300 = 322888411U,
			BodyReceiveRMEmail = 4249345458U,
			QuotaWarningFoldersCountSubject = 1733655566U,
			QuotaWarningDetails = 3171360660U,
			HumanText5_3_0 = 3080592406U,
			HumanText5_7_100 = 605213413U,
			HumanText5_1_6 = 3604836458U,
			ShortText5_7_7 = 2382339578U,
			QuotaWarningFolderHierarchyChildrenNoLimitSubject = 347238620U,
			ShortText5_7_0 = 3141854465U,
			HumanText5_5_0 = 3362917408U,
			QuotaProhibitReceiveFolderHierarchyChildrenCountDetails = 1321639685U,
			ShortText5_1_0 = 3988829471U,
			QuotaWarningFoldersCountNoLimitSubject = 2455731400U,
			HumanText5_4_0 = 1776630491U,
			ExAttachmentRemovedRecipientDescription = 750807939U,
			QuotaCurrentSize = 3452592194U,
			DecisionUpdateTopText = 2915608171U,
			HumanText5_4_1 = 3342714432U,
			ShortText5_4_6 = 3565341968U,
			ShortText5_7_6 = 3948423519U,
			ApprovalRequestSubjectPrefix = 1242161506U,
			E4ESignIn = 4133660744U,
			InternetConfidentialName = 831975206U,
			HumanText5_7_7 = 1272589415U,
			ShortText5_7_3 = 412971110U,
			QuotaWarningNoLimitDetails = 452614234U,
			E4EEncryptedMessage = 1230109185U,
			HumanText5_5_1 = 1796833467U,
			DataCenterHumanText5_7_1 = 3316231944U,
			ShortText5_5_6 = 2261380053U,
			QuotaWarningFolderHierarchyChildrenCountDetails = 1525463967U,
			ShortText5_7_9 = 1219540164U,
			ShortText5_1_2 = 856661589U,
			QuotaWarningMailboxMessagesPerFolderCountDetails = 859070570U,
			HumanText5_7_3 = 3241957883U,
			DoNotForwardDescription = 1793216387U,
			ModerationExpiryNoticationForSenderTopText = 4019621683U,
			HumanText5_4_7 = 210546550U,
			DataCenterHumanText4_4_7 = 1569096212U,
			HumanText5_6_3 = 492871552U,
			HumanText5_3_2 = 4243391820U,
			ApprovalCommentAttachmentFilename = 3124761826U,
			ArchiveQuotaWarningNoLimitSubject = 2375073438U,
			HumanText5_1_4 = 472668576U,
			E4EHosted = 2988353893U,
			QuotaSendReceiveDetails = 3224920035U,
			ShortText5_2_4 = 3041097916U,
			DSNEnhanced_5_7_1_RESOLVER_RST_NotAuthorized = 843859989U,
			ShortText5_5_4 = 1098580639U,
			QuotaWarningMailboxMessagesPerFolderNoLimitDetails = 86736107U,
			ShortText5_6_4 = 2120217552U,
			ShortText5_1_4 = 2019461003U,
			HumanText5_1_2 = 1635467990U,
			QuotaWarningFolderHierarchyChildrenCountSubject = 3715345541U,
			HumanText5_5_2 = 2200117994U,
			ShortText5_4_0 = 433174086U,
			HumanText5_7_1 = 2079158469U,
			HumanText5_2_4 = 3463673957U,
			DSNEnhanced_5_2_3_RESOLVER_RST_RecipSizeLimit = 357259713U,
			E4EWaitMessage = 20682666U,
			ApprovalRequestExpiryTopText = 1418666303U,
			E4EViewMessageButton = 1835138854U,
			ShortText5_6_3 = 554133611U,
			HumanText5_1_1 = 1232183463U,
			Moderation_Reencryption_Exception = 2489798467U,
			ShortText5_6_0 = 150849084U,
			ShortText5_6_1 = 1716933025U,
			ApprovalRequestExpiredNotificationSubjectPrefix = 2028225464U,
			HumanText5_5_4 = 1393548940U,
			ShortText5_4_1 = 1999258027U,
			QuotaWarningNoLimitSubject = 2807992444U,
			DecisionConflictTopText = 724420710U,
			FailedHumanReadableTopTextForTextMessage = 2707856071U,
			HumanText5_6_2 = 3221754907U,
			QuotaSendDetailsPF = 96782060U,
			HumanText5_7_6 = 2838673356U,
			QuotaSendReceiveTopText = 2912990139U,
			HumanText5_2_3 = 4223188844U,
			ApproveButtonText = 3163998196U,
			DiagnosticInformationTitle = 3827208783U,
			HumanText5_4_6 = 2939429905U,
			HumanText5_5_6 = 230749526U,
			ShortText5_5_5 = 3827463994U,
			ShortText5_7_5 = 3545138992U,
			DSNEnhanced_5_7_1_TRANSPORT_RULES_RejectMessage = 2279047500U,
			HumanText5_3_1 = 1514508465U,
			ShortText5_3_1 = 2140420528U,
			ShortText5_3_2 = 2543705055U,
			ShortText5_7_10 = 2107776300U
		}

		private enum ParamIDs
		{
			HumanReadableBoldedSubjectLine,
			DsnParamTextMessageSizePerRecipientInMB,
			QuotaProhibitReceiveFoldersCountTopText,
			QuotaWarningFolderHierarchyDepthTopText,
			QuotaProhibitReceiveFolderHierarchyDepthTopText,
			ModeratedDLApprovalRequestRecipientList,
			QuotaWarningFoldersCountNoLimitTopText,
			QuotaProhibitReceiveMailboxMessagesPerFolderCountTopText,
			DsnParamTextRecipientCount,
			HumanTextFailedSmtpToSmsGatewayNotification,
			QuotaWarningFolderHierarchyDepthNoLimitTopText,
			ArchiveQuotaWarningNoLimitTopText,
			HumanReadableBoldedToLine,
			DelayedHumanReadableBottomTextHours,
			E4EOpenAttachment,
			QuotaWarningFolderHierarchyChildrenNoLimitTopText,
			ModeratedDLApprovalRequest,
			QuotaProhibitReceiveFolderHierarchyChildrenCountTopText,
			ExternalFailedHumanReadableErrorText,
			DecisionConflictWithDetailsNotification,
			DsnParamTextMessageSizePerMessageInMB,
			DecisionConflictNotification,
			HumanReadableBoldedCcLine,
			ModeratorExpiryNotification,
			ApprovalRequestExpiryNotification,
			ExternalFailedHumanReadableTopText,
			QuotaWarningMailboxMessagesPerFolderCountTopText,
			ExternalFailedHumanReadableErrorNoDetailText,
			QuotaWarningFoldersCountTopText,
			SizeInFolders,
			HumanReadableSubjectLine,
			DelayedHumanReadableBottomText,
			DsnParamTextMessageSizePerMessageInKB,
			DsnParamTextMessageSizePerRecipientInKB,
			QuotaWarningTopTextPF,
			QuotaSendTopTextPF,
			QuotaWarningNoLimitTopTextPF,
			QuotaWarningNoLimitTopText,
			QuotaWarningMailboxMessagesPerFolderNoLimitTopText,
			HumanTextFailedPasscodeWithReason,
			SizeInMessages,
			HumanReadableBoldedFromLine,
			HumanTextFailedPasscodeWithoutReason,
			QuotaWarningFolderHierarchyChildrenCountTopText,
			HumanTextFailedOmsNotification,
			HumanReadableRemoteServerText,
			SizeInMB
		}
	}
}
