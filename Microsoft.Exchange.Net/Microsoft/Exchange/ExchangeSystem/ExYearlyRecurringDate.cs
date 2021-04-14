using System;

namespace Microsoft.Exchange.ExchangeSystem
{
	public sealed class ExYearlyRecurringDate : ExYearlyRecurringTime
	{
		public ExYearlyRecurringDate()
		{
		}

		public ExYearlyRecurringDate(int month, int day, int hour, int minute, int second, int milliseconds) : this()
		{
			base.Month = month;
			this.Day = day;
			base.Hour = hour;
			base.Minute = minute;
			base.Second = second;
			base.Milliseconds = milliseconds;
			this.Validate();
		}

		public override DateTime GetInstance(int year)
		{
			return new DateTime(year, base.Month, this.Day, base.Hour, base.Minute, base.Second, base.Milliseconds, DateTimeKind.Unspecified);
		}

		protected override int GetSortIndex()
		{
			return 31 * (base.Month - 1) + (this.Day - 1) / 7;
		}

		public override string ToString()
		{
			return string.Format("Yearly recurring date: Month={0}; Day={1}; Hour={2}; Minute={3}; Second={4}; Millisecond={5}", new object[]
			{
				base.Month,
				this.Day,
				base.Hour,
				base.Minute,
				base.Second,
				base.Milliseconds
			});
		}

		internal override void Validate()
		{
			if (base.Month < 1 || base.Month > 12)
			{
				throw new ArgumentOutOfRangeException("Month");
			}
			int num = (base.Month == 2) ? 28 : DateTime.DaysInMonth(2000, base.Month);
			if (this.Day < 1 || this.Day > num)
			{
				throw new ArgumentOutOfRangeException("Day");
			}
			if (base.Hour < 0 || base.Hour > 23)
			{
				throw new ArgumentOutOfRangeException("Hour");
			}
			if (base.Minute < 0 || base.Minute > 59)
			{
				throw new ArgumentOutOfRangeException("Minute");
			}
			if (base.Second < 0 || base.Second > 59)
			{
				throw new ArgumentOutOfRangeException("Second");
			}
			if (base.Milliseconds < 0 || base.Milliseconds > 999)
			{
				throw new ArgumentOutOfRangeException("Milliseconds");
			}
		}

		public int Day
		{
			get
			{
				return this.day;
			}
			set
			{
				this.day = value;
			}
		}

		private int day;
	}
}
