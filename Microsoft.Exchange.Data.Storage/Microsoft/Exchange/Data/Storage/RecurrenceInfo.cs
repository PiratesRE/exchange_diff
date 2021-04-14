using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RecurrenceInfo
	{
		private RecurrenceInfo()
		{
			this.Period = 1;
			this.FirstDayOfWeek = 0;
		}

		public RecurrenceGroup Group { get; private set; }

		public RecurrenceTypeInBlob Type { get; private set; }

		public int Period { get; private set; }

		public DaysOfWeek DayMask { get; private set; }

		public int DayOfMonth { get; private set; }

		public RecurrenceOrderType NthOccurrence { get; private set; }

		public int MonthOfYear { get; private set; }

		public RecurrenceRangeType Range { get; private set; }

		public int NumberOfOccurrences { get; private set; }

		public int FirstDayOfWeek { get; private set; }

		public ExDateTime StartDate { get; private set; }

		public ExDateTime EndDate { get; private set; }

		public ExDateTime[] DeletedOccurrences { get; private set; }

		public IList<OccurrenceInfo> ModifiedOccurrences { get; private set; }

		public AnomaliesFlags Anomalies { get; private set; }

		internal static RecurrenceInfo GetInfo(Recurrence recurrenceObject)
		{
			RecurrenceInfo recurrenceInfo = null;
			InternalRecurrence internalRecurrence = recurrenceObject as InternalRecurrence;
			if (internalRecurrence != null)
			{
				recurrenceInfo = new RecurrenceInfo();
				recurrenceInfo.StartDate = internalRecurrence.Range.StartDate;
				recurrenceInfo.EndDate = internalRecurrence.EndDate;
				if (internalRecurrence.Range is EndDateRecurrenceRange)
				{
					recurrenceInfo.Range = RecurrenceRangeType.End;
					recurrenceInfo.EndDate = ((EndDateRecurrenceRange)internalRecurrence.Range).EndDate;
				}
				else if (internalRecurrence.Range is NumberedRecurrenceRange)
				{
					recurrenceInfo.Range = RecurrenceRangeType.AfterNOccur;
					recurrenceInfo.NumberOfOccurrences = ((NumberedRecurrenceRange)internalRecurrence.Range).NumberOfOccurrences;
				}
				else
				{
					recurrenceInfo.Range = RecurrenceRangeType.NoEnd;
				}
				recurrenceInfo.SetPatternSpecificProperties(internalRecurrence.Pattern);
				recurrenceInfo.Anomalies = internalRecurrence.Anomalies;
				recurrenceInfo.ModifiedOccurrences = internalRecurrence.GetModifiedOccurrences();
				recurrenceInfo.DeletedOccurrences = internalRecurrence.GetDeletedOccurrences();
			}
			return recurrenceInfo;
		}

		private void SetPatternSpecificProperties(RecurrencePattern pattern)
		{
			if (pattern is DailyRecurrencePattern)
			{
				DailyRecurrencePattern dailyRecurrencePattern = (DailyRecurrencePattern)pattern;
				this.Group = RecurrenceGroup.Daily;
				this.Type = RecurrenceTypeInBlob.Minute;
				this.Period = dailyRecurrencePattern.RecurrenceInterval * 24 * 60;
				return;
			}
			if (pattern is WeeklyRecurrencePattern)
			{
				WeeklyRecurrencePattern weeklyRecurrencePattern = (WeeklyRecurrencePattern)pattern;
				this.Group = RecurrenceGroup.Weekly;
				this.Type = RecurrenceTypeInBlob.Week;
				this.Period = weeklyRecurrencePattern.RecurrenceInterval;
				this.DayMask = weeklyRecurrencePattern.DaysOfWeek;
				this.FirstDayOfWeek = (int)((WeeklyRecurrencePattern)pattern).FirstDayOfWeek;
				return;
			}
			if (pattern is MonthlyRecurrencePattern)
			{
				MonthlyRecurrencePattern monthlyRecurrencePattern = (MonthlyRecurrencePattern)pattern;
				this.Group = RecurrenceGroup.Monthly;
				this.Type = ((monthlyRecurrencePattern.CalendarType == CalendarType.Hijri) ? RecurrenceTypeInBlob.HjMonth : RecurrenceTypeInBlob.Month);
				this.Period = monthlyRecurrencePattern.RecurrenceInterval;
				this.DayOfMonth = monthlyRecurrencePattern.DayOfMonth;
				return;
			}
			if (pattern is YearlyRecurrencePattern)
			{
				YearlyRecurrencePattern yearlyRecurrencePattern = (YearlyRecurrencePattern)pattern;
				this.Group = RecurrenceGroup.Yearly;
				this.Type = ((yearlyRecurrencePattern.CalendarType == CalendarType.Hijri) ? RecurrenceTypeInBlob.HjMonth : RecurrenceTypeInBlob.Month);
				this.DayOfMonth = yearlyRecurrencePattern.DayOfMonth;
				this.Period = 12;
				return;
			}
			if (pattern is MonthlyThRecurrencePattern)
			{
				MonthlyThRecurrencePattern monthlyThRecurrencePattern = (MonthlyThRecurrencePattern)pattern;
				this.Group = RecurrenceGroup.Monthly;
				this.Type = ((monthlyThRecurrencePattern.CalendarType == CalendarType.Hijri) ? RecurrenceTypeInBlob.HjMonthNth : RecurrenceTypeInBlob.MonthNth);
				this.DayMask = monthlyThRecurrencePattern.DaysOfWeek;
				this.NthOccurrence = monthlyThRecurrencePattern.Order;
				this.Period = monthlyThRecurrencePattern.RecurrenceInterval;
				return;
			}
			if (pattern is YearlyThRecurrencePattern)
			{
				YearlyThRecurrencePattern yearlyThRecurrencePattern = (YearlyThRecurrencePattern)pattern;
				this.Group = RecurrenceGroup.Yearly;
				this.Type = ((yearlyThRecurrencePattern.CalendarType == CalendarType.Hijri) ? RecurrenceTypeInBlob.HjMonthNth : RecurrenceTypeInBlob.MonthNth);
				this.DayMask = yearlyThRecurrencePattern.DaysOfWeek;
				this.NthOccurrence = yearlyThRecurrencePattern.Order;
				this.MonthOfYear = yearlyThRecurrencePattern.Month;
				this.Period = 12;
				return;
			}
			if (pattern is DailyRegeneratingPattern)
			{
				DailyRegeneratingPattern dailyRegeneratingPattern = (DailyRegeneratingPattern)pattern;
				this.Type = RecurrenceTypeInBlob.Minute;
				this.Group = RecurrenceGroup.Daily;
				this.Period = dailyRegeneratingPattern.RecurrenceInterval * 24 * 60;
				return;
			}
			if (pattern is WeeklyRegeneratingPattern)
			{
				WeeklyRegeneratingPattern weeklyRegeneratingPattern = (WeeklyRegeneratingPattern)pattern;
				this.Type = RecurrenceTypeInBlob.Minute;
				this.Group = RecurrenceGroup.Weekly;
				this.DayMask = DaysOfWeek.Monday;
				this.Period = weeklyRegeneratingPattern.RecurrenceInterval * 7 * 24 * 60;
				return;
			}
			if (pattern is MonthlyRegeneratingPattern)
			{
				MonthlyRegeneratingPattern monthlyRegeneratingPattern = (MonthlyRegeneratingPattern)pattern;
				this.Type = RecurrenceTypeInBlob.MonthNth;
				this.Group = RecurrenceGroup.Monthly;
				this.DayMask = DaysOfWeek.Monday;
				this.NthOccurrence = (RecurrenceOrderType)0;
				this.Period = monthlyRegeneratingPattern.RecurrenceInterval;
				return;
			}
			if (pattern is YearlyRegeneratingPattern)
			{
				YearlyRegeneratingPattern yearlyRegeneratingPattern = (YearlyRegeneratingPattern)pattern;
				this.Type = RecurrenceTypeInBlob.Month;
				this.Group = RecurrenceGroup.Yearly;
				this.DayOfMonth = 1;
				this.Period = 12 * yearlyRegeneratingPattern.RecurrenceInterval;
			}
		}
	}
}
