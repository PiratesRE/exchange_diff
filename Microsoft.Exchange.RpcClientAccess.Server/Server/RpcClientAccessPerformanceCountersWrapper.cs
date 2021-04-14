using System;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal static class RpcClientAccessPerformanceCountersWrapper
	{
		public static IRcaPerformanceCounters RcaPerformanceCounters
		{
			get
			{
				return RpcClientAccessPerformanceCountersWrapper.rcaPerformanceCounters;
			}
		}

		public static IRpcHttpConnectionRegistrationPerformanceCounters RpcHttpConnectionRegistrationPerformanceCounters
		{
			get
			{
				return RpcClientAccessPerformanceCountersWrapper.rpcHttpConnectionRegistrationPerformanceCounters;
			}
		}

		public static IXtcPerformanceCounters XtcPerformanceCounters
		{
			get
			{
				return RpcClientAccessPerformanceCountersWrapper.xtcPerformanceCounters;
			}
		}

		public static void Initialize(IRcaPerformanceCounters rcaPerformanceCounters, IRpcHttpConnectionRegistrationPerformanceCounters rpcHttpConnectionRegistrationPerformanceCounters, IXtcPerformanceCounters xtcPerformanceCounters)
		{
			RpcClientAccessPerformanceCountersWrapper.rcaPerformanceCounters = rcaPerformanceCounters;
			RpcClientAccessPerformanceCountersWrapper.InitializeCounters(RpcClientAccessPerformanceCountersWrapper.rcaPerformanceCounters);
			RpcClientAccessPerformanceCountersWrapper.rpcHttpConnectionRegistrationPerformanceCounters = rpcHttpConnectionRegistrationPerformanceCounters;
			RpcClientAccessPerformanceCountersWrapper.InitializeCounters(RpcClientAccessPerformanceCountersWrapper.rpcHttpConnectionRegistrationPerformanceCounters);
			RpcClientAccessPerformanceCountersWrapper.xtcPerformanceCounters = xtcPerformanceCounters;
			RpcClientAccessPerformanceCountersWrapper.InitializeCounters(RpcClientAccessPerformanceCountersWrapper.xtcPerformanceCounters);
		}

		private static void InitializeCounters(object performanceCounters)
		{
			Type typeFromHandle = typeof(IExPerformanceCounter);
			foreach (PropertyInfo propertyInfo in performanceCounters.GetType().GetProperties())
			{
				if (typeFromHandle.IsAssignableFrom(propertyInfo.PropertyType))
				{
					IExPerformanceCounter exPerformanceCounter = propertyInfo.GetValue(performanceCounters, null) as IExPerformanceCounter;
					if (exPerformanceCounter != null)
					{
						exPerformanceCounter.RawValue = 0L;
					}
				}
			}
		}

		private static IRcaPerformanceCounters rcaPerformanceCounters = new NullRcaPerformanceCounters();

		private static IRpcHttpConnectionRegistrationPerformanceCounters rpcHttpConnectionRegistrationPerformanceCounters = new NullRpcHttpConnectionRegistrationPerformanceCounters();

		private static IXtcPerformanceCounters xtcPerformanceCounters = new NullXtcPerformanceCounters();
	}
}
