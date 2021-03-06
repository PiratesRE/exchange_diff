using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Uninstall", "CentralAdminRole", SupportsShouldProcess = true)]
	public sealed class UninstallCentralAdminRole : ManageRole
	{
		protected override LocalizedString Description
		{
			get
			{
				return Strings.InstallCentralAdminRoleDescription;
			}
		}
	}
}
