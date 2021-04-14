using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class YearlyRecurrencePattern : IntervalRecurrencePattern, IYearlyPatternInfo, IMonthlyPatternInfo
	{
		public YearlyRecurrencePattern() : this(ExDateTime.Today.Day, ExDateTime.Today.Month)
		{
		}

		public YearlyRecurrencePattern(int dayOfMonth, int month) : this(dayOfMonth, month, false, 1, CalendarType.Default)
		{
		}

		public YearlyRecurrencePattern(int dayOfMonth, int month, int months) : this(dayOfMonth, month, false, CalendarType.Default, months)
		{
		}

		public YearlyRecurrencePattern(int dayOfMonth, int month, bool isLeapMonth, CalendarType calendarType) : this(dayOfMonth, month, isLeapMonth, 1, calendarType)
		{
		}

		public YearlyRecurrencePattern(int dayOfMonth, int month, bool isLeapMonth, int recurrenceInterval, CalendarType calendarType) : this(dayOfMonth, month, isLeapMonth, calendarType, 12 * recurrenceInterval)
		{
		}

		internal YearlyRecurrencePattern(int dayOfMonth, int month, bool isLeapMonth, CalendarType calendarType, int months)
		{
			EnumValidator.ThrowIfInvalid<CalendarType>(calendarType);
			this.isLeapMonth = isLeapMonth;
			this.DayOfMonth = dayOfMonth;
			this.Month = month;
			this.calendarType = calendarType;
			this.Months = months;
			base.RecurrenceInterval = months / 12;
		}

		public int DayOfMonth
		{
			get
			{
				return this.dayOfMonth;
			}
			private set
			{
				if (value < 1 || value > 31)
				{
					throw new ArgumentOutOfRangeException(ServerStrings.ExInvalidDayOfMonth, "DayOfMonth");
				}
				this.dayOfMonth = value;
			}
		}

		public int Month
		{
			get
			{
				return this.month;
			}
			private set
			{
				if (value < 1 || value > 12)
				{
					throw new ArgumentOutOfRangeException(ServerStrings.ExInvalidMonth, "Month");
				}
				this.month = value;
			}
		}

		public bool IsLeapMonth
		{
			get
			{
				return this.isLeapMonth;
			}
		}

		public CalendarType CalendarType
		{
			get
			{
				return this.calendarType;
			}
		}

		internal override RecurrenceType MapiRecurrenceType
		{
			get
			{
				return RecurrenceType.Yearly;
			}
		}

		public override bool Equals(RecurrencePattern value, bool ignoreCalendarTypeAndIsLeapMonth)
		{
			if (!(value is YearlyRecurrencePattern))
			{
				return false;
			}
			YearlyRecurrencePattern yearlyRecurrencePattern = (YearlyRecurrencePattern)value;
			return yearlyRecurrencePattern.DayOfMonth == this.DayOfMonth && yearlyRecurrencePattern.Month == this.Month && (ignoreCalendarTypeAndIsLeapMonth || yearlyRecurrencePattern.CalendarType == this.CalendarType) && (ignoreCalendarTypeAndIsLeapMonth || yearlyRecurrencePattern.IsLeapMonth == this.IsLeapMonth);
		}

		internal override LocalizedString When()
		{
			LocalizedString result;
			if (Recurrence.IsGregorianCompatible(this.CalendarType))
			{
				if (base.RecurrenceObjectType == RecurrenceObjectType.CalendarRecurrence)
				{
					result = ClientStrings.CalendarWhenYearly(new LocalizedMonth(this.month), this.DayOfMonth);
				}
				else
				{
					result = ClientStrings.TaskWhenYearly(new LocalizedMonth(this.month), this.DayOfMonth);
				}
			}
			else if (this.IsLeapMonth)
			{
				if (base.RecurrenceObjectType == RecurrenceObjectType.CalendarRecurrence)
				{
					result = ClientStrings.AlternateCalendarWhenYearlyLeap(Recurrence.GetCalendarName(this.CalendarType), this.month, this.DayOfMonth);
				}
				else
				{
					result = ClientStrings.AlternateCalendarTaskWhenYearlyLeap(Recurrence.GetCalendarName(this.CalendarType), this.month, this.DayOfMonth);
				}
			}
			else if (base.RecurrenceObjectType == RecurrenceObjectType.CalendarRecurrence)
			{
				result = ClientStrings.AlternateCalendarWhenYearly(Recurrence.GetCalendarName(this.CalendarType), this.month, this.DayOfMonth);
			}
			else
			{
				result = ClientStrings.AlternateCalendarTaskWhenYearly(Recurrence.GetCalendarName(this.CalendarType), this.month, this.DayOfMonth);
			}
			return result;
		}

		internal int Months
		{
			get
			{
				return this.months;
			}
			set
			{
				this.months = value;
			}
		}

		private int dayOfMonth;

		private int month;

		private bool isLeapMonth;

		private CalendarType calendarType;

		private int months;
	}
}
