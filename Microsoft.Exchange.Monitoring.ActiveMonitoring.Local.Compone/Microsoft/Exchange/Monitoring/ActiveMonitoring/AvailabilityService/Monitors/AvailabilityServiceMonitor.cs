using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.AvailabilityService.Monitors
{
	public class AvailabilityServiceMonitor : MonitorWorkItem
	{
		protected int Threshold
		{
			get
			{
				return (int)base.Definition.MonitoringThreshold;
			}
		}

		protected override void DoMonitorWork(CancellationToken cancellationToken)
		{
			base.GetLastFailedProbeResultId(base.Definition.SampleMask, cancellationToken);
			IDataAccessQuery<AvailabilityServiceMonitor.ErrorCodesPair> errorCodeQuery = this.GetErrorCodeQuery();
			errorCodeQuery.ExecuteAsync(delegate(AvailabilityServiceMonitor.ErrorCodesPair result)
			{
				if (result != null)
				{
					this.ErrorCodes.AppendLine(string.Format("[{0}] ErrorCode:{1} ProbeId:{2}", result.ExecutionTime, result.ErrorCode, result.ExecutionId));
				}
			}, cancellationToken, base.TraceContext);
			IDataAccessQuery<AvailabilityServiceMonitor.TargetServerPair> targetServerQuery = this.GetTargetServerQuery();
			targetServerQuery.ExecuteAsync(delegate(AvailabilityServiceMonitor.TargetServerPair result)
			{
				if (result != null)
				{
					base.Result.StateAttribute1 = result.Server;
					base.Result.StateAttribute2 = this.ErrorCodes.ToString();
					base.Result.StateAttribute6 = (double)result.Count;
					base.Result.IsAlert = true;
				}
			}, cancellationToken, base.TraceContext);
		}

		protected virtual IDataAccessQuery<AvailabilityServiceMonitor.ErrorCodesPair> GetErrorCodeQuery()
		{
			IEnumerable<AvailabilityServiceMonitor.ErrorCodesPair> query = (from r in base.Broker.GetProbeResults(base.Definition.SampleMask, base.MonitoringWindowStartTime, base.Result.ExecutionStartTime)
			where r.ResultType == ResultType.Failed
			select new AvailabilityServiceMonitor.ErrorCodesPair
			{
				ExecutionTime = r.ExecutionStartTime,
				ErrorCode = r.StateAttribute2,
				ExecutionId = r.ExecutionId
			}).Take(this.Threshold);
			return base.Broker.AsDataAccessQuery<AvailabilityServiceMonitor.ErrorCodesPair>(query);
		}

		protected virtual IDataAccessQuery<AvailabilityServiceMonitor.TargetServerPair> GetTargetServerQuery()
		{
			IEnumerable<AvailabilityServiceMonitor.TargetServerPair> query = from r in base.Broker.GetProbeResults(base.Definition.SampleMask, base.MonitoringWindowStartTime, base.Result.ExecutionStartTime)
			where r.ResultType == ResultType.Failed
			group r by r.StateAttribute14 into r
			select new AvailabilityServiceMonitor.TargetServerPair
			{
				Server = r.Key,
				Count = r.Count<ProbeResult>()
			} into r
			where r.Count >= this.Threshold
			select r;
			return base.Broker.AsDataAccessQuery<AvailabilityServiceMonitor.TargetServerPair>(query);
		}

		protected StringBuilder ErrorCodes = new StringBuilder();

		protected class ErrorCodesPair
		{
			internal DateTime ExecutionTime { get; set; }

			internal string ErrorCode { get; set; }

			internal int ExecutionId { get; set; }
		}

		protected class TargetServerPair
		{
			internal string Server { get; set; }

			internal int Count { get; set; }
		}
	}
}
