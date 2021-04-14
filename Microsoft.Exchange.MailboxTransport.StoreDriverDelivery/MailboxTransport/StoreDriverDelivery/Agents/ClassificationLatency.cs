using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal static class ClassificationLatency
	{
		public static ClassificationLatencyInstance GetInstance(string instanceName)
		{
			return (ClassificationLatencyInstance)ClassificationLatency.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			ClassificationLatency.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return ClassificationLatency.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return ClassificationLatency.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			ClassificationLatency.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			ClassificationLatency.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			ClassificationLatency.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new ClassificationLatencyInstance(instanceName, (ClassificationLatencyInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new ClassificationLatencyInstance(instanceName);
		}

		public static ClassificationLatencyInstance TotalInstance
		{
			get
			{
				return (ClassificationLatencyInstance)ClassificationLatency.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (ClassificationLatency.counters == null)
			{
				return;
			}
			ClassificationLatency.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchangeInference Classification Latency";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchangeInference Classification Latency", new CreateInstanceDelegate(ClassificationLatency.CreateInstance), new CreateTotalInstanceDelegate(ClassificationLatency.CreateTotalInstance));
	}
}
