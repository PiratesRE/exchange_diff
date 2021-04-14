using System;

namespace Microsoft.Exchange.Diagnostics
{
	public delegate PerformanceCounterInstance CreateInstanceDelegate(string instanceName, PerformanceCounterInstance totalInstance);
}
