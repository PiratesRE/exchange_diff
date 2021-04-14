using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.PerformanceCounters;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal class XtcPerformanceCounters : IXtcPerformanceCounters
	{
		public IExPerformanceCounter XTCDispatchTaskActiveThreads
		{
			get
			{
				return RpcClientAccessPerformanceCounters.XTCDispatchTaskActiveThreads;
			}
		}

		public IExPerformanceCounter XTCDispatchTaskOperationsRate
		{
			get
			{
				return RpcClientAccessPerformanceCounters.XTCDispatchTaskOperationsRate;
			}
		}

		public IExPerformanceCounter XTCDispatchTaskQueueLength
		{
			get
			{
				return RpcClientAccessPerformanceCounters.XTCDispatchTaskQueueLength;
			}
		}

		public IExPerformanceCounter XTCDispatchTaskThreads
		{
			get
			{
				return RpcClientAccessPerformanceCounters.XTCDispatchTaskThreads;
			}
		}
	}
}
