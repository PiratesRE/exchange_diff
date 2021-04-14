using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class WellKnownNamedProperties
	{
		private static Dictionary<NamedProp, NamedProp> GenerateTable()
		{
			Dictionary<NamedProp, NamedProp> dictionary = new Dictionary<NamedProp, NamedProp>(WellKnownNamedProperties.namedPropList.Length);
			for (int i = 0; i < WellKnownNamedProperties.namedPropList.Length; i++)
			{
				dictionary.Add(WellKnownNamedProperties.namedPropList[i], WellKnownNamedProperties.namedPropList[i]);
			}
			return dictionary;
		}

		public static NamedProp Find(NamedProp property)
		{
			NamedProp result;
			if (WellKnownNamedProperties.Properties.TryGetValue(property, out result))
			{
				return result;
			}
			return null;
		}

		private const int dispidABPArrayType = 32809;

		private const int dispidABPEmailList = 32808;

		private const int dispidAddressCountryCode = 32989;

		private const int dispidAgingDontAgeMe = 34062;

		private const int dispidAllAttendeesString = 33336;

		private const int dispidAllowedFlagString = 61624;

		private const int dispidAllowExternCheck = 33350;

		private const int dispidAnniversaryEventEID = 32846;

		private const int dispidApptAuxFlags = 33287;

		private const int dispidApptColor = 33300;

		private const int dispidApptCounterProposal = 33367;

		private const int dispidApptDuration = 33299;

		private const int dispidApptEndDate = 33297;

		private const int dispidApptEndTime = 33296;

		private const int dispidApptEndWhole = 33294;

		private const int dispidApptExtractTime = 33325;

		private const int dispidApptExtractVersion = 33324;

		private const int dispidApptLastSequence = 33283;

		private const int dispidApptMessageClass = 36;

		private const int dispidApptNotAllowPropose = 33370;

		private const int dispidApptOpenViewProposal = 33371;

		private const int dispidApptProposalNum = 33369;

		private const int dispidApptProposedDuration = 33366;

		private const int dispidApptProposedEndDate = 33365;

		private const int dispidApptProposedEndTime = 33363;

		private const int dispidApptProposedEndWhole = 33361;

		private const int dispidApptProposedLocation = 33372;

		private const int dispidApptProposedStartDate = 33364;

		private const int dispidApptProposedStartTime = 33362;

		private const int dispidApptProposedStartWhole = 33360;

		private const int dispidApptRecur = 33302;

		private const int dispidApptReplyName = 33328;

		private const int dispidApptReplyTime = 33312;

		private const int dispidApptSeqTime = 33282;

		private const int dispidApptSequence = 33281;

		private const int dispidApptStartDate = 33298;

		private const int dispidApptStartTime = 33295;

		private const int dispidApptStartTimes = 34227;

		private const int dispidApptStartWhole = 33293;

		private const int dispidApptStateFlags = 33303;

		private const int dispidApptStickerID = 33305;

		private const int dispidApptSubType = 33301;

		private const int dispidApptTZDefEndDisplay = 33375;

		private const int dispidApptTZDefRecur = 33376;

		private const int dispidApptTZDefStartDisplay = 33374;

		private const int dispidApptUnsendableRecips = 33341;

		private const int dispidApptUpdateTime = 33318;

		private const int dispidAttachStripped = 34122;

		private const int dispidAutoFillLocation = 33338;

		private const int dispidAutoLog = 32805;

		private const int dispidAutoSaveOriginalItemInfo = 34216;

		private const int dispidAutoStartCheck = 33348;

		private const int dispidAutoStartWhen = 33349;

		private const int dispidBCCardPicture = 32833;

		private const int dispidBCDisplayDefinition = 32832;

		private const int dispidBilling = 34101;

		private const int dispidBirthdayEventEID = 32845;

		private const int dispidBusyStatus = 33285;

		private const int dispidCalendarLastChangeAction = 2;

		private const int dispidCalendarProcessed = 1;

		private const int dispidCategories = 9000;

		private const int dispidCategoriesStr = 9001;

		private const int dispidCCAttendeesString = 33340;

		private const int dispidChangeHighlight = 33284;

		private const int dispidCheckAddressButton = 32772;

		private const int dispidCheckNameButton = 32771;

		private const int dispidChildrenStr = 32780;

		private const int dispidClassDesc = 34231;

		private const int dispidClassGuid = 34232;

		private const int dispidClassification = 34230;

		private const int dispidClassified = 34229;

		private const int dispidClassKeep = 34234;

		private const int dispidCleanGlobalObjId = 35;

		private const int dispidClipEnd = 33334;

		private const int dispidClipStart = 33333;

		private const int dispidCollaborateDoc = 33351;

		private const int dispidColorSchemeMappingXML = 34243;

		private const int dispidCommonEnd = 34071;

		private const int dispidCommonStart = 34070;

		private const int dispidCompanies = 34105;

		private const int dispidCompaniesStr = 34107;

		private const int dispidCompanyAndFullName = 32792;

		private const int dispidCompanyLastFirstNoSpace = 32818;

		private const int dispidCompanyLastFirstSpaceOnly = 32819;

		private const int dispidConfAliasDisplay = 32863;

		private const int dispidConfBackupServerIndex = 32856;

		private const int dispidConfCheck = 33344;

		private const int dispidConfCheckChanged = 33343;

		private const int dispidConfDefServerIndex = 32855;

		private const int dispidConfEmailIndex = 32857;

		private const int dispidConflictItems = 34197;

		private const int dispidConfReserved = 32858;

		private const int dispidConfServerDisplay = 32864;

		private const int dispidConfServerNames = 32854;

		private const int dispidConfServerNamesStr = 32862;

		private const int dispidConfType = 33345;

		private const int dispidContactCharSet = 32803;

		private const int dispidContactEmailAddressesStr = 32861;

		private const int dispidContactItemData = 32775;

		private const int dispidContactItemData2 = 32779;

		private const int dispidContactItemDataBase = 32872;

		private const int dispidContactLinkDateTime = 32993;

		private const int dispidContactLinkEntry = 34181;

		private const int dispidContactLinkName = 34182;

		private const int dispidContactLinkSearchKey = 34180;

		private const int dispidContactLinkVersion = 32999;

		private const int dispidContacts = 34106;

		private const int dispidContactSelectedBase = 32884;

		private const int dispidContactsStr = 34108;

		private const int dispidContactUserField1 = 32847;

		private const int dispidContactUserField2 = 32848;

		private const int dispidContactUserField3 = 32849;

		private const int dispidContactUserField4 = 32850;

		private const int dispidContentClass = 34200;

		private const int dispidContentType = 34201;

		private const int dispidCreator = 34236;

		private const int dispidCurrentVersion = 34130;

		private const int dispidCurrentVersionName = 34132;

		private const int dispidCustomFlag = 34114;

		private const int dispidCustomPages = 34069;

		private const int dispidDayOfMonth = 4096;

		private const int dispidDayOfWeekMask = 4097;

		private const int dispidDeleteAssocRequest = 33317;

		private const int dispidDepartment = 32784;

		private const int dispidDirectory = 33346;

		private const int dispidDirtyTimesOrStatus = 33319;

		private const int dispidDLChecksum = 32844;

		private const int dispidDLCountMembers = 32843;

		private const int dispidDLMembers = 32853;

		private const int dispidDLName = 32851;

		private const int dispidDLOneOffMembers = 32852;

		private const int dispidDocObjWordmail = 34183;

		private const int dispidDontAgeLog = 32810;

		private const int dispidDrmAttachmentNumber = 34205;

		private const int dispidEmail1DisplayName = 32896;

		private const int dispidEmail1EmailType = 32903;

		private const int dispidEmail1EntryID = 32897;

		private const int dispidEmail1RTF = 32902;

		private const int dispidEmail2AddrType = 32914;

		private const int dispidEmail2DisplayName = 32912;

		private const int dispidEmail2EmailAddress = 32915;

		private const int dispidEmail2EmailType = 32919;

		private const int dispidEmail2EntryID = 32913;

		private const int dispidEmail2OriginalDisplayName = 32916;

		private const int dispidEmail2OriginalEntryID = 32917;

		private const int dispidEmail2RTF = 32918;

		private const int dispidEmail3AddrType = 32930;

		private const int dispidEmail3DisplayName = 32928;

		private const int dispidEmail3EmailAddress = 32931;

		private const int dispidEmail3EmailType = 32935;

		private const int dispidEmail3EntryID = 32929;

		private const int dispidEmail3OriginalDisplayName = 32932;

		private const int dispidEmail3OriginalEntryID = 32933;

		private const int dispidEmail3RTF = 32934;

		private const int dispidEmailAddrType = 32898;

		private const int dispidEmailDisplayName = 32896;

		private const int dispidEmailEmailAddress = 32899;

		private const int dispidEmailEmailType = 32903;

		private const int dispidEmailEntryID = 32897;

		private const int dispidEmailList = 32807;

		private const int dispidEmailOriginalDisplayName = 32900;

		private const int dispidEmailOriginalEntryID = 32901;

		private const int dispidEmailRTF = 32902;

		private const int dispidEMSAbX509Cert = 32985;

		private const int dispidEndRecurDate = 4098;

		private const int dispidEndRecurTime = 4108;

		private const int dispidExceptionReplaceTime = 33320;

		private const int dispidExceptions = 4110;

		private const int dispidFax1AddrType = 32946;

		private const int dispidFax1DisplayName = 32944;

		private const int dispidFax1EmailAddress = 32947;

		private const int dispidFax1EmailType = 32951;

		private const int dispidFax1EntryID = 32945;

		private const int dispidFax1OriginalDisplayName = 32948;

		private const int dispidFax1OriginalEntryID = 32949;

		private const int dispidFax1RTF = 32950;

		private const int dispidFax2AddrType = 32962;

		private const int dispidFax2DisplayName = 32960;

		private const int dispidFax2EmailAddress = 32963;

		private const int dispidFax2EmailType = 32967;

		private const int dispidFax2EntryID = 32961;

		private const int dispidFax2OriginalDisplayName = 32964;

		private const int dispidFax2OriginalEntryID = 32965;

		private const int dispidFax2RTF = 32966;

		private const int dispidFax3AddrType = 32978;

		private const int dispidFax3DisplayName = 32976;

		private const int dispidFax3EmailAddress = 32979;

		private const int dispidFax3EmailType = 32983;

		private const int dispidFax3EntryID = 32977;

		private const int dispidFax3OriginalDisplayName = 32980;

		private const int dispidFax3OriginalEntryID = 32981;

		private const int dispidFax3RTF = 32982;

		private const int dispidFDirtyLocation = 33325;

		private const int dispidFDirtyTimes = 33324;

		private const int dispidFEndByOcc = 4111;

		private const int dispidFExceptionalAttendees = 33323;

		private const int dispidFExceptionalBody = 33286;

		private const int dispidFHaveWrittenTracking = 34824;

		private const int dispidFileUnder = 32773;

		private const int dispidFileUnderId = 32774;

		private const int dispidFileUnderList = 32806;

		private const int dispidFInvited = 33321;

		private const int dispidFirstMiddleLastSuffix = 32823;

		private const int dispidFlagStringEnum = 34240;

		private const int dispidFloatTest = 33038;

		private const int dispidFMtgDataChanged = 33337;

		private const int dispidFNoEndDate = 4107;

		private const int dispidFormPropStream = 34075;

		private const int dispidFormStorage = 34063;

		private const int dispidForwardNotificationRecipients = 33377;

		private const int dispidFOthersAppt = 33327;

		private const int dispidFPostalAddress = 32770;

		private const int dispidFramesetBody = 34186;

		private const int dispidFreeBusyLocation = 32984;

		private const int dispidFSendPlainText = 32769;

		private const int dispidFShouldTNEF = 34213;

		private const int dispidFSliding = 4106;

		private const int dispidFullNameAndCompany = 32793;

		private const int dispidFwrdInstance = 33290;

		private const int dispidHasPicture = 32789;

		private const int dispidHeaderItem = 34168;

		private const int dispidHomeAddress = 32794;

		private const int dispidHomeAddressCountryCode = 32986;

		private const int dispidHTML = 32811;

		private const int dispidHtmlForm = 34110;

		private const int dispidImapDeleted = 34160;

		private const int dispidImgAttchmtsCompressLevel = 34195;

		private const int dispidImgAttchmtsPreviewSize = 34196;

		private const int dispidInetAcctName = 34176;

		private const int dispidInetAcctStamp = 34177;

		private const int dispidInfoPathFormType = 34225;

		private const int dispidInstance = 4099;

		private const int dispidInstMsg = 32866;

		private const int dispidIntegerTest = 33037;

		private const int dispidIntendedBusyStatus = 33316;

		private const int dispidInterConnectBizcard = 32834;

		private const int dispidInterConnectBizcardFlag = 32835;

		private const int dispidInterConnectBizcardLastUpdate = 32836;

		private const int dispidInterval = 4100;

		private const int dispidIsException = 10;

		private const int dispidIsInfoMailPost = 34207;

		private const int dispidIsIPFax = 34203;

		private const int dispidIsRecurring = 5;

		private const int dispidJunkEmailMove = 34304;

		private const int dispidJunkMoveStamp = 34214;

		private const int dispidLastAuthorClass = 34179;

		private const int dispidLastFirstAndSuffix = 32822;

		private const int dispidLastFirstNoSpace = 32816;

		private const int dispidLastFirstNoSpaceAndSuffix = 32824;

		private const int dispidLastFirstNoSpaceCompany = 32820;

		private const int dispidLastFirstSpaceOnly = 32817;

		private const int dispidLastFirstSpaceOnlyCompany = 32821;

		private const int dispidLastNameAndFirstName = 32791;

		private const int dispidLCContentClass = 34206;

		private const int dispidLinkedApptItems = 34226;

		private const int dispidLinkedTaskItems = 33292;

		private const int dispidLocation = 33288;

		private const int dispidLogContactLog = 34573;

		private const int dispidLogDocPosted = 34577;

		private const int dispidLogDocPrinted = 34574;

		private const int dispidLogDocRouted = 34576;

		private const int dispidLogDocSaved = 34575;

		private const int dispidLogDuration = 34567;

		private const int dispidLogEnd = 34568;

		private const int dispidLogFlags = 34572;

		private const int dispidLogStart = 34566;

		private const int dispidLogStartDate = 34564;

		private const int dispidLogStartTime = 34565;

		private const int dispidLogType = 34560;

		private const int dispidLogTypeDesc = 34578;

		private const int dispidMarkedForDownload = 34161;

		private const int dispidMeetingType = 38;

		private const int dispidMeFlag = 32865;

		private const int dispidMileage = 34100;

		private const int dispidMinReadVersion = 34128;

		private const int dispidMinWriteVersion = 34129;

		private const int dispidMonthOfYear = 4102;

		private const int dispidMoreAddrType = 32859;

		private const int dispidMoreEmailAddress = 32860;

		private const int dispidMWSURL = 33289;

		private const int dispidNetMeetingConferenceSerPassword = 33353;

		private const int dispidNetMeetingConferenceServerAllowExternal = 33350;

		private const int dispidNetMeetingDocPathName = 33345;

		private const int dispidNetMeetingOrganizerAlias = 32867;

		private const int dispidNetShowURL = 33352;

		private const int dispidNonSendableBCC = 34104;

		private const int dispidNonSendableCC = 34103;

		private const int dispidNonSendableTo = 34102;

		private const int dispidNonSendBccTrackStatus = 34117;

		private const int dispidNonSendCcTrackStatus = 34116;

		private const int dispidNonSendToTrackStatus = 34115;

		private const int dispidNoteColor = 35584;

		private const int dispidNoteHeight = 35587;

		private const int dispidNoteOnTop = 35585;

		private const int dispidNoteWidth = 35586;

		private const int dispidNoteX = 35588;

		private const int dispidNoteY = 35589;

		private const int dispidOccurrences = 4101;

		private const int dispidOfficeCommunicatorOptions = 2;

		private const int dispidOfflineStatus = 34233;

		private const int dispidOldLocation = 40;

		private const int dispidOldWhen = 39;

		private const int dispidOldWhenEndWhole = 42;

		private const int dispidOldWhenStartWhole = 41;

		private const int dispidOnlinePassword = 33353;

		private const int dispidOrgAlias = 33347;

		private const int dispidOrganizerExceptionReplaceTime = 33322;

		private const int dispidOrigStoreEid = 33335;

		private const int dispidOtherAddress = 32796;

		private const int dispidOtherAddressCountryCode = 32988;

		private const int dispidOwnerName = 33326;

		private const int dispidPageDirStream = 34067;

		private const int dispidPercentComplete = 33026;

		private const int dispidPhishingStamp = 34215;

		private const int dispidPostalAddressId = 32802;

		private const int dispidPostRssChannel = 35076;

		private const int dispidPostRssChannelLink = 35072;

		private const int dispidPostRssItemGuid = 35075;

		private const int dispidPostRssItemHash = 35074;

		private const int dispidPostRssItemLink = 35073;

		private const int dispidPostRssItemXml = 35077;

		private const int dispidPostRssSubscription = 35078;

		private const int dispidPrivate = 34054;

		private const int dispidPropDefStream = 34112;

		private const int dispidProposedWhenProp = 37;

		private const int dispidRecallTime = 34121;

		private const int dispidRecurDuration = 4109;

		private const int dispidRecurPattern = 33330;

		private const int dispidRecurrenceType = 4103;

		private const int dispidRecurring = 33315;

		private const int dispidRecurType = 33329;

		private const int dispidReferenceEID = 34237;

		private const int dispidReferredBy = 32782;

		private const int dispidReminderDelta = 34049;

		private const int dispidReminderFileParam = 34079;

		private const int dispidReminderNextTime = 34144;

		private const int dispidReminderOverride = 34076;

		private const int dispidReminderPlaySound = 34078;

		private const int dispidReminderSet = 34051;

		private const int dispidReminderTime = 34050;

		private const int dispidReminderTimeDate = 34053;

		private const int dispidReminderTimeTime = 34052;

		private const int dispidReminderType = 34077;

		private const int dispidRemoteAttachment = 36615;

		private const int dispidRemoteEID = 36609;

		private const int dispidRemoteMsgClass = 36610;

		private const int dispidRemoteSearchKey = 36614;

		private const int dispidRemoteStatus = 34065;

		private const int dispidRemoteXferSize = 36613;

		private const int dispidRemoteXferTime = 36612;

		private const int dispidRemoteXP = 36611;

		private const int dispidRequest = 34096;

		private const int dispidResendTime = 36096;

		private const int dispidResponseState = 33;

		private const int dispidResponseStatus = 33304;

		private const int dispidScriptStream = 34113;

		private const int dispidSelectedEmailAddress = 32776;

		private const int dispidSelectedOriginalDisplayName = 32777;

		private const int dispidSelectedOriginalEntryID = 32778;

		private const int dispidSendMtgAsICAL = 33280;

		private const int dispidSharedItemOwner = 34241;

		private const int dispidSharingAggregationProtocolName = 35586;

		private const int dispidSharingAggregationStatus = 35494;

		private const int dispidSharingAnonymity = 35353;

		private const int dispidSharingAutoPane = 34192;

		private const int dispidSharingBindingEid = 35373;

		private const int dispidSharingBrowseUrl = 35409;

		private const int dispidSharingCaps = 35351;

		private const int dispidSharingConfigUrl = 35364;

		private const int dispidSharingDataRangeEnd = 35397;

		private const int dispidSharingDataRangeStart = 35396;

		private const int dispidSharingDetail = 35371;

		private const int dispidSharingDetailedStatus = 35488;

		private const int dispidSharingDontUse1 = 35345;

		private const int dispidSharingDontUse2 = 35346;

		private const int dispidSharingDontUse3 = 35350;

		private const int dispidSharingDontUse4 = 35380;

		private const int dispidSharingDontUse5 = 35411;

		private const int dispidSharingDontUse6 = 35412;

		private const int dispidSharingEnabled = 34188;

		private const int dispidSharingErrorCause = 35530;

		private const int dispidSharingErrorResolution = 35531;

		private const int dispidSharingETagHeader = 35490;

		private const int dispidSharingExtXml = 35361;

		private const int dispidSharingFilter = 35347;

		private const int dispidSharingFlags = 35338;

		private const int dispidSharingFlavor = 35352;

		private const int dispidSharingFolderEid = 35349;

		private const int dispidSharingFooterID = 34193;

		private const int dispidSharingIndexEid = 35374;

		private const int dispidSharingInitiatorEid = 35337;

		private const int dispidSharingInitiatorName = 35335;

		private const int dispidSharingInitiatorSmtp = 35336;

		private const int dispidSharingInstanceGuid = 35356;

		private const int dispidSharingLastAggregatedItemTimestamp = 35491;

		private const int dispidSharingLastAutoSync = 35413;

		private const int dispidSharingLastModifiedHeader = 35489;

		private const int dispidSharingLastSuccessSyncTime = 35492;

		private const int dispidSharingLastSync = 35359;

		private const int dispidSharingLocalComment = 35405;

		private const int dispidSharingLocalEwsId = 35430;

		private const int dispidSharingLocalLastMod = 35363;

		private const int dispidSharingLocalName = 35343;

		private const int dispidSharingLocalPath = 35342;

		private const int dispidSharingLocalStoreUid = 35401;

		private const int dispidSharingLocalType = 35348;

		private const int dispidSharingLocalUid = 35344;

		private const int dispidSharingMaxAttachments = 35526;

		private const int dispidSharingMaxMessageSize = 35527;

		private const int dispidSharingMaxNumberOfEmails = 35524;

		private const int dispidSharingMaxNumberOfFolders = 35525;

		private const int dispidSharingMaxObjectsInSync = 35523;

		private const int dispidSharingMaxRecipients = 35528;

		private const int dispidSharingMigrationState = 35529;

		private const int dispidSharingMinSettingPollInterval = 35521;

		private const int dispidSharingMinSyncPollInterval = 35520;

		private const int dispidSharingOriginalMessageEid = 35369;

		private const int dispidSharingParentBindingEid = 35420;

		private const int dispidSharingParticipants = 35358;

		private const int dispidSharingPermissions = 35355;

		private const int dispidSharingProviderExtension = 35339;

		private const int dispidSharingProviderGuid = 35329;

		private const int dispidSharingProviderName = 35330;

		private const int dispidSharingProviderUrl = 35331;

		private const int dispidSharingRangeEnd = 35399;

		private const int dispidSharingRangeStart = 35398;

		private const int dispidSharingReciprocation = 35354;

		private const int dispidSharingRemoteByteSize = 35403;

		private const int dispidSharingRemoteComment = 35375;

		private const int dispidSharingRemoteCrc = 35404;

		private const int dispidSharingRemoteEwsId = 35429;

		private const int dispidSharingRemoteLastMod = 35362;

		private const int dispidSharingRemoteMsgCount = 35407;

		private const int dispidSharingRemoteName = 35333;

		private const int dispidSharingRemotePass = 35341;

		private const int dispidSharingRemotePath = 35332;

		private const int dispidSharingRemoteStoreUid = 35400;

		private const int dispidSharingRemoteType = 35357;

		private const int dispidSharingRemoteUid = 35334;

		private const int dispidSharingRemoteUser = 35340;

		private const int dispidSharingRemoteVersion = 35419;

		private const int dispidSharingResponseTime = 35368;

		private const int dispidSharingResponseType = 35367;

		private const int dispidSharingRoamLog = 35406;

		private const int dispidSharingRssHash = 35360;

		private const int dispidSharingSavedSession = 35422;

		private const int dispidSharingServerStatus = 34202;

		private const int dispidSharingServerUrl = 34190;

		private const int dispidSharingSharingAggregationProtocolVersion = 35585;

		private const int dispidSharingStart = 35365;

		private const int dispidSharingStatus = 35328;

		private const int dispidSharingStop = 35366;

		private const int dispidSharingSubscriptionConfiguration = 35584;

		private const int dispidSharingSubscriptionName = 35587;

		private const int dispidSharingSubscriptions = 35840;

		private const int dispidSharingSyncFlags = 35424;

		private const int dispidSharingSyncInterval = 35370;

		private const int dispidSharingSyncMultiplier = 35522;

		private const int dispidSharingSyncRange = 35493;

		private const int dispidSharingTimeToLive = 35372;

		private const int dispidSharingTimeToLiveAuto = 35414;

		private const int dispidSharingTitle = 34191;

		private const int dispidSharingWebUrl = 34198;

		private const int dispidSharingWlidAuthPolicy = 35504;

		private const int dispidSharingWlidAuthToken = 35506;

		private const int dispidSharingWlidAuthTokenExpireTime = 35507;

		private const int dispidSharingWlidUserPuid = 35505;

		private const int dispidSharingWorkingHoursDays = 35394;

		private const int dispidSharingWorkingHoursEnd = 35393;

		private const int dispidSharingWorkingHoursStart = 35392;

		private const int dispidSharingWorkingHoursTZ = 35395;

		private const int dispidSharingWssAllFolderIDs = 35426;

		private const int dispidSharingWssAlternateUrls = 35418;

		private const int dispidSharingWssCachedSchema = 35421;

		private const int dispidSharingWssCmd = 35377;

		private const int dispidSharingWssFileRelUrl = 35416;

		private const int dispidSharingWssFolderID = 35425;

		private const int dispidSharingWssFolderRelUrl = 35415;

		private const int dispidSharingWssListRelUrl = 35378;

		private const int dispidSharingWssPrevFolderRelUrls = 35417;

		private const int dispidSharingWssServerRelUrl = 35423;

		private const int dispidSharingWssSiteName = 35379;

		private const int dispidSharingWssVer = 35376;

		private const int dispidSideEffects = 34064;

		private const int dispidSmartNoAttach = 34068;

		private const int dispidSniffState = 34074;

		private const int dispidSpamOriginalFolder = 34204;

		private const int dispidStampedAccount = 34184;

		private const int dispidStartRecurDate = 4104;

		private const int dispidStartRecurTime = 4105;

		private const int dispidStsContentTypeId = 34238;

		private const int dispidSyncFailures = 34199;

		private const int dispidTaskAccepted = 33032;

		private const int dispidTaskActualEffort = 33040;

		private const int dispidTaskCardData = 33067;

		private const int dispidTaskComplete = 33052;

		private const int dispidTaskCustomFlags = 33081;

		private const int dispidTaskCustomPriority = 33080;

		private const int dispidTaskCustomStatus = 33079;

		private const int dispidTaskDateCompleted = 33039;

		private const int dispidTaskDeadOccur = 33033;

		private const int dispidTaskDelegator = 33057;

		private const int dispidTaskDelegValue = 33066;

		private const int dispidTaskDueDate = 33029;

		private const int dispidTaskDuration = 33030;

		private const int dispidTaskEstimatedEffort = 33041;

		private const int dispidTaskFCreator = 33054;

		private const int dispidTaskFFixOffline = 33068;

		private const int dispidTaskFormURN = 33074;

		private const int dispidTaskFRecur = 33062;

		private const int dispidTaskGlobalObjId = 34073;

		private const int dispidTaskHistory = 33050;

		private const int dispidTaskLastDelegate = 33061;

		private const int dispidTaskLastUpdate = 33045;

		private const int dispidTaskLastUser = 33058;

		private const int dispidTaskMode = 34072;

		private const int dispidTaskMultRecips = 33056;

		private const int dispidTaskMyDelegators = 33047;

		private const int dispidTaskNoCompute = 33060;

		private const int dispidTaskOrdinal = 33059;

		private const int dispidTaskOriginalRecurring = 33053;

		private const int dispidTaskOwner = 33055;

		private const int dispidTaskOwnership = 33065;

		private const int dispidTaskRecur = 33046;

		private const int dispidTaskResetReminder = 33031;

		private const int dispidTaskRole = 33063;

		private const int dispidTaskSchdPrio = 33071;

		private const int dispidTaskSOC = 33049;

		private const int dispidTaskStartDate = 33028;

		private const int dispidTaskState = 33043;

		private const int dispidTaskStatus = 33025;

		private const int dispidTaskUpdates = 33051;

		private const int dispidTaskVersion = 33042;

		private const int dispidTaskWebUrl = 33076;

		private const int dispidTeamTask = 33027;

		private const int dispidThemeDataXML = 34242;

		private const int dispidTimeZoneDesc = 33332;

		private const int dispidTimeZoneStruct = 33331;

		private const int dispidTNEFDump = 34207;

		private const int dispidToAttendeesString = 33339;

		private const int dispidToDoOrdinalDate = 34208;

		private const int dispidToDoSubOrdinal = 34209;

		private const int dispidToDoTitle = 34212;

		private const int dispidToDoTitleOM = 64543;

		private const int dispidTrustRecipHighlights = 33342;

		private const int dispidUberGroup = 34189;

		private const int dispidUnifiedMessagingOptions = 1;

		private const int dispidUnifiedTracking = 34825;

		private const int dispidUnsendableRecipients = 33373;

		private const int dispidUrl = 34109;

		private const int dispidUseInternetZone = 34185;

		private const int dispidUserApprovedLink = 33003;

		private const int dispidUserCertificateStr = 32790;

		private const int dispidUseTNEF = 34178;

		private const int dispidValidFlagStringProof = 34239;

		private const int dispidVerbResponse = 34084;

		private const int dispidVerbStream = 34080;

		private const int dispidVisibleSize = 36848;

		private const int dispidWhenProp = 34;

		private const int dispidWorkAddress = 32795;

		private const int dispidWorkAddressCity = 32838;

		private const int dispidWorkAddressCountry = 32841;

		private const int dispidWorkAddressCountryCode = 32987;

		private const int dispidWorkAddressPostalCode = 32840;

		private const int dispidWorkAddressPostOfficeBox = 32842;

		private const int dispidWorkAddressState = 32839;

		private const int dispidWorkAddressStreet = 32837;

		private const int dispidXSharingBrowseUrl = 35410;

		private const int dispidXSharingCapabilities = 35387;

		private const int dispidXSharingConfigUrl = 35381;

		private const int dispidXSharingFlavor = 35388;

		private const int dispidXSharingInstanceGuid = 35386;

		private const int dispidXSharingLocalType = 35408;

		private const int dispidXSharingProviderGuid = 35389;

		private const int dispidXSharingProviderName = 35390;

		private const int dispidXSharingProviderUrl = 35391;

		private const int dispidXSharingRemoteName = 35383;

		private const int dispidXSharingRemotePath = 35382;

		private const int dispidXSharingRemoteStoreUid = 35402;

		private const int dispidXSharingRemoteType = 35385;

		private const int dispidXSharingRemoteUid = 35384;

		private const int dispidYomiButton = 32815;

		private const int dispidYomiCompanyName = 32814;

		private const int dispidYomiFirstName = 32812;

		private const int dispidYomiLastName = 32813;

		private const int LID_ALL_ATTENDEES_LIST = 29;

		private const int LID_ATTENDEE_CRITICAL_CHANGE = 1;

		private const int LID_CALENDAR_TYPE = 28;

		private const int LID_DAY_INTERVAL = 17;

		private const int LID_DELEGATE_MAIL = 9;

		private const int LID_DOM_MASK = 22;

		private const int LID_DOW_MASK = 21;

		private const int LID_DOW_PREF = 25;

		private const int LID_END_RECUR_DATE = 15;

		private const int LID_END_RECUR_TIME = 16;

		private const int LID_GLOBAL_OBJID = 3;

		private const int LID_IS_EXCEPTION = 10;

		private const int LID_IS_RECURRING = 5;

		private const int LID_IS_SILENT = 4;

		private const int LID_MONTH_INTERVAL = 19;

		private const int LID_MOY_MASK = 23;

		private const int LID_OPTIONAL_ATTENDEES = 7;

		private const int LID_OWNER_CRITICAL_CHANGE = 26;

		private const int LID_RECUR_TYPE = 24;

		private const int LID_REQUIRED_ATTENDEES = 6;

		private const int LID_RESOURCE_ATTENDEES = 8;

		private const int LID_SINGLE_INVITE = 11;

		private const int LID_START_RECUR_DATE = 13;

		private const int LID_START_RECUR_TIME = 14;

		private const int LID_TIME_ZONE = 12;

		private const int LID_WANT_SILENT_RESP = 27;

		private const int LID_WEEK_INTERVAL = 18;

		private const int LID_WHERE = 2;

		private const int LID_YEAR_INTERVAL = 20;

		private const int PID_STG_NAME = 10;

		private const int pridDAVAcct = 41216;

		private const int pridDAVMime = 41219;

		private const int pridDAVOffLine = 41220;

		private const int pridDAVUrl = 41217;

		private const int pridIMAPFoldFlags = 40997;

		private const int pridIMAPFoldNextUID = 40994;

		private const int pridIMAPFoldPendingAppend = 40998;

		private const int pridIMAPFoldSep = 40996;

		private const int pridIMAPFoldUIDValidity = 40993;

		private const int pridIMAPMsgFlags = 41025;

		private const int pridIMAPMsgGUID = 41028;

		private const int pridIMAPMsgHeaders = 41030;

		private const int pridIMAPMsgOfflineChgs = 41029;

		private const int pridIMAPMsgState = 41027;

		private const int pridIMAPMsgUID = 41024;

		private const int pridIMAPStoreAcct = 41072;

		private const int pridIMAPStoreOfflineChgNum = 41075;

		private const int pridIMAPStoreOfflineFldrs = 41076;

		private const int pridIMAPStoreOfflineMsg = 41074;

		private const int pridIMAPStorePrefix = 41073;

		private const int pridMFLSAccociatedMessageEID = 40960;

		private const int pridObjType = 40960;

		private const int iContactAddress = 0;

		private const int iContactEmail = 1;

		private const int iContactPhone1 = 2;

		private const int iContactPhone2 = 3;

		private const int iContactPhone3 = 4;

		private const int iContactPhone4 = 5;

		private const int iContactPhone5 = 6;

		private const int iContactPhone6 = 7;

		private const int iContactPhone7 = 8;

		private const int iContactPhone8 = 9;

		private const int dispidAddressSelection = 32872;

		private const int dispidEmailSelection = 32873;

		private const int dispidPhone1Selection = 32874;

		private const int dispidPhone2Selection = 32875;

		private const int dispidPhone3Selection = 32876;

		private const int dispidPhone4Selection = 32877;

		private const int dispidPhone5Selection = 32878;

		private const int dispidPhone6Selection = 32879;

		private const int dispidPhone7Selection = 32880;

		private const int dispidPhone8Selection = 32881;

		private const int dispidSelectedAddress = 32884;

		private const int dispidSelectedPhone1 = 32886;

		private const int dispidSelectedPhone2 = 32887;

		private const int dispidSelectedPhone3 = 32888;

		private const int dispidSelectedPhone4 = 32889;

		private const int dispidSelectedPhone5 = 32890;

		private const int dispidSelectedPhone6 = 32891;

		private const int dispidSelectedPhone7 = 32892;

		private const int dispidSelectedPhone8 = 32893;

		private const int dispidViewStartTime = 33;

		private const int dispidViewEndTime = 34;

		private const int dispidCalendarFolderVersion = 35;

		private const int dispidCharmId = 37;

		private static readonly NamedProp[] namedPropList = new NamedProp[]
		{
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, "InstanceCreationIndex"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, "EventClientId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, "SeriesId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, "SeriesReminderIsSet"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, "SeriesCreationHash"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, "SeriesMasterId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "DEPRECATED_IsGroupEscalationMessage"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, "OccurrencesExceptionalViewProperties"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, "SeriesSequenceNumber"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, "PropertyChangeMetadataRaw"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, "IsHiddenFromLegacyClients"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, "CalendarInteropActionQueueInternal"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, "CalendarInteropActionQueueHasDataInternal"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, "LastExecutedCalendarInteropAction"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, "PropertyChangeMetadataProcessingFlags"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, "ParkedMessagesFolderEntryId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, "MasterGlobalObjectId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, "ParkedCorrelationId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "MailboxAssociationFolderEntryId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "MailboxAssociationExternalId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "MailboxAssociationLegacyDN"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "MailboxAssociationIsMember"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "MailboxAssociationSmtpAddress"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "MailboxAssociationJoinedBy"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "MailboxAssociationIsPin"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "MailboxAssociationShouldEscalate"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "MailboxAssociationIsAutoSubscribed"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "MailboxAssociationJoinDate"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "MailboxAssociationLastVisitedDate"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "MailboxAssociationPinDate"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "MailboxAssociationCurrentVersion"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "MailboxAssociationSyncedVersion"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "MailboxAssociationLastSyncError"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "MailboxAssociationSyncAttempts"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "MailboxAssociationSyncedSchemaVersion"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "MailboxAssociationSyncedIdentityHash"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "HierarchySyncLastAttemptedSyncTime"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "HierarchySyncLastFailedSyncTime"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "HierarchySyncLastSuccessfulSyncTime"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "HierarchySyncFirstFailedSyncTimeAfterLastSuccess"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "HierarchySyncLastSyncFailure"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "HierarchySyncNumberOfAttemptsAfterLastSuccess"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "HierarchySyncNumberOfBatchesExecuted"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "HierarchySyncNumberOfFoldersSynced"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "HierarchySyncNumberOfFoldersToBeSynced"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "HierarchySyncBatchSize"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, "BirthdayContactAttributionDisplayName"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, "BirthdayContactEntryId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, "IsBirthdayContactWritable"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "ExchangeApplicationFlags"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "OfficeGraphLocation"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "DlpSenderOverride"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "DlpFalsePositive"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "DlpDetectedClassifications"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "DlpDetectedClassificationObjects"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "HasDlpDetectedClassifications"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "RecoveryOptions"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Compliance, "NeedGroupExpansion"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Reminders, "EventTimeBasedInboxReminders"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Reminders, "QuickCaptureReminders"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Reminders, "EventTimeBasedInboxRemindersState"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Reminders, "ModernReminders"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Reminders, "ModernRemindersState"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Reminders, "HasExceptionalInboxReminders"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Reminders, "ReminderId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Reminders, "ReminderItemGlobalObjectId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Reminders, "ReminderOccurrenceGlobalObjectId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Reminders, "ScheduledReminderTime"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Reminders, "ReminderText"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Reminders, "ReminderStartTime"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Reminders, "ReminderEndTime"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "OriginalSentTimeForEscalation"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Inference, "InferenceProcessingNeeded"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Inference, "InferenceProcessingActions"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.WorkingSet, "WorkingSetId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.WorkingSet, "WorkingSetSource"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.WorkingSet, "WorkingSetSourcePartition"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.WorkingSet, "WorkingSetSourcePartitionInternal"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.WorkingSet, "WorkingSetFlags"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.WorkingSet, "WorkingSetFolderEntryId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Inference, "InferenceActionTruth"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Inference, "InferenceNeverClutterOverrideApplied"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Inference, "InferenceUniqueActionLabelData"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "ItemMovedByRule"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "ItemMovedByConversationAction"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "ItemSenderClass"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "ItemCurrentFolderReason"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, "DoItTime"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "BirthdayCalendarFolderEntryId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.ConsumerCalendar, "ConsumerCalendarGuid"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.ConsumerCalendar, "ConsumerCalendarOwnerId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.ConsumerCalendar, "ConsumerCalendarPrivateFreeBusyId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.ConsumerCalendar, "ConsumerCalendarPrivateDetailId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.ConsumerCalendar, "ConsumerCalendarPublishVisibility"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.ConsumerCalendar, "ConsumerCalendarSharingInvitations"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.ConsumerCalendar, "ConsumerCalendarPermissionLevel"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.ConsumerCalendar, "ConsumerCalendarSynchronizationState"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 0),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 1),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 2),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 3),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 4),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 5),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 6),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 7),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 8),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 9),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 10),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 11),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 12),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 13),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 14),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 15),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 16),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 17),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 18),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 19),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 20),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 21),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 22),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 23),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 24),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 25),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 26),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 27),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 28),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 29),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 30),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 31),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 32),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 33),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 34),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 35),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 36),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 37),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 38),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 39),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 40),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 41),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 42),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 43),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 44),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 45),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 46),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 47),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 48),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 49),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 50),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PICW, 51),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.GroupNotifications, "GroupNotificationsFolderEntryId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 33008),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 33010),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, "MyContactsFolderEntryId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, "MyContactsExtendedFolderEntryId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, "MyContactsFolders"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, "PeopleIKnowEmailAddressRelevanceScoreCollection"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, "SenderRelevanceScore"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "PeopleHubSortGroupPriority"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "PeopleHubSortGroupPriorityVersion"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "IsPeopleConnectSyncFolder"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "DoNotDeliver"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "DropMessageInHub"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "MapiSubmitLamNotificationId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "MapiSubmitSystemProbeActivityId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "TemporarySavesFolderEntryId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "SnackyAppsFolderEntryId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34145),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34245),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34246),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34247),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34248),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34249),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34250),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34251),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34254),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32868),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 34251),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33382),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33383),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 34251),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, "OnlineMeetingExternalLink"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, 3),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, 34251),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, "CalendarVersionStoreEntryId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Location, "LocationDisplayName"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Location, "LocationAnnotation"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Location, "LocationType"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Location, "LocationSource"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Location, "LocationUri"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Location, "Latitude"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Location, "Longitude"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Location, "Accuracy"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Location, "Altitude"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Location, "AltitudeAccuracy"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Location, "StreetAddress"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, 33383),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, 34251),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "TelURI"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "OnlineMeetingInternalLink"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "OnlineMeetingExternalLink"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "OnlineMeetingConfLink"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DRMRights"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DRMExpiryTime"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DRMPropsSignature"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DRMServerLicense"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DRMServerLicenseCompressed"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/fldfltr"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/fltract"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/fltrflg"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/fltrfrm"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/fltrsrcfldr"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/fltrto"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/signedoutofim"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/treenodecollapsestatus"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "x-requireprotectedplayonphone"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Elc, "AdminAuditLogsFolderEntryId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Elc, "RecoverableItemsDeletionsEntryId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Elc, "RecoverableItemsPurgesEntryId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Elc, "RecoverableItemsVersionsEntryId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.ExternalSharing, "PermissionChangeBlocked"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "TextMessaging:DeliveryStatus"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "LegacyArchiveJournalsFolderEntryId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, "OWA-NavigationNodeCalendarTypeFromOlderExchange"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.UnifiedMessaging, "PstnCallbackTelephoneNumber"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.UnifiedMessaging, "X-MS-Exchange-UM-PartnerContent"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.UnifiedMessaging, "X-MS-Exchange-UM-PartnerStatus"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.UnifiedMessaging, "IsVoiceReminderEnabled"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.UnifiedMessaging, "VoiceReminderPhoneNumber"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.IConverterSession, "Internet Charset Body"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.IConverterSession, "Partial Message Id"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.IConverterSession, "Partial Message Number"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.IConverterSession, "Partial Message Total"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmCharacterization, 2),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmDocSummaryInformation, 2),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmDocSummaryInformation, 3),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmDocSummaryInformation, 4),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmDocSummaryInformation, 5),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmDocSummaryInformation, 6),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmDocSummaryInformation, 7),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmDocSummaryInformation, 8),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmDocSummaryInformation, 9),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmDocSummaryInformation, 10),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmDocSummaryInformation, 11),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmDocSummaryInformation, 12),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmDocSummaryInformation, 13),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmDocSummaryInformation, 14),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmDocSummaryInformation, 15),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmDocSummaryInformation, 16),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmDocSummaryInformation, 17),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmGatherer, 4),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmHTMLInformation, "A.HREF"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmHTMLInformation, "APPLET.CODE"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmHTMLInformation, "APPLET.CODEBASE"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmHTMLInformation, "AREA.HREF"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmHTMLInformation, "BASE.HREF"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmHTMLInformation, "BGSOUND.SRC"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmHTMLInformation, "BODY.BACKGROUND"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmHTMLInformation, "EMBED.SRC"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmHTMLInformation, "FRAME.SRC"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmHTMLInformation, "IFRAME.SRC"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmHTMLInformation, "IMG.ALT"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmHTMLInformation, "IMG.DYNSRC"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmHTMLInformation, "IMG.SRC"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmHTMLInformation, "IMG.USEMAP"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmHTMLInformation, "LINK.HREF"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmHTMLInformation, "META.URL"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmHTMLInformation, "OBJECT.CODEBASE"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmHTMLInformation, "OBJECT.NAME"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmHTMLInformation, "OBJECT.USEMAP"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmHTMLInformation, 3),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmHTMLInformation, 4),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmHTMLInformation, 5),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmHTMLInformation, 6),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmHTMLInformation, 7),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmHTMLInformation, 8),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmIndexServerQuery, 10),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmIndexServerQuery, 11),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmIndexServerQuery, 2),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmIndexServerQuery, 4),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmIndexServerQuery, 5),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmIndexServerQuery, 6),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmIndexServerQuery, 7),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmIndexServerQuery, 9),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmLinkInformation, "A.HREF"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmLinkInformation, "APPLET.CODE"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmLinkInformation, "APPLET.CODEBASE"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmLinkInformation, "AREA.HREF"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmLinkInformation, "BASE.HREF"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmLinkInformation, "BGSOUND.SRC"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmLinkInformation, "BODY.BACKGROUND"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmLinkInformation, "EMBED.SRC"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmLinkInformation, "FRAME.SRC"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmLinkInformation, "IFRAME.SRC"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmLinkInformation, "IMG.DYNSRC"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmLinkInformation, "IMG.SRC"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmLinkInformation, "IMG.USEMAP"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmLinkInformation, "LINK.HREF"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmLinkInformation, "META.URL"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmLinkInformation, "OBJECT.CODEBASE"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmLinkInformation, "OBJECT.NAME"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmLinkInformation, "OBJECT.USEMAP"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmMetaInformation, "Product"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmMetaInformation, "Topic"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmNetLibraryInfo, 3),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmScriptInfo, "JavaScript"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmScriptInfo, "VBScript"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Search, 10),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Search, 11),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Search, 20),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Search, 4),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Search, 5),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Search, 6),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Search, 7),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmSummaryInformation, 2),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmSummaryInformation, 3),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmSummaryInformation, 4),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmSummaryInformation, 5),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmSummaryInformation, 6),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmSummaryInformation, 7),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmSummaryInformation, 8),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmSummaryInformation, 9),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmSummaryInformation, 10),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmSummaryInformation, 11),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmSummaryInformation, 12),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmSummaryInformation, 13),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmSummaryInformation, 14),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmSummaryInformation, 15),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmSummaryInformation, 16),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmSummaryInformation, 17),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmSummaryInformation, 18),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PkmSummaryInformation, 19),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "AcceptLanguage"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Approved"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Bcc"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Cc"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Comment"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Content-Base"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Content-Disposition"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Content-ID"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Content-Language"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Content-Location"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Content-Transfer-Encoding"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Content-Type"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Control"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Disposition-Notification-Options"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Disposition-Notification-To"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Disposition"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Distribution"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Expires"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Expiry-Date"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Followup-To"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "From"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Importance"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "In-Reply-To"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Keywords"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Message-ID"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Mime-Version"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Newsgroups"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "NNTP-Posting-Host"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "NNTP-Posting-User"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Organization"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Original-Recipient"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Path"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Posting-Version"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Priority"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Received"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "References"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Relay-Version"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Reply-By"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Reply-To"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Return-Path"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Return-Receipt-To"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Sender"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Sensitivity"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Subject"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Summary"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Thread-Index"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Thread-Topic"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "To"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "x-attachmentorder"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "x-callid"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "x-callingtelephonenumber"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "x-faxnumberofpages"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-Mailer"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-Message-Completed"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-Message-Flag"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-MS-Exchange-ImapAppendStamp"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-MS-Exchange-Organization-AuthAs"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-MS-Exchange-Organization-AuthDomain"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-MS-Exchange-Organization-AuthMechanism"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-MS-Exchange-Organization-AuthSource"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-MS-Journal-Report"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-MS-TNEF-Correlator"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-Sharing-Browse-Url"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-Sharing-Capabilities"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-Sharing-Config-Url"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-Sharing-Exended-Caps"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-Sharing-Flavor"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-Sharing-Instance-Guid"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-Sharing-Local-Type"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-Sharing-Provider-Guid"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-Sharing-Provider-Name"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-Sharing-Provider-Url"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-Sharing-Remote-Name"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-Sharing-Remote-Path"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-Sharing-Remote-Store-Uid"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-Sharing-Remote-Type"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-Sharing-Remote-Uid"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-Unsent"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "x-voicemessageduration"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "x-voicemessagesendername"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Xref"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, 36864),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, 36865),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, 9000),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, 9001),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, 32993),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, 32999),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "% Complete"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "% Work Complete"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "AddLevel1"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "AddLevel2"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "AllowOleAct"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "AllowOlePack"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "AllowOneOffs"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "AllowUserAttachSetting"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "AllUsers"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "AppName"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "AttachClosePrompt"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "AttachSendPrompt"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "Author"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "ByteCount"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "Category"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "CharCount"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "Comments"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "Company"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "Cost"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "CreateDtmRo"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DAV:abstract"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DAV:childcount"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DAV:displayname"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DAV:getcontentlanguage"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DAV:getcontentlength"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DAV:getcontenttype"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DAV:getetag"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DAV:haschildren"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DAV:id"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DAV:iscollection"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DAV:isroot"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DAV:isstructureddocument"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DAV:lockdiscovery"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DAV:nosubs"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DAV:objectcount"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DAV:parentname"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DAV:resourcetype"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DAV:searchrequest"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DAV:searchtype"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DAV:supportedlock"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DAV:uid"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DocParts"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "doformlookup"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "DRMLicense"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "Duration"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "EditTime"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "Finish"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "FormControlProp"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "FormVersion"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "HeadingPair"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "HeadingsPair"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "HiddenCount"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/age/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/astrologysign/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/bloodtype/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/custom1/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/custom2/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/custom3/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/custom4/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/custom5/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/custom6/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/custom7/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/custom8/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/customdate1/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/customdate2/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/customphone1/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/customphone2/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/customphone3/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/customphone4/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/email10originaldisplayname/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/email11originaldisplayname/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/email12originaldisplayname/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/email13originaldisplayname/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/email4originaldisplayname/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/email5originaldisplayname/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/email6originaldisplayname/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/email7originaldisplayname/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/email8originaldisplayname/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/email9originaldisplayname/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/emaillabel1/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/emaillabel10/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/emaillabel11/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/emaillabel12/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/emaillabel13/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/emaillabel2/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/emaillabel3/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/emaillabel4/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/emaillabel5/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/emaillabel6/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/emaillabel7/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/emaillabel8/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/emaillabel9/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/flagged/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/japanesecontact/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/japaneseformat/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/entourage/spousefurigana/"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/ablumultiline"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/ablupreview"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/abpkmultiline"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/anrcontactsfirst"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/archive"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/autoaddsignature"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/baseitemuri"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/blockexternalcontent"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/calviewtype"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/composefontcolor"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/composefontflags"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/composefontname"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/composefontsize"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/content-href"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/dailyviewdays"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/deliverandredirect"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/enablereminders"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/fboldbusystatus"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/fboldend"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/fboldstart"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/fbqueryend"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/fbqueryinterval"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/fbquerystart"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/fbquerystring"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/fbrecurisdirty"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/firstweekofyear"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/forwardingaddress"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/intendedbusystatus"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/junkemailmovestamp"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/longdateformat"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/mailbox-owner-entryid"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/mailbox-owner-name"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/modifyexceptionstruct"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/msexchembaccepteddevices"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/msexchembcultureinfo"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/msexchembdateformat"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/msexchembdefaultmailfolder"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/msexchembdefaultmailfoldertype"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/msexchembmarkread"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/msexchembtimeformat"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/msexchembtimezone"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/newmailnotify"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/nextsel"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/nomodifyexceptions"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/oof-state"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/otherproxyaddresses"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/patternend"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/patternstart"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/preview"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/previewmarkasread"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/previewMultiDay"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/prevreaddelaytime"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/publicfolderemailaddress"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/quicklinks"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/readreceipt"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/reminderinterval"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/rootfid"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/runat"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/schemaversion"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/searchfolder"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/shortdateformat"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/showrulepont"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/signaturehtml"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/signaturetext"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/smallicon"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/smimeencrypt"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/smimesign"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/spellingcheckbeforesend"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/spellingdictionarylanguage"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/spellingignoremixeddigits"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/spellingignoreuppercase"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/themeid"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/timeformat"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/timezone"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/user-entryid"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/viewrowcount"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/vwflt"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/wcmultiline"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/wcsortcolumn"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/wcsortorder"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/wcviewheight"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/wcviewwidth"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/webclientlastusedsortcols"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/webclientlastusedview"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/webclientnavbarwidth"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/webclientshowhierarchy"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/weekstartday"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/workdayendtime"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/workdays"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/exchange/workdaystarttime"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/outlook/phishingstamp"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/outlook/spoofingstamp"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/repl/repl-uid"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "http://schemas.microsoft.com/repl/resourcetag"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "journal-remote-accounts"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "Keywords"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "LastAuthor"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "LastPrinted"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "LastSaveDtm"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "LineCount"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "LinksDirty"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "MAIL:submissionuri"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "Manager"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "MMClipCount"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "NoteCount"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "NumElements"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "PageCount"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "ParCount"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "PresFormat"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "ProgAddrBookCDO"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "ProgAddrBookOOM"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "ProgAddrBookSMAPI"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "ProgCustVerb"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "ProgRespond"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "ProgSaveAs"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "ProgSearch"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "ProgSendCDO"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "ProgSendOOM"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "ProgSendSMAPI"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "quarantine-original-sender"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "RemoveLevel1"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "RemoveLevel2"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "RevNumber"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "Scale"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "Security"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "ShowAllAttach"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "SlideCount"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "Start"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "STS_AttachIDTable"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "STS_BaseURL"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "STS_EventType"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "STS_EventUID"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "STS_FooterInfo"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "STS_HeaderInfo"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "STS_HiddenVer"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "STS_Id"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "STS_IDTable"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "STS_LastSync"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "STS_ListFriendlyName"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "STS_ListGUID"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "STS_ListURL"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "STS_RecurrenceID"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "STS_RecurXml"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "STS_SharePointFolder"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "STS_SiteName"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "STS_TimeStamp"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "STS_TimeStamp2"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "STS_UserId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "Subject"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "Template"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "ThumbNail"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "Title"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "TrustedCode"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:datatypes#type"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:exch-data:baseschema"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:exch-data:closedexpectedcontentclasses"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:exch-data:codebase"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:exch-data:comclassid"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:exch-data:comprogid"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:exch-data:default"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:exch-data:dictionary"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:exch-data:expected-content-class"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:exch-data:form"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:exch-data:iscontentindexed"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:exch-data:isindexed"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:exch-data:ismultivalued"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:exch-data:isreadonly"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:exch-data:isrequired"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:exch-data:isvisible"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:exch-data:propertydef"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:exch-data:schema-collection-ref"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:exch-data:schema-uri"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:exch-data:synchronize"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:exch-data:version"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:office:designer#Membership"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:office:designer#templatedescription"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:office:office#doformlookup"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:office:outlook#allornonemtgupdatedlg"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:office:outlook#docoercion"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:office:outlook#fixupfbfolder"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:office:outlook#hidefiltertab"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:office:outlook#ishidden"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:office:outlook#noaclui"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:office:outlook#noduplicatedocuments"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:office:outlook#serverfoldersizes"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:office:outlook#showforward"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:office:outlook#showreply"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:office:outlook#storetypeprivate"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:publishing:BaseDoc"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:publishing:LastApprovedVersion"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:publishing:LastUnapprovedVersion"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:publishing:MainFolder"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:publishing:WorkingCopy"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:xml-data#element"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:xml-data#extends"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas-microsoft-com:xml-data#name"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:attendeerole"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:busystatus"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:contact"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:contacturl"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:created"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:descriptionurl"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:duration"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:exdate"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:exrule"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:geolatitude"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:geolongitude"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:instancetype"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:isorganizer"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:lastmodified"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:locationurl"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:meetingstatus"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:method"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:prodid"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:rdate"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:recurrenceidrange"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:reminderoffset"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:resources"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:rsvp"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:sequence"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:timezone"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:timezoneid"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:transparent"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:uid"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:calendar:version"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:contacts:alternaterecipient"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:contacts:c"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:contacts:email1"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:contacts:email2"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:contacts:email3"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:contacts:fileas"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:contacts:fileasid"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:contacts:homelatitude"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:contacts:homelongitude"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:contacts:hometimezone"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:contacts:mapurl"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:contacts:othercountrycode"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:contacts:otherpager"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:contacts:othertimezone"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:contacts:proxyaddresses"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:contacts:rdate"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:contacts:rrule"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:contacts:secretaryurl"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:contacts:sourceurl"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:bcc"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:calendar"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:cc"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:contacts"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:content-disposition-type"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:content-media-type"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:deleteditems"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:drafts"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:from"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:fromemail"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:htmldescription"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:inbox"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:journal"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:junkemail"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:msgfolderroot"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:notes"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:outbox"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:reply-to"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:savedestination"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:saveinsent"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:sender"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:senderemail"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:sendmsg"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:sentitems"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:special"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:submitted"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:tasks"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "urn:schemas:httpmail:to"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "UserFormula"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "WordCount"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PublicStrings, "Work"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32773),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32774),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32794),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32795),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32796),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32802),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32808),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32837),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32838),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32839),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32840),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32841),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32842),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32898),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32899),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32900),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32901),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32914),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32915),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32916),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32917),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32930),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32931),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32932),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32933),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, "ImContactSipUriAddress"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, "LinkChangeHistory"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32809),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32989),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32872),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32846),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32805),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32833),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32832),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32845),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32780),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32792),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32818),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32819),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32863),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32856),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32855),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32857),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32864),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32854),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32862),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32803),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32861),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32775),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32779),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32847),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32848),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32849),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32850),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32784),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32844),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32843),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32853),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32851),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32852),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32810),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32896),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32903),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32897),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32902),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32912),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32919),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32913),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32918),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32928),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32935),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32929),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32934),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32807),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32873),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32985),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32946),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32944),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32947),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32951),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32945),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32948),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32949),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32950),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32962),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32960),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32963),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32967),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32961),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32964),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32965),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32966),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32978),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32976),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32979),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32983),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32977),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32980),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32981),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32982),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32806),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32823),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32770),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32984),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32769),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32793),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32789),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32986),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32811),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32866),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32834),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32835),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32836),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32822),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32816),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32824),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32820),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32817),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32821),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32791),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32865),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32859),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32860),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 33353),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 33350),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 33345),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32867),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32988),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32874),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32875),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32876),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32877),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32878),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32879),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32880),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32881),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32782),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32884),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32776),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32777),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32778),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32886),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32887),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32888),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32889),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32890),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32891),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32892),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32893),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 33003),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32790),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32987),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32814),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32812),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Address, 32813),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.AirSync, "AirSync:AirSyncClientCategoryList"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.AirSync, "AirSync:AirSyncConversationMode"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.AirSync, "AirSync:AirSyncDeletedCountTotal"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.AirSync, "AirSync:AirSyncFilter"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.AirSync, "AirSync:AirSyncLastPingHeartbeatInterval"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.AirSync, "AirSync:AirSyncLastSeenClientIds"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.AirSync, "AirSync:AirSyncLastSyncAttemptTime"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.AirSync, "AirSync:AirSyncLastSyncDay"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.AirSync, "AirSync:AirSyncLastSyncSuccessTime"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.AirSync, "AirSync:AirSyncLastSyncTime"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.AirSync, "AirSync:AirSyncLastSyncUserAgent"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.AirSync, "AirSync:AirSyncLocalCommitTimeMax"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.AirSync, "AirSync:AirSyncMaxItems"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.AirSync, "AirSync:AirSyncSettingsHash"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.AirSync, "AirSync:AirSyncStoreObectIdProperty"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.AirSync, "AirSync:AirSyncSynckey"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.AirSync, "AirSync:DeviceBlockedUntil"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.AirSync, "AirSync:DeviceBlockedAt"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.AirSync, "AirSync:DeviceBlockedReason"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.AirSync, "AirSync:IMAddress2"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.AirSync, "AirSync:IMAddress3"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.AirSync, "AirSync:MMS"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33285),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33288),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33293),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33294),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33295),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33296),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33297),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33298),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33299),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33302),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33303),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33315),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33316),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33321),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33329),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33330),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33331),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33332),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33333),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33334),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33336),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33350),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33287),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33300),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33367),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33283),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33370),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33371),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33369),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33366),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33365),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33363),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33361),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33364),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33362),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33360),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33328),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33312),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33282),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33281),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33301),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33375),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33376),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33374),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33341),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33318),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33338),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33348),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33349),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33340),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33284),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33351),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33344),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33343),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33345),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33317),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33346),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33319),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33320),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33325),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33324),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33323),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33286),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33377),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33327),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33290),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33292),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33289),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33352),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33353),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33347),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33322),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33335),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33326),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33304),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33280),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33339),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33342),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Appointment, 33373),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Attachment, "AttachmentMacContentType"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Attachment, "AttachmentMacInfo"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Attachment, "OriginalMimeReadTime"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, 6),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, 7),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, 10),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, 11),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, 12),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, 13),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, 14),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, 15),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, 16),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, 17),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, 18),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, 19),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, 20),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, 33),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, 34),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, 37),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, 35),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, 2),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.CalendarAssistant, 1),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "ArchiveSourceSupportMask"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "CrawlSourceSupportMask"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34050),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34051),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34064),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34070),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34071),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34144),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34062),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 61624),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 33325),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 33324),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34227),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 33305),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34122),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34216),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34101),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34231),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34232),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34230),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34229),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34234),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34243),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34105),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34107),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34197),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34181),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34182),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34180),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34106),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34108),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34200),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34236),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34130),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34132),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34114),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34069),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 4096),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 4097),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34183),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34205),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34240),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 4107),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34075),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34063),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34186),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34213),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 4106),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34168),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34110),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34160),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34195),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34176),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34177),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34225),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 4099),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 4100),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34207),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34203),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34179),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34226),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34161),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34100),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34128),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34129),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 4102),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34104),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34103),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34102),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34117),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34116),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34115),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 4101),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34233),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34067),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34054),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34112),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34121),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 4109),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34237),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34049),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34079),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34076),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34078),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34053),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34052),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34077),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34065),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34096),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34113),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34241),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34192),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34188),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34193),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34202),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34190),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34191),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34198),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34068),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34074),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34204),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34184),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34238),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34199),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34073),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34072),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34242),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34208),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34209),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34212),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 64543),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34189),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34185),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34178),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34239),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34084),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 34080),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, 36848),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "RecipientCacheFolderEntryId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "ActivityAttachmentIdBytes"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "ActivityGlobalObjectIdBytes"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Common, "ActivityModuleSelected"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Conversations, "ConversationIndexTracking"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Conversations, "ConversationIndexTrackingEx"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.DAV, 41216),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.DAV, 41217),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.DAV, 41219),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.DAV, 41220),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Elc, "CalendarLoggingEntryId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Elc, "DumpsterEntryId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Elc, "ElcFolderLocalizedName"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.ExternalSharing, "ExternalSharingDataType"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.ExternalSharing, "ExternalSharingIsPrimary"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.ExternalSharing, "ExternalSharingLastSuccessfulSyncTime"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.ExternalSharing, "ExternalSharingLevelOfDetails"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.ExternalSharing, "ExternalSharingLocalFolderId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.ExternalSharing, "ExternalSharingMasterId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.ExternalSharing, "ExternalSharingRemoteFolderId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.ExternalSharing, "ExternalSharingRemoteFolderName"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.ExternalSharing, "ExternalSharingSharerIdentity"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.ExternalSharing, "ExternalSharingSharerIdentityFederationUri"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.ExternalSharing, "ExternalSharingSharerName"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.ExternalSharing, "ExternalSharingSharingKey"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.ExternalSharing, "ExternalSharingSubscriberIdentity"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.ExternalSharing, "ExternalSharingSyncState"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.ExternalSharing, "ExternalSharingUrl"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.IMAPFold, 40997),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.IMAPFold, 40994),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.IMAPFold, 40998),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.IMAPFold, 40996),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.IMAPFold, 40993),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.IMAPMsg, "PopImap:ImapMIMEOptions"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.IMAPMsg, "PopImap:ImapMIMESize"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.IMAPMsg, "PopImap:PopMIMEOptions"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.IMAPMsg, "PopImap:PopMIMESize"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.IMAPMsg, 41025),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.IMAPMsg, 41028),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.IMAPMsg, 41030),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.IMAPMsg, 41029),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.IMAPMsg, 41027),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.IMAPMsg, 41024),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.IMAPStore, 41072),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.IMAPStore, 41075),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.IMAPStore, 41076),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.IMAPStore, 41074),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.IMAPStore, 41073),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InboxFolderLazyStream, 40960),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "content-class"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "Lines"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-MS-Exchange-Organization-Approval-Allowed-Decision-Makers"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-MS-Exchange-Organization-Approval-Requestor"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "X-MS-Exchange-Organization-RightsProtectMessage"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.InternetHeaders, "x-ms-has-attach"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Log, 34573),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Log, 34577),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Log, 34574),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Log, 34576),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Log, 34575),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Log, 34567),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Log, 34568),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Log, 34572),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Log, 34566),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Log, 34564),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Log, 34565),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Log, 34560),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Log, 34578),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 1),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 2),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 3),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 5),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 10),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 11),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 12),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 13),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 14),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 15),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 16),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 17),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 18),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 19),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 20),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 21),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 22),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 23),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 24),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 25),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 35),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 32867),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 33345),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 33350),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 33353),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 36),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 38),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 40),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 39),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 42),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 41),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 37),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 33),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 34),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 29),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 28),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 9),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 4),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 7),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 26),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 6),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 8),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Meeting, 27),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "AggregationSyncProgress"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "ApprovalApplicationData"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "ApprovalApplicationId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "ApprovalDecision"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "ApprovalDecisionMaker"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "ApprovalDecisionMakersNdred"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "ApprovalDecisionTime"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "ApprovalRequestMessageId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "ApprovalStatus"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "CloudId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "CloudVersion"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "MessageBccMe"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "MessageHistoryOriginalInternetMessageId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "MessageHistoryOriginalRecipientList"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "MessageHistoryOriginalReportDestination"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "MessageHistoryOriginalSender"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "MessageHistoryOriginalSentTime"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "OriginalScl"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Messaging, "Likers"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Note, 35584),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Note, 35587),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Note, 35585),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Note, 35586),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Note, 35588),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Note, 35589),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PostRss, 35076),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PostRss, 35072),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PostRss, 35075),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PostRss, 35074),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PostRss, 35073),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PostRss, 35077),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.PostRss, 35078),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Proxy, 40960),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Remote, 36615),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Remote, 36609),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Remote, 36610),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Remote, 36614),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Remote, 36613),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Remote, 36612),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Remote, 36611),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Report, 36096),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, "SharingEwsUri"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, "SharingImapPathPrefix"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, "SharingRemoteExchangeVersion"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, "SharingRemoteUserDomain"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, "SharingSubscriptionVersion"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, "SharingSendAsState"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, "SharingSendAsSubmissionUrl"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, "SharingSendAsValidatedEmail"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, "SharingSubscriptionCreationType"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, "SharingSubscriptionSyncPhase"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, "SharingSendAsVerificationEmailState"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, "SharingSendAsVerificationMessageId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, "SharingSendAsVerificationTimestamp"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, "SharingSubscriptionEvents"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, "SharingSubscriptionExclusionFolders"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, "SharingSubscriptionItemsSynced"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, "SharingSubscriptionItemsSkipped"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, "SharingSubscriptionTotalItemsInSourceMailbox"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, "SharingSubscriptionTotalSizeOfSourceMailbox"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, "SharingLastSyncNowRequest"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35427),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35428),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35586),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35494),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35353),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35373),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35409),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35351),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35364),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35397),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35396),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35371),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35488),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35530),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35531),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35490),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35361),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35347),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35338),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35352),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35349),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35374),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35337),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35335),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35336),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35356),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35491),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35413),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35489),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35492),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35359),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35405),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35430),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35363),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35343),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35342),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35401),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35348),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35344),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35526),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35527),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35524),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35525),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35523),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35528),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35529),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35521),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35520),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35369),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35420),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35358),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35355),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35339),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35329),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35330),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35331),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35399),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35398),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35354),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35403),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35375),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35404),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35429),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35362),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35407),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35333),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35341),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35332),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35400),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35357),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35334),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35340),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35419),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35368),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35367),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35406),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35360),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35422),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35585),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35365),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35328),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35366),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35584),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35587),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35840),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35424),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35370),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35522),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35493),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35372),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35414),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35504),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35506),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35507),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35505),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35394),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35393),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35392),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35395),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35426),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35418),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35421),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35377),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35416),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35425),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35415),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35378),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35417),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35423),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35379),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Sharing, 35376),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33026),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33027),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33028),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33031),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33033),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33039),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33040),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33041),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33042),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33043),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33045),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33046),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33050),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33052),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33055),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33056),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33057),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33058),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33059),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33060),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33062),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33063),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33065),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33066),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33068),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33032),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33067),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33081),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33080),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33079),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33029),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33054),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33074),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33061),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33047),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33053),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33071),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33049),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33025),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33051),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Task, 33076),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Tracking, 34824),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Tracking, 34825),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.UnifiedContactStore, "ImContactListFolderEntryId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.UnifiedContactStore, "QuickContactsFolderEntryId"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.UnifiedMessaging, "UMAudioNotes"),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.UnifiedMessaging, 2),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.UnifiedMessaging, 1),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Storage, 10),
			new WellKnownNamedProperties.KnownNamedProp(WellKnownNamedPropertyGuid.Storage, 9)
		};

		public static Dictionary<NamedProp, NamedProp> Properties = WellKnownNamedProperties.GenerateTable();

		private class KnownNamedProp : NamedProp
		{
			internal KnownNamedProp(Guid guid, string name) : base(guid, name)
			{
			}

			internal KnownNamedProp(Guid guid, int id) : base(guid, id)
			{
			}

			public override bool IsSingleInstanced
			{
				get
				{
					return true;
				}
			}
		}
	}
}
