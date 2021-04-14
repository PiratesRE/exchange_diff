using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Storage.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TeamMailboxSyncResourceMonitorFactory : IResourceMonitorFactory
	{
		public IResourceMonitor Create(Guid teamMailboxMdbGuid)
		{
			if (teamMailboxMdbGuid == Guid.Empty)
			{
				throw new ArgumentNullException("teamMailboxMdbGuid");
			}
			return new TeamMailboxSyncResourceMonitorFactory.TeamMailboxSyncResourceMonitor(new ResourceKey[]
			{
				ProcessorResourceKey.Local,
				new MdbReplicationResourceHealthMonitorKey(teamMailboxMdbGuid),
				new MdbResourceHealthMonitorKey(teamMailboxMdbGuid)
			}, StandardBudget.Acquire(new UnthrottledBudgetKey("TeamMailboxSync", BudgetType.ResourceTracking)));
		}

		private const string BudgetKey = "TeamMailboxSync";

		private class TeamMailboxSyncResourceMonitor : IResourceMonitor
		{
			public TeamMailboxSyncResourceMonitor(ResourceKey[] resourcesToAccess, IBudget ibudget)
			{
				this.resourcesToAccess = resourcesToAccess;
				this.ibudget = ibudget;
			}

			public void CheckResourceHealth()
			{
				ResourceLoadDelayInfo.CheckResourceHealth(this.ibudget, TeamMailboxSyncResourceMonitorFactory.TeamMailboxSyncResourceMonitor.workloadSettings, this.resourcesToAccess);
			}

			public DelayInfo GetDelay()
			{
				return ResourceLoadDelayInfo.GetDelay(this.ibudget, TeamMailboxSyncResourceMonitorFactory.TeamMailboxSyncResourceMonitor.workloadSettings, this.resourcesToAccess, true);
			}

			public void StartChargingBudget()
			{
				this.ibudget.StartLocal("TeamMailboxSyncResourceMonitorFactory.StartChargingBudget", default(TimeSpan));
			}

			public void ResetBudget()
			{
				this.ibudget.EndLocal();
			}

			public IBudget GetBudget()
			{
				return this.ibudget;
			}

			private static readonly WorkloadSettings workloadSettings = new WorkloadSettings(WorkloadType.TeamMailboxSync, true);

			private readonly ResourceKey[] resourcesToAccess;

			private readonly IBudget ibudget;
		}
	}
}
