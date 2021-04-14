using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal class MonitorResultIdGenerator : ResultIdGenerator<MonitorResult>
	{
		protected override ExPerformanceCounter Counter
		{
			get
			{
				return LocalDataAccessPerfCounters.LastMonitorResultId;
			}
		}
	}
}
