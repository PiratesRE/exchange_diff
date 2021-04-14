using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HttpProxy.Common
{
	internal sealed class HttpProxyCacheCountersInstance : PerformanceCounterInstance
	{
		internal HttpProxyCacheCountersInstance(string instanceName, HttpProxyCacheCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange HttpProxy Cache")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.AnchorMailboxCacheSize = new ExPerformanceCounter(base.CategoryName, "AnchorMailbox to Database Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AnchorMailboxCacheSize);
				this.AnchorMailboxLocalCacheHitsRate = new ExPerformanceCounter(base.CategoryName, "AnchorMailbox to Database Local Cache Hit Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AnchorMailboxLocalCacheHitsRate);
				this.AnchorMailboxLocalCacheHitsRateBase = new ExPerformanceCounter(base.CategoryName, "AnchorMailbox to Database Local Cache Hit Rate Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AnchorMailboxLocalCacheHitsRateBase);
				this.AnchorMailboxOverallCacheHitsRate = new ExPerformanceCounter(base.CategoryName, "AnchorMailbox to Database Overall Cache Hit Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AnchorMailboxOverallCacheHitsRate);
				this.AnchorMailboxOverallCacheHitsRateBase = new ExPerformanceCounter(base.CategoryName, "AnchorMailbox to Database Overall Cache Hit Rate Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AnchorMailboxOverallCacheHitsRateBase);
				this.NegativeAnchorMailboxCacheSize = new ExPerformanceCounter(base.CategoryName, "NegativeAnchorMailbox Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NegativeAnchorMailboxCacheSize);
				this.NegativeAnchorMailboxLocalCacheHitsRate = new ExPerformanceCounter(base.CategoryName, "NegativeAnchorMailbox Local Cache Hit Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NegativeAnchorMailboxLocalCacheHitsRate);
				this.NegativeAnchorMailboxLocalCacheHitsRateBase = new ExPerformanceCounter(base.CategoryName, "NegativeAnchorMailbox Local Cache Hit Rate Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NegativeAnchorMailboxLocalCacheHitsRateBase);
				this.BackEndServerCacheSize = new ExPerformanceCounter(base.CategoryName, "DatabaseGuid to MailboxServer Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BackEndServerCacheSize);
				this.BackEndServerLocalCacheHitsRate = new ExPerformanceCounter(base.CategoryName, "DatabaseGuid to MailboxServer Local Cache Hit Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BackEndServerLocalCacheHitsRate);
				this.BackEndServerLocalCacheHitsRateBase = new ExPerformanceCounter(base.CategoryName, "DatabaseGuid to MailboxServer Local Cache Hit Rate Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BackEndServerLocalCacheHitsRateBase);
				this.BackEndServerOverallCacheHitsRate = new ExPerformanceCounter(base.CategoryName, "DatabaseGuid to MailboxServer Overall Cache Hit Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BackEndServerOverallCacheHitsRate);
				this.BackEndServerOverallCacheHitsRateBase = new ExPerformanceCounter(base.CategoryName, "DatabaseGuid to MailboxServer Overall Cache Hit Rate Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BackEndServerOverallCacheHitsRateBase);
				this.BackEndServerCacheLocalServerListCount = new ExPerformanceCounter(base.CategoryName, "DatabaseGuid to MailboxServer Cache Local Site MailboxServers", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BackEndServerCacheLocalServerListCount);
				this.BackEndServerCacheRefreshingQueueLength = new ExPerformanceCounter(base.CategoryName, "DatabaseGuid to MailboxServer Cache Refreshing Queue Length", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BackEndServerCacheRefreshingQueueLength);
				this.BackEndServerCacheRefreshingStatus = new ExPerformanceCounter(base.CategoryName, "DatabaseGuid to MailboxServer Cache Refreshing Status", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BackEndServerCacheRefreshingStatus);
				this.FbaModuleKeyCacheSize = new ExPerformanceCounter(base.CategoryName, "FBAModule Key Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.FbaModuleKeyCacheSize);
				this.FbaModuleKeyCacheHitsRate = new ExPerformanceCounter(base.CategoryName, "FBAModule Key Cache Hits Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.FbaModuleKeyCacheHitsRate);
				this.FbaModuleKeyCacheHitsRateBase = new ExPerformanceCounter(base.CategoryName, "FBAModule Key Cache Hits Rate Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.FbaModuleKeyCacheHitsRateBase);
				this.CookieUseRate = new ExPerformanceCounter(base.CategoryName, "AnchorMailbox to MailboxServer Cookie Hit Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CookieUseRate);
				this.CookieUseRateBase = new ExPerformanceCounter(base.CategoryName, "AnchorMailbox to MailboxServer Cookie Hit Rate Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CookieUseRateBase);
				this.OverallCacheEffectivenessRate = new ExPerformanceCounter(base.CategoryName, "Overall Cache Effectiveness (% of requests)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OverallCacheEffectivenessRate);
				this.OverallCacheEffectivenessRateBase = new ExPerformanceCounter(base.CategoryName, "Overall Cache Effectiveness (% of requests) Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OverallCacheEffectivenessRateBase);
				this.RouteRefresherSuccessfulMailboxServerCacheUpdates = new ExPerformanceCounter(base.CategoryName, "Route Refresher Mailbox Server Cache Updates", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RouteRefresherSuccessfulMailboxServerCacheUpdates);
				this.RouteRefresherTotalMailboxServerCacheUpdateAttempts = new ExPerformanceCounter(base.CategoryName, "Route Refresher Mailbox Server Cache Update Attempts", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RouteRefresherTotalMailboxServerCacheUpdateAttempts);
				this.RouteRefresherSuccessfulAnchorMailboxCacheUpdates = new ExPerformanceCounter(base.CategoryName, "Route Refresher Anchor Mailbox Cache Updates", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RouteRefresherSuccessfulAnchorMailboxCacheUpdates);
				this.RouteRefresherTotalAnchorMailboxCacheUpdateAttempts = new ExPerformanceCounter(base.CategoryName, "Route Refresher Anchor Mailbox Cache Update Attempts", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RouteRefresherTotalAnchorMailboxCacheUpdateAttempts);
				this.MovingPercentageBackEndServerLocalCacheHitsRate = new ExPerformanceCounter(base.CategoryName, "DatabaseGuid to MailboxServer Local Cache Hit Rate (Moving Average)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MovingPercentageBackEndServerLocalCacheHitsRate);
				this.MovingPercentageBackEndServerOverallCacheHitsRate = new ExPerformanceCounter(base.CategoryName, "DatabaseGuid to MailboxServer Overall Cache Hit Rate (Moving Average)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MovingPercentageBackEndServerOverallCacheHitsRate);
				this.MovingPercentageCookieUseRate = new ExPerformanceCounter(base.CategoryName, "AnchorMailbox to MailboxServer Cookie Hit Rate (Moving Average)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MovingPercentageCookieUseRate);
				long num = this.AnchorMailboxCacheSize.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal HttpProxyCacheCountersInstance(string instanceName) : base(instanceName, "MSExchange HttpProxy Cache")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.AnchorMailboxCacheSize = new ExPerformanceCounter(base.CategoryName, "AnchorMailbox to Database Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AnchorMailboxCacheSize);
				this.AnchorMailboxLocalCacheHitsRate = new ExPerformanceCounter(base.CategoryName, "AnchorMailbox to Database Local Cache Hit Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AnchorMailboxLocalCacheHitsRate);
				this.AnchorMailboxLocalCacheHitsRateBase = new ExPerformanceCounter(base.CategoryName, "AnchorMailbox to Database Local Cache Hit Rate Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AnchorMailboxLocalCacheHitsRateBase);
				this.AnchorMailboxOverallCacheHitsRate = new ExPerformanceCounter(base.CategoryName, "AnchorMailbox to Database Overall Cache Hit Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AnchorMailboxOverallCacheHitsRate);
				this.AnchorMailboxOverallCacheHitsRateBase = new ExPerformanceCounter(base.CategoryName, "AnchorMailbox to Database Overall Cache Hit Rate Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AnchorMailboxOverallCacheHitsRateBase);
				this.NegativeAnchorMailboxCacheSize = new ExPerformanceCounter(base.CategoryName, "NegativeAnchorMailbox Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NegativeAnchorMailboxCacheSize);
				this.NegativeAnchorMailboxLocalCacheHitsRate = new ExPerformanceCounter(base.CategoryName, "NegativeAnchorMailbox Local Cache Hit Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NegativeAnchorMailboxLocalCacheHitsRate);
				this.NegativeAnchorMailboxLocalCacheHitsRateBase = new ExPerformanceCounter(base.CategoryName, "NegativeAnchorMailbox Local Cache Hit Rate Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NegativeAnchorMailboxLocalCacheHitsRateBase);
				this.BackEndServerCacheSize = new ExPerformanceCounter(base.CategoryName, "DatabaseGuid to MailboxServer Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BackEndServerCacheSize);
				this.BackEndServerLocalCacheHitsRate = new ExPerformanceCounter(base.CategoryName, "DatabaseGuid to MailboxServer Local Cache Hit Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BackEndServerLocalCacheHitsRate);
				this.BackEndServerLocalCacheHitsRateBase = new ExPerformanceCounter(base.CategoryName, "DatabaseGuid to MailboxServer Local Cache Hit Rate Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BackEndServerLocalCacheHitsRateBase);
				this.BackEndServerOverallCacheHitsRate = new ExPerformanceCounter(base.CategoryName, "DatabaseGuid to MailboxServer Overall Cache Hit Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BackEndServerOverallCacheHitsRate);
				this.BackEndServerOverallCacheHitsRateBase = new ExPerformanceCounter(base.CategoryName, "DatabaseGuid to MailboxServer Overall Cache Hit Rate Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BackEndServerOverallCacheHitsRateBase);
				this.BackEndServerCacheLocalServerListCount = new ExPerformanceCounter(base.CategoryName, "DatabaseGuid to MailboxServer Cache Local Site MailboxServers", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BackEndServerCacheLocalServerListCount);
				this.BackEndServerCacheRefreshingQueueLength = new ExPerformanceCounter(base.CategoryName, "DatabaseGuid to MailboxServer Cache Refreshing Queue Length", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BackEndServerCacheRefreshingQueueLength);
				this.BackEndServerCacheRefreshingStatus = new ExPerformanceCounter(base.CategoryName, "DatabaseGuid to MailboxServer Cache Refreshing Status", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BackEndServerCacheRefreshingStatus);
				this.FbaModuleKeyCacheSize = new ExPerformanceCounter(base.CategoryName, "FBAModule Key Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.FbaModuleKeyCacheSize);
				this.FbaModuleKeyCacheHitsRate = new ExPerformanceCounter(base.CategoryName, "FBAModule Key Cache Hits Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.FbaModuleKeyCacheHitsRate);
				this.FbaModuleKeyCacheHitsRateBase = new ExPerformanceCounter(base.CategoryName, "FBAModule Key Cache Hits Rate Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.FbaModuleKeyCacheHitsRateBase);
				this.CookieUseRate = new ExPerformanceCounter(base.CategoryName, "AnchorMailbox to MailboxServer Cookie Hit Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CookieUseRate);
				this.CookieUseRateBase = new ExPerformanceCounter(base.CategoryName, "AnchorMailbox to MailboxServer Cookie Hit Rate Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CookieUseRateBase);
				this.OverallCacheEffectivenessRate = new ExPerformanceCounter(base.CategoryName, "Overall Cache Effectiveness (% of requests)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OverallCacheEffectivenessRate);
				this.OverallCacheEffectivenessRateBase = new ExPerformanceCounter(base.CategoryName, "Overall Cache Effectiveness (% of requests) Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OverallCacheEffectivenessRateBase);
				this.RouteRefresherSuccessfulMailboxServerCacheUpdates = new ExPerformanceCounter(base.CategoryName, "Route Refresher Mailbox Server Cache Updates", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RouteRefresherSuccessfulMailboxServerCacheUpdates);
				this.RouteRefresherTotalMailboxServerCacheUpdateAttempts = new ExPerformanceCounter(base.CategoryName, "Route Refresher Mailbox Server Cache Update Attempts", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RouteRefresherTotalMailboxServerCacheUpdateAttempts);
				this.RouteRefresherSuccessfulAnchorMailboxCacheUpdates = new ExPerformanceCounter(base.CategoryName, "Route Refresher Anchor Mailbox Cache Updates", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RouteRefresherSuccessfulAnchorMailboxCacheUpdates);
				this.RouteRefresherTotalAnchorMailboxCacheUpdateAttempts = new ExPerformanceCounter(base.CategoryName, "Route Refresher Anchor Mailbox Cache Update Attempts", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RouteRefresherTotalAnchorMailboxCacheUpdateAttempts);
				this.MovingPercentageBackEndServerLocalCacheHitsRate = new ExPerformanceCounter(base.CategoryName, "DatabaseGuid to MailboxServer Local Cache Hit Rate (Moving Average)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MovingPercentageBackEndServerLocalCacheHitsRate);
				this.MovingPercentageBackEndServerOverallCacheHitsRate = new ExPerformanceCounter(base.CategoryName, "DatabaseGuid to MailboxServer Overall Cache Hit Rate (Moving Average)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MovingPercentageBackEndServerOverallCacheHitsRate);
				this.MovingPercentageCookieUseRate = new ExPerformanceCounter(base.CategoryName, "AnchorMailbox to MailboxServer Cookie Hit Rate (Moving Average)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MovingPercentageCookieUseRate);
				long num = this.AnchorMailboxCacheSize.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		public override void GetPerfCounterDiagnosticsInfo(XElement topElement)
		{
			XElement xelement = null;
			foreach (ExPerformanceCounter exPerformanceCounter in this.counters)
			{
				try
				{
					if (xelement == null)
					{
						xelement = new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.InstanceName));
						topElement.Add(xelement);
					}
					xelement.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					topElement.Add(content);
				}
			}
		}

		public readonly ExPerformanceCounter AnchorMailboxCacheSize;

		public readonly ExPerformanceCounter AnchorMailboxLocalCacheHitsRate;

		public readonly ExPerformanceCounter AnchorMailboxLocalCacheHitsRateBase;

		public readonly ExPerformanceCounter AnchorMailboxOverallCacheHitsRate;

		public readonly ExPerformanceCounter AnchorMailboxOverallCacheHitsRateBase;

		public readonly ExPerformanceCounter NegativeAnchorMailboxCacheSize;

		public readonly ExPerformanceCounter NegativeAnchorMailboxLocalCacheHitsRate;

		public readonly ExPerformanceCounter NegativeAnchorMailboxLocalCacheHitsRateBase;

		public readonly ExPerformanceCounter BackEndServerCacheSize;

		public readonly ExPerformanceCounter BackEndServerLocalCacheHitsRate;

		public readonly ExPerformanceCounter BackEndServerLocalCacheHitsRateBase;

		public readonly ExPerformanceCounter BackEndServerOverallCacheHitsRate;

		public readonly ExPerformanceCounter BackEndServerOverallCacheHitsRateBase;

		public readonly ExPerformanceCounter BackEndServerCacheLocalServerListCount;

		public readonly ExPerformanceCounter BackEndServerCacheRefreshingQueueLength;

		public readonly ExPerformanceCounter BackEndServerCacheRefreshingStatus;

		public readonly ExPerformanceCounter FbaModuleKeyCacheSize;

		public readonly ExPerformanceCounter FbaModuleKeyCacheHitsRate;

		public readonly ExPerformanceCounter FbaModuleKeyCacheHitsRateBase;

		public readonly ExPerformanceCounter CookieUseRate;

		public readonly ExPerformanceCounter CookieUseRateBase;

		public readonly ExPerformanceCounter OverallCacheEffectivenessRate;

		public readonly ExPerformanceCounter OverallCacheEffectivenessRateBase;

		public readonly ExPerformanceCounter RouteRefresherSuccessfulMailboxServerCacheUpdates;

		public readonly ExPerformanceCounter RouteRefresherTotalMailboxServerCacheUpdateAttempts;

		public readonly ExPerformanceCounter RouteRefresherSuccessfulAnchorMailboxCacheUpdates;

		public readonly ExPerformanceCounter RouteRefresherTotalAnchorMailboxCacheUpdateAttempts;

		public readonly ExPerformanceCounter MovingPercentageBackEndServerLocalCacheHitsRate;

		public readonly ExPerformanceCounter MovingPercentageBackEndServerOverallCacheHitsRate;

		public readonly ExPerformanceCounter MovingPercentageCookieUseRate;
	}
}
