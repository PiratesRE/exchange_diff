using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.HttpProxy.Routing;

namespace Microsoft.Exchange.HttpProxy.RouteSelector
{
	internal class RouteSelectorDiagnostics : IRouteSelectorModuleDiagnostics, IRouteSelectorDiagnostics, IRoutingDiagnostics
	{
		public RouteSelectorDiagnostics(RequestLogger baseLogger)
		{
			this.baseLogger = baseLogger;
			this.accountForestLatencies = new List<long>(2);
			this.globalLocatorLatencies = new List<long>(2);
			this.resourceForestLatencies = new List<long>(2);
			this.serverLocatorLatencies = new List<long>(2);
			this.sharedCacheLatencies = new List<long>(2);
		}

		public static void UpdateRoutingFailurePerfCounter(string serverFqdn, bool wasFailure)
		{
			if (RouteSelectorModule.IsTesting)
			{
				return;
			}
			if (!PerfCounters.RoutingErrorsEnabled)
			{
				return;
			}
			string text = string.Empty;
			if (serverFqdn != null)
			{
				string[] array = serverFqdn.Split(new char[]
				{
					'.'
				});
				if (array[0].Length > 5)
				{
					text = array[0].Substring(0, array[0].Length - 5);
				}
				else
				{
					text = array[0];
				}
			}
			if (wasFailure)
			{
				PerfCounters.UpdateMovingPercentagePerformanceCounter(PerfCounters.GetHttpProxyPerSiteCountersInstance(text).MovingPercentageRoutingFailure);
				PerfCounters.GetHttpProxyPerSiteCountersInstance(text).TotalFailedRequests.Increment();
			}
			if (!string.IsNullOrEmpty(text))
			{
				PerfCounters.IncrementMovingPercentagePerformanceCounterBase(PerfCounters.GetHttpProxyPerSiteCountersInstance(text).MovingPercentageRoutingFailure);
			}
			PerfCounters.IncrementMovingPercentagePerformanceCounterBase(PerfCounters.GetHttpProxyPerSiteCountersInstance(string.Empty).MovingPercentageRoutingFailure);
		}

		public void SetTargetServer(string value)
		{
			this.baseLogger.LogField(LogKey.TargetServer, value);
		}

		public void SetTargetServerVersion(string value)
		{
			this.baseLogger.LogField(LogKey.TargetServerVersion, value);
		}

		public void SetOrganization(string value)
		{
			this.baseLogger.LogField(LogKey.Organization, value);
		}

		public void AddRoutingEntry(string value)
		{
			this.baseLogger.AppendGenericInfo("RoutingEntry", value);
		}

		public void AddErrorInfo(object value)
		{
			this.baseLogger.AppendErrorInfo("RouteSelector", value);
		}

		public void SaveRoutingLatency(Action operationToTrack)
		{
			this.baseLogger.LatencyTracker.LogLatency(LogKey.CalculateTargetBackEndLatency, operationToTrack);
		}

		public void ProcessRoutingKey(IRoutingKey key)
		{
			if (ServerLocator.IsMailboxServerCacheKey(key))
			{
				PerfCounters.HttpProxyCacheCountersInstance.BackEndServerOverallCacheHitsRateBase.Increment();
				PerfCounters.IncrementMovingPercentagePerformanceCounterBase(PerfCounters.HttpProxyCacheCountersInstance.MovingPercentageBackEndServerOverallCacheHitsRate);
			}
			if (ServerLocator.IsAnchorMailboxCacheKey(key))
			{
				PerfCounters.HttpProxyCacheCountersInstance.OverallCacheEffectivenessRateBase.Increment();
				PerfCounters.HttpProxyCacheCountersInstance.AnchorMailboxOverallCacheHitsRateBase.Increment();
			}
		}

		public void ProcessRoutingEntry(IRoutingEntry entry)
		{
			if (ServerLocator.IsMailboxServerCacheKey(entry.Key))
			{
				PerfCounters.HttpProxyCacheCountersInstance.BackEndServerOverallCacheHitsRate.Increment();
				PerfCounters.UpdateMovingPercentagePerformanceCounter(PerfCounters.HttpProxyCacheCountersInstance.MovingPercentageBackEndServerOverallCacheHitsRate);
				return;
			}
			if (ServerLocator.IsAnchorMailboxCacheKey(entry.Key))
			{
				PerfCounters.HttpProxyCacheCountersInstance.AnchorMailboxOverallCacheHitsRate.Increment();
				PerfCounters.HttpProxyCacheCountersInstance.OverallCacheEffectivenessRate.Increment();
			}
		}

		public void LogLatencies()
		{
			this.baseLogger.LogField(LogKey.AccountForestLatencyBreakup, RouteSelectorDiagnostics.GetBreakupOfLatencies(this.accountForestLatencies));
			this.baseLogger.LogField(LogKey.TotalAccountForestLatency, this.accountForestLatencies.Sum());
			this.baseLogger.LogField(LogKey.GlsLatencyBreakup, RouteSelectorDiagnostics.GetBreakupOfLatencies(this.globalLocatorLatencies));
			this.baseLogger.LogField(LogKey.TotalGlsLatency, this.globalLocatorLatencies.Sum());
			this.baseLogger.LogField(LogKey.ResourceForestLatencyBreakup, RouteSelectorDiagnostics.GetBreakupOfLatencies(this.resourceForestLatencies));
			this.baseLogger.LogField(LogKey.TotalResourceForestLatency, this.resourceForestLatencies.Sum());
			this.baseLogger.LogField(LogKey.SharedCacheLatencyBreakup, RouteSelectorDiagnostics.GetBreakupOfLatencies(this.sharedCacheLatencies));
			this.baseLogger.LogField(LogKey.TotalSharedCacheLatency, this.sharedCacheLatencies.Sum());
			this.baseLogger.LogField(LogKey.ServerLocatorLatency, this.serverLocatorLatencies.Sum());
		}

		public void ProcessLatencyPerfCounters()
		{
			long num = this.serverLocatorLatencies.Sum();
			PerfCounters.HttpProxyCountersInstance.MailboxServerLocatorLatency.RawValue = num;
			PerfCounters.HttpProxyCountersInstance.MailboxServerLocatorAverageLatency.IncrementBy(num);
			PerfCounters.HttpProxyCountersInstance.MailboxServerLocatorAverageLatencyBase.Increment();
			PerfCounters.UpdateMovingAveragePerformanceCounter(PerfCounters.HttpProxyCountersInstance.MovingAverageMailboxServerLocatorLatency, num);
		}

		public void AddAccountForestLatency(TimeSpan latency)
		{
			this.accountForestLatencies.Add(Convert.ToInt64(latency.TotalMilliseconds));
		}

		public void AddActiveManagerLatency(TimeSpan latency)
		{
		}

		public void AddDiagnosticText(string text)
		{
			this.baseLogger.AppendGenericInfo("SharedCache", text);
		}

		public void AddGlobalLocatorLatency(TimeSpan latency)
		{
			this.globalLocatorLatencies.Add(Convert.ToInt64(latency.TotalMilliseconds));
		}

		public void AddResourceForestLatency(TimeSpan latency)
		{
			this.resourceForestLatencies.Add(Convert.ToInt64(latency.TotalMilliseconds));
		}

		public void AddServerLocatorLatency(TimeSpan latency)
		{
			this.serverLocatorLatencies.Add(Convert.ToInt64(latency.TotalMilliseconds));
		}

		public void AddSharedCacheLatency(TimeSpan latency)
		{
			this.sharedCacheLatencies.Add(Convert.ToInt64(latency.TotalMilliseconds));
		}

		private static string GetBreakupOfLatencies(List<long> latencies)
		{
			if (latencies == null)
			{
				throw new ArgumentNullException("latencies");
			}
			StringBuilder result = new StringBuilder();
			latencies.ForEach(delegate(long latency)
			{
				result.Append(latency);
				result.Append(';');
			});
			return result.ToString();
		}

		private readonly RequestLogger baseLogger;

		private readonly List<long> accountForestLatencies;

		private readonly List<long> globalLocatorLatencies;

		private readonly List<long> resourceForestLatencies;

		private readonly List<long> serverLocatorLatencies;

		private readonly List<long> sharedCacheLatencies;
	}
}
