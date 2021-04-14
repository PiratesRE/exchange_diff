using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.ApplicationLogic.TimeZoneOffsets
{
	internal class TimeZoneOffsets
	{
		public static TimeZoneOffset[] GetTheTimeZoneOffsets(ExDateTime startTime, ExDateTime endTime, string timeZoneId = null, ExPerformanceCounter performanceCounter = null)
		{
			if (endTime < startTime)
			{
				throw new ArgumentException("End time must be greater than start time");
			}
			if (startTime.AddYears(100) < endTime)
			{
				throw new ArgumentException("Time range is too large: " + (endTime - startTime));
			}
			TimeZoneOffsets.TimeZoneOffsetTable timeZoneOffsetTable = TimeZoneOffsets.GetTableFromCache(startTime.Year, endTime.Year);
			if (timeZoneOffsetTable == null)
			{
				timeZoneOffsetTable = TimeZoneOffsets.BuildCache(startTime.Year, endTime.Year, performanceCounter);
				TimeZoneOffsets.AddTableToCache(timeZoneOffsetTable);
			}
			new List<TimeZoneOffset>();
			if (timeZoneId != null)
			{
				foreach (TimeZoneOffset timeZoneOffset in timeZoneOffsetTable.TimeZoneOffsets)
				{
					if (timeZoneOffset.TimeZoneId == timeZoneId)
					{
						return new TimeZoneOffset[]
						{
							timeZoneOffset
						};
					}
				}
				return null;
			}
			return timeZoneOffsetTable.TimeZoneOffsets;
		}

		private static TimeZoneOffsets.TimeZoneOffsetTable GetTableFromCache(int startYear, int endYear)
		{
			TimeZoneOffsets.TimeZoneOffsetTable timeZoneOffsetTable = null;
			lock (TimeZoneOffsets.cacheMutex)
			{
				int i = 0;
				while (i < TimeZoneOffsets.tableCache.Count)
				{
					timeZoneOffsetTable = TimeZoneOffsets.tableCache[i];
					if (timeZoneOffsetTable.StartYear == startYear && timeZoneOffsetTable.EndYear == endYear)
					{
						if (i > 0)
						{
							TimeZoneOffsets.tableCache.RemoveAt(i);
							TimeZoneOffsets.tableCache.Insert(0, timeZoneOffsetTable);
							break;
						}
						break;
					}
					else
					{
						timeZoneOffsetTable = null;
						i++;
					}
				}
			}
			return timeZoneOffsetTable;
		}

		private static void AddTableToCache(TimeZoneOffsets.TimeZoneOffsetTable newTable)
		{
			lock (TimeZoneOffsets.cacheMutex)
			{
				for (int i = 0; i < TimeZoneOffsets.tableCache.Count; i++)
				{
					TimeZoneOffsets.TimeZoneOffsetTable timeZoneOffsetTable = TimeZoneOffsets.tableCache[i];
					if (timeZoneOffsetTable.StartYear == newTable.StartYear && timeZoneOffsetTable.EndYear == newTable.EndYear)
					{
						return;
					}
				}
				if (TimeZoneOffsets.tableCache.Count >= 5)
				{
					TimeZoneOffsets.tableCache.RemoveRange(4, TimeZoneOffsets.tableCache.Count - 5 + 1);
				}
				TimeZoneOffsets.tableCache.Insert(0, newTable);
			}
		}

		private static TimeZoneOffsets.TimeZoneOffsetTable BuildCache(int startYear, int endYear, ExPerformanceCounter performanceCounter)
		{
			if (performanceCounter != null)
			{
				performanceCounter.Increment();
			}
			TimeZoneOffsets.TimeZoneOffsetTable timeZoneOffsetTable = new TimeZoneOffsets.TimeZoneOffsetTable();
			timeZoneOffsetTable.StartYear = startYear;
			timeZoneOffsetTable.EndYear = endYear;
			List<TimeZoneOffset> list = new List<TimeZoneOffset>();
			foreach (ExTimeZone exTimeZone in ExTimeZoneEnumerator.Instance)
			{
				TimeZoneOffset timeZoneOffset = new TimeZoneOffset();
				timeZoneOffset.TimeZoneId = exTimeZone.Id;
				List<TimeZoneRange> list2 = new List<TimeZoneRange>();
				ExDateTime t = new ExDateTime(ExTimeZone.UtcTimeZone, endYear + 1, 1, 1);
				ExDateTime exDateTime = new ExDateTime(ExTimeZone.UtcTimeZone, startYear, 1, 1);
				for (;;)
				{
					int num = (int)exTimeZone.ConvertDateTime(exDateTime).Bias.TotalMinutes;
					list2.Add(new TimeZoneRange
					{
						UtcTime = exDateTime,
						Offset = num
					});
					TimeSpan timeSpan = TimeSpan.Zero;
					TimeSpan timeSpan2 = TimeSpan.Zero;
					while (exDateTime + timeSpan < t && exTimeZone.ConvertDateTime(exDateTime + timeSpan2).Bias.TotalMinutes == (double)num)
					{
						timeSpan = timeSpan2;
						timeSpan2 = timeSpan2.Add(TimeSpan.FromDays(7.0));
					}
					if (exDateTime + timeSpan >= t)
					{
						break;
					}
					while ((timeSpan2 - timeSpan).TotalMilliseconds > 1.0)
					{
						TimeSpan timeSpan3 = timeSpan.Add(TimeSpan.FromMilliseconds((timeSpan2.TotalMilliseconds - timeSpan.TotalMilliseconds) / 2.0));
						if (exTimeZone.ConvertDateTime(exDateTime + timeSpan3).Bias.TotalMinutes == (double)num)
						{
							timeSpan = timeSpan3;
						}
						else
						{
							timeSpan2 = timeSpan3;
						}
					}
					exDateTime += timeSpan2;
				}
				timeZoneOffset.OffsetRanges = list2.ToArray();
				list.Add(timeZoneOffset);
			}
			timeZoneOffsetTable.TimeZoneOffsets = list.ToArray();
			return timeZoneOffsetTable;
		}

		private const int MaxCachedTables = 5;

		private static object cacheMutex = new object();

		private static List<TimeZoneOffsets.TimeZoneOffsetTable> tableCache = new List<TimeZoneOffsets.TimeZoneOffsetTable>();

		private class TimeZoneOffsetTable
		{
			public int StartYear { get; set; }

			public int EndYear { get; set; }

			public TimeZoneOffset[] TimeZoneOffsets { get; set; }
		}
	}
}
