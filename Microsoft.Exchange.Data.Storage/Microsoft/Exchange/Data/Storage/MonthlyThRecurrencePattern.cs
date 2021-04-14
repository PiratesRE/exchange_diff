using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MonthlyThRecurrencePattern : IntervalRecurrencePattern, IMonthlyThPatternInfo, IMonthlyPatternInfo
	{
		public MonthlyThRecurrencePattern() : this((DaysOfWeek)(1 << (int)DateTime.Today.DayOfWeek), RecurrenceOrderType.First)
		{
		}

		public MonthlyThRecurrencePattern(DaysOfWeek daysOfWeek, RecurrenceOrderType order) : this(daysOfWeek, order, 1)
		{
		}

		public MonthlyThRecurrencePattern(DaysOfWeek daysOfWeek, RecurrenceOrderType order, int recurrenceInterval) : this(daysOfWeek, order, recurrenceInterval, CalendarType.Default)
		{
		}

		public MonthlyThRecurrencePattern(DaysOfWeek daysOfWeek, RecurrenceOrderType order, int recurrenceInterval, CalendarType calendarType)
		{
			EnumValidator.ThrowIfInvalid<CalendarType>(calendarType);
			this.DaysOfWeek = daysOfWeek;
			this.Order = order;
			base.RecurrenceInterval = recurrenceInterval;
			this.calendarType = calendarType;
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
				return RecurrenceType.Monthly;
			}
		}

		public override bool Equals(RecurrencePattern value, bool ignoreCalendarTypeAndIsLeapMonth)
		{
			if (!(value is MonthlyThRecurrencePattern))
			{
				return false;
			}
			MonthlyThRecurrencePattern monthlyThRecurrencePattern = (MonthlyThRecurrencePattern)value;
			return monthlyThRecurrencePattern.DaysOfWeek == this.daysOfWeek && monthlyThRecurrencePattern.Order == this.order && (ignoreCalendarTypeAndIsLeapMonth || monthlyThRecurrencePattern.calendarType == this.calendarType) && base.Equals(value, ignoreCalendarTypeAndIsLeapMonth);
		}

		internal static LocalizedString OrderAsString(RecurrenceOrderType order)
		{
			switch (order)
			{
			case RecurrenceOrderType.Last:
				return ClientStrings.WhenLast;
			case RecurrenceOrderType.First:
				return ClientStrings.WhenFirst;
			case RecurrenceOrderType.Second:
				return ClientStrings.WhenSecond;
			case RecurrenceOrderType.Third:
				return ClientStrings.WhenThird;
			case RecurrenceOrderType.Fourth:
				return ClientStrings.WhenFourth;
			}
			ExDiagnostics.FailFast("Invalid value for Order", false);
			throw new ArgumentOutOfRangeException("Order");
		}

		internal override LocalizedString When()
		{
			if (Recurrence.IsGregorianCompatible(this.CalendarType))
			{
				if (base.RecurrenceInterval == 1)
				{
					if (base.RecurrenceObjectType == RecurrenceObjectType.CalendarRecurrence)
					{
						return ClientStrings.CalendarWhenMonthlyThEveryMonth(MonthlyThRecurrencePattern.OrderAsString(this.Order), new LocalizedDaysOfWeek(this.DaysOfWeek));
					}
					return ClientStrings.TaskWhenMonthlyThEveryMonth(MonthlyThRecurrencePattern.OrderAsString(this.Order), new LocalizedDaysOfWeek(this.DaysOfWeek));
				}
				else if (base.RecurrenceInterval == 2)
				{
					if (base.RecurrenceObjectType == RecurrenceObjectType.CalendarRecurrence)
					{
						return ClientStrings.CalendarWhenMonthlyThEveryOtherMonth(MonthlyThRecurrencePattern.OrderAsString(this.Order), new LocalizedDaysOfWeek(this.DaysOfWeek));
					}
					return ClientStrings.TaskWhenMonthlyThEveryOtherMonth(MonthlyThRecurrencePattern.OrderAsString(this.Order), new LocalizedDaysOfWeek(this.DaysOfWeek));
				}
				else
				{
					if (base.RecurrenceObjectType == RecurrenceObjectType.CalendarRecurrence)
					{
						return ClientStrings.CalendarWhenMonthlyThEveryNMonths(MonthlyThRecurrencePattern.OrderAsString(this.Order), new LocalizedDaysOfWeek(this.DaysOfWeek), base.RecurrenceInterval);
					}
					return ClientStrings.TaskWhenMonthlyThEveryNMonths(MonthlyThRecurrencePattern.OrderAsString(this.Order), new LocalizedDaysOfWeek(this.DaysOfWeek), base.RecurrenceInterval);
				}
			}
			else if (base.RecurrenceInterval == 1)
			{
				if (base.RecurrenceObjectType == RecurrenceObjectType.CalendarRecurrence)
				{
					return ClientStrings.AlternateCalendarWhenMonthlyThEveryMonth(Recurrence.GetCalendarName(this.CalendarType), MonthlyThRecurrencePattern.OrderAsString(this.Order), new LocalizedDaysOfWeek(this.DaysOfWeek));
				}
				return ClientStrings.AlternateCalendarTaskWhenMonthlyThEveryMonth(Recurrence.GetCalendarName(this.CalendarType), MonthlyThRecurrencePattern.OrderAsString(this.Order), new LocalizedDaysOfWeek(this.DaysOfWeek));
			}
			else if (base.RecurrenceInterval == 2)
			{
				if (base.RecurrenceObjectType == RecurrenceObjectType.CalendarRecurrence)
				{
					return ClientStrings.AlternateCalendarWhenMonthlyThEveryOtherMonth(Recurrence.GetCalendarName(this.CalendarType), MonthlyThRecurrencePattern.OrderAsString(this.Order), new LocalizedDaysOfWeek(this.DaysOfWeek));
				}
				return ClientStrings.AlternateCalendarTaskWhenMonthlyThEveryOtherMonth(Recurrence.GetCalendarName(this.CalendarType), MonthlyThRecurrencePattern.OrderAsString(this.Order), new LocalizedDaysOfWeek(this.DaysOfWeek));
			}
			else
			{
				if (base.RecurrenceObjectType == RecurrenceObjectType.CalendarRecurrence)
				{
					return ClientStrings.AlternateCalendarWhenMonthlyThEveryNMonths(Recurrence.GetCalendarName(this.CalendarType), MonthlyThRecurrencePattern.OrderAsString(this.Order), new LocalizedDaysOfWeek(this.DaysOfWeek), base.RecurrenceInterval);
				}
				return ClientStrings.AlternateCalendarTaskWhenMonthlyThEveryNMonths(Recurrence.GetCalendarName(this.CalendarType), MonthlyThRecurrencePattern.OrderAsString(this.Order), new LocalizedDaysOfWeek(this.DaysOfWeek), base.RecurrenceInterval);
			}
		}

		private RecurrenceOrderType order;

		private DaysOfWeek daysOfWeek;

		private CalendarType calendarType;
	}
}
