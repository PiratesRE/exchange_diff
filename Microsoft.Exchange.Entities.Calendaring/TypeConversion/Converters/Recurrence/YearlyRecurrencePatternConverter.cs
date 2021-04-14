using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters.Recurrence
{
	internal struct YearlyRecurrencePatternConverter
	{
		public YearlyRecurrencePatternConverter(IDayOfWeekConverter dayOfWeekConverter, IWeekIndexConverter weekIndexConverter)
		{
			this = default(YearlyRecurrencePatternConverter);
			this.dayOfWeekConverter = dayOfWeekConverter;
			this.weekIndexConverter = weekIndexConverter;
		}

		public RecurrencePattern ConvertEntitiesToStorage(AbsoluteYearlyRecurrencePattern value)
		{
			if (value != null)
			{
				return new YearlyRecurrencePattern(value.DayOfMonth, value.Month, value.Interval * 12);
			}
			return null;
		}

		public RecurrencePattern ConvertEntitiesToStorage(RelativeYearlyRecurrencePattern value)
		{
			if (value != null)
			{
				return new YearlyThRecurrencePattern(this.dayOfWeekConverter.Convert(value.DaysOfWeek), this.weekIndexConverter.Convert(value.Index), value.Month, false, value.Interval, CalendarType.Default);
			}
			return null;
		}

		public RecurrencePattern ConvertStorageToEntities(YearlyRecurrencePattern value)
		{
			if (value != null)
			{
				return new AbsoluteYearlyRecurrencePattern
				{
					DayOfMonth = value.DayOfMonth,
					Interval = value.RecurrenceInterval,
					Month = value.Month
				};
			}
			return null;
		}

		public RecurrencePattern ConvertStorageToEntities(YearlyThRecurrencePattern value)
		{
			if (value != null)
			{
				return new RelativeYearlyRecurrencePattern
				{
					DaysOfWeek = this.dayOfWeekConverter.Convert(value.DaysOfWeek),
					Index = this.weekIndexConverter.Convert(value.Order),
					Interval = value.RecurrenceInterval,
					Month = value.Month
				};
			}
			return null;
		}

		private readonly IDayOfWeekConverter dayOfWeekConverter;

		private readonly IWeekIndexConverter weekIndexConverter;
	}
}
