using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal interface IRpcHttpConnectionRegistrationPerformanceCounters
	{
		IExPerformanceCounter RpcHttpConnectionRegistrationDispatchTaskActiveThreads { get; }

		IExPerformanceCounter RpcHttpConnectionRegistrationDispatchTaskOperationsRate { get; }

		IExPerformanceCounter RpcHttpConnectionRegistrationDispatchTaskQueueLength { get; }

		IExPerformanceCounter RpcHttpConnectionRegistrationDispatchTaskThreads { get; }
	}
}
