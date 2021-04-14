using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.LogUploader
{
	internal static class PerfCountersInstanceCache
	{
		static PerfCountersInstanceCache()
		{
			PerfCountersInstanceCache.instances.Add("_Total", PerfCountersInstanceCache.TotalInstance);
			PerfCountersInstanceCache.GetPerfCountersInstance = ((string instanceName) => new LogUploaderDefaultCommonPerfCountersInstance(instanceName, PerfCountersInstanceCache.TotalInstance));
		}

		public static Func<string, ILogUploaderPerformanceCounters> GetPerfCountersInstance { get; set; }

		public static ILogUploaderPerformanceCounters GetInstance(string instanceName)
		{
			ILogUploaderPerformanceCounters logUploaderPerformanceCounters;
			if (!PerfCountersInstanceCache.instances.TryGetValue(instanceName, out logUploaderPerformanceCounters))
			{
				lock (PerfCountersInstanceCache.InstancesMutex)
				{
					if (!PerfCountersInstanceCache.instances.TryGetValue(instanceName, out logUploaderPerformanceCounters))
					{
						Tools.DebugAssert(PerfCountersInstanceCache.GetPerfCountersInstance != null, "Performance counters factory method expected.");
						logUploaderPerformanceCounters = PerfCountersInstanceCache.GetPerfCountersInstance(instanceName);
						PerfCountersInstanceCache.instances = new Dictionary<string, ILogUploaderPerformanceCounters>(PerfCountersInstanceCache.instances, StringComparer.OrdinalIgnoreCase)
						{
							{
								instanceName,
								logUploaderPerformanceCounters
							}
						};
					}
				}
			}
			return logUploaderPerformanceCounters;
		}

		private const string TotalInstanceName = "_Total";

		private static readonly object InstancesMutex = new object();

		private static readonly LogUploaderDefaultCommonPerfCountersInstance TotalInstance = new LogUploaderDefaultCommonPerfCountersInstance("_Total", null);

		private static volatile Dictionary<string, ILogUploaderPerformanceCounters> instances = new Dictionary<string, ILogUploaderPerformanceCounters>(StringComparer.OrdinalIgnoreCase);
	}
}
