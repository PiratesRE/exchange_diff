using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Install", "MonitoringRole", SupportsShouldProcess = true)]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class InstallMonitoringRole : ManageMonitoringRole
	{
		public InstallMonitoringRole()
		{
			base.Fields["CustomerFeedbackEnabled"] = null;
		}

		protected override LocalizedString Description
		{
			get
			{
				return Strings.InstallMonitoringRoleDescription;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? CustomerFeedbackEnabled
		{
			get
			{
				return (bool?)base.Fields["CustomerFeedbackEnabled"];
			}
			set
			{
				base.Fields["CustomerFeedbackEnabled"] = value;
			}
		}
	}
}
