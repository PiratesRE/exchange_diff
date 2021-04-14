using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.AnchorService
{
	internal class AnchorIssueCache : ServiceIssueCache
	{
		public AnchorIssueCache(AnchorContext context, JobCache cache)
		{
			this.Context = context;
			this.Cache = cache;
		}

		public override bool ScanningIsEnabled
		{
			get
			{
				return this.Context.Config.GetConfig<bool>("IssueCacheIsEnabled");
			}
		}

		protected override TimeSpan FullScanFrequency
		{
			get
			{
				return this.Context.Config.GetConfig<TimeSpan>("IssueCacheScanFrequency");
			}
		}

		protected override int IssueLimit
		{
			get
			{
				return this.Context.Config.GetConfig<int>("IssueCacheItemLimit");
			}
		}

		private AnchorContext Context { get; set; }

		private JobCache Cache { get; set; }

		internal static bool TrySendEventNotification(AnchorContext context, string notificationReason, string message, ResultSeverityLevel severity = ResultSeverityLevel.Error)
		{
			if (!context.Config.GetConfig<bool>("IssueCacheIsEnabled"))
			{
				return false;
			}
			string config = context.Config.GetConfig<string>("MonitoringComponentName");
			if (string.IsNullOrEmpty(config))
			{
				return false;
			}
			Component a = Component.FindWellKnownComponent(config);
			if (a == null)
			{
				return false;
			}
			new EventNotificationItem(config, config, notificationReason, message, severity).Publish(false);
			return true;
		}

		protected override ICollection<ServiceIssue> RunFullIssueScan()
		{
			ICollection<ServiceIssue> collection = new List<ServiceIssue>();
			foreach (CacheEntryBase cacheEntryBase in this.Cache.Get())
			{
				if (cacheEntryBase.ServiceException != null)
				{
					collection.Add(new DiagnosableServiceIssue(cacheEntryBase, cacheEntryBase.ServiceException.ToString()));
				}
			}
			return collection;
		}

		public const string CacheEntryIsPoisonedNotification = "CacheEntryIsPoisoned";
	}
}
