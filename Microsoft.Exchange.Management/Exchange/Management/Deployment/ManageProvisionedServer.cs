using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public class ManageProvisionedServer : ComponentInfoBasedTask
	{
		public ManageProvisionedServer()
		{
			base.ImplementsResume = false;
			base.Fields["InstallationMode"] = InstallationModes.Unknown;
			base.Fields["IsProvisionServer"] = true;
			base.Fields["ServerName"] = base.GetFqdnOrNetbiosName();
			base.ComponentInfoFileNames = new List<string>();
			base.ComponentInfoFileNames.Add("setup\\data\\ProvisionServerComponent.xml");
		}

		[Parameter(Mandatory = false)]
		public string ServerName
		{
			get
			{
				return (string)base.Fields["ServerName"];
			}
			set
			{
				base.Fields["ServerName"] = value;
			}
		}

		protected override LocalizedString Description
		{
			get
			{
				return LocalizedString.Empty;
			}
		}

		protected override void PopulateContextVariables()
		{
			base.Fields["NetBIOSName"] = base.GetNetBIOSName(this.ServerName);
			base.PopulateContextVariables();
		}
	}
}
