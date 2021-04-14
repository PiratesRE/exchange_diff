using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal class ProbeResultIdGenerator : ResultIdGenerator<ProbeResult>
	{
		protected override ExPerformanceCounter Counter
		{
			get
			{
				return LocalDataAccessPerfCounters.LastProbeResultId;
			}
		}
	}
}
