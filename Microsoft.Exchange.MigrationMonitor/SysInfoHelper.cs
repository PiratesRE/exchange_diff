using System;
using System.Management;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	public static class SysInfoHelper
	{
		public static int? GetCPUCores(bool runningInMigrationMon = true)
		{
			int num = -1;
			try
			{
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("Select * from Win32_Processor"))
				{
					ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
					foreach (ManagementBaseObject managementBaseObject in managementObjectCollection)
					{
						ManagementObject managementObject = (ManagementObject)managementBaseObject;
						if (managementObject["NumberOfCores"] != null)
						{
							int.TryParse(managementObject["NumberOfCores"].ToString(), out num);
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (ex is ManagementException || ex is COMException)
				{
					if (runningInMigrationMon)
					{
						MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Cannot get CPU cores from ManagementObjectSearcher {0}", new object[]
						{
							ex.ToString()
						});
					}
					return null;
				}
				throw;
			}
			if (num == -1)
			{
				return null;
			}
			return new int?(num);
		}

		public static ByteQuantifiedSize? GetDiskSize(bool runningInMigrationMon = true)
		{
			ulong num = 0UL;
			try
			{
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("select Size from Win32_LogicalDisk where DriveType=3"))
				{
					ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
					foreach (ManagementBaseObject managementBaseObject in managementObjectCollection)
					{
						ManagementObject managementObject = (ManagementObject)managementBaseObject;
						ulong num2;
						if (managementObject["Size"] != null && ulong.TryParse(managementObject["Size"].ToString(), out num2))
						{
							num += num2;
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (ex is ManagementException || ex is COMException)
				{
					if (runningInMigrationMon)
					{
						MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Cannot get disk size from ManagementObjectSearcher {0}", new object[]
						{
							ex.ToString()
						});
					}
					return null;
				}
				throw;
			}
			if (num > 0UL)
			{
				return new ByteQuantifiedSize?(ByteQuantifiedSize.FromBytes(num));
			}
			return null;
		}
	}
}
