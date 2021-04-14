using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class MsExchangeTransportSyncManagerBySLAPerf
	{
		public static MsExchangeTransportSyncManagerBySLAPerfInstance GetInstance(string instanceName)
		{
			return (MsExchangeTransportSyncManagerBySLAPerfInstance)MsExchangeTransportSyncManagerBySLAPerf.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MsExchangeTransportSyncManagerBySLAPerf.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MsExchangeTransportSyncManagerBySLAPerf.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MsExchangeTransportSyncManagerBySLAPerf.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MsExchangeTransportSyncManagerBySLAPerf.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MsExchangeTransportSyncManagerBySLAPerf.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MsExchangeTransportSyncManagerBySLAPerf.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MsExchangeTransportSyncManagerBySLAPerfInstance(instanceName, (MsExchangeTransportSyncManagerBySLAPerfInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MsExchangeTransportSyncManagerBySLAPerfInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MsExchangeTransportSyncManagerBySLAPerf.counters == null)
			{
				return;
			}
			MsExchangeTransportSyncManagerBySLAPerf.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Transport Sync Manager By SLA";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange Transport Sync Manager By SLA", new CreateInstanceDelegate(MsExchangeTransportSyncManagerBySLAPerf.CreateInstance));
	}
}
