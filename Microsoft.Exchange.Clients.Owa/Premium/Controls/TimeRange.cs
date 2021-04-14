using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class TimeRange
	{
		public TimeRange.RangeId Range
		{
			get
			{
				return this.range;
			}
		}

		public ExDateTime Start
		{
			get
			{
				return this.start;
			}
		}

		public ExDateTime End
		{
			get
			{
				return this.end;
			}
		}

		private TimeRange(TimeRange.RangeId range, ExDateTime start, ExDateTime end)
		{
			this.range = range;
			this.start = start;
			this.end = end;
		}

		public static List<TimeRange> GetTimeRanges(UserContext userContext)
		{
			TimeRange.RangeId rangeId = TimeRange.RangeId.All;
			ExDateTime normalizedDate = TimeRange.GetNormalizedDate(DateTimeUtilities.GetLocalTime());
			int num = (7 + (normalizedDate.DayOfWeek - userContext.UserOptions.WeekStartDay)) % 7;
			ExDateTime normalizedDate2 = TimeRange.GetNormalizedDate(normalizedDate.IncrementDays(1));
			ExDateTime normalizedDate3 = TimeRange.GetNormalizedDate(normalizedDate.IncrementDays(-1));
			ExDateTime normalizedDate4 = TimeRange.GetNormalizedDate(normalizedDate.IncrementDays(7 - num));
			ExDateTime normalizedDate5 = TimeRange.GetNormalizedDate(normalizedDate4.IncrementDays(7));
			ExDateTime normalizedDate6 = TimeRange.GetNormalizedDate(normalizedDate5.IncrementDays(7));
			ExDateTime normalizedDate7 = TimeRange.GetNormalizedDate(normalizedDate6.IncrementDays(7));
			ExDateTime normalizedDate8 = TimeRange.GetNormalizedDate(normalizedDate.IncrementDays(-1 * (7 + num)));
			ExDateTime normalizedDate9 = TimeRange.GetNormalizedDate(normalizedDate8.IncrementDays(-7));
			ExDateTime normalizedDate10 = TimeRange.GetNormalizedDate(normalizedDate9.IncrementDays(-7));
			if (normalizedDate7.Month != normalizedDate.Month)
			{
				rangeId &= ~TimeRange.RangeId.LaterThisMonth;
			}
			if (num != 6)
			{
				rangeId &= ~TimeRange.RangeId.Tomorrow;
			}
			if (num != 0)
			{
				rangeId &= ~TimeRange.RangeId.Yesterday;
			}
			if (normalizedDate10.Month != normalizedDate.Month || 1 >= normalizedDate10.Day)
			{
				rangeId &= ~TimeRange.RangeId.EarlierThisMonth;
			}
			List<TimeRange> list = new List<TimeRange>(18);
			ExDateTime exDateTime = new ExDateTime(userContext.TimeZone, normalizedDate.Year, normalizedDate.Month, 1);
			exDateTime = TimeRange.GetNormalizedDate(exDateTime.AddMonths(2));
			ExDateTime exDateTime2 = ExDateTime.MaxValue;
			list.Insert(0, new TimeRange(TimeRange.RangeId.BeyondNextMonth, exDateTime, exDateTime2));
			exDateTime2 = exDateTime;
			int num2 = ExDateTime.DaysInMonth(normalizedDate.Year, normalizedDate.Month) - normalizedDate.Day;
			if (21 < num2)
			{
				exDateTime = TimeRange.GetNormalizedDate(exDateTime2.AddMonths(-1));
			}
			else
			{
				exDateTime = TimeRange.GetNormalizedDate(normalizedDate6.IncrementDays(7));
			}
			list.Insert(0, new TimeRange(TimeRange.RangeId.NextMonth, exDateTime, exDateTime2));
			if ((TimeRange.RangeId)0 < (rangeId & TimeRange.RangeId.LaterThisMonth))
			{
				exDateTime2 = exDateTime;
				exDateTime = TimeRange.GetNormalizedDate(normalizedDate6.IncrementDays(7));
				list.Insert(0, new TimeRange(TimeRange.RangeId.LaterThisMonth, exDateTime, exDateTime2));
			}
			exDateTime2 = exDateTime;
			exDateTime = normalizedDate6;
			list.Insert(0, new TimeRange(TimeRange.RangeId.ThreeWeeksAway, exDateTime, exDateTime2));
			exDateTime2 = exDateTime;
			exDateTime = normalizedDate5;
			list.Insert(0, new TimeRange(TimeRange.RangeId.TwoWeeksAway, exDateTime, exDateTime2));
			exDateTime2 = exDateTime;
			if (num == 6)
			{
				exDateTime = TimeRange.GetNormalizedDate(normalizedDate4.IncrementDays(1));
			}
			else
			{
				exDateTime = normalizedDate4;
			}
			list.Insert(0, new TimeRange(TimeRange.RangeId.NextWeek, exDateTime, exDateTime2));
			if ((TimeRange.RangeId)0 < (rangeId & TimeRange.RangeId.Tomorrow))
			{
				exDateTime2 = exDateTime;
				exDateTime = TimeRange.GetNormalizedDate(exDateTime.IncrementDays(-1));
				list.Insert(0, new TimeRange(TimeRange.RangeId.Tomorrow, exDateTime, exDateTime2));
			}
			int num3 = 7;
			while (0 < num3)
			{
				exDateTime2 = exDateTime;
				exDateTime = TimeRange.GetNormalizedDate(exDateTime.IncrementDays(-1));
				TimeRange.RangeId rangeId2 = TimeRange.RangeId.None;
				if (normalizedDate2.Equals(exDateTime))
				{
					rangeId2 = TimeRange.RangeId.Tomorrow;
				}
				else if (normalizedDate.Equals(exDateTime))
				{
					rangeId2 = TimeRange.RangeId.Today;
				}
				else if (normalizedDate3.Equals(exDateTime))
				{
					rangeId2 = TimeRange.RangeId.Yesterday;
				}
				else
				{
					switch (exDateTime.DayOfWeek)
					{
					case DayOfWeek.Sunday:
						rangeId2 = TimeRange.RangeId.Sunday;
						break;
					case DayOfWeek.Monday:
						rangeId2 = TimeRange.RangeId.Monday;
						break;
					case DayOfWeek.Tuesday:
						rangeId2 = TimeRange.RangeId.Tuesday;
						break;
					case DayOfWeek.Wednesday:
						rangeId2 = TimeRange.RangeId.Wednesday;
						break;
					case DayOfWeek.Thursday:
						rangeId2 = TimeRange.RangeId.Thursday;
						break;
					case DayOfWeek.Friday:
						rangeId2 = TimeRange.RangeId.Friday;
						break;
					case DayOfWeek.Saturday:
						rangeId2 = TimeRange.RangeId.Saturday;
						break;
					}
				}
				list.Insert(0, new TimeRange(rangeId2, exDateTime, exDateTime2));
				num3--;
			}
			if ((TimeRange.RangeId)0 < (rangeId & TimeRange.RangeId.Yesterday))
			{
				exDateTime2 = exDateTime;
				exDateTime = TimeRange.GetNormalizedDate(exDateTime.IncrementDays(-1));
				list.Insert(0, new TimeRange(TimeRange.RangeId.Yesterday, exDateTime, exDateTime2));
			}
			exDateTime2 = exDateTime;
			exDateTime = normalizedDate8;
			list.Insert(0, new TimeRange(TimeRange.RangeId.LastWeek, exDateTime, exDateTime2));
			exDateTime2 = exDateTime;
			exDateTime = normalizedDate9;
			list.Insert(0, new TimeRange(TimeRange.RangeId.TwoWeeksAgo, exDateTime, exDateTime2));
			exDateTime2 = exDateTime;
			exDateTime = normalizedDate10;
			list.Insert(0, new TimeRange(TimeRange.RangeId.ThreeWeeksAgo, exDateTime, exDateTime2));
			if ((TimeRange.RangeId)0 < (rangeId & TimeRange.RangeId.EarlierThisMonth))
			{
				exDateTime2 = exDateTime;
				exDateTime = TimeRange.GetNormalizedDate(exDateTime.IncrementDays(-1 * (exDateTime.Day - 1)));
				list.Insert(0, new TimeRange(TimeRange.RangeId.EarlierThisMonth, exDateTime, exDateTime2));
			}
			exDateTime2 = exDateTime;
			if (exDateTime2.Day == 1)
			{
				exDateTime = TimeRange.GetNormalizedDate(exDateTime.IncrementDays(-1));
			}
			exDateTime = TimeRange.GetNormalizedDate(exDateTime.IncrementDays(-1 * (exDateTime.Day - 1)));
			list.Insert(0, new TimeRange(TimeRange.RangeId.LastMonth, exDateTime, exDateTime2));
			exDateTime2 = exDateTime;
			exDateTime = ExDateTime.MinValue.AddTicks(1L);
			list.Insert(0, new TimeRange(TimeRange.RangeId.Older, exDateTime, exDateTime2));
			list.Insert(0, new TimeRange(TimeRange.RangeId.None, ExDateTime.MinValue, ExDateTime.MinValue));
			return list;
		}

		private static ExDateTime GetNormalizedDate(ExDateTime dateTime)
		{
			return dateTime.Date;
		}

		private TimeRange.RangeId range;

		private ExDateTime start;

		private ExDateTime end;

		[Flags]
		internal enum RangeId
		{
			Older = 1,
			LastMonth = 2,
			EarlierThisMonth = 4,
			ThreeWeeksAgo = 8,
			TwoWeeksAgo = 16,
			LastWeek = 32,
			Sunday = 64,
			Monday = 128,
			Tuesday = 256,
			Wednesday = 512,
			Thursday = 1024,
			Friday = 2048,
			Saturday = 4096,
			NextWeek = 8192,
			TwoWeeksAway = 16384,
			ThreeWeeksAway = 32768,
			LaterThisMonth = 65536,
			NextMonth = 131072,
			BeyondNextMonth = 262144,
			Yesterday = 524288,
			Today = 1048576,
			Tomorrow = 2097152,
			None = 4194304,
			All = 16777215
		}
	}
}
