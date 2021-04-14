using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.SenderId
{
	internal static class SenderIdPerfCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (SenderIdPerfCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in SenderIdPerfCounters.AllCounters)
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

		public const string CategoryName = "MSExchange Sender Id Agent";

		private static readonly ExPerformanceCounter MessagesValidatedPerSecond = new ExPerformanceCounter("MSExchange Sender Id Agent", "Messages Validated/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesValidated = new ExPerformanceCounter("MSExchange Sender Id Agent", "Messages Validated", string.Empty, null, new ExPerformanceCounter[]
		{
			SenderIdPerfCounters.MessagesValidatedPerSecond
		});

		private static readonly ExPerformanceCounter MessagesThatBypassedValidationPerSecond = new ExPerformanceCounter("MSExchange Sender Id Agent", "Messages That Bypassed Validation/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesThatBypassedValidation = new ExPerformanceCounter("MSExchange Sender Id Agent", "Messages That Bypassed Validation", string.Empty, null, new ExPerformanceCounter[]
		{
			SenderIdPerfCounters.MessagesThatBypassedValidationPerSecond
		});

		private static readonly ExPerformanceCounter MessagesWithPassResultPerSecond = new ExPerformanceCounter("MSExchange Sender Id Agent", "Messages Validated/sec with a Pass Result", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesWithPassResult = new ExPerformanceCounter("MSExchange Sender Id Agent", "Messages Validated with a Pass Result", string.Empty, null, new ExPerformanceCounter[]
		{
			SenderIdPerfCounters.MessagesWithPassResultPerSecond
		});

		private static readonly ExPerformanceCounter MessagesWithNeutralResultPerSecond = new ExPerformanceCounter("MSExchange Sender Id Agent", "Messages Validated/sec with a Neutral Result", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesWithNeutralResult = new ExPerformanceCounter("MSExchange Sender Id Agent", "Messages Validated with a Neutral Result", string.Empty, null, new ExPerformanceCounter[]
		{
			SenderIdPerfCounters.MessagesWithNeutralResultPerSecond
		});

		private static readonly ExPerformanceCounter MessagesWithSoftFailResultPerSecond = new ExPerformanceCounter("MSExchange Sender Id Agent", "Messages Validated/sec with a SoftFail Result", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesWithSoftFailResult = new ExPerformanceCounter("MSExchange Sender Id Agent", "Messages Validated with a SoftFail Result", string.Empty, null, new ExPerformanceCounter[]
		{
			SenderIdPerfCounters.MessagesWithSoftFailResultPerSecond
		});

		private static readonly ExPerformanceCounter MessagesWithFailNotPermittedResultPerSecond = new ExPerformanceCounter("MSExchange Sender Id Agent", "Messages Validated/sec with a Fail Not - Permitted Result", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesWithFailNotPermittedResult = new ExPerformanceCounter("MSExchange Sender Id Agent", "Messages Validated with a Fail - Not Permitted Result", string.Empty, null, new ExPerformanceCounter[]
		{
			SenderIdPerfCounters.MessagesWithFailNotPermittedResultPerSecond
		});

		private static readonly ExPerformanceCounter MessagesWithFailMalformedDomainResultPerSecond = new ExPerformanceCounter("MSExchange Sender Id Agent", "Messages Validated/sec with a Fail - Malformed Domain Result", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesWithFailMalformedDomainResult = new ExPerformanceCounter("MSExchange Sender Id Agent", "Messages Validated with a Fail - Malformed Domain Result", string.Empty, null, new ExPerformanceCounter[]
		{
			SenderIdPerfCounters.MessagesWithFailMalformedDomainResultPerSecond
		});

		private static readonly ExPerformanceCounter MessagesWithFailNonExistentDomainResultPerSecond = new ExPerformanceCounter("MSExchange Sender Id Agent", "Messages Validated/sec with a Fail - Non-Existent Domain Result", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesWithFailNonExistentDomainResult = new ExPerformanceCounter("MSExchange Sender Id Agent", "Messages Validated with a Fail - Non-Existent Domain Result", string.Empty, null, new ExPerformanceCounter[]
		{
			SenderIdPerfCounters.MessagesWithFailNonExistentDomainResultPerSecond
		});

		private static readonly ExPerformanceCounter MessagesWithNoneResultPerSecond = new ExPerformanceCounter("MSExchange Sender Id Agent", "Messages Validated/sec with a None Result", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesWithNoneResult = new ExPerformanceCounter("MSExchange Sender Id Agent", "Messages Validated with a None Result", string.Empty, null, new ExPerformanceCounter[]
		{
			SenderIdPerfCounters.MessagesWithNoneResultPerSecond
		});

		private static readonly ExPerformanceCounter MessagesWithTempErrorResultPerSecond = new ExPerformanceCounter("MSExchange Sender Id Agent", "Messages Validated/sec with a TempError Result", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesWithTempErrorResult = new ExPerformanceCounter("MSExchange Sender Id Agent", "Messages Validated with a TempError Result", string.Empty, null, new ExPerformanceCounter[]
		{
			SenderIdPerfCounters.MessagesWithTempErrorResultPerSecond
		});

		private static readonly ExPerformanceCounter MessagesWithPermErrorResultPerSecond = new ExPerformanceCounter("MSExchange Sender Id Agent", "Messages Validated/sec with a PermError Result", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesWithPermErrorResult = new ExPerformanceCounter("MSExchange Sender Id Agent", "Messages Validated with a PermError Result", string.Empty, null, new ExPerformanceCounter[]
		{
			SenderIdPerfCounters.MessagesWithPermErrorResultPerSecond
		});

		private static readonly ExPerformanceCounter DnsQueriesPerSecond = new ExPerformanceCounter("MSExchange Sender Id Agent", "DNS Queries/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDnsQueries = new ExPerformanceCounter("MSExchange Sender Id Agent", "DNS Queries", string.Empty, null, new ExPerformanceCounter[]
		{
			SenderIdPerfCounters.DnsQueriesPerSecond
		});

		private static readonly ExPerformanceCounter MessagesWithNoPRAPerSecond = new ExPerformanceCounter("MSExchange Sender Id Agent", "Messages With No PRA/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesWithNoPRA = new ExPerformanceCounter("MSExchange Sender Id Agent", "Messages with No PRA", string.Empty, null, new ExPerformanceCounter[]
		{
			SenderIdPerfCounters.MessagesWithNoPRAPerSecond
		});

		private static readonly ExPerformanceCounter MessagesMissingOriginatingIPPerSecond = new ExPerformanceCounter("MSExchange Sender Id Agent", "Messages Missing Originating IP/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesMissingOriginatingIP = new ExPerformanceCounter("MSExchange Sender Id Agent", "Messages Missing Originating IP", string.Empty, null, new ExPerformanceCounter[]
		{
			SenderIdPerfCounters.MessagesMissingOriginatingIPPerSecond
		});

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			SenderIdPerfCounters.TotalMessagesValidated,
			SenderIdPerfCounters.TotalMessagesThatBypassedValidation,
			SenderIdPerfCounters.TotalMessagesWithPassResult,
			SenderIdPerfCounters.TotalMessagesWithNeutralResult,
			SenderIdPerfCounters.TotalMessagesWithSoftFailResult,
			SenderIdPerfCounters.TotalMessagesWithFailNotPermittedResult,
			SenderIdPerfCounters.TotalMessagesWithFailMalformedDomainResult,
			SenderIdPerfCounters.TotalMessagesWithFailNonExistentDomainResult,
			SenderIdPerfCounters.TotalMessagesWithNoneResult,
			SenderIdPerfCounters.TotalMessagesWithTempErrorResult,
			SenderIdPerfCounters.TotalMessagesWithPermErrorResult,
			SenderIdPerfCounters.TotalDnsQueries,
			SenderIdPerfCounters.TotalMessagesWithNoPRA,
			SenderIdPerfCounters.TotalMessagesMissingOriginatingIP
		};
	}
}
