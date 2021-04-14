using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal class MaintenanceResultIdGenerator : ResultIdGenerator<MaintenanceResult>
	{
		protected override ExPerformanceCounter Counter
		{
			get
			{
				return LocalDataAccessPerfCounters.LastMaintenanceResultId;
			}
		}
	}
}
