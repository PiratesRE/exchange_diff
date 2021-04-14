using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class OverallPercentSuccessHistoryByFailureCategoryMonitor : OverallPercentSuccessNoTimeoutMonitor
	{
		public static MonitorDefinition CreateDefinition(string name, string sampleMask, string serviceName, Component component, double availabilityPercentage, TimeSpan monitoringInterval, TimeSpan recurrenceInterval, TimeSpan secondaryMonitoringInterval, int failureCategoryMask = -1, bool enabled = true)
		{
			double monitoringSamplesPercentage = 100.0;
			return OverallPercentSuccessHistoryByFailureCategoryMonitor.CreateDefinition(name, sampleMask, serviceName, component, availabilityPercentage, monitoringSamplesPercentage, monitoringInterval, recurrenceInterval, secondaryMonitoringInterval, failureCategoryMask, enabled);
		}

		public static MonitorDefinition CreateDefinition(string name, string sampleMask, string serviceName, Component component, double availabilityPercentage, double monitoringSamplesPercentage, TimeSpan monitoringInterval, TimeSpan recurrenceInterval, TimeSpan secondaryMonitoringInterval, int failureCategoryMask = -1, bool enabled = true)
		{
			return new MonitorDefinition
			{
				AssemblyPath = OverallPercentSuccessHistoryByFailureCategoryMonitor.AssemblyPath,
				TypeName = OverallPercentSuccessHistoryByFailureCategoryMonitor.TypeName,
				Name = name,
				SampleMask = sampleMask,
				ServiceName = serviceName,
				Component = component,
				MaxRetryAttempts = 0,
				Enabled = enabled,
				TimeoutSeconds = 200,
				MonitoringThreshold = availabilityPercentage,
				MonitoringSamplesThreshold = monitoringSamplesPercentage,
				MonitoringIntervalSeconds = (int)monitoringInterval.TotalSeconds,
				RecurrenceIntervalSeconds = (int)recurrenceInterval.TotalSeconds,
				SecondaryMonitoringThreshold = (double)((int)secondaryMonitoringInterval.TotalSeconds),
				FailureCategoryMask = failureCategoryMask
			};
		}

		internal virtual Task SetProbeFailureCategoryNumbers(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "OverallPercentSuccessHistoryByFailureCategoryMonitor: Getting FailureCategory values of: {0}.", base.Definition.SampleMask, null, "SetProbeFailureCategoryNumbers", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Monitors\\OverallPercentSuccessHistoryByFailureCategoryMonitor.cs", 147);
			Task<Dictionary<int, int>> failureCategoryCountsForNewFailedProbeResults = base.GetFailureCategoryCountsForNewFailedProbeResults(base.Definition.SampleMask, cancellationToken);
			failureCategoryCountsForNewFailedProbeResults.Continue(delegate(Dictionary<int, int> failureCategoryCounts)
			{
				int newFailureCategoryValue;
				int newFailureCategoryCount;
				double newFailureCategoryPercent;
				this.GetProbeFailureCategoryStatistics(failureCategoryCounts, out newFailureCategoryValue, out newFailureCategoryCount, out newFailureCategoryPercent);
				base.Result.NewFailureCategoryValue = newFailureCategoryValue;
				base.Result.NewFailureCategoryCount = newFailureCategoryCount;
				base.Result.NewFailureCategoryPercent = newFailureCategoryPercent;
				WTFDiagnostics.TraceDebug<int>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "OverallPercentSuccessHistoryByFailureCategoryMonitor: Processed new result(s), max calculated attribute is {0}.", base.Result.NewFailureCategoryValue, null, "SetProbeFailureCategoryNumbers", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Monitors\\OverallPercentSuccessHistoryByFailureCategoryMonitor.cs", 168);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
			Task<Dictionary<int, int>> failureCategoryCountsForAllFailedProbeResults = base.GetFailureCategoryCountsForAllFailedProbeResults(base.Definition.SampleMask, cancellationToken);
			return failureCategoryCountsForAllFailedProbeResults.Continue(delegate(Dictionary<int, int> failureCategoryCounts)
			{
				int totalFailureCategoryValue;
				int totalFailureCategoryCount;
				double totalFailureCategoryPercent;
				this.GetProbeFailureCategoryStatistics(failureCategoryCounts, out totalFailureCategoryValue, out totalFailureCategoryCount, out totalFailureCategoryPercent);
				base.Result.TotalFailureCategoryValue = totalFailureCategoryValue;
				base.Result.TotalFailureCategoryCount = totalFailureCategoryCount;
				base.Result.TotalFailureCategoryPercent = totalFailureCategoryPercent;
				WTFDiagnostics.TraceDebug<int>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "OverallPercentSuccessHistoryByFailureCategoryMonitor: Processed total result(s), total max caluculated attribute is {0}.", base.Result.TotalFailureCategoryValue, null, "SetProbeFailureCategoryNumbers", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Monitors\\OverallPercentSuccessHistoryByFailureCategoryMonitor.cs", 192);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		internal void GetProbeFailureCategoryStatistics(Dictionary<int, int> failureCategoryCounts, out int maxAttributeValue, out int maxAttributeCount, out double maxAttributePercent)
		{
			if (failureCategoryCounts.Count == 0)
			{
				maxAttributeValue = -1;
				maxAttributeCount = 0;
				maxAttributePercent = 0.0;
				return;
			}
			maxAttributeValue = (from k in failureCategoryCounts
			orderby k.Value descending
			select k.Key).FirstOrDefault<int>();
			failureCategoryCounts.TryGetValue(maxAttributeValue, out maxAttributeCount);
			int num = failureCategoryCounts.Sum((KeyValuePair<int, int> attribute) => attribute.Value);
			maxAttributePercent = 100.0;
			if (num != 0)
			{
				maxAttributePercent = (double)(maxAttributeCount * 100 / num);
			}
		}

		internal virtual Task<bool> AlertBasedOnResultHistory(CancellationToken cancellationToken)
		{
			bool isAlert = true;
			int monitorCounter = 0;
			int chunkSuccess = 0;
			DateTime startTime = base.Result.ExecutionStartTime.AddSeconds(-base.Definition.SecondaryMonitoringThreshold);
			base.Result.StateAttribute5 = startTime.ToString();
			IOrderedEnumerable<MonitorResult> query = from r in base.Broker.GetSuccessfulMonitorResults(base.Definition, startTime)
			where r.ExecutionStartTime < base.Result.ExecutionStartTime
			orderby r.ExecutionEndTime descending
			select r;
			DateTime firstMonitorTime = DateTime.MaxValue;
			Task<int> task = base.Broker.AsDataAccessQuery<MonitorResult>(query).ExecuteAsync(delegate(MonitorResult result)
			{
				if (result.ExecutionStartTime.AddTicks(-(result.ExecutionStartTime.Ticks % 10000000L)) > startTime)
				{
					monitorCounter++;
					if (result.TotalSampleCount > 0)
					{
						if (result.TotalValue >= this.Definition.MonitoringThreshold)
						{
							this.Result.StateAttribute6 = 1.0;
							chunkSuccess++;
						}
						else if (this.Definition.FailureCategoryMask >= 0 && result.TotalFailureCategoryValue != this.Definition.FailureCategoryMask)
						{
							this.Result.StateAttribute6 = 4.0;
							isAlert = false;
						}
						if (result.ExecutionStartTime < firstMonitorTime)
						{
							firstMonitorTime = result.ExecutionStartTime;
						}
					}
				}
			}, cancellationToken, base.TraceContext);
			return task.Continue(delegate(Task<int> t)
			{
				if (isAlert)
				{
					if ((double)monitorCounter < this.Definition.SecondaryMonitoringThreshold / (double)this.Definition.RecurrenceIntervalSeconds - 1.0 || (this.Result.ExecutionStartTime - firstMonitorTime).TotalSeconds < this.Definition.SecondaryMonitoringThreshold - (double)this.Definition.RecurrenceIntervalSeconds)
					{
						this.Result.StateAttribute6 = 2.0;
						isAlert = false;
					}
					else if (monitorCounter != 0)
					{
						int num = monitorCounter - chunkSuccess;
						double num2 = (double)(100 * num) / (double)monitorCounter;
						if (num2 < this.Definition.MonitoringSamplesThreshold)
						{
							this.Result.StateAttribute7 = 1.0;
							isAlert = false;
						}
					}
				}
				return isAlert;
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		protected override void DoMonitorWork(CancellationToken cancellationToken)
		{
			Task task = Task.Factory.StartNew(delegate()
			{
				this.SetPercentSuccessNumbers(cancellationToken);
				if (this.Definition.FailureCategoryMask >= 0)
				{
					this.SetProbeFailureCategoryNumbers(cancellationToken);
				}
				WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, this.TraceContext, "OverallPercentSuccessHistoryByFailureCategoryMonitor: Finished collecting result data.", null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Monitors\\OverallPercentSuccessHistoryByFailureCategoryMonitor.cs", 370);
			}, cancellationToken, TaskCreationOptions.AttachedToParent, TaskScheduler.Current);
			task.ContinueWith(delegate(Task t)
			{
				if (this.Result.TotalValue < this.Definition.MonitoringThreshold)
				{
					Task<bool> task2 = this.AlertBasedOnResultHistory(cancellationToken);
					task2.Continue(delegate(bool alertTask)
					{
						if (alertTask)
						{
							if (this.Definition.FailureCategoryMask >= 0)
							{
								if (this.Result.TotalFailureCategoryValue == this.Definition.FailureCategoryMask)
								{
									this.Result.IsAlert = true;
								}
								else
								{
									this.Result.IsAlert = false;
								}
								if (!this.Result.IsAlert)
								{
									this.Result.StateAttribute6 = 3.0;
									return;
								}
							}
							else
							{
								this.Result.IsAlert = true;
							}
						}
					}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
				}
				WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, this.TraceContext, "OverallPercentSuccessHistoryByFailureCategoryMonitor: Finished analyzing probe results.", null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Monitors\\OverallPercentSuccessHistoryByFailureCategoryMonitor.cs", 420);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled, TaskScheduler.Default);
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(OverallPercentSuccessHistoryByFailureCategoryMonitor).FullName;
	}
}
