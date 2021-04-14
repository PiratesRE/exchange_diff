using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Store.Responders
{
	public class DatabaseRPCLatencyResponder : ResponderWorkItem
	{
		internal bool ShouldAlert(List<StoreUsageStatisticsData> storeUsageStatisticsData, int ropLatencyThreshold, int averageTimeInServerThreshold, int percentSampleBelowThresholdToAlert, int minimumStoreUsageStatisticsSampleCount, out List<string> mailboxGuids)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			mailboxGuids = new List<string>();
			if (storeUsageStatisticsData.Count == 0)
			{
				return true;
			}
			int num4 = (from susData in storeUsageStatisticsData
			where susData.RopCount > 0 && susData.TimeInServer / susData.RopCount >= ropLatencyThreshold
			select susData).Count<StoreUsageStatisticsData>();
			double num5 = Math.Round(100.0 * (double)(storeUsageStatisticsData.Count - num4) / (double)storeUsageStatisticsData.Count, 1);
			if (storeUsageStatisticsData.Count >= minimumStoreUsageStatisticsSampleCount && num5 < (double)percentSampleBelowThresholdToAlert)
			{
				var orderedEnumerable = from susData in storeUsageStatisticsData
				group susData by susData.MailboxGuid into groupByList
				select new
				{
					MailboxGuid = groupByList.Key,
					SampleCount = groupByList.Count<StoreUsageStatisticsData>(),
					TimeInServerSum = groupByList.Sum((StoreUsageStatisticsData t) => t.TimeInServer),
					RopCountSum = groupByList.Sum((StoreUsageStatisticsData r) => r.RopCount)
				} into desc
				orderby desc.TimeInServerSum descending
				select desc;
				foreach (var <>f__AnonymousType in orderedEnumerable)
				{
					if (<>f__AnonymousType.SampleCount > 0)
					{
						num3 = <>f__AnonymousType.TimeInServerSum / <>f__AnonymousType.SampleCount;
						num2 = <>f__AnonymousType.RopCountSum / <>f__AnonymousType.SampleCount;
					}
					if (num2 > 0)
					{
						num = num3 / num2;
					}
					if (num3 > averageTimeInServerThreshold || num > ropLatencyThreshold)
					{
						mailboxGuids.Add(<>f__AnonymousType.MailboxGuid);
					}
				}
				return true;
			}
			return false;
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			string targetExtension = base.Definition.TargetExtension;
			Guid databaseGuid = new Guid(targetExtension);
			base.Result.StateAttribute1 = targetExtension;
			base.Result.StateAttribute2 = base.Definition.TargetResource;
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.StoreTracer, base.TraceContext, "Starting responder for RPC average latency validation against database {0}", targetExtension, null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\DatabaseRPCLatencyResponder.cs", 123);
			List<StoreUsageStatisticsData> storeUsageStatisticsData = StoreUsageStatisticsResult.GetStoreUsageStatisticsData(databaseGuid, "TimeInServer");
			if (storeUsageStatisticsData.Count == 0)
			{
				base.Result.StateAttribute3 = Strings.UnableToGetStoreUsageStatisticsData(targetExtension);
				return;
			}
			List<string> list;
			bool flag = this.ShouldAlert(storeUsageStatisticsData, int.Parse(base.Definition.Attributes["RopLatencyThreshold"]), int.Parse(base.Definition.Attributes["AverageTimeInServerThreshold"]), int.Parse(base.Definition.Attributes["PercentSampleBelowThresholdToAlert"]), int.Parse(base.Definition.Attributes["MinimumStoreUsageStatisticsSampleCount"]), out list);
			if (flag)
			{
				base.Result.StateAttribute4 = StoreUsageStatisticsResult.SaveStoreUsageStatisticsData(storeUsageStatisticsData, Environment.MachineName, base.Result.StateAttribute2, DirectoryAccessor.Instance.IsDatabaseCopyActiveOnLocalServer(databaseGuid) ? bool.TrueString : bool.FalseString);
				if (list.Count > 0)
				{
					base.Result.StateAttribute5 = string.Join(", ", list);
					return;
				}
			}
			else
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.StoreTracer, base.TraceContext, "Database RPC latency responder unable to root cause issue during validation against database {0}; Alerting", targetExtension, null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\DatabaseRPCLatencyResponder.cs", 176);
				this.PublishSuccessNotification(base.Definition.TargetResource);
			}
		}

		private void PublishSuccessNotification(string databaseName)
		{
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.StoreTracer, base.TraceContext, "Database RPC latency responder found no issues during validation against database {0}; publishing green event for monitor to consume", databaseName, null, "PublishSuccessNotification", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\DatabaseRPCLatencyResponder.cs", 192);
			base.Result.RecoveryResult = ServiceRecoveryResult.Skipped;
			EventNotificationItem eventNotificationItem = new EventNotificationItem(ExchangeComponent.Eds.Name, base.Definition.Attributes.ContainsKey("RPCAverageLatencyErrorThreshold") ? "StoreRpcAverageLatencyTrigger_Error" : "StoreRpcAverageLatencyTrigger_Warning", databaseName, "SuppressDatabaseRPCLatencyMonitor", ResultSeverityLevel.Informational);
			eventNotificationItem.Publish(false);
		}
	}
}
