using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Probes
{
	public class SearchQueryFailureProbe : SearchProbeBase
	{
		protected override bool SkipOnNonHealthyCatalog
		{
			get
			{
				return true;
			}
		}

		protected override bool SkipOnNonActiveDatabase
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

		internal static string GetFullTextIndexExceptionEventsCached(int seconds)
		{
			if (DateTime.UtcNow < SearchQueryFailureProbe.recentFailureEventsCacheTimeoutTime)
			{
				return SearchQueryFailureProbe.recentFailureEventsCached;
			}
			bool flag = false;
			string result;
			try
			{
				object obj;
				Monitor.Enter(obj = SearchQueryFailureProbe.recentFailureEventsCacheLock, ref flag);
				if (DateTime.UtcNow < SearchQueryFailureProbe.recentFailureEventsCacheTimeoutTime)
				{
					result = SearchQueryFailureProbe.recentFailureEventsCached;
				}
				else
				{
					StringBuilder sb = new StringBuilder();
					Action delegateGetEvents = delegate()
					{
						try
						{
							List<EventRecord> events = SearchMonitoringHelper.GetEvents("Application", 1012, "MSExchangeIS", seconds, 2, null);
							foreach (EventRecord eventRecord in events)
							{
								sb.AppendLine("=====================================");
								sb.AppendLine(eventRecord.TimeCreated.ToString());
								sb.AppendLine(eventRecord.FormatDescription());
								eventRecord.Dispose();
							}
						}
						catch (Exception ex)
						{
							SearchMonitoringHelper.LogInfo("Exception caught reading query failure event logs:\n{0}.", new object[]
							{
								ex
							});
						}
					};
					IAsyncResult asyncResult = delegateGetEvents.BeginInvoke(delegate(IAsyncResult r)
					{
						delegateGetEvents.EndInvoke(r);
					}, null);
					if (!asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromMinutes(2.0)))
					{
						SearchMonitoringHelper.LogInfo("Timeout reading query failure event logs.", new object[0]);
						result = null;
					}
					else
					{
						string text = sb.ToString();
						if (text.Length > 0)
						{
							SearchQueryFailureProbe.recentFailureEventsCached = text;
							SearchQueryFailureProbe.recentFailureEventsCacheTimeoutTime = DateTime.UtcNow.AddMinutes(20.0);
						}
						result = text;
					}
				}
			}
			finally
			{
				if (flag)
				{
					object obj;
					Monitor.Exit(obj);
				}
			}
			return result;
		}

		protected override void InternalDoWork(CancellationToken cancellationToken)
		{
			string targetResource = base.Definition.TargetResource;
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = (double)SearchMonitoringHelper.GetPerformanceCounterValue("MSExchangeIS Store", "Total searches", targetResource);
			double num5 = (double)SearchMonitoringHelper.GetPerformanceCounterValue("MSExchangeIS Store", "Total number of successful search queries", targetResource);
			double num6 = (double)SearchMonitoringHelper.GetPerformanceCounterValue("MSExchangeIS Store", "Total search queries completed in > 60 sec", targetResource);
			base.Result.StateAttribute6 = num4;
			base.Result.StateAttribute7 = num5;
			base.Result.StateAttribute8 = num6;
			ProbeResult lastProbeResult = SearchMonitoringHelper.GetLastProbeResult(this, base.Broker, cancellationToken);
			if (lastProbeResult != null && lastProbeResult.StateAttribute6 <= num4 && lastProbeResult.StateAttribute7 <= num5 && lastProbeResult.StateAttribute8 <= num6)
			{
				num = lastProbeResult.StateAttribute6;
				num2 = lastProbeResult.StateAttribute7;
				num3 = lastProbeResult.StateAttribute8;
			}
			double num7 = num4 - num;
			if (num7 == 0.0)
			{
				return;
			}
			double num8 = (num7 - (num5 - num2)) / num7;
			double @double = base.AttributeHelper.GetDouble("FailureRateThreshold", true, 0.0, null, null);
			if (num8 > @double)
			{
				string text = SearchQueryFailureProbe.GetFullTextIndexExceptionEventsCached(base.Definition.RecurrenceIntervalSeconds);
				if (string.IsNullOrWhiteSpace(text))
				{
					text = Strings.SearchInformationNotAvailable;
				}
				throw new SearchProbeFailureException(Strings.SearchQueryFailure(targetResource, num8.ToString("P"), @double.ToString("P"), num7.ToString(), (num5 - num2).ToString(), text));
			}
			double double2 = base.AttributeHelper.GetDouble("SlowRateThreshold", true, 0.0, null, null);
			double num9 = (num6 - num3) / (num5 - num2);
			if (num9 > double2)
			{
				throw new SearchProbeFailureException(Strings.SearchQuerySlow(targetResource, num8.ToString("P"), @double.ToString("P")));
			}
		}

		internal const string ApplicationEventLogName = "Application";

		internal const int FullTextIndexExceptionEventId = 1012;

		internal const string MSExchangeISProviderName = "MSExchangeIS";

		private const int RecentFailureEventsCachedTimeoutMinutes = 20;

		private const int GetEventsTimeoutMinutes = 2;

		private static string recentFailureEventsCached;

		private static DateTime recentFailureEventsCacheTimeoutTime = DateTime.MinValue;

		private static readonly object recentFailureEventsCacheLock = new object();
	}
}
