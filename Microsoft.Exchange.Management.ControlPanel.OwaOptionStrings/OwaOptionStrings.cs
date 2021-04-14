using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class OwaOptionStrings
	{
		static OwaOptionStrings()
		{
			OwaOptionStrings.stringIDs.Add(4145884488U, "NeverSyncText");
			OwaOptionStrings.stringIDs.Add(1481793251U, "FromAddressContainsConditionFormat");
			OwaOptionStrings.stringIDs.Add(2278445393U, "CalendarPublishingBasic");
			OwaOptionStrings.stringIDs.Add(1471007325U, "ChangePhoneNumber");
			OwaOptionStrings.stringIDs.Add(566587615U, "TimeZoneNote");
			OwaOptionStrings.stringIDs.Add(2785077264U, "ShowWorkWeekAsCheckBoxText");
			OwaOptionStrings.stringIDs.Add(85271849U, "ViewInboxRule");
			OwaOptionStrings.stringIDs.Add(3270571164U, "DeviceMobileOperatorLabel");
			OwaOptionStrings.stringIDs.Add(62320074U, "FinishButtonText");
			OwaOptionStrings.stringIDs.Add(3179829084U, "UserNameMOSIDLabel");
			OwaOptionStrings.stringIDs.Add(3449770263U, "ChangeMyMobilePhoneSettings");
			OwaOptionStrings.stringIDs.Add(2657242021U, "RequirementsReadWriteMailboxDescription");
			OwaOptionStrings.stringIDs.Add(1654895332U, "MessageTypeMatchesConditionFormat");
			OwaOptionStrings.stringIDs.Add(226228553U, "NewRoomCreationWarningText");
			OwaOptionStrings.stringIDs.Add(932151145U, "InboxRuleMyNameInToBoxConditionFlyOutText");
			OwaOptionStrings.stringIDs.Add(2712198432U, "FirstSyncOnLabel");
			OwaOptionStrings.stringIDs.Add(2830841285U, "DeleteGroupConfirmation");
			OwaOptionStrings.stringIDs.Add(2230814600U, "PendingWipeCommandIssuedLabel");
			OwaOptionStrings.stringIDs.Add(3090796481U, "OwnerChangedModerationReminder");
			OwaOptionStrings.stringIDs.Add(495329060U, "InboxRuleFromConditionPreCannedText");
			OwaOptionStrings.stringIDs.Add(3366412874U, "MaximumConflictInstances");
			OwaOptionStrings.stringIDs.Add(2502183272U, "EmailSubscriptions");
			OwaOptionStrings.stringIDs.Add(2174801140U, "InboxRuleFlaggedForActionConditionFlyOutText");
			OwaOptionStrings.stringIDs.Add(3159390523U, "ViewRPTDurationLabel");
			OwaOptionStrings.stringIDs.Add(926911138U, "OnOffColumn");
			OwaOptionStrings.stringIDs.Add(4270339173U, "FromSubscriptionConditionFormat");
			OwaOptionStrings.stringIDs.Add(2476211802U, "EmailComposeModeSeparateForm");
			OwaOptionStrings.stringIDs.Add(1739026864U, "ReadingPaneSlab");
			OwaOptionStrings.stringIDs.Add(77678270U, "OOF");
			OwaOptionStrings.stringIDs.Add(1330353058U, "SearchResultsCaption");
			OwaOptionStrings.stringIDs.Add(3130274824U, "SubjectLabel");
			OwaOptionStrings.stringIDs.Add(2220842206U, "Minute");
			OwaOptionStrings.stringIDs.Add(753397160U, "SendAtColon");
			OwaOptionStrings.stringIDs.Add(177845278U, "NewCommandText");
			OwaOptionStrings.stringIDs.Add(4084881753U, "RetentionTypeRequired");
			OwaOptionStrings.stringIDs.Add(1138078555U, "UninstallExtensionsConfirmation");
			OwaOptionStrings.stringIDs.Add(3158885598U, "InboxRuleHasAttachmentConditionFlyOutText");
			OwaOptionStrings.stringIDs.Add(2285322212U, "MyselfEntFormat");
			OwaOptionStrings.stringIDs.Add(3472797073U, "ConnectedAccounts");
			OwaOptionStrings.stringIDs.Add(1659711502U, "GroupNotes");
			OwaOptionStrings.stringIDs.Add(1959773104U, "Status");
			OwaOptionStrings.stringIDs.Add(2313850175U, "TeamMailboxSyncStatusString");
			OwaOptionStrings.stringIDs.Add(993996681U, "AccountSecondaryNavigation");
			OwaOptionStrings.stringIDs.Add(2256263033U, "EmailSubscriptionSlabDescription");
			OwaOptionStrings.stringIDs.Add(3692628685U, "TeamMailboxMailString");
			OwaOptionStrings.stringIDs.Add(166656410U, "TeamMailboxLifecycleStatusString2");
			OwaOptionStrings.stringIDs.Add(298572739U, "JunkEmailTrustedListDescription");
			OwaOptionStrings.stringIDs.Add(3700740504U, "SundayCheckBoxText");
			OwaOptionStrings.stringIDs.Add(3309799253U, "ExtensionVersionTag");
			OwaOptionStrings.stringIDs.Add(2902697354U, "MailTip");
			OwaOptionStrings.stringIDs.Add(861811732U, "CalendarPublishingRestricted");
			OwaOptionStrings.stringIDs.Add(884476035U, "MailboxUsageUnavailable");
			OwaOptionStrings.stringIDs.Add(2108510325U, "Customize");
			OwaOptionStrings.stringIDs.Add(1115629481U, "ModerationEnabled");
			OwaOptionStrings.stringIDs.Add(3758991955U, "PreviewMarkAsReadBehaviorDelayed");
			OwaOptionStrings.stringIDs.Add(3807282937U, "ShareInformation");
			OwaOptionStrings.stringIDs.Add(1441613828U, "RetentionActionTypeArchive");
			OwaOptionStrings.stringIDs.Add(2977853317U, "SetUpNotifications");
			OwaOptionStrings.stringIDs.Add(2600878742U, "InboxRuleMoveToFolderActionFlyOutText");
			OwaOptionStrings.stringIDs.Add(854412308U, "JunkEmailContactsTrusted");
			OwaOptionStrings.stringIDs.Add(3522427619U, "TeamMailboxManagementString");
			OwaOptionStrings.stringIDs.Add(636192742U, "MessageTrackingTransferredEvent");
			OwaOptionStrings.stringIDs.Add(2329485994U, "SendToAllGalLessText");
			OwaOptionStrings.stringIDs.Add(2574861626U, "CalendarReminderInstruction");
			OwaOptionStrings.stringIDs.Add(3667432429U, "TotalMembers");
			OwaOptionStrings.stringIDs.Add(1262029551U, "MailboxUsageUnlimitedText");
			OwaOptionStrings.stringIDs.Add(394064455U, "CalendarTroubleshootingLinkText");
			OwaOptionStrings.stringIDs.Add(2661286284U, "DisplayRecoveryPasswordCommandText");
			OwaOptionStrings.stringIDs.Add(366963388U, "VoicemailWizardStep4Description");
			OwaOptionStrings.stringIDs.Add(365990270U, "IncomingSecurityLabel");
			OwaOptionStrings.stringIDs.Add(1133353571U, "InboxRuleForwardToActionText");
			OwaOptionStrings.stringIDs.Add(3807748391U, "InboxRuleMyNameIsGroupText");
			OwaOptionStrings.stringIDs.Add(3420690180U, "MailboxFolderDialogLabel");
			OwaOptionStrings.stringIDs.Add(21771688U, "ReturnToView");
			OwaOptionStrings.stringIDs.Add(469603767U, "DeviceActiveSyncVersionLabel");
			OwaOptionStrings.stringIDs.Add(4036777603U, "InstallFromPrivateUrlCaption");
			OwaOptionStrings.stringIDs.Add(3877512413U, "DeleteEmailSubscriptionConfirmation");
			OwaOptionStrings.stringIDs.Add(3119904789U, "VoicemailWizardComplete");
			OwaOptionStrings.stringIDs.Add(303742195U, "InboxRuleMarkAsReadActionFlyOutText");
			OwaOptionStrings.stringIDs.Add(3738802218U, "RPTDay");
			OwaOptionStrings.stringIDs.Add(2590635398U, "DeviceAccessStateSetByLabel");
			OwaOptionStrings.stringIDs.Add(1799036564U, "ViewGroupDetails");
			OwaOptionStrings.stringIDs.Add(2408048685U, "ToOnlyLabel");
			OwaOptionStrings.stringIDs.Add(2384846465U, "SensitivityDialogTitle");
			OwaOptionStrings.stringIDs.Add(1200890284U, "TeamMailboxLifecycleStatusString");
			OwaOptionStrings.stringIDs.Add(3028241800U, "WednesdayCheckBoxText");
			OwaOptionStrings.stringIDs.Add(2254792003U, "ExtensionRequirementsLabel");
			OwaOptionStrings.stringIDs.Add(2459846522U, "AlwaysShowBcc");
			OwaOptionStrings.stringIDs.Add(1703184725U, "ConflictPercentageAllowedErrorMessage");
			OwaOptionStrings.stringIDs.Add(3413282040U, "JoinRestrictionApprovalRequiredDetails");
			OwaOptionStrings.stringIDs.Add(1917681062U, "InboxRuleMarkImportanceActionText");
			OwaOptionStrings.stringIDs.Add(1073903281U, "InboxRuleRecipientAddressContainsConditionFlyOutText");
			OwaOptionStrings.stringIDs.Add(2228506517U, "Regional");
			OwaOptionStrings.stringIDs.Add(3644924947U, "VoicemailWizardTestDoneText");
			OwaOptionStrings.stringIDs.Add(2743720278U, "RemoveOldMeetingMessagesCheckBoxText");
			OwaOptionStrings.stringIDs.Add(63766217U, "InboxRuleBodyContainsConditionText");
			OwaOptionStrings.stringIDs.Add(1435597588U, "QLForward");
			OwaOptionStrings.stringIDs.Add(4103313421U, "VoicemailPhoneNumberColon");
			OwaOptionStrings.stringIDs.Add(1416133447U, "AddCommandText");
			OwaOptionStrings.stringIDs.Add(3917579091U, "Voicemail");
			OwaOptionStrings.stringIDs.Add(2321140182U, "StringArrayDialogTitle");
			OwaOptionStrings.stringIDs.Add(2696088269U, "MailboxUsageUnitTB");
			OwaOptionStrings.stringIDs.Add(1272238732U, "CalendarPublishingCopyLink");
			OwaOptionStrings.stringIDs.Add(1224467389U, "TimeStyles");
			OwaOptionStrings.stringIDs.Add(559979509U, "RPTYear");
			OwaOptionStrings.stringIDs.Add(2493642031U, "VoicemailLearnMoreVideo");
			OwaOptionStrings.stringIDs.Add(2564457623U, "InboxRuleHeaderContainsConditionFlyOutText");
			OwaOptionStrings.stringIDs.Add(1916260782U, "InboxRuleHasClassificationConditionText");
			OwaOptionStrings.stringIDs.Add(1806836259U, "ImportContactListPage1Caption");
			OwaOptionStrings.stringIDs.Add(3804549298U, "VoicemailWizardStep2Description");
			OwaOptionStrings.stringIDs.Add(1269143998U, "AddExtension");
			OwaOptionStrings.stringIDs.Add(2355901091U, "WithinSizeRangeDialogTitle");
			OwaOptionStrings.stringIDs.Add(2269401639U, "GetCalendarLogButtonText");
			OwaOptionStrings.stringIDs.Add(954238138U, "BlockDeviceConfirmMessage");
			OwaOptionStrings.stringIDs.Add(1294877450U, "CalendarPublishingRangeFrom");
			OwaOptionStrings.stringIDs.Add(3777014377U, "EnterPasscodeStepMessage");
			OwaOptionStrings.stringIDs.Add(3308373688U, "VoicemailCallFwdHavingTrouble");
			OwaOptionStrings.stringIDs.Add(3543278019U, "StartForward");
			OwaOptionStrings.stringIDs.Add(2118875232U, "VoicemailCallFwdStep2");
			OwaOptionStrings.stringIDs.Add(1225604874U, "JoinRestrictionOpenDetails");
			OwaOptionStrings.stringIDs.Add(2863884442U, "IncomingSecurityNone");
			OwaOptionStrings.stringIDs.Add(1827349577U, "InboxRuleMyNameNotInToBoxConditionText");
			OwaOptionStrings.stringIDs.Add(2515135494U, "ChangePermissions");
			OwaOptionStrings.stringIDs.Add(1807321962U, "InboxRuleCopyToFolderActionFlyOutText");
			OwaOptionStrings.stringIDs.Add(3118863998U, "InboxRuleSubjectContainsConditionFlyOutText");
			OwaOptionStrings.stringIDs.Add(1238462568U, "RequirementsRestrictedValue");
			OwaOptionStrings.stringIDs.Add(394662652U, "InboxRuleRedirectToActionText");
			OwaOptionStrings.stringIDs.Add(1472234713U, "ImportContactListPage1Step2");
			OwaOptionStrings.stringIDs.Add(1087517319U, "JunkEmailWatermarkText");
			OwaOptionStrings.stringIDs.Add(3803411495U, "TextMessagingStatusPrefixStatus");
			OwaOptionStrings.stringIDs.Add(4182339999U, "ShowHoursIn");
			OwaOptionStrings.stringIDs.Add(3390931208U, "DefaultFormat");
			OwaOptionStrings.stringIDs.Add(2893153135U, "SubscriptionDialogTitle");
			OwaOptionStrings.stringIDs.Add(3581746357U, "NewItemNotificationEmailToast");
			OwaOptionStrings.stringIDs.Add(994337869U, "TeamMailboxTabUsersHelpString1");
			OwaOptionStrings.stringIDs.Add(307942560U, "SchedulingPermissionsSlab");
			OwaOptionStrings.stringIDs.Add(3141117137U, "ConversationSortOrderInstruction");
			OwaOptionStrings.stringIDs.Add(3348884585U, "WipeDeviceCommandText");
			OwaOptionStrings.stringIDs.Add(757309800U, "InboxRuleSentOrReceivedGroupText");
			OwaOptionStrings.stringIDs.Add(1884883826U, "Myself");
			OwaOptionStrings.stringIDs.Add(885509158U, "NewestOnBottom");
			OwaOptionStrings.stringIDs.Add(4104125374U, "NewItemNotificationFaxToast");
			OwaOptionStrings.stringIDs.Add(4049797668U, "EmailComposeModeInline");
			OwaOptionStrings.stringIDs.Add(880505963U, "NewRuleString");
			OwaOptionStrings.stringIDs.Add(2688608043U, "NoMessageCategoryAvailable");
			OwaOptionStrings.stringIDs.Add(1328547437U, "CurrentStatus");
			OwaOptionStrings.stringIDs.Add(1395647162U, "SubscriptionProcessingError");
			OwaOptionStrings.stringIDs.Add(1075069807U, "StopAndRetrieveLogCommandText");
			OwaOptionStrings.stringIDs.Add(1917268543U, "TimeIncrementThirtyMinutes");
			OwaOptionStrings.stringIDs.Add(3253517897U, "RetentionActionNeverMove");
			OwaOptionStrings.stringIDs.Add(3600602092U, "VoicemailMobileOperatorColon");
			OwaOptionStrings.stringIDs.Add(2788873687U, "ConnectedAccountsDescriptionForForwarding");
			OwaOptionStrings.stringIDs.Add(2316526399U, "StopForward");
			OwaOptionStrings.stringIDs.Add(3739699132U, "FirstWeekOfYear");
			OwaOptionStrings.stringIDs.Add(2986818840U, "RegionListLabel");
			OwaOptionStrings.stringIDs.Add(2982857632U, "InstallFromMarketplace");
			OwaOptionStrings.stringIDs.Add(738818108U, "RulesNameColumn");
			OwaOptionStrings.stringIDs.Add(3166948736U, "DeviceOSLabel");
			OwaOptionStrings.stringIDs.Add(781854721U, "InboxRuleSentOnlyToMeConditionText");
			OwaOptionStrings.stringIDs.Add(364458648U, "EditYourPassword");
			OwaOptionStrings.stringIDs.Add(3609822921U, "EnforceSchedulingHorizon");
			OwaOptionStrings.stringIDs.Add(872104809U, "TeamMailboxManagementString2");
			OwaOptionStrings.stringIDs.Add(3459526114U, "SearchMessageTipForIWUser");
			OwaOptionStrings.stringIDs.Add(904314811U, "ConnectedAccountsDescriptionForSubscription");
			OwaOptionStrings.stringIDs.Add(3180823751U, "QLManageOrganization");
			OwaOptionStrings.stringIDs.Add(2201160322U, "JoinRestrictionApprovalRequired");
			OwaOptionStrings.stringIDs.Add(3344174056U, "ExtensionCanNotBeUninstalled");
			OwaOptionStrings.stringIDs.Add(1952564584U, "VoicemailWizardStep4Title");
			OwaOptionStrings.stringIDs.Add(1001261206U, "ViewExtensionDetails");
			OwaOptionStrings.stringIDs.Add(4181395631U, "VoicemailCarrierRatesMayApply");
			OwaOptionStrings.stringIDs.Add(2288925761U, "DeliveryReports");
			OwaOptionStrings.stringIDs.Add(3052389362U, "AllRequestOutOfPolicyText");
			OwaOptionStrings.stringIDs.Add(3585825647U, "RemoveDeviceConfirmMessage");
			OwaOptionStrings.stringIDs.Add(1401723706U, "StatusLabel");
			OwaOptionStrings.stringIDs.Add(3255907698U, "InboxRuleSubjectOrBodyContainsConditionText");
			OwaOptionStrings.stringIDs.Add(1547367963U, "OwnerLabel");
			OwaOptionStrings.stringIDs.Add(3925202351U, "RequireSenderAuthFalse");
			OwaOptionStrings.stringIDs.Add(2460807174U, "AllowedSendersLabel");
			OwaOptionStrings.stringIDs.Add(759422978U, "IncomingSecuritySSL");
			OwaOptionStrings.stringIDs.Add(952725660U, "CarrierListLabel");
			OwaOptionStrings.stringIDs.Add(3525277106U, "InboxRuleDescriptionNote");
			OwaOptionStrings.stringIDs.Add(3736448266U, "NewImapSubscription");
			OwaOptionStrings.stringIDs.Add(3089967521U, "TeamMailboxStartSyncButtonString");
			OwaOptionStrings.stringIDs.Add(2243312622U, "NotificationsForCalendarUpdate");
			OwaOptionStrings.stringIDs.Add(1134119669U, "ReadReceiptsSlab");
			OwaOptionStrings.stringIDs.Add(1550773931U, "DetailsLinkText");
			OwaOptionStrings.stringIDs.Add(1454393937U, "Help");
			OwaOptionStrings.stringIDs.Add(491491954U, "SearchGroups");
			OwaOptionStrings.stringIDs.Add(3108971784U, "ShowConversationAsTreeInstruction");
			OwaOptionStrings.stringIDs.Add(3137025124U, "BypassModerationSenders");
			OwaOptionStrings.stringIDs.Add(3781814528U, "RetentionActionDeleteAndAllowRecovery");
			OwaOptionStrings.stringIDs.Add(999443141U, "PreviewMarkAsReadDelaytimeTextPre");
			OwaOptionStrings.stringIDs.Add(565319905U, "RPTMonths");
			OwaOptionStrings.stringIDs.Add(1719300497U, "AfterMoveOrDeleteBehavior");
			OwaOptionStrings.stringIDs.Add(1359223188U, "HideGroupFromAddressLists");
			OwaOptionStrings.stringIDs.Add(2825394221U, "VoicemailWizardStep1Description");
			OwaOptionStrings.stringIDs.Add(2930172971U, "ReviewLinkText");
			OwaOptionStrings.stringIDs.Add(2325389167U, "Processing");
			OwaOptionStrings.stringIDs.Add(312564594U, "DailyCalendarAgendas");
			OwaOptionStrings.stringIDs.Add(1969050778U, "PreviewMarkAsReadBehaviorOnSelectionChange");
			OwaOptionStrings.stringIDs.Add(1817328658U, "TimeZoneLabelText");
			OwaOptionStrings.stringIDs.Add(3165477128U, "QLVoiceMail");
			OwaOptionStrings.stringIDs.Add(1490355023U, "VoicemailSignUpIntro");
			OwaOptionStrings.stringIDs.Add(4294954307U, "VoicemailStep2");
			OwaOptionStrings.stringIDs.Add(519599668U, "TeamMailboxMembershipString");
			OwaOptionStrings.stringIDs.Add(3463577248U, "PasscodeLabel");
			OwaOptionStrings.stringIDs.Add(1836294011U, "PersonalSettingPasswordAfterChange");
			OwaOptionStrings.stringIDs.Add(3090647821U, "VerificationSuccessPageTitle");
			OwaOptionStrings.stringIDs.Add(843476083U, "EnableAutomaticProcessingNote");
			OwaOptionStrings.stringIDs.Add(2422328107U, "Days");
			OwaOptionStrings.stringIDs.Add(521914504U, "NotificationsNotSetUp");
			OwaOptionStrings.stringIDs.Add(2608069889U, "ModerationNotificationsInternal");
			OwaOptionStrings.stringIDs.Add(3040964019U, "ProtocolSettings");
			OwaOptionStrings.stringIDs.Add(4275718891U, "EnableAutomaticProcessing");
			OwaOptionStrings.stringIDs.Add(2045071371U, "MessageOptionsSlab");
			OwaOptionStrings.stringIDs.Add(2789466833U, "ChooseMessageFont");
			OwaOptionStrings.stringIDs.Add(3563093647U, "Password");
			OwaOptionStrings.stringIDs.Add(1901616043U, "OWAExtensions");
			OwaOptionStrings.stringIDs.Add(664237562U, "StringArrayDialogLabel");
			OwaOptionStrings.stringIDs.Add(944449543U, "Unlimited");
			OwaOptionStrings.stringIDs.Add(4033068416U, "VoicemailSMSOptionVoiceMailOnly");
			OwaOptionStrings.stringIDs.Add(3853542865U, "Rules");
			OwaOptionStrings.stringIDs.Add(860120982U, "ModeratedByEmptyDataText");
			OwaOptionStrings.stringIDs.Add(499094223U, "TextMessaging");
			OwaOptionStrings.stringIDs.Add(2038251428U, "FromLabel");
			OwaOptionStrings.stringIDs.Add(2108668357U, "GroupModerators");
			OwaOptionStrings.stringIDs.Add(4089833856U, "ReadingPaneInstruction");
			OwaOptionStrings.stringIDs.Add(1749888754U, "RPTNone");
			OwaOptionStrings.stringIDs.Add(2618236676U, "Spelling");
			OwaOptionStrings.stringIDs.Add(3198693159U, "CancelWipeDeviceCommandText");
			OwaOptionStrings.stringIDs.Add(3747885709U, "AutomaticRepliesEnabledText");
			OwaOptionStrings.stringIDs.Add(2372820691U, "DisplayNameLabel");
			OwaOptionStrings.stringIDs.Add(2449169153U, "CancelButtonText");
			OwaOptionStrings.stringIDs.Add(3156651890U, "GroupMembershipApproval");
			OwaOptionStrings.stringIDs.Add(3398667566U, "InlcudedRecipientTypesLabel");
			OwaOptionStrings.stringIDs.Add(2328219947U, "Name");
			OwaOptionStrings.stringIDs.Add(590111363U, "RetentionActionTypeDelete");
			OwaOptionStrings.stringIDs.Add(3652102134U, "InboxRuleMyNameInCcBoxConditionFlyOutText");
			OwaOptionStrings.stringIDs.Add(2328532194U, "ThursdayCheckBoxText");
			OwaOptionStrings.stringIDs.Add(258131565U, "JoinGroup");
			OwaOptionStrings.stringIDs.Add(767759945U, "Account");
			OwaOptionStrings.stringIDs.Add(1107043549U, "InboxRuleMessageTypeMatchesConditionFlyOutText");
			OwaOptionStrings.stringIDs.Add(1934149463U, "InboxRuleMoveCopyDeleteGroupText");
			OwaOptionStrings.stringIDs.Add(294641626U, "RegionalSettingsInstruction");
			OwaOptionStrings.stringIDs.Add(1009337581U, "NameColumn");
			OwaOptionStrings.stringIDs.Add(2921806668U, "InboxRuleWithSensitivityConditionFlyOutText");
			OwaOptionStrings.stringIDs.Add(744201260U, "ClassificationDialogTitle");
			OwaOptionStrings.stringIDs.Add(2012947571U, "RuleFromAndMoveToFolderTemplate");
			OwaOptionStrings.stringIDs.Add(2112722880U, "DomainNameNotSetError");
			OwaOptionStrings.stringIDs.Add(3382481163U, "MakeSecurityGroup");
			OwaOptionStrings.stringIDs.Add(111122248U, "ContactNumbersBookmark");
			OwaOptionStrings.stringIDs.Add(3443085801U, "InboxRuleSentToConditionText");
			OwaOptionStrings.stringIDs.Add(1620775745U, "MemberOfGroups");
			OwaOptionStrings.stringIDs.Add(2393490804U, "InboxRuleIncludeTheseWordsGroupText");
			OwaOptionStrings.stringIDs.Add(691241872U, "MailTipLabel");
			OwaOptionStrings.stringIDs.Add(1413097981U, "MessageTypeDialogLabel");
			OwaOptionStrings.stringIDs.Add(1541052658U, "RegionalSettingsSlab");
			OwaOptionStrings.stringIDs.Add(346591353U, "VoicemailWizardStep3Title");
			OwaOptionStrings.stringIDs.Add(2731962334U, "InboxRuleMyNameNotInToBoxConditionFlyOutText");
			OwaOptionStrings.stringIDs.Add(510973434U, "ReturnToOWA");
			OwaOptionStrings.stringIDs.Add(1564510770U, "InboxRuleMyNameInToBoxConditionText");
			OwaOptionStrings.stringIDs.Add(3567818904U, "CommitButtonText");
			OwaOptionStrings.stringIDs.Add(1864411629U, "TeamMailboxShowInMyClientString");
			OwaOptionStrings.stringIDs.Add(2397374352U, "InboxRuleMarkAsReadActionText");
			OwaOptionStrings.stringIDs.Add(663639812U, "ClassificationDialogLabel");
			OwaOptionStrings.stringIDs.Add(3313607735U, "WarningAlt");
			OwaOptionStrings.stringIDs.Add(2034904223U, "TeamMailboxManagementString4");
			OwaOptionStrings.stringIDs.Add(405905481U, "Mail");
			OwaOptionStrings.stringIDs.Add(624357391U, "ImportContactList");
			OwaOptionStrings.stringIDs.Add(1680686955U, "QLImportContacts");
			OwaOptionStrings.stringIDs.Add(3391394280U, "InboxRule");
			OwaOptionStrings.stringIDs.Add(3979669380U, "WithinDateRangeDialogTitle");
			OwaOptionStrings.stringIDs.Add(1449143764U, "ReminderSoundEnabled");
			OwaOptionStrings.stringIDs.Add(1634623908U, "RecipientAddressContainsConditionFormat");
			OwaOptionStrings.stringIDs.Add(481168139U, "MessageFormatPlainText");
			OwaOptionStrings.stringIDs.Add(203738408U, "DeleteInboxRuleConfirmation");
			OwaOptionStrings.stringIDs.Add(2879015231U, "ForwardEmailTitle");
			OwaOptionStrings.stringIDs.Add(3395154598U, "BypassModerationSendersEmptyDataText");
			OwaOptionStrings.stringIDs.Add(396113514U, "RuleSubjectContainsAndDeleteMessageTemplate");
			OwaOptionStrings.stringIDs.Add(1053461607U, "HideDeletedItems");
			OwaOptionStrings.stringIDs.Add(3137581701U, "VoicemailSetupNowButtonText");
			OwaOptionStrings.stringIDs.Add(797589720U, "CalendarPermissions");
			OwaOptionStrings.stringIDs.Add(1108157303U, "ModerationNotificationsAlways");
			OwaOptionStrings.stringIDs.Add(1038822924U, "ExternalMessageInstruction");
			OwaOptionStrings.stringIDs.Add(2936411810U, "RequirementsReadItemValue");
			OwaOptionStrings.stringIDs.Add(4225166530U, "SchedulingOptionsSlab");
			OwaOptionStrings.stringIDs.Add(742094856U, "ShowConversationTree");
			OwaOptionStrings.stringIDs.Add(1303277130U, "RetentionPolicies");
			OwaOptionStrings.stringIDs.Add(2271033387U, "CalendarPublishingSubscriptionUrl");
			OwaOptionStrings.stringIDs.Add(1808002020U, "ResendVerificationEmailCommandText");
			OwaOptionStrings.stringIDs.Add(2949887068U, "TextMessagingNotification");
			OwaOptionStrings.stringIDs.Add(3246855305U, "InstalledByColumn");
			OwaOptionStrings.stringIDs.Add(3100669839U, "GroupOrganizationalUnit");
			OwaOptionStrings.stringIDs.Add(1750301704U, "MailboxFolderDialogTitle");
			OwaOptionStrings.stringIDs.Add(2669253750U, "PersonalSettingOldPassword");
			OwaOptionStrings.stringIDs.Add(2728870366U, "VoicemailStep3");
			OwaOptionStrings.stringIDs.Add(3140976517U, "CityLabel");
			OwaOptionStrings.stringIDs.Add(1022114031U, "SentToConditionFormat");
			OwaOptionStrings.stringIDs.Add(260433958U, "QLSubscription");
			OwaOptionStrings.stringIDs.Add(803108465U, "ViewRPTDetailsTitle");
			OwaOptionStrings.stringIDs.Add(3201916728U, "MyGroups");
			OwaOptionStrings.stringIDs.Add(349837376U, "TeamMailboxesString");
			OwaOptionStrings.stringIDs.Add(1220880656U, "DeliveryReport");
			OwaOptionStrings.stringIDs.Add(3294022635U, "LastNameLabel");
			OwaOptionStrings.stringIDs.Add(813081783U, "CalendarPublishingStop");
			OwaOptionStrings.stringIDs.Add(3339583031U, "VoicemailWizardStep3Description");
			OwaOptionStrings.stringIDs.Add(2176445071U, "QLWhatsNewForOrganizations");
			OwaOptionStrings.stringIDs.Add(3853933336U, "ReadReceiptResponseAlways");
			OwaOptionStrings.stringIDs.Add(3555736038U, "JunkEmailTrustedListsOnly");
			OwaOptionStrings.stringIDs.Add(2391157043U, "MatchSortOrder");
			OwaOptionStrings.stringIDs.Add(2421998101U, "DevicePhoneNumberLabel");
			OwaOptionStrings.stringIDs.Add(3176989690U, "InboxRuleMessageTypeMatchesConditionText");
			OwaOptionStrings.stringIDs.Add(4266991160U, "RetentionTypeOptional");
			OwaOptionStrings.stringIDs.Add(3996928596U, "UseSettings");
			OwaOptionStrings.stringIDs.Add(1958633523U, "SearchAllGroups");
			OwaOptionStrings.stringIDs.Add(2117869041U, "MembersLabel");
			OwaOptionStrings.stringIDs.Add(1063608921U, "FreeBusyInformation");
			OwaOptionStrings.stringIDs.Add(133785142U, "DeviceIMEILabel");
			OwaOptionStrings.stringIDs.Add(696030412U, "Day");
			OwaOptionStrings.stringIDs.Add(192786603U, "InboxRuleMoveToFolderActionText");
			OwaOptionStrings.stringIDs.Add(779120846U, "SelectOne");
			OwaOptionStrings.stringIDs.Add(846466190U, "InboxRuleApplyCategoryActionFlyOutText");
			OwaOptionStrings.stringIDs.Add(3990783281U, "SetupEmailNotificationsLink");
			OwaOptionStrings.stringIDs.Add(117083423U, "NewDistributionGroupCaption");
			OwaOptionStrings.stringIDs.Add(1217964679U, "ViewRPTRetentionActionLabel");
			OwaOptionStrings.stringIDs.Add(2135021465U, "ShowWeekNumbersCheckBoxText");
			OwaOptionStrings.stringIDs.Add(1136770150U, "UserNameWLIDLabel");
			OwaOptionStrings.stringIDs.Add(2028785684U, "SearchMessagesIReceivedLabel");
			OwaOptionStrings.stringIDs.Add(2151247442U, "InboxRuleWithinDateRangeConditionText");
			OwaOptionStrings.stringIDs.Add(839503915U, "InboxRuleSendTextMessageNotificationToActionText");
			OwaOptionStrings.stringIDs.Add(3985530922U, "SentLabel");
			OwaOptionStrings.stringIDs.Add(2324730983U, "GroupInformation");
			OwaOptionStrings.stringIDs.Add(2601154324U, "VoicemailAskOperator");
			OwaOptionStrings.stringIDs.Add(4016415205U, "OWA");
			OwaOptionStrings.stringIDs.Add(574892128U, "MailTipWaterMark");
			OwaOptionStrings.stringIDs.Add(969182653U, "InboxRuleWithSensitivityConditionText");
			OwaOptionStrings.stringIDs.Add(1090313663U, "RetentionActionTypeDefaultArchive");
			OwaOptionStrings.stringIDs.Add(454922625U, "RemoveOptionalRPTConfirmation");
			OwaOptionStrings.stringIDs.Add(1570694400U, "DevicePolicyUpdateTimeLabel");
			OwaOptionStrings.stringIDs.Add(1674956465U, "MyselfLiveFormat");
			OwaOptionStrings.stringIDs.Add(2143618034U, "EmailDomain");
			OwaOptionStrings.stringIDs.Add(148346506U, "RequireSenderAuthTrue");
			OwaOptionStrings.stringIDs.Add(774036985U, "InstallFromPrivateUrlInformation");
			OwaOptionStrings.stringIDs.Add(446237888U, "PhoneLabel");
			OwaOptionStrings.stringIDs.Add(731708393U, "SubscriptionAction");
			OwaOptionStrings.stringIDs.Add(2401508539U, "Weeks");
			OwaOptionStrings.stringIDs.Add(3825281971U, "InboxRuleForwardRedirectGroupText");
			OwaOptionStrings.stringIDs.Add(3719582777U, "DisplayName");
			OwaOptionStrings.stringIDs.Add(3507262891U, "MembershipApprovalLabel");
			OwaOptionStrings.stringIDs.Add(1542072756U, "InboxRuleSendTextMessageNotificationToActionFlyOutText");
			OwaOptionStrings.stringIDs.Add(3475829388U, "TeamMailboxSPSiteString");
			OwaOptionStrings.stringIDs.Add(1559211265U, "InboxRuleSubjectOrBodyContainsConditionFlyOutText");
			OwaOptionStrings.stringIDs.Add(1690161621U, "PleaseWait");
			OwaOptionStrings.stringIDs.Add(258867536U, "JoinRestrictionOpen");
			OwaOptionStrings.stringIDs.Add(418352798U, "CalendarSharingConfirmDeletionSingle");
			OwaOptionStrings.stringIDs.Add(1869505729U, "InboxRuleSubjectContainsConditionPreCannedText");
			OwaOptionStrings.stringIDs.Add(2815967975U, "CalendarPublishingStart");
			OwaOptionStrings.stringIDs.Add(1632853873U, "TextMessagingSlabMessageNotificationOnly");
			OwaOptionStrings.stringIDs.Add(3304439859U, "RequirementsRestrictedDescription");
			OwaOptionStrings.stringIDs.Add(4114414654U, "SelectAUser");
			OwaOptionStrings.stringIDs.Add(3042824760U, "NotificationStepOneMessage");
			OwaOptionStrings.stringIDs.Add(2897641359U, "QLPushEmail");
			OwaOptionStrings.stringIDs.Add(1890566340U, "NewInboxRuleTitle");
			OwaOptionStrings.stringIDs.Add(929330042U, "SendToKnownContactsText");
			OwaOptionStrings.stringIDs.Add(48495520U, "IncomingAuthenticationSpa");
			OwaOptionStrings.stringIDs.Add(2696088282U, "MailboxUsageUnitGB");
			OwaOptionStrings.stringIDs.Add(3651870140U, "MessageTrackingDeliveredEvent");
			OwaOptionStrings.stringIDs.Add(3772909321U, "SelectOneOrMoreText");
			OwaOptionStrings.stringIDs.Add(2600859168U, "InboxRuleForwardAsAttachmentToActionText");
			OwaOptionStrings.stringIDs.Add(1105583403U, "DontSeeMyRegionOrMobileOperator");
			OwaOptionStrings.stringIDs.Add(1436160346U, "PreviewMarkAsReadDelaytimeTextPost");
			OwaOptionStrings.stringIDs.Add(2060430325U, "NoMessageClassificationAvailable");
			OwaOptionStrings.stringIDs.Add(3929633211U, "TeamMailboxTabPropertiesHelpString");
			OwaOptionStrings.stringIDs.Add(2438188750U, "TeamMailboxManagementString1");
			OwaOptionStrings.stringIDs.Add(4047036620U, "RetentionPeriodHeader");
			OwaOptionStrings.stringIDs.Add(4263477646U, "Phone");
			OwaOptionStrings.stringIDs.Add(2862314013U, "WhoHasPermission");
			OwaOptionStrings.stringIDs.Add(1766818386U, "NotAvailable");
			OwaOptionStrings.stringIDs.Add(1253897847U, "FlaggedForActionConditionFormat");
			OwaOptionStrings.stringIDs.Add(4028994607U, "EndTimeText");
			OwaOptionStrings.stringIDs.Add(234118977U, "BookingWindowInDays");
			OwaOptionStrings.stringIDs.Add(3253425509U, "RemovingPreviewPhoto");
			OwaOptionStrings.stringIDs.Add(1666931485U, "CalendarDiagnosticLogSlab");
			OwaOptionStrings.stringIDs.Add(3454695329U, "ModerationNotification");
			OwaOptionStrings.stringIDs.Add(1207804035U, "VoicemailWizardPinlessOptionsText");
			OwaOptionStrings.stringIDs.Add(2476883967U, "VoicemailClearSettings");
			OwaOptionStrings.stringIDs.Add(1094425034U, "PersonalGroups");
			OwaOptionStrings.stringIDs.Add(259319032U, "DistributionGroupText");
			OwaOptionStrings.stringIDs.Add(1335943323U, "ProfileContactNumbers");
			OwaOptionStrings.stringIDs.Add(167657761U, "DeliveryReportFor");
			OwaOptionStrings.stringIDs.Add(232975871U, "InboxRuleRedirectToActionFlyOutText");
			OwaOptionStrings.stringIDs.Add(2251801313U, "AutomateProcessing");
			OwaOptionStrings.stringIDs.Add(3493646365U, "CalendarPublishingAccessLevel");
			OwaOptionStrings.stringIDs.Add(248447020U, "ForwardEmailTo");
			OwaOptionStrings.stringIDs.Add(2062923077U, "SettingUpProtocolAccess");
			OwaOptionStrings.stringIDs.Add(1732697476U, "AdminTools");
			OwaOptionStrings.stringIDs.Add(1669803655U, "InstalledExtensionDescription");
			OwaOptionStrings.stringIDs.Add(568490923U, "EmailComposeModeInstruction");
			OwaOptionStrings.stringIDs.Add(3271192817U, "InboxRuleDeleteMessageActionText");
			OwaOptionStrings.stringIDs.Add(442231679U, "SummaryToDate");
			OwaOptionStrings.stringIDs.Add(2631270417U, "Extension");
			OwaOptionStrings.stringIDs.Add(1549389490U, "CalendarSharingConfirmDeletionMultiple");
			OwaOptionStrings.stringIDs.Add(1314739276U, "Depart");
			OwaOptionStrings.stringIDs.Add(2193710474U, "EmailNotificationsLink");
			OwaOptionStrings.stringIDs.Add(1192767596U, "OpenPreviousItem");
			OwaOptionStrings.stringIDs.Add(1886990352U, "RPTPickerDialogTitle");
			OwaOptionStrings.stringIDs.Add(625886851U, "CheckForForgottenAttachments");
			OwaOptionStrings.stringIDs.Add(2755319165U, "FaxLabel");
			OwaOptionStrings.stringIDs.Add(2677919833U, "SentTime");
			OwaOptionStrings.stringIDs.Add(673404970U, "DeviceTypeHeaderText");
			OwaOptionStrings.stringIDs.Add(338457679U, "RPTExpireNever");
			OwaOptionStrings.stringIDs.Add(779255172U, "TeamMailboxEmailAddressString");
			OwaOptionStrings.stringIDs.Add(530176925U, "InboxRuleWithinSizeRangeConditionText");
			OwaOptionStrings.stringIDs.Add(2213908942U, "Week");
			OwaOptionStrings.stringIDs.Add(2185035188U, "WithinDateRangeConditionFormat");
			OwaOptionStrings.stringIDs.Add(2502896170U, "DisplayNameNotSetError");
			OwaOptionStrings.stringIDs.Add(198361899U, "FirstDayOfWeek");
			OwaOptionStrings.stringIDs.Add(2556260573U, "ProcessExternalMeetingMessagesCheckBoxText");
			OwaOptionStrings.stringIDs.Add(2041861674U, "DepartRestrictionClosed");
			OwaOptionStrings.stringIDs.Add(886092638U, "MailTipShortLabel");
			OwaOptionStrings.stringIDs.Add(2483718204U, "StateOrProvinceLabel");
			OwaOptionStrings.stringIDs.Add(67466988U, "PrimaryEmailAddress");
			OwaOptionStrings.stringIDs.Add(2578200319U, "AllowedSendersEmptyLabel");
			OwaOptionStrings.stringIDs.Add(1878748957U, "HotmailSubscription");
			OwaOptionStrings.stringIDs.Add(2638705563U, "DeviceIDLabel");
			OwaOptionStrings.stringIDs.Add(1891390470U, "AlwaysShowFrom");
			OwaOptionStrings.stringIDs.Add(1795719989U, "SearchButtonText");
			OwaOptionStrings.stringIDs.Add(4001240631U, "ViewRPTDescriptionLabel");
			OwaOptionStrings.stringIDs.Add(2624099920U, "Hour");
			OwaOptionStrings.stringIDs.Add(3567033964U, "FlagStatusDialogTitle");
			OwaOptionStrings.stringIDs.Add(1463926066U, "NewSubscriptionProgress");
			OwaOptionStrings.stringIDs.Add(2455977975U, "EditProfilePhoto");
			OwaOptionStrings.stringIDs.Add(614024525U, "InstallFromPrivateUrl");
			OwaOptionStrings.stringIDs.Add(2869098816U, "DeleteGroupsConfirmation");
			OwaOptionStrings.stringIDs.Add(870015935U, "OwnedGroups");
			OwaOptionStrings.stringIDs.Add(2322301053U, "NewDistributionGroupTitle");
			OwaOptionStrings.stringIDs.Add(2562827638U, "JunkEmailConfiguration");
			OwaOptionStrings.stringIDs.Add(3873537801U, "ProfileGeneral");
			OwaOptionStrings.stringIDs.Add(2344432611U, "CalendarSharingOutlookNote");
			OwaOptionStrings.stringIDs.Add(829380600U, "RemoveOptionalRPTsConfirmation");
			OwaOptionStrings.stringIDs.Add(2472855514U, "VerificationSuccessTitle");
			OwaOptionStrings.stringIDs.Add(3585330084U, "ResponseMessageSlab");
			OwaOptionStrings.stringIDs.Add(1695816201U, "TeamMailboxMaintenanceString");
			OwaOptionStrings.stringIDs.Add(1565811752U, "UrlLabelText");
			OwaOptionStrings.stringIDs.Add(3765209046U, "DepartRestrictionOpen");
			OwaOptionStrings.stringIDs.Add(2239331369U, "PendingWipeCommandSentLabel");
			OwaOptionStrings.stringIDs.Add(2494867144U, "SubjectOrBodyContainsConditionFormat");
			OwaOptionStrings.stringIDs.Add(1747388690U, "LeaveMailOnServerLabel");
			OwaOptionStrings.stringIDs.Add(2054798096U, "JoinRestrictionClosed");
			OwaOptionStrings.stringIDs.Add(46775972U, "ScheduleOnlyDuringWorkHours");
			OwaOptionStrings.stringIDs.Add(614963610U, "ImportanceDialogLabel");
			OwaOptionStrings.stringIDs.Add(3410351290U, "CalendarPublishingLinkNotes");
			OwaOptionStrings.stringIDs.Add(1356158868U, "InboxRuleSentOnlyToMeConditionFlyOutText");
			OwaOptionStrings.stringIDs.Add(2409360890U, "InternalMessageInstruction");
			OwaOptionStrings.stringIDs.Add(1192855764U, "JunkEmailDisabled");
			OwaOptionStrings.stringIDs.Add(624477965U, "InboxRuleForwardAsAttachmentToActionFlyOutText");
			OwaOptionStrings.stringIDs.Add(2343908218U, "WarningNote");
			OwaOptionStrings.stringIDs.Add(4104737160U, "HomePagePrimaryLink");
			OwaOptionStrings.stringIDs.Add(837698603U, "LimitDuration");
			OwaOptionStrings.stringIDs.Add(3506885255U, "MailboxUsageExceededText");
			OwaOptionStrings.stringIDs.Add(3647997407U, "UnSupportedRule");
			OwaOptionStrings.stringIDs.Add(37810473U, "DeviceModelLabel");
			OwaOptionStrings.stringIDs.Add(2737072517U, "NotificationLinksMessage");
			OwaOptionStrings.stringIDs.Add(2702941902U, "ResourceSlab");
			OwaOptionStrings.stringIDs.Add(4137250117U, "RetentionPolicyTagPageTitle");
			OwaOptionStrings.stringIDs.Add(1485698978U, "AllBookInPolicyText");
			OwaOptionStrings.stringIDs.Add(2179002981U, "MessageFormatHtml");
			OwaOptionStrings.stringIDs.Add(755136567U, "FridayCheckBoxText");
			OwaOptionStrings.stringIDs.Add(3099627267U, "InboxRuleMyNameInCcBoxConditionText");
			OwaOptionStrings.stringIDs.Add(2592606558U, "PreviewMarkAsReadDelaytimeErrorMessage");
			OwaOptionStrings.stringIDs.Add(3576971014U, "JunkEmailValidationErrorMessage");
			OwaOptionStrings.stringIDs.Add(3723221224U, "TeamMailboxTabUsersHelpString2");
			OwaOptionStrings.stringIDs.Add(856468152U, "AllRequestInPolicyText");
			OwaOptionStrings.stringIDs.Add(3383857716U, "ImapSubscription");
			OwaOptionStrings.stringIDs.Add(2201998933U, "InboxRuleWithImportanceConditionFlyOutText");
			OwaOptionStrings.stringIDs.Add(600392665U, "TeamMailboxDisplayNameString");
			OwaOptionStrings.stringIDs.Add(3600988164U, "TeamMailboxManagementString3");
			OwaOptionStrings.stringIDs.Add(1735921300U, "ConflictPercentageAllowed");
			OwaOptionStrings.stringIDs.Add(695525694U, "ImportanceDialogTitle");
			OwaOptionStrings.stringIDs.Add(265625760U, "SecurityGroupText");
			OwaOptionStrings.stringIDs.Add(1528543719U, "SubjectContainsConditionFormat");
			OwaOptionStrings.stringIDs.Add(1566070952U, "VoicemailStep1");
			OwaOptionStrings.stringIDs.Add(1472234710U, "ImportContactListPage1Step1");
			OwaOptionStrings.stringIDs.Add(2919653011U, "CalendarPublishingRangeTo");
			OwaOptionStrings.stringIDs.Add(924528516U, "Installed");
			OwaOptionStrings.stringIDs.Add(2532105814U, "JoinRestrictionClosedDetails");
			OwaOptionStrings.stringIDs.Add(3686719114U, "EnterNumberStepMessage");
			OwaOptionStrings.stringIDs.Add(3769321652U, "TeamMailboxString");
			OwaOptionStrings.stringIDs.Add(1116185611U, "ExternalAudienceCheckboxText");
			OwaOptionStrings.stringIDs.Add(3758567081U, "RetentionActionNeverDelete");
			OwaOptionStrings.stringIDs.Add(3601061204U, "TeamMailboxOwnersString");
			OwaOptionStrings.stringIDs.Add(2454510675U, "SentOnlyToMeConditionFormat");
			OwaOptionStrings.stringIDs.Add(685553614U, "PostalCodeLabel");
			OwaOptionStrings.stringIDs.Add(2399940313U, "StreetAddressLabel");
			OwaOptionStrings.stringIDs.Add(1949934660U, "ModerationNotificationsNever");
			OwaOptionStrings.stringIDs.Add(3043881226U, "AutoAddSignature");
			OwaOptionStrings.stringIDs.Add(362303575U, "VoicemailConfiguration");
			OwaOptionStrings.stringIDs.Add(1231202734U, "TeamMailboxUsersString");
			OwaOptionStrings.stringIDs.Add(4116169389U, "Minutes");
			OwaOptionStrings.stringIDs.Add(1475482257U, "TextMessagingStatusPrefixNotifications");
			OwaOptionStrings.stringIDs.Add(947054436U, "OfficeLabel");
			OwaOptionStrings.stringIDs.Add(3570705387U, "SetupCalendarNotificationsLink");
			OwaOptionStrings.stringIDs.Add(2829325136U, "WipeDeviceConfirmMessage");
			OwaOptionStrings.stringIDs.Add(1970261471U, "JunkEmailEnabled");
			OwaOptionStrings.stringIDs.Add(2091217323U, "ProfilePhoto");
			OwaOptionStrings.stringIDs.Add(1213316404U, "JoinRestriction");
			OwaOptionStrings.stringIDs.Add(1603898988U, "SubscriptionAccountInformation");
			OwaOptionStrings.stringIDs.Add(1983970360U, "PopSubscription");
			OwaOptionStrings.stringIDs.Add(1651913438U, "ConnectedAccountsDescriptionForSendAs");
			OwaOptionStrings.stringIDs.Add(1355905147U, "VoicemailNotConfiguredText");
			OwaOptionStrings.stringIDs.Add(2328219770U, "Date");
			OwaOptionStrings.stringIDs.Add(2118902877U, "SubscriptionEmailAddress");
			OwaOptionStrings.stringIDs.Add(1262918758U, "CalendarAppearanceInstruction");
			OwaOptionStrings.stringIDs.Add(779604653U, "DisableReminders");
			OwaOptionStrings.stringIDs.Add(58190828U, "UninstallExtensionConfirmation");
			OwaOptionStrings.stringIDs.Add(272167933U, "ReadReceiptResponseAskBefore");
			OwaOptionStrings.stringIDs.Add(2721707952U, "AutomaticRepliesDisabledText");
			OwaOptionStrings.stringIDs.Add(2824009732U, "PermissionGranted");
			OwaOptionStrings.stringIDs.Add(473555910U, "CalendarTroubleShootingSlab");
			OwaOptionStrings.stringIDs.Add(3318664764U, "QLRemotePowerShell");
			OwaOptionStrings.stringIDs.Add(3266331791U, "Ownership");
			OwaOptionStrings.stringIDs.Add(1330376901U, "DevicePhoneNumberHeaderText");
			OwaOptionStrings.stringIDs.Add(1467122827U, "OwnerApprovalRequired");
			OwaOptionStrings.stringIDs.Add(936255600U, "AddExtensionTitle");
			OwaOptionStrings.stringIDs.Add(2172431100U, "DevicePolicyApplicationStatusLabel");
			OwaOptionStrings.stringIDs.Add(3387549369U, "CalendarWorkflowSlab");
			OwaOptionStrings.stringIDs.Add(1249796258U, "SettingNotAvailable");
			OwaOptionStrings.stringIDs.Add(1244143874U, "DepartRestriction");
			OwaOptionStrings.stringIDs.Add(3089991175U, "RetentionTypeRequiredDescription");
			OwaOptionStrings.stringIDs.Add(4116953573U, "RemoveForwardedMeetingNotificationsCheckBoxText");
			OwaOptionStrings.stringIDs.Add(3708929833U, "Everyone");
			OwaOptionStrings.stringIDs.Add(3452175682U, "TeamMailboxAppTitle");
			OwaOptionStrings.stringIDs.Add(2525011123U, "PersonalSettingConfirmPassword");
			OwaOptionStrings.stringIDs.Add(2696088292U, "MailboxUsageUnitMB");
			OwaOptionStrings.stringIDs.Add(4155090378U, "SubscriptionServerInformation");
			OwaOptionStrings.stringIDs.Add(2385969192U, "UserNameLabel");
			OwaOptionStrings.stringIDs.Add(776327314U, "TimeIncrementFifteenMinutes");
			OwaOptionStrings.stringIDs.Add(2051542189U, "LastSuccessfulSync");
			OwaOptionStrings.stringIDs.Add(4052999952U, "SchedulingPermissionsInstruction");
			OwaOptionStrings.stringIDs.Add(1517497942U, "EmptyDeletedItemsOnLogoff");
			OwaOptionStrings.stringIDs.Add(2251065475U, "BeforeDateDisplayTemplate");
			OwaOptionStrings.stringIDs.Add(2121350552U, "EmailAddressLabel");
			OwaOptionStrings.stringIDs.Add(1895146354U, "JunkEmailBlockedListDescription");
			OwaOptionStrings.stringIDs.Add(3334007327U, "NewItemNotificationSound");
			OwaOptionStrings.stringIDs.Add(272267124U, "NewInboxRuleCaption");
			OwaOptionStrings.stringIDs.Add(1462746207U, "UpdateTimeZoneNoteLinkText");
			OwaOptionStrings.stringIDs.Add(5631064U, "HasClassificationConditionFormat");
			OwaOptionStrings.stringIDs.Add(705899704U, "NoneAccessRightRole");
			OwaOptionStrings.stringIDs.Add(3116350389U, "MobileDeviceDetailTitle");
			OwaOptionStrings.stringIDs.Add(624738140U, "EditCommandText");
			OwaOptionStrings.stringIDs.Add(3623666006U, "DeviceTypeLabel");
			OwaOptionStrings.stringIDs.Add(4012923742U, "AllowConflicts");
			OwaOptionStrings.stringIDs.Add(3458491959U, "CalendarPublishing");
			OwaOptionStrings.stringIDs.Add(2717011154U, "RetentionNameHeader");
			OwaOptionStrings.stringIDs.Add(1036626U, "ImportContactListPage1InformationText");
			OwaOptionStrings.stringIDs.Add(3055482209U, "VoicemailAskPhoneNumber");
			OwaOptionStrings.stringIDs.Add(453080500U, "TeamMailboxDocumentsString");
			OwaOptionStrings.stringIDs.Add(3751262095U, "CountryOrRegionLabel");
			OwaOptionStrings.stringIDs.Add(1816995256U, "CalendarPublishingLearnMore");
			OwaOptionStrings.stringIDs.Add(1989730689U, "GroupModeratedBy");
			OwaOptionStrings.stringIDs.Add(4168470247U, "DidntReceivePasscodeMessage");
			OwaOptionStrings.stringIDs.Add(3197162109U, "InboxRuleFromAddressContainsConditionText");
			OwaOptionStrings.stringIDs.Add(1889609004U, "IncomingAuthenticationLabel");
			OwaOptionStrings.stringIDs.Add(2280348446U, "InboxRuleRecipientAddressContainsConditionText");
			OwaOptionStrings.stringIDs.Add(1975661589U, "DepartGroupsConfirmation");
			OwaOptionStrings.stringIDs.Add(2084354126U, "CalendarAppearanceSlab");
			OwaOptionStrings.stringIDs.Add(2908173103U, "InitialsLabel");
			OwaOptionStrings.stringIDs.Add(3446713793U, "InboxRuleFlaggedForActionConditionText");
			OwaOptionStrings.stringIDs.Add(3191690622U, "QuickLinks");
			OwaOptionStrings.stringIDs.Add(3789018700U, "TeamMailboxMyRoleString2");
			OwaOptionStrings.stringIDs.Add(2359136379U, "RoomEmailAddressLabel");
			OwaOptionStrings.stringIDs.Add(1233816147U, "ToColumnLabel");
			OwaOptionStrings.stringIDs.Add(1628295023U, "SensitivityDialogLabel");
			OwaOptionStrings.stringIDs.Add(3728295286U, "AllowedSendersEmptyLabelForEndUser");
			OwaOptionStrings.stringIDs.Add(3699558192U, "RequirementsReadWriteMailboxValue");
			OwaOptionStrings.stringIDs.Add(2323655060U, "FlagStatusDialogLabel");
			OwaOptionStrings.stringIDs.Add(993324455U, "ImapSetting");
			OwaOptionStrings.stringIDs.Add(1286082276U, "VoicemailConfiguredText");
			OwaOptionStrings.stringIDs.Add(517504798U, "JunkEmailTrustedListHeader");
			OwaOptionStrings.stringIDs.Add(1255618141U, "RequirementsReadItemDescription");
			OwaOptionStrings.stringIDs.Add(311875894U, "RequireSenderAuth");
			OwaOptionStrings.stringIDs.Add(2656242830U, "IWantToEditMyNotificationSettings");
			OwaOptionStrings.stringIDs.Add(4223818683U, "DevicePolicyAppliedLabel");
			OwaOptionStrings.stringIDs.Add(980863707U, "VoicemailSMSOptionNone");
			OwaOptionStrings.stringIDs.Add(4218310877U, "EditDistributionGroupTitle");
			OwaOptionStrings.stringIDs.Add(1540907116U, "MaximumDurationInMinutes");
			OwaOptionStrings.stringIDs.Add(3482669506U, "NewPopSubscription");
			OwaOptionStrings.stringIDs.Add(3896686788U, "MobileDeviceHeadNoteInfo");
			OwaOptionStrings.stringIDs.Add(4151878805U, "HomePhoneLabel");
			OwaOptionStrings.stringIDs.Add(2680900463U, "BlockDeviceCommandText");
			OwaOptionStrings.stringIDs.Add(572559498U, "SelectUsers");
			OwaOptionStrings.stringIDs.Add(3849562096U, "SearchGroupsButtonDescription");
			OwaOptionStrings.stringIDs.Add(382247602U, "DeleteEmailSubscriptionsConfirmation");
			OwaOptionStrings.stringIDs.Add(844022153U, "LearnHowToUseRedirectTo");
			OwaOptionStrings.stringIDs.Add(1008265668U, "InboxRuleDeleteMessageActionFlyOutText");
			OwaOptionStrings.stringIDs.Add(187395633U, "RetentionExplanationLabel");
			OwaOptionStrings.stringIDs.Add(2136591767U, "MessageTrackingPendingEvent");
			OwaOptionStrings.stringIDs.Add(773553315U, "ProviderColumn");
			OwaOptionStrings.stringIDs.Add(718036160U, "SettingAccessDisabled");
			OwaOptionStrings.stringIDs.Add(1339697241U, "RPTDays");
			OwaOptionStrings.stringIDs.Add(433722170U, "InboxRuleFromSubscriptionConditionFlyOutText");
			OwaOptionStrings.stringIDs.Add(2080149828U, "SmtpAddressExample");
			OwaOptionStrings.stringIDs.Add(759967779U, "InboxRuleMarkImportanceActionFlyOutText");
			OwaOptionStrings.stringIDs.Add(3100350500U, "CategoryDialogLabel");
			OwaOptionStrings.stringIDs.Add(3531743005U, "DeviceAccessStateLabel");
			OwaOptionStrings.stringIDs.Add(854461327U, "JunkEmailBlockedListHeader");
			OwaOptionStrings.stringIDs.Add(3007171092U, "VoicemailNotAvailableText");
			OwaOptionStrings.stringIDs.Add(290121153U, "ContactLocationBookmark");
			OwaOptionStrings.stringIDs.Add(1437070541U, "InboxRuleSubjectContainsConditionText");
			OwaOptionStrings.stringIDs.Add(130154415U, "VoicemailWizardConfirmPinLabel");
			OwaOptionStrings.stringIDs.Add(1880859719U, "NoSubscriptionAvailable");
			OwaOptionStrings.stringIDs.Add(2399040354U, "QLPassword");
			OwaOptionStrings.stringIDs.Add(540558863U, "CustomAccessRightRole");
			OwaOptionStrings.stringIDs.Add(570023042U, "ClearSettings");
			OwaOptionStrings.stringIDs.Add(2696088286U, "MailboxUsageUnitKB");
			OwaOptionStrings.stringIDs.Add(3947650662U, "ExtensionPackageLocation");
			OwaOptionStrings.stringIDs.Add(3084881239U, "InboxRuleApplyCategoryActionText");
			OwaOptionStrings.stringIDs.Add(2226605436U, "OWAVoicemail");
			OwaOptionStrings.stringIDs.Add(853755201U, "PersonalSettingPassword");
			OwaOptionStrings.stringIDs.Add(726021877U, "SendDuringWorkHoursOnly");
			OwaOptionStrings.stringIDs.Add(677776990U, "AliasLabelForDataCenter");
			OwaOptionStrings.stringIDs.Add(1351408741U, "DeliverAndForward");
			OwaOptionStrings.stringIDs.Add(468777496U, "Language");
			OwaOptionStrings.stringIDs.Add(3918450461U, "NameAndAccountBookmark");
			OwaOptionStrings.stringIDs.Add(2977885649U, "SendMyPhoneColon");
			OwaOptionStrings.stringIDs.Add(2956146252U, "DateStyles");
			OwaOptionStrings.stringIDs.Add(4067290517U, "GroupsIBelongToDescription");
			OwaOptionStrings.stringIDs.Add(2161945740U, "StartTimeText");
			OwaOptionStrings.stringIDs.Add(91158280U, "RemoveCommandText");
			OwaOptionStrings.stringIDs.Add(1057459323U, "PersonalSettingChangePassword");
			OwaOptionStrings.stringIDs.Add(3598785721U, "PasswordLabel");
			OwaOptionStrings.stringIDs.Add(1986416405U, "InboxRuleHasAttachmentConditionText");
			OwaOptionStrings.stringIDs.Add(988028286U, "CalendarPublishingLinks");
			OwaOptionStrings.stringIDs.Add(3784078605U, "RequirementsReadWriteItemValue");
			OwaOptionStrings.stringIDs.Add(1710677402U, "ImportContactListProgress");
			OwaOptionStrings.stringIDs.Add(874684615U, "TrialReminderActionLinkText");
			OwaOptionStrings.stringIDs.Add(2040917585U, "VoiceMailNotificationsLink");
			OwaOptionStrings.stringIDs.Add(856915648U, "InboxRuleMyNameInToCcBoxConditionText");
			OwaOptionStrings.stringIDs.Add(2736378449U, "InstallFromFile");
			OwaOptionStrings.stringIDs.Add(2000126950U, "DeviceMOWAVersionLabel");
			OwaOptionStrings.stringIDs.Add(1732412034U, "Subject");
			OwaOptionStrings.stringIDs.Add(1266539147U, "EditAccountCommandText");
			OwaOptionStrings.stringIDs.Add(3491325124U, "AutomaticRepliesScheduledCheckboxText");
			OwaOptionStrings.stringIDs.Add(3616424413U, "RetentionActionTypeHeader");
			OwaOptionStrings.stringIDs.Add(3507358566U, "QLOutlook");
			OwaOptionStrings.stringIDs.Add(3300930250U, "InboxRuleFromConditionText");
			OwaOptionStrings.stringIDs.Add(582625299U, "MessageTrackingFailedEvent");
			OwaOptionStrings.stringIDs.Add(1068599355U, "IncomingSecurityTLS");
			OwaOptionStrings.stringIDs.Add(961444931U, "WithinSizeRangeConditionFormat");
			OwaOptionStrings.stringIDs.Add(2118875233U, "VoicemailCallFwdStep3");
			OwaOptionStrings.stringIDs.Add(3710804460U, "MyMailbox");
			OwaOptionStrings.stringIDs.Add(2534471784U, "CalendarPublishingDetail");
			OwaOptionStrings.stringIDs.Add(1753433743U, "DeviceLastSuccessfulSyncLabel");
			OwaOptionStrings.stringIDs.Add(1279019904U, "ClearButtonText");
			OwaOptionStrings.stringIDs.Add(1541555783U, "InboxRuleCopyToFolderActionText");
			OwaOptionStrings.stringIDs.Add(1806917741U, "RPTYearsMonths");
			OwaOptionStrings.stringIDs.Add(3714415956U, "FromConditionFormat");
			OwaOptionStrings.stringIDs.Add(908779566U, "DeviceUserAgentLabel");
			OwaOptionStrings.stringIDs.Add(2657380710U, "DepartGroupConfirmation");
			OwaOptionStrings.stringIDs.Add(2291011248U, "InboxRuleWithImportanceConditionText");
			OwaOptionStrings.stringIDs.Add(492355799U, "PersonalSettingDomainUser");
			OwaOptionStrings.stringIDs.Add(1452418752U, "SiteMailboxEmailMeDiagnosticsButtonString");
			OwaOptionStrings.stringIDs.Add(2174687123U, "DeliveryManagement");
			OwaOptionStrings.stringIDs.Add(1249600476U, "PersonalSettingPasswordBeforeChange");
			OwaOptionStrings.stringIDs.Add(1511367726U, "SmtpSetting");
			OwaOptionStrings.stringIDs.Add(1472422U, "MessageFormatSlab");
			OwaOptionStrings.stringIDs.Add(3994376130U, "InboxRuleForwardToActionFlyOutText");
			OwaOptionStrings.stringIDs.Add(3663958361U, "DuplicateProxyAddressError");
			OwaOptionStrings.stringIDs.Add(2209887359U, "CreatedBy");
			OwaOptionStrings.stringIDs.Add(2999683987U, "ReadReceiptResponseNever");
			OwaOptionStrings.stringIDs.Add(992729393U, "AccessDeniedFooterBottom");
			OwaOptionStrings.stringIDs.Add(900932107U, "OwnerChangedUpdateModerator");
			OwaOptionStrings.stringIDs.Add(1233400920U, "AllowRecurringMeetings");
			OwaOptionStrings.stringIDs.Add(508731823U, "VoicemailWizardStep1Title");
			OwaOptionStrings.stringIDs.Add(2265850499U, "TeamMailboxMembersString");
			OwaOptionStrings.stringIDs.Add(417105304U, "AfterDateDisplayTemplate");
			OwaOptionStrings.stringIDs.Add(1413204805U, "TuesdayCheckBoxText");
			OwaOptionStrings.stringIDs.Add(1367192090U, "Join");
			OwaOptionStrings.stringIDs.Add(2408804455U, "AddAdditionalResponse");
			OwaOptionStrings.stringIDs.Add(737660655U, "OkButtonText");
			OwaOptionStrings.stringIDs.Add(3175109089U, "FoldersSyncedLabel");
			OwaOptionStrings.stringIDs.Add(3789286439U, "SendToAllText");
			OwaOptionStrings.stringIDs.Add(3957821083U, "MaximumDurationInMinutesErrorMessage");
			OwaOptionStrings.stringIDs.Add(3260418603U, "TextMessagingTurnedOnViaEas");
			OwaOptionStrings.stringIDs.Add(3047217661U, "StartLoggingCommandText");
			OwaOptionStrings.stringIDs.Add(2811760173U, "MailboxUsageLegacyText");
			OwaOptionStrings.stringIDs.Add(1144080682U, "RPTYears");
			OwaOptionStrings.stringIDs.Add(143410217U, "ReadReceiptResponseInstruction");
			OwaOptionStrings.stringIDs.Add(636289130U, "RemindersEnabled");
			OwaOptionStrings.stringIDs.Add(1284433590U, "AddNewRequestsTentativelyCheckBoxText");
			OwaOptionStrings.stringIDs.Add(1865553596U, "MessageTrackingSubmitEvent");
			OwaOptionStrings.stringIDs.Add(1981955278U, "VoicemailLearnMore");
			OwaOptionStrings.stringIDs.Add(2288506116U, "EmailThisReport");
			OwaOptionStrings.stringIDs.Add(1435311110U, "CalendarPublishingDateRange");
			OwaOptionStrings.stringIDs.Add(3485844721U, "SelectUsersAndGroups");
			OwaOptionStrings.stringIDs.Add(1183459890U, "PhotoBookmark");
			OwaOptionStrings.stringIDs.Add(2440572518U, "InboxRuleHeaderContainsConditionText");
			OwaOptionStrings.stringIDs.Add(1356144580U, "BookingWindowInDaysErrorMessage");
			OwaOptionStrings.stringIDs.Add(1292798904U, "Calendar");
			OwaOptionStrings.stringIDs.Add(1152863492U, "RuleSubjectContainsAndMoveToFolderTemplate");
			OwaOptionStrings.stringIDs.Add(817924740U, "RuleStateOn");
			OwaOptionStrings.stringIDs.Add(3913001929U, "UserLogonNameLabel");
			OwaOptionStrings.stringIDs.Add(2426358350U, "RPTMonth");
			OwaOptionStrings.stringIDs.Add(1824826658U, "EndTime");
			OwaOptionStrings.stringIDs.Add(2917434001U, "InstallFromPrivateUrlTitle");
			OwaOptionStrings.stringIDs.Add(3131435199U, "LastSyncAttemptTimeHeaderText");
			OwaOptionStrings.stringIDs.Add(2572011012U, "RequirementsReadWriteItemDescription");
			OwaOptionStrings.stringIDs.Add(940101428U, "NewItemNotificationVoiceMailToast");
			OwaOptionStrings.stringIDs.Add(2853482896U, "RenameDefaultFoldersCheckBoxText");
			OwaOptionStrings.stringIDs.Add(1380850475U, "InboxRuleFromSubscriptionConditionText");
			OwaOptionStrings.stringIDs.Add(1440211402U, "RetentionSelectOptionalLabel");
			OwaOptionStrings.stringIDs.Add(1914666238U, "FromColumnLabel");
			OwaOptionStrings.stringIDs.Add(524522579U, "ExtensionCanNotBeDisabledNorUninstalled");
			OwaOptionStrings.stringIDs.Add(2157690544U, "CalendarPublishingPublic");
			OwaOptionStrings.stringIDs.Add(1380849357U, "CalendarSharingExplanation");
			OwaOptionStrings.stringIDs.Add(1525092013U, "CalendarPublishingViewUrl");
			OwaOptionStrings.stringIDs.Add(3391167931U, "SaturdayCheckBoxText");
			OwaOptionStrings.stringIDs.Add(1194500941U, "MailboxUsageUnitB");
			OwaOptionStrings.stringIDs.Add(2771220587U, "HasAttachmentConditionFormat");
			OwaOptionStrings.stringIDs.Add(2993887811U, "VoicemailWizardStep5Title");
			OwaOptionStrings.stringIDs.Add(3866819705U, "PreviewMarkAsReadBehaviorNever");
			OwaOptionStrings.stringIDs.Add(4136480461U, "SubscriptionDialogLabel");
			OwaOptionStrings.stringIDs.Add(1033615903U, "NewSubscription");
			OwaOptionStrings.stringIDs.Add(826260386U, "LastSynchronization");
			OwaOptionStrings.stringIDs.Add(1189779660U, "ChangeCalendarPermissions");
			OwaOptionStrings.stringIDs.Add(3183463582U, "DefaultReminderTimeLabel");
			OwaOptionStrings.stringIDs.Add(2947609428U, "OpenNextItem");
			OwaOptionStrings.stringIDs.Add(3213113944U, "EmailOptions");
			OwaOptionStrings.stringIDs.Add(461256164U, "RetentionActionTypeDefaultDelete");
			OwaOptionStrings.stringIDs.Add(3032482008U, "ChangeCommandText");
			OwaOptionStrings.stringIDs.Add(1054605049U, "ExternalMessageGalLessInstruction");
			OwaOptionStrings.stringIDs.Add(508072824U, "ImportContactListNoFileUploaded");
			OwaOptionStrings.stringIDs.Add(1294601601U, "BadOfficeCallbackMessage");
			OwaOptionStrings.stringIDs.Add(2511894476U, "DefaultImage");
			OwaOptionStrings.stringIDs.Add(393831119U, "JoinAndDepart");
			OwaOptionStrings.stringIDs.Add(3881794065U, "InboxRuleMyNameInToCcBoxConditionFlyOutText");
			OwaOptionStrings.stringIDs.Add(2202590644U, "TeamMailboxTitleString");
			OwaOptionStrings.stringIDs.Add(1746211700U, "NewestOnTop");
			OwaOptionStrings.stringIDs.Add(2730942909U, "ToWithExample");
			OwaOptionStrings.stringIDs.Add(1571878129U, "GroupOwners");
			OwaOptionStrings.stringIDs.Add(1456975951U, "DefaultRetentionTagDescription");
			OwaOptionStrings.stringIDs.Add(2811696355U, "Hours");
			OwaOptionStrings.stringIDs.Add(643576831U, "MessageTypeDialogTitle");
			OwaOptionStrings.stringIDs.Add(3392276543U, "SearchDeliveryReports");
			OwaOptionStrings.stringIDs.Add(2118875235U, "VoicemailCallFwdStep1");
			OwaOptionStrings.stringIDs.Add(4130400892U, "SendAddressSetting");
			OwaOptionStrings.stringIDs.Add(787898729U, "InboxRuleItIsGroupText");
			OwaOptionStrings.stringIDs.Add(1694176600U, "LaunchOfficeMarketplace");
			OwaOptionStrings.stringIDs.Add(534802423U, "CalendarDiagnosticLogDescription");
			OwaOptionStrings.stringIDs.Add(1084745363U, "InboxRules");
			OwaOptionStrings.stringIDs.Add(942329033U, "VoicemailSMSOptionVoiceMailAndMissedCalls");
			OwaOptionStrings.stringIDs.Add(2138448017U, "StartTime");
			OwaOptionStrings.stringIDs.Add(1074402166U, "TextMessagingSlabMessage");
			OwaOptionStrings.stringIDs.Add(1548729791U, "WipeDeviceConfirmMessageDetail");
			OwaOptionStrings.stringIDs.Add(2711159495U, "VoicemailWizardStep2DescriptionNoPasscode");
			OwaOptionStrings.stringIDs.Add(3410748733U, "DeleteInboxRulesConfirmation");
			OwaOptionStrings.stringIDs.Add(3782284348U, "CalendarNotificationsLink");
			OwaOptionStrings.stringIDs.Add(2245512691U, "InboxRuleHasClassificationConditionFlyOutText");
			OwaOptionStrings.stringIDs.Add(3223016465U, "CalendarWorkflowInstruction");
			OwaOptionStrings.stringIDs.Add(672959953U, "AutomaticRepliesInstruction");
			OwaOptionStrings.stringIDs.Add(3095426072U, "DeviceLanguageLabel");
			OwaOptionStrings.stringIDs.Add(137938150U, "MoveUp");
			OwaOptionStrings.stringIDs.Add(810469194U, "InstallButtonText");
			OwaOptionStrings.stringIDs.Add(3324278371U, "WithSensitivityConditionFormat");
			OwaOptionStrings.stringIDs.Add(434719229U, "VoicemailWizardPinLabel");
			OwaOptionStrings.stringIDs.Add(2560210044U, "MondayCheckBoxText");
			OwaOptionStrings.stringIDs.Add(984832422U, "WithImportanceConditionFormat");
			OwaOptionStrings.stringIDs.Add(2489355482U, "NewDistributionGroupText");
			OwaOptionStrings.stringIDs.Add(1716421970U, "CalendarReminderSlab");
			OwaOptionStrings.stringIDs.Add(2879758792U, "EmailAddressesLabel");
			OwaOptionStrings.stringIDs.Add(1512647751U, "PortLabel");
			OwaOptionStrings.stringIDs.Add(2405760124U, "SetupVoiceMailNotificationsLink");
			OwaOptionStrings.stringIDs.Add(1075596584U, "TextMessagingOff");
			OwaOptionStrings.stringIDs.Add(602937486U, "RuleSentToAndMoveToFolderTemplate");
			OwaOptionStrings.stringIDs.Add(1393180149U, "UserNameNotSetError");
			OwaOptionStrings.stringIDs.Add(1065442140U, "MailboxUsageWarningText");
			OwaOptionStrings.stringIDs.Add(2112942921U, "PopSetting");
			OwaOptionStrings.stringIDs.Add(4023555694U, "InboxRuleFromAddressContainsConditionFlyOutText");
			OwaOptionStrings.stringIDs.Add(1511584348U, "Options");
			OwaOptionStrings.stringIDs.Add(3474229968U, "EMailSignatureSlab");
			OwaOptionStrings.stringIDs.Add(3820898101U, "RetentionPeriodHoldFor");
			OwaOptionStrings.stringIDs.Add(1362687599U, "MaximumConflictInstancesErrorMessage");
			OwaOptionStrings.stringIDs.Add(3952282363U, "Organize");
			OwaOptionStrings.stringIDs.Add(4129994431U, "MessageTrackingDLExpandedEvent");
			OwaOptionStrings.stringIDs.Add(1620472852U, "VoicemailBrowserNotSupported");
			OwaOptionStrings.stringIDs.Add(1877028846U, "SendAddressSettingSlabDescription");
			OwaOptionStrings.stringIDs.Add(2765871439U, "RetrieveLogConfirmMessage");
			OwaOptionStrings.stringIDs.Add(460637234U, "DescriptionLabel");
			OwaOptionStrings.stringIDs.Add(3776153215U, "CustomAttributeDialogTitle");
			OwaOptionStrings.stringIDs.Add(527639998U, "MessageApproval");
			OwaOptionStrings.stringIDs.Add(931483573U, "TeamMailboxPropertiesString");
			OwaOptionStrings.stringIDs.Add(2650315815U, "BodyContainsConditionFormat");
			OwaOptionStrings.stringIDs.Add(3936874395U, "GroupModeration");
			OwaOptionStrings.stringIDs.Add(2556623818U, "FreeBusySubjectLocationInformation");
			OwaOptionStrings.stringIDs.Add(3000766872U, "InboxRuleMarkMessageGroupText");
			OwaOptionStrings.stringIDs.Add(2673767273U, "InboxRuleConditionSectionHeader");
			OwaOptionStrings.stringIDs.Add(3741421195U, "RecipientEmailButtonDescription");
			OwaOptionStrings.stringIDs.Add(1779041463U, "TimeZone");
			OwaOptionStrings.stringIDs.Add(286364476U, "InboxRuleSentToConditionFlyOutText");
			OwaOptionStrings.stringIDs.Add(3496306419U, "MoveDown");
			OwaOptionStrings.stringIDs.Add(105290154U, "UnblockDeviceCommandText");
			OwaOptionStrings.stringIDs.Add(3851792428U, "TextMessagingSms");
			OwaOptionStrings.stringIDs.Add(3338384120U, "LanguageInstruction");
			OwaOptionStrings.stringIDs.Add(2653201341U, "InboxRuleFromConditionFlyOutText");
			OwaOptionStrings.stringIDs.Add(773833243U, "OutOfOffice");
			OwaOptionStrings.stringIDs.Add(2449687995U, "FirstNameLabel");
			OwaOptionStrings.stringIDs.Add(271571182U, "InboxRuleBodyContainsConditionFlyOutText");
			OwaOptionStrings.stringIDs.Add(2979279075U, "AllowedSendersLabelForEndUser");
			OwaOptionStrings.stringIDs.Add(3600235422U, "VoicemailWizardStep2Title");
			OwaOptionStrings.stringIDs.Add(2483845882U, "ConversationsSlab");
			OwaOptionStrings.stringIDs.Add(3601145337U, "AutomaticRepliesSlab");
			OwaOptionStrings.stringIDs.Add(1137051325U, "IncomingAuthenticationNtlm");
			OwaOptionStrings.stringIDs.Add(1448959650U, "MobilePhoneLabel");
			OwaOptionStrings.stringIDs.Add(2211346257U, "DeviceNameLabel");
			OwaOptionStrings.stringIDs.Add(2946311768U, "HeaderContainsConditionFormat");
			OwaOptionStrings.stringIDs.Add(944969227U, "TeamMailboxDescription");
			OwaOptionStrings.stringIDs.Add(2267981074U, "EnterNumberClickNext");
			OwaOptionStrings.stringIDs.Add(3063130671U, "MobileDevices");
			OwaOptionStrings.stringIDs.Add(1188277387U, "DisplayRecoveryPasswordCommandDescription");
			OwaOptionStrings.stringIDs.Add(3451245278U, "AtMostOnlyDisplayTemplate");
			OwaOptionStrings.stringIDs.Add(777656741U, "InboxRuleSentToConditionPreCannedText");
			OwaOptionStrings.stringIDs.Add(2303439412U, "InboxRuleMarkedWithGroupText");
			OwaOptionStrings.stringIDs.Add(3455154594U, "Membership");
			OwaOptionStrings.stringIDs.Add(31525559U, "VoicemailSlab");
			OwaOptionStrings.stringIDs.Add(2801441113U, "AllInformation");
			OwaOptionStrings.stringIDs.Add(3723544685U, "BlockOrAllow");
			OwaOptionStrings.stringIDs.Add(1716779644U, "CalendarDiagnosticLogWatermarkText");
			OwaOptionStrings.stringIDs.Add(2601540438U, "EditGroups");
			OwaOptionStrings.stringIDs.Add(3335990695U, "TurnOnTextMessaging");
			OwaOptionStrings.stringIDs.Add(1418080636U, "AliasLabelForEnterprise");
			OwaOptionStrings.stringIDs.Add(3019822456U, "CategoryDialogTitle");
			OwaOptionStrings.stringIDs.Add(1806349341U, "SetYourWorkingHours");
			OwaOptionStrings.stringIDs.Add(3830590072U, "ProfileMailboxUsage");
			OwaOptionStrings.stringIDs.Add(1985922850U, "AtLeastAtMostDisplayTemplate");
			OwaOptionStrings.stringIDs.Add(1502911297U, "NotificationsForMeetingReminders");
			OwaOptionStrings.stringIDs.Add(1650094581U, "IncomingServerLabel");
			OwaOptionStrings.stringIDs.Add(3052975740U, "IncomingAuthenticationNone");
			OwaOptionStrings.stringIDs.Add(1925535397U, "TrialReminderText");
			OwaOptionStrings.stringIDs.Add(783706065U, "GroupsIBelongToAndGroupsIOwnDescription");
		}

		public static LocalizedString NeverSyncText
		{
			get
			{
				return new LocalizedString("NeverSyncText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FromAddressContainsConditionFormat
		{
			get
			{
				return new LocalizedString("FromAddressContainsConditionFormat", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarPublishingBasic
		{
			get
			{
				return new LocalizedString("CalendarPublishingBasic", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ChangePhoneNumber
		{
			get
			{
				return new LocalizedString("ChangePhoneNumber", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetSubscriptionSucceed(string feedback)
		{
			return new LocalizedString("SetSubscriptionSucceed", "", false, false, OwaOptionStrings.ResourceManager, new object[]
			{
				feedback
			});
		}

		public static LocalizedString TimeZoneNote
		{
			get
			{
				return new LocalizedString("TimeZoneNote", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShowWorkWeekAsCheckBoxText
		{
			get
			{
				return new LocalizedString("ShowWorkWeekAsCheckBoxText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ViewInboxRule
		{
			get
			{
				return new LocalizedString("ViewInboxRule", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceMobileOperatorLabel
		{
			get
			{
				return new LocalizedString("DeviceMobileOperatorLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FinishButtonText
		{
			get
			{
				return new LocalizedString("FinishButtonText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserNameMOSIDLabel
		{
			get
			{
				return new LocalizedString("UserNameMOSIDLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ChangeMyMobilePhoneSettings
		{
			get
			{
				return new LocalizedString("ChangeMyMobilePhoneSettings", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequirementsReadWriteMailboxDescription
		{
			get
			{
				return new LocalizedString("RequirementsReadWriteMailboxDescription", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTypeMatchesConditionFormat
		{
			get
			{
				return new LocalizedString("MessageTypeMatchesConditionFormat", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NewRoomCreationWarningText
		{
			get
			{
				return new LocalizedString("NewRoomCreationWarningText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMyNameInToBoxConditionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleMyNameInToBoxConditionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FirstSyncOnLabel
		{
			get
			{
				return new LocalizedString("FirstSyncOnLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeleteGroupConfirmation
		{
			get
			{
				return new LocalizedString("DeleteGroupConfirmation", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PendingWipeCommandIssuedLabel
		{
			get
			{
				return new LocalizedString("PendingWipeCommandIssuedLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OwnerChangedModerationReminder
		{
			get
			{
				return new LocalizedString("OwnerChangedModerationReminder", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleFromConditionPreCannedText
		{
			get
			{
				return new LocalizedString("InboxRuleFromConditionPreCannedText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MaximumConflictInstances
		{
			get
			{
				return new LocalizedString("MaximumConflictInstances", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmailSubscriptions
		{
			get
			{
				return new LocalizedString("EmailSubscriptions", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleFlaggedForActionConditionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleFlaggedForActionConditionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ViewRPTDurationLabel
		{
			get
			{
				return new LocalizedString("ViewRPTDurationLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OnOffColumn
		{
			get
			{
				return new LocalizedString("OnOffColumn", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FromSubscriptionConditionFormat
		{
			get
			{
				return new LocalizedString("FromSubscriptionConditionFormat", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmailComposeModeSeparateForm
		{
			get
			{
				return new LocalizedString("EmailComposeModeSeparateForm", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReadingPaneSlab
		{
			get
			{
				return new LocalizedString("ReadingPaneSlab", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OOF
		{
			get
			{
				return new LocalizedString("OOF", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchResultsCaption
		{
			get
			{
				return new LocalizedString("SearchResultsCaption", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubjectLabel
		{
			get
			{
				return new LocalizedString("SubjectLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Minute
		{
			get
			{
				return new LocalizedString("Minute", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SendAtColon
		{
			get
			{
				return new LocalizedString("SendAtColon", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NewCommandText
		{
			get
			{
				return new LocalizedString("NewCommandText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RetentionTypeRequired
		{
			get
			{
				return new LocalizedString("RetentionTypeRequired", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UninstallExtensionsConfirmation
		{
			get
			{
				return new LocalizedString("UninstallExtensionsConfirmation", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleHasAttachmentConditionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleHasAttachmentConditionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MyselfEntFormat
		{
			get
			{
				return new LocalizedString("MyselfEntFormat", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectedAccounts
		{
			get
			{
				return new LocalizedString("ConnectedAccounts", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupNotes
		{
			get
			{
				return new LocalizedString("GroupNotes", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Status
		{
			get
			{
				return new LocalizedString("Status", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxSyncStatusString
		{
			get
			{
				return new LocalizedString("TeamMailboxSyncStatusString", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AccountSecondaryNavigation
		{
			get
			{
				return new LocalizedString("AccountSecondaryNavigation", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmailSubscriptionSlabDescription
		{
			get
			{
				return new LocalizedString("EmailSubscriptionSlabDescription", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxMailString
		{
			get
			{
				return new LocalizedString("TeamMailboxMailString", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxLifecycleStatusString2
		{
			get
			{
				return new LocalizedString("TeamMailboxLifecycleStatusString2", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImportContactListPage2ResultNumber(int numContacts)
		{
			return new LocalizedString("ImportContactListPage2ResultNumber", "", false, false, OwaOptionStrings.ResourceManager, new object[]
			{
				numContacts
			});
		}

		public static LocalizedString JunkEmailTrustedListDescription
		{
			get
			{
				return new LocalizedString("JunkEmailTrustedListDescription", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SundayCheckBoxText
		{
			get
			{
				return new LocalizedString("SundayCheckBoxText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExtensionVersionTag
		{
			get
			{
				return new LocalizedString("ExtensionVersionTag", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailTip
		{
			get
			{
				return new LocalizedString("MailTip", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarPublishingRestricted
		{
			get
			{
				return new LocalizedString("CalendarPublishingRestricted", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JoinDlsSuccess(int num)
		{
			return new LocalizedString("JoinDlsSuccess", "", false, false, OwaOptionStrings.ResourceManager, new object[]
			{
				num
			});
		}

		public static LocalizedString MailboxUsageUnavailable
		{
			get
			{
				return new LocalizedString("MailboxUsageUnavailable", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Customize
		{
			get
			{
				return new LocalizedString("Customize", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ModerationEnabled
		{
			get
			{
				return new LocalizedString("ModerationEnabled", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PreviewMarkAsReadBehaviorDelayed
		{
			get
			{
				return new LocalizedString("PreviewMarkAsReadBehaviorDelayed", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShareInformation
		{
			get
			{
				return new LocalizedString("ShareInformation", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RetentionActionTypeArchive
		{
			get
			{
				return new LocalizedString("RetentionActionTypeArchive", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetUpNotifications
		{
			get
			{
				return new LocalizedString("SetUpNotifications", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMoveToFolderActionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleMoveToFolderActionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmailContactsTrusted
		{
			get
			{
				return new LocalizedString("JunkEmailContactsTrusted", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxManagementString
		{
			get
			{
				return new LocalizedString("TeamMailboxManagementString", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTrackingTransferredEvent
		{
			get
			{
				return new LocalizedString("MessageTrackingTransferredEvent", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SendToAllGalLessText
		{
			get
			{
				return new LocalizedString("SendToAllGalLessText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarReminderInstruction
		{
			get
			{
				return new LocalizedString("CalendarReminderInstruction", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TotalMembers
		{
			get
			{
				return new LocalizedString("TotalMembers", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxUsageUnlimitedText
		{
			get
			{
				return new LocalizedString("MailboxUsageUnlimitedText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarTroubleshootingLinkText
		{
			get
			{
				return new LocalizedString("CalendarTroubleshootingLinkText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DisplayRecoveryPasswordCommandText
		{
			get
			{
				return new LocalizedString("DisplayRecoveryPasswordCommandText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailWizardStep4Description
		{
			get
			{
				return new LocalizedString("VoicemailWizardStep4Description", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncomingSecurityLabel
		{
			get
			{
				return new LocalizedString("IncomingSecurityLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleForwardToActionText
		{
			get
			{
				return new LocalizedString("InboxRuleForwardToActionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMyNameIsGroupText
		{
			get
			{
				return new LocalizedString("InboxRuleMyNameIsGroupText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxFolderDialogLabel
		{
			get
			{
				return new LocalizedString("MailboxFolderDialogLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReturnToView
		{
			get
			{
				return new LocalizedString("ReturnToView", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceActiveSyncVersionLabel
		{
			get
			{
				return new LocalizedString("DeviceActiveSyncVersionLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstallFromPrivateUrlCaption
		{
			get
			{
				return new LocalizedString("InstallFromPrivateUrlCaption", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeleteEmailSubscriptionConfirmation
		{
			get
			{
				return new LocalizedString("DeleteEmailSubscriptionConfirmation", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailWizardComplete
		{
			get
			{
				return new LocalizedString("VoicemailWizardComplete", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMarkAsReadActionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleMarkAsReadActionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RPTDay
		{
			get
			{
				return new LocalizedString("RPTDay", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceAccessStateSetByLabel
		{
			get
			{
				return new LocalizedString("DeviceAccessStateSetByLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ViewGroupDetails
		{
			get
			{
				return new LocalizedString("ViewGroupDetails", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ToOnlyLabel
		{
			get
			{
				return new LocalizedString("ToOnlyLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SensitivityDialogTitle
		{
			get
			{
				return new LocalizedString("SensitivityDialogTitle", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxLifecycleStatusString
		{
			get
			{
				return new LocalizedString("TeamMailboxLifecycleStatusString", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WednesdayCheckBoxText
		{
			get
			{
				return new LocalizedString("WednesdayCheckBoxText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExtensionRequirementsLabel
		{
			get
			{
				return new LocalizedString("ExtensionRequirementsLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlwaysShowBcc
		{
			get
			{
				return new LocalizedString("AlwaysShowBcc", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConflictPercentageAllowedErrorMessage
		{
			get
			{
				return new LocalizedString("ConflictPercentageAllowedErrorMessage", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JoinRestrictionApprovalRequiredDetails
		{
			get
			{
				return new LocalizedString("JoinRestrictionApprovalRequiredDetails", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMarkImportanceActionText
		{
			get
			{
				return new LocalizedString("InboxRuleMarkImportanceActionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleRecipientAddressContainsConditionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleRecipientAddressContainsConditionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Regional
		{
			get
			{
				return new LocalizedString("Regional", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailWizardTestDoneText
		{
			get
			{
				return new LocalizedString("VoicemailWizardTestDoneText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoveOldMeetingMessagesCheckBoxText
		{
			get
			{
				return new LocalizedString("RemoveOldMeetingMessagesCheckBoxText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleBodyContainsConditionText
		{
			get
			{
				return new LocalizedString("InboxRuleBodyContainsConditionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QLForward
		{
			get
			{
				return new LocalizedString("QLForward", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailPhoneNumberColon
		{
			get
			{
				return new LocalizedString("VoicemailPhoneNumberColon", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddCommandText
		{
			get
			{
				return new LocalizedString("AddCommandText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Voicemail
		{
			get
			{
				return new LocalizedString("Voicemail", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StringArrayDialogTitle
		{
			get
			{
				return new LocalizedString("StringArrayDialogTitle", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxUsageUnitTB
		{
			get
			{
				return new LocalizedString("MailboxUsageUnitTB", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarPublishingCopyLink
		{
			get
			{
				return new LocalizedString("CalendarPublishingCopyLink", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeStyles
		{
			get
			{
				return new LocalizedString("TimeStyles", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RPTYear
		{
			get
			{
				return new LocalizedString("RPTYear", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailLearnMoreVideo
		{
			get
			{
				return new LocalizedString("VoicemailLearnMoreVideo", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleHeaderContainsConditionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleHeaderContainsConditionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleHasClassificationConditionText
		{
			get
			{
				return new LocalizedString("InboxRuleHasClassificationConditionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImportContactListPage1Caption
		{
			get
			{
				return new LocalizedString("ImportContactListPage1Caption", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailWizardStep2Description
		{
			get
			{
				return new LocalizedString("VoicemailWizardStep2Description", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddExtension
		{
			get
			{
				return new LocalizedString("AddExtension", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WithinSizeRangeDialogTitle
		{
			get
			{
				return new LocalizedString("WithinSizeRangeDialogTitle", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetCalendarLogButtonText
		{
			get
			{
				return new LocalizedString("GetCalendarLogButtonText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BlockDeviceConfirmMessage
		{
			get
			{
				return new LocalizedString("BlockDeviceConfirmMessage", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarPublishingRangeFrom
		{
			get
			{
				return new LocalizedString("CalendarPublishingRangeFrom", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EnterPasscodeStepMessage
		{
			get
			{
				return new LocalizedString("EnterPasscodeStepMessage", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailCallFwdHavingTrouble
		{
			get
			{
				return new LocalizedString("VoicemailCallFwdHavingTrouble", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StartForward
		{
			get
			{
				return new LocalizedString("StartForward", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailCallFwdStep2
		{
			get
			{
				return new LocalizedString("VoicemailCallFwdStep2", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JoinRestrictionOpenDetails
		{
			get
			{
				return new LocalizedString("JoinRestrictionOpenDetails", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncomingSecurityNone
		{
			get
			{
				return new LocalizedString("IncomingSecurityNone", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMyNameNotInToBoxConditionText
		{
			get
			{
				return new LocalizedString("InboxRuleMyNameNotInToBoxConditionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ChangePermissions
		{
			get
			{
				return new LocalizedString("ChangePermissions", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleCopyToFolderActionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleCopyToFolderActionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleSubjectContainsConditionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleSubjectContainsConditionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequirementsRestrictedValue
		{
			get
			{
				return new LocalizedString("RequirementsRestrictedValue", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleRedirectToActionText
		{
			get
			{
				return new LocalizedString("InboxRuleRedirectToActionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImportContactListPage1Step2
		{
			get
			{
				return new LocalizedString("ImportContactListPage1Step2", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmailWatermarkText
		{
			get
			{
				return new LocalizedString("JunkEmailWatermarkText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TextMessagingStatusPrefixStatus
		{
			get
			{
				return new LocalizedString("TextMessagingStatusPrefixStatus", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShowHoursIn
		{
			get
			{
				return new LocalizedString("ShowHoursIn", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DefaultFormat
		{
			get
			{
				return new LocalizedString("DefaultFormat", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubscriptionDialogTitle
		{
			get
			{
				return new LocalizedString("SubscriptionDialogTitle", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NewItemNotificationEmailToast
		{
			get
			{
				return new LocalizedString("NewItemNotificationEmailToast", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxTabUsersHelpString1
		{
			get
			{
				return new LocalizedString("TeamMailboxTabUsersHelpString1", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SchedulingPermissionsSlab
		{
			get
			{
				return new LocalizedString("SchedulingPermissionsSlab", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversationSortOrderInstruction
		{
			get
			{
				return new LocalizedString("ConversationSortOrderInstruction", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WipeDeviceCommandText
		{
			get
			{
				return new LocalizedString("WipeDeviceCommandText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleSentOrReceivedGroupText
		{
			get
			{
				return new LocalizedString("InboxRuleSentOrReceivedGroupText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Myself
		{
			get
			{
				return new LocalizedString("Myself", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NewestOnBottom
		{
			get
			{
				return new LocalizedString("NewestOnBottom", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NewItemNotificationFaxToast
		{
			get
			{
				return new LocalizedString("NewItemNotificationFaxToast", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmailComposeModeInline
		{
			get
			{
				return new LocalizedString("EmailComposeModeInline", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NewRuleString
		{
			get
			{
				return new LocalizedString("NewRuleString", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EditRuleCaption(string name)
		{
			return new LocalizedString("EditRuleCaption", "", false, false, OwaOptionStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString NoMessageCategoryAvailable
		{
			get
			{
				return new LocalizedString("NoMessageCategoryAvailable", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CurrentStatus
		{
			get
			{
				return new LocalizedString("CurrentStatus", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubscriptionProcessingError
		{
			get
			{
				return new LocalizedString("SubscriptionProcessingError", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StopAndRetrieveLogCommandText
		{
			get
			{
				return new LocalizedString("StopAndRetrieveLogCommandText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeIncrementThirtyMinutes
		{
			get
			{
				return new LocalizedString("TimeIncrementThirtyMinutes", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RetentionActionNeverMove
		{
			get
			{
				return new LocalizedString("RetentionActionNeverMove", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailMobileOperatorColon
		{
			get
			{
				return new LocalizedString("VoicemailMobileOperatorColon", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectedAccountsDescriptionForForwarding
		{
			get
			{
				return new LocalizedString("ConnectedAccountsDescriptionForForwarding", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StopForward
		{
			get
			{
				return new LocalizedString("StopForward", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FirstWeekOfYear
		{
			get
			{
				return new LocalizedString("FirstWeekOfYear", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RegionListLabel
		{
			get
			{
				return new LocalizedString("RegionListLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstallFromMarketplace
		{
			get
			{
				return new LocalizedString("InstallFromMarketplace", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RulesNameColumn
		{
			get
			{
				return new LocalizedString("RulesNameColumn", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceOSLabel
		{
			get
			{
				return new LocalizedString("DeviceOSLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerificationEmailFailedToSend(string emailAddress)
		{
			return new LocalizedString("VerificationEmailFailedToSend", "", false, false, OwaOptionStrings.ResourceManager, new object[]
			{
				emailAddress
			});
		}

		public static LocalizedString InboxRuleSentOnlyToMeConditionText
		{
			get
			{
				return new LocalizedString("InboxRuleSentOnlyToMeConditionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EditYourPassword
		{
			get
			{
				return new LocalizedString("EditYourPassword", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EnforceSchedulingHorizon
		{
			get
			{
				return new LocalizedString("EnforceSchedulingHorizon", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxManagementString2
		{
			get
			{
				return new LocalizedString("TeamMailboxManagementString2", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchMessageTipForIWUser
		{
			get
			{
				return new LocalizedString("SearchMessageTipForIWUser", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectedAccountsDescriptionForSubscription
		{
			get
			{
				return new LocalizedString("ConnectedAccountsDescriptionForSubscription", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QLManageOrganization
		{
			get
			{
				return new LocalizedString("QLManageOrganization", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JoinRestrictionApprovalRequired
		{
			get
			{
				return new LocalizedString("JoinRestrictionApprovalRequired", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExtensionCanNotBeUninstalled
		{
			get
			{
				return new LocalizedString("ExtensionCanNotBeUninstalled", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailWizardStep4Title
		{
			get
			{
				return new LocalizedString("VoicemailWizardStep4Title", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ViewExtensionDetails
		{
			get
			{
				return new LocalizedString("ViewExtensionDetails", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailCarrierRatesMayApply
		{
			get
			{
				return new LocalizedString("VoicemailCarrierRatesMayApply", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeliveryReports
		{
			get
			{
				return new LocalizedString("DeliveryReports", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AllRequestOutOfPolicyText
		{
			get
			{
				return new LocalizedString("AllRequestOutOfPolicyText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoveDeviceConfirmMessage
		{
			get
			{
				return new LocalizedString("RemoveDeviceConfirmMessage", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StatusLabel
		{
			get
			{
				return new LocalizedString("StatusLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleSubjectOrBodyContainsConditionText
		{
			get
			{
				return new LocalizedString("InboxRuleSubjectOrBodyContainsConditionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OwnerLabel
		{
			get
			{
				return new LocalizedString("OwnerLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequireSenderAuthFalse
		{
			get
			{
				return new LocalizedString("RequireSenderAuthFalse", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AllowedSendersLabel
		{
			get
			{
				return new LocalizedString("AllowedSendersLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncomingSecuritySSL
		{
			get
			{
				return new LocalizedString("IncomingSecuritySSL", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CarrierListLabel
		{
			get
			{
				return new LocalizedString("CarrierListLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDescriptionNote
		{
			get
			{
				return new LocalizedString("InboxRuleDescriptionNote", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NewImapSubscription
		{
			get
			{
				return new LocalizedString("NewImapSubscription", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxStartSyncButtonString
		{
			get
			{
				return new LocalizedString("TeamMailboxStartSyncButtonString", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotificationsForCalendarUpdate
		{
			get
			{
				return new LocalizedString("NotificationsForCalendarUpdate", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReadReceiptsSlab
		{
			get
			{
				return new LocalizedString("ReadReceiptsSlab", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DetailsLinkText
		{
			get
			{
				return new LocalizedString("DetailsLinkText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Help
		{
			get
			{
				return new LocalizedString("Help", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchGroups
		{
			get
			{
				return new LocalizedString("SearchGroups", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShowConversationAsTreeInstruction
		{
			get
			{
				return new LocalizedString("ShowConversationAsTreeInstruction", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BypassModerationSenders
		{
			get
			{
				return new LocalizedString("BypassModerationSenders", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RetentionActionDeleteAndAllowRecovery
		{
			get
			{
				return new LocalizedString("RetentionActionDeleteAndAllowRecovery", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PreviewMarkAsReadDelaytimeTextPre
		{
			get
			{
				return new LocalizedString("PreviewMarkAsReadDelaytimeTextPre", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RPTMonths
		{
			get
			{
				return new LocalizedString("RPTMonths", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AfterMoveOrDeleteBehavior
		{
			get
			{
				return new LocalizedString("AfterMoveOrDeleteBehavior", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerificationSuccessText(string emailAddress)
		{
			return new LocalizedString("VerificationSuccessText", "", false, false, OwaOptionStrings.ResourceManager, new object[]
			{
				emailAddress
			});
		}

		public static LocalizedString HideGroupFromAddressLists
		{
			get
			{
				return new LocalizedString("HideGroupFromAddressLists", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailWizardStep1Description
		{
			get
			{
				return new LocalizedString("VoicemailWizardStep1Description", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReviewLinkText
		{
			get
			{
				return new LocalizedString("ReviewLinkText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Processing
		{
			get
			{
				return new LocalizedString("Processing", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DailyCalendarAgendas
		{
			get
			{
				return new LocalizedString("DailyCalendarAgendas", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PreviewMarkAsReadBehaviorOnSelectionChange
		{
			get
			{
				return new LocalizedString("PreviewMarkAsReadBehaviorOnSelectionChange", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZoneLabelText
		{
			get
			{
				return new LocalizedString("TimeZoneLabelText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QLVoiceMail
		{
			get
			{
				return new LocalizedString("QLVoiceMail", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailSignUpIntro
		{
			get
			{
				return new LocalizedString("VoicemailSignUpIntro", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailStep2
		{
			get
			{
				return new LocalizedString("VoicemailStep2", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxMembershipString
		{
			get
			{
				return new LocalizedString("TeamMailboxMembershipString", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PasscodeLabel
		{
			get
			{
				return new LocalizedString("PasscodeLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PersonalSettingPasswordAfterChange
		{
			get
			{
				return new LocalizedString("PersonalSettingPasswordAfterChange", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerificationSuccessPageTitle
		{
			get
			{
				return new LocalizedString("VerificationSuccessPageTitle", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EnableAutomaticProcessingNote
		{
			get
			{
				return new LocalizedString("EnableAutomaticProcessingNote", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Days
		{
			get
			{
				return new LocalizedString("Days", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotificationsNotSetUp
		{
			get
			{
				return new LocalizedString("NotificationsNotSetUp", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ModerationNotificationsInternal
		{
			get
			{
				return new LocalizedString("ModerationNotificationsInternal", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProtocolSettings
		{
			get
			{
				return new LocalizedString("ProtocolSettings", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EnableAutomaticProcessing
		{
			get
			{
				return new LocalizedString("EnableAutomaticProcessing", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageOptionsSlab
		{
			get
			{
				return new LocalizedString("MessageOptionsSlab", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ChooseMessageFont
		{
			get
			{
				return new LocalizedString("ChooseMessageFont", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Password
		{
			get
			{
				return new LocalizedString("Password", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OWAExtensions
		{
			get
			{
				return new LocalizedString("OWAExtensions", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StringArrayDialogLabel
		{
			get
			{
				return new LocalizedString("StringArrayDialogLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Unlimited
		{
			get
			{
				return new LocalizedString("Unlimited", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailSMSOptionVoiceMailOnly
		{
			get
			{
				return new LocalizedString("VoicemailSMSOptionVoiceMailOnly", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Rules
		{
			get
			{
				return new LocalizedString("Rules", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ModeratedByEmptyDataText
		{
			get
			{
				return new LocalizedString("ModeratedByEmptyDataText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TextMessaging
		{
			get
			{
				return new LocalizedString("TextMessaging", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FromLabel
		{
			get
			{
				return new LocalizedString("FromLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupModerators
		{
			get
			{
				return new LocalizedString("GroupModerators", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReadingPaneInstruction
		{
			get
			{
				return new LocalizedString("ReadingPaneInstruction", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RPTNone
		{
			get
			{
				return new LocalizedString("RPTNone", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Spelling
		{
			get
			{
				return new LocalizedString("Spelling", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CancelWipeDeviceCommandText
		{
			get
			{
				return new LocalizedString("CancelWipeDeviceCommandText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutomaticRepliesEnabledText
		{
			get
			{
				return new LocalizedString("AutomaticRepliesEnabledText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DisplayNameLabel
		{
			get
			{
				return new LocalizedString("DisplayNameLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CancelButtonText
		{
			get
			{
				return new LocalizedString("CancelButtonText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMembershipApproval
		{
			get
			{
				return new LocalizedString("GroupMembershipApproval", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InlcudedRecipientTypesLabel
		{
			get
			{
				return new LocalizedString("InlcudedRecipientTypesLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Name
		{
			get
			{
				return new LocalizedString("Name", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RetentionActionTypeDelete
		{
			get
			{
				return new LocalizedString("RetentionActionTypeDelete", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMyNameInCcBoxConditionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleMyNameInCcBoxConditionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ThursdayCheckBoxText
		{
			get
			{
				return new LocalizedString("ThursdayCheckBoxText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JoinGroup
		{
			get
			{
				return new LocalizedString("JoinGroup", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Account
		{
			get
			{
				return new LocalizedString("Account", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMessageTypeMatchesConditionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleMessageTypeMatchesConditionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMoveCopyDeleteGroupText
		{
			get
			{
				return new LocalizedString("InboxRuleMoveCopyDeleteGroupText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RegionalSettingsInstruction
		{
			get
			{
				return new LocalizedString("RegionalSettingsInstruction", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NameColumn
		{
			get
			{
				return new LocalizedString("NameColumn", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleWithSensitivityConditionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleWithSensitivityConditionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClassificationDialogTitle
		{
			get
			{
				return new LocalizedString("ClassificationDialogTitle", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleFromAndMoveToFolderTemplate
		{
			get
			{
				return new LocalizedString("RuleFromAndMoveToFolderTemplate", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DomainNameNotSetError
		{
			get
			{
				return new LocalizedString("DomainNameNotSetError", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MakeSecurityGroup
		{
			get
			{
				return new LocalizedString("MakeSecurityGroup", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContactNumbersBookmark
		{
			get
			{
				return new LocalizedString("ContactNumbersBookmark", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleSentToConditionText
		{
			get
			{
				return new LocalizedString("InboxRuleSentToConditionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MemberOfGroups
		{
			get
			{
				return new LocalizedString("MemberOfGroups", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleIncludeTheseWordsGroupText
		{
			get
			{
				return new LocalizedString("InboxRuleIncludeTheseWordsGroupText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailTipLabel
		{
			get
			{
				return new LocalizedString("MailTipLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTypeDialogLabel
		{
			get
			{
				return new LocalizedString("MessageTypeDialogLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RegionalSettingsSlab
		{
			get
			{
				return new LocalizedString("RegionalSettingsSlab", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailWizardStep3Title
		{
			get
			{
				return new LocalizedString("VoicemailWizardStep3Title", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMyNameNotInToBoxConditionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleMyNameNotInToBoxConditionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReturnToOWA
		{
			get
			{
				return new LocalizedString("ReturnToOWA", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMyNameInToBoxConditionText
		{
			get
			{
				return new LocalizedString("InboxRuleMyNameInToBoxConditionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CommitButtonText
		{
			get
			{
				return new LocalizedString("CommitButtonText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxShowInMyClientString
		{
			get
			{
				return new LocalizedString("TeamMailboxShowInMyClientString", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMarkAsReadActionText
		{
			get
			{
				return new LocalizedString("InboxRuleMarkAsReadActionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClassificationDialogLabel
		{
			get
			{
				return new LocalizedString("ClassificationDialogLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WarningAlt
		{
			get
			{
				return new LocalizedString("WarningAlt", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxManagementString4
		{
			get
			{
				return new LocalizedString("TeamMailboxManagementString4", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Mail
		{
			get
			{
				return new LocalizedString("Mail", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImportContactList
		{
			get
			{
				return new LocalizedString("ImportContactList", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JoinOtherDlsSuccess(int num)
		{
			return new LocalizedString("JoinOtherDlsSuccess", "", false, false, OwaOptionStrings.ResourceManager, new object[]
			{
				num
			});
		}

		public static LocalizedString QLImportContacts
		{
			get
			{
				return new LocalizedString("QLImportContacts", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRule
		{
			get
			{
				return new LocalizedString("InboxRule", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WithinDateRangeDialogTitle
		{
			get
			{
				return new LocalizedString("WithinDateRangeDialogTitle", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReminderSoundEnabled
		{
			get
			{
				return new LocalizedString("ReminderSoundEnabled", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecipientAddressContainsConditionFormat
		{
			get
			{
				return new LocalizedString("RecipientAddressContainsConditionFormat", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageFormatPlainText
		{
			get
			{
				return new LocalizedString("MessageFormatPlainText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeleteInboxRuleConfirmation
		{
			get
			{
				return new LocalizedString("DeleteInboxRuleConfirmation", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ForwardEmailTitle
		{
			get
			{
				return new LocalizedString("ForwardEmailTitle", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BypassModerationSendersEmptyDataText
		{
			get
			{
				return new LocalizedString("BypassModerationSendersEmptyDataText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleSubjectContainsAndDeleteMessageTemplate
		{
			get
			{
				return new LocalizedString("RuleSubjectContainsAndDeleteMessageTemplate", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HideDeletedItems
		{
			get
			{
				return new LocalizedString("HideDeletedItems", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailSetupNowButtonText
		{
			get
			{
				return new LocalizedString("VoicemailSetupNowButtonText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarPermissions
		{
			get
			{
				return new LocalizedString("CalendarPermissions", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ModerationNotificationsAlways
		{
			get
			{
				return new LocalizedString("ModerationNotificationsAlways", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalMessageInstruction
		{
			get
			{
				return new LocalizedString("ExternalMessageInstruction", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequirementsReadItemValue
		{
			get
			{
				return new LocalizedString("RequirementsReadItemValue", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SchedulingOptionsSlab
		{
			get
			{
				return new LocalizedString("SchedulingOptionsSlab", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShowConversationTree
		{
			get
			{
				return new LocalizedString("ShowConversationTree", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RetentionPolicies
		{
			get
			{
				return new LocalizedString("RetentionPolicies", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarPublishingSubscriptionUrl
		{
			get
			{
				return new LocalizedString("CalendarPublishingSubscriptionUrl", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ResendVerificationEmailCommandText
		{
			get
			{
				return new LocalizedString("ResendVerificationEmailCommandText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TextMessagingNotification
		{
			get
			{
				return new LocalizedString("TextMessagingNotification", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstalledByColumn
		{
			get
			{
				return new LocalizedString("InstalledByColumn", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupOrganizationalUnit
		{
			get
			{
				return new LocalizedString("GroupOrganizationalUnit", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxFolderDialogTitle
		{
			get
			{
				return new LocalizedString("MailboxFolderDialogTitle", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PersonalSettingOldPassword
		{
			get
			{
				return new LocalizedString("PersonalSettingOldPassword", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailStep3
		{
			get
			{
				return new LocalizedString("VoicemailStep3", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CityLabel
		{
			get
			{
				return new LocalizedString("CityLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SentToConditionFormat
		{
			get
			{
				return new LocalizedString("SentToConditionFormat", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QLSubscription
		{
			get
			{
				return new LocalizedString("QLSubscription", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ViewRPTDetailsTitle
		{
			get
			{
				return new LocalizedString("ViewRPTDetailsTitle", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MyGroups
		{
			get
			{
				return new LocalizedString("MyGroups", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxesString
		{
			get
			{
				return new LocalizedString("TeamMailboxesString", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeliveryReport
		{
			get
			{
				return new LocalizedString("DeliveryReport", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LastNameLabel
		{
			get
			{
				return new LocalizedString("LastNameLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarPublishingStop
		{
			get
			{
				return new LocalizedString("CalendarPublishingStop", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailWizardStep3Description
		{
			get
			{
				return new LocalizedString("VoicemailWizardStep3Description", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReceiveNotificationsUsingFormat(string phoneNumber)
		{
			return new LocalizedString("ReceiveNotificationsUsingFormat", "", false, false, OwaOptionStrings.ResourceManager, new object[]
			{
				phoneNumber
			});
		}

		public static LocalizedString QLWhatsNewForOrganizations
		{
			get
			{
				return new LocalizedString("QLWhatsNewForOrganizations", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReadReceiptResponseAlways
		{
			get
			{
				return new LocalizedString("ReadReceiptResponseAlways", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmailTrustedListsOnly
		{
			get
			{
				return new LocalizedString("JunkEmailTrustedListsOnly", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MatchSortOrder
		{
			get
			{
				return new LocalizedString("MatchSortOrder", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DevicePhoneNumberLabel
		{
			get
			{
				return new LocalizedString("DevicePhoneNumberLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMessageTypeMatchesConditionText
		{
			get
			{
				return new LocalizedString("InboxRuleMessageTypeMatchesConditionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RetentionTypeOptional
		{
			get
			{
				return new LocalizedString("RetentionTypeOptional", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UseSettings
		{
			get
			{
				return new LocalizedString("UseSettings", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchAllGroups
		{
			get
			{
				return new LocalizedString("SearchAllGroups", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MembersLabel
		{
			get
			{
				return new LocalizedString("MembersLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FreeBusyInformation
		{
			get
			{
				return new LocalizedString("FreeBusyInformation", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceIMEILabel
		{
			get
			{
				return new LocalizedString("DeviceIMEILabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Day
		{
			get
			{
				return new LocalizedString("Day", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMoveToFolderActionText
		{
			get
			{
				return new LocalizedString("InboxRuleMoveToFolderActionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SelectOne
		{
			get
			{
				return new LocalizedString("SelectOne", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleApplyCategoryActionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleApplyCategoryActionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetupEmailNotificationsLink
		{
			get
			{
				return new LocalizedString("SetupEmailNotificationsLink", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NewDistributionGroupCaption
		{
			get
			{
				return new LocalizedString("NewDistributionGroupCaption", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ViewRPTRetentionActionLabel
		{
			get
			{
				return new LocalizedString("ViewRPTRetentionActionLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShowWeekNumbersCheckBoxText
		{
			get
			{
				return new LocalizedString("ShowWeekNumbersCheckBoxText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserNameWLIDLabel
		{
			get
			{
				return new LocalizedString("UserNameWLIDLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchMessagesIReceivedLabel
		{
			get
			{
				return new LocalizedString("SearchMessagesIReceivedLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleWithinDateRangeConditionText
		{
			get
			{
				return new LocalizedString("InboxRuleWithinDateRangeConditionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleSendTextMessageNotificationToActionText
		{
			get
			{
				return new LocalizedString("InboxRuleSendTextMessageNotificationToActionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SentLabel
		{
			get
			{
				return new LocalizedString("SentLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupInformation
		{
			get
			{
				return new LocalizedString("GroupInformation", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailAskOperator
		{
			get
			{
				return new LocalizedString("VoicemailAskOperator", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OWA
		{
			get
			{
				return new LocalizedString("OWA", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailTipWaterMark
		{
			get
			{
				return new LocalizedString("MailTipWaterMark", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleWithSensitivityConditionText
		{
			get
			{
				return new LocalizedString("InboxRuleWithSensitivityConditionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RetentionActionTypeDefaultArchive
		{
			get
			{
				return new LocalizedString("RetentionActionTypeDefaultArchive", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoveOptionalRPTConfirmation
		{
			get
			{
				return new LocalizedString("RemoveOptionalRPTConfirmation", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DevicePolicyUpdateTimeLabel
		{
			get
			{
				return new LocalizedString("DevicePolicyUpdateTimeLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MyselfLiveFormat
		{
			get
			{
				return new LocalizedString("MyselfLiveFormat", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmailDomain
		{
			get
			{
				return new LocalizedString("EmailDomain", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequireSenderAuthTrue
		{
			get
			{
				return new LocalizedString("RequireSenderAuthTrue", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstallFromPrivateUrlInformation
		{
			get
			{
				return new LocalizedString("InstallFromPrivateUrlInformation", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PhoneLabel
		{
			get
			{
				return new LocalizedString("PhoneLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubscriptionAction
		{
			get
			{
				return new LocalizedString("SubscriptionAction", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Weeks
		{
			get
			{
				return new LocalizedString("Weeks", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImportContactListPage2Result(string filename)
		{
			return new LocalizedString("ImportContactListPage2Result", "", false, false, OwaOptionStrings.ResourceManager, new object[]
			{
				filename
			});
		}

		public static LocalizedString InboxRuleForwardRedirectGroupText
		{
			get
			{
				return new LocalizedString("InboxRuleForwardRedirectGroupText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DisplayName
		{
			get
			{
				return new LocalizedString("DisplayName", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MembershipApprovalLabel
		{
			get
			{
				return new LocalizedString("MembershipApprovalLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleSendTextMessageNotificationToActionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleSendTextMessageNotificationToActionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxSPSiteString
		{
			get
			{
				return new LocalizedString("TeamMailboxSPSiteString", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleSubjectOrBodyContainsConditionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleSubjectOrBodyContainsConditionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PleaseWait
		{
			get
			{
				return new LocalizedString("PleaseWait", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JoinRestrictionOpen
		{
			get
			{
				return new LocalizedString("JoinRestrictionOpen", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarSharingConfirmDeletionSingle
		{
			get
			{
				return new LocalizedString("CalendarSharingConfirmDeletionSingle", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleSubjectContainsConditionPreCannedText
		{
			get
			{
				return new LocalizedString("InboxRuleSubjectContainsConditionPreCannedText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarPublishingStart
		{
			get
			{
				return new LocalizedString("CalendarPublishingStart", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TextMessagingSlabMessageNotificationOnly
		{
			get
			{
				return new LocalizedString("TextMessagingSlabMessageNotificationOnly", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequirementsRestrictedDescription
		{
			get
			{
				return new LocalizedString("RequirementsRestrictedDescription", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SelectAUser
		{
			get
			{
				return new LocalizedString("SelectAUser", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotificationStepOneMessage
		{
			get
			{
				return new LocalizedString("NotificationStepOneMessage", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QLPushEmail
		{
			get
			{
				return new LocalizedString("QLPushEmail", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NewInboxRuleTitle
		{
			get
			{
				return new LocalizedString("NewInboxRuleTitle", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SendToKnownContactsText
		{
			get
			{
				return new LocalizedString("SendToKnownContactsText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncomingAuthenticationSpa
		{
			get
			{
				return new LocalizedString("IncomingAuthenticationSpa", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxUsageUnitGB
		{
			get
			{
				return new LocalizedString("MailboxUsageUnitGB", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTrackingDeliveredEvent
		{
			get
			{
				return new LocalizedString("MessageTrackingDeliveredEvent", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SelectOneOrMoreText
		{
			get
			{
				return new LocalizedString("SelectOneOrMoreText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleForwardAsAttachmentToActionText
		{
			get
			{
				return new LocalizedString("InboxRuleForwardAsAttachmentToActionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DontSeeMyRegionOrMobileOperator
		{
			get
			{
				return new LocalizedString("DontSeeMyRegionOrMobileOperator", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PreviewMarkAsReadDelaytimeTextPost
		{
			get
			{
				return new LocalizedString("PreviewMarkAsReadDelaytimeTextPost", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoMessageClassificationAvailable
		{
			get
			{
				return new LocalizedString("NoMessageClassificationAvailable", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxTabPropertiesHelpString
		{
			get
			{
				return new LocalizedString("TeamMailboxTabPropertiesHelpString", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxManagementString1
		{
			get
			{
				return new LocalizedString("TeamMailboxManagementString1", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RetentionPeriodHeader
		{
			get
			{
				return new LocalizedString("RetentionPeriodHeader", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Phone
		{
			get
			{
				return new LocalizedString("Phone", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhoHasPermission
		{
			get
			{
				return new LocalizedString("WhoHasPermission", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotAvailable
		{
			get
			{
				return new LocalizedString("NotAvailable", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FlaggedForActionConditionFormat
		{
			get
			{
				return new LocalizedString("FlaggedForActionConditionFormat", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EndTimeText
		{
			get
			{
				return new LocalizedString("EndTimeText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BookingWindowInDays
		{
			get
			{
				return new LocalizedString("BookingWindowInDays", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemovingPreviewPhoto
		{
			get
			{
				return new LocalizedString("RemovingPreviewPhoto", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarDiagnosticLogSlab
		{
			get
			{
				return new LocalizedString("CalendarDiagnosticLogSlab", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ModerationNotification
		{
			get
			{
				return new LocalizedString("ModerationNotification", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailWizardPinlessOptionsText
		{
			get
			{
				return new LocalizedString("VoicemailWizardPinlessOptionsText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailClearSettings
		{
			get
			{
				return new LocalizedString("VoicemailClearSettings", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PersonalGroups
		{
			get
			{
				return new LocalizedString("PersonalGroups", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DistributionGroupText
		{
			get
			{
				return new LocalizedString("DistributionGroupText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProfileContactNumbers
		{
			get
			{
				return new LocalizedString("ProfileContactNumbers", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeliveryReportFor
		{
			get
			{
				return new LocalizedString("DeliveryReportFor", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleRedirectToActionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleRedirectToActionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutomateProcessing
		{
			get
			{
				return new LocalizedString("AutomateProcessing", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarPublishingAccessLevel
		{
			get
			{
				return new LocalizedString("CalendarPublishingAccessLevel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ForwardEmailTo
		{
			get
			{
				return new LocalizedString("ForwardEmailTo", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SettingUpProtocolAccess
		{
			get
			{
				return new LocalizedString("SettingUpProtocolAccess", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdminTools
		{
			get
			{
				return new LocalizedString("AdminTools", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstalledExtensionDescription
		{
			get
			{
				return new LocalizedString("InstalledExtensionDescription", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmailComposeModeInstruction
		{
			get
			{
				return new LocalizedString("EmailComposeModeInstruction", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDeleteMessageActionText
		{
			get
			{
				return new LocalizedString("InboxRuleDeleteMessageActionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SummaryToDate
		{
			get
			{
				return new LocalizedString("SummaryToDate", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Extension
		{
			get
			{
				return new LocalizedString("Extension", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarSharingConfirmDeletionMultiple
		{
			get
			{
				return new LocalizedString("CalendarSharingConfirmDeletionMultiple", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Depart
		{
			get
			{
				return new LocalizedString("Depart", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmailNotificationsLink
		{
			get
			{
				return new LocalizedString("EmailNotificationsLink", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OpenPreviousItem
		{
			get
			{
				return new LocalizedString("OpenPreviousItem", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RPTPickerDialogTitle
		{
			get
			{
				return new LocalizedString("RPTPickerDialogTitle", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CheckForForgottenAttachments
		{
			get
			{
				return new LocalizedString("CheckForForgottenAttachments", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FaxLabel
		{
			get
			{
				return new LocalizedString("FaxLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SentTime
		{
			get
			{
				return new LocalizedString("SentTime", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceTypeHeaderText
		{
			get
			{
				return new LocalizedString("DeviceTypeHeaderText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RPTExpireNever
		{
			get
			{
				return new LocalizedString("RPTExpireNever", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxEmailAddressString
		{
			get
			{
				return new LocalizedString("TeamMailboxEmailAddressString", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleWithinSizeRangeConditionText
		{
			get
			{
				return new LocalizedString("InboxRuleWithinSizeRangeConditionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Week
		{
			get
			{
				return new LocalizedString("Week", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WithinDateRangeConditionFormat
		{
			get
			{
				return new LocalizedString("WithinDateRangeConditionFormat", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DisplayNameNotSetError
		{
			get
			{
				return new LocalizedString("DisplayNameNotSetError", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FirstDayOfWeek
		{
			get
			{
				return new LocalizedString("FirstDayOfWeek", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProcessExternalMeetingMessagesCheckBoxText
		{
			get
			{
				return new LocalizedString("ProcessExternalMeetingMessagesCheckBoxText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DepartRestrictionClosed
		{
			get
			{
				return new LocalizedString("DepartRestrictionClosed", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailTipShortLabel
		{
			get
			{
				return new LocalizedString("MailTipShortLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StateOrProvinceLabel
		{
			get
			{
				return new LocalizedString("StateOrProvinceLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PrimaryEmailAddress
		{
			get
			{
				return new LocalizedString("PrimaryEmailAddress", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AllowedSendersEmptyLabel
		{
			get
			{
				return new LocalizedString("AllowedSendersEmptyLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HotmailSubscription
		{
			get
			{
				return new LocalizedString("HotmailSubscription", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceIDLabel
		{
			get
			{
				return new LocalizedString("DeviceIDLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AlwaysShowFrom
		{
			get
			{
				return new LocalizedString("AlwaysShowFrom", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchButtonText
		{
			get
			{
				return new LocalizedString("SearchButtonText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ViewRPTDescriptionLabel
		{
			get
			{
				return new LocalizedString("ViewRPTDescriptionLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Hour
		{
			get
			{
				return new LocalizedString("Hour", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FlagStatusDialogTitle
		{
			get
			{
				return new LocalizedString("FlagStatusDialogTitle", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NewSubscriptionProgress
		{
			get
			{
				return new LocalizedString("NewSubscriptionProgress", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EditProfilePhoto
		{
			get
			{
				return new LocalizedString("EditProfilePhoto", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstallFromPrivateUrl
		{
			get
			{
				return new LocalizedString("InstallFromPrivateUrl", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeleteGroupsConfirmation
		{
			get
			{
				return new LocalizedString("DeleteGroupsConfirmation", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OwnedGroups
		{
			get
			{
				return new LocalizedString("OwnedGroups", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NewDistributionGroupTitle
		{
			get
			{
				return new LocalizedString("NewDistributionGroupTitle", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmailConfiguration
		{
			get
			{
				return new LocalizedString("JunkEmailConfiguration", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProfileGeneral
		{
			get
			{
				return new LocalizedString("ProfileGeneral", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarSharingOutlookNote
		{
			get
			{
				return new LocalizedString("CalendarSharingOutlookNote", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoveOptionalRPTsConfirmation
		{
			get
			{
				return new LocalizedString("RemoveOptionalRPTsConfirmation", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerificationSuccessTitle
		{
			get
			{
				return new LocalizedString("VerificationSuccessTitle", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ResponseMessageSlab
		{
			get
			{
				return new LocalizedString("ResponseMessageSlab", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxMaintenanceString
		{
			get
			{
				return new LocalizedString("TeamMailboxMaintenanceString", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UrlLabelText
		{
			get
			{
				return new LocalizedString("UrlLabelText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DepartRestrictionOpen
		{
			get
			{
				return new LocalizedString("DepartRestrictionOpen", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PendingWipeCommandSentLabel
		{
			get
			{
				return new LocalizedString("PendingWipeCommandSentLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubjectOrBodyContainsConditionFormat
		{
			get
			{
				return new LocalizedString("SubjectOrBodyContainsConditionFormat", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LeaveMailOnServerLabel
		{
			get
			{
				return new LocalizedString("LeaveMailOnServerLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JoinRestrictionClosed
		{
			get
			{
				return new LocalizedString("JoinRestrictionClosed", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ScheduleOnlyDuringWorkHours
		{
			get
			{
				return new LocalizedString("ScheduleOnlyDuringWorkHours", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImportanceDialogLabel
		{
			get
			{
				return new LocalizedString("ImportanceDialogLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarPublishingLinkNotes
		{
			get
			{
				return new LocalizedString("CalendarPublishingLinkNotes", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleSentOnlyToMeConditionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleSentOnlyToMeConditionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InternalMessageInstruction
		{
			get
			{
				return new LocalizedString("InternalMessageInstruction", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmailDisabled
		{
			get
			{
				return new LocalizedString("JunkEmailDisabled", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleForwardAsAttachmentToActionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleForwardAsAttachmentToActionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WarningNote
		{
			get
			{
				return new LocalizedString("WarningNote", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HomePagePrimaryLink
		{
			get
			{
				return new LocalizedString("HomePagePrimaryLink", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LimitDuration
		{
			get
			{
				return new LocalizedString("LimitDuration", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxUsageExceededText
		{
			get
			{
				return new LocalizedString("MailboxUsageExceededText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnSupportedRule
		{
			get
			{
				return new LocalizedString("UnSupportedRule", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceModelLabel
		{
			get
			{
				return new LocalizedString("DeviceModelLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotificationLinksMessage
		{
			get
			{
				return new LocalizedString("NotificationLinksMessage", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ResourceSlab
		{
			get
			{
				return new LocalizedString("ResourceSlab", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RetentionPolicyTagPageTitle
		{
			get
			{
				return new LocalizedString("RetentionPolicyTagPageTitle", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LargeRecipientList(int number)
		{
			return new LocalizedString("LargeRecipientList", "", false, false, OwaOptionStrings.ResourceManager, new object[]
			{
				number
			});
		}

		public static LocalizedString AllBookInPolicyText
		{
			get
			{
				return new LocalizedString("AllBookInPolicyText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageFormatHtml
		{
			get
			{
				return new LocalizedString("MessageFormatHtml", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FridayCheckBoxText
		{
			get
			{
				return new LocalizedString("FridayCheckBoxText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMyNameInCcBoxConditionText
		{
			get
			{
				return new LocalizedString("InboxRuleMyNameInCcBoxConditionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PreviewMarkAsReadDelaytimeErrorMessage
		{
			get
			{
				return new LocalizedString("PreviewMarkAsReadDelaytimeErrorMessage", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmailValidationErrorMessage
		{
			get
			{
				return new LocalizedString("JunkEmailValidationErrorMessage", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxTabUsersHelpString2
		{
			get
			{
				return new LocalizedString("TeamMailboxTabUsersHelpString2", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AllRequestInPolicyText
		{
			get
			{
				return new LocalizedString("AllRequestInPolicyText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapSubscription
		{
			get
			{
				return new LocalizedString("ImapSubscription", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleWithImportanceConditionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleWithImportanceConditionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxDisplayNameString
		{
			get
			{
				return new LocalizedString("TeamMailboxDisplayNameString", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxManagementString3
		{
			get
			{
				return new LocalizedString("TeamMailboxManagementString3", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConflictPercentageAllowed
		{
			get
			{
				return new LocalizedString("ConflictPercentageAllowed", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImportanceDialogTitle
		{
			get
			{
				return new LocalizedString("ImportanceDialogTitle", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SecurityGroupText
		{
			get
			{
				return new LocalizedString("SecurityGroupText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubjectContainsConditionFormat
		{
			get
			{
				return new LocalizedString("SubjectContainsConditionFormat", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailStep1
		{
			get
			{
				return new LocalizedString("VoicemailStep1", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImportContactListPage1Step1
		{
			get
			{
				return new LocalizedString("ImportContactListPage1Step1", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarPublishingRangeTo
		{
			get
			{
				return new LocalizedString("CalendarPublishingRangeTo", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Installed
		{
			get
			{
				return new LocalizedString("Installed", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JoinRestrictionClosedDetails
		{
			get
			{
				return new LocalizedString("JoinRestrictionClosedDetails", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EnterNumberStepMessage
		{
			get
			{
				return new LocalizedString("EnterNumberStepMessage", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxString
		{
			get
			{
				return new LocalizedString("TeamMailboxString", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalAudienceCheckboxText
		{
			get
			{
				return new LocalizedString("ExternalAudienceCheckboxText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RetentionActionNeverDelete
		{
			get
			{
				return new LocalizedString("RetentionActionNeverDelete", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxOwnersString
		{
			get
			{
				return new LocalizedString("TeamMailboxOwnersString", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SentOnlyToMeConditionFormat
		{
			get
			{
				return new LocalizedString("SentOnlyToMeConditionFormat", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PostalCodeLabel
		{
			get
			{
				return new LocalizedString("PostalCodeLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerificationEmailSucceeded(string emailAddress)
		{
			return new LocalizedString("VerificationEmailSucceeded", "", false, false, OwaOptionStrings.ResourceManager, new object[]
			{
				emailAddress
			});
		}

		public static LocalizedString StreetAddressLabel
		{
			get
			{
				return new LocalizedString("StreetAddressLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ModerationNotificationsNever
		{
			get
			{
				return new LocalizedString("ModerationNotificationsNever", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoAddSignature
		{
			get
			{
				return new LocalizedString("AutoAddSignature", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailConfiguration
		{
			get
			{
				return new LocalizedString("VoicemailConfiguration", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxUsersString
		{
			get
			{
				return new LocalizedString("TeamMailboxUsersString", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Minutes
		{
			get
			{
				return new LocalizedString("Minutes", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TextMessagingStatusPrefixNotifications
		{
			get
			{
				return new LocalizedString("TextMessagingStatusPrefixNotifications", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NewSubscriptionSucceed(string feedback)
		{
			return new LocalizedString("NewSubscriptionSucceed", "", false, false, OwaOptionStrings.ResourceManager, new object[]
			{
				feedback
			});
		}

		public static LocalizedString OfficeLabel
		{
			get
			{
				return new LocalizedString("OfficeLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetupCalendarNotificationsLink
		{
			get
			{
				return new LocalizedString("SetupCalendarNotificationsLink", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WipeDeviceConfirmMessage
		{
			get
			{
				return new LocalizedString("WipeDeviceConfirmMessage", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmailEnabled
		{
			get
			{
				return new LocalizedString("JunkEmailEnabled", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProfilePhoto
		{
			get
			{
				return new LocalizedString("ProfilePhoto", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JoinRestriction
		{
			get
			{
				return new LocalizedString("JoinRestriction", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubscriptionAccountInformation
		{
			get
			{
				return new LocalizedString("SubscriptionAccountInformation", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PopSubscription
		{
			get
			{
				return new LocalizedString("PopSubscription", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectedAccountsDescriptionForSendAs
		{
			get
			{
				return new LocalizedString("ConnectedAccountsDescriptionForSendAs", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailNotConfiguredText
		{
			get
			{
				return new LocalizedString("VoicemailNotConfiguredText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Date
		{
			get
			{
				return new LocalizedString("Date", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubscriptionEmailAddress
		{
			get
			{
				return new LocalizedString("SubscriptionEmailAddress", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarAppearanceInstruction
		{
			get
			{
				return new LocalizedString("CalendarAppearanceInstruction", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DisableReminders
		{
			get
			{
				return new LocalizedString("DisableReminders", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UninstallExtensionConfirmation
		{
			get
			{
				return new LocalizedString("UninstallExtensionConfirmation", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReadReceiptResponseAskBefore
		{
			get
			{
				return new LocalizedString("ReadReceiptResponseAskBefore", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutomaticRepliesDisabledText
		{
			get
			{
				return new LocalizedString("AutomaticRepliesDisabledText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PermissionGranted
		{
			get
			{
				return new LocalizedString("PermissionGranted", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarTroubleShootingSlab
		{
			get
			{
				return new LocalizedString("CalendarTroubleShootingSlab", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QLRemotePowerShell
		{
			get
			{
				return new LocalizedString("QLRemotePowerShell", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Ownership
		{
			get
			{
				return new LocalizedString("Ownership", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DevicePhoneNumberHeaderText
		{
			get
			{
				return new LocalizedString("DevicePhoneNumberHeaderText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OwnerApprovalRequired
		{
			get
			{
				return new LocalizedString("OwnerApprovalRequired", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddExtensionTitle
		{
			get
			{
				return new LocalizedString("AddExtensionTitle", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DevicePolicyApplicationStatusLabel
		{
			get
			{
				return new LocalizedString("DevicePolicyApplicationStatusLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarWorkflowSlab
		{
			get
			{
				return new LocalizedString("CalendarWorkflowSlab", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SettingNotAvailable
		{
			get
			{
				return new LocalizedString("SettingNotAvailable", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DepartRestriction
		{
			get
			{
				return new LocalizedString("DepartRestriction", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RetentionTypeRequiredDescription
		{
			get
			{
				return new LocalizedString("RetentionTypeRequiredDescription", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoveForwardedMeetingNotificationsCheckBoxText
		{
			get
			{
				return new LocalizedString("RemoveForwardedMeetingNotificationsCheckBoxText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Everyone
		{
			get
			{
				return new LocalizedString("Everyone", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxAppTitle
		{
			get
			{
				return new LocalizedString("TeamMailboxAppTitle", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PersonalSettingConfirmPassword
		{
			get
			{
				return new LocalizedString("PersonalSettingConfirmPassword", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxUsageUnitMB
		{
			get
			{
				return new LocalizedString("MailboxUsageUnitMB", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubscriptionServerInformation
		{
			get
			{
				return new LocalizedString("SubscriptionServerInformation", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserNameLabel
		{
			get
			{
				return new LocalizedString("UserNameLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeIncrementFifteenMinutes
		{
			get
			{
				return new LocalizedString("TimeIncrementFifteenMinutes", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LastSuccessfulSync
		{
			get
			{
				return new LocalizedString("LastSuccessfulSync", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SchedulingPermissionsInstruction
		{
			get
			{
				return new LocalizedString("SchedulingPermissionsInstruction", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmptyDeletedItemsOnLogoff
		{
			get
			{
				return new LocalizedString("EmptyDeletedItemsOnLogoff", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BeforeDateDisplayTemplate
		{
			get
			{
				return new LocalizedString("BeforeDateDisplayTemplate", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmailAddressLabel
		{
			get
			{
				return new LocalizedString("EmailAddressLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmailBlockedListDescription
		{
			get
			{
				return new LocalizedString("JunkEmailBlockedListDescription", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NewItemNotificationSound
		{
			get
			{
				return new LocalizedString("NewItemNotificationSound", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NewInboxRuleCaption
		{
			get
			{
				return new LocalizedString("NewInboxRuleCaption", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateTimeZoneNoteLinkText
		{
			get
			{
				return new LocalizedString("UpdateTimeZoneNoteLinkText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HasClassificationConditionFormat
		{
			get
			{
				return new LocalizedString("HasClassificationConditionFormat", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoneAccessRightRole
		{
			get
			{
				return new LocalizedString("NoneAccessRightRole", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MobileDeviceDetailTitle
		{
			get
			{
				return new LocalizedString("MobileDeviceDetailTitle", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EditCommandText
		{
			get
			{
				return new LocalizedString("EditCommandText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceTypeLabel
		{
			get
			{
				return new LocalizedString("DeviceTypeLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AllowConflicts
		{
			get
			{
				return new LocalizedString("AllowConflicts", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarPublishing
		{
			get
			{
				return new LocalizedString("CalendarPublishing", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RetentionNameHeader
		{
			get
			{
				return new LocalizedString("RetentionNameHeader", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImportContactListPage1InformationText
		{
			get
			{
				return new LocalizedString("ImportContactListPage1InformationText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailAskPhoneNumber
		{
			get
			{
				return new LocalizedString("VoicemailAskPhoneNumber", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxDocumentsString
		{
			get
			{
				return new LocalizedString("TeamMailboxDocumentsString", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CountryOrRegionLabel
		{
			get
			{
				return new LocalizedString("CountryOrRegionLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarPublishingLearnMore
		{
			get
			{
				return new LocalizedString("CalendarPublishingLearnMore", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupModeratedBy
		{
			get
			{
				return new LocalizedString("GroupModeratedBy", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DidntReceivePasscodeMessage
		{
			get
			{
				return new LocalizedString("DidntReceivePasscodeMessage", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleFromAddressContainsConditionText
		{
			get
			{
				return new LocalizedString("InboxRuleFromAddressContainsConditionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncomingAuthenticationLabel
		{
			get
			{
				return new LocalizedString("IncomingAuthenticationLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleRecipientAddressContainsConditionText
		{
			get
			{
				return new LocalizedString("InboxRuleRecipientAddressContainsConditionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DepartGroupsConfirmation
		{
			get
			{
				return new LocalizedString("DepartGroupsConfirmation", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarAppearanceSlab
		{
			get
			{
				return new LocalizedString("CalendarAppearanceSlab", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InitialsLabel
		{
			get
			{
				return new LocalizedString("InitialsLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleFlaggedForActionConditionText
		{
			get
			{
				return new LocalizedString("InboxRuleFlaggedForActionConditionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuickLinks
		{
			get
			{
				return new LocalizedString("QuickLinks", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxMyRoleString2
		{
			get
			{
				return new LocalizedString("TeamMailboxMyRoleString2", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RoomEmailAddressLabel
		{
			get
			{
				return new LocalizedString("RoomEmailAddressLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ToColumnLabel
		{
			get
			{
				return new LocalizedString("ToColumnLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SensitivityDialogLabel
		{
			get
			{
				return new LocalizedString("SensitivityDialogLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AllowedSendersEmptyLabelForEndUser
		{
			get
			{
				return new LocalizedString("AllowedSendersEmptyLabelForEndUser", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequirementsReadWriteMailboxValue
		{
			get
			{
				return new LocalizedString("RequirementsReadWriteMailboxValue", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FlagStatusDialogLabel
		{
			get
			{
				return new LocalizedString("FlagStatusDialogLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapSetting
		{
			get
			{
				return new LocalizedString("ImapSetting", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailConfiguredText
		{
			get
			{
				return new LocalizedString("VoicemailConfiguredText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmailTrustedListHeader
		{
			get
			{
				return new LocalizedString("JunkEmailTrustedListHeader", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequirementsReadItemDescription
		{
			get
			{
				return new LocalizedString("RequirementsReadItemDescription", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequireSenderAuth
		{
			get
			{
				return new LocalizedString("RequireSenderAuth", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IWantToEditMyNotificationSettings
		{
			get
			{
				return new LocalizedString("IWantToEditMyNotificationSettings", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DevicePolicyAppliedLabel
		{
			get
			{
				return new LocalizedString("DevicePolicyAppliedLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailSMSOptionNone
		{
			get
			{
				return new LocalizedString("VoicemailSMSOptionNone", "ExFB5221", false, true, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EditDistributionGroupTitle
		{
			get
			{
				return new LocalizedString("EditDistributionGroupTitle", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MaximumDurationInMinutes
		{
			get
			{
				return new LocalizedString("MaximumDurationInMinutes", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NewPopSubscription
		{
			get
			{
				return new LocalizedString("NewPopSubscription", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MobileDeviceHeadNoteInfo
		{
			get
			{
				return new LocalizedString("MobileDeviceHeadNoteInfo", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HomePhoneLabel
		{
			get
			{
				return new LocalizedString("HomePhoneLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BlockDeviceCommandText
		{
			get
			{
				return new LocalizedString("BlockDeviceCommandText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SelectUsers
		{
			get
			{
				return new LocalizedString("SelectUsers", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchGroupsButtonDescription
		{
			get
			{
				return new LocalizedString("SearchGroupsButtonDescription", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeleteEmailSubscriptionsConfirmation
		{
			get
			{
				return new LocalizedString("DeleteEmailSubscriptionsConfirmation", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LearnHowToUseRedirectTo
		{
			get
			{
				return new LocalizedString("LearnHowToUseRedirectTo", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleDeleteMessageActionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleDeleteMessageActionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RetentionExplanationLabel
		{
			get
			{
				return new LocalizedString("RetentionExplanationLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTrackingPendingEvent
		{
			get
			{
				return new LocalizedString("MessageTrackingPendingEvent", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProviderColumn
		{
			get
			{
				return new LocalizedString("ProviderColumn", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SettingAccessDisabled
		{
			get
			{
				return new LocalizedString("SettingAccessDisabled", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RPTDays
		{
			get
			{
				return new LocalizedString("RPTDays", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleFromSubscriptionConditionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleFromSubscriptionConditionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SmtpAddressExample
		{
			get
			{
				return new LocalizedString("SmtpAddressExample", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMarkImportanceActionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleMarkImportanceActionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CategoryDialogLabel
		{
			get
			{
				return new LocalizedString("CategoryDialogLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceAccessStateLabel
		{
			get
			{
				return new LocalizedString("DeviceAccessStateLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmailBlockedListHeader
		{
			get
			{
				return new LocalizedString("JunkEmailBlockedListHeader", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailNotAvailableText
		{
			get
			{
				return new LocalizedString("VoicemailNotAvailableText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContactLocationBookmark
		{
			get
			{
				return new LocalizedString("ContactLocationBookmark", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleSubjectContainsConditionText
		{
			get
			{
				return new LocalizedString("InboxRuleSubjectContainsConditionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailWizardConfirmPinLabel
		{
			get
			{
				return new LocalizedString("VoicemailWizardConfirmPinLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoSubscriptionAvailable
		{
			get
			{
				return new LocalizedString("NoSubscriptionAvailable", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QLPassword
		{
			get
			{
				return new LocalizedString("QLPassword", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomAccessRightRole
		{
			get
			{
				return new LocalizedString("CustomAccessRightRole", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClearSettings
		{
			get
			{
				return new LocalizedString("ClearSettings", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxUsageUnitKB
		{
			get
			{
				return new LocalizedString("MailboxUsageUnitKB", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExtensionPackageLocation
		{
			get
			{
				return new LocalizedString("ExtensionPackageLocation", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleApplyCategoryActionText
		{
			get
			{
				return new LocalizedString("InboxRuleApplyCategoryActionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OWAVoicemail
		{
			get
			{
				return new LocalizedString("OWAVoicemail", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PersonalSettingPassword
		{
			get
			{
				return new LocalizedString("PersonalSettingPassword", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SendDuringWorkHoursOnly
		{
			get
			{
				return new LocalizedString("SendDuringWorkHoursOnly", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AliasLabelForDataCenter
		{
			get
			{
				return new LocalizedString("AliasLabelForDataCenter", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeliverAndForward
		{
			get
			{
				return new LocalizedString("DeliverAndForward", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Language
		{
			get
			{
				return new LocalizedString("Language", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NameAndAccountBookmark
		{
			get
			{
				return new LocalizedString("NameAndAccountBookmark", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SendMyPhoneColon
		{
			get
			{
				return new LocalizedString("SendMyPhoneColon", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DateStyles
		{
			get
			{
				return new LocalizedString("DateStyles", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupsIBelongToDescription
		{
			get
			{
				return new LocalizedString("GroupsIBelongToDescription", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StartTimeText
		{
			get
			{
				return new LocalizedString("StartTimeText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoveCommandText
		{
			get
			{
				return new LocalizedString("RemoveCommandText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PersonalSettingChangePassword
		{
			get
			{
				return new LocalizedString("PersonalSettingChangePassword", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PasswordLabel
		{
			get
			{
				return new LocalizedString("PasswordLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleHasAttachmentConditionText
		{
			get
			{
				return new LocalizedString("InboxRuleHasAttachmentConditionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarPublishingLinks
		{
			get
			{
				return new LocalizedString("CalendarPublishingLinks", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequirementsReadWriteItemValue
		{
			get
			{
				return new LocalizedString("RequirementsReadWriteItemValue", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImportContactListProgress
		{
			get
			{
				return new LocalizedString("ImportContactListProgress", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrialReminderActionLinkText
		{
			get
			{
				return new LocalizedString("TrialReminderActionLinkText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoiceMailNotificationsLink
		{
			get
			{
				return new LocalizedString("VoiceMailNotificationsLink", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMyNameInToCcBoxConditionText
		{
			get
			{
				return new LocalizedString("InboxRuleMyNameInToCcBoxConditionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstallFromFile
		{
			get
			{
				return new LocalizedString("InstallFromFile", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceMOWAVersionLabel
		{
			get
			{
				return new LocalizedString("DeviceMOWAVersionLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Subject
		{
			get
			{
				return new LocalizedString("Subject", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EditAccountCommandText
		{
			get
			{
				return new LocalizedString("EditAccountCommandText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutomaticRepliesScheduledCheckboxText
		{
			get
			{
				return new LocalizedString("AutomaticRepliesScheduledCheckboxText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RetentionActionTypeHeader
		{
			get
			{
				return new LocalizedString("RetentionActionTypeHeader", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QLOutlook
		{
			get
			{
				return new LocalizedString("QLOutlook", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleFromConditionText
		{
			get
			{
				return new LocalizedString("InboxRuleFromConditionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTrackingFailedEvent
		{
			get
			{
				return new LocalizedString("MessageTrackingFailedEvent", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncomingSecurityTLS
		{
			get
			{
				return new LocalizedString("IncomingSecurityTLS", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WithinSizeRangeConditionFormat
		{
			get
			{
				return new LocalizedString("WithinSizeRangeConditionFormat", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailCallFwdStep3
		{
			get
			{
				return new LocalizedString("VoicemailCallFwdStep3", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MyMailbox
		{
			get
			{
				return new LocalizedString("MyMailbox", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarPublishingDetail
		{
			get
			{
				return new LocalizedString("CalendarPublishingDetail", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceLastSuccessfulSyncLabel
		{
			get
			{
				return new LocalizedString("DeviceLastSuccessfulSyncLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClearButtonText
		{
			get
			{
				return new LocalizedString("ClearButtonText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleCopyToFolderActionText
		{
			get
			{
				return new LocalizedString("InboxRuleCopyToFolderActionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RPTYearsMonths
		{
			get
			{
				return new LocalizedString("RPTYearsMonths", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FromConditionFormat
		{
			get
			{
				return new LocalizedString("FromConditionFormat", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceUserAgentLabel
		{
			get
			{
				return new LocalizedString("DeviceUserAgentLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DepartGroupConfirmation
		{
			get
			{
				return new LocalizedString("DepartGroupConfirmation", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleWithImportanceConditionText
		{
			get
			{
				return new LocalizedString("InboxRuleWithImportanceConditionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PersonalSettingDomainUser
		{
			get
			{
				return new LocalizedString("PersonalSettingDomainUser", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SiteMailboxEmailMeDiagnosticsButtonString
		{
			get
			{
				return new LocalizedString("SiteMailboxEmailMeDiagnosticsButtonString", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeliveryManagement
		{
			get
			{
				return new LocalizedString("DeliveryManagement", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PersonalSettingPasswordBeforeChange
		{
			get
			{
				return new LocalizedString("PersonalSettingPasswordBeforeChange", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SmtpSetting
		{
			get
			{
				return new LocalizedString("SmtpSetting", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageFormatSlab
		{
			get
			{
				return new LocalizedString("MessageFormatSlab", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleForwardToActionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleForwardToActionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DuplicateProxyAddressError
		{
			get
			{
				return new LocalizedString("DuplicateProxyAddressError", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CreatedBy
		{
			get
			{
				return new LocalizedString("CreatedBy", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReadReceiptResponseNever
		{
			get
			{
				return new LocalizedString("ReadReceiptResponseNever", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AccessDeniedFooterBottom
		{
			get
			{
				return new LocalizedString("AccessDeniedFooterBottom", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OwnerChangedUpdateModerator
		{
			get
			{
				return new LocalizedString("OwnerChangedUpdateModerator", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AllowRecurringMeetings
		{
			get
			{
				return new LocalizedString("AllowRecurringMeetings", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailWizardStep1Title
		{
			get
			{
				return new LocalizedString("VoicemailWizardStep1Title", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxMembersString
		{
			get
			{
				return new LocalizedString("TeamMailboxMembersString", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AfterDateDisplayTemplate
		{
			get
			{
				return new LocalizedString("AfterDateDisplayTemplate", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TuesdayCheckBoxText
		{
			get
			{
				return new LocalizedString("TuesdayCheckBoxText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Join
		{
			get
			{
				return new LocalizedString("Join", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddAdditionalResponse
		{
			get
			{
				return new LocalizedString("AddAdditionalResponse", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OkButtonText
		{
			get
			{
				return new LocalizedString("OkButtonText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FoldersSyncedLabel
		{
			get
			{
				return new LocalizedString("FoldersSyncedLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SendToAllText
		{
			get
			{
				return new LocalizedString("SendToAllText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MaximumDurationInMinutesErrorMessage
		{
			get
			{
				return new LocalizedString("MaximumDurationInMinutesErrorMessage", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TextMessagingTurnedOnViaEas
		{
			get
			{
				return new LocalizedString("TextMessagingTurnedOnViaEas", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StartLoggingCommandText
		{
			get
			{
				return new LocalizedString("StartLoggingCommandText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxUsageLegacyText
		{
			get
			{
				return new LocalizedString("MailboxUsageLegacyText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RPTYears
		{
			get
			{
				return new LocalizedString("RPTYears", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReadReceiptResponseInstruction
		{
			get
			{
				return new LocalizedString("ReadReceiptResponseInstruction", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemindersEnabled
		{
			get
			{
				return new LocalizedString("RemindersEnabled", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddNewRequestsTentativelyCheckBoxText
		{
			get
			{
				return new LocalizedString("AddNewRequestsTentativelyCheckBoxText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTrackingSubmitEvent
		{
			get
			{
				return new LocalizedString("MessageTrackingSubmitEvent", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailLearnMore
		{
			get
			{
				return new LocalizedString("VoicemailLearnMore", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmailThisReport
		{
			get
			{
				return new LocalizedString("EmailThisReport", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarPublishingDateRange
		{
			get
			{
				return new LocalizedString("CalendarPublishingDateRange", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SelectUsersAndGroups
		{
			get
			{
				return new LocalizedString("SelectUsersAndGroups", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PhotoBookmark
		{
			get
			{
				return new LocalizedString("PhotoBookmark", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleHeaderContainsConditionText
		{
			get
			{
				return new LocalizedString("InboxRuleHeaderContainsConditionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BookingWindowInDaysErrorMessage
		{
			get
			{
				return new LocalizedString("BookingWindowInDaysErrorMessage", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Calendar
		{
			get
			{
				return new LocalizedString("Calendar", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleSubjectContainsAndMoveToFolderTemplate
		{
			get
			{
				return new LocalizedString("RuleSubjectContainsAndMoveToFolderTemplate", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleStateOn
		{
			get
			{
				return new LocalizedString("RuleStateOn", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserLogonNameLabel
		{
			get
			{
				return new LocalizedString("UserLogonNameLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RPTMonth
		{
			get
			{
				return new LocalizedString("RPTMonth", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EndTime
		{
			get
			{
				return new LocalizedString("EndTime", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstallFromPrivateUrlTitle
		{
			get
			{
				return new LocalizedString("InstallFromPrivateUrlTitle", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LastSyncAttemptTimeHeaderText
		{
			get
			{
				return new LocalizedString("LastSyncAttemptTimeHeaderText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequirementsReadWriteItemDescription
		{
			get
			{
				return new LocalizedString("RequirementsReadWriteItemDescription", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NewItemNotificationVoiceMailToast
		{
			get
			{
				return new LocalizedString("NewItemNotificationVoiceMailToast", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RenameDefaultFoldersCheckBoxText
		{
			get
			{
				return new LocalizedString("RenameDefaultFoldersCheckBoxText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleFromSubscriptionConditionText
		{
			get
			{
				return new LocalizedString("InboxRuleFromSubscriptionConditionText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RetentionSelectOptionalLabel
		{
			get
			{
				return new LocalizedString("RetentionSelectOptionalLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FromColumnLabel
		{
			get
			{
				return new LocalizedString("FromColumnLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExtensionCanNotBeDisabledNorUninstalled
		{
			get
			{
				return new LocalizedString("ExtensionCanNotBeDisabledNorUninstalled", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarPublishingPublic
		{
			get
			{
				return new LocalizedString("CalendarPublishingPublic", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarSharingExplanation
		{
			get
			{
				return new LocalizedString("CalendarSharingExplanation", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarPublishingViewUrl
		{
			get
			{
				return new LocalizedString("CalendarPublishingViewUrl", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SaturdayCheckBoxText
		{
			get
			{
				return new LocalizedString("SaturdayCheckBoxText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxUsageUnitB
		{
			get
			{
				return new LocalizedString("MailboxUsageUnitB", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HasAttachmentConditionFormat
		{
			get
			{
				return new LocalizedString("HasAttachmentConditionFormat", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailWizardStep5Title
		{
			get
			{
				return new LocalizedString("VoicemailWizardStep5Title", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PreviewMarkAsReadBehaviorNever
		{
			get
			{
				return new LocalizedString("PreviewMarkAsReadBehaviorNever", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubscriptionDialogLabel
		{
			get
			{
				return new LocalizedString("SubscriptionDialogLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NewSubscription
		{
			get
			{
				return new LocalizedString("NewSubscription", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LastSynchronization
		{
			get
			{
				return new LocalizedString("LastSynchronization", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ChangeCalendarPermissions
		{
			get
			{
				return new LocalizedString("ChangeCalendarPermissions", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DefaultReminderTimeLabel
		{
			get
			{
				return new LocalizedString("DefaultReminderTimeLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OpenNextItem
		{
			get
			{
				return new LocalizedString("OpenNextItem", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmailOptions
		{
			get
			{
				return new LocalizedString("EmailOptions", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RetentionActionTypeDefaultDelete
		{
			get
			{
				return new LocalizedString("RetentionActionTypeDefaultDelete", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ChangeCommandText
		{
			get
			{
				return new LocalizedString("ChangeCommandText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalMessageGalLessInstruction
		{
			get
			{
				return new LocalizedString("ExternalMessageGalLessInstruction", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImportContactListNoFileUploaded
		{
			get
			{
				return new LocalizedString("ImportContactListNoFileUploaded", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BadOfficeCallbackMessage
		{
			get
			{
				return new LocalizedString("BadOfficeCallbackMessage", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DefaultImage
		{
			get
			{
				return new LocalizedString("DefaultImage", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JoinAndDepart
		{
			get
			{
				return new LocalizedString("JoinAndDepart", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMyNameInToCcBoxConditionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleMyNameInToCcBoxConditionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxTitleString
		{
			get
			{
				return new LocalizedString("TeamMailboxTitleString", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NewestOnTop
		{
			get
			{
				return new LocalizedString("NewestOnTop", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ToWithExample
		{
			get
			{
				return new LocalizedString("ToWithExample", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupOwners
		{
			get
			{
				return new LocalizedString("GroupOwners", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DefaultRetentionTagDescription
		{
			get
			{
				return new LocalizedString("DefaultRetentionTagDescription", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Hours
		{
			get
			{
				return new LocalizedString("Hours", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTypeDialogTitle
		{
			get
			{
				return new LocalizedString("MessageTypeDialogTitle", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchDeliveryReports
		{
			get
			{
				return new LocalizedString("SearchDeliveryReports", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailCallFwdStep1
		{
			get
			{
				return new LocalizedString("VoicemailCallFwdStep1", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SendAddressSetting
		{
			get
			{
				return new LocalizedString("SendAddressSetting", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleItIsGroupText
		{
			get
			{
				return new LocalizedString("InboxRuleItIsGroupText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LaunchOfficeMarketplace
		{
			get
			{
				return new LocalizedString("LaunchOfficeMarketplace", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarDiagnosticLogDescription
		{
			get
			{
				return new LocalizedString("CalendarDiagnosticLogDescription", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRules
		{
			get
			{
				return new LocalizedString("InboxRules", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailSMSOptionVoiceMailAndMissedCalls
		{
			get
			{
				return new LocalizedString("VoicemailSMSOptionVoiceMailAndMissedCalls", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StartTime
		{
			get
			{
				return new LocalizedString("StartTime", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TextMessagingSlabMessage
		{
			get
			{
				return new LocalizedString("TextMessagingSlabMessage", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JoinDlSuccess(string name)
		{
			return new LocalizedString("JoinDlSuccess", "", false, false, OwaOptionStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString WipeDeviceConfirmMessageDetail
		{
			get
			{
				return new LocalizedString("WipeDeviceConfirmMessageDetail", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailWizardStep2DescriptionNoPasscode
		{
			get
			{
				return new LocalizedString("VoicemailWizardStep2DescriptionNoPasscode", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeleteInboxRulesConfirmation
		{
			get
			{
				return new LocalizedString("DeleteInboxRulesConfirmation", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarNotificationsLink
		{
			get
			{
				return new LocalizedString("CalendarNotificationsLink", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleHasClassificationConditionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleHasClassificationConditionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarWorkflowInstruction
		{
			get
			{
				return new LocalizedString("CalendarWorkflowInstruction", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutomaticRepliesInstruction
		{
			get
			{
				return new LocalizedString("AutomaticRepliesInstruction", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceLanguageLabel
		{
			get
			{
				return new LocalizedString("DeviceLanguageLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MoveUp
		{
			get
			{
				return new LocalizedString("MoveUp", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstallButtonText
		{
			get
			{
				return new LocalizedString("InstallButtonText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WithSensitivityConditionFormat
		{
			get
			{
				return new LocalizedString("WithSensitivityConditionFormat", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailWizardPinLabel
		{
			get
			{
				return new LocalizedString("VoicemailWizardPinLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MondayCheckBoxText
		{
			get
			{
				return new LocalizedString("MondayCheckBoxText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WithImportanceConditionFormat
		{
			get
			{
				return new LocalizedString("WithImportanceConditionFormat", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NewDistributionGroupText
		{
			get
			{
				return new LocalizedString("NewDistributionGroupText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarReminderSlab
		{
			get
			{
				return new LocalizedString("CalendarReminderSlab", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmailAddressesLabel
		{
			get
			{
				return new LocalizedString("EmailAddressesLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PortLabel
		{
			get
			{
				return new LocalizedString("PortLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetupVoiceMailNotificationsLink
		{
			get
			{
				return new LocalizedString("SetupVoiceMailNotificationsLink", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TextMessagingOff
		{
			get
			{
				return new LocalizedString("TextMessagingOff", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleSentToAndMoveToFolderTemplate
		{
			get
			{
				return new LocalizedString("RuleSentToAndMoveToFolderTemplate", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserNameNotSetError
		{
			get
			{
				return new LocalizedString("UserNameNotSetError", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxUsageWarningText
		{
			get
			{
				return new LocalizedString("MailboxUsageWarningText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PopSetting
		{
			get
			{
				return new LocalizedString("PopSetting", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleFromAddressContainsConditionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleFromAddressContainsConditionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Options
		{
			get
			{
				return new LocalizedString("Options", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EMailSignatureSlab
		{
			get
			{
				return new LocalizedString("EMailSignatureSlab", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RetentionPeriodHoldFor
		{
			get
			{
				return new LocalizedString("RetentionPeriodHoldFor", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MaximumConflictInstancesErrorMessage
		{
			get
			{
				return new LocalizedString("MaximumConflictInstancesErrorMessage", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Organize
		{
			get
			{
				return new LocalizedString("Organize", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTrackingDLExpandedEvent
		{
			get
			{
				return new LocalizedString("MessageTrackingDLExpandedEvent", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailBrowserNotSupported
		{
			get
			{
				return new LocalizedString("VoicemailBrowserNotSupported", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SendAddressSettingSlabDescription
		{
			get
			{
				return new LocalizedString("SendAddressSettingSlabDescription", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RetrieveLogConfirmMessage
		{
			get
			{
				return new LocalizedString("RetrieveLogConfirmMessage", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DescriptionLabel
		{
			get
			{
				return new LocalizedString("DescriptionLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomAttributeDialogTitle
		{
			get
			{
				return new LocalizedString("CustomAttributeDialogTitle", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageApproval
		{
			get
			{
				return new LocalizedString("MessageApproval", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailAccessNumbersTemplate(string number)
		{
			return new LocalizedString("VoicemailAccessNumbersTemplate", "", false, false, OwaOptionStrings.ResourceManager, new object[]
			{
				number
			});
		}

		public static LocalizedString TeamMailboxPropertiesString
		{
			get
			{
				return new LocalizedString("TeamMailboxPropertiesString", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BodyContainsConditionFormat
		{
			get
			{
				return new LocalizedString("BodyContainsConditionFormat", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupModeration
		{
			get
			{
				return new LocalizedString("GroupModeration", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FreeBusySubjectLocationInformation
		{
			get
			{
				return new LocalizedString("FreeBusySubjectLocationInformation", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMarkMessageGroupText
		{
			get
			{
				return new LocalizedString("InboxRuleMarkMessageGroupText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleConditionSectionHeader
		{
			get
			{
				return new LocalizedString("InboxRuleConditionSectionHeader", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecipientEmailButtonDescription
		{
			get
			{
				return new LocalizedString("RecipientEmailButtonDescription", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TimeZone
		{
			get
			{
				return new LocalizedString("TimeZone", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleSentToConditionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleSentToConditionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MoveDown
		{
			get
			{
				return new LocalizedString("MoveDown", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnblockDeviceCommandText
		{
			get
			{
				return new LocalizedString("UnblockDeviceCommandText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TextMessagingSms
		{
			get
			{
				return new LocalizedString("TextMessagingSms", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LanguageInstruction
		{
			get
			{
				return new LocalizedString("LanguageInstruction", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleFromConditionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleFromConditionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OutOfOffice
		{
			get
			{
				return new LocalizedString("OutOfOffice", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FirstNameLabel
		{
			get
			{
				return new LocalizedString("FirstNameLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleBodyContainsConditionFlyOutText
		{
			get
			{
				return new LocalizedString("InboxRuleBodyContainsConditionFlyOutText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AllowedSendersLabelForEndUser
		{
			get
			{
				return new LocalizedString("AllowedSendersLabelForEndUser", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailWizardStep2Title
		{
			get
			{
				return new LocalizedString("VoicemailWizardStep2Title", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversationsSlab
		{
			get
			{
				return new LocalizedString("ConversationsSlab", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutomaticRepliesSlab
		{
			get
			{
				return new LocalizedString("AutomaticRepliesSlab", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncomingAuthenticationNtlm
		{
			get
			{
				return new LocalizedString("IncomingAuthenticationNtlm", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MobilePhoneLabel
		{
			get
			{
				return new LocalizedString("MobilePhoneLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceNameLabel
		{
			get
			{
				return new LocalizedString("DeviceNameLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HeaderContainsConditionFormat
		{
			get
			{
				return new LocalizedString("HeaderContainsConditionFormat", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxDescription
		{
			get
			{
				return new LocalizedString("TeamMailboxDescription", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EnterNumberClickNext
		{
			get
			{
				return new LocalizedString("EnterNumberClickNext", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MobileDevices
		{
			get
			{
				return new LocalizedString("MobileDevices", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DisplayRecoveryPasswordCommandDescription
		{
			get
			{
				return new LocalizedString("DisplayRecoveryPasswordCommandDescription", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AtMostOnlyDisplayTemplate
		{
			get
			{
				return new LocalizedString("AtMostOnlyDisplayTemplate", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleSentToConditionPreCannedText
		{
			get
			{
				return new LocalizedString("InboxRuleSentToConditionPreCannedText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMarkedWithGroupText
		{
			get
			{
				return new LocalizedString("InboxRuleMarkedWithGroupText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Membership
		{
			get
			{
				return new LocalizedString("Membership", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailSlab
		{
			get
			{
				return new LocalizedString("VoicemailSlab", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AllInformation
		{
			get
			{
				return new LocalizedString("AllInformation", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BlockOrAllow
		{
			get
			{
				return new LocalizedString("BlockOrAllow", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarDiagnosticLogWatermarkText
		{
			get
			{
				return new LocalizedString("CalendarDiagnosticLogWatermarkText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EditGroups
		{
			get
			{
				return new LocalizedString("EditGroups", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TurnOnTextMessaging
		{
			get
			{
				return new LocalizedString("TurnOnTextMessaging", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AliasLabelForEnterprise
		{
			get
			{
				return new LocalizedString("AliasLabelForEnterprise", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CategoryDialogTitle
		{
			get
			{
				return new LocalizedString("CategoryDialogTitle", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetYourWorkingHours
		{
			get
			{
				return new LocalizedString("SetYourWorkingHours", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProfileMailboxUsage
		{
			get
			{
				return new LocalizedString("ProfileMailboxUsage", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AtLeastAtMostDisplayTemplate
		{
			get
			{
				return new LocalizedString("AtLeastAtMostDisplayTemplate", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotificationsForMeetingReminders
		{
			get
			{
				return new LocalizedString("NotificationsForMeetingReminders", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncomingServerLabel
		{
			get
			{
				return new LocalizedString("IncomingServerLabel", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncomingAuthenticationNone
		{
			get
			{
				return new LocalizedString("IncomingAuthenticationNone", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrialReminderText
		{
			get
			{
				return new LocalizedString("TrialReminderText", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupsIBelongToAndGroupsIOwnDescription
		{
			get
			{
				return new LocalizedString("GroupsIBelongToAndGroupsIOwnDescription", "", false, false, OwaOptionStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(OwaOptionStrings.IDs key)
		{
			return new LocalizedString(OwaOptionStrings.stringIDs[(uint)key], OwaOptionStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(839);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Management.ControlPanel.OwaOptionStrings", typeof(OwaOptionStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			NeverSyncText = 4145884488U,
			FromAddressContainsConditionFormat = 1481793251U,
			CalendarPublishingBasic = 2278445393U,
			ChangePhoneNumber = 1471007325U,
			TimeZoneNote = 566587615U,
			ShowWorkWeekAsCheckBoxText = 2785077264U,
			ViewInboxRule = 85271849U,
			DeviceMobileOperatorLabel = 3270571164U,
			FinishButtonText = 62320074U,
			UserNameMOSIDLabel = 3179829084U,
			ChangeMyMobilePhoneSettings = 3449770263U,
			RequirementsReadWriteMailboxDescription = 2657242021U,
			MessageTypeMatchesConditionFormat = 1654895332U,
			NewRoomCreationWarningText = 226228553U,
			InboxRuleMyNameInToBoxConditionFlyOutText = 932151145U,
			FirstSyncOnLabel = 2712198432U,
			DeleteGroupConfirmation = 2830841285U,
			PendingWipeCommandIssuedLabel = 2230814600U,
			OwnerChangedModerationReminder = 3090796481U,
			InboxRuleFromConditionPreCannedText = 495329060U,
			MaximumConflictInstances = 3366412874U,
			EmailSubscriptions = 2502183272U,
			InboxRuleFlaggedForActionConditionFlyOutText = 2174801140U,
			ViewRPTDurationLabel = 3159390523U,
			OnOffColumn = 926911138U,
			FromSubscriptionConditionFormat = 4270339173U,
			EmailComposeModeSeparateForm = 2476211802U,
			ReadingPaneSlab = 1739026864U,
			OOF = 77678270U,
			SearchResultsCaption = 1330353058U,
			SubjectLabel = 3130274824U,
			Minute = 2220842206U,
			SendAtColon = 753397160U,
			NewCommandText = 177845278U,
			RetentionTypeRequired = 4084881753U,
			UninstallExtensionsConfirmation = 1138078555U,
			InboxRuleHasAttachmentConditionFlyOutText = 3158885598U,
			MyselfEntFormat = 2285322212U,
			ConnectedAccounts = 3472797073U,
			GroupNotes = 1659711502U,
			Status = 1959773104U,
			TeamMailboxSyncStatusString = 2313850175U,
			AccountSecondaryNavigation = 993996681U,
			EmailSubscriptionSlabDescription = 2256263033U,
			TeamMailboxMailString = 3692628685U,
			TeamMailboxLifecycleStatusString2 = 166656410U,
			JunkEmailTrustedListDescription = 298572739U,
			SundayCheckBoxText = 3700740504U,
			ExtensionVersionTag = 3309799253U,
			MailTip = 2902697354U,
			CalendarPublishingRestricted = 861811732U,
			MailboxUsageUnavailable = 884476035U,
			Customize = 2108510325U,
			ModerationEnabled = 1115629481U,
			PreviewMarkAsReadBehaviorDelayed = 3758991955U,
			ShareInformation = 3807282937U,
			RetentionActionTypeArchive = 1441613828U,
			SetUpNotifications = 2977853317U,
			InboxRuleMoveToFolderActionFlyOutText = 2600878742U,
			JunkEmailContactsTrusted = 854412308U,
			TeamMailboxManagementString = 3522427619U,
			MessageTrackingTransferredEvent = 636192742U,
			SendToAllGalLessText = 2329485994U,
			CalendarReminderInstruction = 2574861626U,
			TotalMembers = 3667432429U,
			MailboxUsageUnlimitedText = 1262029551U,
			CalendarTroubleshootingLinkText = 394064455U,
			DisplayRecoveryPasswordCommandText = 2661286284U,
			VoicemailWizardStep4Description = 366963388U,
			IncomingSecurityLabel = 365990270U,
			InboxRuleForwardToActionText = 1133353571U,
			InboxRuleMyNameIsGroupText = 3807748391U,
			MailboxFolderDialogLabel = 3420690180U,
			ReturnToView = 21771688U,
			DeviceActiveSyncVersionLabel = 469603767U,
			InstallFromPrivateUrlCaption = 4036777603U,
			DeleteEmailSubscriptionConfirmation = 3877512413U,
			VoicemailWizardComplete = 3119904789U,
			InboxRuleMarkAsReadActionFlyOutText = 303742195U,
			RPTDay = 3738802218U,
			DeviceAccessStateSetByLabel = 2590635398U,
			ViewGroupDetails = 1799036564U,
			ToOnlyLabel = 2408048685U,
			SensitivityDialogTitle = 2384846465U,
			TeamMailboxLifecycleStatusString = 1200890284U,
			WednesdayCheckBoxText = 3028241800U,
			ExtensionRequirementsLabel = 2254792003U,
			AlwaysShowBcc = 2459846522U,
			ConflictPercentageAllowedErrorMessage = 1703184725U,
			JoinRestrictionApprovalRequiredDetails = 3413282040U,
			InboxRuleMarkImportanceActionText = 1917681062U,
			InboxRuleRecipientAddressContainsConditionFlyOutText = 1073903281U,
			Regional = 2228506517U,
			VoicemailWizardTestDoneText = 3644924947U,
			RemoveOldMeetingMessagesCheckBoxText = 2743720278U,
			InboxRuleBodyContainsConditionText = 63766217U,
			QLForward = 1435597588U,
			VoicemailPhoneNumberColon = 4103313421U,
			AddCommandText = 1416133447U,
			Voicemail = 3917579091U,
			StringArrayDialogTitle = 2321140182U,
			MailboxUsageUnitTB = 2696088269U,
			CalendarPublishingCopyLink = 1272238732U,
			TimeStyles = 1224467389U,
			RPTYear = 559979509U,
			VoicemailLearnMoreVideo = 2493642031U,
			InboxRuleHeaderContainsConditionFlyOutText = 2564457623U,
			InboxRuleHasClassificationConditionText = 1916260782U,
			ImportContactListPage1Caption = 1806836259U,
			VoicemailWizardStep2Description = 3804549298U,
			AddExtension = 1269143998U,
			WithinSizeRangeDialogTitle = 2355901091U,
			GetCalendarLogButtonText = 2269401639U,
			BlockDeviceConfirmMessage = 954238138U,
			CalendarPublishingRangeFrom = 1294877450U,
			EnterPasscodeStepMessage = 3777014377U,
			VoicemailCallFwdHavingTrouble = 3308373688U,
			StartForward = 3543278019U,
			VoicemailCallFwdStep2 = 2118875232U,
			JoinRestrictionOpenDetails = 1225604874U,
			IncomingSecurityNone = 2863884442U,
			InboxRuleMyNameNotInToBoxConditionText = 1827349577U,
			ChangePermissions = 2515135494U,
			InboxRuleCopyToFolderActionFlyOutText = 1807321962U,
			InboxRuleSubjectContainsConditionFlyOutText = 3118863998U,
			RequirementsRestrictedValue = 1238462568U,
			InboxRuleRedirectToActionText = 394662652U,
			ImportContactListPage1Step2 = 1472234713U,
			JunkEmailWatermarkText = 1087517319U,
			TextMessagingStatusPrefixStatus = 3803411495U,
			ShowHoursIn = 4182339999U,
			DefaultFormat = 3390931208U,
			SubscriptionDialogTitle = 2893153135U,
			NewItemNotificationEmailToast = 3581746357U,
			TeamMailboxTabUsersHelpString1 = 994337869U,
			SchedulingPermissionsSlab = 307942560U,
			ConversationSortOrderInstruction = 3141117137U,
			WipeDeviceCommandText = 3348884585U,
			InboxRuleSentOrReceivedGroupText = 757309800U,
			Myself = 1884883826U,
			NewestOnBottom = 885509158U,
			NewItemNotificationFaxToast = 4104125374U,
			EmailComposeModeInline = 4049797668U,
			NewRuleString = 880505963U,
			NoMessageCategoryAvailable = 2688608043U,
			CurrentStatus = 1328547437U,
			SubscriptionProcessingError = 1395647162U,
			StopAndRetrieveLogCommandText = 1075069807U,
			TimeIncrementThirtyMinutes = 1917268543U,
			RetentionActionNeverMove = 3253517897U,
			VoicemailMobileOperatorColon = 3600602092U,
			ConnectedAccountsDescriptionForForwarding = 2788873687U,
			StopForward = 2316526399U,
			FirstWeekOfYear = 3739699132U,
			RegionListLabel = 2986818840U,
			InstallFromMarketplace = 2982857632U,
			RulesNameColumn = 738818108U,
			DeviceOSLabel = 3166948736U,
			InboxRuleSentOnlyToMeConditionText = 781854721U,
			EditYourPassword = 364458648U,
			EnforceSchedulingHorizon = 3609822921U,
			TeamMailboxManagementString2 = 872104809U,
			SearchMessageTipForIWUser = 3459526114U,
			ConnectedAccountsDescriptionForSubscription = 904314811U,
			QLManageOrganization = 3180823751U,
			JoinRestrictionApprovalRequired = 2201160322U,
			ExtensionCanNotBeUninstalled = 3344174056U,
			VoicemailWizardStep4Title = 1952564584U,
			ViewExtensionDetails = 1001261206U,
			VoicemailCarrierRatesMayApply = 4181395631U,
			DeliveryReports = 2288925761U,
			AllRequestOutOfPolicyText = 3052389362U,
			RemoveDeviceConfirmMessage = 3585825647U,
			StatusLabel = 1401723706U,
			InboxRuleSubjectOrBodyContainsConditionText = 3255907698U,
			OwnerLabel = 1547367963U,
			RequireSenderAuthFalse = 3925202351U,
			AllowedSendersLabel = 2460807174U,
			IncomingSecuritySSL = 759422978U,
			CarrierListLabel = 952725660U,
			InboxRuleDescriptionNote = 3525277106U,
			NewImapSubscription = 3736448266U,
			TeamMailboxStartSyncButtonString = 3089967521U,
			NotificationsForCalendarUpdate = 2243312622U,
			ReadReceiptsSlab = 1134119669U,
			DetailsLinkText = 1550773931U,
			Help = 1454393937U,
			SearchGroups = 491491954U,
			ShowConversationAsTreeInstruction = 3108971784U,
			BypassModerationSenders = 3137025124U,
			RetentionActionDeleteAndAllowRecovery = 3781814528U,
			PreviewMarkAsReadDelaytimeTextPre = 999443141U,
			RPTMonths = 565319905U,
			AfterMoveOrDeleteBehavior = 1719300497U,
			HideGroupFromAddressLists = 1359223188U,
			VoicemailWizardStep1Description = 2825394221U,
			ReviewLinkText = 2930172971U,
			Processing = 2325389167U,
			DailyCalendarAgendas = 312564594U,
			PreviewMarkAsReadBehaviorOnSelectionChange = 1969050778U,
			TimeZoneLabelText = 1817328658U,
			QLVoiceMail = 3165477128U,
			VoicemailSignUpIntro = 1490355023U,
			VoicemailStep2 = 4294954307U,
			TeamMailboxMembershipString = 519599668U,
			PasscodeLabel = 3463577248U,
			PersonalSettingPasswordAfterChange = 1836294011U,
			VerificationSuccessPageTitle = 3090647821U,
			EnableAutomaticProcessingNote = 843476083U,
			Days = 2422328107U,
			NotificationsNotSetUp = 521914504U,
			ModerationNotificationsInternal = 2608069889U,
			ProtocolSettings = 3040964019U,
			EnableAutomaticProcessing = 4275718891U,
			MessageOptionsSlab = 2045071371U,
			ChooseMessageFont = 2789466833U,
			Password = 3563093647U,
			OWAExtensions = 1901616043U,
			StringArrayDialogLabel = 664237562U,
			Unlimited = 944449543U,
			VoicemailSMSOptionVoiceMailOnly = 4033068416U,
			Rules = 3853542865U,
			ModeratedByEmptyDataText = 860120982U,
			TextMessaging = 499094223U,
			FromLabel = 2038251428U,
			GroupModerators = 2108668357U,
			ReadingPaneInstruction = 4089833856U,
			RPTNone = 1749888754U,
			Spelling = 2618236676U,
			CancelWipeDeviceCommandText = 3198693159U,
			AutomaticRepliesEnabledText = 3747885709U,
			DisplayNameLabel = 2372820691U,
			CancelButtonText = 2449169153U,
			GroupMembershipApproval = 3156651890U,
			InlcudedRecipientTypesLabel = 3398667566U,
			Name = 2328219947U,
			RetentionActionTypeDelete = 590111363U,
			InboxRuleMyNameInCcBoxConditionFlyOutText = 3652102134U,
			ThursdayCheckBoxText = 2328532194U,
			JoinGroup = 258131565U,
			Account = 767759945U,
			InboxRuleMessageTypeMatchesConditionFlyOutText = 1107043549U,
			InboxRuleMoveCopyDeleteGroupText = 1934149463U,
			RegionalSettingsInstruction = 294641626U,
			NameColumn = 1009337581U,
			InboxRuleWithSensitivityConditionFlyOutText = 2921806668U,
			ClassificationDialogTitle = 744201260U,
			RuleFromAndMoveToFolderTemplate = 2012947571U,
			DomainNameNotSetError = 2112722880U,
			MakeSecurityGroup = 3382481163U,
			ContactNumbersBookmark = 111122248U,
			InboxRuleSentToConditionText = 3443085801U,
			MemberOfGroups = 1620775745U,
			InboxRuleIncludeTheseWordsGroupText = 2393490804U,
			MailTipLabel = 691241872U,
			MessageTypeDialogLabel = 1413097981U,
			RegionalSettingsSlab = 1541052658U,
			VoicemailWizardStep3Title = 346591353U,
			InboxRuleMyNameNotInToBoxConditionFlyOutText = 2731962334U,
			ReturnToOWA = 510973434U,
			InboxRuleMyNameInToBoxConditionText = 1564510770U,
			CommitButtonText = 3567818904U,
			TeamMailboxShowInMyClientString = 1864411629U,
			InboxRuleMarkAsReadActionText = 2397374352U,
			ClassificationDialogLabel = 663639812U,
			WarningAlt = 3313607735U,
			TeamMailboxManagementString4 = 2034904223U,
			Mail = 405905481U,
			ImportContactList = 624357391U,
			QLImportContacts = 1680686955U,
			InboxRule = 3391394280U,
			WithinDateRangeDialogTitle = 3979669380U,
			ReminderSoundEnabled = 1449143764U,
			RecipientAddressContainsConditionFormat = 1634623908U,
			MessageFormatPlainText = 481168139U,
			DeleteInboxRuleConfirmation = 203738408U,
			ForwardEmailTitle = 2879015231U,
			BypassModerationSendersEmptyDataText = 3395154598U,
			RuleSubjectContainsAndDeleteMessageTemplate = 396113514U,
			HideDeletedItems = 1053461607U,
			VoicemailSetupNowButtonText = 3137581701U,
			CalendarPermissions = 797589720U,
			ModerationNotificationsAlways = 1108157303U,
			ExternalMessageInstruction = 1038822924U,
			RequirementsReadItemValue = 2936411810U,
			SchedulingOptionsSlab = 4225166530U,
			ShowConversationTree = 742094856U,
			RetentionPolicies = 1303277130U,
			CalendarPublishingSubscriptionUrl = 2271033387U,
			ResendVerificationEmailCommandText = 1808002020U,
			TextMessagingNotification = 2949887068U,
			InstalledByColumn = 3246855305U,
			GroupOrganizationalUnit = 3100669839U,
			MailboxFolderDialogTitle = 1750301704U,
			PersonalSettingOldPassword = 2669253750U,
			VoicemailStep3 = 2728870366U,
			CityLabel = 3140976517U,
			SentToConditionFormat = 1022114031U,
			QLSubscription = 260433958U,
			ViewRPTDetailsTitle = 803108465U,
			MyGroups = 3201916728U,
			TeamMailboxesString = 349837376U,
			DeliveryReport = 1220880656U,
			LastNameLabel = 3294022635U,
			CalendarPublishingStop = 813081783U,
			VoicemailWizardStep3Description = 3339583031U,
			QLWhatsNewForOrganizations = 2176445071U,
			ReadReceiptResponseAlways = 3853933336U,
			JunkEmailTrustedListsOnly = 3555736038U,
			MatchSortOrder = 2391157043U,
			DevicePhoneNumberLabel = 2421998101U,
			InboxRuleMessageTypeMatchesConditionText = 3176989690U,
			RetentionTypeOptional = 4266991160U,
			UseSettings = 3996928596U,
			SearchAllGroups = 1958633523U,
			MembersLabel = 2117869041U,
			FreeBusyInformation = 1063608921U,
			DeviceIMEILabel = 133785142U,
			Day = 696030412U,
			InboxRuleMoveToFolderActionText = 192786603U,
			SelectOne = 779120846U,
			InboxRuleApplyCategoryActionFlyOutText = 846466190U,
			SetupEmailNotificationsLink = 3990783281U,
			NewDistributionGroupCaption = 117083423U,
			ViewRPTRetentionActionLabel = 1217964679U,
			ShowWeekNumbersCheckBoxText = 2135021465U,
			UserNameWLIDLabel = 1136770150U,
			SearchMessagesIReceivedLabel = 2028785684U,
			InboxRuleWithinDateRangeConditionText = 2151247442U,
			InboxRuleSendTextMessageNotificationToActionText = 839503915U,
			SentLabel = 3985530922U,
			GroupInformation = 2324730983U,
			VoicemailAskOperator = 2601154324U,
			OWA = 4016415205U,
			MailTipWaterMark = 574892128U,
			InboxRuleWithSensitivityConditionText = 969182653U,
			RetentionActionTypeDefaultArchive = 1090313663U,
			RemoveOptionalRPTConfirmation = 454922625U,
			DevicePolicyUpdateTimeLabel = 1570694400U,
			MyselfLiveFormat = 1674956465U,
			EmailDomain = 2143618034U,
			RequireSenderAuthTrue = 148346506U,
			InstallFromPrivateUrlInformation = 774036985U,
			PhoneLabel = 446237888U,
			SubscriptionAction = 731708393U,
			Weeks = 2401508539U,
			InboxRuleForwardRedirectGroupText = 3825281971U,
			DisplayName = 3719582777U,
			MembershipApprovalLabel = 3507262891U,
			InboxRuleSendTextMessageNotificationToActionFlyOutText = 1542072756U,
			TeamMailboxSPSiteString = 3475829388U,
			InboxRuleSubjectOrBodyContainsConditionFlyOutText = 1559211265U,
			PleaseWait = 1690161621U,
			JoinRestrictionOpen = 258867536U,
			CalendarSharingConfirmDeletionSingle = 418352798U,
			InboxRuleSubjectContainsConditionPreCannedText = 1869505729U,
			CalendarPublishingStart = 2815967975U,
			TextMessagingSlabMessageNotificationOnly = 1632853873U,
			RequirementsRestrictedDescription = 3304439859U,
			SelectAUser = 4114414654U,
			NotificationStepOneMessage = 3042824760U,
			QLPushEmail = 2897641359U,
			NewInboxRuleTitle = 1890566340U,
			SendToKnownContactsText = 929330042U,
			IncomingAuthenticationSpa = 48495520U,
			MailboxUsageUnitGB = 2696088282U,
			MessageTrackingDeliveredEvent = 3651870140U,
			SelectOneOrMoreText = 3772909321U,
			InboxRuleForwardAsAttachmentToActionText = 2600859168U,
			DontSeeMyRegionOrMobileOperator = 1105583403U,
			PreviewMarkAsReadDelaytimeTextPost = 1436160346U,
			NoMessageClassificationAvailable = 2060430325U,
			TeamMailboxTabPropertiesHelpString = 3929633211U,
			TeamMailboxManagementString1 = 2438188750U,
			RetentionPeriodHeader = 4047036620U,
			Phone = 4263477646U,
			WhoHasPermission = 2862314013U,
			NotAvailable = 1766818386U,
			FlaggedForActionConditionFormat = 1253897847U,
			EndTimeText = 4028994607U,
			BookingWindowInDays = 234118977U,
			RemovingPreviewPhoto = 3253425509U,
			CalendarDiagnosticLogSlab = 1666931485U,
			ModerationNotification = 3454695329U,
			VoicemailWizardPinlessOptionsText = 1207804035U,
			VoicemailClearSettings = 2476883967U,
			PersonalGroups = 1094425034U,
			DistributionGroupText = 259319032U,
			ProfileContactNumbers = 1335943323U,
			DeliveryReportFor = 167657761U,
			InboxRuleRedirectToActionFlyOutText = 232975871U,
			AutomateProcessing = 2251801313U,
			CalendarPublishingAccessLevel = 3493646365U,
			ForwardEmailTo = 248447020U,
			SettingUpProtocolAccess = 2062923077U,
			AdminTools = 1732697476U,
			InstalledExtensionDescription = 1669803655U,
			EmailComposeModeInstruction = 568490923U,
			InboxRuleDeleteMessageActionText = 3271192817U,
			SummaryToDate = 442231679U,
			Extension = 2631270417U,
			CalendarSharingConfirmDeletionMultiple = 1549389490U,
			Depart = 1314739276U,
			EmailNotificationsLink = 2193710474U,
			OpenPreviousItem = 1192767596U,
			RPTPickerDialogTitle = 1886990352U,
			CheckForForgottenAttachments = 625886851U,
			FaxLabel = 2755319165U,
			SentTime = 2677919833U,
			DeviceTypeHeaderText = 673404970U,
			RPTExpireNever = 338457679U,
			TeamMailboxEmailAddressString = 779255172U,
			InboxRuleWithinSizeRangeConditionText = 530176925U,
			Week = 2213908942U,
			WithinDateRangeConditionFormat = 2185035188U,
			DisplayNameNotSetError = 2502896170U,
			FirstDayOfWeek = 198361899U,
			ProcessExternalMeetingMessagesCheckBoxText = 2556260573U,
			DepartRestrictionClosed = 2041861674U,
			MailTipShortLabel = 886092638U,
			StateOrProvinceLabel = 2483718204U,
			PrimaryEmailAddress = 67466988U,
			AllowedSendersEmptyLabel = 2578200319U,
			HotmailSubscription = 1878748957U,
			DeviceIDLabel = 2638705563U,
			AlwaysShowFrom = 1891390470U,
			SearchButtonText = 1795719989U,
			ViewRPTDescriptionLabel = 4001240631U,
			Hour = 2624099920U,
			FlagStatusDialogTitle = 3567033964U,
			NewSubscriptionProgress = 1463926066U,
			EditProfilePhoto = 2455977975U,
			InstallFromPrivateUrl = 614024525U,
			DeleteGroupsConfirmation = 2869098816U,
			OwnedGroups = 870015935U,
			NewDistributionGroupTitle = 2322301053U,
			JunkEmailConfiguration = 2562827638U,
			ProfileGeneral = 3873537801U,
			CalendarSharingOutlookNote = 2344432611U,
			RemoveOptionalRPTsConfirmation = 829380600U,
			VerificationSuccessTitle = 2472855514U,
			ResponseMessageSlab = 3585330084U,
			TeamMailboxMaintenanceString = 1695816201U,
			UrlLabelText = 1565811752U,
			DepartRestrictionOpen = 3765209046U,
			PendingWipeCommandSentLabel = 2239331369U,
			SubjectOrBodyContainsConditionFormat = 2494867144U,
			LeaveMailOnServerLabel = 1747388690U,
			JoinRestrictionClosed = 2054798096U,
			ScheduleOnlyDuringWorkHours = 46775972U,
			ImportanceDialogLabel = 614963610U,
			CalendarPublishingLinkNotes = 3410351290U,
			InboxRuleSentOnlyToMeConditionFlyOutText = 1356158868U,
			InternalMessageInstruction = 2409360890U,
			JunkEmailDisabled = 1192855764U,
			InboxRuleForwardAsAttachmentToActionFlyOutText = 624477965U,
			WarningNote = 2343908218U,
			HomePagePrimaryLink = 4104737160U,
			LimitDuration = 837698603U,
			MailboxUsageExceededText = 3506885255U,
			UnSupportedRule = 3647997407U,
			DeviceModelLabel = 37810473U,
			NotificationLinksMessage = 2737072517U,
			ResourceSlab = 2702941902U,
			RetentionPolicyTagPageTitle = 4137250117U,
			AllBookInPolicyText = 1485698978U,
			MessageFormatHtml = 2179002981U,
			FridayCheckBoxText = 755136567U,
			InboxRuleMyNameInCcBoxConditionText = 3099627267U,
			PreviewMarkAsReadDelaytimeErrorMessage = 2592606558U,
			JunkEmailValidationErrorMessage = 3576971014U,
			TeamMailboxTabUsersHelpString2 = 3723221224U,
			AllRequestInPolicyText = 856468152U,
			ImapSubscription = 3383857716U,
			InboxRuleWithImportanceConditionFlyOutText = 2201998933U,
			TeamMailboxDisplayNameString = 600392665U,
			TeamMailboxManagementString3 = 3600988164U,
			ConflictPercentageAllowed = 1735921300U,
			ImportanceDialogTitle = 695525694U,
			SecurityGroupText = 265625760U,
			SubjectContainsConditionFormat = 1528543719U,
			VoicemailStep1 = 1566070952U,
			ImportContactListPage1Step1 = 1472234710U,
			CalendarPublishingRangeTo = 2919653011U,
			Installed = 924528516U,
			JoinRestrictionClosedDetails = 2532105814U,
			EnterNumberStepMessage = 3686719114U,
			TeamMailboxString = 3769321652U,
			ExternalAudienceCheckboxText = 1116185611U,
			RetentionActionNeverDelete = 3758567081U,
			TeamMailboxOwnersString = 3601061204U,
			SentOnlyToMeConditionFormat = 2454510675U,
			PostalCodeLabel = 685553614U,
			StreetAddressLabel = 2399940313U,
			ModerationNotificationsNever = 1949934660U,
			AutoAddSignature = 3043881226U,
			VoicemailConfiguration = 362303575U,
			TeamMailboxUsersString = 1231202734U,
			Minutes = 4116169389U,
			TextMessagingStatusPrefixNotifications = 1475482257U,
			OfficeLabel = 947054436U,
			SetupCalendarNotificationsLink = 3570705387U,
			WipeDeviceConfirmMessage = 2829325136U,
			JunkEmailEnabled = 1970261471U,
			ProfilePhoto = 2091217323U,
			JoinRestriction = 1213316404U,
			SubscriptionAccountInformation = 1603898988U,
			PopSubscription = 1983970360U,
			ConnectedAccountsDescriptionForSendAs = 1651913438U,
			VoicemailNotConfiguredText = 1355905147U,
			Date = 2328219770U,
			SubscriptionEmailAddress = 2118902877U,
			CalendarAppearanceInstruction = 1262918758U,
			DisableReminders = 779604653U,
			UninstallExtensionConfirmation = 58190828U,
			ReadReceiptResponseAskBefore = 272167933U,
			AutomaticRepliesDisabledText = 2721707952U,
			PermissionGranted = 2824009732U,
			CalendarTroubleShootingSlab = 473555910U,
			QLRemotePowerShell = 3318664764U,
			Ownership = 3266331791U,
			DevicePhoneNumberHeaderText = 1330376901U,
			OwnerApprovalRequired = 1467122827U,
			AddExtensionTitle = 936255600U,
			DevicePolicyApplicationStatusLabel = 2172431100U,
			CalendarWorkflowSlab = 3387549369U,
			SettingNotAvailable = 1249796258U,
			DepartRestriction = 1244143874U,
			RetentionTypeRequiredDescription = 3089991175U,
			RemoveForwardedMeetingNotificationsCheckBoxText = 4116953573U,
			Everyone = 3708929833U,
			TeamMailboxAppTitle = 3452175682U,
			PersonalSettingConfirmPassword = 2525011123U,
			MailboxUsageUnitMB = 2696088292U,
			SubscriptionServerInformation = 4155090378U,
			UserNameLabel = 2385969192U,
			TimeIncrementFifteenMinutes = 776327314U,
			LastSuccessfulSync = 2051542189U,
			SchedulingPermissionsInstruction = 4052999952U,
			EmptyDeletedItemsOnLogoff = 1517497942U,
			BeforeDateDisplayTemplate = 2251065475U,
			EmailAddressLabel = 2121350552U,
			JunkEmailBlockedListDescription = 1895146354U,
			NewItemNotificationSound = 3334007327U,
			NewInboxRuleCaption = 272267124U,
			UpdateTimeZoneNoteLinkText = 1462746207U,
			HasClassificationConditionFormat = 5631064U,
			NoneAccessRightRole = 705899704U,
			MobileDeviceDetailTitle = 3116350389U,
			EditCommandText = 624738140U,
			DeviceTypeLabel = 3623666006U,
			AllowConflicts = 4012923742U,
			CalendarPublishing = 3458491959U,
			RetentionNameHeader = 2717011154U,
			ImportContactListPage1InformationText = 1036626U,
			VoicemailAskPhoneNumber = 3055482209U,
			TeamMailboxDocumentsString = 453080500U,
			CountryOrRegionLabel = 3751262095U,
			CalendarPublishingLearnMore = 1816995256U,
			GroupModeratedBy = 1989730689U,
			DidntReceivePasscodeMessage = 4168470247U,
			InboxRuleFromAddressContainsConditionText = 3197162109U,
			IncomingAuthenticationLabel = 1889609004U,
			InboxRuleRecipientAddressContainsConditionText = 2280348446U,
			DepartGroupsConfirmation = 1975661589U,
			CalendarAppearanceSlab = 2084354126U,
			InitialsLabel = 2908173103U,
			InboxRuleFlaggedForActionConditionText = 3446713793U,
			QuickLinks = 3191690622U,
			TeamMailboxMyRoleString2 = 3789018700U,
			RoomEmailAddressLabel = 2359136379U,
			ToColumnLabel = 1233816147U,
			SensitivityDialogLabel = 1628295023U,
			AllowedSendersEmptyLabelForEndUser = 3728295286U,
			RequirementsReadWriteMailboxValue = 3699558192U,
			FlagStatusDialogLabel = 2323655060U,
			ImapSetting = 993324455U,
			VoicemailConfiguredText = 1286082276U,
			JunkEmailTrustedListHeader = 517504798U,
			RequirementsReadItemDescription = 1255618141U,
			RequireSenderAuth = 311875894U,
			IWantToEditMyNotificationSettings = 2656242830U,
			DevicePolicyAppliedLabel = 4223818683U,
			VoicemailSMSOptionNone = 980863707U,
			EditDistributionGroupTitle = 4218310877U,
			MaximumDurationInMinutes = 1540907116U,
			NewPopSubscription = 3482669506U,
			MobileDeviceHeadNoteInfo = 3896686788U,
			HomePhoneLabel = 4151878805U,
			BlockDeviceCommandText = 2680900463U,
			SelectUsers = 572559498U,
			SearchGroupsButtonDescription = 3849562096U,
			DeleteEmailSubscriptionsConfirmation = 382247602U,
			LearnHowToUseRedirectTo = 844022153U,
			InboxRuleDeleteMessageActionFlyOutText = 1008265668U,
			RetentionExplanationLabel = 187395633U,
			MessageTrackingPendingEvent = 2136591767U,
			ProviderColumn = 773553315U,
			SettingAccessDisabled = 718036160U,
			RPTDays = 1339697241U,
			InboxRuleFromSubscriptionConditionFlyOutText = 433722170U,
			SmtpAddressExample = 2080149828U,
			InboxRuleMarkImportanceActionFlyOutText = 759967779U,
			CategoryDialogLabel = 3100350500U,
			DeviceAccessStateLabel = 3531743005U,
			JunkEmailBlockedListHeader = 854461327U,
			VoicemailNotAvailableText = 3007171092U,
			ContactLocationBookmark = 290121153U,
			InboxRuleSubjectContainsConditionText = 1437070541U,
			VoicemailWizardConfirmPinLabel = 130154415U,
			NoSubscriptionAvailable = 1880859719U,
			QLPassword = 2399040354U,
			CustomAccessRightRole = 540558863U,
			ClearSettings = 570023042U,
			MailboxUsageUnitKB = 2696088286U,
			ExtensionPackageLocation = 3947650662U,
			InboxRuleApplyCategoryActionText = 3084881239U,
			OWAVoicemail = 2226605436U,
			PersonalSettingPassword = 853755201U,
			SendDuringWorkHoursOnly = 726021877U,
			AliasLabelForDataCenter = 677776990U,
			DeliverAndForward = 1351408741U,
			Language = 468777496U,
			NameAndAccountBookmark = 3918450461U,
			SendMyPhoneColon = 2977885649U,
			DateStyles = 2956146252U,
			GroupsIBelongToDescription = 4067290517U,
			StartTimeText = 2161945740U,
			RemoveCommandText = 91158280U,
			PersonalSettingChangePassword = 1057459323U,
			PasswordLabel = 3598785721U,
			InboxRuleHasAttachmentConditionText = 1986416405U,
			CalendarPublishingLinks = 988028286U,
			RequirementsReadWriteItemValue = 3784078605U,
			ImportContactListProgress = 1710677402U,
			TrialReminderActionLinkText = 874684615U,
			VoiceMailNotificationsLink = 2040917585U,
			InboxRuleMyNameInToCcBoxConditionText = 856915648U,
			InstallFromFile = 2736378449U,
			DeviceMOWAVersionLabel = 2000126950U,
			Subject = 1732412034U,
			EditAccountCommandText = 1266539147U,
			AutomaticRepliesScheduledCheckboxText = 3491325124U,
			RetentionActionTypeHeader = 3616424413U,
			QLOutlook = 3507358566U,
			InboxRuleFromConditionText = 3300930250U,
			MessageTrackingFailedEvent = 582625299U,
			IncomingSecurityTLS = 1068599355U,
			WithinSizeRangeConditionFormat = 961444931U,
			VoicemailCallFwdStep3 = 2118875233U,
			MyMailbox = 3710804460U,
			CalendarPublishingDetail = 2534471784U,
			DeviceLastSuccessfulSyncLabel = 1753433743U,
			ClearButtonText = 1279019904U,
			InboxRuleCopyToFolderActionText = 1541555783U,
			RPTYearsMonths = 1806917741U,
			FromConditionFormat = 3714415956U,
			DeviceUserAgentLabel = 908779566U,
			DepartGroupConfirmation = 2657380710U,
			InboxRuleWithImportanceConditionText = 2291011248U,
			PersonalSettingDomainUser = 492355799U,
			SiteMailboxEmailMeDiagnosticsButtonString = 1452418752U,
			DeliveryManagement = 2174687123U,
			PersonalSettingPasswordBeforeChange = 1249600476U,
			SmtpSetting = 1511367726U,
			MessageFormatSlab = 1472422U,
			InboxRuleForwardToActionFlyOutText = 3994376130U,
			DuplicateProxyAddressError = 3663958361U,
			CreatedBy = 2209887359U,
			ReadReceiptResponseNever = 2999683987U,
			AccessDeniedFooterBottom = 992729393U,
			OwnerChangedUpdateModerator = 900932107U,
			AllowRecurringMeetings = 1233400920U,
			VoicemailWizardStep1Title = 508731823U,
			TeamMailboxMembersString = 2265850499U,
			AfterDateDisplayTemplate = 417105304U,
			TuesdayCheckBoxText = 1413204805U,
			Join = 1367192090U,
			AddAdditionalResponse = 2408804455U,
			OkButtonText = 737660655U,
			FoldersSyncedLabel = 3175109089U,
			SendToAllText = 3789286439U,
			MaximumDurationInMinutesErrorMessage = 3957821083U,
			TextMessagingTurnedOnViaEas = 3260418603U,
			StartLoggingCommandText = 3047217661U,
			MailboxUsageLegacyText = 2811760173U,
			RPTYears = 1144080682U,
			ReadReceiptResponseInstruction = 143410217U,
			RemindersEnabled = 636289130U,
			AddNewRequestsTentativelyCheckBoxText = 1284433590U,
			MessageTrackingSubmitEvent = 1865553596U,
			VoicemailLearnMore = 1981955278U,
			EmailThisReport = 2288506116U,
			CalendarPublishingDateRange = 1435311110U,
			SelectUsersAndGroups = 3485844721U,
			PhotoBookmark = 1183459890U,
			InboxRuleHeaderContainsConditionText = 2440572518U,
			BookingWindowInDaysErrorMessage = 1356144580U,
			Calendar = 1292798904U,
			RuleSubjectContainsAndMoveToFolderTemplate = 1152863492U,
			RuleStateOn = 817924740U,
			UserLogonNameLabel = 3913001929U,
			RPTMonth = 2426358350U,
			EndTime = 1824826658U,
			InstallFromPrivateUrlTitle = 2917434001U,
			LastSyncAttemptTimeHeaderText = 3131435199U,
			RequirementsReadWriteItemDescription = 2572011012U,
			NewItemNotificationVoiceMailToast = 940101428U,
			RenameDefaultFoldersCheckBoxText = 2853482896U,
			InboxRuleFromSubscriptionConditionText = 1380850475U,
			RetentionSelectOptionalLabel = 1440211402U,
			FromColumnLabel = 1914666238U,
			ExtensionCanNotBeDisabledNorUninstalled = 524522579U,
			CalendarPublishingPublic = 2157690544U,
			CalendarSharingExplanation = 1380849357U,
			CalendarPublishingViewUrl = 1525092013U,
			SaturdayCheckBoxText = 3391167931U,
			MailboxUsageUnitB = 1194500941U,
			HasAttachmentConditionFormat = 2771220587U,
			VoicemailWizardStep5Title = 2993887811U,
			PreviewMarkAsReadBehaviorNever = 3866819705U,
			SubscriptionDialogLabel = 4136480461U,
			NewSubscription = 1033615903U,
			LastSynchronization = 826260386U,
			ChangeCalendarPermissions = 1189779660U,
			DefaultReminderTimeLabel = 3183463582U,
			OpenNextItem = 2947609428U,
			EmailOptions = 3213113944U,
			RetentionActionTypeDefaultDelete = 461256164U,
			ChangeCommandText = 3032482008U,
			ExternalMessageGalLessInstruction = 1054605049U,
			ImportContactListNoFileUploaded = 508072824U,
			BadOfficeCallbackMessage = 1294601601U,
			DefaultImage = 2511894476U,
			JoinAndDepart = 393831119U,
			InboxRuleMyNameInToCcBoxConditionFlyOutText = 3881794065U,
			TeamMailboxTitleString = 2202590644U,
			NewestOnTop = 1746211700U,
			ToWithExample = 2730942909U,
			GroupOwners = 1571878129U,
			DefaultRetentionTagDescription = 1456975951U,
			Hours = 2811696355U,
			MessageTypeDialogTitle = 643576831U,
			SearchDeliveryReports = 3392276543U,
			VoicemailCallFwdStep1 = 2118875235U,
			SendAddressSetting = 4130400892U,
			InboxRuleItIsGroupText = 787898729U,
			LaunchOfficeMarketplace = 1694176600U,
			CalendarDiagnosticLogDescription = 534802423U,
			InboxRules = 1084745363U,
			VoicemailSMSOptionVoiceMailAndMissedCalls = 942329033U,
			StartTime = 2138448017U,
			TextMessagingSlabMessage = 1074402166U,
			WipeDeviceConfirmMessageDetail = 1548729791U,
			VoicemailWizardStep2DescriptionNoPasscode = 2711159495U,
			DeleteInboxRulesConfirmation = 3410748733U,
			CalendarNotificationsLink = 3782284348U,
			InboxRuleHasClassificationConditionFlyOutText = 2245512691U,
			CalendarWorkflowInstruction = 3223016465U,
			AutomaticRepliesInstruction = 672959953U,
			DeviceLanguageLabel = 3095426072U,
			MoveUp = 137938150U,
			InstallButtonText = 810469194U,
			WithSensitivityConditionFormat = 3324278371U,
			VoicemailWizardPinLabel = 434719229U,
			MondayCheckBoxText = 2560210044U,
			WithImportanceConditionFormat = 984832422U,
			NewDistributionGroupText = 2489355482U,
			CalendarReminderSlab = 1716421970U,
			EmailAddressesLabel = 2879758792U,
			PortLabel = 1512647751U,
			SetupVoiceMailNotificationsLink = 2405760124U,
			TextMessagingOff = 1075596584U,
			RuleSentToAndMoveToFolderTemplate = 602937486U,
			UserNameNotSetError = 1393180149U,
			MailboxUsageWarningText = 1065442140U,
			PopSetting = 2112942921U,
			InboxRuleFromAddressContainsConditionFlyOutText = 4023555694U,
			Options = 1511584348U,
			EMailSignatureSlab = 3474229968U,
			RetentionPeriodHoldFor = 3820898101U,
			MaximumConflictInstancesErrorMessage = 1362687599U,
			Organize = 3952282363U,
			MessageTrackingDLExpandedEvent = 4129994431U,
			VoicemailBrowserNotSupported = 1620472852U,
			SendAddressSettingSlabDescription = 1877028846U,
			RetrieveLogConfirmMessage = 2765871439U,
			DescriptionLabel = 460637234U,
			CustomAttributeDialogTitle = 3776153215U,
			MessageApproval = 527639998U,
			TeamMailboxPropertiesString = 931483573U,
			BodyContainsConditionFormat = 2650315815U,
			GroupModeration = 3936874395U,
			FreeBusySubjectLocationInformation = 2556623818U,
			InboxRuleMarkMessageGroupText = 3000766872U,
			InboxRuleConditionSectionHeader = 2673767273U,
			RecipientEmailButtonDescription = 3741421195U,
			TimeZone = 1779041463U,
			InboxRuleSentToConditionFlyOutText = 286364476U,
			MoveDown = 3496306419U,
			UnblockDeviceCommandText = 105290154U,
			TextMessagingSms = 3851792428U,
			LanguageInstruction = 3338384120U,
			InboxRuleFromConditionFlyOutText = 2653201341U,
			OutOfOffice = 773833243U,
			FirstNameLabel = 2449687995U,
			InboxRuleBodyContainsConditionFlyOutText = 271571182U,
			AllowedSendersLabelForEndUser = 2979279075U,
			VoicemailWizardStep2Title = 3600235422U,
			ConversationsSlab = 2483845882U,
			AutomaticRepliesSlab = 3601145337U,
			IncomingAuthenticationNtlm = 1137051325U,
			MobilePhoneLabel = 1448959650U,
			DeviceNameLabel = 2211346257U,
			HeaderContainsConditionFormat = 2946311768U,
			TeamMailboxDescription = 944969227U,
			EnterNumberClickNext = 2267981074U,
			MobileDevices = 3063130671U,
			DisplayRecoveryPasswordCommandDescription = 1188277387U,
			AtMostOnlyDisplayTemplate = 3451245278U,
			InboxRuleSentToConditionPreCannedText = 777656741U,
			InboxRuleMarkedWithGroupText = 2303439412U,
			Membership = 3455154594U,
			VoicemailSlab = 31525559U,
			AllInformation = 2801441113U,
			BlockOrAllow = 3723544685U,
			CalendarDiagnosticLogWatermarkText = 1716779644U,
			EditGroups = 2601540438U,
			TurnOnTextMessaging = 3335990695U,
			AliasLabelForEnterprise = 1418080636U,
			CategoryDialogTitle = 3019822456U,
			SetYourWorkingHours = 1806349341U,
			ProfileMailboxUsage = 3830590072U,
			AtLeastAtMostDisplayTemplate = 1985922850U,
			NotificationsForMeetingReminders = 1502911297U,
			IncomingServerLabel = 1650094581U,
			IncomingAuthenticationNone = 3052975740U,
			TrialReminderText = 1925535397U,
			GroupsIBelongToAndGroupsIOwnDescription = 783706065U
		}

		private enum ParamIDs
		{
			SetSubscriptionSucceed,
			ImportContactListPage2ResultNumber,
			JoinDlsSuccess,
			EditRuleCaption,
			VerificationEmailFailedToSend,
			VerificationSuccessText,
			JoinOtherDlsSuccess,
			ReceiveNotificationsUsingFormat,
			ImportContactListPage2Result,
			LargeRecipientList,
			VerificationEmailSucceeded,
			NewSubscriptionSucceed,
			JoinDlSuccess,
			VoicemailAccessNumbersTemplate
		}
	}
}
