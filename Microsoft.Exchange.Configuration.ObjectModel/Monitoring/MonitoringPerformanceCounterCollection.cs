using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	public class MonitoringPerformanceCounterCollection : List<MonitoringPerformanceCounter>
	{
	}
}
