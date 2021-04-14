using System;
using System.Configuration;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class OverallConsecutiveProbeFailuresMonitor : OverallConsecutiveFailuresMonitor
	{
		public static MonitorDefinition CreateDefinition(string name, string sampleMask, string serviceName, Component component, int failureCount, bool enabled = true, int monitoringInterval = 300)
		{
			MonitorDefinition monitorDefinition = new MonitorDefinition();
			monitorDefinition.AssemblyPath = OverallConsecutiveProbeFailuresMonitor.AssemblyPath;
			monitorDefinition.TypeName = OverallConsecutiveProbeFailuresMonitor.TypeName;
			monitorDefinition.Name = name;
			monitorDefinition.SampleMask = sampleMask;
			monitorDefinition.ServiceName = serviceName;
			monitorDefinition.Component = component;
			monitorDefinition.MaxRetryAttempts = 0;
			monitorDefinition.Enabled = enabled;
			monitorDefinition.TimeoutSeconds = 30;
			monitorDefinition.MonitoringThreshold = (double)failureCount;
			monitorDefinition.MonitoringIntervalSeconds = monitoringInterval;
			monitorDefinition.RecurrenceIntervalSeconds = monitorDefinition.MonitoringIntervalSeconds / 2;
			monitorDefinition.InsufficientSamplesIntervalSeconds = Math.Max(5 * monitorDefinition.RecurrenceIntervalSeconds, OverallConsecutiveProbeFailuresMonitor.DefaultInsufficientSamplesIntervalSeconds);
			return monitorDefinition;
		}

		protected override bool ShouldAlert()
		{
			return base.Result.TotalValue >= base.Definition.MonitoringThreshold;
		}

		protected override bool HaveInsufficientSamples()
		{
			double num = 0.0;
			if (base.Definition.Attributes.ContainsKey("MinimumSampleCount"))
			{
				num = double.Parse(base.Definition.Attributes["MinimumSampleCount"]);
			}
			return (double)base.Result.TotalSampleCount < num;
		}

		protected override Task SetConsecutiveFailureNumbers(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "OverallConsecutiveProbeFailuresMonitor.SetConsecutiveProbeFailureNumbers: Getting overall consecutive failures of: {0}.", base.Definition.SampleMask, null, "SetConsecutiveFailureNumbers", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Monitors\\OverallConsecutiveProbeFailuresMonitor.cs", 114);
			base.GetLastFailedProbeResultId(base.Definition.SampleMask, cancellationToken);
			return base.GetConsecutiveProbeFailureInformation(base.Definition.SampleMask, (int)base.Definition.MonitoringThreshold, delegate(int newValue)
			{
				base.Result.NewValue = (double)newValue;
			}, delegate(int totalValue)
			{
				base.Result.TotalValue = (double)totalValue;
			}, cancellationToken);
		}

		private const string MinimumSampleCount = "MinimumSampleCount";

		private static readonly int DefaultInsufficientSamplesIntervalSeconds = Math.Max(Convert.ToInt32(ConfigurationManager.AppSettings["InsufficientSamplesIntervalInSeconds"]), (int)TimeSpan.FromHours(8.0).TotalSeconds);

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(OverallConsecutiveProbeFailuresMonitor).FullName;
	}
}
