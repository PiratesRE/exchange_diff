using System;
using System.Collections.Generic;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Triggers
{
	public class E2ETransportLatencyLowTrigger : TransportOverThresholdGatedTrigger
	{
		public E2ETransportLatencyLowTrigger(IJob job) : base(job, "MSExchangeTransport End To End Latency\\(total - low\\)\\\\Percentile80$", 50.0, new PerfLogCounterTrigger.TriggerConfiguration("E2ETransportLatencyLowTrigger", 450.0, 900.0, TimeSpan.FromMinutes(5.0), TimeSpan.FromMinutes(5.0), TimeSpan.FromMinutes(5.0), 0), E2ETransportLatencyLowTrigger.additionalContext, E2ETransportLatencyLowTrigger.excludedInstances)
		{
		}

		protected override DiagnosticMeasurement AdditionalDiagnosticMeasurement(PerfLogCounterTrigger.SurpassedThresholdContext context)
		{
			return DiagnosticMeasurement.GetMeasure(context.Counter.MachineName, E2ETransportLatencyLowTrigger.gatingCounter.ObjectName, E2ETransportLatencyLowTrigger.gatingCounter.CounterName, E2ETransportLatencyLowTrigger.gatingCounter.InstanceName);
		}

		private static readonly DiagnosticMeasurement gatingCounter = DiagnosticMeasurement.GetMeasure("MSExchangeTransport End To End Latency", "Percentile80Samples", "Total - Low");

		private static readonly HashSet<string> excludedInstances = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		private static readonly HashSet<DiagnosticMeasurement> additionalContext = new HashSet<DiagnosticMeasurement>(DiagnosticMeasurement.CounterFilterComparer.Comparer)
		{
			E2ETransportLatencyLowTrigger.gatingCounter
		};
	}
}
