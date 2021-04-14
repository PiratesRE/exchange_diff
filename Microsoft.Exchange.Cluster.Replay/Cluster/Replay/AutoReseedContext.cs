using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Replay.MountPoint;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class AutoReseedContext
	{
		public IADDatabase Database { get; private set; }

		public IEnumerable<IADDatabase> Databases { get; private set; }

		public AmServerName TargetServerName { get; private set; }

		public IADServer TargetServer { get; private set; }

		public CopyStatusClientCachedEntry TargetCopyStatus { get; private set; }

		public IEnumerable<CopyStatusClientCachedEntry> CopyStatusesForTargetDatabase { get; private set; }

		public IEnumerable<CopyStatusClientCachedEntry> CopyStatusesForTargetServer { get; private set; }

		public Dictionary<AmServerName, IEnumerable<CopyStatusClientCachedEntry>> CopyStatusesForDag { get; private set; }

		public IADDatabaseAvailabilityGroup Dag { get; private set; }

		public IVolumeManager VolumeManager { get; private set; }

		public IReplicaInstanceManager ReplicaInstanceManager { get; private set; }

		public AutoReseedServerLimiter ReseedLimiter { get; private set; }

		public IMonitoringADConfig AdConfig { get; private set; }

		public AutoReseedContext(IVolumeManager volumeManager, IReplicaInstanceManager replicaInstanceManager, IADDatabaseAvailabilityGroup dag, IADDatabase database, IEnumerable<IADDatabase> databases, AmServerName targetServerName, IADServer targetServer, CopyStatusClientCachedEntry targetCopyStatus, IEnumerable<CopyStatusClientCachedEntry> dbCopyStatuses, IEnumerable<CopyStatusClientCachedEntry> serverCopyStatuses, Dictionary<AmServerName, IEnumerable<CopyStatusClientCachedEntry>> dagCopyStatuses, AutoReseedServerLimiter reseedLimiter, IMonitoringADConfig adConfig)
		{
			this.VolumeManager = volumeManager;
			this.ReplicaInstanceManager = replicaInstanceManager;
			this.Dag = dag;
			this.Database = database;
			this.Databases = databases;
			this.TargetServerName = targetServerName;
			this.TargetServer = targetServer;
			this.TargetCopyStatus = targetCopyStatus;
			this.CopyStatusesForTargetDatabase = dbCopyStatuses;
			this.CopyStatusesForTargetServer = serverCopyStatuses;
			this.CopyStatusesForDag = dagCopyStatuses;
			this.ReseedLimiter = reseedLimiter;
			this.AdConfig = adConfig;
		}
	}
}
