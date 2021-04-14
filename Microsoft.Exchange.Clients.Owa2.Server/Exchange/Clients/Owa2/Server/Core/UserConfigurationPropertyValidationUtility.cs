using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public sealed class UserConfigurationPropertyValidationUtility
	{
		private UserConfigurationPropertyValidationUtility()
		{
		}

		internal static object ValidateViewRowCountCallbackInternal(object value, int defaultValue)
		{
			if (value != null)
			{
				try
				{
					int num = (int)value;
					int num2;
					if (UserConfigurationPropertyValidationUtility.ViewRowCountValues.IsConfigValueDefined)
					{
						num2 = ((1000 < UserConfigurationPropertyValidationUtility.ViewRowCountValues.MaxViewRowCountConfigValue) ? 1000 : UserConfigurationPropertyValidationUtility.ViewRowCountValues.MaxViewRowCountConfigValue);
					}
					else
					{
						num2 = 100;
					}
					if (num >= 5 && num <= num2 && ((num <= 50 && num % 5 == 0) || (num > 50 && num % 25 == 0)))
					{
						ExTraceGlobals.UserOptionsDataTracer.TraceDebug(0L, "Returning original value: {0}", new object[]
						{
							value
						});
						return value;
					}
				}
				catch (InvalidCastException)
				{
					ExTraceGlobals.UserOptionsTracer.TraceError(0L, "Failed to cast '{0}' to int type", new object[]
					{
						value
					});
				}
			}
			ExTraceGlobals.UserOptionsDataTracer.TraceDebug<int>(0L, "Returning default value: {0}", defaultValue);
			return defaultValue;
		}

		internal static object ValidateViewRowCountCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateViewRowCountCallbackInternal(value, 50);
		}

		internal static object ValidateBasicViewRowCountCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateViewRowCountCallbackInternal(value, 20);
		}

		internal static object ValidateNextSelectionCallback(object value)
		{
			return (NextSelectionDirection)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, 1, 0, 2);
		}

		internal static object ValidateTimeZoneCallback(object value)
		{
			string text = value as string;
			if (text != null)
			{
				ExTimeZone exTimeZone;
				bool flag = ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(text, out exTimeZone);
				if (flag)
				{
					ExTraceGlobals.UserOptionsDataTracer.TraceDebug(0L, "Returning original value: {0}", new object[]
					{
						value
					});
					return value;
				}
			}
			ExTraceGlobals.UserOptionsDataTracer.TraceDebug<string>(0L, "Returning default value: {0}", UserConfigurationPropertyValidationUtility.DefaultTimeZone);
			return UserConfigurationPropertyValidationUtility.DefaultTimeZone;
		}

		internal static object ValidateTimeFormatCallback(object value)
		{
			string text;
			if (!MailboxRegionalConfiguration.ValidateTimeFormat(CultureInfo.CurrentUICulture, value, out text))
			{
				ExTraceGlobals.UserOptionsDataTracer.TraceDebug<string, object>(0L, "Returning default TimeFormat value: {0}. OriginalFormat: '{1}'", text, (value == null) ? "isnull" : ((value is string) ? value : "is not a string"));
				return text;
			}
			ExTraceGlobals.UserOptionsDataTracer.TraceDebug(0L, "Returning user specified TimeFormat value: {0}", new object[]
			{
				value
			});
			return value;
		}

		internal static object ValidateDateFormatCallback(object value)
		{
			string text;
			if (!MailboxRegionalConfiguration.ValidateDateFormat(CultureInfo.CurrentUICulture, value, out text))
			{
				ExTraceGlobals.UserOptionsDataTracer.TraceDebug<string, object>(0L, "Returning default DateFormat value: {0}. OriginalFormat: '{1}'", text, (value == null) ? "isnull" : ((value is string) ? value : "is not a string"));
				return text;
			}
			ExTraceGlobals.UserOptionsDataTracer.TraceDebug(0L, "Returning user specified DateFormat value: {0}", new object[]
			{
				value
			});
			return value;
		}

		internal static object ValidateWeekStartDayCallback(object value)
		{
			return (System.DayOfWeek)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, (int)DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek, 0, 6);
		}

		internal static object ValidateHourIncrementCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateIntCollection(value, 30, UserConfigurationPropertyValidationUtility.ValidHourIncrementOptions);
		}

		internal static object ValidateShowWeekNumbersCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateCheckNameInContactsFirstCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateFirstWeekOfYearCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateIntRange(value, DateTimeFormatInfo.CurrentInfo.CalendarWeekRule.ToFirstWeekRules(), 0, 3);
		}

		internal static object ValidateEnableRemindersCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, true);
		}

		internal static object ValidateEnableReminderSoundCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, true);
		}

		internal static object ValidateNewItemNotifyCallback(object value)
		{
			return (NewNotification)UserConfigurationPropertyValidationUtility.ValidateIntCollection(value, 15, UserConfigurationPropertyValidationUtility.ValidNewItemNotifyCollection);
		}

		internal static object ValidateSpellingDictionaryLanguageCallback(object value)
		{
			if (value is int)
			{
				return value;
			}
			return -1;
		}

		internal static object ValidateSpellingIgnoreUppercaseCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateSpellingIgnoreMixedDigitsCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateSpellingCheckBeforeSendCallback(object value)
		{
			bool flag = UserConfigurationPropertyValidationUtility.SpellingCheckBeforeSendValues.IsCofigValueDefine && UserConfigurationPropertyValidationUtility.SpellingCheckBeforeSendValues.SpellingCheckBeforeSendConfigValue;
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, flag);
		}

		internal static object ValidateSmimeEncryptCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateSmimeSignCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateAlwaysShowBccCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateAlwaysShowFromCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateComposeMarkupCallback(object value)
		{
			return (Markup)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, 0, 0, 1);
		}

		internal static object ValidateComposeFontNameCallback(object value)
		{
			int num = 100;
			string text = (string)value;
			if (!string.IsNullOrEmpty(text) && text.Length <= num)
			{
				ExTraceGlobals.UserOptionsDataTracer.TraceDebug<string>(0L, "Returning value: {0}", text);
				return value;
			}
			return null;
		}

		internal static object ValidateComposeFontSizeCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateIntRange(value, 3, 1, 7);
		}

		internal static object ValidateComposeFontColorCallback(object value)
		{
			string text = (string)value;
			if (!string.IsNullOrEmpty(text) && UserConfigurationPropertyValidationUtility.validColorRegex.Match(text).Success)
			{
				ExTraceGlobals.UserOptionsDataTracer.TraceDebug(0L, "Returning original value: {0}", new object[]
				{
					value
				});
				return value;
			}
			ExTraceGlobals.UserOptionsDataTracer.TraceDebug<string>(0L, "Returning default value: {0}", "#000000");
			return "#000000";
		}

		internal static object ValidateComposeFontFlagsCallback(object value)
		{
			return (FontFlags)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, 0, 0, 7);
		}

		internal static object ValidateAutoAddSignatureCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateSignatureTextCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateSignatureTextCallbackCommon(value, UserConfigurationPropertyValidationUtility.SignatureValues.SignatureMaxLengthConfigValue, 16000, 4096);
		}

		internal static object ValidateAutoAddSignatureOnMobileCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, true);
		}

		internal static object ValidateSignatureTextOnMobileCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateSignatureTextCallbackCommon(value, UserConfigurationPropertyValidationUtility.MOWASignatureValues.SignatureMaxLengthConfigValue, 512, 512);
		}

		internal static object ValidateUseDesktopSignatureCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, true);
		}

		internal static object ValidateSignatureTextCallbackCommon(object value, int signatureMaxLengthConfigValue, int allowedSignatureMaxLength, int defaultSignatureMaxLength)
		{
			string text = (string)value;
			if (string.IsNullOrEmpty(text))
			{
				return value;
			}
			int num;
			if (UserConfigurationPropertyValidationUtility.SignatureValues.IsConfigValueDefined)
			{
				num = ((signatureMaxLengthConfigValue < allowedSignatureMaxLength) ? signatureMaxLengthConfigValue : allowedSignatureMaxLength);
			}
			else
			{
				num = defaultSignatureMaxLength;
			}
			if (text.Length <= num)
			{
				ExTraceGlobals.UserOptionsDataTracer.TraceDebug(0L, "Returning original value: {0}", new object[]
				{
					value
				});
				return value;
			}
			string text2 = text.Substring(0, num);
			ExTraceGlobals.UserOptionsDataTracer.TraceDebug<int, string>(0L, "Signature is longer that max length '{0}'. Returning truncated value: {1}", num, text2);
			return text2;
		}

		internal static object ValidateSignatureHtmlCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateSignatureTextCallback(value);
		}

		internal static object ValidateBlockExternalContentCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, true);
		}

		internal static object ValidatePreviewMarkAsReadCallback(object value)
		{
			return (MarkAsRead)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, 1, 0, 2);
		}

		internal static object ValidateEmailComposeModeCallback(object value)
		{
			return (EmailComposeMode)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, 0, 0, 1);
		}

		internal static object ValidateSendAsMruAddressesCallback(object value)
		{
			string[] array = value as string[];
			if (array != null && array.Length <= 10)
			{
				return array;
			}
			return null;
		}

		internal static object ValidateCheckForForgottenAttachmentsCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, true);
		}

		internal static object ValidateMarkAsReadDelaytimeCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateIntRange(value, 5, 0, 30);
		}

		internal static object ValidateReadReceiptCallback(object value)
		{
			return (ReadReceiptResponse)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, 0, 0, 2);
		}

		internal static object ValidateEmptyDeletedItemsOnLogoffCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateNavigationBarWidthCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateIntRange(value, 214, 50, 2000);
		}

		internal static object ValidateNavigationBarWidthRatioCallback(object value)
		{
			if (!(value is string))
			{
				return null;
			}
			return value;
		}

		internal static object ValidateMailFolderPaneExpandedCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, true);
		}

		internal static object ValidateIsFavoritesFolderTreeCollapsedCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateIsPeopleIKnowFolderTreeCollapsedCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateShowReadingPaneOnFirstLoadCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateIsMailRootFolderTreeCollapsedCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateMiniBarCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateQuickLinksCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateTaskDetailsCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, true);
		}

		internal static object ValidateDocumentFavoritesCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, true);
		}

		internal static object ValidateOutlookSharedFoldersCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, true);
		}

		internal static object ValidateFormatBarStateCallback(object value)
		{
			return (FormatBarButtonGroups)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, FormatBarButtonGroups.BoldItalicUnderline | FormatBarButtonGroups.Lists | FormatBarButtonGroups.ForegroundColor | FormatBarButtonGroups.BackgroundColor | FormatBarButtonGroups.Customize, 0, 16383);
		}

		internal static object ValidateMruFontsCallback(object value)
		{
			int num = 100;
			string[] array = (string[])value;
			if (array == null || array.Length > num)
			{
				ExTraceGlobals.UserOptionsDataTracer.TraceDebug<string[]>(0L, "Returning default value: {0}", UserConfigurationPropertyValidationUtility.DefaultMruFonts);
				return UserConfigurationPropertyValidationUtility.DefaultMruFonts;
			}
			return array;
		}

		internal static object ValidatePrimaryNavigationCollapsedCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateThemeStorageIdCallback(object value)
		{
			string text = value as string;
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return string.Empty;
		}

		internal static object ValidateFindBarOnCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, true);
		}

		internal static object ValidateSearchScopeCallback(object value)
		{
			return (SearchScope)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, 3, 0, 3);
		}

		internal static object ValidateContactsSearchScopeCallback(object value)
		{
			return (SearchScope)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, 0, 0, 2);
		}

		internal static object ValidateTasksSearchScopeCallback(object value)
		{
			return (SearchScope)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, 0, 0, 2);
		}

		internal static object ValidateIsOptimizedForAccessibilityCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateEnabledPontsCallback(object value)
		{
			return (PontType)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, int.MaxValue, 0, int.MaxValue);
		}

		internal static object ValidateFlagActionCallback(object value)
		{
			return (FlagAction)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, 2, 2, 6);
		}

		internal static object ValidateAddRecipientsToAutoCompleteCacheCallBack(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, true);
		}

		internal static object ValidateManuallyPickCertificateCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateSigningCertificateSubjectCallback(object value)
		{
			return value;
		}

		internal static object ValidateSigningCertificateIdCallback(object value)
		{
			return value;
		}

		internal static object ValidateUseDataCenterCustomThemeCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateIntCollection(value, -1, new int[]
			{
				0,
				1
			});
		}

		internal static object ValidateConversationSortOrderCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateIntCollection(value, 5, UserConfigurationPropertyValidationUtility.ValidConversationSortOrderValues);
		}

		internal static object ValidateShowTreeInListViewCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateHideDeletedItemsCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateHideMailTipsByDefaultCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateSendAddressDefaultCallback(object value)
		{
			return value;
		}

		internal static object ValidateCalendarViewType(object value)
		{
			return (int)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, CalendarViewType.Monthly, 1, 5);
		}

		internal static object ValidateCalendarViewTypeNarrow(object value)
		{
			return (int)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, CalendarViewType.Daily, 1, 5);
		}

		internal static object ValidateCalendarSidePanelMonthPickerCount(object value)
		{
			return (int)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, 1, 1, 20);
		}

		internal static object ValidateUserOptionsMigrationState(object value)
		{
			return (int)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, 0, 0, int.MaxValue);
		}

		internal static object ValidateCalendarSidePanelIsExpanded(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, true);
		}

		internal static object ValidateShowInferenceUiElementsCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, true);
		}

		internal static object ValidateIsClutterUIEnabledCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, UserConfigurationPropertyValidationUtility.DefaultIsClutterUIEnabled);
		}

		internal static object ValidateDefaultAttachmentsUploadFolderIdCallback(object value)
		{
			string text = (string)value;
			if (!string.IsNullOrEmpty(text) && text.Length <= 100)
			{
				return value;
			}
			return null;
		}

		internal static object ValidateHasShownClutterBarIntroductionMouseCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateHasShownClutterDeleteAllIntroductionMouseCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateHasShownClutterBarIntroductionTNarrowCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateHasShownClutterDeleteAllIntroductionTNarrowCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateHasShownClutterBarIntroductionTWideCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateHasShownClutterDeleteAllIntroductionTWideCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateIsInferenceSurveyCompleteCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateDontShowSurveysCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateActiveSurveyCallback(object value)
		{
			return (int)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, 0, 0, int.MaxValue);
		}

		internal static object ValidateCompletedSurveysCallback(object value)
		{
			return (int)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, 0, 0, int.MaxValue);
		}

		internal static object ValidateDismissedSurveysCallback(object value)
		{
			return (int)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, 0, 0, int.MaxValue);
		}

		internal static object ValidateLastSurveyDateCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateDateTimeString(value, UserConfigurationPropertyValidationUtility.DefaultLastSurveyDate);
		}

		internal static object ValidateInferenceSurveyDateCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateDateTimeString(value, UserConfigurationPropertyValidationUtility.DefaultSurveyDate);
		}

		internal static object ValidatePeopleIKnowFirstUseDateCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateDateTimeString(value, UserConfigurationPropertyValidationUtility.DefaultSurveyDate);
		}

		internal static object ValidatePeopleIKnowLastUseDateCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateDateTimeString(value, UserConfigurationPropertyValidationUtility.DefaultSurveyDate);
		}

		internal static object ValidatePeopleIKnowUseCallback(object value)
		{
			return (int)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, 0, 0, int.MaxValue);
		}

		internal static object ValidateModernGroupsFirstUseDateCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateDateTimeString(value, UserConfigurationPropertyValidationUtility.DefaultSurveyDate);
		}

		internal static object ValidateModernGroupsLastUseDateCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateDateTimeString(value, UserConfigurationPropertyValidationUtility.DefaultSurveyDate);
		}

		internal static object ValidateModernGroupsUseCountCallback(object value)
		{
			return (int)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, 0, 0, int.MaxValue);
		}

		internal static object ValidateShowSenderOnTopInListViewCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, true);
		}

		internal static object ValidateShowPreviewTextInListViewCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, true);
		}

		internal static object ValidateGlobalReadingPanePosition(object value)
		{
			return (int)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, MailReadingPanePosition.Right, 0, 2);
		}

		internal static object ValidateSchedulingViewType(object value)
		{
			return (int)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, CalendarViewType.Daily, 1, 2);
		}

		internal static object ValidatePeopleHubDisplayOptionType(object value)
		{
			return (int)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, PeopleDisplayOptionsType.Uninitialized, 1, 2);
		}

		internal static object ValidatePeopleHubSortOptionType(object value)
		{
			return (int)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, PeopleSortOptionsType.Uninitialized, 1, 6);
		}

		internal static object ValidateAttachmentsFilePickerViewType(object value)
		{
			return (int)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, AttachmentsFilePickerViewType.None, 0, 2);
		}

		internal static object ValidateAttachmentsFilePickerHideBanner(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateCurrentWeatherLocationBookmarkIndex(object value)
		{
			return (int)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, CurrentWeatherLocationBookmarkIndexOption.None, -1, 4);
		}

		internal static object ValidateTemperatureUnit(object value)
		{
			return (int)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, TemperatureUnit.Default, 0, 2);
		}

		internal static object ValidateHasShownIntroductionForPeopleCentricTriageCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateHasShownIntroductionForModernGroupsCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateLearnabilityTypesShownCallback(object value)
		{
			return (int)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, 0, 0, int.MaxValue);
		}

		internal static object ValidateNavigationPaneViewOptionCallback(object value)
		{
			return (int)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, NavigationPaneView.Default, 0, 4);
		}

		internal static object ValidateHasShownPeopleIKnowCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateCalendarSearchUseCountCallback(object value)
		{
			return (int)UserConfigurationPropertyValidationUtility.ValidateIntRange(value, 0, 0, int.MaxValue);
		}

		internal static object ValidateFrequentlyUsedFoldersCallback(object value)
		{
			string[] array = value as string[];
			if (array == null)
			{
				return null;
			}
			if (array.Length <= 10)
			{
				return array;
			}
			ExTraceGlobals.UserOptionsDataTracer.TraceDebug<int>(0L, "FrequentlyUsedFolders length exceends maximum size.  length: {0}", array.Length);
			string[] array2 = new string[10];
			for (int i = 0; i < 10; i++)
			{
				array2[i] = array[i];
			}
			return array2;
		}

		internal static object ValidateCalendarAgendaViewIsExpandedMouse(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, true);
		}

		internal static object ValidateCalendarAgendaViewIsExpandedTWide(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, true);
		}

		internal static object ValidateArchiveFolderIdCallback(object value)
		{
			string text = value as string;
			if (!string.IsNullOrEmpty(text))
			{
				try
				{
					if (Folder.IsFolderId(StoreId.EwsIdToFolderStoreObjectId(text)))
					{
						return value;
					}
				}
				catch (InvalidIdMalformedException)
				{
					ExTraceGlobals.UserOptionsTracer.TraceDebug(0L, "Invalid archive folder id: '{0}'", new object[]
					{
						value
					});
				}
			}
			return null;
		}

		private static object ValidateIntCollection(object value, object defaultValue, int[] validValues)
		{
			if (value != null && validValues != null)
			{
				try
				{
					int num = (int)value;
					for (int i = 0; i < validValues.Length; i++)
					{
						if (num == validValues[i])
						{
							ExTraceGlobals.UserOptionsDataTracer.TraceDebug(0L, "Returning original value: {0}", new object[]
							{
								value
							});
							return value;
						}
					}
				}
				catch (InvalidCastException)
				{
					ExTraceGlobals.UserOptionsTracer.TraceDebug(0L, "Failed to cast '{0}' to int type", new object[]
					{
						value
					});
				}
			}
			ExTraceGlobals.UserOptionsDataTracer.TraceDebug(0L, "Returning default value: {0}", new object[]
			{
				defaultValue
			});
			return defaultValue;
		}

		private static object ValidateIntRange(object value, object defaultValue, int minValidValue, int maxValidValue)
		{
			if (value != null)
			{
				try
				{
					int num = (int)value;
					if (num <= maxValidValue && num >= minValidValue)
					{
						ExTraceGlobals.UserOptionsDataTracer.TraceDebug(0L, "Returning original value: {0}", new object[]
						{
							value
						});
						return value;
					}
				}
				catch (InvalidCastException)
				{
					ExTraceGlobals.UserOptionsTracer.TraceDebug(0L, "Failed to cast '{0}' to int type", new object[]
					{
						value
					});
				}
			}
			ExTraceGlobals.UserOptionsDataTracer.TraceDebug(0L, "Returning default value: {0}", new object[]
			{
				defaultValue
			});
			return defaultValue;
		}

		private static object ValidateBoolValue(object value, object defaultValue)
		{
			if (value != null)
			{
				try
				{
					bool flag = (bool)value;
					ExTraceGlobals.UserOptionsDataTracer.TraceDebug(0L, "Returning original value: {0}", new object[]
					{
						value
					});
					return value;
				}
				catch (InvalidCastException)
				{
					ExTraceGlobals.UserOptionsTracer.TraceDebug(0L, "Failed to cast '{0}' to bool type", new object[]
					{
						value
					});
				}
			}
			ExTraceGlobals.UserOptionsDataTracer.TraceDebug(0L, "Returning default value: {0}", new object[]
			{
				defaultValue
			});
			return defaultValue;
		}

		private static object ValidateDateTimeString(object value, string defaultDateTimeString)
		{
			string text = value as string;
			if (!string.IsNullOrEmpty(text))
			{
				ExDateTime exDateTime;
				if (ExDateTime.TryParse(text, out exDateTime))
				{
					ExTraceGlobals.UserOptionsDataTracer.TraceDebug(0L, "Returning original value: '{0}'", new object[]
					{
						value
					});
					return text;
				}
				ExTraceGlobals.UserOptionsTracer.TraceDebug(0L, "'{0}' is not a valid DateTime string.", new object[]
				{
					value
				});
			}
			ExTraceGlobals.UserOptionsDataTracer.TraceDebug<string>(0L, "Returning default value: '{0}'", defaultDateTimeString);
			return defaultDateTimeString;
		}

		internal static object ValidateReportJunkSelectedCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateCheckForReportJunkDialogCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateBuildGreenLightSurveyLastShownDateCallback(object value)
		{
			return UserConfigurationPropertyValidationUtility.ValidateDateTimeString(value, UserConfigurationPropertyValidationUtility.DefaultSurveyDate);
		}

		private const int MaxSendAsMruAddressCount = 10;

		private const int DefaultViewRowCount = 50;

		private const int DefaultBasicViewRowCount = 20;

		private const NextSelectionDirection DefaultNextSelection = NextSelectionDirection.Next;

		private const int MaxFrequentlyUsedFoldersCount = 10;

		private const int DefaultHourIncrement = 30;

		private const bool DefaultShowWeekNumbers = false;

		private const bool DefaultCheckNameInContactsFirst = false;

		private const bool DefaultEnableReminders = true;

		private const bool DefaultEnableReminderSound = true;

		private const NewNotification DefaultNewItemNotify = NewNotification.Sound | NewNotification.EMailToast | NewNotification.VoiceMailToast | NewNotification.FaxToast;

		private const int DefaultMarkAsReadDelaytime = 5;

		private const int DefaultNavBarWidth = 214;

		private const bool DefaultBlockExternalContent = true;

		private const FormatBarButtonGroups DefaultFormatBarState = FormatBarButtonGroups.BoldItalicUnderline | FormatBarButtonGroups.Lists | FormatBarButtonGroups.ForegroundColor | FormatBarButtonGroups.BackgroundColor | FormatBarButtonGroups.Customize;

		private const SearchScope DefaultSearchScope = SearchScope.AllFoldersAndItems;

		private const bool DefaultIsOptimizedForAccessibility = false;

		private const bool DefaultManuallyPickCertificate = false;

		private const bool DefaultShowTreeInListView = false;

		private const bool DefaultHideDeletedItems = false;

		private const bool DefaultHideMailTipsByDefault = false;

		private const ConversationSortOrder DefaultConversationSortOrder = ConversationSortOrder.ChronologicalNewestOnTop;

		private const int DefaultUseDataCenterCustomTheme = -1;

		private const bool DefaultAddRecipientsToAutoCompleteCache = true;

		private const FlagAction DefaultFlagAction = FlagAction.Today;

		private const PontType DefaultEnabledPonts = PontType.All;

		private const SearchScope DefaultTasksSearchScope = SearchScope.SelectedFolder;

		private const SearchScope DefaultContactsSearchScope = SearchScope.SelectedFolder;

		private const bool DefaultFindBarOn = true;

		private const bool PrimaryNavigationCollapsed = false;

		private const bool DefaultOutlookSharedFoldersVisible = true;

		private const bool DefaultDocumentFavoritesVisible = true;

		private const bool DefaultTaskDetailsVisible = true;

		private const bool DefaultMiniBarVisible = false;

		private const bool DefaultEmptyDeletedItemsOnLogoff = false;

		private const ReadReceiptResponse DefaultReadReceipt = ReadReceiptResponse.DoNotAutomaticallySend;

		private const bool DefaultQuickLinksVisible = false;

		private const MarkAsRead DefaultPreviewMarkAsRead = MarkAsRead.OnSelectionChange;

		private const EmailComposeMode DefaultEmailComposeMode = EmailComposeMode.Inline;

		private const bool DefaultCheckForForgottenAttachments = true;

		private const int DefaultComposeFontSize = 3;

		private const int DefaultMonthPickerCount = 1;

		private const int MinimumMonthPickerCount = 1;

		private const int MaximumMonthPickerCount = 20;

		private const string DefaultComposeFontColor = "#000000";

		private const bool DefaultSpellingIgnoreMixedDigits = false;

		private const bool DefaultSpellingIgnoreUppercase = false;

		private const bool DefaultSpellingCheckBeforeSend = false;

		private const bool DefaultSmimeEncrypt = false;

		private const bool DefaultSmimeSign = false;

		private const bool DefaultAlwaysShowBcc = false;

		private const bool DefaultAlwaysShowFrom = false;

		private const Markup DefaultComposeMarkup = Markup.Html;

		private const FontFlags DefaultComposeFontFlags = FontFlags.Normal;

		private const bool DefaultAutoAddSignature = false;

		private const int DefaultSignatureMaxLength = 4096;

		private const bool DefaultAutoAddSignatureOnMobile = true;

		private const int DefaultSignatureOnMobileMaxLength = 512;

		private const bool DefaultUseDesktopSignature = true;

		private const bool DefaultShowInferenceUiElements = true;

		private const bool DefaultHasShownClutterBarIntroductionMouse = false;

		private const bool DefaultHasShownClutterDeleteAllIntroductionMouse = false;

		private const bool DefaultHasShownClutterBarIntroductionTNarrow = false;

		private const bool DefaultHasShownClutterDeleteAllIntroductionTNarrow = false;

		private const bool DefaultHasShownClutterBarIntroductionTWide = false;

		private const bool DefaultHasShownClutterDeleteAllIntroductionTWide = false;

		private const bool DefaultIsInferenceSurveyComplete = false;

		private const int DefaultActiveSurvey = 0;

		private const int DefaultCompletedSurveys = 0;

		private const int DefaultDismissedSurveys = 0;

		private const bool DefaultDontShowSurveys = false;

		private const bool DefaultShowSenderOnTopInListView = true;

		private const bool DefaultShowPreviewTextInListView = true;

		private const bool DefaultReportJunkSelected = false;

		private const bool DefaultCheckForReportJunkDialog = false;

		private const bool DefaultHasShownIntroductionForPeopleCentricTriage = false;

		private const bool DefaultHasShownIntroductionForModernGroups = false;

		private const bool DefaultHasShownPeopleIKnow = false;

		private const bool DefaultAttachmentsFilePickerHideBanner = false;

		private static readonly int[] ValidHourIncrementOptions = new int[]
		{
			15,
			30
		};

		private static readonly string[] DefaultMruFonts = new string[0];

		private static readonly string DefaultTimeZone = string.Empty;

		private static readonly int[] ValidConversationSortOrderValues = new int[]
		{
			5,
			9
		};

		private static readonly Regex validColorRegex = new Regex("^\\#[0-9a-fA-F]{6}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly bool? DefaultIsClutterUIEnabled = null;

		private static readonly string DefaultLastSurveyDate = null;

		private static readonly string DefaultSurveyDate = null;

		private static readonly int[] ValidNewItemNotifyCollection = new int[]
		{
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			8,
			9,
			10,
			11,
			12,
			13,
			14,
			15
		};

		internal struct ViewRowCountValues
		{
			internal const int DefaultMaxViewRowCount = 100;

			internal const int AllowedMaxViewRowCount = 1000;

			internal const int MinViewRowCount = 5;

			internal const int ThreshholdViewRowCount = 50;

			internal const int LowMultipleRowCount = 5;

			internal const int HighMultipleRowCount = 25;

			internal static int MaxViewRowCountConfigValue = 1000;

			internal static bool IsConfigValueDefined = false;
		}

		internal struct SignatureValues
		{
			internal const int AllowedSignatureMaxLength = 16000;

			internal static int SignatureMaxLengthConfigValue = 16000;

			internal static bool IsConfigValueDefined = false;
		}

		internal struct MOWASignatureValues
		{
			internal const int AllowedSignatureMaxLength = 512;

			internal static int SignatureMaxLengthConfigValue = 512;

			internal static bool IsConfigValueDefined = false;
		}

		internal struct SpellingCheckBeforeSendValues
		{
			internal static bool IsCofigValueDefine;

			internal static bool SpellingCheckBeforeSendConfigValue;
		}
	}
}
