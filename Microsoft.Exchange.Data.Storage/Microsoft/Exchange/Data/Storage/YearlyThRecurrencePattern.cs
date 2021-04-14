using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class YearlyThRecurrencePattern : IntervalRecurrencePattern, IYearlyPatternInfo, IMonthlyThPatternInfo, IMonthlyPatternInfo
	{
		public YearlyThRecurrencePattern() : this((DaysOfWeek)(1 << (int)DateTime.Today.DayOfWeek), RecurrenceOrderType.First, DateTime.Today.Month)
		{
		}

		public YearlyThRecurrencePattern(DaysOfWeek daysOfWeek, RecurrenceOrderType order, int month) : this(daysOfWeek, order, month, false, CalendarType.Default)
		{
		}

		public YearlyThRecurrencePattern(DaysOfWeek daysOfWeek, RecurrenceOrderType order, int month, bool isLeapMonth, CalendarType calendarType) : this(daysOfWeek, order, month, isLeapMonth, 1, calendarType)
		{
		}

		public YearlyThRecurrencePattern(DaysOfWeek daysOfWeek, RecurrenceOrderType order, int month, bool isLeapMonth, int recurrenceInterval, CalendarType calendarType)
		{
			EnumValidator.ThrowIfInvalid<CalendarType>(calendarType);
			this.Month = month;
			this.DaysOfWeek = daysOfWeek;
			this.Order = order;
			this.isLeapMonth = isLeapMonth;
			this.calendarType = calendarType;
			base.RecurrenceInterval = recurrenceInterval;
		}

		public DaysOfWeek DaysOfWeek
		{
			get
			{
				return this.daysOfWeek;
			}
			private set
			{
				EnumValidator.ThrowIfInvalid<DaysOfWeek>(value);
				this.daysOfWeek = value;
			}
		}

		public RecurrenceOrderType Order
		{
			get
			{
				return this.order;
			}
			private set
			{
				EnumValidator.ThrowIfInvalid<RecurrenceOrderType>(value);
				this.order = value;
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
			if (!(value is YearlyThRecurrencePattern))
			{
				return false;
			}
			YearlyThRecurrencePattern yearlyThRecurrencePattern = (YearlyThRecurrencePattern)value;
			return yearlyThRecurrencePattern.DaysOfWeek == this.DaysOfWeek && yearlyThRecurrencePattern.Month == this.Month && yearlyThRecurrencePattern.Order == this.Order && (ignoreCalendarTypeAndIsLeapMonth || yearlyThRecurrencePattern.CalendarType == this.CalendarType) && (ignoreCalendarTypeAndIsLeapMonth || yearlyThRecurrencePattern.IsLeapMonth == this.IsLeapMonth);
		}

		internal override LocalizedString When()
		{
			LocalizedString result;
			if (Recurrence.IsGregorianCompatible(this.CalendarType))
			{
				if (base.RecurrenceObjectType == RecurrenceObjectType.CalendarRecurrence)
				{
					result = ClientStrings.CalendarWhenYearlyTh(MonthlyThRecurrencePattern.OrderAsString(this.Order), new LocalizedDaysOfWeek(this.DaysOfWeek), new LocalizedMonth(this.Month));
				}
				else
				{
					result = ClientStrings.TaskWhenYearlyTh(MonthlyThRecurrencePattern.OrderAsString(this.Order), new LocalizedDaysOfWeek(this.DaysOfWeek), new LocalizedMonth(this.Month));
				}
			}
			else if (this.IsLeapMonth)
			{
				if (base.RecurrenceObjectType == RecurrenceObjectType.CalendarRecurrence)
				{
					result = ClientStrings.AlternateCalendarWhenYearlyThLeap(Recurrence.GetCalendarName(this.CalendarType), MonthlyThRecurrencePattern.OrderAsString(this.Order), new LocalizedDaysOfWeek(this.DaysOfWeek), this.Month);
				}
				else
				{
					result = ClientStrings.AlternateCalendarTaskWhenYearlyThLeap(Recurrence.GetCalendarName(this.CalendarType), MonthlyThRecurrencePattern.OrderAsString(this.Order), new LocalizedDaysOfWeek(this.DaysOfWeek), this.Month);
				}
			}
			else if (base.RecurrenceObjectType == RecurrenceObjectType.CalendarRecurrence)
			{
				result = ClientStrings.AlternateCalendarWhenYearlyTh(Recurrence.GetCalendarName(this.CalendarType), MonthlyThRecurrencePattern.OrderAsString(this.Order), new LocalizedDaysOfWeek(this.DaysOfWeek), this.Month);
			}
			else
			{
				result = ClientStrings.AlternateCalendarTaskWhenYearlyTh(Recurrence.GetCalendarName(this.CalendarType), MonthlyThRecurrencePattern.OrderAsString(this.Order), new LocalizedDaysOfWeek(this.DaysOfWeek), this.Month);
			}
			return result;
		}

		private RecurrenceOrderType order;

		private DaysOfWeek daysOfWeek;

		private int month;

		private bool isLeapMonth;

		private CalendarType calendarType;
	}
}
