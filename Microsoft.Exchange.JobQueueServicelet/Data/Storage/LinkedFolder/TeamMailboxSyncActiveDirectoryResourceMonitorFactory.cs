using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TeamMailboxSyncActiveDirectoryResourceMonitorFactory : IResourceMonitorFactory
	{
		public IResourceMonitor Create(Guid teamMailboxMdbGuid)
		{
			return new TeamMailboxSyncActiveDirectoryResourceMonitorFactory.TeamMailboxSyncActiveDirectoryResourceMonitor();
		}

		private class TeamMailboxSyncActiveDirectoryResourceMonitor : IResourceMonitor
		{
			public void CheckResourceHealth()
			{
			}

			public DelayInfo GetDelay()
			{
				return DelayInfo.NoDelay;
			}

			public void StartChargingBudget()
			{
			}

			public void ResetBudget()
			{
			}

			public IBudget GetBudget()
			{
				return null;
			}
		}
	}
}
