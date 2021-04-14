using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class UserOptionPropertySchema : UserConfigurationPropertySchemaBase
	{
		private UserOptionPropertySchema()
		{
		}

		internal static UserOptionPropertySchema Instance
		{
			get
			{
				if (UserOptionPropertySchema.instance == null)
				{
					UserOptionPropertySchema.instance = new UserOptionPropertySchema();
				}
				return UserOptionPropertySchema.instance;
			}
		}

		internal override UserConfigurationPropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return UserOptionPropertySchema.propertyDefinitions;
			}
		}

		internal override UserConfigurationPropertyId PropertyDefinitionsBaseId
		{
			get
			{
				return UserConfigurationPropertyId.TimeZone;
			}
		}

		private static readonly UserConfigurationPropertyDefinition[] propertyDefinitions = new UserConfigurationPropertyDefinition[]
		{
			new UserConfigurationPropertyDefinition("timezone", typeof(string), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateTimeZoneCallback)),
			new UserConfigurationPropertyDefinition("timeformat", typeof(string), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateTimeFormatCallback)),
			new UserConfigurationPropertyDefinition("dateformat", typeof(string), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateDateFormatCallback)),
			new UserConfigurationPropertyDefinition("weekstartday", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateWeekStartDayCallback)),
			new UserConfigurationPropertyDefinition("hourincrement", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateHourIncrementCallback)),
			new UserConfigurationPropertyDefinition("showweeknumbers", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateShowWeekNumbersCallback)),
			new UserConfigurationPropertyDefinition("checknameincontactsfirst", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateCheckNameInContactsFirstCallback)),
			new UserConfigurationPropertyDefinition("firstweekofyear", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateFirstWeekOfYearCallback)),
			new UserConfigurationPropertyDefinition("enablereminders", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateEnableRemindersCallback)),
			new UserConfigurationPropertyDefinition("enableremindersound", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateEnableReminderSoundCallback)),
			new UserConfigurationPropertyDefinition("newitemnotify", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateNewItemNotifyCallback)),
			new UserConfigurationPropertyDefinition("viewrowcount", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateViewRowCountCallback)),
			new UserConfigurationPropertyDefinition("basicviewrowcount", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateBasicViewRowCountCallback)),
			new UserConfigurationPropertyDefinition("spellingdictionarylanguage", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateSpellingDictionaryLanguageCallback)),
			new UserConfigurationPropertyDefinition("spellingignoreuppercase", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateSpellingIgnoreUppercaseCallback)),
			new UserConfigurationPropertyDefinition("spellingignoremixeddigits", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateSpellingIgnoreMixedDigitsCallback)),
			new UserConfigurationPropertyDefinition("spellingcheckbeforesend", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateSpellingCheckBeforeSendCallback)),
			new UserConfigurationPropertyDefinition("SmimeEncrypt", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateSmimeEncryptCallback)),
			new UserConfigurationPropertyDefinition("SmimeSign", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateSmimeSignCallback)),
			new UserConfigurationPropertyDefinition("alwaysshowbcc", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateAlwaysShowBccCallback)),
			new UserConfigurationPropertyDefinition("alwaysshowfrom", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateAlwaysShowFromCallback)),
			new UserConfigurationPropertyDefinition("composemarkup", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateComposeMarkupCallback)),
			new UserConfigurationPropertyDefinition("composefontname", typeof(string), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateComposeFontNameCallback)),
			new UserConfigurationPropertyDefinition("composefontsize", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateComposeFontSizeCallback)),
			new UserConfigurationPropertyDefinition("composefontcolor", typeof(string), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateComposeFontColorCallback)),
			new UserConfigurationPropertyDefinition("composefontflags", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateComposeFontFlagsCallback)),
			new UserConfigurationPropertyDefinition("autoaddsignature", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateAutoAddSignatureCallback)),
			new UserConfigurationPropertyDefinition("signaturetext", typeof(string), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateSignatureTextCallback)),
			new UserConfigurationPropertyDefinition("signaturehtml", typeof(string), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateSignatureHtmlCallback)),
			new UserConfigurationPropertyDefinition("autoaddsignatureonmobile", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateAutoAddSignatureOnMobileCallback)),
			new UserConfigurationPropertyDefinition("signaturetextonmobile", typeof(string), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateSignatureTextOnMobileCallback)),
			new UserConfigurationPropertyDefinition("usedesktopsignature", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateUseDesktopSignatureCallback)),
			new UserConfigurationPropertyDefinition("blockexternalcontent", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateBlockExternalContentCallback)),
			new UserConfigurationPropertyDefinition("previewmarkasread", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidatePreviewMarkAsReadCallback)),
			new UserConfigurationPropertyDefinition("markasreaddelaytime", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateMarkAsReadDelaytimeCallback)),
			new UserConfigurationPropertyDefinition("nextselection", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateNextSelectionCallback)),
			new UserConfigurationPropertyDefinition("readreceipt", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateReadReceiptCallback)),
			new UserConfigurationPropertyDefinition("emptydeleteditemsonlogoff", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateEmptyDeletedItemsOnLogoffCallback)),
			new UserConfigurationPropertyDefinition("NavigationBarWidth", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateNavigationBarWidthCallback)),
			new UserConfigurationPropertyDefinition("NavigationBarWidthRatio", typeof(string), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateNavigationBarWidthRatioCallback)),
			new UserConfigurationPropertyDefinition("MailFolderPaneExpanded", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateMailFolderPaneExpandedCallback)),
			new UserConfigurationPropertyDefinition("IsFavoritesFolderTreeCollapsed", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateIsFavoritesFolderTreeCollapsedCallback)),
			new UserConfigurationPropertyDefinition("IsMailRootFolderTreeCollapsed", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateIsMailRootFolderTreeCollapsedCallback)),
			new UserConfigurationPropertyDefinition("IsPeopleIKnowFolderTreeCollapsed", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateIsPeopleIKnowFolderTreeCollapsedCallback)),
			new UserConfigurationPropertyDefinition("ShowReadingPaneOnFirstLoad", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateShowReadingPaneOnFirstLoadCallback)),
			new UserConfigurationPropertyDefinition("isminibarvisible", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateMiniBarCallback)),
			new UserConfigurationPropertyDefinition("isquicklinksbarvisible", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateQuickLinksCallback)),
			new UserConfigurationPropertyDefinition("istaskdetailsvisible", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateTaskDetailsCallback)),
			new UserConfigurationPropertyDefinition("isdocumentfavoritesvisible", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateDocumentFavoritesCallback)),
			new UserConfigurationPropertyDefinition("isoutlooksharedfoldersvisible", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateOutlookSharedFoldersCallback)),
			new UserConfigurationPropertyDefinition("FormatBarState", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateFormatBarStateCallback)),
			new UserConfigurationPropertyDefinition("MruFonts", typeof(string[]), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateMruFontsCallback)),
			new UserConfigurationPropertyDefinition("primarynavigationcollapsed", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidatePrimaryNavigationCollapsedCallback)),
			new UserConfigurationPropertyDefinition("themeStorageId", typeof(string), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateThemeStorageIdCallback)),
			new UserConfigurationPropertyDefinition("MailFindBarOn", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateFindBarOnCallback)),
			new UserConfigurationPropertyDefinition("CalendarFindBarOn", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateFindBarOnCallback)),
			new UserConfigurationPropertyDefinition("ContactsFindBarOn", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateFindBarOnCallback)),
			new UserConfigurationPropertyDefinition("SearchScope", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateSearchScopeCallback)),
			new UserConfigurationPropertyDefinition("ContactsSearchScope", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateContactsSearchScopeCallback)),
			new UserConfigurationPropertyDefinition("TasksSearchScope", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateTasksSearchScopeCallback)),
			new UserConfigurationPropertyDefinition("IsOptimizedForAccessibility", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateIsOptimizedForAccessibilityCallback)),
			new UserConfigurationPropertyDefinition("NewEnabledPonts", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateEnabledPontsCallback)),
			new UserConfigurationPropertyDefinition("FlagAction", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateFlagActionCallback)),
			new UserConfigurationPropertyDefinition("AddRecipientsToAutoCompleteCache", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateAddRecipientsToAutoCompleteCacheCallBack)),
			new UserConfigurationPropertyDefinition("ManuallyPickCertificate", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateManuallyPickCertificateCallback)),
			new UserConfigurationPropertyDefinition("SigningCertificateSubject", typeof(string), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateSigningCertificateSubjectCallback)),
			new UserConfigurationPropertyDefinition("SigningCertificateId", typeof(string), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateSigningCertificateIdCallback)),
			new UserConfigurationPropertyDefinition("UseDataCenterCustomTheme", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateUseDataCenterCustomThemeCallback)),
			new UserConfigurationPropertyDefinition("ConversationSortOrder", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateConversationSortOrderCallback)),
			new UserConfigurationPropertyDefinition("ShowTreeInListView", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateShowTreeInListViewCallback)),
			new UserConfigurationPropertyDefinition("HideDeletedItems", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateHideDeletedItemsCallback)),
			new UserConfigurationPropertyDefinition("HideMailTipsByDefault", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateHideMailTipsByDefaultCallback)),
			new UserConfigurationPropertyDefinition("sendaddressdefault", typeof(string), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateSendAddressDefaultCallback)),
			new UserConfigurationPropertyDefinition("EmailComposeMode", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateEmailComposeModeCallback)),
			new UserConfigurationPropertyDefinition("SendAsMruAddresses", typeof(string[]), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateSendAsMruAddressesCallback)),
			new UserConfigurationPropertyDefinition("CheckForForgottenAttachments", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateCheckForForgottenAttachmentsCallback)),
			new UserConfigurationPropertyDefinition("ShowInferenceUiElements", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateShowInferenceUiElementsCallback)),
			new UserConfigurationPropertyDefinition("HasShownClutterBarIntroductionMouse", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateHasShownClutterBarIntroductionMouseCallback)),
			new UserConfigurationPropertyDefinition("HasShownClutterDeleteAllIntroductionMouse", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateHasShownClutterDeleteAllIntroductionMouseCallback)),
			new UserConfigurationPropertyDefinition("HasShownClutterBarIntroductionTNarrow", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateHasShownClutterBarIntroductionTNarrowCallback)),
			new UserConfigurationPropertyDefinition("HasShownClutterDeleteAllIntroductionTNarrow", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateHasShownClutterDeleteAllIntroductionTNarrowCallback)),
			new UserConfigurationPropertyDefinition("HasShownClutterBarIntroductionTWide", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateHasShownClutterBarIntroductionTWideCallback)),
			new UserConfigurationPropertyDefinition("HasShownClutterDeleteAllIntroductionTWide", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateHasShownClutterDeleteAllIntroductionTWideCallback)),
			new UserConfigurationPropertyDefinition("ShowSenderOnTopInListView", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateShowSenderOnTopInListViewCallback)),
			new UserConfigurationPropertyDefinition("ShowPreviewTextInListView", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateShowPreviewTextInListViewCallback)),
			new UserConfigurationPropertyDefinition("GlobalReadingPanePosition", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateGlobalReadingPanePosition)),
			new UserConfigurationPropertyDefinition("UserOptionsMigrationState", typeof(UserOptionsMigrationState), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateUserOptionsMigrationState)),
			new UserConfigurationPropertyDefinition("IsInferenceSurveyComplete", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateIsInferenceSurveyCompleteCallback)),
			new UserConfigurationPropertyDefinition("InferenceSurveyDate", typeof(string), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateInferenceSurveyDateCallback)),
			new UserConfigurationPropertyDefinition("PeopleIKnowFirstUseDate", typeof(string), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidatePeopleIKnowFirstUseDateCallback)),
			new UserConfigurationPropertyDefinition("PeopleIKnowLastUseDate", typeof(string), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidatePeopleIKnowLastUseDateCallback)),
			new UserConfigurationPropertyDefinition("PeopleIKnowUse", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidatePeopleIKnowUseCallback)),
			new UserConfigurationPropertyDefinition("ActiveSurvey", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateActiveSurveyCallback)),
			new UserConfigurationPropertyDefinition("CompletedSurveys", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateCompletedSurveysCallback)),
			new UserConfigurationPropertyDefinition("DismissedSurveys", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateDismissedSurveysCallback)),
			new UserConfigurationPropertyDefinition("LastSurveyDate", typeof(string), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateLastSurveyDateCallback)),
			new UserConfigurationPropertyDefinition("DontShowSurveys", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateDontShowSurveysCallback)),
			new UserConfigurationPropertyDefinition("ReportJunkSelected", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateReportJunkSelectedCallback)),
			new UserConfigurationPropertyDefinition("CheckForReportJunkDialog", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateCheckForReportJunkDialogCallback)),
			new UserConfigurationPropertyDefinition("HasShownIntroductionForPeopleCentricTriage", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateHasShownIntroductionForPeopleCentricTriageCallback)),
			new UserConfigurationPropertyDefinition("HasShownIntroductionForModernGroups", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateHasShownIntroductionForModernGroupsCallback)),
			new UserConfigurationPropertyDefinition("ModernGroupsFirstUseDate", typeof(string), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateModernGroupsFirstUseDateCallback)),
			new UserConfigurationPropertyDefinition("ModernGroupsLastUseDate", typeof(string), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateModernGroupsLastUseDateCallback)),
			new UserConfigurationPropertyDefinition("ModernGroupsUseCount", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateModernGroupsUseCountCallback)),
			new UserConfigurationPropertyDefinition("BuildGreenLightSurveyLastShownDate", typeof(string), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateBuildGreenLightSurveyLastShownDateCallback)),
			new UserConfigurationPropertyDefinition("HasShownPeopleIKnow", typeof(bool), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateHasShownPeopleIKnowCallback)),
			new UserConfigurationPropertyDefinition("LearnabilityTypesShown", typeof(UserOptionsLearnabilityTypes), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateLearnabilityTypesShownCallback)),
			new UserConfigurationPropertyDefinition("NavigationPaneViewOption", typeof(NavigationPaneView), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateNavigationPaneViewOptionCallback)),
			new UserConfigurationPropertyDefinition("CalendarSearchUseCount", typeof(int), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateCalendarSearchUseCountCallback)),
			new UserConfigurationPropertyDefinition("FrequentlyUsedFolders", typeof(string[]), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateFrequentlyUsedFoldersCallback)),
			new UserConfigurationPropertyDefinition("ArchiveFolderId", typeof(string), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateArchiveFolderIdCallback)),
			new UserConfigurationPropertyDefinition("DefaultAttachmentsUploadFolderId", typeof(string), new UserConfigurationPropertyDefinition.Validate(UserConfigurationPropertyValidationUtility.ValidateDefaultAttachmentsUploadFolderIdCallback))
		};

		private static UserOptionPropertySchema instance;
	}
}
