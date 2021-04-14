using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Exchange.HttpProxy.Routing
{
	public class RoutingUpdateDiagnostics : IRoutingDiagnostics
	{
		public RoutingUpdateDiagnostics()
		{
			this.accountForestLatencies = new List<long>(2);
			this.globalLocatorLatencies = new List<long>(2);
			this.resourceForestLatencies = new List<long>(2);
			this.serverLocatorLatencies = new List<long>(2);
			this.activeManagerLatencies = new List<long>(2);
		}

		void IRoutingDiagnostics.AddAccountForestLatency(TimeSpan latency)
		{
			this.accountForestLatencies.Add(Convert.ToInt64(latency.TotalMilliseconds));
		}

		void IRoutingDiagnostics.AddResourceForestLatency(TimeSpan latency)
		{
			this.resourceForestLatencies.Add(Convert.ToInt64(latency.TotalMilliseconds));
		}

		void IRoutingDiagnostics.AddActiveManagerLatency(TimeSpan latency)
		{
			this.activeManagerLatencies.Add(Convert.ToInt64(latency.TotalMilliseconds));
		}

		void IRoutingDiagnostics.AddGlobalLocatorLatency(TimeSpan latency)
		{
			this.globalLocatorLatencies.Add(Convert.ToInt64(latency.TotalMilliseconds));
		}

		void IRoutingDiagnostics.AddServerLocatorLatency(TimeSpan latency)
		{
			this.serverLocatorLatencies.Add(Convert.ToInt64(latency.TotalMilliseconds));
		}

		void IRoutingDiagnostics.AddSharedCacheLatency(TimeSpan latency)
		{
		}

		void IRoutingDiagnostics.AddDiagnosticText(string text)
		{
		}

		internal long GetTotalLatency()
		{
			long num = 0L;
			num += this.accountForestLatencies.Sum();
			num += this.globalLocatorLatencies.Sum();
			num += this.resourceForestLatencies.Sum();
			num += this.serverLocatorLatencies.Sum();
			return num + this.activeManagerLatencies.Sum();
		}

		internal void Clear()
		{
			this.accountForestLatencies.Clear();
			this.globalLocatorLatencies.Clear();
			this.resourceForestLatencies.Clear();
			this.serverLocatorLatencies.Clear();
			this.activeManagerLatencies.Clear();
		}

		internal void LogLatencies(RequestDetailsLogger logger)
		{
			logger.Set(RoutingUpdateModuleMetadata.AccountForestLatencyBreakup, RoutingUpdateDiagnostics.GetBreakupOfLatencies(this.accountForestLatencies));
			logger.Set(RoutingUpdateModuleMetadata.TotalAccountForestLatency, this.accountForestLatencies.Sum());
			logger.Set(RoutingUpdateModuleMetadata.GlsLatencyBreakup, RoutingUpdateDiagnostics.GetBreakupOfLatencies(this.globalLocatorLatencies));
			logger.Set(RoutingUpdateModuleMetadata.TotalGlsLatency, this.globalLocatorLatencies.Sum());
			logger.Set(RoutingUpdateModuleMetadata.ResourceForestLatencyBreakup, RoutingUpdateDiagnostics.GetBreakupOfLatencies(this.resourceForestLatencies));
			logger.Set(RoutingUpdateModuleMetadata.TotalResourceForestLatency, this.resourceForestLatencies.Sum());
			logger.Set(RoutingUpdateModuleMetadata.ActiveManagerLatencyBreakup, RoutingUpdateDiagnostics.GetBreakupOfLatencies(this.activeManagerLatencies));
			logger.Set(RoutingUpdateModuleMetadata.TotalActiveManagerLatency, this.activeManagerLatencies.Sum());
			logger.Set(RoutingUpdateModuleMetadata.ServerLocatorLatency, this.serverLocatorLatencies.Sum());
		}

		private static string GetBreakupOfLatencies(List<long> latencies)
		{
			if (latencies == null)
			{
				throw new ArgumentNullException("latencies");
			}
			StringBuilder result = new StringBuilder();
			latencies.ForEach(delegate(long latency)
			{
				result.Append(latency);
				result.Append(';');
			});
			return result.ToString();
		}

		private readonly List<long> accountForestLatencies;

		private readonly List<long> globalLocatorLatencies;

		private readonly List<long> resourceForestLatencies;

		private readonly List<long> serverLocatorLatencies;

		private readonly List<long> activeManagerLatencies;
	}
}
