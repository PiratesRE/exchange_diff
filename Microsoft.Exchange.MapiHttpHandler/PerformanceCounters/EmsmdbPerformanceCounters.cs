using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MapiHttp.PerformanceCounters
{
	internal static class EmsmdbPerformanceCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (EmsmdbPerformanceCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in EmsmdbPerformanceCounters.AllCounters)
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

		public const string CategoryName = "MSExchange MapiHttp Emsmdb";

		public static readonly ExPerformanceCounter Requests = new ExPerformanceCounter("MSExchange MapiHttp Emsmdb", "Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PacketsRate = new ExPerformanceCounter("MSExchange MapiHttp Emsmdb", "Packets/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter OperationsRate = new ExPerformanceCounter("MSExchange MapiHttp Emsmdb", "Operations/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AveragedLatency = new ExPerformanceCounter("MSExchange MapiHttp Emsmdb", "Averaged Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter BytesRead = new ExPerformanceCounter("MSExchange MapiHttp Emsmdb", "Clients Bytes Read", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter BytesWritten = new ExPerformanceCounter("MSExchange MapiHttp Emsmdb", "Clients Bytes Written", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter UncompressedBytesRead = new ExPerformanceCounter("MSExchange MapiHttp Emsmdb", "Clients Uncompressed Bytes Read", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter UncompressedBytesWritten = new ExPerformanceCounter("MSExchange MapiHttp Emsmdb", "Clients Uncompressed Bytes Written", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ConnectionCount = new ExPerformanceCounter("MSExchange MapiHttp Emsmdb", "Connection Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ActiveUserCount = new ExPerformanceCounter("MSExchange MapiHttp Emsmdb", "Active User Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter UserCount = new ExPerformanceCounter("MSExchange MapiHttp Emsmdb", "User Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ClientCallsAttempted = new ExPerformanceCounter("MSExchange MapiHttp Emsmdb", "Client: Calls attempted", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ClientCallsSucceeded = new ExPerformanceCounter("MSExchange MapiHttp Emsmdb", "Client: Calls succeeded", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ClientBackgroundCallsSucceeded = new ExPerformanceCounter("MSExchange MapiHttp Emsmdb", "Client: Background calls succeeded", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ClientForegroundCallsSucceeded = new ExPerformanceCounter("MSExchange MapiHttp Emsmdb", "Client: Foreground calls succeeded", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ClientCallsFailed = new ExPerformanceCounter("MSExchange MapiHttp Emsmdb", "Client: calls Failed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ClientBackgroundCallsFailed = new ExPerformanceCounter("MSExchange MapiHttp Emsmdb", "Client: Background calls Failed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ClientForegroundCallsFailed = new ExPerformanceCounter("MSExchange MapiHttp Emsmdb", "Client: Foreground calls Failed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ClientCallsSlow1 = new ExPerformanceCounter("MSExchange MapiHttp Emsmdb", "Client: Latency > 2 sec calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ClientCallsSlow2 = new ExPerformanceCounter("MSExchange MapiHttp Emsmdb", "Client: Latency > 5 sec calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ClientCallsSlow3 = new ExPerformanceCounter("MSExchange MapiHttp Emsmdb", "Client: Latency > 10 sec calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DispatchTaskQueueLength = new ExPerformanceCounter("MSExchange MapiHttp Emsmdb", "Dispatch task queue length", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DispatchTaskThreads = new ExPerformanceCounter("MSExchange MapiHttp Emsmdb", "Dispatch task threads", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DispatchTaskActiveThreads = new ExPerformanceCounter("MSExchange MapiHttp Emsmdb", "Dispatch task active threads", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DispatchTaskOperationsRate = new ExPerformanceCounter("MSExchange MapiHttp Emsmdb", "Dispatch task operations/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			EmsmdbPerformanceCounters.Requests,
			EmsmdbPerformanceCounters.PacketsRate,
			EmsmdbPerformanceCounters.OperationsRate,
			EmsmdbPerformanceCounters.AveragedLatency,
			EmsmdbPerformanceCounters.BytesRead,
			EmsmdbPerformanceCounters.BytesWritten,
			EmsmdbPerformanceCounters.UncompressedBytesRead,
			EmsmdbPerformanceCounters.UncompressedBytesWritten,
			EmsmdbPerformanceCounters.ConnectionCount,
			EmsmdbPerformanceCounters.ActiveUserCount,
			EmsmdbPerformanceCounters.UserCount,
			EmsmdbPerformanceCounters.ClientCallsAttempted,
			EmsmdbPerformanceCounters.ClientCallsSucceeded,
			EmsmdbPerformanceCounters.ClientBackgroundCallsSucceeded,
			EmsmdbPerformanceCounters.ClientForegroundCallsSucceeded,
			EmsmdbPerformanceCounters.ClientCallsFailed,
			EmsmdbPerformanceCounters.ClientBackgroundCallsFailed,
			EmsmdbPerformanceCounters.ClientForegroundCallsFailed,
			EmsmdbPerformanceCounters.ClientCallsSlow1,
			EmsmdbPerformanceCounters.ClientCallsSlow2,
			EmsmdbPerformanceCounters.ClientCallsSlow3,
			EmsmdbPerformanceCounters.DispatchTaskQueueLength,
			EmsmdbPerformanceCounters.DispatchTaskThreads,
			EmsmdbPerformanceCounters.DispatchTaskActiveThreads,
			EmsmdbPerformanceCounters.DispatchTaskOperationsRate
		};
	}
}
