using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Monitors
{
	public class SmtpProbeMonitor : MonitorWorkItem
	{
		protected CancellationToken CancellationToken { get; set; }

		protected virtual int NewFailedCount
		{
			get
			{
				return base.Result.NewFailedCount;
			}
		}

		protected virtual int TotalFailedCount
		{
			get
			{
				return base.Result.TotalFailedCount;
			}
		}

		protected override void DoMonitorWork(CancellationToken cancellationToken)
		{
			this.CancellationToken = cancellationToken;
			DateTime windowEndTime = base.Result.ExecutionStartTime;
			DateTime startTime = base.MonitoringWindowStartTime;
			if (base.LastSuccessfulResult != null && base.LastSuccessfulResult.ExecutionStartTime > base.MonitoringWindowStartTime)
			{
				startTime = base.LastSuccessfulResult.ExecutionStartTime;
			}
			IEnumerable<MonitorWorkItem.ProbeResultsGroup> query = from p in base.Broker.GetProbeResults(base.Definition.SampleMask, startTime, windowEndTime)
			where (p.ResultType == ResultType.Succeeded && p.StateAttribute12 == RecordType.DeliverMail.ToString()) || p.ResultType == ResultType.Failed || p.ResultType == ResultType.TimedOut
			group p by p.ResultType into g
			select new MonitorWorkItem.ProbeResultsGroup
			{
				ResultType = g.Key,
				Count = g.Count<ProbeResult>()
			};
			Task<int> task = base.Broker.AsDataAccessQuery<MonitorWorkItem.ProbeResultsGroup>(query).ExecuteAsync(delegate(MonitorWorkItem.ProbeResultsGroup group)
			{
				base.Result.NewSampleCount += group.Count;
				if (group.ResultType != ResultType.Succeeded)
				{
					base.Result.NewFailedCount += group.Count;
				}
			}, cancellationToken, base.TraceContext);
			Task<int> task2 = task.Continue(delegate(int i)
			{
				IOrderedEnumerable<ProbeResult> query2 = from p in this.Broker.GetProbeResults(this.Definition.SampleMask, this.MonitoringWindowStartTime, windowEndTime)
				where (p.ResultType == ResultType.Succeeded && p.StateAttribute12 == RecordType.DeliverMail.ToString()) || p.ResultType == ResultType.Failed || p.ResultType == ResultType.TimedOut
				orderby p.ExecutionEndTime
				select p;
				return this.Broker.AsDataAccessQuery<ProbeResult>(query2).ExecuteAsync(delegate(ProbeResult probeResult)
				{
					this.Result.TotalSampleCount++;
					if (probeResult.ResultType != ResultType.Succeeded)
					{
						this.Result.TotalFailedCount++;
						this.Result.LastFailedProbeId = probeResult.WorkItemId;
						this.Result.LastFailedProbeResultId = probeResult.ResultId;
					}
				}, cancellationToken, this.TraceContext);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
			task2.Continue(delegate(int i)
			{
				base.Result.NewValue = 100.0;
				if (base.Result.NewSampleCount != 0)
				{
					base.Result.NewValue = (double)((base.Result.NewSampleCount - base.Result.NewFailedCount) * 100 / base.Result.NewSampleCount);
					base.Result.NewValue = (double)((base.Result.NewSampleCount - this.NewFailedCount) * 100 / base.Result.NewSampleCount);
				}
				base.Result.TotalValue = 100.0;
				if (base.Result.TotalSampleCount != 0)
				{
					base.Result.TotalValue = (double)((base.Result.TotalSampleCount - base.Result.TotalFailedCount) * 100 / base.Result.TotalSampleCount);
					base.Result.TotalValue = (double)((base.Result.TotalSampleCount - this.TotalFailedCount) * 100 / base.Result.TotalSampleCount);
				}
				base.Result.IsAlert = (base.Result.TotalValue < base.Definition.MonitoringThreshold);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}
	}
}
