using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal interface IXtcPerformanceCounters
	{
		IExPerformanceCounter XTCDispatchTaskActiveThreads { get; }

		IExPerformanceCounter XTCDispatchTaskOperationsRate { get; }

		IExPerformanceCounter XTCDispatchTaskQueueLength { get; }

		IExPerformanceCounter XTCDispatchTaskThreads { get; }
	}
}
