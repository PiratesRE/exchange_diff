using System;

namespace Microsoft.Exchange.ExchangeSystem
{
	public sealed class ExYearlyRecurringDay : ExYearlyRecurringTime
	{
		public ExYearlyRecurringDay()
		{
		}

		public ExYearlyRecurringDay(int occurrence, DayOfWeek dayOfWeek, int month, int hour, int minute, int second, int milliseconds) : this()
		{
			this.Occurrence = occurrence;
			this.DayOfWeek = dayOfWeek;
			base.Month = month;
			base.Hour = hour;
			base.Minute = minute;
			base.Second = second;
			base.Milliseconds = milliseconds;
			this.Validate();
		}

		public override DateTime GetInstance(int year)
		{
			DateTime result;
			if (this.Occurrence == -1)
			{
				DateTime dateTime = new DateTime(year, base.Month, DateTime.DaysInMonth(year, base.Month), base.Hour, base.Minute, base.Second, base.Milliseconds, DateTimeKind.Unspecified);
				int num = dateTime.DayOfWeek - this.DayOfWeek;
				if (num < 0)
				{
					num += 7;
				}
				result = dateTime.AddDays((double)(-(double)num));
			}
			else
			{
				DateTime dateTime2 = new DateTime(year, base.Month, 1, base.Hour, base.Minute, base.Second, base.Milliseconds, DateTimeKind.Unspecified);
				int num2 = this.DayOfWeek - dateTime2.DayOfWeek;
				if (num2 < 0)
				{
					num2 += 7;
				}
				num2 += (this.Occurrence - 1) * 7;
				result = dateTime2.AddDays((double)num2);
			}
			return result;
		}

		public override string ToString()
		{
			return string.Format("Yearly recurring day: Occurrence={0}; DayOfWeek={1}; Month={2}; Hour={3}; Minute={4}; Second={5}; Millisecond={6}", new object[]
			{
				this.Occurrence,
				this.DayOfWeek,
				base.Month,
				base.Hour,
				base.Minute,
				base.Second,
				base.Milliseconds
			});
		}

		internal override void Validate()
		{
			if (this.Occurrence > 4 || (this.Occurrence < 1 && this.Occurrence != -1))
			{
				throw new ArgumentOutOfRangeException("Occurrence");
			}
			if (this.DayOfWeek < DayOfWeek.Sunday || this.DayOfWeek > DayOfWeek.Saturday)
			{
				throw new ArgumentOutOfRangeException("DayOfWeek");
			}
			if (base.Month < 1 || base.Month > 12)
			{
				throw new ArgumentOutOfRangeException("Month");
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

		public int Occurrence
		{
			get
			{
				return this.occurrence;
			}
			set
			{
				this.occurrence = value;
			}
		}

		public DayOfWeek DayOfWeek
		{
			get
			{
				return this.dayOfWeek;
			}
			set
			{
				this.dayOfWeek = value;
			}
		}

		protected override int GetSortIndex()
		{
			int num = (this.Occurrence == -1) ? 4 : this.Occurrence;
			return 31 * (base.Month - 1) + num;
		}

		private DayOfWeek dayOfWeek;

		private int occurrence;
	}
}
