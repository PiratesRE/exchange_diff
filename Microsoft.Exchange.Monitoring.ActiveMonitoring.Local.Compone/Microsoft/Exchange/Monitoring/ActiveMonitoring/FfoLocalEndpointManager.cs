using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal class FfoLocalEndpointManager
	{
		public static bool IsForefrontForOfficeDatacenter
		{
			get
			{
				return Datacenter.IsForefrontForOfficeDatacenter();
			}
		}

		public static bool IsForefrontForOfficeGallatinDatacenter
		{
			get
			{
				return Datacenter.IsFFOGallatinDatacenter();
			}
		}

		public static bool IsBackgroundRoleInstalled
		{
			get
			{
				return FfoLocalEndpointManager.GetRole("Background");
			}
		}

		public static bool IsCentralAdminRoleInstalled
		{
			get
			{
				RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\CentralAdminRole");
				if (registryKey != null)
				{
					registryKey.Close();
					registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\PowerShell\\1\\PowerShellSnapIns\\Microsoft.Exchange.Management.Powershell.FfoCentralAdmin");
					if (registryKey != null)
					{
						registryKey.Close();
						return true;
					}
				}
				return false;
			}
		}

		public static bool IsSyslogListenerRoleInstalled
		{
			get
			{
				RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\services\\MS Forefront Networking SysLogListener Service");
				if (registryKey != null)
				{
					registryKey.Close();
					return true;
				}
				return false;
			}
		}

		public static bool IsDatabaseRoleInstalled
		{
			get
			{
				return FfoLocalEndpointManager.GetRole("Database");
			}
		}

		public static bool IsInfraDatabaseRoleInstalled
		{
			get
			{
				return FfoLocalEndpointManager.GetRole("InfraDatabase");
			}
		}

		public static bool IsDomainNameServerRoleInstalled
		{
			get
			{
				return FfoLocalEndpointManager.GetRole("DomainNameServer");
			}
		}

		public static bool IsHubTransportRoleInstalled
		{
			get
			{
				return FfoLocalEndpointManager.GetRole("HubTransport");
			}
		}

		public static bool IsFrontendTransportRoleInstalled
		{
			get
			{
				return FfoLocalEndpointManager.GetRole("FrontendTransport");
			}
		}

		public static bool IsWebServiceInstalled
		{
			get
			{
				return FfoLocalEndpointManager.GetRole("WebService");
			}
		}

		public static bool IsWebstoreInstalled
		{
			get
			{
				int num = 0;
				object value = Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\Webstore", "regMasterControllerEvents", 0);
				if (value != null && value is int)
				{
					num = (int)value;
				}
				return num == 1;
			}
		}

		internal static bool GetRole(string role)
		{
			if (string.IsNullOrWhiteSpace(role))
			{
				return false;
			}
			string text = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\ExchangeLabs", "Role", null);
			return !string.IsNullOrWhiteSpace(text) && new List<string>(text.ToLower().Split(new char[]
			{
				','
			})).Contains(role.ToLower());
		}
	}
}
