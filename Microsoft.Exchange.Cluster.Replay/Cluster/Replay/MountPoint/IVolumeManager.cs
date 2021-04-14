using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay.MountPoint
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IVolumeManager
	{
		IEnumerable<ExchangeVolume> Volumes { get; }

		ExchangeVolume AssignSpare(DatabaseSpareInfo[] dbInfos);

		ExchangeVolume FixupMountPointForDatabase(DatabaseSpareInfo dbInfo, MountedFolderPath volumeToAssign);

		bool FixActiveDatabaseMountPoint(IADDatabase database, IEnumerable<IADDatabase> databases, IMonitoringADConfig adConfig, out Exception exception, bool checkDatabaseGroupExists = true);

		void UpdateVolumeInfoCopyState(Guid guid, IReplicaInstanceManager rim);
	}
}
