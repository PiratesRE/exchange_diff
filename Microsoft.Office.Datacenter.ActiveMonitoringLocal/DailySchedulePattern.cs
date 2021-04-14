using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal class DailySchedulePattern
	{
		internal DailySchedulePattern(string dailySchedulePattern)
		{
			this.ParseDailySchedulePattern(dailySchedulePattern);
		}

		internal TimeZoneInfo TimeZoneInfo { get; private set; }

		internal HashSet<DayOfWeek> ScheduledDays { get; private set; }

		internal DateTime StartTime { get; private set; }

		internal DateTime EndTime { get; private set; }

		private void ParseDailySchedulePattern(string dailySchedulePattern)
		{
			try
			{
				dailySchedulePattern = ((!string.IsNullOrWhiteSpace(dailySchedulePattern)) ? dailySchedulePattern : "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59");
				string[] array = dailySchedulePattern.Split(new char[]
				{
					'/'
				});
				if (array.Length != 4)
				{
					throw new FormatException("Must have 4 components.");
				}
				this.TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(array[0]);
				this.ScheduledDays = new HashSet<DayOfWeek>();
				foreach (string value in array[1].Split(new char[]
				{
					',',
					';'
				}))
				{
					DayOfWeek item;
					if (Enum.TryParse<DayOfWeek>(value, true, out item) && !this.ScheduledDays.Contains(item))
					{
						this.ScheduledDays.Add(item);
					}
				}
				if (this.ScheduledDays.Count == 0)
				{
					throw new FormatException("Found no valid days.");
				}
				this.StartTime = DateTime.ParseExact(array[2], "t", CultureInfo.InvariantCulture, DateTimeStyles.None);
				this.EndTime = DateTime.ParseExact(array[3], "t", CultureInfo.InvariantCulture, DateTimeStyles.None);
				if (this.EndTime <= this.StartTime)
				{
					this.EndTime = this.StartTime.Date.AddDays(1.0).AddMilliseconds(-1.0);
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError<string, string>(ExTraceGlobals.CommonComponentsTracer, DailySchedulePattern.traceContext, "Invalid DailySchedulePattern value '{0}': {1}", dailySchedulePattern, ex.ToString(), null, "ParseDailySchedulePattern", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Responders\\DailySchedulePattern.cs", 119);
				if (!(dailySchedulePattern != "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59"))
				{
					throw new Exception("No valid DailySchedulePattern value is available.");
				}
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, DailySchedulePattern.traceContext, "Default value '{0}' will be used.", "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", null, "ParseDailySchedulePattern", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Responders\\DailySchedulePattern.cs", 123);
				this.ParseDailySchedulePattern("Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59");
			}
		}

		internal const string DefaultDailySchedulePattern = "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59";

		private static TracingContext traceContext = TracingContext.Default;
	}
}
