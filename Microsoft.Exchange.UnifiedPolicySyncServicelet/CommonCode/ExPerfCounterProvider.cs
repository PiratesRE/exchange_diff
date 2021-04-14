using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.CompliancePolicy.Monitor;

namespace Microsoft.Exchange.Servicelets.CommonCode
{
	public sealed class ExPerfCounterProvider : PerfCounterProvider
	{
		public ExPerfCounterProvider(string categoryName, IEnumerable<ExPerformanceCounter> perfCounters) : base(categoryName)
		{
			if (perfCounters == null || !perfCounters.Any<ExPerformanceCounter>())
			{
				throw new ArgumentException("The perf counter collection is set to null or empty", "perfCounters");
			}
			this.perfCounters = perfCounters.ToDictionary((ExPerformanceCounter p) => p.CounterName);
		}

		public override void Increment(string counterName)
		{
			ExPerformanceCounter exPerformanceCounter = this.perfCounters[counterName];
			exPerformanceCounter.Increment();
		}

		public override void Increment(string counterName, string baseCounterName)
		{
			ExPerformanceCounter exPerformanceCounter = this.perfCounters[counterName];
			ExPerformanceCounter exPerformanceCounter2 = this.perfCounters[baseCounterName];
			exPerformanceCounter.Increment();
			exPerformanceCounter2.Increment();
		}

		public override void IncrementBy(string counterName, long incrementValue)
		{
			ExPerformanceCounter exPerformanceCounter = this.perfCounters[counterName];
			exPerformanceCounter.IncrementBy(incrementValue);
		}

		public override void IncrementBy(string counterName, long incrementValue, string baseCounterName)
		{
			ExPerformanceCounter exPerformanceCounter = this.perfCounters[counterName];
			ExPerformanceCounter exPerformanceCounter2 = this.perfCounters[baseCounterName];
			exPerformanceCounter.IncrementBy(incrementValue);
			exPerformanceCounter2.Increment();
		}

		private readonly Dictionary<string, ExPerformanceCounter> perfCounters;
	}
}
