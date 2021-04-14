using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MonthlyRecurrencePattern : IntervalRecurrencePattern, IMonthlyPatternInfo
	{
		public MonthlyRecurrencePattern() : this(ExDateTime.GetNow(ExTimeZone.CurrentTimeZone).Day)
		{
		}

		public MonthlyRecurrencePattern(int dayOfMonth) : this(dayOfMonth, 1)
		{
		}

		public MonthlyRecurrencePattern(int dayOfMonth, int recurrenceInterval) : this(dayOfMonth, recurrenceInterval, CalendarType.Default)
		{
		}

		public MonthlyRecurrencePattern(int dayOfMonth, int recurrenceInterval, CalendarType calendarType)
		{
			EnumValidator.ThrowIfInvalid<CalendarType>(calendarType);
			this.DayOfMonth = dayOfMonth;
			base.RecurrenceInterval = recurrenceInterval;
			this.calendarType = calendarType;
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

		public CalendarType CalendarType
		{
			get
			{
				return this.calendarType;
			}
		}

		public override bool Equals(RecurrencePattern value, bool ignoreCalendarTypeAndIsLeapMonth)
		{
			if (!(value is MonthlyRecurrencePattern))
			{
				return false;
			}
			MonthlyRecurrencePattern monthlyRecurrencePattern = (MonthlyRecurrencePattern)value;
			return monthlyRecurrencePattern.DayOfMonth == this.dayOfMonth && (ignoreCalendarTypeAndIsLeapMonth || monthlyRecurrencePattern.calendarType == this.calendarType) && base.Equals(value, ignoreCalendarTypeAndIsLeapMonth);
		}

		internal override LocalizedString When()
		{
			if (Recurrence.IsGregorianCompatible(this.CalendarType))
			{
				if (base.RecurrenceInterval == 1)
				{
					if (base.RecurrenceObjectType == RecurrenceObjectType.CalendarRecurrence)
					{
						return ClientStrings.CalendarWhenMonthlyEveryMonth(this.DayOfMonth);
					}
					return ClientStrings.TaskWhenMonthlyEveryMonth(this.DayOfMonth);
				}
				else if (base.RecurrenceInterval == 2)
				{
					if (base.RecurrenceObjectType == RecurrenceObjectType.CalendarRecurrence)
					{
						return ClientStrings.CalendarWhenMonthlyEveryOtherMonth(this.DayOfMonth);
					}
					return ClientStrings.TaskWhenMonthlyEveryOtherMonth(this.DayOfMonth);
				}
				else
				{
					if (base.RecurrenceObjectType == RecurrenceObjectType.CalendarRecurrence)
					{
						return ClientStrings.CalendarWhenMonthlyEveryNMonths(this.DayOfMonth, base.RecurrenceInterval);
					}
					return ClientStrings.TaskWhenMonthlyEveryNMonths(this.DayOfMonth, base.RecurrenceInterval);
				}
			}
			else if (base.RecurrenceInterval == 1)
			{
				if (base.RecurrenceObjectType == RecurrenceObjectType.CalendarRecurrence)
				{
					return ClientStrings.AlternateCalendarWhenMonthlyEveryMonth(Recurrence.GetCalendarName(this.CalendarType), this.DayOfMonth);
				}
				return ClientStrings.AlternateCalendarTaskWhenMonthlyEveryMonth(Recurrence.GetCalendarName(this.CalendarType), this.DayOfMonth);
			}
			else if (base.RecurrenceInterval == 2)
			{
				if (base.RecurrenceObjectType == RecurrenceObjectType.CalendarRecurrence)
				{
					return ClientStrings.AlternateCalendarWhenMonthlyEveryOtherMonth(Recurrence.GetCalendarName(this.CalendarType), this.DayOfMonth);
				}
				return ClientStrings.AlternateCalendarTaskWhenMonthlyEveryOtherMonth(Recurrence.GetCalendarName(this.CalendarType), this.DayOfMonth);
			}
			else
			{
				if (base.RecurrenceObjectType == RecurrenceObjectType.CalendarRecurrence)
				{
					return ClientStrings.AlternateCalendarWhenMonthlyEveryNMonths(Recurrence.GetCalendarName(this.CalendarType), this.DayOfMonth, base.RecurrenceInterval);
				}
				return ClientStrings.AlternateCalendarTaskWhenMonthlyEveryNMonths(Recurrence.GetCalendarName(this.CalendarType), this.DayOfMonth, base.RecurrenceInterval);
			}
		}

		internal override RecurrenceType MapiRecurrenceType
		{
			get
			{
				return RecurrenceType.Monthly;
			}
		}

		private int dayOfMonth;

		private CalendarType calendarType;
	}
}
