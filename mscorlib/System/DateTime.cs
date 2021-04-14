using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[__DynamicallyInvokable]
	[Serializable]
	[StructLayout(LayoutKind.Auto)]
	public struct DateTime : IComparable, IFormattable, IConvertible, ISerializable, IComparable<DateTime>, IEquatable<DateTime>
	{
		[__DynamicallyInvokable]
		public DateTime(long ticks)
		{
			if (ticks < 0L || ticks > 3155378975999999999L)
			{
				throw new ArgumentOutOfRangeException("ticks", Environment.GetResourceString("ArgumentOutOfRange_DateTimeBadTicks"));
			}
			this.dateData = (ulong)ticks;
		}

		private DateTime(ulong dateData)
		{
			this.dateData = dateData;
		}

		[__DynamicallyInvokable]
		public DateTime(long ticks, DateTimeKind kind)
		{
			if (ticks < 0L || ticks > 3155378975999999999L)
			{
				throw new ArgumentOutOfRangeException("ticks", Environment.GetResourceString("ArgumentOutOfRange_DateTimeBadTicks"));
			}
			if (kind < DateTimeKind.Unspecified || kind > DateTimeKind.Local)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidDateTimeKind"), "kind");
			}
			this.dateData = (ulong)(ticks | (long)kind << 62);
		}

		internal DateTime(long ticks, DateTimeKind kind, bool isAmbiguousDst)
		{
			if (ticks < 0L || ticks > 3155378975999999999L)
			{
				throw new ArgumentOutOfRangeException("ticks", Environment.GetResourceString("ArgumentOutOfRange_DateTimeBadTicks"));
			}
			this.dateData = (ulong)(ticks | (isAmbiguousDst ? -4611686018427387904L : long.MinValue));
		}

		[__DynamicallyInvokable]
		public DateTime(int year, int month, int day)
		{
			this.dateData = (ulong)DateTime.DateToTicks(year, month, day);
		}

		public DateTime(int year, int month, int day, Calendar calendar)
		{
			this = new DateTime(year, month, day, 0, 0, 0, calendar);
		}

		[__DynamicallyInvokable]
		public DateTime(int year, int month, int day, int hour, int minute, int second)
		{
			if (second == 60 && DateTime.s_isLeapSecondsSupportedSystem && DateTime.IsValidTimeWithLeapSeconds(year, month, day, hour, minute, second, DateTimeKind.Unspecified))
			{
				second = 59;
			}
			this.dateData = (ulong)(DateTime.DateToTicks(year, month, day) + DateTime.TimeToTicks(hour, minute, second));
		}

		[__DynamicallyInvokable]
		public DateTime(int year, int month, int day, int hour, int minute, int second, DateTimeKind kind)
		{
			if (kind < DateTimeKind.Unspecified || kind > DateTimeKind.Local)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidDateTimeKind"), "kind");
			}
			if (second == 60 && DateTime.s_isLeapSecondsSupportedSystem && DateTime.IsValidTimeWithLeapSeconds(year, month, day, hour, minute, second, kind))
			{
				second = 59;
			}
			long num = DateTime.DateToTicks(year, month, day) + DateTime.TimeToTicks(hour, minute, second);
			this.dateData = (ulong)(num | (long)kind << 62);
		}

		public DateTime(int year, int month, int day, int hour, int minute, int second, Calendar calendar)
		{
			if (calendar == null)
			{
				throw new ArgumentNullException("calendar");
			}
			int num = second;
			if (second == 60 && DateTime.s_isLeapSecondsSupportedSystem)
			{
				second = 59;
			}
			this.dateData = (ulong)calendar.ToDateTime(year, month, day, hour, minute, second, 0).Ticks;
			if (num == 60)
			{
				DateTime dateTime = new DateTime(this.dateData);
				if (!DateTime.IsValidTimeWithLeapSeconds(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 60, DateTimeKind.Unspecified))
				{
					throw new ArgumentOutOfRangeException(null, Environment.GetResourceString("ArgumentOutOfRange_BadHourMinuteSecond"));
				}
			}
		}

		[__DynamicallyInvokable]
		public DateTime(int year, int month, int day, int hour, int minute, int second, int millisecond)
		{
			if (millisecond < 0 || millisecond >= 1000)
			{
				throw new ArgumentOutOfRangeException("millisecond", Environment.GetResourceString("ArgumentOutOfRange_Range", new object[]
				{
					0,
					999
				}));
			}
			if (second == 60 && DateTime.s_isLeapSecondsSupportedSystem && DateTime.IsValidTimeWithLeapSeconds(year, month, day, hour, minute, second, DateTimeKind.Unspecified))
			{
				second = 59;
			}
			long num = DateTime.DateToTicks(year, month, day) + DateTime.TimeToTicks(hour, minute, second);
			num += (long)millisecond * 10000L;
			if (num < 0L || num > 3155378975999999999L)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_DateTimeRange"));
			}
			this.dateData = (ulong)num;
		}

		[__DynamicallyInvokable]
		public DateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, DateTimeKind kind)
		{
			if (millisecond < 0 || millisecond >= 1000)
			{
				throw new ArgumentOutOfRangeException("millisecond", Environment.GetResourceString("ArgumentOutOfRange_Range", new object[]
				{
					0,
					999
				}));
			}
			if (kind < DateTimeKind.Unspecified || kind > DateTimeKind.Local)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidDateTimeKind"), "kind");
			}
			if (second == 60 && DateTime.s_isLeapSecondsSupportedSystem && DateTime.IsValidTimeWithLeapSeconds(year, month, day, hour, minute, second, kind))
			{
				second = 59;
			}
			long num = DateTime.DateToTicks(year, month, day) + DateTime.TimeToTicks(hour, minute, second);
			num += (long)millisecond * 10000L;
			if (num < 0L || num > 3155378975999999999L)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_DateTimeRange"));
			}
			this.dateData = (ulong)(num | (long)kind << 62);
		}

		public DateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, Calendar calendar)
		{
			if (calendar == null)
			{
				throw new ArgumentNullException("calendar");
			}
			if (millisecond < 0 || millisecond >= 1000)
			{
				throw new ArgumentOutOfRangeException("millisecond", Environment.GetResourceString("ArgumentOutOfRange_Range", new object[]
				{
					0,
					999
				}));
			}
			int num = second;
			if (second == 60 && DateTime.s_isLeapSecondsSupportedSystem)
			{
				second = 59;
			}
			long num2 = calendar.ToDateTime(year, month, day, hour, minute, second, 0).Ticks;
			num2 += (long)millisecond * 10000L;
			if (num2 < 0L || num2 > 3155378975999999999L)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_DateTimeRange"));
			}
			this.dateData = (ulong)num2;
			if (num == 60)
			{
				DateTime dateTime = new DateTime(this.dateData);
				if (!DateTime.IsValidTimeWithLeapSeconds(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 60, DateTimeKind.Unspecified))
				{
					throw new ArgumentOutOfRangeException(null, Environment.GetResourceString("ArgumentOutOfRange_BadHourMinuteSecond"));
				}
			}
		}

		public DateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, Calendar calendar, DateTimeKind kind)
		{
			if (calendar == null)
			{
				throw new ArgumentNullException("calendar");
			}
			if (millisecond < 0 || millisecond >= 1000)
			{
				throw new ArgumentOutOfRangeException("millisecond", Environment.GetResourceString("ArgumentOutOfRange_Range", new object[]
				{
					0,
					999
				}));
			}
			if (kind < DateTimeKind.Unspecified || kind > DateTimeKind.Local)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidDateTimeKind"), "kind");
			}
			int num = second;
			if (second == 60 && DateTime.s_isLeapSecondsSupportedSystem)
			{
				second = 59;
			}
			long num2 = calendar.ToDateTime(year, month, day, hour, minute, second, 0).Ticks;
			num2 += (long)millisecond * 10000L;
			if (num2 < 0L || num2 > 3155378975999999999L)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_DateTimeRange"));
			}
			this.dateData = (ulong)(num2 | (long)kind << 62);
			if (num == 60)
			{
				DateTime dateTime = new DateTime(this.dateData);
				if (!DateTime.IsValidTimeWithLeapSeconds(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 60, kind))
				{
					throw new ArgumentOutOfRangeException(null, Environment.GetResourceString("ArgumentOutOfRange_BadHourMinuteSecond"));
				}
			}
		}

		private DateTime(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			bool flag = false;
			bool flag2 = false;
			long num = 0L;
			ulong num2 = 0UL;
			SerializationInfoEnumerator enumerator = info.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string name = enumerator.Name;
				if (!(name == "ticks"))
				{
					if (name == "dateData")
					{
						num2 = Convert.ToUInt64(enumerator.Value, CultureInfo.InvariantCulture);
						flag2 = true;
					}
				}
				else
				{
					num = Convert.ToInt64(enumerator.Value, CultureInfo.InvariantCulture);
					flag = true;
				}
			}
			if (flag2)
			{
				this.dateData = num2;
			}
			else
			{
				if (!flag)
				{
					throw new SerializationException(Environment.GetResourceString("Serialization_MissingDateTimeData"));
				}
				this.dateData = (ulong)num;
			}
			long internalTicks = this.InternalTicks;
			if (internalTicks < 0L || internalTicks > 3155378975999999999L)
			{
				throw new SerializationException(Environment.GetResourceString("Serialization_DateTimeTicksOutOfRange"));
			}
		}

		internal long InternalTicks
		{
			get
			{
				return (long)(this.dateData & 4611686018427387903UL);
			}
		}

		private ulong InternalKind
		{
			get
			{
				return this.dateData & 13835058055282163712UL;
			}
		}

		[__DynamicallyInvokable]
		public DateTime Add(TimeSpan value)
		{
			return this.AddTicks(value._ticks);
		}

		private DateTime Add(double value, int scale)
		{
			long num = (long)(value * (double)scale + ((value >= 0.0) ? 0.5 : -0.5));
			if (num <= -315537897600000L || num >= 315537897600000L)
			{
				throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_AddValue"));
			}
			return this.AddTicks(num * 10000L);
		}

		[__DynamicallyInvokable]
		public DateTime AddDays(double value)
		{
			return this.Add(value, 86400000);
		}

		[__DynamicallyInvokable]
		public DateTime AddHours(double value)
		{
			return this.Add(value, 3600000);
		}

		[__DynamicallyInvokable]
		public DateTime AddMilliseconds(double value)
		{
			return this.Add(value, 1);
		}

		[__DynamicallyInvokable]
		public DateTime AddMinutes(double value)
		{
			return this.Add(value, 60000);
		}

		[__DynamicallyInvokable]
		public DateTime AddMonths(int months)
		{
			if (months < -120000 || months > 120000)
			{
				throw new ArgumentOutOfRangeException("months", Environment.GetResourceString("ArgumentOutOfRange_DateTimeBadMonths"));
			}
			int num;
			int num2;
			int num3;
			this.GetDatePart(out num, out num2, out num3);
			int num4 = num2 - 1 + months;
			if (num4 >= 0)
			{
				num2 = num4 % 12 + 1;
				num += num4 / 12;
			}
			else
			{
				num2 = 12 + (num4 + 1) % 12;
				num += (num4 - 11) / 12;
			}
			if (num < 1 || num > 9999)
			{
				throw new ArgumentOutOfRangeException("months", Environment.GetResourceString("ArgumentOutOfRange_DateArithmetic"));
			}
			int num5 = DateTime.DaysInMonth(num, num2);
			if (num3 > num5)
			{
				num3 = num5;
			}
			return new DateTime((ulong)(DateTime.DateToTicks(num, num2, num3) + this.InternalTicks % 864000000000L | (long)this.InternalKind));
		}

		[__DynamicallyInvokable]
		public DateTime AddSeconds(double value)
		{
			return this.Add(value, 1000);
		}

		[__DynamicallyInvokable]
		public DateTime AddTicks(long value)
		{
			long internalTicks = this.InternalTicks;
			if (value > 3155378975999999999L - internalTicks || value < 0L - internalTicks)
			{
				throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_DateArithmetic"));
			}
			return new DateTime((ulong)(internalTicks + value | (long)this.InternalKind));
		}

		[__DynamicallyInvokable]
		public DateTime AddYears(int value)
		{
			if (value < -10000 || value > 10000)
			{
				throw new ArgumentOutOfRangeException("years", Environment.GetResourceString("ArgumentOutOfRange_DateTimeBadYears"));
			}
			return this.AddMonths(value * 12);
		}

		[__DynamicallyInvokable]
		public static int Compare(DateTime t1, DateTime t2)
		{
			long internalTicks = t1.InternalTicks;
			long internalTicks2 = t2.InternalTicks;
			if (internalTicks > internalTicks2)
			{
				return 1;
			}
			if (internalTicks < internalTicks2)
			{
				return -1;
			}
			return 0;
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is DateTime))
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_MustBeDateTime"));
			}
			long internalTicks = ((DateTime)value).InternalTicks;
			long internalTicks2 = this.InternalTicks;
			if (internalTicks2 > internalTicks)
			{
				return 1;
			}
			if (internalTicks2 < internalTicks)
			{
				return -1;
			}
			return 0;
		}

		[__DynamicallyInvokable]
		public int CompareTo(DateTime value)
		{
			long internalTicks = value.InternalTicks;
			long internalTicks2 = this.InternalTicks;
			if (internalTicks2 > internalTicks)
			{
				return 1;
			}
			if (internalTicks2 < internalTicks)
			{
				return -1;
			}
			return 0;
		}

		private static long DateToTicks(int year, int month, int day)
		{
			if (year >= 1 && year <= 9999 && month >= 1 && month <= 12)
			{
				int[] array = DateTime.IsLeapYear(year) ? DateTime.DaysToMonth366 : DateTime.DaysToMonth365;
				if (day >= 1 && day <= array[month] - array[month - 1])
				{
					int num = year - 1;
					int num2 = num * 365 + num / 4 - num / 100 + num / 400 + array[month - 1] + day - 1;
					return (long)num2 * 864000000000L;
				}
			}
			throw new ArgumentOutOfRangeException(null, Environment.GetResourceString("ArgumentOutOfRange_BadYearMonthDay"));
		}

		private static long TimeToTicks(int hour, int minute, int second)
		{
			if (hour >= 0 && hour < 24 && minute >= 0 && minute < 60 && second >= 0 && second < 60)
			{
				return TimeSpan.TimeToTicks(hour, minute, second);
			}
			throw new ArgumentOutOfRangeException(null, Environment.GetResourceString("ArgumentOutOfRange_BadHourMinuteSecond"));
		}

		[__DynamicallyInvokable]
		public static int DaysInMonth(int year, int month)
		{
			if (month < 1 || month > 12)
			{
				throw new ArgumentOutOfRangeException("month", Environment.GetResourceString("ArgumentOutOfRange_Month"));
			}
			int[] array = DateTime.IsLeapYear(year) ? DateTime.DaysToMonth366 : DateTime.DaysToMonth365;
			return array[month] - array[month - 1];
		}

		internal static long DoubleDateToTicks(double value)
		{
			if (value >= 2958466.0 || value <= -657435.0)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_OleAutDateInvalid"));
			}
			long num = (long)(value * 86400000.0 + ((value >= 0.0) ? 0.5 : -0.5));
			if (num < 0L)
			{
				num -= num % 86400000L * 2L;
			}
			num += 59926435200000L;
			if (num < 0L || num >= 315537897600000L)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_OleAutDateScale"));
			}
			return num * 10000L;
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool LegacyParseMode();

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool EnableAmPmParseAdjustment();

		[__DynamicallyInvokable]
		public override bool Equals(object value)
		{
			return value is DateTime && this.InternalTicks == ((DateTime)value).InternalTicks;
		}

		[__DynamicallyInvokable]
		public bool Equals(DateTime value)
		{
			return this.InternalTicks == value.InternalTicks;
		}

		[__DynamicallyInvokable]
		public static bool Equals(DateTime t1, DateTime t2)
		{
			return t1.InternalTicks == t2.InternalTicks;
		}

		[__DynamicallyInvokable]
		public static DateTime FromBinary(long dateData)
		{
			if ((dateData & -9223372036854775808L) == 0L)
			{
				return DateTime.FromBinaryRaw(dateData);
			}
			long num = dateData & 4611686018427387903L;
			if (num > 4611685154427387904L)
			{
				num -= 4611686018427387904L;
			}
			bool isAmbiguousDst = false;
			long ticks;
			if (num < 0L)
			{
				ticks = TimeZoneInfo.GetLocalUtcOffset(DateTime.MinValue, TimeZoneInfoOptions.NoThrowOnInvalidTime).Ticks;
			}
			else if (num > 3155378975999999999L)
			{
				ticks = TimeZoneInfo.GetLocalUtcOffset(DateTime.MaxValue, TimeZoneInfoOptions.NoThrowOnInvalidTime).Ticks;
			}
			else
			{
				DateTime time = new DateTime(num, DateTimeKind.Utc);
				bool flag = false;
				ticks = TimeZoneInfo.GetUtcOffsetFromUtc(time, TimeZoneInfo.Local, out flag, out isAmbiguousDst).Ticks;
			}
			num += ticks;
			if (num < 0L)
			{
				num += 864000000000L;
			}
			if (num < 0L || num > 3155378975999999999L)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_DateTimeBadBinaryData"), "dateData");
			}
			return new DateTime(num, DateTimeKind.Local, isAmbiguousDst);
		}

		internal static DateTime FromBinaryRaw(long dateData)
		{
			long num = dateData & 4611686018427387903L;
			if (num < 0L || num > 3155378975999999999L)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_DateTimeBadBinaryData"), "dateData");
			}
			return new DateTime((ulong)dateData);
		}

		[__DynamicallyInvokable]
		public static DateTime FromFileTime(long fileTime)
		{
			return DateTime.FromFileTimeUtc(fileTime).ToLocalTime();
		}

		[__DynamicallyInvokable]
		public static DateTime FromFileTimeUtc(long fileTime)
		{
			if (fileTime < 0L || fileTime > 2650467743999999999L)
			{
				throw new ArgumentOutOfRangeException("fileTime", Environment.GetResourceString("ArgumentOutOfRange_FileTimeInvalid"));
			}
			if (DateTime.s_isLeapSecondsSupportedSystem)
			{
				return DateTime.InternalFromFileTime(fileTime);
			}
			long ticks = fileTime + 504911232000000000L;
			return new DateTime(ticks, DateTimeKind.Utc);
		}

		[__DynamicallyInvokable]
		public static DateTime FromOADate(double d)
		{
			return new DateTime(DateTime.DoubleDateToTicks(d), DateTimeKind.Unspecified);
		}

		[SecurityCritical]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("ticks", this.InternalTicks);
			info.AddValue("dateData", this.dateData);
		}

		[__DynamicallyInvokable]
		public bool IsDaylightSavingTime()
		{
			return this.Kind != DateTimeKind.Utc && TimeZoneInfo.Local.IsDaylightSavingTime(this, TimeZoneInfoOptions.NoThrowOnInvalidTime);
		}

		[__DynamicallyInvokable]
		public static DateTime SpecifyKind(DateTime value, DateTimeKind kind)
		{
			return new DateTime(value.InternalTicks, kind);
		}

		[__DynamicallyInvokable]
		public long ToBinary()
		{
			if (this.Kind == DateTimeKind.Local)
			{
				TimeSpan localUtcOffset = TimeZoneInfo.GetLocalUtcOffset(this, TimeZoneInfoOptions.NoThrowOnInvalidTime);
				long ticks = this.Ticks;
				long num = ticks - localUtcOffset.Ticks;
				if (num < 0L)
				{
					num = 4611686018427387904L + num;
				}
				return num | long.MinValue;
			}
			return (long)this.dateData;
		}

		internal long ToBinaryRaw()
		{
			return (long)this.dateData;
		}

		[__DynamicallyInvokable]
		public DateTime Date
		{
			[__DynamicallyInvokable]
			get
			{
				long internalTicks = this.InternalTicks;
				return new DateTime((ulong)(internalTicks - internalTicks % 864000000000L | (long)this.InternalKind));
			}
		}

		private int GetDatePart(int part)
		{
			long internalTicks = this.InternalTicks;
			int i = (int)(internalTicks / 864000000000L);
			int num = i / 146097;
			i -= num * 146097;
			int num2 = i / 36524;
			if (num2 == 4)
			{
				num2 = 3;
			}
			i -= num2 * 36524;
			int num3 = i / 1461;
			i -= num3 * 1461;
			int num4 = i / 365;
			if (num4 == 4)
			{
				num4 = 3;
			}
			if (part == 0)
			{
				return num * 400 + num2 * 100 + num3 * 4 + num4 + 1;
			}
			i -= num4 * 365;
			if (part == 1)
			{
				return i + 1;
			}
			int[] array = (num4 == 3 && (num3 != 24 || num2 == 3)) ? DateTime.DaysToMonth366 : DateTime.DaysToMonth365;
			int num5 = i >> 6;
			while (i >= array[num5])
			{
				num5++;
			}
			if (part == 2)
			{
				return num5;
			}
			return i - array[num5 - 1] + 1;
		}

		internal void GetDatePart(out int year, out int month, out int day)
		{
			long internalTicks = this.InternalTicks;
			int i = (int)(internalTicks / 864000000000L);
			int num = i / 146097;
			i -= num * 146097;
			int num2 = i / 36524;
			if (num2 == 4)
			{
				num2 = 3;
			}
			i -= num2 * 36524;
			int num3 = i / 1461;
			i -= num3 * 1461;
			int num4 = i / 365;
			if (num4 == 4)
			{
				num4 = 3;
			}
			year = num * 400 + num2 * 100 + num3 * 4 + num4 + 1;
			i -= num4 * 365;
			int[] array = (num4 == 3 && (num3 != 24 || num2 == 3)) ? DateTime.DaysToMonth366 : DateTime.DaysToMonth365;
			int num5 = (i >> 5) + 1;
			while (i >= array[num5])
			{
				num5++;
			}
			month = num5;
			day = i - array[num5 - 1] + 1;
		}

		[__DynamicallyInvokable]
		public int Day
		{
			[__DynamicallyInvokable]
			get
			{
				return this.GetDatePart(3);
			}
		}

		[__DynamicallyInvokable]
		public DayOfWeek DayOfWeek
		{
			[__DynamicallyInvokable]
			get
			{
				return (DayOfWeek)((this.InternalTicks / 864000000000L + 1L) % 7L);
			}
		}

		[__DynamicallyInvokable]
		public int DayOfYear
		{
			[__DynamicallyInvokable]
			get
			{
				return this.GetDatePart(1);
			}
		}

		[__DynamicallyInvokable]
		public override int GetHashCode()
		{
			long internalTicks = this.InternalTicks;
			return (int)internalTicks ^ (int)(internalTicks >> 32);
		}

		[__DynamicallyInvokable]
		public int Hour
		{
			[__DynamicallyInvokable]
			get
			{
				return (int)(this.InternalTicks / 36000000000L % 24L);
			}
		}

		internal bool IsAmbiguousDaylightSavingTime()
		{
			return this.InternalKind == 13835058055282163712UL;
		}

		[__DynamicallyInvokable]
		public DateTimeKind Kind
		{
			[__DynamicallyInvokable]
			get
			{
				ulong internalKind = this.InternalKind;
				if (internalKind == 0UL)
				{
					return DateTimeKind.Unspecified;
				}
				if (internalKind != 4611686018427387904UL)
				{
					return DateTimeKind.Local;
				}
				return DateTimeKind.Utc;
			}
		}

		[__DynamicallyInvokable]
		public int Millisecond
		{
			[__DynamicallyInvokable]
			get
			{
				return (int)(this.InternalTicks / 10000L % 1000L);
			}
		}

		[__DynamicallyInvokable]
		public int Minute
		{
			[__DynamicallyInvokable]
			get
			{
				return (int)(this.InternalTicks / 600000000L % 60L);
			}
		}

		[__DynamicallyInvokable]
		public int Month
		{
			[__DynamicallyInvokable]
			get
			{
				return this.GetDatePart(2);
			}
		}

		[__DynamicallyInvokable]
		public static DateTime Now
		{
			[__DynamicallyInvokable]
			get
			{
				DateTime utcNow = DateTime.UtcNow;
				bool isAmbiguousDst = false;
				long ticks = TimeZoneInfo.GetDateTimeNowUtcOffsetFromUtc(utcNow, out isAmbiguousDst).Ticks;
				long num = utcNow.Ticks + ticks;
				if (num > 3155378975999999999L)
				{
					return new DateTime(3155378975999999999L, DateTimeKind.Local);
				}
				if (num < 0L)
				{
					return new DateTime(0L, DateTimeKind.Local);
				}
				return new DateTime(num, DateTimeKind.Local, isAmbiguousDst);
			}
		}

		[__DynamicallyInvokable]
		public static DateTime UtcNow
		{
			[SecuritySafeCritical]
			[__DynamicallyInvokable]
			get
			{
				if (DateTime.s_isLeapSecondsSupportedSystem)
				{
					DateTime.FullSystemTime fullSystemTime = default(DateTime.FullSystemTime);
					DateTime.GetSystemTimeWithLeapSecondsHandling(ref fullSystemTime);
					return DateTime.CreateDateTimeFromSystemTime(ref fullSystemTime);
				}
				long systemTimeAsFileTime = DateTime.GetSystemTimeAsFileTime();
				return new DateTime((ulong)(systemTimeAsFileTime + 504911232000000000L | 4611686018427387904L));
			}
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern long GetSystemTimeAsFileTime();

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool ValidateSystemTime(ref DateTime.FullSystemTime time, bool localTime);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void GetSystemTimeWithLeapSecondsHandling(ref DateTime.FullSystemTime time);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern bool IsLeapSecondsSupportedSystem();

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool SystemFileTimeToSystemTime(long fileTime, ref DateTime.FullSystemTime time);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool SystemTimeToSystemFileTime(ref DateTime.FullSystemTime time, ref long fileTime);

		[SecuritySafeCritical]
		internal static bool SystemSupportLeapSeconds()
		{
			return DateTime.IsLeapSecondsSupportedSystem();
		}

		[SecuritySafeCritical]
		internal static DateTime InternalFromFileTime(long fileTime)
		{
			DateTime.FullSystemTime fullSystemTime = default(DateTime.FullSystemTime);
			if (DateTime.SystemFileTimeToSystemTime(fileTime, ref fullSystemTime))
			{
				fullSystemTime.hundredNanoSecond = fileTime % 10000L;
				return DateTime.CreateDateTimeFromSystemTime(ref fullSystemTime);
			}
			throw new ArgumentOutOfRangeException("fileTime", Environment.GetResourceString("ArgumentOutOfRange_DateTimeBadTicks"));
		}

		[SecuritySafeCritical]
		internal static long InternalToFileTime(long ticks)
		{
			long num = 0L;
			DateTime.FullSystemTime fullSystemTime = new DateTime.FullSystemTime(ticks);
			if (DateTime.SystemTimeToSystemFileTime(ref fullSystemTime, ref num))
			{
				return num + ticks % 10000L;
			}
			throw new ArgumentOutOfRangeException(null, Environment.GetResourceString("ArgumentOutOfRange_FileTimeInvalid"));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static DateTime CreateDateTimeFromSystemTime(ref DateTime.FullSystemTime time)
		{
			long num = DateTime.DateToTicks((int)time.wYear, (int)time.wMonth, (int)time.wDay);
			num += DateTime.TimeToTicks((int)time.wHour, (int)time.wMinute, (int)time.wSecond);
			num += (long)((ulong)time.wMillisecond * 10000UL);
			num += time.hundredNanoSecond;
			return new DateTime((ulong)(num | 4611686018427387904L));
		}

		[SecuritySafeCritical]
		internal static bool IsValidTimeWithLeapSeconds(int year, int month, int day, int hour, int minute, int second, DateTimeKind kind)
		{
			DateTime dateTime = new DateTime(year, month, day);
			DateTime.FullSystemTime fullSystemTime = new DateTime.FullSystemTime(year, month, dateTime.DayOfWeek, day, hour, minute, second);
			if (kind == DateTimeKind.Utc)
			{
				return DateTime.ValidateSystemTime(ref fullSystemTime, false);
			}
			if (kind == DateTimeKind.Local)
			{
				return DateTime.ValidateSystemTime(ref fullSystemTime, true);
			}
			return DateTime.ValidateSystemTime(ref fullSystemTime, true) || DateTime.ValidateSystemTime(ref fullSystemTime, false);
		}

		[__DynamicallyInvokable]
		public int Second
		{
			[__DynamicallyInvokable]
			get
			{
				return (int)(this.InternalTicks / 10000000L % 60L);
			}
		}

		[__DynamicallyInvokable]
		public long Ticks
		{
			[__DynamicallyInvokable]
			get
			{
				return this.InternalTicks;
			}
		}

		[__DynamicallyInvokable]
		public TimeSpan TimeOfDay
		{
			[__DynamicallyInvokable]
			get
			{
				return new TimeSpan(this.InternalTicks % 864000000000L);
			}
		}

		[__DynamicallyInvokable]
		public static DateTime Today
		{
			[__DynamicallyInvokable]
			get
			{
				return DateTime.Now.Date;
			}
		}

		[__DynamicallyInvokable]
		public int Year
		{
			[__DynamicallyInvokable]
			get
			{
				return this.GetDatePart(0);
			}
		}

		[__DynamicallyInvokable]
		public static bool IsLeapYear(int year)
		{
			if (year < 1 || year > 9999)
			{
				throw new ArgumentOutOfRangeException("year", Environment.GetResourceString("ArgumentOutOfRange_Year"));
			}
			return year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);
		}

		[__DynamicallyInvokable]
		public static DateTime Parse(string s)
		{
			return DateTimeParse.Parse(s, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.None);
		}

		[__DynamicallyInvokable]
		public static DateTime Parse(string s, IFormatProvider provider)
		{
			return DateTimeParse.Parse(s, DateTimeFormatInfo.GetInstance(provider), DateTimeStyles.None);
		}

		[__DynamicallyInvokable]
		public static DateTime Parse(string s, IFormatProvider provider, DateTimeStyles styles)
		{
			DateTimeFormatInfo.ValidateStyles(styles, "styles");
			return DateTimeParse.Parse(s, DateTimeFormatInfo.GetInstance(provider), styles);
		}

		[__DynamicallyInvokable]
		public static DateTime ParseExact(string s, string format, IFormatProvider provider)
		{
			return DateTimeParse.ParseExact(s, format, DateTimeFormatInfo.GetInstance(provider), DateTimeStyles.None);
		}

		[__DynamicallyInvokable]
		public static DateTime ParseExact(string s, string format, IFormatProvider provider, DateTimeStyles style)
		{
			DateTimeFormatInfo.ValidateStyles(style, "style");
			return DateTimeParse.ParseExact(s, format, DateTimeFormatInfo.GetInstance(provider), style);
		}

		[__DynamicallyInvokable]
		public static DateTime ParseExact(string s, string[] formats, IFormatProvider provider, DateTimeStyles style)
		{
			DateTimeFormatInfo.ValidateStyles(style, "style");
			return DateTimeParse.ParseExactMultiple(s, formats, DateTimeFormatInfo.GetInstance(provider), style);
		}

		[__DynamicallyInvokable]
		public TimeSpan Subtract(DateTime value)
		{
			return new TimeSpan(this.InternalTicks - value.InternalTicks);
		}

		[__DynamicallyInvokable]
		public DateTime Subtract(TimeSpan value)
		{
			long internalTicks = this.InternalTicks;
			long ticks = value._ticks;
			if (internalTicks < ticks || internalTicks - 3155378975999999999L > ticks)
			{
				throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_DateArithmetic"));
			}
			return new DateTime((ulong)(internalTicks - ticks | (long)this.InternalKind));
		}

		private static double TicksToOADate(long value)
		{
			if (value == 0L)
			{
				return 0.0;
			}
			if (value < 864000000000L)
			{
				value += 599264352000000000L;
			}
			if (value < 31241376000000000L)
			{
				throw new OverflowException(Environment.GetResourceString("Arg_OleAutDateInvalid"));
			}
			long num = (value - 599264352000000000L) / 10000L;
			if (num < 0L)
			{
				long num2 = num % 86400000L;
				if (num2 != 0L)
				{
					num -= (86400000L + num2) * 2L;
				}
			}
			return (double)num / 86400000.0;
		}

		[__DynamicallyInvokable]
		public double ToOADate()
		{
			return DateTime.TicksToOADate(this.InternalTicks);
		}

		[__DynamicallyInvokable]
		public long ToFileTime()
		{
			return this.ToUniversalTime().ToFileTimeUtc();
		}

		[__DynamicallyInvokable]
		public long ToFileTimeUtc()
		{
			long num = ((this.InternalKind & 9223372036854775808UL) != 0UL) ? this.ToUniversalTime().InternalTicks : this.InternalTicks;
			if (DateTime.s_isLeapSecondsSupportedSystem)
			{
				return DateTime.InternalToFileTime(num);
			}
			num -= 504911232000000000L;
			if (num < 0L)
			{
				throw new ArgumentOutOfRangeException(null, Environment.GetResourceString("ArgumentOutOfRange_FileTimeInvalid"));
			}
			return num;
		}

		[__DynamicallyInvokable]
		public DateTime ToLocalTime()
		{
			return this.ToLocalTime(false);
		}

		internal DateTime ToLocalTime(bool throwOnOverflow)
		{
			if (this.Kind == DateTimeKind.Local)
			{
				return this;
			}
			bool flag = false;
			bool isAmbiguousDst = false;
			long ticks = TimeZoneInfo.GetUtcOffsetFromUtc(this, TimeZoneInfo.Local, out flag, out isAmbiguousDst).Ticks;
			long num = this.Ticks + ticks;
			if (num > 3155378975999999999L)
			{
				if (throwOnOverflow)
				{
					throw new ArgumentException(Environment.GetResourceString("Arg_ArgumentOutOfRangeException"));
				}
				return new DateTime(3155378975999999999L, DateTimeKind.Local);
			}
			else
			{
				if (num >= 0L)
				{
					return new DateTime(num, DateTimeKind.Local, isAmbiguousDst);
				}
				if (throwOnOverflow)
				{
					throw new ArgumentException(Environment.GetResourceString("Arg_ArgumentOutOfRangeException"));
				}
				return new DateTime(0L, DateTimeKind.Local);
			}
		}

		public string ToLongDateString()
		{
			return DateTimeFormat.Format(this, "D", DateTimeFormatInfo.CurrentInfo);
		}

		public string ToLongTimeString()
		{
			return DateTimeFormat.Format(this, "T", DateTimeFormatInfo.CurrentInfo);
		}

		public string ToShortDateString()
		{
			return DateTimeFormat.Format(this, "d", DateTimeFormatInfo.CurrentInfo);
		}

		public string ToShortTimeString()
		{
			return DateTimeFormat.Format(this, "t", DateTimeFormatInfo.CurrentInfo);
		}

		[__DynamicallyInvokable]
		public override string ToString()
		{
			return DateTimeFormat.Format(this, null, DateTimeFormatInfo.CurrentInfo);
		}

		[__DynamicallyInvokable]
		public string ToString(string format)
		{
			return DateTimeFormat.Format(this, format, DateTimeFormatInfo.CurrentInfo);
		}

		[__DynamicallyInvokable]
		public string ToString(IFormatProvider provider)
		{
			return DateTimeFormat.Format(this, null, DateTimeFormatInfo.GetInstance(provider));
		}

		[__DynamicallyInvokable]
		public string ToString(string format, IFormatProvider provider)
		{
			return DateTimeFormat.Format(this, format, DateTimeFormatInfo.GetInstance(provider));
		}

		[__DynamicallyInvokable]
		public DateTime ToUniversalTime()
		{
			return TimeZoneInfo.ConvertTimeToUtc(this, TimeZoneInfoOptions.NoThrowOnInvalidTime);
		}

		[__DynamicallyInvokable]
		public static bool TryParse(string s, out DateTime result)
		{
			return DateTimeParse.TryParse(s, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.None, out result);
		}

		[__DynamicallyInvokable]
		public static bool TryParse(string s, IFormatProvider provider, DateTimeStyles styles, out DateTime result)
		{
			DateTimeFormatInfo.ValidateStyles(styles, "styles");
			return DateTimeParse.TryParse(s, DateTimeFormatInfo.GetInstance(provider), styles, out result);
		}

		[__DynamicallyInvokable]
		public static bool TryParseExact(string s, string format, IFormatProvider provider, DateTimeStyles style, out DateTime result)
		{
			DateTimeFormatInfo.ValidateStyles(style, "style");
			return DateTimeParse.TryParseExact(s, format, DateTimeFormatInfo.GetInstance(provider), style, out result);
		}

		[__DynamicallyInvokable]
		public static bool TryParseExact(string s, string[] formats, IFormatProvider provider, DateTimeStyles style, out DateTime result)
		{
			DateTimeFormatInfo.ValidateStyles(style, "style");
			return DateTimeParse.TryParseExactMultiple(s, formats, DateTimeFormatInfo.GetInstance(provider), style, out result);
		}

		[__DynamicallyInvokable]
		public static DateTime operator +(DateTime d, TimeSpan t)
		{
			long internalTicks = d.InternalTicks;
			long ticks = t._ticks;
			if (ticks > 3155378975999999999L - internalTicks || ticks < 0L - internalTicks)
			{
				throw new ArgumentOutOfRangeException("t", Environment.GetResourceString("ArgumentOutOfRange_DateArithmetic"));
			}
			return new DateTime((ulong)(internalTicks + ticks | (long)d.InternalKind));
		}

		[__DynamicallyInvokable]
		public static DateTime operator -(DateTime d, TimeSpan t)
		{
			long internalTicks = d.InternalTicks;
			long ticks = t._ticks;
			if (internalTicks < ticks || internalTicks - 3155378975999999999L > ticks)
			{
				throw new ArgumentOutOfRangeException("t", Environment.GetResourceString("ArgumentOutOfRange_DateArithmetic"));
			}
			return new DateTime((ulong)(internalTicks - ticks | (long)d.InternalKind));
		}

		[__DynamicallyInvokable]
		public static TimeSpan operator -(DateTime d1, DateTime d2)
		{
			return new TimeSpan(d1.InternalTicks - d2.InternalTicks);
		}

		[__DynamicallyInvokable]
		public static bool operator ==(DateTime d1, DateTime d2)
		{
			return d1.InternalTicks == d2.InternalTicks;
		}

		[__DynamicallyInvokable]
		public static bool operator !=(DateTime d1, DateTime d2)
		{
			return d1.InternalTicks != d2.InternalTicks;
		}

		[__DynamicallyInvokable]
		public static bool operator <(DateTime t1, DateTime t2)
		{
			return t1.InternalTicks < t2.InternalTicks;
		}

		[__DynamicallyInvokable]
		public static bool operator <=(DateTime t1, DateTime t2)
		{
			return t1.InternalTicks <= t2.InternalTicks;
		}

		[__DynamicallyInvokable]
		public static bool operator >(DateTime t1, DateTime t2)
		{
			return t1.InternalTicks > t2.InternalTicks;
		}

		[__DynamicallyInvokable]
		public static bool operator >=(DateTime t1, DateTime t2)
		{
			return t1.InternalTicks >= t2.InternalTicks;
		}

		[__DynamicallyInvokable]
		public string[] GetDateTimeFormats()
		{
			return this.GetDateTimeFormats(CultureInfo.CurrentCulture);
		}

		[__DynamicallyInvokable]
		public string[] GetDateTimeFormats(IFormatProvider provider)
		{
			return DateTimeFormat.GetAllDateTimes(this, DateTimeFormatInfo.GetInstance(provider));
		}

		[__DynamicallyInvokable]
		public string[] GetDateTimeFormats(char format)
		{
			return this.GetDateTimeFormats(format, CultureInfo.CurrentCulture);
		}

		[__DynamicallyInvokable]
		public string[] GetDateTimeFormats(char format, IFormatProvider provider)
		{
			return DateTimeFormat.GetAllDateTimes(this, format, DateTimeFormatInfo.GetInstance(provider));
		}

		public TypeCode GetTypeCode()
		{
			return TypeCode.DateTime;
		}

		[__DynamicallyInvokable]
		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", new object[]
			{
				"DateTime",
				"Boolean"
			}));
		}

		[__DynamicallyInvokable]
		char IConvertible.ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", new object[]
			{
				"DateTime",
				"Char"
			}));
		}

		[__DynamicallyInvokable]
		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", new object[]
			{
				"DateTime",
				"SByte"
			}));
		}

		[__DynamicallyInvokable]
		byte IConvertible.ToByte(IFormatProvider provider)
		{
			throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", new object[]
			{
				"DateTime",
				"Byte"
			}));
		}

		[__DynamicallyInvokable]
		short IConvertible.ToInt16(IFormatProvider provider)
		{
			throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", new object[]
			{
				"DateTime",
				"Int16"
			}));
		}

		[__DynamicallyInvokable]
		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", new object[]
			{
				"DateTime",
				"UInt16"
			}));
		}

		[__DynamicallyInvokable]
		int IConvertible.ToInt32(IFormatProvider provider)
		{
			throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", new object[]
			{
				"DateTime",
				"Int32"
			}));
		}

		[__DynamicallyInvokable]
		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", new object[]
			{
				"DateTime",
				"UInt32"
			}));
		}

		[__DynamicallyInvokable]
		long IConvertible.ToInt64(IFormatProvider provider)
		{
			throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", new object[]
			{
				"DateTime",
				"Int64"
			}));
		}

		[__DynamicallyInvokable]
		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", new object[]
			{
				"DateTime",
				"UInt64"
			}));
		}

		[__DynamicallyInvokable]
		float IConvertible.ToSingle(IFormatProvider provider)
		{
			throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", new object[]
			{
				"DateTime",
				"Single"
			}));
		}

		[__DynamicallyInvokable]
		double IConvertible.ToDouble(IFormatProvider provider)
		{
			throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", new object[]
			{
				"DateTime",
				"Double"
			}));
		}

		[__DynamicallyInvokable]
		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", new object[]
			{
				"DateTime",
				"Decimal"
			}));
		}

		[__DynamicallyInvokable]
		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return this;
		}

		[__DynamicallyInvokable]
		object IConvertible.ToType(Type type, IFormatProvider provider)
		{
			return Convert.DefaultToType(this, type, provider);
		}

		internal static bool TryCreate(int year, int month, int day, int hour, int minute, int second, int millisecond, out DateTime result)
		{
			result = DateTime.MinValue;
			if (year < 1 || year > 9999 || month < 1 || month > 12)
			{
				return false;
			}
			int[] array = DateTime.IsLeapYear(year) ? DateTime.DaysToMonth366 : DateTime.DaysToMonth365;
			if (day < 1 || day > array[month] - array[month - 1])
			{
				return false;
			}
			if (hour < 0 || hour >= 24 || minute < 0 || minute >= 60 || second < 0 || second > 60)
			{
				return false;
			}
			if (millisecond < 0 || millisecond >= 1000)
			{
				return false;
			}
			if (second == 60)
			{
				if (!DateTime.s_isLeapSecondsSupportedSystem || !DateTime.IsValidTimeWithLeapSeconds(year, month, day, hour, minute, second, DateTimeKind.Unspecified))
				{
					return false;
				}
				second = 59;
			}
			long num = DateTime.DateToTicks(year, month, day) + DateTime.TimeToTicks(hour, minute, second);
			num += (long)millisecond * 10000L;
			if (num < 0L || num > 3155378975999999999L)
			{
				return false;
			}
			result = new DateTime(num, DateTimeKind.Unspecified);
			return true;
		}

		private const long TicksPerMillisecond = 10000L;

		private const long TicksPerSecond = 10000000L;

		private const long TicksPerMinute = 600000000L;

		private const long TicksPerHour = 36000000000L;

		private const long TicksPerDay = 864000000000L;

		private const int MillisPerSecond = 1000;

		private const int MillisPerMinute = 60000;

		private const int MillisPerHour = 3600000;

		private const int MillisPerDay = 86400000;

		private const int DaysPerYear = 365;

		private const int DaysPer4Years = 1461;

		private const int DaysPer100Years = 36524;

		private const int DaysPer400Years = 146097;

		private const int DaysTo1601 = 584388;

		private const int DaysTo1899 = 693593;

		internal const int DaysTo1970 = 719162;

		private const int DaysTo10000 = 3652059;

		internal const long MinTicks = 0L;

		internal const long MaxTicks = 3155378975999999999L;

		private const long MaxMillis = 315537897600000L;

		private const long FileTimeOffset = 504911232000000000L;

		private const long DoubleDateOffset = 599264352000000000L;

		private const long OADateMinAsTicks = 31241376000000000L;

		private const double OADateMinAsDouble = -657435.0;

		private const double OADateMaxAsDouble = 2958466.0;

		private const int DatePartYear = 0;

		private const int DatePartDayOfYear = 1;

		private const int DatePartMonth = 2;

		private const int DatePartDay = 3;

		internal static readonly bool s_isLeapSecondsSupportedSystem = DateTime.SystemSupportLeapSeconds();

		private static readonly int[] DaysToMonth365 = new int[]
		{
			0,
			31,
			59,
			90,
			120,
			151,
			181,
			212,
			243,
			273,
			304,
			334,
			365
		};

		private static readonly int[] DaysToMonth366 = new int[]
		{
			0,
			31,
			60,
			91,
			121,
			152,
			182,
			213,
			244,
			274,
			305,
			335,
			366
		};

		[__DynamicallyInvokable]
		public static readonly DateTime MinValue = new DateTime(0L, DateTimeKind.Unspecified);

		[__DynamicallyInvokable]
		public static readonly DateTime MaxValue = new DateTime(3155378975999999999L, DateTimeKind.Unspecified);

		private const ulong TicksMask = 4611686018427387903UL;

		private const ulong FlagsMask = 13835058055282163712UL;

		private const ulong LocalMask = 9223372036854775808UL;

		private const long TicksCeiling = 4611686018427387904L;

		private const ulong KindUnspecified = 0UL;

		private const ulong KindUtc = 4611686018427387904UL;

		private const ulong KindLocal = 9223372036854775808UL;

		private const ulong KindLocalAmbiguousDst = 13835058055282163712UL;

		private const int KindShift = 62;

		private const string TicksField = "ticks";

		private const string DateDataField = "dateData";

		private ulong dateData;

		internal struct FullSystemTime
		{
			internal FullSystemTime(int year, int month, DayOfWeek dayOfWeek, int day, int hour, int minute, int second)
			{
				this.wYear = (ushort)year;
				this.wMonth = (ushort)month;
				this.wDayOfWeek = (ushort)dayOfWeek;
				this.wDay = (ushort)day;
				this.wHour = (ushort)hour;
				this.wMinute = (ushort)minute;
				this.wSecond = (ushort)second;
				this.wMillisecond = 0;
				this.hundredNanoSecond = 0L;
			}

			internal FullSystemTime(long ticks)
			{
				DateTime dateTime = new DateTime(ticks);
				int num;
				int num2;
				int num3;
				dateTime.GetDatePart(out num, out num2, out num3);
				this.wYear = (ushort)num;
				this.wMonth = (ushort)num2;
				this.wDayOfWeek = (ushort)dateTime.DayOfWeek;
				this.wDay = (ushort)num3;
				this.wHour = (ushort)dateTime.Hour;
				this.wMinute = (ushort)dateTime.Minute;
				this.wSecond = (ushort)dateTime.Second;
				this.wMillisecond = (ushort)dateTime.Millisecond;
				this.hundredNanoSecond = 0L;
			}

			internal ushort wYear;

			internal ushort wMonth;

			internal ushort wDayOfWeek;

			internal ushort wDay;

			internal ushort wHour;

			internal ushort wMinute;

			internal ushort wSecond;

			internal ushort wMillisecond;

			internal long hundredNanoSecond;
		}
	}
}
