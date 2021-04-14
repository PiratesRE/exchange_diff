using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Uninstall", "MonitoringRole", SupportsShouldProcess = true)]
	public sealed class UninstallMonitoringRole : ManageMonitoringRole
	{
		protected override LocalizedString Description
		{
			get
			{
				return Strings.UninstallMonitoringRoleDescription;
			}
		}
	}
}
