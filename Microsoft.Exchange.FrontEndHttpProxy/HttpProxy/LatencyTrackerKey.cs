using System;

namespace Microsoft.Exchange.HttpProxy
{
	internal enum LatencyTrackerKey
	{
		CalculateTargetBackEndLatency,
		CalculateTargetBackEndSecondRoundLatency,
		HandlerToModuleSwitchingLatency,
		ModuleToHandlerSwitchingLatency,
		RequestHandlerLatency,
		ProxyModuleInitLatency,
		ProxyModuleLatency,
		AuthenticationLatency,
		BackendRequestInitLatency,
		BackendProcessingLatency,
		BackendResponseInitLatency,
		HandlerCompletionLatency,
		StreamingLatency,
		RouteRefresherLatency
	}
}
