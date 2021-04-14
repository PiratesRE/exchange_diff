using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class MsExchangeTransportSyncManagerByDatabasePerf
	{
		public static MsExchangeTransportSyncManagerByDatabasePerfInstance GetInstance(string instanceName)
		{
			return (MsExchangeTransportSyncManagerByDatabasePerfInstance)MsExchangeTransportSyncManagerByDatabasePerf.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MsExchangeTransportSyncManagerByDatabasePerf.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MsExchangeTransportSyncManagerByDatabasePerf.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MsExchangeTransportSyncManagerByDatabasePerf.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MsExchangeTransportSyncManagerByDatabasePerf.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MsExchangeTransportSyncManagerByDatabasePerf.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MsExchangeTransportSyncManagerByDatabasePerf.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MsExchangeTransportSyncManagerByDatabasePerfInstance(instanceName, (MsExchangeTransportSyncManagerByDatabasePerfInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MsExchangeTransportSyncManagerByDatabasePerfInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MsExchangeTransportSyncManagerByDatabasePerf.counters == null)
			{
				return;
			}
			MsExchangeTransportSyncManagerByDatabasePerf.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Transport Sync Manager By Database";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange Transport Sync Manager By Database", new CreateInstanceDelegate(MsExchangeTransportSyncManagerByDatabasePerf.CreateInstance));
	}
}
