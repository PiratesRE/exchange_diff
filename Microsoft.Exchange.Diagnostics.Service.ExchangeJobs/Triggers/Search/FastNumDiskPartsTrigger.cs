using System;
using System.Collections.Generic;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Triggers.Search
{
	public class FastNumDiskPartsTrigger : PerInstanceTrigger
	{
		public FastNumDiskPartsTrigger(IJob job) : base(job, "Search Fs\\(.+?\\.Single\\)\\\\NumDiskParts", new PerfLogCounterTrigger.TriggerConfiguration("FastNumDiskPartsTrigger", 100.0, double.MaxValue, TimeSpan.FromMinutes(5.0), TimeSpan.FromHours(4.0), TimeSpan.FromDays(1.0), 0), new HashSet<DiagnosticMeasurement>(), new HashSet<string>())
		{
		}
	}
}
