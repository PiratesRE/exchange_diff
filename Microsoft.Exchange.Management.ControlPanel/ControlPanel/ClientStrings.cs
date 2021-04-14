using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class ClientStrings
	{
		static ClientStrings()
		{
			ClientStrings.stringIDs.Add(616132976U, "ReconnectProviderCommandText");
			ClientStrings.stringIDs.Add(1708942702U, "FieldsInError");
			ClientStrings.stringIDs.Add(1038968483U, "TlsTitle");
			ClientStrings.stringIDs.Add(2085954050U, "UMKeyMappingTimeout");
			ClientStrings.stringIDs.Add(4245786716U, "RequiredFieldValidatorErrorMessage");
			ClientStrings.stringIDs.Add(385068607U, "OwaMailboxPolicyTasks");
			ClientStrings.stringIDs.Add(375881263U, "CopyIsIEOnly");
			ClientStrings.stringIDs.Add(1064772435U, "MinimumCriteriaFieldsInErrorDeliveryStatus");
			ClientStrings.stringIDs.Add(1884426512U, "EnterHybridUIConfirm");
			ClientStrings.stringIDs.Add(1692971068U, "DisableCommandText");
			ClientStrings.stringIDs.Add(1752619070U, "UMHolidayScheduleHolidayStartDateRequiredError");
			ClientStrings.stringIDs.Add(3063981087U, "EnterHybridUIButtonText");
			ClientStrings.stringIDs.Add(3891394090U, "ConstraintViolationValueOutOfRangeForQuota");
			ClientStrings.stringIDs.Add(1245979421U, "HydratingMessage");
			ClientStrings.stringIDs.Add(3561883164U, "GroupNamingPolicyPreviewDescriptionHeader");
			ClientStrings.stringIDs.Add(866598955U, "Validating");
			ClientStrings.stringIDs.Add(1125975266U, "UriKindRelative");
			ClientStrings.stringIDs.Add(3033402446U, "Close");
			ClientStrings.stringIDs.Add(3496306419U, "MoveDown");
			ClientStrings.stringIDs.Add(155777604U, "PeopleConnectBusy");
			ClientStrings.stringIDs.Add(509325205U, "LegacyRegExEnabledRuleLabel");
			ClientStrings.stringIDs.Add(1609794183U, "HydrationDataLossWarning");
			ClientStrings.stringIDs.Add(1398029738U, "NoTreeItem");
			ClientStrings.stringIDs.Add(1133914413U, "LearnMore");
			ClientStrings.stringIDs.Add(1032072896U, "AddEAPCondtionButtonText");
			ClientStrings.stringIDs.Add(848625314U, "InvalidDateRange");
			ClientStrings.stringIDs.Add(2717328418U, "DefaultRuleEditorCaption");
			ClientStrings.stringIDs.Add(3701726907U, "CtrlKeyGoToSearch");
			ClientStrings.stringIDs.Add(3084590849U, "DisableFVA");
			ClientStrings.stringIDs.Add(3237053118U, "Searching");
			ClientStrings.stringIDs.Add(859380341U, "Update");
			ClientStrings.stringIDs.Add(2781755715U, "CurrentPolicyCaption");
			ClientStrings.stringIDs.Add(4233379362U, "FollowedByColon");
			ClientStrings.stringIDs.Add(1281963896U, "EnabledDisplayText");
			ClientStrings.stringIDs.Add(2288607912U, "OffDisplayText");
			ClientStrings.stringIDs.Add(3866481608U, "OwaMailboxPolicyActiveSyncIntegration");
			ClientStrings.stringIDs.Add(915129961U, "PlayOnPhoneDisconnecting");
			ClientStrings.stringIDs.Add(4221859415U, "LegacyOUError");
			ClientStrings.stringIDs.Add(1158475769U, "WebServiceRequestServerError");
			ClientStrings.stringIDs.Add(1861340610U, "Warning");
			ClientStrings.stringIDs.Add(2089076466U, "BlockedPendingDisplayText");
			ClientStrings.stringIDs.Add(4153102186U, "MessageTypePickerInvalid");
			ClientStrings.stringIDs.Add(1693255708U, "OwaMailboxPolicyTextMessaging");
			ClientStrings.stringIDs.Add(717626182U, "OwaMailboxPolicyContacts");
			ClientStrings.stringIDs.Add(2200441302U, "Expand");
			ClientStrings.stringIDs.Add(2959308597U, "DisableConnectorConfirm");
			ClientStrings.stringIDs.Add(2142856376U, "CmdLogTitleForHybridEnterprise");
			ClientStrings.stringIDs.Add(3292878272U, "ProviderConnectedWithError");
			ClientStrings.stringIDs.Add(3612561219U, "RequestSpamDetail");
			ClientStrings.stringIDs.Add(1414246128U, "None");
			ClientStrings.stringIDs.Add(94147420U, "PassiveText");
			ClientStrings.stringIDs.Add(506561007U, "DisableFederationInProgress");
			ClientStrings.stringIDs.Add(1174301401U, "MobileDeviceDisableText");
			ClientStrings.stringIDs.Add(3442569333U, "MoreOptions");
			ClientStrings.stringIDs.Add(2522455672U, "MidnightAM");
			ClientStrings.stringIDs.Add(4104581892U, "NotificationCount");
			ClientStrings.stringIDs.Add(906828259U, "ContactsSharing");
			ClientStrings.stringIDs.Add(1644825107U, "LongRunInProgressDescription");
			ClientStrings.stringIDs.Add(2545492551U, "MailboxToSearchRequiredErrorMessage");
			ClientStrings.stringIDs.Add(321700842U, "DomainNoValue");
			ClientStrings.stringIDs.Add(3455735334U, "MyOptions");
			ClientStrings.stringIDs.Add(335355257U, "VoicemailConfigurationDetails");
			ClientStrings.stringIDs.Add(1803016445U, "CloseWindowOnLogout");
			ClientStrings.stringIDs.Add(3832063564U, "CustomizeColumns");
			ClientStrings.stringIDs.Add(3018361386U, "EnterProductKey");
			ClientStrings.stringIDs.Add(4158450825U, "PlayOnPhoneDialing");
			ClientStrings.stringIDs.Add(207011947U, "OwaMailboxPolicyUMIntegration");
			ClientStrings.stringIDs.Add(1066902945U, "HydrationFailed");
			ClientStrings.stringIDs.Add(506890308U, "EnableActiveSyncConfirm");
			ClientStrings.stringIDs.Add(3423705850U, "ConstraintViolationStringLengthIsEmpty");
			ClientStrings.stringIDs.Add(2455574594U, "SelectOneLink");
			ClientStrings.stringIDs.Add(2336951795U, "ConstraintNotNullOrEmpty");
			ClientStrings.stringIDs.Add(4193083863U, "LitigationHoldOwnerNotSet");
			ClientStrings.stringIDs.Add(1189851274U, "RequiredFieldIndicator");
			ClientStrings.stringIDs.Add(3508492594U, "FolderTree");
			ClientStrings.stringIDs.Add(3771622829U, "IncidentReportSelectAll");
			ClientStrings.stringIDs.Add(3819852093U, "Notification");
			ClientStrings.stringIDs.Add(2571312913U, "HydrationDoneFeatureFailed");
			ClientStrings.stringIDs.Add(1749577099U, "LongRunWarningLabel");
			ClientStrings.stringIDs.Add(1796910490U, "PublicFoldersEmptyDataTextRoot");
			ClientStrings.stringIDs.Add(1517827063U, "Unsuccessful");
			ClientStrings.stringIDs.Add(3951041989U, "TextMessagingNotificationNotSetupText");
			ClientStrings.stringIDs.Add(879792491U, "VoicemailConfigurationConfirmationMessage");
			ClientStrings.stringIDs.Add(865985204U, "EnableFederationInProgress");
			ClientStrings.stringIDs.Add(3625629356U, "OwaMailboxPolicyAllowCopyContactsToDeviceAddressBook");
			ClientStrings.stringIDs.Add(2357636744U, "OwaMailboxPolicyInformationManagement");
			ClientStrings.stringIDs.Add(3506211787U, "WarningPanelDisMissMsg");
			ClientStrings.stringIDs.Add(1523028278U, "OwaMailboxPolicyJournal");
			ClientStrings.stringIDs.Add(993523547U, "DatesNotDefined");
			ClientStrings.stringIDs.Add(4062764466U, "EnableOWAConfirm");
			ClientStrings.stringIDs.Add(2266587541U, "CancelWipePendingDisplayText");
			ClientStrings.stringIDs.Add(2546927256U, "DeliveryReportSearchFieldsInError");
			ClientStrings.stringIDs.Add(2954372719U, "MyOrganization");
			ClientStrings.stringIDs.Add(3927445923U, "Today");
			ClientStrings.stringIDs.Add(1992800455U, "ExtendedReportsInsufficientData");
			ClientStrings.stringIDs.Add(3963002503U, "EnableConnectorLoggingConfirm");
			ClientStrings.stringIDs.Add(2805968874U, "MessageTraceInvalidEndDate");
			ClientStrings.stringIDs.Add(1788720538U, "AddSubnetCaption");
			ClientStrings.stringIDs.Add(4091981230U, "CustomizeSenderLabel");
			ClientStrings.stringIDs.Add(233002034U, "SharedUMAutoAttendantPilotIdentifierListE164Error");
			ClientStrings.stringIDs.Add(1562506021U, "PreviousMonth");
			ClientStrings.stringIDs.Add(3833953726U, "Stop");
			ClientStrings.stringIDs.Add(3958687323U, "AllAvailableIPV6Address");
			ClientStrings.stringIDs.Add(3462654059U, "LetCallersInterruptGreetingsText");
			ClientStrings.stringIDs.Add(3386445864U, "GroupNamingPolicyEditorPrefixLabel");
			ClientStrings.stringIDs.Add(1102979128U, "Transferred");
			ClientStrings.stringIDs.Add(2787872604U, "NewDomain");
			ClientStrings.stringIDs.Add(2390512187U, "PublicFoldersEmptyDataTextChildren");
			ClientStrings.stringIDs.Add(2682114028U, "FacebookDelayed");
			ClientStrings.stringIDs.Add(1342839575U, "Collapse");
			ClientStrings.stringIDs.Add(4211282305U, "GroupNamingPolicyEditorSuffixLabel");
			ClientStrings.stringIDs.Add(630233035U, "MessageFontSampleText");
			ClientStrings.stringIDs.Add(3186571665U, "HideModalDialogErrorReport");
			ClientStrings.stringIDs.Add(187569137U, "JobSubmissionWaitText");
			ClientStrings.stringIDs.Add(4151098027U, "ServiceNone");
			ClientStrings.stringIDs.Add(2328220407U, "Page");
			ClientStrings.stringIDs.Add(2828598385U, "NavigateAway");
			ClientStrings.stringIDs.Add(1122075U, "RemoveDisabledLinkedInConnectionText");
			ClientStrings.stringIDs.Add(59720971U, "HCWStoppedDescription");
			ClientStrings.stringIDs.Add(2658434708U, "OnDisplayText");
			ClientStrings.stringIDs.Add(1981061025U, "LongRunErrorLabel");
			ClientStrings.stringIDs.Add(809706446U, "EIAE");
			ClientStrings.stringIDs.Add(1907621764U, "NegotiateAuth");
			ClientStrings.stringIDs.Add(3122995196U, "RequestDLPDetailReportTitle");
			ClientStrings.stringIDs.Add(2736422594U, "WipeConfirmMessage");
			ClientStrings.stringIDs.Add(317346726U, "MobileDeviceEnableText");
			ClientStrings.stringIDs.Add(3197491520U, "EnableConnectorConfirm");
			ClientStrings.stringIDs.Add(1875331145U, "OffCommandText");
			ClientStrings.stringIDs.Add(3778906594U, "HydrationDoneTitle");
			ClientStrings.stringIDs.Add(1144247250U, "ConnectorAllAvailableIPv6");
			ClientStrings.stringIDs.Add(1795974124U, "UMCallAnsweringRulesEditorRuleConditionLabelText");
			ClientStrings.stringIDs.Add(3225869645U, "OwaMailboxPolicyPlaces");
			ClientStrings.stringIDs.Add(602293241U, "JournalEmailAddressLabel");
			ClientStrings.stringIDs.Add(259008929U, "PopupBlockedMessage");
			ClientStrings.stringIDs.Add(2802879859U, "IUnderstandAction");
			ClientStrings.stringIDs.Add(2532213629U, "Select15Minutes");
			ClientStrings.stringIDs.Add(1811854250U, "AllowedPendingDisplayText");
			ClientStrings.stringIDs.Add(1848756223U, "OwaMailboxPolicyTimeManagement");
			ClientStrings.stringIDs.Add(3077953159U, "PasswordNote");
			ClientStrings.stringIDs.Add(573217698U, "HasLinkQueryField");
			ClientStrings.stringIDs.Add(1318187683U, "VoicemailClearSettingsTitle");
			ClientStrings.stringIDs.Add(3269293275U, "ConfigureOAuth");
			ClientStrings.stringIDs.Add(3068683287U, "And");
			ClientStrings.stringIDs.Add(4210295185U, "VoicemailResetPINSuccessMessage");
			ClientStrings.stringIDs.Add(3121279917U, "FileDownloadFailed");
			ClientStrings.stringIDs.Add(3545744552U, "ConfirmRemoveLinkedIn");
			ClientStrings.stringIDs.Add(1354285736U, "RemoveFacebookSupportingText");
			ClientStrings.stringIDs.Add(4266824568U, "ListViewMoreResultsWarning");
			ClientStrings.stringIDs.Add(795464922U, "DisableReplicationCommandText");
			ClientStrings.stringIDs.Add(519362517U, "EnterpriseMainHeader");
			ClientStrings.stringIDs.Add(313078617U, "AddGroupNamingPolicyElementButtonText");
			ClientStrings.stringIDs.Add(571113625U, "UseAlias");
			ClientStrings.stringIDs.Add(1300295012U, "FileUploadFailed");
			ClientStrings.stringIDs.Add(1277846131U, "CustomDateLink");
			ClientStrings.stringIDs.Add(4122267321U, "PolicyGroupMembership");
			ClientStrings.stringIDs.Add(1548165396U, "NextPage");
			ClientStrings.stringIDs.Add(437474464U, "HydrationDone");
			ClientStrings.stringIDs.Add(4033288601U, "ProviderDisabled");
			ClientStrings.stringIDs.Add(306768456U, "IndividualSettings");
			ClientStrings.stringIDs.Add(616309426U, "CalendarSharingFreeBusyDetail");
			ClientStrings.stringIDs.Add(2134452995U, "LongRunInProgressTips");
			ClientStrings.stringIDs.Add(2004565666U, "OwaMailboxPolicyChangePassword");
			ClientStrings.stringIDs.Add(1595040325U, "VoicemailClearSettingsDetailsContactOperator");
			ClientStrings.stringIDs.Add(2738758716U, "DisableFederationStopped");
			ClientStrings.stringIDs.Add(2659939651U, "Success");
			ClientStrings.stringIDs.Add(2197391249U, "NoOnboardingPermission");
			ClientStrings.stringIDs.Add(183956792U, "HydratingTitle");
			ClientStrings.stringIDs.Add(2622420678U, "TextMessagingNotificationSetupLinkText");
			ClientStrings.stringIDs.Add(533757016U, "WipePendingPendingDisplayText");
			ClientStrings.stringIDs.Add(1565584202U, "InvalidMultiEmailAddress");
			ClientStrings.stringIDs.Add(4033771799U, "DataCenterMainHeader");
			ClientStrings.stringIDs.Add(893456894U, "BulkEditNotificationTenMinuteLabel");
			ClientStrings.stringIDs.Add(124543120U, "DefaultRuleExceptionLabel");
			ClientStrings.stringIDs.Add(2402178338U, "SelectTheTextAndCopy");
			ClientStrings.stringIDs.Add(2165251921U, "FailedToRetrieveMailboxOnboarding");
			ClientStrings.stringIDs.Add(3546966747U, "DisabledDisplayText");
			ClientStrings.stringIDs.Add(1361553573U, "ConditionValueSeparator");
			ClientStrings.stringIDs.Add(2388288698U, "LinkedInDelayed");
			ClientStrings.stringIDs.Add(933672694U, "ErrorTitle");
			ClientStrings.stringIDs.Add(3042237373U, "InvalidSmtpAddress");
			ClientStrings.stringIDs.Add(128074410U, "RemoveLinkedInSupportingText");
			ClientStrings.stringIDs.Add(2368306281U, "ResumeDBCopyConfirmation");
			ClientStrings.stringIDs.Add(1168324224U, "MessageTypeAll");
			ClientStrings.stringIDs.Add(3966162852U, "CmdLogTitleForHybridO365");
			ClientStrings.stringIDs.Add(207358046U, "ConfirmRemoveFacebook");
			ClientStrings.stringIDs.Add(3329734225U, "BulkEditNotificationMinuteLabel");
			ClientStrings.stringIDs.Add(2523533992U, "VoiceMailText");
			ClientStrings.stringIDs.Add(2616506832U, "CollapseAll");
			ClientStrings.stringIDs.Add(1728961555U, "DefaultContactsFolderText");
			ClientStrings.stringIDs.Add(979822077U, "OwaMailboxPolicyWeather");
			ClientStrings.stringIDs.Add(1129028723U, "LegacyFolderError");
			ClientStrings.stringIDs.Add(2904016598U, "MessageTraceReportTitle");
			ClientStrings.stringIDs.Add(2966716344U, "JournalEmailAddressInvalid");
			ClientStrings.stringIDs.Add(2501247758U, "JobSubmitted");
			ClientStrings.stringIDs.Add(3058620969U, "UMEnableE164ActionSummary");
			ClientStrings.stringIDs.Add(2041362128U, "OK");
			ClientStrings.stringIDs.Add(3303348785U, "LastPage");
			ClientStrings.stringIDs.Add(2715655425U, "OwaMailboxPolicyRemindersAndNotifications");
			ClientStrings.stringIDs.Add(1359139161U, "DataLossWarning");
			ClientStrings.stringIDs.Add(3428137968U, "SuspendComments");
			ClientStrings.stringIDs.Add(1531436154U, "Delivered");
			ClientStrings.stringIDs.Add(479196852U, "Retry");
			ClientStrings.stringIDs.Add(1777112844U, "Descending");
			ClientStrings.stringIDs.Add(872019732U, "SimpleFilterTextBoxWaterMark");
			ClientStrings.stringIDs.Add(2058638459U, "TypingDescription");
			ClientStrings.stringIDs.Add(4012579652U, "NonEditingAuthor");
			ClientStrings.stringIDs.Add(3976037671U, "MinimumCriteriaFieldsInError");
			ClientStrings.stringIDs.Add(1349255527U, "ListSeparator");
			ClientStrings.stringIDs.Add(18372887U, "ExpandAll");
			ClientStrings.stringIDs.Add(968612268U, "AutoInternal");
			ClientStrings.stringIDs.Add(3953482277U, "NeverUse");
			ClientStrings.stringIDs.Add(4257989793U, "NoonPM");
			ClientStrings.stringIDs.Add(2204193123U, "EnableReplicationCommandText");
			ClientStrings.stringIDs.Add(1536001239U, "HCWCompletedDescription");
			ClientStrings.stringIDs.Add(3618788766U, "PWTNS");
			ClientStrings.stringIDs.Add(3667211102U, "DeviceModelPickerAll");
			ClientStrings.stringIDs.Add(1040160067U, "NextMonth");
			ClientStrings.stringIDs.Add(3066268041U, "UploaderUnhandledExceptionMessage");
			ClientStrings.stringIDs.Add(1677432033U, "MobileExternal");
			ClientStrings.stringIDs.Add(184677197U, "SearchButtonTooltip");
			ClientStrings.stringIDs.Add(1715033809U, "SavingCompletedInformation");
			ClientStrings.stringIDs.Add(2274379572U, "SetupExchangeHybrid");
			ClientStrings.stringIDs.Add(1355682252U, "EnableFVA");
			ClientStrings.stringIDs.Add(3247026326U, "PWTNAB");
			ClientStrings.stringIDs.Add(1759109339U, "ForceConnectMailbox");
			ClientStrings.stringIDs.Add(4291377740U, "ShowModalDialogErrorReport");
			ClientStrings.stringIDs.Add(2583693733U, "Imap");
			ClientStrings.stringIDs.Add(3122803756U, "ConnectToFacebookMessage");
			ClientStrings.stringIDs.Add(1638106043U, "DateRangeError");
			ClientStrings.stringIDs.Add(1138816392U, "OwaMailboxPolicyUserExperience");
			ClientStrings.stringIDs.Add(3831099320U, "WebServiceRequestInetError");
			ClientStrings.stringIDs.Add(839901080U, "FindMeText");
			ClientStrings.stringIDs.Add(3186529231U, "CtrlKeySelectAllInListView");
			ClientStrings.stringIDs.Add(3086537020U, "RemoveAction");
			ClientStrings.stringIDs.Add(2835992379U, "UMHolidayScheduleHolidayEndDateRequiredError");
			ClientStrings.stringIDs.Add(3179034952U, "NetworkCredentialUserNameErrorMessage");
			ClientStrings.stringIDs.Add(1579815837U, "SetupHybridUIFirst");
			ClientStrings.stringIDs.Add(1205297021U, "UMKeyMappingActionSummaryAnnounceBusinessLocation");
			ClientStrings.stringIDs.Add(1139168619U, "GroupNamingPolicyCaption");
			ClientStrings.stringIDs.Add(3022174399U, "TransferToGalContactText");
			ClientStrings.stringIDs.Add(4125561529U, "UnhandledExceptionMessage");
			ClientStrings.stringIDs.Add(2614439623U, "OwaMailboxPolicyJunkEmail");
			ClientStrings.stringIDs.Add(4070877873U, "DynamicDistributionGroupText");
			ClientStrings.stringIDs.Add(2522496037U, "ServerNameColumnText");
			ClientStrings.stringIDs.Add(1445885649U, "TextMessagingNotificationNotSetupLinkText");
			ClientStrings.stringIDs.Add(3450836391U, "QuerySyntaxError");
			ClientStrings.stringIDs.Add(1278568394U, "SharingDomainOptionAll");
			ClientStrings.stringIDs.Add(1907037086U, "VoicemailWizardEnterPinText");
			ClientStrings.stringIDs.Add(4068892486U, "AddressExists");
			ClientStrings.stringIDs.Add(1987425903U, "ModifyExchangeHybrid");
			ClientStrings.stringIDs.Add(2944220729U, "HydrationAndFeatureDone");
			ClientStrings.stringIDs.Add(997295902U, "ProviderConnected");
			ClientStrings.stringIDs.Add(3174388608U, "NoNamingPolicySetup");
			ClientStrings.stringIDs.Add(3000098309U, "DoNotShowDialog");
			ClientStrings.stringIDs.Add(238804546U, "RemoveMailboxDeleteLiveID");
			ClientStrings.stringIDs.Add(1527687315U, "GreetingsAndPromptsTitleText");
			ClientStrings.stringIDs.Add(3621606012U, "ConfigureVoicemailButtonText");
			ClientStrings.stringIDs.Add(613554384U, "FirstNameLastName");
			ClientStrings.stringIDs.Add(3021629903U, "Yes");
			ClientStrings.stringIDs.Add(3560108081U, "Author");
			ClientStrings.stringIDs.Add(148099519U, "PWTRAS");
			ClientStrings.stringIDs.Add(2482463458U, "TransferToGalContactVoicemailText");
			ClientStrings.stringIDs.Add(2706637479U, "MailboxDelegationDetail");
			ClientStrings.stringIDs.Add(381169853U, "Or");
			ClientStrings.stringIDs.Add(1594942057U, "Reset");
			ClientStrings.stringIDs.Add(1904041070U, "UpdateTimeZonePrompt");
			ClientStrings.stringIDs.Add(3369491564U, "DontSave");
			ClientStrings.stringIDs.Add(2908249814U, "VoicemailSummaryAccessNumber");
			ClientStrings.stringIDs.Add(2328220357U, "Save");
			ClientStrings.stringIDs.Add(2864395898U, "OwaMailboxPolicyThemeSelection");
			ClientStrings.stringIDs.Add(1393796710U, "ReadThis");
			ClientStrings.stringIDs.Add(2379979535U, "SubnetIPEditorTitle");
			ClientStrings.stringIDs.Add(3782146179U, "UMEnableExtensionAuto");
			ClientStrings.stringIDs.Add(1062541333U, "WarningPanelMultipleWarnings");
			ClientStrings.stringIDs.Add(502251987U, "UMHolidayScheduleHolidayNameRequiredError");
			ClientStrings.stringIDs.Add(3674505245U, "Select24Hours");
			ClientStrings.stringIDs.Add(2794185135U, "MemberUpdateTypeApprovalRequired");
			ClientStrings.stringIDs.Add(639660141U, "DefaultRuleActionLabel");
			ClientStrings.stringIDs.Add(4195962670U, "EmptyValueError");
			ClientStrings.stringIDs.Add(2794530675U, "ApplyToAllCalls");
			ClientStrings.stringIDs.Add(3908900425U, "OutOfMemoryErrorMessage");
			ClientStrings.stringIDs.Add(1594930120U, "Never");
			ClientStrings.stringIDs.Add(2694533749U, "OABExternal");
			ClientStrings.stringIDs.Add(320649385U, "ReconnectToFacebookMessage");
			ClientStrings.stringIDs.Add(1889215887U, "MemberApprovalHasChanged");
			ClientStrings.stringIDs.Add(2257237083U, "CreatingFolder");
			ClientStrings.stringIDs.Add(2743921036U, "UMEnableMailboxAutoSipDescription");
			ClientStrings.stringIDs.Add(367868548U, "FirstNameLastNameInitial");
			ClientStrings.stringIDs.Add(1690161621U, "PleaseWait");
			ClientStrings.stringIDs.Add(2091505548U, "WhatDoesThisMean");
			ClientStrings.stringIDs.Add(3036539697U, "ModalDialogErrorReport");
			ClientStrings.stringIDs.Add(354624350U, "SecondaryNavigation");
			ClientStrings.stringIDs.Add(305410994U, "ConnectToLinkedInMessage");
			ClientStrings.stringIDs.Add(503026860U, "MessageTraceMessageIDCannotContainComma");
			ClientStrings.stringIDs.Add(918737566U, "ApplyToAllMessages");
			ClientStrings.stringIDs.Add(689982738U, "ActiveSyncDisableText");
			ClientStrings.stringIDs.Add(637136194U, "BasicAuth");
			ClientStrings.stringIDs.Add(2870521773U, "VoicemailResetPINTitle");
			ClientStrings.stringIDs.Add(2777189520U, "LongRunCompletedTips");
			ClientStrings.stringIDs.Add(3817888713U, "GroupNamingPolicyPreviewLabel");
			ClientStrings.stringIDs.Add(2053059583U, "AASL");
			ClientStrings.stringIDs.Add(924670953U, "MemberUpdateTypeOpen");
			ClientStrings.stringIDs.Add(2013213994U, "PublishingEditor");
			ClientStrings.stringIDs.Add(1356895513U, "LossDataWarning");
			ClientStrings.stringIDs.Add(2014560916U, "OwaMailboxPolicyInstantMessaging");
			ClientStrings.stringIDs.Add(2093942654U, "TextTooLongErrorMessage");
			ClientStrings.stringIDs.Add(1833904645U, "EnableFederationStopped");
			ClientStrings.stringIDs.Add(912552836U, "GreetingsAndPromptsInstructionsText");
			ClientStrings.stringIDs.Add(771355430U, "UMHolidayScheduleHolidayPromptRequiredError");
			ClientStrings.stringIDs.Add(1515892343U, "AddDagMember");
			ClientStrings.stringIDs.Add(259319032U, "DistributionGroupText");
			ClientStrings.stringIDs.Add(1244644071U, "KeyMappingDisplayTextFormat");
			ClientStrings.stringIDs.Add(3924205880U, "RequestRuleDetailReportTitle");
			ClientStrings.stringIDs.Add(1865766159U, "RequestMalwareDetailReportTitle");
			ClientStrings.stringIDs.Add(173806984U, "ConfirmRemoveConnectionTitle");
			ClientStrings.stringIDs.Add(2320161129U, "WebServicesInternal");
			ClientStrings.stringIDs.Add(49675595U, "Wait");
			ClientStrings.stringIDs.Add(4196350991U, "DisableOWAConfirm");
			ClientStrings.stringIDs.Add(2663567466U, "UriKindRelativeOrAbsolute");
			ClientStrings.stringIDs.Add(2122906481U, "SessionTimeout");
			ClientStrings.stringIDs.Add(1742879518U, "Change");
			ClientStrings.stringIDs.Add(339824766U, "LastNameFirstName");
			ClientStrings.stringIDs.Add(204633756U, "AddIPAddress");
			ClientStrings.stringIDs.Add(1091377885U, "InvalidUnlimitedQuotaRegex");
			ClientStrings.stringIDs.Add(1623670356U, "NoSenderAddressWarning");
			ClientStrings.stringIDs.Add(3333839866U, "TextMessagingNotificationSetupText");
			ClientStrings.stringIDs.Add(2334202318U, "LastNameFirstNameInitial");
			ClientStrings.stringIDs.Add(2124576504U, "UnhandledExceptionTitle");
			ClientStrings.stringIDs.Add(3689797535U, "NoConditionErrorText");
			ClientStrings.stringIDs.Add(3348900521U, "FirstPage");
			ClientStrings.stringIDs.Add(1673297715U, "CannotUploadMultipleFiles");
			ClientStrings.stringIDs.Add(3328778458U, "ClearButtonTooltip");
			ClientStrings.stringIDs.Add(2941133106U, "AutoExternal");
			ClientStrings.stringIDs.Add(768075136U, "DayAndTimeRangeTooltip");
			ClientStrings.stringIDs.Add(3147146074U, "ViewNotificationDetails");
			ClientStrings.stringIDs.Add(2053059451U, "EASL");
			ClientStrings.stringIDs.Add(965845689U, "Outlook");
			ClientStrings.stringIDs.Add(3904375665U, "ConnectProviderCommandText");
			ClientStrings.stringIDs.Add(2391603832U, "SpecificPhoneNumberText");
			ClientStrings.stringIDs.Add(334057287U, "Pending");
			ClientStrings.stringIDs.Add(3216786356U, "CalendarSharingFreeBusyReviewer");
			ClientStrings.stringIDs.Add(3097242366U, "LitigationHoldDateNotSet");
			ClientStrings.stringIDs.Add(2796848775U, "OwaMailboxPolicyAllAddressLists");
			ClientStrings.stringIDs.Add(22442200U, "Error");
			ClientStrings.stringIDs.Add(1466672057U, "Externaldnslookups");
			ClientStrings.stringIDs.Add(3849511688U, "EditVoicemailButtonText");
			ClientStrings.stringIDs.Add(903130308U, "FailedToRetrieveMailboxLocalMove");
			ClientStrings.stringIDs.Add(1144247248U, "ConnectorAllAvailableIPv4");
			ClientStrings.stringIDs.Add(2262897061U, "HydrationAndFeatureDoneTitle");
			ClientStrings.stringIDs.Add(2058499689U, "ConstraintViolationNoLeadingOrTrailingWhitespace");
			ClientStrings.stringIDs.Add(2862582797U, "CalendarSharingFreeBusySimple");
			ClientStrings.stringIDs.Add(4277755996U, "PageSize");
			ClientStrings.stringIDs.Add(2742068696U, "ConstraintFieldsNotMatchError");
			ClientStrings.stringIDs.Add(3900615856U, "Updating");
			ClientStrings.stringIDs.Add(353630484U, "AdditionalPropertiesLabel");
			ClientStrings.stringIDs.Add(1011589268U, "JumpToMigrationSlabConfirmation");
			ClientStrings.stringIDs.Add(7739616U, "VoicemailCallFwdContactOperator");
			ClientStrings.stringIDs.Add(2680934369U, "InvalidValueRange");
			ClientStrings.stringIDs.Add(2697991219U, "NoActionRuleAuditSeverity");
			ClientStrings.stringIDs.Add(3273613659U, "VoicemailClearSettingsConfirmationMessage");
			ClientStrings.stringIDs.Add(1655108951U, "WebServicesExternal");
			ClientStrings.stringIDs.Add(2101584371U, "UploaderValidationError");
			ClientStrings.stringIDs.Add(3336188783U, "ServerAboutToExpireWarningText");
			ClientStrings.stringIDs.Add(3200788962U, "ConditionValueRequriedErrorMessage");
			ClientStrings.stringIDs.Add(439636863U, "PreviousePage");
			ClientStrings.stringIDs.Add(3390434404U, "Ascending");
			ClientStrings.stringIDs.Add(2068973737U, "EditSubnetCaption");
			ClientStrings.stringIDs.Add(267746805U, "ChooseAtLeastOneColumn");
			ClientStrings.stringIDs.Add(2900910704U, "EditSharingEnabledDomains");
			ClientStrings.stringIDs.Add(986397318U, "Recipients");
			ClientStrings.stringIDs.Add(2778558511U, "Back");
			ClientStrings.stringIDs.Add(3068528108U, "VoicemailPostFwdRecordGreeting");
			ClientStrings.stringIDs.Add(1857390484U, "ColumnChooseFailed");
			ClientStrings.stringIDs.Add(1219538243U, "TransportRuleBusinessContinuity");
			ClientStrings.stringIDs.Add(936826416U, "TitleSectionMobileDevices");
			ClientStrings.stringIDs.Add(2894502448U, "NewFolder");
			ClientStrings.stringIDs.Add(3265439859U, "EnableCommandText");
			ClientStrings.stringIDs.Add(583273118U, "PrimaryAddressLabel");
			ClientStrings.stringIDs.Add(3843987112U, "IncidentReportContentCustom");
			ClientStrings.stringIDs.Add(3566331407U, "MessageTraceInvalidStartDate");
			ClientStrings.stringIDs.Add(3393270247U, "InvalidDecimal1");
			ClientStrings.stringIDs.Add(2636721601U, "ActivateDBCopyConfirmation");
			ClientStrings.stringIDs.Add(1063565289U, "DisableActiveSyncConfirm");
			ClientStrings.stringIDs.Add(652753372U, "PleaseWaitWhileSaving");
			ClientStrings.stringIDs.Add(4082347473U, "AddConditionButtonText");
			ClientStrings.stringIDs.Add(3022976242U, "AcceptedDomainAuthoritativeWarning");
			ClientStrings.stringIDs.Add(3653626825U, "Editor");
			ClientStrings.stringIDs.Add(1801819654U, "PlayOnPhoneConnected");
			ClientStrings.stringIDs.Add(2762661174U, "InvalidDomainName");
			ClientStrings.stringIDs.Add(2068086983U, "SetECPAuthConfirmText");
			ClientStrings.stringIDs.Add(4262899755U, "RuleNameTextBoxLabel");
			ClientStrings.stringIDs.Add(1414245989U, "More");
			ClientStrings.stringIDs.Add(2549262094U, "FirstNameInitialLastName");
			ClientStrings.stringIDs.Add(2810364182U, "OwaMailboxPolicyCommunicationManagement");
			ClientStrings.stringIDs.Add(2731136937U, "Owner");
			ClientStrings.stringIDs.Add(1400729458U, "EditDomain");
			ClientStrings.stringIDs.Add(3134721922U, "InvalidEmailAddressName");
			ClientStrings.stringIDs.Add(467409560U, "LeadingTrailingSpaceError");
			ClientStrings.stringIDs.Add(2307360811U, "DayRangeAndTimeRangeTooltip");
			ClientStrings.stringIDs.Add(3180237931U, "OwaMailboxPolicyPremiumClient");
			ClientStrings.stringIDs.Add(1849292272U, "IncidentReportDeselectAll");
			ClientStrings.stringIDs.Add(1911032188U, "OwaMailboxPolicyNotes");
			ClientStrings.stringIDs.Add(3151174489U, "UMEnableSipActionSummary");
			ClientStrings.stringIDs.Add(1045743613U, "Reviewer");
			ClientStrings.stringIDs.Add(1761836208U, "LongRunStoppedTips");
			ClientStrings.stringIDs.Add(1259584465U, "RequestSpamDetailReportTitle");
			ClientStrings.stringIDs.Add(288995097U, "AddExceptionButtonText");
			ClientStrings.stringIDs.Add(1283935370U, "OwaMailboxPolicyRecoverDeletedItems");
			ClientStrings.stringIDs.Add(117844352U, "AddActionButtonText");
			ClientStrings.stringIDs.Add(2009538927U, "EndDateMustBeYesterday");
			ClientStrings.stringIDs.Add(3100497742U, "OwaInternal");
			ClientStrings.stringIDs.Add(2920201570U, "CheckNames");
			ClientStrings.stringIDs.Add(2752727145U, "UMHolidayScheduleHolidayStartEndDateValidationError");
			ClientStrings.stringIDs.Add(3104459904U, "InvalidMailboxSearchName");
			ClientStrings.stringIDs.Add(2559957656U, "RemoveMailboxKeepLiveID");
			ClientStrings.stringIDs.Add(3766407320U, "OwaMailboxPolicySignatures");
			ClientStrings.stringIDs.Add(3982517327U, "WorkingHoursText");
			ClientStrings.stringIDs.Add(3189265994U, "ESHL");
			ClientStrings.stringIDs.Add(1911656207U, "FileStillUploading");
			ClientStrings.stringIDs.Add(472573372U, "StartOverMailboxSearch");
			ClientStrings.stringIDs.Add(9358886U, "DisableConnectorLoggingConfirm");
			ClientStrings.stringIDs.Add(228304082U, "LongRunDataLossWarning");
			ClientStrings.stringIDs.Add(798401137U, "ActiveText");
			ClientStrings.stringIDs.Add(461710906U, "DisableMOWAConfirm");
			ClientStrings.stringIDs.Add(1400887235U, "WarningPanelSeeMoreMsg");
			ClientStrings.stringIDs.Add(2860566285U, "EnterpriseLogoutFail");
			ClientStrings.stringIDs.Add(3375594338U, "AlwaysUse");
			ClientStrings.stringIDs.Add(1271124552U, "PublishingAuthor");
			ClientStrings.stringIDs.Add(3934389177U, "CopyError");
			ClientStrings.stringIDs.Add(3980910748U, "LastNameInitialFirstName");
			ClientStrings.stringIDs.Add(2757672907U, "Internaldnslookups");
			ClientStrings.stringIDs.Add(2794407267U, "ApplyAllMessagesWarning");
			ClientStrings.stringIDs.Add(3799642099U, "EditIPAddress");
			ClientStrings.stringIDs.Add(265625760U, "SecurityGroupText");
			ClientStrings.stringIDs.Add(356602967U, "ResumeMailboxSearch");
			ClientStrings.stringIDs.Add(3688802170U, "JumpToOffice365MigrationSlabFailed");
			ClientStrings.stringIDs.Add(3738684211U, "DeliveryReportEmailBodyForMailTo");
			ClientStrings.stringIDs.Add(2929991304U, "DefaultRuleConditionLabel");
			ClientStrings.stringIDs.Add(1506239268U, "RequestDLPDetail");
			ClientStrings.stringIDs.Add(667665198U, "AuditSeverityLevelLabel");
			ClientStrings.stringIDs.Add(3800601796U, "NoneForEmpty");
			ClientStrings.stringIDs.Add(210154378U, "OwaMailboxPolicyRules");
			ClientStrings.stringIDs.Add(4035228826U, "CtrlKeyToSave");
			ClientStrings.stringIDs.Add(3041967243U, "CustomPeriodText");
			ClientStrings.stringIDs.Add(3189266341U, "NSHL");
			ClientStrings.stringIDs.Add(663637318U, "TransferToNumberText");
			ClientStrings.stringIDs.Add(2950784627U, "UMKeyMappingActionSummaryAnnounceBusinessHours");
			ClientStrings.stringIDs.Add(912527261U, "OwaMailboxPolicyFacebook");
			ClientStrings.stringIDs.Add(1929634872U, "ConnectMailboxLaunchWizard");
			ClientStrings.stringIDs.Add(280551769U, "SomeSelectionNotAdded");
			ClientStrings.stringIDs.Add(32190104U, "LoadingInformation");
			ClientStrings.stringIDs.Add(1710116876U, "ProceedWithoutTenantInfo");
			ClientStrings.stringIDs.Add(3599592070U, "Loading");
			ClientStrings.stringIDs.Add(1891837447U, "DeviceTypePickerAll");
			ClientStrings.stringIDs.Add(892936075U, "VoicemailConfigurationTitle");
			ClientStrings.stringIDs.Add(2336878616U, "RemoveDBCopyConfirmation");
			ClientStrings.stringIDs.Add(1308147703U, "MemberUpdateTypeClosed");
			ClientStrings.stringIDs.Add(2359945479U, "OnCommandText");
			ClientStrings.stringIDs.Add(779120846U, "SelectOne");
			ClientStrings.stringIDs.Add(49706295U, "NtlmAuth");
			ClientStrings.stringIDs.Add(1716565179U, "EnableMOWAConfirm");
			ClientStrings.stringIDs.Add(1502600087U, "Pop");
			ClientStrings.stringIDs.Add(882294734U, "DisabledPendingDisplayText");
			ClientStrings.stringIDs.Add(2145940917U, "FirstFocusTextForScreenReader");
			ClientStrings.stringIDs.Add(137938150U, "MoveUp");
			ClientStrings.stringIDs.Add(347463522U, "GalOrPersonalContactText");
			ClientStrings.stringIDs.Add(2903019109U, "AllAvailableIPV4Address");
			ClientStrings.stringIDs.Add(3765422820U, "OwaExternal");
			ClientStrings.stringIDs.Add(1015147527U, "ActiveSyncEnableText");
			ClientStrings.stringIDs.Add(2594574748U, "ConstraintViolationInputUnlimitedValue");
			ClientStrings.stringIDs.Add(1603757297U, "ResetPinMessageTitle");
			ClientStrings.stringIDs.Add(1496915101U, "No");
			ClientStrings.stringIDs.Add(2846873532U, "LongRunStoppedDescription");
			ClientStrings.stringIDs.Add(809706807U, "NIAE");
			ClientStrings.stringIDs.Add(116135148U, "CtrlKeyCloseForm");
			ClientStrings.stringIDs.Add(3705047298U, "InvalidUrl");
			ClientStrings.stringIDs.Add(3403459873U, "InvalidDomain");
			ClientStrings.stringIDs.Add(2852593944U, "Display");
			ClientStrings.stringIDs.Add(2814896254U, "SslTitle");
			ClientStrings.stringIDs.Add(4028691857U, "ProviderDelayed");
			ClientStrings.stringIDs.Add(3512966386U, "TrialExpiredWarningText");
			ClientStrings.stringIDs.Add(1496927839U, "StopProcessingRuleLabel");
			ClientStrings.stringIDs.Add(1872640986U, "UMEnabledPolicyRequired");
			ClientStrings.stringIDs.Add(4231482709U, "All");
			ClientStrings.stringIDs.Add(238716676U, "ClickHereForHelp");
			ClientStrings.stringIDs.Add(945688236U, "LongRunCompletedDescription");
			ClientStrings.stringIDs.Add(4193953058U, "ConnectToO365");
			ClientStrings.stringIDs.Add(2021920769U, "NoRetentionPolicy");
			ClientStrings.stringIDs.Add(995902246U, "SavingInformation");
			ClientStrings.stringIDs.Add(2939777527U, "InvalidAlias");
			ClientStrings.stringIDs.Add(1226897823U, "Contributor");
			ClientStrings.stringIDs.Add(2358390244U, "Cancel");
			ClientStrings.stringIDs.Add(3309738928U, "NoIPWarning");
			ClientStrings.stringIDs.Add(427688167U, "Custom");
			ClientStrings.stringIDs.Add(664253366U, "RemovePendingDisplayText");
			ClientStrings.stringIDs.Add(892169149U, "EnabledPendingDisplayText");
			ClientStrings.stringIDs.Add(3027332405U, "RecordGreetingLinkText");
			ClientStrings.stringIDs.Add(3423762231U, "Next");
			ClientStrings.stringIDs.Add(4164526598U, "PrimaryNavigation");
			ClientStrings.stringIDs.Add(2835676179U, "KeyMappingVoiceMailDisplayText");
			ClientStrings.stringIDs.Add(2381414750U, "WarningPanelMultipleWarningsMsg");
			ClientStrings.stringIDs.Add(67348609U, "RequestMalwareDetail");
			ClientStrings.stringIDs.Add(2193536568U, "Information");
			ClientStrings.stringIDs.Add(1749370292U, "RequestRuleDetail");
			ClientStrings.stringIDs.Add(2470451672U, "OutsideWorkingHoursText");
			ClientStrings.stringIDs.Add(1325539240U, "UMDialPlanRequiredField");
			ClientStrings.stringIDs.Add(3205750596U, "DisableFederationCompleted");
			ClientStrings.stringIDs.Add(3276462966U, "SharingRuleEntryDomainConflict");
			ClientStrings.stringIDs.Add(4005194968U, "EditSharingEnabledDomainsStep2");
			ClientStrings.stringIDs.Add(2220221264U, "NoSpaceValidatorMessage");
			ClientStrings.stringIDs.Add(3125926615U, "WaitForHybridUIReady");
			ClientStrings.stringIDs.Add(684930172U, "MailboxDelegation");
			ClientStrings.stringIDs.Add(1396631711U, "OwaMailboxPolicySecurity");
			ClientStrings.stringIDs.Add(3453349145U, "SenderRequiredErrorMessage");
			ClientStrings.stringIDs.Add(1294686955U, "ServerTrailDaysColumnText");
			ClientStrings.stringIDs.Add(1173301205U, "ResetPinMessage");
			ClientStrings.stringIDs.Add(1482742945U, "RemoveDisabledFacebookConnectionText");
			ClientStrings.stringIDs.Add(214141546U, "ReportTitleMissing");
			ClientStrings.stringIDs.Add(2593970847U, "OwaMailboxPolicyLinkedIn");
			ClientStrings.stringIDs.Add(1904327111U, "ConstraintViolationValueOutOfRange");
			ClientStrings.stringIDs.Add(3379654077U, "MTRTDetailsTitle");
			ClientStrings.stringIDs.Add(2658661820U, "Column");
			ClientStrings.stringIDs.Add(4021298411U, "ClickToShowText");
			ClientStrings.stringIDs.Add(2116880466U, "HybridConfiguration");
			ClientStrings.stringIDs.Add(2408935185U, "PWTAS");
			ClientStrings.stringIDs.Add(3243184343U, "ClickToCopyToClipboard");
			ClientStrings.stringIDs.Add(3296253953U, "UriKindAbsolute");
			ClientStrings.stringIDs.Add(1147585405U, "VoicemailResetPINConfirmationMessage");
			ClientStrings.stringIDs.Add(1513997499U, "ConnectMailboxWizardCaption");
			ClientStrings.stringIDs.Add(2506867635U, "MobileInternal");
			ClientStrings.stringIDs.Add(2184256814U, "RetrievingStatistics");
			ClientStrings.stringIDs.Add(3983756775U, "WebServiceRequestTimeout");
			ClientStrings.stringIDs.Add(3868849319U, "UMEnableTelexActionSummary");
			ClientStrings.stringIDs.Add(3698078783U, "PermissionLevelWarning");
			ClientStrings.stringIDs.Add(3774473073U, "GetAllResults");
			ClientStrings.stringIDs.Add(259417423U, "LongRunCopyToClipboardLabel");
			ClientStrings.stringIDs.Add(721624957U, "EnableFederationCompleted");
			ClientStrings.stringIDs.Add(1957457715U, "OABInternal");
			ClientStrings.stringIDs.Add(2767693563U, "OwaMailboxPolicyCalendar");
		}

		public static string ReconnectProviderCommandText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ReconnectProviderCommandText");
			}
		}

		public static string FieldsInError
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("FieldsInError");
			}
		}

		public static string TlsTitle
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("TlsTitle");
			}
		}

		public static string ModalDialgMultipleSRMsgTemplate(int n, string msg)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("ModalDialgMultipleSRMsgTemplate"), n, msg);
		}

		public static string UMKeyMappingTimeout
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("UMKeyMappingTimeout");
			}
		}

		public static string RequiredFieldValidatorErrorMessage
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("RequiredFieldValidatorErrorMessage");
			}
		}

		public static string OwaMailboxPolicyTasks
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicyTasks");
			}
		}

		public static string CopyIsIEOnly
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("CopyIsIEOnly");
			}
		}

		public static string MinimumCriteriaFieldsInErrorDeliveryStatus
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MinimumCriteriaFieldsInErrorDeliveryStatus");
			}
		}

		public static string EnterHybridUIConfirm
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EnterHybridUIConfirm");
			}
		}

		public static string DisableCommandText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DisableCommandText");
			}
		}

		public static string UMHolidayScheduleHolidayStartDateRequiredError
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("UMHolidayScheduleHolidayStartDateRequiredError");
			}
		}

		public static string EnterHybridUIButtonText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EnterHybridUIButtonText");
			}
		}

		public static string ErrorMessageInvalidInteger(string str)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("ErrorMessageInvalidInteger"), str);
		}

		public static string ConstraintViolationValueOutOfRangeForQuota
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ConstraintViolationValueOutOfRangeForQuota");
			}
		}

		public static string HydratingMessage
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("HydratingMessage");
			}
		}

		public static string GroupNamingPolicyPreviewDescriptionHeader
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("GroupNamingPolicyPreviewDescriptionHeader");
			}
		}

		public static string Validating
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Validating");
			}
		}

		public static string UriKindRelative
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("UriKindRelative");
			}
		}

		public static string Close
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Close");
			}
		}

		public static string MoveDown
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MoveDown");
			}
		}

		public static string PeopleConnectBusy
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("PeopleConnectBusy");
			}
		}

		public static string LegacyRegExEnabledRuleLabel
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("LegacyRegExEnabledRuleLabel");
			}
		}

		public static string HydrationDataLossWarning
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("HydrationDataLossWarning");
			}
		}

		public static string NoTreeItem
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("NoTreeItem");
			}
		}

		public static string LearnMore
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("LearnMore");
			}
		}

		public static string AddEAPCondtionButtonText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("AddEAPCondtionButtonText");
			}
		}

		public static string InvalidDateRange
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("InvalidDateRange");
			}
		}

		public static string DefaultRuleEditorCaption
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DefaultRuleEditorCaption");
			}
		}

		public static string CtrlKeyGoToSearch
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("CtrlKeyGoToSearch");
			}
		}

		public static string DisableFVA
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DisableFVA");
			}
		}

		public static string Searching
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Searching");
			}
		}

		public static string ModalDialogSRHelpTemplate(string help)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("ModalDialogSRHelpTemplate"), help);
		}

		public static string Update
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Update");
			}
		}

		public static string CurrentPolicyCaption
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("CurrentPolicyCaption");
			}
		}

		public static string RemoveCertificateConfirm(string name, string server)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("RemoveCertificateConfirm"), name, server);
		}

		public static string FollowedByColon
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("FollowedByColon");
			}
		}

		public static string EnabledDisplayText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EnabledDisplayText");
			}
		}

		public static string OffDisplayText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OffDisplayText");
			}
		}

		public static string OwaMailboxPolicyActiveSyncIntegration
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicyActiveSyncIntegration");
			}
		}

		public static string PlayOnPhoneDisconnecting
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("PlayOnPhoneDisconnecting");
			}
		}

		public static string QueryValueStartsWithStar(string queryName)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("QueryValueStartsWithStar"), queryName);
		}

		public static string LegacyOUError
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("LegacyOUError");
			}
		}

		public static string WebServiceRequestServerError
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("WebServiceRequestServerError");
			}
		}

		public static string Warning
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Warning");
			}
		}

		public static string BlockedPendingDisplayText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("BlockedPendingDisplayText");
			}
		}

		public static string MessageTypePickerInvalid
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MessageTypePickerInvalid");
			}
		}

		public static string OwaMailboxPolicyTextMessaging
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicyTextMessaging");
			}
		}

		public static string OwaMailboxPolicyContacts
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicyContacts");
			}
		}

		public static string Expand
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Expand");
			}
		}

		public static string InvalidInteger(string str)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("InvalidInteger"), str);
		}

		public static string DisableConnectorConfirm
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DisableConnectorConfirm");
			}
		}

		public static string CmdLogTitleForHybridEnterprise
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("CmdLogTitleForHybridEnterprise");
			}
		}

		public static string ProviderConnectedWithError
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ProviderConnectedWithError");
			}
		}

		public static string RequestSpamDetail
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("RequestSpamDetail");
			}
		}

		public static string None
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("None");
			}
		}

		public static string PassiveText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("PassiveText");
			}
		}

		public static string DisableFederationInProgress
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DisableFederationInProgress");
			}
		}

		public static string MobileDeviceDisableText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MobileDeviceDisableText");
			}
		}

		public static string MoreOptions
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MoreOptions");
			}
		}

		public static string MidnightAM
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MidnightAM");
			}
		}

		public static string NotificationCount
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("NotificationCount");
			}
		}

		public static string ContactsSharing
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ContactsSharing");
			}
		}

		public static string LongRunInProgressDescription
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("LongRunInProgressDescription");
			}
		}

		public static string MailboxToSearchRequiredErrorMessage
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MailboxToSearchRequiredErrorMessage");
			}
		}

		public static string DomainNoValue
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DomainNoValue");
			}
		}

		public static string InvalidCharacter(string str)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("InvalidCharacter"), str);
		}

		public static string MyOptions
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MyOptions");
			}
		}

		public static string VoicemailConfigurationDetails
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("VoicemailConfigurationDetails");
			}
		}

		public static string RenewCertificateHelp(string name)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("RenewCertificateHelp"), name);
		}

		public static string CloseWindowOnLogout
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("CloseWindowOnLogout");
			}
		}

		public static string CustomizeColumns
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("CustomizeColumns");
			}
		}

		public static string EnterProductKey
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EnterProductKey");
			}
		}

		public static string PlayOnPhoneDialing
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("PlayOnPhoneDialing");
			}
		}

		public static string OwaMailboxPolicyUMIntegration
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicyUMIntegration");
			}
		}

		public static string HydrationFailed
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("HydrationFailed");
			}
		}

		public static string EnableActiveSyncConfirm
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EnableActiveSyncConfirm");
			}
		}

		public static string ConstraintViolationStringLengthIsEmpty
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ConstraintViolationStringLengthIsEmpty");
			}
		}

		public static string SelectOneLink
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("SelectOneLink");
			}
		}

		public static string CancelForAjaxUploader(string str)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("CancelForAjaxUploader"), str);
		}

		public static string ConstraintNotNullOrEmpty
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ConstraintNotNullOrEmpty");
			}
		}

		public static string UMKeyMappingActionSummaryTransferToAA(string name)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("UMKeyMappingActionSummaryTransferToAA"), name);
		}

		public static string LitigationHoldOwnerNotSet
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("LitigationHoldOwnerNotSet");
			}
		}

		public static string RequiredFieldIndicator
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("RequiredFieldIndicator");
			}
		}

		public static string FolderTree
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("FolderTree");
			}
		}

		public static string IncidentReportSelectAll
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("IncidentReportSelectAll");
			}
		}

		public static string UMKeyMappingActionSummaryTransferToExtension(string str)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("UMKeyMappingActionSummaryTransferToExtension"), str);
		}

		public static string AutoProvisionFailedMsg(string msg)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("AutoProvisionFailedMsg"), msg);
		}

		public static string Notification
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Notification");
			}
		}

		public static string HydrationDoneFeatureFailed
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("HydrationDoneFeatureFailed");
			}
		}

		public static string NameFromFirstInitialsLastName(string firstName, string initials, string lastName)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("NameFromFirstInitialsLastName"), firstName, initials, lastName);
		}

		public static string LongRunWarningLabel
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("LongRunWarningLabel");
			}
		}

		public static string WipeConfirmTitle(string model)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("WipeConfirmTitle"), model);
		}

		public static string PublicFoldersEmptyDataTextRoot
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("PublicFoldersEmptyDataTextRoot");
			}
		}

		public static string Unsuccessful
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Unsuccessful");
			}
		}

		public static string TextMessagingNotificationNotSetupText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("TextMessagingNotificationNotSetupText");
			}
		}

		public static string VoicemailConfigurationConfirmationMessage
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("VoicemailConfigurationConfirmationMessage");
			}
		}

		public static string EnableFederationInProgress
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EnableFederationInProgress");
			}
		}

		public static string OwaMailboxPolicyAllowCopyContactsToDeviceAddressBook
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicyAllowCopyContactsToDeviceAddressBook");
			}
		}

		public static string OwaMailboxPolicyInformationManagement
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicyInformationManagement");
			}
		}

		public static string WarningPanelDisMissMsg
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("WarningPanelDisMissMsg");
			}
		}

		public static string OwaMailboxPolicyJournal
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicyJournal");
			}
		}

		public static string DatesNotDefined
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DatesNotDefined");
			}
		}

		public static string EnableOWAConfirm
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EnableOWAConfirm");
			}
		}

		public static string CancelWipePendingDisplayText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("CancelWipePendingDisplayText");
			}
		}

		public static string DeliveryReportSearchFieldsInError
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DeliveryReportSearchFieldsInError");
			}
		}

		public static string MyOrganization
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MyOrganization");
			}
		}

		public static string Today
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Today");
			}
		}

		public static string ExtendedReportsInsufficientData
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ExtendedReportsInsufficientData");
			}
		}

		public static string EnableConnectorLoggingConfirm
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EnableConnectorLoggingConfirm");
			}
		}

		public static string MessageTraceInvalidEndDate
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MessageTraceInvalidEndDate");
			}
		}

		public static string BulkEditProgressNoSuccessFailedCount(int m)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("BulkEditProgressNoSuccessFailedCount"), m);
		}

		public static string AddSubnetCaption
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("AddSubnetCaption");
			}
		}

		public static string CustomizeSenderLabel
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("CustomizeSenderLabel");
			}
		}

		public static string SharedUMAutoAttendantPilotIdentifierListE164Error
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("SharedUMAutoAttendantPilotIdentifierListE164Error");
			}
		}

		public static string PreviousMonth
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("PreviousMonth");
			}
		}

		public static string InvalidNumberValue(long minValue, long maxValue)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("InvalidNumberValue"), minValue, maxValue);
		}

		public static string Stop
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Stop");
			}
		}

		public static string AllAvailableIPV6Address
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("AllAvailableIPV6Address");
			}
		}

		public static string LetCallersInterruptGreetingsText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("LetCallersInterruptGreetingsText");
			}
		}

		public static string ClearForAjaxUploader(string str1, string str2)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("ClearForAjaxUploader"), str1, str2);
		}

		public static string GroupNamingPolicyEditorPrefixLabel
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("GroupNamingPolicyEditorPrefixLabel");
			}
		}

		public static string DeliveryReportEmailBody(string url)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("DeliveryReportEmailBody"), url);
		}

		public static string Transferred
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Transferred");
			}
		}

		public static string NewDomain
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("NewDomain");
			}
		}

		public static string PublicFoldersEmptyDataTextChildren
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("PublicFoldersEmptyDataTextChildren");
			}
		}

		public static string WrongExtension(string exts)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("WrongExtension"), exts);
		}

		public static string FacebookDelayed
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("FacebookDelayed");
			}
		}

		public static string Collapse
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Collapse");
			}
		}

		public static string GroupNamingPolicyEditorSuffixLabel
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("GroupNamingPolicyEditorSuffixLabel");
			}
		}

		public static string MessageFontSampleText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MessageFontSampleText");
			}
		}

		public static string HideModalDialogErrorReport
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("HideModalDialogErrorReport");
			}
		}

		public static string JobSubmissionWaitText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("JobSubmissionWaitText");
			}
		}

		public static string ConstraintViolationLocalLongFullPath(string str)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("ConstraintViolationLocalLongFullPath"), str);
		}

		public static string ConstraintViolationUNCFilePath(string str)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("ConstraintViolationUNCFilePath"), str);
		}

		public static string ServiceNone
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ServiceNone");
			}
		}

		public static string Page
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Page");
			}
		}

		public static string NavigateAway
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("NavigateAway");
			}
		}

		public static string RemoveDisabledLinkedInConnectionText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("RemoveDisabledLinkedInConnectionText");
			}
		}

		public static string HCWStoppedDescription
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("HCWStoppedDescription");
			}
		}

		public static string OnDisplayText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OnDisplayText");
			}
		}

		public static string LongRunErrorLabel
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("LongRunErrorLabel");
			}
		}

		public static string EmptyQueryValue(string queryName)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("EmptyQueryValue"), queryName);
		}

		public static string EIAE
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EIAE");
			}
		}

		public static string NegotiateAuth
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("NegotiateAuth");
			}
		}

		public static string RequestDLPDetailReportTitle
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("RequestDLPDetailReportTitle");
			}
		}

		public static string WipeConfirmMessage
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("WipeConfirmMessage");
			}
		}

		public static string MobileDeviceEnableText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MobileDeviceEnableText");
			}
		}

		public static string EnableConnectorConfirm
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EnableConnectorConfirm");
			}
		}

		public static string OffCommandText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OffCommandText");
			}
		}

		public static string HydrationDoneTitle
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("HydrationDoneTitle");
			}
		}

		public static string ConnectorAllAvailableIPv6
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ConnectorAllAvailableIPv6");
			}
		}

		public static string UMCallAnsweringRulesEditorRuleConditionLabelText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("UMCallAnsweringRulesEditorRuleConditionLabelText");
			}
		}

		public static string OwaMailboxPolicyPlaces
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicyPlaces");
			}
		}

		public static string JournalEmailAddressLabel
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("JournalEmailAddressLabel");
			}
		}

		public static string PopupBlockedMessage
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("PopupBlockedMessage");
			}
		}

		public static string IUnderstandAction
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("IUnderstandAction");
			}
		}

		public static string Select15Minutes
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Select15Minutes");
			}
		}

		public static string AllowedPendingDisplayText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("AllowedPendingDisplayText");
			}
		}

		public static string OwaMailboxPolicyTimeManagement
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicyTimeManagement");
			}
		}

		public static string PasswordNote
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("PasswordNote");
			}
		}

		public static string HasLinkQueryField
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("HasLinkQueryField");
			}
		}

		public static string VoicemailClearSettingsTitle
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("VoicemailClearSettingsTitle");
			}
		}

		public static string ConfigureOAuth
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ConfigureOAuth");
			}
		}

		public static string ApplyToAllSelected(int n)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("ApplyToAllSelected"), n);
		}

		public static string ValidationErrorFormat(string msg)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("ValidationErrorFormat"), msg);
		}

		public static string And
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("And");
			}
		}

		public static string VoicemailResetPINSuccessMessage
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("VoicemailResetPINSuccessMessage");
			}
		}

		public static string FileDownloadFailed
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("FileDownloadFailed");
			}
		}

		public static string ConfirmRemoveLinkedIn
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ConfirmRemoveLinkedIn");
			}
		}

		public static string SyncedMailboxText(int cur, int n)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("SyncedMailboxText"), cur, n);
		}

		public static string RemoveFacebookSupportingText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("RemoveFacebookSupportingText");
			}
		}

		public static string ListViewMoreResultsWarning
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ListViewMoreResultsWarning");
			}
		}

		public static string DisableReplicationCommandText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DisableReplicationCommandText");
			}
		}

		public static string EnterpriseMainHeader
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EnterpriseMainHeader");
			}
		}

		public static string AddGroupNamingPolicyElementButtonText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("AddGroupNamingPolicyElementButtonText");
			}
		}

		public static string UseAlias
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("UseAlias");
			}
		}

		public static string FileUploadFailed
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("FileUploadFailed");
			}
		}

		public static string NameFromFirstLastName(string firstName, string lastName)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("NameFromFirstLastName"), firstName, lastName);
		}

		public static string CustomDateLink
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("CustomDateLink");
			}
		}

		public static string PolicyGroupMembership
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("PolicyGroupMembership");
			}
		}

		public static string InvalidEmailAddress(string str)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("InvalidEmailAddress"), str);
		}

		public static string NextPage
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("NextPage");
			}
		}

		public static string HydrationDone
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("HydrationDone");
			}
		}

		public static string ProviderDisabled
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ProviderDisabled");
			}
		}

		public static string StateOrProvinceConditionText(string names)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("StateOrProvinceConditionText"), names);
		}

		public static string IndividualSettings
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("IndividualSettings");
			}
		}

		public static string CalendarSharingFreeBusyDetail
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("CalendarSharingFreeBusyDetail");
			}
		}

		public static string LongRunInProgressTips
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("LongRunInProgressTips");
			}
		}

		public static string OwaMailboxPolicyChangePassword
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicyChangePassword");
			}
		}

		public static string VoicemailClearSettingsDetailsContactOperator
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("VoicemailClearSettingsDetailsContactOperator");
			}
		}

		public static string DisableFederationStopped
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DisableFederationStopped");
			}
		}

		public static string Success
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Success");
			}
		}

		public static string NoOnboardingPermission
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("NoOnboardingPermission");
			}
		}

		public static string HydratingTitle
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("HydratingTitle");
			}
		}

		public static string TextMessagingNotificationSetupLinkText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("TextMessagingNotificationSetupLinkText");
			}
		}

		public static string WipePendingPendingDisplayText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("WipePendingPendingDisplayText");
			}
		}

		public static string InvalidMultiEmailAddress
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("InvalidMultiEmailAddress");
			}
		}

		public static string DataCenterMainHeader
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DataCenterMainHeader");
			}
		}

		public static string BulkEditNotificationTenMinuteLabel
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("BulkEditNotificationTenMinuteLabel");
			}
		}

		public static string DefaultRuleExceptionLabel
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DefaultRuleExceptionLabel");
			}
		}

		public static string SelectTheTextAndCopy
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("SelectTheTextAndCopy");
			}
		}

		public static string FailedToRetrieveMailboxOnboarding
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("FailedToRetrieveMailboxOnboarding");
			}
		}

		public static string DisabledDisplayText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DisabledDisplayText");
			}
		}

		public static string ConditionValueSeparator
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ConditionValueSeparator");
			}
		}

		public static string LinkedInDelayed
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("LinkedInDelayed");
			}
		}

		public static string ErrorTitle
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ErrorTitle");
			}
		}

		public static string InvalidSmtpAddress
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("InvalidSmtpAddress");
			}
		}

		public static string RemoveLinkedInSupportingText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("RemoveLinkedInSupportingText");
			}
		}

		public static string BulkEditedProgressSomeOperationsFailed(int m)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("BulkEditedProgressSomeOperationsFailed"), m);
		}

		public static string ResumeDBCopyConfirmation
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ResumeDBCopyConfirmation");
			}
		}

		public static string MessageTypeAll
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MessageTypeAll");
			}
		}

		public static string CmdLogTitleForHybridO365
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("CmdLogTitleForHybridO365");
			}
		}

		public static string ConfirmRemoveFacebook
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ConfirmRemoveFacebook");
			}
		}

		public static string DepartmentConditionText(string names)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("DepartmentConditionText"), names);
		}

		public static string BulkEditNotificationMinuteLabel
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("BulkEditNotificationMinuteLabel");
			}
		}

		public static string RecipientContainerText(string name)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("RecipientContainerText"), name);
		}

		public static string VoiceMailText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("VoiceMailText");
			}
		}

		public static string CollapseAll
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("CollapseAll");
			}
		}

		public static string DefaultContactsFolderText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DefaultContactsFolderText");
			}
		}

		public static string OwaMailboxPolicyWeather
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicyWeather");
			}
		}

		public static string LegacyFolderError
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("LegacyFolderError");
			}
		}

		public static string MessageTraceReportTitle
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MessageTraceReportTitle");
			}
		}

		public static string InvalidSmtpDomainWithSubdomains(string str)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("InvalidSmtpDomainWithSubdomains"), str);
		}

		public static string JournalEmailAddressInvalid
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("JournalEmailAddressInvalid");
			}
		}

		public static string JobSubmitted
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("JobSubmitted");
			}
		}

		public static string UMEnableE164ActionSummary
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("UMEnableE164ActionSummary");
			}
		}

		public static string OK
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OK");
			}
		}

		public static string LastPage
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("LastPage");
			}
		}

		public static string OwaMailboxPolicyRemindersAndNotifications
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicyRemindersAndNotifications");
			}
		}

		public static string DataLossWarning
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DataLossWarning");
			}
		}

		public static string SuspendComments
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("SuspendComments");
			}
		}

		public static string Delivered
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Delivered");
			}
		}

		public static string Retry
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Retry");
			}
		}

		public static string Descending
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Descending");
			}
		}

		public static string SimpleFilterTextBoxWaterMark
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("SimpleFilterTextBoxWaterMark");
			}
		}

		public static string TypingDescription
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("TypingDescription");
			}
		}

		public static string NonEditingAuthor
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("NonEditingAuthor");
			}
		}

		public static string MinimumCriteriaFieldsInError
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MinimumCriteriaFieldsInError");
			}
		}

		public static string InvalidNumber(string str)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("InvalidNumber"), str);
		}

		public static string ListSeparator
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ListSeparator");
			}
		}

		public static string ExpandAll
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ExpandAll");
			}
		}

		public static string AutoInternal
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("AutoInternal");
			}
		}

		public static string NeverUse
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("NeverUse");
			}
		}

		public static string NoonPM
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("NoonPM");
			}
		}

		public static string EnableReplicationCommandText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EnableReplicationCommandText");
			}
		}

		public static string HCWCompletedDescription
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("HCWCompletedDescription");
			}
		}

		public static string PWTNS
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("PWTNS");
			}
		}

		public static string DeviceModelPickerAll
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DeviceModelPickerAll");
			}
		}

		public static string NextMonth
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("NextMonth");
			}
		}

		public static string UploaderUnhandledExceptionMessage
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("UploaderUnhandledExceptionMessage");
			}
		}

		public static string MobileExternal
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MobileExternal");
			}
		}

		public static string SearchButtonTooltip
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("SearchButtonTooltip");
			}
		}

		public static string SavingCompletedInformation
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("SavingCompletedInformation");
			}
		}

		public static string SetupExchangeHybrid
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("SetupExchangeHybrid");
			}
		}

		public static string EnableFVA
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EnableFVA");
			}
		}

		public static string PWTNAB
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("PWTNAB");
			}
		}

		public static string ForceConnectMailbox
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ForceConnectMailbox");
			}
		}

		public static string ShowModalDialogErrorReport
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ShowModalDialogErrorReport");
			}
		}

		public static string Imap
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Imap");
			}
		}

		public static string ConnectToFacebookMessage
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ConnectToFacebookMessage");
			}
		}

		public static string ConstraintNoTrailingSpecificCharacter(string str, char invalidChar)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("ConstraintNoTrailingSpecificCharacter"), str, invalidChar);
		}

		public static string DateRangeError
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DateRangeError");
			}
		}

		public static string ConstraintViolationStringOnlyCanContainSpecificCharacters(string characters, string value)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("ConstraintViolationStringOnlyCanContainSpecificCharacters"), characters, value);
		}

		public static string OwaMailboxPolicyUserExperience
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicyUserExperience");
			}
		}

		public static string WebServiceRequestInetError
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("WebServiceRequestInetError");
			}
		}

		public static string FindMeText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("FindMeText");
			}
		}

		public static string CtrlKeySelectAllInListView
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("CtrlKeySelectAllInListView");
			}
		}

		public static string RemoveAction
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("RemoveAction");
			}
		}

		public static string UMHolidayScheduleHolidayEndDateRequiredError
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("UMHolidayScheduleHolidayEndDateRequiredError");
			}
		}

		public static string NetworkCredentialUserNameErrorMessage
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("NetworkCredentialUserNameErrorMessage");
			}
		}

		public static string SetupHybridUIFirst
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("SetupHybridUIFirst");
			}
		}

		public static string UMKeyMappingActionSummaryAnnounceBusinessLocation
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("UMKeyMappingActionSummaryAnnounceBusinessLocation");
			}
		}

		public static string UMExtensionWithDigitLabel(int length)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("UMExtensionWithDigitLabel"), length);
		}

		public static string GroupNamingPolicyCaption
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("GroupNamingPolicyCaption");
			}
		}

		public static string TransferToGalContactText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("TransferToGalContactText");
			}
		}

		public static string UnhandledExceptionMessage
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("UnhandledExceptionMessage");
			}
		}

		public static string OwaMailboxPolicyJunkEmail
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicyJunkEmail");
			}
		}

		public static string InvalidSmtpDomainWithSubdomainsOrIP6(string str)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("InvalidSmtpDomainWithSubdomainsOrIP6"), str);
		}

		public static string DynamicDistributionGroupText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DynamicDistributionGroupText");
			}
		}

		public static string ConstraintViolationStringDoesNotMatchRegularExpression(string pattern, string value)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("ConstraintViolationStringDoesNotMatchRegularExpression"), pattern, value);
		}

		public static string ServerNameColumnText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ServerNameColumnText");
			}
		}

		public static string TextMessagingNotificationNotSetupLinkText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("TextMessagingNotificationNotSetupLinkText");
			}
		}

		public static string QuerySyntaxError
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("QuerySyntaxError");
			}
		}

		public static string SharingDomainOptionAll
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("SharingDomainOptionAll");
			}
		}

		public static string VoicemailWizardEnterPinText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("VoicemailWizardEnterPinText");
			}
		}

		public static string UMEnablePinDigitLabel(int minLength)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("UMEnablePinDigitLabel"), minLength);
		}

		public static string AddressExists
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("AddressExists");
			}
		}

		public static string ModifyExchangeHybrid
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ModifyExchangeHybrid");
			}
		}

		public static string HydrationAndFeatureDone
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("HydrationAndFeatureDone");
			}
		}

		public static string ProviderConnected
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ProviderConnected");
			}
		}

		public static string NoNamingPolicySetup
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("NoNamingPolicySetup");
			}
		}

		public static string DoNotShowDialog
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DoNotShowDialog");
			}
		}

		public static string RemoveMailboxDeleteLiveID
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("RemoveMailboxDeleteLiveID");
			}
		}

		public static string GreetingsAndPromptsTitleText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("GreetingsAndPromptsTitleText");
			}
		}

		public static string VoicemailClearSettingsDetails(string number)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("VoicemailClearSettingsDetails"), number);
		}

		public static string ConfigureVoicemailButtonText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ConfigureVoicemailButtonText");
			}
		}

		public static string FirstNameLastName
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("FirstNameLastName");
			}
		}

		public static string Yes
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Yes");
			}
		}

		public static string Author
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Author");
			}
		}

		public static string PWTRAS
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("PWTRAS");
			}
		}

		public static string TransferToGalContactVoicemailText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("TransferToGalContactVoicemailText");
			}
		}

		public static string MailboxDelegationDetail
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MailboxDelegationDetail");
			}
		}

		public static string Or
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Or");
			}
		}

		public static string Reset
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Reset");
			}
		}

		public static string UpdateTimeZonePrompt
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("UpdateTimeZonePrompt");
			}
		}

		public static string DontSave
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DontSave");
			}
		}

		public static string VoicemailSummaryAccessNumber
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("VoicemailSummaryAccessNumber");
			}
		}

		public static string Save
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Save");
			}
		}

		public static string OwaMailboxPolicyThemeSelection
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicyThemeSelection");
			}
		}

		public static string ReadThis
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ReadThis");
			}
		}

		public static string SubnetIPEditorTitle
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("SubnetIPEditorTitle");
			}
		}

		public static string ModalDialgSingleSRMsgTemplate(int n, string msg)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("ModalDialgSingleSRMsgTemplate"), n, msg);
		}

		public static string UMEnableExtensionAuto
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("UMEnableExtensionAuto");
			}
		}

		public static string WarningPanelMultipleWarnings
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("WarningPanelMultipleWarnings");
			}
		}

		public static string UMHolidayScheduleHolidayNameRequiredError
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("UMHolidayScheduleHolidayNameRequiredError");
			}
		}

		public static string Select24Hours
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Select24Hours");
			}
		}

		public static string MemberUpdateTypeApprovalRequired
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MemberUpdateTypeApprovalRequired");
			}
		}

		public static string DefaultRuleActionLabel
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DefaultRuleActionLabel");
			}
		}

		public static string EmptyValueError
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EmptyValueError");
			}
		}

		public static string ApplyToAllCalls
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ApplyToAllCalls");
			}
		}

		public static string OutOfMemoryErrorMessage
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OutOfMemoryErrorMessage");
			}
		}

		public static string Never
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Never");
			}
		}

		public static string OABExternal
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OABExternal");
			}
		}

		public static string ReconnectToFacebookMessage
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ReconnectToFacebookMessage");
			}
		}

		public static string MemberApprovalHasChanged
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MemberApprovalHasChanged");
			}
		}

		public static string CreatingFolder
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("CreatingFolder");
			}
		}

		public static string UMEnableMailboxAutoSipDescription
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("UMEnableMailboxAutoSipDescription");
			}
		}

		public static string FirstNameLastNameInitial
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("FirstNameLastNameInitial");
			}
		}

		public static string CompanyConditionText(string names)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("CompanyConditionText"), names);
		}

		public static string PleaseWait
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("PleaseWait");
			}
		}

		public static string WhatDoesThisMean
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("WhatDoesThisMean");
			}
		}

		public static string ModalDialogErrorReport
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ModalDialogErrorReport");
			}
		}

		public static string ClearForPicker(string str1, string str2)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("ClearForPicker"), str1, str2);
		}

		public static string SecondaryNavigation
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("SecondaryNavigation");
			}
		}

		public static string ConnectToLinkedInMessage
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ConnectToLinkedInMessage");
			}
		}

		public static string MessageTraceMessageIDCannotContainComma
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MessageTraceMessageIDCannotContainComma");
			}
		}

		public static string ApplyToAllMessages
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ApplyToAllMessages");
			}
		}

		public static string ActiveSyncDisableText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ActiveSyncDisableText");
			}
		}

		public static string BasicAuth
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("BasicAuth");
			}
		}

		public static string VoicemailResetPINTitle
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("VoicemailResetPINTitle");
			}
		}

		public static string LongRunCompletedTips
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("LongRunCompletedTips");
			}
		}

		public static string GroupNamingPolicyPreviewLabel
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("GroupNamingPolicyPreviewLabel");
			}
		}

		public static string AASL
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("AASL");
			}
		}

		public static string MemberUpdateTypeOpen
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MemberUpdateTypeOpen");
			}
		}

		public static string PublishingEditor
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("PublishingEditor");
			}
		}

		public static string LossDataWarning
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("LossDataWarning");
			}
		}

		public static string OwaMailboxPolicyInstantMessaging
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicyInstantMessaging");
			}
		}

		public static string TextTooLongErrorMessage
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("TextTooLongErrorMessage");
			}
		}

		public static string EnableFederationStopped
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EnableFederationStopped");
			}
		}

		public static string GreetingsAndPromptsInstructionsText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("GreetingsAndPromptsInstructionsText");
			}
		}

		public static string UMHolidayScheduleHolidayPromptRequiredError
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("UMHolidayScheduleHolidayPromptRequiredError");
			}
		}

		public static string AddDagMember
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("AddDagMember");
			}
		}

		public static string DistributionGroupText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DistributionGroupText");
			}
		}

		public static string KeyMappingDisplayTextFormat
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("KeyMappingDisplayTextFormat");
			}
		}

		public static string RequestRuleDetailReportTitle
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("RequestRuleDetailReportTitle");
			}
		}

		public static string RequestMalwareDetailReportTitle
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("RequestMalwareDetailReportTitle");
			}
		}

		public static string ConfirmRemoveConnectionTitle
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ConfirmRemoveConnectionTitle");
			}
		}

		public static string WebServicesInternal
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("WebServicesInternal");
			}
		}

		public static string Wait
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Wait");
			}
		}

		public static string DisableOWAConfirm
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DisableOWAConfirm");
			}
		}

		public static string UriKindRelativeOrAbsolute
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("UriKindRelativeOrAbsolute");
			}
		}

		public static string SessionTimeout
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("SessionTimeout");
			}
		}

		public static string Change
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Change");
			}
		}

		public static string LastNameFirstName
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("LastNameFirstName");
			}
		}

		public static string AddIPAddress
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("AddIPAddress");
			}
		}

		public static string InvalidUnlimitedQuotaRegex
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("InvalidUnlimitedQuotaRegex");
			}
		}

		public static string NoSenderAddressWarning
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("NoSenderAddressWarning");
			}
		}

		public static string TextMessagingNotificationSetupText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("TextMessagingNotificationSetupText");
			}
		}

		public static string LastNameFirstNameInitial
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("LastNameFirstNameInitial");
			}
		}

		public static string DateChooserListName(string str1, string str2)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("DateChooserListName"), str1, str2);
		}

		public static string UnhandledExceptionTitle
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("UnhandledExceptionTitle");
			}
		}

		public static string NoConditionErrorText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("NoConditionErrorText");
			}
		}

		public static string FirstPage
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("FirstPage");
			}
		}

		public static string CannotUploadMultipleFiles
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("CannotUploadMultipleFiles");
			}
		}

		public static string ClearButtonTooltip
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ClearButtonTooltip");
			}
		}

		public static string AutoExternal
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("AutoExternal");
			}
		}

		public static string DayAndTimeRangeTooltip
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DayAndTimeRangeTooltip");
			}
		}

		public static string ViewNotificationDetails
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ViewNotificationDetails");
			}
		}

		public static string EASL
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EASL");
			}
		}

		public static string Outlook
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Outlook");
			}
		}

		public static string ConnectProviderCommandText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ConnectProviderCommandText");
			}
		}

		public static string SpecificPhoneNumberText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("SpecificPhoneNumberText");
			}
		}

		public static string Pending
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Pending");
			}
		}

		public static string CalendarSharingFreeBusyReviewer
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("CalendarSharingFreeBusyReviewer");
			}
		}

		public static string LitigationHoldDateNotSet
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("LitigationHoldDateNotSet");
			}
		}

		public static string OwaMailboxPolicyAllAddressLists
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicyAllAddressLists");
			}
		}

		public static string Error
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Error");
			}
		}

		public static string Externaldnslookups
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Externaldnslookups");
			}
		}

		public static string EditVoicemailButtonText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EditVoicemailButtonText");
			}
		}

		public static string FailedToRetrieveMailboxLocalMove
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("FailedToRetrieveMailboxLocalMove");
			}
		}

		public static string AddressLabel(int index)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("AddressLabel"), index);
		}

		public static string ConnectorAllAvailableIPv4
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ConnectorAllAvailableIPv4");
			}
		}

		public static string HydrationAndFeatureDoneTitle
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("HydrationAndFeatureDoneTitle");
			}
		}

		public static string ConstraintViolationNoLeadingOrTrailingWhitespace
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ConstraintViolationNoLeadingOrTrailingWhitespace");
			}
		}

		public static string CalendarSharingFreeBusySimple
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("CalendarSharingFreeBusySimple");
			}
		}

		public static string PageSize
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("PageSize");
			}
		}

		public static string ConstraintFieldsNotMatchError
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ConstraintFieldsNotMatchError");
			}
		}

		public static string Updating
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Updating");
			}
		}

		public static string AdditionalPropertiesLabel
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("AdditionalPropertiesLabel");
			}
		}

		public static string SharedUMAutoAttendantPilotIdentifierListNumberError(int number)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("SharedUMAutoAttendantPilotIdentifierListNumberError"), number);
		}

		public static string JumpToMigrationSlabConfirmation
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("JumpToMigrationSlabConfirmation");
			}
		}

		public static string VoicemailCallFwdContactOperator
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("VoicemailCallFwdContactOperator");
			}
		}

		public static string InvalidValueRange
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("InvalidValueRange");
			}
		}

		public static string NoActionRuleAuditSeverity
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("NoActionRuleAuditSeverity");
			}
		}

		public static string VoicemailClearSettingsConfirmationMessage
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("VoicemailClearSettingsConfirmationMessage");
			}
		}

		public static string WebServicesExternal
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("WebServicesExternal");
			}
		}

		public static string UploaderValidationError
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("UploaderValidationError");
			}
		}

		public static string ServerAboutToExpireWarningText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ServerAboutToExpireWarningText");
			}
		}

		public static string ConditionValueRequriedErrorMessage
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ConditionValueRequriedErrorMessage");
			}
		}

		public static string UMEnableExtensionValidation(int length)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("UMEnableExtensionValidation"), length);
		}

		public static string PreviousePage
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("PreviousePage");
			}
		}

		public static string Ascending
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Ascending");
			}
		}

		public static string EditSubnetCaption
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EditSubnetCaption");
			}
		}

		public static string ChooseAtLeastOneColumn
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ChooseAtLeastOneColumn");
			}
		}

		public static string ConstraintViolationStringLengthTooShort(int minLength, int realLength)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("ConstraintViolationStringLengthTooShort"), minLength, realLength);
		}

		public static string EditSharingEnabledDomains
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EditSharingEnabledDomains");
			}
		}

		public static string Recipients
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Recipients");
			}
		}

		public static string Back
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Back");
			}
		}

		public static string VoicemailPostFwdRecordGreeting
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("VoicemailPostFwdRecordGreeting");
			}
		}

		public static string ColumnChooseFailed
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ColumnChooseFailed");
			}
		}

		public static string TransportRuleBusinessContinuity
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("TransportRuleBusinessContinuity");
			}
		}

		public static string TitleSectionMobileDevices
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("TitleSectionMobileDevices");
			}
		}

		public static string NewFolder
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("NewFolder");
			}
		}

		public static string EnableCommandText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EnableCommandText");
			}
		}

		public static string PrimaryAddressLabel
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("PrimaryAddressLabel");
			}
		}

		public static string IncidentReportContentCustom
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("IncidentReportContentCustom");
			}
		}

		public static string MessageTraceInvalidStartDate
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MessageTraceInvalidStartDate");
			}
		}

		public static string InvalidDecimal1
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("InvalidDecimal1");
			}
		}

		public static string ActivateDBCopyConfirmation
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ActivateDBCopyConfirmation");
			}
		}

		public static string DisableActiveSyncConfirm
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DisableActiveSyncConfirm");
			}
		}

		public static string PleaseWaitWhileSaving
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("PleaseWaitWhileSaving");
			}
		}

		public static string CustomAttributeConditionText(int index, string value)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("CustomAttributeConditionText"), index, value);
		}

		public static string AddConditionButtonText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("AddConditionButtonText");
			}
		}

		public static string AcceptedDomainAuthoritativeWarning
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("AcceptedDomainAuthoritativeWarning");
			}
		}

		public static string Editor
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Editor");
			}
		}

		public static string PlayOnPhoneConnected
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("PlayOnPhoneConnected");
			}
		}

		public static string InvalidDomainName
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("InvalidDomainName");
			}
		}

		public static string SetECPAuthConfirmText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("SetECPAuthConfirmText");
			}
		}

		public static string RuleNameTextBoxLabel
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("RuleNameTextBoxLabel");
			}
		}

		public static string More
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("More");
			}
		}

		public static string FirstNameInitialLastName
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("FirstNameInitialLastName");
			}
		}

		public static string OwaMailboxPolicyCommunicationManagement
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicyCommunicationManagement");
			}
		}

		public static string Owner
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Owner");
			}
		}

		public static string EditDomain
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EditDomain");
			}
		}

		public static string InvalidEmailAddressName
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("InvalidEmailAddressName");
			}
		}

		public static string LeadingTrailingSpaceError
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("LeadingTrailingSpaceError");
			}
		}

		public static string DayRangeAndTimeRangeTooltip
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DayRangeAndTimeRangeTooltip");
			}
		}

		public static string OwaMailboxPolicyPremiumClient
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicyPremiumClient");
			}
		}

		public static string IncidentReportDeselectAll
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("IncidentReportDeselectAll");
			}
		}

		public static string OwaMailboxPolicyNotes
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicyNotes");
			}
		}

		public static string UMEnableSipActionSummary
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("UMEnableSipActionSummary");
			}
		}

		public static string Reviewer
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Reviewer");
			}
		}

		public static string UMExtensionValidationFailure(int length)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("UMExtensionValidationFailure"), length);
		}

		public static string LongRunStoppedTips
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("LongRunStoppedTips");
			}
		}

		public static string RequestSpamDetailReportTitle
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("RequestSpamDetailReportTitle");
			}
		}

		public static string AddExceptionButtonText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("AddExceptionButtonText");
			}
		}

		public static string OwaMailboxPolicyRecoverDeletedItems
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicyRecoverDeletedItems");
			}
		}

		public static string GroupNamingPolicyPreviewDescriptionFooter(string name)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("GroupNamingPolicyPreviewDescriptionFooter"), name);
		}

		public static string AddActionButtonText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("AddActionButtonText");
			}
		}

		public static string EndDateMustBeYesterday
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EndDateMustBeYesterday");
			}
		}

		public static string OwaInternal
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaInternal");
			}
		}

		public static string DateExceedsRange(int numdays)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("DateExceedsRange"), numdays);
		}

		public static string CheckNames
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("CheckNames");
			}
		}

		public static string UMHolidayScheduleHolidayStartEndDateValidationError
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("UMHolidayScheduleHolidayStartEndDateValidationError");
			}
		}

		public static string InvalidMailboxSearchName
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("InvalidMailboxSearchName");
			}
		}

		public static string ConstraintViolationInvalidUriKind(string uri, string expectedUriKind)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("ConstraintViolationInvalidUriKind"), uri, expectedUriKind);
		}

		public static string FinalizedMailboxText(int cur, int n)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("FinalizedMailboxText"), cur, n);
		}

		public static string RemoveMailboxKeepLiveID
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("RemoveMailboxKeepLiveID");
			}
		}

		public static string OwaMailboxPolicySignatures
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicySignatures");
			}
		}

		public static string WorkingHoursText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("WorkingHoursText");
			}
		}

		public static string ESHL
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ESHL");
			}
		}

		public static string FileStillUploading
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("FileStillUploading");
			}
		}

		public static string StartOverMailboxSearch
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("StartOverMailboxSearch");
			}
		}

		public static string DisableConnectorLoggingConfirm
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DisableConnectorLoggingConfirm");
			}
		}

		public static string LongRunDataLossWarning
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("LongRunDataLossWarning");
			}
		}

		public static string ActiveText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ActiveText");
			}
		}

		public static string DisableMOWAConfirm
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DisableMOWAConfirm");
			}
		}

		public static string WarningPanelSeeMoreMsg
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("WarningPanelSeeMoreMsg");
			}
		}

		public static string BulkEditProgress(int m, int n, int o)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("BulkEditProgress"), m, n, o);
		}

		public static string EnterpriseLogoutFail
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EnterpriseLogoutFail");
			}
		}

		public static string AlwaysUse
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("AlwaysUse");
			}
		}

		public static string GuideToSubscriptionPages(string popUrl, string imapUrl, string onClick)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("GuideToSubscriptionPages"), popUrl, imapUrl, onClick);
		}

		public static string PublishingAuthor
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("PublishingAuthor");
			}
		}

		public static string CopyError
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("CopyError");
			}
		}

		public static string LastNameInitialFirstName
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("LastNameInitialFirstName");
			}
		}

		public static string Internaldnslookups
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Internaldnslookups");
			}
		}

		public static string ApplyAllMessagesWarning
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ApplyAllMessagesWarning");
			}
		}

		public static string ModalDialogSREachMsgTemplate(int index, string messageType, string message, string details, string helpMessage)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("ModalDialogSREachMsgTemplate"), new object[]
			{
				index,
				messageType,
				message,
				details,
				helpMessage
			});
		}

		public static string EditIPAddress
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EditIPAddress");
			}
		}

		public static string SecurityGroupText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("SecurityGroupText");
			}
		}

		public static string ResumeMailboxSearch
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ResumeMailboxSearch");
			}
		}

		public static string JumpToOffice365MigrationSlabFailed
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("JumpToOffice365MigrationSlabFailed");
			}
		}

		public static string DeliveryReportEmailBodyForMailTo
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DeliveryReportEmailBodyForMailTo");
			}
		}

		public static string DefaultRuleConditionLabel
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DefaultRuleConditionLabel");
			}
		}

		public static string RequestDLPDetail
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("RequestDLPDetail");
			}
		}

		public static string AuditSeverityLevelLabel
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("AuditSeverityLevelLabel");
			}
		}

		public static string NoneForEmpty
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("NoneForEmpty");
			}
		}

		public static string OwaMailboxPolicyRules
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicyRules");
			}
		}

		public static string CtrlKeyToSave
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("CtrlKeyToSave");
			}
		}

		public static string CustomPeriodText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("CustomPeriodText");
			}
		}

		public static string NSHL
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("NSHL");
			}
		}

		public static string TransferToNumberText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("TransferToNumberText");
			}
		}

		public static string UMKeyMappingActionSummaryAnnounceBusinessHours
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("UMKeyMappingActionSummaryAnnounceBusinessHours");
			}
		}

		public static string OwaMailboxPolicyFacebook
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicyFacebook");
			}
		}

		public static string ConnectMailboxLaunchWizard
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ConnectMailboxLaunchWizard");
			}
		}

		public static string SomeSelectionNotAdded
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("SomeSelectionNotAdded");
			}
		}

		public static string ListViewStatusText(int select, int total)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("ListViewStatusText"), select, total);
		}

		public static string LoadingInformation
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("LoadingInformation");
			}
		}

		public static string ProceedWithoutTenantInfo
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ProceedWithoutTenantInfo");
			}
		}

		public static string IncorrectQueryFieldName(string queryFieldName)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("IncorrectQueryFieldName"), queryFieldName);
		}

		public static string Loading
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Loading");
			}
		}

		public static string DeviceTypePickerAll
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DeviceTypePickerAll");
			}
		}

		public static string VoicemailConfigurationTitle
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("VoicemailConfigurationTitle");
			}
		}

		public static string RemoveDBCopyConfirmation
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("RemoveDBCopyConfirmation");
			}
		}

		public static string MemberUpdateTypeClosed
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MemberUpdateTypeClosed");
			}
		}

		public static string OnCommandText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OnCommandText");
			}
		}

		public static string SelectOne
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("SelectOne");
			}
		}

		public static string NtlmAuth
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("NtlmAuth");
			}
		}

		public static string EnableMOWAConfirm
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EnableMOWAConfirm");
			}
		}

		public static string Pop
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Pop");
			}
		}

		public static string DisabledPendingDisplayText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DisabledPendingDisplayText");
			}
		}

		public static string FirstFocusTextForScreenReader
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("FirstFocusTextForScreenReader");
			}
		}

		public static string MoveUp
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MoveUp");
			}
		}

		public static string GalOrPersonalContactText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("GalOrPersonalContactText");
			}
		}

		public static string AllAvailableIPV4Address
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("AllAvailableIPV4Address");
			}
		}

		public static string OwaExternal
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaExternal");
			}
		}

		public static string ActiveSyncEnableText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ActiveSyncEnableText");
			}
		}

		public static string ConstraintViolationInputUnlimitedValue
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ConstraintViolationInputUnlimitedValue");
			}
		}

		public static string ResetPinMessageTitle
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ResetPinMessageTitle");
			}
		}

		public static string No
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("No");
			}
		}

		public static string LongRunStoppedDescription
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("LongRunStoppedDescription");
			}
		}

		public static string NIAE
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("NIAE");
			}
		}

		public static string BulkEditedProgressNoSuccessFailedCount(int m)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("BulkEditedProgressNoSuccessFailedCount"), m);
		}

		public static string ConstraintViolationStringLengthTooLong(int maxLength, int realLength)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("ConstraintViolationStringLengthTooLong"), maxLength, realLength);
		}

		public static string CtrlKeyCloseForm
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("CtrlKeyCloseForm");
			}
		}

		public static string InvalidUrl
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("InvalidUrl");
			}
		}

		public static string InvalidDomain
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("InvalidDomain");
			}
		}

		public static string Display
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Display");
			}
		}

		public static string SslTitle
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("SslTitle");
			}
		}

		public static string ProviderDelayed
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ProviderDelayed");
			}
		}

		public static string TrialExpiredWarningText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("TrialExpiredWarningText");
			}
		}

		public static string StopProcessingRuleLabel
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("StopProcessingRuleLabel");
			}
		}

		public static string UMEnabledPolicyRequired
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("UMEnabledPolicyRequired");
			}
		}

		public static string All
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("All");
			}
		}

		public static string ClickHereForHelp
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ClickHereForHelp");
			}
		}

		public static string LongRunCompletedDescription
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("LongRunCompletedDescription");
			}
		}

		public static string ConnectToO365
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ConnectToO365");
			}
		}

		public static string NoRetentionPolicy
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("NoRetentionPolicy");
			}
		}

		public static string BulkEditedProgress(int m, int n, int o)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("BulkEditedProgress"), m, n, o);
		}

		public static string SavingInformation
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("SavingInformation");
			}
		}

		public static string InvalidAlias
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("InvalidAlias");
			}
		}

		public static string Contributor
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Contributor");
			}
		}

		public static string Cancel
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Cancel");
			}
		}

		public static string NoIPWarning
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("NoIPWarning");
			}
		}

		public static string UnhandledExceptionDetails(string msg)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("UnhandledExceptionDetails"), msg);
		}

		public static string Custom
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Custom");
			}
		}

		public static string SendPasscodeSucceededFormat(string phone)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("SendPasscodeSucceededFormat"), phone);
		}

		public static string RemovePendingDisplayText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("RemovePendingDisplayText");
			}
		}

		public static string EnabledPendingDisplayText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EnabledPendingDisplayText");
			}
		}

		public static string RecordGreetingLinkText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("RecordGreetingLinkText");
			}
		}

		public static string Next
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Next");
			}
		}

		public static string PrimaryNavigation
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("PrimaryNavigation");
			}
		}

		public static string DeliveryReportEmailSubject(string recipient)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("DeliveryReportEmailSubject"), recipient);
		}

		public static string KeyMappingVoiceMailDisplayText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("KeyMappingVoiceMailDisplayText");
			}
		}

		public static string DuplicatedQueryFieldName(string queryFieldName)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("DuplicatedQueryFieldName"), queryFieldName);
		}

		public static string WarningPanelMultipleWarningsMsg
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("WarningPanelMultipleWarningsMsg");
			}
		}

		public static string RequestMalwareDetail
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("RequestMalwareDetail");
			}
		}

		public static string Information
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Information");
			}
		}

		public static string RequestRuleDetail
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("RequestRuleDetail");
			}
		}

		public static string OutsideWorkingHoursText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OutsideWorkingHoursText");
			}
		}

		public static string UMDialPlanRequiredField
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("UMDialPlanRequiredField");
			}
		}

		public static string DisableFederationCompleted
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("DisableFederationCompleted");
			}
		}

		public static string SharingRuleEntryDomainConflict
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("SharingRuleEntryDomainConflict");
			}
		}

		public static string EditSharingEnabledDomainsStep2
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EditSharingEnabledDomainsStep2");
			}
		}

		public static string NoSpaceValidatorMessage
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("NoSpaceValidatorMessage");
			}
		}

		public static string WaitForHybridUIReady
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("WaitForHybridUIReady");
			}
		}

		public static string MailboxDelegation
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MailboxDelegation");
			}
		}

		public static string UMKeyMappingActionSummaryLeaveVM(string name)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("UMKeyMappingActionSummaryLeaveVM"), name);
		}

		public static string OwaMailboxPolicySecurity
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicySecurity");
			}
		}

		public static string SenderRequiredErrorMessage
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("SenderRequiredErrorMessage");
			}
		}

		public static string ServerTrailDaysColumnText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ServerTrailDaysColumnText");
			}
		}

		public static string ResetPinMessage
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ResetPinMessage");
			}
		}

		public static string RemoveDisabledFacebookConnectionText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("RemoveDisabledFacebookConnectionText");
			}
		}

		public static string InvalidIPAddress(string str)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("InvalidIPAddress"), str);
		}

		public static string ReportTitleMissing
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ReportTitleMissing");
			}
		}

		public static string OwaMailboxPolicyLinkedIn
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicyLinkedIn");
			}
		}

		public static string ConstraintViolationValueOutOfRange
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ConstraintViolationValueOutOfRange");
			}
		}

		public static string MTRTDetailsTitle
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MTRTDetailsTitle");
			}
		}

		public static string Column
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("Column");
			}
		}

		public static string ClickToShowText
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ClickToShowText");
			}
		}

		public static string HybridConfiguration
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("HybridConfiguration");
			}
		}

		public static string PWTAS
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("PWTAS");
			}
		}

		public static string ClickToCopyToClipboard
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ClickToCopyToClipboard");
			}
		}

		public static string UriKindAbsolute
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("UriKindAbsolute");
			}
		}

		public static string GuideToSubscriptionPagePopOrImapOnly(string url, string text, string onClick)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("GuideToSubscriptionPagePopOrImapOnly"), url, text, onClick);
		}

		public static string VoicemailResetPINConfirmationMessage
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("VoicemailResetPINConfirmationMessage");
			}
		}

		public static string ConnectMailboxWizardCaption
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("ConnectMailboxWizardCaption");
			}
		}

		public static string MobileInternal
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("MobileInternal");
			}
		}

		public static string RetrievingStatistics
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("RetrievingStatistics");
			}
		}

		public static string WebServiceRequestTimeout
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("WebServiceRequestTimeout");
			}
		}

		public static string UMEnableTelexActionSummary
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("UMEnableTelexActionSummary");
			}
		}

		public static string ConstraintViolationStringDoesNotContainNonWhitespaceCharacter(string str)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("ConstraintViolationStringDoesNotContainNonWhitespaceCharacter"), str);
		}

		public static string PermissionLevelWarning
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("PermissionLevelWarning");
			}
		}

		public static string GetAllResults
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("GetAllResults");
			}
		}

		public static string MultipleRecipientsSelected(int n)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("MultipleRecipientsSelected"), n);
		}

		public static string ConstraintViolationStringDoesContainsNonASCIICharacter(string value)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("ConstraintViolationStringDoesContainsNonASCIICharacter"), value);
		}

		public static string LongRunCopyToClipboardLabel
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("LongRunCopyToClipboardLabel");
			}
		}

		public static string UMEnablePinValidation(int length)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("UMEnablePinValidation"), length);
		}

		public static string EnableFederationCompleted
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("EnableFederationCompleted");
			}
		}

		public static string OABInternal
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OABInternal");
			}
		}

		public static string QueryValNotInValidRange(string queryValue, string queryName)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("QueryValNotInValidRange"), queryValue, queryName);
		}

		public static string ConstraintViolationStringMustNotContainSpecificCharacters(string characters, string value)
		{
			return string.Format(ClientStrings.ResourceManager.GetString("ConstraintViolationStringMustNotContainSpecificCharacters"), characters, value);
		}

		public static string OwaMailboxPolicyCalendar
		{
			get
			{
				return ClientStrings.ResourceManager.GetString("OwaMailboxPolicyCalendar");
			}
		}

		public static string GetLocalizedString(ClientStrings.IDs key)
		{
			return ClientStrings.ResourceManager.GetString(ClientStrings.stringIDs[(uint)key]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(539);

		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.Management.ControlPanel.ClientStrings", typeof(ClientStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ReconnectProviderCommandText = 616132976U,
			FieldsInError = 1708942702U,
			TlsTitle = 1038968483U,
			UMKeyMappingTimeout = 2085954050U,
			RequiredFieldValidatorErrorMessage = 4245786716U,
			OwaMailboxPolicyTasks = 385068607U,
			CopyIsIEOnly = 375881263U,
			MinimumCriteriaFieldsInErrorDeliveryStatus = 1064772435U,
			EnterHybridUIConfirm = 1884426512U,
			DisableCommandText = 1692971068U,
			UMHolidayScheduleHolidayStartDateRequiredError = 1752619070U,
			EnterHybridUIButtonText = 3063981087U,
			ConstraintViolationValueOutOfRangeForQuota = 3891394090U,
			HydratingMessage = 1245979421U,
			GroupNamingPolicyPreviewDescriptionHeader = 3561883164U,
			Validating = 866598955U,
			UriKindRelative = 1125975266U,
			Close = 3033402446U,
			MoveDown = 3496306419U,
			PeopleConnectBusy = 155777604U,
			LegacyRegExEnabledRuleLabel = 509325205U,
			HydrationDataLossWarning = 1609794183U,
			NoTreeItem = 1398029738U,
			LearnMore = 1133914413U,
			AddEAPCondtionButtonText = 1032072896U,
			InvalidDateRange = 848625314U,
			DefaultRuleEditorCaption = 2717328418U,
			CtrlKeyGoToSearch = 3701726907U,
			DisableFVA = 3084590849U,
			Searching = 3237053118U,
			Update = 859380341U,
			CurrentPolicyCaption = 2781755715U,
			FollowedByColon = 4233379362U,
			EnabledDisplayText = 1281963896U,
			OffDisplayText = 2288607912U,
			OwaMailboxPolicyActiveSyncIntegration = 3866481608U,
			PlayOnPhoneDisconnecting = 915129961U,
			LegacyOUError = 4221859415U,
			WebServiceRequestServerError = 1158475769U,
			Warning = 1861340610U,
			BlockedPendingDisplayText = 2089076466U,
			MessageTypePickerInvalid = 4153102186U,
			OwaMailboxPolicyTextMessaging = 1693255708U,
			OwaMailboxPolicyContacts = 717626182U,
			Expand = 2200441302U,
			DisableConnectorConfirm = 2959308597U,
			CmdLogTitleForHybridEnterprise = 2142856376U,
			ProviderConnectedWithError = 3292878272U,
			RequestSpamDetail = 3612561219U,
			None = 1414246128U,
			PassiveText = 94147420U,
			DisableFederationInProgress = 506561007U,
			MobileDeviceDisableText = 1174301401U,
			MoreOptions = 3442569333U,
			MidnightAM = 2522455672U,
			NotificationCount = 4104581892U,
			ContactsSharing = 906828259U,
			LongRunInProgressDescription = 1644825107U,
			MailboxToSearchRequiredErrorMessage = 2545492551U,
			DomainNoValue = 321700842U,
			MyOptions = 3455735334U,
			VoicemailConfigurationDetails = 335355257U,
			CloseWindowOnLogout = 1803016445U,
			CustomizeColumns = 3832063564U,
			EnterProductKey = 3018361386U,
			PlayOnPhoneDialing = 4158450825U,
			OwaMailboxPolicyUMIntegration = 207011947U,
			HydrationFailed = 1066902945U,
			EnableActiveSyncConfirm = 506890308U,
			ConstraintViolationStringLengthIsEmpty = 3423705850U,
			SelectOneLink = 2455574594U,
			ConstraintNotNullOrEmpty = 2336951795U,
			LitigationHoldOwnerNotSet = 4193083863U,
			RequiredFieldIndicator = 1189851274U,
			FolderTree = 3508492594U,
			IncidentReportSelectAll = 3771622829U,
			Notification = 3819852093U,
			HydrationDoneFeatureFailed = 2571312913U,
			LongRunWarningLabel = 1749577099U,
			PublicFoldersEmptyDataTextRoot = 1796910490U,
			Unsuccessful = 1517827063U,
			TextMessagingNotificationNotSetupText = 3951041989U,
			VoicemailConfigurationConfirmationMessage = 879792491U,
			EnableFederationInProgress = 865985204U,
			OwaMailboxPolicyAllowCopyContactsToDeviceAddressBook = 3625629356U,
			OwaMailboxPolicyInformationManagement = 2357636744U,
			WarningPanelDisMissMsg = 3506211787U,
			OwaMailboxPolicyJournal = 1523028278U,
			DatesNotDefined = 993523547U,
			EnableOWAConfirm = 4062764466U,
			CancelWipePendingDisplayText = 2266587541U,
			DeliveryReportSearchFieldsInError = 2546927256U,
			MyOrganization = 2954372719U,
			Today = 3927445923U,
			ExtendedReportsInsufficientData = 1992800455U,
			EnableConnectorLoggingConfirm = 3963002503U,
			MessageTraceInvalidEndDate = 2805968874U,
			AddSubnetCaption = 1788720538U,
			CustomizeSenderLabel = 4091981230U,
			SharedUMAutoAttendantPilotIdentifierListE164Error = 233002034U,
			PreviousMonth = 1562506021U,
			Stop = 3833953726U,
			AllAvailableIPV6Address = 3958687323U,
			LetCallersInterruptGreetingsText = 3462654059U,
			GroupNamingPolicyEditorPrefixLabel = 3386445864U,
			Transferred = 1102979128U,
			NewDomain = 2787872604U,
			PublicFoldersEmptyDataTextChildren = 2390512187U,
			FacebookDelayed = 2682114028U,
			Collapse = 1342839575U,
			GroupNamingPolicyEditorSuffixLabel = 4211282305U,
			MessageFontSampleText = 630233035U,
			HideModalDialogErrorReport = 3186571665U,
			JobSubmissionWaitText = 187569137U,
			ServiceNone = 4151098027U,
			Page = 2328220407U,
			NavigateAway = 2828598385U,
			RemoveDisabledLinkedInConnectionText = 1122075U,
			HCWStoppedDescription = 59720971U,
			OnDisplayText = 2658434708U,
			LongRunErrorLabel = 1981061025U,
			EIAE = 809706446U,
			NegotiateAuth = 1907621764U,
			RequestDLPDetailReportTitle = 3122995196U,
			WipeConfirmMessage = 2736422594U,
			MobileDeviceEnableText = 317346726U,
			EnableConnectorConfirm = 3197491520U,
			OffCommandText = 1875331145U,
			HydrationDoneTitle = 3778906594U,
			ConnectorAllAvailableIPv6 = 1144247250U,
			UMCallAnsweringRulesEditorRuleConditionLabelText = 1795974124U,
			OwaMailboxPolicyPlaces = 3225869645U,
			JournalEmailAddressLabel = 602293241U,
			PopupBlockedMessage = 259008929U,
			IUnderstandAction = 2802879859U,
			Select15Minutes = 2532213629U,
			AllowedPendingDisplayText = 1811854250U,
			OwaMailboxPolicyTimeManagement = 1848756223U,
			PasswordNote = 3077953159U,
			HasLinkQueryField = 573217698U,
			VoicemailClearSettingsTitle = 1318187683U,
			ConfigureOAuth = 3269293275U,
			And = 3068683287U,
			VoicemailResetPINSuccessMessage = 4210295185U,
			FileDownloadFailed = 3121279917U,
			ConfirmRemoveLinkedIn = 3545744552U,
			RemoveFacebookSupportingText = 1354285736U,
			ListViewMoreResultsWarning = 4266824568U,
			DisableReplicationCommandText = 795464922U,
			EnterpriseMainHeader = 519362517U,
			AddGroupNamingPolicyElementButtonText = 313078617U,
			UseAlias = 571113625U,
			FileUploadFailed = 1300295012U,
			CustomDateLink = 1277846131U,
			PolicyGroupMembership = 4122267321U,
			NextPage = 1548165396U,
			HydrationDone = 437474464U,
			ProviderDisabled = 4033288601U,
			IndividualSettings = 306768456U,
			CalendarSharingFreeBusyDetail = 616309426U,
			LongRunInProgressTips = 2134452995U,
			OwaMailboxPolicyChangePassword = 2004565666U,
			VoicemailClearSettingsDetailsContactOperator = 1595040325U,
			DisableFederationStopped = 2738758716U,
			Success = 2659939651U,
			NoOnboardingPermission = 2197391249U,
			HydratingTitle = 183956792U,
			TextMessagingNotificationSetupLinkText = 2622420678U,
			WipePendingPendingDisplayText = 533757016U,
			InvalidMultiEmailAddress = 1565584202U,
			DataCenterMainHeader = 4033771799U,
			BulkEditNotificationTenMinuteLabel = 893456894U,
			DefaultRuleExceptionLabel = 124543120U,
			SelectTheTextAndCopy = 2402178338U,
			FailedToRetrieveMailboxOnboarding = 2165251921U,
			DisabledDisplayText = 3546966747U,
			ConditionValueSeparator = 1361553573U,
			LinkedInDelayed = 2388288698U,
			ErrorTitle = 933672694U,
			InvalidSmtpAddress = 3042237373U,
			RemoveLinkedInSupportingText = 128074410U,
			ResumeDBCopyConfirmation = 2368306281U,
			MessageTypeAll = 1168324224U,
			CmdLogTitleForHybridO365 = 3966162852U,
			ConfirmRemoveFacebook = 207358046U,
			BulkEditNotificationMinuteLabel = 3329734225U,
			VoiceMailText = 2523533992U,
			CollapseAll = 2616506832U,
			DefaultContactsFolderText = 1728961555U,
			OwaMailboxPolicyWeather = 979822077U,
			LegacyFolderError = 1129028723U,
			MessageTraceReportTitle = 2904016598U,
			JournalEmailAddressInvalid = 2966716344U,
			JobSubmitted = 2501247758U,
			UMEnableE164ActionSummary = 3058620969U,
			OK = 2041362128U,
			LastPage = 3303348785U,
			OwaMailboxPolicyRemindersAndNotifications = 2715655425U,
			DataLossWarning = 1359139161U,
			SuspendComments = 3428137968U,
			Delivered = 1531436154U,
			Retry = 479196852U,
			Descending = 1777112844U,
			SimpleFilterTextBoxWaterMark = 872019732U,
			TypingDescription = 2058638459U,
			NonEditingAuthor = 4012579652U,
			MinimumCriteriaFieldsInError = 3976037671U,
			ListSeparator = 1349255527U,
			ExpandAll = 18372887U,
			AutoInternal = 968612268U,
			NeverUse = 3953482277U,
			NoonPM = 4257989793U,
			EnableReplicationCommandText = 2204193123U,
			HCWCompletedDescription = 1536001239U,
			PWTNS = 3618788766U,
			DeviceModelPickerAll = 3667211102U,
			NextMonth = 1040160067U,
			UploaderUnhandledExceptionMessage = 3066268041U,
			MobileExternal = 1677432033U,
			SearchButtonTooltip = 184677197U,
			SavingCompletedInformation = 1715033809U,
			SetupExchangeHybrid = 2274379572U,
			EnableFVA = 1355682252U,
			PWTNAB = 3247026326U,
			ForceConnectMailbox = 1759109339U,
			ShowModalDialogErrorReport = 4291377740U,
			Imap = 2583693733U,
			ConnectToFacebookMessage = 3122803756U,
			DateRangeError = 1638106043U,
			OwaMailboxPolicyUserExperience = 1138816392U,
			WebServiceRequestInetError = 3831099320U,
			FindMeText = 839901080U,
			CtrlKeySelectAllInListView = 3186529231U,
			RemoveAction = 3086537020U,
			UMHolidayScheduleHolidayEndDateRequiredError = 2835992379U,
			NetworkCredentialUserNameErrorMessage = 3179034952U,
			SetupHybridUIFirst = 1579815837U,
			UMKeyMappingActionSummaryAnnounceBusinessLocation = 1205297021U,
			GroupNamingPolicyCaption = 1139168619U,
			TransferToGalContactText = 3022174399U,
			UnhandledExceptionMessage = 4125561529U,
			OwaMailboxPolicyJunkEmail = 2614439623U,
			DynamicDistributionGroupText = 4070877873U,
			ServerNameColumnText = 2522496037U,
			TextMessagingNotificationNotSetupLinkText = 1445885649U,
			QuerySyntaxError = 3450836391U,
			SharingDomainOptionAll = 1278568394U,
			VoicemailWizardEnterPinText = 1907037086U,
			AddressExists = 4068892486U,
			ModifyExchangeHybrid = 1987425903U,
			HydrationAndFeatureDone = 2944220729U,
			ProviderConnected = 997295902U,
			NoNamingPolicySetup = 3174388608U,
			DoNotShowDialog = 3000098309U,
			RemoveMailboxDeleteLiveID = 238804546U,
			GreetingsAndPromptsTitleText = 1527687315U,
			ConfigureVoicemailButtonText = 3621606012U,
			FirstNameLastName = 613554384U,
			Yes = 3021629903U,
			Author = 3560108081U,
			PWTRAS = 148099519U,
			TransferToGalContactVoicemailText = 2482463458U,
			MailboxDelegationDetail = 2706637479U,
			Or = 381169853U,
			Reset = 1594942057U,
			UpdateTimeZonePrompt = 1904041070U,
			DontSave = 3369491564U,
			VoicemailSummaryAccessNumber = 2908249814U,
			Save = 2328220357U,
			OwaMailboxPolicyThemeSelection = 2864395898U,
			ReadThis = 1393796710U,
			SubnetIPEditorTitle = 2379979535U,
			UMEnableExtensionAuto = 3782146179U,
			WarningPanelMultipleWarnings = 1062541333U,
			UMHolidayScheduleHolidayNameRequiredError = 502251987U,
			Select24Hours = 3674505245U,
			MemberUpdateTypeApprovalRequired = 2794185135U,
			DefaultRuleActionLabel = 639660141U,
			EmptyValueError = 4195962670U,
			ApplyToAllCalls = 2794530675U,
			OutOfMemoryErrorMessage = 3908900425U,
			Never = 1594930120U,
			OABExternal = 2694533749U,
			ReconnectToFacebookMessage = 320649385U,
			MemberApprovalHasChanged = 1889215887U,
			CreatingFolder = 2257237083U,
			UMEnableMailboxAutoSipDescription = 2743921036U,
			FirstNameLastNameInitial = 367868548U,
			PleaseWait = 1690161621U,
			WhatDoesThisMean = 2091505548U,
			ModalDialogErrorReport = 3036539697U,
			SecondaryNavigation = 354624350U,
			ConnectToLinkedInMessage = 305410994U,
			MessageTraceMessageIDCannotContainComma = 503026860U,
			ApplyToAllMessages = 918737566U,
			ActiveSyncDisableText = 689982738U,
			BasicAuth = 637136194U,
			VoicemailResetPINTitle = 2870521773U,
			LongRunCompletedTips = 2777189520U,
			GroupNamingPolicyPreviewLabel = 3817888713U,
			AASL = 2053059583U,
			MemberUpdateTypeOpen = 924670953U,
			PublishingEditor = 2013213994U,
			LossDataWarning = 1356895513U,
			OwaMailboxPolicyInstantMessaging = 2014560916U,
			TextTooLongErrorMessage = 2093942654U,
			EnableFederationStopped = 1833904645U,
			GreetingsAndPromptsInstructionsText = 912552836U,
			UMHolidayScheduleHolidayPromptRequiredError = 771355430U,
			AddDagMember = 1515892343U,
			DistributionGroupText = 259319032U,
			KeyMappingDisplayTextFormat = 1244644071U,
			RequestRuleDetailReportTitle = 3924205880U,
			RequestMalwareDetailReportTitle = 1865766159U,
			ConfirmRemoveConnectionTitle = 173806984U,
			WebServicesInternal = 2320161129U,
			Wait = 49675595U,
			DisableOWAConfirm = 4196350991U,
			UriKindRelativeOrAbsolute = 2663567466U,
			SessionTimeout = 2122906481U,
			Change = 1742879518U,
			LastNameFirstName = 339824766U,
			AddIPAddress = 204633756U,
			InvalidUnlimitedQuotaRegex = 1091377885U,
			NoSenderAddressWarning = 1623670356U,
			TextMessagingNotificationSetupText = 3333839866U,
			LastNameFirstNameInitial = 2334202318U,
			UnhandledExceptionTitle = 2124576504U,
			NoConditionErrorText = 3689797535U,
			FirstPage = 3348900521U,
			CannotUploadMultipleFiles = 1673297715U,
			ClearButtonTooltip = 3328778458U,
			AutoExternal = 2941133106U,
			DayAndTimeRangeTooltip = 768075136U,
			ViewNotificationDetails = 3147146074U,
			EASL = 2053059451U,
			Outlook = 965845689U,
			ConnectProviderCommandText = 3904375665U,
			SpecificPhoneNumberText = 2391603832U,
			Pending = 334057287U,
			CalendarSharingFreeBusyReviewer = 3216786356U,
			LitigationHoldDateNotSet = 3097242366U,
			OwaMailboxPolicyAllAddressLists = 2796848775U,
			Error = 22442200U,
			Externaldnslookups = 1466672057U,
			EditVoicemailButtonText = 3849511688U,
			FailedToRetrieveMailboxLocalMove = 903130308U,
			ConnectorAllAvailableIPv4 = 1144247248U,
			HydrationAndFeatureDoneTitle = 2262897061U,
			ConstraintViolationNoLeadingOrTrailingWhitespace = 2058499689U,
			CalendarSharingFreeBusySimple = 2862582797U,
			PageSize = 4277755996U,
			ConstraintFieldsNotMatchError = 2742068696U,
			Updating = 3900615856U,
			AdditionalPropertiesLabel = 353630484U,
			JumpToMigrationSlabConfirmation = 1011589268U,
			VoicemailCallFwdContactOperator = 7739616U,
			InvalidValueRange = 2680934369U,
			NoActionRuleAuditSeverity = 2697991219U,
			VoicemailClearSettingsConfirmationMessage = 3273613659U,
			WebServicesExternal = 1655108951U,
			UploaderValidationError = 2101584371U,
			ServerAboutToExpireWarningText = 3336188783U,
			ConditionValueRequriedErrorMessage = 3200788962U,
			PreviousePage = 439636863U,
			Ascending = 3390434404U,
			EditSubnetCaption = 2068973737U,
			ChooseAtLeastOneColumn = 267746805U,
			EditSharingEnabledDomains = 2900910704U,
			Recipients = 986397318U,
			Back = 2778558511U,
			VoicemailPostFwdRecordGreeting = 3068528108U,
			ColumnChooseFailed = 1857390484U,
			TransportRuleBusinessContinuity = 1219538243U,
			TitleSectionMobileDevices = 936826416U,
			NewFolder = 2894502448U,
			EnableCommandText = 3265439859U,
			PrimaryAddressLabel = 583273118U,
			IncidentReportContentCustom = 3843987112U,
			MessageTraceInvalidStartDate = 3566331407U,
			InvalidDecimal1 = 3393270247U,
			ActivateDBCopyConfirmation = 2636721601U,
			DisableActiveSyncConfirm = 1063565289U,
			PleaseWaitWhileSaving = 652753372U,
			AddConditionButtonText = 4082347473U,
			AcceptedDomainAuthoritativeWarning = 3022976242U,
			Editor = 3653626825U,
			PlayOnPhoneConnected = 1801819654U,
			InvalidDomainName = 2762661174U,
			SetECPAuthConfirmText = 2068086983U,
			RuleNameTextBoxLabel = 4262899755U,
			More = 1414245989U,
			FirstNameInitialLastName = 2549262094U,
			OwaMailboxPolicyCommunicationManagement = 2810364182U,
			Owner = 2731136937U,
			EditDomain = 1400729458U,
			InvalidEmailAddressName = 3134721922U,
			LeadingTrailingSpaceError = 467409560U,
			DayRangeAndTimeRangeTooltip = 2307360811U,
			OwaMailboxPolicyPremiumClient = 3180237931U,
			IncidentReportDeselectAll = 1849292272U,
			OwaMailboxPolicyNotes = 1911032188U,
			UMEnableSipActionSummary = 3151174489U,
			Reviewer = 1045743613U,
			LongRunStoppedTips = 1761836208U,
			RequestSpamDetailReportTitle = 1259584465U,
			AddExceptionButtonText = 288995097U,
			OwaMailboxPolicyRecoverDeletedItems = 1283935370U,
			AddActionButtonText = 117844352U,
			EndDateMustBeYesterday = 2009538927U,
			OwaInternal = 3100497742U,
			CheckNames = 2920201570U,
			UMHolidayScheduleHolidayStartEndDateValidationError = 2752727145U,
			InvalidMailboxSearchName = 3104459904U,
			RemoveMailboxKeepLiveID = 2559957656U,
			OwaMailboxPolicySignatures = 3766407320U,
			WorkingHoursText = 3982517327U,
			ESHL = 3189265994U,
			FileStillUploading = 1911656207U,
			StartOverMailboxSearch = 472573372U,
			DisableConnectorLoggingConfirm = 9358886U,
			LongRunDataLossWarning = 228304082U,
			ActiveText = 798401137U,
			DisableMOWAConfirm = 461710906U,
			WarningPanelSeeMoreMsg = 1400887235U,
			EnterpriseLogoutFail = 2860566285U,
			AlwaysUse = 3375594338U,
			PublishingAuthor = 1271124552U,
			CopyError = 3934389177U,
			LastNameInitialFirstName = 3980910748U,
			Internaldnslookups = 2757672907U,
			ApplyAllMessagesWarning = 2794407267U,
			EditIPAddress = 3799642099U,
			SecurityGroupText = 265625760U,
			ResumeMailboxSearch = 356602967U,
			JumpToOffice365MigrationSlabFailed = 3688802170U,
			DeliveryReportEmailBodyForMailTo = 3738684211U,
			DefaultRuleConditionLabel = 2929991304U,
			RequestDLPDetail = 1506239268U,
			AuditSeverityLevelLabel = 667665198U,
			NoneForEmpty = 3800601796U,
			OwaMailboxPolicyRules = 210154378U,
			CtrlKeyToSave = 4035228826U,
			CustomPeriodText = 3041967243U,
			NSHL = 3189266341U,
			TransferToNumberText = 663637318U,
			UMKeyMappingActionSummaryAnnounceBusinessHours = 2950784627U,
			OwaMailboxPolicyFacebook = 912527261U,
			ConnectMailboxLaunchWizard = 1929634872U,
			SomeSelectionNotAdded = 280551769U,
			LoadingInformation = 32190104U,
			ProceedWithoutTenantInfo = 1710116876U,
			Loading = 3599592070U,
			DeviceTypePickerAll = 1891837447U,
			VoicemailConfigurationTitle = 892936075U,
			RemoveDBCopyConfirmation = 2336878616U,
			MemberUpdateTypeClosed = 1308147703U,
			OnCommandText = 2359945479U,
			SelectOne = 779120846U,
			NtlmAuth = 49706295U,
			EnableMOWAConfirm = 1716565179U,
			Pop = 1502600087U,
			DisabledPendingDisplayText = 882294734U,
			FirstFocusTextForScreenReader = 2145940917U,
			MoveUp = 137938150U,
			GalOrPersonalContactText = 347463522U,
			AllAvailableIPV4Address = 2903019109U,
			OwaExternal = 3765422820U,
			ActiveSyncEnableText = 1015147527U,
			ConstraintViolationInputUnlimitedValue = 2594574748U,
			ResetPinMessageTitle = 1603757297U,
			No = 1496915101U,
			LongRunStoppedDescription = 2846873532U,
			NIAE = 809706807U,
			CtrlKeyCloseForm = 116135148U,
			InvalidUrl = 3705047298U,
			InvalidDomain = 3403459873U,
			Display = 2852593944U,
			SslTitle = 2814896254U,
			ProviderDelayed = 4028691857U,
			TrialExpiredWarningText = 3512966386U,
			StopProcessingRuleLabel = 1496927839U,
			UMEnabledPolicyRequired = 1872640986U,
			All = 4231482709U,
			ClickHereForHelp = 238716676U,
			LongRunCompletedDescription = 945688236U,
			ConnectToO365 = 4193953058U,
			NoRetentionPolicy = 2021920769U,
			SavingInformation = 995902246U,
			InvalidAlias = 2939777527U,
			Contributor = 1226897823U,
			Cancel = 2358390244U,
			NoIPWarning = 3309738928U,
			Custom = 427688167U,
			RemovePendingDisplayText = 664253366U,
			EnabledPendingDisplayText = 892169149U,
			RecordGreetingLinkText = 3027332405U,
			Next = 3423762231U,
			PrimaryNavigation = 4164526598U,
			KeyMappingVoiceMailDisplayText = 2835676179U,
			WarningPanelMultipleWarningsMsg = 2381414750U,
			RequestMalwareDetail = 67348609U,
			Information = 2193536568U,
			RequestRuleDetail = 1749370292U,
			OutsideWorkingHoursText = 2470451672U,
			UMDialPlanRequiredField = 1325539240U,
			DisableFederationCompleted = 3205750596U,
			SharingRuleEntryDomainConflict = 3276462966U,
			EditSharingEnabledDomainsStep2 = 4005194968U,
			NoSpaceValidatorMessage = 2220221264U,
			WaitForHybridUIReady = 3125926615U,
			MailboxDelegation = 684930172U,
			OwaMailboxPolicySecurity = 1396631711U,
			SenderRequiredErrorMessage = 3453349145U,
			ServerTrailDaysColumnText = 1294686955U,
			ResetPinMessage = 1173301205U,
			RemoveDisabledFacebookConnectionText = 1482742945U,
			ReportTitleMissing = 214141546U,
			OwaMailboxPolicyLinkedIn = 2593970847U,
			ConstraintViolationValueOutOfRange = 1904327111U,
			MTRTDetailsTitle = 3379654077U,
			Column = 2658661820U,
			ClickToShowText = 4021298411U,
			HybridConfiguration = 2116880466U,
			PWTAS = 2408935185U,
			ClickToCopyToClipboard = 3243184343U,
			UriKindAbsolute = 3296253953U,
			VoicemailResetPINConfirmationMessage = 1147585405U,
			ConnectMailboxWizardCaption = 1513997499U,
			MobileInternal = 2506867635U,
			RetrievingStatistics = 2184256814U,
			WebServiceRequestTimeout = 3983756775U,
			UMEnableTelexActionSummary = 3868849319U,
			PermissionLevelWarning = 3698078783U,
			GetAllResults = 3774473073U,
			LongRunCopyToClipboardLabel = 259417423U,
			EnableFederationCompleted = 721624957U,
			OABInternal = 1957457715U,
			OwaMailboxPolicyCalendar = 2767693563U
		}

		private enum ParamIDs
		{
			ModalDialgMultipleSRMsgTemplate,
			ErrorMessageInvalidInteger,
			ModalDialogSRHelpTemplate,
			RemoveCertificateConfirm,
			QueryValueStartsWithStar,
			InvalidInteger,
			InvalidCharacter,
			RenewCertificateHelp,
			CancelForAjaxUploader,
			UMKeyMappingActionSummaryTransferToAA,
			UMKeyMappingActionSummaryTransferToExtension,
			AutoProvisionFailedMsg,
			NameFromFirstInitialsLastName,
			WipeConfirmTitle,
			BulkEditProgressNoSuccessFailedCount,
			InvalidNumberValue,
			ClearForAjaxUploader,
			DeliveryReportEmailBody,
			WrongExtension,
			ConstraintViolationLocalLongFullPath,
			ConstraintViolationUNCFilePath,
			EmptyQueryValue,
			ApplyToAllSelected,
			ValidationErrorFormat,
			SyncedMailboxText,
			NameFromFirstLastName,
			InvalidEmailAddress,
			StateOrProvinceConditionText,
			BulkEditedProgressSomeOperationsFailed,
			DepartmentConditionText,
			RecipientContainerText,
			InvalidSmtpDomainWithSubdomains,
			InvalidNumber,
			ConstraintNoTrailingSpecificCharacter,
			ConstraintViolationStringOnlyCanContainSpecificCharacters,
			UMExtensionWithDigitLabel,
			InvalidSmtpDomainWithSubdomainsOrIP6,
			ConstraintViolationStringDoesNotMatchRegularExpression,
			UMEnablePinDigitLabel,
			VoicemailClearSettingsDetails,
			ModalDialgSingleSRMsgTemplate,
			CompanyConditionText,
			ClearForPicker,
			DateChooserListName,
			AddressLabel,
			SharedUMAutoAttendantPilotIdentifierListNumberError,
			UMEnableExtensionValidation,
			ConstraintViolationStringLengthTooShort,
			CustomAttributeConditionText,
			UMExtensionValidationFailure,
			GroupNamingPolicyPreviewDescriptionFooter,
			DateExceedsRange,
			ConstraintViolationInvalidUriKind,
			FinalizedMailboxText,
			BulkEditProgress,
			GuideToSubscriptionPages,
			ModalDialogSREachMsgTemplate,
			ListViewStatusText,
			IncorrectQueryFieldName,
			BulkEditedProgressNoSuccessFailedCount,
			ConstraintViolationStringLengthTooLong,
			BulkEditedProgress,
			UnhandledExceptionDetails,
			SendPasscodeSucceededFormat,
			DeliveryReportEmailSubject,
			DuplicatedQueryFieldName,
			UMKeyMappingActionSummaryLeaveVM,
			InvalidIPAddress,
			GuideToSubscriptionPagePopOrImapOnly,
			ConstraintViolationStringDoesNotContainNonWhitespaceCharacter,
			MultipleRecipientsSelected,
			ConstraintViolationStringDoesContainsNonASCIICharacter,
			UMEnablePinValidation,
			QueryValNotInValidRange,
			ConstraintViolationStringMustNotContainSpecificCharacters
		}
	}
}
