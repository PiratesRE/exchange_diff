using System;
using System.Collections.Generic;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Triggers
{
	public class TransportMessagesDeferredDuringCategorizationTrigger : TransportOverThresholdGatedTrigger
	{
		public TransportMessagesDeferredDuringCategorizationTrigger(IJob job) : base(job, "MSExchangeTransport Queues\\(_total\\)\\\\Messages Deferred during Categorization", 500.0, new PerfLogCounterTrigger.TriggerConfiguration("TransportMessagesDeferredDuringCategorizationTrigger", 5.0, 10.0, TimeSpan.FromMinutes(5.0), TimeSpan.FromMinutes(5.0), TimeSpan.FromMinutes(5.0), 0), TransportMessagesDeferredDuringCategorizationTrigger.additionalContext, TransportMessagesDeferredDuringCategorizationTrigger.excludedInstances)
		{
		}

		protected override DiagnosticMeasurement AdditionalDiagnosticMeasurement(PerfLogCounterTrigger.SurpassedThresholdContext context)
		{
			return DiagnosticMeasurement.GetMeasure(context.Counter.MachineName, TransportMessagesDeferredDuringCategorizationTrigger.gatingCounter.ObjectName, TransportMessagesDeferredDuringCategorizationTrigger.gatingCounter.CounterName, TransportMessagesDeferredDuringCategorizationTrigger.gatingCounter.InstanceName);
		}

		private static readonly DiagnosticMeasurement gatingCounter = DiagnosticMeasurement.GetMeasure("MSExchangeTransport Queues", "Messages Queued For Delivery", "_total");

		private static readonly HashSet<string> excludedInstances = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		private static readonly HashSet<DiagnosticMeasurement> additionalContext = new HashSet<DiagnosticMeasurement>(DiagnosticMeasurement.CounterFilterComparer.Comparer)
		{
			TransportMessagesDeferredDuringCategorizationTrigger.gatingCounter
		};
	}
}
