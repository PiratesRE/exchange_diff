using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Cluster.Replay.MountPoint
{
	internal class ExchangeVolumeDbMountPointsComparer : Comparer<ExchangeVolume>
	{
		public static ExchangeVolumeDbMountPointsComparer Instance
		{
			get
			{
				return ExchangeVolumeDbMountPointsComparer.s_instance;
			}
		}

		private ExchangeVolumeDbMountPointsComparer()
		{
		}

		public override int Compare(ExchangeVolume x, ExchangeVolume y)
		{
			if (!x.IsExchangeVolume && !y.IsExchangeVolume)
			{
				MountedFolderPath mostAppropriateMountPoint = x.GetMostAppropriateMountPoint();
				MountedFolderPath mostAppropriateMountPoint2 = y.GetMostAppropriateMountPoint();
				if (MountedFolderPath.IsNullOrEmpty(mostAppropriateMountPoint) && !MountedFolderPath.IsNullOrEmpty(mostAppropriateMountPoint2))
				{
					return -1;
				}
				if (!MountedFolderPath.IsNullOrEmpty(mostAppropriateMountPoint) && MountedFolderPath.IsNullOrEmpty(mostAppropriateMountPoint2))
				{
					return 1;
				}
				if (MountedFolderPath.IsNullOrEmpty(mostAppropriateMountPoint) && MountedFolderPath.IsNullOrEmpty(mostAppropriateMountPoint2))
				{
					return x.VolumeName.CompareTo(y.VolumeName);
				}
				return mostAppropriateMountPoint.CompareTo(mostAppropriateMountPoint2);
			}
			else if (x.IsExchangeVolume && y.IsExchangeVolume)
			{
				if (x.IsDatabaseMountPointsNullOrEmpty() && !y.IsDatabaseMountPointsNullOrEmpty())
				{
					return -1;
				}
				if (!x.IsDatabaseMountPointsNullOrEmpty() && y.IsDatabaseMountPointsNullOrEmpty())
				{
					return 1;
				}
				if (x.IsDatabaseMountPointsNullOrEmpty() && y.IsDatabaseMountPointsNullOrEmpty())
				{
					return x.ExchangeVolumeMountPoint.CompareTo(y.ExchangeVolumeMountPoint);
				}
				return x.DatabaseMountPoints[0].CompareTo(y.DatabaseMountPoints[0]);
			}
			else
			{
				if (!x.IsExchangeVolume && y.IsExchangeVolume)
				{
					return -1;
				}
				return 1;
			}
		}

		private static readonly ExchangeVolumeDbMountPointsComparer s_instance = new ExchangeVolumeDbMountPointsComparer();
	}
}
