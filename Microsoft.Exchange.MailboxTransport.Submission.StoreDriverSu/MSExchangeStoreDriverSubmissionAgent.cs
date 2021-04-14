using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal static class MSExchangeStoreDriverSubmissionAgent
	{
		public static MSExchangeStoreDriverSubmissionAgentInstance GetInstance(string instanceName)
		{
			return (MSExchangeStoreDriverSubmissionAgentInstance)MSExchangeStoreDriverSubmissionAgent.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MSExchangeStoreDriverSubmissionAgent.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MSExchangeStoreDriverSubmissionAgent.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MSExchangeStoreDriverSubmissionAgent.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MSExchangeStoreDriverSubmissionAgent.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MSExchangeStoreDriverSubmissionAgent.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MSExchangeStoreDriverSubmissionAgent.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MSExchangeStoreDriverSubmissionAgentInstance(instanceName, (MSExchangeStoreDriverSubmissionAgentInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MSExchangeStoreDriverSubmissionAgentInstance(instanceName);
		}

		public static MSExchangeStoreDriverSubmissionAgentInstance TotalInstance
		{
			get
			{
				return (MSExchangeStoreDriverSubmissionAgentInstance)MSExchangeStoreDriverSubmissionAgent.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MSExchangeStoreDriverSubmissionAgent.counters == null)
			{
				return;
			}
			MSExchangeStoreDriverSubmissionAgent.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Submission Store Driver Agents";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchange Submission Store Driver Agents", new CreateInstanceDelegate(MSExchangeStoreDriverSubmissionAgent.CreateInstance), new CreateTotalInstanceDelegate(MSExchangeStoreDriverSubmissionAgent.CreateTotalInstance));
	}
}
