using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors
{
	public abstract class OverallConsecutiveAggregationMonitor : MonitorWorkItem
	{
		protected int LastProbeValue { get; set; }

		private protected int[] ProbeValueHistory { protected get; private set; }

		private protected int NumberOfValuesToStore { protected get; private set; }

		protected override void DoMonitorWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "OverallConsecutiveAggregationMonitor: Starting monitor action.", null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Monitors\\OverallConsecutiveAggregationMonitor.cs", 66);
			if (base.Definition.Attributes.ContainsKey(OverallConsecutiveAggregationMonitor.ValueCountsToStoreKey))
			{
				this.NumberOfValuesToStore = int.Parse(base.Definition.Attributes[OverallConsecutiveAggregationMonitor.ValueCountsToStoreKey]);
			}
			else
			{
				this.NumberOfValuesToStore = Math.Max(1, base.Definition.MonitoringIntervalSeconds / base.Definition.RecurrenceIntervalSeconds);
			}
			DateTime executionStartTime = base.Result.ExecutionStartTime;
			Task<ProbeResult> task = base.Broker.GetProbeResults(base.Definition.SampleMask, base.MonitoringWindowStartTime, executionStartTime).ExecuteAsync(cancellationToken, base.TraceContext);
			Task task2 = task.Continue(new Action<ProbeResult>(this.GetLastProbeMetric), cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
			Task<MonitorResult> task3 = base.Broker.GetLastMonitorResult(base.Definition, TimeSpan.FromSeconds((double)(base.Definition.RecurrenceIntervalSeconds * 2))).ExecuteAsync(cancellationToken, base.TraceContext);
			string probeHistory = null;
			task3.Continue(delegate(MonitorResult lastMonitorResult)
			{
				if (lastMonitorResult != null)
				{
					probeHistory = lastMonitorResult.StateAttribute1;
				}
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled).Wait();
			this.ProbeValueHistory = OverallConsecutiveAggregationMonitor.ParseProbeHistory(probeHistory);
			task2.Wait();
			this.ProbeValueHistory[this.ProbeValueHistory.Length - 1] = this.LastProbeValue;
			base.Result.StateAttribute1 = OverallConsecutiveAggregationMonitor.ProbeHistoryToString(this.ProbeValueHistory, this.NumberOfValuesToStore);
			base.Result.IsAlert = this.ShouldAlert();
			WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "OverallConsecutiveAggregationMonitor: Completed monitor action.", null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Monitors\\OverallConsecutiveAggregationMonitor.cs", 107);
		}

		protected abstract bool ShouldAlert();

		protected virtual void GetLastProbeMetric(ProbeResult probeResult)
		{
			this.LastProbeValue = ((probeResult == null) ? 0 : ((int)probeResult.StateAttribute6));
		}

		private static int[] ParseProbeHistory(string probeHistory)
		{
			if (string.IsNullOrEmpty(probeHistory))
			{
				return new int[1];
			}
			string[] array = probeHistory.TrimEnd(new char[]
			{
				OverallConsecutiveAggregationMonitor.ErrorListDelimiter
			}).Split(new char[]
			{
				OverallConsecutiveAggregationMonitor.ErrorListDelimiter
			});
			int[] array2 = new int[array.Length + 1];
			int num = 0;
			foreach (string s in array)
			{
				array2[num++] = int.Parse(s);
			}
			return array2;
		}

		private static string ProbeHistoryToString(int[] errorCounts, int maxArrayLength)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = Math.Max(0, errorCounts.Length - maxArrayLength); i < errorCounts.Length; i++)
			{
				stringBuilder.AppendFormat("{0}{1}", errorCounts[i].ToString(), OverallConsecutiveAggregationMonitor.ErrorListDelimiter);
			}
			return stringBuilder.ToString();
		}

		public static readonly string ValueCountsToStoreKey = "ValueCountsToStore";

		private static readonly char ErrorListDelimiter = ',';
	}
}
