using System;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class UserOptionPropertySchema
	{
		private UserOptionPropertySchema()
		{
		}

		internal static UserOptionPropertyDefinition GetPropertyDefinition(UserOptionPropertySchema.UserOptionPropertyID id)
		{
			return UserOptionPropertySchema.GetPropertyDefinition((int)id);
		}

		internal static UserOptionPropertyDefinition GetPropertyDefinition(int index)
		{
			ExTraceGlobals.UserOptionsDataTracer.TraceDebug<int, string>(0L, "Get UserOptionPropertyDefinition: index = '{0}', name = '{1}'", index, UserOptionPropertySchema.propertyDefinitions[index].PropertyName);
			return UserOptionPropertySchema.propertyDefinitions[index];
		}

		internal static int Count
		{
			get
			{
				return UserOptionPropertySchema.propertyDefinitions.Length;
			}
		}

		private static readonly UserOptionPropertyDefinition[] propertyDefinitions = new UserOptionPropertyDefinition[]
		{
			new UserOptionPropertyDefinition("timezone", typeof(string), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateTimeZoneCallback)),
			new UserOptionPropertyDefinition("timeformat", typeof(string), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateTimeFormatCallback)),
			new UserOptionPropertyDefinition("dateformat", typeof(string), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateDateFormatCallback)),
			new UserOptionPropertyDefinition("weekstartday", typeof(int), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateWeekStartDayCallback)),
			new UserOptionPropertyDefinition("hourincrement", typeof(int), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateHourIncrementCallback)),
			new UserOptionPropertyDefinition("showweeknumbers", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateShowWeekNumbersCallback)),
			new UserOptionPropertyDefinition("checknameincontactsfirst", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateCheckNameInContactsFirstCallback)),
			new UserOptionPropertyDefinition("firstweekofyear", typeof(int), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateFirstWeekOfYearCallback)),
			new UserOptionPropertyDefinition("enablereminders", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateEnableRemindersCallback)),
			new UserOptionPropertyDefinition("enableremindersound", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateEnableReminderSoundCallback)),
			new UserOptionPropertyDefinition("newitemnotify", typeof(int), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateNewItemNotifyCallback)),
			new UserOptionPropertyDefinition("viewrowcount", typeof(int), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateViewRowCountCallback)),
			new UserOptionPropertyDefinition("basicviewrowcount", typeof(int), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateBasicViewRowCountCallback)),
			new UserOptionPropertyDefinition("spellingdictionarylanguage", typeof(int), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateSpellingDictionaryLanguageCallback)),
			new UserOptionPropertyDefinition("spellingignoreuppercase", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateSpellingIgnoreUppercaseCallback)),
			new UserOptionPropertyDefinition("spellingignoremixeddigits", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateSpellingIgnoreMixedDigitsCallback)),
			new UserOptionPropertyDefinition("spellingcheckbeforesend", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateSpellingCheckBeforeSendCallback)),
			new UserOptionPropertyDefinition("smimeencrypt", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateSmimeEncryptCallback)),
			new UserOptionPropertyDefinition("smimesign", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateSmimeSignCallback)),
			new UserOptionPropertyDefinition("alwaysshowbcc", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateAlwaysShowBccCallback)),
			new UserOptionPropertyDefinition("alwaysshowfrom", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateAlwaysShowFromCallback)),
			new UserOptionPropertyDefinition("composemarkup", typeof(int), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateComposeMarkupCallback)),
			new UserOptionPropertyDefinition("composefontname", typeof(string), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateComposeFontNameCallback)),
			new UserOptionPropertyDefinition("composefontsize", typeof(int), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateComposeFontSizeCallback)),
			new UserOptionPropertyDefinition("composefontcolor", typeof(string), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateComposeFontColorCallback)),
			new UserOptionPropertyDefinition("composefontflags", typeof(int), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateComposeFontFlagsCallback)),
			new UserOptionPropertyDefinition("autoaddsignature", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateAutoAddSignatureCallback)),
			new UserOptionPropertyDefinition("signaturetext", typeof(string), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateSignatureTextCallback)),
			new UserOptionPropertyDefinition("signaturehtml", typeof(string), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateSignatureHtmlCallback)),
			new UserOptionPropertyDefinition("blockexternalcontent", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateBlockExternalContentCallback)),
			new UserOptionPropertyDefinition("previewmarkasread", typeof(int), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidatePreviewMarkAsReadCallback)),
			new UserOptionPropertyDefinition("markasreaddelaytime", typeof(int), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateMarkAsReadDelaytimeCallback)),
			new UserOptionPropertyDefinition("nextselection", typeof(int), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateNextSelectionCallback)),
			new UserOptionPropertyDefinition("readreceipt", typeof(int), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateReadReceiptCallback)),
			new UserOptionPropertyDefinition("emptydeleteditemsonlogoff", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateEmptyDeletedItemsOnLogoffCallback)),
			new UserOptionPropertyDefinition("navigationbarwidth", typeof(int), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateNavigationBarWidthCallback)),
			new UserOptionPropertyDefinition("isminibarvisible", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateMiniBarCallback)),
			new UserOptionPropertyDefinition("isquicklinksbarvisible", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateQuickLinksCallback)),
			new UserOptionPropertyDefinition("istaskdetailsvisible", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateTaskDetailsCallback)),
			new UserOptionPropertyDefinition("isdocumentfavoritesvisible", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateDocumentFavoritesCallback)),
			new UserOptionPropertyDefinition("isoutlooksharedfoldersvisible", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateOutlookSharedFoldersCallback)),
			new UserOptionPropertyDefinition("formatbarstate", typeof(int), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateFormatBarStateCallback)),
			new UserOptionPropertyDefinition("mrufonts", typeof(string), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateMruFontsCallback)),
			new UserOptionPropertyDefinition("primarynavigationcollapsed", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidatePrimaryNavigationCollapsedCallback)),
			new UserOptionPropertyDefinition("themeStorageId", typeof(string), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateThemeStorageIdCallback)),
			new UserOptionPropertyDefinition("MailFindBarOn", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateFindBarOnCallback)),
			new UserOptionPropertyDefinition("CalendarFindBarOn", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateFindBarOnCallback)),
			new UserOptionPropertyDefinition("ContactsFindBarOn", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateFindBarOnCallback)),
			new UserOptionPropertyDefinition("SearchScope", typeof(int), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateSearchScopeCallback)),
			new UserOptionPropertyDefinition("ContactsSearchScope", typeof(int), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateContactsSearchScopeCallback)),
			new UserOptionPropertyDefinition("TasksSearchScope", typeof(int), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateTasksSearchScopeCallback)),
			new UserOptionPropertyDefinition("IsOptimizedForAccessibility", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateIsOptimizedForAccessibilityCallback)),
			new UserOptionPropertyDefinition("NewEnabledPonts", typeof(int), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateEnabledPontsCallback)),
			new UserOptionPropertyDefinition("FlagAction", typeof(int), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateFlagActionCallback)),
			new UserOptionPropertyDefinition("AddRecipientsToAutoCompleteCache", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateAddRecipientsToAutoCompleteCacheCallBack)),
			new UserOptionPropertyDefinition("ManuallyPickCertificate", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateManuallyPickCertificateCallback)),
			new UserOptionPropertyDefinition("SigningCertificateSubject", typeof(string), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateSigningCertificateSubjectCallback)),
			new UserOptionPropertyDefinition("SigningCertificateId", typeof(string), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateSigningCertificateIdCallback)),
			new UserOptionPropertyDefinition("UseDataCenterCustomTheme", typeof(int), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateUseDataCenterCustomThemeCallback)),
			new UserOptionPropertyDefinition("ConversationSortOrder", typeof(int), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateConversationSortOrderCallback)),
			new UserOptionPropertyDefinition("ShowTreeInListView", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateShowTreeInListViewCallback)),
			new UserOptionPropertyDefinition("HideDeletedItems", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateHideDeletedItemsCallback)),
			new UserOptionPropertyDefinition("HideMailTipsByDefault", typeof(bool), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateHideMailTipsByDefaultCallback)),
			new UserOptionPropertyDefinition("sendaddressdefault", typeof(string), new UserOptionPropertyDefinition.Validate(UserOptionPropertyValidationUtility.ValidateSendAddressDefaultCallback))
		};

		internal enum UserOptionPropertyID
		{
			TimeZone,
			TimeFormat,
			DateFormat,
			WeekStartDay,
			HourIncrement,
			ShowWeekNumbers,
			CheckNameInContactsFirst,
			FirstWeekOfYear,
			EnableReminders,
			EnableReminderSound,
			NewItemNotify,
			ViewRowCount,
			BasicViewRowCount,
			SpellingDictionaryLanguage,
			SpellingIgnoreUppercase,
			SpellingIgnoreMixedDigits,
			SpellingCheckBeforeSend,
			SmimeEncrypt,
			SmimeSign,
			AlwaysShowBcc,
			AlwaysShowFrom,
			ComposeMarkup,
			ComposeFontName,
			ComposeFontSize,
			ComposeFontColor,
			ComposeFontFlags,
			AutoAddSignature,
			SignatureText,
			SignatureHtml,
			BlockExternalContent,
			PreviewMarkAsRead,
			MarkAsReadDelaytime,
			NextSelection,
			ReadReceipt,
			EmptyDeletedItemsOnLogoff,
			NavigationBarWidth,
			IsMiniBarVisible,
			IsQuickLinksBarVisible,
			IsTaskDetailsVisible,
			IsDocumentFavoritesVisible,
			IsOutlookSharedFoldersVisible,
			FormatBarState,
			MruFonts,
			PrimaryNavigationCollapsed,
			ThemeStorageId,
			MailFindBarOn,
			CalendarFindBarOn,
			ContactsFindBarOn,
			SearchScope,
			ContactsSearchScope,
			TasksSearchScope,
			IsOptimizedForAccessibility,
			NewEnabledPonts,
			FlagAction,
			AddRecipientsToAutoCompleteCache,
			ManuallyPickCertificate,
			SigningCertificateSubject,
			SigningCertificateId,
			UseDataCenterCustomTheme,
			ConversationSortOrder,
			ShowTreeInListView,
			HideDeletedItems,
			HideMailTipsByDefault,
			SendAddressDefault
		}
	}
}
