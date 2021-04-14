using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ResourceHealth
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class MSExchangeDatabasePinger
	{
		public static MSExchangeDatabasePingerInstance GetInstance(string instanceName)
		{
			return (MSExchangeDatabasePingerInstance)MSExchangeDatabasePinger.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MSExchangeDatabasePinger.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MSExchangeDatabasePinger.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MSExchangeDatabasePinger.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MSExchangeDatabasePinger.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MSExchangeDatabasePinger.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MSExchangeDatabasePinger.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MSExchangeDatabasePingerInstance(instanceName, (MSExchangeDatabasePingerInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MSExchangeDatabasePingerInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MSExchangeDatabasePinger.counters == null)
			{
				return;
			}
			MSExchangeDatabasePinger.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Database Pinger";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange Database Pinger", new CreateInstanceDelegate(MSExchangeDatabasePinger.CreateInstance));
	}
}
