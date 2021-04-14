using System;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class RpcProcessingTimeProcessor : IClientPerformanceDataSink
	{
		private RpcProcessingTimeProcessor()
		{
		}

		void IClientPerformanceDataSink.ReportEvent(ClientPerformanceEventArgs clientEvent)
		{
		}

		void IClientPerformanceDataSink.ReportLatency(TimeSpan latency)
		{
			ProtocolLog.UpdateClientRpcLatency(latency);
		}

		internal static RpcProcessingTimeProcessor Create()
		{
			return RpcProcessingTimeProcessor.instance;
		}

		private static readonly RpcProcessingTimeProcessor instance = new RpcProcessingTimeProcessor();
	}
}
