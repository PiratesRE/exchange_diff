using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class RecurrenceHelper
	{
		private static DateTime ParseDateTime(string dateTimeString)
		{
			return XmlConvert.ToDateTime(dateTimeString, XmlDateTimeSerializationMode.RoundtripKind);
		}

		private const int MinDayOfMonth = 1;

		private const int MaxDayOfMonth = 31;

		private const int MinMonthlyInterval = 1;

		private const int MaxMonthlyInterval = 99;

		private const string XmlElementNameNumberOfOccurrences = "NumberOfOccurrences";

		private const string XmlElementNameDayOfMonth = "DayOfMonth";

		private const string XmlElementNameInterval = "Interval";

		public static class Recurrence
		{
			public static RecurrenceType Render(Microsoft.Exchange.Data.Storage.Recurrence recurrence)
			{
				if (recurrence != null)
				{
					return new RecurrenceType
					{
						RecurrencePattern = RecurrenceHelper.RecurrencePattern.Render(recurrence.Pattern),
						RecurrenceRange = RecurrenceHelper.RecurrenceRange.Render(recurrence.Range)
					};
				}
				return null;
			}

			public static TaskRecurrenceType RenderForTask(Microsoft.Exchange.Data.Storage.Recurrence recurrence)
			{
				if (recurrence != null)
				{
					return new TaskRecurrenceType
					{
						RecurrencePattern = RecurrenceHelper.RecurrencePattern.Render(recurrence.Pattern),
						RecurrenceRange = RecurrenceHelper.RecurrenceRange.Render(recurrence.Range)
					};
				}
				return null;
			}

			public static bool Parse(ExTimeZone timezone, RecurrenceType recurrenceType, out Microsoft.Exchange.Data.Storage.Recurrence outRecurrence)
			{
				Microsoft.Exchange.Data.Storage.RecurrencePattern recurrencePattern;
				Microsoft.Exchange.Data.Storage.RecurrenceRange recurrenceRange;
				if (RecurrenceHelper.RecurrencePattern.Parse(recurrenceType.RecurrencePattern, out recurrencePattern) && RecurrenceHelper.RecurrenceRange.Parse(recurrenceType.RecurrenceRange, timezone, out recurrenceRange))
				{
					outRecurrence = RecurrenceHelper.Recurrence.CreateAndAssignRecurrence(recurrencePattern, recurrenceRange, timezone, timezone, null);
					return true;
				}
				outRecurrence = null;
				return false;
			}

			public static bool Parse(ExTimeZone timezone, TaskRecurrenceType recurrenceType, out Microsoft.Exchange.Data.Storage.Recurrence outRecurrence)
			{
				Microsoft.Exchange.Data.Storage.RecurrencePattern recurrencePattern;
				Microsoft.Exchange.Data.Storage.RecurrenceRange recurrenceRange;
				if (RecurrenceHelper.RecurrencePattern.Parse(recurrenceType.RecurrencePattern, out recurrencePattern) && RecurrenceHelper.RecurrenceRange.Parse(recurrenceType.RecurrenceRange, timezone, out recurrenceRange))
				{
					outRecurrence = RecurrenceHelper.Recurrence.CreateAndAssignRecurrence(recurrencePattern, recurrenceRange, timezone, timezone, null);
					return true;
				}
				outRecurrence = null;
				return false;
			}

			internal static Microsoft.Exchange.Data.Storage.Recurrence CreateAndAssignRecurrence(Microsoft.Exchange.Data.Storage.RecurrencePattern recurrencePattern, Microsoft.Exchange.Data.Storage.RecurrenceRange recurrenceRange, ExTimeZone createTimeZone, ExTimeZone readTimeZone, CalendarItem calendarItem)
			{
				Microsoft.Exchange.Data.Storage.Recurrence recurrence;
				try
				{
					if (createTimeZone == null && readTimeZone == null)
					{
						recurrence = new Microsoft.Exchange.Data.Storage.Recurrence(recurrencePattern, recurrenceRange);
					}
					else
					{
						recurrence = new Microsoft.Exchange.Data.Storage.Recurrence(recurrencePattern, recurrenceRange, createTimeZone, readTimeZone);
					}
					if (calendarItem != null)
					{
						calendarItem.Recurrence = recurrence;
					}
				}
				catch (InvalidOperationException)
				{
					throw new CalendarExceptionInvalidRecurrence();
				}
				catch (ArgumentException)
				{
					throw new CalendarExceptionInvalidRecurrence();
				}
				return recurrence;
			}
		}

		public static class RequestTimeZone
		{
			public static bool TimeZoneContextIsAvailable()
			{
				return ExchangeVersion.Current.Equals(ExchangeVersion.Exchange2007SP1) && EWSSettings.RequestTimeZone != ExTimeZone.UtcTimeZone;
			}

			public static bool NeedTimeZoneContextForTask(IdAndSession idAndSession)
			{
				StoreObjectId asStoreObjectId = idAndSession.GetAsStoreObjectId();
				return asStoreObjectId.ObjectType.Equals(StoreObjectType.Task) && RecurrenceHelper.RequestTimeZone.TimeZoneContextIsAvailable();
			}
		}

		public static class MeetingTimeZone
		{
			internal static ExTimeZone DefaultMeetingTimeZone
			{
				get
				{
					if (ExchangeVersion.Current.Equals(ExchangeVersion.Exchange2007))
					{
						return ExTimeZone.CurrentTimeZone;
					}
					if (RecurrenceHelper.MeetingTimeZone.utcExTimeZone == null)
					{
						string text = "Greenwich Standard Time";
						string daylightName = "Greenwich Daylight Time";
						RecurrenceHelper.MeetingTimeZone.utcExTimeZone = RecurrenceHelper.MeetingTimeZone.CreateExTimeZoneFromRegTimeZoneInfo(new REG_TIMEZONE_INFO
						{
							Bias = 0,
							DaylightBias = 0,
							StandardBias = 0
						}, text, text, text, daylightName);
					}
					return RecurrenceHelper.MeetingTimeZone.utcExTimeZone;
				}
			}

			public static ExTimeZone GetMeetingTimeZone(TimeZoneType meetingTimeZone, out ServiceError warning)
			{
				warning = null;
				if (meetingTimeZone != null)
				{
					return RecurrenceHelper.MeetingTimeZone.Parse(meetingTimeZone, out warning);
				}
				return RecurrenceHelper.MeetingTimeZone.DefaultMeetingTimeZone;
			}

			public static ExTimeZone GetLastMeetingTimeZone(PropertyUpdate[] propertyUpdates, out ServiceError warning)
			{
				warning = null;
				for (int i = propertyUpdates.Length - 1; i >= 0; i--)
				{
					PropertyUri propertyUri = propertyUpdates[i].PropertyPath as PropertyUri;
					if (propertyUri != null && propertyUri.Uri == PropertyUriEnum.MeetingTimeZone)
					{
						SetPropertyUpdate setPropertyUpdate = propertyUpdates[i] as SetPropertyUpdate;
						if (setPropertyUpdate != null && setPropertyUpdate.ServiceObject != null && setPropertyUpdate.ServiceObject.PropertyBag.Contains(CalendarItemSchema.MeetingTimeZone))
						{
							TimeZoneType timeZoneType = setPropertyUpdate.ServiceObject[CalendarItemSchema.MeetingTimeZone] as TimeZoneType;
							if (timeZoneType != null)
							{
								return RecurrenceHelper.MeetingTimeZone.Parse(timeZoneType, out warning);
							}
						}
					}
				}
				return RecurrenceHelper.MeetingTimeZone.DefaultMeetingTimeZone;
			}

			public static TimeZoneType Render(CalendarItem calendarItem)
			{
				if (calendarItem == null || calendarItem.Recurrence == null || !calendarItem.Recurrence.HasTimeZone)
				{
					return null;
				}
				ExTimeZone createdExTimeZone = calendarItem.Recurrence.CreatedExTimeZone;
				REG_TIMEZONE_INFO reg_TIMEZONE_INFO = TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(createdExTimeZone);
				TimeZoneType timeZoneType = new TimeZoneType
				{
					TimeZoneName = (string.IsNullOrEmpty(createdExTimeZone.AlternativeId) ? null : createdExTimeZone.AlternativeId),
					BaseOffset = RecurrenceHelper.TimeZoneOffset.ToString(reg_TIMEZONE_INFO.Bias)
				};
				if (reg_TIMEZONE_INFO.StandardDate.Month == 0 || reg_TIMEZONE_INFO.DaylightDate.Month == 0)
				{
					return timeZoneType;
				}
				ExTimeZoneInformation timeZoneInformation = createdExTimeZone.TimeZoneInformation;
				if (timeZoneInformation.Groups.Count >= 1)
				{
					ExTimeZoneRuleGroup exTimeZoneRuleGroup = timeZoneInformation.Groups[0];
					if (exTimeZoneRuleGroup.Rules.Count >= 2)
					{
						timeZoneType.Standard = RecurrenceHelper.TimeChangePattern.Render(exTimeZoneRuleGroup.Rules[0].DisplayName, reg_TIMEZONE_INFO.StandardBias, reg_TIMEZONE_INFO.StandardDate);
						timeZoneType.Daylight = RecurrenceHelper.TimeChangePattern.Render(exTimeZoneRuleGroup.Rules[1].DisplayName, reg_TIMEZONE_INFO.DaylightBias, reg_TIMEZONE_INFO.DaylightDate);
					}
				}
				return timeZoneType;
			}

			public static ExTimeZone Parse(TimeZoneType meetingTimeZone, out ServiceError warning)
			{
				REG_TIMEZONE_INFO timeZoneInfo = default(REG_TIMEZONE_INFO);
				bool timeZoneInfoSpecified = false;
				string timeZoneName = meetingTimeZone.TimeZoneName;
				string timeZoneStdName = null;
				string timeZoneDltName = null;
				timeZoneInfo.DaylightBias = 0;
				timeZoneInfo.StandardBias = 0;
				if (RecurrenceHelper.TimeZoneOffset.Parse(meetingTimeZone.BaseOffset, out timeZoneInfo.Bias))
				{
					RecurrenceHelper.TimeChangePattern.Parse(meetingTimeZone.Standard, out timeZoneInfo.StandardBias, out timeZoneInfo.StandardDate, out timeZoneStdName);
					RecurrenceHelper.TimeChangePattern.Parse(meetingTimeZone.Daylight, out timeZoneInfo.DaylightBias, out timeZoneInfo.DaylightDate, out timeZoneDltName);
					timeZoneInfoSpecified = true;
				}
				return RecurrenceHelper.MeetingTimeZone.ProcessAndValidateExTimeZone(timeZoneInfo, timeZoneInfoSpecified, timeZoneName, timeZoneStdName, timeZoneDltName, out warning);
			}

			private static ExTimeZone CreateExTimeZoneFromRegTimeZoneInfo(REG_TIMEZONE_INFO regInfo, string keyName, string displayName, string standardName, string daylightName)
			{
				return TimeZoneHelper.CreateCustomExTimeZoneFromRegTimeZoneInfo(regInfo, keyName, displayName);
			}

			private static ExTimeZone ProcessAndValidateExTimeZone(REG_TIMEZONE_INFO timeZoneInfo, bool timeZoneInfoSpecified, string timeZoneKeyName, string timeZoneStdName, string timeZoneDltName, out ServiceError warning)
			{
				warning = null;
				ExTimeZone result = null;
				if (timeZoneInfoSpecified)
				{
					if (string.IsNullOrEmpty(timeZoneKeyName))
					{
						timeZoneKeyName = "UnnamedCustomTimeZone";
					}
					if (timeZoneStdName == null)
					{
						timeZoneStdName = string.Empty;
					}
					if (timeZoneDltName == null)
					{
						timeZoneDltName = string.Empty;
					}
					try
					{
						result = RecurrenceHelper.MeetingTimeZone.CreateExTimeZoneFromRegTimeZoneInfo(timeZoneInfo, timeZoneKeyName, timeZoneKeyName, timeZoneStdName, timeZoneDltName);
					}
					catch (InvalidTimeZoneException exception)
					{
						throw new CalendarExceptionInvalidTimeZone(exception);
					}
				}
				if (ExchangeVersion.Current == ExchangeVersion.Exchange2007)
				{
					return result;
				}
				RecurrenceHelper.MeetingTimeZone.TimeZoneNameMatchKind timeZoneNameMatchKind;
				CultureInfo cultureInfo;
				ExTimeZone exTimeZone = RecurrenceHelper.MeetingTimeZone.FindServerExTimeZone(timeZoneKeyName, out timeZoneNameMatchKind, out cultureInfo);
				IDictionary<string, string> dictionary;
				if (timeZoneNameMatchKind == RecurrenceHelper.MeetingTimeZone.TimeZoneNameMatchKind.KeyName)
				{
					if (!timeZoneInfoSpecified)
					{
						return exTimeZone;
					}
					if (TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(exTimeZone).Equals(timeZoneInfo))
					{
						return exTimeZone;
					}
					dictionary = new Dictionary<string, string>();
					dictionary.Add("ServerTimeZone.TimeZoneName", exTimeZone.AlternativeId);
				}
				else
				{
					if (!timeZoneInfoSpecified)
					{
						throw new CalendarExceptionInvalidTimeZone(CoreResources.IDs.ErrorCalendarInvalidTimeZone);
					}
					if (timeZoneNameMatchKind == RecurrenceHelper.MeetingTimeZone.TimeZoneNameMatchKind.NotFound)
					{
						return result;
					}
					dictionary = new Dictionary<string, string>();
					if (timeZoneNameMatchKind == RecurrenceHelper.MeetingTimeZone.TimeZoneNameMatchKind.LocalizableDisplayName)
					{
						dictionary.Add("ServerTimeZone.Display.TimeZoneName", exTimeZone.LocalizableDisplayName.ToString(cultureInfo));
					}
					dictionary.Add("ServerTimeZone.Culture", cultureInfo.TextInfo.CultureName);
					dictionary.Add("ServerTimeZone.TimeZoneName", exTimeZone.AlternativeId);
				}
				warning = new ServiceError(CoreResources.IDs.ErrorCalendarInvalidTimeZone, ResponseCodeType.ErrorCalendarInvalidTimeZone, 0, ExchangeVersion.Exchange2007, dictionary);
				return result;
			}

			private static ExTimeZone FindServerExTimeZone(string timeZoneName, out RecurrenceHelper.MeetingTimeZone.TimeZoneNameMatchKind matchKind, out CultureInfo matchCulture)
			{
				matchKind = RecurrenceHelper.MeetingTimeZone.TimeZoneNameMatchKind.NotFound;
				matchCulture = null;
				if (string.IsNullOrEmpty(timeZoneName))
				{
					return null;
				}
				foreach (ExTimeZone exTimeZone in ExTimeZoneEnumerator.Instance)
				{
					if (string.Compare(timeZoneName, exTimeZone.AlternativeId, StringComparison.OrdinalIgnoreCase) == 0)
					{
						matchKind = RecurrenceHelper.MeetingTimeZone.TimeZoneNameMatchKind.KeyName;
					}
					else
					{
						if (!RecurrenceHelper.MeetingTimeZone.MatchLocalizeName(timeZoneName, exTimeZone.LocalizableDisplayName, out matchCulture))
						{
							continue;
						}
						matchKind = RecurrenceHelper.MeetingTimeZone.TimeZoneNameMatchKind.LocalizableDisplayName;
					}
					return exTimeZone;
				}
				return null;
			}

			private static bool MatchLocalizeName(string name, LocalizedString localizedName, out CultureInfo matchCulture)
			{
				matchCulture = null;
				foreach (CultureInfo cultureInfo in RecurrenceHelper.MeetingTimeZone.clientCultures)
				{
					if (cultureInfo.CompareInfo.Compare(name, localizedName.ToString(cultureInfo), CompareOptions.IgnoreCase) == 0)
					{
						matchCulture = cultureInfo;
						return true;
					}
				}
				return false;
			}

			private const string UnnamedCustomTimeZone = "UnnamedCustomTimeZone";

			private static CultureInfo[] clientCultures = ClientCultures.GetAllSupportedDsnLanguages();

			private static ExTimeZone utcExTimeZone = null;

			private enum TimeZoneNameMatchKind
			{
				NotFound,
				KeyName,
				LocalizableDisplayName
			}
		}

		private static class CSharpInt
		{
			public static void Validate(int value, string elementName, int minValue, int maxValue)
			{
				if (value < minValue || value > maxValue)
				{
					throw new CalendarExceptionOutOfRange(new ExceptionPropertyUri(RecurrenceHelper.CSharpInt.GetPropertyUri(elementName)));
				}
			}

			private static ExceptionPropertyUriEnum GetPropertyUri(string elementName)
			{
				ExceptionPropertyUriEnum result = ExceptionPropertyUriEnum.Month;
				if (elementName != null && !(elementName == "DayOfMonth"))
				{
					if (!(elementName == "Interval"))
					{
						if (elementName == "NumberOfOccurrences")
						{
							result = ExceptionPropertyUriEnum.NumberOfOccurrences;
						}
					}
					else
					{
						result = ExceptionPropertyUriEnum.Interval;
					}
				}
				return result;
			}
		}

		private static class TimeZoneOffset
		{
			public static string Render(int minutes)
			{
				return RecurrenceHelper.TimeZoneOffset.ToString(minutes);
			}

			public static string ToString(int minutes)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (minutes < 0)
				{
					stringBuilder.Append("-PT");
					minutes = -minutes;
				}
				else
				{
					stringBuilder.Append("PT");
				}
				stringBuilder.Append(minutes.ToString(CultureInfo.InvariantCulture));
				stringBuilder.Append("M");
				return stringBuilder.ToString();
			}

			public static bool Parse(string offsetString, out int minutes)
			{
				if (string.IsNullOrEmpty(offsetString))
				{
					minutes = 0;
					return false;
				}
				TimeSpan timeSpan = XmlConvert.ToTimeSpan(offsetString);
				if (timeSpan.TotalMilliseconds <= -86400000.0 || timeSpan.TotalMilliseconds >= 86400000.0)
				{
					throw new CalendarExceptionOutOfRange(new ExceptionPropertyUri(ExceptionPropertyUriEnum.Offset));
				}
				minutes = Convert.ToInt32(timeSpan.TotalMinutes);
				return true;
			}

			private const int MaxMilliseconds = 86400000;

			private const int MinMilliseconds = -86400000;
		}

		private static class SystemTimeSpan
		{
			public static string Render(TimeSpan timeSpan)
			{
				return ExDateTimeConverter.ToUtcXsdTime(timeSpan);
			}

			public static bool Parse(string timeSpanString, out short hour, out short minute, out short second, out short millisecond)
			{
				if (string.IsNullOrEmpty(timeSpanString))
				{
					hour = 0;
					minute = 0;
					second = 0;
					millisecond = 0;
					return false;
				}
				DateTime dateTime = DateTime.Parse(timeSpanString, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
				hour = (short)dateTime.Hour;
				minute = (short)dateTime.Minute;
				second = (short)dateTime.Second;
				millisecond = (short)dateTime.Millisecond;
				return true;
			}

			private const string XmlElementNameTime = "Time";
		}

		private static class SystemDateTime
		{
			public static string Render(ExDateTime dateTime, RecurrenceHelper.SystemDateTime.RenderKind renderKind)
			{
				switch (renderKind)
				{
				case RecurrenceHelper.SystemDateTime.RenderKind.Time:
					return dateTime.UniversalTime.ToString("HH:mm:ssZ", CultureInfo.InvariantCulture);
				case RecurrenceHelper.SystemDateTime.RenderKind.Date:
					return RecurrenceHelper.SystemDateTime.ConvertExDateToString(dateTime);
				case RecurrenceHelper.SystemDateTime.RenderKind.DateTime:
					return dateTime.UniversalTime.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
				default:
					return null;
				}
			}

			public static bool Parse(ExTimeZone timeZone, string dateTimeString, out ExDateTime dateTime)
			{
				if (string.IsNullOrEmpty(dateTimeString))
				{
					dateTime = (ExDateTime)default(DateTime);
					return false;
				}
				try
				{
					DateTime dateTime2 = RecurrenceHelper.ParseDateTime(dateTimeString);
					if (dateTime2.Kind == DateTimeKind.Unspecified)
					{
						if (timeZone == null)
						{
							dateTime2 = DateTime.SpecifyKind(dateTime2, DateTimeKind.Local);
							dateTime = (ExDateTime)dateTime2;
						}
						else
						{
							dateTime = new ExDateTime(timeZone, dateTime2.Year, dateTime2.Month, dateTime2.Day, dateTime2.Hour, dateTime2.Minute, dateTime2.Second, dateTime2.Millisecond);
						}
					}
					else if (dateTime2.Kind == DateTimeKind.Local)
					{
						if (timeZone == null)
						{
							dateTime = (ExDateTime)dateTime2.ToUniversalTime();
						}
						else
						{
							dateTime = timeZone.ConvertDateTime((ExDateTime)dateTime2.ToUniversalTime());
						}
					}
					else
					{
						dateTime = (ExDateTime)dateTime2;
					}
				}
				catch (ArgumentException innerException)
				{
					throw new InvalidValueForPropertyException(CoreResources.IDs.ErrorInvalidValueForPropertyDate, innerException);
				}
				return true;
			}

			private static string ConvertExDateToString(ExDateTime date)
			{
				TimeSpan timeOfDay = date.TimeOfDay;
				if (timeOfDay.Ticks != 0L)
				{
					ExTraceGlobals.CalendarDataTracer.TraceDebug<string, string, string>(0L, "[RecurrenceHelper.SystemDateTime.ConvertExDateToString] TimeOfDay is not zero, date={0}; date.timeOfDay={1}; date.Bias={2};", date.ToString(), timeOfDay.ToString(), date.Bias.ToString());
				}
				string str = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
				if (date.Bias.Ticks == 0L)
				{
					return str + "Z";
				}
				TimeSpan timeSpan;
				string format;
				if (date.Bias.Ticks < 0L)
				{
					timeSpan = date.Bias.Negate();
					format = "-00";
				}
				else
				{
					timeSpan = date.Bias;
					format = "+00";
				}
				return str + timeSpan.Hours.ToString(format) + timeSpan.Minutes.ToString(":00");
			}

			private const string TimeUtcFormat = "HH:mm:ssZ";

			private const string DateTimeUtcFormat = "yyyy-MM-ddTHH:mm:ssZ";

			public enum RenderKind
			{
				Time,
				Date,
				DateTime
			}
		}

		private static class Month
		{
			public static string Render(int month)
			{
				return RecurrenceHelper.Month.ToString(month);
			}

			public static bool Parse(string monthString, out int month)
			{
				if (string.IsNullOrEmpty(monthString))
				{
					month = 0;
					return false;
				}
				month = RecurrenceHelper.Month.Parse(monthString);
				return true;
			}

			private static string ToString(int month)
			{
				switch (month)
				{
				case 1:
					return "January";
				case 2:
					return "February";
				case 3:
					return "March";
				case 4:
					return "April";
				case 5:
					return "May";
				case 6:
					return "June";
				case 7:
					return "July";
				case 8:
					return "August";
				case 9:
					return "September";
				case 10:
					return "October";
				case 11:
					return "November";
				case 12:
					return "December";
				default:
					throw new CalendarExceptionInvalidPropertyState(new ExceptionPropertyUri(ExceptionPropertyUriEnum.Month));
				}
			}

			private static int Parse(string monthName)
			{
				switch (monthName)
				{
				case "January":
					return 1;
				case "February":
					return 2;
				case "March":
					return 3;
				case "April":
					return 4;
				case "May":
					return 5;
				case "June":
					return 6;
				case "July":
					return 7;
				case "August":
					return 8;
				case "September":
					return 9;
				case "October":
					return 10;
				case "November":
					return 11;
				case "December":
					return 12;
				}
				throw new CalendarExceptionInvalidPropertyState(new ExceptionPropertyUri(ExceptionPropertyUriEnum.Month));
			}

			private const string January = "January";

			private const string February = "February";

			private const string March = "March";

			private const string April = "April";

			private const string May = "May";

			private const string June = "June";

			private const string July = "July";

			private const string August = "August";

			private const string September = "September";

			private const string October = "October";

			private const string November = "November";

			private const string December = "December";
		}

		private static class DayOfWeekIndex
		{
			public static string Render(int dayOfWeekIndex)
			{
				return RecurrenceHelper.DayOfWeekIndex.ToString(dayOfWeekIndex);
			}

			public static bool Parse(string dayOfWeekIndexString, out int dayOfWeekIndex)
			{
				if (string.IsNullOrEmpty(dayOfWeekIndexString))
				{
					dayOfWeekIndex = 0;
					return false;
				}
				if (dayOfWeekIndexString != null)
				{
					if (!(dayOfWeekIndexString == "First"))
					{
						if (!(dayOfWeekIndexString == "Second"))
						{
							if (!(dayOfWeekIndexString == "Third"))
							{
								if (!(dayOfWeekIndexString == "Fourth"))
								{
									if (!(dayOfWeekIndexString == "Last"))
									{
										goto IL_6E;
									}
									dayOfWeekIndex = -1;
								}
								else
								{
									dayOfWeekIndex = 4;
								}
							}
							else
							{
								dayOfWeekIndex = 3;
							}
						}
						else
						{
							dayOfWeekIndex = 2;
						}
					}
					else
					{
						dayOfWeekIndex = 1;
					}
					return true;
				}
				IL_6E:
				throw new CalendarExceptionInvalidPropertyValue(new ExceptionPropertyUri(ExceptionPropertyUriEnum.DayOfWeekIndex));
			}

			private static string ToString(int dayOfWeekIndex)
			{
				switch (dayOfWeekIndex)
				{
				case -1:
					return "Last";
				case 1:
					return "First";
				case 2:
					return "Second";
				case 3:
					return "Third";
				case 4:
					return "Fourth";
				}
				throw new CalendarExceptionInvalidPropertyState(new ExceptionPropertyUri(ExceptionPropertyUriEnum.DayOfWeekIndex));
			}

			private const string First = "First";

			private const string Second = "Second";

			private const string Third = "Third";

			private const string Fourth = "Fourth";

			private const string Last = "Last";
		}

		private static class DaysOfWeek
		{
			public static string Render(Microsoft.Exchange.Data.DaysOfWeek daysOfWeek, RecurrenceHelper.DaysOfWeek.Kind kind)
			{
				return RecurrenceHelper.DaysOfWeek.ToString(daysOfWeek, kind);
			}

			public static string Render(short dayOfWeek)
			{
				return RecurrenceHelper.DaysOfWeek.ToString(dayOfWeek);
			}

			public static bool Parse(string daysOfWeekString, RecurrenceHelper.DaysOfWeek.Kind kind, out Microsoft.Exchange.Data.DaysOfWeek daysOfWeek)
			{
				daysOfWeek = Microsoft.Exchange.Data.DaysOfWeek.None;
				if (string.IsNullOrEmpty(daysOfWeekString))
				{
					return false;
				}
				if (kind == RecurrenceHelper.DaysOfWeek.Kind.SetOfPrimary)
				{
					string[] array = daysOfWeekString.Split(RecurrenceHelper.DaysOfWeek.dayNamesSeparator, StringSplitOptions.RemoveEmptyEntries);
					for (int i = 0; i < array.Length; i++)
					{
						daysOfWeek |= RecurrenceHelper.DaysOfWeek.Parse(array[i], RecurrenceHelper.DaysOfWeek.Kind.SetOfPrimary);
					}
					if (daysOfWeek == Microsoft.Exchange.Data.DaysOfWeek.None)
					{
						throw new CalendarExceptionInvalidPropertyValue(new ExceptionPropertyUri(ExceptionPropertyUriEnum.DaysOfWeek));
					}
				}
				else
				{
					daysOfWeek = RecurrenceHelper.DaysOfWeek.Parse(daysOfWeekString, kind);
				}
				return true;
			}

			public static short ToShort(Microsoft.Exchange.Data.DaysOfWeek daysOfWeek)
			{
				if (daysOfWeek <= Microsoft.Exchange.Data.DaysOfWeek.Wednesday)
				{
					switch (daysOfWeek)
					{
					case Microsoft.Exchange.Data.DaysOfWeek.Sunday:
						return 0;
					case Microsoft.Exchange.Data.DaysOfWeek.Monday:
						return 1;
					case Microsoft.Exchange.Data.DaysOfWeek.Sunday | Microsoft.Exchange.Data.DaysOfWeek.Monday:
						break;
					case Microsoft.Exchange.Data.DaysOfWeek.Tuesday:
						return 2;
					default:
						if (daysOfWeek == Microsoft.Exchange.Data.DaysOfWeek.Wednesday)
						{
							return 3;
						}
						break;
					}
				}
				else
				{
					if (daysOfWeek == Microsoft.Exchange.Data.DaysOfWeek.Thursday)
					{
						return 4;
					}
					if (daysOfWeek == Microsoft.Exchange.Data.DaysOfWeek.Friday)
					{
						return 5;
					}
					if (daysOfWeek == Microsoft.Exchange.Data.DaysOfWeek.Saturday)
					{
						return 6;
					}
				}
				throw new CalendarExceptionInvalidPropertyState(new ExceptionPropertyUri(ExceptionPropertyUriEnum.DaysOfWeek));
			}

			private static string ToString(short daysOfWeek)
			{
				switch (daysOfWeek)
				{
				case 0:
					return "Sunday";
				case 1:
					return "Monday";
				case 2:
					return "Tuesday";
				case 3:
					return "Wednesday";
				case 4:
					return "Thursday";
				case 5:
					return "Friday";
				case 6:
					return "Saturday";
				default:
					throw new CalendarExceptionInvalidPropertyState(new ExceptionPropertyUri(ExceptionPropertyUriEnum.DaysOfWeek));
				}
			}

			private static string ToString(Microsoft.Exchange.Data.DaysOfWeek daysOfWeek, RecurrenceHelper.DaysOfWeek.Kind kind)
			{
				if (kind != RecurrenceHelper.DaysOfWeek.Kind.SetOfPrimary)
				{
					if (daysOfWeek <= Microsoft.Exchange.Data.DaysOfWeek.Wednesday)
					{
						switch (daysOfWeek)
						{
						case Microsoft.Exchange.Data.DaysOfWeek.Sunday:
							return "Sunday";
						case Microsoft.Exchange.Data.DaysOfWeek.Monday:
							return "Monday";
						case Microsoft.Exchange.Data.DaysOfWeek.Sunday | Microsoft.Exchange.Data.DaysOfWeek.Monday:
							break;
						case Microsoft.Exchange.Data.DaysOfWeek.Tuesday:
							return "Tuesday";
						default:
							if (daysOfWeek == Microsoft.Exchange.Data.DaysOfWeek.Wednesday)
							{
								return "Wednesday";
							}
							break;
						}
					}
					else
					{
						if (daysOfWeek == Microsoft.Exchange.Data.DaysOfWeek.Thursday)
						{
							return "Thursday";
						}
						if (daysOfWeek == Microsoft.Exchange.Data.DaysOfWeek.Friday)
						{
							return "Friday";
						}
						if (daysOfWeek == Microsoft.Exchange.Data.DaysOfWeek.Saturday)
						{
							return "Saturday";
						}
					}
					if (kind == RecurrenceHelper.DaysOfWeek.Kind.Extended)
					{
						if (daysOfWeek == Microsoft.Exchange.Data.DaysOfWeek.Weekdays)
						{
							return "Weekday";
						}
						if (daysOfWeek == Microsoft.Exchange.Data.DaysOfWeek.WeekendDays)
						{
							return "WeekendDay";
						}
						if (daysOfWeek == Microsoft.Exchange.Data.DaysOfWeek.AllDays)
						{
							return "Day";
						}
					}
					throw new CalendarExceptionInvalidPropertyState(new ExceptionPropertyUri(ExceptionPropertyUriEnum.DaysOfWeek));
				}
				StringBuilder stringBuilder = new StringBuilder();
				int i = 1;
				while (i <= 64)
				{
					stringBuilder.Append(" ");
					Microsoft.Exchange.Data.DaysOfWeek daysOfWeek2 = daysOfWeek & (Microsoft.Exchange.Data.DaysOfWeek)i;
					if (daysOfWeek2 <= Microsoft.Exchange.Data.DaysOfWeek.Wednesday)
					{
						switch (daysOfWeek2)
						{
						case Microsoft.Exchange.Data.DaysOfWeek.Sunday:
							stringBuilder.Append("Sunday");
							break;
						case Microsoft.Exchange.Data.DaysOfWeek.Monday:
							stringBuilder.Append("Monday");
							break;
						case Microsoft.Exchange.Data.DaysOfWeek.Sunday | Microsoft.Exchange.Data.DaysOfWeek.Monday:
							goto IL_B9;
						case Microsoft.Exchange.Data.DaysOfWeek.Tuesday:
							stringBuilder.Append("Tuesday");
							break;
						default:
							if (daysOfWeek2 != Microsoft.Exchange.Data.DaysOfWeek.Wednesday)
							{
								goto IL_B9;
							}
							stringBuilder.Append("Wednesday");
							break;
						}
					}
					else if (daysOfWeek2 != Microsoft.Exchange.Data.DaysOfWeek.Thursday)
					{
						if (daysOfWeek2 != Microsoft.Exchange.Data.DaysOfWeek.Friday)
						{
							if (daysOfWeek2 != Microsoft.Exchange.Data.DaysOfWeek.Saturday)
							{
								goto IL_B9;
							}
							stringBuilder.Append("Saturday");
						}
						else
						{
							stringBuilder.Append("Friday");
						}
					}
					else
					{
						stringBuilder.Append("Thursday");
					}
					IL_C7:
					i *= 2;
					continue;
					IL_B9:
					stringBuilder.Length--;
					goto IL_C7;
				}
				if (stringBuilder.Length > 0)
				{
					return stringBuilder.ToString(1, stringBuilder.Length - 1);
				}
				return null;
			}

			private static Microsoft.Exchange.Data.DaysOfWeek Parse(string dayOfWeekName, RecurrenceHelper.DaysOfWeek.Kind kind)
			{
				switch (dayOfWeekName)
				{
				case "Sunday":
					return Microsoft.Exchange.Data.DaysOfWeek.Sunday;
				case "Monday":
					return Microsoft.Exchange.Data.DaysOfWeek.Monday;
				case "Tuesday":
					return Microsoft.Exchange.Data.DaysOfWeek.Tuesday;
				case "Wednesday":
					return Microsoft.Exchange.Data.DaysOfWeek.Wednesday;
				case "Thursday":
					return Microsoft.Exchange.Data.DaysOfWeek.Thursday;
				case "Friday":
					return Microsoft.Exchange.Data.DaysOfWeek.Friday;
				case "Saturday":
					return Microsoft.Exchange.Data.DaysOfWeek.Saturday;
				}
				if (kind == RecurrenceHelper.DaysOfWeek.Kind.Extended || kind == RecurrenceHelper.DaysOfWeek.Kind.SetOfPrimary)
				{
					if (dayOfWeekName != null)
					{
						if (dayOfWeekName == "Day")
						{
							return Microsoft.Exchange.Data.DaysOfWeek.AllDays;
						}
						if (dayOfWeekName == "Weekday")
						{
							return Microsoft.Exchange.Data.DaysOfWeek.Weekdays;
						}
						if (dayOfWeekName == "WeekendDay")
						{
							return Microsoft.Exchange.Data.DaysOfWeek.WeekendDays;
						}
					}
					if (kind == RecurrenceHelper.DaysOfWeek.Kind.SetOfPrimary)
					{
						throw new CalendarExceptionInvalidDayForWeeklyRecurrence(new ExceptionPropertyUri(ExceptionPropertyUriEnum.DaysOfWeek));
					}
					throw new CalendarExceptionInvalidPropertyValue(new ExceptionPropertyUri(ExceptionPropertyUriEnum.DaysOfWeek));
				}
				else
				{
					if (kind == RecurrenceHelper.DaysOfWeek.Kind.Primary)
					{
						throw new CalendarExceptionInvalidDayForWeeklyRecurrence(new ExceptionPropertyUri(ExceptionPropertyUriEnum.DaysOfWeek));
					}
					if (kind == RecurrenceHelper.DaysOfWeek.Kind.PrimaryStartDayOfWeek)
					{
						throw new ServiceInvalidOperationException((CoreResources.IDs)3284680126U);
					}
					throw new CalendarExceptionInvalidDayForTimeChangePattern(new ExceptionPropertyUri(ExceptionPropertyUriEnum.DaysOfWeek));
				}
			}

			private const string AllDays = "Day";

			private const string Weekdays = "Weekday";

			private const string WeekendDays = "WeekendDay";

			private const string Sunday = "Sunday";

			private const string Monday = "Monday";

			private const string Tuesday = "Tuesday";

			private const string Wednesday = "Wednesday";

			private const string Thursday = "Thursday";

			private const string Friday = "Friday";

			private const string Saturday = "Saturday";

			private static char[] dayNamesSeparator = new char[]
			{
				' '
			};

			public enum Kind
			{
				Primary,
				PrimaryTimeChangePattern,
				PrimaryStartDayOfWeek,
				Extended,
				SetOfPrimary
			}
		}

		private static class YearlyThRecurrencePattern
		{
			public static bool Render(Microsoft.Exchange.Data.Storage.YearlyThRecurrencePattern yearlyThRecurrencePattern, out RecurrencePatternBaseType pattern)
			{
				if (yearlyThRecurrencePattern == null)
				{
					pattern = null;
					return false;
				}
				pattern = new RelativeYearlyRecurrencePatternType
				{
					DaysOfWeek = RecurrenceHelper.DaysOfWeek.Render(yearlyThRecurrencePattern.DaysOfWeek, RecurrenceHelper.DaysOfWeek.Kind.Extended),
					DayOfWeekIndexString = RecurrenceHelper.DayOfWeekIndex.Render((int)yearlyThRecurrencePattern.Order),
					Month = RecurrenceHelper.Month.Render(yearlyThRecurrencePattern.Month)
				};
				return true;
			}

			public static RelativeYearlyRecurrencePatternType Render(short dayOfWeek, short dayOfWeekIndex, short month)
			{
				return new RelativeYearlyRecurrencePatternType
				{
					DaysOfWeek = RecurrenceHelper.DaysOfWeek.Render(dayOfWeek),
					DayOfWeekIndexString = RecurrenceHelper.DayOfWeekIndex.Render((int)dayOfWeekIndex),
					Month = RecurrenceHelper.Month.Render((int)month)
				};
			}

			public static bool Parse(RelativeYearlyRecurrencePatternType pattern, RecurrenceHelper.DaysOfWeek.Kind dayType, out Microsoft.Exchange.Data.Storage.RecurrencePattern yearlyThRecurrencePattern)
			{
				Microsoft.Exchange.Data.DaysOfWeek daysOfWeek;
				int order;
				int month;
				if (pattern != null && RecurrenceHelper.DaysOfWeek.Parse(pattern.DaysOfWeek, dayType, out daysOfWeek) && RecurrenceHelper.DayOfWeekIndex.Parse(pattern.DayOfWeekIndexString, out order) && RecurrenceHelper.Month.Parse(pattern.Month, out month))
				{
					yearlyThRecurrencePattern = new Microsoft.Exchange.Data.Storage.YearlyThRecurrencePattern(daysOfWeek, (RecurrenceOrderType)order, month);
					return true;
				}
				yearlyThRecurrencePattern = null;
				return false;
			}
		}

		private static class YearlyRecurrencePattern
		{
			public static bool Render(Microsoft.Exchange.Data.Storage.YearlyRecurrencePattern yearlyRecurrencePattern, out RecurrencePatternBaseType pattern)
			{
				if (yearlyRecurrencePattern == null)
				{
					pattern = null;
					return false;
				}
				pattern = new AbsoluteYearlyRecurrencePatternType
				{
					DayOfMonth = yearlyRecurrencePattern.DayOfMonth,
					Month = RecurrenceHelper.Month.Render(yearlyRecurrencePattern.Month)
				};
				return true;
			}

			public static bool Parse(AbsoluteYearlyRecurrencePatternType pattern, out Microsoft.Exchange.Data.Storage.RecurrencePattern yearlyRecurrencePattern)
			{
				if (pattern != null)
				{
					int dayOfMonth = pattern.DayOfMonth;
					RecurrenceHelper.CSharpInt.Validate(dayOfMonth, "DayOfMonth", 1, 31);
					int month;
					if (RecurrenceHelper.Month.Parse(pattern.Month, out month))
					{
						yearlyRecurrencePattern = new Microsoft.Exchange.Data.Storage.YearlyRecurrencePattern(dayOfMonth, month);
						return true;
					}
				}
				yearlyRecurrencePattern = null;
				return false;
			}
		}

		private static class MonthlyThRecurrencePattern
		{
			public static bool Render(Microsoft.Exchange.Data.Storage.MonthlyThRecurrencePattern monthlyThRecurrencePattern, out RecurrencePatternBaseType pattern)
			{
				if (monthlyThRecurrencePattern == null)
				{
					pattern = null;
					return false;
				}
				pattern = new RelativeMonthlyRecurrencePatternType
				{
					Interval = monthlyThRecurrencePattern.RecurrenceInterval,
					DaysOfWeek = RecurrenceHelper.DaysOfWeek.Render(monthlyThRecurrencePattern.DaysOfWeek, RecurrenceHelper.DaysOfWeek.Kind.Extended),
					DayOfWeekIndexString = RecurrenceHelper.DayOfWeekIndex.Render((int)monthlyThRecurrencePattern.Order)
				};
				return true;
			}

			public static bool Parse(RelativeMonthlyRecurrencePatternType pattern, out Microsoft.Exchange.Data.Storage.RecurrencePattern monthlyThRecurrencePattern)
			{
				if (pattern != null)
				{
					RecurrenceHelper.CSharpInt.Validate(pattern.Interval, "Interval", 1, 99);
					Microsoft.Exchange.Data.DaysOfWeek daysOfWeek;
					int order;
					if (RecurrenceHelper.DaysOfWeek.Parse(pattern.DaysOfWeek, RecurrenceHelper.DaysOfWeek.Kind.Extended, out daysOfWeek) && RecurrenceHelper.DayOfWeekIndex.Parse(pattern.DayOfWeekIndexString, out order))
					{
						monthlyThRecurrencePattern = new Microsoft.Exchange.Data.Storage.MonthlyThRecurrencePattern(daysOfWeek, (RecurrenceOrderType)order, pattern.Interval);
						return true;
					}
				}
				monthlyThRecurrencePattern = null;
				return false;
			}
		}

		private static class MonthlyRecurrencePattern
		{
			public static bool Render(Microsoft.Exchange.Data.Storage.MonthlyRecurrencePattern monthlyRecurrencePattern, out RecurrencePatternBaseType pattern)
			{
				if (monthlyRecurrencePattern == null)
				{
					pattern = null;
					return false;
				}
				pattern = new AbsoluteMonthlyRecurrencePatternType
				{
					Interval = monthlyRecurrencePattern.RecurrenceInterval,
					DayOfMonth = monthlyRecurrencePattern.DayOfMonth
				};
				return true;
			}

			public static bool Parse(AbsoluteMonthlyRecurrencePatternType pattern, out Microsoft.Exchange.Data.Storage.RecurrencePattern monthlyRecurrencePattern)
			{
				if (pattern != null)
				{
					RecurrenceHelper.CSharpInt.Validate(pattern.Interval, "Interval", 1, 99);
					RecurrenceHelper.CSharpInt.Validate(pattern.DayOfMonth, "DayOfMonth", 1, 31);
					monthlyRecurrencePattern = new Microsoft.Exchange.Data.Storage.MonthlyRecurrencePattern(pattern.DayOfMonth, pattern.Interval);
					return true;
				}
				monthlyRecurrencePattern = null;
				return false;
			}
		}

		private static class WeeklyRecurrencePattern
		{
			public static bool Render(Microsoft.Exchange.Data.Storage.WeeklyRecurrencePattern weeklyRecurrencePattern, out RecurrencePatternBaseType pattern)
			{
				if (weeklyRecurrencePattern == null)
				{
					pattern = null;
					return false;
				}
				pattern = new WeeklyRecurrencePatternType
				{
					Interval = weeklyRecurrencePattern.RecurrenceInterval,
					DaysOfWeek = RecurrenceHelper.DaysOfWeek.Render(weeklyRecurrencePattern.DaysOfWeek, RecurrenceHelper.DaysOfWeek.Kind.SetOfPrimary),
					FirstDayOfWeek = (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010SP1) ? RecurrenceHelper.DaysOfWeek.Render((short)weeklyRecurrencePattern.FirstDayOfWeek) : null)
				};
				return true;
			}

			private static DayOfWeek? GetOWAStartDayOfWeekIfAppropriate()
			{
				if (CallContext.Current.AccessingPrincipal != null && ExchangeVersionDeterminer.MatchesLocalServerVersion(CallContext.Current.AccessingPrincipal.MailboxInfo.Location.ServerVersion))
				{
					MailboxSession mailboxIdentityMailboxSession = CallContext.Current.SessionCache.GetMailboxIdentityMailboxSession();
					UserConfiguration userConfiguration = null;
					try
					{
						try
						{
							userConfiguration = mailboxIdentityMailboxSession.UserConfigurationManager.GetFolderConfiguration("OWA.UserOptions", UserConfigurationTypes.Dictionary, mailboxIdentityMailboxSession.GetDefaultFolderId(DefaultFolderType.Configuration));
						}
						catch (ObjectNotFoundException)
						{
							return null;
						}
						object obj = userConfiguration.GetDictionary()["weekstartday"];
						if (obj is int && EnumValidator.IsValidValue<DayOfWeek>((DayOfWeek)obj))
						{
							return new DayOfWeek?((DayOfWeek)obj);
						}
						return null;
					}
					finally
					{
						if (userConfiguration != null)
						{
							userConfiguration.Dispose();
						}
					}
				}
				return null;
			}

			public static bool Parse(WeeklyRecurrencePatternType pattern, out Microsoft.Exchange.Data.Storage.RecurrencePattern weeklyRecurrencePattern)
			{
				if (pattern != null)
				{
					RecurrenceHelper.CSharpInt.Validate(pattern.Interval, "Interval", 1, 99);
					Microsoft.Exchange.Data.DaysOfWeek daysOfWeek;
					if (RecurrenceHelper.DaysOfWeek.Parse(pattern.DaysOfWeek, RecurrenceHelper.DaysOfWeek.Kind.SetOfPrimary, out daysOfWeek))
					{
						DayOfWeek? dayOfWeek = null;
						Microsoft.Exchange.Data.DaysOfWeek daysOfWeek2;
						if (RecurrenceHelper.DaysOfWeek.Parse(pattern.FirstDayOfWeek, RecurrenceHelper.DaysOfWeek.Kind.PrimaryStartDayOfWeek, out daysOfWeek2))
						{
							dayOfWeek = new DayOfWeek?((DayOfWeek)RecurrenceHelper.DaysOfWeek.ToShort(daysOfWeek2));
						}
						else
						{
							dayOfWeek = RecurrenceHelper.WeeklyRecurrencePattern.GetOWAStartDayOfWeekIfAppropriate();
						}
						if (dayOfWeek != null)
						{
							weeklyRecurrencePattern = new Microsoft.Exchange.Data.Storage.WeeklyRecurrencePattern(daysOfWeek, pattern.Interval, dayOfWeek.Value);
						}
						else
						{
							weeklyRecurrencePattern = new Microsoft.Exchange.Data.Storage.WeeklyRecurrencePattern(daysOfWeek, pattern.Interval);
						}
						return true;
					}
				}
				weeklyRecurrencePattern = null;
				return false;
			}

			private const int MinWeeklyInterval = 1;

			private const int MaxWeeklyInterval = 99;
		}

		private static class DailyRecurrencePattern
		{
			public static bool Render(Microsoft.Exchange.Data.Storage.DailyRecurrencePattern dailyRecurrencePattern, out RecurrencePatternBaseType pattern)
			{
				if (dailyRecurrencePattern == null)
				{
					pattern = null;
					return false;
				}
				pattern = new DailyRecurrencePatternType
				{
					Interval = dailyRecurrencePattern.RecurrenceInterval
				};
				return true;
			}

			public static bool Parse(DailyRecurrencePatternType pattern, out Microsoft.Exchange.Data.Storage.RecurrencePattern dailyRecurrencePattern)
			{
				if (pattern != null)
				{
					RecurrenceHelper.CSharpInt.Validate(pattern.Interval, "Interval", 1, 999);
					dailyRecurrencePattern = new Microsoft.Exchange.Data.Storage.DailyRecurrencePattern(pattern.Interval);
					return true;
				}
				dailyRecurrencePattern = null;
				return false;
			}

			private const int MinDailyInterval = 1;

			private const int MaxDailyInterval = 999;
		}

		private static class RecurrencePattern
		{
			public static RecurrencePatternBaseType Render(Microsoft.Exchange.Data.Storage.RecurrencePattern recurrencePattern)
			{
				RecurrencePatternBaseType result;
				if (RecurrenceHelper.YearlyThRecurrencePattern.Render(recurrencePattern as Microsoft.Exchange.Data.Storage.YearlyThRecurrencePattern, out result))
				{
					return result;
				}
				if (RecurrenceHelper.YearlyRecurrencePattern.Render(recurrencePattern as Microsoft.Exchange.Data.Storage.YearlyRecurrencePattern, out result))
				{
					return result;
				}
				if (RecurrenceHelper.MonthlyThRecurrencePattern.Render(recurrencePattern as Microsoft.Exchange.Data.Storage.MonthlyThRecurrencePattern, out result))
				{
					return result;
				}
				if (RecurrenceHelper.MonthlyRecurrencePattern.Render(recurrencePattern as Microsoft.Exchange.Data.Storage.MonthlyRecurrencePattern, out result))
				{
					return result;
				}
				if (RecurrenceHelper.WeeklyRecurrencePattern.Render(recurrencePattern as Microsoft.Exchange.Data.Storage.WeeklyRecurrencePattern, out result))
				{
					return result;
				}
				if (RecurrenceHelper.DailyRecurrencePattern.Render(recurrencePattern as Microsoft.Exchange.Data.Storage.DailyRecurrencePattern, out result))
				{
					return result;
				}
				if (RecurrenceHelper.DailyRegeneratingPattern.Render(recurrencePattern as Microsoft.Exchange.Data.Storage.DailyRegeneratingPattern, out result))
				{
					return result;
				}
				if (RecurrenceHelper.WeeklyRegeneratingPattern.Render(recurrencePattern as Microsoft.Exchange.Data.Storage.WeeklyRegeneratingPattern, out result))
				{
					return result;
				}
				if (RecurrenceHelper.MonthlyRegeneratingPattern.Render(recurrencePattern as Microsoft.Exchange.Data.Storage.MonthlyRegeneratingPattern, out result))
				{
					return result;
				}
				if (RecurrenceHelper.YearlyRegeneratingPattern.Render(recurrencePattern as Microsoft.Exchange.Data.Storage.YearlyRegeneratingPattern, out result))
				{
					return result;
				}
				return null;
			}

			public static bool Parse(RecurrencePatternBaseType pattern, out Microsoft.Exchange.Data.Storage.RecurrencePattern recurrencePattern)
			{
				if (RecurrenceHelper.YearlyThRecurrencePattern.Parse(pattern as RelativeYearlyRecurrencePatternType, RecurrenceHelper.DaysOfWeek.Kind.Extended, out recurrencePattern))
				{
					return true;
				}
				if (RecurrenceHelper.YearlyRecurrencePattern.Parse(pattern as AbsoluteYearlyRecurrencePatternType, out recurrencePattern))
				{
					return true;
				}
				if (RecurrenceHelper.MonthlyThRecurrencePattern.Parse(pattern as RelativeMonthlyRecurrencePatternType, out recurrencePattern))
				{
					return true;
				}
				if (RecurrenceHelper.MonthlyRecurrencePattern.Parse(pattern as AbsoluteMonthlyRecurrencePatternType, out recurrencePattern))
				{
					return true;
				}
				if (RecurrenceHelper.WeeklyRecurrencePattern.Parse(pattern as WeeklyRecurrencePatternType, out recurrencePattern))
				{
					return true;
				}
				if (RecurrenceHelper.DailyRecurrencePattern.Parse(pattern as DailyRecurrencePatternType, out recurrencePattern))
				{
					return true;
				}
				if (RecurrenceHelper.DailyRegeneratingPattern.Parse(pattern as DailyRegeneratingPatternType, out recurrencePattern))
				{
					return true;
				}
				if (RecurrenceHelper.WeeklyRegeneratingPattern.Parse(pattern as WeeklyRegeneratingPatternType, out recurrencePattern))
				{
					return true;
				}
				if (RecurrenceHelper.MonthlyRegeneratingPattern.Parse(pattern as MonthlyRegeneratingPatternType, out recurrencePattern))
				{
					return true;
				}
				if (RecurrenceHelper.YearlyRegeneratingPattern.Parse(pattern as YearlyRegeneratingPatternType, out recurrencePattern))
				{
					return true;
				}
				recurrencePattern = null;
				return false;
			}
		}

		private static class NoEndRecurrenceRange
		{
			public static bool Render(Microsoft.Exchange.Data.Storage.NoEndRecurrenceRange recurrenceRange, out RecurrenceRangeBaseType range)
			{
				if (recurrenceRange == null)
				{
					range = null;
					return false;
				}
				range = new NoEndRecurrenceRangeType
				{
					StartDate = RecurrenceHelper.SystemDateTime.Render(recurrenceRange.StartDate, RecurrenceHelper.SystemDateTime.RenderKind.Date)
				};
				return true;
			}

			public static bool Parse(ExTimeZone timezone, NoEndRecurrenceRangeType range, out Microsoft.Exchange.Data.Storage.RecurrenceRange recurrenceRange)
			{
				recurrenceRange = null;
				ExDateTime startDate;
				if (range != null && RecurrenceHelper.SystemDateTime.Parse(timezone, range.StartDate, out startDate))
				{
					recurrenceRange = new Microsoft.Exchange.Data.Storage.NoEndRecurrenceRange(startDate);
					return true;
				}
				recurrenceRange = null;
				return false;
			}
		}

		private static class EndDateRecurrenceRange
		{
			public static bool Render(Microsoft.Exchange.Data.Storage.EndDateRecurrenceRange endDateRecurrenceRange, out RecurrenceRangeBaseType range)
			{
				if (endDateRecurrenceRange == null)
				{
					range = null;
					return false;
				}
				range = new EndDateRecurrenceRangeType
				{
					StartDate = RecurrenceHelper.SystemDateTime.Render(endDateRecurrenceRange.StartDate, RecurrenceHelper.SystemDateTime.RenderKind.Date),
					EndDate = RecurrenceHelper.SystemDateTime.Render(endDateRecurrenceRange.EndDate, RecurrenceHelper.SystemDateTime.RenderKind.Date)
				};
				return true;
			}

			public static bool Parse(ExTimeZone timezone, EndDateRecurrenceRangeType range, out Microsoft.Exchange.Data.Storage.RecurrenceRange endDateRecurrenceRange)
			{
				ExDateTime exDateTime;
				ExDateTime exDateTime2;
				if (range == null || !RecurrenceHelper.SystemDateTime.Parse(timezone, range.StartDate, out exDateTime) || !RecurrenceHelper.SystemDateTime.Parse(timezone, range.EndDate, out exDateTime2))
				{
					endDateRecurrenceRange = null;
					return false;
				}
				if (exDateTime > exDateTime2)
				{
					throw new CalendarExceptionEndDateIsEarlierThanStartDate();
				}
				endDateRecurrenceRange = new Microsoft.Exchange.Data.Storage.EndDateRecurrenceRange(exDateTime, exDateTime2);
				return true;
			}
		}

		private static class NumberedRecurrenceRange
		{
			public static bool Render(Microsoft.Exchange.Data.Storage.NumberedRecurrenceRange numberedRecurrenceRange, out RecurrenceRangeBaseType range)
			{
				if (numberedRecurrenceRange == null)
				{
					range = null;
					return false;
				}
				range = new NumberedRecurrenceRangeType
				{
					StartDate = RecurrenceHelper.SystemDateTime.Render(numberedRecurrenceRange.StartDate, RecurrenceHelper.SystemDateTime.RenderKind.Date),
					NumberOfOccurrences = numberedRecurrenceRange.NumberOfOccurrences
				};
				return true;
			}

			public static bool Parse(ExTimeZone timezone, NumberedRecurrenceRangeType pattern, out Microsoft.Exchange.Data.Storage.RecurrenceRange numberedRecurrenceRange)
			{
				ExDateTime startDate;
				if (pattern != null && RecurrenceHelper.SystemDateTime.Parse(timezone, pattern.StartDate, out startDate))
				{
					RecurrenceHelper.CSharpInt.Validate(pattern.NumberOfOccurrences, "NumberOfOccurrences", 1, 999);
					numberedRecurrenceRange = new Microsoft.Exchange.Data.Storage.NumberedRecurrenceRange(startDate, pattern.NumberOfOccurrences);
					return true;
				}
				numberedRecurrenceRange = null;
				return false;
			}

			private const string XmlElementNameNumberOfOccurrences = "NumberOfOccurrences";

			private const int MinNumberOfOccurrences = 1;

			private const int MaxNumberOfOccurrences = 999;
		}

		private static class RecurrenceRange
		{
			public static RecurrenceRangeBaseType Render(Microsoft.Exchange.Data.Storage.RecurrenceRange recurrenceRange)
			{
				RecurrenceRangeBaseType result;
				if (RecurrenceHelper.NoEndRecurrenceRange.Render(recurrenceRange as Microsoft.Exchange.Data.Storage.NoEndRecurrenceRange, out result))
				{
					return result;
				}
				if (RecurrenceHelper.EndDateRecurrenceRange.Render(recurrenceRange as Microsoft.Exchange.Data.Storage.EndDateRecurrenceRange, out result))
				{
					return result;
				}
				if (RecurrenceHelper.NumberedRecurrenceRange.Render(recurrenceRange as Microsoft.Exchange.Data.Storage.NumberedRecurrenceRange, out result))
				{
					return result;
				}
				return null;
			}

			public static bool Parse(RecurrenceRangeBaseType pattern, ExTimeZone timezone, out Microsoft.Exchange.Data.Storage.RecurrenceRange recurrenceRange)
			{
				try
				{
					if (RecurrenceHelper.NoEndRecurrenceRange.Parse(timezone, pattern as NoEndRecurrenceRangeType, out recurrenceRange))
					{
						return true;
					}
					if (RecurrenceHelper.EndDateRecurrenceRange.Parse(timezone, pattern as EndDateRecurrenceRangeType, out recurrenceRange))
					{
						return true;
					}
					if (RecurrenceHelper.NumberedRecurrenceRange.Parse(timezone, pattern as NumberedRecurrenceRangeType, out recurrenceRange))
					{
						return true;
					}
				}
				catch (ArgumentException innerException)
				{
					throw new InvalidValueForPropertyException(CoreResources.IDs.ErrorInvalidValueForPropertyDate, innerException);
				}
				recurrenceRange = null;
				return false;
			}
		}

		private static class TimeChangePattern
		{
			public static TimeChangeType Render(string timeZoneName, int offset, NativeMethods.SystemTime systemTime)
			{
				TimeSpan timeSpan = new TimeSpan(0, (int)systemTime.Hour, (int)systemTime.Minute, (int)systemTime.Second, (int)systemTime.Milliseconds);
				TimeChangeType timeChangeType = new TimeChangeType
				{
					TimeZoneName = (string.IsNullOrEmpty(timeZoneName) ? null : timeZoneName),
					Offset = RecurrenceHelper.TimeZoneOffset.Render(offset),
					Time = RecurrenceHelper.SystemTimeSpan.Render(timeSpan)
				};
				if (systemTime.Year == 0)
				{
					short num = (short)systemTime.Day;
					if (num == 5)
					{
						num = -1;
					}
					timeChangeType.RelativeYearlyRecurrence = RecurrenceHelper.YearlyThRecurrencePattern.Render((short)systemTime.DayOfWeek, num, (short)systemTime.Month);
				}
				else
				{
					ExDateTime dateTime = new ExDateTime(ExTimeZone.UnspecifiedTimeZone, (int)systemTime.Year, (int)systemTime.Month, (int)systemTime.Day);
					timeChangeType.AbsoluteDate = RecurrenceHelper.SystemDateTime.Render(dateTime, RecurrenceHelper.SystemDateTime.RenderKind.Date);
				}
				return timeChangeType;
			}

			public static bool Parse(TimeChangeType timeChangeType, out int offset, out NativeMethods.SystemTime systemTime, out string timeZoneName)
			{
				offset = 0;
				systemTime = default(NativeMethods.SystemTime);
				systemTime.Month = 0;
				timeZoneName = null;
				if (timeChangeType == null || string.IsNullOrEmpty(timeChangeType.Offset))
				{
					return false;
				}
				timeZoneName = timeChangeType.TimeZoneName;
				if (RecurrenceHelper.TimeZoneOffset.Parse(timeChangeType.Offset, out offset))
				{
					Microsoft.Exchange.Data.Storage.RecurrencePattern recurrencePattern;
					ExDateTime exDateTime;
					if (RecurrenceHelper.YearlyThRecurrencePattern.Parse(timeChangeType.RelativeYearlyRecurrence, RecurrenceHelper.DaysOfWeek.Kind.PrimaryTimeChangePattern, out recurrencePattern))
					{
						Microsoft.Exchange.Data.Storage.YearlyThRecurrencePattern yearlyThRecurrencePattern = (Microsoft.Exchange.Data.Storage.YearlyThRecurrencePattern)recurrencePattern;
						systemTime.Year = 0;
						if (yearlyThRecurrencePattern.Order == RecurrenceOrderType.Last)
						{
							systemTime.Day = 5;
						}
						else
						{
							systemTime.Day = (ushort)yearlyThRecurrencePattern.Order;
						}
						systemTime.DayOfWeek = (ushort)RecurrenceHelper.DaysOfWeek.ToShort(yearlyThRecurrencePattern.DaysOfWeek);
						systemTime.Month = (ushort)yearlyThRecurrencePattern.Month;
						short num;
						short num2;
						short num3;
						short num4;
						if (RecurrenceHelper.SystemTimeSpan.Parse(timeChangeType.Time, out num, out num2, out num3, out num4))
						{
							systemTime.Hour = (ushort)num;
							systemTime.Minute = (ushort)num2;
							systemTime.Second = (ushort)num3;
							systemTime.Milliseconds = (ushort)num4;
							return true;
						}
					}
					else if (RecurrenceHelper.SystemDateTime.Parse(null, timeChangeType.AbsoluteDate, out exDateTime))
					{
						systemTime.DayOfWeek = 0;
						systemTime.Year = (ushort)exDateTime.Year;
						systemTime.Month = (ushort)exDateTime.Month;
						systemTime.Day = (ushort)exDateTime.Day;
						short num5;
						short num6;
						short num7;
						short num8;
						if (RecurrenceHelper.SystemTimeSpan.Parse(timeChangeType.Time, out num5, out num6, out num7, out num8))
						{
							systemTime.Hour = (ushort)num5;
							systemTime.Minute = (ushort)num6;
							systemTime.Second = (ushort)num7;
							systemTime.Milliseconds = (ushort)num8;
							return true;
						}
					}
				}
				return false;
			}
		}

		private abstract class IntervalRecurrencePattern
		{
			protected static bool TryGetRecurrenceInterval(IntervalRecurrencePatternBaseType pattern, int minIntervalValue, int maxIntervalValue, out int recurrenceInterval)
			{
				if (pattern != null)
				{
					RecurrenceHelper.CSharpInt.Validate(pattern.Interval, "Interval", minIntervalValue, maxIntervalValue);
					recurrenceInterval = pattern.Interval;
					return true;
				}
				recurrenceInterval = maxIntervalValue;
				return false;
			}
		}

		private abstract class DailyRegeneratingPattern : RecurrenceHelper.IntervalRecurrencePattern
		{
			public static bool Parse(DailyRegeneratingPatternType pattern, out Microsoft.Exchange.Data.Storage.RecurrencePattern dailyRegeneratingPattern)
			{
				int recurrenceInterval;
				if (RecurrenceHelper.IntervalRecurrencePattern.TryGetRecurrenceInterval(pattern, RecurrenceHelper.DailyRegeneratingPattern.minIntervalValue, RecurrenceHelper.DailyRegeneratingPattern.maxIntervalValue, out recurrenceInterval))
				{
					dailyRegeneratingPattern = new Microsoft.Exchange.Data.Storage.DailyRegeneratingPattern(recurrenceInterval);
					return true;
				}
				dailyRegeneratingPattern = null;
				return false;
			}

			public static bool Render(Microsoft.Exchange.Data.Storage.IntervalRecurrencePattern intervalRecurrencePattern, out RecurrencePatternBaseType pattern)
			{
				if (intervalRecurrencePattern == null)
				{
					pattern = null;
					return false;
				}
				pattern = new DailyRegeneratingPatternType
				{
					Interval = intervalRecurrencePattern.RecurrenceInterval
				};
				return true;
			}

			private static int minIntervalValue = 1;

			private static int maxIntervalValue = 999;
		}

		private abstract class WeeklyRegeneratingPattern : RecurrenceHelper.IntervalRecurrencePattern
		{
			public static bool Parse(WeeklyRegeneratingPatternType pattern, out Microsoft.Exchange.Data.Storage.RecurrencePattern weeklyRegeneratingPattern)
			{
				int recurrenceInterval;
				if (RecurrenceHelper.IntervalRecurrencePattern.TryGetRecurrenceInterval(pattern, RecurrenceHelper.WeeklyRegeneratingPattern.minIntervalValue, RecurrenceHelper.WeeklyRegeneratingPattern.maxIntervalValue, out recurrenceInterval))
				{
					weeklyRegeneratingPattern = new Microsoft.Exchange.Data.Storage.WeeklyRegeneratingPattern(recurrenceInterval);
					return true;
				}
				weeklyRegeneratingPattern = null;
				return false;
			}

			public static bool Render(Microsoft.Exchange.Data.Storage.IntervalRecurrencePattern intervalRecurrencePattern, out RecurrencePatternBaseType pattern)
			{
				if (intervalRecurrencePattern == null)
				{
					pattern = null;
					return false;
				}
				pattern = new WeeklyRegeneratingPatternType
				{
					Interval = intervalRecurrencePattern.RecurrenceInterval
				};
				return true;
			}

			private static int minIntervalValue = 1;

			private static int maxIntervalValue = 999;
		}

		private abstract class MonthlyRegeneratingPattern : RecurrenceHelper.IntervalRecurrencePattern
		{
			public static bool Parse(MonthlyRegeneratingPatternType pattern, out Microsoft.Exchange.Data.Storage.RecurrencePattern monthlyRegeneratingPattern)
			{
				int recurrenceInterval;
				if (RecurrenceHelper.IntervalRecurrencePattern.TryGetRecurrenceInterval(pattern, RecurrenceHelper.MonthlyRegeneratingPattern.minIntervalValue, RecurrenceHelper.MonthlyRegeneratingPattern.maxIntervalValue, out recurrenceInterval))
				{
					monthlyRegeneratingPattern = new Microsoft.Exchange.Data.Storage.MonthlyRegeneratingPattern(recurrenceInterval);
					return true;
				}
				monthlyRegeneratingPattern = null;
				return false;
			}

			public static bool Render(Microsoft.Exchange.Data.Storage.MonthlyRegeneratingPattern monthlyRegeneratingPattern, out RecurrencePatternBaseType pattern)
			{
				if (monthlyRegeneratingPattern == null)
				{
					pattern = null;
					return false;
				}
				pattern = new MonthlyRegeneratingPatternType
				{
					Interval = monthlyRegeneratingPattern.RecurrenceInterval
				};
				return true;
			}

			private static int minIntervalValue = 1;

			private static int maxIntervalValue = 999;
		}

		private abstract class YearlyRegeneratingPattern : RecurrenceHelper.IntervalRecurrencePattern
		{
			public static bool Parse(YearlyRegeneratingPatternType pattern, out Microsoft.Exchange.Data.Storage.RecurrencePattern yearlyRegeneratingPattern)
			{
				int recurrenceInterval;
				if (RecurrenceHelper.IntervalRecurrencePattern.TryGetRecurrenceInterval(pattern, RecurrenceHelper.YearlyRegeneratingPattern.minIntervalValue, RecurrenceHelper.YearlyRegeneratingPattern.maxIntervalValue, out recurrenceInterval))
				{
					yearlyRegeneratingPattern = new Microsoft.Exchange.Data.Storage.YearlyRegeneratingPattern(recurrenceInterval);
					return true;
				}
				yearlyRegeneratingPattern = null;
				return false;
			}

			public static bool Render(Microsoft.Exchange.Data.Storage.YearlyRegeneratingPattern yearlyRegeneratingPattern, out RecurrencePatternBaseType pattern)
			{
				if (yearlyRegeneratingPattern == null)
				{
					pattern = null;
					return false;
				}
				pattern = new YearlyRegeneratingPatternType
				{
					Interval = yearlyRegeneratingPattern.RecurrenceInterval
				};
				return true;
			}

			private static int minIntervalValue = 1;

			private static int maxIntervalValue = 999;
		}
	}
}
