using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Start", "PreSetup", SupportsShouldProcess = true)]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class StartPreSetup : ManageSetupBindingTasks
	{
		protected override void PopulateContextVariables()
		{
			base.PopulateContextVariables();
			string text = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup", "MsiInstallPath", null);
			base.Fields["MsiInstallPath"] = text;
			base.Fields["MsiInstallPathBin"] = ((text == null) ? null : Path.Combine(text, "Bin"));
		}

		protected override void PopulateComponentInfoFileNames()
		{
			base.PopulateComponentInfoFileNames();
			base.ComponentInfoFileNames.Add("setup\\data\\AllRolesPreSetupLastComponent.xml");
		}

		protected override LocalizedString Description
		{
			get
			{
				return Strings.StartPreSetupDescription;
			}
		}
	}
}
