using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal class CalendarItemData : CalendarItemBaseData
	{
		public Recurrence Recurrence
		{
			get
			{
				return this.recurrence;
			}
			set
			{
				this.recurrence = value;
			}
		}

		public CalendarItemData()
		{
		}

		public CalendarItemData(CalendarItem calendarItem)
		{
			this.SetFrom(calendarItem);
		}

		public CalendarItemData(CalendarItemBase calendarItemBase)
		{
			this.SetFrom(calendarItemBase);
		}

		public CalendarItemData(CalendarItemData other) : base(other)
		{
			this.recurrence = CalendarItemData.CloneRecurrence(other.recurrence);
		}

		public static bool IsRecurrenceEqual(Recurrence r1, Recurrence r2)
		{
			if (r1 == null != (r2 == null))
			{
				return false;
			}
			if (r1 != null)
			{
				if (!CalendarItemData.IsRecurrencePatternEqual(r1.Pattern, r2.Pattern))
				{
					return false;
				}
				if (!CalendarItemData.IsRecurrenceRangeEqual(r1.Range, r2.Range))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsRecurrencePatternEqual(RecurrencePattern p1, RecurrencePattern p2)
		{
			if (p1 == null != (p2 == null))
			{
				return false;
			}
			if (p1 != null)
			{
				if (!p1.GetType().Equals(p2.GetType()))
				{
					return false;
				}
				DailyRecurrencePattern dailyRecurrencePattern = p1 as DailyRecurrencePattern;
				if (dailyRecurrencePattern != null)
				{
					DailyRecurrencePattern dailyRecurrencePattern2 = p2 as DailyRecurrencePattern;
					return dailyRecurrencePattern.RecurrenceInterval == dailyRecurrencePattern2.RecurrenceInterval;
				}
				WeeklyRecurrencePattern weeklyRecurrencePattern = p1 as WeeklyRecurrencePattern;
				if (weeklyRecurrencePattern != null)
				{
					WeeklyRecurrencePattern weeklyRecurrencePattern2 = p2 as WeeklyRecurrencePattern;
					return weeklyRecurrencePattern.DaysOfWeek == weeklyRecurrencePattern2.DaysOfWeek && weeklyRecurrencePattern.FirstDayOfWeek == weeklyRecurrencePattern2.FirstDayOfWeek && weeklyRecurrencePattern.RecurrenceInterval == weeklyRecurrencePattern2.RecurrenceInterval;
				}
				MonthlyRecurrencePattern monthlyRecurrencePattern = p1 as MonthlyRecurrencePattern;
				if (monthlyRecurrencePattern != null)
				{
					MonthlyRecurrencePattern monthlyRecurrencePattern2 = p2 as MonthlyRecurrencePattern;
					return monthlyRecurrencePattern.CalendarType == monthlyRecurrencePattern2.CalendarType && monthlyRecurrencePattern.DayOfMonth == monthlyRecurrencePattern2.DayOfMonth && monthlyRecurrencePattern.RecurrenceInterval == monthlyRecurrencePattern2.RecurrenceInterval;
				}
				MonthlyThRecurrencePattern monthlyThRecurrencePattern = p1 as MonthlyThRecurrencePattern;
				if (monthlyThRecurrencePattern != null)
				{
					MonthlyThRecurrencePattern monthlyThRecurrencePattern2 = p2 as MonthlyThRecurrencePattern;
					return monthlyThRecurrencePattern.CalendarType == monthlyThRecurrencePattern2.CalendarType && monthlyThRecurrencePattern.DaysOfWeek == monthlyThRecurrencePattern2.DaysOfWeek && monthlyThRecurrencePattern.Order == monthlyThRecurrencePattern2.Order && monthlyThRecurrencePattern.RecurrenceInterval == monthlyThRecurrencePattern2.RecurrenceInterval;
				}
				YearlyRecurrencePattern yearlyRecurrencePattern = p1 as YearlyRecurrencePattern;
				if (yearlyRecurrencePattern != null)
				{
					YearlyRecurrencePattern yearlyRecurrencePattern2 = p2 as YearlyRecurrencePattern;
					return yearlyRecurrencePattern.CalendarType == yearlyRecurrencePattern2.CalendarType && yearlyRecurrencePattern.DayOfMonth == yearlyRecurrencePattern2.DayOfMonth && yearlyRecurrencePattern.IsLeapMonth == yearlyRecurrencePattern2.IsLeapMonth && yearlyRecurrencePattern.Month == yearlyRecurrencePattern2.Month;
				}
				YearlyThRecurrencePattern yearlyThRecurrencePattern = p1 as YearlyThRecurrencePattern;
				if (yearlyThRecurrencePattern != null)
				{
					YearlyThRecurrencePattern yearlyThRecurrencePattern2 = p2 as YearlyThRecurrencePattern;
					return yearlyThRecurrencePattern.CalendarType == yearlyThRecurrencePattern2.CalendarType && yearlyThRecurrencePattern.DaysOfWeek == yearlyThRecurrencePattern2.DaysOfWeek && yearlyThRecurrencePattern.IsLeapMonth == yearlyThRecurrencePattern2.IsLeapMonth && yearlyThRecurrencePattern.Month == yearlyThRecurrencePattern2.Month && yearlyThRecurrencePattern.Order == yearlyThRecurrencePattern2.Order;
				}
			}
			return true;
		}

		public static bool IsRecurrenceRangeEqual(RecurrenceRange r1, RecurrenceRange r2)
		{
			if (r1 == null != (r2 == null))
			{
				return false;
			}
			if (r1 != null)
			{
				if (!r1.GetType().Equals(r2.GetType()))
				{
					return false;
				}
				if (r1.StartDate != r2.StartDate)
				{
					return false;
				}
				EndDateRecurrenceRange endDateRecurrenceRange = r1 as EndDateRecurrenceRange;
				if (endDateRecurrenceRange != null)
				{
					EndDateRecurrenceRange endDateRecurrenceRange2 = r2 as EndDateRecurrenceRange;
					return !(endDateRecurrenceRange.EndDate != endDateRecurrenceRange2.EndDate);
				}
				NumberedRecurrenceRange numberedRecurrenceRange = r1 as NumberedRecurrenceRange;
				if (numberedRecurrenceRange != null)
				{
					NumberedRecurrenceRange numberedRecurrenceRange2 = r2 as NumberedRecurrenceRange;
					return numberedRecurrenceRange.NumberOfOccurrences == numberedRecurrenceRange2.NumberOfOccurrences;
				}
			}
			return true;
		}

		public static Recurrence CloneRecurrence(Recurrence recurrence)
		{
			Recurrence result = null;
			if (recurrence != null)
			{
				if (recurrence.CreatedExTimeZone != ExTimeZone.UtcTimeZone && recurrence.ReadExTimeZone != ExTimeZone.UtcTimeZone)
				{
					result = new Recurrence(CalendarItemData.CloneRecurrencePattern(recurrence.Pattern), CalendarItemData.CloneRecurrenceRange(recurrence.Range), recurrence.CreatedExTimeZone, recurrence.ReadExTimeZone);
				}
				else
				{
					result = new Recurrence(CalendarItemData.CloneRecurrencePattern(recurrence.Pattern), CalendarItemData.CloneRecurrenceRange(recurrence.Range));
				}
			}
			return result;
		}

		public static RecurrenceRange CloneRecurrenceRange(RecurrenceRange range)
		{
			RecurrenceRange result = null;
			if (range == null)
			{
				return result;
			}
			EndDateRecurrenceRange endDateRecurrenceRange = range as EndDateRecurrenceRange;
			if (endDateRecurrenceRange != null)
			{
				return new EndDateRecurrenceRange(endDateRecurrenceRange.StartDate, endDateRecurrenceRange.EndDate);
			}
			NoEndRecurrenceRange noEndRecurrenceRange = range as NoEndRecurrenceRange;
			if (noEndRecurrenceRange != null)
			{
				return new NoEndRecurrenceRange(noEndRecurrenceRange.StartDate);
			}
			NumberedRecurrenceRange numberedRecurrenceRange = range as NumberedRecurrenceRange;
			if (numberedRecurrenceRange != null)
			{
				return new NumberedRecurrenceRange(numberedRecurrenceRange.StartDate, numberedRecurrenceRange.NumberOfOccurrences);
			}
			throw new ArgumentException("Unhandled RecurrenceRange type.");
		}

		public static RecurrencePattern CloneRecurrencePattern(RecurrencePattern pattern)
		{
			RecurrencePattern result = null;
			if (pattern == null)
			{
				return result;
			}
			DailyRecurrencePattern dailyRecurrencePattern = pattern as DailyRecurrencePattern;
			if (dailyRecurrencePattern != null)
			{
				return new DailyRecurrencePattern(dailyRecurrencePattern.RecurrenceInterval);
			}
			MonthlyRecurrencePattern monthlyRecurrencePattern = pattern as MonthlyRecurrencePattern;
			if (monthlyRecurrencePattern != null)
			{
				return new MonthlyRecurrencePattern(monthlyRecurrencePattern.DayOfMonth, monthlyRecurrencePattern.RecurrenceInterval, monthlyRecurrencePattern.CalendarType);
			}
			MonthlyThRecurrencePattern monthlyThRecurrencePattern = pattern as MonthlyThRecurrencePattern;
			if (monthlyThRecurrencePattern != null)
			{
				return new MonthlyThRecurrencePattern(monthlyThRecurrencePattern.DaysOfWeek, monthlyThRecurrencePattern.Order, monthlyThRecurrencePattern.RecurrenceInterval, monthlyThRecurrencePattern.CalendarType);
			}
			WeeklyRecurrencePattern weeklyRecurrencePattern = pattern as WeeklyRecurrencePattern;
			if (weeklyRecurrencePattern != null)
			{
				return new WeeklyRecurrencePattern(weeklyRecurrencePattern.DaysOfWeek, weeklyRecurrencePattern.RecurrenceInterval, weeklyRecurrencePattern.FirstDayOfWeek);
			}
			YearlyRecurrencePattern yearlyRecurrencePattern = pattern as YearlyRecurrencePattern;
			if (yearlyRecurrencePattern != null)
			{
				return new YearlyRecurrencePattern(yearlyRecurrencePattern.DayOfMonth, yearlyRecurrencePattern.Month, yearlyRecurrencePattern.IsLeapMonth, yearlyRecurrencePattern.CalendarType);
			}
			YearlyThRecurrencePattern yearlyThRecurrencePattern = pattern as YearlyThRecurrencePattern;
			if (yearlyThRecurrencePattern != null)
			{
				return new YearlyThRecurrencePattern(yearlyThRecurrencePattern.DaysOfWeek, yearlyThRecurrencePattern.Order, yearlyThRecurrencePattern.Month, yearlyThRecurrencePattern.IsLeapMonth, yearlyThRecurrencePattern.CalendarType);
			}
			throw new ArgumentException("Unhandled RecurrencePattern type.");
		}

		public override void SetFrom(CalendarItemBase calendarItemBase)
		{
			base.SetFrom(calendarItemBase);
			CalendarItem calendarItem = calendarItemBase as CalendarItem;
			if (calendarItem != null)
			{
				this.recurrence = CalendarItemData.CloneRecurrence(calendarItem.Recurrence);
			}
		}

		public override EditCalendarItemHelper.CalendarItemUpdateFlags CopyTo(CalendarItemBase calendarItemBase)
		{
			EditCalendarItemHelper.CalendarItemUpdateFlags calendarItemUpdateFlags = base.CopyTo(calendarItemBase);
			CalendarItem calendarItem = calendarItemBase as CalendarItem;
			if (calendarItem != null && !CalendarItemData.IsRecurrenceEqual(calendarItem.Recurrence, this.recurrence))
			{
				calendarItem.Recurrence = CalendarItemData.CloneRecurrence(this.recurrence);
				calendarItemUpdateFlags |= EditCalendarItemHelper.CalendarItemUpdateFlags.OtherChanged;
			}
			return calendarItemUpdateFlags;
		}

		private Recurrence recurrence;
	}
}
