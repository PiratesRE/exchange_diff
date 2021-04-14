using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal static class SharedMailboxSentItemsDeliveryAgent
	{
		public static SharedMailboxSentItemsDeliveryAgentInstance GetInstance(string instanceName)
		{
			return (SharedMailboxSentItemsDeliveryAgentInstance)SharedMailboxSentItemsDeliveryAgent.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			SharedMailboxSentItemsDeliveryAgent.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return SharedMailboxSentItemsDeliveryAgent.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return SharedMailboxSentItemsDeliveryAgent.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			SharedMailboxSentItemsDeliveryAgent.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			SharedMailboxSentItemsDeliveryAgent.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			SharedMailboxSentItemsDeliveryAgent.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new SharedMailboxSentItemsDeliveryAgentInstance(instanceName, (SharedMailboxSentItemsDeliveryAgentInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new SharedMailboxSentItemsDeliveryAgentInstance(instanceName);
		}

		public static SharedMailboxSentItemsDeliveryAgentInstance TotalInstance
		{
			get
			{
				return (SharedMailboxSentItemsDeliveryAgentInstance)SharedMailboxSentItemsDeliveryAgent.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (SharedMailboxSentItemsDeliveryAgent.counters == null)
			{
				return;
			}
			SharedMailboxSentItemsDeliveryAgent.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Shared Mailbox Sent Items Agent";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchange Shared Mailbox Sent Items Agent", new CreateInstanceDelegate(SharedMailboxSentItemsDeliveryAgent.CreateInstance), new CreateTotalInstanceDelegate(SharedMailboxSentItemsDeliveryAgent.CreateTotalInstance));
	}
}
