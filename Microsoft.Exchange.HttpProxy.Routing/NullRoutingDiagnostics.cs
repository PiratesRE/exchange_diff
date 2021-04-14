using System;

namespace Microsoft.Exchange.HttpProxy.Routing
{
	public class NullRoutingDiagnostics : IRoutingDiagnostics
	{
		void IRoutingDiagnostics.AddAccountForestLatency(TimeSpan latency)
		{
		}

		void IRoutingDiagnostics.AddResourceForestLatency(TimeSpan latency)
		{
		}

		void IRoutingDiagnostics.AddActiveManagerLatency(TimeSpan latency)
		{
		}

		void IRoutingDiagnostics.AddGlobalLocatorLatency(TimeSpan latency)
		{
		}

		void IRoutingDiagnostics.AddServerLocatorLatency(TimeSpan latency)
		{
		}

		void IRoutingDiagnostics.AddSharedCacheLatency(TimeSpan latency)
		{
		}

		void IRoutingDiagnostics.AddDiagnosticText(string text)
		{
		}

		public static readonly NullRoutingDiagnostics Instance = new NullRoutingDiagnostics();
	}
}
