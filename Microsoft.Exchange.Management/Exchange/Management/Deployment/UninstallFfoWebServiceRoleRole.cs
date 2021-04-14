using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Uninstall", "FfoWebServiceRole", SupportsShouldProcess = true)]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class UninstallFfoWebServiceRoleRole : ManageFfoWebServiceRole
	{
		protected override LocalizedString Description
		{
			get
			{
				return Strings.UninstallFfoWebServiceRoleDescription;
			}
		}
	}
}
