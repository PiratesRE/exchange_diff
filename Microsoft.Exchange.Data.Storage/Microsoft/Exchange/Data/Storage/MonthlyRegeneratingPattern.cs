using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MonthlyRegeneratingPattern : RegeneratingPattern
	{
		public MonthlyRegeneratingPattern(int recurrenceInterval) : this(recurrenceInterval, CalendarType.Default)
		{
		}

		public MonthlyRegeneratingPattern(int recurrenceInterval, CalendarType calendarType)
		{
			EnumValidator.ThrowIfInvalid<CalendarType>(calendarType, "calendarType");
			base.RecurrenceInterval = recurrenceInterval;
			this.calendarType = calendarType;
		}

		public override bool Equals(RecurrencePattern value, bool ignoreCalendarTypeAndIsLeapMonth)
		{
			MonthlyRegeneratingPattern monthlyRegeneratingPattern = value as MonthlyRegeneratingPattern;
			return monthlyRegeneratingPattern != null && (ignoreCalendarTypeAndIsLeapMonth || monthlyRegeneratingPattern.CalendarType == this.CalendarType) && base.Equals(value, ignoreCalendarTypeAndIsLeapMonth);
		}

		public CalendarType CalendarType
		{
			get
			{
				return this.calendarType;
			}
		}

		internal override LocalizedString When()
		{
			if (base.RecurrenceInterval == 1)
			{
				return ClientStrings.TaskWhenMonthlyRegeneratingPattern;
			}
			return ClientStrings.TaskWhenNMonthsRegeneratingPattern(base.RecurrenceInterval);
		}

		private CalendarType calendarType;
	}
}
