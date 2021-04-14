using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Remove", "ProvisionedServer", SupportsShouldProcess = true)]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class RemoveProvisionedServer : ManageProvisionedServer
	{
		public RemoveProvisionedServer()
		{
			base.Fields["InstallationMode"] = InstallationModes.Uninstall;
		}

		protected override LocalizedString Description
		{
			get
			{
				return Strings.RemoveProvisionedServerDescription;
			}
		}
	}
}
