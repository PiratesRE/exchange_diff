using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("Install", "CentralAdminFrontEndRole", SupportsShouldProcess = true)]
	public sealed class InstallCentralAdminFrontEndRole : ManageRole
	{
		public InstallCentralAdminFrontEndRole()
		{
			base.Fields["CustomerFeedbackEnabled"] = null;
		}

		protected override LocalizedString Description
		{
			get
			{
				return Strings.InstallCentralAdminFrontEndRoleDescription;
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
