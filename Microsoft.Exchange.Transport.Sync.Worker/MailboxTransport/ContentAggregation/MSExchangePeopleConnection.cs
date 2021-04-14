using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class MSExchangePeopleConnection
	{
		public static MSExchangePeopleConnectionInstance GetInstance(string instanceName)
		{
			return (MSExchangePeopleConnectionInstance)MSExchangePeopleConnection.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MSExchangePeopleConnection.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MSExchangePeopleConnection.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MSExchangePeopleConnection.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MSExchangePeopleConnection.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MSExchangePeopleConnection.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MSExchangePeopleConnection.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MSExchangePeopleConnectionInstance(instanceName, (MSExchangePeopleConnectionInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MSExchangePeopleConnectionInstance(instanceName);
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MSExchangePeopleConnection.counters == null)
			{
				return;
			}
			MSExchangePeopleConnection.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Transport Sync - Contacts";

		private static readonly PerformanceCounterMultipleInstance counters = new PerformanceCounterMultipleInstance("MSExchange Transport Sync - Contacts", new CreateInstanceDelegate(MSExchangePeopleConnection.CreateInstance));
	}
}
