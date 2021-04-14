using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class MsExchangeTransportSyncManagerByProtocolPerf
	{
		public static MsExchangeTransportSyncManagerByProtocolPerfInstance GetInstance(string instanceName)
		{
			return (MsExchangeTransportSyncManagerByProtocolPerfInstance)MsExchangeTransportSyncManagerByProtocolPerf.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MsExchangeTransportSyncManagerByProtocolPerf.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MsExchangeTransportSyncManagerByProtocolPerf.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MsExchangeTransportSyncManagerByProtocolPerf.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MsExchangeTransportSyncManagerByProtocolPerf.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MsExchangeTransportSyncManagerByProtocolPerf.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MsExchangeTransportSyncManagerByProtocolPerf.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MsExchangeTransportSyncManagerByProtocolPerfInstance(instanceName, (MsExchangeTransportSyncManagerByProtocolPerfInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MsExchangeTransportSyncManagerByProtocolPerfInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MsExchangeTransportSyncManagerByProtocolPerf.counters == null)
			{
				return;
			}
			MsExchangeTransportSyncManagerByProtocolPerf.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Transport Sync Manager By Protocol";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange Transport Sync Manager By Protocol", new CreateInstanceDelegate(MsExchangeTransportSyncManagerByProtocolPerf.CreateInstance));
	}
}
