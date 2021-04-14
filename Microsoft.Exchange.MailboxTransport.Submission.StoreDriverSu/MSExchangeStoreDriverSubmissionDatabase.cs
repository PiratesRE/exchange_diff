using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal static class MSExchangeStoreDriverSubmissionDatabase
	{
		public static MSExchangeStoreDriverSubmissionDatabaseInstance GetInstance(string instanceName)
		{
			return (MSExchangeStoreDriverSubmissionDatabaseInstance)MSExchangeStoreDriverSubmissionDatabase.counters.GetInstance(instanceName);
		}

		public static void CloseInstance(string instanceName)
		{
			MSExchangeStoreDriverSubmissionDatabase.counters.CloseInstance(instanceName);
		}

		public static bool InstanceExists(string instanceName)
		{
			return MSExchangeStoreDriverSubmissionDatabase.counters.InstanceExists(instanceName);
		}

		public static string[] GetInstanceNames()
		{
			return MSExchangeStoreDriverSubmissionDatabase.counters.GetInstanceNames();
		}

		public static void RemoveInstance(string instanceName)
		{
			MSExchangeStoreDriverSubmissionDatabase.counters.RemoveInstance(instanceName);
		}

		public static void ResetInstance(string instanceName)
		{
			MSExchangeStoreDriverSubmissionDatabase.counters.ResetInstance(instanceName);
		}

		public static void RemoveAllInstances()
		{
			MSExchangeStoreDriverSubmissionDatabase.counters.RemoveAllInstances();
		}

		private static PerformanceCounterInstance CreateInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			return new MSExchangeStoreDriverSubmissionDatabaseInstance(instanceName, (MSExchangeStoreDriverSubmissionDatabaseInstance)totalInstance);
		}

		private static PerformanceCounterInstance CreateTotalInstance(string instanceName)
		{
			return new MSExchangeStoreDriverSubmissionDatabaseInstance(instanceName);
		}

		public static MSExchangeStoreDriverSubmissionDatabaseInstance TotalInstance
		{
			get
			{
				return (MSExchangeStoreDriverSubmissionDatabaseInstance)MSExchangeStoreDriverSubmissionDatabase.counters.TotalInstance;
			}
		}

		public static void GetPerfCounterInfo(XElement element)
		{
			if (MSExchangeStoreDriverSubmissionDatabase.counters == null)
			{
				return;
			}
			MSExchangeStoreDriverSubmissionDatabase.counters.GetPerfCounterDiagnosticsInfo(element);
		}

		public const string CategoryName = "MSExchange Submission Store Driver Database";

		private static readonly PerformanceCounterMultipleInstanceWithAutoUpdateTotal counters = new PerformanceCounterMultipleInstanceWithAutoUpdateTotal("MSExchange Submission Store Driver Database", new CreateInstanceDelegate(MSExchangeStoreDriverSubmissionDatabase.CreateInstance), new CreateTotalInstanceDelegate(MSExchangeStoreDriverSubmissionDatabase.CreateTotalInstance));
	}
}
