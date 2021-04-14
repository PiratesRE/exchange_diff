using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal class NullXtcPerformanceCounters : IXtcPerformanceCounters
	{
		public IExPerformanceCounter XTCDispatchTaskActiveThreads
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter XTCDispatchTaskOperationsRate
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter XTCDispatchTaskQueueLength
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter XTCDispatchTaskThreads
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}
	}
}
