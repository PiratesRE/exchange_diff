using System;
using System.Configuration;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class OverallConsecutiveSampleValueAboveThresholdMonitor : OverallConsecutiveFailuresMonitor
	{
		public static MonitorDefinition CreateDefinition(string name, string sampleMask, string serviceName, Component component, double threshold, int numberOfSamples, bool enabled = true)
		{
			MonitorDefinition monitorDefinition = new MonitorDefinition();
			monitorDefinition.AssemblyPath = OverallConsecutiveSampleValueAboveThresholdMonitor.AssemblyPath;
			monitorDefinition.TypeName = OverallConsecutiveSampleValueAboveThresholdMonitor.TypeName;
			monitorDefinition.Name = name;
			monitorDefinition.SampleMask = sampleMask;
			monitorDefinition.ServiceName = serviceName;
			monitorDefinition.MaxRetryAttempts = 0;
			monitorDefinition.Enabled = enabled;
			monitorDefinition.TimeoutSeconds = 30;
			monitorDefinition.Component = component;
			monitorDefinition.MonitoringThreshold = threshold;
			monitorDefinition.SecondaryMonitoringThreshold = (double)numberOfSamples;
			monitorDefinition.MonitoringIntervalSeconds = (numberOfSamples + 1) * 300;
			monitorDefinition.RecurrenceIntervalSeconds = monitorDefinition.MonitoringIntervalSeconds / 2;
			monitorDefinition.InsufficientSamplesIntervalSeconds = Math.Max(5 * monitorDefinition.RecurrenceIntervalSeconds, OverallConsecutiveSampleValueAboveThresholdMonitor.DefaultInsufficientSamplesIntervalSeconds);
			return monitorDefinition;
		}

		protected override bool ShouldAlert()
		{
			return base.Result.TotalValue >= base.Definition.SecondaryMonitoringThreshold;
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
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "OverallConsecutiveSampleValueAboveThresholdMonitor.SetConsecutiveFailureNumbers: Getting overall consecutive samples of: {0}.", base.Definition.SampleMask, null, "SetConsecutiveFailureNumbers", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Monitors\\OverallConsecutiveSampleValueAboveThresholdMonitor.cs", 113);
			Task consecutiveSampleValueAboveThresholdCounts = base.GetConsecutiveSampleValueAboveThresholdCounts(base.Definition.SampleMask, (double)((int)base.Definition.MonitoringThreshold), (int)base.Definition.SecondaryMonitoringThreshold, delegate(int newValue)
			{
				base.Result.NewValue = (double)newValue;
			}, delegate(int totalValue)
			{
				base.Result.TotalValue = (double)totalValue;
			}, cancellationToken);
			this.SetStateAttribute6ForScopeMonitoring(base.Result.NewValue);
			return consecutiveSampleValueAboveThresholdCounts;
		}

		private const string MinimumSampleCount = "MinimumSampleCount";

		private static readonly int DefaultInsufficientSamplesIntervalSeconds = Math.Max(Convert.ToInt32(ConfigurationManager.AppSettings["InsufficientSamplesIntervalInSeconds"]), (int)TimeSpan.FromHours(8.0).TotalSeconds);

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(OverallConsecutiveSampleValueAboveThresholdMonitor).FullName;
	}
}
