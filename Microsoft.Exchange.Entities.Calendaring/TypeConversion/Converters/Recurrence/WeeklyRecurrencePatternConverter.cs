using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters.Recurrence
{
	internal struct WeeklyRecurrencePatternConverter
	{
		public WeeklyRecurrencePatternConverter(DayOfWeekConverter dayOfWeekConverter)
		{
			this.dayOfWeekConverter = dayOfWeekConverter;
		}

		public RecurrencePattern ConvertEntitiesToStorage(WeeklyRecurrencePattern value)
		{
			if (value != null)
			{
				return new WeeklyRecurrencePattern(this.dayOfWeekConverter.Convert(value.DaysOfWeek), value.Interval, value.FirstDayOfWeek);
			}
			return null;
		}

		public RecurrencePattern ConvertStorageToEntities(WeeklyRecurrencePattern value)
		{
			if (value != null)
			{
				return new WeeklyRecurrencePattern
				{
					DaysOfWeek = this.dayOfWeekConverter.Convert(value.DaysOfWeek),
					FirstDayOfWeek = value.FirstDayOfWeek,
					Interval = value.RecurrenceInterval
				};
			}
			return null;
		}

		private readonly DayOfWeekConverter dayOfWeekConverter;
	}
}
