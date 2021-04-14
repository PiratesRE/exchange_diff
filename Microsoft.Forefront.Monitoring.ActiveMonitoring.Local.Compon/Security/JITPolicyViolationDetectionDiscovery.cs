using System;
using System.Management;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Security
{
	public sealed class JITPolicyViolationDetectionDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("InstalledRoleFeatureID list:");
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_ServerFeature WHERE ParentID = 0"))
			{
				using (ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get())
				{
					foreach (ManagementBaseObject managementBaseObject in managementObjectCollection)
					{
						ManagementObject managementObject = (ManagementObject)managementBaseObject;
						stringBuilder.Append(managementObject["ID"]);
						stringBuilder.Append("|");
						uint num = (uint)managementObject["ID"];
						if (num == 10U)
						{
							flag = true;
						}
					}
				}
			}
			MaintenanceResult result = base.Result;
			result.StateAttribute3 += stringBuilder.ToString();
			if (flag)
			{
				base.Result.StateAttribute1 = "JITPolicyViolationDetectionDiscovery: Domain Controller installed, start install probes.";
				GenericWorkItemHelper.CreateAllDefinitions(new string[]
				{
					"JITPolicyViolationDetection.xml"
				}, base.Broker, base.TraceContext, base.Result);
				return;
			}
			base.Result.StateAttribute1 = "JITPolicyViolationDetectionDiscovery: Domain Controller is not installed, ignore this server.";
		}

		private const string WmiQueryString = "SELECT * FROM Win32_ServerFeature WHERE ParentID = 0";

		private const uint DirectoryServiceFeatureId = 10U;
	}
}
