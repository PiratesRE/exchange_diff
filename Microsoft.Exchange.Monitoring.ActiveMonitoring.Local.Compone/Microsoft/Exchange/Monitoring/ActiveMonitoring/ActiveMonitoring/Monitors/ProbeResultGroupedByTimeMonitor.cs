using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors
{
	public class ProbeResultGroupedByTimeMonitor : MonitorWorkItem
	{
		public static MonitorDefinition CreateDefinition(string name, string sampleMask, string serviceName, Component component, int failureCount, TimeSpan recurrenceInterval, TimeSpan monitoringInterval, TimeSpan probeResultSpread, bool enabled = true)
		{
			return new MonitorDefinition
			{
				AssemblyPath = ProbeResultGroupedByTimeMonitor.AssemblyPath,
				TypeName = ProbeResultGroupedByTimeMonitor.TypeName,
				Name = name,
				SampleMask = sampleMask,
				ServiceName = serviceName,
				Component = component,
				MaxRetryAttempts = 1,
				Enabled = enabled,
				MonitoringThreshold = (double)failureCount,
				MonitoringIntervalSeconds = (int)monitoringInterval.TotalSeconds,
				RecurrenceIntervalSeconds = (int)recurrenceInterval.TotalSeconds,
				SecondaryMonitoringThreshold = (double)((int)probeResultSpread.TotalSeconds),
				TimeoutSeconds = (int)ProbeResultGroupedByTimeMonitor.DefaultTimeout.TotalSeconds
			};
		}

		protected override void DoMonitorWork(CancellationToken cancellationToken)
		{
			int num = 0;
			TimeSpan t = ProbeResultGroupedByTimeMonitor.DefaultSpread;
			WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Starting ProbeResultGroupedByTimeMonitor:DoMonitorWork", null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Monitors\\ProbeResultGroupedByTimeMonitor.cs", 93);
			List<ProbeResult> allProbeResults = WorkItemResultHelper.GetAllProbeResults(base.Broker, base.Result.ExecutionStartTime, base.Definition.SampleMask, base.MonitoringWindowStartTime, cancellationToken);
			if ((double)allProbeResults.Count >= base.Definition.MonitoringThreshold)
			{
				if (base.Definition.SecondaryMonitoringThreshold > 0.0)
				{
					t = TimeSpan.FromSeconds(base.Definition.SecondaryMonitoringThreshold);
				}
				for (int i = 1; i <= allProbeResults.Count; i++)
				{
					if (i == allProbeResults.Count)
					{
						num++;
						break;
					}
					if (allProbeResults[i - 1].ExecutionEndTime.Subtract(allProbeResults[i].ExecutionEndTime) > t)
					{
						num++;
					}
				}
				base.Result.IsAlert = ((double)num >= base.Definition.MonitoringThreshold);
				base.Result.StateAttribute6 = (double)num;
				return;
			}
			base.Result.IsAlert = false;
			base.Result.StateAttribute6 = (double)allProbeResults.Count;
		}

		private static TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30.0);

		private static TimeSpan DefaultSpread = TimeSpan.FromMinutes(1.0);

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(ProbeResultGroupedByTimeMonitor).FullName;
	}
}
