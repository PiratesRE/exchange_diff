using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.Common;

namespace Microsoft.Exchange.Common
{
	[Serializable]
	public struct ScheduleInterval : IComparable, IComparable<ScheduleInterval>
	{
		public ScheduleInterval(DayOfWeek startDay, int startHour, int startMinute, DayOfWeek endDay, int endHour, int endMinute)
		{
			this.startTime = new WeekDayAndTime(startDay, startHour, startMinute / 15 * 15);
			this.endTime = new WeekDayAndTime(endDay, endHour, endMinute / 15 * 15);
		}

		public ScheduleInterval(WeekDayAndTime startTime, WeekDayAndTime endTime)
		{
			this.startTime = startTime.AlignToMinutes(15);
			this.endTime = endTime.AlignToMinutes(15);
		}

		internal ScheduleInterval(DateTime start, DateTime end)
		{
			this.startTime = new WeekDayAndTime(start.DayOfWeek, start.Hour, start.Minute);
			this.endTime = new WeekDayAndTime(end.DayOfWeek, end.Hour, end.Minute);
		}

		public WeekDayAndTime StartTime
		{
			get
			{
				return this.startTime;
			}
		}

		public WeekDayAndTime EndTime
		{
			get
			{
				return this.endTime;
			}
		}

		public DayOfWeek StartDay
		{
			get
			{
				return this.startTime.DayOfWeek;
			}
		}

		public int StartHour
		{
			get
			{
				return this.startTime.Hour;
			}
		}

		public int StartMinute
		{
			get
			{
				return this.startTime.Minute;
			}
		}

		public DayOfWeek EndDay
		{
			get
			{
				return this.endTime.DayOfWeek;
			}
		}

		public int EndHour
		{
			get
			{
				return this.endTime.Hour;
			}
		}

		public int EndMinute
		{
			get
			{
				return this.endTime.Minute;
			}
		}

		public TimeSpan Length
		{
			get
			{
				return this.endTime - this.startTime;
			}
		}

		public static ScheduleInterval Parse(string s)
		{
			ExTraceGlobals.ScheduleIntervalTracer.TraceDebug<string>(19305, 0L, "Parse called with string {0}", s);
			if (s.Length < 6)
			{
				throw new FormatException(CommonStrings.InvalidScheduleIntervalFormat);
			}
			DateTimeFormatInfo dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
			if (!dateTimeFormat.Calendar.IsReadOnly)
			{
				dateTimeFormat.Calendar = ScheduleInterval.internalCalendar;
			}
			string[] formats = new string[]
			{
				"t",
				"HH:mm",
				"H:mm",
				"HH:mm tt",
				"H:mm tt",
				CultureInfo.InvariantCulture.DateTimeFormat.ShortTimePattern
			};
			DayOfWeek dayOfWeek = ScheduleInterval.ParseDayOfWeek(ref s, dateTimeFormat);
			string s2 = s;
			string text = null;
			int num = s.IndexOf('-');
			if (num > 0)
			{
				s2 = s.Substring(0, num);
				text = s.Substring(num + 1);
			}
			CultureInfo cultureInfo = null;
			DateTime dateTime;
			if (!DateTime.TryParseExact(s2, formats, null, DateTimeStyles.NoCurrentDateDefault, out dateTime))
			{
				cultureInfo = ScheduleInterval.GetCultureInfo("en-US");
				dateTime = DateTime.ParseExact(s2, formats, cultureInfo, DateTimeStyles.NoCurrentDateDefault);
			}
			DayOfWeek endDay = dayOfWeek;
			DateTime dateTime2 = dateTime;
			if (text != null)
			{
				try
				{
					endDay = ScheduleInterval.ParseDayOfWeek(ref text, dateTimeFormat);
				}
				catch (FormatException)
				{
					ExTraceGlobals.ScheduleIntervalTracer.TraceDebug(27497, 0L, "Schedule has no end date.");
				}
				if (!DateTime.TryParseExact(text, formats, null, DateTimeStyles.NoCurrentDateDefault, out dateTime2))
				{
					cultureInfo = (cultureInfo ?? ScheduleInterval.GetCultureInfo("en-US"));
					dateTime2 = DateTime.ParseExact(text, formats, cultureInfo, DateTimeStyles.NoCurrentDateDefault);
				}
			}
			return new ScheduleInterval(dayOfWeek, dateTime.Hour, dateTime.Minute, endDay, dateTime2.Hour, dateTime2.Minute);
		}

		public static byte[] GetWeekBitmapFromIntervals(ScheduleInterval[] intervals)
		{
			DateTime weekBitmapReference = ScheduleInterval.WeekBitmapReference;
			byte[] array = new byte[84];
			foreach (ScheduleInterval scheduleInterval in intervals)
			{
				WeekDayAndTime t = scheduleInterval.StartTime.ToUniversalTime(weekBitmapReference);
				WeekDayAndTime t2 = scheduleInterval.EndTime.ToUniversalTime(weekBitmapReference);
				if (t <= t2)
				{
					ScheduleInterval.UpdateBitmap((int)t.DayOfWeek, t.Hour, t.Minute, (int)t2.DayOfWeek, t2.Hour, t2.Minute, array);
				}
				else
				{
					ScheduleInterval.UpdateBitmap((int)t.DayOfWeek, t.Hour, t.Minute, 6, 23, 60, array);
					ScheduleInterval.UpdateBitmap(0, 0, 0, (int)t2.DayOfWeek, t2.Hour, t2.Minute, array);
				}
			}
			return array;
		}

		public static ScheduleInterval[] GetIntervalsFromWeekBitmap(byte[] week)
		{
			DateTime weekBitmapReference = ScheduleInterval.WeekBitmapReference;
			TimeSpan utcOffset = TimeZoneInfo.Local.GetUtcOffset(weekBitmapReference);
			int num = utcOffset.Hours * 4 + utcOffset.Minutes / 15;
			List<ScheduleInterval> list = new List<ScheduleInterval>();
			int num2 = 0;
			bool flag = false;
			for (int i = 0; i < 672; i++)
			{
				int num3 = (i - num + 672) % 672;
				int num4 = num3 / 8;
				int num5 = num3 % 8;
				if ((week[num4] & (byte)(1 << 7 - num5)) > 0 && !flag)
				{
					flag = true;
					num2 = i;
				}
				else if ((week[num4] & (byte)(1 << 7 - num5)) == 0 && flag)
				{
					flag = false;
					int num6 = i;
					ScheduleInterval item = new ScheduleInterval((DayOfWeek)(num2 / 96), (num2 - num2 / 96 * 96) / 4, (num2 - num2 / 4 * 4) * 15, (DayOfWeek)(num6 / 96), (num6 - num6 / 96 * 96) / 4, (num6 - num6 / 4 * 4) * 15);
					list.Add(item);
				}
			}
			if (flag)
			{
				DayOfWeek endDay = DayOfWeek.Sunday;
				int endHour = 0;
				int endMinute = 0;
				int num7 = (672 - num) % 672;
				if (((int)week[num7 / 8] & 1 << 7 - num7 % 8) > 0)
				{
					if (list.Count > 0)
					{
						ScheduleInterval scheduleInterval = list[0];
						list.RemoveAt(0);
						endDay = scheduleInterval.EndDay;
						endHour = scheduleInterval.EndHour;
						endMinute = scheduleInterval.EndMinute;
					}
					else
					{
						list.Add(new ScheduleInterval(DayOfWeek.Sunday, 0, 0, DayOfWeek.Saturday, 23, 45));
						num2 = 671;
					}
				}
				list.Add(new ScheduleInterval((DayOfWeek)(num2 / 96), (num2 - num2 / 96 * 96) / 4, (num2 - num2 / 4 * 4) * 15, endDay, endHour, endMinute));
			}
			return list.ToArray();
		}

		public override string ToString()
		{
			return this.startTime.ToString() + "-" + this.endTime.ToString();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ScheduleInterval))
			{
				return false;
			}
			ScheduleInterval scheduleInterval = (ScheduleInterval)obj;
			return this.StartTime == scheduleInterval.StartTime && this.EndTime == scheduleInterval.EndTime;
		}

		public bool ConjointWith(ScheduleInterval other)
		{
			return this.startTime == other.endTime || this.endTime == other.startTime;
		}

		public bool Overlaps(ScheduleInterval other)
		{
			return this.Contains(other.startTime) || other.Contains(this.startTime);
		}

		public bool Contains(WeekDayAndTime dt)
		{
			if (this.startTime <= this.endTime)
			{
				return this.startTime <= dt && dt < this.endTime;
			}
			return this.endTime > dt || dt >= this.startTime;
		}

		public bool Contains(DateTime dt)
		{
			return this.Contains(new WeekDayAndTime(dt));
		}

		public bool Contains(DayOfWeek day, int hour, int minute)
		{
			return this.Contains(new WeekDayAndTime(day, hour, minute));
		}

		public override int GetHashCode()
		{
			return this.startTime.GetHashCode() ^ this.endTime.GetHashCode();
		}

		public int CompareTo(object value)
		{
			if (value is ScheduleInterval)
			{
				return this.CompareTo((ScheduleInterval)value);
			}
			throw new ArgumentException(CommonStrings.InvalidTypeToCompare);
		}

		public int CompareTo(ScheduleInterval interval)
		{
			int num = this.StartTime.CompareTo(interval.StartTime);
			if (num == 0)
			{
				return this.Length.CompareTo(interval.Length);
			}
			return num;
		}

		private static DayOfWeek ParseDayOfWeek(ref string s, DateTimeFormatInfo dtfi)
		{
			ExTraceGlobals.ScheduleIntervalTracer.TraceDebug<string>(23401, 0L, "ParseDayOfWeek called with string {0}", s);
			if (s.Length < 2)
			{
				throw new FormatException(CommonStrings.InvalidScheduleIntervalFormat);
			}
			if (char.IsDigit(s[0]) && s[1] == '.')
			{
				string text = s.Substring(0, 1);
				s = s.Substring(2);
				ExTraceGlobals.ScheduleIntervalTracer.TraceDebug<string>(31593, 0L, "ParseDayOfWeek matched {0}", text);
				return (DayOfWeek)int.Parse(text);
			}
			string[] abbreviatedDayNames = dtfi.AbbreviatedDayNames;
			string[] dayNames = dtfi.DayNames;
			for (DayOfWeek dayOfWeek = DayOfWeek.Sunday; dayOfWeek < (DayOfWeek)7; dayOfWeek++)
			{
				string text2 = abbreviatedDayNames[(int)dayOfWeek];
				if (s.Length > text2.Length && s[text2.Length] == '.' && s.StartsWith(text2, StringComparison.OrdinalIgnoreCase))
				{
					s = s.Substring(text2.Length + 1);
					ExTraceGlobals.ScheduleIntervalTracer.TraceDebug<string>(17257, 0L, "ParseDayOfWeek matched {0}", text2);
					return dayOfWeek;
				}
				text2 = dayNames[(int)dayOfWeek];
				if (s.Length > text2.Length && s[text2.Length] == '.' && s.StartsWith(text2, StringComparison.OrdinalIgnoreCase))
				{
					s = s.Substring(text2.Length + 1);
					ExTraceGlobals.ScheduleIntervalTracer.TraceDebug<string>(25449, 0L, "ParseDayOfWeek matched {0}", text2);
					return dayOfWeek;
				}
			}
			DateTimeFormatInfo dateTimeFormat = CultureInfo.InvariantCulture.DateTimeFormat;
			abbreviatedDayNames = dateTimeFormat.AbbreviatedDayNames;
			dayNames = dateTimeFormat.DayNames;
			for (DayOfWeek dayOfWeek = DayOfWeek.Sunday; dayOfWeek < (DayOfWeek)7; dayOfWeek++)
			{
				string text3 = abbreviatedDayNames[(int)dayOfWeek];
				if (s.Length > text3.Length && s[text3.Length] == '.' && s.StartsWith(text3, StringComparison.OrdinalIgnoreCase))
				{
					s = s.Substring(text3.Length + 1);
					ExTraceGlobals.ScheduleIntervalTracer.TraceDebug<string>(21353, 0L, "ParseDayOfWeek matched {0}", text3);
					return dayOfWeek;
				}
				text3 = dayNames[(int)dayOfWeek];
				if (s.Length > text3.Length && s[text3.Length] == '.' && s.StartsWith(text3, StringComparison.OrdinalIgnoreCase))
				{
					s = s.Substring(text3.Length + 1);
					ExTraceGlobals.ScheduleIntervalTracer.TraceDebug<string>(29545, 0L, "ParseDayOfWeek matched {0}", text3);
					return dayOfWeek;
				}
			}
			throw new FormatException(CommonStrings.InvalidScheduleIntervalFormat);
		}

		private static void UpdateBitmap(int startDay, int startHour, int startMinute, int endDay, int endHour, int endMinute, byte[] week)
		{
			for (int i = startDay * 12 + startHour / 2 + 1; i < endDay * 12 + endHour / 2; i++)
			{
				week[i] = byte.MaxValue;
			}
			int num = (startHour / 2 + 1) * 2 * 4 - startHour * 4 - startMinute / 15;
			byte b = (byte)((1 << num) - 1);
			num = (endHour / 2 + 1) * 2 * 4 - endHour * 4 - endMinute / 15;
			byte b2 = (byte)(255 - ((1 << num) - 1));
			if (startDay == endDay && startHour / 2 == endHour / 2)
			{
				int num2 = startDay * 12 + startHour / 2;
				week[num2] |= (b & b2);
				return;
			}
			int num3 = startDay * 12 + startHour / 2;
			week[num3] |= b;
			int num4 = endDay * 12 + endHour / 2;
			week[num4] |= b2;
		}

		private static CultureInfo GetCultureInfo(string culture)
		{
			return new CultureInfo(culture, false);
		}

		public static Calendar GetLocalCalendar()
		{
			return new GregorianCalendar(GregorianCalendarTypes.Localized);
		}

		private const int BitmapLength = 84;

		public static readonly DateTime WeekBitmapReference = new DateTime(2006, 1, 1, 0, 0, 0, DateTimeKind.Local);

		private static readonly Calendar internalCalendar = ScheduleInterval.GetLocalCalendar();

		private WeekDayAndTime startTime;

		private WeekDayAndTime endTime;
	}
}
