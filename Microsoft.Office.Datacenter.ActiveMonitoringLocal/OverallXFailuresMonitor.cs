using System;
using System.Configuration;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class OverallXFailuresMonitor : OverallPercentSuccessMonitor
	{
		public static MonitorDefinition CreateDefinition(string name, string sampleMask, string serviceName, Component component, int monitoringInterval, int recurrenceInterval, int numberOfFailures, bool enabled = true)
		{
			return new MonitorDefinition
			{
				AssemblyPath = OverallXFailuresMonitor.AssemblyPath,
				Component = component,
				Enabled = enabled,
				TypeName = OverallXFailuresMonitor.TypeName,
				MaxRetryAttempts = 0,
				MonitoringIntervalSeconds = monitoringInterval,
				MonitoringThreshold = (double)numberOfFailures,
				Name = name,
				RecurrenceIntervalSeconds = recurrenceInterval,
				SampleMask = sampleMask,
				ServiceName = serviceName,
				TargetResource = serviceName,
				TimeoutSeconds = Math.Max(recurrenceInterval / 2, 30),
				InsufficientSamplesIntervalSeconds = Math.Max(5 * recurrenceInterval, OverallXFailuresMonitor.DefaultInsufficientSamplesIntervalSeconds)
			};
		}

		protected override void DoMonitorWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "OverallXFailuresMonitor: Calling into OverallPercentSuccessMonitor to get probe result counts.", null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Monitors\\OverallXFailuresMonitor.cs", 96);
			double minimumSampleCount = 0.0;
			if (base.Definition.Attributes.ContainsKey("MinimumSampleCount"))
			{
				minimumSampleCount = double.Parse(base.Definition.Attributes["MinimumSampleCount"]);
			}
			this.SetPercentSuccessNumbers(cancellationToken).ContinueWith(delegate(Task t)
			{
				this.HandleInsufficientSamples(() => (double)this.Result.TotalSampleCount < minimumSampleCount, cancellationToken);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled, TaskScheduler.Current).ContinueWith(delegate(Task t)
			{
				if (!base.Result.IsAlert)
				{
					if ((double)base.Result.TotalFailedCount >= base.Definition.MonitoringThreshold)
					{
						base.Result.IsAlert = true;
						this.OnAlert();
					}
					else
					{
						base.Result.IsAlert = false;
					}
				}
				WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "OverallXFailuresMonitor: Finished analyzing probe results.", null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Monitors\\OverallXFailuresMonitor.cs", 134);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled, TaskScheduler.Current);
		}

		protected virtual void OnAlert()
		{
		}

		private const string MinimumSampleCount = "MinimumSampleCount";

		private const int DefaultTimeoutSeconds = 30;

		private static readonly int DefaultInsufficientSamplesIntervalSeconds = Math.Max(Convert.ToInt32(ConfigurationManager.AppSettings["InsufficientSamplesIntervalInSeconds"]), (int)TimeSpan.FromHours(8.0).TotalSeconds);

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(OverallXFailuresMonitor).FullName;
	}
}
