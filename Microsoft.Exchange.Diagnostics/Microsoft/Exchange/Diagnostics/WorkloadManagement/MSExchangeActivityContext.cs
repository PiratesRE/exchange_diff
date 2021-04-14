using System;
using System.Xml.Linq;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement
{
	internal static class MSExchangeActivityContext
	{
		public static MSExchangeActivityContextInstance GetInstance(string instanceName)
		{
			return (MSExchangeActivityContextInstance)MSExchangeActivityContext.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MSExchangeActivityContext.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MSExchangeActivityContext.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MSExchangeActivityContext.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MSExchangeActivityContext.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MSExchangeActivityContext.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MSExchangeActivityContext.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MSExchangeActivityContextInstance(instanceName, (MSExchangeActivityContextInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MSExchangeActivityContextInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MSExchangeActivityContext.counters == null)
			{
				return;
			}
			MSExchangeActivityContext.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Activity Context Resources";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange Activity Context Resources", new CreateInstanceDelegate(MSExchangeActivityContext.CreateInstance));
	}
}
