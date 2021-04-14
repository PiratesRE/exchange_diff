using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics.Components.WorkloadManagement;
using Microsoft.Exchange.WorkloadManagement.EventLogs;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal class ClassificationPerfCounterWrapper
	{
		public ClassificationPerfCounterWrapper(WorkloadClassification classification)
		{
			this.Classification = classification;
			string text = null;
			try
			{
				text = ResourceLoadPerfCounterWrapper.GetDefaultInstanceName();
				text = text + "_" + classification;
				this.perfCounters = MSExchangeWorkloadManagementClassification.GetInstance(text);
				ExTraceGlobals.CommonTracer.TraceDebug<string>((long)this.GetHashCode(), "[ClassificationPerfCounterWrapper.ctor] Creating perf counter wrapper instance for '{0}'", text);
			}
			catch (Exception ex)
			{
				ExTraceGlobals.CommonTracer.TraceError<string, Exception>((long)this.GetHashCode(), "[ClassificationPerfCounterWrapper.ctor] Failed to create perf counter instance '{0}'.  Exception: {1}", text ?? "<NULL>", ex);
				WorkloadManagerEventLogger.LogEvent(WorkloadManagementEventLogConstants.Tuple_ClassificationPerformanceCounterInitializationFailure, classification.ToString(), new object[]
				{
					classification,
					ex
				});
				this.perfCounters = null;
			}
			this.UpdateActiveThreads(0L);
		}

		public WorkloadClassification Classification { get; private set; }

		public void UpdateWorkloadCount(long workloadCount)
		{
			if (this.perfCounters != null)
			{
				this.perfCounters.WorkloadCount.RawValue = workloadCount;
			}
		}

		public void UpdateActiveThreads(long activeThreadCount)
		{
			if (this.perfCounters != null)
			{
				this.perfCounters.ActiveThreadCount.RawValue = activeThreadCount;
			}
		}

		public void UpdateFairnessFactor(long fairnessFactor)
		{
			if (this.perfCounters != null)
			{
				this.perfCounters.FairnessFactor.RawValue = fairnessFactor;
			}
		}

		private MSExchangeWorkloadManagementClassificationInstance perfCounters;
	}
}
