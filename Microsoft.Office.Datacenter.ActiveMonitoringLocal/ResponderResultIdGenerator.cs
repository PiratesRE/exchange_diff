using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal class ResponderResultIdGenerator : ResultIdGenerator<ResponderResult>
	{
		protected override ExPerformanceCounter Counter
		{
			get
			{
				return LocalDataAccessPerfCounters.LastResponderResultId;
			}
		}
	}
}
