using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	public static class WorkItemResultHelper
	{
		internal static ProbeResult GetLastFailedProbeResult(ResponderWorkItem responder, IResponderWorkBroker broker, CancellationToken cancellationToken)
		{
			if (broker == null)
			{
				throw new ArgumentNullException("broker");
			}
			ProbeResult result = null;
			WTFDiagnostics.TraceInformation<ResponderDefinition>(ExTraceGlobals.CommonComponentsTracer, WorkItemResultHelper.traceContext, "Trying to get last failed probe result corresponding to responder: '{0}'.", responder.Definition, null, "GetLastFailedProbeResult", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\WorkItemResultHelper.cs", 45);
			MonitorResult lastFailedMonitorResult = WorkItemResultHelper.GetLastFailedMonitorResult(responder, broker, cancellationToken);
			if (lastFailedMonitorResult != null)
			{
				result = WorkItemResultHelper.GetLastFailedProbeResult(lastFailedMonitorResult, broker, cancellationToken);
			}
			return result;
		}

		internal static ProbeResult GetLastFailedProbeResult(MonitorResult monitorResult, IResponderWorkBroker broker, CancellationToken cancellationToken)
		{
			if (broker == null)
			{
				throw new ArgumentNullException("broker");
			}
			ProbeResult result = null;
			if (monitorResult != null)
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, WorkItemResultHelper.traceContext, "Trying to get last failed probe result corresponding to monitor result: '{0}'.", monitorResult.ResultName, null, "GetLastFailedProbeResult", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\WorkItemResultHelper.cs", 88);
				IDataAccessQuery<ProbeResult> probeResult = broker.GetProbeResult(monitorResult.LastFailedProbeId, monitorResult.LastFailedProbeResultId);
				Task<ProbeResult> task = probeResult.ExecuteAsync(cancellationToken, WorkItemResultHelper.traceContext);
				task.Continue(delegate(ProbeResult lastProbeResult)
				{
					result = lastProbeResult;
				}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled).Wait();
			}
			return result;
		}

		internal static MonitorResult GetLastFailedMonitorResult(ResponderWorkItem responder, IResponderWorkBroker broker, CancellationToken cancellationToken)
		{
			if (broker == null)
			{
				throw new ArgumentNullException("broker");
			}
			MonitorResult result = null;
			WTFDiagnostics.TraceInformation<ResponderDefinition>(ExTraceGlobals.CommonComponentsTracer, WorkItemResultHelper.traceContext, "Trying to get last failed monitor result corresponding to responder: '{0}'.", responder.Definition, null, "GetLastFailedMonitorResult", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\WorkItemResultHelper.cs", 126);
			IDataAccessQuery<ResponderResult> lastSuccessfulResponderResult = broker.GetLastSuccessfulResponderResult(responder.Definition);
			Task<ResponderResult> task = lastSuccessfulResponderResult.ExecuteAsync(cancellationToken, WorkItemResultHelper.traceContext);
			task.Continue(delegate(ResponderResult lastResponderResult)
			{
				DateTime startTime = DateTime.MinValue;
				if (lastResponderResult != null)
				{
					startTime = lastResponderResult.ExecutionStartTime;
				}
				IDataAccessQuery<MonitorResult> lastSuccessfulMonitorResult = broker.GetLastSuccessfulMonitorResult(responder.Definition.AlertMask, startTime, responder.Result.ExecutionStartTime);
				Task<MonitorResult> task2 = lastSuccessfulMonitorResult.ExecuteAsync(cancellationToken, WorkItemResultHelper.traceContext);
				task2.Continue(delegate(MonitorResult monitorResult)
				{
					result = monitorResult;
				}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled).Wait();
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled).Wait();
			return result;
		}

		internal static ProbeResult GetProbeResultById(int probeId, int resultId, IResponderWorkBroker broker, CancellationToken cancellationToken)
		{
			ProbeResult probeResult = null;
			broker.GetProbeResult(probeId, resultId).ExecuteAsync(delegate(ProbeResult result)
			{
				probeResult = result;
			}, cancellationToken, WorkItemResultHelper.traceContext);
			return probeResult;
		}

		internal static ProbeResult GetLastProbeResult(string sampleMask, IMonitorWorkBroker broker, DateTime windowStartTime, DateTime windowEndTime, CancellationToken cancellationToken)
		{
			ProbeResult probeResult = null;
			IOrderedEnumerable<ProbeResult> query = from r in broker.GetProbeResults(sampleMask, windowStartTime, windowEndTime)
			orderby r.ExecutionEndTime
			select r;
			broker.AsDataAccessQuery<ProbeResult>(query).ExecuteAsync(delegate(ProbeResult result)
			{
				probeResult = result;
			}, cancellationToken, WorkItemResultHelper.traceContext);
			return probeResult;
		}

		internal static List<ProbeResult> GetAllProbeResults(IMonitorWorkBroker broker, DateTime executionStartTime, string sampleMask, DateTime monitoringWindowStartTime, CancellationToken cancellationToken)
		{
			List<ProbeResult> results = new List<ProbeResult>();
			IDataAccessQuery<ProbeResult> probeResults = broker.GetProbeResults(sampleMask, monitoringWindowStartTime, executionStartTime);
			probeResults.ExecuteAsync(delegate(ProbeResult r)
			{
				results.Add(r);
			}, cancellationToken, WorkItemResultHelper.traceContext);
			return results;
		}

		internal static List<ProbeResult> GetAllProbeResults(IProbeWorkBroker broker, DateTime executionStartTime, string sampleMask, DateTime monitoringWindowStartTime, CancellationToken cancellationToken)
		{
			List<ProbeResult> results = new List<ProbeResult>();
			IDataAccessQuery<ProbeResult> probeResults = broker.GetProbeResults(sampleMask, monitoringWindowStartTime, executionStartTime);
			probeResults.ExecuteAsync(delegate(ProbeResult r)
			{
				results.Add(r);
			}, cancellationToken, WorkItemResultHelper.traceContext);
			return results;
		}

		private static readonly TracingContext traceContext = TracingContext.Default;
	}
}
