using System;

namespace Microsoft.Exchange.HttpProxy.Routing
{
	internal enum RoutingUpdateModuleMetadata
	{
		Protocol,
		ServerLocatorLatency,
		GlsLatencyBreakup,
		TotalGlsLatency,
		AccountForestLatencyBreakup,
		TotalAccountForestLatency,
		ResourceForestLatencyBreakup,
		TotalResourceForestLatency,
		ActiveManagerLatencyBreakup,
		TotalActiveManagerLatency
	}
}
