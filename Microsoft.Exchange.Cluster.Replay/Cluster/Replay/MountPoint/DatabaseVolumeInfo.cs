using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay.MountPoint
{
	internal class DatabaseVolumeInfo
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.VolumeManagerTracer;
			}
		}

		public MountedFolderPath ExchangeVolumeMountPoint { get; private set; }

		public MountedFolderPath DatabaseVolumeMountPoint { get; private set; }

		public MountedFolderPath DatabaseVolumeName { get; private set; }

		public bool IsDatabasePathOnMountedFolder { get; private set; }

		public MountedFolderPath LogVolumeMountPoint { get; private set; }

		public MountedFolderPath LogVolumeName { get; private set; }

		public bool IsLogPathOnMountedFolder { get; private set; }

		public bool IsValid { get; private set; }

		public bool IsExchangeVolumeMountPointValid { get; private set; }

		public DatabaseVolumeInfoException LastException { get; private set; }

		private DatabaseVolumeInfo()
		{
			this.ExchangeVolumeMountPoint = MountedFolderPath.Empty;
		}

		public static DatabaseVolumeInfo GetInstance(string edbPath, string logPath, string databaseName, string autoDagVolumesRootFolderPath, string autoDagDatabasesRootFolderPath, int autoDagDatabaseCopiesPerVolume)
		{
			Exception ex = null;
			DatabaseVolumeInfo databaseVolumeInfo = new DatabaseVolumeInfo();
			MountedFolderPath volumePathName = MountPointUtil.GetVolumePathName(edbPath, out ex);
			if (ex != null)
			{
				DatabaseVolumeInfo.Tracer.TraceError<string, string, Exception>(0L, "DatabaseVolumeInfo.GetInstance( {0} ): GetVolumePathName() for EDB path '{1}' failed with error: {2}", databaseName, edbPath, ex);
				databaseVolumeInfo.LastException = new DatabaseVolumeInfoInitException(databaseName, ex.Message, ex);
				return databaseVolumeInfo;
			}
			databaseVolumeInfo.DatabaseVolumeMountPoint = volumePathName;
			MountedFolderPath volumeNameForVolumeMountPoint = MountPointUtil.GetVolumeNameForVolumeMountPoint(volumePathName, out ex);
			if (ex != null)
			{
				DatabaseVolumeInfo.Tracer.TraceError<string, MountedFolderPath, Exception>(0L, "DatabaseVolumeInfo.GetInstance( {0} ): GetVolumeNameForVolumeMountPoint() for EDB mount point '{1}' failed with error: {2}", databaseName, volumePathName, ex);
				databaseVolumeInfo.LastException = new DatabaseVolumeInfoInitException(databaseName, ex.Message, ex);
				return databaseVolumeInfo;
			}
			databaseVolumeInfo.DatabaseVolumeName = volumeNameForVolumeMountPoint;
			bool isDatabasePathOnMountedFolder = MountPointUtil.IsDirectoryMountPoint(volumePathName.Path, out ex);
			if (ex != null)
			{
				DatabaseVolumeInfo.Tracer.TraceError<string, MountedFolderPath, Exception>(0L, "DatabaseVolumeInfo.GetInstance( {0} ): IsDirectoryMountPoint() for EDB mount point '{1}' failed with error: {2}", databaseName, volumePathName, ex);
				databaseVolumeInfo.LastException = new DatabaseVolumeInfoInitException(databaseName, ex.Message, ex);
				return databaseVolumeInfo;
			}
			databaseVolumeInfo.IsDatabasePathOnMountedFolder = isDatabasePathOnMountedFolder;
			MountedFolderPath volumePathName2 = MountPointUtil.GetVolumePathName(logPath, out ex);
			if (ex != null)
			{
				DatabaseVolumeInfo.Tracer.TraceError<string, string, Exception>(0L, "DatabaseVolumeInfo.GetInstance( {0} ): GetVolumePathName() for LOG path '{1}' failed with error: {2}", databaseName, logPath, ex);
				databaseVolumeInfo.LastException = new DatabaseVolumeInfoInitException(databaseName, ex.Message, ex);
				return databaseVolumeInfo;
			}
			databaseVolumeInfo.LogVolumeMountPoint = volumePathName2;
			MountedFolderPath volumeNameForVolumeMountPoint2 = MountPointUtil.GetVolumeNameForVolumeMountPoint(volumePathName2, out ex);
			if (ex != null)
			{
				DatabaseVolumeInfo.Tracer.TraceError<string, MountedFolderPath, Exception>(0L, "DatabaseVolumeInfo.GetInstance( {0} ): GetVolumeNameForVolumeMountPoint() for LOG mount point '{1}' failed with error: {2}", databaseName, volumePathName2, ex);
				databaseVolumeInfo.LastException = new DatabaseVolumeInfoInitException(databaseName, ex.Message, ex);
				return databaseVolumeInfo;
			}
			databaseVolumeInfo.LogVolumeName = volumeNameForVolumeMountPoint2;
			bool isLogPathOnMountedFolder = MountPointUtil.IsDirectoryMountPoint(volumePathName2.Path, out ex);
			if (ex != null)
			{
				DatabaseVolumeInfo.Tracer.TraceError<string, MountedFolderPath, Exception>(0L, "DatabaseVolumeInfo.GetInstance( {0} ): IsDirectoryMountPoint() for LOG mount point '{1}' failed with error: {2}", databaseName, volumePathName2, ex);
				databaseVolumeInfo.LastException = new DatabaseVolumeInfoInitException(databaseName, ex.Message, ex);
				return databaseVolumeInfo;
			}
			databaseVolumeInfo.IsLogPathOnMountedFolder = isLogPathOnMountedFolder;
			if (!string.IsNullOrEmpty(autoDagVolumesRootFolderPath) && !string.IsNullOrEmpty(autoDagDatabasesRootFolderPath))
			{
				ExchangeVolume instance = ExchangeVolume.GetInstance(volumeNameForVolumeMountPoint, autoDagVolumesRootFolderPath, autoDagDatabasesRootFolderPath, autoDagDatabaseCopiesPerVolume);
				if (instance.IsValid)
				{
					databaseVolumeInfo.ExchangeVolumeMountPoint = (instance.ExchangeVolumeMountPoint ?? MountedFolderPath.Empty);
					databaseVolumeInfo.IsExchangeVolumeMountPointValid = !MountedFolderPath.IsNullOrEmpty(databaseVolumeInfo.ExchangeVolumeMountPoint);
				}
			}
			databaseVolumeInfo.IsValid = true;
			return databaseVolumeInfo;
		}

		public static DatabaseVolumeInfo GetInstance(IReplayConfiguration config)
		{
			return DatabaseVolumeInfo.GetInstance(config.DestinationEdbPath, config.DestinationLogPath, config.DisplayName, config.AutoDagVolumesRootFolderPath, config.AutoDagDatabasesRootFolderPath, config.AutoDagDatabaseCopiesPerVolume);
		}
	}
}
