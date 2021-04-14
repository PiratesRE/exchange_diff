using System;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	internal sealed class ArbitrationConfigFromAD
	{
		public ArbitrationConfigFromAD(SyncServiceInstance syncDaemonArbitrationConfig, RidMasterInfo ridMasterInfo)
		{
			this.SyncDaemonArbitrationConfig = syncDaemonArbitrationConfig;
			this.RidMasterInfo = ridMasterInfo;
			this.PassiveInstanceSleepInterval = TimeSpan.FromSeconds((double)this.SyncDaemonArbitrationConfig.PassiveInstanceSleepInterval);
			this.ActiveInstanceSleepInterval = TimeSpan.FromSeconds((double)this.SyncDaemonArbitrationConfig.ActiveInstanceSleepInterval);
		}

		public TimeSpan ActiveInstanceSleepInterval { get; private set; }

		public TimeSpan PassiveInstanceSleepInterval { get; private set; }

		public Guid CurrentSiteGuid
		{
			get
			{
				return ArbitrationConfigFromAD.SiteGuid.Value;
			}
		}

		public SyncServiceInstance SyncDaemonArbitrationConfig { get; private set; }

		public RidMasterInfo RidMasterInfo { get; private set; }

		private static readonly Lazy<Guid> SiteGuid = new Lazy<Guid>(() => LocalSiteCache.LocalSite.Guid);
	}
}
