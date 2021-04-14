using System;
using System.Diagnostics;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class SystemInstrumentationStrategy : InstrumentationBaseStrategy
	{
		protected override void InternalInitialize()
		{
			try
			{
				base.PerfCounters.Add(new PerformanceCounter("Processor", "% Processor Time", "_Total"));
				base.PerfCounters.Add(new PerformanceCounter("Process", "Page File Bytes", "_Total"));
				base.PerfCounters.Add(new PerformanceCounter("Memory", "Available Mbytes", null));
				base.PerfCounters.Add(new PerformanceCounter("Memory", "Page Faults/sec", null));
				base.PerfCounters.Add(new PerformanceCounter("Memory", "Page Reads/sec", null));
				base.PerfCounters.Add(new PerformanceCounter("Memory", "Pages Input/sec", null));
				PerformanceCounterCategory performanceCounterCategory = new PerformanceCounterCategory("LogicalDisk");
				foreach (string text in performanceCounterCategory.GetInstanceNames())
				{
					if (!string.Equals(text, "_Total", StringComparison.OrdinalIgnoreCase))
					{
						base.PerfCounters.Add(new PerformanceCounter(performanceCounterCategory.CategoryName, "% Disk Time", text));
						base.PerfCounters.Add(new PerformanceCounter(performanceCounterCategory.CategoryName, "Current Disk Queue Length", text));
					}
				}
			}
			catch (Exception ex)
			{
				InstrumentationBaseStrategy.TraceDebug("SystemInstrumentationStrategey: Failed to setup performance counters. error={0}", new object[]
				{
					ex
				});
			}
		}
	}
}
