using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("DisasterRecovery", "MonitoringRole", SupportsShouldProcess = true)]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class DisasterRecoveryMonitoringRole : ManageMonitoringRole
	{
		protected override LocalizedString Description
		{
			get
			{
				return Strings.DisasterRecoveryMonitoringRoleDescription;
			}
		}
	}
}
