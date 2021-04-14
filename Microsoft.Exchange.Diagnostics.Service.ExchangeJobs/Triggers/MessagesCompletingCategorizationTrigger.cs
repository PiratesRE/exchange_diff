using System;
using System.Collections.Generic;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Triggers
{
	public class MessagesCompletingCategorizationTrigger : TransportOverThresholdGatedTrigger
	{
		public MessagesCompletingCategorizationTrigger(IJob job) : base(job, "MSExchangeTransport Queues\\(_total\\)\\\\Messages Completing Categorization$", 0.0, new PerfLogCounterTrigger.TriggerConfiguration("MessagesCompletingCategorizationTrigger", 5.0, 1.0, TimeSpan.FromMinutes(5.0), TimeSpan.FromMinutes(5.0), TimeSpan.FromMinutes(5.0), 1), MessagesCompletingCategorizationTrigger.additionalContext, MessagesCompletingCategorizationTrigger.excludedInstances)
		{
		}

		protected override DiagnosticMeasurement AdditionalDiagnosticMeasurement(PerfLogCounterTrigger.SurpassedThresholdContext context)
		{
			return DiagnosticMeasurement.GetMeasure(context.Counter.MachineName, MessagesCompletingCategorizationTrigger.gatingCounter.ObjectName, MessagesCompletingCategorizationTrigger.gatingCounter.CounterName, MessagesCompletingCategorizationTrigger.gatingCounter.InstanceName);
		}

		private static readonly DiagnosticMeasurement gatingCounter = DiagnosticMeasurement.GetMeasure("MSExchangeTransport Queues", "Messages Submitted Per Second", "_total");

		private static readonly HashSet<string> excludedInstances = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		private static readonly HashSet<DiagnosticMeasurement> additionalContext = new HashSet<DiagnosticMeasurement>(DiagnosticMeasurement.CounterFilterComparer.Comparer)
		{
			MessagesCompletingCategorizationTrigger.gatingCounter
		};
	}
}
