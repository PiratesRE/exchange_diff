using System;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class RpcFailureProcessor : IClientPerformanceDataSink
	{
		private RpcFailureProcessor()
		{
		}

		void IClientPerformanceDataSink.ReportEvent(ClientPerformanceEventArgs clientEvent)
		{
			if (clientEvent.EventType == ClientPerformanceEventType.RpcFailed && clientEvent is ClientFailureEventArgs)
			{
				ClientFailureEventArgs clientFailureEventArgs = (ClientFailureEventArgs)clientEvent;
				ProtocolLog.UpdateClientRpcFailureData(clientFailureEventArgs.TimeStamp, new FailureCounterData
				{
					FailureCode = clientFailureEventArgs.FailureCode
				});
				return;
			}
			if (clientEvent.EventType == ClientPerformanceEventType.RpcAttempted && clientEvent is ClientTimeStampedEventArgs)
			{
				ClientTimeStampedEventArgs clientTimeStampedEventArgs = (ClientTimeStampedEventArgs)clientEvent;
				ProtocolLog.UpdateClientRpcAttemptsData(clientTimeStampedEventArgs.TimeStamp, null);
			}
		}

		void IClientPerformanceDataSink.ReportLatency(TimeSpan clientLatency)
		{
		}

		internal static RpcFailureProcessor Create()
		{
			return RpcFailureProcessor.instance;
		}

		private static readonly RpcFailureProcessor instance = new RpcFailureProcessor();
	}
}
