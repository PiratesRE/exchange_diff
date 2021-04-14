using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.PerformanceCounters
{
	internal static class RpcClientAccessPerformanceCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (RpcClientAccessPerformanceCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in RpcClientAccessPerformanceCounters.AllCounters)
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

		public const string CategoryName = "MSExchange RpcClientAccess";

		public static readonly ExPerformanceCounter RPCRequests = new ExPerformanceCounter("MSExchange RpcClientAccess", "RPC Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RPCPacketsRate = new ExPerformanceCounter("MSExchange RpcClientAccess", "RPC Packets/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RPCOperationsRate = new ExPerformanceCounter("MSExchange RpcClientAccess", "RPC Operations/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RPCAveragedLatency = new ExPerformanceCounter("MSExchange RpcClientAccess", "RPC Averaged Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RPCBytesRead = new ExPerformanceCounter("MSExchange RpcClientAccess", "RPC Clients Bytes Read", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RPCBytesWritten = new ExPerformanceCounter("MSExchange RpcClientAccess", "RPC Clients Bytes Written", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RPCUncompressedBytesRead = new ExPerformanceCounter("MSExchange RpcClientAccess", "RPC Clients Uncompressed Bytes Read", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RPCUncompressedBytesWritten = new ExPerformanceCounter("MSExchange RpcClientAccess", "RPC Clients Uncompressed Bytes Written", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ConnectionCount = new ExPerformanceCounter("MSExchange RpcClientAccess", "Connection Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ActiveUserCount = new ExPerformanceCounter("MSExchange RpcClientAccess", "Active User Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter UserCount = new ExPerformanceCounter("MSExchange RpcClientAccess", "User Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ClientRpcAttempted = new ExPerformanceCounter("MSExchange RpcClientAccess", "Client: RPCs attempted", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ClientRpcSucceeded = new ExPerformanceCounter("MSExchange RpcClientAccess", "Client: RPCs succeeded", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ClientBackgroundRpcSucceeded = new ExPerformanceCounter("MSExchange RpcClientAccess", "Client: Background RPCs succeeded", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ClientForegroundRpcSucceeded = new ExPerformanceCounter("MSExchange RpcClientAccess", "Client: Foreground RPCs succeeded", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ClientRpcFailed = new ExPerformanceCounter("MSExchange RpcClientAccess", "Client: RPCs Failed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ClientBackgroundRpcFailed = new ExPerformanceCounter("MSExchange RpcClientAccess", "Client: Background RPCs Failed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ClientForegroundRpcFailed = new ExPerformanceCounter("MSExchange RpcClientAccess", "Client: Foreground RPCs Failed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ClientRpcSlow1 = new ExPerformanceCounter("MSExchange RpcClientAccess", "Client: Latency > 2 sec RPCs", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ClientRpcSlow2 = new ExPerformanceCounter("MSExchange RpcClientAccess", "Client: Latency > 5 sec RPCs", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ClientRpcSlow3 = new ExPerformanceCounter("MSExchange RpcClientAccess", "Client: Latency > 10 sec RPCs", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RPCDispatchTaskQueueLength = new ExPerformanceCounter("MSExchange RpcClientAccess", "RPC dispatch task queue length", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RPCDispatchTaskThreads = new ExPerformanceCounter("MSExchange RpcClientAccess", "RPC dispatch task threads", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RPCDispatchTaskActiveThreads = new ExPerformanceCounter("MSExchange RpcClientAccess", "RPC dispatch task active threads", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RPCDispatchTaskOperationsRate = new ExPerformanceCounter("MSExchange RpcClientAccess", "RPC dispatch task operations/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter XTCDispatchTaskQueueLength = new ExPerformanceCounter("MSExchange RpcClientAccess", "XTC dispatch task queue length", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter XTCDispatchTaskThreads = new ExPerformanceCounter("MSExchange RpcClientAccess", "XTC dispatch task threads", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter XTCDispatchTaskActiveThreads = new ExPerformanceCounter("MSExchange RpcClientAccess", "XTC dispatch task active threads", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter XTCDispatchTaskOperationsRate = new ExPerformanceCounter("MSExchange RpcClientAccess", "XTC dispatch task operations/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RpcHttpConnectionRegistrationDispatchTaskQueueLength = new ExPerformanceCounter("MSExchange RpcClientAccess", "RpcHttpConnectionRegistration dispatch task queue length", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RpcHttpConnectionRegistrationDispatchTaskThreads = new ExPerformanceCounter("MSExchange RpcClientAccess", "RpcHttpConnectionRegistration dispatch task threads", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RpcHttpConnectionRegistrationDispatchTaskActiveThreads = new ExPerformanceCounter("MSExchange RpcClientAccess", "RpcHttpConnectionRegistration dispatch task active threads", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RpcHttpConnectionRegistrationDispatchTaskOperationsRate = new ExPerformanceCounter("MSExchange RpcClientAccess", "RpcHttpConnectionRegistration dispatch task operations/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			RpcClientAccessPerformanceCounters.RPCRequests,
			RpcClientAccessPerformanceCounters.RPCPacketsRate,
			RpcClientAccessPerformanceCounters.RPCOperationsRate,
			RpcClientAccessPerformanceCounters.RPCAveragedLatency,
			RpcClientAccessPerformanceCounters.RPCBytesRead,
			RpcClientAccessPerformanceCounters.RPCBytesWritten,
			RpcClientAccessPerformanceCounters.RPCUncompressedBytesRead,
			RpcClientAccessPerformanceCounters.RPCUncompressedBytesWritten,
			RpcClientAccessPerformanceCounters.ConnectionCount,
			RpcClientAccessPerformanceCounters.ActiveUserCount,
			RpcClientAccessPerformanceCounters.UserCount,
			RpcClientAccessPerformanceCounters.ClientRpcAttempted,
			RpcClientAccessPerformanceCounters.ClientRpcSucceeded,
			RpcClientAccessPerformanceCounters.ClientBackgroundRpcSucceeded,
			RpcClientAccessPerformanceCounters.ClientForegroundRpcSucceeded,
			RpcClientAccessPerformanceCounters.ClientRpcFailed,
			RpcClientAccessPerformanceCounters.ClientBackgroundRpcFailed,
			RpcClientAccessPerformanceCounters.ClientForegroundRpcFailed,
			RpcClientAccessPerformanceCounters.ClientRpcSlow1,
			RpcClientAccessPerformanceCounters.ClientRpcSlow2,
			RpcClientAccessPerformanceCounters.ClientRpcSlow3,
			RpcClientAccessPerformanceCounters.RPCDispatchTaskQueueLength,
			RpcClientAccessPerformanceCounters.RPCDispatchTaskThreads,
			RpcClientAccessPerformanceCounters.RPCDispatchTaskActiveThreads,
			RpcClientAccessPerformanceCounters.RPCDispatchTaskOperationsRate,
			RpcClientAccessPerformanceCounters.XTCDispatchTaskQueueLength,
			RpcClientAccessPerformanceCounters.XTCDispatchTaskThreads,
			RpcClientAccessPerformanceCounters.XTCDispatchTaskActiveThreads,
			RpcClientAccessPerformanceCounters.XTCDispatchTaskOperationsRate,
			RpcClientAccessPerformanceCounters.RpcHttpConnectionRegistrationDispatchTaskQueueLength,
			RpcClientAccessPerformanceCounters.RpcHttpConnectionRegistrationDispatchTaskThreads,
			RpcClientAccessPerformanceCounters.RpcHttpConnectionRegistrationDispatchTaskActiveThreads,
			RpcClientAccessPerformanceCounters.RpcHttpConnectionRegistrationDispatchTaskOperationsRate
		};
	}
}
