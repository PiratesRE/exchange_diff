using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("New", "ProvisionedServer", SupportsShouldProcess = true)]
	public sealed class NewProvisionedServer : ManageProvisionedServer
	{
		public NewProvisionedServer()
		{
			base.Fields["InstallationMode"] = InstallationModes.Install;
		}

		protected override LocalizedString Description
		{
			get
			{
				return Strings.ProvisionServerDescription;
			}
		}
	}
}
