using System;

namespace Microsoft.Office.Datacenter.ActiveMonitoring.Management
{
	internal class CmdletDynamicParameterTypes
	{
		internal static string[] CmdletTypeNames
		{
			get
			{
				return new string[]
				{
					"Microsoft.Office.Datacenter.ActiveMonitoring.Management.GetHealthReport",
					"Microsoft.Office.Datacenter.ActiveMonitoring.Management.GetServerHealth",
					"Microsoft.Office.Datacenter.ActiveMonitoring.Management.InvokeMonitoringProbe"
				};
			}
		}

		internal static Type[] DynamicParameterTypes
		{
			get
			{
				return new Type[]
				{
					typeof(GetHealthReport),
					typeof(GetServerHealth),
					typeof(InvokeMonitoringProbe)
				};
			}
		}
	}
}
