using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Probes
{
	public class SearchSingleCopyProbe : SearchProbeBase
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
			int @int = base.AttributeHelper.GetInt("SeedingAllowedMinutes", false, 90, null, null);
			List<CopyStatusClientCachedEntry> cachedDatabaseCopyStatus = SearchMonitoringHelper.GetCachedDatabaseCopyStatus(targetResource);
			if (cachedDatabaseCopyStatus == null)
			{
				base.Result.StateAttribute1 = "CopyStatusNull";
				return;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (CopyStatusClientCachedEntry copyStatusClientCachedEntry in cachedDatabaseCopyStatus)
			{
				if (copyStatusClientCachedEntry.Result != CopyStatusRpcResult.Success)
				{
					string copyName = string.Format("{0}\\{1}", targetResource, copyStatusClientCachedEntry.ServerContacted.NetbiosName);
					stringBuilder.AppendLine(Strings.SearchIndexCopyStatusError(copyName, copyStatusClientCachedEntry.Result.ToString(), (copyStatusClientCachedEntry.LastException != null) ? copyStatusClientCachedEntry.LastException.Message : string.Empty));
				}
				else
				{
					RpcDatabaseCopyStatus2 copyStatus = copyStatusClientCachedEntry.CopyStatus;
					num6++;
					if (copyStatus.CopyStatus == CopyStatusEnum.Mounted)
					{
						flag = true;
						num5++;
					}
					else if (copyStatus.CopyStatus == CopyStatusEnum.Healthy)
					{
						num5++;
					}
					if (copyStatus.ContentIndexStatus == ContentIndexStatusType.Healthy || copyStatus.ContentIndexStatus == ContentIndexStatusType.HealthyAndUpgrading)
					{
						num++;
					}
					else if (copyStatus.ContentIndexStatus == ContentIndexStatusType.Crawling)
					{
						num2++;
						string mailboxServer = copyStatus.MailboxServer;
					}
					else if (copyStatus.ContentIndexStatus == ContentIndexStatusType.AutoSuspended)
					{
						num3++;
					}
					else if (copyStatus.ContentIndexStatus == ContentIndexStatusType.Seeding)
					{
						num4++;
					}
					string copyName = string.Format("{0}\\{1}", copyStatus.DBName, copyStatus.MailboxServer);
					stringBuilder.AppendLine(Strings.SearchIndexCopyStatus(copyName, copyStatus.CopyStatus.ToString(), copyStatus.ContentIndexStatus.ToString(), copyStatus.ContentIndexErrorMessage));
				}
			}
			base.Result.StateAttribute1 = stringBuilder.ToString();
			if (!flag)
			{
				return;
			}
			if (num == 1 && num6 > 1)
			{
				if (num5 == 2 && num3 == 1)
				{
					return;
				}
				if (num5 <= 1)
				{
					return;
				}
				ProbeResult lastProbeResult = SearchMonitoringHelper.GetLastProbeResult(this, base.Broker, cancellationToken);
				DateTime dateTime = DateTime.UtcNow;
				if (lastProbeResult != null && !string.IsNullOrEmpty(lastProbeResult.StateAttribute2))
				{
					dateTime = DateTime.Parse(lastProbeResult.StateAttribute2);
					base.Result.StateAttribute2 = lastProbeResult.StateAttribute2;
				}
				if (num4 <= 0)
				{
					throw new SearchProbeFailureException(Strings.SearchIndexSingleHealthyCopy(targetResource, stringBuilder.ToString()));
				}
				base.Result.StateAttribute2 = dateTime.ToString();
				if (DateTime.UtcNow > dateTime.AddMinutes((double)@int))
				{
					throw new SearchProbeFailureException(Strings.SearchIndexSingleHealthyCopyWithSeeding(targetResource, stringBuilder.ToString(), dateTime.ToString()));
				}
				return;
			}
			else
			{
				if (num2 > 1)
				{
					throw new SearchProbeFailureException(Strings.SearchIndexMultiCrawling(targetResource, num2, stringBuilder.ToString()));
				}
				return;
			}
		}
	}
}
