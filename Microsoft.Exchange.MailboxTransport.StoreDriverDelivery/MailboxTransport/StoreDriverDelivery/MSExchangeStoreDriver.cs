using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal static class MSExchangeStoreDriver
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (MSExchangeStoreDriver.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in MSExchangeStoreDriver.AllCounters)
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

		public const string CategoryName = "MSExchange Delivery Store Driver";

		private static readonly ExPerformanceCounter LocalDeliveryCallsPerSecond = new ExPerformanceCounter("MSExchange Delivery Store Driver", "Inbound: LocalDeliveryCallsPerSecond", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter LocalDeliveryCalls = new ExPerformanceCounter("MSExchange Delivery Store Driver", "Inbound: LocalDeliveryCalls", string.Empty, null, new ExPerformanceCounter[]
		{
			MSExchangeStoreDriver.LocalDeliveryCallsPerSecond
		});

		private static readonly ExPerformanceCounter MessageDeliveryAttemptsPerSecond = new ExPerformanceCounter("MSExchange Delivery Store Driver", "Inbound: MessageDeliveryAttemptsPerSecond", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MessageDeliveryAttempts = new ExPerformanceCounter("MSExchange Delivery Store Driver", "Inbound: MessageDeliveryAttempts", string.Empty, null, new ExPerformanceCounter[]
		{
			MSExchangeStoreDriver.MessageDeliveryAttemptsPerSecond
		});

		private static readonly ExPerformanceCounter RecipientsDeliveredPerSecond = new ExPerformanceCounter("MSExchange Delivery Store Driver", "Inbound: Recipients Delivered Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RecipientsDelivered = new ExPerformanceCounter("MSExchange Delivery Store Driver", "Inbound: Recipients Delivered", string.Empty, null, new ExPerformanceCounter[]
		{
			MSExchangeStoreDriver.RecipientsDeliveredPerSecond
		});

		public static readonly ExPerformanceCounter RecipientLevelPercentFailedDeliveries = new ExPerformanceCounter("MSExchange Delivery Store Driver", "Percent of Permanent Failed Deliveries within the last 5 minutes", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RecipientLevelPercentTemporaryFailedDeliveries = new ExPerformanceCounter("MSExchange Delivery Store Driver", "Percent of Temporary Failed Deliveries within the last 5 minutes", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter BytesDelivered = new ExPerformanceCounter("MSExchange Delivery Store Driver", "Inbound: Bytes Delivered", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CurrentDeliveryThreads = new ExPerformanceCounter("MSExchange Delivery Store Driver", "Inbound: Number of Delivering Threads", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DeliveryRetry = new ExPerformanceCounter("MSExchange Delivery Store Driver", "Inbound: Retried Recipients", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DeliveryReroute = new ExPerformanceCounter("MSExchange Delivery Store Driver", "Inbound: Rerouted Recipients", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DuplicateDelivery = new ExPerformanceCounter("MSExchange Delivery Store Driver", "Inbound: Duplicate Deliveries", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter MailboxRulesActiveDirectoryQueriesPerSecond = new ExPerformanceCounter("MSExchange Delivery Store Driver", "Mailbox Rules: Active Directory queries per second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MailboxRulesActiveDirectoryQueries = new ExPerformanceCounter("MSExchange Delivery Store Driver", "Mailbox Rules: Active Directory queries", string.Empty, null, new ExPerformanceCounter[]
		{
			MSExchangeStoreDriver.MailboxRulesActiveDirectoryQueriesPerSecond
		});

		private static readonly ExPerformanceCounter MailboxRulesMapiOperationsPerSecond = new ExPerformanceCounter("MSExchange Delivery Store Driver", "Mailbox Rules: MAPI operations per second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MailboxRulesMapiOperations = new ExPerformanceCounter("MSExchange Delivery Store Driver", "Mailbox Rules: MAPI operations", string.Empty, null, new ExPerformanceCounter[]
		{
			MSExchangeStoreDriver.MailboxRulesMapiOperationsPerSecond
		});

		public static readonly ExPerformanceCounter MailboxRulesMilliseconds = new ExPerformanceCounter("MSExchange Delivery Store Driver", "Mailbox Rules: Average milliseconds spent processing rules.", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MailboxRulesMilliseconds90thPercentile = new ExPerformanceCounter("MSExchange Delivery Store Driver", "Mailbox Rules: 90th percentile of milliseconds spent processing rules.", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMeetingMessages = new ExPerformanceCounter("MSExchange Delivery Store Driver", "Inbound: TotalMeetingMessages", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMeetingFailures = new ExPerformanceCounter("MSExchange Delivery Store Driver", "Inbound: TotalMeetingFailures", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PendingDeliveries = new ExPerformanceCounter("MSExchange Delivery Store Driver", "Pending Deliveries", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DeliveryAttempts = new ExPerformanceCounter("MSExchange Delivery Store Driver", "Delivery attempts per minute over the last 5 minutes", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DeliveryFailures = new ExPerformanceCounter("MSExchange Delivery Store Driver", "Delivery failures per minute over the last 5 minutes", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SuccessfulDeliveriesPerSecond = new ExPerformanceCounter("MSExchange Delivery Store Driver", "SuccessfulDeliveriesPerSecond", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SuccessfulDeliveries = new ExPerformanceCounter("MSExchange Delivery Store Driver", "SuccessfulDeliveries", string.Empty, null, new ExPerformanceCounter[]
		{
			MSExchangeStoreDriver.SuccessfulDeliveriesPerSecond
		});

		private static readonly ExPerformanceCounter FailedDeliveriesPerSecond = new ExPerformanceCounter("MSExchange Delivery Store Driver", "FailedDeliveriesPerSecond", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FailedDeliveries = new ExPerformanceCounter("MSExchange Delivery Store Driver", "FailedDeliveries", string.Empty, null, new ExPerformanceCounter[]
		{
			MSExchangeStoreDriver.FailedDeliveriesPerSecond
		});

		public static readonly ExPerformanceCounter TotalUnjournaledItemsDelivered = new ExPerformanceCounter("MSExchange Delivery Store Driver", "Unjournaling: Total number of unjournaled items delivered", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DeliveryLatencyPerRecipientMilliseconds = new ExPerformanceCounter("MSExchange Delivery Store Driver", "Average delivery latency per recipient in milliseconds", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			MSExchangeStoreDriver.LocalDeliveryCalls,
			MSExchangeStoreDriver.MessageDeliveryAttempts,
			MSExchangeStoreDriver.RecipientsDelivered,
			MSExchangeStoreDriver.RecipientLevelPercentFailedDeliveries,
			MSExchangeStoreDriver.RecipientLevelPercentTemporaryFailedDeliveries,
			MSExchangeStoreDriver.BytesDelivered,
			MSExchangeStoreDriver.CurrentDeliveryThreads,
			MSExchangeStoreDriver.DeliveryRetry,
			MSExchangeStoreDriver.DeliveryReroute,
			MSExchangeStoreDriver.DuplicateDelivery,
			MSExchangeStoreDriver.MailboxRulesActiveDirectoryQueries,
			MSExchangeStoreDriver.MailboxRulesMapiOperations,
			MSExchangeStoreDriver.MailboxRulesMilliseconds,
			MSExchangeStoreDriver.MailboxRulesMilliseconds90thPercentile,
			MSExchangeStoreDriver.TotalMeetingMessages,
			MSExchangeStoreDriver.TotalMeetingFailures,
			MSExchangeStoreDriver.PendingDeliveries,
			MSExchangeStoreDriver.DeliveryAttempts,
			MSExchangeStoreDriver.DeliveryFailures,
			MSExchangeStoreDriver.SuccessfulDeliveries,
			MSExchangeStoreDriver.FailedDeliveries,
			MSExchangeStoreDriver.TotalUnjournaledItemsDelivered,
			MSExchangeStoreDriver.DeliveryLatencyPerRecipientMilliseconds
		};
	}
}
