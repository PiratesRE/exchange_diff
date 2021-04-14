using System;
using System.Globalization;

namespace Microsoft.Exchange.Common
{
	[Serializable]
	public struct WeekDayAndTime : IComparable<WeekDayAndTime>, IComparable, IEquatable<WeekDayAndTime>
	{
		public WeekDayAndTime(DayOfWeek dayOfWeek, int hour, int minute)
		{
			if (hour < 0 || hour >= 24)
			{
				throw new ArgumentOutOfRangeException("Hour");
			}
			if (minute < 0 || minute >= 60)
			{
				throw new ArgumentOutOfRangeException("Minute");
			}
			this.dayOfWeek = dayOfWeek;
			this.hour = hour;
			this.minute = minute;
		}

		public WeekDayAndTime(DateTime dt)
		{
			this.dayOfWeek = dt.DayOfWeek;
			this.hour = dt.Hour;
			this.minute = dt.Minute;
		}

		public DayOfWeek DayOfWeek
		{
			get
			{
				return this.dayOfWeek;
			}
		}

		public int Hour
		{
			get
			{
				return this.hour;
			}
		}

		public int Minute
		{
			get
			{
				return this.minute;
			}
		}

		public TimeSpan TimeOfDay
		{
			get
			{
				return new TimeSpan(this.Hour, this.Minute, 0);
			}
		}

		public static bool operator ==(WeekDayAndTime t1, WeekDayAndTime t2)
		{
			return t1.CompareTo(t2) == 0;
		}

		public static bool operator !=(WeekDayAndTime t1, WeekDayAndTime t2)
		{
			return t1.CompareTo(t2) != 0;
		}

		public static TimeSpan operator -(WeekDayAndTime to, WeekDayAndTime from)
		{
			if (from <= to)
			{
				return TimeSpan.FromDays((double)(to.dayOfWeek - from.dayOfWeek)) + (to.TimeOfDay - from.TimeOfDay);
			}
			return TimeSpan.FromDays(7.0) - (from - to);
		}

		public static bool operator <(WeekDayAndTime t1, WeekDayAndTime t2)
		{
			return t1.CompareTo(t2) < 0;
		}

		public static bool operator >(WeekDayAndTime t1, WeekDayAndTime t2)
		{
			return t1.CompareTo(t2) > 0;
		}

		public static bool operator <=(WeekDayAndTime t1, WeekDayAndTime t2)
		{
			return t1.CompareTo(t2) <= 0;
		}

		public static bool operator >=(WeekDayAndTime t1, WeekDayAndTime t2)
		{
			return t1.CompareTo(t2) >= 0;
		}

		public int CompareTo(WeekDayAndTime t)
		{
			if (this.dayOfWeek < t.dayOfWeek)
			{
				return -1;
			}
			if (this.dayOfWeek > t.dayOfWeek)
			{
				return 1;
			}
			if (this.hour < t.hour)
			{
				return -1;
			}
			if (this.hour > t.hour)
			{
				return 1;
			}
			return this.minute - t.minute;
		}

		public int CompareTo(object t)
		{
			if (t is WeekDayAndTime)
			{
				return this.CompareTo((WeekDayAndTime)t);
			}
			throw new ArgumentException(CommonStrings.InvalidTypeToCompare);
		}

		public override int GetHashCode()
		{
			return (int)((int)this.dayOfWeek << 16 | (DayOfWeek)(this.hour << 8) | (DayOfWeek)this.minute);
		}

		public bool Equals(WeekDayAndTime t)
		{
			return this.CompareTo(t) == 0;
		}

		public override bool Equals(object t)
		{
			if (t is WeekDayAndTime)
			{
				return this.Equals((WeekDayAndTime)t);
			}
			throw new ArgumentException(CommonStrings.InvalidTypeToCompare);
		}

		public WeekDayAndTime ToUniversalTime(DateTime referenceTime)
		{
			TimeSpan utcOffset = TimeZoneInfo.Local.GetUtcOffset(referenceTime);
			DateTime dt = new DateTime(2006, 1, (int)(1 + this.dayOfWeek), this.hour, this.minute, 0, DateTimeKind.Local).Subtract(utcOffset);
			return new WeekDayAndTime(dt);
		}

		public override string ToString()
		{
			DateTimeFormatInfo dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
			if (!dateTimeFormat.Calendar.IsReadOnly)
			{
				dateTimeFormat.Calendar = ScheduleInterval.GetLocalCalendar();
			}
			return new DateTime(2006, 1, (int)(1 + this.dayOfWeek), this.hour, this.minute, 0).ToString("ddd." + dateTimeFormat.ShortTimePattern);
		}

		public WeekDayAndTime AlignToMinutes(int min)
		{
			if (min < 0 || min >= 60)
			{
				throw new ArgumentOutOfRangeException("Minute");
			}
			return new WeekDayAndTime(this.dayOfWeek, this.hour, this.minute / min * min);
		}

		private readonly DayOfWeek dayOfWeek;

		private readonly int hour;

		private readonly int minute;
	}
}
