using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Search;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Probes
{
	public class SearchIndexBacklogProbe : SearchProbeBase
	{
		internal static bool TryGetCompletedCallWithCache(out long count, out int ctsProcessId, out DateTime timestamp)
		{
			count = 0L;
			timestamp = DateTime.MinValue;
			ctsProcessId = 0;
			bool result;
			lock (SearchIndexBacklogProbe.completedCallbackLock)
			{
				if (DateTime.UtcNow - SearchIndexBacklogProbe.completedCallbackCountTimestamp > SearchIndexBacklogProbe.CompletedCallbackCacheTime)
				{
					try
					{
						count = SearchMonitoringHelper.GetPerformanceCounterValue("Search Content Processing", "# Completed Callbacks Total", "ContentEngineNode1");
						timestamp = DateTime.UtcNow;
						Dictionary<string, int> nodeProcessIds = SearchMonitoringHelper.GetNodeProcessIds();
						ctsProcessId = nodeProcessIds["ContentEngineNode1"];
					}
					catch (Exception ex)
					{
						SearchMonitoringHelper.LogInfo("Failed to get the counter value for '{0}' or CTS process ID. Exception is caught: {1}", new object[]
						{
							"# Completed Callbacks Total",
							ex.ToString()
						});
						return false;
					}
					SearchIndexBacklogProbe.completedCallbackCount = count;
					SearchIndexBacklogProbe.completedCallbackCountTimestamp = timestamp;
					SearchIndexBacklogProbe.ctsProcessId = ctsProcessId;
					result = true;
				}
				else
				{
					count = SearchIndexBacklogProbe.completedCallbackCount;
					timestamp = SearchIndexBacklogProbe.completedCallbackCountTimestamp;
					ctsProcessId = SearchIndexBacklogProbe.ctsProcessId;
					result = true;
				}
			}
			return result;
		}

		protected override bool SkipOnNonHealthyCatalog
		{
			get
			{
				return true;
			}
		}

		protected override bool SkipOnAutoDagExcludeFromMonitoring
		{
			get
			{
				return true;
			}
		}

		protected override void InternalDoWork(CancellationToken cancellationToken)
		{
			string targetResource = base.Definition.TargetResource;
			int @int = base.AttributeHelper.GetInt("BacklogThresholdMinutes", true, 0, null, null);
			int int2 = base.AttributeHelper.GetInt("RetryQueueSizeThreshold", true, 0, null, null);
			int int3 = base.AttributeHelper.GetInt("RetryQueueSizeHighThreshold", true, 0, null, null);
			int int4 = base.AttributeHelper.GetInt("SystemUpTimeGracePeriodHours", true, 0, null, null);
			bool @bool = base.AttributeHelper.GetBool("SkipAgeOfLastNotificationProcessedCheck", false, false);
			bool bool2 = base.AttributeHelper.GetBool("SkipRetryItemsCheck", false, false);
			int num;
			int num2;
			if (!this.TryGetBacklogAndRetryItems(targetResource, out num, out num2))
			{
				SearchMonitoringHelper.LogInfo(this, "Failed to get backlog/retry items from diagnostic info.", new object[0]);
				return;
			}
			base.Result.StateAttribute1 = num.ToString();
			base.Result.StateAttribute2 = num2.ToString();
			int num3 = num / 60;
			if (num3 > @int || num2 > int2)
			{
				TimeSpan systemUpTime = SearchMonitoringHelper.GetSystemUpTime();
				if (systemUpTime.TotalHours < (double)int4)
				{
					base.Result.StateAttribute11 = "FailedButSuppressed";
					return;
				}
				bool flag = false;
				int num4 = 0;
				int num5 = 0;
				string lastTime = Strings.SearchInformationNotAvailable;
				ProbeResult lastProbeResult = SearchMonitoringHelper.GetLastProbeResult(this, base.Broker, cancellationToken);
				if (lastProbeResult != null && !string.IsNullOrEmpty(lastProbeResult.StateAttribute1) && !string.IsNullOrEmpty(lastProbeResult.StateAttribute2))
				{
					num4 = int.Parse(lastProbeResult.StateAttribute1);
					num5 = int.Parse(lastProbeResult.StateAttribute2);
					base.Result.StateAttribute3 = num4.ToString();
					base.Result.StateAttribute4 = num5.ToString();
					lastTime = lastProbeResult.ExecutionEndTime.ToShortTimeString();
					flag = true;
				}
				if (((num2 > int3 || num2 >= num5) && !bool2) || (num3 > @int && !@bool))
				{
					string severBacklogStatus = this.GetSeverBacklogStatus();
					string upTime = systemUpTime.ToString();
					double num6;
					string completedCount;
					string minutes;
					LocalizedString localizedString;
					if (SearchIndexBacklogProbe.TryGetCompletedCallbackPerSecond(this, base.Broker, cancellationToken, out num6, out completedCount, out minutes))
					{
						if (flag)
						{
							localizedString = Strings.SearchIndexBacklogWithProcessingRateAndHistory(targetResource, num.ToString(), num2.ToString(), lastTime, num4.ToString(), num5.ToString(), completedCount, num6.ToString("F"), minutes, upTime, severBacklogStatus);
						}
						else
						{
							localizedString = Strings.SearchIndexBacklogWithProcessingRate(targetResource, num.ToString(), num2.ToString(), completedCount, num6.ToString("F"), minutes, upTime, severBacklogStatus);
						}
					}
					else if (flag)
					{
						localizedString = Strings.SearchIndexBacklogWithHistory(targetResource, num.ToString(), num2.ToString(), lastTime, num4.ToString(), num5.ToString(), upTime, severBacklogStatus);
					}
					else
					{
						localizedString = Strings.SearchIndexBacklog(targetResource, num.ToString(), num2.ToString(), upTime, severBacklogStatus);
					}
					throw new SearchProbeFailureException(localizedString);
				}
			}
		}

		internal static bool TryGetCompletedCallbackPerSecond(ProbeWorkItem probe, IProbeWorkBroker broker, CancellationToken cancellationToken, out double processingRate, out string completedCallbackCount, out string timeWindowMinutes)
		{
			processingRate = 0.0;
			completedCallbackCount = Strings.SearchInformationNotAvailable.ToString();
			timeWindowMinutes = Strings.SearchInformationNotAvailable.ToString();
			long num;
			int num2;
			DateTime d;
			if (SearchIndexBacklogProbe.TryGetCompletedCallWithCache(out num, out num2, out d))
			{
				probe.Result.StateAttribute5 = d.ToString("u");
				probe.Result.StateAttribute6 = (double)num;
				probe.Result.StateAttribute7 = (double)num2;
				ProbeResult lastProbeResult = SearchMonitoringHelper.GetLastProbeResult(probe, broker, cancellationToken);
				if (lastProbeResult != null && !string.IsNullOrEmpty(lastProbeResult.StateAttribute5))
				{
					DateTime d2 = DateTime.Parse(lastProbeResult.StateAttribute5).ToUniversalTime();
					int num3 = (int)lastProbeResult.StateAttribute7;
					long num4 = (long)lastProbeResult.StateAttribute6;
					if (num3 == num2 && num >= num4)
					{
						timeWindowMinutes = (d - d2).TotalMinutes.ToString("F2");
						completedCallbackCount = (num - num4).ToString();
						processingRate = ((double)num - (double)num4) / (d - d2).TotalSeconds;
						return true;
					}
				}
			}
			return false;
		}

		private bool TryGetBacklogAndRetryItems(string databaseName, out int backlog, out int retryItems)
		{
			backlog = 0;
			retryItems = 0;
			DiagnosticInfo.FeedingControllerDiagnosticInfo cachedFeedingControllerDiagnosticInfo = SearchMonitoringHelper.GetCachedFeedingControllerDiagnosticInfo(databaseName);
			if (cachedFeedingControllerDiagnosticInfo != null)
			{
				backlog = cachedFeedingControllerDiagnosticInfo.AgeOfLastNotificationProcessed;
				retryItems = cachedFeedingControllerDiagnosticInfo.RetryItems;
				return true;
			}
			return false;
		}

		private string GetSeverBacklogStatus()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in LocalEndpointManager.Instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend)
			{
				string mailboxDatabaseName = mailboxDatabaseInfo.MailboxDatabaseName;
				int num;
				int num2;
				if (this.TryGetBacklogAndRetryItems(mailboxDatabaseName, out num, out num2))
				{
					stringBuilder.AppendFormat("{0}: ContentIndexBacklog: {1}, ContentIndexRetryQueueSize: {2}. ", mailboxDatabaseName, num, num2);
					stringBuilder.AppendLine();
				}
			}
			return stringBuilder.ToString();
		}

		private static readonly TimeSpan CompletedCallbackCacheTime = TimeSpan.FromMinutes(5.0);

		private static long completedCallbackCount;

		private static int ctsProcessId;

		private static DateTime completedCallbackCountTimestamp = DateTime.MinValue;

		private static readonly object completedCallbackLock = new object();
	}
}
