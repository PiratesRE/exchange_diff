using System;
using System.Collections.Generic;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring.Reporting
{
	internal sealed class Utils
	{
		public static void GetStartEndDateForReportingPeriod(ReportingPeriod reportingPeriod, out ExDateTime startDateTime, out ExDateTime endDateTime)
		{
			ExDateTime date = ExDateTime.UtcNow.Date;
			switch (reportingPeriod)
			{
			case ReportingPeriod.Today:
				startDateTime = date;
				endDateTime = startDateTime.Add(TimeSpan.FromDays(1.0)).Subtract(TimeSpan.FromSeconds(1.0));
				return;
			case ReportingPeriod.Yesterday:
				startDateTime = date.Subtract(TimeSpan.FromDays(1.0));
				endDateTime = startDateTime.AddDays(1.0).Subtract(TimeSpan.FromSeconds(1.0));
				return;
			case ReportingPeriod.LastWeek:
				startDateTime = date.Subtract(TimeSpan.FromDays((double)(7 + date.DayOfWeek)));
				endDateTime = date.Subtract(TimeSpan.FromDays((double)date.DayOfWeek)).Subtract(TimeSpan.FromSeconds(1.0));
				return;
			case ReportingPeriod.LastMonth:
				startDateTime = Utils.SubtractMonths(date, 1);
				endDateTime = Utils.GetLastMonthLastDate(date);
				return;
			case ReportingPeriod.Last3Months:
				startDateTime = Utils.SubtractMonths(date, 3);
				endDateTime = Utils.GetLastMonthLastDate(date);
				return;
			case ReportingPeriod.Last6Months:
				startDateTime = Utils.SubtractMonths(date, 6);
				endDateTime = Utils.GetLastMonthLastDate(date);
				return;
			case ReportingPeriod.Last12Months:
				startDateTime = Utils.SubtractMonths(date, 12);
				endDateTime = Utils.GetLastMonthLastDate(date);
				return;
			default:
				throw new ArgumentException(Strings.InvalidReportingPeriod);
			}
		}

		public static IEnumerable<StartEndDateTimePair> GetAllDaysInGivenRange(ExDateTime startDate, ExDateTime endDate)
		{
			List<StartEndDateTimePair> list = new List<StartEndDateTimePair>();
			if (endDate.Date < startDate.Date)
			{
				throw new ArgumentException(Strings.InvalidTimeRange, "StartDate");
			}
			if (endDate.Date == startDate.Date)
			{
				list.Add(new StartEndDateTimePair(startDate.Date, startDate.Date.AddDays(1.0).Subtract(TimeSpan.FromSeconds(1.0))));
			}
			else if (endDate.Date > startDate.Date)
			{
				ExDateTime t = startDate.Date;
				while (t <= endDate.Date)
				{
					list.Add(new StartEndDateTimePair(t.Date, t.Date.AddDays(1.0).Subtract(TimeSpan.FromSeconds(1.0))));
					t = t.AddDays(1.0);
				}
			}
			return list;
		}

		private static ExDateTime SubtractMonths(ExDateTime dateTime, int monthsToSubtract)
		{
			int num = dateTime.Year;
			int num2 = dateTime.Month;
			num2 -= monthsToSubtract;
			if (num2 <= 0)
			{
				num--;
				num2 += 12;
			}
			return new ExDateTime(ExTimeZone.UtcTimeZone, num, num2, 1);
		}

		private static ExDateTime GetLastMonthLastDate(ExDateTime datetime)
		{
			return new ExDateTime(ExTimeZone.UtcTimeZone, datetime.Year, datetime.Month, 1).Subtract(TimeSpan.FromSeconds(1.0));
		}

		private const int TotalDaysInWeek = 7;
	}
}
