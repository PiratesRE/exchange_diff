using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MapiHttp.PerformanceCounters;
using Microsoft.Exchange.RpcClientAccess.Server;

namespace Microsoft.Exchange.MapiHttp
{
	internal class EmsmdbPerformanceCountersWrapper : IRcaPerformanceCounters
	{
		public IExPerformanceCounter ActiveUserCount
		{
			get
			{
				return EmsmdbPerformanceCounters.ActiveUserCount;
			}
		}

		public IExPerformanceCounter AveragedLatency
		{
			get
			{
				return EmsmdbPerformanceCounters.AveragedLatency;
			}
		}

		public IExPerformanceCounter BytesRead
		{
			get
			{
				return EmsmdbPerformanceCounters.BytesRead;
			}
		}

		public IExPerformanceCounter BytesWritten
		{
			get
			{
				return EmsmdbPerformanceCounters.BytesWritten;
			}
		}

		public IExPerformanceCounter ClientBackgroundCallsFailed
		{
			get
			{
				return EmsmdbPerformanceCounters.ClientBackgroundCallsFailed;
			}
		}

		public IExPerformanceCounter ClientBackgroundCallsSucceeded
		{
			get
			{
				return EmsmdbPerformanceCounters.ClientBackgroundCallsSucceeded;
			}
		}

		public IExPerformanceCounter ClientCallsAttempted
		{
			get
			{
				return EmsmdbPerformanceCounters.ClientCallsAttempted;
			}
		}

		public IExPerformanceCounter ClientCallsFailed
		{
			get
			{
				return EmsmdbPerformanceCounters.ClientCallsFailed;
			}
		}

		public IExPerformanceCounter ClientCallsSlow1
		{
			get
			{
				return EmsmdbPerformanceCounters.ClientCallsSlow1;
			}
		}

		public IExPerformanceCounter ClientCallsSlow2
		{
			get
			{
				return EmsmdbPerformanceCounters.ClientCallsSlow2;
			}
		}

		public IExPerformanceCounter ClientCallsSlow3
		{
			get
			{
				return EmsmdbPerformanceCounters.ClientCallsSlow3;
			}
		}

		public IExPerformanceCounter ClientCallsSucceeded
		{
			get
			{
				return EmsmdbPerformanceCounters.ClientCallsSucceeded;
			}
		}

		public IExPerformanceCounter ClientForegroundCallsFailed
		{
			get
			{
				return EmsmdbPerformanceCounters.ClientForegroundCallsFailed;
			}
		}

		public IExPerformanceCounter ClientForegroundCallsSucceeded
		{
			get
			{
				return EmsmdbPerformanceCounters.ClientForegroundCallsSucceeded;
			}
		}

		public IExPerformanceCounter ConnectionCount
		{
			get
			{
				return EmsmdbPerformanceCounters.ConnectionCount;
			}
		}

		public IExPerformanceCounter DispatchTaskActiveThreads
		{
			get
			{
				return EmsmdbPerformanceCounters.DispatchTaskActiveThreads;
			}
		}

		public IExPerformanceCounter DispatchTaskOperationsRate
		{
			get
			{
				return EmsmdbPerformanceCounters.DispatchTaskOperationsRate;
			}
		}

		public IExPerformanceCounter DispatchTaskQueueLength
		{
			get
			{
				return EmsmdbPerformanceCounters.DispatchTaskQueueLength;
			}
		}

		public IExPerformanceCounter DispatchTaskThreads
		{
			get
			{
				return EmsmdbPerformanceCounters.DispatchTaskThreads;
			}
		}

		public IExPerformanceCounter OperationsRate
		{
			get
			{
				return EmsmdbPerformanceCounters.OperationsRate;
			}
		}

		public IExPerformanceCounter PacketsRate
		{
			get
			{
				return EmsmdbPerformanceCounters.PacketsRate;
			}
		}

		public IExPerformanceCounter Requests
		{
			get
			{
				return EmsmdbPerformanceCounters.Requests;
			}
		}

		public IExPerformanceCounter UncompressedBytesRead
		{
			get
			{
				return EmsmdbPerformanceCounters.UncompressedBytesRead;
			}
		}

		public IExPerformanceCounter UncompressedBytesWritten
		{
			get
			{
				return EmsmdbPerformanceCounters.UncompressedBytesWritten;
			}
		}

		public IExPerformanceCounter UserCount
		{
			get
			{
				return EmsmdbPerformanceCounters.UserCount;
			}
		}
	}
}
