using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Common.FBL
{
	internal static class FBLPerfCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (FBLPerfCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in FBLPerfCounters.AllCounters)
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

		public const string CategoryName = "MSExchange FBL";

		public static readonly ExPerformanceCounter NumberOfFblRequestsReceived = new ExPerformanceCounter("MSExchange FBL", "Number of FBL Requests Received", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfFblClassificationRequestsReceived = new ExPerformanceCounter("MSExchange FBL", "Number of FBL Classification Requests Received", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfFblSubscriptionRequestsReceived = new ExPerformanceCounter("MSExchange FBL", "Number of FBL Subscription Requests Received", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfFblOptInRequestsReceived = new ExPerformanceCounter("MSExchange FBL", "Number of FBL Opt-In Requests Received", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfFblOptOutRequestsReceived = new ExPerformanceCounter("MSExchange FBL", "Number of FBL Opt-Out Requests Received", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfFblRequestsSuccessfullyProcessed = new ExPerformanceCounter("MSExchange FBL", "Number of FBL Requests Successfully Processed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfFblClassificationRequestsSuccessfullyProcessed = new ExPerformanceCounter("MSExchange FBL", "Number of FBL Classification Requests Successfully Processed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfFblSubscriptionRequestsSuccessfullyProcessed = new ExPerformanceCounter("MSExchange FBL", "Number of FBL Subscription Requests Successfully Processed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfFblOptInRequestsSuccessfullyProcessed = new ExPerformanceCounter("MSExchange FBL", "Number of FBL Opt-In Requests Successfully Processed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfFblOptOutRequestsSuccessfullyProcessed = new ExPerformanceCounter("MSExchange FBL", "Number of FBL Opt-Out Requests Successfully Processed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfFblRequestsFailed = new ExPerformanceCounter("MSExchange FBL", "Number of FBL Requests Failed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfFblClassificationRequestsFailed = new ExPerformanceCounter("MSExchange FBL", "Number of FBL Classification Requests Failed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfFblSubscriptionRequestsFailed = new ExPerformanceCounter("MSExchange FBL", "Number of FBL Subscription Requests Failed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfFblOptInRequestsFailed = new ExPerformanceCounter("MSExchange FBL", "Number of FBL Opt-In Requests Failed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfFblOptOutRequestsFailed = new ExPerformanceCounter("MSExchange FBL", "Number of FBL Opt-Out Requests Failed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfSuccessfulMSERVReadRequests = new ExPerformanceCounter("MSExchange FBL", "Number of Successful MSERV Read Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfFailedMSERVReadRequests = new ExPerformanceCounter("MSExchange FBL", "Number of Failed MSERV Read Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfSuccessfulMSERVWriteRequests = new ExPerformanceCounter("MSExchange FBL", "Number of Successful MSERV Write Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfFailedMSERVWriteRequests = new ExPerformanceCounter("MSExchange FBL", "Number of Failed MSERV Write Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfXMRMessagesSuccessfullySent = new ExPerformanceCounter("MSExchange FBL", "Number of XMR Messages Successfully Sent", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfXMRMessagesFailedToSend = new ExPerformanceCounter("MSExchange FBL", "Number of XMR Messages Failed To Send", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			FBLPerfCounters.NumberOfFblRequestsReceived,
			FBLPerfCounters.NumberOfFblClassificationRequestsReceived,
			FBLPerfCounters.NumberOfFblSubscriptionRequestsReceived,
			FBLPerfCounters.NumberOfFblOptInRequestsReceived,
			FBLPerfCounters.NumberOfFblOptOutRequestsReceived,
			FBLPerfCounters.NumberOfFblRequestsSuccessfullyProcessed,
			FBLPerfCounters.NumberOfFblClassificationRequestsSuccessfullyProcessed,
			FBLPerfCounters.NumberOfFblSubscriptionRequestsSuccessfullyProcessed,
			FBLPerfCounters.NumberOfFblOptInRequestsSuccessfullyProcessed,
			FBLPerfCounters.NumberOfFblOptOutRequestsSuccessfullyProcessed,
			FBLPerfCounters.NumberOfFblRequestsFailed,
			FBLPerfCounters.NumberOfFblClassificationRequestsFailed,
			FBLPerfCounters.NumberOfFblSubscriptionRequestsFailed,
			FBLPerfCounters.NumberOfFblOptInRequestsFailed,
			FBLPerfCounters.NumberOfFblOptOutRequestsFailed,
			FBLPerfCounters.NumberOfSuccessfulMSERVReadRequests,
			FBLPerfCounters.NumberOfFailedMSERVReadRequests,
			FBLPerfCounters.NumberOfSuccessfulMSERVWriteRequests,
			FBLPerfCounters.NumberOfFailedMSERVWriteRequests,
			FBLPerfCounters.NumberOfXMRMessagesSuccessfullySent,
			FBLPerfCounters.NumberOfXMRMessagesFailedToSend
		};
	}
}
