using System;
using System.Collections.Generic;
using System.Management;
using Microsoft.Exchange.Cluster.Replay.MountPoint;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Common.Bitlocker.Utilities;
using Microsoft.Exchange.Common.DiskManagement.Utilities;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class BitlockerHelper
	{
		public static void EncryptEmptyWin7Volumes(VolumeManager volumeManager, bool fipsCompliant)
		{
			if (Util.IsOperatingSystemWin8OrHigher())
			{
				ReplayCrimsonEvents.Win7EncryptionMethodCalledByWin8.Log();
				return;
			}
			List<ExchangeVolume>[] array = volumeManager.DetermineVolumeSpareStatuses();
			List<ExchangeVolume> list = array[1];
			if (list == null || list.Count == 0)
			{
				ReplayCrimsonEvents.NoEmptySpaceVolumesFoundToEncrypt.Log();
				return;
			}
			Exception ex;
			List<string> encryptionPausedVolumes = BitlockerUtil.GetEncryptionPausedVolumes(out ex);
			if (ex != null || (encryptionPausedVolumes != null && encryptionPausedVolumes.Count > 0))
			{
				return;
			}
			List<string> encryptingVolumes = BitlockerUtil.GetEncryptingVolumes(out ex);
			if (ex != null || (encryptingVolumes != null && encryptingVolumes.Count > 0))
			{
				return;
			}
			ReplayCrimsonEvents.CandidatesForEncryption.Log<int>(list.Count);
			int num = 0;
			int num2 = 0;
			foreach (ExchangeVolume exchangeVolume in list)
			{
				string path = exchangeVolume.VolumeName.Path;
				Exception ex2;
				ManagementObject encryptableVolume = BitlockerUtil.GetEncryptableVolume(path, out ex2);
				if (encryptableVolume == null)
				{
					ReplayCrimsonEvents.BitlockerEncryptableVolumeGetFailed.Log<string, string>(path, ex2.Message);
				}
				else
				{
					int num3;
					ex = BitlockerUtil.ValidateAndEncryptEmptyWin7Volume(encryptableVolume, fipsCompliant, out num3);
					if (ex == null)
					{
						ReplayCrimsonEvents.BitlockerFullVolumeEncryptionStartSucceeded.Log<string, string>(exchangeVolume.VolumeName.Path, exchangeVolume.ExchangeVolumeMountPoint.Path);
						num++;
						break;
					}
					ReplayCrimsonEvents.BitlockerFullVolumeEncryptionStartFailed.Log<string, string, string, int, string>(exchangeVolume.VolumeName.Path, exchangeVolume.ExchangeVolumeMountPoint.Path, ex.Message, num3, Util.WindowsErrorMessageLookup(num3));
					num2++;
				}
			}
			ReplayCrimsonEvents.BitlockerFullVolumeEncryptionStartReport.Log<int, int>(num, num2);
		}

		public static void EncryptEmptyWin8Volumes(VolumeManager volumeManager, bool fipsCompliant)
		{
			if (!Util.IsOperatingSystemWin8OrHigher())
			{
				ReplayCrimsonEvents.Win8EncryptionMethodCalledByWin7.Log();
				return;
			}
			List<ExchangeVolume>[] array = volumeManager.DetermineVolumeSpareStatuses();
			List<ExchangeVolume> list = array[1];
			if (list == null || list.Count == 0)
			{
				ReplayCrimsonEvents.NoEmptySpaceVolumesFoundToEncrypt.Log();
				return;
			}
			ReplayCrimsonEvents.CandidatesForEncryption.Log<int>(list.Count);
			int num = 0;
			int num2 = 0;
			foreach (ExchangeVolume exchangeVolume in list)
			{
				string path = exchangeVolume.VolumeName.Path;
				Exception ex;
				ManagementObject encryptableVolume = BitlockerUtil.GetEncryptableVolume(path, out ex);
				if (encryptableVolume == null)
				{
					ReplayCrimsonEvents.BitlockerEncryptableVolumeGetFailed.Log<string, string>(path, ex.Message);
				}
				else
				{
					int num3;
					Exception ex2 = BitlockerUtil.ValidateAndEncryptEmptyWin8Volume(encryptableVolume, fipsCompliant, out num3);
					if (ex2 != null)
					{
						ReplayCrimsonEvents.BitlockerUsedOnlyEncryptionFailed.Log<string, string, string, int, string>(exchangeVolume.VolumeName.Path, exchangeVolume.ExchangeVolumeMountPoint.Path, ex2.Message, num3, Util.WindowsErrorMessageLookup(num3));
						num2++;
					}
					else
					{
						ReplayCrimsonEvents.BitlockerUsedOnlyEncryptionSucceeded.Log<string, string>(exchangeVolume.VolumeName.Path, exchangeVolume.ExchangeVolumeMountPoint.Path);
						num++;
					}
				}
			}
			ReplayCrimsonEvents.BitlockerUsedOnlyEncryptionReport.Log<int, int>(num, num2);
		}

		public static void LogEncryptionPercentagesForEncryptingVolumes()
		{
			List<string> list = new List<string>();
			Exception ex;
			List<string> encryptionPausedVolumes = BitlockerUtil.GetEncryptionPausedVolumes(out ex);
			if (ex == null && encryptionPausedVolumes != null && encryptionPausedVolumes.Count > 0)
			{
				list.AddRange(encryptionPausedVolumes);
			}
			if (encryptionPausedVolumes != null)
			{
				foreach (string text in encryptionPausedVolumes)
				{
					if (BitlockerUtil.GetEncryptableVolume(text, out ex) == null)
					{
						ReplayCrimsonEvents.BitlockerEncryptableVolumeGetFailed.Log<string, string>(text, ex.Message);
					}
					else
					{
						string text2;
						string text3;
						bool flag = BitlockerUtil.IsEncryptionPausedDueToBadBlocks(text, out ex, out text2, out text3);
						if (flag)
						{
							ReplayCrimsonEvents.EncryptionPausedDueToBadBlocks.Log<string, string, string, string>(text, ex.Message, text2, text3);
						}
					}
				}
			}
			List<string> encryptingVolumes = BitlockerUtil.GetEncryptingVolumes(out ex);
			if (ex == null && encryptingVolumes != null && encryptingVolumes.Count > 0)
			{
				list.AddRange(encryptingVolumes);
			}
			foreach (string text4 in list)
			{
				ManagementObject encryptableVolume = BitlockerUtil.GetEncryptableVolume(text4, out ex);
				if (encryptableVolume == null)
				{
					ReplayCrimsonEvents.BitlockerEncryptableVolumeGetFailed.Log<string, string>(text4, ex.Message);
				}
				else
				{
					int num;
					int bitlockerEncryptionPercentage = BitlockerUtil.GetBitlockerEncryptionPercentage(encryptableVolume, out num, out ex);
					if (ex == null)
					{
						ReplayCrimsonEvents.BitlockerEncryptionPercentage.Log<string, int>(text4, bitlockerEncryptionPercentage);
					}
				}
			}
		}
	}
}
