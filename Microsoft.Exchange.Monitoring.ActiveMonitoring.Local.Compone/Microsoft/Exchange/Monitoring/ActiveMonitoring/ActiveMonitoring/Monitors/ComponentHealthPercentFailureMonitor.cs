using System;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors
{
	public class ComponentHealthPercentFailureMonitor : ComponentHealthMonitor
	{
		public static MonitorDefinition CreateDefinition(Component component, int recurrenceIntervalSeconds, int timeoutSeconds, int maxRetryAttempts, int monitoringIntervalSeconds, double monitoringThreshold)
		{
			if (0.0 > monitoringThreshold || monitoringThreshold > 99.0)
			{
				throw new ArgumentOutOfRangeException("monitoringThreshold", "Must be between 0 and 99");
			}
			return new MonitorDefinition
			{
				AssemblyPath = ComponentHealthMonitor.AssemblyPath,
				TypeName = typeof(ComponentHealthPercentFailureMonitor).FullName,
				Name = string.Format("{0}: {1}", typeof(ComponentHealthPercentFailureMonitor).Name, component.ToString()),
				RecurrenceIntervalSeconds = recurrenceIntervalSeconds,
				TimeoutSeconds = recurrenceIntervalSeconds / 2,
				MaxRetryAttempts = maxRetryAttempts,
				MonitoringIntervalSeconds = monitoringIntervalSeconds,
				TargetResource = Environment.MachineName,
				Component = component,
				MonitoringThreshold = monitoringThreshold,
				SampleMask = "*"
			};
		}

		protected override void AnalyzeComponentMonitorResults(int totalResults, int failedResults, string errorContent)
		{
			WTFDiagnostics.TraceFunction<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "{0}: Starting analyzing component results...", base.Definition.Name, null, "AnalyzeComponentMonitorResults", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Monitors\\ComponentHealthPercentFailureMonitor.cs", 89);
			int num = (totalResults > 0) ? (failedResults * 100 / totalResults) : 0;
			if ((double)num > base.Definition.MonitoringThreshold)
			{
				base.Result.IsAlert = true;
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "{0}: Result.IsAlert set to true.", base.Definition.Name, null, "AnalyzeComponentMonitorResults", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Monitors\\ComponentHealthPercentFailureMonitor.cs", 102);
			}
			else
			{
				base.Result.IsAlert = false;
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "{0}: Result.IsAlert set to false.", base.Definition.Name, null, "AnalyzeComponentMonitorResults", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Monitors\\ComponentHealthPercentFailureMonitor.cs", 112);
			}
			WTFDiagnostics.TraceFunction<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "{0}: Finished analyzing component results.", base.Definition.Name, null, "AnalyzeComponentMonitorResults", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Monitors\\ComponentHealthPercentFailureMonitor.cs", 119);
		}
	}
}
