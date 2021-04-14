using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public static class DateTimeUtilities
	{
		public static string GetJavascriptDate(ExDateTime date)
		{
			return date.ToString(DateTimeUtilities.JSDateFormat, CultureInfo.InvariantCulture);
		}

		public static string GetIsoDateFormat(ExDateTime date)
		{
			return date.ToString(DateTimeUtilities.IsoDateFormatString, CultureInfo.InvariantCulture);
		}

		public static string GetHoursFormat(string timeFormat)
		{
			if (timeFormat == null)
			{
				throw new ArgumentNullException("timeFormat");
			}
			return DateFormatParser.GetPart(timeFormat, DateFormatPart.HoursFormat);
		}

		public static string GetDaysFormat(string dateFormat)
		{
			if (dateFormat == null)
			{
				throw new ArgumentNullException("dateFormat");
			}
			return DateFormatParser.GetPart(dateFormat, DateFormatPart.DaysFormat);
		}

		public static ExDateTime ParseIsoDate(string isoDate, ExTimeZone timeZone)
		{
			if (isoDate == null)
			{
				throw new ArgumentNullException("isoDate");
			}
			if (timeZone == null)
			{
				throw new ArgumentNullException("timeZone");
			}
			if (isoDate.Length != DateTimeUtilities.IsoDateFormatString.Length)
			{
				throw new OwaParsingErrorException(string.Format(CultureInfo.InvariantCulture, "The expected length of a Iso date format is {0} but the actual length is {1}. The actual format is {2}.", new object[]
				{
					DateTimeUtilities.IsoDateFormatString.Length,
					isoDate.Length,
					isoDate
				}));
			}
			ExDateTime result;
			try
			{
				result = new ExDateTime(timeZone, DateTime.ParseExact(isoDate, DateTimeUtilities.IsoDateFormatString, DateTimeFormatInfo.InvariantInfo));
			}
			catch (ArgumentOutOfRangeException innerException)
			{
				throw new OwaParsingErrorException("The date represented by the input string, is out of range.", innerException);
			}
			catch (FormatException innerException2)
			{
				throw new OwaParsingErrorException("The date represented by the input string, is out of range.", innerException2);
			}
			return result;
		}

		public static ExDateTime[] GetWeekFromDay(ExDateTime day, DayOfWeek weekStartDay, int workDays, bool getWorkingWeek)
		{
			int num = day.DayOfWeek - weekStartDay;
			if (num < 0)
			{
				num += 7;
			}
			if (num != 0)
			{
				day = day.IncrementDays(-num);
			}
			int num2 = 7;
			if (getWorkingWeek)
			{
				num2 = 0;
				for (int i = 0; i < 7; i++)
				{
					if ((workDays >> i & 1) != 0)
					{
						num2++;
					}
				}
			}
			ExDateTime[] array;
			if (num2 == 0)
			{
				array = new ExDateTime[]
				{
					day
				};
			}
			else
			{
				array = new ExDateTime[num2];
				int num3 = 0;
				for (int j = 0; j < 7; j++)
				{
					if (getWorkingWeek)
					{
						if ((workDays >> (int)day.DayOfWeek & 1) != 0)
						{
							array[num3++] = day;
						}
					}
					else
					{
						array[num3++] = day;
					}
					day = day.IncrementDays(1);
				}
			}
			return array;
		}

		public static bool IsWorkingDay(ExDateTime date, int workDays)
		{
			return (workDays >> (int)date.DayOfWeek & 1) != 0;
		}

		public static bool IsToday(ExDateTime date)
		{
			ExDateTime localTime = DateTimeUtilities.GetLocalTime();
			return date.Year == localTime.Year && date.Month == localTime.Month && date.Day == localTime.Day;
		}

		public static string FormatDuration(int minutes)
		{
			if (minutes == 0)
			{
				return string.Format(Strings.MinuteFormat, minutes);
			}
			double num = (double)minutes % DateTimeUtilities.MinutesInWeek;
			if (num == 0.0)
			{
				num = (double)minutes / DateTimeUtilities.MinutesInWeek;
				return string.Format((1.0 < num) ? Strings.WeeksFormat : Strings.WeekFormat, num);
			}
			num = (double)minutes % DateTimeUtilities.MinutesInDay;
			if (num == 0.0)
			{
				num = (double)minutes / DateTimeUtilities.MinutesInDay;
				return string.Format((1.0 < num) ? Strings.DaysFormat : Strings.DayFormat, num);
			}
			num = (double)minutes % DateTimeUtilities.MinutesInHour;
			if (num == 0.0)
			{
				num = (double)minutes / DateTimeUtilities.MinutesInHour;
				return string.Format((1.0 < num) ? Strings.HoursFormat : Strings.HourFormat, num);
			}
			if (90 <= minutes)
			{
				num = (double)(minutes % 30);
				if (num == 0.0)
				{
					double num2 = (double)minutes / DateTimeUtilities.MinutesInHour;
					return string.Format(Strings.HoursFormat, num2);
				}
			}
			return string.Format((1 < minutes) ? Strings.MinutesFormat : Strings.MinuteFormat, minutes);
		}

		public static void SetSessionTimeZone(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			ExTimeZone timeZone = null;
			if (!ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(userContext.UserOptions.TimeZone, out timeZone))
			{
				throw new OwaInvalidOperationException("Invalid time zone name : " + userContext.UserOptions.TimeZone);
			}
			userContext.TimeZone = timeZone;
		}

		public static ExDateTime GetLocalTime()
		{
			return DateTimeUtilities.GetLocalTime(OwaContext.Current.SessionContext);
		}

		public static ExDateTime GetLocalTime(ISessionContext sessionContext)
		{
			if (sessionContext == null)
			{
				throw new ArgumentNullException("sessionContext");
			}
			return ExDateTime.GetNow(sessionContext.TimeZone);
		}

		public static int GetDayOfWeek(UserContext userContext, ExDateTime dateTime)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			return (7 + (dateTime.DayOfWeek - userContext.UserOptions.WeekStartDay)) % 7;
		}

		public static bool IsValidTimeZoneKeyName(string timeZoneKeyName)
		{
			ExTimeZone exTimeZone = null;
			return ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(timeZoneKeyName, out exTimeZone);
		}

		public static string GetLongDatePatternWithWeekDay(CultureInfo cultureInfo)
		{
			if (!DateTimeUtilities.longDatePatternWithWeekDay.ContainsKey(cultureInfo.LCID))
			{
				lock (DateTimeUtilities.lockObject)
				{
					if (!DateTimeUtilities.longDatePatternWithWeekDay.ContainsKey(cultureInfo.LCID))
					{
						string[] allDateTimePatterns = cultureInfo.DateTimeFormat.GetAllDateTimePatterns('D');
						foreach (string text in allDateTimePatterns)
						{
							if (text.Contains("ddd"))
							{
								DateTimeUtilities.longDatePatternWithWeekDay.Add(cultureInfo.LCID, text);
								break;
							}
						}
						if (!DateTimeUtilities.longDatePatternWithWeekDay.ContainsKey(cultureInfo.LCID))
						{
							DateTimeUtilities.longDatePatternWithWeekDay.Add(cultureInfo.LCID, cultureInfo.DateTimeFormat.LongDatePattern);
						}
					}
				}
			}
			return DateTimeUtilities.longDatePatternWithWeekDay[cultureInfo.LCID];
		}

		private static readonly double MinutesInHour = 60.0;

		private static readonly double MinutesInDay = DateTimeUtilities.MinutesInHour * 24.0;

		private static readonly double MinutesInWeek = DateTimeUtilities.MinutesInDay * 7.0;

		private static Dictionary<int, string> longDatePatternWithWeekDay = new Dictionary<int, string>(CultureInfo.GetCultures(CultureTypes.AllCultures).Length);

		private static object lockObject = new object();

		internal static string IsoDateFormatString = "yyyy-MM-ddTHH:mm:ss";

		internal static string JSDateFormat = "MMM dd, yyy HH:mm:ss UTC";

		public static readonly DateTime ExampleDate = new DateTime(1999, 1, 21);
	}
}
