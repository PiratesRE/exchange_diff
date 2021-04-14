using System;
using System.Management;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Transport
{
	internal static class TransportCommon
	{
		internal static bool IsServiceDisabledAndInactive(string serviceName, ServerComponentEnum serviceComponent)
		{
			return ServerComponentStateManager.GetEffectiveState(serviceComponent) == ServiceState.Inactive && TransportCommon.IsServiceDisabled(serviceName);
		}

		internal static bool IsServiceDisabled(string serviceName)
		{
			ObjectQuery query = new ObjectQuery(string.Format("SELECT StartMode FROM Win32_Service WHERE Name='{0}'", serviceName));
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(query))
			{
				foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
				{
					ManagementObject managementObject = (ManagementObject)managementBaseObject;
					using (managementObject)
					{
						return managementObject["StartMode"].ToString() == "Disabled";
					}
				}
				throw new ArgumentException(string.Format("No service (Name={0}) found.", serviceName));
			}
			bool result;
			return result;
		}
	}
}
