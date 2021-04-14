using System;
using System.Collections.Generic;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Triggers
{
	public class BootloaderOutstandingItemsTrigger : TransportOverThresholdGatedTrigger
	{
		public BootloaderOutstandingItemsTrigger(IJob job) : base(job, "Process\\(EdgeTransport\\)\\\\Elapsed Time$", 1.0, new PerfLogCounterTrigger.TriggerConfiguration("BootloaderOutstandingItemsTrigger", 1200.0, 1800.0, TimeSpan.FromMinutes(5.0), TimeSpan.FromMinutes(5.0), TimeSpan.FromMinutes(5.0), 0), new HashSet<DiagnosticMeasurement>(DiagnosticMeasurement.CounterFilterComparer.Comparer)
		{
			BootloaderOutstandingItemsTrigger.GatingCounter
		}, new HashSet<string>(StringComparer.OrdinalIgnoreCase))
		{
		}

		protected override DiagnosticMeasurement AdditionalDiagnosticMeasurement(PerfLogCounterTrigger.SurpassedThresholdContext context)
		{
			return DiagnosticMeasurement.GetMeasure(context.Counter.MachineName, BootloaderOutstandingItemsTrigger.GatingCounter.ObjectName, BootloaderOutstandingItemsTrigger.GatingCounter.CounterName, BootloaderOutstandingItemsTrigger.GatingCounter.InstanceName);
		}

		private const int OutstandingItemsTriggerThreshold = 1;

		private const int ProcessElapsedTriggerWarningThreshold = 1200;

		private const int ProcessElapsedTriggerErrorThreshold = 1800;

		private static readonly DiagnosticMeasurement GatingCounter = DiagnosticMeasurement.GetMeasure("MsExchangeTransport Database", "Bootloader Outstanding Items", "other");
	}
}
