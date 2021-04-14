using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.PerformanceCounters;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal class RpcHttpConnectionRegistrationPerformanceCounters : IRpcHttpConnectionRegistrationPerformanceCounters
	{
		public IExPerformanceCounter RpcHttpConnectionRegistrationDispatchTaskActiveThreads
		{
			get
			{
				return RpcClientAccessPerformanceCounters.RpcHttpConnectionRegistrationDispatchTaskActiveThreads;
			}
		}

		public IExPerformanceCounter RpcHttpConnectionRegistrationDispatchTaskOperationsRate
		{
			get
			{
				return RpcClientAccessPerformanceCounters.RpcHttpConnectionRegistrationDispatchTaskOperationsRate;
			}
		}

		public IExPerformanceCounter RpcHttpConnectionRegistrationDispatchTaskQueueLength
		{
			get
			{
				return RpcClientAccessPerformanceCounters.RpcHttpConnectionRegistrationDispatchTaskQueueLength;
			}
		}

		public IExPerformanceCounter RpcHttpConnectionRegistrationDispatchTaskThreads
		{
			get
			{
				return RpcClientAccessPerformanceCounters.RpcHttpConnectionRegistrationDispatchTaskThreads;
			}
		}

		public IExPerformanceCounter[] AllCounters
		{
			get
			{
				return this.allCounters;
			}
		}

		private readonly IExPerformanceCounter[] allCounters = new IExPerformanceCounter[]
		{
			RpcClientAccessPerformanceCounters.RpcHttpConnectionRegistrationDispatchTaskActiveThreads,
			RpcClientAccessPerformanceCounters.RpcHttpConnectionRegistrationDispatchTaskOperationsRate,
			RpcClientAccessPerformanceCounters.RpcHttpConnectionRegistrationDispatchTaskQueueLength,
			RpcClientAccessPerformanceCounters.RpcHttpConnectionRegistrationDispatchTaskThreads
		};
	}
}
