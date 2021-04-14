using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal static class MSExchangeStoreDriverDeliveryAgent
	{
		public static MSExchangeStoreDriverDeliveryAgentInstance GetInstance(string instanceName)
		{
			return (MSExchangeStoreDriverDeliveryAgentInstance)MSExchangeStoreDriverDeliveryAgent.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MSExchangeStoreDriverDeliveryAgent.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MSExchangeStoreDriverDeliveryAgent.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MSExchangeStoreDriverDeliveryAgent.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MSExchangeStoreDriverDeliveryAgent.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MSExchangeStoreDriverDeliveryAgent.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MSExchangeStoreDriverDeliveryAgent.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MSExchangeStoreDriverDeliveryAgentInstance(instanceName, (MSExchangeStoreDriverDeliveryAgentInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MSExchangeStoreDriverDeliveryAgentInstance(instanceName);
		}

		public static MSExchangeStoreDriverDeliveryAgentInstance TotalInstance
		{
			get
			{
				return (MSExchangeStoreDriverDeliveryAgentInstance)MSExchangeStoreDriverDeliveryAgent.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MSExchangeStoreDriverDeliveryAgent.counters == null)
			{
				return;
			}
			MSExchangeStoreDriverDeliveryAgent.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Delivery Store Driver Agents";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchange Delivery Store Driver Agents", new CreateInstanceDelegate(MSExchangeStoreDriverDeliveryAgent.CreateInstance), new CreateTotalInstanceDelegate(MSExchangeStoreDriverDeliveryAgent.CreateTotalInstance));
	}
}
