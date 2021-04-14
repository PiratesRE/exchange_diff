using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal class NullRpcHttpConnectionRegistrationPerformanceCounters : IRpcHttpConnectionRegistrationPerformanceCounters
	{
		public IExPerformanceCounter RpcHttpConnectionRegistrationDispatchTaskActiveThreads
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter RpcHttpConnectionRegistrationDispatchTaskOperationsRate
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter RpcHttpConnectionRegistrationDispatchTaskQueueLength
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter RpcHttpConnectionRegistrationDispatchTaskThreads
		{
			get
			{
				return NullPerformanceCounter.Instance;
			}
		}
	}
}
