using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.ExchangeSystem
{
	[ComVisible(true)]
	[Serializable]
	public class ExCalendar : ICloneable
	{
		public Calendar InnerCalendar { get; private set; }

		public ExCalendar(Calendar calendar)
		{
			this.InnerCalendar = calendar;
		}

		public int[] Eras
		{
			get
			{
				return this.InnerCalendar.Eras;
			}
		}

		[ComVisible(false)]
		public bool IsReadOnly
		{
			get
			{
				return this.InnerCalendar.IsReadOnly;
			}
		}

		[ComVisible(false)]
		public ExDateTime MaxSupportedDateTime
		{
			get
			{
				return (ExDateTime)this.InnerCalendar.MaxSupportedDateTime;
			}
		}

		[ComVisible(false)]
		public ExDateTime MinSupportedDateTime
		{
			get
			{
				return (ExDateTime)this.InnerCalendar.MinSupportedDateTime;
			}
		}

		public int TwoDigitYearMax
		{
			get
			{
				return this.InnerCalendar.TwoDigitYearMax;
			}
			set
			{
				this.InnerCalendar.TwoDigitYearMax = value;
			}
		}

		public ExDateTime AddDays(ExDateTime time, int days)
		{
			return new ExDateTime(time.TimeZone, this.InnerCalendar.AddDays(time.LocalTime, days));
		}

		public ExDateTime AddHours(ExDateTime time, int hours)
		{
			return new ExDateTime(time.TimeZone, this.InnerCalendar.AddHours(time.LocalTime, hours));
		}

		public ExDateTime AddMilliseconds(ExDateTime time, double milliseconds)
		{
			return new ExDateTime(time.TimeZone, this.InnerCalendar.AddMilliseconds(time.LocalTime, milliseconds));
		}

		public ExDateTime AddMinutes(ExDateTime time, int minutes)
		{
			return new ExDateTime(time.TimeZone, this.InnerCalendar.AddMinutes(time.LocalTime, minutes));
		}

		public ExDateTime AddMonths(ExDateTime time, int months)
		{
			return new ExDateTime(time.TimeZone, this.InnerCalendar.AddMonths(time.LocalTime, months));
		}

		public ExDateTime AddSeconds(ExDateTime time, int seconds)
		{
			return new ExDateTime(time.TimeZone, this.InnerCalendar.AddSeconds(time.LocalTime, seconds));
		}

		public ExDateTime AddWeeks(ExDateTime time, int weeks)
		{
			return new ExDateTime(time.TimeZone, this.InnerCalendar.AddWeeks(time.LocalTime, weeks));
		}

		public ExDateTime AddYears(ExDateTime time, int years)
		{
			return new ExDateTime(time.TimeZone, this.InnerCalendar.AddYears(time.LocalTime, years));
		}

		public int GetDayOfMonth(ExDateTime time)
		{
			return this.InnerCalendar.GetDayOfMonth(time.LocalTime);
		}

		public DayOfWeek GetDayOfWeek(ExDateTime time)
		{
			return this.InnerCalendar.GetDayOfWeek(time.LocalTime);
		}

		public int GetDayOfYear(ExDateTime time)
		{
			return this.InnerCalendar.GetDayOfYear(time.LocalTime);
		}

		public int GetDaysInMonth(int year, int month)
		{
			return this.InnerCalendar.GetDaysInMonth(year, month);
		}

		public int GetDaysInMonth(int year, int month, int era)
		{
			return this.InnerCalendar.GetDaysInMonth(year, month, era);
		}

		public int GetDaysInYear(int year)
		{
			return this.InnerCalendar.GetDaysInYear(year);
		}

		public int GetDaysInYear(int year, int era)
		{
			return this.InnerCalendar.GetDaysInYear(year, era);
		}

		public int GetEra(ExDateTime time)
		{
			return this.InnerCalendar.GetEra(time.LocalTime);
		}

		public int GetHour(ExDateTime time)
		{
			return this.InnerCalendar.GetHour(time.LocalTime);
		}

		[ComVisible(false)]
		public int GetLeapMonth(int year)
		{
			return this.InnerCalendar.GetLeapMonth(year, this.InnerCalendar.GetEra(DateTime.UtcNow));
		}

		[ComVisible(false)]
		public int GetLeapMonth(int year, int era)
		{
			return this.InnerCalendar.GetLeapMonth(year, era);
		}

		public double GetMilliseconds(ExDateTime time)
		{
			return this.InnerCalendar.GetMilliseconds(time.LocalTime);
		}

		public int GetMinute(ExDateTime time)
		{
			return this.InnerCalendar.GetMinute(time.LocalTime);
		}

		public int GetMonth(ExDateTime time)
		{
			return this.InnerCalendar.GetMonth(time.LocalTime);
		}

		public int GetMonthsInYear(int year)
		{
			return this.InnerCalendar.GetMonthsInYear(year);
		}

		public int GetMonthsInYear(int year, int era)
		{
			return this.InnerCalendar.GetMonthsInYear(year, era);
		}

		public int GetSecond(ExDateTime time)
		{
			return this.InnerCalendar.GetSecond(time.LocalTime);
		}

		public int GetWeekOfYear(ExDateTime time, CalendarWeekRule rule, DayOfWeek firstDayOfWeek)
		{
			return this.InnerCalendar.GetWeekOfYear(time.LocalTime, rule, firstDayOfWeek);
		}

		public int GetYear(ExDateTime time)
		{
			return this.InnerCalendar.GetYear(time.LocalTime);
		}

		public bool IsLeapDay(int year, int month, int day)
		{
			return this.InnerCalendar.IsLeapDay(year, month, day);
		}

		public bool IsLeapDay(int year, int month, int day, int era)
		{
			return this.InnerCalendar.IsLeapDay(year, month, day, era);
		}

		public bool IsLeapMonth(int year, int month)
		{
			return this.InnerCalendar.IsLeapMonth(year, month);
		}

		public bool IsLeapMonth(int year, int month, int era)
		{
			return this.InnerCalendar.IsLeapMonth(year, month, era);
		}

		public bool IsLeapYear(int year)
		{
			return this.InnerCalendar.IsLeapYear(year);
		}

		public bool IsLeapYear(int year, int era)
		{
			return this.IsLeapYear(year, era);
		}

		public ExDateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond)
		{
			return (ExDateTime)this.InnerCalendar.ToDateTime(year, month, day, hour, minute, second, millisecond);
		}

		public ExDateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era)
		{
			return (ExDateTime)this.InnerCalendar.ToDateTime(year, month, day, hour, minute, second, millisecond, era);
		}

		public int ToFourDigitYear(int year)
		{
			return this.InnerCalendar.ToFourDigitYear(year);
		}

		[ComVisible(false)]
		public CalendarAlgorithmType AlgorithmType
		{
			get
			{
				return this.InnerCalendar.AlgorithmType;
			}
		}

		[ComVisible(false)]
		public object Clone()
		{
			return new ExCalendar((Calendar)this.InnerCalendar.Clone());
		}

		[ComVisible(false)]
		public static ExCalendar ReadOnly(ExCalendar calendar)
		{
			return new ExCalendar(Calendar.ReadOnly(calendar.InnerCalendar));
		}

		public const int CurrentEra = 0;
	}
}
