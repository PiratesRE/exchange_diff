using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters.Recurrence
{
	internal class PatternConverter : IConverter<RecurrencePattern, RecurrencePattern>, IConverter<RecurrencePattern, RecurrencePattern>
	{
		public RecurrencePattern Convert(RecurrencePattern value)
		{
			if (value == null)
			{
				return null;
			}
			DailyRecurrencePattern dailyRecurrencePattern = value as DailyRecurrencePattern;
			if (dailyRecurrencePattern != null)
			{
				return PatternConverter.dailyPatternConverter.ConvertStorageToEntities(dailyRecurrencePattern);
			}
			WeeklyRecurrencePattern weeklyRecurrencePattern = value as WeeklyRecurrencePattern;
			if (weeklyRecurrencePattern != null)
			{
				return PatternConverter.weeklyPatternConverter.ConvertStorageToEntities(weeklyRecurrencePattern);
			}
			MonthlyRecurrencePattern monthlyRecurrencePattern = value as MonthlyRecurrencePattern;
			if (monthlyRecurrencePattern != null)
			{
				return PatternConverter.monthlyPatternConverter.ConvertStorageToEntities(monthlyRecurrencePattern);
			}
			MonthlyThRecurrencePattern monthlyThRecurrencePattern = value as MonthlyThRecurrencePattern;
			if (monthlyThRecurrencePattern != null)
			{
				return PatternConverter.monthlyPatternConverter.ConvertStorageToEntities(monthlyThRecurrencePattern);
			}
			YearlyRecurrencePattern yearlyRecurrencePattern = value as YearlyRecurrencePattern;
			if (yearlyRecurrencePattern != null)
			{
				return PatternConverter.yearlyPatternConverter.ConvertStorageToEntities(yearlyRecurrencePattern);
			}
			YearlyThRecurrencePattern yearlyThRecurrencePattern = value as YearlyThRecurrencePattern;
			if (yearlyThRecurrencePattern != null)
			{
				return PatternConverter.yearlyPatternConverter.ConvertStorageToEntities(yearlyThRecurrencePattern);
			}
			if (value is DailyRegeneratingPattern || value is WeeklyRegeneratingPattern || value is MonthlyRegeneratingPattern || value is YearlyRegeneratingPattern)
			{
				throw new NotImplementedException("Regenerating tasks are not implemented in Entities yet.");
			}
			throw new ArgumentValueCannotBeParsedException("value", value.GetType().FullName, typeof(RecurrencePattern).FullName);
		}

		public RecurrencePattern Convert(RecurrencePattern value)
		{
			if (value == null)
			{
				return null;
			}
			switch (value.Type)
			{
			case RecurrencePatternType.Daily:
				return PatternConverter.dailyPatternConverter.ConvertEntitiesToStorage((DailyRecurrencePattern)value);
			case RecurrencePatternType.Weekly:
				return PatternConverter.weeklyPatternConverter.ConvertEntitiesToStorage((WeeklyRecurrencePattern)value);
			case RecurrencePatternType.AbsoluteMonthly:
				return PatternConverter.monthlyPatternConverter.ConvertEntitiesToStorage((AbsoluteMonthlyRecurrencePattern)value);
			case RecurrencePatternType.RelativeMonthly:
				return PatternConverter.monthlyPatternConverter.ConvertEntitiesToStorage((RelativeMonthlyRecurrencePattern)value);
			case RecurrencePatternType.AbsoluteYearly:
				return PatternConverter.yearlyPatternConverter.ConvertEntitiesToStorage((AbsoluteYearlyRecurrencePattern)value);
			case RecurrencePatternType.RelativeYearly:
				return PatternConverter.yearlyPatternConverter.ConvertEntitiesToStorage((RelativeYearlyRecurrencePattern)value);
			default:
				throw new ArgumentValueCannotBeParsedException("value.Type", value.Type.ToString(), value.GetType().FullName);
			}
		}

		private static readonly DayOfWeekConverter DayOfWeekConverter = default(DayOfWeekConverter);

		private static readonly WeekIndexConverter WeekIndexConverter = default(WeekIndexConverter);

		private static DailyRecurrencePatternConverter dailyPatternConverter = default(DailyRecurrencePatternConverter);

		private static WeeklyRecurrencePatternConverter weeklyPatternConverter = new WeeklyRecurrencePatternConverter(PatternConverter.DayOfWeekConverter);

		private static MonthlyRecurrencePatternConverter monthlyPatternConverter = new MonthlyRecurrencePatternConverter(PatternConverter.DayOfWeekConverter, PatternConverter.WeekIndexConverter);

		private static YearlyRecurrencePatternConverter yearlyPatternConverter = new YearlyRecurrencePatternConverter(PatternConverter.DayOfWeekConverter, PatternConverter.WeekIndexConverter);
	}
}
