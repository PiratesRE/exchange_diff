using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LatencyReportingThresholdChecker
	{
		private LatencyReportingThresholdChecker()
		{
		}

		internal static LatencyReportingThresholdChecker Instance
		{
			get
			{
				return LatencyReportingThresholdChecker.singletonInstance;
			}
		}

		internal void ClearHistory()
		{
			foreach (LatencyDetectionLocation latencyDetectionLocation in this.thresholdCollection.Locations.Values)
			{
				latencyDetectionLocation.ClearBackLogs();
			}
		}

		internal bool ShouldCreateReport(LatencyDetectionContext currentContext, LoggingType loggingType, out LatencyDetectionContext trigger, out LatencyReportingThreshold thresholdToCheck, out ICollection<LatencyDetectionContext> dataToLog)
		{
			bool flag = false;
			dataToLog = null;
			thresholdToCheck = null;
			trigger = null;
			if (Thread.VolatileRead(ref this.creatingReport) == 0)
			{
				LatencyDetectionLocation location = currentContext.Location;
				BackLog backLog = location.GetBackLog(loggingType);
				thresholdToCheck = location.GetThreshold(loggingType);
				flag = backLog.AddAndQueryThreshold(currentContext);
				if (flag)
				{
					flag = (Interlocked.CompareExchange(ref this.creatingReport, 1, 0) == 0);
					if (flag)
					{
						try
						{
							flag = backLog.IsBeyondThreshold(out trigger);
							if (flag)
							{
								dataToLog = this.MoveBacklogDataToReport(loggingType);
							}
						}
						finally
						{
							Thread.VolatileWrite(ref this.creatingReport, 0);
						}
					}
				}
			}
			return flag;
		}

		private ICollection<LatencyDetectionContext> MoveBacklogDataToReport(LoggingType type)
		{
			IDictionary<string, LatencyDetectionLocation> locations = this.thresholdCollection.Locations;
			int count = locations.Count;
			List<LatencyDetectionContext> list = null;
			foreach (KeyValuePair<string, LatencyDetectionLocation> keyValuePair in locations)
			{
				BackLog backLog = keyValuePair.Value.GetBackLog(type);
				if (list == null)
				{
					list = new List<LatencyDetectionContext>(count * backLog.Count);
				}
				backLog.MoveToList(list);
			}
			return list;
		}

		private static readonly LatencyReportingThresholdChecker singletonInstance = new LatencyReportingThresholdChecker();

		private readonly LatencyReportingThresholdContainer thresholdCollection = LatencyReportingThresholdContainer.Instance;

		private int creatingReport;
	}
}
