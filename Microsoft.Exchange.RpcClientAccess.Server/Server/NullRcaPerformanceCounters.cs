using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal class NullRcaPerformanceCounters : IRcaPerformanceCounters
	{
		public IExPerformanceCounter ActiveUserCount
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter AveragedLatency
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter BytesRead
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter BytesWritten
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter ClientBackgroundCallsFailed
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter ClientBackgroundCallsSucceeded
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter ClientCallsAttempted
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter ClientCallsFailed
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter ClientCallsSlow1
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter ClientCallsSlow2
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter ClientCallsSlow3
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter ClientCallsSucceeded
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter ClientForegroundCallsFailed
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter ClientForegroundCallsSucceeded
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter ConnectionCount
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter DispatchTaskActiveThreads
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter DispatchTaskOperationsRate
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter DispatchTaskQueueLength
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter DispatchTaskThreads
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter OperationsRate
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter PacketsRate
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter Requests
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter UncompressedBytesRead
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter UncompressedBytesWritten
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter UserCount
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}
	}
}
