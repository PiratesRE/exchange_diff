using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class ActiveManagerClientPerfmon
	{
		public static ActiveManagerClientPerfmonInstance GetInstance(string instanceName)
		{
			return (ActiveManagerClientPerfmonInstance)ActiveManagerClientPerfmon.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			ActiveManagerClientPerfmon.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return ActiveManagerClientPerfmon.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return ActiveManagerClientPerfmon.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			ActiveManagerClientPerfmon.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			ActiveManagerClientPerfmon.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			ActiveManagerClientPerfmon.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new ActiveManagerClientPerfmonInstance(instanceName, (ActiveManagerClientPerfmonInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new ActiveManagerClientPerfmonInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (ActiveManagerClientPerfmon.counters == null)
			{
				return;
			}
			ActiveManagerClientPerfmon.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Active Manager Client";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange Active Manager Client", new CreateInstanceDelegate(ActiveManagerClientPerfmon.CreateInstance));
	}
}
