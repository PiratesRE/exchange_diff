using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics.Components.WorkloadManagement;
using Microsoft.Exchange.WorkloadManagement.EventLogs;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal class WorkloadManagementPerfCounterWrapper
	{
		public WorkloadManagementPerfCounterWrapper()
		{
			string text = null;
			try
			{
				text = ResourceLoadPerfCounterWrapper.GetDefaultInstanceName();
				this.perfCounters = MSExchangeWorkloadManagement.GetInstance(text);
				ExTraceGlobals.CommonTracer.TraceDebug<string>((long)this.GetHashCode(), "[WorkloadManagementPerfCounterWrapper.ctor] Creating perf counter wrapper instance for '{0}'", text);
			}
			catch (Exception ex)
			{
				ExTraceGlobals.CommonTracer.TraceError<string, Exception>((long)this.GetHashCode(), "[WorkloadManagementPerfCounterWrapper.ctor] Failed to create perf counter instance '{0}'.  Exception: {1}", text ?? "<NULL>", ex);
				WorkloadManagerEventLogger.LogEvent(WorkloadManagementEventLogConstants.Tuple_WorkloadManagementPerformanceCounterInitializationFailure, text, new object[]
				{
					ex
				});
				this.perfCounters = null;
			}
		}

		public void UpdateWorkloadCount(long workloadCount)
		{
			if (this.perfCounters != null)
			{
				this.perfCounters.WorkloadCount.RawValue = workloadCount;
			}
		}

		public void UpdateActiveClassifications(long activeClassifications)
		{
			if (this.perfCounters != null)
			{
				this.perfCounters.ActiveClassifications.RawValue = activeClassifications;
			}
		}

		private MSExchangeWorkloadManagementInstance perfCounters;
	}
}
