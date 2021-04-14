using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors
{
	public abstract class ComponentHealthMonitor : MonitorWorkItem
	{
		protected abstract void AnalyzeComponentMonitorResults(int totalResults, int failedResults, string errorContent);

		protected override void DoMonitorWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceFunction<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "{0}: Starting component health monitoring...", base.Definition.Name, null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Monitors\\ComponentHealthMonitor.cs", 57);
			DateTime monitoringWindowStartTime = base.MonitoringWindowStartTime;
			IOrderedEnumerable<MonitorResult> query = from r in base.Broker.GetSuccessfulMonitorResults(base.Definition.Component, monitoringWindowStartTime)
			where r.ExecutionStartTime < base.Result.ExecutionStartTime
			orderby r.ExecutionStartTime
			select r;
			int totalResults = 0;
			int failedResults = 0;
			StringBuilder errorContent = new StringBuilder();
			Task<int> task = base.Broker.AsDataAccessQuery<MonitorResult>(query).ExecuteAsync(delegate(MonitorResult result)
			{
				if (result.WorkItemId == this.Definition.Id || result.ResultName.StartsWith("ComponentHealth"))
				{
					return;
				}
				totalResults++;
				if (result.IsAlert)
				{
					failedResults++;
					errorContent.AppendLine(Strings.ComponentHealthErrorContent(result.ComponentName, result.ResultName, result.ExecutionEndTime));
				}
				WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, this.TraceContext, string.Format("[failedResults={0}/totalResults={1}] Processed monitor result for: ResultId={2}; ResultName='{3}'; WorkItemId={4}; ExecutionStartTime='{5}'; ExecutionEndTime='{6}'; ResultType='{7}'; ComponentName='{8}'; IsAlert={9}", new object[]
				{
					failedResults,
					totalResults,
					result.ResultId,
					result.ResultName,
					result.WorkItemId,
					result.ExecutionStartTime,
					result.ExecutionEndTime,
					result.ResultType.ToString(),
					result.ComponentName,
					result.IsAlert.ToString()
				}), null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Monitors\\ComponentHealthMonitor.cs", 99);
			}, cancellationToken, base.TraceContext);
			task.ContinueWith(delegate(Task<int> t)
			{
				string text = errorContent.ToString();
				WTFDiagnostics.TraceInformation<string, int, int, string>(ExTraceGlobals.CommonComponentsTracer, this.TraceContext, "{0}: Gathered monitor results for further analysis: totalResult={1}; failedResults={2}; errorContentString='{3}'", this.Definition.Name, totalResults, failedResults, text, null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Monitors\\ComponentHealthMonitor.cs", 132);
				if (!string.IsNullOrWhiteSpace(text))
				{
					this.Result.Error = string.Format("{0}{1}{2}", Strings.ComponentHealthErrorHeader(failedResults), Environment.NewLine, text);
				}
				this.AnalyzeComponentMonitorResults(totalResults, failedResults, text);
				WTFDiagnostics.TraceFunction<string>(ExTraceGlobals.CommonComponentsTracer, this.TraceContext, "{0}: Finished component health monitoring.", this.Definition.Name, null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Monitors\\ComponentHealthMonitor.cs", 157);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled, TaskScheduler.Default);
		}

		private const string ComponentHealthPrefix = "ComponentHealth";

		internal static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;
	}
}
