using System;
using System.Collections.Generic;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Triggers
{
	public class StorePercentRpcRequestsTrigger : PerInstanceTrigger
	{
		public StorePercentRpcRequestsTrigger(IJob job) : base(job, "MSExchangeIS Store\\(.+?\\)\\\\% RPC Requests", new PerfLogCounterTrigger.TriggerConfiguration("StorePercentRpcRequestsTrigger", double.NaN, 90.0, TimeSpan.FromMinutes(5.0), TimeSpan.FromMinutes(10.0), TimeSpan.FromMinutes(5.0), 0), new HashSet<DiagnosticMeasurement>(), StorePercentRpcRequestsTrigger.excludedInstances)
		{
		}

		private static readonly HashSet<string> excludedInstances = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"_Total"
		};
	}
}
