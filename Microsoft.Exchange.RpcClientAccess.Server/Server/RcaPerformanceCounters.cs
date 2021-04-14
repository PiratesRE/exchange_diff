using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.PerformanceCounters;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal class RcaPerformanceCounters : IRcaPerformanceCounters
	{
		public IExPerformanceCounter ActiveUserCount
		{
			get
			{
				return RpcClientAccessPerformanceCounters.ActiveUserCount;
			}
		}

		public IExPerformanceCounter AveragedLatency
		{
			get
			{
				return RpcClientAccessPerformanceCounters.RPCAveragedLatency;
			}
		}

		public IExPerformanceCounter BytesRead
		{
			get
			{
				return RpcClientAccessPerformanceCounters.RPCBytesRead;
			}
		}

		public IExPerformanceCounter BytesWritten
		{
			get
			{
				return RpcClientAccessPerformanceCounters.RPCBytesWritten;
			}
		}

		public IExPerformanceCounter ClientBackgroundCallsFailed
		{
			get
			{
				return RpcClientAccessPerformanceCounters.ClientBackgroundRpcFailed;
			}
		}

		public IExPerformanceCounter ClientBackgroundCallsSucceeded
		{
			get
			{
				return RpcClientAccessPerformanceCounters.ClientBackgroundRpcSucceeded;
			}
		}

		public IExPerformanceCounter ClientCallsAttempted
		{
			get
			{
				return RpcClientAccessPerformanceCounters.ClientRpcAttempted;
			}
		}

		public IExPerformanceCounter ClientCallsFailed
		{
			get
			{
				return RpcClientAccessPerformanceCounters.ClientRpcFailed;
			}
		}

		public IExPerformanceCounter ClientCallsSlow1
		{
			get
			{
				return RpcClientAccessPerformanceCounters.ClientRpcSlow1;
			}
		}

		public IExPerformanceCounter ClientCallsSlow2
		{
			get
			{
				return RpcClientAccessPerformanceCounters.ClientRpcSlow2;
			}
		}

		public IExPerformanceCounter ClientCallsSlow3
		{
			get
			{
				return RpcClientAccessPerformanceCounters.ClientRpcSlow3;
			}
		}

		public IExPerformanceCounter ClientCallsSucceeded
		{
			get
			{
				return RpcClientAccessPerformanceCounters.ClientRpcSucceeded;
			}
		}

		public IExPerformanceCounter ClientForegroundCallsFailed
		{
			get
			{
				return RpcClientAccessPerformanceCounters.ClientForegroundRpcFailed;
			}
		}

		public IExPerformanceCounter ClientForegroundCallsSucceeded
		{
			get
			{
				return RpcClientAccessPerformanceCounters.ClientForegroundRpcSucceeded;
			}
		}

		public IExPerformanceCounter ConnectionCount
		{
			get
			{
				return RpcClientAccessPerformanceCounters.ConnectionCount;
			}
		}

		public IExPerformanceCounter DispatchTaskActiveThreads
		{
			get
			{
				return RpcClientAccessPerformanceCounters.RPCDispatchTaskActiveThreads;
			}
		}

		public IExPerformanceCounter DispatchTaskOperationsRate
		{
			get
			{
				return RpcClientAccessPerformanceCounters.RPCDispatchTaskOperationsRate;
			}
		}

		public IExPerformanceCounter DispatchTaskQueueLength
		{
			get
			{
				return RpcClientAccessPerformanceCounters.RPCDispatchTaskQueueLength;
			}
		}

		public IExPerformanceCounter DispatchTaskThreads
		{
			get
			{
				return RpcClientAccessPerformanceCounters.RPCDispatchTaskThreads;
			}
		}

		public IExPerformanceCounter OperationsRate
		{
			get
			{
				return RpcClientAccessPerformanceCounters.RPCOperationsRate;
			}
		}

		public IExPerformanceCounter PacketsRate
		{
			get
			{
				return RpcClientAccessPerformanceCounters.RPCPacketsRate;
			}
		}

		public IExPerformanceCounter Requests
		{
			get
			{
				return RpcClientAccessPerformanceCounters.RPCRequests;
			}
		}

		public IExPerformanceCounter UncompressedBytesRead
		{
			get
			{
				return RpcClientAccessPerformanceCounters.RPCUncompressedBytesRead;
			}
		}

		public IExPerformanceCounter UncompressedBytesWritten
		{
			get
			{
				return RpcClientAccessPerformanceCounters.RPCUncompressedBytesWritten;
			}
		}

		public IExPerformanceCounter UserCount
		{
			get
			{
				return RpcClientAccessPerformanceCounters.UserCount;
			}
		}
	}
}
