using System;

namespace Microsoft.Exchange.HttpProxy.Routing
{
	public interface IRoutingDiagnostics
	{
		void AddAccountForestLatency(TimeSpan latency);

		void AddResourceForestLatency(TimeSpan latency);

		void AddActiveManagerLatency(TimeSpan latency);

		void AddGlobalLocatorLatency(TimeSpan latency);

		void AddServerLocatorLatency(TimeSpan latency);

		void AddSharedCacheLatency(TimeSpan latency);

		void AddDiagnosticText(string text);
	}
}
