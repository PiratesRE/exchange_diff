using System;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Common.Bitlocker.Utilities;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay.MountPoint
{
	internal class ExchangeVolume
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.VolumeManagerTracer;
			}
		}

		public MountedFolderPath VolumeName
		{
			get
			{
				return this.m_volumeName;
			}
		}

		public DriveType DriveType { get; private set; }

		public bool IsExchangeVolume { get; private set; }

		public bool IsAvailableAsSpare { get; private set; }

		public MountedFolderPath ExchangeVolumeMountPoint { get; private set; }

		public string DatabasesRootPath
		{
			get
			{
				return this.m_databasesRootPath;
			}
		}

		public string VolumeLabel { get; private set; }

		public MountedFolderPath[] DatabaseMountPoints { get; private set; }

		public MountedFolderPath[] MountPoints { get; private set; }

		public bool IsValid { get; private set; }

		public DatabaseVolumeInfoException LastException { get; private set; }

		private string ExchangeVolumesRootPath
		{
			get
			{
				return this.m_exchangeVolumesRootPath;
			}
		}

		private int NumDbsPerVolume
		{
			get
			{
				return this.m_numDbsPerVolume;
			}
		}

		private ExchangeVolume(MountedFolderPath volumeName, string exchangeVolumesRootPath, string databasesRootPath, int numDbsPerVolume)
		{
			this.m_volumeName = volumeName;
			this.m_exchangeVolumesRootPath = exchangeVolumesRootPath;
			this.m_databasesRootPath = databasesRootPath;
			this.m_numDbsPerVolume = numDbsPerVolume;
		}

		public static ExchangeVolume GetInstance(MountedFolderPath volumeName, string exchangeVolumesRootPath, string databasesRootPath, int numDbsPerVolume)
		{
			ExchangeVolume exchangeVolume = new ExchangeVolume(volumeName, exchangeVolumesRootPath, databasesRootPath, numDbsPerVolume);
			exchangeVolume.Refresh();
			return exchangeVolume;
		}

		public void Refresh()
		{
			Exception ex = null;
			this.Init();
			DriveType driveType = NativeMethods.GetDriveType(this.VolumeName.Path);
			this.DriveType = driveType;
			if (this.DriveType != DriveType.Fixed)
			{
				ExchangeVolume.Tracer.TraceError<MountedFolderPath, DriveType>((long)this.GetHashCode(), "ExchangeVolume.GetInstance( {0} ): Volume is not a Fixed DriveType. Actual: {1}", this.VolumeName, this.DriveType);
				this.IsValid = true;
				return;
			}
			string volumeLabel = MountPointUtil.GetVolumeLabel(this.VolumeName, out ex);
			if (ex != null)
			{
				ExchangeVolume.Tracer.TraceError<MountedFolderPath, Exception>((long)this.GetHashCode(), "ExchangeVolume.GetInstance( {0} ): Could not retrieve volume label. Error - ", this.VolumeName, ex);
				ex = null;
			}
			else
			{
				ExchangeVolume.Tracer.TraceDebug<MountedFolderPath, string>((long)this.GetHashCode(), "ExchangeVolume.GetInstance( {0} ): Volume has a label '{1}'.", this.VolumeName, volumeLabel);
				this.VolumeLabel = volumeLabel;
			}
			MountedFolderPath[] volumePathNamesForVolumeName = MountPointUtil.GetVolumePathNamesForVolumeName(this.VolumeName, out ex);
			if (ex != null)
			{
				ExchangeVolume.Tracer.TraceError<MountedFolderPath, Exception>((long)this.GetHashCode(), "ExchangeVolume.GetInstance( {0} ): GetVolumePathNamesForVolumeName() failed with error: {1}", this.VolumeName, ex);
				this.LastException = new ExchangeVolumeInfoInitException(this.VolumeName.Path, ex.Message, ex);
				return;
			}
			this.MountPoints = volumePathNamesForVolumeName;
			MountedFolderPath[] array = (from mp in volumePathNamesForVolumeName
			where MountPointUtil.IsPathDirectlyUnderParentPath(mp.Path, this.ExchangeVolumesRootPath, out ex) && ex == null
			select mp).ToArray<MountedFolderPath>();
			if (ex != null)
			{
				ExchangeVolume.Tracer.TraceError<MountedFolderPath, Exception>((long)this.GetHashCode(), "ExchangeVolume.GetInstance( {0} ): IsPathDirectlyUnderParentPath() for ExchangeVolumeMountPoints failed with error: {1}", this.VolumeName, ex);
				this.LastException = new ExchangeVolumeInfoInitException(this.VolumeName.Path, ex.Message, ex);
				return;
			}
			int num = array.Length;
			if (num > 0)
			{
				this.IsExchangeVolume = true;
				this.ExchangeVolumeMountPoint = array[0];
				if (num > 1)
				{
					string text = string.Join(", ", from mp in array
					select mp.Path);
					ExchangeVolume.Tracer.TraceError<MountedFolderPath, string, string>((long)this.GetHashCode(), "ExchangeVolume.GetInstance( {0} ): Multiple mount points found under '{1}': {2}", this.VolumeName, this.ExchangeVolumesRootPath, text);
					this.LastException = new ExchangeVolumeInfoMultipleExMountPointsException(this.VolumeName.Path, this.ExchangeVolumesRootPath, text);
					return;
				}
			}
			MountedFolderPath[] array2 = (from mp in volumePathNamesForVolumeName
			where MountPointUtil.IsPathDirectlyUnderParentPath(mp.Path, this.DatabasesRootPath, out ex) && ex == null
			orderby mp
			select mp).ToArray<MountedFolderPath>();
			if (ex != null)
			{
				ExchangeVolume.Tracer.TraceError<MountedFolderPath, Exception>((long)this.GetHashCode(), "ExchangeVolume.GetInstance( {0} ): IsPathDirectlyUnderParentPath() for DatabaseMountPoints failed with error: {1}", this.VolumeName, ex);
				this.LastException = new ExchangeVolumeInfoInitException(this.VolumeName.Path, ex.Message, ex);
				return;
			}
			int num2 = array2.Length;
			if (num2 < this.NumDbsPerVolume)
			{
				if (this.IsExchangeVolume)
				{
					Exception ex2;
					VolumeSpareStatus spareStatus = this.GetSpareStatus(out ex2);
					if (spareStatus == VolumeSpareStatus.EncryptingEmptySpare && ex2 == null)
					{
						ExchangeVolume.Tracer.TraceDebug<MountedFolderPath, int, int>((long)this.GetHashCode(), "ExchangeVolume.GetInstance( {0} ): Volume has {1} Database mount points. It should have {2}. But volume is getting Encrypted. Not setting as spare.", this.VolumeName, num2, this.NumDbsPerVolume);
						this.IsAvailableAsSpare = false;
					}
					else
					{
						ExchangeVolume.Tracer.TraceDebug<MountedFolderPath, int, int>((long)this.GetHashCode(), "ExchangeVolume.GetInstance( {0} ): Volume has {1} Database mount points. It should have {2}. Setting as spare.", this.VolumeName, num2, this.NumDbsPerVolume);
						this.IsAvailableAsSpare = true;
					}
				}
			}
			else
			{
				if (num2 > this.NumDbsPerVolume)
				{
					string text2 = string.Join(", ", from mp in array2
					select mp.Path);
					ExchangeVolume.Tracer.TraceError((long)this.GetHashCode(), "ExchangeVolume.GetInstance( {0} ): Volume has {1} Database mount points, but should only have MAX of {2}: {3}", new object[]
					{
						this.VolumeName,
						num2,
						this.NumDbsPerVolume,
						text2
					});
					this.LastException = new ExchangeVolumeInfoMultipleDbMountPointsException(this.VolumeName.Path, this.DatabasesRootPath, text2, this.NumDbsPerVolume);
					return;
				}
				ExchangeVolume.Tracer.TraceDebug<MountedFolderPath, int>((long)this.GetHashCode(), "ExchangeVolume.GetInstance( {0} ): Volume has expected {1} Database mount points.", this.VolumeName, num2);
			}
			this.DatabaseMountPoints = array2;
			this.IsValid = true;
		}

		public VolumeSpareStatus GetSpareStatus(out Exception exception)
		{
			exception = null;
			ExchangeVolume.Tracer.TraceDebug<MountedFolderPath>((long)this.GetHashCode(), "ExchangeVolume.GetSpareStatus(): Computing status of volume '{0}'.", this.VolumeName);
			if (!this.IsValid)
			{
				ExchangeVolume.Tracer.TraceError<DatabaseVolumeInfoException>((long)this.GetHashCode(), "ExchangeVolume.GetSpareStatus(): Returning Error because the ExchangeVolume instance is invalid. Error: {0}", this.LastException);
				exception = this.LastException;
				return VolumeSpareStatus.Error;
			}
			if (!this.IsExchangeVolume || !this.IsAvailableAsSpare || this.DatabaseMountPoints.Length != 0)
			{
				ExchangeVolume.Tracer.TraceDebug<MountedFolderPath>((long)this.GetHashCode(), "ExchangeVolume.GetSpareStatus(): Returning Volume '{0}' is NotUsableAsSpare.", this.VolumeName);
				return VolumeSpareStatus.NotUsableAsSpare;
			}
			ExchangeVolume.Tracer.TraceDebug((long)this.GetHashCode(), "ExchangeVolume.GetSpareStatus(): Found a potential spare volume...");
			MountedFolderPath exchangeVolumeMountPoint = this.ExchangeVolumeMountPoint;
			if (MountPointUtil.IsDirectoryNonExistentOrEmpty(exchangeVolumeMountPoint.Path, out exception))
			{
				ExchangeVolume.Tracer.TraceDebug<MountedFolderPath>((long)this.GetHashCode(), "ExchangeVolume.GetSpareStatus(): Returning Volume '{0}' is EmptySpare.", this.VolumeName);
				bool flag = BitlockerUtil.IsVolumeMountedOnVirtualDisk(this.VolumeName.Path, out exception);
				if (exception != null)
				{
					ExchangeVolume.Tracer.TraceDebug<MountedFolderPath, Exception>((long)this.GetHashCode(), "ExchangeVolume.GetSpareStatus(): Exception finding whether Volume '{0}' is mounted on a virtual disk or not. Reason {1}", this.VolumeName, exception);
				}
				if (!flag)
				{
					bool flag2 = false;
					bool flag3 = false;
					exception = BitlockerUtil.IsVolumeEncryptedOrEncrypting(this.VolumeName.Path, out flag2, out flag3);
					if (exception != null)
					{
						ExchangeVolume.Tracer.TraceDebug<MountedFolderPath, Exception>((long)this.GetHashCode(), "ExchangeVolume.GetSpareStatus(): Exception finding whether Volume '{0}' is encrypting or not. Reason {1}", this.VolumeName, exception);
					}
					else
					{
						if (flag2)
						{
							return VolumeSpareStatus.EncryptingEmptySpare;
						}
						if (flag3)
						{
							return VolumeSpareStatus.EncryptedEmptySpare;
						}
						string arg;
						string arg2;
						bool flag4 = BitlockerUtil.IsEncryptionPausedDueToBadBlocks(this.VolumeName.Path, out exception, out arg, out arg2);
						if (exception != null)
						{
							ExchangeVolume.Tracer.TraceDebug<MountedFolderPath, Exception>((long)this.GetHashCode(), "ExchangeVolume.GetSpareStatus(): Exception finding whether Volume '{0}' has encryption paused due to bad blocks. Reason {1}", this.VolumeName, exception);
							return VolumeSpareStatus.Error;
						}
						if (flag4)
						{
							ExchangeVolume.Tracer.TraceDebug<MountedFolderPath, string, string>((long)this.GetHashCode(), "ExchangeVolume.GetSpareStatus(): Returning Volume '{0}' is Qurantined due to encryption paused due to bad blocks. Mount point {1}. Event Xml {2}", this.VolumeName, arg, arg2);
							return VolumeSpareStatus.Quarantined;
						}
					}
				}
				else
				{
					ExchangeVolume.Tracer.TraceDebug<MountedFolderPath>((long)this.GetHashCode(), "ExchangeVolume.GetSpareStatus(): Volume mounted on virtual disk. Not attempting to find bitlocker spare status", this.VolumeName);
				}
				return VolumeSpareStatus.UnEncryptedEmptySpare;
			}
			int num = 0;
			if (exception == null || FileOperations.IsCorruptedIOException((IOException)exception, out num))
			{
				ExchangeVolume.Tracer.TraceDebug<MountedFolderPath, string>((long)this.GetHashCode(), "ExchangeVolume.GetSpareStatus(): Returning Volume '{0}' is Quarantined because it has some files/directories under mountPath '{1}'.", this.VolumeName, exchangeVolumeMountPoint.Path);
				return VolumeSpareStatus.Quarantined;
			}
			ExchangeVolume.Tracer.TraceError<string, Exception>((long)this.GetHashCode(), "ExchangeVolume.GetSpareStatus(): Returning Error because IsDirectoryNonExistentOrEmpty() returned exception for mountPath '{0}'. Exception: {1}", exchangeVolumeMountPoint.Path, exception);
			return VolumeSpareStatus.Error;
		}

		public bool IsVolumeMissingDatabaseMountPoints(out Exception exception)
		{
			bool flag = true;
			int num = -1;
			exception = null;
			if (!this.IsValid)
			{
				if (this.LastException == null)
				{
					exception = new InvalidVolumeMissingException(this.VolumeName.Path);
				}
				else
				{
					exception = this.LastException;
				}
				ExchangeVolume.Tracer.TraceError<MountedFolderPath, Exception>((long)this.GetHashCode(), "ExchangeVolume.IsVolumeMissingDatabaseMountPoints(): Volume '{0}' is either not valid. Error: {1}", this.VolumeName, exception);
				return flag;
			}
			if (this.DatabaseMountPoints != null)
			{
				num = this.DatabaseMountPoints.Length;
				if (num >= this.m_numDbsPerVolume)
				{
					flag = false;
				}
			}
			ExchangeVolume.Tracer.TraceDebug((long)this.GetHashCode(), "ExchangeVolume.IsVolumeMissingDatabaseMountPoints(): Volume '{0}' is supposed to have {1} database mountpoints but only found {2}. Result: {3}.", new object[]
			{
				this.VolumeName,
				this.m_numDbsPerVolume,
				num,
				flag
			});
			return flag;
		}

		public MountedFolderPath GetMostAppropriateMountPoint()
		{
			if (this.IsExchangeVolume && !MountedFolderPath.IsNullOrEmpty(this.ExchangeVolumeMountPoint))
			{
				return this.ExchangeVolumeMountPoint;
			}
			if (this.MountPoints == null || this.MountPoints.Length == 0)
			{
				return MountedFolderPath.Empty;
			}
			return this.MountPoints[0];
		}

		public bool IsDatabaseMountPointsNullOrEmpty()
		{
			return this.DatabaseMountPoints == null || this.DatabaseMountPoints.Length == 0;
		}

		private void Init()
		{
			this.IsValid = false;
			this.LastException = null;
			this.DriveType = DriveType.Unknown;
			this.IsExchangeVolume = false;
			this.IsAvailableAsSpare = false;
			this.ExchangeVolumeMountPoint = MountedFolderPath.Empty;
			this.DatabaseMountPoints = new MountedFolderPath[0];
			this.MountPoints = new MountedFolderPath[0];
		}

		private readonly MountedFolderPath m_volumeName;

		private readonly string m_exchangeVolumesRootPath;

		private readonly string m_databasesRootPath;

		private readonly int m_numDbsPerVolume;
	}
}
