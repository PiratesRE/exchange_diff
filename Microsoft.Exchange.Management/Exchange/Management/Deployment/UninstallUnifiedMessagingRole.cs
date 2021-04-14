using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("Uninstall", "UnifiedMessagingRole", SupportsShouldProcess = true)]
	public sealed class UninstallUnifiedMessagingRole : ManageUnifiedMessagingRole
	{
		protected override void PopulateContextVariables()
		{
			base.PopulateContextVariables();
			base.LogFilePath = Path.Combine((string)base.Fields["SetupLoggingPath"], "remove-UMLanguagePack.en-us.msilog");
			base.WriteVerbose(Strings.UmLanguagePackLogFile(base.LogFilePath));
		}

		protected override LocalizedString Description
		{
			get
			{
				return Strings.UninstallUnifiedMessagingRoleDescription;
			}
		}
	}
}
