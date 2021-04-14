using System;
using Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal static class PatternedRecurrenceConverter
	{
		internal static PatternedRecurrence ToPatternedRecurrence(this PatternedRecurrence dataEntityPatternedRecurrence)
		{
			if (dataEntityPatternedRecurrence == null)
			{
				return null;
			}
			PatternedRecurrence patternedRecurrence = new PatternedRecurrence();
			RecurrencePattern pattern = dataEntityPatternedRecurrence.Pattern;
			if (pattern != null)
			{
				RecurrencePattern recurrencePattern = new RecurrencePattern();
				recurrencePattern.Interval = pattern.Interval;
				if (pattern is AbsoluteMonthlyRecurrencePattern)
				{
					AbsoluteMonthlyRecurrencePattern absoluteMonthlyRecurrencePattern = (AbsoluteMonthlyRecurrencePattern)pattern;
					recurrencePattern.Type = RecurrencePatternType.AbsoluteMonthly;
					recurrencePattern.DayOfMonth = absoluteMonthlyRecurrencePattern.DayOfMonth;
				}
				else if (pattern is AbsoluteYearlyRecurrencePattern)
				{
					AbsoluteYearlyRecurrencePattern absoluteYearlyRecurrencePattern = (AbsoluteYearlyRecurrencePattern)pattern;
					recurrencePattern.Type = RecurrencePatternType.AbsoluteYearly;
					recurrencePattern.DayOfMonth = absoluteYearlyRecurrencePattern.DayOfMonth;
					recurrencePattern.Month = absoluteYearlyRecurrencePattern.Month;
				}
				else if (pattern is DailyRecurrencePattern)
				{
					recurrencePattern.Type = RecurrencePatternType.Daily;
				}
				else if (pattern is RelativeMonthlyRecurrencePattern)
				{
					RelativeMonthlyRecurrencePattern relativeMonthlyRecurrencePattern = (RelativeMonthlyRecurrencePattern)pattern;
					recurrencePattern.Type = RecurrencePatternType.RelativeMonthly;
					recurrencePattern.DaysOfWeek = relativeMonthlyRecurrencePattern.DaysOfWeek;
					recurrencePattern.Index = EnumConverter.CastEnumType<WeekIndex>(relativeMonthlyRecurrencePattern.Index);
				}
				else if (pattern is RelativeYearlyRecurrencePattern)
				{
					RelativeYearlyRecurrencePattern relativeYearlyRecurrencePattern = (RelativeYearlyRecurrencePattern)pattern;
					recurrencePattern.Type = RecurrencePatternType.RelativeYearly;
					recurrencePattern.DaysOfWeek = relativeYearlyRecurrencePattern.DaysOfWeek;
					recurrencePattern.Index = EnumConverter.CastEnumType<WeekIndex>(relativeYearlyRecurrencePattern.Index);
					recurrencePattern.Month = relativeYearlyRecurrencePattern.Month;
				}
				else
				{
					if (!(pattern is WeeklyRecurrencePattern))
					{
						throw new InvalidOperationException(string.Format("Unknown RecurrencePattern type {0}", pattern.Type));
					}
					WeeklyRecurrencePattern weeklyRecurrencePattern = (WeeklyRecurrencePattern)pattern;
					recurrencePattern.Type = RecurrencePatternType.Weekly;
					recurrencePattern.DaysOfWeek = weeklyRecurrencePattern.DaysOfWeek;
					recurrencePattern.Interval = weeklyRecurrencePattern.Interval;
					recurrencePattern.FirstDayOfWeek = weeklyRecurrencePattern.FirstDayOfWeek;
				}
				patternedRecurrence.Pattern = recurrencePattern;
			}
			RecurrenceRange range = dataEntityPatternedRecurrence.Range;
			if (range != null)
			{
				RecurrenceRange recurrenceRange = new RecurrenceRange();
				recurrenceRange.StartDate = range.StartDate.ToDateTimeOffset();
				if (range is EndDateRecurrenceRange)
				{
					EndDateRecurrenceRange endDateRecurrenceRange = (EndDateRecurrenceRange)range;
					recurrenceRange.Type = RecurrenceRangeType.EndDate;
					recurrenceRange.EndDate = endDateRecurrenceRange.EndDate.ToDateTimeOffset();
				}
				else if (range is NoEndRecurrenceRange)
				{
					NoEndRecurrenceRange noEndRecurrenceRange = (NoEndRecurrenceRange)range;
					recurrenceRange.Type = RecurrenceRangeType.NoEnd;
				}
				else
				{
					if (!(range is NumberedRecurrenceRange))
					{
						throw new InvalidOperationException(string.Format("Unknown RecurrenceRange type {0}", range.Type));
					}
					NumberedRecurrenceRange numberedRecurrenceRange = (NumberedRecurrenceRange)range;
					recurrenceRange.NumberOfOccurrences = numberedRecurrenceRange.NumberOfOccurrences;
				}
				patternedRecurrence.Range = recurrenceRange;
			}
			return patternedRecurrence;
		}

		internal static PatternedRecurrence ToDataEntityPatternedRecurrence(this PatternedRecurrence patternedRecurrence)
		{
			if (patternedRecurrence == null)
			{
				return null;
			}
			PatternedRecurrence patternedRecurrence2 = new PatternedRecurrence();
			RecurrencePattern pattern = patternedRecurrence.Pattern;
			if (pattern != null)
			{
				RecurrencePattern recurrencePattern;
				switch (pattern.Type)
				{
				case RecurrencePatternType.Daily:
					recurrencePattern = new DailyRecurrencePattern();
					break;
				case RecurrencePatternType.Weekly:
					recurrencePattern = new WeeklyRecurrencePattern
					{
						DaysOfWeek = pattern.DaysOfWeek,
						FirstDayOfWeek = pattern.FirstDayOfWeek
					};
					break;
				case RecurrencePatternType.AbsoluteMonthly:
					recurrencePattern = new AbsoluteMonthlyRecurrencePattern
					{
						DayOfMonth = pattern.DayOfMonth
					};
					break;
				case RecurrencePatternType.RelativeMonthly:
					recurrencePattern = new RelativeMonthlyRecurrencePattern
					{
						DaysOfWeek = pattern.DaysOfWeek,
						Index = EnumConverter.CastEnumType<WeekIndex>(pattern.Index)
					};
					break;
				case RecurrencePatternType.AbsoluteYearly:
					recurrencePattern = new AbsoluteYearlyRecurrencePattern
					{
						DayOfMonth = pattern.DayOfMonth,
						Month = pattern.Month
					};
					break;
				case RecurrencePatternType.RelativeYearly:
					recurrencePattern = new RelativeYearlyRecurrencePattern
					{
						DaysOfWeek = pattern.DaysOfWeek,
						Index = EnumConverter.CastEnumType<WeekIndex>(pattern.Index),
						Month = pattern.Month
					};
					break;
				default:
					throw new InvalidOperationException(string.Format("Unknown RecurrencePattern type {0}", pattern.Type));
				}
				recurrencePattern.Interval = pattern.Interval;
				patternedRecurrence2.Pattern = recurrencePattern;
			}
			RecurrenceRange range = patternedRecurrence.Range;
			if (range != null)
			{
				RecurrenceRange recurrenceRange;
				switch (range.Type)
				{
				case RecurrenceRangeType.EndDate:
					recurrenceRange = new EndDateRecurrenceRange
					{
						EndDate = range.EndDate.ToExDateTime()
					};
					break;
				case RecurrenceRangeType.NoEnd:
					recurrenceRange = new NoEndRecurrenceRange();
					break;
				case RecurrenceRangeType.Numbered:
					recurrenceRange = new NumberedRecurrenceRange
					{
						NumberOfOccurrences = range.NumberOfOccurrences
					};
					break;
				default:
					throw new InvalidOperationException(string.Format("Unknown RecurrenceRange type {0}", range.Type));
				}
				recurrenceRange.StartDate = range.StartDate.ToExDateTime();
				patternedRecurrence2.Range = recurrenceRange;
			}
			return patternedRecurrence2;
		}
	}
}
