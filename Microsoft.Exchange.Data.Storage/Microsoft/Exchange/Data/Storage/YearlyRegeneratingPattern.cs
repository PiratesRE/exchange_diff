using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class YearlyRegeneratingPattern : RegeneratingPattern
	{
		public YearlyRegeneratingPattern(int recurrenceInterval) : this(recurrenceInterval, CalendarType.Default)
		{
		}

		public YearlyRegeneratingPattern(int recurrenceInterval, CalendarType calendarType)
		{
			EnumValidator.ThrowIfInvalid<CalendarType>(calendarType, "calendarType");
			base.RecurrenceInterval = recurrenceInterval;
			this.calendarType = calendarType;
		}

		public override bool Equals(RecurrencePattern value, bool ignoreCalendarTypeAndIsLeapMonth)
		{
			YearlyRegeneratingPattern yearlyRegeneratingPattern = value as YearlyRegeneratingPattern;
			return yearlyRegeneratingPattern != null && (ignoreCalendarTypeAndIsLeapMonth || yearlyRegeneratingPattern.calendarType == this.calendarType) && base.Equals(value, ignoreCalendarTypeAndIsLeapMonth);
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
				return ClientStrings.TaskWhenYearlyRegeneratingPattern;
			}
			return ClientStrings.TaskWhenNYearsRegeneratingPattern(base.RecurrenceInterval);
		}

		private CalendarType calendarType;
	}
}
