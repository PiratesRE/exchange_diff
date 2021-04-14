using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.Common;

namespace Microsoft.Exchange.ExchangeSystem
{
	public struct ExDateTime : IComparable, IFormattable, IComparable<ExDateTime>, IEquatable<ExDateTime>
	{
		public ExDateTime(ExTimeZone timeZone, long ticks)
		{
			this = new ExDateTime(timeZone, new DateTime(ticks));
		}

		public ExDateTime(ExTimeZone timeZone, int year, int month, int day)
		{
			this = new ExDateTime(timeZone, year, month, day, 0, 0, 0, 0);
		}

		public ExDateTime(ExTimeZone timeZone, int year, int month, int day, int hour, int minute, int second)
		{
			this = new ExDateTime(timeZone, year, month, day, hour, minute, second, 0);
		}

		public ExDateTime(ExTimeZone timeZone, int year, int month, int day, int hour, int minute, int second, int millisecond)
		{
			this = new ExDateTime(timeZone, new DateTime(year, month, day, hour, minute, second, millisecond, (timeZone == ExTimeZone.UtcTimeZone) ? DateTimeKind.Utc : DateTimeKind.Unspecified));
		}

		public ExDateTime(ExTimeZone desiredTimeZone, DateTime dateTime)
		{
			if (desiredTimeZone == null)
			{
				throw new ArgumentNullException("desiredTimeZone");
			}
			this.timeZone = desiredTimeZone;
			this.universalTime = TimeLibConsts.MinSystemDateTimeValue;
			this.localTime = null;
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				if (this.timeZone == ExTimeZone.UnspecifiedTimeZone)
				{
					this.timeZone = ExTimeZone.UtcTimeZone;
				}
				else
				{
					ExTimeZoneHelperForMigrationOnly.CheckValidationLevel<ExTimeZone>(this.timeZone.Id == ExTimeZone.UtcTimeZone.Id, ExTimeZoneHelperForMigrationOnly.ValidationLevel.Mid, "ExDateTime. Invalid time zone for UTC date/time: {0}", this.timeZone);
				}
				this.localTime = null;
				this.UniversalTime = dateTime;
				return;
			}
			if (dateTime == DateTime.MinValue)
			{
				this.UniversalTime = TimeLibConsts.MinSystemDateTimeValue;
				return;
			}
			if (dateTime == DateTime.MaxValue)
			{
				this.UniversalTime = TimeLibConsts.MaxSystemDateTimeValue;
				return;
			}
			if (this.timeZone == ExTimeZone.UtcTimeZone)
			{
				this.UniversalTime = dateTime;
				this.LocalTime = this.UniversalTime;
				return;
			}
			TimeSpan value;
			if (ExDateTime.FindLeastBiasForLocalTime(this.timeZone, dateTime, out value))
			{
				this.LocalTime = dateTime;
				this.UniversalTime = dateTime.Subtract(value);
				return;
			}
			TimeSpan timeSpan = TimeSpan.MaxValue;
			int num = 0;
			while (num < 25 && timeSpan == TimeSpan.MaxValue)
			{
				dateTime = dateTime.AddHours(1.0);
				using (IEnumerator<TimeSpan> enumerator = this.timeZone.GetBiasesForLocalTime(dateTime).GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						TimeSpan timeSpan2 = enumerator.Current;
						timeSpan = timeSpan2;
					}
				}
				num++;
			}
			if (timeSpan == TimeSpan.MaxValue)
			{
				throw new InvalidOperationException(string.Format("ExDateTime constructor failed to find a rule.\nTimeZone={0}\nDateTime={1}", this.timeZone, dateTime));
			}
			this.LocalTime = dateTime;
			this.UniversalTime = dateTime.Subtract(timeSpan);
		}

		internal ExDateTime(ExTimeZone timeZone, DateTime universalTime, DateTime? localTime)
		{
			if (timeZone == null)
			{
				throw new ArgumentNullException("timeZone");
			}
			if (universalTime < TimeLibConsts.MinSystemDateTimeValue)
			{
				universalTime = TimeLibConsts.MinSystemDateTimeValue;
			}
			if (universalTime > TimeLibConsts.MaxSystemDateTimeValue)
			{
				universalTime = TimeLibConsts.MaxSystemDateTimeValue;
			}
			this.timeZone = timeZone;
			this.universalTime = universalTime;
			this.localTime = localTime;
		}

		private ExDateTime(ExTimeZone timeZone, DateTime universalTime, TimeSpan bias)
		{
			if (timeZone == null)
			{
				throw new ArgumentNullException("timeZone");
			}
			if (universalTime < TimeLibConsts.MinSystemDateTimeValue || universalTime > TimeLibConsts.MaxSystemDateTimeValue)
			{
				throw new ArgumentOutOfRangeException("universalTime");
			}
			this.localTime = null;
			this.universalTime = DateTime.MinValue;
			this.timeZone = timeZone;
			DateTime dateTime = universalTime + bias;
			TimeSpan biasForUtcTime = timeZone.GetBiasForUtcTime(universalTime);
			if (biasForUtcTime != bias)
			{
				if (ExDateTime.FindLeastBiasForLocalTime(timeZone, dateTime, out biasForUtcTime))
				{
					universalTime = dateTime - biasForUtcTime;
				}
				else
				{
					dateTime = universalTime + timeZone.GetBiasForUtcTime(universalTime);
				}
			}
			this.LocalTime = dateTime;
			this.UniversalTime = universalTime;
			this.timeZone = timeZone;
		}

		public static ExDateTime UtcNow
		{
			get
			{
				DateTime utcNow = DateTime.UtcNow;
				return new ExDateTime(ExTimeZone.UtcTimeZone, utcNow, new DateTime?(utcNow));
			}
		}

		public static ExDateTime Now
		{
			get
			{
				if (ExTimeZone.CurrentTimeZone == null)
				{
					string message = string.Format("Current time zone is null, please check server registry at HKLM:{0}\\{1}", "SYSTEM\\CurrentControlSet\\Control\\TimeZoneInformation", "TimeZoneKeyName");
					ExTraceGlobals.CommonTracer.TraceError(0L, message);
					throw new InvalidTimeZoneException(message);
				}
				return ExTimeZone.CurrentTimeZone.ConvertDateTime(ExDateTime.UtcNow);
			}
		}

		public static ExDateTime Today
		{
			get
			{
				return ExDateTime.Now.Date;
			}
		}

		public ExTimeZone TimeZone
		{
			get
			{
				if (this.timeZone == null)
				{
					this.Initialize();
				}
				return this.timeZone;
			}
		}

		public bool HasTimeZone
		{
			get
			{
				return this.TimeZone != ExTimeZone.UnspecifiedTimeZone;
			}
		}

		public TimeSpan Bias
		{
			get
			{
				return this.LocalTime - this.UniversalTime;
			}
		}

		public long UtcTicks
		{
			get
			{
				return this.UniversalTime.Ticks;
			}
		}

		public ExDateTime Date
		{
			get
			{
				return new ExDateTime(this.TimeZone, this.LocalTime.Date);
			}
		}

		public int Millisecond
		{
			get
			{
				return this.LocalTime.Millisecond;
			}
		}

		public int Second
		{
			get
			{
				return this.LocalTime.Second;
			}
		}

		public int Minute
		{
			get
			{
				return this.LocalTime.Minute;
			}
		}

		public int Hour
		{
			get
			{
				return this.LocalTime.Hour;
			}
		}

		public TimeSpan TimeOfDay
		{
			get
			{
				return this.LocalTime.TimeOfDay;
			}
		}

		public DayOfWeek DayOfWeek
		{
			get
			{
				return this.LocalTime.DayOfWeek;
			}
		}

		public int Day
		{
			get
			{
				return this.LocalTime.Day;
			}
		}

		public int DayOfYear
		{
			get
			{
				return this.LocalTime.DayOfYear;
			}
		}

		public int Month
		{
			get
			{
				return this.LocalTime.Month;
			}
		}

		public int Year
		{
			get
			{
				return this.LocalTime.Year;
			}
		}

		public DateTime UniversalTime
		{
			get
			{
				if (this.timeZone == null)
				{
					this.Initialize();
				}
				return this.universalTime;
			}
			private set
			{
				if (value == DateTime.MinValue)
				{
					value = TimeLibConsts.MinSystemDateTimeValue;
				}
				else if (value == DateTime.MaxValue)
				{
					value = TimeLibConsts.MaxSystemDateTimeValue;
				}
				else if (value < TimeLibConsts.MinSystemDateTimeValue)
				{
					ExTimeZoneHelperForMigrationOnly.CheckValidationLevel(false, ExTimeZoneHelperForMigrationOnly.ValidationLevel.High, "ExDateTime.set_UniversalTime: DateTime less than TimeLibConsts.MinSystemDateTimeValue", new object[0]);
					value = TimeLibConsts.MinSystemDateTimeValue;
				}
				else if (value > TimeLibConsts.MaxSystemDateTimeValue)
				{
					ExTimeZoneHelperForMigrationOnly.CheckValidationLevel(false, ExTimeZoneHelperForMigrationOnly.ValidationLevel.High, "ExDateTime.set_UniversalTime: DateTime greater than TimeLibConsts.MaxSystemDateTimeValue", new object[0]);
					value = TimeLibConsts.MaxSystemDateTimeValue;
				}
				this.universalTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
			}
		}

		internal DateTime LocalTime
		{
			get
			{
				if (this.localTime == null)
				{
					if (this.timeZone == null)
					{
						this.Initialize();
					}
					else
					{
						this.localTime = new DateTime?(DateTime.SpecifyKind(this.UniversalTime + this.timeZone.GetBiasForUtcTime(this.UniversalTime), DateTimeKind.Unspecified));
					}
				}
				return this.localTime.Value;
			}
			private set
			{
				this.localTime = new DateTime?(DateTime.SpecifyKind(value, DateTimeKind.Unspecified));
			}
		}

		private DateTime SystemDateTime
		{
			get
			{
				DateTime value = this.LocalTime;
				if (this.UniversalTime == TimeLibConsts.MaxSystemDateTimeValue)
				{
					value = DateTime.MaxValue;
				}
				else if (this.UniversalTime == TimeLibConsts.MinSystemDateTimeValue)
				{
					value = DateTime.MinValue;
				}
				DateTimeKind kind;
				if (this.TimeZone == ExTimeZone.UtcTimeZone)
				{
					kind = DateTimeKind.Utc;
				}
				else if (this.TimeZone == ExTimeZone.UnspecifiedTimeZone)
				{
					kind = DateTimeKind.Unspecified;
				}
				else
				{
					kind = DateTimeKind.Unspecified;
				}
				return DateTime.SpecifyKind(value, kind);
			}
		}

		public static explicit operator DateTime(ExDateTime exDateTime)
		{
			return exDateTime.SystemDateTime;
		}

		public static explicit operator DateTime?(ExDateTime? exDateTime)
		{
			if (exDateTime == null)
			{
				return null;
			}
			return new DateTime?((DateTime)exDateTime.Value);
		}

		public static DateTime[] ToDateTimeArray(ExDateTime[] exDateTime)
		{
			if (exDateTime == null)
			{
				throw new ArgumentNullException("exDateTime");
			}
			DateTime[] array = new DateTime[exDateTime.Length];
			for (int i = 0; i < exDateTime.Length; i++)
			{
				array[i] = (DateTime)exDateTime[i];
			}
			return array;
		}

		public static explicit operator ExDateTime(DateTime dateTime)
		{
			return new ExDateTime(ExTimeZone.TimeZoneFromKind(dateTime.Kind), dateTime);
		}

		public static explicit operator ExDateTime?(DateTime? dateTime)
		{
			if (dateTime == null)
			{
				return null;
			}
			return new ExDateTime?((ExDateTime)dateTime.Value);
		}

		public static bool IsValidDateTime(DateTime dateTime)
		{
			return dateTime == DateTime.MinValue || dateTime == DateTime.MaxValue || (!(dateTime < TimeLibConsts.MinSystemDateTimeValue) && !(dateTime > TimeLibConsts.MaxSystemDateTimeValue));
		}

		public static ExDateTime Parse(string s)
		{
			return (ExDateTime)DateTime.Parse(s);
		}

		public static ExDateTime Parse(string s, IFormatProvider provider)
		{
			return (ExDateTime)DateTime.Parse(s, provider);
		}

		public static ExDateTime Parse(string s, IFormatProvider provider, DateTimeStyles styles)
		{
			return (ExDateTime)DateTime.Parse(s, provider, styles);
		}

		public static ExDateTime ParseExact(string s, string format, IFormatProvider provider)
		{
			return (ExDateTime)DateTime.ParseExact(s, format, provider);
		}

		public static ExDateTime ParseExact(string s, string format, IFormatProvider provider, DateTimeStyles style)
		{
			return (ExDateTime)DateTime.ParseExact(s, format, provider, style);
		}

		public static ExDateTime ParseExact(string s, string[] formats, IFormatProvider provider, DateTimeStyles style)
		{
			return (ExDateTime)DateTime.ParseExact(s, formats, provider, style);
		}

		public static ExDateTime Parse(ExTimeZone exTimeZone, string s)
		{
			return new ExDateTime(exTimeZone, DateTime.Parse(s));
		}

		public static ExDateTime Parse(ExTimeZone exTimeZone, string s, IFormatProvider provider)
		{
			return new ExDateTime(exTimeZone, DateTime.Parse(s, provider));
		}

		public static ExDateTime Parse(ExTimeZone exTimeZone, string s, IFormatProvider provider, DateTimeStyles styles)
		{
			return new ExDateTime(exTimeZone, DateTime.Parse(s, provider, styles));
		}

		public static ExDateTime ParseExact(ExTimeZone exTimeZone, string s, string format, IFormatProvider provider)
		{
			return new ExDateTime(exTimeZone, DateTime.ParseExact(s, format, provider));
		}

		public static ExDateTime ParseExact(ExTimeZone exTimeZone, string s, string format, IFormatProvider provider, DateTimeStyles style)
		{
			return new ExDateTime(exTimeZone, DateTime.ParseExact(s, format, provider, style));
		}

		public static ExDateTime ParseExact(ExTimeZone exTimeZone, string s, string[] formats, IFormatProvider provider, DateTimeStyles style)
		{
			return new ExDateTime(exTimeZone, DateTime.ParseExact(s, formats, provider, style));
		}

		public static bool TryParse(string s, out ExDateTime result)
		{
			DateTime dateTime;
			bool flag = DateTime.TryParse(s, out dateTime);
			result = (flag ? ((ExDateTime)dateTime) : ExDateTime.MinValue);
			return flag;
		}

		public static bool TryParse(string s, IFormatProvider provider, DateTimeStyles styles, out ExDateTime result)
		{
			DateTime dateTime;
			bool flag = DateTime.TryParse(s, provider, styles, out dateTime);
			result = (flag ? ((ExDateTime)dateTime) : ExDateTime.MinValue);
			return flag;
		}

		public static bool TryParseExact(string s, string format, IFormatProvider provider, DateTimeStyles style, out ExDateTime result)
		{
			DateTime dateTime;
			bool flag = DateTime.TryParseExact(s, format, provider, style, out dateTime);
			result = (flag ? ((ExDateTime)dateTime) : ExDateTime.MinValue);
			return flag;
		}

		public static bool TryParseExact(string s, string[] formats, IFormatProvider provider, DateTimeStyles style, out ExDateTime result)
		{
			DateTime dateTime;
			bool flag = DateTime.TryParseExact(s, formats, provider, style, out dateTime);
			result = (flag ? ((ExDateTime)dateTime) : ExDateTime.MinValue);
			return flag;
		}

		public static bool TryParse(ExTimeZone exTimeZone, string s, out ExDateTime result)
		{
			DateTime dateTime;
			bool flag = DateTime.TryParse(s, out dateTime);
			result = (flag ? new ExDateTime(exTimeZone, dateTime) : exTimeZone.ConvertDateTime(ExDateTime.MinValue));
			return flag;
		}

		public static bool TryParse(ExTimeZone exTimeZone, string s, IFormatProvider provider, DateTimeStyles styles, out ExDateTime result)
		{
			DateTime dateTime;
			bool flag = DateTime.TryParse(s, provider, styles, out dateTime);
			result = (flag ? new ExDateTime(exTimeZone, dateTime) : exTimeZone.ConvertDateTime(ExDateTime.MinValue));
			return flag;
		}

		public static bool TryParseExact(ExTimeZone exTimeZone, string s, string format, IFormatProvider provider, DateTimeStyles style, out ExDateTime result)
		{
			DateTime dateTime;
			bool flag = DateTime.TryParseExact(s, format, provider, style, out dateTime);
			result = (flag ? new ExDateTime(exTimeZone, dateTime) : exTimeZone.ConvertDateTime(ExDateTime.MinValue));
			return flag;
		}

		public static bool TryParseExact(ExTimeZone exTimeZone, string s, string[] formats, IFormatProvider provider, DateTimeStyles style, out ExDateTime result)
		{
			DateTime dateTime;
			bool flag = DateTime.TryParseExact(s, formats, provider, style, out dateTime);
			result = (flag ? new ExDateTime(exTimeZone, dateTime) : exTimeZone.ConvertDateTime(ExDateTime.MinValue));
			return flag;
		}

		public static ExDateTime FromBinary(long dateData)
		{
			return (ExDateTime)DateTime.FromBinary(dateData);
		}

		public static ExDateTime FromFileTimeUtc(long fileTime)
		{
			return new ExDateTime(ExTimeZone.UtcTimeZone, DateTime.FromFileTimeUtc(fileTime));
		}

		public static bool Equals(ExDateTime dt1, ExDateTime dt2)
		{
			return ExDateTime.Compare(dt1, dt2) == 0;
		}

		public static ExDateTime GetNow(ExTimeZone timeZone)
		{
			ExDateTime utcNow = ExDateTime.UtcNow;
			if (timeZone != null)
			{
				return timeZone.ConvertDateTime(utcNow);
			}
			return utcNow;
		}

		public static ExDateTime GetToday(ExTimeZone timeZone)
		{
			return ExDateTime.GetNow(timeZone).Date;
		}

		public static int DaysInMonth(int year, int month)
		{
			return DateTime.DaysInMonth(year, month);
		}

		public static bool IsLeapYear(int year)
		{
			return DateTime.IsLeapYear(year);
		}

		public static TimeSpan operator -(ExDateTime dt1, ExDateTime dt2)
		{
			return ExDateTime.TimeDiff(dt1, dt2);
		}

		public static ExDateTime operator -(ExDateTime d, TimeSpan t)
		{
			return d.AddTicks(-t.Ticks);
		}

		public static bool operator !=(ExDateTime d1, ExDateTime d2)
		{
			return ExDateTime.Compare(d1, d2) != 0;
		}

		public static ExDateTime operator +(ExDateTime d, TimeSpan t)
		{
			return d.AddTicks(t.Ticks);
		}

		public static bool operator <(ExDateTime t1, ExDateTime t2)
		{
			return ExDateTime.Compare(t1, t2) < 0;
		}

		public static bool operator <=(ExDateTime t1, ExDateTime t2)
		{
			return ExDateTime.Compare(t1, t2) <= 0;
		}

		public static bool operator ==(ExDateTime d1, ExDateTime d2)
		{
			return ExDateTime.Compare(d1, d2) == 0;
		}

		public static bool operator >(ExDateTime t1, ExDateTime t2)
		{
			return ExDateTime.Compare(t1, t2) > 0;
		}

		public static bool operator >=(ExDateTime t1, ExDateTime t2)
		{
			return ExDateTime.Compare(t1, t2) >= 0;
		}

		public static ExDateTime CreatedNormalizedExDateTime(ExTimeZone timeZone, DateTime universalTime, TimeSpan bias)
		{
			return new ExDateTime(timeZone, universalTime + bias);
		}

		public static IList<ExDateTime> Create(ExTimeZone timeZone, DateTime dateTime)
		{
			if (timeZone == null)
			{
				throw new ArgumentNullException("timeZone");
			}
			List<ExDateTime> list = new List<ExDateTime>(2);
			foreach (TimeSpan value in timeZone.GetBiasesForLocalTime(dateTime))
			{
				list.Add(new ExDateTime(timeZone, dateTime.Subtract(value), new DateTime?(dateTime)));
			}
			return list;
		}

		public static IList<ExDateTime> Create(ExTimeZone timeZone, int year, int month, int day, int hour, int minute, int second, int millisecond)
		{
			DateTime dateTime = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Unspecified);
			return ExDateTime.Create(timeZone, dateTime);
		}

		public static int Compare(ExDateTime dt1, ExDateTime dt2)
		{
			return ExDateTime.Compare(dt1, dt2, TimeSpan.Zero);
		}

		public static int Compare(ExDateTime dt1, ExDateTime dt2, TimeSpan threshold)
		{
			DateTime dateTime;
			DateTime dateTime2;
			if (dt1.TimeZone == ExTimeZone.UnspecifiedTimeZone || dt2.TimeZone == ExTimeZone.UnspecifiedTimeZone)
			{
				ExTimeZoneHelperForMigrationOnly.CheckValidationLevel(false, ExTimeZoneHelperForMigrationOnly.ValidationLevel.Low, "ExDateTime.Compare: UnspecifiedTimeZone", new object[0]);
				dateTime = dt1.LocalTime;
				dateTime2 = dt2.LocalTime;
			}
			else
			{
				dateTime = dt1.UniversalTime;
				dateTime2 = dt2.UniversalTime;
			}
			int num = DateTime.Compare(dateTime, dateTime2);
			if (num != 0 && threshold != TimeSpan.Zero)
			{
				TimeSpan t = (num > 0) ? (dateTime - dateTime2) : (dateTime2 - dateTime);
				if (t <= threshold)
				{
					num = 0;
				}
			}
			return num;
		}

		public static ExDateTime ParseISO(string s)
		{
			return ExDateTime.Parse(s, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
		}

		public static ExDateTime ParseISO(ExTimeZone exTimeZone, string s)
		{
			return ExDateTime.Parse(exTimeZone, s, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
		}

		public static bool TryParseISO(string s, out ExDateTime result)
		{
			DateTime dateTime;
			bool flag = DateTime.TryParse(s, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out dateTime);
			result = (flag ? ((ExDateTime)dateTime) : ExDateTime.MinValue);
			return flag;
		}

		public static bool TryParseISO(ExTimeZone exTimeZone, string s, out ExDateTime result)
		{
			DateTime dateTime;
			bool flag = DateTime.TryParse(s, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out dateTime);
			result = (flag ? new ExDateTime(exTimeZone, dateTime) : exTimeZone.ConvertDateTime(ExDateTime.MinValue));
			return flag;
		}

		public void CheckExpectedTimeZone(ExTimeZone timeZone)
		{
			this.CheckExpectedTimeZone(timeZone, ExTimeZoneHelperForMigrationOnly.ValidationLevel.Mid);
		}

		public void CheckExpectedTimeZone(ExTimeZone timeZone, ExTimeZoneHelperForMigrationOnly.ValidationLevel level)
		{
			if (timeZone == null)
			{
				throw new ArgumentNullException("timeZone");
			}
			ExTimeZoneHelperForMigrationOnly.CheckValidationLevel(this.TimeZone.Id == timeZone.Id, level, "CheckExpectedTimeZone. Expected: {0}. Actual: {1}", new object[]
			{
				timeZone,
				this.TimeZone
			});
		}

		public ExDateTime ToUtc()
		{
			return ExTimeZone.UtcTimeZone.ConvertDateTime(this);
		}

		public ExDateTime AddTicks(long ticks)
		{
			long num = this.UniversalTime.Ticks + ticks;
			if (num < TimeLibConsts.MinSystemDateTimeValue.Ticks || num > TimeLibConsts.MaxSystemDateTimeValue.Ticks)
			{
				throw new ArgumentOutOfRangeException("ticks");
			}
			return new ExDateTime(this.TimeZone, this.UniversalTime.AddTicks(ticks), null);
		}

		public ExDateTime Add(TimeSpan value)
		{
			return this.AddTicks(value.Ticks);
		}

		public ExDateTime AddMilliseconds(double value)
		{
			return this.Add(TimeSpan.FromMilliseconds(value));
		}

		public ExDateTime AddSeconds(double value)
		{
			return this.Add(TimeSpan.FromSeconds(value));
		}

		public ExDateTime AddMinutes(double value)
		{
			return this.Add(TimeSpan.FromMinutes(value));
		}

		public ExDateTime AddHours(double value)
		{
			return this.Add(TimeSpan.FromHours(value));
		}

		public ExDateTime IncrementDays(int value)
		{
			return new ExDateTime(this.TimeZone, this.LocalTime.AddDays((double)value));
		}

		public ExDateTime IncrementMonths(int value)
		{
			return new ExDateTime(this.TimeZone, this.LocalTime.AddMonths(value));
		}

		public ExDateTime IncrementYears(int value)
		{
			return new ExDateTime(this.TimeZone, this.LocalTime.AddYears(value));
		}

		public TimeSpan Subtract(ExDateTime value)
		{
			return this - value;
		}

		public ExDateTime Subtract(TimeSpan value)
		{
			return this - value;
		}

		public ExDateTime AddDays(double value)
		{
			return this.IncrementDays((int)value);
		}

		public ExDateTime AddMonths(int months)
		{
			return this.IncrementMonths(months);
		}

		public ExDateTime AddYears(int value)
		{
			return this.IncrementYears(value);
		}

		public override string ToString()
		{
			return this.LocalTime.ToString();
		}

		public string ToString(IFormatProvider provider)
		{
			return this.LocalTime.ToString(provider);
		}

		public string ToString(string format)
		{
			return this.LocalTime.ToString(format);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return this.LocalTime.ToString(format, provider);
		}

		public string ToShortDateString()
		{
			return this.LocalTime.ToString("d");
		}

		public string ToShortTimeString()
		{
			return this.LocalTime.ToString("t");
		}

		public string ToLongDateString()
		{
			return this.LocalTime.ToString("D");
		}

		public string ToLongTimeString()
		{
			return this.LocalTime.ToString("T");
		}

		public string ToISOString()
		{
			if (this.TimeZone == ExTimeZone.UtcTimeZone)
			{
				string format = (this.LocalTime.Millisecond == 0) ? "{0:yyyy-MM-ddTHH:mm:ss}Z" : "{0:yyyy-MM-ddTHH:mm:ss.fff}Z";
				return string.Format(CultureInfo.InvariantCulture, format, new object[]
				{
					this.LocalTime
				});
			}
			string format2 = (this.LocalTime.Millisecond == 0) ? "{0:yyyy-MM-ddTHH:mm:ss}{1}{2:00}:{3:00}" : "{0:yyyy-MM-ddTHH:mm:ss.fff}{1}{2:00}:{3:00}";
			TimeSpan bias = this.Bias;
			return string.Format(CultureInfo.InvariantCulture, format2, new object[]
			{
				this.LocalTime,
				(bias.Ticks < 0L) ? '-' : '+',
				Math.Abs(bias.Hours),
				Math.Abs(bias.Minutes)
			});
		}

		public long ToBinary()
		{
			return this.LocalTime.ToBinary();
		}

		public long ToFileTime()
		{
			return this.LocalTime.ToFileTime();
		}

		public long ToFileTimeUtc()
		{
			return this.UniversalTime.ToFileTimeUtc();
		}

		public bool Equals(ExDateTime other)
		{
			return ExDateTime.Equals(this, other);
		}

		public override bool Equals(object other)
		{
			return other is ExDateTime && ExDateTime.Equals(this, (ExDateTime)other);
		}

		public override int GetHashCode()
		{
			return this.UtcTicks.GetHashCode() ^ this.Bias.GetHashCode();
		}

		public int CompareTo(ExDateTime other, TimeSpan threshold)
		{
			return ExDateTime.Compare(this, other, threshold);
		}

		public int CompareTo(ExDateTime other)
		{
			return ExDateTime.Compare(this, other);
		}

		public int CompareTo(object other)
		{
			if (other is ExDateTime)
			{
				return ExDateTime.Compare(this, (ExDateTime)other);
			}
			throw new ArgumentException("Invalid comparison of ExDateTime value to a different type");
		}

		internal static TimeSpan TimeDiff(ExDateTime t1, ExDateTime t2)
		{
			if (t1.TimeZone == ExTimeZone.UnspecifiedTimeZone || t2.TimeZone == ExTimeZone.UnspecifiedTimeZone)
			{
				ExTimeZoneHelperForMigrationOnly.CheckValidationLevel(false, ExTimeZoneHelperForMigrationOnly.ValidationLevel.Mid, "ExDateTime.Compare: UnspecifiedTimeZone", new object[0]);
				return TimeSpan.FromTicks(t1.LocalTime.Ticks - t2.LocalTime.Ticks);
			}
			return TimeSpan.FromTicks(t1.UtcTicks - t2.UtcTicks);
		}

		private static bool FindLeastBiasForLocalTime(ExTimeZone timeZone, DateTime originalLocalTime, out TimeSpan bestBias)
		{
			return timeZone.TimeZoneInformation.FindLeastBiasForLocalTime(originalLocalTime, out bestBias);
		}

		private void Initialize()
		{
			this.timeZone = ExTimeZone.UtcTimeZone;
			this.universalTime = ExDateTime.MinValue.UniversalTime;
			this.localTime = new DateTime?(this.universalTime);
		}

		public ExDateTime(ExTimeZone timeZone, int year, int month, int day, Calendar calendar)
		{
			this = new ExDateTime(timeZone, new DateTime(year, month, day, calendar));
		}

		public ExDateTime(ExTimeZone timeZone, int year, int month, int day, ExCalendar calendar)
		{
			this = new ExDateTime(timeZone, new DateTime(year, month, day, calendar.InnerCalendar));
		}

		public double ToOADate()
		{
			return this.UniversalTime.ToOADate();
		}

		public static ExDateTime FromOADate(double d)
		{
			return new ExDateTime(ExTimeZone.UtcTimeZone, DateTime.FromOADate(d));
		}

		public static readonly ExDateTime MaxValue = new ExDateTime(ExTimeZone.UtcTimeZone, TimeLibConsts.MaxSystemDateTimeValue, new DateTime?(TimeLibConsts.MaxSystemDateTimeValue));

		public static readonly ExDateTime MinValue = new ExDateTime(ExTimeZone.UtcTimeZone, TimeLibConsts.MinSystemDateTimeValue, new DateTime?(TimeLibConsts.MinSystemDateTimeValue));

		public static DateTime OutlookDateTimeMin = new DateTime(1601, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		public static DateTime OutlookDateTimeMax = new DateTime(4501, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		private ExTimeZone timeZone;

		private DateTime universalTime;

		private DateTime? localTime;
	}
}
