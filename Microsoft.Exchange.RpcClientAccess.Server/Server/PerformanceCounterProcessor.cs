using System;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class PerformanceCounterProcessor : IClientPerformanceDataSink
	{
		private PerformanceCounterProcessor()
		{
		}

		void IClientPerformanceDataSink.ReportEvent(ClientPerformanceEventArgs clientEvent)
		{
			switch (clientEvent.EventType)
			{
			case ClientPerformanceEventType.BackgroundRpcFailed:
				RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.ClientBackgroundCallsFailed.Increment();
				return;
			case ClientPerformanceEventType.BackgroundRpcSucceeded:
				RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.ClientBackgroundCallsSucceeded.Increment();
				return;
			case ClientPerformanceEventType.ForegroundRpcFailed:
				RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.ClientForegroundCallsFailed.Increment();
				return;
			case ClientPerformanceEventType.ForegroundRpcSucceeded:
				RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.ClientForegroundCallsSucceeded.Increment();
				return;
			case ClientPerformanceEventType.RpcAttempted:
				RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.ClientCallsAttempted.Increment();
				return;
			case ClientPerformanceEventType.RpcFailed:
				RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.ClientCallsFailed.Increment();
				return;
			case ClientPerformanceEventType.RpcSucceeded:
				RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.ClientCallsSucceeded.Increment();
				return;
			default:
				throw new ArgumentException(string.Format("Counter {0} not implemented.", clientEvent.EventType), "counter");
			}
		}

		void IClientPerformanceDataSink.ReportLatency(TimeSpan latency)
		{
			if (latency > PerformanceCounterProcessor.slow1)
			{
				RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.ClientCallsSlow1.Increment();
			}
			if (latency > PerformanceCounterProcessor.slow2)
			{
				RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.ClientCallsSlow2.Increment();
			}
			if (latency > PerformanceCounterProcessor.slow3)
			{
				RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.ClientCallsSlow3.Increment();
			}
		}

		internal static PerformanceCounterProcessor Create()
		{
			return PerformanceCounterProcessor.instance;
		}

		private static readonly PerformanceCounterProcessor instance = new PerformanceCounterProcessor();

		private static readonly TimeSpan slow1 = TimeSpan.FromSeconds(2.0);

		private static readonly TimeSpan slow2 = TimeSpan.FromSeconds(5.0);

		private static readonly TimeSpan slow3 = TimeSpan.FromSeconds(10.0);
	}
}
