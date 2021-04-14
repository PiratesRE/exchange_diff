using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal static class OfficeGraph
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (OfficeGraph.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in OfficeGraph.AllCounters)
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

		public const string CategoryName = "Office Graph Delivery Agent";

		public static readonly ExPerformanceCounter ItemsSeen = new ExPerformanceCounter("Office Graph Delivery Agent", "Items - Seen", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ItemsFilteredTotal = new ExPerformanceCounter("Office Graph Delivery Agent", "Items - Filtered (Total)", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FilterCriteriaMessage = new ExPerformanceCounter("Office Graph Delivery Agent", "Filter Criteria - Message", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FilterCriteriaHasAttachment = new ExPerformanceCounter("Office Graph Delivery Agent", "Filter Criteria - Has Attachment", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FilterCriteriaInterestingAttachment = new ExPerformanceCounter("Office Graph Delivery Agent", "Filter Criteria - Interesting Attachment", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FilterCriteriaHasSharePointUrl = new ExPerformanceCounter("Office Graph Delivery Agent", "Filter Criteria - Has SharePoint URL", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FilterCriteriaIsFromFavoriteSender = new ExPerformanceCounter("Office Graph Delivery Agent", "Filter Criteria - Is From Favorite Sender", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SignalPersisted = new ExPerformanceCounter("Office Graph Delivery Agent", "Persisted Signals - Total", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalExceptions = new ExPerformanceCounter("Office Graph Delivery Agent", "Exceptions - Total", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter LastSharePointUrlRetrievalTime = new ExPerformanceCounter("Office Graph Delivery Agent", "Last SharePoint Url Retrieval time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageSharePointUrlRetrievalTime = new ExPerformanceCounter("Office Graph Delivery Agent", "Average SharePoint Url Retrieval time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageSharePointUrlRetrievalTimeBase = new ExPerformanceCounter("Office Graph Delivery Agent", "Average SharePoint Url Retrieval time Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter LastSignalCreationTime = new ExPerformanceCounter("Office Graph Delivery Agent", "Last Signal Creation Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageSignalCreationTime = new ExPerformanceCounter("Office Graph Delivery Agent", "Average Signal Creation Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageSignalCreationTimeBase = new ExPerformanceCounter("Office Graph Delivery Agent", "Average Signal Creation Time Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter LastSignalPersistingTime = new ExPerformanceCounter("Office Graph Delivery Agent", "Last Signal Persisting Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageSignalPersistingTime = new ExPerformanceCounter("Office Graph Delivery Agent", "Average Signal Persisting Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageSignalPersistingTimeBase = new ExPerformanceCounter("Office Graph Delivery Agent", "Average Signal Persisting Time Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			OfficeGraph.ItemsSeen,
			OfficeGraph.ItemsFilteredTotal,
			OfficeGraph.FilterCriteriaMessage,
			OfficeGraph.FilterCriteriaHasAttachment,
			OfficeGraph.FilterCriteriaInterestingAttachment,
			OfficeGraph.FilterCriteriaHasSharePointUrl,
			OfficeGraph.FilterCriteriaIsFromFavoriteSender,
			OfficeGraph.SignalPersisted,
			OfficeGraph.TotalExceptions,
			OfficeGraph.LastSharePointUrlRetrievalTime,
			OfficeGraph.AverageSharePointUrlRetrievalTime,
			OfficeGraph.AverageSharePointUrlRetrievalTimeBase,
			OfficeGraph.LastSignalCreationTime,
			OfficeGraph.AverageSignalCreationTime,
			OfficeGraph.AverageSignalCreationTimeBase,
			OfficeGraph.LastSignalPersistingTime,
			OfficeGraph.AverageSignalPersistingTime,
			OfficeGraph.AverageSignalPersistingTimeBase
		};
	}
}
