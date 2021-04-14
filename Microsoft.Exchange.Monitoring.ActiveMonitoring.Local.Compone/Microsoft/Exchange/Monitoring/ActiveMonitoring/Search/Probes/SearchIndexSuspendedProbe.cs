using System;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Probes
{
	public class SearchIndexSuspendedProbe : SearchProbeBase
	{
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
			IndexStatus cachedLocalDatabaseIndexStatus = SearchMonitoringHelper.GetCachedLocalDatabaseIndexStatus(targetResource, false);
			if (cachedLocalDatabaseIndexStatus == null)
			{
				return;
			}
			base.Result.StateAttribute1 = cachedLocalDatabaseIndexStatus.IndexingState.ToString();
			if (cachedLocalDatabaseIndexStatus.IndexingState == ContentIndexStatusType.Suspended)
			{
				CopyStatusClientCachedEntry cachedLocalDatabaseCopyStatus = SearchMonitoringHelper.GetCachedLocalDatabaseCopyStatus(targetResource);
				if (cachedLocalDatabaseCopyStatus == null || cachedLocalDatabaseCopyStatus.CopyStatus == null)
				{
					base.Result.StateAttribute2 = "CopyStatusNull";
					return;
				}
				base.Result.StateAttribute2 = cachedLocalDatabaseCopyStatus.CopyStatus.CopyStatus.ToString();
				if (cachedLocalDatabaseCopyStatus.CopyStatus.CopyStatus == CopyStatusEnum.Mounted || cachedLocalDatabaseCopyStatus.CopyStatus.CopyStatus == CopyStatusEnum.Healthy)
				{
					throw new SearchProbeFailureException(Strings.SearchCatalogSuspended(targetResource, cachedLocalDatabaseCopyStatus.CopyStatus.CopyStatus.ToString()));
				}
			}
		}
	}
}
