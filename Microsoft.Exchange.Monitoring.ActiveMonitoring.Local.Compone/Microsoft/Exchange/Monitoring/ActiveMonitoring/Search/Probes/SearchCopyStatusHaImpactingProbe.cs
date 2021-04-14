using System;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Exchange.Search.Core;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Probes
{
	public class SearchCopyStatusHaImpactingProbe : SearchProbeBase
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
			IndexStatus indexStatus = null;
			try
			{
				indexStatus = SearchMonitoringHelper.GetCachedLocalDatabaseIndexStatus(targetResource, true);
			}
			catch (IndexStatusException ex)
			{
				base.Result.StateAttribute1 = "IndexStatusException";
				throw new SearchProbeFailureException(new LocalizedString(ex.ToString()));
			}
			base.Result.StateAttribute1 = indexStatus.IndexingState.ToString();
			if (indexStatus.IndexingState != ContentIndexStatusType.Healthy && indexStatus.IndexingState != ContentIndexStatusType.HealthyAndUpgrading && indexStatus.IndexingState != ContentIndexStatusType.AutoSuspended)
			{
				throw new SearchProbeFailureException(new LocalizedString(indexStatus.IndexingState.ToString()));
			}
		}
	}
}
