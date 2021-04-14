using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal interface IRcaPerformanceCounters
	{
		IExPerformanceCounter ActiveUserCount { get; }

		IExPerformanceCounter AveragedLatency { get; }

		IExPerformanceCounter BytesRead { get; }

		IExPerformanceCounter BytesWritten { get; }

		IExPerformanceCounter ClientBackgroundCallsFailed { get; }

		IExPerformanceCounter ClientBackgroundCallsSucceeded { get; }

		IExPerformanceCounter ClientCallsAttempted { get; }

		IExPerformanceCounter ClientCallsFailed { get; }

		IExPerformanceCounter ClientCallsSlow1 { get; }

		IExPerformanceCounter ClientCallsSlow2 { get; }

		IExPerformanceCounter ClientCallsSlow3 { get; }

		IExPerformanceCounter ClientCallsSucceeded { get; }

		IExPerformanceCounter ClientForegroundCallsFailed { get; }

		IExPerformanceCounter ClientForegroundCallsSucceeded { get; }

		IExPerformanceCounter ConnectionCount { get; }

		IExPerformanceCounter DispatchTaskActiveThreads { get; }

		IExPerformanceCounter DispatchTaskOperationsRate { get; }

		IExPerformanceCounter DispatchTaskQueueLength { get; }

		IExPerformanceCounter DispatchTaskThreads { get; }

		IExPerformanceCounter OperationsRate { get; }

		IExPerformanceCounter PacketsRate { get; }

		IExPerformanceCounter Requests { get; }

		IExPerformanceCounter UncompressedBytesRead { get; }

		IExPerformanceCounter UncompressedBytesWritten { get; }

		IExPerformanceCounter UserCount { get; }
	}
}
