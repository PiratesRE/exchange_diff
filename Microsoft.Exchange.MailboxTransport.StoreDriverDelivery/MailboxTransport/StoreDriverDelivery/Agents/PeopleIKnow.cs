using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal static class PeopleIKnow
	{
		public static PeopleIKnowInstance GetInstance(string instanceName)
		{
			return (PeopleIKnowInstance)PeopleIKnow.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			PeopleIKnow.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return PeopleIKnow.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return PeopleIKnow.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			PeopleIKnow.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			PeopleIKnow.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			PeopleIKnow.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new PeopleIKnowInstance(instanceName, (PeopleIKnowInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new PeopleIKnowInstance(instanceName);
		}

		public static PeopleIKnowInstance TotalInstance
		{
			get
			{
				return (PeopleIKnowInstance)PeopleIKnow.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (PeopleIKnow.counters == null)
			{
				return;
			}
			PeopleIKnow.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "People-I-Know Delivery Agent";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("People-I-Know Delivery Agent", new CreateInstanceDelegate(PeopleIKnow.CreateInstance), new CreateTotalInstanceDelegate(PeopleIKnow.CreateTotalInstance));
	}
}
