using System;
using System.Collections.Generic;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Triggers
{
	public class StoreQuarantinedMailboxCountTrigger : PerInstanceTrigger
	{
		public StoreQuarantinedMailboxCountTrigger(IJob job) : base(job, "MSExchangeIS Store\\(.+?\\)\\\\Quarantined Mailbox Count", new PerfLogCounterTrigger.TriggerConfiguration("StoreQuarantinedMailboxCountTrigger", double.NaN, 1.0, TimeSpan.FromMinutes(5.0), TimeSpan.FromMinutes(10.0), TimeSpan.FromMinutes(5.0), 0), new HashSet<DiagnosticMeasurement>(), StoreQuarantinedMailboxCountTrigger.excludedInstances)
		{
		}

		private static readonly HashSet<string> excludedInstances = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"_Total"
		};
	}
}
