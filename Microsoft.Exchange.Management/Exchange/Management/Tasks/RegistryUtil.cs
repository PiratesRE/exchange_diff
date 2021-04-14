using System;
using System.Globalization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Tasks
{
	public class RegistryUtil
	{
		public static RegistryKey OpenRemoteBaseKey(RegistryHive hive, string serverName)
		{
			string strB = Environment.MachineName;
			if (serverName != null && serverName.Contains("."))
			{
				strB = NativeHelpers.GetLocalComputerFqdn(false);
			}
			RegistryKey result;
			if (string.IsNullOrEmpty(serverName) || string.Compare(serverName, strB, true, CultureInfo.InvariantCulture) == 0)
			{
				TaskLogger.Trace("Local ServerName matches the ServerName on which the remote registry call is done.Skipping OpenRemoteBaseKey and retrieving the local registry hive instead", new object[0]);
				switch (hive)
				{
				case RegistryHive.ClassesRoot:
					return Registry.ClassesRoot;
				case RegistryHive.CurrentUser:
					return Registry.CurrentUser;
				case RegistryHive.Users:
					return Registry.Users;
				case RegistryHive.PerformanceData:
					return Registry.PerformanceData;
				case RegistryHive.CurrentConfig:
					return Registry.CurrentConfig;
				}
				result = Registry.LocalMachine;
			}
			else
			{
				result = RegistryKey.OpenRemoteBaseKey(hive, serverName);
			}
			return result;
		}
	}
}
