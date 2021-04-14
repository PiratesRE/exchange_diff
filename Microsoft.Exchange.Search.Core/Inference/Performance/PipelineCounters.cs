using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Inference.Performance
{
	internal static class PipelineCounters
	{
		public static PipelineCountersInstance GetInstance(string instanceName)
		{
			return (PipelineCountersInstance)PipelineCounters.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			PipelineCounters.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return PipelineCounters.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return PipelineCounters.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			PipelineCounters.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			PipelineCounters.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			PipelineCounters.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new PipelineCountersInstance(instanceName, (PipelineCountersInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new PipelineCountersInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (PipelineCounters.counters == null)
			{
				return;
			}
			PipelineCounters.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeInference Pipeline";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchangeInference Pipeline", new CreateInstanceDelegate(PipelineCounters.CreateInstance));
	}
}
