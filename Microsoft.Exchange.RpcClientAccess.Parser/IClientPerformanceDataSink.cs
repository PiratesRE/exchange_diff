using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal interface IClientPerformanceDataSink
	{
		void ReportEvent(ClientPerformanceEventArgs clientEvent);

		void ReportLatency(TimeSpan clientLatency);
	}
}
