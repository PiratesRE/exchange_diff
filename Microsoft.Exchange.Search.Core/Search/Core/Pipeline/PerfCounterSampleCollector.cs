using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Inference.Performance;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Search.Core.Pipeline
{
	internal sealed class PerfCounterSampleCollector
	{
		internal PerfCounterSampleCollector(PipelineCountersInstance instancePerfCounter, IDiagnosticsSession diagnosticsSession)
		{
			this.instancePerfCounter = instancePerfCounter;
			this.diagnosticsSession = diagnosticsSession;
		}

		internal void Start()
		{
			this.stopWatch = Stopwatch.StartNew();
			this.IncrementCounter(this.instancePerfCounter.NumberOfOutstandingDocuments);
		}

		internal void Stop(bool succeeded)
		{
			this.stopWatch.Stop();
			this.DecrementCounter(this.instancePerfCounter.NumberOfOutstandingDocuments);
			this.IncrementCounter(this.instancePerfCounter.NumberOfProcessedDocuments);
			this.IncrementCounterBy(this.instancePerfCounter.AverageDocumentProcessingTime, this.stopWatch.ElapsedTicks);
			this.IncrementCounter(this.instancePerfCounter.AverageDocumentProcessingTimeBase);
			if (succeeded)
			{
				this.IncrementCounter(this.instancePerfCounter.NumberOfSucceededDocuments);
				return;
			}
			this.IncrementCounter(this.instancePerfCounter.NumberOfFailedDocuments);
		}

		private void DecrementCounter(ExPerformanceCounter counter)
		{
			this.diagnosticsSession.DecrementCounter(counter);
		}

		private void IncrementCounter(ExPerformanceCounter counter)
		{
			this.diagnosticsSession.IncrementCounter(counter);
		}

		private void IncrementCounterBy(ExPerformanceCounter counter, long incrementValue)
		{
			this.diagnosticsSession.IncrementCounterBy(counter, incrementValue);
		}

		private readonly PipelineCountersInstance instancePerfCounter;

		private readonly IDiagnosticsSession diagnosticsSession;

		private Stopwatch stopWatch;
	}
}
