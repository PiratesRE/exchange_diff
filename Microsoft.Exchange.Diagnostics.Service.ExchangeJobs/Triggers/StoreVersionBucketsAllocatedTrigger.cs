using System;
using System.Collections.Generic;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Triggers
{
	public class StoreVersionBucketsAllocatedTrigger : PerInstanceTrigger
	{
		public StoreVersionBucketsAllocatedTrigger(IJob job) : base(job, "MSExchange Database ==> Instances\\(Information Store.+?\\)\\\\Version Buckets Allocated", new PerfLogCounterTrigger.TriggerConfiguration("StoreVersionBucketsAllocatedTrigger", double.NaN, 12000.0, TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(5.0), TimeSpan.FromMinutes(2.0), 0), new HashSet<DiagnosticMeasurement>(), StoreVersionBucketsAllocatedTrigger.excludedInstances)
		{
		}

		private const double VersionBucketsAllocatedThreshold = 12000.0;

		private static readonly HashSet<string> excludedInstances = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"_Total"
		};
	}
}
