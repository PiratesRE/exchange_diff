using System;

namespace Microsoft.Exchange.Cluster.Replay.MountPoint
{
	internal struct DatabaseSpareInfo
	{
		public DatabaseSpareInfo(string dbName, MountedFolderPath dbMountPoint)
		{
			this.DbName = dbName;
			this.DatabaseMountPoint = dbMountPoint;
		}

		public string DbName;

		public MountedFolderPath DatabaseMountPoint;
	}
}
