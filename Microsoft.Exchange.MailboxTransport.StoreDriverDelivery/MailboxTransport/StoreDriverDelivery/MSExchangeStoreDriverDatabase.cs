using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal static class MSExchangeStoreDriverDatabase
	{
		public static MSExchangeStoreDriverDatabaseInstance GetInstance(string instanceName)
		{
			return (MSExchangeStoreDriverDatabaseInstance)MSExchangeStoreDriverDatabase.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MSExchangeStoreDriverDatabase.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MSExchangeStoreDriverDatabase.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MSExchangeStoreDriverDatabase.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MSExchangeStoreDriverDatabase.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MSExchangeStoreDriverDatabase.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MSExchangeStoreDriverDatabase.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MSExchangeStoreDriverDatabaseInstance(instanceName, (MSExchangeStoreDriverDatabaseInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MSExchangeStoreDriverDatabaseInstance(instanceName);
		}

		public static MSExchangeStoreDriverDatabaseInstance TotalInstance
		{
			get
			{
				return (MSExchangeStoreDriverDatabaseInstance)MSExchangeStoreDriverDatabase.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MSExchangeStoreDriverDatabase.counters == null)
			{
				return;
			}
			MSExchangeStoreDriverDatabase.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Delivery Store Driver Database";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchange Delivery Store Driver Database", new CreateInstanceDelegate(MSExchangeStoreDriverDatabase.CreateInstance), new CreateTotalInstanceDelegate(MSExchangeStoreDriverDatabase.CreateTotalInstance));
	}
}
