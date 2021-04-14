using System;
using System.Collections.Generic;
using System.Management;
using Microsoft.Exchange.Common.DiskManagement;
using Microsoft.Exchange.Common.DiskManagement.Utilities;

namespace Microsoft.Exchange.Common.Bitlocker.Utilities
{
	public static class BitlockerLockUtil
	{
		public static Exception UnlockWithNumericalPassword(ManagementObject encryptableVolume, string numericalPassword, out int returnValueUnlockWithNumericalPassword)
		{
			ManagementBaseObject outParams = null;
			int returnValue = -1;
			Exception ex = Util.HandleExceptions(delegate
			{
				string deviceId = BitlockerUtil.GetDeviceId(encryptableVolume);
				Exception ex2;
				if (!BitlockerLockUtil.IsVolumeLocked(encryptableVolume, out ex2))
				{
					returnValue = 0;
					throw new VolumeLockedFindException(deviceId, ex2.Message, ex2);
				}
				returnValue = WMIUtil.CallWMIMethod(encryptableVolume, "UnlockWithNumericalPassword", new string[]
				{
					"NumericalPassword"
				}, new object[]
				{
					numericalPassword
				}, out outParams);
			});
			returnValueUnlockWithNumericalPassword = returnValue;
			Util.AssertReturnValueExceptionInconsistency(returnValueUnlockWithNumericalPassword, "UnlockWithNumericalPassword", ex);
			return Util.ReturnWMIErrorExceptionOnExceptionOrError(returnValueUnlockWithNumericalPassword, "UnlockWithNumericalPassword", ex);
		}

		public static Exception LockDataVolume(ManagementObject encryptableVolume, out int returnValueLock)
		{
			ManagementBaseObject outParams = null;
			int returnValue = -1;
			Exception ex = Util.HandleExceptions(delegate
			{
				string deviceId = BitlockerUtil.GetDeviceId(encryptableVolume);
				Exception ex2;
				if (BitlockerLockUtil.IsVolumeLocked(encryptableVolume, out ex2))
				{
					if (ex2 != null)
					{
						throw new VolumeLockedFindException(deviceId, ex2.Message, ex2);
					}
				}
				else
				{
					returnValue = WMIUtil.CallWMIMethod(encryptableVolume, "Lock", null, null, out outParams);
					if (BitlockerLockUtil.IsVolumeLocked(encryptableVolume, out ex2) && ex2 != null)
					{
						throw new VolumeLockedFindException(deviceId, ex2.Message, ex2);
					}
				}
			});
			returnValueLock = returnValue;
			Util.AssertReturnValueExceptionInconsistency(returnValueLock, "LockDataVolume", ex);
			return Util.ReturnWMIErrorExceptionOnExceptionOrError(returnValueLock, "LockDataVolume", ex);
		}

		public static bool IsVolumeLocked(ManagementObject encryptableVolume, out Exception ex)
		{
			ManagementBaseObject outParams = null;
			bool success = false;
			ex = Util.HandleExceptions(delegate
			{
				if (WMIUtil.CallWMIMethod(encryptableVolume, "GetLockStatus", null, null, out outParams) == 0)
				{
					int num = Convert.ToInt32(outParams["LockStatus"]);
					if (Enum.IsDefined(typeof(BitlockerLockUtil.LockStatus), num))
					{
						BitlockerLockUtil.LockStatus lockStatus = (BitlockerLockUtil.LockStatus)num;
						success = (lockStatus == BitlockerLockUtil.LockStatus.Locked);
					}
				}
			});
			Util.ThrowIfNotNull(ex);
			return success;
		}

		public static List<string> GetLockedVolumes(out Exception ex)
		{
			List<string> lockedVolumes = null;
			ex = Util.HandleExceptions(delegate
			{
				ManagementObjectCollection encryptableVolumes = BitlockerUtil.GetEncryptableVolumes();
				if (encryptableVolumes != null)
				{
					foreach (ManagementBaseObject managementBaseObject in encryptableVolumes)
					{
						ManagementObject managementObject = (ManagementObject)managementBaseObject;
						string item = managementObject.GetPropertyValue("DeviceId").ToString();
						Exception ex2;
						bool flag = BitlockerLockUtil.IsVolumeLocked(managementObject, out ex2);
						Util.ThrowIfNotNull(ex2);
						if (flag)
						{
							if (lockedVolumes == null)
							{
								lockedVolumes = new List<string>();
							}
							lockedVolumes.Add(item);
						}
					}
				}
			});
			return lockedVolumes;
		}

		public enum LockStatus
		{
			Unlocked,
			Locked
		}
	}
}
