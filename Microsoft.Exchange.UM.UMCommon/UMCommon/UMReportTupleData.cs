using System;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.UM.UMCommon
{
	[DataContract(Name = "UMReportTupleData", Namespace = "http://schemas.microsoft.com/v1.0/UMReportAggregatedData")]
	internal class UMReportTupleData
	{
		public UMReportTupleData()
		{
			this.dailyReport = new DailyReportDictionary();
			this.monthlyReport = new MonthlyReportDictionary();
			this.totalReport = new UMReportRawCounters(default(DateTime));
		}

		public void AddCDR(CDRData cdrData)
		{
			this.AddCDRToDailyData(cdrData);
			this.AddCDRToMonthlyData(cdrData);
			this.AddCDRToTotalData(cdrData);
		}

		public void CleanUp()
		{
			this.dailyReport = this.CleanupHelper<DailyReportDictionary>(this.dailyReport, true);
			this.monthlyReport = this.CleanupHelper<MonthlyReportDictionary>(this.monthlyReport, false);
		}

		private T CleanupHelper<T>(T toClean, bool addDays) where T : UMReportDictionaryBase, new()
		{
			if (toClean.Count == 0)
			{
				return toClean;
			}
			DateTime dateTime;
			if (addDays)
			{
				dateTime = this.GetDateTimeValue(ExTimeZone.UtcTimeZone, ExDateTime.UtcNow.Year, ExDateTime.UtcNow.Month, ExDateTime.UtcNow.Day);
			}
			else
			{
				dateTime = this.GetDateTimeValue(ExTimeZone.UtcTimeZone, ExDateTime.UtcNow.Year, ExDateTime.UtcNow.Month, 1);
			}
			DateTime t = dateTime;
			foreach (DateTime dateTime2 in toClean.Keys)
			{
				if (dateTime2 < t)
				{
					t = dateTime2;
				}
			}
			T result = Activator.CreateInstance<T>();
			while (result.Count < result.MaxItemsInDictionary && dateTime >= t)
			{
				UMReportRawCounters value;
				if (!toClean.TryGetValue(dateTime, out value))
				{
					value = new UMReportRawCounters(dateTime);
				}
				result.Add(dateTime, value);
				dateTime = (addDays ? dateTime.AddDays(-1.0) : dateTime.AddMonths(-1));
			}
			return result;
		}

		public UMReportRawCounters[] QueryReport(GroupBy groupBy)
		{
			switch (groupBy)
			{
			case GroupBy.Day:
				return (from x in this.dailyReport.Values
				orderby x.Date descending
				select x).ToArray<UMReportRawCounters>();
			case GroupBy.Month:
				return (from x in this.monthlyReport.Values
				orderby x.Date descending
				select x).ToArray<UMReportRawCounters>();
			case GroupBy.Total:
				return new UMReportRawCounters[]
				{
					this.totalReport
				};
			default:
				throw new NotImplementedException("Unknown value for GroupBy");
			}
		}

		private void AddCDRToMonthlyData(CDRData cdrData)
		{
			DateTime dateTimeValue = this.GetDateTimeValue(ExTimeZone.UtcTimeZone, cdrData.CallStartTime.Year, cdrData.CallStartTime.Month, 1);
			UMReportRawCounters umreportRawCounters;
			if (!this.monthlyReport.TryGetValue(dateTimeValue, out umreportRawCounters))
			{
				umreportRawCounters = new UMReportRawCounters(dateTimeValue);
				this.monthlyReport.Add(dateTimeValue, umreportRawCounters);
			}
			umreportRawCounters.AddCDR(cdrData);
		}

		private void AddCDRToDailyData(CDRData cdrData)
		{
			DateTime dateTimeValue = this.GetDateTimeValue(ExTimeZone.UtcTimeZone, cdrData.CallStartTime.Year, cdrData.CallStartTime.Month, cdrData.CallStartTime.Day);
			UMReportRawCounters umreportRawCounters;
			if (!this.dailyReport.TryGetValue(dateTimeValue, out umreportRawCounters))
			{
				umreportRawCounters = new UMReportRawCounters(dateTimeValue);
				this.dailyReport.Add(dateTimeValue, umreportRawCounters);
			}
			umreportRawCounters.AddCDR(cdrData);
		}

		private void AddCDRToTotalData(CDRData cdrData)
		{
			this.totalReport.AddCDR(cdrData);
		}

		private DateTime GetDateTimeValue(ExTimeZone timeZone, int year, int month, int day)
		{
			ExDateTime exDateTime = new ExDateTime(timeZone, year, month, day, 0, 0, 0);
			return exDateTime.LocalTime;
		}

		[DataMember(Name = "DailyReport")]
		private DailyReportDictionary dailyReport;

		[DataMember(Name = "MonthlyReport")]
		private MonthlyReportDictionary monthlyReport;

		[DataMember(Name = "TotalReport")]
		private UMReportRawCounters totalReport;
	}
}
