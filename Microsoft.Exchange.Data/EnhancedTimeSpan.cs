using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Data
{
	[ComVisible(true)]
	[Serializable]
	public struct EnhancedTimeSpan : IComparable, IComparable<EnhancedTimeSpan>, IEquatable<EnhancedTimeSpan>, IComparable<TimeSpan>, IEquatable<TimeSpan>
	{
		private EnhancedTimeSpan(TimeSpan timeSpan)
		{
			this.timeSpan = timeSpan;
		}

		public static implicit operator TimeSpan(EnhancedTimeSpan enhancedTimeSpan)
		{
			return enhancedTimeSpan.timeSpan;
		}

		public static implicit operator EnhancedTimeSpan(TimeSpan timeSpan)
		{
			return new EnhancedTimeSpan(timeSpan);
		}

		public static EnhancedTimeSpan operator -(EnhancedTimeSpan t)
		{
			return t.Negate();
		}

		public static EnhancedTimeSpan operator -(EnhancedTimeSpan t1, EnhancedTimeSpan t2)
		{
			return t1.Subtract(t2);
		}

		public static EnhancedTimeSpan operator -(EnhancedTimeSpan t1, TimeSpan t2)
		{
			return t1.Subtract(t2);
		}

		public static EnhancedTimeSpan operator -(TimeSpan t1, EnhancedTimeSpan t2)
		{
			return t1.Subtract(t2);
		}

		public static bool operator !=(EnhancedTimeSpan t1, EnhancedTimeSpan t2)
		{
			return !t1.Equals(t2);
		}

		public static bool operator !=(EnhancedTimeSpan t1, TimeSpan t2)
		{
			return !t1.Equals(t2);
		}

		public static bool operator !=(TimeSpan t1, EnhancedTimeSpan t2)
		{
			return !t1.Equals(t2);
		}

		public static EnhancedTimeSpan operator +(EnhancedTimeSpan t)
		{
			return t;
		}

		public static EnhancedTimeSpan operator +(EnhancedTimeSpan t1, EnhancedTimeSpan t2)
		{
			return t1.Add(t2);
		}

		public static EnhancedTimeSpan operator +(EnhancedTimeSpan t1, TimeSpan t2)
		{
			return t1.Add(t2);
		}

		public static EnhancedTimeSpan operator +(TimeSpan t1, EnhancedTimeSpan t2)
		{
			return t1.Add(t2);
		}

		public static bool operator <(EnhancedTimeSpan t1, EnhancedTimeSpan t2)
		{
			return t1.CompareTo(t2) < 0;
		}

		public static bool operator <(EnhancedTimeSpan t1, TimeSpan t2)
		{
			return t1.CompareTo(t2) < 0;
		}

		public static bool operator <(TimeSpan t1, EnhancedTimeSpan t2)
		{
			return t1.CompareTo(t2) < 0;
		}

		public static bool operator <=(EnhancedTimeSpan t1, EnhancedTimeSpan t2)
		{
			return t1.CompareTo(t2) <= 0;
		}

		public static bool operator <=(EnhancedTimeSpan t1, TimeSpan t2)
		{
			return t1.CompareTo(t2) <= 0;
		}

		public static bool operator <=(TimeSpan t1, EnhancedTimeSpan t2)
		{
			return t1.CompareTo(t2) <= 0;
		}

		public static bool operator ==(EnhancedTimeSpan t1, EnhancedTimeSpan t2)
		{
			return t1.Equals(t2);
		}

		public static bool operator ==(EnhancedTimeSpan t1, TimeSpan t2)
		{
			return t1.Equals(t2);
		}

		public static bool operator ==(TimeSpan t1, EnhancedTimeSpan t2)
		{
			return t1.Equals(t2);
		}

		public static bool operator >(EnhancedTimeSpan t1, EnhancedTimeSpan t2)
		{
			return t1.CompareTo(t2) > 0;
		}

		public static bool operator >(EnhancedTimeSpan t1, TimeSpan t2)
		{
			return t1.CompareTo(t2) > 0;
		}

		public static bool operator >(TimeSpan t1, EnhancedTimeSpan t2)
		{
			return t1.CompareTo(t2) > 0;
		}

		public static bool operator >=(EnhancedTimeSpan t1, EnhancedTimeSpan t2)
		{
			return t1.CompareTo(t2) >= 0;
		}

		public static bool operator >=(EnhancedTimeSpan t1, TimeSpan t2)
		{
			return t1.CompareTo(t2) >= 0;
		}

		public static bool operator >=(TimeSpan t1, EnhancedTimeSpan t2)
		{
			return t1.CompareTo(t2) >= 0;
		}

		public int Days
		{
			get
			{
				return this.timeSpan.Days;
			}
		}

		public int Hours
		{
			get
			{
				return this.timeSpan.Hours;
			}
		}

		public int Milliseconds
		{
			get
			{
				return this.timeSpan.Milliseconds;
			}
		}

		public int Minutes
		{
			get
			{
				return this.timeSpan.Minutes;
			}
		}

		public int Seconds
		{
			get
			{
				return this.timeSpan.Seconds;
			}
		}

		public long Ticks
		{
			get
			{
				return this.timeSpan.Ticks;
			}
		}

		public double TotalDays
		{
			get
			{
				return this.timeSpan.TotalDays;
			}
		}

		public double TotalHours
		{
			get
			{
				return this.timeSpan.TotalHours;
			}
		}

		public double TotalMilliseconds
		{
			get
			{
				return this.timeSpan.TotalMilliseconds;
			}
		}

		public double TotalMinutes
		{
			get
			{
				return this.timeSpan.TotalMinutes;
			}
		}

		public double TotalSeconds
		{
			get
			{
				return this.timeSpan.TotalSeconds;
			}
		}

		public EnhancedTimeSpan Add(TimeSpan ts)
		{
			EnhancedTimeSpan result;
			try
			{
				result = this.timeSpan.Add(ts);
			}
			catch (OverflowException innerException)
			{
				throw new OverflowException(DataStrings.ExceptionDurationOverflow, innerException);
			}
			return result;
		}

		public static int Compare(TimeSpan t1, TimeSpan t2)
		{
			return TimeSpan.Compare(t1, t2);
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (value is TimeSpan)
			{
				return this.CompareTo((TimeSpan)value);
			}
			if (value is EnhancedTimeSpan)
			{
				return this.CompareTo((EnhancedTimeSpan)value);
			}
			throw new ArgumentException(DataStrings.ExceptionTypeNotEnhancedTimeSpanOrTimeSpan, "value");
		}

		public int CompareTo(EnhancedTimeSpan value)
		{
			return this.CompareTo(value.timeSpan);
		}

		public int CompareTo(TimeSpan value)
		{
			return this.timeSpan.CompareTo(value);
		}

		public EnhancedTimeSpan Duration()
		{
			return this.timeSpan.Duration();
		}

		public override bool Equals(object value)
		{
			if (value is EnhancedTimeSpan)
			{
				return this.Equals((EnhancedTimeSpan)value);
			}
			return value is TimeSpan && this.Equals((TimeSpan)value);
		}

		public bool Equals(EnhancedTimeSpan obj)
		{
			return this.Equals(obj.timeSpan);
		}

		public bool Equals(TimeSpan obj)
		{
			return this.timeSpan.Equals(obj);
		}

		public static bool Equals(TimeSpan t1, TimeSpan t2)
		{
			return TimeSpan.Equals(t1, t2);
		}

		public static EnhancedTimeSpan FromDays(double value)
		{
			return TimeSpan.FromDays(value);
		}

		public static EnhancedTimeSpan FromHours(double value)
		{
			return TimeSpan.FromHours(value);
		}

		public static EnhancedTimeSpan FromMilliseconds(double value)
		{
			return TimeSpan.FromMilliseconds(value);
		}

		public static EnhancedTimeSpan FromMinutes(double value)
		{
			return TimeSpan.FromMinutes(value);
		}

		public static EnhancedTimeSpan FromSeconds(double value)
		{
			return TimeSpan.FromSeconds(value);
		}

		public static EnhancedTimeSpan FromTicks(long value)
		{
			return TimeSpan.FromTicks(value);
		}

		public override int GetHashCode()
		{
			return this.timeSpan.GetHashCode();
		}

		public EnhancedTimeSpan Negate()
		{
			return this.timeSpan.Negate();
		}

		public EnhancedTimeSpan Subtract(TimeSpan ts)
		{
			EnhancedTimeSpan result;
			try
			{
				result = this.timeSpan.Subtract(ts);
			}
			catch (OverflowException innerException)
			{
				throw new OverflowException(DataStrings.ExceptionDurationOverflow, innerException);
			}
			return result;
		}

		public int Sign
		{
			get
			{
				if (0L == this.timeSpan.Ticks)
				{
					return 0;
				}
				if (0L >= this.timeSpan.Ticks)
				{
					return -1;
				}
				return 1;
			}
		}

		public static EnhancedTimeSpan operator *(EnhancedTimeSpan t, long n)
		{
			return t.Multiply(n);
		}

		public static EnhancedTimeSpan operator *(long n, EnhancedTimeSpan t)
		{
			return t.Multiply(n);
		}

		public static long operator /(EnhancedTimeSpan t1, EnhancedTimeSpan t2)
		{
			return t1.Divide(t2);
		}

		public static EnhancedTimeSpan operator %(EnhancedTimeSpan t1, EnhancedTimeSpan t2)
		{
			return t1.Mod(t2);
		}

		public long Divide(TimeSpan ts)
		{
			return this.timeSpan.Ticks / ts.Ticks;
		}

		public EnhancedTimeSpan Mod(TimeSpan ts)
		{
			return EnhancedTimeSpan.FromTicks(this.timeSpan.Ticks % ts.Ticks);
		}

		public EnhancedTimeSpan Multiply(long n)
		{
			long value = 0L;
			try
			{
				value = this.timeSpan.Ticks * n;
			}
			catch (OverflowException innerException)
			{
				throw new OverflowException(DataStrings.ExceptionDurationOverflow, innerException);
			}
			return EnhancedTimeSpan.FromTicks(value);
		}

		public static EnhancedTimeSpan Parse(string s)
		{
			return TimeSpan.Parse(s);
		}

		public override string ToString()
		{
			return this.timeSpan.ToString();
		}

		public static bool TryParse(string s, out EnhancedTimeSpan result)
		{
			TimeSpan timeSpan;
			bool flag = TimeSpan.TryParse(s, out timeSpan);
			result = (flag ? new EnhancedTimeSpan(timeSpan) : default(EnhancedTimeSpan));
			return flag;
		}

		public const long TicksPerDay = 864000000000L;

		public const long TicksPerHour = 36000000000L;

		public const long TicksPerMillisecond = 10000L;

		public const long TicksPerMinute = 600000000L;

		public const long TicksPerSecond = 10000000L;

		private TimeSpan timeSpan;

		public static readonly EnhancedTimeSpan MaxValue = TimeSpan.MaxValue;

		public static readonly EnhancedTimeSpan MinValue = TimeSpan.MinValue;

		public static readonly EnhancedTimeSpan Zero = TimeSpan.Zero;

		public static readonly EnhancedTimeSpan OneDay = TimeSpan.FromTicks(864000000000L);

		public static readonly EnhancedTimeSpan OneHour = TimeSpan.FromTicks(36000000000L);

		public static readonly EnhancedTimeSpan OneMillisecond = TimeSpan.FromTicks(10000L);

		public static readonly EnhancedTimeSpan OneMinute = TimeSpan.FromTicks(600000000L);

		public static readonly EnhancedTimeSpan OneSecond = TimeSpan.FromTicks(10000000L);

		public static readonly EnhancedTimeSpan OneTick = TimeSpan.FromTicks(1L);
	}
}
