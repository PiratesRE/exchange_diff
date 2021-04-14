using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public sealed class UserOptionPropertyValidationUtility
	{
		private UserOptionPropertyValidationUtility()
		{
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

		private static object ValidateDateTimeFormat(object value, string defaultFormat)
		{
			string text = (string)value;
			if (!string.IsNullOrEmpty(text) && text.Length <= 80)
			{
				try
				{
					ExDateTime.UtcNow.ToString(text, CultureInfo.CurrentUICulture);
					ExTraceGlobals.UserOptionsDataTracer.TraceDebug(0L, "Returning original value: {0}", new object[]
					{
						value
					});
					return value;
				}
				catch (FormatException)
				{
					ExTraceGlobals.UserOptionsTracer.TraceDebug(0L, "'{0}' is not a valid DateTime format.", new object[]
					{
						value
					});
				}
			}
			ExTraceGlobals.UserOptionsDataTracer.TraceDebug<string>(0L, "Returning default value: {0}", defaultFormat);
			return defaultFormat;
		}

		internal static object ValidateViewRowCountCallbackHelp(object value, int defaultValue)
		{
			if (value != null)
			{
				try
				{
					int num = (int)value;
					int num2;
					if (UserOptionPropertyValidationUtility.ViewRowCountValues.IsConfigValueDefined)
					{
						num2 = ((1000 < UserOptionPropertyValidationUtility.ViewRowCountValues.MaxViewRowCountConfigValue) ? 1000 : UserOptionPropertyValidationUtility.ViewRowCountValues.MaxViewRowCountConfigValue);
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
					ExTraceGlobals.UserOptionsTracer.TraceDebug(0L, "Failed to cast '{0}' to int type", new object[]
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
			return UserOptionPropertyValidationUtility.ValidateViewRowCountCallbackHelp(value, 50);
		}

		internal static object ValidateBasicViewRowCountCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateViewRowCountCallbackHelp(value, 20);
		}

		internal static object ValidateNextSelectionCallback(object value)
		{
			return (NextSelectionDirection)UserOptionPropertyValidationUtility.ValidateIntRange(value, 1, 0, 2);
		}

		internal static object ValidateTimeZoneCallback(object value)
		{
			string text = value as string;
			if (text != null)
			{
				ExTimeZone exTimeZone = null;
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
			ExTraceGlobals.UserOptionsDataTracer.TraceDebug<string>(0L, "Returning default value: {0}", UserOptionPropertyValidationUtility.DefaultTimeZone);
			return UserOptionPropertyValidationUtility.DefaultTimeZone;
		}

		internal static object ValidateTimeFormatCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateDateTimeFormat(value, DateTimeFormatInfo.CurrentInfo.ShortTimePattern);
		}

		internal static object ValidateDateFormatCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateDateTimeFormat(value, DateTimeFormatInfo.CurrentInfo.ShortDatePattern);
		}

		internal static object ValidateWeekStartDayCallback(object value)
		{
			return (DayOfWeek)UserOptionPropertyValidationUtility.ValidateIntRange(value, 0, 0, 6);
		}

		internal static object ValidateHourIncrementCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateIntCollection(value, 30, new int[]
			{
				15,
				30
			});
		}

		internal static object ValidateShowWeekNumbersCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateCheckNameInContactsFirstCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateFirstWeekOfYearCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateIntRange(value, 1, 0, 3);
		}

		internal static object ValidateEnableRemindersCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, true);
		}

		internal static object ValidateEnableReminderSoundCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, true);
		}

		internal static object ValidateNewItemNotifyCallback(object value)
		{
			return (NewNotification)UserOptionPropertyValidationUtility.ValidateIntCollection(value, 15, UserOptionPropertyValidationUtility.ValidNewItemNotifyCollection);
		}

		internal static object ValidateSpellingDictionaryLanguageCallback(object value)
		{
			return value;
		}

		internal static object ValidateSpellingIgnoreUppercaseCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateSpellingIgnoreMixedDigitsCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateSpellingCheckBeforeSendCallback(object value)
		{
			bool flag = UserOptionPropertyValidationUtility.SpellingCheckBeforeSendValues.IsCofigValueDefine && UserOptionPropertyValidationUtility.SpellingCheckBeforeSendValues.SpellingCheckBeforeSendConfigValue;
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, flag);
		}

		internal static object ValidateSmimeEncryptCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateSmimeSignCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateAlwaysShowBccCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateAlwaysShowFromCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateComposeMarkupCallback(object value)
		{
			return (Markup)UserOptionPropertyValidationUtility.ValidateIntRange(value, 0, 0, 1);
		}

		internal static object ValidateComposeFontNameCallback(object value)
		{
			int num = 100;
			string defaultFontName = Utilities.GetDefaultFontName();
			string text = (string)value;
			if (!string.IsNullOrEmpty(text) && text.Length <= num)
			{
				ExTraceGlobals.UserOptionsDataTracer.TraceDebug<string>(0L, "Returning default value: {0}", defaultFontName);
				return value;
			}
			return defaultFontName;
		}

		internal static object ValidateComposeFontSizeCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateIntRange(value, 2, 1, 7);
		}

		internal static object ValidateComposeFontColorCallback(object value)
		{
			string text = (string)value;
			if (!string.IsNullOrEmpty(text) && UserOptionPropertyValidationUtility.validColorRegex.Match(text).Success)
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
			return (FontFlags)UserOptionPropertyValidationUtility.ValidateIntRange(value, 0, 0, 7);
		}

		internal static object ValidateAutoAddSignatureCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateSignatureTextCallback(object value)
		{
			string text = (string)value;
			if (string.IsNullOrEmpty(text))
			{
				return value;
			}
			int num;
			if (UserOptionPropertyValidationUtility.SignatureValues.IsConfigValueDefined)
			{
				num = ((UserOptionPropertyValidationUtility.SignatureValues.SignatureMaxLengthConfigValue < 16000) ? UserOptionPropertyValidationUtility.SignatureValues.SignatureMaxLengthConfigValue : 16000);
			}
			else
			{
				num = 4096;
			}
			if (text.Length <= num)
			{
				ExTraceGlobals.UserOptionsDataTracer.TraceDebug(0L, "Returning original value: {0}", new object[]
				{
					value
				});
				return value;
			}
			string text2 = text.Substring(0, 4096);
			ExTraceGlobals.UserOptionsDataTracer.TraceDebug<int, string>(0L, "Signature is longer that max length '{0}'. Returning truncated value: {1}", 4096, text2);
			return text2;
		}

		internal static object ValidateSignatureHtmlCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateSignatureTextCallback(value);
		}

		internal static object ValidateBlockExternalContentCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, true);
		}

		internal static object ValidatePreviewMarkAsReadCallback(object value)
		{
			return (MarkAsRead)UserOptionPropertyValidationUtility.ValidateIntRange(value, 1, 0, 2);
		}

		internal static object ValidateMarkAsReadDelaytimeCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateIntRange(value, 5, 0, 30);
		}

		internal static object ValidateReadReceiptCallback(object value)
		{
			return (ReadReceiptResponse)UserOptionPropertyValidationUtility.ValidateIntRange(value, 0, 0, 2);
		}

		internal static object ValidateEmptyDeletedItemsOnLogoffCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateNavigationBarWidthCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateIntRange(value, 175, 50, 2000);
		}

		internal static object ValidateMiniBarCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateQuickLinksCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateTaskDetailsCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, true);
		}

		internal static object ValidateDocumentFavoritesCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, true);
		}

		internal static object ValidateOutlookSharedFoldersCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, true);
		}

		internal static object ValidateFormatBarStateCallback(object value)
		{
			return (FormatBarButtonGroups)UserOptionPropertyValidationUtility.ValidateIntRange(value, FormatBarButtonGroups.BoldItalicUnderline | FormatBarButtonGroups.Lists | FormatBarButtonGroups.Indenting | FormatBarButtonGroups.ForegroundColor | FormatBarButtonGroups.BackgroundColor, 0, 16383);
		}

		internal static object ValidateMruFontsCallback(object value)
		{
			int num = 1000;
			string empty = string.Empty;
			string text = (string)value;
			if (string.IsNullOrEmpty(text) || text.Length > num)
			{
				ExTraceGlobals.UserOptionsDataTracer.TraceDebug<string>(0L, "Returning default value: {0}", empty);
				return empty;
			}
			return text;
		}

		internal static object ValidatePrimaryNavigationCollapsedCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateThemeStorageIdCallback(object value)
		{
			string text = value as string;
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}
			return text;
		}

		internal static object ValidateFindBarOnCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, true);
		}

		internal static object ValidateSearchScopeCallback(object value)
		{
			return (SearchScope)UserOptionPropertyValidationUtility.ValidateIntRange(value, 3, 0, 3);
		}

		internal static object ValidateContactsSearchScopeCallback(object value)
		{
			return (SearchScope)UserOptionPropertyValidationUtility.ValidateIntRange(value, 0, 0, 2);
		}

		internal static object ValidateTasksSearchScopeCallback(object value)
		{
			return (SearchScope)UserOptionPropertyValidationUtility.ValidateIntRange(value, 0, 0, 2);
		}

		internal static object ValidateIsOptimizedForAccessibilityCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateEnabledPontsCallback(object value)
		{
			return (PontType)UserOptionPropertyValidationUtility.ValidateIntRange(value, int.MaxValue, 0, int.MaxValue);
		}

		internal static object ValidateFlagActionCallback(object value)
		{
			return (FlagAction)UserOptionPropertyValidationUtility.ValidateIntRange(value, 2, 2, 6);
		}

		internal static object ValidateAddRecipientsToAutoCompleteCacheCallBack(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, true);
		}

		internal static object ValidateManuallyPickCertificateCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, false);
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
			return UserOptionPropertyValidationUtility.ValidateIntCollection(value, -1, new int[]
			{
				0,
				1
			});
		}

		internal static object ValidateConversationSortOrderCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateIntCollection(value, 5, UserOptionPropertyValidationUtility.ValidConversationSortOrderValues);
		}

		internal static object ValidateShowTreeInListViewCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateHideDeletedItemsCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateHideMailTipsByDefaultCallback(object value)
		{
			return UserOptionPropertyValidationUtility.ValidateBoolValue(value, false);
		}

		internal static object ValidateSendAddressDefaultCallback(object value)
		{
			return value;
		}

		private const int DefaultViewRowCount = 50;

		private const int DefaultBasicViewRowCount = 20;

		private const NextSelectionDirection DefaultNextSelection = NextSelectionDirection.Next;

		private const DayOfWeek DefaultWeekStartDay = DayOfWeek.Sunday;

		private const int DefaultHourIncrement = 30;

		private const bool DefaultShowWeekNumbers = false;

		private const bool DefaultCheckNameInContactsFirst = false;

		private const int DefaultFirstWeekOfYear = 1;

		private const bool DefaultEnableReminders = true;

		private const bool DefaultEnableReminderSound = true;

		private const NewNotification DefaultNewItemNotify = NewNotification.Sound | NewNotification.EMailToast | NewNotification.VoiceMailToast | NewNotification.FaxToast;

		private const bool DefaultSpellingIgnoreUppercase = false;

		private const bool DefaultSpellingIgnoreMixedDigits = false;

		private const bool DefaultSpellingCheckBeforeSend = false;

		private const bool DefaultSmimeEncrypt = false;

		private const bool DefaultSmimeSign = false;

		private const bool DefaultAlwaysShowBcc = false;

		private const bool DefaultAlwaysShowFrom = false;

		private const Markup DefaultComposeMarkup = Markup.Html;

		private const int DefaultComposeFontSize = 2;

		private const string DefaultComposeFontColor = "#000000";

		private const FontFlags DefaultComposeFontFlags = FontFlags.Normal;

		private const bool DefaultAutoAddSignature = false;

		private const int DefaultSignatureMaxLength = 4096;

		private const bool DefaultBlockExternalContent = true;

		private const MarkAsRead DefaultPreviewMarkAsRead = MarkAsRead.OnSelectionChange;

		private const int DefaultMarkAsReadDelaytime = 5;

		private const ReadReceiptResponse DefaultReadReceipt = ReadReceiptResponse.DoNotAutomaticallySend;

		private const bool DefaultEmptyDeletedItemsOnLogoff = false;

		private const int DefaultNavBarWidth = 175;

		private const bool DefaultMiniBarVisible = false;

		private const bool DefaultQuickLinksVisible = false;

		private const bool DefaultTaskDetailsVisible = true;

		private const bool DefaultDocumentFavoritesVisible = true;

		private const bool DefaultOutlookSharedFoldersVisible = true;

		private const FormatBarButtonGroups DefaultFormatBarState = FormatBarButtonGroups.BoldItalicUnderline | FormatBarButtonGroups.Lists | FormatBarButtonGroups.Indenting | FormatBarButtonGroups.ForegroundColor | FormatBarButtonGroups.BackgroundColor;

		private const bool PrimaryNavigationCollapsed = false;

		private const bool DefaultFindBarOn = true;

		private const SearchScope DefaultSearchScope = SearchScope.AllFoldersAndItems;

		private const SearchScope DefaultContactsSearchScope = SearchScope.SelectedFolder;

		private const SearchScope DefaultTasksSearchScope = SearchScope.SelectedFolder;

		private const bool DefaultIsOptimizedForAccessibility = false;

		private const PontType DefaultEnabledPonts = PontType.All;

		private const FlagAction DefaultFlagAction = FlagAction.Today;

		private const bool DefaultAddRecipientsToAutoCompleteCache = true;

		private const bool DefaultManuallyPickCertificate = false;

		private const int DefaultUseDataCenterCustomTheme = -1;

		private const ConversationSortOrder DefaultConversationSortOrder = ConversationSortOrder.ChronologicalNewestOnTop;

		private const bool DefaultShowTreeInListView = false;

		private const bool DefaultHideDeletedItems = false;

		private const bool DefaultHideMailTipsByDefault = false;

		private static readonly string DefaultTimeZone = string.Empty;

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

		private static readonly Regex validColorRegex = new Regex("^\\#[0-9a-fA-F]{6}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly int[] ValidConversationSortOrderValues = new int[]
		{
			5,
			9
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

		internal struct SpellingCheckBeforeSendValues
		{
			internal static bool IsCofigValueDefine;

			internal static bool SpellingCheckBeforeSendConfigValue;
		}

		internal struct SignatureValues
		{
			internal const int AllowedSignatureMaxLength = 16000;

			internal static int SignatureMaxLengthConfigValue = 16000;

			internal static bool IsConfigValueDefined = false;
		}
	}
}
