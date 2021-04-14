using System;
using Microsoft.Exchange.HttpProxy.Routing;

namespace Microsoft.Exchange.HttpProxy.RouteSelector
{
	internal interface IRouteSelectorModuleDiagnostics : IRouteSelectorDiagnostics, IRoutingDiagnostics
	{
		void SetTargetServer(string value);

		void SetTargetServerVersion(string value);

		void SaveRoutingLatency(Action operationToTrack);

		void LogLatencies();

		void ProcessLatencyPerfCounters();
	}
}
