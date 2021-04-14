using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Probes
{
	public class SearchMountedCopyStatusProbe : SearchProbeBase
	{
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

		protected override void InternalDoWork(CancellationToken cancellationToken)
		{
			string targetResource = base.Definition.TargetResource;
			IndexStatus cachedLocalDatabaseIndexStatus = SearchMonitoringHelper.GetCachedLocalDatabaseIndexStatus(targetResource, true);
			int @int = base.AttributeHelper.GetInt("MailboxesToCrawlThreshold", true, 0, null, null);
			if (cachedLocalDatabaseIndexStatus.IndexingState == ContentIndexStatusType.Crawling)
			{
				base.Result.StateAttribute1 = cachedLocalDatabaseIndexStatus.MailboxesToCrawl.ToString();
				if (cachedLocalDatabaseIndexStatus.MailboxesToCrawl > @int)
				{
					RpcDatabaseCopyStatus2 healthyNonLagCopy = this.GetHealthyNonLagCopy(targetResource);
					if (healthyNonLagCopy != null)
					{
						string allCopyStatusForDatabaseString = SearchMonitoringHelper.GetAllCopyStatusForDatabaseString(targetResource);
						throw new SearchProbeFailureException(Strings.SearchIndexDatabaseCopyStatus(Strings.SearchIndexCrawlingWithHealthyCopy(targetResource, healthyNonLagCopy.MailboxServer), allCopyStatusForDatabaseString));
					}
				}
			}
			if (cachedLocalDatabaseIndexStatus.IndexingState == ContentIndexStatusType.Seeding)
			{
				RpcDatabaseCopyStatus2 healthyNonLagCopy2 = this.GetHealthyNonLagCopy(targetResource);
				if (healthyNonLagCopy2 != null)
				{
					string allCopyStatusForDatabaseString2 = SearchMonitoringHelper.GetAllCopyStatusForDatabaseString(targetResource);
					throw new SearchProbeFailureException(Strings.SearchIndexDatabaseCopyStatus(Strings.SearchIndexActiveCopySeedingWithHealthyCopy(targetResource, healthyNonLagCopy2.MailboxServer), allCopyStatusForDatabaseString2));
				}
			}
			if (cachedLocalDatabaseIndexStatus.IndexingState == ContentIndexStatusType.Disabled || cachedLocalDatabaseIndexStatus.IndexingState == ContentIndexStatusType.FailedAndSuspended || cachedLocalDatabaseIndexStatus.IndexingState == ContentIndexStatusType.AutoSuspended)
			{
				string allCopyStatusForDatabaseString3 = SearchMonitoringHelper.GetAllCopyStatusForDatabaseString(targetResource);
				throw new SearchProbeFailureException(Strings.SearchIndexDatabaseCopyStatus(Strings.SearchIndexActiveCopyNotIndxed(targetResource, cachedLocalDatabaseIndexStatus.IndexingState.ToString()), allCopyStatusForDatabaseString3));
			}
		}

		private RpcDatabaseCopyStatus2 GetHealthyNonLagCopy(string databaseName)
		{
			List<CopyStatusClientCachedEntry> cachedDatabaseCopyStatus = SearchMonitoringHelper.GetCachedDatabaseCopyStatus(databaseName);
			if (cachedDatabaseCopyStatus == null)
			{
				base.Result.StateAttribute3 = "OtherCopyStatusNull";
				return null;
			}
			foreach (CopyStatusClientCachedEntry copyStatusClientCachedEntry in cachedDatabaseCopyStatus)
			{
				RpcDatabaseCopyStatus2 copyStatus = copyStatusClientCachedEntry.CopyStatus;
				if ((copyStatus != null && copyStatus.ContentIndexStatus == ContentIndexStatusType.Healthy) || copyStatus.ContentIndexStatus == ContentIndexStatusType.HealthyAndUpgrading || copyStatus.ContentIndexStatus == ContentIndexStatusType.AutoSuspended)
				{
					if (copyStatus.ReplayLagEnabled != ReplayLagEnabledEnum.Enabled)
					{
						return copyStatus;
					}
					base.Result.StateAttribute2 = "Healthy Lag Copy: " + copyStatus.MailboxServer;
				}
			}
			return null;
		}
	}
}
