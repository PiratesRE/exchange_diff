using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters.Recurrence
{
	internal struct MonthlyRecurrencePatternConverter
	{
		public MonthlyRecurrencePatternConverter(IDayOfWeekConverter dayOfWeekConverter, IWeekIndexConverter weekIndexConverter)
		{
			this.dayOfWeekConverter = dayOfWeekConverter;
			this.weekIndexConverter = weekIndexConverter;
		}

		public RecurrencePattern ConvertEntitiesToStorage(AbsoluteMonthlyRecurrencePattern value)
		{
			if (value != null)
			{
				return new MonthlyRecurrencePattern(value.DayOfMonth, value.Interval);
			}
			return null;
		}

		public RecurrencePattern ConvertEntitiesToStorage(RelativeMonthlyRecurrencePattern value)
		{
			if (value != null)
			{
				return new MonthlyThRecurrencePattern(this.dayOfWeekConverter.Convert(value.DaysOfWeek), this.weekIndexConverter.Convert(value.Index), value.Interval);
			}
			return null;
		}

		public RecurrencePattern ConvertStorageToEntities(MonthlyRecurrencePattern value)
		{
			if (value != null)
			{
				return new AbsoluteMonthlyRecurrencePattern
				{
					DayOfMonth = value.DayOfMonth,
					Interval = value.RecurrenceInterval
				};
			}
			return null;
		}

		public RecurrencePattern ConvertStorageToEntities(MonthlyThRecurrencePattern value)
		{
			if (value != null)
			{
				return new RelativeMonthlyRecurrencePattern
				{
					DaysOfWeek = this.dayOfWeekConverter.Convert(value.DaysOfWeek),
					Index = this.weekIndexConverter.Convert(value.Order),
					Interval = value.RecurrenceInterval
				};
			}
			return null;
		}

		private readonly IDayOfWeekConverter dayOfWeekConverter;

		private readonly IWeekIndexConverter weekIndexConverter;
	}
}
