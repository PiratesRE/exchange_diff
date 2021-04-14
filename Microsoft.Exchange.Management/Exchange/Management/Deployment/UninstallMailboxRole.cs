using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("Uninstall", "MailboxRole", SupportsShouldProcess = true)]
	public sealed class UninstallMailboxRole : ManageMailboxRole
	{
		protected override LocalizedString Description
		{
			get
			{
				return Strings.UninstallMailboxRoleDescription;
			}
		}
	}
}
