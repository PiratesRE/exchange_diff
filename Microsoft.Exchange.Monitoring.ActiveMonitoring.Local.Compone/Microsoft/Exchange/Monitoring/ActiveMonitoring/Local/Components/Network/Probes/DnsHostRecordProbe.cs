using System;
using System.Linq;
using System.Management;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Network.Probes
{
	public class DnsHostRecordProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			string arg = string.Empty;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Services\\Tcpip\\Parameters"))
			{
				arg = (registryKey.GetValue("Domain") as string);
			}
			string text = string.Format("{0}.{1}.", Environment.MachineName, arg);
			string[] array = null;
			string[] array2 = null;
			WindowsServerRoleEndpoint windowsServerRoleEndpoint = LocalEndpointManager.Instance.WindowsServerRoleEndpoint;
			bool flag = false;
			if (windowsServerRoleEndpoint == null)
			{
				throw new ApplicationException("WindowsServerRoleEndpoint is not valid");
			}
			flag = windowsServerRoleEndpoint.IsNatServerRoleInstalled;
			WqlObjectQuery query = new WqlObjectQuery("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = TRUE");
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(query))
			{
				using (ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get())
				{
					foreach (ManagementBaseObject managementBaseObject in managementObjectCollection)
					{
						ManagementObject managementObject = (ManagementObject)managementBaseObject;
						using (managementObject)
						{
							if ((!flag && managementObject["DefaultIPGateway"] != null) || (flag && managementObject["DefaultIPGateway"] == null))
							{
								array = (string[])managementObject["DnsServerSearchOrder"];
								array2 = (string[])managementObject["IPAddress"];
								break;
							}
						}
					}
				}
			}
			if (array == null || array.Length < 1)
			{
				throw new ApplicationException("No DNS servers found in any of the adapter configurations");
			}
			NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "Found the following DNS servers: {0}", new object[]
			{
				string.Join(", ", array)
			});
			NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "Found the following MAPI NIC IPs: {0}", new object[]
			{
				string.Join(", ", array2)
			});
			foreach (string text2 in array)
			{
				string text3 = NetworkUtils.ResolveHostARecord(text, text2);
				NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "The DNS server {0} resolved the FQDN to: [{1}]", new object[]
				{
					text2,
					text3
				});
				if (array2.Contains(text3))
				{
					return;
				}
			}
			throw new Exception(string.Format("Invalid or no DNS host A record found for this computer. FQDN: {0}", text));
		}
	}
}
