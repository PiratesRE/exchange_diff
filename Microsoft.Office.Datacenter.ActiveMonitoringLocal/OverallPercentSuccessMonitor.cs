using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class OverallPercentSuccessMonitor : MonitorWorkItem
	{
		public static MonitorDefinition CreateDefinition(string name, string sampleMask, string serviceName, Component component, double availabilityPercentage, TimeSpan monitoringInterval, bool enabled = true)
		{
			return OverallPercentSuccessMonitor.CreateDefinition(name, sampleMask, serviceName, component, availabilityPercentage, monitoringInterval, 0, enabled);
		}

		public static MonitorDefinition CreateDefinition(string name, string sampleMask, string serviceName, Component component, double availabilityPercentage, TimeSpan monitoringInterval, int minimumErrorCount, bool enabled = true)
		{
			MonitorDefinition monitorDefinition = new MonitorDefinition();
			monitorDefinition.AssemblyPath = OverallPercentSuccessMonitor.AssemblyPath;
			monitorDefinition.TypeName = OverallPercentSuccessMonitor.TypeName;
			monitorDefinition.Name = name;
			monitorDefinition.SampleMask = sampleMask;
			monitorDefinition.ServiceName = serviceName;
			monitorDefinition.Component = component;
			monitorDefinition.MaxRetryAttempts = 0;
			monitorDefinition.Enabled = enabled;
			monitorDefinition.TimeoutSeconds = 30;
			monitorDefinition.MonitoringThreshold = availabilityPercentage;
			monitorDefinition.MonitoringIntervalSeconds = (int)monitoringInterval.TotalSeconds;
			monitorDefinition.RecurrenceIntervalSeconds = monitorDefinition.MonitoringIntervalSeconds / 2;
			monitorDefinition.MinimumErrorCount = minimumErrorCount;
			monitorDefinition.InsufficientSamplesIntervalSeconds = 5 * monitorDefinition.RecurrenceIntervalSeconds;
			return monitorDefinition;
		}

		protected virtual Task SetPercentSuccessNumbers(CancellationToken cancellationToken)
		{
			base.GetLastFailedProbeResultId(base.Definition.SampleMask, cancellationToken);
			Task<Dictionary<ResultType, int>> resultTypeCountsForNewProbeResults = base.GetResultTypeCountsForNewProbeResults(base.Definition.SampleMask, cancellationToken);
			resultTypeCountsForNewProbeResults.Continue(delegate(Dictionary<ResultType, int> resultTypeCounts)
			{
				int newSampleCount;
				int newFailedCount;
				double newValue;
				this.GetResultStatistics(resultTypeCounts, out newSampleCount, out newFailedCount, out newValue);
				base.Result.NewSampleCount = newSampleCount;
				base.Result.NewFailedCount = newFailedCount;
				base.Result.NewValue = newValue;
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
			Task<Dictionary<ResultType, int>> resultTypeCountsForAllProbeResults = base.GetResultTypeCountsForAllProbeResults(base.Definition.SampleMask, cancellationToken);
			return resultTypeCountsForAllProbeResults.Continue(delegate(Dictionary<ResultType, int> resultTypeCounts)
			{
				int totalSampleCount;
				int totalFailedCount;
				double totalValue;
				this.GetResultStatistics(resultTypeCounts, out totalSampleCount, out totalFailedCount, out totalValue);
				base.Result.TotalSampleCount = totalSampleCount;
				base.Result.TotalFailedCount = totalFailedCount;
				base.Result.TotalValue = totalValue;
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		protected override void DoMonitorWork(CancellationToken cancellationToken)
		{
			this.SetPercentSuccessNumbers(cancellationToken).ContinueWith(delegate(Task t)
			{
				double minimumSampleCount = 0.0;
				if (this.Definition.Attributes.ContainsKey("MinimumSampleCount"))
				{
					minimumSampleCount = double.Parse(this.Definition.Attributes["MinimumSampleCount"]);
				}
				this.HandleInsufficientSamples(() => (double)this.Result.TotalSampleCount < minimumSampleCount, cancellationToken);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled, TaskScheduler.Current).ContinueWith(delegate(Task t)
			{
				if (!base.Result.IsAlert)
				{
					if ((base.Result.TotalValue < base.Definition.MonitoringThreshold || (base.Result.TotalValue == 0.0 && base.Definition.MonitoringThreshold == 0.0)) && (base.Definition.MinimumErrorCount == 0 || base.Result.TotalFailedCount >= base.Definition.MinimumErrorCount))
					{
						base.Result.IsAlert = true;
						return;
					}
					base.Result.IsAlert = false;
				}
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled, TaskScheduler.Current);
		}

		protected virtual void GetResultStatistics(Dictionary<ResultType, int> resultTypeCounts, out int totalCount, out int totalFailedCount, out double percentSuccess)
		{
			resultTypeCounts.TryGetValue(ResultType.Failed, out totalFailedCount);
			int num;
			resultTypeCounts.TryGetValue(ResultType.Succeeded, out num);
			totalCount = num + totalFailedCount;
			percentSuccess = 100.0;
			if (totalCount != 0)
			{
				percentSuccess = (double)((totalCount - totalFailedCount) * 100 / totalCount);
			}
		}

		private const string MinimumSampleCount = "MinimumSampleCount";

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(OverallPercentSuccessMonitor).FullName;
	}
}
