using System;
using System.Management.Automation.Runspaces;
using Microsoft.Exchange.Configuration.Authorization;

namespace Microsoft.Office.Datacenter.ActiveMonitoring.Management
{
	public class CmdletConfiguration
	{
		internal static CmdletConfigurationEntry[] CmdletConfigurationEntries
		{
			get
			{
				return CmdletConfiguration.cmdletConfigurationEntries;
			}
		}

		internal static FormatConfigurationEntry[] FormatConfigurationEntries
		{
			get
			{
				return CmdletConfiguration.formatConfigurationEntries;
			}
		}

		public static void PopulateISSCmdletConfigurationEntries()
		{
			for (int i = 0; i < CmdletDynamicParameterTypes.CmdletTypeNames.Length; i++)
			{
				InitialSessionStateBuilder.AddDynamicParameter(CmdletDynamicParameterTypes.CmdletTypeNames[i], CmdletDynamicParameterTypes.DynamicParameterTypes[i]);
			}
		}

		private static CmdletConfigurationEntry[] cmdletConfigurationEntries = new CmdletConfigurationEntry[]
		{
			new CmdletConfigurationEntry("Get-HealthReport", typeof(GetHealthReport), "Microsoft.Office.Datacenter.ActiveMonitoring.Management.dll-Help.xml"),
			new CmdletConfigurationEntry("Get-ServerHealth", typeof(GetServerHealth), "Microsoft.Office.Datacenter.ActiveMonitoring.Management.dll-Help.xml"),
			new CmdletConfigurationEntry("Invoke-MonitoringProbe", typeof(InvokeMonitoringProbe), "Microsoft.Office.Datacenter.ActiveMonitoring.Management.dll-Help.xml")
		};

		private static FormatConfigurationEntry[] formatConfigurationEntries = new FormatConfigurationEntry[]
		{
			new FormatConfigurationEntry("Microsoft.Office.Datacenter.ActiveMonitoring.Management.ps1xml")
		};
	}
}
