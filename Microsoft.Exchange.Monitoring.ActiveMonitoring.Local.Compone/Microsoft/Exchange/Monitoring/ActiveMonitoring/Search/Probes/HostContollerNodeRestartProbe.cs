using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Probes
{
	public class HostContollerNodeRestartProbe : SearchProbeBase
	{
		protected override void InternalDoWork(CancellationToken cancellationToken)
		{
			int @int = base.AttributeHelper.GetInt("RestartThreshold", true, 0, null, null);
			double num = base.AttributeHelper.GetDouble("RestartHistoryCheckWindowMinutes", true, 0.0, null, null);
			num *= 1.1;
			string targetResource = base.Definition.TargetResource;
			long performanceCounterValue = SearchMonitoringHelper.GetPerformanceCounterValue("Search Host Controller", "Component Restarts", targetResource + " Fsis");
			base.Result.StateAttribute1 = performanceCounterValue.ToString();
			long num2 = 0L;
			ProbeResult lastProbeResult = SearchMonitoringHelper.GetLastProbeResult(this, base.Broker, cancellationToken);
			if (lastProbeResult == null || string.IsNullOrEmpty(lastProbeResult.StateAttribute1))
			{
				return;
			}
			long.TryParse(lastProbeResult.StateAttribute1, out num2);
			base.Result.StateAttribute2 = num2.ToString();
			DateTime cutOffTime = base.Result.ExecutionStartTime.ToUniversalTime().AddMinutes(-num);
			List<HostContollerNodeRestartProbe.RestartHistoryRecord> list = this.ReadRestartHistory(lastProbeResult, cutOffTime);
			long num3 = (performanceCounterValue >= num2) ? (performanceCounterValue - num2) : performanceCounterValue;
			list.Add(new HostContollerNodeRestartProbe.RestartHistoryRecord
			{
				StartTime = lastProbeResult.ExecutionEndTime.ToUniversalTime(),
				EndTime = base.Result.ExecutionStartTime.ToUniversalTime(),
				RestartCount = (int)num3
			});
			this.PersistRestartHistory(list);
			int num4 = 0;
			foreach (HostContollerNodeRestartProbe.RestartHistoryRecord restartHistoryRecord in list)
			{
				num4 += restartHistoryRecord.RestartCount;
			}
			if (num4 > @int)
			{
				string minutes = ((int)(base.Result.ExecutionStartTime.ToUniversalTime() - list[0].StartTime).TotalMinutes).ToString();
				string details = this.FormatRestartHistory(list);
				throw new SearchProbeFailureException(Strings.HostControllerExcessiveNodeRestarts(targetResource, num4.ToString(), minutes, details));
			}
		}

		private void PersistRestartHistory(List<HostContollerNodeRestartProbe.RestartHistoryRecord> records)
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<HostContollerNodeRestartProbe.RestartHistoryRecord> list = new List<HostContollerNodeRestartProbe.RestartHistoryRecord>(records.ToArray());
			list.Sort((HostContollerNodeRestartProbe.RestartHistoryRecord x, HostContollerNodeRestartProbe.RestartHistoryRecord y) => y.StartTime.CompareTo(x.StartTime));
			foreach (HostContollerNodeRestartProbe.RestartHistoryRecord restartHistoryRecord in list)
			{
				stringBuilder.AppendFormat("{0}|{1}|{2}`", restartHistoryRecord.StartTime.ToString("s"), restartHistoryRecord.EndTime.ToString("s"), restartHistoryRecord.RestartCount);
				if (stringBuilder.Length > 768)
				{
					break;
				}
			}
			base.Result.StateAttribute3 = stringBuilder.ToString();
		}

		private List<HostContollerNodeRestartProbe.RestartHistoryRecord> ReadRestartHistory(ProbeResult probeResult, DateTime cutOffTime)
		{
			List<HostContollerNodeRestartProbe.RestartHistoryRecord> list = new List<HostContollerNodeRestartProbe.RestartHistoryRecord>();
			if (!string.IsNullOrEmpty(probeResult.StateAttribute3))
			{
				string[] array = probeResult.StateAttribute3.Split(new char[]
				{
					'`'
				});
				foreach (string text in array)
				{
					string[] array3 = text.Split(new char[]
					{
						'|'
					});
					if (array3.Length == 3)
					{
						HostContollerNodeRestartProbe.RestartHistoryRecord item = default(HostContollerNodeRestartProbe.RestartHistoryRecord);
						item.StartTime = DateTime.SpecifyKind(DateTime.Parse(array3[0]), DateTimeKind.Utc);
						item.EndTime = DateTime.SpecifyKind(DateTime.Parse(array3[1]), DateTimeKind.Utc);
						item.RestartCount = int.Parse(array3[2]);
						if (item.EndTime >= cutOffTime)
						{
							list.Add(item);
						}
					}
				}
			}
			list.Sort((HostContollerNodeRestartProbe.RestartHistoryRecord x, HostContollerNodeRestartProbe.RestartHistoryRecord y) => x.StartTime.CompareTo(y.StartTime));
			return list;
		}

		private string FormatRestartHistory(List<HostContollerNodeRestartProbe.RestartHistoryRecord> records)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (HostContollerNodeRestartProbe.RestartHistoryRecord restartHistoryRecord in records)
			{
				stringBuilder.AppendFormat(Strings.HostControllerNodeRestartDetails(restartHistoryRecord.StartTime.ToString(), restartHistoryRecord.EndTime.ToString(), restartHistoryRecord.RestartCount.ToString()), new object[0]);
			}
			return stringBuilder.ToString();
		}

		private const string RestartHistoryRecordTemplate = "{0}|{1}|{2}`";

		private const int StateAttributeLenghCap = 768;

		private struct RestartHistoryRecord
		{
			public DateTime StartTime { get; set; }

			public DateTime EndTime { get; set; }

			public int RestartCount { get; set; }
		}
	}
}
