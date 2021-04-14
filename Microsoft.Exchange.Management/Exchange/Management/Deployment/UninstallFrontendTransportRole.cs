using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Uninstall", "FrontendTransportRole", SupportsShouldProcess = true)]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class UninstallFrontendTransportRole : ManageRole
	{
		protected override LocalizedString Description
		{
			get
			{
				return Strings.UninstallFrontendTransportRoleDescription;
			}
		}
	}
}
