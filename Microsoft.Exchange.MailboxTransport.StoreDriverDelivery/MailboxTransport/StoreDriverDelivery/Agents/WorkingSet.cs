using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal static class WorkingSet
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (WorkingSet.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in WorkingSet.AllCounters)
			{
				try
				{
					element.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					element.Add(content);
				}
			}
		}

		public const string CategoryName = "Working Set Delivery Agent";

		public static readonly ExPerformanceCounter AverageStopWatchTime = new ExPerformanceCounter("Working Set Delivery Agent", "Average Time to Process Message", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageStopWatchTimeBase = new ExPerformanceCounter("Working Set Delivery Agent", "Base for Average Time to Process Message", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageCpuTime = new ExPerformanceCounter("Working Set Delivery Agent", "Average CPU Time to Process Message", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageCpuTimeBase = new ExPerformanceCounter("Working Set Delivery Agent", "Base for Average CPU Time to Process Message", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageStoreRPCs = new ExPerformanceCounter("Working Set Delivery Agent", "Average Store RPCs", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageStoreRPCsBase = new ExPerformanceCounter("Working Set Delivery Agent", "Base for Average Store RPCs", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ProcessingAccepted = new ExPerformanceCounter("Working Set Delivery Agent", "Signal Mails Received - Agent Enabled", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ProcessingRejected = new ExPerformanceCounter("Working Set Delivery Agent", "Signal Mails Rejected - Agent Disabled", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ProcessingSuccess = new ExPerformanceCounter("Working Set Delivery Agent", "Processing - Success", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ProcessingFailed = new ExPerformanceCounter("Working Set Delivery Agent", "Processing - Failed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AddItem = new ExPerformanceCounter("Working Set Delivery Agent", "Add Item - Total", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AddExchangeItem = new ExPerformanceCounter("Working Set Delivery Agent", "Add Item - Exchange", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ItemsNotSupported = new ExPerformanceCounter("Working Set Delivery Agent", "Add Item - Not Supported", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PartitionsCreated = new ExPerformanceCounter("Working Set Delivery Agent", "Partitions - Created", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PartitionsDeleted = new ExPerformanceCounter("Working Set Delivery Agent", "Partitions - Deleted", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter LastSignalProcessingTime = new ExPerformanceCounter("Working Set Delivery Agent", "Last Signal Processing Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageSignalProcessingTime = new ExPerformanceCounter("Working Set Delivery Agent", "Average Signal Processing Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageSignalProcessingTimeBase = new ExPerformanceCounter("Working Set Delivery Agent", "Average Signal Processing Time Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			WorkingSet.AverageStopWatchTime,
			WorkingSet.AverageStopWatchTimeBase,
			WorkingSet.AverageCpuTime,
			WorkingSet.AverageCpuTimeBase,
			WorkingSet.AverageStoreRPCs,
			WorkingSet.AverageStoreRPCsBase,
			WorkingSet.ProcessingAccepted,
			WorkingSet.ProcessingRejected,
			WorkingSet.ProcessingSuccess,
			WorkingSet.ProcessingFailed,
			WorkingSet.AddItem,
			WorkingSet.AddExchangeItem,
			WorkingSet.ItemsNotSupported,
			WorkingSet.PartitionsCreated,
			WorkingSet.PartitionsDeleted,
			WorkingSet.LastSignalProcessingTime,
			WorkingSet.AverageSignalProcessingTime,
			WorkingSet.AverageSignalProcessingTimeBase
		};
	}
}
