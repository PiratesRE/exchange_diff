using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class WeeklyRecurrencePattern : IntervalRecurrencePattern, IWeeklyPatternInfo
	{
		public WeeklyRecurrencePattern() : this((DaysOfWeek)(1 << (int)ExDateTime.Today.DayOfWeek))
		{
		}

		public WeeklyRecurrencePattern(DaysOfWeek daysOfWeek) : this(daysOfWeek, 1)
		{
		}

		public WeeklyRecurrencePattern(DaysOfWeek daysOfWeek, int recurrenceInterval) : this(daysOfWeek, recurrenceInterval, DayOfWeek.Sunday)
		{
		}

		public WeeklyRecurrencePattern(DaysOfWeek daysOfWeek, int recurrenceInterval, DayOfWeek firstDayOfWeek)
		{
			EnumValidator.ThrowIfInvalid<DayOfWeek>(firstDayOfWeek);
			this.DaysOfWeek = daysOfWeek;
			base.RecurrenceInterval = recurrenceInterval;
			this.firstDayOfWeek = firstDayOfWeek;
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

		public DayOfWeek FirstDayOfWeek
		{
			get
			{
				return this.firstDayOfWeek;
			}
		}

		public override bool Equals(RecurrencePattern value, bool ignoreCalendarTypeAndIsLeapMonth)
		{
			if (!(value is WeeklyRecurrencePattern))
			{
				return false;
			}
			WeeklyRecurrencePattern weeklyRecurrencePattern = (WeeklyRecurrencePattern)value;
			return weeklyRecurrencePattern.DaysOfWeek == this.daysOfWeek && weeklyRecurrencePattern.FirstDayOfWeek == this.FirstDayOfWeek && base.Equals(value, ignoreCalendarTypeAndIsLeapMonth);
		}

		internal override LocalizedString When()
		{
			LocalizedString result;
			if (base.RecurrenceInterval == 1)
			{
				if (base.RecurrenceObjectType == RecurrenceObjectType.CalendarRecurrence)
				{
					result = ClientStrings.CalendarWhenWeeklyEveryWeek(new LocalizedDaysOfWeek(this.DaysOfWeek, this.FirstDayOfWeek));
				}
				else
				{
					result = ClientStrings.TaskWhenWeeklyEveryWeek(new LocalizedDaysOfWeek(this.DaysOfWeek, this.FirstDayOfWeek));
				}
			}
			else if (base.RecurrenceInterval == 2)
			{
				if (base.RecurrenceObjectType == RecurrenceObjectType.CalendarRecurrence)
				{
					result = ClientStrings.CalendarWhenWeeklyEveryAlterateWeek(new LocalizedDaysOfWeek(this.DaysOfWeek, this.FirstDayOfWeek));
				}
				else
				{
					result = ClientStrings.TaskWhenWeeklyEveryAlterateWeek(new LocalizedDaysOfWeek(this.DaysOfWeek, this.FirstDayOfWeek));
				}
			}
			else if (base.RecurrenceObjectType == RecurrenceObjectType.CalendarRecurrence)
			{
				result = ClientStrings.CalendarWhenWeeklyEveryNWeeks(base.RecurrenceInterval, new LocalizedDaysOfWeek(this.DaysOfWeek, this.FirstDayOfWeek));
			}
			else
			{
				result = ClientStrings.TaskWhenWeeklyEveryNWeeks(base.RecurrenceInterval, new LocalizedDaysOfWeek(this.DaysOfWeek, this.FirstDayOfWeek));
			}
			return result;
		}

		internal override RecurrenceType MapiRecurrenceType
		{
			get
			{
				return RecurrenceType.Weekly;
			}
		}

		private DaysOfWeek daysOfWeek;

		private DayOfWeek firstDayOfWeek;
	}
}
