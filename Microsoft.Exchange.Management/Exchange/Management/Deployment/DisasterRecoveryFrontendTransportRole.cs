using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("DisasterRecovery", "FrontendTransportRole", SupportsShouldProcess = true)]
	public sealed class DisasterRecoveryFrontendTransportRole : ManageRole
	{
		public DisasterRecoveryFrontendTransportRole()
		{
			this.StartTransportService = true;
		}

		protected override LocalizedString Description
		{
			get
			{
				return Strings.DisasterRecoveryFrontendTransportRoleDescription;
			}
		}

		[Parameter(Mandatory = false)]
		public bool StartTransportService
		{
			get
			{
				return (bool)base.Fields["StartTransportService"];
			}
			set
			{
				base.Fields["StartTransportService"] = value;
			}
		}
	}
}
