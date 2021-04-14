using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	public static class ClientStrings
	{
		static ClientStrings()
		{
			ClientStrings.stringIDs.Add(4109493280U, "PostedTo");
			ClientStrings.stringIDs.Add(3201544700U, "UnknownDelegateUser");
			ClientStrings.stringIDs.Add(2520011618U, "ClutterNotificationInvitationIfYouDontLikeIt");
			ClientStrings.stringIDs.Add(2979702410U, "Inbox");
			ClientStrings.stringIDs.Add(2660925179U, "CategoryKqlKeyword");
			ClientStrings.stringIDs.Add(2842725608U, "RequestedActionReply");
			ClientStrings.stringIDs.Add(699667745U, "ClutterNotificationAutoEnablementNoticeHeader");
			ClientStrings.stringIDs.Add(605024017U, "MeetingTentative");
			ClientStrings.stringIDs.Add(3115038347U, "BodyKqlKeyword");
			ClientStrings.stringIDs.Add(2958797346U, "WhenAllDays");
			ClientStrings.stringIDs.Add(1567502238U, "GroupMailboxWelcomeEmailFooterUnsubscribeLinkText");
			ClientStrings.stringIDs.Add(800057761U, "TaskWhenDailyRegeneratingPattern");
			ClientStrings.stringIDs.Add(1082093008U, "UpdateRumLocationFlag");
			ClientStrings.stringIDs.Add(3496891301U, "CcColon");
			ClientStrings.stringIDs.Add(3728294467U, "GroupMailboxWelcomeEmailUnsubscribeToInboxTitle");
			ClientStrings.stringIDs.Add(729677225U, "HighKqlKeyword");
			ClientStrings.stringIDs.Add(2868846894U, "OnBehalfOf");
			ClientStrings.stringIDs.Add(3636275243U, "SentKqlKeyword");
			ClientStrings.stringIDs.Add(996097864U, "UpdateRumCancellationFlag");
			ClientStrings.stringIDs.Add(2420384142U, "PrivateAppointmentSubject");
			ClientStrings.stringIDs.Add(2485040215U, "GroupMailboxWelcomeMessageHeader2");
			ClientStrings.stringIDs.Add(2052801377U, "Busy");
			ClientStrings.stringIDs.Add(2694011047U, "CalendarWhenEveryOtherDay");
			ClientStrings.stringIDs.Add(2530535283U, "WhenWeekDays");
			ClientStrings.stringIDs.Add(2983597720U, "WhenOnEveryDayOfTheWeek");
			ClientStrings.stringIDs.Add(186518193U, "ClutterNotificationAutoEnablementNoticeHowItWorks");
			ClientStrings.stringIDs.Add(1289378056U, "RequestedActionRead");
			ClientStrings.stringIDs.Add(3076491009U, "GroupMailboxWelcomeEmailPublicTypeText");
			ClientStrings.stringIDs.Add(4206219086U, "PolicyTipDefaultMessageRejectOverride");
			ClientStrings.stringIDs.Add(3798065398U, "WhenLast");
			ClientStrings.stringIDs.Add(920038251U, "GroupMailboxWelcomeEmailUnsubscribeToInboxSubtitle");
			ClientStrings.stringIDs.Add(3798064771U, "WhenPart");
			ClientStrings.stringIDs.Add(998340857U, "GroupMailboxAddedMemberMessageDocument2");
			ClientStrings.stringIDs.Add(3514687136U, "UpdateRumStartTimeFlag");
			ClientStrings.stringIDs.Add(3831476238U, "MyContactsFolderName");
			ClientStrings.stringIDs.Add(2537597608U, "DocumentSyncIssues");
			ClientStrings.stringIDs.Add(2892702617U, "MeetingAccept");
			ClientStrings.stringIDs.Add(3827225687U, "SearchFolders");
			ClientStrings.stringIDs.Add(77678270U, "OOF");
			ClientStrings.stringIDs.Add(28323196U, "GroupMailboxWelcomeEmailFooterUnsubscribeDescirptionText");
			ClientStrings.stringIDs.Add(3435327809U, "GroupMailboxWelcomeEmailShareFilesSubTitle");
			ClientStrings.stringIDs.Add(2986714461U, "GroupMailboxWelcomeEmailPrivateTypeText");
			ClientStrings.stringIDs.Add(4003125105U, "GroupMailboxWelcomeEmailFooterSubscribeLinkText");
			ClientStrings.stringIDs.Add(295620541U, "SentColon");
			ClientStrings.stringIDs.Add(109016526U, "ClutterNotificationO365Closing");
			ClientStrings.stringIDs.Add(606093953U, "MyCalendars");
			ClientStrings.stringIDs.Add(2337243514U, "AttachmentNamesKqlKeyword");
			ClientStrings.stringIDs.Add(3055530828U, "UMFaxFolderName");
			ClientStrings.stringIDs.Add(4038690134U, "GroupMailboxSuggestToSubscribe");
			ClientStrings.stringIDs.Add(3709219576U, "WherePart");
			ClientStrings.stringIDs.Add(2985148172U, "MeetingProposedNewTime");
			ClientStrings.stringIDs.Add(2966158940U, "Tasks");
			ClientStrings.stringIDs.Add(2957657555U, "ClutterNotificationAutoEnablementNoticeWeCallIt");
			ClientStrings.stringIDs.Add(2243726652U, "Configuration");
			ClientStrings.stringIDs.Add(2687792634U, "ReceivedKqlKeyword");
			ClientStrings.stringIDs.Add(2676822854U, "ItemForward");
			ClientStrings.stringIDs.Add(4037918816U, "PublicFolderMailboxHierarchyInfo");
			ClientStrings.stringIDs.Add(998340860U, "GroupMailboxAddedMemberMessageDocument1");
			ClientStrings.stringIDs.Add(1139539543U, "GroupMailboxAddedSelfMessageHeader");
			ClientStrings.stringIDs.Add(627342781U, "GroupSubscriptionPageSubscribedInfo");
			ClientStrings.stringIDs.Add(2377878649U, "TaskWhenMonthlyRegeneratingPattern");
			ClientStrings.stringIDs.Add(2677919833U, "SentTime");
			ClientStrings.stringIDs.Add(590977256U, "SentItems");
			ClientStrings.stringIDs.Add(931240115U, "UpdateRumDuplicateFlags");
			ClientStrings.stringIDs.Add(3416726118U, "ToDoSearch");
			ClientStrings.stringIDs.Add(831219486U, "GroupMailboxWelcomeEmailO365FooterBrowseViewCalendar");
			ClientStrings.stringIDs.Add(586169060U, "UserPhotoNotAnImage");
			ClientStrings.stringIDs.Add(51726234U, "OofReply");
			ClientStrings.stringIDs.Add(2992898169U, "UpdateRumDescription");
			ClientStrings.stringIDs.Add(1190007884U, "SharingRequestDenied");
			ClientStrings.stringIDs.Add(3198424631U, "CommonViews");
			ClientStrings.stringIDs.Add(257954985U, "TaskWhenDailyEveryDay");
			ClientStrings.stringIDs.Add(2561496264U, "NoDataAvailable");
			ClientStrings.stringIDs.Add(3430669166U, "Root");
			ClientStrings.stringIDs.Add(3011322582U, "KoreanLunar");
			ClientStrings.stringIDs.Add(3694564633U, "SyncIssues");
			ClientStrings.stringIDs.Add(2696977948U, "UnifiedInbox");
			ClientStrings.stringIDs.Add(843955115U, "ContactSubjectFormat");
			ClientStrings.stringIDs.Add(2093606811U, "ChineseLunar");
			ClientStrings.stringIDs.Add(629464291U, "Outbox");
			ClientStrings.stringIDs.Add(2692137911U, "UpdateRumInconsistencyFlagsLabel");
			ClientStrings.stringIDs.Add(2173140516U, "Hijri");
			ClientStrings.stringIDs.Add(946577161U, "MeetingsKqlKeyword");
			ClientStrings.stringIDs.Add(2717587388U, "ClutterNotificationInvitationHowItWorks");
			ClientStrings.stringIDs.Add(998340858U, "GroupMailboxAddedMemberMessageDocument3");
			ClientStrings.stringIDs.Add(732114395U, "UpdateRumRecurrenceFlag");
			ClientStrings.stringIDs.Add(1261883245U, "PolicyTagKqlKeyword");
			ClientStrings.stringIDs.Add(3061781790U, "GroupSubscriptionPageBodyFont");
			ClientStrings.stringIDs.Add(780435470U, "ClutterNotificationOptInSubject");
			ClientStrings.stringIDs.Add(2831541605U, "ClutterNotificationAutoEnablementNoticeItsAutomatic");
			ClientStrings.stringIDs.Add(1987690819U, "WorkingElsewhere");
			ClientStrings.stringIDs.Add(1210128587U, "NTUser");
			ClientStrings.stringIDs.Add(3623590716U, "ElcRoot");
			ClientStrings.stringIDs.Add(3637574683U, "ContactFullNameFormat");
			ClientStrings.stringIDs.Add(2442729051U, "GroupMailboxAddedMemberMessageCalendar1");
			ClientStrings.stringIDs.Add(3064946485U, "WhenFifth");
			ClientStrings.stringIDs.Add(690865655U, "ClutterNotificationInvitationIntro");
			ClientStrings.stringIDs.Add(725603143U, "ExpiresKqlKeyword");
			ClientStrings.stringIDs.Add(1486137843U, "GroupMailboxAddedMemberMessageConversation2");
			ClientStrings.stringIDs.Add(462355316U, "CommunicatorHistoryFolderName");
			ClientStrings.stringIDs.Add(898888055U, "LowKqlKeyword");
			ClientStrings.stringIDs.Add(3493739462U, "AllKqlKeyword");
			ClientStrings.stringIDs.Add(3753304505U, "GroupMailboxAddedMemberToPublicGroupMessage");
			ClientStrings.stringIDs.Add(1712196986U, "Followup");
			ClientStrings.stringIDs.Add(1669914831U, "HebrewLunar");
			ClientStrings.stringIDs.Add(1601836855U, "Notes");
			ClientStrings.stringIDs.Add(4285257241U, "UpdateRumEndTimeFlag");
			ClientStrings.stringIDs.Add(4086680553U, "JournalsKqlKeyword");
			ClientStrings.stringIDs.Add(2395883303U, "AttendeeInquiryRumDescription");
			ClientStrings.stringIDs.Add(1358110507U, "ClutterNotificationOptInHeader");
			ClientStrings.stringIDs.Add(2274376870U, "LegacyPDLFakeEntry");
			ClientStrings.stringIDs.Add(2426305242U, "PolicyTipDefaultMessageReject");
			ClientStrings.stringIDs.Add(637088748U, "VoicemailKqlKeyword");
			ClientStrings.stringIDs.Add(2543409328U, "PostedOn");
			ClientStrings.stringIDs.Add(20079527U, "MeetingCancel");
			ClientStrings.stringIDs.Add(2918743951U, "FromColon");
			ClientStrings.stringIDs.Add(3613623199U, "DeletedItems");
			ClientStrings.stringIDs.Add(242457402U, "DocsKqlKeyword");
			ClientStrings.stringIDs.Add(3850103120U, "ClutterNotificationO365DisplayName");
			ClientStrings.stringIDs.Add(2773211886U, "LocalFailures");
			ClientStrings.stringIDs.Add(3659541315U, "GroupMailboxWelcomeEmailO365FooterBrowseConversations");
			ClientStrings.stringIDs.Add(2676513095U, "JapaneseLunar");
			ClientStrings.stringIDs.Add(3411057958U, "ClutterNotificationPeriodicSurveySteps");
			ClientStrings.stringIDs.Add(3709255463U, "TaskStatusInProgress");
			ClientStrings.stringIDs.Add(4078173350U, "AutomaticDisplayName");
			ClientStrings.stringIDs.Add(587246061U, "SharingInvitationInstruction");
			ClientStrings.stringIDs.Add(2610926242U, "BirthdayCalendarFolderName");
			ClientStrings.stringIDs.Add(3519087566U, "NormalKqlKeyword");
			ClientStrings.stringIDs.Add(2937701464U, "ClutterNotificationPeriodicCheckBack");
			ClientStrings.stringIDs.Add(104437952U, "ToKqlKeyword");
			ClientStrings.stringIDs.Add(115734878U, "Drafts");
			ClientStrings.stringIDs.Add(3043371929U, "MeetingDecline");
			ClientStrings.stringIDs.Add(3230188362U, "WhenSecond");
			ClientStrings.stringIDs.Add(232501731U, "SubjectKqlKeyword");
			ClientStrings.stringIDs.Add(2664647494U, "ServerFailures");
			ClientStrings.stringIDs.Add(1134735502U, "ClutterNotificationInvitationHeader");
			ClientStrings.stringIDs.Add(1618289846U, "UMVoiceMailFolderName");
			ClientStrings.stringIDs.Add(1744667246U, "CalendarWhenDailyEveryDay");
			ClientStrings.stringIDs.Add(954396011U, "WhenThird");
			ClientStrings.stringIDs.Add(2864414954U, "ClutterNotificationOptInPrivacy");
			ClientStrings.stringIDs.Add(1025048278U, "SharingInvitationUpdatedSubjectLine");
			ClientStrings.stringIDs.Add(4052458853U, "OtherCalendars");
			ClientStrings.stringIDs.Add(1178930756U, "GroupMailboxWelcomeEmailSubscribeToInboxSubtitle");
			ClientStrings.stringIDs.Add(1797669216U, "Tentative");
			ClientStrings.stringIDs.Add(2411966652U, "AttachmentKqlKeyword");
			ClientStrings.stringIDs.Add(3710484300U, "WhenOnWeekDays");
			ClientStrings.stringIDs.Add(3291021260U, "ClutterNotificationHeaderFont");
			ClientStrings.stringIDs.Add(3252707377U, "TaskWhenWeeklyRegeneratingPattern");
			ClientStrings.stringIDs.Add(4048827998U, "FaxesKqlKeyword");
			ClientStrings.stringIDs.Add(3033231921U, "TaskStatusNotStarted");
			ClientStrings.stringIDs.Add(3465339554U, "ToColon");
			ClientStrings.stringIDs.Add(3882605011U, "IsReadKqlKeyword");
			ClientStrings.stringIDs.Add(2572393147U, "IsFlaggedKqlKeyword");
			ClientStrings.stringIDs.Add(136108241U, "CancellationRumTitle");
			ClientStrings.stringIDs.Add(2244917544U, "ClutterNotificationPeriodicHeader");
			ClientStrings.stringIDs.Add(3855098919U, "GroupSubscriptionPageRequestFailedInfo");
			ClientStrings.stringIDs.Add(3366209149U, "ClutterNotificationInvitationSubject");
			ClientStrings.stringIDs.Add(2530009445U, "FromKqlKeyword");
			ClientStrings.stringIDs.Add(3938988508U, "GroupSubscriptionPageUnsubscribedInfo");
			ClientStrings.stringIDs.Add(1708086969U, "GroupMailboxAddedMemberToPrivateGroupMessage");
			ClientStrings.stringIDs.Add(4132923658U, "WhenOneMoreOccurrence");
			ClientStrings.stringIDs.Add(4182366305U, "GroupSubscriptionUnsubscribeLinkWord");
			ClientStrings.stringIDs.Add(1233830609U, "PublicFolderMailboxDumpsterInfo");
			ClientStrings.stringIDs.Add(2188469818U, "TaskStatusWaitOnOthers");
			ClientStrings.stringIDs.Add(2197783321U, "GroupMailboxWelcomeEmailO365FooterBrowseShareFiles");
			ClientStrings.stringIDs.Add(433190147U, "ClutterNotificationPeriodicSubject");
			ClientStrings.stringIDs.Add(2755014636U, "GroupMailboxAddedMemberNoJoinedByMessageHeader");
			ClientStrings.stringIDs.Add(3895923139U, "RequestedActionNoResponseNecessary");
			ClientStrings.stringIDs.Add(1562762538U, "TaskWhenYearlyRegeneratingPattern");
			ClientStrings.stringIDs.Add(3323263744U, "Free");
			ClientStrings.stringIDs.Add(1871485283U, "CancellationRumDescription");
			ClientStrings.stringIDs.Add(99688204U, "RequestedActionReplyToAll");
			ClientStrings.stringIDs.Add(3733626243U, "GroupMailboxWelcomeEmailO365FooterTitle");
			ClientStrings.stringIDs.Add(2403605275U, "ItemReply");
			ClientStrings.stringIDs.Add(3824460724U, "WhenFirst");
			ClientStrings.stringIDs.Add(483059378U, "RpcClientInitError");
			ClientStrings.stringIDs.Add(348255911U, "RequestedActionDoNotForward");
			ClientStrings.stringIDs.Add(2074560464U, "SizeKqlKeyword");
			ClientStrings.stringIDs.Add(2936941939U, "GroupSubscriptionPagePrivateGroupInfo");
			ClientStrings.stringIDs.Add(693971404U, "UserPhotoNotFound");
			ClientStrings.stringIDs.Add(2768584303U, "FromFavoriteSendersFolderName");
			ClientStrings.stringIDs.Add(2869025391U, "GroupMailboxAddedMemberMessageFont");
			ClientStrings.stringIDs.Add(2905210277U, "RequestedActionForward");
			ClientStrings.stringIDs.Add(4167892423U, "CcKqlKeyword");
			ClientStrings.stringIDs.Add(1697583518U, "Conversations");
			ClientStrings.stringIDs.Add(3614445595U, "GroupSubscriptionPagePublicGroupInfo");
			ClientStrings.stringIDs.Add(677901016U, "ContactsKqlKeyword");
			ClientStrings.stringIDs.Add(3513814141U, "WhenBothWeekendDays");
			ClientStrings.stringIDs.Add(2377377097U, "Conflicts");
			ClientStrings.stringIDs.Add(51369050U, "PostsKqlKeyword");
			ClientStrings.stringIDs.Add(1940443510U, "UserPhotoPreviewNotFound");
			ClientStrings.stringIDs.Add(1236916945U, "WhenEveryDay");
			ClientStrings.stringIDs.Add(1844900681U, "SharingRequestInstruction");
			ClientStrings.stringIDs.Add(940910702U, "ClutterNotificationAutoEnablementNoticeIntro");
			ClientStrings.stringIDs.Add(1418871467U, "ClutterNotificationBodyFont");
			ClientStrings.stringIDs.Add(1685151768U, "ClutterNotificationInvitationO365Helps");
			ClientStrings.stringIDs.Add(3953967230U, "WhenFourth");
			ClientStrings.stringIDs.Add(1657114432U, "RequestedActionFollowUp");
			ClientStrings.stringIDs.Add(3221177017U, "GroupSubscriptionPageRequestFailed");
			ClientStrings.stringIDs.Add(1300788680U, "ClutterNotificationAutoEnablementNoticeSubject");
			ClientStrings.stringIDs.Add(2028589045U, "PeoplesCalendars");
			ClientStrings.stringIDs.Add(1660374630U, "EHAMigration");
			ClientStrings.stringIDs.Add(3413891549U, "SubjectColon");
			ClientStrings.stringIDs.Add(1292798904U, "Calendar");
			ClientStrings.stringIDs.Add(899464318U, "RequestedActionAny");
			ClientStrings.stringIDs.Add(2912044233U, "WhenEveryWeekDay");
			ClientStrings.stringIDs.Add(2607340737U, "TasksKqlKeyword");
			ClientStrings.stringIDs.Add(802022316U, "NotesKqlKeyword");
			ClientStrings.stringIDs.Add(1726717990U, "TaskStatusCompleted");
			ClientStrings.stringIDs.Add(3501929050U, "ClutterNotificationInvitationItsAutomatic");
			ClientStrings.stringIDs.Add(3598244064U, "RssSubscriptions");
			ClientStrings.stringIDs.Add(153688296U, "RequestedActionCall");
			ClientStrings.stringIDs.Add(3795294554U, "PolicyTipDefaultMessageNotifyOnly");
			ClientStrings.stringIDs.Add(219237725U, "FailedToUploadToSharePointTitle");
			ClientStrings.stringIDs.Add(2255519906U, "TaskStatusDeferred");
			ClientStrings.stringIDs.Add(3425704829U, "SharingRequestAllowed");
			ClientStrings.stringIDs.Add(1694200734U, "UpdateRumMissingItemFlag");
			ClientStrings.stringIDs.Add(963851651U, "ParticipantsKqlKeyword");
			ClientStrings.stringIDs.Add(1044563024U, "GroupMailboxWelcomeEmailStartConversationTitle");
			ClientStrings.stringIDs.Add(2833103505U, "GroupMailboxWelcomeEmailShareFilesTitle");
			ClientStrings.stringIDs.Add(3635271833U, "LegacyArchiveJournals");
			ClientStrings.stringIDs.Add(3126868667U, "RecipientsKqlKeyword");
			ClientStrings.stringIDs.Add(2904245352U, "RssFeedsKqlKeyword");
			ClientStrings.stringIDs.Add(2776152054U, "GroupMailboxWelcomeEmailSubscribeToInboxTitle");
			ClientStrings.stringIDs.Add(2442729049U, "GroupMailboxAddedMemberMessageCalendar3");
			ClientStrings.stringIDs.Add(1616483908U, "RequestedActionReview");
			ClientStrings.stringIDs.Add(3043633671U, "KindKqlKeyword");
			ClientStrings.stringIDs.Add(916455203U, "EmailKqlKeyword");
			ClientStrings.stringIDs.Add(2450331336U, "UTC");
			ClientStrings.stringIDs.Add(1793503025U, "BccKqlKeyword");
			ClientStrings.stringIDs.Add(3029761970U, "RequestedActionForYourInformation");
			ClientStrings.stringIDs.Add(4137480277U, "Journal");
			ClientStrings.stringIDs.Add(1486137846U, "GroupMailboxAddedMemberMessageConversation1");
			ClientStrings.stringIDs.Add(1486137844U, "GroupMailboxAddedMemberMessageConversation3");
			ClientStrings.stringIDs.Add(136364712U, "HasAttachmentKqlKeyword");
			ClientStrings.stringIDs.Add(1780326221U, "GroupSubscriptionPageGroupInfoFont");
			ClientStrings.stringIDs.Add(1716044995U, "Contacts");
			ClientStrings.stringIDs.Add(2442729048U, "GroupMailboxAddedMemberMessageCalendar2");
			ClientStrings.stringIDs.Add(1836161108U, "FavoritesFolderName");
			ClientStrings.stringIDs.Add(1516506571U, "ImKqlKeyword");
			ClientStrings.stringIDs.Add(2755562843U, "ImportanceKqlKeyword");
			ClientStrings.stringIDs.Add(1165698602U, "MyTasks");
			ClientStrings.stringIDs.Add(143815687U, "SharingInvitationAndRequestInstruction");
			ClientStrings.stringIDs.Add(632725474U, "TaskWhenEveryOtherDay");
			ClientStrings.stringIDs.Add(710925581U, "Conversation");
			ClientStrings.stringIDs.Add(3191048500U, "FailedToDeleteFromSharePointTitle");
			ClientStrings.stringIDs.Add(2241039844U, "JunkEmail");
			ClientStrings.stringIDs.Add(765381967U, "GroupMailboxWelcomeEmailFooterSubscribeDescriptionText");
			ClientStrings.stringIDs.Add(1976982380U, "ClutterFolderName");
			ClientStrings.stringIDs.Add(261418034U, "ClutterNotificationInvitationWeCallIt");
			ClientStrings.stringIDs.Add(1524084310U, "GroupMailboxWelcomeEmailDefaultDescription");
		}

		public static LocalizedString PostedTo
		{
			get
			{
				return new LocalizedString("PostedTo", "Ex6A8EA5", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClutterNotificationAutoEnablementNoticeYoullBeEnabed(string optOutUrl)
		{
			return new LocalizedString("ClutterNotificationAutoEnablementNoticeYoullBeEnabed", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				optOutUrl
			});
		}

		public static LocalizedString UnknownDelegateUser
		{
			get
			{
				return new LocalizedString("UnknownDelegateUser", "Ex18B5E4", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToSynchronizeChangesFromSharePoint(string sharePointUrl)
		{
			return new LocalizedString("FailedToSynchronizeChangesFromSharePoint", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				sharePointUrl
			});
		}

		public static LocalizedString ClutterNotificationInvitationIfYouDontLikeIt
		{
			get
			{
				return new LocalizedString("ClutterNotificationInvitationIfYouDontLikeIt", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Inbox
		{
			get
			{
				return new LocalizedString("Inbox", "Ex1DF479", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClutterNotificationAutoEnablementNoticeLearnMore(string clutterAnnouncementUrl, string clutterLearnMoreUrl)
		{
			return new LocalizedString("ClutterNotificationAutoEnablementNoticeLearnMore", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				clutterAnnouncementUrl,
				clutterLearnMoreUrl
			});
		}

		public static LocalizedString RemindersSearchFolderName(string uniqueSuffix)
		{
			return new LocalizedString("RemindersSearchFolderName", "Ex69B0D6", false, true, ClientStrings.ResourceManager, new object[]
			{
				uniqueSuffix
			});
		}

		public static LocalizedString CategoryKqlKeyword
		{
			get
			{
				return new LocalizedString("CategoryKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequestedActionReply
		{
			get
			{
				return new LocalizedString("RequestedActionReply", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClutterNotificationAutoEnablementNoticeHeader
		{
			get
			{
				return new LocalizedString("ClutterNotificationAutoEnablementNoticeHeader", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MeetingTentative
		{
			get
			{
				return new LocalizedString("MeetingTentative", "Ex762E2C", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BodyKqlKeyword
		{
			get
			{
				return new LocalizedString("BodyKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenAllDays
		{
			get
			{
				return new LocalizedString("WhenAllDays", "Ex5B0CF6", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxWelcomeEmailFooterUnsubscribeLinkText
		{
			get
			{
				return new LocalizedString("GroupMailboxWelcomeEmailFooterUnsubscribeLinkText", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TaskWhenDailyRegeneratingPattern
		{
			get
			{
				return new LocalizedString("TaskWhenDailyRegeneratingPattern", "Ex6C7B28", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MdnReadNoTime(LocalizedString messageInfo)
		{
			return new LocalizedString("MdnReadNoTime", "ExC8D803", false, true, ClientStrings.ResourceManager, new object[]
			{
				messageInfo
			});
		}

		public static LocalizedString UpdateRumLocationFlag
		{
			get
			{
				return new LocalizedString("UpdateRumLocationFlag", "Ex99408F", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedMaintenanceSynchronizationsText(string sharePointUrl, string error)
		{
			return new LocalizedString("FailedMaintenanceSynchronizationsText", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				sharePointUrl,
				error
			});
		}

		public static LocalizedString CcColon
		{
			get
			{
				return new LocalizedString("CcColon", "Ex9B53F3", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxWelcomeEmailUnsubscribeToInboxTitle
		{
			get
			{
				return new LocalizedString("GroupMailboxWelcomeEmailUnsubscribeToInboxTitle", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HighKqlKeyword
		{
			get
			{
				return new LocalizedString("HighKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarWhenYearly(IFormattable month, int day)
		{
			return new LocalizedString("CalendarWhenYearly", "ExE68E1B", false, true, ClientStrings.ResourceManager, new object[]
			{
				month,
				day
			});
		}

		public static LocalizedString OnBehalfOf
		{
			get
			{
				return new LocalizedString("OnBehalfOf", "ExC7CC16", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SentKqlKeyword
		{
			get
			{
				return new LocalizedString("SentKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateRumCancellationFlag
		{
			get
			{
				return new LocalizedString("UpdateRumCancellationFlag", "ExE60B17", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PrivateAppointmentSubject
		{
			get
			{
				return new LocalizedString("PrivateAppointmentSubject", "ExBAF266", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlternateCalendarWhenMonthlyEveryNMonths(LocalizedString calendar, int day, int interval)
		{
			return new LocalizedString("AlternateCalendarWhenMonthlyEveryNMonths", "ExEA6399", false, true, ClientStrings.ResourceManager, new object[]
			{
				calendar,
				day,
				interval
			});
		}

		public static LocalizedString GroupMailboxWelcomeMessageHeader2
		{
			get
			{
				return new LocalizedString("GroupMailboxWelcomeMessageHeader2", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Busy
		{
			get
			{
				return new LocalizedString("Busy", "ExC0FED8", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarWhenEveryOtherDay
		{
			get
			{
				return new LocalizedString("CalendarWhenEveryOtherDay", "ExC2FA6D", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlternateCalendarWhenYearlyThLeap(LocalizedString calendar, LocalizedString order, IFormattable dayOfWeek, IFormattable month)
		{
			return new LocalizedString("AlternateCalendarWhenYearlyThLeap", "Ex74959D", false, true, ClientStrings.ResourceManager, new object[]
			{
				calendar,
				order,
				dayOfWeek,
				month
			});
		}

		public static LocalizedString WhenWeekDays
		{
			get
			{
				return new LocalizedString("WhenWeekDays", "Ex6DE869", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenOnEveryDayOfTheWeek
		{
			get
			{
				return new LocalizedString("WhenOnEveryDayOfTheWeek", "Ex7511ED", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToSynchronizeChangesFromSharePointText(string sharePointDocumentLibUrl, string error)
		{
			return new LocalizedString("FailedToSynchronizeChangesFromSharePointText", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				sharePointDocumentLibUrl,
				error
			});
		}

		public static LocalizedString ClutterNotificationAutoEnablementNoticeHowItWorks
		{
			get
			{
				return new LocalizedString("ClutterNotificationAutoEnablementNoticeHowItWorks", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequestedActionRead
		{
			get
			{
				return new LocalizedString("RequestedActionRead", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxWelcomeEmailPublicTypeText
		{
			get
			{
				return new LocalizedString("GroupMailboxWelcomeEmailPublicTypeText", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClutterNotificationOptInHowToTrain(string clutterFolderName)
		{
			return new LocalizedString("ClutterNotificationOptInHowToTrain", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				clutterFolderName
			});
		}

		public static LocalizedString PolicyTipDefaultMessageRejectOverride
		{
			get
			{
				return new LocalizedString("PolicyTipDefaultMessageRejectOverride", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenLast
		{
			get
			{
				return new LocalizedString("WhenLast", "Ex6DEADF", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxWelcomeEmailUnsubscribeToInboxSubtitle
		{
			get
			{
				return new LocalizedString("GroupMailboxWelcomeEmailUnsubscribeToInboxSubtitle", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenRecurringNoEndDateDaysHours(LocalizedString pattern, object startDate, object startTime, int days, int hours)
		{
			return new LocalizedString("WhenRecurringNoEndDateDaysHours", "ExA993DA", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				startTime,
				days,
				hours
			});
		}

		public static LocalizedString WhenRecurringWithEndDateNoDuration(LocalizedString pattern, object startDate, object endDate, object startTime)
		{
			return new LocalizedString("WhenRecurringWithEndDateNoDuration", "ExB2E4D5", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				endDate,
				startTime
			});
		}

		public static LocalizedString WhenPart
		{
			get
			{
				return new LocalizedString("WhenPart", "ExE5D896", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxAddedMemberMessageDocument2
		{
			get
			{
				return new LocalizedString("GroupMailboxAddedMemberMessageDocument2", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToUploadToSharePointErrorText(string fileName, string sharePointFolderUrl)
		{
			return new LocalizedString("FailedToUploadToSharePointErrorText", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				fileName,
				sharePointFolderUrl
			});
		}

		public static LocalizedString SharingInvitationAndRequest(string user, string email, LocalizedString foldertype)
		{
			return new LocalizedString("SharingInvitationAndRequest", "ExD48D4B", false, true, ClientStrings.ResourceManager, new object[]
			{
				user,
				email,
				foldertype
			});
		}

		public static LocalizedString UpdateRumStartTimeFlag
		{
			get
			{
				return new LocalizedString("UpdateRumStartTimeFlag", "Ex4EED57", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MyContactsFolderName
		{
			get
			{
				return new LocalizedString("MyContactsFolderName", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenFiveDaysOfWeek(object day1, object day2, object day3, object day4, object day5)
		{
			return new LocalizedString("WhenFiveDaysOfWeek", "ExA08DF3", false, true, ClientStrings.ResourceManager, new object[]
			{
				day1,
				day2,
				day3,
				day4,
				day5
			});
		}

		public static LocalizedString DocumentSyncIssues
		{
			get
			{
				return new LocalizedString("DocumentSyncIssues", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlternateCalendarTaskWhenMonthlyThEveryMonth(LocalizedString calendar, LocalizedString order, IFormattable day)
		{
			return new LocalizedString("AlternateCalendarTaskWhenMonthlyThEveryMonth", "ExD6F1A7", false, true, ClientStrings.ResourceManager, new object[]
			{
				calendar,
				order,
				day
			});
		}

		public static LocalizedString MeetingAccept
		{
			get
			{
				return new LocalizedString("MeetingAccept", "ExDBDCBC", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarWhenWeeklyEveryWeek(IFormattable dayOfWeek)
		{
			return new LocalizedString("CalendarWhenWeeklyEveryWeek", "ExCC097A", false, true, ClientStrings.ResourceManager, new object[]
			{
				dayOfWeek
			});
		}

		public static LocalizedString SearchFolders
		{
			get
			{
				return new LocalizedString("SearchFolders", "ExA655CF", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenRecurringNoEndDateOneDayHoursMinutes(LocalizedString pattern, object startDate, object startTime, int hours, int minutes)
		{
			return new LocalizedString("WhenRecurringNoEndDateOneDayHoursMinutes", "Ex838E4B", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				startTime,
				hours,
				minutes
			});
		}

		public static LocalizedString WhenThreeDaysOfWeek(object day1, object day2, object day3)
		{
			return new LocalizedString("WhenThreeDaysOfWeek", "Ex31FBB5", false, true, ClientStrings.ResourceManager, new object[]
			{
				day1,
				day2,
				day3
			});
		}

		public static LocalizedString OOF
		{
			get
			{
				return new LocalizedString("OOF", "ExE39E3C", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxWelcomeEmailFooterUnsubscribeDescirptionText
		{
			get
			{
				return new LocalizedString("GroupMailboxWelcomeEmailFooterUnsubscribeDescirptionText", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlternateCalendarWhenMonthlyEveryMonth(LocalizedString calendar, int day)
		{
			return new LocalizedString("AlternateCalendarWhenMonthlyEveryMonth", "ExD369C7", false, true, ClientStrings.ResourceManager, new object[]
			{
				calendar,
				day
			});
		}

		public static LocalizedString WhenRecurringWithEndDateDaysNoDuration(LocalizedString pattern, object startDate, object endDate, object startTime, int days)
		{
			return new LocalizedString("WhenRecurringWithEndDateDaysNoDuration", "Ex5AF575", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				endDate,
				startTime,
				days
			});
		}

		public static LocalizedString GroupMailboxWelcomeEmailShareFilesSubTitle
		{
			get
			{
				return new LocalizedString("GroupMailboxWelcomeEmailShareFilesSubTitle", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxWelcomeEmailHeader(string groupName)
		{
			return new LocalizedString("GroupMailboxWelcomeEmailHeader", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				groupName
			});
		}

		public static LocalizedString GroupMailboxWelcomeEmailPrivateTypeText
		{
			get
			{
				return new LocalizedString("GroupMailboxWelcomeEmailPrivateTypeText", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxWelcomeEmailFooterSubscribeLinkText
		{
			get
			{
				return new LocalizedString("GroupMailboxWelcomeEmailFooterSubscribeLinkText", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MdnNotReadNoTime(LocalizedString messageInfo)
		{
			return new LocalizedString("MdnNotReadNoTime", "Ex65D268", false, true, ClientStrings.ResourceManager, new object[]
			{
				messageInfo
			});
		}

		public static LocalizedString SentColon
		{
			get
			{
				return new LocalizedString("SentColon", "Ex6B1F4E", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClutterNotificationO365Closing
		{
			get
			{
				return new LocalizedString("ClutterNotificationO365Closing", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MyCalendars
		{
			get
			{
				return new LocalizedString("MyCalendars", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenRecurringWithEndDateDaysOneHourMinutes(LocalizedString pattern, object startDate, object endDate, object startTime, int days, int minutes)
		{
			return new LocalizedString("WhenRecurringWithEndDateDaysOneHourMinutes", "Ex245174", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				endDate,
				startTime,
				days,
				minutes
			});
		}

		public static LocalizedString WhenRecurringNoEndDateOneDayOneHour(LocalizedString pattern, object startDate, object startTime)
		{
			return new LocalizedString("WhenRecurringNoEndDateOneDayOneHour", "Ex60B9D3", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				startTime
			});
		}

		public static LocalizedString AttachmentNamesKqlKeyword
		{
			get
			{
				return new LocalizedString("AttachmentNamesKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UMFaxFolderName
		{
			get
			{
				return new LocalizedString("UMFaxFolderName", "ExCE057A", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxSuggestToSubscribe
		{
			get
			{
				return new LocalizedString("GroupMailboxSuggestToSubscribe", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxWelcomeMessageSubject(string gmName)
		{
			return new LocalizedString("GroupMailboxWelcomeMessageSubject", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				gmName
			});
		}

		public static LocalizedString GroupMailboxAddedMemberNoJoinedBySubject(string gmName)
		{
			return new LocalizedString("GroupMailboxAddedMemberNoJoinedBySubject", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				gmName
			});
		}

		public static LocalizedString WherePart
		{
			get
			{
				return new LocalizedString("WherePart", "ExE44CAD", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MeetingProposedNewTime
		{
			get
			{
				return new LocalizedString("MeetingProposedNewTime", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Tasks
		{
			get
			{
				return new LocalizedString("Tasks", "Ex876B5E", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClutterNotificationAutoEnablementNoticeWeCallIt
		{
			get
			{
				return new LocalizedString("ClutterNotificationAutoEnablementNoticeWeCallIt", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Configuration
		{
			get
			{
				return new LocalizedString("Configuration", "Ex4DD342", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReceivedKqlKeyword
		{
			get
			{
				return new LocalizedString("ReceivedKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ItemForward
		{
			get
			{
				return new LocalizedString("ItemForward", "Ex2D7BDE", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFolderMailboxHierarchyInfo
		{
			get
			{
				return new LocalizedString("PublicFolderMailboxHierarchyInfo", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxAddedMemberMessageDocument1
		{
			get
			{
				return new LocalizedString("GroupMailboxAddedMemberMessageDocument1", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxAddedSelfMessageHeader
		{
			get
			{
				return new LocalizedString("GroupMailboxAddedSelfMessageHeader", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupSubscriptionPageSubscribedInfo
		{
			get
			{
				return new LocalizedString("GroupSubscriptionPageSubscribedInfo", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TaskWhenMonthlyRegeneratingPattern
		{
			get
			{
				return new LocalizedString("TaskWhenMonthlyRegeneratingPattern", "Ex8B6BCA", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxAddedMemberMessageSubject(string gmName)
		{
			return new LocalizedString("GroupMailboxAddedMemberMessageSubject", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				gmName
			});
		}

		public static LocalizedString AlternateCalendarTaskWhenYearlyLeap(LocalizedString calendar, IFormattable month, int day)
		{
			return new LocalizedString("AlternateCalendarTaskWhenYearlyLeap", "Ex8B6D9B", false, true, ClientStrings.ResourceManager, new object[]
			{
				calendar,
				month,
				day
			});
		}

		public static LocalizedString SentTime
		{
			get
			{
				return new LocalizedString("SentTime", "ExA30CE2", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenRecurringWithEndDateOneDayOneHour(LocalizedString pattern, object startDate, object endDate, object startTime)
		{
			return new LocalizedString("WhenRecurringWithEndDateOneDayOneHour", "ExE88870", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				endDate,
				startTime
			});
		}

		public static LocalizedString SentItems
		{
			get
			{
				return new LocalizedString("SentItems", "Ex195085", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateRumDuplicateFlags
		{
			get
			{
				return new LocalizedString("UpdateRumDuplicateFlags", "Ex66CCCF", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxJoinRequestEmailSubject(string userName, string groupName)
		{
			return new LocalizedString("GroupMailboxJoinRequestEmailSubject", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				userName,
				groupName
			});
		}

		public static LocalizedString ToDoSearch
		{
			get
			{
				return new LocalizedString("ToDoSearch", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxWelcomeEmailO365FooterBrowseViewCalendar
		{
			get
			{
				return new LocalizedString("GroupMailboxWelcomeEmailO365FooterBrowseViewCalendar", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserPhotoNotAnImage
		{
			get
			{
				return new LocalizedString("UserPhotoNotAnImage", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SharingAccept(string user, string email, LocalizedString foldertype)
		{
			return new LocalizedString("SharingAccept", "Ex8FC5D4", false, true, ClientStrings.ResourceManager, new object[]
			{
				user,
				email,
				foldertype
			});
		}

		public static LocalizedString OofReply
		{
			get
			{
				return new LocalizedString("OofReply", "ExACCA0D", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TaskWhenNWeeksRegeneratingPattern(int weeks)
		{
			return new LocalizedString("TaskWhenNWeeksRegeneratingPattern", "Ex06AC26", false, true, ClientStrings.ResourceManager, new object[]
			{
				weeks
			});
		}

		public static LocalizedString AlternateCalendarWhenMonthlyEveryOtherMonth(LocalizedString calendar, int day)
		{
			return new LocalizedString("AlternateCalendarWhenMonthlyEveryOtherMonth", "Ex5A3C51", false, true, ClientStrings.ResourceManager, new object[]
			{
				calendar,
				day
			});
		}

		public static LocalizedString UpdateRumDescription
		{
			get
			{
				return new LocalizedString("UpdateRumDescription", "Ex0462D8", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SharingRequestDenied
		{
			get
			{
				return new LocalizedString("SharingRequestDenied", "Ex78C810", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CommonViews
		{
			get
			{
				return new LocalizedString("CommonViews", "Ex52F7B4", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClutterNotificationOptInLearnMore(string clutterLearnMoreUrl)
		{
			return new LocalizedString("ClutterNotificationOptInLearnMore", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				clutterLearnMoreUrl
			});
		}

		public static LocalizedString TaskWhenYearlyTh(LocalizedString order, IFormattable dayOfWeek, IFormattable month)
		{
			return new LocalizedString("TaskWhenYearlyTh", "Ex8CB8FA", false, true, ClientStrings.ResourceManager, new object[]
			{
				order,
				dayOfWeek,
				month
			});
		}

		public static LocalizedString CalendarWhenWeeklyEveryAlterateWeek(IFormattable dayOfWeek)
		{
			return new LocalizedString("CalendarWhenWeeklyEveryAlterateWeek", "Ex4D8837", false, true, ClientStrings.ResourceManager, new object[]
			{
				dayOfWeek
			});
		}

		public static LocalizedString TaskWhenDailyEveryDay
		{
			get
			{
				return new LocalizedString("TaskWhenDailyEveryDay", "ExA30F3D", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TaskWhenMonthlyEveryMonth(int day)
		{
			return new LocalizedString("TaskWhenMonthlyEveryMonth", "Ex531066", false, true, ClientStrings.ResourceManager, new object[]
			{
				day
			});
		}

		public static LocalizedString NoDataAvailable
		{
			get
			{
				return new LocalizedString("NoDataAvailable", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AppendStrings(LocalizedString str1, LocalizedString str2)
		{
			return new LocalizedString("AppendStrings", "Ex622157", false, true, ClientStrings.ResourceManager, new object[]
			{
				str1,
				str2
			});
		}

		public static LocalizedString ClutterNotificationDisableDeepLink(string disableUrl)
		{
			return new LocalizedString("ClutterNotificationDisableDeepLink", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				disableUrl
			});
		}

		public static LocalizedString Root
		{
			get
			{
				return new LocalizedString("Root", "Ex97F5F2", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarWhenWeeklyEveryNWeeks(int interval, IFormattable dayOfWeek)
		{
			return new LocalizedString("CalendarWhenWeeklyEveryNWeeks", "Ex027619", false, true, ClientStrings.ResourceManager, new object[]
			{
				interval,
				dayOfWeek
			});
		}

		public static LocalizedString KoreanLunar
		{
			get
			{
				return new LocalizedString("KoreanLunar", "Ex44E415", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SyncIssues
		{
			get
			{
				return new LocalizedString("SyncIssues", "Ex7D35C0", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnifiedInbox
		{
			get
			{
				return new LocalizedString("UnifiedInbox", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenRecurringWithEndDateNoTimeInDay(LocalizedString pattern, object startDate, object endDate)
		{
			return new LocalizedString("WhenRecurringWithEndDateNoTimeInDay", "Ex4D97BF", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				endDate
			});
		}

		public static LocalizedString ContactSubjectFormat
		{
			get
			{
				return new LocalizedString("ContactSubjectFormat", "ExF57A6A", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedMaintenanceSynchronizations(string sharePointUrl)
		{
			return new LocalizedString("FailedMaintenanceSynchronizations", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				sharePointUrl
			});
		}

		public static LocalizedString ChineseLunar
		{
			get
			{
				return new LocalizedString("ChineseLunar", "Ex435B21", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Outbox
		{
			get
			{
				return new LocalizedString("Outbox", "Ex36473C", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateRumInconsistencyFlagsLabel
		{
			get
			{
				return new LocalizedString("UpdateRumInconsistencyFlagsLabel", "Ex2283D1", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenRecurringNoEndDateNoTimeInDay(LocalizedString pattern, object startDate)
		{
			return new LocalizedString("WhenRecurringNoEndDateNoTimeInDay", "ExF86A19", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate
			});
		}

		public static LocalizedString WhenRecurringWithEndDateDaysOneHour(LocalizedString pattern, object startDate, object endDate, object startTime, int days)
		{
			return new LocalizedString("WhenRecurringWithEndDateDaysOneHour", "Ex537F1F", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				endDate,
				startTime,
				days
			});
		}

		public static LocalizedString UserPhotoFileTooLarge(int limit)
		{
			return new LocalizedString("UserPhotoFileTooLarge", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				limit
			});
		}

		public static LocalizedString Hijri
		{
			get
			{
				return new LocalizedString("Hijri", "Ex33AAED", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MeetingsKqlKeyword
		{
			get
			{
				return new LocalizedString("MeetingsKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClutterNotificationInvitationHowItWorks
		{
			get
			{
				return new LocalizedString("ClutterNotificationInvitationHowItWorks", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxAddedMemberMessageDocument3
		{
			get
			{
				return new LocalizedString("GroupMailboxAddedMemberMessageDocument3", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateRumRecurrenceFlag
		{
			get
			{
				return new LocalizedString("UpdateRumRecurrenceFlag", "Ex21C763", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PolicyTagKqlKeyword
		{
			get
			{
				return new LocalizedString("PolicyTagKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClutterNotificationPeriodicIntro(string clutterFolderName)
		{
			return new LocalizedString("ClutterNotificationPeriodicIntro", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				clutterFolderName
			});
		}

		public static LocalizedString WhenRecurringWithEndDateDaysHoursMinutes(LocalizedString pattern, object startDate, object endDate, object startTime, int days, int hours, int minutes)
		{
			return new LocalizedString("WhenRecurringWithEndDateDaysHoursMinutes", "ExC2BC40", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				endDate,
				startTime,
				days,
				hours,
				minutes
			});
		}

		public static LocalizedString GroupMailboxWelcomeEmailStartConversationSubtitle(string emailAddress)
		{
			return new LocalizedString("GroupMailboxWelcomeEmailStartConversationSubtitle", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				emailAddress
			});
		}

		public static LocalizedString GroupSubscriptionPageBodyFont
		{
			get
			{
				return new LocalizedString("GroupSubscriptionPageBodyFont", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClutterNotificationOptInSubject
		{
			get
			{
				return new LocalizedString("ClutterNotificationOptInSubject", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClutterNotificationAutoEnablementNoticeItsAutomatic
		{
			get
			{
				return new LocalizedString("ClutterNotificationAutoEnablementNoticeItsAutomatic", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MdnRead(LocalizedString messageInfo, object dateTime, LocalizedString timeZoneName)
		{
			return new LocalizedString("MdnRead", "Ex3D3070", false, true, ClientStrings.ResourceManager, new object[]
			{
				messageInfo,
				dateTime,
				timeZoneName
			});
		}

		public static LocalizedString WorkingElsewhere
		{
			get
			{
				return new LocalizedString("WorkingElsewhere", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NTUser
		{
			get
			{
				return new LocalizedString("NTUser", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarWhenMonthlyEveryMonth(int day)
		{
			return new LocalizedString("CalendarWhenMonthlyEveryMonth", "ExD8A463", false, true, ClientStrings.ResourceManager, new object[]
			{
				day
			});
		}

		public static LocalizedString TaskWhenWeeklyEveryWeek(IFormattable dayOfWeek)
		{
			return new LocalizedString("TaskWhenWeeklyEveryWeek", "ExDFC345", false, true, ClientStrings.ResourceManager, new object[]
			{
				dayOfWeek
			});
		}

		public static LocalizedString WhenRecurringNoEndDateDaysHoursMinutes(LocalizedString pattern, object startDate, object startTime, int days, int hours, int minutes)
		{
			return new LocalizedString("WhenRecurringNoEndDateDaysHoursMinutes", "ExBC53EB", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				startTime,
				days,
				hours,
				minutes
			});
		}

		public static LocalizedString GroupMailboxAddedSelfMessageSubject(string gmName)
		{
			return new LocalizedString("GroupMailboxAddedSelfMessageSubject", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				gmName
			});
		}

		public static LocalizedString ElcRoot
		{
			get
			{
				return new LocalizedString("ElcRoot", "Ex1873C0", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarWhenMonthlyThEveryOtherMonth(LocalizedString order, IFormattable day)
		{
			return new LocalizedString("CalendarWhenMonthlyThEveryOtherMonth", "ExF258AC", false, true, ClientStrings.ResourceManager, new object[]
			{
				order,
				day
			});
		}

		public static LocalizedString ContactFullNameFormat
		{
			get
			{
				return new LocalizedString("ContactFullNameFormat", "ExBD0F67", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlternateCalendarTaskWhenYearly(LocalizedString calendar, IFormattable month, int day)
		{
			return new LocalizedString("AlternateCalendarTaskWhenYearly", "Ex0D998D", false, true, ClientStrings.ResourceManager, new object[]
			{
				calendar,
				month,
				day
			});
		}

		public static LocalizedString GroupMailboxAddedMemberMessageCalendar1
		{
			get
			{
				return new LocalizedString("GroupMailboxAddedMemberMessageCalendar1", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenFifth
		{
			get
			{
				return new LocalizedString("WhenFifth", "Ex476F6D", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClutterNotificationInvitationIntro
		{
			get
			{
				return new LocalizedString("ClutterNotificationInvitationIntro", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExpiresKqlKeyword
		{
			get
			{
				return new LocalizedString("ExpiresKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenRecurringNoEndDateDaysOneHourMinutes(LocalizedString pattern, object startDate, object startTime, int days, int minutes)
		{
			return new LocalizedString("WhenRecurringNoEndDateDaysOneHourMinutes", "Ex558993", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				startTime,
				days,
				minutes
			});
		}

		public static LocalizedString GroupMailboxAddedMemberMessageConversation2
		{
			get
			{
				return new LocalizedString("GroupMailboxAddedMemberMessageConversation2", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CommunicatorHistoryFolderName
		{
			get
			{
				return new LocalizedString("CommunicatorHistoryFolderName", "ExD56E0C", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenRecurringWithEndDateOneDayNoDuration(LocalizedString pattern, object startDate, object endDate, object startTime)
		{
			return new LocalizedString("WhenRecurringWithEndDateOneDayNoDuration", "ExC6260D", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				endDate,
				startTime
			});
		}

		public static LocalizedString LowKqlKeyword
		{
			get
			{
				return new LocalizedString("LowKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AllKqlKeyword
		{
			get
			{
				return new LocalizedString("AllKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxAddedMemberToPublicGroupMessage
		{
			get
			{
				return new LocalizedString("GroupMailboxAddedMemberToPublicGroupMessage", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Followup
		{
			get
			{
				return new LocalizedString("Followup", "Ex32BF1B", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HebrewLunar
		{
			get
			{
				return new LocalizedString("HebrewLunar", "Ex2A9AA3", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Notes
		{
			get
			{
				return new LocalizedString("Notes", "ExE296E8", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupSubscriptionPageSubscribeFailedMaxSubscribers(string groupName)
		{
			return new LocalizedString("GroupSubscriptionPageSubscribeFailedMaxSubscribers", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				groupName
			});
		}

		public static LocalizedString WhenRecurringWithEndDateOneDayHoursMinutes(LocalizedString pattern, object startDate, object endDate, object startTime, int hours, int minutes)
		{
			return new LocalizedString("WhenRecurringWithEndDateOneDayHoursMinutes", "ExA34C75", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				endDate,
				startTime,
				hours,
				minutes
			});
		}

		public static LocalizedString UpdateRumEndTimeFlag
		{
			get
			{
				return new LocalizedString("UpdateRumEndTimeFlag", "Ex035A43", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JournalsKqlKeyword
		{
			get
			{
				return new LocalizedString("JournalsKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SharingICal(string icalurl)
		{
			return new LocalizedString("SharingICal", "ExB5F831", false, true, ClientStrings.ResourceManager, new object[]
			{
				icalurl
			});
		}

		public static LocalizedString AttendeeInquiryRumDescription
		{
			get
			{
				return new LocalizedString("AttendeeInquiryRumDescription", "Ex1DA182", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClutterNotificationOptInHeader
		{
			get
			{
				return new LocalizedString("ClutterNotificationOptInHeader", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LegacyPDLFakeEntry
		{
			get
			{
				return new LocalizedString("LegacyPDLFakeEntry", "ExE78DB7", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PolicyTipDefaultMessageReject
		{
			get
			{
				return new LocalizedString("PolicyTipDefaultMessageReject", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailKqlKeyword
		{
			get
			{
				return new LocalizedString("VoicemailKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PostedOn
		{
			get
			{
				return new LocalizedString("PostedOn", "Ex06826A", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MeetingCancel
		{
			get
			{
				return new LocalizedString("MeetingCancel", "ExEF9EF1", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FromColon
		{
			get
			{
				return new LocalizedString("FromColon", "Ex804C4B", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeletedItems
		{
			get
			{
				return new LocalizedString("DeletedItems", "Ex86E37A", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SharingAnonymous(string user, LocalizedString foldertype, string foldername, string browseurl)
		{
			return new LocalizedString("SharingAnonymous", "ExE57B1A", false, true, ClientStrings.ResourceManager, new object[]
			{
				user,
				foldertype,
				foldername,
				browseurl
			});
		}

		public static LocalizedString AlternateCalendarTaskWhenMonthlyThEveryOtherMonth(LocalizedString calendar, LocalizedString order, IFormattable day)
		{
			return new LocalizedString("AlternateCalendarTaskWhenMonthlyThEveryOtherMonth", "ExA2E77C", false, true, ClientStrings.ResourceManager, new object[]
			{
				calendar,
				order,
				day
			});
		}

		public static LocalizedString DocsKqlKeyword
		{
			get
			{
				return new LocalizedString("DocsKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenRecurringNoEndDateNoDuration(LocalizedString pattern, object startDate, object startTime)
		{
			return new LocalizedString("WhenRecurringNoEndDateNoDuration", "ExF8844E", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				startTime
			});
		}

		public static LocalizedString MdnNotRead(LocalizedString messageInfo, object dateTime, LocalizedString timeZoneName)
		{
			return new LocalizedString("MdnNotRead", "Ex6E8C2A", false, true, ClientStrings.ResourceManager, new object[]
			{
				messageInfo,
				dateTime,
				timeZoneName
			});
		}

		public static LocalizedString ClutterNotificationO365DisplayName
		{
			get
			{
				return new LocalizedString("ClutterNotificationO365DisplayName", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LocalFailures
		{
			get
			{
				return new LocalizedString("LocalFailures", "Ex54C8EC", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxWelcomeEmailO365FooterBrowseConversations
		{
			get
			{
				return new LocalizedString("GroupMailboxWelcomeEmailO365FooterBrowseConversations", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JapaneseLunar
		{
			get
			{
				return new LocalizedString("JapaneseLunar", "Ex682E5C", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxWelcomeEmailSecondaryHeaderYouJoined(string groupName)
		{
			return new LocalizedString("GroupMailboxWelcomeEmailSecondaryHeaderYouJoined", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				groupName
			});
		}

		public static LocalizedString ClutterNotificationPeriodicSurveySteps
		{
			get
			{
				return new LocalizedString("ClutterNotificationPeriodicSurveySteps", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TaskStatusInProgress
		{
			get
			{
				return new LocalizedString("TaskStatusInProgress", "Ex1C5C27", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlternateCalendarWhenMonthlyThEveryMonth(LocalizedString calendar, LocalizedString order, IFormattable day)
		{
			return new LocalizedString("AlternateCalendarWhenMonthlyThEveryMonth", "Ex1B4E51", false, true, ClientStrings.ResourceManager, new object[]
			{
				calendar,
				order,
				day
			});
		}

		public static LocalizedString GroupSubscriptionUnsubscribeInfoHtml(string groupName, string link)
		{
			return new LocalizedString("GroupSubscriptionUnsubscribeInfoHtml", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				groupName,
				link
			});
		}

		public static LocalizedString AutomaticDisplayName
		{
			get
			{
				return new LocalizedString("AutomaticDisplayName", "Ex084271", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SharingInvitationInstruction
		{
			get
			{
				return new LocalizedString("SharingInvitationInstruction", "Ex04F28A", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupSubscriptionPageSubscribeFailedNotAMember(string groupName)
		{
			return new LocalizedString("GroupSubscriptionPageSubscribeFailedNotAMember", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				groupName
			});
		}

		public static LocalizedString WhenRecurringNoEndDateDaysOneHour(LocalizedString pattern, object startDate, object startTime, int days)
		{
			return new LocalizedString("WhenRecurringNoEndDateDaysOneHour", "ExE54279", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				startTime,
				days
			});
		}

		public static LocalizedString BirthdayCalendarFolderName
		{
			get
			{
				return new LocalizedString("BirthdayCalendarFolderName", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NormalKqlKeyword
		{
			get
			{
				return new LocalizedString("NormalKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClutterNotificationPeriodicCheckBack
		{
			get
			{
				return new LocalizedString("ClutterNotificationPeriodicCheckBack", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenSixDaysOfWeek(object day1, object day2, object day3, object day4, object day5, object day6)
		{
			return new LocalizedString("WhenSixDaysOfWeek", "Ex5EC036", false, true, ClientStrings.ResourceManager, new object[]
			{
				day1,
				day2,
				day3,
				day4,
				day5,
				day6
			});
		}

		public static LocalizedString AlternateCalendarWhenYearly(LocalizedString calendar, IFormattable month, int day)
		{
			return new LocalizedString("AlternateCalendarWhenYearly", "ExFDD6D7", false, true, ClientStrings.ResourceManager, new object[]
			{
				calendar,
				month,
				day
			});
		}

		public static LocalizedString WhenTwoDaysOfWeek(object day1, object day2)
		{
			return new LocalizedString("WhenTwoDaysOfWeek", "Ex8009C4", false, true, ClientStrings.ResourceManager, new object[]
			{
				day1,
				day2
			});
		}

		public static LocalizedString ToKqlKeyword
		{
			get
			{
				return new LocalizedString("ToKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SharingRequest(string user, string email, LocalizedString foldertype)
		{
			return new LocalizedString("SharingRequest", "Ex0A9AF7", false, true, ClientStrings.ResourceManager, new object[]
			{
				user,
				email,
				foldertype
			});
		}

		public static LocalizedString JointStrings(LocalizedString str1, LocalizedString str2)
		{
			return new LocalizedString("JointStrings", "Ex4C0B94", false, true, ClientStrings.ResourceManager, new object[]
			{
				str1,
				str2
			});
		}

		public static LocalizedString Drafts
		{
			get
			{
				return new LocalizedString("Drafts", "Ex573013", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserPhotoImageTooSmall(int min)
		{
			return new LocalizedString("UserPhotoImageTooSmall", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				min
			});
		}

		public static LocalizedString MeetingDecline
		{
			get
			{
				return new LocalizedString("MeetingDecline", "Ex8FCFE3", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenSecond
		{
			get
			{
				return new LocalizedString("WhenSecond", "Ex7FD30B", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubjectKqlKeyword
		{
			get
			{
				return new LocalizedString("SubjectKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupSubscriptionPageUnsubscribed(string groupName)
		{
			return new LocalizedString("GroupSubscriptionPageUnsubscribed", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				groupName
			});
		}

		public static LocalizedString ServerFailures
		{
			get
			{
				return new LocalizedString("ServerFailures", "ExFB6DB7", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClutterNotificationInvitationHeader
		{
			get
			{
				return new LocalizedString("ClutterNotificationInvitationHeader", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlternateCalendarTaskWhenMonthlyThEveryNMonths(LocalizedString calendar, LocalizedString order, IFormattable day, int interval)
		{
			return new LocalizedString("AlternateCalendarTaskWhenMonthlyThEveryNMonths", "Ex9DEDEE", false, true, ClientStrings.ResourceManager, new object[]
			{
				calendar,
				order,
				day,
				interval
			});
		}

		public static LocalizedString ClutterNotificationOptInFeedback(string feedbackMailtoUrl)
		{
			return new LocalizedString("ClutterNotificationOptInFeedback", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				feedbackMailtoUrl
			});
		}

		public static LocalizedString UMVoiceMailFolderName
		{
			get
			{
				return new LocalizedString("UMVoiceMailFolderName", "ExA1DE2B", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarWhenDailyEveryDay
		{
			get
			{
				return new LocalizedString("CalendarWhenDailyEveryDay", "Ex862A92", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenThird
		{
			get
			{
				return new LocalizedString("WhenThird", "Ex3BC251", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClutterNotificationOptInPrivacy
		{
			get
			{
				return new LocalizedString("ClutterNotificationOptInPrivacy", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenFourDaysOfWeek(object day1, object day2, object day3, object day4)
		{
			return new LocalizedString("WhenFourDaysOfWeek", "Ex69B20B", false, true, ClientStrings.ResourceManager, new object[]
			{
				day1,
				day2,
				day3,
				day4
			});
		}

		public static LocalizedString SharingInvitationUpdatedSubjectLine
		{
			get
			{
				return new LocalizedString("SharingInvitationUpdatedSubjectLine", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarWhenMonthlyThEveryNMonths(LocalizedString order, IFormattable day, int interval)
		{
			return new LocalizedString("CalendarWhenMonthlyThEveryNMonths", "ExCD657F", false, true, ClientStrings.ResourceManager, new object[]
			{
				order,
				day,
				interval
			});
		}

		public static LocalizedString OtherCalendars
		{
			get
			{
				return new LocalizedString("OtherCalendars", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClutterNotificationOptInIntro(string clutterFolderName)
		{
			return new LocalizedString("ClutterNotificationOptInIntro", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				clutterFolderName
			});
		}

		public static LocalizedString SharingInvitation(string user, string email, LocalizedString foldertype)
		{
			return new LocalizedString("SharingInvitation", "ExAF24EA", false, true, ClientStrings.ResourceManager, new object[]
			{
				user,
				email,
				foldertype
			});
		}

		public static LocalizedString GroupMailboxWelcomeEmailSubscribeToInboxSubtitle
		{
			get
			{
				return new LocalizedString("GroupMailboxWelcomeEmailSubscribeToInboxSubtitle", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Tentative
		{
			get
			{
				return new LocalizedString("Tentative", "Ex49F338", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarWhenYearlyTh(LocalizedString order, IFormattable dayOfWeek, IFormattable month)
		{
			return new LocalizedString("CalendarWhenYearlyTh", "Ex5A2275", false, true, ClientStrings.ResourceManager, new object[]
			{
				order,
				dayOfWeek,
				month
			});
		}

		public static LocalizedString FailedToSynchronizeMembershipFromSharePointText(string sharePointUrl, string error)
		{
			return new LocalizedString("FailedToSynchronizeMembershipFromSharePointText", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				sharePointUrl,
				error
			});
		}

		public static LocalizedString AttachmentKqlKeyword
		{
			get
			{
				return new LocalizedString("AttachmentKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenOnWeekDays
		{
			get
			{
				return new LocalizedString("WhenOnWeekDays", "Ex298111", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TaskWhenMonthlyEveryOtherMonth(int day)
		{
			return new LocalizedString("TaskWhenMonthlyEveryOtherMonth", "Ex34620A", false, true, ClientStrings.ResourceManager, new object[]
			{
				day
			});
		}

		public static LocalizedString SharingInvitationNonPrimary(string user, string email, string foldername, LocalizedString foldertype)
		{
			return new LocalizedString("SharingInvitationNonPrimary", "Ex74C6C0", false, true, ClientStrings.ResourceManager, new object[]
			{
				user,
				email,
				foldername,
				foldertype
			});
		}

		public static LocalizedString WhenSevenDaysOfWeek(object day1, object day2, object day3, object day4, object day5, object day6, object day7)
		{
			return new LocalizedString("WhenSevenDaysOfWeek", "ExFFC7B4", false, true, ClientStrings.ResourceManager, new object[]
			{
				day1,
				day2,
				day3,
				day4,
				day5,
				day6,
				day7
			});
		}

		public static LocalizedString GroupMailboxAddedMemberMessageHeader(string userName)
		{
			return new LocalizedString("GroupMailboxAddedMemberMessageHeader", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString ClutterNotificationHeaderFont
		{
			get
			{
				return new LocalizedString("ClutterNotificationHeaderFont", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TaskWhenWeeklyRegeneratingPattern
		{
			get
			{
				return new LocalizedString("TaskWhenWeeklyRegeneratingPattern", "Ex279C6D", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FaxesKqlKeyword
		{
			get
			{
				return new LocalizedString("FaxesKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TaskStatusNotStarted
		{
			get
			{
				return new LocalizedString("TaskStatusNotStarted", "Ex6F7061", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ToColon
		{
			get
			{
				return new LocalizedString("ToColon", "Ex2F3835", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IsReadKqlKeyword
		{
			get
			{
				return new LocalizedString("IsReadKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IsFlaggedKqlKeyword
		{
			get
			{
				return new LocalizedString("IsFlaggedKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CancellationRumTitle
		{
			get
			{
				return new LocalizedString("CancellationRumTitle", "Ex5D18C8", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClutterNotificationPeriodicHeader
		{
			get
			{
				return new LocalizedString("ClutterNotificationPeriodicHeader", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenOneDayOfWeek(object day1)
		{
			return new LocalizedString("WhenOneDayOfWeek", "ExFBF8CE", false, true, ClientStrings.ResourceManager, new object[]
			{
				day1
			});
		}

		public static LocalizedString GroupSubscriptionPageRequestFailedInfo
		{
			get
			{
				return new LocalizedString("GroupSubscriptionPageRequestFailedInfo", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClutterNotificationInvitationSubject
		{
			get
			{
				return new LocalizedString("ClutterNotificationInvitationSubject", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FromKqlKeyword
		{
			get
			{
				return new LocalizedString("FromKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupSubscriptionPageUnsubscribedInfo
		{
			get
			{
				return new LocalizedString("GroupSubscriptionPageUnsubscribedInfo", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxAddedMemberToPrivateGroupMessage
		{
			get
			{
				return new LocalizedString("GroupMailboxAddedMemberToPrivateGroupMessage", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlternateCalendarTaskWhenMonthlyEveryOtherMonth(LocalizedString calendar, int day)
		{
			return new LocalizedString("AlternateCalendarTaskWhenMonthlyEveryOtherMonth", "Ex1A371B", false, true, ClientStrings.ResourceManager, new object[]
			{
				calendar,
				day
			});
		}

		public static LocalizedString WhenOneMoreOccurrence
		{
			get
			{
				return new LocalizedString("WhenOneMoreOccurrence", "Ex130EC1", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupSubscriptionUnsubscribeLinkWord
		{
			get
			{
				return new LocalizedString("GroupSubscriptionUnsubscribeLinkWord", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFolderMailboxDumpsterInfo
		{
			get
			{
				return new LocalizedString("PublicFolderMailboxDumpsterInfo", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarWhenDailyEveryNDays(int interval)
		{
			return new LocalizedString("CalendarWhenDailyEveryNDays", "ExCAD81F", false, true, ClientStrings.ResourceManager, new object[]
			{
				interval
			});
		}

		public static LocalizedString TaskStatusWaitOnOthers
		{
			get
			{
				return new LocalizedString("TaskStatusWaitOnOthers", "Ex66CFEB", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxWelcomeEmailO365FooterBrowseShareFiles
		{
			get
			{
				return new LocalizedString("GroupMailboxWelcomeEmailO365FooterBrowseShareFiles", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarWhenMonthlyEveryOtherMonth(int day)
		{
			return new LocalizedString("CalendarWhenMonthlyEveryOtherMonth", "Ex7FC862", false, true, ClientStrings.ResourceManager, new object[]
			{
				day
			});
		}

		public static LocalizedString ClutterNotificationPeriodicSubject
		{
			get
			{
				return new LocalizedString("ClutterNotificationPeriodicSubject", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxAddedMemberNoJoinedByMessageHeader
		{
			get
			{
				return new LocalizedString("GroupMailboxAddedMemberNoJoinedByMessageHeader", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToSynchronizeHierarchyChangesFromSharePointText(string siteUrl, string error)
		{
			return new LocalizedString("FailedToSynchronizeHierarchyChangesFromSharePointText", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				siteUrl,
				error
			});
		}

		public static LocalizedString RequestedActionNoResponseNecessary
		{
			get
			{
				return new LocalizedString("RequestedActionNoResponseNecessary", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RpcClientRequestError(string error)
		{
			return new LocalizedString("RpcClientRequestError", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString WhenRecurringNoEndDateOneDayHours(LocalizedString pattern, object startDate, object startTime, int hours)
		{
			return new LocalizedString("WhenRecurringNoEndDateOneDayHours", "Ex924D65", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				startTime,
				hours
			});
		}

		public static LocalizedString WhenRecurringWithEndDateDaysMinutes(LocalizedString pattern, object startDate, object endDate, object startTime, int days, int minutes)
		{
			return new LocalizedString("WhenRecurringWithEndDateDaysMinutes", "Ex0A0761", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				endDate,
				startTime,
				days,
				minutes
			});
		}

		public static LocalizedString SharingDecline(string user, string email, LocalizedString foldertype)
		{
			return new LocalizedString("SharingDecline", "Ex3767B2", false, true, ClientStrings.ResourceManager, new object[]
			{
				user,
				email,
				foldertype
			});
		}

		public static LocalizedString TaskWhenYearlyRegeneratingPattern
		{
			get
			{
				return new LocalizedString("TaskWhenYearlyRegeneratingPattern", "ExECFD1E", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Free
		{
			get
			{
				return new LocalizedString("Free", "ExA3A029", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CancellationRumDescription
		{
			get
			{
				return new LocalizedString("CancellationRumDescription", "Ex62915A", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequestedActionReplyToAll
		{
			get
			{
				return new LocalizedString("RequestedActionReplyToAll", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxWelcomeEmailO365FooterTitle
		{
			get
			{
				return new LocalizedString("GroupMailboxWelcomeEmailO365FooterTitle", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClutterNotificationTakeSurveyDeepLink(string surveyUrl)
		{
			return new LocalizedString("ClutterNotificationTakeSurveyDeepLink", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				surveyUrl
			});
		}

		public static LocalizedString ItemReply
		{
			get
			{
				return new LocalizedString("ItemReply", "ExB56A1F", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenFirst
		{
			get
			{
				return new LocalizedString("WhenFirst", "Ex265D19", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlternateCalendarTaskWhenMonthlyEveryMonth(LocalizedString calendar, int day)
		{
			return new LocalizedString("AlternateCalendarTaskWhenMonthlyEveryMonth", "Ex8E0C57", false, true, ClientStrings.ResourceManager, new object[]
			{
				calendar,
				day
			});
		}

		public static LocalizedString TaskWhenDailyEveryNDays(int interval)
		{
			return new LocalizedString("TaskWhenDailyEveryNDays", "Ex1D8736", false, true, ClientStrings.ResourceManager, new object[]
			{
				interval
			});
		}

		public static LocalizedString RpcClientInitError
		{
			get
			{
				return new LocalizedString("RpcClientInitError", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequestedActionDoNotForward
		{
			get
			{
				return new LocalizedString("RequestedActionDoNotForward", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SizeKqlKeyword
		{
			get
			{
				return new LocalizedString("SizeKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupSubscriptionPagePrivateGroupInfo
		{
			get
			{
				return new LocalizedString("GroupSubscriptionPagePrivateGroupInfo", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserPhotoNotFound
		{
			get
			{
				return new LocalizedString("UserPhotoNotFound", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FromFavoriteSendersFolderName
		{
			get
			{
				return new LocalizedString("FromFavoriteSendersFolderName", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxAddedMemberMessageFont
		{
			get
			{
				return new LocalizedString("GroupMailboxAddedMemberMessageFont", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequestedActionForward
		{
			get
			{
				return new LocalizedString("RequestedActionForward", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CcKqlKeyword
		{
			get
			{
				return new LocalizedString("CcKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TaskWhenNMonthsRegeneratingPattern(int months)
		{
			return new LocalizedString("TaskWhenNMonthsRegeneratingPattern", "ExD7271C", false, true, ClientStrings.ResourceManager, new object[]
			{
				months
			});
		}

		public static LocalizedString Conversations
		{
			get
			{
				return new LocalizedString("Conversations", "Ex43F63E", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupSubscriptionPagePublicGroupInfo
		{
			get
			{
				return new LocalizedString("GroupSubscriptionPagePublicGroupInfo", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContactsKqlKeyword
		{
			get
			{
				return new LocalizedString("ContactsKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenBothWeekendDays
		{
			get
			{
				return new LocalizedString("WhenBothWeekendDays", "ExE8A92D", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Conflicts
		{
			get
			{
				return new LocalizedString("Conflicts", "Ex623D81", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PostsKqlKeyword
		{
			get
			{
				return new LocalizedString("PostsKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxAddedMemberMessageTitle(string groupName)
		{
			return new LocalizedString("GroupMailboxAddedMemberMessageTitle", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				groupName
			});
		}

		public static LocalizedString UserPhotoPreviewNotFound
		{
			get
			{
				return new LocalizedString("UserPhotoPreviewNotFound", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenEveryDay
		{
			get
			{
				return new LocalizedString("WhenEveryDay", "Ex239A05", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenNMoreOccurrences(int numberOccurrence)
		{
			return new LocalizedString("WhenNMoreOccurrences", "Ex25956F", false, true, ClientStrings.ResourceManager, new object[]
			{
				numberOccurrence
			});
		}

		public static LocalizedString SharingRequestInstruction
		{
			get
			{
				return new LocalizedString("SharingRequestInstruction", "Ex77A584", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxJoinRequestEmailBody(string attachedMessage)
		{
			return new LocalizedString("GroupMailboxJoinRequestEmailBody", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				attachedMessage
			});
		}

		public static LocalizedString ClutterNotificationAutoEnablementNoticeIntro
		{
			get
			{
				return new LocalizedString("ClutterNotificationAutoEnablementNoticeIntro", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlternateCalendarTaskWhenYearlyThLeap(LocalizedString calendar, LocalizedString order, IFormattable dayOfWeek, IFormattable month)
		{
			return new LocalizedString("AlternateCalendarTaskWhenYearlyThLeap", "ExF91A57", false, true, ClientStrings.ResourceManager, new object[]
			{
				calendar,
				order,
				dayOfWeek,
				month
			});
		}

		public static LocalizedString ClutterNotificationBodyFont
		{
			get
			{
				return new LocalizedString("ClutterNotificationBodyFont", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClutterNotificationInvitationLearnMore(string clutterLearnMoreUrl)
		{
			return new LocalizedString("ClutterNotificationInvitationLearnMore", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				clutterLearnMoreUrl
			});
		}

		public static LocalizedString CalendarWhenMonthlyThEveryMonth(LocalizedString order, IFormattable day)
		{
			return new LocalizedString("CalendarWhenMonthlyThEveryMonth", "Ex1CF625", false, true, ClientStrings.ResourceManager, new object[]
			{
				order,
				day
			});
		}

		public static LocalizedString ClutterNotificationInvitationO365Helps
		{
			get
			{
				return new LocalizedString("ClutterNotificationInvitationO365Helps", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenFourth
		{
			get
			{
				return new LocalizedString("WhenFourth", "Ex641093", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlternateCalendarTaskWhenYearlyTh(LocalizedString calendar, LocalizedString order, IFormattable dayOfWeek, IFormattable month)
		{
			return new LocalizedString("AlternateCalendarTaskWhenYearlyTh", "Ex794037", false, true, ClientStrings.ResourceManager, new object[]
			{
				calendar,
				order,
				dayOfWeek,
				month
			});
		}

		public static LocalizedString TaskWhenMonthlyThEveryNMonths(LocalizedString order, IFormattable day, int interval)
		{
			return new LocalizedString("TaskWhenMonthlyThEveryNMonths", "Ex68FD6B", false, true, ClientStrings.ResourceManager, new object[]
			{
				order,
				day,
				interval
			});
		}

		public static LocalizedString WhenRecurringWithEndDateOneDayMinutes(LocalizedString pattern, object startDate, object endDate, object startTime, int minutes)
		{
			return new LocalizedString("WhenRecurringWithEndDateOneDayMinutes", "Ex649875", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				endDate,
				startTime,
				minutes
			});
		}

		public static LocalizedString RequestedActionFollowUp
		{
			get
			{
				return new LocalizedString("RequestedActionFollowUp", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupSubscriptionPageRequestFailed
		{
			get
			{
				return new LocalizedString("GroupSubscriptionPageRequestFailed", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenRecurringWithEndDateOneDayOneHourMinutes(LocalizedString pattern, object startDate, object endDate, object startTime, int minutes)
		{
			return new LocalizedString("WhenRecurringWithEndDateOneDayOneHourMinutes", "Ex20EE81", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				endDate,
				startTime,
				minutes
			});
		}

		public static LocalizedString ClutterNotificationAutoEnablementNoticeSubject
		{
			get
			{
				return new LocalizedString("ClutterNotificationAutoEnablementNoticeSubject", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxWelcomeMessageHeader1(string groupName)
		{
			return new LocalizedString("GroupMailboxWelcomeMessageHeader1", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				groupName
			});
		}

		public static LocalizedString WhenRecurringNoEndDateDaysMinutes(LocalizedString pattern, object startDate, object startTime, int days, int minutes)
		{
			return new LocalizedString("WhenRecurringNoEndDateDaysMinutes", "ExCF9103", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				startTime,
				days,
				minutes
			});
		}

		public static LocalizedString PeoplesCalendars
		{
			get
			{
				return new LocalizedString("PeoplesCalendars", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EHAMigration
		{
			get
			{
				return new LocalizedString("EHAMigration", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlternateCalendarWhenYearlyTh(LocalizedString calendar, LocalizedString order, IFormattable dayOfWeek, IFormattable month)
		{
			return new LocalizedString("AlternateCalendarWhenYearlyTh", "Ex5C8400", false, true, ClientStrings.ResourceManager, new object[]
			{
				calendar,
				order,
				dayOfWeek,
				month
			});
		}

		public static LocalizedString SubjectColon
		{
			get
			{
				return new LocalizedString("SubjectColon", "Ex6FE519", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarWhenMonthlyEveryNMonths(int day, int interval)
		{
			return new LocalizedString("CalendarWhenMonthlyEveryNMonths", "Ex3B3883", false, true, ClientStrings.ResourceManager, new object[]
			{
				day,
				interval
			});
		}

		public static LocalizedString Calendar
		{
			get
			{
				return new LocalizedString("Calendar", "Ex7A113F", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequestedActionAny
		{
			get
			{
				return new LocalizedString("RequestedActionAny", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenEveryWeekDay
		{
			get
			{
				return new LocalizedString("WhenEveryWeekDay", "ExC63BFC", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TaskWhenYearly(IFormattable month, int day)
		{
			return new LocalizedString("TaskWhenYearly", "Ex421B2D", false, true, ClientStrings.ResourceManager, new object[]
			{
				month,
				day
			});
		}

		public static LocalizedString TaskWhenMonthlyEveryNMonths(int day, int interval)
		{
			return new LocalizedString("TaskWhenMonthlyEveryNMonths", "ExD14484", false, true, ClientStrings.ResourceManager, new object[]
			{
				day,
				interval
			});
		}

		public static LocalizedString TasksKqlKeyword
		{
			get
			{
				return new LocalizedString("TasksKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenStartsEndsSameDay(object startDate, object startTime, object endTime)
		{
			return new LocalizedString("WhenStartsEndsSameDay", "Ex85CF71", false, true, ClientStrings.ResourceManager, new object[]
			{
				startDate,
				startTime,
				endTime
			});
		}

		public static LocalizedString FailedToSynchronizeMembershipFromSharePoint(string sharePointUrl)
		{
			return new LocalizedString("FailedToSynchronizeMembershipFromSharePoint", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				sharePointUrl
			});
		}

		public static LocalizedString GroupSubscriptionPageSubscribed(string groupName)
		{
			return new LocalizedString("GroupSubscriptionPageSubscribed", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				groupName
			});
		}

		public static LocalizedString NotesKqlKeyword
		{
			get
			{
				return new LocalizedString("NotesKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TaskStatusCompleted
		{
			get
			{
				return new LocalizedString("TaskStatusCompleted", "ExD86FAF", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RumFooter(string version)
		{
			return new LocalizedString("RumFooter", "Ex6501CD", false, true, ClientStrings.ResourceManager, new object[]
			{
				version
			});
		}

		public static LocalizedString FailedToDeleteFromSharePointErrorText(string fileName, string sharePointFolderUrl)
		{
			return new LocalizedString("FailedToDeleteFromSharePointErrorText", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				fileName,
				sharePointFolderUrl
			});
		}

		public static LocalizedString ClutterNotificationInvitationItsAutomatic
		{
			get
			{
				return new LocalizedString("ClutterNotificationInvitationItsAutomatic", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RssSubscriptions
		{
			get
			{
				return new LocalizedString("RssSubscriptions", "ExD1870D", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequestedActionCall
		{
			get
			{
				return new LocalizedString("RequestedActionCall", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TaskWhenNDaysRegeneratingPattern(int days)
		{
			return new LocalizedString("TaskWhenNDaysRegeneratingPattern", "Ex3CC5D3", false, true, ClientStrings.ResourceManager, new object[]
			{
				days
			});
		}

		public static LocalizedString PolicyTipDefaultMessageNotifyOnly
		{
			get
			{
				return new LocalizedString("PolicyTipDefaultMessageNotifyOnly", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToUploadToSharePointTitle
		{
			get
			{
				return new LocalizedString("FailedToUploadToSharePointTitle", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TaskWhenWeeklyEveryAlterateWeek(IFormattable dayOfWeek)
		{
			return new LocalizedString("TaskWhenWeeklyEveryAlterateWeek", "ExFD87C4", false, true, ClientStrings.ResourceManager, new object[]
			{
				dayOfWeek
			});
		}

		public static LocalizedString PublicFolderMailboxInfoFolderEnumeration(int current, int total)
		{
			return new LocalizedString("PublicFolderMailboxInfoFolderEnumeration", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				current,
				total
			});
		}

		public static LocalizedString ClutterNotificationPeriodicLearnMore(string clutterLearnMoreUrl)
		{
			return new LocalizedString("ClutterNotificationPeriodicLearnMore", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				clutterLearnMoreUrl
			});
		}

		public static LocalizedString TaskStatusDeferred
		{
			get
			{
				return new LocalizedString("TaskStatusDeferred", "Ex7BEE4A", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingItemRumDescription(string version)
		{
			return new LocalizedString("MissingItemRumDescription", "Ex5C85B5", false, true, ClientStrings.ResourceManager, new object[]
			{
				version
			});
		}

		public static LocalizedString SharingRequestAllowed
		{
			get
			{
				return new LocalizedString("SharingRequestAllowed", "Ex78AF25", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenRecurringNoEndDate(LocalizedString pattern, object startDate, object startTime, object endTime)
		{
			return new LocalizedString("WhenRecurringNoEndDate", "Ex89790F", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				startTime,
				endTime
			});
		}

		public static LocalizedString ClutterNotificationEnableDeepLink(string enableUrl)
		{
			return new LocalizedString("ClutterNotificationEnableDeepLink", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				enableUrl
			});
		}

		public static LocalizedString UpdateRumMissingItemFlag
		{
			get
			{
				return new LocalizedString("UpdateRumMissingItemFlag", "Ex3B3373", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ParticipantsKqlKeyword
		{
			get
			{
				return new LocalizedString("ParticipantsKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlternateCalendarWhenMonthlyThEveryNMonths(LocalizedString calendar, LocalizedString order, IFormattable day, int interval)
		{
			return new LocalizedString("AlternateCalendarWhenMonthlyThEveryNMonths", "ExCBAD9A", false, true, ClientStrings.ResourceManager, new object[]
			{
				calendar,
				order,
				day,
				interval
			});
		}

		public static LocalizedString GroupMailboxWelcomeEmailStartConversationTitle
		{
			get
			{
				return new LocalizedString("GroupMailboxWelcomeEmailStartConversationTitle", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxWelcomeEmailShareFilesTitle
		{
			get
			{
				return new LocalizedString("GroupMailboxWelcomeEmailShareFilesTitle", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LegacyArchiveJournals
		{
			get
			{
				return new LocalizedString("LegacyArchiveJournals", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TaskWhenMonthlyThEveryMonth(LocalizedString order, IFormattable day)
		{
			return new LocalizedString("TaskWhenMonthlyThEveryMonth", "Ex2BEA98", false, true, ClientStrings.ResourceManager, new object[]
			{
				order,
				day
			});
		}

		public static LocalizedString RecipientsKqlKeyword
		{
			get
			{
				return new LocalizedString("RecipientsKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenRecurringNoEndDateOneDayMinutes(LocalizedString pattern, object startDate, object startTime, int minutes)
		{
			return new LocalizedString("WhenRecurringNoEndDateOneDayMinutes", "Ex9AB11F", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				startTime,
				minutes
			});
		}

		public static LocalizedString RssFeedsKqlKeyword
		{
			get
			{
				return new LocalizedString("RssFeedsKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxWelcomeEmailSubscribeToInboxTitle
		{
			get
			{
				return new LocalizedString("GroupMailboxWelcomeEmailSubscribeToInboxTitle", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxAddedMemberMessageCalendar3
		{
			get
			{
				return new LocalizedString("GroupMailboxAddedMemberMessageCalendar3", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequestedActionReview
		{
			get
			{
				return new LocalizedString("RequestedActionReview", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KindKqlKeyword
		{
			get
			{
				return new LocalizedString("KindKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmailKqlKeyword
		{
			get
			{
				return new LocalizedString("EmailKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UTC
		{
			get
			{
				return new LocalizedString("UTC", "ExBFFB60", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenRecurringNoEndDateOneDayNoDuration(LocalizedString pattern, object startDate, object startTime)
		{
			return new LocalizedString("WhenRecurringNoEndDateOneDayNoDuration", "ExE6F8BA", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				startTime
			});
		}

		public static LocalizedString WhenRecurringNoEndDateDaysNoDuration(LocalizedString pattern, object startDate, object startTime, int days)
		{
			return new LocalizedString("WhenRecurringNoEndDateDaysNoDuration", "Ex3BA4A0", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				startTime,
				days
			});
		}

		public static LocalizedString BccKqlKeyword
		{
			get
			{
				return new LocalizedString("BccKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenRecurringWithEndDateDaysHours(LocalizedString pattern, object startDate, object endDate, object startTime, int days, int hours)
		{
			return new LocalizedString("WhenRecurringWithEndDateDaysHours", "Ex87F34E", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				endDate,
				startTime,
				days,
				hours
			});
		}

		public static LocalizedString ErrorSharePointSiteHasNoValidUrl(string url)
		{
			return new LocalizedString("ErrorSharePointSiteHasNoValidUrl", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString RequestedActionForYourInformation
		{
			get
			{
				return new LocalizedString("RequestedActionForYourInformation", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Journal
		{
			get
			{
				return new LocalizedString("Journal", "Ex72205E", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxAddedMemberMessageConversation1
		{
			get
			{
				return new LocalizedString("GroupMailboxAddedMemberMessageConversation1", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxAddedMemberMessageConversation3
		{
			get
			{
				return new LocalizedString("GroupMailboxAddedMemberMessageConversation3", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TaskWhenNYearsRegeneratingPattern(int years)
		{
			return new LocalizedString("TaskWhenNYearsRegeneratingPattern", "ExF1A6C8", false, true, ClientStrings.ResourceManager, new object[]
			{
				years
			});
		}

		public static LocalizedString UserPhotoFileTooSmall(int min)
		{
			return new LocalizedString("UserPhotoFileTooSmall", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				min
			});
		}

		public static LocalizedString HasAttachmentKqlKeyword
		{
			get
			{
				return new LocalizedString("HasAttachmentKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupSubscriptionPageGroupInfoFont
		{
			get
			{
				return new LocalizedString("GroupSubscriptionPageGroupInfoFont", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TaskWhenMonthlyThEveryOtherMonth(LocalizedString order, IFormattable day)
		{
			return new LocalizedString("TaskWhenMonthlyThEveryOtherMonth", "Ex4B4F6E", false, true, ClientStrings.ResourceManager, new object[]
			{
				order,
				day
			});
		}

		public static LocalizedString Contacts
		{
			get
			{
				return new LocalizedString("Contacts", "ExD0E0B2", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupSubscriptionUnsubscribeInfoText(string groupName, string link)
		{
			return new LocalizedString("GroupSubscriptionUnsubscribeInfoText", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				groupName,
				link
			});
		}

		public static LocalizedString WhenRecurringWithEndDate(LocalizedString pattern, object startDate, object endDate, object startTime, object endTime)
		{
			return new LocalizedString("WhenRecurringWithEndDate", "Ex99B4D6", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				endDate,
				startTime,
				endTime
			});
		}

		public static LocalizedString WhenRecurringNoEndDateOneDayOneHourMinutes(LocalizedString pattern, object startDate, object startTime, int minutes)
		{
			return new LocalizedString("WhenRecurringNoEndDateOneDayOneHourMinutes", "Ex70B707", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				startTime,
				minutes
			});
		}

		public static LocalizedString GroupMailboxAddedMemberMessageCalendar2
		{
			get
			{
				return new LocalizedString("GroupMailboxAddedMemberMessageCalendar2", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FavoritesFolderName
		{
			get
			{
				return new LocalizedString("FavoritesFolderName", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImKqlKeyword
		{
			get
			{
				return new LocalizedString("ImKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxWelcomeEmailSecondaryHeaderAddedBy(string addedBy, string groupName)
		{
			return new LocalizedString("GroupMailboxWelcomeEmailSecondaryHeaderAddedBy", "", false, false, ClientStrings.ResourceManager, new object[]
			{
				addedBy,
				groupName
			});
		}

		public static LocalizedString ImportanceKqlKeyword
		{
			get
			{
				return new LocalizedString("ImportanceKqlKeyword", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MyTasks
		{
			get
			{
				return new LocalizedString("MyTasks", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlternateCalendarWhenYearlyLeap(LocalizedString calendar, IFormattable month, int day)
		{
			return new LocalizedString("AlternateCalendarWhenYearlyLeap", "Ex488312", false, true, ClientStrings.ResourceManager, new object[]
			{
				calendar,
				month,
				day
			});
		}

		public static LocalizedString WhenRecurringWithEndDateOneDayHours(LocalizedString pattern, object startDate, object endDate, object startTime, int hours)
		{
			return new LocalizedString("WhenRecurringWithEndDateOneDayHours", "Ex481B2E", false, true, ClientStrings.ResourceManager, new object[]
			{
				pattern,
				startDate,
				endDate,
				startTime,
				hours
			});
		}

		public static LocalizedString SharingInvitationAndRequestInstruction
		{
			get
			{
				return new LocalizedString("SharingInvitationAndRequestInstruction", "Ex85F32F", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenStartsEndsDifferentDay(object startDate, object startTime, object endDate, object endTime)
		{
			return new LocalizedString("WhenStartsEndsDifferentDay", "Ex9BF390", false, true, ClientStrings.ResourceManager, new object[]
			{
				startDate,
				startTime,
				endDate,
				endTime
			});
		}

		public static LocalizedString TaskWhenEveryOtherDay
		{
			get
			{
				return new LocalizedString("TaskWhenEveryOtherDay", "ExB24AF6", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Conversation
		{
			get
			{
				return new LocalizedString("Conversation", "ExED2DA4", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToDeleteFromSharePointTitle
		{
			get
			{
				return new LocalizedString("FailedToDeleteFromSharePointTitle", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlternateCalendarTaskWhenMonthlyEveryNMonths(LocalizedString calendar, int day, int interval)
		{
			return new LocalizedString("AlternateCalendarTaskWhenMonthlyEveryNMonths", "Ex791D9D", false, true, ClientStrings.ResourceManager, new object[]
			{
				calendar,
				day,
				interval
			});
		}

		public static LocalizedString AlternateCalendarWhenMonthlyThEveryOtherMonth(LocalizedString calendar, LocalizedString order, IFormattable day)
		{
			return new LocalizedString("AlternateCalendarWhenMonthlyThEveryOtherMonth", "Ex23A3FF", false, true, ClientStrings.ResourceManager, new object[]
			{
				calendar,
				order,
				day
			});
		}

		public static LocalizedString JunkEmail
		{
			get
			{
				return new LocalizedString("JunkEmail", "Ex9A426E", false, true, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxWelcomeEmailFooterSubscribeDescriptionText
		{
			get
			{
				return new LocalizedString("GroupMailboxWelcomeEmailFooterSubscribeDescriptionText", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClutterFolderName
		{
			get
			{
				return new LocalizedString("ClutterFolderName", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TaskWhenWeeklyEveryNWeeks(int interval, IFormattable dayOfWeek)
		{
			return new LocalizedString("TaskWhenWeeklyEveryNWeeks", "Ex45C71F", false, true, ClientStrings.ResourceManager, new object[]
			{
				interval,
				dayOfWeek
			});
		}

		public static LocalizedString ClutterNotificationInvitationWeCallIt
		{
			get
			{
				return new LocalizedString("ClutterNotificationInvitationWeCallIt", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxWelcomeEmailDefaultDescription
		{
			get
			{
				return new LocalizedString("GroupMailboxWelcomeEmailDefaultDescription", "", false, false, ClientStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(ClientStrings.IDs key)
		{
			return new LocalizedString(ClientStrings.stringIDs[(uint)key], ClientStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(251);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Data.Storage.ClientStrings", typeof(ClientStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			PostedTo = 4109493280U,
			UnknownDelegateUser = 3201544700U,
			ClutterNotificationInvitationIfYouDontLikeIt = 2520011618U,
			Inbox = 2979702410U,
			CategoryKqlKeyword = 2660925179U,
			RequestedActionReply = 2842725608U,
			ClutterNotificationAutoEnablementNoticeHeader = 699667745U,
			MeetingTentative = 605024017U,
			BodyKqlKeyword = 3115038347U,
			WhenAllDays = 2958797346U,
			GroupMailboxWelcomeEmailFooterUnsubscribeLinkText = 1567502238U,
			TaskWhenDailyRegeneratingPattern = 800057761U,
			UpdateRumLocationFlag = 1082093008U,
			CcColon = 3496891301U,
			GroupMailboxWelcomeEmailUnsubscribeToInboxTitle = 3728294467U,
			HighKqlKeyword = 729677225U,
			OnBehalfOf = 2868846894U,
			SentKqlKeyword = 3636275243U,
			UpdateRumCancellationFlag = 996097864U,
			PrivateAppointmentSubject = 2420384142U,
			GroupMailboxWelcomeMessageHeader2 = 2485040215U,
			Busy = 2052801377U,
			CalendarWhenEveryOtherDay = 2694011047U,
			WhenWeekDays = 2530535283U,
			WhenOnEveryDayOfTheWeek = 2983597720U,
			ClutterNotificationAutoEnablementNoticeHowItWorks = 186518193U,
			RequestedActionRead = 1289378056U,
			GroupMailboxWelcomeEmailPublicTypeText = 3076491009U,
			PolicyTipDefaultMessageRejectOverride = 4206219086U,
			WhenLast = 3798065398U,
			GroupMailboxWelcomeEmailUnsubscribeToInboxSubtitle = 920038251U,
			WhenPart = 3798064771U,
			GroupMailboxAddedMemberMessageDocument2 = 998340857U,
			UpdateRumStartTimeFlag = 3514687136U,
			MyContactsFolderName = 3831476238U,
			DocumentSyncIssues = 2537597608U,
			MeetingAccept = 2892702617U,
			SearchFolders = 3827225687U,
			OOF = 77678270U,
			GroupMailboxWelcomeEmailFooterUnsubscribeDescirptionText = 28323196U,
			GroupMailboxWelcomeEmailShareFilesSubTitle = 3435327809U,
			GroupMailboxWelcomeEmailPrivateTypeText = 2986714461U,
			GroupMailboxWelcomeEmailFooterSubscribeLinkText = 4003125105U,
			SentColon = 295620541U,
			ClutterNotificationO365Closing = 109016526U,
			MyCalendars = 606093953U,
			AttachmentNamesKqlKeyword = 2337243514U,
			UMFaxFolderName = 3055530828U,
			GroupMailboxSuggestToSubscribe = 4038690134U,
			WherePart = 3709219576U,
			MeetingProposedNewTime = 2985148172U,
			Tasks = 2966158940U,
			ClutterNotificationAutoEnablementNoticeWeCallIt = 2957657555U,
			Configuration = 2243726652U,
			ReceivedKqlKeyword = 2687792634U,
			ItemForward = 2676822854U,
			PublicFolderMailboxHierarchyInfo = 4037918816U,
			GroupMailboxAddedMemberMessageDocument1 = 998340860U,
			GroupMailboxAddedSelfMessageHeader = 1139539543U,
			GroupSubscriptionPageSubscribedInfo = 627342781U,
			TaskWhenMonthlyRegeneratingPattern = 2377878649U,
			SentTime = 2677919833U,
			SentItems = 590977256U,
			UpdateRumDuplicateFlags = 931240115U,
			ToDoSearch = 3416726118U,
			GroupMailboxWelcomeEmailO365FooterBrowseViewCalendar = 831219486U,
			UserPhotoNotAnImage = 586169060U,
			OofReply = 51726234U,
			UpdateRumDescription = 2992898169U,
			SharingRequestDenied = 1190007884U,
			CommonViews = 3198424631U,
			TaskWhenDailyEveryDay = 257954985U,
			NoDataAvailable = 2561496264U,
			Root = 3430669166U,
			KoreanLunar = 3011322582U,
			SyncIssues = 3694564633U,
			UnifiedInbox = 2696977948U,
			ContactSubjectFormat = 843955115U,
			ChineseLunar = 2093606811U,
			Outbox = 629464291U,
			UpdateRumInconsistencyFlagsLabel = 2692137911U,
			Hijri = 2173140516U,
			MeetingsKqlKeyword = 946577161U,
			ClutterNotificationInvitationHowItWorks = 2717587388U,
			GroupMailboxAddedMemberMessageDocument3 = 998340858U,
			UpdateRumRecurrenceFlag = 732114395U,
			PolicyTagKqlKeyword = 1261883245U,
			GroupSubscriptionPageBodyFont = 3061781790U,
			ClutterNotificationOptInSubject = 780435470U,
			ClutterNotificationAutoEnablementNoticeItsAutomatic = 2831541605U,
			WorkingElsewhere = 1987690819U,
			NTUser = 1210128587U,
			ElcRoot = 3623590716U,
			ContactFullNameFormat = 3637574683U,
			GroupMailboxAddedMemberMessageCalendar1 = 2442729051U,
			WhenFifth = 3064946485U,
			ClutterNotificationInvitationIntro = 690865655U,
			ExpiresKqlKeyword = 725603143U,
			GroupMailboxAddedMemberMessageConversation2 = 1486137843U,
			CommunicatorHistoryFolderName = 462355316U,
			LowKqlKeyword = 898888055U,
			AllKqlKeyword = 3493739462U,
			GroupMailboxAddedMemberToPublicGroupMessage = 3753304505U,
			Followup = 1712196986U,
			HebrewLunar = 1669914831U,
			Notes = 1601836855U,
			UpdateRumEndTimeFlag = 4285257241U,
			JournalsKqlKeyword = 4086680553U,
			AttendeeInquiryRumDescription = 2395883303U,
			ClutterNotificationOptInHeader = 1358110507U,
			LegacyPDLFakeEntry = 2274376870U,
			PolicyTipDefaultMessageReject = 2426305242U,
			VoicemailKqlKeyword = 637088748U,
			PostedOn = 2543409328U,
			MeetingCancel = 20079527U,
			FromColon = 2918743951U,
			DeletedItems = 3613623199U,
			DocsKqlKeyword = 242457402U,
			ClutterNotificationO365DisplayName = 3850103120U,
			LocalFailures = 2773211886U,
			GroupMailboxWelcomeEmailO365FooterBrowseConversations = 3659541315U,
			JapaneseLunar = 2676513095U,
			ClutterNotificationPeriodicSurveySteps = 3411057958U,
			TaskStatusInProgress = 3709255463U,
			AutomaticDisplayName = 4078173350U,
			SharingInvitationInstruction = 587246061U,
			BirthdayCalendarFolderName = 2610926242U,
			NormalKqlKeyword = 3519087566U,
			ClutterNotificationPeriodicCheckBack = 2937701464U,
			ToKqlKeyword = 104437952U,
			Drafts = 115734878U,
			MeetingDecline = 3043371929U,
			WhenSecond = 3230188362U,
			SubjectKqlKeyword = 232501731U,
			ServerFailures = 2664647494U,
			ClutterNotificationInvitationHeader = 1134735502U,
			UMVoiceMailFolderName = 1618289846U,
			CalendarWhenDailyEveryDay = 1744667246U,
			WhenThird = 954396011U,
			ClutterNotificationOptInPrivacy = 2864414954U,
			SharingInvitationUpdatedSubjectLine = 1025048278U,
			OtherCalendars = 4052458853U,
			GroupMailboxWelcomeEmailSubscribeToInboxSubtitle = 1178930756U,
			Tentative = 1797669216U,
			AttachmentKqlKeyword = 2411966652U,
			WhenOnWeekDays = 3710484300U,
			ClutterNotificationHeaderFont = 3291021260U,
			TaskWhenWeeklyRegeneratingPattern = 3252707377U,
			FaxesKqlKeyword = 4048827998U,
			TaskStatusNotStarted = 3033231921U,
			ToColon = 3465339554U,
			IsReadKqlKeyword = 3882605011U,
			IsFlaggedKqlKeyword = 2572393147U,
			CancellationRumTitle = 136108241U,
			ClutterNotificationPeriodicHeader = 2244917544U,
			GroupSubscriptionPageRequestFailedInfo = 3855098919U,
			ClutterNotificationInvitationSubject = 3366209149U,
			FromKqlKeyword = 2530009445U,
			GroupSubscriptionPageUnsubscribedInfo = 3938988508U,
			GroupMailboxAddedMemberToPrivateGroupMessage = 1708086969U,
			WhenOneMoreOccurrence = 4132923658U,
			GroupSubscriptionUnsubscribeLinkWord = 4182366305U,
			PublicFolderMailboxDumpsterInfo = 1233830609U,
			TaskStatusWaitOnOthers = 2188469818U,
			GroupMailboxWelcomeEmailO365FooterBrowseShareFiles = 2197783321U,
			ClutterNotificationPeriodicSubject = 433190147U,
			GroupMailboxAddedMemberNoJoinedByMessageHeader = 2755014636U,
			RequestedActionNoResponseNecessary = 3895923139U,
			TaskWhenYearlyRegeneratingPattern = 1562762538U,
			Free = 3323263744U,
			CancellationRumDescription = 1871485283U,
			RequestedActionReplyToAll = 99688204U,
			GroupMailboxWelcomeEmailO365FooterTitle = 3733626243U,
			ItemReply = 2403605275U,
			WhenFirst = 3824460724U,
			RpcClientInitError = 483059378U,
			RequestedActionDoNotForward = 348255911U,
			SizeKqlKeyword = 2074560464U,
			GroupSubscriptionPagePrivateGroupInfo = 2936941939U,
			UserPhotoNotFound = 693971404U,
			FromFavoriteSendersFolderName = 2768584303U,
			GroupMailboxAddedMemberMessageFont = 2869025391U,
			RequestedActionForward = 2905210277U,
			CcKqlKeyword = 4167892423U,
			Conversations = 1697583518U,
			GroupSubscriptionPagePublicGroupInfo = 3614445595U,
			ContactsKqlKeyword = 677901016U,
			WhenBothWeekendDays = 3513814141U,
			Conflicts = 2377377097U,
			PostsKqlKeyword = 51369050U,
			UserPhotoPreviewNotFound = 1940443510U,
			WhenEveryDay = 1236916945U,
			SharingRequestInstruction = 1844900681U,
			ClutterNotificationAutoEnablementNoticeIntro = 940910702U,
			ClutterNotificationBodyFont = 1418871467U,
			ClutterNotificationInvitationO365Helps = 1685151768U,
			WhenFourth = 3953967230U,
			RequestedActionFollowUp = 1657114432U,
			GroupSubscriptionPageRequestFailed = 3221177017U,
			ClutterNotificationAutoEnablementNoticeSubject = 1300788680U,
			PeoplesCalendars = 2028589045U,
			EHAMigration = 1660374630U,
			SubjectColon = 3413891549U,
			Calendar = 1292798904U,
			RequestedActionAny = 899464318U,
			WhenEveryWeekDay = 2912044233U,
			TasksKqlKeyword = 2607340737U,
			NotesKqlKeyword = 802022316U,
			TaskStatusCompleted = 1726717990U,
			ClutterNotificationInvitationItsAutomatic = 3501929050U,
			RssSubscriptions = 3598244064U,
			RequestedActionCall = 153688296U,
			PolicyTipDefaultMessageNotifyOnly = 3795294554U,
			FailedToUploadToSharePointTitle = 219237725U,
			TaskStatusDeferred = 2255519906U,
			SharingRequestAllowed = 3425704829U,
			UpdateRumMissingItemFlag = 1694200734U,
			ParticipantsKqlKeyword = 963851651U,
			GroupMailboxWelcomeEmailStartConversationTitle = 1044563024U,
			GroupMailboxWelcomeEmailShareFilesTitle = 2833103505U,
			LegacyArchiveJournals = 3635271833U,
			RecipientsKqlKeyword = 3126868667U,
			RssFeedsKqlKeyword = 2904245352U,
			GroupMailboxWelcomeEmailSubscribeToInboxTitle = 2776152054U,
			GroupMailboxAddedMemberMessageCalendar3 = 2442729049U,
			RequestedActionReview = 1616483908U,
			KindKqlKeyword = 3043633671U,
			EmailKqlKeyword = 916455203U,
			UTC = 2450331336U,
			BccKqlKeyword = 1793503025U,
			RequestedActionForYourInformation = 3029761970U,
			Journal = 4137480277U,
			GroupMailboxAddedMemberMessageConversation1 = 1486137846U,
			GroupMailboxAddedMemberMessageConversation3 = 1486137844U,
			HasAttachmentKqlKeyword = 136364712U,
			GroupSubscriptionPageGroupInfoFont = 1780326221U,
			Contacts = 1716044995U,
			GroupMailboxAddedMemberMessageCalendar2 = 2442729048U,
			FavoritesFolderName = 1836161108U,
			ImKqlKeyword = 1516506571U,
			ImportanceKqlKeyword = 2755562843U,
			MyTasks = 1165698602U,
			SharingInvitationAndRequestInstruction = 143815687U,
			TaskWhenEveryOtherDay = 632725474U,
			Conversation = 710925581U,
			FailedToDeleteFromSharePointTitle = 3191048500U,
			JunkEmail = 2241039844U,
			GroupMailboxWelcomeEmailFooterSubscribeDescriptionText = 765381967U,
			ClutterFolderName = 1976982380U,
			ClutterNotificationInvitationWeCallIt = 261418034U,
			GroupMailboxWelcomeEmailDefaultDescription = 1524084310U
		}

		private enum ParamIDs
		{
			ClutterNotificationAutoEnablementNoticeYoullBeEnabed,
			FailedToSynchronizeChangesFromSharePoint,
			ClutterNotificationAutoEnablementNoticeLearnMore,
			RemindersSearchFolderName,
			MdnReadNoTime,
			FailedMaintenanceSynchronizationsText,
			CalendarWhenYearly,
			AlternateCalendarWhenMonthlyEveryNMonths,
			AlternateCalendarWhenYearlyThLeap,
			FailedToSynchronizeChangesFromSharePointText,
			ClutterNotificationOptInHowToTrain,
			WhenRecurringNoEndDateDaysHours,
			WhenRecurringWithEndDateNoDuration,
			FailedToUploadToSharePointErrorText,
			SharingInvitationAndRequest,
			WhenFiveDaysOfWeek,
			AlternateCalendarTaskWhenMonthlyThEveryMonth,
			CalendarWhenWeeklyEveryWeek,
			WhenRecurringNoEndDateOneDayHoursMinutes,
			WhenThreeDaysOfWeek,
			AlternateCalendarWhenMonthlyEveryMonth,
			WhenRecurringWithEndDateDaysNoDuration,
			GroupMailboxWelcomeEmailHeader,
			MdnNotReadNoTime,
			WhenRecurringWithEndDateDaysOneHourMinutes,
			WhenRecurringNoEndDateOneDayOneHour,
			GroupMailboxWelcomeMessageSubject,
			GroupMailboxAddedMemberNoJoinedBySubject,
			GroupMailboxAddedMemberMessageSubject,
			AlternateCalendarTaskWhenYearlyLeap,
			WhenRecurringWithEndDateOneDayOneHour,
			GroupMailboxJoinRequestEmailSubject,
			SharingAccept,
			TaskWhenNWeeksRegeneratingPattern,
			AlternateCalendarWhenMonthlyEveryOtherMonth,
			ClutterNotificationOptInLearnMore,
			TaskWhenYearlyTh,
			CalendarWhenWeeklyEveryAlterateWeek,
			TaskWhenMonthlyEveryMonth,
			AppendStrings,
			ClutterNotificationDisableDeepLink,
			CalendarWhenWeeklyEveryNWeeks,
			WhenRecurringWithEndDateNoTimeInDay,
			FailedMaintenanceSynchronizations,
			WhenRecurringNoEndDateNoTimeInDay,
			WhenRecurringWithEndDateDaysOneHour,
			UserPhotoFileTooLarge,
			ClutterNotificationPeriodicIntro,
			WhenRecurringWithEndDateDaysHoursMinutes,
			GroupMailboxWelcomeEmailStartConversationSubtitle,
			MdnRead,
			CalendarWhenMonthlyEveryMonth,
			TaskWhenWeeklyEveryWeek,
			WhenRecurringNoEndDateDaysHoursMinutes,
			GroupMailboxAddedSelfMessageSubject,
			CalendarWhenMonthlyThEveryOtherMonth,
			AlternateCalendarTaskWhenYearly,
			WhenRecurringNoEndDateDaysOneHourMinutes,
			WhenRecurringWithEndDateOneDayNoDuration,
			GroupSubscriptionPageSubscribeFailedMaxSubscribers,
			WhenRecurringWithEndDateOneDayHoursMinutes,
			SharingICal,
			SharingAnonymous,
			AlternateCalendarTaskWhenMonthlyThEveryOtherMonth,
			WhenRecurringNoEndDateNoDuration,
			MdnNotRead,
			GroupMailboxWelcomeEmailSecondaryHeaderYouJoined,
			AlternateCalendarWhenMonthlyThEveryMonth,
			GroupSubscriptionUnsubscribeInfoHtml,
			GroupSubscriptionPageSubscribeFailedNotAMember,
			WhenRecurringNoEndDateDaysOneHour,
			WhenSixDaysOfWeek,
			AlternateCalendarWhenYearly,
			WhenTwoDaysOfWeek,
			SharingRequest,
			JointStrings,
			UserPhotoImageTooSmall,
			GroupSubscriptionPageUnsubscribed,
			AlternateCalendarTaskWhenMonthlyThEveryNMonths,
			ClutterNotificationOptInFeedback,
			WhenFourDaysOfWeek,
			CalendarWhenMonthlyThEveryNMonths,
			ClutterNotificationOptInIntro,
			SharingInvitation,
			CalendarWhenYearlyTh,
			FailedToSynchronizeMembershipFromSharePointText,
			TaskWhenMonthlyEveryOtherMonth,
			SharingInvitationNonPrimary,
			WhenSevenDaysOfWeek,
			GroupMailboxAddedMemberMessageHeader,
			WhenOneDayOfWeek,
			AlternateCalendarTaskWhenMonthlyEveryOtherMonth,
			CalendarWhenDailyEveryNDays,
			CalendarWhenMonthlyEveryOtherMonth,
			FailedToSynchronizeHierarchyChangesFromSharePointText,
			RpcClientRequestError,
			WhenRecurringNoEndDateOneDayHours,
			WhenRecurringWithEndDateDaysMinutes,
			SharingDecline,
			ClutterNotificationTakeSurveyDeepLink,
			AlternateCalendarTaskWhenMonthlyEveryMonth,
			TaskWhenDailyEveryNDays,
			TaskWhenNMonthsRegeneratingPattern,
			GroupMailboxAddedMemberMessageTitle,
			WhenNMoreOccurrences,
			GroupMailboxJoinRequestEmailBody,
			AlternateCalendarTaskWhenYearlyThLeap,
			ClutterNotificationInvitationLearnMore,
			CalendarWhenMonthlyThEveryMonth,
			AlternateCalendarTaskWhenYearlyTh,
			TaskWhenMonthlyThEveryNMonths,
			WhenRecurringWithEndDateOneDayMinutes,
			WhenRecurringWithEndDateOneDayOneHourMinutes,
			GroupMailboxWelcomeMessageHeader1,
			WhenRecurringNoEndDateDaysMinutes,
			AlternateCalendarWhenYearlyTh,
			CalendarWhenMonthlyEveryNMonths,
			TaskWhenYearly,
			TaskWhenMonthlyEveryNMonths,
			WhenStartsEndsSameDay,
			FailedToSynchronizeMembershipFromSharePoint,
			GroupSubscriptionPageSubscribed,
			RumFooter,
			FailedToDeleteFromSharePointErrorText,
			TaskWhenNDaysRegeneratingPattern,
			TaskWhenWeeklyEveryAlterateWeek,
			PublicFolderMailboxInfoFolderEnumeration,
			ClutterNotificationPeriodicLearnMore,
			MissingItemRumDescription,
			WhenRecurringNoEndDate,
			ClutterNotificationEnableDeepLink,
			AlternateCalendarWhenMonthlyThEveryNMonths,
			TaskWhenMonthlyThEveryMonth,
			WhenRecurringNoEndDateOneDayMinutes,
			WhenRecurringNoEndDateOneDayNoDuration,
			WhenRecurringNoEndDateDaysNoDuration,
			WhenRecurringWithEndDateDaysHours,
			ErrorSharePointSiteHasNoValidUrl,
			TaskWhenNYearsRegeneratingPattern,
			UserPhotoFileTooSmall,
			TaskWhenMonthlyThEveryOtherMonth,
			GroupSubscriptionUnsubscribeInfoText,
			WhenRecurringWithEndDate,
			WhenRecurringNoEndDateOneDayOneHourMinutes,
			GroupMailboxWelcomeEmailSecondaryHeaderAddedBy,
			AlternateCalendarWhenYearlyLeap,
			WhenRecurringWithEndDateOneDayHours,
			WhenStartsEndsDifferentDay,
			AlternateCalendarTaskWhenMonthlyEveryNMonths,
			AlternateCalendarWhenMonthlyThEveryOtherMonth,
			TaskWhenWeeklyEveryNWeeks
		}
	}
}
