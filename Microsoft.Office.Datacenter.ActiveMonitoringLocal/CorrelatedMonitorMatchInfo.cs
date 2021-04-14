using System;
using System.Collections.Generic;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class CorrelatedMonitorMatchInfo
	{
		internal CorrelatedMonitorMatchInfo(CorrelatedMonitorInfo monitorInfo)
		{
			this.CorrelatedMonitorInfo = monitorInfo;
			this.DetailedMonitorResultMap = new Dictionary<int, CorrelatedMonitorMatchInfo.MonitorResultDetailed>();
			this.MatchingMonitorResultsDetailed = new List<CorrelatedMonitorMatchInfo.MonitorResultDetailed>();
		}

		internal CorrelatedMonitorInfo CorrelatedMonitorInfo { get; set; }

		internal Dictionary<int, CorrelatedMonitorMatchInfo.MonitorResultDetailed> DetailedMonitorResultMap { get; set; }

		internal List<CorrelatedMonitorMatchInfo.MonitorResultDetailed> MatchingMonitorResultsDetailed { get; set; }

		public class MonitorResultDetailed
		{
			internal MonitorResultDetailed(MonitorDefinition definition)
			{
				this.Definition = definition;
			}

			internal MonitorDefinition Definition { get; set; }

			internal MonitorResult Result { get; set; }
		}
	}
}
