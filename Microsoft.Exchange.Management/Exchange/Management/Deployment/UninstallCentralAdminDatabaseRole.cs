using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Uninstall", "CentralAdminDatabaseRole", SupportsShouldProcess = true)]
	public sealed class UninstallCentralAdminDatabaseRole : ManageRole
	{
		protected override LocalizedString Description
		{
			get
			{
				return Strings.InstallCentralAdminDatabaseRoleDescription;
			}
		}
	}
}
